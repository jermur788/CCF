# scen1 — Manager #424: #527 NEEDS CHANGES DRAFT quality and single bounded repair dispatch

**Date:** 15 November 2026. **Actual Manager:** OpenAI GPT-6, GitHub/browser docs-only. **Claim:** [#529](https://github.com/jermur788/CCF/issues/529). **Worktree NONE; Unity/Editor/physical machine lease NONE**.  
**Verdict:** **NEEDS CHANGES — DRAFT QUALITY ONLY**, direct adoption of independent [#527 verified nonauthor review](https://github.com/jermur788/CCF/blob/cb2013782238f89a6f9deb9d6e73db4a8a4605d7/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-THREE-PROFILE-SAVE-DRAFT-REVIEW-RESULT-20261115.md) with **2 MAJOR +1 MINOR** documentation defects. No approved save architecture, D-082 first-demo decision, source integration, Unity qualification, real-world truck approval or Windows build.

## Source-anchored findings and minimum disposition

I read the exact immutable #527 report and its [closed HANDED-OFF claim](https://github.com/jermur788/CCF/issues/527), plus the actual #524 amended save-policy DRAFT **spec.md**, **plan.md**, **tasks.md** and **ISO-01..ISO-63 requirements.md** at the immutable author branch, not an ungrounded chat summary. I verified the #527 reviewer branch had **only 1 ADDED Markdown report** versus registered main starting HEAD; it was a genuinely independent OpenAI gpt-6.1-sol High/Codex reviewer, not original #524 Claude Opus 5.5 author, with no Unity or local source edits. The amended save DRAFT author comparison remains **4 modified four-file Markdown +1 new handoff**, ISO **63 unique contiguous unchecked**, baseline 36 retained.

Direct source citations in #527 show:
1. **M-1 MAJOR: missing historical save `version` vs parsed numeric ≤0** are wrongly equated in #524 **FR-07 spec.md:109–111**. Current source initialises `ForestSaveData.version` to current schema version; parsed ≤0 normalises to 1, whereas absent raw key is NOT proven equivalent. Amend table and focused existing ISO-49 test wording, without selecting a new historical acceptance policy.
2. **M-2 MAJOR: overwrite confirmation is simultaneously mandatory (spec W-02:45 / ISO-03:12) and unresolved for F5 (spec FR-05:103 / Q-03:181)**. Current F5 direct Save has no prompt; F10 asks. Make F5 clause conditional on **open Q-03**, preserving F10, slot containment and no-mutation expectations; do NOT pick owner UX.
3. **m-1 MINOR: pre-v20 geometry source description (spec.md:29)** overstates rejection. Current `ValidateGeometry` rejects **nonzero** pre-v20 values, accepts explicit **0**, and pre-v20 resolves to Legacy40. Correct sentence only, no code/test updates.

**A-1/A-2 nonblocking advisories** remain optional small cross-reference when/if B-13 and envelope policy get owner approval, not a new re-architecture or reason for additional review cycles.

## Policy and execution boundaries

This Manager disposition **ACCEPTS the independent documentation NEEDS CHANGES verdict**. It does **not** grant #424 authority to rewrite #425's shared save policy, approve IDs/envelope, tighten authentic historical newer-version best-effort, choose B-01..B-15/Q-01..Q-03, or release source/Unity. Original O1 historical v1–v24/five-rack/Legacy40/model0/Reference Future v1, separate compact O1/O2 profiles, strict new-profile rules, backups/prefs, old-source version semantics remain protected as **requirements**, not runtime verified. Current main canonical D-082/#471 is still PAUSED.

**One next task:** [#530 READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/530), **complete binding author-only repair packet**: https://github.com/jermur788/CCF/blob/docs/scen1-527-save-draft-bounded-repair-dispatch-20261115-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-BROWSER-THREE-PROFILE-SAVE-527-BOUNDED-DOCS-REPAIR-PACKET-20261115.md.

- **Purpose:** update exactly those three defects; no owner policy rulings or repeated full review.
- **Difficulty LOW–MEDIUM**, reason: source-backed doc wording and conditional owner UX.
- **Best-suited:** original #524 document author **Claude Opus 5.5 browser/GitHub**, or another explicitly #425-delegated worker; actual model/provider to be registered.
- **Authority prerequisite:** **#425 must explicitly delegate this NEW #530 documentation-only repair**; previous #524 authorisation was task-scoped, not blanket. If missing, #530 BLOCKED and no tracked write.
- **Exact worker lane:** GitHub/browser only, new branch `docs/scen1-530-three-finding-save-draft-repair-20261115-v1` from verified #524 amended save-DRAFT original head, **NOT main**, worktree NONE, Unity lease NONE.
- **Only current required modified tracked files:** `specs/scenario-independent-save-isolation-v1/spec.md` and `checklists/requirements.md`; plan/tasks only if a genuine direct contradiction is proven. **ONE new handoff** `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-SHARED-SAVE-527-THREE-FINDING-BOUNDED-REPAIR-RESULT-20261115.md`.
- **Test/approval boundary:** ISO-01..ISO-63 remain **all UNCHECKED**, B-01..B-15 and Q-01..Q-03 OPEN, DRAFT NOT APPROVED. **No Unity, source, actual saves, historical fixture replay, PlayerPrefs, compile, Windows build or canonical/coordination edits.**
- **Next after author handoff:** at most **one independent delta review** of exact repairs, and only then named #425/owner/save decisions and separate canonical/spec approval before implementation.

## Manager publication and status

Manager #529 branch `docs/scen1-527-save-draft-bounded-repair-dispatch-20261115-v1` created from verified main with no local worktree. The only allowed tracked changes are this NEW `scen1-` Manager verdict and the NEW #530 binding packet. GitHub readback/compare must confirm exactly 2 ADDED Markdown files and 0 other changes before #529 is HANDED-OFF/CLOSED. #527 already HANDED-OFF/CLOSED; #530 READY/NOT RUNNING until #425 delegation + actual worker registration. Notify #424/#425/#527. Dashboard/Drive sync **NOT VERIFIED**.

**User action:** forward the #530 link to #425 for explicit repair delegation, then to an independent docs author. No decision on F5's actual UX is needed merely to repair the contradictory DRAFT wording.
