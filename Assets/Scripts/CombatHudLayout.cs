using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class CombatHudLayoutSnapshot
    {
        public Rect Header;
        public Rect[] StatChips;
        public Rect Message;
        public Rect HelpButton;
        public Rect InfoButton;
        public Rect SelectedSkill;
        public Rect[] SkillCards;
        public Rect CancelButton;
        public Rect EndTurnButton;
        public Rect HazardLegend;

        public bool RequiredElementsFit(Rect panel)
        {
            return Contains(panel, Header)
                && StatChips.All(r => Contains(panel, r))
                && Contains(panel, Message)
                && Contains(panel, HelpButton)
                && Contains(panel, InfoButton)
                && Contains(panel, SelectedSkill)
                && SkillCards.All(r => Contains(panel, r))
                && Contains(panel, CancelButton)
                && Contains(panel, EndTurnButton)
                && Contains(panel, HazardLegend);
        }

        public bool HasEssentialOverlap()
        {
            var essential = new[] { Header, Message, HelpButton, InfoButton, SelectedSkill, CancelButton, EndTurnButton, HazardLegend }
                .Concat(StatChips)
                .Concat(SkillCards)
                .ToArray();
            for (int i = 0; i < essential.Length; i++)
            for (int j = i + 1; j < essential.Length; j++)
                if (essential[i].Overlaps(essential[j])) return true;
            return false;
        }

        public bool TouchTargetsValid(float minimum = MobileLayoutSnapshot.MinimumTouchTarget)
        {
            return SkillCards.All(r => r.width >= minimum && r.height >= minimum)
                && HelpButton.width >= minimum && HelpButton.height >= minimum
                && EndTurnButton.width >= minimum && EndTurnButton.height >= minimum
                && CancelButton.width >= minimum && CancelButton.height >= minimum;
        }

        static bool Contains(Rect outer, Rect inner)
        {
            return inner.xMin >= outer.xMin && inner.xMax <= outer.xMax && inner.yMin >= outer.yMin && inner.yMax <= outer.yMax;
        }
    }

    public static class CombatHudLayout
    {
        public static CombatHudLayoutSnapshot Compute(Rect panel, bool portrait, bool compact)
        {
            float pad = 10f;
            float gap = compact ? 6f : 8f;
            float x = panel.x + pad;
            float w = panel.width - pad * 2f;
            float y = panel.y + (compact ? 6f : 10f);
            bool shortPanel = compact && panel.height < 420f;
            var snap = new CombatHudLayoutSnapshot();

            snap.Header = new Rect(x, y, w, shortPanel ? 28f : portrait ? 28f : compact ? 34f : 44f);
            y += snap.Header.height + gap;

            float chipGap = 6f;
            float chipW = (w - chipGap * 2f) / 3f;
            float chipH = shortPanel ? 32f : portrait ? 36f : compact ? 38f : 42f;
            snap.StatChips = new[]
            {
                new Rect(x, y, chipW, chipH),
                new Rect(x + chipW + chipGap, y, chipW, chipH),
                new Rect(x + (chipW + chipGap) * 2f, y, chipW, chipH)
            };
            y += chipH + gap;

            snap.Message = new Rect(x, y, w, shortPanel ? 36f : portrait ? 38f : compact ? 48f : 58f);
            y += snap.Message.height + gap;

            float smallButtonH = 48f;
            snap.HelpButton = new Rect(x, y, w * .48f, smallButtonH);
            snap.InfoButton = new Rect(x + w * .52f, y, w * .48f, smallButtonH);
            y += smallButtonH + gap;

            snap.SelectedSkill = new Rect(x, y, w, shortPanel ? 24f : portrait ? 28f : compact ? 30f : 34f);
            y += snap.SelectedSkill.height + gap;

            bool rowSkills = portrait || (compact && panel.height < 520f);
            if (rowSkills)
            {
                float cardW = (w - gap * 2f) / 3f;
                float cardH = shortPanel ? 58f : portrait ? 72f : 66f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, cardW, cardH),
                    new Rect(x + cardW + gap, y, cardW, cardH),
                    new Rect(x + (cardW + gap) * 2f, y, cardW, cardH)
                };
                y += cardH + gap;
            }
            else
            {
                float cardH = compact ? 72f : 82f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, w, cardH),
                    new Rect(x, y + cardH + gap, w, cardH),
                    new Rect(x, y + (cardH + gap) * 2f, w, cardH)
                };
                y += cardH * 3f + gap * 3f;
            }

            float actionH = shortPanel ? 42f : portrait ? 50f : compact ? 48f : 54f;
            snap.CancelButton = new Rect(x, y, w * .48f, actionH);
            snap.EndTurnButton = new Rect(x + w * .52f, y, w * .48f, actionH);
            y += actionH + gap;

            snap.HazardLegend = new Rect(x, y, w, Mathf.Max(shortPanel ? 24f : 36f, panel.yMax - y - pad));
            return snap;
        }
    }
}
