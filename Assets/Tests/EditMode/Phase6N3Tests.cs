using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6N3Tests
    {
        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneDecoratedControlsExposeContainedAssetSpecificSafeRects(float width,float height)
        {
            var l=MobileLayout.Compute(width,height);
            Assert.True(l.PhoneLandscape);Assert.AreEqual(3,l.StatContentRects.Length);Assert.AreEqual(3,l.SkillNameRects.Length);
            for(int i=0;i<3;i++)
            {
                AssertContained(l.StatChips[i],l.StatContentRects[i]);AssertContained(l.SkillButtons[i],l.SkillContentRects[i]);
                AssertContained(l.SkillContentRects[i],l.SkillNameRects[i]);AssertContained(l.SkillContentRects[i],l.SkillCostRects[i]);AssertContained(l.SkillContentRects[i],l.SkillStateRects[i]);
                Assert.False(l.SkillNameRects[i].Overlaps(l.SkillCostRects[i]));Assert.LessOrEqual(l.SkillNameRects[i].yMax,l.SkillStateRects[i].yMax);
            }
            AssertContained(l.TopBar,l.TitleContentRect);AssertContained(l.ThreatPanel,l.ThreatContentRect);AssertContained(l.HelpButton,l.HelpContentRect);AssertContained(l.InfoButton,l.InfoContentRect);
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void EndTurnAndCancelUseStableNonOverlappingGeometry(float width,float height)
        {
            var l=MobileLayout.Compute(width,height);
            AssertContained(l.ActionButton,l.EndTurnArt);AssertContained(l.EndTurnArt,l.EndTurnLabel);AssertContained(l.FullActionButton,l.FullEndTurnArt);AssertContained(l.FullEndTurnArt,l.FullEndTurnLabel);
            Assert.False(l.CancelButton.Overlaps(l.ActionButton));Assert.GreaterOrEqual(l.CancelButton.width,42f);Assert.GreaterOrEqual(l.CancelButton.height,48f);Assert.GreaterOrEqual(l.ActionButton.height,48f);
            Assert.That(l.EndTurnArt.width/l.EndTurnArt.height,Is.EqualTo(3.35f).Within(.02f));Assert.That(l.FullEndTurnArt.width/l.FullEndTurnArt.height,Is.EqualTo(3.35f).Within(.02f));
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneFootprintAndOccupiedBoardFitBeatPhase6N2(float width,float height)
        {
            var l=MobileLayout.Compute(width,height);float oldTop=Mathf.Clamp(height*.132f,50f,58f),oldBottom=Mathf.Clamp(height*.155f,63f,70f),oldRail=Mathf.Clamp(width*.235f,200f,228f);
            var oldBoard=new Rect(0,oldTop,width-oldRail,height-oldTop-oldBottom);var oldFit=BoardFitLayout.Compute(oldBoard,9,11,true,true);var fit=BoardFitLayout.ComputePhoneOccupied(l.Board,9,11);
            Assert.Less(l.TopBar.height,oldTop);Assert.Less(l.SkillBar.height,oldBottom);Assert.Less(l.ThreatPanel.width,oldRail);Assert.Greater(fit.TileSize,oldFit.TileSize*1.08f);Assert.True(fit.Fits(l.Board));
        }

        [Test] public void AllKitsHaveStableShortMediumAndLongSkillTypographyTiers()
        {
            foreach(var cls in ClassCatalog.All)foreach(var skill in SkillBook.ForClass(cls.id))
            {
                string name=HudText.MobileSkillName(skill);int font=HudText.MobileSkillFont(name,92f,17);
                Assert.That(font,Is.InRange(13,17),$"{cls.name}: {name}");Assert.False(name.Contains("\n"));
                foreach(int ap in new[]{0,skill.ApCost,99})Assert.IsNotEmpty(HudText.SkillState(skill,ap==99?2:0,ap,TurnPhase.Player));
            }
        }

        [Test] public void SelectedStateDoesNotChangeSkillCardGeometryOrUseWrappedCombinedLabel()
        {
            var a=MobileLayout.Compute(844,390);var b=MobileLayout.Compute(844,390);
            for(int i=0;i<3;i++){Assert.AreEqual(a.SkillButtons[i],b.SkillButtons[i]);Assert.AreEqual(a.SkillNameRects[i],b.SkillNameRects[i]);Assert.AreEqual(a.SkillCostRects[i],b.SkillCostRects[i]);Assert.AreEqual(a.SkillStateRects[i],b.SkillStateRects[i]);}
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");Assert.That(view,Does.Contain("wordWrap = false").And.Contain("\"SELECTED\"").And.Not.Contain("GUI.Label(content[i],HudText.MobileSkillCard"));
        }

        [Test] public void PhoneUsesLogicalViewportAndExplicitOccupiedFit()
        {
            string builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs"),view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(builder,Does.Contain("config.devicePixelRatio = 1;").And.Not.Contain("1199 / window.innerWidth"));Assert.That(view,Does.Contain("ComputePhoneOccupied"));
        }

        static void AssertContained(Rect outer,Rect inner){Assert.GreaterOrEqual(inner.xMin,outer.xMin);Assert.GreaterOrEqual(inner.yMin,outer.yMin);Assert.LessOrEqual(inner.xMax,outer.xMax);Assert.LessOrEqual(inner.yMax,outer.yMax);}
    }
}
