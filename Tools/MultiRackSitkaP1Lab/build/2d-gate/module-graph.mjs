#!/usr/bin/env node
// S0c static module-graph verifier (SPEC-multi-2d-s0-seam-extraction-V1 §7.5; Node built-ins only).
// Checks:
//   - LF-only, no BOM, no CR across all modules under src/Ui/multi/
//   - multi.js is deleted
//   - every import is static, single-line, top-of-file; no dynamic import( or re-exports
//   - every specifier starts with ./ or ../, ends with .js, resolves inside src/Ui/multi/, exists,
//     and has an explicit allow-listed route in MultiRoutes.cs and EmbeddedResource in SitkaP1.Ui.csproj
//   - layer order respected (imports point only downward or within L4)
//   - every module is reachable from main.js
//   - 3-colour DFS finds zero cycles and outputs exact topological order
//   - multi.html has exactly one <script> element (<script type="module" src="multi/main.js"></script>) and zero inline scripts
'use strict';
import { readdirSync, readFileSync, statSync, existsSync, writeFileSync } from 'node:fs';
import { dirname, join, relative, resolve, posix } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const p1Root = resolve(__dirname, '../..');

const args = process.argv.slice(2);
let modulesDir = join(p1Root, 'src/Ui/multi');
let routesCsPath = join(p1Root, 'src/Ui/MultiRoutes.cs');
let csprojPath = join(p1Root, 'src/Ui/SitkaP1.Ui.csproj');
let htmlPath = join(p1Root, 'src/Ui/multi/multi.html');
let outJson = null;

for (let i = 0; i < args.length; i++) {
  const a = args[i];
  if (a === '--modules') { modulesDir = resolve(args[++i]); }
  else if (a === '--routes') { routesCsPath = resolve(args[++i]); }
  else if (a === '--csproj') { csprojPath = resolve(args[++i]); }
  else if (a === '--html') { htmlPath = resolve(args[++i]); }
  else if (a === '--out') { outJson = resolve(args[++i]); }
  else if (!a.startsWith('-')) {
    if (a.endsWith('.json')) outJson = resolve(a);
    else modulesDir = resolve(a);
  } else {
    console.error(`USAGE ERROR: unknown option ${a}`);
    process.exit(64);
  }
}

const LAYER = {
  'store.js': 0,
  'dom.js': 1,
  'api.js': 2,
  'map/contract.js': 3,
  'map/minimap.js': 3,
  'map/renderer-svg.js': 3,
  'dialogs/execute-confirm.js': 4,
  'dialogs/saves.js': 4,
  'dialogs/woodland.js': 4,
  'modes/interactive.js': 4,
  'panels/fell-draft.js': 4,
  'panels/inspector.js': 4,
  'panels/time-history.js': 4,
  'panels/work-plan.js': 4,
  'commands.js': 4,
  'main.js': 5,
};

function fail(msg) {
  console.error('MODULE-GRAPH FAIL: ' + msg);
  process.exit(1);
}

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

function stripCommentsAndStrings(src) {
  let out = '';
  let i = 0;
  const n = src.length;
  let str = null;
  while (i < n) {
    const c = src[i];
    const nxt = i + 1 < n ? src[i + 1] : '';
    if (str) {
      if (c === '\\') { out += '  '; i += 2; }
      else if (c === str) { out += ' '; str = null; i += 1; }
      else { out += c === '\n' ? '\n' : ' '; i += 1; }
    } else if (c === '/' && nxt === '/') {
      while (i < n && src[i] !== '\n') { out += ' '; i += 1; }
    } else if (c === '/' && nxt === '*') {
      out += '  '; i += 2;
      while (i < n - 1 && !(src[i] === '*' && src[i + 1] === '/')) {
        out += src[i] === '\n' ? '\n' : ' ';
        i += 1;
      }
      out += '  '; i += 2;
    } else if (c === "'" || c === '"' || c === '`') {
      str = c; out += ' '; i += 1;
    } else {
      out += c; i += 1;
    }
  }
  return out;
}

if (existsSync(join(modulesDir, 'multi.js'))) {
  fail('legacy multi.js still exists under ' + modulesDir);
}

const modules = listJsFiles(modulesDir);
if (modules.length === 0) fail('no .js modules found under ' + modulesDir);
if (!modules.includes('main.js')) fail('entry module main.js missing');

// Parse MultiRoutes.cs explicit allow-listed module routes
const routesText = readFileSync(routesCsPath, 'utf8');
if (/routes\.Add\(\s*"multi\.js"/.test(routesText)) {
  fail('MultiRoutes.cs still registers legacy multi.js route');
}
const allowedRoutes = new Set();
for (const m of routesText.matchAll(/"(multi\/[a-zA-Z0-9_./-]+\.js)"/g)) {
  allowedRoutes.add(m[1]);
}

// Parse SitkaP1.Ui.csproj EmbeddedResources
const csprojText = readFileSync(csprojPath, 'utf8');
const embeddedIncludes = new Set();
for (const m of csprojText.matchAll(/<EmbeddedResource\s+Include="(multi\/[^"]+\.js)"\s+LogicalName="(SitkaP1\.Ui\.multi\.[^"]+\.js)"\s*\/>/g)) {
  const inc = m[1];
  const expectedLogical = 'SitkaP1.Ui.' + inc.replace(/\//g, '.');
  if (m[2] !== expectedLogical) {
    fail(`SitkaP1.Ui.csproj LogicalName mismatch for ${inc}: got ${m[2]}, expected ${expectedLogical}`);
  }
  embeddedIncludes.add(inc);
}

for (const mod of modules) {
  const routeKey = 'multi/' + mod;
  if (!allowedRoutes.has(routeKey)) fail(`module ${mod} missing allow-listed route ${routeKey} in MultiRoutes.cs`);
  if (!embeddedIncludes.has(routeKey)) fail(`module ${mod} missing EmbeddedResource ${routeKey} in SitkaP1.Ui.csproj`);
}
for (const r of allowedRoutes) {
  const mod = r.slice('multi/'.length);
  if (!modules.includes(mod)) fail(`MultiRoutes.cs registers route ${r} whose file does not exist`);
}

// Inspect every module's imports, syntax rules, and layer rules
const edges = new Map();
const importRe = /^import\s+\{[^}]+\}\s+from\s+'([^']+)';$/;
const anyImportRe = /^\s*import\b/;

