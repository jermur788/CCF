# S1-C T-05 — Independent review (objective/progression history, save v22)

**Final verdict (round 2, candidate `7ae0bbc`): S1-C T-05 REVIEW PASS — APPROVED FOR V22 INTEGRATION LINEAGE**

Round 1 (`cd7b518`) verdict was NEEDS CHANGES; it is kept below unchanged for the record, followed by the round-2 re-review.

Read-only static/offline review. No Unity Editor, no Unity lock, no Windows build. No production, test, spec or coordination file was edited. Claim #104 stays open.

## Identity

- Reviewer: CLAUDE A, **Claude Opus 5.5** (Anthropic, `claude-opus-5-5`), via T3 Code / Claude Code. Did not implement T-05 (implementer: Claude Sonnet 5.5, separate thread).
- Worktree/branch: `/media/jer/Videos/CCF-S1C-T05`, `feature/s1-c-t05-objective-history-v1`.
- **Reviewed production/check SHA: `cd7b5183bec72919a953a2d50d878eeead545303`**; docs tip at review start `4247d90`; base `aca74ac`.
- Authority: `specs/scenario-one-review-history-v1/` (APPROVED, R-1…R-4 binding) — `spec.md` FR-4, FR-10, FR-12, §11, AC-4, R-1; `plan.md` §1.6, §2 item 5; `tasks.md` T-05 scope/verification.

## Commands and evidence

```
git -C /media/jer/Videos/CCF-S1C-T05 log --oneline -4          # 4247d90 / 8e78d66 / cd7b518 / aca74ac
git diff --stat aca74ac cd7b518                                 # 16 files, 11 production + 5 verification
git diff aca74ac cd7b518 -- Assets/                             # full production diff read line by line
git diff --stat aca74ac cd7b518 -- Assets/ForestPrototype/Resources   # empty
python3 -I -B Tools/Verification/S1CObjectiveHistory/run_offline_check.py
    -> S1C_T05_OFFLINE_PASS checks=101
    -> REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_aca74ac
    (git status clean before and after)
grep over Assets/ and Tools/Verification for: SHA256/ComputeHash/Hash(, JsonUtility.ToJson of saves,
     CurrentVersion / version == 2x pins, "racks":[]} tail assumptions
Source reads: ScenarioOneObjectives.Evaluate, ScenarioOneManager EvaluateProgress / AdvanceYear /
     EnsureBaselineSnapshot / RecordEcologicalSnapshot / Start / Resolve* paths, ScenarioReferenceArchive hash paths
```

