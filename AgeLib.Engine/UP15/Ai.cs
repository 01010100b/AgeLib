using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Ai
{
    private fixed byte Unknown0[5988];
    public fixed int BaseGoals[40];
    private fixed byte Unknown1[4596];
    public readonly int* ExtendedGoals; // start at index 4
    public readonly int* ExtendedStrategicNumbers; // start at index 44
    private fixed byte Unknown2[7476];
    public fixed int BaseStrategicNumbers[242];
}
