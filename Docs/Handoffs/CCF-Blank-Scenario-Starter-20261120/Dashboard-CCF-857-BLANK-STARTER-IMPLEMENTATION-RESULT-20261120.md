# CCF #857 — blank Unity/3D starter implementation

**IMPLEMENTED / NOT INDEPENDENTLY REVIEWED / NOT INTEGRATED.** This is an Editor authoring fixture, not a forestry scenario or a player build.

## Scope and custody

- Worker: OpenCode `openai/gpt-6.2-sol`, session `ses_e07995150ffeJJDYGO8Usc2FTJ`, `jer-laptop`.
- Branch `feat/ccf-857-blank-starter-20261120-v1`, base/context `cbcb3850c3952a8d74e54606279165b2f4be9add`, worktree `/media/jer/ZX20/CCF-Sen1-857-BlankStarter-20261120`, lane `Sen1-857-BlankStarter`. Live `main` at preflight was `cba0a905b766ce8fc0356d351ac5bff1c2a9a008`, one coordination-only delta. No merge or rebase.
- [Authorisation receipt](https://github.com/jermur788/CCF/issues/857#issuecomment-7036660288). Approved bytes (separate receipt; frozen DRAFT banners unchanged) `23da9850a7245a8623244ff2e2e76fbf46c33f0c`; verified all four with `gh api … --jq .content | base64 -d` and `git hash-object`: `d93d00d71bc9d1835723804eb15152f90cfa18a7`, `445cd03720eb94aef82821b5407cf789f8ca0ed9`, `4bb84c8dd55fdd0ef3b84c26715067266b19cad8`, `846380b1c5b8acb5e696b7815927ba50a8a5ee6f`. Original blocker `blk-7c3c05c913d8de26a` is recorded at [handoff-only `ebbd33fa`](https://github.com/jermur788/CCF/commit/ebbd33fa0d47b3b9949760da44da168f269a47a5); the continuation explicitly grants `ForestPlayer.cs` for the cached preview IMGUI guard only.
- The original lease `f736db4d50914aafbf82f920d707ef8c` was released normally after the licence smoke. The clean amended attempt acquired `860e848a47914b06903e85a2bb03cb9b` at `ebbd33fa`, task `CCF-857-BLANK-STARTER`, owner `openai/gpt-6.2-sol (#857 implementation, jer-laptop)`, ETA `2027-12-31T23:59:59+00:00`. The same ID was released normally at approximately `2026-11-20T10:44:43Z`, `forced=false`, lock free with no Editor PIDs (`attempt2-lease-*.json`, `attempt2-lease-after.txt`).
- Unity `6000.6.0f1`, Flatpak Hub `3611728743`. Empty-project licence smoke exited 0 after entitlement and licence update (access-token refresh reported unavailable separately). The first cold import automatically changed Linux SDK/sysroot/toolchain pins 1.1.0 → 1.1.1. The worker restored the exact generated changes and ran with `UNITY_DISABLE_LINUX_TOOLCHAIN_MIGRATOR=1` and `UNITY_DISABLE_LINUX_AUTO_TOOLCHAIN_INSTALLATION=1`. Subsequent graphical Editor runs also generated VS Code settings and seven URP material normalization deltas (Bilberry and Woodland Forbs); none belongs to the candidate. The test rendered those existing resources after Unity normalized them. No package, ProjectSettings, authored art or `.vscode` change is committed.

## What changed

- The two scenes contain only the preview config/UI/save controller, a level ground cuboid, the shared player/camera, and lighting. On Play the scene-local config validates finite dimensions and spawn clearance, sets the physical ground and spawn, and supplies the three optional text slots. The test-only scene changes its width/depth and name, not a line of shared C#.
- The existing views render exact empty messages without manager, ecology, or persisted lessons. A reduced UI lifecycle allows navigation and locks walking only while a modal is open. The shared player still owns walking, looking, physics and inspection; the amended `ForestPlayer` change only suppresses IMGUI using a cached preview reference. Both scenes serialize an empty planting-species list, so pressing the legacy key cannot enter planting; the blank scene supplies no management/planting authority.
- The shared map source clears every world layer and uses collider bounds. Stand Map and minimap apply the same north-up transform at different panel sizes. `max(width, depth)` fixes a single scale and the shorter side is centered, without drawing fake cells or changing the historical renderer or ecological geometry. The existing plain-.NET square/map regression is 176/176 PASS.
- Startup, F10, Save/Load/CanLoad, F5/F9, SaveAndClose and Restart use or reach one fail-closed scene predicate. There is no save destination, persisted ID, schema change, or scenario registry. Native Editor Play can report an index for an unlisted open scene (this run reported `1`, not `−1`), so the preview marker, not an index assumption, is the primary restart guard; negative indices also refuse.

## Historical save witness — read before further qualification

**The real host directory was not restored byte-for-byte.** The first exact-path run staged the authorised v20 copy at the initially absent slot, with `strace -f -P` for `.json`, `.tmp`, `.bak`. Startup `RefreshContinueState` visibly refused and the trace was empty up to an early harness assertion. The staged file was removed and the three historical save paths plus `save-backup-20261104/forest-save.json` match their prior contents/presence and recorded mtimes. The fixture SHA-256 was `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075`; the pre-existing `.bak` was `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3`.

However, Unity updated `prefs` and files under `Unity/*/Editor/Analytics` and `Unity/*/Insights` despite the guard; those are outside the three save paths. `native-r1/host-side-effect-summary.txt` and `save-prior.json` / `save-restored.json` list the differences. The first protocol recorded their hashes, not all their bytes; this worker did not substitute an older backup or claim to recover the original bytes. No further graphical launch was allowed to see the host CCF directory. A private mount namespace makes a preserved witness copy visible at the exact required path names for the safe retry and allows exact post-incident host before/after comparison. **This does not retroactively satisfy restoration of the first host directory.**

## Reproduction and evidence

Evidence root `/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120/`. The approved fixture is in `native-r1/historical-v20-fixture.json`; never point a replay at the host config without resolving the incident and obtaining custody approval. Native screenshots use graphical Vulkan Editor `:0`, the Game view at 1280 × 720, and `ScreenCapture.CaptureScreenshotAsTexture`. The harness is temporarily copied under `Assets/ForestPrototype` and removed with its `.meta` after each run.

```sh
e=/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120
runner=Tools/Verification/BlankStarter/run-native.sh
bash "$runner" compile 3611728743 "$e/isolated-final"
bash "$runner" exact-shadow 3611728743 "$e/isolation-retry" "$e/native-r1/historical-v20-fixture.json"
bash "$runner" historical 3611728743 "$e/historical-final" "$e/native-r1/historical-v20-fixture.json"
mkdir -p "$e/reference-retry/reference-config"
bash "$runner" reference 3611728743 "$e/reference-retry"
python3 Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py --out /tmp/opencode/ccf-857-standmap-offline
```

Exact scope diff from the base:

```text
A Assets/ForestPrototype/BlankStarter.meta
A Assets/ForestPrototype/BlankStarter/BlankStarterPreview.cs
A Assets/ForestPrototype/BlankStarter/BlankStarterPreview.cs.meta
M Assets/ForestPrototype/ForestPlayer.cs
M Assets/ForestPrototype/ForestSaveController.cs
M Assets/ForestPrototype/UI/AnnualReviewView.cs
M Assets/ForestPrototype/UI/HudMinimapView.cs
M Assets/ForestPrototype/UI/LearningObjectivesView.cs
M Assets/ForestPrototype/UI/Map/StandMapModel.cs
M Assets/ForestPrototype/UI/Map/StandMapSource.cs
M Assets/ForestPrototype/UI/Map/StandMapTransform.cs
M Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs
M Assets/ForestPrototype/UI/StandMapView.cs
M Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs
M Assets/ForestPrototype/UI/StandaloneSessionMenu.cs
M Assets/ForestPrototype/UI/WalkingHudView.cs
M Assets/ForestPrototype/UI/WorkPlanView.cs
A Assets/Scenes/BlankStarter.unity
A Assets/Scenes/BlankStarter.unity.meta
A Assets/Scenes/BlankStarter45x60.unity
A Assets/Scenes/BlankStarter45x60.unity.meta
A Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-IMPLEMENTATION-RESULT-20261120.md
A Tools/Verification/BlankStarter/BlankStarterQualification.cs
A Tools/Verification/BlankStarter/run-native.sh
A Tools/Verification/BlankStarter/save_witness.py
A Tools/Verification/BlankStarter/shadow-save-dir.py
```

The exact-path instrumentation was separately challenged with three disposable sentinel files; `exact-trace-positive-control.strace` contains all three expected `openat` and `read` records at the literal historical path names. The run wrapper compares the shadow slot to the preserved fixture and tests sidecar absence after Play. It checks the native PASS marker because the Flatpak launcher can return zero even after a harness assertion.

## Qualification

Tested source and verification tooling: `fac8a909c7028ef2d9e74cca2bc28511bce2f132` (`native-tested-source-head.txt`, `native-tested-sources.sha256`); this handoff update does not change the candidate bytes. Pinned-package compile exited 0 (`isolated-final/candidate-pinned-compile.log`). Graphical blank run: `isolation-retry/exact-shadow-console.log` reports **109 checks passed, no runtime errors**, with a **zero-byte** `isolation-retry/exact-shadow-save-access.strace`, unchanged witness bytes and no sidecars. The preceding same-candidate trace (`isolated-final/`) contained two `???( <detached ...>` records, not named file operations, so it was not treated as a clean trace. The 1280 × 720 captures are `isolated-final/01-…` through `09-…` plus `slot-*.png` (also in `isolation-retry/`). Both scenes had a surface at y=0 and no world actors. Spawn clearances were 10.000 m and 22.500 m respectively, exceeding the 0.300 m controller radius. Full map bounds were 30.000 × 30.000 m and 45.000 × 60.000 m, with the rectangular side inset 86 logical panel units at 11.467 units/m and rendered rounding within one display pixel.

| Criterion | Result | Evidence / limit |
|---|---|---|
| AC-01 / N-13 | PASS | Import, true bounds, walking/look and invalid input rejection on both scene configurations. |
| AC-02 / N-01 | PASS | Shared rendered UI and four headings; no phantom tree, no fake inventory, no cloned UI. |
| AC-03 / N-02 | PASS | All seven layers empty, no editing or rack help/tool/markers; square map regression 176/176. Same-source transition reported separately below. |
| AC-04 / N-03 | PASS | Tab state, exact text, safe buttons and direct refusal; G while aiming at ground did not enter planting. |
| AC-05 / N-04 | PASS | Exact review and Objectives strings; no older years, teaching state or acknowledgement even with a rack preference. |
| AC-06 / N-05–N-08 | PASS in exact-path shadow; NOT TESTED to completion against the real directory | Startup Continue/RefreshContinueState, F10 with absent Save/Load actions, public SaveAndClose/Restart/Save/Load/CanLoad and F5/F9; wrapper and telemetry evidence above. |
| AC-06 / N-09 | FAIL for full host-directory restoration | Save files and sidecars preserved, but original real-path run rewrote nonsave preference/analytics files. No original bytes substituted. Full qualification is held on this incident. |
| AC-07 / N-10 | PASS | The original and copied scenes share GUIDs; the copy walks and renders the true rectangle, with no stored geometry change. |
| AC-08 / N-11 | PASS | Input → observed label recorded for intro, objectives and guidance at empty, distinctive, cleared states; banner always visible. |
| AC-09 / N-12 | PASS for targeted historical regression | `historical-final/historical-console.log`: 17 checks, no runtime errors. Existing Legacy40 override gives 64 cells / 336 living trees. Real Continue loads the v20 Enlarged80 witness with 1,344 mapped trees, five legacy rack routes, year 1, ten work orders and one Annual Review; current-version Save/Load round trip succeeds only in the isolated config. Rendered map / Work Plan / review in `historical-final/10-…` through `12-…`. No exhaustive version-by-version or long-run test. |
| AC-10 | NOT TESTED — independent review not yet performed | Author scoped diff and unlisted scene test passed. No integration/owner acceptance or Player/Windows build. |
| N-14 | PASS | After populated historical rendering, the verifier disconnects ecology from the same map source in memory; all layers, player/waypoint flags and grid/bounds caches clear. Reopening a blank scene normally creates a separate source; no historical asset was modified for this test. |

Frozen reference scope: all four native milestones were admitted by `ScenarioReferenceArchive.Load` in the historical run. The unchanged archive also matched all four embedded world hashes and the separate schedule with the offline FNV-1a check; it rejected an in-memory tamper (`frozen-archive-offline.log`). The first attempt to run the existing Unity `ReportFrozen` entry exited 139 after licensing and before invoking the method; a same-source rerun with its isolated config directory pre-created exited 0 and reported `REFERENCE_ARCHIVE_INTEGRITY_PASS` and `REFERENCE_ARCHIVE_NEGATIVE_CONTROL_PASS` (`reference-retry/reference-console.log`). Full long-horizon deterministic simulation, complete v1–v24 fixture replay and review/integration/owner acceptance are **NOT TESTED**.

Final checks: the isolated reruns' real host snapshots matched byte-for-byte (`isolation-retry/host-directory-snapshot.json` and `reference-retry/host-directory-snapshot.json`), relative to the *post-incident* state. Known automatic Editor-generated tracked deltas were restored after its last exit (`final-automatic-editor-deltas.diff`); production source hashes matched and no temporary `Assets` harness or `.meta` remained. The live save slot is absent again. Ops/owner must reconcile the original preference/analytics incident before anyone treats the required original directory restore as passed; do not repeat the original launch.
