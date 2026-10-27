# Current Development Milestone

## Purpose

Define current scope. Verified implementation belongs in Overview; accepted decisions in Decision Log; temporary ownership in live coordination.

## Milestone

**Scenario One — Complete-Loop Three-Species Demo Readiness**

Deliver a first external demo of the **complete** Scenario One in which Sitka spruce, Oak and Beech are represented to comparable modelling depth, and in which the player can run the full management cycle from first assessment to scenario completion (D-059, D-060). P3 Work Plan residual/cash overview is integrated on main at `0ab73994153d55d08adbd65f928926e6e9165f7c`. S1-A Teaching Readiness is integrated and post-integration verified on main at `c01de2dbc89e760c49a9812a80dbed19632c185e`. S1-B Standalone-Build Readiness is integrated as the approved source `4359e500c6b312ebcf957397b4fe53205f3f1ca4` plus the separate presentation correction `fac782072f23e28518e8840c621112fe4074db45` (D-055, unavailable construction frames hidden), and is post-integration verified with one open gate (P2 interactive rendered layout; see the Overview). The 80 × 80 m Scenario One stand expansion (D-056) is integrated and verified on main at `a309f99e0f2c6a770044604bbf88ee2e2471a5b5` (see the Overview). Enlarged80 area-sensitive calibration (D-057) is integrated and post-integration verified on main at `23f84f9ac0d4a02d385869d3985e11a24cc5a23f`. Minimal startup/title screen (D-058) is **DONE**, integrated and Editor-verified on main at `b99ba6993b6b7cfb9fc3b8de6c4eda88abd8a348`. Malformed cell-index save hardening is **DONE**, integrated and verified on main at `12cc8ef0659f54055088047c2c6727d874bdffd8`.

The species-parity audit at base `c1564b857972ee18917e8a461c457533a54cc3f1` is complete and its findings are recorded in the Unity Project Overview. Its accepted outcomes are D-059, D-061 and D-062. **Species parity P0 work has not started.** S1-C and S1-D are not implemented. No external-demo candidate exists: no pilot Windows candidate exists, Player and native-Windows performance have not been measured, and native Windows technical smoke has not been performed.

## In scope

- **Sitka/Oak/Beech parity P0 (D-059, D-061, D-062)** — species-aware density / self-thinning with pure-Sitka preservation proven exact; Oak and Beech adult parameter parity with labelled transfer/ordinal evidence; species-specific regeneration, juvenile and browse traits; species-specific canopy, light and site traits; an Oak/Beech timber assortment and economic layer; player-facing inspection and teaching parity; and integrated multi-species verification.
- **S1-C:** review/history work needed to understand repeated intervention. **PRE-DEMO (D-060).**
- **S1-D:** second and subsequent intervention support needed for the full Scenario One loop. **PRE-DEMO (D-060).**
- **Multi-scale forestry management V1 — representative-management slice (D-065):** after the existing Lesson 5 review, the player's executed sample treatment yields a reviewable prescription that can be applied to comparable rack-served areas through pending ordinary work (new lesson "Extend your treatment"). **REQUIRED BEFORE BEGINNER/EXTERNAL SCENARIO ONE TESTING.** Spec `specs/multiscale-forestry-management-v1/` is approved for implementation planning; production coding is gated by Rack V2 (D-064) persistence being integrated and verified, and **has not started**. It may contribute to S1-D but does not satisfy it until S1-D's own acceptance requirements are verified. Rack V2 and other authorised prerequisite work continue in the meantime.
- Scenario completion reachable and understandable by the player.
- The multi-scale "Extend your treatment" step (D-065) is integrated, its calibration impact on canopy, economy and completion is qualified, and the user has personally playtested it before any beginner/external Scenario One test. Its spec verdict does not itself authorise a Windows build.
- Integrated full Scenario One qualification, followed by a fresh post-parity/full-loop Windows candidate, independent review and native Windows smoke.
- S1-A: teaching readiness for the management cycle — integrated and verified.
- S1-B: standalone-build readiness — integrated and verified (one open gate).
- Scenario One stand expansion to 80 × 80 m (D-056): integrated and verified. New games use Enlarged80; Legacy40 saves and Reference Future v1 are unchanged.
- Enlarged80 area-sensitive calibration (D-057): **DONE**, integrated and verified at `23f84f9ac0d4a02d385869d3985e11a24cc5a23f`. Extensive objective/century targets are 4×; cash, contractor minimum, owner capacity, unit values, canopy means and years remain unchanged. This is gameplay continuity calibration, not empirical forestry thresholds.
- Minimal startup/title screen (D-058): **DONE**, integrated and Editor-verified at `b99ba6993b6b7cfb9fc3b8de6c4eda88abd8a348`; built-Player/native-Windows startup remains UNVERIFIED.
- Malformed cell-index save hardening: **DONE**, integrated and verified at `12cc8ef0659f54055088047c2c6727d874bdffd8`; 131 bounded integration cases PASS, with valid sentinel/shared-cell semantics preserved and save v20 unchanged.
- Bounded task specifications, verification evidence and external-test feedback for each slice. Detailed mechanics remain subject to their own accepted packets.

