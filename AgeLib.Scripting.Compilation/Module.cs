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
    public List<string> Imports { get; set; } = [];
    public List<Type> Types { get; set; } = [];
    public List<Method> Methods { get; set; } = [];
    public Scope Scope { get; set; } = new();

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
