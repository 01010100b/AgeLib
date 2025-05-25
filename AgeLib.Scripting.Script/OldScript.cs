using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script;

public class OldScript
{
    public List<string> Comments { get; } = [];
    public List<string> Includes { get; } = [];
    public Dictionary<string, int> Constants { get; } = [];
    public List<string> Symbols { get; } = [];
    public List<Rule> Rules { get; } = [];

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var comment in Comments.SelectMany(x => x.Split('\n')))
        {
            sb.AppendLine($"; {comment}");
        }

        sb.AppendLine();

        foreach (var include in Includes)
        {
            sb.AppendLine($"(include \"{include}\")");
        }

        sb.AppendLine();

        foreach (var constant in Constants)
        {
            sb.AppendLine($"(defconst {constant.Key} {constant.Value})");
        }

        sb.AppendLine();

        foreach (var symbol in Symbols)
        {
            sb.AppendLine($"#load-if-defined {symbol}");
            sb.AppendLine($"(defconst SYM-{symbol} 1)");
            sb.AppendLine("#else");
            sb.AppendLine($"(defconst SYM-{symbol} 0)");
            sb.AppendLine("#end-if");
            sb.AppendLine();
        }

        var labels = new Dictionary<string, int>();

        for (int i = 0; i < Rules.Count; i++)
        {
            var rule = Rules[i];
            sb.AppendLine($"; {i}");

            foreach (var comment in rule.Comments.SelectMany(x => x.Split('\n')))
            {
                sb.AppendLine($"; {comment}");
            }

            sb.AppendLine(rule.ToString());
            sb.AppendLine();

            foreach (var label in rule.Labels)
            {
                labels.Add(label, i);
            }
        }

        var per = sb.ToString();

        foreach (var label in labels)
        {
            per = per.Replace(label.Key, label.Value.ToString());
        }

        return per;
    }
}
