using AgeLib.AiModule.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

public class Bot : IBot
{
    public void Tick(IEngine engine)
    {
        engine.ChatToAll($"I am Deimos {Random.Shared.Next(1000)}");
    }
}
