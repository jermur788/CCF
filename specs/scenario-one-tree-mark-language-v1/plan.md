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
3. Keep the persisted model untouched. Add only read-only presentation data for marked rack entrances to the existing map model/source if approved. Use orange Fell, orange-or-FR-4-fallback-pink rack and blue Crop in full-map glyphs and legend, matching qualified world shades; keep route lines and edit controls distinct. Keep minimap paint filtering explicit.
4. Deliver shape changes, duplicate-cue migration, work-state feedback and accurate copy as one qualified feature. Pending-work cues are derived from work orders; a visual transition must not restore consumed Fell proposal state. Preserve other MFM review/inspection/navigation overlays; audit collisions instead of rewriting them.
5. Restrict new visual policy to the approved Enlarged80 Rack V2 workflow; preserve the existing legacy branch without copying or changing save data. An overly broad shared-material edit could recolour legacy cues, so prefer a scoped runtime presentation choice. Manager must approve any necessary serialized material/scene edits separately.

The current near-orange pair has no demonstrated separation. FR-4 therefore requires >=3:1 measured paint/glyph interior luminance separation and two independent scorers each 100% correct under the fixed five-second protocol; failed orange rack/Fell checks trigger hot-pink rack across all surfaces and a complete rerun, then stop if pink fails. Exact shades are calibration, not sourced values. A field-painted ring and an overhead map circle are equivalent representations, not identical geometry. Whole-surface consistency is a contract; a new central service or saved visual-policy field is not required.

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

Follow spec migration steps 1–5, including RackEntrances.cs:175–176 “blue designation” / “red designation” → “Crop Tree designation” / “Fell designation”, plus colour-sensitive entrance prompts. Keep enum numbers, ID lists, rack entrance state, order provenance/history and schema/rule versions unchanged. New visual geometry is rebuilt from existing state after load. A help migration note needs no forest persistence and must remain discoverable despite existing local “seen” preferences. Existing saves are not recoloured by rewriting their content. No state/world hash change is intended or authorised; qualification must prove this under comparable configuration. Keep Legacy40 and Reference Future v1 unchanged.

## Verification strategy

- **Now, Tier 1 docs/static:** exact Git identity and scope, candidate source citations, primary-source retrieval/provenance, internal links/requirement mapping, `git diff --check`; no runtime claim.
- **After approval:** meaningful offline shape/legend/filter and read-only adapter/state tests, compiler/import gate, then bounded Tier-2 spec AC-1–AC-7 at both resolutions. Real input, old-save transitions and rendered shape recognition cannot be replaced by compilation or text assertions.
- **Independent conformance:** another provider/reviewer checks forestry attribution, current-source corrections, O1–O4 disposition and feature evidence. Worker output remains DRAFT/IMPLEMENTED until that review; the writer cannot approve itself.
- **Integration:** Manager assigns integration after required review and runtime gates; no protected test branch promotion and no Windows build from this packet.

## Risks

Shared orange removal shades may be confused; old overhead cues may obscure paint; map legend/swatch renderer may hardcode rings; consumed proposals may disappear or falsely persist; marked-entrance projection may accidentally be inferred from confirmation; existing first-use preferences may hide migration copy; a shared material change may affect legacy rendering. Each needs scoped inspection/evidence before coding. Performance budget and human colour-vision readability remain unmeasured; recognition passes only by the FR-4 answer-key protocol. The source’s quality ring inspires the game Crop label without defining it.

## Implementation sequence (not released)

1. T-01: Manager approval, narrow authority reconciliation and fresh source/ownership audit.
2. T-02: scoped paint and work-state cue transition.
3. T-03: read-only map projection, legend and atomic help/curriculum alignment.
4. T-04: offline and later authorised Unity/old-save/render/input/performance evidence.
5. T-05: independent review and Manager disposition/integration packet.

## Review response

**DRAFT revision for PR #266; no production or Unity change, no approval of O1–O4.**

| Finding | Disposition / change |
|---|---|
| 1 — MAJOR: near-orange rack/Fell separation and missing fallback | FIXED in draft: FR-4 requires >=3:1 measured interior luminance separation at every required view, including thin textured trunks at 15 m, independently of stripe direction; complete-matrix pink fallback defined. Measurements remain UNMEASURED. |
| 2 — map/legend colours unspecified | FIXED: explicitly orange Fell glyph/legend (replacing current red), orange rack glyph/legend or selected fallback pink, and blue Crop glyph/legend; each matches its qualified world colour. |
| 3 — in-world designation prompts omitted | FIXED: migration inventory includes `ForestTreeMarkingManager.RackEntrances.cs:175–176` blue/red designation prompts → Crop Tree/Fell designation; also audit colour-sensitive entrance prompts. Rejection behaviour stays unchanged. |
| 4 — recognisable depended on reviewer verdict | FIXED: FR-4 sets five-second, label-hidden, answer-key scoring by two independent people, each 100% correct per resolution/mode/distance; unsure/late/missing fails, raw answers/timings retained. AC-1/2/4 reference it; other AC rows retain observable state assertions and AC-7 requires an approved numeric budget. |
| 5 — source quality ring conflated with Crop Tree | FIXED: source blue/white ring denotes high timber quality; “Crop Tree — retain/favour” remains an explicit game calibration label inspired by that practice, not its definition. |

**New fallback rule:** if orange rack versus orange Fell fails either >=3:1 luminance separation or scored recognition in any required view, switch rack paint/glyph/legend/help swatch/colour wording atomically to luminance-adjusted hot pink, keep Fell orange, and rerun the full matrix; stop for redesign if pink fails.

Source claims remain attributed practice, with no adoption of the publisher’s “widely applied in private and state forests” claim. No enum/schema/save field, loaded meaning, gameplay or current marker colour changes; 3D/full-map/minimap/help/curriculum remain covered. This resolves draft wording defects only, pending independent re-review and O1–O4.
