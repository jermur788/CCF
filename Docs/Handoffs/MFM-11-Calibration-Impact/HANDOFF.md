# MFM-11 — Calibration Impact Qualification — Handoff

**Claim:** #220
**Task:** MFM-11 — Calibration Impact Qualification
**Date:** 2026-11-01
**Status:** REVISED MATRIX EXECUTED — GAPS REMAIN
**Verdict:** `QUALIFICATION INCOMPLETE — GAP`

## Exact candidate / ancestry

- **Exact candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- **Branch:** `verify/mfm-11-calibration-impact-v1`
- **Worktree:** `/media/jer/ZX20/CCF-U1` (serialized Unity lane U1)
- **Operative Unity gate HEAD:** `f642550c9171a6f2ccf22825d0284cabd8d6c13e` (run 5)
- **Candidate identity:** Accepted `6784ab17...` MFM-09/MFM-10 lineage plus the tested #199 presentation/focus repair only.

## Independent review disposition

Independent review at `2bb42a36` returned **NEEDS CHANGES**. Its bounded follow-up (5 items: equal horizons, post-approval timber/economy, planting + objective logging, MFM-03 cross-year re-measurement, handoff corrections) is implemented in the gate and evidenced below. Item 1 of the latest round (equal horizons) is now **measured**; item 2 (post-treatment CI parity) is **provably unmeasurable** and is reported as such with the model-structural reason.

## Three-path comparison — OPERATIVE RUN 5, equal horizon (all paths at year 5)

All three paths restore from the same baseline (`baselineState`: year 2, after racks + sample release; 1,177 living, 25.471506 m² BA, cash 776,854, canopy 0.970165) and each advances exactly 3 years. `comparison-table.txt` rows: `PATH_A|year=2->5`, `PATH_B_RESULT|year=5`, `PATH_C_RESULT|year=5`.

| Metric (DIRECT model outputs) | A sample-only (y5) | B prescription (y5) | C manual override (y5) |
|---|---|---|---|
| Living stems | 1,177 | 1,093 | 1,168 |
| BA m² | 29.496807 | 27.308218 | 29.321141 |
| Cash cents | 776,854 | 594,310 | 531,053 |
| Canopy (model) | 0.957621 | 0.939526 | 0.956893 |
| Light (model) | 0.042379 | 0.060474 | 0.043107 |
| Regen cells occupied | 86 | 106 | 89 |
| Deadwood records / volume m³ | 1 / 0.127555 | 1 / 0.127555 | 1 / 0.127555 |
| Species split (sitka/beech/oak) | 1177/0/0 | 1093/0/0 | 1168/0/0 |
| Provenance orders / manual marks | 0 / 0 | 84 / 0 | 0 / 9 |

**B vs A at equal horizon:** −84 stems, −2.19 m² BA, **canopy −0.018095** (0.939526 vs 0.957621), light **+0.018095**, regen cells **+20**, cash −182,544.
**C vs B at equal horizon:** +75 stems, canopy +0.017367, light −0.017367, regen −17, cash −63,257.

### Handoff corrections to earlier drafts

- The earlier "B-vs-A canopy +0.002 lower" figure is **withdrawn** — it was an arithmetic error on a 2-year growth difference. The independent review's ≈0.0075 correction was itself computed on the **unequal-horizon** run-4 table (A at y5 vs B at y3). The correct like-for-like equal-horizon figure is **0.018095 lower** (run 5, both at y5).
- **Horizon disclosure:** run 4 measured A at year 5 but B and C at year 3 (defect: `MeasurePathResult` measured without advancing). The run-4 "equal horizons (year 5)" summary line was false. Run 5 corrects all three paths to year 5; the table shows it. All run-4 B/C absolute values were 2 years short of A and are superseded.
- "Timber/deadwood outcomes coherent" is now **supported** by data (below), not asserted.
- Protected compatibility is labelled **PARITY-09 evidence (cited) + save/load self-consistency only** — this gate's WorldHash is not compared to a pin.

