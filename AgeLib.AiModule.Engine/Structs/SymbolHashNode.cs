using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct SymbolHashNode
{
    public readonly SymbolHashNode* Next;
    public readonly byte* Text;
    public readonly byte Type;
    public readonly ushort Id;
}
