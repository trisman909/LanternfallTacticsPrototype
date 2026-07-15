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

        public static float Contrast(Color a, Color b) => Mathf.Abs(Luminance(a) - Luminance(b));

        public static Color EnvironmentTextureTint(BiomeId biome)=>biome==BiomeId.SiltglassObservatory?new Color(.42f,.42f,.42f):Color.white;

        public static float EnvironmentQuietingAlpha(BiomeId biome)=>biome switch
        {
            BiomeId.DrownedNarthex=>.50f,
            BiomeId.SiltglassObservatory=>.66f,
            BiomeId.EmberOssuary=>.24f,
            BiomeId.GloamOrchard=>.56f,
            BiomeId.StormvaultFoundry=>.40f,
            _=>.44f
        };

        public static Color QuietEnvironmentOverlay(BiomeTheme theme,bool hazard=false)
        {
            float grey=Luminance(theme.Floor);
            Color muted=Color.Lerp(theme.Floor,new Color(grey,grey,grey),.28f);
            muted.a=hazard ? Mathf.Min(.24f,EnvironmentQuietingAlpha(theme.Id)*.45f) : EnvironmentQuietingAlpha(theme.Id);
            return muted;
        }

        public static Color PaintedLightOverlay(BiomeTheme theme,float diagonal)
        {
            diagonal=Mathf.Clamp01(diagonal);
            if(diagonal<.5f)
            {
                float a=(.5f-diagonal)*.08f;
                return new Color(theme.Accent.r*.34f,theme.Accent.g*.34f,theme.Accent.b*.34f,a);
            }
            return new Color(.01f,.012f,.02f,(diagonal-.5f)*.14f);
        }

        public static float TacticalOverlayAlpha(TileVisualState state)=>state switch
        {
            TileVisualState.Hit=>.80f,
            TileVisualState.EnemyPreview=>.72f,
            TileVisualState.AreaPreview=>.68f,
            TileVisualState.MoveTarget=>.64f,
            TileVisualState.SkillTarget=>.68f,
            TileVisualState.Hazard=>.38f,
            TileVisualState.ArmedHazard=>.62f,
            _=>.52f
        };

        public static float UnitTokenScale(bool player,bool boss)=>boss ? .99f : player ? .98f : .96f;

        static Color Boost(Color c, float amount) => new(Mathf.Clamp01(c.r + amount), Mathf.Clamp01(c.g + amount), Mathf.Clamp01(c.b + amount), c.a);
        static float Luminance(Color c) => c.r * .299f + c.g * .587f + c.b * .114f;
    }
}
