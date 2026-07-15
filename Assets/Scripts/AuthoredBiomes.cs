using UnityEngine;

namespace Lanternfall
{
    public enum BiomeArtCell { Floor = 0, Alternate = 1, Obstacle = 2, Hazard = 3, HealingPickup = 4, PropA = 5, PropB = 6, PropC = 7, BossAccent = 8 }

    public static class AuthoredBiomes
    {
        public const int Columns = 3;
        public const int Rows = 3;
        static readonly Texture2D[] Atlases = new Texture2D[5];
        static readonly bool[] LoadAttempted = new bool[5];

        public static string Resource(BiomeId biome) => biome switch
        {
            BiomeId.DrownedNarthex => "Biomes/phase6f_drowned_narthex_atlas",
            BiomeId.SiltglassObservatory => "Biomes/phase6f_siltglass_observatory_atlas",
            BiomeId.EmberOssuary => "Biomes/phase6f_ember_ossuary_atlas",
            BiomeId.GloamOrchard => "Biomes/phase6f_gloam_orchard_atlas",
            BiomeId.StormvaultFoundry => "Biomes/phase6f_stormvault_foundry_atlas",
            _ => "Biomes/phase6f_drowned_narthex_atlas"
        };

        public static BiomeArtCell PropCell(Vector2Int position) =>
            (BiomeArtCell)((int)BiomeArtCell.PropA + Mathf.Abs(position.x * 31 + position.y * 17) % 3);

        public static bool Draw(Rect rect, BiomeId biome, BiomeArtCell cell, Color tint)
        {
            int index = (int)biome;
            if (!LoadAttempted[index]) { Atlases[index] = Resources.Load<Texture2D>(Resource(biome)); LoadAttempted[index] = true; }
            if (Atlases[index] == null) return false;
            var old = GUI.color;
            GUI.color = tint;
            GUI.DrawTextureWithTexCoords(rect, Atlases[index], AuthoredArt.CellUv((int)cell, Columns, Rows), true);
            GUI.color = old;
            return true;
        }
    }
}
