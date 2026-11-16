#!/usr/bin/env bash
# Layered proof that a cold P1 build + test + CLI run does not touch the P0 lab tree (spec §9.2 L0–L4, m02).
# Named build-input tree for L2–L4: the in-repo Tools/MultiRackSitkaLab of THIS worktree. The P0 Core binary itself is
# built from a `git archive` extraction outside the repository (build/build-p0-core-ref.sh); that script hashes its own
# extracted inputs before/after, which is the mechanism-(a) check. L0/L1 always run against the real repository.
#
# Usage: tests/p0-isolation.sh <evidence-dir>
# Exit 0 only if every layer passes. Never cleans, restores or chmods anything under the P0 tree.
set -euo pipefail

evidence="${1:?usage: tests/p0-isolation.sh <evidence-dir>}"
mkdir -p "$evidence"
p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
p0="$repo/Tools/MultiRackSitkaLab"
pin_commit="a7f41983c2f2f233b8fe999d1a5b10e72bdd2866"
pin_tree="f212c8f64fb28b83aa870c7b5b7c72e03051e46a"
fail=0
say() { echo "$*" | tee -a "$evidence/summary.txt"; }
: > "$evidence/summary.txt"

# L0 — no commit since the pin touched the P0 subtree
l0="$(git -C "$repo" log --oneline "$pin_commit..HEAD" -- Tools/MultiRackSitkaLab)"
if [[ -z "$l0" ]]; then say "L0 PASS: no commit in $pin_commit..HEAD touches Tools/MultiRackSitkaLab"; else say "L0 FAIL:"; say "$l0"; fail=1; fi

# L1 — committed tree identity
head_tree="$(git -C "$repo" rev-parse HEAD:Tools/MultiRackSitkaLab)"
if [[ "$head_tree" == "$pin_tree" ]]; then say "L1 PASS: HEAD P0 tree $head_tree == pin"; else say "L1 FAIL: HEAD P0 tree $head_tree != $pin_tree"; fail=1; fi

l2() { git -C "$repo" status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab; }
l3() { (cd "$repo" && git ls-files --others --ignored --exclude-standard -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z \
        | xargs -0 -r stat --printf '%n\t%s\t%.9Y\n'); }
# Also record tracked-file content hashes so an in-place rewrite of a tracked file would show even if Git's stat cache missed it.
tracked() { (cd "$repo" && git ls-files -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z | xargs -0 sha256sum); }

l2 > "$evidence/L2-before.txt"; l3 > "$evidence/L3-before.txt"; tracked > "$evidence/tracked-before.txt"

# Cold run: remove only P1-owned build output, then build the P0 Core reference, build P1, run tests and CLI under strace.
rm -rf -- "$p1_root/.p0ref" "$p1_root/src/Core/bin" "$p1_root/src/Core/obj" "$p1_root/src/Cli/bin" "$p1_root/src/Cli/obj" \
          "$p1_root/tests/Core.Tests/bin" "$p1_root/tests/Core.Tests/obj"
# #643 m-02: wider syscall set (incl. fd-based mutators and open_by_handle_at) plus chdir/fchdir/fork/clone so that
# tests/l4_analyze.py can resolve legacy relative paths against each process's working directory.
readonly L4_SYSCALLS=open,openat,openat2,creat,rename,renameat,renameat2,unlink,unlinkat,rmdir,mkdir,mkdirat,mknod,mknodat,link,linkat,symlink,symlinkat,truncate,ftruncate,fallocate,chmod,fchmod,fchmodat,chown,fchown,lchown,fchownat,utime,utimes,utimensat,futimesat,setxattr,lsetxattr,fsetxattr,removexattr,lremovexattr,fremovexattr,open_by_handle_at,chdir,fchdir,clone,clone3,fork,vfork
analyze() { python3 -I -B "$p1_root/tests/l4_analyze.py" "$@"; }

