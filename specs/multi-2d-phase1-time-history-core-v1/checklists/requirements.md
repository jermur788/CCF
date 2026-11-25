# CCF Multi 2D — Phase 1 Step 1: Time Pacing, History Query & Woodland Structure Descriptors — Requirements Checklist

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-core-V1`
- **Spec status:** `IMPLEMENTING`
- **Checklist owner:** Independent Reviewer / Overall Manager (every box starts unchecked `[ ]`; only an independent reviewer or Manager ticks rows after inspecting evidence)
- **Implementation under test:** `feat/multi-2d-phase1-time-history-core-20261124-v1` (base `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43`)
- **Date:** 2026-11-25

> **Rule:** Every row below MUST remain unchecked `[ ]` until the independent reviewer returns a verdict. The Primary Implementer records observed evidence in `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md` without ticking any checklist box.

## Acceptance rows

- [ ] **P1-TH-01 — Preview cadence rules, pre-Fell observation period, Fell anchoring, overdue fallback & fence enforcement:**
  - Pre-first-Fell (`AnchorFellYear == null`): `Eligible == true` unless open work exists, `IsObservationPeriod == true`, `SuggestedTargetYear == currentYear + 4` (clamped `+5`), labelled as a quiet OBSERVATION period (explicitly not a completed silvicultural thinning cycle) with `GAMEPLAY CALIBRATION` note.
  - Post-Fell: anchors to latest `MultiAreaFellRecord.EcologicalYear` across committed `Events[].Areas[].Fells` (`SuggestedTargetYear = min(anchor + 4, currentYear + 5)`, `MaxTicksPerPress = 5`); non-Fell work does not set a Fell anchor.
  - Overdue fallback (`anchor + 4 <= currentYear`): `SuggestedTargetYear = min(currentYear + 4, currentYear + 5)`.
  - Stale or mismatched fence refused via `RequireFence` (`PlayerPlanStateStale` / `PlayerSourceStale` / `PlayerSessionMismatch`) with zero mutation.
  - Evidence:

- [ ] **P1-TH-02 — Dual skip-vs-manual parity on both MA-1 and MA-1-MIXED:**
  - Pre-Fell observation skip and post-Fell review skip via `AdvanceYears` match $N$ direct single-year `Advance` calls identically across the full authoritative root (canonical `CaptureSave` + `Phase1SaveCodec` byte comparison with session authority normalised) and annual history on both `MA-1` and `MA-1-MIXED`.
  - Evidence:

- [ ] **P1-TH-03 — WorkPlanOpen refusals, target bounds & between-step CancellationToken:**
  - `WorkPlanOpen` refused with zero mutation for Pending and Approved orders of every type (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`); never auto-approves, auto-executes, or passes `consentToCancelWork = true`.
  - `InvalidTargetYear` refused with zero mutation for `requestedTargetYear <= currentYear` and `requestedTargetYear - currentYear > 5`.
  - `CancellationToken` checked strictly between annual steps, returning `CancelledBetweenYears`, `ReachedTarget = false`, already-completed years only, and zero partial year or fabricated remainder records.
  - Evidence:

- [ ] **P1-TH-04 — SchemaVersion 2 Save → Load round-trip continuity:**
  - Save → Decode → Restore into a fresh continuation (both pre-Fell and post-Fell + several years) produces identical `PreviewNextReviewYear`, `AdvanceYears`, `ReadHistoryIndex`, `ReadHistoryYear`, and `ComputeStructure` results vs an uninterrupted run.
  - `Phase1SaveFormat.SchemaVersion` remains `2` and `src/Core/Phase1/` remains 100% untouched.
  - Evidence:

