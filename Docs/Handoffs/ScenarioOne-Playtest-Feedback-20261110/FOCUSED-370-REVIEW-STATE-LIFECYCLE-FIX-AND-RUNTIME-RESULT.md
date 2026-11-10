# #370 — Annual Review unread reset: source fix and result

**10 November 2026.** Owner: Anthropic Claude Sonnet 5.5 (Claude Code harness in T3 Code). The earlier OpenAI Codex run for this task stopped on a usage limit before any edit. Branch `fix/scenario-one-annual-review-reset-370-20261110-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, start HEAD `ebff8e70` (source `85568c4f`).

## Verdict

- **Source fix: IMPLEMENTED, offline source checks PASS.**
- **ACTUAL UNITY REGRESSION: NOT RUN.** No lock acquired, no Editor started, no save or preference touched. The user chose a source-only handoff, so the Year 2 runtime check remains open. Source checks are not runtime proof.
- Not independently reviewed. No main merge, Player build or game acceptance.

## Exact candidate

Source + verification commit: **`4db486db7a132bd01725442ef33e00f702c4dff9`**. The whole production change is one statement in `ScenarioOneManager.AdvanceYear()`, directly after `annualReports.Add(report);`:

```csharp
annualReviewSeen = false;
```

It runs only on the success path, after the ecology advance and after every early `return false`. A blocked or failed advance never reaches it, so the previous flag and report list stand. No schema, event, stage, score, economy or save-version change, and no retrospective migration. `ScenarioOneManager.cs` SHA-256 `d3be7fee689be4b9734febe83bc36b08bebcdce9a5bd292ee4afff659a7ac937`. The only `Assets` change since `85568c4f` is those two lines.

Note: that commit was not made by this session. It appeared in the worktree while checks were running, signed as Claude Sonnet 5.5. Its content matches what this session wrote (including a `SOURCE-ALLOWLIST.md` addendum). It suggests a second session may have been working on #370 in the same lane; this is not resolved.

## SOURCE AUTHOR CHECKS (offline, no Unity)

Receipts: `/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-370-Repair-20261110/`.

| Gate | Result |
|---|---|
| Unity-managed-reference C# compile | 189 sources / 116 refs / exit 0 / 0 errors |
| New lifecycle check (`review_reset.py`, `ReviewReset.cs`) | **18 PASS** |
| Named mutant `advance-year-omits-unread-reset` | killed by `R370_YEAR2_NO_WORK_REPORT_UNREAD` (its own assertion, not a generic crash) |
| Existing #363/#365 assertions | 160 PASS, unchanged |
| Existing #367 consumer assertions | 219 PASS, unchanged |
| Original seven mutants and four #367 negatives | all still killed or refused |

The lifecycle check compiles exact production slices of `MarkAnnualReviewSeen`, `RecordEvent`, the `AdvanceYear` report tail, the Capture and Restore lines and `NeedsAnnualReview` into a plain-data fixture. It drives the exact #367 consumers (HUD, Objectives short/full/card, Map, Work Plan, Annual Review reason and target). The chain covered:

1. Year 1 report is unread; its acknowledgement marks it read and records one event.
2. Year 2 advance with no work appends a report that is unread, with no fabricated acknowledgement. Review-first shows on every surface and the lesson stays Objective 2.
3. Year 2 acknowledgement marks it read, records a year-specific second event, and restores Crop Tree guidance. A repeat acknowledgement adds no event.
4. Year 3 report is unread again.
5. Save and reload preserves an unread Year 2 as unread and an acknowledged one as read; a legitimately read save loads as read. The fixture has no new field.
6. Failed advance: pinned by source contract (one reset, after the report append and after all failure exits, no other unread writes), plus a state-unchanged fixture case. This is a source-structure proof, not an executed failing `AdvanceYear`.

Limits: the fixture models the end of a successful advance and `CurrentYear++`; it does not run ecology, Unity objects, layout or real save files. Legacy40 and Reference-world behaviour is covered only by the unchanged existing old-world assertions plus the one-line diff scope.

## ACTUAL UNITY: NOT RUN

Cause: user decision to hand off source-only, plus a possible concurrent writer in the lane (above). The binding Unity gates were not evaluated: U3 admission, ≥35 GiB on ZX20, licence, 27 protected assets, frozen Reference, original eight-dirty Workbench, disposable save and preference isolation. At the last read `unity-lock status` was free with no Editor process. ZX20 showed about 36 GiB free, near the 35 GiB threshold, so a Unity run needs a fresh disk check.

Still needed: one genuine Year 2 no-work run at 720p and 1080p showing Review-first on all surfaces before acknowledgement and Crop Tree after it. #369's passing gameplay evidence is retained unchanged. Its Year 2 FAIL stays recorded as the original defect.

## Unchanged historical FAILs

Strict Workbench exit 1, StandMapParity ×2, save03b, UxPedagogy 38, #360 synthetic 171 PASS / 2 FAIL, and 26 all-black captures are not rerun or relabelled.

Next: Manager #200 assigns the Unity regression and any independent review. Reproduction commands are in `SOURCE-ALLOWLIST.md`.
