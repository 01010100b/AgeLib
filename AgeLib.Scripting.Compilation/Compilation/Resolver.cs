using AgeLib.Scripting.Compilation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class Resolver
{
    public IEnumerable<Module> ResolvedModules => Modules.Values;

    private Dictionary<string, Module> Modules { get; } = [];
    private Dictionary<string, PointerType> PointerTypes { get; } = [];
    private Dictionary<string, ArrayType> ArrayTypes { get; } = [];

    public Resolver(Module main_module, List<Func<string, Module?>> resolvers)
    {
        var system_module = new Module() { Name = "System" };
        system_module.Types.Add(new PrimitiveType() { Name = "System.Void", TypeSize = 0 });
        system_module.Types.Add(new PrimitiveType() { Name = "System.Int", TypeSize = 1 });
        system_module.Types.Add(new PrimitiveType() { Name = "System.Bool", TypeSize = 1 });
        system_module.Types.Add(new CompoundType()
        {
            Name = "System.Point",
            Fields =
            [
                new() { Name = "X", TypeName = "System.Int"},
                new() { Name = "Y", TypeName = "System.Int"}
            ]
        });

        Modules.Add("System", system_module);

        Modules.Add(main_module.Name, main_module);
        var stack = new Stack<string>();
        
        foreach (var import in main_module.Imports.Where(x => !Modules.ContainsKey(x)))
        {
            stack.Push(import);
        }

        while (stack.Count > 0)
        {
            var name = stack.Pop();

            if (!Modules.TryGetValue(name, out var module))
            {
                foreach (var resolver in resolvers)
                {
                    module = resolver(name);

                    if (module is not null)
                    {
                        Modules.Add(name, module);

                        break;
                    }
                }

                if (module is null)
                {
                    throw new Exception($"Failed to resolve module {name}");
                }
            }

            foreach (var import in module.Imports.Where(x => !Modules.ContainsKey(x)))
            {
                stack.Push(import);
            }
        }
    }

    public Module ResolveModule(string name)
        => Modules[name];

    public Type ResolveType(string name)
    {
        Validated.ValidateTypeName(name);

        if (name.EndsWith('*'))
        {
            // pointer
            if (!PointerTypes.TryGetValue(name, out var type))
            {
                type = new() { Name = name, PointedType = ResolveType(name[..^1]) };
                type.Validate(this);
                PointerTypes.Add(name, type);
            }

            return type;
        }
        else if (name.EndsWith("[]"))
        {
            // array
            if (!ArrayTypes.TryGetValue(name, out var type))
            {
                type = new() { Name = name, ElementType = ResolveType(name[..^2]) };
                type.Validate(this);
                ArrayTypes.Add(name, type);
            }

            return type;
        }
        else
        {
            return ResolveModule(Validated.GetModuleName(name)).Types.Single(x => x.Name == name);
        }
    }

    public Method ResolveMethod(string name)
        => ResolveModule(Validated.GetModuleName(name)).Methods.Single(x => x.Name == name);

    public Variable ResolveVariable(string name, Method? method)
    {
        throw new NotImplementedException();
    }

    public bool IsExported(string name)
    {
        if (!name.Contains('.'))
        {
            return false;
        }

        var module = ResolveModule(Validated.GetModuleName(name));

        return module.Exports.Contains(name);
    }
}
