using AgeLib.AiModule.Engine.V15;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal class ExpertEngine : IEngine
{
    public int MyPlayer { get; internal set; } = -1;

    private IntPtr ExpertPtr { get; set; } = IntPtr.Zero;
    private IntPtr GamePtr { get; set; } = IntPtr.Zero;
    private Dictionary<string, Command> Commands { get; } = [];
    private Dictionary<string, int> Strings { get; } = [];
    private HashSet<string> Symbols { get; } = [];

    public bool Initialize(IntPtr expert_ptr, IntPtr game_ptr)
    {
        if (ExpertPtr == expert_ptr)
        {
            return false;
        }

        ExpertPtr = expert_ptr;
        GamePtr = game_ptr;
        var expert = Marshal.PtrToStructure<AiExpert>(expert_ptr);
        
        unsafe
        {
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
        }
        
        Strings.Clear();
        Commands.Clear();
        Symbols.Clear();

        unsafe
        {
            for (int i = 0; i < expert.NumStrings; i++)
            {
                var str = Marshal.PtrToStringAnsi((IntPtr)expert.Strings[i]) ?? throw new Exception();
                Strings[str] = i;
            }

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

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
    {
        var command = Commands[name];

        return command.Execute(arg1, arg2, arg3, arg4);
    }

    public int GetStringId(string str) 
        => Strings[str];

    public IReadOnlySet<string> GetSymbols()
        => Symbols;

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

    public void SetGoal(int goal, int value)
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

    public void Log(string message)
    {
        BinaryLibs.Utils.Log.Shared.Information($"Player {MyPlayer}: {message}");
    }

    private unsafe Ai* GetAi(int player)
    {
        var game = *(Game**)GamePtr;
        var world = game->World;
        var p = world->Players[player];

        return p->Ai;
    }
}
