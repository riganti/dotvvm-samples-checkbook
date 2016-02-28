using Microsoft.AspNet.Identity;
using System.Security.Principal;

namespace CheckBook.App.Models
{
    public class UserIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { return DefaultAuthenticationTypes.ApplicationCookie; }
        }

        public bool IsAuthenticated { get; set; }

        public string Name { get; private set; }

        public UserIdentity(string name)
        {
            Name = name;
        }
    }
}