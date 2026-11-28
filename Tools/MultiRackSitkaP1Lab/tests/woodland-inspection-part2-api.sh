#!/usr/bin/env bash
# Woodland Inspection Part 2 (spec multi-2d-woodland-inspection-v1): a bash + curl + python3 witness of the /multi
# real-woodland survey inspection served by the BUILT UI process. It starts one fresh process against a scratch copy of
# the committed P1 fixture on an OS-assigned 127.0.0.1 port and checks:
#   - C: the offline built-in catalogue lists Donadea first and Mersch second, CC BY 4.0, DOI, neither can advance time;
#   - L: both plots load into read-only Year-0 inspection: 20 trees, exact Part 1 golden view SHA-256, honest Missing
#     fields (crown radius, age), no Simulated value, no inferred boundary, all four species categories, TreMs records,
#     full attribution and disclosures;
#   - R: while inspecting, Advance, Skip, Work Plan approve/execute/cancel, Save, Money switch and Work Plan edits are
#     refused with HTTP 409 WoodlandInspectionReadOnly and nothing changes; read-only routes still answer;
#   - X: Return to normal woodland clears the inspection and Advance works again; New woodland and Load save both clear
#     any inspection; exit is idempotent; malformed and unknown plot requests are refused;
#   - K: commands admitted before a survey opens (request body held back on a raw socket until after the survey opened)
#     are refused at the commit-time recheck (checkpoint "commit"): multi Advance, Skip, Save, Recover and single-area
#     Advance; nothing changes; a control proves the same paused request commits when no survey opens;
#   - O: 50 overlapping Donadea/Mersch load pairs never return a reply whose current survey and status disagree, and the
#     served view bytes always hash to that plot's Part 1 golden;
#   - S: 30 simultaneous Save + Recover pairs on one slot all answer within a timeout (one lock order: host, then slot),
#     and a blocked Save to an existing slot is refused for the survey before any SlotExists check;
#   - M: successful and refused saves report the correct live progress marker, including a newer same-slot save;
#   - SIGINT ends the process cleanly; no file appears in the lab tree; git status of the lab is unchanged.
#
# Usage: tests/woodland-inspection-part2-api.sh [<evidence-dir>]
#   Build first: ~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj   (DOTNET overrides the dotnet)
#   TMPDIR and <evidence-dir> must be outside the repository. Exit 0 only if every check passes.
set -euo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
golden_donadea="b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6"
golden_mersch="e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4"
evidence="${1:-}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
command -v curl > /dev/null && command -v python3 > /dev/null || { echo "STOP: curl and python3 are required" >&2; exit 2; }
if [[ -n "$evidence" ]]; then
  mkdir -p -- "$evidence"
  case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: the evidence directory is inside the repository" >&2; exit 2 ;; esac
fi

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-insp2-api.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
log="$scratch/woodland-inspection-part2-api.txt"
: > "$log"
server_pid=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2> /dev/null; then kill -KILL "$server_pid" 2> /dev/null || true; fi
  if [[ -n "$evidence" ]]; then cp -- "$log" "$evidence/woodland-inspection-part2-api.txt" 2> /dev/null || true; fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

passes=0
fails=0
say() { echo "$*" | tee -a "$log"; }
check() {
  local label="$1"; shift
  if "$@"; then say "PASS $label"; passes=$((passes + 1)); else say "FAIL $label"; fails=$((fails + 1)); fi
}

tree_snapshot() { (cd "$p1_root" && find . -path ./src/Ui/bin -prune -o -path ./src/Ui/obj -prune -o -path ./src/Core/bin -prune -o -path ./src/Core/obj -prune -o -printf '%y %p %s\n' | LC_ALL=C sort); }
git_status() { git -C "$repo" status --porcelain --untracked-files=all -- Tools/MultiRackSitkaP1Lab; }
tree_before="$(tree_snapshot)"; status_before="$(git_status)"
say "woodland-inspection-part2-api $(date -u +%FT%TZ) HEAD $(git -C "$repo" rev-parse HEAD) dotnet $("$dotnet_bin" --version)"

mkdir "$scratch/fixture" "$scratch/xdg"
cat -- "$p1_root/fixtures/derived/$fixture_name" > "$scratch/fixture/$fixture_name"

# Job control gives the process its own process group with the default SIGINT action, as in an interactive terminal.
set -m
( cd "$p1_root" && exec env XDG_DATA_HOME="$scratch/xdg" "$dotnet_bin" "$ui_dll" "$scratch/fixture" ) \
  > "$scratch/server.out" 2> "$scratch/server.err" &
server_pid=$!
set +m
url=""
for _ in $(seq 1 300); do
  url="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$scratch/server.out" | head -1 || true)"
  [[ -n "$url" ]] && break
  kill -0 "$server_pid" 2> /dev/null || break
  sleep 0.1
done
if [[ -z "$url" ]]; then
  say "FAIL S01 the process printed no launch URL"; sed 's/^/    /' "$scratch/server.err" | tee -a "$log"; exit 1
