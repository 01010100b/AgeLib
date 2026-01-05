using AgeLib.AiModule.Engine;
using AgeLib.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

public class Bot : IBot
{
    internal IEngine Engine => CurrentEngine!;
    internal int Tick { get; private set; } = 0;
    internal TimeSpan GameTime { get; private set; } = TimeSpan.Zero;
    internal List<Player> Players { get; } = [];

    private IEngine? CurrentEngine { get; set; } = null;
    private Dictionary<int, Unit> Units { get; } = [];

    public void Update(IEngine engine)
    {
        engine.ChatToAll($"I am Deimos {Random.Shared.Next(1000)}");
        CurrentEngine = engine;
        Tick++;

        UpdatePlayers();
        UpdateUnits();

        engine.Log(JsonConvert.SerializeObject(Units, Formatting.Indented));
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
        var ids = new List<int>();

        foreach (var player in Players)
        {
            ids.Clear();

            Engine.FindUnits(player.Id, ObjectStatus.READY, ObjectList.ACTIVE, ids);

            foreach (var id in ids)
            {
                if (!Units.TryGetValue(id, out var unit))
                {
                    unit = new(id, this);
                    Units.Add(id, unit);
                }

                unit.Update(this);
            }
        }
    }
}
