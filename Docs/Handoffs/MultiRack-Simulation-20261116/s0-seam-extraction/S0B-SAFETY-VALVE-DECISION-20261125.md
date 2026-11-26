# CCF Multi 2D — S0b Safety-Valve Decision Record (DP-S0b)

- **Decision Point:** `DP-S0b` (`SPEC-multi-2d-s0-seam-extraction-V1` §6.2–§6.3, `tasks.md` T-B3, OQ-5 default = single extraction attempt with M1/M2 only)
- **Outcome:** **`EXTRACTED`** (triggers fired: **none**; **T5** pending independent non-author reviewer confirmation below)
- **Date:** 2026-11-25
- **Packet / Branch:** S0b — `feat/multi-2d-s0b-engine-seams-20261125-v1`
- **Base (`origin/integration/2d-sim` after S0a):** `2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88`
- **T0 Baseline:** `47b27a8d3873b32726f315ce1182118da38c50b1`
- **Implementer:** Gemini 4 Neon (XHigh) (single writer for S0b)
- **Review Status:** **NOT REVIEWED, NOT INTEGRATED** (must be confirmed by independent non-author reviewer under RG-S0b before merge)

---

## 1. Decision Summary

`MultiAreaContinuation.Advance` is routed through the fixed, compile-time ordered `AnnualPipeline.Steps` array in `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/AnnualPipeline.cs`:

1. `AdultGrowthStep.Instance`
2. `ReservedPhase3DisturbanceStep.Instance` *(RESERVED, EMPTY no-op in S0b)*
3. `DeadwoodStep.Instance` *(permitted relocation M1)*
4. `RegenerationStep.Instance`
5. `YearRecordStep.Instance` *(permitted relocation M2)*
6. `ReservedPhase2BYearEndSummaryStep.Instance` *(RESERVED, EMPTY no-op in S0b)*

`MultiAreaContinuation` remains the sole holder of `sync` and the sole code that assigns `root`. All guards (`RequireFence`, `WorkPendingConsentRequired`, `ContinuationNoLivingStems`), both test-only fault seams (`afterArea`, `beforeCommit`), and the single atomic `root = root with { ... }` commit remain unchanged in `MultiAreaContinuation.Advance`. `AnnualPipeline.cs` contains zero locks, zero `root` assignments, and zero reflection, DI, service locator, or runtime registration mechanisms.

---

## 2. Trigger Evaluation (T1–T6 per Spec §6.3)

