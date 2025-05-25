using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Scope
{
    public Scope? Parent { get; set; }
    public List<Variable> Variables { get; } = [];

    internal int GetSize(Resolver resolver)
    {
        throw new NotImplementedException();
    }

    internal int GetLocalSize(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
