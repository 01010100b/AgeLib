using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Common.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Point(int x, int y)
{
    public static bool operator ==(Point a, Point b)
        => a.Equals(b);

    public static bool operator !=(Point a, Point b)
        => !(a == b);

    public readonly int X = x;
    public readonly int Y = y;

    public Point ToPrecise() => new(X * 100, Y * 100);
    public Point FromPrecise() => new(X / 100, Y / 100);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Point p && X == p.X && Y == p.Y;

    public override int GetHashCode()
        => HashCode.Combine(X, Y);
}