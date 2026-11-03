# Stand Map Species Canopy Differentiation — Implementation Handoff (#232)

**Status:** IMPLEMENTED AND VERIFIED BY THE IMPLEMENTER — **INDEPENDENT CROSS-PROVIDER REVIEW PENDING.** Not accepted. The success token is **not** claimed here (it requires an approved independent review).
**Implementer:** Claude Sonnet 5.5 (medium), local T3 Code sub-agent, sole production writer.
**Release / packet:** `Docs/Handoffs/StandMap-Species-Canopy-Differentiation/RELEASE-20261103.md`, `TASK-PACKET.md` (on `origin/main`).

## Lineage

| Item | Value |
|---|---|
| Production base (exact) | `6849caa0ef05baf1a52b95465ec6bba4ff94202b` (`integration/scenario-one-player-test-ready-v1`) |
| Feature branch | `feature/standmap-species-canopy-differentiation-v1` |
| Production commit | `72b4efbd1c653fa2231f632a79030a83ff76f833` |
| Final head | the commit that adds this file (child of `72b4efbd`; `git diff 72b4efbd HEAD` is this document only) |
| Verification overlay (local, unpublished) | `qual/standmap-species-canopy-v1` @ `9e3ed546152bdb1a90762cfba508e08d12d4b56f` = `72b4efbd` + merge of `69c6f0db` (Tier-3 tooling, incl. the exact six-tab Rack correction) + the disposable shots harness. `git diff 72b4efbd 9e3ed546 -- Assets Packages ProjectSettings` is empty, so every Unity gate ran on a tree whose product files are identical to the feature candidate. |
| Before control (local, unpublished) | `qual/standmap-species-canopy-before-control-v1` @ `29643f89…` = overlay with `StandMapCanopyRaster.cs` restored to base; `Assets/` identical to `6849caa0`. |

The qualification lineage (`69c6f0db`) is preserved; unrelated Tier-3 tooling was **not** committed to the feature branch.

## Changed files (production diff `6849caa0..72b4efbd`, 2 files, +69/−4)

1. `Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs` — `MakeCrown` only:
   - **Living conifer (Sitka):** outline is a triangle-wave whorl star (9–12 tips; 6–8 on the minimap / small crowns), tips ≈ crown radius (reach factor 0.94±0.10 ⇒ ≤ ~1.04 r), notches at 0.70 r (0.62 r small). Branch-ray shading (lit along each branch axis, shaded between), darker needle mass (×0.90) and a bright central leader. Texture noise lowered (clump 0.10). Ray strength reduced to 0.07 on the minimap / radius < 3.2 px, so detail the pixels cannot carry is not drawn; the pointed outline remains.
   - **Beech:** outline lobing reduced (amp 0.12/0.06 → 0.07/0.03) so it reads as a smoother rounded crown than Oak.
   - **Unchanged:** Oak, standing-dead crowns (a dead conifer keeps its previous ragged outline), stumps, shadows, ground, sprite cache key, bucketing, draw order, `StandMapCanopyLayer`, transforms, colours/palette.
