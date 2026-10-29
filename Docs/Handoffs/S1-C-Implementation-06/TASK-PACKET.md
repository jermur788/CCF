# S1-C T-06 — Prior-Treatment Guidance / Understand Why

**Parent:** #91 / #106  
**Claim:** #147  
**Spec:** `specs/scenario-one-review-history-v1/` T-06  
**Status:** **PREPARED FOR DISPATCH**  
**Purpose:** add bounded historical guidance that helps the player connect earlier treatment to the next inspection/decision without inventing facts or prescribing one forestry answer.

## Difficulty and routing

- **Difficulty:** **3/5** — bounded history-query/UI/copy integration, but provenance, causal-language and waypoint correctness require focused rendered verification.
- **Preferred worker:** T3 Code using Claude Sonnet 5.5 Medium-or-Ultra or OpenAI Sol/Codex high reasoning.
- **Ultra Code:** suitable, but not required; this task should not need architectural invention.

## Exact start gate / lineage

S1-C T-04 #123 is Manager-approved at final production candidate:
`633775fd8650db3cf3482e550d106a0febd7b9cb`.

Its pushed handoff successor:
`3ea4af8b`
contains only `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` after the final production SHA and is the exact starting lineage for this task.

The earlier T-04 production candidate `ba2922e...` and pushed successor `cff79f0...` are superseded and must not be used as the start of T-06.

Suggested branch/worktree:
- branch `feature/s1-c-t06-prior-treatment-guidance-v1`
- worktree `/media/jer/Videos/CCF-S1C-T06`

Before production edits:
1. refresh live `Docs/Project/coordination/active-tasks.md` and #91/#106/#123/#147;
2. verify #123 history/review ownership is released and no newer writer owns the intended files;
3. verify exact starting HEAD `3ea4af8b` and a clean worktree;
4. mark #147 `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD.

A claim records execution presence only; live coordination is the ownership authority.

## Required scope

Implement S1-C FR-7 / AC-5 using the existing Annual Review/history system. From a reviewed year or intervention, expose a **short L4 explanatory teaching view** that names prior treatment relevant to what the player should inspect before another judgement.

Use only:
- persisted S1-C **L1 recorded history**;
- clearly separated **L2 current observed state** where useful;
- existing read-only `ScenarioManagementHistory` / `YearHistoryView` / intervention data rather than a competing history implementation.

Examples may identify a prior thinning/release, retained deadwood choice, planting/protection history, recorded storm loss, or other recorded work relevant to present inspection.

Where a faithful recorded location exists, reuse the existing waypoint/navigation mechanism so the player can go and look. If the record has no faithful target/cell/position, do not reconstruct one from the current world: omit the waypoint and use honest not-recorded wording where necessary.

## Provenance and wording invariants

The final Manager-approved T-04 provenance contract at `633775fd...` is binding:
- recorded facts remain visibly RECORDED;
- current observations remain CURRENT and are not presented as earlier-year facts;
- display-time calculations/groupings remain DERIVED, including the Management History intervention-count heading;
- reconstructed/session-only/not-recorded facts retain their own treatment;
- the L4 guidance itself is visibly guidance/explanation, not a new recorded fact.

S1-C spec §10 classifies this forestry-practice copy as **management guidance**, not measured evidence. It must not be worded as a universal prescription or a single correct answer.

FR-6 remains binding: only claim causality when the recorded causal account actually supports it. Otherwise use co-occurrence/inspection framing such as "you treated this area in year X; inspect what is there now" rather than "this treatment caused Y".

## Exclusions / invariants

- no new simulation facts or coefficients;
- no universal thinning/release interval or prescription;
- no waypoint to an invented/unrecorded location;
- no new lesson/curriculum IDs or MFM-09 teaching;
- no second history UI or map-history overlay;
- no MFM prescription/proposal/approval logic;
- no Work Plan commitment-semantic change;
- no save/schema/validator/objective-history persistence change;
- no ecology/economy/simulation change;
- preserve Reference Future and compatibility anchors;
- no Windows build.

## Verification

### Offline/source/copy audit

- map every guidance sentence/template to exact recorded field(s) and/or current observation;
- prove read-only history access and no save/schema/version change;
- audit copy for unsupported causal language and universal prescriptions;
- prove missing-location cases do not produce an invented waypoint;
- confirm final T-04 provenance semantics remain intact, including the DERIVED Management History count heading.

### Unity / rendered gate

At **1280×720** and **1920×1080**:
- render at least **two distinct intervention histories** with useful bounded guidance;
- exercise at least one case with a faithful recorded location and verify the offered waypoint/look action reaches that recorded target/area;
- exercise at least one missing-location case and verify no fabricated location/waypoint is offered;
- browse an earlier year and prove the operation remains read-only and does not acknowledge/change the latest review;
- verify T-04 provenance labels remain correct and legible;
- run affected T-02/T-03/T-04 focused regressions.

The narrow question is: **can the player use recorded prior treatment to know what to go and inspect before making another judgement, without the game inventing history or prescribing the decision?** Stop once that is evidenced.

## Stop conditions

STOP and return to Manager if:
- useful guidance requires a fact/location the save does not record;
- the design needs a new persisted fact or schema change;
- the copy requires ecological causality not present in the recorded causal account;
- the implementation needs a new curriculum/lesson flow;
- another live writer owns `AnnualReviewView.cs`, `ManagementHistoryText.cs` or the chosen history helper;
- merging/rebasing onto a newer integration lineage produces a semantic conflict with the final approved T-04 provenance contract.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-06/HANDOFF.md` with exact start/end SHA, branch/worktree/model/provider, production files changed, guidance case → source record(s) → wording → waypoint-source matrix, rendered/waypoint evidence at both resolutions, copy audit/regressions, read-only/save invariance evidence, residuals, cleanliness and Unity-lock state.

Success token:
`S1-C PRIOR-TREATMENT GUIDANCE QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line

`Run claim #147 from Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md. Start from exact pushed lineage 3ea4af8b (final Manager-approved T-04 production 633775fd8650db3cf3482e550d106a0febd7b9cb plus docs-only handoff), create feature/s1-c-t06-prior-treatment-guidance-v1 in /media/jer/Videos/CCF-S1C-T06, refresh live ownership first, and implement only bounded L4 prior-treatment guidance from recorded history plus clearly separated current observation. Do not invent missing history/location, do not prescribe a forestry decision, and do not add persistence/curriculum/simulation/MFM changes. Preserve the final T-04 provenance contract including the DERIVED Management History count heading. Run the offline/copy audit and two-resolution Unity checks including faithful-waypoint and missing-location cases, publish HANDOFF.md, and return the success token. No Windows build.`

**User action required:** dispatch the copy/paste line when ready; no Manager decision currently blocks this task.
