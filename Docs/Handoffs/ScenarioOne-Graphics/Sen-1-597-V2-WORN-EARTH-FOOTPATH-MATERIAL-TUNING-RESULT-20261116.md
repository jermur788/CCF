# Sen-1 #605 — Worn-earth footpath material: v1 → v2 bounded tuning pass result

**Issue:** [#605](https://github.com/jermur788/CCF/issues/605), which tunes [#597](https://github.com/jermur788/CCF/issues/597) after the [#603 independent review](https://github.com/jermur788/CCF/blob/review/sen-1-603-597-wornearth-scratch-art-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-603-INDEPENDENT-597-WORN-EARTH-MATERIAL-SCRATCH-REVIEW-RESULT-20261116.md) returned NEEDS CHANGES (moderate).
**Date:** 2026-11-16.
**Status:** **v2 SCRATCH CANDIDATE, AWAITING NONAUTHOR RE-REVIEW.** It is not approved and has no Unity or production GO.

## 1. Worker, registration and lane

| Item | Value |
|---|---|
| Worker | Claude **Opus 5.5** (`claude-opus-5-5`), Claude Code in T3 Code, Windows host. Not the #597 author (Sonnet 5.5) and not the #603 reviewer session. |
| Tuning issue | The owner authorised opening #605 in chat because #424 had not yet opened one. #424 is notified. |
| Registration | [#605 comment](https://github.com/jermur788/CCF/issues/605#issuecomment-6918763589), 2026-11-16T14:09:45Z. Posted before any scratch file or Git write. |
| Branch | `docs/sen-1-597-v2-worn-earth-material-tuning-result-20261116-v1` (new, exclusive) |
| Worktree | `C:\Users\hp\.t3\worktrees\CCF\sen-1-597-v2-tuning` |
| Starting HEAD | `6954b6cb9798cbf0dfd32b2eb9106a3dac93fe49` (origin/main) |
| v2 scratch folder (outside Git) | `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-v2-20261116\` |
| Read-only inputs | The v1 folder `…\Sen-1-597-WornEarth-20261116\` and the #603 folder `…\Sen-1-603-WornEarth-Review-20261116\`. Both re-verified unchanged afterwards against their own `SHA256SUMS` (31/31 and 14/14). |
| Tools | Python 3.15.2 with the standard library only; zlib 1.3.1.zlib-ng. **Blender was not installed and not executed. Unity was not used.** All images are offline Python software previews. |

## 2. What changed (all #603 §8 items, one pass)

The material name (`Sen1_Compact_WornEarth_Footpath_Mat01`), seed `597001`, size 1024² and the 1 tile = 1 m assumption are all unchanged.

1. **§8.1 parameters, all applied exactly:**
   - `dampness` 0.42→0.24, `damp_period` 9→13, `damp_edge_softness` 0.40→0.60, `damp_coverage` 0.30→0.24
   - `normal_strength` 1.25→0.45
   - `needle_width_mm` 1.9→1.3, `needle_alpha` 0.90→0.80
   - `col_compact_dry` [132,122,108]→[118,110,98], `col_loose_dark` [108,98,85]→[94,86,75]
2. **§8.2 residual 1 m banding:** both options were trialled at full size with the same renders.
   - **Option A selected:** `compaction_contrast` 0.65→0.45.
   - **Option B rejected:** `compaction_period` 5→7. It brought back distinct light and dark blotches that repeat on every tile in the 3×3, and raised the autocorrelation to 0.152 against A's 0.129.
   - Option B's 3×3, angle view and metrics are kept in `review_aids/`, labelled NOT SELECTED.
3. **§8.3 needle source constants:**
   - (a) Halo albedo darkening `0.10*hl`→`0.03*hl`, and halo height groove `−0.16*nh`→`−0.05*nh`.
   - (b) The needle core tapers to half width at each tip, using `half_w·(1−0.5·t)` along each centre-to-tip segment. This equals the reviewer's `1−0.5·|2T−1|` over the whole needle.
   - (c) Grain height weight 0.60→0.40 (the optional item; applied).
   - The pale straw needle colour `[146,132,106]` was **removed** because no reference supports it. The remaining four needle tones are illustrative only, **not** researched Sitka litter colours.
4. **§8.4 evidence additions:**
   - A new `normal_tilt` metric (median, p90, mean, fraction above 30° and 45°).
   - A new albedo-luma variance share at 12.5 cm blocks.
   - Render 08 redrawn with join ticks in a margin **outside** the image, so the joins are no longer covered.

**Side effect:** edits 3a–3c are in shared code, so they also apply to the illustrative dark-litter reference and to the negative control. Those two surfaces' own parameters are unchanged.

The exact source change is `source/v1_to_v2_source.diff` (183 lines), and it can be rebuilt from v1 with `source/patch_v1_to_v2.py`. Every edit carries a `v2:` comment in the generator.

## 3. Results

**Calibration check:** the v1 baseline below was recomputed by the v2 metric code from the read-only v1 maps. It reproduces the #603 figures exactly (47.4°, 64.8°, 68.7 %, 53.5 % and 0.130), so the two columns are comparable.

| Metric | v1 | #603 trial (params only) | **v2** |
|---|---|---|---|
| Normal tilt median / p90 | 47.4° / 64.8° | 21.5° / 37.6° | **18.85° / 33.66°** |
| Normal tilt >30° / >45° | 68.7 % / 53.5 % | — / 0.7 % | **18.9 % / 0.03 %** |
| Albedo luma variance share, 12.5 cm blocks | 0.130 | 0.111 | **0.083** |
| Within-tile max autocorrelation | 0.132 | — | 0.129 |
| Mean albedo sRGB | 113 / 104 / 90 | 105 / 98 / 85 | **105.1 / 97.8 / 84.8** |
| Roughness mean | 0.815 | — | 0.827 |
| Lit-grey seam ratio (px x/y, block8 x/y) | 1.02 / 1.07 / 1.09 / 1.03 | 1.03 / 1.04 / 1.11 / 1.03 | **1.032 / 1.023 / 1.023 / 1.080** |
| All delivered-map seam ratios (min–max) | 0.90–1.09 | — | **0.864–1.111** |
| Negative-control lit-grey seam ratio | 1.2–2.4 (all maps) | — | **1.305 / 1.222 / 1.869 / 1.930** (height up to 2.575) |

- The tilt target (median ≤ ~25°) is the #603 review calibration target, **not a physical specification**.
- The autocorrelation measures self-similarity within a tile only. As #603 noted, it cannot detect tile-to-tile repetition.

**Visual findings** from my own inspection of the delivered PNGs (author self-assessment, not review evidence):

- **Grit (V-2):** it now reads as fine granular compacted soil, not embossed leather. In the 3× native crops (`review_aids/crop_v1_…` against `crop_v2_…`) the domed-pebble relief is gone.
- **Damp-patch repetition (V-1):**
  - In the 3×3 (`03`), the high-contrast blotch and crescent motif from v1 is gone, and the surface looks almost uniform at review size.
  - **A faint residual 1 m structure remains.** A careful viewer can still pick out soft darker patches recurring per tile. In the angle view (`05`) there are faint soft vertical columns toward the horizon.
  - I judge this **reduced but not eliminated**. The re-reviewer should decide whether it is still "obvious" under the #603 §8.5 stop rule. Under #603 §8.6, a fully repeat-free path needs world-space macro variation at integration, which is out of scope here.
- **Needles (V-3):** they are now thin, tapered and have no dark outline or groove. They read as fine needles lying in the soil surface rather than matchsticks. At 3× zoom a few darker ones still look like fine twigs, and they do not look visibly pressed in. Improved, but the re-reviewer should judge.
- **Colour (V-4):** the surface is darker and less sandy, with mean albedo sRGB 105/98/85. The angle view still has a light grey-brown cast; this is partly the uncalibrated preview light. The lit sRGB grey mean is 0.364, against 0.1735 for the illustrative dark litter. That is a single-scene indicator, not a threshold, and the dark surface is a stand-in, not `Forest Ground.mat`.
- **Read:** it still reads as a walked earth surface, not road, gravel, paving or a strip. No ruts, path width or edges are baked in.
- **Negative control (08):** with the joins uncovered, the broken-tiling joins are **faintly visible** as straight discontinuities, most clearly the horizontal joins. Because they are only faint, the numeric control remains the stronger evidence.

**Stop rule (#603 §8.5):** this was one pass only. No further iteration, new material or Blender build was started. I do **not** declare v2 passed; the residual items above are handed to the re-reviewer.

## 4. Determinism and reproducibility

- Two complete runs into separate folders produced identical generator manifests: `diff` printed nothing, and all 21 deterministic outputs matched. They also matched the option-A selection trial. See `determinism_check.txt`.
- This was one machine only (Windows 11, Python 3.15.2, zlib-ng 1.3.1). Other machines are **NOT CHECKED**.
- The replay commands are in `REPLAY.md`. The core replay is `python -I source/sen1_wornearth_gen_v2.py --all --out <new-empty-folder>`, followed by a `diff` against `SHA256SUMS_generator_run1.txt`.

## 5. Scratch files (outside Git): 41 files plus `SHA256SUMS`, 27,259,688 bytes

**Whole-folder `SHA256SUMS`** = `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673`. It was verified 41/41 with `sha256sum -c` after the final write.

What each folder holds:
- `source/`: the v2 generator, the v1→v2 diff and patch, the whole-folder hash script, the crop helper and the v1 baseline-metric helper.
- `maps/`: five review-only maps: base colour (sRGB), normal (tangent space, OpenGL Y+), roughness, 16-bit height and height-derived AO.
- `renders/`: 01 and 02, the flat tile in colour and grayscale; 03 and 03b, the 3×3 clean and with joins marked; 04, the half-tile seam roll; 05 and 05b, the angle view in colour and grayscale; 06, the channel sheet; 07, the grayscale comparison against the illustrative dark litter; 08, the negative control with ticks outside the image.
- `reference_dark_litter_ILLUSTRATIVE/`: a labelled stand-in surface, not `Forest Ground.mat`.
- `review_aids/`: the option-B trial (not selected), the v1/v2 3× crops and the v1 baseline metrics.
- `metrics.json`, `params.json`, `PROVENANCE.md` (with the v1→v2 change table), `REPLAY.md`, `determinism_check.txt`, the run manifests, the log and the timing.

| File | Bytes | SHA-256 |
|---|---|---|
| `PROVENANCE.md` | 5782 | `169a3a813f7d5022748c6d70ad46d1fa1e46f01c7fa01970a05ac071a2dff998` |
| `REPLAY.md` | 1592 | `2bb85d1d32009f0af6b3e8ecb669d0c3984c2545afbff7fdf75aadaa343d1376` |
| `SHA256SUMS_generator_run1.txt` | 2538 | `9e23c19f21a374dce70c8417aaddb92411d0ba7f488955c35929fb46eacfd274` |
| `SHA256SUMS_generator_run2_replay.txt` | 2538 | `9e23c19f21a374dce70c8417aaddb92411d0ba7f488955c35929fb46eacfd274` |
| `Sen1_Compact_WornEarth_Footpath_Mat01.params.json` | 1373 | `7031e1789267494bd100b4639884c3f62bfcef38a59e78e492d5bc2c5d5a5577` |
| `build_log.txt` | 714 | `668e28d6eb04613a3da0db3f255992459cf7fccd5088da6d7613708bde5a96a6` |
| `build_timing_NOT_HASHED.json` | 522 | `5f219020258f66214af212d51f20f241d287e8171fe1ccac0ac95582fcfe0d23` |
| `build_timing_replay_run2_NOT_HASHED.json` | 525 | `4b852779c67ee780a7e0c36e821decd465a5be0fff533d727b2094ac55f433a4` |
| `determinism_check.txt` | 1484 | `330ee10b81bc8e15f3ab19c83f7e6cf2588a07ce9d8eb492b9d10c91414b4386` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_AO_heightderived.png` | 544672 | `30a06d422710dc279f601b510040ef13a731eb964b6f533a74e3c8c3796887f7` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_BaseColor.png` | 1418088 | `b0de0b456d9b136e532e305fed528444c9a0c960563635a9f965eaf89f8155c0` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Height16.png` | 2085198 | `8fcc9dae2783c08cac4c06115c2a6119fb0a680871f04b756fd4b4fb0b084507` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Normal_GL.png` | 2589774 | `d6d21c97e42b68c36b788f888b924b2a0b6f763dcbf2d594bafb1fd4f41c80a2` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Roughness.png` | 656062 | `36810e66e2df91344d9a0960775fe714eadcdf3606873b185285c74530e2201c` |
| `metrics.json` | 6476 | `d22eb03e8def979da468ff802cb8504163990cd8e591d1c662d3fde0eb1254bd` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_BaseColor.png` | 1176292 | `ae79aeb9b3f955d51458e9a5ef139cfe6d0c1e545ca6b2fbac9d04d389e3314f` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_Normal_GL.png` | 2163091 | `7dbbc85d2fe257aa3bc554812f18dee851d18e5dfd0a3b65e20de2a5083b3de0` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_Roughness.png` | 708328 | `953c8434ac0d35930a52187e1d74ff953118df7afe931f270013327c46f33faa` |
| `reference_dark_litter_ILLUSTRATIVE/renders_flat_neutral_colour.png` | 1527016 | `49ab6179635c84c5a78068a4ea51aa9878235f623f69eb8b21acd0e7ed7743bc` |
| `renders/01_flat_tile_neutral_colour.png` | 1680104 | `32fb05fa7d30d74f71374b3f0ba1e92a5ed70fa205b9875031493df9ce69c3f7` |
| `renders/02_flat_tile_neutral_grayscale.png` | 687346 | `74af4eb7851ab8c9bc4891c0054ccb033565ccedeee1c0bb41a46e159e66de26` |
| `renders/03_tiling_3x3_neutral_clean_1536.png` | 1346619 | `5b05e8f90d105fdb827c46f1f3ece186e7399b34eea98d91d89b7e464f0fc9d5` |
| `renders/03b_tiling_3x3_joins_marked_1536.png` | 1347729 | `3efd5f469a3a73c2ba68e1ac2473c801f6396fa8498b9d047d581d26f2a15817` |
| `renders/04_seam_stress_half_tile_roll_1024.png` | 1679764 | `1d984baba328202714dcaba122458e4b5c43fbc6ef1053cc05ad6d5faeefe461` |
| `renders/05_angle_view_neutral_colour_960x540.png` | 322394 | `27adde24f6d4e78e63ebacd832658e026b3e11375a62b72252681e3a5f64d207` |
| `renders/05b_angle_view_neutral_grayscale_960x540.png` | 148185 | `2d3363c0312ffc6352aa9e63698a004a7a2e3407f13d2b7e5d0ad66c2429d82e` |
| `renders/06_channel_sheet.png` | 2370532 | `633b2051dc621a103bb7dd27bfbeb55558201711c48e83aba087c53216856ec4` |
| `renders/07_grayscale_comparison_vs_illustrative_dark_litter.png` | 1346841 | `b39c2f8a79a3b08087d0e4650f8c82bd96548b5ac8e703d53f578a20aad7af43` |
| `renders/08_negative_control_broken_tiling_3x3_ticks_outside.png` | 1360339 | `2b36a4f94dc84c05000914c40acf22da23bc0c5d7e9aa54d468164736dadad33` |
| `review_aids/crop_v1_01_x300_y400_256px_x3_READONLY_SOURCE.png` | 158648 | `905b755beccfdbdf3b257203427fdb09410ea36b610106b2570e2cb2de280d8f` |
| `review_aids/crop_v2_01_x300_y400_256px_x3.png` | 150779 | `d2ea2f1d1dcf016ad8d81b74a8a2684701a0ce053042385264f379e10d997e18` |
| `review_aids/optionB_NOT_SELECTED_compaction_period7_03_tiling_3x3.png` | 1354703 | `462f710c651d3cef4e99499b683a3ea18f9cfccb0557c64d0984f9e8c0aa8ffb` |
| `review_aids/optionB_NOT_SELECTED_compaction_period7_05_angle_view.png` | 325367 | `1810b224c33807670ac9e8346a9c196763a1b8d5f7c645d7973c2347ea001a42` |
| `review_aids/optionB_NOT_SELECTED_metrics.json` | 6476 | `5b5edfd3e4026610bca087d6ce25a7d47395ed9815909467ae0af63dd91dff36` |
| `review_aids/v1_baseline_metrics_by_v2_code.json` | 367 | `069101c80202410ba4ad3ae4f649bbf495e411feac315f92345961625bdefb3d` |
| `source/crop_review_aid.py` | 588 | `549963954390c3167f415769ac3d7da1d2df1268a782ec829e33e4808f532501` |
| `source/make_sha256sums.py` | 821 | `199d5cad82ba044c028f924b08f59506357d6a26a0e900a3bed6b35b617b2792` |
| `source/patch_v1_to_v2.py` | 8157 | `2f8f0d8eff70f632a0300f21094e301ec69a4d9daade4d5d15928fc8b87522ec` |
| `source/sen1_wornearth_gen_v2.py` | 55756 | `eeb1b6e03698f70f186c779247e43091cbf7aee64875f6db23b660f2ce7009fb` |
| `source/v1_baseline_metrics.py` | 959 | `5cd8c370a7c374a7c0a457fc51d0afacded3726632df5a55ec9d99710500fd40` |
| `source/v1_to_v2_source.diff` | 10585 | `3f99cee4897c069a3847f83525ae846983442b6c07d68e99cc97fedbe60788d0` |

## 6. Limits

- **Scope:** material only. No path mesh, collider, H1 apron, Route A, guard, scene, other asset or code.
- **Not native:** these are Python software previews, not Blender or Unity/URP renders. There is no `.blend` or node graph.
- **Not checked:** Unity import, URP, VRAM, compression, LOD, performance and the Unity normal-map convention.
- **Untouched:** the protected shared `Forest Ground.mat` and `Forest Path.mat`, `Assets/`, specs, the board/`active-tasks.md`, saves, Prefs, and the v1 and #603 folders.
- **Open:** D-032 (licence, storage and source archiving) stays OPEN, and the graphics spec stays DRAFT. No binary is in Git.
- **Not done:** cross-machine reproducibility, a low-light or wet render, and owner native-view sign-off. Dashboard/Drive is **NOT VERIFIED**.

## 7. Next step

- **A nonauthor re-review of v2 is required.** It should be done by an agent that is neither this worker, the #597 author nor, preferably, the #603 reviewer session. It needs read access to the v2 scratch folder on the Windows host, or an authorised hash-verified transfer, following the #603 Linux correction on the board.
- The reviewer should judge in particular: the residual faint 1 m patch and column structure (V-1), whether the needles still read as twigs (V-3), and the angle-view cast (V-4).
- **No production, import or Unity GO** is given. v1 remains on hold per #603. Original source references: #577 W look, #593 owner W approval, #597 packet, #603 review.

## 8. Git readback

The expected diff from the starting head `6954b6cb` is exactly one added file: this Markdown. The commit SHA and the GitHub readback are reported in the #605 closing comment, because a file cannot contain its own commit SHA.
