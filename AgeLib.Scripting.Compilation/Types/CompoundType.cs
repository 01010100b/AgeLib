using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Types;

public class CompoundType : Type
{
    public required List<Field> Fields { get; set; }
    public override int Size => ComputedSize;

    private int ComputedSize { get; set; } = 0;

    internal void ComputeSize(Resolver resolver)
    {
        var size = 0;

        foreach (var field in Fields)
        {
            var type = resolver.ResolveType(field.TypeName);

            if (type.Size > 0)
            {
                size += type.Size;
            }
            else
            {
                return;
            }
        }

        ComputedSize = size;
    }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
