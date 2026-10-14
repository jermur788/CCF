#!/usr/bin/env bash
# Oak/Beech calibration qualification driver. One Editor at a time. Run INSIDE the demo-HDR quarantine wrapper:
#   python3 Tools/Build/S1B/with_demo_hdr_quarantined.py bash Tools/Verification/OakBeech/run_growth.sh [steps...]
# Steps (default): compile rows release cohort save existing regression
set -u
ROOT=$(cd "$(dirname "$0")/../../.." && pwd); cd "$ROOT"
OUT=$ROOT/Build/OBQ; DATA=$OUT/data; SUM=$OUT/summary.txt; mkdir -p "$OUT" "$DATA"
Q=Tools/Verification/Qualification/run_qualification.py
G=Tools/Verification/OakBeech/OakBeechQualification.cs
step() { local label="$1"; shift; echo "$(date -Is) START $label" >> "$SUM"; "$@" > "$OUT/$label.stdout.txt" 2>&1; local rc=$?; echo "$(date -Is) END $label exit=$rc" >> "$SUM"; return $rc; }
phase() { # label phase [runner args...]
  local label="$1" ph="$2"; shift 2
  python3 $Q --label "$label" --out "$OUT/run" --method OakBeechQualification.Begin --file $G --marker OBQ_VERIFY_PASS \
    --env OBQ_PHASE="$ph" --env OBQ_OUT="$DATA" "$@" --timeout 7200
}
existing() { # label method file marker [runner args...]
  local label="$1" method="$2" file="$3" marker="$4"; shift 4
  python3 $Q --label "$label" --out "$OUT/run" --method "$method" --file "$file" --marker "$marker" "$@" --timeout 3600
}
STEPS=("$@"); [ ${#STEPS[@]} -eq 0 ] && STEPS=(compile rows release cohort save existing regression)
echo "$(date -Is) BEGIN head=$(git rev-parse HEAD) steps=${STEPS[*]}" >> "$SUM"
for s in "${STEPS[@]}"; do
  case $s in
    compile)
      step compile bash -c "mkdir -p $OUT/run/config/unity3d/Unity; [ -d $OUT/run/config/unity3d/Unity/licenses ] || cp -r \$HOME/.config/unity3d/Unity/licenses $OUT/run/config/unity3d/Unity/; XDG_CONFIG_HOME=$OUT/run/config /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -nographics -quit -projectPath $ROOT -job-worker-count 2 -logFile $OUT/compile.log" ;;
    rows) step rows phase rows rows ;;
    release) step release phase release release ;;
    cohort) step cohort phase cohort cohort
            step cohort-m2 phase cohort-m2 cohort --env OBQ_COHORT_MODEL=2 --env OBQ_TAG=-m2 ;;
    save) step save1 phase save1 save1
          H20=$(grep -h "candidate_Y20_hash\|cohort_candidate" "$DATA/evidence-cohort.txt" 2>/dev/null | sed -n 's/.*Y20=\([0-9A-F]*\).*/\1/p' | head -1)
          H30=$(grep -h "candidate_Y30_hash" "$DATA/evidence-cohort.txt" 2>/dev/null | sed 's/.*candidate_Y30_hash //' | head -1)
          step save2 phase save2 save2 --env OBQ_EXPECT_Y20_HASH="$H20" --env OBQ_EXPECT_Y30_HASH="$H30" ;;
    existing)
      step broadleaf-growth-e80 existing broadleaf-growth-e80 BroadleafAdultGrowthVerification.Begin Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs BROADLEAF_GROWTH_VERIFY_PASS
      step broadleaf-growth-l40 existing broadleaf-growth-l40 BroadleafAdultGrowthVerification.Begin Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs BROADLEAF_GROWTH_VERIFY_PASS --env CCF_STAND_GEOMETRY=0
      step broadleaf-timber existing broadleaf-timber BroadleafTimberEconomyVerification.Begin Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs BROADLEAF_TIMBER_VERIFY_PASS --env CCF_STAND_GEOMETRY=0
      step mixed-density-l40 existing mixed-density-l40 MixedDensityVerification.Begin Tools/Verification/ThreeSpecies/MixedDensityVerification.cs MIXED_DENSITY_VERIFY_PASS --env CCF_STAND_GEOMETRY=0
      step mixed-density-e80 existing mixed-density-e80 MixedDensityVerification.Begin Tools/Verification/ThreeSpecies/MixedDensityVerification.cs MIXED_DENSITY_VERIFY_PASS ;;
    regression)
      mkdir -p Build/UnderstoreyRecruitment/config/unity3d/Unity
      [ -d Build/UnderstoreyRecruitment/config/unity3d/Unity/licenses ] || cp -r "$HOME/.config/unity3d/Unity/licenses" Build/UnderstoreyRecruitment/config/unity3d/Unity/
      step regression-24 python3 Tools/Verification/WindthrowV1/run_regression.py ;;
  esac
done
echo "$(date -Is) OBQ-DONE" >> "$SUM"
