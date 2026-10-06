"""Verify frame-accurate PCM cuts and invalid capture rejection."""
import json
import struct
import tempfile
import unittest
import wave
from pathlib import Path
from capture_audio import cut_capture, validate_capture


class CaptureAudioTests(unittest.TestCase):
    def setUp(self):
        self.folder = tempfile.TemporaryDirectory()
        self.source = Path(self.folder.name)
        self.report = {"source": "Unity AudioRenderer main mix", "fps": 30, "videoFrames": 10,
                       "sampleRate": 48000, "channels": 2, "sampleFrames": 16000,
                       "maximumTimingErrorSamples": 512, "dspBufferSize": 1024, "peak": .1}
        self.write_report()
        with wave.open(str(self.source / "audio.wav"), "wb") as audio:
            audio.setparams((2, 2, 48000, 0, "NONE", "not compressed"))
            for frame in range(10):
                audio.writeframesraw(struct.pack("<hh", frame + 1, -(frame + 1)) * 1600)

    def tearDown(self):
        self.folder.cleanup()

    def write_report(self):
        (self.source / "audio.json").write_text(json.dumps(self.report), encoding="utf-8")

    def test_cuts_exact_intervals_without_changing_samples_or_pitch(self):
        sequence = [0, 1, 5, 8, 9]
        output = self.source / "edited.wav"
        result = cut_capture(self.source, output, sequence, 10)
        self.assertEqual(result["sample_frames"], 8000)
        with wave.open(str(output), "rb") as audio:
            self.assertEqual(audio.getnframes(), 8000)
            for frame in sequence:
                self.assertEqual(audio.readframes(1600), struct.pack("<hh", frame + 1, -(frame + 1)) * 1600)

    def test_rejects_silent_and_drifting_capture(self):
        for key, value in [("peak", 0), ("maximumTimingErrorSamples", 2049), ("videoFrames", 11)]:
            with self.subTest(key=key):
                original = self.report[key]
                self.report[key] = value
                self.write_report()
                with self.assertRaises(ValueError):
                    validate_capture(self.source, 10)
                self.report[key] = original

    def test_rejects_reordered_duplicate_or_outside_cuts(self):
        for sequence in [[1, 0], [1, 1], [-1, 0], [10], []]:
            with self.subTest(sequence=sequence), self.assertRaises(ValueError):
                cut_capture(self.source, self.source / "edited.wav", sequence, 10)


if __name__ == "__main__":
    unittest.main()
