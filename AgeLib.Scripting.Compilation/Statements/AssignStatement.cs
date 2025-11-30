using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class AssignStatement : Statement
{
    public required string Result { get; set; }
    public required string Source { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        var instructions = new List<Instruction>();
        var result = state.Resolver.ResolveVariable(Result, Scope);
        var source = state.Resolver.ResolveVariable(Source, Scope);
        var type = state.Resolver.ResolveType(result.TypeName);
        var from = state.Memory.GetAddress(source);
        var to = state.Memory.GetAddress(result);
        var size = type.Size;

        instructions.AddRange(state.Copy(from, false, to, false, size, false));

        return instructions;
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<string, string> variables)
    {
        return new AssignStatement()
        {
            Scope = scope,
            Result = variables[Result],
            Source = variables[Source]
        };
    }

    internal override IEnumerable<string> GetVariables()
        => [Result, Source];

    private protected override void ValidateStatement(Resolver resolver)
    {
        var result = resolver.ResolveVariable(Result, Scope);
        var source = resolver.ResolveVariable(Source, Scope);

        ThrowIf(result is Constant, $"Result is a constant.");
        ThrowIf(result.TypeName != source.TypeName, $"Result and Source are not same type.");
    }
}
