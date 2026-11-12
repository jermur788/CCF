# Requirements and acceptance — Cadence V1

**APPROVED DESIGN / ALL IMPLEMENTATION AND UNITY CHECKS UNRUN.** The accepted user product direction and independent #396 review are design inputs, not code verification. Source fixes and tests are mandatory before `VERIFIED`.

## Preflight and exact spec contract
- [ ] G1 Latest candidate source, branch and worktree/owner/Unity lock rechecked; no source writer overlap or personal #382 modification.
- [ ] G2 Two modes visible: unchanged `Advance 1 Year` and `Skip to Year Y (N years)`; read-only reason and confirmation.
- [ ] G3 Gap=4 years, max=5 ticks, Year25 and Year100 bounds, last successful matching `FellTree` year including access cutting as **navigation-only** anchor; no imported forestry rule or compulsory treatment.
- [ ] G4 Only eligible after current stage 7 completion, open Work Plan, empty Pending/Approved, newest review acknowledged, no rack/model/preview/modal/teaching blockers. Other stages fail closed, including Legacy40 unknown stage.
- [ ] G5 Keep `HasReviewedTwoCycleSequence` / S1-D latch / `ScenarioCycleState` unchanged; separate required D-074 spaced major-intervention learning objective remains a distinct unapproved design.

## Behaviour and model identity
- [ ] AC-1 Loop uses existing `ScenarioOneManager.AdvanceYear()` exactly once **per frame** per year, no direct year increment or second model.
- [ ] AC-2 Skip **never writes `AnnualReviewAcknowledged`** for intermediate years, never calls `MarkAnnualReviewSeen`; all individual reports, events and snapshots still recorded.
- [ ] AC-3 At stop, latest Review opens, shows exact elapsed year range/count/why, includes existing year-history browse and one real destination acknowledgement by player.
- [ ] AC-4 Pre/post stop at newly Completed/Failed, newly achieved objective, new storm/victims, Year25, Year100, lesson shift, guard or actual failure; cap reached; cancellation between ticks; no partial-year cancellation or fabricated rollback.
- [ ] AC-5 Pending/Approved work (including MFM orders) blocks, no autoprescription, auto-mark, rack selection, Crop Tree confirmation, work approval or execution; game treats management year as an option to reassess, not automatic felling.
- [ ] AC-6 Existing 1-year route unchanged, including ordinary Annual Review, Work Plan guard and player choice for earlier planting/protection/maintenance.
- [ ] AC-7 Prevent double-click, UI/save/load concurrency and cancellation during a tick; allow interrupt between ticks; on error show k actual years done and manager reason.
- [ ] AC-8 Existing annual object/financial/ecology/RNG and objective evaluation are not modified; exact world-save identity or compatibility of frozen historical worlds not weakened.

## Two distinctly different parity proofs
- [ ] P-A: For N 1…5, one identical authentic initial save, compare repeated **direct manager** annual calls with **no acknowledgment** to skip **after every tick**, requiring identical JSON, `CurrentWorldHash`, annual reports, snapshots, events, objective achievements, trees, mortality/juveniles/cash/outcome.
- [ ] P-B: Compare manual **UI** stepping with **real annual acknowledgments** versus skip and normalize *only* acknowledged events and their ID renumbering. All other yearly facts and eventual objective/outcome values equal; **do not assert full-world hash equality** for this comparison.
- [ ] P-C: Real completed two-cycle fixture, destination acknowledgment proof, within-window completion/year25/Year100, seeded storm stop and injected k-step failure; no artifact generated from fabricated history.
- [ ] P-D: Negative tests for every teach/Work Plan/preview/modal/failed condition, MFM approved prescription without pending proposal state and all history/mark nonmutation.
- [ ] P-E: Actual 1280×720 and 1920×1080 UI, 5-tick progress/performance, save/load at safe boundary, no midtick save, Legacy40/frozen Reference Future v1, current candidate exact source and authorized Unity lane.

## Exit
- [ ] E1 Implementation claim handoff published and [HANDED-OFF]/closed after source checks; independent review where necessary.
- [ ] E2 Manager APPROVED / NEEDS CHANGES / REJECTED after source/Unity evidence, accepted integration in exact UI lineage; milestone Tier-3 only when needed.
- [ ] E3 D-074 broader spaced-major-management **objective redesign separately tracked**; current cadence V1 is not declared full objective completion.
- [ ] E4 Windows/Player build withheld until personal Scenario One playtest and explicit permission; user #382 saves/Prefs untouched.

**No box above may be checked from the original reviewer or static source alone.**
