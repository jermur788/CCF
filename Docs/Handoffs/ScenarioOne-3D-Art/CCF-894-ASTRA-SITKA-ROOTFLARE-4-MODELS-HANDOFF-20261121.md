# CCF #894 → Astra: four new Sitka models with scanned root/trunk bases (Blender asset authoring)

**PREPARED, NOT DISPATCHED.** This packet records preparation only. It does not grant authority, readiness or dispatch beyond what the owner gives Astra directly.

| | |
|---|---|
| Prepared | 2026-11-21 by Claude Opus 5.5 (`claude-opus-5-5`), Claude Code / T3 Code, on the owner's direct request in this session: *"create a handoff for Astra to implement this … 4 new Sitka models using the design of the 4 new root and trunk assets including the moss, but the upper part will be the same as the original models."* |
| Recipient | **Astra**, implementer. The work is Blender asset authoring only. |
| Context commit | `origin/main` @ `cfc802d26d12f3578723bec9c94a8ce165ae7124` (2026-11-20 22:48 UTC) |
| Issue | https://github.com/jermur788/CCF/issues/894 |
| Lease | **None needed for Unity: Unity must not be opened.** Write ownership is filesystem only, limited to the two new folders in §6. |
| Durable copy | This file, on `docs/894-opus-phase0-inspection-20261120-v1`, plus a copy at `/media/jer/Files/Documents/ChatGPT/Local Game Dev/Handoffs/CCF-894-Astra-Sitka-RootFlare-4-Models/HANDOFF.md` |
| Readiness evidence | None supplied in v1 shape. The owner instruction above is the only authority. |

---

## 1. Objective

Author **four new Sitka spruce tree assets**. Each one combines:

- the **upper tree of one existing CCF Sitka model**: crown, branches, foliage, upper trunk, silhouette and authored height, all **unchanged**;
- the **root flare and lower-trunk shape of one Quixel spruce-trunk scan**, including the scan's **moss and soil at the base**;
- **that model's own Sitka bark** on the whole trunk, from roots to crown tip, with **zero visible join** between the scanned base and the original trunk.

Deliver editable `.blend` sources and Unity-ready FBX exports with LODs, textures, manifest, validation and previews, following the conventions of the existing CCF asset packages.

## 2. Why: background and what is already proven

