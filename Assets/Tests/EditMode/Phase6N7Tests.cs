using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public sealed class Phase6N7Tests
    {
        static readonly Vector2[] Phones={new(780,250),new(780,360),new(844,390),new(896,414),new(932,430)};

        [Test]
        public void SkillFramesAndTextRegionsStayInsideFixedCardsAtEveryPhoneSize()
        {
            foreach(var size in Phones)
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                for(int i=0;i<3;i++)
                {
                    Contained(layout.SkillBar,layout.SkillButtons[i],size);
                    Contained(layout.SkillButtons[i],layout.SkillContentRects[i],size);
                    Contained(layout.SkillContentRects[i],layout.SkillNameRects[i],size);
                    Contained(layout.SkillContentRects[i],layout.SkillCostRects[i],size);
                    Contained(layout.SkillContentRects[i],layout.SkillStateRects[i],size);
                    Assert.False(layout.SkillNameRects[i].Overlaps(layout.SkillCostRects[i]),size.ToString());
                }
            }
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(view,Does.Contain("DrawOutline(MobileLayout.Inset(r,1f,1f)").And.Not.Contain("r.x - 2"));
        }

        [Test]
        public void TargetingNeverChangesSkillOrActionGeometry()
        {
            foreach(var size in Phones)
            {
                var idle=MobileLayout.Compute(size.x,size.y);var targeting=MobileLayout.Compute(size.x,size.y);
                Assert.That(targeting.SkillButtons,Is.EqualTo(idle.SkillButtons),size.ToString());
                Assert.That(targeting.SkillNameRects,Is.EqualTo(idle.SkillNameRects),size.ToString());
                Assert.That(targeting.SkillCostRects,Is.EqualTo(idle.SkillCostRects),size.ToString());
                Assert.That(targeting.SkillStateRects,Is.EqualTo(idle.SkillStateRects),size.ToString());
                Assert.AreEqual(idle.ActionButton,targeting.ActionButton,size.ToString());
                Assert.AreEqual(idle.ActionButton,idle.FullActionButton,size.ToString());
            }
        }

        [Test]
        public void CancelAndEndTurnUseContainedNonOverlappingTouchArtAndLabelRects()
        {
            foreach(var size in Phones)
            {
                var layout=MobileLayout.Compute(size.x,size.y);var viewport=new Rect(0,0,size.x,size.y);
                Contained(viewport,layout.CancelButton,size);Contained(layout.CancelButton,layout.CancelArt,size);Contained(layout.CancelArt,layout.CancelLabel,size);
                Contained(viewport,layout.ActionButton,size);Contained(layout.ActionButton,layout.EndTurnArt,size);Contained(layout.EndTurnArt,layout.EndTurnLabel,size);
                Assert.False(layout.CancelButton.Overlaps(layout.ActionButton),size.ToString());
                Assert.GreaterOrEqual(layout.CancelButton.width,44f,size.ToString());Assert.GreaterOrEqual(layout.CancelButton.height,44f,size.ToString());
                Assert.GreaterOrEqual(layout.ActionButton.width,44f,size.ToString());Assert.GreaterOrEqual(layout.ActionButton.height,44f,size.ToString());
                Assert.That(layout.EndTurnArt.width/layout.EndTurnArt.height,Is.EqualTo(2.8f).Within(.02f),size.ToString());
            }
        }

        [Test]
        public void BottomRowsNeverExceedTheirAllocatedWidthsFromRounding()
        {
            foreach(var size in Phones)
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                Assert.GreaterOrEqual(layout.SkillButtons[0].xMin,layout.SkillBar.xMin,size.ToString());
                Assert.LessOrEqual(layout.SkillButtons[2].xMax,layout.SkillBar.xMax,size.ToString());
                for(int i=0;i<2;i++)Assert.LessOrEqual(layout.SkillButtons[i].xMax,layout.SkillButtons[i+1].xMin,size.ToString());
                Assert.LessOrEqual(layout.ActionButton.xMax,size.x,size.ToString());
            }
        }

        [Test]
        public void RewardModalUsesFixedContainedRegionsForLongestCurrentCopy()
        {
            foreach(var size in Phones.Where(s=>s.y>=250))
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                for(int i=0;i<3;i++)
                {
                    Contained(layout.ModalPanel,layout.ModalCards[i],size);Contained(layout.ModalCards[i],layout.ModalCardContentRects[i],size);
                    Contained(layout.ModalCardContentRects[i],layout.ModalCardNameRects[i],size);Contained(layout.ModalCardContentRects[i],layout.ModalCardEffectRects[i],size);Contained(layout.ModalCardContentRects[i],layout.ModalCardDetailRects[i],size);
                    Assert.False(layout.ModalCardNameRects[i].Overlaps(layout.ModalCardEffectRects[i]),size.ToString());
                    Assert.False(layout.ModalCardEffectRects[i].Overlaps(layout.ModalCardDetailRects[i]),size.ToString());
                }
                Assert.True(RewardCatalog.LabelsReadable,size.ToString());
                Assert.That(RewardCatalog.All.Max(r=>r.Name.Length),Is.LessThanOrEqualTo(14));
                Assert.That(RewardCatalog.All.Max(r=>r.Detail.Length),Is.LessThanOrEqualTo(24));
            }
        }

        [Test]
        public void XiaomiBoardSizingAndDesktopLayoutRemainUnchanged()
        {
            foreach(var size in new[]{new Vector2(780,250),new Vector2(780,360),new Vector2(844,390),new Vector2(932,430)})
            {
                var layout=MobileLayout.Compute(size.x,size.y);var fit=BoardFitLayout.ComputePhoneOccupied(layout.Board,9,11);
                Assert.True(layout.PhoneLandscape,size.ToString());Assert.True(fit.Fits(layout.Board),size.ToString());
            }
            var desktop=MobileLayout.Compute(1366,768);Assert.AreEqual(MobileLayoutMode.Desktop,desktop.Mode);Assert.False(desktop.PhoneHud);
        }

        static void Contained(Rect outer,Rect inner,Vector2 context)
        {
            Assert.GreaterOrEqual(inner.xMin,outer.xMin-.01f,context.ToString());Assert.GreaterOrEqual(inner.yMin,outer.yMin-.01f,context.ToString());
            Assert.LessOrEqual(inner.xMax,outer.xMax+.01f,context.ToString());Assert.LessOrEqual(inner.yMax,outer.yMax+.01f,context.ToString());
        }
    }
}
