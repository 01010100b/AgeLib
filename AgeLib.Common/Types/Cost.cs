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
    public readonly int Food = food;
    public readonly int Wood = wood;
    public readonly int Stone = stone;
    public readonly int Gold = gold;
}