Not run: Unity, any Unity-hosted gate, other protected offline harnesses (I relied on the handoff's regression table for those).

## 1. Schema — CONFIRMED

- `CurrentVersion = 22`. The only persisted addition is `ScenarioOneSaveData.objectiveAchievements : List<ScenarioObjectiveAchievement>` with exactly `{ string objectiveId; int achievedYear; float targetValueAtAchievement; }` (spec §11). It is the last declared field of `ScenarioOneSaveData`, after the v21 rack run (`rackPlanningModel, rackPlanConfirmed, nextRackId, racks`). No other persisted field changed. `.meta` for the new partial is present.

## 2. Old-save behaviour and retrospective writing — DISPOSITION

- Pre-v22 loads get the empty list (`NormalizeObjectiveHistory`, and `RestoreObjectiveHistory` only reads the list when `saveVersion >= 22`). Nothing is back-filled into the save. **No retrospective write into old saves: confirmed.**
- **R-1 derivation is a spec requirement, not a product ambiguity.** `tasks.md` T-05 scope: "R-1 behaviour for pre-feature saves (labelled exact derivation from year-tagged records where evidence-complete, otherwise 'not recorded')"; T-05 verification: "old-save behaviour matches R-1 (one exactly derivable objective and one 'not recorded')"; AC-4 requires the same evidence; FR-10/R-1 define the rule; `plan.md` §1.6 records the feasibility (pure `Evaluate` over year-tagged snapshots/events/resolved orders). Derivation is an L3 render-time value and is **not** a retrospective write, so it does not conflict with the "no retrospective writes" exclusion. The handoff discloses that it is missing (R-1 note); it is a scope gap in T-05, not a Manager decision. See **B-2**.
- The "already achieved on load stays not recorded" guard is a reasonable reading of "no invented past" in intent, but as implemented it is **unsound in both directions** — see **B-1** (suppresses genuine v22 records) and **B-3** (still fabricates a first-achievement year for pre-v22 lineage).

## 3. Validation — CONFIRMED (one non-blocking note)

- Semantic `ValidateObjectiveHistory` runs after `ValidateRackV2` and immediately before `ValidateManagementEventIds`, which keeps its final position. JSON `ValidateObjectiveHistoryJson` is appended to the existing pre-mutation `TryReadSave` chain. Both are pure and run before any scene change.
- Refuses: null record, unknown id, duplicate id, `achievedYear < 0` or `> ecologicalYear`, decreasing years, non-finite/negative target, non-empty list in a pre-v22 save, missing field/non-array/untyped members/duplicate keys/parse failure in v22 JSON. Empty/missing = valid "not recorded".
- Ordering is deterministic: append order (objective order within a year) at runtime; `Canonicalize` sorts by (year, ordinal id) for hashing only.
- N-1 below: the fixed-id allow-list duplicates the id literals in `ScenarioOneObjectives.Evaluate`.

## 4. Hash-strip compatibility — DISPOSITION: ACCEPT the always-strip rule (G3)

- Every world-identity path drops `,"objectiveAchievements":[…]` regardless of content: `CurrentWorldHash` and `LegacyCompatibleWorldHash` via `HashLayout`; `LegacyV15/V16/V17/V18WorldHash` explicitly, applied before `WithoutRackV2Fields`, so the rack strip's `…"racks":[]` literal still matches. The regex cannot be terminated early by a quoted id and the record contains no nested arrays.
- Header relabel: rack-default worlds → `"version":20` (unchanged rule), rack-state worlds → `"version":21` (what they hashed with at v21). `LegacyCompatibleWorldHash` still keys on `data.version >= 20` → `"version":19`. Correct.
- I accept the always-strip rule. The record is a write-once observation derived from the simulated state already in the hash; "strip only when empty" would move every accepted anchor whose run reaches an objective, which is exactly the silent rebaseline G3 forbids. Recommend this rule is recorded in the decision log / G3 record at integration.
- Reference Future v1: `Load`, `IdentityMatches` (`saveVersion == 12`) and original-bytes re-hash unchanged; `Resources/` byte-identical to `aca74ac` (runner + `git diff --stat`).
- Old v21 hashes: the offline gate reproduces 8 fixture worlds × 7 hash functions exactly against a baseline built from `aca74ac` sources. Residual: proven with `System.Text.Json`, not `JsonUtility` float formatting — Unity gate required.
- Hidden hash paths: the only production hash function is `ScenarioReferenceArchive.Hash`; every save-JSON hash goes through the paths above. Verification harnesses that SHA-256 their own text (`RackV2Combined`, `RackV2Editing`, `MixedDensityOfflineCheck`, etc.) hash evidence/text they build, not save identity; any that hash a raw `CaptureData()` JSON across a run that reaches an objective would now differ — only measurable under Unity (listed under Unity follow-up). No hidden production path missed.

## 5. Recording — PARTIAL

- `RecordObjectiveAchievements(objectives, snapshot.year)` runs inside the `Active` branch of `EvaluateProgress`, before `SetOutcome`; one record per objective, first achievement only; a later loss keeps the record; nothing after the outcome is final. `outcome`/`outcomeYear` code untouched. Reload does not double-stamp (existing record short-circuits). Capture clones; new-scenario reset clears.
- Defects in the load guard: **B-1**, **B-3**.

## 6. History UI — CONFIRMED (subject to B-2)

- Existing `AnnualReviewView` only: one line in the selected year's Forest column ("Objective first reached: …") and one faint line per achieved objective in the existing OBJECTIVES card ("First reached in year N" / "Year reached: not recorded"). No second history UI. "Not recorded" is honest. There is no "derived" label because derivation is absent (B-2). N-2 below on wording.

## Findings

### Blocking

- **B-1 — Load guard permanently suppresses recording in genuine v22 games (FR-10 "new games MUST record").** `ScenarioOneManager.Start` → `EnsureBaselineSnapshot` writes a year-0 snapshot that is **never evaluated** (`EvaluateProgress` runs only from the year advance, `ScenarioOneManager.cs:1355`). `RestoreObjectiveHistory` evaluates the last snapshot and puts every achieved-but-unrecorded objective into `achievedBeforeRecording`. Scenario: new v22 game → save in year 0 (manual save is available) → load. Retention-type objectives such as `retained-canopy` and `continuous-canopy` are expected to be met by the untouched baseline stand (inferred from the objective definitions, not measured here), so they are guarded. At the year-1 evaluation they are skipped, never recorded; the next save is v22 with no record, so every later load re-guards them. The player of a brand-new v22 game sees "Year reached: not recorded" forever. The premise "a save always matches its last evaluation" is false for the baseline snapshot. (Same failure for any future change in targets between builds that makes a previously unmet objective evaluate as met on load.)
- **B-2 — R-1 exact derivation for pre-v22 saves not implemented.** In T-05 scope and verification (see §2). Required: a labelled-derived first-achievement year where it is exactly recomputable from persisted year-tagged L1 records plus the accepted objective logic, otherwise "not recorded", plus AC-4 evidence (one exactly derivable, one "not recorded"). Render-time only; no schema change.
- **B-3 — Pre-v22 lineage can still persist a fabricated "first reached" year (AC-4, FR-10, FR-12).** The guard only covers objectives achieved *at the last snapshot*. Scenario: a v21 save in which `regeneration` was met in year 8 and lost by the last snapshot (year 12); load; it is re-met in year 14 → recorded `achievedYear = 14`, shown "First reached in year 14". The same happens if it was achieved at load, lost, the game saved as v22 and reloaded (the guard set is rebuilt from the new last snapshot only). The true first year is knowable from the persisted year-8 snapshot. Because the list is append-only and indistinguishable from a genuine record afterwards, merging this would write unrepairable false history into test saves.

Suggested direction (implementer's choice; no schema change needed): build the guard and the R-1 derivation from the same pass — evaluate each **evaluated** snapshot year (exclude the never-evaluated baseline, i.e. snapshots without a corresponding `YearAdvanced`) with events/orders filtered to that year. Any objective achieved in a past evaluated snapshot without a record predates recording: it gets a labelled-derived year when exact, else "not recorded", and is never recorded later. Objectives met only at the unevaluated baseline must remain recordable. Add offline cases for both scenarios above.

### Non-blocking

- **N-1** `FixedObjectiveIds` hard-codes the six literals from `ScenarioOneObjectives.Evaluate`; a future objective would make every v22 save carrying it refuse to load. Prefer sourcing the ids from one place, or a source-audit check that keeps them in sync.
- **N-2** "First reached in year N" reads as a fact about the forest, but `achievedYear` is the year-end evaluation that first *observed* it (spec §11 wording). Objectives met at the baseline will show year 1. Consider "First recorded at the year-N review" or similar; wording choice is the Manager's.
- **N-3** `Canonicalize` sorts `objectiveAchievements` in the object passed to the hash functions. Harmless (the field is stripped), but it mutates the caller's data for no effect on the hash; it can be removed.
- **N-4** The handoff's stale-pin list is incomplete (see below).

## 7. Version-pin relabel scope

None of the pins is a production compatibility break. Production version logic uses `>=`/`<` thresholds (`StandGeometryModel.ForSave >= 20`, `RackV2 < 21`, `Migration >= 21/22`, `Controller > CurrentVersion`, `Reference saveVersion == 12`), all of which remain correct at 22. Every pin is in `Tools/Verification/` and asserts "the schema is still 21" as a label.

**Pure label relabels (21 → 22), listed in the handoff:**
`ScenarioOneEconomyIntegrationVerification.cs:104`, `ScenarioOneTeachingCopyVerification.cs:87`, `ScenarioOneCompletionVerification.cs:153`, `ScenarioOneRackUx/RackUxRack.cs:507`, `ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs:82`, `ScenarioOneCompletion/N3/N3CompletionVerification.cs:104`, `ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs:215-216` (source-text pin), `ModelDefault/ModelDefaultVerification.cs:253`, `Enlarged80Calibration/AreaCalibrationVerification.cs:85`, `SequentialObjectives/SequentialObjectivesVerification.cs:229,339`, `RackV2FieldMarking/RackV2FieldMarkingVerification.cs:162`, `StandGeometry/StandGeometryVerification.cs:118`, `U4Guidance/U4GuidanceSmoke.cs:213`, `U4/U4Qualification.cs:335`, `ThreeSpecies/MixedDensityVerification.cs:192`, `ThreeSpecies/ScenarioHistoryVerification.cs:110`, `ThreeSpecies/BroadleafTimberEconomyVerification.cs:107,164`, `RackV2Persistence/RackV2PersistenceVerification.cs:174`.

**Missed by the handoff (captured `version == 21` — also pure label relabels):**
`ScenarioOneCompletionVerification.cs:443`, `ModelDefault/ModelDefaultVerification.cs:327`, `WindthrowV1/StormCoreVerification.cs:37`, `StandGeometry/Enlarged80Verification.cs:273`, `StandGeometry/StandGeometryVerification.cs:157`, `RackV2Save/RackV2SaveVerification.cs:119,232`, `SitkaGrowthMortality/SitkaGrowthModelVerification.cs:97`, `ThreeSpecies/ScenarioHistoryVerification.cs:187`.

**Needs judgement, not just a number:**
- `ThreeSpecies/Qualification/DensityQualification.cs:952` — relabel `CurrentVersion == 21` and `"version":21`. Its key-set equality between growth model 1 and model 2 now includes the `objectiveAchievements` key in both, but if one run records achievements and the other does not, the record keys (`objectiveId`, `achievedYear`, `targetValueAtAchievement`) appear in only one set and the check fails for a reason unrelated to growth models. Decide under Unity whether that is possible in this fixture; if so, exclude the record's keys from the comparison rather than weaken the check.
- `RackV2Save/RackV2SaveVerification.cs:221` (`v20Parsed.version == 20`) is a fixture-input assertion, not a pin; leave it.
- Any harness that compares raw `CaptureData()` JSON byte-for-byte across a run that reaches an objective (e.g. `SequentialObjectivesVerification.cs:343/361` compares `scenarioOne` JSON around a rack action, `RackV2Editing` `SaveJson()` hashes) may now differ because of a new record. Only measurable under Unity; treat any such diff as needing explanation, not a relabel.

Recommendation stands as in the handoff: one narrowly scoped, reviewed "v22 pin relabel" change covering the full list above, owned by whoever runs the Unity qualification, with no assertion weakened.

## 8. Merge decision

- **`cd7b518` is not safe to merge into the v22 integration lineage as-is**: B-3 would persist false first-achievement years into saves that cannot later be told apart from genuine records, and B-1 defeats FR-10 for new games.
- The v22 **schema shape, validation, hash-strip/relabel rule and Reference protection are approved** and should be kept unchanged by the fix; B-1/B-2/B-3 are confined to `ScenarioOneManager.ObjectiveHistory.cs`, the history view/UI labelling and the offline check. No further schema change is needed, so v22 remains the correct number.
- After the fix: re-review (cross-provider) of the delta, then the v22 pin relabel, then Unity gates.

## Required follow-up

1. Fix B-1, B-3; implement B-2 (labelled derived / "not recorded"); extend the offline check with the year-0-save reload case, the pre-v22 achieved-lost-reachieved case, and one exactly-derivable + one non-derivable pre-v22 objective.
2. Re-review of the fix delta.
3. v22 pin-relabel change (full list in §7, including the eight missed sites and the `DensityQualification` key-set judgement).
4. Unity gates (single lock owner): import/compile; real `ScenarioOneManager` capture/restore/`EvaluateProgress`; real `ForestSaveController` disk load and refusals; `JsonUtility` hash reproduction of all v21/v20 and legacy anchors and Reference Future v1 identity; all deterministic/lifecycle anchors and hash-bearing gates; genuine v20 and v21 save load (AC-4 evidence); runtime fixture achieving ≥ 2 objectives in different years with save/load round trip; rendered Annual Review for the new lines.

## Round 1 verdict (superseded by round 2)

S1-C T-05 REVIEW NEEDS CHANGES (at `cd7b518`)

---

# Round 2 — re-review of the fix range `cd7b518..7ae0bbc`

- Reviewer: CLAUDE A, **Claude Opus 5.5** (Anthropic, `claude-opus-5-5`). Static/offline only; no Unity, no lock, no Windows build.
- **Reviewed candidate: `7ae0bbcc5b70385b51a608f62037bf9782a34020`.** Scope: the fix range only, against B-1/B-2/B-3 and N-1…N-4.

## Commands and evidence

```
git diff --stat cd7b518 7ae0bbc
    production: ScenarioOne/ScenarioOneManager.ObjectiveHistory.cs (+57/-), UI/AnnualReviewView.cs (+6/-)
    verification: S1CObjectiveHistory/ManagerStub.cs, ObjectiveHistoryOfflineCheck.cs; docs: HANDOFF.md
git diff --stat cd7b518 7ae0bbc -- ForestSaveData.cs ForestSaveValidation.cs ForestSaveController.cs \
    ScenarioReferenceArchive.cs ScenarioOneSaveMigration.cs Resources/          # empty: schema/validation/hash/Reference untouched
python3 -I -B Tools/Verification/S1CObjectiveHistory/run_offline_check.py      # at 7ae0bbc, clean tree before/after
    -> exit 0, S1C_T05_OFFLINE_PASS checks=115 (115 PASS lines, no FAIL)
    -> BASELINE_DUMP_WRITTEN version=21, REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_aca74ac
Source reads: AdvanceYear (report.year = EcologicalYear + 1; YearAdvanced event then snapshot then EvaluateProgress),
    ResolveFelling (resolvedYear = EcologicalYear + 1), other Resolve* (resolvedYear = report.year),
    git log -S: ecologicalSnapshots, managementEvents and the YearAdvanced event were introduced in the same commit (5866673);
    no pruning of managementEvents/ecologicalSnapshots anywhere.
```

I did not re-run the 5 new mutants; I relied on the implementer's report for those.

## Findings disposition

- **B-1 (year-0 reload) — FIXED.** The guard now replays only years that have a `YearAdvanced` event. The baseline snapshot is year 0 and no `YearAdvanced` event exists at year 0, so a new game saved and reloaded at year 0 guards nothing, and objectives met at the start are recorded at the year-1 review. A genuine v22 game cannot be guarded wrongly afterwards: every evaluated year that achieved an objective wrote a record, so the replay finds no unrecorded achievement. Offline: "new game: reload at year 0 …" (2 checks).
- **B-2 (R-1 derivation) — FIXED.** Display-only `derivedAchievementYears` (never saved), filled from the first evaluated year at which the objective is achieved, using the year's snapshot, events with `year <= Y` and Completed orders with `resolvedYear <= Y`. This matches the live evaluation inputs: `WorkResolved` events and `resolvedYear` both carry `report.year`, which equals that year's snapshot year. A missing snapshot for an evaluated year (`evidenceGap`) stops all later derivations, so the result is "not recorded" rather than a guess. Evaluation after a final outcome is excluded (`lastEvaluatedYear = outcomeYear`). The card labels each year "(recorded)" or "(derived from saved records)", or shows "Year reached: not recorded". There is no write into old saves: `NormalizeObjectiveHistory`/the v22-only read are unchanged, and nothing adds to `objectiveAchievements` except `RecordObjectiveAchievements`. Offline: derivable (3, 2, 5), evidence gap → not recorded, gap earlier → not exact, post-outcome snapshots → not derived, pre-v22 → v22 re-save keeps derived/not recorded.
- **B-3 (lost-and-regained) — FIXED.** The guard is built from **every** evaluated year, not the last snapshot, and is rebuilt the same way on every load. Met 8 / lost 12 / met 14 → never recorded, shown "derived 8"; still derived 8 after saving as v22 and reloading; a v22 control game records 8. Old saves are covered back to the first save that has history: snapshots and `YearAdvanced` events entered the save together, so no save with snapshots lacks the events the guard needs.
- **N-2 (wording)** — partly addressed by the "(recorded)/(derived)" labels. The remaining "first reached" vs "first observed at the year-end review" wording is the Manager's call. Not blocking.
- **N-1 (hard-coded id allow-list), N-3 (`Canonicalize` sorts a stripped field)** — unchanged, still non-blocking.
- **N-4 (pin list)** — FIXED in the handoff. All nine sites I added (`ScenarioOneCompletionVerification.cs:443`, `ModelDefaultVerification.cs:327`, `StormCoreVerification.cs:37`, `Enlarged80Verification.cs:273`, `StandGeometryVerification.cs:157`, `RackV2SaveVerification.cs:119,232`, `SitkaGrowthModelVerification.cs:97`, `ScenarioHistoryVerification.cs:187`) and `DensityQualification.cs:952` are now listed. The pins themselves are correctly untouched in this branch.

## New non-blocking notes (round 2)

- **N-5** The offline check replaces `ScenarioOneObjectives.Evaluate` with a hook. It proves the replay, guard, gap and labelling logic, but not that the real evaluator gives the same answer under the year-filtered events and orders. My source reading says it does (above). The Unity gate must confirm this with a real derivable objective (for example `managed-opening` or a snapshot-threshold objective) on a genuine v20/v21 save.
- **N-6** The replay filter keeps every non-Completed order regardless of when it was created. That is harmless today because `Evaluate` reads only Completed orders. If an objective ever starts to depend on open orders, this filter (and the derivation's exactness) must be revisited, as `plan.md` §1.6 already warns.
- **N-7** Derivation replays with the current build's targets and logic ("accepted objective logic", R-1). If a later build changes targets, a derived year reflects the new logic. It is labelled derived, so this is acceptable, but note it if targets change.
- **N-8** The per-year Forest column ("Objective first reached: …") shows recorded entries only, not derived years. That is honest, and the OBJECTIVES card carries the derived year. Product may later want derived entries there too, with the same label.

## Round 2 merge decision

- The v22 schema, validation, hash-strip/relabel rule and Reference Future v1 protection are byte-identical to what round 1 approved. The fix is confined to the history partial, one UI block and the offline check. **`7ae0bbc` is safe to integrate into the v22 integration lineage for T-05.** This is not acceptance and not Unity verification, and claim #104 stays open.

## Remaining after integration

1. **v22 version-pin relabel**, as one narrowly scoped, reviewed change, using the full list in `HANDOFF.md` / §7 above (the handoff list plus the nine added sites, with `RackV2SaveVerification.cs:221` left alone as fixture input). `DensityQualification.cs:952` needs judgement: relabel `21` → `22`, then confirm under Unity that the model-1/model-2 key-set equality cannot diverge only because one run has achievement records. If it can, exclude the record keys rather than weaken the check. No assertion may be weakened.
2. **Unity gates (single lock owner):** import/compile; the real `ScenarioOneManager` capture/restore/`EvaluateProgress` path, including the year-0 save → reload → year-1 record case; real `ForestSaveController` disk load and refusals; `JsonUtility` reproduction of every v20/v21/legacy hash anchor and the Reference Future v1 identity; all deterministic/lifecycle and hash-bearing gates (explain any raw-save-JSON comparison that moves because of a new record; do not just relabel it); genuine v20 and v21 save loads with AC-4 evidence of one derived and one "not recorded" objective using the real evaluator (N-5); a runtime fixture achieving at least two objectives in different years with a save/load round trip; rendered Annual Review lines for "(recorded)", "(derived from saved records)" and "not recorded".

## Final verdict

**S1-C T-05 REVIEW PASS — APPROVED FOR V22 INTEGRATION LINEAGE**
