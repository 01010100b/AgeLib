using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.UP15;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Game
{
    private fixed byte Unknown0[40];
    public readonly void* ProgramInformation;
    private fixed byte Unknown1[436];
    public readonly int ProgramModeId;
    private fixed byte Unknown2[52];
    public fixed byte WorkingDirectory[261];
    private fixed byte Unknown3[263];
    public readonly World* World;
}
