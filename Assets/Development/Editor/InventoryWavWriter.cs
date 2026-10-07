using System;
using System.IO;
using System.Text;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryWavWriter : IDisposable
    {
        private readonly BinaryWriter writer;
        private readonly int sampleRate, channels;
        private bool completed;
        public long SampleFrames { get; private set; }
        public float Peak { get; private set; }
        public InventoryWavWriter(string path, int sampleRate, int channels)
        {
            if (sampleRate <= 0 || channels <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRate));
            this.sampleRate = sampleRate; this.channels = channels;
            writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read));
            writer.Write(new byte[44]);
        }
        public void Write(float[] samples, int count)
        {
            if (completed) throw new InvalidOperationException("Recording is already complete.");
            if (count < 0 || count > samples.Length || count % channels != 0) throw new ArgumentOutOfRangeException(nameof(count));
            for (int index = 0; index < count; index++)
            {
                float sample = samples[index];
                if (float.IsNaN(sample) || float.IsInfinity(sample)) throw new InvalidDataException("Audio contains a non-finite sample.");
                Peak = Math.Max(Peak, Math.Abs(sample));
                writer.Write((short)Math.Round(Math.Max(-1, Math.Min(1, sample)) * short.MaxValue));
            }
            SampleFrames += count / channels;
        }
        public void Complete(long sampleFrames)
        {
            if (completed) return;
            if (sampleFrames < 0) throw new ArgumentOutOfRangeException(nameof(sampleFrames));
            int bytes = checked((int)(sampleFrames * channels * sizeof(short)));
            writer.Flush(); writer.BaseStream.SetLength(44L + bytes); writer.BaseStream.Position = 0;
            writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + bytes); writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
            writer.Write(16); writer.Write((short)1); writer.Write((short)channels); writer.Write(sampleRate);
            writer.Write(sampleRate * channels * sizeof(short)); writer.Write((short)(channels * sizeof(short))); writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(bytes); writer.Flush();
            SampleFrames = sampleFrames; completed = true;
        }
        public void Dispose() { if (!completed) Complete(SampleFrames); writer.Dispose(); }
    }
}
