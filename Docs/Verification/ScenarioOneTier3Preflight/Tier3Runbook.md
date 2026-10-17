# Scenario One Tier-3 execution runbook

Purpose: make the single Tier-3 Unity qualification as close to one-shot as
possible. Every command below was prepared statically at
`971c5f8cc75e571a159a5f38415d0005fdf30ae4`; only the non-Unity portions were
executed. Run the steps in order.

Authority: `integration/scenario-one-tier3-candidate` at the candidate SHA.
This runbook does not change production, thresholds, anchors or acceptance.

---

## 0. Environment constants

Set these once. Substitute the UTC label.

```sh
export T3_CANDIDATE=971c5f8cc75e571a159a5f38415d0005fdf30ae4
export UNITY=/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity
export LIC=/home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses
# Fallback if the Flatpak Hub licence tree is absent (both exist on this host):
# export LIC=/home/jer/.config/unity3d/Unity/licenses
export T3_TS=$(date -u +%Y%m%dT%H%M%SZ)
export EV=/media/jer/ZX20/CCF-Verification/ScenarioOneTier3-$T3_TS
export TMPDIR=/media/jer/ZX20/CCF-Temp/tier3-run
mkdir -p "$EV" "$TMPDIR"
```

**Outputs must stay on the ZX20 SSD.** Keep `TMPDIR` under ZX20 and always pass
explicit `--out "$EV/..."`; several runners default their scratch/output to
`/tmp` (main SSD) when `--out`/`TMPDIR` is omitted.

---

## 1. Full-checkout preflight Git checks (no Unity)

**Operating condition 1 — full checkout required.** Run in the **Tier-3 lane** —
a *full (non-sparse)* checkout at the candidate SHA, not a sparse Non-Unity
worktree. The sparse worktree lacks `Assets/` and yields a false typecheck
NEW_ERRORS cascade and unusable offline/build runners (see
`StaticPreflightFindings.md` §B1).

```sh
cd <full-tier3-lane>
git status --porcelain                 # MUST be empty
git rev-parse HEAD                     # MUST equal $T3_CANDIDATE
git diff c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2 HEAD -- Assets Packages ProjectSettings
                                       # MUST be empty (production invariant)
test -f Assets/Scenes/ForestTest.unity && test -d Assets/ForestPrototype  # full checkout
ls Assets/ForestPrototype/{DensityQualification,DensityProbe,ScenarioHistoryVerification,
    MixedDensityVerification,N3CompletionVerification,ScenarioOneCompletionVerification,
    UxQualification,U4GuidanceSmoke,RngModelPolicyVerification,LinuxPlayerBuilder}.cs \
    2>/dev/null                            # MUST print nothing (no staged harness copy)
find . -maxdepth 2 -name 'mono_crash.*'    # MUST print nothing (no leftover crash blob)
```

If any check fails, **stop**.

---

## 2. External machine-wide Unity lock (required)

**Operating condition 2 — external machine-wide lock required.** The runners
only partially guard concurrency: the base and ThreeSpecies `run_qualification.py`
refuse an Editor *on this project*, the UX runner refuses *any* Editor from this
install, and `build_linux.py` has **no** guard. Hold an **external** machine-wide
lock for the whole sequence and assert no Unity is running before the first
launch.

```sh
export T3_LOCK=/media/jer/ZX20/CCF-Temp/tier3-unity.lock
exec 9>"$T3_LOCK"
flock -n 9 || { echo "STOP: another Tier-3 holder exists"; exit 1; }
pgrep -af "$UNITY" && { echo "STOP: Unity already running"; exit 1; }
```

Keep file descriptor 9 held for every step; release at cleanup (step 14).

---

## 3. Isolated XDG / save / config and explicit licence seeding (required)

**Operating condition 3 — explicit licence seeding required.** Every Unity
runner copies a licence into `<out>/config/unity3d/Unity/licenses` and sets
`XDG_CONFIG_HOME=<out>/config`, so no real EditorPrefs or player save is
touched. Pass `--license-dir "$LIC"` **explicitly** on every runner that
supports it; pre-seed the config where a runner cannot.

- ThreeSpecies and UX `run_qualification.py` support `--license-dir` (default
  `~/.config/unity3d/Unity/licenses`).
