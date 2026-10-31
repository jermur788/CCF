# S1-C T-08 — Review / Management History Feature Qualification — HANDOFF

**Claim:** #206 (under local orchestration #203/#201; parent #91)
**Result token:** `S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`
**Scope:** Tier-2 qualification only. No production file changed. Awaiting browser/Overall Manager `APPROVED / NEEDS CHANGES / REJECTED` (not self-issued).

## Identity

- Worker: Anthropic Claude Sonnet 5.5 (Medium), the only local worker. DeepSeek orchestrates; it did not execute.
- **Exact production candidate: `34e360f85835afd15f5323f287372e0f2ee83dd9`** = `origin/integration/scenario-one-player-test-ready-v1` (re-verified live; `origin/main` was `c76b2da8`). Ancestry of the candidate (verified `merge-base --is-ancestor`): S1-C T-01 `808d8165`, T-02 `1a91f1c6`, T-03 `50a0c38c`, T-04 `3ea4af8b`, T-05 `2424f316` (+ v22 closure `b3b47649`), T-06 `5a0fc519`, rendered qual `51d4fc53`. T-07 (`verify/s1-c-t07-data-contract-v1`, offline verification only) is deliberately not production and is not an ancestor; its tooling was copied in.
- Branch/worktree: `qual/s1-c-review-history-v1` @ `/media/jer/ZX20/CCF-U1`.
- Verification commits on top of the candidate (production bytes unchanged — `git diff 34e360f8 HEAD -- Assets Packages ProjectSettings .vscode` is empty):
  - `d0689034` — T-07 offline tooling (from `e64b337e`), T-08 runner, T-05 harness version pins 22 → 24;
  - `f3470edb` — runner fix: t02–t06 gates must not inherit `CCF_TEST_DIRECT_ENTRY`;
  - `e0f0437c` — `S1CHashBaselineGate` reconciliation (see Deviations).
- **Accepted current schema: `ForestSaveData.CurrentVersion == 24`** (inspected in `Assets/ForestPrototype/ForestSaveData.cs:7`; asserted exactly by the objective gate `A0_CURRENT_VERSION_24`).
- Unity 6000.6.0f1, Linux Editor, private Xephyr `:84`, isolated XDG config, `TMPDIR=/tmp/`. No Windows/Player build.
- Durable evidence: `/media/jer/Files/CCF/Verification/S1C-T08-20261031/` (nothing new on ZX20). Per-gate `command.json`, `unity.log`, `process-result.json`, evidence folder; aggregate `all-gate-receipts.json`.

## Lock and cleanliness receipts

| Lock ID | Head | Acquired (UTC) | Released | Use |
|---|---|---|---|---|
| `802d7312961d48f782dfe333df095772` | `d0689034` | 18:34:58 | normal, same owner/task/ID (`attempt1-head-d0689034/lock-release-1.json`) | compile, objective, render PASS; T-02 FAILED on runner bug |
| `f098ea74e7a645cc8044583cbe90eebd` | `f3470edb` | 18:46:59 | normal (`lock-release-2.json`) | full chain below |
| `8440d0581b7d4fd09d89583d154c5503` | `e0f0437c` | 19:18:53 | normal (`lock-release-final.json`) | compile + baseline-hash |

All three: owner `Claude-S1C-T08`, task `S1-C-T08`, lane U1, branch `qual/s1-c-review-history-v1`; no force release. Fresh preflight before each acquisition: lane clean at exact head, no Editor/Xephyr, lock free, ZX20 ≈ 18 GiB free (floor 15 GiB), `:84` unoccupied.
Every gate receipt: harness + `.meta` removed, copied licence removed, host save digests unchanged, no isolated save residue, `remaining_lane_changes == ""`. Known `.vscode/settings.json` and SectionFive material Editor side effects were restored byte-exactly (patches kept in each gate folder). One orphaned Unity Roslyn compiler server (`VBCSCompiler`, spawned by my runs) was terminated at close. Final state: U1 clean, no Editor/Xephyr/openbox, lock free, `~/.config/unity3d/DefaultCompany/CCF/forest-save.json` untouched.

