"""Render verified Unity captures with English chapters and no audio track."""
import argparse
import json
import os
import shutil
import struct
import subprocess
import uuid
from pathlib import Path

import imageio_ffmpeg
from edit_container_showcase import text_filter


def edit_plan():
    return [
        ("containers", "Independent pockets", [
            ([(0, 18), (1, 18), (2, 18), (3, 9), (4, 18), (5, 9), (6, 18), (7, 30)],
             "Store ammunition, a medical kit and a rail grip in separate pockets.")]),
        ("containers", "Placement validation", [
            ([(8, 18), (9, 18)], "Occupied pocket: rejected. The original item stays in place."),
            ([(10, 18), (11, 18)], "A 4 x 2 weapon cannot fit inside a 1 x 2 pocket."),
            ([(12, 24)], "Drop onto a carrier icon: storage requires contiguous space.")]),
        ("containers", "Carrier inside a pack", [
            ([(13, 9), (14, 18), (15, 9), (16, 18), (17, 30)],
             "Drag a filled carrier into a pack window. Its contents and window remain.")]),
        ("containers", "Pack inside a pack", [
            ([(18, 9), (19, 18), (20, 9), (21, 18)], "Open a nested compact pack to inspect its medical kit."),
            ([(22, 24), (23, 18)], "Drop onto another pack icon: rejected when there is insufficient space.")]),
        ("containers", "Nested contents and window identity", [
            ([(24, 18), (25, 9)], "Take the filled compact pack back out into storage."),
            ([(26, 18), (27, 30), (28, 18)], "Pack > pack > carrier > items: contents stay with their containers."),
            ([(29, 36)], "Reopen the same pack: focus its existing window without creating a duplicate.")]),
        ("containers", "Cycle protection", [
            ([(30, 30), (31, 30)], "A parent pack cannot go inside its own descendant. State is preserved.")]),
        ("containers", "Category rules: medical case", [
            ([(32, 9), (33, 18), (34, 18), (35, 18)], "Medical kit: accepted by the medical case."),
            ([(36, 24), (37, 24)], "Ammunition: rejected by the medical case, including icon drops.")]),
        ("containers", "Category rules: ammunition case", [
            ([(38, 9), (39, 18), (40, 18), (41, 18)], "Twenty rounds: accepted by the ammunition case."),
            ([(42, 24), (43, 24)], "Weapon: rejected even when the case has enough empty space."),
            ([(44, 18), (45, 24), (46, 24), (47, 6)], "Medical kit: rejected. Both cases keep their original contents.")]),
        ("walkthrough", "Stack limits and splitting", [
            ([(19, 18), (20, 30)], "Merge 20 into 40: a 50-round stack plus 10 left over."),
            ([(21, 18), (22, 18)], "Choose five rounds to split. A new instance is created only on placement.")]),
        ("walkthrough", "Rotation, cancel and reset", [
            ([(23, 18), (23, 21, 24), (24, 30)], "Rotate during drag. Cancel preserves placement; Delete removes selection."),
            ([(25, 9), (26, 36)], "Reset creates the initial state again. Multiple pack windows remain independent.")]),
    ]


def validate_capture(folder, count):
    frames = sorted(folder.glob("frame-*.png"))
    if len(frames) != count:
        raise ValueError(f"Expected {count} frames in {folder}, got {len(frames)}")
    for index, frame in enumerate(frames):
        if frame.name != f"frame-{index:04d}.png" or struct.unpack(">II", frame.read_bytes()[16:24]) != (1280, 720):
            raise ValueError(f"Invalid frame: {frame}")
    return frames


def assemble(captures, work):
    sequence, chapters, details = [], [], []
    for source, title, sections in edit_plan():
        start = len(sequence)
        for cuts, detail in sections:
            detail_start = len(sequence)
            for cut in cuts:
                stage, count, *offset = cut
                first = stage * (36 if source == "containers" else 45) + (offset[0] if offset else 0)
                sequence.extend((source, index) for index in range(first, first + count))
            details.append((detail_start, len(sequence), detail))
        chapters.append((start, len(sequence), title))
    for index, (source, frame) in enumerate(sequence):
        destination = work / f"frame-{index:04d}.png"
        try:
            os.link(captures[source][frame], destination)
        except OSError:
            shutil.copyfile(captures[source][frame], destination)
    manifest = {"fps": 30, "playback_speed": 1, "audio": False,
                "source_frames": sequence, "chapters": chapters, "details": details}
    (work / "edit.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    return manifest


def render(args):
    captures = {"containers": validate_capture(args.containers, 1764),
                "walkthrough": validate_capture(args.walkthrough, 1215)}
    if not args.font.is_file():
        raise ValueError(f"Missing font: {args.font}")
    work = args.containers.parent / "store-demo-edit" / uuid.uuid4().hex
    work.mkdir(parents=True)
    manifest = assemble(captures, work)
    filters = ["scale=1728:972:flags=lanczos", "pad=1920:1080:96:108:color=0x080d11"]
    for index, (start, end, title) in enumerate(manifest["chapters"]):
        filters.append(text_filter(work, args.font, title, f"title-{index}", start, end, 36, 96, 12, "0xe8f0f2"))
        filters.append(text_filter(work, args.font, f"{index + 1:02d} / 10", f"number-{index}", start, end, 24, 1700, 23, "0x5edbd1"))
    for index, (start, end, detail) in enumerate(manifest["details"]):
        filters.append(text_filter(work, args.font, detail, f"detail-{index}", start, end, 25, 96, 64, "0xc2cdd3"))
    args.output.parent.mkdir(parents=True, exist_ok=True)
    subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
                    "-framerate", "30", "-i", str(work / "frame-%04d.png"), "-an",
                    "-vf", ",".join(filters), "-c:v", "libx264", "-crf", "18", "-preset", "fast",
                    "-pix_fmt", "yuv420p", "-movflags", "+faststart", str(args.output)], check=True)
    print(json.dumps({"video": str(args.output.resolve()), "frames": len(manifest["source_frames"]),
                      "seconds": len(manifest["source_frames"]) / 30, "audio_tracks": 0,
                      "manifest": str(work / "edit.json")}, indent=2))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--containers", type=Path, default=Path("Recordings/container-showcase-silent"))
    parser.add_argument("--walkthrough", type=Path, default=Path("Recordings/frames-silent"))
    parser.add_argument("--output", type=Path, default=Path("docs/modular-grid-inventory-demo-en.mp4"))
    parser.add_argument("--font", type=Path, default=Path("C:/Windows/Fonts/segoeui.ttf"))
    render(parser.parse_args())
