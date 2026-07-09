using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public enum BiomeId { DrownedNarthex, SiltglassObservatory, EmberOssuary, GloamOrchard, StormvaultFoundry }
    public enum HazardKind { ShallowWater, Prism, EmberVent, GraspingRoots, ChargedFloor }

    public sealed class BiomeTheme
    {
        public BiomeId Id; public string Name; public string StableId; public HazardKind Hazard;
        public string HazardName; public string HazardRule; public string PropGlyph;
        public Color Background; public Color Floor; public Color Alternate; public Color HazardColor; public Color WarningColor; public Color Accent;
        public float TileContrast => Mathf.Abs(Luminance(Floor)-Luminance(HazardColor));
        static float Luminance(Color c)=>.2126f*c.r+.7152f*c.g+.0722f*c.b;
    }

    public static class BiomeCatalog
    {
        public static readonly BiomeTheme[] All =
        {
            new(){Id=BiomeId.DrownedNarthex,Name="The Drowned Narthex",StableId="biome.drowned_narthex",Hazard=HazardKind.ShallowWater,HazardName="Shallow Water",HazardRule="Water reduces movement by 1",PropGlyph="T",Background=new(.035f,.065f,.09f),Floor=new(.12f,.25f,.28f),Alternate=new(.16f,.34f,.34f),HazardColor=new(.20f,.55f,.58f),WarningColor=new(.55f,.95f,.90f),Accent=new(.52f,1f,.78f)},
            new(){Id=BiomeId.SiltglassObservatory,Name="Siltglass Observatory",StableId="biome.siltglass_observatory",Hazard=HazardKind.Prism,HazardName="Prism Glass",HazardRule="Prisms give damage skills +1 range and +1 damage",PropGlyph="<>",Background=new(.12f,.10f,.16f),Floor=new(.25f,.18f,.12f),Alternate=new(.52f,.41f,.30f),HazardColor=new(.58f,.25f,.72f),WarningColor=new(.82f,.42f,1f),Accent=new(1f,.88f,.70f)},
            new(){Id=BiomeId.EmberOssuary,Name="The Ember Ossuary",StableId="biome.ember_ossuary",Hazard=HazardKind.EmberVent,HazardName="Ember Vent",HazardRule="Vents warn, then deal 2 damage next turn",PropGlyph="B",Background=new(.16f,.045f,.025f),Floor=new(.27f,.16f,.13f),Alternate=new(.38f,.23f,.17f),HazardColor=new(.62f,.18f,.06f),WarningColor=new(1f,.45f,.08f),Accent=new(1f,.68f,.57f)},
            new(){Id=BiomeId.GloamOrchard,Name="The Gloam Orchard",StableId="biome.gloam_orchard",Hazard=HazardKind.GraspingRoots,HazardName="Grasping Roots",HazardRule="Roots reduce movement to 2 tiles",PropGlyph="M",Background=new(.035f,.09f,.06f),Floor=new(.13f,.24f,.16f),Alternate=new(.23f,.22f,.31f),HazardColor=new(.55f,.16f,.68f),WarningColor=new(.69f,1f,.74f),Accent=new(.38f,1f,.48f)},
            new(){Id=BiomeId.StormvaultFoundry,Name="Stormvault Foundry",StableId="biome.stormvault_foundry",Hazard=HazardKind.ChargedFloor,HazardName="Charged Floor",HazardRule="Charged plates warn, then zap adjacent units",PropGlyph="C",Background=new(.055f,.07f,.13f),Floor=new(.16f,.20f,.26f),Alternate=new(.27f,.25f,.24f),HazardColor=new(.18f,.38f,.60f),WarningColor=new(.28f,.75f,1f),Accent=new(.68f,.89f,1f)}
        };
        public static BiomeTheme Get(BiomeId id)=>All[(int)id];
        public static BiomeTheme ForRoom(int room)=>All[Mathf.Clamp(room-1,0,4)];
    }

    public static class BiomeRules
    {
        public static int MoveRange(PlayerModel player, BiomeTheme theme, ISet<Vector2Int> hazards)
        {
            if(!hazards.Contains(player.Position)) return player.MoveRange;
            return theme.Hazard switch { HazardKind.ShallowWater=>Mathf.Max(1,player.MoveRange-1), HazardKind.GraspingRoots=>Mathf.Min(player.MoveRange,2), _=>player.MoveRange };
        }
        public static int SkillRangeBonus(BiomeTheme theme, Vector2Int position, ISet<Vector2Int> hazards, SkillId skill)=>theme.Hazard==HazardKind.Prism&&hazards.Contains(position)&&SkillBook.Get(skill).Damage>0?1:0;
        public static int SkillDamageBonus(BiomeTheme theme, Vector2Int position, ISet<Vector2Int> hazards, SkillId skill)=>theme.Hazard==HazardKind.Prism&&hazards.Contains(position)&&SkillBook.Get(skill).Damage>0?1:0;
        public static bool IsDelayedDamage(BiomeTheme theme)=>theme.Hazard==HazardKind.EmberVent||theme.Hazard==HazardKind.ChargedFloor;
        public static int HazardDamage(BiomeTheme theme, Vector2Int position, ISet<Vector2Int> armed)
        {
            if(theme.Hazard==HazardKind.EmberVent) return armed.Contains(position)?2:0;
            if(theme.Hazard==HazardKind.ChargedFloor) foreach(var h in armed)if(Mathf.Abs(h.x-position.x)+Mathf.Abs(h.y-position.y)<=1)return 2;
            return 0;
        }
    }
}