- The base `Tools/Verification/Qualification/run_qualification.py` used by
  `run_u4.sh` has **no** `--license-dir`; it relies on a pre-seeded config. Do
  **not** use the base runner in this runbook — use the ThreeSpecies/UX runners
  for every gate.
- `run_unity_gates.py`, `run_model2.py`, `run_regression.py` set
  `XDG_CONFIG_HOME` but never copy a licence. If used, pre-seed the licence
  first. This runbook avoids them except where noted.

---

## 4. Density, pure-Sitka, mixed, save/reload and hashes (batch Editor)

One command per geometry. The default phase set covers density arithmetic,
pure-Sitka compatibility and legacy-route parity, mixed stands, continuity and
save/reload, and PERF/LONGRUN measurement. Both harnesses are staged and always
removed.

```sh
Q3=Tools/Verification/ThreeSpecies/Qualification/run_qualification.py
H1=Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs
H2=Tools/Verification/ThreeSpecies/Qualification/DensityProbe.cs

for geo in 0 1; do
  python3 $Q3 --label density-geo$geo --out "$EV/density/geo$geo" \
    --method DensityQualification.Begin --file $H1 --file $H2 \
    --marker DENSITY_QUAL_PASS --license-dir "$LIC" --timeout 5400 \
    --env CCF_STAND_GEOMETRY=$geo
done
```

- Geometry `0` = Legacy40, `1` = Enlarged80.
- Default phases = `RATIOS,ROLLS,ANCHORS,SITKA100,SITKACOMP,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF,LONGRUN`.
- **Expected per run:** `QUAL_START geometry=L40|E80 ... phases=...`, a
  `QUAL_PHASE_DONE` per phase, and terminal
  `DENSITY_QUAL_PASS geometry=L40|E80 hardChecks=<n> measurementOnly=False ...`.
  Check-bearing phases must report `hardChecks >= 1` (the runner enforces this).
- **Pure-Sitka compatibility** is inside `SITKA100,SITKACOMP,PARITY,CURVE` plus
  the offline `THREE_SPECIES_OFFLINE` pure-Sitka identities and the two isolated
  compiles (step 8). To isolate it explicitly:
  ```sh
  python3 $Q3 --label pure-sitka-geo0 --out "$EV/density/pure-sitka" \
    --method DensityQualification.Begin --file $H1 --file $H2 \
    --marker DENSITY_QUAL_PASS --license-dir "$LIC" --timeout 3600 \
    --env CCF_STAND_GEOMETRY=0 \
    --env CCF_QUAL_PHASES=SITKA100,SITKACOMP,PARITY,CURVE
  ```
- **Hashes:** the `ANCHORS` phase hard-asserts the accepted Legacy40 lifecycle
  anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` for geometry `0`; Enlarged80
  is measured, not an accepted anchor.
- For a pure measurement long-run (no hard checks by design) use
  `--env CCF_QUAL_PHASES=LONGRUN --env CCF_QUAL_SCENARIOS=B,F --env CCF_QUAL_YEARS=300 --env CCF_QUAL_REPLAY=0`,
  then `python3 Tools/Verification/ThreeSpecies/Qualification/max_ba_loss.py "$EV/.../evidence/longrun-E80.csv"`.
  A completed collection alone is not a passing BA-loss analysis.

### Mixed stands (Unity gate)

```sh
python3 $Q3 --label mixed-l40 --out "$EV/mixed/l40" \
  --method MixedDensityVerification.Begin \
  --file Tools/Verification/ThreeSpecies/MixedDensityVerification.cs \
  --marker MIXED_DENSITY_VERIFY_PASS --min-hard-checks 1 \
  --license-dir "$LIC" --timeout 3600 --env CCF_STAND_GEOMETRY=0
```

**Expected:** `MIXED_DENSITY_VERIFY_PASS checks=<n>` (no `MIXED_DENSITY_VERIFY_FAIL`,
no `MIXED_DENSITY_CHECK_FAIL`).

---

## 5. History (Legacy40)

```sh
python3 $Q3 --label history --out "$EV/history" \
  --method ScenarioHistoryVerification.Begin \
  --file Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs \
  --marker SCENARIO_HISTORY_VERIFY_PASS --min-hard-checks 1 \
  --license-dir "$LIC" --timeout 3600 --env CCF_STAND_GEOMETRY=0
