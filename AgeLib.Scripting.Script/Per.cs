using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script;

public class Per
{
    public List<string> Comments { get; set; } = [];
    public List<string> Includes { get; set; } = [];
    public Dictionary<string, int> Constants { get; set; } = [];
    public Dictionary<string, string> StringConstants { get; set; } = [];
    public List<string> Symbols { get; set; } = [];
    public List<Rule> Rules { get; set; } = [];

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

        foreach (var constant in StringConstants)
        {
            var cst = constant.Value;

            if (cst == "")
            {
                continue;
            }

            if (!cst.StartsWith('"'))
            {
                cst = "\"" + cst;
            }

            if (!cst.EndsWith('"'))
            {
                cst += "\"";
            }

            sb.AppendLine($"(defconst {constant.Key} {cst})");
        }

        sb.AppendLine();

        foreach (var symbol in Symbols)
        {
            sb.AppendLine($"#load-if-defined {symbol}");
            sb.AppendLine($"    (defconst SYM-{symbol} 1)");
            sb.AppendLine("#else");
            sb.AppendLine($"    (defconst SYM-{symbol} 0)");
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
