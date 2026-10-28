# RACK-V2-08-LAYOUT-REPAIR-01 — Combined HUD pre-existing layout repair

## Purpose

Repair the two **pre-existing mainline layout defects** that currently block RACK-V2-08 qualification. This is a bounded product/UI repair. It is **not** a RACK-V2-08 implementation expansion and it is **not** a protected-test rebaseline.

Canonical execution claim: GitHub issue **#59**.

## Authority / baseline

- Current Manager baseline at dispatch: `231959fbcc76810e613d2be89b4dc4834db413e4` or descendant.
- Blocking RACK-V2-08 handoff: `Docs/Handoffs/RACK-V2-08/HANDOFF.md` on branch `feature/scenario-one-rack-v2-no-competing-author-v1`, tip `5b0ed56`.
- RACK-V2-08 exact tested candidate: `7dd54ee679729da07013c3e129d8485c6067495f`; production implementation commit `6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5`.
- Baseline reproduction: unmodified main `8f8e6f03c65894b0d933aad23a4ad69227fb33e8` reproduces the same Combined HUD failures.
- RACK-V2-08 claim #49 remains **BLOCKED** pending this repair and must not be merged/integrated first.

## Manager disposition

The Combined HUD failures are accepted as **pre-existing mainline defects**, not regressions introduced by RACK-V2-08, because the same failures reproduce on unmodified main and occur in Stand Map/minimap layout code outside the RACK-V2-08 production diff.

The RACK-V2-08 replacement of one new, unprotected V2-only `Show trees` assertion is accepted: V2 does not create the legacy Work Plan rack group, the protected legacy/model-0 `Show trees` assertion remains unchanged and passes, and the replacement asserts the actual V2 per-job presentation and enabled Sell/Remove controls.

No RACK-V2-08 feature acceptance is granted yet. Combined HUD must be repaired and rerun before #49 may return its success token.

## Defects to repair

### A. Stand Map caption below the window

Observed Combined HUD failures:

`MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW`

The failures occur at both 1280×720 and 1920×1080 and reproduce in both model branches. Evidence in the RACK-V2-08 handoff attributes the regression to RACK-V2-04 commit `76772f4`, which moved `Plan rack(s)` onto a separate `map-tools` row and pushes the fixed-height map/caption down.

Required result:

- Stand Map caption, controls and map content fit inside the intended window at both required resolutions.
- Preserve Rack V2 Stand Map authoring, selection/editing, spacing teaching, confirmation/reopen and Legacy40 compatibility.
- Do not remove required player controls or merely hide/clip content to satisfy the gate.

Likely ownership: `Assets/ForestPrototype/UI/StandMapView.cs` and only the narrow layout/style dependencies actually required.

### B. Rack V2 minimap teaching note squeezes HUD panels

Observed failures:

- `ESTIMATE_PANEL_IS_NOT_CLIPPED`
- `COMPETITOR_LIST_IS_NOT_CLIPPED`

They reproduce on unmodified main in Rack V2 worlds. The time-limited two-line minimap teaching note introduced by RACK-V2-07 increases minimap height and can squeeze the estimate/competitor panel above it.

Required result:

- Keep the required first-time Rack V2 minimap teaching note and its meaning.
- Prevent the note/minimap from causing estimate or competitor content to clip at 1280×720 or 1920×1080.
- Do not remove the note or shorten it merely to bypass its teaching requirement unless the same required meaning remains clear and the layout is intentionally designed for both resolutions.
- Model-0 HUD/minimap presentation must remain correct.

Likely ownership: `Assets/ForestPrototype/UI/HudMinimapView.cs` and only narrow layout/style dependencies actually required.

## Protected-test / G3 rule

`Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` remains a protected gate. Its existing clipping/window assertions must remain **byte-identical** unless a new contradiction is discovered and returned to the Manager for explicit G3 disposition.

Do not weaken thresholds, skip sections, alter fixtures to avoid the defect, or replace a visual/product failure with a looser assertion.

If a protected assertion is demonstrably wrong after the repair attempt, stop with:

