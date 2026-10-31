# Scenario One Combined HUD / Focus Regression Repair 01 — Task Packet

Claim: #199

## Purpose
Root-cause and minimally repair the stable current-lineage Combined HUD/focus regressions while preserving the protected gate and both Rack V2/model-0 modes.

## Exact start
`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

## Authority
Parent diagnosis: `Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/HANDOFF.md`.

Accepted historical protected-gate baseline after RACK-V2-08 layout repair on `0904f0f6bb23b55758e3e61724f89aba5ee58ef9`:
- Combined HUD model-0 450/0;
- Combined HUD Rack V2 455/0.

#195 current failures:
- V2: `ESTIMATE_MINIMAP_STAYS_VISIBLE`; 1280 `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP`; 1920 `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN`; `FOCUS_04/06/07` both sizes.
- legacy: `ESTIMATE_MINIMAP_STAYS_VISIBLE`; `FOCUS_04/06/07` both sizes.

The set is stable across `cfdc93bf`, MFM candidate `e5fb4246`, and current integration, so it is not MFM-specific and not treated as a transient run failure.

## Scope
Bounded production UI/focus repair after root-cause evidence. Small verification instrumentation is allowed, but protected assertions remain semantically unchanged.

Forbidden: Rack geometry/marking/persistence changes; MFM changes; schema/anchors/ecology/economy; deleting/skipping/rebaselining failing assertions; threshold relaxation merely to obtain green; Windows build.

If evidence proves an assertion was superseded by a later accepted product contract, stop for Manager disposition instead of editing that assertion.

## Execution
- Difficulty: Medium/High.
- Preferred: strong local Unity/code worker; independent review required.
- Branch: `fix/scenario-one-combined-hud-focus-regression-v1`.
- Dedicated worktree only after live preflight.
- Unity serialized; fresh storage/process/lock checks and normal acquisition required.
- Before edits: refresh active tasks/#199/exact start/ownership; post `[RUNNING]` with provider/model/branch/worktree/start SHA.

## Required work
1. Inspect diff/history from last qualified Combined baseline `0904f0f...` to current exact start and identify likely responsible UI/focus changes.
2. Reproduce exact current failures if existing #195 receipts are not directly consumable.
3. Root-cause separately: estimate/minimap, competitor panel/tags, focus 04/06/07.
4. Make minimum production repair.
5. Run full `combined-v2` and `combined-legacy` at 1280×720 and 1920×1080 with all sections enabled.
6. Run proportionate affected regression gates covering minimap, inspection/competitor presentation, focus ownership and Rack/model-0 compatibility.
7. Obtain independent review of the production diff and evidence.
8. Return Editor/worktree/lock clean.

## Handoff
`Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md`

Success token: `SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`

Copy/paste:
`Run #199 from Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/TASK-PACKET.md starting at exact ce6737146a7aa9ea953d7db0ca93b103f6c2c996. Preserve the protected Combined HUD assertions. Root-cause and minimally repair the stable estimate/minimap, competitor panel/tags and FOCUS_04/06/07 failures in Rack V2 and legacy modes. Do not attribute them to MFM-08 or weaken/rebaseline the tests. Fresh-preflight/acquire Unity, run both complete combined gates at 720p/1080p plus affected regressions, publish HANDOFF.md, and return the exact success token or blocker. No Windows build.`
