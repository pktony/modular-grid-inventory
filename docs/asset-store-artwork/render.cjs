'use strict';
// Authors the Asset Store artwork as native HTML/CSS/SVG and renders it with Playwright.
// Writes claude_*.html (source) next to this file, then cover/card/icon/publisher/banner PNGs.
// The real Unity screenshot is referenced as-is (no crop, no re-encode, no stretching).

const fs = require('fs');
const path = require('path');
const { pathToFileURL } = require('url');
const { chromium } = require(process.env.PLAYWRIGHT_MODULE || 'playwright');

const OUT_DIR = __dirname;
const SCREENSHOT = path.join(__dirname, 'sample.png');
const SHOT_W = 1920;
const SHOT_H = 1080;

const C = {
  bg: '#0b1517',
  bg2: '#102528',
  panel: '#0f2326',
  line: '#264043',
  teal: '#3cc9b9',
  tealDim: '#2f5f5d',
  amber: '#e2b45f',
  text: '#eef5f3',
  muted: '#a9bdb9',
};

const FONT = "'Segoe UI', 'Helvetica Neue', Arial, sans-serif";

// Original grid mark: a 3x3 inventory grid with four differently sized items and one empty slot.
function mark(size) {
  return `<svg width="${size}" height="${size}" viewBox="0 0 120 120" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
  <rect x="3" y="3" width="114" height="114" rx="18" fill="${C.panel}" stroke="${C.teal}" stroke-width="4"/>
  <rect x="46" y="46" width="28" height="28" rx="4" stroke="${C.tealDim}" stroke-width="2.5"/>
  <rect x="14" y="14" width="60" height="28" rx="4" fill="${C.teal}"/>
  <rect x="78" y="14" width="28" height="60" rx="4" fill="${C.amber}"/>
  <rect x="14" y="46" width="28" height="60" rx="4" fill="#27a89b"/>
  <rect x="46" y="78" width="60" height="28" rx="4" fill="${C.teal}"/>
</svg>`;
}

const BASE_CSS = `
*{box-sizing:border-box;margin:0;padding:0}
html,body{background:${C.bg};color:${C.text};font-family:${FONT};-webkit-font-smoothing:antialiased}
.art{position:relative;overflow:hidden;background:linear-gradient(135deg,${C.bg} 0%,${C.bg2} 100%)}
.grid{position:absolute;inset:0;background-image:linear-gradient(${C.line}66 1px,transparent 1px),linear-gradient(90deg,${C.line}66 1px,transparent 1px);background-size:60px 60px;opacity:.45}
.abs{position:absolute}
`;

function page(width, height, body, css) {
  return `<!doctype html>
<html lang="en"><head><meta charset="utf-8"><title>Modular Grid Inventory artwork</title>
<style>${BASE_CSS}
.art{width:${width}px;height:${height}px}
${css}</style></head>
<body><main class="art" data-w="${width}" data-h="${height}"><div class="grid"></div>${body}</main></body></html>`;
}

