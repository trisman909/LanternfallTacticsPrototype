using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class GeneratedRoom
    {
        public GridModel Grid;
        public Vector2Int PlayerSpawn;
        public List<Vector2Int> EnemySpawns = new();
        public BiomeTheme Theme;
        public HashSet<Vector2Int> HazardTiles = new();
        public HashSet<Vector2Int> PropTiles = new();
    }

    public sealed class RoomGenerator
    {
        public GeneratedRoom Generate(int seed, int roomNumber)
        {
            var rng = new System.Random(seed);
            var grid = new GridModel(9, 11);
            var p = new Vector2Int(4, 1); grid.SetFloor(p);
            for (int i = 0; i < 48; i++)
            {
                var dirs = new[]{Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left};
                var next = p + dirs[rng.Next(dirs.Length)];
                next.x = Mathf.Clamp(next.x, 1, 7); next.y = Mathf.Clamp(next.y, 1, 9);
                p = next; grid.SetFloor(p);
                if (rng.NextDouble() < .35) foreach (var n in dirs) grid.SetFloor(p + n);
            }
            // Ensure a readable central spine and generous combat pockets.
            for (int y = 1; y <= 9; y++) for (int x = 2; x <= 6; x++) if (x == 4 || y % 3 != 0) grid.SetFloor(new Vector2Int(x, y));
            var player = new Vector2Int(4, 1); grid.SetFloor(player);
            var candidates = grid.Floors().Where(v => v.y >= 6).OrderBy(_ => rng.Next()).ToList();
            int count = BalanceConfig.EnemyCount(roomNumber);
            var enemies=candidates.Take(count).ToList();
            var dressing=grid.Floors().Where(v=>v!=player&&!enemies.Contains(v)&&v.y>2).OrderBy(_=>rng.Next()).ToList();
            return new GeneratedRoom { Grid = grid, PlayerSpawn = player, EnemySpawns = enemies, Theme=BiomeCatalog.ForRoom(roomNumber), HazardTiles=dressing.Take(5).ToHashSet(), PropTiles=dressing.Skip(5).Take(3).ToHashSet() };
        }

        public bool IsConnected(GridModel grid)
        {
            var all = grid.Floors().ToList(); if (all.Count == 0) return false;
            var seen = new HashSet<Vector2Int>{all[0]}; var q = new Queue<Vector2Int>(); q.Enqueue(all[0]);
            while(q.Count > 0) foreach(var n in grid.Neighbors(q.Dequeue())) if(seen.Add(n)) q.Enqueue(n);
            return seen.Count == all.Count;
        }
    }
}
