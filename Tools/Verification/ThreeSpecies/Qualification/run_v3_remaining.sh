#!/usr/bin/env bash
# Density v3: the Unity runs that were NOT completed in the first qualification (see Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV3Handoff.md,
# "Unity status"): the live Sitka-component identity, the two nearest grid neighbours of the chosen setting, a clean-exit rerun of the fast
# Enlarged80 qualification, the 24-gate default regression with the Phase-0 anchor carriers, and three more Legacy40 seeds.
#
# Run it INSIDE the demo-HDR wrapper, on a machine with no other heavy Unity session:
#     python3 Tools/Build/S1B/with_demo_hdr_quarantined.py bash Tools/Verification/ThreeSpecies/Qualification/run_v3_remaining.sh
# One Editor at a time; a step starts only with 4.5 GB of free memory and 3 GB of free swap. The neighbour runs edit two constants in
# MixedDensityModel.Composite.cs and restore them with git checkout, so the tree must be clean when you start. Output: Build/V3r, Build/V3x.
set -u
ROOT=$(git rev-parse --show-toplevel)
cd $ROOT
OUT=$ROOT/Build/V3r; OUTX=$ROOT/Build/V3x; SUM=$OUT/summary.txt
mkdir -p $OUT $OUTX Build/UnderstoreyRecruitment/config/unity3d/Unity
cp -r $HOME/.config/unity3d/Unity/licenses Build/UnderstoreyRecruitment/config/unity3d/Unity/ 2>/dev/null || true
if ! pgrep -f "Xephyr :78" >/dev/null; then
  (Xephyr :78 -screen 1600x900 -ac -noreset >/dev/null 2>&1 &) ; sleep 3
  (DISPLAY=:78 openbox >/dev/null 2>&1 &) ; sleep 2
fi
export DISPLAY=${DISPLAY:-:78}
free_editor() { until [ "$(awk '/MemAvailable/ {print int($2/1024)}' /proc/meminfo)" -gt 4500 ] && [ "$(awk '/SwapFree/ {print int($2/1024)}' /proc/meminfo)" -gt 3000 ]; do sleep 20; done; }
# step LABEL OUTDIR CMD...: waits for a free machine, retries when the runner says another Editor started in the meantime
step() { local label="$1" dir="$2"; shift 2
  for attempt in $(seq 1 40); do
    free_editor
    echo "$(date -Is) START $label attempt $attempt" >> $SUM
    "$@" > $dir/$label.stdout.txt 2>&1; code=$?
    if grep -qi "another Unity Editor is running" $dir/$label.stdout.txt; then echo "$(date -Is) $label blocked by another Editor" >> $SUM; sleep 60; continue; fi
    echo "$(date -Is) END $label exit=$code" >> $SUM; return
  done
  echo "$(date -Is) GAVE UP $label" >> $SUM
}
echo "$(date -Is) BEGIN head=$(git rev-parse HEAD)" >> $SUM
Q=Tools/Verification/ThreeSpecies/Qualification/run_qualification.py
H1=Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs
H2=Tools/Verification/ThreeSpecies/Qualification/DensityProbe.cs
harness() { local label="$1" geo="$2" phases="$3"; shift 3
  python3 $Q --label $label --out $OUTX/$label --method DensityQualification.Begin --file $H1 --file $H2 --marker DENSITY_QUAL_PASS \
    --env CCF_STAND_GEOMETRY=$geo --env CCF_QUAL_PHASES=$phases "$@" --timeout 5400; }
PURE=A,B,E,F
for geo in "0 l40" "1 e80"; do set -- $geo; step sitkacomp-$2 $OUTX harness sitkacomp-$2 $1 SITKACOMP; done
C=Assets/ForestPrototype/MixedDensityModel.Composite.cs
for cand in "0.35 1.60 n1" "0.50 0.80 n2"; do
  set -- $cand; lmax=$1; k=$2; tag=$3
  sed -i "s/public const double GenericResponseStrength = [0-9.]*;/public const double GenericResponseStrength = $k;/; s/public const double GenericMaximumAnnualLoss = [0-9.]*;/public const double GenericMaximumAnnualLoss = $lmax;/" $C
  grep -n "GenericResponseStrength =\|GenericMaximumAnnualLoss =" $C >> $SUM
  step l40-$tag $OUTX harness l40-$tag 0 LONGRUN --env CCF_QUAL_YEARS=300 --env CCF_QUAL_SCENARIOS=$PURE,C,G --env CCF_QUAL_REPLAY=0
  step e80-$tag $OUTX harness e80-$tag 1 LONGRUN --env CCF_QUAL_YEARS=300 --env CCF_QUAL_SCENARIOS=$PURE,C,G --env CCF_QUAL_REPLAY=0
  git checkout -- $C
done
git checkout -- $C
grep -n "GenericResponseStrength =\|GenericMaximumAnnualLoss =" $C >> $SUM
echo "$(date -Is) V3-NEIGHBOURS-DONE" >> $SUM
FAST=RATIOS,ROLLS,ANCHORS,SITKA100,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF
step fast-e80-rerun $OUTX harness fast-e80-rerun 1 $FAST
step regression          $OUT python3 Tools/Verification/WindthrowV1/run_regression.py
cp -r Build/WindthrowV1/Regression $OUT/regression-results 2>/dev/null
step anchor-model2       $OUT python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Verification
step anchor-storm-core   $OUT python3 Tools/Verification/WindthrowV1/run_storms.py --gate StormCoreVerification
step anchor-storm-replay $OUT python3 Tools/Verification/WindthrowV1/run_storms.py --gate StormReplayVerification
step anchor-geometry     $OUT python3 Tools/Verification/StandGeometry/run_stand_geometry.py v3-geometry
step anchor-e80          $OUT python3 Tools/Verification/StandGeometry/run_enlarged80.py v3-e80
step anchor-p2           $OUT python3 Tools/Verification/ScenarioOneCompletion/P2/run_p2_gates.py
cp -r Build/P2 $OUT/P2-results 2>/dev/null
step anchor-p3           $OUT python3 Tools/Verification/ScenarioOneCompletion/P3/run_p3_gates.py
cp -r Build/P3 $OUT/P3-results 2>/dev/null
echo "$(date -Is) V3-REG-DONE" >> $SUM
for seed in 102 103 104; do
  step l40-seed$seed $OUTX harness l40-seed$seed 0 LONGRUN --env CCF_QUAL_YEARS=300 --env CCF_QUAL_SCENARIOS=$PURE,C,G --env CCF_QUAL_REPLAY=0 --env CCF_QUAL_SEED=$seed
done
echo "$(date -Is) V3-ALL-DONE" >> $SUM
