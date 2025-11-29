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

    internal int GetOffset(IEnumerable<string> accessors, Resolver resolver)
    {
        var lst = accessors.ToList();
        var offset = 0;

        foreach (var field in Fields)
        {
            if (field.Name != lst[0])
            {
                offset += resolver.ResolveType(field.TypeName).Size;
            }
            else
            {
                lst.RemoveAt(0);

                if (lst.Count == 0)
                {
                    return offset;
                }
                else
                {
                    var type = (CompoundType)resolver.ResolveType(field.TypeName);

                    return offset + type.GetOffset(lst, resolver);
                }
            }
        }

        throw new Exception();
    }

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
