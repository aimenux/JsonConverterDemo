using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using App.Launchers;
using Bullseye;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App;

public static class Extensions
{
    public static void WriteLine(this ConsoleColor color, object value)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(value);
        Console.ResetColor();
    }
    
    public static Task RunAndExitAsync(this IHost host, IEnumerable<string> args)
    {
        var launchers = host.Services.GetServices<ILauncher>();
        
        var readOnlyScenarios = launchers
            .OrderBy(_ => Guid.NewGuid())
            .ToImmutableArray();
        
        return readOnlyScenarios.ToTargets().RunAndExitAsync(args);
    }

    private static Targets ToTargets(this IReadOnlyCollection<ILauncher> launchers)
    {
        var targets = new Targets();
        targets.Add("DEFAULT", dependsOn: launchers.Select(x => x.Name));
        foreach (var launcher in launchers)
        {
            targets.Add(launcher.Name, launcher.DependsOn, () => launcher.Launch());
        }
        return targets;
    }
}