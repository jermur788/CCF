# #450 rack-marks amendment: orchestration result, ready for Manager #424's integration verdict

**13 November 2026, 11:22 UTC. State: IMPLEMENTED, REVIEWED (independent APPROVED), VERIFIED by one native run at both resolutions. NOT integrated, NOT user-accepted. Manager verdict requested, not yet given.**

Orchestrator: Anthropic Claude Fable 5.2, existing #447 claim, control branch `orchestration/scenario-one-objective1-447-20261113-v1`. Event record: [ledger](T3-FABLE-447-ORCHESTRATION-LEDGER-20261113.md). Earlier stage: [#447 result for `e115b540`](T3-FABLE-447-OBJECTIVE1-CONTINUOUS-ORCHESTRATION-RESULT-20261113.md).

## Candidate

- **Candidate to integrate:** `7633eaf1e720821af70d6b243b53fb9424cedd28` on `feature/scenario-one-objective1-ux-repair-20261112-v1` (branch head `acb4e111` adds the amendment result doc only).
- **Previously approved baseline:** `e115b540`. [Amendment diff](https://github.com/jermur788/CCF/compare/e115b54053fe96fd2c0b85d0516aa15cb65b1387...7633eaf1e720821af70d6b243b53fb9424cedd28): four production files, +12/−10, display text only. [Whole change from base `55863f1b`](https://github.com/jermur788/CCF/compare/55863f1b7719011bba16b3f3ab377e379c0b988a...7633eaf1e720821af70d6b243b53fb9424cedd28).
- **Source result (author GPT-6.1 Sol):** [SOL-OBJECTIVE1-RACK-MARKS-PROGRESS-UX-AMENDMENT-RESULT-20261113.md](https://github.com/jermur788/CCF/blob/acb4e1111fdc9aa9b295413038eda4e03fe56402/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-OBJECTIVE1-RACK-MARKS-PROGRESS-UX-AMENDMENT-RESULT-20261113.md).
- **Independent review (Claude Opus 5.5, claim #451), APPROVED, no blocking findings:** [report](https://github.com/jermur788/CCF/blob/a420982f/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE1-450-RACK-MARKS-INDEPENDENT-DELTA-REVIEW-RESULT-20261113.md).
- **Authority:** [#450 packet](https://github.com/jermur788/CCF/blob/docs/objective1-rack-marks-progress-450-dispatch-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/T3-FABLE-450-OBJECTIVE1-RACK-MARKS-PROGRESS-UX-IMPLEMENTATION-PACKET-20261113.md).
- **Raw evidence (local):** `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/450-g5-full-7633eaf1/` and `450-tier1-*`; reviewer scratch `/media/jer/Files/CCF/Verification/CCF-451-Objective1-RackMarks-Review-20261113/`.

## What the player now sees in the guided field stage

| Surface | Text |
|---|---|
| Arrow | Next rack mark · 53 m |
| Minimap target | Rack mark |
| Counter | Rack marks: X of 5; at five, "Rack marks: 5 of 5 — complete" |
| Aimed at an unmarked entrance tree | Rack mark · [Left-click] Spray orange… |
| Aimed at a marked tree | Rack mark already made · Orange… |
| After a spray | Marked the entrance tree PO0203. |

Stored rack IDs, tree and work-order associations, target selection (south to north, next unmarked) and spray logic are unchanged; the reviewer confirmed those methods byte-identical to `e115b540`.

## Acceptance points from the packet

| Point | Status | Basis |
|---|---|---|
| 1. Clearing before any spray: arrow rendered, bottom rack suggested, ring, distance, "Rack marks: 0 of 5", no rack number | PASS in a fresh world | Native captures at both sizes; reviewer and orchestrator viewed them. |
| 2. Any order counts; next target is the next unmarked | PASS | Default plan 0, 1, 2 of 5; reversed-ID plan top first 1 of 5, then 2 of 5. |
| 3. Repeat spray adds nothing; clear recomputes; stale target fails closed | PASS with limits | Repeat-spray rule proven from source; the native step does not prove the click was handled (R-2). Clear-mark has no in-game caller, so it is untestable in play. Stale target: bounded in-memory test only, not a corrupted save file. |
| 4. Five of five, then Work Plan, Advance, Annual Review; associations intact on both plans | PASS | 928 native checks, zero association mismatches. |
| 5. Visual quality and regressions | PASS | No overlaps; minimap 2.50–2.51x area; F-2 accent and F-3 instruction still correct. |
| 6. Negative controls, saves, no build | PASS | Old-world and frozen Reference controls; protected manifests rc 0; no disk save; no Player build. |

The 20 G3 items are as in the earlier #447 result: 16 PASS, 4 DEFERRED (O1-08, O1-09, O1-11, O1-12). O1-14 and O1-15 are now met by the unnumbered presentation, not by Rack 1..5 labels.

## Open, for Manager #424 or the user

- **R-1 (scope ruling):** the Work Plan approval advisory in `LearningObjectivesView.Sequence.cs:196` still says "rack entrances … X / 5 marked". Before 5 of 5 it shows on the same Work Plan screen as the new "Rack marks: X of 5" line. No rack number leaks. The file is outside the four allowed; not edited. Found from source; no capture exists.
- **D-1 (unchanged):** the Stand Map rack editor and Treatment Review still show stored creation-order rack numbers. With no numbers in the field there is nothing for them to contradict, but they remain.
- **R-2 (harness only):** strengthen the repeat-spray step to confirm the click was processed.
- **The player's own-world missing arrow is still unreproduced.** Possible cause noted by the reviewer (N-4, pre-existing): an invalid first entrance removes the arrow and ring with no explanation.
- Cosmetic: "rack mark trunk" wording reads slightly as if already marked; some messages from excluded code still say "entrance".
- Carried forward: N-1 help backdrop legibility, N-2 Stand Map destination diamond, N-3 later-objective hotbar and the larger minimap (untested).
- Not done: load of the player's real save, disk save/load round trip after marks, Tier 3, Player or Windows build, integrated-main test.

## Custody

- One writer throughout: GPT-6.1 Sol (second OpenAI account), same lane. Reviewer: a separate Claude Opus 5.5 session; disclosure, the G3 spec and earlier reviews were also Claude Opus 5.5 sessions. Repair loops for #450: 0.
- Orchestrator checks at 11:22Z: `unity-lock` free, no Editor PIDs; lane clean at `acb4e111` = origin; host37, backup81, production4011 re-verified (rc 0) after the writer session. Re-checks, not a launcher EXIT-trap receipt.
- `active-tasks.md`: I have not seen the Sol writer or #450 RUNNING recorded by the Manager.
- Tokens, quota, cost: UNKNOWN.

## Next

1. **Manager #424:** integration verdict for `7633eaf1`; rulings on R-1 and D-1.
2. **User:** personal re-test in your own saved world; checklist in the #451 review. Acceptance is yours alone and is not claimed here.
3. No main merge and no Windows build without approval.
