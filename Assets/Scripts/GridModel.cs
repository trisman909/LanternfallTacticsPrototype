using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public sealed class GridModel
    {
        public readonly int Width;
        public readonly int Height;
        readonly bool[,] floors;

        public GridModel(int width, int height)
        {
            Width = width; Height = height; floors = new bool[width, height];
        }

        public bool InBounds(Vector2Int p) => p.x >= 0 && p.y >= 0 && p.x < Width && p.y < Height;
        public bool IsFloor(Vector2Int p) => InBounds(p) && floors[p.x, p.y];
        public void SetFloor(Vector2Int p, bool value = true) { if (InBounds(p)) floors[p.x, p.y] = value; }
        public IEnumerable<Vector2Int> Floors()
        {
            for (int y = 0; y < Height; y++) for (int x = 0; x < Width; x++) if (floors[x, y]) yield return new Vector2Int(x, y);
        }
        public IEnumerable<Vector2Int> Neighbors(Vector2Int p)
        {
            Vector2Int[] d = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            foreach (var v in d) if (IsFloor(p + v)) yield return p + v;
        }
        public HashSet<Vector2Int> Reachable(Vector2Int start, int range, Func<Vector2Int, bool> blocked)
        {
            var found = new HashSet<Vector2Int> { start };
            var q = new Queue<(Vector2Int p, int d)>(); q.Enqueue((start, 0));
            while (q.Count > 0)
            {
                var n = q.Dequeue(); if (n.d == range) continue;
                foreach (var next in Neighbors(n.p))
                    if (!found.Contains(next) && !blocked(next)) { found.Add(next); q.Enqueue((next, n.d + 1)); }
            }
            found.Remove(start); return found;
        }
        public List<Vector2Int> ShortestPath(Vector2Int start, Vector2Int goal, Func<Vector2Int, bool> blocked)
        {
            var prev = new Dictionary<Vector2Int, Vector2Int>(); var q = new Queue<Vector2Int>(); var seen = new HashSet<Vector2Int>{start}; q.Enqueue(start);
            while (q.Count > 0)
            {
                var p = q.Dequeue(); if (p == goal) break;
                foreach (var n in Neighbors(p)) if (!seen.Contains(n) && (!blocked(n) || n == goal)) { seen.Add(n); prev[n] = p; q.Enqueue(n); }
            }
            var path = new List<Vector2Int>(); if (!seen.Contains(goal)) return path;
            for (var p = goal; p != start; p = prev[p]) path.Add(p); path.Reverse(); return path;
        }

        public int WeightedDistance(Vector2Int start, Vector2Int goal, Func<Vector2Int,bool> blocked, Func<Vector2Int,int> traversalCost)
        {
            var distance=new Dictionary<Vector2Int,int>{{start,0}};
            var open=new HashSet<Vector2Int>{start};
            while(open.Count>0)
            {
                Vector2Int current=start; int best=int.MaxValue;
                foreach(var p in open)if(distance[p]<best){current=p;best=distance[p];}
                open.Remove(current);
                if(current==goal)return best;
                foreach(var next in Neighbors(current))
                {
                    if(blocked(next)&&next!=goal)continue;
                    int candidate=best+Mathf.Max(1,traversalCost(next));
                    if(!distance.TryGetValue(next,out int known)||candidate<known){distance[next]=candidate;open.Add(next);}
                }
            }
            return int.MaxValue;
        }
    }
}
