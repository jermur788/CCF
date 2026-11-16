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
strace -f -y -qq -o "$evidence/L4-strace.txt" \
  -e trace=open,openat,openat2,creat,rename,renameat,renameat2,unlink,unlinkat,mkdir,mkdirat,link,linkat,symlink,symlinkat,truncate,chmod,fchmodat,chown,fchownat,utimensat,setxattr,removexattr \
  bash -c '
    set -e
    cd "$1"
    build/build-p0-core-ref.sh
    dotnet build MultiRackSitkaP1Lab.sln --nologo -v quiet
    dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$2/tests.txt"
    dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll verify-derived p1d-30m-sitka-adult-v1 fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json > "$2/cli-verify-derived.txt"
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

# L4 — any write-intent or mutating call naming the P0 tree. -y decodes descriptor paths, so dirfd-relative calls are covered.
p0_esc="$(printf '%s' "$p0/" | sed 's/[.[\*^$/]/\\&/g')"
grep -E "$p0_esc" "$evidence/L4-strace.txt" > "$evidence/L4-p0-lines.txt" || true
grep -E 'O_WRONLY|O_RDWR|O_CREAT|O_TRUNC|O_APPEND|^[0-9]+ +(creat|rename|renameat2?|unlink|unlinkat|mkdir|mkdirat|link|linkat|symlink|symlinkat|truncate|chmod|fchmodat|chown|fchownat|utimensat|setxattr|removexattr)\(' \
  "$evidence/L4-p0-lines.txt" > "$evidence/L4-p0-writes.txt" || true
total_calls="$(wc -l < "$evidence/L4-strace.txt")"
p0_calls="$(wc -l < "$evidence/L4-p0-lines.txt")"
if [[ ! -s "$evidence/L4-p0-writes.txt" ]]; then
  say "L4 PASS: 0 write-intent/mutating calls on the P0 tree ($p0_calls read-only P0 calls among $total_calls traced file calls)"
else say "L4 FAIL: $(wc -l < "$evidence/L4-p0-writes.txt") write-intent calls on the P0 tree"; fail=1; fi

say "P1 tests: $(tail -1 "$evidence/tests.txt")"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL)"
exit "$fail"
