using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;
using Microsoft.AspNet.Identity;
using CheckBook.DataAccess.Enums;
using DotVVM.Framework.Hosting;

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

        public void SignOut()
        {
            // sign out
            Context.GetAuthentication().SignOut();

            // redirect to the login route
            Context.RedirectToRoute("login");
        }
    }
}