## Pre-demo sequence

The first external demo is the **complete** Scenario One (D-060). The intended sequence is:

1. **Internal technical qualification** — builds, performance, platform smoke, current-slice reviews.
2. **Sitka/Oak/Beech parity P0** (D-059, D-061, D-062).
3. **S1-C** review/history.
4. **S1-D** second and subsequent intervention support.
5. **Scenario completion** reachable and understood.
6. **Integrated full Scenario One qualification.**
7. **Fresh post-parity/full-loop Windows candidate.**
8. **Independent review + native Windows smoke.**
9. **Beginner external testing of the complete Scenario One.**

**Build candidate policy.** B06 is historical evidence only and must not be sent to testers. **B07 is an internal technical baseline and is not the eventual external-demo candidate** (D-060); it is not started and not built. Any pre-parity or pre-full-loop build is an internal engineering build only: useful for engineering tests, never an external-demo candidate. The external-demo candidate is the fresh post-parity/full-loop Windows candidate built at step 7.

**Carried forward from the accepted 2026-10-10 pre-pilot work:** the Enlarged80 retained former-edge-band residual remains; no stocking reduction is authorised. Reference Future v1 is not resized, regenerated, migrated or reinterpreted. Built-Player performance and native Windows text/input/performance remain unqualified until the corresponding gates pass; synthetic InputSystem graphical delivery remains unverified.

**Superseded:** the previous "Stage A — first-cycle pilot" external boundary, and the statement that "S1-C and S1-D are not prerequisites for the Stage-A first-cycle pilot", are superseded by D-060. There is no longer a first-intervention external pilot as the intended Scenario One demo boundary.

## Completion checks

- Before the first external demo, the player can start from the initial forest, assess it, make an intervention, advance time, observe consequences, understand and review previous management, make second and later interventions, repeat the management cycle, and reach Scenario One completion (D-060).
- By scenario completion the player has an opportunity to learn the basic CCF management loop through repeated intervention and adaptation.
- Sitka spruce, Oak and Beech are represented to comparable modelling depth before that demo (D-059). Comparable depth does not mean equal empirical precision; evidence quality may differ between species where labelled honestly.
- Oak and Beech do not silently inherit materially inappropriate Sitka-specific assumptions (D-059). No wording or model treats Oak or Beech as Sitka under another name.
- Mixed-stand density uses ordinal species-aware growing-space-demand classes (D-061). 100 % Sitka composition routes through the existing exact calibrated Sitka density implementation and existing Sitka arithmetic is unchanged. No DEC, no DMC, no mixture-productivity bonus. Sitka anchors are not silently re-baselined.
- Oak and Beech have an appropriate timber assortment and economic layer (D-062) that does not simply reuse Sitka assortment boundaries or prices, and that labels empirical evidence, transfer and calibration distinctly.
- Every species-sensitive mechanism uses species-specific behaviour where evidence shows material differences; where quantitative evidence is insufficient, the representation is ordinal/transfer/calibrated and **labelled accurately**. No pseudo-empirical coefficients.
- Integrated full Scenario One qualification is green, followed by a fresh post-parity/full-loop Windows candidate, independent review and a native Windows technical smoke, before beginner external testing.
- Each result identifies the committed implementation and the tested slice; research proposals and future infrastructure are not presented as shipped features.

