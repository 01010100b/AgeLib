using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Variable : Validated
{
    public required string Name { get; set; }
    public required string TypeName { get; set; }

    internal override void Validate(Resolver resolver)
    {
        ThrowIf(TypeName == "System.Void", $"Variable {Name} has type System.Void.");
        ThrowIf(!IsValidVariableName(Name), $"{Name} is not a valid variable name.");
        ThrowIf(!IsValidTypeName(TypeName), $"{TypeName} is not a valid type name.");
    }
}
