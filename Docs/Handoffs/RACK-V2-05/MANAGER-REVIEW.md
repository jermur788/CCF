# RACK-V2-05 — MANAGER REVIEW

**Task:** `RACK-V2-05 — Direct map rack editing`  
**Task claim:** #43 (`[HANDED-OFF]`, closed)  
**Worker handoff:** `Docs/Handoffs/RACK-V2-05/HANDOFF.md`  
**Exact Unity-tested candidate:** `cc643082b14f0b8bf96a5472e3505608c3025c26`  
**Published branch tip:** `da54734c9a05d43ec13f203188ffe607cc617db6`  
**Integration:** PR #44 / merge `0ec6bc4d473975e9938040026737e70a33432c48`  
**Manager verdict:** **APPROVED / INTEGRATED — RACK EDITING READY**

## Accepted evidence

- Real-pointer Unity editing qualification passed: `RACK_V2_EDITING_UNITY_PASS checks=326 shots=17` at 1280×720 and 1920×1080.
- Offline editing rules passed 61 checks.
- Shared Stand Map checks passed 165; suggested-layout checks passed 147.
- Runtime type compile passed with 172 sources and zero errors.
- Unchanged RACK-V2-04 map gate passed 142 checks with eight captures.
- Unchanged RACK-V2-03C runtime gate passed 14 checks.
- Gate sources were byte-identical to BASE; no expected values or protected gates were rebaselined.
- Machine-wide Unity lock was acquired/released normally; U1 ended clean.

## Accepted implementation boundary

RACK-V2-05 now provides direct map create/select/move/rotate/delete, one-step undo, reset-to-suggested with confirmation when edits would be lost, advisory snapping, entrance re-snap, invalid-geometry refusal and executed-rack locking. It continues to use the authoritative Rack V2 plan/runtime rather than creating a parallel rack state.

The three-line `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` change is accepted as in-scope because it is the narrow input-delivery wiring required for the packet-mandated Delete/Backspace interaction; it is not an unrelated UI redesign.

The `ScenarioOneManager.RackPlan.cs` change is accepted. Preventing `UpdateRack` from creating orders for a draft rack that has never held work restores the already-authoritative D-064 contract that rack-removal Work Plan orders are created/refreshed on Confirm, not during draft geometry editing. The optional ignored-rack parameter is a bounded dry-run support path with existing callers unchanged.

## OPEN-3 conformance

The accepted OPEN-3 rule is preserved:

- racks with zero executed removals remain editable through the authoritative reconciliation/cancellation path;
- any rack with executed removals is geometry-locked and represented as harvested;
- executed removals are never reversed;
- only changed editable racks lose geometry-bound entrance-mark state.

The Stand Map currently does not supply explicit consent to cancel approved-but-unexecuted work. This is acceptable for RACK-V2-05 because confirmed planning is not reopened by this task; the user-facing reopen/consent flow belongs to RACK-V2-07.

## Measurement-integrity note

Cross-run fresh-world full-save SHA values are not accepted as a stable comparator for this gate: the unchanged BASE gate and repeated identical-candidate runs produced different full-save hashes. This does **not** establish a branch regression. Acceptance instead relies on within-run save nonmutation where asserted, unchanged world hash/counters, protected Rack gates, and the BASE reproduction. Do not treat the variable cross-run full-save SHA as a new canonical anchor.

## Calibration status

The RACK-V2-05 editing-feel constants (including minimum length, duplicate tolerance and snap thresholds) are accepted as **GAMEPLAY CALIBRATION**, not forestry facts. RACK-V2-06 may retune spacing/teaching-facing behaviour if its own verification justifies it; material changes to editing semantics return to Manager review.

## Limitations / not claimed

- No Windows/Player build or user acceptance.
- RACK-V2-06 spacing teaching is not implemented or accepted.
- RACK-V2-07 confirmation/reopen/minimap flow is not implemented or accepted.
- RACK-V2-08+ first-person-author removal, field marking, Help, persistence and whole Objective-1 qualification remain pending.
- The small refused-ghost presentation and one-resolution harvested capture are acceptable for this functional slice and remain candidates for RACK-V2-13 rendered polish.

**Verdict:** `APPROVED / INTEGRATED — RACK EDITING READY`

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
