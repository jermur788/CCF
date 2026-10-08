# Scenario One — Definition of Done (Workstream X)

**Status:** proposed readiness checklist. Not accepted. Each item is concrete and checkable. "Gate" names an existing or proposed harness; "Human" means a person must do it. Packet IDs refer to `ImplementationRoadmap.md`.

Current position at `a8596df`: **functionally complete and presentation-accepted** (D-043, D-045). The model stack is new (D-046/047/048). It is **not yet a learnable management scenario** by the criteria below.

---

## Tier 1 — PLAYABLE INTERNAL ALPHA

*An internal player can play a new game to completion and back without blocking defects, and the scenario does not lie.*

| Area | Requirement | How checked | Status at `a8596df` |
|---|---|---|---|
| Simulation | New-game stack RNG 1 / regeneration 1 / growth 1 runs 100 years without exceptions | Completion + lifecycle gates | **Met** (D-048 verification) |
| Simulation | Completion reachable on the reference plan | `ScenarioOneCompletionVerification` (`D7C4DDD36B53FCCE`, Year 25) | **Met** |
| Economy | Every cost/revenue line in the Work Plan equals settlement ±0 | Economy integration gate (120) | **Met** |
| Economy | The cash dead end (S1) is warned at the moment it happens | New UI check | **Not met** (P3) |
| UI | All five screens open, close and do not overlap at 1280 × 720 / 1600 × 900 / 1920 × 1080 | MenuTutorial + Clearance interactive gates | **Met** (P1 branch evidence; main earlier) |
| Tutorial | CCF is defined; positive selection is framed; clearance copy is honest for the active regeneration model | `ScenarioOneTeachingCopyVerification` | **Not met on main** (P1 candidate) |
| Saves | v1–v17 saves load; save/load mid-scenario is deterministic | Save hardening, RNG policy, continuation gates | **Met** |
| Determinism | Lifecycle and completion hashes reproduce in two processes | Existing gates | **Met** |
| Reference | Reference Future v1 integrity and continuation | Reference gate | **Met** |
| Performance | Annual advance on the Scenario One stand < 1 s in the editor on the GTX 980M machine; walking median ≤ 33 ms | Existing evidence (≈ 20–30 ms walking) | **Met** (informal) |
| Harnesses | The three red gates run in their documented modes | `Docs/Verification/ScenarioOneGateModes.md` | **Not on main** (P1 candidate carries the fixes) |

**Alpha = integrate P1 + the S1 warning.**

---

## Tier 2 — BEGINNER PLAYTEST READY

*A person with no forestry knowledge can be observed learning, and failures will be about understanding, not about broken flow.*

| Area | Requirement | Check |
|---|---|---|
| Tutorial | Tree Inspection shows a Crop Tree's competitors with shares (P2) | Gate: list equals a `HegyiTerm` recomputation for fixture P0707 |
| Tutorial | The marking forecast shows the Crop Tree figure separate from the stand average, **in a readable panel** (V1) | Rendered gate at 1280 × 720 |
| Tutorial | Work Plan "What you are leaving" (P3) | Gate: T2/T5 fixture produce the documented facts; no score fields |
| Economy | The Work Plan explains why the thinning loses money (scale/minimum, scenario rule) | String gate |
| Economy | S1 warning on approval or purchase | Gate: buy stock to below €2,500 before thinning → warning text |
| UI | No information-bearing text < 13 px base (V2) | `.uss` static check |
| UI | No raw species IDs in rendered text (TerminologyAudit §3) | String sweep gate |
| UI | Objectives/progress visible as a list from the HUD (V3) | Interactive gate |
| Assets | Packet 01 (juvenile vs woody understorey) and packet 07 (mark contrast) pass rendered review | Rendered review + recognition check in the pilot session |
| Assets | Beech display-height fix on main (from M2, or cherry-picked) | Display-height gate |
| Human | One **pilot** session (internal, non-forester) through the protocol without facilitator intervention beyond controls | `BeginnerPlaytestProtocol.md` |
| Testing | A playtest build SHA recorded; PlayerPrefs reset script; save backup procedure | Protocol §1 |
| Accessibility | Tested at 1366 × 768 as well | Rendered gate |

---

## Tier 3 — SCENARIO ONE FEATURE COMPLETE

*Scenario One teaches repeated, selective, reasoned CCF management; the forest and the player's history are legible; completion means demonstrated management; the chosen ecological scope is integrated and verified.*

| Area | Requirement | Check |
|---|---|---|
| Simulation | Regeneration Model 2 integrated with accepted parameters (or explicitly deferred by decision) | Sol's M2 gates on main; decision recorded |
| Simulation | Storms v1 integrated with calibrated defaults (or explicitly deferred by decision); first-storm year known and accepted (`StormTeaching.md` §2) | Storm calibration matrix; `storm_timeline.py` on the final roll |
| Simulation | Completion viability ≥ 90 % across the calibration matrix under storms | Storm W2 calibration |
| Economy | Decision recorded on the cash dead end (warning only / standing sale / net settlement); sapling-price inconsistency resolved or documented | Decision Log entries |
| UI | Annual Review v2 with PLACES TO INSPECT and Set waypoint; Forest Diary; map cell history (P4) | Gates: derivation equals saved data; identical across save/load |
| UI | Storm stability bands replace "Wind exposure" (or the field is removed) | String gate |
| Tutorial | Stage progression S1–S12 (P5), per-forest progress if the save decision is taken | Interactive gate: no stage completes without its evidence; inaction completes none |
| Tutorial | Model-dependent clearance copy (M1 vs M2) | String gate per model |
| Tutorial | Storm teaching moments (StormTeaching §3) | Interactive gate with a forced storm in a harness |
| Completion | Completion model decided (recommended D) and implemented; new completion anchor recorded with authorisation; negative controls fail as designed | Completion gate + negative controls |
| Completion | Second-look trigger fires in Years 6–20 for T0–T5-type plans | Calibration harness |
| Failure | Hard-fail decision implemented (recommended: none; the Year-100 outcome reworded) | Gate |
| Assets | Packets 01, 02, 03, 04, 07 accepted; storm minimum visual package accepted (if storms are in scope) | Rendered review |
| Performance | 100-year run with storms and M2 within +10 % of the current annual-step time; walking frame time unchanged at the reference view | Performance harness (Sol pattern) |
| Saves | v1 → latest load; per-forest progress restores; old `outcome = Failed` saves display correctly | Save gates |
| Determinism | All anchors (legacy, M1, M2, storm) reproduced in two processes; Reference Future v1 unchanged | Regression suite |
| Accessibility | ReadabilityAudit BLOCKER and IMPORTANT items closed or explicitly waived | Rendered review |
| Human | Beginner playtest 1 (5–6 people) meets ≥ 8 of 11 concept criteria; findings fixed (P8); playtest 2 confirms | Protocol |
| Human | Forester review: no unresolved **MISLEADING** item | Protocol |
| Docs | Overview, Milestone and Decision Log updated; no stale tutorial text (`TutorialHint` dead branch removed) | Review |

**Not required for feature complete** (deferred, D-044 or later): fencing gameplay, browse-history persistence, stem form/quality, pruning premium, broadleaf market, the training mode (Q2), sample plots, player-declared goals.
