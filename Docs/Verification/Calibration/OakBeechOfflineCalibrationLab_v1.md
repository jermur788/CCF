# Oak/Beech offline calibration lab v1

Exact base/preregistered parent `60cfd822916325e618dd8c51267db10d5cac2412`. Current relevant broadleaf GrowAdults math also checked byte-for-byte equal to b4894cc and density-v2 a7a759d. **Non-Unity, disposable tooling; no Assets/species/growth/default edits.**

## Result: B — kernel works but remains weakly identifiable

486 grid candidates evaluated before any narrowing. Oak 234/243 survive; Beech243/243 survive. Nine Oak candidates are rejected for proxy D60 before80 under strong release. All failures remain in candidates.json and trajectories.csv. There is no uniquely identified or “correct” parameter set. Beech has no rejected family on this grid; that is weak discrimination, not biological proof. The legacy .4/.4/120/40/3 control also survives hard guards while missing provisional height design bands.

## Reproduction and faithfulness

`python3 Tools/Calibration/OakBeechGrowth/run_lab.py`

Stdlib only; no random draws or clock metadata. Two complete runs produced byte-identical candidates.json, summary.json and trajectories.csv. 87,480 checkpoint rows are repeated controlled trajectory samples, not independent empirical observations. Every input range/treatment/checkpoint and hard guard is fixed in the preregistered parent. Summary includes source-file SHA256 and preregistration SHA256. Source expression guards check diameter competition/size brake, height size brake, storage clamps, promotion relation and species juvenile constants. A 150-step closed-form recurrence comparison passed at <1e-10 for DBH/height, along with promotion timing and release-direction checks. Python double arithmetic reproduces mathematics, **not Unity float bit patterns or anchors**. Real C# / Unity remains a qualification dependency.

Adult increment and storage boundaries faithfully match GrowAdults/ForestTree. Juvenile controlled surviving stock uses the actual .35m/year full-light/site1 law until3.5m promotion, then generic broadleaf growth. It excludes juvenile survival/browsing, origin/spatial placement and feedback on purpose; it is not the full ecological simulator. Adult four initial-state fixtures and release-at50 fixture are separate from lifetime stock. All management heights agree exactly; no artificial doubled height response is present.

## Surviving and rejected families

| Species | Bounded survivor family | Rejected family |
|---|---|---|
| Oak | Whole registered grid except Pd1.2/Dmax160/Ci50=8 combinations; 234 remain across all height choices | Pd1.2,Dmax160,Ci50=8: all 3 Ph × 3 Hmax combinations (9) reach the D60 screening proxy before80 in strong release |
| Beech | All 243 registered combinations | None under hard guards |

Both species include multi-age height-band matches with diagnostic distance0; matching [C] bands alone cannot identify DBH rate, Dmax or Ci50, and does not validate empirical top height. Lifetime model height is an individual control, not the N2 crop-group/top-height statistics. “Plausibility” means bounded controls/design diagnostics, never Irish observed DBH-age fit.

Representative lowest-distance examples (deterministic ordering, not recommendations):

| Species | Candidate | Pd | Ph | Dmax | Hmax | Ci50 | Multi-age [C] height distance |
|---|---|---:|---:|---:|---:|---:|---:|
| Beech | Beech-058 | 0.4 | 1.1 | 80 | 42.5 | 3 | 0 |
| Beech | Beech-059 | 0.4 | 1.1 | 80 | 42.5 | 6.5 | 0 |
| Beech | Beech-060 | 0.4 | 1.1 | 80 | 42.5 | 10 | 0 |
| Oak | Oak-031 | 0.4 | 0.65 | 80 | 42.5 | 2 | 0 |
| Oak | Oak-032 | 0.4 | 0.65 | 80 | 42.5 | 5 | 0 |
| Oak | Oak-033 | 0.4 | 0.65 | 80 | 42.5 | 8 | 0 |

## Sensitivity

One-factor groups marginalise over every other grid dimension (81 combinations/value), including failures. These fractions measure grid survival, not probability or confidence. Height scores depend on Ph/Hmax; DBH release depends on Pd/Dmax/Ci50. The independent height law makes management-height matching structural and cannot identify any diameter parameter. Pd and Ci50 trade off for fixed CI; Dmax weakly constrains young trajectories. Adult initial-state and multiple-age fixtures reduce endpoint overfitting but do not resolve lack of measured reference trajectories.

