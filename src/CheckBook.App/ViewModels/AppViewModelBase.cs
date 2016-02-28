using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNet.Identity;
using CheckBook.DataAccess.Enums;

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
        public bool IsAdmin => Context.OwinContext.Authentication.User.IsInRole(UserRole.Admin.ToString());

        /// <summary>
        /// Gets or sets the active page. This is used in the top menu bar to highlight the current menu item.
        /// </summary>
        public virtual string ActivePage => Context.Route.RouteName;

        /// <summary>
        /// Gets the ID of currently logged user.
        /// </summary>
        protected int GetUserId()
        {
            return int.Parse(Context.OwinContext.Authentication.User.Identity.GetUserId());
        }

        public void SignOut()
        {
            // sign out
            Context.OwinContext.Authentication.SignOut();

            // redirect to the login route
            Context.Redirect("login", null);
        }
    }
}

