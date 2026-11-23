// Dependency-free headless Microsoft Edge driver over the Chrome DevTools Protocol (Windows node v26 global WebSocket),
// for the Stage 4 owner UX repair (Phase A). It is the committed ../browser-driver/cdp.mjs pattern plus real keyboard
// input, clipped screenshots and a minimal PNG decoder for pixel checks. It never prints the capability: every string
// that leaves this module goes through redact().
import { spawn } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import zlib from 'node:zlib';

export const EDGE = 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';
export const redact = (s) => String(s).replace(/\/s\/[0-9a-f]{64}\//g, '/s/<capability>/');
export const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

export async function reachable(url, timeoutMs = 15000) {
  const end = Date.now() + timeoutMs;
  while (Date.now() < end) {
    try {
      const c = new AbortController();
      const t = setTimeout(() => c.abort(), 2000);
      const r = await fetch(url, { signal: c.signal });
      clearTimeout(t);
      if (r.status === 200) return true;
    } catch { /* WSL loopback relay not ready yet */ }
    await sleep(250);
  }
  return false;
}

// Decodes an 8-bit, non-interlaced RGB or RGBA PNG (what Page.captureScreenshot returns) into { width, height, px(x, y) }.
export function decodePng(bytes) {
  let at = 8, width = 0, height = 0, colourType = 0;
  const idat = [];
  while (at < bytes.length) {
    const length = bytes.readUInt32BE(at);
    const type = bytes.toString('ascii', at + 4, at + 8);
    const data = bytes.subarray(at + 8, at + 8 + length);
    if (type === 'IHDR') {
      width = data.readUInt32BE(0); height = data.readUInt32BE(4);
      if (data[8] !== 8 || data[12] !== 0) throw new Error('unsupported PNG (bit depth or interlace)');
      colourType = data[9];
    } else if (type === 'IDAT') idat.push(data);
    else if (type === 'IEND') break;
    at += 12 + length;
  }
  const channels = colourType === 6 ? 4 : colourType === 2 ? 3 : 0;
  if (!channels) throw new Error('unsupported PNG colour type ' + colourType);
  const raw = zlib.inflateSync(Buffer.concat(idat));
  const stride = width * channels;
  const out = Buffer.alloc(height * stride);
  for (let y = 0; y < height; y++) {
    const filter = raw[y * (stride + 1)];
    const line = raw.subarray(y * (stride + 1) + 1, (y + 1) * (stride + 1));
    for (let x = 0; x < stride; x++) {
      const a = x >= channels ? out[y * stride + x - channels] : 0;
      const b = y > 0 ? out[(y - 1) * stride + x] : 0;
      const c = x >= channels && y > 0 ? out[(y - 1) * stride + x - channels] : 0;
      let v = line[x];
      if (filter === 1) v += a;
      else if (filter === 2) v += b;
      else if (filter === 3) v += (a + b) >> 1;
      else if (filter === 4) { const p = a + b - c, pa = Math.abs(p - a), pb = Math.abs(p - b), pc = Math.abs(p - c); v += pa <= pb && pa <= pc ? a : pb <= pc ? b : c; }
      out[y * stride + x] = v & 255;
    }
  }
  return { width, height, px: (x, y) => [0, 1, 2].map((i) => out[y * stride + x * channels + i]) };
}

export async function launch() {
  const profile = fs.mkdtempSync(path.join(os.tmpdir(), 'ownerux-edge-'));
  const proc = spawn(EDGE, ['--headless=new', '--remote-debugging-port=0', '--user-data-dir=' + profile, '--no-first-run',
    '--no-default-browser-check', '--disable-extensions', '--disable-sync', '--disable-features=msEdgeSidebarV2', 'about:blank'],
    { stdio: 'ignore' });
  const portFile = path.join(profile, 'DevToolsActivePort');
  let text = '';
  for (let i = 0; i < 200 && !text; i++) { await sleep(100); try { text = fs.readFileSync(portFile, 'utf8'); } catch { text = ''; } }
  if (!text) throw new Error('Edge wrote no DevToolsActivePort');
  const [port, wsPath] = text.trim().split(/\r?\n/);
  const ws = new WebSocket('ws://127.0.0.1:' + port + wsPath);
  await new Promise((resolve, reject) => { ws.onopen = resolve; ws.onerror = reject; });
  let id = 0;
  const pending = new Map();
  const listeners = [];
  ws.onmessage = (m) => {
    const msg = JSON.parse(m.data);
    if (msg.id) {
      const p = pending.get(msg.id);
      pending.delete(msg.id);
      if (msg.error) p.reject(new Error(msg.error.message)); else p.resolve(msg.result);
    } else for (const l of listeners) l(msg);
  };
  const send = (method, params = {}, sessionId) => new Promise((resolve, reject) => {
    const i = ++id;
    pending.set(i, { resolve, reject });
    ws.send(JSON.stringify({ id: i, method, params, sessionId }));
  });
  const { targetId } = await send('Target.createTarget', { url: 'about:blank' });
  const { sessionId } = await send('Target.attachToTarget', { targetId, flatten: true });
  const S = (method, params) => send(method, params, sessionId);
  const log = { console: [], exceptions: [], logEntries: [], http5xx: [], requests: 0, statuses: {}, posts: [] };
  listeners.push((msg) => {
    if (msg.sessionId !== sessionId) return;
    const p = msg.params;
    if (msg.method === 'Runtime.consoleAPICalled' && (p.type === 'error' || p.type === 'warning' || p.type === 'assert'))
      log.console.push(p.type + ': ' + redact(p.args.map((a) => a.value ?? a.description ?? '').join(' ')));
    else if (msg.method === 'Runtime.exceptionThrown') log.exceptions.push(redact(JSON.stringify(p.exceptionDetails).slice(0, 600)));
    else if (msg.method === 'Log.entryAdded' && (p.entry.level === 'error' || p.entry.level === 'warning'))
      log.logEntries.push(p.entry.level + ' ' + p.entry.source + ': ' + redact(p.entry.text));
    else if (msg.method === 'Network.requestWillBeSent' && p.request.method === 'POST')
      log.posts.push(redact(p.request.url).replace(/^.*\/s\/<capability>\//, ''));
    else if (msg.method === 'Network.responseReceived') {
      log.requests++;
      const s = p.response.status;
      log.statuses[s] = (log.statuses[s] || 0) + 1;
      if (s >= 500) log.http5xx.push(s + ' ' + redact(p.response.url));
    }
  });
  await S('Page.enable');
  await S('Runtime.enable');
  await S('Log.enable');
  await S('Network.enable');
  await S('Page.addScriptToEvaluateOnNewDocument', { source:
    "window.__csp=[];document.addEventListener('securitypolicyviolation',function(e){window.__csp.push(e.violatedDirective+' '+e.blockedURI);});" });

  const KEYS = { Escape: 27, Tab: 9, Enter: 13, ' ': 32 };
  const page = {
    log,
    async viewport(width, height) {
      await S('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
      await sleep(400);
    },
    async goto(url) { await S('Page.navigate', { url }); },
    async eval(expression) {
      const r = await S('Runtime.evaluate', { expression, awaitPromise: true, returnByValue: true });
      if (r.exceptionDetails) throw new Error('eval failed: ' + redact(JSON.stringify(r.exceptionDetails).slice(0, 500)));
      return r.result.value;
    },
    async waitFor(expression, what, timeout = 20000) {
      const end = Date.now() + timeout;
      while (Date.now() < end) {
        try { if (await page.eval(expression)) return true; } catch { /* page still loading */ }
        await sleep(100);
      }
      throw new Error('timed out waiting for ' + what);
    },
    async clickAt(x, y) {
      await S('Input.dispatchMouseEvent', { type: 'mouseMoved', x, y });
      await S('Input.dispatchMouseEvent', { type: 'mousePressed', x, y, button: 'left', clickCount: 1 });
      await S('Input.dispatchMouseEvent', { type: 'mouseReleased', x, y, button: 'left', clickCount: 1 });
    },
    // A real mouse click at the centre of the first element matching the selector (scrolled into view only if needed).
    async click(selector) {
      const r = await page.eval(`(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null;
        e.scrollIntoView({ block: 'nearest', inline: 'nearest' }); const b = e.getBoundingClientRect();
        return { x: b.left + b.width / 2, y: b.top + b.height / 2, w: b.width, h: b.height, disabled: !!e.disabled }; })()`);
      if (!r) throw new Error('no element ' + selector);
      if (r.w === 0 || r.h === 0) throw new Error('element not visible ' + selector);
      if (r.disabled) throw new Error('element disabled ' + selector);
      await page.clickAt(r.x, r.y);
      await sleep(80);
    },
    // A real key press through the browser's input pipeline (so a modal dialog's own Escape handling runs).
    async key(key) {
      const code = key === ' ' ? 'Space' : key;
      const vk = KEYS[key];
      await S('Input.dispatchKeyEvent', { type: 'keyDown', key, code, windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk, text: key === ' ' ? ' ' : undefined });
      await S('Input.dispatchKeyEvent', { type: 'keyUp', key, code, windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk });
      await sleep(120);
    },
    async idle() { await page.waitFor("!document.documentElement.classList.contains('busy')", 'the page to be idle', 30000); await sleep(150); },
    async shot(file) {
      const r = await S('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
      const bytes = Buffer.from(r.data, 'base64');
      fs.writeFileSync(file, bytes);
      return bytes.length;
    },
    async capture(clip) {
      const r = await S('Page.captureScreenshot', { format: 'png', clip: { ...clip, scale: 1 }, captureBeyondViewport: false });
      return decodePng(Buffer.from(r.data, 'base64'));
    },
    async close() {
      try { await send('Browser.close'); } catch { /* already closing */ }
      await sleep(800);
      try { fs.rmSync(profile, { recursive: true, force: true }); } catch { /* Edge may still hold a file briefly */ }
      if (proc.exitCode === null) { /* only our own child, never Edge by name */ try { proc.kill(); } catch { } }
    },
  };
  return page;
}
