using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Types;

public class PrimitiveType : Type
{
    public required int TypeSize { get; set; }
    public override int Size => TypeSize;

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
