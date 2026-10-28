const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const { runInNewContext } = require('node:vm');
const { test } = require('node:test');
const source = readFileSync('Assets/WebGLTemplates/Inventory/player.js', 'utf8');

function harness() {
  const listeners = {}, sent = [], elements = {};
  const parent = { postMessage: value => sent.push(value) };
  const element = id => elements[id] ??= { hidden: false, textContent: '', addEventListener() {}, setAttribute() {}, focus() {} };
  let loader, resolveInstance, quitCalls = 0;
  const instance = { Quit: async () => { quitCalls++; } };
  const window = { parent, inventoryConfig: { loader: 'loader.js' }, addEventListener: (type, callback) => { listeners[type] = callback; } };
  const document = { getElementById: element, createElement: () => ({}), body: { append: value => { loader = value; } } };
  runInNewContext(source, {
    window, document, location: { origin: 'https://pktony.github.io' }, console,
    setTimeout: () => 1, clearTimeout() {},
    createUnityInstance: () => new Promise(resolve => { resolveInstance = resolve; }),
  });
  return { sent, elements, listeners, parent, instance, load: () => loader.onload(), resolve: () => resolveInstance(instance), quitCalls: () => quitCalls };
}

test('ready requires both Unity creation and the inventory scene handshake', async () => {
  const h = harness(); h.load(); h.resolve(); await Promise.resolve();
  assert.equal(h.sent.some(x => x.type === 'ready'), false);
  h.listeners['inventory-ready']();
  assert.equal(h.sent.filter(x => x.type === 'ready').length, 1);
  assert.equal(h.elements.status.hidden, true);
});

test('scene-first startup also waits for createUnityInstance', async () => {
  const h = harness(); h.load(); h.listeners['inventory-ready']();
  assert.equal(h.sent.some(x => x.type === 'ready'), false);
  h.resolve(); await Promise.resolve();
  assert.equal(h.sent.some(x => x.type === 'ready'), true);
});

test('a quit during startup never publishes ready and quits a late instance', async () => {
  const h = harness(); h.load();
  h.listeners.message({ origin: 'https://pktony.github.io', source: h.parent, data: { channel: 'modular-grid-inventory', type: 'quit' } });
  h.resolve(); h.listeners['inventory-ready'](); await Promise.resolve(); await Promise.resolve();
  assert.equal(h.quitCalls(), 1);
  assert.equal(h.sent.some(x => x.type === 'ready'), false);
});

test('foreign messages cannot quit the player', async () => {
  const h = harness(); h.load(); h.resolve(); await Promise.resolve();
  h.listeners.message({ origin: 'https://example.com', source: h.parent, data: { channel: 'modular-grid-inventory', type: 'quit' } });
  assert.equal(h.quitCalls(), 0);
});
