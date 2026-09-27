using AgeLib.Common.Types;
using Xunit;

namespace AgeLib.Common.Tests.Types;

public class CostTests
{
    [Fact]
    public void Addition_AddsAllComponents()
    {
        var a = new Cost(1, 2, 3, 4);
        var b = new Cost(10, 20, 30, 40);

        var result = a + b;

        Assert.Equal(11, result.Food);
        Assert.Equal(22, result.Wood);
        Assert.Equal(33, result.Stone);
        Assert.Equal(44, result.Gold);
    }

    [Fact]
    public void Subtraction_SubtractsAllComponents()
    {
        var a = new Cost(10, 20, 30, 40);
        var b = new Cost(1, 2, 3, 4);

        var result = a - b;

        Assert.Equal(9, result.Food);
        Assert.Equal(18, result.Wood);
        Assert.Equal(27, result.Stone);
        Assert.Equal(36, result.Gold);
    }

    [Fact]
    public void Multiplication_ScalesAllComponents()
    {
        var cost = new Cost(1, 2, 3, 4);

        var result = cost * 3;

        Assert.Equal(3, result.Food);
        Assert.Equal(6, result.Wood);
        Assert.Equal(9, result.Stone);
        Assert.Equal(12, result.Gold);
    }

    [Fact]
    public void Division_DividesAllComponents()
    {
        var cost = new Cost(10, 20, 30, 40);

        var result = cost / 2;

        Assert.Equal(5, result.Food);
        Assert.Equal(10, result.Wood);
        Assert.Equal(15, result.Stone);
        Assert.Equal(20, result.Gold);
    }
}
