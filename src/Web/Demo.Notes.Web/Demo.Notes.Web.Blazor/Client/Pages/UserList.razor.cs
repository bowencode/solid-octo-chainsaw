using System.Net.Http.Json;
using Demo.Notes.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Demo.Notes.Web.Blazor.Client.Pages
{
    [Authorize]
    public partial class UserList
    {
        protected override async Task OnInitializedAsync()
        {
            var response = await Http.GetAsync("api/admin/UserNames");
            
            var users = await response.Content.ReadFromJsonAsync<List<UserSummary>>();
            if (users != null)
            {
                Users = users;
            }
        }

        public List<UserSummary> Users { get; set; } = new List<UserSummary>();

        [Inject]
        public HttpClient Http { get; set; } = null!;
    }
}