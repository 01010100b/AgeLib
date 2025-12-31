using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Module : Validated
{
    public required string Name { get; set; }
    public List<string> Exports { get; set; } = [];
    public List<string> Imports { get; set; } = [];
    public List<Type> Types { get; set; } = [];
    public List<Method> Methods { get; set; } = [];
    public Scope GlobalScope { get; set; } = new();

    internal override void Validate(Resolver resolver)
    {
        ThrowIf(!IsValidModuleName(Name), $"{Name} is not a valid module name.");

        var names = new HashSet<string>();
        
        foreach (var name in Types.Select(x => x.Name)
            .Concat(Methods.Select(x => x.Name))
            .Concat(GlobalScope.Variables.Select(x => x.Name)))
        {
            ThrowIf(names.Contains(name), $"Name {name} already exists.");
            ThrowIf(GetModuleName(name) != Name, $"Name {name} does not start with module name {Name}.");
        }

        foreach (var export in Exports)
        {
            ThrowIf(!names.Contains(export), $"Export {export} does not exist in module.");
        }

        foreach (var import in Imports)
        {
            ThrowIf(!IsValidModuleName(import), $"{import} is not a valid import name.");
        }

        foreach (var type in Types)
        {
            type.Validate(resolver);
        }

        foreach (var method in Methods)
        {
            method.Validate(resolver);
        }

        GlobalScope.Validate(resolver);
    }
}
