using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Launchers;
using Bullseye;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                var launchers = host.Services.GetServices<ILauncher>().ToList();
                var targets = InitializeTargets(launchers);
                foreach (var launcher in launchers)
                {
                    targets.Add(launcher.Name, () =>
                    {
                        launcher.Launch();
                    });
                }
                targets.RunAndExit(args);
            }

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddTransient<ILauncher, LibOneLauncher>();
                    services.AddTransient<ILauncher, LibTwoLauncher>();
                })
                .ConfigureLogging((_, loggingBuilder) =>
                {
                    loggingBuilder.AddNonGenericLogger();
                })
                .UseConsoleLifetime();

        private static void AddNonGenericLogger(this ILoggingBuilder loggingBuilder)
        {
            var categoryName = typeof(Program).Namespace;
            var services = loggingBuilder.Services;
            services.AddSingleton(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger(categoryName);
            });
        }

        private static Targets InitializeTargets(IEnumerable<ILauncher> launchers)
        {
            var targets = new Targets();
            var names = launchers
                .Select(x => x.Name)
                .OrderBy(_ => Guid.NewGuid())
                .ToList();
            targets.Add("DEFAULT", dependsOn: names);
            return targets;
        }
    }
}
