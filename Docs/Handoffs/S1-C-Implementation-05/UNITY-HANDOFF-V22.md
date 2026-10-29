# S1-C T-05 v22 Unity closure handoff — BLOCKED

**S1-C T-05 UNITY QUALIFICATION BLOCKED.** The runtime behaviour, migration, evidence-gap checks, rendered history and measured protected identities passed, but the unchanged objective harness has two refusal-message assertion failures and the unchanged completion harness has an invalid synthetic v14 fixture. No failed assertion is waived. No production defect was established; no production fix was made.

## Identity, authority and scope

- Worker: Unity qualification executor, OpenAI GPT-6.1 Sol, under parent #91 / T3 #106. The user records this as a capability escalation after Claude Sonnet was rate-limited before execution.
- Exact production candidate: **`4fb5ba99de52384932456d68da112432c899480a`**. T-05 production is the independently approved `2424f31` tree; `d5dbe97` is the existing verification-only pin relabel; candidate merges integration `22ba458`.
- Qualification branch/worktree: `qual/s1-c-t05-v22-closure-v1`, `/media/jer/Videos/CCF-S1C-T05Qual`. The new verification commit is a descendant of the candidate; the candidate itself was never amended.
- Canonical context observed in the candidate: `63e95ba2175f7e239a278c616681b286719821f8`. Read the required project workflow/authority/milestone/vision/overview and live checkout coordination record. The latest user packet explicitly grants this task's U1 ownership and execution order.
- Approved task authority: `specs/scenario-one-review-history-v1/`, T-05 / FR-10 / R-1; latest direct user qualification packet controls this run.
- Unity: **6000.6.0f1**, Linux graphical Editor, private Xephyr display `:84`, existing installed packages. No Windows build or Player build.
- U1: `/media/jer/ZX20/CCF-U1`, initially clean/detached at `217595feba6658a38c40f1eaacd54d1d6ff98973`; moved cleanly/detached to `4fb5ba99de52384932456d68da112432c899480a` and left there clean.
- Initial ZX20 storage preflight: approximately 21 GiB free, above the mandatory 15 GiB floor; no other Editor and lock free.
- Lock: **`731e5e098a7b486e843ba93726532e9d`**, owner `T3-S1C-T05`, task `S1-C-T05`, lane U1, branch `HEAD`, exact candidate head. Acquired 2026-10-29T12:21:27Z, ETA 16:30Z; **released normally**, no force, same owner/task/lock ID, no Editor remaining. See `lock-acquired.json` and `lock-release.json`.
- Evidence root: **`/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/`**. No new evidence/build/archive path was created on ZX20.
- Initial qualification status was not entirely clean: one untracked `Tools/Verification/S1CObjectiveHistoryUnity/S1CObjectiveHistoryUnityVerification.cs` was already present. It was inspected, preserved byte-for-byte and included in this verification-only commit. SHA-256: `2fd5a10b398a4587a08b34a485940cb2594a8bd79ca5ad584462ab9c77461144`. No authorship is inferred from its presence.

## Changes and safety

Only additive disposable tooling under `Tools/Verification/S1CObjectiveHistoryUnity/` and this handoff are committed. No production code, schema, validation, UI, version pin, existing assertion, frozen anchor or package was changed. Primary and other worktrees were not modified. Nothing was pushed.

The runner is modeled on the inspected cap/lesson runner and save-event-ID harness. It validates exact clean U1/head/lock ownership; stages inspected sources only under `Assets/ForestPrototype`; uses isolated XDG configuration; copies an existing licence solely for the run and removes it; records known VS Code/material import side effects, restores their original bytes; removes all staged sources and `.meta` files; checks host save digests and lane cleanliness. Later gates require the successful `01-compile` receipt for the same head and lock.

Historical reference/progress/completion/interaction/density fixtures explicitly select Legacy40 and a valid legacy rack model using the unchanged `TeachingCapGateSetup`. The objective test uses the real Enlarged80/Rack V2 Start New path. Model-1 completion uses the existing historical reconstruction, without changing assets on disk. New baseline DTO/hash types are archived pre-v22 source with type-name prefixes to coexist with current types; source provenance and an exact transformation audit are recorded. This is verification code, not an alternate production implementation.

