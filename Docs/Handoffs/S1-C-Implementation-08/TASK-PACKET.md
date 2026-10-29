# S1-C T-08 — Review / Management History Feature Qualification

**Parent:** #91  
**Spec:** `specs/scenario-one-review-history-v1/` T-08  
**Status:** PREPARED / BLOCKED on T-04, T-06 and T-07 completion  
**Purpose:** perform the bounded Tier-2 Unity qualification of the complete S1-C history/review slice and return exact evidence for independent browser/spec-conformance review.

## Difficulty and routing

- **Difficulty:** High — cross-cutting save/history/UI/rendered qualification with old-save and grouped-money honesty requirements.
- **Preferred executor:** Claude Sonnet 5.5 Medium / Anthropic T3 Code lane, unless local complexity justifies Claude Opus 5.5 Low.
- **Independent conformance:** browser/Manager may inspect published evidence; use OpenAI 6.1 Sol for cross-provider review if a code/save finding requires independent model review.

## Start gate / base

Do not start until S1-C T-01…T-07 are accepted on the active lineage, including the already reported T-05 v22 work and the new T-04/T-06/T-07 slices. Start from one exact current integration SHA after MFM-06/PARITY save-path work that is already accepted at that time. Refresh #91/#106/active-tasks and acquire Unity only through the normal machine-wide lock.

Suggested branch/worktree: `qual/s1-c-review-history-v1` / `/media/jer/Videos/CCF-S1C-T08`.

Create a child claim only when all dependencies are green.

## Required qualification matrix

On the exact candidate, exercise and evidence:

1. a multi-year session with at least three completed years;
2. earlier-year navigation and return to latest year;
3. two different intervention types with drill-down;
4. a grouped-harvest year with job-level money honesty;
5. a no-work year;
6. recorded/current/derived/reconstructed/session-only/not-recorded provenance states;
7. prior-treatment guidance plus any recorded-location waypoint;
8. objective-achievement history;
9. save/load round-trip on the current schema;
10. genuine pre-feature/old-save load with exact derivation vs `not recorded` behaviour;
11. stable identity/data-contract checks;
12. Annual Review acknowledgement semantics unchanged;
13. no history mutation from browsing/querying.

Rendered evidence at 1280×720 and 1920×1080 must include the material states above without overflow/clipping.

## Protected gates

- frozen Reference Future v1 unchanged;
- Legacy40/pre-feature semantics preserved;
- MFM v23 provenance not lost or mislabelled;
- S1-D management-event identity hardening preserved;
- no protected hash/anchor silently rebaselined;
- no second history UI/store;
- no Windows build.

If any production defect is found, STOP qualification and return an exact bounded repair packet; do not edit production under this qualification task.

## Handoff / independent review

Publish `Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md` with exact candidate SHA, commands/gates, rendered evidence paths, checklist mapping and deviations.

Browser #118 or its successor then completes `specs/scenario-one-review-history-v1/checklists/requirements.md` against published evidence and issues `APPROVED / NEEDS CHANGES / REJECTED`.

Success token: `S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`

## Copy/paste dispatch line

`Run S1-C T-08 from Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md only after T-01..T-07 are accepted on one exact integration candidate. Use the T3 Unity lane with Claude Sonnet 5.5 Medium by default. Execute the complete multi-year/history/old-save/grouped-harvest/no-work/provenance/rendered matrix at 1280×720 and 1920×1080; do not fix production inside qualification. Publish the exact candidate handoff for independent spec-conformance review. No Windows build.`

**User action required now:** none; blocked on preceding S1-C implementation tasks.
