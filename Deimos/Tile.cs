using AgeLib.Common;
using AgeLib.Common.Enums;
using AgeLib.Common.Types;
using AgeLib.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Tile(Point position)
{
    public Point Position { get; } = position;
    public bool Explored { get; private set; } = false;
    public int Elevation { get; private set; } = 0;
    public int Terrain { get; private set; } = 0;
    public int Zone { get; private set; } = 0;

    public void Update(IEngine engine)
    {
        engine.SetPoint(100, Position);

        if (engine.Check("up-point-explored", 100, CompareOp.C_NOT_EQUAL, (int)ExploredState.NO))
        {
            Explored = true;
            engine.Execute("up-get-point-elevation", 100, 105);
            Elevation = engine.GetGoal(105);
            engine.Execute("up-get-point-terrain", 100, 105);
            Terrain = engine.GetGoal(105);
            engine.Execute("up-get-point-zone", 100, 105);
            Zone = engine.GetGoal(105);
        }
    }
}
