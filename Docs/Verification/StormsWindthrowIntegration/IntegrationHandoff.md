# STORMS & WINDTHROW v1 INTEGRATION HANDOFF

SOURCE: `857150b2f20afa52d7b36be88a835f07a4ee5a51`

PRE-INTEGRATION MAIN: `1fbefd8a40c69dd90abb83f2149e24b546a184cf`

POST-INTEGRATION MAIN: publication commit containing this handoff; the full resulting SHA is recorded in the delivered publication receipt after the push. Gameplay integrated and tested at the exact approved source SHA above.

MERGE METHOD: complete two-commit fast-forward into clean `integration/storms-windthrow`, `/media/jer/ZX20/CCF-integration-model2`. The dirty checkout's local main branch was not moved; the verified integration tip is published directly to origin/main without force.

SAVE: 19. Exactly root stormModel and resolved event year/severity/directionDegrees/cropTreesLost; no additional storm persistence.

NEW GAME: rng1 / regen2 / growth1 / storm0. Existing/missing-field saves and Reference Future remain storm0.

STORMMODEL1 PROFILE: annual .02 / severity .02-.06-.18 / uniform weights / ReducedProposal / BoundedRational / neutral site1 / neutral or absent external edge. Frozen [C] gameplay calibration, not measured Irish hazard or engineering constants. Material recalibration needs StormModel2 or separately approved persisted-profile/version architecture.

MODEL0: PASS, dormant fields produce no storms/events and historical layouts retain their exact anchors.

MODEL1: PASS, resolved forced storm, ordinary random-storm future, event state, windthrow mortality/deadwood and optional salvage survive save/reload.

SAVE/LOAD: focused core, salvage, separate-process replay, RNG0/RNG1 replay and SaveHardening PASS. One authoritative mortality/deadwood/work path; no duplicate victim or salvage history.

DETERMINISM: ID-keyed failure, reversed iteration, save/reload and independent-process future through year 10 PASS. Storm fixture resolved 84796FAEF211823C / future 52037B943B62DD39. These are fixture evidence, not frozen Reference anchors or a cross-platform promise.

WINDTHROW: one uprooted/fallen outcome; removes live stock, records cause/year, updates existing recentOpening, batches canopy/seed rebuilds, and preserves causal UI. No snapped stems added.

DEADWOOD: exactly one record per victim; ordinary lifecycle and persistence gates PASS.

SALVAGE: Leave default; optional individual/partial SellAndExtract or KeepForUse; repeated extraction and save/reload checks PASS.

MODEL2 INTERACTION: shared canopy/light and ordinary understorey/recruitment causality; focused 120 checks and Model2 1400 checks PASS. No coefficient retuning.

ECONOMY: salvage work multiplier 1.00; grouped settlement, prices and contractor minimum unchanged. Salvage 983 checks, economy integration and work/yield gates PASS.

UI: retained qualitative Stable/Watch/Exposed, causal/storms-off explanation, fallen-stem selection, Work Plan salvage and Annual Review. Existing menu/clearance/waypoint/P1 pedagogy regression checks PASS. No new storm tutorial objectives or player-facing exact hazard percentages. The approved source's rendered UI evidence remains under Docs/Research/WindthrowV1/Evidence; no new storm artwork or performance acceptance is claimed.

PERFORMANCE: fresh 40-case simulation gate PASS, with exact ID-roll victims, bounded record/rebuild counts and stable production bytes. Non-warmup annual times:

| Trees | Samples | Annual ms min–max | Median ms |
|---|---:|---:|---:|
| 336 | 9 | 59.23–93.49 | 61.94 |
| 1300 | 9 | 289.50–408.89 | 313.25 |
| 5000 | 12 | 1475.86–1945.80 | 1745.09 |

These are Editor simulation timings, not Player frame times. Approved source rendered evidence reported about 100 ms developed/century Editor medians even with storm visuals disabled, and about 240 ms in the 1300-tree stress scene; those are retained source measurements, not fresh integration measurements. Current evidence suffices for dormant integration only. Standalone/developed/century/combined profiling and acceptable crown/root behaviour remain release requirements before default activation; no retrospective FPS target is introduced.

24-GATE REGRESSION: 24/24 PASS, freshly run from the integrated production state. Exact modes, immutable P0 fixture source SHA, disclosed synthetic-v5 fixture adjustment and verification source hashes are in regression-results.json. All launch/finish production manifests agree. No P0 UI branch or future P2/P3 conflicts were integrated.

