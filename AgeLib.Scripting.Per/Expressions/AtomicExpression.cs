using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Per.Expressions;

public class AtomicExpression(Command command) : FactExpression
{
    public override int CommandCount => 1;
    public Command Command { get; set; } = command;

    public override string Format(int spaces = 4)
    {
        var ins = new string(' ', spaces);

        return $"{ins}({Command})";
    }
}
