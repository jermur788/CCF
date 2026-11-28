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
 *   /fixtures/abandoned-then-ok.txt   B24 mixed duplicate: the FIRST receipt of each exact request target
 *                                     is never answered (the client aborts it), every later one gets 200
 *
 * requestLog records, for every response actually written, the CSP and Referrer-Policy values
 * the server sent, so the driver can check headers for requests the browser never reports
 * (aborted, held or node-side requests) as well as the ones it does.
 * receiptLog records every request the server received, in arrival order, answered or not, with
 * its response status (null if none was ever written) and whether the client had closed the socket
 * first. B24 pairs each browser request with its own receipt (repair cycle 5).
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
  const receiptLog = [];
  const held = [];
  const abandonedOnce = new Set(); // exact targets of /fixtures/abandoned-then-ok.txt already received once

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
    // Every request the server receives gets a receipt (in arrival order), answered or not, so B24 can
    // tell a receipt the client abandoned before any response from a response that was written.
    const receipt = { seq: receiptLog.length + 1, method: req.method, url: req.url || '/', pathname, receivedAt: Date.now(),
      clientPort: req.socket.remotePort, responseStatus: null, responseToClosedSocket: null, clientClosedBeforeResponse: false, clientClosedAfterResponse: false };
    receiptLog.push(receipt);
    res.on('close', () => {
      if (receipt.responseStatus === null) receipt.clientClosedBeforeResponse = true;
      else if (!res.writableFinished) receipt.clientClosedAfterResponse = true;
    });
    const send = (status, type, body) => {
      res.writeHead(status, { 'Content-Type': type });
      receipt.responseStatus = status;
      requestLog.push({
        method: req.method,
        pathname,
        url: req.url || '/', // exact request target (path + query), reconciled with browser events in B24
        status,
        at: Date.now(),
        receiptSeq: receipt.seq,
        csp: res.getHeader('Content-Security-Policy') ?? null,
        referrer: res.getHeader('Referrer-Policy') ?? null,
        // true when the client had already closed the socket, so this response could not be delivered
        clientClosed: Boolean(res.destroyed || req.destroyed || receipt.clientClosedBeforeResponse),
      });
      receipt.responseToClosedSocket = requestLog[requestLog.length - 1].clientClosed;
      res.end(body);
    };

    if (pathname === '/' || pathname === '/harness.html') return send(200, MIME['.html'], readFileSync(resolve(HERE, 'harness.html')));
    if (pathname === '/fixtures/synthetic-imagery.png') return send(200, MIME['.png'], imagery);
    if (pathname === '/fixtures/held-imagery.png') {
      held.push(() => send(200, MIME['.png'], imagery));
      return undefined;
    }
    if (pathname === '/fixtures/abandoned-then-ok.txt') {
      if (!abandonedOnce.has(receipt.url)) { abandonedOnce.add(receipt.url); return undefined; } // never answered
      return send(200, MIME['.txt'], 'ok');
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
    receiptLog,
    heldCount: () => held.length,
    /** Answer every held imagery request now (sockets the browser already closed are written anyway). */
    releaseHeld: () => { const n = held.length; held.splice(0).forEach((reply) => reply()); return n; },
    close: () => new Promise((done) => { held.splice(0).forEach((reply) => reply()); server.closeAllConnections?.(); server.close(() => done()); }),
  };
}
