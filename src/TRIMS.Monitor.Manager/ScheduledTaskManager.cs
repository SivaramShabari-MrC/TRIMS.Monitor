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
    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly IScheduledTaskService _scheduledTaskService;
        private readonly IConfiguration _config;

        public ScheduledTaskManager(IConfiguration config, IScheduledTaskService scheduledTaskService)
        {
            _scheduledTaskService = scheduledTaskService;
            _config = config;
        }

        public async Task<ScheduledTask[]> GetScheduledTasks(EnvironmentType environment)
        {
            return await _scheduledTaskService.GetScheduledTasks(Utils.GetEnvironmentServer(environment, _config));
        }
    }
}
