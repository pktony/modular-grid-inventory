# Asset Store artwork

Claude Code authored the native HTML/CSS/SVG layout. PNGs were rendered and visually checked for clipping and overlap. `sample.png` is an unchanged screenshot of the real Unity sample.

| File | Dimensions | Use |
|---|---|---|
| cover.png | 1950 × 1300 | Package cover |
| card.png | 420 × 280 | Package card |
| icon.png | 160 × 160 | Package icon |
| publisher.png | 400 × 400 | Publisher profile; crop zoom 0.4 |
| promo.png | 1920 × 1280 | Publisher promo; 3:2 crop at zoom 1 |

Run `node render.cjs` with Playwright installed and Chrome available. `PLAYWRIGHT_MODULE` can point to an existing Playwright module. Rendering only loads these local artifacts and blocks remote assets. Marketing files are outside the exported Unity asset root.
