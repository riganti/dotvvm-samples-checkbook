using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System.Security.Principal;

namespace CheckBook.App.Models
{
    public class UserIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { return CookieAuthenticationDefaults.AuthenticationType; }
        }

        public bool IsAuthenticated { get; set; }

        public string Name { get; private set; }

        public UserIdentity(string name)
        {
            Name = name;
        }
    }
}