using System.Threading.Tasks;
using CheckBook.App.Helpers;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Routing;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CheckBook.App
{
    public class Startup
    {

        public IConfiguration Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();

            services.AddDotVVM<DotvvmStartup>();

            // presenters
            services.AddScoped<IdenticonPresenter>();

            // helpers
            services.AddSingleton<FileStorageHelper>();
            services.Configure<LoginOptions>(Configuration.GetSection("loginOptions"));
            services.AddScoped<LoginHelper>();

            // services
            services.AddScoped<GroupService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<SettlementService>();
            services.AddScoped<UserService>();
            services.AddScoped<DataSeedingService>();

            // database
            services.AddEntityFrameworkSqlite()
                .AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(Configuration.GetSection("dbOptions").GetValue<string>("connectionString")); 
                });

            var loginOptions = Configuration.GetSection("loginOptions").Get<LoginOptions>();

            // authentication
            var authBuilder = services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            if (loginOptions.AADEnabled)
            {
                authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = loginOptions.ClientId;
                    options.Authority = string.Format(loginOptions.AadInstance, loginOptions.Tenant);
                    options.SignedOutRedirectUri = loginOptions.PostLogoutRedirectUri;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = (loginOptions.Tenant != "common")
                    };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/error");
                            return Task.CompletedTask;
                        },
                        OnRedirectToIdentityProvider = context =>
                        {
                            var message = context.ProtocolMessage;
                            if (!string.IsNullOrEmpty(message.State))
                            {
                                context.Properties.Items[OpenIdConnectDefaults.UserstatePropertiesKey] = message.State;
                            }

                            message.State = context.Options.StateDataFormat.Protect(context.Properties);
                            DotvvmAuthenticationHelper.ApplyRedirectResponse(context.HttpContext, context.ProtocolMessage.BuildRedirectUrl());
                            return Task.CompletedTask;
                        }
                    };
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
            dotvvmConfiguration.AssertConfigurationIsValid();
            
            // use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(env.WebRootPath)
            });
        }
    }
}
