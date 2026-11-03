# Scenario One Access Spine & Diagonal Rack Network V1

**Status: DRAFT — MANAGER STOP FOR IMPLEMENTATION following #250; #251 topology/spec analysis READY. NOT APPROVED.**  
**Owner:** Overall Manager; **date:** 2026-11-03.  
**Authority:** latest user's personal playtest choice; Decision Log D-067 (confirmed design direction), subject to D-064 / approved Rack V2 until explicitly reconciled.  
**Milestone:** complete-loop three-species Scenario One demo readiness; **scope:** Enlarged80 only.  
**Evidence status:** #246 source reconnaissance **APPROVED AS RESEARCH, NOT UNITY MEASUREMENT**; #248 real geometry and MFM mask measurement **BLOCKED / PREPARED**. No A1 or A2 implementation authorised. [Manager disposition](../../Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-REVIEW-20261103.md).
**Evidence:** user-supplied Objective 1 / 3D rack screenshots, [#246](https://github.com/jermur788/CCF/issues/246) read-only investigation when returned, [Pro Silva Ireland access guidance](https://silviculture.org.uk/wp-content/uploads/2024/01/Pro-Silva-Ireland-Webinar-08-09-2020-Final.pdf), [Teagasc thinning guidance](https://teagasc.ie/crops/forestry/advice/timber-harvesting/tending-and-thinning-of-broadleaf-forests/). External evidence supports planned permanent access and connected extraction; the specific spine/diagonal layout is **GAME DESIGN / SITE-SPECIFIC PROPOSAL**, not a universal Irish standard.

## 1. User-approved conceptual direction

During personal playtest the user preferred the **existing central path extended into an access spine/axis** with **diagonal extraction-rack branches**, rather than detached corridors and unmapped apparent trails. This establishes a planning direction only: **it does not prove the real 80×80-m geometry or approve topology/persistence changes yet**.

## 2. Proposed solution

- **Critical A1 caveat:** under the inspected candidate, `Dirt Path*` geometry also enters the MFM `MaskKind.Track` exclusion set. `MaskKind.Headland` covers feeder/headland/pads and both mask kinds exclude prescription treatment by membership; simply adding path planks can change excluded treatment units and prescription results without any new saved field. **Do not treat a plank extension as cosmetic or old-save safe by default.** A visually mapped-only route would require deliberately designed semantics independent from treatment-exclusion masks, with player-facing honesty about whether a physical corridor already exists.
- Use the existing central route as the continuous **feeder/access spine**, extending it toward the required upper stand area only where site geometry and actual access permit. A walkable route, a machine-capable extraction spine and a planned-but-not-yet-cleared corridor must not be visually conflated.
- Draw approximately parallel **diagonal branch extraction racks** toward/from the spine (a herringbone-like arrangement where geometry permits). Preserve the initial five-rack gameplay target and 16–20 m *site-specific* spacing guidance until actual geometry proves a change is needed. A feeder spine **does not automatically count as one of the five extraction racks**.
- Treat only one map-authored, persistent, authoritative access/rack network as geometry truth. The 3D forest, Stand Map, minimap, rack entrance marker, waypoint, objective guidance and Work Plan consume the same connectivity/status distinctions.
- Do **not** render unexecuted plans as pre-existing cleared corridors. Proposed rack geometry, marked entrances, approved work and actual cleared access are separate states.
- Support the user's desire to **draft suboptimal/disconnected rack origins** and see why they are poor, rather than silently snapping to the map edge. Precisely whether disconnected proposals can be confirmed/approved is **OPEN** and cannot override current D-064 confirmation rules until spec approval.
- If physical tree density allows, off-route player movement should be possible but slower than on maintained access, with honest collision/traversal feedback. No user movement-speed values are approved yet.

## 3. Protected contract

Preserve Rack V2's map-only rack authoring, physical orange entrance-tree field marks, one rack object set, five-count until geometry reconciled, spacing feedback, unique IDs, valid entrance targeting, explicit Work Plan commitment gate and approved reopening/executed-removal rules. Do not imply plans automatically fell trees or that painted entrances clear paths. Keep MFM V1 coverage aligned with persisted, confirmed rack geometry; avoid divergent connectivity sources.

Preserve Legacy40 and frozen Reference Future v1; old saves must not fabricate newly established access routes. No change to the current user playtest candidate before explicit Manager integration/qualification; no Windows build before personal playtest completion and later explicit authorisation.

## 4. Reconciliation and open decisions

0. What is the incremental excluded MFM coverage for candidate A1 route geometry, including existing-save treatment coverage effects? Does the representation use `Dirt Path*` (Track), headland geometry (Headland) or a separate non-masking visual path, and why?
1. Where does the existing visible path actually start/end in verified world coordinates? Does it support machine access, foot access or only decoration? Which visible trails are misleading/unmapped?
2. Can diagonal branches be placed in the actual Enlarged80 stand with five coherent racks and the 16–20 m recommendation? Are there turning/headland, width, tree-removal, slope or access constraints?
3. Does the spine count as its own distinct work item, existing access, planned extension, or some combination? What does extending it remove/cost, when does it become physically traversable, and how does it affect MFM/persistence?
4. How are interior/disconnected rack proposals represented and explained? Can such proposals be confirmed with warnings, or must a genuinely non-extractable proposal be repaired before Work Plan approval? Distinguish poor but possible from physically impossible.
5. What movement slowdown is evidence-consistent and playable when pushing through standing spruce, without clipping/no-collision exploitation?
6. What changes, if any, are required to Rack V2 accepted spec sections 11–24, 34+ and Decision D-064? A reviewed amendment is mandatory for conflicts.

## 5. Manager design direction after corrected Unity measurement — D-068

**Status: CURRENT DIRECTION, still DRAFT / NOT IMPLEMENTABLE.** The #248 Unity measurements and cross-provider corrections are described in [Manager measurement review](../../Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-MEASUREMENT-REVIEW-20261103.md). The corrected report resides in unmerged docs-only PR #249 at the review point; original and corrected data remain distinct.

- **Scope of "spine":** one continuous **existing central in-stand route PLUS a proposed northern continuation**, with planned diagonal extraction branches along both sections. The feeder spine is not one of the five extraction racks. **This is a planning definition, not evidence that the route is physically cleared or suitable for extraction machinery.**
- **Reference orientation:** search for **45° off geographic north**, not 45° off the curved route tangent. Use a two-sided **3+2** near-parallel arrangement as a test candidate and preserve the site-specific 16–20 m spacing recommendation. The sampled lower independent 45° best-case minimum is ~**18.274 m**, but that number does not qualify lengths, entry trees, stand impact or turning room. **Reject 40° as default** because its 16 mm threshold margin is too fragile. Do not infer continuously feasible angles from seven tested angles. 50° or 60° best-case distances may exceed 20 m.
- **Extension-only approach:** do not choose a forced five-rack extension-only system with the observed ~8.9 m best-case minimum spacing. The impossibility finding applies to the studied **two-sided parallel 3+2 layout**, not to every imaginable access design.
- **A1 and actual coverage:** do not implement additional `Dirt Path*` planks as "visual-only". They add MFM exclusion masks; hypothetical tested spine alone loses 18 treatable stems / 65.80 m², spine plus one branch 31 / 125.96 m². A coverage-neutral *proposal overlay* and an actual route must have expressly distinct, authoritative statuses and mask semantics before implementation.
- **Removal/forestry judgement:** 3–38 standing trees per branch corridor is **an evaluated consequence, not authorisation to fell**. Proposed rack geometry is not a Fell mark, confirmed entrance painting is not execution, and Work Plan approval remains the commitment gate. Do not add another removal system or alter MFM on a mere suggestion.
- **Meaning of "sensible":** the complete candidate must demonstrate actual usable rack lengths and widths, valid on-spine entrance targets, turning near the northern limit, conflict/overlap/boundary checks, explainable poor placement, realistic tree-removal cost, accessible in-forest marking, Stand Map/minimap/3D consistency, and preservation of old saves/Legacy40/frozen Reference Future v1. Current measurement **did not** render the maps and did not clear Rack V2 §39.6.1.

**Further work:** a separate read-only five-rack *reference-candidate* task and independent review are needed before the spec can be made APPROVED. Current Rack V2/D-064 controls all production. The Manager has not approved A1/A2 implementation or any Windows build.

## #250 evidence and explicit implementation STOP — latest controlling draft note

**Manager verdict:** [#250 Manager review](../../Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-FIVE-RACK-REVIEW-20261103.md) accepts corrected measurements **as research**, but **STOPS production**. The 45° full-spine 3W+2E reference candidate has same-side centreline spacings 16.623 / 17.510 / 17.855 m and an estimated **120 distinct standing trees** inside its proposed corridors (not Fell marks, not authorisation). It also has overlapping R1/R2 corridor widths, one endpoint in a trunk collider, insufficient proven turning clearance, a straight northern route running 0.035 m from the Forestry Workbench, five entrance points that are presently illegal under headland-only rack validation, and **no completed 3D/Stand Map/minimap rendered qualification**.

**Primary cross-system STOP:** the actual candidate's five racks make `ScenarioOneRackSampleGeometry.Derive` return **`Valid=false`** because the shared-normal projection cannot give R4/R5 a defensible order. This specifically blocks candidate-related MFM sample/coverage. Do not weaken derivation validity or fabricate a usable coverage result. The #250 hypothetical Track/Headland mask numbers are sensitivity evidence, **not a qualified MFM prescription**.

**Required next work #251:** [read-only MFM topology/sample/coverage design options](../../Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MFM-TOPOLOGY-OPTIONS-PACKET.md) comparing (A) deterministic branch/side-local sample coverage, (B) graph/sector spatial model, (C) a narrower rack topology retaining the present parallel-rack derivation. Decide whether one representative player-demonstrated 3D sample can legitimately inform both sides, preserve manual/Excluded/NotCovered states and ensure every stem/area is treated **at most once**. Protect D-064/D-065, MFM masks and Work Plan, confirmed/approved/executed state, old saves and frozen Reference Future v1. **This task does not authorise model changes**.

**Correction/provenance requirement:** raw JSON/reviewer figures must be re-extracted at publication, with keys and totals reconciled. #248 independent review verdict was corrected to **9 PASS / 10 corrections / 1 FAIL = 20**. #250 independent review is **7 PASS / 6 corrections / 2 FAIL = 15**. Earlier contradictory tallies and material errors remain disclosed in the handoffs and ledger.

## 6. Approval / stop conditions

**DRAFT only.** The #246 read-only reconnaissance has returned and is Manager-approved **as a source audit**, not as fit or MFM impact proof. #248 real Unity geometry/mask measurement remains blocked until the user's playtest and Unity lease gates clear. Overall Manager then revises `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, records any explicit decision supersession and obtains approval before production dispatch. Stop if the suggested spine/diagonals cannot fit real geometry, introduce unplanned old-save/schema changes, conflict with an active writer/Unity lease, or threaten protected reference results.
