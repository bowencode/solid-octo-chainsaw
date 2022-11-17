using Demo.Notes.Common.Configuration;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Demo.Notes.Web.UserApi.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            AddAuthenticatedApiAccess(builder.Services, builder.Configuration);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.Authority = "https://localhost:5001";
                    opt.Audience = "https://localhost:5001/resources";
                    opt.MapInboundClaims = false;
                    opt.RequireHttpsMetadata = true;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        RequireAudience = true,
                    };
                });

            builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "ClientSideSPA",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:3000");
                        policy.WithHeaders(HeaderNames.Authorization);
                    });
            });

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

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AddAuthenticatedApiAccess(IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("AdminApi").Get<AdminApiOptions>();

            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("client", new ClientCredentialsTokenRequest
                {
                    Address = $"{apiOptions.Identity.Authority.TrimEnd('/')}/connect/token",
                    ClientId = apiOptions.Identity.ClientId,
                    ClientSecret = apiOptions.Identity.ClientSecret,
                    Scope = string.Join(" ", apiOptions.Identity.Scopes),
                });
            });

            services.AddClientAccessTokenHttpClient("adminApiClient", configureClient: client =>
            {
                client.BaseAddress = new Uri(apiOptions.Host);
            });
        }
    }
}