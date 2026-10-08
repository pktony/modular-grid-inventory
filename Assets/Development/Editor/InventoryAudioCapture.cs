using System;
using System.IO;
using Unity.Collections;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryAudioCapture : IDisposable
    {
        private readonly InventoryWavWriter writer;
        private readonly string folder;
        private readonly int fps, sampleRate, channels, dspBufferSize;
        private NativeArray<float> buffer;
        private float[] samples;
        private int frames;
        private long timingError;
        private bool disposed;
        public InventoryAudioCapture(string folder, int fps)
        {
            this.folder = folder; this.fps = fps; sampleRate = AudioSettings.outputSampleRate;
            channels = AudioSettings.GetConfiguration().speakerMode switch {
                AudioSpeakerMode.Mono => 1, AudioSpeakerMode.Stereo or AudioSpeakerMode.Prologic => 2,
                AudioSpeakerMode.Quad => 4, AudioSpeakerMode.Surround => 5, AudioSpeakerMode.Mode5point1 => 6,
                AudioSpeakerMode.Mode7point1 => 8, AudioSpeakerMode.Mode7point1point4 => 12,
                _ => throw new InvalidOperationException("Unsupported audio speaker mode.") };
            AudioSettings.GetDSPBufferSize(out dspBufferSize, out _);
            if (!AudioRenderer.Start()) throw new InvalidOperationException("Unity audio renderer is already recording or unavailable.");
            try { writer = new InventoryWavWriter(Path.Combine(folder, "audio.wav"), sampleRate, channels); }
            catch { AudioRenderer.Stop(); throw; }
        }
        public void CaptureFrame()
        {
            int count = AudioRenderer.GetSampleCountForCaptureFrame() * channels;
            if (count <= 0) throw new InvalidOperationException("Unity produced no capture audio samples.");
            if (!buffer.IsCreated || buffer.Length < count)
            {
                if (buffer.IsCreated) buffer.Dispose();
                buffer = new NativeArray<float>(count, Allocator.Persistent); samples = new float[count];
            }
            if (!AudioRenderer.Render(buffer.GetSubArray(0, count))) throw new InvalidOperationException("Unity audio rendering failed.");
            buffer.CopyTo(samples); writer.Write(samples, count); frames++;
            timingError = Math.Max(timingError, Math.Abs(writer.SampleFrames - (long)frames * sampleRate / fps));
        }
        public InventoryAudioCaptureReport Complete(int videoFrames)
        {
            if (frames != videoFrames || timingError > dspBufferSize * 2L)
                throw new InvalidOperationException($"Audio/video capture drift: {frames}/{videoFrames} frames, {timingError} samples.");
            if (writer.Peak < 0.00001f) throw new InvalidOperationException("Captured audio is silent. Check inventory audio settings.");
            var report = new InventoryAudioCaptureReport { fps = fps, videoFrames = frames, sampleRate = sampleRate,
                channels = channels, dspBufferSize = dspBufferSize, rawSampleFrames = writer.SampleFrames,
                sampleFrames = (long)frames * sampleRate / fps, maximumTimingErrorSamples = timingError, peak = writer.Peak };
            writer.Complete(report.sampleFrames);
            File.WriteAllText(Path.Combine(folder, "audio.json"), JsonUtility.ToJson(report, true));
            return report;
        }
        public void Dispose()
        {
            if (disposed) return; disposed = true;
            try { AudioRenderer.Stop(); }
            finally { if (buffer.IsCreated) buffer.Dispose(); writer.Dispose(); }
        }
    }
}
