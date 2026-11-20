#!/usr/bin/env bash
# #849 PM-T07 (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §3.6, AC-PM-11; plan P3 step 5): a bash + curl witness of the UI-A
# presentation process. It starts the BUILT UI against a scratch copy of the committed fixture, with hostile URL and
# Kestrel environment variables set, then checks:
#   - the only listening socket is 127.0.0.1 (/proc/net/tcp 0100007F) on an ephemeral port other than 5187;
#   - the guards: 404 without the capability, 403 for a wrong Host or Origin, 405, 415, 413 and 400; the page, style and
#     script it serves are byte-identical to the committed src/Ui/wwwroot files;
#   - one full Work Plan cycle through the JSON API: refusals, Add, a stale-fence refusal, hit-tests (candidates exactly
#     as an oracle computed from the scene at the most-overlapped point, none on bare ground inside the square, 400
#     without a point), Approve at zero Fell, Execute (one typed Fell in the same year), replay, key conflict, Advance
#     refused with the old fence and then committed with a fresh fence, the consent refusal and consented cancellation,
#     and a further decision in Year+2 that also uses Remove and Cancel approved plan (and their refusals); an Execute
#     under an unminted key that a later plan is minted with is refused, and that later plan then executes normally;
#   - the page's own script against this process (J checks, Tier-2 D1 and D2): an Execute confirmation stays bound to the
#     plan it showed when a pending re-read brings a different plan, and is withdrawn unsent; after an outcome that may
#     or may not have committed, every engine command is disabled and refused and no request but a read leaves the page
#     until a re-read succeeds;
#   - SIGINT (Ctrl+C) ends the process cleanly; no file appears in the lab tree or in ~/.aspnet; P0 is untouched.
# curl talks only to 127.0.0.1. python3 (standard-library json only) reads the JSON answers. node (no packages) runs the
# served app.js in a small DOM stand-in; that stand-in is not a browser (no layout, CSS or real focus rules).
#
# Usage: tests/ui-smoke.sh [<evidence-dir>]
#   Build first: ~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj   (DOTNET overrides the dotnet used; default ~/.dotnet/dotnet)
#   NODE overrides the node used (default: node on PATH).
#   TMPDIR and <evidence-dir> must be outside the repository. Exit 0 only if every check passes.
# Idempotent: every run uses a fresh temporary directory, which is removed at exit together with the process.
set -euo pipefail

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
dotnet_bin="${DOTNET:-$HOME/.dotnet/dotnet}"
node_bin="${NODE:-$(command -v node || true)}"
ui_dll="$p1_root/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
fixture_name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
committed_fixture="$p1_root/fixtures/derived/$fixture_name"
committed_sha="bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4"
evidence="${1:-}"

[[ -x "$dotnet_bin" ]] || { echo "STOP: $dotnet_bin is not executable (set DOTNET)" >&2; exit 2; }
[[ -f "$ui_dll" ]] || { echo "STOP: build first: $dotnet_bin build $p1_root/src/Ui/SitkaP1.Ui.csproj" >&2; exit 2; }
command -v curl > /dev/null && command -v python3 > /dev/null || { echo "STOP: curl and python3 are required" >&2; exit 2; }
[[ -n "$node_bin" && -x "$node_bin" ]] || { echo "STOP: node is required for the page witness (set NODE)" >&2; exit 2; }
if [[ -n "$evidence" ]]; then
  mkdir -p -- "$evidence"
  case "$(cd "$evidence" && pwd -P)/" in "$repo"/*) echo "STOP: the evidence directory is inside the repository" >&2; exit 2 ;; esac
fi

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-p1-uismoke.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; rm -rf -- "$scratch"; exit 2 ;; esac
log="$scratch/ui-smoke.txt"
: > "$log"
server_pid=""
cleanup() {
  if [[ -n "$server_pid" ]] && kill -0 "$server_pid" 2> /dev/null; then kill -KILL "$server_pid" 2> /dev/null || true; fi
  if [[ -n "$evidence" ]]; then cp -- "$log" "$evidence/ui-smoke.txt" 2> /dev/null || true; fi
  rm -rf -- "$scratch"
}
trap cleanup EXIT

fail=0
say() { echo "$*" | tee -a "$log"; }
pass() { say "PASS $*"; }
bad() { say "FAIL $*"; fail=1; }
check() { local label="$1"; shift; if "$@"; then pass "$label"; else bad "$label"; fi; }

# ---- baselines: the whole lab tree (bin/obj/.p0ref included), ~/.aspnet, P0 and git status ---------------------------------
tree_snapshot() { (cd "$p1_root" && find . -printf '%y %p %s %T@\n' | LC_ALL=C sort); }
aspnet_snapshot() {
  if [[ -e "$HOME/.aspnet" || -L "$HOME/.aspnet" ]]; then (cd "$HOME/.aspnet" && find . -printf '%y %p %s %T@\n' | LC_ALL=C sort)
  else echo "absent"; fi
}
p0_digest() { (cd "$repo" && git ls-files -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum | cut -c1-64); }
git_status() { git -C "$repo" status --porcelain --untracked-files=all -- Tools/MultiRackSitkaP1Lab Tools/MultiRackSitkaLab; }
tree_before="$(tree_snapshot)"; aspnet_before="$(aspnet_snapshot)"; p0_before="$(p0_digest)"; status_before="$(git_status)"
say "ui-smoke $(date -u +%FT%TZ) HEAD $(git -C "$repo" rev-parse HEAD) dotnet $("$dotnet_bin" --version) node $(cd "$scratch" && "$node_bin" --version) ~/.aspnet: $([[ "$aspnet_before" == absent ]] && echo absent || echo present)"

# ---- scratch fixture copy (a fresh regular, single-link file) -------------------------------------------------------------
mkdir "$scratch/fixture"
cat -- "$committed_fixture" > "$scratch/fixture/$fixture_name"
copy_sha="$(sha256sum "$scratch/fixture/$fixture_name" | cut -c1-64)"
check "S00 scratch fixture copy sha256 $copy_sha equals the committed fixture" test "$copy_sha" == "$committed_sha"

# ---- start: hostile environment, so only the code's own Listen(127.0.0.1, 0) can explain the bind ---------------------------
# Job control gives the process its own process group with the default SIGINT action, as in an interactive terminal.
set -m
( cd "$p1_root" && exec env ASPNETCORE_URLS="http://0.0.0.0:5187" DOTNET_URLS="http://0.0.0.0:5187" ASPNETCORE_HTTP_PORTS=5187 \
    Kestrel__Endpoints__Lan__Url="http://0.0.0.0:5187" ASPNETCORE_ENVIRONMENT=Development \
    "$dotnet_bin" "$ui_dll" "$scratch/fixture" ) > "$scratch/server.out" 2> "$scratch/server.err" &
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
  bad "S01 the process printed no launch URL"; sed 's/^/    /' "$scratch/server.err" | tee -a "$log"; exit 1
fi
port="${url#http://127.0.0.1:}"; port="${port%%/*}"
cap="${url#*/s/}"; cap="${cap%/}"
origin="http://127.0.0.1:$port"
say "launch line: $(grep -m1 '^Open ' "$scratch/server.out" | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#')"
check "S01 printed URL is http://127.0.0.1:<port>/s/<64 hex>/ with port $port (not 5187, not privileged)" \
  test "$port" != 5187 -a "$port" -ge 1024

