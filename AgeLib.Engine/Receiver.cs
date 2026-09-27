
using AgeLib.Engine.UP15;
using BinaryLibs.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine;

public static class Receiver
{
    private static string Folder { get; } = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new Exception();
    private static EngineBase Engine { get; set; } = new Engine15();
    private static Dictionary<int, IBot> Bots { get; } = [];
    private static long ReceiveCallCount;
    private static long MissingBotLogCount;
    private static long BotUpdateCount;

    static Receiver()
    {
        var file = Path.Combine(Folder, "agelib-engine.log");
        Log.Shared.AddFileListener(file);
        Log.Shared.Information($"Folder: {Folder}");
        Log.Shared.Information("Receiver static constructor completed.");
    }

    [UnmanagedCallersOnly(EntryPoint = "Receive")]
    public static void Receive(int version, IntPtr config)
    {
        var receive_call_count = Interlocked.Increment(ref ReceiveCallCount);
        var log_heartbeat = receive_call_count == 1 || receive_call_count % 500 == 0;
        if (log_heartbeat)
        {
            Log.Shared.Information($"Receiver.Receive entered. Call={receive_call_count}, version={version}, config=0x{config.ToInt64():X}.");
        }

        try
        {
            if (version != Engine.Version)
            {
                Engine = version switch
                {
                    15 => new Engine15(),
                    _ => throw new NotSupportedException($"Version {version} is not supported.")
                };
            }

            var newgame = Engine.Initialize(config);
            if (newgame || log_heartbeat)
            {
                Log.Shared.Information($"Engine.Initialize completed. NewGame={newgame}.");
            }

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
            Log.Shared.Information($"Receiver.Receive failed: {e}");
            Log.Shared.Exception(e);
        }
    }

    private static void StartNewGame()
    {
        Log.Shared.Information($"Starting new game");

        Bots.Clear();
        var file = Path.Combine(Folder, "agelib-engine.config");
        Log.Shared.Information($"Loading bot configuration: {file}. Exists={File.Exists(file)}.");
        var players = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(file)) ?? throw new Exception();
        Log.Shared.Information($"Loaded {players.Count} bot configuration entries.");
        
        foreach (var player in players)
        {
            Log.Shared.Information($"Loading bot for player {player.Key} from '{player.Value}'.");
            var bot = Loader.Create(player.Value);
            Bots.Add(player.Key, bot);
            Log.Shared.Information($"Bot for player {player.Key} loaded as {bot.GetType().FullName}.");
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
            var missing_bot_log_count = Interlocked.Increment(ref MissingBotLogCount);
            if (missing_bot_log_count == 1 || missing_bot_log_count % 500 == 0)
            {
                Log.Shared.Information($"No configured bot for detected player {Engine.MyPlayer} (occurrence {missing_bot_log_count}).");
            }

            return;
        }

        var bot_update_count = Interlocked.Increment(ref BotUpdateCount);
        if (bot_update_count == 1 || bot_update_count % 100 == 0)
        {
            Log.Shared.Information($"Calling bot.Update for player {Engine.MyPlayer}: {bot.GetType().FullName} (call {bot_update_count}).");
        }

        bot.Update(Engine);
    }
}
