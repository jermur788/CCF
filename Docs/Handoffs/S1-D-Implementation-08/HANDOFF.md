# S1-D T-08 — Multi-cycle Integrated Witness — HANDOFF

**Claim:** #208 (parent #91; local orchestration #201/#203; Manager #200). **Task:** S1-D T-08, qualification only.
**Result token:** `S1-D T-08 MULTI-CYCLE WITNESS PASS — READY FOR INDEPENDENT SPEC REVIEW`
**Scope:** no production file changed. Awaiting the separate T-09 independent conformance review; this is not self-accepted as S1-D conformance.

## 1. Identity and ancestry

- Worker: Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), sole Unity execution worker. DeepSeek orchestrates; no subagent was spawned.
- **Exact production candidate: `34e360f85835afd15f5323f287372e0f2ee83dd9`** = `origin/integration/scenario-one-player-test-ready-v1` (re-verified; `origin/main` was `adeaa008`). Ancestry (`git merge-base --is-ancestor … 34e360f8`, all true): S1-D T-05 production `4ddf4aa5` and integration merge `46fc6d55`, handoff successor `ce673714`, T-04/#143 candidate `c838aca0`, S1-C T-01…T-06 `808d8165`, `1a91f1c6`, `50a0c38c`, `3ea4af8b`, `2424f316`, `5a0fc519`, `b3b47649`, rendered qual `51d4fc53`.
- Lane/branch: `/media/jer/ZX20/CCF-U1`, `verify/s1-d-multicycle-witness-v1`, started at `34e360f8`. Packet: `TASK-PACKET.md` + `REPIN-20261031.md` + `RELEASE-20261031.md`.
- Verification commits on top of the candidate (all under `Tools/Verification/`; this handoff is the only `Docs/` addition):

| Commit | What |
|---|---|
| `4da34797`, `49f6ed6a`, `96ceae7d` | cherry-picks of the three accepted S1-C T-08 verification commits (`d0689034`, `f3470edb`, `e0f0437c`): T-07 offline tooling, S1-C runner, v22→v24 pin reconciliation, direct-entry fix, baseline-hash accepted-table comparison |
| `2b6f0bc5` | new T-08 runner (`S1DMultiCycleWitness/run_t08_gate.py`) with the S1-D protected gates; stages the T-05 gate source (`643bb390`) and the T-04 guide harness (r3 from the T-04 evidence driver) |
| `b7e3a4a7` | the witness gate `S1DMultiCycleWitnessGate.cs` |
| `0dfa7ea2` | witness: retry-then-stop on blank screen capture; runner: Editor still alive after its shutdown log line is killed and recorded as hung |
| `8671c413` | cross-session resume probe `S1DResumeProbeGate.cs` + runner gate `resume` |
| `c7516697` | T-06 offline event-ID check: version pin `23` → accepted `24` (two assertions, same reconciliation S1-C T-08 made; nothing deleted/loosened) |

## 2. Lane / Unity / lock receipts

Unity 6000.6.0f1 Linux Editor, private Xephyr `:84`, isolated `XDG_CONFIG_HOME`, `TMPDIR=/tmp/`, Hub licence copied per run and removed (no licence file anywhere under the evidence root), no Windows/Player build. ZX20 free 18 GiB (floor 15 GiB) throughout. Evidence: `/media/jer/Files/CCF/Verification/S1D-T08-20261031/` (nothing durable on ZX20).

Every Unity gate receipt (`process-result.json`) records: exact HEAD, lock ID, Unity exit, seconds, pass marker, `harness_removed`, `licence_copy_removed`, `host_saves_unchanged`, `remaining_lane_changes == ""`. The runner fails closed unless the lock is held by this owner/task/id at the exact lane HEAD, the lane is clean, no Editor runs and `:84` is free.

