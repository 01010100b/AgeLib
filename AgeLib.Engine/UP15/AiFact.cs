using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct AiFact
{
    public readonly int Type;
    public readonly int Touched;
    public readonly int LastResult;
    public readonly byte Argc;
    public readonly void* Ptr;
    public readonly byte Arg1Type;
    public readonly byte Arg2Type;
    public readonly byte Arg3Type;
    public readonly byte Arg4Type;
}
