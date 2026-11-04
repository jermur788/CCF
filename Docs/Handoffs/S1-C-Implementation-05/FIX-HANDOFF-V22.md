# S1-C T-05 — v22 blocking-fix handoff (review of `7ae0bbc`)

Static/offline only. No Unity, no Unity lock, no Windows build, no version-pin edit, no anchor rebaseline. Nothing here is Unity-verified or integrated. The exact fix commit SHA is in the worker report (a commit cannot name itself). Base `aca74ac`, previous production candidate `7ae0bbc`, docs tip `410b6fe`.

## Finding 1 — false later first-achievement after an evidence gap

Root cause: `RestoreObjectiveHistory` set `evidenceGap` only to suppress derived years. An objective achieved only inside a missing-snapshot year was never added to `achievedBeforeRecording`, so a later re-achievement was recorded as a false first. The replay also walked only surviving `YearAdvanced` events, so a missing event never produced a gap.

Fix (`ScenarioOneManager.ObjectiveHistory.cs`): the replay now walks the union of `YearAdvanced` event years and snapshot years > 0 (year 0 is the never-evaluated baseline). A year missing either half is an evidence gap. A gap sets session state `objectiveFirstnessUnknown`, and `RecordObjectiveAchievements` then writes nothing further, so unproven objectives stay "not recorded" (rebuilt identically on every reload, so it also holds after a v22 re-save). Evidence-complete first achievements before the earliest gap still derive; complete games record as before. Existing records are untouched.

Consequence to note: after a gap, no new first-achievement records are written for that world (conservative; firstness cannot be proven for any unrecorded objective).

## Finding 2 — parsed v21 default-rack hash identity

Root cause: `HashLayout` keyed the header rewrite on `ForestSaveData.CurrentVersion` (22), so a parsed v21 input kept `"version":21` instead of being relabelled to 20.

Fix (`ScenarioReferenceArchive.cs`): the header is the input's own `data.version`. Default-rack: rewritten to 20 when the input is >= 21. Rack-state: rewritten to 21 when the input is >= 22 (a v21 input stays 21). Older inputs are unchanged, as at the base. Achievement strip unchanged and always on.

## New offline coverage (`Tools/Verification/S1CObjectiveHistory/`)

- F-1a: missing snapshot + achieved only in the gap + lost + re-met in 14 -> no record, "not recorded", also after v22 re-save/reload.
- F-1b: missing `YearAdvanced` event year (snapshot kept) -> same, plus re-save/reload.
- F-1c: first achievement proven before the earliest gap still derives (year 3); complete new game still records and reloads.
- Hash: `HashFixtures` gains four `parsed-v21-*` worlds (default-rack legacy40/enlarged80, rack-state, rack-order-only) built from JSON with version 21. The same objects go through the aca74ac and candidate hash paths; WorldHash, CurrentWorldHash, LegacyCompatible and V15-V18 lines match exactly. A further check shows a v21 input and the same world as v22 hash identically.
- Both new groups were confirmed to FAIL against the unfixed production code, then pass with the fix.

## Result

`python3 -I -B Tools/Verification/S1CObjectiveHistory/run_offline_check.py` -> exit 0, `S1C_T05_OFFLINE_PASS checks=137` (115 existing + 22 new), `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_aca74ac`.

## Outstanding

Unity verification, the v22 pin relabel (`ObjectiveHistoryOfflineCheck.cs:67` stays v21, `RackV2SaveVerification.cs:221` stays v20), integration, and re-review by OpenAI 6.1 Sol.
