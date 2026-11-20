# Multi 2D MA-1 — implementation plan

**Status: APPROVED — MA-1 PRODUCT/SPEC SCOPE ONLY, 2026-11-20. NOT IMPLEMENTED, NOT REVIEWED SOURCE, NOT PLAYER-ACCEPTED.** Owner directly approved the limited G5-MA1 design: two distinct synthetic Sitka areas; isolated per-area ecology; one shared atomic Work Plan; manual Crop/Fell in both; no Fell-to-empty by player; naturally empty areas remain empty while populated areas advance. Owner explicitly requested **do not start source implementation yet**. Manager #885 has finalised the scoped specification. MA-2 remains DRAFT, scientific positive G1 BLOCKED. This approval never grants a source worktree, physical lock, C#/Unity/fixtures changes, integration, merge, save or Windows build.

## Delivery order — one controlled owner-visible feature
**P0 Admission (NOT STARTED):** after #882 owner playtest outcome, inspect current main/feature branch and source lineage. Verify exact Git HEAD/working tree, area of change/compilation/commit reach, live coordination board, open worktree and physical locks; secure new single writer/branch/worktree and owner decision whether to begin coding. Do **not** infer that #882 reviewed source `e11dd5192d7a89abf859613ce92814bdd6338ea2` is merged; it was reviewed but not owner-accepted at plan authoring. Confirm no conflict with currently running #882 private player server and #867/#877. If G4-protected source expansion is needed, stop that action and seek explicit approval.

**P1 Independent synthetic area definition:** new `MultiArea`-only versioned canonical records and fixtures (per #852), two truly different forest areas and source provenance. Negative parsing and identity tests before any live UI; original P30 path untouched.

**P2 Independent annual ecology:** separate area snapshot/run/report using existing pure annual rule per area, shared experiment year, deterministic outcomes/area denominators; all-or-none computation and publication. Verify an original one-area execution oracle and the ten unchanged frozen digests.

**P3 Atomic manual cross-area Work Plan:** one immutable experiment root/lock, global session revisions and plan, per-area Crop/Fell guards, honest reasons, pending/approval/Execute/same-key replay/year cancellation, zero-survivor policy and actual receipts. Refuse whole operation on any invalid sub-order.

**P4 Minimal playable 2D view:** one authoritative selector for A/B, genuinely independent maps/inspectors, manual Crop/Fell, one combined Work Plan and per-area Year+1. Reuse existing #882 UI patterns only after inspecting exact source and obtaining *explicit disjoint* edit permission for touched JS/HTML/CSS/C#; no automatic reuse of #882's 13-file grant. Separate UI patch if source lane already owns any path.

**P5 Narrow independent feature qualification & owner G2:** candidate source SHA checked by nonauthor reviewer, Tier2 through real app/browser including negative stale/partial/replay and 1280×720/keyboard/zoom; owner personally demonstrates A+B real once-only Fell and annual consequences. Reviewer verdict APPROVED/NEEDS CHANGES/REJECTED; Manager integration verdict distinct; no claim of G2 ACCEPTED until owner reports it.

## Architecture constraints
- Use one state root for transactions; multiple `AnnualContinuation` instances independently committing are not atomic. Leverage unchanged `AnnualRules.Calculate` per area, no duplicate biological rules or independent growth fork without approved justification.
- Isolated area ecology deliberately excludes cross-boundary competitors; no hidden edge coupling or synthetic site equality claim.
- Distinct canonical IDs, seeds and areaHa; project must visibly show genuinely different A/B tree rosters. Include one wrong-area source/refusal and one fault-before-publish test.
- Retain end-to-end deterministic equivalence for original single-area protected fixture. Do not edit existing source under alleged permission from this docs plan.

## Resource/verification limits
Proposal is **small and staged**, not an assertion of low engineering cost: source graph, transaction atomicity, area identities and UI source binding may involve multiple internal files. Work happens as *one non-overlapping source implementation lane at a time*, with different owner/reviewer contexts; no speculative tooling build-out. Tier1 fast targeted; Tier2 feature negative tests; no T3 until milestone/release request. No physical reservation, branch/worktree or author is started by this plan.

## Hard stops
Any changed protected P0/save/golden, unapproved compile-linked file, shared Unity lock collision, wrong-source candidate, leaking private browser token, independent area atomicity failure, source test weakening, off-task MA-2, or full-site change necessitating new authority → stop offending action and escalate Manager #885. Other pre-authorised independent planning may continue.

Binding owner approval: 2026-11-20 21:04:04Z (Owner: "I approve the proposed limited G5-MA1 design direction. Finalise only the MA-1 spec and prepare the bounded implementation packet; do not start source implementation yet."). Parent manager verdict: https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-ma2-draft-manager-verdict-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MANAGER-MA1-MA2-DRAFT-SPEC-ACCEPTANCE-AND-G5-DECISIONS-20261120.md . Engineering reference (design-only): https://github.com/jermur788/CCF/blob/c23e411555771320cb327e918c7ae80887388889/Docs/Handoffs/MultiRack-Simulation-20261116/852-orchestration/Multi-852-G1-FIXTURE-AREA-IDENTITY-PER-AREA-ENGINE-AND-WORK-PLAN-DESIGN-RESULT-20261120.md . Retained MA-2 DRAFT: https://github.com/jermur788/CCF/blob/docs/multi-885-incremental-multi-area-draft-spec-gpt6-20261120-v1/specs/multi-2d-incremental-area-management-and-crop-transfer-v1/spec.md . #828 positive G1 source qualification unfulfilled; #851 empirically positive G1 NO-GO on inspected evidence.
