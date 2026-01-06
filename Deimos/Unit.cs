using AgeLib.Engine;
using AgeLib.Common.Enums;
using AgeLib.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Unit
{
    public int Id { get; }
    public bool Exists { get; private set; } = true;
    public int LastUpdateTick { get; private set; } = -1;
    public TimeSpan LastUpdateGameTime { get; private set; } = TimeSpan.Zero;
    public int Player { get; private set; } = -2;
    public int BaseType { get; private set; } = -2;
    public int UpogradeType { get; private set; } = -2;
    public Point PrecisePosition { get; private set; }
    public int TargetId { get; private set; } = -1;

    public Unit(int id, Bot bot)
    {
        Id = id;
        Update(bot);
    }

    public void Update(Bot bot)
    {
        if (LastUpdateTick == bot.Tick)
        {
            return;
        }

        LastUpdateTick = bot.Tick;
        LastUpdateGameTime = bot.GameTime;
        var engine = bot.Engine;

        if (!engine.Check("up-set-target-by-id", TypeOp.C, Id))
        {
            Exists = false;

            return;
        }

        Player = engine.GetObjectData(ObjectData.PLAYER);
        BaseType = engine.GetObjectData(ObjectData.BASE_TYPE);
        UpogradeType = engine.GetObjectData(ObjectData.UPGRADE_TYPE);
        var x = engine.GetObjectData(ObjectData.PRECISE_X);
        var y = engine.GetObjectData(ObjectData.PRECISE_Y);
        PrecisePosition = new Point(x, y);
        TargetId = engine.GetObjectData(ObjectData.TARGET_ID);

        return;
    }
}
