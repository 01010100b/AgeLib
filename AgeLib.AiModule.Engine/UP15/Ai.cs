using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Ai
{
    private fixed byte Unknown0[5988];
    public fixed int BaseGoals[40];
    private fixed byte Unknown1[4596];
    public readonly int* ExtendedGoals;
}