| Lock ID | Owner / task | Lane HEAD | Acquired (UTC) | Release |
|---|---|---|---|---|
| `266e8f7f42724f05a333a28544184ebe` | Claude-S1D-T08 / S1-D-T08 | `34e360f8` (given) | 19:46:31 | normal, `locks/01-release-34e360f.json` |
| `67534c1882ed4567b6551707641b70a4` | same | `2b6f0bc5` | 19:56:19 | normal, `locks/03-release.json` |
| `b22f37f8b5f7403aa342c8884997051e` | same | `b7e3a4a7` | 20:22:36 | normal, `locks/04-release.json` |
| `6f2f34ceab95405c839a373c15e00efa` | same | `0dfa7ea2` | 20:33:22 | normal, `locks/05-release.json` |
| `7e9a4d5f62c241a18ba7fa6f5a56544a` | same | `8671c413` | 22:01:04 | normal, `locks/06-release.json` |
| `db7da321b65549a88667a40e358e64aa` | same | `c7516697` | 22:05:18 | normal, `locks/07-release-final.json` (same owner/task/id; `unity-lock status` afterwards: `free`, no Editor pids) |

No force release was used. Each re-lock released then re-acquired under the same owner/task, after verifying the lane was clean.

## 3. Production-byte proof

`git diff 34e360f85835afd15f5323f287372e0f2ee83dd9 -- Assets Packages ProjectSettings .vscode` is **empty** (0 bytes) at every verification head. `git diff --name-only 34e360f8 HEAD` lists only files under `Tools/Verification/` plus this handoff. Known Editor import side effects (`.vscode/settings.json`, SectionFive `.mat`) were restored byte-exactly after every gate; the patches are kept in each gate folder (`editor-side-effects.patch`, `material-import-side-effects.patch`). Host save `~/.config/unity3d/DefaultCompany/CCF/forest-save.json` SHA-256 `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075` was identical before and after every gate and at close.

## 4. What the witness does (reproducible)

`Tools/Verification/S1DMultiCycleWitness/S1DMultiCycleWitnessGate.cs`, staged and removed by `run_t08_gate.py --gate witness` (harness SHA-256 `3f15f6a6933a015821cae8b0195ec0967916e55f228adec06de65d16cda031ed`). One **production Enlarged80 / Rack Planning V2** forest, new game, direct entry, managed for 26 years through the **real** UI actions (`ui.ApproveWorkPlan`, `ui.AdvanceFromWorkPlan`, `ui.AcknowledgeAnnualReview`, `ui.TryShowReviewYear`), the real manager calls the field tools use (`AddRack`, `MarkRackEntrance`, `CreatePrescription`, `ApprovePrescription`), queued keyboard input for the first guided rack review, and the real `ForestSaveController.Save()/CanLoad()/Load()` disk path. Deterministic setup is limited to *which* trees are marked / where saplings are planted. No final completion predicate, order status, year stamp or cash value is set anywhere.

Reproduce: `python3 -I -B Tools/Verification/S1DMultiCycleWitness/run_t08_gate.py --out <dir under Files/CCF/Verification> --lock-id <held id> --owner Claude-S1D-T08 --expected-head <HEAD> --gate witness --fixtures <fixtures>` (compile gate first at `<dir parent>/01-compile`).

Each of the **15 disk checkpoints** performs: fingerprint → `Save()` → copy to evidence → fingerprint again (save is non-mutating) → `CanLoad()` (read-only) → `Load()` → fingerprint equal (chronology, event IDs, orders incl. `createdYear`, history years, cycle state, objectives, scenario JSON hash, world hash, latch, guide index) → ten refreshes write nothing → re-save is **byte-identical**.

## 5. Cycle table

Ecology year, guide/latch, ordinary work, history, checkpoint, cash and completion. Work group columns are read **after** the cycle's advance (so they are empty by design); the pre-approval/carried states are in the second table. Cash in euro cents. The exact table is `final/02-witness/evidence/cycle-table.md` / `.json`.

