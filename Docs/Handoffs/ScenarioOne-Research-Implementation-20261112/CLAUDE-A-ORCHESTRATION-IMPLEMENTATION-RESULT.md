# Claude A orchestration result — Research-Informed Management Learning V1 (copy-only)

**Date:** 2026-11-12 03:00 UTC. **Claim:** [#390](https://github.com/jermur788/CCF/issues/390). **Orchestrator:** Claude A, `claude-fable-5-2`. **Children:** [#391](https://github.com/jermur788/CCF/issues/391) implementation and Unity qualification (GPT-6.1 Sol), [#392](https://github.com/jermur788/CCF/issues/392) independent review (Claude B, Sonnet 5.5).

## Outcome

**Implemented, independently reviewed APPROVED, Unity-qualified on author-run evidence with stated gaps. NOT integrated:** by user decision in session the patch stays on its feature branch for a Manager #200 merge decision. Not user-accepted. No Windows build.

| Item | Value |
|---|---|
| Revised UI candidate | `feature/scenario-one-research-copy-v1-20261112` @ `a314d308315b5894263d30461a1e8b52ca0f3605` |
| Based on | `integration/scenario-one-personal-playtest-candidate-20261111-v1` @ `b62446caf40fc2ac939d55a6bea6e201f41de700` (game source `2aaacad6`, the post-#373 / #377–#379 UI lineage; no later successor existed) |
| Source worktree | `/media/jer/Files/CCF/Operations/CCF-S1-ResearchCopy-V1-Sol` (clean, on the feature branch) |
| New Unity lane | `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, branch `qualify/scenario-one-research-copy-v1-20261112` @ `a314d308`, clean, Library kept (~6.5 GB), registered as `ResearchCopyQual391` |
| User #382 playtest lane | `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` — still clean @ `2aaacad6`; never opened in Unity, never checked out; 27 licensed ignored assets were read and copied from it (hashes matched) |

## What changed (whole diff `b62446ca..a314d308`)

Six files under `Assets/ForestPrototype/UI` (`MenuHelpView.Text`, `LearningObjectivesView.Curriculum`, `ScenarioOneTeachingSequence`, `ScenarioOneUiFacts`, `WorkPlanOverview`, `AnnualReviewView`), two verifier pins (`Tools/Verification/ScenarioOneTeachingCopyVerification.cs`, `Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs`) and the author report. Nothing under `Assets/ForestPrototype/ScenarioOne`; no save, serializer, objective, displayName, hash, Share, storm, MFM, Rack or ecology code.

- **FR-C1:** Help now says Share is a local (8 m) size-and-distance indicator that does not itself assess species, crown shape, height, shade tolerance, light direction, timber quality, habitat or wind stability, and that the 30% target is this game's calibrated exercise, not a universal rule or a verdict on one neighbour. The "being small is not a reason to fell" lines are narrowed to "smaller neighbours usually contribute less to the measured Share". The optional fuller Help paragraph was omitted.
- **FR-C2:** "Areas with young trees" kept. "regeneration cohort(s)" wording replaced in Work Plan, Annual Review and curriculum; Oak/Beech objective displays as "Plant, with species present — <species>"; Annual Review "Objective first reached" now uses the friendly label. History strings "Areas with young regeneration" deliberately left (parsed by prefix in `ManagementHistoryText.cs`).
- **FR-C3:** one general Help sentence on canopy shade, gap edges and open spaces.
- **FR-C4:** "The default Scenario One does not simulate wind damage." once in Help.
- **Exception beyond strings-only (needs Manager acknowledgement):** two non-literal presentation changes — `AnnualReviewView.cs:714` displays `ScenarioOneUiFacts.ObjectiveName(match)` instead of the raw `displayName`; `WorkPlanOverview.cs` passes `valueBelow: true` on the clearance row, added after the rendered check failed (F391-01). Both are display-only; Claude B judged both acceptable.

## Acceptance (evidence class stated)

| AC | Status | Evidence |
|---|---|---|
| AC-1 Share correctness | PASS | Claude B source review; rendered Help captures (author-run, audited) |
| AC-2 No unique correct tree | PASS | Claude B source review; no logic changed |
| AC-3 Regeneration truth | PASS | Claude B source review; protected diff empty |
| AC-4 Qualitative regeneration Help | PASS | Claude B source review; rendered capture |
| AC-5 Wind limitation | PASS in game Help | External test-note repetition **not done** (no such note was in scope of the child packet) |
| AC-6 Protected identity | PASS at source level; Reference gate PASS on `868643a1` with anchors equal to the #381 record | **NOT RUN:** loading a baseline save and comparing `CurrentWorldHash` (no retained fixture with a recorded hash) |
| AC-7 Rendered readability 720p/1080p | PASS on author-run Unity captures audited by Claude B | Help/objectives/curriculum/lesson/Annual Review captured on `868643a1` and carried to `a314d308` by inference; clearance rows captured on `a314d308`. Oak/Beech "first reached" variants **NOT RUN** |
| AC-8 Proportionate handoff | PASS | this file, ledger, child reports |

## Verification facts

- Offline presentation checks `run_checks.py`: 157 PASS, 5 mutants killed (author and Claude B, on `a314d308`).
- Offline compile: 191 sources, 0 errors (author; reproduced by Claude B on `868643a1`).
- Real Editor (Unity 6000.6.0f1): import and compile 0 errors on `868643a1` and `a314d308`; unmodified P3 `WorkPlanResidualVerification` PASS on both.
- **Rendered finding F391-01** (found on `868643a1`, fixed in `a314d308`): Work Plan clearance label wrapped one character per line beside the lengthened value. Before/after images are in the Unity report.
- **Failures kept visible, not waived:** `RackV2HelpTeaching` offline checker FAILS on baseline and candidate (pre-existing S6 + ten step-6 checks; candidate adds S10, that tool's own file whitelist). Old `ScenarioOneTeachingCopyVerification` Unity harness FAILED `nursery warning visible` in the cold-import session whose captures were black; classification **UNDETERMINED** (Claude B: implausibly caused by this patch, unproven; no baseline rerun).
- Unity attempts: 1 BLOCKED (lane unregistered), 2 PARTIAL (black captures), 3 FAIL on one panel, 4 PASS for the fix. One Editor hung after shutdown in attempt 2 and was killed by recorded PID.
- All Unity runtime evidence is **author-run** (Sol); Claude B audited screenshots and logs but ran no Editor.

## Custody

Nine host save/PlayerPrefs files hashed before and after every Unity attempt: unchanged. Unity lock FREE, no Editor running. ZX20 free 17,659,592,704 bytes (floor 16,106,127,360) — **about 1.45 GiB of margin; the new lane's Library is the cost**. One entry appended to `/media/jer/ZX20/CCF-Ops/lanes.json` by Claude A (backup and diff in `/media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/`). Durable evidence root: that same directory.

## Links

- Author implementation report: https://github.com/jermur788/CCF/blob/a314d308315b5894263d30461a1e8b52ca0f3605/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-IMPLEMENTATION-RESULT.md
- Author Unity qualification report with screenshots: https://github.com/jermur788/CCF/blob/f4bd3a02f8f826dd9f0c9787d36b63e5bb428a72/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md
- Claude B reviews (NEEDS CHANGES → APPROVED → final APPROVED): https://github.com/jermur788/CCF/blob/100a43a252c4d370c4b407b1d6aa5663c710063f/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-INDEPENDENT-REVIEW.md , https://github.com/jermur788/CCF/blob/100a43a252c4d370c4b407b1d6aa5663c710063f/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-DELTA-REVIEW.md , https://github.com/jermur788/CCF/blob/100a43a252c4d370c4b407b1d6aa5663c710063f/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-FINAL-DELTA-REVIEW.md
- Routing ledger: [CLAUDE-A-ROUTING-DECISION-LEDGER.md](CLAUDE-A-ROUTING-DECISION-LEDGER.md)

## For Manager #200

1. **Merge decision** for `a314d308` onto the Scenario One candidate lineage (fast-forward of `integration/scenario-one-personal-playtest-candidate-20261111-v1` is possible; its code is a strict descendant plus report commits). The user's #382 pin `2aaacad6` is unaffected until you decide.
2. Acknowledge or reject the two display-only non-literal changes above.
3. Confirm or revert the `ResearchCopyQual391` lane registration, and decide whether the new ZX20 lane and its Library are kept (disk margin is thin).
4. Decide whether the remaining gaps need closing before merge: baseline-save world-hash load, Oak/Beech first-reached variants, nursery-warning assertion, external test-note wind sentence.
5. Sync `Docs/Project/coordination/active-tasks.md` and the dashboard/Drive mirrors: **not done by Claude A** (Manager is sole writer); dashboard sync status NOT VERIFIED.
