#!/usr/bin/env bash
# Multi #885 MA-1: a bash + curl + python3 + node witness of the two-area simulator served by the BUILT UI process. It
# starts the process against a scratch copy of the committed P1 fixture (the MA-1 experiment itself is generated in code),
# with hostile URL environment variables set, on an OS-assigned 127.0.0.1 port, and then checks:
#   - M: the new page, style and script are served with the same strict headers; multi.js passes node --check; the
#     published manifest's SHA-256 body hash equals its content hash, the token's hash and the pinned value;
#   - G: the unchanged guards also cover every new route: 404 without the capability, 403 for a wrong Host or Origin, 405,
#     415, 413, and 400 for malformed, duplicate, unknown, missing or null members (each refusal leaves the view intact);
#   - W: one full two-area workflow through the JSON API: read-only competition for non-Crop trees in BOTH areas, Crop,
#     reasoned Fell in A and B, stale and wrong-area refusals, Approve at zero Fell, an unminted key refused, Execute (one
#     receipt, both areas changed), replay once, an altered replay refused, Year+1 refused with the old fence and committed
#     with a fresh one, consent, hit-tests, and the owner's "never Fell an area to zero" safeguard over HTTP;
#   - J: the page's own served multi.js, run in a small DOM stand-in against this real process: any-tree competitors before
#     Crop, area switching with exact IDs, map click, Crop, typed and release reasons, Approve, the Execute confirmation,
#     the receipt, Year+1 results, and the fail-closed lock after an unconfirmed outcome;
#   - SIGINT ends the process cleanly; no file appears in the lab tree; git status of the lab is unchanged.
# The DOM stand-in is copied from tests/ui-smoke.sh and is NOT a browser: no layout, CSS, real focus or input devices.
#
# Usage: tests/ui-smoke-multiarea.sh [<evidence-dir>]
#   Build first: ~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj   (DOTNET overrides the dotnet; NODE overrides node)
#   TMPDIR and <evidence-dir> must be outside the repository. Exit 0 only if every check passes.
set -euo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_bin="${NODE:-$(command -v node || true)}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
pinned_hash="c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95"
evidence="${1:-}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
command -v curl > /dev/null && command -v python3 > /dev/null || { echo "STOP: curl and python3 are required" >&2; exit 2; }
[[ -n "$node_bin" && -x "$node_bin" ]] || { echo "STOP: node is required for the page witness (set NODE)" >&2; exit 2; }
if [[ -n "$evidence" ]]; then
  mkdir -p -- "$evidence"
  case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: the evidence directory is inside the repository" >&2; exit 2 ;; esac
fi

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-ma1-uismoke.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
log="$scratch/ui-smoke-multiarea.txt"
: > "$log"
server_pid=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2> /dev/null; then kill -KILL "$server_pid" 2> /dev/null || true; fi
  if [[ -n "$evidence" ]]; then cp -- "$log" "$evidence/ui-smoke-multiarea.txt" 2> /dev/null || true; fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

fail=0
say() { echo "$*" | tee -a "$log"; }
pass() { say "PASS $*"; }
bad() { say "FAIL $*"; fail=1; }
check() { local label="$1"; shift; if "$@"; then pass "$label"; else bad "$label"; fi; }

tree_snapshot() { (cd "$p1_root" && find . -printf '%y %p %s %T@\n' | LC_ALL=C sort); }
git_status() { git -C "$repo" status --porcelain --untracked-files=all -- Tools/MultiRackSitkaP1Lab Tools/MultiRackSitkaLab; }
tree_before="$(tree_snapshot)"; status_before="$(git_status)"
say "ui-smoke-multiarea $(date -u +%FT%TZ) HEAD $(git -C "$repo" rev-parse HEAD) dotnet $("$dotnet_bin" --version) node $(cd "$scratch" && "$node_bin" --version)"

mkdir "$scratch/fixture"
cat -- "$p1_root/fixtures/derived/$fixture_name" > "$scratch/fixture/$fixture_name"

# start_server <run> : starts one fresh UI process (one fresh in-memory session) and sets server_pid, url, port, cap, origin.
# Job control gives the process its own process group with the default SIGINT action, as in an interactive terminal.
start_server() {
  local run="$1"
  set -m
  ( cd "$p1_root" && exec env ASPNETCORE_URLS="http://0.0.0.0:5187" DOTNET_URLS="http://0.0.0.0:5187" ASPNETCORE_HTTP_PORTS=5187 \
      Kestrel__Endpoints__Lan__Url="http://0.0.0.0:5187" ASPNETCORE_ENVIRONMENT=Development \
      XDG_DATA_HOME="$scratch/xdg" \
      "$dotnet_bin" "$ui_dll" "$scratch/fixture" ) > "$scratch/server-$run.out" 2> "$scratch/server-$run.err" &
  server_pid=$!
  set +m
  url=""
  for _ in $(seq 1 300); do
    url="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$scratch/server-$run.out" | head -1 || true)"
    [[ -n "$url" ]] && break
    kill -0 "$server_pid" 2> /dev/null || break
    sleep 0.1
  done
  if [[ -z "$url" ]]; then
    bad "S01-$run the process printed no launch URL"; sed 's/^/    /' "$scratch/server-$run.err" | tee -a "$log"; exit 1
  fi
  port="${url#http://127.0.0.1:}"; port="${port%%/*}"
  cap="${url#*/s/}"; cap="${cap%/}"
  origin="http://127.0.0.1:$port"
  say "run $run: server pid $server_pid (this script's own process), port $port"
  check "S01-$run the process prints the two-area page address on the same listener" \
    grep -qx "Two-area MA-1 simulator: http://127.0.0.1:$port/s/$cap/multi" "$scratch/server-$run.out"
  check "S02-$run the port is ephemeral, not 5187 and not privileged" test "$port" != 5187 -a "$port" -ge 1024
}

# stop_server <run> : stops this script's own process like Ctrl+C and checks that it ended cleanly.
stop_server() {
  local run="$1" saved="${2:-0}" rc=0
  kill -INT "$server_pid"
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2> /dev/null; then bad "Z01-$run still running 15 s after SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fi
  wait "$server_pid" 2> /dev/null || rc=$?
  server_pid=""
  if [[ "$saved" == 1 ]]; then
    check "Z01-$run SIGINT ends this script's own process with exit 0 and the truthful 'saved games remain' line (exit $rc)" \
      bash -c 'test "$1" == 0 && grep -qx "Session ended. Saved games remain in their slot files." "$2"' _ "$rc" "$scratch/server-$run.out"
  else
    check "Z01-$run SIGINT ends this script's own process with exit 0 and 'Session ended; nothing was saved.' (exit $rc)" \
      bash -c 'test "$1" == 0 && grep -qx "Session ended; nothing was saved." "$2"' _ "$rc" "$scratch/server-$run.out"
  fi
  check "Z02-$run the server wrote nothing to stderr (no unconfirmed outcome, no fault)" test ! -s "$scratch/server-$run.err"
}

start_server api

req() {
  local name="$1" method="$2" path="$3"; shift 3
  curl --silent --noproxy '*' --max-time 15 -o "$scratch/$name.body" -D "$scratch/$name.head" -w '%{http_code}' \
    -X "$method" "$@" "$origin$path" || true
}
header() { grep -i "^$2:" "$scratch/$1.head" | head -1 | cut -d: -f2- | tr -d '\r' | sed 's/^ *//'; }

# ---- M: page assets and manifest provenance --------------------------------------------------------------------------
code="$(req page GET "/s/$cap/multi")"
check "M01 GET multi: 200 text/html with only its own external script and style" bash -c \
  'test "$1" == 200 && grep -q "<script src=\"multi.js\" defer></script>" "$2" && grep -q "<link rel=\"stylesheet\" href=\"multi.css\">" "$2" && ! grep -qi "<script>" "$2" && ! grep -qi " style=" "$2"' _ "$code" "$scratch/page.body"
check "M02 the page answer has the strict headers: CSP self only, no-store, nosniff, no-referrer, DENY framing, no CORS, no Server" bash -c '
  h="$1"; ! grep -qi "^access-control-" "$h" && ! grep -qi "^server:" "$h" && grep -qi "^cache-control: no-store" "$h" &&
  grep -qi "^content-security-policy: default-src .none.; script-src .self.; style-src .self." "$h" && grep -qi "^x-content-type-options: nosniff" "$h" &&
  grep -qi "^referrer-policy: no-referrer" "$h" && grep -qi "^x-frame-options: DENY" "$h" && grep -qi "^content-type: text/html" "$h"' _ "$scratch/page.head"
code="$(req css GET "/s/$cap/multi.css")"; check "M03 multi.css: 200 text/css" test "$code" == 200 -a "$(header css content-type)" == "text/css; charset=utf-8"
code="$(req jsfile GET "/s/$cap/multi.js")"; check "M04 multi.js: 200 text/javascript" test "$code" == 200 -a "$(header jsfile content-type)" == "text/javascript; charset=utf-8"
cp "$scratch/jsfile.body" "$scratch/multi.js"
check "M05 the served multi.js passes node --check" "$node_bin" --check "$scratch/multi.js"
code="$(req manifest GET "/s/$cap/api/multi/manifest")"
code_view="$(req v0 GET "/s/$cap/api/multi/view")"
check "M06 GET api/multi/manifest and api/multi/view: 200 and 200" test "$code" == 200 -a "$code_view" == 200
check "M07 the manifest's SHA-256 body hash equals its contentHash, the view token, the experiment record and the pinned $pinned_hash" \
  python3 -I -B -c 'import hashlib, json, sys
raw = open(sys.argv[1], "rb").read(); text = raw.decode("ascii")
cut = text.rindex(",\"contentHash\":\"")
body_hash = hashlib.sha256((text[:cut] + "}").encode("ascii")).hexdigest()
manifest = json.loads(text); view = json.load(open(sys.argv[2]))["view"]
ok = body_hash == manifest["contentHash"] == view["source"]["manifestContentHash"] == view["experiment"]["manifestContentHash"] == sys.argv[3]
ok = ok and manifest["experimentId"] == "mad-sitka-ma1-two-area-v1" and manifest["reviewStatus"] == "PendingIndependentReview"
ok = ok and [a["areaId"] for a in manifest["areas"]] == ["MA-A", "MA-B"] and all(a["site"] == "UNKNOWN" for a in manifest["areas"])
sys.exit(0 if ok else 1)' "$scratch/manifest.body" "$scratch/v0.body" "$pinned_hash"
check "M08 zero data:image/webp remains in served multi, multi.css and multi.js" bash -c \
  '! grep -q "data:image/webp" "$1" "$2" "$3"' _ "$scratch/page.body" "$scratch/css.body" "$scratch/jsfile.body"
check "M09 all 21 required inline SVG <defs> IDs exist in served multi HTML" \
  python3 -I -B -c 'import sys
html = open(sys.argv[1], encoding="utf-8").read()
ids = ["ccf-sitka","ccf-oak","ccf-beech","ccf-standing-dead","ccf-log","ccf-crop-ring",
       "ccf-sapling-sitka","ccf-sapling-oak","ccf-sapling-beech","ccf-shelter-active","ccf-shelter-expired",
       "ccf-spot-clear","ccf-natural-seedlings","ccf-bramble-hatch","ccf-bracken-hatch",
       "ccf-action-fell","ccf-action-prune","ccf-action-clear","ccf-action-plant","ccf-action-shelter","ccf-action-spot"]
sys.exit(0 if all(f"id=\"{i}\"" in html for i in ids) else 1)' "$scratch/page.body"
check "M10 GET api/multi/manifest reports truthful experimentBadge and 1/1/1/1/1 models for ma-1" \
  python3 -I -B -c 'import json, sys
m = json.load(open(sys.argv[1], encoding="utf-8"))
ok = m.get("experimentKey") == "ma-1" and m.get("experimentBadge") == "Synthetic MA-1 (all Sitka)"
ok = ok and m.get("models") == {"adult": 1, "mortality": 1, "crown": 1, "regeneration": 1, "species": 1}
sys.exit(0 if ok else 1)' "$scratch/manifest.body"
check "M11 GET api/multi/view raw JSON <= 512 KiB and raw ecology span <= 150 KiB on ma-1" \
  python3 -I -B -c 'import json, sys
raw = open(sys.argv[1], "rb").read()
text = raw.decode("utf-8")
obj = json.loads(text)
idx = text.index("\"ecology\":") + len("\"ecology\":")
_, end = json.JSONDecoder().raw_decode(text, idx)
eco_bytes = len(text[idx:end].encode("utf-8"))
sys.exit(0 if len(raw) <= 524288 and eco_bytes <= 153600 and obj["ecology"]["models"]["species"] == 1 else 1)' "$scratch/v0.body"

# ---- G and W: guards and the full two-area workflow through the JSON API --------------------------------------------------
cat > "$scratch/workflow.py" << 'WORKFLOW'
import copy, json, sys, urllib.error, urllib.request
base = sys.argv[1]
origin = base.split('/s/')[0]
count = 0

def send(path, body=None, raw=None, headers=None, method=None, target=None):
    h = {'Origin': origin, 'Content-Type': 'application/json'}
    h.update(headers or {})
    data = raw if raw is not None else json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request((target or base) + path, data=data, headers=h, method=method)
    try: r = urllib.request.urlopen(req, timeout=15)
    except urllib.error.HTTPError as e: r = e
    content = r.read()
    try: payload = json.loads(content) if content else None
    except ValueError: payload = None
    return r.status, payload

def view(): return send('api/multi/view')[1]['view']
def fence(v): return {'sessionId': v['sessionId'], 'source': v['source'], 'planStateRevision': v['planStateRevision']}
def area(v, area_id): return next(a for a in v['areas'] if a['areaId'] == area_id)
def living(v, area_id): return [t for t in area(v, area_id)['trees'] if t['isLiving']]

def check(ok, title):
    global count
    count += 1
    print(('PASS' if ok else 'FAIL') + ' W%03d ' % count + title, flush=True)
    if not ok: raise AssertionError(title)

def ok(path, body, title):
    code, payload = send(path, body)
    check(code == 200 and payload['refusal'] is None, title + ' (HTTP %d)' % code)
    return payload

def refused(path, body, want, title, **kw):
    before = view()
    code, payload = send(path, body, **kw)
    got = (payload or {}).get('refusal') or (payload or {}).get('error') or {}
    expected_status = 409 if want[0].isupper() and not want.startswith('HTTP') else int(want[4:])
    check(code == expected_status and (expected_status != 409 or got.get('code') == want) and view() == before,
          title + ' -> HTTP %d %s; the entire view is unchanged' % (code, got.get('code')))
    return payload

v0 = view()
check(len(v0['areas']) == 2 and [a['areaId'] for a in v0['areas']] == ['MA-A', 'MA-B'], 'two areas MA-A and MA-B')
check([len(a['trees']) for a in v0['areas']] == [171, 73] and all(t['isLiving'] for a in v0['areas'] for t in a['trees']), '171 and 73 living trees')
check([v0['source'][k] for k in ('ecologicalYear', 'annualRevision', 'workRevision')] + [v0['planStateRevision']] == [0, 0, 0, 0], 'fresh clocks')
check(v0['areas'][0]['areaM2Exact'] == '915' and v0['areas'][1]['areaM2Exact'] == '1500' and v0['areas'][0]['simulationSeed'] != v0['areas'][1]['simulationSeed'],
      'geometry-derived areas and distinct seeds')
ids = [t['treeId'] for a in v0['areas'] for t in a['trees']]
check(len(set(ids)) == len(ids) and not any(i.startswith('P30-') for i in ids) and all(t['treeId'].startswith(a['areaId'] + '-') for a in v0['areas'] for t in a['trees']),
      'globally unique, area-prefixed IDs; no P30- ID')

# Guards on the new routes: the same HandleAsync guards answer before any adapter call.
f0 = fence(v0)
body = {'fence': f0, 'areaId': 'MA-A', 'treeId': v0['areas'][0]['trees'][0]['treeId']}
refused('api/multi/competition', body, 'HTTP403', 'foreign Origin', headers={'Origin': 'http://example.invalid'})
refused('api/multi/crop/mark', body, 'HTTP403', 'wrong Host', headers={'Host': 'localhost:' + origin.rsplit(':', 1)[1]})
refused('api/multi/pending/add', dict(body, reason='x'), 'HTTP404', 'wrong capability', target=origin + '/s/' + '0' * 64 + '/')
refused('api/multi/plan/approve', {'fence': f0}, 'HTTP405', 'GET on a POST route', method='GET')
refused('api/multi/plan/approve', {'fence': f0}, 'HTTP415', 'text/plain', headers={'Content-Type': 'text/plain'})
refused('api/multi/plan/approve', None, 'HTTP413', 'body over 16 KiB', raw=b' ' * 17000)
refused('api/multi/plan/approve', None, 'HTTP400', 'malformed JSON', raw=b'{"fence":')
refused('api/multi/plan/approve', None, 'HTTP400', 'JSON null body', raw=b'null')
refused('api/multi/plan/approve', {}, 'HTTP400', 'missing fence')
refused('api/multi/plan/approve', {'fence': None}, 'HTTP400', 'null fence')
for member in list(f0['source']):
    b = copy.deepcopy({'fence': f0}); del b['fence']['source'][member]
    refused('api/multi/plan/approve', b, 'HTTP400', 'missing token member ' + member)
refused('api/multi/competition', dict(body, extra=1), 'HTTP400', 'unknown member')
refused('api/multi/competition', dict(body, AreaId='MA-B'), 'HTTP400', 'case-shifted member name')
refused('api/multi/competition', None, 'HTTP400', 'duplicate member', raw=json.dumps(body).replace('"treeId":', '"treeId":"MA-A-002","treeId":').encode())
b = copy.deepcopy(body); b['fence']['source']['workRevision'] = '0'
refused('api/multi/competition', b, 'HTTP400', 'string in a numeric token member')
refused('api/multi/competition', dict(body, treeId=''), 'HTTP400', 'empty tree ID')
refused('api/multi/competition', dict(body, treeId='MA-A 001'), 'HTTP400', 'tree ID with whitespace')
refused('api/multi/pending/add', dict(body, reason='r' * 2001), 'HTTP400', 'reason over 2000 characters')
refused('api/multi/hit', {'fence': f0, 'areaId': 'MA-A', 'u': 1}, 'HTTP400', 'hit-test without v')
refused('api/multi/year/advance', {'fence': f0}, 'HTTP400', 'Year+1 without an explicit consent member')

