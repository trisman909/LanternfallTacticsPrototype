using UnityEngine;

namespace Lanternfall
{
    public static class PresentationMotion
    {
        public const string PreferenceKey = "LanternfallTactics.ReducedMotion";
        public static bool Reduced { get => PlayerPrefs.GetInt(PreferenceKey, 0) == 1; set { PlayerPrefs.SetInt(PreferenceKey, value ? 1 : 0); PlayerPrefs.Save(); } }
        public static float Duration(float full, float reduced = .06f) => Reduced ? reduced : full;
        public static float Ease(float t) { t = Mathf.Clamp01(t); return 1f - Mathf.Pow(1f - t, 3f); }
    }
}