| # | Stage (cycle) | Year | Guide (index) | Latch | Work approved this cycle | Executed this cycle | Review acked / history year viewed | Checkpoint (raw SHA-256 12) | Cash before → after | Completion |
|---|---|---:|---|---|---|---|---|---|---|---|
| 1 | cycle 1 — rack access (5 racks, entrances marked, ordinary approval) | 1 | future-trees (1) | false | n=149 [#1–149] | n=149 | 1 / 1 | cycle1-access-reviewed `a22bcc1b7e14` | 1,200,000 → 1,022,554 | Active |
| 2 | cycle 2 — release (18 felling orders, sold + deadwood); first guided rack review earns the latch | 2 | response (4) → **recruitment (5)** | false → **true** | n=18 [#150–167] | n=18 | 2 / 1 | cycle2-latch-earned `d5550b07d629` | 1,022,554 → 777,533 | Active |
| 3 | idle year; MFM work carried over | 3 | recruitment (5) | true | – | – | 3 / 2 | year3-carried-mfm-idle `da98d7105546` | 777,533 → 777,533 | Active |
| 4 | cycle 3 — carried MFM (86) + new planting (16) approved together | 4 | establishment (6) | true | n=102 [#168–269] | n=102 | 4 / 2 | cycle3-executed-reviewed `8584403808d1` | 777,533 → 593,553 | Active |
| 5 | idle years 5–9 | 6 | transformation (7) | true | – | – | 6 / 2 | idle-year6 `95bfc545d412` | 593,553 → 593,553 | Active |
| 6 | cycle 4 — second ordinary thinning (year 9 plan, 10 trees) | 10 | transformation (7) | true | n=10 [#270–279] | n=10 | 10 / 1 | cycle4-second-thinning-reviewed `d39922dfd400` | 593,553 → 358,331 | Active |
| 7 | idle years 11–24 | 16 | transformation (7) | true | – | – | 16 / 1 | idle-year16 `2a82ccbae98f` | 358,331 → 358,331 | Active |
| 8 | year 25: objectives met by the ordinary rules; completion-year review acknowledged | 25 | transformation (7) | true | – | – | 25 / 1 | completion-acknowledged `e734a4150714` | 358,331 → 358,331 | **Completed@25** |
| 9 | post-completion cycle 5 (6 trees, approved and executed) | 26 | transformation (7) | true | n=6 [#280–285] | n=6 | 26 / 25 | post-completion-managed-reviewed `ede778462f04` | 358,331 → 133,604 | **Completed@25** (unchanged) |

Intra-cycle checkpoints (group counts computed independently from the recorded fields, then asserted against the Work Plan card):

| Checkpoint | Year | Ordinary open work | Latch / guide | Raw SHA-256 12 | Canonical content hash 12 |
|---|---:|---|---|---|---|
| cycle2-release-executed-prelatch | 2 | none open; release executed, not yet reviewed | false / response (4) | `4005c318121b` | `e6a0b1a5c98d` |
| cycle2-release-reviewed-prelatch | 2 | none open; Annual Review acknowledged | false / response (4) | `ad2c465c8350` | `89116b004944` |
| year2-mfm-pending-new | 2 | **Pending new 86** (MFM prescription 1, `createdYear 2`, ordinary Pending, cash unchanged) | true / ≥5 | `ec9144570e30` | `35d1efd3d751` |
| year3-carried-mfm-idle | 3 | **Pending carried over 86** (still Pending, `createdYear 2` unchanged) | true / recruitment | `da98d7105546` | `53fa472a6e6a` |
| year3-carried-mfm-plus-new-planting-pending | 3 | **Pending carried 86 + Pending new 16**; cash 7,695.33 after stock | true / recruitment | `eb5965237f88` | `ce272f6bdac9` |
| year3-approved-before-advance | 3 | **Approved 102** (carried MFM + new planting), `createdYear` unchanged | true / recruitment | `9feec8eaca1b` | `ad434b09d501` |
| completion-boundary-unacknowledged | 25 | none; outcome Completed@25 before the year-25 review is acknowledged | true / transformation | `33657c9ab4d7` | `b08f7a59352c` |

Raw file bytes differ between Editor sessions because Unity lists trees/buildables/storages in unspecified order (within one session a re-save after load is byte-identical, asserted at every checkpoint). `canon_compare.py` sorts those lists and hashes the content: **all 15 checkpoints have identical canonical content hashes in two independent Editor sessions** (`final/cross-run-canonical-compare.txt`, the dev run `dev/r03-full` vs the final run), so the same chronology reproduces the same saved content.

## 6. D-057 / economy witness (exact candidate, no coefficient change)

Asserted live at boot from the unmodified definition: starting cash €12,000 (1,200,000 ¢), €2,500 harvest minimum, 2,400 owner minutes, completion year 25, century review 100, canopy 0.35 / aspirational 0.65, **Enlarged80 effective targets 240 retained originals / 12 regeneration cells / 0.08 m³ deadwood**, century targets 480 / 40 per species / 48 / 2.0 m³, Legacy40 baseline 60 / 3 / 0.02. Live objective targets matched.

| Year | Cash after (€) | Minimum so far (€) | Goals met (of 8) / notes |
|---:|---:|---:|---|
| 1 | 10,225.54 | 10,225.54 | 4 of 8 (retained, canopy, regeneration, managed opening met; deadwood, beech, oak, year open) |
| 2 | 7,775.33 | 7,775.33 | 5 of 8 (deadwood 0.128 ≥ 0.08 now met) |
| 4 | 5,935.53 | 5,935.53 | 7 of 8 (beech + oak established); only the year-25 goal remains |
| 10 | 3,583.31 | 3,583.31 | |
| 25 | 3,583.31 | 3,583.31 | **8/8 met**: retained 898/240, canopy 0.911/0.35, regeneration 224/12, deadwood 82.25/0.08, managed opening, beech, oak, year 25/25 → Completed |
| 26 | **1,336.04** | **1,336.04** | post-completion work executed; Completed unchanged |

Evidence class: **observed live Editor values on the exact candidate** (`final/02-witness/evidence/d057-cash-and-objectives.txt`). Cash stayed positive and the scenario never entered Failed. **Honest limit (recorded, not hidden):** each harvest job pays the €2,500 small-job minimum, so four small thinning jobs left €1,336.04. The cross-session probe (§7, `final-h4/02-resume`) shows that at that balance one more felling job is **refused by the ordinary rule** (`Upfront cash is required; future timber receipts do not prefinance the job.`, state unchanged), while an idle year still advances. The Work Plan's own cash-risk panel states this (`CASH AFTER THIS PLAN … less than the €2500.00 minimum`; captured in `post-completion-pending-work`). This is accepted-economy behaviour under unchanged D-057 coefficients, not a defect found; whether a plan this thin is the intended player experience is a product question for T-09/Manager. The older D-057 viability witness (minimum €11,451.16) used a different plan and was not re-derived.

## 7. Gate results

All Unity gates ran through `run_t08_gate.py` under the held lock (receipts in `all-gate-receipts.json`, `receipts.md`). **29/29 pass; 0 `CHECK_FAIL`/`error CS` lines.** The main chain ran at `0dfa7ea2` (lock `6f2f34ce`); `resume` ran at `8671c413`; the closing compile re-ran at `c7516697`. Between those heads only verification files not staged by those gates changed (see §12).

| # | Gate | Exit | s | Marker |
|---|---|---:|---:|---|
| 01 | compile (`0dfa7ea2`) | 0 | 65.1 | `S1C_T05_COMPILE_PASS Unity=6000.6.0f1` |
| 02 | **witness** | 0 | 440.8 | `S1D_T08_WITNESS_PASS checks=4413 shots=24 rows=9` |
| 03 | S1-C T-05 objective/save/old-save | 0 | 255.4 | `S1C_T05_UNITY_PASS checks=544 cases=34 shots=12` |
| 04 | rendered objective history | 0 | 113.1 | `T05_RENDER_PASS captures=6` |
| 05 | S1-C T-02 year browsing | 0 | 147.2 | `S1C_T02_HARNESS_PASS checks=246 shots=12` |
| 06 | S1-C T-03 drill-down | 0 | 133.2 | `S1C_T03_HARNESS_PASS checks=279 shots=16` |
| 07 | S1-C T-04 provenance | 0 | 182.0 | `S1C_T04_HARNESS_PASS checks=529 shots=30` |
| 08 | S1-C T-06 guidance | 0 | 119.4 | `S1C_T06_HARNESS_PASS checks=170 shots=8` |
| 08 | Reference Future v1 | 0 | 119.2 | `REFERENCE_FUTURE_V1_PASS` |
| 09 | SaveHardening | 0 | 95.1 | `SAVE_HARDENING_VERIFY_PASS` |
| 10 | ScenarioOneProgress | 0 | 85.2 | `SCENARIO_ONE_PROGRESS_VERIFY_PASS` |
| 11 | baseline-hash (30 cases) | 0 | 67.1 | `T05_BASELINE_HASH_PASS cases=30 … noAnchorsRelabelled=true` |
| 12 | interaction (canonical Sitka) | 0 | 109.3 | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` |
| 13 | density-save | 0 | 103.2 | `DENSITY_QUAL_PASS geometry=L40 hardChecks=6` |
| 14 | completion (model 2) | 0 | 249.5 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` |
| 15 | completion (model 1) | 0 | 170.7 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` |
| 16 | N3 | 0 | 123.3 | `N3_U4_VERIFY_PASS` |
| 17 | T-03 teaching cap | 0 | 231.4 | `TEACHING_CAP_UNITY_PASS checks=223 shots=12` |
| 18 | RackLesson | 0 | 241.2 | `LESSON_PASS checks=36` |
| 19 | MenuTutorial | 0 | 229.3 | `MENU_TUTORIAL_PLAYTHROUGH_PASS` |
| 20 | TeachingCopy | 0 | 125.1 | `S1A_TEACHING_VERIFY_PASS checks=61` |
| 21 | Rack V2 runtime | 0 | 87.1 | `RACKV2RUNTIME_VERIFY_PASS checks=14` |
| 22 | T-06 event-ID save hardening | 0 | 135.1 | `SAVE_EVENT_ID_VERIFY_PASS cases=19 rejected=9 accepted=10 … noMutation=exact` |
| 23 | T-04P teaching latch | 0 | 207.2 | `TEACHING_LATCH_UNITY_PASS checks=60` |
| 24 | T-04 guide monotonicity | 0 | 209.2 | `S1D_T04_GUIDE_UNITY_PASS checks=93` |
| 25 | T-05 previous/proposed (all three groups + fixtures) | 0 | 139.4 | `S1D_T05_UNITY_PASS checks=579 shots=12` |
| 26 | MFM approval boundary | 0 | 119.1 | `MFM07_UNITY_PASS checks=106` |
| R | **cross-session resume** (`8671c413`) | 0 | 91.2 | `S1D_T08_RESUME_PASS checks=16` |
| C | compile (`c7516697`) | 0 | 65.2 | `S1C_T05_COMPILE_PASS` |

Offline (plain .NET SDK, no Unity, run at `c7516697`; logs in `offline/`): T-02 cycle state `CYCLE_STATE_OFFLINE_PASS checks=93` + 7/7 mutants caught; T-03 cap `TEACHING_CAP_OFFLINE_PASS checks=48` and `TEACHING_CAP_PROTECTED_DIFF_PASS base=34e360f8 paths=16` (protected paths have no diff against the exact candidate); T-03/T-04 sequence monotonicity `…OFFLINE_PASS checks=62` + 5/5 mutants; T-04P persistence `TEACHING_PERSISTENCE_OFFLINE_PASS checks=36`, 80-row hash baseline pass, protected sources unchanged; T-05 previous/proposed `checks=219 failures=0`; T-06 event-ID `EVENT_ID_OFFLINE_PASS checks=75 failures=0`; N3 `N3_OFFLINE_PASS checks=88`; MFM approval `MFM07_APPROVAL_BOUNDARY_PASS checks=125` + byte-identical repeat; S1-C history query `55`, drill-down `72`, provenance `56`, guidance `53`; S1-C T-07 `S1C_T07_OFFLINE_PASS checks=304 failures=0 mutations=8 caught=8`.

**Cross-session resume (`final-h4/02-resume`)**: a fresh Editor process starts a brand-new year-0 forest, loads the earlier session's final disk checkpoint through the real controller, and the resumed state fingerprint **equals** the earlier session's (`c35d5601fb48…`: events, orders, history, cycle state, objectives, scenario JSON hash, world hash, latch). It then proves: year 26 / Completed@25 / latch / guide 7 / acknowledgement survive; the unaffordable harvest is refused without changing state; an idle year (27) advances, Completed stays 25 with no second completion event; the new year can be acknowledged and saved.

## 8. The packet's eight acceptance questions

1. **Second/third/later intervention without first-cycle teaching residue blocking Approve/Advance — YES.** Cycles 2, 3, 4 and the post-completion cycle were all approved and advanced through `ui.ApproveWorkPlan`/`ui.AdvanceFromWorkPlan`. The only gates met were the ordinary ones: unmarked rack entrances (cycle 1, refused then marked), the unacknowledged-review rule (`ADVANCE_WAITS_FOR_REVIEW_ACK_WITHOUT_RESIDUE_LOCK`, cleared by one acknowledgement), and pre-latch the hard 40 % cap (refused over-cap draft at year 1; after the latch the same excess is advisory only — `POST_LATCH_OVER_CAP_IS_ADVISORY_ONLY…`).
2. **Guide monotone across drift and reload while pre-completion teaching is preserved — YES.** Pre-latch, the world stays at *response (4)* after execution, after Annual Review alone, after ten reads and after the history browse; the unlatched copy of the very same world stays at 4 and 25 reads never write the latch (`NEGATIVE_CONTROL_*`). After the real review (3 inspections + 6 stand portions, [J]) the latch is true and the guide is 5. Afterwards Crop Tree count up/down, MFM-added Crop Trees, felling plans, executed work, completion and 15 save/load cycles never moved the index below the floor; the row indices read 1, 5, 5, 6, 7, 7, 7, 7, 7, non-decreasing and ≥5 once latched. Gates 23/24 add the accepted forced-mortality, executed-share and pre-v24 cases.
3. **Executed/prior, Approved, new Pending, carried Pending understandable through the existing surfaces — YES.** Work Plan groups (counts, per-order `created in year N`, `prescription 1 · …` provenance, `Creation-year review · Year N` links) and Annual Review `Work done`; captures inspected (§10). All three groups at once exist only in gate 25's labelled in-memory fixture because real approval approves every Pending order at once; the real flow shows Pending new + carried together (year 3) and Approved afterwards.
4. **Creation-year history navigation exact and read-only — YES.** `LINK_ARRIVES_ON_EXACT_CREATION_YEAR` at both sizes; links only for carried orders whose creation year exists in history, labelled "creation year", never approval/cause; missing, future and negative years refused with no latest-year fallback; the historical review has no acknowledge control; world hash, scenario JSON, events, reports, orders, cash, year and acknowledgement unchanged after navigation, Earlier/Later stepping and refused years.
5. **Save/load and chronology deterministic and non-mutating — YES.** 15 checkpoints (§5): save non-mutating, `CanLoad` read-only, load restores the identical fingerprint, re-save byte-identical, ten refreshes write nothing, load never reconstructs the session review facts, `createdYear` not re-stamped on load; canonical content identical across two Editor sessions; fresh-process resume fingerprint identical; gate 22 (T-06) rejects/accepts event-ID cases with no mutation.
6. **Completion reachable on repeated management and management possible afterwards — YES.** Completed@25 through all eight goals plus the reviewed two-cycle sequence (no residual completion acknowledgement needed); the completion-year review was acknowledged once through the UI (idempotent on repeat); a further cycle was planned, approved, executed and acknowledged at year 26 with Completed unchanged and exactly one `ScenarioCompleted` event; management stays open below the year-100 horizon (`CanEditWorkPlan`). Limit: affordability of further harvests (§6).
7. **Solvent under D-057 without coefficient changes — YES, with the recorded thin margin.** Positive cash throughout (min €1,336.04), never Failed; definition read-only asserts (§6); Legacy40 / frozen Reference / deterministic hashes unchanged (§9).
8. **MFM-generated Pending work stays ordinary Pending until Work Plan approval — YES.** Prescription acceptance creates no open work; prescription *approval* creates 86 ordinary Pending orders with `createdYear 2`, no cash movement, nothing Approved; carried unchanged through an idle year and a reload; only `ui.ApproveWorkPlan` approved them (with the new planting) and the normal advance executed them. Gate 26 (MFM approval boundary) and the T-05 gate also pass.

## 9. Protected anchors (none rebaselined; all matched the accepted values)

Frozen Reference Future v1: schedule `56C8B99FA1E8DDD1`, archive FNV `1A42C7BD0439E72F`, years 0/20/50/100 `A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` / `7AD177B3CC2F73C7` (`pinned=True`), continuation v12→v24 `EBD49ADEF18C3F36`. Canonical Sitka `BFC55473C1506067`; Legacy40 lifecycle `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`; mixed continuation `2A8003ABD1E4B810`; dying stand `F37DF35F6649D1C2`; completion model 2 `9C77EB4BEE427FB9`; model 1 `83096F04350D84F8` (historical v18 projected `702766DECE591E21`); baseline-hash 30/30 cases equal the accepted pre-v22 table; Enlarged80 anchors `AC41978AB7FFBD7A` / `D3EDEE31228265F5`; N3 `N3_U4_VERIFY_PASS` + offline 88; T-04P 80-row hash baseline; S1-C T-07 304/304; T-06 event IDs; MFM boundary; teaching cap / RackLesson / MenuTutorial / TeachingCopy / Rack runtime.

## 10. Rendered evidence

The witness captured **24** PNGs (12 states × 1280×720 and 1920×1080) under `final/02-witness/evidence/shots/`; the harness asserts exact capture size, non-blank, every visible text inside its panel horizontally, no vertically clipped text (measured), and every visible button ≥ 24×16 px for each. The protected gates added further captures under their own folders: S1-C objective 12, render 6, T-02 12, T-03 16, T-04 30, T-06 8; S1-D cap 12, previous/proposed 12. States: guide after sample completion, MFM Pending new, year-3 groups collapsed/carried/new expanded, creation-year Annual Review, Approved work, completion-year review, completion objectives/guide, post-completion Pending work, post-completion latest review and post-completion historical (year-25) review.

**Directly viewed by me (final run unless noted):** `guide-after-sample-completion-1920x1080`; `year2-mfm-pending-new-1280x720` (first-use Work Plan help dialog overlays the plan, as on any fresh profile; the clean Pending-new state is in the year-3 captures); `year3-groups-collapsed-counts-1920x1080`; `year3-approved-work-1920x1080`; `creation-year-review-year2-1280x720`; `completion-objectives-guide-1280x720`; `post-completion-pending-work-1280x720`; `post-completion-history-year25-1920x1080`. From the development run (identical code path, not the qualification run): `guide-after-sample-completion-1280x720`, `year3-groups-new-and-carried-1280x720`, `year3-groups-collapsed-counts-1280x720`, `year3-carried-group-expanded-1920x1080`, `creation-year-review-year2-1920x1080`, `completion-year-review-1280x720`. In these: group counts and order text legible, Annual Review provenance badges (RECORDED/DERIVED/CURRENT) and navigation buttons usable, the cash-risk panel readable, no blocking clipping. **All other captures (including all S1-C/S1-D protected-gate captures) were not individually viewed**; they rest on the harnesses' in-engine fit assertions. Observation: the year-25 review viewed from year 26 shows live `CURRENT`-badged goal values (26/25), which is the accepted S1-C provenance labelling, not a defect.

## 11. Requirement coverage (packet list)

(1) idle years 3, 5–9, 11–24; (2) 86 MFM orders genuinely created in year 2, carried to year 3, ordinary Pending until approval; (3) disk save/load at cycle 1, cycle 2 (pre-review, post-review, latched), idle-carried, completion boundary, post-completion, plus 8 more; (4) Annual Review acknowledged and a historical year viewed each cycle (`history-*.txt`); (5) T-05 presentation and exact links (§8); (6) completion by ordinary rules, acknowledgement exercised; (7) post-completion cycle; (8) D-057 table; (9) ordinary approval/execution authoritative; (10) latch monotone, no write-on-read/load.

## 12. Deviations, failed attempts and uncertainties

All failed attempts are preserved (`attempts/`, `dev/`, `dev2/`); none is qualification evidence.

1. **Witness attempt 1 failed** (`attempts/02-witness-attempt1-FAIL-blank-captures-then-editor-exit-hang-reaped`): every `ScreenCapture` came back blank for the whole run (24 `SHOT_NOT_BLANK` failures; all other 4,389 checks passed) and the Editor then hung after its own shutdown (the known intermittent hang; killed by PID after about 9 min). Classified environment. Fix (verification only): the capture retries up to 4× and stops the run if it stays blank; the runner kills an Editor still alive after its shutdown log line. The rerun passed with no blank captures.
2. Runner issues: the S1-C `render` gate expects its prior evidence at `<parent>/02-objective`; my numbering made it `03-objective`, so `04-render` failed (`attempts/04-render-attempt1-…`); fixed with a symlink `final/02-objective → 03-objective`, no code change. A resume-probe compile error (`CS0165`) and a wrong expected refusal wording were fixed in my own new gate before its qualification run (`dev2/`).
3. **Not one chain at a single head.** Gates 01–26 ran at `0dfa7ea2`; `resume` at `8671c413`; compile re-ran at `c7516697`. The later commits add only the resume probe/runner entry and the offline event-ID pin; the witness and every protected harness SHA-256 recorded in each `command.json` are unchanged.
4. **Offline items not clean as run, with causes (no assertion weakened):** T-06 offline check asserted `CurrentVersion == 23`; reconciled to the accepted 24 (`c7516697`), then 75/75. The mutation phases of the T-03 cap and T-06 offline runners stop at mutants whose source-text anchors no longer match production (`expected exactly one match … found 0`); the mutants that still apply were caught, the checks themselves pass, and the stale mutant anchors were reported, not repaired (`*-WITH-MUTATIONS-stale-mutant-anchor.log`). The T-03 cap runner's default protected-diff base is the historical T-03 base `23fe4b8` and fails by design on later accepted slices; it passes against the exact candidate base (both logs kept). S1-C **T-05 offline** check (`S1CObjectiveHistory/run_offline_check.py`, pinned `CurrentVersion is 22`) was **not** reconciled or counted (it aborted on that pin); its authoritative current form is Unity gate 03 (asserts v24), the same position S1-C T-08 took.
5. Input is synthetic: queued `InputSystem` keyboard events and direct UI-callback invocation (`Clickable.Invoke`); OS-native pointer/keyboard is not qualified. The first guided review's player positioning uses the accepted T-04P aiming helper.
6. Not run / not claimed: Tier-3 sweeps, native-input or personal playtest, any Windows/Player build, completion replay beyond gates 14–15, the Rack V2 combined/HUD suite. The three-group Approved + Pending new + carried state at once is exercised only by the accepted T-05 gate's labelled in-memory fixture.
7. Economy: the §6 limit. I did not compare against the earlier viability witness and cannot say the witness plan is the "typical" player path; it is one honest controlled plan.
8. Checkpoint raw SHA-256 values are not comparable across Editor sessions (array order); use the canonical hashes. Dev run `dev/r03-full` and the final run agree on all 15.
9. `ORCHESTRATION-LEDGER-20261031.md` and `active-tasks.md` were not touched; nothing was pushed.

## 13. Cleanup and lock release

Per gate: staged harness and `.meta` removed, copied licence removed, `.vscode`/material import side effects restored, isolated saves removed, host saves unchanged (see receipts). Two orphaned Roslyn compiler servers started by my runs were terminated; no Editor/Xephyr/openbox remains. Final state, verified after the handoff commit: `git status --porcelain --untracked-files=all` empty in `/media/jer/ZX20/CCF-U1`; production diff against `34e360f8` empty; no Editor/Xephyr/openbox; no staged harness or `.meta` under `Assets/`; host save unchanged; lock released normally (`locks/07-release-final.json`) and shown `free`.

## 14. Appendix V2 — orchestration evidence (worker-side facts only)

The ledger `ORCHESTRATION-LEDGER-20261031.md` is the orchestrator's and was left untouched; no capacity values were supplied to the worker and none are invented here.

- Execution worker: Claude Sonnet 5.5 (`claude-sonnet-5-5`), one session, no model switch, no Opus escalation, no subagent. Work unit #208, High difficulty (cross-system save/history/economy/runtime qualification), qualification only.
- Observed capacity/interruption events: none (no quota, rate-limit, context or provider failures). Wall-clock about 2.5 h from the first lock (19:46 UTC) to the last gate and offline rerun (about 22:15 UTC).
- Failure classes encountered (none were capability failures): environment — blank Editor screen captures and the intermittent post-shutdown Editor hang; harness/driver — evidence-directory naming expectation, own-gate compile error, stale offline version pins and stale mutant anchors; lock — none (one owner; five re-locks by the same owner/task with exact heads).
- Lock transitions: six receipts above; no conflict, no force release.
- Evidence anchors: gate receipts `all-gate-receipts.json`; failed attempts `attempts/`; development runs `dev/`, `dev2/`; offline `offline/`.

## 15. Result

**S1-D T-08 MULTI-CYCLE WITNESS PASS — READY FOR INDEPENDENT SPEC REVIEW**

Next action: #200 releases T-09 independent conformance review on this evidence; T-09 should decide whether the thin post-completion cash margin (§6) is acceptable product behaviour.
