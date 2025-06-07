using AgeLib.Scripting.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly.Instructions;

public class RuleInstruction : Instruction
{
    public List<FactExpression> Facts { get; set; } = [];
    public List<Command> Actions { get; set; } = [];
}
