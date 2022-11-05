using Demo.Notes.Common.Configuration;
using Microsoft.AspNetCore.ResponseCompression;

namespace Demo.Notes.Web.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            AddIdentityAuthentication(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseBff();
            app.UseAuthorization();

            app.MapBffManagementEndpoints();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }

        private static void AddIdentityAuthentication(WebApplicationBuilder builder)
        {
            var identityOptions = builder.Configuration.GetSection("Identity").Get<IdentityServerOptions>();

            builder.Services.AddBff();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                    options.DefaultSignOutScheme = "oidc";
                })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "__Host-blazor";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = identityOptions.Authority;

                    options.ClientId = identityOptions.ClientId;
                    options.ClientSecret = identityOptions.ClientSecret;
                    options.ResponseType = "code";
                    options.ResponseMode = "query";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    //options.Scope.Add("api");
                    options.Scope.Add("myApi");
                    options.Scope.Add("offline_access");

                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                });
        }
    }
}