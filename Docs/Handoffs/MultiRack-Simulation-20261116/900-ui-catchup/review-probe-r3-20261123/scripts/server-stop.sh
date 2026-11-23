#!/bin/bash
# server-stop.sh <tag> : stop ONLY the PID this review started for <tag> (verified by its command line).
source /mnt/c/Users/hp/AppData/Local/Temp/review-r3/env.sh
TAG="$1"
PID="$(cat "$RUN/pid-$TAG.txt" 2>/dev/null)"
[ -z "$PID" ] && { echo "no pid file for $TAG"; exit 0; }
if kill -0 "$PID" 2>/dev/null; then
  CMD="$(tr '\0' ' ' < /proc/$PID/cmdline)"
  case "$CMD" in
    *sitkap1-ui.dll*"fixture-$TAG"*)
      kill -INT "$PID"
      for _ in $(seq 1 30); do kill -0 "$PID" 2>/dev/null || break; sleep 0.1; done
      if kill -0 "$PID" 2>/dev/null; then echo "SIGINT ignored (background job); sending SIGTERM to this review's own pid $PID"; kill -TERM "$PID"; fi
      for _ in $(seq 1 50); do kill -0 "$PID" 2>/dev/null || break; sleep 0.1; done
      ;;
    *) echo "pid $PID is not this review's server ($CMD) - NOT touching it"; exit 1;;
  esac
  kill -0 "$PID" 2>/dev/null && echo "STILL RUNNING pid $PID" || echo "server $TAG ($PID) stopped"
else
  echo "server $TAG ($PID) already gone"
fi
echo "--- server stderr bytes: $(wc -c < "$TMPDIR/server-$TAG.err")"
rm -f "$RUN/url-$TAG.txt"
