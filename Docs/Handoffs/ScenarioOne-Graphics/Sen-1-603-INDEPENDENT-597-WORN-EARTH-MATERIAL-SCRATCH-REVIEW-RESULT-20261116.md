# Sen-1 #603 — Independent review of #597 `Sen1_Compact_WornEarth_Footpath_Mat01` (scratch material)

**Issue:** [#603](https://github.com/jermur788/CCF/issues/603) · **Parent:** [#424](https://github.com/jermur788/CCF/issues/424) · **Reviewed:** [#597](https://github.com/jermur788/CCF/issues/597) artist result at `docs/sen-1-597-worn-earth-material-artist-result-20261116-v1@e9ce51cd` · **Packet:** `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-603-INDEPENDENT-597-WORN-EARTH-SCRATCH-MATERIAL-ART-REVIEW-PACKET-20261116.md` on `docs/sen-1-603-wornearth-independent-art-review-packet-20261116-v1` · **Date:** 16 November 2026

> ## VERDICT: **NEEDS CHANGES** — severity **MODERATE**, salvageable with **one bounded tuning pass**
> The source, provenance, determinism and seams are good: **PASS**. The art does not yet meet the #597 §4/§5 rubric:
> - **Macro repetition FAIL:** "acceptable repetition" and "minimise obvious periodic details and macro texture repetition" are not met. The damp-patch motif is obvious every 1 m in the 3×3 and the angle view.
> - **Grit relief FAIL:** the creation packet asks for "subtle normal variation". The measured median normal tilt is **47°**, and **54 %** of texels are tilted more than 45°.
> - **Needle form and colour:** minor-to-moderate.
>
> A parameter-only trial run with the author's **unmodified** generator fixed the relief and most of the damp repetition (§5). That trial is evidence of direction, not a delivered material. Two small source-constant edits are still recommended for the needles.
> **Nothing here permits Unity import, production use, binary commits or licence/distribution. D-032 stays OPEN and the Compact graphics spec stays DRAFT.**

## 1. Reviewer, registration, branch, platform

| Item | Value |
|---|---|
| Reviewer | **Claude Opus 5.5** (`claude-opus-5-5`), effort High, local Claude Code via T3 Code. It ran in a new session, separate from the #597 Sonnet 5.5 artist and its Opus parent session. No subagents were used. |
| Registration | Posted **before** any tracked edit: <https://github.com/jermur788/CCF/issues/603#issuecomment-6918476115> |
| Branch / worktree | `review/sen-1-603-597-wornearth-scratch-art-20261116-v1`. It was vacant on the remote and was created from the exact #597 tip `e9ce51cd8d72537c27547f44973bde1e34b3a73b`. The worktree is its own clean one at `C:/Users/hp/AppData/Local/Temp/ccf-603-review`; the #597 artist worktree was not used. |
| Starting HEAD | `e9ce51cd8d72537c27547f44973bde1e34b3a73b`. `origin/main` at start was `6954b6cb9798cbf0dfd32b2eb9106a3dac93fe49`. |
| **Platform: deviation from the packet** | The packet and the #603 issue say to review on the **Linux laptop**. The owner instead dispatched this review **directly in this session on the Windows host** and named the Windows path. I therefore inspected the artist's **original** folder `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-20261116\` **in place, READ ONLY**. No Linux copy, mount or transfer exists or was made, and I assert no Linux path. Manager: please record this deviation. |
| Tools actually used | Git, Python 3.15.2 (stdlib only, run with `-I`), `sha256sum`, an image viewer (the Read tool) and the GitHub REST API through stored Git credentials (`gh` is not installed). **Not used:** Unity, Blender, Windows build, `Assets/`. |
| Sole tracked write | This file. |

## 2. Evidence actually inspected

**Hash manifest.** I ran `sha256sum -c SHA256SUMS` in the author folder **before and after** the review, and both runs gave **31/31 OK**. The folder holds 32 files (31 listed plus `SHA256SUMS` itself), about 28 MB. `sha256(SHA256SUMS)` = `b44d4f4ff52ee5de5cb268a6d6bf7cb25b238476a605bae4f31d4310059e7b1a`, which matches the #597 result §4. The source hash `ac5f45ec…e55e2` and every map and render hash match the #597 table.

**Files opened and viewed (all at native resolution; crops were made at native pixels and upscaled with nearest-neighbour):**
- Renders: `01_flat_tile_neutral_colour`, `02_flat_tile_neutral_grayscale`, `03_tiling_3x3_neutral_clean_1536`, `03b_tiling_3x3_joins_marked_1536`, `04_seam_stress_half_tile_roll_1024`, `05_angle_view_neutral_colour_960x540`, `05b_angle_view_neutral_grayscale_960x540`, `06_channel_sheet`, `07_grayscale_comparison_vs_illustrative_dark_litter`, `08_negative_control_broken_tiling_3x3_1536`.
- Maps (decoded and measured by my own stdlib reader, see §4): `BaseColor`, `Normal_GL`, `Roughness`, `Height16`, `AO_heightderived`. I also viewed native crops of `BaseColor` and `Normal_GL`.
- Text: `source/sen1_wornearth_gen.py` (read in full, 1,175 lines), `source/make_sha256sums.py`, `Sen1_Compact_WornEarth_Footpath_Mat01.params.json`, `PROVENANCE.md`, `REPLAY.md`, `SHA256SUMS`, `SHA256SUMS_generator_run1.txt`, `SHA256SUMS_generator_run2_replay.txt`, `determinism_check.txt`, `metrics.json`, `build_log.txt`.
- Background (read-only, for the context gap the packet named): the #597 creation packet §4–§5, the #597 artist result (full), the #593 GQ-01 owner W selection row, and the #577 W/N direction table and C2 findings.

**Process timing check.** The #597 registration comment is dated 2026-11-16T13:10:13Z. The generator file's creation time is 13:15:49Z and the artist commits are 13:31–13:32Z, so registration came before scratch work, which is consistent. The #597 branch differs from `main` by exactly one added Markdown file (154 lines).

## 3. Source, provenance and determinism — **PASS**

- **Dependencies and I/O:** only `argparse, hashlib, json, math, operator, os, random, struct, sys, time, zlib`.
  - There is no network or subprocess code, and no external image or texture is read.
  - The only file reads are the optional `--params` JSON and an unused minimal PNG reader for the generator's own outputs.
  - Writes are confined to `--out`.
  - Fonts are an inline 5×7 bitmap table. I found no copied asset or table.
- **Seeds:** master `597001`; dark-litter reference `597002`. Each layer has its own `random.Random(seed*1000+k)` stream, and only `.random()` is used, so the int-seeded Mersenne Twister sequence is stable. All named parameters are in `BASE_PARAMS` and the params JSON, and `--params` overrides work (I used that path for §5).
- **Reviewer replay:** I ran `python -I …/source/sen1_wornearth_gen.py --all --out <new temp folder>` into a separate scratchpad directory, never into the author folder. `diff` of its `SHA256SUMS` against `SHA256SUMS_generator_run1.txt` was **empty: 21/21 byte-identical**. This was the **same machine**, Python 3.15.2, zlib `1.3.1.zlib-ng`. **Cross-platform or other-zlib PNG byte identity is NOT CHECKED.**
- **Originality:** the declaration in `PROVENANCE.md` agrees with the code. The authorship declaration is **not** a licence. **Licence/redistribution: NOT DETERMINED (D-032 OPEN)**, and I do not invent one.

## 4. Technical channels — **PASS** (labels correct; Unity behaviour not checked)

- **Normal is OpenGL / Y+.** I measured this independently. Correlation of the green channel with `h[row+1]−h[row−1]` from `Height16` is **+0.971**, and of red with `−(h[col+1]−h[col−1])` is **+0.971**. So green means image-up, as labelled. Unity import flags and tangent convention under URP are **NOT CHECKED**. A later adoption needs a real import test, and Unity's default normal-map handling must not be assumed.
- **Height16** is a true 16-bit grey PNG; the 8-bit channel-sheet panel is only a view. **AO** is a height-derived cavity approximation and is labelled as such. **Roughness** ranges 0.702–0.914 (mean 0.815), which is matte. That is acceptable for scratch, and it stays consistent through the tuning pass.

## 5. Visual and quality findings (the four disclosed weaknesses, quantified)

All views are **offline Python software previews** (Lambert + hemispherical ambient + Blinn-Phong). They are **not** Blender, Unity or URP renders, and the light and ambient levels are uncalibrated.

| # | Issue | Where seen | Measured / observed | Severity |
|---|---|---|---|---|
| **V-1** | **Damp-patch macro repetition** | `03`/`03b` 3×3 and `05`/`05b` angle view | High-contrast blotches about 10–25 cm across, recognisable shapes such as the crescent at the top of each tile, a visible **1 m lattice**. In the angle view they form repeating rows toward the horizon, and in grayscale they are the dominant value structure. About 13 % of albedo-luminance variance survives at 12.5 cm block scale. The author's metric (autocorr 0.132) only measures *within-tile* self-similarity and **cannot detect tile-to-tile repetition**. | **MODERATE — blocking** (#597 §5 "acceptable repetition") |
| **V-2** | **Raised, embossed grit** | `01`/`02` at 1:1, native crops; `06` normal panel | Reads as hammered or embossed leather rather than compacted earth. Normal tilt from `Normal_GL` at full resolution: **median 47.4°, p90 64.8°, 68.7 % >30°, 53.5 % >45°**. Cause in source: dense 1–3 px dome grains carry weight 0.60 in a contrast-stretched height map, multiplied by `normal_strength 1.25`. | **MODERATE — blocking** (#597 §4 "subtle normal variation") |
| **V-3** | **Needles look like sticks** | Native `BaseColor` and `01` crops | Constant-width capsules about 1.9 px wide with rounded ends and flat colour. The `0.10·halo` albedo darkening and `−0.16·halo` height groove give them **dark outlines**, so they read as matchsticks or twigs lying on top rather than pressed-in Sitka litter. The pale entry `[146,132,106]` adds straw-like sticks. | MINOR–MODERATE |
| **V-4** | **Pale, sandy, dusty colour** | `05` angle view, `07` | Mean albedo sRGB `[113, 104, 90]` (linear luminance 0.144). It reads as a dry dusty lot more than damp Irish woodland earth. Lit mean sRGB grey is W 0.353 against the illustrative dark litter at 0.163 (2.2× in sRGB grey). That is not a threshold, and the dark stand-in is not `Forest Ground.mat`, but it signals a risk of the "bright slab" look #577 already warned about for H1. Partly a renderer-calibration effect. | MINOR (nonblocking alone) |

**Not defects:** the surface reads as a walked earth path, not road, concrete, gravel, rack or timber. There are no ruts, footprints, painted edges or path width baked in. The damp-in-hollows rule is labelled a simplification, and needle dimensions and density are labelled illustrative, not researched. There are no false site-history or forestry claims.

## 6. Tiling, seams and controls

- **Continuity (seams): PASS.**
  - No join is visible in `03` or in the full-resolution half-roll `04` (the join cross sits at the image centre).
  - I recomputed the base-colour luminance boundary ratio from `BaseColor.png` with my own code and got **0.927 / 1.041**, matching `metrics.json`.
  - All of the author's delivered-map ratios fall in 0.90–1.09.
  - Tileability holds by construction: integer-period lattices, toroidal warp, toroidal splats and periodic blur.
- **Periodicity: FAIL** (V-1). This is a separate finding: continuity passes while repetition fails.
- **Negative control:** the numeric control works (1.2–2.4×). The **visual** control `08` is weak, because the magenta join lines are drawn exactly over the joins and hide them; the author disclosed this. The numeric evidence is accepted, and the visual control is NOT PASSED as an independent test.
- **Grayscale comparison (`07`):** correctly labelled illustrative and NOT `Forest Ground.mat`. It is used here only as an indicator, not as a contrast requirement, and it is not comparable with #577's 1.13.

## 7. Parameter-only verification trial (reviewer scratch, NOT a delivered material)

I ran the author's **unmodified** generator with `--params` overrides into a separate scratchpad folder. Overrides: `dampness 0.24, damp_edge_softness 0.60, damp_period 13, damp_coverage 0.24, normal_strength 0.45, needle_width_mm 1.3, needle_alpha 0.80, col_compact_dry [118,110,98], col_loose_dark [94,86,75]`.

| Metric | #597 delivered | Trial |
|---|---|---|
| Normal tilt median / p90 / >45° | 47.4° / 64.8° / 53.5 % | **21.5° / 37.6° / 0.7 %** |
| Albedo variance share at 12.5 cm blocks | 0.130 | 0.111 |
| Mean albedo sRGB | 113 / 104 / 90 | 105 / 98 / 85 |
| Lit-gray seam ratios (px x/y, block8 x/y) | 1.02 / 1.07 / 1.09 / 1.03 | 1.03 / 1.04 / 1.11 / 1.03 (still seamless) |

Visually, the damp motif is now hard to pick out in the 3×3, and the grit reads as fine granular soil rather than embossing. **A residual faint 1 m light/dark banding remains**, now carried by the period-5 compaction field and visible as soft columns in the trial 3×3 and angle view. The needles **still** read as sticks, because the halo is a source constant.

Reviewer evidence is kept **local only, outside Git** in `C:\Users\hp\Documents\CCF-Scratch\Sen-1-603-WornEarth-Review-20261116\` (14 files, about 4.2 MB, own `SHA256SUMS` = `47fbae9e72e2475b06b84e2f83b540d2efe2bab12a44a0c57f94c2113a3e9ad0`). It contains the overrides JSON, trial `metrics.json`, trial renders 01/03/05, native crops, my replay manifest and my read-only stdlib analysis/crop scripts. The author folder was not written to.

## 8. Exact bounded tuning packet (one pass, original author or any artist)

**Scope:** the same material name, seed `597001` and 1024² size. Write a **new** scratch folder (for example `…\Sen-1-597-WornEarth-v2-<date>\`) and **never overwrite v1**.

1. **Parameters (tested together in §7):**
   - `dampness 0.42→0.24`, `damp_period 9→13`, `damp_edge_softness 0.40→0.60`, `damp_coverage 0.30→0.24`
   - `normal_strength 1.25→0.45`
   - `needle_width_mm 1.9→1.3`, `needle_alpha 0.90→0.80`
   - `col_compact_dry [132,122,108]→[118,110,98]`, `col_loose_dark [108,98,85]→[94,86,75]`
2. **Parameter (untested, for the residual 1 m banding):** `compaction_contrast 0.65→~0.45`, or `compaction_period 5→7`. Choose by viewing the 3×3 and angle view.
3. **Source constants (small edits in `build_material` / `splat_needles`):**
   - (a) needle halo albedo darkening `0.10*hl → ~0.03` and height `−0.16*nh → ~−0.05`;
   - (b) taper needle core width toward the tips (for example scale `half_w` by `1−0.5·|2t−1|` along each segment) so the needles are not uniform rounded capsules;
   - (c) optional: grain height weight `0.60 → ~0.40` so clods and compaction carry more of the relief.
   - Replace or reduce the pale straw needle colour `[146,132,106]` unless an owner or research reference supports it. **Do not claim Sitka litter colours as fact** without a source.
4. **Re-deliver the same evidence set:** 01–08, maps, metrics, replay, determinism check and whole-folder `SHA256SUMS`. **Add** a normal-tilt statistic: median ≤ ~25° is the reviewer's calibration target for "subtle", **not** a physical specification. Also redraw the `08` visual control with offset or tick-only join markers so the joins stay visible.
5. **Stop conditions:** one pass only. If the damp/compaction repetition is still obvious in the 3×3 and angle view, or the needles still read as sticks, stop and report. Do not escalate into a new material or a Blender build.
6. **Known limit, not to be solved in this tile:** any 1 m tile repeats. Hiding repetition in the final path needs world-space macro variation or blending in the future Unity/URP integration. That is out of scope and still requires D-032, an approved spec and native verification.

## 9. Rubric summary (#597 §5 and #603 §B)

| Criterion | Result |
|---|---|
| Real files present and inspected | **PASS** (original Windows folder, read-only) |
| Hash manifest 31/31 | **PASS** (before and after review) |
| Reviewer replay determinism (same machine) | **PASS** 21/21; cross-platform **NOT CHECKED** |
| Source originality / no external reads, network or dependency surprises | **PASS** |
| Licence / redistribution | **NOT DETERMINED** (D-032 OPEN) |
| Tangent space / channel labels | **PASS** (OpenGL Y+ measured); Unity convention **NOT CHECKED** |
| Seams / continuity | **PASS** |
| Macro repetition / periodicity | **FAIL** (V-1) |
| Art: walked-earth read, not road | **PASS** |
| Art: subtle relief | **FAIL** (V-2) |
| Art: pressed-in needle read | **NEEDS CHANGES** (V-3) |
| Art: colour vs owner W "low-saturation brown-grey earth" | **MINOR** (V-4, renderer-calibration caveat) |
| Single-asset scope / no Unity, Assets or binaries | **PASS** |
| #597 process: registration before work, one Markdown, readback | **PASS** (timestamps checked) |
| Unity import, URP, VRAM, performance, owner native view | **NOT CHECKED / out of scope** |

## 10. Next actor and limits

- **Next:** Scenario One Manager #424 may dispatch **one** bounded tuning pass per §8, with the original author preferred so scratch custody stays clear. After that, a **nonauthor re-review** of the v2 folder.
- **Hold:** v1 is **not** an approved scratch candidate. Do not import it, archive it as accepted, or reference it as the chosen W material.
- **Not claimed:** Unity, URP or Blender rendering; performance or VRAM; owner final visual sign-off; licence; cross-platform reproducibility; a Linux-side review; Dashboard/Drive state (**NOT VERIFIED**).
- **Untouched:** the author scratch folder, the #597 result, the DRAFT spec, the live board, `Assets/`, scripts, scenes, ProjectSettings, saves and the old ground/path materials. No binary is in Git.

## 11. Git diff readback

The readback against the starting head `e9ce51cd8d72537c27547f44973bde1e34b3a73b` (expected: exactly one added Markdown, this file) and the GitHub contents-API hash comparison are reported in the closing #603 comment. A file cannot contain its own commit SHA.
