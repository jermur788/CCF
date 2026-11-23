#!/bin/bash
# run-probe.sh <tag> <WxH> <sections> [labdir]  (start a fresh server, run the probe with Windows node, stop the server)
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
TAG="$1"; SIZE="$2"; SECTIONS="$3"; LABDIR="${4:-$LAB}"
PROBE="${PROBE:-review-probe-r3b.mjs}"
B=/mnt/c/Users/hp/AppData/Local/Temp/review-r3
OUTW='C:\Users\hp\AppData\Local\Temp\review-r3\out\'"$TAG"
mkdir -p "$B/out/$TAG"
bash $B/server-start.sh "$TAG" "$LABDIR" | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g'
URLW='C:\Users\hp\AppData\Local\Temp\review-r3\run\url-'"$TAG"'.txt'
"/mnt/c/Program Files/nodejs/node.exe" 'C:\Users\hp\AppData\Local\Temp\review-r3\probe\'"$PROBE" "$OUTW" "$URLW" "$SIZE" "$SECTIONS" > "$B/out/$TAG/run.log" 2>&1
echo "probe exit: $? (probe file: $PROBE)"
bash $B/server-stop.sh "$TAG"
grep -E '^(RESULT|ABORT)' "$B/out/$TAG/run.log" | sed -E 's#/s/[0-9a-f]{64}/#/s/<capability>/#g'
echo "FAIL lines: $(grep -c '^FAIL' $B/out/$TAG/run.log)"
