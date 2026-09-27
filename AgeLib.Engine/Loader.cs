using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine;

internal static class Loader
{
    private static Dictionary<string, Type> Bots { get; } = [];

    public static IBot Create(string dll)
    {
        var path = Path.GetFullPath(dll);
        if (!Bots.TryGetValue(path, out var type))
        {
            Log.Shared.Information($"Loading bot assembly '{path}'. Exists={File.Exists(path)}.");
            var assembly = new BotLoadContext(path).LoadFromAssemblyPath(path);
            Log.Shared.Information($"Loaded assembly '{assembly.FullName}'. Finding IBot implementation.");
            type = assembly.GetTypes().Single(x => x.IsAssignableTo(typeof(IBot)));
            Log.Shared.Information($"Found bot type '{type.FullName}'.");
            Assert.That(type.GetConstructors().Any(x => x.GetParameters().Length == 0));
            Bots.Add(path, type);
        }

        Log.Shared.Information($"Creating bot instance of '{type.FullName}'.");
        var bot = Activator.CreateInstance(type) ?? throw new Exception();

        return (IBot)bot;
    }

    private sealed class BotLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver Resolver { get; }

        public BotLoadContext(string bot_path)
        {
            Resolver = new(bot_path);
        }

        protected override Assembly? Load(AssemblyName assembly_name)
        {
            var host_assembly = GetHostAssembly(assembly_name);
            if (host_assembly is not null)
            {
                return host_assembly;
            }

            var path = Resolver.ResolveAssemblyToPath(assembly_name);
            return path is null ? null : LoadFromAssemblyPath(path);
        }

        private static Assembly? GetHostAssembly(AssemblyName assembly_name)
        {
            return new[] { typeof(IBot).Assembly, typeof(AgeLib.Common.Enums.FactId).Assembly }
                .FirstOrDefault(x => string.Equals(
                    x.GetName().Name,
                    assembly_name.Name,
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}
