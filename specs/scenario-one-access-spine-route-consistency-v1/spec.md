# Scenario One Access Spine & Diagonal Rack Network V1

**Status: DRAFT — NOT APPROVED FOR IMPLEMENTATION.**  
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

## 5. Approval / stop conditions

**DRAFT only.** The #246 read-only reconnaissance has returned and is Manager-approved **as a source audit**, not as fit or MFM impact proof. #248 real Unity geometry/mask measurement remains blocked until the user's playtest and Unity lease gates clear. Overall Manager then revises `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, records any explicit decision supersession and obtains approval before production dispatch. Stop if the suggested spine/diagonals cannot fit real geometry, introduce unplanned old-save/schema changes, conflict with an active writer/Unity lease, or threaten protected reference results.
