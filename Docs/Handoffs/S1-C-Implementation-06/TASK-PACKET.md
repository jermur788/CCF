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

S1-C T-04 #123 is Manager-approved at exact production candidate:
`ba2922e57467326beb4571e71d5373d73eaaa049`.

Its pushed branch successor:
`cff79f0f35d79bb2db723d0b923658c437ebdc7f`
contains only verification/docs changes after the approved production SHA and is the exact starting lineage for this task.

Suggested branch/worktree:
- branch `feature/s1-c-t06-prior-treatment-guidance-v1`
- worktree `/media/jer/Videos/CCF-S1C-T06`

Before production edits:
1. refresh live `Docs/Project/coordination/active-tasks.md` and #91/#106/#123/#147;
2. verify #123 history/review ownership has been released and no newer writer owns the intended files;
3. verify the exact starting HEAD above and clean worktree;
4. mark #147 `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD.

A claim records execution presence only; live coordination is the ownership authority.

## Required scope

Implement S1-C FR-7 / AC-5 using the existing Annual Review/history system. From a reviewed year or intervention, expose a **short L4 explanatory teaching view** that names prior treatment relevant to what the player should inspect before another judgement.

Use only:
- persisted S1-C **L1 recorded history**;
- clearly separated **L2 current observed state** where useful;
- existing read-only `ScenarioManagementHistory` / `YearHistoryView` / intervention data rather than a competing history implementation.

Examples may identify:
- a prior thinning/release in the reviewed area;
- retained deadwood choice;
- planting/protection history;
- recorded storm loss;
- other recorded work relevant to present inspection.

Where a faithful recorded location exists, reuse the existing waypoint/navigation mechanism so the player can go and look. If the record has no faithful target/cell/position, do not reconstruct one from the current world: omit the waypoint and use honest not-recorded wording where necessary.

## Provenance and wording invariants

The Manager-approved T-04 provenance contract is binding:
- recorded facts remain visibly RECORDED;
- current observations remain CURRENT and are not presented as earlier-year facts;
- display-time calculations/groupings remain DERIVED;
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

- map every guidance sentence/template to the exact recorded field(s) and/or current observation it uses;
- prove read-only history access (no mutation);
- prove no save/schema/version change;
- audit copy for unsupported causal language and universal prescriptions;
- prove missing-location cases do not produce an invented waypoint;
- confirm T-04 provenance semantics remain intact.

### Unity / rendered gate

At **1280×720** and **1920×1080**:
- render at least **two distinct intervention histories** with useful bounded guidance;
- exercise at least one case with a faithful recorded location and verify the offered waypoint/look action reaches that recorded target/area;
- exercise at least one missing-location case and verify no fabricated location/waypoint is offered;
- browse an earlier year and prove the operation remains read-only and does not acknowledge/change the latest review;
- verify T-04 provenance labels remain correct and legible;
- run the affected T-02/T-03/T-04 focused regressions.

The narrow question is: **can the player use recorded prior treatment to know what to go and inspect before making another judgement, without the game inventing history or prescribing the decision?** Stop once that is evidenced.

## Stop conditions

STOP and return to Manager if:
- useful guidance requires a fact/location the save does not record;
- the design needs a new persisted fact or schema change;
- the copy requires ecological causality not present in the recorded causal account;
- the implementation needs a new curriculum/lesson flow;
- another live writer owns `AnnualReviewView.cs`, `ManagementHistoryText.cs` or the chosen history helper;
- merging/rebasing onto a newer integration lineage produces a semantic conflict with the approved T-04 provenance contract.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-06/HANDOFF.md` with:
- exact start/end SHA;
- branch/worktree/model/provider;
- production files changed;
- guidance case → source record(s) → wording → waypoint-source matrix;
- rendered/waypoint evidence at both resolutions;
- copy audit and focused regression results;
- read-only/save invariance evidence;
- residuals, cleanliness and Unity-lock state.

Success token:
`S1-C PRIOR-TREATMENT GUIDANCE QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line

`Run claim #147 from Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md. Start from exact pushed lineage cff79f0f35d79bb2db723d0b923658c437ebdc7f (Manager-approved T-04 production ba2922e plus verification/docs only), create feature/s1-c-t06-prior-treatment-guidance-v1 in /media/jer/Videos/CCF-S1C-T06, refresh live ownership first, and implement only bounded L4 prior-treatment guidance from recorded history plus clearly separated current observation. Do not invent missing history/location, do not prescribe a forestry decision, and do not add persistence/curriculum/simulation/MFM changes. Run the offline/copy audit and two-resolution Unity checks including faithful-waypoint and missing-location cases, publish HANDOFF.md, and return the success token. No Windows build.`

**User action required:** dispatch the copy/paste line when ready; no Manager decision is currently blocking this task.
