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
        public async Task<IActionResult> Get(EnvironmentType environment, SystemType system, bool includeFiles, FolderType folder)
        {
            try
            {
                var res = await _fileMonitorThreadManager.GetMonitorThreads(environment, system, includeFiles, folder);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/files")]
        public async Task<IActionResult> GetFilecount(EnvironmentType environment, SystemType system, string threadNames, FolderType folder)
        {
            try
            {
                var res = await _fileMonitorThreadManager.GetFilesFromThreadFolder(environment, system, threadNames.Trim().Split(",").Select(x=>x.Trim()).ToArray(), folder);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/downloadFile")]
        public async Task<IActionResult> DownloadFile(EnvironmentType environment, SystemType system, string threadName, FolderType folder, string fileName)
        {
            try
            {
                return Ok(await _fileMonitorThreadManager.DownloadFile(environment, system, threadName, folder, fileName));
            }
            catch (Exception e)
            {
                return NotFound("Error : " + e.Message);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/moveFile")]
        public async Task<IActionResult> MoveFile(EnvironmentType environment, SystemType system, string threadName, FolderType from, FolderType to, string fileName)
        {
            try
            {
                await _fileMonitorThreadManager.MoveFile(environment, system, threadName, from, to, fileName);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("Error : " + e.Message);
            }
        }

        [HttpGet]
        [Route("/fileMonitorThreads/windowsService/status")]
        public async Task<IActionResult> GetFMSWindowsServiceStatus(EnvironmentType environment)
        {
            return Ok(await _fileMonitorThreadManager.GetFMSWindowsServiceStatus(environment));
        }

        [HttpGet]
        [Route("/fileMonitorThreads/windowsService/execute")]
        public async Task<IActionResult> ExecuteWindowsServiceAction(EnvironmentType environment, SystemType system, FMSWindowsServiceCommand command)
        {
            await _fileMonitorThreadManager.ExecuteWindowsServiceAction(environment, system, command);
            return Ok();
        }
    }
}
