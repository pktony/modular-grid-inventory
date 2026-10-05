using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class InventoryFrameRecorder
    {
        private static ExpandedInventory inventory;
        private static Action<float> tick;
        private static Action<int> verify;
        private static IDisposable scenario;
        private static string output, report;
        private static int frame, frameCount, framesPerStage, verifiedStages;
        private static double nextCapture;
        private static bool previouslyEnabled;
        public static void Begin(ExpandedInventory source, Action<float> update, Action<int> check, IDisposable lifetime,
            int stages, int stageFrames, string folder, string reportName)
        {
            if (!Application.isPlaying || source?.ReadModel == null) throw new InvalidOperationException("Enter a ready inventory Play mode first.");
            Stop(); inventory = source; previouslyEnabled = inventory.enabled; inventory.enabled = false;
            tick = update; verify = check; scenario = lifetime;
            frameCount = stages * stageFrames; framesPerStage = stageFrames; verifiedStages = 0;
            output = Path.GetFullPath(Path.Combine(Application.dataPath, "../Recordings", folder)); Directory.CreateDirectory(output);
            foreach (string path in Directory.GetFiles(output, "frame-*.png")) File.Delete(path);
            report = Path.GetFullPath(Path.Combine(Application.dataPath, "../TestResults", reportName));
            if (File.Exists(report)) File.Delete(report);
            frame = 0; nextCapture = EditorApplication.timeSinceStartup;
            EditorApplication.update += Capture; EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        private static void Capture()
        {
            if (EditorApplication.timeSinceStartup < nextCapture) return;
            if (frame > 0 && !File.Exists(Path.Combine(output, $"frame-{frame - 1:D4}.png"))) return;
            try
            {
                if (frame >= frameCount)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(report));
                    File.WriteAllText(report, $"{frameCount} frames / 30 fps / {(frameCount / 30f).ToString(CultureInfo.InvariantCulture)} seconds / Unity pointer events / click indicator / {verifiedStages} verified stages");
                    Stop(); return;
                }
                tick(frame / 30f);
                if (frame % framesPerStage == 0) { verify(frame / framesPerStage); verifiedStages++; }
                ScreenCapture.CaptureScreenshot(Path.Combine(output, $"frame-{frame:D4}.png"));
                frame++; nextCapture = EditorApplication.timeSinceStartup + 1.0 / 30;
            }
            catch (Exception error)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(report)); File.WriteAllText(report, "Failed: " + error);
                Stop(); Debug.LogException(error);
            }
        }
        private static void OnPlayModeChanged(PlayModeStateChange state)
        { if (state == PlayModeStateChange.ExitingPlayMode) Stop(); }
        private static void Stop()
        {
            EditorApplication.update -= Capture; EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            scenario?.Dispose();
            if (inventory != null) inventory.enabled = previouslyEnabled;
            inventory = null; scenario = null; tick = null; verify = null;
        }
    }
}
