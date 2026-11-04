# Scenario One Verification-Debt Repair (#223) — Handoff

**Status:** COMPLETE — verification-only repair returned for Browser Overall review.
**Success token:** `SCENARIO ONE VERIFICATION DEBT REPAIRED — READY FOR MFM-12 REPIN`

## Binding identity

| Field | Value |
|---|---|
| Claim | #223 (under #203 / #200 / #221) |
| Packet | `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/TASK-PACKET.md` (main `d62eed87`) |
| Disposition | `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/BROWSER-DISPOSITION.md` |
| Branch | `verify/scenario-one-debt-repair-v1` |
| Starting production HEAD | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` |
| Verification commit | `ad9a8f761c062e97895838fb0f12db1aa71a6ff3` |
| Handoff commit | the commit that adds this file; linked on #223 (a file cannot contain its own hash) |
| Worktree | `/media/jer/ZX20/CCF-U1` (dedicated Unity-capable lane, warm Library) |
| Execution provider/model | `opencode-go/deepseek-v4.1-flash` |
| Requested routing | SOL/CODEX high-reasoning local lane. Delegation to `opencode/gpt-5.3-codex` (xhigh) failed immediately with `Upstream request failed: Invalid credential`; this is an environment/provider failure, not a model-capability failure. The repair continued on the active local lane. |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneVerificationDebtRepair-20261102/` |

## What changed (verification only)

No production C#, UI, gameplay, ecology, economy, calibration, save schema/semantics, protected anchor or reference future was changed. `git diff 3acdf9cb..ad9a8f76 -- Assets Packages ProjectSettings` is empty.

| File | Change |
|---|---|
| `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json` | Repinned only the `ForestSaveData.cs` normalized hash to the accepted v24/D-066 source `129d0ace` (`ed2b48b6…`). The other six protected entries are unchanged. |
| `Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs` | `G_SAVE_VERSION_IS_STILL_23` → `G_SAVE_VERSION_IS_STILL_24` (assertion regex 24). |
| `Tools/Verification/SiteInfrastructure/run_site_check.py` | Stale docstring “still 23” → 24 (comment only). |
| `Tools/Verification/ThreeSpecies/OfflineLogic/run_pruning_check.py` | Pins the accepted moved save-layer sources (`ForestSaveData.cs` → `129d0ace`; `ForestSaveController.cs`/`ForestSaveValidation.cs` → PARITY-08 `3eb9c954`) while every other protected file stays byte-pinned to BASE. The manager check now compares the **pruning call-site lines only**, because later accepted features legitimately changed the manager outside pruning; the pruning consumer/gate guard and legacy-method guard are unchanged. |
| `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` | `POLICY_SAVE_VERSION_UNCHANGED` now asserts v24. |
| `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs` | Both save-version assertions now v24. |
| `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs` | Both v23 pins now v24 (history harness mechanical pin). |
| `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py` | Added `ScenarioOne/ScenarioManagementHistory.cs` to the history compile inputs. |
| `Tools/Verification/ThreeSpecies/OfflineLogic/HistoryOfflineStubs.cs` | The history stub now mirrors the one `StormEventRecord.year` field the per-year history reads. |
| `Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityOfflineCheck.cs` | Narrowed only the “maximum basal/SDI” identifier rule so a maximum **removal share** is not treated as an absolute species maximum density/basal area. The species+limit+density rule, `densityFiles` rule, string rule and lexer probe are unchanged. |
| `Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs` | Applied the accepted #198 D-066 latch reconciliation: save/load keeps the completed teaching stage, session review detail stays non-persistent, and the two schema pins read v24. |
| `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs` | Applied the accepted RACK-V2-09 field step: all five confirmed V2 rack entrances are marked through the authoritative manager API before approval, then the existing annual-advance assertion is retained. |

The packet’s “five” PARITY-09 surfaces undercounted the same mechanical v23 pin family in the history and guided-v2 harnesses. Those additional pins were repaired only where a required gate exposed them, under the same accepted v24/D-066 authority.

## Offline/static gates

All commands ran from `/media/jer/ZX20/CCF-U1` with `PYTHONDONTWRITEBYTECODE=1`; outputs are under the evidence root.

| Gate | Result |
|---|---|
| `SiteInfrastructure/run_site_check.py --out …/site` | **PASS** — `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` |
| `Tools/Calibration/OakBeechGrowth/check_integrated_v1.py --out …/growth-integrated.json` | **PASS** — `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` |
| `ThreeSpecies/OfflineLogic/run_offline_check.py --suite timber` | **PASS** — `THREE_SPECIES_TIMBER_PASS checks=73` |
| `ThreeSpecies/OfflineLogic/run_offline_check.py --suite density` | **PASS for the scanner/isolation repairs; exit 1 only on the three accepted pure-Beech R2 misses.** `THREE_SPECIES_OFFLINE_PASS checks=91`, `MIXED_DENSITY_ISOLATED_PASS`, `SITKA_MODEL_ISOLATED_PASS`; `COMPOSITE_DENSITY_FAIL checks=107 failures=3` = `LONG_RUN_L40_B_BEECH_DENSE` 0.423, `LONG_RUN_L40_F_BEECH_YOUNG` 0.407, `LONG_RUN_E80_F_BEECH_YOUNG` 0.417. No `MaxRemovedBasalShare` violation. The 0.40 criterion was not changed. |
| `ThreeSpecies/OfflineLogic/run_pruning_check.py --out …/pruning` | **PASS** — `PARITY06_PRUNING_PASS checks=3946 failures=0 NOT_UNITY_VERIFIED`; preservation 17 files, 1296 legacy-differential cases. |
| `ThreeSpecies/OfflineLogic/run_offline_check.py --suite history` | **PASS** — `THREE_SPECIES_HISTORY_PASS checks=74` |