# Read-only competition on NON-Crop living trees of BOTH areas, before any designation.
for area_id, index in (('MA-A', 60), ('MA-B', 20)):
    tree = area(v0, area_id)['trees'][index]
    code, r = send('api/multi/competition', {'fence': f0, 'areaId': area_id, 'treeId': tree['treeId']})
    d = r['competition']
    check(code == 200 and r['refusal'] is None and r['areaId'] == area_id and d['targetTreeId'] == tree['treeId'] and
          len(d['topFive']) == min(5, len(d['neighbours'])) and all(n['treeId'].startswith(area_id + '-') for n in d['neighbours']),
          area_id + ' non-Crop tree %s: ranked competitors from its own area only (%d neighbours, CI %.4f)' % (tree['treeId'], len(d['neighbours']), d['currentTotal']))
    check(all(n['strength'] in ('Low', 'Moderate', 'Strong') for n in d['topFive']) and d['topFive'] == sorted(d['neighbours'], key=lambda n: (-n['contribution'], n['treeId']))[:5],
          area_id + ' top five carry share and strength, strongest first')
check(view() == v0, 'competition reads changed nothing: no Crop, no pending, same revisions')
refused('api/multi/competition', {'fence': f0, 'areaId': 'MA-B', 'treeId': area(v0, 'MA-A')['trees'][3]['treeId']}, 'MultiAreaWrongArea', 'competition for an A tree under B')
refused('api/multi/competition', {'fence': f0, 'areaId': 'MA-A', 'treeId': 'MA-A-999'}, 'CompetitionTreeUnknown', 'competition for an unknown ID')

# Crop in A, a reasoned release Fell in A and a typed Fell in B: one combined plan.
crop = area(v0, 'MA-A')['trees'][60]['treeId']
v = ok('api/multi/crop/mark', {'fence': f0, 'areaId': 'MA-A', 'treeId': crop}, 'Crop ' + crop)['view']
check(area(v, 'MA-A')['cropTreeIds'] == [crop] and v['planStateRevision'] == 1, 'Crop is explicit session state; plan-state 1')
competitor = send('api/multi/competition', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': crop})[1]['competition']['topFive'][0]['treeId']
b_tree = area(v0, 'MA-B')['trees'][20]['treeId']
stale = fence(v0)
v = ok('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': competitor, 'reason': 'Competition release for retained Crop Tree ' + crop + ' (operator intent)'}, 'Fell ' + competitor + ' in A')['view']
v = ok('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': b_tree, 'reason': 'ui-smoke-multiarea: manual selection in B'}, 'Fell ' + b_tree + ' in B')['view']
check([(o['areaId'], o['treeId']) for o in v['pending']] == [('MA-A', competitor), ('MA-B', b_tree)], 'one pending list covers both areas')
# The northernmost A tree is far from the Crop, so it is neither the Crop nor its pending competitor.
far = area(v0, 'MA-A')['trees'][-1]['treeId']
check(far not in (crop, competitor), 'probe tree ' + far + ' is not the Crop or its competitor')
refused('api/multi/pending/add', {'fence': stale, 'areaId': 'MA-A', 'treeId': far, 'reason': 'stale'}, 'PlayerPlanStateStale', 'Add with a stale fence')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': far, 'reason': 'wrong area'}, 'MultiAreaWrongArea', 'Add an A tree under B')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': crop, 'reason': 'crop'}, 'CropTreeRetained', 'Fell the retained Crop')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': competitor, 'reason': 'twice'}, 'WorkDuplicateTreeId', 'duplicate ID')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-C', 'treeId': far, 'reason': 'no area'}, 'MultiAreaUnknownArea', 'unknown area')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-A', 'treeId': far, 'reason': '   '}, 'WorkReasonRequired', 'blank reason')

# Approve fells ZERO; the engine mints the PlanId and key.
p = ok('api/multi/plan/approve', {'fence': fence(v)}, 'Approve the two-area plan')
v = p['view']; plan = v['approved']
check(plan['planId'] == v['sessionId'] + ':ma-plan:%d' % v['planStateRevision'] and plan['executionKey'] == plan['planId'] + ':execute', 'engine-minted PlanId and key')
check([a['stand']['livingCount'] for a in v['areas']] == [171, 73] and v['events'] == [] and v['source']['workRevision'] == 0 and v['pending'] == [],
      'Approve fells zero trees in both areas; no event')
check({i['treeId']: i['status'] for i in p['scenes']['MA-A'] + p['scenes']['MA-B'] if i['treeId'] in (competitor, b_tree)} == {competitor: 'ApprovedFell', b_tree: 'ApprovedFell'},
      'both maps show the approved trees still standing')
refused('api/multi/plan/execute', {'fence': fence(v), 'planId': plan['planId'], 'executionKey': plan['planId'] + ':other'}, 'WorkExecutionKeyConflict', 'Execute under an unminted key')

# Execute: one receipt, both areas changed, exactly the approved trees felled.
approved_fence = fence(v)
e = ok('api/multi/plan/execute', {'fence': approved_fence, 'planId': plan['planId'], 'executionKey': plan['executionKey']}, 'Execute')
v = e['view']; ev = e['execution']['event']
check(not e['execution']['replayed'] and len(v['events']) == 1 and v['source']['workRevision'] == 1 and v['source']['ecologicalYear'] == 0 and v['approved'] is None,
      'one event; work revision 0 -> 1; same year; approval cleared')
check([c['areaId'] for c in ev['areas']] == ['MA-A', 'MA-B'] and [c['before']['livingCount'] for c in ev['areas']] == [171, 73] and
      [c['after']['livingCount'] for c in ev['areas']] == [170, 72] and all(c['after']['basalAreaM2PerHa'] < c['before']['basalAreaM2PerHa'] for c in ev['areas']),
      'receipt per area: living 171 -> 170 and 73 -> 72, basal area falls in both')
check(sorted((f['areaId'], f['treeId'], f['cause'], f['mechanism']) for c in ev['areas'] for f in c['fells']) ==
      sorted([('MA-A', competitor, 'fell', 'manual-work-plan'), ('MA-B', b_tree, 'fell', 'manual-work-plan')]), 'typed Fell of exactly the approved IDs, in their own areas')
check(all(c['after'] == area(v, c['areaId'])['stand'] for c in ev['areas']), 'the receipt after-stands equal the committed stands')
print('INFO receipt BA m2/ha', [(c['areaId'], round(c['before']['basalAreaM2PerHa'], 3), round(c['after']['basalAreaM2PerHa'], 3)) for c in ev['areas']], flush=True)
code, r = send('api/multi/plan/execute', {'fence': approved_fence, 'planId': plan['planId'], 'executionKey': plan['executionKey']})
check(code == 200 and r['execution']['replayed'] and r['execution']['event'] == ev and len(r['view']['events']) == 1 and r['view']['source']['workRevision'] == 1,
      'identical replay returns the same receipt once; no second Fell')
refused('api/multi/plan/execute', {'fence': approved_fence, 'planId': plan['planId'] + 'x', 'executionKey': plan['executionKey']}, 'WorkExecutionKeyConflict', 'altered replay (other PlanId)')
refused('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': b_tree, 'reason': 'again'}, 'WorkTreeNotLive', 'Fell an already felled tree')

# Year+1: refused with the pre-Execute fence; committed with a fresh one; per-area results.
refused('api/multi/year/advance', {'fence': approved_fence, 'consentToCancelWork': False}, 'PlayerSourceStale', 'Year+1 with the pre-Execute fence')
y = ok('api/multi/year/advance', {'fence': fence(view()), 'consentToCancelWork': False}, 'Year+1')
v = y['view']; record = v['annualHistory'][-1]
check(v['source']['ecologicalYear'] == 1 and v['source']['annualRevision'] == 1 and [r['areaId'] for r in y['annual']['areas']] == ['MA-A', 'MA-B'] and
      all(r['report'] is not None for r in y['annual']['areas']), 'one shared year; one annual report per area')
check([a['before']['livingCount'] for a in record['areas']] == [170, 72] and not record['cancelledUnexecutedWork'], 'each area advanced from its real post-Fell roster')
check(all(t['death'] and t['death']['cause'] == 'fell' for a in v['areas'] for t in a['trees'] if t['treeId'] in (competitor, b_tree)), 'felled trees stay felled')
print('INFO year 0 -> 1 per area', [(a['areaId'], a['before']['livingCount'], a['after']['livingCount'], len(a['naturalDeaths'])) for a in record['areas']], flush=True)

# Consent: unexecuted work blocks Year+1 until the player chooses; consent cancels all of it at zero Fell.
b_next = living(v, 'MA-B')[0]['treeId']
v = ok('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': b_next, 'reason': 'consent case'}, 'pending in B')['view']
refused('api/multi/year/advance', {'fence': fence(v), 'consentToCancelWork': False}, 'WorkPendingConsentRequired', 'Year+1 with pending work, no consent')
c = ok('api/multi/year/advance', {'fence': fence(v), 'consentToCancelWork': True}, 'Year+1 with consent')['view']
check(c['pending'] == [] and len(c['events']) == 1 and c['annualHistory'][-1]['cancelledUnexecutedWork'] and
      any(t['treeId'] == b_next and t['isLiving'] for t in area(c, 'MA-B')['trees']), 'consent cancelled the whole Work Plan at zero Fell')

# Hit-tests in each area's own map metres.
for area_id in ('MA-A', 'MA-B'):
    scene = send('api/multi/view')[1]['scenes'][area_id]
    item = next(i for i in scene if i['status'] == 'Living')
    code, h = send('api/multi/hit', {'fence': fence(c), 'areaId': area_id, 'u': item['u'], 'v': item['v']})
    check(code == 200 and item['treeId'] in [x['treeId'] for x in h['candidates']] and all(x['treeId'].startswith(area_id + '-') for x in h['candidates']),
          area_id + ' hit-test at a crown centre returns that exact ID')
code, h = send('api/multi/hit', {'fence': fence(c), 'areaId': 'MA-B', 'u': 0.1, 'v': 0.1})
check(code == 200 and h['candidates'] == [], 'a hit-test on bare ground selects nothing')
code, h = send('api/multi/hit', {'fence': stale, 'areaId': 'MA-A', 'u': 5, 'v': 5})
check(code == 409 and h['candidates'] == [] and h['refusal']['code'] in ('PlayerSourceStale', 'PlayerPlanStateStale'), 'a stale hit-test is refused with no candidate')

# Read-only POST api/multi/cell (Pass 1): 400 guards, 409 stale/OOB/unknown-area refusals, 200 valid cells with zero state change.
v_cell_before = view()
f_cell = fence(v_cell_before)
refused('api/multi/cell', {'fence': f_cell, 'areaId': 'MA-A', 'column': 0}, 'HTTP400', 'cell inspect without row')
refused('api/multi/cell', {'fence': f_cell, 'areaId': 'MA-A', 'column': 0, 'row': 0, 'extra': 1}, 'HTTP400', 'cell inspect unknown member')
refused('api/multi/cell', None, 'HTTP400', 'cell inspect duplicate member',
        raw=json.dumps({'fence': f_cell, 'areaId': 'MA-A', 'column': 0, 'row': 0}).replace('"row":', '"row":1,"row":').encode())
refused('api/multi/cell', {'fence': stale, 'areaId': 'MA-A', 'column': 0, 'row': 0}, 'PlayerSourceStale', 'cell inspect stale fence')
refused('api/multi/cell', {'fence': f_cell, 'areaId': 'MA-A', 'column': 999, 'row': 999}, 'CellNotInArea', 'cell inspect out-of-bounds cell')
refused('api/multi/cell', {'fence': f_cell, 'areaId': 'MA-C', 'column': 0, 'row': 0}, 'MultiAreaUnknownArea', 'cell inspect unknown area')
s4_now = send('api/multi/view')[1]['stage4']
for area_id in ('MA-A', 'MA-B'):
    g = next(a for a in s4_now['areas'] if a['areaId'] == area_id)['ground']
    mid = g[len(g) // 2]
    code, r = send('api/multi/cell', {'fence': f_cell, 'areaId': area_id, 'column': mid['column'], 'row': mid['row']})
    cd = r.get('cell') if r else None
    check(code == 200 and r['refusal'] is None and cd is not None and cd['areaId'] == area_id and
          cd['column'] == mid['column'] and cd['row'] == mid['row'] and len(cd['polygon']) >= 3 and
          isinstance(cd['cohorts'], list) and r.get('ecology') is not None and view() == v_cell_before,
          area_id + ' POST api/multi/cell returns full cell detail + fresh composite view with zero state change')

# Owner safeguard over HTTP: a plan felling every living tree of B (A keeps its own) is refused whole; the plan stays.
v = c
codes = []
for t in living(c, 'MA-B'):
    code, r = send('api/multi/pending/add', {'fence': fence(v), 'areaId': 'MA-B', 'treeId': t['treeId'], 'reason': 'safeguard probe'})
    codes.append(code)
    v = r['view']
check(codes == [200] * 72 and len(v['pending']) == len(living(v, 'MA-B')) == 72, 'every living B tree is pending (72 adds, each HTTP 200)')
v = ok('api/multi/plan/approve', {'fence': fence(v)}, 'Approve a plan that would empty B')['view']
refused('api/multi/plan/execute', {'fence': fence(v), 'planId': v['approved']['planId'], 'executionKey': v['approved']['executionKey']},
        'WorkWouldLeaveNoLivingStems', 'Execute a plan that would leave B with no living tree')
check(view()['approved'] is not None and area(view(), 'MA-B')['stand']['livingCount'] == 72, 'nothing felled; the plan is still approved')
ok('api/multi/plan/cancel', {'fence': fence(view())}, 'Cancel the refused plan')

# ---- Pass 2 fenced biological Work Plan HTTP endpoints (6 verbs): guards, 14 refusals, commitment gate, raw payload budgets ----
def raw_view_and_eco_bytes():
    req = urllib.request.Request(base + 'api/multi/view', headers={'Origin': origin})
    raw = urllib.request.urlopen(req, timeout=15).read()
    text = raw.decode('utf-8')
    idx = text.index('"ecology":') + len('"ecology":')
    _, end = json.JSONDecoder().raw_decode(text, idx)
    return len(raw), len(text[idx:end].encode('utf-8')), json.loads(text)

def strictly_inside(poly, x, z):
    n = len(poly)
    for i in range(n):
        a = poly[i]; b = poly[(i + 1) % n]
        if (b['xm'] - a['xm']) * (z - a['zm']) - (b['zm'] - a['zm']) * (x - a['xm']) <= 0:
            return False
    return True

full_p2 = send('api/multi/view')[1]
v_p2 = full_p2['view']
f_p2 = fence(v_p2)
s4_p2 = full_p2['stage4']
area_b = area(v_p2, 'MA-B')
area_a = area(v_p2, 'MA-A')
cells_b = next(a for a in s4_p2['areas'] if a['areaId'] == 'MA-B')['ground']
cells_a = next(a for a in s4_p2['areas'] if a['areaId'] == 'MA-A')['ground']
clear_cell = cells_b[0]
other_cell_a = cells_a[0]
adults_all = [t for a in v_p2['areas'] for t in a['trees'] if t['isLiving']]
cell_m = full_p2['ecology']['gridCellM']
all_cell_keys = {(g['column'], g['row']) for g in cells_a + cells_b}

# Find a valid open planting point (>= 1.0 m from all living adults, inside an in-polygon cell) and a boundary sliver
import math
open_pt = None
sliver_pt = None
for a_obj in (area_b, area_a):
    xs = [p['xm'] for p in a_obj['polygon']]; zs = [p['zm'] for p in a_obj['polygon']]
    z = float(math.floor(min(zs))) + 0.125
    while z < max(zs) and (open_pt is None or sliver_pt is None):
        x = float(math.floor(min(xs))) + 0.125
        while x < max(xs) and (open_pt is None or sliver_pt is None):
            if strictly_inside(a_obj['polygon'], x, z) and all((t['xm'] - x) ** 2 + (t['zm'] - z) ** 2 >= 1.0 for t in adults_all):
                has_cell = (math.floor(x / cell_m), math.floor(z / cell_m)) in all_cell_keys
                if has_cell and a_obj['areaId'] == 'MA-B' and open_pt is None:
                    open_pt = (x, z)
                elif not has_cell and sliver_pt is None:
                    sliver_pt = (a_obj['areaId'], x, z)
            x += 0.25
        z += 0.25
check(open_pt is not None and sliver_pt is not None, 'Pass 2 probe found valid open MA-B point and boundary-sliver point')
px, pz = open_pt
sx_area, sx, sz = sliver_pt

# 400 / 403 / 405 / 413 / 415 guards across the 6 Pass 2 routes
refused('api/multi/clear/add', {'fence': f_p2, 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']},
        'HTTP403', 'clear/add foreign Origin', headers={'Origin': 'http://example.invalid'})
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': px, 'zM': pz},
        'HTTP405', 'GET on plant/add', method='GET')
refused('api/multi/shelter/add', {'fence': f_p2, 'areaId': 'MA-B', 'xM': px, 'zM': pz},
        'HTTP415', 'text/plain on shelter/add', headers={'Content-Type': 'text/plain'})
refused('api/multi/spot-clear/add', None, 'HTTP413', 'body over 16 KiB on spot-clear/add', raw=b' ' * 17000)
refused('api/multi/plant/add', None, 'HTTP400', 'malformed JSON on plant/add', raw=b'{"fence":')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xm': px, 'zm': pz},
        'HTTP400', 'wrong-case xm/zm on plant/add')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': px, 'zM': pz, 'extra': 1},
        'HTTP400', 'unknown member on plant/add')
refused('api/multi/plant/add', None, 'HTTP400', 'duplicate member on plant/add',
        raw=json.dumps({'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': px, 'zM': pz}).replace('"xM":', '"xM":1,"xM":').encode())
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': '10', 'zM': pz},
        'HTTP400', 'string coordinate on plant/add')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile oak', 'xM': px, 'zM': pz},
        'HTTP400', 'whitespace speciesId on plant/add')
refused('api/multi/clear/add', {'fence': f_p2, 'areaId': 'MA-B', 'column': 1.5, 'row': 1},
        'HTTP400', 'fractional column on clear/add')
refused('api/multi/planting/remove', {'fence': f_p2, 'orderId': 'P2W 0001'},
        'HTTP400', 'whitespace orderId on planting/remove')
refused('api/multi/planting/remove', {'fence': f_p2}, 'HTTP400', 'missing orderId on planting/remove')

# 409 Core refusals with fresh view + stage4 + ecology and zero state mutation
r_inf = refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': 1e40, 'zM': pz},
                'Phase2CoordinatesInvalid', 'float32 overflow (1e40) on plant/add')
check(r_inf.get('ecology') is not None and r_inf.get('stage4') is not None, '409 Phase2CoordinatesInvalid includes fresh stage4 + ecology')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': -50.0, 'zM': -50.0},
        'Phase2PointOutsideArea', 'point outside area on plant/add')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': sx_area, 'speciesId': 'sessile-oak', 'xM': sx, 'zM': sz},
        'Phase2PointWithoutGroundCell', 'boundary sliver without ground cell on plant/add')
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sitka-spruce', 'xM': px, 'zM': pz},
        'Phase2SpeciesUnknown', 'non-plantable species on plant/add')
