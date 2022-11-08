using Demo.Notes.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Demo.Notes.Web.Host.Pages
{
    public class UserListModel : PageModel
    {
        private readonly AdminApiOptions _options;

        public UserListModel(IOptions<AdminApiOptions> options)
        {
            _options = options.Value;
        }

        public async Task OnGet()
        {
            var tokenClient = new HttpClient
            {
                BaseAddress = new Uri(_options.Identity.Authority)
            };
            var formValues = new Dictionary<string, string>
            {
                { "client_id", _options.Identity.ClientId },
                { "client_secret", _options.Identity.ClientSecret },
                { "grant_type", "client_credentials" },
            };
            foreach (string scope in _options.Identity.Scopes)
            {
                formValues.Add("scope", scope);
            }
            var response = await tokenClient.PostAsync("connect/token", new FormUrlEncodedContent(formValues));
            if (!response.IsSuccessStatusCode)
            {
                // show error
            }

            await response.Content.ReadFromJsonAsync();

            var dataClient = new HttpClient();

        }
    }
}
