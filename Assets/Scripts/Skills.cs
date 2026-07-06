using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public enum SkillId { EmberBolt, LanternDash, RadiantSweep }
    public sealed class SkillDefinition
    {
        public SkillId Id; public string Name; public int Range; public int Cooldown; public string Hint;
    }
    public static class SkillBook
    {
        public static readonly SkillDefinition[] All = {
            new(){Id=SkillId.EmberBolt, Name="Ember Bolt", Range=4, Cooldown=2, Hint="3 damage at range"},
            new(){Id=SkillId.LanternDash, Name="Lantern Dash", Range=4, Cooldown=3, Hint="Dash; scorch adjacent foes"},
            new(){Id=SkillId.RadiantSweep, Name="Radiant Sweep", Range=1, Cooldown=3, Hint="2 damage around you"}
        };
        public static SkillDefinition Get(SkillId id) => All[(int)id];
        public static HashSet<Vector2Int> Targets(GridModel grid, PlayerModel player, SkillId id, System.Func<Vector2Int,bool> occupied)
        {
            var result = new HashSet<Vector2Int>(); var def = Get(id);
            foreach (var p in grid.Floors())
            {
                int d = Mathf.Abs(p.x-player.Position.x)+Mathf.Abs(p.y-player.Position.y);
                bool valid = id switch
                {
                    SkillId.EmberBolt => d > 0 && d <= def.Range && occupied(p),
                    SkillId.LanternDash => d > 0 && d <= def.Range && !occupied(p),
                    _ => d > 0 && d <= def.Range
                };
                if (valid) result.Add(p);
            }
            return result;
        }
    }
}
