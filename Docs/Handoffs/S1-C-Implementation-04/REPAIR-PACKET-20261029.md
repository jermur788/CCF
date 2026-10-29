# S1-C T-04 provenance labelling — bounded repair packet

**Claim:** #123  
**Status:** RELEASED FOR BOUNDED REPAIR  
**Parent:** #91 / #106  
**Difficulty:** **2/5** — three localized provenance/presentation corrections on an already qualified branch; requires focused Unity rerun but no architecture or persistence change.  
**Best-suited executor:** existing Claude Code / Anthropic Claude Sonnet 5.5 Medium-or-Ultra lane; T3 Code is also suitable.  
**Branch/worktree:** `feature/s1-c-t04-guide-monotonicity-v1` is NOT this task. Use existing `feature/s1-c-t04-provenance-labels-v1` / `/media/jer/Videos/CCF-S1C-T04`.  
**Reviewed production candidate to repair:** `d524de6ccc3de2366f39bdc939ce7c00657ba93c`; branch handoff tip previously `cea8a41bd537dcb9d705981eee33f9ff3d698b07`.

## Purpose

Correct only the three Manager findings in `Docs/Handoffs/S1-C-Implementation-04/MANAGER-REVIEW.md` while preserving the otherwise successful T-04 implementation and evidence.

## Binding corrections

1. **Presentation aggregates are DERIVED unless that displayed aggregate itself is persisted.** Counts/groupings such as “N tree(s) thinned/planted/pruned”, intervention summary strings assembled from multiple events, and similar render-time aggregation must not be labelled RECORDED merely because their inputs are recorded. Atomic persisted facts can remain RECORDED.
2. **Storm mixed provenance must be separated.** Do not put a live/current “deadwood remaining now” value and a historical/derived “salvaged since event” value under one provenance chip. Earlier-year storm browsing must not inject a CURRENT live-state line under the old-year record. Preserve the recorded storm aggregate and reconstructed victim/volume/map-square labelling.
3. **Drill-down provenance must be explicit per displayed fact.** Do not use one NOT RECORDED chip to label a row that also contains recorded facts. Fully populated drill-down facts must also have an explicit provenance treatment. Missing target/location/execution/failure-reason facts remain NOT RECORDED without relabelling the recorded pieces.

## Scope

Allowed production files remain:
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`

Focused verification tooling under `Tools/Verification/S1CManagementHistory/` may change as needed.

Do not edit save/schema/validator, `ScenarioOneManager`, MFM, ecology/economy, scenes/prefabs, objective semantics, Reference Future, or any S1-D/MFM curriculum file.

## Verification

Before Unity, rerun the focused offline provenance check and affected S1-C history/T-03 checks. Then acquire the normal serialized Unity lock and rerun:
- T-04 rendered gate at **1280×720** and **1920×1080**;
- a fixture that explicitly browses an **earlier storm year** and proves no CURRENT storm/live-state line is presented under that year;
- affected T-03 drill-down regression;
- read-only browsing/world/save invariance.

Do not weaken the harness to accept mixed provenance. Existing old-save recorded/derived/not-recorded cases must remain green.

## Handoff

Update `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` with the exact corrected production SHA and branch tip, evidence root, changed classifications, gate results and residuals. Push the branch and return:

`S1-C PROVENANCE LABELLING REPAIR QUALIFIED — READY FOR MANAGER REVIEW`

No integration and no Windows build.

## Copy/paste dispatch line

`Continue claim #123 in /media/jer/Videos/CCF-S1C-T04 on feature/s1-c-t04-provenance-labels-v1. Read Docs/Handoffs/S1-C-Implementation-04/MANAGER-REVIEW.md and REPAIR-PACKET-20261029.md. Fix only the three provenance findings: render-time aggregates => DERIVED unless the displayed aggregate is itself persisted; split storm mixed provenance and suppress live CURRENT storm state in earlier-year browsing; label drill-down facts explicitly without one NOT RECORDED chip covering recorded facts. Rerun the focused offline and two-resolution Unity gates including an earlier storm year, push the exact corrected candidate, update HANDOFF.md, and return for Manager review. No Windows build.`
