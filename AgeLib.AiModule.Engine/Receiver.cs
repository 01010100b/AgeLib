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
    private static string Folder { get; } = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new Exception();
    private static ExpertEngine Engine { get; } = new();
    private static Dictionary<int, IBot> Bots { get; } = [];

    static Receiver()
    {
        var file = Path.Combine(Folder, "agelib-ai-module.log");
        Log.Shared.AddFileListener(file);
        Log.Shared.Information($"Folder: {Folder}");
    }

    public static void Receive(IntPtr expert_ptr, IntPtr game_ptr)
    {
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
        Log.Shared.Information($"Starting new game");

        Bots.Clear();

        for (int i = 1; i <= 8; i++)
        {
            Bots.Add(i, new TestBot());
        }
    }

    private static void Tick()
    {
        Engine.MyPlayer = -1;

        for (int i = 1; i <= 8; i++)
        {
            if (Engine.Execute("player-number", i) == 1)
            {
                Engine.MyPlayer = i;

                break;
            }
        }

        if (Engine.MyPlayer == -1 || !Bots.TryGetValue(Engine.MyPlayer, out var bot))
        {
            return;
        }

        bot.Tick(Engine);
    }
}
