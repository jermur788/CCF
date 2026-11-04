# MFM-06 — Independent Review (#112)

- **Reviewer:** Claude Opus 5.5 (Claude Code via T3), independent of the implementing thread. Code-reading and offline checks only. No Unity run, no production/tooling edits, no commits, Unity lock not taken.
- **Candidate reviewed:** `60f17ef95fb2fca2c04bc61e38ff10bedc1857d8` (base `ac876a64969752a3864034521f42559671264b30`). `d23a29c` adds only the handoff doc.

**Verdict:**

NEEDS CHANGES

There is one narrow blocking finding (F1): a small fix plus one test. Everything else is approvable as it stands. No existing anchor, Legacy40 world or Reference Future identity is affected by F1. It has to be fixed before MFM-07 mints any prescription-world identity, because fixing it afterwards would mean a rebaseline.

## Findings (most severe first)

### F1 — MEDIUM (blocking): prescription-bearing worlds put v22 objective history into the world identity hash
`Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs:249-250` (`HashLayout`): `if (!PrescriptionStateIsDefault(data)) return json;` returns before `WithoutLaterFields`, so `WithoutObjectiveAchievements` is never applied. The accepted v22 rule, in the same file's header comment (~lines 216-220) and in `ForestSaveData.cs:99`, says objectiveAchievements is "NEVER part of a world identity … whether or not it is empty". With this code, two prescription worlds that are identical apart from observation history get different `WorldHash`/`CurrentWorldHash` values. The same thing already happens for rack-bearing worlds after v22, and that path strips history. The prescription path is the only one that does not.
**Fix:** in the non-default branch, return `WithoutObjectiveAchievements(json)`, keeping the full v23 header and fields. Add an offline check that a prescription world with history and the same world without it hash equal. The handoff's "hashes its full current JSON" wording should then be adjusted. Note that the offline check "rule, decision and provenance changes each move the identity" passes either way, so no existing test covers this.

### F2 — LOW: order provenance is not cross-checked against its prescription
`Assets/ForestPrototype/ForestSaveValidation.cs` `ValidateOrderProvenance` (around diff line 254-274 / file ~466+). An order naming an approved prescription is accepted when:
- its `prescriptionCoverageUnitId` is not among that prescription's `coverageDecisions`, or has choice `LeftManual`;
- its `prescriptionCropTreeId` is not in `sampleCropTreeIds` or the save's Crop Tree set.

J3 provenance can therefore point at a unit the prescription never covered. Nothing writes such orders yet, so this is not blocking for MFM-06. It is recommended either in MFM-07's validator or here, if cheap. Whether "covered" units are only those with stored decisions depends on the MFM-07 recompute rule, so this is flagged as a design question rather than a defect.

### F3 — LOW: reason code 0 is both the only valid reason and the field default
`ForestSaveData.cs` `PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree = 0`. A generated order whose reason was never set validates as a real reason. The JSON-presence check stops this for disk loads, but not for in-memory `LoadData`. A sentinel of 0 = none with real codes from 1, mapped from `ProposalReason`, would be safer. This is cheap to change now, before any v23 save exists in the wild, and costly afterwards.

### F4 — LOW: weak source checks
- `sourceAnchorX/Z` are only checked as finite. They are not checked against the stand bounds or against the source rack.
- `sourceRackId` is checked only against the allocator. This is a deliberate trade-off, disclosed in handoff residual 4, and I accept it.

### F5 — INFO: stale comment
`ForestSaveData.cs:103` refers to `ScenarioReferenceArchive.WithoutPrescriptionState`, which does not exist. The method is `WithoutLaterFields` / `PrescriptionStateIsDefault`.

### F6 — INFO: `LegacyCompatibleWorldHash` still relabels the header to v19 and drops the geometry field for a prescription world
A Legacy40 world is not a Rack V2 world, so the validator already refuses prescriptions there and the case cannot happen. It is only an inconsistency with the "full current JSON" description.

### Checked and found sound
- **Schema:** `CurrentVersion` goes 22→23. Base was 22 at ac876a6.
- **Migration:** `NormalizePrescriptions` (version <23) clears the allocator, records, and order/event links. `RestorePrescriptions` reads only from v23+ saves. `InitializeNewScenario` calls `ClearPrescriptions`. A grep of `Assets` finds no prescription creator, approval or order linking outside restore and clone. Old marks and orders cannot acquire provenance.
- **Pre-v23 saves that claim state are refused.** `Validate` has no early `return null` before the call, so the check is always reached.
- **Validation before mutation:**
  - `ValidatePrescriptions` is called last in `Validate`.
  - `Validate` is reached only through `FindLoadProblem`, which `LoadData` and `CanLoad` share.
  - `ValidatePrescriptionsJson` is called last in the `TryReadSave` chain, which `Load` and `CanLoad` share.
  - The first mutation (`ApplyStandGeometry`) comes after `FindLoadProblem`.
  - Null records, null lists and null decisions are handled. A JSON parse failure is caught. I found no path that throws instead of returning a refusal.
