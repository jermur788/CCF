#!/usr/bin/env bash
# CCF Multi 2D — Fail-closed self-test for build/2d-gate.sh (SPEC-multi-2d-s0-seam-extraction-V1 §4.5).
#
# Clones the repository locally into an external temporary directory using
# `git clone --shared --no-checkout` + sparse checkout (NEVER touches the real working tree).
# Plants one violation per stage/exit-code class and asserts the exact non-zero exit code
# (1, 2, 3, 4, 5, 64) for every case.
#
# Usage:
#   Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh [--p0ref-dir <dir>] <evidence-dir>
set -euo pipefail

P1_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd -P)"
REPO_ROOT="$(git -C "$P1_ROOT" rev-parse --show-toplevel)"
REPO_ROOT="$(cd "$REPO_ROOT" && pwd -P)"

P0REF_DIR="${CCF_P0REF_DIR:-}"
EVIDENCE_DIR=""

usage() {
  echo "Usage: build/2d-gate/selftest.sh [--p0ref-dir <dir>] <evidence-dir>" >&2
  exit 64
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --p0ref-dir)
      [[ $# -ge 2 ]] || usage
      P0REF_DIR="$2"
      shift 2
      ;;
    --help|-h)
      usage
      ;;
    -*)
      usage
      ;;
    *)
      [[ -z "$EVIDENCE_DIR" ]] || usage
      EVIDENCE_DIR="$1"
      shift
      ;;
  esac
done
[[ -n "$EVIDENCE_DIR" ]] || usage

TMP_BASE="${TMPDIR:-/tmp}"
python3 - "$REPO_ROOT" "$EVIDENCE_DIR" "$TMP_BASE" <<'PY'
import os, sys
repo, ev, tmp = (os.path.realpath(x) for x in sys.argv[1:])
def inside(p, r):
    return p == r or p.startswith(r + os.sep)
if inside(ev, repo):
    sys.exit(f"STOP: evidence dir inside repo: {ev}")
if inside(tmp, repo):
    sys.exit(f"STOP: TMPDIR inside repo: {tmp}")
PY

mkdir -p -- "$EVIDENCE_DIR"
EVIDENCE_DIR="$(cd "$EVIDENCE_DIR" && pwd -P)"
LOG="$EVIDENCE_DIR/selftest.log"
: > "$LOG"
say() { printf '%s\n' "$*" | tee -a "$LOG"; }

SCRATCH="$(mktemp -d "${TMP_BASE}/ccf-2d-gate-selftest.XXXXXXXX")"
cleanup() {
  rm -rf -- "$SCRATCH"
}
trap cleanup EXIT

HEAD_SHA="$(git -C "$REPO_ROOT" rev-parse HEAD)"
BASE_SHA="$(git -C "$REPO_ROOT" rev-parse origin/integration/2d-sim)"
REAL_STATUS_BEFORE="$(git -C "$REPO_ROOT" status --porcelain)"
REAL_HEAD_BEFORE="$HEAD_SHA"

# Discover tracked specs/multi-2d-* paths on HEAD for sparse checkout.
mapfile -t MULTI2D_SPECS < <(git -C "$REPO_ROOT" ls-tree --name-only HEAD specs/ 2>/dev/null | grep '^specs/multi-2d-' | sed 's|^|/|; s|$|/|' || true)

make_sparse_clone() {
  local dest="$1"
  git clone --quiet --shared --no-checkout "$REPO_ROOT" "$dest"
  git -C "$dest" sparse-checkout init --no-cone
  git -C "$dest" sparse-checkout set \
    /Tools/MultiRackSitkaP1Lab/ \
    /Tools/MultiRackSitkaLab/ \
    /Docs/ \
    "${MULTI2D_SPECS[@]}" \
    /Assets/ForestPrototype/SimulationRandom.cs \
    /Assets/ForestPrototype/SitkaGrowthModel.cs \
    /Assets/ForestPrototype/Species/SitkaSpruce.asset \
    /Assets/ForestPrototype/TreeSpeciesDefinition.cs
  git -C "$dest" update-ref refs/remotes/origin/integration/2d-sim "$BASE_SHA"
  git -C "$dest" checkout --quiet -b selftest-work "$HEAD_SHA"
  git -C "$dest" config user.name "CCF Gate Selftest"
  git -C "$dest" config user.email "selftest@local.invalid"
}

GATE_COMMON_ARGS=()
if [[ -n "$P0REF_DIR" ]]; then
  GATE_COMMON_ARGS+=(--p0ref-dir "$P0REF_DIR")
fi

RESULTS_DIR="$SCRATCH/results"
mkdir -p -- "$RESULTS_DIR"