// ---------- cover 1950x1300 ----------
function cover(shotUrl) {
  const FRAME_W = 1460; // 2px border + 1456px image => exact 16:9 (1456x819)
  const FRAME_L = (1950 - FRAME_W) / 2;
  const css = `
.mark{left:90px;top:68px}
.title{left:266px;top:58px;font-size:80px;line-height:1.02;font-weight:700;letter-spacing:-2px;white-space:nowrap}
.pub{right:90px;top:84px;text-align:right}
.pub b{display:block;font-size:36px;font-weight:700;letter-spacing:.5px;color:${C.teal}}
.pub span{display:block;margin-top:6px;font-size:25px;color:${C.muted}}
.frame{left:${FRAME_L}px;top:246px;width:${FRAME_W}px;border:2px solid ${C.tealDim};border-radius:10px;overflow:hidden;background:#000;box-shadow:0 24px 70px #000b}
.frame img{display:block;width:1456px;height:819px}
.feat{left:90px;right:90px;bottom:46px;display:grid;grid-template-columns:repeat(3,1fr);column-gap:56px;border-top:1px solid ${C.line};padding-top:22px}
.feat h3{font-size:22px;font-weight:600;letter-spacing:3px;color:${C.teal};margin-bottom:8px}
.feat p{font-size:29px;line-height:1.28;color:${C.text}}
`;
  const body = `
<div class="abs mark">${mark(140)}</div>
<h1 class="abs title">Modular<br>Grid Inventory</h1>
<div class="abs pub"><b>doroks</b><span>Free source package &middot; Unity desktop uGUI</span></div>
<div class="abs frame"><img src="${shotUrl}" alt="Actual Modular Grid Inventory sample in Unity Play Mode"></div>
<section class="abs feat">
  <div><h3>CONFIGURE</h3><p>Item sizes, categories<br>&amp; stack limits</p></div>
  <div><h3>COMPOSE</h3><p>Separated pockets<br>&amp; nested containers</p></div>
  <div><h3>AUTHOR</h3><p>ScriptableObjects &amp;<br>Inspector layout preview</p></div>
</section>`;
  return page(1950, 1300, body, css);
}

// ---------- card 420x280 ----------
function card() {
  const css = `
.top{left:28px;top:26px;right:28px;display:flex;align-items:center;justify-content:space-between}
.top b{font-size:24px;font-weight:700;color:${C.teal};letter-spacing:.5px}
.name{left:28px;bottom:62px;font-size:42px;line-height:1.06;font-weight:700;letter-spacing:-1px;white-space:nowrap}
.sub{left:28px;bottom:26px;font-size:20px;color:${C.muted};white-space:nowrap}
.rule{left:28px;bottom:56px;width:56px;height:3px;background:${C.teal};border-radius:2px}
`;
  const body = `
<div class="abs top">${mark(68)}<b>doroks</b></div>
<h1 class="abs name">Modular<br>Grid Inventory</h1>
<div class="abs sub">Unity uGUI &middot; Free source</div>`;
  return page(420, 280, body, css);
}

// ---------- icon 160x160 ----------
function icon() {
  const css = `
.art{background:linear-gradient(135deg,${C.bg} 0%,${C.bg2} 100%)}
.grid{display:none}
.mark{left:20px;top:20px}
`;
  return page(160, 160, `<div class="abs mark">${mark(120)}</div>`, css);
}

// ---------- publisher 400x400 ----------
function publisher() {
  const css = `
.grid{background-size:50px 50px}
.mark{left:56px;top:56px}
`;
  return page(400, 400, `<div class="abs mark">${mark(288)}</div>`, css);
}

// ---------- banner 1920x320 ----------
function banner() {
  const css = `
.grid{background-size:40px 40px;opacity:.35}
.row{left:100px;right:100px;top:0;bottom:0;display:flex;align-items:center;gap:56px}
.label{font-size:28px;font-weight:700;letter-spacing:7px;color:${C.teal};margin-bottom:10px}
.head{font-size:66px;line-height:1.08;font-weight:700;letter-spacing:-1.5px;white-space:nowrap}
.sub{margin-top:14px;font-size:28px;color:${C.muted};white-space:nowrap}
`;
  const body = `
<div class="abs row">${mark(168)}
  <div><div class="label">DOROKS</div><h1 class="head">Configurable inventory tools for Unity</h1><div class="sub">Modular Grid Inventory &middot; free source package</div></div>
</div>`;
  return page(1920, 320, body, css);
}

