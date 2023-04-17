using Microsoft.AspNetCore.Mvc;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Manager;

namespace TRIMS.Monitor.API.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionManager _transactionManager;
        public TransactionController(ILogger<TransactionController> logger, ITransactionManager transactionManager)
        {
            _logger = logger;
            _transactionManager = transactionManager;
        }

        [Route("/transactionReport")]
        [HttpGet]
        public async Task<IActionResult> GetTransactionReport(EnvironmentType environment, DateTime from, DateTime to)
        {
            try
            {
                var result = await _transactionManager.GetTransactionReport(environment, from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting transaction repoert", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
