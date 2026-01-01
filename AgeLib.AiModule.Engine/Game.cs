using AgeLib.AiModule.Engine.Structs;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal class Game
{
    private IntPtr LastInitialization { get; set; } = IntPtr.Zero;
    private Dictionary<string, Command> Commands { get; } = [];

    public bool Initialize(IntPtr expert_ptr)
    {
        if (LastInitialization == expert_ptr)
        {
            return false;
        }

        LastInitialization = expert_ptr;
        var expert = Marshal.PtrToStructure<AiExpert>(expert_ptr);
        Commands.Clear();

        unsafe
        {
            for (int table = 0; table < expert.GlobalSymbolTableSize; table++)
            {
                var node = expert.Symbols[table];

                while (node is not null)
                {
                    var name = Marshal.PtrToStringAnsi((IntPtr)node->Text) ?? throw new Exception();
                    var type = node->Type;

                    if (type == 0)
                    {
                        if (!Commands.ContainsKey(name))
                        {
                            var function = (IntPtr)expert.Actions[node->Id].Ptr;
                            var argc = expert.Actions[node->Id].Argc;
                            Commands.Add(name, new(name, function, argc, false));
                        }
                        
                    }
                    else if (type == 3)
                    {
                        var function = (IntPtr)expert.Facts[node->Id].Ptr;
                        var argc = expert.Facts[node->Id].Argc;
                        Commands[name] = new(name, function, argc, true);
                    }

                    node = node->Next;
                }
            }
        }

        return true;
    }

    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        => Execute(name, arg1, arg2, arg3, arg4) != 0;

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
    {
        var command = Commands[name];

        return command.Execute(arg1, arg2, arg3, arg4);
    }
}
