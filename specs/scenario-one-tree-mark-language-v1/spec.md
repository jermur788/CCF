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
| FR-1 | Rack / machine-path removal | Orange **vertical stripes on four sides**; retain the accepted entrance-only teaching action. | Upright double-stroke glyph at the actual marked entrance tree; label “Rack entrance marked”. An unmarked selected entrance remains an editing control, not a painted mark. | A rack entrance communication mark derived from the rack record, never a loose individual Fell enum. It identifies a removal tree, not a retained route-side tree or an already cleared route. |
| FR-2 | Fell | Orange **two separated parallel diagonal slashes**, repeated around the stem so visible from all sides. | Matching two diagonal strokes, label “Fell proposal”. | Existing individual removal proposal only; not approved/executed work and not a rack designation. |
| FR-3 | Crop Tree / future timber | Blue **continuous ring around the trunk**. | Blue open circular glyph, label “Crop Tree — retain/favour”. | A retained/favoured tree, not guaranteed future quality, market value, automatic pruning or biodiversity designation. |

The map double-stroke icon and the exact two-slash count are game abstractions; the primary source specifies vertical stripes and diagonal slashes, not these precise map icons/counts. A ring on a trunk appears as a horizontal band from the side; the help swatch must show that relationship. The rack and Fell shapes must remain distinguishable when both appear grey.

- **FR-4 — Accessibility:** colour MUST be redundant with shape and a nearby legend/inspection label. Upright strokes, diagonal slashes and a ring MUST be recognisable in grayscale and protanopia/deuteranopia/tritanopia simulations at both target resolutions. Add a contrasting edge/backing where necessary; it must not imply another paint meaning. Proposed acceptance distance: 3.5 m, 5 m and 15 m in unobstructed lit and shaded trunk views; 30 m is a recorded diagnostic, not a mandatory long-range recognition guarantee. These distances are deliberate gameplay calibration needing O4 approval, not forestry measurements. Small-map strokes must not collapse into indistinguishable dots or be hidden behind crowns; if scale/occlusion prevents recognition, show a readable selected-item label rather than guess a meaning. Human accessibility review remains required; simulated colour vision is supporting evidence only.
- **FR-5 — Surface consistency:** 3D, full-map glyphs, map legend, inspection/HUD prompts, help and curriculum MUST name the same state and shape. Use the existing shared map source; any entrance-painted display datum is read-only and derived from `entranceMarked` plus `entranceTreeId`. Do not infer painted status from an amber route or confirmed plan. The minimap intentionally remains a reduced route view with no individual paint glyphs: its help MUST say to use the Stand Map's marking layer or inspect a tree for individual marks. O3 must approve this explicit visibility policy; adding minimap paint icons later is new scope. Dashed/solid/pale route states retain their existing meanings.
- **FR-6 — Three means three:** show only FR-1–FR-3 in this introduction. Do not introduce retained route-side double horizontal lines, bio-tree triangles, ring-barking or deadwood symbols. The source documents more categories; three is a game teaching subset. Editing handles, waypoints, sample-review overlays and work-state labels are interface cues, not extra forestry paint categories.
- **FR-7 — Honest teaching copy:** proposed text: “These game markings are informed by Pro Silva Ireland practice. Orange vertical stripes identify rack-removal entrances; orange diagonal slashes propose an individual tree for felling; a blue ring identifies a Crop Tree to retain and favour. In this game you paint only the entrance tree for each rack; real operations may mark additional trees. Marking cuts nothing. Review and approve work in the Work Plan.” Use the requested heading/introduction above; avoid “CCF source material” and any “official standard implemented” claim. Detailed source links belong in reference/credits material, not the mandatory action path. Final copy ships only alongside qualified visuals.
- **FR-8 — State safety:** preserve mutual exclusion, target/range/tool checks, mark clearing, pending-order consumption, approval, execution, rack reopening and geometry-bound invalidation rules. Changing a visual MUST NOT set/clear an instruction, create/approve an order, award resources, change objective counts, or modify a saved field. Candidate suggestions remain proposals, not Fell marks (D-065).
- **FR-9 — Planned work is distinct:** when a loose Fell proposal is consumed into an order, its label becomes the actual current work state (“Pending felling”, “Approved felling”, as applicable). It must not remain labelled a fresh Fell proposal. Recommended migration: replace the old generic floating diamond/disc cues in the scoped Enlarged80 workflow with the appropriate shape and existing work-state text; do not treat that derived pending-work display as a new saved paint mark or fourth introductory category. Rack V2 pending orders MUST NOT auto-paint entrances. O2 must settle this transition before coding; failed consistency is a STOP.
- **FR-10 — Bounded scope:** no alteration of Legacy40/frozen-reference presentation, archive bytes or progression; no changes to ecology/economy, routes, MFM coverage or work results. Shared rendering code must preserve the existing legacy path and its help. No binary/source-art import is needed by this proposal.

