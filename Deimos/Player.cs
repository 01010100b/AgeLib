using AgeLib.AiModule.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Player
{
    public int Id { get; }
    public List<Unit> Units { get; } = [];

    public Player(int id, IEngine engine)
    {
        Id = id;
        Update(engine);
    }

    public void Update(IEngine engine)
    {

    }
}
