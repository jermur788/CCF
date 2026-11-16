# Sen-1 #597 — Worn-Earth Footpath MATERIAL: scratch asset result

**Issue:** [#597](https://github.com/jermur788/CCF/issues/597) · **Parent:** [#424](https://github.com/jermur788/CCF/issues/424) · **Packet:** `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-597-WORN-EARTH-FOOTPATH-MATERIAL-SINGLE-ASSET-CREATION-PACKET-20261116.md` on branch `docs/sen-1-597-worn-earth-material-scratch-handoff-20261116-v1`
**Date:** 16 November 2026 · **Verdict requested of this author:** none. This is an author handoff for a **separate nonauthor source-art review**.

> **STATUS: SCRATCH ART PREPRODUCTION ONLY.** One original procedural material, built with a pure-Python generator **outside Git**. **Blender was NOT installed and NOT executed.** Nothing here was imported into Unity, rendered in Unity or Blender, or approved for production. D-032 is OPEN and the Compact graphics spec is DRAFT / NOT APPROVED. **No production or Unity GO is given or implied.**

## 1. Worker identity, registration, branch, starting head

| Item | Value |
|---|---|
| Worker (observed) | **Claude Sonnet 5.5** (`claude-sonnet-5-5`), effort **High**, in Claude Code via T3 Code; delegated child of an Opus 5.5 parent session. No subagents used. |
| Registration | Posted on #597 **before any scratch or tracked work**: <https://github.com/jermur788/CCF/issues/597#issuecomment-6917339426>. The issue title was set to `[RUNNING]` at the same time. One disclosure: the local branch was created from `origin/main` shortly before the comment; nothing had been written or committed. |
| Branch | `docs/sen-1-597-worn-earth-material-artist-result-20261116-v1` (the packet's exact name; the remote branch did not exist beforehand) |
| Worktree | `C:\Users\hp\.t3\worktrees\CCF\t3-1708803e-d614da16bf048895` (clean at start; switched from `t3/1708803e`, which was at `04defad6`, to the new branch at `origin/main`) |
| Starting HEAD | `afad222f607a2dbc05de04a030ba3d8a1cf6cb71` (`origin/main` after `git fetch`) |
| Scratch evidence folder (outside Git) | `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-20261116\` (created new and empty; used the packet's rule to pick a new unique path because the packet's Linux path does not exist on this Windows machine) |
| Write allowlist | Exactly one new tracked Markdown file (this one) + the scratch folder. |
| Packet reads | #597 issue body and comments, packet §§0–7 (full), `AGENTS.md` and `CLAUDE.md` (as supplied), `Docs/Project/AgentWorkflow.md` (searched for handoff/branch/worktree rules; **not read in full**), `main:Docs/Project/coordination/active-tasks.md` (read-only search for #597/scratch/worn: no #597 lock, no Unity lease, no conflicting scratch path found). **Not read:** the #593 result, the spec DRAFT, the #577 result, #573, `decision-log.md`, `current-milestone.md`; the packet's summary of the owner-approved W look (desaturated brown-grey earth, fine grit, needles) was used as the brief, and the packet's D-032/DRAFT statements were taken as stated. |
| Dashboard / Drive | **NOT VERIFIED.** |

## 2. Tools — and what was NOT used

- **Blender: NOT installed, NOT executed.** No `.blend`, no Blender node graph, no Blender render exists. The packet allows "Blender 4.x shader nodes **or offline procedural code**"; this delivery is the latter.
- **Actual tool:** a self-contained, seeded, pure-Python 3.15.2 generator (standard library only: `argparse, hashlib, json, math, operator, os, random, struct, sys, time, zlib`). No numpy, no PIL, no pip installs, no third-party textures or images.
- **Previews are offline software previews** produced by the same script (Lambert diffuse + hemispherical ambient + Blinn-Phong, neutral white light; a small perspective renderer for the angle view). They are **not Blender renders and not Unity/URP renders**.
- Maps are **generated directly**, not baked from a shader graph (there is no node graph).
- No Unity Editor, no Unity lease, no Windows build, no GitHub CLI (`gh` not installed; REST API used for issue steps).

## 3. What was created (scratch, outside Git)

One material, `Sen1_Compact_WornEarth_Footpath_Mat01`: a seamless 1024×1024 repeatable set — base colour, tangent-space normal (OpenGL/Y+), roughness, 16-bit height and a height-derived AO approximation — plus editable source, previews, metrics and provenance.

- **Look implemented:** desaturated brown-grey compacted mineral earth, mottled slightly towards olive-grey; fine grit (1–3 mm grains with occasional light and dark mineral flecks); pressed-in spruce needles (15–30 mm, some broken, olive/brown/pale, with a slight contact darkening); irregular damp patches that prefer low spots; matte surface with damp patches a little less rough.
- **Measured (maps):** mean base colour sRGB `[113.2, 104.3, 89.6]`, mean HSV saturation 0.2089, roughness mean 0.815, min 0.702, max 0.914 ("moderate" interpreted as a matte 0.70–0.91 range, not smooth), height-AO mean 0.941, min 0.588, needle coverage ≈ 5.2% of texels, damp coverage ≈ 30% (by design setting 0.30).
- **Neutral physical scale (design assumption only):** 1 tile = 1.0 m, ≈ 0.98 mm/texel. This is NOT an authorised path width or geometry setting; no 0.35 m / 0.20 m edge or 1.2 m width is baked into the texture.
- **Editable parameters:** seed, size, grain scale/density, needle density/length/width, colour range (five named colours), roughness range, normal strength, dampness, damp coverage, warp amount, compaction contrast — all named in `BASE_PARAMS` and `Sen1_Compact_WornEarth_Footpath_Mat01.params.json`.
- **Look development was iterative:** I viewed the previews and re-tuned parameters over about six full runs (first draft had hard-edged cow-hide damp patches, faint needles and salt-and-pepper height/AO noise, all fixed). Only the final run is kept as evidence; earlier runs were discarded and are not claimed.

## 4. Scratch files: purpose, bytes, SHA-256

All paths relative to `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-20261116\`. Hashes are from the local `SHA256SUMS`, which was re-verified with `sha256sum -c` (31/31 OK) after the final write. **None of these files are in Git.** Total folder size 28.3 MB.

| File | Purpose | Bytes | SHA-256 |
|---|---|---:|---|
| `PROVENANCE.md` | Author/originality declaration, seeds, parameters, assumptions, not-done list. | 4,813 | `eb0517ebb68a515420f168c769eb52ee4fe17d81fa19f27bfa9d382ce54572a7` |
| `REPLAY.md` | Rebuild and compare commands. | 1,108 | `7d6cdc10cdfd57d92adac6b94d4f58a7fae86def126967901f95c7ce4c3d38eb` |
| `SHA256SUMS_generator_run1.txt` | Generator-written manifest, run 1 (21 deterministic outputs). | 2,529 | `cd15874ff7f9489154af6417775aefba04051064f31ccdd317ae611520d19e90` |
| `SHA256SUMS_generator_run2_replay.txt` | Generator-written manifest, replay run 2 (identical to run 1). | 2,529 | `cd15874ff7f9489154af6417775aefba04051064f31ccdd317ae611520d19e90` |
| `Sen1_Compact_WornEarth_Footpath_Mat01.params.json` | Exact parameter set used (editable; feed back with --params). | 1,410 | `99e8bed888c50997b28b79e026c5a53a0106fe19ce782be2948f08cb456c5a72` |
| `build_log.txt` | Build log of run 1 (stage timings; not part of determinism comparison). | 714 | `8fb3b99aeef062602be5723dd77a757aed07e3313e2208edf57ac08b853e9cc5` |
| `build_timing_NOT_HASHED.json` | Run 1 timing (non-deterministic; excluded from determinism comparison). | 525 | `63ce48faed94d5d52d8ebb42b29eee49ca965d8d525419055e2afc3607f5a680` |
| `build_timing_replay_run2_NOT_HASHED.json` | Replay run 2 timing (non-deterministic). | 523 | `34e4b0177db69fd0d61eff6162dc529c6fe86a75d5892577b29f25e2666247a3` |
| `determinism_check.txt` | Record of the two-run identical-hash determinism check. | 1,163 | `ed328fd2c2f5925cf9864b67a7a36709a7689895391cf4ac2a1b419792bece2d` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_AO_heightderived.png` | Review-only height-derived cavity AO approximation (gray8). | 557,444 | `2ea9c2d505b050b612edd53d5d6e775cb7bee39b5b1977e73816bb6778e50e0a` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_BaseColor.png` | Review-only base colour (sRGB, 1024x1024, RGB8). | 1,520,473 | `e81ab23453f6737796e7a08930f8e3e47b4ecea07b524c6346ee7ec21eb22f19` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Height16.png` | Review-only height (1024x1024, gray16). | 2,085,759 | `d8dfdaa8ef7586fa084e1a31d8593c86b1fa42f4d1f841f3d6b3b7a1c672a00c` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Normal_GL.png` | Review-only tangent-space normal, OpenGL/Y+ (1024x1024, RGB8). | 2,888,377 | `3c2ac6559cb9bac1645a67eb5701b3bb68b42336cdcfdd4d049191f9f707fc00` |
| `maps/Sen1_Compact_WornEarth_Footpath_Mat01_Roughness.png` | Review-only roughness (1024x1024, gray8). | 694,518 | `a2c16e06d536ba0c2abbeb79903065760a2ae04dc835060a99773f500ee211fd` |
| `metrics.json` | All numeric results (seam metrics, autocorrelation, stats, grayscale comparison). | 5,949 | `edb86f3b789ca3089e4f3829c2f20dcd80e4c5d7ac17c21039f16e5628c7f767` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_BaseColor.png` | Illustrative dark-litter comparison surface base colour (NOT Forest Ground.mat). | 1,218,713 | `85f4dfdabd29ace6624f1ca246ef4ecfc36c84ba8ec7f21f193056bdb5de07d3` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_Normal_GL.png` | Illustrative dark-litter normal. | 2,689,750 | `9773658308f7f6214f69d4b9d0191aaccd48072def813cf684b8a527a386422c` |
| `reference_dark_litter_ILLUSTRATIVE/maps/ILLUSTRATIVE_DarkNeedleLitter_Roughness.png` | Illustrative dark-litter roughness. | 707,518 | `7ff60c0117550131ff9b860889709d3a3f22170f9784144fac30f83b4533a5fc` |
| `reference_dark_litter_ILLUSTRATIVE/renders_flat_neutral_colour.png` | Illustrative dark-litter flat neutral-light preview. | 1,738,944 | `b257aa63a252e7a0ba9b44274bdc477094dffabf7020f404ba544ce98e9abf2a` |
| `renders/01_flat_tile_neutral_colour.png` | Close flat colour tile, neutral light (software preview, 1024). | 1,952,528 | `035426e6fce78ef2f8ddb929c16ed9999717e3c309469876423a6c48137f4cfc` |
| `renders/02_flat_tile_neutral_grayscale.png` | Grayscale (luminance) of the same preview. | 783,470 | `cb90c7b7f03d233dcd088a66d562f60a3d9dfea4ce02b714d806199b061c1ccf` |
| `renders/03_tiling_3x3_neutral_clean_1536.png` | 3x3 repeat, clean (each tile 512 px). | 1,574,748 | `104544337c1e573ea742288fd9d897d061650e2b7b72c828be07b0a79aa36658` |
| `renders/03b_tiling_3x3_joins_marked_1536.png` | 3x3 repeat with magenta lines at the join positions. | 1,575,715 | `5a05b05cc54d2aff181da58ef0134ae7ebccec98bff4a173efd947872b4e34b9` |
| `renders/04_seam_stress_half_tile_roll_1024.png` | Tile rolled half a tile in X and Y so the join cross sits at the image centre (full-res seam stress). | 1,952,385 | `e70368023db9cc435404d1e72226b3b6a6dff7b3c5d702d34a0a9a7930f73a07` |
| `renders/05_angle_view_neutral_colour_960x540.png` | Perspective angle view of the infinitely repeated tile, neutral light (software renderer, mip-mapped). | 372,737 | `fea8dc4d89cf7c151b354244539952542b13e10555a140cf969f64d3876deaac` |
| `renders/05b_angle_view_neutral_grayscale_960x540.png` | Grayscale of the angle view. | 173,508 | `7c7c5f5a98825582dfa31d6487fec1906b5b4ef4b48562853678b8ce6d5d7fb0` |
| `renders/06_channel_sheet.png` | Channel sheet: base colour, normal, roughness, height, AO (labelled). | 2,543,694 | `6c51c31f087ec920e6f0a69409539c411880aaea7c0fa8e267167cb0e0f3eb8f` |
| `renders/07_grayscale_comparison_vs_illustrative_dark_litter.png` | Colour + grayscale comparison with the labelled ILLUSTRATIVE dark needle-litter stand-in. | 1,576,554 | `eaad1f204b994778661f9db4ae610254e607de5260bd029f7dfce484ab858679` |
| `renders/08_negative_control_broken_tiling_3x3_1536.png` | Negative control: deliberately non-tileable variant, 3x3 with joins (labelled; NOT the delivered material). | 1,583,722 | `bb2321718ba274b3fd8184aa318d1fc59d23266f742205c8331418fd249edc87` |
| `source/make_sha256sums.py` | Whole-folder SHA-256 manifest writer. | 821 | `199d5cad82ba044c028f924b08f59506357d6a26a0e900a3bed6b35b617b2792` |
| `source/sen1_wornearth_gen.py` | Original editable procedural generator (named parameters, seeded). The material source. | 52,175 | `ac5f45ec15e7bc3d350c3a8c85ee20b2b8891cfd8061f199d3ce61c49e2b55e2` |
| `SHA256SUMS` | Whole-folder manifest of the 31 files above (itself not listed in itself). | 3,441 | `b44d4f4ff52ee5de5cb268a6d6bf7cb25b238476a605bae4f31d4310059e7b1a` |

Size and cost disclosure: generator source 52,175 B; the five review maps total 7,746,571 B (all 1024², PNG); 5 textures, 0 shader nodes (there is no node graph; 8 fBm fields built from 30 value-noise layers plus grit and needle splatting); full build (worn earth + negative control + illustrative reference + all renders and metrics) ≈ 63 s, of which the worn-earth map build is ≈ 14 s. **Unity VRAM / URP cost: NOT MEASURED.** No budget is claimed.

## 5. Replay and determinism

```powershell
python -I "C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-20261116\source\sen1_wornearth_gen.py" --all --out "<new empty folder>"
fc "<new empty folder>\SHA256SUMS" "C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-20261116\SHA256SUMS_generator_run1.txt"
```

Seeds: worn earth `597001`, illustrative dark litter `597002`; each layer has its own `random.Random(seed*1000+k).random()` stream. **Determinism verified:** two independent full runs into different folders produced identical generator manifests (`diff` printed nothing, 21 of 21 files byte-identical; see `determinism_check.txt`). Conditions: Python 3.15.2, same Windows machine. Other Python/zlib builds and other machines were **not tested** (PNG bytes depend on the zlib build). The `--params` override path was smoke-tested at size 128.

## 6. Render and seam results (honest)

**Views I actually inspected (software previews):**

- **Flat colour tile (01):** reads as compacted, slightly damp brown-grey earth with fine granular grit and sparse thin needles lying in it; at 1:1 the grit looks slightly "embossed/leathery" and the needles read as thin dark sticks, with few clearly olive ones. It does not read as gravel road (no pebbles), paving (no regular edges or cracks) or an artificial strip.
- **Angle view (05):** reads as a plain trodden earth surface with irregular darker damp zones, needle litter visible in the foreground and fading to texture mush in the distance (mip-mapped). A little pale/sandy; no ruts, edges or footprints.
- **Grayscale (02, 05b):** value structure is carried by the damp patches and needles; fine grit adds low-contrast texture.
- **Channel sheet (06):** all five channels present; the height panel is dominated by grain-scale detail (an 8-bit view of a 16-bit map).

**3×3 repeat and seams (03, 03b, 04):**

- **Visually:** no visible seam or discontinuity at any join in the clean 3×3 (03), the join-marked version (03b; the magenta lines hide the join, so judge on 03) or the half-tile-rolled full-resolution seam stress (04).
- **Numerically** (seam-to-interior boundary difference ratio; 1.00 = the seam is indistinguishable from any interior pixel boundary; columns: pixel-adjacent X / Y, 8×8-block X / Y):

| Map | pixel X / pixel Y / block8 X / block8 Y |
|---|---|
| ao | 0.94 / 1.00 / 0.95 / 0.92 |
| basecolor_luma | 0.93 / 1.04 / 1.00 / 1.06 |
| height | 0.97 / 0.97 / 0.90 / 0.98 |
| normal_x | 1.09 / 1.00 / 1.06 / 0.99 |
| normal_y | 0.99 / 1.04 / 0.94 / 1.01 |
| roughness | 1.01 / 1.02 / 1.07 / 1.09 |
| lit grayscale preview | 1.02 / 1.07 / 1.09 / 1.03 |

  All delivered maps are within ≈ 0.90–1.09, i.e. no detectable seam. They are tileable by construction (integer-period lattices, toroidal splatting and warp), and the metrics agree.
- **Repetition:** maximum normalised autocorrelation of the albedo luminance at shifts ≥ ¼ tile is **0.132** (at 64-cell shift [36, 46]), i.e. no strong internal repeat. **However**, on the 3×3 the arrangement of damp patches is recognisable as a repeating motif on careful viewing — a one-metre tile cannot hide that by itself. The two dominant fixes (smaller more numerous damp patches and lower large-scale compaction contrast) reduced but did not remove it. I make **no claim that macro repetition is solved**; world-space macro variation / detail blending belongs to later integration work, not to this swatch.

**Negative controls (both required by the packet's wording, as I interpreted it — the packet does not define "negative control" further):**

1. **Seam-detector negative control (08 + metrics):** the same material rebuilt with deliberately **non-periodic** low-frequency layers. Same ratios, which **should be well above 1.00**:

| Broken variant | pixel X / pixel Y / block8 X / block8 Y |
|---|---|
| basecolor_luma | 2.25 / 1.82 / 2.32 / 2.00 |
| height | 1.71 / 1.57 / 1.79 / 1.58 |
| lit_gray | 1.29 / 1.20 / 2.06 / 1.85 |
| roughness | 1.43 / 1.55 / 1.88 / 2.35 |

  The detector flags it (1.2–2.4×), so the ≈ 1.0 results for the delivered maps are meaningful. **Limit disclosed:** in the 3×3 image (08) the deliberate seams are only faintly visible to the eye. Visual inspection alone is therefore a weak seam test for low-contrast joins; the metrics are the stronger evidence.
2. **Grayscale negative control (07):** under the same neutral light and texel scale, W is compared against a clearly labelled **ILLUSTRATIVE dark Sitka needle-litter stand-in** (a different procedural material from this same generator — **not** `Forest Ground.mat`, not a Unity frame, not a real photo). Measured: W lit mean sRGB gray **0.3526** vs dark litter **0.1626**; mean linear luminance **0.10575** vs **0.02393** (ratio **4.419**). W is clearly lighter and its damp/needle structure remains visible in grayscale; the dark stand-in is near-black with fine pale needle lines. **This ratio is a single-scene software number and is NOT a mandatory ratio; it is not comparable to #577's Blender-derived 1.13** (different surface, light and renderer).

## 7. Limits

- **Material only.** No path mesh, collider, H1/Route A geometry, shelters, guards, vegetation or scene; no 0.35 m / 0.20 m / 1.2 m edge or width authored.
- **Not native Blender; not Unity.** No node graph, no `.blend`, no URP/Unity import or render, no lightmap/GI, no real mip/compression/VRAM test, no Unity normal-map convention check (OpenGL/Y+ is stated, not verified in the engine).
- **Source-first scratch review candidate only.** AO is a height-derived approximation; height/needle/damp values are illustrative, not researched ecology; no low-light or wet render (omitted: not cheap and not clearly useful).
- **Protected shared materials unchanged:** `Forest Ground.mat`, `Forest Path.mat`, their shared soil texture, old planks and headland/approach templates were not opened, read, copied or edited. `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `specs/`, saves and Prefs were not touched. No binary, PNG, `.mat` or Unity asset is in Git.
- **Decision state:** D-032 (binary storage / source archiving / licence) is **OPEN**; the Compact graphics spec is **DRAFT / NOT APPROVED**. I make no licence claim: originality and authorship are declared in `PROVENANCE.md`, but redistribution rights and the future source-archiving policy are **unresolved**. The scratch folder is local to one machine; nothing in it is durable until a storage decision is made.
- **Possible review defects to expect:** macro repetition of the damp-patch motif on the 3×3; grit slightly embossed at 1:1; needles read as dark thin lines; pale/sandy cast in the angle view under a neutral light; roughness range is matte rather than mid-range.

## 8. Recommendation

A **separate nonauthor graphics/source-art reviewer** should assess this against packet §5 (art, tiling, single-asset scope, provenance, technical honesty, issue/Git) using the scratch folder (and the numbers above) and issue **APPROVED AS SCRATCH MATERIAL CANDIDATE / NEEDS CHANGES / REJECTED**. The reviewer needs local access to the scratch folder (or the author must copy selected previews somewhere reviewable); the PNGs are deliberately not in Git. Original design-approach references: #577 result (`Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-ASSET-PREPRODUCTION-RESULT-20261116.md` on branch `docs/sen-1-compact-asset-lookdev-opus55-20261116-v1`, §§3–5, W worn-earth look) — used only as the packet's summary of the W direction; none of its images or source were read or copied.

**NO production GO. NO Unity import GO.** A later adoption would need an accepted D-032, a licensed exact source, an approved Compact graphics spec, the parent scene bridge, and native Unity visual/LOD/performance verification, as the packet states.

## 9. Git diff readback

READBACK-PENDING

Issue #597: set to **[HANDED-OFF]** and closed with a comment linking this file (see the issue). Notification of #424 is included in that comment. Dashboard/Drive: **NOT VERIFIED.**
