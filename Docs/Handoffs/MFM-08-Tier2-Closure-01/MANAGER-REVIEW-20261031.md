# MFM-08 Tier-2 Closure 01 — Manager Review — 2026-10-31

## Verdict

**NEEDS CHANGES**

The closure task itself was executed correctly and returned a useful blocker rather than over-claiming success. MFM-08 #156 is **not yet Manager APPROVED** because the authoritative RackV2MapPlanning gate did not complete on the exact MFM-08 candidate.

This is **not a rejection of MFM-08 feature behavior**. The returned evidence isolates one MFM-specific verification-contract mismatch and separately shows that the three existing Rack red gates are not attributable to MFM-08.

## Reviewed evidence

Returned handoff:

- `Docs/Handoffs/MFM-08-Tier2-Closure-01/HANDOFF.md`
- branch `verify/mfm-08-tier2-closure-v1`
- handoff head `911a8a34`
- exact MFM-08 production candidate `e5fb424635f16182748efcb0539e8a49539495a9`
- exact current integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

The worker acquired and released the authoritative Unity lock normally, changed no production/test/scene files, and returned the lane clean.

## Finding 1 — RackV2MapPlanning is blocked by a stale exact-count expectation

On exact MFM-08 candidate `e5fb4246...`, RackV2MapPlanning stopped at check 17 on the assertion that "five navigation tabs remain navigation". The MFM-08 V2 Stand Map UI adds the authorised `Treatment review` navigation layer, so the old exact-five count no longer describes the accepted V2 UI.

The same gate passes all 142 checks on feature start `cfdc93bf...`. Therefore the failure is introduced by the MFM-08 UI addition, but the evidence supports classifying it as a **verification-contract mismatch**, not as evidence that the authorised Treatment Review feature is wrong.

### Manager decision

**AUTHORISE a bounded verification-only reconciliation.**

The repair must not be a blind `5 -> 6` replacement. The reconciled contract must prove:

1. the original five navigation tabs remain present and retain their navigation role;
2. the authorised V2 `Treatment review` tab is additive and does not displace or rename those five;
3. the historical/non-MFM state still retains the five-tab expectation where applicable;
4. no unrelated MapPlanning assertion is weakened.

No production, UI, scene, save, calibration, spec or protected-anchor change is authorised.

Success requires the **complete authoritative RackV2MapPlanning gate** to run and pass on exact production candidate `e5fb4246...` plus only the bounded verification patch.

Successor claim: **#196 — MFM-08 MapPlanning Contract Reconciliation 01**.

## Finding 2 — guided-v2 / combined-v2 / combined-legacy are not MFM-08-specific

The current integration `ce673714...`, which does not contain the MFM-08 production delta, remains red:

- `combined-v2`: 447 pass / 11 fail, matching the MFM-08 candidate and feature start;
- `combined-legacy`: 440 pass / 8 fail, matching the MFM-08 candidate and materially improved from 416 / 32 on feature start;
- `guided-v2`: the compared first failure line matches candidate/feature-start behavior, while current integration also has fresh-rack-review sequence failures.

That evidence is sufficient to reject attribution of these reds to MFM-08. It is not sufficient to classify or waive the Rack failures themselves.

### Manager decision

**OPEN a separate Rack V2 current-lineage diagnosis.**

The diagnosis must classify each failing assertion/family before any repair is authorised. It begins browser/static/read-only and owns no production/test edits. A future production or verification repair, if actually required, needs its own bounded claim.

Successor claim: **#197 — Rack V2 Current-Lineage Red-Gate Diagnosis 01**.

## Effect on MFM-08 #156

#156 remains **NEEDS CHANGES — VERIFICATION CLOSURE ONLY** pending #196.

The unrelated inherited/current-lineage Rack reds under #197 do **not** automatically block MFM-08 approval. If #196 demonstrates the full authoritative MapPlanning gate passes under the correctly reconciled, non-weakened contract and no new MFM-specific failure appears, the Manager may upgrade #156 to APPROVED while #197 continues separately, unless #197 finds evidence that directly invalidates accepted Rack V2/D-064 behavior required by MFM-08.

The already-qualified MFM-08 behavior (353/353 successor gate and strengthened 373-check conflict gate at both render sizes) remains accepted evidence; it is not rerun by default.

## Claim disposition

#195 is complete as a closure/diagnostic execution and should be closed after this review is posted. Its outcome is **NEEDS CHANGES**, with executable successors #196 and #197.

## Next sequence

1. Run #196 first in the serialized Unity lane after fresh preflight/acquisition.
2. Run #197 browser/static diagnosis in parallel where it does not consume the Unity lane.
3. On #196 handoff, Manager reviews the exact verification diff and full MapPlanning result and decides whether #156 can be upgraded.
4. #197 returns a separate diagnosis and any proposed repair packet; do not silently couple it back into MFM-08 unless evidence shows a direct dependency.

No Windows build is authorised.
