using System.Reflection;
using Xunit;

namespace AgeLib.Engine.Tests;

public class LoaderTests
{
    [Fact]
    public void Create_ValidBotAssembly_ReturnsBotInstance()
    {
        var dll = Assembly.GetExecutingAssembly().Location;

        var bot = Loader.Create(dll);

        var typed_bot = Assert.IsAssignableFrom<IBot>(bot);
        Assert.Equal(nameof(TestBot), typed_bot.GetType().Name);
    }

    [Fact]
    public void Create_CalledTwice_ReturnsDistinctInstances()
    {
        var dll = Assembly.GetExecutingAssembly().Location;

        var first = Loader.Create(dll);
        var second = Loader.Create(dll);

        Assert.NotSame(first, second);
    }
}
