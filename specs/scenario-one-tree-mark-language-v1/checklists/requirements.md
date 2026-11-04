# Scenario One Tree Marking Language V1 Requirements Checklist

- **Spec ID:** SPEC-S1-TREE-MARK-LANGUAGE-V1
- **Spec status:** DRAFT — not approved / not implemented
- **Checklist owner:** Overall Manager / future independent reviewer
- **Implementation under test:** none; static research baseline `72b4efbd1c653fa2231f632a79030a83ff76f833`
- **Date:** 2026-11-04

All boxes intentionally unchecked: this is a future implementation acceptance checklist, not the author's self-approval. Docs/static checks are reported separately in the handoff.

## Functional requirements

- [ ] FR-1: rack paint remains four-sided vertical orange or FR-4 fallback pink; actual marked entrance, route geometry and edit control distinguished.
- [ ] FR-2: individual Fell displays diagonal orange slashes and matching full-map strokes; same removal meaning.
- [ ] FR-3: Crop displays blue continuous trunk ring / map circle with retain/favour game label explicitly distinguished from the source’s high-timber-quality meaning.
- [ ] FR-4: distinct shapes and text survive both resolutions, light/shade and colour-vision/grayscale review; >=3:1 interior luminance and both scorers 100% correct per FR-4, required distances and human feedback recorded; failed orange pair triggers pink rack and complete rerun, with stop on fallback failure.
- [ ] FR-5: 3D/map/legend/help/curriculum match; marked entrance comes from saved rack state; minimap omissions accurately disclosed; full-map/legend explicitly orange Fell, orange-or-fallback-pink rack and blue Crop, matching world shades.
- [ ] FR-6: exactly three introductory meanings; route-side retention/deadwood/bio marks excluded.
- [ ] FR-7: requested heading/intro, accurate source-informed game-convention wording and entrance-only simplification; no official-compliance assertion.
- [ ] FR-8: mark conflicts, target/tool/range checks, Work Plan commitment and saved semantics unchanged.
- [ ] FR-9: consumed proposal becomes correctly labelled work-state display; no automatic rack entrance paint or fourth meaning.
- [ ] FR-10: Legacy40/frozen reference and ecology/economy/MFM/geometry preserved.

## Player workflow

- [ ] Existing first-use/F1 route remains reachable; all three labelled swatches fit without hiding action text.
- [ ] Valid/invalid entrance spray, X/C, inspection, walking and menu transitions checked with real authorised input.
- [ ] Unmarked/marked/confirmed/approved/executed/reopened/locked states are distinct and truthful.
- [ ] No map click remotely marks a tree; no new permission to fell.

## Authority and evidence

- [ ] O1–O4 explicitly resolved; D-013/Rack V2 narrow amendment approved; new packets/ownership published.
- [ ] Sourced practice, simplified game convention and deliberate gameplay calibration remain separate.
- [ ] Primary source attribution and claimed adoption limits independently checked; no universal/statutory/official game-standard claim.
- [ ] RackEntrances.cs:175–176 blue/red designation prompts migrated to Crop Tree/Fell designation; colour-sensitive entrance prompts audited.
- [ ] Source-level Fell colour correction and map rings versus old squares/diamonds wording reconciled at fresh candidate.

## Save / backward compatibility

- [ ] No enum numbers, schema/rule version, IDs, state/history, preference key or saved semantics changed.
- [ ] Current and genuine pre-change Fell/Crop/rack/order saves load with matching authoritative data; no inferred marks from colours.
- [ ] Equivalent verified-configuration hashes/state match; any drift is stopped rather than rebaselined.
- [ ] Legacy40 path and frozen archive bytes/anchors/preview round trip preserved.
- [ ] Migration help is accessible despite previously seen first-use help; no forest-save teaching history reset.

## Security / write-back

- [ ] Display adapters read only authoritative state; no new telemetry/network/write path or map marking action.
- [ ] Existing validation stays intact; no external source graphics reused without permission/licence.

## Acceptance evidence (every row at 1280×720 and 1920×1080)

- [ ] AC-1: field shapes, four viewing sides, required distances/30 m diagnostic, representative eligible tree sizes/species, light/shade/occlusion evidence.
- [ ] AC-2: full-map matching glyphs, labels, legend, tree positions and actual entrance-painted state.
- [ ] AC-3: minimap route states and disclosed individual-mark omission.
- [ ] AC-4: exactly three help swatches, migration copy, colour-vision/grayscale plus human accessibility review.
- [ ] AC-5: mark/order transitions, save/load, allowed reopen/lock behaviour, no duplicate work/resources or semantic/hash drift.
- [ ] AC-6: Legacy40 and frozen Reference Future compatibility.
- [ ] AC-7: same-fixture performance/object/material/allocation delta against approved budget and native Linux input evidence.

## Spec conformance / verdict

- [ ] Independent reviewer checks the approved specification and exact implementation/evidence; deviations recorded.
- [ ] All remaining failures/unknowns and admission/build limits disclosed; Manager disposition published.
- **Spec conformance:** NOT VERIFIED — no implementation.
- **Implementation correctness:** NOT VERIFIED — no implementation.
- **Runtime / rendered / input:** UNVERIFIED; **performance:** UNMEASURED. Unity deliberately not started; every acceptance row awaits approval and admission.

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
