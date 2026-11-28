#!/usr/bin/env bash
# CCF Multi 2D — Map-First Menu & Lenses Upgrade real-browser acceptance suite (Microsoft Edge over CDP).
# Covers MLUI-01..12 at BOTH 1920x1080 and 1280x720 against fresh WSL2 sitkap1-ui processes (including cross-process Save v3 restart).
# Usage: tests/map-first-lenses-ui/run-map-first-lenses-edge.sh [<evidence-dir>]
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/mnt/c/Users/hp/AppData/Local/Temp/ccf-map-first-lenses-edge-$stamp}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac
evidence_win="$(wslpath -w "$evidence")"
driver_win="$(wslpath -w "$p1_root/tests/map-first-lenses-ui/drive-map-first-lenses.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-mlui-edge.XXXXXXXX")"
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

# 1. Full 1920x1080 qualification + Save v3 cross-process restart
start_server mlui-1920-main "$scratch/xdg-1920"
run drive vp1920 main 1920 1080 mlui-v3-1920 "${server_url%/}/multi"
stop_server mlui-1920-main

start_server mlui-1920-load "$scratch/xdg-1920"
run drive vp1920 load 1920 1080 mlui-v3-1920 "${server_url%/}/multi"
stop_server mlui-1920-load

# 2. Full 1280x720 qualification + Save v3 cross-process restart
start_server mlui-1280-main "$scratch/xdg-1280"
run drive vp1280 main 1280 720 mlui-v3-1280 "${server_url%/}/multi"
stop_server mlui-1280-main

start_server mlui-1280-load "$scratch/xdg-1280"
run drive vp1280 load 1280 720 mlui-v3-1280 "${server_url%/}/multi"
stop_server mlui-1280-load

vp1920_pass="$(grep -c '^PASS \[1920x1080\]' "$log" || true)"
vp1920_fail="$(grep -c '^FAIL \[1920x1080\]' "$log" || true)"
vp1280_pass="$(grep -c '^PASS \[1280x720\]' "$log" || true)"
vp1280_fail="$(grep -c '^FAIL \[1280x720\]' "$log" || true)"
say "VIEWPORT 1920x1080: ${vp1920_pass} passed, ${vp1920_fail} failed"
say "VIEWPORT 1280x720: ${vp1280_pass} passed, ${vp1280_fail} failed"
say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
