using UnityEditor;
using UnityEditor.Build.Reporting;
namespace InventorySystem.Editor
{
    public static class InventoryBuild
    {
        [MenuItem("Inventory/Build Windows Demo")]
        public static void BuildWindows()
        {
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] {"Assets/Scenes/Inventory.unity"},
                locationPathName = "Build/TacticalInventory.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded) throw new System.InvalidOperationException("Windows build failed.");
        }
    }
}
