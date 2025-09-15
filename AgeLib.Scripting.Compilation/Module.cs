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
        Validated.ValidateModuleName(Name);

        var names = Types.Select(x => x.Name).Concat(Methods.Select(x => x.Name))
            .Concat(GlobalScope.Variables.Select(x => x.Name)).ToList();

        foreach (var name in names)
        {
            if (Validated.GetModuleName(name) != Name)
            {
                throw new Exception($"Name {name} does not start with module name.");
            }
        }

        foreach (var export in Exports)
        {
            if (!names.Contains(export))
            {
                throw new Exception($"Export {export} does not exist in module.");
            }
        }

        throw new NotImplementedException();
    }
}
