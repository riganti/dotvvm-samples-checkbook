using System.Configuration;
using System.Security.Claims;
using CheckBook.App.Models;
using CheckBook.DataAccess.Services;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace CheckBook.App.Helpers
{
    public static class LoginHelper
    {
        public static ClaimsIdentity GetClaimsIdentity(string email, string password)
        {
            // try to find the user
            var user = UserService.GetUserWithPassword(email);
            if (user == null)
            {
                return null;
            }

            // verify the password
            if (!DataAccess.Security.PasswordHelper.VerifyHashedPassword(user.PasswordHash, user.PasswordSalt, password))
            {
                return null;
            }

            // build the user identity
            var claimsIdentity = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            }, CookieAuthenticationDefaults.AuthenticationType);
            
            return claimsIdentity;
        }

        public static ClaimsIdentity GetClaimsIdentityForAzure(string email)
        {
            // try to find the user
            var user = UserService.GetUserWithPassword(email);
            if (user == null)
            {
                return null;
            }

            // build the user identity
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            return claimsIdentity;
        }

        public static bool AADEnabled => !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ida:ClientId"]);
    }
}