```

**Expected:** reaches cycles 2–3, stale orders, reviewed-two-cycle chronology,
history identity after reload, all six marked-target completed orders, and
completion-summary checks, ending
`SCENARIO_HISTORY_VERIFY_PASS assertions=43` (accept `assertions>=43`), exit 0,
clean shutdown.

---

## 6. N3 / completion (Legacy40)

Neither marker carries a check count, so do **not** pass `--min-hard-checks`.

```sh
python3 $Q3 --label n3 --out "$EV/n3" \
  --method N3CompletionVerification.Begin \
  --file Tools/Verification/ScenarioOneCompletion/N3/N3CompletionVerification.cs \
  --marker N3_U4_VERIFY_PASS --license-dir "$LIC" --timeout 3600 \
  --env CCF_STAND_GEOMETRY=0

python3 $Q3 --label completion --out "$EV/completion" \
  --method ScenarioOneCompletionVerification.Begin \
  --file Tools/Verification/ScenarioOneCompletionVerification.cs \
  --marker SCENARIO_ONE_COMPLETION_VERIFY_PASS --license-dir "$LIC" --timeout 3600 \
  --env CCF_STAND_GEOMETRY=0
```

**Expected:** `N3_U4_VERIFY_PASS` and `SCENARIO_ONE_COMPLETION_VERIFY_PASS`
respectively, exit 0, clean shutdown.

---

## 7. UX 1280×720 and F10 (graphical Editor)

**Operating condition 4 — U4 is authoritative for actual 1280×720
verification.** `U4GuidanceSmoke` (7b) hard-asserts that the real Game view is
1280×720; the broad `UxQualification` matrix (7a) does **not**, so 7a alone is
not proof of actual 1280×720. Treat 7b as authoritative and use 7a only as
supporting broad-layout evidence.

```sh
QU=Tools/Verification/ScenarioOneUx/Qualification/run_qualification.py
HU=Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs

# 7a. Broad UX matrix, forced to 1280x720 only
python3 $QU --label ux-1280 --out "$EV/ux" \
  --method UxQualification.Begin --file $HU \
  --marker UX_QUAL_DONE_MARKER --interactive --license-dir "$LIC" --timeout 5400 \
  --env CCF_UI_SIZES=1280x720

# 7b. U4 guidance/completion, hard 1280x720 assertion
python3 $QU --label u4 --out "$EV/u4" \
  --method U4GuidanceSmoke.Begin \
  --file Tools/Verification/U4Guidance/U4GuidanceSmoke.cs \
  --marker U4_GUIDANCE_PASS --interactive --license-dir "$LIC" --timeout 3600 \
  --env CCF_STAND_GEOMETRY=1 --env U4_OUT="$EV/u4/evidence"

# 7c. F10 focused Editor check
python3 $QU --label f10-editor --out "$EV/f10" \
  --method UxQualification.Begin --file $HU \
  --marker UX_QUAL_DONE_MARKER --interactive --license-dir "$LIC" --timeout 1800 \
  --env CCF_UI_SIZES=1280x720 --env CCF_UXQ_PHASES=F10_INPUT
