using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Per.Expressions;

public class OrExpression(FactExpression left, FactExpression right) : FactExpression
{
    public override int CommandCount => 1 + Left.CommandCount + Right.CommandCount;
    public FactExpression Left { get; set; } = left;
    public FactExpression Right { get; set; } = right;

    public override string Format(int spaces = 4) => FormatOp("or", spaces, Left, Right);
}
