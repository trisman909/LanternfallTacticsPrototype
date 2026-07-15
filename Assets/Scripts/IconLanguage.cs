using UnityEngine;

namespace Lanternfall
{
    public enum VisualIcon
    {
        Health, ActionPoint, MovementPoint, Shield,
        Burn, Root, Mark, Heal, ActionDrain, MovementDrain,
        ImmediateDanger, DelayedDanger, BossDanger, BlockedLineOfSight,
        HealingPickup, ShallowWater, Prism, EmberVent, GraspingRoots, ChargedFloor,
        DelayedCast, Shielded, BossOvercharge, Move, ValidTarget, InvalidTarget, Obstacle
    }

    public readonly struct IconDescriptor
    {
        public readonly string StableId;
        public readonly string Meaning;
        public readonly Color Color;
        public IconDescriptor(string stableId, string meaning, Color color)
        { StableId = stableId; Meaning = meaning; Color = color; }
    }

    // Phase 6C's asset-free icon vocabulary. The view draws these as a few solid
    // rectangles/lines, so the full set adds no textures, atlases, shaders, or allocations.
    public static class IconLanguage
    {
        public static readonly VisualIcon[] All = (VisualIcon[])System.Enum.GetValues(typeof(VisualIcon));

        public static IconDescriptor Describe(VisualIcon icon) => icon switch
        {
            VisualIcon.Health => new("stat.hp", "Health", new Color(.94f, .28f, .28f)),
            VisualIcon.ActionPoint => new("stat.ap", "Action points", new Color(1f, .72f, .20f)),
            VisualIcon.MovementPoint => new("stat.mp", "Movement points", new Color(.30f, .88f, 1f)),
            VisualIcon.Shield => new("status.shield", "Shield", new Color(.72f, .84f, 1f)),
            VisualIcon.Burn => new("status.burn", "Burn", new Color(1f, .36f, .10f)),
            VisualIcon.Root => new("status.root", "Root", new Color(.44f, .92f, .38f)),
            VisualIcon.Mark => new("status.mark", "Mark", new Color(.90f, .48f, 1f)),
            VisualIcon.Heal => new("status.heal", "Heal", new Color(.30f, 1f, .52f)),
            VisualIcon.ActionDrain => new("threat.ap_drain", "Action-point drain", new Color(.90f, .52f, 1f)),
            VisualIcon.MovementDrain => new("threat.mp_drain", "Movement-point drain", new Color(.52f, .70f, 1f)),
            VisualIcon.ImmediateDanger => new("danger.immediate", "Immediate danger", new Color(1f, .22f, .16f)),
            VisualIcon.DelayedDanger => new("danger.delayed", "Delayed danger", new Color(.78f, .34f, 1f)),
            VisualIcon.BossDanger => new("danger.boss", "Boss danger", new Color(1f, .56f, .12f)),
            VisualIcon.BlockedLineOfSight => new("board.blocked_los", "Blocked line of sight", new Color(.72f, .66f, .88f)),
            VisualIcon.HealingPickup => new("pickup.heal", "Healing pickup", new Color(.28f, 1f, .48f)),
            VisualIcon.ShallowWater => new("hazard.water", "Shallow water", new Color(.44f, .92f, .94f)),
            VisualIcon.Prism => new("hazard.prism", "Prism glass", new Color(.82f, .42f, 1f)),
            VisualIcon.EmberVent => new("hazard.ember", "Ember vent", new Color(1f, .45f, .08f)),
            VisualIcon.GraspingRoots => new("hazard.roots", "Grasping roots", new Color(.52f, 1f, .48f)),
            VisualIcon.ChargedFloor => new("hazard.charge", "Charged floor", new Color(.34f, .78f, 1f)),
            VisualIcon.DelayedCast => new("status.delayed_cast", "Delayed cast", new Color(.72f, .36f, 1f)),
            VisualIcon.Shielded => new("status.shielded", "Shielded", new Color(.76f, .86f, 1f)),
            VisualIcon.BossOvercharge => new("status.boss_overcharge", "Boss overcharge", new Color(1f, .42f, .14f)),
            VisualIcon.Move => new("target.move", "Move", new Color(.28f, .90f, 1f)),
            VisualIcon.ValidTarget => new("target.valid", "Valid target", new Color(1f, .72f, .18f)),
            VisualIcon.InvalidTarget => new("target.invalid", "Invalid target", new Color(1f, .18f, .16f)),
            VisualIcon.Obstacle => new("board.obstacle", "Obstacle", new Color(.62f, .60f, .64f)),
            _ => new("unknown", "Unknown", Color.white)
        };

        public static int AtlasCell(VisualIcon icon) => icon switch
        {
            VisualIcon.Health => 0, VisualIcon.ActionPoint => 1, VisualIcon.MovementPoint => 2,
            VisualIcon.Shield => 3, VisualIcon.Burn => 4, VisualIcon.Root => 5, VisualIcon.Mark => 6,
            VisualIcon.Heal => 7, VisualIcon.ActionDrain => 8, VisualIcon.MovementDrain => 9,
            VisualIcon.DelayedCast => 10, VisualIcon.Shielded => 11, VisualIcon.BossOvercharge => 12,
            VisualIcon.Move => 13, VisualIcon.ValidTarget => 14, VisualIcon.InvalidTarget => 15,
            VisualIcon.BlockedLineOfSight => 16, VisualIcon.ImmediateDanger => 17,
            VisualIcon.DelayedDanger => 18, VisualIcon.BossDanger => 19, VisualIcon.HealingPickup => 20,
            VisualIcon.Obstacle => 21, VisualIcon.ShallowWater => 22, VisualIcon.Prism => 23,
            VisualIcon.EmberVent => 4, VisualIcon.GraspingRoots => 5, VisualIcon.ChargedFloor => 24,
            _ => 0
        };

        public static VisualIcon ForHazard(HazardKind hazard) => hazard switch
        {
            HazardKind.ShallowWater => VisualIcon.ShallowWater,
            HazardKind.Prism => VisualIcon.Prism,
            HazardKind.EmberVent => VisualIcon.EmberVent,
            HazardKind.GraspingRoots => VisualIcon.GraspingRoots,
            _ => VisualIcon.ChargedFloor
        };

        public static VisualIcon ForThreat(ThreatKind threat) => threat switch
        {
            ThreatKind.AP => VisualIcon.ActionDrain,
            ThreatKind.MP => VisualIcon.MovementDrain,
            ThreatKind.Mixed => VisualIcon.BossDanger,
            _ => VisualIcon.ImmediateDanger
        };
    }
}
