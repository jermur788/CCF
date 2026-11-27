#!/usr/bin/env bash
# CCF Multi 2D — Phase 2B-2b Dispositions UI + Save v3 Economy real-browser acceptance suite (Microsoft Edge over CDP).
# Verifies at BOTH 1920x1080 and 1280x720 against fresh WSL2 sitkap1-ui processes (with real process restart for Save v3):
#   - Money Off default unchanged (History drawer shows FinancialLedger: NOT RECORDED, zero €, 1 History button)
#   - Forest Maker woodland started with Money On -> Top strip Cash, Landowner (40 / 40 hrs), Retained timber (0.000 m³)
#   - 3-way felling dispositions (Sell & Extract / Keep for Own Use / Leave as Deadwood) on pending Fell orders
#   - Zero editable volume input under Keep for Own Use (exact engine-graded m³ from server)
#   - Disposition change bumps planStateRevision + forces fresh server quote; Approve locks dispositions; Cancel + re-quote works
#   - €2,500 floor highlight, contractor-only Fell lock + 409 refusal, calibrated-estimate disclaimer intact
#   - Execute updates Retained timber strip, Cash, Landowner hours, Stage 4 fallen deadwood count (DW####, cause=fell), receipt
#   - Existing single History drawer shows Financial Ledger + Felling dispositions + retained m³
#   - Save v3 slot -> STOP server #1 -> START fresh server #2 -> Load v3 slot -> all economy + retained m³ + deadwood + history intact
#   - Zero console errors, zero CSP violations, zero 404/5xx, zero horizontal scroll, <= 3 columns at both viewports
#
# Usage: tests/phase2b2b-ui/run-phase2b2b-edge.sh [<evidence-dir>]
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/mnt/c/Users/hp/AppData/Local/Temp/ccf-phase2b2b-edge-$stamp}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac
evidence_win="$(wslpath -w "$evidence")"
driver_win="$(wslpath -w "$p1_root/tests/phase2b2b-ui/drive-phase2b2b.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-p2b2b-edge.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
mkdir -p "$scratch/fixture"
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
  local run="$1" xdg_dir="$2" attempt
  mkdir -p "$xdg_dir"
  set -m
  ( cd "$p1_root" && exec env XDG_DATA_HOME="$xdg_dir" "$dotnet_bin" "$ui_dll" "$scratch/fixture" \
      --multi-experiment=ma-1-mixed ) > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
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
  local run="$1" rc=0
  kill -INT "$server_pid" 2> /dev/null || true
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2> /dev/null; then say "FAIL Z01-$run did not stop on SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fail=1; fi
  wait "$server_pid" 2> /dev/null || rc=$?
  server_pid=""
  if [[ "$rc" == 0 ]]; then say "PASS Z01-$run stopped cleanly on SIGINT"; else say "FAIL Z01-$run exit $rc"; fail=1; fi
  if [[ -s "$scratch/server-$run.err" ]]; then
    say "FAIL Z02-$run the server wrote to stderr"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"; fail=1
  else
    say "PASS Z02-$run the server wrote nothing to stderr"
  fi
  sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' "$scratch/server-$run.out" > "$evidence/server-$run.out" 2> /dev/null || true
}

drive() {
  local tag="$1" mode="$2" width="$3" height="$4" slot="$5" url="$6"
  say "DRIVE $tag ($mode, ${width}x${height}, slot=${slot})"
  "$node_win" "$driver_win" --tag "$tag" --mode "$mode" --width "$width" --height "$height" --slot "$slot" \
    --url "$url" --out "$evidence_win" 2>&1 \
    | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | tee -a "$log"
  return "${PIPESTATUS[0]}"
}

fail=0
run() { if "$@"; then :; else fail=1; fi; }

# 1. Full 1920x1080 flow: dispositions + execute + history + Save v3 -> STOP server -> START fresh server -> Load v3
start_server p2b2b-1920-save "$scratch/xdg-1920"
run drive vp1920 save 1920 1080 p2b2b-v3-1920 "${server_url%/}/multi"
stop_server p2b2b-1920-save

start_server p2b2b-1920-load "$scratch/xdg-1920"
run drive vp1920 load 1920 1080 p2b2b-v3-1920 "${server_url%/}/multi"
stop_server p2b2b-1920-load

# 2. Full 1280x720 flow: dispositions + execute + history + Save v3 -> STOP server -> START fresh server -> Load v3
start_server p2b2b-1280-save "$scratch/xdg-1280"
run drive vp1280 save 1280 720 p2b2b-v3-1280 "${server_url%/}/multi"
stop_server p2b2b-1280-save

start_server p2b2b-1280-load "$scratch/xdg-1280"
run drive vp1280 load 1280 720 p2b2b-v3-1280 "${server_url%/}/multi"
stop_server p2b2b-1280-load

vp1920_pass="$(grep -c '^PASS \[1920x1080\]' "$log" || true)"
vp1920_fail="$(grep -c '^FAIL \[1920x1080\]' "$log" || true)"
vp1280_pass="$(grep -c '^PASS \[1280x720\]' "$log" || true)"
vp1280_fail="$(grep -c '^FAIL \[1280x720\]' "$log" || true)"
say "VIEWPORT 1920x1080: ${vp1920_pass} passed, ${vp1920_fail} failed"
say "VIEWPORT 1280x720: ${vp1280_pass} passed, ${vp1280_fail} failed"
say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
