# S1-C T-04 — Manager Review

**Task:** S1-C T-04 — provenance labelling  
**Claim:** #123  
**Production candidate reviewed:** `d524de6ccc3de2366f39bdc939ce7c00657ba93c`  
**Branch tip / evidence successor:** `cea8a41bd537dcb9d705981eee33f9ff3d698b07`  
**Verdict:** **NEEDS CHANGES**

## Evidence accepted

The pushed candidate is rooted at the authorised `b971f4da0540e606bfee9702b0c99f81a0a89355` base. Production changes are confined to `ManagementHistoryText.cs` and `AnnualReviewView.cs`; later changes are verification/docs only. The published handoff records the required two-resolution rendered gate, genuine old-save cases, read-only browsing, T-02/T-03 regressions, and no save/schema/validator/simulation change. The worker correctly withheld Manager acceptance.

The reported rendered and regression evidence is sufficient to avoid rerunning the entire feature from scratch after the corrections below. The next candidate must rerun the focused provenance/offline gate and the rendered T-04 fixture at both resolutions; rerun any affected T-03 assertion if its rendered structure changes.

## Finding 1 — presentation aggregations are misclassified as RECORDED

The T-04 packet explicitly defines presentation aggregations as **DERIVED**. The candidate instead treats several query/render-time aggregates as **RECORDED**.

Examples:

- `ManagementHistoryText.WorkProvenance` returns `Recorded` although `Work(...)` builds a presentation aggregate from `InterventionSummary` counts.
- Annual Review work lines such as `N windthrow stems salvaged`, `N trees thinned`, planting/pruning counts, `No work was resolved this year`, and `No tasks failed` are produced by counting/querying recorded events at render time. They are not themselves stored in the displayed form.
- Completion/history statements such as the number of interventions are likewise presentation aggregates unless the exact displayed value is itself a persisted field.

**Required correction:** classify the displayed aggregate as `DERIVED`, or split a line where necessary so a genuinely persisted scalar can remain `RECORDED` while the calculated aggregate is `DERIVED`. Do not relabel the underlying events/orders themselves; drill-down records remain recorded facts.

## Finding 2 — the storm line mixes provenance layers and leaks live state into an earlier-year card

`BuildStorm` emits one `CURRENT` line containing both the live `DeadwoodRemainingM3` value and `SalvagedVolumeM3 ... since this event`. Those facts are not the same layer: the remaining deadwood is current observed state, while salvage-since-event is a historical/query-derived quantity. One line therefore carries mixed provenance.

The same live-current line is emitted whenever an earlier storm year is browsed. The task packet requires historical-year presentation not to place live forest facts under the old-year record unless separated as an explicitly current-state surface.

**Required correction:** split the mixed storm line. Keep historical/query-derived salvage information under the appropriate historical/derived label. Show the live `deadwood remaining now` value only on the latest review, or move it into a clearly separate current-state section that is not presented as part of the old-year storm record.

## Finding 3 — drill-down rows need a single honest classification per displayed fact

`BuildDetailItem` currently applies a `NOT RECORDED` chip to the entry header if *any* field is missing, while the following untagged facts line can mix recorded values with missing-value text. Fully populated detail entries can also present the facts line without an entry-level provenance label.

T-04's requirement is not merely that an entry contains some provenance chip; every history/review value must be visibly distinguishable by layer.

**Required correction:** either tag each logical detail line/value group explicitly, or split missing fields into `NOT RECORDED` rows while keeping recorded event/order facts visibly `RECORDED`. Avoid a single chip that implies the whole mixed entry has one provenance when it does not.

## Accepted residuals

- DBH-growth labelling in `TreeInspectionView` remains outside this task's owned Annual Review/history surface. No change required in #123.
- The stale T-05 `CurrentVersion is 22` assertion is unrelated to this task and remains a harness-maintenance residual.
- The T-05 Unity render gate does not need to be rerun solely for these T-04 corrections; the focused T-04 old-save fixture is the relevant gate.
- No Windows build is authorised.

## Required return

Keep the same branch/worktree and bounded ownership. Correct the three findings above, rerun:

1. T-04 offline provenance checks;
2. T-04 rendered fixture at 1280×720 and 1920×1080, including an **earlier storm year** case so the historical/current separation is directly exercised;
3. affected T-03 drill-down regression if row structure changes;
4. the existing S1-C history offline regression.

Push an exact corrected production SHA, update `HANDOFF.md`, and return the same success token for a second Manager review. Do not integrate yet.
