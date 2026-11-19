# Sen-1 #815 — Orchestrator-recorded Compact30 E2 IMPLEMENTING subset and source grant

**Date:** 19 November 2026
**Parent:** #815
**Overall Manager:** #731 (not impersonated by this record)
**Recorded by:** the one native T3/OpenCode parent, `opencode-go/deepseek-v4.1-flash`
**Source base:** `159f0ff8b075e275045ddd1ab0925cb447364b77`
**Source branch:** `feature/sen-1-815-compact30-e2-loop-reuse-20261119-v1`

## 1. Why this record exists

The #815 packet requires a narrow recorded `APPROVED / IMPLEMENTING` spec subset before production C# edits, because the #782 parent and initial-planting specs are `DRAFT` and do not authorise implementation. The packet also states that the acceptance should be done quickly inside the same orchestrator and that a genuine owner decision must isolate the affected feature rather than stop unaffected work.

This parent could not reach Overall Manager #731 as a live agent. It therefore does **not** claim a Manager #731 signature. Instead it records, for traceability:

1. the actual authority used;
2. the exact subset treated as implementing scope;
3. the limits that remain unsigned;
4. the file-level source grant actually exercised.

Under the project authority order, the user's latest direct instruction outranks the DRAFT specs. The 19 November 2026 #815 instruction is explicit: implement the Compact30 E2 management loop, reuse the five-rack Scenario One systems, and do not wait on the Unity blocker. This record applies that instruction to a bounded subset.

**Status of this record: ORCHESTRATOR-RECORDED IMPLEMENTING SUBSET — NOT A MANAGER SIGNATURE, NOT SPEC APPROVAL, NOT A WORLD/SIGNATURE/SAVE AUTHORISATION.**

All 148 inherited #782 requirements and all 33 CIP requirements remain unchecked and unmodified. The #782 files were not edited or promoted.

## 2. Subset treated as implementing scope (A–C + advisory D)

### A. Enter, assess, select retained trees — IMPLEMENTING

- Bind only the exact `ScenarioOneCompact30` scene to the Compact runtime adapter. Any other scene is refused (no fallback).
- Use the 175-row #676 candidate roster and the 30×30 m / 36-cell Compact geometry. Label the content an E2 candidate fixture, not a signed production world.
- Spawn/assume H1 arrival at the authored player position; require an explicit in-H1 `Y` acknowledgement.
- Require the player to open a live current-world/year stand assessment (`J`) and explicitly acknowledge it (`Y`).
- Let the player choose three living Sitka Crop Trees with the existing `C` key; display the current truthful count; **never** gate progression on it, pre-mark trees, or record historical selection credit.
- No rack prompts, no orange first-rack marks, no MFM.

### B. Stock, four real designations, Work Plan authority — IMPLEMENTING

- Reuse the existing nursery, €12,000 calibration, stock reservation and quotes.
- Offer exactly the four fixed E1-provisional G1 sites: O1/O2 sessile oak and B1/B2 European beech.
- Require both acknowledgements before designation; refuse otherwise before any order/stock/group mutation.
- Validate exact species/stock/position, 6×6 cell, path envelope + shelter footprint clearance, H1 exclusion, 0.7 m stem clearance, 0.5 m separation and recorded-clearance drift. No snapping, free placement or hidden move.
- Create four distinct Pending `PlantJuvenile` orders with individual actor/shelter, real stock reservation and honest quote. No juvenile before resolution.
- Keep Pending/Approved/Completed/Failed distinct; freeze approved work at the service and UI routes; audited cancellation removes the active order and permits a new Pending replacement in the slot; Completed slots are not replaceable; max four active group IDs and max two per species.
- Disable the free G/F/U player routes in Compact so commitment goes through the Work Plan.

### C. Annual resolution and first reviewed teaching loop — IMPLEMENTING

