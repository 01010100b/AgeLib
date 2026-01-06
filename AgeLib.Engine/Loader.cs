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
            var assembly = Assembly.UnsafeLoadFrom(dll);
            type = assembly.GetTypes().Single(x => x.IsAssignableTo(typeof(IBot)));
            Assert.That(type.GetConstructors().Any(x => x.GetParameters().Length == 0));
            Bots.Add(dll, type);
        }

        var bot = Activator.CreateInstance(type) ?? throw new Exception();

        return (IBot)bot;
    }
}
