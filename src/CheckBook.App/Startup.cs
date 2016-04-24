using System;
using System.Data.Entity;
using System.IO;
using System.Web.Hosting;
using CheckBook.DataAccess.Context;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Storage;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(CheckBook.App.Startup))]
namespace CheckBook.App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // set up Entity Framework Migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext, DataAccess.Migrations.Configuration>());

            // use cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = context =>
                    {
                        DotvvmAuthenticationHelper.ApplyRedirectResponse(context.OwinContext, context.RedirectUri);
                    }
                }
            });

            // use DotVVM
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath);
            ConfigureDotvvmServices(dotvvmConfiguration);


            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });
        }

        private void ConfigureDotvvmServices(DotvvmConfiguration config)
        {
            // add support for file uploads
            var uploadPath = Path.Combine(config.ApplicationPhysicalPath, "App_Data\\UploadTemp");
            config.ServiceLocator.RegisterSingleton<IUploadedFileStorage>(() => new FileSystemUploadedFileStorage(uploadPath, TimeSpan.FromMinutes(30)));
        }
    }
}
