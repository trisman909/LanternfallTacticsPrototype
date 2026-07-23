using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6N2Tests
    {
        [TestCase(844f,390f)] [TestCase(852f,393f)] [TestCase(932f,430f)]
        public void PhoneBarsAreSlimmerAndBoardFitIsMateriallyLarger(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);
            float oldTop=Mathf.Clamp(height*.15f,56f,66f),oldBottom=Mathf.Clamp(height*.18f,68f,80f);
            float oldPanel=Mathf.Clamp(width*.255f,210f,250f);
            var oldArea=new Rect(0,oldTop,width-oldPanel,height-oldTop-oldBottom);
            var oldFit=BoardFitLayout.Compute(oldArea,9,11,true,false);
            var polishedFit=BoardFitLayout.Compute(layout.Board,9,11,true,true);
            Assert.True(layout.PhoneLandscape);Assert.Less(layout.TopBar.height,oldTop*.91f);Assert.Less(layout.SkillBar.height,oldBottom*.91f);
            Assert.Greater(layout.Board.height,oldArea.height);Assert.Greater(polishedFit.TileSize,oldFit.TileSize*1.15f);Assert.True(polishedFit.Fits(layout.Board));
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneControlsHaveContainedVisualAndTextSafeAreas(float width,float height)
        {
            var l=MobileLayout.Compute(width,height);
            Assert.AreEqual(3,l.StatChips.Length);Assert.AreEqual(3,l.SkillContentRects.Length);
            Assert.True(l.StatChips.All(r=>l.TopBar.Contains(r.min)&&l.TopBar.Contains(r.max)));
            Assert.True(l.StatChips.All(r=>Mathf.Abs(r.height-l.StatChips[0].height)<.01f&&Mathf.Abs(r.y-l.StatChips[0].y)<.01f));
            for(int i=0;i<3;i++){Assert.True(l.SkillButtons[i].Contains(l.SkillContentRects[i].min));Assert.True(l.SkillButtons[i].Contains(l.SkillContentRects[i].max));Assert.GreaterOrEqual(l.SkillButtons[i].height,44f);}
            AssertContained(l.ActionButton,l.EndTurnArt);AssertContained(l.EndTurnArt,l.EndTurnLabel);
            Assert.Greater(l.EndTurnArt.width/l.EndTurnArt.height,l.ActionButton.width/l.ActionButton.height);
            Assert.True(l.ThreatPanel.xMin>=l.Board.xMax&&l.ThreatPanel.xMax<=width);
            Assert.AreEqual(l.FullActionButton,l.ActionButton);AssertContained(l.FullActionButton,l.FullEndTurnArt);
        }

        [Test] public void PhoneBoardIsExplicitlyHeaderlessAndDesktopContractIsUnchanged()
        {
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");string builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(view,Does.Contain("DrawBoard(layout.Board, layout.Portrait || layout.CompactLandscape, layout.PhoneLandscape)"));
            Assert.That(view,Does.Contain("if(!phoneBoard)"));Assert.That(builder,Does.Contain("config.devicePixelRatio = 1;").And.Not.Contain("1199 / window.innerWidth"));
            var desktop=MobileLayout.Compute(1920,1080);Assert.False(desktop.PhoneLandscape);Assert.AreEqual(0f,desktop.TopBar.width);Assert.AreEqual(desktop.Board.xMax,desktop.Panel.xMin,.01f);
        }

        [Test] public void SafeAreaDimensionsStillProduceContainedPhoneRegions()
        {
            var safe=MobileLayout.ToGuiSafeArea(430,new Rect(18,0,896,430));var l=MobileLayout.Compute(safe.width,safe.height);
            Assert.True(l.PhoneLandscape);Assert.True(l.TopBar.xMin>=0&&l.ThreatPanel.xMax<=safe.width);Assert.True(l.SkillBar.yMax<=safe.height&&l.ActionButton.yMax<=safe.height);
        }

        static void AssertContained(Rect outer,Rect inner){Assert.GreaterOrEqual(inner.xMin,outer.xMin);Assert.GreaterOrEqual(inner.yMin,outer.yMin);Assert.LessOrEqual(inner.xMax,outer.xMax);Assert.LessOrEqual(inner.yMax,outer.yMax);}
    }
}
