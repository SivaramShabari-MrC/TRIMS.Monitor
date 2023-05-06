using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Service;

namespace TRIMS.Monitor.Manager
{
    public interface IScheduledTaskManager
    {
        public Task<ScheduledTask[]> GetScheduledTasks();
    }
}
