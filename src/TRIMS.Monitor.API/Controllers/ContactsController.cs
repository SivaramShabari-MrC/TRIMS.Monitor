using Microsoft.AspNetCore.Mvc;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Service;

namespace TRIMS.Monitor.API.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : Controller
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly IConfiguration _config;
        private readonly IContactApiService _service;

        public ContactsController(ILogger<ContactsController> logger, IConfiguration config, IContactApiService service)
        {
            _logger = logger;
            _config = config;
            _service = service;
        }

        [HttpGet]
        [Route("/contacts")]
        public async Task<IActionResult> GetContacts(string emailIds)
        {
            try
            {
                GraphApiBatchResponseContacts contacts = await _service.GetContacts(emailIds.Split(","));
                GraphApiBatchResponsePhotos photos = await _service.GetPhotos(emailIds.Split(","));
                List<Contact> response = new();
                foreach (var contact in contacts.Responses)
                    response.Add(contact.Body!);
                return Ok(response);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet]
        [Route("/contact/photo/{email}")]
        public async Task<IActionResult> GetPhoto([FromRoute] string email)
        {
            try
            {

                string photo = await _service.GetPhoto(email);
                return Ok(photo);
            }
            catch (Exception ex)
            {
                _logger.LogError("",ex);
                return NotFound();
            }
        }
    }
}
