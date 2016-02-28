using System.Security.Claims;
using CheckBook.App.Models;
using CheckBook.DataAccess.Services;

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
            var claimsIdentity = new ClaimsIdentity(new UserIdentity(user.FirstName + " " + user.LastName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.UserRole.ToString()));
            return claimsIdentity;
        }
    }
}