# Every TCP socket of the process, from /proc/net/tcp and tcp6, with its state and addresses.
sockets() {
  local inodes="" fd link
  for fd in /proc/"$server_pid"/fd/*; do
    link="$(readlink "$fd" 2> /dev/null)" || continue
    [[ "$link" == socket:\[*\] ]] && inodes+="${link//[^0-9]/} "
  done
  awk -v inodes="$inodes" 'BEGIN { n = split(inodes, a, " "); for (i = 1; i <= n; i++) want[a[i]] = 1 }
    FNR > 1 && ($10 in want) { print FILENAME, $4, $2, $3 }' /proc/net/tcp /proc/net/tcp6
}
listeners="$(sockets | awk '$2 == "0A" { print $1, $3 }')"
expected_listener="/proc/net/tcp 0100007F:$(printf '%04X' "$port")"
check "S02 the only LISTEN socket of pid $server_pid is [$expected_listener] (got [$listeners])" test "$listeners" == "$expected_listener"

# ---- HTTP helpers -----------------------------------------------------------------------------------------------------------
# req <name> <method> <path> [curl args...] : saves <name>.body and <name>.head, prints the HTTP status ("000" if none).
req() {
  local name="$1" method="$2" path="$3"; shift 3
  curl --silent --noproxy '*' --max-time 15 -o "$scratch/$name.body" -D "$scratch/$name.head" -w '%{http_code}' \
    -X "$method" "$@" "$origin$path" || true
}
# post <name> <path> <json> [curl args...] : a POST exactly as the page sends it (own Origin, application/json).
post() {
  local name="$1" path="$2" json="$3"; shift 3
  printf '%s' "$json" > "$scratch/$name.req"
  req "$name" POST "$path" -H "Origin: $origin" -H 'Content-Type: application/json' --data-binary @"$scratch/$name.req" "$@"
}
# js <name> <python expression over r, the parsed body> : prints the value (strings bare, everything else as JSON).
js() {
  python3 -I -B -c 'import json, sys
r = json.load(open(sys.argv[1], encoding="utf-8"))
# Globals, not locals, so that nested generator expressions can see these names too.
names = {"__builtins__": {}, "r": r, "len": len, "all": all, "sorted": sorted, "str": str, "list": list, "range": range}
v = eval(sys.argv[2], names)
print(v if isinstance(v, str) else json.dumps(v))' "$scratch/$1.body" "$2"
}
# fence <name> : the fence exactly as the view in <name>.body gave it.
fence() {
  python3 -I -B -c 'import json, sys
v = json.load(open(sys.argv[1], encoding="utf-8"))["view"]
print(json.dumps({"sessionId": v["sessionId"], "source": v["source"], "planStateRevision": v["planStateRevision"]}, separators=(",", ":")))' \
    "$scratch/$1.body"
}
header() { grep -i "^$2:" "$scratch/$1.head" | head -1 | cut -d: -f2- | tr -d '\r' | sed 's/^ *//'; }
expect() { # expect <label> <got> <want> [empty <name>] : the status, and with "empty" also an empty body
  local label="$1" got="$2" want="$3" empty="${4:-}" name="${5:-}"
  if [[ "$got" == "$want" && ( -z "$empty" || ! -s "$scratch/$name.body" ) ]]; then pass "$label: HTTP $got"; else bad "$label: HTTP $got (wanted $want)"; fi
}
refusal() { # refusal <label> <name> <code> : the HTTP status in $code is 409, with the engine's code and a fresh view
  local label="$1" name="$2" want="$3" got
  got="$(js "$name" '(r.get("refusal") or {}).get("code", "") + "|" + str(len(r["view"]["frame"]["trees"]) if "view" in r else "no-view")' 2> /dev/null || echo "?|?")"
  if [[ "$code" == 409 && "$got" == "$want|100" ]]; then pass "$label: HTTP 409 $want, fresh view attached"; else bad "$label: HTTP $code, got $got (wanted 409 $want)"; fi
}

# ---- guards -----------------------------------------------------------------------------------------------------------------
code="$(req g-root GET /)";                                         expect "G01 GET / without capability: 404, empty body" "$code" 404 empty g-root
code="$(req g-zero GET "/s/$(printf '0%.0s' $(seq 64))/")";         expect "G02 wrong capability: 404, empty body" "$code" 404 empty g-zero
code="$(req g-short GET "/s/${cap:0:63}/")";                        expect "G03 63-character capability prefix: 404, empty body" "$code" 404 empty g-short
code="$(req g-host GET "/s/$cap/" -H "Host: localhost:$port")";     expect "G04 Host localhost:$port: 403, empty body" "$code" 403 empty g-host
code="$(req g-rebind GET "/s/$cap/api/view" -H "Host: rebind.example:$port")"; expect "G05 DNS-rebinding Host: 403, empty body" "$code" 403 empty g-rebind
code="$(req page GET "/s/$cap/")";                                  expect "G06 page with capability and exact Host" "$code" 200
check "G07 page is text/html with the external script and style only" grep -q '<script src="app.js" defer></script>' "$scratch/page.body"
check "G08 page answer: no CORS header, Cache-Control no-store, strict CSP, nosniff, no-referrer, DENY framing, no Server" bash -c '
  h="$1"; ! grep -qi "^access-control-" "$h" && ! grep -qi "^server:" "$h" && grep -qi "^cache-control: no-store" "$h" &&
  grep -qi "^content-security-policy: default-src .none.; script-src .self." "$h" && grep -qi "^x-content-type-options: nosniff" "$h" &&
  grep -qi "^referrer-policy: no-referrer" "$h" && grep -qi "^x-frame-options: DENY" "$h" && grep -qi "^content-type: text/html" "$h"' _ "$scratch/page.head"
code="$(req redirect GET "/s/$cap")"
check "G09 /s/<capability> without slash: 308 to /s/<capability>/ (got $code)" test "$code" == 308 -a "$(header redirect location)" == "/s/$cap/"
code="$(req css GET "/s/$cap/app.css")"; check "G10 app.css: 200 text/css" test "$code" == 200 -a "$(header css content-type)" == "text/css; charset=utf-8"
code="$(req jsfile GET "/s/$cap/app.js")"; check "G11 app.js: 200 text/javascript" test "$code" == 200 -a "$(header jsfile content-type)" == "text/javascript; charset=utf-8"
check "G11b the page, app.css and app.js answers are byte-identical to the committed src/Ui/wwwroot files" bash -c \
  'cmp -s "$1" "$4/index.html" && cmp -s "$2" "$4/app.css" && cmp -s "$3" "$4/app.js"' _ \
  "$scratch/page.body" "$scratch/css.body" "$scratch/jsfile.body" "$p1_root/src/Ui/wwwroot"
code="$(req g-xorigin GET "/s/$cap/api/view" -H "Origin: http://attacker.example")"; expect "G12 GET naming a foreign Origin: 403" "$code" 403
code="$(req g-method GET "/s/$cap/api/plan/approve")"
check "G13 GET on a POST endpoint: 405, Allow: POST (got $code)" test "$code" == 405 -a "$(header g-method allow)" == POST
code="$(req g-unknown GET "/s/$cap/api/none")";                     expect "G14 unknown endpoint under the capability: 404" "$code" 404

code="$(req v0 GET "/s/$cap/api/view")"; expect "V00 GET api/view" "$code" 200
check "V01 view: 100 exact fixture IDs, 100 scene items, year 0, annual 0, work 0, plan-state 0, nothing pending or approved" \
  test "$(js v0 '[len(r["view"]["frame"]["trees"]), len(r["scene"]), r["view"]["frame"]["ecologicalYear"], r["view"]["frame"]["annualRevision"], r["view"]["frame"]["workRevision"], r["view"]["planStateRevision"], len(r["view"]["pending"]), r["view"]["approved"]]')" == "[100, 100, 0, 0, 0, 0, 0, null]"
check "V02 labels come from Core: G1 disabled, money unavailable (not 0), session only" \
  test "$(js v0 'r["labels"]["ComparisonExtension"] + "|" + r["labels"]["Money"] + "|" + r["labels"]["SessionOnly"]')" == \
  "Comparison extension: DISABLED — Manual/NotCovered|Costs, revenue, budget: UNAVAILABLE (not €0)|Session only — nothing is saved"
check "V03 every frame tree and scene item carries the JSON names the page reads (xm, zm, crownRadiusM, u, v, radiusM, paintOrder)" \
  test "$(js v0 '[all(all(k in t for k in ["treeId", "xm", "zm", "ageYears", "dbhCm", "heightM", "crownRadiusM", "isLiving"]) for t in r["view"]["frame"]["trees"]), all(all(k in i for k in ["treeId", "paintOrder", "u", "v", "radiusM", "heightM", "dbhCm", "status", "deathYear"]) for i in r["scene"])]')" == "[true, true]"
check "V04 the scene has exactly the frame's exact IDs, in paint order 0..99" \
  test "$(js v0 '[sorted([i["treeId"] for i in r["scene"]]) == sorted([t["treeId"] for t in r["view"]["frame"]["trees"]]), [i["paintOrder"] for i in r["scene"]] == list(range(100))]')" == "[true, true]"

f0="$(fence v0)"
code="$(req g-noorigin POST "/s/$cap/api/plan/approve" -H 'Content-Type: application/json' --data-binary "{\"fence\":$f0}")"
expect "G15 POST without Origin: 403" "$code" 403
code="$(post g-badorigin "/s/$cap/api/plan/approve" "{\"fence\":$f0}" -H "Origin: http://localhost:$port")"
expect "G16 POST with Origin http://localhost:$port: 403" "$code" 403
code="$(post g-type "/s/$cap/api/plan/approve" "{\"fence\":$f0}" -H 'Content-Type: text/plain')"
expect "G17 POST text/plain: 415" "$code" 415
code="$(post g-form "/s/$cap/api/plan/approve" "fence=1" -H 'Content-Type: application/x-www-form-urlencoded')"
expect "G18 POST form-urlencoded: 415" "$code" 415
python3 -I -B -c 'print("{\"fence\":null,\"pad\":\"" + "x" * 17000 + "\"}", end="")' > "$scratch/big.json"
code="$(req g-big POST "/s/$cap/api/plan/approve" -H "Origin: $origin" -H 'Content-Type: application/json' --data-binary @"$scratch/big.json")"
expect "G19 POST body over 16 KiB: 413" "$code" 413
code="$(post g-malformed "/s/$cap/api/plan/approve" '{"fence":')";                  expect "G20 malformed JSON: 400" "$code" 400
code="$(post g-planobject "/s/$cap/api/plan/execute" "{\"fence\":$f0,\"planId\":\"x\",\"executionKey\":\"x\",\"plan\":{\"orders\":[]}}")"
expect "G21 a caller-built plan object (unknown member): 400" "$code" 400
code="$(post g-null "/s/$cap/api/plan/approve" 'null')";                            expect "G22 JSON null body: 400" "$code" 400
code="$(req v-after-guards GET "/s/$cap/api/view")"
check "G23 after every guard refusal the view is unchanged (plan-state 0, nothing pending, work 0)" \
  test "$(js v-after-guards '[r["view"]["planStateRevision"], len(r["view"]["pending"]), r["view"]["frame"]["workRevision"]]')" == "[0, 0, 0]"

# ---- engine refusals (409 with a fresh view; nothing changes) -----------------------------------------------------------------
t1="$(js v0 'r["view"]["frame"]["trees"][0]["treeId"]')"
t2="$(js v0 'r["view"]["frame"]["trees"][1]["treeId"]')"
wrong_case="$(printf '%s' "$t1" | tr '[:upper:]' '[:lower:]')"
say "trees used: $t1 (Fell in Year 0), $t2 (consented cancellation); wrong-case form: $wrong_case"
code="$(post r-case "/s/$cap/api/pending/add" "{\"fence\":$f0,\"treeId\":\"$wrong_case\",\"reason\":\"ui-smoke\"}")"
refusal "R01 Add with a wrong-case ID" r-case UnknownTreeId
code="$(post r-reason "/s/$cap/api/pending/add" "{\"fence\":$f0,\"treeId\":\"$t1\",\"reason\":\"   \"}")"
refusal "R02 Add with a blank reason" r-reason WorkReasonRequired
wrong_session="$(printf '%s' "$f0" | python3 -I -B -c 'import json, sys; f = json.load(sys.stdin); f["sessionId"] = "0" * 32; print(json.dumps(f))')"
code="$(post r-session "/s/$cap/api/pending/add" "{\"fence\":$wrong_session,\"treeId\":\"$t1\",\"reason\":\"ui-smoke\"}")"
refusal "R03 Add with another session's fence" r-session PlayerSessionMismatch
code="$(post r-nofence "/s/$cap/api/plan/approve" '{}')"
refusal "R04 Approve without a fence" r-nofence PlayerSessionMismatch
code="$(post r-empty "/s/$cap/api/plan/approve" "{\"fence\":$f0}")"
refusal "R05 Approve with nothing pending" r-empty WorkEmptyPlan
code="$(post r-noplan "/s/$cap/api/plan/execute" "{\"fence\":$f0,\"planId\":\"invented\",\"executionKey\":\"invented:execute\"}")"
refusal "R06 Execute without an approved plan" r-noplan WorkPlanNotApproved
check "R07 after the refusals: plan-state 0, nothing pending, 100 living" \
  test "$(js r-noplan '[r["view"]["planStateRevision"], len(r["view"]["pending"]), r["view"]["frame"]["stand"]["livingCount"]]')" == "[0, 0, 100]"

# ---- the Work Plan cycle ----------------------------------------------------------------------------------------------------
code="$(post a1 "/s/$cap/api/pending/add" "{\"fence\":$f0,\"treeId\":\"$t1\",\"reason\":\"ui-smoke: manual selection\"}")"
expect "A01 Add $t1 with a reason" "$code" 200
check "A02 Add: pending [$t1], plan-state 1, work 0, 100 living, year 0, no event (no tree felled)" \
  test "$(js a1 '[[o["treeId"] for o in r["view"]["pending"]], r["view"]["planStateRevision"], r["view"]["frame"]["workRevision"], r["view"]["frame"]["stand"]["livingCount"], r["view"]["frame"]["ecologicalYear"], len(r["view"]["frame"]["events"])]')" == "[[\"$t1\"], 1, 0, 100, 0, 0]"
check "A03 the scene marks $t1 PendingFell" test "$(js a1 "[i['status'] for i in r['scene'] if i['treeId'] == '$t1']")" == '["PendingFell"]'
code="$(post a-stale "/s/$cap/api/pending/add" "{\"fence\":$f0,\"treeId\":\"$t2\",\"reason\":\"stale\"}")"
refusal "A04 Add with the pre-Add fence" a-stale PlayerPlanStateStale
check "A05 the stale Add changed nothing (pending [$t1], plan-state 1)" \
  test "$(js a-stale '[[o["treeId"] for o in r["view"]["pending"]], r["view"]["planStateRevision"]]')" == "[[\"$t1\"], 1]"

f1="$(fence a1)"
read -r u1 v1 < <(js a1 "[(i['u'], i['v']) for i in r['scene'] if i['treeId'] == '$t1'][0]" | tr -d '[],' )
code="$(post h1 "/s/$cap/api/hit" "{\"fence\":$f1,\"u\":$u1,\"v\":$v1}")"
expect "H01 hit-test at the centre of $t1 ($u1, $v1)" "$code" 200
check "H02 candidates include $t1, topmost first, and every candidate disc contains the point" \
  test "$(js h1 "['$t1' in [c['treeId'] for c in r['candidates']], [c['paintOrder'] for c in r['candidates']] == sorted([c['paintOrder'] for c in r['candidates']], reverse=True), all((c['u'] - $u1) ** 2 + (c['v'] - $v1) ** 2 <= c['radiusM'] ** 2 for c in r['candidates'])]")" == "[true, true, true]"
say "H01 candidates at $t1: $(js h1 '[c["treeId"] for c in r["candidates"]]')"
code="$(post h-bare "/s/$cap/api/hit" "{\"fence\":$f1,\"u\":-2.5,\"v\":-2.5}")"
check "H03 hit-test on bare ground outside every crown: 200 and no candidate (got $code)" test "$code" == 200 -a "$(js h-bare 'len(r["candidates"])')" == 0
code="$(post h-stale "/s/$cap/api/hit" "{\"fence\":$f0,\"u\":$u1,\"v\":$v1}")"
refusal "H04 hit-test with a stale fence" h-stale PlayerPlanStateStale
check "H05 the stale hit-test returned no candidate" test "$(js h-stale 'len(r["candidates"])')" == 0
code="$(post h-nopoint "/s/$cap/api/hit" "{\"fence\":$f1}")"
expect "H06 hit-test without u and v: 400 (a missing point is never read as 0)" "$code" 400

# Oracle points, computed by this test from the scene in a1 on a 0.25 m grid over the closed 30 m square, with the same
# arithmetic as the server (du*du + dv*dv <= r*r): the point covered by the most crowns, and the first point no crown
# covers. The server's candidates must be exactly the discs that contain the point, highest paint order first.
read -r ux vx < <(js a1 'sorted([(-len([i for i in r["scene"] if (x / 4 - i["u"]) * (x / 4 - i["u"]) + (y / 4 - i["v"]) * (y / 4 - i["v"]) <= i["radiusM"] * i["radiusM"]]), x / 4, y / 4) for x in range(121) for y in range(121)])[0][1:]' | tr -d '[],')
oracle_x="$(js a1 "[i['treeId'] for i in sorted(r['scene'], key=lambda i: -i['paintOrder']) if ($ux - i['u']) * ($ux - i['u']) + ($vx - i['v']) * ($vx - i['v']) <= i['radiusM'] * i['radiusM']]")"
code="$(post h-max "/s/$cap/api/hit" "{\"fence\":$f1,\"u\":$ux,\"v\":$vx}")"
check "H07 hit-test at the most-overlapped grid point ($ux, $vx): 200, candidates exactly $oracle_x, topmost first (got $code)" \
  test "$code" == 200 -a "$(js h-max '[c["treeId"] for c in r["candidates"]]')" == "$oracle_x"
check "H08 that point is a real overlap: $(js h-max 'len(r["candidates"])') crowns (at least 2)" test "$(js h-max 'len(r["candidates"])')" -ge 2
read -r ub vb < <(js a1 '[(x / 4, y / 4) for x in range(121) for y in range(121) if all((x / 4 - i["u"]) * (x / 4 - i["u"]) + (y / 4 - i["v"]) * (y / 4 - i["v"]) > i["radiusM"] * i["radiusM"] for i in r["scene"])][0]' | tr -d '[],')
code="$(post h-gap "/s/$cap/api/hit" "{\"fence\":$f1,\"u\":$ub,\"v\":$vb}")"
check "H09 hit-test on bare ground inside the square ($ub, $vb; no crown covers it): 200 and no candidate, so the nearest tree is not chosen (got $code)" \
  test "$code" == 200 -a "$(js h-gap 'len(r["candidates"])')" == 0

code="$(post p1 "/s/$cap/api/plan/approve" "{\"fence\":$f1}")"
expect "P01 Approve the whole Work Plan" "$code" 200
session="$(js p1 'r["view"]["sessionId"]')"
plan_id="$(js p1 'r["view"]["approved"]["planId"]')"
exec_key="$(js p1 'r["view"]["approved"]["executionKey"]')"
check "P02 engine-minted PlanId $plan_id = <session>:plan:2 and key = <PlanId>:execute" \
  test "$plan_id" == "$session:plan:2" -a "$exec_key" == "$session:plan:2:execute"
check "P03 Approve fells nothing: 100 living, work 0, no event, pending moved into the plan, plan-state 2" \
  test "$(js p1 '[r["view"]["frame"]["stand"]["livingCount"], r["view"]["frame"]["workRevision"], len(r["view"]["frame"]["events"]), len(r["view"]["pending"]), [o["treeId"] for o in r["view"]["approved"]["orders"]], r["view"]["planStateRevision"]]')" == "[100, 0, 0, 0, [\"$t1\"], 2]"

f2="$(fence p1)"
# Tier-2 D3: the key plan:12 is minted only at D02 below (plan-state 12). Taking it now, for this plan, is refused with
# nothing changed, so D03 can later execute that plan under its own minted key.
code="$(post e-unminted "/s/$cap/api/plan/execute" "{\"fence\":$f2,\"planId\":\"$plan_id\",\"executionKey\":\"$session:plan:12:execute\"}")"
refusal "E00 Execute the approved plan under an unminted key (the key plan 12 will be minted with)" e-unminted WorkExecutionKeyConflict
check "E00b the refused Execute changed nothing (plan $plan_id still approved, plan-state 2, work 0, 100 living)" \
  test "$(js e-unminted '[r["view"]["approved"]["planId"], r["view"]["planStateRevision"], r["view"]["frame"]["workRevision"], r["view"]["frame"]["stand"]["livingCount"]]')" == "[\"$plan_id\", 2, 0, 100]"
code="$(post e1 "/s/$cap/api/plan/execute" "{\"fence\":$f2,\"planId\":\"$plan_id\",\"executionKey\":\"$exec_key\"}")"
expect "E01 Execute the approved plan" "$code" 200
check "E02 one typed Fell of exactly [$t1] (fell / manual-work-plan), not a replay" \
  test "$(js e1 '[r["execution"]["replayed"], [(f["treeId"], f["cause"], f["mechanism"]) for f in r["execution"]["event"]["fells"]]]')" == "[false, [[\"$t1\", \"fell\", \"manual-work-plan\"]]]"
check "E03 same year 0 and annual revision 0; work revision 0 -> 1; 99 living; plan-state 3; approval cleared" \
  test "$(js e1 '[r["view"]["frame"]["ecologicalYear"], r["view"]["frame"]["annualRevision"], r["view"]["frame"]["workRevision"], r["view"]["frame"]["stand"]["livingCount"], r["view"]["planStateRevision"], r["view"]["approved"]]')" == "[0, 0, 1, 99, 3, null]"
check "E04 the work event: living 100 -> 99 and basal area falls; the frame stand equals the event's after-stand" \
  test "$(js e1 '[r["execution"]["event"]["before"]["livingCount"], r["execution"]["event"]["after"]["livingCount"], r["execution"]["event"]["after"]["basalAreaM2PerHa"] < r["execution"]["event"]["before"]["basalAreaM2PerHa"], r["execution"]["event"]["after"] == r["view"]["frame"]["stand"]]')" == "[100, 99, true, true]"
check "E05 $t1 is drawn as a Felled stump (radius 0.35 m) with death year 0, cause fell" \
  test "$(js e1 "[(i['status'], i['radiusM'], i['deathYear'], i['deathCause']) for i in r['scene'] if i['treeId'] == '$t1']")" == '[["Felled", 0.35, 0, "fell"]]'
say "E01 basal area m2/ha: $(js e1 '[r["execution"]["event"]["before"]["basalAreaM2PerHa"], r["execution"]["event"]["after"]["basalAreaM2PerHa"]]')"

code="$(post e-replay "/s/$cap/api/plan/execute" "{\"fence\":$f2,\"planId\":\"$plan_id\",\"executionKey\":\"$exec_key\"}")"
expect "E06 the same Execute again with the original fence (a retry)" "$code" 200
check "E07 replay: replayed=true and nothing changed (work 1, 99 living, one event, plan-state 3)" \
  test "$(js e-replay '[r["execution"]["replayed"], r["view"]["frame"]["workRevision"], r["view"]["frame"]["stand"]["livingCount"], len(r["view"]["frame"]["events"]), r["view"]["planStateRevision"]]')" == "[true, 1, 99, 1, 3]"
code="$(post e-conflict "/s/$cap/api/plan/execute" "{\"fence\":$f2,\"planId\":\"$session:plan:999\",\"executionKey\":\"$exec_key\"}")"
refusal "E08 the same key with another PlanId" e-conflict WorkExecutionKeyConflict
code="$(post e-dead "/s/$cap/api/pending/add" "{\"fence\":$(fence e1),\"treeId\":\"$t1\",\"reason\":\"already felled\"}")"
refusal "E09 Add of the felled tree $t1" e-dead UnknownTreeId

code="$(post y-stale "/s/$cap/api/year/advance" "{\"fence\":$f2,\"consentToCancelWork\":false}")"
refusal "Y01 Advance with the pre-Execute fence" y-stale PlayerSourceStale
check "Y02 the refused Advance changed nothing (year 0, annual revision 0)" \
  test "$(js y-stale '[r["view"]["frame"]["ecologicalYear"], r["view"]["frame"]["annualRevision"]]')" == "[0, 0]"
code="$(req v1 GET "/s/$cap/api/view")"; expect "Y03 re-read for a fresh fence" "$code" 200
code="$(post y1 "/s/$cap/api/year/advance" "{\"fence\":$(fence v1),\"consentToCancelWork\":false}")"
expect "Y04 Advance with the fresh fence" "$code" 200
check "Y05 one genuine year 0 -> 1 (annual revision 1); $t1 stays dead with its Fell cause; plan-state unchanged (3)" \
  test "$(js y1 "[r['annual']['yearBefore'], r['annual']['yearAfter'], r['view']['frame']['ecologicalYear'], r['view']['frame']['annualRevision'], [t['death']['cause'] for t in r['view']['frame']['trees'] if t['treeId'] == '$t1'], r['view']['planStateRevision']]")" == "[0, 1, 1, 1, [\"fell\"], 3]"
check "Y06 history: one AnnualYearRecord 0 -> 1 without cancelled work; the year started from 99 living" \
  test "$(js y1 '[(h["yearBefore"], h["yearAfter"], h["cancelledUnexecutedWork"], h["before"]["livingCount"]) for h in r["view"]["annualHistory"]]')" == "[[0, 1, false, 99]]"
say "Y04 year 0 -> 1: living $(js y1 '[r["annual"]["before"]["livingCount"], r["annual"]["after"]["livingCount"]]'), natural deaths $(js y1 '[d["treeId"] for d in r["annual"]["deaths"]]')"

code="$(post c1 "/s/$cap/api/pending/add" "{\"fence\":$(fence y1),\"treeId\":\"$t2\",\"reason\":\"ui-smoke: consent case\"}")"
expect "C01 Add $t2 in Year 1" "$code" 200
code="$(post c-refused "/s/$cap/api/year/advance" "{\"fence\":$(fence c1),\"consentToCancelWork\":false}")"
refusal "C02 Advance with unexecuted work and no consent" c-refused WorkPendingConsentRequired
check "C03 the refused Advance changed nothing (year 1, pending [$t2])" \
  test "$(js c-refused '[r["view"]["frame"]["ecologicalYear"], [o["treeId"] for o in r["view"]["pending"]]]')" == "[1, [\"$t2\"]]"
code="$(post c2 "/s/$cap/api/year/advance" "{\"fence\":$(fence c-refused),\"consentToCancelWork\":true}")"
expect "C04 Advance with consent to cancel the unexecuted work" "$code" 200
check "C05 consent: Year 2, nothing pending, still one work event (zero Fell), $t2 not felled, plan-state 5, record marks the cancellation" \
  test "$(js c2 "[r['view']['frame']['ecologicalYear'], len(r['view']['pending']), len(r['view']['frame']['events']), [t['death'] for t in r['view']['frame']['trees'] if t['treeId'] == '$t2' and t['death'] and t['death']['cause'] == 'fell'], r['view']['planStateRevision'], r['view']['annualHistory'][-1]['cancelledUnexecutedWork']]")" == "[2, 0, 1, [], 5, true]"

# A further decision in Year 2 with fresh fences (the loop continues after a year), using Remove and Cancel on the way.
t3_live="$(js c2 "[t['treeId'] for t in r['view']['frame']['trees'] if t['death'] is None][2]")"
t4_live="$(js c2 "[t['treeId'] for t in r['view']['frame']['trees'] if t['death'] is None][3]")"
code="$(post d1 "/s/$cap/api/pending/add" "{\"fence\":$(fence c2),\"treeId\":\"$t3_live\",\"reason\":\"ui-smoke: Year 2 decision\"}")"
expect "D01 Year 2: Add $t3_live" "$code" 200
code="$(post d1b "/s/$cap/api/pending/add" "{\"fence\":$(fence d1),\"treeId\":\"$t4_live\",\"reason\":\"ui-smoke: to be removed\"}")"
expect "D01b Year 2: Add $t4_live" "$code" 200
code="$(post d1c "/s/$cap/api/pending/remove" "{\"fence\":$(fence d1b),\"treeId\":\"$t4_live\"}")"
expect "D01c Year 2: Remove $t4_live" "$code" 200
check "D01d Remove: pending back to [$t3_live], plan-state 8 (5, then add, add, remove), work 1, still one event (no Fell)" \
  test "$(js d1c '[[o["treeId"] for o in r["view"]["pending"]], r["view"]["planStateRevision"], r["view"]["frame"]["workRevision"], len(r["view"]["frame"]["events"])]')" == "[[\"$t3_live\"], 8, 1, 1]"
code="$(post d1e "/s/$cap/api/pending/remove" "{\"fence\":$(fence d1c),\"treeId\":\"$t4_live\"}")"
refusal "D01e Remove of an ID that is no longer pending" d1e UnknownWorkOrder
code="$(post d2a "/s/$cap/api/plan/approve" "{\"fence\":$(fence d1c)}")"; expect "D02a Year 2: Approve" "$code" 200
code="$(post d2b "/s/$cap/api/plan/cancel" "{\"fence\":$(fence d2a)}")"; expect "D02b Year 2: Cancel the approved plan" "$code" 200
check "D02c Cancel: no approved plan, its orders not returned to pending, plan-state 10, work 1, still one event (no Fell)" \
  test "$(js d2b '[r["view"]["approved"], len(r["view"]["pending"]), r["view"]["planStateRevision"], r["view"]["frame"]["workRevision"], len(r["view"]["frame"]["events"])]')" == "[null, 0, 10, 1, 1]"
code="$(post d2d "/s/$cap/api/plan/execute" "{\"fence\":$(fence d2b),\"planId\":\"$(js d2a 'r["view"]["approved"]["planId"]')\",\"executionKey\":\"$(js d2a 'r["view"]["approved"]["executionKey"]')\"}")"
refusal "D02d Execute of the cancelled plan" d2d WorkPlanNotApproved
code="$(post d2e "/s/$cap/api/pending/add" "{\"fence\":$(fence d2b),\"treeId\":\"$t3_live\",\"reason\":\"ui-smoke: Year 2 decision, re-added\"}")"
expect "D02e Year 2: re-add $t3_live" "$code" 200
code="$(post d2 "/s/$cap/api/plan/approve" "{\"fence\":$(fence d2e)}")"; expect "D02 Year 2: Approve" "$code" 200
check "D02f this plan is minted with the key that E00 tried to take: $session:plan:12:execute" \
  test "$(js d2 'r["view"]["approved"]["executionKey"]')" == "$session:plan:12:execute"
code="$(post d3 "/s/$cap/api/plan/execute" "{\"fence\":$(fence d2),\"planId\":\"$(js d2 'r["view"]["approved"]["planId"]')\",\"executionKey\":\"$(js d2 'r["view"]["approved"]["executionKey"]')\"}")"
expect "D03 Year 2: Execute under its own minted key (not blocked by E00)" "$code" 200
check "D04 Year 2 Fell of [$t3_live] in year 2; two work events; work revision 2" \
  test "$(js d3 '[[f["treeId"] for f in r["execution"]["event"]["fells"]], r["execution"]["event"]["ecologicalYear"], len(r["view"]["frame"]["events"]), r["view"]["frame"]["workRevision"]]')" == "[[\"$t3_live\"], 2, 2, 2]"

# ---- the page's own script (J checks; Tier-2 D1 and D2) ---------------------------------------------------------------------
# node runs app.js exactly as this process serves it (G11b: byte-identical to the committed file) inside a small standard-
# DOM stand-in built from the served index.html. Every request the page makes goes through a recording fetch to this real
# process, which can lose a response after the server has answered, fail a read, or hold a read. A second client uses the
# same API directly. The stand-in is NOT a browser: no layout, no CSS, simplified focus. It dispatches events on disabled
# buttons on purpose, so that each handler's own refusal is tested and not only the disabled state.
cat > "$scratch/page-witness.mjs" << 'WITNESS'
import vm from 'node:vm';

const pageUrl = process.argv[2];
const origin = new URL(pageUrl).origin;
const redact = (text) => String(text).replace(/\/s\/[0-9a-f]{64}\//g, '/s/<capability>/');
let failures = 0;
function check(ok, label, detail) {
  if (!ok) failures++;
  console.log((ok ? 'PASS ' : 'FAIL ') + label + (ok || detail === undefined ? '' : ' :: ' + redact(JSON.stringify(detail))));
}
const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));
async function until(predicate, what, timeout = 8000) {
  for (const end = Date.now() + timeout; Date.now() < end; await sleep(10)) if (predicate()) return true;
  check(false, 'timed out waiting for ' + what);
  return false;
}

