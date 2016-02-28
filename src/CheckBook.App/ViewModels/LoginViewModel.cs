using System.ComponentModel.DataAnnotations;
using CheckBook.App.Helpers;
using DotVVM.Framework.ViewModel;

namespace CheckBook.App.ViewModels
{
    public class LoginViewModel : DotvvmViewModelBase
    {

        [Required(ErrorMessage = "The e-mail address is required!")]
        [EmailAddress(ErrorMessage = "The e-mail address is not valid!")]
        public string Email { get; set; }


        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }
        

        // The user cannot change this field in the browser so there is no point in transferring it from the client to the server
        [Bind(Direction.ServerToClient)]
        public string ErrorMessage { get; set; }



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
                Context.OwinContext.Authentication.SignIn(identity);

                // redirect to the home page
                Context.Redirect("home");
            }
        }
        
    }
}