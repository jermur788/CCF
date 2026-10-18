# Proposed canonical-document updates (not applied)

Per the packet, the canonical documents are **not** edited by this task and nothing was published midway. The text below is for the Manager or integrator to apply when the work is accepted. The Unity overview text describes only behaviour that was verified; items that were not verified are labelled so. The decision number is a placeholder; the Manager assigns it.

## 1. `Docs/Project/decision-log.md`

Add one row (next free number; `D-0XX` here):

| D-0XX | 2026-10-18 | Scenario One teaches extraction racks before selective marking | Confirmed (Manager amendment); implemented and Editor/Linux-Player verified; not integrated | Scenario One's first management cycle is the creation of extraction racks: straight corridors of felled trees across the planted rows, so machinery can reach the stand now and in future thinnings. Individual selective marking (inspection, Crop Trees, X/C) is introduced afterwards, once the first intervention has been resolved. Racks are created with a bounded, deterministic backend (corridor width configurable, default 2.4 m; start/end clipped to the stand; trees chosen from tree positions; crop trees and trees already planned skipped) through an in-world tool (B) and a Stand Map "Plan rack" tool, and are ordinary FellTree work orders in the DRAFT Work Plan. The authored Scenario One layout is five racks with seven rows left standing between them; this spacing is Scenario One's layout data, not a universal rule. No save schema change (v20), no ecology, calibration or economy change. The teaching gate is soft (prompts, cues, lesson order); X and C still work. Rack grouping is session-only. Source: branch `feature/scenario-one-rack-first-ux-v1`, production head `cd0692a`; handoff `Docs/Verification/ScenarioOneRackFirstUx/Handoff.md`. |

The same revision also repairs the playtest UX items (Help window, tree inspection card and world indicator, plain-language terminology, Stand Map modal, Work Plan single "Approve Work Plan" action, HUD buttons responding to a real click); these need no separate decision.

## 2. `Docs/Project/current-milestone.md`

Under "In scope", add:

- Scenario One playtest UX revision and rack-first tutorial: implemented and verified on `feature/scenario-one-rack-first-ux-v1` (production head `cd0692a`); **awaiting Manager acceptance and integration**. Native-Windows behaviour UNVERIFIED. Open decisions: soft versus hard gating of selective marking; whether to persist rack grouping (needs a save-schema change); acceptance of the year-one cash cost of the authored layout (−€1,747.41).

Do not change the Stage A "NOT READY" wording on account of this work.

## 3. `Docs/Project/unity-project-overview.md`

Add a section after "Integrated malformed cell-index save hardening" **once the Manager accepts and integrates the branch**; until then it is proposed behaviour on an unintegrated branch.

> ## Integrated Scenario One rack-first tutorial and playtest UX revision (date, D-0XX)
>
> - **Racks.** `Racks/RackGeometry.cs` (pure geometry: bounded start/end, configurable corridor width default 2.4 m, boundary-to-boundary clipping to the 80 × 80 m stand, deterministic selection ordered along the line then by tree id, crop trees and already-planned trees skipped and counted) and `Racks/RackPlanner.cs` (row detection from tree positions, the authored layout, `Propose` and `ProposeBetween`). `Racks/RackTool.cs` is the in-world tool (B; aim at a planted row; translucent corridor; mouse wheel turns it in 15° steps; violet rings and posts on the trees it would take; figures card; click or Enter confirms; Esc or B cancels with no change). The Stand Map has a "Plan rack" mode using the same planner. `ScenarioOneManager.Racks.cs` confirms a rack as ordinary Pending FellTree orders (`ConfirmRack`) and keeps the session-only grouping used by the Work Plan and the violet planned-tree markers (`Racks/PlannedFellMarkers.cs`). No rack object is persisted; save version 20 unchanged.
> - **Authored layout.** `ScenarioOneRackLayout.RowsBetween = 7`: five racks across the 41 planted rows (row index 4, 12, 20, 28, 36), 157 of 1,344 trees (11.7%), 17.89 m³ stem volume, timber income €752.59 against the shared €2,500 contractor minimum, net −€1,747.41 in year one. This is Scenario One layout data, not a general rule.
> - **Teaching order.** The first lessons are racks, approve, advance, Annual Review; inspection, Crop Trees and selective felling follow once the first intervention exists (`ScenarioOneManager.RackFirstStageActive`). The gate is soft (prompts, cues, status line, lesson order, Help); X and C remain available.
> - **Work Plan.** Racks are grouped at the top; the single approval action is "Approve Work Plan" (same all-or-nothing approval as before); "Add marked trees" and the "What are you doing?" heading were removed. Marks still convert to Pending orders when the plan opens.
> - **Playtest UX.** F1 Help is a modal window; the tree inspection card no longer covers the side columns and a neutral white cage marks the inspected tree; the Stand Map is modal over a dimmed, input-blocking backdrop; HUD Map/Objectives/Help buttons respond to a real click when the mouse is free (previously `ForestPlayer` re-captured the mouse on the press); wording uses "square", "average forest-floor light", "areas with young regeneration", "Establish successful regeneration" (objective and calculation unchanged); Sitka foliage is tinted darker in code.
> - **Verification.** Compile/import clean; rendered harness at four sizes; 96 rack checks; narrow Tier-2-style regression (UX, Menu, U4 guidance, Startup, P2, P3, Interact) PASS; Linux Player build, save/load smoke and a real-pointer run at 1920×1080. Not run: Tier-3, native Windows. The reported "More Detail does nothing" and top-tab hitbox offset were not reproduced.
> - **Modal input rule.** While the help and first-use window is open (`ScenarioOneManager.ModalOpen`, set by `MenuHelpView`) it owns the keyboard: Tab, M and O do nothing (the manager ignores Tab, the UI root ignores M and O), so no screen opens behind it or replaces it. Continue, Esc and F1 close it and the shortcuts work again at once.
> - **Unchanged.** Growth, regeneration, browsing, density, Model2 calibration, objective thresholds, economy values, Reference Future and historical hashes, Scenario Two, Asset Store files.

## 4. `Docs/Project/coordination/active-tasks.md`

Only the Manager edits this file. Proposed row (clear it when the branch is integrated or abandoned):

| Scenario One playtest UX revision and rack-first tutorial | Claude (implementation, by Manager packet and amendment) | feature/scenario-one-rack-first-ux-v1 | /media/jer/ZX20/CCF-U1 (lane U1) | Scenario One UI, Racks, rack-related ScenarioOneManager partial, Work Plan, verification tools | No gameplay, ecology, economy or save-schema change; Unity lock released | Implemented and verified, awaiting Manager review; not integrated |
