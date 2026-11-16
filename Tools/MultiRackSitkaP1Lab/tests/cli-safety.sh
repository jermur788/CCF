#!/usr/bin/env bash
# #643 B-01 / M-02 direct tests of the documented `sitkap1` CLI (not only library helpers). Every hostile target is a
# scratch stand-in under TMPDIR, including a stand-in copy of a P0 fixture placed at a P0-looking path. Real P0 is only
# read for its digest before and after.
#
# Usage: tests/cli-safety.sh <evidence-dir>     (build first; TMPDIR must be outside the repository)
set -euo pipefail
evidence="${1:?usage: tests/cli-safety.sh <evidence-dir>}"
mkdir -p "$evidence"
p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo="$(git -C "$p1_root" rev-parse --show-toplevel)"
cli=(dotnet "$p1_root/src/Cli/bin/Debug/net8.0/sitkap1.dll")
fixture_dir="$p1_root/fixtures/derived"
name="p1d-30m-sitka-adult-v1.sitkap1fx.json"
id="p1d-30m-sitka-adult-v1"
committed_sha="bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4"
log="$evidence/cli-safety.txt"; : > "$log"
say() { echo "$*" | tee -a "$log"; }
fail=0

scratch="$(mktemp -d "${TMPDIR:-/tmp}/ccf-p1-clisafety.XXXXXXXX")"
case "$(cd "$scratch" && pwd -P)/" in "$repo"/*) echo "STOP: TMPDIR inside repository" >&2; exit 2 ;; esac
p0_digest() { (cd "$repo" && git ls-files -z -- Tools/MultiRackSitkaLab | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum | cut -c1-64); }
p0_before="$(p0_digest)"

# Stand-in "protected P0" tree with a copy of a real fixture.
standin="$scratch/standin/Tools/MultiRackSitkaLab/fixtures/synthetic"
mkdir -p "$standin"
cp "$repo/Tools/MultiRackSitkaLab/fixtures/synthetic/oracle-quarter-ha.sitkalab.json" "$standin/"
standin_file="$standin/oracle-quarter-ha.sitkalab.json"
standin_sha="$(sha256sum "$standin_file" | cut -c1-64)"

# check <label> <expected-exit> <expected-text-or-empty> -- command...
check() {
  local label="$1" want="$2" text="$3"; shift 4
  local rc=0
  "$@" > "$evidence/$label.out" 2> "$evidence/$label.err" || rc=$?
  if [[ "$rc" == "$want" && ( -z "$text" || -n "$(grep -h -- "$text" "$evidence/$label.out" "$evidence/$label.err")" ) ]]; then
    say "PASS $label: exit $rc ${text:+($text)}"
  else
    say "FAIL $label: exit $rc (wanted $want ${text})"; sed 's/^/    /' "$evidence/$label.err" | tee -a "$log"; fail=1
  fi
}

# ---- B-01 generate-manifest -------------------------------------------------------------------------------------------
rc=0; got="$("${cli[@]}" generate-manifest --stdout 2> "$evidence/B01-pipe.err" | sha256sum | cut -c1-64)" || rc=$?
if [[ "$rc" == 0 && "$got" == "$committed_sha" ]]; then say "PASS B01-stdout-pipe: streamed bytes sha256 = committed fixture"; else say "FAIL B01-stdout-pipe: $got"; fail=1; fi

# stdout redirected in APPEND mode onto the P0-looking stand-in (append, so the shell itself truncates nothing).
check B01-stdout-to-standin-p0-file 2 OutputNotStream -- bash -c '"$@" >> "'"$standin_file"'"' _ "${cli[@]}" generate-manifest --stdout
check B01-stdout-to-new-file 2 OutputNotStream -- bash -c '"$@" >> "'"$scratch/new-out.json"'"' _ "${cli[@]}" generate-manifest --stdout
[[ ! -s "$scratch/new-out.json" ]] && say "PASS B01-new-file-left-empty: 0 bytes written by P1" || { say "FAIL B01 new file has content"; fail=1; }

# The pre-repair path form, aimed at the stand-in, a traversal and a new name: usage refusal, nothing created or changed.
check B01-old-path-form-standin 64 usage -- "${cli[@]}" generate-manifest "$standin_file"
check B01-old-path-form-traversal 64 usage -- "${cli[@]}" generate-manifest "$scratch/standin/Tools/MultiRackSitkaLab/../MultiRackSitkaLab/fixtures/synthetic/oracle-quarter-ha.sitkalab.json"
check B01-old-path-form-new-name 64 usage -- "${cli[@]}" generate-manifest "$scratch/standin/created.json"
check B01-extra-argument 64 usage -- "${cli[@]}" generate-manifest --stdout "$standin_file"
[[ ! -e "$scratch/standin/created.json" ]] && say "PASS B01-no-file-created" || { say "FAIL B01 file created"; fail=1; }
[[ "$(sha256sum "$standin_file" | cut -c1-64)" == "$standin_sha" ]] && say "PASS B01-standin-p0-fixture-unchanged" || { say "FAIL B01 stand-in changed"; fail=1; }

check B01-check-committed 0 "identical to committed fixture" -- "${cli[@]}" generate-manifest --check "$fixture_dir"

# ---- M-02 verify-derived ----------------------------------------------------------------------------------------------
good="$scratch/good"; mkdir "$good"; cp "$fixture_dir/$name" "$good/"
check M02-good-directory 0 "trees=100" -- "${cli[@]}" verify-derived "$id" "$good"
check M02-old-file-path-form 2 DirectorySymlinkRefused -- "${cli[@]}" verify-derived "$id" "$good/$name"
d="$scratch/symleaf"; mkdir "$d"; ln -s "$good/$name" "$d/$name"
check M02-symlink-leaf 2 SymlinkRefused -- "${cli[@]}" verify-derived "$id" "$d"
ln -s "$scratch" "$scratch/ancestor-link"
check M02-symlink-ancestor 2 DirectorySymlinkRefused -- "${cli[@]}" verify-derived "$id" "$scratch/ancestor-link/good"
d="$scratch/hard"; mkdir "$d"; cp "$fixture_dir/$name" "$d/"; ln "$d/$name" "$d/alias"
check M02-hardlink 2 HardLinked -- "${cli[@]}" verify-derived "$id" "$d"
d="$scratch/fifo"; mkdir "$d"; mkfifo "$d/$name"
check M02-fifo-no-hang 2 NotRegularFile -- timeout 30 "${cli[@]}" verify-derived "$id" "$d"
d="$scratch/dir"; mkdir -p "$d/$name"
check M02-directory 2 NotRegularFile -- "${cli[@]}" verify-derived "$id" "$d"
d="$scratch/big"; mkdir "$d"; { cat "$fixture_dir/$name"; head -c 67108864 /dev/zero; } > "$d/$name"
check M02-oversized-64MiB 2 FileSizeMismatch -- "${cli[@]}" verify-derived "$id" "$d"
d="$scratch/wrongdomain"; mkdir "$d"; sed 's/"domain":"DerivedP1ThirtyMetreExperimentV1"/"domain":"OriginalPolygonExploratoryV1"/' "$fixture_dir/$name" > "$d/$name"
check M02-wrong-domain-bytes 2 FileSizeMismatch -- "${cli[@]}" verify-derived "$id" "$d"
check M02-original-id 2 NotDerivedFixtureId -- "${cli[@]}" verify-derived oracle-quarter-ha "$standin"
check M02-missing 2 FileMissing -- "${cli[@]}" verify-derived "$id" "$scratch"

p0_after="$(p0_digest)"
[[ "$p0_before" == "$p0_after" ]] && say "PASS P0 tracked-file digest unchanged ($p0_after)" || { say "FAIL P0 digest changed"; fail=1; }
say "OVERALL: $([[ $fail == 0 ]] && echo PASS || echo FAIL)"
exit "$fail"
