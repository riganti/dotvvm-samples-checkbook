namespace CheckBook.App.Tests
{
    public class HomeHelper : DotVVM.Framework.Testing.SeleniumHelpers.SeleniumHelperBase
    {
        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.RepeaterProxy<GroupsRepeaterHelper> Groups
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy MyBalance
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy MySpending
        {
            get;
        }

        public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy TotalAmount
        {
            get;
        }

        public HomeHelper(OpenQA.Selenium.IWebDriver webDriver, DotVVM.Framework.Testing.SeleniumHelpers.SeleniumHelperBase parentHelper = null, System.String selectorPrefix = ""): base (webDriver, parentHelper, selectorPrefix)
        {
            Groups = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.RepeaterProxy<GroupsRepeaterHelper>(this, "Groups");
            MyBalance = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy(this, "MyBalance");
            MySpending = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy(this, "MySpending");
            TotalAmount = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy(this, "TotalAmount");
        }

        public class GroupsRepeaterHelper : DotVVM.Framework.Testing.SeleniumHelpers.SeleniumHelperBase
        {
            public DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy TotalSpending
            {
                get;
            }

            public GroupsRepeaterHelper(OpenQA.Selenium.IWebDriver webDriver, DotVVM.Framework.Testing.SeleniumHelpers.SeleniumHelperBase parentHelper = null, System.String selectorPrefix = ""): base (webDriver, parentHelper, selectorPrefix)
            {
                TotalSpending = new DotVVM.Framework.Testing.SeleniumHelpers.Proxies.LiteralProxy(this, "TotalSpending");
            }
        }
    }
}