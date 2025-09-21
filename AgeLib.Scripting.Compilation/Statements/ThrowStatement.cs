using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class ThrowStatement : Statement
{
    public required string Message { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        var instructions = new List<Instruction>
        {
            new CommandInstruction()
            {
                Command = new()
                {
                    Name = "up-modify-goal",
                    Arg0 = state.Memory.ExceptionCode.ToString(),
                    Arg1 = "c:=",
                    Arg2 = state.Exceptions[Message].ToString()
                }
            },
            new JumpInstruction()
            {
                Label = state.ExceptionLabel
            }
        };

        return instructions;
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<Variable, Variable> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<Variable> GetVariables()
    {
        throw new NotImplementedException();
    }

    protected private override void ValidateStatement(Resolver resolver)
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            throw new ValidationException($"Message is empty");
        }
    }
}
