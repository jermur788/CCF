# CCF #894 — Phase 0 inspection result (Nordic trunk root-flare graft)

**Verdict: INSPECTION ONLY / UNITY WAITING ON LEGAL LEASE / ASSET FORMAT BLOCKED / NOT INDEPENDENTLY REVIEWED / NOT INTEGRATED / NOT ACCEPTED**

- **Date:** 2026-11-20 ~23:05 UTC
- **Author:** Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, jer-laptop. This is a fresh session. It is **not** the #891 WG-ground Opus actor.
- **Packet:** `docs/894-opus-trunk-root-flare-prototype-20261120-v1` @ `8e9c988cb2680610d6bc99c8774bf96926bb9b93`, including the 22:48 UTC owner amendment.
- **Reference source inspected:** #876 `fix/sen1-876-mark-visual-scale-20261120-v1` @ `fce41bd3b77a054bf82d2521648d15ec18f81141`. I read it from Git blobs only.
- **What was not run:** Unity was not launched and no Unity lock was claimed. The #891 worktree was not entered. No asset bytes were extracted into any repository or worktree, and the owner's ZIP was left untouched.

## 1. Asset identity: confirmed, but the download is the wrong format

| Field | Value |
|---|---|
| File | `/media/jer/ZX20/NewAssets/nordic_forest_tree_trunk_spruce_large_zbrush.zip` |
| Size / SHA-256 | 1,098,039,240 B / `e432ce914bb91cf885d44cceae2d3b84d253867cc84932c108d8e037e9172dd8` |
| Product name (from the archive JSON) | **Nordic Forest Tree Trunk Spruce Large**, pack `nordic-forest` |
| Quixel internal ID | `xeticd0ga` (process date 2023-12-08) |
| Tags | spruce, roots, moss, bark, "old"; Sweden; taiga/forest biome; mesh type **Open** |
| Real-world size | footprint 1.75 × 1.81 m, height 2.52 m (`isScaleFixed: true`) |

**"Bruce Large" was a speech misrecognition of "Spruce Large".** Only one candidate matches, so no owner choice is needed on identity.

**Blocker: the archive contains only `xeticd0ga.json` and `xeticd0ga.ztl`.** The `.ztl` file is a 1.1 GB native ZBrush document (header `ZBrush File… PIXOLOGIC`). The JSON is a catalogue record that *lists* FBX tiers (LOD0 999,999 tris, LOD1 18,250, LOD2 4,348, LOD3 920) and 4K/8K maps. **None of those FBX files or textures are in this ZIP.** Neither Blender nor Unity can open `.ztl`, and ZBrush is not installed. A graft cannot start from this download.

**Owner action needed:** re-download the same asset (`xeticd0ga`) from Fab as the **FBX / standard 3D format with 2K textures**, and place it in `/media/jer/ZX20/NewAssets/`. LOD1–LOD3 plus 2K basecolor/normal/roughness/AO are enough for this prototype. The 8K/ZBrush tiers are not needed.

**Licence:** the archive has no licence file. Entitlement, and any Fab Standard licence terms covering AI-assisted processing, are **UNVERIFIED**. Under D-032 (OPEN), the bytes stay local-only. This document records metadata only.

**Fit risk to flag now:** a 1.8 m footprint and the "old" tag suggest a large veteran spruce bole. The best in-scene mature model (below) is authored with roughly **48 cm DBH**. If the scan's stem is much thicker than that, it can't simply be scaled down without shrinking bark texel density and root-buttress scale unrealistically. I can only measure the stem diameter once the FBX is available. The result may be **UNSUITABLE for anything except very large trees**. In that case the alternative is a scan or flare sized to the trunk, rather than forcing this one to fit.

## 2. Spruce variants actually used in `ScenarioOneCompact30.unity` at fce41bd3

The scene references exactly four spruce prefabs, all "Unpruned", and no other tree prefabs.

| Prefab (GUID) | Instances in scene file | Mesh source | Bark material | Renderers / LOD |
|---|---:|---|---|---|
| `Sitka_Young_02_Unpruned` (`c54cc46c`) | 114 | CropTreePruning `Sitka_Young_02_Unpruned_LOD0-2.fbx` | `Sitka_Bark.mat` (albedo-only, older family) | 3 × 3 LODs |
| `Sitka_Pole_01_Unpruned` (`6fc4c8b0`) | 109 | CropTreePruning `Sitka_Pole_01_Unpruned_LOD0-2.fbx` | `Sitka_Pole_Bark.mat` | 3 × 3 |
| `Sitka_Mature_Benchmark_01_Unpruned` (`69a172cf`) | 92 | SitkaMatureBenchmark `…Unpruned_LOD0-2.fbx` (44,050 / 19,690 / 6,762 tris) | `SS_Benchmark_Bark.mat` (v2 basecolor + normal + roughness) | 3 × 3 (bark / branches / needles) |
| `Sitka_Mature_01_Unpruned` (`b3a74f27`) | 86 | CropTreePruning `Sitka_Mature_01_Unpruned_LOD0-2.fbx` | `Sitka_Bark.mat` (albedo-only) | 2 × 3 (bark / needles) |

