// CCF Multi 2D — Scots Pine & Site Moisture Part 2 real-browser acceptance driver (Microsoft Edge over CDP).
// Modelled on tests/map-first-lenses-ui/drive-map-first-lenses.mjs. Phases:
//   main: New Woodland moisture picker (NeutralLegacy default, StagnantPoorBog caution), the 0.64 ha Dry Ridge & Moist Slope
//         preset with pine + Sitka at Year 0 and the Two-Zone label, five live lenses, the pine inspector, Plant Scots pine
//         through the Plant menu (shelter + spot clear, EUR 0.45 sapling quote), fell a pine as deadwood, Execute, Advance,
//         the Scots pine History card, listener-leak check, then Save v3 through the UI.
//   load: a NEW server process loads the slot; pine trees, the planted pine juvenile, pine deadwood and the two-zone site
//         persist.
'use strict';
import { spawn } from 'node:child_process';
import { mkdirSync, readFileSync, writeFileSync, rmSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

const args = process.argv.slice(2);
const opt = (name, fallback = null) => {
  const i = args.indexOf('--' + name);
  return i >= 0 && i + 1 < args.length ? args[i + 1] : fallback;
};
const tag = opt('tag', 'vp');
const mode = opt('mode', 'main'); // 'main' | 'load'
const width = Number(opt('width', '1280'));
const height = Number(opt('height', '720'));
const slotName = opt('slot', 'scots-pine-v3');
const url = opt('url');
const out = opt('out', '.');
const shots = opt('shots', '0') === '1';
const vpLabel = `[${width}x${height}]`;
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';
const expectedPath = join(out, `${tag}-expected.json`);
const TWO_ZONE = 'Two-Zone (Dry Ridge & Moist Slope)';
const PRESET = 'fm-064-scots-pine-sitka-two-zone-v1';

let failures = 0, passes = 0;
const log = (line) => console.log(line);
function check(ok, title) {
  if (ok) { passes++; log(`PASS ${vpLabel} ${tag}/${mode}: ${title}`); }
  else { failures++; log(`FAIL ${vpLabel} ${tag}/${mode}: ${title}`); }
}

class Cdp {
  constructor(ws) { this.ws = ws; this.id = 0; this.pending = new Map(); this.events = []; }
  static async connect(wsUrl) {
    const ws = new WebSocket(wsUrl);
    await new Promise((resolve, reject) => {
      ws.addEventListener('open', resolve, { once: true });
      ws.addEventListener('error', (e) => reject(new Error('websocket failed: ' + (e.message || 'error'))), { once: true });
    });
    const cdp = new Cdp(ws);
    ws.addEventListener('message', (event) => {
      const message = JSON.parse(event.data);
      if (message.id && cdp.pending.has(message.id)) {
        const { resolve, reject } = cdp.pending.get(message.id);
        cdp.pending.delete(message.id);
        if (message.error) reject(new Error(message.error.message)); else resolve(message.result);
      } else if (message.method) cdp.events.push(message);
    });
    return cdp;
  }
  send(method, params = {}) {
    const id = ++this.id;
    return new Promise((resolve, reject) => {
      this.pending.set(id, { resolve, reject });
      this.ws.send(JSON.stringify({ id, method, params }));
    });
  }
  close() { try { this.ws.close(); } catch { /* ignore */ } }
}

async function freePort() {
  const net = await import('node:net');
  return new Promise((resolve) => {
    const server = net.createServer();
    server.listen(0, '127.0.0.1', () => { const { port } = server.address(); server.close(() => resolve(port)); });
  });
}

async function launch() {
  const port = await freePort();
  const profile = join(tmpdir(), 'ccf-scotspine-edge-' + process.pid + '-' + Date.now());
  mkdirSync(profile, { recursive: true });
  const child = spawn(EDGE, [
    '--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', '--disable-extensions',
    '--hide-scrollbars', '--force-device-scale-factor=1', `--window-size=${width},${height}`,
    '--remote-debugging-port=' + port, '--user-data-dir=' + profile, 'about:blank',
  ], { stdio: 'ignore', detached: false });
  let target = null;
  for (let attempt = 0; attempt < 200 && !target; attempt++) {
    await new Promise((r) => setTimeout(r, 100));
    try {
      const list = await (await fetch('http://127.0.0.1:' + port + '/json/list')).json();
      target = list.find((t) => t.type === 'page');
    } catch { /* wait */ }
  }
  if (!target) {
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    throw new Error('Edge did not expose a page target');
  }
  const cdp = await Cdp.connect(target.webSocketDebuggerUrl);
  return { cdp, child, profile };
}

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function setViewport(cdp, w, h) {
  await cdp.send('Emulation.setDeviceMetricsOverride', { width: w, height: h, deviceScaleFactor: 1, mobile: false });
  await sleep(180);
}

async function evaluate(cdp, expression, commandLineApi = false) {
  const result = await cdp.send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true, includeCommandLineAPI: commandLineApi });
  if (result.exceptionDetails) throw new Error('page error: ' + (result.exceptionDetails.exception?.description || result.exceptionDetails.text));
  return result.result.value;
}

