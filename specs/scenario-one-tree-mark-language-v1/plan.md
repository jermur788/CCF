# Scenario One Tree Marking Language V1 Draft Plan

- **Spec ID:** SPEC-S1-TREE-MARK-LANGUAGE-V1
- **Spec / plan status:** DRAFT — PLANNING ONLY, NO PRODUCTION RELEASE
- **Owner:** Overall Manager
- **Created / updated:** 2026-11-04
- **Authority:** [spec.md](spec.md); implementation requires approval and resolved O1–O4.

## Architecture / current-state findings

`ForestTreeMarkingManager` builds both loose Fell/Crop cues. Its rack-entrance partial builds four separate orange paint patches from persisted rack state. Rack orange is not a third `TreeMarkType`; keep that separation. `StandMapSource` reads the live state; `StandMapDrawPlan`/`StandMapSymbols` supply both map presentations. Individual mark rings are currently full-map-only. The current map model has no entrance-painted datum. `MenuHelpView.Text.cs` and `LearningObjectivesView.Curriculum.cs` contain divergent visual descriptions. See [handoff source audit](../../Docs/Handoffs/Overnight-Continuation-20261104/MARKING-SYSTEM-SPEC-HANDOFF.md) for pinned file/line evidence.

The current source-defined Fell shade is orange-toned even though help says red. Do not implement from the issue summary alone. No rendered appearance, input path or performance was measured for this task.

## Proposed technical approach and trade-offs

1. Resolve O1–O4, reconcile the narrow accepted-spec/Decision Log visual amendment through Manager-owned files, then pin a fresh implementation candidate.
2. Reuse existing focused marking components and URP materials. Adapt trunk geometry to vertical patches, diagonal patches and continuous Crop band, with supporting contrast where needed. Prefer runtime-generated geometry already used by rack paint; no texture family/package/framework is justified. Actual mesh counts/dimensions await evidence, not fabricated optimisation.
3. Keep the persisted model untouched. Add only read-only presentation data for marked rack entrances to the existing map model/source if approved. Match full-map shape icons/legend to field paint; keep route lines and edit controls distinct. Keep minimap paint filtering explicit.
4. Deliver shape changes, duplicate-cue migration, work-state feedback and accurate copy as one qualified feature. Pending-work cues are derived from work orders; a visual transition must not restore consumed Fell proposal state. Preserve other MFM review/inspection/navigation overlays; audit collisions instead of rewriting them.
5. Restrict new visual policy to the approved Enlarged80 Rack V2 workflow; preserve the existing legacy branch without copying or changing save data. An overly broad shared-material edit could recolour legacy cues, so prefer a scoped runtime presentation choice. Manager must approve any necessary serialized material/scene edits separately.

Two orange meanings depend on shape; this follows the source's removal family but raises a legibility risk compared with colour-separated marks. The proposal therefore requires distinct vertical/diagonal silhouettes, labels and direct accessibility review. A field-painted ring and an overhead map circle are equivalent representations, not identical geometry. Whole-surface consistency is a contract; a new central service or saved visual-policy field is not required.

## Integration boundaries / affected files

**This task writes only the four new spec files and its assigned handoff.** The following is an inspection-derived future candidate list, not ownership or permission:

- `Assets/ForestPrototype/ForestTreeMarkingManager.cs` and `.RackEntrances.cs` — scoped runtime paint/cue display.
- `Assets/ForestPrototype/Racks/PlannedFellMarkers.cs` — consumed proposal / work-order transition display.
- `Assets/ForestPrototype/UI/Map/StandMapModel.cs`, `StandMapSource.cs`, `StandMapDrawPlan.cs`, `StandMapSymbols.cs`, `StandMapLegend.cs` and any directly required legend renderer — read-only glyph/state projection.
- `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`, `LearningObjectivesView.Curriculum.cs` and directly affected inspection/HUD prompts — matching words and help swatches; inspect actual widgets before assigning changes.
- Focused existing verification entry points and new bounded feature checks only after harness side-effect inspection.

No save classes, ecology/calibration/RNG, work executor, route mask, package, ProjectSettings, frozen archive, scene or prefab edits are assumed. No new external image/font/binary assets. Preserve all `.meta`/GUID relationships. Do not hand-edit Unity YAML to achieve a quick colour change.

## Ownership / concurrency

#200 owns active coordination; DPC owns the routing ledger. #255 currently owns two copy files, #256 owns StandMapView; do not take their files or cherry-pick into their lanes. Future code ownership must be freshly assigned after those changes are inspected. One writer for any serialized asset. Unity must remain off tonight; future execution needs a new authorised exact candidate/lane, current disk/process checks and exclusive lease. A past free receipt is insufficient.

## Migration / save implications

Follow spec migration steps 1–5. Keep enum numbers, ID lists, rack entrance state, order provenance/history and schema/rule versions unchanged. New visual geometry is rebuilt from existing state after load. A help migration note needs no forest persistence and must remain discoverable despite existing local “seen” preferences. Existing saves are not recoloured by rewriting their content. No state/world hash change is intended or authorised; qualification must prove this under comparable configuration. Keep Legacy40 and Reference Future v1 unchanged.

## Verification strategy

- **Now, Tier 1 docs/static:** exact Git identity and scope, candidate source citations, primary-source retrieval/provenance, internal links/requirement mapping, `git diff --check`; no runtime claim.
- **After approval:** meaningful offline shape/legend/filter and read-only adapter/state tests, compiler/import gate, then bounded Tier-2 spec AC-1–AC-7 at both resolutions. Real input, old-save transitions and rendered shape recognition cannot be replaced by compilation or text assertions.
- **Independent conformance:** another provider/reviewer checks forestry attribution, current-source corrections, O1–O4 disposition and feature evidence. Worker output remains DRAFT/IMPLEMENTED until that review; the writer cannot approve itself.
- **Integration:** Manager assigns integration after required review and runtime gates; no protected test branch promotion and no Windows build from this packet.

## Risks

Shared orange removal shades may be confused; old overhead cues may obscure paint; map legend/swatch renderer may hardcode rings; consumed proposals may disappear or falsely persist; marked-entrance projection may accidentally be inferred from confirmation; existing first-use preferences may hide migration copy; a shared material change may affect legacy rendering. Each needs scoped inspection/evidence before coding. Performance budget and human colour-vision readability remain unmeasured.

## Implementation sequence (not released)

1. T-01: Manager approval, narrow authority reconciliation and fresh source/ownership audit.
2. T-02: scoped paint and work-state cue transition.
3. T-03: read-only map projection, legend and atomic help/curriculum alignment.
4. T-04: offline and later authorised Unity/old-save/render/input/performance evidence.
5. T-05: independent review and Manager disposition/integration packet.