- **Owner complaint (#894):** Compact30 trunk bases and their root entry into the soil look poor. The current procedural trunks are near-cylinders with small spike roots.
- **One-tree Unity prototype by Claude:** branch `art/ccf-894-root-flare-prototype-20261120-v1`, local only, HEAD `ca327677`. It showed the following:
  1. A scanned base, fitted by **uniform scale plus axis alignment only**, sits convincingly on the original trunk. For `thksce3da` on the 26 m Benchmark model, the scan's inscribed top radius had to enclose the trunk with a 4 % margin. In the game the tree is scaled to 12.14 m, which put the seam at 1.51 m.
  2. **Scan bark against model bark makes a visible ring** at the seam: different colour, pattern scale and normal detail.
  3. **Wrapping the model's bark over the scan shape removes the ring**, using UVs that continue the trunk's cylindrical unwrap. It also **loses the scan's moss and AO**, so the roots look clean and pale, and the bark-to-soil edge chosen per triangle looks jagged.
- **This task fixes point 3 properly in Blender:** one continuous trunk mesh, the model's own bark everywhere, and the scan's moss and soil **baked in as an overlay** on the lower trunk.

Prototype evidence:
- Result doc: `Docs/Handoffs/ScenarioOne-3D-Art/CCF-894-ONE-TREE-ROOT-FLARE-PROTOTYPE-RESULT-20261121.md` on this docs branch, including the addendum.
- Captures: `/media/jer/Files/CCF/Verification/CCF894-RootFlare-20261120/run2`–`run4` (`sheet-*.png`, `crop-roots-scan-vs-benchbark.png`).

These are reference only. **Do not reuse the Unity prototype's assets as sources.**

## 3. Canonical sources (all verified present 2026-11-21)

### 3.1 Original upper models: use the editable sources, do not modify them

The four Sitka models actually used in `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` (at `fce41bd3`). They account for all 175 roster trees plus 226 outside trees.

| Original | Roster / outside instances | Authored height | Unpruned LOD0/1/2 tris | Editable source (sha256 prefix) |
|---|---|---|---|---|
| `Sitka_Young_02` | 50 / 64 | 10.50 m | 7,044 / 3,640 / 1,404 | `/media/jer/Files/CCF/Archives/UnityAssets/Crop_Tree_Pruning/Sitka_Young_02/Sitka_Young_02_Pruning.blend` (`01ea2dd78dbaf889`). Pre-pruning source: `…/Tree_Variation_Additions/Sitka_Young_02/Sitka_Young_02.blend` |
| `Sitka_Pole_01` | 57 / 52 | 8.00 m | 10,552 / 5,198 / 1,905 | `…/Crop_Tree_Pruning/Sitka_Pole_01/Sitka_Pole_01_Pruning.blend` (`b81e0828373d6953`). Pre-pruning source: `/media/jer/Files/Documents/ChatGPT/Local Game Dev/Sitka_Pole_01/` (`.blend` and `build_sitka_pole.py`) |
| `Sitka_Mature_01` | 31 / 55 | 28.01 m | 49,888 / 25,576 / 9,042 | `…/Crop_Tree_Pruning/Sitka_Mature_01/Sitka_Mature_01_Pruning.blend` (`c5e48d236feac62c`). Pre-pruning source: `/media/jer/Files/Documents/ChatGPT/Local Game Dev/Sitka_Mature_01/` |
| `Sitka_Mature_Benchmark_01` | 37 / 55 | 26.00 m (DBH 0.48 m) | 44,050 / 19,690 / 6,762 | `/media/jer/Files/CCF/Archives/UnityAssets/Sitka_Mature_Benchmark_01/Sitka_Mature_Benchmark_01.blend` (`913300fa29668e87`), with generator `build_benchmark.py` and bark sources `create_textures.py`, `bake_bark_relief.py`, `Bark_Material_Bake_Source.blend` and `Textures/Sitka_Bark_*_v2.png` |

- **Generators:**
  - `Crop_Tree_Pruning/build_pruning.py` (pruning states). Its `BASE = OUT.parent` expects source folders beside the package. It requires Blender 4.0.2 plus NumPy and the "original library at its recorded paths".
  - `Sitka_Mature_Benchmark_01/build_benchmark.py`. It is parametric (`H`, `radius(z)`, branch seeds).
- **Path caveat:** these were built under `/home/jer/Documents/ChatGPT/Local Game Dev/`, which **no longer exists**. The owner has moved the asset workspace to `/media/jer/Files/Documents/ChatGPT/Local Game Dev/`, and the complete package archive is at `/media/jer/Files/CCF/Archives/UnityAssets/`. Fix paths in **your copies only**.
- **Bark per model:** each model keeps its **own** bark.
  - Benchmark: `SS_Benchmark_Bark` (v2 BaseColor/Normal/MetallicSmoothness).
  - CropTreePruning family: `Sitka_Bark` (Young_02, Mature_01) and `Sitka_Pole_Bark` (Pole_01), from `Crop_Tree_Pruning/Textures/`.
  - These bark images are procedural or AI-authored approximations, not photographed Sitka. Keep them anyway: the owner asked for the upper part to stay the same.

### 3.2 Scanned bases (owner-downloaded Fab/Quixel; owner states the licence is not an issue)

**Verified location: `/media/jer/ZX20/NewAssets/`.** The owner said the folder had been moved to `Files/Documents/ChatGPT/…`, but on 2026-11-21 it was **not found there**. Use whichever location holds files with these **exact hashes**.

| Scan (Quixel ID) | Archive | sha256 | Raw measurements (FBX in cm → m) |
|---|---|---|---|
| Spruce Tree Trunk `thksce3da` | `spruce_tree_trunk_thksce3da_mid(1).zip` (Mid 3,857 tris, 2K) | `dc336238c068944d8b858fddb8808f460873c34b877cfa8c4526c28d4434dc19` | 1.69 × 1.42 × **2.94 m** tall; stem Ø 0.52 m at 1.3 m; leans ~5° |
| ″ | `spruce_tree_trunk_thksce3da_raw.zip` (Raw ~2.0 M tris, 8K) | `22214e24a5d1e23423feb068c7452ef6cf63e7d446e0f72996c020949ea924d0` | ″ |
| Spruce Tree Trunk `ugjoddlfa` | `spruce_tree_trunk_ugjoddlfa_mid.zip` (Mid 3,188 tris, 2K) | `60db6c978280944ce2d717074a5272c76e8471876770aaba48a57d9bacb98852` | 1.86 × 1.57 × **1.96 m**; Ø 0.44 m; straight |
| ″ | `spruce_tree_trunk_ugjoddlfa_raw.zip` | `9e5e0995e26d4e7c6e84361554387c91210c6125701db6e4a6c70455f259a996` | ″ |
| Spruce Tree Trunk `ugggcjmla` | `spruce_tree_trunk_ugggcjmla_mid.zip` (Mid 5,971 tris, 2K) | `f452d1ce6f9a5216a06f7a1de8a1207f24b09296aac41c9844fb2358735bc08b` | 2.10 × 1.47 × **1.43 m**; Ø 0.36 m |
| ″ | `spruce_tree_trunk_ugggcjmla_raw.zip` | `d37cc71245daef78db5e25ab79e36b7909d39fa9f67fc8977b17afb1283baab2` | ″ |
| Spruce Tree Trunk `ugghcjqfa` | `spruce_tree_trunk_ugghcjqfa_raw.zip` (**Raw only**, ~1.0 M tris, 8K) | `5eb8278d4c57b3083f9e931725be0fdffeb7fd7c5b0b8ceac037b1cc00b62c0e` | 1.70 × 1.52 × **1.83 m**; Ø 0.53 m; leans ~6° |
| *(not a default; reserve)* Nordic `ugfmbftfa` | `nordic_forest_tree_trunk_spruce_large_ugfmbftfa_raw.zip` | `d744ee588a36b1d648f3839f8f9b8995a649e3ce3cca40d1ff507c204c639d2b` | 1.47 × 1.32 × 2.05 m; thin 0.26 m stem on a 1.3 m flare |

**Scan notes:**
- Quixel tags all four as old, mossy spruce, mostly Sweden (thksce3da's country is unrecorded). Mesh type "Open": there is no cap at the top cut.
- The texture is a **photogrammetry atlas** (irregular UV islands), not a tileable texture.
- The Raw zips contain only the high mesh plus 8K maps. They have **no LOD tiers**.
- Unity imported the Mid FBX 100× too large. Blender imported thksce3da Mid correctly in metres. Verify your import scale every time.

### 3.3 Default model ↔ scan pairing (owner may change it; see O-1)

The pairing matches the scan's stem thickness and height to each model's size. The thinnest and shortest scan goes on the smallest model.

| New asset | Upper from | Base from | Reason |
|---|---|---|---|
| `Sitka_Young_02_RootFlare_01` | Sitka_Young_02 | `ugggcjmla` | Thinnest (Ø 0.36) and shortest (1.43 m) |
| `Sitka_Pole_01_RootFlare_01` | Sitka_Pole_01 | `ugjoddlfa` | Straight, medium (Ø 0.44) |
| `Sitka_Mature_01_RootFlare_01` | Sitka_Mature_01 | `ugghcjqfa` | Thick (Ø 0.53), Raw only |
| `Sitka_Mature_Benchmark_01_RootFlare_01` | Sitka_Mature_Benchmark_01 | `thksce3da` | Thick, tallest (2.94 m), colour-proven on this model in the Unity prototype |

## 4. Required design

### 4.1 Geometry: one continuous trunk

1. **Fit.**
   - Measure the original Unpruned LOD0 trunk's cross-section near the intended junction, and the scan's cross-section near its top. Use triangle–plane slices; the prototype method is in `Assets/ForestPrototype/Art/Experiments/CCF894/Editor/CCF894RootFlareBuilder.cs` on the prototype branch.
   - Scale the scan **uniformly** so its stem matches the trunk diameter at the junction.
   - Place the scan base on the soil (z = 0) and the centre of its top ring on the trunk axis.
   - Correct the lean of thksce3da and ugghcjqfa so the stem continues the trunk axis without a kink.
   - **No non-uniform stretching** of the scan.
2. **Merge.**
   - Remove the original trunk below a junction band.
   - Join the scan's upper stem to the original trunk into **one manifold surface**, for example by matching edge loops and bridging them, followed by a local remesh or relax.
   - Leave **no overlap, step, gap or second surface**.
   - Hide the junction band inside the stem, well above the moss fade (see 4.2), and smooth it over at least ~1 stem diameter.
3. **Preserve:**
   - the authored height to within ±2 cm;
   - the soil-level pivot, metre scale and orientation;
   - everything above the junction band, unchanged: branches, foliage, dead-branch remnants and object split (Trunk / Branches / Foliage, or each model's existing split).
4. **Ground contact.** The root flare and soil skirt sit at z ≈ 0, within the existing 5 mm ground tolerance:
   - no floating roots;
   - at most a few cm of buried skirt;
   - no underground geometry visible from above.
5. **Size realism.** After the fit, report the footprint and the junction height in metres for each model. The scans come from old, large trees; a young or pole tree needs a proportionately small flare.
   - If the uniform fit leaves the base implausibly large or small for the model, record it, together with a proposed scaling compromise.
   - **Do not invent new root geometry.**

### 4.2 Texture: model bark everywhere, scan moss and soil as an overlay, zero join

1. **One continuous bark mapping.** Give the merged trunk a UV layout in which the model's **own tiling bark** has the **same texel scale and orientation** as on the original trunk, from root tips to crown. Either extend the original cylindrical unwrap down the flare, letting bark grain run down the stem and out along the roots, or re-unwrap the whole trunk consistently.
2. **Lower-trunk unique texture.**
   - The lower part (base plus a fade band, a few metres at most) needs unique UVs and its own baked 2K maps: BaseColor, Normal, and AO or MetallicSmoothness as the model's material requires.
   - Its boundary with the tiled upper trunk must sit on an edge loop **above the moss fade**, so both sides show the *same* tiled bark at that loop.
   - Bake the lower texture with the same tiled bark mapping, so the pixels match across the boundary.
   - Test at grazing and close (0.3–0.5 m) views for a filtering or mip line.
3. **Moss and soil overlay.**
   - Bake the scan's BaseColor, Normal and AO onto the new lower mesh (selected → active, cage/extrusion as needed; **prefer the Raw scan as the bake source** for detail).
   - Derive a **mask** of moss and soil only, from hue/saturation, height and AO.
   - Composite: `final = mix(model bark, scan bake, mask)`.
   - The mask's upper edge must be **irregular and naturally faded**, with moss creeping higher on root tops and the up-facing sides. No straight horizontal line.
   - The scan's own bark (non-moss) areas are replaced by the model's bark.
   - Normal detail: blend the scan's root and moss relief with the model bark normal by the same mask.
4. **No visible join anywhere** (geometry, colour, normal, AO or texel scale) at walking views (1.65 m eye height, 1.5–4 m away) or close up (0.4 m).

### 4.3 LODs, materials and export (follow existing CCF package conventions)

- **LODs:** one triangle budget per LOD, roughly the original's count **plus** at most +6,000 (LOD0), +3,000 (LOD1) and +1,000 (LOD2) for the new base. LOD2 may use a simplified base, and it may drop the separate lower texture only if the join stays invisible at LOD2 distance.
- **Materials:** in Blender, keep URP-portable materials: BaseColor, Normal (OpenGL convention), and MetallicSmoothness or roughness as the original. Bark/wood roughness high. Foliage unchanged (alpha clip ~0.35, double-sided).
- **Export:**
  - FBX: **-Z forward / Y up**, metre scale, applied transforms, selected tree objects only.
  - **No** cameras, lights, animation, colliders or modifiers.
  - Portable PNG textures beside the FBX.
- **Scope of states:** **Phase A = Unpruned only** (4 models × 3 LODs = 12 FBX). Phase B (pruning states) only after owner approval; see 5 and O-3.

### 4.4 Ecology and visual boundary

These are visual assets only. The scans are of old Swedish (probably Norway) spruce. **Whether Sitka spruce at these ages and sizes shows this root form and moss cover is UNVERIFIED.** Record that as a visual calibration in the README, not as an ecological fact.

Do not encode age, DBH, growth, pruning history or any gameplay state in names, bounds or manifests, beyond the descriptive authored dimensions. This matches the existing CCF art rule.

## 5. Scope and exclusions

**In scope:** four new Unpruned models (Phase A) with LOD0–2, sources, textures, manifest, validation, previews, README and a result handoff.

**Excluded:**
- Unity (do not open it; see §7);
- the CCF Git repository and any branch, commit or push;
- `Assets/**`, scenes and prefabs;
- the #894 prototype lane `/media/jer/ZX20/CCF-894-RootFlare-20261120` and the #891 lane;
- editing or deleting **any** existing source, archive, FBX or texture, including `Archives/UnityAssets/**`, the owner's downloaded ZIPs and the old `Local Game Dev/Sitka_*` folders;
- pruning states (Phase B, after approval);
- other species;
- new packages, add-ons or system installs;
- collider, simulation, gameplay or save changes;
- deciding Unity integration;
- committing licensed or derived binaries anywhere public. **D-032 binary storage is still OPEN.**

## 6. Write owner and target paths (exclusive to Astra for this task)

- **Work folder:** `/media/jer/Files/Documents/ChatGPT/Local Game Dev/Sitka_RootFlare_01/`. Copy the needed sources and scans in here, and keep scripts, intermediate `.blend` files and bakes here.
- **Delivery package:** `/media/jer/Files/CCF/Archives/UnityAssets/Sitka_RootFlare_01/`. This is a **new folder** following the CCF convention: the old `/media/jer/ZX20/Unity Assets/` maps to it under `Docs/Operations/StorageArchitecture.md`.
- **Result handoff:** `/media/jer/Files/Documents/ChatGPT/Local Game Dev/Handoffs/CCF-894-Astra-Sitka-RootFlare-4-Models/RESULT.md`.
- Nothing else. Do not create new evidence or build paths on ZX20.

### Expected delivery layout

```
Sitka_RootFlare_01/
  README.md                         # method, pairing, fit numbers, limits, biology caveat
  Asset_Manifest.json               # per asset: source model+scan (+sha256), authored height, junction height, footprint, LOD tris, files
  Validation_Summary.json
  Textures/                         # shared original bark/foliage + per-asset lower-trunk bakes (2K PNG)
  <Asset>/                          # x4, e.g. Sitka_Mature_Benchmark_01_RootFlare_01/
    <Asset>.blend                   # editable, all LODs at the soil pivot, packed or relative textures
    <Asset>_Unpruned_LOD0.fbx / _LOD1.fbx / _LOD2.fbx
    Manifest.json  Validation.json
    Previews/  full_tree.png  base_walk_eye.png  base_close.png  junction_grazing.png  lod_lineup.png  before_after.png  moss_mask.png
  scripts/                          # every script you ran (build, bake, validate, render)
```

## 7. Allowed tools and environment

- **Blender 4.0.2** at `/usr/bin/blender` (Ubuntu package). On 2026-11-21 the owner had **`python3-numpy` 1.26.4** installed, so FBX import and export now work in headless Blender. That was verified with the thksce3da Mid scan (metres, 1 UV layer) and Benchmark LOD0 (Trunk/Branches/Foliage, 26 m), plus an FBX export round-trip.
- **Headless recipe:**

  ```
  BLENDER_USER_CONFIG=… BLENDER_USER_SCRIPTS=… BLENDER_USER_DATAFILES=… TMPDIR=… blender -b --factory-startup -noaudio --python script.py -- args
  ```

  - Use a task-local folder for the environment variables.
  - Blender may write `/tmp/quit.blend`. Move it aside or report it; it overwrites the owner's "Recover Last Session".
  - An unfocused GUI Blender throttles.
- **Cycles baking in headless mode is UNTESTED here.** OIDN is unavailable, so **turn denoising off**. Run a tiny bake test first. EEVEE cannot bake.
- Python 3 with Pillow is available for image compositing and hashing.
- **Not allowed:**
  - Unity;
  - Git writes;
  - installing anything;
  - network downloads (ask the owner for any extra Fab asset, such as a Mid `ugghcjqfa`);
  - subagents, unless the owner asks for them.

## 8. Acceptance criteria (Phase A)

| # | Criterion |
|---|---|
| A-1 | Four assets exist in the delivery layout, named and paired per §3.3 or the owner's later choice, each with LOD0–2 FBX and an editable `.blend`. |
| A-2 | Above the junction band, each asset's crown, branches and foliage are **geometrically identical** to the original Unpruned LOD of the same level. Same vertex count and positions within 1 mm, same materials and same object split. Proven by an automated comparison in `Validation.json`. |
| A-3 | Authored height within ±2 cm of the original; pivot at soil; ground contact within 5 mm (lowest point, plus no floating roots); -Z forward / Y up export; metre scale; no cameras, lights, animation, colliders or modifiers in the FBX. |
| A-4 | The trunk is **one continuous surface** through the junction: no overlapping or duplicate trunk shells, no step greater than 1 mm, no open holes visible from outside. Report the non-manifold and boundary edge counts at the junction. |
| A-5 | **Zero visible join** in the bark: `junction_grazing.png` and `base_close.png` show no colour, normal, scale or AO line. Bark texel scale is the same above and below within 10 % (report the measured px/m). |
| A-6 | **Moss and soil present at the base** from the scan, with an irregular, faded upper edge; `moss_mask.png` is supplied. No scan photo bark remains outside the mask. |
| A-7 | LOD triangle counts within the §4.3 budgets. Zero degenerate faces; finite coordinates; all UVs present; texture paths resolve on reimport. |
| A-8 | Every FBX is independently reimported in a fresh Blender session and checked as in the existing `validate_*.py` scripts. Results go in `Validation.json` / `Validation_Summary.json`. |
| A-9 | A before/after preview per asset, original against new, from the same camera, at walking eye height (1.65 m, ~3 m away) and close (0.4–0.6 m), under neutral lighting. |
| A-10 | The README records:<br>• the pairing, fit scale, junction height and footprint (in authored metres, and at 8 / 12 / 16 m in-game heights for scaled use);<br>• the biology caveat;<br>• the scan IDs and hashes;<br>• the Phase B plan;<br>• known limits. |
| A-11 | No file outside §6 changed. Report a before/after listing (or hash list) of the source folders you read. |

**Verification is visual and automated in Blender only.** In-game acceptance, LOD transitions, performance, wind and Unity import are **NOT TESTED** by this task and must be reported as such.

## 9. Stop conditions (stop that stage, report evidence, do not improvise around it)

- **S-1 Identity:** a scan file is missing or its hash differs, or the original `.blend` does not contain the expected Unpruned LOD objects.
- **S-2 Scope:** meeting A-2 (unchanged upper tree) would require editing the crown or branches.
- **S-3 Join:** after two honest attempts, a visible join remains at walking distance on any asset. Deliver your best attempt plus close-ups, and explain why.
- **S-4 Bake:** Cycles baking fails headless. Report the exact error. Do not switch to GUI clicking, and do not install anything.
- **S-5 Fit:** a scan cannot be fitted to its model without non-uniform stretching, or it gives an implausible flare. Propose a re-pairing, such as swapping in the Nordic reserve; do not apply it unasked.
- **S-6 Budget:** a LOD cannot meet the §4.3 budget without visibly breaking the base.
- **S-7 Environment:** you are in a read-only mirror or lack access to the §3 paths. This happened in #243. Report and stop.

## 10. Open questions (defaults apply unless the owner changes them before dispatch)

- **O-1 Pairing.** Use §3.3 by default. Note that Phase 0 rated **Sitka_Mature_01 as the weaker mature model, a REPLACE candidate** in favour of Benchmark_01. The owner's instruction here keeps "the upper part the same as the original models", so Mature_01 is included as asked. The owner may prefer two Benchmark-upper variants instead.
- **O-2 ugghcjqfa is Raw only.** The default is to decimate in Blender to Mid-like ~4–6 k tris for LOD0 geometry and bake detail from the Raw. Alternatively, the owner downloads the **Mid** zip.
- **O-3 Pruning states.** Phase B would add Low, Medium and High pruning, each Recent or Healed (21 FBX per asset), using `build_pruning.py`, **after** the owner approves Phase A visually.
- **O-4 Production use.** Putting these into Compact30 means a Unity integration task: new prefabs, swapping scene visual children while keeping IDs, colliders and roster, and #886 collision reconciliation. That task is **SPEC REQUIRED** and separate. This packet authorises none of it.

## 11. Deliverable and next recipient

- **Astra delivers:** the §6 package plus `RESULT.md`. RESULT.md must state the pairing used, the per-asset fit numbers, the A-1…A-11 table with PASS / FAIL / NOT TESTED, the exact paths and sha256 of every delivered file, open limits, and whether Phase B is recommended.
- **Next:**
  1. The **owner** does a visual review of the previews.
  2. If approved, an **independent reviewer**, not Astra and not the prototype author, checks A-1…A-11 from the raw files.
  3. A separate **Unity integration** task follows, with a spec, its own lease and native tests.
