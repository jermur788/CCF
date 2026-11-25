#!/usr/bin/env bash
# CCF Multi 2D — Fail-closed 2D integration trunk gate (SPEC-multi-2d-s0-seam-extraction-V1 §4.2–§4.6).
#
# Usage:
#   Tools/MultiRackSitkaP1Lab/build/2d-gate.sh [--base <ref>] [--p0ref-dir <dir>] [--browser] [--headless-only] <evidence-dir>
#
# Exit codes (spec §4.3):
#   0  PASS: every stage G0-G7 passed (and B when --browser was requested)
#   1  FAIL: a check ran and failed (includes unexpected termination or missing GATE marker)
#   2  REFUSED: preflight precondition (dirty tree, TMPDIR/evidence inside repo, wrong root, unresolvable base)
#   3  PREREQUISITE MISSING: required tool or host capability absent (incl. invalid/missing .p0ref seed or Edge)
#   4  PROTECTED-PATH VIOLATION (G7)
#   5  PARTIAL: only with explicit --headless-only and no --browser when G0-G7 passed and browser tier is skipped
#   64 Usage error
set -euo pipefail

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

SCRIPT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd -P)/$(basename "${BASH_SOURCE[0]}")"
P1_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
GATE_DATA_DIR="$P1_ROOT/build/2d-gate"

# Internal netns worker mode (only when invoked by this script itself under unshare -rn).
INTERNAL_MODE=0
if [[ "${1:-}" == "--__internal-g1-g2" ]]; then
  if [[ "${CCF_2D_GATE_INTERNAL:-0}" != "1" ]]; then
    echo "USAGE ERROR: unknown option --__internal-g1-g2" >&2
    exit 64
  fi
  INTERNAL_MODE=1
  shift
fi

BASE_REF="origin/integration/2d-sim"
P0REF_DIR="${CCF_P0REF_DIR:-}"
WITH_BROWSER=0
HEADLESS_ONLY=0
EVIDENCE_DIR=""

usage() {
  cat >&2 <<'EOF'
Usage: build/2d-gate.sh [--base <ref>] [--p0ref-dir <dir>] [--browser] [--headless-only] <evidence-dir>
EOF
  exit 64
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --base)
      [[ $# -ge 2 ]] || usage
      BASE_REF="$2"
      shift 2
      ;;
    --p0ref-dir)
      [[ $# -ge 2 ]] || usage
      P0REF_DIR="$2"
      shift 2
      ;;
    --browser)
      WITH_BROWSER=1
      shift
      ;;
    --headless-only)
      HEADLESS_ONLY=1
      shift
      ;;
    --help|-h)
      usage
      ;;
    -*)
      echo "USAGE ERROR: unknown option: $1" >&2
      usage
      ;;
    *)
      if [[ -n "$EVIDENCE_DIR" ]]; then
        echo "USAGE ERROR: unexpected extra argument: $1" >&2
        usage
      fi
      EVIDENCE_DIR="$1"
      shift
      ;;
  esac
done

[[ -n "$EVIDENCE_DIR" ]] || usage
if [[ "$WITH_BROWSER" -eq 1 && "$HEADLESS_ONLY" -eq 1 ]]; then
  echo "USAGE ERROR: --browser and --headless-only are mutually exclusive" >&2
  usage
fi

# State tracked for fail-closed cleanup & summary.
GATE_CLEAN_EXIT=0
GATE_ABORT_CODE=1
GATE_LOG=""
SCRATCH_DIR=""
REPO_ROOT=""
BASE_COMMIT=""
HEAD_SHA=""
NET_ISOLATION="NOT PROVEN"
P0_MODE="UNKNOWN"
HOST_NA_LIST=()
P0REF_BACKUP_DIR=""

declare -A STAGE_STATUS=(
  [G0]="NOT RUN"
  [G1]="NOT RUN"
  [G2]="NOT RUN"
  [G3]="NOT RUN"
  [G4]="NOT RUN"
  [G5]="NOT RUN"
  [G6]="NOT RUN"
  [G7]="NOT RUN"
  [B]="NOT RUN"
)

log_line() {
  local msg="$*"
  if [[ -n "$GATE_LOG" ]]; then
    printf '%s\n' "$msg" | tee -a "$GATE_LOG"
  else
    printf '%s\n' "$msg"
  fi
}

emit_marker() {
  local stage="$1"
  local status="$2"
  local detail="${3:-}"
  STAGE_STATUS["$stage"]="$status"
  if [[ -n "$detail" ]]; then
    log_line "GATE ${stage} ${status} (${detail})"
  fi
  log_line "GATE ${stage} ${status}"
}

restore_p0ref_backup_if_needed() {
  if [[ -n "$P0REF_BACKUP_DIR" && -d "$P0REF_BACKUP_DIR" && -n "$P1_ROOT" ]]; then
    local target="$P1_ROOT/.p0ref"
    if [[ -L "$target" ]]; then
      rm -f -- "$target"
    elif [[ -e "$target" ]]; then
      rm -rf -- "$target"
    fi
    mkdir -p -- "$target"
    cp --no-dereference "$P0REF_BACKUP_DIR/SitkaLab.Core.dll" "$target/SitkaLab.Core.dll"
    cp --no-dereference "$P0REF_BACKUP_DIR/SitkaLab.Core.dll.sha256" "$target/SitkaLab.Core.dll.sha256"
    cp --no-dereference "$P0REF_BACKUP_DIR/provenance.txt" "$target/provenance.txt"
  fi
}

write_summary_json() {
  local final_code="$1"
  local final_verdict="$2"
  [[ -n "$GATE_LOG" && -n "$EVIDENCE_DIR" && -d "$EVIDENCE_DIR" ]] || return 0
  local host_na_lines
  host_na_lines="$(printf '%s\n' "${HOST_NA_LIST[@]:-}")"
  python3 - "$EVIDENCE_DIR/gate-summary.json" "$final_code" "$final_verdict" \
    "$REPO_ROOT" "$HEAD_SHA" "$BASE_REF" "$BASE_COMMIT" "$NET_ISOLATION" "$P0_MODE" \
    "${STAGE_STATUS[G0]}" "${STAGE_STATUS[G1]}" "${STAGE_STATUS[G2]}" "${STAGE_STATUS[G3]}" \
    "${STAGE_STATUS[G4]}" "${STAGE_STATUS[G5]}" "${STAGE_STATUS[G6]}" "${STAGE_STATUS[G7]}" \
    "${STAGE_STATUS[B]}" "$WITH_BROWSER" "$HEADLESS_ONLY" "$host_na_lines" <<'PY' || true
import datetime, json, sys
(
    out_path, code, verdict, repo, head, base_ref, base_commit, net_iso, p0_mode,
    g0, g1, g2, g3, g4, g5, g6, g7, sb, with_browser, headless_only, host_na_raw
) = sys.argv[1:]
summary = {
    "timestampUtc": datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
    "exitCode": int(code),
    "verdict": verdict,
    "repoRoot": repo,
    "headCommit": head,
    "baseRef": base_ref,
    "baseCommit": base_commit,
    "networkIsolation": net_iso,
    "p0ReferenceMode": p0_mode,
    "withBrowser": with_browser == "1",
    "headlessOnly": headless_only == "1",
    "stages": {
        "G0": g0,
        "G1": g1,
        "G2": g2,
        "G3": g3,
        "G4": g4,
        "G5": g5,
        "G6": g6,
        "G7": g7,
        "B": sb,
    },
    "hostNa": [x for x in host_na_raw.splitlines() if x],
}
with open(out_path, "w", encoding="utf-8") as f:
    json.dump(summary, f, indent=2)
    f.write("\n")
PY
}

on_exit() {
  local rc=$?
  set +e
  restore_p0ref_backup_if_needed
  if [[ "$INTERNAL_MODE" -eq 0 ]]; then
    if [[ "$GATE_CLEAN_EXIT" -ne 1 ]]; then
      if [[ "$rc" -eq 0 ]]; then
        rc=1
      fi
      log_line "GATE UNEXPECTED_TERMINATION FAIL (exit=${rc})"
      for st in G0 G1 G2 G3 G4 G5 G6 G7 B; do
        if [[ "${STAGE_STATUS[$st]}" == "NOT RUN" ]]; then
          log_line "GATE ${st} NOT RUN"
        fi
      done
      write_summary_json "$rc" "FAIL"
    fi
  fi
  if [[ -n "$SCRATCH_DIR" && -d "$SCRATCH_DIR" ]]; then
    rm -rf -- "$SCRATCH_DIR"
  fi
  exit "$rc"
}

trap on_exit EXIT

fail_stage() {
  local stage="$1"
  local code="$2"
  local reason="$3"
  STAGE_STATUS["$stage"]="FAIL"
  log_line "ERROR [${stage}]: ${reason}"
  log_line "GATE ${stage} FAIL"
  if [[ "$INTERNAL_MODE" -eq 0 ]]; then
    for st in G0 G1 G2 G3 G4 G5 G6 G7 B; do
      if [[ "${STAGE_STATUS[$st]}" == "NOT RUN" ]]; then
        log_line "GATE ${st} NOT RUN"
      fi
    done
    local verdict="FAIL"
    case "$code" in
      2) verdict="REFUSED" ;;
      3) verdict="PREREQUISITE MISSING" ;;
      4) verdict="PROTECTED-PATH VIOLATION" ;;
    esac
    log_line "VERDICT: ${verdict} (exit=${code})"
    write_summary_json "$code" "$verdict"
    GATE_CLEAN_EXIT=1
  fi
  exit "$code"
}

p0_tracked_digest() {
  (cd "$REPO_ROOT" && git ls-files -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum | cut -c1-64)
}

