using AgeLib.Scripting.Compilation.Compilation;
using BinaryLibs.Utils;
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

    internal (Type, int) GetAccessor(IEnumerable<string> accessors, Resolver resolver)
    {
        var lst = accessors.ToList();
        Assert.That(lst.Count > 0);
        var offset = 0;

        foreach (var field in Fields)
        {
            var type = resolver.ResolveType(field.TypeName);

            if (field.Name != lst[0])
            {
                offset += type.Size;
            }
            else
            {
                lst.RemoveAt(0);

                if (lst.Count == 0)
                {
                    return (type, offset);
                }
                else
                {
                    (type, var extra) = ((CompoundType)type).GetAccessor(lst, resolver);

                    return (type, offset + extra);
                }
            }
        }

        throw new Exception($"Failed accessor on type {Name}");
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
