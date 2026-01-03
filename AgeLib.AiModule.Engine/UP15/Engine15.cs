using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine.UP15;

internal class Engine15 : EngineBase
{
    public override int Version => 15;

    private IntPtr ExpertPtr { get; set; } = IntPtr.Zero;
    private IntPtr GamePtr { get; set; } = IntPtr.Zero;
    private IntPtr CustomStringPtr { get; set; } = IntPtr.Zero;

    public override bool Initialize(IntPtr config_ptr)
    {
        var config = Marshal.PtrToStructure<Config>(config_ptr);
        Assert.That(config.ExpertPtr != IntPtr.Zero);
        Assert.That(config.GamePtr != IntPtr.Zero);
        Assert.That(config.CustomStringPtr != IntPtr.Zero);

        if (ExpertPtr == config.ExpertPtr)
        {
            return false;
        }

        ExpertPtr = config.ExpertPtr;
        GamePtr = config.GamePtr;
        CustomStringPtr = config.CustomStringPtr;
        
        unsafe
        {
            var expert = Marshal.PtrToStructure<AiExpert>(ExpertPtr);
            var has_symbol = false;

            for (int table = 0; table < expert.GlobalSymbolTableSize; table++)
            {
                var node = expert.Symbols[table];

                while (node is not null)
                {
                    if (node->Type == 5)
                    {
                        var name = Marshal.PtrToStringAnsi((IntPtr)node->Text) ?? throw new Exception();

                        if (name == "UP-AVAILABLE")
                        {
                            has_symbol = true;

                            break;
                        }
                    }

                    node = node->Next;
                }

                if (has_symbol)
                {
                    break;
                }
            }

            if (!has_symbol)
            {
                return false;
            }

            Commands.Clear();
            Symbols.Clear();

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
                    else if (type == 5)
                    {
                        Symbols.Add(name);
                    }

                    node = node->Next;
                }
            }
        }
        
        return true;
    }

    public override void SetCustomString(string str)
    {
        var bytes = Encoding.ASCII.GetBytes(str);
        var length = Math.Min(255, bytes.Length);

        unsafe
        {
            var ptr = (byte*)CustomStringPtr;

            for (int i = 0; i < length; i++)
            {
                ptr[i] = bytes[i];
            }

            ptr[length] = 0;
        }
    }

    public override int GetGoal(int goal)
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

    public override void SetGoal(int goal, int value)
    {
        Assert.That(goal >= 1 && goal <= 512);

        goal--;

        unsafe
        {
            var ai = GetAi(MyPlayer);

            if (goal < 40)
            {
                ai->BaseGoals[goal] = value;
            }
            else
            {
                ai->ExtendedGoals[4 + goal - 40] = value;
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