validate_seed_dir() {
  local dir="$1"
  local anchors_json="$GATE_DATA_DIR/frozen-anchors.json"
  python3 - "$dir" "$anchors_json" <<'PY'
import hashlib, json, os, stat, sys

seed_dir, anchors_path = sys.argv[1], sys.argv[2]
with open(anchors_path, "r", encoding="utf-8") as f:
    manifest = json.load(f)

expected_commit = manifest["p0QualifiedCommit"]
expected_tree = manifest["p0QualifiedTree"]
expected_sha = next(a["sha256"] for a in manifest["anchors"] if a["id"] == 8)

if not os.path.exists(seed_dir) or os.path.islink(seed_dir) or not os.path.isdir(seed_dir):
    sys.exit(f"seed directory missing, symlinked, or not a directory: {seed_dir}")

required = ["SitkaLab.Core.dll", "SitkaLab.Core.dll.sha256", "provenance.txt"]
for name in required:
    p = os.path.join(seed_dir, name)
    if not os.path.lexists(p):
        sys.exit(f"seed missing file: {p}")
    st = os.lstat(p)
    if stat.S_ISLNK(st.st_mode):
        sys.exit(f"seed file is a symlink: {p}")
    if not stat.S_ISREG(st.st_mode):
        sys.exit(f"seed file is not a regular file: {p}")
    if st.st_nlink != 1:
        sys.exit(f"seed file has nlink={st.st_nlink} != 1: {p}")

dll_path = os.path.join(seed_dir, "SitkaLab.Core.dll")
actual_sha = hashlib.sha256(open(dll_path, "rb").read()).hexdigest()
if actual_sha != expected_sha:
    sys.exit(f"seed SitkaLab.Core.dll sha256={actual_sha} != expected {expected_sha}")

sha_file_text = open(os.path.join(seed_dir, "SitkaLab.Core.dll.sha256"), "r", encoding="utf-8").read().strip()
if not sha_file_text.startswith(expected_sha):
    sys.exit(f"seed SitkaLab.Core.dll.sha256 mismatch: {sha_file_text!r}")

prov_lines = set(open(os.path.join(seed_dir, "provenance.txt"), "r", encoding="utf-8").read().splitlines())
needed_lines = [
    f"p0QualifiedCommit={expected_commit}",
    f"p0QualifiedTree={expected_tree}",
    f"dllSha256={expected_sha}",
]
for line in needed_lines:
    if line not in prov_lines:
        sys.exit(f"seed provenance.txt missing exact line: {line}")

print(f"SEED_VALID sha256={actual_sha}")
PY
}

# ==============================================================================
# Stage G0 — Preflight (environment only; no checks belonging to G1-G7)
# ==============================================================================
run_stage_G0() {
  # Verify inside a git checkout and Tools/MultiRackSitkaP1Lab is a real directory.
  if ! REPO_ROOT="$(git -C "$P1_ROOT" rev-parse --show-toplevel 2>/dev/null)"; then
    fail_stage G0 2 "not inside a Git repository"
  fi
  REPO_ROOT="$(cd "$REPO_ROOT" && pwd -P)"
  if [[ ! -d "$REPO_ROOT/Tools/MultiRackSitkaP1Lab" || -L "$REPO_ROOT/Tools/MultiRackSitkaP1Lab" ]]; then
    fail_stage G0 2 "Tools/MultiRackSitkaP1Lab is missing or a symlink"
  fi
  local real_p1
  real_p1="$(cd "$REPO_ROOT/Tools/MultiRackSitkaP1Lab" && pwd -P)"
  if [[ "$real_p1" != "$P1_ROOT" ]]; then
    fail_stage G0 2 "script P1_ROOT ($P1_ROOT) != repo Tools/MultiRackSitkaP1Lab ($real_p1)"
  fi

  # Verify TMPDIR and evidence directory are strictly outside the repository.
  local tmp_base="${TMPDIR:-/tmp}"
  if ! command -v python3 >/dev/null 2>&1; then
    fail_stage G0 3 "required tool missing on PATH: python3"
  fi
  local ev_check
  if ! ev_check="$(python3 - "$REPO_ROOT" "$EVIDENCE_DIR" "$tmp_base" <<'PY'
import os, sys
repo = os.path.realpath(sys.argv[1])
ev = os.path.realpath(sys.argv[2])
tmp = os.path.realpath(sys.argv[3])
def inside(path, root):
    return path == root or path.startswith(root + os.sep)
if inside(ev, repo):
    sys.exit(f"evidence directory is inside repository: {ev}")
if inside(tmp, repo):
    sys.exit(f"TMPDIR is inside repository: {tmp}")
print(ev)
PY
  )"; then
    fail_stage G0 2 "$ev_check"
  fi

  mkdir -p -- "$EVIDENCE_DIR"
  EVIDENCE_DIR="$(cd "$EVIDENCE_DIR" && pwd -P)"
  GATE_LOG="$EVIDENCE_DIR/gate.log"
  : > "$GATE_LOG"

  SCRATCH_DIR="$(mktemp -d "${tmp_base}/ccf-2d-gate.XXXXXXXX")"

  # Check dirty working tree (G0 precondition -> exit 2).
  local dirty
  dirty="$(git -C "$REPO_ROOT" status --porcelain)"
  if [[ -n "$dirty" ]]; then
    printf '%s\n' "$dirty" > "$EVIDENCE_DIR/g0-dirty.txt"
    fail_stage G0 2 "working tree is dirty (see g0-dirty.txt)"
  fi

  # Check required data files exist.
  for req_data in frozen-anchors.json protected-paths.txt determinism-allowlist.txt baseline-counts.json; do
    if [[ ! -f "$GATE_DATA_DIR/$req_data" ]]; then
      fail_stage G0 2 "missing gate data file: $GATE_DATA_DIR/$req_data"
    fi
  done

  # Resolve base ref (G0 precondition -> exit 2).
  if ! git -C "$REPO_ROOT" rev-parse --verify "${BASE_REF}^{commit}" >/dev/null 2>&1; then
    fail_stage G0 2 "unresolvable base ref: $BASE_REF"
  fi
  if ! BASE_COMMIT="$(git -C "$REPO_ROOT" merge-base HEAD "$BASE_REF" 2>/dev/null)"; then
    fail_stage G0 2 "cannot compute merge-base of HEAD and $BASE_REF"
  fi
  HEAD_SHA="$(git -C "$REPO_ROOT" rev-parse HEAD)"

  # Check required tools on PATH (exit 3 on missing).
  local tool
  for tool in git bash dotnet python3 node strace curl ss; do
    if ! command -v "$tool" >/dev/null 2>&1; then
      fail_stage G0 3 "required tool missing on PATH: $tool"
    fi
  done

  # Verify dotnet SDK satisfies Tools/MultiRackSitkaP1Lab/global.json.
  local dotnet_ver
  dotnet_ver="$(dotnet --version 2>/dev/null || true)"
  local sdk_err
  if ! sdk_err="$(python3 - "$P1_ROOT/global.json" "$dotnet_ver" <<'PY'
import json, re, sys
with open(sys.argv[1], "r", encoding="utf-8") as f:
    gj = json.load(f)
want = gj["sdk"]["version"]
got = sys.argv[2].strip()
m_w = re.match(r"^(\d+)\.(\d+)\.(\d+)", want)
m_g = re.match(r"^(\d+)\.(\d+)\.(\d+)", got)
if not m_w or not m_g:
    sys.exit(f"unparseable dotnet SDK version: got={got!r}, global.json={want!r}")
w_maj, w_min, w_pat = map(int, m_w.groups())
g_maj, g_min, g_pat = map(int, m_g.groups())
if (g_maj, g_min) != (w_maj, w_min) or (g_pat // 100) != (w_pat // 100) or g_pat < w_pat:
    sys.exit(f"dotnet SDK {got} does not satisfy global.json ({want} latestPatch)")
print(got)
PY
  )"; then
    fail_stage G0 3 "$sdk_err"
  fi

  # If --browser was requested, verify Windows Edge + Windows Node prerequisites now (exit 3).
  if [[ "$WITH_BROWSER" -eq 1 ]]; then
    local win_edge="/mnt/c/Program Files (x86)/Microsoft/Edge/Application/msedge.exe"
    local win_node="/mnt/c/Program Files/nodejs/node.exe"
    if [[ ! -x "$win_edge" ]]; then
      fail_stage G0 3 "browser prerequisite missing: $win_edge"
    fi
    if [[ ! -x "$win_node" ]]; then
      fail_stage G0 3 "browser prerequisite missing: $win_node"
    fi
  fi

  # Record protected local listeners OUTSIDE any network namespace.
  ss -tln > "$EVIDENCE_DIR/listeners-before.txt"

  # Record tool versions and preflight metadata.
  python3 - "$EVIDENCE_DIR/preflight.json" "$REPO_ROOT" "$HEAD_SHA" "$BASE_REF" "$BASE_COMMIT" \
    "$dotnet_ver" "$(node --version)" "$(python3 --version)" "$(git --version)" \
    "$(strace -V 2>&1 | head -n1)" "$(uname -a)" <<'PY'
import datetime, json, sys
out, repo, head, bref, bcom, dver, nver, pver, gver, sver, un = sys.argv[1:]
with open(out, "w", encoding="utf-8") as f:
    json.dump({
        "timestampUtc": datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "repoRoot": repo,
        "head": head,
        "baseRef": bref,
        "baseCommit": bcom,
        "tools": {
            "dotnet": dver,
            "node": nver,
            "python3": pver,
            "git": gver,
            "strace": sver,
            "uname": un,
        },
    }, f, indent=2)
    f.write("\n")
PY

  emit_marker G0 PASS
}