2. `Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs` — five durable species-silhouette assertions (outline sharpness conifer > 1.5× Beech and > 1.15× Oak; Beech smoother than Oak; conifer tip extent within ±12 % of the tree's crown radius; deterministic silhouette; dead conifer still draws). No existing assertion changed or weakened.

No gameplay, geometry, crown-radius scale, cache, state, simulation, save, schema, Rack, MFM, economy, package or asset change. No build.

**Trade-off (disclosed):** a star covers less area than a disc of the same radius; Sitka crowns therefore look slightly less “filled” at the same stand density, but tips still reach the real crown radius (asserted offline and by the 1,416-tree fingerprint being unchanged).

## Evidence (all under `/media/jer/Files/CCF/Verification/StandMapSpeciesCanopy-20261103/`)

- `offline/out_before|out_after/` — plain-.NET mixed-species renders (768/256 px) and species sheet; `offline/standmap_check/`.
- `unity/before/evidence/`, `unity/after/evidence/` — **before/after mixed-species** (1,344 Sitka + 36 Beech + 36 Oak, 1,416 trees, in-memory fixtures, nothing saved): `map-access-{1280x720,1920x1080}.png` (full Stand Map), `map-marks-…png` (fell/crop marks over crowns), `hud-minimap-…png` (HUD + minimap), `canopy-full-…png` / `canopy-mini-…png` (the exact canopy pictures), `notes.txt`.
- `unity/before-ATTEMPT1-startmenu-covered/`, `unity/after-ATTEMPT1-startmenu-covered/` — first attempts, start menu covered the captures (harness lacked `CCF_TEST_DIRECT_ENTRY=1`); kept, not evidence.
- `gates/` — `13-combined-v2/`, `38-rack-map/`, `40-rack-confirm/`, `14-parity07-r2/`, `completion/{01-compile,33-completion}/`, `logs/`; `run_affected.sh`, `run_affected2.sh` drivers.
- `harness/StandMapSpeciesCanopyShots.cs` (also on the local overlay branch under `Tools/Verification/StandMapSpeciesCanopy/`).

## Results

| Gate | Candidate | Result |
|---|---|---|
| Offline Stand Map check (`run_standmap_check.py`, matrix step 21) | feature | **176 passed, 0 failed** (171 existing + 5 new) |
| Mixed-species before/after render, both sizes | overlay @ `9e3ed546` vs control | map-model fingerprint `AC4F3EFD31D360D6` (1,416 trees: positions, crown radius, kind, state, seed, height), simulation WorldHash `8B26C9DB5F085F9E`, crown sprites made (99) are **identical before and after** |
| Combined HUD / minimap (`combined-v2`) | overlay | **465 checks, 0 fail, 24 shots** (1280×720 + 1920×1080) |
| Rack map-planning | overlay | **142 checks PASS, 8 shots** |
| Rack confirm / minimap | overlay | **213 checks PASS, 21 shots** |
| PARITY-07 rendered | overlay | **PASS**, 36 screenshots (run 1 refused by preflight because the lane was dirty from the previous Editor run's known side-effects; no Unity ran; rerun on a clean lane passed) |
| Tier-3 delta: compile + Scenario One completion | overlay | both **PASS**; completion fingerprint `9C77EB4BEE427FB9` (matches the accepted value) |

Not rerun (per the user-directed test-volume reduction): the rest of the Tier-3 matrix, MFM/S1-C/S1-D/economy/persistence/Legacy40/Reference gates (no code path they exercise was touched; the diff is presentation-only in the map raster). The matrix runner itself was not used because it hard-codes the `FINAL-TIER-3` lock identity; the same per-gate commands were run under this task's own lock (`CCF-232`).

Rack **six-tab** verification correction: present only in the overlay (from `69c6f0db`); the rack-map gate passed with it. It is not in the feature branch.

## Visual assessment (implementer's reading — review requested)

Before: Sitka is a dark rounded lobed blob, close to the broadleaf silhouette. After: Sitka is an unmistakable dark pointed star with ray shading and a bright leader, at 1920×1080 and 1280×720 full map, in isolation and in the mixed stand; Oak keeps its coarse irregular lobing, Beech its smoother rounded outline; no broadleaf acquires spikes. Racks/access lines, marks, path, player marker and legend remain legible over all crown types. On the minimap the pointed outline survives at ~1.6 px/m as a spiky dark texture; Sitka vs broadleaf is distinguishable by silhouette and not only colour. Known limitation: the minimap points are only a few pixels, so there the cue is partly texture + darker mass.

## Performance / redraw (diagnostic, software-presented Editor)

| | before | after |
|---|---|---|
| minimap raster, 1,416 trees (mean of 5) | 6.6 ms | 5.8–5.9 ms |
| full raster 768 px (mean of 5) | 55.0–56.3 ms | 47.7–49.9 ms |
| crown sprites made | 99 | 99 |
| first-draw full map | 56.0 ms | 55.3 ms |

Cached-sprite architecture and sprite count unchanged; no regression. Combined-HUD perf samples passed.

## Cleanup / protection state

- Unity lock: `free` (all three acquisitions released by exact owner + lock id); no Unity/Xephyr process left.
- U1 left clean on `feature/standmap-species-canopy-differentiation-v1`. Editor side effects (`.vscode/settings.json`, SectionFive `.mat`) restored by exact path after every run; patches preserved in the evidence dirs. No staged harness / `.meta` remains in `Assets/ForestPrototype/`. Licence copies deleted from evidence `config/`.
- Host `forest-save.json` untouched (all runs used isolated `XDG_CONFIG_HOME`); sha256 prefix `7effe9b72c741d37` before.
- Frozen Reference Future v1 / Legacy40 not touched. #194 no action. No Windows build.

## Review

Fresh cross-provider read-only review: **PENDING** (parent will dispatch). Verdict vocabulary `APPROVED / NEEDS CHANGES / REJECTED`.
