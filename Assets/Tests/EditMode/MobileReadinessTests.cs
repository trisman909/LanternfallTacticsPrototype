using System.Linq;
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

        [Test] public void TouchFlow_SkillSelectionAndCancellationAreExplicit()
        {
            var go=new GameObject("TouchFlow");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            game.SelectSkill(SkillId.LanternDash);Assert.AreEqual(SkillId.LanternDash,game.SelectedSkill);Assert.True(game.LastInputAccepted);
            game.CancelSkill();Assert.IsNull(game.SelectedSkill);Assert.True(game.LastInputAccepted);Assert.That(game.Message,Does.Contain("cancelled"));
            Object.DestroyImmediate(go);
        }

        [Test] public void TouchFlow_InvalidAndValidTileTapsGiveClearResults()
        {
            var go=new GameObject("TileTap");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            var start=game.Player.Position;game.TapTile(new Vector2Int(-1,-1));Assert.False(game.LastInputAccepted);Assert.AreEqual(start,game.Player.Position);Assert.That(game.Message,Does.StartWith("✕"));
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

        [Test] public void TacticalWarnings_EnemyPreviewsAndEveryHazardRemainVisible()
        {
            var go=new GameObject("Warnings");var game=go.AddComponent<LanternfallGame>();game.StartRun();
            Assert.True(game.Enemies.Any(e=>e.Preview.Count>0));
            Assert.True(BiomeCatalog.All.All(b=>b.TileContrast>.06f&&!string.IsNullOrWhiteSpace(b.HazardName)));
            Object.DestroyImmediate(go);
        }
    }
}
