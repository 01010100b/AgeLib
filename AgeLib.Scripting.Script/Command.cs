using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script;

public class Command
{
    public required string Name { get; set; }
    public string Arg0 { get; set; } = "";
    public string Arg1 { get; set; } = "";
    public string Arg2 { get; set; } = "";
    public string Arg3 { get; set; } = "";

    public override string ToString() => $"{Name} {Arg0} {Arg1} {Arg2} {Arg3}".Trim();
}
