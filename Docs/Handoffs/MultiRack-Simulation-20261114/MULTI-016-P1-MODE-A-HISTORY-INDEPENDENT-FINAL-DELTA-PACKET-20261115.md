# MULTI-016 — Independent final delta review: Sitka P1 DRAFT Mode A event-history and approval-gate wording

**Date:** 15 November 2026  
**Execution claim:** [CCF #547](https://github.com/jermur788/CCF/issues/547) — READY / NOT RUNNING; no reviewer, host worktree, source ownership or Unity lock registered.  
**Difficulty:** **MEDIUM**: only two narrow documentation defects remain, but event sourcing, approval session epochs and hash comparison must be exact.  
**Best-suited independent model:** **Claude Sonnet 5.5 High** in a fresh local T3 Code/Claude Code reviewer lane, or an equivalently capable reviewer independent of current GPT-6 draft/repair author. Earlier findings/repair were Claude Opus 5.5, and an independent Codex Sol High review confirmed the residual defects. Disclose actual identity and prior involvement; this review is not a fresh forestry scientific audit.  
**Verdict authority:** **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** on the two documentation fixes only. Even APPROVED AS DRAFT QUALITY does NOT approve the P1 specification, C-1…C-7, P1-01…05 implementation, a Unity session, game integration or an Irish field forecast.

## 1. Exact source, reviewed defects and corrections

**Revised candidate (pinned, do not merge or update other refs):**
- \`docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1\`, full HEAD \`3af7b7ff30804bc9aff0d19ea394b9c45946c5a8\`.
- Full [MULTI-015 Manager docs repair handoff](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-015-P1-DRAFT-MODE-A-HASH-AND-GATE-DOC-REPAIR-HANDOFF-20261115.md), [#546](https://github.com/jermur788/CCF/issues/546), HANDED-OFF/CLOSED.
- Previous source \`8756c452cf76a8dd3169f2ab40c93349cb1edc60\` from MULTI-013. Manager's connected-GitHub comparison/readback of MULTI-015 showed **exactly FOUR MODIFIED DRAFT Markdown + ONE ADDED handoff Markdown**, zero other changes.
- [Independent MULTI-014 Codex Sol High review](https://github.com/jermur788/CCF/blob/3714d4e01d9ab55e0e6a76a1d51ce0bc0fc3688f/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md), [#542](https://github.com/jermur788/CCF/issues/542) HANDED-OFF/CLOSED, graded **8/10 repairs PASS** and identified:
  - **D-1 / R-2 MAJOR**: Mode A still promised identical event hashes while allowing a load event or later session-epoch-changed approval. P0 uses a session epoch; source-derived counterexample at P0 \`LabSession.cs:69–70,75–85,221–225\` and \`CanonicalJson.cs:133–148,171–175\`. Raw snapshot hashing and event history are different concepts. The prior phrase "remove load event and compare hash" lacked a defined canonical projection.
  - **D-2 / R-4 MINOR**: \`checklists/requirements.md\` REQ-F09 still used \`P1-G0\` instead of \`P1-G2\`.
- [Formal Manager NEEDS CHANGES disposition](https://github.com/jermur788/CCF/blob/docs/manager-multi-014-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-014-P1-DRAFT-RESIDUAL-NEEDS-CHANGES-DECISION-20261115.md). The Manager directly performed MULTI-015 small docs edits and did **not self-approve**.

**Read and check precisely these four DRAFT files on the pinned candidate (all READ-ONLY for the reviewer):**
1. [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md): §4 source fork/state/hashes, Mode A/B and AC-P1-05.
2. [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md): §5 replay invariant and Mode A/B.
3. [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md): §4 event/replay, Mode A/B and negative cases, final readiness status.
4. [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md): REQ-E06 plus Mode A/B and REQ-F09.

These remain **DRAFT / NOT APPROVED**. The owner previously approved preparing the draft **only**. All seven design conditions C-1…C-7, including C-1A, C-2a/C-2b and C-6 Unity scheduling, must remain **OPEN**. No source/Unity/closed-loop P0 change is in scope.

## 2. Mandatory D-1 / R-2 review cases — evaluate the actual acceptance rule, not wording alone

**Required direct/source-derived equivalence contract:**

- **Biology and legally executed work:** when legal actions and underlying source/model/seed are comparable, a split/restart run must reproduce per-ID biological state, ecological year, model+RNG version, seed, mortality roll inputs, death/fell IDs/causes/years, executed-work outcomes and data provenance. If legal work diverges because an approval legitimately stales, report that policy-dependent difference; never call different legal sequences "the same" without defining equivalence.
- **Raw canonical-history hashes:** require equality **ONLY when the complete canonical event history is identical**, including order, payload, sequence, previous/post state hash links and all hashed session-epoch fields. A history can differ even if biological output matches.
- **Whole-snapshot hash versus history:** distinguish a complete hashed snapshot, a biological projection and an audit/event-history hash. The P0 snapshot hash does not become a generic P1 history hash by assertion; later approvals can include new session epochs in hashed state.
- **No invented normalization:** if a future projected/normalized-history equality is desired, it must be a separate **named, versioned C-1/C-1A decision**, with precisely defined retained/excluded event and snapshot fields, session-epoch handling, sequence/renumbering, stable serialization and rehashing, followed by independent review. Merely "removing a load event" is not sufficient.

**Independent negative/positive thought tests (no executable P1 exists):**
1. **Mode A, no open APPROVED/PENDING_WORK plan at split, NO load audit event and no later session-dependent payload drift.** If both complete canonical histories really are byte-identical, raw hash equality may be asserted. Biological equivalence remains necessary.
2. **Mode A, no open approval but a recorded load audit event.** Biological state can match while raw histories differ; each actual chain independently verifies. No claim to comparable "rehashed event-deleted" histories without separately defined C-1/C-1A projection.
3. **Mode A, no open approval at split but a NEW approval occurs later on each run.** Reload creates a new random epoch; its approval payload/snapshot hashes may differ even without an audit event. Raw whole-history equality is NOT a valid unconditional acceptance criterion; comparable biological and legal-work outcomes remain the claim.
4. **Mode B, open APPROVED/PENDING_WORK plan at split, future C-1A Option 1 retains P0 policy.** Exactly ONE named \`PlanStale(SessionLoaded)\` per affected plan at load; a different next-step stale in uninterrupted mode means different audit histories. Compare named lifecycle and legally equivalent work outcomes, reject missing/duplicate stales and false approval persistence. If C-1A Option 2 is chosen later, its distinct reviewed policy must supply its own oracle; don't pre-decide the product choice.
5. **P1 version/persistence and P0 isolation:** P0 schema/events/saves unchanged, P1 replay policy future-only; all C-1…C-7 OPEN and requirements checkboxes unmarked.

The reviewer must check cross-file agreement, precise treatment of legal work and cause-specific mortality, and whether any wording still implies unconditional raw history-hash equality for Mode A. A verified contradiction is **NEEDS CHANGES** with the smallest doc-only correction. The reviewer should not quietly resolve C-1A by preferring one plan-approval policy.

## 3. Mandatory D-2 / R-4 review

Check exact \`checklists/requirements.md\` **REQ-F09** says **\`P1-G2\`** future spec/product approval, not obsolete \`P1-G0\`. Search **all four DRAFT files** for obsolete \`P1-G0\`. The legitimate completed evidence-plan **plan G0** stays distinct from \`P1-S0 / G1\`, \`P1-G2 / G2\` and \`P1-01…05 / G3…G7\`. No product approval can be inferred from a fixed gate name.

## 4. Independence, ownership and allowed output

**Independent reviewer branch already created, untouched at pinned candidate:**
\`review/multi-016-sitka-p1-mode-a-history-final-delta-20261115-v1\`.

**Proposed physical worktree, NOT CREATED / NOT RESERVED:**
\`/media/jer/Files/CCF/Operations/CCF-MULTI-016-SitkaP1-ModeA-DeltaReview-20261115\`, only if the reviewer actually runs on the free Linux laptop with verified ownership and no process conflict. If on Windows, choose a separate WSL ext4 worktree. A GitHub-only reviewer may state no local worktree. **No Unity machine lease is needed or authorised.**

**Exact sole writable tracked file (a new Markdown handoff):**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-016-P1-MODE-A-HISTORY-AND-GATE-FINAL-INDEPENDENT-REVIEW-20261115.md\`

All existing \`specs/**\`, \`Assets/**\`, \`Tools/**\`, P0 lab, Unity/ProjectSettings, live \`Docs/Project/coordination/active-tasks.md\`, main, source worktrees, older handoffs, frozen Reference and production saves are **READ-ONLY**. No code, C# compilation, run, test changes, GROWFOR/NFI broad review, Windows build, package install or dashboard/Drive sync.

**Mandatory preflight before source inspection/any checks/report writing:**
1. Fresh-read this binding packet, [#547](https://github.com/jermur788/CCF/issues/547), [#546](https://github.com/jermur788/CCF/issues/546), parent review and Manager ruling. Fresh-read the LIVE coordination board and verify source branch/worktree/host/process ownership and current full HEAD. Do not treat a missing board row as an ownership right; do not touch other dirty worktrees or run \`git clean\`/\`reset\`/\`stash\` on them.
2. Register **actual provider/model, machine, worktree (or GitHub-only none), review branch, starting full HEAD, source refs and allowed read-only phase as [RUNNING] on #547 BEFORE inspection/tests/writing**. If collision or missing critical files, mark [BLOCKED] and return factual reason; no hidden source substitutions.
3. Reviewer must be **not** the GPT-6 Manager who authored/repaired. Disclose if the reviewer participated in earlier scientific/review work; independent means not author of the exact repaired files.

**Evidence and exit:**
- Focused result **D-1 PASS / PARTIAL / FAIL** and **D-2 PASS / PARTIAL / FAIL**, with exact file/section/line and smallest documentation corrections for failures. Optionally re-render changed Markdown through GitHub GFM/renderer and report actual outcome, but don't claim browser or runtime test if none. Confirm all old R-1/R-3/R-5…R-10 PASS topics remain outside new changes, all seven conditions OPEN.
- Distinct overall verdict **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED**. Passing this review qualifies the **DRAFT for later Manager and product-owner decisions only**, not SPEC APPROVED or Unity scheduling.
- Publish only the exact new handoff above, GitHub fetch/readback and compare reviewer branch to \`3af7b7ff30804bc9aff0d19ea394b9c45946c5a8\`: **exactly ONE ADDED Markdown**, no DRAFT edits. Mark #547 [HANDED-OFF] and close; notify [#546](https://github.com/jermur788/CCF/issues/546), parent [#542](https://github.com/jermur788/CCF/issues/542), and [Overall Manager #425](https://github.com/jermur788/CCF/issues/425).
- Next Manager verdict then owner-level C-1A/C-6/C-7 decisions if DRAFT quality passes. **No automatic P1 code, Unity or Windows game build.** Dashboard/Drive NOT VERIFIED.

## 5. Paste-ready dispatch

> Take ownership of **MULTI-016 / CCF #547** — independent final two-defect documentation delta review of the Sitka P1 adult-rules DRAFT. Read the full binding GitHub packet: https://github.com/jermur788/CCF/blob/docs/multi-016-mode-a-history-final-delta-review-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-016-P1-MODE-A-HISTORY-INDEPENDENT-FINAL-DELTA-PACKET-20261115.md . **Difficulty MEDIUM**; recommended **Claude Sonnet 5.5 High**, or a comparably independent non-GPT-6 source-literate reviewer, in new T3 Code/Claude Code. Original/last repair was written by GPT-6 Manager. Reviewer branch \`review/multi-016-sitka-p1-mode-a-history-final-delta-20261115-v1\` is untouched from exact corrected DRAFT \`3af7b7ff30804bc9aff0d19ea394b9c45946c5a8\`. Proposed **uncreated/unreserved** Linux-laptop worktree \`/media/jer/Files/CCF/Operations/CCF-MULTI-016-SitkaP1-ModeA-DeltaReview-20261115\` ONLY if actually free after live coordination and host/branch check; else use separate WSL ext4 or GitHub-only read-only lane. Register your ACTUAL model/provider/host/worktree/branch/full starting HEAD as RUNNING on #547 **before inspecting/testing/editing**. Review D-1 (Mode A biology vs exact raw history hashes, named load event, later fresh approval epoch, future projected policy OPEN C-1/C-1A, Mode B stale-on-load) and D-2 (REQ-F09 uses P1-G2). All 7 C-conditions stay OPEN, P1 spec remains DRAFT; no Unity/session/build/code/source/spec modifications. Publish only \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-016-P1-MODE-A-HISTORY-AND-GATE-FINAL-INDEPENDENT-REVIEW-20261115.md\` on reviewer branch, verify GitHub exactly one added Markdown, close #547 HANDED-OFF and return **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** with evidence. No Windows Unity Player build or field validation.
