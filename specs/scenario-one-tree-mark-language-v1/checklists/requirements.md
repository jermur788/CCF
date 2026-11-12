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
- [ ] FR-4: distinct shapes and text survive both resolutions, light/shade and colour-vision/grayscale review; normal-colour ΔE76 >=40, mark/background contrast >=3:1 and both scorers 100% correct per FR-4; required distances and human feedback recorded; exact conditional pink and documented redesign path followed.
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