fi
port="${url#http://127.0.0.1:}"; port="${port%%/*}"
cap="${url#*/s/}"; cap="${cap%/}"
origin="http://127.0.0.1:$port"
api="$origin/s/$cap/api/multi"
say "server pid $server_pid, port $port"

# get <name> <path> / post <name> <path> <json-body> : body goes to $scratch/<name>.json; prints the HTTP status.
get() { curl --silent --noproxy '*' --max-time 30 -o "$scratch/$1.json" -w '%{http_code}' "$api/$2" || true; }
post() {
  curl --silent --noproxy '*' --max-time 30 -o "$scratch/$1.json" -w '%{http_code}' -X POST \
    -H 'Content-Type: application/json' -H "Origin: $origin" --data-binary "$3" "$api/$2" || true
}
# py <name> <python-expression over d> : true when the expression holds for the JSON body of <name>.
py() { python3 - "$scratch/$1.json" "$2" << 'PY'
import json, sys
d = json.load(open(sys.argv[1], encoding='utf-8'))
sys.exit(0 if eval(sys.argv[2], {'d': d, 'json': json}) else 1)
PY
}
fence() { python3 -c 'import json,sys; v=json.load(open(sys.argv[1]))["view"]; print(json.dumps({"sessionId": v["sessionId"], "source": v["source"], "planStateRevision": v["planStateRevision"]}))' "$scratch/$1.json"; }
year_of() { python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["view"]["source"]["ecologicalYear"])' "$scratch/$1.json"; }
# Concurrency and byte-level helpers (stdlib only), written into the scratch directory.
cat > "$scratch/insp_race.py" << 'PY'
import hashlib, http.client, json, socket, sys, threading, time

def call(port, path, body=None, timeout=120):
    conn = http.client.HTTPConnection('127.0.0.1', port, timeout=timeout)
    headers = {} if body is None else {'Content-Type': 'application/json', 'Origin': 'http://127.0.0.1:%d' % port}
    conn.request('GET' if body is None else 'POST', path, body=body, headers=headers)
    response = conn.getresponse()
    data = response.read()
    conn.close()
    return response.status, data.decode('utf-8')

def view_bytes_sha(text):
    # SHA-256 of the exact "view" bytes as served (the value that follows current.entry), not of a re-serialisation.
    dec = json.JSONDecoder()
    key = '"current":{"entry":'
    i = text.index(key) + len(key)
    _, end = dec.raw_decode(text, i)
    sep = ',"view":'
    if not text.startswith(sep, end):
        raise ValueError('view does not follow entry')
    start = end + len(sep)
    _, vend = dec.raw_decode(text, start)
    return hashlib.sha256(text[start:vend].encode('utf-8')).hexdigest()

mode = sys.argv[1]
if mode == 'viewsha':
    print(view_bytes_sha(open(sys.argv[2], encoding='utf-8').read()))
elif mode == 'paused':
    # paused <port> <base> <route> <body> <plotId|none> <out>: send headers only, open a survey, then release the body.
    port, base, route, body, plot, out = int(sys.argv[2]), sys.argv[3], sys.argv[4], sys.argv[5].encode('utf-8'), sys.argv[6], sys.argv[7]
    sock = socket.create_connection(('127.0.0.1', port), timeout=120)
    sock.sendall(('POST %s%s HTTP/1.1\r\nHost: 127.0.0.1:%d\r\nOrigin: http://127.0.0.1:%d\r\nContent-Type: application/json\r\n'
                  'Content-Length: %d\r\nConnection: close\r\n\r\n' % (base, route, port, port, len(body))).encode('ascii'))
    time.sleep(0.8)  # the request has passed admission (no survey open) and its handler is waiting for the body
    load_status, loaded_plot = None, None
    if plot != 'none':
        load_status, text = call(port, base + 'api/multi/woodland/inspection/load', json.dumps({'plotId': plot}))
        loaded_plot = json.loads(text)['inspection']['plotId']
    sock.sendall(body)
    raw = b''
    while True:
        chunk = sock.recv(65536)
        if not chunk:
            break
        raw += chunk
    sock.close()
    head, _, payload = raw.partition(b'\r\n\r\n')
    status = int(head.split(b' ')[1])
    json.dump({'loadStatus': load_status, 'loadedPlot': loaded_plot, 'status': status, 'body': json.loads(payload)}, open(out, 'w'))
elif mode == 'overlap':
    # overlap <port> <base> <pairs> <golden-donadea> <golden-mersch> <out>: opposite plot loads released together.
    port, base, pairs, out = int(sys.argv[2]), sys.argv[3], int(sys.argv[4]), sys.argv[7]
    golden = {'IPlus_Donadea_MHF_2': sys.argv[5], 'IPlus_Mersch': sys.argv[6]}
    results = []
    for _ in range(pairs):
        barrier = threading.Barrier(2)
        def one(plot):
            barrier.wait()
            results.append((plot, call(port, base + 'api/multi/woodland/inspection/load', json.dumps({'plotId': plot}))))
        threads = [threading.Thread(target=one, args=(p,)) for p in golden]
        for t in threads: t.start()
        for t in threads: t.join()
    ok = contradictory = mismatched = 0
    for asked, (status, text) in results:
        if status != 200:
            continue
        ok += 1
        d = json.loads(text)
        cur, ins = d['current']['entry']['plotId'], d['inspection']['plotId']
        if cur != ins or cur != asked:
            contradictory += 1
        if d['current']['viewSha256'] != golden[cur] or view_bytes_sha(text) != golden[cur]:
            mismatched += 1
    json.dump({'responses': len(results), 'ok200': ok, 'contradictory': contradictory, 'hashMismatched': mismatched}, open(out, 'w'))
