using Demo.Notes.Common.Configuration;
using Demo.Notes.Common.Extensions;
using Demo.Notes.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Demo.Notes.Web.AdminApi.Host.Controllers
{
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> _logger;
        private readonly CosmosOptions _options;

        public NotesController(ILogger<NotesController> logger, IOptions<CosmosOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            GetClient();
        }

        [HttpGet("api/notes/")]
        public async Task<IActionResult> Get()
        {
            var container = await GetNotesContainerAsync();

            var allNotes = container
                .GetItemLinqQueryable<NoteData>()
                .ToList();

            return Ok(allNotes);
        }

        [HttpGet("api/notes/{userId}/")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var container = await GetNotesContainerAsync();

            var allNotes = container.GetItemLinqQueryable<NoteData>()
                .Where(n => n.UserId == userId)
                .ToList();
            return Ok(allNotes);
        }

        private CosmosClient GetClient()
        {
            CosmosClient cosmosClient = new CosmosClient(_options.ConnectionString);
            return cosmosClient;
        }

        private async Task<Container> GetNotesContainerAsync()
        {
            var client = GetClient();
            Database db = await client.CreateDatabaseIfNotExistsAsync(_options.Database);
            Container container = await db.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = _options.NotesContainer,
                PartitionKeyPath = "/id",
            }, ThroughputProperties.CreateAutoscaleThroughput(4000));
            return container;
        }
    }
}