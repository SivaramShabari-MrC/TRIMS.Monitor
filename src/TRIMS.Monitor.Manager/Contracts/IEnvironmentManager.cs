using Microsoft.Extensions.Configuration;
using System.Globalization;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Manager
{
    public interface IEnvironmentManager
    {
        public string GetEnvironmentServer(EnvironmentType env);
        public string GetDatabaseConnectionString(EnvironmentType env);
        public string GetConfigFilePath(EnvironmentType environment, SystemType system);
        public string GetWindowsServiceName(SystemType system);
    }

}
