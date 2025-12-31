using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements.Basic;

public class ChatStatement : Statement
{
    public required string PlayerString { get; set; }
    public required string MessageString { get; set; }
    public string? Data { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        throw new NotImplementedException();
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<string, string> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<string> GetVariables()
    {
        if (Data is not null)
        {
            yield return Data;
        }
    }

    private protected override void ValidateStatement(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
