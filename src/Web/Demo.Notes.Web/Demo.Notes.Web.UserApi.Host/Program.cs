using Demo.Notes.Common.Configuration;
using IdentityModel.Client;

namespace Demo.Notes.Web.UserApi.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            AddAuthenticatedApiAccess(builder.Services, builder.Configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void AddAuthenticatedApiAccess(IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("AdminApi").Get<AdminApiOptions>();
            var identityOptions = configuration.GetSection("Identity").Get<IdentityServerOptions>();

            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("client", new ClientCredentialsTokenRequest
                {
                    Address = $"{identityOptions.Authority.TrimEnd('/')}/connect/token",
                    ClientId = identityOptions.ClientId,
                    ClientSecret = identityOptions.ClientSecret,
                    Scope = "api"
                });
            });

            services.AddClientAccessTokenHttpClient("adminApiClient", configureClient: client =>
            {
                client.BaseAddress = new Uri(apiOptions.Host);
            });
        }
    }
}