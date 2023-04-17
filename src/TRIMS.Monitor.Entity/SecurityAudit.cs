using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRIMS.Monitor.Entity
{
    public class SecurityAudit
    {
        public SecurityAudit() { }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } 
        public ActionType Action { get; set; }
        public EnvironmentType Environment { get; set; }
        public SystemType? SystemType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ThreadName { get; set; }

    }
    public enum ActionType
    {
        StartFMS,
        StartBFMS,
        StopFMS,
        StopBFMS,
        MoveFile,
        DownloadFile,
    }
}
