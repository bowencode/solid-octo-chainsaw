﻿namespace Demo.Notes.Common.Configuration
{
    public class UserApiOptions
    {
        public string Host { get; set; }
        public IdentityServerOptions Identity { get; set; }
    }
}