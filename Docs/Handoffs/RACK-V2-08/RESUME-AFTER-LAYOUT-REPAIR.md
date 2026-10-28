# RACK-V2-08 — Resume after Combined HUD layout repair

**Task:** RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative  
**Task claim:** #49  
**Existing branch:** `feature/scenario-one-rack-v2-no-competing-author-v1`  
**Existing worktree:** `/media/jer/ZX20/CCF-RackV2-08`  
**Layout-repair integration:** PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1`  
**Layout-repair Manager review:** `Docs/Handoffs/RACK-V2-08-Layout-Repair-01/MANAGER-REVIEW.md`

## Resume authority

The Combined HUD layout blocker is resolved and integrated. Claim #49 may resume on its existing branch/worktree.

Before any production edit or Unity execution:

1. refresh current `main`, claim #49 and live `Docs/Project/coordination/active-tasks.md`;
2. confirm no other writer owns the RACK-V2-08 files or the machine-wide Unity lock;
3. bring current `main` containing PR #80 into the existing RACK-V2-08 branch without discarding or rewriting the already-implemented RACK-V2-08 work;
4. inspect overlap in `ScenarioOneUiRoot.cs`, `StandMapView.cs` and verification harnesses before resolving any conflict;
5. under bounded assigned-integrator authority, update **only the RACK-V2-08 row** in `active-tasks.md` to actual executor/model/provider, branch/worktree, exact resume HEAD and `[RUNNING]`; do not alter unrelated rows;
6. mark claim #49 `[RUNNING]` with the same identity and exact starting/resume SHA before Unity or new production edits.

Preserve unrelated dirty work. Do not reuse the released layout-repair worktree.

## Manager disposition carried forward

The layout repair is accepted. Its packet item 7 was satisfied by bounded smoke evidence plus direct unaffected-path analysis. This is **not** a waiver for RACK-V2-08's own Guided/Sequential qualification.

The layout-repair model-0 guided attempt recorded 396 PASS including 288 layout checks, then seven failures in the real-OS-input chain after relative mouse movement failed to rotate the camera. Those seven assertions remain unclaimed. RACK-V2-08 must exercise its required model-0 and V2 Guided/Sequential paths on the repaired integration candidate and either pass them or return a precise blocker.

## Required qualification after bringing in repaired main

Use the original RACK-V2-08 packet, G3 disposition and coordination amendment as authority. At minimum rerun on the **exact post-merge RACK-V2-08 candidate**:

1. Combined HUD — model-0/Legacy40-compatible branch, including both required resolutions;
2. Combined HUD — Rack V2 branch, including both required resolutions;
3. Guided/Sequential — model-0/Legacy40-compatible branch;
4. Guided/Sequential — Rack V2 branch;
5. Legacy40/model-0 compatibility preservation;
6. RACK-V2-07 confirm/reopen/minimap preservation;
7. proportionate Stand Map and static/offline preservation checks affected by the merge.

Do not weaken assertions, rebaseline hashes or treat the layout-repair Manager exception as proof of these feature gates.

## Scope remains unchanged

Remove/disable competing first-person rack-authoring routes for Rack V2/Enlarged80 while preserving:

- Stand Map as authoritative Rack V2 planning;
- Legacy40/model-0 first-person compatibility;
- ordinary Fell/Crop marking;
- later world interaction needed for field marking;
- OPEN-3 lifecycle rules and frozen Reference Future v1 compatibility.

Do not absorb RACK-V2-09 field marking, RACK-V2-10 Help expansion, RACK-V2-11 save/schema, MFM, S1-C/S1-D, ecology/economy or unrelated UI work.

No Windows/Player build.

## Handoff

Update the existing canonical handoff:

`Docs/Handoffs/RACK-V2-08/HANDOFF.md`

At successful publication, mark claim #49 `[HANDED-OFF]`, link the handoff, close the claim and return:

`NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`

If a protected gate or merge conflict requires product/test-policy disposition, stop with a precise blocker and leave the claim open.
