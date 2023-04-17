namespace TRIMS.Monitor.Entity
{
    public class ApplicationConfig
    {
        public string? Contact { get; set; }
        public string? ContactEmail { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public string? Organization { get; set; }
        public string? Title { get; set; }
        public string? Version { get; set; }
        public string? Environment { get; set; }
    }
    public class AuthenticationConfig
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string Scopes { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TRIMSMonitorClientUrl { get; set; } = string.Empty;

    }
    public class AzureAd
    {
        public string ClientId { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string Authority => $"{Instance}{TenantId}/v2.0/";
    }

    public class FileMonitorConfig
    {
        ConfigFilePath ConfigFilePath { get; set; }
        WindowsServiceName WindowsServiceName { get; set; }
    }

    public class ConfigFilePath {
        public string FMS { get; set; } = string.Empty;
        public string BFMS { get; set; } = string.Empty;
    }

    public class WindowsServiceName
    {
        public string FMS { get; set; } = string.Empty;
        public string BFMS { get; set; } = string.Empty;
    }

}
