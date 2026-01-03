using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Common.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Point(int x, int y)
{
    public readonly int X = x;
    public readonly int Y = y;
}