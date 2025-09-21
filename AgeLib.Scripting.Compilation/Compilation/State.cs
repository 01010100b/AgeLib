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
            if (!exceptions.ContainsKey(thrw.Message))
            {
                var code = Math.Max(100, exceptions.Values.Max() + 1);
                exceptions.Add(thrw.Message, code);
            }
        }

        Exceptions = exceptions;
    }
}