// ---- a minimal standard-DOM stand-in: only what app.js uses ------------------------------------------------------------
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
}

class FakeDocument {
  constructor() { this.focused = null; this.title = ''; this.documentElement = null; this.body = null; }
  createElement(name) { return new FakeElement(this, name, 'http://www.w3.org/1999/xhtml'); }
  createElementNS(namespaceURI, name) { return new FakeElement(this, name, namespaceURI); }
  getElementById(id) { return this.documentElement.querySelector('#' + id); }
  querySelector(selector) { return this.documentElement.querySelector(selector); }
  querySelectorAll(selector) { return this.documentElement.querySelectorAll(selector); }
  // Focus fix-up, as in a browser: an element that is gone, hidden or disabled no longer has focus.
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
    return { amp: '&', lt: '<', gt: '>', quot: '"', apos: "'", nbsp: ' ' }[lower];
  });
}

// Enough of an HTML parser for the served page: elements, attributes, text, comments; void elements; SVG namespace.
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
const requests = []; // every request the page made: { method, path, body }
const faults = []; // test-controlled faults for the page's next matching request: lose, fail or hold
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
  return { status: answer.status, json: async () => answer.json };
}
const fenceOf = (view) => ({ sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision });
const serverView = async () => (await send('GET', 'api/view')).json.view;
async function serverCommand(path, body) {
  const answer = await send('POST', path, JSON.stringify(body));
  if (answer.status !== 200) throw new Error('second client: ' + path + ' answered HTTP ' + answer.status);
  return answer.json.view;
}
const living = (view, treeId) => view.frame.trees.some((tree) => tree.treeId === treeId && tree.isLiving);

