# STORMS & WINDTHROW v 1 HANDOFF

Worker: Codex / Sol. Role: primary implementation and calibration worker. This is task-branch work, not integration or acceptance.

## BASE / BRANCH / HEAD

- Base: `1fbefd8a40c69dd90abb83f2149e24b546a184cf`, published main after Regeneration Model 2 integration.
- Context: `62593b28e14a5b5818c414450f168c333df683d7`.
- Branch: `task/storms-windthrow-v1`.
- Worktree: `/media/jer/ZX20/CCF-storms-windthrow`.
- Implementation HEAD: `4c40b0eae801a8816af05583874e38ada863ac2e`. The handoff is committed separately; use the final branch HEAD when integrating.

## SAVE / STORM MODEL / NEW-GAME DEFAULT

Save 19. RNG 1, Regeneration 2 and Growth 1 remain the current new-game models. StormModel 0 preserves historical no-storm behaviour; StormModel 1 implements events and windthrow. Existing/missing-field saves, Reference Future v 1 and new Scenario One games retain StormModel 0. The user's current direction is to keep storms off by default.

Only the approved compact additions are persisted: root `stormModel` and `scenarioOne.stormEvents` entries with `year`, `severity`, `directionDegrees` and approved `cropTreesLost`. Victims, deadwood and salvage history use existing authoritative state. No duplicate victim-ID list, per-tree thinning history, per-log bearing or saved calibration profile was added. Validation precedes mutation.

## STORM OCCURRENCE CANDIDATES / SELECTED EVENT MODEL

Compared 1%, 2%, 3% and 5% annual occurrence, with a no-storm control, bounded severities .02/.06/.18 and uniform 1:1:1 or light-biased 4:2:1 weights. Event occurrence is separate from tree vulnerability; zero-victim events remain valid history. No immunity period. Provisional comparison profile: 2%, uniform weights, ReducedProposal(D) and BoundedRational failure transform. This is gameplay calibration, not an empirical Irish hazard rate, and is not an accepted activation/default decision.

## TREE VULNERABILITY / PARAMETER GRADES

Compared the existing diagnostic(A), height/H-D(B), full proposal(C), and reduced proposal(D); compared exponential and bounded-rational failure transforms. The reduced provisional candidate uses tree height, relative local top height, H/D, local light and saved nearby recent opening. Local top is the tallest-20% mean in a cached 3×3-cell neighbourhood. It is not a physical engineering stability prediction or an exact storm probability.

Numeric occurrence/intensity weights, coefficients, relative bands and salvage work choices are [C] game calibration. Published biological context and the readiness hypotheses remain supporting evidence; calibration does not promote them to measured site-specific hazard.

## RECENT OPENING / EDGE FACTOR

Reuses saved cell recentOpening and its decay. Windthrow updates local opening and affects subsequent vulnerability. Same-tree math tests verify recent exposure exceeds recovered exposure. Population/treatment comparisons also account for removed stems and later growth. Site and external-edge factors are neutral 1 because outside-property landscape is not canonically defined. No new directional landscape exposure was invented.

## WINDTHROW OUTCOME / DEADWOOD

One outcome: uprooted/fallen, mortality cause `windthrow`, recorded death year and frozen victim dimensions. Victims stop living/growing and leave live basal area. Exactly one existing deadwood record is created per victim. Nonempty events batch deaths and perform one canopy rebuild and one seed rebuild; zero-victim events do neither. Opening/light consequences feed shared ecology state.

## VISUAL / ASSETS

Reuses existing logs and fresh/weathered root plates. A rotated living Sitka crown remains provisional, limited to 20 crowns within 35 m by a shared distance/ID-ordered budget, using existing LODs. Root/log representations persist at distance and after crown decay. Extracted zero-volume stems cannot recreate a crown during budget refresh. The new small Resources catalog references existing root prefabs; no new binary art family, package or replaced prefab GUID is introduced.

## SALVAGE / SALVAGE COST

Leave deadwood is the default. Aim at an eligible fallen stem and press X to select/cancel optional salvage. Existing Work Plan offers Sell or Keep for selected stems and permits partial salvage. Mixed felling/salvage uses the existing yield, work, reservation and settlement paths and charges the existing contractor minimum once. Timber prices are unchanged.

