using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNet.Identity;
using CheckBook.DataAccess.Enums;
using DotVVM.Framework.Hosting;
using Microsoft.Owin.Security.Cookies;
using System.Linq;
using System.Security.Claims;
using Microsoft.Owin.Security.OpenIdConnect;

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
        public bool IsAdmin => Context.GetAuthentication().User.IsInRole(UserRole.Admin.ToString());

        /// <summary>
        /// Gets or sets the active page. This is used in the top menu bar to highlight the current menu item.
        /// </summary>
        public virtual string ActivePage => Context.Route.RouteName;

        /// <summary>
        /// Gets the ID of currently logged user.
        /// </summary>
        protected int GetUserId()
        {
            return int.Parse(Context.GetAuthentication().User.Identity.GetUserId());
        }
        /// <summary>
        /// Gets the Name of currently logged user.
        /// </summary>
        protected string GetUserName()
        {
            return Context.GetAuthentication().User.Identity.Name;
        }

        public void SignOut()
        {
            // sign out
            var identity = (ClaimsIdentity)Context.HttpContext.User.Identity;
            if (identity.FindFirstValue(ClaimTypes.AuthenticationMethod) == OpenIdConnectAuthenticationDefaults.AuthenticationType)
            {
                Context.GetAuthentication().SignOut(CookieAuthenticationDefaults.AuthenticationType, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                Context.InterruptRequest();
            }
            else
            {
                Context.GetAuthentication().SignOut(CookieAuthenticationDefaults.AuthenticationType);
                Context.RedirectToRoute("login");
            }
        }
    }
}

