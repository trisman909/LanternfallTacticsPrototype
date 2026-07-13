using System.Linq;

namespace Lanternfall
{
    public sealed class RewardOption
    {
        public string Name;
        public string ShortName;
        public string Effect;
        public string Detail;
        public string FullLabel => $"{Name}\n{Effect}\n{Detail}";
        public string CompactLabel => $"{Name}\n{Effect}";
    }

    public static class RewardCatalog
    {
        public static readonly RewardOption[] All =
        {
            new(){Name="Vital Ember",ShortName="Vital",Effect="+3 Max HP",Detail="Heal 3 now"},
            new(){Name="Bright Wick",ShortName="Wick",Effect="+1 all skill damage",Detail="Every class benefits"},
            new(){Name="Swift Flame",ShortName="Swift",Effect="+1 MP movement",Detail="Move farther each turn"}
        };

        public static RewardOption Get(int index) => All[index];
        public static string WebGLCardLabel(int index)
        {
            var r = Get(index);
            return $"{r.Name}\n{r.Effect}\n{r.Detail}";
        }
        public static bool LabelsReadable => All.Length == 3 && All.All(r => !string.IsNullOrWhiteSpace(r.Name) && !string.IsNullOrWhiteSpace(r.Effect) && !string.IsNullOrWhiteSpace(r.Detail) && r.Name.Length <= 14 && r.Effect.Length <= 22 && r.Detail.Length <= 24);
    }
}
