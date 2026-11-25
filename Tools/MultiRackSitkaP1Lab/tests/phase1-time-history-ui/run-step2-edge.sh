#!/usr/bin/env bash
# Phase 1 Step 2 real-browser acceptance: Time Pacing (Skip to Review), Decision->Outcome History drawer,
# Woodland Structure (M_i & T_i), bounded multi-decade payloads, NB-2 Plant-drawer fold polish, and
# Save -> quit -> restart -> Load continuity in installed Microsoft Edge (Windows) against real WSL2 UI processes.
#
# Runs three independent server processes (genuine process restart for Save/Load continuity):
#   1. ma-1         : 1920x1080 & 1280x720 matrix (zero h-scroll, topbar single row, R3 5-competitor fold),
#                     NB-2 Plant-drawer partial-refusal fold proof at BOTH 1280x720 and 1920x1080,
#                     Year-0 observation skip (+4 Obs., GAMEPLAY CALIBRATION), WorkPlanOpen refusal,
#                     post-Fell anchor + overdue fallback, MA-1 Structure (M_i=0.00, DERIVED, exact disclaimer),
#                     and measured raw payload budgets at Year 0, Year 5, Year 15+, and Year 25+ truncation proof.
#   2. ma-1-mixed-a : 1920x1080 & 1280x720 matrix (zero h-scroll, topbar single row, R3 5-competitor fold),
#                     Plant/Shelter/SpotClear/Clear/Fell/Prune -> Approve -> Execute -> Skip -> Year 15+,
#                     measured raw payload budgets at Year 0, Year 5, Year 15+, and Year 20+,
#                     History drawer 15+ yr paging, Work(Y) vs Outcome(Y+1), all 6 provenance pills, NOT RECORDED,
#                     non-causal note (no weather), zero economy, Escape preserves draft Fell tick,
#                     Structure (M_i > 0, DERIVED, disclaimer), Save slot to server-managed folder.
#   3. ma-1-mixed-b : FRESH restarted server process: Load slot from Saved games dialog, verify restored Preview,
#                     History index + Year 0 detail, and Structure match pre-save exactly.
#
# Usage: tests/phase1-time-history-ui/run-step2-edge.sh [<evidence-dir>]
#   TMPDIR and the evidence dir must be outside the repository. Exit 0 only if every assertion passes.
set -uo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_win="${NODE_WIN:-/mnt/c/Program Files/nodejs/node.exe}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
stamp="$(date -u +%Y%m%dT%H%M%SZ)"
evidence="${1:-/mnt/c/Users/$USER/AppData/Local/Temp/ccf-step2-edge-$stamp}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -x "$node_win" ]] || { echo "STOP: Windows node not found at $node_win (set NODE_WIN)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
[[ -f "$p1_root/fixtures/derived/$fixture_name" ]] || { echo "STOP: fixture missing" >&2; exit 2; }

mkdir -p -- "$evidence"
case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: evidence dir is inside repository" >&2; exit 2 ;; esac
evidence_win="$(wslpath -w "$evidence")"
driver_win="$(wslpath -w "$p1_root/tests/phase1-time-history-ui/drive-step2.mjs")"
log="$evidence/run.log"
: > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-step2-edge.XXXXXXXX")"
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
  local run="$1" experiment="$2" attempt
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
  say "SERVER $run pid $server_pid (${experiment})"
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
  local command="$1" url="$2"
  shift 2
  say "DRIVE $command"
  "$node_win" "$driver_win" "$command" --url "$url" --out "$evidence_win" --slot "step2-e2e" \
    --snap "$evidence_win\\step2-pre-save.json" "$@" 2>&1 | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | tee -a "$log"
  return "${PIPESTATUS[0]}"
}

fail=0
run() { if "$@"; then :; else fail=1; fi; }

# ---- 1. MA-1 (all Sitka): 4-cell matrix half, NB-2 fold proof, Skip pacing, Structure, Year 0/5/15+/25+ budgets ----
start_server ma1 ma-1
run drive ma1-full "${server_url%/}/multi"
stop_server ma1

# ---- 2. MA-1-MIXED: matrix half, Plant/Fell/Prune/Clear -> Skip -> Year 15+, History paging/detail, Structure, Save ----
start_server mixed-a ma-1-mixed
run drive mixed-play-save "${server_url%/}/multi"
stop_server mixed-a

# ---- 3. Quit & Restart fresh server: Load slot and verify Preview, History pages, and Structure match ----
start_server mixed-b ma-1-mixed
run drive mixed-restart-load "${server_url%/}/multi"
stop_server mixed-b

