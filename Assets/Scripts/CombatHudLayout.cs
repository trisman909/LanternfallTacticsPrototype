using System.Linq;
using UnityEngine;

namespace Lanternfall
{
    public sealed class CombatHudLayoutSnapshot
    {
        public Rect Header;
        public Rect[] StatChips;
        public Rect HazardNote;
        public Rect HelpButton;
        public Rect InfoButton;
        public Rect SelectedSkill;
        public Rect[] SkillCards;
        public Rect CancelButton;
        public Rect EndTurnButton;
        public Rect Message;

        public bool RequiredElementsFit(Rect panel)
        {
            return Contains(panel, Header)
                && StatChips.All(r => Contains(panel, r))
                && Contains(panel, HazardNote)
                && Contains(panel, HelpButton)
                && Contains(panel, InfoButton)
                && Contains(panel, SelectedSkill)
                && SkillCards.All(r => Contains(panel, r))
                && Contains(panel, CancelButton)
                && Contains(panel, EndTurnButton)
                && Contains(panel, Message);
        }

        public bool HasEssentialOverlap()
        {
            var essential = new[] { Header, HazardNote, HelpButton, InfoButton, SelectedSkill, CancelButton, EndTurnButton, Message }
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

        public float MinimumControlHeight()
        {
            return new[] { HelpButton.height, InfoButton.height, CancelButton.height, EndTurnButton.height }
                .Concat(SkillCards.Select(r => r.height))
                .Min();
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
            bool phoneLandscape = compact && panel.height < 500f && panel.width >= 320f && panel.width > panel.height * .75f;
            bool phonePortrait = portrait && panel.width < 520f;
            bool ultraShortPhonePortrait = phonePortrait && panel.height < 570f;
            bool ultraShortPhoneLandscape = phoneLandscape && panel.height < 360f;
            bool shortPanel = compact && panel.height < 420f && !phoneLandscape;
            float pad = shortPanel ? 8f : phoneLandscape || phonePortrait ? 10f : 12f;
            float gap = ultraShortPhonePortrait ? 4f : ultraShortPhoneLandscape ? 1f : shortPanel || phoneLandscape ? 3f : phonePortrait ? 6f : compact ? 8f : 10f;
            float x = panel.x + pad;
            float w = panel.width - pad * 2f;
            float y = panel.y + (phoneLandscape ? 4f : phonePortrait ? 10f : shortPanel ? 10f : compact ? 14f : 16f);
            var snap = new CombatHudLayoutSnapshot();

            snap.Header = new Rect(x, y, w, shortPanel ? 36f : ultraShortPhonePortrait ? 28f : phonePortrait ? 34f : portrait ? 44f : ultraShortPhoneLandscape ? 20f : phoneLandscape ? 30f : compact ? 56f : 58f);
            y += snap.Header.height + gap;

            float chipGap = 6f;
            float chipW = (w - chipGap * 2f) / 3f;
            float chipH = shortPanel ? 30f : ultraShortPhonePortrait ? 44f : phonePortrait ? 48f : portrait ? 44f : ultraShortPhoneLandscape ? 44f : phoneLandscape ? 44f : compact ? 42f : 44f;
            snap.StatChips = new[]
            {
                new Rect(x, y, chipW, chipH),
                new Rect(x + chipW + chipGap, y, chipW, chipH),
                new Rect(x + (chipW + chipGap) * 2f, y, chipW, chipH)
            };
            y += chipH + gap;

            snap.HazardNote = new Rect(x, y, w, shortPanel ? 24f : ultraShortPhonePortrait ? 28f : phonePortrait ? 32f : portrait ? 44f : ultraShortPhoneLandscape ? 18f : phoneLandscape ? 26f : compact ? 46f : 50f);
            y += snap.HazardNote.height + gap;

            float smallButtonH = ultraShortPhonePortrait ? 48f : phonePortrait ? 52f : ultraShortPhoneLandscape ? 50f : phoneLandscape ? 50f : shortPanel ? 44f : 48f;
            snap.HelpButton = new Rect(x, y, w * .48f, smallButtonH);
            snap.InfoButton = new Rect(x + w * .52f, y, w * .48f, smallButtonH);
            y += smallButtonH + gap;

            snap.SelectedSkill = new Rect(x, y, w, shortPanel ? 20f : ultraShortPhonePortrait ? 20f : phonePortrait ? 22f : portrait ? 28f : ultraShortPhoneLandscape ? 14f : phoneLandscape ? 18f : compact ? 28f : 32f);
            y += snap.SelectedSkill.height + gap;

            bool rowSkills = (portrait && !phonePortrait) || (compact && panel.height < 520f);
            if (rowSkills)
            {
                float cardW = (w - gap * 2f) / 3f;
                float cardH = shortPanel ? 56f : phonePortrait ? 82f : portrait ? 84f : phoneLandscape ? 64f : 66f;
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
                float cardH = ultraShortPhonePortrait ? 56f : phonePortrait ? 66f : phoneLandscape ? 58f : compact ? 88f : 92f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, w, cardH),
                    new Rect(x, y + cardH + gap, w, cardH),
                    new Rect(x, y + (cardH + gap) * 2f, w, cardH)
                };
                y += cardH * 3f + gap * 3f;
            }

            float actionH = shortPanel ? 44f : ultraShortPhonePortrait ? 60f : phonePortrait ? 68f : portrait ? 58f : ultraShortPhoneLandscape ? 56f : phoneLandscape ? 56f : compact ? 52f : 54f;
            snap.CancelButton = new Rect(x, y, w * .48f, actionH);
            snap.EndTurnButton = new Rect(x + w * .52f, y, w * .48f, actionH);
            y += actionH + gap;

            snap.Message = new Rect(x, y, w, Mathf.Max(ultraShortPhonePortrait ? 42f : ultraShortPhoneLandscape ? 28f : shortPanel || phoneLandscape ? 42f : phonePortrait ? 48f : 56f, panel.yMax - y - pad));
            return snap;
        }
    }
}
