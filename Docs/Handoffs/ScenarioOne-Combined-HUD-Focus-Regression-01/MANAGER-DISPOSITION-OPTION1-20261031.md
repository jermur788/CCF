# Scenario One Combined HUD / Focus Regression Repair 01 — Manager Disposition — 2026-10-31

## Decision

**OPTION 1 APPROVED — BOUNDED PRODUCTION LAYOUT REPAIR**

The accepted/protected contracts are to be made compatible in production. Do not weaken or supersede the Combined minimap/focus assertions merely to fit the current implementation, and do not delete or materially shorten the accepted PARITY-07 estimate content merely to make space.

## Evidence reviewed

Partial branch: `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a`, exact start `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

The partial work changed only the Combined verification fixture so it no longer tries to Crop-mark Rack V2-owned trees. That correction is accepted as a harness fixture repair; no production `Assets/` file has yet changed.

Observed post-fixture gates:

- `combined-v2`: **453 pass / 10 fail / 24 captures**;
- `combined-legacy`: **440 pass / 8 fail / 24 captures**.

The prior V2-only 1920 false failures `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN` are gone, confirming the fixture diagnosis. Once the competitor panel actually renders, a real 1920 clipping defect is exposed: `COMPETITOR_LIST_IS_NOT_CLIPPED` with `lastLabelBottom=915`, `panelBottom=896`.

The remaining estimate/minimap and FOCUS failures are production-layout behavior. Repository evidence shows PARITY-07 deliberately hid the minimap for ordinary estimate inspection because the taller estimate copy did not fit in the previous layout. No later user/Decision Log authority was found that supersedes the protected Combined rule that ordinary estimate inspection keeps the minimap available and that the minimap steps aside for the competitor list.

## Binding repair direction

The local continuation must satisfy **both** accepted/protected contracts:

1. Restore minimap visibility for ordinary estimate inspection.
2. Preserve the accepted PARITY-07 estimate information/copy unless a later direct user or Decision Log instruction explicitly changes it.
3. Preserve PARITY-07 central-prompt and detail-column clearance requirements.
4. Keep the protected Combined minimap/focus assertions semantically unchanged.
5. Repair the exposed 1920 V2 competitor-list clipping in the same bounded UI/presentation pass where practical because it shares the inspection/HUD layout surface.
6. Prefer responsive/reflow/layout changes over copy deletion, assertion relaxation or arbitrary threshold changes.
7. Preserve model-0/Legacy behavior as well as Rack V2 behavior.

Do not choose:

- shortening/condensing accepted estimate copy as the primary solution;
- formally superseding the Combined minimap assertions merely because the current implementation hides the minimap;
- ending inspection in the harness solely to bypass the real close-panel-while-inspecting focus case;
- deferring the known regression past Tier-3 while the protected contract remains applicable.

## Hard exclusions

No Rack geometry/marking/persistence change. No MFM prescription/coverage/candidate/proposal behavior change. No save/schema/anchor/reference change. No ecology/economy change. No test weakening. No Windows build.

## Required verification

The production continuation must run at least:

- full `combined-v2` at 1280x720 and 1920x1080;
- full `combined-legacy` at 1280x720 and 1920x1080;
- PARITY-07 rendered/UI qualification sufficient to prove estimate/detail/prompt clearance remains intact;
- focused minimap/inspection/focus regressions covering `ESTIMATE_MINIMAP_STAYS_VISIBLE`, estimate-vs-minimap geometry, and FOCUS_04/06/07;
- the competitor-list clipping assertion at 1920 V2;
- proportionate guided/Rack regression if the modified layout surface is shared with those flows;
- independent review by a provider/thread independent of the production implementer.

If no layout can satisfy both accepted contracts without a genuine product decision, stop and return the smallest concrete conflict. Do not weaken tests to obtain green.

## Sequencing / repin

MFM-08 integration #205 is the next critical-path local task and owns overlapping UI integration risk first. Therefore #199 production continuation is **Manager-authorised but must not start from the stale `ce673714...` branch while #205 is active**.

After #205 is Manager-accepted, #200 must repin #199 to the exact combined integration SHA. The existing fixture correction may be replayed if still applicable, but the old partial branch must not be blindly merged into the new lineage.

This preserves one writer per UI surface and avoids repairing the HUD on a lineage that is immediately superseded by MFM-08 integration.

No Windows build is authorised.
