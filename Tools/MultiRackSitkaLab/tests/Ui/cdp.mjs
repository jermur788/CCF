// Minimal Chrome DevTools Protocol driver. No npm packages: Node's built-in WebSocket + child_process only.
import { spawn } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';

export class Browser {
  static async launch({ chromium = process.env.CHROMIUM || '/usr/bin/chromium', width = 1440, height = 900 } = {}) {
    const userDir = fs.mkdtempSync(path.join(os.tmpdir(), 'sitkalab-chrome-'));
    const proc = spawn(chromium, [
      '--headless=new', '--remote-debugging-port=0', `--user-data-dir=${userDir}`, '--no-sandbox', '--disable-gpu',
      '--no-first-run', '--no-default-browser-check', '--disable-extensions', '--disable-background-networking',
      `--window-size=${width},${height}`, 'about:blank',
    ], { stdio: 'ignore' });
    const portFile = path.join(userDir, 'DevToolsActivePort');
    for (let i = 0; i < 200 && !fs.existsSync(portFile); i++) await new Promise((r) => setTimeout(r, 50));
    if (!fs.existsSync(portFile)) { proc.kill(); throw new Error('Chromium did not start (no DevToolsActivePort)'); }
    const [port, wsPath] = fs.readFileSync(portFile, 'utf8').trim().split('\n');
    const b = new Browser(proc, userDir);
    await b.connect(`ws://127.0.0.1:${port}${wsPath}`);
    return b;
  }

  constructor(proc, userDir) { this.proc = proc; this.userDir = userDir; this.id = 0; this.pending = new Map(); this.listeners = []; }

  connect(url) {
    return new Promise((resolve, reject) => {
      this.ws = new WebSocket(url);
      this.ws.onopen = () => resolve();
      this.ws.onerror = (e) => reject(new Error('CDP websocket error'));
      this.ws.onmessage = (ev) => {
        const msg = JSON.parse(ev.data);
        if (msg.id && this.pending.has(msg.id)) {
          const { res, rej } = this.pending.get(msg.id); this.pending.delete(msg.id);
          msg.error ? rej(new Error(`${msg.error.message}`)) : res(msg.result);
        } else if (msg.method) for (const l of this.listeners) l(msg);
      };
    });
  }

  send(method, params = {}, sessionId) {
    const id = ++this.id;
    return new Promise((res, rej) => {
      this.pending.set(id, { res, rej });
      this.ws.send(JSON.stringify({ id, method, params, ...(sessionId ? { sessionId } : {}) }));
    });
  }

  async version() { return this.send('Browser.getVersion'); }

  async newPage(width, height) {
    const { targetId } = await this.send('Target.createTarget', { url: 'about:blank' });
    const { sessionId } = await this.send('Target.attachToTarget', { targetId, flatten: true });
    const page = new Page(this, sessionId, targetId);
    await page.send('Page.enable'); await page.send('Runtime.enable');
    await page.viewport(width, height);
    return page;
  }

  async close() {
    try { await this.send('Browser.close'); } catch { /* already closing */ }
    try { this.ws.close(); } catch { /* ignore */ }
    await new Promise((r) => { this.proc.once('exit', r); setTimeout(r, 3000); });
    try { this.proc.kill(); } catch { /* ignore */ }
    fs.rmSync(this.userDir, { recursive: true, force: true });
  }
}

export class Page {
  constructor(browser, sessionId, targetId) { this.b = browser; this.sessionId = sessionId; this.targetId = targetId; }
  send(method, params = {}) { return this.b.send(method, params, this.sessionId); }

  async viewport(width, height) {
    await this.send('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
  }

  async goto(url) {
    const loaded = new Promise((res) => { const h = (m) => { if (m.sessionId === this.sessionId && m.method === 'Page.loadEventFired') { this.b.listeners = this.b.listeners.filter((x) => x !== h); res(); } }; this.b.listeners.push(h); });
    await this.send('Page.navigate', { url });
    await loaded;
  }

  async eval(expression) {
    const r = await this.send('Runtime.evaluate', { expression, awaitPromise: true, returnByValue: true });
    if (r.exceptionDetails) throw new Error('page eval failed: ' + (r.exceptionDetails.exception?.description || r.exceptionDetails.text));
    return r.result.value;
  }

  async waitFor(expression, { timeout = 8000, what = expression } = {}) {
    const t0 = Date.now();
    for (;;) {
      let v; try { v = await this.eval(expression); } catch { v = false; }
      if (v) return v;
      if (Date.now() - t0 > timeout) throw new Error('timed out waiting for: ' + what);
      await new Promise((r) => setTimeout(r, 40));
    }
  }

  /** Real input events through the browser's input pipeline (not element.click()). */
  async mouse(type, x, y, { button = 'left', clickCount = 1, modifiers = 0, deltaY, buttons } = {}) {
    await this.send('Input.dispatchMouseEvent', { type, x, y, button: type === 'mouseMoved' && !buttons ? 'none' : button, buttons: buttons ?? 0, clickCount, modifiers, ...(deltaY !== undefined ? { deltaX: 0, deltaY } : {}) });
  }

  async clickAt(x, y, opts = {}) {
    await this.mouse('mouseMoved', x, y, { modifiers: opts.modifiers || 0 });
    await this.mouse('mousePressed', x, y, { ...opts, buttons: 1 });
    await this.mouse('mouseReleased', x, y, { ...opts, buttons: 0 });
  }

  /** Centre of an element in viewport coordinates. scroll=false never moves the page (needed when comparing positions over time). */
  async center(selector, { scroll = true } = {}) {
    const r = await this.eval(`(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null; if (${scroll}) e.scrollIntoView({block:'center',inline:'center'}); const b = e.getBoundingClientRect(); return {x: b.left + b.width/2, y: b.top + b.height/2, w: b.width, h: b.height}; })()`);
    if (!r) throw new Error('no element for ' + selector);
    return r;
  }

  async click(selector, opts = {}) { const c = await this.center(selector); await this.clickAt(c.x, c.y, opts); }

  async clickTree(id, opts = {}) {
    const c = await this.center(`g.tree[data-tree-id="${id}"] circle.hit`);
    await this.clickAt(c.x, c.y, opts);
  }

  async type(selector, text) {
    await this.click(selector);
    await this.eval(`document.querySelector(${JSON.stringify(selector)}).select()`);
    await this.send('Input.insertText', { text });
  }

  async screenshot(file) {
    const { data } = await this.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
    fs.writeFileSync(file, Buffer.from(data, 'base64'));
    return file;
  }

  async close() { try { await this.b.send('Target.closeTarget', { targetId: this.targetId }); } catch { /* ignore */ } }
}
