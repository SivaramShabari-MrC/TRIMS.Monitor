using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Manager;

namespace TRIMS.Monitor.API.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class FileMonitorThreadController : Controller
    {
        private readonly ILogger<FileMonitorThreadController> _logger;
        private readonly IFileMonitorThreadManager _fileMonitorThreadManager;

        public FileMonitorThreadController(ILogger<FileMonitorThreadController> logger, IFileMonitorThreadManager fileMonitorThreadManager)
        {
            _logger = logger;
            _fileMonitorThreadManager = fileMonitorThreadManager;
        }

        [Route("/fileMonitorThreads/all")]
        [HttpGet]
        public IActionResult Get(SystemType system)
        {
            try
            {
                var res =  _fileMonitorThreadManager.GetMonitorThreads(system);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/files")]
        public async Task<IActionResult> GetFilecount(SystemType system, string threadNames, FolderType folder)
        {
            try
            {
                var res = await _fileMonitorThreadManager.GetFilesFromThreadFolder(system, threadNames.Trim().Split(",").Select(x=>x.Trim()).ToArray(), folder);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/downloadFile")]
        public async Task<IActionResult> DownloadFile(SystemType system, string threadName, FolderType folder, string fileName)
        {
            try
            {
                return Ok(await _fileMonitorThreadManager.DownloadFile(system, threadName, folder, fileName));
            }
            catch (Exception e)
            {
                return NotFound("Error : " + e.Message);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/moveFile")]
        public async Task<IActionResult> MoveFile(SystemType system, string threadName, FolderType from, FolderType to, string fileName)
        {
            try
            {
                await _fileMonitorThreadManager.MoveFile(system, threadName, from, to, fileName);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("Error : " + e.Message);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/windowsService/status")]
        public async Task<IActionResult> GetFMSWindowsServiceStatus()
        {
            return Ok(await _fileMonitorThreadManager.GetFMSWindowsServiceStatus());
        }

        [HttpGet]
        [Route("/fileMonitorThreads/windowsService/execute")]
        public async Task<IActionResult> ExecuteWindowsServiceAction(SystemType system, FMSWindowsServiceCommand command)
        {
            await _fileMonitorThreadManager.ExecuteWindowsServiceAction(system, command);
            return Ok();
        }
    }
}
