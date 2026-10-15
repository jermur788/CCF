# Phase-0 anchors observed in the Unity logs of this v3 run

Head `90e3496` (production unchanged since `8e1e7b9`). **23/23** anchors found in the real Editor logs of gates of this run that passed (24-gate regression, anchor carriers, P2/P3 batch+interactive).

| Anchor | What | Observed in |
|---|---|---|
| `BFC55473C1506067` | Neutral lifecycle, legacy (RNG0/regen0/growth0) | BrowsingProtectionVerification, CCFIntegrationVerificationTemp, JuvenileMortalityFoundationVerification, RegenerationModelVerification, RngModelPolicyVerification, ScenarioOneInteractionVerification, SitkaGrowthModelVerification |
| `3485B6630C9EA448` | Scenario One lifecycle (browse 0.2), legacy | BrowsingProtectionVerification, JuvenileMortalityFoundationVerification, RegenerationModelVerification, RngModelPolicyVerification, SitkaGrowthModelVerification, regression-regeneration-rerun/RegenerationModelVerification, regression-results/BrowsingProtectionVerification |
| `962846D2F517B293` | Neutral, RNG1 regen1 growth0 | RegenerationModelVerification, SitkaGrowthModelVerification, regression-regeneration-rerun/RegenerationModelVerification, regression-results/RegenerationModelVerification, regression-results/SitkaGrowthModelVerification |
| `FBB8F470D85FF815` | Normal, RNG1 regen1 growth0 | RegenerationModelVerification, SitkaGrowthModelVerification, regression-regeneration-rerun/RegenerationModelVerification, regression-results/RegenerationModelVerification, regression-results/SitkaGrowthModelVerification |
| `7E57B9DAEF5BF4D0` | Neutral lifecycle, growth model 1 (pure Sitka) | SitkaGrowthModelVerification, regression-results/SitkaGrowthModelVerification |
| `35E2BF1C2F55C2DE` | Normal lifecycle, growth model 1 (pure Sitka) | SitkaGrowthModelVerification, regression-results/SitkaGrowthModelVerification |
| `7AD177B3CC2F73C7` | Reference Future v1 Y100 | ScenarioReferenceVerification, regression-results/ScenarioReferenceVerification |
| `9CDF21A541C5968D` | Reference Future v1 continuation | ScenarioOneInteractionVerification, ScenarioReferenceVerification, regression-results/ScenarioOneInteractionVerification, regression-results/ScenarioReferenceVerification |
| `56C8B99FA1E8DDD1` | Reference schedule | ScenarioReferenceVerification, regression-results/ScenarioReferenceVerification |
| `A564039D9B7CE31D` | Reference start | ScenarioReferenceVerification, regression-results/ScenarioReferenceVerification |
| `F7DF7DAB53B6FD32` | Reference Y20 | ScenarioReferenceVerification, regression-results/ScenarioReferenceVerification |
| `D5E75D6D21D631AC` | Reference Y50 | ScenarioReferenceVerification, regression-results/ScenarioReferenceVerification |
| `FA855239CDDA32D8` | Model2 start | Model2/Model2Verification |
| `02334804F65C0234` | Model2 year 1 | Model2/Model2Verification |
| `702766DECE591E21` | Model2 completion (v18 layout) | ScenarioOneCompletionVerification, regression-results/ScenarioOneCompletionVerification |
| `04A78188A6F8E6F9` | Enlarged80 start | Enlarged80/v3-e80 |
| `081DB58CE3034648` | Enlarged80 year 3 | Enlarged80/v3-e80 |
| `EBE7A228F630AE41` | Legacy40 start world | Model2/Model2Verification, P3-results/batch-1, P3-results/batch-2, P3-results/interactive, P3/batch-1, P3/batch-2, P3/interactive |
| `F58FB0B1A421D28B` | P2 determinism | P2-results/batch-1, P2-results/batch-2, P2-results/interactive, P2/batch-1, P2/batch-2, P2/interactive |
| `F7C2FC966816BBAD` | P3 determinism | P3-results/batch-1, P3-results/batch-2, P3-results/interactive, P3/batch-1, P3/batch-2, P3/interactive |
| `8333BAA4126E8A09` | Storm anchor 1 | StormCoreVerification |
| `84796FAEF211823C` | Storm anchor 2 | StormCoreVerification, StormReplayVerification |
| `52037B943B62DD39` | Storm anchor 3 | StormCoreVerification, StormReplayVerification |
