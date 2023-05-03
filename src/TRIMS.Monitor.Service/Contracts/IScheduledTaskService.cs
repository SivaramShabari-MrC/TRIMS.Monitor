using Newtonsoft.Json;
using System.Diagnostics;
using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Service
{
    public interface IScheduledTaskService
    {
        public Task<ScheduledTask[]> GetScheduledTasks(string serverName);
    }
}
