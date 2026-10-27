# TASK PACKET — MFM-02 Rack-Served Sample and Coverage Geometry Adapter

**Feature:** Multi-Scale Forestry Management V1 (D-065)  
**Spec:** `specs/multiscale-forestry-management-v1/` — APPROVED  
**Task:** `MFM-02`  
**Class:** approved-spec implementation / bounded geometry adapter  
**Manager:** ChatGPT Game Dev / Overall Manager  
**Task claim:** GitHub issue #22

## Base and authority

Start from current clean `main`. Do not rely on the stale project-context Overview for Rack V2 implementation truth; inspect current repo and bind the handoff to the exact starting SHA.

Prerequisites:
- Rack V2 core is integrated;
- MFM-01 is Manager-approved in `Docs/Handoffs/MFM-Implementation-01/MANAGER-REVIEW.md`;
- authoritative MFM behaviour comes from D-065 and `specs/multiscale-forestry-management-v1/`.

Recommended branch: `feature/mfm-rack-sample-geometry-v1`.
Use a dedicated clean worktree. Refresh live `Docs/Project/coordination/active-tasks.md` before editing.

## Goal

Implement MFM-02 only: repoint the existing Lessons 2–5 teaching footprint to persisted confirmed Rack V2 geometry and add plain deterministic rack-served coverage geometry.

Exit target:
`RACK-SERVED SAMPLE GEOMETRY QUALIFIED — READY FOR MANAGER REVIEW`

## Required behaviour

### Teaching rack and footprint
- applicable world: Scenario One Enlarged80 with Rack V2 plan confirmed;
- order the five persisted Rack V2 racks by perpendicular offset along their common normal, with stable rack ID tie-break;
- representative teaching rack = third/central rack;
- anchor = closest point on that confirmed rack centreline to stand centre `(0,0)`, clamped so ±12 m stays on the segment;
- teaching half-length remains 12.0 m;
- fixed teaching half-width = 9.0 m on each side;
- rack corridor itself is excluded using actual confirmed rack segment/width geometry;
- preserve existing near/middle/far and both-side teaching semantics;
- preserve existing Lessons 2–5 intent.

### Parameter separation
- introduce/retain an explicit teaching Crop Tree spacing constant = 8 m;
- do not alias that constant to the 8 m Hegyi neighbourhood;
- do not change either accepted value in this task.

### Rack-served coverage geometry
- derive coverage boundaries independently of the fixed 9.0 m teaching footprint;
- interior unit boundaries lie halfway between neighbouring rack centrelines along the common normal;
- represent left/right outer remainders explicitly rather than silently assigning them to the nearest rack;
- provide enough plain deterministic data for later MFM-03 calibration and MFM-08 map rendering;
- no coverage/proposal decision policy yet.

### Confirmation/reopen boundary
- derive MFM teaching geometry from persisted `RackPlan` records only while the Rack V2 plan is confirmed;
- do not change `OPEN-3`, `ReopenRackPlan`, completed-work locking, or Rack V2 persistence semantics;
- if plan confirmation is absent, do not silently rebind MFM teaching geometry to legacy/authored lines or a different rack;
- model-0/Legacy40 keeps existing historical authored-layout teaching behaviour.

## Expected production surface