# ==============================================================================
# Stage G1 — Clean build, 0 warnings / 0 errors
# ==============================================================================
run_stage_G1() {
  # Remove ONLY P1-owned bin/ and obj/ directories under src/ and tests/.
  find "$P1_ROOT/src" "$P1_ROOT/tests" -type d \( -name bin -o -name obj \) -prune -exec rm -rf -- {} +

  # Check P0 tree ID on HEAD matches frozen pin.
  local expected_p0_tree expected_p0_sha
  expected_p0_tree="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["p0QualifiedTree"])' "$GATE_DATA_DIR/frozen-anchors.json")"
  expected_p0_sha="$(python3 -c 'import json,sys; d=json.load(open(sys.argv[1])); print(next(a["sha256"] for a in d["anchors"] if a["id"]==8))' "$GATE_DATA_DIR/frozen-anchors.json")"

  local actual_p0_tree
  actual_p0_tree="$(git -C "$REPO_ROOT" rev-parse HEAD:Tools/MultiRackSitkaLab 2>/dev/null || true)"
  if [[ "$actual_p0_tree" != "$expected_p0_tree" ]]; then
    fail_stage G1 1 "HEAD:Tools/MultiRackSitkaLab tree=${actual_p0_tree} != pinned ${expected_p0_tree}"
  fi

  # Acquire verified .p0ref/SitkaLab.Core.dll (spec §4.6 Option A).
  if [[ -n "$P0REF_DIR" ]]; then
    local val_out
    if ! val_out="$(validate_seed_dir "$P0REF_DIR" 2>&1)"; then
      fail_stage G1 3 "invalid P0 seed (${P0REF_DIR}): ${val_out}"
    fi
    log_line "G1: validated P0 seed: ${val_out}"
    local target="$P1_ROOT/.p0ref"
    if [[ -L "$target" ]]; then
      fail_stage G1 1 "refusing to overwrite symlinked $target"
    fi
    rm -rf -- "$target"
    mkdir -p -- "$target"
    cp --no-dereference "$P0REF_DIR/SitkaLab.Core.dll" "$target/SitkaLab.Core.dll"
    cp --no-dereference "$P0REF_DIR/SitkaLab.Core.dll.sha256" "$target/SitkaLab.Core.dll.sha256"
    cp --no-dereference "$P0REF_DIR/provenance.txt" "$target/provenance.txt"
    local installed_sha
    installed_sha="$(sha256sum "$target/SitkaLab.Core.dll" | cut -c1-64)"
    if [[ "$installed_sha" != "$expected_p0_sha" ]]; then
      fail_stage G1 3 "installed .p0ref/SitkaLab.Core.dll sha256=${installed_sha} != expected ${expected_p0_sha}"
    fi
    P0_MODE="PRE-SEEDED VERIFIED ${expected_p0_sha}"
  else
    log_line "G1: no CCF_P0REF_DIR/--p0ref-dir provided; attempting cold build via build/build-p0-core-ref.sh"
    local cold_rc=0
    bash "$P1_ROOT/build/build-p0-core-ref.sh" > "$EVIDENCE_DIR/g1-build-p0-ref.log" 2>&1 || cold_rc=$?
    local cold_sha=""
    if [[ -f "$P1_ROOT/.p0ref/SitkaLab.Core.dll" ]]; then
      cold_sha="$(sha256sum "$P1_ROOT/.p0ref/SitkaLab.Core.dll" | cut -c1-64)"
    fi
    if [[ "$cold_rc" -ne 0 || "$cold_sha" != "$expected_p0_sha" ]]; then
      fail_stage G1 3 "cold build of .p0ref/SitkaLab.Core.dll produced sha=${cold_sha:-MISSING} (rc=${cold_rc}) != frozen pin ${expected_p0_sha}; supply verified CCF_P0REF_DIR/--p0ref-dir per spec §4.6"
    fi
    P0_MODE="COLD-REBUILT VERIFIED ${expected_p0_sha}"
  fi

  # Check SitkaP1.Ui.csproj EmbeddedResource LogicalNames have no duplicates (plan §2.4).
  local dup_err
  if ! dup_err="$(python3 - "$P1_ROOT/src/Ui/SitkaP1.Ui.csproj" <<'PY'
import sys, xml.etree.ElementTree as ET
tree = ET.parse(sys.argv[1])
seen = {}
for elem in tree.iter("EmbeddedResource"):
    ln = elem.attrib.get("LogicalName")
    inc = elem.attrib.get("Include", "")
    if ln:
        if ln in seen:
            sys.exit(f"duplicate EmbeddedResource LogicalName {ln!r} ({seen[ln]} vs {inc})")
        seen[ln] = inc
PY
  )"; then
    fail_stage G1 1 "$dup_err"
  fi

  # Create an offline NuGet.Config with zero remote sources so builds never touch the network
  # (works under unshare -rn and fails closed if any PackageReference is ever added).
  local offline_nuget="$SCRATCH_DIR/offline-nuget.config"
  cat > "$offline_nuget" <<'XML'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
  </packageSources>
</configuration>
XML
  export RestoreConfigFile="$offline_nuget"

  # Run the three required clean builds with --disable-build-servers; enforce 0 Warning(s) and 0 Error(s).
  local build_targets=(
    "sln:MultiRackSitkaP1Lab.sln"
    "ui:src/Ui/SitkaP1.Ui.csproj"
    "save:tests/Phase1.Save.Tests/Phase1Save.Tests.csproj"
  )
  local item label proj log_file
  for item in "${build_targets[@]}"; do
    label="${item%%:*}"
    proj="${item#*:}"
    log_file="$EVIDENCE_DIR/g1-build-${label}.log"
    if ! (cd "$P1_ROOT" && dotnet build "$proj" --disable-build-servers -p:RestoreConfigFile="$offline_nuget") > "$log_file" 2>&1; then
      tail -n 40 "$log_file" >&2 || true
      fail_stage G1 1 "dotnet build failed for ${proj} (see g1-build-${label}.log)"
    fi
    local warn_err
    if ! warn_err="$(python3 - "$log_file" "$proj" <<'PY'
import re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
proj = sys.argv[2]
warns = re.findall(r"(\d+)\s+Warning\(s\)", text)
errs = re.findall(r"(\d+)\s+Error\(s\)", text)
if not warns or not errs:
    sys.exit(f"{proj}: MSBuild summary missing Warning(s)/Error(s) lines")
if any(int(w) != 0 for w in warns):
    sys.exit(f"{proj}: non-zero Warning(s): {warns}")
if any(int(e) != 0 for e in errs):
    sys.exit(f"{proj}: non-zero Error(s): {errs}")
PY
    )"; then
      fail_stage G1 1 "$warn_err"
    fi
  done

  emit_marker G1 PASS "P0 REFERENCE: ${P0_MODE}"
}

