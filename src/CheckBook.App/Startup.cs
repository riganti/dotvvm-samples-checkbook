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
using Microsoft.Extensions.DependencyInjection;

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
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(applicationPhysicalPath, options: options =>
            {
                options.AddDefaultTempStorages("App_Data\\UploadTemp");
            });

            // use static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileSystem = new PhysicalFileSystem(applicationPhysicalPath)
            });
        }
        
    }
}