async function waitFor(cdp, expression, what, timeoutMs = 30000) {
  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    if (await evaluate(cdp, 'Boolean(' + expression + ')').catch(() => false)) return true;
    await sleep(100);
  }
  const diag = await evaluate(cdp, `(() => ({
    busy: document.documentElement.classList.contains('busy'),
    year: document.getElementById('year-value')?.textContent,
    pane: document.getElementById('side')?.getAttribute('data-pane'),
    alert: document.getElementById('alert')?.textContent?.slice(0, 300) || '',
    woodland: document.getElementById('woodland-status')?.textContent || '',
    plant: document.getElementById('plant-result')?.textContent?.slice(0, 300) || '',
  }))()`).catch(() => null);
  throw new Error('timed out waiting for ' + what + ' | diag=' + JSON.stringify(diag));
}

async function rectOf(cdp, selector) {
  return evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null;
    const r = e.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2, w: r.width, h: r.height,
    visible: r.width > 0 && r.height > 0 && getComputedStyle(e).visibility !== 'hidden' }; })()`);
}

async function mouseClickAt(cdp, x, y) {
  const base = { x, y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(160);
}

async function click(cdp, selector) {
  await waitFor(cdp, "!document.documentElement.classList.contains('busy')", 'page idle before clicking ' + selector, 15000);
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (e && e.scrollIntoView) e.scrollIntoView({ block: 'center', inline: 'nearest' }); })()`);
  await sleep(80);
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  await mouseClickAt(cdp, rect.x, rect.y);
}

async function pressKey(cdp, key) {
  const vk = key === 'Escape' ? 27 : key.toUpperCase().charCodeAt(0);
  const params = { key, code: key === 'Escape' ? 'Escape' : undefined, text: key.length === 1 ? key : undefined,
    windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk };
  await cdp.send('Input.dispatchKeyEvent', { type: 'keyDown', ...params });
  await cdp.send('Input.dispatchKeyEvent', { type: 'keyUp', ...params });
  await sleep(160);
}

async function setValue(cdp, selector, value, eventName = 'change') {
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.value = ${JSON.stringify(value)};
    e.dispatchEvent(new Event(${JSON.stringify(eventName)}, { bubbles: true })); })()`);
  await sleep(120);
}

async function typeInto(cdp, selector, text) {
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.focus(); e.value = ${JSON.stringify(text)}; e.dispatchEvent(new Event('input', { bubbles: true })); })()`);
  await sleep(80);
}

async function shot(cdp, name) {
  if (!shots) return;
  await sleep(140);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, `${tag}-${mode}-${name}-${width}x${height}.png`);
  writeFileSync(file, Buffer.from(data.data, 'base64'));
  log('SHOT ' + file);
}

async function fetchJson(cdp, path, body) {
  const expression = `(async () => { const r = await fetch(${JSON.stringify(path)}, ${body === undefined
    ? `{ method: 'GET', cache: 'no-store', credentials: 'omit' }`
    : `{ method: 'POST', cache: 'no-store', credentials: 'omit', headers: { 'Content-Type': 'application/json' }, body: ${JSON.stringify(JSON.stringify(body))} }`});
    const raw = await r.text();
    let p = null; try { p = JSON.parse(raw); } catch {}
    return { status: r.status, bytes: new TextEncoder().encode(raw).length, payload: p }; })()`;
  return evaluate(cdp, expression);
}

const deliberateRefusal = (entry) => entry.source === 'network' && /status of (400|409)/.test(entry.text || '');

function makeAuditor(cdp) {
  const badNetwork = [];
  const consoleIssues = [];
  let initialOk = 0;
  const collect = (isInitial = false) => {
    for (const e of cdp.events) {
      if (e.method === 'Runtime.exceptionThrown') consoleIssues.push(e.params.exceptionDetails?.exception?.description || e.params.exceptionDetails?.text || 'exception');
      if (e.method === 'Runtime.consoleAPICalled' && e.params.type === 'error') consoleIssues.push(JSON.stringify(e.params.args).slice(0, 300));
      if (e.method === 'Log.entryAdded' && e.params.entry.level === 'error' && !deliberateRefusal(e.params.entry)) consoleIssues.push(e.params.entry.text);
      if (e.method === 'Network.responseReceived') {
        const st = e.params.response?.status;
        if (st === 404 || st >= 500) badNetwork.push(`${st} ${e.params.response?.url}`);
        else if (isInitial && st === 200) initialOk++;
      }
      if (e.method === 'Network.loadingFailed' && !e.params.canceled) badNetwork.push(`LOAD_FAIL ${e.params.errorText || 'unknown'}`);
    }
    cdp.events.length = 0;
  };
  return { collect, badNetwork, consoleIssues, initialOk: () => initialOk };
}

