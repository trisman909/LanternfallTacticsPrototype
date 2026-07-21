using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEngine;
using Process=System.Diagnostics.Process;
using ProcessStartInfo=System.Diagnostics.ProcessStartInfo;

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
            BuildWebGLInternal(BuildOptions.None);
        }

        public static void BuildWebGLFull()
        {
            BuildWebGLInternal(BuildOptions.CleanBuildCache);
        }

        static void BuildWebGLInternal(BuildOptions options)
        {
            ConfigureMobileFriendlyDefaults();
            ConfigureWebGLDefaults();
            Directory.CreateDirectory("Assets/Scenes");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Main.unity");
            Directory.CreateDirectory("Builds/WebGL/LanternfallTactics");
            var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/Main.unity"},"Builds/WebGL/LanternfallTactics",BuildTarget.WebGL,options);
            if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("WebGL build failed: "+report.summary.result);
            PatchWebGLForResponsivePreview("Builds/WebGL/LanternfallTactics");
            Debug.Log("WEBGL_BUILD_OK "+report.summary.totalSize);
        }

        static void ConfigureMobileFriendlyDefaults()
        {
            PlayerSettings.productName="Lanternfall Tactics Prototype";
            PlayerSettings.companyName="Lanternfall";
            PlayerSettings.bundleVersion="0.6N.1+"+GitShortHash();
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

        static string GitShortHash()
        {
            try
            {
                var start=new ProcessStartInfo("git","rev-parse --short=8 HEAD"){UseShellExecute=false,RedirectStandardOutput=true,CreateNoWindow=true};
                using var process=Process.Start(start);string hash=process.StandardOutput.ReadToEnd().Trim();process.WaitForExit();
                return process.ExitCode==0&&hash.Length>=7?hash:"unknown";
            }
            catch{return "unknown";}
        }

        static void PatchWebGLForResponsivePreview(string path)
        {
            var index=Path.Combine(path,"index.html");
            if(File.Exists(index))
            {
                var html=File.ReadAllText(index);
                if(!html.Contains("Cache-Control"))
                    html=html.Replace("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">","<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n    <meta http-equiv=\"Cache-Control\" content=\"no-cache, no-store, must-revalidate\">\n    <meta http-equiv=\"Pragma\" content=\"no-cache\">\n    <meta http-equiv=\"Expires\" content=\"0\">");
                html=html.Replace("<title>Unity Web Player | Lanternfall Tactics Prototype</title>","<title>Lanternfall Tactics</title>\n    <meta name=\"description\" content=\"A compact dark-fantasy turn-based tactics prototype.\">\n    <meta name=\"theme-color\" content=\"#b9832d\">\n    <meta name=\"apple-mobile-web-app-capable\" content=\"yes\">\n    <meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black-translucent\">\n    <link rel=\"manifest\" href=\"manifest.webmanifest\">");
                html=html.Replace("href=\"TemplateData/style.css\"","href=\"TemplateData/style.css?v=6N1LHUD\"");
                html=html.Replace("var loaderUrl = buildUrl + \"/LanternfallTactics.loader.js\";","var cacheBust = \"v=6N1LHUD\";\n      var loaderUrl = buildUrl + \"/LanternfallTactics.loader.js?\" + cacheBust;");
                html=html.Replace("dataUrl: buildUrl + \"/LanternfallTactics.data\",","dataUrl: buildUrl + \"/LanternfallTactics.data?\" + cacheBust,");
                html=html.Replace("frameworkUrl: buildUrl + \"/LanternfallTactics.framework.js\",","frameworkUrl: buildUrl + \"/LanternfallTactics.framework.js?\" + cacheBust,");
                html=html.Replace("codeUrl: buildUrl + \"/LanternfallTactics.wasm\",","codeUrl: buildUrl + \"/LanternfallTactics.wasm?\" + cacheBust,");
                html=html.Replace("// config.devicePixelRatio = 1;","config.devicePixelRatio = 1;");
                html=html.Replace("      // By default, Unity keeps WebGL canvas render target size matched with", "      config.devicePixelRatio = (window.innerWidth <= 1200 && window.innerHeight <= 620) ? (1199 / window.innerWidth) : 1;\n\n      // By default, Unity keeps WebGL canvas render target size matched with");
                html=html.Replace("canvas.style.width = \"960px\";","canvas.style.width = \"100%\";");
                html=html.Replace("canvas.style.height = \"600px\";","canvas.style.height = \"100%\";");
                html=html.Replace("height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes","height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes, viewport-fit=cover");
                html=html.Replace("<body>","<body>\n    <div id=\"lanternfall-build-proof\">Phase 6N.1 — L HUD</div>\n    <div id=\"lanternfall-rotate-overlay\" aria-live=\"polite\">\n      <div class=\"lanternfall-rotate-card\">\n        <div class=\"lanternfall-rotate-title\">Rotate your phone to play</div>\n        <div class=\"lanternfall-rotate-body\">Lanternfall Tactics is best played in landscape.</div>\n        <div class=\"lanternfall-rotate-note\">Add to Home Screen for more space.</div>\n      </div>\n    </div>");
                html=html.Replace("<div id=\"unity-progress-bar-empty\">","<div id=\"lanternfall-loading-copy\">Loading Lanternfall Tactics - Prototype v0.6N.1 mobile-HUD build. Audio unlocks after your first tap. Rotate your phone to landscape; Add to Home Screen/fullscreen is best if available.</div>\n        <div id=\"unity-progress-bar-empty\">");
                html=html.Replace("</body>","    <script>\n      (function () {\n        function updateLanternfallViewportMode() {\n          var w = Math.max(1, window.innerWidth || document.documentElement.clientWidth || screen.width || 1);\n          var h = Math.max(1, window.innerHeight || document.documentElement.clientHeight || screen.height || 1);\n          var coarse = window.matchMedia && window.matchMedia('(pointer: coarse)').matches;\n          var touch = (navigator.maxTouchPoints || 0) > 0 || 'ontouchstart' in window;\n          var mobileUA = /Android|iPhone|iPod|Mobile|Windows Phone/i.test(navigator.userAgent || '');\n          var likelyPhone = (coarse || touch || mobileUA) && Math.min(w, h) <= 700 && Math.max(w, h) <= 1200;\n          var portraitPhone = likelyPhone && h > w;\n          var landscapePhone = likelyPhone && w > h;\n          document.body.classList.toggle('lanternfall-phone-portrait', portraitPhone);\n          document.body.classList.toggle('lanternfall-phone-landscape', landscapePhone);\n          document.body.classList.toggle('lanternfall-desktop', !portraitPhone && !landscapePhone);\n          document.documentElement.style.setProperty('--lf-vw', w + 'px');\n          document.documentElement.style.setProperty('--lf-vh', h + 'px');\n        }\n        updateLanternfallViewportMode();\n        window.addEventListener('resize', updateLanternfallViewportMode, { passive: true });\n        window.addEventListener('orientationchange', updateLanternfallViewportMode, { passive: true });\n        document.addEventListener('visibilitychange', updateLanternfallViewportMode);\n      })();\n    </script>\n  </body>");
                File.WriteAllText(index,html);
            }
            File.WriteAllText(Path.Combine(path,"manifest.webmanifest"),"{\n  \"name\": \"Lanternfall Tactics\",\n  \"short_name\": \"Lanternfall\",\n  \"description\": \"A compact dark-fantasy turn-based tactics prototype.\",\n  \"start_url\": \"./\",\n  \"scope\": \"./\",\n  \"display\": \"fullscreen\",\n  \"orientation\": \"landscape\",\n  \"background_color\": \"#05040b\",\n  \"theme_color\": \"#b9832d\",\n  \"icons\": [{\"src\": \"TemplateData/favicon.ico\", \"sizes\": \"any\", \"type\": \"image/x-icon\"}]\n}\n");
            var css=Path.Combine(path,"TemplateData","style.css");
            if(File.Exists(css))
            {
                var text=File.ReadAllText(css);
                text += "\nhtml, body { width: 100%; max-width: 100%; height: 100%; min-height: 100dvh; margin: 0; overflow: hidden; background: #000; position: fixed; inset: 0; touch-action: manipulation; overscroll-behavior: none; }\n";
                text += "@supports (height: 100svh) { html, body, #unity-container.unity-desktop, #unity-container.unity-mobile, #unity-canvas { min-height: 100svh; } }\n";
                text += "#unity-container.unity-desktop, #unity-container.unity-mobile { position: fixed; left: 0; top: 0; right: 0; bottom: 0; transform: none; width: 100%; max-width: 100%; height: 100dvh; max-height: 100dvh; padding: env(safe-area-inset-top) env(safe-area-inset-right) env(safe-area-inset-bottom) env(safe-area-inset-left); box-sizing: border-box; overflow: hidden; }\n";
                text += "body.lanternfall-phone-landscape #unity-container.unity-desktop, body.lanternfall-phone-landscape #unity-container.unity-mobile { height: 100dvh; width: 100%; max-width: 100%; padding-left: max(0px, env(safe-area-inset-left)); padding-right: max(0px, env(safe-area-inset-right)); }\n";
                text += "#unity-canvas { width: 100% !important; max-width: 100% !important; height: 100% !important; max-height: 100% !important; display: block; box-sizing: border-box; }\n";
                text += "#unity-footer { display: none; }\n";
                text += "#lanternfall-build-proof { position: fixed; z-index: 100000; left: max(6px, env(safe-area-inset-left)); top: max(50px, calc(env(safe-area-inset-top) + 46px)); padding: 3px 8px; border: 1px solid #b9832d; border-radius: 4px; background: rgba(0,0,0,.88); color: #ffd36f; font: 700 13px/16px Arial, sans-serif; pointer-events: none; }\n";
                text += "#lanternfall-loading-copy { color: #f4d27a; font: 700 16px Arial, sans-serif; text-align: center; margin: 10px auto; max-width: 520px; line-height: 1.35; }\n";
                text += "#unity-loading-bar { min-width: min(84vw, 520px); padding: 24px; border: 2px solid #8f6728; border-radius: 14px; background: radial-gradient(circle at 50% 0%, rgba(46,36,18,.92), rgba(8,6,16,.96) 62%); box-shadow: 0 18px 60px rgba(0,0,0,.62); box-sizing: border-box; }\n";
                text += "#unity-logo { width: 100% !important; height: 52px !important; background: none !important; position: relative; } #unity-logo::before { content: 'LANTERNFALL'; display: block; color: #ffd36f; font: 900 30px/38px Georgia, serif; letter-spacing: 3px; text-align: center; text-shadow: 0 2px 14px rgba(255,157,25,.24); } #unity-logo::after { content: 'CARRY THE LIGHT'; display: block; color: #c7d7e9; font: 700 10px/12px Arial, sans-serif; letter-spacing: 2px; text-align: center; }\n";
                text += "#unity-progress-bar-empty { border: 1px solid #785622; background: #100d18; } #unity-progress-bar-full { background: linear-gradient(90deg, #9b5f14, #ffd36f); }\n";
                text += "#lanternfall-rotate-overlay { display: none; position: fixed; inset: 0; z-index: 99999; min-height: 100dvh; padding: max(22px, env(safe-area-inset-top)) max(18px, env(safe-area-inset-right)) max(22px, env(safe-area-inset-bottom)) max(18px, env(safe-area-inset-left)); box-sizing: border-box; align-items: center; justify-content: center; background: radial-gradient(circle at 50% 18%, #142237 0%, #080611 55%, #020208 100%); color: #fff2ca; text-align: center; }\n";
                text += ".lanternfall-rotate-card { width: min(88vw, 440px); border: 3px solid #b9832d; border-radius: 18px; background: rgba(16, 13, 30, .94); box-shadow: 0 18px 60px rgba(0,0,0,.55); padding: 28px 22px; font-family: Arial, sans-serif; }\n";
                text += ".lanternfall-rotate-title { font-size: clamp(32px, 9vw, 48px); line-height: 1.05; font-weight: 900; color: #ffd36f; margin-bottom: 18px; }\n";
                text += ".lanternfall-rotate-body { font-size: clamp(22px, 5.8vw, 30px); line-height: 1.18; font-weight: 800; margin-bottom: 16px; }\n";
                text += ".lanternfall-rotate-note { font-size: clamp(17px, 4.5vw, 23px); line-height: 1.25; color: #d7e5ff; }\n";
                text += "body.lanternfall-phone-portrait #lanternfall-rotate-overlay { display: flex; }\n";
                text += "body.lanternfall-phone-portrait #unity-container, body.lanternfall-phone-portrait #unity-canvas { visibility: hidden !important; pointer-events: none !important; }\n";
                File.WriteAllText(css,text);
            }
        }
    }
}




