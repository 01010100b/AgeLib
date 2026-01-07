using AgeLib.Common;
using AgeLib.Common.Enums;
using AgeLib.Common.Types;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine;

internal abstract class EngineBase : IEngine
{
    private const int CUSTOM_STRING_ID = 89733;
    private const int TEMP_GOAL = 123;

    public abstract int Version { get; }
    public int MyPlayer { get; internal set; } = -1;

    protected Dictionary<string, Command> Commands { get; } = [];
    protected HashSet<string> Symbols { get; } = [];

    public abstract bool Initialize(IntPtr config_ptr);
    public abstract void SetCustomString(string str);

    public abstract int GetStrategicNumber(int sn);
    public abstract void SetStrategicNumber(int sn, int value);

    public int GetStrategicNumber(StrategicNumber sn)
        => GetStrategicNumber((int)sn);

    public void SetStrategicNumber(StrategicNumber sn, int value)
        => SetStrategicNumber((int)sn, value);

    public abstract int GetGoal(int goal);
    public abstract void SetGoal(int goal, int value);

    public virtual Point GetPoint(int goal)
    {
        Assert.That(goal >= 41 && goal <= 511);

        return new(GetGoal(goal), GetGoal(goal + 1));
    }

    public virtual void SetPoint(int goal, Point point)
    {
        Assert.That(goal >= 41 && goal <= 511);

        SetGoal(goal, point.X);
        SetGoal(goal + 1, point.Y);
    }

    public virtual Cost GetCost(int goal)
    {
        Assert.That(goal >= 41 && goal <= 509);

        return new(GetGoal(goal), GetGoal(goal + 1), GetGoal(goal + 2), GetGoal(goal + 3));
    }

    public virtual void SetCost(int goal, Cost cost)
    {
        Assert.That(goal >= 41 && goal <= 509);

        SetGoal(goal, cost.Food);
        SetGoal(goal + 1, cost.Wood);
        SetGoal(goal + 2, cost.Stone);
        SetGoal(goal + 3, cost.Gold);
    }

    public virtual SearchState GetSearchState(int goal)
    {
        Assert.That(goal >= 41 && goal <= 509);

        return new(GetGoal(goal), GetGoal(goal + 1), GetGoal(goal + 2), GetGoal(goal + 3));
    }

    public virtual void SetSearchState(int goal, SearchState search_state)
    {
        Assert.That(goal >= 41 && goal <= 509);

        SetGoal(goal, search_state.LocalTotal);
        SetGoal(goal + 1, search_state.LocalLast);
        SetGoal(goal + 2, search_state.RemoteTotal);
        SetGoal(goal + 3, search_state.RemoteLast);
    }

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
        Assert.That(player >= 0 && player <= 8);

        SetCustomString(str);
        Execute("up-chat-data-to-player", player, CUSTOM_STRING_ID, type_op, value);
    }

    public int GetFact(int player, FactId fact, int parameter = 0)
    {
        Assert.That(player >= 0 && player <= 8);

        Execute("up-get-player-fact", player, (int)fact, parameter, TEMP_GOAL);

        return GetGoal(TEMP_GOAL);
    }

    public int GetObjectData(ObjectData data)
    {
        Execute("up-get-object-data", (int)data, TEMP_GOAL);

        return GetGoal(TEMP_GOAL);
    }

    public void FindUnits(int player, ObjectStatus status, ObjectList list, List<int> ids)
    {
        Assert.That(player >= 0 && player <= 8);

        SetStrategicNumber(StrategicNumber.FOCUS_PLAYER_NUMBER, player);
        Execute("up-full-reset-search");
        Execute("up-filter-status", TypeOp.C, (int)status, TypeOp.C, (int)list);

        while (true)
        {
            Execute("up-reset-search", 0, 0, 0, 1);
            Execute("up-find-status-remote", TypeOp.C, -1, TypeOp.C, Constants.MAX_REMOTE_SEARCH);
            Execute("up-get-search-state", TEMP_GOAL);
            var remote = GetGoal(TEMP_GOAL + 2);

            for (int i = 0; i < remote; i++)
            {
                Execute("up-set-target-object", (int)SearchSource.REMOTE, TypeOp.C, i);
                Execute("up-get-object-data", (int)ObjectData.ID, TEMP_GOAL);
                var id = GetGoal(TEMP_GOAL);
                ids.Add(id);
            }

            if (remote < 40)
            {
                break;
            }
        }
    }

    public void Log(string message)
        => BinaryLibs.Utils.Log.Shared.Information($"Player {MyPlayer}: {message}");
}