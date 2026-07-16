using System.Linq;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public class MobileReadinessTests
    {
        [Test] public void PortraitLayout_PhoneShowsRotateInstructionInsteadOfTinyGameUi()
        {
            var layout=MobileLayout.Compute(360,800);
            Assert.True(layout.Portrait);
            Assert.True(layout.PhoneHud);
            var viewSource=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(viewSource,Does.Contain("Rotate your phone to play"));
            Assert.That(viewSource,Does.Contain("Lanternfall Tactics is best played in landscape"));
        }

        [Test] public void LandscapeLayout_FitsShortPhoneWithoutBoardOverlap()
        {
            var layout=MobileLayout.Compute(800,360);
            Assert.False(layout.Portrait);Assert.True(layout.CompactLandscape);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
            Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(12));
            Assert.True(layout.PhoneLandscape);
            Assert.That(layout.Panel.y,Is.EqualTo(layout.Board.yMax).Within(.01f));
        }

        [Test] public void Phase5G_DefaultBrowserViewportUsesCompactReadableHud()
        {
            var layout=MobileLayout.Compute(1280,720);
            Assert.False(layout.Portrait);Assert.True(layout.CompactLandscape);Assert.False(layout.HasOverlap);Assert.True(layout.TouchTargetsValid);
        }

        [Test] public void Phase5K2_DesktopViewportGivesBoardMorePriorityWithoutBreakingHud()
        {
            var layout=MobileLayout.Compute(1280,720);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            Assert.That(layout.Board.width,Is.GreaterThanOrEqualTo(940f));
            Assert.That(layout.Panel.width,Is.LessThanOrEqualTo(340f));
            Assert.False(layout.HasOverlap);
            Assert.True(hud.RequiredElementsFit(layout.Panel));
            Assert.False(hud.HasEssentialOverlap());
            Assert.True(hud.TouchTargetsValid());
            Assert.That(hud.SkillCards.All(r=>r.height>=86f));
        }

        [Test] public void Phase5K2_BoardFitUsesPlayableFootprintInsteadOfFullGrid()
        {
            var layout=MobileLayout.Compute(1280,720);
            var fullGrid=BoardFitLayout.Compute(layout.Board,9,11,layout.CompactLandscape);
            var playableFootprint=BoardFitLayout.Compute(layout.Board,7,10,layout.CompactLandscape);
            Assert.True(fullGrid.Fits(layout.Board));
            Assert.True(playableFootprint.Fits(layout.Board));
            Assert.That(playableFootprint.TileSize,Is.GreaterThan(fullGrid.TileSize*1.08f));
        }

        [Test] public void Phase5K2_GeneratedRoomsFitBoardAreaWithoutHudOverlap()
        {
            var layout=MobileLayout.Compute(1280,720);
            var gen=new RoomGenerator();
            for(int room=1;room<=5;room++)
            {
                var generated=gen.Generate(9100+room,room);
                var floors=generated.Grid.Floors().ToList();
                int cols=floors.Max(p=>p.x)-floors.Min(p=>p.x)+1;
                int rows=floors.Max(p=>p.y)-floors.Min(p=>p.y)+1;
                var fit=BoardFitLayout.Compute(layout.Board,cols,rows,layout.CompactLandscape);
                Assert.True(fit.Fits(layout.Board),$"room {room}");
                Assert.That(fit.TileSize,Is.GreaterThanOrEqualTo(layout.EstimatedTileSize),$"room {room}");
            }
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
            Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(13));
        }

        [Test] public void Phase5O1_PhonePortraitIsRotateOnlyNotPlayableHud()
        {
            var safe=MobileLayout.ToGuiSafeArea(852,new Rect(0,34,393,759));
            var layout=MobileLayout.Compute(safe.width,safe.height);
            Assert.True(layout.Portrait);
            Assert.True(layout.PhoneHud);
            Assert.That(File.ReadAllText("PLAYTEST_GUIDE.md"),Does.Contain("rotate").IgnoreCase);
        }

        [Test] public void Phase5O1_PhoneLandscapeUsesWiderReadableHudWithoutClipping()
        {
            foreach(var size in new[]{new Vector2(800,360),new Vector2(734,372)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                Assert.False(layout.Portrait);
                Assert.True(layout.PhoneLandscape);
                Assert.That(layout.Panel.width,Is.EqualTo(size.x).Within(.01f));
                Assert.That(layout.Panel.y,Is.EqualTo(layout.Board.yMax).Within(.01f));
                Assert.That(layout.Board.width,Is.EqualTo(size.x).Within(.01f));
                Assert.True(hud.RequiredElementsFit(layout.Panel),size.ToString());
                Assert.False(hud.HasEssentialOverlap(),size.ToString());
                Assert.True(hud.TouchTargetsValid(),size.ToString());
                Assert.AreEqual(3,hud.SkillCards.Length);
                Assert.That(hud.SkillCards.All(r=>r.height>=50f&&r.width>=MobileLayoutSnapshot.MinimumTouchTarget),size.ToString());
                Assert.That(hud.StatChips.All(r=>r.height>=40f),size.ToString());
                Assert.That(hud.EndTurnButton.height,Is.GreaterThanOrEqualTo(50f),size.ToString());
                Assert.That(hud.EndTurnButton.yMax,Is.LessThanOrEqualTo(layout.Panel.yMax),size.ToString());
            }
        }

        [Test] public void FirstTimeFlow_StartScreenAndHelpPanelAreExplicit()
        {
            var go=new GameObject("FirstTime");var game=go.AddComponent<LanternfallGame>();
            Assert.False(game.HasStarted);Assert.That(game.Message,Does.Contain("Start Run"));
            Assert.That(LanternfallGame.HowToPlayLines.Length,Is.GreaterThanOrEqualTo(5));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("room five")));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("AP")));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("Red")));
            Assert.That(LanternfallGame.HowToPlayLines.Any(l=>l.Contains("not gold")));
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
            var start=game.Player.Position;game.TapTile(new Vector2Int(-1,-1));Assert.False(game.LastInputAccepted);Assert.AreEqual(start,game.Player.Position);Assert.AreEqual("INVALID: Invalid destination.",game.Message);Assert.False(game.Message.Contains("Cyan ="));Assert.True(game.RejectedTile.HasValue);
            var valid=game.ValidTargets.First();game.TapTile(valid);Assert.True(game.LastInputAccepted);Assert.AreEqual(valid,game.Player.Position);
            Object.DestroyImmediate(go);
        }

        [Test] public void TouchFlow_RewardAdvancesRoomAndRestartRestoresRun()
        {
            var go=new GameObject("RewardTap");var game=go.AddComponent<LanternfallGame>();game.StartRun();game.Turns.ShowReward();
            game.ChooseReward(1);Assert.AreEqual(2,game.RoomNumber);Assert.AreEqual(1,game.Player.Power);Assert.That(game.Message,Does.Contain("Reward applied: Bright Wick"));
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

        [Test] public void Phase5M_AllClassesCanStartAndSeeLabeledActions()
        {
            foreach(var cls in ClassCatalog.All)
            {
                var go=new GameObject("ClassStart");var game=go.AddComponent<LanternfallGame>();game.SelectClass(cls.id);game.StartRun(6100+(int)cls.id);
                Assert.AreEqual(cls.id,game.Player.ClassId);
                Assert.That(game.ValidTargets.Count,Is.GreaterThan(0),cls.name);
                Assert.True(SkillBook.ForClass(cls.id).All(s=>s.ApCost>0&&!string.IsNullOrWhiteSpace(s.Name)&&!string.IsNullOrWhiteSpace(s.Hint)),cls.name);
                Object.DestroyImmediate(go);
            }
        }

        [Test] public void Phase5M_CooldownsAndInvalidSkillTargetsRejectSafely()
        {
            var go=new GameObject("Cooldowns");var game=go.AddComponent<LanternfallGame>();game.SelectClass(PlayerClassId.Cantor);game.StartRun(6200);
            var enemy=game.Enemies.First();enemy.Position=game.Grid.Neighbors(game.Player.Position).First(p=>game.Grid.IsFloor(p));
            game.RefreshPreviews();game.SelectSkill(SkillId.EmberBolt);Assert.True(game.ValidTargets.Contains(enemy.Position));
            game.TapTile(enemy.Position);Assert.True(game.Player.Cooldowns[SkillBook.Get(SkillId.EmberBolt).Name]>0);
            game.SelectSkill(SkillId.EmberBolt);Assert.False(game.LastInputAccepted);Assert.That(game.Message,Does.Contain("cooling"));
            game.SelectSkill(SkillId.CinderBloom);var invalid=game.Grid.Floors().First(p=>!game.ValidTargets.Contains(p)&&p!=game.Player.Position);
            game.TapTile(invalid);Assert.False(game.LastInputAccepted);Assert.That(game.Message,Does.StartWith("INVALID"));
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5M_InvalidRewardChoiceDoesNotAdvanceOrMutateStats()
        {
            var go=new GameObject("BadReward");var game=go.AddComponent<LanternfallGame>();game.StartRun(6300);game.Turns.ShowReward();
            int room=game.RoomNumber,hp=game.Player.MaxHealth,power=game.Player.Power,mp=game.Player.MoveRange;
            game.ChooseReward(99);
            Assert.False(game.LastInputAccepted);Assert.AreEqual(room,game.RoomNumber);Assert.AreEqual(hp,game.Player.MaxHealth);Assert.AreEqual(power,game.Player.Power);Assert.AreEqual(mp,game.Player.MoveRange);
            Assert.That(game.Message,Does.Contain("reward"));
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5D_RewardsSupportDifferentClassNeeds()
        {
            var go=new GameObject("Rewards");var game=go.AddComponent<LanternfallGame>();game.StartRun(8010);game.Turns.ShowReward();
            int hp=game.Player.MaxHealth;game.ChooseReward(0);Assert.AreEqual(hp+3,game.Player.MaxHealth);
            game.Turns.ShowReward();int power=game.Player.Power;game.ChooseReward(1);Assert.AreEqual(power+1,game.Player.Power);
            game.Turns.ShowReward();int mp=game.Player.MoveRange;game.ChooseReward(2);Assert.AreEqual(mp+1,game.Player.MoveRange);
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5I_GameFeelMessagesCallOutTransitionsAndOutcomes()
        {
            var go=new GameObject("FeelTurn");var game=go.AddComponent<LanternfallGame>();game.StartRun(9090);
            game.WaitTurn();Assert.That(game.Message,Does.Contain("ENEMY TURN"));
            Object.DestroyImmediate(go);

            go=new GameObject("FeelBoss");game=go.AddComponent<LanternfallGame>();game.StartRun(9091);
            for(int i=0;i<4;i++){game.Turns.ShowReward();game.ChooseReward(0);}
            Assert.That(game.Message,Does.Contain("Reward applied").And.Contain("BOSS ROOM"));
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
            Assert.AreEqual(0,BalanceConfig.BetweenRoomRecovery);
            Assert.AreEqual(3,BalanceConfig.HealingPickupAmount);
            Assert.AreEqual(24,BalanceConfig.EnemyStats(EnemyKind.LanternWarden).health);
            Assert.AreEqual(1,SkillBook.Get(SkillId.EmberBolt).Cooldown);
            Assert.That(BalanceConfig.EnemyStats(EnemyKind.StoneSentinel).damage,Is.GreaterThanOrEqualTo(3));
            Assert.AreEqual(4,BalanceConfig.EnemyCount(4));
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
            var builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(builder,Does.Contain("canvas.style.width = \\\"100%\\\""));
            Assert.That(builder,Does.Contain("canvas.style.height = \\\"100%\\\""));
            Assert.That(builder,Does.Contain("Cache-Control"));
            Assert.That(builder,Does.Contain("cacheBust"));
            Assert.That(builder,Does.Contain("LanternfallTactics.wasm?"));
            Assert.That(builder,Does.Contain("#unity-footer { display: none; }"));
            Assert.That(builder,Does.Contain("width: 100%"));
            Assert.That(builder,Does.Contain("max-width: 100%"));
            Assert.That(builder,Does.Contain("height: 100dvh"));
        }

        [Test] public void Phase5G_PlaytestReleaseFilesAndVersionLabelArePrepared()
        {
            Assert.AreEqual("Prototype v0.6M",LanternfallView.PrototypeVersion);
            Assert.True(File.Exists("PLAYTEST_GUIDE.md"));
            var guide=File.ReadAllText("PLAYTEST_GUIDE.md");
            Assert.That(guide,Does.Contain("https://trisman909.github.io/LanternfallTacticsPrototype/"));
            Assert.That(guide,Does.Contain("Did the game load?"));
            Assert.That(guide,Does.Contain("Were AP and MP clear?"));
            Assert.That(guide,Does.Contain("Did anything break?"));
        }

        [Test] public void Phase5H_ShareReadyGuideAndLoadingCopyArePrepared()
        {
            var guide=File.ReadAllText("PLAYTEST_GUIDE.md");
            Assert.That(guide,Does.Contain("Wait for the Unity loading bar"));
            Assert.That(guide,Does.Contain("Was End Turn easy to find?"));
            Assert.That(guide,Does.Contain("Known limitations"));
            if(File.Exists("docs/index.html"))
                Assert.That(File.ReadAllText("docs/index.html"),Does.Contain("Loading Lanternfall Tactics"));
            if(File.Exists("docs/TemplateData/style.css"))
                Assert.That(File.ReadAllText("docs/TemplateData/style.css"),Does.Contain("lanternfall-loading-copy"));
        }

        [Test] public void Phase5J_FeedbackLogTemplateIsReadyForFirstPlaytests()
        {
            Assert.True(File.Exists("PLAYTEST_FEEDBACK_LOG.md"));
            var log=File.ReadAllText("PLAYTEST_FEEDBACK_LOG.md");
            Assert.That(log,Does.Contain("first external tester feedback"));
            Assert.That(log,Does.Contain("Entry template"));
            Assert.That(log,Does.Contain("Device/browser"));
            Assert.That(log,Does.Contain("Severity"));
            Assert.That(log,Does.Contain("Fixed in commit"));
            Assert.That(File.ReadAllText("PLAYTEST_GUIDE.md"),Does.Contain("PLAYTEST_FEEDBACK_LOG.md"));
        }

        [Test] public void Phase5K_WebGLShareDocsRemainDesktopFirstAndTroubleshootable()
        {
            var readme=File.ReadAllText("README.md");
            var guide=File.ReadAllText("PLAYTEST_GUIDE.md");
            var webgl=File.ReadAllText("WEBGL_PREVIEW.md");
            Assert.That(readme,Does.Contain("Best played first on a desktop browser"));
            Assert.That(guide,Does.Contain("Mobile browser play is landscape-first"));
            Assert.That(webgl,Does.Contain("GitHub Pages troubleshooting"));
            Assert.That(webgl,Does.Contain("master").And.Contain("/docs"));
            Assert.That(readme,Does.Contain("https://trisman909.github.io/LanternfallTacticsPrototype/"));
        }

        [Test] public void Phase5K_SmokeLogsContainNoObviousRuntimeErrorsWhenPresent()
        {
            foreach(var path in Directory.GetFiles(".", "WindowsSmoke_*.log"))
            {
                var text=File.ReadAllText(path);
                Assert.That(text,Does.Not.Contain("NullReferenceException"),path);
                Assert.That(text,Does.Not.Contain("Crash"),path);
                Assert.That(text,Does.Not.Contain("Unhandled"),path);
            }
        }

        [Test] public void Phase5L_PlaytestInfoAndImportantLabelsExist()
        {
            Assert.That(LanternfallGame.PlaytestInfoLines.Length,Is.GreaterThanOrEqualTo(5));
            Assert.True(LanternfallGame.PlaytestInfoLines.All(l=>l.Length<=100));
            Assert.True(LanternfallGame.HowToPlayLines.All(l=>l.Length<=100));
            Assert.That(LanternfallGame.PlaytestInfoLines.Any(l=>l.Contains("desktop browser")));
            Assert.That(LanternfallGame.PlaytestInfoLines.Any(l=>l.Contains("mobile browser")));
            Assert.That(LanternfallGame.PlaytestInfoLines.Any(l=>l.Contains("Known limits")));
            var guide=File.ReadAllText("PLAYTEST_GUIDE.md");
            Assert.That(guide,Does.Contain("Prototype v0.6M"));
            Assert.That(guide,Does.Contain("what confused you"));
            Assert.That(guide,Does.Contain("What device/browser did you use?"));
            Assert.That(guide,Does.Contain("Which class felt best/worst?"));
            Assert.That(LanternfallGame.PlaytestInfoLines.Any(l=>l.Contains("confused")&&l.Contains("fun")&&l.Contains("broke")));
        }

        [Test] public void Phase5L_UnityGeneratedAndCacheFoldersStayIgnored()
        {
            var ignore=File.ReadAllText(".gitignore");
            foreach(var pattern in new[]{"/Library/","/Temp/","/Obj/","/Logs/","/UserSettings/","/Builds/","/.vs/","*.dmp"})
                Assert.That(ignore,Does.Contain(pattern),pattern);
        }

        [Test] public void Phase5K_CombatHudShowsHpApAndMpAsSeparateReadableValues()
        {
            var player=new PlayerModel(PlayerClassId.Artificer);
            Assert.AreEqual("HP 12/12",HudText.Hp(player.Health,player.MaxHealth));
            Assert.AreEqual("AP 7/7",HudText.Ap(player.ActionPoints,player.MaxActionPoints));
            Assert.AreEqual("MP 3/3",HudText.Mp(player.MovementPoints,player.MoveRange));
            Assert.AreEqual("PLAYER TURN",HudText.TurnLabel(TurnPhase.Player));
            Assert.AreEqual("END TURN",HudText.EndTurnButton);
        }

        [Test] public void Phase5K_AllSkillCardsExposeCostStateAndReadableSummary()
        {
            foreach(var cls in ClassCatalog.All)
            foreach(var s in SkillBook.ForClass(cls.id))
            {
                var ready=HudText.SkillCard(s,0,9,TurnPhase.Player,false,false);
                Assert.That(ready,Does.Contain(s.Name),cls.name);
                Assert.That(ready,Does.Contain($"AP {s.ApCost}"),s.Name);
                Assert.That(ready,Does.Contain("READY"),s.Name);
                Assert.That(ready,Does.Contain(s.Hint),s.Name);
                Assert.That(HudText.SkillState(s,2,9,TurnPhase.Player),Does.Contain("CD 2"));
                Assert.That(HudText.SkillState(s,0,0,TurnPhase.Player),Does.Contain("AP"));
            }
        }

        [Test] public void Phase5K_DesktopWebGLCombatHudFitsWithoutEssentialCropping()
        {
            var layout=MobileLayout.Compute(1280,720);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            Assert.False(layout.HasOverlap);
            Assert.True(hud.RequiredElementsFit(layout.Panel));
            Assert.False(hud.HasEssentialOverlap());
            Assert.True(hud.TouchTargetsValid());
            Assert.That(hud.Header.yMin,Is.GreaterThanOrEqualTo(layout.Panel.yMin+12f));
            Assert.That(hud.Header.height,Is.GreaterThanOrEqualTo(52f));
            Assert.AreEqual(3,hud.StatChips.Length);
            Assert.AreEqual(3,hud.SkillCards.Length);
            Assert.That(hud.StatChips.All(r=>r.yMin>hud.Header.yMax),Is.True);
            Assert.That(hud.HazardNote.yMin,Is.GreaterThan(hud.StatChips.Max(r=>r.yMax)));
            Assert.That(hud.SkillCards.All(r=>r.yMin>hud.SelectedSkill.yMax),Is.True);
            Assert.That(hud.SkillCards.All(r=>r.width>=MobileLayoutSnapshot.MinimumTouchTarget),Is.True);
            Assert.That(hud.SkillCards.All(r=>r.height>=86f),Is.True);
            Assert.That(hud.Message.yMin,Is.GreaterThan(hud.EndTurnButton.yMax));
        }

        [Test] public void Phase5N_RoomClearRewardLayoutKeepsHeaderAndCardsSeparated()
        {
            var layout=MobileLayout.Compute(1280,720);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            var reward=RewardPanelLayout.Compute(hud.SelectedSkill.x,hud.SelectedSkill.y,hud.SelectedSkill.width,true);
            Assert.True(reward.Fits(layout.Panel));
            Assert.False(reward.HasOverlap());
            Assert.AreEqual(3,reward.Cards.Length);
            Assert.That(reward.Header.height,Is.GreaterThanOrEqualTo(50f));
            Assert.That(reward.Cards.All(r=>r.yMin>=reward.Header.yMax+RewardPanelLayout.Gap-.01f));
            Assert.That(reward.Cards.All(r=>r.height>=76f));

            var portrait=MobileLayout.Compute(393,852);
            var portraitHud=CombatHudLayout.Compute(portrait.Panel,portrait.Portrait,portrait.CompactLandscape);
            var portraitReward=RewardPanelLayout.Compute(portraitHud.SelectedSkill.x,portraitHud.SelectedSkill.y,portraitHud.SelectedSkill.width,false);
            Assert.True(portraitReward.Fits(portrait.Panel));
            Assert.False(portraitReward.HasOverlap());
        }

        [Test] public void Phase5N_HealingPickupHealsCapsAndDisappears()
        {
            var go=new GameObject("PickupHeal");var game=go.AddComponent<LanternfallGame>();game.StartRun(5555);
            var target=game.ValidTargets.First(t=>game.Grid.IsFloor(t)&&t!=game.Player.Position);
            typeof(LanternfallGame).GetProperty("HealingPickup").GetSetMethod(true).Invoke(game,new object[]{(Vector2Int?)target});
            game.Player.Damage(5);int before=game.Player.Health;
            game.TapTile(target);
            Assert.AreEqual(before+BalanceConfig.HealingPickupAmount,game.Player.Health);
            Assert.False(game.HealingPickup.HasValue);
            Assert.That(game.Message,Does.Contain("Lantern bloom"));
            Object.DestroyImmediate(go);

            go=new GameObject("PickupCap");game=go.AddComponent<LanternfallGame>();game.StartRun(5556);
            target=game.ValidTargets.First(t=>game.Grid.IsFloor(t)&&t!=game.Player.Position);
            typeof(LanternfallGame).GetProperty("HealingPickup").GetSetMethod(true).Invoke(game,new object[]{(Vector2Int?)target});
            int max=game.Player.MaxHealth;
            game.TapTile(target);
            Assert.AreEqual(max,game.Player.Health);
            Assert.False(game.HealingPickup.HasValue);
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5O_ApMpPressureCanBeQueuedForNextPlayerTurn()
        {
            var go=new GameObject("Pressure");var game=go.AddComponent<LanternfallGame>();game.StartRun(9191);
            var archer=new EnemyModel(EnemyKind.GloomArcher,game.Grid.Neighbors(game.Player.Position).First());
            EnemyAI.AssignIntent(archer,game.Player.Position,game.Grid);
            typeof(LanternfallGame).GetMethod("ApplyIntentPressure",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(game,new object[]{archer});
            Assert.That(game.PendingApDrain,Is.GreaterThan(0));
            var sentinel=new EnemyModel(EnemyKind.StoneSentinel,game.Grid.Neighbors(game.Player.Position).Last());
            EnemyAI.AssignIntent(sentinel,game.Player.Position,game.Grid);
            typeof(LanternfallGame).GetMethod("ApplyIntentPressure",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(game,new object[]{sentinel});
            Assert.That(game.PendingMpDrain,Is.GreaterThan(0));
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase5O_HealingPickupAndWebGLLoadingCopyAreVisible()
        {
            var guide=File.ReadAllText("PLAYTEST_GUIDE.md");
            Assert.That(guide,Does.Contain("green").And.Contain("healing pickup"));
            if(File.Exists("docs/index.html"))
                Assert.That(File.ReadAllText("docs/index.html"),Does.Contain("v=").And.Contain("Loading Lanternfall Tactics"));
        }

        [Test] public void Phase5P_ReadabilityDocsAndMobileControlsStayPrepared()
        {
            Assert.That(File.ReadAllText("PLAYTEST_GUIDE.md"),Does.Contain("Purple").Or.Contain("purple"));
            var portrait=MobileLayout.Compute(393,759);
            Assert.True(portrait.PhoneHud);
            Assert.True(portrait.Portrait);
            Assert.That(File.ReadAllText("Assets/Scripts/LanternfallView.cs"),Does.Contain("Rotate your phone to play"));
            var landscape=MobileLayout.Compute(800,360);
            var landscapeHud=CombatHudLayout.Compute(landscape.Panel,landscape.Portrait,landscape.CompactLandscape);
            Assert.True(landscape.PhoneHud);
            Assert.True(landscapeHud.RequiredElementsFit(landscape.Panel));
            Assert.True(landscapeHud.TouchTargetsValid());
        }

        [Test] public void Phase5Q4_PhonePortraitAlwaysUsesRotateScreenContract()
        {
            foreach(var size in new[]{new Vector2(393,759),new Vector2(390,700),new Vector2(360,640),new Vector2(360,740),new Vector2(430,932),new Vector2(500,1000)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                Assert.True(layout.Portrait,size.ToString());
                Assert.True(layout.PhoneHud,size.ToString());
                var source=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
                Assert.That(source,Does.Contain("DrawRotatePhoneScreen"));
                Assert.That(source,Does.Contain("Lanternfall Tactics is best played in landscape"));
            }
        }

        [Test] public void Phase5Q4_PhoneLandscapeUsesBottomHudForRealBrowserViewports()
        {
            foreach(var size in new[]{new Vector2(800,360),new Vector2(734,340),new Vector2(734,372),new Vector2(844,390),new Vector2(852,393),new Vector2(915,412),new Vector2(932,430),new Vector2(1024,500),new Vector2(1100,560)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                Assert.False(layout.Portrait,size.ToString());
                Assert.True(layout.PhoneLandscape,size.ToString());
                Assert.True(layout.PhoneHud,size.ToString());
                Assert.That(layout.FontSize,Is.GreaterThanOrEqualTo(34),size.ToString());
                Assert.That(layout.Panel.width,Is.EqualTo(size.x).Within(.01f),size.ToString());
                Assert.That(layout.Panel.y,Is.EqualTo(layout.Board.yMax).Within(.01f),size.ToString());
                Assert.That(layout.Board.width,Is.EqualTo(size.x).Within(.01f),size.ToString());
                float minBoardShare = size.y <= 360 ? .47f : size.y <= 380 ? .52f : size.y <= 400 ? .54f : size.y <= 430 ? .56f : .60f;
                Assert.That(layout.Board.height/size.y,Is.InRange(minBoardShare,.72f),size.ToString());
                float maxPanelShare = size.y <= 360 ? .53f : size.y <= 380 ? .48f : size.y <= 400 ? .46f : size.y <= 430 ? .44f : .42f;
                Assert.That(layout.Panel.height/size.y,Is.InRange(.28f,maxPanelShare),size.ToString());
                Assert.True(hud.RequiredElementsFit(layout.Panel),size.ToString());
                Assert.False(hud.HasEssentialOverlap(),size.ToString());
                Assert.True(hud.TouchTargetsValid(),size.ToString());
                Assert.That(hud.SkillCards.All(r=>r.height>=60f&&r.width>=MobileLayoutSnapshot.MinimumTouchTarget),size.ToString());
                Assert.That(hud.StatChips.All(r=>r.height>=44f&&r.width>=MobileLayoutSnapshot.MinimumTouchTarget),size.ToString());
                Assert.That(hud.EndTurnButton.height,Is.GreaterThanOrEqualTo(56f),size.ToString());
                Assert.That(hud.Header.height,Is.EqualTo(0f),size.ToString());
                Assert.That(hud.HelpButton.height,Is.EqualTo(0f),size.ToString());
                Assert.That(hud.InfoButton.height,Is.EqualTo(0f),size.ToString());
                Assert.That(hud.MinimumControlHeight(),Is.GreaterThanOrEqualTo(50f),size.ToString());
                Assert.That(hud.Message.yMax,Is.LessThanOrEqualTo(layout.Panel.yMax+.01f),size.ToString());
                Assert.That(hud.EndTurnButton.xMax,Is.LessThanOrEqualTo(size.x+.01f),size.ToString());
                Assert.That(hud.SkillCards.All(r=>r.xMin>=0&&r.xMax<=size.x+.01f),size.ToString());
            }
        }

        [Test] public void Phase6A_MobileHudUsesDedicatedBottomCommandBarAndHidesSecondaryInfo()
        {
            var layout=MobileLayout.Compute(932,430);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            Assert.True(layout.PhoneLandscape);
            Assert.That(layout.Panel.width,Is.EqualTo(932).Within(.01f));
            Assert.That(layout.Panel.y,Is.EqualTo(layout.Board.yMax).Within(.01f));
            Assert.That(layout.Board.height/layout.Panel.height,Is.GreaterThan(1.4f));
            Assert.That(hud.Header.height,Is.EqualTo(0f));
            Assert.That(hud.HelpButton.height,Is.EqualTo(0f));
            Assert.That(hud.InfoButton.height,Is.EqualTo(0f));
            Assert.That(hud.Message.height,Is.EqualTo(0f).Within(1.1f));
            Assert.That(hud.EndTurnButton.width,Is.GreaterThanOrEqualTo(118f));
            Assert.That(hud.SkillCards.All(r=>r.height>=56f&&r.width>=MobileLayoutSnapshot.MinimumTouchTarget));
            Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode);
        }

        [Test] public void Phase6B_IPhoneLandscapeSafeAreasNeverClipBoardOrHud()
        {
            var cases=new[]
            {
                new {Name="iPhone 13 Safari", ScreenH=390f, Safe=new Rect(47,0,750,390)},
                new {Name="iPhone 14 Safari", ScreenH=393f, Safe=new Rect(59,21,734,372)},
                new {Name="iPhone 15 Safari", ScreenH=430f, Safe=new Rect(59,0,814,430)},
                new {Name="iPhone 16 Safari", ScreenH=440f, Safe=new Rect(62,0,832,440)},
                new {Name="iPhone Chrome", ScreenH=402f, Safe=new Rect(48,0,778,402)}
            };
            foreach(var c in cases)
            {
                var safe=MobileLayout.ToGuiSafeArea(c.ScreenH,c.Safe);
                var layout=MobileLayout.Compute(safe.width,safe.height);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode,c.Name);
                Assert.False(layout.HasOverlap,c.Name);
                Assert.That(layout.Board.xMin,Is.GreaterThanOrEqualTo(0f),c.Name);
                Assert.That(layout.Board.xMax,Is.LessThanOrEqualTo(safe.width+.01f),c.Name);
                Assert.That(layout.Panel.xMin,Is.GreaterThanOrEqualTo(0f),c.Name);
                Assert.That(layout.Panel.xMax,Is.LessThanOrEqualTo(safe.width+.01f),c.Name);
                Assert.True(hud.RequiredElementsFit(layout.Panel),c.Name);
                Assert.False(hud.HasEssentialOverlap(),c.Name);
                Assert.True(hud.TouchTargetsValid(),c.Name);
                Assert.That(hud.StatChips.Concat(hud.SkillCards).Append(hud.EndTurnButton).All(r=>r.xMin>=layout.Panel.xMin&&r.xMax<=layout.Panel.xMax+.01f),c.Name);
                Assert.That(layout.EstimatedTileSize,Is.GreaterThan(9f),c.Name);
            }
        }

        [Test] public void Phase6B_WebGLShellAvoidsSafeAreaHorizontalOverflow()
        {
            var builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(builder,Does.Contain("canvas.style.width = \\\"100%\\\""));
            Assert.That(builder,Does.Contain("#unity-canvas { width: 100%"));
            Assert.That(builder,Does.Contain("max-width: 100%"));
            Assert.That(builder,Does.Contain("overflow: hidden"));
            Assert.That(builder,Does.Contain("env(safe-area-inset-left)"));
            Assert.That(builder,Does.Contain("v=6M"));
            Assert.That(builder,Does.Contain("Prototype v0.6M"));
        }

        [Test] public void Phase6B1_PhoneLandscapeUsesLargerReadableFontsAndRowsWithoutDesktopChange()
        {
            var phoneCases=new[]{new Vector2(734,372),new Vector2(844,390),new Vector2(932,430),new Vector2(1024,500)};
            foreach(var size in phoneCases)
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                var readable=MobileHudReadability.Compute(size.x,size.y);
                Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode,size.ToString());
                Assert.That(layout.FontSize,Is.GreaterThanOrEqualTo(38),size.ToString());
                Assert.That(readable.StatFont,Is.GreaterThanOrEqualTo(42),size.ToString());
                Assert.That(readable.ButtonFont,Is.GreaterThanOrEqualTo(38),size.ToString());
                Assert.That(readable.CompactSkillFont,Is.GreaterThanOrEqualTo(32),size.ToString());
                Assert.That(hud.StatChips.All(r=>r.height>=44f),size.ToString());
                Assert.That(hud.SkillCards.All(r=>r.height>=60f),size.ToString());
                Assert.That(hud.EndTurnButton.height,Is.GreaterThanOrEqualTo(56f),size.ToString());
                Assert.That(layout.Board.xMin,Is.EqualTo(0f).Within(.01f),size.ToString());
                Assert.That(layout.Board.xMax,Is.EqualTo(size.x).Within(.01f),size.ToString());
                Assert.That(hud.StatChips.Concat(hud.SkillCards).Append(hud.EndTurnButton).All(r=>r.xMin>=layout.Panel.xMin&&r.xMax<=layout.Panel.xMax+.01f),size.ToString());
                Assert.False(layout.HasOverlap,size.ToString());
            }

            var desktopLayout=MobileLayout.Compute(1280,720);
            var desktopReadable=MobileHudReadability.Compute(1280,720);
            Assert.AreEqual(MobileLayoutMode.TabletLandscape,desktopLayout.Mode);
            Assert.That(desktopReadable.StatFont,Is.EqualTo(19));
            Assert.That(desktopReadable.ButtonFont,Is.EqualTo(17));
            Assert.That(desktopReadable.CompactSkillFont,Is.EqualTo(14));
        }

        [Test] public void Phase6B1_MobileSkillLabelsStayCompactAndNonDescriptive()
        {
            var expected=new System.Collections.Generic.Dictionary<string,string>
            {
                {"Spear Thrust","SPEAR"},{"Guard Stance","GUARD"},{"Sun Charge","CHARGE"},
                {"Straight Shot","SHOT"},{"Marked Target","MARK"},{"Piercing Prism","PRISM"},
                {"Ember Bolt","BOLT"},{"Cinder Bloom","BLOOM"},{"Delayed Blast","BLAST"},
                {"Lens Trap","TRAP"},{"Redirect Shot","REDIRECT"},{"Shield Gadget","SHIELD"}
            };
            foreach(var cls in ClassCatalog.All)
            foreach(var skill in SkillBook.ForClass(cls.id))
            {
                var mobileName=HudText.MobileSkillName(skill);
                if(expected.TryGetValue(skill.Name,out var shortName)) Assert.AreEqual(shortName,mobileName,skill.Name);
                var label=HudText.MobileSkillCard(skill,0,9,TurnPhase.Player,false);
                Assert.That(label,Does.Contain(mobileName),skill.Name);
                Assert.That(label,Does.Contain($"AP {skill.ApCost}"),skill.Name);
                Assert.That(label,Does.Contain("READY"),skill.Name);
                Assert.That(label,Does.Not.Contain(skill.Hint),skill.Name);
                Assert.That(label.Split('\n').Length,Is.LessThanOrEqualTo(2),skill.Name);
                Assert.That(label.Length,Is.LessThanOrEqualTo(24),skill.Name);
            }
        }

        [Test] public void Phase6C_EndTurnIsCenteredInExistingPhoneLandscapeCommandBar()
        {
            foreach(var size in new[]{new Vector2(734,372),new Vector2(844,390),new Vector2(932,430),new Vector2(1024,500)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode,size.ToString());
                Assert.That(hud.EndTurnButton.center.x,Is.EqualTo(layout.Panel.center.x).Within(.01f),size.ToString());
                Assert.That(hud.CancelButton.xMax,Is.LessThanOrEqualTo(hud.EndTurnButton.xMin),size.ToString());
                Assert.False(hud.HasEssentialOverlap(),size.ToString());
            }
        }

        [Test] public void Phase6D_AuthoredAtlasesAreLightweightAndPhoneLayoutRemainsUntouched()
        {
            var iconPath="Assets/Resources/UI/phase6d_icon_atlas.png";
            var uiPath="Assets/Resources/UI/phase6d_ui_atlas.png";
            Assert.True(File.Exists(iconPath));Assert.True(File.Exists(uiPath));
            Assert.That(new FileInfo(iconPath).Length,Is.LessThan(700000));
            Assert.That(new FileInfo(uiPath).Length,Is.LessThan(700000));
            foreach(var size in new[]{new Vector2(734,372),new Vector2(844,390),new Vector2(932,430)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
                Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode);
                Assert.True(hud.RequiredElementsFit(layout.Panel));Assert.False(hud.HasEssentialOverlap());
                Assert.That(hud.EndTurnButton.center.x,Is.EqualTo(layout.Panel.center.x).Within(.01f));
            }
            Assert.True(File.Exists("ART_ASSETS_6D.md"));
        }

        [Test] public void Phase6E_UnitAtlasIsLightweightAndDocumented()
        {
            const string path="Assets/Resources/Units/phase6e_unit_atlas.png";
            Assert.True(File.Exists(path));Assert.That(new FileInfo(path).Length,Is.LessThan(600000));
            Assert.True(File.Exists("ART_ASSETS_6E.md"));
            var notes=File.ReadAllText("ART_ASSETS_6E.md");
            foreach(var name in new[]{"Vanguard","Wayfinder","Cantor","Gloamstep","Artificer","Ashling","Gloom Archer","Stone Sentinel","Lantern Warden"})Assert.That(notes,Does.Contain(name));
            Assert.That(notes,Does.Contain("production-placeholder").And.Contain("Phase 2"));
        }

        [Test] public void Phase5Q4_WebGLTemplateAddsBrowserLevelPortraitBlocker()
        {
            var builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(builder,Does.Contain("lanternfall-rotate-overlay"));
            Assert.That(builder,Does.Contain("window.innerWidth"));
            Assert.That(builder,Does.Contain("window.innerHeight"));
            Assert.That(builder,Does.Contain("orientationchange"));
            Assert.That(builder,Does.Contain("lanternfall-phone-portrait"));
            Assert.That(builder,Does.Contain("v=6M"));
            Assert.That(builder,Does.Contain("Prototype v0.6M"));
        }

        [Test] public void Phase5Q2_PhoneHudUsesShortSkillLabelsAndHidesSecondaryCombatInfo()
        {
            foreach(var cls in ClassCatalog.All)
            foreach(var skill in SkillBook.ForClass(cls.id))
            {
                var label=HudText.MobileSkillCard(skill,0,9,TurnPhase.Player,false);
                Assert.That(label,Does.Contain("AP"));
                Assert.That(label,Does.Contain("READY"));
                Assert.That(label,Does.Not.Contain(skill.Hint),skill.Name);
                Assert.That(HudText.MobileSkillName(skill).Length,Is.LessThanOrEqualTo(8),skill.Name);
            }
            var landscape=MobileLayout.Compute(734,372);
            var landscapeHud=CombatHudLayout.Compute(landscape.Panel,landscape.Portrait,landscape.CompactLandscape);
            Assert.True(landscape.PhoneHud);
            Assert.False(landscape.Portrait);
            Assert.That(landscapeHud.HelpButton.height,Is.EqualTo(0f));
            Assert.That(landscapeHud.InfoButton.height,Is.EqualTo(0f));
            Assert.That(landscapeHud.Header.height,Is.EqualTo(0f));
            Assert.That(landscapeHud.StatChips.All(r=>r.height>=40f));
            Assert.That(landscapeHud.SkillCards.All(r=>r.height>=56f));
            Assert.That(landscapeHud.SkillCards.All(r=>r.width>=MobileLayoutSnapshot.MinimumTouchTarget));
            Assert.That(landscapeHud.EndTurnButton.height,Is.GreaterThanOrEqualTo(50f));
        }

        [Test] public void Phase5K_ShortMobileLandscapeKeepsAllSkillsAndEndTurnAccessible()
        {
            var safe=MobileLayout.ToGuiSafeArea(393,new Rect(59,21,734,372));
            var layout=MobileLayout.Compute(safe.width,safe.height);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            Assert.True(layout.CompactLandscape);
            Assert.True(hud.RequiredElementsFit(layout.Panel));
            Assert.False(hud.HasEssentialOverlap());
            Assert.True(hud.TouchTargetsValid());
            Assert.That(layout.Panel.y,Is.EqualTo(layout.Board.yMax).Within(.01f));
            Assert.That(hud.SkillCards.All(r=>r.height>=56f));
            Assert.That(hud.HelpButton.height,Is.EqualTo(0f));
            Assert.That(hud.EndTurnButton.height,Is.GreaterThanOrEqualTo(50f));
        }

        [Test] public void Phase5K_HelpIsCollapsedDuringCombatAndCanOpenClose()
        {
            var go=new GameObject("HudHelpContract");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            Assert.False(game.HelpVisible);
            game.ShowHelp();Assert.True(game.HelpVisible);
            game.HideHelp();Assert.False(game.HelpVisible);
            game.ShowPlaytestInfo();Assert.True(game.PlaytestInfoVisible);
            game.HidePlaytestInfo();Assert.False(game.PlaytestInfoVisible);
            Object.DestroyImmediate(go);
        }

        [Test] public void Phase6F_BiomeAtlasesAreLightweightAndDocumented()
        {
            foreach(BiomeId biome in System.Enum.GetValues(typeof(BiomeId)))
            {
                var path="Assets/Resources/"+AuthoredBiomes.Resource(biome)+".png";
                Assert.True(File.Exists(path),path);
                Assert.That(new FileInfo(path).Length,Is.LessThan(700000),path);
            }
            Assert.True(File.Exists("ART_ASSETS_6F.md"));
            var notes=File.ReadAllText("ART_ASSETS_6F.md");
            Assert.That(notes,Does.Contain("production-placeholder"));
            Assert.That(notes,Does.Contain("No generation, connectivity, hazard behavior, combat, balance, AI, or layout logic changes"));
        }

        [Test] public void Phase6G_EffectsAreLightweightReducedMotionAwareAndDocumented()
        {
            Assert.True(File.Exists("EFFECTS_6G.md"));
            var notes=File.ReadAllText("EFFECTS_6G.md");
            Assert.That(notes,Does.Contain("Visual effect checklist"));
            Assert.That(notes,Does.Contain("Full/Reduced Motion"));
            var view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(view,Does.Contain("PresentationMotion.Reduced"));
            Assert.That(view,Does.Contain("DrawBoardEffects"));
            Assert.That(view,Does.Not.Contain("ParticleSystem"));
            Assert.That(view,Does.Not.Contain("WaitForSeconds"));
        }

        [Test] public void Phase6H_ReadabilityPolishIsDocumentedAndReusesExistingAtlases()
        {
            Assert.True(File.Exists("READABILITY_6H.md"));
            var notes=File.ReadAllText("READABILITY_6H.md");
            Assert.That(notes,Does.Contain("player > enemies > danger/targets > board > background"));
            Assert.That(notes,Does.Contain("No textures or runtime systems were added"));
            var view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(view,Does.Contain("QuietEnvironmentOverlay"));
            Assert.That(view,Does.Contain("UnitTokenScale"));
            Assert.That(view,Does.Not.Contain("PostProcess"));
            Assert.That(view,Does.Not.Contain("DynamicLight"));
        }

        [Test] public void Phase6I_RemovesPlaceholderGlyphPathsAndAddsPwaPresentation()
        {
            Assert.True(File.Exists("INTEGRATION_6I.md"));
            var notes=File.ReadAllText("INTEGRATION_6I.md");
            Assert.That(notes,Does.Contain("Remaining placeholders"));
            Assert.That(notes,Does.Contain("centered End Turn command bar"));
            var view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            var visual=File.ReadAllText("Assets/Scripts/VisualReadability.cs");
            var biome=File.ReadAllText("Assets/Scripts/BiomeThemes.cs");
            Assert.That(view,Does.Not.Contain("DrawTileGlyph"));
            Assert.That(visual,Does.Not.Contain("ClassGlyph").And.Not.Contain("EnemyGlyph").And.Not.Contain("FloorGlyph").And.Not.Contain("StatusGlyph"));
            Assert.That(biome,Does.Not.Contain("PropGlyph"));
            Assert.That(visual,Does.Not.Contain("\u00e2").And.Not.Contain("\u00c2").And.Not.Contain("\u00c3"));
            var builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(builder,Does.Contain("manifest.webmanifest"));
            Assert.That(builder,Does.Contain("theme-color"));
            Assert.That(builder,Does.Contain("orientation\\\": \\\"landscape"));
            Assert.That(builder,Does.Contain("CARRY THE LIGHT"));
        }
    }
}