live_b = living(v_p2, 'MA-B')[0]
refused('api/multi/plant/add', {'fence': f_p2, 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': live_b['xm'], 'zM': live_b['zm']},
        'PlantTooCloseToAdult', 'point at living adult on plant/add')
refused('api/multi/shelter/add', {'fence': f_p2, 'areaId': 'MA-B', 'xM': px, 'zM': pz},
        'ShelterNoTarget', 'shelter with no PJ or pending Plant')
refused('api/multi/clear/add', {'fence': f_p2, 'areaId': 'MA-B', 'column': 999, 'row': 999},
        'ClearCellNotInArea', 'out-of-bounds cell on clear/add')
refused('api/multi/clear/add', {'fence': f_p2, 'areaId': 'MA-B', 'column': other_cell_a['column'], 'row': other_cell_a['row']},
        'MultiAreaWrongArea', 'MA-A cell under MA-B on clear/add')
refused('api/multi/clear/remove', {'fence': f_p2, 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']},
        'UnknownWorkOrder', 'remove non-pending clear')
refused('api/multi/planting/remove', {'fence': f_p2, 'orderId': 'P2W9999'},
        'UnknownWorkOrder', 'remove unknown planting orderId')

# Queue ClearUndergrowth + Plant + Shelter + SpotClear; test duplicate, spacing, stale, remove, commitment gate, raw payload budgets
eco_before = full_p2['ecology']
stale_p2 = f_p2
p_cl = ok('api/multi/clear/add', {'fence': f_p2, 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']}, 'Queue ClearUndergrowth')
refused('api/multi/clear/add', {'fence': fence(p_cl['view']), 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']},
        'ClearDuplicateCell', 'duplicate ClearUndergrowth cell')
refused('api/multi/clear/add', {'fence': stale_p2, 'areaId': 'MA-B', 'column': cells_b[1]['column'], 'row': cells_b[1]['row']},
        'PlayerPlanStateStale', 'stale fence on clear/add')
p_cl_rm = ok('api/multi/clear/remove', {'fence': fence(p_cl['view']), 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']}, 'Remove pending ClearUndergrowth')
check(p_cl_rm['ecology']['pendingClear'] == [], 'pending ClearUndergrowth removed cleanly')
p_cl = ok('api/multi/clear/add', {'fence': fence(p_cl_rm['view']), 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']}, 'Re-queue ClearUndergrowth')

p_pl = ok('api/multi/plant/add', {'fence': fence(p_cl['view']), 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': px, 'zM': pz}, 'Queue Plant (sessile-oak)')
refused('api/multi/plant/add', {'fence': fence(p_pl['view']), 'areaId': 'MA-B', 'speciesId': 'sessile-oak', 'xM': px, 'zM': pz},
        'Phase2DuplicateOrder', 'duplicate exact Plant order')
refused('api/multi/plant/add', {'fence': fence(p_pl['view']), 'areaId': 'MA-B', 'speciesId': 'beech', 'xM': px + 0.2, 'zM': pz},
        'PlantSpacingTooClose', 'Plant spacing < 0.5 m from queued Plant')
p_sh = ok('api/multi/shelter/add', {'fence': fence(p_pl['view']), 'areaId': 'MA-B', 'xM': px, 'zM': pz}, 'Queue Shelter at Plant point')
refused('api/multi/shelter/add', {'fence': fence(p_sh['view']), 'areaId': 'MA-B', 'xM': px, 'zM': pz},
        'Phase2DuplicateOrder', 'duplicate Shelter order')
p_sp = ok('api/multi/spot-clear/add', {'fence': fence(p_sh['view']), 'areaId': 'MA-B', 'xM': px, 'zM': pz}, 'Queue SpotClear at Plant point')
refused('api/multi/spot-clear/add', {'fence': fence(p_sp['view']), 'areaId': 'MA-B', 'xM': px, 'zM': pz},
        'Phase2DuplicateOrder', 'duplicate SpotClear order')
spot_oid = next(o['orderId'] for o in p_sp['ecology']['pendingPlanting'] if o['kind'] == 'SpotClear')
p_sp_rm = ok('api/multi/planting/remove', {'fence': fence(p_sp['view']), 'orderId': spot_oid}, 'Remove pending SpotClear via planting/remove')
check(len(p_sp_rm['ecology']['pendingPlanting']) == 2, 'planting/remove removed SpotClear only')
p_sp = ok('api/multi/spot-clear/add', {'fence': fence(p_sp_rm['view']), 'areaId': 'MA-B', 'xM': px, 'zM': pz}, 'Re-queue SpotClear')

# Commitment gate while Pending: ZERO ecology mutation
check(p_sp['view']['source'] == v_p2['source'] and p_sp['ecology']['planted'] == [] and p_sp['ecology']['shelters'] == [] and
      p_sp['ecology']['patches'] == [] and len(p_sp['ecology']['pendingClear']) == 1 and len(p_sp['ecology']['pendingPlanting']) == 3,
      'Pass 2 pending orders mutate zero ecology (planted/shelters/patches empty, year and workRevision unchanged)')
pend_view_b, pend_eco_b, _ = raw_view_and_eco_bytes()
check(pend_view_b <= 524288 and pend_eco_b <= 153600,
      'Pass 2 pending raw payload budgets hold (view=%d B <= 524288, ecology=%d B <= 153600)' % (pend_view_b, pend_eco_b))

# Approve: ZERO ecology mutation + all 6 verbs refused with WorkPlanAlreadyApproved
p_app = ok('api/multi/plan/approve', {'fence': fence(p_sp['view'])}, 'Approve Pass 2 mixed plan')
v_app = p_app['view']; f_app = fence(v_app)
check(v_app['source'] == v_p2['source'] and p_app['ecology']['planted'] == [] and p_app['ecology']['shelters'] == [] and
      p_app['ecology']['patches'] == [] and len(p_app['ecology']['approvedClear']) == 1 and len(p_app['ecology']['approvedPlanting']) == 3,
      'Approve Pass 2 mixed plan mutates zero ecology')
refused('api/multi/clear/add', {'fence': f_app, 'areaId': 'MA-B', 'column': cells_b[1]['column'], 'row': cells_b[1]['row']}, 'WorkPlanAlreadyApproved', 'clear/add while approved')
refused('api/multi/clear/remove', {'fence': f_app, 'areaId': 'MA-B', 'column': clear_cell['column'], 'row': clear_cell['row']}, 'WorkPlanAlreadyApproved', 'clear/remove while approved')
refused('api/multi/plant/add', {'fence': f_app, 'areaId': 'MA-B', 'speciesId': 'beech', 'xM': px, 'zM': pz}, 'WorkPlanAlreadyApproved', 'plant/add while approved')
refused('api/multi/shelter/add', {'fence': f_app, 'areaId': 'MA-B', 'xM': px, 'zM': pz}, 'WorkPlanAlreadyApproved', 'shelter/add while approved')
refused('api/multi/spot-clear/add', {'fence': f_app, 'areaId': 'MA-B', 'xM': px, 'zM': pz}, 'WorkPlanAlreadyApproved', 'spot-clear/add while approved')
refused('api/multi/planting/remove', {'fence': f_app, 'orderId': p_app['ecology']['approvedPlanting'][0]['orderId']}, 'WorkPlanAlreadyApproved', 'planting/remove while approved')

# Execute: atomic commit with zero calendar advance -> PJ sapling, active 8-yr shelter, 0.564 m patch, cleared cell
p_ex = ok('api/multi/plan/execute', {'fence': f_app, 'planId': v_app['approved']['planId'], 'executionKey': v_app['approved']['executionKey']},
          'Execute Pass 2 mixed plan')
ex_eco = p_ex['ecology']; ex_v = p_ex['view']
check(ex_v['source']['ecologicalYear'] == v_p2['source']['ecologicalYear'] and
      ex_v['source']['workRevision'] == v_p2['source']['workRevision'] + 1 and
      len(ex_eco['planted']) == 1 and ex_eco['planted'][0]['speciesId'] == 'sessile-oak' and ex_eco['planted'][0]['isActive'] and
      len(ex_eco['shelters']) == 1 and ex_eco['shelters'][0]['isEffective'] and
      ex_eco['shelters'][0]['juvenileId'] == ex_eco['planted'][0]['juvenileId'] and
      ex_eco['shelters'][0]['expiryYear'] == ex_eco['shelters'][0]['installedYear'] + 8 and
      len(ex_eco['patches']) == 1 and abs(ex_eco['patches'][0]['radiusM'] - 0.5641896) < 0.001,
      'Execute atomically committed PJ sapling, active 8-year shelter, and 0.564 m SpotClear patch at zero year advance')
refused('api/multi/shelter/add', {'fence': fence(ex_v), 'areaId': 'MA-B', 'xM': ex_eco['planted'][0]['xm'], 'zM': ex_eco['planted'][0]['zm']},
        'ShelterAlreadyPresent', 'shelter/add on already-sheltered PJ sapling')
exec_view_b, exec_eco_b, _ = raw_view_and_eco_bytes()
check(exec_view_b <= 524288 and exec_eco_b <= 153600,
      'Pass 2 post-Execute raw payload budgets hold (view=%d B <= 524288, ecology=%d B <= 153600)' % (exec_view_b, exec_eco_b))
# ---- SL: Save/Load named slots (Phase 1) ------------------------------------------------------------------------------
import os

def save_slots():
    code, payload = send('api/multi/saves', method='GET')
    return code, payload

sl_view = view()
sl_year = sl_view['source']['ecologicalYear']
sl_code, sl_list = save_slots()
check(sl_code == 200 and sl_list.get('slots') == [], 'GET api/multi/saves answers an empty slot list on a fresh process')
sl_fence = fence(sl_view)

code, _ = send('api/multi/saves', body=sl_fence)
check(code == 405, 'POST on the GET-only saves list is 405')
code, _ = send('api/multi/load', body={'fence': sl_fence, 'slot': 'x'}, headers={'Content-Type': 'text/plain'})
check(code == 415, 'load with text/plain is 415')
code, _ = send('api/multi/save', body={'fence': sl_fence, 'slot': 'x'})
check(code == 400, 'save without the explicit replaceExisting member is 400')

code, payload = send('api/multi/save', body={'fence': sl_fence, 'slot': '../escape', 'replaceExisting': False})
check(code == 409 and payload['refusal']['code'] == 'SlotNameInvalid' and view() == sl_view,
      'a traversal slot name is refused and nothing changed')
code, payload = send('api/multi/save', body={'fence': sl_fence, 'slot': 'Upper', 'replaceExisting': False})
check(code == 409 and payload['refusal']['code'] == 'SlotNameInvalid', 'a non-lowercase slot name is refused')

code, saved = send('api/multi/save', body={'fence': sl_fence, 'slot': 'e2e', 'replaceExisting': False})
check(code == 200 and saved['saved']['slot'] == 'e2e' and saved['saved']['status'] == 'durablyCommitted' and
      saved['saved']['ecologicalYear'] == sl_year and saved['saved']['saveGeneration'] == 1 and len(saved['saved']['checksum']) == 64,
      'Save wrote slot e2e durably at year %d (HTTP %d)' % (sl_year, code))
row = next((s for s in save_slots()[1]['slots'] if s['slot'] == 'e2e'), None)
check(row is not None and row['status'] == 'valid' and row['canLoad'] and row['ecologicalYear'] == sl_year and
      row['label'] == 'e2e' and row['models'] == '1/1/1/1/1' and row['savedUtc'].endswith('Z'),
      'the slot is listed as valid with its label, year, saved UTC and model vector, and no filesystem path')
check(row is not None and not any(str(row.get(k) or '').startswith('/') for k in row),
      'the slot listing carries no absolute filesystem path')

code, payload = send('api/multi/save', body={'fence': sl_fence, 'slot': 'e2e', 'replaceExisting': False})
check(code == 409 and payload['refusal']['code'] == 'SlotExists' and view() == sl_view,
      'an existing slot is never overwritten without explicit confirmation')
code, replaced = send('api/multi/save', body={'fence': sl_fence, 'slot': 'e2e', 'replaceExisting': True})
check(code == 200 and replaced['saved']['backupUpdated'] and replaced['saved']['saveGeneration'] == 2,
      'a confirmed replace bumps the save generation and keeps the earlier save as the backup')
row = next(s for s in save_slots()[1]['slots'] if s['slot'] == 'e2e')
check(row['backupExists'] and row['backupLoadable'], 'the list reports a loadable prior-good backup')
check(not row['recoverable'], 'a slot whose primary still verifies is NOT offered backup recovery')
code, payload = send('api/multi/saves/recover', body={'slot': 'e2e'})
check(code == 409 and payload['refusal']['code'] == 'SlotPrimaryValid',
      'recovery over a verified primary is refused, so one click cannot roll a good newer save back')
row = next(s for s in save_slots()[1]['slots'] if s['slot'] == 'e2e')
check(row['status'] == 'valid' and row['ecologicalYear'] == sl_year, 'the newer save is still the loaded primary')

adv = ok('api/multi/year/advance', {'fence': sl_fence, 'consentToCancelWork': False}, 'Year+1 after saving')
after = adv['view']
check(after['source']['ecologicalYear'] == sl_year + 1, 'the live session moved on one year after the save')

code, payload = send('api/multi/load', body={'fence': sl_fence, 'slot': 'e2e'})
check(code == 409 and payload['refusal']['code'] == 'PlayerSourceStale' and view() == after,
      'a load with the pre-advance fence is refused and the live session is unchanged')
code, payload = send('api/multi/load', body={'fence': fence(after), 'slot': 'no-such-slot'})
check(code == 409 and payload['refusal']['code'] == 'SlotMissing' and view() == after,
      'loading a missing slot is refused and the live session is unchanged')

code, loaded = send('api/multi/load', body={'fence': fence(after), 'slot': 'e2e'})
check(code == 200 and loaded['refusal'] is None and loaded['view']['source']['ecologicalYear'] == sl_year and
      loaded['view']['sessionId'] != after['sessionId'] and loaded['restored']['draftOrderCount'] == 0,
      'Load restored the saved year from the slot under a NEW session id (HTTP %d)' % code)
new_view = loaded['view']
check([a['trees'] for a in new_view['areas']] == [a['trees'] for a in sl_view['areas']],
      'every saved tree record came back exactly (rosters, coordinates, crowns and deaths)')
check(new_view['source'] == sl_view['source'] and new_view['planStateRevision'] == sl_view['planStateRevision'],
      'the source token and plan-state revision are restored exactly')

code, payload = send('api/multi/year/advance', body={'fence': fence(after), 'consentToCancelWork': False})
check(code == 409 and payload['refusal']['code'] == 'PlayerSessionMismatch',
      'the replaced session fence cannot mutate the restored forest')
adv2 = ok('api/multi/year/advance', {'fence': fence(new_view), 'consentToCancelWork': False}, 'the restored session advances a real year')
check(adv2['view']['source']['ecologicalYear'] == sl_year + 1 and adv2['view']['sessionId'] == new_view['sessionId'],
      'the restored forest keeps its new authority and continues one real year')

# Corrupt the primary on disk (the smoke owns this scratch folder) and prove the honest backup path.
slot_dir = sys.argv[2]
primary = os.path.join(slot_dir, 'e2e.ccf2d.json')
before_backup = open(primary + '.bak', 'rb').read()
open(primary, 'wb').write(b'\x00\x01 broken primary')
row = next(s for s in save_slots()[1]['slots'] if s['slot'] == 'e2e')
check(row['status'] == 'corrupt' and row['canLoad'] is False and row['backupExists'] and row['backupLoadable'] and
      row['recoverable'], 'a corrupt primary is reported as corrupt, with its verified backup offered for recovery')
code, payload = send('api/multi/load', body={'fence': fence(adv2['view']), 'slot': 'e2e'})
check(code == 409 and payload['refusal']['code'] in
      ('JsonInvalid', 'SaveUtf8Invalid', 'SaveNotCanonical', 'SaveChecksumMismatch', 'SaveInconsistent', 'SaveTooLarge',
       'SaveSchemaUnsupported') and view() == adv2['view'],
      'a corrupt slot refuses to load as %s and the live session is unchanged' % payload['refusal']['code'])
code, recovery = send('api/multi/saves/recover', body={'slot': 'e2e'})
check(code == 200 and recovery['refusal'] is None and recovery['slot'] == 'e2e',
      'Restore backup put the verified prior-good save back over the corrupt primary')
check(open(primary + '.bak', 'rb').read() == before_backup, 'the backup itself is kept, not consumed')
code, loaded2 = send('api/multi/load', body={'fence': fence(adv2['view']), 'slot': 'e2e'})
check(code == 200 and loaded2['view']['source']['ecologicalYear'] == sl_year,
      'the recovered slot loads the saved year again (HTTP %d)' % code)

# A plan that was approved when the save was written comes back as an UNAPPROVED pending draft (owner load policy A).
cur = view()
free = next(t['treeId'] for t in cur['areas'][0]['trees']
            if t['isLiving'] and not any(o['treeId'] == t['treeId'] for o in cur['pending']))
queued = ok('api/multi/pending/add', {'fence': fence(cur), 'areaId': 'MA-A', 'treeId': free, 'reason': 'approved before save'},
            'queue a plan to approve before saving')
approved = ok('api/multi/plan/approve', {'fence': fence(queued['view'])}, 'approve it before saving')
check(approved['view']['approved'] is not None and approved['view']['pending'] == [], 'the saved plan really is approved')
code, _ = send('api/multi/save', body={'fence': fence(approved['view']), 'slot': 'approved-draft', 'replaceExisting': False})
check(code == 200, 'Save accepted a session holding an approved plan (HTTP %d)' % code)
code, reloaded = send('api/multi/load', body={'fence': fence(approved['view']), 'slot': 'approved-draft'})
check(code == 200 and reloaded['view']['approved'] is None and len(reloaded['view']['pending']) == 1 and
      reloaded['restored']['draftOrderCount'] == 1,
      'the formerly approved plan returns as an unapproved pending draft that needs a fresh Approve')
code, reapproved = send('api/multi/plan/approve', body={'fence': fence(reloaded['view'])})
check(code == 200 and reapproved['view']['approved'] is not None and
      reapproved['view']['approved']['planId'] != approved['view']['approved']['planId'],
      'approving the restored draft mints a plan id for the NEW session')

# ---- S2: Phase 1 Step 2 HTTP contracts (preview-review, skip, history/index, history/year, structure, bounded view) ----
s2_app_view = reapproved['view']
s2_app_fence = fence(s2_app_view)
s2_stale_fence = fence(approved['view'])

# Guards across all five Step 2 routes (403, 404, 405, 415, 413, 400)
refused('api/multi/year/preview-review', {'fence': s2_app_fence}, 'HTTP403', 'Step 2 preview-review foreign Origin', headers={'Origin': 'http://example.invalid'})
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': 4}, 'HTTP403', 'Step 2 skip foreign Origin', headers={'Origin': 'http://example.invalid'})
refused('api/multi/history/year', {'fence': s2_app_fence, 'year': 0}, 'HTTP403', 'Step 2 history/year foreign Origin', headers={'Origin': 'http://example.invalid'})
refused('api/multi/structure', None, 'HTTP403', 'Step 2 structure wrong Host', method='GET', headers={'Host': 'localhost:' + origin.rsplit(':', 1)[1]})
refused('api/multi/history/index', None, 'HTTP404', 'Step 2 history/index wrong capability', method='GET', target=origin + '/s/' + '0' * 64 + '/')
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': 4}, 'HTTP404', 'Step 2 skip wrong capability', target=origin + '/s/' + '0' * 64 + '/')
refused('api/multi/year/preview-review', None, 'HTTP405', 'GET on preview-review', method='GET')
refused('api/multi/year/skip', None, 'HTTP405', 'GET on skip', method='GET')
refused('api/multi/history/year', None, 'HTTP405', 'GET on history/year', method='GET')
refused('api/multi/history/index', {'fence': s2_app_fence}, 'HTTP405', 'POST on history/index', method='POST')
refused('api/multi/structure', {'fence': s2_app_fence}, 'HTTP405', 'POST on structure', method='POST')
refused('api/multi/year/preview-review', {'fence': s2_app_fence}, 'HTTP415', 'text/plain on preview-review', headers={'Content-Type': 'text/plain'})
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': 4}, 'HTTP415', 'text/plain on skip', headers={'Content-Type': 'text/plain'})
refused('api/multi/history/year', {'fence': s2_app_fence, 'year': 0}, 'HTTP415', 'text/plain on history/year', headers={'Content-Type': 'text/plain'})
refused('api/multi/year/skip', None, 'HTTP413', 'body over 16 KiB on skip', raw=b' ' * 17000)
refused('api/multi/year/preview-review', None, 'HTTP400', 'malformed JSON on preview-review', raw=b'{"fence":')
refused('api/multi/year/preview-review', {'fence': s2_app_fence, 'extra': 1}, 'HTTP400', 'unknown member on preview-review')
refused('api/multi/year/skip', {'fence': s2_app_fence}, 'HTTP400', 'skip without targetYear')
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': 4, 'extra': 1}, 'HTTP400', 'unknown member on skip')
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': '4'}, 'HTTP400', 'string targetYear on skip')
refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': 4.5}, 'HTTP400', 'fractional targetYear on skip')
refused('api/multi/year/skip', None, 'HTTP400', 'duplicate member on skip',
        raw=json.dumps({'fence': s2_app_fence, 'targetYear': 4}).replace('"targetYear":', '"targetYear":3,"targetYear":').encode())
refused('api/multi/history/year', {'fence': s2_app_fence}, 'HTTP400', 'history/year without year')
refused('api/multi/history/year', {'fence': s2_app_fence, 'year': '0'}, 'HTTP400', 'string year on history/year')
refused('api/multi/history/year', {'fence': s2_app_fence, 'year': 0, 'extra': 1}, 'HTTP400', 'unknown member on history/year')
refused('api/multi/history/index?unknown=1', None, 'HTTP400', 'unknown query param on history/index', method='GET')
refused('api/multi/history/index?pageSize=abc', None, 'HTTP400', 'non-integer pageSize on history/index', method='GET')
refused('api/multi/history/index?pageSize=2&pageSize=3', None, 'HTTP400', 'duplicate pageSize on history/index', method='GET')
refused('api/multi/history/index?beforeYear=-1', None, 'HTTP400', 'negative beforeYear on history/index', method='GET')
refused('api/multi/structure?extra=1', None, 'HTTP400', 'unexpected query param on structure', method='GET')

# WorkPlanOpen (Approved): preview-review reports ineligible + skip returns 409 without cancelling approved work
code, prev_app = send('api/multi/year/preview-review', {'fence': s2_app_fence})
check(code == 200 and prev_app['refusal'] is None and prev_app['eligible'] is False and
      prev_app['stopReason'] == 'WorkPlanOpen' and prev_app['anchorFellYear'] == 0 and
      prev_app['isObservationPeriod'] is False and 'GAMEPLAY CALIBRATION' in prev_app['calibrationNote'] and
      prev_app['maxTicksPerPress'] == 5 and prev_app['fence'] == s2_app_fence,
      'preview-review with Approved plan returns 200 ineligible WorkPlanOpen with anchorFellYear=0 and GAMEPLAY CALIBRATION')
r_skip_app = refused('api/multi/year/skip', {'fence': s2_app_fence, 'targetYear': prev_app['suggestedTargetYear']},
                     'WorkPlanOpen', 'skip refused while Approved plan is open')
check(r_skip_app.get('view') is not None and r_skip_app['view']['approved'] is not None and r_skip_app.get('ecology') is not None,
      'skip 409 WorkPlanOpen preserves open Approved plan and returns fresh composite payload')

# WorkPlanOpen (Pending): cancel approved, queue 1 Pending Fell, verify skip never cancels Pending work
s2_v = ok('api/multi/plan/cancel', {'fence': s2_app_fence}, 'cancel Approved plan for Step 2 Pending check')['view']
s2_free = next(t['treeId'] for t in living(s2_v, 'MA-A'))
s2_v = ok('api/multi/pending/add', {'fence': fence(s2_v), 'areaId': 'MA-A', 'treeId': s2_free, 'reason': 'step2 pending open'}, 'queue Pending Fell for Step 2')['view']
code, prev_pend = send('api/multi/year/preview-review', {'fence': fence(s2_v)})
check(code == 200 and prev_pend['eligible'] is False and prev_pend['stopReason'] == 'WorkPlanOpen',
      'preview-review with Pending Fell reports ineligible WorkPlanOpen')
r_skip_pend = refused('api/multi/year/skip', {'fence': fence(s2_v), 'targetYear': prev_pend['suggestedTargetYear']},
                      'WorkPlanOpen', 'skip refused while Pending Fell is open')
check(len(view()['pending']) == 1 and view()['pending'][0]['treeId'] == s2_free,
      'skip 409 WorkPlanOpen leaves Pending Fell intact (never cancels open work)')
s2_v = ok('api/multi/pending/remove', {'fence': fence(s2_v), 'areaId': 'MA-A', 'treeId': s2_free}, 'remove Pending Fell')['view']

# Stale fence on preview-review and skip; invalid targetYear bounds on skip
r_prev_stale = refused('api/multi/year/preview-review', {'fence': s2_stale_fence}, 'PlayerSessionMismatch', 'preview-review stale fence')
check(r_prev_stale.get('fence') == fence(s2_v) and r_prev_stale.get('view') is not None, 'preview-review 409 returns fresh fence and view')
refused('api/multi/year/skip', {'fence': s2_stale_fence, 'targetYear': 4}, 'PlayerSessionMismatch', 'skip stale fence')
refused('api/multi/year/skip', {'fence': fence(s2_v), 'targetYear': s2_v['source']['ecologicalYear']}, 'InvalidTargetYear', 'skip targetYear == currentYear')
refused('api/multi/year/skip', {'fence': fence(s2_v), 'targetYear': s2_v['source']['ecologicalYear'] + 6}, 'InvalidTargetYear', 'skip targetYear > currentYear + 5')

# Valid skip to suggested review year (Year 2 -> Year 4, anchored to Fell Year 0) + overdue fallback at Year 4 -> Year 8
code, prev_ok = send('api/multi/year/preview-review', {'fence': fence(s2_v)})
check(code == 200 and prev_ok['eligible'] is True and prev_ok['suggestedTargetYear'] == 4 and
      prev_ok['anchorFellYear'] == 0 and prev_ok['isObservationPeriod'] is False and view() == s2_v,
      'preview-review at Year 2 (anchor Fell Year 0) suggests Year 4 read-only')
skip1 = ok('api/multi/year/skip', {'fence': fence(s2_v), 'targetYear': 4}, 'skip Year 2 -> Year 4')
check(skip1['startYear'] == 2 and skip1['destinationYear'] == 4 and skip1['requestedTargetYear'] == 4 and
      len(skip1['completedYears']) == 2 and skip1['reachedTarget'] is True and skip1['stopReason'] == 'ReachedReviewYear' and
      skip1['anchorFellYear'] == 0 and skip1['isObservationPeriod'] is False and
      'GAMEPLAY CALIBRATION' in skip1['calibrationNote'] and skip1['view']['source']['ecologicalYear'] == 4 and
      skip1.get('scenes') is not None and skip1.get('stage4') is not None and skip1.get('ecology') is not None,
      'skip Year 2 -> 4 returns full DTO + fresh composite payload')
code, prev_over = send('api/multi/year/preview-review', {'fence': fence(skip1['view'])})
check(code == 200 and prev_over['eligible'] is True and prev_over['suggestedTargetYear'] == 8 and prev_over['anchorFellYear'] == 0,
      'preview-review overdue fallback at Year 4 (anchor 0) suggests Year 8')

# Advance to Year 15 via bounded skips (+5, +5, +1) for 15+ year history paging & bounded view payload verification
cur_s2 = skip1['view']
for target_yr in (9, 14, 15):
    res = ok('api/multi/year/skip', {'fence': fence(cur_s2), 'targetYear': target_yr}, 'skip to Year %d' % target_yr)
    cur_s2 = res['view']
check(cur_s2['source']['ecologicalYear'] == 15, 'reached Year 15 via bounded skip')

# GET api/multi/history/index: clamping (0 -> 1, 99 -> 50, default 10) and newest-first cursor paging over 16 recorded years
code, idx_def = send('api/multi/history/index', method='GET')
check(code == 200 and idx_def['pageSize'] == 10 and len(idx_def['items']) == 10 and
      idx_def['totalRecordedYears'] == 16 and idx_def['items'][0]['year'] == 15 and idx_def['nextCursorBeforeYear'] == 6,
      'history/index default pageSize=10 returns Years 15..6 with cursor=6 and totalRecordedYears=16')
code, idx_lo = send('api/multi/history/index?pageSize=0', method='GET')
code_hi, idx_hi = send('api/multi/history/index?pageSize=99', method='GET')
check(code == 200 and idx_lo['pageSize'] == 1 and len(idx_lo['items']) == 1 and
      code_hi == 200 and idx_hi['pageSize'] == 50 and len(idx_hi['items']) == 16,
      'history/index clamps pageSize to [1, 50]')
paged_years = []
cursor = None
for _ in range(10):
    q = 'api/multi/history/index?pageSize=5' + ('' if cursor is None else '&beforeYear=%d' % cursor)
    c_pg, pg = send(q, method='GET')
    check(c_pg == 200, 'history/index page fetch (%s)' % q)
    paged_years.extend(item['year'] for item in pg['items'])
    cursor = pg['nextCursorBeforeYear']
    if cursor is None:
        break
check(paged_years == list(range(15, -1, -1)), 'history/index cursor paging walks all 16 years newest-first without gaps or duplicates')

# POST api/multi/history/year: 409 stale/invalid/unrecorded + 200 full Year 0 & Year 2 Decision -> Outcome details
refused('api/multi/history/year', {'fence': s2_stale_fence, 'year': 0}, 'PlayerSessionMismatch', 'history/year stale fence')
refused('api/multi/history/year', {'fence': fence(cur_s2), 'year': -1}, 'HistoryYearInvalid', 'history/year negative year')
refused('api/multi/history/year', {'fence': fence(cur_s2), 'year': 99}, 'HistoryYearNotRecorded', 'history/year unrecorded future year')
code, det0 = send('api/multi/history/year', {'fence': fence(cur_s2), 'year': 0})
det0_text = json.dumps(det0)
gloss_labels = {g['label'] for g in det0.get('provenanceGlossary', [])}
disc_keys = {d['fieldKey'] for d in det0.get('notRecordedDisclosures', [])}
non_causal = (det0.get('laterObservedComparison') or {}).get('nonCausalStatement', '')
check(code == 200 and det0['workYear'] == 0 and det0['observedYear'] == 1 and det0['hasRecordedData'] is True and
      det0['totalWorkCounts']['fell'] == 2 and det0['annualOutcomeNextYear']['yearAfter'] == 1 and
      gloss_labels == {'RECORDED', 'CURRENT', 'DERIVED', 'RECONSTRUCTED', 'SESSION-ONLY', 'NOT RECORDED'} and
      len(disc_keys) >= 3 and 'not a single-cause effect' in non_causal and
      'weather' not in non_causal.lower() and '\u20ac' not in det0_text and 'revenue' not in det0_text.lower(),
      'history/year(0) separates WorkYear 0 vs ObservedYear 1, includes all 6 provenance labels, NOT RECORDED disclosures, exact non-causal note (no weather), zero economy')
code, det2 = send('api/multi/history/year', {'fence': fence(cur_s2), 'year': 2})
check(code == 200 and det2['workYear'] == 2 and det2['observedYear'] == 3 and det2['totalWorkCounts']['plant'] == 1 and
      len(det2['plantedWorkFollowUp']) == 1 and det2['plantedWorkFollowUp'][0]['provenance'] == 'Reconstructed' and
      len(det2['annualOutcomeNextYear']['wholeWoodlandPlantedAccounts']) >= 1 and
      'NOT RECORDED' in det2['annualOutcomeNextYear']['perAreaPlantedSplitNote'],
      'history/year(2) reports RECONSTRUCTED planted juvenile follow-up and whole-woodland planted accounts with NOT RECORDED per-area split')

# GET api/multi/structure on MA-1 (all Sitka -> meanMingling == 0.0)
code, st_ma1 = send('api/multi/structure', method='GET')
check(code == 200 and st_ma1['provenance'] == 'DERIVED' and st_ma1['provenanceLabel'] == 'DERIVED' and
      st_ma1['maxNeighbourCountK'] == 3 and st_ma1['meanMingling'] == 0.0 and
      st_ma1['meanDiameterDifferentiation'] is not None and st_ma1['meanDiameterDifferentiation'] > 0.0 and
      len(st_ma1['areas']) == 2 and
      st_ma1['honestyLabel'] == 'Spatial structure descriptor (up to k=3 nearest living trees) \u2014 not measured biodiversity, habitat quality, or an Irish forecast.',
      'GET api/multi/structure on MA-1 returns DERIVED M_i=0.00, T_i>0, k=3 and exact honesty disclaimer')

# Year 15 bounded view payload budget on MA-1
y15_view_b, y15_eco_b, y15_obj = raw_view_and_eco_bytes()
check(y15_view_b <= 524288 and y15_eco_b <= 153600 and y15_obj['totalRecordedYears'] == 16 and isinstance(y15_obj['historyTruncated'], bool),
      'MA-1 Year 15 raw payload budgets hold (view=%d B <= 524288, ecology=%d B <= 153600, totalRecordedYears=16)' % (y15_view_b, y15_eco_b))

print('INFO workflow checks: %d' % count, flush=True)
WORKFLOW
workflow_rc=0
python3 -I -B "$scratch/workflow.py" "$url" "$scratch/xdg/ccf-multi-2d-saves" > "$scratch/workflow.out" 2> "$scratch/workflow.err" || workflow_rc=$?
cat "$scratch/workflow.out" | tee -a "$log"
check "W999 the HTTP workflow witness completed without stderr" bash -c 'test "$1" == 0 && test ! -s "$2"' _ "$workflow_rc" "$scratch/workflow.err"
if [[ $workflow_rc != 0 ]]; then fail=1; cat "$scratch/workflow.err" | tee -a "$log"; fi
stop_server api 1

# ---- M12-M13: --multi-experiment=ma-1-mixed truthful badge/budgets and invalid CLI exit 64 --------------------------------
check "M12 invalid --multi-experiment flag exits non-zero (64) with usage message" bash -c '
  set +e
  "$1" "$2" "$3" --multi-experiment=invalid >/dev/null 2>"$4"
  rc=$?
  test "$rc" -eq 64 && grep -q "expected ma-1 or ma-1-mixed" "$4"' _ "$dotnet_bin" "$ui_dll" "$scratch/fixture" "$scratch/badcli.err"
set -m
( cd "$p1_root" && exec env ASPNETCORE_URLS="http://0.0.0.0:5187" DOTNET_URLS="http://0.0.0.0:5187" ASPNETCORE_HTTP_PORTS=5187 \
    Kestrel__Endpoints__Lan__Url="http://0.0.0.0:5187" ASPNETCORE_ENVIRONMENT=Development \
    "$dotnet_bin" "$ui_dll" "$scratch/fixture" --multi-experiment=ma-1-mixed ) > "$scratch/server-mixed.out" 2> "$scratch/server-mixed.err" &
server_pid=$!
set +m
mixed_url=""
for _ in $(seq 1 300); do
  mixed_url="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$scratch/server-mixed.out" | head -1 || true)"
  [[ -n "$mixed_url" ]] && break
  kill -0 "$server_pid" 2> /dev/null || break
  sleep 0.1
done
check "M13 --multi-experiment=ma-1-mixed serves truthful badge, 3 species in Area B and stays within payload budgets" \
  bash -c '
    test -n "$1" &&
    curl -fsS --noproxy "*" "${1}api/multi/manifest" -o "$2" &&
    curl -fsS --noproxy "*" "${1}api/multi/view" -o "$3" &&
    python3 -I -B -c "import json, sys
m = json.load(open(sys.argv[1], encoding=\"utf-8\"))
raw = open(sys.argv[2], \"rb\").read()
text = raw.decode(\"utf-8\")
v = json.loads(text)
idx = text.index(\"\\\"ecology\\\":\") + len(\"\\\"ecology\\\":\")
_, end = json.JSONDecoder().raw_decode(text, idx)
eco_bytes = len(text[idx:end].encode(\"utf-8\"))
b_sp = next(a for a in v[\"ecology\"][\"areas\"] if a[\"areaId\"] == \"MA-B\")[\"liveAdultsBySpecies\"]
ok = m.get(\"experimentKey\") == \"ma-1-mixed\"
ok = ok and m.get(\"experimentBadge\") == \"Synthetic MA-1-MIXED demo \u2014 Area A Sitka; Area B \u2153 Sitka / \u2153 Oak / \u2153 Beech (not a surveyed Irish forecast)\"
ok = ok and len(raw) <= 524288 and eco_bytes <= 153600
ok = ok and all(b_sp.get(sp, 0) > 0 for sp in (\"sitka-spruce\", \"sessile-oak\", \"beech\"))
sys.exit(0 if ok else 1)" "$2" "$3"' _ "$mixed_url" "$scratch/mixed-man.json" "$scratch/mixed-view.json"
check "M14 --multi-experiment=ma-1-mixed serves GET api/multi/structure (M_i > 0, DERIVED) and Year-0 preview-review (Obs., +4)" \
  bash -c '
    test -n "$1" &&
    curl -fsS --noproxy "*" "${1}api/multi/structure" -o "$2" &&
    python3 -I -B -c "import json, sys, urllib.request
base = sys.argv[1]; origin = base.split(\"/s/\")[0]
st = json.load(open(sys.argv[2], encoding=\"utf-8\"))
view_obj = json.load(open(sys.argv[3], encoding=\"utf-8\"))
v = view_obj[\"view\"]
fence = {\"sessionId\": v[\"sessionId\"], \"source\": v[\"source\"], \"planStateRevision\": v[\"planStateRevision\"]}
req = urllib.request.Request(base + \"api/multi/year/preview-review\", data=json.dumps({\"fence\": fence}).encode(),
    headers={\"Origin\": origin, \"Content-Type\": \"application/json\"}, method=\"POST\")
prev = json.loads(urllib.request.urlopen(req, timeout=15).read())
area_a = next(a for a in st[\"areas\"] if a[\"areaId\"] == \"MA-A\")
area_b = next(a for a in st[\"areas\"] if a[\"areaId\"] == \"MA-B\")
ok = st.get(\"provenance\") == \"DERIVED\" and st.get(\"maxNeighbourCountK\") == 3
ok = ok and st.get(\"meanMingling\", 0) > 0 and area_a.get(\"meanMingling\") == 0.0 and area_b.get(\"meanMingling\", 0) > 0
ok = ok and st.get(\"honestyLabel\") == \"Spatial structure descriptor (up to k=3 nearest living trees) \u2014 not measured biodiversity, habitat quality, or an Irish forecast.\"
ok = ok and prev.get(\"eligible\") is True and prev.get(\"suggestedTargetYear\") == 4 and prev.get(\"isObservationPeriod\") is True
ok = ok and prev.get(\"anchorFellYear\") is None and \"GAMEPLAY CALIBRATION\" in (prev.get(\"calibrationNote\") or \"\")
sys.exit(0 if ok else 1)" "$1" "$2" "$3"' _ "$mixed_url" "$scratch/mixed-structure.json" "$scratch/mixed-view.json"
stop_server mixed

# ---- J: the page's own script against a second, fresh process (a fresh session at Year 0) --------------------------------
start_server page
cat > "$scratch/page-witness.mjs" << 'WITNESS'
import vm from 'node:vm';

const pageUrl = process.argv[2];
const origin = new URL(pageUrl).origin;
const redact = (text) => String(text).replace(/\/s\/[0-9a-f]{64}\//g, '/s/<capability>/');
let failures = 0;
let numbered = 0;
function check(ok, label, detail) {
  if (!ok) failures++;
  numbered++;
  console.log((ok ? 'PASS ' : 'FAIL ') + 'J' + String(numbered).padStart(2, '0') + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
}
const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));
async function until(predicate, what, timeout = 8000) {
  for (const end = Date.now() + timeout; Date.now() < end; await sleep(10)) if (predicate()) return true;
  check(false, 'timed out waiting for ' + what);
  return false;
}

// ---- the minimal standard-DOM stand-in of tests/ui-smoke.sh: only what the page scripts use -----------------------------
const SVG_NS = 'http://www.w3.org/2000/svg';
const pageErrors = [];
process.on('unhandledRejection', (error) => pageErrors.push(String((error && error.stack) || error)));

class FakeEvent {
  constructor(type, init = {}) {
    Object.assign(this, { bubbles: true }, init);
    this.type = type;
    this.defaultPrevented = false;
    this.stopped = false;
  }
  preventDefault() { this.defaultPrevented = true; }
  stopPropagation() { this.stopped = true; }
}

function compileSelector(selector) {
  return selector.split(',').map((part) => {
    const text = part.trim();
    const tests = [];
    const token = /([a-zA-Z][\w-]*)|#([\w-]+)|\.([\w-]+)|\[([\w-]+)(?:="((?:[^"\\]|\\.)*)")?\]/y;
    for (let at = 0; at < text.length; at = token.lastIndex) {
      token.lastIndex = at;
      const m = token.exec(text);
      if (!m) throw new Error('selector not supported by the stand-in: ' + selector);
      if (m[1]) tests.push((e) => e.localName === m[1].toLowerCase());
      else if (m[2]) tests.push((e) => e.getAttribute('id') === m[2]);
      else if (m[3]) tests.push((e) => e.classList.contains(m[3]));
      else if (m[5] !== undefined) { const value = m[5].replace(/\\(.)/g, '$1'); tests.push((e) => e.getAttribute(m[4]) === value); }
      else tests.push((e) => e.hasAttribute(m[4]));
    }
    return (element) => tests.every((test) => test(element));
  });
}

class FakeNode {
  constructor(document) { this.ownerDocument = document; this.parentNode = null; this.childNodes = []; this.listeners = []; }
  get isConnected() { let node = this; while (node.parentNode) node = node.parentNode; return node === this.ownerDocument.documentElement; }
  get textContent() { return this.childNodes.map((child) => child.textContent).join(''); }
  contains(other) { for (let node = other; node; node = node.parentNode) if (node === this) return true; return false; }
  addEventListener(type, listener) { this.listeners.push({ type, listener }); }
  dispatchEvent(event) {
    event.target = this;
    for (let node = this; node; node = node.parentNode) {
      event.currentTarget = node;
      for (const entry of node.listeners.slice()) {
        if (entry.type !== event.type) continue;
        try {
          const result = entry.listener.call(node, event);
          if (result && typeof result.catch === 'function') result.catch((error) => pageErrors.push(String((error && error.stack) || error)));
        } catch (error) { pageErrors.push(String((error && error.stack) || error)); }
      }
      if (!event.bubbles || event.stopped) break;
    }
    return !event.defaultPrevented;
  }
}

class FakeText extends FakeNode {
  constructor(document, data) { super(document); this.data = String(data); this.nodeType = 3; }
  get textContent() { return this.data; }
  set textContent(value) { this.data = String(value); }
}

class FakeClassList {
  constructor(element) { this.element = element; }
  names() { return (this.element.getAttribute('class') || '').split(/\s+/).filter(Boolean); }
  add(...names) { this.element.setAttribute('class', [...new Set([...this.names(), ...names])].join(' ')); }
  remove(...names) { this.element.setAttribute('class', this.names().filter((name) => !names.includes(name)).join(' ')); }
  contains(name) { return this.names().includes(name); }
  toggle(name, force) {
    const on = force === undefined ? !this.contains(name) : Boolean(force);
    if (on) this.add(name); else this.remove(name);
    return on;
  }
}

class FakeElement extends FakeNode {
  constructor(document, name, namespaceURI) {
    super(document);
    this.localName = namespaceURI === SVG_NS ? name : name.toLowerCase();
    this.namespaceURI = namespaceURI;
    this.nodeType = 1;
    this.attributesMap = new Map();
    this.typedValue = null;
    this.selected = false;
    this.open = false;
    this.scrollLeft = 0;
    this.scrollTop = 0;
    this.clientWidth = 300;
    this.clientHeight = 300;
  }
  get tagName() { return this.localName.toUpperCase(); }
  get classList() { return new FakeClassList(this); }
  setAttribute(name, value) { this.attributesMap.set(name, String(value)); }
  getAttribute(name) { return this.attributesMap.has(name) ? this.attributesMap.get(name) : null; }
  hasAttribute(name) { return this.attributesMap.has(name); }
  removeAttribute(name) { this.attributesMap.delete(name); }
  get id() { return this.getAttribute('id') || ''; }
  get hidden() { return this.hasAttribute('hidden'); }
  set hidden(on) { if (on) this.setAttribute('hidden', ''); else this.removeAttribute('hidden'); }
  get disabled() { return this.hasAttribute('disabled'); }
  set disabled(on) { if (on) this.setAttribute('disabled', ''); else this.removeAttribute('disabled'); }
  get textContent() { return super.textContent; }
  set textContent(value) { this.replaceChildren(String(value)); }
  get options() { return this.childNodes.filter((child) => child.localName === 'option'); }
  get value() {
    if (this.localName === 'select') { const chosen = this.options.find((o) => o.selected) || this.options[0]; return chosen ? chosen.value : ''; }
    if (this.localName === 'option') return this.getAttribute('value') ?? this.textContent;
    return this.typedValue ?? '';
  }
  set value(value) {
    if (this.localName === 'select') { for (const option of this.options) option.selected = option.value === String(value); return; }
    this.typedValue = String(value);
  }
  append(...items) {
    for (const item of items) {
      const node = item instanceof FakeNode ? item : new FakeText(this.ownerDocument, String(item));
      if (node.parentNode) node.parentNode.childNodes.splice(node.parentNode.childNodes.indexOf(node), 1);
      node.parentNode = this;
      this.childNodes.push(node);
    }
  }
  replaceChildren(...items) {
    for (const child of this.childNodes) child.parentNode = null;
    this.childNodes = [];
    this.append(...items);
  }
  // Standard Element.prepend / firstElementChild / scrollIntoView, which the owner UX repair R2 page uses to keep the
  // ground panel below the inspector's primary block (the stand-in has no layout, so scrollIntoView does nothing).
  prepend(...items) {
    const nodes = items.map((item) => item instanceof FakeNode ? item : new FakeText(this.ownerDocument, String(item)));
    for (const node of nodes) {
      if (node.parentNode) node.parentNode.childNodes.splice(node.parentNode.childNodes.indexOf(node), 1);
      node.parentNode = this;
    }
    this.childNodes.unshift(...nodes);
  }
  get firstElementChild() { return this.childNodes.find((child) => child instanceof FakeElement) || null; }
  scrollIntoView() {}
  matches(selector) { return compileSelector(selector).some((test) => test(this)); }
  closest(selector) {
    const tests = compileSelector(selector);
    for (let node = this; node instanceof FakeElement; node = node.parentNode) if (tests.some((test) => test(node))) return node;
    return null;
  }
  querySelectorAll(selector) {
    const tests = compileSelector(selector);
    const found = [];
    const walk = (node) => {
      for (const child of node.childNodes) {
        if (!(child instanceof FakeElement)) continue;
        if (tests.some((test) => test(child))) found.push(child);
        walk(child);
      }
    };
    walk(this);
    return found;
  }
  querySelector(selector) { return this.querySelectorAll(selector)[0] || null; }
  focus() {
    const focusable = ['button', 'input', 'select', 'textarea', 'summary', 'a'].includes(this.localName) || this.hasAttribute('tabindex');
    if (!focusable || this.disabled || !this.isConnected) return;
    for (let node = this; node; node = node.parentNode) if (node.hidden || (node.localName === 'dialog' && !node.open)) return;
    this.ownerDocument.focused = this;
  }
  showModal() {
    if (this.open) throw new Error('InvalidStateError: the dialog is already open');
    this.open = true;
  }
  close() {
    if (!this.open) return;
    this.open = false;
    setTimeout(() => this.dispatchEvent(new FakeEvent('close', { bubbles: false })), 0);
  }
  // The stand-in has no layout, so client coordinates are map metres (an identity screen transform).
  getScreenCTM() {
    const identity = { a: 1, b: 0, c: 0, d: 1, e: 0, f: 0, inverse: () => identity };
    return identity;
  }
  // Standard Node.cloneNode, which the #885 page's drawGrid uses for the inner grid lines (J01 root cause on the base:
  // without it the first render threw before the tree table was filled).
  cloneNode(deep) {
    const copy = new FakeElement(this.ownerDocument, this.localName, this.namespaceURI);
    for (const [name, value] of this.attributesMap) copy.attributesMap.set(name, value);
    if (deep) for (const child of this.childNodes) copy.append(child instanceof FakeElement ? child.cloneNode(true) : new FakeText(this.ownerDocument, child.textContent));
    return copy;
  }
}

class FakeDocument {
  constructor() { this.focused = null; this.title = ''; this.documentElement = null; this.body = null; }
  createElement(name) { return new FakeElement(this, name, 'http://www.w3.org/1999/xhtml'); }
  createElementNS(namespaceURI, name) { return new FakeElement(this, name, namespaceURI); }
  getElementById(id) { return this.documentElement.querySelector('#' + id); }
  querySelector(selector) { return this.documentElement.querySelector(selector); }
  querySelectorAll(selector) { return this.documentElement.querySelectorAll(selector); }
  get activeElement() {
    const node = this.focused;
    return !node || !node.isConnected || node.disabled || node.closest('[hidden]') ? this.body : node;
  }
}

class FakeDOMPoint {
  constructor(x = 0, y = 0) { this.x = x; this.y = y; }
  matrixTransform(m) { return new FakeDOMPoint(m.a * this.x + m.c * this.y + m.e, m.b * this.x + m.d * this.y + m.f); }
}

function decodeEntities(text) {
  return text.replace(/&(#x[0-9a-f]+|#\d+|amp|lt|gt|quot|apos|nbsp);/gi, (all, name) => {
    const lower = name.toLowerCase();
    if (lower[0] === '#') return String.fromCodePoint(lower[1] === 'x' ? parseInt(lower.slice(2), 16) : parseInt(lower.slice(1), 10));
    return { amp: '&', lt: '<', gt: '>', quot: '"', apos: "'", nbsp: ' ' }[lower];
  });
}

function parsePage(document, html) {
  const VOID = new Set(['meta', 'link', 'input', 'br', 'hr', 'img', 'source', 'area', 'base', 'col', 'wbr']);
  const holder = document.createElement('holder');
  const stack = [holder];
  const token = /<!--[\s\S]*?-->|<!doctype[^>]*>|<\/([a-zA-Z][\w-]*)\s*>|<([a-zA-Z][\w-]*)([^>]*)>|([^<]+)/gi;
  for (let m = token.exec(html); m; m = token.exec(html)) {
    if (m[1]) {
      const name = m[1].toLowerCase();
      for (let i = stack.length - 1; i > 0; i--) if (stack[i].localName.toLowerCase() === name) { stack.length = i; break; }
    } else if (m[2]) {
      const parent = stack[stack.length - 1];
      const inSvg = m[2].toLowerCase() === 'svg' || stack.some((node) => node.namespaceURI === SVG_NS);
      const element = inSvg ? document.createElementNS(SVG_NS, m[2]) : document.createElement(m[2]);
      const attributeText = m[3].replace(/\/\s*$/, '');
      const attribute = /([^\s=]+)(?:\s*=\s*(?:"([^"]*)"|'([^']*)'|([^\s"']+)))?/g;
      for (let a = attribute.exec(attributeText); a; a = attribute.exec(attributeText)) {
        element.setAttribute(a[1], decodeEntities(a[2] ?? a[3] ?? a[4] ?? ''));
      }
      parent.append(element);
      if (!VOID.has(m[2].toLowerCase()) && !/\/\s*$/.test(m[3])) stack.push(element);
    } else if (m[4]) {
      stack[stack.length - 1].append(decodeEntities(m[4]));
    }
  }
  const html5 = holder.childNodes.find((node) => node instanceof FakeElement && node.localName === 'html');
  html5.parentNode = null;
  document.documentElement = html5;
  document.body = html5.querySelector('body');
}

// ---- the real process: a recording fetch for the page, and a direct client --------------------------------------------
const requests = [];
const faults = [];
function arm(method, path, mode) {
  const fault = { method, path, mode };
  fault.held = new Promise((resolve) => { fault.release = resolve; });
  faults.push(fault);
  return fault;
}
async function send(method, path, body) {
  const headers = method === 'POST' ? { 'Content-Type': 'application/json', Origin: origin } : {};
  const response = await fetch(new URL(path, pageUrl), { method, headers, body, redirect: 'error' });
  const text = await response.text();
  let json = null;
  try { json = JSON.parse(text); } catch { json = null; }
  return { status: response.status, json };
}
async function pageFetch(path, init = {}) {
  const method = init.method || 'GET';
  const parsedUrl = new URL(path, pageUrl);
  const relative = parsedUrl.pathname.replace(/^\/s\/[0-9a-f]{64}\//, '') + parsedUrl.search;
  requests.push({ method, path: relative, body: init.body === undefined ? null : JSON.parse(init.body) });
  const index = faults.findIndex((fault) => fault.method === method && fault.path === relative);
  const fault = index < 0 ? null : faults.splice(index, 1)[0];
  if (fault && fault.mode === 'fail') throw new TypeError('Failed to fetch (witness: the request was not sent)');
  if (fault && fault.mode === 'hold') await fault.held;
  const answer = await send(method, relative, init.body);
  if (fault && fault.mode === 'lose') throw new TypeError('Failed to fetch (witness: the answer was lost after HTTP ' + answer.status + ')');
  // 'unpaired': the server's answer exactly, except that its engine read-out is missing (stage4: null), as the server sends
  // when the session moved between a command and its read-out (owner UX repair F-2).
  if (fault && fault.mode === 'unpaired' && answer.json) answer.json.stage4 = null;
  // 'null-structure': simulates a stand with < 2 living trees (null M_i / T_i) to witness Unavailable rendering (never 0.00).
  if (fault && fault.mode === 'null-structure' && answer.json) {
    answer.json.meanMingling = null;
    answer.json.meanDiameterDifferentiation = null;
    for (const a of (answer.json.areas || [])) { a.meanMingling = null; a.meanDiameterDifferentiation = null; }
  }
  return { status: answer.status, ok: answer.status >= 200 && answer.status < 300, json: async () => answer.json };
}
const serverView = async () => (await send('GET', 'api/multi/view')).json.view;
const areaOf = (view, areaId) => view.areas.find((area) => area.areaId === areaId);
const living = (view, areaId, treeId) => areaOf(view, areaId).trees.some((tree) => tree.treeId === treeId && tree.isLiving);

// ---- start the page ----------------------------------------------------------------------------------------------------
const multiUrl = new URL('multi', pageUrl).href;
for (let attempt = 0; attempt < 40; attempt++) {
  try { const probe = await fetch(multiUrl); if (probe.status === 200) break; } catch { /* wait for loopback relay */ }
  await sleep(100);
}
const document = new FakeDocument();
parsePage(document, await (await fetch(multiUrl)).text());
const multiJs = await (await fetch(new URL('multi.js', pageUrl))).text();
const sandbox = {
  document, fetch: pageFetch, setTimeout, clearTimeout,
  CSS: { escape: (value) => String(value).replace(/[^a-zA-Z0-9_ -￿-]/g, (c) => '\\' + c) },
  DOMPoint: FakeDOMPoint, Node: FakeNode, Element: FakeElement,
  matchMedia: (media) => ({ media, matches: false, addEventListener() {}, removeEventListener() {} }),
  devicePixelRatio: 1, addEventListener() {},
};
sandbox.window = sandbox;
// The page's relative URLs resolve against .../multi, which is the same /s/<capability>/ directory as the API.
vm.runInContext(multiJs, vm.createContext(sandbox), { filename: 'multi.js' });

const byId = (id) => document.getElementById(id);
const click = (element, init) => element.dispatchEvent(new FakeEvent('click', init));
const alertText = () => byId('alert').textContent;
const rowIds = () => byId('tree-rows').querySelectorAll('th').map((th) => th.textContent);
const pendingIds = () => byId('pending-areas').querySelectorAll('.mono').map((node) => node.textContent);
const approvedIds = () => byId('approved-areas').querySelectorAll('.mono').map((node) => node.textContent);
async function selectInTable(treeId) {
  click(byId('tree-rows').querySelector('[data-select="' + treeId + '"]'));
  await until(() => !byId('inspector-body').hidden && byId('insp-id').textContent === treeId, 'the inspector to show ' + treeId);
}
const competitorButtons = () => byId('rank-layer').querySelectorAll('[data-rank-toggle]');

await until(() => byId('tree-rows').childNodes.length === 171, 'the first view (Area A, 171 rows)');
const start = await serverView();
check(requests.length === 1 && requests[0].method === 'GET' && requests[0].path === 'api/multi/view',
  'the served page starts with one GET api/multi/view', requests);
check(byId('area-switch').querySelectorAll('[data-area]').length === 2 && byId('area-switch').querySelector('[data-area="MA-A"]').getAttribute('aria-pressed') === 'true',
  'two area buttons; Area A is shown first');
// /multi runs Stage 4 (1/1/1) by default (owner decision 2026-11-23, spec multi-stage4-ui-catchup-v1): growth and crowns
// use the connected woodland, so the page states that instead of the model 0 "edge neighbours excluded" statement.
check(['NOT A SURVEYED IRISH FOREST', 'AREAS NOT ASSESSED COMPARABLE', 'SITE UNKNOWN', 'CONNECTED WOODLAND', 'NO TREATMENT TRANSFER']
  .every((phrase) => byId('disclosure').textContent.includes(phrase)) && byId('flags').textContent.includes('Site UNKNOWN') &&
  byId('flags').textContent.includes('Connected woodland') && byId('flags').textContent.includes('No treatment transfer') &&
  !byId('disclosure').textContent.includes('EDGE NEIGHBOURS EXCLUDED') &&
  byId('flags').querySelectorAll('li').every((chip) => chip.getAttribute('title')),
  'the disclosure and the site UNKNOWN / connected woodland / no treatment transfer statements are visible');

// Any living tree: competitors appear immediately, before any Crop designation, and nothing changes on the server (N-3 restored exact checks).
const aTree = areaOf(start, 'MA-A').trees[60].treeId;
await selectInTable(aTree);
await until(() => competitorButtons().length > 0, 'competitors of a non-Crop tree in A');
const compDraftIds = byId('competition-content').querySelectorAll('[data-draft]').map((el) => el.getAttribute('data-draft'));
const rankToggleIds = byId('rank-layer').querySelectorAll('[data-rank-toggle]').map((el) => el.getAttribute('data-rank-toggle'));
check(requests.some((r) => r.path === 'api/multi/competition' && r.body.areaId === 'MA-A' && r.body.treeId === aTree) &&
  byId('crop-button').textContent === 'Retain as Crop Tree' && byId('reason-release').disabled && byId('insp-chip').hidden &&
  byId('competition-more').textContent.includes('Current total Hegyi CI') && compDraftIds.length === 5,
  'selecting a non-Crop living tree in A reads its ranked competitors at once (confirmed non-Crop)');
check(rankToggleIds.length === compDraftIds.length && rankToggleIds.every((id, i) => id === compDraftIds[i]),
  'the map numbers the exact strongest competitors matching the ranked list');
let server = await serverView();
check(server.areas.every((area) => area.cropTreeIds.length === 0) && server.pending.length === 0 && server.planStateRevision === 0,
  'inspection created no Crop mark and no Fell order');

// ---- F-2 (owner UX repair Phase A, spec multi-stage4-owner-ux-20261123-v1): a view whose engine read-out could not be
// paired (stage4: null) is shown as "unavailable", never as model 0 / legacy, and Execute is withdrawn until a paired
// re-read; the server's stale-fence refusal stays fail-closed. Labelled F2a.. so the J numbering below is unchanged.
{
  let f2 = 0;
  const checkF2 = (ok, label, detail) => {
    if (!ok) failures++;
    f2++;
    console.log((ok ? 'PASS ' : 'FAIL ') + 'F2' + String.fromCharCode(96 + f2) + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
  };
  const waitF2 = async (predicate, what) => {
    for (const end = Date.now() + 8000; Date.now() < end; await sleep(10)) if (predicate()) return true;
    checkF2(false, 'timed out waiting for ' + what);
    return false;
  };
  const fenceOf = (view) => ({ sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision });
  let v = await serverView();
  const f2Tree = areaOf(v, 'MA-A').trees.find((t) => t.isLiving && t.treeId !== aTree).treeId;
  const added = await send('POST', 'api/multi/pending/add', JSON.stringify({ fence: fenceOf(v), areaId: 'MA-A', treeId: f2Tree, reason: 'page witness F-2' }));
  const staleFence = fenceOf(v);
  v = await serverView();
  const approved = await send('POST', 'api/multi/plan/approve', JSON.stringify({ fence: fenceOf(v) }));
  checkF2(added.status === 200 && approved.status === 200, 'setup: one Fell order queued and approved through the API', [added.status, approved.status]);
  const plan = (await serverView()).approved;
  arm('GET', 'api/multi/view', 'unpaired');
  let f2Mark = requests.length;
  click(byId('reread'));
  if (await waitF2(() => !byId('approved-box').hidden && !byId('readout-unavailable').hidden, 'the unpaired view with its approved plan')) {
    const disclosure = byId('disclosure').textContent + ' | ' + byId('flags').textContent;
    checkF2(!/EDGE NEIGHBOURS EXCLUDED|Edge neighbours excluded/i.test(disclosure) && disclosure.includes('unavailable') && !disclosure.includes('CONNECTED WOODLAND'),
      'unavailable: the notice claims neither model 0 edge exclusion nor the Stage 4 model', disclosure);
    const details = byId('statements').textContent + ' | ' + byId('details-note').textContent;
    checkF2(!details.includes('Edge neighbours excluded') && !details.includes('Light, seed') && details.includes('unavailable'),
      'unavailable: the Details pane says so and makes no legacy "Light NOT MODELLED" or edge-exclusion claim', details);
    checkF2(byId('execute').disabled && byId('approve').disabled && byId('execute').textContent.includes('re-read needed') &&
      byId('area-cards').textContent.includes('read-out unavailable'),
      'unavailable: Execute and Approve are withdrawn and the rack cards say the read-out is unavailable', byId('execute').textContent);
    click(byId('execute'));
    await sleep(100);
    checkF2(!byId('execute-dialog').open && alertText().includes('Execute needs a re-read') &&
      !requests.slice(f2Mark).some((r) => r.path === 'api/multi/plan/execute'), 'unavailable: no Execute confirmation opens and no Execute is sent', alertText());
  }
  const stale = await send('POST', 'api/multi/plan/execute', JSON.stringify({ fence: staleFence, planId: plan.planId, executionKey: plan.executionKey }));
  checkF2(stale.status === 409 && stale.json && stale.json.refusal && (await serverView()).events.length === 0,
    'the server still refuses an Execute with a stale fence (409, fail-closed, nothing felled)', [stale.status, stale.json && stale.json.refusal]);
  click(byId('readout-reread'));
  await waitF2(() => byId('readout-unavailable').hidden && !byId('execute').disabled && byId('disclosure').textContent.includes('CONNECTED WOODLAND'),
    'a paired re-read restoring the Stage 4 statements and Execute');
  checkF2(byId('readout-unavailable').hidden && !byId('execute').disabled, 'after a paired re-read Execute is offered again');
  v = await serverView();
  const cancelled = await send('POST', 'api/multi/plan/cancel', JSON.stringify({ fence: fenceOf(v) }));
  click(byId('reread'));
  await waitF2(() => byId('approved-box').hidden, 'the cancelled plan to disappear');
  checkF2(cancelled.status === 200 && (await serverView()).approved === null, 'cleanup: the F-2 plan was cancelled through the API (nothing was felled)');
}

// ---- R2 (owner UX repair Phase A, repair 2): (1) a new selection puts the inspector back at the top and, with Inspect
// ground on, the ground panel sits below the primary block; (2) a map hit-test answer that arrives after Escape, Close or
// a newer selection is discarded whole. Labelled R2a.. so the J numbering below is unchanged. The stand-in has no layout,
// so (1) is checked structurally here and by pixels in the real-browser driver.
{
  let r2 = 0;
  const checkR2 = (ok, label, detail) => {
    if (!ok) failures++;
    r2++;
    console.log((ok ? 'PASS ' : 'FAIL ') + 'R2' + String.fromCharCode(96 + r2) + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
  };
  const waitR2 = async (predicate, what) => {
    for (const end = Date.now() + 8000; Date.now() < end; await sleep(10)) if (predicate()) return true;
    checkR2(false, 'timed out waiting for ' + what);
    return false;
  };
  const escape = () => document.documentElement.dispatchEvent(new FakeEvent('keydown', { key: 'Escape' }));
  const pane = byId('pane-tree');
  const v = (await send('GET', 'api/multi/view')).json;
  const aScene = v.scenes['MA-A'];
  const aLiving = areaOf(v.view, 'MA-A').trees.filter((t) => t.isLiving).map((t) => t.treeId);
  const itemOf = (id) => aScene.find((item) => item.treeId === id);
  const crownAt = (id) => byId('crowns').querySelector('[data-tree-id="' + id + '"]').childNodes[0];
  const firstTree = aLiving.find((id) => id !== aTree && itemOf(id));
  const crownTree = aLiving.filter((id) => itemOf(id))[Math.floor(aLiving.length / 2)];
  const crown = itemOf(crownTree);
  // A newer selection far from the clicked crown, so the stale hit-test cannot name it.
  const farTree = aLiving.filter((id) => itemOf(id) && id !== crownTree)
    .sort((x, y) => Math.hypot(itemOf(y).u - crown.u, itemOf(y).v - crown.v) - Math.hypot(itemOf(x).u - crown.u, itemOf(x).v - crown.v))[0];

  // (1) Scroll reset on a table selection and on a crown click.
  pane.scrollTop = 420;
  await selectInTable(firstTree);
  checkR2(pane.scrollTop === 0, 'a new selection from the Trees table puts the inspector back at the top (scrollTop 420 → 0)', pane.scrollTop);
  pane.scrollTop = 420;
  let mark = requests.length;
  click(crownAt(crownTree), { clientX: crown.u, clientY: crown.v });
  await waitR2(() => requests.slice(mark).some((r) => r.path === 'api/multi/hit') && byId('insp-id').textContent !== firstTree, 'a crown click to select a tree');
  checkR2(pane.scrollTop === 0 && !byId('inspector-body').hidden, 'a new selection by a crown click puts the inspector back at the top', [pane.scrollTop, byId('insp-id').textContent]);

  // (1) Pass 1 Inspect Cell explicit swap (#cell-panel replaces #tree-panel), overlay selector, POST api/multi/cell,
  // and Escape exiting cell mode WITHOUT clearing draft Fell ticks or active overlay.
  await waitR2(() => byId('rank-layer').querySelector('[data-rank-toggle]'), 'competitor rank button before Inspect Cell');
  const draftRank = byId('rank-layer').querySelector('[data-rank-toggle]');
  if (draftRank) click(draftRank);
  await waitR2(() => byId('add-button').textContent.includes('1'), 'one draft choice before entering Inspect Cell');
  byId('overlay-select').value = 'light';
  byId('overlay-select').dispatchEvent(new FakeEvent('change'));
  click(byId('mode-cell'));
  await waitR2(() => !byId('cell-panel').hidden && byId('tree-panel').hidden, 'Inspect Cell swaps #cell-panel in and hides #tree-panel');
  checkR2(!byId('cell-panel').hidden && byId('tree-panel').hidden &&
    byId('mode-cell').getAttribute('aria-pressed') === 'true' && byId('mode-tree').getAttribute('aria-pressed') === 'false' &&
    document.activeElement === byId('mode-cell') && pane.scrollTop === 0 &&
    byId('overlay-select').value === 'light' && !byId('overlay-legend').hidden,
    'Inspect Cell explicitly swaps the right-hand inspector body, keeps focus on the pressed toggle without scrolling (N-4), and keeps Light Guide active',
    [document.activeElement && document.activeElement.id, pane.scrollTop]);
  const midCell = v.stage4.areas.find((a) => a.areaId === 'MA-A').ground[Math.floor(v.stage4.areas.find((a) => a.areaId === 'MA-A').ground.length / 2)];
  mark = requests.length;
  click(byId('map'), { clientX: midCell.centreU, clientY: midCell.centreV });
  await waitR2(() => requests.slice(mark).some((r) => r.path === 'api/multi/cell') && byId('cell-heading').textContent.includes('cell (' + midCell.column + ', ' + midCell.row + ')'),
    'POST api/multi/cell to render cell detail');
  checkR2(byId('cell-summary').textContent.includes('Modelled light · not calibrated PAR') && byId('cell-cohorts').textContent.includes('Natural regeneration cohorts'),
    'POST api/multi/cell renders light caveat, undergrowth and cohorts read-only');
  escape();
  await waitR2(() => byId('cell-panel').hidden && !byId('tree-panel').hidden, 'Escape exits Inspect Cell mode');
  checkR2(byId('add-button').textContent === 'Add selected 1 to Fell plan' && byId('overlay-select').value === 'light' &&
    document.activeElement === byId('mode-tree'),
    'Escape exits Inspect Cell back to Select Tree (focus on mode-tree) WITHOUT clearing draft Fell ticks or active overlay',
    [byId('add-button').textContent, byId('overlay-select').value, document.activeElement && document.activeElement.id]);
  byId('overlay-select').value = 'off';
  byId('overlay-select').dispatchEvent(new FakeEvent('change'));
  click(byId('inspect-close'));
  await waitR2(() => byId('inspector-body').hidden, 'Close inspection');

  // (2) Escape while the hit-test is in flight: the late answer must not reopen the inspection.
  await selectInTable(firstTree);
  let held = arm('POST', 'api/multi/hit', 'hold');
  mark = requests.length;
  click(crownAt(crownTree), { clientX: crown.u, clientY: crown.v });
  await waitR2(() => requests.slice(mark).some((r) => r.path === 'api/multi/hit'), 'the held hit-test to be sent');
  escape();
  checkR2(byId('inspector-body').hidden, 'Escape closes the inspection at once while the hit-test is still in flight');
  await sleep(80);
  const announcedAfterEscape = byId('announce').textContent;
  held.release();
  await sleep(400);
  checkR2(byId('inspector-body').hidden && !byId('inspector-empty').hidden && !byId('tree-rows').querySelector('[aria-current="true"]'),
    'the late hit-test answer did not reselect a tree or reopen the inspection', byId('insp-id').textContent);
  checkR2(byId('announce').textContent === announcedAfterEscape && !byId('announce').textContent.startsWith('Selected tree') &&
    !requests.slice(mark).some((r) => r.path === 'api/multi/competition') && alertText() === '',
    'the late answer made no announcement, no competitor read and no alert', [byId('announce').textContent, alertText()]);

  // (2) A newer selection while the hit-test is in flight wins, with its draft intact.
  await selectInTable(firstTree);
  held = arm('POST', 'api/multi/hit', 'hold');
  mark = requests.length;
  click(crownAt(crownTree), { clientX: crown.u, clientY: crown.v });
  await waitR2(() => requests.slice(mark).some((r) => r.path === 'api/multi/hit'), 'the second held hit-test to be sent');
  await selectInTable(farTree);
  await sleep(250); // the competitor read for farTree answers while the hit-test is still held (busy), so it is dropped
  held.release();
  await waitR2(() => byId('rank-layer').querySelector('[data-rank-toggle]'), 'the newer selection\'s competitors (re-read after the stale answer)');
  const rank = byId('rank-layer').querySelector('[data-rank-toggle]');
  if (rank) click(rank);
  await waitR2(() => byId('add-button').textContent.includes('1'), 'one draft choice for the newer selection');
  const addText = byId('add-button').textContent;
  await sleep(300);
  checkR2(byId('insp-id').textContent === farTree && byId('add-button').textContent === addText && addText.includes('1') &&
    !byId('announce').textContent.includes(crownTree),
    'a stale hit-test answer never overwrites a newer selection or clears its draft', [byId('insp-id').textContent, farTree, addText, byId('announce').textContent]);
  click(byId('inspect-close'));
  await waitR2(() => byId('inspector-body').hidden, 'cleanup: close the inspection');
  await selectInTable(aTree);
}

// ---- R3 (owner UX repair Phase A, repair 3): a LOADED session. Fell orders are queued through the page's own Add for the
// top competitors of one reference, so its rows are blocked. The stand-in has no layout, so the fold is checked
// structurally here (every row, blocked or not, is the same two-line grid: rank + ID + distance, then one meta line that
// starts with the short reason) and by pixels in the real-browser driver and fold sweep. Also: the post-Add card takes the
// Add form's place with Details and Dismiss, a draft change ends it, and an Escape that only closes a dialog leaves a
// pending map click alone (F-05). The orders are removed again at the end so the checks below see the same session as
// before. Labelled R3a.. so the J numbering below is unchanged.
{
  let r3 = 0;
  const checkR3 = (ok, label, detail) => {
    if (!ok) failures++;
    r3++;
    console.log((ok ? 'PASS ' : 'FAIL ') + 'R3' + String.fromCharCode(96 + r3) + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
  };
  const waitR3 = async (predicate, what) => {
    for (const end = Date.now() + 8000; Date.now() < end; await sleep(10)) if (predicate()) return true;
    checkR3(false, 'timed out waiting for ' + what);
    return false;
  };
  const escape = () => document.documentElement.dispatchEvent(new FakeEvent('keydown', { key: 'Escape' }));
  const errorsBefore = pageErrors.length;
  const v0 = await serverView();
  const fenceOf = (view) => ({ sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision });
  const aIds = areaOf(v0, 'MA-A').trees.filter((t) => t.isLiving).map((t) => t.treeId);
  let refA = null;
  let topA = null;
  for (const id of aIds) {
    if (id === aTree) continue;
    const r = await send('POST', 'api/multi/competition', JSON.stringify({ fence: fenceOf(v0), areaId: 'MA-A', treeId: id }));
    if (r.json && r.json.competition && r.json.competition.topFive.length === 5) { refA = id; topA = r.json.competition.topFive.map((n) => n.treeId); break; }
  }
  const rankButton = (id) => byId('rank-layer').querySelector('[data-rank-toggle="' + id + '"]');
  const rowsOf = () => byId('competition-content').querySelectorAll('li');
  // Looked up by class too, so that against an older page these checks fail instead of crashing the witness.
  const dock = byId('add-dock') || byId('inspector-body').querySelector('.add-dock');
  const card = byId('batch-result');
  const control = (name) => card.querySelector('[data-batch="' + name + '"]');
  const addThrough = async (ids, what) => {
    for (const id of ids) click(rankButton(id));
    await waitR3(() => byId('add-button').textContent === 'Add selected ' + ids.length + ' to Fell plan', what + ': ' + ids.length + ' draft choices');
    byId('add-form').dispatchEvent(new FakeEvent('submit'));
    await waitR3(() => !card.hidden && !document.documentElement.classList.contains('busy'), what + ': the Add result card');
    await rowsBack(what);
  };
  // After an Add the page re-reads the reference's competition; the rows and rank buttons return when it answers.
  async function rowsBack(what) {
    await waitR3(() => rowsOf().length === 5 && byId('rank-layer').querySelectorAll('[data-rank-toggle]').length === 5, what + ': the competitor rows again');
  }
  // The two-line row structure: exactly input, ID, distance, CI and one meta line; a blocked row puts its short reason at
  // the start of the meta line (no extra line), with the full reason in the tooltip and in screen-reader text.
  const rowShape = () => rowsOf().map((li) => {
    const label = li.querySelector('label');
    const kids = label.childNodes.filter((n) => n instanceof FakeElement);
    const line = label.querySelector('.comp-line');
    const why = line && line.querySelector('.comp-why');
    const sr = line && line.querySelector('.sr-only');
    return { blocked: li.classList.contains('blocked'), shape: kids.map((k) => k.localName === 'input' ? 'input' : (k.getAttribute('class') || '').split(' ').pop()).join(','),
      first: line && line.firstElementChild ? line.firstElementChild.getAttribute('class') : null, why: why ? why.textContent : null,
      whyHidden: why ? why.getAttribute('aria-hidden') : null, sr: sr ? sr.textContent : null, title: label.getAttribute('title') };
  });
  const shapeOk = (rows, blockedIds) => rows.length === 5 && rows.every((r) => r.shape === 'input,comp-id,comp-dist,comp-ci,comp-line') &&
    rows.filter((r) => r.blocked).length === blockedIds &&
    rows.every((r) => r.blocked ? r.first === 'comp-why' && r.why === 'Pending Fell' && r.whyHidden === 'true' && r.sr === 'Cannot be chosen: Already pending Fell. ' &&
      r.title && r.title.includes('Already pending Fell') : r.why === null && r.sr === null && r.title === null);

  if (!refA) checkR3(false, 'no MA-A reference with five competitors found');
  else {
    await selectInTable(refA);
    await waitR3(() => byId('rank-layer').querySelectorAll('[data-rank-toggle]').length === 5, 'the five rank buttons of ' + refA);
    checkR3(shapeOk(rowShape(), 0), 'clean session: all five rows of ' + refA + ' have the same two-line structure and no reason', rowShape());
    await addThrough(topA.slice(0, 3), 'first Add');
    let s = await serverView();
    checkR3(topA.slice(0, 3).every((id) => s.pending.some((o) => o.areaId === 'MA-A' && o.treeId === id)) && s.pending.length === v0.pending.length + 3,
      'three of ' + refA + '\'s competitors were queued through the page\'s Add', s.pending.map((o) => o.treeId));
    checkR3(shapeOk(rowShape(), 3), 'loaded: the three blocked rows keep the two-line structure; "Pending Fell" opens the meta line; full reason in the tooltip and screen-reader text', rowShape());
    const more = control('more');
    const dismiss = control('dismiss');
    checkR3(dock.classList.contains('showing-result') && card.querySelector('.alert-title').textContent === 'Added 3 trees to the Fell plan' && more &&
      more.getAttribute('aria-expanded') === 'false' && more.getAttribute('aria-controls') === 'batch-body' && dismiss && byId('batch-body') &&
      byId('batch-body').textContent.includes(topA[0]),
      'F-02: the post-Add card is in the dock with a one-line title, Details (collapsed) and Dismiss; the dock is marked so a short desktop shows it instead of the Add form',
      { cls: dock.getAttribute('class'), title: card.querySelector('.alert-title') && card.querySelector('.alert-title').textContent });
    if (more) click(more);
    const opened = Boolean(more) && card.getAttribute('class').includes('open') && control('more').getAttribute('aria-expanded') === 'true';
    if (more) click(control('more'));
    checkR3(opened && !card.getAttribute('class').includes('open') && control('more').getAttribute('aria-expanded') === 'false',
      'F-02: Details opens and closes the card\'s detail lines (aria-expanded follows)');
    click(rankButton(topA[3]));
    await waitR3(() => card.hidden, 'a draft change to end the card');
    checkR3(card.hidden && !dock.classList.contains('showing-result') && byId('add-button').textContent === 'Add selected 1 to Fell plan',
      'F-02: changing the draft ends the card and brings the Add form back', [card.hidden, dock.getAttribute('class'), byId('add-button').textContent]);
    if (byId('add-button').textContent === 'Add selected 1 to Fell plan') byId('add-form').dispatchEvent(new FakeEvent('submit'));
    await waitR3(() => !card.hidden && !document.documentElement.classList.contains('busy'), 'second Add: the result card');
    await rowsBack('second Add');
    const sBefore = JSON.stringify(await serverView());
    if (control('dismiss')) click(control('dismiss'));
    await waitR3(() => byId('announce').textContent.startsWith('Dismissed the Add result'), 'the Dismiss announcement');
    checkR3(card.hidden && !dock.classList.contains('showing-result') && JSON.stringify(await serverView()) === sBefore &&
      [byId('add-button'), byId('inspector-heading')].includes(document.activeElement) && byId('announce').textContent.startsWith('Dismissed the Add result'),
      'F-02: Dismiss ends the card, the Add form returns, focus lands on Add or the heading, nothing is sent', [document.activeElement && document.activeElement.id, byId('announce').textContent]);
    checkR3(shapeOk(rowShape(), 4), 'loaded: four blocked rows and one free row, all the same two-line structure', rowShape());

    // F-05: an Escape that only closes a dialog no longer drops a map click whose hit-test is still in flight.
    const scene = (await send('GET', 'api/multi/view')).json.scenes['MA-A'];
    // The living crown farthest from the reference, so the hit-test cannot answer with the reference itself.
    const refItem = scene.find((item) => item.treeId === refA);
    const target = scene.filter((item) => aIds.includes(item.treeId) && item.treeId !== refA)
      .sort((x, y) => Math.hypot(y.u - refItem.u, y.v - refItem.v) - Math.hypot(x.u - refItem.u, x.v - refItem.v))[0];
    const held = arm('POST', 'api/multi/hit', 'hold');
    const mark = requests.length;
    click(byId('crowns').querySelector('[data-tree-id="' + target.treeId + '"]').childNodes[0], { clientX: target.u, clientY: target.v });
    await waitR3(() => requests.slice(mark).some((r) => r.path === 'api/multi/hit'), 'the held hit-test (F-05)');
    byId('consent-dialog').setAttribute('open', '');
    escape();
    byId('consent-dialog').removeAttribute('open');
    held.release();
    await waitR3(() => byId('insp-id').textContent !== refA && byId('announce').textContent.startsWith('Selected tree'), 'the map click answer after a dialog-only Escape');
    checkR3(!byId('inspector-body').hidden && byId('insp-id').textContent !== refA && byId('announce').textContent.startsWith('Selected tree') && alertText() === '',
      'F-05: an Escape that only closed a dialog leaves the pending map click alone; its answer selects ' + byId('insp-id').textContent, [byId('insp-id').textContent, byId('announce').textContent]);

    // Clean-up: remove the four orders and re-read, so the rest of the witness sees the session it saw before R3.
    click(byId('inspect-close'));
    for (const id of topA.slice(0, 4)) {
      const now = await serverView();
      await send('POST', 'api/multi/pending/remove', JSON.stringify({ fence: fenceOf(now), areaId: 'MA-A', treeId: id }));
    }
    click(byId('reread'));
    await waitR3(() => !document.documentElement.classList.contains('busy') && byId('pending-areas').querySelectorAll('.mono').length === 0, 'the re-read after clean-up');
    s = await serverView();
    checkR3(s.pending.length === v0.pending.length && pageErrors.length === errorsBefore, 'clean-up: the queued orders are gone; no page error during R3',
      { pending: s.pending.length, errors: pageErrors.slice(errorsBefore) });
  }
  await selectInTable(aTree);
}

// Area B: switch, then select a tree by a click on its drawn crown.
click(byId('area-switch').querySelector('[data-area="MA-B"]'));
await until(() => byId('tree-rows').childNodes.length === 73, 'the Area B table (73 rows)');
check(rowIds().every((id) => id.startsWith('MA-B-')) && byId('inspector-body').hidden, 'Area B shows only its own exact IDs; nothing selected there yet');
const bScene = (await send('GET', 'api/multi/view')).json.scenes['MA-B'];
const bItem = bScene.find((item) => item.treeId === areaOf(start, 'MA-B').trees[20].treeId);
let mark = requests.length;
click(byId('crowns').querySelector('[data-tree-id="' + bItem.treeId + '"]').childNodes[0], { clientX: bItem.u, clientY: bItem.v });
await until(() => !byId('inspector-body').hidden && byId('insp-id').textContent.startsWith('MA-B-'), 'a B tree selected by map click');
const hit = requests.slice(mark).find((r) => r.path === 'api/multi/hit');
const bTree = byId('insp-id').textContent;
check(hit && hit.body.areaId === 'MA-B' && hit.body.u === bItem.u && hit.body.v === bItem.v && byId('insp-area').textContent.includes('Area B'),
  'the map click was hit-tested by the server in Area B and selected ' + bTree);
await until(() => competitorButtons().length > 0, 'competitors of the B tree');
check(byId('competition-content').querySelectorAll('[data-draft]').every((i) => i.getAttribute('data-draft').startsWith('MA-B-')), 'the B competitors are reported for Area B');
byId('reason').value = 'page witness: typed reason in B';
byId('reason').dispatchEvent(new FakeEvent('input'));
click(byId('add-reference'));
await until(() => pendingIds().includes(bTree), 'the B order in the combined plan');

// Back to A: the selection is remembered; retain a Crop, then propose its strongest competitor with the release reason.
click(byId('area-switch').querySelector('[data-area="MA-A"]'));
await until(() => byId('insp-id').textContent === aTree && competitorButtons().length > 0, 'the remembered A selection with competitors');
click(byId('crop-button'));
await until(() => byId('crop-button').textContent === 'Remove Crop designation', 'the Crop designation');
await until(() => competitorButtons().length > 0 && !byId('reason-release').disabled, 'the Crop competitors and release reason');
const competitorId = competitorButtons()[0].getAttribute('data-rank-toggle');
check(!byId('reason-release').disabled && byId('release-label').textContent === 'Record this as competition release for retained Crop Tree ' + aTree,
  'the release reason is offered with exact text for competitors of the inspected Crop Tree', byId('release-label').textContent);
click(competitorButtons()[0]);
await until(() => byId('add-button').textContent === 'Add selected 1 to Fell plan', 'one draft competitor chosen');
byId('reason-release').checked = true;
byId('reason-release').dispatchEvent(new FakeEvent('change'));
check(byId('reason-preview').textContent === 'Recorded reason for each tree added here: Competition release for retained Crop Tree ' + aTree + ' (operator intent)',
  'reason preview matches the exact Crop Tree release reason text', byId('reason-preview').textContent);
byId('add-form').dispatchEvent(new FakeEvent('submit'));
await until(() => pendingIds().includes(competitorId), 'the A order in the combined plan');
check(byId('pending-total').textContent === '2 pending Fell · not felled (Area A 1 · Area B 1)', 'one Work Plan lists both area sections and the counts',
  byId('pending-total').textContent);
server = await serverView();
check(server.pending.map((o) => o.areaId + ':' + o.treeId).join() === 'MA-B:' + bTree + ',MA-A:' + competitorId &&
  server.pending.find((o) => o.treeId === competitorId).reason === 'Competition release for retained Crop Tree ' + aTree + ' (operator intent)',
  'the server recorded both orders with their exact reasons');

// Approve fells zero; Execute through the frozen confirmation; one receipt names both areas.
click(byId('approve'));
await until(() => approvedIds().includes(competitorId) && approvedIds().includes(bTree), 'the approved two-area plan');
server = await serverView();
check(server.areas.map((a) => a.stand.livingCount).join() === '171,73' && server.events.length === 0, 'Approve felled nothing');
click(byId('execute'));
await until(() => byId('execute-dialog').open, 'the Execute confirmation');
check(byId('execute-text').textContent === 'Fells exactly 2 named trees across 2 areas in Year 0, or refuses all.' &&
  byId('execute-areas').textContent.includes(competitorId) && byId('execute-areas').textContent.includes(bTree),
  'the confirmation names both areas\' trees and the exact count', byId('execute-text').textContent);
check(document.activeElement === byId('execute-back'), 'focus starts on Back, so a repeated key never confirms');
mark = requests.length;
click(byId('execute-confirm'));
await until(() => byId('receipt').textContent.includes(competitorId) && byId('receipt').textContent.includes(bTree), 'the receipt');
const sent = requests.slice(mark).find((r) => r.path === 'api/multi/plan/execute').body;
server = await serverView();
check(sent && sent.planId === server.events[0].planId && sent.executionKey === server.events[0].executionKey, 'Execute sent the confirmed PlanId and minted key');
check(!living(server, 'MA-A', competitorId) && !living(server, 'MA-B', bTree) && server.events.length === 1 &&
  byId('receipt').textContent.includes('171 → 170') && byId('receipt').textContent.includes('73 → 72'),
  'one event felled exactly the two trees; the receipt shows both areas before → after');

// Year+1 from the page: results per area.
click(byId('advance'));
await until(() => byId('year-latest').textContent.includes('Latest year 0 → 1'), 'the Year+1 results');
server = await serverView();
check(server.source.ecologicalYear === 1 && byId('year-latest').textContent.includes('Area A') && byId('year-latest').textContent.includes('Area B') &&
  byId('year-latest').textContent.includes('170 →') && byId('year-latest').textContent.includes('72 →'), 'Year+1 shows each area\'s own result');

// Fail closed: an Add whose answer is lost locks every command until a re-read.
const shown = byId('pending-total').textContent;
arm('POST', 'api/multi/pending/add', 'lose');
const lostTree = areaOf(server, 'MA-A').trees.find((t) => t.isLiving && t.treeId !== aTree).treeId;
await selectInTable(lostTree);
byId('reason').value = 'page witness: lost answer';
byId('reason').dispatchEvent(new FakeEvent('input'));
click(byId('add-reference'));
await until(() => alertText().includes('Outcome unconfirmed'), 'the unconfirmed alert');
check(byId('approve').disabled && byId('advance').disabled && byId('crop-button').disabled && byId('pending-total').textContent === shown,
  'after an unconfirmed outcome every command is disabled and the page claims nothing');
mark = requests.length;
click(byId('advance'));
click(byId('crop-button'));
await sleep(150);
check(requests.length === mark, 'no request but a read can leave the page while locked', requests.slice(mark));
click(byId('alert').querySelector('button'));
await until(() => !byId('advance').disabled && pendingIds().includes(lostTree), 'the re-read to lift the lock and show the committed Add');
check(alertText() === '', 'a successful re-read lifts the lock and clears the alert');

// ---- P2 (Pass 2 interactive Work Plan modes, provisional Draft previews, sequential queue partial disclosure,
// ClearUndergrowth, Plant + Shelter + SpotClear -> Approve -> Execute, live SVG markers, PJ shelter, Escape) ----
{
  let p2 = 0;
  const checkP2 = (ok, label, detail) => {
    if (!ok) failures++;
    p2++;
    console.log((ok ? 'PASS ' : 'FAIL ') + 'P2' + String.fromCharCode(96 + p2) + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
  };
  const waitP2 = async (predicate, what) => {
    for (const end = Date.now() + 8000; Date.now() < end; await sleep(10)) if (predicate()) return true;
    checkP2(false, 'timed out waiting for ' + what);
    return false;
  };
  const escape = () => document.documentElement.dispatchEvent(new FakeEvent('keydown', { key: 'Escape' }));
  const fenceOf = (view) => ({ sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision });
  const inPoly = (pts, u, v) => {
    let inside = false;
    for (let i = 0, j = pts.length - 1; i < pts.length; j = i++) {
      const a = pts[i], b = pts[j];
      if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) inside = !inside;
    }
    return inside;
  };

  // Clear the J21 pending Fell so Work Plan starts clean in MA-A.
  let sv = await serverView();
  for (const o of sv.pending) {
    await send('POST', 'api/multi/pending/remove', JSON.stringify({ fence: fenceOf(sv), areaId: o.areaId, treeId: o.treeId }));
    sv = await serverView();
  }
  click(byId('reread'));
  await waitP2(() => byId('pending-total').textContent === 'No pending orders', 'clean Work Plan for Pass 2 witness');

  // Pick two well-separated open points in MA-A (>= 1.2 m from all living adults, inside an in-polygon cell).
  const fullP2 = (await send('GET', 'api/multi/view')).json;
  const areaA = areaOf(fullP2.view, 'MA-A');
  const s4A = fullP2.stage4.areas.find((a) => a.areaId === 'MA-A');
  const extA = areaA.extent;
  const liveAll = fullP2.view.areas.flatMap((a) => a.trees).filter((t) => t.isLiving);
  const openPts = [];
  for (const cell of s4A.ground) {
    const x = extA.minXM + cell.centreU;
    const z = extA.minZM + extA.sideM - cell.centreV;
    if (liveAll.every((t) => Math.hypot(t.xm - x, t.zm - z) >= 1.2) &&
        openPts.every((p) => Math.hypot(p.x - x, p.z - z) >= 3.0)) {
      openPts.push({ u: cell.centreU, v: cell.centreV, x, z, cell });
      if (openPts.length === 2) break;
    }
  }
  checkP2(openPts.length === 2 && inPoly(openPts[0].cell.polygon, openPts[0].u, openPts[0].v),
    'found two well-separated open planting points inside MA-A ground cells', openPts.length);
  const pt1 = openPts[0], pt2 = openPts[1];

  // Select a living tree in MA-A, tick 1 draft competitor, enable Light Guide overlay.
  await selectInTable(aTree);
  await waitP2(() => competitorButtons().length > 0, 'competitors for MA-A reference');
  click(competitorButtons()[0]);
  await waitP2(() => byId('add-button').textContent === 'Add selected 1 to Fell plan', '1 draft Fell tick before Plant mode');
  byId('overlay-select').value = 'light';
  byId('overlay-select').dispatchEvent(new FakeEvent('change'));

  // Plant mode: focus on toggle, scrollTop 0, provisional Draft preview (snap OFF, zero request), Escape preserves draft Fell ticks.
  const reqBeforePlant = requests.length;
  click(byId('mode-plant'));
  checkP2(!byId('plant-panel').hidden && byId('tree-panel').hidden &&
    document.activeElement === byId('mode-plant') && byId('pane-tree').scrollTop === 0,
    'Plant mode swaps drawer, keeps focus on mode-plant and does not scroll', [document.activeElement && document.activeElement.id, byId('pane-tree').scrollTop]);
  click(byId('map'), { clientX: pt1.u, clientY: pt1.v });
  byId('plant-opt-shelter').checked = true;
  byId('plant-opt-shelter').dispatchEvent(new FakeEvent('change'));
  byId('plant-opt-spot').checked = true;
  byId('plant-opt-spot').dispatchEvent(new FakeEvent('change'));
  const draftPlant = byId('draft-plant-preview');
  const draftSpotRing = draftPlant && draftPlant.querySelector('[data-draft-spot-ring]');
  checkP2(Boolean(draftPlant) && Boolean(draftSpotRing) && draftSpotRing.getAttribute('r') === '0.564' &&
    !byId('plant-queue').disabled && requests.length === reqBeforePlant,
    'Plant click places provisional Draft preview + 0.564 m ring (snap OFF) with zero HTTP request');
  escape();
  checkP2(byId('plant-panel').hidden && !byId('tree-panel').hidden && !byId('draft-plant-preview') &&
    document.activeElement === byId('mode-tree') &&
    byId('add-button').textContent === 'Add selected 1 to Fell plan' && byId('overlay-select').value === 'light',
    'Escape exits Plant mode to Select Tree (focus on mode-tree), clears draft preview, keeps draft Fell ticks & overlay');

  // Spot Clear mode: provisional seedling-free 0.564 m ring -> Queue Spot Clear (~1 m2).
  const reqBeforeSpot = requests.length;
  click(byId('mode-spot'));
  checkP2(!byId('spot-panel').hidden && byId('tree-panel').hidden &&
    document.activeElement === byId('mode-spot') && byId('pane-tree').scrollTop === 0,
    'Spot Clear mode swaps drawer, keeps focus on mode-spot and does not scroll');
  click(byId('map'), { clientX: pt1.u, clientY: pt1.v });
  const spotPreview = byId('draft-spot-preview');
  const spotPreviewRing = spotPreview && spotPreview.querySelector('[data-draft-spot-ring]');
  checkP2(Boolean(spotPreview) && Boolean(spotPreviewRing) && spotPreviewRing.getAttribute('r') === '0.564' &&
    requests.length === reqBeforeSpot,
    'Spot Clear click places provisional seedling-free 0.564 m ring with zero HTTP request');
  click(byId('spot-queue'));
  await waitP2(() => byId('spot-patches').querySelectorAll('[data-spot-order]').length === 1, 'pending SpotClear queued');
  const preSpotOrderId = byId('spot-patches').querySelector('[data-spot-order]').getAttribute('data-spot-order');
  checkP2(byId('pending-areas').querySelector('[data-planting-remove="' + preSpotOrderId + '"]') !== null,
    'Work Plan lists pending SpotClear with targeted remove button');

  // Sequential fence-chained Plant -> Shelter -> SpotClear hitting step-3 409 Phase2DuplicateOrder:
  // stops immediately, discloses both earlier queued orders with one-click remove buttons, and zero ecology mutates.
  click(byId('mode-plant'));
  click(byId('map'), { clientX: pt1.u, clientY: pt1.v });
  byId('plant-species').value = 'sessile-oak';
  byId('plant-opt-shelter').checked = true;
  byId('plant-opt-spot').checked = true;
  click(byId('plant-queue'));
  await waitP2(() => !byId('plant-result').hidden && byId('plant-result').textContent.includes('Phase2DuplicateOrder'),
    'sequential queue stops on step-3 409 Phase2DuplicateOrder');
  const partialRemBtns = byId('plant-result').querySelectorAll('[data-planting-remove]');
  checkP2(byId('plant-result').classList.contains('partial') && partialRemBtns.length === 2 &&
    alertText().includes('Phase2DuplicateOrder'),
    'step-3 409 discloses exact Phase2DuplicateOrder and offers one-click remove for both earlier pending orders',
    [byId('plant-result').textContent, partialRemBtns.length]);
  // Use one-click Remove buttons to clean up earlier Plant & Shelter, then remove the pre-existing SpotClear.
  click(byId('plant-result').querySelector('[data-planting-remove]'));
  await waitP2(() => byId('plant-result').querySelectorAll('[data-planting-remove]').length === 1, 'first one-click remove');
  click(byId('plant-result').querySelector('[data-planting-remove]'));
  await waitP2(() => byId('plant-result').querySelectorAll('[data-planting-remove]').length === 0, 'second one-click remove');
  click(byId('pending-areas').querySelector('[data-planting-remove="' + preSpotOrderId + '"]'));
  await waitP2(() => byId('pending-total').textContent === 'No pending orders', 'pre-existing SpotClear removed from Work Plan');
  checkP2(byId('pending-total').textContent === 'No pending orders', 'one-click partial recovery and Work Plan remove restored empty plan');

  // Full mixed Pass 2 workflow: Inspect Cell ClearUndergrowth + Plant (Oak) + Shelter + SpotClear -> Approve -> Execute -> live SVG DOM.
  click(byId('mode-cell'));
  click(byId('map'), { clientX: pt1.u, clientY: pt1.v });
  await waitP2(() => !byId('cell-actions').hidden &&
    byId('cell-heading').textContent.includes('cell (' + pt1.cell.column + ', ' + pt1.cell.row + ')') &&
    !byId('cell-clear-add').hidden && !byId('cell-clear-add').disabled, 'Inspect Cell ClearUndergrowth button');
  click(byId('cell-clear-add'));
  await waitP2(() => byId('clear-cells').querySelectorAll('[data-clear-cell]').length === 1 && !byId('cell-clear-remove').hidden,
    'ClearUndergrowth queued from Inspect Cell');
  click(byId('mode-plant'));
  click(byId('map'), { clientX: pt1.u, clientY: pt1.v });
  byId('plant-species').value = 'sessile-oak';
  byId('plant-opt-shelter').checked = true;
  byId('plant-opt-spot').checked = true;
  click(byId('plant-queue'));
  await waitP2(() => byId('pending-areas').querySelectorAll('[data-planting-remove]').length === 3 &&
    byId('pending-areas').querySelectorAll('[data-clear-remove]').length === 1, '4 Pass 2 orders pending');
  const preExecState = (await send('GET', 'api/multi/view')).json;
  checkP2(preExecState.ecology.planted.length === 0 && preExecState.ecology.shelters.length === 0 &&
    preExecState.ecology.patches.length === 0 && !byId('pane-plan').textContent.includes('Expired Shelter'),
    'Pending Pass 2 batch mutates zero ecology and exposes zero Expired Shelter buttons');

  const yearBeforeExec = preExecState.view.source.ecologicalYear;
  click(byId('approve'));
  await waitP2(() => !byId('approved-box').hidden && byId('approved-total').textContent.includes('1 Clear') &&
    byId('approved-total').textContent.includes('1 Plant') && byId('approved-total').textContent.includes('1 Shelter') &&
    byId('approved-total').textContent.includes('1 SpotClear'), 'Approve mixed Pass 2 Work Plan');
  const postApproveState = (await send('GET', 'api/multi/view')).json;
  checkP2(postApproveState.ecology.planted.length === 0 && postApproveState.ecology.shelters.length === 0 &&
    postApproveState.ecology.patches.length === 0, 'Approve Pass 2 batch mutates zero ecology');

  click(byId('execute'));
  await waitP2(() => byId('execute-dialog').open, 'Execute confirmation for Pass 2 batch');
  checkP2(document.activeElement === byId('execute-back') && byId('execute-areas').textContent.includes('ClearUndergrowth') &&
    byId('execute-areas').textContent.includes('SpotClear'), 'Execute confirmation lists Pass 2 orders and focuses Back first');
  click(byId('execute-confirm'));
  await waitP2(() => byId('planted-layer').querySelectorAll('[data-juvenile-id]').length === 1 &&
    byId('planted-layer').querySelectorAll('[data-shelter-id]').length === 1 &&
    byId('spot-patches').querySelectorAll('[data-patch-id]').length === 1, 'live SVG markers after Execute');

  const pjNode = byId('planted-layer').querySelector('[data-juvenile-id]');
  const shNode = byId('planted-layer').querySelector('[data-shelter-id]');
  const patchNode = byId('spot-patches').querySelector('[data-patch-id]');
  const svgChildren = byId('map').childNodes.map((n) => n.id);
  const postExecState = (await send('GET', 'api/multi/view')).json;
  checkP2(postExecState.view.source.ecologicalYear === yearBeforeExec &&
    pjNode.querySelector('use').getAttribute('href') === '#ccf-sapling-oak' &&
    shNode.getAttribute('data-effective') === 'true' && shNode.querySelector('use').getAttribute('href') === '#ccf-shelter-active' &&
    patchNode.getAttribute('href') === '#ccf-spot-clear' && patchNode.getAttribute('data-radius-m') === '0.564' &&
    svgChildren.indexOf('spot-patches') < svgChildren.indexOf('planted-layer'),
    'post-Execute renders Oak PJ sapling, active 8-year shelter, and 0.564 m spot-clear patch beneath planted-layer at zero year advance');

  // Unsheltered Beech PJ sapling -> Select Tree PJ click -> Queue 8-year Shelter -> Remove pending shelter -> Escape preserves Fell draft.
  click(byId('mode-plant'));
  click(byId('map'), { clientX: pt2.u, clientY: pt2.v });
  byId('plant-species').value = 'beech';
  byId('plant-opt-shelter').checked = false;
  byId('plant-opt-spot').checked = false;
  click(byId('plant-queue'));
  await waitP2(() => byId('pending-areas').querySelectorAll('[data-planting-remove]').length === 1, 'unsheltered Beech Plant queued');
  click(byId('approve'));
  await waitP2(() => !byId('approved-box').hidden, 'Approve unsheltered Beech');
  click(byId('execute'));
  await waitP2(() => byId('execute-dialog').open, 'Execute dialog for unsheltered Beech');
  click(byId('execute-confirm'));
  await waitP2(() => byId('planted-layer').querySelectorAll('[data-juvenile-id]').length === 2, 'second PJ sapling rendered');
  click(byId('mode-tree'));
  const beechPjNode = byId('planted-layer').querySelectorAll('[data-juvenile-id]').find((n) => n.getAttribute('data-species') === 'beech');
  click(beechPjNode.querySelector('.pj-hit'), { clientX: pt2.u, clientY: pt2.v });
  await waitP2(() => !byId('juvenile-panel').hidden && byId('tree-panel').hidden && !byId('pj-shelter-add').hidden,
    'clicking unsheltered Beech PJ opens #juvenile-panel with Queue 8-year Shelter');
  click(byId('pj-shelter-add'));
  await waitP2(() => !byId('pj-shelter-remove').hidden && byId('pending-areas').querySelectorAll('[data-planting-remove]').length === 1,
    'Queue 8-year Shelter queued pending shelter for unsheltered PJ');
  click(byId('pj-shelter-remove'));
  await waitP2(() => !byId('pj-shelter-add').hidden && byId('pending-total').textContent === 'No pending orders',
    'Remove pending shelter removed it');
  escape();
  checkP2(byId('juvenile-panel').hidden && !byId('tree-panel').hidden &&
    byId('add-button').textContent === 'Add selected 1 to Fell plan',
    'Escape closes PJ sapling inspection back to adult tree inspector preserving draft Fell ticks');
}

// ---- S2 (Phase 1 Step 2 UI witness: Skip button + WorkPlanOpen refusal + History drawer + Escape preserves Fell draft +
// Structure on-demand load + null -> Unavailable never 0.00 + NB-2 Plant-drawer top #plant-result layout) ----
{
  let s2 = 0;
  const checkS2 = (ok, label, detail) => {
    if (!ok) failures++;
    s2++;
    console.log((ok ? 'PASS ' : 'FAIL ') + 'S2' + String.fromCharCode(96 + s2) + ' ' + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
  };
  const waitS2 = async (predicate, what) => {
    for (const end = Date.now() + 8000; Date.now() < end; await sleep(10)) if (predicate()) return true;
    checkS2(false, 'timed out waiting for ' + what);
    return false;
  };
  const escape = () => document.documentElement.dispatchEvent(new FakeEvent('keydown', { key: 'Escape' }));

  // NB-2 structural check: #plant-result sits directly below #plant-heading at the top of #plant-panel.
  const plantKids = byId('plant-panel').childNodes.filter((n) => n instanceof FakeElement).map((n) => n.id || n.getAttribute('class'));
  checkS2(plantKids.indexOf('plant-result') === plantKids.indexOf('plant-heading') + 1,
    'NB-2: #plant-result sits directly below the Plant heading at the top of #plant-panel', plantKids.slice(0, 4));

  // Draft Fell tick is still 1 from P2; test History drawer open, index load, year detail, Escape preserves draft Fell tick.
  let markS2 = requests.length;
  click(byId('history-open'));
  await waitS2(() => !byId('history-drawer').hidden && byId('history-year-list').querySelectorAll('[data-history-year]').length >= 2,
    'History drawer open and newest-first index loaded');
  checkS2(requests.slice(markS2).some((r) => r.method === 'GET' && r.path === 'api/multi/history/index?pageSize=10') &&
    byId('history-prov-bar').textContent.includes('RECORDED') &&
    byId('history-prov-bar').textContent.includes('NOT RECORDED'),
    'History drawer loads GET api/multi/history/index?pageSize=10 on demand and displays provenance bar');

  click(byId('history-year-list').querySelector('[data-history-year="0"]'));
  await waitS2(() => byId('history-detail-pane').textContent.includes('Work Executed — Year 0') &&
    byId('history-detail-pane').textContent.includes('Observed Outcome — Year 0 → Year 1'),
    'History drawer renders Year 0 Work Executed vs Observed Year 1 Outcome');
  const histText = byId('history-detail-pane').textContent;
  checkS2(histText.includes('not a single-cause effect') && !/weather/i.test(histText) &&
    !/\u20ac|\beuro\b|\brevenue\b/i.test(histText) && histText.includes('NOT RECORDED') &&
    histText.includes('CURRENT') && histText.includes('DERIVED') && histText.includes('SESSION-ONLY'),
    'History Year 0 detail shows separate Work/Outcome cards, provenance pills, NOT RECORDED disclosures, non-causal note without weather, and zero economy',
    { nonCausal: histText.includes('not a single-cause effect'), weather: /weather/i.test(histText), econ: /\u20ac|\beuro\b|\brevenue\b/i.test(histText), notRec: histText.includes('NOT RECORDED'), cur: histText.includes('CURRENT'), der: histText.includes('DERIVED'), sess: histText.includes('SESSION-ONLY'), sample: histText.slice(0, 400) });

  // Escape closes History drawer without clearing draft Fell ticks.
  escape();
  checkS2(byId('history-drawer').hidden && document.activeElement === byId('history-open') &&
    byId('add-button').textContent === 'Add selected 1 to Fell plan',
    'Escape closes History drawer, returns focus to #history-open, and preserves draft Fell ticks');

  // Details / Structure on-demand load + null -> Unavailable (never 0.00).
  markS2 = requests.length;
  click(byId('notice-structure'));
  await waitS2(() => byId('structure-content').textContent.includes('Whole woodland') && byId('structure-content').textContent.includes('0.00'),
    'Structure panel loads GET api/multi/structure on demand');
  checkS2(requests.slice(markS2).some((r) => r.method === 'GET' && r.path === 'api/multi/structure') &&
    byId('structure-disclaimer').textContent.includes('not measured biodiversity, habitat quality, or an Irish forecast'),
    'Structure section displays M_i / T_i, DERIVED provenance, and exact spatial-structure-only disclaimer');

  arm('GET', 'api/multi/structure', 'null-structure');
  click(byId('structure-refresh'));
  await waitS2(() => byId('structure-content').textContent.includes('Unavailable'), 'null structure values to render Unavailable');
  const nullTableText = byId('structure-content').querySelector('table').textContent;
  checkS2(nullTableText.includes('Unavailable') && !nullTableText.includes('0.00'),
    'null M_i / T_i render as Unavailable and never 0.00', nullTableText);

  // Skip to Review: WorkPlanOpen inline refusal without cancelling Pending work, then valid Skip -> Year 4.
  byId('reason-release').checked = false;
  byId('reason').value = 'step2 open work guard';
  byId('reason').dispatchEvent(new FakeEvent('input'));
  byId('add-form').dispatchEvent(new FakeEvent('submit'));
  await waitS2(() => byId('pending-areas').querySelectorAll('[data-remove]').length === 1, 'pending Fell queued before Skip refusal');
  await waitS2(() => byId('skip-review-sub').textContent.includes('Work plan open'), 'Skip button refreshed to Work plan open');
  markS2 = requests.length;
  click(byId('skip-review'));
  await waitS2(() => alertText().includes('WorkPlanOpen'), 'Skip inline WorkPlanOpen refusal');
  checkS2(!requests.slice(markS2).some((r) => r.path === 'api/multi/year/skip') &&
    byId('pending-areas').querySelectorAll('[data-remove]').length === 1 &&
    alertText().includes('GAMEPLAY CALIBRATION'),
    'Skip refuses WorkPlanOpen via preview-review without sending skip or cancelling pending work');

  click(byId('pending-areas').querySelector('[data-remove]'));
  await waitS2(() => byId('pending-total').textContent === 'No pending orders' && !byId('skip-review').classList.contains('blocked'),
    'removing Pending Fell unblocks Skip');
  checkS2(byId('skip-review-label').textContent === 'Skip → Yr 4' && byId('skip-review-sub').textContent.includes('GAMEPLAY CALIBRATION'),
    'Skip label shows anchored target Yr 4 and GAMEPLAY CALIBRATION', [byId('skip-review-label').textContent, byId('skip-review-sub').textContent]);

  click(byId('skip-review'));
  await waitS2(() => byId('year-value').textContent.trim() === '4' && !byId('skip-summary').hidden, 'Skip to advance to Year 4');
  checkS2(byId('skip-summary').textContent.includes('Year 1 → Year 4') &&
    byId('skip-summary').textContent.includes('ReachedReviewYear') &&
    !byId('skip-summary').textContent.includes('null') &&
    byId('skip-summary').textContent.includes('GAMEPLAY CALIBRATION'),
    'Skip advances Year 1 -> Year 4 and displays stepped-years summary with stopReason and GAMEPLAY CALIBRATION', byId('skip-summary').textContent);
}

check(pageErrors.length === 0, 'no script error in the page', pageErrors.map(redact));
process.exit(failures === 0 ? 0 : 1);
WITNESS
witness_rc=0
(cd "$scratch" && "$node_bin" page-witness.mjs "$url") > "$scratch/witness.out" 2> "$scratch/witness.err" || witness_rc=$?
sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' "$scratch/witness.out" | tee -a "$log"
if grep -q '^FAIL' "$scratch/witness.out"; then fail=1; fi
check "J99 the page witness ran to the end (exit $witness_rc) with nothing on stderr" \
  bash -c 'test "$1" == 0 && test ! -s "$2"' _ "$witness_rc" "$scratch/witness.err"
if [[ -s "$scratch/witness.err" ]]; then sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g; s/^/    /' "$scratch/witness.err" | tee -a "$log"; fi

# ---- stop like Ctrl+C, then check that nothing was written --------------------------------------------------------------
stop_server page
check "Z03 no file created, changed or removed anywhere in the lab tree" test "$tree_before" == "$(tree_snapshot)"
check "Z04 git status of both labs unchanged" test "$status_before" == "$(git_status)"

say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit "$fail"