## Receipts and actual results

Every run has `command.json`, `unity.log`, `markers.txt`, `process-result.json`, host-save digests and side-effect patches under its named evidence directory. Every receipt contains the exact command, candidate head, lock ID, exit, seconds, marker result, lane changes and cleanup result. All 13 receipts report harness/meta removal, copied-licence removal, unchanged host saves, no cleanup failure and no remaining lane changes. Isolated leftover saves from legacy protected runners were hashed in their receipts and removed to restore their initial absence.

| Evidence run | Exit | Seconds | Overall gate | Raw terminal evidence |
|---|---:|---:|---|---|
| 01-compile | 0 | 80.39 | PASS | `S1C_T05_COMPILE_PASS` |
| 02-objective | 1 | 247.06 | FAIL | `S1C_T05_UNITY_FAIL checks=542 failures=2` |
| 03-reference | 0 | 103.85 | PASS | `REFERENCE_FUTURE_V1_PASS` |
| 04-render | 0 | 112.55 | PASS | `T05_RENDER_PASS captures=6` |
| 05-hardening | 0 | 71.80 | PASS | `SAVE_HARDENING_VERIFY_PASS` |
| 06-progress | 0 | 66.72 | PASS | `SCENARIO_ONE_PROGRESS_VERIFY_PASS` |
| 07-completion | 1 | 77.14 | FAIL | `v14-shaped save rejected` |
| 08-completion-model1 | 1 | 67.07 | FAIL | `v14-shaped save rejected` |
| 09-n3 | 0 | 99.47 | PASS | `N3_U4_VERIFY_PASS` |
| 10-density-save | 0 | 92.22 | PASS | `DENSITY_QUAL_PASS geometry=L40 hardChecks=6` |
| 11-interaction | 0 | 103.07 | PASS | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` |
| 12-baseline-hash | 0 | 63.64 | PASS | `T05_BASELINE_HASH_PASS cases=30` |
| 13-completion-replay | 1 | 117.62 | FAIL | `v14-shaped save rejected` |

`all-gate-receipts.json` aggregates these unmodified receipt fields. No overall PASS is inferred from a partial marker.

## Objective history, real evaluator and real save paths

The main gate completed **542 passed checks, 2 failed checks, 34 cases, 12 initial captures**; all phases ran to completion. Its full raw result is `02-objective/evidence/t05-unity-result.txt`; extracted evidence is `objective-summary.json`.

- New game → real year-0 disk save → reload → year-1 `AdvanceYear`/`EvaluateProgress`: retained-canopy, continuous-canopy and regeneration first recorded at year 1. Year-0 baseline is never treated as an already evaluated year (B-1). Year-0 world hash before/after reload: `A0E0B2A51CC9274A`.
- A real commissioned thinning first records managed-opening at year 2; real purchased/designated/sheltered Oak planting first records introduced-sessile-oak at year 3. Recorded targets are 240, 0.35, 12, 1 and 1 respectively. Every round trip uses production capture/restore and preserves years and the exact achievement array. At least two objectives first reached in different years is satisfied (three distinct years measured).
- Every recorded first year is compared against the actual `ScenarioOneObjectives.Evaluate` applied to that year's saved snapshots, events and resolved orders. No cached teaching facts or private objective results are injected.
- `CanLoad`, typed `LoadData` and disk `Load` were exercised. Semantic refusals cover pre-v22 records, duplicate IDs, impossible years, unknown/empty IDs, year-order reversal and negative/NaN/infinite targets. Disk-only refusals cover absent/null/object/string history fields, missing or wrongly typed record fields, duplicate JSON keys and parse errors.
- Each refusal compares the live world hash plus full scenario JSON, year/tree/player state, records and exact disk bytes for `forest-save.json`, `.bak`, `.tmp`, including a delayed check. Refused fixture world hash remains `EF24863B112902AB`. All no-mutation checks passed. The isolated slot's three original files were restored byte-for-byte/absence; host save digests stayed unchanged.

### Genuine old saves and R-1

The fixture files were copied byte-for-byte from existing durable evidence; they were not made by lowering the current save's version. `fixtures/manifest.json` contains exact original paths and SHA-256 digests.

| Fixture | Save version | Ecological year | SHA-256 |
|---|---:|---:|---|
| v20-y5 | 20 | 5 | `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` |
| v21-y1 | 21 | 1 | `a3aa334ffa585cc6db1f908860aa382ce3ae73d1e3a89bd2da2975369c5ba4d7` |
| v21-y0-rack | 21 | 0 | `2b89ee6303a3fcad3b9f5331dd74c2dc879bd37fb3865b94b5762ebc6d5e465a` |

All genuine v20/v21 loads passed both typed and disk routes, migrated to an empty in-memory history, made no retrospective write, re-saved as v22 with an explicit empty list and retained the original bytes as `.bak`. Typed and disk routes agreed on world/scenario fingerprints and history sources, including after v22 reload.

The genuine v20 year-5 and v21 year-1 worlds each exactly derive retained-canopy, continuous-canopy, regeneration and managed-opening as year 1, labelled **derived from saved records**. The genuine v21 year-0 world has achieved current canopy thresholds but no evaluated-year provenance, so its years are **not recorded**. Totals: eight derived results and two achieved-but-not-recorded results across genuine saves.

For the evidence-gap regression, the genuine v20 year-5 save is damaged in two separate ways: remove the year-1 snapshot, or remove year-1 `YearAdvanced`. Both typed/disk routes return NotRecorded; two real year advances, a v22 re-save/reload, and another advance never stamp a later false first achievement. The real evaluator remains in use. This supplies additional achieved-but-not-recorded R-1 evidence through a v22 round trip.

## Hashes, raw JSON and protected anchors

- Unity `JsonUtility` current/legacy/v15/v16/v17/v18 hashing was exercised for synthetic legacy/default-rack/rack-state worlds and parsed v21 worlds (`02-objective/evidence/unity-hash-fixtures.txt`), as well as real v20/v21 loaded worlds.
- `12-baseline-hash` independently compares **30 cases** through Unity's serializer against actual pre-v22 DTO and archive hashing source from `aca74acacc565f72ab802177cf69ca6ebb54d637`. Every corresponding identity matched, with empty and populated objective records and explicit v20/v21 inputs. Raw tables: `12-baseline-hash/evidence/baseline-hashes.txt`; source hashes: tracked `baseline-provenance.json`; audit: `baseline-source-audit.txt`.
- The pre-v22 side uses its actual v21 writer header for new-writer fixture comparisons; persisted v20/v21 headers are retained. No expected anchor was relabelled or rebaselined.
- Main fixture raw v22 JSON with records differs from an otherwise identical v22 capture **only** in the `objectiveAchievements` array. Replacing that array with `[]` makes the strings identical; all archive hashes stay identical. The raw save-schema header also necessarily differs when comparing writers v20/v21 against v22; production archive identity normalization preserves the earlier hashed layout. Raw save JSON is therefore not asserted globally unchanged across schema versions.

| Protected measurement | Observed |
|---|---|
| Frozen schedule | `56C8B99FA1E8DDD1` |
| Frozen archive file FNV | `1A42C7BD0439E72F` |
| Frozen years 0 / 20 / 50 / 100 | `A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` / `7AD177B3CC2F73C7` |
| Reference continuation, two independent runs | `EBD49ADEF18C3F36`, v12 year 50 → v22 year 100; exact agreement |
| Canonical Sitka lifecycle | `BFC55473C1506067` |
| Model-1/model-2 neutral lifecycle, both | `7E57B9DAEF5BF4D0` |
| Model-1/model-2 normal-browse lifecycle, both | `35E2BF1C2F55C2DE` |
| Existing historical model-1 completion projection | `702766DECE591E21` (unchanged existing projection/anchor) |
| Current model-2 year-30 completion, two processes | `9C77EB4BEE427FB9` / `9C77EB4BEE427FB9` |
| Mixed disk-load continuation | `2A8003ABD1E4B810` uninterrupted = resumed |
| Dying mixed stand continuation | `F37DF35F6649D1C2` uninterrupted = restored; mortality sequence exact |

The actual frozen resources are byte-identical to pre-v22 `aca74ac` in Git and remained SHA-256-identical in U1 after all runs (`reference-resource-sha256.json`, `unchanged-sources.json`, `final-lane-before-release.json`). No measured protected anchor moved. Broader unrun suites are not claimed qualified.

### DensityQualification.cs:952

The unchanged runtime `ANCHORS,SAVELOAD` phases passed six hard checks. The full, unfiltered key-set comparison measured:

```text
SAVE_V22_MODEL2_MIXED_ADDS_NO_PERSISTED_FIELD version=22 keys(model1)=101 keys(model2)=101 onlyIn2=[] onlyIn1=[]
```

No key exclusion or assertion change was made. Unsupported growth models 3/99 refused without mutation; the real disk round trip and dying-stand continuation reproduced exact hashes/mortality. The existing best-effort newer-version probe accepted v23, as its existing diagnostic expects.

## Rendered Annual Review and inspection

The initial 12 captures are retained but **not accepted as history-text evidence**: the unchanged harness scrolled only the heading into view, and some shots had a load toast. The added supplemental render gate uses the real `AnnualReviewView`, production silent typed loading, scrolls the whole objective card, and asserts that every history line is fully inside the viewport and its measured text height fits. It passed with six PNGs; all six were directly inspected.

- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-recorded-objectives-1280x720.png`
- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-derived-objectives-1280x720.png`
- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-not-recorded-objectives-1280x720.png`
- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-recorded-objectives-1920x1080.png`
- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-derived-objectives-1920x1080.png`
- `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render/evidence/shots/review-not-recorded-objectives-1920x1080.png`

