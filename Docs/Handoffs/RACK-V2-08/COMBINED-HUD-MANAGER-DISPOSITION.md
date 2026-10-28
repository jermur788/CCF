# RACK-V2-08 — Manager disposition: pre-existing Combined HUD blocker

**Date:** 2026-10-28

**Verdict:** `BLOCKED BY PRE-EXISTING MAINLINE LAYOUT DEFECT — SEPARATE REPAIR REQUIRED`

RACK-V2-08 is not rejected. Its owned implementation and owned qualification evidence are provisionally acceptable, but the feature cannot be marked ready or integrated until the protected Combined HUD gate is green.

## Evidence accepted

The RACK-V2-08 handoff at branch tip `5b0ed56` reports:

- exact Unity candidate `7dd54ee679729da07013c3e129d8485c6067495f` with `Assets/` byte-identical to production commit `6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5`;
- Legacy40 first-person rack flow PASS (15 checks);
- model-0 Guided/Sequential PASS (600 / 0);
- Rack V2 Guided/Sequential PASS (553 / 0);
- unchanged RACK-V2-07 confirm/reopen/minimap gate PASS (213 checks);
- protected assertion inventory preserved;
- Combined HUD failures in both model branches;
- the same relevant failures reproduced on unmodified main `8f8e6f03c65894b0d933aad23a4ad69227fb33e8` in Stand Map/minimap layout code outside the RACK-V2-08 production diff.

The baseline reproduction is sufficient to classify the Combined HUD failures as pre-existing mainline layout defects, not as evidence that RACK-V2-08 caused the regressions.

## G3 / measurement-integrity disposition

The Combined HUD assertions remain valid product measurements. They must **not** be weakened, skipped, rebaselined or made conditional merely to allow RACK-V2-08 through.

The correct route is a separate bounded product/layout repair.

Canonical repair packet:

`Docs/Handoffs/RACK-V2-08-Layout-Repair-01/TASK-PACKET.md`

Canonical repair claim: **#59**.

The repair must restore both:

1. Stand Map content/caption fit within the window at 1280×720 and 1920×1080; and
2. Rack V2 minimap teaching-note presentation without clipping estimate/competitor content.

Combined HUD must pass in both model-0 and Rack V2 branches after the repair.

## V2 `Show trees` assertion disposition

Accepted.

The first V2 run contained a new, unprotected check that required the legacy Work Plan `Show trees` control. That premise is not valid for Rack V2 because V2 exposes rack-owned fell orders per job rather than creating the legacy session `RackGroups` presentation.

Replacing that **new, unprotected V2-only** assertion with checks of the actual V2 presentation is allowed because:

- the protected legacy/model-0 `Show trees` assertion remains unchanged;
- model-0 passes it;
- the V2 replacement asserts concrete player-visible behavior: rack-owned jobs are listed per job and the real Sell/Remove controls are present and enabled;
- no protected test was weakened.

This is a fixture/presentation correction, not a product rebaseline.

## Resume condition for #49

Claim #49 remains `[BLOCKED]` and no RACK-V2-08 PR/integration is authorised yet.

After #59 is Manager-approved and integrated:

1. resume the existing RACK-V2-08 branch/worktree with one writer;
2. bring in the repaired `main` without discarding unrelated work;
3. re-run Combined HUD for model-0 and Rack V2 at both required resolutions;
4. re-run the RACK-V2-08 Guided/Sequential model-0 and V2 gates, Legacy40 compatibility and RACK-V2-07 preservation gate, plus proportionate compile/static checks;
5. return `NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW` only if all required gates are green.

No Windows build is authorised.
