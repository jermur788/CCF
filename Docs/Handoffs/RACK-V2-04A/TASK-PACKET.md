# TASK PACKET — RACK-V2-04A Suggested Rack Layout Read-Only API

**Feature:** Scenario One Objective 1 Rack Planning & Field Marking V2 (D-064)  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — APPROVED  
**Parent task:** `RACK-V2-04`  
**Task:** `RACK-V2-04A`  
**Class:** bounded cross-lane API repair / prerequisite  
**Status:** READY / DISPATCHED  
**Manager:** ChatGPT Game Dev / Overall Manager

## Why this task exists

RACK-V2-04 stopped correctly with `CROSS-LANE API REQUIRED`. The Stand Map must show the approved five-rack V2 suggested layout without duplicating geometry policy or mutating authoritative player rack state. Current production has no read-only API for the qualified V2 suggestion segments.

Manager review: `Docs/Handoffs/RACK-V2-04/MANAGER-REVIEW.md`.

## Base and routing

Start from current `main` descending from Rack V2 core integration `f23549b18b17b3542ec8f633d7309b11020990d4` and S1-C integration `169970f2d6b0268d53a4077e0e97934d290bf7eb`.

Recommended branch:

`feature/scenario-one-rack-v2-suggested-layout-api-v1`

Use a dedicated clean worktree. Refresh `Docs/Project/coordination/active-tasks.md` before edits.

Primary routing: local code-capable Game Dev lane, strongest reasoning-capable coding model available. This task can implement and run non-Unity/static checks while another task owns Unity. Do **not** acquire or bypass the machine-wide Unity lock while an older queued task is running.

## Goal

Expose the already-qualified Scenario One V2 suggested rack layout as deterministic, detached, read-only production data that RACK-V2-04 can consume.

Exit target:

`RACK V2 SUGGESTED-LAYOUT API QUALIFIED — READY FOR RACK-V2-04 RESUME`

## Required API contract

The exact type/method name is implementation choice; examples such as `ScenarioOneSuggestedRackLayout.Get(...)` or `GetSuggestedRackLayout()` are acceptable. The contract is binding:

1. In an applicable fresh/model-1 Rack V2 Scenario One world, return **exactly five** suggestion segments matching the qualified V2 headland layout.
2. Each result exposes at minimum:
   - entrance point;
   - far endpoint;
   - rack/corridor width;
   - deterministic order suitable for map presentation.
3. Geometry selection belongs to the Rack/headland/layout foundation, not `StandMapView` / `UI/Map/*`.
4. Do not expose mutable authoritative rack objects. Return detached immutable/read-only geometry values.
5. Reading the API must **not**:
   - allocate rack IDs;
   - call `AddRack`, `UpdateRack`, `DeleteRack`, `ConfirmRackPlan`, or entrance-marking methods;
   - add/change `RackPlan` / `ConfirmedRacks`;
   - change planned/marked counters;
   - create/reconcile/change work orders;
   - change plan confirmation;
   - mutate save data or world state.
6. Repeated reads on unchanged world state return identical geometry and leave authoritative state byte/logically unchanged.
7. Return no V2 suggestions for:
   - Legacy40;
   - Reference Future v1;
   - Rack planning model 0 / pre-V2 continuation.
8. Do not replace or reinterpret the historical `ScenarioOneRackLayout.Centres()` authored-layout contract. Legacy map/compatibility consumers remain unchanged unless a narrowly necessary call-site adjustment is explicitly justified in the handoff.

## Qualified geometry authority

The existing qualification evidence establishes the five V2 headland routes separately from the older authored layout:

- `Tools/Verification/RackV2Headland/RackV2HeadlandQualification.cs` qualifies entrance trees `PO0203`, `PO1204`, `PO2003`, `PO2803`, `PO3803` at the V2 headland;
- `Tools/Verification/RackV2Combined/RackV2CombinedVerification.cs::AddQualifiedHeadlandRacks()` constructs the qualified route from each entrance's live Z, `ScenarioOneHeadlandGeometry.HeadlandX`, and the stand far extent;
- the combined gate explicitly labels the older authored lines as legacy/informational only.

