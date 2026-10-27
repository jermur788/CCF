# PARITY-04 — Focused Adult Species Unity Qualification — Handoff

**Task:** `PARITY-04-UNITY-QUALIFICATION` · **Claim:** [#27](https://github.com/jermur788/CCF/issues/27) · **Date:** 2026-10-27  
**Recommendation:** **APPROVE** the focused adult qualification, subject to Manager review.  
**Exit:** `ADULT SPECIES PARITY REQUALIFIED — READY FOR MANAGER REVIEW`

## Identity, authority and scope

- Packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Unity/TASK-PACKET.md`, from current main at task start.
- Spec: `SPEC-scenario-one-three-species-parity-p0-V1`, APPROVED; `specs/scenario-one-three-species-parity-p0/spec.md`, principally FR-3 and the adult portion of AC-3. This does not certify all parity acceptance criteria.
- Worker provenance: Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic / Claude Code in T3 Code) performed preflight, static gates, Unity import/compile and Legacy40. After its provider interruption, the user instructed the same thread to continue; OpenAI Codex `gpt-6.1-sol`, xhigh reasoning, through the configured Codex provider in T3 Code recovered that evidence and performed Enlarged80, preservation/cleanup and publication. Both executors are recorded on claim #27; no delegated reviewers or competing Unity workers were launched.
- Thread: `8091249d-2614-4b3b-87d5-9399d9cb3a1e`.
- Starting main / exact tested candidate / final tested HEAD: **`81a037e35126bbc256fcda076019af47e8e0954f`**. This contains PARITY-04 merge `71b5ae4c83f3ce6d489cdec81f3306abb54d9df1` and its Manager review.
- Canonical context snapshot: `17aa738c58b474476ce9c6d1f6163d42c63ae5d5` (latest context-source commit in the tested candidate). Live coordination is separate.
- Verification lane: `/media/jer/ZX20/CCF-U1`, detached HEAD. It was clean before import/compile. Its prior detached HEAD `840475103c33be9f168e55078c1ef56b7530d049` was restored after testing; it was clean including untracked files at this task's release.
- Publication branch: `verify/scenario-one-three-species-parity-adult-unity-v1`, based on the exact tested candidate. Its only change is this Markdown handoff; the publication commit is not a newly tested production candidate.
- Editor: **Unity 6000.6.0f1**, revision `f7f8ed4d1e24`, `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`.
- Verification-only: no production or verification target was changed to obtain PASS. No retuning, assertion weakening, source-contract refresh, expected-value edit, package/schema change, protected-anchor rebaseline or Windows build.

## Prerequisites and Unity ownership

PARITY-03 Unity handoff `03e0c679` was accepted by the Manager on main (`b41a76f`); claim #10 was closed, its lock released, and live coordination showed PARITY-04 READY / DISPATCHED before acquisition. The packet's printed BLOCKED heading predates this explicit acceptance. Receipts: `parity03-manager-review.md`, `live-main-active-tasks.md`, `claim-running-readback.json`.

- Lock ID: `bfc231a548e54217a74b96302f6ed45f`.
- Owner string: `T3-Code-Claude-Sonnet-5.5-Parity04Unity`; task `PARITY-04-UNITY-QUALIFICATION`; U1 at exact candidate.
- Normal acquisition: **2026-10-27T20:44:12.885759+00:00**; prior state FREE, no Editor. Claim marked RUNNING before the first Unity launch.
- Continuation retained this same task's existing owner credential and documented the actual Codex executor; no metadata overwrite, duplicate acquisition, force release or process reaper.
- After audit, U1 returned clean to its prior HEAD at **2026-10-27T20:57:07Z**. Lock then released normally (`forced: False`); readback **FREE / no Editor PIDs**.
- Receipts: `lock-status-before.json`, `lock-acquire.json`, `continuation-note.md`, `lock-release.txt`, `lock-status-after.json`, `audit/lane-restoration.json`.

## Static and Unity results on the exact candidate

All static tools were run unchanged by the first executor. The continuation inspected their retained output and the byte-preservation audit; it did not repeat successful gates or substitute standalone type checking for Unity compilation.

| Gate | Exact output/result |
|---|---|
| Integrated calibration/source contract | `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` |
| Source normalizer self-tests | `SOURCE_NORMALIZATION_PASS` (including semantic negative controls) |
| Site infrastructure | `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` |
| Fixture controls | `FIXTURE_CONTROLS_PASS checks=9 static-contracts=7 NOT_UNITY_VERIFIED` (this marker describes the offline check; actual Unity results follow) |
| Unity import/compile | exit **0**, **85 s**; no C# errors; normal successful batch exit |
| Adult gate, Legacy40 (`CCF_STAND_GEOMETRY=0`) | **`BROADLEAF_GROWTH_VERIFY_PASS checks=30`**, Unity exit **0**, runner PASS, **98.91 s**, no unexpected tree changes |
| Adult gate, Enlarged80 (`CCF_STAND_GEOMETRY=1`) | **`BROADLEAF_GROWTH_VERIFY_PASS checks=30`**, Unity exit **0**, runner PASS, **148.06 s**, no unexpected tree changes |

The 30 checks comprise 26 asserting checks and four informational reports, as integrated; no count was changed. Both results identify tested HEAD `81a037e35126bbc256fcda076019af47e8e0954f` and the unchanged harness SHA-256:

`4f693f74c790dd0a5b47b753bd001440bd9d9954713a740397289a75e810d4f6`

### Observed adult behaviour

Both geometries verified:

- Loaded Sitka/Oak/Beech adult constants exactly match the accepted assets, including growing-space class and absent age-phase tables. Oak/Beech constants remain **GAMEPLAY CALIBRATION**, not measured Irish growth constants.
- Live site scalar exactly **1.0** at scene start and after each species fixture: all **64 / 256 cells** and sampled cell-centre lookups neutral. The separate site check supplies the dormant seam proof; no seam allow-list was broadened.
- Identical geometry produces bit-identical canopy/light across the three species: ten shaded cells, zero differing values. Increasing the Oak crown by 1 m changes 20 values, so the comparison is non-vacuous.
- Shared Hegyi cutoff exactly **8 m**. All nine target/neighbour species pairs count the 7.99 m neighbour identically (CI **0.12516**) and exclude the 8.01 m neighbour exactly (CI **0**).
- Oak/Beech follow the retained species law, remain bounded/monotone, and match across growth models 0/1/2 when density does not act. Competition reduces DBH without changing height in the fixture. Deterministic replay and in-memory save/load continuation agree for both species.
- Protected Sitka Class III comparator: expected **12.4179 m**, actual **12.4179 m**.

These are bounded adult fixtures. Broad protected lifecycle/reference/completion/economy suites were not newly run here; PARITY-03's prerequisite qualification is separately recorded. No broad-suite or whole-parity acceptance is claimed.

### Height diagnostics — not pass/fail targets

Identical output in both geometries:

| Species | Age 50 | Age 75 | Age 100 |
|---|---:|---:|---:|
| Oak | 21.45 m | 28.18 m | 32.76 m |
| Beech | 23.37 m | 30.24 m | 34.65 m |

Beech is **1.63 m** below the provisional age-50 band and **0.35 m** below the age-100 band: `ACCEPTED_PROVISIONAL_HEIGHT_BAND_MISS (PARITY-02 U-11)`. This is the accepted calibration limitation. No band was promoted to an empirical target and no retuning occurred. The independent species-law comparison had maximum height difference **0 m**.

### Retained diagnostics

Candidate compiler warnings were present: CS0618 deprecated Unity API use, CS0108 member hiding, CS0219 unused variable and CS0414 unused field. Logs contain no `error CS`, gate failure marker or C# exception. Counts/log summaries are retained rather than suppressed.

All three Editor logs report `[Licensing::Module] Error: Access token is unavailable; failed to update`, followed by successful entitlement resolution/license update and completed execution. Both adult logs also contain GTK/GLib widget cleanup assertions (`gtk_widget_hide`, `gtk_widget_destroy`, `g_object_unref`). Those platform diagnostics did not prevent the terminal PASS and exit 0; they are not presented as absent or reclassified as compiler errors. No broader platform certification is implied.

## Reproduction and evidence

Run from the exact clean candidate in an available Unity lane under its normal machine-wide lock. Static commands (external output paths):

```bash
python3 -B Tools/Calibration/OakBeechGrowth/check_integrated_v1.py --out <EV>/static/growth-integrated.json
python3 -B Tools/Calibration/OakBeechGrowth/test_source_normalization.py
python3 -I -B Tools/Verification/SiteInfrastructure/run_site_check.py --out <EV>/static/site
python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_fixture_controls.py --out <EV>/static/fixture
```

The retained `run-compile.sh` and `run-gates.sh` record the actual Unity environment/commands. `TMPDIR`, `TMP`, `TEMP` point to `/media/jer/Files/CCF/Operations/UnityTmp-P04U`; `CCF_TEST_DIRECT_ENTRY=1` bypasses the startup menu for the disposable gate. Each invocation has isolated `XDG_CONFIG_HOME` and Hub licence setup. No real save is written by this gate (capture/load is in memory).

```bash
bash <EV>/run-compile.sh
bash <EV>/run-gates.sh legacy40 0
bash <EV>/run-gates.sh enlarged80 1
```

The unchanged `Tools/Verification/ThreeSpecies/run_unity_gates.py` performs copy → execute → remove of the harness and its generated `.meta`. Expected observable results: terminal marker `BROADLEAF_GROWTH_VERIFY_PASS checks=30` in each log, exit 0, JSON runner PASS, exact constants/site/canopy/Hegyi assertions above, and diagnostic-only height output. The runner's marker test alone does not count checks; the exact `checks=30` terminal lines were separately inspected.

**Durable evidence root:** `/media/jer/Files/CCF/Verification/Parity04-UnityQualification-20261027T204332Z/`

- `static/`: four tool logs and integrated calibration JSON.
- `compile/Unity.log`, `compile/result.json`.
- `{legacy40,enlarged80}/BroadleafAdultGrowthVerification.log`, `results-BroadleafAdultGrowthVerification.json`, `runner-result.json`.
- `audit/*-log-summary.json`, `audit/preservation-result.json`, full original SHA-256 manifests, captured settings side-effect patches, and clean-status/restoration receipts.

## Preservation and state at release

Before execution: **5,079 tracked files** and **ten real user save/preferences files** were hashed. After both gates, the exact-candidate audit found:

- **zero tracked byte changes**, **zero real user-file changes**;
- no disposable `BroadleafAdultGrowthVerification.cs` or `.meta` under `Assets/ForestPrototype/`;
- the sole Editor side effect was `.vscode/settings.json` solution-name/format normalization. Its delta matched the captured compile patch exactly and was restored byte-for-byte to the original clean candidate; both delta receipts remain in evidence;
- no production, species asset, scene, prefab, material, package, ProjectSettings, save/schema, verification source, expected value, protected fingerprint or Reference Future v1 change;
- U1 restored to clean detached `840475103c33be9f168e55078c1ef56b7530d049`; Unity lock FREE; no Editor processes at release.

After normal release, a subsequent worker moved U1 to MFM-02 candidate `f632d7a1a79616dde32b347b28ab897dbf24b0bf` and staged its own `MfmRackSampleGeometryVerification` harness. The publication preflight detected that transition before changing anything; this task did not restore, clean, edit or interrupt the new worker. The clean-lane/no-Editor receipts above describe this task's release, not a reservation of U1 after it. Publication uses an isolated Git index from Primary, without changing its working files or real index.

Only this handoff Markdown is added by the publication branch. Qualification does not modify the tested candidate.

Unrelated Primary work preserved: modified `AGENTS.md` and `Docs/Project/coordination/active-tasks.md`; untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md`. Its status exactly matches the original task snapshot. No active-task, board, canon or generated dashboard edit was made by this worker.

## Remaining gates and operational follow-up

- **Ready for Manager review: YES. Recommendation: APPROVE** this focused adult qualification. This worker does not grant final G2/G3/G4 acceptance or integrate its own handoff.
- Manager-owned coordination/context status may now be updated after review; this task does not accept all species parity, full Scenario One/demo readiness, Player/native-input performance or Windows-build readiness.
- **DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO.** No dashboard publisher/ingestion implementation or audit/publication readback endpoint is available in the inspected task surface; the existing Manager review also records it unavailable. Authoritative `active-tasks.md` is Manager-owned and contains unrelated local edits. The handoff and claim carry the worker result; Manager must reconcile the live coordination record, run/allow supported ingestion, and verify the automatic GitHub audit snapshot plus published dashboard. This is an outstanding operational issue, not a waived gate or a claim of dashboard success.
