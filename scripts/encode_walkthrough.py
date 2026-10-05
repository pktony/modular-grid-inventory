"""Encode captured Unity walkthrough frames; requires imageio-ffmpeg."""
import argparse
import subprocess
import math
from pathlib import Path
import imageio_ffmpeg


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--frames", type=Path, default=Path("Recordings/frames"))
    parser.add_argument("--output", type=Path, default=Path("docs/inventory-walkthrough.mp4"))
    parser.add_argument("--count", type=int, default=1215)
    parser.add_argument("--speed", type=float, default=3.0, help="Playback speed; 1 keeps the capture timing")
    args = parser.parse_args()
    if not math.isfinite(args.speed) or args.speed <= 0 or args.count <= 0:
        parser.error("Speed and frame count must be positive")
    output_count = max(1, round(args.count / args.speed))
    for frame in range(args.count):
        if not (args.frames / f"frame-{frame:04d}.png").is_file():
            raise SystemExit(f"Missing frame: {frame}")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    subprocess.run([
        imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
        "-framerate", str(30 * args.speed), "-i", str(args.frames / "frame-%04d.png"),
        "-frames:v", str(output_count), "-r", "30", "-c:v", "libx264", "-pix_fmt", "yuv420p",
        "-crf", "20", "-movflags", "+faststart", str(args.output)
    ], check=True)
    print(f"{args.output.resolve()} / {output_count} frames / {output_count / 30:g}s / {args.speed:g}x")


if __name__ == "__main__":
    main()