# ==============================================================================
# Stage G2 — All Core / Save / UI / Safety test suites + Packet sub-checks
# ==============================================================================
run_stage_G2() {
  local baseline_json="$GATE_DATA_DIR/baseline-counts.json"
  local anchors_json="$GATE_DATA_DIR/frozen-anchors.json"
  local expected_p0_tree expected_p0_sha expected_p0_digest
  expected_p0_tree="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["p0QualifiedTree"])' "$anchors_json")"
  expected_p0_sha="$(python3 -c 'import json,sys; d=json.load(open(sys.argv[1])); print(next(a["sha256"] for a in d["anchors"] if a["id"]==8))' "$anchors_json")"
  expected_p0_digest="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["p0TrackedFilesDigest"])' "$anchors_json")"

  # Capture P0-untouched BEFORE state (spec §4.6 item 4).
  local p0_digest_before
  p0_digest_before="$(p0_tracked_digest)"
  if [[ "$p0_digest_before" != "$expected_p0_digest" ]]; then
    fail_stage G2 1 "P0 tracked digest before G2=${p0_digest_before} != pinned ${expected_p0_digest}"
  fi
  git -C "$REPO_ROOT" status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab \
    > "$EVIDENCE_DIR/g2-p0-status-before.txt"

  # 1. Core.Tests (stdout captured for G3)
  log_line "G2: running Core.Tests..."
  local core_log="$EVIDENCE_DIR/g2-core-tests.log"
  if ! dotnet "$P1_ROOT/tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll" > "$core_log" 2>&1; then
    tail -n 40 "$core_log" >&2 || true
    fail_stage G2 1 "Core.Tests exited non-zero (see g2-core-tests.log)"
  fi
  local check_out
  if ! check_out="$(python3 - "$core_log" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["core"]
matches = re.findall(cfg["summaryRegex"], text, re.MULTILINE)
if not matches:
    sys.exit("Core.Tests summary line missing")
passed, total, failed = map(int, matches[-1])
if failed > cfg["maxFailed"] or passed != total or passed < cfg["minPassed"]:
    sys.exit(f"Core.Tests count check failed: {passed}/{total} passed, {failed} failed (minPassed={cfg['minPassed']})")
print(f"Core.Tests PASS ({passed}/{total} passed, {failed} failed)")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # 2. Phase1.Save.Tests
  log_line "G2: running Phase1.Save.Tests..."
  local save_log="$EVIDENCE_DIR/g2-save-tests.log"
  if ! dotnet "$P1_ROOT/tests/Phase1.Save.Tests/bin/Debug/net8.0/Phase1Save.Tests.dll" > "$save_log" 2>&1; then
    tail -n 40 "$save_log" >&2 || true
    fail_stage G2 1 "Phase1.Save.Tests exited non-zero (see g2-save-tests.log)"
  fi
  if ! check_out="$(python3 - "$save_log" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["save"]
matches = re.findall(cfg["summaryRegex"], text, re.MULTILINE)
if not matches:
    sys.exit("Phase1.Save.Tests summary line missing")
passed, total, failed = map(int, matches[-1])
if failed > cfg["maxFailed"] or passed != total or passed < cfg["minPassed"]:
    sys.exit(f"Phase1.Save.Tests count check failed: {passed}/{total} passed, {failed} failed (minPassed={cfg['minPassed']})")
print(f"Phase1.Save.Tests PASS ({passed}/{total} passed, {failed} failed)")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # 3. tests/ui-smoke-multiarea.sh
  log_line "G2: running tests/ui-smoke-multiarea.sh..."
  local ma_ev="$EVIDENCE_DIR/g2-ui-smoke-multiarea"
  local ma_log="$EVIDENCE_DIR/g2-ui-smoke-multiarea.log"
  if ! bash "$P1_ROOT/tests/ui-smoke-multiarea.sh" "$ma_ev" > "$ma_log" 2>&1; then
    tail -n 40 "$ma_log" >&2 || true
    fail_stage G2 1 "tests/ui-smoke-multiarea.sh exited non-zero (see g2-ui-smoke-multiarea.log)"
  fi
  if ! check_out="$(python3 - "$ma_log" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["uiSmokeMultiarea"]
matches = re.findall(cfg["summaryRegex"], text, re.MULTILINE)
if not matches:
    sys.exit("ui-smoke-multiarea.sh summary line missing")
verdict, passed_s, failed_s = matches[-1]
passed, failed = int(passed_s), int(failed_s)
if verdict != "PASS" or failed > cfg["maxFailed"] or passed < cfg["minPassed"]:
    sys.exit(f"ui-smoke-multiarea.sh failed: {verdict} ({passed} passed, {failed} failed, minPassed={cfg['minPassed']})")
print(f"ui-smoke-multiarea.sh PASS ({passed} passed, {failed} failed)")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # 4. tests/ui-smoke.sh (runs in full; exact 4 T0 #885 drift IDs tolerated per spec §4.6)
  log_line "G2: running tests/ui-smoke.sh..."
  local sa_ev="$EVIDENCE_DIR/g2-ui-smoke-single"
  local sa_log="$EVIDENCE_DIR/g2-ui-smoke-single.log"
  local sa_rc=0
  bash "$P1_ROOT/tests/ui-smoke.sh" "$sa_ev" > "$sa_log" 2>&1 || sa_rc=$?
  if ! check_out="$(python3 - "$sa_log" "$baseline_json" "$sa_rc" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["uiSmokeSingleArea"]
rc = int(sys.argv[3])
matches = re.findall(cfg["summaryRegex"], text, re.MULTILINE)
if not matches:
    sys.exit(f"ui-smoke.sh summary line missing (rc={rc})")
verdict, passed_s, failed_s = matches[-1]
passed, failed = int(passed_s), int(failed_s)
if passed < cfg["minPassed"]:
    sys.exit(f"ui-smoke.sh passed={passed} < minPassed={cfg['minPassed']}")
for req_id in cfg["requiredPassIds"]:
    if not re.search(rf"^PASS\s+{re.escape(req_id)}\b", text, re.MULTILINE):
        sys.exit(f"ui-smoke.sh missing required PASS {req_id}")
fail_ids = re.findall(r"^FAIL\s+(\S+)", text, re.MULTILINE)
if len(fail_ids) != failed:
    sys.exit(f"ui-smoke.sh FAIL line count ({len(fail_ids)}) != summary failed ({failed})")
allowed = set(cfg["knownBaselineFailures"])
unexpected = [fid for fid in fail_ids if fid not in allowed]
if unexpected:
    sys.exit(f"ui-smoke.sh unexpected failure IDs: {unexpected}")
if failed == 0:
    print(f"ui-smoke.sh PASS ({passed} passed, 0 failed)")
else:
    print(f"ui-smoke.sh PASS-BASELINE-DRIFT ({passed} passed, {failed} known T0 #885 failures: {','.join(fail_ids)}; G11b PASS)")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # 5. tests/cli-safety.sh
  log_line "G2: running tests/cli-safety.sh..."
  local cli_ev="$EVIDENCE_DIR/g2-cli-safety"
  local cli_log="$EVIDENCE_DIR/g2-cli-safety.log"
  if ! bash "$P1_ROOT/tests/cli-safety.sh" "$cli_ev" > "$cli_log" 2>&1; then
    tail -n 40 "$cli_log" >&2 || true
    fail_stage G2 1 "tests/cli-safety.sh exited non-zero (see g2-cli-safety.log)"
  fi
  if ! check_out="$(python3 - "$cli_log" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["cliSafety"]
if not re.search(r"^OVERALL:\s+PASS$", text, re.MULTILINE):
    sys.exit("cli-safety.sh missing OVERALL: PASS")
passed = len(re.findall(r"^PASS\s+", text, re.MULTILINE))
failed = len(re.findall(r"^FAIL\s+", text, re.MULTILINE))
if failed > cfg["maxFailed"] or passed < cfg["minPassed"]:
    sys.exit(f"cli-safety.sh count check failed: {passed} passed, {failed} failed (minPassed={cfg['minPassed']})")
print(f"cli-safety.sh PASS ({passed} passed, {failed} failed)")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # 6. tests/build-output-safety.sh (spec §4.6 Option A)
  log_line "G2: running tests/build-output-safety.sh..."
  local bos_ev="$EVIDENCE_DIR/g2-build-output-safety"
  local bos_log="$EVIDENCE_DIR/g2-build-output-safety.log"
  if [[ "$P0_MODE" == PRE-SEEDED* ]]; then
    P0REF_BACKUP_DIR="$SCRATCH_DIR/p0ref-backup"
    rm -rf -- "$P0REF_BACKUP_DIR"
    mkdir -p -- "$P0REF_BACKUP_DIR"
    cp --no-dereference "$P1_ROOT/.p0ref/SitkaLab.Core.dll" "$P0REF_BACKUP_DIR/SitkaLab.Core.dll"
    cp --no-dereference "$P1_ROOT/.p0ref/SitkaLab.Core.dll.sha256" "$P0REF_BACKUP_DIR/SitkaLab.Core.dll.sha256"
    cp --no-dereference "$P1_ROOT/.p0ref/provenance.txt" "$P0REF_BACKUP_DIR/provenance.txt"

    local bos_rc=0
    bash "$P1_ROOT/tests/build-output-safety.sh" "$bos_ev" > "$bos_log" 2>&1 || bos_rc=$?

    # Immediately restore verified .p0ref seed and re-verify SHA-256.
    restore_p0ref_backup_if_needed
    P0REF_BACKUP_DIR=""
    local restored_sha
    restored_sha="$(sha256sum "$P1_ROOT/.p0ref/SitkaLab.Core.dll" | cut -c1-64)"
    if [[ "$restored_sha" != "$expected_p0_sha" ]]; then
      fail_stage G2 1 "restored .p0ref/SitkaLab.Core.dll sha256=${restored_sha} != expected ${expected_p0_sha}"
    fi

    if ! check_out="$(python3 - "$bos_ev/build-output-safety.txt" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["buildOutputSafety"]
for lbl in cfg["requiredPassLabels"]:
    if not re.search(rf"^PASS\s+{re.escape(lbl)}", text, re.MULTILINE):
        sys.exit(f"build-output-safety.sh missing required PASS {lbl}")
host_na = set(cfg["hostNaLabels"])
fail_labels = re.findall(r"^FAIL\s+([^:\s]+)", text, re.MULTILINE)
unexpected = [f for f in fail_labels if f not in host_na]
if unexpected:
    sys.exit(f"build-output-safety.sh unexpected FAIL labels: {unexpected}")
print(f"build-output-safety.sh PASS (C1-C6+P0 PASS; HOST-NA: {','.join(cfg['hostNaLabels'])}; .p0ref restored & re-verified)")
PY
    )"; then
      fail_stage G2 1 "$check_out"
    fi
    HOST_NA_LIST+=("build-output-safety.sh C7-cold-build & C8-rebuild (frozen 8.0.133 pin not cold-reproducible on this host)")
    log_line "G2: ${check_out}"
  else
    if ! bash "$P1_ROOT/tests/build-output-safety.sh" "$bos_ev" > "$bos_log" 2>&1; then
      tail -n 40 "$bos_log" >&2 || true
      fail_stage G2 1 "tests/build-output-safety.sh failed on cold-build host"
    fi
    if ! grep -q '^OVERALL: PASS$' "$bos_ev/build-output-safety.txt"; then
      fail_stage G2 1 "tests/build-output-safety.sh missing OVERALL: PASS"
    fi
    log_line "G2: build-output-safety.sh PASS (all C1-C8 + P0)"
  fi

  # 7. tests/p0-isolation.sh (spec §4.6 Option A)
  log_line "G2: running tests/p0-isolation.sh..."
  local p0i_ev="$EVIDENCE_DIR/g2-p0-isolation"
  local p0i_log="$EVIDENCE_DIR/g2-p0-isolation.log"
  if [[ "$P0_MODE" == PRE-SEEDED* ]]; then
    # Verify GNU coreutils 9.7 shim binaries exist (exit 3 if any absent).
    local gnu_bin="$SCRATCH_DIR/gnu-bin"
    mkdir -p -- "$gnu_bin"
    local gtool
    for gtool in ln chmod touch mv cp rm mkdir truncate stat cat ls head; do
      if [[ ! -x "/usr/bin/gnu${gtool}" ]]; then
        fail_stage G2 3 "host-compat prerequisite missing: /usr/bin/gnu${gtool} (GNU coreutils required for p0-isolation.sh L4 decoy)"
      fi
      ln -sf "/usr/bin/gnu${gtool}" "$gnu_bin/${gtool}"
    done

    # Create disposable --shared sparse clone of exact HEAD (isolated from real worktree; <20 MB).
    local iso_clone="$SCRATCH_DIR/p0iso-clone"
    git clone --quiet --shared --no-checkout "$REPO_ROOT" "$iso_clone"
    git -C "$iso_clone" sparse-checkout init --no-cone
    git -C "$iso_clone" sparse-checkout set \
      /Tools/MultiRackSitkaP1Lab/ \
      /Tools/MultiRackSitkaLab/ \
      /Assets/ForestPrototype/SimulationRandom.cs \
      /Assets/ForestPrototype/SitkaGrowthModel.cs \
      /Assets/ForestPrototype/Species/SitkaSpruce.asset \
      /Assets/ForestPrototype/TreeSpeciesDefinition.cs
    git -C "$iso_clone" checkout --quiet --detach "$HEAD_SHA"

    # Replace ONLY the clone's build-p0-core-ref.sh with the verified-seed host-compat shim.
    cat > "$iso_clone/Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh" <<SHIM
