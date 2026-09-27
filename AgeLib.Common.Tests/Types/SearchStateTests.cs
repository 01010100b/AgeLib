using AgeLib.Common.Types;
using Xunit;

namespace AgeLib.Common.Tests.Types;

public class SearchStateTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var state = new SearchState(1, 2, 3, 4);

        Assert.Equal(1, state.LocalTotal);
        Assert.Equal(2, state.LocalLast);
        Assert.Equal(3, state.RemoteTotal);
        Assert.Equal(4, state.RemoteLast);
    }
}
