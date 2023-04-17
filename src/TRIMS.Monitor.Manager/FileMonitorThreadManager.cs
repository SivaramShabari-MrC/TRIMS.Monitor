using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Entity.API;
using TRIMS.Monitor.Repository;
using TRIMS.Monitor.Service;

namespace TRIMS.Monitor.Manager
{
    public class FileMonitorThreadManager : IFileMonitorThreadManager
    {
        private readonly IEnvironmentManager _environmentManager;
        private readonly IFileMonitorThreadService _fileMonitorThreadService;
        private readonly ILogger<FileMonitorThreadManager> _logger;
        private readonly ISecurityAuditRepository _securityAuditRepository;

        public FileMonitorThreadManager(IEnvironmentManager environmentManager, IFileMonitorThreadService monitorFileService, ISecurityAuditRepository securityAuditRepository, ILogger<FileMonitorThreadManager> logger)
        {
            _environmentManager = environmentManager;
            _fileMonitorThreadService = monitorFileService;
            _securityAuditRepository = securityAuditRepository;

        }

        public async Task<FileMonitorThread[]?> GetMonitorThreads(EnvironmentType environment, SystemType system, bool includeFiles, FolderType? folder)
        {
            string configFilePath = _environmentManager.GetConfigFilePath(environment, system);
            try
            {
                var fileMonitorThreads = await _fileMonitorThreadService.GetMonitorThreads(configFilePath, includeFiles, folder);
                return fileMonitorThreads ?? Array.Empty<FileMonitorThread>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Array.Empty<FileMonitorThread>();
            }
        }

        public async Task<ThreadFolderFiles[]> GetFilesFromThreadFolder(EnvironmentType environment, SystemType system, string[] threadNames, FolderType folder)
        {
            string configFilePath = _environmentManager.GetConfigFilePath(environment, system);
            try
            {
                var fileMonitorThreads = await _fileMonitorThreadService.GetMonitorThreads(configFilePath, false, folder);
                List<ThreadFolderFiles> result = new();
                foreach (var threadName in threadNames)
                {
                    ThreadFolderFiles threadFolderFiles = new(threadName, folder, null)
                    {
                        FolderPath = GetFolderPath(fileMonitorThreads, threadName, folder)
                    };
                    result.Add(threadFolderFiles);
                }
                var tasks = result.Select(threadFolder => Task.Run(() => GetThreadFolderFiles(threadFolder)));
                var results = await Task.WhenAll(tasks);

                return results.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error while getting files from thread folders.", ex);
            }
        }

        public async Task MoveFile(EnvironmentType env, SystemType system, string threadName, FolderType from, FolderType to, string fileName)
        {
            if (from != FolderType.ErrorsFolder && from != FolderType.SourceFolder)
                throw new Exception("Cannot move files from folders other than Error and SourceFolder");
            if (to != FolderType.DebugFolder && to != FolderType.SourceFolder)
                throw new Exception("Cannot move files to folder other then Debug and SourceFolder");
            string configFilePath = _environmentManager.GetConfigFilePath(env, system);
            var fileMonitorThreads = await _fileMonitorThreadService.GetMonitorThreads(configFilePath, false, null);
            var fromPath = GetFolderPath(fileMonitorThreads, threadName, from);
            var toPath = GetFolderPath(fileMonitorThreads, threadName, to);
            await _fileMonitorThreadService.MoveFile(fromPath, toPath, fileName);
            return;
        }

        public async Task<byte[]> DownloadFile(EnvironmentType environment, SystemType system, string threadName, FolderType folder, string fileName)
        {
            string configFilePath = _environmentManager.GetConfigFilePath(environment, system);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = ActionType.DownloadFile,
                DateTime = DateTime.Now,
                Description = $"ThreadName={threadName}; FileName:{fileName}; Folder:{Utils.GetFolderName(folder)}",
                Environment = environment,
                SystemType = system,
                ThreadName = threadName,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
            return await _fileMonitorThreadService.DownloadFile(configFilePath, threadName, folder, fileName);
        }

        public async Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus(EnvironmentType environment)
        {
            string serverName = _environmentManager.GetEnvironmentServer(environment);
            string fmsServiceName = _environmentManager.GetWindowsServiceName(SystemType.FMS);
            string bfmsServiceName = _environmentManager.GetWindowsServiceName(SystemType.BFMS);
            return await _fileMonitorThreadService.GetFMSWindowsServiceStatus(serverName, fmsServiceName, bfmsServiceName);
        }

        public async Task ExecuteWindowsServiceAction(EnvironmentType environment, SystemType systemType, FMSWindowsServiceCommand commandType)
        {
            string serverName = _environmentManager.GetEnvironmentServer(environment);
            string serviceName = systemType == SystemType.FMS ?
                _environmentManager.GetWindowsServiceName(SystemType.FMS) :
                _environmentManager.GetWindowsServiceName(SystemType.BFMS);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = systemType == SystemType.FMS ? 
                                        commandType == FMSWindowsServiceCommand.Start ? ActionType.StartFMS : ActionType.StopFMS 
                                        : commandType == FMSWindowsServiceCommand.Start ? ActionType.StartBFMS : ActionType.StopBFMS,
                DateTime = DateTime.Now,
                Description = $"",
                Environment = environment,
                SystemType = null,
                ThreadName = null,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
            await _fileMonitorThreadService.ExecuteWindowsServiceAction(serverName, serviceName, commandType);
            return;
        }

        private ThreadFolderFiles GetThreadFolderFiles(ThreadFolderFiles threadFolder)
        {
            var files = _fileMonitorThreadService.GetFilesFromFolder(threadFolder.FolderPath);
            var result = new ThreadFolderFiles(threadFolder.ThreadName, threadFolder.Folder, null)
            {
                Files = files
            };
            return result;
        }

        private string GetFolderPath(FileMonitorThread[]? fileMonitorThreads, string threadName, FolderType folder)
        {
            if (fileMonitorThreads == null) return string.Empty;
            if (!fileMonitorThreads.Any()) return string.Empty;

            string folderPath = string.Empty;
            var thread = fileMonitorThreads?.Where(thread => thread.ThreadName == threadName).FirstOrDefault();

            if (folder == FolderType.ProcessedFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].ProcessedFolder ?? thread?.Endpoint?.AddEndPoint?[1].ProcessedFolder ?? "";
            else if (folder == FolderType.ErrorsFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].ErrorsFolder ?? thread?.Endpoint?.AddEndPoint?[1].ErrorsFolder ?? "";
            else if (folder == FolderType.DestinationFolder)
                folderPath = thread?.Endpoint?.AddEndPoint?[0].DestinationFolder ?? thread?.Endpoint?.AddEndPoint?[1].DestinationFolder ?? "";
            else if (folder == FolderType.DebugFolder)
            {
                folderPath = thread?.Endpoint?.AddEndPoint?[0].SourceFolder ?? thread?.Endpoint?.AddEndPoint?[1].SourceFolder ?? "";
                if (folderPath != "") folderPath += "\\Debug";
            }
            else
                folderPath = thread?.Endpoint?.AddEndPoint?[0].SourceFolder ?? thread?.Endpoint?.AddEndPoint?[1].SourceFolder ?? "";

            return folderPath;
        }
    }
}