## Context and setup follow-ups

The repository migration/generator is integrated. Permanent Drive context and Game Dev project instructions/attachments were refreshed after Storm integration and the subsequent canonical consistency cleanup. The post-80 m refresh regenerates the current canonical set from its context commit; its Drive publication and read-back status are reported in the integration handoff delivered to the Manager, and Drive/attachments are not claimed current by this document. Independent Unity smoke checks in worker worktrees remain open and are not completed by documentation publication.

## Live coordination

`Docs/Project/coordination/active-tasks.md` remains live coordination outside the context snapshot. Use exact base/context commits and one writer per worktree.

## Spec-driven work

Substantial or ambiguous work may need an approved spec under `specs/` before implementation (AgentWorkflow). When a milestone item has one, reference its Spec ID and `specs/<feature>/spec.md`. A draft or a separate design conversation is not an approved spec. At the time of this update, no feature spec is recorded as `APPROVED` under `specs/`.

## Explicit exclusions

No Scenario Two, multi-scenario infrastructure, sandbox or Stage 2 implementation in this milestone refresh. No new save/model/economy parameters or research-proposed exact curriculum/mechanics are accepted by this documentation change. D-059–D-062 accept scope, direction and modelling constraints only; concrete coefficients, model versions, save fields and assortment/price values each require their own accepted implementation packet. Fruit/nut systems, performance optimisation work, pilot distribution, mixture productivity bonuses, invented broadleaf yield equations, Reference Future modification and stocking reduction remain out of scope.

## Historical Scenario 1 functional baseline (2026-10-04)

**FUNCTIONALLY COMPLETE; final presentation acceptance PASS (2026-10-04).** The functional and presentation baseline is integrated. Simulation follow-ups remain open (see below); this is not a permanent closure of Scenario 1 simulation work.

Integrated on `main` and verified (see the Unity Project Overview, **Integrated Scenario 1**):
- functional implementation, save schema v15 (v16 from D-047; v17 from D-048);
- browsing default 0.2;
- shelters;
- contractor/landowner execution;
- timber yield and economy;
- completion verification;
- Plantation02 Sitka presentation, corrected forest-floor presentation and compact brash presentation.

Acceptance record:
- the automated completion gate passes;
- the integration regression passed 27/27;
- post-merge gates on `main` pass;
- the user's interactive Scenario 1 smoke passed, with the starting-stand and compact-brash corrections accepted in play;
- no performance problem was reported during that smoke.

Final presentation acceptance (`Docs/Scenario1FinalPresentationAcceptance.md`, merged at `ec938b0`):
- the ten-state rendered review passed: start, marking, pruning, thinning, dispositions, planting, protection, regeneration, later management, save/Reference;
- track and work-clearing now use the existing soil texture;
- inspection/HUD overlap and marking-summary/prompt clipping are fixed;
- Astra ForestFloorV1 was reviewed (manifest verified) and **not adopted**: no demonstrated ground gap;
- dry brash was accepted as subtle but understandable. Evidence limitation: the final review camera was occluded, so acceptance relies on an earlier close-range render;
- performance was acceptable on the observed hardware (editor, GTX 980M: about 20–30 ms median frame). This is not a formal performance certification.

Outstanding simulation decision: the post-Scenario-1 readiness study (`task/post-scenario1-systems-readiness` @ `7f58618`, **not merged**) found that, under the default RNG model 0, planted-juvenile survival and browse rolls are autocorrelated. Shaded planted stock survives far longer than the shared probability intends. Resolved by D-046 (2026-10-05): new Scenario One games use RNG model 1; saves keep their recorded model; saves without the field and Reference Future v1 stay on model 0. Integrated on main at `4eab590`; post-merge gates PASS. Under model 1 the scenario completes in Year 25, stays solvent (minimum cash €5,897.70) and no required objective is newly blocked. **Active follow-on task:** Scenario 1 UI Toolkit redesign (`task/scenario-one-ui-redesign`, not to be merged without Manager/user review).

