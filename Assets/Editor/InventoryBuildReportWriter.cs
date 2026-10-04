using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
namespace InventorySystem.Editor
{
    public sealed class InventoryBuildReportWriter : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPostprocessBuild(BuildReport report)
        {
            var summary = report.summary;
            Directory.CreateDirectory("TestResults");
            File.WriteAllText("TestResults/build.txt", $"{summary.result}\nErrors: {summary.totalErrors}\nWarnings: {summary.totalWarnings}");
        }
    }
}
