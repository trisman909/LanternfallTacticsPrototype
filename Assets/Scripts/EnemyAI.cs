using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public static class EnemyAI
    {
        public static int BossPhase(EnemyModel e)
        {
            if (e.Kind != EnemyKind.LanternWarden) return 0;
            if (e.Health * 3 <= e.MaxHealth) return 3;
            if (e.Health * 3 <= e.MaxHealth * 2) return 2;
            return 1;
        }

        public static void AssignIntent(EnemyModel e, Vector2Int player, GridModel grid)
        {
            e.Preview = BuildPreview(e, player, grid);
            e.DelayedPreview = BuildDelayedPreview(e, player, grid);
            e.Threat = IntentThreat(e);
            e.IntentLabel = IntentLabel(e);
        }

        public static HashSet<Vector2Int> BuildPreview(EnemyModel e, Vector2Int player, GridModel grid)
        {
            var r = new HashSet<Vector2Int>();
            if (e.Kind == EnemyKind.GloomArcher)
            {
                var delta = player - e.Position;
                if (delta.x == 0 || delta.y == 0)
                {
                    var step = new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y)); var p = e.Position + step;
                    for(int i=0;i<5 && grid.IsFloor(p);i++,p+=step) r.Add(p);
                }
            }
            else
            {
                int radius = e.Kind == EnemyKind.LanternWarden ? (BossPhase(e) >= 2 ? 3 : 2) : 1;
                foreach(var p in grid.Floors()) if(Mathf.Abs(p.x-e.Position.x)+Mathf.Abs(p.y-e.Position.y)<=radius && p!=e.Position) r.Add(p);
            }
            return r;
        }

        public static HashSet<Vector2Int> BuildDelayedPreview(EnemyModel e, Vector2Int player, GridModel grid)
        {
            var r = new HashSet<Vector2Int>();
            if (e.Kind == EnemyKind.Ashling)
            {
                foreach (var p in grid.Floors()) if (Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)<=1) r.Add(p);
            }
            else if (e.Kind == EnemyKind.GloomArcher)
            {
                foreach (var p in CrossLine(e.Position, grid, 5)) r.Add(p);
            }
            else if (e.Kind == EnemyKind.StoneSentinel)
            {
                foreach (var p in grid.Floors()) if (Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)<=1) r.Add(p);
            }
            else
            {
                int phase = BossPhase(e);
                if (phase == 1) foreach (var p in CrossLine(e.Position, grid, 3)) r.Add(p);
                if (phase >= 2) foreach (var p in CrossLine(e.Position, grid, phase == 2 ? 5 : 7)) r.Add(p);
                if (phase >= 3) foreach (var p in grid.Floors()) if (Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)<=1) r.Add(p);
            }
            r.Remove(e.Position);
            return r;
        }

        public static ThreatKind IntentThreat(EnemyModel e)
        {
            if (e.Kind == EnemyKind.GloomArcher) return ThreatKind.AP;
            if (e.Kind == EnemyKind.StoneSentinel) return ThreatKind.MP;
            if (e.Kind == EnemyKind.LanternWarden) return BossPhase(e) >= 2 ? ThreatKind.Mixed : ThreatKind.HP;
            return ThreatKind.HP;
        }

        public static string IntentLabel(EnemyModel e)
        {
            if (e.Kind == EnemyKind.GloomArcher) return "AP drain";
            if (e.Kind == EnemyKind.StoneSentinel) return "MP bind";
            if (e.Kind == EnemyKind.LanternWarden) return BossPhase(e) switch { 1 => "ward strike", 2 => "line + AP", _ => "storm blast" };
            return "rush strike";
        }

        static IEnumerable<Vector2Int> CrossLine(Vector2Int origin, GridModel grid, int range)
        {
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            foreach (var d in dirs)
            {
                var p = origin + d;
                for (int i = 0; i < range && grid.IsFloor(p); i++, p += d) yield return p;
            }
        }

        public static Vector2Int ChooseReposition(EnemyModel e, Vector2Int player, GridModel grid, System.Func<Vector2Int, bool> blocked, System.Func<Vector2Int, bool> hazard = null)
        {
            var candidates = grid.Reachable(e.Position, e.MoveRange, p => blocked(p) && p != e.Position).ToList();
            if (!candidates.Contains(e.Position)) candidates.Add(e.Position);
            var escapeTiles = grid.Floors().Where(p => Mathf.Abs(p.x - player.x) + Mathf.Abs(p.y - player.y) <= 2).ToHashSet();
            Vector2Int best = e.Position;
            int bestScore = int.MinValue;
            int startDistance = Mathf.Abs(e.Position.x - player.x) + Mathf.Abs(e.Position.y - player.y);
            foreach (var c in candidates)
            {
                var ghost = new EnemyModel(e.Kind, c){AttackDamage = e.AttackDamage, MoveRange = e.MoveRange, MaxHealth = e.MaxHealth, Health = e.Health};
                var preview = BuildPreview(ghost, player, grid);
                var delayed = BuildDelayedPreview(ghost, player, grid);
                int distance = Mathf.Abs(c.x - player.x) + Mathf.Abs(c.y - player.y);
                int score = 0;
                bool hasLine = c.x == player.x || c.y == player.y;
                bool lineClear = hasLine && SkillBook.HasLineOfSight(grid, c, player);
                if (preview.Contains(player)) score += 120;
                score += preview.Count(escapeTiles.Contains) * 10;
                score += delayed.Count(escapeTiles.Contains) * 5;
                score -= distance * 3;
                if (distance < startDistance) score += (startDistance - distance) * 8;
                if (distance > startDistance) score -= (distance - startDistance) * 10;
                if (e.Kind == EnemyKind.GloomArcher)
                {
                    if (lineClear) score += 42;
                    else if (hasLine) score += 12;
                    int ideal = 4;
                    score -= Mathf.Abs(distance - ideal) * 2;
                }
                else if (e.Kind == EnemyKind.Ashling)
                {
                    if (distance <= 2) score += 24;
                    if (distance == 1) score += 18;
                }
                else if (e.Kind == EnemyKind.StoneSentinel)
                {
                    if (distance <= 2) score += 18;
                    if (delayed.Contains(player) || delayed.Count(escapeTiles.Contains) > 0) score += 14;
                }
                if (e.Kind == EnemyKind.LanternWarden && distance <= (BossPhase(e) >= 3 ? 4 : 3)) score += 22;
                if (hazard != null && hazard(c)) score += e.Kind == EnemyKind.LanternWarden ? 8 : 3;
                if (c == e.Position && !preview.Contains(player)) score -= 30;
                if (score > bestScore || score == bestScore && BetterTieBreak(c, best, player, e.Position, grid, e))
                {
                    bestScore = score;
                    best = c;
                }
            }
            return best;
        }

        static bool BetterTieBreak(Vector2Int candidate, Vector2Int incumbent, Vector2Int player, Vector2Int start, GridModel grid, EnemyModel e)
        {
            int candidateDistance = Mathf.Abs(candidate.x - player.x) + Mathf.Abs(candidate.y - player.y);
            int incumbentDistance = Mathf.Abs(incumbent.x - player.x) + Mathf.Abs(incumbent.y - player.y);
            if (candidateDistance != incumbentDistance) return candidateDistance < incumbentDistance;
            bool candidateLine = (candidate.x == player.x || candidate.y == player.y) && SkillBook.HasLineOfSight(grid, candidate, player);
            bool incumbentLine = (incumbent.x == player.x || incumbent.y == player.y) && SkillBook.HasLineOfSight(grid, incumbent, player);
            if (candidateLine != incumbentLine) return candidateLine;
            if (incumbent == start && candidate != start) return true;
            if (candidate == start && incumbent != start) return false;
            var toward = new Vector2Int(System.Math.Sign(player.x - start.x), System.Math.Sign(player.y - start.y));
            int candidateToward = (candidate.x - start.x) * toward.x + (candidate.y - start.y) * toward.y;
            int incumbentToward = (incumbent.x - start.x) * toward.x + (incumbent.y - start.y) * toward.y;
            if (candidateToward != incumbentToward) return candidateToward > incumbentToward;
            return candidate.x != incumbent.x ? candidate.x > incumbent.x : candidate.y > incumbent.y;
        }
    }
}
