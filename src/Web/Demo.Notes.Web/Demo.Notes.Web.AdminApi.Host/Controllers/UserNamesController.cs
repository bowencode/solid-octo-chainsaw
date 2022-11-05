using Demo.Notes.Common.Extensions;
using Demo.Notes.Web.AdminApi.Host.Model;
using Microsoft.AspNetCore.Mvc;
using Demo.Notes.Common.Model;

namespace Demo.Notes.Web.AdminApi.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserNamesController : ControllerBase
    {
        private readonly ILogger _logger;

        public UserNamesController(ILogger<UserNamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetUserNames")]
        public IEnumerable<UserSummary> Get()
        {
            var currentUserId = User.GetUserId();
            _logger.LogInformation("User {userId} accessing user name list", currentUserId);

            return TestUsers.Users.Where(u => u.IsActive).Select(u => new UserSummary
            {
                Id = u.SubjectId,
                Username = u.Username,
            });
        }

    }
}