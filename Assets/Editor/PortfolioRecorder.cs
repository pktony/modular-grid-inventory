using System.IO;
using InventorySystem;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class PortfolioRecorder
    {
        private static Inventory inventory;
        private static InventoryWalkthroughScenario scenario;
        private static string output;
        private static int frame;
        private static double nextCapture;
        [MenuItem("Inventory/Record Walkthrough (Play Mode)")]
        public static void Begin()
        {
            if (!Application.isPlaying) throw new System.InvalidOperationException("Enter Play mode first.");
            Stop(); inventory = Object.FindAnyObjectByType<Inventory>(); inventory.enabled = false;
            scenario = new InventoryWalkthroughScenario(inventory);
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "../Recordings/frames")); Directory.CreateDirectory(output);
            frame = 0; nextCapture = EditorApplication.timeSinceStartup;
            EditorApplication.update += Capture;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        private static void Capture()
        {
            if (EditorApplication.timeSinceStartup < nextCapture) return;
            if (frame >= 1800)
            {
                if (!File.Exists(Path.Combine(output, "frame-1799.png"))) return;
                string results = Path.GetFullPath(Path.Combine(Application.dataPath, "../TestResults"));
                Directory.CreateDirectory(results); File.WriteAllText(Path.Combine(results,"recording.txt"), "1800 frames / 30 fps / 60 seconds / visualized pointer");
                Stop(); return;
            }
            scenario.Tick(frame / 30f);
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
