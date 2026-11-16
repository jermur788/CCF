#!/usr/bin/env bash
# Build the qualified P0 SitkaLab.Core assembly as a read-only BINARY reference for P1, without writing anything under
# Tools/MultiRackSitkaLab (spec §9.2 "build-output isolation", mechanism (a)).
#
# How:
#   1. Check that the P0 subtree in this checkout is exactly the #589-qualified tree. Stop if it is not.
#   2. `git archive` that TREE OBJECT (not the working tree) and extract it into a fresh temporary directory OUTSIDE the
#      repository, so restore/bin/obj are created there and never inside the P0 tree.
#   3. Hash every extracted input before and after the build; any change stops the script.
#   4. Install only the built SitkaLab.Core.dll, its SHA-256 and provenance into the P1-owned, git-ignored .p0ref/ folder,
#      after verifying .p0ref and each destination is not a symlink, foreign object or hard link (#643 M-01).
#   5. Remove the temporary directory this script created (only that directory).
#
# Usage: build/build-p0-core-ref.sh            (run from anywhere inside the P1 worktree)
set -euo pipefail

readonly P0_QUALIFIED_TREE="f212c8f64fb28b83aa870c7b5b7c72e03051e46a"   # #589 Manager-selected qualified P0 subtree
readonly P0_QUALIFIED_COMMIT="a7f41983c2f2f233b8fe999d1a5b10e72bdd2866" # #589 qualified source commit (provenance)

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo_root="$(git -C "$p1_root" rev-parse --show-toplevel)"
out_dir="$p1_root/.p0ref"
readonly OUT_FILES=(SitkaLab.Core.dll SitkaLab.Core.dll.sha256 provenance.txt)

# ---- #643 M-01: the output root must be the real, P1-owned .p0ref, never a link or a foreign object --------------------
# The P1 root must be exactly <repo>/Tools/MultiRackSitkaP1Lab, reached without any symlink, and must not be inside P0.
if [[ -L "$repo_root/Tools/MultiRackSitkaP1Lab" || "$p1_root" != "$(cd "$repo_root/Tools/MultiRackSitkaP1Lab" && pwd -P)" ]]; then
  echo "STOP: P1 root is not the real Tools/MultiRackSitkaP1Lab directory" >&2; exit 4
fi
case "$p1_root/" in "$repo_root/Tools/MultiRackSitkaLab/"*) echo "STOP: P1 root is inside P0" >&2; exit 4 ;; esac

check_output_root() {
  if [[ -L "$out_dir" ]]; then echo "STOP: $out_dir is a symbolic link; refusing to write through it" >&2; exit 4; fi
  [[ -e "$out_dir" ]] || return 0
  if [[ ! -d "$out_dir" ]]; then echo "STOP: $out_dir exists and is not a directory" >&2; exit 4; fi
  if [[ "$(stat -c %u -- "$out_dir")" != "$(id -u)" ]]; then echo "STOP: $out_dir is not owned by the current user" >&2; exit 4; fi
  if [[ "$(cd "$out_dir" && pwd -P)" != "$p1_root/.p0ref" ]]; then echo "STOP: $out_dir resolves elsewhere" >&2; exit 4; fi
  local name
  for name in "${OUT_FILES[@]}"; do
    local f="$out_dir/$name"
    if [[ -L "$f" ]]; then echo "STOP: $f is a symbolic link" >&2; exit 4; fi
    [[ -e "$f" ]] || continue
    if [[ ! -f "$f" ]]; then echo "STOP: $f is not a regular file" >&2; exit 4; fi
    if [[ "$(stat -c %h -- "$f")" != "1" ]]; then echo "STOP: $f has more than one hard link" >&2; exit 4; fi
  done
}
check_output_root   # fail before any build work

head_tree="$(git -C "$repo_root" rev-parse HEAD:Tools/MultiRackSitkaLab)"
pin_tree="$(git -C "$repo_root" rev-parse "$P0_QUALIFIED_COMMIT:Tools/MultiRackSitkaLab")"
if [[ "$head_tree" != "$P0_QUALIFIED_TREE" || "$pin_tree" != "$P0_QUALIFIED_TREE" ]]; then
  echo "STOP: P0 tree mismatch (HEAD $head_tree, pin $pin_tree, expected $P0_QUALIFIED_TREE)" >&2
  exit 2
