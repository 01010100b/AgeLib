using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Scope : Validated
{
    public Scope? Parent { get; set; } = null;
    public List<Variable> Variables { get; set; } = [];

    internal Scope GetGlobalScope()
    {
        var current = this;

        while (current.Parent is not null)
        {
            current = current.Parent;
        }

        return current;
    }

    internal IEnumerable<Variable> GetLocalsInScope()
    {
        var current = this;

        while (current.Parent is not null)
        {
            foreach (var variable in current.Variables)
            {
                yield return variable;
            }

            current = current.Parent;
        }
    }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