Deferred, outside the Scenario 1 minimum unless accepted elsewhere:
- deer fencing as a production feature;
- richer production understorey beyond the accepted Scenario One Model 2 bramble/bracken juvenile-survival competition;
- vegetation-control simulation expansion;
- enabling storms for new Scenario One games (dormant core accepted in D-050);
- adult suppression mortality;
- browse-history/form-damage persistence;
- fence deterioration;
- pruning-quality premium;
- broadleaf market;
- manual forestry;
- co-op.

*This deferral list is the 2026-10-04 record. **"Broadleaf market" is reopened by D-062** and is now pre-demo scope. The remaining entries are unchanged unless separately accepted.*

Previously recorded presentation constraint: ring-barked, windthrow-root and defect assets stay unspawned until matching simulation state exists (D-020).

Remaining presentation polish (not blockers):
- moss cushions on the track edge;
- deadwood volume is shown in objectives and history rather than as its own annual-review line.

## Regeneration Model 1 (2026-10-06)

Integrated on main at `4670e73` (D-047), fast-forward from `402a2b4`. Save schema v16. New Scenario One games use regeneration model 1 (age bands with a sub-threshold accumulator; no seed-independent infill; corrected capacity, origin and promotion representation). Legacy saves and Reference Future v1 stay model 0, which is byte-identical to before.

Scenario 1 under model 1 remains achievable and viable: completed in Year 25 with all 8 objectives (regeneration 51 cells against 3), lowest cash €5,897.73; planting, clearance, shelters and economy gates pass. No objective or economy recalibration.

Open/deferred from this work:
- physical juvenile stems/ha interpretation (undefined);
- model1 retains non-causal habitat cover; new-game Model2 adds separate bramble/bracken survival competition (D-049, below);
- the 0.01 establishment-response cut-off remains an existing calibration rule;
- promotion remains an abstract band → individual handoff;
- harness red gates (Clearance, MenuTutorial, Removal) reproduced identically on `402a2b4`; reclassified in D-048 / pedagogy Workstream F as harness issues, not production failures (see Growth Model 1 below).

## Growth Model 1 (2026-10-07)

Integrated on main at `2cfbabc` (D-048), fast-forward from `3e4ee40`. Save schema v17.

**New Scenario One model stack:** RNG model 1, regeneration model 1, growth model 1, save v17. Legacy saves keep their stored/versioned behaviours; Reference Future v1 stays frozen legacy behaviour (growth 0).

**Growth model 1:**
- **Site:** Irish site Class III; Sitka height follows the Class III site-potential curve. 20.4 m at age 30 is the published Irish anchor [A]; other ages are modelled curve values.
- **Competition:** DBH and local Hegyi competition are unchanged. A separate British-Sitka SDI occupancy signal [B] drives self-thinning toward suppressed trees ([C] parameters).
- **Deadwood:** self-thinning deaths become existing fallen deadwood.

**Simulation validation outputs** (not empirical targets), unthinned: 1,706 / 900 / 538 / 344 / 256 stems/ha at ages 40 / 60 / 80 / 100 / 120; basal area 75.9 → 83.8 m²/ha; RD 0.83 → 0.97–0.99 (legacy growth: 416 m²/ha at 120).

**Scenario 1 under the new stack:** completed in Year 25 with all 8 objectives; lowest cash €6,139.75. No economy or objective changes.

**Harness gate status:**
- **Removal:** stale harness contract (U now plans the accepted species-less area clearance). The corrected harness on `task/scenario-one-pedagogy-overnight` @ `60674f1` passes in batch on the integrated code.
- **Clearance:** stale first-use-help assumption plus an interactive-input requirement. The corrected harness passes in an interactive Editor.
- **MenuTutorial:** interactive-input requirement; passes in an interactive Editor.
- The harness fixes are to be ported in the pedagogy P0 task. None is a production failure.

