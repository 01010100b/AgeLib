using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public abstract class Type : Validated
{
    public required string Name { get; set; }
    public abstract int Size { get; }
}