- [ ] **P1-TH-05 — Read-only Decision → Outcome history query, 2D timing, paging, provenance & omissions:**
  - Newest-first keyset paging (`ReadHistoryIndex`) is cursor-stable over $\ge 15$ years, defines recorded years honestly as the union of `AnnualHistory` (`YearBefore`/`YearAfter`) and work-log years, and leaves `HasAnnualOutcome = false` / `LiveStemsEndOfYear = null` when a year has work without an annual outcome.
  - `ReadHistoryYear` preserves `WorkYear = Y` vs `ObservedYear = Y + 1` timing (never relabelling Year `Y` cuts as Year `Y + 1`), groups work by area and action type with exact recorded counts/IDs, exposes whole-woodland `Phase2PlantedAccount` totals without fabricated per-area splits, and includes later observed comparison (`CURRENT`/`DERIVED`).
  - All six provenance values (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`) are present and mapped onto D-073 tiers; past overhead canopy cover, past tree-by-tree structure, and per-area `Phase2PlantedAccount` splits are explicitly marked `NOT RECORDED`.
  - Exact non-causal wording present; zero weather claims; zero economy/money/price/objective/euro fields or `€0` placeholders.
  - Evidence:

- [ ] **P1-TH-06 — Woodland structure descriptors (`M_i`, `T_i`) analytical oracles & purity:**
  - Pure Sitka yields $M_i = 0.0$.
  - Hand-checked 4-tree and 6-tree mixed fixtures match exact analytical $M_i$ and $T_i$ (per-area and whole-woodland).
  - 2-tree and 3-tree stands compute with $k_i = 1, 2$ while reporting `MaxNeighbourCountK = 3` and effective $k_i$; 0-tree, 1-tree, and zero-positive-DBH-pair ($T_i$) stands return `null`.
  - Exact distance ties break deterministically by ordinal `TreeId`; stumps, standing dead, deadwood, and unpromoted `PJ` saplings are excluded; cross-rack neighbours across shared borders are included; `Compute` mutates zero state (full canonical root byte-identical before/after).
  - Evidence:

- [ ] **P1-TH-07 — Honesty & Gate G3 label assertions:**
  - `GAMEPLAY CALIBRATION` framing verified on pacing contracts (not universal Irish forestry advice).
  - Quiet observation period framing verified (not a completed silvicultural thinning cycle).
  - Non-causal history statement verified (no weather claims).
  - Exact spatial-structure honesty label verified: `"Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast."`
  - `TimePacingStopReason` contains only the 6 valid 2D stop reasons (zero unmodelled 3D stop values: no `StormEvent`, `TeachingGateIncomplete`, `ReviewUnread`, `YearCapReached`, or `ObjectiveOrOutcomeReached`).
  - Evidence:

- [ ] **P1-TH-08 — Scope compliance relative to base `5438c94c…`:**
  - Zero changes under `Tools/MultiRackSitkaP1Lab/src/Ui/`, `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/`, `Assets/`, `Packages/`, `ProjectSettings/`, or coordination docs relative to `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43`.
  - Evidence:

- [ ] **P1-TH-REG — Clean builds, full regression, 9 frozen anchors & runtime hygiene:**
  - Clean build (`--disable-build-servers`, external `TMPDIR`, `~/.dotnet` SDK 8.0.134): `0 warnings, 0 errors` for `MultiRackSitkaP1Lab.sln`, `SitkaP1.Ui.csproj`, and `Phase1Save.Tests.csproj`.
  - All 267 baseline Core tests, all 48 baseline Save tests, and all new Step 1 tests pass (`0 failed`).
  - All 9 frozen anchors unchanged (`7e3a2a86…`, `a692a6d5…`, `814e8165…`, `8a926776…`, `c4b670ed…`, `c67edc6b…`, `776a68df…`, `9a99f781…`, `93e9abda…`).
  - Protected listeners `:33767`, `:43649`, `:46593` remain listening untouched before and after verification.
  - Evidence:

- [ ] **P1-TH-REVIEW — Independent review and verdict:**
  - Independent reviewer verification complete; no self-review accepted.
  - Evidence:

## Verdict

- **Spec conformance:** NOT VERIFIED (pending independent review)
- **Implementation correctness:** NOT VERIFIED (pending independent review)
- **State:** `IMPLEMENTING` (at handoff: `IMPLEMENTED ON TASK BRANCH — NOT REVIEWED, NOT INTEGRATED, NOT ACCEPTED`)