elif mode == 'saverecover':
    # saverecover <port> <base> <fence-json> <slot> <pairs> <out>: a Save and a Recover of one slot released together, each
    # bounded by a 20 s timeout, so a lock-order deadlock shows up as a timeout instead of a hung test.
    port, base, fence, slot, pairs, out = int(sys.argv[2]), sys.argv[3], json.loads(sys.argv[4]), sys.argv[5], int(sys.argv[6]), sys.argv[7]
    statuses = {'save': [], 'recover': []}
    timeouts = 0
    for _ in range(pairs):
        barrier = threading.Barrier(2)
        def one(kind):
            global timeouts
            body = json.dumps({'fence': fence, 'slot': slot, 'replaceExisting': True}) if kind == 'save' else json.dumps({'slot': slot})
            barrier.wait()
            try:
                status, _ = call(port, base + ('api/multi/save' if kind == 'save' else 'api/multi/saves/recover'), body, timeout=20)
                statuses[kind].append(status)
            except (socket.timeout, TimeoutError):
                timeouts += 1
        threads = [threading.Thread(target=one, args=(k,)) for k in ('save', 'recover')]
        for t in threads: t.start()
        for t in threads: t.join()
        if timeouts:
            break  # a deadlocked server never recovers; one timeout is the evidence
    json.dump({'pairs': pairs, 'timeouts': timeouts, 'save': sorted(set(statuses['save'])), 'saveCount': len(statuses['save']),
               'recover': sorted(set(statuses['recover'])), 'recoverCount': len(statuses['recover'])}, open(out, 'w'))
PY
race() { python3 "$scratch/insp_race.py" "$@"; }

read_only_refusal() { python3 - "$scratch/$1.json" << 'PY'
import json, sys
d = json.load(open(sys.argv[1], encoding='utf-8'))
ok = (d.get('error', {}).get('code') == 'WoodlandInspectionReadOnly' and d.get('refusal', {}).get('code') == 'WoodlandInspectionReadOnly'
      and d['inspection']['active'] is True and d['inspection']['canAdvanceTime'] is False
      and 'nothing changed' in d['refusal']['message'] and 'api/' not in d['refusal']['message']
      and d.get('checkpoint') in ('admission', 'commit'))
sys.exit(0 if ok else 1)
PY
}

# ---- C: catalogue ----------------------------------------------------------------------------------------------------
code="$(get cat woodland/inspection/catalogue)"
check "C01 GET catalogue: 200" test "$code" == 200
check "C02 catalogue order is Donadea (Ireland) first, Mersch (Luxembourg) second" py cat \
  "[e['plotId'] for e in d['entries']] == ['IPlus_Donadea_MHF_2', 'IPlus_Mersch'] and [e['countryCode'] for e in d['entries']] == ['IE', 'LU']"
check "C03 both entries: 20 trees, CC BY 4.0, DOI 10.15468/ocof3v, citation, InventoryOnly, cannot advance time" py cat \
  "all(e['treeCount'] == 20 and e['licenceSummary'] == 'CC BY 4.0' and e['doi'] == '10.15468/ocof3v' and e['doiUrl'] == 'https://doi.org/10.15468/ocof3v' and len(e['citation']) > 100 and e['readiness'] == 'InventoryOnly' and e['canAdvanceTime'] is False and e['readinessLabel'] for e in d['entries'])"
check "C04 subset text, observation dates, modifications disclosure and caveats are present" py cat \
  "[e['subsetOfGbifRecords'] for e in d['entries']] == ['20 of 271 GBIF records for this plot', '20 of 134 GBIF records for this plot'] and all(e['observationDates'] and e['modificationsDisclosure'] and len(e['caveats']) >= 2 for e in d['entries'])"
check "C05 no inspection is active at start" py cat "d['inspection']['active'] is False and d['refusal'] is None"
code="$(get cur0 woodland/inspection/current)"
check "C06 GET current at start: 200, inactive, no survey" bash -c 'test "$1" == 200' _ "$code"
check "C07 current payload says inactive" py cur0 "d['inspection']['active'] is False and d['current'] is None"
code="$(get catq 'woodland/inspection/catalogue?x=1')"
check "C08 catalogue with a query string: 400" test "$code" == 400

code="$(get view0 view)"
check "C09 GET view before inspecting: 200" test "$code" == 200
year0="$(year_of view0)"

