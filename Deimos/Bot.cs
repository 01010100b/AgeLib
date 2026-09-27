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
    internal int Tick { get; private set; } = 0;
    internal TimeSpan GameTime { get; private set; } = TimeSpan.Zero;
    internal Map Map { get; } = new();
    internal Town Town { get; } = new();
    internal Production Production { get; } = new();
    internal List<Player> Players { get; } = [];

    private Dictionary<int, Unit> Units { get; } = [];

    public void Update(IEngine engine)
    {
        var sw = Stopwatch.StartNew();
        Tick++;
        GameTime = TimeSpan.FromSeconds(engine.GetFact(engine.MyPlayer, FactId.GAME_TIME));
        Map.Update(engine);
        Town.Update(engine);

        UpdatePlayers(engine);
        UpdateUnits(engine);

        Production.Produce(engine);

        engine.ChatToAll($"I am Deimos {Random.Shared.Next(1000)}");
        engine.ChatToAll($"Took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }

    private void UpdatePlayers(IEngine engine)
    {
        if (Players.Count == 0)
        {
            for (int i = 0; i <= 8; i++)
            {
                if (engine.Check("player-valid", i))
                {
                    Players.Add(new(i, engine));
                }
            }
        }

        foreach (var player in Players)
        {
            player.Update(engine);
        }
    }

    private void UpdateUnits(IEngine engine)
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
                    unit.Update(this, engine);
                    units++;
                }
                else if (unit.Speed == 0 && buildings < MAX_UPDATES)
                {
                    unit.Update(this, engine);
                    buildings++;
                }
            }

            player.Units.Clear();
            ids.Clear();

            engine.FindUnits(player.Id, ObjectStatus.READY, ObjectList.ACTIVE, ids);        

            foreach (var id in ids)
            {
                if (!Units.ContainsKey(id))
                {
                    Units.Add(id, new(id, this, engine));
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
