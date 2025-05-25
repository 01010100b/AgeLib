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
    internal abstract Statement Copy(Scope scope, IReadOnlyDictionary<Variable, Variable> variables);
    internal abstract IEnumerable<Variable> GetVariables();
}
