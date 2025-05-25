using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class LabelStatement : Statement
{
    public string Label { get; } = Guid.NewGuid().ToString();

    internal override List<Instruction> Compile(State state)
    {
        throw new NotImplementedException();
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<Variable, Variable> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<Variable> GetVariables()
    {
        throw new NotImplementedException();
    }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