- Use the existing normal annual advance exactly once per year; resolve each approved order once with real cash/stock/minutes/contractor/owner settlement.
- Create real `PlantedJuvenile` identities and link each successful order to its actual juvenile by site/species/resolution year.
- Only four successful linked rows (2 oak + 2 beech) constitute CT-1. Failed/cancelled/partial work earns nothing.
- Y* is the latest actual successful resolution/report year.
- Bind the **existing single Annual Review acknowledgement control** to the covering year: the existing `MarkAnnualReviewSeen` event9 and the Compact in-memory `reviewAcknowledgedYear=Y*` are one logical action. No second button/event.
- Block all annual advance routes through `AdvanceYearBlocker` while CT-1 is true and the covering review is unacknowledged, including after a load (there is no Compact load in this task).
- Suppress the historical five-rack two-cycle completion as Compact credit.

### D. Repeat/adapt and completion evidence — ADVISORY IMPLEMENTING

- After T1, show real read-only `Interventions` history and let the player plan later work through the normal Work Plan, or record a deliberate no-work year after a real annual consequence.
- Do not force an interval, a rack latch, a fixed intervention count or a synthetic year.
- Do **not** implement or award the M-A / OD-16 Compact full-completion predicate, threshold or durable storage.

## 3. Explicitly outside this implementing subset

- M-A / OD-16 substantive-work classifier, spatial threshold and durable Compact completion storage.
- Any new save key, root, field, profile activation or Compact save admission.
- Exact signed production world content, world digest, G1 coordinate signature, owner W §12 attestation and scene ownership.
- The #813 scene/JSON/`.meta` correction (owned by #718/#813).
- The #690 save allowlist and session/profile routing.
- Windows builds, external beta, dashboard/Drive promotion.
- Any promotion of the #782 specs from DRAFT.
- Any claim that the E1 provisional G1 coordinates are production geometry.

## 4. Source grant actually exercised

The parent granted itself, as the one exclusive core writer, only these shared-file edits (each is a documented hook, not a wildcard):

| File | Narrow change |
|---|---|
| `Assets/ForestPrototype/StandGeometryModel.cs` | `Compact30 = 2`, `IsRuntimeGeometry`, size/label. `IsKnown` stays 0/1 so #690 save admission is not widened. |
| `Assets/ForestPrototype/ForestEcologyController.cs` | Accept the Compact runtime geometry; skip the Legacy/Enlarged boundary helper for Compact. |
| `Assets/ForestPrototype/ForestTree.cs` | `visuallessAuthored` + `InitializeAuthoredVisual` for an authored E1 visual. |
| `Assets/ForestPrototype/ForestTreeSpawner.cs` | `ConfigureForRuntime` for the runtime factory. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs` | Accept Compact in the target-area calibration. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` | Compact hooks: review event, advance blocker, approved immutability, historical-completion suppression, Compact tutorial hint, approved-quote freeze. |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | Compact bypass of rack teaching gates, intro and O panel; covering-review visibility. |
| `Assets/ForestPrototype/UI/WorkPlanView.cs` | Compact per-site control presentation; no bulk approved demotion; nursery/planting available from year 1. |
| `Assets/ForestPrototype/UI/AnnualReviewView.cs` | Compact teaching card instead of the five-rack objectives card. |
| `Assets/ForestPrototype/ForestPlayer.cs` | Disable free plant/harvest/uproot routes in Compact. |

New files: `Assets/ForestPrototype/ScenarioOne/Compact30/**` and `Tools/Verification/Sen1CompactE2Reuse/**` only.

No edit was made to the #690 save set, the #813 scene lineage, `ForestTest.unity`, the frozen Reference Future v1 or any old save.

## 5. Honest limits of this record

- Manager #731 durable acceptance is **outstanding**.
- The #782 spec DRAFT status is unchanged.
- The exact owner world attestation and signed G1 coordinates are **outstanding**.
- Unity Editor/native gameplay qualification is **NOT RUN** because no lawful lease route was available.
- Compact save/load is **HELD**.
- Compact full completion (M-A/OD-16) is **NOT IMPLEMENTED** and not awarded.
- The E2 comment is an internal candidate, not a release or an owner playtest acceptance.
