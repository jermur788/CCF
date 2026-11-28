#!/usr/bin/env node
/**
 * Builds the small committed summary.json from the three gate logs and the CDP report.
 * Usage: node write-summary.mjs <runDir> <summaryPath> <vendorRc> <geoRc> <cdpRc>
 */
import { execFileSync } from 'node:child_process';
import { existsSync, readFileSync, writeFileSync } from 'node:fs';
import { join } from 'node:path';

const [runDir, summaryPath, vendorRc, geoRc, cdpRc] = process.argv.slice(2);
const read = (name) => (existsSync(join(runDir, name)) ? readFileSync(join(runDir, name), 'utf8') : '');
const git = (...args) => { try { return execFileSync('git', args, { encoding: 'utf8' }).trim(); } catch { return null; } };

const vendor = read('01-vendor-integrity.log').match(/VENDOR_INTEGRITY_SUMMARY: (\d+) passed, (\d+) failed, files_checked=(\d+)/);
const geo = read('02-geo-oracle.log').match(/GEO_ORACLE_SUMMARY: (\d+) passed, (\d+) failed, max_roundtrip_m=(\S+), oracle_30m=(\S+), oracle_80m=(\S+), oracle_120m=(\S+)/);
const cdpText = read('cdp-matrix-report.json');
const cdp = cdpText ? JSON.parse(cdpText) : null;
const status = (rc) => (Number(rc) === 0 ? 'PASS' : Number(rc) === 2 ? 'NOT TESTED' : 'FAIL');

const summary = {
  schemaVersion: 7, // v7 (repair cycle 6): counted self-test skips (selfTestsSkipped); v6 (repair cycle 5): per-attempt response accounting (browser request <-> its own server receipt); v5 B01-B31, exact-URL accounting; v4 Script-type worker startup evidence; v3 per-worker-target coverage
  generatedUtc: new Date().toISOString(),
  testedCommit: git('rev-parse', 'HEAD'),
  testedTreeDirty: (git('status', '--porcelain', '--', 'Tools/MultiRackSitkaP1Lab/src/Ui/multi/map', 'Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter') || '') !== '',
  durableEvidenceDir: runDir,
  overall: [vendorRc, geoRc, cdpRc].every((rc) => Number(rc) === 0) ? 'PASS' : 'FAIL',
  vendorIntegrity: vendor
    ? { status: status(vendorRc), checksPassed: +vendor[1], checksFailed: +vendor[2], filesHashed: +vendor[3] }
    : { status: 'FAIL', note: 'summary line missing' },
  geoOracle: geo
    ? { status: status(geoRc), checksPassed: +geo[1], checksFailed: +geo[2], thresholdM: 0.01, maxRoundTripErrorM: +geo[3],
      oracleMaxDistanceErrorM: { stand30m: +geo[4], stand80m: +geo[5], stand120m: +geo[6] } }
    : { status: 'FAIL', note: 'summary line missing' },
  browserMatrix: cdp && cdp.totals
    ? {
      status: status(cdpRc),
      passed: cdp.totals.passed,
      failed: cdp.totals.failed,
      total: cdp.totals.total,
      expected: cdp.totals.expected,
      browser: cdp.browser,
      webglRenderer: cdp.webgl && cdp.webgl.renderer,
      cspHeader: cdp.cspHeader,
      referrerPolicy: cdp.referrerPolicy,
      consoleErrors: cdp.audit.consoleErrors.length,
      cspViolations: cdp.audit.cspViolations.length,
      failedRequests: cdp.audit.failedRequests.length,
      nonLoopbackRequests: cdp.audit.nonLoopbackRequests.length,
      httpRequests: cdp.audit.httpRequests,
      browserResponsesHeaderChecked: cdp.audit.responsesChecked,
      serverResponsesHeaderChecked: cdp.audit.serverResponsesChecked,
      workerSessionResponses: cdp.audit.workerSessionResponses,
      workerTargets: Array.isArray(cdp.audit.workerTargets) ? cdp.audit.workerTargets.length : null,
      auditSetupFailures: Array.isArray(cdp.audit.setupFailures) ? cdp.audit.setupFailures.length : null,
      unansweredRequests: Array.isArray(cdp.audit.unansweredRequests) ? cdp.audit.unansweredRequests.length : null,
      serverReceipts: cdp.audit.serverReceipts ?? null,
      selfTestsSkipped: Array.isArray(cdp.audit.selfTestsSkipped) ? cdp.audit.selfTestsSkipped.length : null,
      selfTestSkipReasons: Array.isArray(cdp.audit.selfTestsSkipped) ? cdp.audit.selfTestsSkipped : null,
      attemptAccounting: cdp.audit.attemptAccounting
        ? {
          serverResponsesWithoutOwnBrowserRequest: cdp.audit.attemptAccounting.serverWithoutBrowser.length,
          browserResponsesWithoutOwnServerResponse: cdp.audit.attemptAccounting.browserWithoutServer.length,
          orphanBrowserResponses: cdp.audit.attemptAccounting.orphanResponses.length,
          abandonedReceipts: cdp.audit.attemptAccounting.abandonedReceipts.length,
          mixedDuplicate: cdp.audit.attemptAccounting.mixedDuplicate,
        }
        : null,
      headerMismatches: cdp.audit.headerMismatches ? cdp.audit.headerMismatches.browser.length + cdp.audit.headerMismatches.server.length + cdp.audit.headerMismatches.node.length : null,
      cases: cdp.cases.map((c) => ({ id: c.id, status: c.status, detail: c.detail })),
    }
    : { status: status(cdpRc), note: cdp && cdp.fatal ? 'harness fatal: ' + cdp.fatal.split('\n')[0] : 'no report' },
};
writeFileSync(summaryPath, JSON.stringify(summary, null, 2) + '\n');
console.log('summary written: ' + summaryPath);
