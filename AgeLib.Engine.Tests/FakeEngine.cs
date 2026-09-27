using System;

namespace AgeLib.Engine.Tests;

// Minimal EngineBase implementation backed by an in-memory goal store, used to exercise the virtual members.
internal sealed class FakeEngine : EngineBase
{
    public override int Version => -1;

    private int[] Goals { get; } = new int[513];

    public override bool Initialize(IntPtr config_ptr)
        => false;

    public override void SetCustomString(string str)
    {
    }

    public override int GetStrategicNumber(int sn)
        => 0;

    public override void SetStrategicNumber(int sn, int value)
    {
    }

    public override int GetGoal(int goal)
        => Goals[goal];

    public override void SetGoal(int goal, int value)
        => Goals[goal] = value;

    public void AddSymbol(string symbol)
        => Symbols.Add(symbol);
}
