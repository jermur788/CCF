# CCF #888 — Compact30 ground-level asset overhaul — DRAFT visual production plan

**Date:** 2026-11-20. **Owner first-hand priority:** ground-level assets look markedly worse than trees; substantial improvement requested. **Manager:** #887 ChatGPT GPT-6, GitHub browser-only 3D Asset Manager. **Issue:** https://github.com/jermur788/CCF/issues/888. **Status:** DRAFT PLAN / NOT APPROVED SPEC / NO NATIVE PRODUCTION. **Task class:** SPEC REQUIRED for substantial multi-asset visual replacement. **Reference gameplay candidate:** #876 `fce41bd3b77a054bf82d2521648d15ec18f81141` until #886 owner-first collision repair produces a new separate candidate. **Main docs snapshot at planning:** `c94f2a2b0dee439cbe2a1269fc47d7421a3e1be2`. Preserve D-083/D-084/D-085 Compact30 geometry and D-032 art-binary/licence OPEN gate.

## Outcome / minimum genuine visual demonstration

One camera-matched, physically walkable 5×5 m (or similarly bounded) **in-situ forest-floor prototype** under real Compact30 Sitka canopy, plus matched present-candidate baseline, capable of demonstrating:
1. close-up ground substrate no longer reads as one flat uniform texture;
2. at 1.65 m eye height and 2–10 m view, broken needle litter, micro-debris, woody fragments and moss patches have believable scale, colour and edge blending;
3. dense plantation shade remains biologically plausible—sparse living understorey under closed canopy and richer pockets only where ecological/light state supports them;
4. H1 existing clearing, footpath and G1 planting sites remain visually/physically recognisable without imported obstacles, with no new world topography or path width;
5. benchmark recorded GPU/CPU/drawcall/triangle/overdraw and capture evidence; accept only improved visuals with no obvious flicker, tiling, vegetation floating, normal errors or hidden gameplay markers.

Owner should judge **before** mass rollout across 0.09 ha. A screenshot/mockup or art tool preview is not a qualified Unity in-scene prototype.

## Current inspectable repo starting point (not a claim of actual rendered appearance)

- `Assets/ForestPrototype/Materials/Forest Ground.mat` and authored scene `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` exist. Scene object/material assignments, lighting and placement should be physically inspected in Unity; browser GitHub file inventory is not proof of their active look.
- `Assets/ForestPrototype/ScenarioOne/RecentAssetVisualCatalog.cs`: arrays for grass/rush, groundMoss, and floor-dressing positions.
- `Assets/ForestPrototype/ScenarioOne/SectionFiveVisualCatalog.cs`: Oak/Beech/mixed leaf litter, small deadwood, shade fern, bracken, bramble, moss-on-wood, log/stump.
- `Assets/ForestPrototype/Prefabs/Forestry/SectionFive/`: Ground_Moss_Patch_01, Ground_Moss_ShootMat_03, Woodland_Grass_Tuft_01..03, Woodland_Rush_Clump_01..02, leaf litter patches, bramble/bracken/fern, two small deadwood scatters, and related prefabs.
- `Assets/ForestPrototype/Prefabs/Forestry/Ground/`: dry/green Sitka brash and windthrow base. Historical `Assets/ForestPrototype/Docs/SectionFiveIntegration.md` reports 13 3-LOD prefabs and rendering tests in earlier scenes, but **not Compact30 owner acceptance**.
- `Docs/Research/ScenarioOneAssets/UnderstoreyAssetAudit.md` distinguishes habitat proxy displays from causal competition; flags moss shoot-mat ~59,954 all-LOD triangles as potential performance problem and advocates reused silhouette sets and bounded seeded density. Treat this as older evidence, not latest scene measurement.
- `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json` has historical PROVISIONAL E1 metadata: flat y=0, H1 worn earth, 1.2m path core / 0.20m visual-only verges and four G1 locations. **Do not promote its unsigned E1 coordinates over #876 actual playable world or accepted D-085.** Re-inspect exact scene and signed source lineage before work.
- The owner judges **all visible ground assets** very poor relative to trees; do not assert from code alone that any one material/shader is the cause. Capture real problem areas.

