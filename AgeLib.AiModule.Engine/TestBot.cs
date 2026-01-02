using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal class TestBot : IBot
{
    public void Tick(IEngine engine)
    {
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < 10_000; i++)
        {
            engine.Execute("set-goal", 171, 237);
            var val = engine.GetGoal(171);
        }
        
        engine.Log($"time {sw.Elapsed}");
    }
}