#!/usr/bin/env bash
set -euo pipefail
p1_root="\$(cd "\$(dirname "\${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="\$(git -C "\$p1_root" rev-parse --show-toplevel)"
actual_tree="\$(git -C "\$repo" rev-parse HEAD:Tools/MultiRackSitkaLab)"
[[ "\$actual_tree" == "${expected_p0_tree}" ]] || { echo "P0 tree mismatch" >&2; exit 2; }
out="\$p1_root/.p0ref"
rm -rf -- "\$out"
mkdir -p -- "\$out"
cp --no-dereference "${P0REF_DIR}/SitkaLab.Core.dll" "\$out/SitkaLab.Core.dll"
cp --no-dereference "${P0REF_DIR}/SitkaLab.Core.dll.sha256" "\$out/SitkaLab.Core.dll.sha256"
cp --no-dereference "${P0REF_DIR}/provenance.txt" "\$out/provenance.txt"
sha="\$(sha256sum "\$out/SitkaLab.Core.dll" | cut -c1-64)"
[[ "\$sha" == "${expected_p0_sha}" ]] || { echo "P0 seed sha mismatch: \$sha" >&2; exit 1; }
echo "P0 reference installed from verified seed: \$sha"
SHIM
    chmod +x "$iso_clone/Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh"

    if ! PATH="$gnu_bin:$PATH" \
         MSBUILDDISABLENODEREUSE=1 \
         UseSharedCompilation=false \
         DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 \
         DOTNET_EnableWriteXorExecute=0 \
         DOTNET_EnableDiagnostics=0 \
         bash "$iso_clone/Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh" "$p0i_ev" > "$p0i_log" 2>&1; then
      tail -n 40 "$p0i_log" >&2 || true
      fail_stage G2 1 "tests/p0-isolation.sh failed in disposable clone (see g2-p0-isolation.log)"
    fi
    rm -rf -- "$iso_clone"
    HOST_NA_LIST+=("p0-isolation.sh cold-rebuild step (verified seed shim in disposable clone; L0/L1/L2/L3/L3+/L4/decoy/trace PASS)")
  else
    if ! MSBUILDDISABLENODEREUSE=1 \
         UseSharedCompilation=false \
         DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1 \
         DOTNET_EnableWriteXorExecute=0 \
         DOTNET_EnableDiagnostics=0 \
         bash "$P1_ROOT/tests/p0-isolation.sh" "$p0i_ev" > "$p0i_log" 2>&1; then
      tail -n 40 "$p0i_log" >&2 || true
      fail_stage G2 1 "tests/p0-isolation.sh failed (see g2-p0-isolation.log)"
    fi
  fi

  if ! check_out="$(python3 - "$p0i_ev/summary.txt" "$baseline_json" <<'PY'
import json, re, sys
text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["suites"]["p0Isolation"]
for m in cfg["requiredMarkers"]:
    if m not in text:
        sys.exit(f"p0-isolation.sh missing required marker: {m!r}")
m_dec = re.search(r"decoy\s+(\d+)/(\d+)\s+distinct expected operation identities matched,\s+(\d+)\s+unresolved", text)
if not m_dec:
    sys.exit("p0-isolation.sh missing decoy count line")
dec_got, dec_tot, dec_unres = map(int, m_dec.groups())
if dec_got < cfg["minDecoyMatched"] or dec_got != dec_tot or dec_unres > cfg["maxUnresolved"]:
    sys.exit(f"p0-isolation.sh decoy check failed: {dec_got}/{dec_tot} unresolved={dec_unres}")
m_l4 = re.search(r"protected hits=(\d+)\s+unresolved=(\d+)\s+failed protected attempts=(\d+)", text)
if not m_l4:
    sys.exit("p0-isolation.sh missing L4 trace counts")
hits, unres, fpa = map(int, m_l4.groups())
if hits != 0 or unres > cfg["maxUnresolved"] or fpa != 0:
    sys.exit(f"p0-isolation.sh L4 trace violation: hits={hits} unresolved={unres} failed_protected={fpa}")
print(f"p0-isolation.sh PASS (decoy={dec_got}/{dec_tot}, protected hits={hits}, unresolved={unres})")
PY
  )"; then
    fail_stage G2 1 "$check_out"
  fi
  log_line "G2: ${check_out}"

  # Mandatory P0-untouched AFTER check (spec §4.6 item 4).
  local p0_tree_after p0_digest_after p0_dll_after
  p0_tree_after="$(git -C "$REPO_ROOT" rev-parse HEAD:Tools/MultiRackSitkaLab)"
  p0_digest_after="$(p0_tracked_digest)"
  p0_dll_after="$(sha256sum "$P1_ROOT/.p0ref/SitkaLab.Core.dll" | cut -c1-64)"
  git -C "$REPO_ROOT" status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab \
    > "$EVIDENCE_DIR/g2-p0-status-after.txt"
  if [[ "$p0_tree_after" != "$expected_p0_tree" ]]; then
    fail_stage G2 1 "P0 tree changed after G2: $p0_tree_after"
  fi
  if [[ "$p0_digest_after" != "$expected_p0_digest" ]]; then
    fail_stage G2 1 "P0 tracked digest changed after G2: $p0_digest_after"
  fi
  if ! cmp -s "$EVIDENCE_DIR/g2-p0-status-before.txt" "$EVIDENCE_DIR/g2-p0-status-after.txt"; then
    fail_stage G2 1 "P0 git status/ignored snapshot changed during G2"
  fi
  if [[ "$p0_dll_after" != "$expected_p0_sha" ]]; then
    fail_stage G2 1 ".p0ref/SitkaLab.Core.dll sha changed after G2: $p0_dll_after"
  fi

  # 8. Packet sub-check: S0a /multi asset byte-identity (from A-C2 onward, whenever identity JSON exists).
  local id_json="$GATE_DATA_DIR/s0a-multi-asset-identity.json"
  if [[ -f "$id_json" ]]; then
    log_line "G2: running S0a /multi asset identity sub-check..."
    if ! check_out="$(python3 - "$REPO_ROOT" "$P1_ROOT" "$id_json" "$EVIDENCE_DIR/g2-s0a-asset-identity.json" "$SCRATCH_DIR/xdg-data" <<'PY'
import hashlib, http.client, json, os, re, subprocess, sys, time

repo, p1_root, id_path, out_path, xdg_dir = sys.argv[1:]
id_doc = json.load(open(id_path, "r", encoding="utf-8"))
mode = id_doc.get("mode")
t0 = id_doc["t0Commit"]
raw_py = f"{p1_root}/build/2d-gate/raw-literal.py"

if mode == "s0a-full":
    # 1. Recompute R_x from T0 blob via raw-literal.py and compare to committed blobs at HEAD.
    specs = [
        ("html", "Html", "multi", "Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.html"),
        ("css", "Css", "multi.css", "Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css"),
        ("js", "Js", "multi.js", "Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js"),
    ]
    recomputed = {}
    for key, const_name, route, git_path in specs:
        entry = id_doc["assets"][key]
        r_sha = subprocess.check_output(
            [sys.executable, raw_py, "--repo", repo, "--rev", t0, "--name", const_name, "--out-sha256"],
            text=True,
        ).strip()
        if r_sha != entry["rSha256"]:
            sys.exit(f"asset {key}: recomputed R={r_sha} != manifest rSha256={entry['rSha256']}")
        if entry["pSha256"] != r_sha or entry["sSha256"] != r_sha or entry["committedSha256"] != r_sha:
            sys.exit(f"asset {key}: manifest R/P/S/committed hashes are not all equal")

        blob = subprocess.check_output(["git", "-C", repo, "show", f"HEAD:{git_path}"])
        if blob.startswith(b"\xef\xbb\xbf"):
            sys.exit(f"asset {key}: committed file has UTF-8 BOM")
        if b"\r" in blob:
            sys.exit(f"asset {key}: committed file contains CR byte")
        if blob.endswith(b"\n"):
            sys.exit(f"asset {key}: committed file has trailing LF")
        blob_sha = hashlib.sha256(blob).hexdigest()
        if blob_sha != r_sha:
            sys.exit(f"asset {key}: HEAD blob sha={blob_sha} != R={r_sha}")
        if len(blob) != entry["bytes"]:
            sys.exit(f"asset {key}: HEAD blob bytes={len(blob)} != manifest {entry['bytes']}")
        recomputed[key] = {"route": route, "rSha256": r_sha, "headBlobSha256": blob_sha, "bytes": len(blob)}

    # 2. Boot built sitkap1-ui on loopback and live-GET multi, multi.css, multi.js.
    ui_dll = f"{p1_root}/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
    fixture_dir = f"{p1_root}/fixtures/derived"
    os.makedirs(xdg_dir, mode=0o700, exist_ok=True)
    env = dict(os.environ)
    env["XDG_DATA_HOME"] = xdg_dir
    proc = subprocess.Popen(
        ["dotnet", ui_dll, fixture_dir, "--multi-experiment=ma-1"],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True,
        env=env,
    )
    cap_url = None
    deadline = time.monotonic() + 20.0
    try:
        while time.monotonic() < deadline:
            line = proc.stdout.readline()
            if not line:
                if proc.poll() is not None:
                    break
                continue
            m = re.search(r"http://(127\.0\.0\.1:\d+)(/s/[0-9a-f]+/)", line)
            if m:
                host_port, prefix = m.group(1), m.group(2)
                cap_url = (host_port, prefix)
                break
        if cap_url is None:
            sys.exit("failed to obtain capability URL from SitkaP1.Ui")
        host, port_s = cap_url[0].split(":")
        port = int(port_s)
        prefix = cap_url[1]

        live_out = {}
        for key, _, route, _ in specs:
            entry = id_doc["assets"][key]
            conn = http.client.HTTPConnection(host, port, timeout=10)
            try:
                conn.request("GET", prefix + route, headers={"Host": f"{host}:{port}", "Connection": "close"})
                resp = conn.getresponse()
                body = resp.read()
                status = resp.status
                hdrs = sorted((k.lower(), v) for k, v in resp.getheaders() if k.lower() != "date")
            finally:
                conn.close()
            if status != 200:
                sys.exit(f"live GET {route}: status={status} != 200")
            s_sha = hashlib.sha256(body).hexdigest()
            if s_sha != recomputed[key]["rSha256"]:
                sys.exit(f"live GET {route}: sha256={s_sha} != R={recomputed[key]['rSha256']}")
            expected_hdrs = [tuple(x) for x in entry["headersNoDate"]]
            if hdrs != expected_hdrs:
                sys.exit(f"live GET {route}: headers mismatch:\n  got={hdrs}\n  want={expected_hdrs}")
            live_out[key] = {
                "route": route,
                "status": status,
                "bytes": len(body),
                "liveSha256": s_sha,
                "rSha256": recomputed[key]["rSha256"],
                "headersMatch": True,
            }
    finally:
        proc.terminate()
        try:
            proc.wait(timeout=5)
        except subprocess.TimeoutExpired:
            proc.kill()
            proc.wait(timeout=5)

    with open(out_path, "w", encoding="utf-8") as f:
        json.dump({"mode": mode, "t0Commit": t0, "results": live_out}, f, indent=2)
        f.write("\n")
    print("S0a /multi asset identity PASS (html/css/js: R == P == S_live == committed; headers match)")
elif mode == "s0c-css-plus-html-delta":
    # S0c mode: multi.css stays byte-identical to T0.
    css_entry = id_doc["assets"]["css"]
    r_css = subprocess.check_output(
        [sys.executable, raw_py, "--repo", repo, "--rev", t0, "--name", "Css", "--out-sha256"],
        text=True,
    ).strip()
    blob = subprocess.check_output(["git", "-C", repo, "show", "HEAD:Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.css"])
    if hashlib.sha256(blob).hexdigest() != r_css or r_css != css_entry["rSha256"]:
        sys.exit("S0c mode: multi.css no longer matches T0 R_css")
    print("S0c multi.css identity PASS")
else:
    sys.exit(f"unknown s0a-multi-asset-identity.json mode: {mode!r}")
