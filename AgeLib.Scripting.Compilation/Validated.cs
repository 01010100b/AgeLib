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
    private class ValidationException(string message) : Exception(message)
    {
    }

    internal static string GetModuleName(string name)
    {
        var index = name.LastIndexOf('.');

        return name[..index];
    }

    internal static string GetSimpleName(string name)
    {
        var index = name.LastIndexOf('.');

        return name[(index + 1)..];
    }

    internal static bool IsValidModuleName(string name)
    {
        const string REGEX = "^[a-zA-Z][a-zA-Z0-9]*(.[a-zA-Z][a-zA-Z0-9]*)*$";

        return Regex.IsMatch(name, REGEX);
    }

    internal static bool IsValidMethodName(string name)
        => IsValidModuleName(name);

    internal static bool IsValidTypeName(string name)
        => IsValidModuleName(Type.GetBaseTypeName(name));

    internal static bool IsValidVariableName(string name)
    {
        if (name.Contains('.'))
        {
            var module_name = GetModuleName(name);
            name = GetSimpleName(name);

            if (!IsValidModuleName(module_name))
            {
                return false;
            }
        }

        const string REGEX = "^[_a-zA-Z][_a-zA-Z0-9]*$";

        return Regex.IsMatch(name, REGEX);
    }

    internal abstract void Validate(Resolver resolver);

    protected private void ThrowIf(bool condition, string message)
    {
        if (condition)
        {
            Throw(message);
        }
    }

    protected private void Throw(string message)
    {
        throw new ValidationException(message);
    }
}
