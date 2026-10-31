# Scenario One Combined HUD / Focus Regression Repair 01 — Option 1 Continuation Packet

**Claim:** #199  
**Parent:** #91 / browser #200/#202 / local #201/#203  
**Decision authority:** `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`

## Status

**MANAGER-AUTHORISED PRODUCTION REPAIR — START BLOCKED ON POST-MFM-08 REPIN**

Do not begin production edits from the stale `ce673714...` branch while MFM-08 integration #205 is active. After #205 is Manager-accepted, #200 will repin this packet/claim to the exact combined integration SHA. The already-proven fixture correction may be replayed if still applicable; do not blindly merge the stale partial branch.

## Accepted partial result

Partial branch: `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a`.

Accepted verification-fixture correction:

- `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` avoids Rack V2-owned trees when selecting the competitor fixture;
- it removed the false 1920 V2 `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN` failures;
- no protected assertion was changed;
- no production file changed.

Observed remaining gates:

- combined-v2: 453 pass / 10 fail / 24 captures;
- combined-legacy: 440 pass / 8 fail / 24 captures.

## Required production repair

On the repinned post-#205 integration candidate:

1. Restore minimap visibility for ordinary estimate inspection.
2. Preserve the accepted PARITY-07 estimate information/copy.
3. Preserve PARITY-07 central-prompt and detail-column clearance requirements.
4. Preserve the protected Combined minimap/focus assertions exactly in meaning.
5. Repair the real 1920 V2 competitor-list clipping (`lastLabelBottom=915`, `panelBottom=896` in the partial-run evidence) if still reproduced.
6. Prefer responsive/reflow/layout changes over copy deletion or assertion/threshold relaxation.
7. Preserve Rack V2 and model-0/Legacy compatibility.

Do not solve FOCUS_04/06/07 by ending inspection only in the harness; the close-panel-while-inspecting case is a real protected interaction state.

## Allowed production surface

Only the smallest Scenario One HUD/inspection/minimap presentation/focus files required by root-cause evidence after repin. Inspect current post-MFM-08 file ownership before editing.

No Rack geometry/marking/persistence, MFM behavior, save/schema/anchors/reference, ecology or economy changes.

## Required local sequence after repin

1. Refresh live `active-tasks.md`, #199/#205/#203 and the exact integration ref.
2. Mark #199 `[RUNNING]` with actual provider/model, dedicated branch/worktree and exact start SHA.
3. Reproduce the remaining failure families on the repinned integration before production repair unless the existing evidence is directly reusable and the relevant UI code is unchanged.
4. Root-cause the layout in the actual combined state.
5. Make the minimum responsive/reflow production change satisfying both contracts.
6. Run full combined-v2 and combined-legacy at 1280x720 and 1920x1080.
7. Run PARITY-07 rendered/UI qualification for estimate/detail/prompt clearance.
8. Run focused minimap/inspection/focus and competitor clipping checks.
9. Run proportionate guided/Rack regression if the modified surface overlaps those flows.
10. Obtain independent review by a provider/thread independent of the production implementer.
11. Return worktree, Editor and Unity lock clean. No Windows build.

## Stop condition

If both accepted contracts cannot be satisfied without changing product copy/semantics or protected assertions, return the smallest concrete conflict to #200. Do not weaken tests or silently choose a product trade-off.

## Handoff

Update/publish:

`Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md`

with exact repinned base, production diff, before/after gate counts, PARITY-07 evidence, independent review, cleanup state and residuals.

Success token remains:

`SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`

## Difficulty / routing

**Difficulty: Medium/High.** This is production responsive-layout/focus work across protected contracts.

**Best-suited provider:** Claude Sonnet 5.5 Medium is appropriate for the production repair because the prior partial worker already established the cross-contract layout conflict; DeepSeek remains the orchestrator and may handle mechanical preparation/verification where capable. Escalate to Opus only after a classified Sonnet capability/reasoning failure or a genuinely high-consequence unresolved architecture conflict.

## Dispatch after repin

`Resume #199 Scenario One Combined HUD / Focus Regression Repair using the repinned Option 1 packet. Preserve both the protected Combined minimap/focus contract and accepted PARITY-07 estimate/prompt/detail requirements; restore ordinary-inspection minimap visibility and fix the reproduced 1920 competitor clipping through bounded responsive/reflow production layout changes. Do not shorten accepted copy or weaken assertions merely to get green. Run both combined modes at both resolutions, PARITY-07, affected focus/minimap checks and proportionate Rack/guided regressions, obtain independent review, publish the handoff, and return the exact success token or smallest contract blocker. No Windows build.`
