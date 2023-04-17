namespace TRIMS.Monitor.Entity
{
    public class ScheduledTask
    {
        public string RunspaceId { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskPath { get; set; } = string.Empty;
        public string Triggers { get; set; } = string.Empty;
    }

    public class ScheduledTaskQuery
    {
        public EnvironmentType Environment { get; set; }
    }


}
