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
        public FileMonitorThread[]? GetMonitorThreads(SystemType system);
        public Task<ThreadFolderFiles[]> GetFilesFromThreadFolder(SystemType system, string[] threadNames, FolderType folder);
        public Task<byte[]> DownloadFile(SystemType system, string threadName, FolderType folder, string fileName);
        public Task MoveFile(SystemType system, string threadName, FolderType from, FolderType to, string fileName);
        public Task<FMSWindowsServiceStatus> GetFMSWindowsServiceStatus();
        public Task ExecuteWindowsServiceAction(SystemType systemType, FMSWindowsServiceCommand commandType);
    }
}
