using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Types;

internal class ArrayType : Type
{
    public required Type ElementType { get; init; }
    public override int Size => 1;

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