## Five ordered work packages (each distinct acceptance)

### A — Scene baseline + art reference (P0, browser research and one legal native capture packet)

At exact approved candidate, authorised native worker records 6–10 consistent player-eye before captures including (i) H1 arrival and road/path transition, (ii) dark interior, (iii) lighter gap, (iv) tree-base detail, (v) litter and brash, (vi) juvenile planting/moss/vegetation close-up. Capture scene object/material names, shader maps/alpha, texture sizes, actual ground orientation, vegetation placements/counts, current GPU frame time/drawcalls/triangle stats. Snapshot copyright/licence provenance for existing ground assets. A single screenshot set and inventory, **no production edits**.

Compare visual references from Irish Sitka plantations and continuous-cover situations (closed canopy needle carpet versus small, lighter gaps); avoid idealised lush temperate rainforest on a dense all-Sitka Year-0 stand. Distinguish reference photo as reference, not empirical game parameter. Quality categories: **surface substrate, micro-detail, midground clusters, deadwood/interaction, light/contact**. Ask owner to choose a visual target using in-Unity captures/reference comparison.

### B — Foundation substrate materials (first actual art prototype)

Create one cohesive tiled/anti-tiling material ensemble with **Sitka needle duff** as dominant, muted exposed earth, dark decomposed organic material, moss/lichen where suitable, worn H1 earth and compacted path. Preserve flat ground collider and exact geometry. Build PBR albedo, normal, roughness/smoothness, optional height/ambient occlusion where justified; true scale: individual needles/twigs should not look palm-sized. Reduce visible grid tiling via macro colour/noise/rotation or controlled blended masks. Keep shader complexity bounded for Unity 6000.6.0f1 URP 17.6.0. Prefer adapting existing ground mesh/material rather than replacing it with a large Unity Terrain without justification. Work must not alter path footprint, physical clearance, H1 or the 36-cell ecology.

Deliver baseline vs prototype at identical camera exposure and light, with ground surface clearly better before adding many separate props. Gate B = owner approves substrate close/mid-distance visual quality.

### C — Micro-geometry, litter and transitions

Add a **small, reusable** set of lower-profile needle mats/sprigs, dry twigs, bark flakes, short brash, variable moss seams and irregular decomposing debris. Use controlled **surface decals / thin mesh clusters** for near-ground coverage; avoid large intersecting transparent planes, harsh alpha clipping, floating litter or repetitive perfect circles. Use contact darkening and blending near trunks, clearings, path edges and stump bases. Litter must not automatically imply new tree deaths or work events.

Use deterministic seeded visual placement independent of gameplay RNG and no new persistence schema, with explicit exclusions for pedestrian path core, H1 collection/start, approved G1 and marker/selectability. No new collision on decorative material. Limit draw calls, atlas variants/materials, transparency overdraw and shadow-casting tiny details; choose shader/decal technique based on existing URP renderer settings, not an assumed Terrain.

Gate C = in-scene near/mid/long visual coherence and no clipping, z-fight, shimmer or unreadable ground interactions.

### D — Living ground vegetation and ecological mapping

Audit existing 3 grass/2 rush + shade fern/bracken/bramble/bilberry/moss assets. For a **single prototype composition**, replace obvious unnatural flat/cloned forms with botanically plausible cards or small clustered geometry at realistic scale, several shapes and bounded colour/rotation variants. Preserve categorical presentation from authoritative understorey/light/ecology; visually distinguish bramble from fern, shrubs from Oak/Beech juveniles; no faux annual growth or ecologically unsupported blanket lushness. Make actual planting/leader/shelters visually readable and avoid blocking click/aim. Moss shoot-mat high poly should be reduced or culled after actual baseline cost measurements; target quality per pixel rather than triangle count.

Gate D = recognisable species/form group silhouettes, logical gap-versus-shade variation, correct survivor/cleared states and acceptable camera-height performance.

### E — Ground-dressing rollout + visual qualification

