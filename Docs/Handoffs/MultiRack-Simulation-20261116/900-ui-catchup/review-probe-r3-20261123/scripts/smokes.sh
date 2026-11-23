#!/bin/bash
# smokes.sh : HEAD's two smoke witnesses (their own safety checks included), then the baseline/negative-control worktrees.
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
NODE_BIN=$(bash -lc 'command -v node')
echo "NODE=$NODE_BIN $($NODE_BIN --version)"
cd "$LAB" || exit 1
echo "git status before: [$(git status --porcelain --untracked-files=all -- . | wc -l) entries]"
echo "=== B1 ui-smoke-multiarea.sh (HEAD)"
DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke-multiarea.sh "$EVID/smoke-multiarea-evidence" > "$EVID/B1-ui-smoke-multiarea.log" 2>&1
echo "multiarea exit=$?"
grep -c '^PASS' "$EVID/B1-ui-smoke-multiarea.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/B1-ui-smoke-multiarea.log" | cut -c1-110 | sed 's/^/  /'
echo "--- R3a..R3j lines"
grep -E '^(PASS|FAIL) R3[a-j]' "$EVID/B1-ui-smoke-multiarea.log" | cut -c1-150
echo "=== B2 ui-smoke.sh (HEAD, single-area)"
DOTNET="$DOTNET" NODE="$NODE_BIN" bash tests/ui-smoke.sh "$EVID/smoke-single-evidence" > "$EVID/B2-ui-smoke.log" 2>&1
echo "single exit=$?"
grep -c '^PASS' "$EVID/B2-ui-smoke.log" | sed 's/^/PASS count: /'
grep '^FAIL' "$EVID/B2-ui-smoke.log" | cut -c1-110 | sed 's/^/  /'
echo "git status after: [$(git status --porcelain --untracked-files=all -- . | wc -l) entries]"
echo "=== DONE smokes"