At both resolutions, the recorded card visibly shows years 1/2/3 with `(recorded)`; the genuine old save shows year 1 with `(derived from saved records)`; the evidence-gap world shows `Year reached: not recorded` and no invented year. No history line is clipped and no toast covers the card. The full inspection note is `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/render-inspection.md`.

## Exact blockers — no assertions changed

1. **Main harness v20 refusal wording.** It lowers the recorded V2 world's header to v20 while retaining Rack V2 state. Actual rejection is `Save not loaded: a save older than version 21 cannot carry Rack Planning & Field Marking V2 state.` Its existing assertion instead wants `older than version 22`. The v21-with-records fixture does directly reach the objective-history refusal. All routes refused the v20 fixture and all mutation checks passed.
2. **Main harness duplicate-key wording.** Actual rejection is `Save refused: invalid stand geometry JSON`; the assertion wants `invalid version 22 objective history JSON`. `TryReadSave` calls strict geometry JSON validation first, and that parser rejects duplicate keys anywhere in the document. CanLoad/Load agree and no world/disk byte changes. This is not evidence of an accepted duplicate key.
3. **Existing completion P10 synthetic v14 fixture.** It parses a recorded v22 `midJson`, assigns `version=14`, and retains nonempty `objectiveAchievements`. Production correctly rejects it: `a save older than version 22 cannot carry objective achievement history`. Both growth models reach P0–P9 and their hash checks first; then overall gate exits 1 at `v14-shaped save rejected`. The second model-2 process reproduces the same year-30 hash and the same fixture failure. P10's later checks and P11 are not reached.

