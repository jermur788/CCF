# Sen-1 #786 — H1 entrance, C0 winding footpath and 30 m ground kit

**Verdict: H1 KIT BUILT — isolated scratch candidates, NOT production-integrated assets.** Actual source, editable Blender master, textures, two readable exports and 16 Cycles CPU captures exist. This is the implementation author's build result; independent #788 G2 scratch-quality acceptance and owner look acceptance remain outstanding. Production import/public binary publication remain held.

Actor: **gpt-6.1-sol / codex / T3 Code / Codex**, implementation child of #785, host `jer-laptop`; session/thread id unavailable. No delegates. [Actual RUNNING registration](https://github.com/jermur788/CCF/issues/786#issuecomment-6992767008). Authorised source is the user's actor-specific task plus [#785 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Assets-20261118/Sen-1-785-TG-FORK-COMPACT30-TWO-ASSET-KITS-ORIGINAL-CREATION-PACKET-20261118.md), with [#688 §4–5 normative blueprint](https://github.com/jermur788/CCF/blob/2a625018d04d13339cafe480bdccf1a670e5f9d2/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-688-NATURAL-ENTRANCE-FOOTPATH-ASSET-BUILD-BLUEPRINT-20261117.md).

Checkout `/media/jer/Files/CCF/Operations/CCF-786-EntrancePath-20261118`; branch `art/sen-1-786-h1-path-offline-kit-20261118-v1`; initial/base HEAD `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. Context-set commit `03fade5e4cac5ff6bf0e6c88c8449366b372181e`. Final docs commit/pushed ref are reported on #786 to avoid self-reference. Exactly this Markdown file is the tracked repository change. No Assets, Unity, packages, project settings, CCF-Ops, specs, board, save, world anchor or binary Git edits.

## G0 custody and storage witness

`/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path` was absent before creation. `/media/jer/Files` actual source `/dev/sdb4`, ext4, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, options `rw,relatime`. Parent directory `jer:jer`, mode775; uid/gid1000. Create/read/remove test PASS. At G0, `df -B1` reported 24,402,096,128 available bytes. Full command outputs and UTC timestamp are in `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/CLAIM.md`. An initial literal-whitespace assertion was corrected to parsed mount fields before any scratch write; actual volume conditions matched throughout. Stale ZX20 Ops state untouched and unused. No claim of an Ops/Unity lease.

The read-only shared anchor's final porcelain output is byte-identical to G0, **exactly seven entries** (recorded in `verify/anchor-final-status.txt`). No stash/reset/clean/checkout/switch/edit there. Owned temporary directories and small prior rebuilds retained; no cleanup threshold was exceeded. All own Blender sessions exited. Final measured `du -sb` footprint: **39,668,768 bytes**, below **1,288,490,188 bytes (1.2 GiB)**. All durable evidence is on Files; no assets/renders/caches are directed to `/tmp` or `/home`.

## Continuation after transport failure — inherited versus new

[Resumption RUNNING receipt](https://github.com/jermur788/CCF/issues/786#issuecomment-6993656845). Before continuation, the lane was clean at the exact base, with **no prior commit**. The full inherited inventory is `verify/inherited-inventory.json`: **65 files / 33,833,767 file bytes**, each with absolute path, size and SHA-256. `verify/INHERITED.md` states prior proof and its limits. All source art, textures, GLB/FBX and **all 16 required renders were already present**; none was missing. Full-image verification identified three defective grayscale backgrounds, repaired below. Delivered mesh, textures, exports, recipe and all thirteen colour/plan renders remain byte-identical to the inherited inventory. The three defective grayscale outputs were repaired as described below. No temporary or duplicate rebuild directory approached 200 MB; nothing was deleted.

Newly executed now: `bash source/rebuild.sh build` into fresh, previously absent `rebuilt/`; `import-glb`, `import-fbx`, `verify`, and fresh `source/resume_checks.py` in separate factory Blender processes. Fresh rebuild passed all 12 numerical tests; current delivered and fresh rebuilt PNGs, recipe and GLB are byte-identical, and master geometry/UV/normals are numerically identical. Logs: `verify/resume-build.log`, `resume-reimport-glb.log`, `resume-reimport-fbx.log`, `resume-file-readback.log`, `resume-checks.log`. The actual .blend readback corrects the stale lazy-normal diagnostic only, retaining `build-result-initial-diagnostic.json`. Fresh render checks decoded all 16 PNGs; three grayscale files have exactly equal RGB channels. Saved Cycles CPU/64-sample/denoising-off/orthographic/emission configuration and the inherited 16 completed render-log outputs were read back. Three repaired grayscale renders were newly produced with Cycles CPU; no render byte-determinism claim. Author inspected spawn, A7 northbound and plan images; no owner acceptance is inferred.

The first continuation image check **failed** because all three inherited grayscale images retained coloured world backgrounds (A7 max RGB delta .09803920984268188). This diagnostic is retained in `verify/resume-checks-initial-diagnostic.*`, with original PNGs in `verify/inherited-gray/` and inherited generator in `verify/ground_h1_path-inherited.py`. The render-only fix uses linear Rec709 luminance for the visible background, preserving ground emission/Raw output. Only spawn/A7/plan grayscale were rendered again; `verify/resume-gray-render.log` records three new real Cycles CPU saves. The subsequent full-image RGB-channel equality check passed. This is a defect fix, with no acceptance threshold changed.

New evidence includes checks.txt, RESULT.json, FILE_BYTES.tsv, SHA256SUMS and this tracked handoff. Generator changes are limited to neutralising the grayscale world background and adding --gray-only for selective real rendering; recipe, mesh, texture and export construction are unchanged; the pre-existing finalizer was corrected to seal continuation evidence and retain small temporary files. CLAIM.md now links the resumption and the render correction. SOURCE_RIGHTS.md was inspected and remains complete within the scratch-only provenance boundary.

Fresh G0 `verify/resume-G0.txt`: `/` = `/dev/sda2` ext4; Files = distinct `/dev/sdb4` ext4; ZX20 = `/dev/sdd2` exFAT. Actual available bytes at 22:35:42 UTC: root **10,695,700,480**, Files **22,272,999,424** (above 10 GiB floor). Earlier <2 GB root observation had changed. Scratch all on Files; no large outputs on root. Controllable TEMP/TMP/TMPDIR, Blender temp/config, cache/state and Python bytecode are directed/disabled under Files for continuation verification. T3/Codex internal log relocation is not exposed; **zero root writes is not claimed**. The G0 command record includes a harmless `git remote` run from non-Git scratch (exit128); actual lane remote is checked separately. No Ops/board or Unity lease change.

## Built assets and measured checks

One mesh, four vertices, two triangles; world x/z bounds exactly ±15m, source y exactly0. One UV layer with (0,0),(0,1),(1,1),(1,0); one opaque material, no modifiers/rigid body/constraint/collision asset, no second surface. Cameras and one preview sun are non-mesh scene objects. Source master has upward normals. H1 and path are logical material regions only; off-path appearance imposes no movement restriction. No scene/tree/G1 state is imported or placed.

C0 preserves all nine anchors and arithmetic-mean tangents. Actual measurements:

| Check | Measured result |
| --- | ---: |
| Max exact same-Z departure | 0.094074074m (cap .095m) |
| Minimum sampled radius, 10,001 samples/segment | 0.625361736m |
| Analytic radius lower bound | 0.625284220m |
| Length, 2,048 chords/segment | 19.509002164m |
| Endpoints | (2.99,−4.75), (−2.10,+13.50) |
| Macro core texels with P=1 | 114,372 |
| Macro outside-support texels with P=0 | 4,039,439 |
| Max positive sampled d + rho | .7999901849942255m |
| Min unsaturated sampled d − rho | .6000091144460222m |
| Exact distance vs 2048-chord cross-check, 440 points | max 7.247248556829788e−8m, bound 7.579789886474604e−8m |
| Detail linear extrema | .46378042267490655, .526157015335317 |

**Known C0 risk:** immediately south of A7, the ~.625m radius is smaller than the .80m footprint. The continuous distance union remains defined and seam-free, but this does not establish naturalness, collision clearance or owner approval. No T1, anchor/site/tree movement or masking workaround was used.

The atlas is 2048² RGBA8; detail 512² RGBA8. Full actual decoded samples (including exact texel centres, RGB and alpha near d0/.59/.60/.61/.70/.79/.81/.90, H1 centre/outside and world corner) are in `verify/checks.txt` and `verify/file-readback.json`. Example actual RGBA8: A0 core `[133,116,92,76]`; requested .70m south-cap sample (actual texel d=.7065652043300539m) `[121,106,84,177]`; .81m request (actual texel d=.8091014488749437m) `[110,96,77,255]`; H1 centre `[123,108,87,170]`; H1 outside/world corner `[112,98,78,255]`. Alpha is quantised detail weight, never transparency. Samples distinguish requested distance from actual nearest texel distance.

Macro support uses rho=√2×30/2048, exact continuous-cubic distances from all real stationary quintic roots plus endpoints, and inward support. 440 independent chord comparisons bound observed numerical disagreement; this is floating-point numerical verification, not a formal symbolic proof. Core saturation/outside zero and H1 inward support pass across all atlas texel centres. The bilinear 1-Lipschitz support argument assumes clamp, no mips, no compression; those Unity import settings remain **unverified assumptions**.

## Export axes and material limitations

Blender is right-handed Z-up: `(Bx,By,Bz)=(CCF x,−CCF z,CCF y)`. GLB Y-up export emits numeric `(x,y,z)` with x/z bounds±15 and exact y0; raw GLB normal is +Y. Raw glTF V is 1−source V (top-left image convention); fresh Blender import restores source UVs. FBX uses forward−Z/upY, scale1, metres. Fresh-process re-import of each proves 1 mesh/4 vertices/2 triangles/1 UV layer/1 material and upward normals. GLB bounds match exactly; FBX maximum ground-height roundoff **6.556708171956416e−7m**, within the explicitly reported 2e−6m readback tolerance. No normal/UV loss observed. Unity importer axis behavior was not tested.

Saved master uses opaque Principled with metallic0, roughness.92, alpha1 unlinked, emission0, no normal/height/parallax. Atlas alpha modulates restrained detail only. Intended URP Lit: white BaseMap atlas, same alpha DetailMask, detail tile scale60/detail scale.15, smoothness.08; actual installed URP behavior has not been inspected/verified. Portable GLB/FBX exports bind the macro atlas directly; layered detail remains in the editable master plus separate texture and is **not claimed portable shader parity**. Nonfatal Draco-library probe in exporter log does not affect the uncompressed GLB; fresh re-import passes.

## Actual renders and camera context

16 real Cycles CPU PNG captures: six colour viewpoints at1920×1080 and1280×720 (spawn/A0 mouth, A0–A1 bend, A7 northbound, A7 southbound, A8 looking back, apron edge toward P0109 reference); one1920×1080 macro-emission orthographic plan; three1920×1080 linear Rec709 grayscale emission diagnostics (spawn, A7 northbound, plan). Every render uses64 samples, denoising OFF, adaptive sampling OFF, seed68804, four CPU threads. Colour is Standard/None/exposure0/gamma1; grayscale uses Raw output and linear weights .2126/.7152/.0722. Plan is unlit macro RGB without microdetail, width56m/height31.5m, image +X right and +CCF Z downward due the documented Blender axis mapping. These are a neutral ground-only approximation, with no forest/HUD/controller. Linear grayscale includes the visible world background after the continuation correction. Eye y1.75m=.10+1.65; spawn yaw16.941542°, pitch target1.30m at5m horizontal. Full camera maths/poses, light settings, actual Python/NumPy/Blender versions and bounces are in `source/recipe.json`. P0109 is only a fixed coordinate reference; no tree was added or moved.

## Determinism and diagnostic corrections

Two independent clean factory builds in `verify/rebuild-A` and `verify/rebuild-B` compare **identical compressed PNG bytes, recipe JSON, numeric geometry/UV/normals/curve/mask check data and GLB bytes**. FBX differs by65 bytes, .blend by31,225 bytes; exact inclusive offset ranges and both hashes are in `verify/determinism-diff.json`. No byte determinism is claimed for those two containers or un-repeated renders. Latest generator texture bake is separately compared with root/A/B. Initial normal diagnostics read lazy uncomputed Blender normals (zero/empty); adding `calc_normals_split` corrected the measurement, with original diagnostics retained and fresh master/export normals passing. No acceptance threshold was relaxed.

## Exact rebuild, re-render and verification commands

```bash
KIT=/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path
export TMPDIR="$KIT/tmp"
export BLENDER_USER_CONFIG="$KIT/tmp/config"
export XDG_CACHE_HOME="$KIT/tmp/cache"
export PYTHONDONTWRITEBYTECODE=1
export PYTHONPATH=/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/_Toolenv/python312-numpy
export TEMP="$TMPDIR" TMP="$TMPDIR" XDG_STATE_HOME="$KIT/tmp/state"
mkdir -p "$TMPDIR" "$BLENDER_USER_CONFIG" "$XDG_CACHE_HOME" "$XDG_STATE_HOME"
# Actual continuation build (rebuilt/ was absent; use a new directory for subsequent builds):
bash "$KIT/source/rebuild.sh" build
# Equivalent direct invocation:
/usr/bin/blender -b --factory-startup --python-exit-code 1 --python "$KIT/source/ground_h1_path.py" -- --out "$KIT/rebuilt" --mode build
# Re-render the saved root master (replaces renders; invalidates sealed hashes):
/usr/bin/blender -b --factory-startup --python-exit-code 1 --python "$KIT/source/ground_h1_path.py" -- --mode render
# Fresh imports and actual-file checks:
bash "$KIT/source/rebuild.sh" import-glb
bash "$KIT/source/rebuild.sh" import-fbx
# To regenerate only the three corrected grayscale diagnostics:
/usr/bin/blender -b --factory-startup --python-exit-code 1 --python "$KIT/source/ground_h1_path.py" -- --mode render --gray-only
bash "$KIT/source/rebuild.sh" verify
/usr/bin/blender -b --factory-startup --python-exit-code 1 --python "$KIT/source/resume_checks.py"
cd "$KIT"
sha256sum -c verify/SHA256SUMS
```

Expected observable result: one flat square with a subtle southern H1 apron and one continuous winding worn-earth strip, no extruded edges; A0 mouth one uninterrupted colour/detail region; continuous A7 distance union with the stated short-radius risk; A8 rounded endpoint cap. Scripts require the provisioned NumPy environment, install nothing, and read no donor assets. Re-running verification writes diagnostics; re-seal before relying on old manifests.

## Remaining gates and preservation

No Unity/URP import, actual CharacterController or physics traversal, native sampler/mips/compression/aliasing test, integrated tree/G1/HUD test, save/load/gameplay test, GPU/CPU runtime performance/memory measurement, owner look acceptance, rights/publication decision, independent #788 review or production integration is asserted. Original mathematical soil/needle appearance is deliberate art calibration, not an ecological claim. Files-only source custody and provenance are recorded in SOURCE_RIGHTS; no D-032 decision was made. Dashboard/Drive synchronization **NOT VERIFIED**, with no board/Ops writes allowed in this packet. These are reviewable original scratch candidates only.

## Complete actual file inventory

All paths below are absolute. SHA256SUMS covers every scratch file except itself (self-hashing is undefined); its actual bytes/hash are included here. FILE_BYTES includes every file, including its own size and the manifest size. No binary below is tracked or publicly published.

| Absolute path | Bytes | SHA-256 |
| --- | ---: | --- |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/CLAIM.md` | 2638 | `0e3d560eae9c21e37e6e01dd85c247a0187eb62ed6d20685ad74635c23cff7d7` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/SOURCE_RIGHTS.md` | 2242 | `7f904f98b4af4add7f772f4705e7445553f3a9c128269c4a43df75bc86d23fbe` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/blend/H1Path_Ground_v1.blend` | 938040 | `0ec555544abd9cbd20b984e00e9a4aba02452762dafc37f95434018ec4e4b7c5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/blend/H1Path_Ground_v1.blend1` | 938040 | `d44472e658ce5046c0d4bf75ad1246689daa59b9ee2169506ed807ab23ae8e32` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/export/H1Path_Ground_v1.fbx` | 294508 | `46fba9593fe85aa882e4408c6281de50813d70ad8ea44abd5fb92f86b26b29d7` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/export/H1Path_Ground_v1.glb` | 277112 | `5af65119d08bb5a1db4e9da37743850f39fee8ce11912afe33945e4ff664a56c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/blend/H1Path_Ground_v1.blend` | 938040 | `181a07bf847bbae790fba6751f74105b246bf8441f2218578d2e8fccaa6be0a1` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/export/H1Path_Ground_v1.fbx` | 294540 | `f0926eb3218618ba3a1feceb2e10113c3972ebd29d84605f97dc05f9ff4f7b84` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/export/H1Path_Ground_v1.glb` | 277112 | `5af65119d08bb5a1db4e9da37743850f39fee8ce11912afe33945e4ff664a56c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/source/recipe.json` | 33072 | `cd7eda4c97b10759a9650ea84ee62c834caf55618dc7deda8184301ebf4c77fc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/textures/CompactGround_Detail_512.png` | 8292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/rebuilt/verify/build-result.json` | 9294 | `11b890685a7931acddbeff24718074f82e06232e02dccf28ace74a62974aa23f` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A7_Northbound_colour_1280x720.png` | 971053 | `b768444b964e54fea7f2484d2af492acc18a1a7edc7a70f1609fa45adc7c071d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A7_Northbound_colour_1920x1080.png` | 2135711 | `dfa9bbe96cc8fc14904a9022cba4a2e0d3727fea19f1dd5377ad111d431c4237` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A7_Northbound_linear_gray_1920x1080.png` | 997627 | `729182cfc267f44a8e50a8e4925c96f503e294dbba6ec749d76731fb7915f92c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A7_Southbound_colour_1280x720.png` | 1047446 | `6ff0effaaa715e49add53d09906d2064562374e3bde33f2bc5986ac50d7881d7` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A7_Southbound_colour_1920x1080.png` | 2319335 | `02832a4d1003c2a6cdc85b9935b16357e096f78fdd322520091bef27ec4fc0bc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A8_LookingBack_colour_1280x720.png` | 1018185 | `ed4b669d1561e823e07eb670c4c708e210ad5d5021494dc2209408ce24e4ae14` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/A8_LookingBack_colour_1920x1080.png` | 2244608 | `e4970ae7f21d807e149b6bbfd701f623653ef60ad3fee05c16767d7f21bb1237` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Apron_P0109_colour_1280x720.png` | 1015028 | `c132f82d4597d3909a385731df69bff91cd750bbf45a4ff24c3dd5420422dba8` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Apron_P0109_colour_1920x1080.png` | 2251925 | `48e092f59b1dc22c3474a4814f5d2583ea06418b823204680099ad9295b62e6e` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/FirstBend_A0_A1_colour_1280x720.png` | 975345 | `bae5298a78a55f1f3b9ef22f5e730139bb0d8d5546e5b9915c94d923688ed018` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/FirstBend_A0_A1_colour_1920x1080.png` | 2141364 | `09ca1d7c4dea73c932c3c727ad3d50dccfa3e51382cf46a0d29ad4869a1df1bf` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Plan_colour_1920x1080.png` | 1736658 | `6563f7a27bca50fbd8dff9a558da6d92f914c23ac621769c314d4b6d2513f78e` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Plan_linear_gray_1920x1080.png` | 963630 | `d2c9fadfc41c4d9463a827438212c20fd796faeb5c1a2a7c6fcd0d72f5303eff` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Spawn_A0_colour_1280x720.png` | 937710 | `d875867e3e7de13d125fe253c1e3dc753a884ed73ce56fc16904cc88599d7ab4` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Spawn_A0_colour_1920x1080.png` | 2035822 | `40b2c215f49d4b369572a18f21d289f2d7f41fc6e87a9243f66b27818d945700` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/renders/Spawn_A0_linear_gray_1920x1080.png` | 946279 | `2b5b0c05e17fcfb79a02da4920834209a9853158e11cf5eb62b4c9b094f1c8a4` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/finalize_handoff.py` | 23888 | `116b61ff9dc0518c04aedc8a4c944b827e5d06fce534d507f04650360d6f9b31` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/ground_h1_path.py` | 27841 | `f585d690080c2ae7e00a2bc3ab659dcf8b84e08ecbc781e08f12ffd0556fdd98` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/rebuild.sh` | 1186 | `9cc48030ef48336b5aeb5382d9b6d359ffdad2e021141162986b5427b66e64f4` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/recipe.json` | 33072 | `cd7eda4c97b10759a9650ea84ee62c834caf55618dc7deda8184301ebf4c77fc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/reimport_check.py` | 2314 | `de74a6bd172b43052b22981d039465356d9a8fba0815da6fe10c2ac0a3cb48ad` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/resume_checks.py` | 6179 | `3b87e4be2bba90d1cffa9735f4787bbad97dfd3768aded461805d299e5125b27` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/source/verify_outputs.py` | 8281 | `8f7079c9dd8271caa3f6216a37c9e49014230d3cf0c4a8307612c9f97d461838` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/textures/CompactGround_Detail_512.png` | 8292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/FILE_BYTES.tsv` | 11383 | `759a3c1bb3dff443c19f8398e2df84e8a45d06ef9776c294e3ce61db18a14456` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/INHERITED.md` | 3323 | `ec68a90db67c178d1ea1fe20afbd6a4ec44ed839f46c69404503aa4b1f55bd4b` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/RESULT.json` | 12557 | `169700ea094b6dbbfc7ca2f7935513b98ff8cfd4a57797aefea2221d95593c60` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/SHA256SUMS` | 10349 | `92f2d64bcb71de486d055ba2d258ac9ef3a18ed755ee709c6322bad4dda052d8` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/anchor-final-status.txt` | 390 | `a3ffcbb4da30e20c1edce5e8d62edde202b89e60085e1688e04fabd2c2345072` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/blueprint.md` | 51968 | `9a3e95cab99a3bb388ecd1dc9fd497746bc6cad46792f074b867b634ce9c2403` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/build-result-initial-diagnostic.json` | 9294 | `34a554df682855bd9b9baa738acff69585e983a114a19bc1f88603a281c8862e` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/build-result.json` | 9441 | `d2235f6682ffd73a4f19cf8409efea7615677855363885740d2a2c7c390a487d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/build.log` | 5062 | `de96e560ccb7c1055a7771771fdb7161d8336c5eb443cabe3c4909037da46fbc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/checks.txt` | 34010 | `f64a063c478acb5038e54393f265185682f5788bb454f6dbaebd80304f00671a` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/determinism-diff.json` | 359605 | `ade06f7ad50d85af0517805ff31d016c73c08e6bebb54d2c647a684afb5bf2c6` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/file-readback.json` | 22559 | `4a91bddb99cb76f0acb0149dcfa5c8e5d386d9cf34a481bcc6f88c99f1856c5f` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/ground_h1_path-inherited.py` | 27290 | `e6259a0dd132574c06c09f1b9f736629ddb33e635f02c18f62836f7c16f48689` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/inherited-gray/A7_Northbound_linear_gray_1920x1080.png` | 1248194 | `a0862de9eb0a38fcf9002ef327aebd9229cd3f9f20b89ec55d00475a9bfd61c9` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/inherited-gray/Plan_linear_gray_1920x1080.png` | 1839268 | `c803be0298bd2a4f49ad7930cc2e5141d732a4d27df0b45dd1ac64165969a22a` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/inherited-gray/Spawn_A0_linear_gray_1920x1080.png` | 1488476 | `bd358ec33f94706500e9275dd9aab205acb9b5c62eb3845190fb22c2a4f8dfce` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/inherited-inventory.json` | 15019 | `2eebc22cd3841c3c0c7eb666f20233274c9519c6d6bb2e9b0a1323700f7224d9` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/issue-resume.json` | 8117 | `0190a4b582ddba8feb026f32a158cf30bcc270538f73e1dda1aac2cc6d03af29` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/latest-texture-check/textures/CompactGround_Detail_512.png` | 8292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/latest-texture-check/textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/latest-texture-check/verify/texture-data.json` | 557 | `109f21d9fa702748e9ad667b782babf18fab9611540f5ac52b8612874d86be92` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/latest-texture-check.log` | 28 | `d1ff5324a507e1be994b9d83c3c67acac81ddad729cd77cfc30cea5acfcdeef6` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/live-active-tasks-resume.md` | 779213 | `cd38c93a0729f624abf34ba31ac1861e1a2b46b0a063a3ee2af759cad3c1672d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/live-active-tasks.md` | 779213 | `cd38c93a0729f624abf34ba31ac1861e1a2b46b0a063a3ee2af759cad3c1672d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/material-probe.log` | 174 | `faecd9769c66bbfd4a2344feee70cf93060fe40a2f1807ff583c83d20acd9735` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/packet.md` | 21638 | `07f1aec6bc7531b234587c83ce23b0b85e2f14366dd28fc45fb41c3a8a0d8c76` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/primary-after.txt` | 336 | `5178e07704b42488d4daa8ecde09d4e900fdc22b3b0de10189f6dc6df4d9a4a5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/primary-before.txt` | 336 | `5178e07704b42488d4daa8ecde09d4e900fdc22b3b0de10189f6dc6df4d9a4a5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/blend/H1Path_Ground_v1.blend` | 938040 | `983014c8bed1b897ec77783ae2cbae7cf4a0537b61e65861e0e81114df38fd22` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/export/H1Path_Ground_v1.fbx` | 294588 | `7312c2984414b566906c254ff3c22bb152cbe1ee3993a48480bcec040d696da4` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/export/H1Path_Ground_v1.glb` | 277112 | `5af65119d08bb5a1db4e9da37743850f39fee8ce11912afe33945e4ff664a56c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/source/recipe.json` | 33072 | `cd7eda4c97b10759a9650ea84ee62c834caf55618dc7deda8184301ebf4c77fc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/textures/CompactGround_Detail_512.png` | 8292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A/verify/build-result.json` | 9294 | `11b890685a7931acddbeff24718074f82e06232e02dccf28ace74a62974aa23f` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-A.log` | 5079 | `347b67ac8c5461ed6a596050e4eb333faaf7745738e879f1179473bce3a6a067` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/blend/H1Path_Ground_v1.blend` | 938040 | `8b00028b4a8e899f327a3315f63d8702686d674f175f276ba7b736a8ca3ddba5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/export/H1Path_Ground_v1.fbx` | 294588 | `5059855fb728192e2c9851f1df17a1eda26628f7430fd0b1f2dec81f22986b61` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/export/H1Path_Ground_v1.glb` | 277112 | `5af65119d08bb5a1db4e9da37743850f39fee8ce11912afe33945e4ff664a56c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/source/recipe.json` | 33072 | `cd7eda4c97b10759a9650ea84ee62c834caf55618dc7deda8184301ebf4c77fc` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/textures/CompactGround_Detail_512.png` | 8292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B/verify/build-result.json` | 9294 | `11b890685a7931acddbeff24718074f82e06232e02dccf28ace74a62974aa23f` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/rebuild-B.log` | 5077 | `24d98d09b5e4d30e56db7f7cac111073553151bd81f73dc30654db4edbe735e8` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-fbx-initial-diagnostic.log` | 416 | `ad820c206fd6c3f67469d9c3db5bb66d4684fa102caa99e05c595e593db0d158` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-fbx.json` | 2437 | `9c497c110f2df912287c273f946ea00926e5a22db97f07ba8bf1d1f584cdef4e` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-fbx.log` | 2483 | `1a70681df07fd7aa9531f235e98f23538b1e1a1928ac9dd8d61b47e6a07813d0` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-glb-initial-diagnostic.log` | 2599 | `0f800f7edcfa7c4321cd37dd6220bc11b5fe6a04151f4176ba686105c8dfcb33` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-glb.json` | 2234 | `c0aa6e8a0ebac4d9be1700c01550eabdb9e2a9983e8fff58b5fb53b5afc84ff6` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/reimport-glb.log` | 2338 | `770d24967973ac8ee7814033c8bf1ad6893864d33a949919abd7501ec597198d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/render.log` | 86940 | `6fc3a9a0770dc3a7752e4fbdc3730780aff89b0751f48a7f9b8a0679107d85ec` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-G0.txt` | 1540 | `8b9697585962edfe75b0c8d5427f58d2de8500d063e91e091ef560fb39f5be10` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-build.log` | 5069 | `963f202989f4ffef86f838d70f1d0328ed5a826633c790a0d7ee9d3bc2ba74f8` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-checks-initial-diagnostic.json` | 30948 | `f2ce0dcfe5ad0bd4e8c623190f0b5c3892216fd6071a5227db6c8a2b463a5440` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-checks-initial-diagnostic.log` | 16383 | `3a1fa759bed10aca2c761ff5125ef194e316c3f004f04f0dfd9318e7b5f8ff97` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-checks.json` | 35519 | `2f512f57ee380b66f220a5c2ec6e44c8cf90584ffc4bfb75c478917c7fe1a8ef` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-checks.log` | 19376 | `731ad1a94bcfd80baea698afb7ec6e417b2b8e190099e288d1570db4bfe9f600` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-comment-url.txt` | 68 | `f9ac00f2bfbfacffc4b8d443c5dda8054651da33810f0dce0a8c4ec11c023650` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-comment.md` | 1324 | `ed1aa7a05d6fd5542aed0bb6b406944e437226e3b9b543a427ff37b43ad91f0c` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-file-readback.log` | 12678 | `558dd6771fe224b9f3cc3b811721cb67a9605202294c645a61eb52715c6ef1a2` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-gray-render.log` | 15900 | `4d0f7d7e0d1309960a971713df2544f6feb8fba89e0868118a3c93c34c86b453` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-reimport-fbx.log` | 2483 | `1a70681df07fd7aa9531f235e98f23538b1e1a1928ac9dd8d61b47e6a07813d0` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-reimport-glb.log` | 2338 | `770d24967973ac8ee7814033c8bf1ad6893864d33a949919abd7501ec597198d` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/resume-storage-final.txt` | 249 | `e7713f118fca9703a471c9786360ecafbb665bfa6006157b4e96c1f59b82ecf3` |
| `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/H1Path/verify/world-reference.json` | 308748 | `342e1afc4d39cf44c22e434dac3b7ad56d9902084bf3f61026e89086a561a677` |
