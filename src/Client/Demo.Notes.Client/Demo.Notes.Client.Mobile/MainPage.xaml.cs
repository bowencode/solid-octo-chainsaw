using System.Security.Claims;
using System.Text;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using System.Text.Json;
using System.Net.Http.Json;
using Demo.Notes.Common.Model;
using System.Collections.ObjectModel;

namespace Demo.Notes.Client.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly OidcClient _client;
        private ClaimsPrincipal _currentUser;
        private string _accessToken;

        public MainPage(OidcClient client)
        {
            InitializeComponent();
            _client = client;
            
            BindingContext = this;
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            CurrentUser = null;
            AccessToken = null;
            NotesList.Clear();
            CalendarList.Clear();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var result = await _client.LoginAsync();

            if (result.IsError)
            {
                ErrorMessage.Text = result.Error;
                return;
            }

            CurrentUser = result.User;
            AccessToken = result.AccessToken;

            if (AccessToken != null)
            {
                var client = new HttpClient();
                client.SetBearerToken(AccessToken);

                var response = await client.GetAsync("https://localhost:7274/api/note/");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<List<NoteData>>();
                    NotesList.Clear();
                    foreach (var item in content)
                    {
                        NotesList.Add(item);
                    }
                }
                else
                {
                    ErrorMessage.Text = response.ReasonPhrase;
                }
            }

            var thirdPartyToken = CurrentUser.FindFirst("externalAccessToken")?.Value;
            if (thirdPartyToken != null)
            {
                var client = new HttpClient();
                client.SetBearerToken(thirdPartyToken);

                var response = await client.GetAsync("https://localhost:7217/calendar/");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadFromJsonAsync<List<CalendarEvent>>();
                    CalendarList.Clear();
                    foreach (var item in content)
                    {
                        CalendarList.Add(item);
                    }
                }
                else
                {
                    ErrorMessage.Text = response.ReasonPhrase;
                }
            }
        }

        public ObservableCollection<NoteData> NotesList { get; } = new ObservableCollection<NoteData>();
        public ObservableCollection<CalendarEvent> CalendarList { get; } = new ObservableCollection<CalendarEvent>();

        public string AccessToken
        {
            get => _accessToken;
            set
            {
                if (value == _accessToken) return;
                _accessToken = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ClaimsData));
            }
        }

        public ClaimsPrincipal CurrentUser
        {
            get => _currentUser;
            set
            {
                if (Equals(value, _currentUser))
                    return;
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsLoggedOut));
                OnPropertyChanged(nameof(ClaimsData));
            }
        }

        public bool IsLoggedIn => CurrentUser != null;
        public bool IsLoggedOut => CurrentUser == null;

        public string ClaimsData
        {
            get
            {
                string userClaims = String.Join('\n', CurrentUser?.Claims.Select(claim => $"{claim.Type}: {claim.Value}") ?? new List<string>());
                return $"{userClaims}\n\nAccess Token:\n{AccessToken}";
            }
        }
    }
}