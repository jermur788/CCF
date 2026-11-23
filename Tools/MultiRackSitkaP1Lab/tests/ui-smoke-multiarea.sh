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
  local run="$1" rc=0
  kill -INT "$server_pid"
  for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$server_pid" 2> /dev/null; then bad "Z01-$run still running 15 s after SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fi
  wait "$server_pid" 2> /dev/null || rc=$?
  server_pid=""
  check "Z01-$run SIGINT ends this script's own process with exit 0 and 'Session ended; nothing was saved.' (exit $rc)" \
    bash -c 'test "$1" == 0 && grep -qx "Session ended; nothing was saved." "$2"' _ "$rc" "$scratch/server-$run.out"
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
print('INFO workflow checks: %d' % count, flush=True)
WORKFLOW
workflow_rc=0
python3 -I -B "$scratch/workflow.py" "$url" > "$scratch/workflow.out" 2> "$scratch/workflow.err" || workflow_rc=$?
cat "$scratch/workflow.out" | tee -a "$log"
check "W999 the HTTP workflow witness completed without stderr" bash -c 'test "$1" == 0 && test ! -s "$2"' _ "$workflow_rc" "$scratch/workflow.err"
if [[ $workflow_rc != 0 ]]; then fail=1; cat "$scratch/workflow.err" | tee -a "$log"; fi
stop_server api

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
  const relative = new URL(path, pageUrl).pathname.replace(/^\/s\/[0-9a-f]{64}\//, '');
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
  return { status: answer.status, json: async () => answer.json };
}
const serverView = async () => (await send('GET', 'api/multi/view')).json.view;
const areaOf = (view, areaId) => view.areas.find((area) => area.areaId === areaId);
const living = (view, areaId, treeId) => areaOf(view, areaId).trees.some((tree) => tree.treeId === treeId && tree.isLiving);

// ---- start the page ----------------------------------------------------------------------------------------------------
const multiUrl = new URL('multi', pageUrl).href;
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
const competitorButtons = () => byId('competition-content').querySelectorAll('[data-competitor]');

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

// Any living tree: competitors appear immediately, before any Crop designation, and nothing changes on the server.
const aTree = areaOf(start, 'MA-A').trees[60].treeId;
await selectInTable(aTree);
await until(() => competitorButtons().length > 0, 'competitors of a non-Crop tree in A');
check(requests.some((r) => r.path === 'api/multi/competition' && r.body.areaId === 'MA-A' && r.body.treeId === aTree) &&
  byId('competition-content').textContent.includes('(not a Crop Tree)') && byId('competition-content').textContent.includes('Current total Hegyi CI'),
  'selecting a non-Crop living tree in A reads its ranked competitors at once');
check(byId('marks').querySelectorAll('.competitor-rank').length === Math.min(5, competitorButtons().length), 'the map numbers the strongest competitors');
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
check(hit && hit.body.areaId === 'MA-B' && hit.body.u === bItem.u && hit.body.v === bItem.v && byId('insp-area').textContent.includes('MA-B'),
  'the map click was hit-tested by the server in Area B and selected ' + bTree);
await until(() => competitorButtons().length > 0, 'competitors of the B tree');
check(byId('competition-content').textContent.includes('in Area B (MA-B)'), 'the B competitors are reported for Area B');
byId('reason-choice').value = 'other';
byId('reason-choice').dispatchEvent(new FakeEvent('change'));
byId('reason').value = 'page witness: typed reason in B';
byId('add-form').dispatchEvent(new FakeEvent('submit'));
await until(() => pendingIds().includes(bTree), 'the B order in the combined plan');

// Back to A: the selection is remembered; retain a Crop, then propose its strongest competitor with the release reason.
click(byId('area-switch').querySelector('[data-area="MA-A"]'));
await until(() => byId('insp-id').textContent === aTree && competitorButtons().length > 0, 'the remembered A selection with competitors');
click(byId('crop-button'));
await until(() => byId('crop-button').textContent === 'Remove Crop designation', 'the Crop designation');
await until(() => competitorButtons().length > 0 && byId('competition-content').textContent.includes('Retained Crop Tree ' + aTree), 'the Crop competitors');
const competitorId = competitorButtons()[0].getAttribute('data-competitor');
click(competitorButtons()[0]);
await until(() => byId('insp-id').textContent === competitorId, 'the listed competitor selected');
check(!byId('reason-release').disabled && byId('reason-release').textContent === 'Competition release for retained Crop Tree ' + aTree + ' (operator intent)',
  'the release reason is offered only for a listed competitor of the inspected Crop Tree');
byId('reason-choice').value = 'release';
byId('reason-choice').dispatchEvent(new FakeEvent('change'));
byId('add-form').dispatchEvent(new FakeEvent('submit'));
await until(() => pendingIds().includes(competitorId), 'the A order in the combined plan');
check(byId('pending-total').textContent === 'Total pending: 2 (Area A 1 · Area B 1)', 'one Work Plan lists both area sections and the counts',
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
check(server.source.ecologicalYear === 1 && byId('year-latest').textContent.includes('A · MA-A') && byId('year-latest').textContent.includes('B · MA-B') &&
  byId('year-latest').textContent.includes('170 →') && byId('year-latest').textContent.includes('72 →'), 'Year+1 shows each area\'s own result');

// Fail closed: an Add whose answer is lost locks every command until a re-read.
const shown = byId('pending-total').textContent;
arm('POST', 'api/multi/pending/add', 'lose');
const lostTree = areaOf(server, 'MA-A').trees.find((t) => t.isLiving && t.treeId !== aTree).treeId;
await selectInTable(lostTree);
byId('reason-choice').value = 'other';
byId('reason-choice').dispatchEvent(new FakeEvent('change'));
byId('reason').value = 'page witness: lost answer';
byId('add-form').dispatchEvent(new FakeEvent('submit'));
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