## Timber / economy — run 5, with exact reconciliation

Quotes are now read **after `ApprovePendingWork`** and post-baseline event totals are filtered to `year > baselineYear`:

| Quantity (cents unless noted) | Path B | Path C |
|---|---|---|
| Quote sold volume (cm³) | 12,537,667 | 888,719 |
| Quote revenue | 67,456 | 4,199 |
| Quote cost (contractor) | 250,000 | 250,000 |
| Post-baseline timber revenue | 67,456 | 4,199 |
| Post-baseline contractor cost | 250,000 | 250,000 |
| Post-baseline stock cost | 0 | 0 |
| Net (revenue − contractor − stock) | **−182,544** | **−245,801** |
| Observed cash delta | **−182,544** | **−245,801** |
| Reconciled | **True** | **True** |

**The B-vs-C cash anomaly is fully explained:** both jobs pay the identical fixed mobilisation/minimum-job fee of **250,000 cents** (`MinimumHarvestJobCents`). B's 84 stems sell 12.54 m³ and recover 67,456 cents; C's 9 stems sell 0.89 m³ and recover 4,199 cents. The cheaper-looking manual job therefore costs the player *more* (−245,801 vs −182,544) because it harvests almost no timber against the same fixed fee. Path A is cash-neutral over its 3 idle years (776,854→776,854), confirming no annual fixed cost confound.

**Deadwood:** volume 0.127555 m³ in all three paths = the single baseline sample-release record (set to `RetainAsFallenDeadwood` by the shared baseline). The prescription's 84 removals resolve as sold timber (`sell-and-extract` felling outcome), leaving no additional deadwood — coherent and now measured, not assumed.

## Long-run completion — run 5 (cause corrected)

**Outcome: `Completed`, outcomeYear 25** (= `MinimumCompletionYear`). All **8/8 objectives achieved** at year 25 (logged in full from `ScenarioOneObjectives.Evaluate`):

| objectiveId | current | target | achieved |
|---|---|---|---|
| minimum-year | 25 | 25 | true |
| retained-canopy | 893 | 240 | true |
| continuous-canopy | 0.901015 | 0.35 | true |
| regeneration | 223 | 12 | true |
| fallen-deadwood | 85.732323 | 0.08 | true |
| managed-opening | 1 | 1 | true |
| introduced-beech | 1 | 1 | true |
| introduced-sessile-oak | 1 | 1 | true |

**Cause correction (review §2):** the earlier "completion unreachable" state was **not** a need for "more interventions". `ScenarioOneObjectives.Evaluate` requires `introduced-beech` and `introduced-sessile-oak` (shop species in `ScenarioOne.asset`); the gate never planted, so completion was unreachable by construction. Run 4/5 plant beech ×3 and sessile-oak ×3 through the ordinary shop → `TryPurchaseStock` → `TryDesignatePlanting` (brightest free cells) → Pending order → `ApprovePendingWork` → `AdvanceYear` execution. Repeated intervention: second thinning (10 trees) at year 17. Two-cycle review sequence satisfied.

## MFM-03 live/cross-year residual — run 5, three states

| State | Whole-stand (DIRECT) | CI parity | Growth drift (DERIVED) | Cohorts/live state |
|---|---|---|---|---|
| **Baseline y2** (actual post-Lesson-5 extension timing) | 1,177 living, 25.471506 m², mean DBH 16.369371 | **4.169829%** (source 8.059146 vs covered 7.723093, 13 covered areas) — within 20% GAMEPLAY CALIBRATION | +0.964878 cm vs y0 sample mean 15.404493 | juveniles 0, shelters 0, deadwood 1, marks 0 |
| **Post-B y5** | 1,093 living, 27.308218 m², mean DBH 17.612307 | **UNAVAILABLE — 0 covered areas** (states: NotCovered=1, Excluded=13, Manual=1; exclusion reasons `PlayerCompletedWork=13`) → **cross-year CI parity NOT DEMONSTRATED** | +2.207813 cm vs y0 | juveniles 0, shelters 0, deadwood 1, marks 0 |
| **Year 25** | 893 living, 49.230307 m², mean DBH 26.078663 | **UNAVAILABLE — `ReadCoverage` refuses:** "The sample's own release has not been executed and passed (Crop Trees 2/3, spread False, basal area kept True, least executed release 0.2618706)" → **NOT DEMONSTRATED** | +10.67417 cm vs y0 | juveniles 0, shelters 0, deadwood 238, marks 0 |