These are verification fixture/expected-message blockers, not demonstrated production failures. The user forbids modifying existing assertions/version pins and restricts changes to this tooling subtree, so the executor did not repair the existing protected suites or waive their failures. A separately authorised, reviewed verification correction is needed before rerunning the failed gates and declaring closure. No production fix is recommended from this evidence.

## Not run / not claimed

- Full completion PASS remains **not achieved**; the P10 remainder and P11 were not run after the fixture refusal. Model-1's historical anchor and model-2's two-process hash agreement are partial results only.
- The broad `CCFIntegrationVerificationTemp` runner was not run. Its old full interaction fixture/configuration is not substituted for the explicitly configured canonical lifecycle in the existing interaction gate; that gate and density lifecycle anchors were run instead.
- Density phases other than `ANCHORS,SAVELOAD` were not run: RATIOS, ROLLS, SITKA100, SITKACOMP, PARITY, CURVE, FAIRNESS, FAIRNESSHIGH, CONTINUITY, FINALBROADLEAF, LATEMIX, PERF, LONGRUN. No performance or whole-density-programme qualification is claimed.
- The other pin-relabel paths below were not executed in this closure chain (including their offline runners). They are explicitly listed, not silently treated as covered:

```text
Tools/Verification/Enlarged80Calibration/AreaCalibrationVerification.cs
Tools/Verification/ModelDefault/ModelDefaultVerification.cs
Tools/Verification/RackV2FieldMarking/RackV2FieldMarkingVerification.cs
Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs
Tools/Verification/RackV2Save/RackV2SaveVerification.cs
Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs
Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs
Tools/Verification/ScenarioOneRackUx/RackUxRack.cs
Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs
Tools/Verification/ScenarioOneTeachingCopyVerification.cs
Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs
Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs
Tools/Verification/SiteInfrastructure/run_site_check.py
Tools/Verification/SitkaGrowthMortality/SitkaGrowthModelVerification.cs
Tools/Verification/StandGeometry/Enlarged80Verification.cs
Tools/Verification/StandGeometry/StandGeometryVerification.cs
Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs
Tools/Verification/ThreeSpecies/MixedDensityVerification.cs
Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs
Tools/Verification/U4/U4Qualification.cs
Tools/Verification/U4Guidance/U4GuidanceSmoke.cs
Tools/Verification/UnderstoreyRecruitment/Model2Verification.cs
Tools/Verification/WindthrowV1/StormCoreVerification.cs
```