## Authority / source of truth

Latest user instruction > Decision Log > Current Milestone > Game Brief. Approved Rack V2 remains binding. This DRAFT does not amend D-013's red Fell convention or Rack V2's existing table; O1 requires explicit Manager/user reconciliation. Implementation facts come from the pinned source, not this spec or older Overview schema summaries. After approval, this spec would authorise only its bounded presentation scope through new owned tasks and fresh baseline inspection.

## Evidence classification

1. **Sourced established practice (documented guidance, not universal empirical adoption):** Pro Silva Ireland's [Tree marking page](https://www.prosilvaireland.com/about-ccf/tree-marking/), sections “Trees to be retained” and “Trees to be removed or modified”, and [2026 Quick Guide 06](https://prosilvaireland.com/wp-content/uploads/2026/05/06-Tree-Selection-and-Marking-in-CCF.pdf), printed pp. 10–13 (PDF pages 6–7), document hot orange/pink removal, diagonal individual-removal slashes, four-sided vertical rack-removal stripes, and white/blue quality rings. They separately document double horizontal marks on **retained** route-side trees. The guide calls its system an “Irish tree marking standard for CCF thinning operations”; that is a cited publisher description, not evidence of statutory status, universal European practice or game compliance.
2. **Simplified game convention:** exactly three meanings, entrance-tree-only spray, one interaction painting all sides, map symbols, existing state/Work Plan progression, and a Crop Tree teaching label that does not certify timber quality. These are not sourced operational rules.
3. **Deliberate gameplay calibration:** orange rather than pink, blue rather than white, exact shades/dimensions, two diagonal strokes, outlines, draw distances and minimum readable UI sizes. Must be tested and approved; source colours alone do not prove accessibility.

## Save / backward compatibility and visual migration

The inspected candidate is save v24 (`ForestSaveData.cs:7`), with Fell/Crop ID lists (`:33–34`) and separate rack state; `TreeMarkType` remains None=0, Fell=1, CropTree=2. **No new enum value, schema/version, rule-version, save-history field, preference key or data migration is proposed.** Orange is a presentation choice, not a saved instruction. A loaded Fell remains Fell; a loaded Crop remains Crop; `entranceMarked` remains attached to the same rack/tree; completed work is never reconstructed or reversed. Recolouring must not turn every orange-looking legacy object into a rack tree.

Proposed release migration, after approval:

1. Inventory current proposal, pending/approved-work, rack and crop cues in the freshly pinned candidate, including MFM review/selection and legacy paths. Remove no asset files.
2. Replace scoped **runtime-generated visuals** from authoritative state, atomically with map glyphs/legends/help/curriculum. Do not ship new copy over old geometry. Keep the existing Legacy40 branch unchanged.
3. Replace duplicate active Fell/Crop diamonds/discs in the scoped workflow so primary recognition no longer depends on two same-shaped floating objects. Preserve necessary selection/navigation affordances with explicit interface styling and labels. Settle pending-work transitions under O2 before implementation.
4. Add a concise, repeatably accessible help note: “Fell proposals now use orange diagonal slashes; Crop Trees use a blue ring. Existing decisions and work are unchanged.” Do not add a forest-save “seen migration” flag. Existing local first-use preferences may already suppress automatic help; show the release/help route without resetting forest history.
5. Compare equivalent old-save loads before/after; assert exact authoritative mark/rack/order/objective/economy values and protected world/state hashes under the same verified configuration. A visually safe intention is not a proven save guarantee. Unchanged schema alone does not prove unchanged hashes. Stop on any semantic/hash drift; no rebaseline.

## Security / write-back

No network service, telemetry, permission system or new save-write path. Existing mark/Work Plan validation remains authoritative. Maps and legends are read-only projections; no map click can remotely mark a tree. External source graphics are references only; do not reproduce them without licence clearance.

## Acceptance criteria and future render/test matrix

Every row is required at **1280×720 AND 1920×1080**, Unity **6000.6.0f1**, on an explicitly authorised exact candidate/lease. All results currently **UNVERIFIED / UNMEASURED**. No Unity/Player/build execution is authorised by this draft. Record candidate SHA, scene/save fixture, camera position/FOV/distance, UI scale, lighting/occlusion, screenshot filename, measured result, and reviewer verdict. Store later evidence under the Files CCF tree, not ZX20.

