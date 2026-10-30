# S1-D T-05 — Final Manager Review Checklist

**Prepared:** 2026-10-30 by browser lane #184  
**Target production/handoff head:** `4ddf4aa535d4ec5267be46d2635679411d595e0f`  
**Status:** REVIEW CHECKLIST ONLY — NO RUNTIME VERDICT YET

Use this immediately when #151 publishes its completed Unity handoff. Do not issue APPROVED from the offline evidence alone.

## 1. Identity / scope

- Confirm the Unity handoff tests exact production lineage `4ddf4aa535d4ec5267be46d2635679411d595e0f`, with any verification-only successor clearly separated.
- Confirm production remains the reviewed `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1` plus the accepted verification/handoff lineage and no unreviewed repair.
- Compare changed files against the accepted bounded scope: `WorkPlanView`, authorised read-only Annual Review exact-year seam, smallest root route, verification/docs only.
- Reject unexpected save/schema/hash/reference/MFM-09/economy/ecology/completion changes.

## 2. Required Unity/import evidence

- Unity 6000.6.0f1 import/compile completes without relevant errors.
- Exact gate/runner code is published or otherwise durable enough to inspect; no local-only success claim.
- Machine-wide lock was properly acquired/released, U1 returned clean, no Editor left running and host saves/world state were restored/unchanged as required.

## 3. Three-cycle presentation

At both **1280×720** and **1920×1080**, inspect durable captures/evidence showing the repeated-intervention presentation remains readable over the required three-cycle fixture:

- Approved work;
- Pending new (`Pending && createdYear == CurrentEcologicalYear`);
- Pending carried over (`Pending && createdYear < CurrentEcologicalYear`);
- future-year inconsistent records are not falsely classified as new/carried;
- grouping/foldouts/layout remain legible without clipping or hiding required evidence.

## 4. MFM / commitment boundary

Require a direct witness that prescription-generated work is ordinary **Pending** after prescription approval and before ordinary Work Plan approval. It must appear in the appropriate Pending grouping and must not be represented as committed/Approved merely because the prescription was approved.

No proposal state may be imported into Work Plan as though it were ordinary work before the accepted MFM-07 boundary creates Pending work.

## 5. Exact-year history navigation

From a prior/carried Work Plan item, prove the creation-year link:

- resolves only to an existing `Manager.HistoryYears` entry equal to `createdYear`;
- opens that exact Annual Review year rather than latest/current;
- preserves exact-year selection through the screen-opening/first-refresh path;
- absent year returns no invented destination/fallback;
- browsing remains read-only and does not acknowledge/mutate history or current world state;
- wording remains creation/origin-year wording, not approval-year or causal overclaim.

## 6. AC-5 completeness witness

Verify the rendered/runtime path still makes required **Executed** and **unplanned Fell-mark** information visible where the S1-D acceptance criterion requires it. The independent offline review specifically left this as a Unity residual.

## 7. Regression evidence

Confirm the required S1-C/S1-D and MFM regressions remain green on the exact candidate, including the previously accepted focused matrix 219/219 + 7/7 mutations or a justified current equivalent. Protected/reference/save fingerprints must not change outside accepted authority.

## 8. Final verdict

Issue exactly one Manager verdict after inspecting the completed durable handoff/evidence:

- `APPROVED` — all required runtime/rendered residuals are directly evidenced and scope is clean;
- `NEEDS CHANGES` — bounded defect/gap with smallest next action;
- `REJECTED` — fundamental mismatch/regression.

If APPROVED, perform or authorize safe integration only after fresh current integration/ownership checks. Record exact integration merge/docs successor separately from the feature verdict.

No Windows build.