Production may adopt a bounded foundation representation of this qualified geometry. Do not copy these fixture IDs or hard-coded route coordinates into map UI code. If the cleanest production foundation needs to own the qualified entrance identifiers/derived values, document why and prove they match the qualified starting-world geometry. Do not invent a different five-rack pattern.

## Expected production surface

Prefer the smallest coherent foundation surface, for example:

- a new `Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs` (+ `.meta`), or
- a narrowly extended V2-specific foundation class if that is clearly cleaner.

Narrow use of:

- `ScenarioOneHeadlandGeometry.cs`;
- `ScenarioOneRackLayout.cs`;
- `RackGeometry.cs` / `RackPoint`;
- read-only manager/ecology identity properties;

is allowed only when necessary.

Do **not** edit Stand Map/UI files in this task. RACK-V2-04 owns the consumer UI after this API lands.

## Exclusions

Do not implement or change:

- `RACK-V2-04` map toolbar/suggested rendering itself;
- RACK-V2-05 editing interactions;
- spacing teaching/measurement;
- confirmation/reopen behaviour or OPEN-3;
- first-person authoring removal;
- field marking;
- Help/curriculum rewrite;
- rack persistence/schema/versioning;
- work-order lifecycle semantics;
- headland geometry dimensions;
- starting-tree generation/omission/replacement;
- five-rack target or the approved ~18 m Scenario One teaching intent;
- MFM, S1-C, S1-D, parity, ecology or economy;
- protected anchor/fingerprint rebaselines;
- Windows build.

## Verification

### Static/offline before Unity

Provide a focused check that proves, where possible without Unity:

- exactly five deterministic segments are represented for V2;
- result values are detached/read-only;
- repeated calls do not mutate supplied/authoritative collections;
- Legacy/model-0 applicability is rejected;
- no UI dependency is introduced;
- no call to rack mutation APIs is used by the getter.

### Focused Unity qualification

When the machine-wide Unity lock is available in normal queue order, run a bounded gate on the exact candidate HEAD:

1. Fresh V2 / Enlarged80 world: getter returns five segments.
2. Each returned entrance/far endpoint/width matches the already-qualified five headland routes used by the combined/headland gates, within the existing geometric tolerance; do not create a new target/rebaseline.
3. Snapshot before/after repeated reads proves unchanged:
   - rack count and IDs;
   - rack confirmation/marks;
   - work-order IDs/status;
   - planned/marked counters;
   - save/world state relevant to Rack V2.
4. Model-0/Legacy40 path returns no V2 suggestions and preserves historical behaviour.
5. Reference Future v1 path returns no V2 suggestions and frozen integrity remains exact if the fixture is exercised.
6. Compile/import clean.

No rendered captures are required for this API-only task. RACK-V2-04 owns rendered map review.

## G3 integrity rule

Do not weaken or rewrite an existing qualification target to make this API pass. If the qualified five-route geometry cannot be reproduced from a defensible production foundation without changing accepted headland/starting-world geometry, STOP `PRODUCT/GEOMETRY CONFLICT` and return to the Manager.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-04A/HANDOFF.md`

Include:

- worker/model/provider;
- branch/worktree/starting HEAD/final tested HEAD;
- files changed;
- API signature and ownership rationale;
- exact source of the five qualified routes;
- static/offline results;
- Unity lock receipt and focused gate result if run;
- before/after non-mutation evidence;
- model-0/Legacy40/reference compatibility evidence;
- any caveats;
- recommendation `APPROVE`, `NEEDS CHANGES`, or `REJECT` for 04A only.

At handoff, update the task claim to `[HANDED-OFF]`, link this handoff and close it.

## Exit

`RACK V2 SUGGESTED-LAYOUT API QUALIFIED — READY FOR RACK-V2-04 RESUME`

or a precise blocker.

No RACK-V2-04 UI completion, whole Objective-1 verification, demo readiness, MFM readiness or Windows-build readiness is implied.
