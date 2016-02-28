using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;

namespace CheckBook.App
{
    public class DotvvmStartup : IDotvvmStartup
    {
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            RegisterRoutes(config);
            RegisterMarkupControls(config);
            RegisterResources(config);
        }

        private void RegisterRoutes(DotvvmConfiguration config)
        {
            // configure a default route
            config.RouteTable.Add("default", "", "Views/login.dothtml");

            // configure routes with parameters
            config.RouteTable.Add("group", "group/{Id}", "Views/group.dothtml", new { Id = (int?)null });
            config.RouteTable.Add("payment", "payment/{GroupId}/{Id}", "Views/payment.dothtml", new { Id = (int?)null });

            // configure customer presenters
            config.RouteTable.Add("identicon", "identicon/{Identicon}", null, null, () => new IdenticonPresenter());

            // auto-discover all missing parameterless routes
            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
        }

        private void RegisterMarkupControls(DotvvmConfiguration config)
        {
            // register markup controls
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Src = "Controls/UserAvatar.dotcontrol",
                TagPrefix = "cc",
                TagName = "UserAvatar"
            });
            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                Src = "Controls/UserDetailForm.dotcontrol",
                TagPrefix = "cc",
                TagName = "UserDetailForm"
            });
        }

        private void RegisterResources(DotvvmConfiguration config)
        {
            // register custom script
            config.Resources.Register("autoHideAlert", new ScriptResource()
            {
                Url = "/Scripts/autoHideAlert.js",
                Dependencies = new[] { "jquery" }
            });

            // Note that the 'jquery' resource is registered in DotVVM and points to official jQuery CDN.
            // We have jQuery in our application, so we have to change its URL
            config.Resources.FindResource("jquery").Url = "/Scripts/jquery-2.1.3.min.js";

            // register bootstrap
            config.Resources.Register("bootstrap", new ScriptResource()
            {
                Url = "/Scripts/bootstrap.min.js",
                Dependencies = new[] { "jquery" }
            });
        }
    }
}