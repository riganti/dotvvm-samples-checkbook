using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using CheckBook.App.Helpers;
using CheckBook.App.Models;
using CheckBook.DataAccess.Data;
using CheckBook.DataAccess.Enums;
using CheckBook.DataAccess.Services;
using DotVVM.Framework.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace CheckBook.App
{
    public partial class Startup
    {
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

        private static void ConfigureBearerAuthentication(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = ConfigurationManager.AppSettings["ida:TenantId"],
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = (ConfigurationManager.AppSettings["ida:TenantId"] != "common"),
                        ValidAudiences = ConfigurationManager.AppSettings["ida:Audiences"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
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
                        // determines the base URL of the application (useful when the app can run on
                        // multiple domains)
                        var appBaseUrl = GetApplicationBaseUrl(context.Request);

                        if (context.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectRequestType.Authentication)
                        {
                            context.ProtocolMessage.RedirectUri = appBaseUrl;
                            // we need to handle the redirect to the login page ourselves because
                            // redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateAuthenticationRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int)HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }
                        else if (context.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectRequestType.Logout)
                        {
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            // we need to handle the redirect to the logout page ourselves because
                            // redirects cannot use HTTP 302 in DotVVM
                            var redirectUri = context.ProtocolMessage.CreateLogoutRequestUrl();
                            DotvvmRequestContext.SetRedirectResponse(DotvvmMiddleware.ConvertHttpContext(context.OwinContext), redirectUri, (int)HttpStatusCode.Redirect, true);
                            context.HandleResponse();
                        }

                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = context =>
                    {
                        context.AuthenticationTicket = ProcessAuthenticationTicket(context.AuthenticationTicket);

                        return Task.FromResult(0);
                    }
                }
            });
        }

        private static AuthenticationTicket ProcessAuthenticationTicket(AuthenticationTicket ticket)
        {
            var isMultiTenant = ConfigurationManager.AppSettings["ida:TenantId"] == "common";
            if (isMultiTenant)
            {
                // validate allowed tenants
                var tenants = ConfigurationManager.AppSettings["ida:Tenants"].Split(',');
                var tokenTenant = ticket.Identity.FindFirstValue(AzureAdClaimTypes.TenantId);
                if (!tenants.Contains(tokenTenant))
                {
                    throw new SecurityTokenValidationException($"Tenant {tokenTenant} is not allowed to sign in to the application!");
                }
            }

            // create user if it doesn't exists
            var upn = ticket.Identity.FindFirstValue(AzureAdClaimTypes.Upn);
            var user = LoginHelper.GetClaimsIdentityForAzure(upn);
            if (user == null)
            {
                var newUser = new UserInfoData
                {
                    Email = ticket.Identity.FindFirstValue(AzureAdClaimTypes.Upn),
                    FirstName = ticket.Identity.FindFirstValue(AzureAdClaimTypes.GivenName),
                    LastName = ticket.Identity.FindFirstValue(AzureAdClaimTypes.Surname),
                    Name = ticket.Identity.FindFirstValue(AzureAdClaimTypes.DisplayName),
                    UserRole = UserRole.User,
                    Password = new Guid().ToString()
                };
                UserService.CreateOrUpdateUserInfo(newUser);

                // create identity for the new user
                user = LoginHelper.GetClaimsIdentityForAzure(upn);
            }

            return new AuthenticationTicket(user, ticket.Properties);
        }

        private static string GetApplicationBaseUrl(IOwinRequest contextRequest)
        {
            return contextRequest.Scheme + "://" + contextRequest.Host + contextRequest.PathBase;
        }
    }
}