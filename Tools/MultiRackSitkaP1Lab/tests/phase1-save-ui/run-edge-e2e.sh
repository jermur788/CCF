#!/usr/bin/env bash
# Phase 1 Save/Load real-browser acceptance: Save -> quit the server -> restart -> Load -> continue, at 1280x720 and
# 1920x1080, in the installed Microsoft Edge (Windows) against the real WSL2 UI process.
#
# It runs three independent server processes (a restart is a genuine process restart, not a reload), all pointing at the
# same server-managed save folder, and drives the page with the DevTools protocol driver beside this script:
#   1. mixed  : build a real MA-1-MIXED forest (real map click, Work Plan Approve/Execute by real button clicks, one real
#               Year+1), then Save the slot from the Saved games dialog at both viewport sizes.
#   2. mixed  : a FRESH process (quit/restart). Load the slot from the UI, prove the old fences are refused, advance a real
#               year through the page's own button and show growth/continuation.
#   3. ma-1   : the all-Sitka experiment saves and loads back in place.
#
# Usage: tests/phase1-save-ui/run-edge-e2e.sh [<evidence-dir (Windows path)>]
#   TMPDIR and the evidence dir must be outside the repository. Exit 0 only if every driver assertion passes.
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
edge_exe="${EDGE:-C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/mnt/c/Users/$USER/AppData/Local/Temp/ccf-save-e2e-$stamp}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
evidence_win="$(wslpath -w "$evidence")"
node_win_path="$(wslpath -w "$p1_root/tests/phase1-save-ui/drive.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-save-e2e.XXXXXXXX")"
save_dir="$scratch/xdg/ccf-multi-2d-saves"
mkdir -p "$scratch/fixture" "$scratch/xdg"
cp -- "$p1_root/fixtures/derived/$fixture_name" "$scratch/fixture/$fixture_name"

server_pid=""
server_url=""
server_run=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2> /dev/null; then kill -KILL -"$server_pid" 2> /dev/null || kill -KILL "$server_pid" 2> /dev/null || true; fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

start_server() {
  local run="$1" experiment="$2" attempt
  server_run="$run"
  # Job control gives the child its own process group with the default SIGINT action (a plain background child of a
  # script would inherit SIGINT ignored), so the same Ctrl+C path the player uses is exercised.
  set -m
  ( cd "$p1_root" && exec env XDG_DATA_HOME="$scratch/xdg" "$dotnet_bin" "$ui_dll" "$scratch/fixture" \
      --multi-experiment="$experiment" ) > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
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
  say "SERVER $run pid $server_pid $server_url ($experiment)"
}

stop_server() {
  local run="$1" rc=0
  kill -INT "$server_pid" 2> /dev/null || true
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2> /dev/null; then say "FAIL Z01-$run did not stop on SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fi
  wait "$server_pid" 2> /dev/null || rc=$?
  server_pid=""
  if [[ "$rc" == 0 ]]; then say "PASS Z01-$run stopped cleanly on SIGINT"; else say "FAIL Z01-$run exit $rc"; fi
  if [[ -s "$scratch/server-$run.err" ]]; then
    say "FAIL Z02-$run the server wrote to stderr"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"
  else
    say "PASS Z02-$run the server wrote nothing to stderr"
  fi
  cp -- "$scratch/server-$run.out" "$evidence/server-$run.out" 2> /dev/null || true
}

drive() {
  local command="$1" url="$2" extra=()
  shift 2
  say "DRIVE $command"
  "$node_win" "$node_win_path" "$command" --url "$url" --out "$evidence_win" --slot "${SLOT:-edge-demo}" \
    --old "$evidence_win\\pre-save.json" "$@" 2>&1 | tee -a "$log"
  return "${PIPESTATUS[0]}"
}

fail=0
run() { if "$@"; then :; else fail=1; fi; }

# ---- 1. MA-1-MIXED: play, then save from the dialog ------------------------------------------------------------------
start_server mixed-a ma-1-mixed
run drive prepare-mixed "${server_url%/}/multi"
run drive save "${server_url%/}/multi"
stop_server mixed-a

# ---- 2. Quit and restart: load the slot in a fresh process ----------------------------------------------------------
start_server mixed-b ma-1-mixed
run drive load "${server_url%/}/multi"
stop_server mixed-b

# ---- 3. MA-1 (all Sitka): the same Save/Load controls ---------------------------------------------------------------
start_server ma1 ma-1
SLOT="edge-demo-ma1" run drive reuse "${server_url%/}/multi"
stop_server ma1

if [[ -d "$save_dir" ]]; then
  say "SAVES $(ls -1 "$save_dir" | tr '\n' ' ')"
  cp -- "$save_dir"/*.ccf2d.json "$evidence/" 2> /dev/null || true
fi

say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
