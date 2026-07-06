using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public static class EnemyAI
    {
        public static HashSet<Vector2Int> BuildPreview(EnemyModel e, Vector2Int player, GridModel grid)
        {
            var r = new HashSet<Vector2Int>();
            if (e.Kind == EnemyKind.GloomArcher)
            {
                var delta = player - e.Position;
                if (delta.x == 0 || delta.y == 0)
                {
                    var step = new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y)); var p = e.Position + step;
                    for(int i=0;i<4 && grid.IsFloor(p);i++,p+=step) r.Add(p);
                }
            }
            else
            {
                int radius = e.Kind == EnemyKind.LanternWarden ? 2 : 1;
                foreach(var p in grid.Floors()) if(Mathf.Abs(p.x-e.Position.x)+Mathf.Abs(p.y-e.Position.y)<=radius && p!=e.Position) r.Add(p);
            }
            return r;
        }
    }
}
