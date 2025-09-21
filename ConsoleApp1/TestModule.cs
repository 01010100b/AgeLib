using AgeLib.Scripting.Compilation;
using AgeLib.Scripting.Compilation.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1;

internal static class TestModule
{
    public static Module Create()
    {
        var module = new Module() { Name = "Test" };

        var method = new Method()
        {
            Name = "Test.Main",
            ReturnTypeName = "System.Void",
            Scope = new() { Parent = module.GlobalScope }
        };
        module.Methods.Add(method);

        var constant = new Constant()
        {
            Name = "some_constant",
            TypeName = "System.Int",
            Value = 77
        };
        method.Scope.Variables.Add(constant);

        var statement = new ChatStatement()
        {
            Scope = method.Scope,
            Player = "my-player-number",
            Message = "test %s",
            Data = constant
        };
        method.Statements.Add(statement);

        var variable = new Variable()
        {
            Name = "some_var",
            TypeName = "System.Int"
        };
        method.Scope.Variables.Add(variable);

        var variable2 = new Variable()
        {
            Name = "some_var2",
            TypeName = "System.Bool"
        };
        method.Scope.Variables.Add(variable2);

        return module;
    }
}