```

**Expected:**
- 7a: `UX_QUAL_DONE shots=<n> hardChecks=<n> unverifiedChecks=<n>`, no
  `UX_QUAL_FAILED`. May be `PASS_WITH_UNVERIFIED` — retain the qualification.
  **Post-check (gap G3):** assert every `$EV/ux/evidence/*-1280x720.png` is
  really 1280×720 and read the `UXQ_MEASURE TEXT_SIZE`/`START` lines
  (`screen=1280x720`); the broad harness does not hard-assert the actual Game
  view size (U4 does).
- 7b: `U4_NOTE GAME_VIEW actual=1280x720 requested=1280x720`, every screenshot
  1280×720, `U4_GUIDANCE_PASS checks=113 failed=0 screen=1280x720`, exit 0.
  `V1_world_hash_Y25_OBSERVED` is a **measurement only** — final adjudication is
  Tier-3, it is not a new expected anchor.
- 7c: `UXQ_UNVERIFIED` for synthetic F10 → runner status
  `PASS_WITH_UNVERIFIED`, exit 0. Synthetic Editor F10 delivery is **not** proof
  of native/Player input; native F10 stays UNVERIFIED (see §11).

---

## 8. Non-Unity static suites (run before or beside Unity; no Editor)

These are the Tier-3 static gates and runner self-tests. All commands write
outside the repository (`$EV`/`TMPDIR`).

```sh
S=$EV/static; mkdir -p $S
TMPDIR=$TMPDIR python3 -B -m unittest discover -s Tools/Verification/Qualification -p 'test_*.py' -v
python3 Tools/Verification/ScenarioOneUx/Qualification/run_f10_self_test.py        --out $S/f10
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all --out $S/offline
python3 Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py              --out $S/n3
python3 Tools/Verification/ScenarioOneUx/run_ux_check.py                           --out $S/ux
python3 Tools/Verification/ThreeSpecies/OfflineLogic/check_fixture_controls.py     --out $S/fixture-controls
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod             --out $S/typecheck
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools            --out $S/typecheck
python3 Tools/Build/S1B/test_build_linux.py
python3 Tools/Calibration/OakBeechGrowth/check_integrated_v1.py --out $S/growth-integrated.json
python3 Tools/Verification/SiteInfrastructure/run_site_check.py --out $S/site
python3 Tools/Verification/ScenarioOneCompletion/P2/OfflineLogic/run_offline_check.py \
  --unity "$UNITY" --out $S/p2
python3 Tools/Verification/ScenarioOneCompletion/P3/OfflineLogic/run_offline_check.py \
  --unity "$UNITY" --out $S/p3
git diff --check
```

Expected results are in `StaticPreflightFindings.md` and
`PreflightManifest.json`.

**EXCLUDED from Tier-3 (must not be used):** `check_sources.py` (stale
candidate-v1 ownership — fails on `MixedDensityModel.Composite.cs`),
`check_production_v1.py` (retired; superseded by `check_integrated_v1.py`) and
`with_demo_hdr_quarantined.py` (wrong worktree `/home/jer/CCF-s1b`). Note the
reduced-model composite density result is **3/107 failures adjudicated as an
accepted known C-level limitation** (values 0.423 / 0.407 / 0.417 vs the
unchanged 0.40 criterion); it stays a non-pass in evidence.

---

## 9. Model-2 new-game verification

Manager-reported state: `feature/scenario-one-model2-default` is at **`40b2419`**
(`feat: start new Scenario One games on growth model 2`; production =
`ScenarioOneManager.NewGameGrowthModel = GrowthModel.SpeciesAware`, plus two
comment updates). The focused Unity gate passed, but the implementation
**remains pending independent review / re-pin**, so this is not yet an accepted
Tier-3 input. Primary gate:

```sh
python3 $Q3 --label model-default --out "$EV/model2/default" \
  --method ModelDefaultVerification.Begin \
  --file Tools/Verification/ModelDefault/ModelDefaultVerification.cs \
  --marker MODEL_DEFAULT_VERIFY_PASS --min-hard-checks 1 \
  --license-dir "$LIC" --timeout 3600 --env CCF_STAND_GEOMETRY=0
```

**Expected:** `MODEL_DEFAULT_VERIFY_PASS checks=<n>` (new game on growth model 2,
`NEW_GAME_CONSTANT_IS_SPECIES_AWARE`, disk-reload retains model 2, pure-Sitka
still routes as model 1). The RNG-policy gate remains relevant for the rng/regen
part of the new-game default:

```sh
python3 $Q3 --label rng-policy --out "$EV/model2/rng" \
  --method RngModelPolicyVerification.Begin \
  --file Tools/Verification/RngModelPolicyVerification.cs \
  --marker RNG_MODEL_POLICY_VERIFY_PASS --license-dir "$LIC" --timeout 3600 \
  --env CCF_STAND_GEOMETRY=0
```

Also re-run §4–§6 on the re-pinned model-2 branch, because new-game
hashes/anchors change with the new default. **Linux Player build/smoke waits for
the final re-pinned candidate** (§11).

---

## 10. Save/reload

- In-Editor: `SAVELOAD` phase inside §4 (in-memory capture/load plus one disk
  round trip into the isolated config folder).
- In-Player: §11 Linux Player smoke runs the save phase then a separate load
  process against the same isolated `XDG_CONFIG_HOME`, comparing
  `saved-world-hash.txt`.

---

## 11. Linux Player build + smoke

```sh
# The build needs the locally acquired Asset Store pack "Ultimate Nature - Starter" (gitignored, never committed);
# pass its folder explicitly (see Tools/Build/S1B/README-linux.md), a sequence of the form L<nn>/S<nn>, and a
# socket-capable TMPDIR (tmpfs, not the exFAT ZX20 drive) so the Unity licensing client can start.
export TMPDIR=/run/user/$(id -u)/ccf-build; mkdir -p "$TMPDIR"
python3 Tools/Build/S1B/build_linux.py --smoke \
  --unity "$UNITY" --license-dir "$LIC" --art-pack "$CCF_ART_PACK_DIR" --date $(date -u +%Y%m%d) --sequence L01
# -> /media/jer/ZX20/CCF-Builds/S1B/CCF-S1-<date>-L01-<sha7>-SMOKE-Linux-x64/

python3 Tools/Build/S1B/run_linux_player_smoke.py \
  --player /media/jer/ZX20/CCF-Builds/S1B/CCF-S1-<date>-L01-<sha7>-SMOKE-Linux-x64/CCF.x86_64 \
  --display "${DISPLAY:-:0}" \
  --evidence-root /media/jer/ZX20/CCF-Verification
```

**Expected:** `S1B_LINUX_BUILD_PASS`, `build-identity.json`,
`package-manifest.json`, then `S1B_PLAYER_SAVE_PHASE_PASS hash=...`,
`S1B_PLAYER_LOAD_RESET_PHASE_PASS`, `smoke-summary.json` `overall=PASS`.
Requires a DISPLAY and a clean committed source tree; no Editor guard exists, so
the §2 lock is mandatory.

---

## 12. Evidence collection (manifest inputs)

Collect under `$EV`:

- every `<label>.log`, `results-<label>.json`, `<label>.markers.txt`;
- density/perf/longrun CSVs under `$EV/density/*/evidence/`;
- U4/UX screenshots and `"$EV/u4/evidence"` notes;
- `package-manifest.json`, `build-identity.json`, `build-result.txt`,
  `included-assets.txt`, `smoke-summary.json`, `saved-world-hash.txt`;
- `$S/*` static-suite outputs and `git diff --check` output;
- `PreflightManifest.json` (tool SHA-256 list) plus a final SHA-256 inventory of
  every collected artifact.

Record for each run: candidate SHA, Unity `6000.6.0f1`, geometry, phases,
marker, hard-check count, exit code and `process_outcome`.

---

## 13. Shutdown / crash classification

The runners distinguish:

- `PASS_CLEAN_SHUTDOWN` — valid verdict, exit 0, no unexpected tree change.
- `VERDICT_PASS_POST_SHUTDOWN_CRASH` — valid verdict **followed by** the known
  `ShutdownControllerHIDAPI` / `Application::CoreShutdown` SIGSEGV, raw exit
  255/139/−11, and only root-level untracked `mono_crash.*` artifacts. The
  runner still reports overall **FAIL** and exits non-zero.
- Otherwise `PROCESS_OR_GATE_FAILURE`.

A single non-reproducible post-verdict shutdown crash is annotated, not waived;
a **reproducible** shutdown crash requires investigation. Preserve raw logs and
crash blobs; never delete evidence to make a run green.

---

## 14. Cleanup

```sh
cd <full-tier3-lane>
# runners already remove their staged .cs and .meta; confirm:
git status --porcelain                 # MUST be empty again (except known rewrites)
find . -type d -name __pycache__ -prune -exec rm -rf {} +
# move any mono_crash.* into $EV/crash-artifacts/ (do not leave in the lane)
exec 9>&-                              # release the Tier-3 lock
```

The runners treat `.vscode/settings.json` and
`Assets/ForestPrototype/Art/SectionFive/*.mat` re-serialisation as recorded
known rewrites (not failures). Any other new working-tree entry fails the run.

---

## Ordered execution summary

1. Preflight Git checks (§1) → 2. machine-wide lock (§2) → 3. static suites
(§8) → 4. density both geometries (§4) → 5. mixed (§4) → 6. history (§5) →
7. N3/completion (§6) → 8. UX/U4/F10 (§7) → 9. model-2 when available (§9) →
10. Linux Player build/smoke (§11) → 11. evidence manifest (§12) →
12. shutdown classification (§13) → 13. cleanup (§14).
