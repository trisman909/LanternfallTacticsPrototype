using System.Linq;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public class MobileReadinessTests
    {
        [Test] public void PortraitLayout_FitsPhoneAndKeepsTouchTargetsReadable()
        {
            var layout=MobileLayout.Compute(360,800);
            Assert.True(layout.Portrait);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
            Assert.That(layout.FontSize,Is.GreaterThanOrEqualTo(16));Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(24));
            Assert.That(layout.Board.yMax,Is.EqualTo(layout.Panel.y).Within(.01f));
        }

        [Test] public void LandscapeLayout_FitsShortPhoneWithoutBoardOverlap()
        {
            var layout=MobileLayout.Compute(800,360);
            Assert.False(layout.Portrait);Assert.True(layout.CompactLandscape);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
            Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(24));Assert.That(layout.Panel.x,Is.EqualTo(layout.Board.xMax).Within(.01f));
        }

        [Test] public void IPhonePortrait_DynamicIslandAndHomeIndicatorStayOutsideUI()
        {
            var safe=MobileLayout.ToGuiSafeArea(852,new Rect(0,34,393,759));
            Assert.AreEqual(new Rect(0,59,393,759),safe);
            var layout=MobileLayout.Compute(safe.width,safe.height);Assert.True(layout.Portrait);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
            Assert.That(layout.SkillButtons.All(r=>r.width>=48&&r.height>=48));Assert.That(layout.RewardButtons.All(r=>r.width>=48&&r.height>=48));Assert.That(layout.RestartButton.height,Is.GreaterThanOrEqualTo(48));
        }

        [Test] public void IPhoneLandscape_NotchInsetsPreserveReadableControls()
        {
            var safe=MobileLayout.ToGuiSafeArea(393,new Rect(59,21,734,372));
            Assert.AreEqual(new Rect(59,0,734,372),safe);
            var layout=MobileLayout.Compute(safe.width,safe.height);Assert.False(layout.Portrait);Assert.True(layout.CompactLandscape);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
            Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(24));
        }

        [Test] public void FirstTimeFlow_StartScreenAndHelpPanelAreExplicit()
        {
            var go=new GameObject("FirstTime");var game=go.AddComponent<LanternfallGame>();
            Assert.False(game.HasStarted);Assert.That(game.Message,Does.Contain("Start Run"));
            Assert.That(LanternfallGame.HowToPlayLines.Length,Is.GreaterThanOrEqualTo(5));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("AP")));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("Red")));
            var previous=game.SelectedClass;game.CycleClass();Assert.AreNotEqual(previous,game.SelectedClass);
            game.ShowHelp();Assert.True(game.HelpVisible);
            game.HideHelp();Assert.False(game.HelpVisible);
            game.StartRun();Assert.True(game.HasStarted);Assert.AreEqual(TurnPhase.Player,game.Turns.Phase);
            Object.DestroyImmediate(go);
        }

        [Test] public void TouchFlow_SkillSelectionAndCancellationAreExplicit()
        {
            var go=new GameObject("TouchFlow");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            game.SelectSkill(SkillId.EmberBolt);Assert.AreEqual(SkillId.EmberBolt,game.SelectedSkill);Assert.True(game.LastInputAccepted);
            game.CancelSkill();Assert.IsNull(game.SelectedSkill);Assert.True(game.LastInputAccepted);Assert.That(game.Message,Does.Contain("cancelled"));
            Object.DestroyImmediate(go);
        }

        [Test] public void TurnEconomy_ApAndMpSpendAndResetClearly()
        {
            var go=new GameObject("Economy");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            int startMp=game.Player.MovementPoints;var valid=game.ValidTargets.First();int moveCost=game.Grid.ShortestPath(game.Player.Position,valid,game.Occupied).Count;
            game.TapTile(valid);Assert.AreEqual(startMp-moveCost,game.Player.MovementPoints);Assert.AreEqual(TurnPhase.Player,game.Turns.Phase);
            game.Enemies[0].Position=game.Grid.Neighbors(game.Player.Position).First();game.RefreshPreviews();
            game.SelectSkill(SkillId.EmberBolt);var target=game.ValidTargets.First();game.TapTile(target);Assert.Less(game.Player.ActionPoints,game.Player.MaxActionPoints);
            game.Player.ActionPoints=0;game.SelectSkill(SkillId.CinderBloom);Assert.False(game.LastInputAccepted);Assert.That(game.Message,Does.Contain("AP"));
            game.Player.ResetTurnResources();Assert.AreEqual(game.Player.MaxActionPoints,game.Player.ActionPoints);Assert.AreEqual(game.Player.MoveRange,game.Player.MovementPoints);
            Object.DestroyImmediate(go);
        }

        [Test] public void TurnEconomy_CannotMoveWithoutEnoughMp()
        {
            var go=new GameObject("NoMP");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            game.Player.MovementPoints=0;game.CancelSkill();Assert.AreEqual(0,game.ValidTargets.Count);
            game.TapTile(game.Player.Position+Vector2Int.up);Assert.False(game.LastInputAccepted);
            Object.DestroyImmediate(go);
        }

        [Test] public void TouchFlow_InvalidAndValidTileTapsGiveClearResults()
        {
            var go=new GameObject("TileTap");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            var start=game.Player.Position;game.TapTile(new Vector2Int(-1,-1));Assert.False(game.LastInputAccepted);Assert.AreEqual(start,game.Player.Position);Assert.That(game.Message,Does.StartWith("INVALID"));Assert.True(game.RejectedTile.HasValue);
            var valid=game.ValidTargets.First();game.TapTile(valid);Assert.True(game.LastInputAccepted);Assert.AreEqual(valid,game.Player.Position);
            Object.DestroyImmediate(go);
        }

        [Test] public void TouchFlow_RewardAdvancesRoomAndRestartRestoresRun()
        {
            var go=new GameObject("RewardTap");var game=go.AddComponent<LanternfallGame>();game.StartRun();game.Turns.ShowReward();
            game.ChooseReward(1);Assert.AreEqual(2,game.RoomNumber);Assert.AreEqual(1,game.Player.Power);
            game.Player.Damage(999);game.Turns.Lose();game.Restart();Assert.AreEqual(1,game.RoomNumber);Assert.True(game.Player.Alive);Assert.AreEqual(TurnPhase.Player,game.Turns.Phase);
            Object.DestroyImmediate(go);
        }

        [Test] public void TacticalFeedback_HitsInvalidTilesAndBossRoomAreReadable()
        {
            var go=new GameObject("Feedback");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            game.Enemies[0].Position=game.Grid.Neighbors(game.Player.Position).First();game.RefreshPreviews();
            game.SelectSkill(SkillId.EmberBolt);
            var enemy=game.Enemies.First(e=>e.Alive&&game.ValidTargets.Contains(e.Position));
            game.TapTile(enemy.Position);
            Assert.True(game.HitTiles.Contains(enemy.Position));
            Assert.That(game.Message,Does.Contain("Ember Bolt"));
            for(int i=0;i<4;i++){game.Turns.ShowReward();game.ChooseReward(0);}
            Assert.AreEqual(5,game.RoomNumber);Assert.That(game.Message,Does.Contain("BOSS ROOM"));
            Object.DestroyImmediate(go);
        }

        [Test] public void TacticalMechanics_SwapAndRootWorkWhereImplemented()
        {
            var go=new GameObject("Mechanics");var game=go.AddComponent<LanternfallGame>();game.SelectClass(PlayerClassId.Gloamstep);game.StartRun();
            game.Enemies[0].Position=game.Grid.Neighbors(game.Player.Position).First();game.RefreshPreviews();
            game.SelectSkill(SkillId.ShadowSwap);var enemy=game.Enemies.FirstOrDefault(e=>e.Alive&&game.ValidTargets.Contains(e.Position));
            if(enemy!=null){var playerPos=game.Player.Position;var enemyPos=enemy.Position;game.TapTile(enemy.Position);Assert.AreEqual(enemyPos,game.Player.Position);Assert.AreEqual(playerPos,enemy.Position);}
            Object.DestroyImmediate(go);

            go=new GameObject("Root");game=go.AddComponent<LanternfallGame>();game.SelectClass(PlayerClassId.Artificer);game.StartRun();
            game.Enemies[0].Position=game.Grid.Neighbors(game.Player.Position).First();game.RefreshPreviews();
            game.SelectSkill(SkillId.LensTrap);var rooted=game.Enemies.First(e=>e.Alive&&game.ValidTargets.Contains(e.Position));game.TapTile(rooted.Position);Assert.Greater(rooted.RootTurns,0);
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5D_EachClassCanCompleteBasicCombatFlow()
        {
            foreach(var cls in ClassCatalog.All)
            {
                var go=new GameObject("ClassFlow");var game=go.AddComponent<LanternfallGame>();game.SelectClass(cls.id);game.StartRun(7000+(int)cls.id);
                game.Enemies[0].Position=game.Grid.Neighbors(game.Player.Position).First();game.RefreshPreviews();
                var usable=SkillBook.ForClass(cls.id).First(s=>s.Effect!=SkillEffect.DashDamage&&s.Effect!=SkillEffect.DiagonalMove&&s.Effect!=SkillEffect.SelfShield);
                int startAp=game.Player.ActionPoints;
                game.SelectSkill(usable.Id);Assert.True(game.LastInputAccepted,cls.name);
                var target=game.ValidTargets.FirstOrDefault(t=>game.Enemies.Any(e=>e.Alive&&e.Position==t));
                Assert.AreNotEqual(default(Vector2Int),target,cls.name);
                game.TapTile(target);
                Assert.Less(game.Player.ActionPoints,startAp,cls.name);
                Assert.That(game.HitTiles.Count,Is.GreaterThan(0),cls.name);
                Object.DestroyImmediate(go);
            }
        }

        [Test] public void Phase5D_RewardsSupportDifferentClassNeeds()
        {
            var go=new GameObject("Rewards");var game=go.AddComponent<LanternfallGame>();game.StartRun(8010);game.Turns.ShowReward();
            int hp=game.Player.MaxHealth;game.ChooseReward(0);Assert.AreEqual(hp+3,game.Player.MaxHealth);
            game.Turns.ShowReward();int power=game.Player.Power;game.ChooseReward(1);Assert.AreEqual(power+1,game.Player.Power);
            game.Turns.ShowReward();int mp=game.Player.MoveRange;game.ChooseReward(2);Assert.AreEqual(mp+1,game.Player.MoveRange);
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5D_SeededRunsReachBossBiomeDeterministically()
        {
            foreach(int seed in new[]{1101,2202,3303})
            {
                var go=new GameObject("Seeded");var game=go.AddComponent<LanternfallGame>();game.StartRun(seed);
                for(int i=0;i<4;i++){game.Turns.ShowReward();game.ChooseReward(i%3);}
                Assert.AreEqual(5,game.RoomNumber,seed);
                Assert.That(game.Theme.Id,Is.EqualTo(BiomeId.StormvaultFoundry),seed.ToString());
                Assert.That(game.Enemies[0].Kind,Is.EqualTo(EnemyKind.LanternWarden),seed.ToString());
                Object.DestroyImmediate(go);
            }
        }

        [Test] public void TacticalWarnings_EnemyPreviewsAndEveryHazardRemainVisible()
        {
            var go=new GameObject("Warnings");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            Assert.True(game.Enemies.Any(e=>e.Preview.Count>0));
            Assert.True(BiomeCatalog.All.All(b=>b.TileContrast>.06f&&!string.IsNullOrWhiteSpace(b.HazardName)));
            Object.DestroyImmediate(go);
        }

        [Test] public void BalancePass_KeepsRunBeatableButNotFree()
        {
            Assert.AreEqual(3,BalanceConfig.BetweenRoomRecovery);
            Assert.AreEqual(15,BalanceConfig.EnemyStats(EnemyKind.LanternWarden).health);
            Assert.AreEqual(1,SkillBook.Get(SkillId.EmberBolt).Cooldown);
            Assert.That(BalanceConfig.EnemyStats(EnemyKind.StoneSentinel).damage,Is.GreaterThanOrEqualTo(3));
        }

        [Test] public void Phase5D_WebGLAndWindowsPreviewFilesRemainPrepared()
        {
            Assert.True(File.Exists("docs/index.html"));
            Assert.True(File.Exists("docs/Build/LanternfallTactics.loader.js"));
            Assert.True(File.Exists("docs/Build/LanternfallTactics.wasm"));
            Assert.True(Directory.Exists("ProjectSettings"));
        }

        [Test] public void Phase5E_WebGLTemplateUsesResponsiveViewportCanvas()
        {
            var index=File.ReadAllText("docs/index.html");
            var css=File.ReadAllText("docs/TemplateData/style.css");
            Assert.That(index,Does.Contain("canvas.style.width = \"100vw\""));
            Assert.That(index,Does.Contain("canvas.style.height = \"100vh\""));
            Assert.That(css,Does.Contain("#unity-footer { display: none; }"));
            Assert.That(css,Does.Contain("width: 100vw"));
            Assert.That(css,Does.Contain("height: 100vh"));
        }
    }
}
