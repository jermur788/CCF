# Verification fixture fixes: BroadleafAdultGrowthVerification and MixedDensityVerification

Branch `tools/verification-fixture-fixes-v1`, base `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a` (the overnight lineage both feature branches share). **Tools only: no file under `Assets/`, `Packages/` or `ProjectSettings/` changed.** No expected hash anywhere was changed.

## Why

Both gates were written in a code-only cloud session and had never run in Unity. The calibration qualification found they fail for reasons that are not the code under test:
- `BroadleafAdultGrowthVerification` failed on both geometries with the old rows too: between years 43 and 50 natural recruits join its nine-tree sparse stand, so `Living().Length == 9` and the probe tree (`Living()[4]`, sorted by ID) break.
- `MixedDensityVerification` failed on Enlarged80 on two assertions that carry Legacy40 expectations: the lifecycle anchors `7E57…`/`35E2…`, and the v16-save load.

## What changed

**`BroadleafAdultGrowthVerification`**: every annual step now goes through `Step()`, which advances one year and removes every `ForestTree` that is not one of the nine grid trees (IDs `O00..O08`, `B00..B08`, `S00..S08`). Nothing else changed. All checks keep their text and condition, including `Living().Length == 9` (its message now also reports the count), the closed-form law with CI 0 and site 1, the competition and determinism checks, and the Sitka check. Removed lines in the diff are only the `e.AdvanceOneYear()` calls that became `Step()` and that one message string. A new report line prints how many recruits were removed per species, so the control is visible.

**`MixedDensityVerification`**:
- `TryLifecycleExpectation(geometry)` selects the lifecycle expectation from the session's own stand geometry. Legacy40: the accepted anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`. Enlarged80: `1FE6639780636394` / `702DE05F33384139`, labelled in the gate as **observed** under growth model 1 in the density-v2 qualification, not Manager-accepted. Any other geometry fails with "no expectation defined" instead of being compared with another geometry's values.
- The lifecycle phase now runs the fixture under growth model 1 **and** 2 in the same geometry, and a new check `LIFECYCLE_ANCHORS_MODEL2_EQUALS_MODEL1_MEASURED` asserts they are equal without any pinned value. `LIFECYCLE_ANCHORS_GROWTH2_EQUAL_GROWTH1` keeps its name and now compares against the selected geometry's table.
- v16 policy: a v16 save is a Legacy40 format (saves up to v19 must state Legacy40). Under Legacy40 the gate asserts it loads and maps to growth model 0, as before. Under Enlarged80 it asserts that a v16-labelled save carrying Enlarged80 geometry is **refused** and the session is unchanged; the v16 to model 0 mapping is a Legacy40 property checked in the Legacy40 run.
- The Enlarged80 run therefore takes about twice as long in the anchor phase (four 80-year lifecycle runs instead of two): 294 s against about 225 s.

## Runs (real Unity 6000.6.0f1, batch, staged by `Tools/Verification/Qualification/run_qualification.py`, working tree otherwise untouched)

Only these two gates were run, on the calibrated growth head's Library (`verify/oak-beech-production-calibration-796da99`, new Oak/Beech rows). Not the regression.

| gate | geometry | result |
|---|---|---|
| `BroadleafAdultGrowthVerification` | Enlarged80 | PASS, 16 checks; recruits removed: 540 after the Oak run, 1 680 cumulative after the Beech run |
| | Legacy40 | PASS, 16 checks; recruits removed: 540 after the Oak run, 1 272 cumulative after the Beech run |
| | Enlarged80 with the **pre-calibration Oak/Beech rows** applied in memory | PASS, 16 checks (the same gate failed there before the fix) |
| `MixedDensityVerification` | Legacy40 | PASS, 23 checks; anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, model 1 = model 2 measured |
| | Enlarged80 | PASS, 23 checks; `1FE6639780636394` / `702DE05F33384139`, model 1 = model 2 measured; v16-labelled Enlarged80 save refused |

Also: `Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools`: no new errors against the baseline of 43. Evidence: `evidence/` (results JSON with run head and staged-gate SHA-256, markers). The staged SHAs are `1c95502a…` (broadleaf) and `2ba4bae5…` (mixed density), the files committed on this branch.

## Not claimed

No full Unity suite. The Enlarged80 values in the mixed-density gate are observations, not accepted anchors, and say so in the gate. I did not run these fixed gates on the U4 or UX lineages; the fix touches no production file.
