using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public abstract class Type : Validated
{
    public static bool IsArrayType(string type_name) => type_name.EndsWith("[]");

    public static string GetBaseTypeName(string type_name)
    {
        if (IsArrayType(type_name))
        {
            return type_name[..^2];
        }
        else
        {
            return type_name;
        }
    }

    public required string Name { get; set; }
    public abstract int Size { get; }
}