- No complete milestone Tier-3 sweep, native-input playthrough, Player/platform qualification, personal playtest, Windows build, independent G2/G3 acceptance or G4 integration was performed.
- No push, GitHub publication, live coordination/dashboard update or publisher synchronization was performed; the user expressly requested a local commit without push and restricted owned files. Parent #91 / T3 #106 owns publication, dashboard synchronization and review disposition. Local handoff publication does not claim remote availability.

## Commands and rerun guidance

All actual Editor commands are in each `command.json` and the receipts; exact driver invocations are archived in `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/commands.txt` and reproduced below. These are historical commands using a now-released lock and used output directories. To rerun, acquire a new matching lock normally, use new empty output directories under Files, run `compile` first at `01-compile`, and supply the new lock ID. Never reuse the released lock or overwrite evidence.

```bash
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/01-compile --gate compile
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/02-objective --gate objective --fixtures /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/03-reference --gate reference
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/04-render --gate render --fixtures /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/05-hardening --gate hardening
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/06-progress --gate progress
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/07-completion --gate completion
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/08-completion-model1 --gate completion-model1
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/09-n3 --gate n3
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/10-density-save --gate density-save
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/11-interaction --gate interaction
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/12-baseline-hash --gate baseline-hash
python3 /media/jer/Videos/CCF-S1C-T05Qual/Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head 4fb5ba99de52384932456d68da112432c899480a --lock-id 731e5e098a7b486e843ba93726532e9d --out /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/13-completion-replay --gate completion
```

Expected observable behaviour: new game → year-0 save/load → year 1 records canopy retention as year 1; commissioning thinning in the next year records year 2; save/load preserves both. Loading genuine v20/v21 saves keeps the stored list empty and displays an exact derived year only where annual evidence is complete. Removing year-1 snapshot or YearAdvanced produces not-recorded wording through later advances and reloads. Opening Annual Review and scrolling the objectives card shows the phrases captured above.

## Final state and next action

All staged verification sources/metas were removed after each run. Known Editor import side effects were restored byte-for-byte. All host saves and frozen resources are unchanged. U1 is clean/detached at the exact candidate, and the machine lock is normally released. Qualification changes are committed locally on the qualification branch; no push and no production integration.

**Verdict: S1-C T-05 UNITY QUALIFICATION BLOCKED by the three verification issues above.** Evidence is ready for Manager/G3 review and a bounded verification-only correction. The candidate's successful behavioural and anchor measurements are retained without converting failed gates into a PASS.
