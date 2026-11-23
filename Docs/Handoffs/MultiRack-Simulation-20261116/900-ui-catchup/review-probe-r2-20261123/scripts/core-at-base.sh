#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
BASE_WT=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2Review-BASE-20261123
L="$BASE_WT/Tools/MultiRackSitkaP1Lab"
cd "$L" || exit 1
echo "BASE worktree HEAD: $(git -C $BASE_WT rev-parse HEAD)"
echo "P0 pin in BASE worktree (copied read-only): $(sha256sum .p0ref/SitkaLab.Core.dll | cut -c1-64)"
"$DOTNET" build MultiRackSitkaP1Lab.sln --disable-build-servers 2>&1 | grep -E 'Warn|Error|Build succeeded' | tail -3 > "$EVID/D1-base-build.log"; cat "$EVID/D1-base-build.log"
"$DOTNET" tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$EVID/D2-base-core.log" 2>&1
echo "core exit: $?"
grep -E 'fingerprint|passed' "$EVID/D2-base-core.log" | sed -E 's/^ +//'
echo "--- HEAD (this review) anchors for comparison:"
grep -E 'fingerprint|passed' "$EVID/A3-core.log" | sed -E 's/^ +//'
echo "--- identical anchor lines BASE vs HEAD?"
diff <(grep -E 'fingerprint' "$EVID/D2-base-core.log") <(grep -E 'fingerprint' "$EVID/A3-core.log") && echo "anchor fingerprints identical: YES"
