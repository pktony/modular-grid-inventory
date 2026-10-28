using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Pktony.GridInventory.Editor
{
    public static class InventoryWebBuild
    {
        [MenuItem("Inventory/Build Web Demo")]
        public static void Build()
        {
            var settingsPath = Path.GetFullPath("ProjectSettings/ProjectSettings.asset");
            var originalSettings = File.ReadAllBytes(settingsPath);
            var output = Environment.GetEnvironmentVariable("INVENTORY_WEB_OUTPUT") ?? "Builds/WebGL/inventory";
            try
            {
                PlayerSettings.productName = "Modular Grid Inventory";
                PlayerSettings.WebGL.template = "PROJECT:Inventory";
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
                PlayerSettings.WebGL.decompressionFallback = true;
                PlayerSettings.WebGL.threadsSupport = false;
                PlayerSettings.WebGL.initialMemorySize = 64;
                PlayerSettings.WebGL.maximumMemorySize = 512;
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { "Assets/ModularGridInventory/Samples/Scenes/Inventory.unity" },
                    locationPathName = output,
                    target = BuildTarget.WebGL,
                    options = BuildOptions.None
                });
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException($"Web build failed: {report.summary.totalErrors} error(s).");
                File.WriteAllText(Path.Combine(output, "build-info.json"), JsonUtility.ToJson(new BuildInfo
                {
                    unityVersion = Application.unityVersion,
                    result = report.summary.result.ToString(),
                    totalBytes = report.summary.totalSize,
                    seconds = report.summary.totalTime.TotalSeconds
                }, true));
            }
            finally
            {
                File.WriteAllBytes(settingsPath, originalSettings);
            }
        }

        [Serializable]
        private sealed class BuildInfo
        {
            public string unityVersion;
            public string result;
            public ulong totalBytes;
            public double seconds;
        }
    }
}
