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
        public static string GetEnvironmentServer(EnvironmentType env, IConfiguration _config)
        {
            var ServerName = env switch
            {
                EnvironmentType.Development => _config["EnvironmentServers:Development"],
                EnvironmentType.QA => _config["EnvironmentServers:QA"],
                EnvironmentType.UAT => _config["EnvironmentServers:UAT"],
                EnvironmentType.Production => _config["EnvironmentServers:Production"],
                _ => _config["EnvironmentServers:Development"],
            };
            return ServerName;
        }

        public static string GetConfigFilePath(EnvironmentType environment, SystemType system, IConfiguration _config)
        {
            string configFilePath;
            string FMS_CONFIG_FILE = _config["FileMonitorConfig:ConfigFilePath:FMS"]!;
            string BFMS_CONFIG_FILE = _config["FileMonitorConfig:ConfigFilePath:BFMS"]!;
            if (system == SystemType.FMS)
            {
                configFilePath = environment switch
                {
                    EnvironmentType.Development => @$"\\{_config["EnvironmentServers:Development"]}\{FMS_CONFIG_FILE}",
                    EnvironmentType.QA => @$"\\{_config["EnvironmentServers:QA"]}\{FMS_CONFIG_FILE}",
                    EnvironmentType.UAT => @$"\\{_config["EnvironmentServers:UAT"]}\{FMS_CONFIG_FILE}",
                    EnvironmentType.Production => @$"\\{_config["EnvironmentServers:Production"]}\{FMS_CONFIG_FILE}",
                    _ => @$"\\{_config["EnvironmentServers:Development"]}\{FMS_CONFIG_FILE}",
                };
            }
            else
            {
                configFilePath = environment switch
                {
                    EnvironmentType.Development => @$"\\{_config["EnvironmentServers:Development"]}\{BFMS_CONFIG_FILE}",
                    EnvironmentType.QA => @$"\\{_config["EnvironmentServers:QA"]}\{BFMS_CONFIG_FILE}",
                    EnvironmentType.UAT => @$"\\{_config["EnvironmentServers:UAT"]}\{BFMS_CONFIG_FILE}",
                    EnvironmentType.Production => @$"\\{_config["EnvironmentServers:Production2"]}\{BFMS_CONFIG_FILE}",
                    _ => @$"\\{_config["EnvironmentServers:Development"]}\{BFMS_CONFIG_FILE}",
                };
            }
            return configFilePath;
        }

        public static string GetWindowsServiceName(SystemType system, IConfiguration _config)
        {
            if (system == SystemType.BFMS) return _config["FileMonitorConfig:WindowsServiceName:BFMS"]!;
            else return _config["FileMonitorConfig:WindowsServiceName:FMS"]!;
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

        public static string GetEnvironmentName(EnvironmentType env)
        {
            switch (env)
            {
                case EnvironmentType.Development:
                    return "Development";
                case EnvironmentType.QA:
                    return "QA";
                case EnvironmentType.UAT:
                    return "UAT";
                case EnvironmentType.Production:
                    return "Production";
                default:
                    return "Unknown";
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

        private static string GetTRIMSDbConnectionString(EnvironmentType env, IConfiguration _config)
        {
            var DbConnectionString = env switch
            {
                EnvironmentType.Development => _config["SqlConnectionString:TRIMS:Development"],
                EnvironmentType.QA => _config["SqlConnectionString:TRIMS:QA"],
                EnvironmentType.UAT => _config["SqlConnectionString:TRIMS:UAT"],
                EnvironmentType.Production => _config["SqlConnectionString:TRIMS:Production"],
                _ => _config["SqlConnectionString:TRIMS:Development"],
            };
            return DbConnectionString;
        }

        public static IDbConnection GetTRIMSDbConnection(EnvironmentType environment, IConfiguration _config)
        {
            return new SqlConnection(GetTRIMSDbConnectionString(environment, _config));
        }

        private static string GetAuditLogDbConnectionString(EnvironmentType env, IConfiguration _config)
        {
            var DbConnectionString = env switch
            {
                EnvironmentType.Development => _config["SqlConnectionString:SecurityAudit:Development"],
                EnvironmentType.QA => _config["SqlConnectionString:SecurityAudit:QA"],
                EnvironmentType.UAT => _config["SqlConnectionString:SecurityAudit:UAT"],
                EnvironmentType.Production => _config["SqlConnectionString:SecurityAudit:Production"],
                _ => _config["SqlConnectionString:SecurityAudit:Development"],
            };
            return DbConnectionString;
        }

        public static IDbConnection GetAuditLogDbConnection(EnvironmentType environment, IConfiguration _config)
        {
            return new SqlConnection(GetAuditLogDbConnectionString(environment, _config));
        }
    }
}
