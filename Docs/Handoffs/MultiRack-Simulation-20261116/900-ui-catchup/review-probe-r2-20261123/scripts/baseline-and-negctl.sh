#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
NODE_BIN=$(bash -lc 'command -v node')
MAIN=/home/jer/Development/CCF-HP
WTS=/home/jer/Development/CCF-HP-Worktrees
BASE_WT=$WTS/CCF-Multi-Stage4OwnerUX-R2Review-BASE-20261123
R1_WT=$WTS/CCF-Multi-Stage4OwnerUX-R2Review-F0DB-20261123
P0SRC="$LAB/.p0ref"
cd "$MAIN" || exit 1

mk() { # mk <path> <sha>
  [ -e "$1" ] && { echo "EXISTS: $1"; return 1; }
  git worktree add --detach "$1" "$2" 2>&1 | tail -1
  mkdir -p "$1/Tools/MultiRackSitkaP1Lab/.p0ref"
  cp -p "$P0SRC"/* "$1/Tools/MultiRackSitkaP1Lab/.p0ref/"
}
mk "$BASE_WT" e7c1a57da519ed1ee03ccfbad240196a66df0905 || exit 1
mk "$R1_WT"   f0db83232641e4f2ce2517b520cd7e034644960d  || exit 1
echo "BASE head: $(git -C $BASE_WT rev-parse HEAD)"
echo "F0DB head: $(git -C $R1_WT rev-parse HEAD)"

# --- negative control: HEAD's smoke script, run against the pre-R2 page (f0db8323)
git -C "$MAIN" show 5f3b5bf3c9f711125fb61ddee55de1ff9690c19b:Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh > "$R1_WT/Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh"

for W in "$BASE_WT" "$R1_WT"; do
  L=$W/Tools/MultiRackSitkaP1Lab
  echo "=== build $(basename $W)"
  (cd "$L" && "$DOTNET" build src/Ui/SitkaP1.Ui.csproj --disable-build-servers 2>&1 | grep -E 'Warn|Error|Build succeeded' | tail -4)
done

echo "=== C1 BASE e7c1a57d: ui-smoke-multiarea.sh (its own script)"
(cd "$BASE_WT/Tools/MultiRackSitkaP1Lab" && DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke-multiarea.sh > "$EVID/C1-base-ui-smoke-multiarea.log" 2>&1)
grep -c '^PASS' "$EVID/C1-base-ui-smoke-multiarea.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/C1-base-ui-smoke-multiarea.log" | sed 's/^/  /'

echo "=== C2 BASE e7c1a57d: ui-smoke.sh (single-area)"
(cd "$BASE_WT/Tools/MultiRackSitkaP1Lab" && DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke.sh > "$EVID/C2-base-ui-smoke.log" 2>&1)
grep -c '^PASS' "$EVID/C2-base-ui-smoke.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/C2-base-ui-smoke.log" | sed 's/^/  /'

echo "=== C3 NEGATIVE CONTROL: HEAD's smoke script vs the pre-R2 page f0db8323"
(cd "$R1_WT/Tools/MultiRackSitkaP1Lab" && DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke-multiarea.sh > "$EVID/C3-negctl-f0db-page-head-tests.log" 2>&1)
grep -c '^PASS' "$EVID/C3-negctl-f0db-page-head-tests.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/C3-negctl-f0db-page-head-tests.log" | sed 's/^/  /'
echo "--- R2a..R2j lines"
grep -E '^(PASS|FAIL) R2[a-j]' "$EVID/C3-negctl-f0db-page-head-tests.log" | cut -c1-160
echo "=== DONE baseline-and-negctl"
