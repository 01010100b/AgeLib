using AgeLib.AiModule.Engine.Structs;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace AgeLib.AiModule.Engine;

public static class Receiver
{
    private static Game Game { get; } = new();
    private static bool Initialized { get; set; } = false;

    public static void Receive(IntPtr expert_ptr)
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
            var newgame = Game.Initialize(expert_ptr);

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
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 10000; i++)
        {
            var regicide = Game.Execute("regicide-game");
        }
        
        Log.Shared.Debug($"took {sw.Elapsed}");
    }
}
