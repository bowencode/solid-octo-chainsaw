using Demo.Notes.Common.Configuration;
using Demo.Notes.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Demo.Notes.Common.Extensions;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Cors;

namespace Demo.Notes.Web.UserApi.Host.Controllers
{
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly HttpClient _httpClient;
        private readonly CosmosOptions _dbOptions;

        private string? CurrentUserId => User.GetUserId();
        private static CosmosLinqSerializerOptions SerializerOptions => new CosmosLinqSerializerOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };

        public NoteController(ILogger<NoteController> logger, IHttpClientFactory httpClientFactory, IOptions<CosmosOptions> dbOptions)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("adminApiClient");
            _dbOptions = dbOptions.Value;

            GetClient();
        }

        [EnableCors("ClientSideSPA")]
        [HttpGet("api/note/")]
        public async Task<IActionResult> Get()
        {
            var container = await GetNotesContainerAsync();

            if (CurrentUserId == null)
                return BadRequest();

            var allNotes = container.GetItemLinqQueryable<NoteData>(
                    allowSynchronousQueryExecution: true, 
                    linqSerializerOptions: SerializerOptions)
                .Where(n => n.UserId == CurrentUserId)
                .ToList();

            return Ok(allNotes);
        }

        [HttpGet("api/note/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var container = await GetNotesContainerAsync();
            var note = container
                .GetItemLinqQueryable<NoteData>(
                    allowSynchronousQueryExecution: true,
                    requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(id) },
                    linqSerializerOptions: SerializerOptions)
                .Where(n => n.UserId == CurrentUserId && n.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (note == null)
            {
                return BadRequest();
            }
            
            return Ok(note);
        }

        [HttpPost("api/note/")]
        public async Task<IActionResult> Post(NoteData note)
        {
            if (string.IsNullOrWhiteSpace(note.UserId))
            {
                return BadRequest();
            }

            string username = await _httpClient.GetStringAsync($"username/{CurrentUserId}");

            note.Id = Guid.NewGuid().ToString();
            note.UserId = CurrentUserId;
            note.Username = username;
            note.Updated = DateTime.UtcNow;

            var container = await GetNotesContainerAsync();
            NoteData created = await container.CreateItemAsync(note);
            if (created == null)
            {
                return BadRequest();
            }

            return Ok(created);
        }

        [HttpPut("api/note/{id}")]
        public async Task<IActionResult> Put(string id, NoteData note)
        {
            if (string.IsNullOrWhiteSpace(note.UserId) || id != note.Id || note.UserId != CurrentUserId)
            {
                return BadRequest();
            }

            note.Updated = DateTime.UtcNow;

            var container = await GetNotesContainerAsync();
            NoteData updated = await container.UpsertItemAsync(note);
            if (updated == null)
            {
                return BadRequest();
            }

            return Ok(updated);
        }

        [HttpDelete("api/note/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var container = await GetNotesContainerAsync();
            var note = container
                .GetItemLinqQueryable<NoteData>(
                    allowSynchronousQueryExecution: true,
                    requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(id) }, 
                    linqSerializerOptions: SerializerOptions)
                .Where(n => n.UserId == CurrentUserId && n.Id == id)
                .AsEnumerable()
                .FirstOrDefault();

            if (note == null)
            {
                return NotFound();
            }

            var response = await container.DeleteItemAsync<NoteData>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return NoContent();
            }

            return NotFound();
        }

        private CosmosClient GetClient()
        {
            CosmosClient cosmosClient = new CosmosClient(_dbOptions.ConnectionString);
            return cosmosClient;
        }

        private async Task<Container> GetNotesContainerAsync()
        {
            var client = GetClient();
            Database db = await client.CreateDatabaseIfNotExistsAsync(_dbOptions.Database);
            Container container = await db.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = _dbOptions.NotesContainer,
                PartitionKeyPath = "/id",
            }, ThroughputProperties.CreateAutoscaleThroughput(4000));
            return container;
        }
    }
}