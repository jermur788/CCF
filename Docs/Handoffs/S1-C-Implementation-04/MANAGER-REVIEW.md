# S1-C T-04 — Manager Review

**Task:** S1-C T-04 — provenance labelling  
**Claim:** #123  
**First production candidate reviewed:** `d524de6ccc3de2366f39bdc939ce7c00657ba93c`  
**First evidence successor:** `cea8a41bd537dcb9d705981eee33f9ff3d698b07`  
**Corrected production candidate reviewed:** `ba2922e57467326beb4571e71d5373d73eaaa049`  
**Corrected branch tip / evidence successor:** `cff79f0f35d79bb2db723d0b923658c437ebdc7f`  
**Final verdict:** **APPROVED** — round-2 corrections satisfy T-04; the earlier NEEDS CHANGES verdict is superseded for the corrected candidate.

## Round-2 Manager verdict

The corrected candidate is **APPROVED** for S1-C T-04. This is candidate acceptance, not integrated-milestone acceptance. No Windows build is authorised.

The candidate remains rooted at authorised base `b971f4da0540e606bfee9702b0c99f81a0a89355`. From that base, production changes remain confined to:

- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`

with focused verification tooling and the handoff. Comparison from corrected production SHA `ba2922e...` to branch tip `cff79f0...` shows no further production change; the successor changes only the provenance verification harness and `HANDOFF.md`.

### Finding 1 — resolved: presentation aggregates are DERIVED

The corrected implementation now distinguishes the displayed aggregate from its recorded inputs:

- `ManagementHistoryText.WorkProvenance` is `DERIVED`;
- work-card counts/groupings such as felled, salvaged, planted, pruned, cancellation count, no-work and failure-count statements are `DERIVED`;
- persisted scalars such as the annual-report regeneration-removal count and individual recorded failure reasons remain `RECORDED`;
- grouped harvest presentation is split so the display-time order count/year grouping is `DERIVED`, while the persisted settlement money is `RECORDED`;
- completion/intervention counts assembled for presentation are `DERIVED`, while genuinely persisted completion/snapshot facts remain `RECORDED`.

This satisfies the packet rule that presentation aggregations are derived rather than silently promoted to recorded facts.

### Finding 2 — resolved: storm historical/current provenance is separated

The mixed storm line is split:

- salvage since the event is labelled `DERIVED`;
- deadwood remaining now is labelled `CURRENT`;
- the CURRENT line is emitted only on the latest review, not while browsing an earlier storm year.

The rendered fixture directly exercises an earlier storm year (year 3 while year 5 is latest), so this is not accepted solely on source inspection.

### Finding 3 — resolved: drill-down facts are classified per displayed row

The corrected drill-down no longer applies one `NOT RECORDED` chip to a mixed entry. Each entry separates:

- recorded title/status row;
- recorded facts row;
- recorded reason row where a reason exists;
- a separate `NOT RECORDED` row naming only the specific missing fields.

This satisfies T-04's requirement that populated recorded facts and missing provenance remain visibly distinguishable.

## Accepted verification

Worker-reported and published round-2 evidence on exact corrected production SHA `ba2922e57467326beb4571e71d5373d73eaaa049`:

- T-04 offline provenance: **55 PASS**;
- S1-C history offline: **55 PASS**;
- T-03 offline: **72 PASS**;
- ThreeSpecies history offline: **74 PASS**;
- T-04 rendered fixture: **527 PASS**, 0 failed, at **1280×720** and **1920×1080**, including the earlier-storm-year case and genuine old-save fixtures;
- T-03 drill-down rendered regression: **279 PASS**;
- captures for earlier/latest storm and drill-down states were inspected by the worker;
- Unity lock was released and U1 restored clean to `c8cfae8c4025cec6e960a050edf4bfde2a981de5`.

The first rerun's overlap-check failure was harness-only and was corrected without a production diff. T-02 did not need to be rerun because the Manager return requirements did not request it after navigation/gate logic remained unchanged; its first-round functional gate had already passed.

## Accepted residuals

These remain non-blocking for T-04:

- DBH-growth labelling in `TreeInspectionView` is outside this task's owned Annual Review/history surface.
- The stale T-05 `CurrentVersion is 22` assertion is unrelated to this task and must be repaired/rebased under the relevant save-lineage work rather than inside T-04.
- The T-05 Unity render gate was not required solely for this bounded provenance repair.
- No Windows build was run or authorised.

## Integration / continuation meaning

`ba2922e57467326beb4571e71d5373d73eaaa049` is the **Manager-approved S1-C T-04 production candidate**. `cff79f0f35d79bb2db723d0b923658c437ebdc7f` is an acceptable evidence/docs successor, not a separately reviewed production candidate.

Claim #123 may close and its history/review production ownership may be released. S1-C T-06 (prior-treatment guidance) is the next dependency-ready S1-C implementation task; it must preserve the approved provenance separation and use only recorded history/current observation for bounded L4 guidance. T-07 remains subject to current identity/save-lineage ownership.

---

## Historical first-round review — NEEDS CHANGES (superseded by the round-2 approval above)

The first candidate had three bounded defects:

1. presentation/query aggregations were labelled RECORDED rather than DERIVED;
2. the storm card mixed current deadwood with derived salvage and exposed live state under earlier-year storm review;
3. drill-down rows used a mixed NOT RECORDED chip and left populated fact rows insufficiently classified.

Those findings were the basis of the earlier return packet and are retained here for audit history. They are resolved by corrected candidate `ba2922e...` as described above.
