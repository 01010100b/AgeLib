using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine;

internal static class Loader
{
    private static Dictionary<string, Type> Bots { get; } = [];

    public static IBot Create(string dll)
    {
        if (!Bots.TryGetValue(dll, out var type))
        {
            Log.Shared.Information($"Loading bot assembly '{dll}'. Exists={File.Exists(dll)}.");
            var assembly = Assembly.UnsafeLoadFrom(dll);
            Log.Shared.Information($"Loaded assembly '{assembly.FullName}'. Finding IBot implementation.");
            type = assembly.GetTypes().Single(x => x.IsAssignableTo(typeof(IBot)));
            Log.Shared.Information($"Found bot type '{type.FullName}'.");
            Assert.That(type.GetConstructors().Any(x => x.GetParameters().Length == 0));
            Bots.Add(dll, type);
        }

        Log.Shared.Information($"Creating bot instance of '{type.FullName}'.");
        var bot = Activator.CreateInstance(type) ?? throw new Exception();

        return (IBot)bot;
    }
}
