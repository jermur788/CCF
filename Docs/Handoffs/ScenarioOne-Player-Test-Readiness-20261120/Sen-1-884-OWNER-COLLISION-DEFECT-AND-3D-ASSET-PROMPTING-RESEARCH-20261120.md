# Sen-1 #884 — Owner walkthrough collision findings and model-specific 3D asset production research

**Date:** 20 November 2026. **Class:** OWNER FIRSTHAND FEEDBACK + SOURCE-INSPECTED BUG HYPOTHESIS + SUPPORTING EXTERNAL RESEARCH / ART PROPOSAL; NOT approved asset redesign, no production edits. **Manager:** GPT-6, GitHub-only. **Candidate:** `fix/sen1-876-mark-visual-scale-20261120-v1` @ `fce41bd3b77a054bf82d2521648d15ec18f81141`. **Scope:** Compact30 Scenario One; do not mutate Legacy40, Enlarged80, saves, Reference Future v1, physics, art, scenes or binaries here.

## Owner firsthand test — scope and remaining uncertainty

Owner completed a walkthrough on 20 Nov 2026 and observed: (a) some standing trees block walking when the avatar seems still far from their rendered trunks, with inconsistency among trees; (b) felling permits passage past/through where tree obstruction had been; (c) visual asset quality is substantially below desired standard. **No precise offending tree IDs, screenshots or native collision probes supplied**, so do not claim all trees affected or the exact visual mismatch distances measured. Owner did not state all Year1/Year2 actions passed; prior manager's 'owner not run' status is now superseded only for a walkthrough having occurred, not a full gate PASS.

## Source inspection — strong diagnosis, not runtime qualification

- `Assets/ForestPrototype/ScenarioOne/Compact30/Compact30Bootstrap.cs` at #876, `BindTrees` lines 208–223: on each authored tree prefab root, add `ForestTree`, then `CapsuleCollider` with `radius=0.25f`, `height=Mathf.Max(1.2f,row.height_m)`, `center=(0,height/2,0)`. #876's exact scale-repair result records Compact Sitka **tree root lossyScale of 9.13–15.90**; multiplying capsule local dimensions by those roots implies effective radius **~2.28–3.98m**, not the nominal 0.25m, and height/vertical centre are amplified as well. This is a **source-derived mechanism consistent with owner's distant/inconsistent blocking**, not an instrumented physical collision distance. Another uncertainty: child-collider/layer/physics states of all different authored prefab instances remain uninspected in Unity.
- `Assets/ForestPrototype/ScenarioOne/Compact30/Compact30Session.cs` `OnAuthoredTreeFelled` lines 479–503: all child colliders are disabled when felled, and replacement stump cylinder has its primitive collider destroyed. Therefore **no stump physical blocker** from that branch. The game does not presently place a persistent obstructing fallen log in this path; do not invent a felled-trunk obstruction.
- `Assets/ForestPrototype/ForestPlayer.cs` declares `[RequireComponent(typeof(CharacterController))]`; collider behaviour should be tested by real controller movement, not only aim raycasts. `ForestTree.ApplyVisualOverride` deliberately strips colliders from decorative polished visual assets, so collision must be restored at authoritative physical root/explicit child, not accidentally on arbitrary mesh visuals.
- **Recommended bounded repair:** decouple physics trunk collider from scaled art root using actual world-space DBH/trunk dimensions (or explicit inverse scale), validate centre/height in actual world metres, preserve mark raycast/interaction and true collisions on all 175 authored Sitka; decide intended solid stump behaviour separately with owner (a low stump may be traversable with the CharacterController step offset). Test representative smallest/largest scaled trunks, fresh and after fell, collisions near path and planting positions, no unintended marking failures. Keep felling result physically/visually consistent. **No source write yet; candidate is not physically verified.**

