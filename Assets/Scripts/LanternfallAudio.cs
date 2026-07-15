using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lanternfall
{
    public enum AudioCue { UiTap,SelectSkill,InvalidTarget,Move,AttackWindup,MeleeHit,RangedHit,SpellCast,Shield,Burn,Root,ActionDrain,MovementDrain,Heal,EnemyPhase,BossPhase,RoomClear,Reward,EndTurn,Victory,Defeat }

    public static class LanternfallAudioSettings
    {
        const string MasterKey="Lanternfall.Audio.Master",SfxKey="Lanternfall.Audio.Sfx",MusicKey="Lanternfall.Audio.Music",MuteKey="Lanternfall.Audio.Mute";
        public static float Master { get=>PlayerPrefs.GetFloat(MasterKey,.8f); set=>Save(MasterKey,value); }
        public static float Sfx { get=>PlayerPrefs.GetFloat(SfxKey,.8f); set=>Save(SfxKey,value); }
        public static float Music { get=>PlayerPrefs.GetFloat(MusicKey,.45f); set=>Save(MusicKey,value); }
        public static bool Muted { get=>PlayerPrefs.GetInt(MuteKey,0)==1; set {PlayerPrefs.SetInt(MuteKey,value?1:0);PlayerPrefs.Save();} }
        static void Save(string key,float value){PlayerPrefs.SetFloat(key,Mathf.Clamp01(value));PlayerPrefs.Save();}
        public static float Output(float channel)=>Muted?0f:Master*channel;
    }

    public sealed class LanternfallAudio : MonoBehaviour
    {
        public bool Unlocked { get; private set; }
        public static bool RequiresInteractionBeforePlayback=>true;
        AudioSource sfx,music;
        readonly Dictionary<AudioCue,AudioClip> clips=new();
        AudioClip explore,boss;
        string lastMessage="";

        void Awake()
        {
            sfx=gameObject.AddComponent<AudioSource>(); sfx.playOnAwake=false;
            music=gameObject.AddComponent<AudioSource>(); music.playOnAwake=false; music.loop=true;
            foreach(AudioCue cue in Enum.GetValues(typeof(AudioCue)))clips[cue]=Tone(cue.ToString(),Pitch(cue),.09f+(int)cue%3*.035f,(int)cue%4);
            explore=Loop("Lanternfall Exploration",new[]{146.83f,174.61f,220f,196f});
            boss=Loop("Lantern Warden",new[]{110f,138.59f,164.81f,123.47f});
        }

        public void Unlock(bool bossRoom=false)
        {
            if(Unlocked)return; Unlocked=true; SetMusic(bossRoom);
        }

        public void SetMusic(bool bossRoom)
        {
            if(!Unlocked)return;
            var wanted=bossRoom?boss:explore;
            music.volume=LanternfallAudioSettings.Output(LanternfallAudioSettings.Music);
            if(music.clip!=wanted){music.clip=wanted;music.Play();}
        }

        public void Play(AudioCue cue)
        {
            if(!Unlocked||LanternfallAudioSettings.Muted)return;
            sfx.PlayOneShot(clips[cue],LanternfallAudioSettings.Output(LanternfallAudioSettings.Sfx)*.42f);
        }

        public void PlayCombat(CombatEffectCue cue)
        {
            Play(cue switch {CombatEffectCue.Move=>AudioCue.Move,CombatEffectCue.Slash or CombatEffectCue.Spear=>AudioCue.MeleeHit,CombatEffectCue.Projectile=>AudioCue.RangedHit,CombatEffectCue.Shield=>AudioCue.Shield,CombatEffectCue.Fire=>AudioCue.Burn,CombatEffectCue.Root=>AudioCue.Root,CombatEffectCue.DrainAP=>AudioCue.ActionDrain,CombatEffectCue.DrainMP=>AudioCue.MovementDrain,CombatEffectCue.Heal=>AudioCue.Heal,_=>AudioCue.SpellCast});
        }

        public void Observe(string message,bool bossRoom)
        {
            SetMusic(bossRoom); if(string.IsNullOrWhiteSpace(message)||message==lastMessage)return; lastMessage=message;
            string m=message.ToLowerInvariant();
            if(m.Contains("invalid"))Play(AudioCue.InvalidTarget); else if(m.Contains("moved"))Play(AudioCue.Move);
            else if(m.Contains("shield"))Play(AudioCue.Shield); else if(m.Contains("burn"))Play(AudioCue.Burn);
            else if(m.Contains("root"))Play(AudioCue.Root); else if(m.Contains("heal"))Play(AudioCue.Heal);
            else if(m.Contains("ap drain"))Play(AudioCue.ActionDrain); else if(m.Contains("mp bind"))Play(AudioCue.MovementDrain);
            else if(m.Contains("phase two"))Play(AudioCue.BossPhase); else if(m.Contains("room clear"))Play(AudioCue.RoomClear);
            else if(m.Contains("victory"))Play(AudioCue.Victory); else if(m.Contains("defeat"))Play(AudioCue.Defeat);
            else if(m.Contains("selected")||m.Contains("choose a gold"))Play(AudioCue.SelectSkill);
        }

        public void RefreshVolumes(){sfx.volume=1f;music.volume=LanternfallAudioSettings.Output(LanternfallAudioSettings.Music);}

        static float Pitch(AudioCue cue)=>cue switch {AudioCue.InvalidTarget=>130f,AudioCue.Move=>220f,AudioCue.Heal=>440f,AudioCue.Victory=>523.25f,AudioCue.Defeat=>92.5f,AudioCue.BossPhase=>110f,_=>180f+(int)cue*11f};
        static AudioClip Tone(string name,float frequency,float seconds,int shape)
        {
            const int rate=22050; int count=Mathf.CeilToInt(rate*seconds); var data=new float[count];
            for(int i=0;i<count;i++){float t=i/(float)rate,env=Mathf.Pow(1f-i/(float)count,2f);float wave=shape%2==0?Mathf.Sin(t*frequency*Mathf.PI*2f):Mathf.Sign(Mathf.Sin(t*frequency*Mathf.PI*2f))*.35f;data[i]=wave*env*.35f;}
            var clip=AudioClip.Create(name,count,1,rate,false);clip.SetData(data,0);return clip;
        }
        static AudioClip Loop(string name,float[] notes)
        {
            const int rate=22050; const float beat=.55f; int count=Mathf.CeilToInt(rate*beat*notes.Length);var data=new float[count];
            for(int i=0;i<count;i++){float t=i/(float)rate;int n=Mathf.Min(notes.Length-1,(int)(t/beat));float local=t-n*beat,env=.25f+.75f*Mathf.Sin(Mathf.Clamp01(local/beat)*Mathf.PI);data[i]=(Mathf.Sin(t*notes[n]*Mathf.PI*2f)+Mathf.Sin(t*notes[n]*.5f*Mathf.PI*2f)*.35f)*env*.055f;}
            var clip=AudioClip.Create(name,count,1,rate,false);clip.SetData(data,0);return clip;
        }
    }
}
