using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public static class CombatTelegraphValidator
    {
        public static int MismatchCount { get; private set; }
        public static void Reset()=>MismatchCount=0;

        public static bool AllowsDamage(string source,Vector2Int damageTile,ISet<Vector2Int> telegraphedTiles)
        {
            if(telegraphedTiles!=null&&telegraphedTiles.Contains(damageTile))return true;
            MismatchCount++;
            Debug.LogError($"TELEGRAPH_MISMATCH source={source} damageTile={damageTile} previewCount={telegraphedTiles?.Count??0}");
            return false;
        }
    }
}
