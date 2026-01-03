using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Player
{
    private fixed byte Unknown0[4828];
    public readonly Ai* Ai;
}
