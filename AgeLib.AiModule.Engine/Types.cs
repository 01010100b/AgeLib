using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Point(int x, int y)
{
    public readonly int X = x;
    public readonly int Y = y;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct Cost(int food, int wood, int stone, int gold)
{
    public readonly int Food = food;
    public readonly int Wood = wood;
    public readonly int Stone = stone;
    public readonly int Gold = gold;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct SearchState(int local_total, int local_last, int remote_total, int remote_last)
{
    public readonly int LocalTotal = local_total;
    public readonly int LocalLast = local_last;
    public readonly int RemoteTotal = remote_total;
    public readonly int RemoteLast = remote_last;
}