Compared work multipliers 1.00/1.15/1.25/1.50. Seven- and 112-stem jobs remained at the €2500 minimum. A 763-stem quote-only stress fixture exposes work costs €12600.20/14490.23/15750.25/18900.30 with identical €43315.51 timber output revenue; the whole stress job exceeds starting cash. Provisionally use 1.00: evidence does not establish a premium. Existing WindDamage grading applies a bounded basal damaged section and delay penalty [C]. Original volume is conserved; extracted records remain at zero rather than regaining material through decay.

## REGEN MODEL 2 INTERACTION

Verified windthrow → canopy/opening → light → bramble/bracken competition → juvenile survival/recruitment through existing shared state.120 focused checks cover three species, exact and natural juvenile paths, zero-victim events and competition strengths. Regeneration Model 2 production was not rewritten and receives no direct hard-coded storm mortality effect.

## FORCED-STORM RESULTS

Established and immediately recent-thinning matrices each cover 288 candidate/transform/treatment/timing/severity cases, with 36 selected-profile cases each. Gates pass 672 and 790 checks respectively. Treatments are unthinned, light/distributed, moderate and heavy/opening; timings are early, peak H/D and later developed stands. CSVs retain victim dimensions, volume, basal area, crops, light/opening and regeneration/competition outcomes.

Recent exposure raises a given retained tree's vulnerability, but total victims also depend on population, stature and growth. Heavy opening is not invariably more destructive than moderate thinning. For selected severe established events, unthinned deaths are 25/336,58/269 and 20/138 across the three timings. See VulnerabilityComparison.md for full paired results and their limitations.

## 25-YEAR / 50-YEAR / 100-YEAR

144/144 complete worlds,18 groups ×8 deterministic seeds, reached all 25/50/100 horizons.40 complete cases came from the original interrupted process;104 came from the explicit resume. Partial case data is excluded.

At provisional 2% uniform, managed mean wind victims are 12.75/20.125/25; mean wind Crop Tree losses 1.625/3.25/4.375. Managed canopy means .871761/.834387/.803903; regeneration 34.5689/40.5114/55.4009; living promoted trees 23.875/12/13.125. These are sampled fixed-policy results, not a population guarantee or proof of ecological benefit. Detailed distributions, individual causes and controls are in the evidence CSVs and CalibrationResults.md.

## COMPLETION / ECONOMY

Every managed candidate group completed 8/8 by 25; unmanaged groups 0/8. This is a fixed policy across eight seeds, not an accepted ≥90% win-rate requirement. Completed status is historical: even no-storm runs fail some current forest objectives later. Do not interpret completion as century-long maintenance of every objective.

At 2% uniform, lowest sampled managed cash is €6054.60. At 5% uniform, mean century victims/crop losses rise to 61.375/8.625; at 1%, several century seeds have no storm or little damage. No prices or general forestry economy were rebalanced. Long-run worlds do not perform salvage; the isolated salvage gates verify its costs and settlement separately.

## PERFORMANCE

40 simulation cases PASS, covering actual ID-roll-derived 10/50/100 victims in 336/1300/5000-tree fixtures and 1000 victims in 5000 trees. Bounded rebuild/record behaviour holds.5000-tree annual steps are about 1.33–1.39 s in the final rerun (earlier about 1.65 s); this is not a frame-time acceptance claim.

Rendered 1167-check UI/performance gate uses Unity 6000.6.0 f 1, Vulkan, GTX 980 M,1920×1080 and fixed-camera warmup/sample windows. Developed/century Editor frame medians are about 100 ms with storm visuals enabled or disabled;1300-tree stress is about 240 ms. These are Editor wall/Main Thread measurements, not GPU timers or standalone Player FPS. Wider profiling and standalone measurements are required before claiming smooth century play. See PerformanceResults.md.

## DETERMINISM / LEGACY / REFERENCE

