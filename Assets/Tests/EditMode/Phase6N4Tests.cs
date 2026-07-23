using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6N4Tests
    {
        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneCombatFootprintReturnsMoreAreaToOccupiedBoard(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);
            float oldTop=Mathf.Clamp(height*.116f,42f,46f),oldBottom=Mathf.Clamp(height*.132f,52f,56f),oldRail=Mathf.Clamp(width*.205f,174f,194f);
            var oldArea=new Rect(0f,oldTop,width-oldRail,height-oldTop-oldBottom);
            var oldFit=BoardFitLayout.ComputePhoneOccupied(oldArea,9,11,.11f,.025f);
            var fit=BoardFitLayout.ComputePhoneOccupied(layout.Board,9,11);
            Assert.True(layout.PhoneLandscape);Assert.Greater(layout.Board.width*layout.Board.height,oldArea.width*oldArea.height*1.06f);
            Assert.Greater(fit.TileSize,oldFit.TileSize*1.045f);Assert.True(fit.Fits(layout.Board));
            Assert.Less(layout.TopBar.height,oldTop);Assert.Less(layout.SkillBar.height,oldBottom);Assert.Less(layout.ThreatPanel.width,oldRail);
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhonePremiumActionGroupUsesSkillFrameGeometryAndAppleTouchTargets(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);
            AssertContained(layout.ActionButton,layout.EndTurnArt);Assert.AreEqual(layout.ActionButton,layout.FullActionButton);Assert.AreEqual(layout.EndTurnArt,layout.FullEndTurnArt);
            AssertContained(layout.EndTurnArt,layout.EndTurnLabel);AssertContained(layout.FullEndTurnArt,layout.FullEndTurnLabel);
            Assert.False(layout.CancelButton.Overlaps(layout.ActionButton));Assert.GreaterOrEqual(layout.CancelButton.width,44f);Assert.GreaterOrEqual(layout.CancelButton.height,44f);
            Assert.GreaterOrEqual(layout.ActionButton.width,44f);Assert.GreaterOrEqual(layout.ActionButton.height,44f);
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(view,Does.Contain("DrawCardFrame(layout.CancelArt").And.Contain("DrawCardFrame(actionArt").And.Contain("UiSkin.SkillCard"));
        }

        [TestCase(844f,390f)] [TestCase(932f,430f)]
        public void PhoneRewardModalContainsHeaderCardsUtilitiesAndSafeStatus(float width,float height)
        {
            var layout=MobileLayout.Compute(width,height);
            AssertContained(new Rect(0,0,width,height),layout.ModalPanel);AssertContained(layout.ModalPanel,layout.ModalHeaderRect);AssertContained(layout.ModalPanel,layout.ModalSubtitleRect);
            Assert.AreEqual(3,layout.ModalCards.Length);Assert.AreEqual(3,layout.ModalCardContentRects.Length);
            for(int i=0;i<3;i++){AssertContained(layout.ModalPanel,layout.ModalCards[i]);AssertContained(layout.ModalCards[i],layout.ModalCardContentRects[i]);Assert.GreaterOrEqual(layout.ModalCards[i].height,44f);}
            AssertNoOverlap(layout.ModalCards);AssertContained(layout.ModalPanel,layout.ModalHelpButton);AssertContained(layout.ModalPanel,layout.ModalInfoButton);AssertContained(layout.ModalPanel,layout.ModalPrimaryButton);
            Assert.GreaterOrEqual(layout.ModalHelpButton.width,44f);Assert.GreaterOrEqual(layout.ModalHelpButton.height,44f);Assert.GreaterOrEqual(layout.ModalInfoButton.width,44f);Assert.GreaterOrEqual(layout.ModalPrimaryButton.height,44f);
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");Assert.That(view,Does.Contain("ROOM CLEAR").And.Contain("CHOOSE ONE REWARD").And.Contain("SAFE — CHOOSE A REWARD").And.Contain("option.Detail"));
        }

        [Test] public void PhoneSkillGeometryRemainsSelectionInvariantAtEveryTargetSize()
        {
            foreach(var size in new[]{new Vector2(844,390),new Vector2(932,430)})
            {
                var idle=MobileLayout.Compute(size.x,size.y);var selected=MobileLayout.Compute(size.x,size.y);
                Assert.That(idle.SkillButtons,Is.EqualTo(selected.SkillButtons));Assert.That(idle.SkillNameRects,Is.EqualTo(selected.SkillNameRects));Assert.That(idle.SkillCostRects,Is.EqualTo(selected.SkillCostRects));Assert.That(idle.SkillStateRects,Is.EqualTo(selected.SkillStateRects));
                Assert.True(idle.SkillButtons.All(r=>r.height>=44f&&r.width>=44f));
            }
        }

        static void AssertContained(Rect outer,Rect inner){Assert.GreaterOrEqual(inner.xMin,outer.xMin);Assert.GreaterOrEqual(inner.yMin,outer.yMin);Assert.LessOrEqual(inner.xMax,outer.xMax);Assert.LessOrEqual(inner.yMax,outer.yMax);}
        static void AssertNoOverlap(Rect[] rects){for(int i=0;i<rects.Length;i++)for(int j=i+1;j<rects.Length;j++)Assert.False(rects[i].Overlaps(rects[j]));}
    }
}
