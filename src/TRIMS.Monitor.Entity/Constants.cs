using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TRIMS.Monitor.Entity
{
    public static class AppConfig
    {
        public const string ApplicationConfig = "ApplicationConfig";
        public const string SecurityDbContext = "SecurityContext";
        public const string AuthenticationConfig = "AuthenticationConfig";
        public const string AzureAd = "AzureAd";
    }
    public enum EnvironmentType
    {
        Development,
        QA,
        UAT,
        Production
    }

    public enum SystemType
    {
        FMS,
        BFMS
    }

    public enum FolderType
    {
        SourceFolder,
        ErrorsFolder,
        ProcessedFolder,
        DebugFolder,
        DestinationFolder
    }

    public enum FMSWindowsServiceCommand
    {
        Start,
        Stop
    }

}
