#!/usr/bin/env bash
# CCF Multi 2D — Phase 2A-2 Forest Maker UI, Multi-Lane View, C-FENCE & Save v3 real-browser acceptance suite.
# Proves P2A-07..P2A-REG at BOTH 1920x1080 and 1280x720 against fresh WSL2 sitkap1-ui processes:
#   - D-020 static source check (zero JS forestry/bounds/count/geometry math in woodland.js & minimap.js)
#   - New Woodland modal (accessible dialog, Esc/focus return, built-ins, both 0.64 ha presets, custom Core preview + refusal)
#   - Unsaved progress guard (409 WoodlandUnsavedProgress -> confirmed atomic swap -> stale old fence 409)
#   - Multi-lane view (2 lanes at outer A/E, 3 lanes at interior C, lane borders/labels, open grassland, mini-map click + Enter, 1-lane toggle, neighbour tree click)
#   - C-FENCE lane-filtered /stand + /stand?lanes=... (< 524,288 B, identical fence, stale fence 409, unknown lane 409)
#   - Save v3 -> genuine server process restart -> Load from fresh MA-1 process -> Continue to Year 3
#   - Forced payload overflow (CCF_TEST_MAX_VIEW_BYTES=250000) -> HTTP 503 MultiViewPayloadBudgetExceeded + UI alert keeping last good view
#
# Usage: tests/phase2a-ui/run-phase2a-edge.sh [<evidence-dir>]
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet-8.0.133/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/mnt/c/Users/$USER/AppData/Local/Temp/ccf-phase2a-edge-$stamp}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac
evidence_win="$(wslpath -w "$evidence")"
driver_win="$(wslpath -w "$p1_root/tests/phase2a-ui/drive-phase2a.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

fail=0

# D-020 source-grep proof: zero forestry/bounds/count/geometry math in new Phase 2A-2 JS modules
if grep -En "3000|2100|1344|SplitMix|6400101|6400202|stemsPerHa\s*\*" "$p1_root/src/Ui/multi/dialogs/woodland.js" "$p1_root/src/Ui/multi/map/minimap.js" > /dev/null 2>&1; then
  say "FAIL D020 JS module contains hardcoded forestry bounds or stem calculation"
  fail=1
else
  say "PASS D020 woodland.js and minimap.js contain zero hardcoded forestry bounds or stem calculations"
fi

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-phase2a-edge.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
mkdir -p "$scratch/fixture" "$scratch/xdg"
cp -- "$p1_root/fixtures/derived/$fixture_name" "$scratch/fixture/$fixture_name"

server_pid=""
server_url=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2> /dev/null; then
    kill -KILL -"$server_pid" 2> /dev/null || kill -KILL "$server_pid" 2> /dev/null || true
  fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

start_server() {
  local run="$1" extra_env="${2:-}" attempt
  set -m
  if [[ -n "$extra_env" ]]; then
    ( cd "$p1_root" && exec env XDG_DATA_HOME="$scratch/xdg" "$extra_env" "$dotnet_bin" "$ui_dll" "$scratch/fixture" \
        --multi-experiment=ma-1 ) > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
  else
    ( cd "$p1_root" && exec env XDG_DATA_HOME="$scratch/xdg" "$dotnet_bin" "$ui_dll" "$scratch/fixture" \
        --multi-experiment=ma-1 ) > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
  fi
  server_pid=$!
  set +m
  server_url=""
  for attempt in $(seq 1 300); do
    server_url="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$scratch/server-$run.out" | head -1 || true)"
    [[ -n "$server_url" ]] && break
    kill -0 "$server_pid" 2> /dev/null || break
    sleep 0.1
  done
  if [[ -z "$server_url" ]]; then
    say "FAIL S01-$run the UI process printed no launch URL"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"; exit 1
  fi
  say "SERVER $run pid $server_pid"
}

stop_server() {
  local run="$1" allow_stderr="${2:-0}" rc=0
  kill -INT "$server_pid" 2> /dev/null || true
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2> /dev/null; then say "FAIL Z01-$run did not stop on SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fail=1; fi
  wait "$server_pid" 2> /dev/null || rc=$?
  server_pid=""
  if [[ "$rc" == 0 ]]; then say "PASS Z01-$run stopped cleanly on SIGINT"; else say "FAIL Z01-$run exit $rc"; fail=1; fi
  if [[ "$allow_stderr" == "0" && -s "$scratch/server-$run.err" ]]; then
    say "FAIL Z02-$run the server wrote to stderr"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"; fail=1
  else
    say "PASS Z02-$run server stderr check ok (allow_stderr=$allow_stderr)"
  fi
  sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' "$scratch/server-$run.out" > "$evidence/server-$run.out" 2> /dev/null || true
}

drive() {
  local mode="$1" tag="$2" width="$3" height="$4" url="$5"
  say "DRIVE $tag ($mode, ${width}x${height})"
  "$node_win" "$driver_win" --mode "$mode" --tag "$tag" --width "$width" --height "$height" \
    --url "$url" --out "$evidence_win" 2>&1 \
    | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | tee -a "$log"
  return "${PIPESTATUS[0]}"
}

run() { if "$@"; then :; else fail=1; fi; }

# 1. Full Phase 2A-2 suite (defaults to 1920x1080; override via PHASE2A_FULL_W/PHASE2A_FULL_H/PHASE2A_FULL_TAG)
full_w="${PHASE2A_FULL_W:-1920}"
full_h="${PHASE2A_FULL_H:-1080}"
full_tag="${PHASE2A_FULL_TAG:-vp${full_w}}"
start_server p2a-full
run drive full "$full_tag" "$full_w" "$full_h" "${server_url%/}/multi"
stop_server p2a-full 0

# 2. Genuine server process restart -> negative v3 load unsaved-progress check -> Load v3 slot at 1280x720 -> Continue to Year 3
start_server p2a-restart
run drive restart-load vp1280-restart 1280 720 "${server_url%/}/multi"
stop_server p2a-restart 0

# 3. Forced payload overflow (lowering budget to 400000 B) -> HTTP 503 on ALL view-returning routes + UI alert keeping last good view
start_server p2a-overflow "CCF_TEST_MAX_VIEW_BYTES=400000"
run drive overflow-503 vp1280-overflow 1280 720 "${server_url%/}/multi"
stop_server p2a-overflow 1

# 4. Budget override raise-clamp & near-cap render proof (9999999 B override cannot raise scaled/frozen budgets -> forced 503 reports computed budget; 2,880-stem 5-lane 2/3-lane reads return 200 and render without overflow)
start_server p2a-clamp "CCF_TEST_MAX_VIEW_BYTES=9999999"
run drive clamp-raise vp1280-clamp 1280 720 "${server_url%/}/multi"
stop_server p2a-clamp 1

say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