// Listener counts on the static page nodes the Part 2 code touches (re-renders must never add listeners).
async function listenerCount(cdp) {
  return evaluate(cdp, `(() => {
    const nodes = [window, document, document.getElementById('map'), document.getElementById('lens-bar'),
      document.getElementById('plant-species'), document.getElementById('woodland-site-moisture'), document.getElementById('lens-slots-list'),
      document.getElementById('insp-facts'), document.getElementById('legend')].filter(Boolean);
    return nodes.reduce((sum, n) => sum + Object.values(getEventListeners(n)).reduce((s, list) => s + list.length, 0), 0);
  })()`, true);
}

async function standLane(cdp, laneId) {
  const r = await fetchJson(cdp, 'api/multi/stand?lanes=' + laneId);
  return r.payload;
}

async function setLens(cdp, lens) {
  await click(cdp, `[data-lens-btn="${lens}"]`);
  await waitFor(cdp, `document.querySelector('[data-lens-btn="${lens}"]').getAttribute('aria-pressed') === 'true'`, 'lens ' + lens);
  await sleep(150);
}

async function slotText(cdp, slotId) {
  return evaluate(cdp, `(() => { const e = document.querySelector('[data-extension-slot="${slotId}"]');
    return e ? { live: e.getAttribute('data-slot-live'), text: e.textContent } : null; })()`);
}

// Selects a living Scots pine of the active lane by clicking its crown; resolves crown overlaps through the pick list.
async function selectPine(cdp, laneId, avoidIds = []) {
  const target = await evaluate(cdp, `(() => {
    const avoid = new Set(${JSON.stringify(avoidIds)});
    const crowns = [...document.querySelectorAll('g[data-lane-crowns="${laneId}"] use.crown-art[data-species="scots-pine"], #crowns > use.crown-art[data-species="scots-pine"]')];
    const sized = crowns.map((c) => { const r = c.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2, w: r.width }; })
      .filter((c) => c.w > 6 && c.x > 40 && c.y > 140 && c.x < window.innerWidth - 40 && c.y < window.innerHeight - 40);
    return sized.length > 0 ? sized[Math.floor(sized.length / 2)] : null; })()`);
  if (!target) throw new Error('no visible Scots pine crown in ' + laneId);
  await mouseClickAt(cdp, target.x, target.y);
  await waitFor(cdp, "!document.getElementById('inspector-body').hidden || !document.getElementById('overlap').hidden", 'a tree selected');
  const role = await evaluate(cdp, "document.getElementById('insp-role').textContent");
  if (!role.includes('Scots pine')) {
    const pick = await evaluate(cdp, `(() => { const avoid = new Set(${JSON.stringify(avoidIds)});
      const li = [...document.querySelectorAll('#overlap-list li')].find((l) => l.textContent.includes('Scots pine') && !avoid.has(l.querySelector('[data-pick]')?.getAttribute('data-pick')));
      return li ? li.querySelector('[data-pick]').getAttribute('data-pick') : null; })()`);
    if (!pick) throw new Error('clicked crown did not select a Scots pine: ' + role);
    await click(cdp, `[data-pick="${pick}"]`);
  }
  await waitFor(cdp, "document.getElementById('insp-role').textContent.includes('Scots pine')", 'Scots pine selected');
  return evaluate(cdp, "document.getElementById('insp-id').textContent.trim()");
}

