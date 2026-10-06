"""Validate Unity main-mix captures and cut PCM using the video frame sequence."""
import json
import wave


def validate_capture(source, video_frames, fps=30):
    report = json.loads((source / "audio.json").read_text(encoding="utf-8-sig"))
    with wave.open(str(source / "audio.wav"), "rb") as audio:
        if (report["fps"] != fps or report["videoFrames"] != video_frames
                or report["peak"] <= 0.00001
                or report["maximumTimingErrorSamples"] > report["dspBufferSize"] * 2
                or audio.getsampwidth() != 2 or audio.getcomptype() != "NONE"
                or audio.getframerate() != report["sampleRate"]
                or audio.getnchannels() != report["channels"]
                or audio.getnframes() != video_frames * audio.getframerate() // fps
                or audio.getnframes() != report["sampleFrames"]):
            raise ValueError("Capture audio is missing, silent, or not synchronized with the video")
    return report


def cut_capture(source, destination, source_frames, video_frames, fps=30):
    report = validate_capture(source, video_frames, fps)
    if not source_frames or source_frames != sorted(set(source_frames)):
        raise ValueError("Audio cuts need an ordered, unique video frame sequence")
    if source_frames[0] < 0 or source_frames[-1] >= video_frames:
        raise ValueError("Audio cut extends outside the capture")
    runs = []
    for frame in source_frames:
        if runs and frame == runs[-1][1]:
            runs[-1][1] = frame + 1
        else:
            runs.append([frame, frame + 1])
    with wave.open(str(source / "audio.wav"), "rb") as original:
        rate, channels = original.getframerate(), original.getnchannels()
        with wave.open(str(destination), "wb") as edited:
            edited.setparams(original.getparams())
            count = 0
            for start, end in runs:
                first, last = start * rate // fps, end * rate // fps
                original.setpos(first)
                samples = original.readframes(last - first)
                if len(samples) != (last - first) * channels * 2:
                    raise ValueError("Capture audio is truncated")
                edited.writeframesraw(samples)
                count += last - first
    if count != len(source_frames) * rate // fps:
        raise ValueError("Edited audio duration differs from the video")
    return {"source": report["source"], "sample_rate": rate, "channels": channels,
            "sample_frames": count, "playback_speed": 1, "cut_runs": runs}
