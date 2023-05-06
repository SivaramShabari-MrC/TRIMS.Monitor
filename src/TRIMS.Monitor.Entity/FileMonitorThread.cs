using Newtonsoft.Json;

namespace TRIMS.Monitor.Entity
{
    
    public class FileMonitorThread
    {
        [JsonProperty("@key")]
        public string? ThreadName { get; set; }

        [JsonProperty("@autostart")]
        public bool? AutoStart { get; set; }

        [JsonProperty("@sendEmailOnError")]
        public bool? SendEmailOnError { get; set; }

        [JsonProperty("@errorEmailRecipients")]
        public string? ErrorEmailRecipients { get; set; }

        [JsonProperty("endPoints")]
        public Endpoints? Endpoint { get; set; }
        //public FileDetail[]? Files { get; set; }

    }
    
    public class Endpoints
    {
        [JsonProperty("addEndPoint")]
        public MonitorEndPoint[]? AddEndPoint { get; set; }
    }
    public class MonitorEndPoint
    {
        [JsonProperty("@key")]
        public string? Name { get; set; }

        [JsonProperty("@type")]
        public string? Type { get; set; }

        [JsonProperty("@sourceFolder")]
        public string? SourceFolder { get; set; }

        [JsonProperty("@processedFolder")]
        public string? ProcessedFolder { get; set; }

        [JsonProperty("@errorsFolder")]
        public string? ErrorsFolder { get; set; }

        [JsonProperty("@destinationFolder")]
        public string? DestinationFolder { get; set; }
        public string? DebugFolder { get; set; }

        [JsonProperty("@className")]
        public string? ClassName { get; set; }

        [JsonProperty("@SystemId")]
        public string? SystemId { get; set; }

        [JsonProperty("@SourceId")]
        public string? SourceId { get; set; }

        [JsonProperty("@assemblyPath")]
        public string? AssemblyPath { get; set; }

    }
    public class ConfigFile
    {
        [JsonProperty("configuration")]
        public Configuration Configuration { get; set; }

    }
    public class Configuration
    {
        [JsonProperty("fileMonitorSection")]
        public FileMonitorSection? FileMonitorSection { get; set; }
    }
    public class FileMonitorSection
    {
        [JsonProperty("monitors")]
        public Monitors? Monitors { get; set; }
    }
    public class Monitors
    {
        [JsonProperty("addMonitor")]
        public FileMonitorThread[]? AddMonitor { get; set; }
    }
}
