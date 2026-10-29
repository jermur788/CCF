# S1-C T-04 — Provenance Labelling Pass

**Parent:** #91  
**Spec:** `specs/scenario-one-review-history-v1/` T-04  
**Status:** PREPARED / NOT DISPATCHED  
**Purpose:** make every history/review value visibly honest about whether it is recorded, current, derived, reconstructed, session-only or not recorded.

## Difficulty and routing

- **Difficulty:** Medium — bounded UI/copy work, but old-save and causal-language honesty are player-facing correctness requirements.
- **Preferred worker:** Claude Sonnet 5.5 Medium / Anthropic.
- **Independent review:** OpenAI 6.1 Sol when a cross-provider copy/source review is useful; mandatory only if the implementation changes save/validation behaviour unexpectedly.

## Start gate / base

Do not start while MFM-06 owns save/persistence or before the exact post-MFM-06 umbrella lineage is published/known. At start:

1. refresh #91/#106/active-tasks;
2. record the exact current integration SHA;
3. confirm S1-C T-01/T-02/T-03/T-05 are present on that lineage;
4. confirm no active writer owns `AnnualReviewView`, the S1-C history surface or shared copy helpers.

Suggested branch/worktree: `feature/s1-c-t04-provenance-labels-v1` / `/media/jer/Videos/CCF-S1C-T04`.

Create a child claim only when this start gate is true.

## Required scope

Implement the approved S1-C content-layer contract without adding new persisted facts:

- L1 recorded historical fact;
- L2 current observed state;
- L3 derived comparison;
- reconstructed historical summaries where source reconstruction is used;
- session-only facts with explicit post-load limits;
- `not recorded` where provenance is absent.

Required cases include:

- storm victim/count reconstruction;
- derived ecological deltas/trends;
- session-only browse/DBH-growth facts;
- pre-feature/old-save missing objective/history facts;
- any current-state value shown beside a historical intervention.

Wording must not turn co-occurrence into causality unless the authoritative record actually stores the causal relationship.

## Exclusions / invariants

- no persistence of session-only browse/DBH facts;
- no new history store or parallel history UI;
- no ecology/economy coefficients;
- no objective/completion semantic change;
- no silent old-save reconstruction;
- preserve v23 MFM state, v22 S1-C objective history, Legacy40 and frozen Reference Future;
- no Windows build.

## Verification

- offline/source checks map every surfaced fact to exactly one provenance class;
- old-save cases show labelled derivation or `not recorded`, never invented values;
- rendered examples include at least one recorded, current, derived, reconstructed, session-only and not-recorded state;
- render at 1280×720 and 1920×1080;
- affected S1-C history/navigation regressions pass;
- no save/schema diff unless an unexpected dependency forces STOP/Manager review.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` with exact base/end SHA, files, provenance matrix, rendered evidence, regressions and residuals.

Success token: `S1-C T-04 PROVENANCE LABELLING QUALIFIED`

## Copy/paste dispatch line

`Run S1-C T-04 from Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md on the exact current published Scenario One integration lineage after MFM-06 save ownership is released. Use Claude Sonnet 5.5 Medium by default. Implement provenance labels only; do not persist session-only facts or invent historical values. Render all required provenance states at 1280×720 and 1920×1080, publish the exact candidate and handoff, and stop on any save/causality ambiguity. No Windows build.`

**User action required now:** none; task remains dependency-gated and not dispatched.
