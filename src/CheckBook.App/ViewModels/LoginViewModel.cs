using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using CheckBook.App.Helpers;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace CheckBook.App.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {
        private readonly LoginHelper loginHelper;

        [Required(ErrorMessage = "The e-mail address is required!")]
        [EmailAddress(ErrorMessage = "The e-mail address is not valid!")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }


        public bool AADEnabled => loginHelper.AADEnabled;



        // The user cannot change this field in the browser so there is no point in transferring it from the client to the server
        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }


        public LoginViewModel(LoginHelper loginHelper)
        {
            this.loginHelper = loginHelper;
        }

        public override Task Init()
        {
            if (!Context.IsPostBack && Context.HttpContext.User.Identity.IsAuthenticated)
            {
                // redirect to the home page if the user is already authenticated
                Context.RedirectToRoute("home");
            }

            return base.Init();
        }


        public async Task SignIn()
        {
            var identity = loginHelper.GetClaimsIdentity(Email, Password);
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
                await Context.GetAuthentication().SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

                // redirect to the home page
                Context.RedirectToRoute("home");
            }
        }
        
        public async Task SignInAAD()
        {
            await Context.GetAuthentication().ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = "/home",
                IsPersistent = RememberMe,
                ExpiresUtc = RememberMe ? DateTime.UtcNow.AddMonths(1) : (DateTime?)null,
            });
            Context.InterruptRequest();
        }
    }
}