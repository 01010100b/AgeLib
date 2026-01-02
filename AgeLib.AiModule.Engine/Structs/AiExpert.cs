using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct AiExpert
{
    public readonly void* Ptr;
    public readonly short MaxStrings;
    public readonly short NumStrings;
    public readonly byte** Strings;
    public readonly short MaxFacts;
    public readonly short NumFacts;
    public readonly AiFact* Facts;
    public readonly short MaxActions;
    public readonly short NumActions;
    public readonly AiAction* Actions;
    public readonly short MaxLists;
    public readonly void* ListInfo;
    public readonly void* GroupTable;
    public readonly void* CurrentRule;
    public readonly void* CurrentList;
    public readonly void* CurrentGroupTable;
    public readonly int GlobalSymbolTableSize;
    public readonly SymbolNode** Symbols;
}
