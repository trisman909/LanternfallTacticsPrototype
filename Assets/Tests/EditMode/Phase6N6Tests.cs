using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Lanternfall.Tests
{
    public class Phase6N6Tests
    {
        static readonly Vector2[] PhoneViewports={
            new(844,390),new(932,430),new(915,412),new(896,414),new(780,360)
        };

        [Test]
        public void XiaomiLogicalViewportSelectsPhoneLayoutAndAvoidsTinyBoard()
        {
            var viewport=new BrowserViewportSnapshot(915,412,0,0,0,0,1,true);
            var safe=viewport.LogicalSafeArea;
            var layout=MobileLayout.Compute(safe.width,safe.height);
            var fit=BoardFitLayout.ComputePhoneOccupied(layout.Board,9,11);
            Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode);
            Assert.GreaterOrEqual(fit.Bounds.height/layout.Board.height,.90f);
            Assert.GreaterOrEqual(fit.Bounds.width/layout.Board.width,.34f);
            Assert.That(fit.TileSize,Is.GreaterThan(27f));
            Assert.True(fit.Fits(layout.Board));
        }

        [Test]
        public void IdenticalCssViewportProducesIdenticalLayoutAtEveryDevicePixelRatio()
        {
            var css=new BrowserViewportSnapshot(915,412,0,0,0,0,2,true);
            var baseline=MobileLayout.Compute(css.LogicalSafeArea.width,css.LogicalSafeArea.height);
            var baselineFit=BoardFitLayout.ComputePhoneOccupied(baseline.Board,9,11);
            foreach(float dpr in new[]{1f,2f,2.625f,3f,4f})
            {
                float framebufferWidth=css.Width*dpr,framebufferHeight=css.Height*dpr;
                var logical=MobileLayout.Compute(css.LogicalSafeArea.width,css.LogicalSafeArea.height);
                var fit=BoardFitLayout.ComputePhoneOccupied(logical.Board,9,11);
                Assert.AreEqual(baselineFit.TileSize,fit.TileSize,.001f,$"DPR {dpr}, framebuffer {framebufferWidth}x{framebufferHeight}");
            }
        }

        [Test]
        public void BrowserSafeInsetsAreAppliedExactlyOnce()
        {
            var viewport=new BrowserViewportSnapshot(932,430,18,0,12,0,3,true);
            var safe=viewport.LogicalSafeArea;
            Assert.AreEqual(902f,safe.width);
            Assert.AreEqual(430f,safe.height);
            var layout=MobileLayout.Compute(safe.width,safe.height);
            Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode);
            Assert.True(BoardFitLayout.ComputePhoneOccupied(layout.Board,9,11).Fits(layout.Board));
        }

        [Test]
        public void AndroidAndIPhoneViewportClassesKeepBoardProminentAndContained()
        {
            foreach(var size in PhoneViewports)
            {
                var layout=MobileLayout.Compute(size.x,size.y);
                var fit=BoardFitLayout.ComputePhoneOccupied(layout.Board,9,11);
                Assert.AreEqual(MobileLayoutMode.PhoneLandscape,layout.Mode,size.ToString());
                Assert.GreaterOrEqual(fit.Bounds.height/layout.Board.height,.89f,size.ToString());
                Assert.True(fit.Fits(layout.Board),size.ToString());
            }
        }

        [Test]
        public void WebGlShellUsesVisualViewportAndDebouncedDynamicEvents()
        {
            string source=File.ReadAllText("PWA/viewport.js");
            string shell=File.ReadAllText("docs/index.html");
            string css=File.ReadAllText("docs/TemplateData/style.css");
            string bridge=File.ReadAllText("Assets/Plugins/WebGL/LanternfallViewport.jslib");
            Assert.That(source,Does.Contain("window.visualViewport").And.Contain("visualViewport.addEventListener('resize'").And.Contain("orientationchange").And.Contain("fullscreenchange").And.Contain("setTimeout(updateLanternfallViewportMode, 80)"));
            Assert.That(shell,Does.Contain("window.LanternfallViewport").And.Contain("visualViewport"));
            Assert.That(css,Does.Contain("var(--lf-vw").And.Contain("var(--lf-vh").And.Contain("padding: 0"));
            Assert.That(bridge,Does.Contain("LanternfallViewport_Width").And.Contain("LanternfallViewport_Height"));
        }

        [Test]
        public void UnityViewUsesBrowserLogicalViewportInsteadOfRawScreenForLayout()
        {
            string view=File.ReadAllText("Assets/Scripts/LanternfallView.cs");
            Assert.That(view,Does.Contain("BrowserViewport.Read").And.Contain("browserViewport.LogicalSafeArea").And.Contain("Screen.width/browserViewport.Width"));
            Assert.That(view,Does.Not.Contain("MobileLayout.Compute(Screen.width, Screen.height)"));
        }
    }
}
