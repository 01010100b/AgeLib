using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public static class Receiver
{
    private static ExpertEngine Engine { get; } = new();
    private static bool Initialized { get; set; } = false;

    public static void Receive(IntPtr expert_ptr, IntPtr game_ptr)
    {
        if (!Initialized)
        {
            Log.Shared.Level = Log.LogLevel.TRACE;
            Log.Shared.AddFileListener(@"F:\text.txt");
            Initialized = true;
        }

        try
        {
            Log.Shared.Trace($"Received expert ptr {expert_ptr}");
            var newgame = Engine.Initialize(expert_ptr, game_ptr);

            if (newgame)
            {
                StartNewGame();
            }
            else
            {
                Tick();
            }
        }
        catch (Exception e)
        {
            Log.Shared.Exception(e);
        }
    }

    private static void StartNewGame()
    {

    }

    private static void Tick()
    {
        Engine.MyPlayer = -1;

        for (int i = 0; i <= 8; i++)
        {
            if (Engine.Check("player-number", i))
            {
                Engine.MyPlayer = i;
                Log.Shared.Debug($"Current player {Engine.MyPlayer}");

                break;
            }
        }

        if (Engine.MyPlayer == -1)
        {
            return;
        }

        Engine.Execute("set-goal", 171, 237);
        var val = Engine.GetGoal(171);
        Log.Shared.Debug($"goal {val}");
    }
}