Storm RNG uses versioned ID-keyed domains; shuffled iteration gives identical victims. Separate-process replay reproduces resolved `84796FAEF211823C` and future `52037B943B62DD39`. Saved RNG 0 and RNG 1 gates both pass throughyear 10, including reverse iteration: RNG 0 `3CDFDAB9E065695C` / `D48FE1364778BB0F`; RNG 1 `5699BD84874A3059` / `6CEAC0BB5401BEAD`. These are recorded fixture hashes, not new canonical anchors or a cross-platform promise.

Inert storm fields are removed only by explicit historical-layout comparators for StormModel 0; active storm worlds retain new state and are refused by legacy comparators. Preserve v 18 Model 2 anchors `FA855239CDDA32D8` / `02334804F65C0234`, completion `702766DECE591E21`. The old altered-site fixture leaks cell 0.SiteProductivity=0; untouched main independently reproduces it and clean one-year `8333BAA4126E8A09`. Disclose both meanings; no regeneration rewrite is needed.

Reference Future v 1 archive remains unchanged, storms 0. Regression verifies originalY 100 `7AD177B3CC2F73C7` and continuation `9CDF21A541C5968D`. No frozen Reference alteration was required.

## UI / TUTORIAL HANDOFF

Implemented relative Stable/Watch/Exposed labels with causal text and storms-off explanation, fallen-stem HUD/X selection, Work Plan Sell/Keep, annual damage/salvage/deadwood review and existing map/walking HUD waypoint. Connected runtime gates exercise New Input System M/X/Tab, actual raycasts, repeated selection/cancel, real UI callbacks and 1280/1600/1920 layouts. Both player and marking raycasts now allow explicit windthrow children through the generic habitat filter.

TutorialUIHandoff.md proposes a multi-year lesson family without new lesson IDs or objective changes: notice disturbance, waypoint/inspect, compare opening/light/juveniles, understand Leave, optionally select partial salvage and review minimum/grades. Hardware mouse/accessibility and human playtest acceptance remain separate. No first-year random-storm requirement or compulsory salvage is introduced.

## ASSET GAPS / CANONICAL PROPOSAL

Rotated green living crowns are provisional and can obscure close-up root plates in dense canopy. AssetFollowupPacket.md describes the art/performance acceptance follow-up without authorizing new binaries. RecommendationB: review StormModel 1 as a future scenario option first. Current user direction remains new-game 0. No option-selection screen or automatic activation was added. A configurable saved profile requires separate schema review. CanonicalDecisionProposal.md is a proposal, not a canonical accepted decision.

## REGRESSION / WORKTREE CLEAN / STATUS

Baseline before production changes:1398 Model 2 checks and 24 gates PASS. Final production: math 152, core 107, recruitment 120, salvage 983, performance 40, visual 241, UI 1167, established/recent forced matrices, separate-process replay/RNG, Model 2 and all 24 applicable regression gates PASS. Production source manifests verify stability across focused final runs.

The final Model 2 gate passes 1400 checks with the exact v 18 start/altered-site anchors. The modern Model 2 completion gate preserves v 18 completion `702766DECE591E21`; the legacy Model 1 completion gate preserves v 17 completion `D7C4DDD36B53FCCE`. All six focused final confirmations pass with unchanged production bytes. Evidence/verification_results.json records source hashes, results and the 24-gate regression. Evidence/tested_production_manifest.json matches the committed implementation for all 204 production files.

Worktree clean: yes after the implementation and accompanying handoff commits, verified before reporting. All Editors are closed; disposable gate scripts/metas are removed. Twelve inspected settings/material import changes were backed up to ignored Build storage and restored to original tracked bytes; the crash artifact is retained in ignored Build. Executable-bit comparisons are disabled only for this exFAT worktree. ZX 20 host mount is read/write; the earlier read-only report was the execution sandbox view.

Unrelated dirty `/home/jer/CCF-main` is preserved. No main integration, main push, dependency change, additional unapproved save field or new art family is part of this handoff. Required nine documents and supporting readiness evidence are retained under Docs/Research/WindthrowV 1; verification scripts remain under Tools/Verification/WindthrowV 1 with disposable-copy cleanup.

Status: READY FOR MANAGER INTEGRATION REVIEW. Manager review must cover simulation/save compatibility, calibration/default policy and stated art/performance limits; integration then requires its own post-merge gates.
