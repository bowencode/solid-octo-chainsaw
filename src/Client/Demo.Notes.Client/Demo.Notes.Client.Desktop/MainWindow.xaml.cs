using IdentityModel.OidcClient;
using System;
using System.Text;
using System.Windows;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityModel.OidcClient.Results;

namespace Demo.Notes.Client.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ClientSecret = "1cf6de3f-0b06-4457-a114-3a7c00658878";
        private static readonly byte[] EntropyData = Encoding.UTF8.GetBytes(ClientSecret);

        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private static async Task SaveUserData(UserTokenData userData)
        {
            var json = JsonSerializer.Serialize(userData);
            var keys = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), EntropyData, DataProtectionScope.CurrentUser);
            string filePath = UserDataFilePath;
            string? folder = Path.GetDirectoryName(filePath);
            if (folder != null && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            await File.WriteAllBytesAsync(filePath, keys);
        }

        private static async Task<UserTokenData?> LoadUserData()
        {
            try
            {
                var keys = await File.ReadAllBytesAsync(UserDataFilePath);
                var data = ProtectedData.Unprotect(keys, EntropyData, DataProtectionScope.CurrentUser);
                var json = Encoding.UTF8.GetString(data);
                return JsonSerializer.Deserialize<UserTokenData>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string UserDataFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NotesAdmin\\userData.dat");

        private async void WindowRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var options = new OidcClientOptions
            {
                Authority = "https://localhost:5001",
                ClientId = "desktop-admin-ui",
                ClientSecret = ClientSecret,
                Scope = "openid profile email offline_access read:user-details",
                RedirectUri = "http://127.0.0.1/wpf-notes-admin-app",
                Browser = new WpfEmbeddedBrowser(),
                Policy = new Policy
                {
                    RequireIdentityTokenSignature = false,
                }
            };

            var oidcClient = new OidcClient(options);

            var savedLogin = await LoadUserData();
            if (savedLogin?.RefreshToken != null)
            {
                var refreshTokenResult = await oidcClient.RefreshTokenAsync(savedLogin.RefreshToken);

                if (!refreshTokenResult.IsError)
                {
                    await SetRefreshUser(oidcClient, refreshTokenResult);

                    await ViewModel.LoadData();

                    return;
                }
            }

            LoginResult loginResult;
            try
            {
                loginResult = await oidcClient.LoginAsync();
            }
            catch (Exception exception)
            {
                ViewModel.ErrorMessage = $"Unexpected Error: {exception.Message}";
                return;
            }

            if (loginResult.IsError)
            {
                ViewModel.ErrorMessage = loginResult.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : loginResult.Error;
                return;
            }

            await SetLoggedInUser(loginResult);

            await ViewModel.LoadData();
        }

        private async Task SetLoggedInUser(LoginResult loginResult)
        {
            ViewModel.ErrorMessage = null;
            ViewModel.User = loginResult.User;

            var userData = new UserTokenData
            {
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                AccessTokenExpiration = loginResult.AccessTokenExpiration,
            };

            ViewModel.Tokens = userData;

            await SaveUserData(userData);
        }

        private async Task SetRefreshUser(OidcClient oidcClient, RefreshTokenResult refreshTokenResult)
        {
            var userData = new UserTokenData
            {
                AccessToken = refreshTokenResult.AccessToken,
                RefreshToken = refreshTokenResult.RefreshToken,
                AccessTokenExpiration = refreshTokenResult.AccessTokenExpiration,
            };

            ViewModel.Tokens = userData;

            await SaveUserData(userData);

            var userInfoResult = await oidcClient.GetUserInfoAsync(userData.AccessToken);

            ViewModel.ErrorMessage = null;
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userInfoResult.Claims, "idp", "name", null));
            ViewModel.User = claimsPrincipal;
        }
    }
}
