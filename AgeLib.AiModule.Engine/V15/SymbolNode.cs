using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.V15;

[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct SymbolNode
{
    public readonly SymbolNode* Next;
    public readonly byte* Text;
    public readonly byte Type;
    public readonly ushort Id;
}