- The **live post-Lesson-5 residual is resolved** at the state the calibration actually governs: at the extension timing state (y2) the source-vs-covered CI delta is 4.17%, comfortably inside the accepted ±20% **GAMEPLAY CALIBRATION** tolerance.
- **Cross-year CI parity is NOT demonstrated**, and is structurally unmeasurable: after treatment the 13 covered units leave the Covered class via `PlayerCompletedWork` exclusions (the coverage model fails closed by design), leaving no comparison set; at y25 the sample read itself refuses because mortality degraded the sample's own release qualification (2/3 crop trees alive, least-executed release 0.2619 < 0.30). This is a model-structural limit, not a calibration failure, and is reported as a gap rather than resolved.
- Growth drift, whole-stand state and cohorts/deadwood **are** tracked cross-year (table above; deadwood accumulates 1→238 by y25).
- N1–N6 caveats carried: the 20% CI tolerance and 16 m spacing are GAMEPLAY CALIBRATION, not empirical forestry; simple SE separation is not independent-spatial proof; no claim here upgrades them.

## Protected compatibility

**Label (review §5): PARITY-09 Reference/Legacy40 evidence cited + save/load self-consistency only.**

- Save/load self-consistency: `WorldHash=AAD4776628E3D45F` preserved through a save/load round-trip (DIRECT check).
- `LegacyV15`/`LegacyV16` null in this world state (expected).
- PARITY-09 Reference gate `REFERENCE_FUTURE_V1_PASS` at exact candidate `3acdf9cb` cited as external evidence for Reference/Legacy40 compatibility. Production bytes are unchanged from that candidate (`git diff --stat 3acdf9cb HEAD` touches only handoff docs and `Tools/Verification/Mfm11CalibrationImpact/*`), so the citation carries. This gate performs **no** new frozen-pin comparison and does not claim to have verified Reference/Legacy40 itself.

## Offline evidence (uncontested by the review)

- Per-unit basal-area safeguard ≤40% on the frozen MFM-03 fixture, all three paths (27 checks, byte-identical repeat).
- MFM-10 species guard: fixture single-species; no Sitka→Oak/Beech quantitative transfer; calibration version `MFM03-CAL-1`.
- Fixture identity pinned (SHA-256 `4b4b9703…` / `c7dd5b81…`).

## Run lineage

| Run | Gate HEAD | Outcome | Notes |
|---|---|---|---|
| run1 | 72972cc4 | PASS (54 checks) | pre-review matrix; horizons unequal, quotes zero |
| run2 | dca81179/822e474c | FAIL→PASS | review-gap fixes v1; completion gap (no planting) |
| run3 | 70db0633 | COMPILE FAIL | duplicate `Run()` declaration; Editor reaped, fixed at 16552dc1 |
| run4 | 16552dc1 | PASS | completion achieved (8/8 objectives); economy non-zero; **horizons still unequal (A y5, B/C y3)**; post-B CI parity silently `covered=0` |
| **run5** | **f642550c** | **PASS (47 checks, 2 shots, 322.4 s)** | **operative: equal horizons y5, reconciled economy, honest CI-parity states, completion 8/8** |

## Commands and receipts (run 5)

