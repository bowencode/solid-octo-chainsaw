using Demo.Notes.Common.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Demo.Notes.Web.Host;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        ConfigureAuthentication(builder);

        builder.Services.Configure<AdminApiOptions>(builder.Configuration.GetSection("AdminApi"));
        builder.Services.Configure<UserApiOptions>(builder.Configuration.GetSection("UserApi"));

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHttpClient("userApi", (sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

            client.BaseAddress = config.GetValue<Uri>("UserApi:Host");
            var token = httpContextAccessor.HttpContext?.GetTokenAsync("access_token").GetAwaiter().GetResult();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // added to enable authentication
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";

                options.ClientId = "web-ui";
                options.ClientSecret = "1f668bf6-5ef5-4e77-ae84-28614dfc9d2d";
                options.ResponseType = "code";

                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.GetClaimsFromUserInfoEndpoint = true;

                options.SaveTokens = true;
            });
    }
}