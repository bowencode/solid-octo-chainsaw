using Demo.Notes.Web.AdminApi.Host.Model;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;

namespace Demo.Notes.Web.AdminApi.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<AppUser> Get()
        {
            string binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dataFilePath = Path.Combine(binDirectory, "TestUsers.json");
            string json = System.IO.File.ReadAllText(dataFilePath);
            var testUsers = JsonSerializer.Deserialize<List<AppUser>>(json);
            return testUsers;
        }
    }
}