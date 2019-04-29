using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using CheckBook.DataAccess.Enums;
using DotVVM.Framework.Hosting;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CheckBook.App.ViewModels
{
    /// <summary>
    /// A base viewmodel. The viewmodels of all pages inherit from this class.
    /// This viewmodel is also referenced in the app.dotmaster page.
    /// </summary>
	public abstract class AppViewModelBase : DotvvmViewModelBase
    {

        /// <summary>
        /// Determines whether the user is administrator.
        /// </summary>
        public bool IsAdmin => Context.HttpContext.User.IsInRole(UserRole.Admin.ToString());

        /// <summary>
        /// Gets or sets the active page. This is used in the top menu bar to highlight the current menu item.
        /// </summary>
        public virtual string ActivePage => Context.Route.RouteName;

        /// <summary>
        /// Gets the ID of currently logged user.
        /// </summary>
        protected int GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)Context.HttpContext.User.Identity;
            return int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        /// <summary>
        /// Gets the Name of currently logged user.
        /// </summary>
        protected string GetUserName()
        {
            return Context.HttpContext.User.Identity.Name;
        }

        public async Task SignOut()
        {
            // sign out
            var identity = (ClaimsIdentity)Context.HttpContext.User.Identity;
            if (identity.FindFirst(ClaimTypes.AuthenticationMethod).Value == OpenIdConnectDefaults.AuthenticationScheme)
            {
                await Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await Context.GetAuthentication().SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                Context.InterruptRequest();
            }
            else
            {
                await Context.GetAuthentication().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Context.RedirectToRoute("login");
            }
        }
    }
}

