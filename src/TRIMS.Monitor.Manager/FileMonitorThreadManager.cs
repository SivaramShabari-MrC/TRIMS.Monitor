using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly IFileMonitorThreadService _fileMonitorThreadService;
        private readonly ILogger<FileMonitorThreadManager> _logger;
        private readonly ISecurityAuditRepository _securityAuditRepository;

        public FileMonitorThreadManager(IConfiguration config, IFileMonitorThreadService monitorFileService, ISecurityAuditRepository securityAuditRepository, ILogger<FileMonitorThreadManager> logger)
        {
            _config = config;
            _fileMonitorThreadService = monitorFileService;
            _securityAuditRepository = securityAuditRepository;

        }

        public async Task<FileMonitorThread[]?> GetMonitorThreads(EnvironmentType environment, SystemType system, bool includeFiles, FolderType? folder)
        {
            string configFilePath = Utils.GetConfigFilePath(environment, system, _config);
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
            string configFilePath = Utils.GetConfigFilePath(environment, system, _config);
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

        public async Task MoveFile(EnvironmentType environment, SystemType system, string threadName, FolderType from, FolderType to, string fileName)
        {
            if (from != FolderType.ErrorsFolder && from != FolderType.SourceFolder)
                throw new Exception("Cannot move files from folders other than Error and SourceFolder");
            if (to != FolderType.DebugFolder && to != FolderType.SourceFolder)
                throw new Exception("Cannot move files to folder other then Debug and SourceFolder");
            string configFilePath = Utils.GetConfigFilePath(environment, system, _config);
            var fileMonitorThreads = await _fileMonitorThreadService.GetMonitorThreads(configFilePath, false, null);
            var fromPath = GetFolderPath(fileMonitorThreads, threadName, from);
            var toPath = GetFolderPath(fileMonitorThreads, threadName, to);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(ActionType.MoveFile),
                DateTime = DateTime.Now,
                Description = $"FileName:{fileName}; FromFolder:{Utils.GetFolderName(from)}; ToFolder:{Utils.GetFolderName(to)}",
                Environment = Utils.GetEnvironmentName(environment),
                SystemType = Utils.GetSystemName(system),
                ThreadName = threadName,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
            await _fileMonitorThreadService.MoveFile(fromPath, toPath, fileName);
            return;
        }

        public async Task<byte[]> DownloadFile(EnvironmentType environment, SystemType system, string threadName, FolderType folder, string fileName)
        {
            string configFilePath = Utils.GetConfigFilePath(environment, system, _config);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(ActionType.DownloadFile),
                DateTime = DateTime.Now,
                Description = $"FileName:{fileName}; Folder:{Utils.GetFolderName(folder)}",
                Environment = Utils.GetEnvironmentName(environment),
                SystemType = Utils.GetSystemName(system),
                ThreadName = threadName,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
            return await _fileMonitorThreadService.DownloadFile(configFilePath, threadName, folder, fileName);
        }

        public async Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus(EnvironmentType environment)
        {
            string serverName = Utils.GetEnvironmentServer(environment, _config);
            string fmsServiceName = Utils.GetWindowsServiceName(SystemType.FMS, _config);
            string bfmsServiceName = Utils.GetWindowsServiceName(SystemType.BFMS, _config);
            return await _fileMonitorThreadService.GetFMSWindowsServiceStatus(serverName, fmsServiceName, bfmsServiceName);
        }

        public async Task ExecuteWindowsServiceAction(EnvironmentType environment, SystemType systemType, FMSWindowsServiceCommand commandType)
        {
            string serverName = Utils.GetEnvironmentServer(environment, _config);
            string serviceName = systemType == SystemType.FMS ?
                Utils.GetWindowsServiceName(SystemType.FMS, _config) :
                Utils.GetWindowsServiceName(SystemType.BFMS, _config);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(systemType == SystemType.FMS ?
                                        commandType == FMSWindowsServiceCommand.Start ? ActionType.StartFMS : ActionType.StopFMS
                                        : commandType == FMSWindowsServiceCommand.Start ? ActionType.StartBFMS : ActionType.StopBFMS),
                DateTime = DateTime.Now,
                Description = $"",
                Environment = Utils.GetEnvironmentName(environment),
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
