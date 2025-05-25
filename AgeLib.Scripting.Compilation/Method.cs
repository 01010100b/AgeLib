using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Method : Validated
{
    public required string Name { get; set; }
    public required string ReturnTypeName { get; set; }
    public List<Variable> Parameters { get; } = [];
    public List<Statement> Statements { get; } = [];
    public required Scope Scope { get; set; }

    internal string Label { get; } = Guid.NewGuid().ToString();

    internal IEnumerable<Scope> GetScopes 
        => Statements.Select(x => x.Scope).Append(Scope).Distinct();

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
