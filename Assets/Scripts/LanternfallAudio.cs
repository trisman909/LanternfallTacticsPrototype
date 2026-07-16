using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
#if !UNITY_WEBGL || UNITY_EDITOR
using UnityEngine.Audio;
#endif

namespace Lanternfall
{
    public enum AudioCue { UiTap,SelectSkill,InvalidTarget,Move,AttackWindup,MeleeHit,RangedHit,SpellCast,Shield,Burn,Root,ActionDrain,MovementDrain,Heal,EnemyPhase,BossPhase,RoomClear,Reward,EndTurn,Victory,Defeat }

    public interface IAudioService
    {
        bool IsUnlocked { get; }
        void Unlock(bool bossRoom);
        void PlayUiSound(AudioCue cue);
        void PlayMovement(AudioCue cue);
        void PlayAttack(AudioCue cue);
        void PlayStatus(AudioCue cue);
        void PlayBossPhase(AudioCue cue);
        void PlayMusic(bool bossRoom);
        void SetMasterVolume(float value);
        void SetSfxVolume(float value);
        void SetMusicVolume(float value);
        void SetMuted(bool value);
        void Refresh(bool bossRoom);
    }

    public static class LanternfallAudioSettings
    {
        const string MasterKey="Lanternfall.Audio.Master",SfxKey="Lanternfall.Audio.Sfx",MusicKey="Lanternfall.Audio.Music",MuteKey="Lanternfall.Audio.Mute";
        public const float DefaultMaster=.8f,DefaultSfx=.8f,DefaultMusic=.45f;
        public static float Master { get=>Read(MasterKey,DefaultMaster); set=>Save(MasterKey,value,DefaultMaster); }
        public static float Sfx { get=>Read(SfxKey,DefaultSfx); set=>Save(SfxKey,value,DefaultSfx); }
        public static float Music { get=>Read(MusicKey,DefaultMusic); set=>Save(MusicKey,value,DefaultMusic); }
        public static bool Muted { get=>PlayerPrefs.GetInt(MuteKey,0)==1; set {PlayerPrefs.SetInt(MuteKey,value?1:0);PlayerPrefs.Save();} }
        public static float SanitizeVolume(float value,float fallback)=>float.IsNaN(value)||float.IsInfinity(value)?fallback:Mathf.Clamp01(value);
        static float Read(string key,float fallback)=>SanitizeVolume(PlayerPrefs.GetFloat(key,fallback),fallback);
        static void Save(string key,float value,float fallback){PlayerPrefs.SetFloat(key,SanitizeVolume(value,fallback));PlayerPrefs.Save();}
        public static float Output(float channel)=>Muted?0f:Master*channel;
    }

    public sealed class LanternfallAudio : MonoBehaviour
    {
        IAudioService service;
        string lastMessage="";
        bool bossMusic;
        public bool Unlocked=>Service.IsUnlocked;
        public static bool RequiresInteractionBeforePlayback=>true;

        IAudioService Service
        {
            get
            {
                if(service==null)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    service=new WebAudioService();
#else
                    service=new UnityAudioService(gameObject);
#endif
                }
                return service;
            }
        }

