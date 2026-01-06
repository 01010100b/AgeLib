using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Common.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Cost(int food, int wood, int stone, int gold)
{
    public static Cost operator +(Cost a, Cost b)
        => new(a.Food + b.Food, a.Wood + b.Wood, a.Stone + b.Stone, a.Gold + b.Gold);

    public static Cost operator -(Cost a, Cost b)
        => new(a.Food - b.Food, a.Wood - b.Wood, a.Stone - b.Stone, a.Gold - b.Gold);

    public static Cost operator *(Cost a, int b)
        => new(a.Food * b, a.Wood * b, a.Stone * b, a.Gold * b);

    public static Cost operator /(Cost a, int b)
        => new(a.Food / b, a.Wood / b, a.Stone / b, a.Gold / b);

    public readonly int Food = food;
    public readonly int Wood = wood;
    public readonly int Stone = stone;
    public readonly int Gold = gold;
}