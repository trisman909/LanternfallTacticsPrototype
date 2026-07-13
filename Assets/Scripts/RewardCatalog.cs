using System.Linq;

namespace Lanternfall
{
    public sealed class RewardOption
    {
        public string Name;
        public string ShortName;
        public string Effect;
        public string FullLabel => $"{Name}\n{Effect}";
        public string CompactLabel => $"{ShortName}\n{Effect}";
    }

    public static class RewardCatalog
    {
        public static readonly RewardOption[] All =
        {
            new(){Name="Vital Ember",ShortName="Vital",Effect="+3 Max HP"},
            new(){Name="Bright Wick",ShortName="Wick",Effect="+1 Skill Damage"},
            new(){Name="Swift Flame",ShortName="Swift",Effect="+1 MP Move"}
        };

        public static RewardOption Get(int index) => All[index];
        public static bool LabelsReadable => All.Length == 3 && All.All(r => !string.IsNullOrWhiteSpace(r.Name) && !string.IsNullOrWhiteSpace(r.Effect) && r.FullLabel.Length <= 32 && r.CompactLabel.Length <= 24);
    }
}
