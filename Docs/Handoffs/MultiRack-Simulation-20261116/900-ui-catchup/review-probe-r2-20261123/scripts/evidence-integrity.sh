#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
cd "$REVIEW_WT" || exit 1
D=Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup
BASE=e7c1a57da519ed1ee03ccfbad240196a66df0905
echo "=== E1 only-allowed-paths: BASE..HEAD name-only, grouped"
git diff --name-only $BASE HEAD | sed -E 's#^(Tools/[^/]+/[^/]+/[^/]+).*#\1#; s#^(Docs/Handoffs/[^/]+/[^/]+/[^/]+).*#\1#' | sort | uniq -c
echo "--- touches of forbidden paths (must be empty):"
git diff --name-only $BASE HEAD | grep -E '^(Assets/|Packages/|ProjectSettings/|Tools/MultiRackSitkaP1Lab/src/Core/|Tools/MultiRackSitkaP1Lab/src/Ui/(PlayerApi|Program)\.cs|Tools/MultiRackSitkaP1Lab/fixtures/|Tools/MultiRackSitkaP1Lab/.*\.sln|specs/)' || echo "(none)"
echo "=== E2 C# outside the MultiAreaPage raw strings: hunks in the product file by position"
git diff -U0 $BASE HEAD -- Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | grep -E '^@@' | awk '{print $3}' | tr -d '+' | awk -F, '{print $1}' | sort -n | sed -n '1p;$p' | sed 's/^/first/;2s/first/last /'
echo "MultiAreaPage class starts at line: $(grep -n 'internal static class MultiAreaPage' Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | cut -d: -f1); Js raw string ends at: $(grep -n '^""";' Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | tail -1 | cut -d: -f1); file lines: $(wc -l < Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs)"
echo "--- is the C# above the class byte-identical to BASE?"
diff <(git show $BASE:Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | sed -n '1,227p') <(sed -n '1,227p' Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs) > /dev/null && echo "lines 1-227 identical: YES" || echo "lines 1-227 identical: NO"
echo "--- anything after the last raw string?"
git show $BASE:Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | sed -n '/^""";$/,$p' | tail -n +1 | md5sum
sed -n '/^""";$/,$p' Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | md5sum
echo "=== E3 commit identity and trailers"
for c in 267be9ff 0e17f641 9fed4eb9 9dd6f601 1a22d01c f0db8323 550e10b4 5f3b5bf3; do
  echo "$c | author: $(git log -1 --format='%an <%ae>' $c) | committer: $(git log -1 --format='%cn' $c) | trailers: $(git log -1 --format='%(trailers:key=Co-Authored-By,valueonly,separator=;)' $c | tr '\n' ' ')"
done
echo "=== E4 committed PNG evidence: count, sizes, sha256 prefix"
ls -l $D/evidence-owner-ux/*.png | awk '{print $5, $9}' | sed "s#$D/evidence-owner-ux/##"
echo "total bytes: $(cat $D/evidence-owner-ux/*.png | wc -c) ; files: $(ls $D/evidence-owner-ux/*.png | wc -l) ; largest: $(ls -l $D/evidence-owner-ux/*.png | awk '{print $5}' | sort -n | tail -1)"
echo "caps (AC-9): per image <= $((900*1000)) or $((900*1024)); set <= $((5*1000*1000)) or $((5*1024*1024))"
sha256sum $D/evidence-owner-ux/07-ground-inspection-1280x720.png $D/evidence-owner-ux/07-ground-inspection-1920x1080.png $D/evidence-owner-ux/10-ground-mode-crown-selects-1280x720.png $D/evidence-owner-ux/01-initial-map-1280x720.png | cut -c1-12,65-
echo "=== E5 capability URL leakage in ALL committed files of the candidate (must be zero)"
git grep -nEI '/s/[0-9a-f]{64}' HEAD -- . ':!*.png' | head -5 || true
git grep -cEI '[0-9a-f]{64}' HEAD -- $D/browser-driver-owner-ux $D/MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md | head
echo "(64-hex strings in the handoff/driver files above are expected only for known SHAs/hashes; listing them:)"
git grep -hoEI '[0-9a-f]{64}' HEAD -- $D/browser-driver-owner-ux $D/MULTI-STAGE4-OWNER-UX-REPAIR-PHASE-A-IMPLEMENTATION-RESULT-20261123.md | sort -u | head
echo "=== E6 driver header comment claim: diff of the committed scenario vs its history"
git log --oneline --follow -- $D/browser-driver-owner-ux/scenario-owner-ux.mjs | cat
echo "=== E7 results JSON headline numbers"
for v in 1280x720 1920x1080; do
  python3 - "$D/browser-driver-owner-ux/browser-results-$v.json" <<'PY'
import json,sys
d=json.load(open(sys.argv[1]))
r=d['results']; chk=[x for x in r if not x.startswith('INFO ')]
print(sys.argv[1].split('/')[-1], 'failures=',d.get('failures'), 'checks=',len(chk), 'PASS=',sum(1 for x in chk if x.startswith('PASS')), 'FAIL=',sum(1 for x in chk if x.startswith('FAIL')), 'aborted=',d.get('aborted'))
PY
done
echo "=== E8 test script diff: any removed/weakened existing check lines?"
git diff $BASE HEAD -- Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh | grep -E '^-[^-]' | head -20
echo "(removed-line count: $(git diff $BASE HEAD -- Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh | grep -cE '^-[^-]'))"
