using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.Notes.Common.Model;
using IdentityModel.Client;

namespace Demo.Notes.Client.Desktop
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ClaimsPrincipal? _user;
        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        public ClaimsPrincipal? User
        {
            get => _user;
            set
            {
                SetField(ref _user, value);
                NotifyOfPropertyChange(nameof(IsLoggedIn));
            }
        }

        public ObservableCollection<ExtendedUserData> AllUsers { get; } = new ObservableCollection<ExtendedUserData>();

        public bool IsLoggedIn => User != null;
        public UserTokenData? Tokens { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyOfPropertyChange([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion

        public async Task LoadData()
        {
            var adminClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7299")
            };
            var userClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7274")
            };
            if (Tokens?.AccessToken != null)
            {
                adminClient.SetBearerToken(Tokens.AccessToken);
                userClient.SetBearerToken(Tokens.AccessToken);
            }

            var response = await adminClient.GetAsync("Users");
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"{response.StatusCode}: {response.ReasonPhrase}";
                return;
            }

            var users = await response.Content.ReadFromJsonAsync<List<ExtendedUserData>>();
            AllUsers.Clear();

            var allNotes = await userClient.GetFromJsonAsync<List<NoteData>>("api/notes");

            if (users != null)
            {
                foreach (ExtendedUserData userData in users)
                {
                    userData.NoteCount = allNotes?.Count(n => n.UserId == userData.Id) ?? -1;
                    AllUsers.Add(userData);
                }
            }
        }
    }
}