# ---- 4. Clean per-year Year 0..30 payload & retention sweep on BOTH MA-1 and MA-1-MIXED (Owner G5 Year 30 horizon) ----
sweep_experiment() {
  local exp="$1"
  start_server "sweep-${exp}" "$exp"
  python3 -I -B - "${server_url}" "$exp" "$evidence/step2-sweep-${exp}.json" << 'SWEEP_PY' | tee -a "$log"
import json, sys, urllib.request
base, exp, out_json = sys.argv[1], sys.argv[2], sys.argv[3]
origin = base.split('/s/')[0]

def send(path, body=None):
    h = {'Origin': origin}
    data = None
    if body is not None:
        h['Content-Type'] = 'application/json'
        data = json.dumps(body).encode()
    req = urllib.request.Request(base + path, data=data, headers=h)
    raw = urllib.request.urlopen(req, timeout=30).read()
    return raw, json.loads(raw)

def fence(v):
    return {'sessionId': v['sessionId'], 'source': v['source'], 'planStateRevision': v['planStateRevision']}

def exec_work(v, p, yr):
    a_live = [t['treeId'] for t in v['areas'][0]['trees'] if t['isLiving']]
    if not a_live:
        return v
    _, r = send('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': a_live[0], 'reason': f'g5 sweep y{yr} fell'})
    v = r['view']
    if exp == 'ma-1-mixed':
        for tid in [t['treeId'] for t in v['areas'][1]['trees'] if t['isLiving']]:
            try:
                _, r = send('api/multi/prune/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': tid})
                v = r['view']
                break
            except Exception:
                pass
        cell = p['stage4']['areas'][1]['ground'][yr % len(p['stage4']['areas'][1]['ground'])]
        _, r = send('api/multi/clear/add', {'fence': fence(v), 'areaId': 'MA-B', 'column': cell['column'], 'row': cell['row']})
        v = r['view']
        extB = v['areas'][1]['extent']
        liveAll = [t for a in v['areas'] for t in a['trees'] if t['isLiving']]
        existing_pts = [(j['xm'], j['zm']) for j in p['ecology']['planted']] + [(pa['xm'], pa['zm']) for pa in p['ecology']['patches']]
        for c in p['stage4']['areas'][1]['ground']:
            x = extB['minXM'] + c['centreU']
            z = extB['minZM'] + extB['sideM'] - c['centreV']
            if all((t['xm'] - x) ** 2 + (t['zm'] - z) ** 2 >= 1.8 for t in liveAll) and \
               all((px - x) ** 2 + (pz - z) ** 2 >= 1.0 for px, pz in existing_pts):
                try:
                    _, r = send('api/multi/plant/add', {'fence': fence(v), 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': x, 'zM': z})
                    v = r['view']
                    _, r = send('api/multi/shelter/add', {'fence': fence(v), 'areaId': 'MA-B', 'xM': x, 'zM': z})
                    v = r['view']
                    _, r = send('api/multi/spot-clear/add', {'fence': fence(v), 'areaId': 'MA-B', 'xM': x, 'zM': z})
                    v = r['view']
                    break
                except Exception:
                    pass
    _, r = send('api/multi/plan/approve', {'fence': fence(v)})
    v = r['view']
    _, r = send('api/multi/plan/execute', {'fence': fence(v), 'planId': v['approved']['planId'], 'executionKey': v['approved']['executionKey']})
    return r['view']

_, p0 = send('api/multi/view')
v = exec_work(p0['view'], p0, 0)
rows = []
bad = 0
for yr in range(0, 31):
    if yr > 0:
        _, r = send('api/multi/year/advance', {'fence': fence(v), 'consentToCancelWork': False})
        v = r['view']
    if yr in (10, 20, 30):
        _, p_pre = send('api/multi/view')
        v = exec_work(p_pre['view'], p_pre, yr)
    raw, p = send('api/multi/view')
    text = raw.decode('utf-8')
    idx = text.index('"ecology":') + len('"ecology":')
    _, end = json.JSONDecoder().raw_decode(text, idx)
    eco_b = len(text[idx:end].encode('utf-8'))
    ev_c = len(p['view']['events'])
    an_c = len(p['view']['annualHistory'])
    ok = (len(raw) <= 524288 and eco_b <= 153600 and p.get('payloadBudgetExceeded') is False and
          ev_c >= 1 and (yr == 0 or an_c >= 1))
    if p.get('stage4Compacted'):
        g0 = p['stage4']['areas'][0]['ground'][0]
        if 'contributingTreeIds' in g0 or not all(k in g0['polygon'][0] for k in ('xm', 'zm', 'u', 'v')):
            ok = False
    if not ok:
        bad += 1
    rows.append({
        'year': yr, 'rawBytes': len(raw), 'ecologyBytes': eco_b,
        'eventsKept': ev_c, 'annualKept': an_c,
        'totalRecordedYears': p['totalRecordedYears'],
        'historyTruncated': p['historyTruncated'],
        'projectionCompacted': p['projectionCompacted'],
        'stage4Compacted': p['stage4Compacted'],
        'payloadBudgetExceeded': p['payloadBudgetExceeded'],
    })
    print(f"SWEEP {exp} Y{yr:02d} raw={len(raw)} eco={eco_b} ev={ev_c} an={an_c} trunc={p['historyTruncated']} proj={p['projectionCompacted']} s4c={p['stage4Compacted']} over={p['payloadBudgetExceeded']}")
with open(out_json, 'w', encoding='utf-8') as f:
    json.dump(rows, f, indent=2)
max_raw = max(r['rawBytes'] for r in rows)
max_eco = max(r['ecologyBytes'] for r in rows)
if bad == 0:
    print(f"PASS G5 Year 0..30 sweep {exp}: all 31 years <= 524288 B raw (max {max_raw}) and <= 153600 B ecology (max {max_eco}), >=1 event & >=1 annual record retained")
    sys.exit(0)
else:
    print(f"FAIL G5 Year 0..30 sweep {exp}: {bad} year(s) failed budget or retention (max_raw={max_raw}, max_eco={max_eco})")
    sys.exit(1)
SWEEP_PY
  local rc="${PIPESTATUS[0]}"
  stop_server "sweep-${exp}"
  return "$rc"
}

run sweep_experiment ma-1
run sweep_experiment ma-1-mixed

say "EVIDENCE $evidence"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit $fail
