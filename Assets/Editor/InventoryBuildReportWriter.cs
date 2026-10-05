using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
namespace InventorySystem.Editor
{
    public sealed class InventoryBuildReportWriter : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPostprocessBuild(BuildReport report) => Write(report);
        [MenuItem("Inventory/Write Latest Build Report")]
        public static void WriteLatest() => Write(BuildReport.GetLatestReport());
        private static void Write(BuildReport report)
        {
            var summary = report.summary;
            Directory.CreateDirectory("TestResults");
            File.WriteAllText("TestResults/build.txt", $"{summary.result}\nErrors: {summary.totalErrors}\nWarnings: {summary.totalWarnings}\nBytes: {Directory.GetFiles("Build", "*", SearchOption.AllDirectories).Sum(path => new FileInfo(path).Length)}\nTarget: {summary.platform}\nUnity: {UnityEngine.Application.unityVersion}");
            File.WriteAllLines("TestResults/build-messages.txt", report.steps.SelectMany(step => step.messages)
                .Where(message => message.type != UnityEngine.LogType.Log)
                .Select(message => $"{message.type}: {message.content}"));
        }
    }
}
