# Scenario One Tree Marking Language V1 Specification

- **Spec ID:** SPEC-S1-TREE-MARK-LANGUAGE-V1
- **Status:** DRAFT — NO IMPLEMENTATION AUTHORITY
- **Owner / approval:** Overall Manager; user retains product decisions.
- **Created / updated:** 2026-11-04
- **Issue:** [#259](https://github.com/jermur788/CCF/issues/259)
- **Context:** `9067554795b662966e243e09bb724aca9bd15649`; docs base `8f0f230190830855112e3e7a1d7544db94a7f591`.
- **Inspected implementation:** `72b4efbd1c653fa2231f632a79030a83ff76f833`, read with `git show`, never checked out.
- **Decision references:** D-013, D-020, D-042, D-063, D-064, D-065. D-067–D-069 do not authorise a new rack network here.
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness.
- **Supersession:** none while DRAFT. Proposed narrow visual/copy amendment to D-013 and approved Rack V2 §§25–28; preserve Rack V2 §§20–24 and all state/commitment rules.
- **Evidence / source-level baseline:** [research handoff](../../Docs/Handoffs/Overnight-Continuation-20261104/MARKING-SYSTEM-SPEC-HANDOFF.md).

## Product intent

Teach three readable management meanings while the player walks the forest. Shape, text and authoritative state must carry the meaning even when colours cannot be distinguished. This is a simplified game convention informed by documented Irish CCF practice, not an official implementation, certification or complete account of that practice.

## Problem and current truth

At the inspected commit, help calls the marks orange rack, red Fell and blue Crop. The actual source paths differ:

| Meaning | 3D source-defined visual | Stand Map | Minimap | Help |
|---|---|---|---|---|
| Rack / machine-path removal | Four orange curved vertical trunk patches, RGB `(0.95, 0.28, 0.025)` | Rack geometry: amber solid planned routes, pale harvested routes; selected entrance is a blue square. No painted-entrance-state glyph. | Suggested dashed routes, confirmed solid routes, harvested pale routes; no individual painted-entrance glyph. | Orange vertical stripes, first entrance tree only as a teaching simplification. |
| Fell proposal | Rotated cube above crown plus ground cylinder/disc; **orange-toned** `(1, 0.35, 0.05)` material/default, despite “red” wording. No diagonal trunk slashes. | Red ring in Fell & crop marks layer. | Individual Fell marks omitted. | “Red — Fell”. |
| Crop Tree / future timber | Blue above-crown diamond and ground disc **plus** a blue circumferential trunk band. | Blue ring in Fell & crop marks layer. | Individual Crop marks omitted. | “Blue — Crop Tree / future timber”. |

These are static source/material facts, not observed renders. References and the separate planned-work markers are detailed in handoff C1–C11. Curriculum text also says squares/diamonds where the inspected draw plan draws rings. Therefore a copy-only statement that Fell has orange diagonal slashes would misinform. “Blue ring” is partly supported by the existing trunk band and map ring, but hides the additional cues and existing cross-surface inconsistencies.

## Goals and non-goals

Goals: exactly three introductory marking meanings; distinctive silhouettes; field-paint versus route/status clarity; consistent 3D/map/help; honest migration; unchanged saved semantics.

Non-goals: no code, colour, material, scene or gameplay changes in this task; no new marking category, habitat treatment, paint simulator, input binding, felling permission, rack geometry, MFM rule, save schema, package or build. No change to the protected player-test candidate or frozen Reference Future v1. Legacy40 presentation remains on its existing path unless separately approved.

## User / player workflows

1. Existing Rack V2 help unlock remains tied to confirmed plan / spray context. Open **Introducing tree marking**. Read **Scenario 1 introduces 3 CCF markings** and see exactly three labelled swatches below.
2. Plan and confirm racks on the Stand Map; confirmation creates pending work, not execution. Walk to the existing valid first entrance-removal tree, equip the existing spray marker and apply the four-sided vertical mark in one interaction.
3. Continue to make individual Fell/Crop decisions in 3D through existing controls and eligibility checks. Inspect labels and matching map glyphs to distinguish removal from retention. Marking itself cuts nothing; Work Plan approval remains the commitment gate.
4. Return after load or execution: marks reflect current authoritative state, never a copied colour history. Help remains available; no forest-save tutorial latch is added.

## Functional requirements

### The three introductory meanings

The following is the **recommended proposal**, conditional on decisions O1–O4 below. Symbol descriptions are requirements for later implementation, not descriptions of current rendering.

| ID | Introductory label | Field-painted shape / colour | Full-map equivalent / label | Boundary |
|---|---|---|---|---|
| FR-1 | Rack / machine-path removal | Orange **vertical stripes on four sides**, subject to FR-4’s conditional pink fallback; retain the accepted entrance-only teaching action. | Orange upright double-stroke glyph and orange legend swatch (both pink if FR-4 fallback is selected), using the qualified world rack colour, at the actual marked entrance tree; label “Rack entrance marked”. An unmarked selected entrance remains an editing control, not a painted mark. | A rack entrance communication mark derived from the rack record, never a loose individual Fell enum. It identifies a removal tree, not a retained route-side tree or an already cleared route. |
| FR-2 | Fell | Orange **two separated parallel diagonal slashes**, repeated around the stem so visible from all sides. | Orange two-diagonal-stroke glyph and orange legend swatch using the qualified world Fell colour; replace the current red map style, label “Fell proposal”. | Existing individual removal proposal only; not approved/executed work and not a rack designation. |
| FR-3 | Crop Tree / future timber | Blue **continuous ring around the trunk**. | Blue open circular glyph and blue legend swatch using the qualified world Crop colour, label “Crop Tree — retain/favour”. | “Crop Tree — retain/favour” is a game calibration label: the source’s blue/white ring denotes high timber quality and does not define “Crop Tree”. A retained/favoured game tree, not guaranteed future quality, market value, automatic pruning or biodiversity designation. |

The map double-stroke icon and the exact two-slash count are game abstractions; the primary source specifies vertical stripes and diagonal slashes, not these precise map icons/counts. A ring on a trunk appears as a horizontal band from the side; the help swatch must show that relationship. The rack and Fell shapes must remain distinguishable when both appear grey, with normal-colour perceptual separation and background contrast under FR-4; the current near-orange RGB pair is not accepted merely because stripe directions differ.

- **FR-4 — Accessibility:** colour MUST be redundant with shape and a nearby legend/inspection label. Upright strokes, diagonal slashes and a ring MUST be recognisable in grayscale and protanopia/deuteranopia/tritanopia simulations at both target resolutions. Add a contrasting edge/backing where necessary; it must not imply another paint meaning. Proposed acceptance distance: 3.5 m, 5 m and 15 m in unobstructed lit and shaded trunk views; 30 m is a recorded diagnostic, not a mandatory long-range recognition guarantee. These distances are deliberate gameplay calibration needing O4 approval, not forestry measurements. Small-map strokes must not collapse into indistinguishable dots or be hidden behind crowns; if scale/occlusion prevents recognition, show a readable selected-item label rather than guess a meaning. Apply the quantitative contrast and recognition protocol below; human accessibility feedback is additional evidence, not a substitute for its scored result. Simulated colour vision is supporting evidence only.
- **FR-5 — Surface consistency:** 3D, full-map glyphs, map legend, inspection/HUD prompts, help and curriculum MUST name the same state and shape. Use the existing shared map source; any entrance-painted display datum is read-only and derived from `entranceMarked` plus `entranceTreeId`. Do not infer painted status from an amber route or confirmed plan. The minimap intentionally remains a reduced route view with no individual paint glyphs: its help MUST say to use the Stand Map's marking layer or inspect a tree for individual marks. O3 must approve this explicit visibility policy; adding minimap paint icons later is new scope. Dashed/solid/pale route states retain their existing meanings.
- **FR-6 — Three means three:** show only FR-1–FR-3 in this introduction. Do not introduce retained route-side double horizontal lines, bio-tree triangles, ring-barking or deadwood symbols. The source documents more categories; three is a game teaching subset. Editing handles, waypoints, sample-review overlays and work-state labels are interface cues, not extra forestry paint categories.
- **FR-7 — Honest teaching copy:** proposed text: “These game markings are informed by Pro Silva Ireland practice. {Rack colour} vertical stripes identify rack-removal entrances; orange diagonal slashes propose an individual tree for felling; a blue ring identifies a Crop Tree to retain and favour (a game label inspired by the source’s high-timber-quality ring). In this game you paint only the entrance tree for each rack; real operations may mark additional trees. Marking cuts nothing. Review and approve work in the Work Plan.” Before release, substitute “Orange” or “Pink” for `{Rack colour}` to match the qualified rack presentation; the placeholder is not player-facing copy. Use the requested heading/introduction above; avoid “CCF source material” and any “official standard implemented” claim. Detailed source links belong in reference/credits material, not the mandatory action path. Final copy ships only alongside qualified visuals.
- **FR-8 — State safety:** preserve mutual exclusion, target/range/tool checks, mark clearing, pending-order consumption, approval, execution, rack reopening and geometry-bound invalidation rules. Changing a visual MUST NOT set/clear an instruction, create/approve an order, award resources, change objective counts, or modify a saved field. Candidate suggestions remain proposals, not Fell marks (D-065).
- **FR-9 — Planned work is distinct:** when a loose Fell proposal is consumed into an order, its label becomes the actual current work state (“Pending felling”, “Approved felling”, as applicable). It must not remain labelled a fresh Fell proposal. Recommended migration: replace the old generic floating diamond/disc cues in the scoped Enlarged80 workflow with the appropriate shape and existing work-state text; do not treat that derived pending-work display as a new saved paint mark or fourth introductory category. Rack V2 pending orders MUST NOT auto-paint entrances. O2 must settle this transition before coding; failed consistency is a STOP.
- **FR-10 — Bounded scope:** no alteration of Legacy40/frozen-reference presentation, archive bytes or progression; no changes to ecology/economy, routes, MFM coverage or work results. Shared rendering code must preserve the existing legacy path and its help. No binary/source-art import is needed by this proposal.

## Authority / source of truth

Latest user instruction > Decision Log > Current Milestone > Game Brief. Approved Rack V2 remains binding. This DRAFT does not amend D-013's red Fell convention or Rack V2's existing table; O1 requires explicit Manager/user reconciliation. Implementation facts come from the pinned source, not this spec or older Overview schema summaries. After approval, this spec would authorise only its bounded presentation scope through new owned tasks and fresh baseline inspection.

## Evidence classification

1. **Sourced established practice (documented guidance, not universal empirical adoption):** Pro Silva Ireland's [Tree marking page](https://www.prosilvaireland.com/about-ccf/tree-marking/), sections “Trees to be retained” and “Trees to be removed or modified”, and [2026 Quick Guide 06](https://prosilvaireland.com/wp-content/uploads/2026/05/06-Tree-Selection-and-Marking-in-CCF.pdf), printed pp. 10–13 (PDF pages 6–7), document hot orange/pink removal, diagonal individual-removal slashes, four-sided vertical rack-removal stripes, and white/blue quality rings. They separately document double horizontal marks on **retained** route-side trees. The guide calls its system an “Irish tree marking standard for CCF thinning operations”; that is a cited publisher description, not evidence of statutory status, universal European practice or game compliance.
2. **Simplified game convention:** exactly three meanings, entrance-tree-only spray, one interaction painting all sides, map symbols, existing state/Work Plan progression, and a Crop Tree teaching label that does not certify timber quality. These are not sourced operational rules.
3. **Deliberate gameplay calibration:** orange-first rack with a pre-agreed pink fallback, orange Fell, blue rather than white, exact shades/dimensions, two diagonal strokes, outlines, draw distances and minimum readable UI sizes. Must be tested and approved; source colours alone do not prove accessibility.

## Save / backward compatibility and visual migration

The inspected candidate is save v24 (`ForestSaveData.cs:7`), with Fell/Crop ID lists (`:33–34`) and separate rack state; `TreeMarkType` remains None=0, Fell=1, CropTree=2. **No new enum value, schema/version, rule-version, save-history field, preference key or data migration is proposed.** Orange is a presentation choice, not a saved instruction. A loaded Fell remains Fell; a loaded Crop remains Crop; `entranceMarked` remains attached to the same rack/tree; completed work is never reconstructed or reversed. Recolouring must not turn every orange-looking legacy object into a rack tree.

Proposed release migration, after approval:

1. Inventory current proposal, pending/approved-work, rack and crop cues in the freshly pinned candidate, including MFM review/selection and legacy paths. Include `ForestTreeMarkingManager.RackEntrances.cs:175–176`: “blue designation” / “red designation” conflict prompts; proposed replacements are “Crop Tree designation” / “Fell designation”, retaining the existing rejection behaviour. Audit its existing “Orange” entrance prompts too: they must use the selected rack colour or colour-independent state wording. Remove no asset files.
2. Replace scoped **runtime-generated visuals** from authoritative state, atomically with map glyphs/legends/help/curriculum. Do not ship new copy over old geometry. Keep the existing Legacy40 branch unchanged.
3. Replace duplicate active Fell/Crop diamonds/discs in the scoped workflow so primary recognition no longer depends on two same-shaped floating objects. Preserve necessary selection/navigation affordances with explicit interface styling and labels. Settle pending-work transitions under O2 before implementation.
4. Add a concise, repeatably accessible help note: “Fell proposals now use orange diagonal slashes; Crop Trees use a blue ring. Existing decisions and work are unchanged.” Do not add a forest-save “seen migration” flag. Existing local first-use preferences may already suppress automatic help; show the release/help route without resetting forest history.
5. Compare equivalent old-save loads before/after; assert exact authoritative mark/rack/order/objective/economy values and protected world/state hashes under the same verified configuration. A visually safe intention is not a proven save guarantee. Unchanged schema alone does not prove unchanged hashes. Stop on any semantic/hash drift; no rebaseline.

## Security / write-back

No network service, telemetry, permission system or new save-write path. Existing mark/Work Plan validation remains authoritative. Maps and legends are read-only projections; no map click can remotely mark a tree. External source graphics are references only; do not reproduce them without licence clearance.

## Acceptance criteria and future render/test matrix

Every row is required at **1280×720 AND 1920×1080**, Unity **6000.6.0f1**, on an explicitly authorised exact candidate/lease. All results currently **UNVERIFIED / UNMEASURED**. No Unity/Player/build execution is authorised by this draft. Record candidate SHA, scene/save fixture, camera position/FOV/distance, UI scale, lighting/occlusion, screenshot filename, measured result, scored recognition answers, and reviewer commentary (commentary cannot override a failed threshold). Store later evidence under the Files CCF tree, not ZX20.

### Proposed perceptual-difference, background-contrast and recognition protocol (FR-4 / O4)

These are pre-agreed **DRAFT gameplay acceptance thresholds**, not measured results or forestry rules; O4 must approve them before implementation.

- **Mark-versus-mark perceptual difference:** encoded-sRGB rack and Fell colours require **CIE76 ΔE*ab >=40**, using D65 Lab and the reproducible arithmetic in the round-2 Review response. Check both specified palette values and median paint/glyph/swatch interior Lab values from matched **normal-colour** AC-1/AC-2/AC-4 captures, excluding edges/backings; match tree, camera, light and exposure, changing only the mark. Record masks and medians; an unmeasurable required interior fails. Do not apply this colour floor to grayscale or colour-vision transforms: distinct upright/slash/ring shapes and scored recognition must carry those modes. Current rack `(0.95,0.28,0.025)` versus Fell `(1,0.35,0.05)` gives ΔE76 **6.0042**, so it already triggers the conditional exact pink candidate `(1,0.08,0.58)`, ΔE76 **78.0777**. Rendered passes remain unverified. There is **no rack-versus-Fell contrast-ratio threshold**.
- **Mark-versus-background contrast:** at every required unobstructed AC-1 view (including thin textured trunks at 15 m in light/shade) and AC-2/AC-4 glyph/swatch view, require **>=3:1** against adjacent bark, map, legend or help background, in normal/grayscale and each named colour-vision simulation. Linearise screenshot sRGB, use `L=0.2126r+0.7152g+0.0722b`, and `(max(L1,L2)+0.05)/(min(L1,L2)+0.05)`. Record tool/transform/version, pixel masks and median luminance of mark and each distinct adjacent background region. A contrasting edge/backing may supply the readable shape boundary: measure that boundary against adjacent background and the paint interior against its backing, each >=3:1. The edge must preserve the mark silhouette and imply no new paint meaning. If no edge is used, measure paint directly against background. Missing measurable interior/boundary or a collapsed shape fails. A background failure requires edge/backing/size/visibility correction and a complete recheck; it does not by itself trigger a hue change. Palette arithmetic cannot establish rendered background contrast.
- **Recognition:** prepare a numbered answer key from authoritative state for every required unobstructed AC-1 capture and each FR-1–FR-3 glyph/swatch in AC-2/AC-4, at both resolutions and all five modes (normal, grayscale, protanopia, deuteranopia, tritanopia). Show the legend once, then hide labels, selection highlights and location clues; display each native-resolution capture in a recorded random order for five seconds with no zoom. Two people who did not author the visuals independently answer Rack / Fell / Crop / Unsure. Pass only if **each person answers 100% correctly in each mode, resolution and required distance**; Unsure, missing or late responses fail. Preserve order, answer key, raw answers and timings so a second scorer obtains the same pass/fail. Occluded views and 30 m captures remain diagnostics, excluded explicitly rather than counted as correct.
- **Conditional fallback and recognition failure actions:** if orange fails the normal-colour ΔE76 floor or any required recognition score, switch rack paint, full-map glyph, legend, help swatch and colour wording atomically to exact encoded sRGB **(1,0.08,0.58)**, keep Fell **(1,0.35,0.05)**, and run the complete perceptual/background/recognition matrix. The static palette floor already selects this pink for future qualification. If pink fails rendered ΔE76 or recognition, document the failed cells and redesign shape/size/edge/visibility within approved scope, then perform a fresh complete matrix with new recorded random order; never repeat unchanged captures until scorers learn the key. Do not switch back to orange or demand pair contrast >=3:1. If passing requires weakened thresholds, changed meanings or expanded scope, STOP and return to Manager/user. Neither a numeric colour pass nor an edge/backing pass overrides a failed recognition score.

Pink is a source-reported alternative hot colour; the exact RGB and floor are game calibration. This docs-only task changes no runtime colour. AC-3/AC-5/AC-6 retain their state/compatibility assertions; AC-7 requires an O4-approved numeric performance budget, never “looks acceptable”.

| ID | Reproducible future action | Required observable result / evidence |
|---|---|---|
| AC-1 | In an isolated new Enlarged80 Scenario One, confirm the existing rack plan; apply a valid entrance mark; create separate eligible Fell/Crop marks through real controls. Capture all three from four trunk sides at 3.5/5/15 m; capture 30 m diagnostics. Repeat in light, shade, crowded trunks and representative Sitka/Oak/Beech sizes where legally eligible. | Correct vertical/slash/ring shapes with no misleading duplicate active glyph; labels and conflict rejection correct. FR-4 normal-colour ΔE76 >=40, mark/background contrast >=3:1 and both scorers 100% correct at required distances; document occlusion, do not claim seeing through vegetation. Record any ineligible species/size fixture as N/A with rule, never silently substitute. |
| AC-2 | Open Stand Map [M], select Fell & crop marks layer; inspect corresponding marked tree positions and selected rack entrance before/after spray; compare legend/help/curriculum. | Full map meets FR-4 scoring/contrast with explicitly orange Fell, orange-or-fallback-pink rack, blue Crop glyphs and identical legend colours; accurate painted-entrance state. Same tree/rack IDs and world positions; edit handle/entrance control not mistaken for paint. No squares/diamonds copy describing circles. No remote marking. |
| AC-3 | Close map; view minimap before confirmation, after confirmation, after spray and after execution. | Dashed/solid/pale route semantics stay correct. Individual marks deliberately absent and accurately explained. Spraying does not turn a planned route into an executed/open route. |
| AC-4 | Capture help on first unlock and F1 revisit, with all three swatches and migration note at both resolutions. Compare normal/grayscale/protanopia/deuteranopia/tritanopia views and obtain human accessibility feedback. | Exactly three meanings, FR-4 scoring/contrast passed for swatches and glyphs, and readable labels; no clipped/wrapped-away action, swatch overlap or colour-only instruction. No official-compliance claim. |
| AC-5 | Use disposable current and genuine pre-change saves covering Fell/Crop, marked/unmarked/reopened/locked racks, pending/approved/executed orders; save/load, cancel/reopen only through allowed flow, and advance one year after authorised fixture work. | Meaning/IDs/geometry/history unchanged; cues track consumed proposals and actual orders; no extra removal/resource award/consumption. Only geometry-changed racks lose marks under existing rules; no duplicate execution. Compare state/hash evidence, not screenshots alone. |
| AC-6 | Load genuine Legacy40 and preview frozen Reference Future v1, then exit preview. | Existing visuals/help/progression remain; archive bytes and protected anchors unchanged, no V2 marks invented. Player world/pose restored by existing contract. |
| AC-7 | Compare representative pre/post CPU/GPU frame time, marker object/material counts and rebuild allocations on the same authorised fixture/configuration. Exercise walking, inspection, spraying, X/C and menu transitions through native Linux input. | No material performance regression against a Manager-approved budget; input/selection work, repeated mark rejection and Work Plan gates remain. Budget is not yet measured/approved. Native Windows remains separately unverified. |

Acceptance requires source/compile checks, affected offline symbol/state tests, the above bounded Tier-2 qualification after admission, independent evidence/spec review, and Manager disposition. Do not repeat an unrelated complete Tier-3 matrix merely for this visual feature.

## Stop conditions

- DRAFT remains unapproved, or any O1–O4 product decision remains unresolved.
- Work requires saved-state reinterpretation, new schema/rule version, felling permission, extra marking meanings, changed rack/MFM geometry, or an edit outside a future assigned ownership packet.
- Unresolved FR-4 failure: apply the separate colour/background/recognition failure actions below; stop for Manager/user if redesign requires weaker gates, changed meanings or expanded scope. Stop if state labels/cues disagree across surfaces, or pending-work transition would mislead.
- Protected legacy/reference/state/hash evidence drifts, or atomic copy/visual delivery is impossible.
- Unity remains deferred; no fresh admission/lease/space/process/ownership proof exists. Report outstanding render/input/performance gates; do not bypass them.

## Open product decisions

| ID | Decision / recommendation | Authority needed |
|---|---|---|
| O1 | Approve orange diagonal Fell, orange-first/pink-fallback rack and blue Crop ring as this game's presentation, explicitly amending D-013 / Rack V2 §§25–28 while preserving removal/retention semantics. | Manager and user product approval; this draft is insufficient. |
| O2 | Approve atomic retirement of scoped floating Fell/Crop cues and work-state-labelled replacement; settle pending/approved-order display so “proposal” never means already commissioned work. | Manager; return to user if teaching/visibility trade-off is material. |
| O3 | Approve explicit orange Fell / orange-or-fallback-pink rack / blue Crop full-map and legend colours and matching paint glyphs and marked-entrance-state display, while retaining a route-only minimap and unchanged Legacy40 path. | Manager scope decision; no new minimap icons assumed. |
| O4 | Approve required recognition distances/visual budget, final shades/dimensions and the proposed FR-4 ΔE76 >=40 / background >=3:1 / 100%-correct protocol with exact conditional pink fallback; set a numeric performance budget before qualification and assess prototype evidence. | Manager/user visual review; no invented measured thresholds. |

No implementation task may be released until these are resolved and the four-file spec receives explicit approval.

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
