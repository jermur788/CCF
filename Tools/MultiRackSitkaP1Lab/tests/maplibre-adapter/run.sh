#!/usr/bin/env bash
# MapLibre Display Adapter + Geo-Anchor Bridge, Part 1: the single verification entry point.
#   1) vendor integrity (SHA-256 + byte sizes against provenance.json)
#   2) geo-anchor round trip + independent Vincenty oracle + rejection cases
#   3) strict-CSP offline headless Chromium matrix B01-B31 (80 x 80 m, 5-lane fixture)
# Usage: tests/maplibre-adapter/run.sh [evidence-root]
# Evidence goes to <evidence-root>/run-<UTC stamp>/ (default root on the Files drive, outside the repo);
# a short summary is written to Docs/Handoffs/.../maplibre-adapter/evidence/summary.json.
set -uo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd -P)"
REPO="$(git -C "$HERE" rev-parse --show-toplevel)"
ROOT="${1:-/media/jer/Files/CCF/Verification/Multi2DMapLibreAdapterPart1-20261128}"
RUN_DIR="$ROOT/run-$(date -u +%Y%m%dT%H%M%SZ)"
SUMMARY="$REPO/Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/evidence/summary.json"

export PATH="$HOME/.local/share/vite-plus/bin:$PATH"
NODE="$(command -v node || true)"
CHROMIUM_BIN="${CHROMIUM:-/usr/bin/chromium}"
[[ -n "$NODE" && "$NODE" != *.exe ]] || { echo "STOP: Linux node not found on PATH" >&2; exit 2; }
[[ -x "$CHROMIUM_BIN" ]] || { echo "STOP: Chromium not found at $CHROMIUM_BIN" >&2; exit 2; }

mkdir -p "$RUN_DIR" "$(dirname "$SUMMARY")"
PIDFILE="$(mktemp -t ccf-maplibre-chromium.XXXXXX)"
cleanup() {
  # The driver stops Chromium itself; this is the safety net if the driver was killed.
  if [[ -s "$PIDFILE" ]]; then
    local pid; pid="$(cat "$PIDFILE")"
    kill -TERM -- "-$pid" 2>/dev/null || true
    sleep 1
    kill -KILL -- "-$pid" 2>/dev/null || true
  fi
  rm -f "$PIDFILE"
}
trap cleanup EXIT

{
  echo "node: $("$NODE" --version) ($NODE)"
  echo "chromium: $("$CHROMIUM_BIN" --version 2>/dev/null)"
  echo "commit: $(git -C "$REPO" rev-parse HEAD)"
  echo "started: $(date -u +%Y-%m-%dT%H:%M:%SZ)"
} | tee "$RUN_DIR/00-environment.log"

echo; echo "=== [1/3] Vendor integrity (MapLibre GL JS 6.20.0) ==="
"$NODE" "$HERE/verify-vendor.mjs" 2>&1 | tee "$RUN_DIR/01-vendor-integrity.log"
vendor_rc=${PIPESTATUS[0]}

echo; echo "=== [2/3] Geo-anchor bridge + independent Vincenty oracle ==="
"$NODE" "$HERE/test-geo-anchor.mjs" 2>&1 | tee "$RUN_DIR/02-geo-oracle.log"
geo_rc=${PIPESTATUS[0]}

echo; echo "=== [3/3] Strict-CSP offline headless Chromium matrix B01-B31 ==="
CHROMIUM="$CHROMIUM_BIN" CCF_CHROMIUM_PIDFILE="$PIDFILE" "$NODE" "$HERE/drive-maplibre-cdp.mjs" --out "$RUN_DIR" 2>&1 | tee "$RUN_DIR/03-cdp-b01-b31.log"
cdp_rc=${PIPESTATUS[0]}

(cd "$REPO" && "$NODE" "$HERE/write-summary.mjs" "$RUN_DIR" "$SUMMARY" "$vendor_rc" "$geo_rc" "$cdp_rc")

echo
echo "=== OVERALL ==="
echo "vendor integrity exit: $vendor_rc"
echo "geo oracle exit:       $geo_rc"
echo "B01-B31 exit:          $cdp_rc"
echo "evidence:              $RUN_DIR"
echo "summary:               $SUMMARY"
if [[ $vendor_rc -eq 0 && $geo_rc -eq 0 && $cdp_rc -eq 0 ]]; then echo "OVERALL: PASS"; exit 0; fi
echo "OVERALL: FAIL"; exit 1
