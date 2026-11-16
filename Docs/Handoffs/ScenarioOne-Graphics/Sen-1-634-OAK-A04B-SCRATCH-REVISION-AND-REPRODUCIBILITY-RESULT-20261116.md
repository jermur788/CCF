# Sen-1 #634 — Oak A-04b scratch revision and reproducibility result

**Date:** 16 November 2026. **Issue:** [#634](https://github.com/jermur788/CCF/issues/634).

**Inputs:**
- [#632 review](https://github.com/jermur788/CCF/blob/review/sen-1-632-oak-juvenile-botany-art-scratch-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-632-INDEPENDENT-OAK-JUVENILE-BOTANY-AND-SCRATCH-ART-REVIEW-20261116.md) (NEEDS CHANGES)
- [#425 Manager handoff](https://github.com/jermur788/CCF/blob/docs/sen-1-425-632-review-verdict-634-asset-revision-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-425-MANAGER-632-OAK-REVIEW-VERDICT-AND-634-A04B-REVISION-HANDOFF-20261116.md)
- [#627 A-04a result](https://github.com/jermur788/CCF/blob/docs/sen-1-627-linux-oak-scratch-resource-profile-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-627-LINUX-OAK-JUVENILE-SCRATCH-AND-RESOURCE-PROFILE-20261116.md)

**Asset:** `Oak_Sessile_Juv060_A04b_Scratch`. Label: **"0.6m sessile Oak bare-root 1+1 SCRATCH CANDIDATE; design assumption pending owner/gameplay decision"**.
- **Status: SCRATCH CANDIDATE only.** It is not reviewed, accepted, integrated, verified or approved for import, distribution or production. D-032 stays OPEN and #619 stays DRAFT.
- The 1+1 stock type is the Manager's **reversible art hypothesis**, not owner ratification. No game mechanic depends on it.
- A-04a keeps its label "0.6m nursery transplant, age not established" and is unchanged.

## 1. Worker, lane, custody and process permissions

| Item | Value |
|---|---|
| Worker | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code. **Sole A-04b art writer.** Registered [RUNNING] on #634 before any scratch, Blender or Markdown work ([comment](https://github.com/jermur788/CCF/issues/634#issuecomment-6924881682)). |
| Authorship note | This session also wrote the #632 review, so the reviewer is now the author. **The follow-on changed-lines review must be by a different model family** (not Claude), as #634 specifies. |
| Host | Linux `jer-laptop` (Ubuntu 24.04 base, Mint Cinnamon, X11; i5-6300HQ, 15.5 GiB, GTX 980M). Blender `/usr/bin/blender` 4.0.2 Ubuntu package. **Nothing installed or downloaded.** |
| Docs branch / worktree | `docs/sen-1-634-oak-a04b-revision-20261116-v1` at `/media/jer/Files/CCF/Operations/CCF-634-Oak-A04b-Revision-20261116` (Files ext4, docs-only, not a Unity lane) |
| Starting HEAD | `37cd047ed39fd649563f70602d458e2970f6780f` (fresh `origin/main`) |
| **New scratch directory** | `/media/jer/Files/CCF/Verification/Sen-1-634-OakA04b-ScratchRevision-20261116/` (Files ext4, outside the repo; vacant before this task, created by this worker, owner `jer`, mode 775) |
| #627 originals | **Read only.** The script was copied (not edited). Old renders and OBJs were read for comparison only. `sha256sum -c` on the #627 `MANIFEST.sha256` gave **35/35 OK** after all work, and **no file in the #627 directory is newer than its manifest**. |
| Tracked output | This one Markdown file. No binaries, images, meshes, `Assets/`, C#, scenes, prefabs, saves, Prefs, `ProjectSettings/`, `Packages/` or CCF-Ops changes. The owner's dirty `CCF-Primary` (`AGENTS.md`, `StorageArchitecture.md`) was not touched. |

**Preflight at about 18:12 UTC, before registration:**
- `unity-lock status` (read-only) reported `free`, metadata `None`, no Unity Editor PIDs. No lease was taken.
- `/tmp/quit.blend` was **absent**.
- No `blender` process was running. A `blender-mcp` Python bridge process was running; it was not used.
- The owner was using the desktop, so the work was **headless only**.

**Temp and desktop protection, a hard gate that held throughout:**
- Every Blender run went through `tools/run_iso.sh`. Before each run it **stops if `/tmp/quit.blend` exists** (never copying, moving or deleting it) and stops if another `blender` process is running.
- It sets `TMPDIR` and `BLENDER_USER_CONFIG/SCRIPTS/DATAFILES` to `blender-iso/` inside the scratch directory, and runs `blender -b --factory-startup` only.
- Five runs, logged in `runs.log`: four builds and one EEVEE render. Each logged `/tmp/quit.blend absent` after exit, and `blender-iso/tmp` and `config` were empty afterwards.
- **No Blender GUI, no window and no focus change.** GUI-mode temp isolation is still **not qualified**, because none was attempted.
- The owner's `~/.config/blender` was not touched.

## 2. What changed and why (closing #632)

All values are **calibration** unless a source is named. The provisional 1+1 reference and the 11 mm root-collar minimum come from Leslie (2024), [*Best practice prescriptions for propagating and establishing pedunculate oak*](https://www.futuretrees.org/wp-content/uploads/2024/11/ftt-monograph-oak.pdf) (Future Trees Trust; Forestry Commission funded), p. 7. That source covers UK oak propagation; **it is not an Irish average or an Irish rule**.

| #632 ID | A-04a | A-04b change | Measured result |
|---|---|---|---|
| **M-1** stem age | The whole 0.6 m stem was one season's spring + lammas growth, with leaves from 0.08 m upward. | A **1+1 plant in leaf in its second growing season**, shown at about lift size.<br>• **Year-1 wood** from the collar to the **year-1 terminal bud-scar ring at 0.240 m**: leafless, greyer bark, 3 dormant axillary buds, 6 bud-scale scar bumps, and a ring swelling of ×1.14.<br>• **Year-2 (current-season) growth**: spring flush 0.24 → 0.44 m, a flush-boundary ring, a lammas flush to the 0.600 m bud top, two side shoots from the year-1 terminal bud cluster, one lammas side shoot, and one short shoot from a lateral bud on year-1 wood.<br>• **Leaves only on current-season shoots.**<br>• Materials renamed `Oak_A04b_Wood_Year1_PreviousSeason`, `Oak_A04b_Shoot_Year2_CurrentSeason` and `Oak_A04b_Leaf`. | Visible in renders `01`, `02` and `05`: grey basal wood up to the ring, red-brown current growth above it. The year-1/year-2 split height is **calibration**. A one-year 0.6 m height is **not** claimed. |
| **M-2** root collar | 9.2 mm nominal. Measured minimum width at 0–5 mm height: 9.4 mm (LOD0), 8.5–8.7 (LOD1), 7.3–7.4 (LOD2). | Target minimum visible width ≥ 11 mm across the collar zone (12.6 → 12.0 mm). Each LOD's ring radius is **polygon-compensated** (circumradius = width ÷ the n-gon minimum-caliper factor), so even the 3-sided LOD2 stays ≥ 11 mm. | Measured from mesh cross-sections at z = 0 / 5 / 10 / 20 mm (minimum to maximum caliper width, mm):<br>• **LOD0:** 12.51–13.55 / 12.36–13.39 / 12.12–13.13 / 11.78–12.76<br>• **LOD1:** 12.18–12.82 / 12.07–12.71 / 11.97–12.60 / 11.76–12.38<br>• **LOD2:** 12.16–14.04 / 12.05–13.91 / 11.94–13.79 / 11.71–13.53<br>Computed both inside Blender (`stats.json` `collar_sections`) and independently from the OBJ files; the two agree. **All ≥ 11 mm.** No universal Irish minimum is claimed. |
| **m-1** leaf widest point | Envelope peak at 0.669 of blade length. | Envelope `u^1.0 (1-u)^0.85` peaks at **0.541**. The exponent 1.0 keeps a straight wedge-shaped base with no auricles. | Per-leaf position of the widest actual margin vertex, LOD0: **mean 0.535**, range 0.471–0.609. The spread comes from where individual lobe tips fall. |
| **m-2** lobe pairs | 4–5 | 5–7 pairs (n parameter: eleven 5s, eleven 6s, seven 7s), still shallow and regular. Petiole 12–22 mm (n-3). | Lobes counted as margin maxima per side, then halved (Potter-style), LOD0: **min 5.0, max 7.0, mean 5.86 pairs**. Petiole %: 17.9–41.0 (mean 27.6). Blade 44–85 mm (calibration, unverified). |
| **m-3** LOD2 attachment | Floating leaves | LOD2 (and LOD1, see m-4) leaves get a **one-triangle stalk from the LOD's own stem axis, welded to the blade base vertex**. | Mesh test (§4): **0 floating leaves out of 29 at LOD0, LOD1 and LOD2.** |
| **m-4** LOD value | 5,171 / 2,188 / 222 tris; LOD2 used rhombic plates. | LOD1 uses tip-and-sinus lobe sampling and a 1-tri stalk. LOD2 uses obovate 8-tri blades (widest about 0.51, wedge base) instead of rhombi. | **6,044 / 1,196 / 311 tris.** LOD1 is 19.8 % of LOD0 (A-04a: 42 %). LOD2 now keeps the obovate silhouette and stalks, for +89 tris. |
| **m-5** venation / shading | Flat V-fold, faceted bands | LOD0 gets an inner vein-direction row at about 45 % of the half-width, and a cupped (non-planar) fold. | Close-up `04` shows smoother blades. **Real veins are DEFERRED**: they need a texture or normal map, which falls under D-032/#619 binary policy. |
| **m-6** fold / orientation | Fold 0.18–0.30, elevation −24° to +8°, roll ±22° | Fold 0.06–0.13, curl 0.04–0.09, elevation −28° to +16°, roll ±30° | Fewer edge-on "half-moons" in `01`/`03`. **Still present in the orthographic front view `02`**: oak leaves held near horizontal will look edge-on in any eye-level elevation. Judging this properly needs the gameplay camera pitch, which is not yet defined. |
| **m-7** overlap | 2 intersecting leaf pairs (LOD0, measured here) | Deterministic greedy clearance: each blade centre is kept ≥ 0.62 × mean blade length from earlier blade centres, trying azimuth offsets of ±25° steps. | **0 intersecting leaf pairs and 0 near-contact (< 2 mm) pairs at every LOD.** One leaf was moved. Leaves still *overlap in plan view* (`03`), which is natural layering, not interpenetration. |

**Unchanged on purpose:**
- seed 627;
- origin at the root collar, Z up, metres;
- stem continuing 0.02 m below ground;
- leader bud top at 0.600 m (the tallest vertex is a leaf tip at 0.6022 m);
- `_LOD0/1/2` naming;
- 3 materials;
- the reference scene and the five render cameras (copied unchanged from #627 `render_previews.py`; only the docstring differs).

## 3. Reproducibility and hashes

**Rebuild.** I rebuilt from the final script into `rebuild-verify/`. **All three OBJs, all three MTLs and `_stats.json` are byte-identical** to `asset/`. The `.blend` is not byte-identical, as is expected for `.blend` files.

**Scratch manifest.** `MANIFEST.sha256` covers 34 files (everything except `blender-iso/`, which is empty). The manifest itself hashes to `4e9f2a27de2e05523fddb990e1723e7c8b3deaa60bdf6f8a339ce0fa50bd9179`.

| File (relative to scratch dir) | SHA-256 |
|---|---|
| `tools/build_oak_a04b.py` (**source**) | `bdcfb6cf940aa93f9710267025a63c6f593ba0f06ba3782267d85fdafddc6268` |
| `tools/run_iso.sh` (isolated runner + hard gate) | `7ca9adfc8a41adfc3eaf9aa40117ba8a2bb1e4f2f27636f13b7fa74a4178a0b8` |
| `tools/render_a04b.py` | `4b48059306ead2e6ec322c0957375761de81694e75fcaebae6ecdd9bb30c4d47` |
| `tools/mesh_checks.py` | `5ee077f5d8958b68e878fb3a363a4d9caab8e52a0504dd4b99b5276bdabe45a1` |
| `asset/Oak_Sessile_Juv060_A04b_Scratch.blend` | `ee0bf444324152d9c1d8c4bb64c51f54c912c86d5757bf0a39102e7434d05c7e` |
| `asset/…_LOD0.obj` | `3bf85983cf5c9f03fadcaa0d6037557fb19b6b45d57c778a411bffa0864eca80` |
| `asset/…_LOD1.obj` | `9c04da496a25b0cb8f6ad7e517a15560546d8dac9af07874e414b0a7ddbde706` |
| `asset/…_LOD2.obj` | `3da81017179f7e78fe9a0dea358baaa3ae175d87e480d95f9474789907ac10df` |
| `asset/…_LODn.mtl` (identical ×3) | `8bdeab95a44002227a23c33d486dd787908fa666355a10cf589a8fbf47ad53da` |
| `asset/…_stats.json` | `5820ea2826f932449f01b60c4d479ed0bbfe28b0c210814c534236ac3790c642` |
| `asset/PROVENANCE.md` | `6c0b8e162c128f415bca434a4b5f33f9e44185bfe2ee1d4bf4b7ead05d6cd4cd` |
| `checks/mesh_checks.json` | `aadd3f9968abefaeb76f8522c893b3aa8ae8018c803aa07297b8caab50f0571a` |
| `renders/01_hero_three_quarter.png` | `fe98b154318897692bbf51c28209708da743e954423a1e5296250cf495f4c020` |
| `renders/02_front_ortho_scale.png` | `ae132300456dda3eceda60a41d7f01172c33e31989812225eed87c14d3901654` |
| `renders/03_top_plan.png` | `0d87d25ad267f9c9ab80b6c10b72b91d2148e175943223895fba67ede430cbeb` |
| `renders/04_leaf_closeup.png` | `6964574932b7541ed4bd195b66c68331b6bc9e2c3a82ebcaa8d8d16e3a6f0159` |
| `renders/05_lod_lineup.png` | `8a1773ca07419d71c298fb1ff2242c62f6eac67d8786df68354d5fc7dbde3de4` |
| `compare/cmp_01…cmp_05_*.png` (A-04a left, read-only; A-04b right) | `c225571c…8ddfa`, `c4d981d4…f7736`, `a5212186…9f53e`, `a63b0308…f059a`, `4385fb65…791ba2` |

**Mesh provenance and licence.**
- Original procedural geometry, derived only from the #627 script (`76edba3b…a3b`).
- No external meshes, textures, scans, photographs, add-ons, asset libraries or AI-image references were used.
- Licence and rights are **not determined** and are governed by D-032.

## 4. Mesh checks (`tools/mesh_checks.py`, pure Python, OBJ only)

**Method:**
- Leaf assemblies are connected components that contain leaf-material faces.
- A leaf counts as **attached** if one of its edges crosses a stem, shoot, petiole or bud triangle, or if one of its vertices is within 0.1 mm of one. Otherwise its gap is the minimum vertex-to-triangle distance.
- Leaf–leaf intersections are found with edge–triangle tests.

| Mesh | Tris | Leaves | Floating | Floating gap (mm, min/median/max) | Intersecting leaf pairs | Near-contact < 2 mm |
|---|---:|---:|---:|---|---:|---:|
| A-04a LOD0 | 5,171 | 30 | 0 | 0 / 0 / 0 | **2** | 1 |
| A-04a LOD1 | 2,188 | 30 | 0 | 0 / 0 / 0 | **2** | 0 |
| A-04a LOD2 | 222 | 30 | **29** | 0 / 9.56 / 13.75 | 1 | 0 |
| **A-04b LOD0** | 6,044 | 29 | **0** | 0 / 0 / 0 | **0** | 0 |
| **A-04b LOD1** | 1,196 | 29 | **0** | 0 / 0 / 0 | **0** | 0 |
| **A-04b LOD2** | 311 | 29 | **0** | 0 / 0 / 0 | **0** | 0 |

**Correction to #632 m-3.** #632 reported LOD2 gaps of 7.9 / 17.4 / 45.0 mm using a *vertex-to-vertex* proxy, which overstates them. The *vertex-to-triangle* method here gives **29/30 leaves floating at up to 13.75 mm** (median 9.56 mm). The finding stands; its numbers are corrected.

**Visual inspection.** I viewed all five A-04b renders and all five side-by-side images. In `05`, no LOD2 leaf visibly floats.

## 5. Visual result and remaining limitations (honest list)

**Improvements, from my own reading of the side-by-side images:**
- The 1+1 structure is readable: a thicker grey leafless base, the bud-scar ring, and current shoots above it.
- In plan view (`03`) the leaves read as elongated, obovate and regularly lobed, much closer to a sessile-oak leaf than A-04a's "cloud" blades.
- The LOD2 silhouette now matches LOD0 instead of looking like rhombic plates.

**Still limited or unverified:**
1. **LOD1 lobes read as zig-zag teeth**, because only tip and sinus are sampled. This is acceptable at mid distance, but a reviewer may prefer 3 samples per cycle (about +300 tris).
2. **Edge-on leaves in the orthographic front view** remain (m-6, partial).
3. **No venation, hairs or underside colour.** These need textures and are deferred under D-032/#619.
4. **Leaves are single-sided.** URP will need *Render Face: Both*. This is a future integration note and was not tested.
5. **The state shown is "1+1 in its second nursery summer"**, not "first summer after field planting". A planted-out variant, with transplant check and year-3 shoots, would be a separate future variant and depends on the not-yet-defined growth model.
6. **Calibration, not evidence:** the year-1/2 split at 0.24 m, the lammas boundary at 0.44 m, 29 leaves, 4 side shoots, the 0.26 × 0.24 m crown, blade 44–85 mm, petiole 12–22 mm, a 12–12.6 mm collar (above the 11 mm minimum), the dormant buds and scar bumps, and all colours.
7. **720p/1080p and gameplay readability: NOT ASSESSED.** There are no in-engine screenshots and no Unity was used. No gameplay PASS is claimed.
8. **Independence:** the author is the same model and session as the #632 reviewer, so the review in §7 must come from a different model family.

## 6. FBX, Unity and resources

**FBX: not produced.** Ubuntu's Blender 4.0.2 FBX add-on needs numpy, and this task installs and downloads nothing. **OBJ is the scratch output.** A native single-file FBX with an auto-generated LODGroup, and any Unity prefab, are **future, separately approved** integration work: they need an owner-approved Blender build, D-032 and a signed #619.

**Unity:** none was launched and no lease was taken.

**Resources:** headless builds took about 1–2 s each and the five EEVEE previews about 4 s in total. These were not measured as a benchmark. Unity and Blender running together is still **NOT TESTED**, so the **TIME-SLICED** rule stands.

## 7. Next action

1. **Different-family changed-lines review** (for example GPT-6.1 Sol High) of A-04b against #632's M-1, M-2 and m-1…m-7 only, using the scratch files in §3 and `checks/mesh_checks.json`. It is not a repeat of the provenance or resource audit.
2. **Manager / owner:** decide whether bare-root 1+1 becomes the reference stock type, whether LOD1 should get 3 samples per lobe, and when an FBX route is approved. None of this blocks Linux core gameplay.
