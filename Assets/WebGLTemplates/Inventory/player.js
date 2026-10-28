(() => {
  "use strict";
  const channel = "modular-grid-inventory";
  const canvas = document.getElementById("unity-canvas");
  const status = document.getElementById("status");
  const message = document.getElementById("message");
  const progress = document.getElementById("progress");
  const config = window.inventoryConfig;
  let instance, sceneReady = false, stopped = false, startupTimeout;
  const send = (type, extra = {}) => {
    if (window.parent !== window) window.parent.postMessage({ channel, type, ...extra }, location.origin);
  };
  const fail = () => {
    if (stopped) return;
    clearTimeout(startupTimeout);
    status.hidden = false;
    status.setAttribute("role", "alert");
    message.textContent = "Unable to start. Reload this page or watch the demonstration.";
    progress.hidden = true;
    document.getElementById("video").hidden = false;
    send("error");
  };
  const ready = () => {
    if (!instance || !sceneReady || stopped) return;
    clearTimeout(startupTimeout);
    status.hidden = true;
    send("ready");
  };
  const quit = async () => {
    stopped = true;
    clearTimeout(startupTimeout);
    if (instance) await instance.Quit();
    send("stopped");
  };
  window.addEventListener("inventory-ready", () => { sceneReady = true; ready(); });
  window.addEventListener("message", event => {
    if (event.origin !== location.origin || event.source !== window.parent || event.data?.channel !== channel) return;
    if (event.data.type === "quit") quit().catch(() => send("stopped"));
    if (event.data.type === "fullscreen" && instance && !stopped) instance.SetFullscreen(1);
    if (event.data.type === "focus" && !stopped) canvas.focus({ preventScroll: true });
  });
  window.addEventListener("error", fail);
  window.addEventListener("unhandledrejection", fail);
  canvas.addEventListener("contextmenu", event => event.preventDefault());
  canvas.addEventListener("pointerdown", () => canvas.focus({ preventScroll: true }));
  config.showBanner = (text, type) => { if (type === "error") fail(); else console.warn(text); };
  const loader = document.createElement("script");
  loader.src = config.loader;
  loader.onerror = fail;
  loader.onload = () => {
    if (stopped) return;
    startupTimeout = setTimeout(fail, 120000);
    createUnityInstance(canvas, config, value => {
      progress.value = value;
      send("progress", { value });
    }).then(async unity => {
      instance = unity;
      if (stopped) { await unity.Quit(); send("stopped"); return; }
      ready();
    }).catch(error => { console.error("Unity startup failed.", error); fail(); });
  };
  document.body.append(loader);
})();
