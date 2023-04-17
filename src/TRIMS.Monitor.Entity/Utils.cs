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
                    return "UnknownFolder";
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
