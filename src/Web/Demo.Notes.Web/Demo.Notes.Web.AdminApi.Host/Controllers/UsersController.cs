using Demo.Notes.Common.Extensions;
using Demo.Notes.Web.AdminApi.Host.Model;
using Microsoft.AspNetCore.Mvc;
using Demo.Notes.Common.Model;

namespace Demo.Notes.Web.AdminApi.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUsers")]
        public IEnumerable<UserData> Get()
        {
            var currentUserId = User.GetUserId();
            _logger.LogInformation("User {userId} accessing complete user list", currentUserId);

            return TestUsers.Users.Select(u => new UserData
            {
                Id = u.SubjectId,
                Username = u.Username,
                IsActive = u.IsActive,
                Name = u.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                Email = u.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                FamilyName = u.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                GivenName = u.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
            });
        }
    }
}