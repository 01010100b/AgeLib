using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Per;

public class Rule
{
    public List<string> Comments { get; } = [];
    public List<string> Labels { get; } = [];
    public List<FactExpression> Facts { get; } = [];
    public List<Command> Actions { get; } = [];

    public int CommandCount => Math.Max(1, Facts.Sum(x => x.CommandCount)) + Math.Max(1, Actions.Count);
    public bool IsAlwaysTrue => Facts.Count == 0;
    public bool IsJump => Actions.Any(x => x.Name.StartsWith("up-jump-"));
    public bool IsEmpty => Facts.Count == 0 && Actions.Count == 0;

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var comment in Comments.SelectMany(x => x.Split('\n')))
        {
            sb.AppendLine($"; {comment}");
        }

        sb.AppendLine("(defrule");

        if (Facts.Count == 0)
        {
            sb.AppendLine("\t(true)");
        }
        else
        {
            foreach (var fact in Facts)
            {
                sb.AppendLine($"{fact.Format()}");
            }
        }

        sb.AppendLine("=>");

        if (Actions.Count == 0)
        {
            sb.AppendLine("    (do-nothing)");
        }
        else
        {
            foreach (var action in Actions)
            {
                sb.AppendLine($"    ({action})");
            }
        }

        sb.AppendLine(")");

        return sb.ToString();
    }
}
