# Implementation and calibration review

Current task branch evidence: [Handoff](Handoff.md), [Implementation record](ImplementationRecord.md), [Calibration](CalibrationResults.md), [Frequency](StormFrequencyComparison.md), [Vulnerability](VulnerabilityComparison.md), [Salvage](SalvageCalibration.md), [Performance](PerformanceResults.md), [Save schema](SaveVersioning.md), [UI handoff](TutorialUIHandoff.md), [Decision proposal](CanonicalDecisionProposal.md). New-game storms remain off; review is not integration acceptance.

The following section is preserved historical supporting readiness research. Its older rates/defaults/base/asset requirements are proposals, not current implementation authority.

---

> Preserved supporting readiness design from task/windthrow-readiness @9a9f4fb. The later Manager implementation packet governs current work: base1fbefd8/context62593b2, save18/regeneration2; external edge neutral; occurrence/severity and salvage costs remain calibration candidates; new-game storms remain off pending review. This historical proposal does not approve its old defaults.

# Storms / windthrow / salvage v1 — pre-production readiness

**Branch:** `task/windthrow-readiness` · **Base:** `origin/main` @ `a8596df9c52669a36709f09e85dfe6568640af49` (Growth Model 1, D-048, save v17).
**Status:** research and design for Manager review. Not decision authority. **No production, save, scene, prefab or material change. Unity and Blender were not run.** Static inspection plus one offline Python prototype.

Evidence labels: **[A]** empirical, **[B]** transferable, **[C]** calibration/gameplay choice, **[I]** inference or unverified general knowledge, **[REPO]** inspected code, **[HIST]** git history, **[PROTO]** offline prototype.

## Read first

1. `CurrentWindSystemAudit.md` — what exists and why every tree reads "high".
2. `StormEventArchitecture.md` + `TreeVulnerabilityDesign.md` — the proposed model.
3. `DecisionMatrix.md` — decisions needed.
4. `SolImplementationPacket.md` — the ready-to-issue packet (start after Regeneration Model 2).

## Contents

| File | Purpose |
|---|---|
| `CurrentWindSystemAudit.md` | Wind code, thresholds, history, mortality/deadwood/economy hooks |
| `BoundaryDecision.md` | 40 m boundary: interior vs bounded vs buffer; recommendation |
| `WindthrowEvidenceReview.md` | Evidence by topic, labelled; what must not be done |
| `StormEventArchitecture.md` | Layer 1 events, placement in the annual step, determinism |
| `TreeVulnerabilityDesign.md` | Layer 2 index, offline ranking check |
| `RecentThinningAssessment.md` | Deriving recent exposure without new fields |
| `WindthrowOutcomeDesign.md` | One fallen state in v1; authoritative effects |
| `SalvageDesign.md` | Salvage via existing work, economy and yield concepts |
| `GameplayCases.md` | Eight required cases + checks |
| `LongRunCalibrationPlan.md` | Unity matrix, targets vs sanity checks |
| `PlayerFeedback.md` | Stability bands replacing "high", forecast, map, Annual Review |
| `TutorialHandoff.md` | Later teaching handoff |
| `StormAssetAudit.md` | Existing assets, state, gaps |
| `StormAssetRequirements.md` | Minimum package, briefs, asset/mechanic matrix |
| `StormSaveAssessment.md` | Minimum save additions |
| `PerformancePlan.md` | Batched pipeline, rendering budget |
| `DecisionMatrix.md` | Accepted / supported / proposed / decision / deferred |
| `ImplementationRoadmap.md` | Sequence, ownership, ecology integration points |
| `SolImplementationPacket.md` | Full packet for W1 + W2 |
| `prototype_output.txt`, `prototype_case_scores.csv` | Offline prototype evidence (`Tools/Verification/WindthrowV1/vulnerability_prototype.py`) |

## Offline prototype

```bash
python3 Tools/Verification/WindthrowV1/vulnerability_prototype.py
```

Standard library only; reads `Docs/Research/SitkaGrowthMortality/Evidence/growth_model1_stand_development.csv` if present. Results:

| Candidate | Required orderings held |
|---|---|
| Current `GetWindRisk` | 7 / 11 |
| H/D only | 6 / 11 |
| Height² × H/D | 6 / 11 |
| Proposed v1 | **11 / 11** |

It shows formulation quality only. It is **not** a calibration of storm probabilities.
