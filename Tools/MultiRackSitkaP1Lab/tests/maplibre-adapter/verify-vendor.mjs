#!/usr/bin/env node
/**
 * Vendor integrity verification for MapLibre GL JS v6.20.0.
 * Re-hashes every committed file against provenance.json (SHA-256 + byte length)
 * and verifies no unexpected files (source maps, dev bundles, package.json) exist.
 */

import { createHash } from 'node:crypto';
import { readdirSync, readFileSync, statSync } from 'node:fs';
import { join, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const vendorDir = join(__dirname, '../../src/Ui/multi/map/maplibre/vendor/maplibre-gl-6.20.0');
const provenancePath = join(vendorDir, 'provenance.json');

let passed = 0;
let failed = 0;

function check(ok, message) {
  if (ok) {
    passed++;
    console.log(`PASS [vendor] ${message}`);
  } else {
    failed++;
    console.error(`FAIL [vendor] ${message}`);
  }
}

const provenance = JSON.parse(readFileSync(provenancePath, 'utf8'));
check(provenance.package === 'maplibre-gl' && provenance.version === '6.20.0' && provenance.license === 'BSD-3-Clause',
  `provenance metadata matches maplibre-gl@6.20.0 BSD-3-Clause`);
check(typeof provenance.tarballSha256 === 'string' && /^[0-9a-f]{64}$/.test(provenance.tarballSha256),
  `provenance records valid 64-hex tarball SHA-256 (${provenance.tarballSha256})`);

const expectedFiles = new Set(['provenance.json']);
for (const entry of provenance.files) {
  expectedFiles.add(entry.path);
  const fullPath = join(vendorDir, entry.path);
  const buf = readFileSync(fullPath);
  const actualBytes = statSync(fullPath).size;
  const actualSha = createHash('sha256').update(buf).digest('hex');
  check(actualBytes === entry.bytes, `${entry.path} byte size exact (${actualBytes} === ${entry.bytes})`);
  check(actualSha === entry.sha256, `${entry.path} SHA-256 exact (${actualSha})`);
}

const dirEntries = readdirSync(vendorDir).sort();
const unexpected = dirEntries.filter((name) => !expectedFiles.has(name));
check(unexpected.length === 0, `no unexpected files in vendor dir (${dirEntries.join(', ')})`);

// Verify vendored JS bundles contain no external static ES imports and CSS contains no non-data url().
const mainJs = readFileSync(join(vendorDir, 'maplibre-gl.mjs'), 'utf8');
const workerJs = readFileSync(join(vendorDir, 'maplibre-gl-worker.mjs'), 'utf8');
const css = readFileSync(join(vendorDir, 'maplibre-gl.css'), 'utf8');

// A static import is `import ... from "spec"` or `import "spec"` at statement level.
const STATIC_IMPORT_RE = /(?:^|[;}\n])\s*import\s*(?:[\w$*{},\s]+?\s*from\s*)?["'][^"']+["']/;
check(!STATIC_IMPORT_RE.test(mainJs), 'maplibre-gl.mjs has zero static ES imports (self-contained bundle)');
check(!STATIC_IMPORT_RE.test(workerJs), 'maplibre-gl-worker.mjs has zero static ES imports (self-contained bundle)');
check(!mainJs.includes('maplibre-gl-shared') && !workerJs.includes('maplibre-gl-shared'),
  'omitted maplibre-gl-shared.mjs (0 bytes upstream) is not referenced by either bundle');
check(provenance.files.length === 4 && Array.isArray(provenance.extraction?.copiedPaths) && provenance.extraction.copiedPaths.length === 4,
  'provenance lists exactly 4 vendored files with extraction paths');
const cssUrls = [...css.matchAll(/url\(([^)]+)\)/g)].map((m) => m[1].trim());
const nonDataCssUrls = cssUrls.filter((u) => !/^['"]?data:/.test(u));
check(nonDataCssUrls.length === 0, `maplibre-gl.css uses data: URI images only (${cssUrls.length} data URIs, 0 external)`);

console.log(`VENDOR_INTEGRITY_SUMMARY: ${passed} passed, ${failed} failed, files_checked=${provenance.files.length}`);
process.exit(failed === 0 ? 0 : 1);
