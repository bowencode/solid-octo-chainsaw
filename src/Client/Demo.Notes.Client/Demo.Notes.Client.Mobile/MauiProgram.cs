using Demo.Notes.Common.Configuration;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Options;

namespace Demo.Notes.Client.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // add main page
            builder.Services.AddSingleton<MainPage>();

            builder.Services.Configure<IdentityServerOptions>(options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "mobile-user-ui";
                options.Scopes = new List<string>
                {
                    "openid",
                    "profile",
                    "thirdParty",
                    "read:notes",
                    "list:notes",
                    "write:notes",
                };
            });
            const string callbackUrl = "notesmobiledemo://callback";

            builder.Services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<IdentityServerOptions>>().Value;

                return new OidcClient(new()
                {
                    Authority = options.Authority,

                    ClientId = options.ClientId,
                    Scope = options.GetScope(),
                    RedirectUri = callbackUrl,

                    Browser = new MauiAuthenticationBrowser(callbackUrl)
                });
            });

            return builder.Build();
        }
    }
}