// A planting point in the brightest full interior ground cell of the lane (Core cell light from the lane stand read), in
// screen pixels, >= 1.3 m from every living stem (crown centre) and visible. Returns the Core light of that cell too.
async function brightPlantingPoint(cdp, laneId) {
  return evaluate(cdp, `(async () => {
    const p = await (await fetch('api/multi/stand?lanes=${laneId}', { cache: 'no-store', credentials: 'omit' })).json();
    const area = p.stage4.areas[0];
    const rows = area.groundCompact || (area.ground || []).map((g) => [g.column, g.row, g.centreXM, g.centreZM, g.centreU, g.centreV, g.clippedAreaM2, g.lightIndex]);
    const map = document.getElementById('map');
    const ctm = map.getScreenCTM();
    const scale = ctm.a;
    const g = document.querySelector('g[data-lane-crowns="${laneId}"]');
    const m = g ? /translate\\(([-\\d.]+)[ ,]+([-\\d.]+)\\)/.exec(g.getAttribute('transform') || '') : null;
    const offX = m ? Number(m[1]) : 0, offY = m ? Number(m[2]) : 0;
    const stems = [...document.querySelectorAll('use.crown-art')].map((c) => { const r = c.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; });
    const need = 1.3 * scale;
    const cells = rows.filter((r) => r[6] >= 6.2).sort((a, b) => b[7] - a[7]);
    for (const r of cells) {
      for (const dy of [0, -0.6, 0.6, -0.9, 0.9]) for (const dx of [0, -0.6, 0.6, -0.9, 0.9]) {
        const pt = map.createSVGPoint();
        pt.x = r[4] + offX + dx; pt.y = r[5] + offY + dy;
        const s = pt.matrixTransform(ctm);
        if (s.x < 60 || s.y < 160 || s.x > window.innerWidth - 60 || s.y > window.innerHeight - 60) continue;
        if (stems.every((q) => (q.x - s.x) * (q.x - s.x) + (q.y - s.y) * (q.y - s.y) >= need * need)) return { x: s.x, y: s.y, light: r[7], cell: r[0] + ',' + r[1] };
      }
    }
    return null; })()`);
}

