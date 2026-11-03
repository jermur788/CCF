# Stand Map Species Canopy Differentiation — Integration Handoff

**Status:** INTEGRATED / FINAL SCENARIO ONE PLAYER-TEST CANDIDATE PUBLISHED  
**Claim:** #242 (integration of #232)  
**Manager verdict on feature:** APPROVED  
**Target branch:** `integration/scenario-one-player-test-ready-v1`

## Integration result

The published Scenario One integration branch was still exactly at the Tier-3-approved production base when Browser Overall performed integration. The approved #232 production commit is a direct child of that exact base.

Browser Overall therefore advanced the integration branch by a lease-checked, non-forced fast-forward directly to the already-qualified #232 production commit. No cherry-pick, merge reconstruction, conflict resolution, or product-byte rewrite occurred.

This establishes byte identity between the integrated product/test delta and the feature delta that received the affected rendered/Tier-3 qualification and independent cross-provider approval.

## Integrated delta

Exactly the already-approved #232 two-file delta:

- `Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs`
- `Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs`

No other production/test files were introduced by integration.

## Qualification inheritance

The accepted feature handoff records:

- offline Stand Map checks 176 passed / 0 failed;
- mixed-species before/after renders at 1280×720 and 1920×1080;
- unchanged 1,416-tree map-model fingerprint and simulation WorldHash;
- Combined HUD/minimap 465 / 0;
- Rack map-planning PASS;
- Rack confirm/minimap PASS;
- PARITY-07 rendered PASS;
- compile PASS;
- Scenario One completion PASS with the accepted completion fingerprint;
- no material canopy redraw regression;
- fresh independent OpenAI read-only review: APPROVED.

Because integration is the exact already-qualified commit on its original parent, no Unity or rendered/Tier-3 rerun is required solely for integration.

## Final candidate

`integration/scenario-one-player-test-ready-v1` is now the exact final Scenario One player-test candidate, including the approved Stand Map species-canopy differentiation.

The next gate is the user's personal Scenario One playtest.

Required token:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

No Windows build is authorised before that personal playtest and explicit post-playtest user authorisation.

## Cleanup note

Browser tooling created two temporary remote branch refs while checking integration mechanics (`tmp-noop-integration-check` and `noop`). They point at the integrated candidate and do not alter product or integration state. The available browser connector does not expose branch deletion; local orchestration should delete those two refs as housekeeping only. This cleanup is non-blocking and does not affect the candidate.

#194 remains NO ACTION.
