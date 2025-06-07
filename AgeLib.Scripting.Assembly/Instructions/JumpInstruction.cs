using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly.Instructions;

public class JumpInstruction : Instruction
{
    public required string Label { get; set; }
}