| ID | Reproducible future action | Required observable result / evidence |
|---|---|---|
| AC-1 | In an isolated new Enlarged80 Scenario One, confirm the existing rack plan; apply a valid entrance mark; create separate eligible Fell/Crop marks through real controls. Capture all three from four trunk sides at 3.5/5/15 m; capture 30 m diagnostics. Repeat in light, shade, crowded trunks and representative Sitka/Oak/Beech sizes where legally eligible. | Correct vertical/slash/ring shapes with no misleading duplicate active glyph; labels and conflict rejection correct. Unobstructed recognition at proposed required distances; document occlusion, do not claim seeing through vegetation. Record any ineligible species/size fixture as N/A with rule, never silently substitute. |
| AC-2 | Open Stand Map [M], select Fell & crop marks layer; inspect corresponding marked tree positions and selected rack entrance before/after spray; compare legend/help/curriculum. | Full map has distinct shape glyphs and accurate painted-entrance state. Same tree/rack IDs and world positions; edit handle/entrance control not mistaken for paint. No squares/diamonds copy describing circles. No remote marking. |
| AC-3 | Close map; view minimap before confirmation, after confirmation, after spray and after execution. | Dashed/solid/pale route semantics stay correct. Individual marks deliberately absent and accurately explained. Spraying does not turn a planned route into an executed/open route. |
| AC-4 | Capture help on first unlock and F1 revisit, with all three swatches and migration note at both resolutions. Compare normal/grayscale/protanopia/deuteranopia/tritanopia views and obtain human accessibility feedback. | Exactly three meanings, visible shape distinctions and readable labels; no clipped/wrapped-away action, swatch overlap or colour-only instruction. No official-compliance claim. |
| AC-5 | Use disposable current and genuine pre-change saves covering Fell/Crop, marked/unmarked/reopened/locked racks, pending/approved/executed orders; save/load, cancel/reopen only through allowed flow, and advance one year after authorised fixture work. | Meaning/IDs/geometry/history unchanged; cues track consumed proposals and actual orders; no extra removal/resource award/consumption. Only geometry-changed racks lose marks under existing rules; no duplicate execution. Compare state/hash evidence, not screenshots alone. |
| AC-6 | Load genuine Legacy40 and preview frozen Reference Future v1, then exit preview. | Existing visuals/help/progression remain; archive bytes and protected anchors unchanged, no V2 marks invented. Player world/pose restored by existing contract. |
| AC-7 | Compare representative pre/post CPU/GPU frame time, marker object/material counts and rebuild allocations on the same authorised fixture/configuration. Exercise walking, inspection, spraying, X/C and menu transitions through native Linux input. | No material performance regression against a Manager-approved budget; input/selection work, repeated mark rejection and Work Plan gates remain. Budget is not yet measured/approved. Native Windows remains separately unverified. |

Acceptance requires source/compile checks, affected offline symbol/state tests, the above bounded Tier-2 qualification after admission, independent evidence/spec review, and Manager disposition. Do not repeat an unrelated complete Tier-3 matrix merely for this visual feature.

## Stop conditions

- DRAFT remains unapproved, or any O1–O4 product decision remains unresolved.
- Work requires saved-state reinterpretation, new schema/rule version, felling permission, extra marking meanings, changed rack/MFM geometry, or an edit outside a future assigned ownership packet.
- New shades/shapes cannot be distinguished at required views, state labels/cues disagree across surfaces, or pending-work transition would mislead.
- Protected legacy/reference/state/hash evidence drifts, or atomic copy/visual delivery is impossible.
- Unity remains deferred; no fresh admission/lease/space/process/ownership proof exists. Report outstanding render/input/performance gates; do not bypass them.

## Open product decisions

| ID | Decision / recommendation | Authority needed |
|---|---|---|
| O1 | Approve orange diagonal Fell and blue Crop ring as this game's presentation, explicitly amending D-013 / Rack V2 §§25–28 while preserving removal/retention semantics. | Manager and user product approval; this draft is insufficient. |
| O2 | Approve atomic retirement of scoped floating Fell/Crop cues and work-state-labelled replacement; settle pending/approved-order display so “proposal” never means already commissioned work. | Manager; return to user if teaching/visibility trade-off is material. |
| O3 | Approve matching full-map paint glyphs and marked-entrance-state display, while retaining a route-only minimap and unchanged Legacy40 path. | Manager scope decision; no new minimap icons assumed. |
| O4 | Approve required recognition distances/visual budget, final shades/dimensions and human accessibility/performance acceptance method after prototype evidence. | Manager/user visual review; no invented measured thresholds. |

No implementation task may be released until these are resolved and the four-file spec receives explicit approval.
