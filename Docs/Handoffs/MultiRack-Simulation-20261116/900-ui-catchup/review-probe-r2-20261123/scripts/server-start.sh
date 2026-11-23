#!/bin/bash
# server-start.sh <tag> : fresh in-memory UI session on an OS-assigned 127.0.0.1 port (never a fixed port).
# Writes the capability base URL ONLY to a file outside the repo; prints a redacted line. Records the PID of this
# script's own process so it can be stopped (and nothing else) by server-stop.sh.
source /mnt/c/Users/hp/AppData/Local/Temp/review-r2/env.sh
TAG="$1"
RUN=/mnt/c/Users/hp/AppData/Local/Temp/review-r2/run
mkdir -p "$RUN" "$TMPDIR/fixture-$TAG"
cp -- "$LAB/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json" "$TMPDIR/fixture-$TAG/"
OUTF="$TMPDIR/server-$TAG.out"; ERRF="$TMPDIR/server-$TAG.err"
: > "$OUTF"; : > "$ERRF"
cd "$LAB" || exit 1
setsid nohup "$DOTNET" src/Ui/bin/Debug/net8.0/sitkap1-ui.dll "$TMPDIR/fixture-$TAG" > "$OUTF" 2> "$ERRF" < /dev/null &
PID=$!
echo "$PID" > "$RUN/pid-$TAG.txt"
URL=""
for _ in $(seq 1 300); do
  URL="$(grep -oE 'http://127\.0\.0\.1:[0-9]+/s/[0-9a-f]{64}/' "$OUTF" | head -1 || true)"
  [ -n "$URL" ] && break
  kill -0 "$PID" 2>/dev/null || break
  sleep 0.1
done
[ -z "$URL" ] && { echo "NO URL; stderr:"; cat "$ERRF"; exit 1; }
printf '%s' "$URL" > "$RUN/url-$TAG.txt"
PORT="${URL#http://127.0.0.1:}"; PORT="${PORT%%/*}"
echo "server pid=$PID port=$PORT (capability written to scratch file only)"
case "$PORT" in 33767|43649|46593) echo "!! PROTECTED PORT BOUND"; exit 9;; esac
echo "port is not one of the protected listeners: OK"
