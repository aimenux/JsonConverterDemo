using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Launchers;
using Bullseye;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App;

public static class Program
{
    public static async Task Main(string[] args)
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
            await targets.RunAndExitAsync(args);
        }

        Console.WriteLine("Press any key to exit !");
        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config
                    .Sources.OfType<JsonConfigurationSource>()
                    .First(x => x.Path == "appsettings.json")
                    .Optional = false;
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
        var categoryName = typeof(Program).Namespace!;
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