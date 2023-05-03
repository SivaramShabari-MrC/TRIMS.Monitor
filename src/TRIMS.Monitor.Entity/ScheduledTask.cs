using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace TRIMS.Monitor.Entity
{
    public class ScheduledTask
    {
        public string TaskName { get; set; } = string.Empty;
        
        public string Author { get; set; } = string.Empty;
        
        public List<string> Triggers { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime Date { get; set; }

        [JsonProperty("@State")]
        public int StateCode { get; set; }

        public string State { get; set; } = string.Empty;
        
        public void GetStateString()
        {
            if (this.StateCode == 0) this.State = "Unknown";
            else if (this.StateCode == 1) this.State = "Disabled";
            else if (this.StateCode == 2) this.State = "Queued";
            else if (this.StateCode == 3) this.State = "Ready";
            else if  (this.StateCode == 4) this.State = "Running";
            else this.State = "Unknown";
        }
    }



}
