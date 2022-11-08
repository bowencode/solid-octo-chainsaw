namespace Demo.Notes.Common.Configuration
{
    public class ExternalApiOptions
    {
        public string Host { get; set; }
        public IdentityServerOptions Identity { get; set; }
    }
}