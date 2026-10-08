# Silent English demonstration

[Play the video](modular-grid-inventory-demo-en.mp4): 36.5 seconds, 1920 x 1080, 30 fps, H.264. The file has one video stream and no audio stream.

The footage comes from the actual Unity sample with the fictional icons supplied in the package. Unity pointer handlers drive the gestures and the circular indicator appears while clicking or dragging. Captions and sample UI text are English. Gesture playback stays at 1x; cuts remove idle frames between actions.

## Demonstrated behavior

| Chapter | Behavior |
|---|---|
| 01 | Ammunition, medical supplies and weapon parts in independent pockets |
| 02 | Occupied pockets, oversized items and missing contiguous space rejected |
| 03 | Filled carrier moved into a pack window, retaining contents and its open window |
| 04 | Nested pack opened; an icon drop rejected when storage space is insufficient |
| 05 | Pack > pack > carrier > items; existing window reused when reopened |
| 06 | Ancestor/descendant cycle rejected without changing state |
| 07 | Medical case accepts medical supplies and rejects ammunition |
| 08 | Ammunition case rejects weapons and medical supplies |
| 09 | Stack maximum, partial merge and quantity splitting |
| 10 | Rotation, drag cancellation, deletion and reset |

## Reproduce

Open the sample scene, set **Inventory → Set Capture Resolution 1280x720**, and enter Play Mode with the Game View visible. Run the two menus in sequence, waiting for each capture to finish:

1. **Inventory → Record Container Showcase (Silent, Play Mode)**: 49 stages, 1,764 frames.
2. **Inventory → Record Walkthrough (Silent, Play Mode)**: 27 stages, 1,215 frames.

```powershell
uv run --with imageio-ffmpeg python scripts/render_store_demo.py
```

The renderer validates frame counts and dimensions, writes an edit manifest under `Recordings/store-demo-edit`, and exports 1,095 frames. All 76 recording stages passed their scenario checks. The final video decoded completely without errors, and chapter text was checked for clipping. Capture files and verification reports stay outside the package.

The default scene and prefab are silent. The package contains no sound clips or preconfigured Audio Settings; host-supplied audio remains an optional extension.
