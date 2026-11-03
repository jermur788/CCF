# CCF #246 — Access-spine & diagonal rack network read-only T3 search

**Date:** 2026-11-03. **Authority:** user's 3 November personal playtest preference; Decision Log D-067, a confirmed design direction, **not a production go-ahead**. Rack V2/D-064 remains the controlling implementation contract until approved spec amendment.  
**Claim:** https://github.com/jermur788/CCF/issues/246  
**Spec:** `specs/scenario-one-access-spine-route-consistency-v1/` — **DRAFT**, four-file structure (spec, plan, tasks, requirements checklist).  
**Difficulty:** **Medium** — real geometry and cross-system consequences cannot be resolved from a screenshot.  
**Best-suited:** OpenAI **T3 Code** with local CCF Git read-only access; Claude Sonnet or equivalent independent reviewer later.  
**Lane/branch/worktree:** read-only inspection of the published `integration/scenario-one-player-test-ready-v1` candidate through GitHub or an existing safe local Git worktree using `git show`/`git grep`; **no checkout of a user's active/dirty worktree**. No new branch/worktree is needed for inspection. If worker must write handoff, use an **isolated docs-only review branch** without touching #243's files; refresh active-tasks and register that ownership before write.  
**Output path:** `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/RECONNAISSANCE-HANDOFF.md` (GitHub Markdown required), link in #246 and mark [HANDED-OFF].

## User's chosen option

Use the existing central route, extended where needed, as a **spine/axis**, with **diagonal racks branching from it** (herringbone-like where coherent). The aim is consistent main-world routes, Stand Map, minimap and entrance/mark locations. Existing separate trails that look like formal paths must be mapped or cease looking like routes.

**Provisional Manager recommendation:** preserve the user's existing five-rack scenario target; treat the access spine as the **feeder connection**, **not automatically as rack #1**. Confirm actual meters/row orientation/geometry before any angled rack layout becomes authoritative. Both the count and 16–20 m guideline are Scenario One gameplay calibration, not universal Irish rules. Any conflict must be returned to Manager/user, not silently resolved.

## Tasks — inspection only

1. Inspect the exact published candidate branch and related Rack V2 implementation/spec, current 3D scene, path assets/logic, map and minimap code; report code paths and line-level evidence for where the existing central path starts/ends, any extra rendered 3D trails, and whether one authoritative route geometry exists. Identify what is **not provable without Unity runtime**.
2. Examine the authoring constraints that currently force rack starts to a stand edge or path/headland, move/rotate snapping, placement constraints, accepted five racks, entrance marking, one rack counter/state, and Work Plan approval. Pin the actual spec sections in conflict with proposed internal/disconnected rack drafts. Distinguish editable proposal, confirmed valid entrance, approved work and executed clearing.
3. Evaluate three layouts in a short trade-off table: **A** extend existing central path as access spine with diagonal branches (preferred); **B** leave footpath and build a separate mapped extraction spine; **C** retain current boundary/headland-only Rack V2. Prefer A unless specific observed geometry/access facts make another materially better.
4. Investigate the user's ability to reach the uppermost entrance, speed through tree stands, colliders and whether main-world lines/path textures correspond to actual traversable or planned geometry; no gameplay tuning without evidence.
5. Report smallest likely file ownership sets, save/persistence and MFM coverage implications, Legacy40/Reference Future v1 restrictions, and whether a Unity geometry measurement packet is essential. No invented path endpoints, angles, row widths, harvest costs or movement factors.

## Output standards / stop conditions

Deliver a **read-only evidence inventory and recommendation**, not a modified game. Separate user-confirmed intent, inspected repository fact, uncertain scene/runtime fact and design proposal. A dry source audit is allowed; **no Unity Editor launch, builds, file edits, worktree deletes, test reruns, lock interactions or active-save operations**. #243 previously reported a held Unity lease; current state must not be presumed FREE. The user's personal playtest continues and the published candidate must remain stable. #194 NO ACTION.

If code or scene evidence shows the chosen spine cannot support five diagonal rack placements, or if recon contradicts Rack V2 persistence/marking constraints, return `STOP — GEOMETRY/SPEC CONFLICT` with specifics; do not implement a workaround.

**Status discipline:** #246 is READY; the worker marks **[RUNNING]** only at actual start with model/provider, inspected published ref and lane; **[HANDED-OFF]** with published result on return. Overall Manager subsequently approves/revises the draft spec. This packet does **not** authorise AS-02 through AS-07.

## Copy/paste dispatch

> T3 Code: perform **read-only** Scenario One access-spine geometry and rack/network inspection from https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/RECONNAISSANCE-PACKET.md and claim https://github.com/jermur788/CCF/issues/246. The user prefers extending the existing central path as access spine, with diagonal extraction racks branching from it. Inspect the actual candidate, relevant Rack V2 rules, world/Stand Map/minimap route consistency, five-rack spacing and movement/entrance constraints. No Unity launch, no code edits, no lock interaction. Return a GitHub Markdown `RECONNAISSANCE-HANDOFF.md` with file/line evidence, alternatives and SPEC conflicts; mark [HANDED-OFF]. Do not claim implementation or verification.

**User action required to execute in local agent:** paste this dispatch into T3 Code with read access to the CCF repo. **No implementation approval needed or implied.**