# ---- L: load both plots ------------------------------------------------------------------------------------------------
common_view_checks() {
  local name="$1" plot="$2" golden="$3"
  check "L-$plot load: 200 with the plot active and read-only" py "$name" \
    "d['refusal'] is None and d['inspection'] == {'active': True, 'plotId': '$plot', 'displayName': d['current']['entry']['displayName'], 'canAdvanceTime': False}"
  check "L-$plot the declared view hash is the Part 1 golden SHA-256 $golden" py "$name" "d['current']['viewSha256'] == '$golden'"
  check "L-$plot the served view BYTES hash to the Part 1 golden (exact canonical JSON, not a re-serialisation)" \
    test "$(race viewsha "$scratch/$name.json")" == "$golden"
  check "L-$plot 20 Year-0 trees, all plotted, summary agrees" py "$name" \
    "len(d['current']['view']['observedYear0']['trees']) == 20 and d['current']['summary'] == {'treeCount': 20, 'plottedTreeCount': 20, 'unplottedTreeCount': 0}"
  check "L-$plot readiness InventoryOnly, cannot advance time, no simulated future, Year 0 only" py "$name" \
    "(lambda v: v['readiness'] == 'InventoryOnly' and v['canAdvanceTime'] is False and v['provenanceBoundary']['observedStateYear'] == 0 and v['provenanceBoundary']['simulatedFutureAvailable'] is False and v['provenanceBoundary']['simulatedValueCount'] == 0)(d['current']['view'])"
  check "L-$plot crown radius and age are honestly Missing with null values on every tree (never invented)" py "$name" \
    "all(t[f]['provenance'] == 'Missing' and t[f]['value'] is None for t in d['current']['view']['observedYear0']['trees'] for f in ('crownRadiusM', 'ageYears'))"
  check "L-$plot no field anywhere carries Simulated provenance; every Missing field has a null value" py "$name" \
    "all(t[f]['provenance'] != 'Simulated' and ((t[f]['provenance'] == 'Missing') == (t[f]['value'] is None)) for t in d['current']['view']['observedYear0']['trees'] for f in ('speciesName','vitalStatus','positionLocalM','dbhCm','heightM','crownRadiusM','ageYears'))"
  check "L-$plot no boundary is inferred: observed false, no vertices, no area" py "$name" \
    "(lambda b: b['observed'] is False and b['verticesLocalM'] == [] and b['areaM2'] is None and b['areaHa'] is None)(d['current']['view']['observedYear0']['boundary'])"
  check "L-$plot species breakdown lists all four categories in order, with per-raw-name rows summing to 20" py "$name" \
    "(lambda s: [c['category'] for c in s['categories']] == ['ModelledAtBase','PendingModelSupport','UnmodelledSpecies','UnidentifiedSpecies'] and sum(r['totalTrees'] for r in s['species']) == 20 and sum(c['totalTrees'] for c in s['categories']) == 20)(d['current']['view']['speciesBreakdown'])"
  check "L-$plot CC BY 4.0 attribution: licence, citation, DOI, observation dates, disclosures" py "$name" \
    "(lambda v: v['attribution']['licence'].startswith('CC BY 4.0 (') and v['attribution']['doi'] == '10.15468/ocof3v' and len(v['attribution']['citation']) > 100 and v['attribution']['observationDates'] and v['disclosures']['modificationsDisclosure'] and v['disclosures']['coordinateInterpretationCaveat'].startswith('Note:'))(d['current']['view'])"
  check "L-$plot blocker checklist names the missing boundary in plain English" py "$name" \
    "any(b['code'] == 'BoundaryMissing' and b['title'] == 'No boundary supplied' and b['explanation'] for b in d['current']['view']['blockerChecklist'])"
  check "L-$plot display labels cover every category, provenance and the Not recorded word" py "$name" \
    "(lambda l: set(l['speciesCategories']) == {'ModelledAtBase','PendingModelSupport','UnmodelledSpecies','UnidentifiedSpecies'} and l['provenance']['Missing'] and l['notRecorded'] == 'Not recorded' and l['bannerTitle'] == 'Survey inspection mode — Year 0')(d['current']['labels'])"
}

code="$(post don woodland/inspection/load '{"plotId":"IPlus_Donadea_MHF_2"}')"
check "L01 POST load Donadea: 200" test "$code" == 200
common_view_checks don IPlus_Donadea_MHF_2 "$golden_donadea"
check "L02 Donadea: unmodelled living species are a blocker and TreMs microhabitats are present" py don \
  "any(b['code'] == 'UnsupportedLivingSpecies' and b['affectedTreeCount'] > 0 for b in d['current']['view']['blockerChecklist']) and len(d['current']['view']['microhabitats']) > 0"
code="$(get cur1 woodland/inspection/current)"
check "L03 GET current after load: the same Donadea survey" bash -c 'test "$1" == 200' _ "$code"
check "L04 current view hash equals the loaded one" py cur1 "d['inspection']['plotId'] == 'IPlus_Donadea_MHF_2' and d['current']['viewSha256'] == '$golden_donadea'"

