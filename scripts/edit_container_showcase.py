"""Cut idle frames from the verified Unity capture while preserving gesture speed."""
import argparse
import json
import os
import shutil
import struct
import subprocess
import uuid
from pathlib import Path

import imageio_ffmpeg


def edit_plan():
    return [
        ("01", "리그의 독립 포켓에 아이템 수납", [
            ([(0, 18), (1, 18), (2, 18), (3, 9), (4, 18), (5, 9), (6, 18), (7, 30)],
             "탄약 40발 · AI-2 의료품 · RK-0 부품을 각각 수납")]),
        ("02", "잘못된 포켓 배치 거절", [
            ([(8, 18), (9, 18)], "다른 탄약이 놓인 칸에 드롭 → 합치기 불가, 원본 유지"),
            ([(10, 18), (11, 18)], "4×2 무기 → 1×2 포켓: 크기 초과로 거절"),
            ([(12, 24)], "리그 아이콘에 드롭 → 연속 공간이 없어 자동 수납 거절")]),
        ("03", "내용물이 든 리그를 가방에 수납", [
            ([(13, 9), (14, 18), (15, 9), (16, 18), (17, 30)],
             "가방 모달로 드롭 → 리그 안의 3개 아이템과 열린 창 유지")]),
        ("04", "가방 속 가방 · 공간 부족 거절", [
            ([(18, 9), (19, 18), (20, 9), (21, 18)], "가방 안의 MBSS를 열어 내부 AI-2 의료품 확인"),
            ([(22, 24), (23, 18)], "리그가 든 가방 아이콘에 MBSS 드롭 → 빈 공간 부족으로 거절")]),
        ("05", "다단계 중첩 · 기존 창 재사용", [
            ([(24, 18), (25, 9)], "내용물이 든 MBSS를 보관함으로 꺼내 빈 가방 준비"),
            ([(26, 18), (27, 30), (28, 18)], "가방 → 가방 → 리그 → 아이템: 중첩 후에도 내용물 유지"),
            ([(29, 36)], "같은 가방 다시 열기 → 기존 창을 앞으로, 새 창은 생성하지 않음")]),
        ("06", "순환 중첩 방지", [
            ([(30, 30), (31, 30)], "부모 가방을 안의 가방에 넣기 → 거절, 원래 소속과 내용물 유지")]),
        ("07", "의료품 케이스의 유형별 수납 규칙", [
            ([(32, 9), (33, 18), (34, 18), (35, 18)], "AI-2 의료품 → 의료품 케이스: 허용, 수납 성공"),
            ([(36, 24), (37, 24)], "탄약 → 의료품 케이스 아이콘: 금지 유형, 수납 거절")]),
        ("08", "탄약 케이스의 유형별 수납 규칙", [
            ([(38, 9), (39, 18), (40, 18), (41, 18)], "탄약 20발 → 탄약 케이스 모달: 허용, 수납 성공"),
            ([(42, 24), (43, 24)], "무기 → 탄약 케이스의 빈 칸: 공간이 있어도 유형 제한으로 거절"),
            ([(44, 18), (45, 24), (46, 24), (47, 6)], "의료품 케이스 안의 AI-2 → 탄약 케이스: 금지 유형으로 거절")]),
        ("완료", "초기화", [([(48, 27)], "전체 49단계 검증 완료 · 실제 Unity Game View · 클릭 중 원형 표시")]),
    ]


def stage_frames(source, work):
    inputs = sorted(source.glob("frame-*.png"))
    if len(inputs) != 1764:
        raise SystemExit("Expected the complete 49-stage / 1764-frame capture")
    for index, path in enumerate(inputs):
        if path.name != f"frame-{index:04d}.png" or struct.unpack(">II", path.read_bytes()[16:24]) != (1280, 720):
            raise SystemExit(f"Invalid capture frame: {path}")
    sequence, chapters, details = [], [], []
    for number, title, sections in edit_plan():
        start = len(sequence)
        for cuts, detail in sections:
            detail_start = len(sequence)
            for stage, count in cuts:
                sequence.extend(range(stage * 36, stage * 36 + count))
            details.append((detail_start, len(sequence), detail))
        chapters.append((start, len(sequence), number, title))
    if sequence != sorted(set(sequence)):
        raise SystemExit("Edit must preserve capture order without duplicate frames")
    for index, original in enumerate(sequence):
        destination = work / f"frame-{index:04d}.png"
        try:
            os.link(inputs[original], destination)
        except OSError:
            shutil.copyfile(inputs[original], destination)
    manifest = {"fps": 30, "playback_speed": 1, "source_frames": sequence,
                "chapters": chapters, "details": details}
    (work / "edit.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding="utf-8")
    return manifest


def text_filter(work, font, text, key, start, end, size, x, y, color):
    def escaped(path):
        return path.resolve().as_posix().replace(":", "\\:").replace("'", "\\'")
    path = work / f"{key}.txt"
    path.write_text(text, encoding="utf-8")
    return (f"drawtext=fontfile='{escaped(font)}':textfile='{escaped(path)}':"
            f"fontsize={size}:fontcolor={color}:x={x}:y={y}:"
            f"enable='gte(t,{start / 30:.6f})*lt(t,{end / 30:.6f})'")


def render(source, output, font):
    if not font.is_file():
        raise SystemExit(f"Font not found: {font}")
    work = source.parent / "container-showcase-edit" / uuid.uuid4().hex
    work.mkdir(parents=True)
    manifest = stage_frames(source, work)
    filters = ["scale=1728:972:flags=lanczos", "pad=1920:1080:96:108:color=0x07110f"]
    for index, (start, end, number, title) in enumerate(manifest["chapters"]):
        filters.append(text_filter(work, font, title, f"title-{index}", start, end, 36, 96, 13, "0xeeeade"))
        badge = number + " / 08" if number != "완료" else number
        filters.append(text_filter(work, font, badge, f"number-{index}", start, end, 25, 1728, 25, "0xa6c6b3"))
    for index, (start, end, detail) in enumerate(manifest["details"]):
        filters.append(text_filter(work, font, detail, f"detail-{index}", start, end, 25, 96, 65, "0xc1c9bf"))
    output.parent.mkdir(parents=True, exist_ok=True)
    subprocess.run([imageio_ffmpeg.get_ffmpeg_exe(), "-hide_banner", "-loglevel", "error", "-y",
                    "-framerate", "30", "-i", str(work / "frame-%04d.png"), "-vf", ",".join(filters),
                    "-frames:v", str(len(manifest["source_frames"])), "-an", "-c:v", "libx264",
                    "-crf", "18", "-preset", "fast", "-pix_fmt", "yuv420p", "-movflags", "+faststart", str(output)], check=True)
    print(f"{output.resolve()} / {len(manifest['source_frames'])} frames / {len(manifest['source_frames']) / 30:g}s / 1x gestures")
    print(f"Edit manifest: {work / 'edit.json'}")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--frames", type=Path, default=Path("Recordings/container-showcase"))
    parser.add_argument("--output", type=Path, default=Path("docs/inventory-container-showcase.mp4"))
    parser.add_argument("--font", type=Path, default=Path("C:/Windows/Fonts/malgunbd.ttf"))
    args = parser.parse_args()
    render(args.frames, args.output, args.font)
