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

        static void ConfigureMobileFriendlyDefaults()
        {
            PlayerSettings.productName="Lanternfall Tactics Prototype";
            PlayerSettings.companyName="Lanternfall";
            PlayerSettings.bundleVersion="0.4.0";
            PlayerSettings.colorSpace=ColorSpace.Gamma;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.lanternfall.tactics.prototype");
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
        }
    }
}
