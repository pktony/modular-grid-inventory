# WebGL demo

The portfolio hosts a separate WebGL build of the silent sample scene. The Asset Store archive remains a separate artifact.

Requires Unity 6000.6.4f1 with Web Build Support. Run `Inventory > Build Web Demo`, or use batch mode:

```powershell
$env:BEE_BUILD_THREADS = '1'
$env:DOTNET_PROCESSOR_COUNT = '2'
$env:EMCC_CORES = '1'
& 'C:/Program Files/Unity/Hub/Editor/6000.6.4f1/Editor/Unity.exe' `
  -batchmode -quit -projectPath . -buildTarget WebGL -job-worker-count 2 `
  -executeMethod Pktony.GridInventory.Editor.InventoryWebBuild.Build `
  -logFile Logs/webgl-build.log
```

Output: `Builds/WebGL/inventory`. Set `INVENTORY_WEB_OUTPUT` to use another directory. Serve over HTTP; opening `index.html` as a local file does not work.

- Single thread, Gzip with decompression fallback, 64–512 MB WASM heap.
- Build settings are restored after success or failure.
- The custom template preserves the 16:9 canvas, suppresses the browser context menu and captures keyboard input only when focused.
- `InventoryWebReady` signals when the actual sample UI and read model exist. The parent waits for this signal and `createUnityInstance`.
- Same-origin messages use channel `modular-grid-inventory`: progress, ready, error, stopped; commands: focus, fullscreen, quit.
- `node --test scripts/test-web-template.cjs` checks startup ordering, exit during startup and foreign-message rejection.

Publish the build as a versioned GitHub Release ZIP, then pin its SHA-256 in the portfolio's `config/webgl-builds.json`. Do not commit binaries into the source tree or replace an existing build ID.

Low-memory build note: the first build exhausted memory with the default compiler concurrency. This project was built successfully with `BEE_BUILD_THREADS=1`; Unity documents that variable in its [release notes](https://unity.com/releases/editor/beta/6000.1.0b2).
