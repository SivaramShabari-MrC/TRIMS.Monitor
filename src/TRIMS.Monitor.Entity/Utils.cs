using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRIMS.Monitor.Entity
{
    public static class Utils
    {
        public static string GetWindowsServiceName(SystemType system, AppSettingsConfig _config)
        {
            if (system == SystemType.BFMS) return _config.FileMonitorConfig.WindowsServiceName.BFMS;
            else return _config.FileMonitorConfig.WindowsServiceName.BFMS;
        }

        public static string GetFolderName(FolderType folder)
        {
            switch (folder)
            {
                case FolderType.SourceFolder:
                    return "Source";
                case FolderType.ErrorsFolder:
                    return "Errors";
                case FolderType.ProcessedFolder:
                    return "Processed";
                case FolderType.DebugFolder:
                    return "Debug";
                case FolderType.DestinationFolder:
                    return "Destination";
                default:
                    return "Unknown Folder";
            }
        }

        public static string GetSystemName(SystemType system)
        {
            switch (system)
            {
                case SystemType.FMS:
                    return "FMS";
                case SystemType.BFMS:
                    return "BFMS";
                default:
                    return "Unknown";
            }
        }

        public static string GetAuditLogActionName(ActionType action)
        {
            switch (action)
            {
                case ActionType.StartFMS:
                    return "StartFMS";
                case ActionType.StartBFMS:
                    return "StartBFMS";
                case ActionType.DownloadFile:
                    return "DownloadFile";
                case ActionType.MoveFile:
                    return "MoveFile";
                case ActionType.StopFMS:
                    return "StopFMS";
                case ActionType.StopBFMS:
                    return "StopBFMS";
                default:
                    return "Unknown";
            }
        }
       
    }
}
