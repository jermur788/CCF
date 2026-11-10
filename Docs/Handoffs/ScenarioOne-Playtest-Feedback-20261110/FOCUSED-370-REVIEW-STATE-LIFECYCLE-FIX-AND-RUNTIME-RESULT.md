# #370 — Annual Review unread reset + one targeted Unity Year 2 regression

**10 November 2026.** Owner: Anthropic Claude Sonnet 5.5 (Claude Code harness in T3 Code), continuing after a Codex usage-limit stop before any edit. Binding packet: [Manager #369→#370](MANAGER-369-TO-370-ANNUAL-REVIEW-RESET-AND-TARGETED-UNITY-REGRESSION.md). **No main merge, no Windows/Player build, no game acceptance, no independent code review (user-deferred).**

| Item | Value |
|---|---|
| Exact patched source + verifier candidate | `4db486db7a132bd01725442ef33e00f702c4dff9` on `fix/scenario-one-annual-review-reset-370-20261110-v1` (parent `ebff8e70`, game source `85568c4f`) |
| This report | report-only commit on `report/ccf370-review-reset-20261110` on top of the candidate; **not** a game source |
| Production diff | one statement in `ScenarioOneManager.AdvanceYear()` |

## The fix

```csharp
annualReports.Add(report);
// Each generated report (a no-work year included) is unread until explicitly acknowledged; reuses the saved flag.
annualReviewSeen = false;
```

It sits at the report-append point, after every failure exit and the ecology advance, so a failed or blocked advance never reaches it. `MarkAnnualReviewSeen()`, save capture/restore, `NeedsAnnualReview`, `ScenarioOneUiRoot` and all consumers are unchanged. No schema, version, event, stage, score or economy change, and no migration of old saves.

## SOURCE AUTHOR CHECKS (offline, not Unity)

Real Unity-managed DLL typed compile `sources=189 errors=0`, then only:

| Check | Result |
|---|---|
| Old focused #363/#365 checks (`run_checks.py`) | PASS 160 (unchanged) |
| Old consumer-path checks | PASS 219 (unchanged; 160 + 219 = the 379 old checks) |
| HUD source contract | PASS |
| **New `review_reset.py`** (exact production slices of `MarkAnnualReviewSeen`, `RecordEvent`, the `AdvanceYear` report tail, capture/restore, `NeedsAnnualReview`, driven through the exact #367 consumers) | PASS 18 |
| **New named mutation `advance-year-omits-unread-reset`** (`mutations_370.py`) | KILLED by the specific assertion `R370_YEAR2_NO_WORK_REPORT_UNREAD` (mutated slice compiled) |

New assertions cover: Year 1 report unread → ACK marks read with one event → **Year 2 no-work report appended, unread, no fabricated ACK** → Review-first on HUD, Objectives short/full/card and Map/Work Plan/Annual Review reason+target, still Objective 2 → Year 2 ACK marks read with a Year 2 event, repeat ACK is idempotent, **Crop Tree short/full guidance restored, no review banner** → Year 3 new report unread again → failed/no advance keeps state, report count and events → save capture/restore of an actually unread Year 2 stays unread, read after ACK stays read, a legacy "seen" save loads seen, and the save record gains no field. Source-pin checks require exactly one reset, at the report append, after all `return false` exits and the ecology advance, and no other unread writes.

**Limits.** The fixture models the successful end of `AdvanceYear`, not Unity, ecology or file persistence. The failed-advance negative is source-pinned, not executed against a live failing advance. Legacy40/Reference worlds and year browsing were not exercised by the new tests (the old 379 checks were rerun unchanged). Old historic FAILs (Workbench exit 1, StandMapParity ×2, save03b, UxPedagogy38, #360 171/2) were not rerun and are not weakened.

## ACTUAL UNITY: PASS (narrow)

One warm Unity 6000.6.0f1 Editor in registered lane `/media/jer/ZX20/CCF-U3`, on a new branch at exactly `4db486db`, **normal exit 0 in 482.6 s**, harness result `PASS`, 0 fail lines. Fresh Enlarged80 disposable world, no seeded save, isolated `XDG_CONFIG_HOME`.

Actual route (all from the genuine fresh world): Plan Racks → reset suggestions → Confirm Rack Plan (5 racks, 149 jobs) → mark all five rack entrances with **native OS `P` + left click** (xdotool, `native-input.log`) → approve the plan → Advance (Year 1 report) → ACK → still Objective 2 → **Advance Year 2 with no work** → captures → F5/F9 → ACK → captures. No flag, teaching stage or world state was injected. Later-stage objectives, felling and Crop Tree marking were not touched.

| State | `seen` | `unread` | reports (latest year) | ACK event years | Observed |
|---|---|---|---|---|---|
| Year 1 report | false | **true** | 1 (1) | – | Annual Review unread |
| After Year 1 ACK (Objective 2) | true | false | 1 (1) | 1 | Crop Tree guidance "Inspect trees [E] and mark three Crop Trees [C]…" |
| **Year 2, no work, before ACK** | **false** | **true** | 2 (2) | 1 (no fabricated Year 2 ACK) | Review-first everywhere (below) |
| After F5/F9 (Year 2 unread) | false | true | 2 (2) | 1 | save file `annualReviewSeen:false`; guidance still Review-first |
| **After Year 2 ACK** | true | false | 2 (2) | 1,2 | Crop Tree guidance restored everywhere; saved world `annualReviewSeen:true` |

**Year 2 unread, 1280×720 and 1920×1080 (all non-black, `positive` ≥ 99.9 %), read from the live UI tree:**
- HUD: title "Review the year", "Next: Read and acknowledge the Annual Review: open the Work Plan [Tab]." (progress "Annual results unread · read and acknowledge them").
- Objectives: "First: Review the year — read and acknowledge the annual results." banner, instruction "Read and acknowledge the Annual Review: open the Work Plan [Tab]."
- Map and Work Plan: next-action reason is the same Review sentence; Work Plan shows "Year 2 complete: 0 task(s)…".
- Annual Review: "Year 2 complete" with the acknowledge button.
- **After the Year 2 ACK** the HUD, Objectives, Map and Work Plan all say "Choose one Crop Tree in each zone, 8 m apart; include both rack sides." and the review banner is gone.

Contact sheet: `Evidence370/contact-sheet.jpg` (Year 1 unread; Year 2 unread Objectives/Map/Work Plan/Review; Year 2 after ACK Objectives/Map/Work Plan). Full PNGs, per-capture state text and UI-tree dumps for every capture are in Files evidence `/media/jer/Files/CCF/Verification/CCF-370-ReviewResetUnity-20261110/` (PNG hashes in `png-sha256.txt`).

**Interaction honesty.** Native OS input: `P`, left click (entrances), F5, F9. UI `Clickable` dispatch (same method as #369): Map/Work Plan buttons, Approve, Advance, ACK. Harness: view switching, window size, positioning/aiming at the entrance tree. `Aim` placed the player and reticle; walking was not claimed.

**Not proven.** The F9 load is not independently confirmed to have run a reload (state was identical before and after; the file written by F5 holds `false`). Year 3 reset is covered only by the offline test, not live. A live failed-advance case was not run. Reduce-motion/grayscale, Legacy40/Reference, Objectives 6+ and late loop remain NOT VERIFIED here. The Editor log still contains 50 UI Toolkit renderer stack-trace lines (`UIRenderDevice:EvaluateChain`), the same class of noise already noted in #369; no `C370_FAIL`, no compile error. This narrow PASS is not a full Objective 1–5 requalification and does not replace the retained #369 gameplay evidence.

## Preflight and protection receipts

- Live before start: unity-lock free, no Editor process; Unity Hub GUI only; U3 status identical to #368's pre-state (eight known generated files, byte-hashes match the triage manifest); ZX20 35.93 GiB free (≥ 35); licence file present (valid in practice: Editor activated and ran); no new human Unity session; #361 release from #368 stood.
- Lane switch: eight dirty generated files backed up, restored worktree-only to HEAD (no stash/reset/clean), new branch `qualify/scenario-one-annual-review-reset-370-20261110` created at the candidate. Real lease `332d2008…` held by `Claude-Sonnet-5.5-CCF370` for lane/branch/exact head; released by own id (`release.json`, status free afterwards).
- Disk monitor every 20 s for the run: stayed 35.93→35.93 GiB, state OK throughout (`disk.log`).
- Disposable profile proven before any input: `Application.persistentDataPath`, `F5/F9` save path and `prefs` all under the evidence `run/config/…` (`paths.txt`); the real user save locations were never touched.
- After exit (`protection-after.json`): 36 valued save files, 5,732 Workbench files, 6 protected manifest rows and **27 licensed ignored assets** all hash-identical; original eight generated U3 files reapplied byte-exact and U3 status identical to the pre-state; Workbench `git status` identical; harness source and `.meta` removed, licence copy removed; no Editor running.
- Primary's unrelated `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications untouched.
- U3 was left on branch `qualify/scenario-one-annual-review-reset-370-20261110` at `4db486db` (it was on the #368 branch at `85568c4f`).

## Scoped statuses

| ID | Status |
|---|---|
| FR-N1 Each successfully generated report starts unread, including a no-work year | **Source PASS (offline) and Unity PASS (Year 2 live)** |
| FR-N2 Explicit ACK marks that report read; one year-specific event; Crop guidance restored | Source PASS; Unity PASS (Year 2) |
| FR-N3 Failed/blocked advance preserves previous state | Source-pinned PASS only; no live negative |
| FR-N4 Save format, stage progression, economics unchanged; no migration | Source PASS (no field/version/event change); Year 2 save/ reload of unread and read states observed in the isolated save file |
| FR-N5 Year 3 resets again | Offline PASS; not live |
| FR-N6 Legacy40/Reference/historic browsing | Old 379 checks unchanged PASS; **no live** check |
| Original #369 FAIL (Year 2 stays read, Crop shown) | **Not reproduced after the patch** |
| Broad requalification, Objective 6+, motion, Windows/Player | NOT RUN |

## Reproduce

```sh
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out <out>/compile
python3 -B Tools/Verification/PlayerFeedbackOvernight/review_reset.py <out>/compile/bin/Release/netstandard2.1/check.dll <out>/reset "$PWD"
python3 -B Tools/Verification/PlayerFeedbackOvernight/mutations_370.py <out>/compile/bin/Release/netstandard2.1/check.dll <out>/mut "$PWD"
```

**Hold for Manager #200:** integration of `4db486db` into the feature line, a verdict on this narrow PASS, any live Year 3/failed-advance/Legacy40 check, and independent review remain Manager/user decisions.
