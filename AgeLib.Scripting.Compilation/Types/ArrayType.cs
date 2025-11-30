using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Types;

internal class ArrayType : Type
{
    public required Type ElementType { get; init; }
    public override int Size => 1;

    internal int GetOffset(Constant index)
    {
        Assert.That(index.TypeName == "System.Int");
        var offset = index.Value;
        offset *= ElementType.Size;
        offset++;

        return offset;
    }

    internal List<Instruction> GetOffset(int goal, Variable index, Memory memory)
    {
        Assert.That(index is not Constant);
        Assert.That(index.TypeName == "System.Int");
        var instructions = new List<Instruction>();
        var stride = ElementType.Size;
        var address = memory.GetAddress(index);

        instructions.AddRange(
        [
            new CommandInstruction()
            {
                Command = new()
                {
                    Name = "up-modify-goal",
                    Arg0 = goal.ToString(),
                    Arg1 = "g:=",
                    Arg2 = address.ToString()
                }
            },
            new CommandInstruction()
            {
                Command = new()
                {
                    Name = "up-modify-goal",
                    Arg0 = goal.ToString(),
                    Arg1 = "c:*",
                    Arg2 = stride.ToString()
                }
            },
            new CommandInstruction()
            {
                Command = new()
                {
                    Name = "up-modify-goal",
                    Arg0 = goal.ToString(),
                    Arg1 = "c:+",
                    Arg2 = "1"
                }
            }
        ]);

        return instructions;
    }

    internal override void Validate(Resolver resolver)
    {
        throw new NotImplementedException();
    }
}
