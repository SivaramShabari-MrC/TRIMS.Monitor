using Microsoft.Extensions.Logging;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Entity.API;
using TRIMS.Monitor.Repository;
using TRIMS.Monitor.Service;

namespace TRIMS.Monitor.Manager
{
    public class FileMonitorThreadManager : IFileMonitorThreadManager
    {
        private readonly AppSettingsConfig _config;
        private readonly IFileMonitorThreadService _fileMonitorThreadService;
        private readonly ISecurityAuditRepository _securityAuditRepository;
        private readonly ILogger _logger;
        public FileMonitorThreadManager(AppSettingsConfig config, IFileMonitorThreadService monitorFileService, ISecurityAuditRepository securityAuditRepository, ILogger<FileMonitorThreadManager> logger)
        {
            _config = config;
            _fileMonitorThreadService = monitorFileService;
            _securityAuditRepository = securityAuditRepository;
            _logger = logger;
        }

        public FileMonitorThread[]? GetMonitorThreads(SystemType system, FolderType? folder)
        {
            string configFilePath = system == SystemType.FMS ? _config.FileMonitorConfig.ConfigFilePath.FMS : _config.FileMonitorConfig.ConfigFilePath.BFMS;
            try
            {
                var fileMonitorThreads =  _fileMonitorThreadService.GetMonitorThreads(configFilePath, folder);
                return fileMonitorThreads ?? Array.Empty<FileMonitorThread>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Array.Empty<FileMonitorThread>();
            }
        }

        public async Task<ThreadFolderFiles[]> GetFilesFromThreadFolder(SystemType system, string[] threadNames, FolderType folder)
        {
            string configFilePath = system == SystemType.FMS ? _config.FileMonitorConfig.ConfigFilePath.FMS : _config.FileMonitorConfig.ConfigFilePath.BFMS;

            try
            {
                var fileMonitorThreads = _fileMonitorThreadService.GetMonitorThreads(configFilePath, folder);
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

        public async Task MoveFile(SystemType system, string threadName, FolderType from, FolderType to, string fileName)
        {
            if (from == FolderType.ProcessedFolder || from == FolderType.DestinationFolder)
                throw new Exception("Cannot move files from ProcessedFolder or DestinationFolder");
            if (to != FolderType.DebugFolder && to != FolderType.SourceFolder)
                throw new Exception("Cannot move files to folder other then Debug and SourceFolder");
            string configFilePath = system == SystemType.FMS ? _config.FileMonitorConfig.ConfigFilePath.FMS : _config.FileMonitorConfig.ConfigFilePath.BFMS;
            var fileMonitorThreads =  _fileMonitorThreadService.GetMonitorThreads(configFilePath, null);
            var fromPath = GetFolderPath(fileMonitorThreads, threadName, from);
            var toPath = GetFolderPath(fileMonitorThreads, threadName, to);
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(ActionType.MoveFile),
                DateTime = DateTime.Now,
                Description = $"FileName:{fileName}; FromFolder:{Utils.GetFolderName(from)}; ToFolder:{Utils.GetFolderName(to)}",
                Environment = "",
                SystemType = Utils.GetSystemName(system),
                ThreadName = threadName,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
             _fileMonitorThreadService.MoveFile(fromPath, toPath, fileName);
            return;
        }

        public async Task<byte[]> DownloadFile(SystemType system, string threadName, FolderType folder, string fileName)
        {
            string configFilePath = system == SystemType.FMS ? _config.FileMonitorConfig.ConfigFilePath.FMS : _config.FileMonitorConfig.ConfigFilePath.BFMS;
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(ActionType.DownloadFile),
                DateTime = DateTime.Now,
                Description = $"FileName:{fileName}; Folder:{Utils.GetFolderName(folder)}",
                Environment = "",
                SystemType = Utils.GetSystemName(system),
                ThreadName = threadName,
                UserEmail = "Sivaram.ShabariA@mrcooper.com"
            });
            return _fileMonitorThreadService.DownloadFile(configFilePath, threadName, folder, fileName);
        }

        public async Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus()
        {
            string serverName = _config.ServerName;
            string fmsServiceName = _config.FileMonitorConfig.WindowsServiceName.FMS;
            string bfmsServiceName = _config.FileMonitorConfig.WindowsServiceName.BFMS;
            return await _fileMonitorThreadService.GetFMSWindowsServiceStatus(serverName, fmsServiceName, bfmsServiceName);
        }

        public async Task ExecuteWindowsServiceAction(SystemType systemType, FMSWindowsServiceCommand commandType)
        {
            string serverName = _config.ServerName;
            string serviceName = systemType == SystemType.FMS ?
                _config.FileMonitorConfig.WindowsServiceName.FMS :
                _config.FileMonitorConfig.WindowsServiceName.BFMS;
            await _securityAuditRepository.LogAudit(new SecurityAudit()
            {
                Action = Utils.GetAuditLogActionName(systemType == SystemType.FMS ?
                                        commandType == FMSWindowsServiceCommand.Start ? ActionType.StartFMS : ActionType.StopFMS
                                        : commandType == FMSWindowsServiceCommand.Start ? ActionType.StartBFMS : ActionType.StopBFMS),
                DateTime = DateTime.Now,
                Description = $"",
                Environment = "",
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
