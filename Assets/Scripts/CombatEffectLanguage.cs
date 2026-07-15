using UnityEngine;

namespace Lanternfall
{
    public enum CombatEffectCue { Move, Slash, Spear, Projectile, Fire, Prism, Shadow, Gadget, DrainAP, DrainMP, Root, Heal, Shield, Heavy }

    public static class CombatEffectLanguage
    {
        public static CombatEffectCue ForMessage(string message)
        {
            string m=(message??"").ToLowerInvariant();
            if(m.Contains("heal"))return CombatEffectCue.Heal;
            if(m.Contains("shield")||m.Contains("guard stance"))return CombatEffectCue.Shield;
            if(m.Contains("root")||m.Contains("lens trap")||m.Contains("bind"))return CombatEffectCue.Root;
            if(m.Contains("ap drain")||m.Contains("drains ap"))return CombatEffectCue.DrainAP;
            if(m.Contains("mp drain")||m.Contains("drains mp"))return CombatEffectCue.DrainMP;
            if(m.Contains("ember")||m.Contains("cinder")||m.Contains("blast")||m.Contains("burn"))return CombatEffectCue.Fire;
            if(m.Contains("prism"))return CombatEffectCue.Prism;
            if(m.Contains("shadow")||m.Contains("diagonal dash"))return CombatEffectCue.Shadow;
            if(m.Contains("spear")||m.Contains("sun charge"))return CombatEffectCue.Spear;
            if(m.Contains("backstab")||m.Contains("slash"))return CombatEffectCue.Slash;
            if(m.Contains("gadget")||m.Contains("trap"))return CombatEffectCue.Gadget;
            if(m.Contains("heavy")||m.Contains("warden")||m.Contains("boss"))return CombatEffectCue.Heavy;
            return CombatEffectCue.Projectile;
        }

        public static Color Color(CombatEffectCue cue)=>cue switch
        {
            CombatEffectCue.Heal=>new Color(.30f,1f,.48f), CombatEffectCue.Shield=>new Color(.32f,.78f,1f),
            CombatEffectCue.Root=>new Color(.42f,1f,.38f), CombatEffectCue.Fire=>new Color(1f,.34f,.08f),
            CombatEffectCue.Prism=>new Color(.66f,.34f,1f), CombatEffectCue.Shadow=>new Color(.78f,.28f,1f),
            CombatEffectCue.DrainAP=>new Color(1f,.68f,.18f), CombatEffectCue.DrainMP=>new Color(.20f,.86f,1f),
            CombatEffectCue.Heavy=>new Color(1f,.18f,.12f), _=>new Color(1f,.84f,.34f)
        };
    }
}
