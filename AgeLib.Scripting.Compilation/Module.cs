using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Module : Validated
{
    public required string Name { get; set; }
    public List<string> Imports { get; } = [];
    public List<Type> Types { get; } = [];
    public List<Method> Methods { get; } = [];
    public required Scope GlobalScope { get; set; }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