PY
    )"; then
      fail_stage G2 1 "S0a /multi asset identity failed: ${check_out}"
    fi
    log_line "G2: ${check_out}"
  fi

  # 9. Packet sub-check: S0a move-proof (when s0a-move-table.json exists with gateEnforce=true).
  local move_table="$GATE_DATA_DIR/s0a-move-table.json"
  local move_py="$GATE_DATA_DIR/move-proof.py"
  if [[ -f "$move_table" && -f "$move_py" ]]; then
    local enforce
    enforce="$(python3 -c 'import json,sys; print("1" if json.load(open(sys.argv[1])).get("gateEnforce", False) else "0")' "$move_table")"
    if [[ "$enforce" == "1" ]]; then
      log_line "G2: running S0a move-proof sub-check..."
      local t0_mp
      t0_mp="$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["t0Commit"])' "$move_table")"
      if ! python3 "$move_py" --repo "$REPO_ROOT" --base "$t0_mp" --head HEAD --table "$move_table" --report "$EVIDENCE_DIR/g2-move-proof.json" > "$EVIDENCE_DIR/g2-move-proof.log" 2>&1; then
        cat "$EVIDENCE_DIR/g2-move-proof.log" >&2 || true
        fail_stage G2 1 "S0a move-proof failed (see g2-move-proof.log)"
      fi
      log_line "G2: $(tail -n1 "$EVIDENCE_DIR/g2-move-proof.log")"
    fi
  fi

  # 10. Packet sub-check: S0c module graph & reconstruction (when present in S0c).
  if [[ -f "$GATE_DATA_DIR/module-graph.mjs" ]]; then
    log_line "G2: running S0c module-graph.mjs..."
    if ! node "$GATE_DATA_DIR/module-graph.mjs" "$EVIDENCE_DIR/g2-s0c-module-graph.json" > "$EVIDENCE_DIR/g2-s0c-module-graph.log" 2>&1; then
      fail_stage G2 1 "S0c module-graph.mjs failed"
    fi
  fi
  if [[ -f "$GATE_DATA_DIR/js-reconstruct.mjs" ]]; then
    log_line "G2: running S0c js-reconstruct.mjs..."
    if ! node "$GATE_DATA_DIR/js-reconstruct.mjs" "$EVIDENCE_DIR/g2-s0c-reconstruct.json" > "$EVIDENCE_DIR/g2-s0c-reconstruct.log" 2>&1; then
      fail_stage G2 1 "S0c js-reconstruct.mjs failed"
    fi
  fi

  emit_marker G2 PASS
}

# ==============================================================================
# Internal worker entry (executes G1 + G2 inside unshare -rn)
# ==============================================================================
if [[ "$INTERNAL_MODE" -eq 1 ]]; then
  REPO_ROOT="$(git -C "$P1_ROOT" rev-parse --show-toplevel)"
  REPO_ROOT="$(cd "$REPO_ROOT" && pwd -P)"
  HEAD_SHA="$(git -C "$REPO_ROOT" rev-parse HEAD)"
  EVIDENCE_DIR="$(cd "$EVIDENCE_DIR" && pwd -P)"
  GATE_LOG="$EVIDENCE_DIR/gate.log"
  SCRATCH_DIR="$(mktemp -d "${TMPDIR:-/tmp}/ccf-2d-gate-netns.XXXXXXXX")"
  ip link set lo up
  run_stage_G1
  run_stage_G2
  {
    printf 'P0_MODE=%q\n' "$P0_MODE"
    printf 'HOST_NA_JOINED=%q\n' "$(IFS='|'; echo "${HOST_NA_LIST[*]:-}")"
  } > "$EVIDENCE_DIR/g1-g2.env"
  exit 0
fi

# ==============================================================================
# Stage G3 — 9/9 frozen SHA-256 anchors
# ==============================================================================
run_stage_G3() {
  local out
  if ! out="$(python3 - "$P1_ROOT" "$GATE_DATA_DIR/frozen-anchors.json" "$EVIDENCE_DIR/g2-core-tests.log" "$EVIDENCE_DIR/g3-anchors.json" <<'PY'
import hashlib, json, os, re, sys

p1_root, manifest_path, core_log_path, report_path = sys.argv[1:]
manifest = json.load(open(manifest_path, "r", encoding="utf-8"))
core_log = open(core_log_path, "r", encoding="utf-8", errors="replace").read()

anchors = manifest["anchors"]
ids = sorted(a["id"] for a in anchors)
if ids != [1, 2, 3, 4, 5, 6, 7, 8, 9]:
    sys.exit(f"expected anchor IDs 1..9, got {ids}")

results = []
for a in anchors:
    aid = a["id"]
    expected = a["sha256"]
    kind = a["kind"]
    entry = {"id": aid, "name": a["name"], "expected": expected, "checks": []}

    for sc in a.get("sourceChecks", []):
        src_path = os.path.join(p1_root, sc["file"])
        src_text = open(src_path, "r", encoding="utf-8").read()
        matches = re.findall(sc["regex"], src_text)
        if len(matches) != 1:
            sys.exit(f"Anchor #{aid} ({a['name']}): expected 1 regex match in {sc['file']}, found {len(matches)}")
        val = matches[0] if isinstance(matches[0], str) else next(x for x in matches[0] if x)
        if val != expected:
            sys.exit(f"Anchor #{aid} ({a['name']}): source pin in {sc['file']}={val} != manifest {expected}")
        entry["checks"].append({"type": "source-pin", "file": sc["file"], "actual": val, "ok": True})

    if kind == "core-stdout-pair":
        matches = re.findall(a["stdoutRegex"], core_log)
        if len(matches) != 1:
            sys.exit(f"Anchor #{aid} ({a['name']}): expected 1 stdout match, found {len(matches)}")
        h0, h1 = matches[0]
        if h0 != expected or h1 != expected:
            sys.exit(f"Anchor #{aid} ({a['name']}): stdout h0={h0}, h1={h1} != manifest {expected}")
        entry["checks"].append({"type": "core-stdout-pair", "h0": h0, "h1": h1, "ok": True})
    elif kind == "core-stdout-single":
        matches = re.findall(a["stdoutRegex"], core_log)
        if len(matches) != 1:
            sys.exit(f"Anchor #{aid} ({a['name']}): expected 1 stdout match, found {len(matches)}")
        h = matches[0]
        if h != expected:
            sys.exit(f"Anchor #{aid} ({a['name']}): stdout h={h} != manifest {expected}")
        entry["checks"].append({"type": "core-stdout-single", "actual": h, "ok": True})
    elif kind == "dll-and-source-pin":
        dll_path = os.path.join(p1_root, a["dllPath"])
        if not os.path.isfile(dll_path):
            sys.exit(f"Anchor #{aid}: missing {dll_path}")
        actual = hashlib.sha256(open(dll_path, "rb").read()).hexdigest()
        if actual != expected:
            sys.exit(f"Anchor #{aid}: {a['dllPath']} sha256={actual} != manifest {expected}")
        entry["checks"].append({"type": "dll-sha256", "path": a["dllPath"], "actual": actual, "ok": True})
    elif kind != "source-pin":
        sys.exit(f"Anchor #{aid}: unknown kind {kind!r}")

    results.append(entry)

with open(report_path, "w", encoding="utf-8") as f:
    json.dump({"verifiedCount": len(results), "anchors": results}, f, indent=2)
    f.write("\n")
print("9/9 frozen SHA-256 anchors verified")
PY
  )"; then
    fail_stage G3 1 "$out"
  fi
  log_line "G3: ${out}"
  emit_marker G3 PASS
}

# ==============================================================================
# Stage G4 — Committed v2 golden saves
# ==============================================================================
run_stage_G4() {
  local out
  if ! out="$(python3 - "$REPO_ROOT" "$P1_ROOT" "$GATE_DATA_DIR/frozen-anchors.json" "$EVIDENCE_DIR/g2-save-tests.log" "$BASE_COMMIT" <<'PY'
import json, os, re, subprocess, sys

repo, p1_root, manifest_path, save_log_path, base_commit = sys.argv[1:]
manifest = json.load(open(manifest_path, "r", encoding="utf-8"))
g = manifest["v2Golden"]
save_log = open(save_log_path, "r", encoding="utf-8", errors="replace").read()

expected_pass_line = f"PASS {g['caseName']}"
if not re.search(rf"^{re.escape(expected_pass_line)}$", save_log, re.MULTILINE):
    sys.exit(f"Save tests log missing exact line: {expected_pass_line!r}")

src_text = open(os.path.join(p1_root, g["sourceFile"]), "r", encoding="utf-8").read()
m_sha = re.findall(r'const string PinnedFileSha256\s*=\s*"([0-9a-f]{64})";', src_text)
m_chk = re.findall(r'const string PinnedEmbeddedChecksum\s*=\s*"([0-9a-f]{64})";', src_text)
m_len = re.findall(r'T\.Eq\((\d+),\s*approvedBytes\.Length,\s*"byte length"\);', src_text)
if m_sha != [g["fileSha256"]]:
    sys.exit(f"v2 golden PinnedFileSha256 mismatch: {m_sha} vs {g['fileSha256']}")
if m_chk != [g["embeddedChecksum"]]:
    sys.exit(f"v2 golden PinnedEmbeddedChecksum mismatch: {m_chk} vs {g['embeddedChecksum']}")
if m_len != [str(g["byteLength"])]:
    sys.exit(f"v2 golden byteLength mismatch: {m_len} vs {g['byteLength']}")

fmt_text = open(os.path.join(p1_root, g["schemaVersionFile"]), "r", encoding="utf-8").read()
if not re.search(rf"public const int SchemaVersion\s*=\s*{g['schemaVersion']};", fmt_text):
    sys.exit(f"{g['schemaVersionFile']} missing SchemaVersion = {g['schemaVersion']}")

for ref_label, ref in [("base", base_commit), ("t0", manifest["t0Commit"])]:
    # Check if ref is an ancestor or valid commit
    chk = subprocess.run(["git", "-C", repo, "cat-file", "-e", f"{ref}^{{commit}}"], capture_output=True)
    if chk.returncode == 0:
        diff = subprocess.check_output(
            ["git", "-C", repo, "diff", "--name-only", f"{ref}..HEAD", "--", "Tools/MultiRackSitkaP1Lab/src/Core/Phase1/"],
            text=True,
        ).strip()
        if diff:
            sys.exit(f"src/Core/Phase1/ modified vs {ref_label} ({ref}): {diff}")

print(f"v2 golden verified (bytes={g['byteLength']}, fileSha={g['fileSha256'][:12]}..., checksum={g['embeddedChecksum'][:12]}..., SchemaVersion={g['schemaVersion']}, src/Core/Phase1 unchanged)")
PY
  )"; then
    fail_stage G4 1 "$out"
  fi
  log_line "G4: ${out}"
  emit_marker G4 PASS
}