# Run a command verbatim (caller provides all args including any evidence dir) and record verdict.
run_case_raw() {
  local cid="$1"
  local expected_rc="$2"
  local expected_pat="$3"
  local desc="$4"
  shift 4

  local case_out="$EVIDENCE_DIR/case-${cid}.stdout.log"
  local rc=0
  "$@" > "$case_out" 2>&1 || rc=$?

  local ok=1
  local reason=""
  if [[ "$rc" -ne "$expected_rc" ]]; then
    ok=0
    reason="expected exit ${expected_rc}, got ${rc}"
  elif [[ -n "$expected_pat" ]] && ! grep -Eq "$expected_pat" "$case_out"; then
    ok=0
    reason="output missing pattern: ${expected_pat}"
  fi

  if [[ "$ok" -eq 1 ]]; then
    printf 'PASS\t%s\t%s\t%s\t%s\n' "$cid" "$expected_rc" "$rc" "$desc" > "$RESULTS_DIR/${cid}.tsv"
  else
    printf 'FAIL\t%s\t%s\t%s\t%s (%s)\n' "$cid" "$expected_rc" "$rc" "$desc" "$reason" > "$RESULTS_DIR/${cid}.tsv"
  fi
}

# Run a gate invocation with "$EVIDENCE_DIR/case-${cid}" appended as the evidence directory.
run_case_gate() {
  local cid="$1"
  local expected_rc="$2"
  local expected_pat="$3"
  local desc="$4"
  shift 4

  local case_ev="$EVIDENCE_DIR/case-${cid}"
  run_case_raw "$cid" "$expected_rc" "$expected_pat" "$desc" "$@" "$case_ev"
}

say "=== 2d-gate.sh fail-closed selftest ==="
say "HEAD=${HEAD_SHA} BASE=${BASE_SHA} P0REF_DIR=${P0REF_DIR:-<none>}"

# ------------------------------------------------------------------------------
# Fast cases (share one clone, reset cleanly between cases)
# ------------------------------------------------------------------------------
FAST_CLONE="$SCRATCH/clone-fast"
make_sparse_clone "$FAST_CLONE"
FAST_GATE="$FAST_CLONE/Tools/MultiRackSitkaP1Lab/build/2d-gate.sh"

# C01: Bad CLI usage -> exit 64
run_case_gate "C01-usage" 64 "USAGE ERROR" "bad CLI usage flag -> exit 64" \
  bash "$FAST_GATE" --not-a-real-flag

# C02: Mutually exclusive flags -> exit 64
run_case_gate "C02-usage-mutex" 64 "mutually exclusive" "--browser + --headless-only -> exit 64" \
  bash "$FAST_GATE" --browser --headless-only

# C03: Dirty working tree -> exit 2 (G0 REFUSED); use .txt (not .tmp which is git-ignored)
touch "$FAST_CLONE/Tools/MultiRackSitkaP1Lab/untracked-selftest.txt"
run_case_gate "C03-g0-dirty" 2 "GATE G0 FAIL" "dirty working tree -> exit 2" \
  bash "$FAST_GATE" "${GATE_COMMON_ARGS[@]}"
rm -f "$FAST_CLONE/Tools/MultiRackSitkaP1Lab/untracked-selftest.txt"

# C04: Evidence dir inside repository -> exit 2 (G0 REFUSED)
run_case_raw "C04-g0-ev-in-repo" 2 "GATE G0 FAIL" "evidence directory inside repository -> exit 2" \
  bash "$FAST_GATE" "${GATE_COMMON_ARGS[@]}" "$FAST_CLONE/Tools/MultiRackSitkaP1Lab/evidence/selftest-bad"

# C05: Unresolvable base ref -> exit 2 (G0 REFUSED)
run_case_gate "C05-g0-bad-base" 2 "GATE G0 FAIL" "unresolvable --base ref -> exit 2" \
  bash "$FAST_GATE" --base refs/heads/does-not-exist-selftest "${GATE_COMMON_ARGS[@]}"

# C06: Missing required tool (strace absent from PATH) -> exit 3 (PREREQUISITE MISSING)
NO_STRACE_BIN="$SCRATCH/no-strace-bin"
mkdir -p -- "$NO_STRACE_BIN"
for t in bash git dotnet python3 node curl ss env mktemp mkdir rm cp mv cat grep sed awk tr cut head tail find xargs tee sha256sum uname ip unshare dirname basename; do
  if p="$(command -v "$t" 2>/dev/null)"; then
    ln -sf "$p" "$NO_STRACE_BIN/$t"
  fi
done
run_case_gate "C06-g0-missing-tool" 3 "required tool missing on PATH: strace" "missing strace on PATH -> exit 3" \
  env PATH="$NO_STRACE_BIN" bash "$FAST_GATE" "${GATE_COMMON_ARGS[@]}"