`COMBINED HUD PROTECTED ASSERTION CONFLICT — MANAGER G3 REVIEW REQUIRED`

## Scope

Allowed:

- the minimum Stand Map layout change needed to restore window fit;
- the minimum minimap/HUD layout change needed to prevent note-induced clipping;
- bounded verification tooling additions if genuinely needed, without weakening protected assertions;
- narrow documentation/handoff.

Excluded:

- RACK-V2-08 first-person authoring production files unless strictly required for compile-only compatibility; do not absorb its feature changes;
- Rack V2 geometry/edit/confirm/reopen mechanics;
- save/schema;
- ecology/economy/MFM;
- objectives beyond layout preservation;
- scenes/prefabs/material authoring unless unavoidable and explicitly justified in the handoff;
- Windows/Player build;
- account, region, privacy or subscription changes.

## Ownership / coordination

This repair is a separate single-writer lane. Before edits:

1. refresh `Docs/Project/coordination/active-tasks.md` live;
2. confirm #49 is BLOCKED and its branch/worktree is not actively editing the same files;
3. claim only the narrow Stand Map/minimap layout files needed here;
4. mark claim #59 `[RUNNING]` with actual agent/model/provider, branch, worktree and starting HEAD.

Recommended branch: `fix/rack-v2-combined-hud-layout-v1`.

Do not reuse or modify the blocked RACK-V2-08 worktree. Preserve unrelated dirty work.

## Model / lane

Preferred first route for this bounded repair: **T3 Code / OpenCode Go / DeepSeek V4.1 Flash, high**, if already available under the current workspace settings.

Fallbacks if the primary cannot complete: OpenAI Terra/Sol or Claude Sonnet 5.5. Record any provider/model transition on claim #59 before continuing. One writer and the same branch/worktree must be preserved.

Do not change OpenCode region/privacy settings to gain model access.

## Verification

### Static/offline

Run proportionate compile/type/static checks for every touched production file. Preserve existing protected assertions.

### Unity — exact candidate only

Acquire the machine-wide Unity lock normally after verifying claim/branch/HEAD/ownership and that no other owner holds it.

Required at **1280×720 and 1920×1080**:

1. Combined HUD — model-0/Legacy40-compatible branch: **all checks PASS**.
2. Combined HUD — Rack V2 branch: **all checks PASS**.
3. Explicitly demonstrate no `MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW` failures.
4. Explicitly demonstrate no `ESTIMATE_PANEL_IS_NOT_CLIPPED` or `COMPETITOR_LIST_IS_NOT_CLIPPED` failures.
5. RACK-V2-07 confirm/reopen/minimap preservation gate: PASS unchanged.
6. Targeted Stand Map/RACK-V2-04 preservation sufficient to show map planning/editing remains intact.
7. If the repair changes a shared HUD/minimap composition path, run a bounded Guided/Sequential smoke for both model branches or explain why the existing exact path is unaffected.

No rebaseline. No Windows build.

Record exact tested commit, Unity version, lock ownership/ids, resolutions, check counts, capture counts and cleanup state.

## Acceptance

PASS only if:

- both Combined HUD model branches are fully green at both resolutions;
- the protected assertions were not weakened;
- Stand Map and Rack V2 minimap teaching remain functionally and visually intact;
- no unrelated product/save/gameplay change is introduced;
- exact-candidate verification and lock hygiene are documented.

## After integration

Once this repair is Manager-approved and integrated, RACK-V2-08 #49 should resume in its **existing branch/worktree**. That lane must bring in the repaired `main`, verify no overlap/conflict with its own production changes, and re-run:

- Combined HUD both model branches;
- RACK-V2-08 Guided/Sequential model-0 and V2;
- Legacy40 compatibility;
- RACK-V2-07 confirm/reopen/minimap preservation;
- any proportionate compile/static preservation gates affected by the merge.

Only then may #49 return:

`NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-08-Layout-Repair-01/HANDOFF.md`

At publication mark #59 `[HANDED-OFF]`, link the handoff and close the issue.

Exit token:

`COMBINED HUD LAYOUT RESTORED — READY FOR MANAGER REVIEW`
