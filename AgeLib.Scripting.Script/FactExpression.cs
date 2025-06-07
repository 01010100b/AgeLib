using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script;

public abstract class FactExpression
{
    public abstract int CommandCount { get; }

    public abstract IEnumerable<Command> GetCommands();
    public abstract string Format(int spaces = 4);

    protected string FormatOp(string op, int spaces, FactExpression first, FactExpression second)
    {
        var ins = new string(' ', spaces);

        return $"{ins}({op}\n{first.Format(spaces + 4)}\n{second.Format(spaces + 4)})";
    }
}
