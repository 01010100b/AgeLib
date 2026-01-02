using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.Structs;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct World
{
    private fixed byte Unknown0[76];
    public readonly Player** Players;
}
