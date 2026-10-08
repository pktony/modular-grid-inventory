using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class InventoryFrameRecorder
    {
        private static InventoryBootstrapper inventory;
        private static Action<float> tick;
        private static Action<int> verify;
        private static IDisposable scenario;
        private static InventoryAudioCapture audio;
        private static Coroutine loop;
        private static string output, report;
        private static int frame, frameCount, framesPerStage, verifiedStages, previousCaptureRate;
        private static bool previouslyEnabled, recording;
        private static bool captureSound;
        public static bool IsRecording => recording;
        public static int CapturedFrames => frame;
        public static void Begin(InventoryBootstrapper source, Action<float> update, Action<int> check, IDisposable lifetime,
            int stages, int stageFrames, string folder, string reportName, bool recordAudio = false)
        {
            if (!Application.isPlaying || source?.ReadModel == null || EditorApplication.isPaused)
                throw new InvalidOperationException("Enter an unpaused, ready inventory Play mode first.");
            if (stages <= 0 || stageFrames <= 0 || update == null || check == null)
                throw new ArgumentException("Recording needs stages and update/verification callbacks.");
            Stop(); previousCaptureRate = Time.captureFramerate; recording = true;
            inventory = source; previouslyEnabled = inventory.enabled; inventory.enabled = false;
            try
            {
                tick = update; verify = check; scenario = lifetime; captureSound = recordAudio;
                frameCount = stages * stageFrames; framesPerStage = stageFrames; verifiedStages = 0;
                output = Path.GetFullPath(Path.Combine(Application.dataPath, "../Recordings", folder)); Directory.CreateDirectory(output);
                foreach (string path in Directory.GetFiles(output, "frame-*.png")) File.Delete(path);
                report = Path.GetFullPath(Path.Combine(Application.dataPath, "../TestResults", reportName));
                if (File.Exists(report)) File.Delete(report);
                foreach (string name in new[] { "audio.wav", "audio.json" })
                    if (File.Exists(Path.Combine(output, name))) File.Delete(Path.Combine(output, name));
                frame = 0; Time.captureFramerate = 30;
                EditorApplication.playModeStateChanged += OnPlayModeChanged;
                AssemblyReloadEvents.beforeAssemblyReload += Stop;
                loop = inventory.StartCoroutine(Capture());
            }
            catch { Stop(); throw; }
        }
        private static IEnumerator Capture()
        {
            yield return null;
            if (captureSound && !Try(() => audio = new InventoryAudioCapture(output, 30))) yield break;
            var endOfFrame = new WaitForEndOfFrame();
            while (frame < frameCount)
            {
                if (!Try(() => {
                    tick(frame / 30f);
                    if (frame % framesPerStage == 0) { verify(frame / framesPerStage); verifiedStages++; }
                })) yield break;
                yield return endOfFrame;
                if (!Try(() => {
                    var texture = ScreenCapture.CaptureScreenshotAsTexture();
                    try { File.WriteAllBytes(Path.Combine(output, $"frame-{frame:D4}.png"), texture.EncodeToPNG()); }
                    finally { UnityEngine.Object.Destroy(texture); }
                    audio?.CaptureFrame(); frame++;
                })) yield break;
                if (frame < frameCount) yield return null;
            }
            Try(() => {
                var captured = audio?.Complete(frameCount); Directory.CreateDirectory(Path.GetDirectoryName(report));
                File.WriteAllText(report, $"{frameCount} frames / 30 fps / {(frameCount / 30f).ToString(CultureInfo.InvariantCulture)} seconds / Unity pointer events / click indicator / {verifiedStages} verified stages\n"
                    + (captured == null ? "Silent capture / no audio track" : $"AudioRenderer / {captured.sampleRate} Hz / {captured.channels} channels / {captured.sampleFrames} sample frames / peak {captured.peak.ToString(CultureInfo.InvariantCulture)} / maximum drift {captured.maximumTimingErrorSamples} samples"));
            });
            Stop();
        }
        private static bool Try(Action action)
        {
            try { action(); return true; }
            catch (Exception error)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(report)); File.WriteAllText(report, "Failed: " + error);
                Stop(); Debug.LogException(error); return false;
            }
        }
        private static void OnPlayModeChanged(PlayModeStateChange state)
        { if (state == PlayModeStateChange.ExitingPlayMode) Stop(); }
        private static void Stop()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged; AssemblyReloadEvents.beforeAssemblyReload -= Stop;
            if (!recording) return;
            if (inventory != null && loop != null) inventory.StopCoroutine(loop);
            try { audio?.Dispose(); }
            finally
            {
                try { scenario?.Dispose(); }
                finally { Time.captureFramerate = previousCaptureRate;
                    if (inventory != null) inventory.enabled = previouslyEnabled;
                    inventory = null; scenario = null; tick = null; verify = null; audio = null; loop = null; recording = false; }
            }
        }
    }
}