// ---- start the page ----------------------------------------------------------------------------------------------------
const document = new FakeDocument();
parsePage(document, await (await fetch(pageUrl)).text());
const appJs = await (await fetch(new URL('app.js', pageUrl))).text();
const sandbox = {
  document,
  fetch: pageFetch,
  setTimeout,
  clearTimeout,
  CSS: { escape: (value) => String(value).replace(/[^a-zA-Z0-9_ -￿-]/g, (c) => '\\' + c) },
  DOMPoint: FakeDOMPoint,
  Node: FakeNode,
  Element: FakeElement,
  // A one-column viewport at device pixel ratio 2, as at 360 CSS px or at 200 % zoom.
  matchMedia: (media) => ({ media, matches: false, addEventListener() {}, removeEventListener() {} }),
  devicePixelRatio: 2,
  addEventListener() {},
};
sandbox.window = sandbox;
vm.runInContext(appJs, vm.createContext(sandbox), { filename: 'app.js' });

const byId = (id) => document.getElementById(id);
const ids = (id) => byId(id).querySelectorAll('.mono').map((node) => node.textContent);
const fact = (term) => {
  const row = byId('facts').querySelectorAll('div').find((div) => div.querySelector('dt').textContent === term);
  return row ? row.querySelector('dd').textContent : null;
};
const click = (element, init) => element.dispatchEvent(new FakeEvent('click', init));
const alertText = () => byId('alert').textContent;
const alertReread = () => byId('alert').querySelector('button');
const COMMANDS = ['add-button', 'insp-remove', 'approve', 'cancel-approved', 'execute', 'execute-confirm', 'advance', 'consent-advance'];
const removeButtons = () => byId('pending-list').querySelectorAll('[data-remove]');
const allCommandsDisabled = () => COMMANDS.every((id) => byId(id).disabled) && removeButtons().every((button) => button.disabled);
const anyCommandDisabled = () => COMMANDS.some((id) => byId(id).disabled);
async function selectInTable(treeId) {
  click(byId('tree-rows').querySelector('[data-select="' + treeId + '"]'));
  await until(() => !byId('inspector-body').hidden && byId('insp-id').textContent === treeId, 'the inspector to show ' + treeId);
}
async function addThroughPage(treeId, reason) {
  await selectInTable(treeId);
  byId('reason').value = reason;
  byId('add-form').dispatchEvent(new FakeEvent('submit'));
}

