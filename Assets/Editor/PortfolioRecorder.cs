using System.IO;
using InventorySystem;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class PortfolioRecorder
    {
        private static ExpandedInventory inventory;
        private static InventoryWalkthroughScenario scenario;
        private static string output;
        private static int frame;
        private static InventoryScenarioVerifier verifier;
        private static double nextCapture;
        [MenuItem("Inventory/Record Walkthrough (Play Mode)")]
        public static void Begin()
        {
            if (!Application.isPlaying) throw new System.InvalidOperationException("Enter Play mode first.");
            Stop(); inventory = Object.FindAnyObjectByType<ExpandedInventory>(); inventory.enabled = false;
            scenario = new InventoryWalkthroughScenario(inventory); verifier = new InventoryScenarioVerifier();
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "../Recordings/frames")); Directory.CreateDirectory(output);
            foreach (string path in Directory.GetFiles(output, "frame-*.png")) File.Delete(path);
            string report = Path.GetFullPath(Path.Combine(Application.dataPath, "../TestResults/recording.txt"));
            if (File.Exists(report)) File.Delete(report);
            frame = 0; nextCapture = EditorApplication.timeSinceStartup;
            EditorApplication.update += Capture;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        private static void Capture()
        {
            if (EditorApplication.timeSinceStartup < nextCapture) return;
            if (frame > 0 && !File.Exists(Path.Combine(output, $"frame-{frame - 1:D4}.png"))) return;
            if (frame >= InventoryWalkthroughScenario.Duration * 30)
            {
                if (!File.Exists(Path.Combine(output, $"frame-{InventoryWalkthroughScenario.Duration * 30 - 1:D4}.png"))) return;
                string results = Path.GetFullPath(Path.Combine(Application.dataPath, "../TestResults"));
                Directory.CreateDirectory(results); File.WriteAllText(Path.Combine(results,"recording.txt"), $"{InventoryWalkthroughScenario.Duration * 30} frames / 30 fps / {InventoryWalkthroughScenario.Duration} seconds / Unity pointer events / click indicator");
                Stop(); return;
            }
            try
            {
                scenario.Tick(frame / 30f);
                if (frame % 120 == 0) verifier.Verify(inventory, frame / 120);
            }
            catch (System.Exception error)
            {
                Directory.CreateDirectory("TestResults"); File.WriteAllText("TestResults/recording.txt", "Failed: " + error);
                Stop(); Debug.LogException(error); return;
            }
            ScreenCapture.CaptureScreenshot(Path.Combine(output, $"frame-{frame:D4}.png"));
            frame++; nextCapture = EditorApplication.timeSinceStartup + 1.0 / 30;
        }
        private static void OnPlayModeChanged(PlayModeStateChange state)
        { if (state == PlayModeStateChange.ExitingPlayMode) Stop(); }
        private static void Stop()
        {
            EditorApplication.update -= Capture; EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            if (inventory != null) { inventory.Interaction.Cancel(); inventory.enabled = true; }
            scenario?.Dispose();
            inventory = null; scenario = null;
        }
    }
}
