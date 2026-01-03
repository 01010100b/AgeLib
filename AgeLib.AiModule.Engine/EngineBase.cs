using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal abstract class EngineBase : IEngine
{
    private const int CUSTOM_STRING_ID = 89733;

    public abstract int Version { get; }
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

    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        => Execute(name, arg1, arg2, arg3, arg4) != 0;

    public IEnumerable<string> GetSymbols()
        => Symbols;

    public bool IsSymbolDefined(string symbol)
        => Symbols.Contains(symbol);

    public abstract int GetGoal(int goal);
    public abstract void SetGoal(int goal, int value);

    public void ChatToAll(string str)
        => ChatDataToAll(str, TypeOp.C, 0);

    public void ChatToPlayer(int player, string str)
        => ChatDataToPlayer(player, str, TypeOp.C, 0);

    public void ChatDataToAll(string str, int type_op, int value)
    {
        SetCustomString(str);
        Execute("up-chat-data-to-all", CUSTOM_STRING_ID, type_op, value);
    }

    public void ChatDataToPlayer(int player, string str, int type_op, int value)
    {
        SetCustomString(str);
        Execute("up-chat-data-to-player", player, CUSTOM_STRING_ID, type_op, value);
    }

    public void Log(string message)
        => BinaryLibs.Utils.Log.Shared.Information($"Player {MyPlayer}: {message}");
}