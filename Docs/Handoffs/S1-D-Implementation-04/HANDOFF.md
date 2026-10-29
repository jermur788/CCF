# S1-D T-04 guide monotonicity — HANDOFF (claim #124), revision 2

**Status: REVISED per MANAGER-REVIEW.md; OFFLINE-VERIFIED; Unity NOT yet run; independent review NOT yet run.** Success token withheld.

- Model/provider: Claude Sonnet 5.5 (claude-sonnet-5-5) / Anthropic
- Branch/worktree: `feature/s1-d-t04-guide-monotonicity-v1` @ `/media/jer/Videos/CCF-S1D-T04`
- Base: `b971f4da0540e606bfee9702b0c99f81a0a89355`. Superseded (do not review): `18e33d4`. Exact corrected candidate: the branch head
  (`git rev-parse` on the pushed branch); this file is committed in it.
- Unity lock: not used. Coordination file not edited (manager to mark #124).

## Manager findings → resolution
1. **Planting as proof of Lesson 5 / [J]/[K]: removed.** No planting-based proof of any boundary before Lesson 6's own completion remains.
2. **Arbitrary in-footprint felling as release proof: removed.** The helper no longer has felling kinds or a footprint input.
3. **T-03/T-04 one interpretation: done.** `ScenarioOneTeachingHistory.SampleHistoricallyComplete` is the single definition; `ScenarioOneTeachingCap.SampleHistoricallyComplete`
   now delegates to it (formula unchanged, so T-03 behaviour is identical). The guide's rules at Lesson 6+ need exactly those release facts, and the
   guide never latches anything before Lesson 6, so the two cannot disagree. No protected API crossed; no blocker.

## Final design
- `ScenarioOneTeachingHistory.Floor` (pure, no UnityEngine, nothing stored) latches only:
  - **Lesson 7 (`establishment`)**: beech and sessile-oak plantings succeeded AND both `introduced-*` objective records exist (v22 recorded years).
  - **Lesson 8 (`transformation`)**: additionally an Annual Review acknowledged after both plantings, in a year later than every planting resolved before that review (replay by event id, so a later planting cannot undo it).
- `ScenarioOneTeachingSequence.Current = Raw` (the unchanged earlier rule) unless `Raw >= 5` (Lesson 6 currently reached, i.e. every earlier lesson currently holds), then `max(Raw, floor)`.
- Wiring: `LearningObjectivesView.Sequence.cs` builds entries from `ManagementEvents` (acknowledgements and succeeded plantings only) + `ObjectiveAchievedYear`.

## Uncovered / fallback states
- **Lessons 1–5: not latched (existing current-state behaviour).** Crop Tree add/loss, Crop-Tree-serving release, share drift, later in-footprint felling and the session-only [J]/[K] review have no faithful persisted proof. These can still move the guide within 1–5, and a reload still returns to Lesson 5 until the inspection is repeated (documented design).
- Consequence: **the floor does not protect a guide that has already fallen below Lesson 6** (e.g. Crop Tree loss while at Lesson 7 drops Raw below 5, so the guide follows the existing rules). It protects Lessons 7/8 only against drift *within* 6–8 (extra planting year, lost seedling, goal flicker). This is narrower than AC-3's full wording; Manager should confirm it is acceptable or supply a persisted Crop-Tree-release record (would need a save field — out of scope).
- Pre-v22 saves with no recorded objective years: no latch.
- The "Lesson 6–8 save at reload" Unity witness will therefore show: reload at Lesson 7/8 returns to Lesson 5 until the inspection review is repeated (Raw < 5), then the floor holds 7/8. Expect this; do not read it as a regression.

## Changed files
`UI/ScenarioOneTeachingHistory.cs` (+.meta, new), `UI/ScenarioOneTeachingSequence.cs`, `UI/LearningObjectivesView.Sequence.cs`, `UI/ScenarioOneTeachingCap.cs` (delegation only),
`Tools/Verification/ScenarioOneRepeatedInterventions/{TeachingHistoryOfflineCheck.cs, run_teaching_history_offline.py, run_teaching_cap_offline.py}` (cap runner: compiles the shared helper, routes the two completion mutations to it, and drops the teaching sequence from its protected list because T-04 owns it).
No save/schema/validator, completion rules, objectives, Rack, ecology/economy, MFM, scene, package or ProjectSettings change (git diff vs base empty for those).

## Offline evidence (plain .NET, revised candidate)
- `run_teaching_history_offline.py --mutations`: `TEACHING_HISTORY_OFFLINE_PASS checks=29`; 5/5 mutations caught. Covers boundaries, prefix monotonicity, fourth-cycle symmetry, later planting, unrecorded/oak-missing/same-year/before-plantings fallbacks, planting alone never latches, eight stable IDs in order, exhaustive grid (floor never alters Raw<5, never lowers Raw>=5), cap/helper equivalence, source wiring (no PlayerPrefs/Unity/[J]/[K]).
- `run_teaching_cap_offline.py --base b971f4d --mutations`: `TEACHING_CAP_OFFLINE_PASS checks=48`, protected-diff PASS, 8/8 mutations caught.
- `run_offline_check.py` (T-02): `CYCLE_STATE_OFFLINE_PASS checks=93`. EventIdIntegrity: `EVENT_ID_OFFLINE_PASS checks=75`.
- `RackV2HelpTeaching/run_offline.py`: wording checks pass; S10/S11 scope guards fail **identically on the untouched base** (they compare against old SHA 6d3228c) — pre-existing, unrelated.
- N3: `ScenarioOneCompletionRules.cs` and objective files untouched (empty diff vs base). TeachingCopy/MenuTutorial/S1-C Unity checks: not yet run.

## Next
Manager code approval of this candidate → Unity (lock refresh, compile, drift trigger, save/reload at 6–8 with the expectation above, TeachingCopy/MenuTutorial, N3 hash, S1-C) → independent OpenAI review → then MFM-09.
