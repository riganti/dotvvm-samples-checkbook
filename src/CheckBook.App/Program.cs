using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CheckBook.DataAccess.Context;
using CheckBook.DataAccess.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheckBook.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var dataSeedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
                    dataSeedingService.Seed();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while seeding the database.");
                    Console.WriteLine(ex);
                    Environment.Exit(1);
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConsole();
                })
                .Build();
    }
}