await until(() => byId('tree-rows').childNodes.length === 100, 'the first view');
const start = await serverView();
const startEvents = start.frame.events.length;
const living0 = start.frame.trees.filter((tree) => tree.isLiving).map((tree) => tree.treeId);
const [treeA, treeB, treeC] = living0.slice(-3); // trees the API checks above did not use
check(requests.length === 1 && requests[0].method === 'GET' && requests[0].path === 'api/view' && byId('tree-rows').childNodes.length === 100,
  `J01 the served page starts with one GET api/view and lists 100 trees (year ${start.frame.ecologicalYear}); trees used: ${treeA}, ${treeB}, ${treeC}`);
check(byId('map').getAttribute('width') === String(38 * 64) && byId('map-scale').textContent.includes('44.8 px across (at least 44 px)'),
  'J02 one-column default: the map is drawn at 64 px per metre and states that the smallest drawn item (a stump) is 44.8 px across',
  { width: byId('map').getAttribute('width'), note: byId('map-scale').textContent });

// ---- D1: a re-read that is still on its way when Execute is opened brings a different plan ----------------------------
await addThroughPage(treeA, 'page witness: plan A');
await until(() => ids('pending-list').includes(treeA), 'A pending');
click(byId('approve'));
await until(() => ids('approved-list').includes(treeA), 'plan A approved');
const viewA = await serverView();
const held = arm('GET', 'api/view', 'hold');
let mark = requests.length;
click(byId('reread'));
await until(() => requests.length === mark + 1, 'the held re-read to be sent');
click(byId('execute'));
await until(() => byId('execute-dialog').open, 'the Execute dialog');
check(ids('execute-ids').join() === treeA && byId('execute-plan').textContent.includes(viewA.approved.planId),
  `J03 with a re-read still pending, the Execute confirmation shows plan A only (${treeA}) and names its PlanId`, ids('execute-ids'));
