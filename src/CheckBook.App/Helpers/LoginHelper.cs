using System.Configuration;
using System.Security.Claims;
using CheckBook.App.Models;
using CheckBook.DataAccess.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace CheckBook.App.Helpers
{
    public class LoginHelper
    {
        private readonly IOptions<LoginOptions> loginOptions;
        private readonly UserService userService;

        public LoginHelper(IOptions<LoginOptions> loginOptions, UserService userService)
        {
            this.loginOptions = loginOptions;
            this.userService = userService;
        }

        public ClaimsIdentity GetClaimsIdentity(string email, string password)
        {
            // try to find the user
            var user = userService.GetUserWithPassword(email);
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
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, CookieAuthenticationDefaults.AuthenticationScheme)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            
            return claimsIdentity;
        }

        public ClaimsIdentity GetClaimsIdentityForAzure(string email)
        {
            // try to find the user
            var user = userService.GetUserWithPassword(email);
            if (user == null)
            {
                return null;
            }

            // build the user identity
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, OpenIdConnectDefaults.AuthenticationScheme)
            }, OpenIdConnectDefaults.AuthenticationScheme);
            return claimsIdentity;
        }

        public bool AADEnabled => loginOptions.Value.AADEnabled;
    }
}