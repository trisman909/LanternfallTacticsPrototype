using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public static class EnemyAI
    {
        public static HashSet<Vector2Int> BuildPreview(EnemyModel e, Vector2Int player, GridModel grid)
        {
            var r = new HashSet<Vector2Int>();
            if (e.Kind == EnemyKind.GloomArcher)
            {
                var delta = player - e.Position;
                if (delta.x == 0 || delta.y == 0)
                {
                    var step = new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y)); var p = e.Position + step;
                    for(int i=0;i<4 && grid.IsFloor(p);i++,p+=step) r.Add(p);
                }
            }
            else
            {
                int radius = e.Kind == EnemyKind.LanternWarden ? (e.Health <= e.MaxHealth / 2 ? 3 : 2) : 1;
                foreach(var p in grid.Floors()) if(Mathf.Abs(p.x-e.Position.x)+Mathf.Abs(p.y-e.Position.y)<=radius && p!=e.Position) r.Add(p);
            }
            return r;
        }

        public static Vector2Int ChooseReposition(EnemyModel e, Vector2Int player, GridModel grid, System.Func<Vector2Int, bool> blocked, System.Func<Vector2Int, bool> hazard = null)
        {
            var candidates = grid.Reachable(e.Position, e.MoveRange, p => blocked(p) && p != e.Position).ToList();
            if (!candidates.Contains(e.Position)) candidates.Add(e.Position);
            var escapeTiles = grid.Floors().Where(p => Mathf.Abs(p.x - player.x) + Mathf.Abs(p.y - player.y) <= 2).ToHashSet();
            Vector2Int best = e.Position;
            int bestScore = int.MinValue;
            foreach (var c in candidates)
            {
                var ghost = new EnemyModel(e.Kind, c){AttackDamage = e.AttackDamage, MoveRange = e.MoveRange, MaxHealth = e.MaxHealth, Health = e.Health};
                var preview = BuildPreview(ghost, player, grid);
                int distance = Mathf.Abs(c.x - player.x) + Mathf.Abs(c.y - player.y);
                int score = 0;
                if (preview.Contains(player)) score += 100;
                score += preview.Count(escapeTiles.Contains) * 10;
                score -= distance * 3;
                if (e.Kind == EnemyKind.GloomArcher && (c.x == player.x || c.y == player.y)) score += 24;
                if (e.Kind == EnemyKind.LanternWarden && distance <= 3) score += 18;
                if (hazard != null && hazard(c)) score += e.Kind == EnemyKind.LanternWarden ? 8 : 3;
                if (c == e.Position) score -= 8;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = c;
                }
            }
            return best;
        }
    }
}
