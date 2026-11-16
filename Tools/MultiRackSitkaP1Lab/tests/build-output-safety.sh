#!/usr/bin/env bash
# #643 M-01 direct test of build/build-p0-core-ref.sh against hostile states of the P1-owned output root `.p0ref`.
# Every "victim" is a test-created scratch stand-in (never real P0). The existing `.p0ref` (P1's own rebuildable build
# output) is moved into this run's scratch folder first and moved back if the run stops early; the last case rebuilds it.
#
# Usage: tests/build-output-safety.sh <evidence-dir>       (TMPDIR must be outside the repository)
set -euo pipefail
evidence="${1:?usage: tests/build-output-safety.sh <evidence-dir>}"
mkdir -p "$evidence"
p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
builder="$p1_root/build/build-p0-core-ref.sh"
out="$p1_root/.p0ref"
expected_sha="9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3"
log="$evidence/build-output-safety.txt"; : > "$log"
say() { echo "$*" | tee -a "$log"; }

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-p1-buildsafety.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; exit 2 ;; esac
# Hard links need the same filesystem as .p0ref, so that victim lives in a test-owned folder under P1's ignored evidence/.
same_fs="$(mktemp -d "$p1_root/evidence/buildsafety.XXXXXXXX")"
saved=""
if [[ -e "$out" || -L "$out" ]]; then saved="$scratch/saved-p0ref"; mv -T -- "$out" "$saved"; fi
restore() {
  if [[ -n "$saved" && ! -e "$out" && ! -L "$out" ]]; then mv -T -- "$saved" "$out"; fi
}
trap restore EXIT

p0_digest() { (cd "$repo" && git ls-files -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum | cut -c1-64); }
p0_status() { git -C "$repo" status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab | wc -l; }
p0_before="$(p0_digest)"; status_before="$(p0_status)"
tree_hash() { (cd "$1" && find . -printf '%y %p %s %n\n' | LC_ALL=C sort && find . -type f -print0 | LC_ALL=C sort -z | xargs -0 -r sha256sum); }
fail=0

# expect_refusal <label> : builder must exit 4 and the victim must be byte/inode-count identical afterwards.
expect_refusal() {
  local label="$1" victim="$2"
  local before after rc=0
  before="$(tree_hash "$victim")"
  "$builder" > "$evidence/case-$label.out" 2>&1 || rc=$?
  after="$(tree_hash "$victim")"
  if [[ "$rc" == 4 && "$before" == "$after" ]]; then
    say "PASS $label: exit $rc, victim unchanged — $(grep STOP "$evidence/case-$label.out" | head -1)"
  else
    say "FAIL $label: exit $rc, victim $( [[ "$before" == "$after" ]] && echo unchanged || echo CHANGED )"; fail=1
  fi
}

# C1 .p0ref is a symlink to a P0-like stand-in directory (the #643 scenario).
fake_p0="$scratch/fake/Tools/MultiRackSitkaLab"; mkdir -p "$fake_p0"; echo "protected" > "$fake_p0/README.md"
ln -s "$fake_p0" "$out"
expect_refusal "C1-p0ref-symlink-to-dir" "$scratch/fake"
[[ -L "$out" ]] && rm -- "$out"

# C2 .p0ref is a symlink to a missing target (mkdir -p would have created it there).
ln -s "$scratch/fake/created-by-builder" "$out"
expect_refusal "C2-p0ref-dangling-symlink" "$scratch/fake"
[[ -e "$scratch/fake/created-by-builder" ]] && { say "FAIL C2: target was created"; fail=1; }
rm -- "$out"

# C3 .p0ref is a regular file.
echo "not a dir" > "$out"
expect_refusal "C3-p0ref-regular-file" "$scratch/fake"
rm -- "$out"

# C4 .p0ref/SitkaLab.Core.dll is a symlink to a stand-in file.
mkdir "$out"; echo "victim" > "$scratch/fake/victim.dll"; ln -s "$scratch/fake/victim.dll" "$out/SitkaLab.Core.dll"
expect_refusal "C4-output-file-symlink" "$scratch/fake"
rm -- "$out/SitkaLab.Core.dll"

# C5 .p0ref/provenance.txt is a hard link to a stand-in file (same filesystem).
echo "victim" > "$same_fs/victim.txt"; ln "$same_fs/victim.txt" "$out/provenance.txt"
expect_refusal "C5-output-file-hardlink" "$same_fs"
rm -- "$out/provenance.txt"

# C6 .p0ref/SitkaLab.Core.dll.sha256 is a directory.
mkdir "$out/SitkaLab.Core.dll.sha256"
expect_refusal "C6-output-name-is-directory" "$scratch/fake"
rmdir -- "$out/SitkaLab.Core.dll.sha256"; rmdir -- "$out"

# C7 positive: absent root -> build succeeds, three regular single-link files, expected DLL hash.
rc=0; "$builder" > "$evidence/case-C7-cold.out" 2>&1 || rc=$?
dll_sha="$(sha256sum "$out/SitkaLab.Core.dll" | cut -c1-64)"
if [[ "$rc" == 0 && "$dll_sha" == "$expected_sha" && ! -L "$out" && "$(find "$out" -maxdepth 1 -type f -links 1 | wc -l)" == 3 && "$(find "$out" -mindepth 1 | wc -l)" == 3 ]]; then
  say "PASS C7-cold-build: exit 0, DLL sha256 $dll_sha, 3 regular single-link files, no leftover temp names"
else say "FAIL C7-cold-build: exit $rc sha $dll_sha"; fail=1; fi

# C8 positive: rebuild over existing regular files (rename replaces them) -> same hash.
rc=0; "$builder" > "$evidence/case-C8-rebuild.out" 2>&1 || rc=$?
dll_sha="$(sha256sum "$out/SitkaLab.Core.dll" | cut -c1-64)"
if [[ "$rc" == 0 && "$dll_sha" == "$expected_sha" && "$(find "$out" -mindepth 1 | wc -l)" == 3 ]]; then
  say "PASS C8-rebuild: exit 0, DLL sha256 unchanged"
else say "FAIL C8-rebuild: exit $rc sha $dll_sha"; fail=1; fi

saved_note="none existed"
if [[ -n "$saved" ]]; then
  cmp -s "$saved/SitkaLab.Core.dll" "$out/SitkaLab.Core.dll" && saved_note="identical DLL rebuilt; previous copy left at $saved" || saved_note="DIFFERENT"
fi
say "previous .p0ref: $saved_note"
p0_after="$(p0_digest)"; status_after="$(p0_status)"
if [[ "$p0_before" == "$p0_after" && "$status_before" == "$status_after" && "$status_after" == 0 ]]; then
  say "PASS P0: tracked-file digest $p0_after unchanged; P0 status/ignored lines 0 before and after"
else say "FAIL P0 changed"; fail=1; fi
rm -rf -- "$same_fs"
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL)"
exit "$fail"
