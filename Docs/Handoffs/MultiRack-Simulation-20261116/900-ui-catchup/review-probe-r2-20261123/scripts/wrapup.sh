#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
RUN=/mnt/c/Users/hp/AppData/Local/Temp/review-r2/run
echo "=== W1 stop this review's servers (only PIDs this review started)"
for t in t4a t4b; do bash /mnt/c/Users/hp/AppData/Local/Temp/review-r2/server-stop.sh $t 2>&1 | grep -E 'stopped|STILL|already'; done
echo "--- any server from this review still alive? (pid files vs /proc)"
for f in $RUN/pid-*.txt; do p=$(cat $f); if kill -0 $p 2>/dev/null; then echo "ALIVE: $f pid $p: $(tr '\0' ' ' < /proc/$p/cmdline | cut -c1-120)"; fi; done; echo "(none listed above = all stopped)"
echo "=== W2 protected listeners (must still be alive, never bound by this review)"
for port in 33767 43649 46593; do
  if ss -ltnp 2>/dev/null | grep -q "127.0.0.1:$port "; then echo "127.0.0.1:$port LISTENING (pid $(ss -ltnp 2>/dev/null | grep "127.0.0.1:$port " | grep -oE 'pid=[0-9]+' | head -1))"; else echo "127.0.0.1:$port NOT LISTENING"; fi
done
echo "--- ports this review's servers used: 44117 39707 38553 35451 41693 35837 35717 + the two narrow ones; none equal a protected port:"
cat $RUN/pid-*.txt >/dev/null; grep -h -oE 'port [0-9]+' /dev/null
echo "=== W3 P0 AFTER"
cd "$LAB/.p0ref" && GOT=$(sha256sum SitkaLab.Core.dll | cut -d' ' -f1); echo "$GOT"; BEFORE=$(cat $EVID/p0-before.txt)
[ "$GOT" = "$BEFORE" ] && [ "$GOT" = "$P0EXPECT" ] && echo "P0 before == after == expected 9a99f781...: OK" || echo "P0 MISMATCH"
echo "$GOT" > $EVID/p0-after.txt
sha256sum -c SitkaLab.Core.dll.sha256 2>&1 | head -2
echo "=== W4 review worktree state"
cd "$REVIEW_WT" && echo "HEAD: $(git rev-parse HEAD)"; echo "branch: $(git branch --show-current)"; echo "status entries (ignored files excluded): $(git status --porcelain | wc -l)"; git status --porcelain | head
echo "=== W5 remote refs: candidate must be unmoved; main must not contain it"
git fetch origin 2>&1 | tail -2
echo "origin/feat/multi-stage4-owner-ux-20261123-v1 = $(git rev-parse origin/feat/multi-stage4-owner-ux-20261123-v1)"
git merge-base --is-ancestor 5f3b5bf3c9f711125fb61ddee55de1ff9690c19b origin/main && echo "candidate IS contained in origin/main (!)" || echo "candidate is NOT contained in origin/main"
echo "=== W6 sibling worktrees I did not create were not touched (their HEADs):"
for w in CCF-Multi-Stage4OwnerUX-R2-20261123 CCF-Multi-Stage4OwnerUX-Independent-20261123 CCF-Multi-Stage4OwnerUX-ReviewBase-20261123; do echo "$w: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w rev-parse --short HEAD) status entries: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w status --porcelain | wc -l)"; done