Expected primary files:
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs`;
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBayReader.cs`;
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`;
- optionally one new plain geometry helper under `Assets/ForestPrototype/ScenarioOne/`;
- `Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs` only if truthful V2 rack-index wording requires a narrow update;
- focused verification under `Tools/Verification/ScenarioOneNonUi/` or a bounded MFM verification location.

Do not edit Rack V2 map/editor UI owned by RACK-V2-04/04A.

## Explicit exclusions

Do not implement:
- prescription derivation or fit logic (MFM-04);
- Crop Tree candidate generation or candidate spacing calibration (MFM-03/05);
- proposal state or Work Plan mutation (MFM-07);
- MFM save/provenance schema (MFM-06);
- MFM coverage map UI (MFM-08);
- new lesson insertion/copy (MFM-09);
- Rack V2 map planning/editing/confirmation/field-marking tasks;
- OPEN-3;
- S1-C/S1-D/parity/economy/ecology changes;
- Windows build.

The unmerged MFM-03B 16 m centre-first candidate result is supporting evidence only and must not become a production constant here.

## Verification

### Static/offline
Prove at minimum:
- shuffled rack input/order still selects the same deterministic third rack;
- common-normal ordering and rack-ID tie-break are deterministic;
- closest-point anchor and end-clamping are correct;
- 9.0 m teaching half-width is independent of neighbour spacing;
- teaching 8 m and Hegyi 8 m are separate constants;
- actual rack corridor geometry is excluded;
- half-distance coverage boundaries are distinct from teaching footprint;
- outer remainders are explicit;
- model-0/Legacy40 existing behaviour remains unchanged;
- no prescription/proposal/save state is introduced.

### Focused Unity qualification
Obey the machine-wide Unity lock and current queue. PARITY-03 Unity claim #10 retains older priority if still pending/running.

A disposable isolated five-rack fixture matching the already-qualified Rack V2 geometry is authorised while RACK-V2-04/05 player authoring is unfinished. It must use isolated save/config state and must not become production geometry policy.

On exact candidate HEAD verify:
1. fresh V2 five-rack confirmed plan selects the deterministic central/third rack;
2. selected rack ID/geometry remain stable through save/load;
3. anchor and ±12 m footprint are on the confirmed segment;
4. fixed 9.0 m footprint and actual corridor exclusion produce truthful live tree membership/counts;
5. Lessons 2–5 remain reachable/valid against the V2 teaching frame;
6. three Crop Trees at 8 m teaching spacing, accepted release rule and 40% safeguard remain feasible in the live footprint;
7. model-0/Legacy40 behaviour remains unchanged;
8. Reference Future protected integrity remains unchanged where applicable;
9. compile/import clean.

Historical 69-tree / 1.29490 m² values are reference-layout fixtures only. If the new qualified V2 footprint differs, record the new measured fixture with provenance; do not silently rebaseline a protected target.

No rendered MFM coverage UI is required here. Capture only if useful for checking teaching-frame placement/copy; MFM-08 owns map coverage rendering.

### Later required requalification
Before beginner/external Scenario One testing, repeat the relevant geometry path using the real player-authored Rack V2 workflow after RACK-V2-04/05/07 are integrated. The disposable fixture does not satisfy that final player-path acceptance.

## Handoff

Publish `Docs/Handoffs/MFM-Implementation-02/HANDOFF.md` with:
- worker/model/provider;
- branch/worktree/starting HEAD/final tested HEAD;
- production files changed;
- exact geometry API/types added or changed;
- deterministic ordering/anchor/coverage contract;
- offline results;
- Unity lock receipt and gate results;
- exact live rack IDs/endpoints/angles/widths and selected teaching rack;
- measured teaching footprint tree count/area and comparison to historical fixture;
- model-0/Legacy40/Reference evidence;
- limitations and deferred real player-authoring requalification;
- recommendation `APPROVE`, `NEEDS CHANGES`, or `REJECT` for MFM-02 only.

Before substantive production edits or Unity execution, mark task claim #22 `[RUNNING]` with actual agent/model/provider, branch/worktree and starting HEAD. At publication mark `[HANDED-OFF]`, link the handoff and close.

## Stop conditions

Stop and return to Manager if:
- fewer/more than five valid confirmed racks make the accepted third-rack contract impossible in the intended Scenario One path;
- accepted Rack V2 geometry cannot produce a defensible common-normal ordering;
- the fixed 9.0 m footprint conflicts materially with valid final Rack V2 geometry or protected Lessons 2–5 behaviour;
- a required fix needs Rack V2 lifecycle/OPEN-3/save-schema changes;
- another writer owns the same production files;
- verification would require weakening a protected gate.

No whole-MFM, external-test, S1-D, or Windows-build readiness is implied by this task.