check(byId('execute-text').textContent === `Fells exactly 1 named tree in Year ${viewA.frame.ecologicalYear}, or refuses all.`,
  'J04 a one-tree confirmation is worded in the singular (D5)', byId('execute-text').textContent);
let other = await serverCommand('api/plan/cancel', { fence: fenceOf(viewA) });
other = await serverCommand('api/pending/add', { fence: fenceOf(other), treeId: treeB, reason: 'second client: plan B' });
other = await serverCommand('api/plan/approve', { fence: fenceOf(other) });
const planB = other.approved;
check(planB.orders.map((order) => order.treeId).join() === treeB,
  `J05 a second client of the same session cancelled plan A and approved plan B (${treeB}) while the dialog was open`);
mark = requests.length;
click(byId('execute-confirm'));
await sleep(150);
check(requests.length === mark && byId('execute-dialog').open,
  'J06 Confirm while the re-read is still pending sends nothing and leaves the dialog as it is', requests.slice(mark));
held.release();
await until(() => ids('approved-list').includes(treeB), 'the page to draw plan B');
await sleep(50);
check(!byId('execute-dialog').open && alertText().includes('Confirmation withdrawn') && alertText().includes(viewA.approved.planId),
  'J07 when the view with plan B arrives, the plan-A confirmation is withdrawn and the alert names plan A', alertText());
