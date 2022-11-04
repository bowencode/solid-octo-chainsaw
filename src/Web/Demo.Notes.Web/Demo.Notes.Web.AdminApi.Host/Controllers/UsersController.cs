using Demo.Notes.Web.AdminApi.Host.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
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
            string binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dataFilePath = Path.Combine(binDirectory, "TestUsers.json");
            string json = System.IO.File.ReadAllText(dataFilePath);
            var testUsers = JsonSerializer.Deserialize<List<AppUser>>(json);
            return testUsers.Select(u => new UserData
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