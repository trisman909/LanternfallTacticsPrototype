using System.IO;
using NUnit.Framework;

namespace Lanternfall.Tests
{
    public sealed class PwaComplianceTests
    {
        [Test] public void ManifestDefinesStableScopedFullscreenApplication()
        {
            string manifest=File.ReadAllText("PWA/manifest.webmanifest");
            Assert.That(manifest,Does.Contain("\"id\": \"./\"").And.Contain("\"start_url\": \"./?source=pwa\"").And.Contain("\"scope\": \"./\""));
            Assert.That(manifest,Does.Contain("\"display\": \"fullscreen\"").And.Contain("\"display_override\"").And.Contain("\"orientation\": \"landscape\""));
            Assert.That(manifest,Does.Contain("\"theme_color\": \"#b9832d\"").And.Contain("\"background_color\": \"#05040b\""));
            Assert.AreEqual(manifest,File.ReadAllText("docs/manifest.webmanifest"));
        }

        [Test] public void ManifestProvidesAnyAndMaskablePngIconsAtRequiredSizes()
        {
            string manifest=File.ReadAllText("PWA/manifest.webmanifest");
            Assert.That(manifest,Does.Contain("192x192").And.Contain("512x512").And.Contain("\"purpose\": \"any\"").And.Contain("\"purpose\": \"maskable\""));
            AssertPng("docs/icons/icon-192.png",192);AssertPng("docs/icons/icon-512.png",512);AssertPng("docs/icons/icon-maskable-192.png",192);AssertPng("docs/icons/icon-maskable-512.png",512);AssertPng("docs/icons/apple-touch-icon.png",180);
        }

        [Test] public void ServiceWorkerOwnsScopeAndProvidesOfflineNavigationAndUnityAssetCaching()
        {
            string worker=File.ReadAllText("docs/service-worker.js");
            Assert.That(worker,Does.Contain("addEventListener(\"install\"").And.Contain("addEventListener(\"activate\"").And.Contain("addEventListener(\"fetch\""));
            Assert.That(worker,Does.Contain("request.mode === \"navigate\"").And.Contain("caches.match(\"./index.html\")").And.Contain("/Build/").And.Contain("ignoreSearch: true"));
            Assert.AreEqual(File.ReadAllText("PWA/service-worker.js"),worker);
        }

        [Test] public void WebShellRegistersWorkerAndSupportsIosHomeScreenMetadata()
        {
            string html=File.ReadAllText("docs/index.html");
            Assert.That(html,Does.Contain("rel=\"manifest\" href=\"manifest.webmanifest\"").And.Contain("navigator.serviceWorker.register('./service-worker.js', { scope: './' })"));
            Assert.That(html,Does.Contain("apple-mobile-web-app-capable").And.Contain("apple-mobile-web-app-title").And.Contain("apple-touch-icon"));
        }

        [Test] public void BuildPipelinePreservesPwaAssetsForFutureWebglBuilds()
        {
            string builder=File.ReadAllText("Assets/Editor/BuildPrototype.cs");
            Assert.That(builder,Does.Contain("CopyPwaAssets").And.Contain("manifest.webmanifest").And.Contain("service-worker.js").And.Contain("apple-touch-icon.png"));
            Assert.That(builder,Does.Contain("serviceWorker.register('./service-worker.js', { scope: './' })"));
        }

        static void AssertPng(string path,int expected)
        {
            byte[] bytes=File.ReadAllBytes(path);Assert.Greater(bytes.Length,24,path);
            Assert.AreEqual(0x89,bytes[0],path);Assert.AreEqual((byte)'P',bytes[1],path);Assert.AreEqual((byte)'N',bytes[2],path);Assert.AreEqual((byte)'G',bytes[3],path);
            int width=ReadBigEndian(bytes,16),height=ReadBigEndian(bytes,20);Assert.AreEqual(expected,width,path);Assert.AreEqual(expected,height,path);
        }

        static int ReadBigEndian(byte[] bytes,int offset)=>bytes[offset]<<24|bytes[offset+1]<<16|bytes[offset+2]<<8|bytes[offset+3];
    }
}