fi

tmp_parent="${TMPDIR:-/tmp}"
case "$(cd "$tmp_parent" && pwd -P)/" in
  "$repo_root"/*) echo "STOP: TMPDIR is inside the repository; extraction must be outside it" >&2; exit 2 ;;
esac
work="$(mktemp -d "$tmp_parent/ccf-p1-p0core.XXXXXXXX")"
cleanup() { rm -rf -- "$work"; }
trap cleanup EXIT

src="$work/p0"
mkdir "$src"
git -C "$repo_root" archive --format=tar "$P0_QUALIFIED_TREE" global.json Directory.Build.props src/Core | tar -x -C "$src"

hash_inputs() { (cd "$src" && find . -type f -print0 | LC_ALL=C sort -z | xargs -0 sha256sum); }
hash_inputs > "$work/inputs-before.sha256"

# All build products go to directories this script owns, outside the repository.
dotnet build "$src/src/Core/SitkaLab.Core.csproj" -c Release --nologo -v quiet \
  -p:BaseIntermediateOutputPath="$work/obj/" -p:OutputPath="$work/bin/" \
  -p:ContinuousIntegrationBuild=true -p:PathMap="$work=/ccf-p1-p0core" >/dev/null

hash_inputs > "$work/inputs-after.sha256"
if ! cmp -s "$work/inputs-before.sha256" "$work/inputs-after.sha256"; then
  echo "STOP: the build changed an extracted P0 input" >&2
  diff "$work/inputs-before.sha256" "$work/inputs-after.sha256" >&2 || true
  exit 3
fi
if [[ -e "$src/src/Core/bin" || -e "$src/src/Core/obj" ]]; then
  echo "STOP: build output appeared beside the extracted P0 sources" >&2
  exit 3
fi

# Prepare all three outputs in the external work directory first.
cp "$work/bin/SitkaLab.Core.dll" "$work/SitkaLab.Core.dll"
dll_sha="$(sha256sum "$work/SitkaLab.Core.dll" | cut -d' ' -f1)"
{
  echo "p0QualifiedCommit=$P0_QUALIFIED_COMMIT"
  echo "p0QualifiedTree=$P0_QUALIFIED_TREE"
  echo "sdk=$(cd "$src" && dotnet --version)"
  echo "dllSha256=$dll_sha"
  echo "inputs:"
  cat "$work/inputs-before.sha256"
} > "$work/provenance.txt"
echo "$dll_sha  SitkaLab.Core.dll" > "$work/SitkaLab.Core.dll.sha256"

# Install into the verified P1-owned root. Re-check immediately before; create the root with plain mkdir (never -p
# through an existing link); write each file to a fresh exclusive temp name (mktemp uses O_CREAT|O_EXCL) and rename(2) it
# over the final name. rename replaces the NAME: it never follows a symlink at the destination and never writes into the
# inode of a hard-linked destination. Residual risk: a same-user process racing between these steps (local script; see README).
check_output_root
[[ -d "$out_dir" ]] || mkdir -m 0755 -- "$out_dir"
root_id="$(stat -c %d:%i -- "$out_dir")"
installing=()
cleanup_install() { local t; for t in "${installing[@]}"; do [[ -e "$t" || -L "$t" ]] && rm -f -- "$t"; done; cleanup; }
trap cleanup_install EXIT
for name in "${OUT_FILES[@]}"; do
  tmp="$(mktemp -- "$out_dir/.install.XXXXXXXX")"
  installing+=("$tmp")
  cat -- "$work/$name" > "$tmp"
  mv -fT -- "$tmp" "$out_dir/$name"
done
installing=()
if [[ -L "$out_dir" || "$(stat -c %d:%i -- "$out_dir")" != "$root_id" ]]; then
  echo "STOP: $out_dir changed during installation" >&2; exit 4
fi
check_output_root
echo "P0 Core reference built: $out_dir/SitkaLab.Core.dll sha256=$dll_sha"
