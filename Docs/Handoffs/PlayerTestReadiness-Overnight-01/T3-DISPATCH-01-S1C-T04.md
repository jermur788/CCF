# T3 Dispatch 01 — S1-C T-04 Provenance Labelling

**Status:** PREPARED FOR DISPATCH — not running by publication alone  
**Claim:** #123  
**Parent:** #91 / T3 lane #106  
**Authority:** `specs/scenario-one-review-history-v1/` T-04 and `Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md`

## Purpose

Make Annual Review / management-history presentation visibly distinguish recorded historical fact, current observed state, derived comparison, reconstructed detail, session-only fact, and not-recorded state without adding persistence or inventing causality.

## Difficulty / routing

- **Difficulty:** **3/5**
- **Recommended executor:** **Claude Sonnet 5.5 Medium** in Claude Code/T3.
- **Claude Sonnet 5.5 Ultra Code:** **not needed**. The task is bounded and mostly presentation/provenance semantics rather than a multi-agent architecture problem.

## Exact base and lane

- Exact authorised production base: `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- Suggested branch: `feature/s1-c-t04-provenance-labels-v1`.
- Suggested worktree: `/media/jer/Videos/CCF-S1C-T04`.

Before edits refresh #91/#106/#121/#122/#123/#127 and live `Docs/Project/coordination/active-tasks.md`; verify the intended files are unowned, then mark #123 `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD.

## Owned surface

Primary expected files:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- only the smallest history-query/helper surface required for provenance presentation

Do not edit save schema/validator, MFM proposal/approval code, prescription persistence, simulation/economy/ecology or unrelated teaching files.

## Required semantics

- **Recorded:** persisted reports/events/orders/objective achievements and other L1 records.
- **Current:** live state, explicitly current and never presented as an old-year fact.
- **Derived:** render-time deltas/trends/money aggregations/grouped-harvest attribution.
- **Reconstructed:** aggregates recovered from persisted lower-level records rather than stored as that exact aggregate event.
- **Session-only:** facts that do not survive save/load; label them as such when shown in review/history.
- **Not recorded:** absent legacy/target/location/provenance facts; never infer from current state.

No new persisted field, save bump, causal invention or second history store/UI.

## Verification

- focused offline/copy checks;
- rendered Annual Review evidence at 1280×720 and 1920×1080;
- at least one applicable example for each provenance class;
- old-save case must show labelled derivation or `not recorded`, not invented values;
- relevant review/history regressions;
- Unity only under the machine-wide lock.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` with exact base/end SHA, changed files, provenance mapping, rendered evidence and residuals.

Success token:

`S1-C PROVENANCE LABELLING QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch

`Run #123 S1-C T-04 using Docs/Handoffs/PlayerTestReadiness-Overnight-01/T3-DISPATCH-01-S1C-T04.md and the canonical Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md. Use Claude Sonnet 5.5 Medium. Root from exact SHA b971f4da0540e606bfee9702b0c99f81a0a89355 after refreshing live ownership. Implement only provenance labelling in Annual Review/history, preserve recorded/current/derived/reconstructed/session-only/not-recorded distinctions, add no persistence, and render both target resolutions. Publish the exact handoff. No Windows build.`