| Gate | Result | Seconds |
|---|---|---:|
| SitkaGrowthModelVerification | PASS | 109.62 |
| RegenerationModelVerification | PASS | 165.28 |
| ScenarioReferenceVerification | PASS | 79.20 |
| ScenarioOneCompletionVerification | PASS | 75.91 |
| ScenarioOneEconomyIntegrationVerification | PASS | 70.84 |
| BrowsingProtectionVerification | PASS | 134.95 |
| ScenarioOnePlantingVerification | PASS | 54.65 |
| ScenarioOnePruningVerification | PASS | 55.36 |
| ScenarioOneDeadwoodVerification | PASS | 56.66 |
| ScenarioOneProgressVerification | PASS | 55.13 |
| SaveHardeningVerification | PASS | 50.00 |
| RngModelVerification | PASS | 40.07 |
| RngModelPolicyVerification | PASS | 89.49 |
| ScenarioOneInteractionVerification | PASS | 76.00 |
| EcologyCalibrationAdoptionVerification | PASS | 100.79 |
| CCFIntegrationVerificationTemp | PASS | 59.72 |
| ScenarioOneRemovalVerification | PASS | 49.75 |
| ClearanceVerification | PASS | 79.10 |
| MenuTutorialVerification | PASS | 89.03 |
| TimberAssortmentYieldVerification | PASS | 50.37 |
| Stage1WorkEconomyFoundationVerification | PASS | 38.75 |
| JuvenileMortalityFoundationVerification | PASS | 54.41 |
| JuvenileDisplayHeightVerification | PASS | 48.89 |
| Model2PedagogyVerification | PASS | 58.40 |

MODEL2: fresh 1400-check gate; v18 start FA855239CDDA32D8 and altered-site year1 02334804F65C0234. Modern completion preserves v18 projection 702766DECE591E21. Full v19 hashes include the intentionally added dormant schema fields. Historical altered-site fixture retains SiteProductivity0; clean one-year 8333BAA4126E8A09 is a distinct fixture, not a correction to the frozen anchor.

LEGACY MODEL1: separately forced historical completion PASS, v17 projection D7C4DDD36B53FCCE. Exact result/markers in legacy-model1-completion.json.

REFERENCE: archive unchanged; storm0. Fresh verification checks Y100 7AD177B3CC2F73C7 and continuation 9CDF21A541C5968D.

CANONICAL COMMIT: `58d6b0b483198abcf0208713d9a759f89d90e318` (D-050 and supporting canonical updates).

CONTEXT COMMIT: `58d6b0b483198abcf0208713d9a759f89d90e318`. The entire 12-source snapshot, 12 mirrors, both composites and manifest use this full SHA; generation refuses dirty canonical files.

DRIVE/MIRRORS: 15/15 permanent Drive files updated in place and downloaded bytes verified; Game Dev instructions saved/reopened with exact generated SHA256 and all seven stamped canonical/manifest attachments ready. Older uploads remain older copies, not current. Claude composite/role mirrors refreshed on Drive; no separate Claude project upload is claimed. Exact publication state and verified readback are recorded in context-publication.json. Permanent raw mirrors are updated in place; IDs/folders/sharing remain unchanged. Platform attachment status is explicitly recorded rather than inferred from Drive publication.

DIRTY CCF-main: preserved. Before/after comparison checks original HEAD 402a2b41108dc9aa91e225db9047956b0a91a97e, exact porcelain status and all 11 dirty/untracked file SHA256s. No stash/reset/clean or local-main branch move.

WORKTREE CLEAN: checked after final documentation commits; disposable verification scripts/metas removed. Twelve inspected Editor-generated settings/material changes are backed up and restored to exact tracked bytes; no unrelated asset change is absorbed. Production manifest still matches all 204 approved files.

FOLLOW-UPS: bespoke fallen crowns; root/crown close-up polish; century combined deadwood/windthrow rendering optimisation; standalone Player FPS profiling; final new-game activation/default decision; full storm lesson/progression; surrounding-landscape directional exposure; snapped stems/snags. None is marked complete.

CLAUDE P2/P3: must rebase/port onto newly published main before final verification because TreeInspectionView, WorkPlanView, AnnualReviewView, ScenarioOneUiRoot and StandMapView changed. Their future conflicts remain outside this task.

STATUS: STORMS & WINDTHROW v1 INTEGRATED, once publication receipt confirms origin/main. See exact verification results, 204-file production manifest and evidence hashes in this directory.
