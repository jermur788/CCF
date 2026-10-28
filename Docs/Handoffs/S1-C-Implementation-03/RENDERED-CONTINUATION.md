# S1-C T-03 — Rendered Tier-2 continuation packet

**Task:** `S1-C-IMPL-03 / spec T-03` — intervention drill-down and money honesty  
**Claim:** reuse #19; do not create a second writer or duplicate claim  
**Manager disposition:** `Docs/Handoffs/S1-C-Implementation-03/MANAGER-DISPOSITION.md`  
**Worker branch:** `feature/s1-c-history-drilldown-v1`  
**Worktree:** `/media/jer/ZX20/CCF-S1C-Impl03`  
**Accepted implementation SHA:** `859f74e2c07c87d7f166f58982d914d72a29b34f`  
**Published worker tip before continuation:** `cf92b9c34976b796a17ca3cd0f12ffa343524858`  
**Mode:** Tier-2 Unity qualification plus only bounded repair required by that qualification  
**Windows build:** prohibited

## Start condition

Do **not** start Unity merely because #59 releases. Manager queue authority requires:

1. #59 — RACK-V2-08 Layout Repair is Manager-approved and integrated;
2. #49 — RACK-V2-08 removal of competing first-person rack planning has resumed and completed its required qualification/integration, unless the Manager explicitly reroutes the Unity lane;
3. live `Docs/Project/coordination/active-tasks.md` and claim #19 show this continuation is released;
4. no other owner holds the machine-wide Unity lock.

When released, refresh current `main`, claim #19, live coordination, #59 and #49. Continue in the **same branch/worktree** unless the Manager explicitly assigns a different lane. Mark #19 `[RUNNING]` again with actual model/provider, branch/worktree, current main SHA, exact candidate SHA and Unity lane before any Unity execution.

## Candidate preparation

Bring current `main` into the existing branch without losing the accepted T-03 production bytes. Main may have moved through docs/coordination/Rack work. Before Unity:

- reconcile normally; preserve unrelated work;
- prove the accepted T-03 production diff remains limited to `AnnualReviewView.cs`, additive `ScenarioManagementHistory.cs`, and T-03 verification files unless a Manager-approved qualification repair is needed;
- preserve `ScenarioOneManager.CycleState` and S1-D T-02 production bytes;
- record the exact candidate SHA;
- acquire the machine-wide Unity lock normally.

If current main introduces a real production conflict in the T-03 owned files, stop before resolving it and return `S1-C T-03 INTEGRATION CONFLICT — MANAGER REQUIRED`.

## Required Unity / rendered Tier-2 gates

Run on the exact candidate:

1. **Import / compile**
   - clean Unity import/compile;
   - zero C# compile errors attributable to the candidate;
   - confirm the integrated S1-D T-02 script/meta import successfully as part of the same Editor session.

2. **T-03 fixture and interaction**
   Use a disposable verification fixture following the existing T-02 harness pattern and exercise real manager APIs where practical. Cover at minimum:
   - grouped felling year, with multiple harvest orders and one grouped settlement;
   - planting year with landowner and contractor execution where available;
   - cancelled order represented only by its cancellation event;
   - failed order with recorded failure reason and one missing-reason case;
   - no-work year;
   - older-year browsing.

3. **Behaviour assertions**
   - `work-detail-toggle-*` and `work-detail-*` controls exist and open/close correctly;
   - switching years and returning restores that year's open/closed presentation state; reopening the review clears presentation state as intended;
   - exactly one `annual-harvest-settlement` appears in a successful harvest year;
   - no harvest item row presents per-order/tree money;
   - successful non-harvest task rows may show their own recorded contractor/material/owner-time values;
   - failed/cancelled chips and reasons are legible and correctly attributed;
   - `not recorded` appears only when the record genuinely lacks the field;
   - older-year drill-down does not alter world state, save state, event/report counts or acknowledgement state;
   - latest Annual Review acknowledgement gate remains exactly T-02 semantics.

4. **T-02 regression**
   Re-run the protected S1-C T-02 Annual Review year-browsing/acknowledgement gate and its review comparison tooling. Expected textual/rendered differences are limited to the T-03 additions: detail toggles, grouped settlement block, cancellation presentation and the grouped-money note. Any other protected behaviour movement is a failed gate; do not rebaseline.

5. **Rendered inspection**
   Capture and inspect **1280×720** and **1920×1080** representative states for:
   - grouped-harvest collapsed and expanded;
   - planting expanded;
   - cancelled expanded;
   - failed expanded.

   Check clipping, wrapping, scroll usability, hierarchy, chips, money wording and whether a representative large work year becomes materially unreadable. Do not add pagination/capping unless the rendered evidence demonstrates a real problem and Manager approves the repair.

6. **Offline preservation**
   Re-run:
   - T-03 offline suite (expected 72 PASS unless a deliberately justified additive assertion increases the count);
   - S1-C history query suite (55 baseline);
   - S1-D T-02 cycle-state suite (93 baseline).

No protected test may be weakened or silently rebaselined.

## Repair authority during qualification

A small repair is authorised only when the Unity/rendered gate exposes a defect directly caused by T-03 and the repair stays inside:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`;
- the additive T-03 portion of `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs`;
- T-03 verification tooling.

Do **not** edit save/schema, objectives, ecology/economy, Rack V2, S1-D, MFM, scenes/prefabs, species assets, ProjectSettings/packages, Reference Future v1 or T-04+ scope. If a repair would need those surfaces or change a product judgement above, stop for Manager review.

## Handoff

Update/publish `Docs/Handoffs/S1-C-Implementation-03/HANDOFF.md` with the continuation evidence, including:

- actual model/provider and controlled model transitions if any;
- refreshed main SHA, exact candidate SHA and ending branch tip;
- Unity lock acquire/release evidence;
- import/compile result;
- exact fixture/gate outputs;
- T-02 regression result;
- capture list and inspection findings at both resolutions;
- any repair diff and why it was necessary;
- final repo/worktree cleanliness and unrelated-work preservation.

On success mark claim #19 `[HANDED-OFF]`, close it, and return:

`S1-C INTERVENTION DRILL-DOWN IMPLEMENTED / QUALIFIED — READY FOR MANAGER REVIEW`

If Unity ownership is still active when released incorrectly, do not bypass it; return:

`S1-C T-03 RENDERED QUALIFICATION WAITING — UNITY OWNER ACTIVE`

**DASHBOARD SYNC:** NOT AVAILABLE  
**Published dashboard reflects this task:** NO