check(!requests.some((request) => request.path === 'api/plan/execute'), 'J08 no Execute request was sent for either plan');
check(allCommandsDisabled() && !byId('reread').disabled && alertReread() && !alertReread().disabled,
  'J09 every engine command is disabled until a re-read; Re-read stays available');
mark = requests.length;
click(byId('execute-confirm'));
click(byId('execute'));
await sleep(150);
check(requests.length === mark && !byId('execute-dialog').open,
  'J10 clicking Execute or the withdrawn dialog\'s Execute button sends nothing and opens nothing', requests.slice(mark));
let server = await serverView();
check(living(server, treeA) && living(server, treeB) && server.frame.workRevision === viewA.frame.workRevision &&
  server.approved.planId === planB.planId, 'J11 on the server, A and B are both still living, no work was executed and plan B is still approved');
click(alertReread());
await until(() => !anyCommandDisabled(), 'the successful re-read to lift the lock');
check(alertText() === '' && !byId('execute').disabled, 'J12 a successful re-read lifts the lock and clears the alert');
click(byId('execute'));
await until(() => byId('execute-dialog').open, 'a fresh Execute dialog');
check(ids('execute-ids').join() === treeB, `J13 the fresh confirmation shows plan B only (${treeB})`);
click(byId('execute-confirm'));
await until(() => requests.some((request) => request.path === 'api/plan/execute'), 'the Execute request');
const sent = requests.find((request) => request.path === 'api/plan/execute').body;
check(sent.planId === planB.planId && sent.executionKey === planB.executionKey && sent.fence.planStateRevision === other.planStateRevision &&
  sent.fence.sessionId === other.sessionId && JSON.stringify(sent.fence.source) === JSON.stringify(other.source),
  'J14 the Execute request carries exactly the confirmed plan B: its PlanId, minted key and fence', sent);
await until(() => byId('receipt').textContent.includes(treeB), 'the receipt for plan B');
server = await serverView();
check(!living(server, treeB) && living(server, treeA) && server.frame.events.length === startEvents + 1,
  `J15 the server felled exactly ${treeB} (plan B) in one event; ${treeA} is still living`);

// ---- D2: an Add that committed, but whose answer was lost ------------------------------------------------------------
const shownPlanState = fact('Plan-state revision');
arm('POST', 'api/pending/add', 'lose');
await addThroughPage(treeC, 'page witness: lost answer');
await until(() => alertText().includes('Outcome unconfirmed'), 'the unconfirmed alert');
server = await serverView();
check(server.pending.some((order) => order.treeId === treeC) && server.planStateRevision === Number(shownPlanState) + 1,
  `J16 the server did commit the Add (${treeC} pending, plan-state ${server.planStateRevision})`);
check(!ids('pending-list').includes(treeC) && fact('Plan-state revision') === shownPlanState,
  `J17 the page drew nothing it was not told: ${treeC} not shown as pending, plan-state still shown as ${shownPlanState}`);
