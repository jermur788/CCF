# S1-C T-04 — Provenance Labelling Pass

**Parent:** #91  
**Claim:** #123  
**Spec:** `specs/scenario-one-review-history-v1/` T-04  
**Status:** **RELEASED FOR CODE EXECUTION**  
**Exact base:** `b971f4da0540e606bfee9702b0c99f81a0a89355`

## Purpose

Make every history/review value visibly honest about whether it is **recorded**, **current**, **derived**, **reconstructed**, **session-only**, or **not recorded**, without adding persistence or inventing historical facts.

The base already contains the S1-C per-year query/year browser/intervention drill-down, v22 objective-achievement history and MFM-06 v23 provenance foundation. T-04 is therefore a bounded presentation/query-labelling pass, not a new history architecture.

## Difficulty and routing

- **Difficulty:** **3/5** — bounded UI/copy/query work, with player-facing provenance and causality correctness plus two-resolution rendered verification.
- **Preferred worker:** **Claude Sonnet 5.5 Medium or stronger Claude Code lane**.
- **Alternative:** OpenAI Sol/Codex.
- Independent cross-provider copy/source review is useful but not mandatory unless implementation unexpectedly touches persistence/validation.

Suggested lane:

- branch `feature/s1-c-t04-provenance-labels-v1`
- worktree `/media/jer/Videos/CCF-S1C-T04`

Before edits, refresh #91/#106/#121/#122/#123 and live `Docs/Project/coordination/active-tasks.md`; verify exact base/clean lane and mark #123 `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD. The claim itself does not grant ownership.

## Expected owned production surface while RUNNING

Primary:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`

Only if strictly required for presentation/query classification:

- `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs`
- focused verification files

Do **not** edit save/schema/validator/hash/reference layout, `ScenarioOneManager` work-order/proposal creation, MFM-07 prescription approval logic, ecology/economy rules, scenes/prefabs/packages/ProjectSettings, or frozen Reference Future v1. If an excluded/shared surface becomes necessary, stop before editing and return to the Manager.

## Required provenance mapping

### RECORDED

Persisted when the fact occurred: annual-report values, management events, surviving recorded order facts, v22 objective-achievement records, recorded ecological snapshots and the persisted fields of storm events. Persisted v23 prescription/work provenance may be labelled recorded if a history item already exposes it; T-04 must not invent a new MFM history UI.

### CURRENT

Live present-state observation only. Current facts must never appear under an earlier-year date. Preserve the existing historical-year suppression of live browsing/protection/current-stock facts.

### DERIVED

Computed at query/render time from recorded/current data: snapshot deltas, trend strips, presentation aggregations, grouped-harvest attribution and exact historical derivations permitted by the approved S1-C rules. `Derived` means the displayed fact was not itself stored in that form; it does not mean approximate.

### RECONSTRUCTED

Historical detail recovered from other surviving records rather than stored at that granularity. In particular, storm victim identities/dimensions/count/location detail recovered from mortality/deadwood/tree records must be visibly labelled reconstructed where shown; the persisted storm aggregate event itself remains recorded.

### SESSION-ONLY

Facts the approved spec says do not survive save/load, including last-session browse/DBH-growth style facts where surfaced in history/review context. Wording must say they are not durable history after load.

### NOT RECORDED

Use where provenance is absent, including legacy/pre-feature gaps and missing target/location/execution detail. Never infer a missing past fact from today's forest merely to avoid this label.

## Causality boundary

Preserve S1-C FR-6. A before/after change or same-period observation is not proof that the work caused it unless authoritative simulation records carry that causal relationship. Labels must not create causal implication the underlying records do not support.

## Required verification

### Offline/static

Prove:

- every applicable class above has explicit player-facing treatment;
- derived/reconstructed/session-only content cannot silently present as recorded;
- historical years do not inject current-state facts;
- missing provenance remains `not recorded`;
- grouped-harvest money stays job-level, never per order/tree;
- existing no-causality wording remains;
- no save/schema/simulation behaviour diff.

### Unity/rendered

Under the normal machine-wide Unity lock:

- import/compile on the exact candidate;
- Annual Review/history examples covering **Recorded**, **Current**, **Derived**, **Reconstructed**, **Session-only**, and **Not recorded** where the current product state supports them;
- genuine old/pre-feature save demonstrates labelled derivation or `not recorded`, never invented value;
- 1280×720 and 1920×1080 rendered readability;
- earlier-year browsing remains read-only and acknowledgement semantics remain unchanged;
- affected S1-C T-02/T-03 regressions remain green.

Do not weaken gates to obtain green output.

## Stop conditions

Stop and return to Manager if:

- a new persisted field/schema bump is required;
- a historical label would require fabrication from current state;
- MFM-07 proposal/work-order creation or save validation must be changed;
- a live writer already owns an allowed file;
- copy would overstate causality.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` with actual model/provider, exact start/end SHA, branch/worktree, changed files, provenance matrix with concrete examples, offline/static results, Unity/rendered evidence, old-save evidence, regressions/residuals and confirmation of zero schema/save/simulation-behaviour change.

On handoff, rename #123 `[HANDED-OFF]`, link the handoff, close the claim and return:

`S1-C PROVENANCE LABELLING QUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

## Copy/paste dispatch line

`Run claim #123 from Docs/Handoffs/S1-C-Implementation-04/TASK-PACKET.md. Root from exact qualified base b971f4da0540e606bfee9702b0c99f81a0a89355. Implement only S1-C T-04 provenance labelling on Annual Review/history: recorded/current/derived/reconstructed/session-only/not-recorded, preserving causal and old-save honesty. Do not edit save/schema/validator, MFM proposal/work-order creation, ecology/economy or Reference Future. Refresh live ownership before edits, mark #123 RUNNING with the actual lane, serialize Unity normally, and publish the exact handoff. No Windows build.`
