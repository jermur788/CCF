#!/usr/bin/env bash
# U4 qualification driver. One Editor at a time. Run INSIDE the demo-HDR quarantine wrapper:
#   python3 Tools/Build/S1B/with_demo_hdr_quarantined.py bash Tools/Verification/U4/run_u4.sh [steps...]
# Steps (default: all non-UI steps): compile core saveA saveB legacy existing ui
set -u
ROOT=$(cd "$(dirname "$0")/../../.." && pwd); cd "$ROOT"
OUT=$ROOT/Build/U4; DATA=$OUT/data; SUM=$OUT/summary.txt; mkdir -p "$OUT" "$DATA"
Q=Tools/Verification/Qualification/run_qualification.py
G=Tools/Verification/U4/U4Qualification.cs
step() { local label="$1"; shift; echo "$(date -Is) START $label" >> "$SUM"; "$@" > "$OUT/$label.stdout.txt" 2>&1; local rc=$?; echo "$(date -Is) END $label exit=$rc" >> "$SUM"; return $rc; }
phase() { # label phase [runner args...]
  local label="$1" ph="$2"; shift 2
  python3 $Q --label "$label" --out "$OUT/run" --method U4Qualification.Begin --file $G --marker U4_VERIFY_PASS \
    --env U4_PHASE="$ph" --env U4_OUT="$DATA" "$@" --timeout 3600
}
existing() { # label method file marker [runner args...]
  local label="$1" method="$2" file="$3" marker="$4"; shift 4
  python3 $Q --label "$label" --out "$OUT/run" --method "$method" --file "$file" --marker "$marker" "$@" --timeout 3600
}
STEPS=("$@"); [ ${#STEPS[@]} -eq 0 ] && STEPS=(compile core saveA saveB legacy existing)
echo "$(date -Is) BEGIN head=$(git rev-parse HEAD) steps=${STEPS[*]}" >> "$SUM"
for s in "${STEPS[@]}"; do
  case $s in
    compile) # import + compile only (no gate), proves a fresh Library builds with 0 errors
      step compile bash -c "mkdir -p $OUT/run/config/unity3d/Unity; [ -d $OUT/run/config/unity3d/Unity/licenses ] || cp -r \$HOME/.config/unity3d/Unity/licenses $OUT/run/config/unity3d/Unity/; XDG_CONFIG_HOME=$OUT/run/config /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -nographics -quit -projectPath $ROOT -job-worker-count 2 -logFile $OUT/compile.log" ;;
    core) step core phase core core ;;
    saveA) step saveA1 phase saveA1 saveA1
           H=$(grep -h "V1_world_hash_Y25" "$DATA/evidence-core.txt" 2>/dev/null | sed 's/.*V1_world_hash_Y25 //' | head -1)
           step saveA2 phase saveA2 saveA2
           step saveA3 phase saveA3 saveA3 --env U4_EXPECT_HASH="$H" ;;
    saveB) step saveB1 phase saveB1 saveB1
           H=$(grep -h "V1_world_hash_Y25" "$DATA/evidence-core.txt" 2>/dev/null | sed 's/.*V1_world_hash_Y25 //' | head -1)
           step saveB2 phase saveB2 saveB2 --env U4_EXPECT_HASH="$H" ;;
    legacy) step legacyMake phase legacyMake legacyMake
            step legacyC phase legacyC legacyC
            step legacyD phase legacyD legacyD
            step legacyE phase legacyE legacyE ;;
    existing)
      step n3-shipped existing n3-shipped N3CompletionVerification.Begin Tools/Verification/ScenarioOneCompletion/N3/N3CompletionVerification.cs N3_U4_VERIFY_PASS --env CCF_STAND_GEOMETRY=0
      step history-gate existing history-gate ScenarioHistoryVerification.Begin Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs SCENARIO_HISTORY_VERIFY_PASS --env CCF_STAND_GEOMETRY=0
      step historical-completion existing historical-completion ScenarioOneCompletionVerification.Begin Tools/Verification/ScenarioOneCompletionVerification.cs SCENARIO_ONE_COMPLETION_VERIFY_PASS --env CCF_STAND_GEOMETRY=0 ;;
    ui) step ui phase ui ui --interactive --window 1280x800 ;;
  esac
done
echo "$(date -Is) U4-DONE" >> "$SUM"