| Species | Parameter | Grid value | Survival fraction | Mean multi-age height-band distance |
|---|---|---:|---:|---:|
| Beech | Ci50 | 3 | 1.000000 | 0.795229 |
| Beech | Ci50 | 6.5 | 1.000000 | 0.795229 |
| Beech | Ci50 | 10 | 1.000000 | 0.795229 |
| Beech | Dmax | 80 | 1.000000 | 0.795229 |
| Beech | Dmax | 120 | 1.000000 | 0.795229 |
| Beech | Dmax | 160 | 1.000000 | 0.795229 |
| Beech | Hmax | 35 | 1.000000 | 1.011690 |
| Beech | Hmax | 42.5 | 1.000000 | 0.669507 |
| Beech | Hmax | 50 | 1.000000 | 0.704489 |
| Beech | Pd | 0.4 | 1.000000 | 0.795229 |
| Beech | Pd | 0.9 | 1.000000 | 0.795229 |
| Beech | Pd | 1.4 | 1.000000 | 0.795229 |
| Beech | Ph | 0.4 | 1.000000 | 1.841613 |
| Beech | Ph | 0.75 | 1.000000 | 0.328286 |
| Beech | Ph | 1.1 | 1.000000 | 0.215787 |
| Oak | Ci50 | 2 | 1.000000 | 0.423995 |
| Oak | Ci50 | 5 | 1.000000 | 0.423995 |
| Oak | Ci50 | 8 | 0.888889 | 0.423995 |
| Oak | Dmax | 80 | 1.000000 | 0.423995 |
| Oak | Dmax | 120 | 1.000000 | 0.423995 |
| Oak | Dmax | 160 | 0.888889 | 0.423995 |
| Oak | Hmax | 35 | 0.962963 | 0.391280 |
| Oak | Hmax | 42.5 | 0.962963 | 0.367966 |
| Oak | Hmax | 50 | 0.962963 | 0.512739 |
| Oak | Pd | 0.4 | 1.000000 | 0.423995 |
| Oak | Pd | 0.8 | 1.000000 | 0.423995 |
| Oak | Pd | 1.2 | 0.888889 | 0.423995 |
| Oak | Ph | 0.4 | 0.962963 | 0.841613 |
| Oak | Ph | 0.65 | 0.962963 | 0.050855 |
| Oak | Ph | 0.9 | 0.962963 | 0.379517 |

## Juvenile, adult, release and premium timing checks

New-stock age25 D spans6.585–19.831cm Oak and6.985–22.921cm Beech among survivors; these are model controls, not field predictions. Stock promotes in the full-light control at age10; that representation change does not imply timber maturity. All adult four-state fixtures show strong>moderate>dense DBH at100. Juvenile age5/10/20/30/40/50 and adult50/75/100/125/150 are retained, plus25/200 checks. No hard pathological saturation/overshoot/storage-clamp failure occurred. No cross-species DBH/height ranking is enforced because N2 does not support a universal paired ranking.

D≥60 timing is only a [C] screening proxy; candidates reaching that size can still fail veneer because production asks for sufficient small-end/mid-log diameter, log length, pruning and defect status. It is not an age or price gate. Existing older Oak is not prohibited from premium production. Reported first-crossing ages/censored >200 values are in candidates.json, separated by lifetime and late-release history. No premium-production claim follows from proxy crossing.

## Evidence limitations and next production experiment

No spatial Hegyi recalculation, canopy feedback, survival, reproduction, density mortality, timber bucking, stand-statistic sampling or money is simulated. Site stays1.0. No bad density behaviour is tuned away. Do not interpret fixed-CI density labels as actual calibrated stands. Provisional heights are transfer/design bands, not evidence coefficients; source-corrected Beech crop-group Dq/mean height are not fitting targets for these individual controls.

Manager can authorise a bounded production-calibration comparison from deliberately contrasted surviving families, varying Pd/Ci50 and Ph/Hmax independently. Keep existing Sitka code/assets/anchors exact, site responses dormant and Model2 default unchanged. Compare new stock and established adults across multiple ages and spatial management patterns after density U3, record tree ages/origins, actual Hegyi, group Dq/mean height and explicitly defined top height separately; include premium timber quality/taper and cash. Require independent review, Unity growth/save/feedback gates and a calibration-labelled choice. Do not choose a single row from this lab as empirical truth.
