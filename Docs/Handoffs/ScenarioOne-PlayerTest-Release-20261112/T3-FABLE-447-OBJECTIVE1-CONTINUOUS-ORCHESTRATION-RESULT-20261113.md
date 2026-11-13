# #447 orchestration result: Objective 1 repair #443, ready for Manager #424's integration verdict

**13 November 2026, 09:30 UTC. State: candidate independently APPROVED; Manager integration verdict REQUESTED, not yet given. Not merged, no Windows build, not user-accepted.**

Orchestrator: Anthropic Claude Fable 5.2 (`claude-fable-5-2`), T3 Code. Control branch `orchestration/scenario-one-objective1-447-20261113-v1`. Full event record: [ledger](T3-FABLE-447-ORCHESTRATION-LEDGER-20261113.md).

## The candidate

- **Final candidate:** `e115b54053fe96fd2c0b85d0516aa15cb65b1387` on `feature/scenario-one-objective1-ux-repair-20261112-v1` (branch head `2432ba7e` adds the result doc only). Base `55863f1b`. [Diff from base](https://github.com/jermur788/CCF/compare/55863f1b7719011bba16b3f3ab377e379c0b988a...e115b54053fe96fd2c0b85d0516aa15cb65b1387).
- **Source result (author GPT-6.1 Sol):** [SOL-OBJECTIVE1-UX-REPAIR-RESULT-20261112.md](https://github.com/jermur788/CCF/blob/2432ba7e47806d87639b76d52e9f38b4dd900fd7/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-OBJECTIVE1-UX-REPAIR-RESULT-20261112.md).
- **Independent review 1 (Claude Opus 5.5), NEEDS CHANGES on `1aa0fa9a`:** [report](https://github.com/jermur788/CCF/blob/d5024fc3/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE1-443-INDEPENDENT-SOURCE-AND-G5-REVIEW-RESULT-20261113.md).
- **Independent delta review (Claude Opus 5.5, separate session), APPROVED on `e115b540`:** [report](https://github.com/jermur788/CCF/blob/e9637bab/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE1-443-INDEPENDENT-DELTA-REVIEW-RESULT-20261113.md). No blocking findings.
- **F-1 authority:** [direct user decision](https://github.com/jermur788/CCF/blob/e97137a03e7c9319cc92b9c981e855da7d5b7254/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/USER-OBJECTIVE1-F1-RACK-NUMBER-DISPLAY-DECISION-20261113.md).
- **Raw evidence (local, durable):** `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/` (author; final run `g5-run4-full-e115b540/`) and `/media/jer/Files/CCF/Verification/CCF-449-Objective1-IndependentReview-20261113/` (reviewers). Screenshots are not in Git (run 1 alone is about 102 MB).

Production change: 14 files. Thirteen are on the G3 allowlist; the fourteenth, `ScenarioOne/ScenarioOneManager.RackPlan.cs`, changes two feedback strings under the user's F-1 decision. Both reviewers confirmed `TrySprayRackEntrance`, aim and equip code byte-identical to base.

## The 20 G3 items

"PASS" means the author's native evidence at 1280x720 and 1920x1080 plus the independent reviews; it does not mean user acceptance.

| Item | Status | Note |
|---|---|---|
| O1-01 first Objectives [O] cue | PASS | Session-only; static under Reduce Motion. |
| O1-02 no `Next:` badge on the O1 Stand Map CTA | PASS | |
| O1-03 Objectives type hierarchy | PASS | |
| O1-04 diagram anchor | PASS | First run failed here at 720p (toggle off-screen); fixed in `1aa0fa9a`. Long detail text still scrolls. |
| O1-05 Stand Map help copy and M/Esc keycaps | PASS | Cosmetic: keycap prose leaves wide word gaps. |
| O1-06 Plan Racks first-visit cue | PASS | |
| O1-07 distinct Plan Racks card | PASS | Failed review 1 (F-2); fixed. Edge measured 2 px at 720p, 4 px at 1080p. Reviewer: visible but restrained. User to judge. |
| O1-08 rotation cursor | DEFERRED | Manager G3 verdict. |
| O1-09 lower-left rack geometry | DEFERRED | Separate design decision. |
| O1-10 darker help backdrop | PASS, qualified | Style alpha 0.94; bright text behind is still faintly legible (about 1.9:1). User to judge. |
| O1-11 wording set 1 | DEFERRED | Not chosen. |
| O1-12 "Now that you've confirmed" lead | DEFERRED | Not confirmed. |
| O1-13 one-mark sentence deletion | PASS | Causes one expected new failure in an old help-copy checker. |
| O1-14 minimap rack target and first arrow | PASS in a fresh world | The player's own world was not reproduced. See limits. |
| O1-15 south-to-north visit numbers | PASS | Failed review 1 (F-1); fixed by the two-string change. Limit D-1 below. |
| O1-16 minimap area | PASS | 248x248 at 720p (2.51x), 372x372 at 1080p (2.50x) against the stated baselines; no overlap. |
| O1-17 Tab keycap at 5/5 | PASS | |
| O1-18 Work Plan hierarchy | PASS | Class names only in C#. |
| O1-19 Annual Review instruction | PASS | F-3 fixed: shown only while acknowledgement is pending. Earlier-year browsing checked in source, not driven natively. |
| O1-20 Annual Review prompt first sentence | PASS | |

Mandatory G5 proofs, final run 4 on `e115b540` (522 checks, PASS), confirmed by the delta reviewer from the pixels: clearing frame at zero sprays with arrow, "Rack 1" label, distance and minimap marker; bottom first gives 1/5 and next Rack 2; top second gives 2/5, next Rack 2, far tree Rack 5; reversed-ID plan keeps each tree on its own rack and its own Pending orders (149 on both plans); old-world and frozen Reference controls.

## What is not verified, and known limits

- **The missing arrow in the player's own world was never reproduced.** Its historical cause is unknown. Two real mechanisms were fixed (arrow suppressed within 2.5 m of an unmarked rack; stored-ID ordering), but neither is proven to be what the player saw.
- **D-1 (new, needs a Manager or user decision; not fixed):** the Stand Map rack editor, its messages, the Treatment Review strip titles and the harvested-rack message still name racks by stored creation number. On a hand-drawn or edited plan the map can say "Rack 1" where the field says "Rack 5". It does not appear during marking. Fixing it needs files outside the approved set.
- Carried from review 1: N-2 (Stand Map destination diamond moves to the rack entrance during marking), N-3 (larger minimap may crowd the planting hotbar in later objectives; untested), N-4 (pre-existing: an invalid first entrance gives no arrow and no reason text), cosmetic C-1..C-5.
- Not done: a load of the player's real save, a disk save/load round trip after new marks, Tier 3, a Player or Windows build, performance measurement, a test on integrated main.
- Older offline checkers fail the same way on the untouched base (NextActionGuidance, RackV2ConfirmMinimap, RackV2HelpTeaching, ScenarioOneFiveRackDiagram); not repaired here.
- Two native runs are preserved as FAIL (run 1 at `8bcc06f7`, run 3 at `a5cf0ef5`); neither is counted as evidence of a pass beyond what the reviewers accepted from run 1 before run 4 replaced it.

## Routing, loops and custody

- **Writer:** the user directed "start with Sol 6.1". Serial transfer from the stopped Claude Opus 5.5 session; the first OpenAI account failed at once on its usage limit with no edits; the second account's GPT-6.1 Sol did all G4/G5 and repair work in four sessions. One writer at all times; no fallback to Claude was needed. **Reviewers:** two separate Claude Opus 5.5 sessions, neither an author. Disclosure: the G3 spec was written by an earlier Claude Opus 5.5 session.
- **Repair/review loops:** 1 of the 2 allowed.
- **Manager coordination:** the writer change was requested on #424 at 00:31Z; I have not seen `active-tasks.md` updated for it.
- **Unity:** four leases by the writer, each released by exact owner. At 09:25Z: `unity-lock` free, no Editor PIDs, lane clean at `2432ba7e` = origin. Protected saves host37, backup81, production4011 re-verified by the orchestrator after every writer session (all rc 0). These are re-checks, not a launcher EXIT-trap receipt.
- **Measured resources:** native runs 326 s, 148 s, (run 3 not recorded here), 300.5 s by the runner. Tokens, quota and cost: UNKNOWN.

## Next steps

1. **Manager #424:** integration verdict APPROVED / NEEDS CHANGES / REJECTED for `e115b540`, and a ruling on D-1 (disclose and follow up, or repair first).
2. **User:** personal re-test of the approved candidate, ideally in your own world. Checklist is in the delta review's "For the user's re-test". Your acceptance is a separate gate and is not claimed here.
3. No merge to main and no Windows build without your approval.
