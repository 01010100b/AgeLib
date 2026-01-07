using AgeLib.Common.Types;
using AgeLib.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Map
{
    public int Width { get; private set; } = 0;
    public int Height { get; private set; } = 0;

    public void Update(IEngine engine)
    {
        var point = new Point(10_000, 10_000);
        engine.SetPoint(100, point);
        engine.Execute("up-bound-point", 100, 100);
        point = engine.GetPoint(100);
        var width = point.X + 1;
        var height = point.Y + 1;

        if (width != Width || height != Height)
        {
            Width = width; 
            Height = height;
        }
    }
}
