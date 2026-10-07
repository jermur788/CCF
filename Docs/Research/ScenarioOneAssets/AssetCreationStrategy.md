# Creation strategy and technical briefs

First reuse current prefabs and procedural placement; then simplify/normalise and add a few silhouettes. No need for a full free-library/Asset Store replacement. All external sources require recorded original licence, source URL/version and attribution obligations; CC0 is a candidate search class, not a verified licence for any downloaded file. No download is authorised by this report. AI base mesh/texture is optional future method only with provenance and manual geometry/URP cleanup; no model output treated as biological reference.

## High-priority custom brief if reuse fails: oak/beech juvenile set

Purpose: recognisable single-leader TREE regeneration distinct from multistem low shrubs. Biological/display stages .15–.6 m seedling, .6–1.2 m small juvenile, 1.2–2 m sapling; age is not a fixed mesh selector. Three silhouettes/species with oak rounded-lobed and beech simple alternate leaves, restrained branch structure. These visual traits must be reviewed against reference photos; never use English pedunculate oak leaf detail indiscriminately for sessile oak.

1 unit=1m; ground-centred pivot at root collar; authored canonical height 1m or explicit measured height metadata, no hidden parent scale. Proposal budgets C/art: seedling LOD0 ≤1,500 tris, LOD1 ≤500, LOD2 ≤120; sapling LOD0 ≤3,000, LOD1 ≤900, LOD2 ≤200. Maximum two materials (stem + foliage), shared 512–1024 atlas, alpha clipping rather than blended layers where suitable. Simple optional interaction proxy owned by runtime system, no leaf MeshColliders. FBX + Blender source + preview/contact sheet + geometry/material/import manifest; names CCF_{SessileOak|Beech}_Juvenile_{Stage}_{Variant}_LOD{n}. Preserve current save/species IDs. Verify runtime dimensions at four heights, all viewports, tree/shrub paired recognition, pruning-independent stage transition and no GUID break.

## High-priority small custom brief if reuse fails: clearance remnants

Purpose: fresh local cut fern/graminoid/woody-stem remnants inside approved footprint, not barren soil. Three low silhouettes .02–.15m high, 0.2–0.6m across; LOD0 ≤300 tris/patch, distant ≤60, maximum one shared material/512 atlas. Ground pivot, no collider/shadow unless justified. Reuse branch/litter assets before modelling. Placement seeded by existing treatment location/year and fading follows current treatment presentation; no new persisted biology or permanent cleared state. Export FBX/source/manifest and show before/after/recovered fixed camera with uncut moss/fungi retained.

## Conditional deadwood variant brief

Two or three tapered/curved log silhouettes with mesh topology compatible with runtime diameter/length scaling. LOD0 ≤1,500 tris/log, LOD1 ≤500, LOD2 ≤120; maximum two shared bark/cut-end materials, 1K tiling bark and 512 cut-end atlas, fresh/decayed parameterised material variants. Ground/support pivot convention explicitly documented; simple existing collider proxy only. No uprooted storm root plates. Test 25/100/300 current records for repetition and render cost before making more meshes.

## Texture/material plan

| Surface | Needed resolution / mapping | Alpha / normal / roughness | Season / rights |
|---|---|---|---|
| Conifer bark/needles | Reuse current; 1K tiling bark, 512–1K foliage atlas | Bark opaque, foliage clip; modest normals, rough | Evergreen; verified original/CC0 source |
| Oak/beech juvenile leaves/stem | 512–1K shared atlas | Leaf clip only; restrained normal; rough wax/wood distinction | Current summer presentation; no seasonal pack now |
| Bramble/bracken/graminoids | Reuse/clean current 512–1K atlas | Clip leaves/blades; avoid expensive blended layers | Accepted current season only; explicit provenance |
| Logs/cut wood/decay | 1K tiling bark + 512 cut ends | Opaque normals/roughness; parameter variation | Fresh/decayed visual state; original/verified source |
| Moss/litter/fungi | 512, occasional 1K if near-camera warranted | Mostly opaque geometry or limited clip | Current existing states; no fake biology from colour |
| Disturbed ground / track | Reuse 1K tiling soil, 512 edge/remnant mask | Opaque/masked blend; subtle normal, no shiny mud assumption | No hydrology mechanic; verified source |
| Mark paint | Runtime material/256 mask | Opaque/clip projection, high contrast | Accepted red/blue meaning; no invented states |

No 4K requirement established. Use URP shaders in the actual package version, enable instancing where supported and verified, avoid unique materials per plant, sensible max-size/compression/mips; inspect alpha edges at distance. Preserve source textures and use bounded import changes in a future owned asset task.

Variation targets: Sitka use current seed/branch/config variants before new meshes; oak/beech juveniles 3 real silhouettes/species; bramble/bracken 2–3 existing/modified silhouettes with count/rotation; grass/rush use current3/2; logs2–3; ground dress small shared set. Biological height/diameter bounds constrain random scale. High/medium/low cover is placement density, not three special meshes. No pairwise placement algorithm needed.
