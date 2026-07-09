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

        static void ConfigureMobileFriendlyDefaults()
        {
            PlayerSettings.productName="Lanternfall Tactics Prototype";
            PlayerSettings.companyName="Lanternfall";
            PlayerSettings.bundleVersion="0.4.5";
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
    }
}
