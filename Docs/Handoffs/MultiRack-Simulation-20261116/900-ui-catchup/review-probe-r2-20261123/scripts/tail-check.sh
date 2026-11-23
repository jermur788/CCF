#!/bin/bash
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
cd "$REVIEW_WT" || exit 1
BASE=e7c1a57da519ed1ee03ccfbad240196a66df0905
F=Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs
bl=$(git show $BASE:$F | grep -n '^""";$' | tail -1 | cut -d: -f1)
hl=$(grep -n '^""";$' $F | tail -1 | cut -d: -f1)
echo "BASE last raw-string terminator line: $bl of $(git show $BASE:$F | wc -l); HEAD: $hl of $(wc -l < $F)"
echo "BASE tail after last terminator:"; git show $BASE:$F | tail -n +$((bl+1)) | md5sum
echo "HEAD tail after last terminator:"; tail -n +$((hl+1)) $F | md5sum
echo "--- the three raw-string openers' preceding C# (class header lines) identical?"
diff <(git show $BASE:$F | sed -n '228,231p') <(sed -n '228,231p' $F) && echo "class header lines identical"
echo "--- any non-raw-string C# between the raw strings changed? (lines between Html end and Css start; Css end and Js start)"
for n in 1 2; do :; done
hs=$(grep -n 'internal const string Css' $F | cut -d: -f1); he=$(grep -n 'internal const string Js' $F | cut -d: -f1)
bhs=$(git show $BASE:$F | grep -n 'internal const string Css' | cut -d: -f1); bhe=$(git show $BASE:$F | grep -n 'internal const string Js' | cut -d: -f1)
echo "HEAD Css@$hs Js@$he ; BASE Css@$bhs Js@$bhe"
diff <(git show $BASE:$F | sed -n "$((bhs-3)),$((bhs))p") <(sed -n "$((hs-3)),$((hs))p" $F) && echo "C# before Css identical"
diff <(git show $BASE:$F | sed -n "$((bhe-3)),$((bhe))p") <(sed -n "$((he-3)),$((he))p" $F) && echo "C# before Js identical"
echo "--- progress of the main browser run:"
tail -c 600 /mnt/c/Users/hp/AppData/Local/Temp/review-r2/out/1280x720/run.log | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g' | cut -c1-240
