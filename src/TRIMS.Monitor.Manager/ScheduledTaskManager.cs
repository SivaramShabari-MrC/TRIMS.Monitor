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
        private readonly AppSettingsConfig _config;

        public ScheduledTaskManager(AppSettingsConfig config, IScheduledTaskService scheduledTaskService)
        {
            _scheduledTaskService = scheduledTaskService;
            _config = config;
        }

        public async Task<ScheduledTask[]> GetScheduledTasks()
        {
            return await _scheduledTaskService.GetScheduledTasks(_config.ServerName);
        }
    }
}
