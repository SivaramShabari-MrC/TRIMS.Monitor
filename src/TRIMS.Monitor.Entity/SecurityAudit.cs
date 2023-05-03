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
        public string Action { get; set; }
        public string Environment { get; set; }
        public string? SystemType { get; set; }
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
