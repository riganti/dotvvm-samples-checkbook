using DotVVM.Contrib;
using DotVVM.Contrib.TypeAhead;
using DotVVM.Framework.Compilation;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CheckBook.App
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);

            config.AddContribTypeAheadConfiguration();

            config.Markup.ImportedNamespaces.Add(new NamespaceImport("CheckBook.DataAccess.Enums"));
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            // configure a default route
            config.RouteTable.Add("default", "", "Views/login.dothtml");

            // configure routes with parameters
            config.RouteTable.Add("group", "group/{Id}", "Views/group.dothtml", new { Id = (int?)null });
            config.RouteTable.Add("payment", "payment/{GroupId}/{Id}", "Views/payment.dothtml", new { Id = (int?)null });

            // configure customer presenters
            config.RouteTable.Add("identicon", "identicon/{Identicon}", typeof(IdenticonPresenter));

            // auto-discover all missing parameterless routes
            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
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

            config.Markup.Controls.Add(new DotvvmControlConfiguration()
            {
                TagPrefix = "cc",
                Namespace = "CheckBook.App.Controls",
                Assembly = "CheckBook.App"
            });
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom scripts
            config.Resources.Register("autoHideAlert", new ScriptResource()
            {
                Location = new FileResourceLocation("wwwroot/Scripts/autoHideAlert.js"),
                Dependencies = new[] { "jquery" }
            });
            config.Resources.Register("preserveTextBoxFocus", new ScriptResource()
            {
                Location = new FileResourceLocation("wwwroot/Scripts/preserveTextBoxFocus.js"),
                Dependencies = new[] { "dotvvm", "jquery" }
            });
            config.Resources.Register("ExpressionTextBox", new ScriptResource()
            {
                Location = new FileResourceLocation("wwwroot/Scripts/ExpressionTextBox.js"),
                Dependencies = new[] { "dotvvm", "jquery" }
            });

            // Note that the 'jquery' resource is registered in DotVVM and points to official jQuery CDN.
            // We have jQuery in our application, so we have to change its URL
            config.Resources.Register("jquery", new ScriptResource()
            {
                Location = new FileResourceLocation("wwwroot/Scripts/jquery-2.1.3.min.js")
            });

            // register bootstrap
            config.Resources.Register("bootstrap", new ScriptResource()
            {
                Location = new FileResourceLocation("wwwroot/Scripts/bootstrap.min.js"),
                Dependencies = new[] { "jquery" }
            });
        }
        public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.AddDefaultTempStorages("temp");
        }
    }
}
