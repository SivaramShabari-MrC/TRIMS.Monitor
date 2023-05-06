using Microsoft.AspNetCore.Mvc;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Manager;

namespace TRIMS.Monitor.API.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class ScheduledTaskController : Controller
    {
        private readonly IScheduledTaskManager _scheduledTaskManager;
        public ScheduledTaskController(IScheduledTaskManager scheduledTaskManager)
        {
            _scheduledTaskManager = scheduledTaskManager;
        }

        [Route("/scheduledTasks")]
        [HttpGet]
        public async Task<IActionResult> GetScheduledTasks()
        {
            return Ok(await _scheduledTaskManager.GetScheduledTasks());
        }
    }
}
