using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckBook.App.Helpers
{
    public class LoginOptions
    {

        public bool AADEnabled { get; set; }

        public string ClientId { get; set; }

        public string AadInstance { get; set; }

        public string Tenant { get; set; }

        public string PostLogoutRedirectUri { get; set; }

    }
}