using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class ReturnStatement : Statement
{
    public string? ResultVariable { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        var instructions = new List<Instruction>();

        if (ResultVariable is not null)
        {
            var variable = state.Resolver.ResolveVariable(ResultVariable, Scope);
            var size = state.Resolver.ResolveType(variable.TypeName).Size;
            var from = state.Memory.GetAddress(variable);
            var to = state.Memory.ReturnValueBase;

            instructions.AddRange(state.Copy(from, false, to, false, size, false));
        }

        instructions.Add(new JumpInstruction() 
        { 
            Label = state.MethodPostfixLabels[state.Resolver.ResolveMethod(this)] 
        });

        return instructions;
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<string, string> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<string> GetVariables()
    {
        throw new NotImplementedException();
    }

    protected private override void ValidateStatement(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
