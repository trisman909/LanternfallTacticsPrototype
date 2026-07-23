using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6N1Tests
    {
        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneUsesTopSideBottomHudAndMakesBoardMateriallyLarger(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);float oldBoardHeight=height-Mathf.Clamp(height*.40f,178f,228f);
            Assert.True(layout.PhoneLandscape);Assert.Greater(layout.Board.height,oldBoardHeight*1.12f);Assert.Greater(layout.EstimatedTileSize,oldBoardHeight/11f);
            Assert.AreEqual(layout.TopBar.yMax,layout.Board.yMin,.01f);Assert.AreEqual(layout.Board.yMax,layout.SkillBar.yMin,.01f);Assert.AreEqual(layout.Board.xMax,layout.ThreatPanel.xMin,.01f);
            Assert.LessOrEqual(layout.TopBar.yMax,layout.Board.yMin+.01f);Assert.LessOrEqual(layout.Board.yMax,layout.SkillBar.yMin+.01f);Assert.LessOrEqual(layout.Board.xMax,layout.ThreatPanel.xMin+.01f);Assert.LessOrEqual(layout.ThreatPanel.yMax,layout.ActionButton.yMin+.01f);
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhonePrimaryControlsRemainComfortableAndContained(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);
            Assert.True(layout.SkillButtons.All(r=>r.height>=44f&&r.width>=MobileLayoutSnapshot.MinimumTouchTarget));Assert.GreaterOrEqual(layout.ActionButton.height,44f);
            Assert.True(layout.SkillButtons.All(r=>r.xMin>=layout.SkillBar.xMin&&r.yMin>=layout.SkillBar.yMin&&r.xMax<=layout.SkillBar.xMax&&r.yMax<=layout.SkillBar.yMax));Assert.True(layout.ActionButton.xMax<=width&&layout.ActionButton.yMax<=height);
            Assert.Greater(layout.TopBar.width,width*.78f);Assert.That(layout.ThreatPanel.width,Is.InRange(152f,174f));
        }

        [Test] public void ThreatRailKeepsRelevantCategoriesConciseAndCollapsesEmptyOnes()
        {
            var go=new GameObject("ThreatRail6N1");var game=go.AddComponent<LanternfallGame>();game.StartRun(16101);game.Enemies.Clear();Assert.Zero(game.MobileThreatRows().Length);
            var adjacent=game.Grid.Neighbors(game.Player.Position).First();game.Enemies.Add(new EnemyModel(EnemyKind.StoneSentinel,adjacent));game.RefreshPreviews();var rows=game.MobileThreatRows();Assert.AreEqual("INCOMING NOW",rows[0].Category);Assert.That(rows[0].Action,Does.Contain("Shield Bash").And.Contain("DMG"));Assert.False(rows[0].Action.Contains("incoming damage"));Object.DestroyImmediate(go);
        }

        [TestCase(1280f,720f)] [TestCase(1920f,1080f)]
        public void DesktopArchitectureIsUnchanged(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);Assert.False(layout.PhoneLandscape);Assert.AreEqual(0f,layout.TopBar.width);Assert.AreEqual(0f,layout.SkillBar.width);Assert.AreEqual(0f,layout.ThreatPanel.width);Assert.That(layout.Panel.xMin,Is.EqualTo(layout.Board.xMax).Within(.01f));
        }

        [Test] public void PortraitStillUsesRotationContract()
        {
            var layout=MobileLayout.Compute(390,844);Assert.True(layout.PhoneHud);Assert.True(layout.Portrait);Assert.AreEqual(MobileLayoutMode.PhonePortrait,layout.Mode);
        }

        [Test] public void PhoneBoardFitUsesReclaimedHeaderSpace()
        {
            var area=MobileLayout.Compute(932,430).Board;
            var legacy=BoardFitLayout.Compute(area,9,11,true);
            var phone=BoardFitLayout.Compute(area,9,11,true,true);
            Assert.Greater(phone.TileSize,legacy.TileSize);
            Assert.True(phone.Fits(area));
        }
    }
}
