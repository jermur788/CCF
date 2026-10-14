# Phase-0 anchors observed in this qualification

Head `2622fe0e10f5` (test-only commits on top of production head `a7a759d`). **23/23** anchors were found in the real Unity Editor logs of gates that passed in this run (searched every `Build/**.log` of this worktree).

| Anchor | What | Observed in |
|---|---|---|
| `BFC55473C1506067` | Neutral lifecycle, legacy (RNG0/regen0/growth0) | BrowsingProtectionVerification, CCFIntegrationVerificationTemp, JuvenileMortalityFoundationVerification, RegenerationModelVerification, Regression/BrowsingProtectionVerification, Regression/CCFIntegrationVerificationTemp, Regression/JuvenileMortalityFoundationVerification |
| `3485B6630C9EA448` | Scenario One lifecycle (browse 0.2), legacy | BrowsingProtectionVerification, JuvenileMortalityFoundationVerification, RegenerationModelVerification, Regression/BrowsingProtectionVerification, Regression/JuvenileMortalityFoundationVerification, Regression/RegenerationModelVerification, Regression/RngModelPolicyVerification |
| `962846D2F517B293` | Neutral, RNG1 regen1 growth0 | RegenerationModelVerification, Regression/RegenerationModelVerification, Regression/SitkaGrowthModelVerification, SitkaGrowthModelVerification |
| `FBB8F470D85FF815` | Normal, RNG1 regen1 growth0 | RegenerationModelVerification, Regression/RegenerationModelVerification, Regression/SitkaGrowthModelVerification, SitkaGrowthModelVerification |
| `7E57B9DAEF5BF4D0` | Neutral lifecycle, growth model 1 (pure Sitka) | Regression/SitkaGrowthModelVerification, SitkaGrowthModelVerification, fast-l40, gate-mixed-l40 |
| `35E2BF1C2F55C2DE` | Normal lifecycle, growth model 1 (pure Sitka) | Regression/SitkaGrowthModelVerification, SitkaGrowthModelVerification, fast-l40, gate-mixed-l40 |
| `7AD177B3CC2F73C7` | Reference Future v1 Y100 | Regression/ScenarioReferenceVerification, ScenarioReferenceVerification |
| `9CDF21A541C5968D` | Reference Future v1 continuation | Regression/ScenarioOneInteractionVerification, Regression/ScenarioReferenceVerification, ScenarioOneInteractionVerification, ScenarioReferenceVerification |
| `56C8B99FA1E8DDD1` | Reference schedule | Regression/ScenarioReferenceVerification, ScenarioReferenceVerification |
| `A564039D9B7CE31D` | Reference start | Regression/ScenarioReferenceVerification, ScenarioReferenceVerification |
| `F7DF7DAB53B6FD32` | Reference Y20 | Regression/ScenarioReferenceVerification, ScenarioReferenceVerification |
| `D5E75D6D21D631AC` | Reference Y50 | Regression/ScenarioReferenceVerification, ScenarioReferenceVerification |
| `FA855239CDDA32D8` | Model2 start | Model2/Model2Verification |
| `02334804F65C0234` | Model2 year 1 | Model2/Model2Verification |
| `702766DECE591E21` | Model2 completion (v18 layout) | Regression/ScenarioOneCompletionVerification, ScenarioOneCompletionVerification |
| `04A78188A6F8E6F9` | Enlarged80 start | Enlarged80/u3-e80 |
| `081DB58CE3034648` | Enlarged80 year 3 | Enlarged80/u3-e80 |
| `EBE7A228F630AE41` | Legacy40 start world | Model2/Model2Verification, P3-results/batch-1, P3-results/batch-2, P3-results/interactive, P3/batch-1, P3/batch-2, P3/interactive |
| `F58FB0B1A421D28B` | P2 determinism | P2-results/batch-1, P2-results/batch-2, P2-results/interactive, P2/batch-1, P2/batch-2, P2/interactive |
| `F7C2FC966816BBAD` | P3 determinism | P3-results/batch-1, P3-results/batch-2, P3-results/interactive, P3/batch-1, P3/batch-2, P3/interactive |
| `8333BAA4126E8A09` | Storm anchor 1 | StormCoreVerification |
| `84796FAEF211823C` | Storm anchor 2 | StormCoreVerification, StormReplayVerification |
| `52037B943B62DD39` | Storm anchor 3 | StormCoreVerification, StormReplayVerification |
