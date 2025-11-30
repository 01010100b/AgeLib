using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public abstract class Statement : Validated
{
    public required Scope Scope { get; set; }

    internal abstract List<Instruction> Compile(State state);
    internal abstract Statement Copy(Scope scope, IReadOnlyDictionary<string, string> variables);
    internal abstract IEnumerable<string> GetVariables();

    internal sealed override void Validate(Resolver resolver)
    {
        foreach (var variable in GetVariables())
        {
            ThrowIf(!resolver.IsAccessible(variable, Scope), $"Variable {variable} is not accessible.");
            var v = resolver.ResolveVariable(variable, Scope);
            ThrowIf(!resolver.IsAccessible(v.TypeName, Scope), $"Type {v.TypeName} is not accessible.");
        }

        ValidateStatement(resolver);
    }

    protected private abstract void ValidateStatement(Resolver resolver);
}