# ---- R: refusals while inspecting (fence is the live, valid one so only the inspection guard can refuse) -------------
f="$(fence view0)"
refuse() {
  local label="$1" path="$2" body="$3" code
  code="$(post "r-$label" "$path" "$body")"
  check "R-$label POST $path while inspecting: 409 WoodlandInspectionReadOnly in plain English" \
    bash -c 'test "$1" == 409' _ "$code"
  check "R-$label typed refusal body (error + refusal code, inspection status, no route text)" read_only_refusal "r-$label"
}
refuse advance year/advance "{\"fence\":$f,\"consentToCancelWork\":false}"
refuse advance-consent year/advance "{\"fence\":$f,\"consentToCancelWork\":true}"
refuse skip year/skip "{\"fence\":$f,\"targetYear\":$((year0 + 4))}"
refuse approve plan/approve "{\"fence\":$f}"
refuse execute plan/execute "{\"fence\":$f,\"planId\":\"p\",\"executionKey\":\"k\"}"
refuse cancel plan/cancel "{\"fence\":$f}"
refuse save save "{\"fence\":$f,\"slot\":\"insp-blocked\",\"replaceExisting\":false}"
refuse money economy/session "{\"fence\":$f,\"enabled\":true,\"confirmReset\":true}"
refuse crop crop/mark "{\"fence\":$f,\"areaId\":\"A\",\"treeId\":\"A-001\"}"
refuse prune prune/add "{\"fence\":$f,\"areaId\":\"A\",\"treeId\":\"A-001\"}"
refuse plant plant/add "{\"fence\":$f,\"areaId\":\"A\",\"speciesId\":\"sitka-spruce\",\"xM\":1,\"zM\":1}"
refuse malformed-advance year/advance 'not json'
code="$(get view1 view)"
check "R01 the playable woodland is untouched: same year, session and revisions" bash -c \
  'test "$1" == 200 && python3 -c "import json,sys; a=json.load(open(sys.argv[1]))[\"view\"]; b=json.load(open(sys.argv[2]))[\"view\"]; sys.exit(0 if (a[\"sessionId\"],a[\"source\"],a[\"planStateRevision\"])==(b[\"sessionId\"],b[\"source\"],b[\"planStateRevision\"]) else 1)" "$2" "$3"' \
  _ "$code" "$scratch/view0.json" "$scratch/view1.json"
code="$(post preview year/preview-review "{\"fence\":$f}")"
check "R02 a read-only POST (review preview) still answers while inspecting (not the inspection refusal)" bash -c \
  'test "$1" == 200 && ! grep -q WoodlandInspectionReadOnly "$2"' _ "$code" "$scratch/preview.json"
code="$(get saves0 saves)"
check "R03 GET saves still answers while inspecting (reads are never blocked)" test "$code" == 200

# ---- L (Mersch) and request validation -------------------------------------------------------------------------------
code="$(post mer woodland/inspection/load '{"plotId":"IPlus_Mersch"}')"
check "L05 POST load Mersch while Donadea is open: 200 (switches plot)" test "$code" == 200
common_view_checks mer IPlus_Mersch "$golden_mersch"
refuse advance-mersch year/advance "{\"fence\":$f,\"consentToCancelWork\":false}"
code="$(post unknown woodland/inspection/load '{"plotId":"Not_A_Plot"}')"
check "V01 unknown plot: 409 WoodlandInspectionPlotUnknown, Mersch stays active" bash -c \
  'test "$1" == 409 && python3 -c "import json,sys; d=json.load(open(sys.argv[1])); sys.exit(0 if d[\"refusal\"][\"code\"]==\"WoodlandInspectionPlotUnknown\" and d[\"inspection\"][\"plotId\"]==\"IPlus_Mersch\" else 1)" "$2"' \
  _ "$code" "$scratch/unknown.json"
code="$(post extra woodland/inspection/load '{"plotId":"IPlus_Mersch","path":"/etc/passwd"}')"
check "V02 unknown member (no path/upload accepted): 400" test "$code" == 400
code="$(post missing woodland/inspection/load '{}')"
check "V03 missing plotId: 400" test "$code" == 400
code="$(curl --silent --noproxy '*' -o /dev/null -w '%{http_code}' -X POST -H 'Content-Type: application/json' \
  -H 'Origin: http://evil.example' --data-binary '{"plotId":"IPlus_Mersch"}' "$api/woodland/inspection/load" || true)"
check "V04 load from a foreign Origin: 403 (existing guards still apply)" test "$code" == 403

# ---- X: return to normal woodland, New woodland and Load both clear the inspection -----------------------------------
code="$(post exit1 woodland/inspection/exit '{}')"
check "X01 POST exit: 200, was active, now inactive" bash -c 'test "$1" == 200' _ "$code"
check "X02 exit payload" py exit1 "d['wasActive'] is True and d['inspection']['active'] is False"
code="$(get cur2 woodland/inspection/current)"
check "X03 current after exit: inactive (no stale inspection)" py cur2 "d['inspection']['active'] is False and d['current'] is None"
code="$(post adv1 year/advance "{\"fence\":$f,\"consentToCancelWork\":false}")"
check "X04 Advance after Return to normal woodland: 200 and the year moves on by one" bash -c \
  'test "$1" == 200 && python3 -c "import json,sys; d=json.load(open(sys.argv[1])); sys.exit(0 if d[\"view\"][\"source\"][\"ecologicalYear\"]==int(sys.argv[2])+1 else 1)" "$2" "$3"' \
  _ "$code" "$scratch/adv1.json" "$year0"
