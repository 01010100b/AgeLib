using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class State(Resolver resolver)
{
    public Resolver Resolver { get; } = resolver;
}
