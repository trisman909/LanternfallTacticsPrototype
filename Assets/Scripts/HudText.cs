namespace Lanternfall
{
    public static class HudText
    {
        public const string HelpButton = "? HELP";
        public const string InfoButton = "INFO";
        public const string EndTurnButton = "END TURN";
        public const string CancelSkillButton = "CANCEL SKILL";

        public static string Hp(int current, int max) => $"HP {current}/{max}";
        public static string Ap(int current, int max) => $"AP {current}/{max}";
        public static string Mp(int current, int max) => $"MP {current}/{max}";

        public static string TurnLabel(TurnPhase phase)
        {
            return phase switch
            {
                TurnPhase.Player => "PLAYER TURN",
                TurnPhase.Enemy => "ENEMY TURN",
                TurnPhase.Reward => "ROOM CLEAR",
                TurnPhase.Won => "VICTORY",
                TurnPhase.Lost => "DEFEAT",
                _ => phase.ToString().ToUpperInvariant()
            };
        }

        public static string SkillState(SkillDefinition skill, int cooldown, int currentAp, TurnPhase phase)
        {
            if (phase != TurnPhase.Player) return "WAIT";
            if (cooldown > 0) return $"CD {cooldown}";
            if (currentAp < skill.ApCost) return $"NEED {skill.ApCost} AP";
            return "READY";
        }

        public static string SkillCard(SkillDefinition skill, int cooldown, int currentAp, TurnPhase phase, bool selected, bool compact) => SkillCard(skill, cooldown, currentAp, phase, selected, compact, true);

        public static string SkillCard(SkillDefinition skill, int cooldown, int currentAp, TurnPhase phase, bool selected, bool compact, bool showDescription)
        {
            string prefix = selected ? "SELECTED - " : "";
            string state = SkillState(skill, cooldown, currentAp, phase);
            if (!showDescription) return $"{prefix}{skill.Name}\nAP {skill.ApCost} - {state}";
            return compact
                ? $"{prefix}{skill.Name}\nAP {skill.ApCost} - {state}\n{skill.Hint}"
                : $"{prefix}{skill.Name}\nAP {skill.ApCost} - {state}\n{skill.Hint}";
        }

        public static string MobileSkillCard(SkillDefinition skill, int cooldown, int currentAp, TurnPhase phase, bool selected)
        {
            string prefix = selected ? "SEL " : "";
            return $"{prefix}{MobileSkillName(skill)} AP {skill.ApCost}\n{SkillState(skill, cooldown, currentAp, phase)}";
        }

        public static string MobileSkillName(SkillDefinition skill)
        {
            return skill.Name switch
            {
                "Spear Thrust" => "SPEAR",
                "Guard Stance" => "GUARD",
                "Sun Charge" => "CHARGE",
                "Straight Shot" => "SHOT",
                "Marked Target" => "MARK",
                "Piercing Prism" => "PRISM",
                "Ember Bolt" => "BOLT",
                "Cinder Bloom" => "BLOOM",
                "Delayed Blast" => "BLAST",
                "Diagonal Dash" => "DASH",
                "Backstab" => "STAB",
                "Shadow Swap" => "SWAP",
                "Lens Trap" => "TRAP",
                "Redirect Shot" => "REDIRECT",
                "Shield Gadget" => "SHIELD",
                _ => skill.Name.ToUpperInvariant()
            };
        }
    }
}
