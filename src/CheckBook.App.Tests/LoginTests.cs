using System;
using System.Threading;
using DotVVM.Framework.Testing.SeleniumHelpers;
using DotVVM.Framework.Testing.SeleniumHelpers.Proxies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;

namespace CheckBook.App.Tests
{
    [TestClass]
    public class LoginTest : SeleniumTest
    {
        [TestMethod]
        public void Login_IncorrectCredentials()
        {
            RunInAllBrowsers(browser =>
            {
                var loginPage = new LoginHelper(browser);
                browser.Navigate().GoToUrl("http://localhost:60319/");

                loginPage.Email.SetText("smith@test.com");
                loginPage.Password.SetText("Pa$$w0rdxxx");
                loginPage.RememberMe.Check(true);
                loginPage.SignIn.Click();

                Thread.Sleep(2000);

                Assert.IsTrue(!string.IsNullOrEmpty(loginPage.ErrorMessage.GetText()));
            });
        }
    }

    
}
