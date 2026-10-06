# Later packets — outlines (decision- or Sol-gated)

These packets are **not ready to issue**. Each needs a product decision (`DecisionMatrix.md`) and, except P9, Sol's growth/mortality and save v17 integration first. They are outlined so file ownership and risks are known in advance.

## P6 — Tutorial arc re-sequencing + per-forest stage progress

- **Gate:** decisions #8 (per forest vs per device), #23 (N years), #24 ("no thinning this year" event).
- **Scope:** stage engine S0–S12 (`ProposedTutorialArc.md`) on top of the existing observation engine; per-forest stage progress (new save field, next save version after Sol's v17); explicit "No thinning this year" event; reassessment prompt after N years.
- **Files:** `UI/LearningObjectivesView.cs`, `UI/MenuHelpView.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/WorkPlanView.cs`, `ScenarioOne/ScenarioOneManager.cs` (event), `ForestSaveData.cs`, `ForestSaveValidation.cs`, `Tools/Verification/MenuTutorialVerification.cs`.
- **Risk:** save schema (single writer); MenuTutorial harness rewrite; must keep the PlayerPrefs migration path.
- **Tests:** beginner-flow tests 1–6 in `ImplementationRoadmap.md` §4; save/load of stage progress; legacy save without the field.

## P4 — Practice mode (marteloscope)

- **Gate:** decision #16.
- **Scope C1:** sandbox on the Year-0 copy (the Reference-preview pattern: capture, load, block saving, banner, restore); plan A/B/C store (id lists, session-only); assessment panel (`MarkingAssessmentFramework.md`); long-term preview at 5/20 years. **C2:** the same on a copy of the current forest; optional "carry marks back as proposals".
- **Files:** `ScenarioOne/ScenarioOneManager.cs` (sandbox state), `ForestSaveController.cs` (save block), `UI/ScenarioOneUiRoot.cs`, `UI/WorkPlanView.cs`, new `UI/PracticeView.cs`, `UI/ResidualStandSummary.cs`, `ForestPlayer.cs` (input guard during transitions).
- **Hard technical requirements (found in this work):** restore from a **serialized copy** and let ≥1 frame pass before enumerating or capturing (`CaptureData` includes inactive trees). Learning/objective/Reference isolation.
- **Tests:** practice-mode tests 17–20.

## P7 — Objective redesign

- **Gate:** decisions #21, #22; **explicit authorisation of a completion-anchor change**.
- **Scope:** competence-based objectives (`DecisionMatrix.md` §2.6), with explanation text.
- **Files:** `ScenarioOne/ScenarioOneObjectives.cs`, `ScenarioOne/ScenarioOneDefinition.cs` (+ the `ScenarioOne.asset` serialized values — high-conflict), `UI/AnnualReviewView.cs`, `Tools/Verification/ScenarioOneCompletionVerification.cs` (anchor), `ScenarioOneProgressVerification.cs`.
- **Risk:** highest. Changes the completion anchor and the completion year; touches serialized assets.

## P8 — Permanent sample plots (Phase 2 diary)

- **Gate:** decision #19.
- **Scope:** ≤3 one-cell plots, 5-year measurements copied from authoritative state (`PermanentPlotDecision.md`).
- **Files:** save data/validation/controller, `ScenarioOneManager.cs` (measurement on annual step), `ForestPlayer.cs` or the map (placement), `UI/AnnualReviewView.cs`, `UI/StandMapView.cs`.
- **Risk:** save schema; determinism of recorded data; Reference isolation.

## P9 — Bio Tree (optional)

- **Gate:** decision #17, and the existence of at least one habitat-tree state.
- **Scope/risks:** `BioTreeDecision.md` "If the user chooses B anyway".

## Calibration — wind and competition labels

- **Gate:** decisions #26/#27, after Sol's height model (H/D changes).
- **Scope:** the threshold values only (`ForestEcologyController` serialized fields `windLabelLowAt`/`windLabelHighAt` live in `ForestTest.unity` — **scene file, high-conflict**), plus the competition label thresholds in code. Re-run the residual harness to show the distribution of labels afterwards.
- **Owner:** the ecology owner (Sol) or with Sol's agreement, because the thresholds sit in `ForestEcologyController`.
