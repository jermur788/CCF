# MFM-02 — Rack-Served Sample and Coverage Geometry Adapter

**Task:** `MFM-02`
**Feature:** Multi-Scale Forestry Management V1 (D-065)
**Packet:** `Docs/Handoffs/MFM-Implementation-02/TASK-PACKET.md`
**Claim:** GitHub issue #22
**Verdict:** **APPROVE** for MFM-02 only
**Exit target:** `RACK-SERVED SAMPLE GEOMETRY QUALIFIED — READY FOR MANAGER REVIEW`

## Worker / provider

| Phase | Worker, model and provider |
|---|---|
| Production implementation and initial qualification | Claude Code (T3 Code harness), Claude Sonnet 5.5 (`claude-sonnet-5-5`) |
| Continuation, final Unity qualification and publication | OpenCode, `deepseek-v4.1-flash` (`opencode-go/deepseek-v4.1-flash`) |
| Final Unity lock owner | `OpenCode-deepseek-v4.1-flash-MFM02` |

Neither run was the GPT-6.1 Sol target named in the issue. The actual models are recorded above.

## Identity

| Item | Exact value |
|---|---|
| Starting `main` at claim | `c9ab5a6bc8f0286b72cd9911434ebcdc3744f8c0` |
| Rebased base before qualification | `81a037e35126bbc256fcda076019af47e8e0954f` |
| Production commit | `f632d7a1a79616dde32b347b28ab897dbf24b0bf` |
| Final Unity-tested HEAD | `8f9be4f64641cfb5cf82ad148ea9d18f2a4f454b` |
| Branch | `feature/mfm-rack-sample-geometry-v1` |
| Worktree | `/media/jer/ZX20/CCF-MFM02` |
| Unity lane | `/media/jer/ZX20/CCF-U1`, restored to `840475103c33be9f168e55078c1ef56b7530d049` |
| `origin/main` at publication | `d72beb877f2c517b240e415bb56e4b5ca2fe285c` (docs/spec-only ahead; branch not rebased after test) |
| Evidence root | `/media/jer/Files/CCF/Verification/MFM02-RackSampleGeometry-20261027T202415Z` |

`8f9be4f` is a verification-only commit on top of production commit `f632d7a`. Its production bytes are identical to `f632d7a`; `f632d7a..8f9be4f` changes only the model-0 gate and its README.

## Production files changed

