using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Types;

public class Field : Validated
{
    public required string Name { get; set; }
    public required string TypeName { get; set; }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
