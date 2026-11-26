#!/usr/bin/env node
// S0c deterministic byte-for-byte JS reconstruction proof (SPEC-multi-2d-s0-seam-extraction-V1 §7.5; Node built-ins only).
// Reverses ONLY permitted transformations P2-P5 using committed s0c-hook-inventory.json and s0c-renderer-callsite-map.json,
// reassembles sections in original order (0..17b), re-wraps the IIFE, and asserts byte-for-byte equality with S0b-tip multi.js.
'use strict';
import { execFileSync } from 'node:child_process';
import { createHash } from 'node:crypto';
import { readFileSync, writeFileSync, readdirSync, statSync } from 'node:fs';
import { dirname, join, relative, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const p1Root = resolve(__dirname, '../..');
const repoRoot = execFileSync('git', ['-C', p1Root, 'rev-parse', '--show-toplevel'], { encoding: 'utf8' }).trim();

const args = process.argv.slice(2);
let modulesDir = join(p1Root, 'src/Ui/multi');
let hooksPath = join(__dirname, 's0c-hook-inventory.json');
let callsitesPath = join(__dirname, 's0c-renderer-callsite-map.json');
let expectSpec = null;
let outJson = null;

for (let i = 0; i < args.length; i++) {
  const a = args[i];
  if (a === '--modules') { modulesDir = resolve(args[++i]); }
  else if (a === '--hooks') { hooksPath = resolve(args[++i]); }
  else if (a === '--callsites') { callsitesPath = resolve(args[++i]); }
  else if (a === '--expect') { expectSpec = args[++i]; }
  else if (a === '--out') { outJson = resolve(args[++i]); }
  else if (!a.startsWith('-')) {
    if (a.endsWith('.json')) outJson = resolve(a);
    else modulesDir = resolve(a);
  } else {
    console.error(`USAGE ERROR: unknown option ${a}`);
    process.exit(64);
  }
}

function fail(msg) {
  console.error('JS-RECONSTRUCT FAIL: ' + msg);
  process.exit(1);
}

const hooksDoc = JSON.parse(readFileSync(hooksPath, 'utf8'));
const callsitesDoc = JSON.parse(readFileSync(callsitesPath, 'utf8'));
if (!expectSpec) {
  expectSpec = `${hooksDoc.sourceBaseCommit}:${hooksDoc.sourcePath}`;
}

const expectedBuf = execFileSync('git', ['-C', repoRoot, 'show', expectSpec]);
const expectedSha = createHash('sha256').update(expectedBuf).digest('hex');

function listJsFiles(dir, baseDir = dir) {
  const results = [];
  for (const name of readdirSync(dir).sort()) {
    const full = join(dir, name);
    const st = statSync(full);
    if (st.isDirectory()) results.push(...listJsFiles(full, baseDir));
    else if (name.endsWith('.js')) results.push(relative(baseDir, full).replace(/\\/g, '/'));
  }
  return results;
}

const EXPECTED_SPLIT_MODULES = new Set([
  'store.js', 'dom.js', 'api.js', 'map/renderer-svg.js',
  'dialogs/saves.js', 'dialogs/execute-confirm.js', 'modes/interactive.js',
  'panels/inspector.js', 'panels/fell-draft.js', 'panels/work-plan.js',
  'panels/time-history.js', 'commands.js', 'main.js',
]);
const actualJs = listJsFiles(modulesDir);
for (const m of actualJs) {
  if (m !== 'map/contract.js' && !EXPECTED_SPLIT_MODULES.has(m)) {
    fail(`unexpected JS module not covered by S0c mapping: ${m}`);
  }
}
for (const m of EXPECTED_SPLIT_MODULES) {
  if (!actualJs.includes(m)) fail(`missing mapped module: ${m}`);
}

function readModuleLines(mod) {
  const raw = readFileSync(join(modulesDir, mod));
  if (raw.includes(0x0d)) fail(`${mod}: contains CR byte`);
  const text = raw.toString('utf8');
  const lines = text.split('\n');
  if (lines[lines.length - 1] === '') lines.pop();
  return lines;
}

// Strip P3 provenance comment, static import lines, and P4/P5 adapter blocks; reverse P3 `  export `.
function extractModuleBody(mod) {
  const lines = readModuleLines(mod);
  let headerLines = [];
  let idx = 0;

  if (mod === 'main.js') {
    headerLines = lines.slice(0, 3);
    idx = 3;
  }
  if (!lines[idx] || !lines[idx].startsWith('// S0c provenance: ')) {
    fail(`${mod}: missing one-line S0c provenance comment at line ${idx + 1}`);
  }
  idx++;
  while (idx < lines.length && /^import\s+\{[^}]+\}\s+from\s+'[^']+';$/.test(lines[idx])) {
    idx++;
  }

  let body = lines.slice(idx);

  // Strip store.js P4 hooks registry block at end
  if (mod === 'store.js') {
    const marker = body.findIndex((ln) => ln === '  // S0c P4 hooks registry');
    if (marker < 1 || body[marker - 1] !== '') fail('store.js: missing P4 hooks registry block');
    body = body.slice(0, marker - 1);
  }

  // Strip map/renderer-svg.js P5 svgRenderer adapter block at end
  if (mod === 'map/renderer-svg.js') {
    const marker = body.findIndex((ln) => ln.startsWith('  // S0c P5 svgRenderer adapter'));
    if (marker < 1 || body[marker - 1] !== '') fail('map/renderer-svg.js: missing P5 svgRenderer adapter block');
    body = body.slice(0, marker - 1);
  }

  // Strip main.js P4 hook assignments immediately before bootstrap
  if (mod === 'main.js') {
    const marker = body.findIndex((ln) => ln === '  // S0c P4 hook assignments');
    if (marker === -1) fail('main.js: missing P4 hook assignments block');
    let end = marker + 1;
    while (end < body.length && /^  hooks\.[A-Za-z_$][A-Za-z0-9_$]* = [A-Za-z_$][A-Za-z0-9_$]*;$/.test(body[end])) {
      end++;
    }
    const assignedCount = end - (marker + 1);
    if (assignedCount !== hooksDoc.hookCount) {
      fail(`main.js: expected ${hooksDoc.hookCount} hook assignments, found ${assignedCount}`);
    }
    body = body.slice(0, marker).concat(body.slice(end));
  }

  // Reverse P3 `  export ` prefix on top-level declarations
  body = body.map((ln) => ln.replace(/^  export ((?:async )?function |const |let |var )/, '  $1'));

  return { headerLines, body };
}

