#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
R2=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2Review-Docs-20261123/Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/review-probe-r2-20261123/results
echo "=== multi-area: FAIL lines of this run (HEAD 0e777455) vs my R2 BASE run (e7c1a57d) vs my R2 HEAD run (5f3b5bf3)"
for f in "$EVID/B1-ui-smoke-multiarea.log" "$R2/C1-base-ui-smoke-multiarea.log" "$R2/B1-ui-smoke-multiarea.log"; do printf '%s : ' "$(echo $f | sed 's#.*/##')"; grep '^FAIL' "$f" | sed -E 's#/s/[0-9a-f]{64}/#/s/<c>/#g' | md5sum | cut -c1-12; done
diff <(grep '^FAIL' "$EVID/B1-ui-smoke-multiarea.log" | sed -E 's#/s/[0-9a-f]{64}/#/s/<c>/#g') <(grep '^FAIL' "$R2/C1-base-ui-smoke-multiarea.log" | sed -E 's#/s/[0-9a-f]{64}/#/s/<c>/#g') > /dev/null && echo "FAIL lines identical to BASE: YES" || { echo "FAIL lines identical to BASE: NO"; diff <(grep '^FAIL' "$EVID/B1-ui-smoke-multiarea.log" | cut -c1-120) <(grep '^FAIL' "$R2/C1-base-ui-smoke-multiarea.log" | cut -c1-120) | head; }
echo "=== single-area"
diff <(grep '^FAIL' "$EVID/B2-ui-smoke.log" | sed -E 's#/s/[0-9a-f]{64}/#/s/<c>/#g') <(grep '^FAIL' "$R2/C2-base-ui-smoke.log" | sed -E 's#/s/[0-9a-f]{64}/#/s/<c>/#g') > /dev/null && echo "FAIL lines identical to BASE: YES" || echo "FAIL lines identical to BASE: NO"
echo "=== which witness check IDs exist at HEAD but not at BASE (should be exactly R2a-R2j, R3a-R3j, F2a-F2h)"
diff <(grep -oE '^(PASS|FAIL) [A-Z][0-9A-Za-z]+' "$R2/C1-base-ui-smoke-multiarea.log" | awk '{print $2}' | sort -u) <(grep -oE '^(PASS|FAIL) [A-Z][0-9A-Za-z]+' "$EVID/B1-ui-smoke-multiarea.log" | awk '{print $2}' | sort -u) | grep '^[<>]' | tr '\n' ' '; echo
echo "=== did the smoke runs leave anything in the review worktree?"
git -C "$REVIEW_WT" status --porcelain --untracked-files=all | wc -l
echo "=== P0 pin now"
( cd "$LAB/.p0ref" && sha256sum SitkaLab.Core.dll | cut -c1-64; sha256sum -c SitkaLab.Core.dll.sha256 2>&1 | head -1 )
cat "$EVID/p0-before.txt"
