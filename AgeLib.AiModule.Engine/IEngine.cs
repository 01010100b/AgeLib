using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public interface IEngine
{
    public const int CUSTOM_STRING_ID = 89733;

    public int MyPlayer { get; }

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);
    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        => Execute(name, arg1, arg2, arg3, arg4) != 0;

    public void SetCustomString(string str);
    public IReadOnlySet<string> GetSymbols();
    public bool IsSymbolDefined(string symbol)
        => GetSymbols().Contains(symbol);

    public int GetGoal(int goal);
    public void SetGoal(int goal, int value);

    public void Log(string message);
}
