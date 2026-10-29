# S1-C T-04 — Manager Review

**Task:** S1-C T-04 — provenance labelling  
**Claim:** #123  
**Initial production candidate:** `d524de6ccc3de2366f39bdc939ce7c00657ba93c` — NEEDS CHANGES  
**Round-2 production candidate:** `ba2922e57467326beb4571e71d5373d73eaaa049` — superseded by final repair  
**Final production candidate reviewed:** `633775fd8650db3cf3482e550d106a0febd7b9cb`  
**Final pushed handoff successor:** `3ea4af8b` — docs-only after final production SHA  
**Final verdict:** **APPROVED**

## Final Manager verdict

`633775fd8650db3cf3482e550d106a0febd7b9cb` is the **Manager-approved S1-C T-04 production candidate**. This supersedes the prior candidate approval at `ba2922e...`; it does not change the task scope or integration status. No Windows build is authorised.

The final repair is bounded to `Assets/ForestPrototype/UI/AnnualReviewView.cs`: the `MANAGEMENT HISTORY · N intervention(s)` heading contains a render-time grouped intervention count and now carries an explicit **DERIVED** provenance label. The final commit also adds focused offline/rendered assertions for that heading. `ManagementHistoryText.cs` is unchanged from the round-2 corrected candidate.

Comparison from final production SHA `633775fd...` to pushed handoff successor `3ea4af8b` shows only `Docs/Handoffs/S1-C-Implementation-04/HANDOFF.md` changes; there is no later production change.

## Findings disposition

### Finding 1 — presentation aggregations: RESOLVED

All identified display-time counts/sums/groupings are labelled **DERIVED** unless the exact displayed scalar/record is persisted. The final repair closes the remaining unlabelled aggregate: the Management History intervention-count heading.

The accepted split remains:

- work-card counts/groupings, intervention counts, failure/cancellation counts, harvest order count and similar render-time summaries: **DERIVED**;
- persisted annual-report scalars, event fields, recorded reasons, settlement money and other exact saved facts: **RECORDED**;
- underlying drill-down events/orders remain recorded facts.

### Finding 2 — storm provenance: RESOLVED

Storm salvage-to-date is a separate **DERIVED** line. Deadwood remaining now is **CURRENT** and appears only on the latest review, not under an earlier storm year. The rendered fixture directly exercises the earlier-year case.

### Finding 3 — drill-down provenance: RESOLVED

Each drill-down entry separates recorded title/facts/reason rows from a dedicated **NOT RECORDED** row naming only absent values. A missing field no longer relabels recorded facts.

## Accepted final verification

Published third-round evidence on exact production SHA `633775fd8650db3cf3482e550d106a0febd7b9cb`:

- T-04 offline provenance: **56 PASS**;
- S1-C history offline: **55 PASS**;
- T-03 offline: **72 PASS**;
- ThreeSpecies history offline: **74 PASS**;
- T-04 rendered fixture at **1280×720** and **1920×1080**, including earlier storm year and old-save cases: **529 PASS**, 0 failed;
- T-03 rendered regression: **279 PASS**, 0 failed;
- the rendered fixture explicitly asserts the Management History heading as DERIVED at both resolutions;
- old-save RECORDED / DERIVED / NOT RECORDED cases and read-only browsing invariance remained green;
- worker inspected relevant captures;
- Unity lock released; U1 restored clean to `c8cfae8c4025cec6e960a050edf4bfde2a981de5`.

## Accepted residuals

Non-blocking for T-04:

- DBH-growth labelling in `TreeInspectionView` remains outside the owned Annual Review/history surface.
- The stale T-05 `CurrentVersion is 22` assertion is unrelated and belongs to the appropriate current save-lineage work.
- T-05 Unity render need not be rerun solely for this bounded provenance repair.
- No Windows build was run or authorised.

## Continuation

Claim #123 remains closed and owns no files. Downstream S1-C work must use the final pushed T-04 lineage `3ea4af8b`, which contains approved production `633775fd...` plus the docs-only final handoff. S1-C T-06 / claim #147 must be repinned from the superseded `cff79f0...` lineage before dispatch.

---

## Audit history

The initial candidate `d524de6...` returned NEEDS CHANGES for three provenance defects: display aggregates mislabelled RECORDED, mixed/current storm state under historical review, and mixed drill-down provenance. Round 2 at `ba2922e...` corrected those three broad findings and passed the focused gates, but a later audit found one remaining unlabelled display aggregate in the Management History heading. Final candidate `633775fd...` fixes that last gap and is the accepted T-04 candidate.