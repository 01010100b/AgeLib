using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Constant : Variable
{
    public required int Value { get; set; }

    internal override void Validate(Resolver resolver)
    {
        base.Validate(resolver);

        throw new NotImplementedException();
    }
}
