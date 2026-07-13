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

        public static string SkillCard(SkillDefinition skill, int cooldown, int currentAp, TurnPhase phase, bool selected, bool compact)
        {
            string prefix = selected ? "SELECTED - " : "";
            string state = SkillState(skill, cooldown, currentAp, phase);
            return compact
                ? $"{prefix}{skill.Name}\nAP {skill.ApCost} - {state}\n{skill.Hint}"
                : $"{prefix}{skill.Name}\nAP {skill.ApCost} - {state}\n{skill.Hint}";
        }
    }
}
