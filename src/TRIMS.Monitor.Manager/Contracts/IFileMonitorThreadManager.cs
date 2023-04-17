using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Entity.API;

namespace TRIMS.Monitor.Manager
{
    public interface IFileMonitorThreadManager
    {
        public Task<FileMonitorThread[]?> GetMonitorThreads(EnvironmentType environment, SystemType system, bool includeFiles, FolderType? folder);
        public Task<ThreadFolderFiles[]> GetFilesFromThreadFolder(EnvironmentType environment, SystemType system, string[] threadNames, FolderType folder);
        public Task<byte[]> DownloadFile(EnvironmentType environment, SystemType system, string threadName, FolderType folder, string fileName);
        public Task MoveFile(EnvironmentType env, SystemType system, string threadName, FolderType from, FolderType to, string fileName);
        public Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus(EnvironmentType environment);
        public Task ExecuteWindowsServiceAction(EnvironmentType environment, SystemType systemType, FMSWindowsServiceCommand commandType);
    }
}
