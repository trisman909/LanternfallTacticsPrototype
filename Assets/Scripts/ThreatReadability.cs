using UnityEngine;

namespace Lanternfall
{
    public static class ThreatReadability
    {
        public static string TileMarker(ThreatKind threat) => threat switch
        {
            ThreatKind.AP => "◆",
            ThreatKind.MP => "◆",
            ThreatKind.Mixed => "✦",
            _ => "•"
        };

        public static Color TileMarkerColor(ThreatKind threat) => threat switch
        {
            ThreatKind.AP => new Color(.86f, .50f, 1f),
            ThreatKind.MP => new Color(.72f, .42f, 1f),
            ThreatKind.Mixed => new Color(1f, .54f, .18f),
            _ => new Color(1f, .64f, .86f)
        };

        public static string EnemyBadge(EnemyModel e) => e.Threat switch
        {
            ThreatKind.AP => "AP",
            ThreatKind.MP => "MP",
            ThreatKind.Mixed => EnemyAI.BossPhase(e) >= 3 ? "BLAST" : e.Kind == EnemyKind.LanternWarden ? "OVER" : "MIX",
            _ => e.Kind == EnemyKind.Ashling ? "HP" : "CAST"
        };

        public static string ThreatName(ThreatKind threat) => threat switch
        {
            ThreatKind.AP => "AP drain",
            ThreatKind.MP => "MP bind",
            ThreatKind.Mixed => "overcharge AP/MP threat",
            _ => "HP damage"
        };

        public static bool IsCompactTileMarker(string marker) => !string.IsNullOrWhiteSpace(marker) && marker.Length <= 1 && marker != "AP" && marker != "MP";
    }
}
