/**
 * Loopback-only static server for the MapLibre adapter harness.
 *
 * Every response (including 404s) carries exactly STRICT_CSP_HEADER and Referrer-Policy: no-referrer.
 * Routes:
 *   /harness.html                     the harness page
 *   /map/<path>                       files under src/Ui/multi/map/ (contract.js + maplibre/)
 *   /harness/<path>                   files in this test folder (harness.js, fixture, stub worker)
 *   /fixtures/synthetic-imagery.png   generated synthetic imagery
 *   /fixtures/not-an-image.png        200 text/plain: imagery that exists but is not an image
 */

import http from 'node:http';
import { existsSync, readFileSync, statSync } from 'node:fs';
import { dirname, extname, resolve, sep } from 'node:path';
import { fileURLToPath } from 'node:url';
import { generateSyntheticWoodlandPng } from './synthetic-png.mjs';

const HERE = dirname(fileURLToPath(import.meta.url));
const MAP_ROOT = resolve(HERE, '../../src/Ui/multi/map');

export const STRICT_CSP_HEADER =
  "default-src 'none'; script-src 'self'; style-src 'self'; connect-src 'self'; worker-src 'self'; img-src data: blob: 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'";
export const REFERRER_POLICY = 'no-referrer';

const MIME = Object.freeze({
  '.html': 'text/html; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8',
  '.mjs': 'text/javascript; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.png': 'image/png',
  '.txt': 'text/plain; charset=utf-8',
});

function fileUnder(root, relative) {
  const full = resolve(root, relative);
  if (!full.startsWith(root + sep)) return null;
  return existsSync(full) && statSync(full).isFile() ? full : null;
}

export async function startHarnessServer() {
  const imagery = generateSyntheticWoodlandPng();
  const requestLog = [];

  const server = http.createServer((req, res) => {
    res.setHeader('Content-Security-Policy', STRICT_CSP_HEADER);
    res.setHeader('Referrer-Policy', REFERRER_POLICY);
    res.setHeader('X-Content-Type-Options', 'nosniff');
    res.setHeader('Cache-Control', 'no-store');
    let pathname = '/';
    try {
      pathname = decodeURIComponent(new URL(req.url || '/', 'http://127.0.0.1').pathname);
    } catch {
      pathname = '<malformed>';
    }
    const send = (status, type, body) => {
      requestLog.push({ method: req.method, pathname, status, at: Date.now() });
      res.writeHead(status, { 'Content-Type': type });
      res.end(body);
    };

    if (pathname === '/' || pathname === '/harness.html') return send(200, MIME['.html'], readFileSync(resolve(HERE, 'harness.html')));
    if (pathname === '/fixtures/synthetic-imagery.png') return send(200, MIME['.png'], imagery);
    if (pathname === '/fixtures/not-an-image.png') return send(200, MIME['.txt'], 'imagery service unavailable');
    if (pathname === '/favicon.ico') return send(204, MIME['.txt'], '');
    const routes = [['/map/', MAP_ROOT], ['/harness/', HERE]];
    for (const [prefix, root] of routes) {
      if (!pathname.startsWith(prefix)) continue;
      const file = fileUnder(root, pathname.slice(prefix.length));
      if (!file) break;
      return send(200, MIME[extname(file)] || 'application/octet-stream', readFileSync(file));
    }
    return send(404, MIME['.txt'], 'Not found');
  });

  await new Promise((ok, fail) => {
    server.once('error', fail);
    server.listen(0, '127.0.0.1', ok);
  });
  const { port } = server.address();
  return {
    baseUrl: 'http://127.0.0.1:' + port,
    requestLog,
    close: () => new Promise((done) => { server.closeAllConnections?.(); server.close(() => done()); }),
  };
}
