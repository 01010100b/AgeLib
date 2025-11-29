using AgeLib.Scripting.Compilation.Statements;
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
    private Dictionary<string, ArrayType> ArrayTypes { get; } = [];

    public Resolver(Module main_module, List<Func<string, Module?>> resolvers)
    {
        Modules.Add("System", GenerateSystemModule());

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
                    throw new Exception($"Failed to resolve module {name}.");
                }
            }

            foreach (var import in module.Imports.Where(x => !Modules.ContainsKey(x)))
            {
                stack.Push(import);
            }
        }

        foreach (var method in ResolvedModules.SelectMany(x => x.Methods).Where(x => x.ReturnTypeName == "System.Void"))
        {
            if (method.Statements[^1] is not ReturnStatement)
            {
                method.Statements.Add(new ReturnStatement() { Scope = method.Scope });
            }
        }
    }

    public Module ResolveModule(string name)
    {
        Validated.ValidateModuleName(name);

        return Modules[name];
    }

    public Type ResolveType(string name)
    {
        Validated.ValidateTypeName(name);

        if (Type.IsArrayType(name))
        {
            if (!ArrayTypes.TryGetValue(name, out var type))
            {
                type = new() { Name = name, ElementType = ResolveType(Type.GetBaseTypeName(name)) };
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

    public Method ResolveMethod(Statement statement)
        => ResolvedModules.SelectMany(x => x.Methods).Single(x => x.Statements.Contains(statement));

    public Variable ResolveVariable(string name, Scope? scope)
    {
        throw new NotImplementedException();
    }

    public bool IsAccessible(string name, Scope scope)
    {
        if (scope.GetLocalsInScope().Select(x => x.Name).Contains(name))
        {
            return true;
        }

        var module = ResolvedModules.Single(x => x.GlobalScope == scope.GetGlobalScope());

        if (Type.IsArrayType(name))
        {
            name = Type.GetBaseTypeName(name);
        }

        if (module.Types.Select(x => x.Name)
            .Concat(module.Methods.Select(x => x.Name))
            .Concat(module.GlobalScope.Variables.Select(x => x.Name))
            .Contains(name))
        {
            return true;
        }

        if (GetAllImports(module).SelectMany(x => x.Exports).Contains(name))
        {
            return true;
        }

        return false;
    }

    private IEnumerable<Module> GetAllImports(Module module)
    {
        var set = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue("System");

        foreach (var import in module.Imports)
        {
            queue.Enqueue(import);
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            set.Add(current);

            foreach (var import in ResolveModule(current).Imports)
            {
                if (!set.Contains(import))
                {
                    queue.Enqueue(import);
                }
            }
        }

        return set.Select(ResolveModule);
    }

    private Module GenerateSystemModule()
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

        return system_module;
    }
}
