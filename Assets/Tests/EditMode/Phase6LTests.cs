using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6LTests
    {
        static GridModel FullGrid(int width,int height){var g=new GridModel(width,height);for(int y=0;y<height;y++)for(int x=0;x<width;x++)g.SetFloor(new Vector2Int(x,y));return g;}

        [Test] public void FutureThreatBorderIsPhoneReadableButQuieterThanCurrentDanger()
        {
            Assert.That(VisualReadability.FutureThreatBorderScale(true),Is.GreaterThan(VisualReadability.FutureThreatBorderScale(false)));
            Assert.That(VisualReadability.FutureThreatBorderScale(true),Is.LessThan(.06f));
            Assert.That(VisualReadability.TacticalOverlayAlpha(TileVisualState.EnemyPreview),Is.GreaterThan(.42f));
        }

        [Test] public void TerrainPropsStayEnvironmentalWithoutTacticalOuterFrames()
        {
            Assert.False(VisualReadability.PropsReceiveTacticalOuterBorders);
            foreach(var theme in BiomeCatalog.All)Assert.That(VisualReadability.EnvironmentalPropTint(theme).maxColorComponent,Is.LessThan(.7f),theme.Name);
        }

        [Test] public void SentinelAdvancesForFourTurnsWhenItHasNoControlReasonToHold()
        {
            var g=FullGrid(11,11);var player=new Vector2Int(9,9);var sentinel=new EnemyModel(EnemyKind.StoneSentinel,new Vector2Int(1,1)){MoveRange=1};
            int idle=0;
            for(int turn=0;turn<4;turn++)
            {
                EnemyAI.AssignIntent(sentinel,player,g);var before=sentinel.Position;
                Assert.False(EnemyAI.ShouldHoldPosition(sentinel,player,g,new[]{sentinel}));
                var next=EnemyAI.ChooseReposition(sentinel,player,g,_=>false,null,new[]{sentinel});
                if(next==before)idle++; sentinel.PreviousPosition=before;sentinel.Position=next;
            }
            Assert.Zero(idle);Assert.Less(Mathf.Abs(sentinel.Position.x-player.x)+Mathf.Abs(sentinel.Position.y-player.y),16);
        }

        [Test] public void InvalidSkillTargetUsesAConciseReasonAndPreservesSelection()
        {
            var go=new GameObject("Invalid6L");var game=go.AddComponent<LanternfallGame>();game.StartRun(6100);
            var skill=SkillBook.ForClass(game.Player.ClassId)[0];game.SelectSkill(skill.Id);game.TapTile(new Vector2Int(-1,-1));
            Assert.True(game.SelectedSkill.HasValue);Assert.That(game.Message,Does.StartWith("INVALID: Invalid destination"));Assert.False(game.Message.Contains("Cyan ="));
            Object.DestroyImmediate(go);
        }

        [Test] public void PhoneStatusBadgesAreApproximatelyTwiceTheLegacyFootprint()
        {
            Assert.That(VisualReadability.StatusIconScale(true)/.19f,Is.InRange(1.8f,2.2f));
            Assert.That(VisualReadability.StatusIconScale(false),Is.LessThanOrEqualTo(VisualReadability.StatusIconScale(true)));
        }

        [Test] public void ThreatBoundaryStopsAtWallsAndDisconnectedGaps()
        {
            var g=new GridModel(5,3);g.SetFloor(new Vector2Int(0,1));g.SetFloor(new Vector2Int(1,1));g.SetFloor(new Vector2Int(3,1));g.SetFloor(new Vector2Int(4,1));
            var affected=new HashSet<Vector2Int>{new(0,1),new(1,1),new(3,1),new(4,1)};
            Assert.True(VisualReadability.IsExposedThreatEdge(affected,new Vector2Int(1,1),Vector2Int.right,g));
            Assert.True(VisualReadability.IsExposedThreatEdge(affected,new Vector2Int(3,1),Vector2Int.left,g));
            Assert.False(VisualReadability.IsExposedThreatEdge(affected,new Vector2Int(0,1),Vector2Int.right,g));
        }

        [Test] public void AudioSettingsPersistAndPlaybackRequiresInteraction()
        {
            float oldMaster=LanternfallAudioSettings.Master,oldSfx=LanternfallAudioSettings.Sfx,oldMusic=LanternfallAudioSettings.Music;bool oldMute=LanternfallAudioSettings.Muted;
            LanternfallAudioSettings.Master=.5f;LanternfallAudioSettings.Sfx=0f;LanternfallAudioSettings.Music=1f;LanternfallAudioSettings.Muted=true;
            Assert.AreEqual(.5f,LanternfallAudioSettings.Master,.001f);Assert.AreEqual(0f,LanternfallAudioSettings.Sfx,.001f);Assert.AreEqual(1f,LanternfallAudioSettings.Music,.001f);Assert.True(LanternfallAudioSettings.Muted);Assert.True(LanternfallAudio.RequiresInteractionBeforePlayback);
            LanternfallAudioSettings.Master=oldMaster;LanternfallAudioSettings.Sfx=oldSfx;LanternfallAudioSettings.Music=oldMusic;LanternfallAudioSettings.Muted=oldMute;
        }

        [Test] public void AudioServiceContractRemainsPlatformNeutral()
        {
            var contract=typeof(IAudioService);
            foreach(var name in new[]{"PlayUiSound","PlayMovement","PlayAttack","PlayStatus","PlayBossPhase","PlayMusic","SetMasterVolume","SetSfxVolume","SetMusicVolume","SetMuted"})
                Assert.NotNull(contract.GetMethod(name),name+" must remain available to platform backends");
        }

        [Test] public void MissingAudioSettingsUseExistingDefaults()
        {
            WithRestoredAudioSettings(()=>
            {
                DeleteAudioKeys();
                Assert.AreEqual(LanternfallAudioSettings.DefaultMaster,LanternfallAudioSettings.Master,.0001f);
                Assert.AreEqual(LanternfallAudioSettings.DefaultSfx,LanternfallAudioSettings.Sfx,.0001f);
                Assert.AreEqual(LanternfallAudioSettings.DefaultMusic,LanternfallAudioSettings.Music,.0001f);
            });
        }

        [TestCase("")]
        [TestCase("not-a-number")]
        public void EmptyOrMalformedSavedVolumesUseExistingDefaults(string stored)
        {
            WithRestoredAudioSettings(()=>
            {
                PlayerPrefs.SetString("Lanternfall.Audio.Master",stored);PlayerPrefs.SetString("Lanternfall.Audio.Sfx",stored);PlayerPrefs.SetString("Lanternfall.Audio.Music",stored);
                Assert.AreEqual(LanternfallAudioSettings.DefaultMaster,LanternfallAudioSettings.Master,.0001f);
                Assert.AreEqual(LanternfallAudioSettings.DefaultSfx,LanternfallAudioSettings.Sfx,.0001f);
                Assert.AreEqual(LanternfallAudioSettings.DefaultMusic,LanternfallAudioSettings.Music,.0001f);
            });
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void NonFiniteVolumesUseChannelDefaults(float value)
        {
            Assert.AreEqual(LanternfallAudioSettings.DefaultMaster,LanternfallAudioSettings.SanitizeVolume(value,LanternfallAudioSettings.DefaultMaster));
            Assert.AreEqual(LanternfallAudioSettings.DefaultSfx,LanternfallAudioSettings.SanitizeVolume(value,LanternfallAudioSettings.DefaultSfx));
            Assert.AreEqual(LanternfallAudioSettings.DefaultMusic,LanternfallAudioSettings.SanitizeVolume(value,LanternfallAudioSettings.DefaultMusic));
        }

        [TestCase(-2f,0f)]
        [TestCase(3f,1f)]
        public void OutOfRangeVolumesAreClamped(float value,float expected)
        {
            Assert.AreEqual(expected,LanternfallAudioSettings.SanitizeVolume(value,LanternfallAudioSettings.DefaultMaster));
        }

        [Test] public void MutedStartupAndValidSettingsRemainUnchanged()
        {
            WithRestoredAudioSettings(()=>
            {
                LanternfallAudioSettings.Master=.63f;LanternfallAudioSettings.Sfx=.27f;LanternfallAudioSettings.Music=.91f;LanternfallAudioSettings.Muted=true;
                Assert.AreEqual(.63f,LanternfallAudioSettings.Master,.0001f);Assert.AreEqual(.27f,LanternfallAudioSettings.Sfx,.0001f);Assert.AreEqual(.91f,LanternfallAudioSettings.Music,.0001f);Assert.True(LanternfallAudioSettings.Muted);
            });
        }

        [Test] public void UnlockVolumesAreAlwaysFiniteEvenWhenPrefsContainNonFiniteValues()
        {
            WithRestoredAudioSettings(()=>
            {
                PlayerPrefs.SetFloat("Lanternfall.Audio.Master",float.NaN);PlayerPrefs.SetFloat("Lanternfall.Audio.Sfx",float.PositiveInfinity);PlayerPrefs.SetFloat("Lanternfall.Audio.Music",float.NegativeInfinity);
                foreach(float value in new[]{LanternfallAudioSettings.Master,LanternfallAudioSettings.Sfx,LanternfallAudioSettings.Music})Assert.False(float.IsNaN(value)||float.IsInfinity(value));
            });
        }

        [Test] public void WebAudioBoundaryInitializesStepAndGuardsEveryAudioParam()
        {
            string source=File.ReadAllText(Path.Combine(Application.dataPath,"Plugins/WebGL/LanternfallAudio.jslib"));
            Assert.That(source,Does.Contain("Number.isFinite"));Assert.That(source,Does.Contain("L.step=Math.max(0,Math.floor(L.finite(L.step,0,'music step')))"));
            Assert.That(source,Does.Contain("o.frequency.value=n"));Assert.That(source,Does.Contain("o.frequency.value=frequency"));
            Assert.That(source,Does.Contain("L.finite(L.ctx.currentTime,0,'audio time')"));Assert.That(source,Does.Contain("L.volume(L.master*L.music*.035,0,'music gain')"));Assert.That(source,Does.Contain("L.volume(master*sfx*.11,0,'SFX gain')"));
            Assert.That(source,Does.Contain("Invalid '+label+'; using safe default"));
        }

        static void DeleteAudioKeys(){foreach(var key in new[]{"Lanternfall.Audio.Master","Lanternfall.Audio.Sfx","Lanternfall.Audio.Music","Lanternfall.Audio.Mute"})PlayerPrefs.DeleteKey(key);}
        static void WithRestoredAudioSettings(System.Action test)
        {
            float master=LanternfallAudioSettings.Master,sfx=LanternfallAudioSettings.Sfx,music=LanternfallAudioSettings.Music;bool muted=LanternfallAudioSettings.Muted;
            try{test();}finally{LanternfallAudioSettings.Master=master;LanternfallAudioSettings.Sfx=sfx;LanternfallAudioSettings.Music=music;LanternfallAudioSettings.Muted=muted;}
        }

        [Test] public void BossThreatTilesRemainWalkableAndBounded()
        {
            var g=FullGrid(11,11);var boss=new EnemyModel(EnemyKind.LanternWarden,new Vector2Int(5,5));BalanceConfig.ApplyRoomScaling(boss,5);
            var threat=EnemyAI.BuildPreview(boss,Vector2Int.zero,g);Assert.True(threat.Count>0);Assert.True(threat.Count<g.Floors().Count());Assert.True(threat.All(g.IsFloor));
        }
    }
}