# ==============================================================================
# Stage G5 — Payload budget checks
# ==============================================================================
run_stage_G5() {
  local out
  if ! out="$(python3 - "$EVIDENCE_DIR/g2-ui-smoke-multiarea/ui-smoke-multiarea.txt" "$GATE_DATA_DIR/baseline-counts.json" <<'PY'
import json, re, sys
log_text = open(sys.argv[1], "r", encoding="utf-8", errors="replace").read()
cfg = json.load(open(sys.argv[2]))["payloadBudgetChecks"]
missing = []
failed = []
for cid in cfg["requiredPassIds"]:
    if re.search(rf"^FAIL\s+{re.escape(cid)}\b", log_text, re.MULTILINE):
        failed.append(cid)
    if not re.search(rf"^PASS\s+{re.escape(cid)}\b", log_text, re.MULTILINE):
        missing.append(cid)
if failed or missing:
    sys.exit(f"payload budget check failed: failed={failed}, missing={missing}")
print(f"payload budget checks verified ({len(cfg['requiredPassIds'])}/{len(cfg['requiredPassIds'])} PASS: {', '.join(cfg['requiredPassIds'])})")
PY
  )"; then
    fail_stage G5 1 "$out"
  fi
  log_line "G5: ${out}"
  emit_marker G5 PASS
}

# ==============================================================================
# Stage G6 — Core determinism grep gate
# ==============================================================================
run_stage_G6() {
  local out
  if ! out="$(python3 - "$P1_ROOT" "$GATE_DATA_DIR/determinism-allowlist.txt" "$EVIDENCE_DIR/g6-determinism.json" <<'PY'
import json, os, re, sys

p1_root, allowlist_path, report_path = sys.argv[1:]
core_dir = os.path.join(p1_root, "src", "Core")

allowlist = set()
for raw in open(allowlist_path, "r", encoding="utf-8"):
    line = raw.strip()
    if not line or line.startswith("#"):
        continue
    parts = [p.strip() for p in line.split("|")]
    if len(parts) < 3:
        sys.exit(f"malformed determinism-allowlist.txt line: {raw!r}")
    allowlist.add((parts[0], parts[1], parts[2]))

f1_tokens = [
    ("new Random(", re.compile(r"\bnew\s+Random\s*\(")),
    ("new System.Random(", re.compile(r"\bnew\s+System\.Random\s*\(")),
    ("DateTime.Now", re.compile(r"\bDateTime\.Now\b")),
    ("DateTimeOffset.Now", re.compile(r"\bDateTimeOffset\.Now\b")),
    ("Guid.NewGuid", re.compile(r"\bGuid\.NewGuid\b")),
    ("Parallel.", re.compile(r"\bParallel\.")),
    (".AsParallel(", re.compile(r"\.AsParallel\s*\(")),
]

f2_tokens = [
    ("RandomNumberGenerator", re.compile(r"\bRandomNumberGenerator\b")),
    ("DateTime.UtcNow", re.compile(r"\bDateTime\.UtcNow\b")),
    ("DateTimeOffset.UtcNow", re.compile(r"\bDateTimeOffset\.UtcNow\b")),
    ("Environment.TickCount", re.compile(r"\bEnvironment\.TickCount\b")),
    ("Stopwatch", re.compile(r"\bStopwatch\b")),
]

member_decl_re = re.compile(
    r"^\s{1,8}(?:public|internal|private|protected)\s+"
    r"(?:(?:static|readonly|sealed|override|virtual|async|partial|extern|unsafe|new)\s+)*"
    r"(?:[\w<>,\[\]\?\.\s]+?)\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:<[^>]+>)?\s*(?:\(|=>|=|\{)"
)

def strip_comments_keep_lines(src):
    out = []
    i = 0
    n = len(src)
    in_str = False
    in_verbatim = False
    in_char = False
    while i < n:
        c = src[i]
        nxt = src[i + 1] if i + 1 < n else ""
        if in_verbatim:
            if c == '"' and nxt == '"':
                out.append('""')
                i += 2
            elif c == '"':
                in_verbatim = False
                out.append(c)
                i += 1
            else:
                out.append(c)
                i += 1
        elif in_str:
            if c == '\\' and i + 1 < n:
                out.append(src[i:i+2])
                i += 2
            elif c == '"':
                in_str = False
                out.append(c)
                i += 1
            else:
                out.append(c)
                i += 1
        elif in_char:
            if c == '\\' and i + 1 < n:
                out.append(src[i:i+2])
                i += 2
            elif c == "'":
                in_char = False
                out.append(c)
                i += 1
            else:
                out.append(c)
                i += 1
        else:
            if c == '/' and nxt == '/':
                while i < n and src[i] != '\n':
                    i += 1
            elif c == '/' and nxt == '*':
                i += 2
                while i < n - 1 and not (src[i] == '*' and src[i + 1] == '/'):
                    if src[i] == '\n':
                        out.append('\n')
                    i += 1
                i += 2
            elif c == '@' and nxt == '"':
                in_verbatim = True
                out.append('@"')
                i += 2
            elif c == '"':
                in_str = True
                out.append(c)
                i += 1
            elif c == "'":
                in_char = True
                out.append(c)
                i += 1
            else:
                out.append(c)
                i += 1
    return "".join(out)

def find_enclosing_member(lines, line_idx):
    for j in range(line_idx, -1, -1):
        m = member_decl_re.match(lines[j])
        if m:
            return m.group(1)
    return "<unknown>"

f1_violations = []
f2_allowed_hits = []
f2_violations = []
rng_violations = []

rng_salts_file = os.path.join(core_dir, "RngSalts.cs")
rng_literals = []
if os.path.isfile(rng_salts_file):
    rs_text = open(rng_salts_file, "r", encoding="utf-8").read()
    rng_literals = sorted(set(re.findall(r'internal const string \w+\s*=\s*"([^"]+)";', rs_text)))

files_scanned = 0
for root, dirs, files in os.walk(core_dir):
    dirs[:] = sorted(d for d in dirs if d not in ("bin", "obj"))
    for fn in sorted(files):
        if not fn.endswith(".cs"):
            continue
        files_scanned += 1
        full = os.path.join(root, fn)
        rel = os.path.relpath(full, p1_root).replace(os.sep, "/")
        raw_text = open(full, "r", encoding="utf-8").read()
        code_text = strip_comments_keep_lines(raw_text)
        code_lines = code_text.splitlines()
        raw_lines = raw_text.splitlines()

        for idx, line in enumerate(code_lines):
            for tok, rx in f1_tokens:
                if rx.search(line):
                    f1_violations.append({"file": rel, "line": idx + 1, "token": tok, "text": raw_lines[idx].strip()})
            for tok, rx in f2_tokens:
                if rx.search(line):
                    member = find_enclosing_member(raw_lines, idx)
                    rec = {"file": rel, "line": idx + 1, "member": member, "token": tok}
                    if (rel, member, tok) in allowlist:
                        f2_allowed_hits.append(rec)
                    else:
                        f2_violations.append(rec)
            if rng_literals and rel != "src/Core/RngSalts.cs":
                for lit in rng_literals:
                    if f'"{lit}"' in line or f'$"{{' not in line and f'$"{lit}' in line:
                        rng_violations.append({"file": rel, "line": idx + 1, "literal": lit})

report = {
    "filesScanned": files_scanned,
    "f1Violations": f1_violations,
    "f2AllowedHits": f2_allowed_hits,
    "f2Violations": f2_violations,
    "rngLiteralViolations": rng_violations,
}
with open(report_path, "w", encoding="utf-8") as f:
    json.dump(report, f, indent=2)
    f.write("\n")

if f1_violations or f2_violations or rng_violations:
    sys.exit(f"determinism gate failed: F1={len(f1_violations)} F2_unlisted={len(f2_violations)} RNG={len(rng_violations)} (see g6-determinism.json)")

print(f"Core determinism gate PASS ({files_scanned} files scanned; F1=0, F2_allowed={len(f2_allowed_hits)}, F2_unlisted=0)")
PY
  )"; then
    fail_stage G6 1 "$out"
  fi
  log_line "G6: ${out}"
  emit_marker G6 PASS
}

# ==============================================================================
# Stage G7 — Protected-path guard (exit 4 on any violation)
# ==============================================================================
run_stage_G7() {
  local out
  if ! out="$(python3 - "$REPO_ROOT" "$GATE_DATA_DIR/protected-paths.txt" "$GATE_DATA_DIR/frozen-anchors.json" "$BASE_COMMIT" "$EVIDENCE_DIR/g7-protected-paths.json" <<'PY'
import json, subprocess, sys

repo, rules_path, anchors_path, base_commit, report_path = sys.argv[1:]
anchors = json.load(open(anchors_path, "r", encoding="utf-8"))
t0_commit = anchors["t0Commit"]
expected_p0_tree = anchors["p0QualifiedTree"]

allow_prefixes = []
forbid_prefixes = []
deny_prefixes = []
wwwroot_tree_pin = None

for raw in open(rules_path, "r", encoding="utf-8"):
    line = raw.strip()
    if not line or line.startswith("#"):
        continue
    parts = line.split(None, 1)
    if len(parts) != 2:
        sys.exit(f"malformed protected-paths.txt line: {raw!r}")
    key, val = parts
    if key == "ALLOW_PREFIX":
        allow_prefixes.append(val)
    elif key == "FORBID_PREFIX":
        forbid_prefixes.append(val)
    elif key == "DENY_PREFIX":
        deny_prefixes.append(val)
    elif key == "WWWROOT_TREE_ID":
        wwwroot_tree_pin = val
    else:
        sys.exit(f"unknown protected-paths.txt directive: {key!r}")

if not wwwroot_tree_pin:
    sys.exit("protected-paths.txt missing WWWROOT_TREE_ID")

def changed_paths(rev_range):
    raw = subprocess.check_output(
        ["git", "-C", repo, "diff", "--name-status", "-z", rev_range],
    )
    tokens = [t.decode("utf-8", errors="surrogateescape") for t in raw.split(b"\x00") if t]
    paths = []
    i = 0
    while i < len(tokens):
        status = tokens[i]
        if status.startswith(("R", "C")):
            paths.append(tokens[i + 1])
            paths.append(tokens[i + 2])
            i += 3
        else:
            paths.append(tokens[i + 1])
            i += 2
    return sorted(set(paths))

ranges_checked = [f"{base_commit}...HEAD"]
if subprocess.run(["git", "-C", repo, "merge-base", "--is-ancestor", t0_commit, "HEAD"]).returncode == 0:
    ranges_checked.append(f"{t0_commit}...HEAD")

all_paths = sorted(set(p for r in ranges_checked for p in changed_paths(r)))
violations = []

for p in all_paths:
    if any(p.startswith(fp) for fp in forbid_prefixes):
        violations.append({"path": p, "reason": "FORBID_PREFIX"})
        continue
    if any(p.startswith(dp) for dp in deny_prefixes):
        violations.append({"path": p, "reason": "DENY_PREFIX"})
        continue
    if not any(p.startswith(ap) for ap in allow_prefixes):
        violations.append({"path": p, "reason": "OUTSIDE_ALLOW_PREFIX"})

actual_wwwroot_tree = subprocess.check_output(
    ["git", "-C", repo, "rev-parse", "HEAD:Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot"],
    text=True,
).strip()
if actual_wwwroot_tree != wwwroot_tree_pin:
    violations.append({"path": "Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot", "reason": f"WWWROOT_TREE_ID {actual_wwwroot_tree} != {wwwroot_tree_pin}"})

actual_p0_tree = subprocess.check_output(
    ["git", "-C", repo, "rev-parse", "HEAD:Tools/MultiRackSitkaLab"],
    text=True,
).strip()
if actual_p0_tree != expected_p0_tree:
    violations.append({"path": "Tools/MultiRackSitkaLab", "reason": f"P0_TREE_ID {actual_p0_tree} != {expected_p0_tree}"})

with open(report_path, "w", encoding="utf-8") as f:
    json.dump({
        "rangesChecked": ranges_checked,
        "changedPathCount": len(all_paths),
        "changedPaths": all_paths,
        "wwwrootTreeId": actual_wwwroot_tree,
        "p0TreeId": actual_p0_tree,
        "violations": violations,
    }, f, indent=2)
    f.write("\n")

if violations:
    sys.exit(f"PROTECTED-PATH VIOLATION ({len(violations)}): {violations[:5]}")

print(f"protected-path guard PASS ({len(all_paths)} paths checked; wwwroot tree={actual_wwwroot_tree[:12]}...; P0 tree={actual_p0_tree[:12]}...)")
PY
  )"; then
    fail_stage G7 4 "$out"
  fi
  log_line "G7: ${out}"
  emit_marker G7 PASS
}

