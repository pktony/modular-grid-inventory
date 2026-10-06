using System;
namespace InventorySystem.Editor
{
    [Serializable]
    public sealed class InventoryAudioCaptureReport
    {
        public string source = "Unity AudioRenderer main mix";
        public int fps, videoFrames, sampleRate, channels, dspBufferSize;
        public long rawSampleFrames, sampleFrames, maximumTimingErrorSamples;
        public float peak;
    }
}
