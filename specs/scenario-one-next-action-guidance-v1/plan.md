# Next-action guidance implementation plan

- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Spec status:** DRAFT
- **Owner:** Overall Manager; drafted EVENT 012, 2026-11-04

## Current-state findings

See spec F1–F9 for revision-specific observed facts. Main and candidate differ materially. Candidate RackV2Instruction already supplies entrance-first wording; current screen highlights close. Keyboard completeness is unverified and Tab conflicts with focus traversal.

## Proposed approach and trade-offs

After approval, add a small transient presentation projection beside existing LearningObjectivesView state logic, consumed by Objectives and Stand Map. Keep existing teaching facts, instruction helper, counters and authoritative validators as the sources; no persistent guidance engine or manager. Refresh the cue with existing view refresh/state changes without moving focus every frame. A static, labelled teaching map avoids confusing an illustrative target with saved geometry; a live inset would require additional state/filtering and is a product decision Q2.

## Boundaries / affected files

Future candidate scope for separate ownership assignment: LearningObjectivesView.cs / .Sequence.cs, StandMapView.cs, TeachingDiagram/TeachingDiagramPlan and shared USS; Curriculum only for reuse/reconciliation with #255. Reuse UI/Map transform/symbols read-only where possible. Any map helper modification needs explicit owner. No production files changed here. No scenes, prefabs, packages, simulation, save schema, Reference Future or legacy geometry edits.

## Ownership and migration

This worker owns only the four spec files and named handoff. #200 owns active-tasks; DPC owns routing ledger. #255/#256 production owners must be reconciled before future work. Unity qualification requires a new authorised lane/lease; this lane must not launch Unity. No migration: derive cue on load, preserve stable IDs.

## Sequence and verification

1. T-01 resolve Q1–Q4 and independent design review; Manager approves spec explicitly.
2. T-02 implement modal-first scope resolution, authoritative RackV2Instruction reuse and FR-3 ordered fallback, then Objective 1 cue projection and cross-screen emphasis with focused state/negative tests.
3. T-03 produce approved reference map-like figure; geometry and visual conformance review.
4. T-04 run spec AC-1–6 matrix; independent review and user clarity decision before integration.

Tier 1 offline/source checks first, then bounded Tier 2 UI qualification on exact HEAD at both resolutions. Do not rerun broad Tier 3 without concrete risk. Implementation correctness checks action dispatch/validators/no mutations; conformance checks user complaint, diagram semantics and accessibility. Risks: stale focus after rebuild, two primaries in nested consent, obsolete copy, false cleared-route art, Tab conflict. No runtime pass is claimed by this plan.

## Review response

- **P1 FIXED:** FR-2 now resolves `editor.ConsentPending`, `editor.ResetPending`, `reopenPending`, then other blocking modals before every background row; only the active scope has primary emphasis/activation. Safe decline/cancel wins, with no-primary fallback when no safe modal choice exists. AC-1 tests each prompt against competing planning/confirmed states and re-derivation after dismissal.
- **P2 FIXED:** FR-3 makes candidate `RackV2Instruction()` authoritative for Objective 1 wording/stage after modal handling, projects its first unfinished step onto the screen, then applies validators. Corrections use a fixed first-eligible allowlist (map, planning, reason-matched undo, over-target selected unlocked delete); otherwise reveal the same instruction/reason. No arbitrary enabled-control selection or second instruction source. AC-6 tests helper branches, ties and ambiguous reasons.
- **P3 FIXED:** F6 was re-derived with `git show` and numbered lines at main `8f0f230190830855112e3e7a1d7544db94a7f591`; every symbol citation now names `StandMapSymbols.Build()` and its `MapSymbol` entry. Racks :86–97, navigation/grid :99–104, tree marks :106–107; not candidate 72b4efbd.

DRAFT remains unchanged. Planned canopy/no-clearing semantics, shape/pattern/text accessibility and saved-state derivation without schema changes remain required. Docs-only; no Unity or production verification claimed.