- **Typed validator:** duplicate ids, the id≥allocator check and allocator<1 are covered. Version text uses a closed character set. Approval state and year are coherent: Accepted ⇒ -1, and Approved ⇒ sourceYear ≤ year ≤ ecologicalYear. Duplicate coverage units and unknown choices are refused. An ordinary order with any provenance is refused. An order from an unapproved prescription, or created before approval, is refused. An event naming an unknown prescription is refused.
- **Hash for default-state worlds:** the exact default v23 run is removed. Order fields come before the bare event field, so the event pattern cannot clip the order run, and a `,"prescriptionId":0` prefix cannot collide with any other key. The header relabel is unchanged. V15-V18 return null for prescription worlds.
- **Tooling diffs:** in the 26 relabelled gates, outside the new `MfmSaveProvenance*` dirs, every changed line is a 22→23 pin or label (I read the full `grep '^[-+]'` output). The three comparators (`StandGeometry`, `RackV2Persistence`, `RackV2Save`) only gained the three default-v23 `Replace` strips plus comments. No assertion was weakened. I did not do a byte-level comparison with `d5dbe97`; the pattern matches its description.
- **Scope:** `git diff --stat ac876a6 60f17ef -- Assets/ForestPrototype/ScenarioOne/Resources` is empty. No `.unity`, `.prefab`, ProjectSettings or Packages files changed. There is no UI, broadleaf or calibration change. The new `.meta` is for the new partial `.cs` only.

## K1–K10

| ID | Result | Basis |
|---|---|---|
| K1 next schema (v23) | PASS | code + base check |
| K2 missing/old ⇒ no prescription | PASS | Normalize/Restore code; offline absent-field checks |
| K3 no fabricated migration | PASS | code + grep; offline audit |
| K4 stable ids | PASS | allocator validation + restore; Unity ×3 round trip (log) |
| K5 approval/history/provenance persisted | PASS (with F1 and F2 caveats) | DTOs cover §14; J1/J2/J3/I6 fields present |
| K6 transients not persisted | PASS | no proposal/marker fields in DTOs |
| K7 Legacy40 unaffected | PASS | hash-dump candidate = base (I diffed it); reference/hardening/n3 etc. PASS receipts |
| K8 pre-feature Enlarged80 valid, nothing fabricated | PASS | offline fixtures; Unity save gate v22-shaped file (log) |
| K9 Reference Future untouched | PASS | empty Resources diff (I ran it); offline runner token |
| K10 malformed fails before mutation | PASS | traced ordering myself; offline ~70 refusals + Unity 23 docs/7 typed (log/handoff) |

## Verified myself vs taken from the handoff

**Verified myself:**
- read the full `Assets` diff and the controller/validator call paths;
- ran `python3 -I -B Tools/Verification/MfmSaveProvenance/run_offline_check.py`, which gave `MFM06_SAVE_OFFLINE_PASS checks=169` and `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_ac876a6`, exit 0;
- `diff run2/03-hash-dump/hash-dump.txt run2/04-base-hash-dump/hash-dump.txt` is identical (18 lines);
- `02-save/unity.log` contains `MFM06_UNITY_PASS checks=155` and no FAIL lines;
- read every `run2/*/process-result.json`: head = 60f17ef (the base dump is ac876a6), unity_exit 0, `host_saves_unchanged` true, no lane changes;
- `progress`: `pass:false` only because `disk_saves_created` holds an isolated `forest-save.json.bak` (`failures: []`, marker present, exit 0). The handoff's explanation is accurate.
- `density-save`: `DENSITY_QUAL_PASS` is at log line 3877. The log ends in the Editor's shutdown memory-leak report, and the runner hit its timeout (`unity_exit: None`). The handoff's explanation is accurate.

Neither of those two blocks integration: both are runner-strictness or post-pass shutdown artefacts, not assertion failures. A clean re-run of `density-save` is advisable but optional.

**Not run:**
- `run_mutation_controls.py` was **skipped** for time. I read its 23 mutant definitions: they are semantic (they remove real guards and invert real conditions), not tautological. The 23/23 kill result comes from the handoff.
- No Unity run by me.

**Taken from the handoff:** the Unity gate details beyond the markers and receipts, and the mutation results.

**On test meaningfulness:**
- The offline round-trip, refusal and baseline-hash fixtures are meaningful.
- The "audit" checks are source-text greps: weak on their own, but they are backed by mutants M20-M22.
- The comparator strips copy the production literals, so they are not independent of production code. That follows the v22 precedent.

## Residual risks

1. F1 must be fixed before any MFM-07 prescription-world anchor exists.
2. F2/F3 semantics need settling with MFM-07, ideally F3 before v23 ships.
3. 20+ relabelled pin gates were not executed. They are only label changes, but they are unrun.
4. The DeepSeek-made runner (`MfmSaveProvenanceUnity/run_gate.py`) has a reasonable pass formula (line ~260). I did not audit its cleanup paths line by line.
5. The implementer deviated from the routed model (disclosed). This does not affect correctness.
