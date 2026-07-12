using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public enum SkillId
    {
        SpearThrust, GuardStance, SunCharge,
        StraightShot, MarkedTarget, PiercingPrism,
        EmberBolt, CinderBloom, DelayedBlast,
        DiagonalDash, Backstab, ShadowSwap,
        LensTrap, RedirectShot, ShieldGadget
    }

    public enum SkillEffect { Damage, SelfShield, DashDamage, Mark, AreaBurn, DelayedArea, DiagonalMove, Swap, Root }

    public sealed class SkillDefinition
    {
        public SkillId Id;
        public PlayerClassId ClassId;
        public string Name;
        public int Range;
        public int Cooldown;
        public int ApCost;
        public int Damage;
        public SkillEffect Effect;
        public bool RequiresLineOfSight = true;
        public string Hint;
    }

    public static class ClassCatalog
    {
        public static readonly (PlayerClassId id, string name, string title, string description)[] All =
        {
            (PlayerClassId.Vanguard, "Vanguard", "Sun Spear", "Durable close-range fighter with guard and push."),
            (PlayerClassId.Wayfinder, "Wayfinder", "Prism Bow", "Long-range line attacker with marks and piercing shots."),
            (PlayerClassId.Cantor, "Cantor", "Cinder Staff", "Spellcaster with burn and delayed blasts."),
            (PlayerClassId.Gloamstep, "Gloamstep", "Echo Blades", "Mobile rogue with diagonal movement and swaps."),
            (PlayerClassId.Artificer, "Artificer", "Lenscaster", "Utility controller with shields, roots, and gadgets.")
        };

        public static (PlayerClassId id, string name, string title, string description) Get(PlayerClassId id) => All[(int)id];
    }

    public static class SkillBook
    {
        public static readonly SkillDefinition[] All =
        {
            new(){Id=SkillId.SpearThrust,ClassId=PlayerClassId.Vanguard,Name="Spear Thrust",Range=1,Cooldown=0,ApCost=3,Damage=3,Effect=SkillEffect.Damage,Hint="Melee hit + push"},
            new(){Id=SkillId.GuardStance,ClassId=PlayerClassId.Vanguard,Name="Guard Stance",Range=0,Cooldown=2,ApCost=2,Damage=0,Effect=SkillEffect.SelfShield,RequiresLineOfSight=false,Hint="Gain 4 shield"},
            new(){Id=SkillId.SunCharge,ClassId=PlayerClassId.Vanguard,Name="Sun Charge",Range=3,Cooldown=2,ApCost=4,Damage=3,Effect=SkillEffect.DashDamage,RequiresLineOfSight=false,Hint="Charge, hit, push"},

            new(){Id=SkillId.StraightShot,ClassId=PlayerClassId.Wayfinder,Name="Straight Shot",Range=6,Cooldown=0,ApCost=3,Damage=3,Effect=SkillEffect.Damage,Hint="Long line shot"},
            new(){Id=SkillId.MarkedTarget,ClassId=PlayerClassId.Wayfinder,Name="Marked Target",Range=5,Cooldown=1,ApCost=2,Damage=1,Effect=SkillEffect.Mark,RequiresLineOfSight=false,Hint="Chip + mark"},
            new(){Id=SkillId.PiercingPrism,ClassId=PlayerClassId.Wayfinder,Name="Piercing Prism",Range=6,Cooldown=2,ApCost=5,Damage=5,Effect=SkillEffect.Damage,Hint="Heavy line hit"},

            new(){Id=SkillId.EmberBolt,ClassId=PlayerClassId.Cantor,Name="Ember Bolt",Range=4,Cooldown=1,ApCost=3,Damage=3,Effect=SkillEffect.Damage,RequiresLineOfSight=false,Hint="Reliable range"},
            new(){Id=SkillId.CinderBloom,ClassId=PlayerClassId.Cantor,Name="Cinder Bloom",Range=4,Cooldown=2,ApCost=4,Damage=2,Effect=SkillEffect.AreaBurn,RequiresLineOfSight=false,Hint="Small burn area"},
            new(){Id=SkillId.DelayedBlast,ClassId=PlayerClassId.Cantor,Name="Delayed Blast",Range=4,Cooldown=2,ApCost=5,Damage=4,Effect=SkillEffect.DelayedArea,RequiresLineOfSight=false,Hint="Previewed blast"},

            new(){Id=SkillId.DiagonalDash,ClassId=PlayerClassId.Gloamstep,Name="Diagonal Dash",Range=3,Cooldown=1,ApCost=3,Damage=0,Effect=SkillEffect.DiagonalMove,RequiresLineOfSight=false,Hint="Diagonal reposition"},
            new(){Id=SkillId.Backstab,ClassId=PlayerClassId.Gloamstep,Name="Backstab",Range=1,Cooldown=0,ApCost=3,Damage=4,Effect=SkillEffect.Damage,Hint="Close burst"},
            new(){Id=SkillId.ShadowSwap,ClassId=PlayerClassId.Gloamstep,Name="Shadow Swap",Range=4,Cooldown=1,ApCost=3,Damage=0,Effect=SkillEffect.Swap,Hint="Swap with foe"},

            new(){Id=SkillId.LensTrap,ClassId=PlayerClassId.Artificer,Name="Lens Trap",Range=4,Cooldown=1,ApCost=3,Damage=2,Effect=SkillEffect.Root,Hint="Damage + root"},
            new(){Id=SkillId.RedirectShot,ClassId=PlayerClassId.Artificer,Name="Redirect Shot",Range=5,Cooldown=1,ApCost=4,Damage=3,Effect=SkillEffect.Damage,Hint="Line control shot"},
            new(){Id=SkillId.ShieldGadget,ClassId=PlayerClassId.Artificer,Name="Shield Gadget",Range=0,Cooldown=2,ApCost=2,Damage=0,Effect=SkillEffect.SelfShield,RequiresLineOfSight=false,Hint="Gain 3 shield"}
        };

        public static SkillDefinition Get(SkillId id) => All.First(s => s.Id == id);
        public static SkillDefinition[] ForClass(PlayerClassId id) => All.Where(s => s.ClassId == id).ToArray();

        public static HashSet<Vector2Int> Targets(GridModel grid, PlayerModel player, SkillDefinition def, System.Func<Vector2Int,bool> occupied, int rangeBonus=0)
        {
            var result = new HashSet<Vector2Int>();
            if (def.Effect == SkillEffect.SelfShield){result.Add(player.Position); return result;}

            foreach (var p in grid.Floors())
            {
                int d = Manhattan(p, player.Position);
                if (d > def.Range + rangeBonus || d == 0 && def.Effect != SkillEffect.DiagonalMove) continue;
                if (def.RequiresLineOfSight && !HasLineOfSight(grid, player.Position, p)) continue;

                bool valid = def.Effect switch
                {
                    SkillEffect.DashDamage => !occupied(p),
                    SkillEffect.DiagonalMove => Mathf.Abs(p.x-player.Position.x)==Mathf.Abs(p.y-player.Position.y) && !occupied(p),
                    SkillEffect.Damage or SkillEffect.Mark or SkillEffect.AreaBurn or SkillEffect.DelayedArea or SkillEffect.Root or SkillEffect.Swap => occupied(p),
                    _ => false
                };
                if (valid) result.Add(p);
            }
            return result;
        }

        public static HashSet<Vector2Int> AffectedTiles(GridModel grid, Vector2Int target, SkillDefinition def)
        {
            var result = new HashSet<Vector2Int>{target};
            if (def.Effect == SkillEffect.AreaBurn || def.Effect == SkillEffect.DelayedArea)
                foreach (var n in grid.Neighbors(target)) result.Add(n);
            return result;
        }

        public static bool HasLineOfSight(GridModel grid, Vector2Int from, Vector2Int to)
        {
            if (from == to) return true;
            int dx = Mathf.Clamp(to.x - from.x, -1, 1);
            int dy = Mathf.Clamp(to.y - from.y, -1, 1);
            bool straight = from.x == to.x || from.y == to.y || Mathf.Abs(to.x - from.x) == Mathf.Abs(to.y - from.y);
            if (!straight) return false;
            var p = from + new Vector2Int(dx, dy);
            while (p != to)
            {
                if (!grid.IsFloor(p)) return false;
                p += new Vector2Int(dx, dy);
            }
            return grid.IsFloor(to);
        }

        static int Manhattan(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
    }
}