| Trigger | Condition (Spec §6.3) | Fired? | Author Evidence |
|---|---|---|---|
| **T1** | Any of the 9 frozen anchors, the v2 golden, or Step 1 `P1-TH-02` skip-vs-manual parity changes or fails | **NO** | Clean builds 0W/0E; `Core.Tests` `282/282 passed, 0 failed` including all 9/9 anchors (#7 `776a68df…`, #9 `93e9abda…`) and `PASS P1-TH-02`; `Phase1.Save.Tests` `50/50 passed, 0 failed` including v2 golden (24,703 B, `c0ea8a46…`, `9629ac16…`). Full `2d-gate.sh` confirms G0–G7 all PASS. |
| **T2** | Any statement relocation beyond **M1** and **M2** is needed | **NO** | Verified by diff inspection and `move-proof.py --member MultiAreaContinuation.Advance --allow M1,M2`: strictly ONLY M1 (deadwood hoist) and M2 (year-record placement) moved. |
| **T3** | Any edit needed in a biology/rules file beyond the D1 `RngSalts` literal substitutions | **NO** | Zero edits to `AnnualRules.cs`, `CrownModel*`, `Stage4*`, `Stage5*`, `Phase2*`, `MultiAreaGenerator.cs`, `ManifestGenerator.cs`, or `DerivedManifest.cs` in the pipeline commit. |
| **T4** | Any existing test assertion, expected refusal code, or expected exception type would have to change | **NO** | Zero existing test assertions, refusal codes, or exception types changed; all 277 pre-existing Core tests + 5 new `RngSaltsTests` + 50 Save tests + all UI/safety suites pass unchanged. |
| **T5** | Independent reviewer rejects the M1/M2 equivalence argument | **OPEN (for RG-S0b)** | Explicit reviewer confirmation slot provided in §5 below; if rejected, S0b reverts `Advance` to byte-identity with `2af3fb1c` (`RETAINED (T5)`). |
| **T6** | Worker cannot show the two reserved steps are no-ops | **NO** | Both `ReservedPhase3DisturbanceStep.Run(YearInProgress year) { }` and `ReservedPhase2BYearEndSummaryStep.Run(YearInProgress year) { }` have empty bodies (zero statements, zero reads/writes, zero allocations), enforced statically by `move-proof.py`. |

---

## 3. M1 & M2 Equivalence Argument

### M1 — Deadwood Hoist (`MultiAreaContinuation.cs` T0 lines 1049–1061 → `DeadwoodStep`)

- **Before (`2af3fb1c`):** Inside the per-area `for (int i = 0; i < root.Areas.Count; i++)` loop, for each area with living stems (`if (area.State.Trees.Any(tree => tree.IsLiving))`), when `stage4 is not null`, fallen deadwood records were appended to `deadwood` in `report.Deaths` order using `stepped.Trees` (`byId[death.TreeId]`), `phase2Start?.SpeciesIdOf(dead.TreeId)`, and post-increment `nextDeadwood++`. Empty areas took the `else` branch (`reports[i] = new MultiAreaAreaReport(area.AreaId, null)`) and emitted no deadwood. Neither `deadwood` nor `nextDeadwood` was read anywhere else in the loop.
- **After (`DeadwoodStep`, Step 3):** Runs immediately after `AdultGrowthStep` (and the empty `ReservedPhase3DisturbanceStep`), strictly **before** `RegenerationStep` (so `progress.NextAreas[i].State` is the exact post-adult `stepped` roster before any promotions are merged). Iterates `i = 0 .. progress.Start.Areas.Count - 1`, skips `report is null` (the exact naturally-empty `else` areas), and appends fallen deadwood records in the exact same area index order and `report.Deaths` order with the exact same `progress.NextDeadwood++` counter and `FallenDeadwoodRecord` arguments.
- **Result:** `progress.Deadwood` and `progress.NextDeadwood` are bit-for-bit identical. The only spec-noted side effect (§6.2) is the test-only `afterArea` invocation count on an unreachable internal invariant failure (`byId[death.TreeId]` missing), where nothing is committed in either version.

### M2 — Year-Record Placement (`MultiAreaContinuation.cs` T0 lines 1081–1082 → `YearRecordStep`)

- **Before (`2af3fb1c`):** `var yearRecord = new MultiAreaYearRecord(year, checked(year + 1), revision, root.WorkRevision, Array.AsReadOnly(records), hasWork);` executed immediately after the adult-step loop and before regeneration.
- **After (`YearRecordStep`, Step 5):** Executes immediately after `RegenerationStep`. Every input (`progress.Year`, `progress.Revision`, `progress.Start.WorkRevision`, `progress.Records`, `progress.HasWork`) is either read-only or written exclusively in `AdultGrowthStep` and never modified by `DeadwoodStep` or `RegenerationStep`.
- **Result:** `progress.YearRecord` is bit-for-bit identical.

---

## 4. Verification & Reproducibility

- Exact move-proof command (`tasks.md` T-B3):
  ```bash
  python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py \
    --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 \
    --head HEAD \
    --member MultiAreaContinuation.Advance \
    --allow M1,M2
  ```
- Full move-proof (`--part all`, invoked automatically by `build/2d-gate.sh` G2): proves all S0a UI splits (`14` blocks, `1,246` non-empty lines), all S0a Core command partials + non-`Advance` root regions (`8` blocks, `1,337` non-empty lines), and the S0b `Advance` extraction invariants simultaneously.
- Raw logs archived externally under `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/`.

---

## 5. Independent Reviewer Confirmation Slot (RG-S0b / Trigger T5)

*(To be completed ONLY by the independent non-author reviewer before merge; author leaves unchecked.)*

- [ ] **M1/M2 equivalence argument accepted (T5 = NOT FIRED):** `DeadwoodStep` (M1) and `YearRecordStep` (M2) preserve exact area/death numbering, `YearRecord` inputs, single-lock/single-commit authority, and zero behaviour change.
- [ ] **Reserved steps verified no-ops (T6 = NOT FIRED):** `ReservedPhase3DisturbanceStep` and `ReservedPhase2BYearEndSummaryStep` perform zero work and touch zero state.
- [ ] **Decision confirmed:** `EXTRACTED` approved for merge to `integration/2d-sim` (or `REJECTED -> RETAINED (T5)` with required revert instructions).

- **Reviewer identity / route:** *(pending)*
- **Reviewed HEAD SHA:** *(pending)*
- **Review date (UTC):** *(pending)*
- **Review handoff reference:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-INDEPENDENT-REVIEW-<YYYYMMDD>.md`
