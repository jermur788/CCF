#!/usr/bin/env bash
# CCF Multi 2D — Scots Pine & Site Moisture Part 2 real-browser acceptance suite (Microsoft Edge over CDP).
# Modelled on tests/map-first-lenses-ui/run-map-first-lenses-edge.sh. At BOTH 1920x1080 and 1280x720 against fresh WSL2
# sitkap1-ui processes: New Woodland moisture picker and bog caution, the 0.64 ha Dry Ridge & Moist Slope preset, five live
# lenses, the pine inspector, Plant Scots pine, Execute + Advance, History, then Save v3 -> kill the server -> a NEW process
# -> Load (pine trees, juveniles, deadwood and moisture persist). Servers must stop cleanly on SIGINT with empty stderr.
# Usage: tests/scots-pine-part2-ui/run-scots-pine-part2-edge.sh [<evidence-dir outside the repo>]   (SHOTS=1 saves PNGs)
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/tmp/scotspine-part2-gate/browser-$stamp}"
shots="${SHOTS:-0}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac
evidence_win="$(wslpath -w "$evidence")"
driver_win="$(wslpath -w "$p1_root/tests/scots-pine-part2-ui/drive-scots-pine-part2.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-scotspine-edge.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
mkdir -p "$scratch/fixture"
cp -- "$p1_root/fixtures/derived/$fixture_name" "$scratch/fixture/$fixture_name"
status_before="$(git -C "$repo" status --porcelain --untracked-files=all -- Tools/MultiRackSitkaP1Lab)"

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
  ( cd "$p1_root" && exec env XDG_DATA_HOME="$xdg_dir" "$dotnet_bin" "$ui_dll" "$scratch/fixture" ) \
    > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
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
  if [[ "$rc" == 0 ]]; then say "PASS Z01-$run server stopped cleanly on SIGINT"; else say "FAIL Z01-$run exit $rc"; fail=1; fi
  if [[ -s "$scratch/server-$run.err" ]]; then
    say "FAIL Z02-$run the server wrote to stderr"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"; fail=1
  else
    say "PASS Z02-$run the server wrote nothing to stderr"
  fi
  if grep -qE '" 5[0-9][0-9] |HTTP/1\.[01]" 5[0-9][0-9]|status 5[0-9][0-9]' "$scratch/server-$run.out"; then
    say "FAIL Z03-$run the server log records a 5xx response"; fail=1
  else
    say "PASS Z03-$run no 5xx in the server log"
  fi
  sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' "$scratch/server-$run.out" > "$evidence/server-$run.out" 2> /dev/null || true
}

drive() {
  local tag="$1" mode="$2" width="$3" height="$4" slot="$5" url="$6"
  say "DRIVE $tag ($mode, ${width}x${height}, slot=${slot})"
  "$node_win" "$driver_win" --tag "$tag" --mode "$mode" --width "$width" --height "$height" --slot "$slot" \
    --url "$url" --out "$evidence_win" --shots "$shots" 2>&1 \
    | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | tee -a "$log"
  return "${PIPESTATUS[0]}"
}

fail=0
run() { if "$@"; then :; else fail=1; fi; }

viewports="${VIEWPORTS:-1920x1080 1280x720}"   # full acceptance runs both; VIEWPORTS narrows a local iteration only
for vp in $viewports; do
  w="${vp%x*}"; h="${vp#*x}"; tag="vp$w"
  start_server "sp2-$w-main" "$scratch/xdg-$w"
  run drive "$tag" main "$w" "$h" "sp2-v3-$w" "${server_url%/}/multi"
  stop_server "sp2-$w-main"
  start_server "sp2-$w-load" "$scratch/xdg-$w"
  run drive "$tag" load "$w" "$h" "sp2-v3-$w" "${server_url%/}/multi"
  stop_server "sp2-$w-load"
done

status_after="$(git -C "$repo" status --porcelain --untracked-files=all -- Tools/MultiRackSitkaP1Lab)"
if [[ "$status_before" == "$status_after" ]]; then say "PASS Z04 git status of the lab unchanged by the run"; else say "FAIL Z04 the run changed the lab tree"; fail=1; fi

vp1920_pass="$(grep -c '^PASS \[1920x1080\]' "$log" || true)"
vp1920_fail="$(grep -c '^FAIL \[1920x1080\]' "$log" || true)"
vp1280_pass="$(grep -c '^PASS \[1280x720\]' "$log" || true)"
vp1280_fail="$(grep -c '^FAIL \[1280x720\]' "$log" || true)"
say "VIEWPORT 1920x1080: ${vp1920_pass} passed, ${vp1920_fail} failed"
say "VIEWPORT 1280x720: ${vp1280_pass} passed, ${vp1280_fail} failed"
say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
