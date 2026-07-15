using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class SquadPlan
    {
        public readonly Dictionary<EnemyModel,Vector2Int> Destinations=new();
        public readonly Dictionary<EnemyModel,HashSet<Vector2Int>> AttackTiles=new();
        public readonly HashSet<Vector2Int> ReservedDestinations=new();
        public readonly HashSet<Vector2Int> ReservedAttackTiles=new();
        public Vector2Int DestinationFor(EnemyModel enemy)=>Destinations.TryGetValue(enemy,out var p)?p:enemy.Position;
    }

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
                int radius = e.Kind == EnemyKind.LanternWarden ? (BossPhase(e) >= 2 ? 4 : 2) : 1;
                foreach(var p in grid.Floors()) if(Mathf.Abs(p.x-e.Position.x)+Mathf.Abs(p.y-e.Position.y)<=radius && p!=e.Position) r.Add(p);
            }
            return r;
        }

        public static HashSet<Vector2Int> BuildDelayedPreview(EnemyModel e, Vector2Int player, GridModel grid)
        {
            var r = new HashSet<Vector2Int>();
            if (e.Kind == EnemyKind.Ashling)
            {
                if (Mathf.Abs(e.Position.x-player.x)+Mathf.Abs(e.Position.y-player.y)<=2)
                    foreach (var p in grid.Floors()) if (Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)<=1) r.Add(p);
            }
            else if (e.Kind == EnemyKind.GloomArcher)
            {
                foreach (var p in CrossLine(e.Position, grid, 5)) r.Add(p);
            }
            else if (e.Kind == EnemyKind.StoneSentinel)
            {
                if (Mathf.Abs(e.Position.x-player.x)+Mathf.Abs(e.Position.y-player.y)<=2)
                    foreach (var p in grid.Floors()) if (Mathf.Abs(p.x-player.x)+Mathf.Abs(p.y-player.y)<=1) r.Add(p);
            }
            else
            {
                int phase = BossPhase(e);
                if (phase == 1) foreach (var p in CrossLine(e.Position, grid, 3)) r.Add(p);
                if (phase >= 2) foreach (var p in CrossLine(e.Position, grid, phase == 2 ? 6 : 8)) r.Add(p);
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
            if (e.Kind == EnemyKind.LanternWarden) return BossPhase(e) switch { 1 => "ward strike", 2 => "OVERCHARGE", _ => "HEAVY BLAST" };
            return "rush strike";
        }

        public static string BossPhaseSummary(EnemyModel e) => BossPhase(e) switch
        {
            2 => "Phase 2: overcharged range lines. Avoid orange boss lanes.",
            3 => "Phase 3: Heavy blast pattern. Avoid red/purple telegraphs.",
            _ => "Phase 1: ward strike."
        };

        static IEnumerable<Vector2Int> CrossLine(Vector2Int origin, GridModel grid, int range)
        {
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            foreach (var d in dirs)
            {
                var p = origin + d;
                for (int i = 0; i < range && grid.IsFloor(p); i++, p += d) yield return p;
            }
        }

        public static SquadPlan BuildSquadPlan(IEnumerable<EnemyModel> enemies,Vector2Int player,GridModel grid,System.Func<Vector2Int,bool> blocked,System.Func<Vector2Int,bool> hazard=null,System.Func<Vector2Int,int> traversalCost=null)
        {
            var living=enemies.Where(e=>e.Alive).OrderBy(RoleOrder).ThenBy(e=>e.Position.y).ThenBy(e=>e.Position.x).ToList();
            var plan=new SquadPlan(); var sectors=new HashSet<int>();
            int escapeLimit=Mathf.Min(3,Mathf.Max(1,living.Count-1));
            foreach(var enemy in living)
            {
                Vector2Int destination=ShouldHoldPosition(enemy,player,grid,living)||enemy.RootTurns>0
                    ? enemy.Position
                    : ChooseReposition(enemy,player,grid,blocked,hazard,living,traversalCost,plan.ReservedDestinations,plan.ReservedAttackTiles,sectors,escapeLimit);
                plan.Destinations[enemy]=destination;
                plan.ReservedDestinations.Add(destination);
                var ghost=new EnemyModel(enemy.Kind,destination){AttackDamage=enemy.AttackDamage,MoveRange=enemy.MoveRange,MaxHealth=enemy.MaxHealth,Health=enemy.Health};
                var attack=BuildPreview(ghost,player,grid); attack.UnionWith(BuildDelayedPreview(ghost,player,grid));
                plan.AttackTiles[enemy]=attack;
                plan.ReservedAttackTiles.UnionWith(attack);
                if(Manhattan(destination,player)<=3)sectors.Add(FlankSector(destination,player));
            }
            return plan;
        }

        public static bool ShouldHoldPosition(EnemyModel enemy,Vector2Int player,GridModel grid,IEnumerable<EnemyModel> allies)
        {
            if(enemy.Preview.Contains(player))return true;
            if(enemy.Kind==EnemyKind.GloomArcher&&enemy.DelayedPreview.Contains(player))return true;
            if(enemy.Kind!=EnemyKind.StoneSentinel)return enemy.DelayedPreview.Contains(player);
            int distance=Manhattan(enemy.Position,player);
            bool protectsRanged=(allies??Enumerable.Empty<EnemyModel>()).Any(a=>a!=enemy&&a.Alive&&a.Kind==EnemyKind.GloomArcher&&Manhattan(a.Position,enemy.Position)<=2);
            return enemy.DelayedPreview.Contains(player)&&(distance<=2||IsChokepoint(enemy.Position,grid)||protectsRanged);
        }

        public static Vector2Int ChooseReposition(EnemyModel e, Vector2Int player, GridModel grid, System.Func<Vector2Int, bool> blocked, System.Func<Vector2Int, bool> hazard = null, IEnumerable<EnemyModel> allies = null, System.Func<Vector2Int,int> traversalCost = null, ISet<Vector2Int> reservedDestinations=null, ISet<Vector2Int> reservedAttackTiles=null, ISet<int> reservedFlankSectors=null, int escapeReservationLimit=3)
        {
            var candidates = grid.Reachable(e.Position, e.MoveRange, p => blocked(p) && p != e.Position).ToList();
            if (!candidates.Contains(e.Position)) candidates.Add(e.Position);
            candidates.Remove(player);
            if(reservedDestinations!=null)candidates.RemoveAll(c=>reservedDestinations.Contains(c)&&c!=e.Position);
            if(e.Kind==EnemyKind.GloomArcher&&reservedDestinations!=null)
            {
                var separated=candidates.Where(c=>reservedDestinations.All(p=>Manhattan(p,c)>1)).ToList();
                if(separated.Count>0)candidates=separated;
            }
            var livingAllies = (allies ?? Enumerable.Empty<EnemyModel>()).Where(a => a != e && a.Alive).ToList();
            var rangedAllies = livingAllies.Where(a => a.Kind == EnemyKind.GloomArcher).ToList();
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
                int routeCost=traversalCost==null ? distance : grid.WeightedDistance(c,player,p=>blocked(p)&&p!=e.Position,traversalCost);
                if(routeCost==int.MaxValue)score-=500;
                else score-=routeCost*2;
                bool hasLine = c.x == player.x || c.y == player.y;
                bool lineClear = hasLine && SkillBook.HasLineOfSight(grid, c, player);
                if (preview.Contains(player)) score += 150;
                score += preview.Count(escapeTiles.Contains) * 10;
                score += delayed.Count(escapeTiles.Contains) * 5;
                if(reservedAttackTiles!=null)
                {
                    int unique=preview.Concat(delayed).Distinct().Count(p=>escapeTiles.Contains(p)&&!reservedAttackTiles.Contains(p));
                    int duplicate=preview.Concat(delayed).Distinct().Count(reservedAttackTiles.Contains);
                    score+=unique*16-duplicate*3;
                }
                score -= distance * 3;
                if (distance < startDistance) score += (startDistance - distance) * 8;
                if (distance > startDistance) score -= (distance - startDistance) * 10;
                if (e.Kind == EnemyKind.GloomArcher)
                {
                    if (distance <= 2) score -= (3 - distance) * 32;
                    if (lineClear) score += 58;
                    else if (hasLine) score += 12;
                    int ideal = 5;
                    score -= Mathf.Abs(distance - ideal) * 5;
                }
                else if (e.Kind == EnemyKind.Ashling)
                {
                    if (distance >= 2 && distance <= 3) score += 32;
                    if (distance == 1) score -= 150;
                    if (hazard != null && grid.Neighbors(c).Any(hazard)) score += 18;
                    score += grid.Neighbors(player).Count(n => preview.Contains(n) || delayed.Contains(n)) * 6;
                }
                else if (e.Kind == EnemyKind.StoneSentinel)
                {
                    if(distance<startDistance)score+=(startDistance-distance)*14;
                    if(c==e.Position&&distance>2)score-=70;
                    if (distance <= 2) score += 16;
                    if (delayed.Contains(player) || delayed.Count(escapeTiles.Contains) > 0) score += 14;
                    if (IsChokepoint(c, grid)) score += 28;
                    if (rangedAllies.Any(a => Mathf.Abs(c.x-a.Position.x)+Mathf.Abs(c.y-a.Position.y)<=2)) score += 18;
                    if (BlocksLineToRangedAlly(c, player, rangedAllies)) score += 24;
                    if(grid.Neighbors(player).Contains(c))score+=24;
                }
                if(distance<=3&&reservedFlankSectors!=null)
                {
                    int sector=FlankSector(c,player);
                    score+=reservedFlankSectors.Contains(sector)?-34:24;
                }
                if(grid.Neighbors(player).Contains(c)&&reservedDestinations!=null)
                {
                    int reservedEscape=grid.Neighbors(player).Count(reservedDestinations.Contains);
                    if(reservedEscape>=escapeReservationLimit)score-=120;
                    else if(!preview.Contains(player))score+=26;
                }
                if(e.Kind==EnemyKind.GloomArcher&&livingAllies.Any(a=>a.Kind==EnemyKind.StoneSentinel&&Manhattan(a.Position,c)<=1))score-=28;
                if(e.Kind==EnemyKind.GloomArcher&&reservedDestinations!=null&&reservedDestinations.Any(p=>Manhattan(p,c)<=1))score-=24;
                if (e.Kind == EnemyKind.LanternWarden && distance <= (BossPhase(e) >= 3 ? 4 : 3)) score += 22;
                if (hazard != null && hazard(c)) score -= Mathf.Max(1,(traversalCost?.Invoke(c)??2)-1)*12;
                if(e.PreviousPosition.HasValue&&c==e.PreviousPosition.Value&&!preview.Contains(player))score-=90;
                if(e.CommittedDestination.HasValue&&c==e.CommittedDestination.Value)score+=12;
                if(e.NoProgressTurns>0&&e.Kind!=EnemyKind.GloomArcher)score+=(startDistance-distance)*12;
                if(e.NoProgressTurns>=2&&c==e.Position&&!preview.Contains(player))score-=100;
                if (c == e.Position && !preview.Contains(player)) score -= 30;
                if (score > bestScore || score == bestScore && BetterTieBreak(c, best, player, e.Position, grid, e))
                {
                    bestScore = score;
                    best = c;
                }
            }
            return best;
        }

        static int RoleOrder(EnemyModel e)=>e.Kind switch {EnemyKind.StoneSentinel=>0,EnemyKind.Ashling=>1,EnemyKind.GloomArcher=>2,EnemyKind.LanternWarden=>3,_=>4};
        static int FlankSector(Vector2Int position,Vector2Int player)
        {
            var d=position-player;
            if(Mathf.Abs(d.x)>=Mathf.Abs(d.y))return d.x>=0?0:2;
            return d.y>=0?1:3;
        }

        static int Manhattan(Vector2Int a,Vector2Int b)=>Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);

        static bool IsChokepoint(Vector2Int p, GridModel grid)
        {
            int open = grid.Neighbors(p).Count();
            return open <= 2;
        }

        static bool BlocksLineToRangedAlly(Vector2Int candidate, Vector2Int player, IEnumerable<EnemyModel> rangedAllies)
        {
            foreach (var ally in rangedAllies)
            {
                if (ally.Position.x == player.x && candidate.x == player.x && Between(candidate.y, ally.Position.y, player.y)) return true;
                if (ally.Position.y == player.y && candidate.y == player.y && Between(candidate.x, ally.Position.x, player.x)) return true;
            }
            return false;
        }

        static bool Between(int value, int a, int b) => value > Mathf.Min(a, b) && value < Mathf.Max(a, b);

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
