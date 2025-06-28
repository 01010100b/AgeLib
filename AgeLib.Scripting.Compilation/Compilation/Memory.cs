using AgeLib.Scripting.Compilation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class Memory
{
    public Memory(Resolver resolver)
    {
        ComputeCompoundTypeSizes(resolver);
    }

    public int GetAddress(Variable variable)
    {
        throw new NotImplementedException();
    }

    private void ComputeCompoundTypeSizes(Resolver resolver)
    {
        var compound_types = new List<CompoundType>();

        foreach (var module in resolver.ResolvedModules)
        {
            foreach (var type in module.Types.OfType<CompoundType>())
            {
                compound_types.Add(type);
            }
        }

        var count = compound_types.Count;

        while (count > 0)
        {
            foreach (var type in compound_types)
            {
                type.ComputeSize(resolver);
            }

            compound_types.RemoveAll(x => x.Size > 0);

            if (compound_types.Count == count)
            {
                throw new Exception($"Circular dependency between compound types:\n{string.Join('\n', compound_types.Select(x => x.Name))}");
            }

            count = compound_types.Count;
        }
    }
}
