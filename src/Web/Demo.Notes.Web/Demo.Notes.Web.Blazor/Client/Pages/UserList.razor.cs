using System.Net.Http.Json;
using Demo.Notes.Web.Blazor.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Notes.Web.Blazor.Client.Pages
{
    [Authorize]
    public partial class UserList
    {
        private WeatherForecast[]? forecasts;
        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        }
    }
}