```bash
/media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner "Mimo-MFM-11" --task-id "MFM-11" --lane U1 \
  --branch "verify/mfm-11-calibration-impact-v1" --head "f642550c9171a6f2ccf22825d0284cabd8d6c13e" ...
python3 Tools/Verification/Mfm11CalibrationImpact/run_gate.py --gate calibration \
  --lock-id "bc6bc18b8d574a63938940a755401acb" --owner "Mimo-MFM-11" \
  --expected-head "f642550c9171a6f2ccf22825d0284cabd8d6c13e" \
  --out /media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/unity/run5
```

| Receipt | Value |
|---|---|
| Marker | `MFM11_UNITY_PASS checks=47 shots=2` |
| Unity exit / failures | 0 / none |
| Harness + licence removed, host saves unchanged, lane clean | true |
| `unity.log` SHA-256 | `89f61be02775d5549cfccca4266bdb56f5f4f2257a06101bce48aa7084ebdf95` |
| `comparison-table.txt` SHA-256 | `e3ea9b1c29e2dcd3ac9be2c1282a7b9f2c8decc30717132fdfe1a12346cebb07` |
| `process-result.json` SHA-256 | `09ac810227729c2f4ac64d67f56c0350227d356abedbc4fa250c0c87a1ccad45` |

Evidence root: `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/` (offline/ + unity/run1..run5).

## Remaining gaps (exact)

1. **Post-treatment cross-year CI parity NOT DEMONSTRATED.** Unmeasurable in the model: after treatment, covered units leave the Covered class via `PlayerCompletedWork` exclusions (0 covered areas at y5; exclusion reasons logged), and at y25 `ReadCoverage` refuses because the sample's own release qualification degrades with mortality (2/3 crop trees, least-executed release 0.2619). The live post-Lesson-5 check the task requires **is** done (y2, delta 4.17% within the 20% GAMEPLAY CALIBRATION tolerance); the cross-year *comparison* cannot be reproduced at post-treatment states and is not claimed.
2. **Protected compatibility is citation-level only:** PARITY-09 Reference/Legacy40 evidence cited + save/load self-consistency; no frozen-pin comparison performed by this gate (Manager acceptance of the cited external gate is required for that pass condition).

## Verdict

**`QUALIFICATION INCOMPLETE — GAP`**

- The two review-blocking items are resolved as far as measurement allows: (1) equal horizons are now **actually measured** (table shows year 5 for A/B/C); (2) post-treatment CI parity is **explicitly reported unavailable with reason** — it cannot be measured in the model, so it remains a gap under the "if they cannot be measured" rule.
- **No material accepted-outcome break was found.** `BLOCKED — CALIBRATION IMPACT` is not warranted: at equal horizon the wider path retains canopy 0.9395 (≈2.7× the 0.35 objective threshold; 0.901 at y25), improves light/regeneration, reconciles exactly through the ordinary economy (cash ≥ 0 throughout, no subsidy/bypass), keeps the per-unit ≤40% safeguard and the species guard, and long-run complete-loop completion is not merely reachable but **demonstrated** (Completed y25, 8/8 objectives, repeated intervention y17).
- `MULTI-SCALE CALIBRATION QUALIFIED` is **not** issued: the full required matrix is not wholly evidence-backed while gap 1 stands (and gap 2 needs Manager acceptance of the cited PARITY-09 evidence). Worker completion does not self-accept; Manager review decides whether the structural unavailability of post-treatment CI parity is acceptable for the token.

**Evidence labels:** DIRECT model outputs (stems, BA, cash, volumes, revenue, canopy, light, regeneration cells, objectives, outcome); DERIVED metrics (species splits, deltas, drift, cash decomposition reconciliation); GAMEPLAY CALIBRATION (±20% CI tolerance, 40% BA ceiling, 30% release share, 250,000-cent minimum-job fee). No empirical forestry claims.

## Clean return state

`git status --porcelain` clean; no harness `.cs`/`.meta` under `Assets/ForestPrototype/`; no Editor/Xephyr/openbox running; machine-wide lock released; no push; #194 NO ACTION; no Windows build; no production retune.
