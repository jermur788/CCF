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
| FR-1 | Rack / machine-path removal | Orange **vertical stripes on four sides**, subject to FR-4’s mandatory pink fallback; retain the accepted entrance-only teaching action. | Orange upright double-stroke glyph and orange legend swatch (both pink if FR-4 fallback is selected), using the qualified world rack colour, at the actual marked entrance tree; label “Rack entrance marked”. An unmarked selected entrance remains an editing control, not a painted mark. | A rack entrance communication mark derived from the rack record, never a loose individual Fell enum. It identifies a removal tree, not a retained route-side tree or an already cleared route. |
| FR-2 | Fell | Orange **two separated parallel diagonal slashes**, repeated around the stem so visible from all sides. | Orange two-diagonal-stroke glyph and orange legend swatch using the qualified world Fell colour; replace the current red map style, label “Fell proposal”. | Existing individual removal proposal only; not approved/executed work and not a rack designation. |
| FR-3 | Crop Tree / future timber | Blue **continuous ring around the trunk**. | Blue open circular glyph and blue legend swatch using the qualified world Crop colour, label “Crop Tree — retain/favour”. | “Crop Tree — retain/favour” is a game calibration label: the source’s blue/white ring denotes high timber quality and does not define “Crop Tree”. A retained/favoured game tree, not guaranteed future quality, market value, automatic pruning or biodiversity designation. |

The map double-stroke icon and the exact two-slash count are game abstractions; the primary source specifies vertical stripes and diagonal slashes, not these precise map icons/counts. A ring on a trunk appears as a horizontal band from the side; the help swatch must show that relationship. The rack and Fell shapes must remain distinguishable when both appear grey, with independent luminance separation under FR-4; the current near-orange RGB pair is not accepted merely because stripe directions differ.

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

### Proposed binary contrast and recognition protocol (FR-4 / O4)

These are pre-agreed **DRAFT gameplay acceptance thresholds**, not measured results or forestry rules; O4 must approve them before implementation.

- **Contrast:** at every unobstructed required AC-1 view (including thin eligible trunks at 15 m in bark texture and shade), measure each mark’s median visible paint-interior luminance in the final screenshot, excluding any outline/backing. Use sRGB-to-linear conversion and `L = 0.2126 R + 0.7152 G + 0.0722 B`; record pixel masks and medians. Matched rack/Fell fixture captures must achieve `(L_lighter + 0.05)/(L_darker + 0.05) >= 3:1` in normal and grayscale captures; apply the same measurement after each named colour-vision transform, recording transform/tool/version. A view with no measurable paint interior fails. Match tree, camera, light and exposure between the pair, changing only the mark. On map/legend/help, measure the same >=3:1 rack/Fell separation on glyph/swatch interiors. Outlines/backings may aid bark/background visibility but cannot satisfy the pair’s interior-luminance requirement.
- **Recognition:** prepare a numbered answer key from authoritative state for every required unobstructed AC-1 capture and each FR-1–FR-3 glyph/swatch in AC-2/AC-4, at both resolutions and all five modes (normal, grayscale, protanopia, deuteranopia, tritanopia). Show the legend once, then hide labels, selection highlights and location clues; display each native-resolution capture in a recorded random order for five seconds with no zoom. Two people who did not author the visuals independently answer Rack / Fell / Crop / Unsure. Pass only if **each person answers 100% correctly in each mode, resolution and required distance**; Unsure, missing or late responses fail. Preserve order, answer key, raw answers and timings so a second scorer obtains the same pass/fail. Occluded views and 30 m captures remain diagnostics, excluded explicitly rather than counted as correct.
- **Fallback rule:** if orange rack versus orange Fell fails either the >=3:1 luminance criterion or the scored recognition check in any required view, switch rack paint, full-map glyph, legend, help swatch and colour wording together to a luminance-adjusted hot pink, keep Fell orange, and rerun the complete contrast/recognition matrix; if pink also fails, stop for redesign without weakening thresholds or changing saved meanings.

Pink is a source-reported alternative hot colour; its precise shade/luminance remains game calibration. This fallback changes no code or colour in this docs-only task. A successful fallback permits qualification to continue under the approved rule without a new choice between the two orange shades. AC-3/AC-5/AC-6 use the stated state/compatibility assertions; AC-7 still needs an O4-approved numeric performance budget before execution and cannot be passed by “looks acceptable”.

| ID | Reproducible future action | Required observable result / evidence |
|---|---|---|
| AC-1 | In an isolated new Enlarged80 Scenario One, confirm the existing rack plan; apply a valid entrance mark; create separate eligible Fell/Crop marks through real controls. Capture all three from four trunk sides at 3.5/5/15 m; capture 30 m diagnostics. Repeat in light, shade, crowded trunks and representative Sitka/Oak/Beech sizes where legally eligible. | Correct vertical/slash/ring shapes with no misleading duplicate active glyph; labels and conflict rejection correct. FR-4 contrast >=3:1 and both scorers 100% correct at required distances; document occlusion, do not claim seeing through vegetation. Record any ineligible species/size fixture as N/A with rule, never silently substitute. |
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
- Orange rack/Fell fails FR-4: apply the defined pink fallback and rerun; stop for redesign only if that fallback also fails. Stop if state labels/cues disagree across surfaces, or pending-work transition would mislead.
- Protected legacy/reference/state/hash evidence drifts, or atomic copy/visual delivery is impossible.
- Unity remains deferred; no fresh admission/lease/space/process/ownership proof exists. Report outstanding render/input/performance gates; do not bypass them.

## Open product decisions

| ID | Decision / recommendation | Authority needed |
|---|---|---|
| O1 | Approve orange diagonal Fell, orange-first/pink-fallback rack and blue Crop ring as this game's presentation, explicitly amending D-013 / Rack V2 §§25–28 while preserving removal/retention semantics. | Manager and user product approval; this draft is insufficient. |
| O2 | Approve atomic retirement of scoped floating Fell/Crop cues and work-state-labelled replacement; settle pending/approved-order display so “proposal” never means already commissioned work. | Manager; return to user if teaching/visibility trade-off is material. |
| O3 | Approve explicit orange Fell / orange-or-fallback-pink rack / blue Crop full-map and legend colours and matching paint glyphs and marked-entrance-state display, while retaining a route-only minimap and unchanged Legacy40 path. | Manager scope decision; no new minimap icons assumed. |
| O4 | Approve required recognition distances/visual budget, final shades/dimensions and the proposed FR-4 >=3:1 / 100%-correct protocol with pink fallback; set a numeric performance budget before qualification and assess prototype evidence. | Manager/user visual review; no invented measured thresholds. |

No implementation task may be released until these are resolved and the four-file spec receives explicit approval.

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
