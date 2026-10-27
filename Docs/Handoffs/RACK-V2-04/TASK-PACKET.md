# TASK PACKET — RACK-V2-04 Map Planning Mode

**Feature:** Scenario One Objective 1 Rack Planning & Field Marking V2 (D-064)  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — APPROVED  
**Task:** `RACK-V2-04`  
**Class:** approved-spec implementation / bounded UI integration  
**Status:** READY / DISPATCHED  
**Manager:** ChatGPT Game Dev / Overall Manager

## Base and prerequisites

Start from current `main` descending from the canonical Rack V2 core integration merge:

`f23549b18b17b3542ec8f633d7309b11020990d4`

Packet-authoring main was `78b82e407410c658c4d1bbf66600382460942fa8`; later management/docs-only descendants are acceptable after preflight.

Rack V2 core/foundation is integrated and qualified. Exact qualified production repair HEAD was `7b557fd43ca9468a1be22c161c24064eec9f955a`. This task must consume the integrated authoritative rack model; do not recreate rack state or counters in UI.

## Routing

Primary: local code-capable implementation lane with later Unity/rendered verification. The UI work is suitable for a strong visual/UI coding model, but local Unity remains authoritative for qualification.

Recommended branch:

`feature/scenario-one-rack-v2-map-planning-mode-v1`

Use a dedicated clean worktree. Preserve unrelated dirty work.

## Goal

Implement only RACK-V2-04 from the approved task list:

> Make `Plan Racks` a tool/mode control, visually separate from navigation tabs, and show the suggested rack layer.

Exit target:

`MAP PLANNING MODE READY`

## Required behaviour

### Plan Racks control

- Add `Plan Racks` as a map **tool/mode control**, not another navigation tab.
- Inactive label/state: `Plan Racks`.
- Active state: `Planning Racks — Active` or an equivalent unmistakable active presentation consistent with the approved UI system.
- Keep existing map navigation tabs behaving as navigation.
- Do not introduce a separate map-editor theme.

### Suggested rack layer

On first entry to rack-planning mode in a fresh applicable V2 world:

- show the approved suggested five-rack layout;
- suggested racks are guidance only and do **not** count as player planned/confirmed racks;
- suggested geometry must come from the qualified Rack V2/headland/layout foundations rather than a duplicated hard-coded UI-only rack model;
- render suggested racks faint/dashed and clearly distinguish them from authoritative player racks;
- legend language must include the approved conceptual distinction:
  - `Dashed = suggested`
  - `Solid = your planned rack`

If current foundations do not expose a defensible suggested-layout API without duplicating authoritative rack geometry, STOP with `CROSS-LANE API REQUIRED` and identify the narrow API needed.

## Scope / ownership

Expected primary surface:

- `Assets/ForestPrototype/UI/StandMapView.cs` and/or existing `UI/Map/*` components that own Stand Map toolbar/mode/legend presentation;
- narrow Rack V2 read-only API consumption if already present;
- focused verification under a new/appropriate Rack V2 UI verification location;
- this task's handoff/evidence docs.

Before editing, refresh `Docs/Project/coordination/active-tasks.md` and confirm no active writer owns the exact files you need. S1-C T-02 owns `AnnualReviewView.cs` and narrow `ScenarioOneUiRoot.cs`; do not expand into that surface unless Manager explicitly reconciles ownership.

## Exclusions

Do **not** implement:

- rack create/select/move/rotate/delete interactions (RACK-V2-05);
- spacing measurement/teaching (RACK-V2-06);
- Confirm Rack Plan or reopening rules (RACK-V2-07 / OPEN-3);
- minimap propagation beyond any strictly necessary unchanged-map plumbing (RACK-V2-07);
- removal of first-person authoring (RACK-V2-08);
- 3D waypoint/orange field marking (RACK-V2-09);
- Help/teaching rewrite (RACK-V2-10);
- MFM proposal/coverage UX;
- S1-C/S1-D/parity/economy/ecology changes;
- save/schema changes;
- Windows build.

OPEN-3 remains unresolved and must not be touched.

## Verification

### Before Unity

- prove the control is structurally separate from navigation-tab selection;
- prove suggested racks cannot mutate authoritative rack state merely by being displayed;
- prove no duplicate persisted/planned counter or rack collection is introduced;
- inspect any harness changes before running them.

### Unity / rendered

This task needs a Tier-1 functional smoke plus rendered evidence at both supported resolutions once the machine-wide Unity lock is available.

S1-C-IMPL-02 is the older queued Unity task and retains priority. Do not bypass it. Implementation/static work may proceed in a separate clean lane, but wait for the normal lock before Unity execution.

Capture at minimum at 1280×720 and 1920×1080:

1. Stand Map before planning mode;
2. planning mode active;
3. first-entry suggested rack layer + legend;
4. an existing authoritative rack state if a non-destructive fixture can display one, proving suggested/player visual distinction.

Check:

- control reads visually as a tool/mode, not a tab;
- active state is unmistakable;
- dashed suggestions are legible but not mistaken for completed/player racks;
- existing map/navigation content remains usable;
- no compiler/import errors.

Do not claim RACK-V2-05 editing or whole Objective 1 playability from this gate.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-04/HANDOFF.md`

Include:

- exact starting main SHA, branch/worktree and final tested HEAD;
- files changed;
- authoritative suggested-layout source/API used;
- proof suggested display does not mutate/count as player racks;
- toolbar/tab separation implementation;
- commands/gates and results;
- Unity lock record;
- 1280×720 and 1920×1080 capture locations/review;
- failures/caveats;
- recommendation `APPROVE`, `NEEDS CHANGES`, or `REJECT` for RACK-V2-04 only.

## Exit

`MAP PLANNING MODE READY — READY FOR MANAGER REVIEW`

or a precise blocker.

No whole Rack V2 feature verification, user acceptance, external-demo readiness, MFM readiness or Windows-build readiness is implied by this task.