async function runMainPhase(cdp) {
  const audit = makeAuditor(cdp);
  audit.collect(true);
  check(audit.initialOk() >= 18 && audit.badNetwork.length === 0 && audit.consoleIssues.length === 0,
    `SP2-UI-01 initial /multi load clean (${audit.initialOk()} HTTP 200 assets, console=${audit.consoleIssues.length}, net=${audit.badNetwork.length})`);

  // 1. New Woodland moisture picker: NeutralLegacy default on existing presets; StagnantPoorBog caution from Core.
  await click(cdp, '#woodland-open');
  await waitFor(cdp, "document.getElementById('woodland-dialog').open && document.querySelectorAll('#woodland-site-moisture option').length === 6",
    'New Woodland options with six site-moisture choices');
  const picker = await evaluate(cdp, `(() => { const s = document.getElementById('woodland-site-moisture');
    return { value: s.value, disabled: s.disabled, options: [...s.options].map((o) => o.value),
      choice: document.querySelector('input[name="woodland-choice"]:checked')?.value }; })()`);
  check(picker.value === 'NeutralLegacy' && !picker.disabled && picker.choice === 'preset:fm-064-sitka-edge-broadleaves-v1' &&
    picker.options.join(',') === 'NeutralLegacy,DryFreeDraining,FreshMoistMineral,SeasonallyWetAerated,WetMovingAlluvial,StagnantPoorBog',
    `SP2-UI-02 picker defaults to NeutralLegacy on the existing preset (${picker.choice}) with the five classes offered (${picker.options.length - 1} + neutral)`);
  await click(cdp, '#wc-preset-fm-064-pure-sitka-v1');
  await sleep(250);
  const pure = await evaluate(cdp, "({ value: document.getElementById('woodland-site-moisture').value, disabled: document.getElementById('woodland-site-moisture').disabled })");
  check(pure.value === 'NeutralLegacy' && !pure.disabled, 'SP2-UI-03 pure Sitka preset keeps the NeutralLegacy default');
  await setValue(cdp, '#woodland-site-moisture', 'StagnantPoorBog');
  await waitFor(cdp, "!document.getElementById('woodland-site-caution').hidden", 'bog caution shown');
  const caution = await evaluate(cdp, "document.getElementById('woodland-site-caution').textContent");
  check(/Habitat protection caution/.test(caution) && /never implies planting approval/.test(caution),
    `SP2-UI-04 StagnantPoorBog shows the Core habitat-protection caution ("${caution.slice(0, 60)}…")`);
  await setValue(cdp, '#woodland-site-moisture', 'DryFreeDraining');
  await waitFor(cdp, "document.getElementById('woodland-site-caution').hidden", 'caution hidden for a non-bog class');
  check(true, 'SP2-UI-05 the caution is hidden again for a non-bog class');

  await click(cdp, `#wc-preset-${PRESET.replace(/[^a-z0-9]+/gi, '-')}`);
  await waitFor(cdp, `document.getElementById('woodland-site-moisture').disabled && document.getElementById('woodland-site-note').textContent.includes(${JSON.stringify(TWO_ZONE)}) &&
    document.querySelector('[data-preview-site]') !== null`, 'two-zone preset locks the picker and previews its site');
  const preview = await evaluate(cdp, "document.getElementById('woodland-preview').textContent");
  check(preview.includes('1344 living stems') && preview.includes(TWO_ZONE) && /\d+ Scots pine at Year 0/.test(preview) && preview.includes(' P)'),
    `SP2-UI-06 Dry Ridge & Moist Slope preset previews 1,344 stems, pine per lane and the Two-Zone site`);
  await click(cdp, '#woodland-economy');
  await click(cdp, '#woodland-create');
  await waitFor(cdp, "!document.getElementById('woodland-dialog').open && document.getElementById('year-value').textContent.trim() === '0'", 'new two-zone woodland at Year 0', 60000);
  await sleep(500);
  await shot(cdp, 'two-zone-year0');

  // 2. Year 0: pine + Sitka present, Two-Zone label, pine crown sprites, zones and legend.
  const laneA = await standLane(cdp, 'MA-A');
  const sp = laneA.ecology?.scotsPine;
  const speciesA = laneA.ecology?.speciesByTreeId || {};
  const treesA = laneA.view.areas[0].trees.filter((t) => t.isLiving);
  const pineA = treesA.filter((t) => speciesA[t.treeId] === 'scots-pine').length;
  check(Boolean(sp) && sp.site.wholeStandLabel === TWO_ZONE && sp.site.dominantByAreaClass === 'DryFreeDraining' && sp.livingPineAdults > 0 &&
    pineA > 0 && treesA.length - pineA > 0,
    `SP2-UI-07 Year 0 has Scots pine (${sp?.livingPineAdults} woodland, ${pineA} in lane A) and Sitka (${treesA.length - pineA} in lane A); label "${sp?.site.wholeStandLabel}" never collapses to the dry tie winner`);
  const overview = (await fetchJson(cdp, 'api/multi/stand')).payload;
  const totalStems = (overview?.areas || []).reduce((s, a) => s + (a.minimapStems ? a.minimapStems.length : 0), 0);
  const minimapPine = (overview?.areas || []).reduce((s, a) => s + (a.minimapStems || []).filter((st) => st.s === 3).length, 0);
  check(totalStems === 1344 && minimapPine === sp.livingPineAdults,
    `SP2-UI-08 whole woodland ${totalStems} stems (1,344 expected); minimap codes ${minimapPine} pine distinctly`);
  await waitFor(cdp, "document.querySelectorAll('use.crown-art[data-species=\"scots-pine\"]').length > 0", 'pine crowns rendered');
  const sprites = await evaluate(cdp, `(() => { const pine = [...document.querySelectorAll('use.crown-art[data-species="scots-pine"]')];
    const circular = pine.every((u) => u.getAttribute('width') === u.getAttribute('height'));
    const rotations = new Set(pine.map((u) => (u.getAttribute('transform') || '').split(' ')[0]));
    return { count: pine.length, href: pine[0]?.getAttribute('href'), circular, rotations: rotations.size,
      sitka: document.querySelectorAll('use.crown-art[data-species="sitka-spruce"]').length }; })()`);
  check(sprites.count > 0 && sprites.href === '#ccf-pine' && sprites.circular && sprites.rotations > 5 && sprites.sitka > 0,
    `SP2-UI-09 circular straight-down pine crown sprites (${sprites.count} drawn, ${sprites.rotations} stable rotations) beside ${sprites.sitka} Sitka crowns`);
  const woodland = await evaluate(cdp, `({ label: document.querySelector('[data-site-label]')?.textContent || '',
    zones: document.querySelectorAll('[data-moisture-zone]').length, edge: document.querySelectorAll('[data-moisture-edge]').length,
    chips: [...document.querySelectorAll('#legend-compact-items [data-legend-chip]')].map((c) => c.getAttribute('data-legend-chip')),
    cols: getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length })`);
  check(woodland.label === TWO_ZONE && woodland.zones > 0 && woodland.chips.includes('pine') && woodland.chips.includes('moisture-zones') && woodland.cols === 2,
    `SP2-UI-10 Woodland lens: Two-Zone label, ${woodland.zones} moisture-zone tints, legend chips [${woodland.chips.join(', ')}], ${woodland.cols} columns (no 4th column)`);

  // 3. The five lenses carry live Scots pine / moisture readouts from Core.
  const listenersBefore = await listenerCount(cdp);
  await setLens(cdp, 'trees');
  const trees = await slotText(cdp, 'scots-pine-profile');
  check(trees?.live === 'true' && trees.text.includes('Living Scots pine adults: ' + sp.livingPineAdults) && trees.text.includes('2.5 m then 5.5 m'),
    'SP2-UI-11 Trees lens: live Scots pine profile (adult count, 2.5/5.5 m pruning lifts)');
  await setLens(cdp, 'regeneration');
  const regen = await slotText(cdp, 'soil-moisture-overlay');
  check(regen?.live === 'true' && regen.text.includes('Site: ' + TWO_ZONE) && regen.text.includes('Scots pine — Dry free-draining: establishment 1.00 · growth 1.00') &&
    regen.text.includes('Sitka spruce — Dry free-draining: establishment 0.70 · growth 0.35') && /seed year/.test(regen.text),
    'SP2-UI-12 Regeneration lens: site label, separate pine/Sitka modifiers per zone, seed-year status');
  await setLens(cdp, 'harvesting');
  const harvest = await slotText(cdp, 'scots-pine-timber-grader');
  check(harvest?.live === 'true' && harvest.text.includes('Sawlog €68/t') && harvest.text.includes('Small roundwood €45/t') &&
    harvest.text.includes('Pulp/chip €38/t') && harvest.text.includes('1.02 green t/m³'),
    'SP2-UI-13 Harvesting lens: Scots pine grades EUR 68/45/38 per t at 1.02 t/m3');
  await setLens(cdp, 'habitat');
  const habitat = await slotText(cdp, 'sensitive-wet-ground');
  check(habitat?.live === 'true' && habitat.text.includes('No stagnant poor bog') && habitat.text.includes('Scots pine deadwood logs: 0'),
    'SP2-UI-14 Habitat lens: no bog caution on this site, pine deadwood count');
  await setLens(cdp, 'woodland');
  let slowest = 0;
  for (const lens of ['trees', 'regeneration', 'harvesting', 'habitat', 'woodland']) {
    const t0 = Date.now();
    await setLens(cdp, lens);
    slowest = Math.max(slowest, Date.now() - t0);
  }
  const listenersAfter = await listenerCount(cdp);
  const drawn = await evaluate(cdp, "document.querySelectorAll('use.crown-art').length");
  check(listenersAfter === listenersBefore, `SP2-UI-15 no leaked listeners across lens cycles (${listenersBefore} → ${listenersAfter})`);
  check(slowest < 2500 && sp.livingPineAdults + 0 > 600,
    `SP2-UI-15b 1,344-stem woodland (${sp.livingPineAdults} pine) re-renders ${drawn} crowns per lens switch; slowest switch ${slowest} ms (< 2500 ms incl. CDP round trips)`);

  // 4. Pine inspector (Core detail: moisture, modifiers, GROWFOR growth, pruning, timber grade).
  await setLens(cdp, 'trees');
  const pineId = await selectPine(cdp, 'MA-A');
  await waitFor(cdp, "document.getElementById('insp-facts').textContent.includes('Timber grade now')", 'pine inspector Core detail');
  const insp = await evaluate(cdp, "document.getElementById('insp-facts').textContent");
  check(insp.includes('Site moisture') && (insp.includes('Dry free-draining') || insp.includes('Fresh moist mineral')) &&
    insp.includes('GROWFOR growth') && insp.includes('Pine pruning') && insp.includes('Timber grade now') && insp.includes('[Calibration]'),
    `SP2-UI-16 inspector shows ${pineId} site moisture, separate modifiers, GROWFOR growth, pruning and timber grade from Core`);
  await shot(cdp, 'pine-inspector');

  // 5. Fell that pine as deadwood (Leave as Deadwood) and plant Scots pine through the Plant menu.
  await click(cdp, '#add-reference');
  await waitFor(cdp, `document.getElementById('side').textContent.includes(${JSON.stringify(pineId)}) && !document.documentElement.classList.contains('busy')`, 'pine Fell queued');
  await pressKey(cdp, 'W');
  await waitFor(cdp, `document.querySelector('[data-disposition-tree="${pineId}"][data-disposition-value="LeaveAsDeadwood"]') !== null &&
    document.querySelector('[data-disposition-tree="${pineId}"][data-disposition-value="LeaveAsDeadwood"]').getBoundingClientRect().width > 0`, 'pine Fell disposition controls');
  await click(cdp, `[data-disposition-tree="${pineId}"][data-disposition-value="LeaveAsDeadwood"]`);
  await waitFor(cdp, `document.querySelector('[data-disposition-tree="${pineId}"][data-disposition-value="LeaveAsDeadwood"]').getAttribute('aria-pressed') === 'true' &&
    !document.documentElement.classList.contains('busy')`, 'Leave as Deadwood chosen');
  await setLens(cdp, 'regeneration');
  await click(cdp, '#mode-plant');
  await waitFor(cdp, "!document.getElementById('plant-panel').hidden && !document.getElementById('plant-species-pine').hidden", 'Plant menu offers Scots pine');
  await setValue(cdp, '#plant-species', 'scots-pine');
  const plantNote = await evaluate(cdp, "document.getElementById('plant-species-note').textContent");
  check(plantNote.includes('EUR 0.45') && plantNote.includes('8-year shelter') && plantNote.includes(TWO_ZONE),
    'SP2-UI-17 Plant menu: Scots pine with the Core EUR 0.45 sapling, 8-year shelter and site note');
  const point = await brightPlantingPoint(cdp, 'MA-A');
  if (!point) throw new Error('no free planting point in MA-A');
  log(`INFO ${vpLabel} planting in the brightest free interior cell ${point.cell} (Core light ${Number(point.light).toFixed(3)})`);
  await mouseClickAt(cdp, point.x, point.y);
  await waitFor(cdp, "document.getElementById('draft-plant-preview') !== null", 'draft planting point placed');
  await click(cdp, '#plant-opt-shelter');
  await click(cdp, '#plant-opt-spot');
  await click(cdp, '#plant-queue');
  await waitFor(cdp, "document.getElementById('plant-result').textContent.includes('Queued 3 pending orders')", 'Plant + Shelter + SpotClear queued');
  check(true, 'SP2-UI-18 Scots pine Plant + 8-year Shelter + ~1 m2 SpotClear queued through the Plant menu');
  await pressKey(cdp, 'Escape');
  await pressKey(cdp, 'W');
  await waitFor(cdp, "document.getElementById('side').textContent.includes('Scots pine sapling')", 'economy quote lists the Scots pine sapling', 20000);
  const quote = await evaluate(cdp, "document.getElementById('side').textContent");
  check(quote.includes('Scots pine sapling (€0.45)'), 'SP2-UI-19 Economy quote shows "Scots pine sapling (€0.45)" (Core EUR 0.45/tree)');

  // 6. Approve, Execute, Advance a year.
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden", 'plan approved', 30000);
  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open", 'execute dialog');
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "document.getElementById('approved-box').hidden && document.getElementById('receipt').textContent.length > 0", 'executed', 40000);
  const ledgerText = await evaluate(cdp, "document.getElementById('side').textContent");
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'advanced to Year 1', 60000);
  await sleep(500);
  const lane1 = await standLane(cdp, 'MA-A');
  const sp1 = lane1.ecology.scotsPine;
  const plantedPine = (lane1.ecology.planted || []).find((j) => j.speciesId === 'scots-pine');
  const dw = lane1.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === pineId);
  // The Core juvenile year is truthful: in light >= 0.20 a pine juvenile grows (shelter blocks browse); in deeper shade it dies.
  const assessed = Boolean(plantedPine) && plantedPine.lastAssessmentYear === 1;
  const truthful = assessed && (plantedPine.isActive ? plantedPine.heightM > 0.6 : ['light', 'vegetation', 'establishment'].includes(plantedPine.deathCause));
  const brightEnough = Number(point.light) >= 0.25;
  check(truthful && (!brightEnough || plantedPine.isActive) && Boolean(dw) && lane1.ecology.speciesByTreeId[pineId] === 'scots-pine' &&
    sp1.pineDeadwood >= 1 && sp1.accounts.length === 1 && ledgerText.length > 0,
    `SP2-UI-20 Year 1: planted pine ${plantedPine?.juvenileId} (cell light ${Number(point.light).toFixed(3)}) ${plantedPine?.isActive
      ? 'grew 0.60 → ' + plantedPine.heightM.toFixed(2) + ' m' : 'died: ' + plantedPine?.deathCause}; felled pine ${pineId} is Scots pine deadwood ${dw?.deadwoodId}; first pine account recorded`);
  if (plantedPine && plantedPine.isActive) {
    await waitFor(cdp, "document.querySelector('.planted-pj[data-species=\"scots-pine\"]') !== null", 'planted pine sapling drawn');
  }
  await setLens(cdp, 'habitat');
  const habitat1 = await slotText(cdp, 'sensitive-wet-ground');
  check(habitat1.text.includes('Scots pine deadwood logs: ' + sp1.pineDeadwood), `SP2-UI-21 Habitat lens shows ${sp1.pineDeadwood} Scots pine deadwood log(s)`);

  // 7. History: the Year 0 → 1 Scots pine card.
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'history index', 30000);
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.querySelector('[data-history-section=\"scots-pine-year\"]') !== null", 'Scots pine history card', 30000);
  const historyCard = await evaluate(cdp, "document.querySelector('[data-history-section=\"scots-pine-year\"]').textContent");
  check(historyCard.includes('Scots pine — Year 0 → 1') && historyCard.includes(TWO_ZONE) && historyCard.includes('seed year'),
    'SP2-UI-22 History drawer shows the recorded Scots pine Year 0 → 1 account and site');
  await shot(cdp, 'history-card');
  await click(cdp, '#history-open');
  await sleep(200);

  // 8. Save v3 through the UI.
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'saves dialog');
  await typeInto(cdp, '#save-slot', slotName);
  await click(cdp, '#save-do');
  await waitFor(cdp, `document.getElementById('saves-status').textContent.includes(${JSON.stringify('Saved "' + slotName + '"')})`, 'Save v3 committed', 30000);
  await click(cdp, '#saves-close');
  check(true, `SP2-UI-23 Save v3 slot ${slotName} committed through the UI`);

  writeFileSync(expectedPath, JSON.stringify({
    livingPineAdults: sp1.livingPineAdults, plantedPineId: plantedPine.juvenileId, plantedActive: plantedPine.isActive,
    plantedHeightM: plantedPine.heightM, plantedDeathCause: plantedPine.deathCause || null, felledPine: pineId, deadwoodId: dw.deadwoodId,
    pineDeadwood: sp1.pineDeadwood, recruits: sp1.recruits.length, accounts: sp1.accounts.length,
  }, null, 2));

  const end = await evaluate(cdp, `({ noHScroll: document.documentElement.scrollWidth <= window.innerWidth + 1,
    cols: getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length })`);
  audit.collect();
  check(end.noHScroll && end.cols === 2 && audit.consoleIssues.length === 0 && audit.badNetwork.length === 0,
    `SP2-UI-24 main phase: zero console errors and zero 404/5xx (console=${audit.consoleIssues.length}${audit.consoleIssues.length ? ' ' + audit.consoleIssues[0] : ''}, net=${audit.badNetwork.length}${audit.badNetwork.length ? ' ' + audit.badNetwork[0] : ''}), no horizontal scroll, ${end.cols} columns`);
}

