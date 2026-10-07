# Regression on unchanged production

All 19 sequential gates passed on a8596df production. Source ref/SHA256, execution mode, exit and exact markers are in Evidence/regression_results.json. Corrected contract sources from 1a36ea9 were used as disposable copies for integration/removal/clearance/menu; production was not cherry-picked or changed. Clearance/menu ran interactively in the actual Editor/Game view; other gates ran batch. One process at a time, isolated task config and fresh per-worktree Library.

| Gate | Mode | Result | Seconds |
|---|---|---|---:|
| SitkaGrowthModelVerification | batch | PASS | 110.01 |
| RegenerationModelVerification | batch | PASS | 158.32 |
| ScenarioReferenceVerification | batch | PASS | 73.13 |
| ScenarioOneCompletionVerification | batch | PASS | 64.94 |
| ScenarioOneEconomyIntegrationVerification | batch | PASS | 60.05 |
| BrowsingProtectionVerification | batch | PASS | 139.94 |
| ScenarioOnePlantingVerification | batch | PASS | 49.44 |
| ScenarioOnePruningVerification | batch | PASS | 49.05 |
| ScenarioOneDeadwoodVerification | batch | PASS | 48.65 |
| ScenarioOneProgressVerification | batch | PASS | 48.33 |
| SaveHardeningVerification | batch | PASS | 49.24 |
| RngModelVerification | batch | PASS | 38.27 |
| RngModelPolicyVerification | batch | PASS | 78.73 |
| ScenarioOneInteractionVerification | batch | PASS | 73.68 |
| EcologyCalibrationAdoptionVerification | batch | PASS | 93.45 |
| CCFIntegrationVerificationTemp | batch | PASS | 53.48 |
| ScenarioOneRemovalVerification | batch | PASS | 48.77 |
| ClearanceVerification | interactive | PASS | 62.45 |
| MenuTutorialVerification | interactive | PASS | 92.37 |

Frozen Reference: year100 world 7AD177B3CC2F73C7; historical continuation legacy layout 9CDF21A541C5968D; archive contract unchanged. New-game completion D7C4DDD36B53FCCE with models1/1/1/save17. Growth and regeneration legacy anchors preserved. Save hardening and around-threshold deterministic model1 continuation passed. Economy integration reports 120 assertions and world restored/no save slot writes.

These passes verify current production and the diagnostic starting point. They do not certify an unimplemented understorey competition response, its save migration, its per-cell vegetation ledger, or candidate economy/long-run viability. The explicit production gate remains unmet. Any later candidate implementation must rerun appropriate checks after its changes.

## Manager continuation

23/23 PASS after renderer normalization: original19 rerun unchanged contracts plus TimberYield, WorkEconomy, historical canonical BeginCanonical and JuvenileDisplayHeightVerification. Evidence/Continuation/regression_results.json records source hashes, actual batch/interactive modes and complete terminal markers; verification_summary.json records the tested controller SHA256. Both frozen Reference anchors and canonical lifecycle anchors passed. The display fixture measures45 natural cases: Sitka/Oak/Beech × five heights × three refreshes, with30 paired Oak/Beech planted checks, .002m tolerance; maximum target/pair error 1.7e-07m. Authoritative heights/relative abundance unchanged by refresh. This verifies a visual correction, not an adopted competition model. Historical19-gate audit evidence remains unchanged.
