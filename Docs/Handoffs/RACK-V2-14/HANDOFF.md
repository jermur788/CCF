# RACK-V2-14 — integration and spec conformance

**Task:** RACK-V2-14, claim #99, umbrella #91.
**Exact integrated candidate:** `60fd9025e0205e7e8b24203295dd606ffa4a8061`
`integration/scenario-one-player-test-ready-v1`
**Integrator:** DeepSeek #91 orchestration/integration operator.
**Exit:** `INTEGRATED CANDIDATE READY`

## Task conformance

| Task | State | Evidence |
|---|---|---|
| RACK-V2-01 inspect | COMPLETE (earlier lineage) | existing architecture inventory/handoff |
| RACK-V2-02 geometry | COMPLETE | FIVE-RACK CALIBRATION VERIFIED |
| RACK-V2-03 authoritative model | COMPLETE | single source of truth; v21 |
| RACK-V2-04 map planning mode | COMPLETE | PR #? / existing Rack V2 lineage |
| RACK-V2-05 editing | COMPLETE | existing Rack V2 editing qualification |
| RACK-V2-06 spacing teaching | COMPLETE | existing spacing qualification |
| RACK-V2-07 confirmation + minimap | COMPLETE | integrated/qualified earlier |
| RACK-V2-08 no competing author | COMPLETE | PR #89 / d925e95 |
| RACK-V2-09 field marking | COMPLETE / REVIEWED / INTEGRATED | candidate `8f8316b`; field 99/10, runtime 14, save 35; review `04913c0` PASS; merge `6d3228c` |
| RACK-V2-10 Help/teaching | COMPLETE / STATIC + RENDERED | static 97/15; rendered captures c06 at both sizes; `6c07c21` |
| RACK-V2-11 persistence | COMPLETE / PASS | `a6ad231`: v2 30, coldstart 2, legacy40 7, save03b 35 |
| RACK-V2-12 functional | COMPLETE / PASS | `a900b7f`: `RACKV2_O1E2E_PASS checks=173 shots=24`; `RV2L_MODEL0_PASS`; `RV2L_LEGACY40_PASS` |
| RACK-V2-13 rendered | COMPLETE / VISUAL PASS | `b77d898`; 720p/1080p review with non-blocking polish notes |
| RACK-V2-14 integration | THIS HANDOFF | merged at `60fd902` |

## Integrated content

- v21 authoritative rack objects, five-rack Enlarged80 plan, map planning/edit/confirm, headland targeting.
- Orange entrance marking before approval; exclusive Fell/Crop/orange semantics; OPEN-3 reopen/edit/executed-lock rules.
- Derived `Rack entrances marked X / 5`; no second mark ledger or counter.
- Persisted disk save/Continue, cold start, Legacy40/model-0 isolation, malformed-V2 refusal, Reference Future v1 untouched.
- End-to-end Objective 1 → approval → execution → `rack.result` → Objective 2, with a real C Crop Tree toggle and save/load.
- Help/teaching: Tree Marks orange/red/blue, orange convention, first-tree simplification, Objective 1 copy.

## Protected anchors / compatibility

- RACK-V2-09/11 gates: Legacy40 never enters V2; model-0 progression passes; previous v20/v21 saves load; malformed V2 refused before mutation.
- Reference Future v1 archive/schedule unchanged; Year-100 `7AD177B3CC2F73C7` verified in R11.
- No save schema change: `CurrentVersion` remains 21; no new fields.
- No scene/prefab/material/ProjectSettings/package change from the Rack work.
- Non-blocking visual notes from R13: prompt/stripe overlap; duplicated marked count; narrow 24° orange patches; disabled Approve button styling; 720p modal Help coverage. None is a functional failure.

## Residuals / next

- G4 independent final review and Manager-owned spec status promotion remain separate; this handoff is a technical integration readiness statement, not user acceptance.
- User playtest is not claimed.
- No Windows build.
- The Rack exit is met: persisted confirmed rack geometry is available to MFM. Remaining programme work (parity, S1-C, S1-D, MFM V1, final Tier-3) continues.
