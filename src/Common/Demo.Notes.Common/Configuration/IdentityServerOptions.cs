namespace Demo.Notes.Common.Configuration
{
    public class IdentityServerOptions
    {
        public string Authority { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;

    }
}