After the owner passes the 5×5 m prototype, propagate through Compact30 deterministically while respecting authored tree roots, path/H1/G1 and world manifest. Assess far-distance repetition and camera movement; test under original and post-year management to verify ground changes are presentation derived from actual state, not fictional ecology. Generate narrow runtime/render/profile evidence with a native Unity-capable worker and independent art reviewer when required; verify no collisions, tree selection, C/X marks, planting locations and old save/reference effects changed. Owner must personally inspect final on exact candidate before any public-ready claim.

## Production route and model usage

- **GPT-6 / Claude (reference & quality briefs):** describe exact material families, measured scale and shot-by-shot targets; produce reusable texture/asset prompts and asset acceptance checklist. Do not claim completed models from text.
- **Photogrammetry/scanned PBR surfaces** (Poly Haven CC0 and/or Fab/Quixel under verified licensing) as **reference or candidate source** for bark chips, earth, needles and moss. Texture baselines are likely a better first intervention than text-to-3D generating hundreds of tiny debris objects. Licence, redistribution and author provenance are per asset; D-032 remains open.
- **Image generation** for isolated seamless PBR maps/atlases if scan coverage is insufficient, but critically check normal/roughness map consistency, tile seams and baked shadow artifacts. Image maps alone do not prove material quality.
- **Blender native/procedural** for optimised variations of brash, twigs, leaf clusters, moss edges and local grounding; deterministic reusable mesh families. **Meshy/Tripo/Rodin** may be evaluated for medium/hero debris, not default repeated grass/litter.
- **Unity technical artist** is essential for actual URP shaders/alpha/distance/LOD integration. Prefer reuse of current scene/material hooks; do not build an alternate asset engine or introduce purchased dependencies without owner approval.

Relevant platform facts: Unity 6000.6 supports URP Terrain Shader Graph and decal shaders, but features should be matched to the **current non-Terrain flat ground setup**. Unity instancing and terrain detail systems are options, not a mandate. [Unity 6000.6 Terrain Shader Graph](https://docs.unity.com/en-us/engine/6000.6/manual/creating-environments/script-terrain/terrain-shader-graph); [Unity URP decals](https://docs.unity.com/en-us/engine/6000.0/manual/visual-effects/decals/renderer-feature-decal/decal). [Poly Haven CC0](https://polyhaven.com/license); [Fab license handling](https://dev.epicgames.com/documentation/en-us/fab/purchasing-and-downloading-assets-in-fab). Fab/Quixel availability and pricing may change; check actual terms at selection time.

## Quality acceptance / measurement

Mandatory: before/after paired same camera and lighting, at 0.5m/2m/8m/15m+ distances; close-up surface texture resolution and colour; visible ground-to-tree contact; repeated patch density; traversable H1/path; recognisable juvenile/shelter/marker; no flickering/floating/decal bleeding or massive overhead. Record measured frame time, drawcalls, material count, triangles/overdraw, memory and platform in the same capture scene. Set **explicit numerical performance budgets only after baseline** and owner target hardware are known; never assert an arbitrary 60fps if not the accepted budget. Preserve existing exact world manifest/source geometry, physical footpath/hitboxes, save/reference/annual causal ecology. No vegetation scatter may create false regeneration or habitat metrics. Evidence label **VISUAL PROTOTYPE ACCEPTED** separately from **FEATURE VERIFIED / INTEGRATED / OWNER GAME ACCEPTED**.

**Stop conditions:** if baseline scene is not exact #876/#886 candidate, required rights for source/material binaries unresolved, files held by another owner, D-032 licence/storage unresolved, physical Unity lease unavailable or proposed change touches ecology/paths/collision/parent source outside approval, stop the affected operation and report. Do not rework #886 physics.

## Planned artifact and authority

This report is **an initial Manager plan only**. Before production, prepare accepted approved four-file spec under `specs/` (spec, plan, tasks, requirements) and a bounded task packet with exact source parent, physical lane/owner/allowed paths, outputs and tests. First no-write native baseline capture may be separate PACKET ONLY if legal and a clear concrete lane exists. Owner accepts target quality, reference and first prototype scope; does not need to choose a paid AI platform until comparison warrants it. No new Unreal/HDRP/terrain rewrite or third-party acquisition authorised.
