# Next-action guidance bounded tasks

- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Spec status:** DRAFT
- **Owner / updated:** Overall Manager / 2026-11-04

All future implementation tasks are BLOCKED until explicit approval. This list grants no production ownership.

| Task | Status | Owner/routing | Depends on |
|---|---|---|---|
| T-01 Resolve design and base | BLOCKED | Manager/user + independent reviewer, provider TBD | Q1–Q4 |
| T-02 State-derived cue | BLOCKED | Future UI owner, TBD | T-01 / APPROVED spec |
| T-03 Faithful teaching map | BLOCKED | Future UI/geometry owner, TBD | T-01 / approved reference |
| T-04 Qualification and acceptance | BLOCKED | Qualifier, independent reviewer, user | T-02, T-03 |

## T-01 — Design release

Goal: resolve keyboard policy, diagram source, later-lesson slice and implementation base. Scope: spec decisions and independent review. Excludes production. Verify source ledger against chosen base and retain corrections. Stop for authority conflict. Done only when Manager records approval and bounded file ownership; this worker cannot approve.

## T-02 — Cue and navigation emphasis

Goal: FR-1–4 and FR-6 on approved slice. Scope: shared transient cue projection and existing screen controls. Excludes new progression/save engine, automatic operations and unapproved key changes. Verify AC-1,2,4,5,6 with focused state tests and real input, including every named prompt over competing background states, helper branches, corrective ties and ambiguous-reason guidance fallback. Stop if a validator/backend change is needed. Done means reviewed code and linked evidence, not integrated/accepted.

## T-03 — Map-like diagram

Goal: FR-5. Scope: Objective 1 only; use approved reference geometry and map symbols, readable labels/text equivalent. Excludes new spine/diagonal layout, onboarding art and assets outside packet. Verify AC-3 at both resolutions against real map; stop if retained-row count/scale cannot be substantiated. Done requires geometry and independent visual review.

## T-04 — Behaviour and conformance

Goal: evidence for every AC, including negative/load/accessibility states. Scope: disposable fixture saves and authorised serial Unity run, no build. Excludes user's save, merges, protected-test rebaseline. Stop on admission failure, behavioural regression or product ambiguity. Done requires raw evidence, honest failed/unrun gates, independent verdict and user/Manager acceptance route; no self-approval.

## Review response

- **P1 FIXED:** FR-2 now resolves `editor.ConsentPending`, `editor.ResetPending`, `reopenPending`, then other blocking modals before every background row; only the active scope has primary emphasis/activation. Safe decline/cancel wins, with no-primary fallback when no safe modal choice exists. AC-1 tests each prompt against competing planning/confirmed states and re-derivation after dismissal.
- **P2 FIXED:** FR-3 makes candidate `RackV2Instruction()` authoritative for Objective 1 wording/stage after modal handling, projects its first unfinished step onto the screen, then applies validators. Corrections use a fixed first-eligible allowlist (map, planning, reason-matched undo, over-target selected unlocked delete); otherwise reveal the same instruction/reason. No arbitrary enabled-control selection or second instruction source. AC-6 tests helper branches, ties and ambiguous reasons.
- **P3 FIXED:** F6 was re-derived with `git show` and numbered lines at main `8f0f230190830855112e3e7a1d7544db94a7f591`; every symbol citation now names `StandMapSymbols.Build()` and its `MapSymbol` entry. Racks :86–97, navigation/grid :99–104, tree marks :106–107; not candidate 72b4efbd.

DRAFT remains unchanged. Planned canopy/no-clearing semantics, shape/pattern/text accessibility and saved-state derivation without schema changes remain required. Docs-only; no Unity or production verification claimed.
