using AgeLib.AiModule.Engine.Structs;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal class ExpertEngine
{
    public int MyPlayer { get; internal set; } = -1;

    private IntPtr ExpertPtr { get; set; } = IntPtr.Zero;
    private IntPtr GamePtr { get; set; } = IntPtr.Zero;
    private Dictionary<string, Command> Commands { get; } = [];

    public bool Initialize(IntPtr expert_ptr, IntPtr game_ptr)
    {
        if (ExpertPtr == expert_ptr)
        {
            return false;
        }

        ExpertPtr = expert_ptr;
        GamePtr = game_ptr;
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

    public int GetGoal(int goal)
    {
        Assert.That(goal >= 1 && goal <= 512);

        goal--;

        unsafe
        {
            var ai = GetAi(MyPlayer);

            if (goal < 40)
            {
                return ai->BaseGoals[goal];
            }
            else
            {
                return ai->ExtendedGoals[4 + goal - 40];
            }
        }
    }

    private unsafe Ai* GetAi(int player)
    {
        var game = *(Game**)GamePtr;
        var world = game->World;
        var p = world->Players[player];

        return p->Ai;
    }
}
