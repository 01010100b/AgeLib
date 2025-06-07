using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script.Expressions;

public class OrExpression : FactExpression
{
    public required FactExpression Left { get; set; }
    public required FactExpression Right { get; set; }
    public override int CommandCount => 1 + Left.CommandCount + Right.CommandCount;

    public override IEnumerable<Command> GetCommands() => Left.GetCommands().Concat(Right.GetCommands());

    public override string Format(int spaces = 4) => FormatOp("or", spaces, Left, Right);
}
