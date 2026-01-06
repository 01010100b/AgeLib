using AgeLib.Engine;
using AgeLib.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Deimos;

public class Bot : IBot
{
    internal IEngine Engine => CurrentEngine!;
    internal int Tick { get; private set; } = 0;
    internal TimeSpan GameTime { get; private set; } = TimeSpan.Zero;
    internal List<Player> Players { get; } = [];
    internal Production Production { get; } = new();

    private IEngine? CurrentEngine { get; set; } = null;
    private Dictionary<int, Unit> Units { get; } = [];

    public void Update(IEngine engine)
    {
        var sw = Stopwatch.StartNew();
        CurrentEngine = engine;
        Tick++;
        GameTime = TimeSpan.FromSeconds(engine.GetFact(engine.MyPlayer, FactId.GAME_TIME, 0));

        UpdatePlayers();
        UpdateUnits();

        engine.ChatToAll($"I am Deimos {Random.Shared.Next(1000)}");
        engine.ChatToAll($"Took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }

    private void UpdatePlayers()
    {
        if (Players.Count == 0)
        {
            for (int i = 0; i <= 8; i++)
            {
                if (Engine.Check("player-valid", i))
                {
                    Players.Add(new(i, Engine));
                }
            }
        }

        foreach (var player in Players)
        {
            player.Update(Engine);
        }
    }

    private void UpdateUnits()
    {
        const int MAX_UPDATES = 20;

        var ids = new List<int>();

        foreach (var player in Players)
        {
            var units = 0;
            var buildings = 0;
            player.Units.Sort((a, b) => a.LastUpdateTick.CompareTo(b.LastUpdateTick));

            for (int i = 0; i < player.Units.Count; i++)
            {
                if (units >= MAX_UPDATES && buildings >= MAX_UPDATES)
                {
                    break;
                }

                var unit = player.Units[i];

                if (unit.Speed != 0 && units < MAX_UPDATES)
                {
                    unit.Update(this);
                    units++;
                }
                else if (unit.Speed == 0 && buildings < MAX_UPDATES)
                {
                    unit.Update(this);
                    buildings++;
                }
            }

            player.Units.Clear();
            ids.Clear();

            Engine.FindUnits(player.Id, ObjectStatus.READY, ObjectList.ACTIVE, ids);

            foreach (var id in ids)
            {
                if (!Units.ContainsKey(id))
                {
                    Units.Add(id, new(id, this));
                }
            }
        }

        var alive = Units.Values.Where(x => x.Exists).ToList();
        Units.Clear();

        foreach (var unit in alive)
        {
            var player = Players.Single(x => x.Id == unit.Player);
            player.Units.Add(unit);
            Units.Add(unit.Id, unit);
        }
    }
}