async function runLoadPhase(cdp) {
  const expected = JSON.parse(readFileSync(expectedPath, 'utf8'));
  const audit = makeAuditor(cdp);
  audit.collect(true);
  await click(cdp, '#saves-open');
  await waitFor(cdp, `document.getElementById('saves-dialog').open && document.querySelector('[data-load="${slotName}"]') !== null`, 'fresh server lists the slot');
  await click(cdp, `[data-load="${slotName}"]`);
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'load confirmation');
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open && document.getElementById('year-value').textContent.trim() === '1'", 'Save v3 restored in a new process', 60000);
  await sleep(500);
  const lane = await standLane(cdp, 'MA-A');
  const sp = lane.ecology.scotsPine;
  const planted = (lane.ecology.planted || []).find((j) => j.juvenileId === expected.plantedPineId);
  const dw = lane.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === expected.felledPine);
  check(Boolean(sp) && sp.site.wholeStandLabel === TWO_ZONE && sp.livingPineAdults === expected.livingPineAdults,
    `SP2-UI-25 after restart: Two-Zone site and ${sp?.livingPineAdults} living Scots pine (expected ${expected.livingPineAdults})`);
  check(Boolean(planted) && planted.speciesId === 'scots-pine' && planted.isActive === expected.plantedActive &&
    planted.heightM === expected.plantedHeightM && (planted.deathCause || null) === expected.plantedDeathCause && Boolean(dw) && dw.deadwoodId === expected.deadwoodId &&
    lane.ecology.speciesByTreeId[expected.felledPine] === 'scots-pine' && sp.pineDeadwood === expected.pineDeadwood &&
    sp.recruits.length === expected.recruits && sp.accounts.length === expected.accounts,
    `SP2-UI-26 after restart: planted pine ${planted?.juvenileId}, pine deadwood ${dw?.deadwoodId} (${sp?.pineDeadwood}), ${sp?.recruits.length} recruits and ${sp?.accounts.length} account(s) persist`);
  await setLens(cdp, 'regeneration');
  if (expected.plantedActive) await waitFor(cdp, "document.querySelector('.planted-pj[data-species=\"scots-pine\"]') !== null", 'restored planted pine drawn');
  const regen = await slotText(cdp, 'soil-moisture-overlay');
  check(regen?.live === 'true' && regen.text.includes(TWO_ZONE), 'SP2-UI-27 restored Regeneration lens shows the live two-zone readout');
  audit.collect();
  check(audit.consoleIssues.length === 0 && audit.badNetwork.length === 0,
    `SP2-UI-28 load phase: zero console errors and zero 404/5xx (console=${audit.consoleIssues.length}, net=${audit.badNetwork.length})`);
}

async function main() {
  const { cdp, child, profile } = await launch();
  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await cdp.send('Log.enable');
    await cdp.send('Network.enable');
    await setViewport(cdp, width, height);
    await cdp.send('Page.navigate', { url });
    await waitFor(cdp, "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() !== '–'", 'the /multi page to render', 40000);
    await sleep(300);
    if (mode === 'load') await runLoadPhase(cdp);
    else await runMainPhase(cdp);
  } catch (err) {
    failures++;
    log(`FAIL ${vpLabel} ${tag}/${mode}: FATAL ${String(err && err.message || err).slice(0, 600)}`);
    try { await shot(cdp, 'fatal'); } catch { /* ignore */ }
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }
  log(`SUMMARY ${vpLabel} ${tag}/${mode}: ${passes} passed, ${failures} failed`);
  process.exit(failures === 0 ? 0 : 1);
}

main();
