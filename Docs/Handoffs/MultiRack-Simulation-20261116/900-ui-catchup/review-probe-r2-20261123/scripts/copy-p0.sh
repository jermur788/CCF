#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
SRC=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123/Tools/MultiRackSitkaP1Lab/.p0ref
DST="$LAB/.p0ref"
mkdir -p "$DST"
cp -p "$SRC/SitkaLab.Core.dll" "$SRC/SitkaLab.Core.dll.sha256" "$SRC/provenance.txt" "$DST/"
cd "$DST"
echo "--- shipped .sha256 file"; cat SitkaLab.Core.dll.sha256
echo "--- computed (BEFORE any build/test)"
GOT=$(sha256sum SitkaLab.Core.dll | cut -d' ' -f1); echo "$GOT"
[ "$GOT" = "$P0EXPECT" ] && echo "P0 BEFORE == expected 9a99f781...: OK" || echo "P0 BEFORE MISMATCH"
sha256sum -c SitkaLab.Core.dll.sha256 2>&1 | head -3
echo "$GOT" > "$EVID/p0-before.txt"
echo "--- git status (should be clean; .p0ref ignored)"
git -C "$REVIEW_WT" status --short | head
echo "--- provenance"
cat provenance.txt
