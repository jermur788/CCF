# Final Independent Review — Scenario One Tier-3 (Task 1 checkpoint)

**Claim:** #231  
**Review date:** 2026-11-03  
**Reviewer:** OpenAI GPT-6.1-Sol, high reasoning, T3 Code read-only child session, cross-provider and independent of the qualification worker  
**Exact starting candidate:** `6849caa0ef05baf1a52b95465ec6bba4ff94202b`  
**Evidence head:** `a95c4c4e35decb7a518d543488678473430f69b6` (production diff to the candidate empty)  
**Blocked handoff reviewed:** `FINAL-TIER3-HANDOFF.md` (commit `9491d6de`)  
**Evidence root:** `/media/jer/Files/CCF/Verification/ScenarioOneFinalTier3-20261103/`

## Verdict

**`NEEDS MORE EVIDENCE`**

The stop/return is justified because qualification remains unresolved. The reviewer modified nothing, ran no Unity, and did not issue a success token.

The review found **no false green** in the retained compile, v24 history, save-safety, MFM boundary or complete-loop data, and **no confirmed production defect**.

## Q1 — Reference-preview finding

**Classification: confirmed same-frame lifecycle failure; ordinary player-facing preview defect NOT established.**

The clean receipt is valid:

- `unity/49i-economy/49i-economy.log:1196` — private return snapshot: `trees=1344 version=24 geometry=1`.
- `:1222` — `1344 → 335` tree records after preview exit.
- `:1235` — original assertion failure.
- `49f-economy.log:1210` and `:1223` independently repeat the loss and failure.
- The staged harness SHA matches the committed evidence-head harness; the candidate → evidence production diff is empty.

The diagnostic did bypass the assertion in 49g/49h by manually restoring the world before the check (`49h-economy.log:1214–1241`). Commit `a95c4c4e` removed that mutation, so **49i is not a diagnostic false green**.

**Material timing caveat:** the harness calls preview entry and exit **without yielding a frame** (`Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs:184`). Production `LoadData` includes inactive trees (`ForestSaveController.cs:331`), deactivates and schedules unwanted trees for destruction (`:379–385`), then reuses matching objects on later loads (`:401`). Immediate same-frame exit therefore reuses objects already scheduled for destruction; they disappear at frame end. This explains the missing outer forest and why a later manual load succeeds.

Retained tests with two frames **inside** preview already report exact Enlarged80 restoration:

- `47b-standgeometry/47b-standgeometry.log:1270`
- `48b-enlarged80/48b-enlarged80.log:1787`
- Timing is explicit in `Tools/Verification/StandGeometry/Enlarged80Verification.cs:334`.

**Required discriminating evidence:** a frame-separated Year-20 reference-preview round trip with the protected-player fixture. Only that result distinguishes a normal-player restoration defect from the confirmed same-frame harness race.

Also, “manual `LoadData` of the same snapshot” is imprecise: the diagnostic restored a clone of the separately captured `beforePreviewCanon`, not the reflected private `previewReturnData` object. Archive integrity is separately checked against embedded historical JSON (`ScenarioReferenceArchive.cs:56–95`); these receipts do not establish archive corruption.

## Q2 — Reduced plan / MUST KEEP coverage

Most MUST KEEP protections are retained and supported by receipts:

| Protection | Evidence |
|---|---|
| Exact-candidate compile/import | `01-compile/unity.log:1537`; production bytes identical across the qualification lineage |
| Mandatory fresh S1-C T-05 v24 save/history | `02-objective/unity.log:11407` (544 checks / 34 cases); v24 at `:1683`; malformed-data no-mutation checks from `:2741`; genuine v20/v21 fixtures in its result file |
| Recorded / derived / not-recorded rendering | `03-render/unity.log:3357`, six captures; representative states visually inspected at both resolutions |
| Current/old/malformed saves and provenance | MFM-06, hardening and T-05 receipts (`06-mfm-save/unity.log:4381`, `08-mfm-hardening/unity.log:1814`) |
| Legacy40 / frozen Reference Future | `07-mfm-reference/unity.log:6737`; `45d-rack-legacy40/legacy40.log:1185–1214`, historical Year-100 hash retained |
| MFM judgement / commitment boundaries | `10-mfm-treatment-review/unity.log:8969`; unavailable-fit refusal `:5813`; invalid-fit refusal without mutation `:6321`; proposal labelling `:6363` |
| Complete-loop / completion / economy data | MFM-11 later intervention `:3246`; Completed Year 25 cash 348152 cents `:3966`; eight objectives `:3980–4092`; completion assertion `:4106` |
| Independent review | This review; final successful qualification acceptance remains pending |

**Material gap:** both `11-mfm-calibration/evidence/mfm11-final-*.png` files are uniformly black. The harness restores baseline for Path C before taking them (`Mfm11CalibrationImpactGate.cs:587`, screenshots at `:655–662`), so even usable images there would not show Year-25 completion. No usable rendered **Completed** state substantiates the handoff’s completion-rendering claim. Other retained suites contain usable both-resolution Rack, species, history, Work Plan and MFM images; the gap concerns completion presentation, not the entire rendered matrix.

The reduced-plan rationale broadly follows amendment #238, but its accounting is not fully accurate: completion rendering is unproven, preview timing was omitted, and the “~10–15 avoided executions” is an estimate without a gate-by-gate reconciliation.

## Q3 — Rack findings

**Map planning: stale assertion.** The harness requires exactly five tabs (`RackV2MapPlanningVerification.cs:318`), while current `Assets/ForestPrototype/UI/StandMapView.cs:255` intentionally adds a sixth, “Treatment review”, tab in V2. `38-rack-map/map.log:1826–1882` already confirms the planning control remains separate from navigation. The `BadWindow` in `logs/unity/38-rack-map.log:3` is real but unnecessary to explain the failure.

**1920×1080 rotation: input-contaminated receipt; product defect unconfirmed.** `39-rack-editing/editing.log:5538` records 0°, while 720p passed at `:4250`. However, `evidence/pointer-trace.txt:198–202` shows the pointer abruptly leaving the intended destination and releasing around world `(-49.88, -30.74)`; the driver requested endpoint `(884, 729)` (`driver/driver.log:433–436`). This does not establish a clean intended rotation/release. The failure wording concerns the **angle**; the entrance is supposed to remain fixed.

## Disposition

Qualification remains **blocked / needs more evidence**. Keeping it blocked avoids a false green. Declaring the normal preview path definitively defective would exceed these receipts. The next evidence actions are:

1. Frame-separated Year-20 preview round trip (protected-player fixture).
2. Uncontaminated Rack 1920×1080 rotation re-run; treat the five-tab map assertion as stale.
3. A usable rendered Completed-state checkpoint at 1280×720 and 1920×1080.

No success token is issued. Browser Overall retains acceptance authority.
