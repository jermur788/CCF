# RACK-V2-12 — Objective 1 functional end-to-end qualification

**Task:** RACK-V2-12, claim #97, umbrella #91.
**Exact qualified candidate:** `a900b7faec3e48f6cfc8be7e3bb3b147fe3a61aa`
(branch `feature/scenario-one-rack-v2-functional-v1` on integration head `1bd9083`, plus the in-scope marking-before-approval UI fix and the harness Cursor disambiguation)
**Unity:** 6000.6.0f1; lane U1; machine-wide lock owner `DeepSeek-RACK-V2-12`, task `RACK-V2-12`; normal release.
**Executors:** Claude A / Claude Opus 5.5 wrote the functional gate and the UI fix before a provider rate-limit stop; the DeepSeek #91 operator completed the gate run and the two compatibility gates.
**Exit:** `OBJECTIVE 1 FUNCTIONAL PASS`

## In-scope functional fix

`LearningObjectivesView.Sequence.cs`, `LearningObjectivesView.cs`, `ScenarioOneUiRoot.cs` and `WorkPlanView.cs` now enforce the D-064/OPEN-2 ordering in a Rack V2 world: while a confirmed rack owns Pending removal work and its entrance is unmarked, the Work Plan will not approve the rack work and explains that the player must mark the entrances first. The manager's own approval API and model-0/Legacy40 worlds are unchanged. This is the fix for the prior `rack.approve` failure after approval.

## Gate results on exact `a900b7f`

| Gate | Result | Evidence |
|---|---|---|
| New Objective 1 end-to-end gate (`RackV2Objective1EndToEnd`) | **PASS** `RACKV2_O1E2E_PASS checks=173 shots=24` | `RackV2-12-20261029/r12-02/` |
| Model-0 progression regression | **PASS** `RV2L_MODEL0_PASS` (85.9 s) | `.../legacy-model0/` |
| Legacy40 progression regression | **PASS** `RV2L_LEGACY40_PASS` (69.6 s) | `.../legacy-legacy40/` |

The end-to-end gate runs one fresh game through: real startup → Start New → walk → Stand Map → Plan Racks → suggested layout → edit/refuse four-rack confirm/undo/move → Confirm → Tree Marks help → approval-before-marking refused → five native spray interactions with negative cases → marking complete → Work Plan approval → execution/`rack.result` → Objective 2 teaching on the confirmed teaching rack → real C Crop Tree toggle → disk save/load preserving ids, geometry, marks, order states and locks. It produced 24 captures across 1280x720 and 1920x1080.

## Carried evidence

- RACK-V2-09 field 99 checks/10 shots, runtime 14, save 35; independent review PASS; integrated.
- RACK-V2-11 persistence v2 30, coldstart 2, legacy40 7, save03b 35; integrated.
- RACK-V2-10 Help/teaching static 97/15; integrated.
- S1-D T-06 Q1-Q5 PASS and post-merge Q1/compile PASS; independently reviewed; integrated.

## Residual / boundaries

- RACK-V2-13 owns the dedicated rendered-appearance acceptance; this handoff does not claim it. The 24 captures are available for that review.
- No growth/economy/species regression is claimed here; the existing protected gates are carried from the integrated candidate and must be included in RACK-V2-13/R14.
- No save-schema, Reference Future v1, scene/prefab/material, package or ProjectSettings change. No Windows build.
- The gate is a functional/rendered-capture gate, not a manual player playthrough; user acceptance remains separate.
