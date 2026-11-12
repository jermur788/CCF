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

The current near-orange pair has ΔE76 6.0042, below the proposed normal-colour floor of 40. FR-4 therefore conditionally selects exact pink sRGB (1, 0.08, 0.58), ΔE76 78.0777 versus Fell; its 1.1579:1 pair contrast is diagnostic only. Require >=3:1 against backgrounds plus mandatory shapes and two non-author scorers each 100% correct within five seconds. Correct background visibility separately; failed pink recognition requires documented shape/size/edge redesign and a fresh complete matrix, returning to Manager/user for any weakened gate or scope change. Exact shades and floors are game calibration, not sourced values. A field-painted ring and an overhead map circle are equivalent representations, not identical geometry. Whole-surface consistency is a contract; a new central service or saved visual-policy field is not required.

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

## Review response — round 2 (Option A)

**DRAFT; same PR #266. Finding 1 was not fixed by the previous revision:** its mark-versus-mark >=3:1 rule was infeasible for the intended hot hues. This round replaces that rule; no implementation, Unity run or O1–O4 approval is claimed.

**Choice and trade-off:** Option A separates perceptual colour difference from background legibility. Normal-colour rack/Fell interiors require **CIE76 ΔE*ab >= 40**, plus mandatory shape redundancy. This is a deliberate game calibration floor, not an accessibility standard or guarantee. Grayscale/colour-vision modes rely on the scored shape-recognition gate, not this normal-colour floor. Contrast **>=3:1** applies to each mark against bark/map/legend/help backgrounds, with a measurable edge/backing allowed. No >=3:1 requirement remains between rack and Fell.

### Arithmetic independently recomputed for this round

Inputs are **encoded sRGB [0,1]**, not linear-light Unity shader values. The exact fallback is **(1.000000, 0.080000, 0.580000)**; do not quantise to an 8-bit approximation for this reference computation. Fell remains **(1.000000, 0.350000, 0.050000)**.

| Colour | Encoded sRGB | Relative luminance L | CIELAB D65 (L*, a*, b*) | Contrast versus Fell (diagnostic only) | ΔE76 versus Fell |
|---|---|---:|---|---:|---:|
| Fell reference | (1, 0.35, 0.05) | 0.284749 | (60.3214, 60.4653, 67.9663) | 1.0000:1 | 0.0000 |
| Current rack orange | (0.95, 0.28, 0.025) | 0.234930 | (55.5828, 63.0698, 65.3562) | 1.1748:1 | 6.0042 — FAIL <40 |
| Exact pink fallback | (1, 0.08, 0.58) | 0.239095 | (56.0036, 84.5737, -6.1705) | 1.1579:1 | 78.0777 — PASS >=40 |

Reproduction: linearise each channel `c` as `c/12.92` if `c <= 0.04045`, otherwise `((c+0.055)/1.055)^2.4`. Contrast uses `L=0.2126r+0.7152g+0.0722b` and `(max(L1,L2)+0.05)/(min(L1,L2)+0.05)`. For Lab use linear-sRGB→XYZ rows `(0.4124564,0.3575761,0.1804375)`, `(0.2126729,0.7151522,0.0721750)`, `(0.0193339,0.1191920,0.9503041)`, D65 white `(0.95047,1,1.08883)`, `f(t)=cuberoot(t)` above `(6/29)^3`, otherwise `t/[3(6/29)^2]+4/29`; Lab is `(116f(Y)-16,500[f(X)-f(Y)],200[f(Y)-f(Z)])` using white-normalised XYZ. ΔE76 is Euclidean Lab distance. Compute at full precision, round only for display.

For example, pink/Fell contrast is `(0.2847485507+0.05)/(0.2390949674+0.05)=1.1579`, while ΔE76 is `sqrt((-4.3178)^2+24.1085^2+(-74.1368)^2)=78.0777`. Against Fell, 3:1 would require `L >= 3(0.2847485507+0.05)-0.05 = 0.954246` or `L <= (0.2847485507+0.05)/3-0.05 = 0.061583`. This explains the discarded rule. Background contrast is feasible: against black (`L=0`), Fell gives **6.6950:1**, pink **5.7819:1**; these examples do not prove rendered bark/background performance.

### Disposition and non-circular failure action

- **Finding 1:** replaced the infeasible pair-contrast gate with normal-colour ΔE76 >=40, separate >=3:1 background contrast, mandatory distinct shapes and binding recognition. Current orange fails the numeric floor now, so the exact pink is the candidate for future qualification. Its numeric pass does not qualify renders.
- **FR-1 / fallback:** pink is conditional on orange failing the perceptual floor or recognition. Switch rack paint, full-map glyph, legend, help swatch and colour wording together; retain orange Fell. Background-only failure first requires edge/backing/visibility correction, not an automatic hue switch.
- **Recognition:** two non-author people independently score every required capture within 5 seconds; each must score 100% in every mode/resolution/required distance. Unsure, missing or late = fail. Orange failure triggers the specified pink candidate once. Pink recognition failure requires a documented shape/size/edge/occlusion redesign and complete fresh matrix with new random order; unchanged reruns cannot erase a failure. If passing needs weaker thresholds, changed meanings or scope expansion, stop and return to Manager/user. No return to orange merely because pink's diagnostic pair contrast is below 3:1.
- **Accepted findings 2–5 preserved:** map/legend follow fallback; blue/red designation prompts are replaced with Crop Tree/Fell designation; two non-author scorers retain 5 s/100%/unsure-or-late-fails; Crop Tree remains a game label inspired by the source's high-quality timber ring.

Source-reported practice stays attributed guidance; the publisher's “widely applied” claim is not adopted. No enum/schema/save field, code, Unity asset or saved meaning changes. Only the four spec files and handoff change. Static arithmetic is computed above; rendered contrast, recognition, input and performance remain UNVERIFIED/UNMEASURED. Independent re-review and O1–O4 approval remain outstanding.