# C07: Invalid .p0ref seed (tampered DLL) -> exit 3 (G1 PREREQUISITE MISSING)
BAD_SEED="$SCRATCH/bad-seed"
mkdir -p -- "$BAD_SEED"
printf 'not-a-valid-dll' > "$BAD_SEED/SitkaLab.Core.dll"
printf '0000000000000000000000000000000000000000000000000000000000000000  SitkaLab.Core.dll\n' > "$BAD_SEED/SitkaLab.Core.dll.sha256"
printf 'p0QualifiedCommit=bad\n' > "$BAD_SEED/provenance.txt"
run_case_gate "C07-g1-bad-seed" 3 "GATE G1 FAIL" "invalid .p0ref seed -> exit 3" \
  bash "$FAST_GATE" --p0ref-dir "$BAD_SEED"

# C08: Compile error in Core -> exit 1 (G1 FAIL)
printf '\n#error selftest planted G1 compile failure\n' >> "$FAST_CLONE/Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs"
git -C "$FAST_CLONE" add Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs
git -C "$FAST_CLONE" commit --quiet -m "selftest: plant G1 compile error"
run_case_gate "C08-g1-compile" 1 "GATE G1 FAIL" "Core compile error -> exit 1" \
  bash "$FAST_GATE" "${GATE_COMMON_ARGS[@]}"
git -C "$FAST_CLONE" reset --quiet --hard "$HEAD_SHA"
git -C "$FAST_CLONE" clean --quiet -fdx

# C09: Deleted Core test (pass count below baseline) -> exit 1 (G2 FAIL)
python3 - "$FAST_CLONE/Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Model0FingerprintTests.cs" <<'PY'
import sys
p = sys.argv[1]
s = open(p, "r", encoding="utf-8").read()
# Skip the first T.Add(...) registration dynamically so TreatWarningsAsErrors produces 0 warnings in G1 and G2 catches the dropped test.
idx = s.find('T.Add(')
assert idx != -1
s2 = s[:idx] + 'if (System.Environment.GetEnvironmentVariable("CCF_SELFTEST_NEVER") == "1") ' + s[idx:]
open(p, "w", encoding="utf-8").write(s2)
PY
git -C "$FAST_CLONE" add Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Model0FingerprintTests.cs
git -C "$FAST_CLONE" commit --quiet -m "selftest: plant G2 dropped Core test"
run_case_gate "C09-g2-dropped-test" 1 "GATE G2 FAIL" "dropped Core test below baseline -> exit 1" \
  bash "$FAST_GATE" "${GATE_COMMON_ARGS[@]}"
git -C "$FAST_CLONE" reset --quiet --hard "$HEAD_SHA"
git -C "$FAST_CLONE" clean --quiet -fdx

say "Fast cases C01-C09 completed; running full end-to-end stage cases C10-C15 sequentially (foreground, zero CPU contention)..."

# ------------------------------------------------------------------------------
# Full end-to-end cases past G2 (each gets its own isolated sparse clone, run sequentially in foreground)
# ------------------------------------------------------------------------------
run_full_case() {
  local cid="$1"
  local expected_rc="$2"
  local expected_pat="$3"
  local desc="$4"
  local mutate_fn="$5"
  shift 5

  local clone="$SCRATCH/clone-${cid}"
  make_sparse_clone "$clone"
  "$mutate_fn" "$clone"
  run_case_gate "$cid" "$expected_rc" "$expected_pat" "$desc" \
    bash "$clone/Tools/MultiRackSitkaP1Lab/build/2d-gate.sh" "${GATE_COMMON_ARGS[@]}" "$@"
  rm -rf -- "$clone"
}

mut_g3_anchor() {
  local clone="$1"
  python3 - "$clone/Tools/MultiRackSitkaP1Lab/build/2d-gate/frozen-anchors.json" <<'PY'
import json, sys
p = sys.argv[1]
d = json.load(open(p, "r", encoding="utf-8"))
d["anchors"][0]["sha256"] = "0" * 64
json.dump(d, open(p, "w", encoding="utf-8"), indent=2)
PY
  git -C "$clone" add Tools/MultiRackSitkaP1Lab/build/2d-gate/frozen-anchors.json
  git -C "$clone" commit --quiet -m "selftest: tamper anchor #1 manifest"
}

mut_g4_phase1() {
  local clone="$1"
  printf '\n// selftest planted Phase1 edit\n' >> "$clone/Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs"
  git -C "$clone" add Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs
  git -C "$clone" commit --quiet -m "selftest: edit frozen src/Core/Phase1"
}

