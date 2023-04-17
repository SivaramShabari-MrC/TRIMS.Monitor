using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace TRIMS.Monitor.API.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class TestCondtroller : Controller
    {
        [Authorize(Roles = "SecAdmin_AppManagement_NonProd")]
        [Route("/test")]
        [HttpGet]
        public IActionResult Index()
        {
            const string emailRegexPattern = "^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\\.)+[a-z]{2,6}$";

            try
            {
                var x = User.Claims;
                if (User.Claims != null && User.Claims.Any())
                {
                    foreach (Claim claim in User.Claims.ToList())
                    {
                        string claimValue = Convert.ToString(claim.Value);
                        if (!string.IsNullOrEmpty(claimValue) && Regex.IsMatch(claimValue.Trim().ToLower(), emailRegexPattern))
                        {
                            string userEmail = claimValue;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Ok("Hello");
        }
    }
}
