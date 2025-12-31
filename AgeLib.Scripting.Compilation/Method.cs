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
    public List<string> Parameters { get; } = [];
    public List<Statement> Statements { get; } = [];
    public required Scope Scope { get; set; }

    internal string Label { get; } = Guid.NewGuid().ToString();

    internal IEnumerable<Scope> GetScopes() 
        => Statements.Select(x => x.Scope).Append(Scope).Distinct();

    internal override void Validate(Resolver resolver)
    {
        ThrowIf(!IsValidMethodName(Name), $"{Name} is not a valid method name.");
        ThrowIf(!IsValidTypeName(ReturnTypeName), $"{ReturnTypeName} is not a valid type name.");
        ThrowIf(Type.IsArrayType(ReturnTypeName), $"Can not return array from method {Name}.");
        ThrowIf(!resolver.IsAccessible(ReturnTypeName, Scope), 
            $"Return type {ReturnTypeName} is not accessible from method {Name}.");

        foreach (var parameter in Parameters)
        {
            ThrowIf(!Scope.Variables.Where(x => x is not Constant).Any(x => x.Name == parameter), 
                $"Parameter {parameter} in method {Name} not found in method scope.");
        }

        foreach (var statement in Statements)
        {
            statement.Validate(resolver);
        }

        foreach (var scope in GetScopes())
        {
            scope.Validate(resolver);
        }

        throw new NotImplementedException();
    }

    internal void ValidateMain()
    {
        ThrowIf(ReturnTypeName != "System.Void", $"Main methd {Name} does not have return type System.Void.");
        ThrowIf(Parameters.Count != 0, $"Main method {Name} has parameters.");
    }
}
