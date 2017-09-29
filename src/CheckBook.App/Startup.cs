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
using Microsoft.Owin.Security.OpenIdConnect;
using System.IdentityModel.Tokens;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using CheckBook.App.Helpers;
using CheckBook.App.Models;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using CheckBook.DataAccess.Services;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using Microsoft.Owin.Security;
using System.Linq;

[assembly: OwinStartup(typeof(CheckBook.App.Startup))]
namespace CheckBook.App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // set up Entity Framework Migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext, DataAccess.Migrations.Configuration>());

            // configure authentication
            ConfigureCookieAuthentication(app);
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
        }

        private static void ConfigureCookieAuthentication(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/"),
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                Provider = new CookieAuthenticationProvider()
                {
                    OnApplyRedirect = context => { DotvvmAuthenticationHelper.ApplyRedirectResponse(context.OwinContext, context.RedirectUri); }
                }
            });
        }

        private static void ConfigureAADAuthentication(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = $"https://login.microsoftonline.com/{ConfigurationManager.AppSettings["ida:TenantId"]}/",
                ClientId = ConfigurationManager.AppSettings["ida:ClientId"],
                AuthenticationMode = AuthenticationMode.Passive,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = (ConfigurationManager.AppSettings["ida:TenantId"] != "common")
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = context =>
                    {
                        // determines the base URL of the application (useful when the app can run on multiple domains)
                        var appBaseUrl = GetApplicationBaseUrl(context.Request);

                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                        {
                            context.ProtocolMessage.RedirectUri = appBaseUrl;
                            // we need to handle the redirect to the login page ourselves because redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateAuthenticationRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int) HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }
                        else if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            // we need to handle the redirect to the logout page ourselves because redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateLogoutRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int) HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = context =>
                    {
                        var isMultiTenant = ConfigurationManager.AppSettings["ida:TenantId"] == "common";
                        if (isMultiTenant)
                        {
                            // validate allowed tenants
                            var tenants = ConfigurationManager.AppSettings["ida:Tenants"].Split(',');
                            var tokenTenant = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.TenantId);
                            if (!tenants.Contains(tokenTenant))
                            {
                                throw new SecurityTokenValidationException($"Tenant {tokenTenant} is not allowed to sign in to the application!");
                            }
                        }

                        // create user if it doesn't exists
                        var upn = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.Upn);
                        var user = LoginHelper.GetClaimsIdentityForAzure(upn);
                        if (user == null)
                        {
                            var newUser = new UserInfoData
                            {
                                Email = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.Upn),
                                FirstName = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.GivenName),
                                LastName = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.Surname),
                                Name = context.AuthenticationTicket.Identity.FindFirstValue(AzureAdClaimTypes.DisplayName),
                                UserRole = UserRole.User,
                                Password = new Guid().ToString()
                            };
                            UserService.CreateOrUpdateUserInfo(newUser);

                            // create identity for the new user
                            user = LoginHelper.GetClaimsIdentityForAzure(upn);
                            context.AuthenticationTicket = new AuthenticationTicket(user, context.AuthenticationTicket.Properties);
                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }

        private static string GetApplicationBaseUrl(IOwinRequest contextRequest)
        {
            return contextRequest.Scheme + "://" + contextRequest.Host + contextRequest.PathBase;
        }

    }
}
