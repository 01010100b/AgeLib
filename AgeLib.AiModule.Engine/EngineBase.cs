using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal abstract class EngineBase : IEngine
{
    public int MyPlayer { get; internal set; } = -1;

    protected Dictionary<string, Command> Commands { get; } = [];
    protected HashSet<string> Symbols { get; } = [];

    public abstract bool Initialize(IntPtr config_ptr);
    public abstract void SetCustomString(string str);

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
    {
        var command = Commands[name];

        return command.Execute(arg1, arg2, arg3, arg4);
    }

    public IReadOnlySet<string> GetSymbols()
        => Symbols;

    public abstract int GetGoal(int goal);
    public abstract void SetGoal(int goal, int value);

    public void Log(string message)
    {
        BinaryLibs.Utils.Log.Shared.Information($"Player {MyPlayer}: {message}");
    }
}
