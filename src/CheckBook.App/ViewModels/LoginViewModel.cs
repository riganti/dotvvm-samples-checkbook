using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CheckBook.App.Helpers;
using DotVVM.Framework.ViewModel;
using Microsoft.Owin.Security;
using DotVVM.Framework.Hosting;
using Microsoft.Owin.Security.OpenIdConnect;

namespace CheckBook.App.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {

        [Required(ErrorMessage = "The e-mail address is required!")]
        [EmailAddress(ErrorMessage = "The e-mail address is not valid!")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }


        public bool AADEnabled => LoginHelper.AADEnabled;



        // The user cannot change this field in the browser so there is no point in transferring it from the client to the server
        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }


        public override Task Init()
        {
            if (!Context.IsPostBack && Context.GetAuthentication().User.Identity.IsAuthenticated)
            {
                // redirect to the home page if the user is already authenticated
                Context.RedirectToRoute("home");
            }

            return base.Init();
        }


        public void SignIn()
        {
            var identity = LoginHelper.GetClaimsIdentity(Email, Password);
            if (identity == null)
            {
                ErrorMessage = "Invalid e-mail address or password!";
            }
            else
            {
                // issue an authentication cookie
                var properties = new AuthenticationProperties()
                {
                    IsPersistent = RememberMe,
                    ExpiresUtc = RememberMe ? DateTime.UtcNow.AddMonths(1) : (DateTime?)null
                };
                Context.GetAuthentication().SignIn(properties, identity);

                // redirect to the home page
                Context.RedirectToRoute("home");
            }
        }
        
        public void SignInAAD()
        {
            Context.GetAuthentication().Challenge(new AuthenticationProperties()
            {
                RedirectUri = "/home",
                IsPersistent = RememberMe,
                ExpiresUtc = RememberMe ? DateTime.UtcNow.AddMonths(1) : (DateTime?)null,
            }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            Context.InterruptRequest();
        }
    }
}