code="$(post exit2 woodland/inspection/exit '{}')"
check "X05 exit again is idempotent: 200, was not active" bash -c 'test "$1" == 200 && grep -q "\"wasActive\":false" "$2"' _ "$code" "$scratch/exit2.json"

code="$(get view2 view)"; f2="$(fence view2)"
code="$(post save1 save "{\"fence\":$f2,\"slot\":\"insp-part2\",\"replaceExisting\":false}")"
check "X06 Save while not inspecting: 200 (saves work in normal play)" test "$code" == 200
code="$(post don2 woodland/inspection/load '{"plotId":"IPlus_Donadea_MHF_2"}')"
check "X07 open Donadea again: 200" test "$code" == 200
code="$(get view3 view)"; f3="$(fence view3)"
code="$(post load1 load "{\"fence\":$f3,\"slot\":\"insp-part2\",\"confirmDiscardUnsaved\":true}")"
check "X08 Load save while inspecting: 200 (allowed)" test "$code" == 200
code="$(get cur3 woodland/inspection/current)"
check "X09 the committed Load ended the inspection" py cur3 "d['inspection']['active'] is False and d['current'] is None"

code="$(post don3 woodland/inspection/load '{"plotId":"IPlus_Donadea_MHF_2"}')"
check "X10 open Donadea a third time: 200" test "$code" == 200
code="$(get view4 view)"; f4="$(fence view4)"
code="$(post new1 woodland/new "{\"fence\":$f4,\"choice\":\"ma-1\",\"presetId\":null,\"recipe\":null,\"economyEnabled\":false,\"confirmDiscardUnsaved\":true}")"
check "X11 New woodland while inspecting: 200 (allowed)" test "$code" == 200
code="$(get cur4 woodland/inspection/current)"
check "X12 the committed New woodland ended the inspection" py cur4 "d['inspection']['active'] is False and d['current'] is None"
f5="$(fence new1)"; y5="$(year_of new1)"
code="$(post adv2 year/advance "{\"fence\":$f5,\"consentToCancelWork\":false}")"
check "X13 Advance in the new woodland: 200 and year $((y5 + 1))" bash -c \
  'test "$1" == 200 && python3 -c "import json,sys; d=json.load(open(sys.argv[1])); sys.exit(0 if d[\"view\"][\"source\"][\"ecologicalYear\"]==int(sys.argv[2])+1 else 1)" "$2" "$3"' \
  _ "$code" "$scratch/adv2.json" "$y5"

# A refused New woodland (stale fence) changes nothing, so the inspection it was sent over stays active.
code="$(post don4 woodland/inspection/load '{"plotId":"IPlus_Donadea_MHF_2"}')"
code="$(post new2 woodland/new "{\"fence\":$f4,\"choice\":\"ma-1\",\"presetId\":null,\"recipe\":null,\"economyEnabled\":false,\"confirmDiscardUnsaved\":true}")"
check "X14 New woodland with a stale fence: 409" test "$code" == 409
code="$(get cur5 woodland/inspection/current)"
check "X15 the refused New woodland changed nothing: the inspection stays active" py cur5 \
  "d['inspection']['active'] is True and d['inspection']['plotId'] == 'IPlus_Donadea_MHF_2'"
code="$(post exit3 woodland/inspection/exit '{}')"
check "X16 final exit: 200" test "$code" == 200
check "R04 a refusal made while a survey was already open is labelled as the admission check" py r-advance "d['checkpoint'] == 'admission'"

# ---- K: commands admitted BEFORE a survey opens are refused at commit time (paused request bodies) ------------------
# Each case sends only the headers of a valid command while no survey is open (so it passes admission), opens a survey,
# and only then sends the body. checkpoint == "commit" proves the request got past admission and was stopped by the
# in-lock recheck, so this exercises the reviewer's race rather than an ordinary sequential refusal.
base="/s/$cap/"
same_fence() { python3 -c 'import json,sys; a=json.load(open(sys.argv[1]))["view"]; b=json.load(open(sys.argv[2]))["view"]; sys.exit(0 if (a["sessionId"],a["source"],a["planStateRevision"])==(b["sessionId"],b["source"],b["planStateRevision"]) else 1)' "$scratch/$1.json" "$scratch/$2.json"; }
commit_refusal() { python3 -c 'import json,sys; d=json.load(open(sys.argv[1])); b=d["body"]; sys.exit(0 if d["loadStatus"]==200 and d["status"]==409 and b["refusal"]["code"]=="WoodlandInspectionReadOnly" and b["error"]["code"]=="WoodlandInspectionReadOnly" and b["checkpoint"]=="commit" and b["inspection"]["active"] else 1)' "$scratch/$1.json"; }

