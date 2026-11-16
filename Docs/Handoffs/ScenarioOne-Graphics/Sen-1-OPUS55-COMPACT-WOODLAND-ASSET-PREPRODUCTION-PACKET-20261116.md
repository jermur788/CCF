# Sen-1 — Claude Opus 5.5 compact woodland asset preproduction and look-development packet

**Date:** 16 November 2026. **Parent manager:** [Scenario One implementation #424](https://github.com/jermur788/CCF/issues/424).
**Execution claim:** [#577 — Opus 5.5 compact woodland asset preproduction](https://github.com/jermur788/CCF/issues/577).
**Status:** **READY / NOT RUNNING**. The Manager has published this handoff only; no Opus agent has actually accepted or started it, and no production/Unity authority is conferred.
**Difficulty:** **HIGH**: quality-sensitive ground/vegetation/3D scene look-dev, uncertain local Asset Store licenses and gitignored assets, and renderer/material/URP/world-state coupling.
**Best-suited model/provider:** **Claude Opus 5.5 High in Claude Code/T3 Code**, if actually available. Do not misreport provider/effort. Use independent separate nonauthor review after the returned result; author may not approve its own release.
**Lane:** local code-capable **asset investigation and isolated scratch look-development**, NOT a Unity/source integration lane. Worker creates an exclusive docs-only Git branch `docs/sen-1-compact-asset-lookdev-opus55-20261116-v1` from freshly inspected `main`, and a **new worktree whose actual path must be chosen only after inspecting all live worktrees and the physical board**. Do not assume a prechosen path is free. Scratch assets must be isolated **outside the Git working tree or under confirmed untracked temporary location**, not in the actual game project's `Assets/`.
**User action to start:** paste the dispatch line at the end into a new Claude Opus 5.5 code-capable chat. For this first preproduction phase, no design decision or Unity approval is required; real production integration is separately gated.

## 1. Product target — not the old five-rack world

The owner **ratified 30 × 30 m** as Scenario One's **first-beta** site on 16 November. [Binding owner-ratified baseline](https://github.com/jermur788/CCF/blob/docs/sen-1-owner-30m-baseline-ratification-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-OWNER-RATIFIED-30M-NO-RACK-BETA-DESIGN-BASELINE-20261116.md) and [browser canonical/feature closure #576](https://github.com/jermur788/CCF/blob/docs/sen-1-576-browser-handoff-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-BROWSER-COMPACT-FIRST-BETA-CANONICAL-SPEC-CLOSURE-PACKET-20261116.md) are prerequisites to read.

The player enters a finite **all-Sitka 30 m woodland** at a **pre-existing ~12 × 10 m H1 entrance/parking clearing**, then follows **one winding pedestrian-only woodland path**, with **no internal extraction rack**. Occasional small-truck timber collection is a **staged event**, not a driveable path, turning-sweep simulator or permanent truck parked among visitors. The first taught planting activity is **two sessile Oak and two European Beech** from nursery stock near a source-screened G1 site. Later player decisions include canopy/shelter/browsing and annual management of the same living world.

Source study used **189 original Sitka before site-layout exclusions**. The proposed pre-existing clearing intersects **14** starting stems; **175** is a *conditional derived result* until the exact tree-ID roster and world manifest are signed and proven. An asset worker must NEVER move/delete/add authored simulated trees, invent G1 coordinates, change stands or interpret a visual gap as an executed Fell mark. The old enlarged five-rack/Legacy40 worlds, Rack V2/MFM and frozen Reference Future v1 are protected regression surfaces, not the new beta art target.

**Visual goal:** credible first-person Irish Sitka woodland at human scale with clear walkability, readable forestry actions, distinct path/forest floor, grounded planted broadleaves, convincing modest biological variation, and no overpromised photorealism or external donor ecology.

## 2. Prior art investigation — read before suggesting any asset write

- [Original Claude Opus source-graded Scenario One graphics audit](https://github.com/jermur788/CCF/blob/docs/sen-1-graphics-parity-audit-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-SCENARIO-ONE-GRAPHICS-PARITY-AUDIT-AND-SPEC-HANDOFF.md). **Historical E80 visual audit**, not proof of compact render state.
- [Independent #573 graphics spec/reference review](https://github.com/jermur788/CCF/blob/review/sen-1-ground-graphics-spec-independent-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md) returned **NEEDS CHANGES — 0 BLOCKER / 7 MAJOR / 9 MINOR / 2 NIT**. Specific risks: shared `VegetationSites` also determines clearance targets, old track renderer/material clones drive headland and approach, shared ground/path materials are rewritten by Editor menu, scene camera has Solid Colour clear/no serialized additional URP camera data, high-surface-coverage cells may have no viable visible vegetation placement, graphics baseline lacks approved gate, locally gitignored Asset Store pack can make visuals unreproducible. **None** of this is permission to edit/retire those systems.
- [Earlier E80-only 4-file DRAFT ground graphics spec](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/specs/scenario-one-ground-graphics-parity-v1/spec.md). It is **not APPROVED**, and its 52 path planks, western headland/southern feeder, E80 spawn/junction test cameras and +15% performance suggestion are **not the compact world**. Useful general art lessons should be proposed as a **new compact-specific spec**, not silently copied as geometry requirements.
- `main:Docs/Project/decision-log.md` **D-032 OPEN**: large PNG/FBX/Blender/Git LFS policy undecided; **no new binary import/commit is authorised** by this packet. **D-045 CONFIRMED** accepts the older presentation/track distinction; do not describe its look as a bug without noting product scope has changed.
- `Docs/Project/unity-project-overview.md`: URP pipeline, `Assets/Scenes/ForestryAssetReview.unity` art/LOD review scene, existing Plantation Sitka 02 tree variants, Oak/Beech juvenile visuals, moss/brash/understorey, historical performance. It is fallback descriptive evidence, not current Editor verification.
- Read-only source inspection of `Assets/ForestPrototype/Art/`, `Assets/ForestPrototype/ScenarioHabitatVisuals.cs`, current materials/prefabs and `Assets/Scenes/ForestTest.unity` **is allowed** only in the isolated existing worktree without modification. **Do not open Unity** and do not infer the true new compact source hierarchy from the E80 scene. Look for existing art inventory, mesh LODs, impostors, materials, texture sizes and duplicates.
- Some earlier art uses a **gitignored local Asset Store pack**; record actual file presence and licence/provenance **without distributing, copying, uploading, or using it as an unacknowledged mandatory asset**. Mark local-only versus tracked assets separately.

## 3. Phase A — START NOW: actual asset inventory and original look-development

**A-00 Safety/registration first.** Read `main:Docs/Project/coordination/active-tasks.md` and the physical CCF-Ops board **read-only**, list Git worktrees, current branch/status/source provenance, available disk capacity, and check no overlap with the browser #576 canonical/spec owner or #544 save writer. Protect the user's dirty Primary `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` exactly, do not stash/discard/clean anyone else's changes. On claim #577, register **[RUNNING]** with real model/provider/effort, branch, isolated worktree, clean starting HEAD, exact doc write allowlist and scratch path. If no safe local lane or provider time is available, mark **[BLOCKED]** and publish a concise GitHub result; never borrow another owner’s lane or Unity lock.

**A-01 Reuse versus remake.** Inspect what genuinely exists, its quality/performance purpose, source/repo status, provenance and possible use for **this 30m world**. Inventory at least:
1. Sitka spruce trunk/branches/crowns/pole age and existing LOD variants; identify whether entirely new tree meshes are actually needed (likely **reuse before remake**, but verify).
2. Seedling/juvenile sessile Oak and Beech visuals, nursery stock, planting/shelters and later growth-state presentation, preserving actual species and tree IDs.
3. Forest soil/needle-litter, modest moss, twig/brash/deadwood dressing, realistic edge/ground textures and normal/roughness availability.
4. Entrance/parking H1 surface, vegetated edge blending, believable but understated collection-day staging **without** an internal road/truck network.
5. **Winding narrow pedestrian footpath** surface/meshes/materials; must be readable as a footpath rather than 2.6m extraction track or motorway.
6. Understorey (fern, bracken, grass, bramble, rush etc) and vegetation exclusion near built H1/path surfaces, with distinct physical display versus underlying ecology/clearance-gameplay objects.
7. Background stand edge and skyline, lighting/sky/material compatibility; off-property fields/hedges are a **visual option** not assumed donor trees or simulation.

For each asset family record: original path or source, is tracked in Git or gitignored/local-only, ownership/licence, expected transforms/scale/LOD count, render/material dependency and whether reuse is safe **without source modifications**. If exact item unreadable, classify **UNVERIFIED**. Never copy Asset Store binary files or user photographs to GitHub.

**A-02 Prioritised asset bill of materials.** Produce **P0 compact beta look-dev shortlist**, ordered:
- **P0-1 H1 entrance clearing + forest-floor edge:** natural readable walk-on transition, convincing ground soil/needle litter and modest existing-access appearance;
- **P0-2 one winding footpath:** human-scale, not an extraction rack; coherent surfacing/edge softness/wayfinding;
- **P0-3 Sitka stand floor + understorey dressing:** sparse where appropriate, not grid-looking, not on built surfaces; visual-only occupancy separate from actual ecological and clearance sites;
- **P0-4 first Oak/Beech planting readability:** species-distinguishable juvenile/shelter variants and planting context without fake mature natural broadleaf cohorts;
- **P0-5 restrained lighting/background:** separately proposed, depends on actual compact camera/URP configuration and owner look choice;
- **P1 later-year diversity and forestry event dressing** if only after these do not violate scope/performance.

For every proposal include **make/reuse/defer**, approximate model/texture count, LOD/mobile/low-end PC considerations, per-asset source/licence, expected material complexity, likely interaction with exact world manifest, missing source/Unity evidence, suggested acceptance screenshot/state and a reasonable **proposed** footprint (NOT an approved budget).

**A-03 Non-destructive original prototype work is ALLOWED in isolated scratch, if actually feasible.** Opus may build **small representative original mesh/material look-dev concepts** using already installed offline tools (for example Blender or pure procedural geometry) **outside the game's repository**, strictly as **UNAPPROVED PREVIEWS**. Focus on **one entrance-to-footpath-ground junction and one juvenile planting/shelter vignette** to test look, rather than a whole speculative forest. Save editable prototype source plus previews **only in its separate, documented scratch/evidence directory**, with source/creation hashes and explicit original/provenance labels. Do not claim any new images/meshes are imported, licensed game production assets, real scene captures or Unity-tested. Never pull dependencies/install software/scrape image files or copy third-party binaries without explicit permission. If tool availability, rights or time prevents prototypes, complete inventory and detailed asset bill instead; **do not improvise unverified Unity screenshots**.

**A-04 Two clearly differentiated visual directions**, for first owner look comparison only:
- **Direction W — worn mineral footpath:** low-saturation earth with needles embedded, subtly cleared central walking line and soft edges. Forestry-road reference material is not an approved road surface for a pedestrian path.
- **Direction N — needle-litter woodland walk:** less visibly constructed footpath, narrow compacted line through needle duff and moss, legible at first-person scale.
Do **not** declare one owner-approved. For each state how it contrasts with surrounding floor (including grayscale), handles wet/shaded Irish atmosphere without false species composition, and where groundcover display is excluded. No unnecessary road engineering or truck-turning study.

## 4. Hard separation: not allowed in Phase A

**NO production asset/game mutation:** do not write, add, stage or commit anything under `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `CCF-Ops/` or `specs/`; do not modify `ForestTest.unity`, `ForestryAssetReview.unity`, shared `Forest Ground.mat` / `Forest Path.mat`, Editor `ScenarioOneFloorPresentationSetup`, runtime camera/URP assets, 52 old track objects, old legacy worlds, save/router, exact trees or scene hierarchy. Do not run material-regeneration menus. Do not change tree placement, species identity, H1/footpath topology, ecological density, `VegetationSites`, clearance preview, growth, harvesting or planting. No compiled production changes or `.meta` files.

**NO new binaries in Git:** no PNG/JPG/FBX/BLEND/external texture/Asset Store package/Git LFS files committed until the owner/portfolio Manager resolves **D-032**, licence use, storage budget and the compact-specific graphics spec has been independently reviewed and marked **APPROVED**. Browser-only #576 may be drafting that new world, not a Unity handoff. Temporary scratch prototypes do not authorise production binaries and must not be treated as distributable assets.

**NO Unity or heavy machine-wide Editor:** no Editor launch, import, URP toggles, runtime changes, new screenshots, tests, benchmark or retained Library writes. The physically shared Unity lock remains independent, and the no-Windows-build rule is still binding. Source-only analysis should not be labelled runtime proof.

## 5. Mandatory blockers to production transfer — Stage B is NOT authorised here

Before commissioning a **separate production asset/Unity integration handoff**, #424 must have:
1. #576 final canonical and specs closure, **approved compact world manifest** with precise source tree omit identities and G1 positions, a genuine no-rack art footprint; approved parent/child **four-file** compact specs and first-use/finance/teaching integration requirements.
2. A **new compact graphics/asset four-file SPEC REQUIRED**, resolving #573 source couplings (display-only vegetation mask; no gameplay clearance mutation; material/approach clones; inherited Legacy40 materials and Editor writers; camera solid-colour/URP/fog feasibility; high-covered cells; gitignored assets). Do not simply run the earlier E80-only DRAFT.
3. **D-032 owner storage/binary policy** and a record of licences/source and texture/mesh byte footprint. Do not invent a 4×2048²/60MB allowance; that was unaccepted E80 audit suggestion. Owner review of Direction W/N plus broader lighting/fog and exterior-scene choices.
4. A fresh true **Unity compact baseline** (look from actual H1, first footpath bend, G1, understory and later-year view) under its own safe lease, and a same-machine performance measurement at native 1280×720 and 1920×1080, with LOD/quality and HUD state pinned. Historical E80 ~40ms is NOT compact baseline. Any performance threshold must be approved on that actual hardware and scene.
5. A dedicated **asset source writer/Unity lane** and a separately claimed **local integrator**, with exact per-file allowlist, physical board and clean worktree, zero overlap with save/Objective1/compact owner, and independently reviewed build/QA. Later 3D visuals cannot count as gameplay or ecological changes.

**STOP rather than bypass** if the actual scene cannot support a proposed visual without editing gameplay/renderer/URP shared code, required local art is unlicensed/untracked, or the owner does not approve a look. Submit a precise proposed integration API/file-set packet and a *NEEDS DECISION / BLOCKED* label, not a speculative C# workaround.

## 6. Exact GitHub deliverable, review and acceptance

**Only permitted tracked write by this Opus task:** one Markdown handoff on the worker’s own docs-only branch:

`Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-ASSET-PREPRODUCTION-RESULT-20261116.md`.

It must include:
- Actual worker identity/model, tool availability, original `main` branch/HEAD, new branch/worktree, scratch path, any physical board owner checks; if absent classify not checked;
- Source-backed inventory table with actual **Git-tracked / local-only / missing** distinction and per-item licence provenance;
- Priority **P0/P1 asset kit**, reuse/make/defer, estimated size and performance risks, concrete **H1/path/G1** visual target, sky/understorey concerns and honest ecology/provenance safeguards;
- Optional original scratch prototype asset source paths and file hashes/preview evidence **not committed to Git**, with licence and creation notes; explain if no local prototype possible;
- Two alternative W/N footpath looks, proposed review still pending owner acceptance; 3D look is not native compact proof;
- Full list of **#573 7 major issues** showing whether each is a blocker/owner decision/non-applicable to compact (with evidence), rather than silently omitting any;
- Every outstanding gate before production and exact suggested *next* spec/asset integration handoff with **status BLOCKED UNTIL APPROVED**;
- Verified **GitHub base-to-head compare: exactly ONE added Markdown**. Do not weaken/erase any existing test or change another file.
- Report **NO Unity / NO production asset commit / NO save/Prefs / NO Windows** and **Dashboard/Drive sync UNVERIFIED** unless independently performed.

At handoff post **[HANDED-OFF]** on #577 with the result GitHub URL, mark claim closed, and notify #424. Manager inspects and issues **APPROVED AS PREPRODUCTION INPUT / NEEDS CHANGES / REJECTED**; such approval **does not automatically approve asset production**. A separately registered nonauthor check can be requested for provenance/high-risk original-assets eligibility. Never present a mere reviewer as user visual acceptance.

## 7. Exact paste-ready dispatch

> Take ownership of CCF Scenario One asset preproduction issue #577 as **Claude Opus 5.5 High in Claude Code**. Follow the COMPLETE GitHub handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-577-opus55-compact-asset-handoff-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-WOODLAND-ASSET-PREPRODUCTION-PACKET-20261116.md . The owner has ratified the **30×30 m no-rack** small Irish Sitka woodland, with H1 entrance clearing, winding pedestrian path and G1 2 Oak+2 Beech. Register actual provider/model, dedicated Git docs branch, isolated worktree, starting HEAD and scratch area on #577 before work. Inspect the existing reusable art/LOD/licences and the #573 NEEDS CHANGES audit; produce a prioritised compact asset kit and **optional original scratch-only editable look-dev prototypes** for entrance/path/forest floor and juvenile planting. Publish the specified ONE GitHub Markdown result. **No Unity, game source, Assets/ changes, binary commits, scene/material edits, saves, Ops/board writes or Windows build.** Do not turn DRAFT graphics, D-032 or #576 drafts into implementation permission.

**Current owner action:** Paste this dispatch into the desired **Opus 5.5** local Claude Code chat. No additional design decision is required for **preproduction**, but owner look/asset storage choices are needed before tracked game asset integration. No background worker is currently running on #577.