# L4 self-test first (#649). Three parts, all must pass before the real run counts:
#  (a) the analyzer's built-in synthetic cases (bare/undecoded fds -> UNRESOLVED, decoded protected fd -> HIT, benign fd and
#      read-only calls -> clean, failed attempts, unresolved dirfd/cwd, duplicate sightings do not satisfy two identities);
#  (b) a scratch decoy where EACH scripted operation targets a distinct name, so every operation has a unique
#      (syscall, target) identity; all 13 expected identities must be matched (not merely >= 13 hit records), and the decoy
#      trace must contain 0 UNRESOLVED calls (an undecoded native descriptor would surface here, never be whitelisted);
#  (c) a read-only control in the same decoy must report nothing.
# Deletion calls are not part of the decoy (this environment blocks scripted removals); they use the same rules.
# Identities are pinned to this host's coreutils/bash/python syscall choices (Ubuntu 24.04), observed in #644.
selftest_ok=1
analyze --self-test > "$evidence/L4-selftest-synthetic.txt" || selftest_ok=0
decoy="$(mktemp -d "${TMPDIR:-/tmp}/ccf-p1-l4decoy.XXXXXXXX")"
mkdir "$decoy/protected" "$decoy/outside"; echo seed > "$decoy/protected/seed.txt"; ln -s "$decoy/protected" "$decoy/outside/via"
(cd "$decoy/outside" && strace -f -y -qq -o "$evidence/L4-selftest-write.trace" -e trace="$L4_SYSCALLS" bash -c '
  cd ../protected; mkdir sub; cd sub; echo x > rel.txt; mkdir d; mv rel.txt r2.txt; ln r2.txt hl; ln -s r2.txt sl
  touch t.txt; chmod 600 r2.txt; truncate -s 0 tr.txt; cd /; echo y > "$1/outside/via/through-link.txt"
  cd "$1/protected"; python3 -I -B -c "import os; os.mkdir(\"pyrel\")"' _ "$decoy")
cat > "$evidence/L4-selftest-expected.txt" <<'EXPECTED'
# one line per scripted operation: <syscall> <target relative to the protected decoy dir>
mkdir sub
openat sub/rel.txt
mkdir sub/d
renameat2 sub/r2.txt
linkat sub/hl
symlinkat sub/sl
openat sub/t.txt
utimensat sub/t.txt
fchmodat sub/r2.txt
openat sub/tr.txt
ftruncate sub/tr.txt
openat through-link.txt
mkdir pyrel
EXPECTED
analyze "$evidence/L4-selftest-write.trace" "$decoy/protected" "$decoy/outside" --json "$evidence/L4-selftest-write.json" \
  --expect "$evidence/L4-selftest-expected.txt" > "$evidence/L4-selftest-write.txt" || true
read -r decoy_matched decoy_expected decoy_unresolved < <(python3 -I -B -c '
import json, sys
r = json.load(open(sys.argv[1]))
print(r["expectCheck"]["matched"], r["expectCheck"]["expected"], len(r["unresolved"]))' "$evidence/L4-selftest-write.json")
(cd "$decoy/outside" && strace -f -y -qq -o "$evidence/L4-selftest-read.trace" -e trace="$L4_SYSCALLS" bash -c '
  cat ../protected/seed.txt > /dev/null; ls -la ../protected > /dev/null; cd ../protected; cat seed.txt > /dev/null')
selftest_read_rc=0
analyze "$evidence/L4-selftest-read.trace" "$decoy/protected" "$decoy/outside" > "$evidence/L4-selftest-read.txt" || selftest_read_rc=$?
if [[ "$selftest_ok" == 1 && "$decoy_expected" == 13 && "$decoy_matched" == 13 && "$decoy_unresolved" == 0 && "$selftest_read_rc" == 0 ]]; then
  say "L4 self-test PASS: synthetic $(tail -1 "$evidence/L4-selftest-synthetic.txt"); decoy $decoy_matched/$decoy_expected distinct expected operation identities matched, 0 unresolved; read-only control clean"
else say "L4 self-test FAIL: synthetic ok=$selftest_ok; decoy identities $decoy_matched/$decoy_expected, unresolved $decoy_unresolved; read control exit $selftest_read_rc"; fail=1; fi

cd "$p1_root"
strace -f -y -qq -o "$evidence/L4-strace.txt" \
  -e trace="$L4_SYSCALLS" \
  bash -c '
    set -e
    cd "$1"
    build/build-p0-core-ref.sh
    dotnet build MultiRackSitkaP1Lab.sln --nologo -v quiet
    dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$2/tests.txt"
    dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll verify-derived p1d-30m-sitka-adult-v1 fixtures/derived > "$2/cli-verify-derived.txt"
    dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll generate-manifest --check fixtures/derived > "$2/cli-generate-check.txt"
    dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll generate-manifest --stdout | sha256sum > "$2/cli-generate-stdout.sha256"
    for id in oracle-quarter-ha sitka-flat-demo-v1 sitka-flat-perf-2000-v1; do
      dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll inspect-original "$3/fixtures/synthetic" "$id" --review-evidence >> "$2/cli-inspect-original.txt"
      dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll inspect-original "$3/fixtures/synthetic" "$id" >> "$2/cli-inspect-original.txt" 2>&1 || true
    done
    for d in DerivedP1ThirtyMetreExperimentV1 DerivedUnitySquareParityV1 OriginalPolygonExploratoryV1; do
      dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual "$d" p1d-30m-sitka-adult-v1 >> "$2/cli-annual.txt" 2>&1 || true
    done
  ' _ "$p1_root" "$evidence" "$p0"

l2 > "$evidence/L2-after.txt"; l3 > "$evidence/L3-after.txt"; tracked > "$evidence/tracked-after.txt"

if cmp -s "$evidence/L2-before.txt" "$evidence/L2-after.txt" && [[ ! -s "$evidence/L2-after.txt" ]]; then
  say "L2 PASS: git status --porcelain --ignored --untracked-files=all is empty before and after"
elif cmp -s "$evidence/L2-before.txt" "$evidence/L2-after.txt"; then
  say "L2 PASS (non-empty, unchanged baseline of $(wc -l < "$evidence/L2-before.txt") lines)"
else say "L2 FAIL: status changed"; fail=1; fi

if cmp -s "$evidence/L3-before.txt" "$evidence/L3-after.txt"; then
  say "L3 PASS: ignored-file path/size/mtime-ns snapshot unchanged ($(wc -l < "$evidence/L3-before.txt") ignored files)"
else say "L3 FAIL: ignored-file snapshot changed"; fail=1; fi
if cmp -s "$evidence/tracked-before.txt" "$evidence/tracked-after.txt"; then
  say "L3+ PASS: SHA-256 of all $(wc -l < "$evidence/tracked-before.txt") tracked P0 files unchanged"
else say "L3+ FAIL: a tracked P0 file changed"; fail=1; fi

# L4 — every traced mutating call, with relative paths resolved against tracked working directories (tests/l4_analyze.py).
# Scope of the claim: the traced syscall set, in processes descended from the traced command, during this run only.
l4_rc=0
analyze "$evidence/L4-strace.txt" "$p0" "$p1_root" --json "$evidence/L4-analysis.json" > "$evidence/L4-analysis.txt" || l4_rc=$?
if [[ "$l4_rc" == 0 ]]; then
  say "L4 PASS (traced scope only): $(head -1 "$evidence/L4-analysis.txt"); $(wc -l < "$evidence/L4-strace.txt") trace lines"
else say "L4 FAIL: $(head -1 "$evidence/L4-analysis.txt")"; fail=1; fi
say "L4 limits: untraced syscalls (e.g. io_uring), processes outside the traced tree, and symlinked intermediate components in legacy relative paths are not observed; L2/L3/L3+ are the independent content and metadata checks"

say "P1 tests: $(tail -1 "$evidence/tests.txt")"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL)"
exit "$fail"
