using System.Collections.Generic;
using System.Linq;
using AgeLib.Common.Types;
using Xunit;

namespace AgeLib.Engine.Tests;

public class EngineBaseTests
{
    private readonly FakeEngine _engine = new();

    [Theory]
    [InlineData(40)]
    [InlineData(512)]
    public void GetPoint_GoalOutOfRange_Throws(int goal)
    {
        Assert.Throws<Exception>(() => _engine.GetPoint(goal));
    }

    [Fact]
    public void GetPoint_ValidGoal_ReadsUnderlyingGoals()
    {
        _engine.SetGoal(100, 5);
        _engine.SetGoal(101, 7);

        var point = _engine.GetPoint(100);

        Assert.Equal(5, point.X);
        Assert.Equal(7, point.Y);
    }

    [Fact]
    public void SetPoint_ValidGoal_WritesUnderlyingGoals()
    {
        _engine.SetPoint(100, new Point(3, 4));

        Assert.Equal(3, _engine.GetGoal(100));
        Assert.Equal(4, _engine.GetGoal(101));
    }

    [Fact]
    public void GetCost_ValidGoal_ReadsUnderlyingGoals()
    {
        _engine.SetGoal(100, 1);
        _engine.SetGoal(101, 2);
        _engine.SetGoal(102, 3);
        _engine.SetGoal(103, 4);

        var cost = _engine.GetCost(100);

        Assert.Equal(1, cost.Food);
        Assert.Equal(2, cost.Wood);
        Assert.Equal(3, cost.Stone);
        Assert.Equal(4, cost.Gold);
    }

    [Theory]
    [InlineData(40)]
    [InlineData(510)]
    public void SetCost_GoalOutOfRange_Throws(int goal)
    {
        Assert.Throws<Exception>(() => _engine.SetCost(goal, new Cost(1, 2, 3, 4)));
    }

    [Fact]
    public void GetSearchState_ValidGoal_ReadsUnderlyingGoals()
    {
        _engine.SetGoal(100, 1);
        _engine.SetGoal(101, 2);
        _engine.SetGoal(102, 3);
        _engine.SetGoal(103, 4);

        var state = _engine.GetSearchState(100);

        Assert.Equal(1, state.LocalTotal);
        Assert.Equal(2, state.LocalLast);
        Assert.Equal(3, state.RemoteTotal);
        Assert.Equal(4, state.RemoteLast);
    }

    [Fact]
    public void Execute_UnknownCommand_ThrowsKeyNotFound()
    {
        Assert.Throws<KeyNotFoundException>(() => _engine.Execute("missing"));
    }

    [Fact]
    public void IsSymbolDefined_KnownSymbol_ReturnsTrue()
    {
        _engine.AddSymbol("UP-AVAILABLE");

        Assert.True(_engine.IsSymbolDefined("UP-AVAILABLE"));
    }

    [Fact]
    public void IsSymbolDefined_UnknownSymbol_ReturnsFalse()
    {
        Assert.False(_engine.IsSymbolDefined("MISSING"));
    }

    [Fact]
    public void GetSymbols_ReturnsAddedSymbols()
    {
        _engine.AddSymbol("A");
        _engine.AddSymbol("B");

        Assert.Equal(["A", "B"], _engine.GetSymbols().OrderBy(x => x));
    }
}
