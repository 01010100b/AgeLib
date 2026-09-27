using System;
using Xunit;

namespace AgeLib.Engine.Tests;

public class CommandTests
{
    [Fact]
    public void Constructor_EmptyName_Throws()
    {
        Assert.Throws<Exception>(() => new Command("", (IntPtr)1, 1, false));
    }

    [Fact]
    public void Constructor_ZeroFunctionPointer_Throws()
    {
        Assert.Throws<Exception>(() => new Command("cmd", IntPtr.Zero, 1, false));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void Constructor_ArgcOutOfRange_Throws(int argc)
    {
        Assert.Throws<Exception>(() => new Command("cmd", (IntPtr)1, argc, false));
    }

    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        var command = new Command("cmd", (IntPtr)1, 2, true);

        Assert.Equal("cmd", command.Name);
        Assert.Equal((IntPtr)1, command.Function);
        Assert.Equal(2, command.Argc);
        Assert.True(command.IsFact);
    }
}
