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

        if (TypeName != "System.Int")
        {
            if (TypeName == "System.Bool")
            {
                ThrowIf(Value != 0 && Value != 1, $"Constant of type System.Bool is not 0 or 1.");
            }
            else
            {
                Throw($"Constant type is not System.Int or System.Bool.");
            }
        }
    }
}
