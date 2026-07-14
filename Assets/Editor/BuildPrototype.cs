using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEngine;

namespace Lanternfall.EditorTools
{
    public static class BuildPrototype
    {
        public static void BuildWindows()
        {
            ConfigureMobileFriendlyDefaults();
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
            Directory.CreateDirectory("Builds/Windows");
            PlayerSettings.productName = "Lanternfall Tactics Prototype";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            var report = BuildPipeline.BuildPlayer(new[] { "Assets/Scenes/Main.unity" }, "Builds/Windows/LanternfallTactics.exe", BuildTarget.StandaloneWindows64, BuildOptions.Development);
            if (report.summary.result != BuildResult.Succeeded) throw new System.Exception("Windows build failed: " + report.summary.result);
            Debug.Log("WINDOWS_BUILD_OK " + report.summary.totalSize);
        }

        public static void BuildAndroid()
        {
            ConfigureMobileFriendlyDefaults();
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Main.unity");
            Directory.CreateDirectory("Builds/Android");
            var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/Main.unity"},"Builds/Android/LanternfallTactics.apk",BuildTarget.Android,BuildOptions.Development);
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android build failed: "+report.summary.result);
            Debug.Log("ANDROID_BUILD_OK "+report.summary.totalSize);
        }

        public static void ExportIOS()
        {
            ConfigureMobileFriendlyDefaults();
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Main.unity");
            Directory.CreateDirectory("Builds/iOS");
            var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/Main.unity"},"Builds/iOS",BuildTarget.iOS,BuildOptions.Development);
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("iOS Xcode export failed: "+report.summary.result);
            Debug.Log("IOS_EXPORT_OK "+report.summary.totalSize);
        }

        public static void BuildWebGL()
        {
            ConfigureMobileFriendlyDefaults();
            ConfigureWebGLDefaults();
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Main.unity");
            Directory.CreateDirectory("Builds/WebGL/LanternfallTactics");
            var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/Main.unity"},"Builds/WebGL/LanternfallTactics",BuildTarget.WebGL,BuildOptions.None);
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("WebGL build failed: "+report.summary.result);
            PatchWebGLForResponsivePreview("Builds/WebGL/LanternfallTactics");
            Debug.Log("WEBGL_BUILD_OK "+report.summary.totalSize);
        }

        static void ConfigureMobileFriendlyDefaults()
        {
            PlayerSettings.productName="Lanternfall Tactics Prototype";
            PlayerSettings.companyName="Lanternfall";
            PlayerSettings.bundleVersion="0.5.17";
            PlayerSettings.colorSpace=ColorSpace.Gamma;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait=true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
            PlayerSettings.allowedAutorotateToLandscapeLeft=true;
            PlayerSettings.allowedAutorotateToLandscapeRight=true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.lanternfall.tactics.prototype");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS,"com.yourstudio.lanternfalltactics");
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.iOS.buildNumber="1";
            PlayerSettings.iOS.targetDevice=iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.targetOSVersionString="15.0";
        }

        static void ConfigureWebGLDefaults()
        {
            PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback=false;
            PlayerSettings.WebGL.dataCaching=true;
            PlayerSettings.WebGL.exceptionSupport=WebGLExceptionSupport.None;
            PlayerSettings.WebGL.memorySize=128;
            PlayerSettings.WebGL.threadsSupport=false;
            PlayerSettings.stripEngineCode=true;
        }

        static void PatchWebGLForResponsivePreview(string path)
        {
            var index=Path.Combine(path,"index.html");
            if(File.Exists(index))
            {
                var html=File.ReadAllText(index);
                if(!html.Contains("Cache-Control"))
                    html=html.Replace("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">","<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n    <meta http-equiv=\"Cache-Control\" content=\"no-cache, no-store, must-revalidate\">\n    <meta http-equiv=\"Pragma\" content=\"no-cache\">\n    <meta http-equiv=\"Expires\" content=\"0\">");
                html=html.Replace("var loaderUrl = buildUrl + \"/LanternfallTactics.loader.js\";","var cacheBust = \"v=5Q3\";\n      var loaderUrl = buildUrl + \"/LanternfallTactics.loader.js?\" + cacheBust;");
                html=html.Replace("dataUrl: buildUrl + \"/LanternfallTactics.data\",","dataUrl: buildUrl + \"/LanternfallTactics.data?\" + cacheBust,");
                html=html.Replace("frameworkUrl: buildUrl + \"/LanternfallTactics.framework.js\",","frameworkUrl: buildUrl + \"/LanternfallTactics.framework.js?\" + cacheBust,");
                html=html.Replace("codeUrl: buildUrl + \"/LanternfallTactics.wasm\",","codeUrl: buildUrl + \"/LanternfallTactics.wasm?\" + cacheBust,");
                html=html.Replace("canvas.style.width = \"960px\";","canvas.style.width = \"100vw\";");
                html=html.Replace("canvas.style.height = \"600px\";","canvas.style.height = \"100dvh\";");
                html=html.Replace("height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes","height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes, viewport-fit=cover");
                html=html.Replace("<div id=\"unity-progress-bar-empty\">","<div id=\"lanternfall-loading-copy\">Loading Lanternfall Tactics - Prototype v0.5Q.3 landscape-first mobile HUD. First phone load may be slow; repeat loads can use browser caching. Rotate your phone to landscape; Add to Home Screen/fullscreen is best if available.</div>\n        <div id=\"unity-progress-bar-empty\">");
                File.WriteAllText(index,html);
            }
            var css=Path.Combine(path,"TemplateData","style.css");
            if(File.Exists(css))
            {
                var text=File.ReadAllText(css);
                text += "\nhtml, body { width: 100%; height: 100%; min-height: 100dvh; overflow: hidden; background: #000; position: fixed; inset: 0; touch-action: manipulation; }\n";
                text += "@supports (height: 100svh) { html, body, #unity-container.unity-desktop, #unity-container.unity-mobile, #unity-canvas { min-height: 100svh; } }\n";
                text += "#unity-container.unity-desktop, #unity-container.unity-mobile { position: fixed; left: 0; top: 0; transform: none; width: 100vw; height: 100dvh; padding: env(safe-area-inset-top) env(safe-area-inset-right) env(safe-area-inset-bottom) env(safe-area-inset-left); box-sizing: border-box; }\n";
                text += "#unity-canvas { width: 100vw !important; height: 100dvh !important; display: block; }\n";
                text += "#unity-footer { display: none; }\n";
                text += "#lanternfall-loading-copy { color: #f4d27a; font: 700 16px Arial, sans-serif; text-align: center; margin: 10px auto; max-width: 520px; line-height: 1.35; }\n";
                File.WriteAllText(css,text);
            }
        }
    }
}




