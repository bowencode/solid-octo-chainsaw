namespace Demo.Notes.Common.Configuration
{
    public class AdminApiOptions
    {
        public string Host { get; set; }
        public IdentityServerOptions Identity { get; set; }
    }
}