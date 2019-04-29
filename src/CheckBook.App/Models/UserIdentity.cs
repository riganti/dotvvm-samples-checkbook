using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CheckBook.App.Models
{
    public class UserIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { return CookieAuthenticationDefaults.AuthenticationScheme; }
        }

        public bool IsAuthenticated { get; set; }

        public string Name { get; private set; }

        public UserIdentity(string name)
        {
            Name = name;
        }
    }
}