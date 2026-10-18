# Scenario One verification debt repair — record

Verification-only repairs to the four stale regression gates and F3. **No production file changed.**
Prepared for later integration/rebase onto the accepted Scenario One line. Not merged.

Base: `8942b5c3c2adb66164573f0c12a92b31adfcb0cf` (branch `tooling/scenario-one-verification-debt-repair`).

| Target | Repair | Production change |
|---|---|---|
| `ScenarioReferenceVerification` (runner) | Protect the frozen contract + `deterministic=True` + the **documented** continuation fingerprint, held in named provenance-linked constants (`REFERENCE_ARCHIVE_YEAR100_HASH`, `REFERENCE_CONTINUATION_V15_LAYOUT_HASH`). No bare literal. Provenance doc brought onto the branch. | none |
| `ScenarioOneEconomyIntegrationVerification` | Non-merchantable stems (below merchantable height / too small to grade) must earn **zero** revenue on any model; the Oak/Beech market expectation is now **model-dependent** (`BroadleafMarketActive`), exercised on both model 2 and legacy. Restores all downstream checks. | none |
| `ScenarioOneProgressVerification` | Fixture now performs the reviewed two-cycle chronology: acknowledge the first Annual Review through the real handler, resolve a second work plan at year 2, then assert review-year completion. No production two-cycle requirement weakened. | none |
| `Model2PedagogyVerification` | Clearance foldout selected by the **unnumbered topic title taken from the curriculum** (`LearningObjectivesView.Topics`), not the `"7."` ordinal. | none |
| `F3 / check_integrated_v1.py` | Source-integrity guard now hashes a **normalized** source (comments and non-semantic whitespace outside string literals ignored); the 360 calibrated checkpoints always run. Positive/negative tests added. | none |

## Provenance brought onto the branch

- `Docs/Verification/AnchorPolicy/ObservedIntentionalAnchorDeltas.md` — copied byte-identically from
  `tools/verification-fixture-fixes-v1` (blob `a1b372e514c14bde8bff30676e041ca683969299`).
  Records `9CDF21A541C5968D -> BFFF66015A6619DA` as an intentional Oak/Beech calibration delta.

## Density (unchanged)

`Docs/Verification/ThreeSpeciesProgramme/CompositeDensityKnownMisses.md` — the three accepted
reduced-simulator misses (0.423 / 0.407 / 0.417) retained against the unchanged 0.40 threshold.

## Verification performed (static, no Unity)

- `check_integrated_v1.py` → `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7`.
- `test_source_normalization.py` → `SOURCE_NORMALIZATION_PASS` (comment/whitespace tolerated;
  constant, token, string-literal and interpolated-expression changes detected).
- Runner ScenarioReference predicate unit-tested (good content passes; stale literal, non-determinism
  and a broken archive anchor all fail).
- `run_typecheck.py --mode tools` → no new errors in the modified files (only the 5 known
  `UxQualification.cs` stub gaps).
- Density control run reproduces `COMPOSITE_DENSITY_FAIL checks=107 failures=3`.
- `py_compile` on all modified Python; brace/paren balance on the modified C#.

Unity Editor / Player re-runs of the four gates are the remaining integration step (not run here).

## Follow-up — residual verification debt closure

Second verification-only commit on the same branch (residual debt from
`ResidualVerificationDebt-20261018T113710Z`).

- `Tools/Verification/run_clearance_gate.py` and `Tools/Verification/run_s1a_gates.py`: the Reference
  continuation expectation now uses the documented fingerprint `BFFF66015A6619DA` (named constant
  `REFERENCE_CONTINUATION_V15_LAYOUT_HASH`) under the marker the gate actually prints
  (`legacyV15LayoutHash=`); the frozen archive anchor `7AD177B3CC2F73C7`
  (`REFERENCE_ARCHIVE_YEAR100_HASH`) is preserved separately. The stale `9CDF…` expectation is gone.
- `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`: the raw whole-file sha256 of
  `ScenarioOneCompletionRules.cs` is replaced by a normalized-source guard
  (`N3_RULES_NORMALIZED_SHA256`) using the shared module; a comment/whitespace edit no longer
  suppresses the 88 N3 checks.
- `Tools/Verification/source_normalization.py` (new): the single comment/whitespace-insensitive
  normalizer, now imported by both the F3 guard and the N3 guard.
- `Tools/Calibration/OakBeechGrowth/check_production_v1.py`: formally RETIRED (stub, clear message,
  non-zero retirement exit code); replacement is `check_integrated_v1.py`.
- `Tools/Verification/S1A_README.md`: required continuation anchor updated to `BFFF66015A6619DA`.
- `Tools/Calibration/OakBeechGrowth/test_source_normalization.py`: positive/negative tests extended to
  `ScenarioOneCompletionRules.cs`.

Deferred: `check_sources.py` (owns a distinct production-ownership contract bound to a stale
candidate-v1 list; needs a Manager re-scope, not a weakening); `Enlarged80ViabilityVerification`
model selection (exact minimal fix documented, deferred — see the residual-debt audit).
