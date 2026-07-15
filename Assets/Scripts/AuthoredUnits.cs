using UnityEngine;

namespace Lanternfall
{
    public static class AuthoredUnits
    {
        public const string AtlasResource = "Units/phase6e_unit_atlas";
        public const int Columns = 3;
        public const int Rows = 3;
        static Texture2D atlas;
        static bool loadAttempted;

        public static int Cell(PlayerClassId playerClass) => playerClass switch
        {
            PlayerClassId.Vanguard => 0,
            PlayerClassId.Wayfinder => 1,
            PlayerClassId.Cantor => 2,
            PlayerClassId.Gloamstep => 3,
            PlayerClassId.Artificer => 4,
            _ => 2
        };

        public static int Cell(EnemyKind enemy) => enemy switch
        {
            EnemyKind.Ashling => 5,
            EnemyKind.GloomArcher => 6,
            EnemyKind.StoneSentinel => 7,
            EnemyKind.LanternWarden => 8,
            _ => 5
        };

        public static Color Tint(bool hit) => hit ? new Color(1f, .62f, .48f) : Color.white;
        public static bool IsOverchargedBoss(EnemyModel enemy) => enemy != null && enemy.Kind == EnemyKind.LanternWarden && EnemyAI.BossPhase(enemy) >= 2;

        public static bool Draw(Rect rect, PlayerClassId? playerClass, EnemyKind? enemy, Color tint)
        {
            if (!loadAttempted) { atlas = Resources.Load<Texture2D>(AtlasResource); loadAttempted = true; }
            if (atlas == null || (!playerClass.HasValue && !enemy.HasValue)) return false;
            int cell = playerClass.HasValue ? Cell(playerClass.Value) : Cell(enemy.Value);
            var old = GUI.color;
            GUI.color = tint;
            GUI.DrawTextureWithTexCoords(rect, atlas, AuthoredArt.CellUv(cell, Columns, Rows), true);
            GUI.color = old;
            return true;
        }
    }
}
