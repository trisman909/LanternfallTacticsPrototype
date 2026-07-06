using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Lanternfall.EditorTools
{
    public static class BuildPrototype
    {
        public static void BuildWindows()
        {
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
    }
}
