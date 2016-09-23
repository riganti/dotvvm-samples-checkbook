namespace CheckBook.App.Tests
{
    public class LoginHelper : DotVVM.Framework.Testing.SeleniumHelpers.SeleniumHelperBase
    {
        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.TextBoxProxy Email
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.TextBoxProxy Password
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.CheckBoxProxy RememberMe
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy ErrorMessage
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.ButtonProxy SignIn
        {
            get;
        }

        public LoginHelper(OpenQA.Selenium.IWebDriver webDriver): base (webDriver)
        {
            Email = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.TextBoxProxy(this, "Email");
            Password = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.TextBoxProxy(this, "Password");
            RememberMe = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.CheckBoxProxy(this, "RememberMe");
            ErrorMessage = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy(this, "ErrorMessage");
            SignIn = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.ButtonProxy(this, "SignIn");
        }
    }
}