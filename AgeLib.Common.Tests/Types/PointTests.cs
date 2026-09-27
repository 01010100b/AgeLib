using AgeLib.Common.Types;
using Xunit;

namespace AgeLib.Common.Tests.Types;

public class PointTests
{
    [Fact]
    public void Equals_SameCoordinates_ReturnsTrue()
    {
        var a = new Point(3, 4);
        var b = new Point(3, 4);

        Assert.True(a.Equals(b));
        Assert.True(a == b);
    }

    [Fact]
    public void Equals_DifferentCoordinates_ReturnsFalse()
    {
        var a = new Point(3, 4);
        var b = new Point(3, 5);

        Assert.False(a.Equals(b));
        Assert.True(a != b);
    }

    [Fact]
    public void GetHashCode_SameCoordinates_ReturnsSameHash()
    {
        var a = new Point(3, 4);
        var b = new Point(3, 4);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToPrecise_ScalesCoordinatesUp()
    {
        var point = new Point(3, 4);

        var precise = point.ToPrecise();

        Assert.Equal(300, precise.X);
        Assert.Equal(400, precise.Y);
    }

    [Fact]
    public void FromPrecise_ScalesCoordinatesDown()
    {
        var point = new Point(300, 400);

        var imprecise = point.FromPrecise();

        Assert.Equal(3, imprecise.X);
        Assert.Equal(4, imprecise.Y);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0.0)]
    [InlineData(0, 0, 3, 4, 5.0)]
    [InlineData(1, 1, 4, 5, 5.0)]
    public void DistanceTo_ReturnsEuclideanDistance(int x1, int y1, int x2, int y2, double expected)
    {
        var a = new Point(x1, y1);
        var b = new Point(x2, y2);

        Assert.Equal(expected, a.DistanceTo(b));
    }
}