# ==============================================================================
# Stage B — Optional browser tier (--browser)
# ==============================================================================
run_stage_B() {
  log_line "B: running Edge browser suites..."
  if ! bash "$P1_ROOT/tests/phase1-save-ui/run-edge-e2e.sh" "$EVIDENCE_DIR/stage-b-save-edge" > "$EVIDENCE_DIR/stage-b-save-edge.log" 2>&1; then
    fail_stage B 1 "tests/phase1-save-ui/run-edge-e2e.sh failed"
  fi
  if ! bash "$P1_ROOT/tests/phase1-time-history-ui/run-step2-edge.sh" "$EVIDENCE_DIR/stage-b-step2-edge" > "$EVIDENCE_DIR/stage-b-step2-edge.log" 2>&1; then
    fail_stage B 1 "tests/phase1-time-history-ui/run-step2-edge.sh failed"
  fi
  if [[ -x "$P1_ROOT/tests/s0c-parity/run-parity-edge.sh" ]]; then
    if ! bash "$P1_ROOT/tests/s0c-parity/run-parity-edge.sh" "$EVIDENCE_DIR/stage-b-s0c-parity" > "$EVIDENCE_DIR/stage-b-s0c-parity.log" 2>&1; then
      fail_stage B 1 "tests/s0c-parity/run-parity-edge.sh failed"
    fi
  fi
  emit_marker B PASS
}

# ==============================================================================
# Main orchestration (locked order G0 -> G1 -> G2 -> G3 -> G4 -> G5 -> G6 -> G7 -> [B])
# ==============================================================================
run_stage_G0

# Run G1 + G2 inside unshare -rn when available (spec §4.4).
if command -v unshare >/dev/null 2>&1 && unshare -rn bash -c 'ip link set lo up' 2>/dev/null; then
  log_line "Running G1-G2 inside isolated Linux network namespace (unshare -rn)..."
  netns_args=(--__internal-g1-g2 --base "$BASE_REF")
  if [[ -n "$P0REF_DIR" ]]; then
    netns_args+=(--p0ref-dir "$P0REF_DIR")
  fi
  netns_args+=("$EVIDENCE_DIR")
  netns_rc=0
  CCF_2D_GATE_INTERNAL=1 unshare -rn bash "$SCRIPT_PATH" "${netns_args[@]}" || netns_rc=$?
  if [[ "$netns_rc" -ne 0 ]]; then
    # Propagate exact exit code from G1/G2; fill remaining NOT RUN markers cleanly.
    if grep -q '^GATE G1 PASS$' "$GATE_LOG"; then
      STAGE_STATUS[G1]="PASS"
    else
      STAGE_STATUS[G1]="FAIL"
    fi
    if grep -q '^GATE G2 PASS$' "$GATE_LOG"; then
      STAGE_STATUS[G2]="PASS"
    elif [[ "${STAGE_STATUS[G1]}" == "PASS" ]]; then
      STAGE_STATUS[G2]="FAIL"
    fi
    for st in G0 G1 G2 G3 G4 G5 G6 G7 B; do
      if [[ "${STAGE_STATUS[$st]}" == "NOT RUN" ]]; then
        log_line "GATE ${st} NOT RUN"
      fi
    done
    verdict="FAIL"
    [[ "$netns_rc" -eq 3 ]] && verdict="PREREQUISITE MISSING"
    log_line "NETWORK ISOLATION: PROVEN"
    log_line "VERDICT: ${verdict} (exit=${netns_rc})"
    write_summary_json "$netns_rc" "$verdict"
    GATE_CLEAN_EXIT=1
    exit "$netns_rc"
  fi
  NET_ISOLATION="PROVEN"
  log_line "NETWORK ISOLATION: PROVEN"
  if [[ -f "$EVIDENCE_DIR/g1-g2.env" ]]; then
    # shellcheck disable=SC1090
    source "$EVIDENCE_DIR/g1-g2.env"
    if [[ -n "${HOST_NA_JOINED:-}" ]]; then
      IFS='|' read -r -a HOST_NA_LIST <<< "$HOST_NA_JOINED"
    fi
  fi
  grep -q '^GATE G1 PASS$' "$GATE_LOG" && STAGE_STATUS[G1]="PASS"
  grep -q '^GATE G2 PASS$' "$GATE_LOG" && STAGE_STATUS[G2]="PASS"
else
  NET_ISOLATION="NOT PROVEN"
  log_line "NETWORK ISOLATION: NOT PROVEN (unshare -rn unavailable)"
  run_stage_G1
  run_stage_G2
fi

run_stage_G3
run_stage_G4
run_stage_G5
run_stage_G6
run_stage_G7

# Check Stage B policy.
BROWSER_REQUIRED=0
if [[ -f "$GATE_DATA_DIR/s0c-hook-inventory.json" || -f "$P1_ROOT/tests/s0c-parity/run-parity-edge.sh" ]]; then
  BROWSER_REQUIRED=1
fi

if [[ "$WITH_BROWSER" -eq 1 ]]; then
  run_stage_B
elif [[ "$HEADLESS_ONLY" -eq 1 ]]; then
  STAGE_STATUS[B]="NOT RUN"
  log_line "GATE B NOT RUN"
else
  if [[ "$BROWSER_REQUIRED" -eq 1 ]]; then
    fail_stage B 1 "browser tier is required for this packet; re-run with --browser (or --headless-only for non-mergeable partial exit 5)"
  fi
  STAGE_STATUS[B]="NOT RUN"
  log_line "GATE B NOT RUN"
fi

# Protected local listener preservation check (OUTSIDE netns, spec §4.3).
ss -tln > "$EVIDENCE_DIR/listeners-after.txt"
if ! listener_err="$(python3 - "$EVIDENCE_DIR/listeners-before.txt" "$EVIDENCE_DIR/listeners-after.txt" <<'PY'
import re, sys
before = open(sys.argv[1], "r", encoding="utf-8").read()
after = open(sys.argv[2], "r", encoding="utf-8").read()
ports = ["33767", "43649", "46593"]
missing = []
for p in ports:
    pat = re.compile(rf":(?:{p})\s")
    if pat.search(before) and not pat.search(after):
        missing.append(p)
if missing:
    sys.exit(f"protected local listener(s) disappeared during gate run: {missing}")
PY
)"; then
  log_line "ERROR: ${listener_err}"
  log_line "VERDICT: FAIL (exit=1)"
  write_summary_json 1 "FAIL"
  GATE_CLEAN_EXIT=1
  exit 1
fi

# Final fail-closed marker verification directly from gate.log (spec §4.3).
REQUIRED_STAGES="G0 G1 G2 G3 G4 G5 G6 G7"
if [[ "$WITH_BROWSER" -eq 1 ]]; then
  REQUIRED_STAGES="$REQUIRED_STAGES B"
fi

if ! marker_err="$(python3 - "$GATE_LOG" $REQUIRED_STAGES <<'PY'
import re, sys
log_text = open(sys.argv[1], "r", encoding="utf-8").read()
stages = sys.argv[2:]
missing = []
failed = []
for st in stages:
    if re.search(rf"^GATE {re.escape(st)} FAIL\b", log_text, re.MULTILINE):
        failed.append(st)
    if not re.search(rf"^GATE {re.escape(st)} PASS$", log_text, re.MULTILINE):
        missing.append(st)
if failed or missing:
    sys.exit(f"fail-closed marker check failed: missing={missing}, failed={failed}")
PY
)"; then
  log_line "ERROR: ${marker_err}"
  log_line "VERDICT: FAIL (exit=1)"
  write_summary_json 1 "FAIL"
  GATE_CLEAN_EXIT=1
  exit 1
fi

if [[ "$HEADLESS_ONLY" -eq 1 ]]; then
  log_line "VERDICT: PARTIAL (exit=5; --headless-only browser tier skipped)"
  write_summary_json 5 "PARTIAL"
  GATE_CLEAN_EXIT=1
  exit 5
fi

log_line "VERDICT: PASS (exit=0; NETWORK ISOLATION: ${NET_ISOLATION}; P0 REFERENCE: ${P0_MODE})"
write_summary_json 0 "PASS"
GATE_CLEAN_EXIT=1
exit 0
