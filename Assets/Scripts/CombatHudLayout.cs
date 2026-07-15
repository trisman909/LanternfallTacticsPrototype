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
                && (HelpButton.height <= 0f || HelpButton.width >= minimum && HelpButton.height >= minimum)
                && (InfoButton.height <= 0f || InfoButton.width >= minimum && InfoButton.height >= minimum)
                && EndTurnButton.width >= minimum && EndTurnButton.height >= minimum
                && CancelButton.width >= minimum && CancelButton.height >= minimum;
        }

        public float MinimumControlHeight()
        {
            return new[] { HelpButton.height, InfoButton.height, CancelButton.height, EndTurnButton.height }
                .Concat(SkillCards.Select(r => r.height))
                .Where(h => h > 0f)
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
            bool phoneLandscape = compact && panel.height <= 230f && panel.width >= 320f && panel.width > panel.height * 2.4f;
            bool phonePortrait = portrait && panel.width < 520f;
            bool ultraShortPhonePortrait = phonePortrait && panel.height < 620f;
            bool ultraShortPhoneLandscape = phoneLandscape && panel.height < 360f;
            bool shortPanel = compact && panel.height < 420f && !phoneLandscape;
            float pad = shortPanel ? 8f : phoneLandscape || phonePortrait ? 10f : 12f;
            float gap = ultraShortPhonePortrait ? 4f : ultraShortPhoneLandscape ? 5f : shortPanel ? 4f : phoneLandscape ? 5f : phonePortrait ? 6f : compact ? 8f : 10f;
            float x = panel.x + pad;
            float w = panel.width - pad * 2f;
            float y = panel.y + (phoneLandscape ? 6f : phonePortrait ? 10f : shortPanel ? 10f : compact ? 14f : 16f);
            var snap = new CombatHudLayoutSnapshot();

            if (phonePortrait)
            {
                float phoneChipGap = 6f;
                float phoneChipH = panel.height < 405f ? 54f : 60f;
                float phoneChipW = (w - phoneChipGap * 2f) / 3f;
                snap.Header = new Rect(x, y, w, 0f);
                snap.StatChips = new[]
                {
                    new Rect(x, y, phoneChipW, phoneChipH),
                    new Rect(x + phoneChipW + phoneChipGap, y, phoneChipW, phoneChipH),
                    new Rect(x + (phoneChipW + phoneChipGap) * 2f, y, phoneChipW, phoneChipH)
                };
                y += phoneChipH + gap;

                float utilityW = 52f;
                float utilityH = 48f;
                snap.HazardNote = new Rect(x, y, w - utilityW * 2f - gap * 2f, utilityH);
                snap.HelpButton = new Rect(x + w - utilityW * 2f - gap, y, utilityW, utilityH);
                snap.InfoButton = new Rect(x + w - utilityW, y, utilityW, utilityH);
                y += utilityH + gap;

                snap.SelectedSkill = new Rect(x, y, w, 0f);
                float cardH = panel.height < 405f ? 76f : 86f;
                float halfW = (w - gap) / 2f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, halfW, cardH),
                    new Rect(x + halfW + gap, y, halfW, cardH),
                    new Rect(x, y + cardH + gap, w, cardH)
                };
                y += cardH * 2f + gap * 2f;

                float phoneActionH = panel.height < 405f ? 72f : 80f;
                snap.CancelButton = new Rect(x, y, w * .27f, phoneActionH);
                snap.EndTurnButton = new Rect(x + w * .30f, y, w * .70f, phoneActionH);
                y += phoneActionH + gap;

                float phoneRemaining = Mathf.Max(0f, panel.yMax - y - pad);
                snap.Message = new Rect(x, y, w, phoneRemaining);
                return snap;
            }

            if (phoneLandscape)
            {
                float landChipGap = 6f;
                float landChipH = Mathf.Clamp(panel.height * .25f, 44f, 52f);
                float landChipW = (w - landChipGap * 2f) / 3f;
                snap.Header = new Rect(x, y, w, 0f);
                snap.StatChips = new[]
                {
                    new Rect(x, y, landChipW, landChipH),
                    new Rect(x + landChipW + landChipGap, y, landChipW, landChipH),
                    new Rect(x + (landChipW + landChipGap) * 2f, y, landChipW, landChipH)
                };
                snap.HazardNote = new Rect(x, y, 0f, 0f);
                snap.HelpButton = new Rect(x, y, 0f, 0f);
                snap.InfoButton = new Rect(x, y, 0f, 0f);
                y += landChipH + gap;

                snap.SelectedSkill = new Rect(x, y, w, 0f);
                float phoneActionH = Mathf.Clamp(panel.height * .30f, 56f, 68f);
                float cardH = Mathf.Max(60f, panel.yMax - y - pad - phoneActionH - gap);
                float skillW = (w - gap * 2f) / 3f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, skillW, cardH),
                    new Rect(x + skillW + gap, y, skillW, cardH),
                    new Rect(x + (skillW + gap) * 2f, y, skillW, cardH)
                };
                y += cardH + gap;
                snap.CancelButton = new Rect(x, y, Mathf.Max(68f, w * .24f), phoneActionH);
                float endTurnW = Mathf.Max(180f, w * .48f);
                snap.EndTurnButton = new Rect(x + (w - endTurnW) * .5f, y, endTurnW, phoneActionH);
                snap.Message = new Rect(x, panel.yMax - 1f, w, 0f);
                return snap;
            }

            float headerH = phonePortrait || phoneLandscape ? 0f : shortPanel ? 36f : portrait ? 44f : compact ? 56f : 58f;
            snap.Header = new Rect(x, y, w, headerH);
            y += snap.Header.height + (headerH > 0f ? gap : 0f);

            float chipGap = 6f;
            float chipW = (w - chipGap * 2f) / 3f;
            float chipH = shortPanel ? 30f : ultraShortPhonePortrait ? 66f : phonePortrait ? 78f : portrait ? 44f : ultraShortPhoneLandscape ? 58f : phoneLandscape ? 66f : compact ? 42f : 44f;
            snap.StatChips = new[]
            {
                new Rect(x, y, chipW, chipH),
                new Rect(x + chipW + chipGap, y, chipW, chipH),
                new Rect(x + (chipW + chipGap) * 2f, y, chipW, chipH)
            };
            y += chipH + gap;

            snap.HazardNote = new Rect(x, y, w, shortPanel ? 24f : ultraShortPhonePortrait ? 36f : phonePortrait ? 48f : portrait ? 44f : ultraShortPhoneLandscape ? 30f : phoneLandscape ? 36f : compact ? 46f : 50f);
            y += snap.HazardNote.height + gap;

            float smallButtonH = phonePortrait || phoneLandscape ? 0f : shortPanel ? 44f : 48f;
            snap.HelpButton = new Rect(x, y, w * .48f, smallButtonH);
            snap.InfoButton = new Rect(x + w * .52f, y, w * .48f, smallButtonH);
            y += smallButtonH + (smallButtonH > 0 ? gap : 0);

            snap.SelectedSkill = new Rect(x, y, w, shortPanel ? 20f : phonePortrait ? 0f : portrait ? 28f : phoneLandscape ? 0f : compact ? 28f : 32f);
            y += snap.SelectedSkill.height + (snap.SelectedSkill.height > 0 ? gap : 0);

            bool rowSkills = (portrait && !phonePortrait) || phoneLandscape || (compact && panel.height < 520f);
            if (rowSkills)
            {
                float cardW = (w - gap * 2f) / 3f;
                float cardH = shortPanel ? 56f : phonePortrait ? 82f : portrait ? 84f : ultraShortPhoneLandscape ? 96f : phoneLandscape ? 112f : 66f;
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
                float cardH = ultraShortPhonePortrait ? 90f : phonePortrait ? 112f : compact ? 88f : 92f;
                snap.SkillCards = new[]
                {
                    new Rect(x, y, w, cardH),
                    new Rect(x, y + cardH + gap, w, cardH),
                    new Rect(x, y + (cardH + gap) * 2f, w, cardH)
                };
                y += cardH * 3f + gap * 3f;
            }

            float actionH = shortPanel ? 44f : ultraShortPhonePortrait ? 86f : phonePortrait ? 98f : portrait ? 58f : ultraShortPhoneLandscape ? 76f : phoneLandscape ? 86f : compact ? 52f : 54f;
            snap.CancelButton = phonePortrait || phoneLandscape ? new Rect(x, y, w * .25f, actionH) : new Rect(x, y, w * .48f, actionH);
            snap.EndTurnButton = phonePortrait || phoneLandscape ? new Rect(x + w * .28f, y, w * .72f, actionH) : new Rect(x + w * .52f, y, w * .48f, actionH);
            y += actionH + gap;

            float minMessage = ultraShortPhonePortrait ? 22f : phonePortrait ? 34f : ultraShortPhoneLandscape ? 18f : phoneLandscape ? 24f : shortPanel ? 42f : 56f;
            float remaining = Mathf.Max(0f, panel.yMax - y - pad);
            snap.Message = new Rect(x, y, w, Mathf.Min(Mathf.Max(minMessage, remaining), remaining));
            return snap;
        }
    }
}
