using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script.Expressions;

public class AndExpression(FactExpression left, FactExpression right) : FactExpression
{
    public override int CommandCount => 1 + Left.CommandCount + Right.CommandCount;
    public FactExpression Left { get; set; } = left;
    public FactExpression Right { get; set; } = right;

    public override IEnumerable<Command> GetCommands() => Left.GetCommands().Concat(Right.GetCommands());

    public override string Format(int spaces = 4) => FormatOp("and", spaces, Left, Right);
}
