using Demo.Notes.Web.AdminApi.Host.Model;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Notes.Web.AdminApi.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsernameController : ControllerBase
    {
        private readonly ILogger _logger;

        public UsernameController(ILogger<UsernameController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUsername")]
        public string? Get(string id)
        {
            return TestUsers.Users
                .Where(u => u.SubjectId == id)
                .Select(u =>  u.Username).FirstOrDefault();
        }

    }
}