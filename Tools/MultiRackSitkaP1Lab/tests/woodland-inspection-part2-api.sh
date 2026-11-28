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
read_only_refusal() { python3 - "$scratch/$1.json" << 'PY'
import json, sys
d = json.load(open(sys.argv[1], encoding='utf-8'))
ok = (d.get('error', {}).get('code') == 'WoodlandInspectionReadOnly' and d.get('refusal', {}).get('code') == 'WoodlandInspectionReadOnly'
      and d['inspection']['active'] is True and d['inspection']['canAdvanceTime'] is False
      and 'nothing changed' in d['refusal']['message'] and 'api/' not in d['refusal']['message'])
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
  check "L-$plot the served view is the exact Part 1 view (golden SHA-256 $golden)" py "$name" "d['current']['viewSha256'] == '$golden'"
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
