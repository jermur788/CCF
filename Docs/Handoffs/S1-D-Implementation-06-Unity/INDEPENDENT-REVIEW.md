# S1-D T-06 — Independent review of the test-only fixture reconciliation

**Reviewer:** Claude A — model `claude-opus-5-5` (Claude Opus 5.5), provider Anthropic, via the Claude Code harness. Read-only review; I did not write any of the reviewed code.
**Task:** S1-D T-06 (claim #88), bounded test-only repair (claim #95), umbrella #91.
**Exact reviewed candidate:** `51e685ef7400a7171d34ba063ece93b1a15d1820`
**Base (reviewed production):** `29bb6cbd6bf5dd81499a1e45bb68666f34969309`
**Docs tip reviewed:** `45459d2a7f2f9b215632e24869dae32202be754a` (adds `HANDOFF.md` only)
**Branch / worktree:** `fix/s1-d-t06-fixture-legacy40-v1` / `/media/jer/Videos/CCF-S1D-T06Fix`
**Scope of this review:** source diff, static audit, archived gate evidence, live read-only lane/licence state. No Unity was run, no lock was taken, and no Windows or Player build was made.

## 1. Change scope and production bytes

| Check | Command | Result |
|---|---|---|
| Files changed by the candidate | `git diff --name-status 29bb6cb..51e685e` | Only `Tools/Verification/SaveCellIndexHardening/SaveCellIndexHardeningVerification.cs` and `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/SaveEventIdIntegrityVerification.cs` |
| Files changed including docs | `git diff --name-status 29bb6cb..45459d2` | The two files above plus `A Docs/Handoffs/S1-D-Implementation-06-Unity/HANDOFF.md` |
| Production/schema/settings | `git diff --stat 29bb6cb 51e685e -- Assets ProjectSettings Packages` | Empty |
| Validator blob at the base | `git ls-tree -r 29bb6cb` | `Assets/ForestPrototype/ForestSaveValidation.cs` = `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb` |
| Validator blob at the candidate | `git ls-tree -r 51e685e` | `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb` (identical); `.meta` = `bb3abfc3…` unchanged |
| U4 harness | `git diff 29bb6cb 51e685e -- Tools/Verification/U4` | Empty, so the U4 `SYN duplicate event IDs` fixture is the reviewed `29bb6cb` version |
| Changes after Q1/Q2 ran | `git diff --stat 1a44526..51e685e` | Only `SaveCellIndexHardeningVerification.cs` (+11/−2), which is used by Q3 alone |

The handoff's path is correct (`Assets/ForestPrototype/ForestSaveValidation.cs`; there is no `Scripts/` subfolder). No production, save, schema, validation or migration file changed.

## 2. Test-change audit

The whole diff is additive except for two changed lines in the Q3 startup block.

**Legacy40 → valid model-0 (both harnesses, identical helper `MakeRackModelZero`)**
- The helper sets `rackPlanningModel = RackPlanningModel.Legacy` (const `0`, confirmed in `Assets/ForestPrototype/RackPlanningModel.cs`), `racks = []`, `rackPlanConfirmed = false` and `nextRackId = 1`, and removes work orders with `rackId > 0`. It returns the number removed.
- The explicit preconditions are: geometry 0, 64 ecology cells and 64 understorey cells; no Rack V2 state; and `ForestSaveValidation.ValidateRackV2(legacy) == null` (public static, `ForestSaveValidation.RackV2.cs:13`). Each precondition throws through `Check`, so the gate fails loudly if the basis drifts. Nothing gets skipped silently.
- Root cause confirmed from the archived evidence. At `29bb6cb`, Q1 failed on its first case with `central validation mismatch: rack planning V2 requires the Enlarged80 stand` (`S1D-T06-Unity-20261029/q1-run-1`, source sha `4226cf53…`). That is the production validator correctly refusing a Legacy40 copy that still had model 1. It was not an event-ID verdict.
- Removing orders excludes nothing. Both runs log `rackOwnedOrdersRemoved=0` and `enlargedRacks=0`.
- The Enlarged80 basis is untouched (`enlargedRackPlanningModel=1`). It still runs every case, so the live model-1 path stays covered.

**Q3 startup Continue**
- The final valid-save case and the on-disk Continue file now use `enlargedBaseline`, a clone of the materialised 256-cell capture taken before the fixture loop. The baseline's own validity is logged (`validation=null cells=256 badCellIndices= patches=0`).
- `if (StandaloneSessionMenu.IsOpen) menu.Close();` runs before `ShowStartupScreen()`. This is justified by production code: `ShowStartupScreen()` returns early when `IsOpen` (`StandaloneSessionMenu.Startup.cs:51`), so `ContinueAvailable` would otherwise keep a stale value. `Close()` (`StandaloneSessionMenu.cs:94`) only restores the UI, time-scale, player and save-enabled state. It does not touch the save file or the world.
- The malformed-save half (`bad = Clone(legacy)`, juvenile index 64) is unchanged. It is now stronger: with the basis valid model-0, the refusal must come from the out-of-range cell index rather than the Rack V2 mismatch.

**No-weakening disposition.** No `Check`, expected verdict, case list, index set, rejection-reason comparison, disk-byte/no-mutation comparison, startup assertion or U4 assertion was removed, edited or made conditional. Case counts match independent prior baselines:
- Q1 `cases=19 rejected=9 accepted=10`. This equals the count pre-declared from source in the `29bb6cb` handoff (`Docs/Handoffs/S1-D-Implementation-06/HANDOFF.md:147`).
- Q3 `cases=131 rejected=78 accepted=53`. This equals the earlier cell-index qualification (`Docs/Verification/SaveCellIndexHardeningIntegration/bounded-results.json:161` at `29bb6cb`).

The new preconditions add evidence and exclude no cases.

## 3. Gate evidence (`/media/jer/Files/CCF/Verification/S1D-T06-Unity-20261029/fix-51e/`)

Q1 and Q3 result files do not record `run_head`. I tied them to the exact source through `source_sha256`, checking it against `git show <sha>:<file> | sha256sum`:

| Gate | Evidence | Bound to | Result |
|---|---|---|---|
| Q1 event-ID | `q1/run-1-result.json` | sha `bd838f7f…` = event-ID file at `1a44526`, `d67581b`, `de21ad6` and `51e685e` (unchanged across them) | PASS, exit 0, `SAVE_EVENT_ID_VERIFY_PASS cases=19 rejected=9 accepted=10 canLoad=LoadData=Load sharedReason=exact diskBytes=unchanged noMutation=exact`; the duplicate-ID and zero/negative-ID cases REJECT with world and disk unchanged |
| Q2 save hardening | `q2/results-save-hardening.json` | `run_head` = `51e685e` | PASS, `PASS_CLEAN_SHUTDOWN`, no unexpected tree changes, no assertion failures |
| Q3 cell index | `q3/new-gate-results.json` | sha `c312fe2c…` = cell-index file at `51e685e` only | run-1 and run-2 both PASS, exit 0, `SAVE_CELL_INDEX_HARDENING_VERIFY_PASS cases=131 …` and `SAVE_CELL_STARTUP_PASS validContinue=enabled malformedContinue=disabled refusalLength=68 startNew=unchanged` |
| Q4 U4 core | `q4q5/run/results-core.json`, `core.markers.txt` | `run_head` = `51e685e` | PASS, `hard_checks=112`, no failures; `SYN duplicate event IDs`: base LoadData accepted, invalid edited save refused, year preserved, Scenario One/world snapshot preserved |
| Q5 compile | `q4q5/compile.log`, `summary.txt` | `summary.txt` compile step `head=51e685e`, exit 0 | 0 `error CS`; `Exiting batchmode successfully` with return code 0 |

**Consistency at `1a44526` (`fix-95/`):**
- `test-fix.diff` is byte-identical to `git diff 29bb6cb 1a44526`.
- Q1 at `1a44526` PASS with the same source sha `bd838f7f…` and the same marker.
- Q2 first attempt exited 198 (the licence issue). The retry PASSed (80.8 s per `q2-driver.out`).
- The lock was acquired by `ClaudeB-S1D-T06` at head `1a44526`.
- Only Q3-file commits (`d67581b`, `de21ad6`, `51e685e`) follow, so the `1a44526` Q1/Q2 results and the `51e685e` reruns agree.

## 4. Licence, lane and lock

- **Licence files unchanged.** The host licences still hash to the values recorded in `licences-unchanged.txt` (`~/.config/…` `24ebbb7a…`; Flatpak Hub `73380f65…`). Their mtimes (2026-09-15, 2026-10-26) predate this run. The per-gate `config/` directories show that runs used a throwaway `XDG_CONFIG_HOME` seeded with a copy of the Hub licence. I found no edit to global config.
- **U1 lane clean.** `/media/jer/ZX20/CCF-U1` HEAD is `51e685e` and `git status --short` is empty. No harness script or `.meta` is left under `Assets/ForestPrototype/` (only the pre-existing tracked verification scripts are there). The `.vscode/settings.json` Editor rewrite was captured in `u1-side-effects-final.patch` and is not present in the lane.
- **No Unity Editor running** (pgrep found no `-projectPath` process).

## 5. Findings

**Blocking:** none.

**Non-blocking:**
1. **Lock evidence for the `51e685e` reruns is missing from `fix-51e/`.** The archived acquire/release JSON covers Claude B's `29bb6cb` session (released, not forced, no Editor PIDs) and the `1a44526` acquire in `fix-95/`. There is no acquire/release record for the `DeepSeek-S1D-T06` session that the handoff describes. The live state (no Editor, clean lane at the exact SHA) is consistent with a normal release, but I could not read the lock record directly. If the manager needs an audit trail, ask Ops to add the release record or a status snapshot.
2. **Licence copies are archived in durable evidence.** `fix-51e/{q1,q2,q3,q4q5/run}/config/unity3d/Unity/licenses/UnityEntitlementLicense.xml` are copies of the live Hub entitlement (`73380f65…`). This edits no global file, but it does keep credential-like material in the evidence tree. Recommend that Ops prune those `config/` directories. They are not needed as gate evidence.
3. **The pristine-baseline change is defensive, not causal.** In the current harness `enlarged` is only ever read through `Clone(...)`, so `enlargedBaseline` is equal to it at use. The menu re-open in `51e685e` is the real Q3 fix (the handoff's ordering already says so). The change is harmless and keeps the assertion the same. No action needed.
4. **Q1/Q3 result JSON lacks `run_head`.** I bound them via `source_sha256`. Recording `run_head` in these drivers would make future reviews simpler. This is an Ops/tooling follow-up, not a change for this candidate.
5. **Host licence debt** (expired `~/.config` copy, exit 198) remains, as the handoff states. It is outside T-06 scope.

## 6. Disposition

- Production bytes: the candidate is byte-identical to `29bb6cb` for all production, save, schema, validation and migration files. `ForestSaveValidation.cs` = `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`.
- No-weakening: confirmed. Only fixture construction and explicit preconditions changed. Every event-ID, cell-index, save-hardening and U4 assertion is intact, and case counts match independent prior baselines.
- Integration: `51e685e` (production `29bb6cb` plus the test-only fixture fix) is acceptable as the T-06 qualification candidate. This review does not integrate it. Integration gates and coordination updates remain the manager's.

**S1-D T-06 REVIEW PASS — APPROVED FOR INTEGRATION**
