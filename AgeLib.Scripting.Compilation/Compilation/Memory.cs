using AgeLib.Scripting.Compilation.Types;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class Memory
{
    public const int EXTRA_REGISTERS = 3;

    public int MaxGoals { get; } = 1;
    public int ExceptionCode { get; } = 2;
    public int Sp0 { get; } = 10;
    public int Sp1 { get; } = 11;
    public int Sp2 { get; } = 12;
    public int Sp3 { get; } = 13;
    public int Sp4 { get; } = 14;
    public int Sp5 { get; } = 15;
    public int Sp6 { get; } = 16;
    public int Sp7 { get; } = 17;
    public int Sp8 { get; } = 18;
    public int Sp9 { get; } = 19;
    public int Intr0 { get; } = 20;
    public int Intr1 { get; } = 21;
    public int Intr2 { get; } = 22;
    public int Intr3 { get; } = 23;
    public int Intr4 { get; } = 24;
    public int Intr5 { get; } = 25;
    public int Intr6 { get; } = 26;
    public int Intr7 { get; } = 27;
    public int Intr8 { get; } = 28;
    public int Intr9 { get; } = 29;
    public int StackBasePtr { get; } = 30;
    public int StackPtr { get; } = 31;
    public int RegisterBase { get; } = 50;
    public int RegisterCount { get; }
    public int ReturnAddress => RegisterBase;
    public int FramePtr => ReturnAddress + 1;
    public int UnwindCount => FramePtr + 1;
    public int ReturnValueBase => RegisterBase + RegisterCount;
    public int ReturnValueCount { get; }
    public int GlobalVariablesBase => ReturnValueBase + ReturnValueCount;
    public int GlobalVariablesCount { get; }
    public int InitialStackBase => GlobalVariablesBase + GlobalVariablesCount;

    private Dictionary<Variable, int> Addresses { get; } = [];

    public Memory(Resolver resolver)
    {
        ComputeCompoundTypeSizes(resolver);

        int get_register_size(Method method)
        {
            var max = 0;

            foreach (var scope in method.GetScopes())
            {
                var size = scope.GetLocalsInScope()
                    .Where(x => x is not Constant)
                    .Sum(x => resolver.ResolveType(x.TypeName).Size);
                max = Math.Max(max, size);
            }

            return max;
        }

        RegisterCount = EXTRA_REGISTERS + resolver.ResolvedModules.SelectMany(x => x.Methods).Max(get_register_size);
        ReturnValueCount = resolver.ResolvedModules.SelectMany(x => x.Methods).Max(x => resolver.ResolveType(x.ReturnTypeName).Size);
        GlobalVariablesCount = ComputeVariableAddresses(resolver);
    }

    public int GetAddress(Variable variable) => Addresses[variable];

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

        var remaining = compound_types.Count;

        while (remaining > 0)
        {
            foreach (var type in compound_types)
            {
                type.ComputeSize(resolver);
            }

            compound_types.RemoveAll(x => x.Size > 0);

            if (compound_types.Count == remaining)
            {
                throw new Exception($"Circular dependency between compound types:\n{string.Join('\n', compound_types.Select(x => x.Name))}");
            }

            remaining = compound_types.Count;
        }
    }

    private int ComputeVariableAddresses(Resolver resolver)
    {
        foreach (var scope in resolver.ResolvedModules
            .SelectMany(x => x.Methods)
            .SelectMany(x => x.GetScopes()))
        {
            var offset = scope.GetLocalsInScope()
                .Except(scope.Variables)
                .Where(x => x is not Constant)
                .Sum(x => resolver.ResolveType(x.TypeName).Size);
            offset += RegisterBase + EXTRA_REGISTERS;

            foreach (var variable in scope.Variables.Where(x => x is not Constant))
            {
                Addresses.Add(variable, offset);
                offset += resolver.ResolveType(variable.TypeName).Size;
            }
        }

        var global = 0;

        foreach (var scope in resolver.ResolvedModules.Select(x => x.GlobalScope))
        {
            foreach (var variable in scope.Variables.Where(x => x is not Constant))
            {
                Addresses.Add(variable, GlobalVariablesBase + global);
                global += resolver.ResolveType(variable.TypeName).Size;
            }
        }

        return global;
    }
}
