using Microsoft.Extensions.Configuration;
using System.Globalization;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Manager
{

    public class EnvironmentManager : IEnvironmentManager
    {
        private readonly IConfiguration _config;
        public EnvironmentManager(IConfiguration config)
        {
            _config = config;
        }
        public string GetEnvironmentServer(EnvironmentType env)
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

        public string GetDatabaseConnectionString(EnvironmentType env)
        {
            var DbConnectionString = env switch
            {
                EnvironmentType.Development => _config["SqlConnectionString:Development"],
                EnvironmentType.QA => _config["SqlConnectionString:QA"],
                EnvironmentType.UAT => _config["SqlConnectionString:UAT"],
                EnvironmentType.Production => _config["SqlConnectionString:Production"],
                _ => _config["SqlConnectionString:Development"],
            };
            return DbConnectionString;
        }

        public string GetConfigFilePath(EnvironmentType environment, SystemType system)
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

        public string GetWindowsServiceName(SystemType system)
        {
            if(system==SystemType.BFMS) return _config["FileMonitorConfig:WindowsServiceName:BFMS"]!;
            else return _config["FileMonitorConfig:WindowsServiceName:FMS"]!;
        }

    }
}
