using Microsoft.AspNetCore.Mvc;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Manager;

namespace TRIMS.Monitor.API.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class BAIFileStatusController : Controller
    {
        private readonly ILogger<BAIFileStatusController> _logger;
        private readonly IBAIFileStatusManager _BAIFileStatusManager;
        public BAIFileStatusController(ILogger<BAIFileStatusController> logger, IBAIFileStatusManager BAIFileStatusManager)
        {
            _logger = logger;
            _BAIFileStatusManager = BAIFileStatusManager;
        }
        [Route("/BAIFileStatus")]
        [HttpGet]
        public async Task<IActionResult> GetBAIFileStatus(EnvironmentType environment, DateTime date)
        {
            try
            {
                var result = await _BAIFileStatusManager.CheckFileForDate(environment, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while loading BAI file statuses."+ ex.Message);
               return StatusCode(StatusCodes.Status500InternalServerError, "Error while loading BAI file statuses.");
            }

        }
    }
}
