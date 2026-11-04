# PARITY-07 — Review-fix handoff (wording only)

**Static-qualified only; NOT UNITY-VERIFIED; no rendered review done.** Responds to the independent OpenAI 6.1 Sol review (NEEDS CHANGES, two blocking wording issues). Executor: Anthropic `claude-sonnet-5-5`. Prior code candidate `1e7b941`, docs tip `c7e88c1`, base `23fe4b8`. The fix commit SHA is reported in the return message (a commit cannot contain its own SHA).

## Corrections
1. **Shared-rules note** (`UI/ScenarioOneUiFacts.cs` `SharedSpeciesRulesNote`): now "Site suitability, canopy shading and wind exposure use the same simplified rules for all three species. Site and wind responses are not calibrated per species; canopy shading still reflects species-specific crown size and crown adjustment."
2. **Pruning teaching in legacy saves**:
   - `LearningObjectivesView.Curriculum.cs` `prune.read`: "When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above." (Production gate: `speciesAware`, passed as `BroadleafMarketActive` from `ScenarioOneManager`.)
   - Oak grade-benefit statements in `LearningObjectivesView.Curriculum.cs` (Pruning topic) and `WorkPlanView.cs` pruning card now start "Where Oak timber is marketed, …".

## Checker change
`check_player_facing_truth.py`: one existing check string updated to match the qualified clause ("use two lifts, …"); 6 checks added (broadleaf clause qualified by active markets; production gate on `BroadleafMarketActive`; shared note keeps crown differences; shared note denies only site/wind calibration; Oak grade qualified in teaching and in Work Plan). No FR-9 check weakened or removed.

## Raw results
- `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_player_facing_truth.py --repo .` → `PARITY07_COPY PASS checks=83 passed=83 failed=0 NOT_UNITY_VERIFIED` (exit 0)
- `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite timber` → `THREE_SPECIES_TIMBER_PASS checks=73` (exit 0)

## Outstanding
Unity import/compile and rendered 1280×720 / 1920×1080 review (longer note and prune.read lines may clip) are NOT done.
