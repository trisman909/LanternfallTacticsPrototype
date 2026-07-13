using UnityEngine;

namespace Lanternfall
{
    public enum TileVisualState { Floor, Hazard, ArmedHazard, EnemyPreview, MoveTarget, SkillTarget, AreaPreview, Hit, Invalid }

    public static class VisualReadability
    {
        public static Color TileColor(BiomeTheme theme, TileVisualState state, bool alternate = false)
        {
            Color floor = alternate ? theme.Alternate : theme.Floor;
            return state switch
            {
                TileVisualState.Hazard => Boost(theme.HazardColor, .08f),
                TileVisualState.ArmedHazard => Boost(theme.WarningColor, .04f),
                TileVisualState.EnemyPreview => new Color(.30f, .015f, .025f),
                TileVisualState.MoveTarget => new Color(.04f, .58f, .64f),
                TileVisualState.SkillTarget => new Color(.90f, .61f, .08f),
                TileVisualState.AreaPreview => new Color(.92f, .38f, .05f),
                TileVisualState.Hit => new Color(1f, .88f, .12f),
                TileVisualState.Invalid => new Color(.95f, .05f, .08f),
                _ => floor
            };
        }

        public static Color ClassAccent(PlayerClassId id) => id switch
        {
            PlayerClassId.Vanguard => new Color(1f, .72f, .18f),
            PlayerClassId.Wayfinder => new Color(.78f, .45f, 1f),
            PlayerClassId.Cantor => new Color(1f, .34f, .12f),
            PlayerClassId.Gloamstep => new Color(.44f, 1f, .58f),
            PlayerClassId.Artificer => new Color(.38f, .82f, 1f),
            _ => Color.cyan
        };

        public static Color EnemyColor(EnemyKind kind) => kind switch
        {
            EnemyKind.Ashling => new Color(.95f, .32f, .18f),
            EnemyKind.GloomArcher => new Color(.70f, .22f, .95f),
            EnemyKind.StoneSentinel => new Color(.67f, .64f, .58f),
            EnemyKind.LanternWarden => new Color(1f, .18f, .78f),
            _ => Color.red
        };

        public static string ClassGlyph(PlayerClassId id) => id switch
        {
            PlayerClassId.Vanguard => "S",
            PlayerClassId.Wayfinder => "B",
            PlayerClassId.Cantor => "F",
            PlayerClassId.Gloamstep => "D",
            PlayerClassId.Artificer => "L",
            _ => "*"
        };

        public static string EnemyGlyph(EnemyKind kind) => kind switch
        {
            EnemyKind.Ashling => "A",
            EnemyKind.GloomArcher => "G",
            EnemyKind.StoneSentinel => "S",
            EnemyKind.LanternWarden => "W",
            _ => "?"
        };

        public static string HazardGlyph(HazardKind hazard) => hazard switch
        {
            HazardKind.ShallowWater => "~",
            HazardKind.Prism => "<>",
            HazardKind.EmberVent => "!",
            HazardKind.GraspingRoots => "#",
            HazardKind.ChargedFloor => "Z",
            _ => "!"
        };

        public static string FloorGlyph(BiomeId id, bool alternate) => id switch
        {
            BiomeId.DrownedNarthex => alternate ? "≈" : "·",
            BiomeId.SiltglassObservatory => alternate ? "◇" : "·",
            BiomeId.EmberOssuary => alternate ? "⌁" : "·",
            BiomeId.GloamOrchard => alternate ? "✣" : "·",
            BiomeId.StormvaultFoundry => alternate ? "═" : "·",
            _ => "·"
        };

        public static string StatusGlyph(UnitModel unit)
        {
            string glyphs = "";
            if (unit.Shield > 0) glyphs += "□";
            if (unit.BurnTurns > 0) glyphs += "•";
            if (unit.RootTurns > 0) glyphs += "×";
            if (unit.MarkedTurns > 0) glyphs += "◇";
            return glyphs;
        }

        public static float Contrast(Color a, Color b) => Mathf.Abs(Luminance(a) - Luminance(b));

        static Color Boost(Color c, float amount) => new(Mathf.Clamp01(c.r + amount), Mathf.Clamp01(c.g + amount), Mathf.Clamp01(c.b + amount), c.a);
        static float Luminance(Color c) => c.r * .299f + c.g * .587f + c.b * .114f;
    }
}
