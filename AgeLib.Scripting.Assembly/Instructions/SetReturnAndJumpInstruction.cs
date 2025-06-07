using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly.Instructions;

public class SetReturnAndJumpInstruction : Instruction
{
    public required int ReturnGoal {  get; set; }
    public required string Label { get; set; }
}
