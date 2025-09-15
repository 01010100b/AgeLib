using AgeLib.Scripting.Compilation.Compilation;
using AgeLib.Scripting.Compilation.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public abstract class Validated
{
    public static string GetModuleName(string name)
    {
        var index = name.LastIndexOf('.');

        return name[..index];
    }

    public static string GetSimpleName(string name)
    {
        var index = name.LastIndexOf('.');

        return name[(index + 1)..];
    }

    public static void ValidateModuleName(string name)
    {
        var regex = "^[a-zA-Z][a-zA-Z0-9]*(.[a-zA-Z][a-zA-Z0-9]*)*$";

        if (!Regex.IsMatch(name, regex))
        {
            throw new Exception($"{name} is not a valid name.");
        }
    }

    public static void ValidateMethodName(string name)
        => ValidateModuleName(name);

    public static void ValidateTypeName(string name)
    {
        ValidateModuleName(Type.GetBaseTypeName(name));
    }

    public static void ValidateVariableName(string name)
    {
        if (name.Contains('.'))
        {
            var module_name = GetModuleName(name);
            ValidateModuleName(module_name);
            name = GetSimpleName(name);
        }

        var regex = "^[_a-zA-Z][_a-zA-Z0-9]*$";
        
        if (!Regex.IsMatch(name, regex))
        {
            throw new Exception($"{name} is not a valid variable name.");
        }
    }

    internal abstract void Validate(Resolver resolver);
}
