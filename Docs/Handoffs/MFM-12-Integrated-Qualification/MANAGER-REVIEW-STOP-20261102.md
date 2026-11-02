# MFM-12 Integrated Qualification — Manager Review of STOP — 2026-11-02

**Claim:** #225  
**Browser Overall:** #200  
**Returned branch:** `qual/mfm-12-integrated-v1`  
**Verdict:** **NEEDS CHANGES — PRODUCTION/SPEC DEFECT CONFIRMED**

## Decision

The MFM-12 STOP is accepted as correct. The qualification does **not** pass and neither the local success token nor the product-ready token is issued.

Blocking defect **S1** is confirmed: the accepted MFM V1 spec requires a source-sample fit replay before wider approval, with player-visible agreement/disagreement information, the sample decisions the derived rule would make differently, and relevant Share/removal consequences. The current product has a tested `ScenarioOnePrescription.CheckFit` core, but the live manager/review/approval flow does not call or present it.

This is a production/spec conformance defect, not merely missing evidence and not a defect in the core fit algorithm.

## Evidence reviewed

- Returned `Docs/Handoffs/MFM-12-Integrated-Qualification/HANDOFF.md` on `qual/mfm-12-integrated-v1`.
- Returned `Docs/Handoffs/MFM-12-Integrated-Qualification/INDEPENDENT-REVIEW.md`.
- Governing `specs/multiscale-forestry-management-v1/spec.md` §7.3.
- Governing checklist E4, E5 and J7.
- Exact production `ScenarioOnePrescription.CheckFit` implementation.
- Exact production `ScenarioOneManager.PrescriptionApproval.cs` create/approve path.
- Exact production `StandMapTreatmentReview.cs` review/approval UI path.

The independent Claude Sonnet 5.5 review returned `NEEDS CHANGES` and independently confirmed S1 from source.

## Confirmed defect boundary

The existing core already computes the required information:

- agreement removals;
- player-only removals;
- rule-only removals;
- per-Crop-Tree player/rule Share;
- player/rule basal-area removal Share;
- whether the accepted removal ceiling limited the rule.

The missing layer is the production integration/presentation path:

1. the live read path does not expose a current fit result;
2. the Treatment Review does not show the fit summary or inspectable disagreements/consequences;
3. the approval flow does not require a valid fit review before wider approval;
4. player-facing teaching copy currently promises a check that the live flow does not actually show.

## Repair classification

**PACKET ONLY — bounded production repair.**

The repair may connect the existing deterministic `CheckFit` core to the existing treatment-review/approval flow and add proportionate verification. It may not redesign the prescription rule, coverage system, candidate algorithm, Work Plan semantics, calibration, persistence model, or broader UI/navigation model.

No fit score claiming forestry correctness may be introduced. The fit remains an honesty device.

## Partial qualification evidence disposition

The stopped MFM-12 package is evidence, but it is not a qualification pass.

Accepted as scoped evidence:

- eleven offline/static gates that actually passed;
- the unchanged three pure-Beech R² misses remain an accepted C-level limitation against the unchanged 0.40 criterion;
- MFM-11's accepted extension-state calibration remains within its GAMEPLAY CALIBRATION boundary;
- post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**;
- Unity compile passed before the stop.

Not accepted as MFM-12 completion evidence:

- Unity steps 02–17, which did not run;
- any rendered qualification, because zero rendered screenshots were produced;
- exact-candidate save/load/malformed-save, player-judgement, proposal/Work Plan, repeated-cycle, provenance/history or complete-loop claims that were not run;
- mutation-controls M14–M25, which did not execute.

The returned non-blocking verification-tool findings (stale MFM-08 boundary pin, stale MFM Coverage World save-version assertion, stale v23 M14 mutation target) must be repaired or explicitly reconciled before the fresh full MFM-12 qualification can claim those gates current. They are not product defects.

## Next action

Release a separate bounded repair claim for **MFM fit-replay review integration**. The repair must:

- use the existing `CheckFit` core rather than invent a replacement;
- make the minimum §7.3 information player-visible and inspectable in Treatment Review;
- ensure wider prescription approval cannot bypass a valid current fit replay/review;
- preserve proposal-vs-Fell, player judgement, Work Plan commitment, deterministic prescription, Sitka-only quantitative logic and all protected compatibility boundaries;
- add focused automated/runtime/rendered evidence and independent review.

After the repair is Manager-approved/integrated, #225 must be freshly repinned and the **full** MFM-12 matrix rerun. Do not resume from Unity step 02 as though the candidate were unchanged; the repaired UI/manager surfaces require fresh exact-candidate qualification.

## Manager verdict

`NEEDS CHANGES — MFM-12 STOP ACCEPTED / FIT-REPLAY REPAIR REQUIRED / FULL REQUALIFICATION AFTER REPAIR`

No Windows build. #194 remains NO ACTION. Tier-3 remains unreleased.