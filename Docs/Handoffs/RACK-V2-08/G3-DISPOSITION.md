# RACK-V2-08 — Manager G3 disposition

**Verdict:** `G3 PASS — SPLIT LEGACY COVERAGE FROM V2 ABSENCE / MAP REPLACEMENT COVERAGE`

**Date:** 2026-10-28

**Applies to:** claim #49, `Docs/Handoffs/RACK-V2-08/TASK-PACKET.md`, branch `feature/scenario-one-rack-v2-no-competing-author-v1`.

## Decision

The protected assertions identified in the blocker are not to be deleted, skipped, weakened, or silently rebaselined. They encode a valid **model-0 / Legacy40 first-person rack-authoring contract**, but that contract is superseded for **Rack Planning V2** (`RackPlanningV2Active`: rack model 1 + Enlarged80).

RACK-V2-08 may therefore change the protected harnesses only to make the model boundary explicit:

1. **Model-0 / Legacy40 coverage is retained.** The existing first-person `B` / origin-selection / preview / confirmation / ordinary-work assertions remain compatibility coverage and must execute under an explicit model-0/Legacy40 fixture. Do not rely on `CCF_STAND_GEOMETRY=1` alone to mean legacy; a fresh Enlarged80 world selects Rack V2.
2. **Rack V2 gets replacement coverage.** Under `RackPlanningV2Active`, first-person rack authoring must be absent/inert and the Stand Map must be the immediately available replacement authoring surface.
3. The sequential/guided gates remain gates. They are not retired merely because their V2 branch changes.
4. No assertion unrelated to the superseded authoring route is authorised for weakening or expected-value movement. Any further protected conflict returns to Manager G3 review.

This is an intentional product-contract split, not a waiver.

## GuidedOnboardingVerification disposition

For `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs`:

### Legacy/model-0 branch

Retain the existing real-input first-person sequence and its substantive checks, including:

- `B` enters the rack tool;
- origin selection / reselection / cancellation are exercised;
- pointer movement changes the preview;
- confirmation succeeds through the legacy route;
- ordinary pending rack work is created;
- subsequent legacy progression remains viable.

Run this branch under an explicit model-0/Legacy40-compatible fixture so the compatibility assertions are actually exercised.

### Rack V2 branch

The current `ConfirmedCount > 0` wait and the first-person confirmation assertions are superseded. Replace them with comparably strong V2 assertions that prove, using real/runtime input where the gate previously required real input:

- `RackPlanningV2Active` is true for the fixture;
- pressing `B` and any deferred/intro entry route does not activate the first-person `RackTool`;
- `RackTool.AnyActive` remains false and cannot suppress ordinary tree interaction;
- direct first-person confirmation cannot create a rack, mutate the authoritative `RackPlan`, increment V2 rack counters, or create rack-removal work;
- V2 player-facing guidance does not instruct `[B] Create rack` and instead directs the player to the Stand Map / `Plan Racks` flow;
- the Stand Map `Plan Racks` replacement is available without first creating a forest rack;
- progression after rack planning uses the existing authoritative V2 plan/confirm flow, not `manager.ConfirmRack(..., "forest")` and not fabricated credits.

The V2 branch does not need to duplicate every RACK-V2-05/06/07 interaction assertion, but it must prove the removed route is absent and the replacement route is reachable and authoritative.

## CombinedHudComposition disposition

For `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs`:

### Legacy/model-0 branch

Retain the existing premise and checks:

- the map may wait for the first forest-authored rack where that is the legacy design;
- legacy `ConfirmRack` may be used to establish that state;
- the existing HUD/minimap/focus assertions remain compatibility coverage.

### Rack V2 branch

The assertions that `Plan rack` waits for the first forest rack and that a legacy `ConfirmRack(..., "forest")` must unlock the map are superseded. Replace them with V2 checks that:

- `Plan Racks` is offered immediately in the Stand Map for the V2 Objective-1 flow;
- no first-person rack must be created to expose that control;
- opening/closing the Stand Map preserves focus/pointer/HUD composition invariants already owned by this gate;
- any rack state needed later in the V2 composition path is established through the authoritative Rack V2 plan/map APIs or the existing confirmed V2 state, never through the legacy forest-authoring backend;
- minimap/full-map rack presentation continues to satisfy the already integrated RACK-V2-07 contract.

Do not delete the legacy branch; make the model boundary explicit.

## Production implementation boundary

The blocker inventory's proposed production approach is accepted as the intended narrow implementation direction, subject to verification:

- guard first-person `RackTool` direct and deferred entry/confirmation by `RackPlanningV2Active` so V2 cannot enter or confirm through the legacy forest author;
- substitute V2-specific Stand Map guidance at render/presentation time;
- preserve stable curriculum ids/titles where practical rather than globally rewriting legacy lesson data;
- preserve model-0/Legacy40 first-person authoring;
- preserve ordinary Fell/Red, Crop Tree/Blue, inspection, later entrance-mark interaction, Work Plan semantics, Rack V2 map editing/confirmation, and all existing authoritative rack state;
- no save/schema, scene/prefab/material, ecology/economy, MFM, or Windows-build expansion.

## G3 verification required before handoff

RACK-V2-08 qualification must demonstrate both sides of the split:

1. **V2 absence:** attempt every former first-person entry/confirmation route; no RackTool activation, rack-plan mutation, counter movement, or work-order creation.
2. **V2 replacement:** Stand Map `Plan Racks` is available immediately and existing map create/edit/confirm behavior remains functional.
3. **V2 copy:** rendered/current guidance contains no obsolete first-person `B / Create rack` instruction in the scoped Objective-1 surfaces.
4. **Ordinary marking preserved:** Fell/Crop marking and inspection remain usable in V2.
5. **RACK-V2-07 preservation:** confirmation/reopen/minimap contract still passes.
6. **Legacy/model-0 compatibility:** explicitly run the retained first-person assertions under a model-0/Legacy40-compatible fixture and prove viable progression.
7. **Protected-harness integrity:** the Guided/Sequential and Combined HUD gates remain active, with the new model split; no unrelated assertion is removed or weakened.

If the only way to get green is to suppress a gate, skip a model branch, weaken unrelated assertions, or fabricate progress, stop and return to Manager G3 review.

## Coordination / resume

Claim #49 remains the same substantive task; do not create a new claim. Resume in the existing branch/worktree with one writer. Before production edits:

- refresh current `main` and live `Docs/Project/coordination/active-tasks.md`;
- ensure current `main` descends from the task base and account for docs-only OPEN-3 sync PR #51 (`c15df9f916f4dab5fca66ad8348a620370841911`);
- update live coordination only under the existing bounded amendment so RACK-V2-07 is recorded integrated and RACK-V2-08 is recorded as RUNNING / G3 CLEARED; preserve unrelated rows byte-for-byte;
- mark claim #49 `[RUNNING]` again with the continuing executor/watcher, same branch/worktree and resume HEAD.

Before Unity, refresh live coordination and acquire the machine-wide lock normally. MFM-03 #39 remains independent; never bypass an actual lock owner.

Success remains:

`NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`
