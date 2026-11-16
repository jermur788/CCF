# Sen-1 #577 — Compact woodland asset preproduction and scratch look-development RESULT

**Date:** 16 November 2026. **Claim:** [#577](https://github.com/jermur788/CCF/issues/577). **Parent Manager:** [#424](https://github.com/jermur788/CCF/issues/424).
**Packet followed:** [Opus 5.5 compact woodland asset preproduction packet](https://github.com/jermur788/CCF/blob/docs/sen-1-577-opus55-compact-asset-handoff-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-WOODLAND-ASSET-PREPRODUCTION-PACKET-20261116.md) (branch head `8b8679ed`).
**Status:** **PREPRODUCTION INPUT — READY FOR MANAGER REVIEW. NOT an approved asset plan, spec, budget or owner look choice.** Author may not approve its own result.

**Hard boundary statement:** NO Unity / Editor launch, import or Library write. NO change to `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `CCF-Ops/`, `specs/`, scenes, materials, prefabs or `.meta`. NO production asset commit and NO binary in Git. NO save / Prefs access. NO Windows build. NO board, ledger or `active-tasks.md` write. Unity lock status was read (`free`) and **not** acquired. **Dashboard/Drive sync UNVERIFIED.**

---

## 1. Worker, lane and provenance

| Field | Value |
|---|---|
| Worker | Claude Opus 5.5 (`claude-opus-5-5`), effort **High**, Claude Code harness in T3 Code, local Linux machine. Single worker, no subagents or delegates. |
| Role | Primary (preproduction / scratch look-dev only) |
| Registration | [#577 [RUNNING] comment](https://github.com/jermur788/CCF/issues/577#issuecomment-6911751474), posted before any work |
| Starting `main` | `a7b01d1b373cfb3d5ccd9afdff5bfe9a3050a594` (fresh fetch 16 Nov) — context commit |
| Branch | `docs/sen-1-compact-asset-lookdev-opus55-20261116-v1` (absent locally and on origin before creation) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-577-SEN1-CompactAssetLookdev-20261116` (new path; clean at start) |
| Scratch / evidence (outside Git) | `/media/jer/Files/CCF/Verification/Sen-1-577-CompactAssetLookdev-20261116/` |
| Tools actually used | git, gh, Python 3 standard library, ImageMagick `convert`/`identify`, **Blender 4.0.2** (EEVEE, background, `--factory-startup`). No NumPy (not installed; not installed by me). No downloads, installs, scraping or network asset fetches. |
| Board / overlap checks | `git worktree list` (144 entries) and live board `/media/jer/ZX20/CCF-Ops/board/active.md` (mtime 13 Nov) **read-only**: no #577/#576/#544 or graphics/asset writer rows. Live `main:active-tasks.md` lists #577 as READY/NOT RUNNING. `unity-lock status`: `free`, no Editor PIDs. #576 is actively writing on its own branch `docs/sen-1-compact-beta-canonical-scope-20261116-v1` (head `1b96f9c8` at read) — read only, no overlap with my one-file allowlist. #544 save writer: no files touched by me. |
| User work protected | Primary checkout `ops/workflow-foundation-v1` dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` left exactly as found. No stash/clean/reset anywhere. |

### Sources read (all read-only)

- Owner [30 m no-rack ratification](https://github.com/jermur788/CCF/blob/docs/sen-1-owner-30m-baseline-ratification-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-OWNER-RATIFIED-30M-NO-RACK-BETA-DESIGN-BASELINE-20261116.md); [#576 closure packet](https://github.com/jermur788/CCF/blob/docs/sen-1-576-browser-handoff-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-BROWSER-COMPACT-FIRST-BETA-CANONICAL-SPEC-CLOSURE-PACKET-20261116.md); #576 **UNSIGNED** manifest candidate on `docs/sen-1-compact-beta-canonical-scope-20261116-v1` (H1 rectangle, Route A waypoints, G1 candidate points used below as *prototype inputs only*).
- [#573 independent review](https://github.com/jermur788/CCF/blob/review/sen-1-ground-graphics-spec-independent-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md) (`507e0423`); [E80 graphics audit](https://github.com/jermur788/CCF/blob/docs/sen-1-graphics-parity-audit-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-SCENARIO-ONE-GRAPHICS-PARITY-AUDIT-AND-SPEC-HANDOFF.md) (historical).
- `main@a7b01d1b`: `Docs/Project/decision-log.md` (D-032 Open, D-045 Confirmed), `unity-project-overview.md`, `Docs/Project/AgentWorkflow.md`, `.gitignore`, `Assets/ForestPrototype/Docs/UltimateNatureSetup.md`, art READMEs/Asset_Info/Geometry_Counts under `Assets/ForestPrototype/Art/`, catalog assets under `ScenarioOne/Resources/`, `ScenarioHabitatVisuals.cs`, `ScenarioProtectionVisuals.cs`, `ForestEcologyController.cs`, `ForestTreeSpawner.cs`, `Forest Ground.mat`, `Forest Path.mat`, and `ForestTest.unity` as text (grep only; never opened in Unity).
- Local-only, read-only listing: `/media/jer/ZX20/CCF-{U1,U2,U3,S1-ResearchCopy-V1-Qual,S1-UI-Presentation-V1-Qual}/Assets/InnerverseInteractive/`, `/media/jer/Files/CCF/Archives/UnityAssets/`, the Unity Hub Asset Store cache. Nothing copied, uploaded or redistributed.

---

## 2. A-01 Reuse-versus-remake inventory

**Legend.** *Tracked* = in Git at `a7b01d1b`. *Local-only* = present on this machine, gitignored or outside the repo. *Missing* = referenced but not found. Licence column records what the repository actually states; **no in-repo art carries an explicit licence statement and the repository has no `LICENSE` file**, so "first-party (stated)" means the README describes original procedural Blender authoring by project agents, not a verified legal licence. Triangle counts are from tracked `Asset_Info.json` / `Geometry_Counts.md` (Blender reimport), **not Unity**.

| # | Family / item | Path(s) | Repo status | Provenance / licence as recorded | Scale / LOD (recorded) | Render / code dependency | Reuse for 30 m world without source edits? |
|---|---|---|---|---|---|---|---|
| 1a | Plantation Sitka 02 (pole 8 m / first-thinning 12 m), unpruned + pruned treatments | `Art/SitkaPlantation02/`, `Meshes/Plantation02/*.asset` (718 mesh assets), `Prefabs/Forestry/Plantation02/*.prefab` | Tracked (FBX not tracked; meshes as `.asset`; textures PNG) | First-party procedural (README); editable `.blend` **local-only** at `Archives/UnityAssets/Sitka_Plantation_02/` | 3 LODs; lower attachments 0.4–2.2 m | `PlantationVisualCatalog` (Resources) + `ForestTree` pruning family; D-043/D-045 active stand presentation for trees < 20 m | **YES — reuse.** Age-20 compact Sitka matches this family's intended range. Needs no new mesh. Unity performance at compact density UNVERIFIED. |
| 1b | Mature/benchmark/old Sitka, pruning families (`Sitka_Mature_01..03`, `Benchmark_01`, `OldLarge_01/02`, `Young_02`, `Pole_01`) | `Art/Trees/`, `Art/SitkaMatureBenchmark/`, `Art/CropTreePruning/` (679 MB, 252 FBX) | Tracked | First-party procedural. `Sitka_Mature_01` README: "interpreted from the generated reference sheet" — that reference image is **not tracked** (provenance gap, UNVERIFIED) | 3 LODs; 26–31 m authored | Pruning-family prefabs | Reuse for later-year growth; not needed for Year-0 compact view. |
| 1c | Sitka defect sets (bent, cavity, coarse-branch, ring-barked) | `Art/SectionFive/SS_*_Refined_Set/`, `Art/CoarseBranchSet/`, `Art/SitkaRingBarked/` | Tracked | First-party procedural | 3 LODs | `RecentAssetVisualCatalog` bent/cavity stages | Reuse sparingly (modest biological variation). P1. |
| 1d | Sitka seedling 1 m | `Art/Trees/Sitka_Seedling_01/`, `Prefabs/SitkaSeedling01.prefab` | Tracked | First-party; `.blend` local-only in Archives | 2,188 / 797 / 158 tris | `ForestTreeSpawner.seedlingVisualPrefab` (Sitka) | Reuse (natural regen, later years). |
| 1e | Ultimate Nature – Starter `UNS_Spruce_01` (legacy stand visual), `UNS_Grass`, `UNS_Bush`, `UNS_Flower` | `Assets/InnerverseInteractive/…` | **Gitignored; local copies are PARTIAL** — all five ZX20 Unity lanes contain only 6 files (`UNS_Stump`, `UNS_Mushroom_Patch`, palette). The four other GUID-listed assets are **missing** from every lane. Full `.unitypackage` exists only in the Unity Hub Asset Store cache. | Third-party Asset Store (Innerverse Interactive). No licence file inside local copies; Asset Store EULA terms apply and were **not reviewed here** (UNVERIFIED). Never redistributable. | n/a | `ForestTest.unity` serialized GUID refs; `ForestSceneBuilder` | **NO for compact production.** Do not make compact visuals depend on it (supports #573 M-7). Stump/mushroom have first-party equivalents (2b, 3e). |
| 2a | Sessile Oak sapling 2 m / young 8 m / mature 20 m | `Art/Trees/Sessile_Oak_Three_Stages/`, `Prefabs/SessileOakSapling01.prefab` | Tracked (sources local-only: `Archives/UnityAssets/Trees/…` and `~/Documents/ChatGPT/Local Game Dev/…`) | First-party; README cites Forest Research species page as reference | Sapling 40,714 / 11,156 / 2,824 tris | `ForestTreeSpawner` oak `seedlingVisualPrefab` → used for **planted** Oak juveniles | Partial — see finding F-2: uniformly shrunk to 0.6 m. Reuse later stages; **make** a real-scale transplant variant. |
| 2b | Beech sapling 2 m | `Art/SectionFive/Tree_Variation_Additions/Beech_Sapling_01/`, `Prefabs/Forestry/SectionFive/Beech_Sapling_01.prefab` | Tracked (source local-only in Archives) | First-party | 1,316 / 876 / 568 tris | `SectionFiveVisualCatalog.beechSapling` → planted Beech juveniles | Partial — same F-2 scaling issue. |
| 2c | Mature Beech | `Art/Trees/Beech_Mature_01/` | Tracked | First-party; README lists an "AI-generated modelling reference and its prompt" that is **not tracked** (provenance gap) | — | `Prefabs/BeechMature01.prefab` | Not in first-beta view (no mature broadleaves at start; owner OPEN-C3 resolved). Defer. |
| 2d | Deer shelter | `ScenarioProtectionVisuals.cs` (runtime `CreatePrimitive(Cylinder)`) | Tracked code; **no art asset exists** | n/a | 1.2 m × 0.2 m Ø, URP/Lit runtime material | Derived from `Browsing.Shelters`; state colours | **MAKE** (P0-4): proper shelter + stake mesh. See F-3. |
| 2e | Nursery stock / planting bag / planting spot | none found | **Missing** | — | — | Planting is order/Work-Plan driven | MAKE minimal (P0-4) only if spec asks for visible stock. |
| 3a | Forest ground material | `Materials/Forest Ground.mat` | Tracked | Uses `ForestryGround/Textures/Forest_RootPlate_Soil_BaseColor.png` (1254², 3.4 MB, generated) **base colour only, no normal/roughness map** | Tiling 16 | Shared with Legacy40/E80; rewritten by `Editor/ScenarioOneFloorPresentationSetup` | **Do not edit** (#573 M-3). Make compact-specific material instances. |
| 3b | Path/track material | `Materials/Forest Path.mat` | Tracked | **Same single soil texture**, tiling 3 | — | Shared; template for headland/approach clones | **Do not edit/reuse for footpath** — it is the extraction-track look. |
| 3c | Ground moss (shoot mat, flat patch) | `Art/SectionFive/Ground_Moss_Shoots_03/`, `Ground_Moss_Flat_01/` | Tracked | First-party procedural (README: "generic moss-like turf, not a scan") | Shoots 48,640 / 8,554 / 2,760 tris; 2048² RGBA + normals | `RecentAssetVisualCatalog.groundMoss` | Reuse; Shoots LOD0 is heavy — use LOD1/2 for sparse dressing. |
| 3d | Leaf litter (oak / beech / mixed), small deadwood scatter | `Art/SectionFive/Forest_Floor_Detail/` | Tracked | First-party; 1254² RGBA atlas | 5 assets 6,132 tris total at LOD0 | `SectionFiveVisualCatalog` | Reuse later-year (broadleaf litter appears only once broadleaves exist). Small deadwood reuse now. |
| 3e | Brash, windthrow root plates, logs, stump | `Art/ForestryGround/`, `Art/SectionFive/Woodland_Assets/SS_Log_*`, `SS_Stump_01` | Tracked (FBX); sources: `Archives/UnityAssets/Forestry_Ground_Additions/`, `Woodland_Assets/` | First-party procedural | Logs 576–1,368 tris; stump 444 | Felling/storm catalogs | Reuse for event dressing (P1). |
| 3f | **ForestFloorV1** candidate pack (19 assets: moss islands, litter scatters, twigs, woody clusters, low tufts; 12 PNG) | `/media/jer/Files/CCF/Archives/UnityAssets/ForestFloorV1/` | **Local-only, never integrated** | README: "External candidate delivery", "original modular presentation assets". Author/licence **UNVERIFIED** | 56–1,260 tris each; LOD0+1 | none | **Candidate reuse for P0-3**, but D-045 records "ForestFloorV1 is not adopted". Needs owner/Manager re-decision + provenance check before any use. |
| 4 | H1 entrance / parking surface | none | **Missing** | — | — | — | **MAKE** (P0-1). |
| 5 | Narrow pedestrian footpath | none. Existing 52 `Dirt Path ##` planks + `StandApproachPath` clones + headland are the E80/Legacy40 **extraction track** | Tracked (track only) | — | — | Track renderer/material coupled to 7 consumers (#573 M-2) | **MAKE** (P0-2) as a separate compact-only surface; do not reuse or hide the track system. |
| 6a | Understorey: shade fern, bracken, bramble | `Art/SectionFive/Woodland_Assets/{ShadeFern,Bracken,Bramble}_Clump_01` | Tracked | First-party; README: "initial stylised/prototype art set" | 2,720 / 8,904 / 5,796 tris LOD0 | `SectionFiveVisualCatalog` via `ScenarioHabitatVisuals` | Reuse. (My own quick procedural fern read wrongly — see §4 — which supports reuse-before-remake.) |
| 6b | Grass tufts ×3, rush ×2, bilberry, forbs ×2 | `Art/SectionFive/Woodland_Grass_Rush/`, `Bilberry_Type_Cover_01/`, `Woodland_Forbs_01/` | Tracked | First-party | Tufts 1,364–2,112 tris LOD0 | `RecentAssetVisualCatalog` | Reuse for H1/path margin dressing. |
| 7a | Sky / lighting / fog | Scene `Main Camera`: `m_ClearFlags: 2` (Solid Colour), **no `UniversalAdditionalCameraData`** in `ForestTest.unity` (re-confirmed at `a7b01d1b`) | Tracked | — | — | Global camera/`RenderSettings` shared with Legacy40/E80 | Needs feasibility gate (#573 M-4). No asset to reuse. |
| 7b | Boundary / off-site edge | none (runtime `StandWorldBoundary` ridges for E80) | — | — | — | — | DEFER / owner option (fence + field). |

### Key source findings (source-only, NOT Unity-verified)

- **F-1 Shared ground texture.** Ground and path both use one 1254² base-colour soil image with no normal or roughness map. The compact world cannot get a distinct footpath or apron from material tiling alone.
- **F-2 Planted juveniles are miniaturised saplings.** `ScenarioHabitatVisuals.PlaceJuveniles` instantiates the 2 m Oak/Beech sapling prefab and uniformly scales it to `juvenile.heightMeters`. `plantedJuvenileHeightM = 0.6` in `ForestTest.unity:5793`. The result is a ×0.3 copy of a 2 m tree: leaves about one third of real size, branch structure of an older plant. Real-scale transplant meshes are needed for G1 readability. (What a real Irish nursery transplant of each species looks like at planting is a **research item**, not asserted here.)
- **F-3 Shelters hide the lesson.** The runtime shelter is an opaque-ish 1.2 m × 0.2 m cylinder. It completely hides a 0.6 m juvenile (confirmed in the scratch render, §4). Whether 1.2 m is the right height (deer vs other browsers) is an ecology/protection-rule question for the spec, not an art choice.
- **F-4 Ultimate Nature is not reproducible.** Every local lane holds only 2 of the 6 GUID-checklist prefabs, so any E80 baseline already renders with fallbacks or missing visuals for spruce/grass/bush/flower. Compact art must use first-party assets only.
- **F-5 Editable sources are outside Git.** No `.blend` is tracked; sources for most first-party families exist only in `/media/jer/Files/CCF/Archives/UnityAssets/` (3.6 GB). Some (`SS_Brash`, `Woodland_Assets` root) were not found by a bounded search. This is a D-032 storage-policy input.
- **F-6 Provenance gaps.** Two tracked READMEs cite generated reference sheets/prompts that are not in the repository. No in-repo licence file exists. The owner should record a project-wide first-party asset licence/provenance statement before external beta.

---

## 3. A-02 Prioritised compact asset kit (PROPOSED, not an approved budget)

Footprints are proposals for discussion only. Nothing here sets a budget; the #573-era "4 × 2048² / 60 MB" figure is **not** adopted. Texture sizes assume PNG source; Unity-compressed size is unmeasured.

| ID | Item | Make / reuse / defer | Approx. models / textures | LOD / low-end PC notes | Licence / source | Material complexity | Manifest interaction | Missing evidence | Acceptance screenshot / state (proposed) | Proposed footprint |
|---|---|---|---|---|---|---|---|---|---|---|
| **P0-1** | H1 entrance apron + ragged forest-floor edge | **MAKE** apron surface (decal-style mesh or mask-blended ground); **REUSE** grass tufts/rush for margin | 1 apron mesh (or ground mask), 1 apron material set: base colour + normal + roughness (+ optional packed mask) | Flat mesh, < 2 k tris; margin dressing via existing tuft LOD1/2 | New first-party; original procedural method (this result) | URP/Lit, 1 extra material; no custom shader required for v1 | Needs the signed H1 polygon; must not imply the 14 omitted stems were felled (no stumps unless the manifest says the site pre-existed with stumps — **owner/Manager choice**) | Signed H1 polygon, terrain height, camera/URP state | From H1 spawn at eye height, 720p + 1080p: apron, path mouth and stand edge readable; grayscale shows apron > floor value | 2–3 textures at 1024² (~6–9 MB PNG source) |
| **P0-2** | One winding pedestrian footpath (W or N look) | **MAKE** compact-only path surface (spline-ribbon mesh or ground mask); do **not** reuse/hide the 52-plank track | 1 path mesh or mask; 1 path material (shares apron/floor texture set where possible) | Ribbon ~1–4 k tris for ~27 m; no LOD needed at this size | New first-party | URP/Lit; soft edge via vertex alpha or mask | Needs signed Route A spline, width and collider decision; **width interacts with G1 (see §4 F-7)** | Spline, collider, sightlines, 3D | First bend view: path continuous and readable as a footpath, not a 2.6 m track; grayscale contrast measured | Shares P0-1 textures; +0–1 texture |
| **P0-3** | Sitka stand floor + sparse understorey dressing | **MAKE** compact floor material instance with normal/roughness; **REUSE** moss (LOD1/2), small deadwood, fern/bracken/bramble, grass; **CONSIDER** ForestFloorV1 litter/twig pieces after provenance + D-045 re-decision | 1 floor material (new instance, not `Forest Ground.mat`), 0–1 new texture set; existing prefabs | Display-only placement; exclude on built surfaces; keep instance counts small (30 m plot ≈ 36 cells) | First-party; ForestFloorV1 UNVERIFIED | URP/Lit | Display-only mask; `VegetationSites` and clearance targets unchanged (#573 M-1/M-5) | Per-cell counts, high-coverage cells inside 30 m, Year-0 light | Interior view: needle duff dominant, moss modest and patchy, no grid look, nothing on path/apron | 1 texture set at 1024²–2048² (~4–12 MB) |
| **P0-4** | Oak/Beech planting readability | **MAKE** real-scale transplant variants (Oak, Beech; 2 each to avoid clones), **MAKE** shelter + stake mesh with state variants; **REUSE** existing saplings for later growth stages | 2 species × 2 variants × 3 LODs; 1 shelter mesh with 3 state materials | < 1.5 k tris per transplant LOD0; shelter < 500 | New first-party | Leaves: alpha-clip or opaque cut geometry; shelter: translucent or vented — **owner choice**, translucency cost unmeasured | G1 coordinates unsigned; must not move G1 points or tree IDs | Real stock size/form (research), shelter rule height, post-H1 light | G1 view from path: four plants individually findable; species distinguishable at inspection distance; shelter state readable | 2–4 small textures (512²–1024²) |
| **P0-5** | Restrained lighting / sky / fog | **DEFER** to feasibility gate; no asset | 0 assets | Fog/post costs unmeasured | n/a | Camera/URP | Global blast radius with Legacy40/E80 (#573 M-4) | Camera data, clear flags, `RenderSettings` restore | Same H1 view with and without, owner chooses | 0 |
| P1-a | Later-year diversity: Sitka regen, broadleaf litter, defects, logs, brash, windthrow | **REUSE** existing families | existing | existing LODs | first-party | existing | Driven by real ecology state only | Unity per-year renders | Year-N views | 0 new |
| P1-b | Staged collection-day dressing (timber stack at H1 edge; vehicle presence implied, not simulated) | **DEFER** until event spec exists | 1–2 props | — | new first-party | — | Must not create a road network or permanent parked truck | Event spec | — | — |
| P1-c | Boundary option: post-and-wire fence + off-site pasture | **DEFER / owner option** | 1 fence kit, 1 ground material | trivial | new first-party | trivial | Visual only; no donor trees, no simulation beyond boundary | Owner choice | — | small |

**Ecology / provenance safeguards for every item:** display art carries no growth, light, browsing or habitat state; species identity comes only from simulation records; no visual gap may be presented as an executed Fell mark; no pre-authored broadleaves; leaf/stock shapes are art approximations unless backed by a cited source.

---

## 4. A-03 Original scratch look-dev prototypes (UNAPPROVED PREVIEWS)

**What they are.** Fully original procedural Blender 4.0.2 scenes generated by one script I wrote. No image file, scan, Asset Store item, repository mesh or texture is read; all surfaces are Blender procedural nodes and all geometry is generated. They are **not** production assets, **not** imported anywhere, **not** Unity captures, **not** the manifest world, and **not** licensed for distribution. Previews and `.blend` files stay outside Git.

**Inputs.** H1 rectangle x ∈ [−9, 3], z ∈ [−15, −5]; Route A waypoints; four G1 candidate points — all copied from the **UNSIGNED** #576 manifest candidate. Sitka positions are an **illustrative jittered 2.2 m lattice (148 stems), not the #508 roster**. Juveniles 0.6 m (= `plantedJuvenileHeightM`). Shelter 1.2 m × 0.2 m (= `ScenarioProtectionVisuals` constants). Overcast sky, weak soft sun, light world fog, AgX view transform (Unity URP tonemapping will differ).

**Location:** `/media/jer/Files/CCF/Verification/Sen-1-577-CompactAssetLookdev-20261116/`

| File (relative to evidence folder) | SHA-256 | What it is |
|---|---|---|
| `source/build_lookdev.py` | `ba3d19a7b513c2aa5834dbbbc39db8783be78c8ae14e07bf6f62af52ad3c5f0c` | Editable generator (only creative input) |
| `PROVENANCE.md` | `d18638e8632e0e1d09a71b8a731aceb074433a74c762386fdfcefa835190e19f` | Origin, inputs, limits, re-run command |
| `renders/lookdev_W.blend` | `871765badcaf9a04d79f58773bf3833f356aa34d4dea5f8c42c5732a578ffb86` | Editable scene, Direction W |
| `renders/lookdev_N.blend` | `38c8cede77b0150aa74e77a556862f52f8fc43adace7bf8d9a648008b4efa053` | Editable scene, Direction N v1 |
| `renders/lookdev_N2.blend` | `ee4df377d2b270478cab323d8c6f78897b5524e6b67c3c56feae6e6d7b52c01d` | Editable scene, Direction N v2 |
| `renders/CMP_C1_H1_arrival.png` | `f657962750d59748443b1f3608f7500dc1ba5214903a44bcf9d8a41737cface4` | W / N / N2 from H1, colour + grayscale |
| `renders/CMP_C2_first_bend.png` | `42a74087ff8dcd9b1712dcdb6a7cf8f5eef96b5160eb0b3a0ff945a021b68042` | W / N / N2 at path start, colour + grayscale |
| `renders/CMP_C3_G1_unsheltered.png` | `4addc931beefc97b3d81461ffb5902e6fe802f66a8848594866c9cc94f435254` | G1 from the path, three directions |
| `renders/CMP_G1_planting.png` | `5935d3fbf21422b586dec707fb06efc59b0ded109d1e79eae2ee45a28803b037` | Unsheltered vs current shelter dims; close species view |
| `SHA256SUMS` | (manifest of all 31 files) | Includes the 18 single-view renders, 3 plan views, 3 JSON reports, `grayscale_contrast_plan.txt` |

Total size 42 MB, on `/media/jer/Files` (durable CCF tree, per storage policy). Re-run: `blender -b --factory-startup --python-exit-code 1 --python source/build_lookdev.py -- {W|N|N2} renders` (~2 min each on this machine).

### Observations (from my own inspection of the renders; not owner acceptance)

1. **Direction W reads as a footpath in colour and grayscale** at the first bend (C2). Top-down grayscale indicator: path/floor mean grey **1.13**. It does not read as a road or rack at ~1.1 m.
2. **Direction N v1 failed.** Path/floor **1.00**; at C2 the path is almost invisible, especially in grayscale. I kept it in the evidence as a negative result rather than replacing it.
3. **Direction N v2** (lighter trampled line + darker shoulders) is readable but weaker: **1.07**. Its flat, uniform line looks slightly artificial at close range.
4. **From the H1 arrival view (C1) the path mouth is weak in every variant** at ~10 m. The apron reads as a clearing but its flat even grey risks a "car-park slab" look. P0-1 needs edge irregularity, damp/worn variation and margin vegetation, and probably an obvious path entrance cue. The H1 apron is the brightest ground surface (H1/floor **1.24**).
5. **G1 juveniles at real 0.6 m scale:** species are distinguishable only at close range (~2–3 m: Oak clustered lobed leaves vs Beech flat two-ranked sprays). At 4–6 m they become faint stems. The player will need an inspection/marker affordance, not only art. Leaf shapes are art approximations.
6. **The current shelter dimensions hide the plants entirely** (F-3). Shelters are the most visible G1 element from H1, which helps find G1 but hides the lesson's subject. The fix is an owner/spec decision (translucency, vented tube, guard type, or UI), not an art-only change.
7. **F-7 — G1 sits on the path shoulders.** Against the raw Route A polyline, the four G1 points are **1.02–1.26 m** from the centreline (Oaks west, Beeches east). Edge clearance at path widths 1.2 / 1.8 / 2.4 m: Beech (3.83, −2.33) **0.42 / 0.12 / −0.18 m**; Oak (1.17, −0.47) **0.46 / 0.16 / −0.14 m**. At the #508-tested 2.4 m width, two plants would be on the path. Even at 1.2 m, groundcover exclusion, trampling dressing and shelter placement will touch G1. This is **not mine to move**: flagged to #576/#579 as a manifest/spec question (path width and G1 offset).
8. Stand stand-ins (cone crowns, stick branches) are deliberately crude and are **not** a proposal to replace Plantation Sitka 02. My quick procedural "fern" read as the wrong plant and was removed, which supports reusing the tracked fern/bracken prefabs.

---

## 5. A-04 Two footpath directions for owner comparison (neither approved)

| | **Direction W — worn mineral footpath** | **Direction N — needle-litter woodland walk** |
|---|---|---|
| Surface | Low-saturation brown-grey earth, fine grit, needles pressed in, ~1.1 m worn core, ~0.35 m soft edge, slightly sunken | Compacted, fragmented needle duff, no exposed mineral; v2: ~0.9 m line, lighter mid-value trampled line with darker loose-litter shoulders |
| Contrast vs floor | Clear in colour **and** grayscale (value step lighter than duff) | v1: almost none — fails in grayscale. v2: visible, weaker than W; relies on value + shoulder cue |
| Wet / shaded Irish look | Darkens toward the floor value when wet; needs a slightly lighter dry value and roughness drop when wet to stay readable | Already dark; wet state would reduce contrast further — highest legibility risk in rain/dusk |
| Constructed feel | Reads as an established walked path, not a road; risk of looking "made" if too uniform | Least constructed, most natural; risk the player loses the route |
| Groundcover exclusion | No dressing on core; twigs and grass only outside soft edge | Fewer twigs on line; moss and grass suppressed on line and shoulders |
| False ecology risk | None if colour stays earth/needle (no imported gravel) | None |
| Proposed wayfinding aid if chosen | Not needed at Year 0 | Probably needs a second cue (edge timber, occasional waymarker post) — **owner choice** |

**Recommendation for the owner's first look (not a decision):** compare W and N v2 at H1 and the first bend in the real compact scene. My reading of the scratch renders favours **W** for first-beta legibility, with N v2 as the more natural-looking alternative if a wayfinding cue is acceptable. This is a scratch-render opinion, not native compact proof.

Forestry-road or rack material is not proposed for either direction.

---

## 6. #573 seven MAJOR findings — compact applicability

| # | #573 finding (short) | Still present on `main@a7b01d1b`? | Compact classification | Reason / required handling in the new compact spec |
|---|---|---|---|---|
| M-1 | Vegetation mask inside `VegetationSites` would change `ClearanceTargets.GroundPlants` / preview | Yes (`ScenarioOneClearance.cs`, `ClearancePreview.cs` present) | **BLOCKER for compact P0-3 implementation** until spec states the rule | Any H1/path groundcover exclusion must be display-loop-only; spec must add an invariant that `VegetationSites` output and the clearance preview summary are unchanged for fixed footprints. |
| M-2 | Hiding plank renderers breaks headland/approach clones | Yes (`StandApproachPath.cs`, `ScenarioOneHeadlandPresentation.cs`) | **Applicable by avoidance** | Compact path must be a separate, compact-only object. Spec must state whether the compact world instantiates the plank/headland/approach systems at all; if it does, the full #573 M-2 rule list applies. Requires the compact world source hierarchy, which does not yet exist. |
| M-3 | Shared `Forest Ground.mat`/`Forest Path.mat` rewritten by Editor tool | Yes (`Editor/ScenarioOneFloorPresentationSetup.cs`) | **BLOCKER rule (easy to satisfy)** | Compact uses new material assets/instances only; shared materials and the Editor setup tool are neither edited nor run. Legacy40/E80 material hash check in the gate. |
| M-4 | Lighting via skybox/volume may be a no-op (Solid Colour clear, no URP camera data) | Yes (`m_ClearFlags: 2`, 0 `UniversalAdditionalCameraData`) | **OWNER DECISION + feasibility gate** | P0-5 deferred. If compact has its own scene/camera the blast radius may shrink; unknown until the compact source hierarchy is decided. Owner chooses whether tone/fog/sky changes are wanted. |
| M-5 | Suppression cannot keep per-cell counts; fully covered cells exist | Census was E80-only | **APPLICABLE, re-measure required** | Compact has 36 cells; H1 alone covers ~120 m² (≈ 4.8 cells' worth). Spec must state the invariant at state level and require a per-cell census on the signed compact layout. |
| M-6 | Unity/task gating inconsistent | DRAFT-specific | **APPLICABLE to the new compact spec** | Every compact Unity/source task must carry "spec APPROVED by Manager" as a gate; baseline capture rules stated explicitly. |
| M-7 | Visuals depend on gitignored Ultimate Nature pack | Yes, and worse: local copies are partial (§2 F-4) | **BLOCKER for using UN; satisfied by first-party-only rule** | Compact art uses tracked first-party assets only; baseline evidence records pack state and fallbacks. |

---

## 7. Gates before any production / Unity work

1. #576 result and its specs/manifest **reviewed (#579) and APPROVED**, with signed H1 polygon, Route A spline and width, final omission IDs, G1 coordinates and post-H1 light. **Path width and G1 offset must be resolved together** (§4 F-7: G1 points are 1.02–1.26 m from the Route A centreline).
2. **New compact graphics/asset four-file spec** (`specs/scenario-one-compact-graphics-v1/` suggested) resolving all seven rows in §6, independently reviewed and APPROVED. Not the E80 DRAFT.
3. **D-032** owner storage/binary policy, plus licence/provenance statement for first-party art (F-6), Ultimate Nature excluded (F-4), and a decision on whether ForestFloorV1 may be reconsidered despite D-045.
4. Owner look choices: path direction W / N v2; shelter look and translucency; H1 appearance (stumps or not); boundary option; tone/fog/sky.
5. A true **Unity compact baseline** under its own lease: H1, first bend, G1, understorey, later-year views; same-machine timing at native 1280×720 and 1920×1080 with LOD/quality/HUD pinned.
6. A dedicated asset-source writer lane and separately claimed integrator, per-file allowlist, physical board entry, independent build/QA.
7. Research items: realistic nursery transplant size/form for sessile Oak and European Beech in Irish planting practice; appropriate shelter height for the modelled browser.

### Suggested next handoff — **STATUS: BLOCKED UNTIL APPROVED**

> **Sen-1 compact graphics SPEC (docs-only) — BLOCKED until gates 1 and 3 close.** Draft `specs/scenario-one-compact-graphics-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` from this result: P0-1..P0-5 requirements, the §6 rules as acceptance criteria, display-only vegetation invariant, material-instance rule, first-party-only rule, path W/N as an owner question. Then independent nonauthor review.
>
> **Sen-1 compact asset production (Blender, outside Unity) — BLOCKED until the spec is APPROVED and D-032 is decided.** Produce P0-1..P0-4 source + exports with provenance, in the storage location D-032 names.
>
> **Sen-1 compact Unity integration — BLOCKED until both above plus manifest signature and a Unity lease.** Single writer; new compact-only objects and materials; no edits to shared materials, the plank/headland/approach systems, `VegetationSites`, camera or URP assets without a separate approved feasibility gate.

---

## 8. Verification performed

- Source inspection (git show/grep/identify) as listed in §1; findings F-1..F-7 are **source-only or scratch-render facts**, none is Unity-verified.
- Three Blender builds exited 0 (`build_W.log`, `build_N.log`, `build_N2.log` in the evidence folder). I visually inspected all four comparison sheets.
- Grayscale contrast indicator computed from the plan renders (`grayscale_contrast_plan.txt`).
- G1-to-Route-A distances computed in plain Python from the manifest-candidate coordinates (both smoothed and raw polyline).
- Worktree confirmed clean before writing; the only tracked change is this file. GitHub base-to-head compare result is recorded in the #577 [HANDED-OFF] comment.
- **Not run:** Unity, any Editor, any test suite, any performance measurement, any save/Prefs read, any Windows build. Asset Store EULA not reviewed. Dashboard/Drive sync not performed.

---

## 9. Standard handoff

```text
IMPLEMENTATION HANDOFF
Task: #577 Sen-1 compact woodland asset preproduction + scratch look-dev
Worker: Claude Opus 5.5 (claude-opus-5-5), High, Claude Code in T3 Code
Role: Primary (preproduction only)
Branch: docs/sen-1-compact-asset-lookdev-opus55-20261116-v1
Base: a7b01d1b373cfb3d5ccd9afdff5bfe9a3050a594
Context commit: a7b01d1b373cfb3d5ccd9afdff5bfe9a3050a594
HEAD: (this commit; see #577 [HANDED-OFF] comment)
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-ASSET-PREPRODUCTION-RESULT-20261116.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Sen-1-577-CompactAssetLookdev-20261116/
WHAT CHANGED: one new Markdown handoff
FILES CHANGED: this file only
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none in Git; scratch-only previews outside Git
MUST-NOT-CHANGE HASHES / ANCHORS: untouched
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN: see section 8
REMAINING VERIFICATION GATES: section 7
KNOWN LIMITATIONS: no Unity; illustrative tree lattice; Blender EEVEE/AgX look differs from URP; Asset Store EULA not reviewed
UNRELATED WORK PRESERVED: Primary dirty AGENTS.md and StorageArchitecture.md untouched
DECISIONS NEEDED: section 7 items 3-4 (owner), 7 (research)
WORKER ASSESSMENT: Ready for review: Yes (as preproduction input only)
RECOMMENDED NEXT STEP: #424 Manager review -> APPROVED AS PREPRODUCTION INPUT / NEEDS CHANGES / REJECTED
```

No Unity. No production asset commit. No save/Prefs. No Windows. Dashboard/Drive sync UNVERIFIED.
