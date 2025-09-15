using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Scope : Validated
{
    public Scope? Parent { get; set; }
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

    internal IEnumerable<Variable> GetVariablesInScope()
    {
        var current = this;

        while (current is not null)
        {
            foreach (var variable in current.Variables)
            {
                yield return variable;
            }

            current = current.Parent;
        }
    }

    internal int GetSize(Resolver resolver)
        => Variables.Where(x => x is not Constant).Sum(x => resolver.ResolveType(x.TypeName).Size);

    internal int GetLocalSize(Resolver resolver)
    {
        var size = 0;
        var current = this;

        while (current.Parent is not null)
        {
            size += current.GetSize(resolver);
            current = current.Parent;
        }

        return size;
    }

    internal override void Validate(Resolver resolver)
    {
        foreach (var variable in Variables)
        {
            variable.Validate(resolver);

            if (!resolver.IsAccessible(variable.TypeName, this))
            {
                throw new Exception($"Type {variable.TypeName} is not accessible.");
            }

            foreach (var other in GetVariablesInScope().Except([variable]))
            {
                if (other.Name == variable.Name)
                {
                    throw new Exception($"A variable with name {variable.Name} already exists in scope.");
                }
            }
        }

        throw new NotImplementedException();
    }
}
