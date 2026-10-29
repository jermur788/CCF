# S1-D T-04 guide monotonicity — HANDOFF (claim #124)

**Status: IMPLEMENTED + OFFLINE-VERIFIED. NOT QUALIFIED.** Unity qualification and independent review have not been run, so the
success token is withheld.

- Model/provider: Claude Sonnet 5.5 (claude-sonnet-5-5) / Anthropic
- Branch/worktree: `feature/s1-d-t04-guide-monotonicity-v1` @ `/media/jer/Videos/CCF-S1D-T04` (local, not pushed)
- Starting SHA: `b971f4da0540e606bfee9702b0c99f81a0a89355`; ending SHA: see `git log` on the branch (single commit).
- Coordination: #124 was NOT edited on origin/main (that file lives on the coordination branch); the manager should mark it.

## Changed files
- `Assets/ForestPrototype/UI/ScenarioOneTeachingHistory.cs` (+`.meta`) — new pure helper, no UnityEngine.
- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs` — `Facts.HistoricalFloor`; `Current = max(Raw, floor)`; `Raw` is the unchanged earlier rule.
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs` — builds the floor each refresh from `ManagementEvents` + `ObjectiveAchievedYear`.
- `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingHistoryOfflineCheck.cs`, `run_teaching_history_offline.py`.

No save field, schema/validator, completion rule, objective target, Rack, ecology/economy or MFM file touched. Nothing is stored; the
floor is recomputed from persisted events on every refresh. PlayerPrefs and session [J]/[K] state are not read.

## Lesson → persisted-proof latch mapping (floor = earliest lesson index the guide may show)
| Floor | Lesson | Proof (event log in event-id order) |
|---|---|---|
| 5 | 6 `recruitment` | in-footprint succeeded sold AND deadwood felling → later Annual Review ack → later succeeded planting |
| 6 | 7 `establishment` | above + beech and sessile-oak plantings after that review + both `introduced-*` objective records exist (v22 recorded years) |
| 7 | 8 `transformation` | above + an ack, after both needed plantings, in a year later than every planting resolved before that ack |

Lessons 1–5 (`access` … `response`) are **uncovered: floor 0, existing behaviour**.

## Uncovered / fallback states and judgement calls (manager attention)
1. **Lessons 1–5 have no faithful persisted proof.** Crop Tree choice, 30% share and the footprint "serves a Crop Tree" relation are not
   persisted; lesson 5 completion is session-only [J]/[K] by design. Crop Tree add/loss and share drift can still move the guide among
   lessons 1–5, unchanged from before.
2. **Lesson 5→6 judgement:** a successful planting after the release review is used as the persisted witness that lesson 5 was passed.
   Consequence: a player who plants without doing [J]/[K] is moved to lesson 6 (before: held at 5). This is also what makes a save at
   lessons 6–8 survive reload. If the manager rejects this, the 6–8 reload requirement cannot be met — this is the packet's
   "no persisted fact" stop condition for that boundary.
3. The release proof counts **any** in-footprint sold+deadwood felling, not only felling serving a Crop Tree (not recoverable). A player
   who never marked Crop Trees but felled in-footprint, read a review and planted would be promoted. Legacy (no rack area) worlds treat all felling as in-footprint, matching their existing rules.
4. Objective records written before v22 saves are "not recorded": floor stays 5 there (no over-claim).
5. **T-03 not unified:** `ScenarioOneTeachingCap.SampleHistoricallyComplete` still reads the current bay. Not changed (T-03 gates stay green);
   it can adopt `ScenarioOneTeachingHistory.ReleaseReviewEventId >= 0` in a follow-up. The packet's "one interpretation" requirement is therefore only partly met.

## Evidence
Offline (plain .NET): `run_teaching_history_offline.py --mutations` → `TEACHING_HISTORY_OFFLINE_PASS checks=27`, 6/6 mutations caught.
Covers boundaries, Crop-Tree/share/felling drift, prefix monotonicity, fourth-cycle symmetry, unfaithful evidence fallbacks,
stable IDs (all eight, in order), 256-row grid proving floor 0 ⇒ `Current == Raw`.
Regression: `run_teaching_cap_offline.py --base b971f4d` → `TEACHING_CAP_OFFLINE_PASS checks=48`; its protected-diff gate reports the
teaching sequence diff, which is expected (that file is this task's owned surface). `run_offline_check.py` (T-02) → `CYCLE_STATE_OFFLINE_PASS checks=93`.

**Not run:** Unity compile/import, save-at-lessons-6–8 reload witnesses, world-drift trigger, TeachingCopy/MenuTutorial Unity checks,
N3 rule-hash check, S1-C history/query checks, independent review. N3: `ScenarioOneCompletionRules.cs` and objective files are untouched (git diff vs base empty).

## Next
Unity gate under the machine lock, independent OpenAI review, manager decision on item 2/3, then MFM-09 may proceed.
