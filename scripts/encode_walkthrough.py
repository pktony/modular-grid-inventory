"""Encode captured Unity walkthrough frames; requires imageio-ffmpeg."""
import argparse
import subprocess
import math
import tempfile
from pathlib import Path
import imageio_ffmpeg


def caption_filter(captions, folder, font, stage_frames, speed):
    def escaped(path):
        return str(path.resolve()).replace("\\", "/").replace(":", "\\:").replace("'", "\\'")

    filters = ["drawbox=x=332:y=7:w=724:h=44:color=0x07110f@0.94:t=fill"]
    seconds = stage_frames / (30 * speed)
    for index, caption in enumerate(captions):
        text = folder / f"caption-{index}.txt"
        text.write_text(caption, encoding="utf-8")
        filters.append(
            f"drawtext=fontfile='{escaped(font)}':textfile='{escaped(text)}':"
            f"fontsize=21:fontcolor=white:x=348:y=18:"
            f"enable='gte(t,{index * seconds})*lt(t,{(index + 1) * seconds})'"
        )
    return ",".join(filters)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--frames", type=Path, default=Path("Recordings/frames"))
    parser.add_argument("--output", type=Path, default=Path("docs/inventory-walkthrough.mp4"))
    parser.add_argument("--count", type=int, default=1215)
    parser.add_argument("--speed", type=float, default=3.0, help="Playback speed; 1 keeps the capture timing")
    parser.add_argument("--captions", type=Path, help="One UTF-8 subtitle per capture stage")
    parser.add_argument("--stage-frames", type=int, default=36)
    parser.add_argument("--font", type=Path, default=Path("C:/Windows/Fonts/malgunbd.ttf"))
    args = parser.parse_args()
    if not math.isfinite(args.speed) or args.speed <= 0 or args.count <= 0:
        parser.error("Speed and frame count must be positive")
    output_count = max(1, round(args.count / args.speed))
    for frame in range(args.count):
        if not (args.frames / f"frame-{frame:04d}.png").is_file():
            raise SystemExit(f"Missing frame: {frame}")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix="inventory-captions-") as folder:
        filters = []
        if args.captions:
            captions = args.captions.read_text(encoding="utf-8-sig").splitlines()
            if args.stage_frames <= 0 or len(captions) * args.stage_frames != args.count or not args.font.is_file():
                parser.error("Caption count must match the stages, and the font file must exist")
            filters = ["-vf", caption_filter(captions, Path(folder), args.font, args.stage_frames, args.speed)]
        subprocess.run([
            imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
            "-framerate", str(30 * args.speed), "-i", str(args.frames / "frame-%04d.png"),
            *filters, "-frames:v", str(output_count), "-r", "30", "-c:v", "libx264", "-pix_fmt", "yuv420p",
            "-crf", "20", "-movflags", "+faststart", str(args.output)
        ], check=True)
    print(f"{args.output.resolve()} / {output_count} frames / {output_count / 30:g}s / {args.speed:g}x")


if __name__ == "__main__":
    main()
