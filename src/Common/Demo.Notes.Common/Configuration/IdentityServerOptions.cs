using System.Collections.Generic;

namespace Demo.Notes.Common.Configuration
{
    public class IdentityServerOptions
    {
        public string Authority { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public List<string> Scopes { get; set; } = new List<string>();
    }
}