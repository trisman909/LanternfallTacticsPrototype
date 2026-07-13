using UnityEngine;

namespace Lanternfall
{
    public readonly struct BoardFitSnapshot
    {
        public readonly Rect Bounds;
        public readonly float TileSize;
        public BoardFitSnapshot(Rect bounds, float tileSize)
        {
            Bounds = bounds;
            TileSize = tileSize;
        }
        public bool Fits(Rect area) => Bounds.xMin >= area.xMin && Bounds.yMin >= area.yMin && Bounds.xMax <= area.xMax && Bounds.yMax <= area.yMax;
    }

    public static class BoardFitLayout
    {
        public static BoardFitSnapshot Compute(Rect area, int columns, int rows, bool compact)
        {
            float top = compact ? 42f : 64f;
            columns = Mathf.Max(1, columns);
            rows = Mathf.Max(1, rows);
            float tile = Mathf.Min((area.width - 18f) / columns, (area.height - top - 10f) / rows);
            float width = columns * tile;
            float height = rows * tile;
            float x = area.x + (area.width - width) / 2f;
            float y = area.y + top + (area.height - top - height) / 2f;
            return new BoardFitSnapshot(new Rect(x, y, width, height), tile);
        }
    }
}