// ---------- publisher profile promo 1920x1280 (3:2) ----------
function promo() {
  const css = `
.grid{background-size:80px 80px;opacity:.4}
.stack{left:140px;right:140px;top:140px;bottom:140px;display:flex;flex-direction:column;align-items:center;justify-content:center;text-align:center}
.stack svg{display:block}
.pub{margin-top:48px;font-size:76px;line-height:1;font-weight:700;letter-spacing:1px;color:${C.teal}}
.head{margin-top:28px;font-size:80px;line-height:1.12;font-weight:700;letter-spacing:-2px}
.label{margin-top:44px;font-size:30px;font-weight:600;letter-spacing:7px;color:${C.muted};padding-top:24px;border-top:2px solid ${C.line}}
`;
  const body = `
<div class="abs stack">${mark(320)}
  <div class="pub">doroks</div>
  <h1 class="head">Configurable inventory<br>tools for Unity</h1>
  <div class="label">MODULAR GRID INVENTORY</div>
</div>`;
  return page(1920, 1280, body, css);
}

function pngSize(file) {
  const b = fs.readFileSync(file);
  if (b.toString('ascii', 1, 4) !== 'PNG') throw new Error(`${file} is not a PNG`);
  return { w: b.readUInt32BE(16), h: b.readUInt32BE(20) };
}

async function main() {
  if (!fs.existsSync(SCREENSHOT)) throw new Error(`Missing screenshot: ${SCREENSHOT}`);
  const shot = pngSize(SCREENSHOT);
  if (shot.w !== SHOT_W || shot.h !== SHOT_H) {
    throw new Error(`Screenshot is ${shot.w}x${shot.h}, expected ${SHOT_W}x${SHOT_H}; cover layout assumes 16:9 at 1456x819.`);
  }
  const shotUrl = pathToFileURL(SCREENSHOT).href;

  const jobs = [
    { name: 'cover', w: 1950, h: 1300, html: cover(shotUrl) },
    { name: 'card', w: 420, h: 280, html: card() },
    { name: 'icon', w: 160, h: 160, html: icon() },
    { name: 'publisher', w: 400, h: 400, html: publisher() },
    { name: 'banner', w: 1920, h: 320, html: banner() },
    { name: 'promo', w: 1920, h: 1280, html: promo() },
  ];

  const browser = await chromium.launch({ channel: 'chrome', headless: true });
  try {
    for (const job of jobs) {
      const htmlPath = path.join(OUT_DIR, `claude_${job.name}.html`);
      const pngPath = path.join(OUT_DIR, `${job.name}.png`);
      fs.writeFileSync(htmlPath, job.html, 'utf8');

      const ctx = await browser.newContext({ viewport: { width: job.w, height: job.h }, deviceScaleFactor: 1 });
      const pg = await ctx.newPage();
      // Local artifacts only: block anything that is not a file:// request.
      await pg.route('**/*', (route) => (route.request().url().startsWith('file:') ? route.continue() : route.abort()));
      await pg.goto(pathToFileURL(htmlPath).href, { waitUntil: 'load' });
      await pg.evaluate(() => document.fonts.ready);

      const report = await pg.evaluate(() => {
        const issues = [];
        document.querySelectorAll('img').forEach((img) => {
          if (!img.complete || img.naturalWidth === 0) issues.push('image failed to load: ' + img.src);
        });
        const art = document.querySelector('.art').getBoundingClientRect();
        document.querySelectorAll('.art *').forEach((el) => {
          if (el.classList.contains('grid')) return;
          const r = el.getBoundingClientRect();
          if (r.width && (r.left < -0.5 || r.top < -0.5 || r.right > art.right + 0.5 || r.bottom > art.bottom + 0.5)) {
            issues.push('outside canvas: <' + el.tagName.toLowerCase() + '> ' + (el.className && el.className.baseVal === undefined ? el.className : ''));
          }
        });
        return issues;
      });
      report.forEach((m) => console.warn(`[${job.name}] WARN ${m}`));

      await pg.screenshot({ path: pngPath, clip: { x: 0, y: 0, width: job.w, height: job.h } });
      await ctx.close();

      const out = pngSize(pngPath);
      const ok = out.w === job.w && out.h === job.h;
      console.log(`${job.name}.png ${out.w}x${out.h}${ok ? '' : `  (EXPECTED ${job.w}x${job.h})`}`);
      if (!ok) process.exitCode = 1;
    }
  } finally {
    await browser.close();
  }
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
