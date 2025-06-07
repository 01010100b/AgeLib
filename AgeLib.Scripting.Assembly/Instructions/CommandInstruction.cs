using AgeLib.Scripting.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly.Instructions;

public class CommandInstruction : Instruction
{
    public required Command Command { get; set; }
}
