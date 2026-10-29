# S1-D T-06 — Manager Disposition

**Task:** S1-D T-06 — management-event ID integrity: reject ambiguous IDs before save-load mutation  
**Implementation branch:** `feature/s1-d-management-event-id-integrity-v1`  
**Implementation SHA:** `8e05b5a24d7c3c74d063feb28002a6d7f28db438`  
**Published implementation tip:** `55787c8a3e99e415c89366fd98c9aa31f4c7096c`  
**Implementer:** Claude Sonnet 5.5 / Anthropic  

## Verdict

**STATIC IMPLEMENTATION ACCEPTED FOR INDEPENDENT REVIEW — DO NOT INTEGRATE; UNITY SAVE-HARDENING QUALIFICATION REQUIRED.**

This is not final acceptance. The production rule has not been compiled or exercised through the real Unity save/load path, and the mandatory independent review has not yet passed.

## What the Manager inspected

- The branch is two commits ahead of its recorded starting base `e6ee19990fa7a16b7c69c2ecfa6393b45cc4ee67` with no behind commits at handoff.
- The only production change is `Assets/ForestPrototype/ForestSaveValidation.cs` (+21 lines).
- The rule is added once at the shared `ForestSaveValidation.Validate` boundary used by both `CanLoad` and `LoadData` through `FindLoadProblem`.
- It rejects `eventId <= 0` and duplicate positive IDs, performs no repair/renumber/reorder, adds no persisted field and does not bump the save version.
- The call is last in the validator, preserving earlier validation-reason precedence.
- The published handoff records a pre-edit fixture audit with no ambiguous shipped/accepted save among Reference Future v1 milestones, the tracked storm saves and the two serialized scenes.
- The handoff records offline 75/75 PASS, 7/7 mutation controls caught, N3 88 PASS and S1-D T-02 93 PASS. These are worker evidence, not a Unity qualification performed by the Manager.

## U4 disposition — Manager decision

`Tools/Verification/U4/U4Qualification.cs` case **"SYN duplicate event IDs" is NOT the T-06 STOP condition.**

Reason: the approved S1-D stop condition applies when a **shipped or accepted save fixture** has duplicate/non-positive management-event IDs. The U4 case is an intentionally edited synthetic negative fixture constructed inside the harness. It is not compatibility evidence for a real save. RD-12/FR-E3 now makes that synthetic save invalid at the load boundary.

Therefore the predicted U4 failure is a **stale verification contract**, not a production compatibility blocker.

### Required verification-contract repair

Before U4 is used as a gate on the T-06 lineage:

1. Replace the synthetic duplicate-ID case's expectation that `LoadData` accepts the edited save with an expectation that the duplicate-ID save is **refused before mutation**.
2. Preserve the substance of the old chronology protection. Do **not** delete or weaken the duplicate-ID completion-rule coverage already provided by the protected N3/rules-level checks.
3. Do not bypass the new validator merely to force an invalid save into live state.
4. Do not relax, version-gate or otherwise weaken the production T-06 rule to keep the old U4 expectation green.
5. The U4 repair is verification-only and may be made by the independent reviewer under the bounded review packet. Any production change returns to the Manager as `NEEDS CHANGES`.

## Remaining gates

- Mandatory independent review by a different provider/model.
- Verification-only U4 contract repair above.
- Real Unity import/compile of the production and Editor harness.
- T-06 runtime `CanLoad` / `LoadData` / `Load()` parity and no-world/no-disk-mutation gate.
- Existing `SAVE_HARDENING_VERIFY_PASS` preservation gate.
- Existing save-cell-index hardening preservation gate.
- No Windows build.

The machine-wide Unity queue remains unchanged: RACK-V2-08 claim #49 has priority unless the Overall Manager explicitly reroutes it.

## State

- IMPLEMENTED: **YES — static**
- OFFLINE VERIFIED: **YES — worker evidence**
- MANAGER STATIC REVIEW: **PASS FOR INDEPENDENT REVIEW**
- INDEPENDENTLY REVIEWED: **NO**
- UNITY VERIFIED: **NO**
- INTEGRATED: **NO**
- ACCEPTED: **NO**
