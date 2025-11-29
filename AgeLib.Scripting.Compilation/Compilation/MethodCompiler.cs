using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Compilation;

internal class MethodCompiler
{
    public static List<Instruction> Compile(Method method, State state, bool unwind = true)
    {
        var instructions = new List<Instruction>();

        if (unwind)
        {
            state.MethodPostfixLabels.Add(method, state.UnwindStackLabel);
        }
        else
        {
            state.MethodPostfixLabels.Add(method, Guid.NewGuid().ToString());
        }

        instructions.Add(new LabelInstruction() { Label = method.Label });

        foreach (var statement in method.Statements)
        {
            instructions.AddRange(statement.Compile(state));
        }

        if (!unwind)
        {
            instructions.Add(new LabelInstruction() { Label = state.MethodPostfixLabels[method] });
        }

        return instructions;
    }
}
