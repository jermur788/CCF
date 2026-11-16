#!/usr/bin/env bash
# Build the qualified P0 SitkaLab.Core assembly as a read-only BINARY reference for P1, without writing anything under
# Tools/MultiRackSitkaLab (spec §9.2 "build-output isolation", mechanism (a)).
#
# How:
#   1. Check that the P0 subtree in this checkout is exactly the #589-qualified tree. Stop if it is not.
#   2. `git archive` that TREE OBJECT (not the working tree) and extract it into a fresh temporary directory OUTSIDE the
#      repository, so restore/bin/obj are created there and never inside the P0 tree.
#   3. Hash every extracted input before and after the build; any change stops the script.
#   4. Copy only the built SitkaLab.Core.dll into the P1-owned, git-ignored .p0ref/ folder and record its SHA-256 and inputs.
#   5. Remove the temporary directory this script created (only that directory).
#
# Usage: build/build-p0-core-ref.sh            (run from anywhere inside the P1 worktree)
set -euo pipefail

readonly P0_QUALIFIED_TREE="f212c8f64fb28b83aa870c7b5b7c72e03051e46a"   # #589 Manager-selected qualified P0 subtree
readonly P0_QUALIFIED_COMMIT="a7f41983c2f2f233b8fe999d1a5b10e72bdd2866" # #589 qualified source commit (provenance)

p1_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd -P)"
repo_root="$(git -C "$p1_root" rev-parse --show-toplevel)"
out_dir="$p1_root/.p0ref"

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

mkdir -p "$out_dir"
cp "$work/bin/SitkaLab.Core.dll" "$out_dir/SitkaLab.Core.dll"
dll_sha="$(sha256sum "$out_dir/SitkaLab.Core.dll" | cut -d' ' -f1)"
{
  echo "p0QualifiedCommit=$P0_QUALIFIED_COMMIT"
  echo "p0QualifiedTree=$P0_QUALIFIED_TREE"
  echo "sdk=$(cd "$src" && dotnet --version)"
  echo "dllSha256=$dll_sha"
  echo "inputs:"
  cat "$work/inputs-before.sha256"
} > "$out_dir/provenance.txt"
echo "$dll_sha  SitkaLab.Core.dll" > "$out_dir/SitkaLab.Core.dll.sha256"
echo "P0 Core reference built: $out_dir/SitkaLab.Core.dll sha256=$dll_sha"
