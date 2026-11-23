#!/bin/bash
WT=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R3ReReview-20261123
cd "$WT" || exit 1
D=Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup
BASE=e7c1a57da519ed1ee03ccfbad240196a66df0905
PREV=5f3b5bf3c9f711125fb61ddee55de1ff9690c19b
F=Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs
echo "HEAD=$(git rev-parse HEAD)  branch=$(git branch --show-current)  status-entries=$(git status --porcelain | wc -l)"
echo "=== E1 forbidden paths touched BASE..HEAD (must be none)"
git diff --name-only $BASE HEAD | grep -E '^(Assets/|Packages/|ProjectSettings/|Tools/MultiRackSitkaP1Lab/src/Core/|Tools/MultiRackSitkaP1Lab/src/Ui/(PlayerApi|Program)\.cs|Tools/MultiRackSitkaP1Lab/fixtures/|.*\.sln$|specs/|.*\.csproj$)' || echo "(none)"
echo "--- files changed PREV..HEAD:"; git diff --name-only $PREV HEAD | sed -E 's#^(Docs/Handoffs/[^/]+/[^/]+/[^/]+/[^/]+).*#\1#' | sort | uniq -c
echo "--- non-doc, non-evidence files changed PREV..HEAD:"; git diff --name-only $PREV HEAD | grep -vE '^Docs/'
echo "=== E2 C# outside the MultiAreaPage raw strings"
echo "lines 1-227 identical to BASE?"
diff <(git show $BASE:$F | sed -n '1,227p') <(sed -n '1,227p' $F) >/dev/null && echo "  YES" || echo "  NO"
bl=$(git show $BASE:$F | grep -n '^""";$' | tail -1 | cut -d: -f1); hl=$(grep -n '^""";$' $F | tail -1 | cut -d: -f1)
echo "tail after the last raw-string terminator md5 (BASE / HEAD):"
git show $BASE:$F | tail -n +$((bl+1)) | md5sum | cut -c1-32; tail -n +$((hl+1)) $F | md5sum | cut -c1-32
hs=$(grep -n 'internal const string Css' $F | cut -d: -f1); he=$(grep -n 'internal const string Js' $F | cut -d: -f1)
bhs=$(git show $BASE:$F | grep -n 'internal const string Css' | cut -d: -f1); bhe=$(git show $BASE:$F | grep -n 'internal const string Js' | cut -d: -f1)
diff <(git show $BASE:$F | sed -n "$((bhs-3)),$((bhs))p") <(sed -n "$((hs-3)),$((hs))p" $F) >/dev/null && echo "C# before Css identical: YES" || echo "C# before Css identical: NO"
diff <(git show $BASE:$F | sed -n "$((bhe-3)),$((bhe))p") <(sed -n "$((he-3)),$((he))p" $F) >/dev/null && echo "C# before Js identical: YES" || echo "C# before Js identical: NO"
echo "class header 228-231 identical:"; diff <(git show $BASE:$F | sed -n '228,231p') <(sed -n '228,231p' $F) >/dev/null && echo "  YES" || echo "  NO"
echo "=== E3 hunks of the page diff PREV..HEAD: any hunk starting outside the three raw strings?"
git diff -U0 $PREV HEAD -- $F | grep -E '^@@' | awk '{print $3}' | tr -d '+' | awk -F, '{print $1}' | sort -n | sed -n '1p;$p' | sed 's/^/hunk start /'
echo "class MultiAreaPage at line $(grep -n 'internal static class MultiAreaPage' $F | cut -d: -f1); last raw string ends at $hl; file lines $(wc -l < $F)"
echo "=== E4 smoke script: removed lines BASE..HEAD and PREV..HEAD (expect 0)"
echo "BASE..HEAD: $(git diff $BASE HEAD -- Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh | grep -cE '^-[^-]')   PREV..HEAD: $(git diff $PREV HEAD -- Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh | grep -cE '^-[^-]')"
echo "added lines PREV..HEAD: $(git diff $PREV HEAD -- Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh | grep -cE '^\+[^+]')"
echo "=== E5 authorship and trailers of every commit BASE..HEAD"
for c in $(git rev-list --reverse $BASE..HEAD); do
  printf '%s | A: %s | C: %s | trailers: %s\n' "$(git log -1 --format='%h' $c)" "$(git log -1 --format='%an <%ae>' $c)" "$(git log -1 --format='%cn' $c)" "$(git log -1 --format='%(trailers:key=Co-Authored-By,valueonly,separator=;)' $c | tr '\n' ' ')"
done
echo "commit count BASE..HEAD: $(git rev-list --count $BASE..HEAD); merges: $(git rev-list --merges --count $BASE..HEAD)"
echo "=== E6 handoff header row 8 (authorship)"
sed -n '8p' $D/MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md | cut -c1-600
echo "=== E7 committed PNG evidence"
ls -l $D/evidence-owner-ux/*.png | awk '{print $5, $9}' | sed "s#$D/evidence-owner-ux/##"
echo "files: $(ls $D/evidence-owner-ux/*.png | wc -l); total bytes: $(cat $D/evidence-owner-ux/*.png | wc -c); largest: $(ls -l $D/evidence-owner-ux/*.png | awk '{print $5}' | sort -n | tail -1)"
echo "caps (AC-9): per image <= $((900*1000)) / $((900*1024)); set <= $((5*1000*1000)) / $((5*1024*1024))"
for f in 02-selected-five-ranks-1280x720 03-ranks-chosen-synced-1280x720 07-ground-inspection-1280x720 07-ground-inspection-1920x1080 10-ground-mode-crown-selects-1280x720 11-blocked-rows-1280x720 12-after-add-card-1280x720; do
  printf '%s  ' "$(sha256sum $D/evidence-owner-ux/$f.png | cut -c1-8)"; echo $f; done
echo "=== E8 capability URL leakage in committed non-PNG files at HEAD (must be zero)"
git grep -nEI '/s/[0-9a-f]{64}' HEAD -- . ':!*.png' | head -5; echo "(end; matches above if any)"
echo "64-hex strings in the lane's driver and handoff files (expect only SHAs/anchors):"
git grep -hoEI '[0-9a-f]{64}' HEAD -- $D/browser-driver-owner-ux $D/MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md | sort -u | head -12
echo "=== E9 writer's results JSON headline"
for v in 1280x720 1920x1080; do python3 - "$D/browser-driver-owner-ux/browser-results-$v.json" <<'PY'
import json,sys
d=json.load(open(sys.argv[1])); r=d['results']; chk=[x for x in r if not x.startswith('INFO ')]
print(sys.argv[1].split('/')[-1],'failures=',d.get('failures'),'checks=',len(chk),'PASS=',sum(1 for x in chk if x.startswith('PASS')),'FAIL=',sum(1 for x in chk if x.startswith('FAIL')),'aborted=',d.get('aborted'))
PY
done
for v in 1280x720 1920x1080; do python3 - "$D/browser-driver-owner-ux/fold-sweep-$v.json" <<'PY'
import json,sys
d=json.load(open(sys.argv[1]))
print(sys.argv[1].split('/')[-1], list(d.keys())[:8])
PY
done