function splitByBanner(lines, bannerPrefix, mod) {
  const idx = lines.findIndex((ln) => ln.startsWith(bannerPrefix));
  if (idx <= 0) fail(`${mod}: expected banner ${bannerPrefix}`);
  return [lines.slice(0, idx), lines.slice(idx)];
}

const store = extractModuleBody('store.js').body;
const [sec0, sec2] = splitByBanner(store, '  // ---- lookups in the latest atomic composite payload', 'store.js');

const sec1 = extractModuleBody('dom.js').body;

const api = extractModuleBody('api.js').body;
const [sec3, sec5] = splitByBanner(api, '  // ---- alerts, announcements and the re-read lock', 'api.js');

const sec4 = extractModuleBody('dialogs/saves.js').body;
const sec6 = extractModuleBody('dialogs/execute-confirm.js').body;

const mainExtracted = extractModuleBody('main.js');
const [sec7, mainRest] = splitByBanner(mainExtracted.body, '  // ---- selection & Escape precedence', 'main.js');
const wireIdx = mainRest.findIndex((ln) => /^  function wire\(/.test(ln));
if (wireIdx <= 0) fail('main.js: missing function wire()');
const sec15 = mainRest.slice(0, wireIdx);
const sec17b = mainRest.slice(wireIdx);

const mapMod = extractModuleBody('map/renderer-svg.js').body;
const [sec8, mapRest] = splitByBanner(mapMod, '  // ---- status marks', 'map/renderer-svg.js');
const [sec9, sec11] = splitByBanner(mapRest, '  // ---- grid and legend', 'map/renderer-svg.js');

const sec10 = extractModuleBody('modes/interactive.js').body;
const sec12 = extractModuleBody('panels/inspector.js').body;
const sec13 = extractModuleBody('panels/fell-draft.js').body;
const sec14 = extractModuleBody('panels/work-plan.js').body;
const sec16 = extractModuleBody('commands.js').body;
const sec17a = extractModuleBody('panels/time-history.js').body;

// Reassemble full multi.js lines with P2 wrapper restored
const reassembled = [
  ...mainExtracted.headerLines,
  "'use strict';",
  '',
  '(function () {',
  ...sec0,
  ...sec1,
  ...sec2,
  ...sec3,
  ...sec4,
  ...sec5,
  ...sec6,
  ...sec7,
  ...sec8,
  ...sec9,
  ...sec10,
  ...sec11,
  ...sec12,
  ...sec13,
  ...sec14,
  ...sec15,
  ...sec16,
  ...sec17a,
  ...sec17b,
  '})();',
];

// Reverse P4 (hooks.<fn> -> <fn>) and P5 (svgRenderer.{mount,render}() -> initFloorDefs()/renderMap())
// strictly at the recorded callsites in s0c-hook-inventory.json and s0c-renderer-callsite-map.json!
const rewritesByLine = new Map();
function addRewrite(lno, fromStr, toStr, kind) {
  if (!rewritesByLine.has(lno)) rewritesByLine.set(lno, []);
  rewritesByLine.get(lno).push({ fromStr, toStr, kind });
}

let p4Sites = 0;
for (const h of hooksDoc.hooks) {
  for (const cs of h.callSites) {
    addRewrite(cs.originalLine, `hooks.${h.function}`, h.function, 'P4');
    p4Sites++;
  }
}

let p5Sites = 0;
for (const cs of callsitesDoc.callSites) {
  if (cs.rewritten) {
    addRewrite(cs.originalLine, cs.after, cs.before, 'P5');
    p5Sites++;
  }
}

for (const [lno, items] of rewritesByLine.entries()) {
  if (lno < 1 || lno > reassembled.length) fail(`rewrite references out-of-range line ${lno}`);
  let line = reassembled[lno - 1];
  for (const { fromStr, toStr, kind } of items) {
    const pos = line.indexOf(fromStr);
    if (pos === -1) {
      fail(`${kind} reverse mismatch at line ${lno}: expected ${JSON.stringify(fromStr)} in ${JSON.stringify(line)}`);
    }
    line = line.slice(0, pos) + toStr + line.slice(pos + fromStr.length);
  }
  reassembled[lno - 1] = line;
}

// Verify zero leftover hooks. or svgRenderer. references remain after reversing recorded sites
for (let i = 0; i < reassembled.length; i++) {
  if (/(?<![.\w$])hooks\./.test(reassembled[i])) {
    fail(`unrecorded hooks. reference left at line ${i + 1}: ${reassembled[i]}`);
  }
  if (/(?<![.\w$])svgRenderer\./.test(reassembled[i])) {
    fail(`unrecorded svgRenderer. reference left at line ${i + 1}: ${reassembled[i]}`);
  }
}

// Join with LF and NO trailing newline (matching S0a raw-literal extraction)
const reconstructedBuf = Buffer.from(reassembled.join('\n'), 'utf8');
const reconstructedSha = createHash('sha256').update(reconstructedBuf).digest('hex');

if (Buffer.compare(reconstructedBuf, expectedBuf) !== 0) {
  const expLines = expectedBuf.toString('utf8').split('\n');
  const maxL = Math.max(expLines.length, reassembled.length);
  for (let i = 0; i < maxL; i++) {
    if (expLines[i] !== reassembled[i]) {
      fail(
        `byte mismatch at line ${i + 1} (reconstructed ${reconstructedBuf.length} B vs expected ${expectedBuf.length} B):\n` +
        `  expected:      ${JSON.stringify(expLines[i])}\n` +
        `  reconstructed: ${JSON.stringify(reassembled[i])}`
      );
    }
  }
  fail(`byte mismatch (reconstructed ${reconstructedBuf.length} B sha=${reconstructedSha} vs expected ${expectedBuf.length} B sha=${expectedSha})`);
}

const report = {
  schemaVersion: 1,
  expectSpec,
  byteIdentical: true,
  bytes: reconstructedBuf.length,
  lineCount: reassembled.length,
  sha256: reconstructedSha,
  expectedSha256: expectedSha,
  hooksReversed: hooksDoc.hookCount,
  p4CallSitesReversed: p4Sites,
  p5CallSitesReversed: p5Sites,
};

if (outJson) {
  writeFileSync(outJson, JSON.stringify(report, null, 2) + '\n', 'utf8');
}
console.log(`JS-RECONSTRUCT PASS: byte-identical to ${expectSpec} (${reconstructedBuf.length} B, ${reassembled.length} lines, sha256=${reconstructedSha}, P4=${p4Sites} sites across ${hooksDoc.hookCount} hooks, P5=${p5Sites} sites)`);