        public void Unlock(bool bossRoom=false)=>Service.Unlock(bossRoom);
        public void SetMusic(bool bossRoom){bossMusic=bossRoom;Service.PlayMusic(bossRoom);}
        public void Play(AudioCue cue)
        {
            if(!Unlocked||LanternfallAudioSettings.Muted)return;
            if(cue==AudioCue.UiTap||cue==AudioCue.SelectSkill||cue==AudioCue.InvalidTarget||cue==AudioCue.Reward||cue==AudioCue.EndTurn)Service.PlayUiSound(cue);
            else if(cue==AudioCue.Move)Service.PlayMovement(cue);
            else if(cue==AudioCue.AttackWindup||cue==AudioCue.MeleeHit||cue==AudioCue.RangedHit||cue==AudioCue.SpellCast)Service.PlayAttack(cue);
            else if(cue==AudioCue.BossPhase)Service.PlayBossPhase(cue);
            else Service.PlayStatus(cue);
        }
        public void PlayCombat(CombatEffectCue cue)=>Play(cue switch {CombatEffectCue.Move=>AudioCue.Move,CombatEffectCue.Slash or CombatEffectCue.Spear=>AudioCue.MeleeHit,CombatEffectCue.Projectile=>AudioCue.RangedHit,CombatEffectCue.Shield=>AudioCue.Shield,CombatEffectCue.Fire=>AudioCue.Burn,CombatEffectCue.Root=>AudioCue.Root,CombatEffectCue.DrainAP=>AudioCue.ActionDrain,CombatEffectCue.DrainMP=>AudioCue.MovementDrain,CombatEffectCue.Heal=>AudioCue.Heal,_=>AudioCue.SpellCast});
        public void Observe(string message,bool bossRoom)
        {
            SetMusic(bossRoom);if(string.IsNullOrWhiteSpace(message)||message==lastMessage)return;lastMessage=message;string m=message.ToLowerInvariant();
            if(m.Contains("invalid"))Play(AudioCue.InvalidTarget);else if(m.Contains("moved"))Play(AudioCue.Move);else if(m.Contains("shield"))Play(AudioCue.Shield);else if(m.Contains("burn"))Play(AudioCue.Burn);else if(m.Contains("root"))Play(AudioCue.Root);else if(m.Contains("heal"))Play(AudioCue.Heal);else if(m.Contains("ap drain"))Play(AudioCue.ActionDrain);else if(m.Contains("mp bind"))Play(AudioCue.MovementDrain);else if(m.Contains("phase two"))Play(AudioCue.BossPhase);else if(m.Contains("room clear"))Play(AudioCue.RoomClear);else if(m.Contains("victory"))Play(AudioCue.Victory);else if(m.Contains("defeat"))Play(AudioCue.Defeat);else if(m.Contains("selected")||m.Contains("choose a gold"))Play(AudioCue.SelectSkill);
        }
        public void SetMasterVolume(float value){LanternfallAudioSettings.Master=value;Service.SetMasterVolume(value);}
        public void SetSfxVolume(float value){LanternfallAudioSettings.Sfx=value;Service.SetSfxVolume(value);}
        public void SetMusicVolume(float value){LanternfallAudioSettings.Music=value;Service.SetMusicVolume(value);}
        public void SetMuted(bool value){LanternfallAudioSettings.Muted=value;Service.SetMuted(value);}
        public void RefreshVolumes()=>Service.Refresh(bossMusic);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    sealed class WebAudioService : IAudioService
    {
        [DllImport("__Internal")] static extern void LanternfallAudio_Unlock(float master,float sfx,float music,int muted,int boss);
        [DllImport("__Internal")] static extern void LanternfallAudio_Play(int cue,float master,float sfx,int muted);
        [DllImport("__Internal")] static extern void LanternfallAudio_Set(float master,float sfx,float music,int muted,int boss);
        public bool IsUnlocked { get; private set; }
        bool boss;
        public void Unlock(bool bossRoom){if(IsUnlocked)return;IsUnlocked=true;boss=bossRoom;LanternfallAudio_Unlock(LanternfallAudioSettings.Master,LanternfallAudioSettings.Sfx,LanternfallAudioSettings.Music,LanternfallAudioSettings.Muted?1:0,boss?1:0);}
        void Play(AudioCue cue){if(IsUnlocked)LanternfallAudio_Play((int)cue,LanternfallAudioSettings.Master,LanternfallAudioSettings.Sfx,LanternfallAudioSettings.Muted?1:0);}
        public void PlayUiSound(AudioCue cue)=>Play(cue);
        public void PlayMovement(AudioCue cue)=>Play(cue);
        public void PlayAttack(AudioCue cue)=>Play(cue);
        public void PlayStatus(AudioCue cue)=>Play(cue);
        public void PlayBossPhase(AudioCue cue)=>Play(cue);
        public void PlayMusic(bool bossRoom){boss=bossRoom;Refresh(boss);}
        public void SetMasterVolume(float value)=>Refresh(boss);
        public void SetSfxVolume(float value)=>Refresh(boss);
        public void SetMusicVolume(float value)=>Refresh(boss);
        public void SetMuted(bool value)=>Refresh(boss);
        public void Refresh(bool bossRoom){boss=bossRoom;if(IsUnlocked)LanternfallAudio_Set(LanternfallAudioSettings.Master,LanternfallAudioSettings.Sfx,LanternfallAudioSettings.Music,LanternfallAudioSettings.Muted?1:0,boss?1:0);}
    }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
    // Native backend scaffold. Final clips and mixer groups can be assigned later without changing callers.
    sealed class UnityAudioService : IAudioService
    {
        readonly AudioSource sfx,music;
        AudioMixer mixer;
        bool boss;
        public bool IsUnlocked { get; private set; }
        public UnityAudioService(GameObject owner)
        {
            sfx=owner.AddComponent<AudioSource>();music=owner.AddComponent<AudioSource>();music.loop=true;
            sfx.playOnAwake=false;music.playOnAwake=false;ApplyVolumes();
        }
        public void Unlock(bool bossRoom){IsUnlocked=true;boss=bossRoom;ApplyVolumes();}
        void Play(AudioCue cue){/* Cue-to-clip library is intentionally empty until native asset production. */}
        public void PlayUiSound(AudioCue cue)=>Play(cue);
        public void PlayMovement(AudioCue cue)=>Play(cue);
        public void PlayAttack(AudioCue cue)=>Play(cue);
        public void PlayStatus(AudioCue cue)=>Play(cue);
        public void PlayBossPhase(AudioCue cue)=>Play(cue);
        public void PlayMusic(bool bossRoom){boss=bossRoom;ApplyVolumes();}
        public void SetMasterVolume(float value)=>ApplyVolumes();
        public void SetSfxVolume(float value)=>ApplyVolumes();
        public void SetMusicVolume(float value)=>ApplyVolumes();
        public void SetMuted(bool value)=>ApplyVolumes();
        public void Refresh(bool bossRoom){boss=bossRoom;ApplyVolumes();}
        void ApplyVolumes(){sfx.volume=LanternfallAudioSettings.Output(LanternfallAudioSettings.Sfx);music.volume=LanternfallAudioSettings.Output(LanternfallAudioSettings.Music);if(mixer!=null)mixer.SetFloat("MasterVolume",LanternfallAudioSettings.Muted?-80f:Mathf.Log10(Mathf.Max(.0001f,LanternfallAudioSettings.Master))*20f);}
    }
#endif
}
