using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class RewardPanelLayoutSnapshot
    {
        public Rect Header;
        public Rect[] Cards;
        public bool Fits(Rect bounds) => Header.xMin >= bounds.xMin && Header.yMin >= bounds.yMin && Header.xMax <= bounds.xMax && Header.yMax <= bounds.yMax
            && Cards.All(r => r.xMin >= bounds.xMin && r.yMin >= bounds.yMin && r.xMax <= bounds.xMax && r.yMax <= bounds.yMax);
        public bool HasOverlap()
        {
            foreach (var c in Cards) if (Header.Overlaps(c)) return true;
            for (int i = 0; i < Cards.Length; i++)
            for (int j = i + 1; j < Cards.Length; j++)
                if (Cards[i].Overlaps(Cards[j])) return true;
            return false;
        }
    }

    public static class RewardPanelLayout
    {
        public const float SideHeaderHeight = 50f;
        public const float PortraitHeaderHeight = 54f;
        public const float SideCardHeight = 76f;
        public const float PortraitCardHeight = 82f;
        public const float Gap = 8f;

        public static RewardPanelLayoutSnapshot Compute(float x, float y, float w, bool sidePanel)
        {
            var snap = new RewardPanelLayoutSnapshot();
            float headerH = sidePanel ? SideHeaderHeight : PortraitHeaderHeight;
            snap.Header = new Rect(x, y, w, headerH);
            float cardY = y + headerH + Gap;
            if (sidePanel)
            {
                snap.Cards = new[]
                {
                    new Rect(x, cardY, w, SideCardHeight),
                    new Rect(x, cardY + SideCardHeight + Gap, w, SideCardHeight),
                    new Rect(x, cardY + (SideCardHeight + Gap) * 2f, w, SideCardHeight)
                };
            }
            else
            {
                float cardW = (w - Gap * 2f) / 3f;
                snap.Cards = new[]
                {
                    new Rect(x, cardY, cardW, PortraitCardHeight),
                    new Rect(x + cardW + Gap, cardY, cardW, PortraitCardHeight),
                    new Rect(x + (cardW + Gap) * 2f, cardY, cardW, PortraitCardHeight)
                };
            }
            return snap;
        }
    }
}
