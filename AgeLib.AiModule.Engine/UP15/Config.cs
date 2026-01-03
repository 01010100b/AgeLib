using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct Config
{
    public readonly IntPtr ExpertPtr;
    public readonly IntPtr GamePtr;
    public readonly IntPtr CustomStringPtr;
}
