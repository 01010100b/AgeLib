using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly.Instructions;

public class JumpIndirectInstruction : Instruction
{
    public required int Goal {  get; set; }
}
