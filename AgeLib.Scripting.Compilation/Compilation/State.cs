using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class State
{
    public Resolver Resolver { get; }
    public Memory Memory { get; }
    public IReadOnlyDictionary<string, int> Exceptions { get; }
    public string ExceptionLabel { get; } = Guid.NewGuid().ToString();
    public string UnwindStackLabel { get; } = Guid.NewGuid().ToString();
    public Dictionary<Method, string> MethodPostfixLabels { get; } = [];

    public State(Resolver resolver)
    {
        Resolver = resolver;
        Memory = new(resolver);

        var exceptions = new Dictionary<string, int>()
        {
            { "Stack Overflow", 1 }
        };

        foreach (var thrw in resolver.ResolvedModules
            .SelectMany(x => x.Methods)
            .SelectMany(x => x.Statements)
            .OfType<ThrowStatement>())
        {
            if (!exceptions.ContainsKey(thrw.MessageString))
            {
                var code = Math.Max(100, exceptions.Values.Max() + 1);
                exceptions.Add(thrw.MessageString, code);
            }
        }

        Exceptions = exceptions;
    }

    public List<Instruction> Clear(int from, bool is_from_ref, int length, bool is_length_goal)
    {
        throw new NotImplementedException();
    }

    public List<Instruction> Copy(int from, bool is_from_ref, int to, bool is_to_ref, int length, bool is_length_goal)
    {
        throw new NotImplementedException();
    }
}