These counts are prefab instances in the scene YAML, 401 in total. The packet's "175 trees" is the authored roster binding (`TryMapAuthoredTrees`). **I have not resolved which instances are roster-bound and which are surround/backdrop.**

### Ranking (preliminary)

The evidence here is provenance plus mesh and material data. **No eye-level renders were made.** Ubuntu Blender 4.0.2 cannot import FBX because NumPy is missing, and Unity was off-limits during the #891 owner test.

1. **`Sitka_Mature_Benchmark_01` — best mature upper/canopy. Proposed as the graft exemplar.** It is documented as the refined revision of the mature family: 26 m tall, ~48 cm DBH, soil-level pivot, normal-mapped v2 bark, irregular crown, baked needle atlas, and three purpose-built LODs with the trunk and branches as separate parts. Its documentation explicitly supersedes the earlier procedural bark. *The bark being a separate object makes it the most likely to allow a hide/trim of the lower trunk without touching the crown. This is UNVERIFIED until I can inspect it in Unity.*
2. **`Sitka_Mature_01` — REPLACE candidate (same size class as #1).** Older CropTreePruning family: albedo-only bark and only two parts per LOD, so the trunk and branches are likely merged.
3. **`Sitka_Pole_01` — AGE-SPECIFIC.** No equivalent pole-class benchmark exists, so keep it until one does.
4. **`Sitka_Young_02` — AGE-SPECIFIC.** Same reasoning as #3.

### Proposed KEEP / REPLACE / AGE-SPECIFIC matrix (proposal only; no changes made)

| Variant | Count | Category | Proposed future action (needs SPEC REQUIRED approval) |
|---|---:|---|---|
| Sitka_Mature_Benchmark_01_Unpruned | 92 | **KEEP** | Receives the root-flare component if the prototype passes |
| Sitka_Mature_01_Unpruned | 86 | **REPLACE** | Swap the visual child only, to Benchmark_01. Keep ID, XY, height/DBH, collider, markings and stump behaviour. Check the per-instance scale first, because Mature_01's authoring height is not yet compared with Benchmark's 26 m |
| Sitka_Pole_01_Unpruned | 109 | **AGE-SPECIFIC** | Keep. Would need a pole-class refinement asset to upgrade |
| Sitka_Young_02_Unpruned | 114 | **AGE-SPECIFIC** | Keep. Same as above |

No files are to be deleted. "Remove" means the approved scene stops referencing a variant; the source assets stay for rollback.

## 3. Lease and coordination state observed

- `unity-lock status` at 2026-11-20 ~22:55 UTC: **free**, no Editor PIDs. The Unity Hub (Flatpak) was running.
- The Ops board row `CCF-891-WG-GROUND` is in **VERIFY** (owner visual test pending). The packet says not to launch Unity during that test, so Phase 1 waits even though the lock is free.
- origin/main `active-tasks.md` @ `cfc802d2` has the #894 amendment entry and says the #894 worker is not claimed. I did not claim a board row; Phase 0 is read-only.

## 4. Changed files

- `Docs/Handoffs/ScenarioOne-3D-Art/CCF-894-PHASE0-INSPECTION-RESULT-20261120.md` (this file). It is the only file on branch `docs/894-opus-phase0-inspection-20261120-v1`, based on 8e9c988c.

## 5. Next steps (in order)

1. **Owner:** re-download `xeticd0ga` in FBX + 2K format to `/media/jer/ZX20/NewAssets/`.
2. **Owner:** finish the #891 visual test and release it, then confirm Phase 1 may start.
3. **#894 worker (Phase 1):** measure the scan's stem diameter against the 48 cm Benchmark. If it fits, claim the board row and the D-091 lock, create the art worktree from fce41bd3, and stage only under `Assets/ForestPrototype/Art/Experiments/CCF894/`, using a disposable scene copy. Then make eye-level renders of all four variants to confirm the ranking above, and build one Benchmark tree with a visual-only flare child (no collider). Finish with matched before/after shots, a smoke test, and lock release.
4. A forest-wide swap stays **SPEC REQUIRED**.
