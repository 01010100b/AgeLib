using AgeLib.Scripting.Assembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class MethodCompiler
{
    public static List<Instruction> Compile(Method method, State state)
    {
        var instructions = new List<Instruction>();

        foreach (var statement in method.Statements)
        {
            instructions.AddRange(statement.Compile(state));
        }

        return instructions;
    }
}
