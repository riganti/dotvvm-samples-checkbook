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
                browser.Navigate().GoToUrl("http://localhost:60319/");

                var loginPage = new LoginHelper(browser);

                loginPage.Email.SetText("smith@test.com");
                loginPage.Password.SetText("Pa$$w0rdxxx");
                loginPage.RememberMe.Check(true);
                loginPage.SignIn.Click();

                Thread.Sleep(2000);

                Assert.IsTrue(!string.IsNullOrEmpty(loginPage.ErrorMessage.GetText()));
            });
        }

        [TestMethod]
        public void Login_CorrectCredentials()
        {
            RunInAllBrowsers(browser =>
            {
                browser.Navigate().GoToUrl("http://localhost:60319/");
                
                var loginPage = new LoginHelper(browser);

                loginPage.Email.SetText("smith@test.com");
                loginPage.Password.SetText("Pa$$w0rd");
                loginPage.RememberMe.Check(true);
                loginPage.SignIn.Click();

                Thread.Sleep(2000);
                Assert.IsTrue(browser.Url.Contains("/home"));

                var homePage = new HomeHelper(browser);
                Assert.AreEqual(2, homePage.Groups.GetItemsCount());

                var firstItem = homePage.Groups.GetItem(0);
                Assert.IsTrue(!string.IsNullOrEmpty(firstItem.TotalSpending.GetText()));

                var secondItem = homePage.Groups.GetItem(1);
                Assert.IsTrue(!string.IsNullOrEmpty(secondItem.TotalSpending.GetText()));
            });
        }
    }

    
}
