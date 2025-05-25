using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Script;

public class Command(string name, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
{
    public string Name { get; set; } = name;
    public string Arg0 { get; set; } = arg0;
    public string Arg1 { get; set; } = arg1;
    public string Arg2 { get; set; } = arg2;
    public string Arg3 { get; set; } = arg3;

    public override string ToString() => $"{Name} {Arg0} {Arg1} {Arg2} {Arg3}".Trim();
}
