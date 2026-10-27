# PARITY-03 Unity Tier-2 qualification handoff

**Verdict:** `MIXED DENSITY PARITY REQUALIFIED — READY FOR MANAGER REVIEW`

**Task / claim:** PARITY-03-UNITY-QUALIFICATION / [#10](https://github.com/jermur788/CCF/issues/10).

**Worker:** T3 Code / Codex, gpt-6.1-sol, xhigh reasoning, configured Codex provider.

**Date:** 2026-10-27. **Role:** Unity Operator; bounded qualification only.

**Source branch:** `feature/scenario-one-three-species-parity-density-requal-v1`.

**Implementation base:** `c6342f79a3e89c040fadd232017a937132779385`.

**Required production HEAD:** `e1515e165eb40b40b5200992398984d69e0e8e8e`.

**Actually tested HEAD:** `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8`, the permitted documentation-only static-handoff descendant. `Assets`, `Packages`, `ProjectSettings` and `Tools` are byte-identical to the required production HEAD. The commit publishing this handoff adds documentation only.

**Context:** packet, approved parity spec and live main coordination captured from `72064a1e9e15e73ec346916f6216076e8ef545d7`; the packet specifies current Git authority rather than a separate context SHA. The stale `eb218ea` attachment snapshot was not used as parity authority.

**Spec:** SPEC-scenario-one-three-species-parity-p0-V1, APPROVED; FR-1/FR-2 / PARITY-03 only.

## Result and production diff

**Production diff: NONE.** No patch, parameter retune, threshold change, fixture change or anchor rebaseline was needed. Only this handoff is added to the task branch.

The existing, unchanged `MixedDensityVerification` passed **34 checks in Legacy40 and 34 in Enlarged80**, with normal Editor exit 0 in both runs. Clean import/compile also passed on the actual Unity version. The static findings at `b22c75f` remain valid; their historical **NOT UNITY-QUALIFIED** execution status is supplemented by these new receipts, not rewritten.

This qualifies the pinned density candidate. It does not qualify current integrated main, full three-species parity, the whole parity spec, an external demo or a Windows build.

## Preflight, lane and lock

- Refreshed live coordination and claim #10. S1-C-IMPL-02 was handed off/integrated, Rack V2 held no Unity lock, and PARITY-03 was next in the Unity queue. Independent static review #9 remains separate.
- Primary's unrelated modifications to `AGENTS.md` and `Docs/Project/coordination/active-tasks.md`, and untracked `Docs/Operations/StorageArchitecture.md` / `Docs/ScenarioTwoPreProductionAudit.md`, were preserved. No writes were made in Primary.
- Configured lane **U1**, `/media/jer/ZX20/CCF-U1`, was clean and unassigned at detached `8763623c7b8a65c357e136a94ded117ff42492e3`. It was placed detached at the tested HEAD; the source branch remained in CCF-Parity03. No duplicate writable branch was created.
- Verified ZX20 `/dev/sda2`, exFAT, UUID `18D6-896E`; storage passed the configured 15 GiB floor. Durable receipts use the Files ext4 volume, `/dev/sdc4`.
- **Unity:** `6000.6.0f1 (f7f8ed4d1e24)`, executable `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`. U1 used its own existing Library; no Library was shared or copied.
- **Lock:** `11ef70090dac4e28b3d5ce45b4eae4a6`; owner `T3-Code-Codex-gpt-6.1-sol-Parity03`; task `PARITY-03-UNITY-QUALIFICATION`. Acquired normally at **20:10:19 UTC**, released normally at **20:26:54 UTC**. No force release or competing Unity execution.
- Claim #10 was changed to **RUNNING**, with actual routing, worktree and starting HEAD, and read back **before the first Unity launch**. `claim-running-readback.json` preserves that record.
- Allowed phases: clean import/compile and focused Tier-2 density gates. Player/native input was not required. Successful Editor PIDs: compile **624318**, Legacy40 **626203**, Enlarged80 **628508**; the import worker belonged to the same execution. No Editor remained at release.

## Gates actually run

| Gate | Actual result |
|---|---|
| Clean Unity import/compile | PASS; Editor exit 0; zero C# compilation errors and zero importer exceptions |
| MixedDensityVerification, `CCF_STAND_GEOMETRY=0` | PASS; 34 checks; exit 0; 144.49 s; no failure markers or unexpected tree changes |
| MixedDensityVerification, `CCF_STAND_GEOMETRY=1` | PASS; 34 checks; exit 0; 409.29 s; no failure markers or unexpected tree changes |
| Protected source comparison against the accepted static handoff | All six SHA-256 values exact: SitkaGrowthModel, MixedDensityModel, MixedDensityModel.Composite, SitkaSubsetDensitySignal, ForestEcologyController and AdultMortalityAccount |
| Final workspace/save audit | All 4,963 tracked files byte-identical to pre-execution; eight real user save/preferences files unchanged; clean Git status; disposable script and `.meta` absent |

Both density logs contain `MIXED_DENSITY_VERIFY_PASS checks=34`. Neither contains a density failure, C# error or exception. Existing Unity deprecation warnings are not compiler errors.

### Runtime conformance evidence

- Pure Sitka under model 2 used `LegacySitkaExact` only, never occupancy. Models 1 and 2 matched each year's density accounting, victim IDs and final tree-state hash exactly, with real mortality exercised.
- One living Oak selected `SpeciesAwareOccupancy` under model 2 in both geometries; the same fixture under model 1 retained `LegacySitkaExact`. Near-pure two-Oak, Oak/Beech and Oak/Sitka fixtures preserved the expected ordinal occupancy steps.
- Pure Oak, pure Beech, Sitka/Oak, Sitka/Beech, Oak/Beech and all-three mixtures exercised the real controller under model 2. Occupancy matched the model prediction and additive class parts; mixed replay was exact. Mortality was bounded, occupancy fell after thinning, and every broadleaf density death produced one deadwood record.
- Below-onset fixtures had zero density mortality; crossing the onset produced pressure; heavier crowding increased pressure while retaining the annual loss bound.
- Mixed save/load continuation matched uninterrupted execution and retained growth model 2. Save version remained **21**, as inherited from Rack V2; no schema change occurred. Unknown growth model 3 was refused. Historical v16 Legacy40 mapped to growth model 0; a v16-labelled Enlarged80 save was refused without changing the session. Reference previews used growth 0 and restored the session model.
- The accepted static proof that models 0/1 remain legacy, that dead/stump/sub-promotion cohorts do not select the mixed route, and that species `Ci50` does not bias density victims is carried forward on byte-identical source. The runtime route, victim-ID and mixed-fixture results are consistent with that proof. A new statistical victim-neutrality sweep was not run.
- The prior measured 1,000-stem near-pure continuity sweep remains the accepted static/offline evidence; it was not rerun as a Unity parameter-search loop. No DEC, DMC, mixture bonus or broadleaf maximum-density equation was introduced.

### Actual fingerprints

| Measurement | Legacy40 | Enlarged80 |
|---|---|---|
| Pure-Sitka ten-year tree state, model 1 = model 2 | `6E7843FA59B43D2A` | `7FDBD35E558C7E9F` |
| Lifecycle neutral browse, model 1 = model 2 | `7E57B9DAEF5BF4D0` | `1FE6639780636394` |
| Lifecycle normal browse, model 1 = model 2 | `35E2BF1C2F55C2DE` | `702DE05F33384139` |
| Mixed save/load, uninterrupted = resumed | `FE5D86EBC605FD82` | `9650A04FBDB48DFF` |

The Legacy40 lifecycle fingerprints are the accepted protected anchors. The Enlarged80 lifecycle values reproduce the harness's previously **observed growth-model-1 baselines**, explicitly labelled **not Manager-accepted anchors**. Both geometries additionally compare freshly measured models 1 and 2 directly. No value was changed or promoted to a stronger evidence status.

## Commands and expected observable result

Working directory: `/media/jer/ZX20/CCF-U1` at the tested detached HEAD, under the lock. The unchanged repository runner performs the documented copy/run/remove workflow. Its staged source SHA-256 was `c1ccf6db575dd3ff539f35b8236155b3b4e4adca6933bc5bab09a64d11e94268` in both runs.

The three isolated configs were populated from the current Hub-managed licence directory, `~/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses`, without changing either real licence location. Those temporary licence copies were removed after execution.

```bash
source /media/jer/ZX20/CCF-Ops/environment.sh
export TMPDIR=/media/jer/Files/CCF/Operations/UnityTmp-P03
export TMP="$TMPDIR" TEMP="$TMPDIR"
export CCF_TEST_DIRECT_ENTRY=1
```

Successful compile command, with `XDG_CONFIG_HOME` set only for that process:

```bash
XDG_CONFIG_HOME=/media/jer/Files/CCF/Verification/Parity03-UnityQualification-20261027T200949Z/compile/config /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -nographics -quit -projectPath /media/jer/ZX20/CCF-U1 -logFile /media/jer/Files/CCF/Verification/Parity03-UnityQualification-20261027T200949Z/compile/Unity-hub-license.log
```

Density commands:

```bash
CCF_STAND_GEOMETRY=0 python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --unity /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity --gate MixedDensityVerification --out /media/jer/Files/CCF/Verification/Parity03-UnityQualification-20261027T200949Z/legacy40 --timeout 1200
CCF_STAND_GEOMETRY=1 python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --unity /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity --gate MixedDensityVerification --out /media/jer/Files/CCF/Verification/Parity03-UnityQualification-20261027T200949Z/enlarged80 --timeout 1200
```

Expected and observed: terminal `MIXED_DENSITY_VERIFY_PASS checks=34`, normal exit 0, exact fingerprints above, no staged harness/meta left behind. Full commands and process environment are retained in `gate-commands.json`. No movement, harvesting, construction, UI rendering, native-input or Player gate was added to this density-only packet.

## Startup incidents, cleanup and caveats

Three unsuccessful startup attempts preceded the successful compile; none reached gameplay assertions:

1. Ops `environment.sh` places temporary files on exFAT ZX20. Licensing-client .NET startup failed with `Failed to create CoreCLR, HRESULT: 0x8007000E`. Its automatic reconnect failed too. After inspecting exact task-owned argv and zombie licensing children, only the task-owned Editor was manually stopped (exit 143).
2. A process-local `.NET` heap-limit probe did not help; that task-owned startup was also stopped after inspection (exit 143). The setting was discarded. [Microsoft's process-level setting documentation](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector#heap-hard-limit) was consulted; no GC option was used for successful qualification.
3. A controlled client-only probe and strace established the actual storage issue: AF_UNIX socket `bind` and FIFO `mknodat` under `CCF-Temp` returned `EPERM`. Changing only `TMPDIR`/`TMP`/`TEMP` to the Files ext4 directory made the same client start normally. The next Editor attempt then exposed a stale host licence copy (exit 198, missing headless entitlement). Using the current Hub licence in isolated task configs resolved it.

The final compile and both required density gates exited **normally with code 0**. No post-PASS reaper or timeout was used. All startup logs and recovery probes are retained; no global environment, licence, dependency or project configuration was patched. The exFAT temp-directory incompatibility is an operational follow-up for the Ops maintainer.

Unity regenerated only `.vscode/settings.json` (the lane solution name and JSON comma normalization). That exact generated patch was recorded, retained consistently during the locked gate chain, then restored to the original tracked bytes after all Editor processes exited. No scene, prefab, material, species asset, package or ProjectSettings file changed. The complete tracked-file audit passed afterwards.

**Beech limitation remains:** the accepted provisional adult height-band miss is labelled calibration, not empirical validation, and was not retuned or tested away. The static handoff also records three pre-existing Beech R2 long-run failures and a stale offline source-scanner false positive; those offline suites were not rerun or relabelled green here. Actual Unity compilation resolves the cloud reference-assembly compile uncertainty for this snapshot without rewriting that historical report.

## Evidence, remaining gates and coordination

Durable evidence root:

`/media/jer/Files/CCF/Verification/Parity03-UnityQualification-20261027T200949Z/`

- `compile/result.json`, successful `compile/Unity-hub-license.log`, and all earlier startup logs.
- `legacy40/` and `enlarged80/`: `MixedDensityVerification.log`, `results-MixedDensityVerification.json`, `assertions.txt` and isolated evidence/config directories.
- `preflight.json`, captured packet/spec/live coordination, `claim-before.json`, `claim-running-readback.json`, `gate-commands.json`, `execution-pids.json`.
- `protected-source-check.json`, `tracked-before.json`, `user-save-prefs-before.json`, `editor-side-effects-final.patch`, `final-audit.json`.
- `lock-acquire.json`, `lock-release.json`, `lock-final-status.json` and startup incident/strace receipts.
- Accepted earlier static evidence remains at `/media/jer/Files/CCF/Verification/Parity03-StaticRequalification-20261027T185345Z/`.

**Files changed:** this `HANDOFF.md` only. **Serialized Unity files touched:** none persistently. **Assets added/modified:** none. **Simulation/save impact:** none. **Ready for review:** yes.

**Spec conformance:** PASS for this packet's FR-1/FR-2 density qualification, combining the accepted unchanged static proof with the new bounded Unity evidence. Full parity-spec conformance is not assessed. Remaining acceptance: Manager G2/G3 review and the separately dispatched independent static review #9; G4 integration and later parity slices remain separate. No acceptance or integration gate is self-approved here.

**DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** Claim #10 and this Git handoff record execution status. No configured ingestion/publisher for this new parity claim was found in the repository or deployed CCF-Ops tooling. The worker has not edited Manager-owned live locks or generated dashboard state. Manager/publisher must update the authoritative PARITY-03 row, ingest this result, confirm the automatic GitHub audit advances and verify the published dashboard readback. This operational synchronization remains outstanding and does not count as completed by closing an execution-presence claim.

**Recommended next step:** review these exact receipts with the independent static review, then issue the next already-authorised parity qualification packet. U1 is clean at `b22c75f`, the machine-wide lock is free and this worker retains no Unity reservation.
