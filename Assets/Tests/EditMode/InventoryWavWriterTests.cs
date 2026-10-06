using System;
using System.IO;
using System.Text;
using InventorySystem.Editor;
using NUnit.Framework;
namespace InventorySystem.Tests
{
    public sealed class InventoryWavWriterTests
    {
        private string path;
        [SetUp] public void SetUp() => path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
        [TearDown] public void TearDown() { if (File.Exists(path)) File.Delete(path); }
        [Test]
        public void WritesPlayableStereoPcmWithClampedSamples()
        {
            using (var writer = new InventoryWavWriter(path, 48000, 2))
            { writer.Write(new[] { 2f, -2f, .5f, -.5f }, 4); writer.Complete(2); }
            using var reader = new BinaryReader(File.OpenRead(path));
            Assert.That(Encoding.ASCII.GetString(reader.ReadBytes(4)), Is.EqualTo("RIFF"));
            Assert.That(reader.ReadInt32(), Is.EqualTo(44));
            Assert.That(Encoding.ASCII.GetString(reader.ReadBytes(8)), Is.EqualTo("WAVEfmt "));
            Assert.That(reader.ReadInt32(), Is.EqualTo(16));
            Assert.That(reader.ReadInt16(), Is.EqualTo(1));
            Assert.That(reader.ReadInt16(), Is.EqualTo(2));
            Assert.That(reader.ReadInt32(), Is.EqualTo(48000));
            Assert.That(reader.ReadInt32(), Is.EqualTo(192000));
            Assert.That(reader.ReadInt16(), Is.EqualTo(4));
            Assert.That(reader.ReadInt16(), Is.EqualTo(16));
            Assert.That(Encoding.ASCII.GetString(reader.ReadBytes(4)), Is.EqualTo("data"));
            Assert.That(reader.ReadInt32(), Is.EqualTo(8));
            Assert.That(reader.ReadInt16(), Is.EqualTo(32767));
            Assert.That(reader.ReadInt16(), Is.EqualTo(-32767));
            Assert.That(reader.ReadInt16(), Is.EqualTo(16384));
            Assert.That(reader.ReadInt16(), Is.EqualTo(-16384));
        }
        [TestCase(1, 48)]
        [TestCase(3, 56)]
        public void CompletesToVideoDurationByTrimmingOrPadding(int frames, int bytes)
        {
            using (var writer = new InventoryWavWriter(path, 48000, 2))
            { writer.Write(new[] { .5f, .5f, .25f, .25f }, 4); writer.Complete(frames); }
            Assert.That(new FileInfo(path).Length, Is.EqualTo(bytes));
            if (frames > 2) Assert.That(File.ReadAllBytes(path)[52..], Is.All.EqualTo(0));
        }
        [Test]
        public void RejectsInvalidAudioAndWritesAfterCompletion()
        {
            using var writer = new InventoryWavWriter(path, 48000, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.Write(new[] { .5f }, 1));
            Assert.Throws<InvalidDataException>(() => writer.Write(new[] { float.NaN, 0 }, 2));
            writer.Complete(0);
            Assert.Throws<InvalidOperationException>(() => writer.Write(new[] { 0f, 0f }, 2));
        }
    }
}
