"""Encode captured Unity walkthrough frames; requires imageio-ffmpeg."""
import argparse
import subprocess
from pathlib import Path
import imageio_ffmpeg


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--frames", type=Path, default=Path("Recordings/frames"))
    parser.add_argument("--output", type=Path, default=Path("docs/inventory-walkthrough.mp4"))
    parser.add_argument("--count", type=int, default=1215)
    args = parser.parse_args()
    for frame in range(args.count):
        if not (args.frames / f"frame-{frame:04d}.png").is_file():
            raise SystemExit(f"Missing frame: {frame}")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    subprocess.run([
        imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
        "-framerate", "30", "-i", str(args.frames / "frame-%04d.png"),
        "-frames:v", str(args.count), "-c:v", "libx264", "-pix_fmt", "yuv420p",
        "-crf", "20", "-movflags", "+faststart", str(args.output)
    ], check=True)
    print(args.output.resolve())


if __name__ == "__main__":
    main()
