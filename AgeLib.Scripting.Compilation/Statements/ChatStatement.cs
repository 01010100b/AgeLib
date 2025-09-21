using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class ChatStatement : Statement
{
    public required string Player { get; set; }
    public required string Message { get; set; }
    public Variable? Data { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        var op = "c:";
        var data = 0;

        if (Data is not null)
        {
            if (Data is Constant cst)
            {
                data = cst.Value;
            }
            else
            {
                op = "g:";
                data = state.Memory.GetAddress(Data);
            }
        }

        var command = new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-chat-data-to-player",
                Arg0 = Player,
                Arg1 = $"\"{Message}\"",
                Arg2 = op,
                Arg3 = data.ToString()
            }
        };

        return [command];
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<Variable, Variable> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<Variable> GetVariables()
    {
        throw new NotImplementedException();
    }

    private protected override void ValidateStatement(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
