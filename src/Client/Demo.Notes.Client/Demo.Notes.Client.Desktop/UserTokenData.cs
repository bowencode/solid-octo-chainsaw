using System;

namespace Demo.Notes.Client.Desktop
{
    public class UserTokenData
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset AccessTokenExpiration { get; set; }
    }
}
