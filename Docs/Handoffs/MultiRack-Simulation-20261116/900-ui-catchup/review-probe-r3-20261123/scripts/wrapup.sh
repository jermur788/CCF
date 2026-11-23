#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
echo "=== W1 this review's servers (pid files vs /proc)"
n=0; for f in $RUN/pid-*.txt; do p=$(cat $f); if kill -0 $p 2>/dev/null; then echo "ALIVE: $f pid $p: $(tr '\0' ' ' < /proc/$p/cmdline | cut -c1-100)"; n=$((n+1)); fi; done; echo "alive: $n (0 = all stopped)"
echo "=== W2 protected listeners (never bound by this review)"
for port in 33767 43649 46593; do ss -ltn 2>/dev/null | grep -q "127.0.0.1:$port " && echo "127.0.0.1:$port LISTENING" || echo "127.0.0.1:$port NOT LISTENING"; done
echo "=== W3 P0 pin after: $(cd $LAB/.p0ref && sha256sum SitkaLab.Core.dll | cut -c1-64)  before: $(cat $EVID/p0-before.txt)  expected: $P0EXPECT"
cd "$LAB/.p0ref" && sha256sum -c SitkaLab.Core.dll.sha256 2>&1 | head -1
cp "$LAB/.p0ref/SitkaLab.Core.dll.sha256" /dev/null
echo "$(cd $LAB/.p0ref && sha256sum SitkaLab.Core.dll | cut -c1-64)" > $EVID/p0-after.txt
echo "=== W4 review worktree"
cd "$REVIEW_WT" && echo "HEAD: $(git rev-parse HEAD)  branch: $(git branch --show-current)  status entries (ignored files excluded): $(git status --porcelain | wc -l)"
echo "=== W5 remote refs"
git fetch origin 2>&1 | tail -2
echo "candidate origin/feat/multi-stage4-owner-ux-20261123-v1 = $(git rev-parse origin/feat/multi-stage4-owner-ux-20261123-v1)"
echo "candidate on remote via ls-remote = $(git ls-remote origin refs/heads/feat/multi-stage4-owner-ux-20261123-v1 | cut -c1-40)"
git merge-base --is-ancestor 0e7774557dbb2947b29993cf032c084aa3b7227f origin/main && echo "candidate IS contained in origin/main (!)" || echo "candidate is NOT contained in origin/main"
git merge-base --is-ancestor 5f3b5bf3c9f711125fb61ddee55de1ff9690c19b 0e7774557dbb2947b29993cf032c084aa3b7227f && echo "PREV is an ancestor of HEAD: yes"
echo "=== W6 sibling worktrees I did not create (HEAD, status entries)"
for w in CCF-Multi-Stage4OwnerUX-R2-20261123 CCF-Multi-Stage4OwnerUX-R3-20261123 CCF-Multi-Stage4OwnerUX-Independent-20261123 CCF-Multi-Stage4OwnerUX-ReviewBase-20261123 CCF-Multi-Stage4OwnerUX-Spec-20261123 CCF-Multi-Stage4UiCatchup-20261123; do echo "$w: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w rev-parse --short HEAD) status: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w status --porcelain | wc -l)"; done
echo "=== W7 worktrees I created"
for w in CCF-Multi-Stage4OwnerUX-R3ReReview-20261123 CCF-Multi-Stage4OwnerUX-R3ReReview-PREV-20261123 CCF-Multi-Stage4OwnerUX-R3ReReview-Docs-20261123 CCF-Multi-Stage4OwnerUX-R2Review-20261123; do echo "$w: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w rev-parse --short HEAD) status: $(git -C /home/jer/Development/CCF-HP-Worktrees/$w status --porcelain | wc -l)"; done
echo "=== W8 main checkout untouched"
git -C /home/jer/Development/CCF-HP status --porcelain | wc -l
git -C /home/jer/Development/CCF-HP rev-parse --short HEAD
powershell.exe -NoProfile -Command "(Get-CimInstance Win32_Process -Filter \"Name='msedge.exe'\" | Where-Object { \$_.CommandLine -match 'review-r3-edge-' } | Measure-Object).Count" 2>/dev/null | tr -d '\r' | sed 's/^/edge processes still using a review-r3 profile: /'
