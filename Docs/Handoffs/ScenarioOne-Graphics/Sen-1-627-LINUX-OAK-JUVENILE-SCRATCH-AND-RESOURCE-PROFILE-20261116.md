# Sen-1 #627 — Linux sessile Oak juvenile scratch asset (A-04a) and Blender resource profile

**Result date:** 16 November 2026. **Issue:** [#627](https://github.com/jermur788/CCF/issues/627). **Packet:** `docs/sen-1-627-linux-oak-asset-resource-handoff-20261116-v1`.
**Asset status:** **SCRATCH AUTHORED only.** Not REVIEWED, INTEGRATED, VERIFIED or ACCEPTED. D-032 stays OPEN and #619 stays DRAFT. This result neither approves nor imports anything.
**Phase B (Unity + Blender at the same time):** **NOT PERFORMED.** No approved Unity lease holder was active (details in §4).
**Scheduling verdict:** **NOT ENOUGH EVIDENCE** for running Unity and Blender in parallel. Use **TIME-SLICED** as the interim operating rule (§5).

## 1. Worker, lane and custody

| Item | Value |
|---|---|
| Worker | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code inside T3 Code. The only art author. Registered [RUNNING] on #627 at 16:33 UTC, before any directory, Blender or Markdown work. |
| Host | `jer-laptop`: Ubuntu 24.04, kernel 7.0.0-42-generic, Linux Mint Cinnamon on X11. i5-6300HQ, 4 cores / 4 threads, 0.8–3.2 GHz. 15.5 GiB RAM. 8 GiB swapfile. NVIDIA GTX 980M 8 GiB on driver 580.178.04, also the OpenGL renderer. Intel HD 530 present. |
| Blender | `/usr/bin/blender` 4.0.2 (Ubuntu package), already installed. **Nothing was installed or downloaded.** |
| Starting HEAD / context | `37cd047ed39fd649563f70602d458e2970f6780f` (fresh `origin/main`) |
| Docs branch / worktree | `docs/sen-1-627-linux-oak-scratch-resource-profile-20261116-v1` at `/media/jer/Files/CCF/Operations/CCF-627-Oak-Scratch-Resource-Profile-20261116` (Files ext4, docs-only, not a Unity lane) |
| Scratch directory (outside repo) | `/media/jer/Files/CCF/Verification/Sen-1-627-LinuxOakScratch-ResourceProfile-20261116/` on Files ext4. Created by this worker after the coordination check; vacant beforehand; owner `jer`, mode 775. |
| Tracked output | This one Markdown file. No binaries, images, meshes, telemetry, `Assets/`, scene, C#, materials, saves, Prefs, `ProjectSettings/`, `Packages/` or CCF-Ops changes. |

**Coordination at start and end.** `unity-lock status` (read-only) returned `free`, `metadata: None`, `unity_editor_pids: []` four times: at registration, before the measured build, after the render and at the end. No Unity process ran at any point. #626 was RUNNING as a read-only docs audit in another session; it used light CPU (`git`/`du` bursts, see A0) and launched no Unity. The lease was never acquired, released or bypassed.

## 2. Asset A-04a — `Oak_Sessile_Juv060_A04a_Scratch`

**What it is.** One original, procedural sessile Oak (*Quercus petraea*) nursery-transplant prototype, 0.6 m tall. Fixed seed 627 in `tools/build_oak_juvenile.py`. No external meshes, textures, scans, photographs, add-ons or asset libraries were used. The existing in-repo `Sessile_Oak_Mature_01` and the 2 m Scenario One sapling were **not** opened, copied, scaled or modified. Rebuilding from the script reproduces byte-identical OBJ LODs and stats JSON; I checked this once.

| Property | Value |
|---|---|
| Units / axes / pivot | 1 BU = 1 m, Z up. Origin = root collar at ground (0,0,0). The stem continues 0.020 m below ground so the plant doesn't float on uneven terrain. |
| Height | Leader bud top **0.600 m** (bbox max Z 0.6002; bbox height 0.6205 including the 0.020 m below ground) |
| Crown spread (bbox X × Y) | 0.244 × 0.238 m |
| Form | Single leader with two visible growth flushes: spring flush to 0.335 m, with a bud-scar ring, then a lammas flush to 0.592 m. 4 short laterals (0.06–0.15 m): one weak low lateral, two at the flush boundary, one in the lammas flush. Clustered ovoid buds at shoot tips. |
| Leaves | 30, alternate, 2/5-type spiral (144°), crowded toward each flush end. Blade 46–85 mm (mean 64 mm). Petiole 10–18 mm (mean 14 mm). Obovate outline, widest above the middle, wedge-shaped base without auricles, 4–5 pairs of shallow rounded lobes with narrow sinuses. Slight V-fold and droop. |
| Stem | 9.2 mm base diameter, tapering to about 3 mm; slight flare at the root collar |
| Materials (3) | `Oak_Juv_Stem_Older` (olive grey-brown), `Oak_Juv_Shoot_Current` (red-brown: lammas flush, laterals, petioles, buds), `Oak_Juv_Leaf` (dark green Principled base colour, 18 % translucent mix, back-face culling off). Constant colours only: no textures and no vertex colours. UVs are present. |

| LOD | Tris | Verts | Detail |
|---|---:|---:|---|
| LOD0 | 5,171 | 4,013 | 8-sided stem, full lobed margins (8 samples per lobe cycle), petioles, 16 buds |
| LOD1 | 2,188 | 1,858 | 5-sided stem, lobes at 4 samples per cycle, petioles, terminal bud only |
| LOD2 | 222 | 243 | 3-sided stem, unlobed 6-tri leaf plates, no petioles or buds |

**Botanical basis and unknowns: all of it needs research review.** The form choices are general field-guide knowledge, not field data for this project: two flushes, crowded leaves at flush ends, clustered terminal buds, an obovate leaf with a visible petiole and no auricles, and shallow lobes. **Calibration assumptions:**
- leaf size for a nursery transplant;
- 30 leaves, 4 laterals and the 0.24 m crown;
- stem diameter;
- lobe count and depth;
- colours.

**Open:**
- **The packet calls the plant "first-year".** I could not verify that a 0.6 m transplant is first-year. UK/Irish nursery size classes such as 40–60 and 60–80 cm often correspond to 1+1 or 1u1 stock, but **that is UNVERIFIED and needs a nursery-standard reference**.
- No roots are modelled; the plant is shown as planted.
- No autumn, dormant, browsed or damaged states.
- No axillary buds.

**Known visual limitations:**
- Leaves are single-sided geometry and need a two-sided shader in Unity.
- At LOD2 the leaves are coarse rhombic plates.
- Leaves overlap in the dense upper cluster.
- Two leaf ranks near the top look edge-on in the front elevation.

**Exports.** Local OBJ only (`-Z` forward, `Y` up, metres). **FBX export is unavailable:** Ubuntu's Blender 4.0.2 FBX add-on imports `numpy`, which is missing, and this task installs nothing. A future integration task should export FBX from a Blender build that bundles numpy, or decide how to handle this. **No Unity import was attempted.**

### Local files and SHA-256 (outside Git; not uploaded)
Root: `/media/jer/Files/CCF/Verification/Sen-1-627-LinuxOakScratch-ResourceProfile-20261116/`. The full list (35 files) is in `MANIFEST.sha256`, whose own SHA-256 is `b8d7e81f242dd07367857a7cd85fc3712bfa2e5672d684e131bf2a57b4f1ef0c`.

| File | SHA-256 |
|---|---|
| `asset/Oak_Sessile_Juv060_A04a_Scratch.blend` (editable source; 3 LOD objects + `Reference_NOT_ASSET` collection: ground disc, 1 m pole in 0.1 m bands, 0.6 m marker, sun, camera) | `e3a65dfaa301c7f9bc8c3465fcb790fd106f06703dc75a851fb8c5d7c978509f` |
| `asset/…_LOD0.obj` | `1ee5b7b9d29a156df624a2a4d7e0865c793529c1735d10f0ec3c4a98011399c7` |
| `asset/…_LOD1.obj` | `e7d3a7bf738c1d39c390b395c69b26468da544526e1a8cc566cc2e0875b32341` |
| `asset/…_LOD2.obj` | `077fba88afd62d124c50a79e54e101fb917b34ba1cbe6ebccb6af15259dfeca3` |
| `asset/…_LODn.mtl` (identical ×3) | `bfb93d0990e70b25ab7d78bd7b03a9ba9d67dee89c83e1c8687628ff6f6267a2` |
| `asset/…_stats.json` (counts, bbox, leaf/petiole ranges) | `a1656d46fb33f18f489d6cb3b37034cbf439ee7af0da279828005e9b60650ff9` |
| `asset/PROVENANCE.md` | `3eed651102833cce606921055fbb7c4de8795f574693250c9b407ebad1e8c348` |
| `tools/build_oak_juvenile.py` (**the original source**) | `76edba3b0be8e6e6560c7093ef2c479e064c7b00b73f3c580a03275675119b3a` |
| `renders/01_hero_three_quarter.png` | `003832bd20238a4cc65c376cb2a110a81a8c153eb2d62aa6c8aa54dda0cf1d2a` |
| `renders/02_front_ortho_scale.png` (bud top at the 0.6 m marker) | `9b76305573e73679d37143ab0edae0f1b86a2c582d575e719751baaab441f392` |
| `renders/03_top_plan.png` | `8823b77cf53b774f18fb4506e04bc33589d79a070dfe919a100b69e202b60214` |
| `renders/04_leaf_closeup.png` | `1bd7e2d50079f4e1665700230359fc1ee78aeacb0332a5d20117ba1e22c40cab` |
| `renders/05_lod_lineup.png` | `c85a8e9c025da717bbbaac611ace4d1f0de53c9c930f3d7dbcab8e7b8d08579d` |
| `telemetry/samples.csv` (all raw 2 s samples) | `88a0644beff79d7a712c25c93990d85e0c621e0ff6d0abad41b716c75f039a60` |
| `tools/sampler.py` | `01f23d4d2ce5998f775b7536977579fde595c03a9fbbc6bf413ba9c164db008a` |

The renders are EEVEE previews under neutral reference lighting. They are a scratch review aid, not a qualified Unity look.

## 3. Resource measurements: idle vs Blender only

**Method.** `tools/sampler.py` ran at 2.0 s cadence and read:
- psutil;
- `/proc/vmstat` (swap pages in/out, major faults);
- `/sys/.../thermal_throttle` counters;
- coretemp;
- `nvidia-smi --query-gpu`.

Process data is limited to **names**, RSS and CPU%, with no command lines. "CPU %" means whole-host utilisation across 4 threads. "Blender CPU" is % of **one** core. Every sensor was available, so no value had to be marked UNKNOWN. `pidstat`/`iostat` aren't installed, and I didn't install them. Peak RSS per Blender run comes from `/usr/bin/time -v`. The desktop session had normal background apps, including T3 Code/Claude sessions and #626's read-only `git`/`du`. That is environmental variance, not an isolated lab.

| Stage (UTC 16 Nov) | n | Host CPU % p50/p95/max | Avail RAM GiB min | Swap used MiB max | Swap out pages/s max | Disk write MiB/s max | GPU util % p50/p95/max | VRAM used MiB max | GPU / CPU-pkg °C max | Throttle Δ | Blender peak RSS (time -v) |
|---|---:|---|---:|---:|---:|---:|---|---:|---|---:|---|
| A0 idle 60 s (17:13) | 30 | 5.3 / 30.2 / 34.8 | 12.67 | 36 | 0 | 3.0 | 0 / 6 / 10 | 201 | 51 / 58 | 0 | — |
| B1 scripted build (17:22) | 10 | 3.3 / 14.9 / 22.7 | 12.60 | 39 | 0 | 0.3 | 3.5 / 16.5 / 21 | 185 | 56 / 57 | 0 | **218 MiB**, 1.61 s wall |
| B2 GUI Material Preview orbit, 1st run **INVALID** (unfocused) | 30 | 5.8 / 13.2 / 28.1 | 12.38 | 39 | 0 | 4.3 | 0 / 13.5 / 24 | 488 | 53 / 57 | 0 | 487 MiB (whole GUI run) |
| B3 GUI Edit Mode, 1st run **INVALID** (unfocused) | 15 | 4.9 / 23.7 / 27.8 | 12.32 | 39 | 0 | 1.7 | 0 / 6.5 / 10 | 384 | 55 / 58 | 0 | (same run) |
| B2r GUI Material Preview orbit, focused (17:25) | 30 | 34.4 / 53.8 / 63.4 ‡ | 11.81 ‡ | 58 ‡ | 1019.5 ‡ | 59.3 ‡ | 25 / 39 / 41 | 418 | 60 / 70 ‡ | 0 | **379 MiB** (whole GUI run) |
| B3r GUI Edit Mode + live vertex edits, focused | 15 | 7.0 / 17.2 / 18.2 | 12.41 | 58 | 0 | 1.2 | 0 / 26 / 27 | 359 | 59 / 60 | 0 | (same run) |
| B4 EEVEE preview renders, 5 images (17:27) | 10 | 2.8 / 33.1 / 34.0 | 12.42 | 58 | 0 | 0.5 | 0 / 0.6 / 1 | 266 | 60 / 62 | 0 | **348 MiB**, 3.93 s wall, 2.56 s summed render time |
| A1 idle after 60 s (17:27) | 30 | 2.1 / 14.2 / 23.2 | 12.63 | 58 | 0 | 2.0 | 0 / 0 / 0 | 194 | 61 / 61 | 0 | — |

‡ **Contaminated by unrelated system work.** From 16 s to 54 s into B2r, Linux Mint's background updater ran (`flatpak-update-worker`, `flatpak-system-helper`, `mint-refresh-cache`). Over that period Blender stayed flat at about 21 % of one core. The B2r spikes in host CPU, disk writes (≤59 MiB/s), a short swap-out burst (≤1019 pages/s ≈ 4 MiB/s) and the 70 °C CPU peak line up with the updater, not with Blender.

**Clean sub-windows** (from `telemetry/stage_subwindows.json`):

| Window | n | Host CPU % p50/p95 | Blender % of one core p50/max | GPU % p50/max | VRAM MiB p50 | Avail RAM GiB min | Disk W MiB/s max | Swap out |
|---|---:|---|---|---|---:|---:|---:|---|
| Idle pooled (A0 + A1) | 60 | 3.0 / 29.9 | — | 0 / 10 | 194 | 12.63 | 3.0 | 0 |
| B2r Material Preview orbit, updater-free (t ≤ 14 s, t ≥ 56 s) | 10 | 11.6 / 16.0 | 21 / 21 | 33 / 41 | 397 | 12.40 | 0.1 | 0 |
| B3r Edit Mode + live edits, while focused (t ≤ 14 s) | 7 | 16.0 / 17.8 | 38.5 / 40 | 26 / 27 | 342 | 12.41 | 0.1 | 0 |
| B3r after apparent focus loss (t ≥ 18 s) | 7 | 2.8 / 4.0 | 3 / 3 | 0 / 0 | 342 | 12.43 | 1.2 | 0 |

**Findings (measured, single host, single session):**
1. **Blender-only demand from this asset is small.**
   - Interactive viewport: roughly 0.2–0.4 of one CPU core (~5–10 % of the host), 26–41 % of the GTX 980M, and about +150–220 MiB VRAM over idle.
   - Process RSS: 218–487 MiB.
   - Host available RAM never fell below 11.8 GiB (76 % of physical), including the updater overlap.
   - Swap barely moved: used swap rose 36 → 58 MiB, with no swap-in thrash.
   - No thermal-throttle counter increments occurred.
   - The scripted build (1.6 s) and the 5 EEVEE previews (2.56 s summed render time) are short spikes.
2. **Blender's GUI workload depends on window focus.**
   - The first GUI run opened with `--no-window-focus`. Its orbit timer ran at about 1.8 Hz instead of the 30 Hz target (175 ticks in about 96 s), Blender used about 3 % of a core, and the GPU stayed near 0 %. That run is marked INVALID as an interactive-load sample.
   - The focused repeat ran 1,758 orbit ticks (about 18 Hz) and 353 live-edit ticks.
   - About 16 s into its Edit Mode window, Blender fell back to about 3 % and 0 % GPU. Most likely the window lost focus or was covered.
   - Inference, not proven: on this X11/NVIDIA desktop, an unfocused or covered Blender window does little work. A focused Blender window therefore competes with Unity for the desktop and pointer far more than for raw CPU/RAM.
3. **Background OS work was a bigger disturbance than Blender.** The automatic Mint/flatpak update tripled host CPU and produced the only disk-write and swap-out bursts of the session. Owner-controlled comparisons should control for it, by pausing or scheduling those updates. That is an owner decision; this worker didn't change any system setting.
4. **Variance:** idle p95 ranged from 14 % to 30 %, driven by other sessions' `git`/`du`/T3 Code bursts. With n = 7–30 samples per window, treat differences under about 10 percentage points of host CPU as noise.

## 4. Phase B: Unity + Blender at the same time — NOT PERFORMED

At every check, the Unity lease was `free` with no Editor PIDs and no approved Unity owner, so there was nobody to ask for consent and no comparable Unity workload to observe. As the packet requires, **I didn't launch Unity, acquire the lease, create a Unity job or simulate overlap.** No Unity-alone or Unity+Blender figure exists in this result, and nothing here should be read as a coexistence PASS.

**Repeatable, owner-controlled protocol for a future test.** Run it only when an approved lease holder volunteers an already-planned workload.
1. **Consent and scope.** The lease holder names the Unity workload, a fixed window (60–120 s) and their stop authority. Blender's worker never touches the Editor, the lease, the project or Prefs.
2. **Pre-conditions.** Same host, same power and thermal state, at least 5 min of settle time. The owner pauses OS auto-updates for the window. Other heavy apps are closed. `unity-lock status` and Editor PIDs are recorded by name only.
3. **Comparable Unity workload,** one of:
   - (a) Editor idle with the same scene open;
   - (b) one owner-triggered script compile or domain reload, timed by the owner;
   - (c) a fixed Play-mode run of the same scene and duration.
   Each is repeated under an **ABBA** order: U-alone → U+Blender → U+Blender → U-alone.
4. **Blender load variants:**
   - **B-bg:** `blender -b` build plus an EEVEE preview loop, with no window;
   - **B-gui:** a viewport orbit on a separate monitor or workspace that never takes focus from Unity.
   **No Blender GUI during any pointer- or xdotool-driven Unity gate**, because the Blender viewport depends on focus and Unity gates depend on the pointer.
5. **Telemetry.** `tools/sampler.py` at 2 s, which already records Unity process count and RSS by name, together with the owner's own Unity-side timings (compile/reload seconds, Play-mode frame time). Report p50/p95/max per window and the Unity timing delta only when the workload matches.
6. **Stop rules.** Stop Blender immediately if any of these happen:
   - available RAM drops below 15 % of physical (< 2.3 GiB here);
   - swap-in/out is sustained above about 25,000 pages/s (≈ 100 MiB/s) for 10 s;
   - any thermal-throttle counter increments;
   - CPU package exceeds 90 °C;
   - an OOM kill occurs;
   - the owner reports stutter or a gate failure.
7. **Interpretation.** Report it as correlation under shared desktop variance, not as a cause.

## 5. Scheduling recommendation

**Verdict: NOT ENOUGH EVIDENCE** for PARALLEL use of Unity and Blender on `jer-laptop`, because Unity's footprint wasn't measured here at all. **Interim operating rule: TIME-SLICED.**
- Blender asset work, background builds and short EEVEE previews are light enough to schedule freely while **no Unity work** is running. Measured headroom: ≥ 11.8 GiB available, < 0.5 core, ≤ 41 % GPU, no throttling.
- Don't run an interactive Blender GUI while any Unity task holds the lease, unless that owner explicitly agrees. The main measured risk is focus and pointer contention, not CPU or RAM, and the RAM footprint of a Unity Editor on this machine is still unmeasured.
- Run the §4 protocol when an owner volunteers. If it shows Unity timings unchanged within noise and RAM above the stop threshold, upgrade the rule for **background Blender only** to PARALLEL ALLOWED with owner consent.

## 6. Deviations and side effects (disclosed)

- **Two GUI windows instead of one.** The first opened unfocused and was throttled (INVALID). The focused repeat took desktop focus for about 112 s. Both used disposable working copies, verified byte-identical to the source `.blend` and then deleted. The source was never saved from the GUI.
- **`/tmp/quit.blend` overwritten.** Blender always writes it on quit, so any earlier "Recover Last Session" file from the user's own Blender sessions was replaced. I moved the R2 copy into the scratch folder (`gui-work/quit.blend.from-R2-session`). Blender user config, scripts and datafiles were redirected to the scratch folder's `blender-user/`, and `~/.config/blender` is unmodified.
- **OBJ instead of FBX,** because numpy is missing and nothing was installed (§2).
- **Iterations before the measured run.** Three leaf-outline revisions were rendered from a `debug/` scratch folder: corrugated banding → stepped lobes → rounded normal-offset lobes. That folder was deleted. The measured B1 build is the final script.
- The primary checkout's pre-existing user changes (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md` on `ops/workflow-foundation-v1`) were not touched.

## 7. Not done / next decisions for #424 / #425

- Not done: no Unity launch, import, prefab, material, LOD group, scene placement, native or performance check, save/load or Windows build. No source or asset commit.
- Needed before any adoption, in order:
  1. independent art and botany review of the renders and the build script, including whether the plant is first-year or 1+1;
  2. a D-032 decision on storage and licensing;
  3. a signed #619 asset spec;
  4. an FBX path;
  5. Unity import into an approved separate scene with a two-sided URP leaf shader;
  6. LOD transition distances;
  7. rendered player review and a performance check;
  8. user acceptance.
- Owner decision: whether to schedule the §4 coexistence test, and whether to pause OS auto-updates during measurement windows.
