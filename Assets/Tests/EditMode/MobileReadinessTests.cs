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
            Assert.That(layout.EstimatedTileSize,Is.GreaterThanOrEqualTo(24));
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
            var start=game.Player.Position;game.TapTile(new Vector2Int(-1,-1));Assert.False(game.LastInputAccepted);Assert.AreEqual(start,game.Player.Position);Assert.That(game.Message,Does.StartWith("INVALID"));Assert.That(game.Message,Does.Contain("Cyan"));Assert.That(game.Message,Does.Contain("gold"));Assert.True(game.RejectedTile.HasValue);
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
            Assert.AreEqual(2,BalanceConfig.BetweenRoomRecovery);
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
            var index=File.ReadAllText("docs/index.html");
            var css=File.ReadAllText("docs/TemplateData/style.css");
            Assert.That(index,Does.Contain("canvas.style.width = \"100vw\""));
            Assert.That(index,Does.Contain("canvas.style.height = \"100vh\""));
            Assert.That(index,Does.Contain("Cache-Control"));
            Assert.That(index,Does.Contain("cacheBust"));
            Assert.That(index,Does.Contain("LanternfallTactics.wasm?"));
            Assert.That(css,Does.Contain("#unity-footer { display: none; }"));
            Assert.That(css,Does.Contain("width: 100vw"));
            Assert.That(css,Does.Contain("height: 100vh"));
        }

        [Test] public void Phase5G_PlaytestReleaseFilesAndVersionLabelArePrepared()
        {
            Assert.AreEqual("Prototype v0.5L",LanternfallView.PrototypeVersion);
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
            Assert.That(guide,Does.Contain("Mobile browser play is experimental"));
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
            Assert.That(guide,Does.Contain("Prototype v0.5L"));
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

        [Test] public void Phase5K_ShortMobileLandscapeKeepsAllSkillsAndEndTurnAccessible()
        {
            var safe=MobileLayout.ToGuiSafeArea(393,new Rect(59,21,734,372));
            var layout=MobileLayout.Compute(safe.width,safe.height);
            var hud=CombatHudLayout.Compute(layout.Panel,layout.Portrait,layout.CompactLandscape);
            Assert.True(layout.CompactLandscape);
            Assert.True(hud.RequiredElementsFit(layout.Panel));
            Assert.False(hud.HasEssentialOverlap());
            Assert.True(hud.TouchTargetsValid(42f));
            Assert.That(hud.SkillCards.All(r=>r.height>=56f));
            Assert.That(hud.HelpButton.height,Is.GreaterThanOrEqualTo(44f));
            Assert.That(hud.EndTurnButton.height,Is.GreaterThanOrEqualTo(44f));
            Assert.That(hud.Message.yMin,Is.GreaterThan(hud.EndTurnButton.yMax));
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
    }
}