for (const mod of modules) {
  const full = join(modulesDir, mod);
  const raw = readFileSync(full);
  if (raw.length >= 3 && raw[0] === 0xef && raw[1] === 0xbb && raw[2] === 0xbf) fail(`${mod}: UTF-8 BOM forbidden`);
  if (raw.includes(0x0d)) fail(`${mod}: CR byte forbidden`);
  const text = raw.toString('utf8');
  const lines = text.split('\n');
  const stripped = stripCommentsAndStrings(text);

  if (/\bimport\s*\(/.test(stripped)) fail(`${mod}: dynamic import() is forbidden`);
  if (/\bexport\s+(?:\*|\{[^}]*\})\s+from\b/.test(stripped)) fail(`${mod}: re-export is forbidden`);

  const deps = [];
  let inHeader = true;
  let inBlockComment = false;
  for (let idx = 0; idx < lines.length; idx++) {
    const ln = lines[idx];
    const trimmed = ln.trim();
    if (inBlockComment) {
      if (trimmed.includes('*/')) inBlockComment = false;
      continue;
    }
    if (trimmed === '' || trimmed.startsWith('//')) continue;
    if (trimmed.startsWith('/*')) {
      if (!trimmed.includes('*/')) inBlockComment = true;
      continue;
    }
    if (anyImportRe.test(ln)) {
      if (!inHeader) fail(`${mod}:${idx + 1}: import declaration must be at top of file`);
      const m = importRe.exec(ln);
      if (!m) fail(`${mod}:${idx + 1}: import must be a single-line static named import using single-quoted relative specifier: ${ln}`);
      const spec = m[1];
      if (!(spec.startsWith('./') || spec.startsWith('../')) || !spec.endsWith('.js')) {
        fail(`${mod}:${idx + 1}: forbidden specifier ${spec} (must start with ./ or ../ and end with .js)`);
      }
      const modDir = posix.dirname(mod);
      const resolved = posix.normalize(modDir === '.' ? spec : posix.join(modDir, spec));
      if (resolved.startsWith('..') || resolved.startsWith('/')) {
        fail(`${mod}:${idx + 1}: specifier ${spec} escapes src/Ui/multi/ (${resolved})`);
      }
      if (!existsSync(join(modulesDir, resolved))) {
        fail(`${mod}:${idx + 1}: imported target does not exist: ${resolved}`);
      }
      if (!allowedRoutes.has('multi/' + resolved)) {
        fail(`${mod}:${idx + 1}: imported target lacks allow-listed route: multi/${resolved}`);
      }
      if (mod in LAYER && resolved in LAYER && LAYER[resolved] > LAYER[mod]) {
        fail(`${mod}:${idx + 1}: upward layer import L${LAYER[mod]} -> L${LAYER[resolved]} (${resolved})`);
      }
      deps.push(resolved);
    } else {
      inHeader = false;
    }
  }
  edges.set(mod, deps);
}

// 3-colour DFS from main.js (0=white, 1=gray, 2=black)
const color = new Map();
const topoOrder = [];
function dfs(u, stack) {
  color.set(u, 1);
  for (const v of edges.get(u) || []) {
    const c = color.get(v) || 0;
    if (c === 1) {
      const cycleStart = stack.indexOf(v);
      const cycle = stack.slice(cycleStart).concat(v).join(' -> ');
      fail(`cycle detected in module graph: ${cycle}`);
    }
    if (c === 0) dfs(v, stack.concat(v));
  }
  color.set(u, 2);
  topoOrder.push(u);
}
dfs('main.js', ['main.js']);

const unreachable = modules.filter((m) => color.get(m) !== 2);
if (unreachable.length > 0) {
  fail(`module(s) not reachable from main.js: ${unreachable.join(', ')}`);
}

// Verify multi.html has exactly one <script> element (the module entry) and zero inline scripts
const htmlText = readFileSync(htmlPath, 'utf8');
const scriptTags = [...htmlText.matchAll(/<script\b[^>]*>([\s\S]*?)<\/script>/gi)];
if (scriptTags.length !== 1) {
  fail(`multi.html must contain exactly 1 <script> element, found ${scriptTags.length}`);
}
if (scriptTags[0][0] !== '<script type="module" src="multi/main.js"></script>' || scriptTags[0][1].trim() !== '') {
  fail(`multi.html <script> element mismatch: ${scriptTags[0][0]}`);
}

const report = {
  schemaVersion: 1,
  modulesDir: modulesDir,
  entry: 'main.js',
  moduleCount: modules.length,
  acyclic: true,
  allReachableFromEntry: true,
  topologicalOrder: topoOrder,
  edges: Object.fromEntries([...edges.entries()].sort((a, b) => a[0].localeCompare(b[0]))),
  htmlScriptTag: scriptTags[0][0],
};

if (outJson) {
  writeFileSync(outJson, JSON.stringify(report, null, 2) + '\n', 'utf8');
}
console.log(`MODULE-GRAPH PASS: ${modules.length} modules, acyclic, all reachable from main.js, 1 module script tag; topo=[${topoOrder.join(', ')}]`);