## Commands and results

All via `Tools/Verification/S1CReviewHistoryQualification/run_t08_gate.py` (derived from the accepted T-05 runner). It requires lock `status == held` with `task_id == S1-C-T08`, the supplied owner and lock ID, lane == head == expected head, clean lane, no Editor; it refuses a missing/non-matching compile receipt, an existing staged harness, an occupied display, and evidence outside `Files/CCF/Verification`. Invocation pattern: `run_t08_gate.py --out <dir> --lock-id <id> --owner Claude-S1C-T08 --expected-head <sha> --gate <gate> --fixtures <fixtures>`.

Offline (no Unity, no lock): `run_t07_gate.py --old-save v20-y5.json --current-save recorded-v22-year3-save.json --mutations` (45.5 s) → `S1C_T07_OFFLINE_PASS checks=304 failures=0 mutations=8 caught=8`, built from the **v24 production** history reader, records, migration and `ForestSaveValidation`.

Unity gates (head column = verification head the receipt was produced at):

| # | Gate | Head | Unity exit | Seconds | Marker (raw) |
|---|---|---|---:|---:|---|
| 01 | compile | `f3470edb` / `e0f0437c` | 0 / 0 | 84.7 / 104.5 | `S1C_T05_COMPILE_PASS Unity=6000.6.0f1` |
| 02 | objective record/save/old-save | `f3470edb` | 0 | 300.8 | `S1C_T05_UNITY_PASS checks=544 cases=34 shots=12` |
| 03 | rendered objective history | `f3470edb` | 0 | 144.1 | `T05_RENDER_PASS captures=6` |
| 04 | T-02 year browsing | `f3470edb` | 0 | 154.8 | `S1C_T02_HARNESS_PASS checks=246 shots=12` |
| 05 | T-03 drill-down | `f3470edb` | 0 | 180.4 | `S1C_T03_HARNESS_PASS checks=279 shots=16` |
| 06 | T-04 provenance | `f3470edb` | 0 | 179.1 | `S1C_T04_HARNESS_PASS checks=529 shots=30` |
| 07 | T-06 guidance | `f3470edb` | 0 | 104.3 | `S1C_T06_HARNESS_PASS checks=170 shots=8` |
| 08 | Reference Future v1 | `f3470edb` | 0 | 104.0 | `REFERENCE_FUTURE_V1_PASS` |
| 09 | SaveHardening | `f3470edb` | 0 | 85.3 | `SAVE_HARDENING_VERIFY_PASS` |
| 10 | ScenarioOneProgress | `f3470edb` | 0 | 73.0 | `SCENARIO_ONE_PROGRESS_VERIFY_PASS` |
| 11 | baseline-hash (30 cases) | `e0f0437c` | 0 | 118.8 | `T05_BASELINE_HASH_PASS cases=30 …noAnchorsRelabelled=true` |
| 12 | interaction (canonical Sitka) | `f3470edb` | 0 | 95.4 | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` |
| 13 | density-save `ANCHORS,SAVELOAD` | `f3470edb` | 0 | 92.1 | `DENSITY_QUAL_PASS geometry=L40 hardChecks=6` |
| 14 | completion (model 2) | `f3470edb` | 0 | 91.4 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` |

Zero `CHECK_FAIL`/`error CS` lines in any final receipt. No assertion was weakened or deleted. Gates 02–10, 12–14 ran at `f3470edb`; compile and gate 11 ran at the final `e0f0437c`. The only files that differ between those two heads are `S1CHashBaselineGate.cs` and the runner (one added env pass-through); every other harness's SHA-256 is recorded in its `command.json` and is identical.

## Current-schema and old-save identities

