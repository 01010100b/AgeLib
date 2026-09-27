using AgeLib.Common.Types;
using AgeLib.Engine;
using BinaryLibs.Utils;
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

    private Tile[] Tiles { get; set; } = [];

    public Tile GetTile(int x, int y)
    {
        Assert.That(x >= 0 && x < Width);
        Assert.That(y >= 0 && y < Height);

        return Tiles[GetIndex(x, y)];
    }

    public Tile GetTile(Point point)
        => GetTile(point.X, point.Y);

    public void Update(IEngine engine)
    {
        UpdateSize(engine);
        UpdateTiles(engine);
    }

    private void UpdateSize(IEngine engine)
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

            Tiles = new Tile[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var index = GetIndex(x, y);
                    Tiles[index] = new(new(x, y));
                }
            }
        }
    }

    private void UpdateTiles(IEngine engine)
    {
        if (Tiles.Length == 0)
        {
            return;
        }

        const int MAX_UPDATES = 200;

        for (int i = 0; i < MAX_UPDATES; i++)
        {
            var tile = Tiles[Random.Shared.Next(Tiles.Length)];

            if (tile.Explored)
            {
                tile = Tiles[Random.Shared.Next(Tiles.Length)];
            }

            tile.Update(engine);
        }
    }

    private int GetIndex(int x, int y)
        => y * Width + x;
}
