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
 *   /fixtures/corrupt-imagery.png     200 image/png whose bytes are not a decodable PNG: the
 *                                     browser's own decoder rejects it (B31)
 *   /fixtures/held-imagery.png        the synthetic PNG, but the response is held until the driver
 *                                     calls releaseHeld() (deterministic "slow imagery" for B22)
 *
 * requestLog records, for every response actually written, the CSP and Referrer-Policy values
 * the server sent, so the driver can check headers for requests the browser never reports
 * (aborted, held or node-side requests) as well as the ones it does.
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
  // A real PNG signature followed by bytes that are not a valid chunk stream, so the MIME check passes
  // and the failure comes from the browser's own image decoder.
  const corrupt = Buffer.concat([imagery.subarray(0, 8), Buffer.from('this is not a PNG chunk stream '.repeat(8))]);
  const requestLog = [];
  const held = [];

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
      res.writeHead(status, { 'Content-Type': type });
      requestLog.push({
        method: req.method,
        pathname,
        url: req.url || '/', // exact request target (path + query), reconciled 1:1 with browser events in B24
        status,
        at: Date.now(),
        csp: res.getHeader('Content-Security-Policy') ?? null,
        referrer: res.getHeader('Referrer-Policy') ?? null,
        clientClosed: res.destroyed || req.destroyed,
      });
      res.end(body);
    };

    if (pathname === '/' || pathname === '/harness.html') return send(200, MIME['.html'], readFileSync(resolve(HERE, 'harness.html')));
    if (pathname === '/fixtures/synthetic-imagery.png') return send(200, MIME['.png'], imagery);
    if (pathname === '/fixtures/held-imagery.png') {
      held.push(() => send(200, MIME['.png'], imagery));
      return undefined;
    }
    if (pathname === '/fixtures/corrupt-imagery.png') return send(200, MIME['.png'], corrupt);
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
    heldCount: () => held.length,
    /** Answer every held imagery request now (sockets the browser already closed are written anyway). */
    releaseHeld: () => { const n = held.length; held.splice(0).forEach((reply) => reply()); return n; },
    close: () => new Promise((done) => { held.splice(0).forEach((reply) => reply()); server.closeAllConnections?.(); server.close(() => done()); }),
  };
}
