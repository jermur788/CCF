# scen1 — T3 Code #508: offline entrance-clearing, winding-path and first-planting spatial feasibility

**Issued:** 14 November 2026 UTC. **Worker task:** [#508 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/508). **Author:** Scenario One Manager #424, OpenAI GPT-6, GitHub/browser-only. **Status:** this packet supersedes the earlier #507 no-rack-only eight-site packet **before worker registration**. It does not authorise production implementation or Unity.
**Difficulty:** MEDIUM–HIGH — original 30m raw tree evidence, source-equivalent spatial filters, starting-history implications, and identifying unknown road/vehicle limitations, all with **NO Unity**.  
**Best-suited worker/provider:** local T3 Code / GPT-6.1 Sol High (Codex) or capable Claude Code with access to the original host evidence. Actual worker registers its real provider/model; no model receives implicit ownership.
**Research branch:** `research/scen1-eight-broadleaf-eligibility-20261114-v1` from freshly inspected `main` only if free; **conditional SSD worktree** `/media/jer/ZX20/CCF-scen1-PlantingEligibility-20261114` if free/safe, or worker reports alternative before edits. Worktree/branch never silently reused.  
**Only writable TRACKED output:** `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-TIER1-30M-4PLUS4-PLANTING-SITE-ELIGIBILITY-RESULT-20261114.md` (existing specified #508 name; contents MUST cover entrance/path as well as plants). No other tracked files; no source, `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, canonical decision docs, specs, coordination, root `AGENTS.md`, personal saves or Unity changes. Optional disposable calculations stay untracked in separate new private scratch location with method/hash reported.  
**Read-only owner spec:** [four-file scen1 entrance+path child DRAFT](https://github.com/jermur788/CCF/blob/docs/scen1-entrance-clearing-winding-path-20261114-v1/specs/scenario-one-compact-initial-planting-v1/spec.md). Previous parent compact complete-loop DRAFT still assumed one extraction rack; its resolution is a Manager product review gate, not worker authority.
**Original evidence:** `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/` — READ ONLY. Pinned `native-result.json` hash `58524ad4f634378a875313909bb5cf42cfb37468abe0e60a61b63ce00a51de40`; `executed-harness.cs` hash `3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863`; inspect `C-B-30.json`, manifest and raw original stage provenance before calculating. The known `closure-receipt.json` manifest discrepancy is post-run and must be explained, not assumed input.
**No Unity executable, Editor/import, no source/FX or cash work, no Windows build.**

## 1. Owner-accepted purpose and critical distinction

The owner has said that the compact forest can have **a clearing at the entrance where a truck can come in to take timber away and cars can park**, followed by **a simple winding path through the plot**. The earlier no-mandatory-extraction-rack concept remains. This is a **concept** for an initial environment, not a measured entrance approval or vehicle type.

Manager's provisional design-screening defaults remain 30×30m bounded all-Sitka woodland and **first player-led planting** target of four sessile Oak + four Beech in two mixed groups. The owner did NOT specify entrance dimensions, vehicle size, road/turning rights, route width, actual planting coordinates or an authorised 30m production profile.

The new **pre-existing entrance clearing and pedestrian path** likely change which original Sitka can be present at time-zero. Therefore **#460 Year0 C-B-30 189 original trees are the UNMODIFIED PRE-LAYOUT REFERENCE, not necessarily the final opening/path scene inventory.** **Never assert both 189 unchanged trees and cleared entrance/path unless genuine spatial evidence supports it.** Do not silently remove source trees or call them Year1 player-marked fells. If this requires an authored starting-generation exclusion/previous-history event, report it as a proposed **spec-required policy**, not code authority.

**Timber truck versus cars:** a safe truck entrance, loading and turn/exit need an appropriate ground footprint and off-road permitted facilities. The clearing can support cars during ordinary visits, but turning/loading must be **kept free of parked cars and visitors when haulage occurs**. A standard articulated timber truck may be infeasible in 900m²; do not pretend that a smaller collection vehicle is the same as a full-size lorry. A rural roadside edge is not evidence of lawful public-road loading, land rights or approved entrance.

Primary planning references for worker interpretation:
- [Teagasc: Prepare for Harvesting Operations](https://teagasc.ie/crops/forestry/advice/timber-harvesting/harvesting-and-selling-timber-from-conifer-forests/prepare-for-harvesting-operations/) — safe access, loading, turn and exit.
- [Teagasc: Construction of forest roads](https://teagasc.ie/crops/forestry/advice/management/construction-of-forest-roads/) — some small sites use only a suitable bellmouth, but loading on public roads is not permitted.
- [Coillte: Code of Practice for Timber Removals](https://www.coillte.ie/driverportal/docs/Coillte-Code-of-Practice-for-Timber-Removals_2023.pdf) — keep turn/loading areas free of parked vehicles.

These do **not** provide our chosen 30m site's real road, permission, terrain, vehicle turning simulation or cleared entrance dimensions. Label each such field UNKNOWN unless a directly relevant source/evidence exists.

## 2. Local worker safety and data gate before analysis

1. Read current root `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`, `unity-project-overview.md`, **LIVE** `Docs/Project/coordination/active-tasks.md`, claim #508, original #460 report, #484 finite Hegyi result, #490 long-run report and this updated DRAFT.
2. Before any tracked edit **register on #508 actual provider/model, branch, starting full HEAD, actual worktree or none, exact single-file output and OFFLINE ONLY / NO UNITY LEASE**. Resolve any existing owner/dirty worktree overlap first; claim doesn't grant edit ownership or Unity access.
3. Verify original raw evidence names, manifest, hashes and stage. Use actual 30×30m **YEAR0 pre-#460 rack** source-generated trees/coordinates, IDs/live status, available terrain/cell/road/entry fields. Do NOT subtract the 19 source access fells from #460 or use the #490 post-rack two Year2 Oak/Beech spots as new design proof.
4. If original source tree positions/stand bounds are not accessible or inputs differ, STOP DATA GATE, publish exact missing fields and source needed; **do not regenerate artificial data or launch Unity**. Do not edit original evidence.
5. Inspect actual `ForestStartingStand` generator/omission rules, `ForestEcologyController` cells, `ScenarioOneManager.TryDesignateExactPlanting` (supported stock, exact marker uniqueness, valid cell and **minimum 0.7m from each living stem at designation**), and relevant boundary/road/stand-map source. Source-only constraints are **not** proof of road rights, route navigation, slope or timber loading.

## 3. Stage A — entrance-clearing footprint: source-screening, not road approval

Use only explicit **hypothetical footprint sketches/parameters** with known assumptions, rather than inventing a road location or asserting a safe truck turn:
- Locate actual source-evidenced public-road connection or existing entry only if present. If absent, say **ROAD CONNECTION UNKNOWN** and analyse up to an abstract selected boundary edge clearly labelled Manager/worker hypothetical.
- For one, at most two simple candidate entrance-clearance footprints that could plausibly be near the parcel edge, calculate overlapping original Sitka IDs and canopy/stand area left, based on real source X/Z. **This is not permission to fell those trees**; list them as pre-existing opening omissions *if* later authorised.
- State the assumed footprint geometry, available parking/staging areas and **do not claim truck manoeuvre/swept turning, stack loading, surface drainage, civil/forestry road approvals, public-road sightlines or legal parking** without source. Keep visitor car parking away from truck loading/turning by time/operational closure; geometry cannot certify site safety.
- If the constraints suggest a proper truck cannot enter, mark **VEHICLE OPERATIONS UNQUALIFIED / NEEDS REDESIGN**, and propose the smallest realistic design change: perhaps an actually permitted roadside/off-plot apron, appropriately sized collection vehicle subject to owner decision, or a larger parcel. Do not place part of the yard beyond legal land without stating unapproved permission.
- A Teagasc entrance standard may refer to substantially longer setback/turn lengths for articulated lorries. Do not turn such numbers into an invented entrance footprint that silently consumes most of 30m; this is a stop/decision gate.

## 4. Stage B — winding pedestrian path source-screening

From the proposed entrance footprint, consider **one simple winding path** through genuine retained Sitka with two inspectable planting areas. This is a **people-only** route, not a forestry rack:
- If ground walkability is unmeasured, propose only a candidate 2D polyline with explicit path-width UNKNOWN (or marked Manager calibration for a test, not law). Do not claim 3D Unity collider/terrain/slope validation.
- Give a source-coordinate/segment table, original living-tree trunk proximity, exact Sitka IDs colliding with tentative path clearance, and path continuation to first planting groups. **Preserve** nonintersecting trees; don't turn a path into a disguised straight 2.4m through-rack.
- Compare a curving route that avoids existing trunks versus one that needs minor historical clearing, but don't force either outcome. If source positions are too dense to provide useful real human access, stop at NEEDS CHANGES with available options.
- Player testing must later verify navigability, camera sightlines, and whether the route makes Oak/Beech findable and the plot feel woodland rather than a yard.

## 5. Stage C — original-source eight-sapling eligibility AFTER entry/path

Check 4 sessile Oak +4 Beech in **two accessible mixed groups** of two each only after explicitly recording the entrance/path conflict set. For every candidate: actual x/z, source ecology cell, side/area, original living nearest Sitka ID and distance (treat authorised proposed historical exclusions separately), nearest new sapling, actual planning spacing, accessible footpath distance if derivable, known shade/ground/browse evidence or **NOT MEASURED**. The **0.7m existing living-tree guard** is source admission only; **~2m between NEW saplings** is a gameplay clarity preference, not Irish practice.

Do not invent planting stock or claim a Pending marker creates a planted juvenile. Source current game calibration: 4 Oak stock €22 plus 4 Beech €18 = **€40 stock only**; eight optional €5 shelters = **€40 material**, before real owner/contractor/harvest work. Do not treat these as real Irish retail rates or grant reimbursement; existing full-scale job fee does not automatically apply to small site planting.

**Negative checks:** potential tree collision under clearing/path; original unchanged 189 contradicted by clearing; edge/road overlap; public-road loading assumption; vehicle and visitor overlap during haulage; path crossing a living stem without approved omission; site within entrance or log stack; exact planting mark <0.7m from live remaining tree; duplicate site, unowned/insufficient stock or unknown growth/light counted as PASS. Report results with clear labels **DIRECT ORIGINAL SOURCE**, **SOURCE-EQUIVALENT COMPUTED**, **MANAGER CALIBRATION**, **UNKNOWN/NOT MEASURED**, **REQUIRES NATIVE**, or **REQUIRES OWNER/SAFETY APPROVAL**.

## 6. Required result, exit and handoff

One **NEW GitHub Markdown only**:
`Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-TIER1-30M-4PLUS4-PLANTING-SITE-ELIGIBILITY-RESULT-20261114.md`

Include full original checksum provenance, exact source code semantics, at most 2 entrance footprint hypotheses, suggested path polyline if grounded, full intersecting Sitka ID ledger (or exact unavailable fields), 4+4 possible planting sites/negative controls or an explicit block, area/cash inference and crucial gaps **separating spatial site eligibility from real vehicle operation, route walkability, biological survival and owner approval**. No off-road vehicle lorry-turning PASS purely from occupancy.

Verdict must be **APPROVED FOR FURTHER DESIGN ONLY / NEEDS CHANGES / BLOCKED-DATA GATE**; all physical/Unity, old saved world, full 40-year growth, historical Reference Future replay, Windows build remain NOT RUN unless later separately claimed. Explain whether a compact 30m site remains advisable given the loading/parking/path trade-off and what smallest fallback the Manager should consider.

At completion, read back report from GitHub, compare branch to actual registered starting HEAD for **exactly one new Markdown and no other modified file**, mark #508 **[HANDED-OFF]** / close and notify Manager #424 with the direct GitHub blob URL. User action is to dispatch this packet to a local T3 Code chat; no actual worker starts by reading a claim.

## Paste-ready dispatch

> Take ownership of [CCF #508](https://github.com/jermur788/CCF/issues/508) as a **local T3 Code offline original-data feasibility worker**. Follow the COMPLETE updated `scen1` packet: https://github.com/jermur788/CCF/blob/docs/scen1-entrance-clearing-winding-path-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-T3CODE-TIER1-ENTRANCE-PATH-PLANTING-FOOTPRINT-PACKET-20261114.md. Register actual provider/model, fresh starting HEAD, `research/scen1-eight-broadleaf-eligibility-20261114-v1` branch and safe worktree before editing. Using original #460 30m **189 Sitka PRE-LAYOUT positions**, evaluate a hypothetical pre-existing **boundary entrance clearing** for timed car parking/timber truck collection, a **simple winding pedestrian path**, and two mixed planting groups totalling **4 Oak+4 Beech**. Identify real Sitka IDs intersecting entrance/path, remaining genuine eligible planting positions, source 0.7m live-stem and stock rules. DO NOT claim an engineered truck turn or approved road access from a 2D sketch; clearly mark unknown vehicle rights, off-road loading, pedestrian/Unity walkability, canopy/light and impact on actual starting-tree count. If original data or 30m layout is incompatible, report DATA GATE/NEEDS CHANGES and the smallest defensible alternative. NO Unity, source edits, original evidence changes, saves, canonical files, coordination edits or Windows build. Publish ONLY the exact specified new one-file `scen1` result, GitHub readback/diff, close #508 HANDED-OFF and notify #424.
