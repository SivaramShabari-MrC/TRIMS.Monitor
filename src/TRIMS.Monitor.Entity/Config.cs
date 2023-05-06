namespace TRIMS.Monitor.Entity
{
    public class LoggingConfig
    {
        public LogLevelConfig LogLevel { get; set; }
    }

    public class LogLevelConfig
    {
        public string Default { get; set; }
        public string Microsoft_AspNetCore { get; set; }
    }

    public class ConnectionStringsConfig
    {
        public string TRIMS { get; set; }
        public string SecurityAuditDb { get; set; }
    }

    public class ApplicationConfig
    {
        public string Contact { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Environment { get; set; }
    }

    public class AzureAdConfig
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
    }

    public class AuthenticationConfig
    {
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public string Scopes { get; set; }
        public string Description { get; set; }
        public string TRIMSMonitorUrl { get; set; }
    }

    public class FileMonitorConfig
    {
        public ConfigFilePathConfig ConfigFilePath { get; set; }
        public WindowsServiceNameConfig WindowsServiceName { get; set; }
    }

    public class ConfigFilePathConfig
    {
        public string FMS { get; set; }
        public string BFMS { get; set; }
    }

    public class WindowsServiceNameConfig
    {
        public string FMS { get; set; }
        public string BFMS { get; set; }
    }

    public class MiscrosoftGraphApiConfig
    {
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }

    public class AppSettingsConfig
    {
        public LoggingConfig Logging { get; set; }
        public string ServerName { get; set; }
        public ConnectionStringsConfig ConnectionStrings { get; set; }
        public ApplicationConfig ApplicationConfig { get; set; }
        public AzureAdConfig AzureAd { get; set; }
        public AuthenticationConfig AuthenticationConfig { get; set; }
        public FileMonitorConfig FileMonitorConfig { get; set; }
        public MiscrosoftGraphApiConfig MiscrosoftGraphApi { get; set; }
    }


}
