#!/bin/bash
set -e
cd /home/jer/Development/CCF-HP
SHA=5f3b5bf3c9f711125fb61ddee55de1ff9690c19b
WT=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2Review-20261123
BR=review/multi-stage4-owner-ux-r2-20261123
if [ -e "$WT" ]; then echo "WORKTREE PATH ALREADY EXISTS: $WT"; exit 3; fi
git worktree add -B "$BR" "$WT" "$SHA"
cd "$WT"
echo "--- HEAD check"
git rev-parse HEAD
test "$(git rev-parse HEAD)" = "$SHA" && echo "HEAD == expected SHA: OK" || { echo "HEAD MISMATCH"; exit 4; }
git status --short | head
git branch --show-current
mkdir -p /home/jer/.cache/review-r2-tmp
echo "TMPDIR dir ready: /home/jer/.cache/review-r2-tmp"