Unity references: [Character Controller](https://docs.unity.com/en-us/engine/6000.6/manual/physics-section/physics-overview/character-control-section/character-controllers), [collider performance](https://docs.unity.com/en-us/engine/6000.6/manual/physics-section/physics-overview/physics-optimization/cpu/collider-types).

## Art direction — test a quality bar before bulk regeneration

The existing delivered #873 candidate incorporated 74 art blobs, but owner rejects appearance. Don't assume 'more triangles' means better forestry quality. The primary impression is plausibly dominated by coherent **tree silhouette, branch whorls/taper, foliage density/transparency, bark/moss variation, proportional trunks, spatial spacing, ground litter and physically plausible lighting**. These are *art acceptance criteria to propose*, not source-proven fault attribution. Before reauthoring 175 trees, create and compare **one hero Sitka + one smaller Sitka + one first-year Oak/one Beech** from the same viewed location and actual existing lighting; a side-by-side with screenshot of current target is necessary to prove improvement. Variation should be deterministic by existing tree ID and not alter simulation heights/DBH, original placement or saves.

### Preferred multi-model strategy

1. **Research/reference and prompt designer (GPT-6 / Claude)**: audit real Irish Sitka plantation reference and decide silhouette, foliage, trunk bark, crown-base, stage, distinctive Oak/Beech; produce measurable asset brief and model output checklist. Language models don't manufacture verified usable topology merely by writing a detailed prompt.
2. **Image concept generator** (capable image model): generate *clean isolated orthographic front/side/back views of exactly the same species/stage* with transparent or neutral background, no ground, no labels, no other trees. Use this as visual control. Multi-view consistency is more important than cinematic mood.
3. **Image-to-3D generator**: evaluate **Meshy 7.x**, **Tripo P-series/H-series**, **Hyper3D Rodin Gen-2.5** from same concept sheets. Compare geometry quality in Unity, not marketing previews. Meshy supports refinement, remeshing, PBR; Tripo supports topology choices/Unity bridge and P-series smart low-poly; Rodin supports up to 5 image views. These tools may struggle with slender branch systems, transparency and billboard-like foliage, so procedural species-specific branch/foliage authoring or manual repair may be superior.
4. **Procedural/tree specialist (Blender scripting or SpeedTree where licensed)**: build controlled trunks, branching, needle cards/leaf clusters, LOD0/1/2, variation seeds, wind rig/motion, atlases and realistic materials. For repeated trees, this may outperform one-off image-to-mesh generation in editability, silhouettes and cost. SpeedTree's Unity support offers LOD/wind pipeline; licensing and D-032 large-binary policy unresolved, so **no procurement or production import authorised**.
5. **Independent visual critic**: inspect **turntable + in-scene captures** at character height, daylight and overcast, including close trunk and 15m/30m forest views. Score species identity, silhouette, foliage transparency, bark, stability, scale, ground integration, LOD popping, GPU cost, licence/provenance and collision separation. Reject models with fused/floaty foliage, plastic materials, shadows/lighting baked into albedo, uneditable density, and fake biological state.

### A reusable concept-image prompt (image model, not mesh task)

> Photorealistic botanical asset reference sheet of **one Irish plantation-grown Sitka spruce (Picea sitchensis)**, mature first-thinning stage with a tall straight slightly tapering trunk, realistic whorled primary branches, uneven fine drooping secondary branchlets, dense but visibly layered blue-green needles, credible natural variation and distinct crown base. Accurate standing tree shape, not a generic Christmas tree. Exactly same individual tree shown in **front, side, back three-quarter views** on a neutral studio background, full trunk and crown contained in every view, consistent proportions and colours. Even neutral illumination, no cast scene shadows, scenery, text, ground plane, stylisation, cartoon foliage or dramatic fog. Reference for construction of a physically based real-time Unity asset; separate material details may be shown in a labelled companion sheet, not on the geometry views.

**Important:** Request one view per output or matching orthographic multi-view reference where the service cannot ensure correspondence; don't expect a single 2D image to specify hidden branches.

### Meshy prompt (prefer multi-view image input; text-only as fallback)

> Single isolated **Sitka spruce tree asset**, straight tapered trunk with natural bark, realistic branching whorls at varied irregular heights, asymmetrical branch lengths, layered fine blue-green needle boughs with see-through gaps, 12–16m scale target **to be enforced in Blender/Unity**, roots grounded at origin; real-time PC forestry game, physically realistic bark and foliage, independent UV/material groups, natural roughness and normal surface detail. **One tree only**, no floor, rocks, background, labels or extra props. No cartoon/conical Christmas tree, no fused solid foliage cone, no floating parts.

**Settings outside text**: preview → inspect topology → refine with PBR; geometry remesh target derived from actual frame budget, preserve UVs and transparency; request GLB or FBX and texture maps. Meshy API docs emphasize mode/refine and PBR, and some settings change across versions; do not assume current model string from an old example.

### Tripo P/H prompt (for text-to-3D; image-to-3D emphasizes the uploaded views)

> One real-world **European beech sapling (Fagus sylvatica)** 1.5m tall, slender continuous trunk, alternate branch architecture, broad oval leaves, nonuniform leaf clusters and gaps allowing light through, subtle bark/leaf physical materials, standalone Unity game asset at world scale, no pot, terrain, text or forest backdrop. Preserve a simple editable branching hierarchy and distinct foliage geometry rather than one fused lumpy canopy.

**Settings:** choose P-series/Smart Mesh or triangle vs quad suitable to editability and runtime budget, PBR maps on, verify export axis/metres. Tripo's image-to-3D may not receive free-text prompts in some connected workflows, so the *reference image* carries most instructions; set face limits/PBR through actual tool settings.

### Rodin Gen-2.5 prompt (image-to-3D with up to five consistent reference views)

> Generate only the exact depicted tree. Keep full standing trunk and all branching, with accurate species leaf/needle shapes; preserve leaf-canopy openings instead of closing them into a solid shell. Realistic rough PBR bark/leaf materials and a grounded base. No added grass pedestal, rock, second tree, landscape, invented ornaments, logos or floating disconnected mesh.

**Settings:** supply consistent front/back/side input images with directions when supported; inspect shape before accepting textured output. A generated mesh still needs Blender/Unity cleanup.

### Blender/SpeedTree specialist implementation prompt (for a code-capable model with real native tools)

> Using the provided approved forestry reference sheets and existing Compact30 scene's pinned Sitka roster, create **one non-destructive, reproducible species-authored prototype**, not a bulk scene replacement. Build a straight tapered trunk with tree-age/DBH-dependent bark and plausible nonuniform whorl spacing, explicit branch segments and carefully culled leaf/needle cards. Deliver separate trunk/foliage materials with albedo free from baked lighting, normals, roughness, opacity/alpha test, meaningful UVs, wind-ready foliage groups and three LOD levels. Keep art under a visual root separate from simulation tree identity and independently sized physics trunk collider. Do not alter tree positions, authoritative Height/DBH, planting, marker math, saves, references, Unity version or packages. Document triangles, material count, textures, drawcalls, assets licence/origin, deterministic variant inputs, render screenshots and importer settings. Deliver prototype and reproducible source steps for review; do not replace all trees or merge.

### Independent reviewer / acceptance prompt

> Review the candidate as a 3D forestry artist and Unity technical artist, against pinned real species reference and prior in-game screenshots. Do **not** score from a glossy close-up alone. Inspect front/side canopy silhouette, low-camera height at 2m/8m/20m, trunk surface, branch-to-trunk junctions, foliage transparency/overdraw, ground blending, shadow coherence, wind/LODs, performance, file size and licence. Show objective comparison to old asset and precise defect examples; give APPROVED/NEEDS CHANGES/REJECTED for **prototype quality only**; do not infer biological simulation, collision, save or full gameplay acceptance.

## Model/tool observations and provenance (current docs inspected 20 Nov 2026)

- Meshy official [text-to-3D two-stage preview/refine cookbook](https://www.meshy.ai/developers/cookbook/recipes/generate-3d-model-from-text/) and [AI PBR texturing](https://docs.meshy.ai/en/webapp/guides/3d-model/ai-texturing); [API image-to-3D](https://docs.meshy.ai/en/api/image-to-3d) changes version/option support.
- Tripo official [image-to-3D guide (Oct 2026)](https://www.tripo3d.ai/product-doc/model-generation/image-to-3d), [text-to-3D](https://www.tripo3d.ai/product-doc/model-generation/text-to-3d), [game scene pipeline](https://www.tripo3d.ai/tutorials/workflow-game-scene).
- Hyper3D Rodin [Gen2.5 spec](https://docs.hyper3d.ai/en/api-specification/rodin-gen2-5) and [multi-view guide](https://hyper3d.ai/help/articles/create-3d-models-from-images-with-rodin).
- Tencent [Hunyuan3D-2.1](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1) is an image-conditioned mesh+PBR experimental/local alternative; compute, licensing and runtime viability must be checked before use.
- Unity [SpeedTree importer in 6000.6](https://docs.unity.com/en-us/engine/6000.6/manual/creating-environments/script-terrain/terrain-trees/speed-tree/class-speed-tree-importer/speed-tree-importer-model). Distinguish renderer LOD from authoritative tree/stump state.

**Research caveat:** Product documentation establishes capabilities, not comparative visual quality in this game's scene. No head-to-head generation or native Unity render was run. External model ranking is provisional. **D-032 remains OPEN** for binaries/licensing/LFS and cannot be bypassed. A short approved art prototype spec and scene test are required before production change.

## Management routing

**Collision:** PACKET ONLY source diagnosis/fix, with native Codex/Sol or equivalent authorised Unity-capable worker after refreshed live task/physical lock and exact write allowlist; include independent narrow review only if warranted by affected reference/collision contracts. No writer dispatched by this note.

**Art quality:** SPEC REQUIRED for substantial visual redesign and species/stage-wide asset pipeline; owner has rejected current visual standard but not yet approved an exact new visual target, licensing, budget or source reference. Prepare concept/reference and isolated prototype only after proper art authority; keep current playable candidate available.

**Integration:** do not bind a collision fix to bulk asset overhaul. Both may feed a later *exact* playtest candidate after independent review and Tier2 checks; no Windows before owner's exact qualifying Editor candidate and separate express authorisation.