- Current-schema save/load round trip: gate 02 (v24 disk save at year 0 and year 3, typed and disk routes agree, bytes unchanged on accept/refusal, 24 explicit refusal cases with unchanged world hash `EF24863B112902AB`). The year-3 recorded v24 save it produced is `02-objective/evidence/recorded-v22-year3-save.json` (name is historical; contents `version: 24`), SHA-256 `3e3a495642eefb7ef532954fc060d7c814a2fbd0e2f2b636e6d06fd1dcd1ac41`.
- **Genuine old save: v20-y5, SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`** (verified before use; the rejected synthetic `v20-records` was not used). Loaded through typed and disk routes on v24, migrated to an empty in-memory history, no retrospective write, re-saved as v24 with an explicit empty list: retained-canopy, continuous-canopy, regeneration and managed-opening are **`Derived:1`**, the other four stay unachieved. Also loaded: v21-y1 (same four Derived:1) and v21-y0-rack (`NotRecorded` for the evidence-less pair). v20-y5 was also rendered in gate 06 (`old-recorded/derived/notrecorded` shots) and gate 07.
- Pinned T-07/T-04/T-06 fixtures (SHA-256 verified): `recorded-v22-year3-save.json` `95e5dd37d7f546d78cb94cda8e5af1641b576c925cc37313ce88aa0a54167b9e` (genuine v22 save, loaded and migrated to v24 by gates 04–07), `render-gap-v20-y5-no-year1-snapshot.json` `3620c6de4919ab6ad81a038eb00c18df7dec5501f6d94b75f20933330d5e8ce9`, v21-y1 `a3aa334ffa585cc6db1f908860aa382ce3ae73d1e3a89bd2da2975369c5ba4d7`, v21-y0-rack `2b89ee6303a3fcad3b9f5331dd74c2dc879bd37fb3865b94b5762ebc6d5e465a`.

## Protected / cross-feature results (all unchanged)

- Frozen Reference Future v1: schedule `56C8B99FA1E8DDD1`, archive file FNV `1A42C7BD0439E72F`; years 0/20/50/100 = `A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` / `7AD177B3CC2F73C7` (`pinned=True`); continuation v12 year 50 → v24 year 100 = `EBD49ADEF18C3F36`. Frozen resource bytes untouched (clean tree).
- Legacy40 / pre-feature: canonical Sitka `BFC55473C1506067`; neutral/normal lifecycle `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`; mixed continuation `2A8003ABD1E4B810`; dying stand `F37DF35F6649D1C2`; model-2 completion `9C77EB4BEE427FB9`. No anchor or hash was rebaselined.
- Gate 11: all 30 world-identity cases reproduce the accepted pre-v22 (`aca74ac`) identities exactly (see Deviations for how the comparison was made).
- S1-D event-ID hardening: T-07 offline gate 304/304 on v24 production, including production `ForestSaveValidation` rejecting duplicate/non-positive management-event IDs and never renumbering; 8/8 mutants caught.
- MFM provenance: the history reader/UI contain no prescription code path (grep: 0 references), so history neither displays nor invents prescription provenance; T-07 asserts the raw v23 prescription fields on events/orders pass through the read path unchanged. No MFM teaching/Work Plan work was done.

## Rendered capture manifest and inspection

All gates captured at 1280×720 and 1920×1080 (harness asserts capture size, non-blank, text inside panel, no clipping): 12 + 6 (objective/render), 12 (T-02), 16 (T-03), 30 (T-04), 8 (T-06) = 84 PNGs under `<gate>/evidence/shots/`.
**Directly inspected by me (visually):** `04-t02/year1-earlier-top-1280x720`; `05-t03/harvest-expanded-1920x1080`, `cancelled-expanded-1280x720`; `06-t04/old-derived-objectives-1280x720`, `old-notrecorded-objectives-1920x1080`, `year3-earlier-storm-1280x720`, `year4-not-recorded-1920x1080`; `07-t06/planting-guidance-missing-location-1280x720`, `felling-guidance-recorded-area-1920x1080`, `recorded-area-waypoint-hud-1280x720`. In these: all six provenance badges are legible and used honestly (RECORDED/DERIVED/CURRENT/RECONSTRUCTED/NOT RECORDED, session-only line asserted by gate 06); grouped harvest shows one job settlement with the text "No money is recorded for a single order or tree"; the no-work year reads "No work was resolved this year"; the cancelled order says "execution method not recorded"; the genuine v20 save shows "First reached in year 1 (derived from saved records)" and "Year reached: not recorded"; guidance names the recorded square with a waypoint button, and for the missing-location planting says "Map square not recorded; no inspection waypoint offered"; nav buttons stay visible; no blocking clipping/overflow seen. **The remaining ~74 captures were not individually viewed**; they are covered only by the harnesses' in-engine fit/clipping assertions, which all passed. (Note: the first-year view at 1280×720 truncates the lowest "Retained material" row at the scroll edge, which is ordinary scrolled-card behaviour, not clipped content.)

## Requirements matrix (FR / AC) — evidence: gate numbers above

| Item | Result | Evidence |
|---|---|---|
| FR-1 year-browsable, no second system | PASS | 04: all completed years 4,3,2,1 visited, latest preserved; 05/06 five-year fixtures; offline T-01 reader is the only query authority |
| FR-2 intervention record | PASS | 05: felling/planting/cancelled/failed expanded rows (type, year, target, method, result, status) + shots |
| FR-3 year record + snapshot delta | PASS | 04/05/06 Forest card "Canopy 0.98 → 0.97" etc. (DERIVED) |
| FR-4 provenance fidelity / not recorded / R-1 | PASS | 06 (529 checks), 02 old-save R-1 (Derived:1 ×4 for v20-y5/v21-y1, NotRecorded for v21-y0 and evidence-gap saves) |
| FR-5 layers labelled everywhere | PASS | 06 one example per layer (`STORM_CARD_CLASSES… reconstructed=2 recorded=1 current=1 derived=1`) |
| FR-6 no unearned causality | PASS | 06/07 wording checks; inspected copy: "An opening creates an opportunity, not guaranteed regeneration"; "A planting record does not establish survival" |
| FR-7 bounded guidance + waypoint | PASS | 07 (170): recorded-area waypoint = exact recorded cell centre; missing location → no waypoint |
| FR-8 aggregate-first drill-down (R-3) | PASS | 05: collapsed aggregate, "Show recorded details" |
| FR-9 grouped harvest job-level money | PASS | 05: `TWELVE_ORDERS_ONE_REAL_JOB`, `ONE_SETTLEMENT`, `NO_PER_ORDER_HARVEST_MONEY`, `NO_SETTLEMENT_NON_HARVEST_YEAR`; T-07 `harvestJobId` is a year label only |
| FR-10 objective history, outcomeYear | PASS | 02 (544): recorded years 1/2/3 recompute against the real evaluator, survive v24 save/load; 03 render |
| FR-11 storms | PASS | 06 storm card: recorded 1 / reconstructed 2 / game bands wording |
| FR-12 append-only | PASS | 02 (no retro-writes; re-save only adds explicit empty list), 07/T-07 identity and retention, `WORLD_SAVE_COUNTS_ACK_UNCHANGED_*` throughout |
| FR-13 session-only | PASS | 06 `SESSION_ONLY_BROWSING_PRESENT/WORDING` at both resolutions |
| FR-14 S1-D contract | PASS | T-07 304/304, 8/8 mutants |
| FR-15 bounded UI | PASS | inspected shots: simple trend strips only, no grids/export/scoreboards |
| FR-16 independence from Rack V2/MFM | PASS | reader has no prescription/rack dependency; T-07 builds it standalone on the plain .NET SDK |
| AC-1 | PASS | gates 04–06 |
| AC-2 ≥3 advanced years, earlier/return, two resolutions | PASS | 04 (count=4, `BACK_ON_LATEST_AFTER_STEPPING_BACK`), 05/06 (5 years) |
| AC-3 one rendered example per layer | PASS | 06 + inspected shots |
| AC-4 genuine v20 no invented history | PASS | 02 + 06 (v20-y5 derived/not-recorded, rendered) |
| AC-5 guidance on ≥2 interventions | PASS | 07: felling (recorded-area waypoint) and planting (missing-location, none offered) |
| AC-6 objective years survive save/load | PASS | 02 |
| AC-7 no authoritative per-order grouped money | PASS | 05, T-07 |
| AC-8 opening history mutates nothing; round trip stable | PASS | `WORLD_SAVE_COUNTS_ACK_UNCHANGED_*` in 04–07; 02 disk-bytes-unchanged |
| AC-9 old-save and Reference integrity | PASS | 02, 08–14 |
| AC-10 S1-D contract | PASS | T-07 |
| AC-11 no analytics surface | PASS | inspected shots |
| Annual Review acknowledgement unchanged | PASS | 04: `ADVANCE_BLOCKED_WHILE_REVIEW_UNACKNOWLEDGED_OLDER_VIEW`, `OLDER_YEAR_ACK_CALLBACK_DOES_NOTHING`, 05 `LATEST_ACK_EXACTLY_ONE_EVENT` |
| Spec R-1…R-4 | PASS | R-1 (02/06), R-2 (06), R-3 (05); R-4 not separately exercised beyond the T-07 contract checks (spec §9/§15 note read, not independently re-derived) |
| Save impact | PASS with note | History added only `objectiveAchievements` (v22). Current v24 additionally carries other accepted features' fields (MFM-06 v23 prescription state, teaching v24 `firstGuidedRackReviewComplete`), which history neither reads nor needs; their hash-identity stripping is exercised by gates 08–14 |

## Deviations and honest notes

1. **Runner bug, one retry.** The first T-02 run (lock `802d…`, head `d0689034`) failed with `the startup screen did not open` because my runner left `CCF_TEST_DIRECT_ENTRY=1` set for t02–t06. Classified as a runner issue (no production involvement); fixed in `f3470edb`; the lock was released and re-acquired for that head and the chain was rerun from compile. Failed attempt preserved in `attempt1-head-d0689034/`.
2. **Stale baseline-hash gate, reconciled verification-only.** Gate 11's original live comparison failed at `e0f0437c`'s predecessor (`11-baseline-hash-attempt1-stale-live-derivation`): `legacy40-default` candidate hashes were **byte-identical to the T-05-accepted values (`47B9EB27…`)**, but the live "pre-v22 baseline" column moved, because `PreV22SaveTypes` shares the current work-order/event types, which now serialise MFM-06 prescription fields the frozen pre-v22 hashing code cannot strip. This is the verification derivation going stale, not a production hash change. Reconciliation: the gate now (when `CCF_T08_ACCEPTED_BASELINE` is supplied) requires the candidate to equal the accepted T-05 table (30 rows, SHA-256 `81c6f13ede7c746f592c9e1776ed2da11382e46e184c6d790caabc9659daf2cd`, produced from the real `aca74ac` source, each row candidate == baseline), fails on missing/extra rows, and logs the stale live value for transparency. Strength is preserved (exact equality with the original pre-v22 identities, 30/30). Reviewer may prefer to confirm this against MFM-06's own base-vs-candidate hash-dump review (identical dumps).
3. **Version-pin reconciliation.** Five T-05 assertions pinned to 22 now pin the exact current schema 24 (`ForestSaveData.CurrentVersion == 24`, disk/migrated/re-saved version 24). The refusal text "older than version 22" is production wording and unchanged.
4. **Not one chain at a single head** (see note under the gate table); only the baseline gate file and a runner env line differ between `f3470edb` and `e0f0437c`, and compile was re-run at the final head.
5. Fixture file `recorded-v22-year3-save.json` keeps its historical name though the gate-02 regeneration is now a v24 save.
6. **Unrun/not claimed:** ~74 captures not individually viewed; `n3`, `completion-model1` and `completion` replay gates (available in the runner) were not run — only protected gates in the packet's scope plus interaction/density/completion; Tier-3 sweep, native-input/personal playtest, Windows/Player build — not run, not authorised.
7. Did not edit `active-tasks.md`; did not touch #199/#207 or S1-D T-08.

## Result

**S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED** (local Tier-2 result; Manager verdict pending). No production defect appeared.
