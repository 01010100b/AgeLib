using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public abstract class Validated
{
    public static void ValidateName(string name)
    {
        // module & method & type
        var regex = "^[a-zA-Z]+(.[a-zA-Z]+)*$";

        if (!Regex.IsMatch(name, regex))
        {
            throw new Exception($"{name} is not a valid name");
        }
    }

    public static void ValidateVariableName(string name)
    {
        var regex = "^[_a-zA-Z][_a-zA-Z0-9]*$";
        
        if (!Regex.IsMatch(name, regex))
        {
            throw new Exception($"{name} is not a valid variable name");
        }
    }

    internal abstract void Validate(Resolver resolver);
}