code="$(get k-before view)"; fk="$(fence k-before)"; yk="$(year_of k-before)"
race paused "$port" "$base" api/multi/year/advance "{\"fence\":$fk,\"consentToCancelWork\":false}" IPlus_Donadea_MHF_2 "$scratch/k1.json"
check "K01 Advance admitted before Donadea opened, body sent after: 409 WoodlandInspectionReadOnly at the commit check" commit_refusal k1
code="$(get k-after1 view)"
check "K02 the playable woodland did not change: same year ($yk), session and revisions" same_fence k-before k-after1
code="$(get k-cur1 woodland/inspection/current)"
check "K03 the survey stayed open (Donadea)" py k-cur1 "d['inspection']['active'] is True and d['inspection']['plotId'] == 'IPlus_Donadea_MHF_2'"
code="$(post k-exit1 woodland/inspection/exit '{}')"

race paused "$port" "$base" api/multi/year/skip "{\"fence\":$fk,\"targetYear\":$((yk + 2))}" IPlus_Mersch "$scratch/k2.json"
check "K04 Skip admitted before Mersch opened: 409 at the commit check" commit_refusal k2
code="$(get k-after2 view)"
check "K05 Skip changed nothing" same_fence k-before k-after2
code="$(post k-exit2 woodland/inspection/exit '{}')"

race paused "$port" "$base" api/multi/save "{\"fence\":$fk,\"slot\":\"insp-race\",\"replaceExisting\":false}" IPlus_Donadea_MHF_2 "$scratch/k3.json"
check "K06 Save admitted before a survey opened: 409 at the commit check" commit_refusal k3
code="$(get k-saves saves)"
check "K07 no save slot was written by the refused Save" py k-saves "all(s['slot'] != 'insp-race' for s in d['slots'])"
code="$(post k-exit3 woodland/inspection/exit '{}')"

race paused "$port" "$base" api/multi/saves/recover '{"slot":"insp-part2"}' IPlus_Mersch "$scratch/k4.json"
check "K08 save Recover admitted before a survey opened: 409 at the commit check (before any slot file is touched)" commit_refusal k4
code="$(post k-exit4 woodland/inspection/exit '{}')"

curl --silent --noproxy '*' --max-time 30 -o "$scratch/k-sa-before.json" "$origin/s/$cap/api/view"
fsa="$(fence k-sa-before)"
race paused "$port" "$base" api/year/advance "{\"fence\":$fsa,\"consentToCancelWork\":false}" IPlus_Donadea_MHF_2 "$scratch/k5.json"
check "K09 single-area Advance admitted before a survey opened: 409 at the commit check" commit_refusal k5
curl --silent --noproxy '*' --max-time 30 -o "$scratch/k-sa-after.json" "$origin/s/$cap/api/view"
check "K10 the single-area session did not change" same_fence k-sa-before k-sa-after
code="$(post k-exit5 woodland/inspection/exit '{}')"

race paused "$port" "$base" api/multi/year/advance "{\"fence\":$fk,\"consentToCancelWork\":false}" none "$scratch/k6.json"
check "K11 control: the same paused Advance with no survey opened commits (200, year $yk -> $((yk + 1)))" python3 -c \
  'import json,sys; d=json.load(open(sys.argv[1])); sys.exit(0 if d["status"]==200 and d["body"]["view"]["source"]["ecologicalYear"]==int(sys.argv[2])+1 else 1)' \
  "$scratch/k6.json" "$yk"

# ---- O: overlapping loads of opposite plots never return a contradictory reply -----------------------------------------
race overlap "$port" "$base" 50 "$golden_donadea" "$golden_mersch" "$scratch/o1.json"
check "O01 50 overlapping Donadea/Mersch load pairs: 100 replies, all 200" py o1 "d['responses'] == 100 and d['ok200'] == 100"
check "O02 every reply's current survey, inspection status and requested plot agree (0 contradictory)" py o1 "d['contradictory'] == 0"
check "O03 every reply's declared and actual view-byte SHA-256 match that plot's golden (0 mismatched)" py o1 "d['hashMismatched'] == 0"
code="$(get o-cur woodland/inspection/current)"
check "O04 after the overlap, GET current is self-consistent" py o-cur \
  "d['inspection']['active'] is True and d['current']['entry']['plotId'] == d['inspection']['plotId']"
code="$(post o-exit woodland/inspection/exit '{}')"
check "O05 exit after the overlap: inactive status from the same commit" py o-exit "d['wasActive'] is True and d['inspection']['active'] is False"

# ---- S: Save and Recover of one slot, released together, never deadlock (one lock order: host, then slot) ----------
code="$(get s-before view)"; fs="$(fence s-before)"
race saverecover "$port" "$base" "$fs" insp-part2 30 "$scratch/s1.json"
check "S01 30 simultaneous Save + Recover pairs on one slot: every request answered within 20 s (no deadlock)" py s1 \
  "d['timeouts'] == 0 and d['saveCount'] == 30 and d['recoverCount'] == 30"
