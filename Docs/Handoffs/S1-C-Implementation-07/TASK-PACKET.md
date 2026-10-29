# S1-C T-07 — S1-D Data Contract and Stability Checks

**Parent:** #91  
**Spec:** `specs/scenario-one-review-history-v1/` T-07  
**Status:** PREPARED / NOT DISPATCHED  
**Purpose:** turn the approved S1-C identity/history compatibility contract into code-facing notes and verification checks so S1-D/MFM consume one stable record model.

## Difficulty and routing

- **Difficulty:** Medium — bounded source/verification work, but identity drift can corrupt later history/repeated-intervention reasoning.
- **Preferred worker:** Claude Sonnet 5.5 Medium / Anthropic.
- **Independent review:** OpenAI 6.1 Sol if changes touch save validation rather than verification/docs only.

## Start gate / base

Start after MFM-06 is accepted/integrated and the exact current integration SHA is published. Refresh #91/#106/active-tasks and confirm there is no save/identity writer. T-01 history query implementation must remain the single derived-history path.

Suggested branch/worktree: `verify/s1-c-t07-history-contract-v1` / `/media/jer/Videos/CCF-S1C-T07`.

Create a claim only at dispatch.

## Required contract checks

Document and assert, using genuine current and old fixtures where available:

- tree identity formats and load-by-ID behaviour remain stable;
- work-order IDs use the persisted monotonic counter and resolved/failed orders remain retained;
- management-event IDs are positive/unique under the already accepted S1-D T-06 hardening and the append-only event log remains stable;
- planted-juvenile, promoted-tree, deadwood and shelter identity/linkage remain stable;
- cell identity remains geometry-relative and must not be presented as universally stable across stand-geometry changes;
- `harvestJobId` remains a resolution-year grouping label, not a unique job identifier;
- S1-C introduces no competing tree/order/event/history identity scheme;
- MFM prescription/work-order provenance consumes existing IDs rather than replacing them.

## Exclusions / invariants

- no new S1-D feature behaviour;
- no identity renumbering/repair;
- no new schema field unless a demonstrated accepted-contract defect forces STOP/Manager review;
- no fabricated old-save data;
- preserve MFM v23, S1-C v22 history, Legacy40 and Reference Future;
- no Windows build.

## Verification

- offline contract suite passes on a current fixture and at least one genuine old save;
- mutation/negative controls for ID ambiguity use existing accepted validation semantics rather than weakening them;
- field-for-field history query rebuild remains non-mutating;
- source/docs make `harvestJobId` limitation explicit wherever exposed;
- no second history implementation is introduced.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md` with exact base/end SHA, identity table, fixtures, commands/results and any compatibility caveat.

Success token: `S1-C T-07 HISTORY DATA CONTRACT QUALIFIED`

## Copy/paste dispatch line

`Run S1-C T-07 from Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md on the exact published post-MFM-06 integration lineage. Use Claude Sonnet 5.5 Medium by default. Verify and document the stable S1-C/S1-D identity/history contract with current and genuine old-save fixtures; do not add a competing identity scheme or repair historical IDs. Publish the exact candidate/handoff and stop on any legitimate compatibility conflict. No Windows build.`

**User action required now:** none; dependency-gated, not dispatched.