mut_g6_f1_token() {
  local clone="$1"
  cat > "$clone/Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/SelftestNondeterminism.cs" <<'CS'
namespace SitkaP1.Core;

internal static class SelftestNondeterminism
{
    internal static int Bad() => new System.Random(0).Next();
}
CS
  git -C "$clone" add Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/SelftestNondeterminism.cs
  git -C "$clone" commit --quiet -m "selftest: plant F1 new Random in Core"
}

mut_g7_assets() {
  local clone="$1"
  mkdir -p -- "$clone/Assets"
  printf 'selftest protected-path violation\n' > "$clone/Assets/selftest-violation.txt"
  git -C "$clone" add --sparse -f Assets/selftest-violation.txt
  git -C "$clone" commit --quiet -m "selftest: touch forbidden Assets/"
}

mut_missing_marker() {
  local clone="$1"
  python3 - "$clone/Tools/MultiRackSitkaP1Lab/build/2d-gate.sh" <<'PY'
import sys
p = sys.argv[1]
s = open(p, "r", encoding="utf-8").read()
target = '  emit_marker G7 PASS'
assert target in s
s = s.replace(target, '  STAGE_STATUS[G7]="PASS" # selftest: suppressed GATE G7 PASS marker', 1)
open(p, "w", encoding="utf-8").write(s)
PY
  git -C "$clone" add Tools/MultiRackSitkaP1Lab/build/2d-gate.sh
  git -C "$clone" commit --quiet -m "selftest: suppress GATE G7 PASS marker"
}

mut_noop() {
  :
}

run_full_case "C10-g3-anchor" 1 "GATE G3 FAIL" "tampered anchor manifest -> exit 1" mut_g3_anchor
run_full_case "C11-g4-phase1" 1 "GATE G4 FAIL" "frozen src/Core/Phase1 edit -> exit 1" mut_g4_phase1
run_full_case "C12-g6-f1-random" 1 "GATE G6 FAIL" "planted F1 new Random() in Core -> exit 1" mut_g6_f1_token
run_full_case "C13-g7-assets" 4 "GATE G7 FAIL" "planted file under Assets/ -> exit 4" mut_g7_assets
run_full_case "C14-missing-marker" 1 "fail-closed marker check failed" "suppressed GATE G7 PASS marker -> exit 1" mut_missing_marker
run_full_case "C15-headless-only" 5 "VERDICT: PARTIAL" "--headless-only partial mode -> exit 5" mut_noop --headless-only

# Verify real working tree was never touched.
REAL_STATUS_AFTER="$(git -C "$REPO_ROOT" status --porcelain)"
REAL_HEAD_AFTER="$(git -C "$REPO_ROOT" rev-parse HEAD)"
if [[ "$REAL_STATUS_AFTER" != "$REAL_STATUS_BEFORE" || "$REAL_HEAD_AFTER" != "$REAL_HEAD_BEFORE" ]]; then
  say "FAIL real working tree changed during selftest!"
  exit 1
fi

# Collate results and check coverage of exit codes {1, 2, 3, 4, 5, 64}.
python3 - "$RESULTS_DIR" "$EVIDENCE_DIR/selftest-summary.json" <<'PY' | tee -a "$LOG"
import glob, json, os, sys

res_dir, out_json = sys.argv[1], sys.argv[2]
rows = []
for path in sorted(glob.glob(os.path.join(res_dir, "*.tsv"))):
    parts = open(path, "r", encoding="utf-8").read().strip().split("\t")
    rows.append({
        "status": parts[0],
        "id": parts[1],
        "expectedExit": int(parts[2]),
        "actualExit": int(parts[3]),
        "description": parts[4],
    })

passed = sum(1 for r in rows if r["status"] == "PASS")
failed = sum(1 for r in rows if r["status"] != "PASS")
covered_codes = sorted({r["actualExit"] for r in rows if r["status"] == "PASS"})
required_codes = [1, 2, 3, 4, 5, 64]
missing_codes = [c for c in required_codes if c not in covered_codes]

for r in rows:
    print(f"{r['status']} {r['id']} (expected={r['expectedExit']} actual={r['actualExit']}): {r['description']}")

summary = {
    "total": len(rows),
    "passed": passed,
    "failed": failed,
    "coveredExitCodes": covered_codes,
    "missingExitCodes": missing_codes,
    "cases": rows,
}
with open(out_json, "w", encoding="utf-8") as f:
    json.dump(summary, f, indent=2)
    f.write("\n")

print(f"Covered exit codes: {covered_codes}")
if failed > 0 or missing_codes or len(rows) != 15:
    print(f"OVERALL: FAIL ({passed}/{len(rows)} passed, {failed} failed, missingCodes={missing_codes})")
    sys.exit(1)
print(f"OVERALL: PASS ({passed}/{len(rows)} passed, 0 failed; all exit codes 1,2,3,4,5,64 proven)")
PY