## Unity gates

Machine-wide lock `8da639a53a5b4d7b9c54b08639f540fc` was acquired normally at 2026-11-02T13:13:45Z for U1 / `verify/scenario-one-debt-repair-v1` / `ad9a8f76…`, and released normally after the worktree was restored clean. Final lock state: **free**, metadata None, no Unity Editor PIDs.

The host licence (`~/.config/unity3d/Unity/licenses`) is stale for this Editor (`com.unity.editor.headless` 404 / exit 198). The gates used the newer Hub flatpak licence (`~/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses`), which is the already-documented licence-source divergence and not a product failure.

| Gate | Result |
|---|---|
| MixedDensityVerification — Legacy40 | **PASS** — `MIXED_DENSITY_VERIFY_PASS checks=34` |
| BroadleafTimberEconomyVerification — Legacy40 | **PASS** — `BROADLEAF_TIMBER_VERIFY_PASS assertions=66` |
| BroadleafTimberEconomyVerification — Enlarged80 | **PASS** — `BROADLEAF_TIMBER_VERIFY_PASS assertions=66` |
| ScenarioHistoryVerification — Legacy40 | **PASS** — `SCENARIO_HISTORY_VERIFY_PASS assertions=43` |
| guided-v2 | **533 PASS / 2 FAIL.** Both remaining failures are `current objective fits` at 1280×720 and 1920×1080 — the #224 Objectives-window product/UX defect. The gate is honestly still red and was not suppressed, weakened or hidden. |

Guided-v2 stale families are gone: `V2 confirmed rack lesson clearly requests annual advance`, `eight original goals and the v24 schema`, `the save schema version is unchanged`, and all three `sequence save/load … requires fresh rack review` / `three future trees and six stand portions reviewed` failures now pass. The only failures left are #224’s objective-fit family.

### Guided-v2 environment note

The first guided-v2 attempt timed out at its default 2430 s with 361 passes and no failures; two subsequent attempts hung at Editor start because a killed run left a stale `Temp/UnityLockfile` and generated `Temp` state. After removing the generated `Temp` directory (no production or Library deletion), the gate completed in 337 s. This is runner/Editor hygiene debt, not a production failure. The failed/timeout runs are preserved in `…/unity/guided-v2`, `guided-v2-r2`, `guided-v2-r3`; the completed run is `…/unity/guided-v2-r4`.

## Before/after failure families

| Family | Before | After |
|---|---|---|
| OakBeechGrowth `ForestSaveData.cs` normalized hash | FAIL | PASS |
| SiteInfrastructure v23 pin | FAIL | PASS |
| Pruning protected save-layer pins / whole-manager assumption | FAIL | PASS |
| MixedDensity `POLICY_SAVE_VERSION_UNCHANGED` | FAIL (v24) | PASS |
| BroadleafTimber save-version pins | FAIL | PASS |
| History compile (`WorkItemDetail`) | FAIL (compile) | PASS |
| `MaxRemovedBasalShare` scanner | FAIL (false positive) | PASS |
| guided-v2 stale schema/sequence/rack expectations | FAIL (9 of 11) | PASS |
| guided-v2 `current objective fits` | FAIL (2) | **still FAIL — #224** |
| Three accepted pure-Beech R2 misses | FAIL (3) | **still FAIL — accepted C-level limitation** |

## Remaining defects / debt

1. **#224 Objectives-window overflow** — `current objective fits` at 1280×720 and 1920×1080 remains red. This is the excluded product/UI repair and must not be called green by #223.
2. **Three pure-Beech R2 misses** — 0.423 / 0.407 / 0.417 against the unchanged 0.40 criterion. Accepted C-level limitation; no retune or threshold change.
3. **Licence-source divergence** — repo runners default to the stale host licence; the Hub flatpak licence is current. Tooling debt only.
4. **Guided-runner Temp hygiene** — a killed Editor can leave `Temp/UnityLockfile` and stall the next launch. Tooling/runner debt; documented here.
5. **Codex lane credential failure** — the requested SOL/CODEX delegation failed upstream with `Invalid credential`; recorded as provider/environment state, not model capability.

## Cleanup and boundaries

- U1 restored clean at `ad9a8f76…` (including the known Unity material/settings rewrites restored to HEAD); no staged harness or `.meta` under `Assets/ForestPrototype/`.
- Machine-wide Unity lock released normally; lock free; no Editor.
- No production file, save schema, anchor, Reference Future, ecology/economy/calibration or package change.
- No Windows build. #194 NO ACTION.
- #224 and MFM-12 were not started; Browser Overall reviews #223 first.
- Live coordination was refreshed from `origin/main` (`Docs/Project/coordination/active-tasks.md`, #200 is its sole writer during this split); it was not edited here.

## Recommended next step

Browser Overall reviews this handoff and the evidence root. Then release #224 (Objectives-window overflow) from the same production candidate; after #224 is accepted/integrated, repin #225/MFM-12 to the exact resulting production SHA.
