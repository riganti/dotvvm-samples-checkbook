using CheckBook.App.Helpers;
using CheckBook.DataAccess.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Data.Entity;
using System.Web.Hosting;
using System.Web.Http;

[assembly: OwinStartup(typeof(CheckBook.App.Startup))]

namespace CheckBook.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // set up Entity Framework Migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, DataAccess.Migrations.Configuration>());

            // configure authentication
            ConfigureCookieAuthentication(app);
            ConfigureBearerAuthentication(app);
            if (LoginHelper.AADEnabled)
            {
                ConfigureAADAuthentication(app);
            }

            // use DotVVM
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath, options: options =>
            {
                options.AddDefaultTempStorages("App_Data\\UploadTemp");
            });

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
            SwaggerConfig.Register(config);
        }
    }
}