| File | Change |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs` | `TeachingHalfWidthM = 9f`; `FutureTreeSpacingM` is now its own literal `8f`, not an alias of `HegyiCutoffMeters`; `Analysis.TeachingRackId`; `Corridor`; `Frame.Corridors`, `Frame.Anchored`, `Frame.InCorridor`; V2 `Contains` excludes actual confirmed corridors; `AnalyseTeachingRack` and shared `Evaluate`. Model-0 `Analyse` geometry and authored-corridor exclusion are unchanged. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBayReader.cs` | New `Read(trees, orders, ScenarioOneRackSampleGeometry.Result)` overload; an invalid/absent sample returns no area. Existing authored-layout `Read` unchanged. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs` | New plain deterministic helper (plus `.meta`): five-rack ordering, third-rack teaching sample, closest-point anchor, half-distance coverage units, explicit outer remainders, point classification and exact `Signature()`. |
| `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs` | V2 branch gated on `RackPlanningV2Active` and `RackPlanConfirmed`; derives the sample from `ConfirmedRacks` and the Enlarged80 half-extent. No authored-line fallback when confirmation is absent. Model-0 path unchanged. |

Verification additions: `Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs`, its `README.md`, `Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackSampleGeometryOfflineCheck.cs`, `NonUiOfflineMain.cs`, `run_nonui_offline.py`, and the existing non-UI README. No Rack V2 map/editor UI was edited.

Production commit stat: 11 files, +1559/−14. Final verification commit stat: 2 files, +9/−3.

## Geometry API and contract

### Types added

- `ScenarioOneRackSampleGeometry.Result`, `RackInput`, `OrderedRack`, `CoverageUnit`, `OuterRemainder`, `Placement`, `PlacementKind`, `Side`.
- `Derive(IEnumerable<RackPlanRack> racks, float standHalfExtentM)` — live confirmed records.
- `Derive(IReadOnlyList<RackInput> racks, float standHalfExtentM)` — plain comparison entry.
- `Result.Classify(x, z)` — `RackCorridor`, `CoverageUnit`, `OuterRemainder` or `OutsideStand`; a corridor wins over the strip containing it, and outside the stand wins over everything.
- `Result.Signature()` — exact text of all derived values for shuffled/repeated comparison.
- `ScenarioOneRackBay.Corridor`, `Frame.Anchored`, `Frame.Corridors`, `Frame.InCorridor`, `Analysis.TeachingRackId`, `AnalyseTeachingRack`.
- `ScenarioOneRackBayReader.Read(..., ScenarioOneRackSampleGeometry.Result)`.

### Deterministic ordering

- Requires exactly five racks (`TeachingRackCount = 5`).
- Common axis is the length-weighted mean **direction** using doubled-angle averaging, so entrance/far orientation cannot flip it. Common normal is the axis turned 90° left.
- Racks are sorted by the midpoint offset along the normal, rounded to the nearest millimetre, then by ascending rack ID.
- Crossing/interleaved racks are refused; the derivation is invalid rather than arbitrarily ordered.
- Teaching rack is index 2 (the third). Shuffled input and a second plan with IDs `6..10` both selected the same physical rack.

### Teaching sample

- Anchor is the closest point on the third rack's centreline to stand centre `(0,0)`, clamped along the rack to `[12, length − 12]`.
- Frame is anchored at that point, runs along the rack direction, uses fixed `TeachingHalfWidthM = 9.0 m` on each side and `HalfLengthM = 12.0 m` along.
- All confirmed rack corridors are excluded using their actual width; the footprint does not depend on neighbour spacing.
- V2 is used only when `RackPlanningV2Active` **and** `RackPlanConfirmed`. A draft, reopened or invalid plan produces no teaching area; it never silently rebinds to an authored line or another rack. `OPEN-3`, `ReopenRackPlan`, completed-work locking and Rack V2 persistence were not changed.
- Model-0/Legacy40 keeps the authored-layout path: middle authored line, `HalfWidth` from the nearest authored gap, `DefaultHalfWidthM = 7.6 m` fallback, authored-corridor exclusion and `TeachingRackId = 0`.

### Rack-served coverage

- Units are full-length strips between halfway boundaries of neighbouring rack-centreline offsets.
- The outermost unit mirrors the adjacent half-gap on its outside edge; it is flagged as an outer edge.
- Left and right outer remainders are always returned as explicit `OuterRemainder` data (side, strip edge, stand limit, area, polygon), including zero-area cases.
- Strips are clipped to the square stand. No coverage/proposal/treatment decision policy is introduced.

### Parameter separation

- `ScenarioOneRackBay.FutureTreeSpacingM = 8f` is now a teaching literal and is no longer assigned from `ForestEcologyController.HegyiCutoffMeters`.
- The Hegyi 8 m neighbourhood and the teaching 8 m Crop Tree spacing are separate constants with the same accepted value; neither value changed.

## Offline verification

Evidence: `offline-candidate/run.log`, `offline-mutations.txt`, `runtime-compile/candidate-rebased/`.

| Check | Result |
|---|---|
| `NONUI_RACKSAMPLE_PASS` | **PASS, checks=75** |
| `NONUI_KEYBOARD_PASS` | **PASS, checks=46** |
| `NONUI_RACKBAY_PASS` | **PASS, checks=87** |
| `NONUI_APPROACH_PASS` | **PASS, checks=49** |
| Candidate log totals | 257 `PASS`, 0 `FAIL` |
| Injected mutations caught | **10/10** |
| Runtime type-compile | **169 sources, 0 errors, 0 warnings** (`dotnet build` rc=0) |

The ten caught mutations were: 9 m width changed to 8 m; second rack selected; midpoint anchor; descending ID tie-break; teaching 8 m aliased to Hegyi; non-halfway coverage boundaries; scalar corridor instead of actual rack width; missing anchor clamp; Sequence V2 branch using authored routes; outer remainder silently assigned. Full failure lines are in `offline-mutations.txt`.

The mutation base matches the production worktree byte-for-byte. No save schema, prescription, proposal, candidate, mark, order or coverage-map state is introduced.

## Unity qualification

Machine-wide lock and lane queue were obeyed. PARITY-03/claim #10 and PARITY-04/claim #27 were already released before MFM-02 acquired the lock; MFM-02 did not bypass another owner.

### Lock receipts and attempts

| Lock | Window (UTC) | Outcome |
|---|---|---|
| `03c5e45a04c34a87a2c60c7de33c0eaa` | 20:57:44–21:23:03 | v2 attempt 1 timed out at 1500 s. Cold initial import took 1266 s; 110 checks passed, no `MFM02_V2_FAIL`, no `error CS`. Not counted as a pass. Lane restored; lock released normally. |
| `ae96aecef5d24686b4bc1281d6e33253` | 21:30:47–22:19:47 | v2 retry PASS, 112 checks. The retry had been blocked by a Unity `Scene Backup Detected` modal left by the killed first Editor; the modal was cleared. The next gate (model-0) then failed only on the copied historical 8.00385 m scene fixture; chain stopped. Lane restored; lock released normally. |
| `391797c8192c448ca6571521c8b420a5` | 22:44:48–22:58:17 | **Final clean chain on `8f9be4f`: all four gates PASS.** Owner `OpenCode-deepseek-v4.1-flash-MFM02`; ETA 2026-10-28T00:14:46Z. Lane restored to `8404751`; lock released; status `free`. |

Final lock receipt: `/media/jer/Files/CCF/Verification/MFM02-RackSampleGeometry-20261027T202415Z/unity-lock-acquire.txt`, `unity-lock-release.txt`, `unity-lock-status-after.json`.

### Final gate results on `8f9be4f`

| Gate | Result | Seconds | Evidence |
|---|---|---|---|
| `unity-v2` | **PASS**, `MFM02_V2_PASS checks=112` | 330.13 | `unity-v2/result.json` |
| `unity-model0` | **PASS**, `MFM02_MODEL0_PASS checks=38` | 274.31 | `unity-model0/result.json` |
| `unity-legacy40` | **PASS**, `MFM02_LEGACY40_PASS checks=12` | 73.32 | `unity-legacy40/result.json` |
| `unity-reference` | **PASS**, `REFERENCE_FUTURE_V1_PASS`, contract pass, both anchors | 106.75 | `unity-reference/result.json` |

No gate log contains `error CS`. Import, script compilation and the disposable harnesses completed cleanly. Each gate's known Editor side effects were restored and the lane was clean after every gate. No Player build or native-input run was performed.

## Live V2 measurements

Five confirmed racks, all east/west, width 2.4 m, length 75.4 m:

| ID | Entrance (x,z) | Far (x,z) | Angle | Width | Length |
|---|---|---|---|---|---|
| 1 | (-35.4000015, -35.2711754) | (40, -35.2711754) | 0° | 2.4 m | 75.4 m |
| 2 | (-35.4000015, -16.4217644) | (40, -16.4217644) | 0° | 2.4 m | 75.4 m |
| 3 | (-35.4000015, 0.2382353) | (40, 0.2382353) | 0° | 2.4 m | 75.4 m |
| 4 | (-35.4000015, 15.7935295) | (40, 15.7935295) | 0° | 2.4 m | 75.4 m |
| 5 | (-35.4000015, 35.2323532) | (40, 35.2323532) | 0° | 2.4 m | 75.4 m |

Selected teaching rack: **rack 3**. Anchor: **`(0, 0.2382353)`**, the unclamped closest point to `(0,0)`, 35.4 m from the entrance and within the allowed `[12, 63.4] m`; the ±12 m footprint is on the confirmed segment. Half-width **9.0 m**, half-length **12.0 m**.

| Measurement | Value |
|---|---|
| Footprint trees before access | **75** |
| Footprint basal area before access | **1.4417530703 m²** |
| Corridor-excluded trees in the band | 10 |
| Trees between the historical 8.00385 m and the new 9.0 m edge | 11 |
| Living trees outside 9 m but within 10 m | 8 |
| Footprint tree-set SHA | `c2f2fd5418794915` |
| After save/load and re-confirm | identical rack 3, anchor, 75 trees, 1.4417530703 m², same SHA |
| After access (one growth step) | 75 trees, 1.5170994824 m² |
| Plan 2 (racks 6–10, permuted order) | selected rack 6 at z 0.2382353; same anchor, 75 trees, 1.4417530703 m², same SHA |

Historical model-0 reference: **69 trees, 1.2949037088 m²** before access, half-width 8.00385 m on the old scene fixture. The qualified V2 footprint is **+6 trees and +0.1468493615 m² (+11.34% before access)** at the fixed 9.0 m width. This is a new measured V2 fixture, recorded with provenance; no protected target was rebaselined.

Coverage (stand 80 × 80 m; total **6400.00024 m²**):

| Unit | Offset interval (m) | Area (m²) |
|---|---|---|
| rack 1 | [-44.69588, -25.84647] | 1132.28247 |
| rack 2 | [-25.84647, -8.09176] | 1420.37646 |
| rack 3 | [-8.09176, 8.01588] | 1288.61182 |
| rack 4 | [8.01588, 25.51294] | 1399.76477 |
| rack 5 | [25.51294, 44.95177] | 1158.96472 |

Right remainder: edge −44.69588, **explicit**, area 0. Left remainder: edge 44.95177, **explicit**, area 0. The two remainders are reported even when empty.

Lessons 2–5 live chain: Crop Trees `P0810@(0.11824,-3.59235)`, `P1206@(-8.40412,4.06529)`, `P1214@(7.79706,3.90294)` at the separate 8 m teaching spacing. Initial release: 18 removals, BA share 0.1789843, minimum share 0.3103536. Executed release: 18 removals, BA share 0.1713693, minimum executed 0.3042529. The accepted release rule and 40% cumulative safeguard checks passed; the complete 112-check chain includes Work Plan, Annual Review, J/K return review, completion and reload.

## Model-0 / Legacy40 / Reference Future

- **Model-0 PASS, 38 checks.** Regenerated model-0 Enlarged80 world: 69 living trees and 1.2949037088 m² before access, exactly matching the historical fixture; 2.0 m scene lattice spacing; middle authored line; `TeachingRackId = 0`; authored-corridor exclusion; full Lessons 2–5 chain passed.
- The regenerated authored rows put the nearest gap at 15.989349 m (half-width **7.9946747 m**), 9.2 mm inside the older 8.00385 m scene fixture, because the harness regenerates the stand rather than using the historical scene row offset. Tree membership and BA are identical. The gate now asserts the exact live gap formula and the accepted ~8.0 m calibration band (0.05 m); it does **not** rebaseline the accepted scene fixture or any protected target. The production model-0 path is unchanged in the diff; the only shared-constant change is `FutureTreeSpacingM` from the Hegyi alias to a literal 8, the same value.
- **Legacy40 PASS, 12 checks.** No authored layout lines, no teaching area (`bay=null`), one legacy rack path exercised, no V2 binding.
- **Reference Future v1 PASS.** `REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule=56C8B99FA1E8DDD1 archiveFileFnv=1A42C7BD0439E72F`; negative control detected a tampered year-100 value; preview hashes include year 100 `7AD177B3CC2F73C7`; continuation `EBD49ADEF18C3F36`, legacy V15 layout `BFFF66015A6619DA`; `REFERENCE_FUTURE_V1_CONTRACT_PASS` and `REFERENCE_FUTURE_V1_PASS`.

## Limitations and deferred work

- The five-rack V2 fixture is **verification-only**, reproducing the already-qualified Rack V2 geometry while RACK-V2-04/05 player authoring is unfinished. The final feature qualification must be repeated on the real player-authored Rack V2 workflow after RACK-V2-04/05/07 are integrated, before beginner/external Scenario One testing.
- No MFM coverage map UI (MFM-08), prescription/fit logic (MFM-04), Crop Tree candidate generation (MFM-03/05), proposal/Work Plan mutation (MFM-07) or MFM save/provenance schema (MFM-06) is implemented.
- `OPEN-3` (reopen-after-execution) remains open and unchanged. The V2 teaching area suspends while the plan is unconfirmed; it never falls back to authored lines.
- Unity gates are Editor-only and use queued InputSystem events for some lesson interactions, not real OS input. No Player/native-input run and no Windows build were performed.
- The first v2 attempt's cold import exceeded the original 1500 s gate budget at 110 passing checks. The final runs used the documented 3600 s budget and are warm. The interrupted attempts are recorded above and are not counted as passes.
- `origin/main` advanced by docs/spec-only commits after the tested base; the branch was not rebased after qualification, so the tested HEAD `8f9be4f` remains exact.

## Recommendation

**APPROVE** for MFM-02 only. The rack-served teaching sample and plain coverage geometry meet the packet's required behaviour and pass the focused offline, compile, model-0, Legacy40 and Reference Future gates on `8f9be4f`.

`RACK-SERVED SAMPLE GEOMETRY QUALIFIED — READY FOR MANAGER REVIEW`
