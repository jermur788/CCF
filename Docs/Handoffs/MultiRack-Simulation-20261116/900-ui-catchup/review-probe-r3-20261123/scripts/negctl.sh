#!/bin/bash
# negctl.sh : HEAD's smoke witness vs the UNFIXED 5f3b5bf3 page (throwaway detached worktree; my R2 review worktree is not touched),
# and the BASE-vs-HEAD comparison of the pre-existing failures using the R2 review's committed BASE log.
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
NODE_BIN=$(bash -lc 'command -v node')
MAIN=/home/jer/Development/CCF-HP
WTS=/home/jer/Development/CCF-HP-Worktrees
NEG=$WTS/CCF-Multi-Stage4OwnerUX-R3ReReview-PREV-20261123
P0SRC="$LAB/.p0ref"
cd "$MAIN" || exit 1
[ -e "$NEG" ] && { echo "EXISTS: $NEG"; exit 1; }
git worktree add --detach "$NEG" 5f3b5bf3c9f711125fb61ddee55de1ff9690c19b 2>&1 | grep -v 'Updating files' | tail -1
mkdir -p "$NEG/Tools/MultiRackSitkaP1Lab/.p0ref"
cp -p "$P0SRC"/* "$NEG/Tools/MultiRackSitkaP1Lab/.p0ref/"
echo "NEG head: $(git -C $NEG rev-parse HEAD)"
# HEAD's smoke script, into the throwaway worktree only
git -C "$MAIN" show 0e7774557dbb2947b29993cf032c084aa3b7227f:Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh > "$NEG/Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh"
L=$NEG/Tools/MultiRackSitkaP1Lab
(cd "$L" && "$DOTNET" build src/Ui/SitkaP1.Ui.csproj --disable-build-servers 2>&1 | grep -E 'Warn|Error|Build succeeded' | tail -3)
echo "=== C3 NEGATIVE CONTROL: HEAD's ui-smoke-multiarea.sh vs the unfixed page 5f3b5bf3"
(cd "$L" && DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke-multiarea.sh > "$EVID/C3-negctl-5f3b-page-head-tests.log" 2>&1)
grep -c '^PASS' "$EVID/C3-negctl-5f3b-page-head-tests.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/C3-negctl-5f3b-page-head-tests.log" | cut -c1-110 | sed 's/^/  /'
echo "--- R3a..R3j lines vs the unfixed page"
grep -E '^(PASS|FAIL) R3[a-j]' "$EVID/C3-negctl-5f3b-page-head-tests.log" | cut -c1-150
echo "=== DONE negctl"
