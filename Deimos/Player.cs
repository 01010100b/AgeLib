using AgeLib.Common.Enums;
using AgeLib.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Player
{
    public int Id { get; }
    public bool InGame { get; private set; } = true;
    public List<Unit> Units { get; } = [];

    public Player(int id, IEngine engine)
    {
        Id = id;
        Update(engine);
    }

    public void Update(IEngine engine)
    {
        InGame = engine.GetFact(Id, FactId.PLAYER_IN_GAME) == 1;
    }
}
