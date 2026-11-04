# Overall Manager review — Rack V2 committed editing-gate repair (#277)

**Date:** 2026-11-04  
**Scope verdict:** **APPROVED — BOUNDED TEST REPAIR / CLAIM MAY CLOSE.**  
**Stack integration verdict:** **HOLD — NOT INTEGRATED, NOT RELEASED TO THE PUBLISHED PLAYER-TEST CANDIDATE.**  
**Reviewed:** [PR #278](https://github.com/jermur788/CCF/pull/278), [claim #277](https://github.com/jermur788/CCF/issues/277), [worker handoff on repair branch](https://github.com/jermur788/CCF/blob/fix/rack-v2-editing-verification-free-rotation-20261104/Docs/Handoffs/Overnight-Continuation-20261104/RACK-V2-EDITING-GATE-REPAIR-HANDOFF.md), [#275 Manager review](MANAGER-FOCUSED-UI-QUALIFICATION-REVIEW-20261104.md).

## Examined directly in GitHub

- PR #278 is open and stacked on the **PR #276 qualification branch**, not on `main` or the published player-test branch. Its final head is `9220730cd232d8ca100a423d2c438ad75f7e5c79`; the reviewed Unity-tested source head is `17288a419b883718738cfc4e452d883a82bda580`. The later commit adds only the 182-line handoff. Direct file reads show the verification-file blob is **identical** at the tested source head and final PR head.
- Against PR #276 the complete PR #278 file list is exactly **two paths**: `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` (+58/−17) and the new repair handoff. No production C#, serialized assets, save/schema, project settings or frozen reference files changed in this PR.
- Inspected Part-B native-pointer diff: real Shift is asserted during the handle drag; both Shift/no-Shift near-level releases must be **0.4–3° unsnapped**, preserve fixed entrance, and commit the captured last valid ghost without misleading snap wording. The Shift-only rotation is undone exactly back to its level predecessor. The later large rotation is undone exactly to the *prior free near-level* geometry with plan-signature equality and one-level Undo checks. The new capture helper checks a valid live ghost. Unchanged existing Create/Move/refusal/save-byte/history/locking assertions were retained in the diff.
- GitHub compares confirm strict ancestry: PR #278 is exactly **2 commits ahead / 0 behind** PR #276; PR #276 exactly **2 commits ahead / 0 behind** PR #273; final #278 is **8 commits ahead / 0 behind** the currently published personal-playtest branch. This establishes an exact reviewable *proposed* stack; it does not confer permission to move that branch.

## Verification evidence and limits

The worker's durable GitHub handoff records an exact-head native Unity test, not a Manager rerun. At the unchanged starting test head, the **unmodified** committed gate failed the obsolete snap assertion at 1280×720 (exit 1, 178 checks reached, 4 screenshots). At the repaired source head, the **committed** `RackV2EditingVerification` passed real OS-pointer and Shift input at 1280×720 and 1920×1080: **356 checks, 17 captures, exit 0**, 0 C# compiler errors, 0 reported gate FAIL, 0 pixel calibration error. It observed ~1.166° near-level unsnapped rotation with/without Shift and ~12.06–12.09° large rotation. The recorded offline gate passed **61** checks, `dotnet build` reported 0 warnings/errors and diff whitespace validation passed.

Independent Claude Sonnet 5.5 Medium source/evidence review of the exact tested source reported **APPROVED WITH NOTES**, no blocking issue, and stronger enforcement than before. Notes: exact card-text match could be stricter than prefix plus negative tokens, culture-sensitive ASCII substring checks are nonmaterial, and the near-level angle depends on rack-length calibration. The new comment already explains that Reshape ignores Shift.

**Evidence boundary:** this Manager inspected GitHub source, diff, history, handoff and reported independent review; did **not** directly inspect local raw screenshots/logs, the running Unity Editor, the deployed CCF-Ops lock or uncommitted local checkout. Reported PASS is worker runtime evidence supported by durable records, not a newly performed Manager Unity qualification.

## Manager determination

**APPROVED for #277 as a verification-only repair.** The one specific missing committed-gate conformance condition identified in the #275 review is now met on the isolated repair stack. Claim #277 can be marked Manager-reviewed/closed; this does **not** mean product acceptance or production integration.

**The surrounding staged UI change remains HOLD FOR INTEGRATION**, not because #277 failed, but because the previous direction to avoid staged merges/published-candidate changes remains binding and the full stack has other unaccepted behaviour. Specifically preserve:

1. source-static **FAIL 08**: Objective 1 primary CTA still returns “Back to forest” rather than the unapproved intended Show Map behaviour;
2. **D-064 / OPEN-2**: approval-before-field-marking remains an unresolved behavioural issue;
3. the `16–20 m` Stand Map card presentation residual;
4. three draft UX specs #257–#259 remain unapproved, and experimental MFM PR #274 remains research only;
5. frozen Reference Future v1, Legacy40, user saves, warm CCF-U1 Library, personal-playtest candidate and Windows-build gate.

## Integration order — binding planning decision; no merge authorised

The safe dependency order is:

1. **PR #273** — candidate-relative source staging (#255/#256/#260), anchored to the published personal-playtest baseline.
2. **PR #276** — docs compaction and the already accepted #275 Tier-2 qualification handoff on exactly that stage.
3. **PR #278** — this accepted committed editing-test repair and its handoff on PR #276.

For future *isolated* composition, preserve this exact base-to-tip ancestry; the current PR #278 head already contains all three layers. Do not squash/rebase/retarget onto divergent `main` or an unrelated Unity checkout without fresh evidence and coordination. **No PR is merged by this verdict**, no publication branch is moved, and the user’s personal-playtest candidate stays unchanged. A later separate Manager integration release must verify fresh live ownership/current refs, decide scope of unresolved player-visible defects and explicitly authorise the precise target. Any production or product-workflow expansion requires its own accepted authority; #277 cannot silently supply it. No full Tier-3 rerun is authorised merely by this test-only repair; later candidate-specific verification is risk-based.

## Operations and preservation

The local worker reported a normal release of its exclusive Unity lease and a final FREE/no-Editor receipt, plus a clean worktree after restoring four known Editor-generated side-effect files. This is a **historical release receipt**, not proof of current free ownership. The worker also reported changing only the stale `git_stores[].git_dir` in deployed `CCF-Ops/lanes.json` from an obsolete path to `/media/jer/Files/CCF-Legacy/.git`, with a backup/diff in the evidence root. Treat as a disclosed, narrowly justified local operational repair **requiring local #203/#200 configuration reconciliation**, not an independently verified GitHub source change. Do not roll it back blindly or fold it into user-owned uncommitted work.

The authoritative local coordination/context anchor is `/media/jer/Files/CCF-Primary`, **not a Unity lane**. Its user-owned uncommitted `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` path fixes remain untouched. This browser Manager cannot directly inspect that filesystem.

## Next state

- **#277:** APPROVED / CLOSED (verification repair only).
- **#275:** remains approved Tier-2 feature evidence; **#273/#276/#278:** OPEN and unmerged pending a separate integration release.
- **Next actionable boundary:** preserve the isolated stack and let the user's personal playtest and/or explicitly accepted CTA/approval UX decisions govern later publication. Do not start another Unity lane, modify the player-test candidate or authorise Windows build.
- **Coordination/dashboard:** reconcile live `Docs/Project/coordination/active-tasks.md` and `jermur788/Overall-Dev/updates/ccf/latest.md`; check read-back. Automated dashboard ingestion audit remains separately unverified.
- **User action required:** none for closure of #277. User playtest and post-playtest Windows-build authorisation are distinct future gates.
