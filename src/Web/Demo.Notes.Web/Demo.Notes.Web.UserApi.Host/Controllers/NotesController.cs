using Demo.Notes.Common.Configuration;
using Demo.Notes.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Demo.Notes.Web.UserApi.Host.Controllers
{
    [ApiController]
    [Authorize("ListNotes")]
    public class NotesController : ControllerBase
    {
        private readonly ILogger<NotesController> _logger;
        private readonly CosmosOptions _options;
        private static CosmosLinqSerializerOptions SerializerOptions => new CosmosLinqSerializerOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };

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
                .GetItemLinqQueryable<NoteData>(allowSynchronousQueryExecution: true)
                .ToList();

            return Ok(allNotes);
        }

        [HttpGet("api/notes/{userId}/")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var container = await GetNotesContainerAsync();

            var allNotes = container.GetItemLinqQueryable<NoteData>(
                    allowSynchronousQueryExecution: true,
                    linqSerializerOptions: SerializerOptions)
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