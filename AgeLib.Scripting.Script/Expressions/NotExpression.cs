using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script.Expressions;

public class NotExpression(FactExpression expression) : FactExpression
{
    public override int CommandCount => 1 + Expression.CommandCount;
    public FactExpression Expression { get; set; } = expression;

    public override IEnumerable<Command> GetCommands() => Expression.GetCommands();

    public override string Format(int spaces = 4)
    {
        var ins = new string(' ', spaces);

        return $"{ins}(not {Expression.Format(spaces + 4).Trim()})";
    }
}