check "S02 every Save committed (200) and every Recover answered 200 or a typed 409" py s1 \
  "d['save'] == [200] and set(d['recover']) <= {200, 409}"
code="$(get s-after view)"
check "S03 the server still answers reads afterwards and the playable woodland did not change" bash -c \
  'test "$1" == 200' _ "$code"
check "S04 same session, year and revisions after the Save/Recover burst" same_fence s-before s-after
# ---- M: verify the persisted progress marker through the API ------------------------------------------------------
# These checks observe the marker. The host lock in MultiSaveAsync establishes the ordering for an overlapping load.
code="$(post m-advance year/advance "{\"fence\":$fs,\"consentToCancelWork\":false}")"
check "M01 advancing creates unsaved progress" py m-advance "d['hasUnsavedProgress'] is True"
code="$(get m-dirty view)"; fm="$(fence m-dirty)"; ym="$(year_of m-dirty)"
check "M02 GET view reports unsaved progress before either refusal" py m-dirty "d['hasUnsavedProgress'] is True"
race paused "$port" "$base" api/multi/save "{\"fence\":$fm,\"slot\":\"insp-part2\",\"replaceExisting\":false}" IPlus_Mersch "$scratch/s5.json"
check "S05 a blocked Save to an EXISTING slot is refused as WoodlandInspectionReadOnly at commit, before any SlotExists check" commit_refusal s5
code="$(get m-blocked view)"
check "M03 the refused Save leaves the live marker dirty during survey inspection" py m-blocked "d['hasUnsavedProgress'] is True"
code="$(post s-exit woodland/inspection/exit '{}')"

code="$(post m-exists save "{\"fence\":$fm,\"slot\":\"insp-part2\",\"replaceExisting\":false}")"
check "M04 an overwrite refused in normal play returns SlotExists" py m-exists "d['refusal']['code'] == 'SlotExists'"
code="$(get m-after-exists view)"
check "M05 the failed overwrite leaves the live marker dirty" py m-after-exists "d['hasUnsavedProgress'] is True"
code="$(post m-save1 save "{\"fence\":$fm,\"slot\":\"insp-part2\",\"replaceExisting\":true}")"
check "M06 the first overwrite saves the current year" py m-save1 "d['refusal'] is None and d['saved']['ecologicalYear'] == $ym"
code="$(get m-clean view)"
check "M07 the successful overwrite marks the current progress saved" py m-clean "d['hasUnsavedProgress'] is False"

code="$(post m-advance2 year/advance "{\"fence\":$fm,\"consentToCancelWork\":false}")"
check "M08 advancing again makes the next year unsaved" py m-advance2 \
  "d['hasUnsavedProgress'] is True and d['view']['source']['ecologicalYear'] == $((ym + 1))"
fn="$(fence m-advance2)"
code="$(post m-save2 save "{\"fence\":$fn,\"slot\":\"insp-part2\",\"replaceExisting\":true}")"
check "M09 the latest same-slot Save records the newer year and save generation" python3 -c \
  'import json,sys; older,newer=(json.load(open(p))["saved"] for p in sys.argv[1:]); assert newer["ecologicalYear"]==older["ecologicalYear"]+1 and newer["saveGeneration"]==older["saveGeneration"]+1' \
  "$scratch/m-save1.json" "$scratch/m-save2.json"
code="$(post m-survey woodland/inspection/load '{"plotId":"IPlus_Donadea_MHF_2"}')"
check "M10 survey opens after the latest same-slot Save" py m-survey \
  "d['inspection']['active'] is True and d['inspection']['plotId'] == 'IPlus_Donadea_MHF_2'"
code="$(get m-survey-view view)"
check "M11 GET playable view still answers during the survey" bash -c 'test "$1" == 200' _ "$code"
check "M12 the latest saved marker and fence are visible during inspection" py m-survey-view \
  "d['hasUnsavedProgress'] is False and d['view']['source']['ecologicalYear'] == $((ym + 1))"
check "M13 the survey leaves the latest playable session and revisions intact" same_fence m-advance2 m-survey-view
code="$(post m-exit woodland/inspection/exit '{}')"

# ---- Z: clean shutdown ---------------------------------------------------------------------------------------------
kill -INT "$server_pid"
for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
if kill -0 "$server_pid" 2> /dev/null; then say "FAIL Z00 still running 15 s after SIGINT"; fails=$((fails + 1)); kill -KILL "$server_pid" 2> /dev/null || true; fi
rc=0; wait "$server_pid" 2> /dev/null || rc=$?
server_pid=""
check "Z01 SIGINT ends the process with exit 0 (rc $rc)" test "$rc" == 0
check "Z02 the server wrote nothing to stderr (no unconfirmed outcome, no fault)" test ! -s "$scratch/server.err"
check "Z03 no file appeared or changed in the lab tree (build output excluded)" test "$(tree_snapshot)" == "$tree_before"
check "Z04 git status of the lab is unchanged" test "$(git_status)" == "$status_before"

say "woodland-inspection-part2-api: $passes passed, $fails failed"
[[ "$fails" == 0 ]]
