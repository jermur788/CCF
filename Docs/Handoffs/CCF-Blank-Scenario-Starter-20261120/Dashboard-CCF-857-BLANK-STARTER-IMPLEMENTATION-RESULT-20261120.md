# CCF #857 — blank Unity/3D starter implementation

**IMPLEMENTED / REPAIR AWAITING INDEPENDENT RE-REVIEW / NOT INTEGRATED.** This is an Editor authoring fixture, not a forestry scenario or a player build. The [independent review of `c5e0547a`](https://github.com/jermur788/CCF/blob/32a028c753f877b9d35eebbac2ee0c45640ce246/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-INDEPENDENT-CANDIDATE-REVIEW-20261120.md) returned **NEEDS CHANGES**. This repair changes verification tooling and this report only; all production source, scenes and metadata are byte-identical to the reviewed revision.

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

However, the Unity run changed `prefs` and files under `Unity/*/Editor/Analytics` and `Unity/*/Insights`; those are outside the three save paths. `native-r1/host-side-effect-summary.txt` and `save-prior.json` / `save-restored.json` list the differences. Production learning/help code does contain `PlayerPrefs` writes. Those paths were not identified as reachable in the exercised blank preview, and the harness only queries `HasKey`; the first protocol preserved hashes, not prior preference bytes, so it cannot identify which owner keys changed or attribute every change to Unity housekeeping. No older backup was substituted. No further graphical launch was allowed to see the host CCF directory. A private mount namespace makes disposable witnesses visible at the exact path names and allows comparison of the real host against its post-incident baseline. **This does not retroactively satisfy restoration of the first host directory.**

## Reproduction and evidence

Evidence root `/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120/`. The approved fixture is in `native-r1/historical-v20-fixture.json`; never point a replay at the host config without resolving the incident and obtaining custody approval. Native screenshots use graphical Vulkan Editor `:0`, the Game view at 1280 × 720, and `ScreenCapture.CaptureScreenshotAsTexture`. The harness is temporarily copied under `Assets/ForestPrototype` and removed with its `.meta` after each run.

```sh
e=/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120
runner=Tools/Verification/BlankStarter/run-native.sh
mkdir -p "$e/reproduce/"{negative,determinism}
bash "$runner" exact-shadow 3611728743 "$e/reproduce/negative" "$e/native-r1/historical-v20-fixture.json"
bash "$runner" reference-continuation 3611728743 "$e/reproduce/determinism"
bash "$runner" enlarged80 3611728743 "$e/reproduce/determinism"
python3 Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py --out /tmp/opencode/ccf-857-standmap-offline
```

Exact scope diff from the base: **26 paths total, including all four verification files** (owner’s R-03 clarification supersedes the erroneous three-file count).

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

The R-01 wrapper now stages the preserved v20 fixture and distinct synthetic temporary and backup witnesses before Play. It compares SHA-256, byte counts, modes and nanosecond modification times; its positive control performs `stat` and reads through the same private mount and three `-P` filters. It checks the native PASS marker because the Flatpak launcher can return zero even after a harness assertion. F10 Save and Load buttons are absent in this preview: the verifier tests that omission and exercises the public Save, Load, CanLoad, SaveAndClose and Restart paths directly, rather than claiming to click absent buttons.

## Qualification

Original qualification source: `fac8a909c7028ef2d9e74cca2bc28511bce2f132` (`native-tested-source-head.txt`, `native-tested-sources.sha256`). Pinned-package compile exited 0 (`isolated-final/candidate-pinned-compile.log`). The original `isolation-retry/` run passed 109 checks but did **not** stage existing sidecars, so its clean trace alone did not satisfy R-01. The 1280 × 720 captures are `isolated-final/01-…` through `09-…` plus `slot-*.png` (also in `isolation-retry/`). Both scenes had a surface at y=0 and no world actors. Spawn clearances were 10.000 m and 22.500 m respectively, exceeding the 0.300 m controller radius. Full map bounds were 30.000 × 30.000 m and 45.000 × 60.000 m, with the rectangular side inset 86 logical panel units at 11.467 units/m and rendered rounding within one display pixel.

Repair native revision: `40182dca2e373b2289af4fda0c544b714b3072dd`. `review-repair/repair-tree-identities.txt` records identical `Assets`, `Packages`, `ProjectSettings` and `specs` Git trees compared with `c5e0547a`. Commands above use the unchanged existing deterministic harness entry points, adapted only for the licensed Flatpak instance and isolated config paths. The Reference continuation gate uses the project's explicit Legacy40 override and expects both independent v12 Year-50 → current-schema Year-100 hashes to match. The Enlarged80 gate uses the default new-game geometry with no override; it checks equality across regenerations and uninterrupted/reloaded Year-3 runs. The published historical model-0 Enlarged80 anchors are start `94589C749852F359` and Year 3 `92F9635566BF6AFE`; the Reference continuation comparators are current diagnostic `EBD49ADEF18C3F36` and v15-layout `BFFF66015A6619DA`. Sources: [Rack V2 03C handoff](../../RACK-V2-03C/HANDOFF.md) (Enlarged80) and [MFM-12 integration](../../MFM-12-Integrated-Qualification/HANDOFF.md) (Reference).

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
| AC-09 | NOT TESTED in full | Documented Reference Future deterministic simulation gate **PASS** and Enlarged80 deterministic start hash exact. Unmodified `Enlarged80Verification.Begin` **FAIL** at its v23-only save assertion before the Year-3 anchor; that anchor was **NOT TESTED**. Historical load/render/round-trip (N-12) passed. |
| N-12 | PASS | `historical-final/historical-console.log`: 17 checks, no runtime errors. Legacy40 override gives 64 cells / 336 living trees. Continue loads the v20 Enlarged80 witness with 1,344 mapped trees, five legacy rack routes, year 1, ten work orders and one Annual Review; current-version Save/Load round trip succeeds in the isolated config. |
| AC-10 | NOT TESTED on repaired HEAD | First independent review returned NEEDS CHANGES; the repaired tooling and evidence await re-review. No integration/owner acceptance or Player/Windows build. |
| N-14 | PASS | After populated historical rendering, the verifier disconnects ecology from the same map source in memory; all layers, player/waypoint flags and grid/bounds caches clear. Reopening a blank scene normally creates a separate source; no historical asset was modified for this test. |

**Frozen archive versus simulation:** `ReportFrozen`, the offline FNV-1a check and the archive phase of `ScenarioReferenceVerification.Begin` prove only stored archive integrity (pinned years 0/20/50/100 and schedule; tamper rejected). The separately exercised **continuation** phase of `ScenarioReferenceVerification.Begin` runs the actual current simulation twice from the frozen v12 Year-50 state to Year 100. `review-repair/determinism/reference-continuation-console.log` records `REFERENCE_CONTINUATION_PASS` with equal current-schema hashes `EBD49ADEF18C3F36` and v15-layout comparator `BFFF66015A6619DA`, and `REFERENCE_FUTURE_V1_CONTRACT_PASS`. All four native previews also matched the frozen archive. The existing harness does not assert that the simulated successor matches frozen Year-100 biology; the documented contract excludes that replay. A separate Year-75 resume and complete v1–v24 fixture replay were not exercised by this repair.

**Enlarged80 anchor limit:** `review-repair/determinism/enlarged80-console.log` records regenerated start `94589C749852F359` (exact match) and then `ENLARGED80_VERIFY_FAIL: … an Enlarged80 capture is v23 + geometry 1`. The unchanged gate hardcodes schema 23 at line 273; `ForestSaveData.CurrentVersion` on the candidate is 24. It stops before the deterministic Year-3 comparison at lines 311–323. The older `StandGeometryVerification` similarly hardcodes v23 before its Legacy40 save/determinism assertions; its source was inspected, not rerun here. No gate expectation, schema, geometry, or production source was changed. A correctly versioned Enlarged80 Year-3 gate remains required to claim all of AC-09.

Final checks: the isolated reruns' real host snapshots matched byte-for-byte (`isolation-retry/host-directory-snapshot.json` and `reference-retry/host-directory-snapshot.json`), relative to the *post-incident* state. Known automatic Editor-generated tracked deltas were restored after its last exit (`final-automatic-editor-deltas.diff`); production source hashes matched and no temporary `Assets` harness or `.meta` remained. The live save slot is absent again. Ops/owner must reconcile the original preference/analytics incident before anyone treats the required original directory restore as passed; do not repeat the original launch.
