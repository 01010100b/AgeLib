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

internal class Town
{
    public Point Home { get; private set; } = new(0, 0);

    public void Update(IEngine engine)
    {
        UpdateHome(engine);
    }

    private void UpdateHome(IEngine engine)
    {
        engine.SetPoint(100, Home);

        if (engine.Check("up-point-explored", 100, CompareOp.C_EQUAL, (int)ExploredState.ACTIVE)
            && engine.Check("up-point-contains", 100, TypeOp.C, Objects.TownCenter))
        {
            return;
        }

        int results()
        {
            engine.Execute("up-get-search-state", 105);

            return engine.GetGoal(105);
        }

        engine.Execute("up-full-reset-search");
        engine.Execute("up-find-local", TypeOp.C, Objects.TownCenter, TypeOp.C, Constants.MAX_LOCAL_SEARCH);

        if (results() == 0)
        {
            engine.Execute("up-find-local", TypeOp.C, (int)UnitClass.BUILDING, TypeOp.C, Constants.MAX_LOCAL_SEARCH);
        }

        if (results() == 0)
        {
            engine.Execute("up-find-local", TypeOp.C, -1, TypeOp.C, Constants.MAX_LOCAL_SEARCH);
        }

        if (results() == 0)
        {
            return;
        }

        engine.Execute("up-clean-search", (int)SearchSource.LOCAL, (int)ObjectData.ID, (int)SearchOrder.ASCENDING);
        engine.Execute("up-set-target-object", (int)SearchSource.LOCAL, TypeOp.C, 0);
        var x = engine.GetObjectData(ObjectData.POINT_X);
        var y = engine.GetObjectData(ObjectData.POINT_Y);
        Home = new(x, y);
    }
}
