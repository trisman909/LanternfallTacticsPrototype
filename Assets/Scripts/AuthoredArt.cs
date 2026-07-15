using UnityEngine;

namespace Lanternfall
{
    public enum UiSkin
    {
        SkillCard, StatChip, EndTurn, Utility, SelectedSkill,
        RewardCard, VictoryPanel, DefeatPanel, Tooltip
    }

    public static class AuthoredArt
    {
        public const string IconAtlasResource = "UI/phase6d_icon_atlas";
        public const string UiAtlasResource = "UI/phase6d_ui_atlas";
        public const int IconColumns = 5;
        public const int IconRows = 5;
        public const int UiColumns = 3;
        public const int UiRows = 3;

        static Texture2D iconAtlas;
        static Texture2D uiAtlas;
        static bool iconLoadAttempted;
        static bool uiLoadAttempted;

        public static bool DrawIcon(Rect rect, VisualIcon icon)
        {
            if (!iconLoadAttempted) { iconAtlas = Resources.Load<Texture2D>(IconAtlasResource); iconLoadAttempted = true; }
            if (iconAtlas == null) return false;
            var uv = CellUv(IconLanguage.AtlasCell(icon), IconColumns, IconRows);
            float insetX = uv.width * .17f, insetY = uv.height * .17f;
            uv = new Rect(uv.x + insetX, uv.y + insetY, uv.width - insetX * 2f, uv.height - insetY * 2f);
            GUI.DrawTextureWithTexCoords(rect, iconAtlas, uv, true);
            return true;
        }

        public static bool DrawSkin(Rect rect, UiSkin skin)
            =>DrawSkin(rect,skin,Color.white);

        public static bool DrawSkin(Rect rect, UiSkin skin,Color tint)
        {
            if (!uiLoadAttempted) { uiAtlas = Resources.Load<Texture2D>(UiAtlasResource); uiLoadAttempted = true; }
            if (uiAtlas == null) return false;
            var old=GUI.color; GUI.color=tint;
            GUI.DrawTextureWithTexCoords(rect, uiAtlas, CellUv((int)skin, UiColumns, UiRows), true);
            GUI.color=old;
            return true;
        }

        public static Rect CellUv(int index, int columns, int rows)
        {
            int col = Mathf.Clamp(index % columns, 0, columns - 1);
            int row = Mathf.Clamp(index / columns, 0, rows - 1);
            return new Rect(col / (float)columns, 1f - (row + 1f) / rows, 1f / columns, 1f / rows);
        }
    }
}
