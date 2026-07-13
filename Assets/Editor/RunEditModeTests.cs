using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Lanternfall.EditorTools
{
    public static class RunEditModeTests
    {
        const string ResultPath = "TestResults_Phase5N.xml";

        public static void Run()
        {
            var api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new ExitWhenDone());
            api.Execute(new ExecutionSettings(new Filter { testMode = TestMode.EditMode }));
        }

        sealed class ExitWhenDone : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"EDITMODE_TESTS_STARTED {testsToRun.TestCaseCount}");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                TestRunnerApi.SaveResultToFile(result, ResultPath);
                Debug.Log($"EDITMODE_TESTS_FINISHED passed={result.PassCount} failed={result.FailCount} skipped={result.SkipCount} inconclusive={result.InconclusiveCount}");
                EditorApplication.Exit(result.FailCount == 0 && result.InconclusiveCount == 0 ? 0 : 1);
            }

            public void TestStarted(ITestAdaptor test) { }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result.TestStatus.ToString() == "Failed")
                    Debug.LogError($"{result.FullName}: {result.Message}\n{result.StackTrace}");
            }
        }
    }
}