check(allCommandsDisabled(), 'J18 every engine command control is disabled (Add, Remove, Approve, Cancel, Execute, its confirmation, Advance, consent)',
  COMMANDS.filter((id) => !byId(id).disabled));
check(!byId('reread').disabled && alertReread() && !alertReread().disabled && byId('alert').contains(document.activeElement),
  'J19 Re-read is the available action (status bar and alert) and focus is in the alert');
mark = requests.length;
byId('reason').value = 'must never be sent';
byId('add-form').dispatchEvent(new FakeEvent('submit'));
for (const id of ['approve', 'cancel-approved', 'execute', 'execute-confirm', 'advance', 'consent-advance', 'insp-remove']) click(byId(id));
for (const button of removeButtons()) click(button);
const crown = byId('crowns').querySelector('[data-tree-id="' + treeA + '"]');
const crownItem = (await send('GET', 'api/view')).json.scene.find((item) => item.treeId === treeA);
click(crown.childNodes[0], { clientX: crownItem.u, clientY: crownItem.v });
await sleep(300);
check(requests.length === mark, 'J20 while the outcome is unconfirmed, activating every engine command handler (Add, Approve, Cancel, ' +
  'Execute, its confirmation, Advance, consent, Remove) and a map hit-test sends no request at all', requests.slice(mark));
check(!byId('execute-dialog').open && !byId('consent-dialog').open && alertText().includes('Not sent'),
  'J21 no dialog opened, and the alert says what was not sent', alertText());
arm('GET', 'api/view', 'fail');
click(alertReread());
await until(() => requests.length === mark + 1, 'the failing re-read');
await sleep(50);
const afterFailedRead = alertText();
click(byId('advance'));
await sleep(100);
check(requests.length === mark + 1 && allCommandsDisabled() && afterFailedRead.includes('no answer') &&
  afterFailedRead.includes('Outcome unconfirmed'),
  'J22 a failed re-read says so and leaves the lock in place: Advance is still not sent, nothing but reads can be', afterFailedRead);
click(alertReread());
await until(() => ids('pending-list').includes(treeC), 'the successful re-read');
check(fact('Plan-state revision') === String(server.planStateRevision) && !byId('approve').disabled && !byId('approve').hidden && alertText() === '',
  'J23 a successful re-read shows the committed Add, lifts the lock and clears the alert');
click(byId('approve'));
await until(() => ids('approved-list').includes(treeC), 'Approve after the re-read');
check(requests[requests.length - 1].method === 'POST' && requests[requests.length - 1].path === 'api/plan/approve',
  'J24 after the re-read a command is sent again (Approve)');

// ---- D2: an Execute that committed, but whose answer was lost ----------------------------------------------------------
arm('POST', 'api/plan/execute', 'lose');
click(byId('execute'));
await until(() => byId('execute-dialog').open, 'the Execute dialog for C');
click(byId('execute-confirm'));
await until(() => alertText().includes('Outcome unconfirmed'), 'the unconfirmed alert after Execute');
server = await serverView();
check(!living(server, treeC) && server.approved === null, `J25 the server did commit the Execute (${treeC} felled, approval cleared)`);
check(ids('approved-list').includes(treeC) && allCommandsDisabled(),
  'J26 the page still shows the plan as it last knew it, claims no Fell, and every command is disabled');
mark = requests.length;
click(byId('execute'));
click(byId('execute-confirm'));
click(byId('advance'));
await sleep(150);
check(requests.length === mark, 'J27 neither a retry nor any other command can be sent', requests.slice(mark));
click(alertReread());
await until(() => byId('receipt').textContent.includes(treeC), 'the receipt for C');
check(byId('approved-box').hidden && fact('Work revision') === String(server.frame.workRevision) && alertText() === '',
  'J28 the re-read shows the receipt and no approved plan, so nothing is left to execute twice');
server = await serverView();
check(server.frame.events.length === startEvents + 2, 'J29 the server recorded exactly two work events from the page (plans B and C)');
check(pageErrors.length === 0, 'J30 no script error in the page', pageErrors.map(redact));
process.exit(failures === 0 ? 0 : 1);
WITNESS
witness_rc=0
(cd "$scratch" && "$node_bin" page-witness.mjs "$url") > "$scratch/witness.out" 2> "$scratch/witness.err" || witness_rc=$?
sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' "$scratch/witness.out" | tee -a "$log"
if grep -q '^FAIL' "$scratch/witness.out"; then fail=1; fi
check "J31 the page witness ran to the end (exit $witness_rc) with nothing on stderr" \
  bash -c 'test "$1" == 0 && test ! -s "$2"' _ "$witness_rc" "$scratch/witness.err"
if [[ -s "$scratch/witness.err" ]]; then sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g; s/^/    /' "$scratch/witness.err" | tee -a "$log"; fi

all_sockets="$(sockets)"
not_loopback="$(printf '%s\n' "$all_sockets" | awk 'NF { split($3, l, ":"); split($4, r, ":")
  if (l[1] != "0100007F" || (r[1] != "0100007F" && r[1] != "00000000")) print }')"
check "N01 every TCP socket of the process is on 127.0.0.1 (socket count at this moment: $(printf '%s\n' "$all_sockets" | grep -c .))" \
  test -z "$not_loopback"

# ---- stop like Ctrl+C -------------------------------------------------------------------------------------------------------
kill -INT "$server_pid"
for _ in $(seq 1 150); do kill -0 "$server_pid" 2> /dev/null || break; sleep 0.1; done
if kill -0 "$server_pid" 2> /dev/null; then bad "Z01 still running 15 s after SIGINT"; kill -KILL "$server_pid" 2> /dev/null || true; fi
rc=0; wait "$server_pid" 2> /dev/null || rc=$?
server_pid=""
check "Z01 SIGINT ends the process with exit 0 and 'Session ended; nothing was saved.' (exit $rc)" \
  bash -c 'test "$1" == 0 && grep -qx "Session ended; nothing was saved." "$2"' _ "$rc" "$scratch/server.out"
check "Z02 the server wrote nothing to stderr (no unconfirmed outcome, no fault)" test ! -s "$scratch/server.err"
code="$(req z-gone GET "/s/$cap/api/view")"; check "Z03 the port no longer answers after exit (curl status $code)" test "$code" == 000

# ---- nothing written --------------------------------------------------------------------------------------------------------
tree_after="$(tree_snapshot)"
check "W01 no file created, changed or removed anywhere in the lab tree ($(printf '%s\n' "$tree_before" | wc -l) entries, bin/obj/.p0ref included)" \
  test "$tree_before" == "$tree_after"
check "W02 ~/.aspnet unchanged ($([[ "$aspnet_before" == absent ]] && echo 'absent before and after' || echo 'present, identical listing'))" \
  test "$aspnet_before" == "$(aspnet_snapshot)"
check "W03 P0 tracked-file digest unchanged ($p0_before)" test "$p0_before" == "$(p0_digest)"
check "W04 git status of both labs unchanged" test "$status_before" == "$(git_status)"
check "W05 committed fixture sha256 unchanged" test "$(sha256sum "$committed_fixture" | cut -c1-64)" == "$committed_sha"

say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL) ($(grep -c '^PASS' "$log") passed, $(grep -c '^FAIL' "$log") failed)"
exit "$fail"