**Open calibration items:**
- realised top height 6–9 % below the Class III curve at older ages;
- mortality onset/strength/exponent ([C]);
- 0.5 stem form factor unvalidated;
- Forest Yield cross-validation;
- Irish maximum-density data;
- recruit mortality pulses under a full canopy.

## Regeneration Bottlenecks v1

Browsing & Protection v1 (shared browsing response, accepted shelter protection, deterministic restore) shipped as part of Scenario 1. Scenario One Regeneration Model 2 now supplies the accepted bramble/bracken juvenile-survival competition (D-049); the other candidate elements remain queued direction, not active, and each needs its own accepted packet:
- richer understorey beyond the accepted Scenario One Model 2 bramble/bracken juvenile-survival competition;
- competition/concealment;
- spot control/regrowth.

## Regeneration Model 2 (2026-10-08)

The accepted Scenario One Model 2 v1 definition (D-049) is part of current main `89a5f34d8ab54d51daaebf84ed55a5397ac07406`. Current Save 19 adds the dormant D-050 storm fields; new games use RNG 1/regeneration 2/growth 1/storm 0. Independent bramble/bracken competitor state applies shared linear juvenile-survival pressure, with separately accounted vegetation/light/browse loss. The accepted values and evidence grades are D-049; they are gameplay abstractions, not measured Irish constants.

Clearance has useful, wasted and harmful outcomes, costs real cash and can remove existing juveniles. Do not make it compulsory or teach a repeat interval. The learning-panel copy follows the active saved model, preserving legacy truth. Biological heights are unchanged; the Beech renderer fix is retained.

Open calibration/playtest items: botanical type differences, bramble concealment, competitive graminoids, source/neighbor spread, stronger site/moisture calibration, target generalization beyond Scenario One, visual recognition and human clearance decisions. These do not block this accepted v1 integration. See `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md` for its original verification. After Storm integration, D-050, Drive mirrors and Game Dev project instructions/attachments were republished at context `58d6b0b483198abcf0208713d9a759f89d90e318`; worker-worktree smoke gates remain open.

## Storms & Windthrow Model 1 — dormant integration (2026-10-08)

Manager approved source `857150b2f20afa52d7b36be88a835f07a4ee5a51` on current main `1fbefd8a40c69dd90abb83f2149e24b546a184cf`; the complete two-commit branch was fast-forwarded into the clean ZX20 integration checkout and verified. Save 19; new-game RNG 1/regeneration 2/growth 1/storm 0. Existing/missing-field saves and frozen Reference stay storm 0. D-050 fixes the accepted Model1 profile and schema; no activation/default change is implied.

Model1 supports one fallen/uprooted outcome, deterministic mortality/deadwood/opening, optional partial salvage through the existing work/economy path and shared Model2 causality. Existing causal inspection, salvage/work plan and disturbance-review UI is retained; no new tutorial objectives are introduced.

The profile (.02 occurrence; .02/.06/.18 uniform severity; ReducedProposal; BoundedRational; neutral site/external edge) is frozen gameplay calibration [C]. Future material changes need StormModel 2 or an approved persisted-profile/version architecture.

Open follow-ups: crown/root visual quality, combined developed/century deadwood and windthrow rendering, standalone Player profiling, final storm activation policy, full lesson/progression, directional outside-landscape exposure and snapped stems/snags. Current Editor measurements permit dormant integration only. Before default activation require standalone/developed/century/combined profiling and acceptable crown/root behaviour; no retrospective FPS target is introduced.

Claude P2/P3 work touching TreeInspectionView, WorkPlanView, AnnualReviewView, ScenarioOneUiRoot or StandMapView must rebase/port onto the newly published main before final verification; conflicts are outside this integration. Exact source, regression, compatibility, context/publication and follow-up identities are in `Docs/Verification/StormsWindthrowIntegration/IntegrationHandoff.md`. Dirty `/home/jer/CCF-main` remains preserved.
