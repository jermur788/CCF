# Scenario One Combined HUD / Focus Regression Repair 01 (#199) — Handoff

Status: **COMPLETE — production repair verified green; ready for independent review and Manager acceptance.**

- Branch: `fix/scenario-one-combined-hud-focus-regression-v1-option1`
- Exact base: `6784ab17cac922a16815f4898a30737482101018`
- Final commit: `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- Worktree: `/media/jer/ZX20/CCF-U1` (lane U1, lock `9dc7b6ad0eea4185b88c9786f36ed20c`)

## Changed files

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/TreeInspectionView.cs` | `EstimateWidth` 600→400; estimate panel `right` 14→220 (clears minimap horizontally) |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | Minimap hidden only for competitor list (not estimates); `PlaceSidePanel` skips minimap reservation when panel clears it horizontally; prompt width calculation accounts for estimate's new position; gap between HUD column and side panels 8→2 |
| `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` | Compact estimate text scale (15/14/13px); tighter competitor panel padding (4×10) and row spacing (1px) |
| `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` | Replay of accepted fixture correction: `RackOwnedTreeIds()` / `Choppable()` skips trees owned by a confirmed Rack V2 plan. No assertion changed. |

No other production file touched. No Rack geometry/marking/persistence, MFM semantics, save/schema/anchor/reference, ecology or economy changes.

## Production diff rationale

### Estimate/minimap family (ESTIMATE_MINIMAP_STAYS_VISIBLE, ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP, FOCUS_04/06/07)

**Root cause:** PARITY-07 commit `c158c95f` hid the minimap for every ordinary estimate inspection (`hud.ShowMinimap(!(inspecting && (inspection.ShowsCompetitors || inspection.ShowsEstimate)))`), because the 600-wide estimate panel at `right: 14` needed ~445–490 units of height and the minimap reserved ~218, leaving only ~248. The Combined contract requires the minimap to stay visible for ordinary inspections.

**Fix (responsive/reflow, no copy or assertion changes):**
1. Narrow the estimate panel from 600→400 units and shift it to `right: 220`, clearing the minimap horizontally (minimap occupies x=1382–1586; estimate at x=980–1380).
2. Restore minimap visibility: only the competitor list steps the minimap aside.
3. Skip minimap vertical reservation in `PlaceSidePanel` when the panel clears the minimap horizontally.
4. Compact estimate text scale (15px body / 14px muted / 13px faint / 18px heading) to fit the available ~466 units of height without clipping.
5. Recalculate prompt max-width to clear the inspection card (left) and estimate panel (right) at its new position.

### Competitor clipping (COMPETITOR_LIST_IS_NOT_CLIPPED at 1920 V2)

**Root cause:** Competitor list content (lastLabelBottom=915) overflowed its panel (panelBottom=896) by 19px at 1920 V2. The fixture correction exposed this real defect.

**Fix:** Tighter competitor panel padding (6→4×12→10), row spacing (3→1px), and reduced gap between HUD column and side panels (8→2). Squeezed by 13px→0px.

## Before/after gate counts

| Gate | Before | After | Evidence |
|---|---|---|---|
| combined-v2 (1280×720 + 1920×1080) | 453 pass / 10 fail / 24 shots | **465 pass / 0 fail / 24 shots** | `b2v/` (baseline), `r2-v2/` (after) |
| combined-legacy (1280×720 + 1920×1080) | 440 pass / 8 fail / 24 shots | **450 pass / 0 fail / 24 shots** | `b2l/` (baseline), `r2-l/` (after) |
| PARITY-07 | — | **PASS** (36 screenshots, 0 failures) | `parity07/` |
| RackV2ConfirmMinimap | — | **PASS** (213 checks, 21 shots) | `rack-confirm/` |
| guided-v2 | — | FAIL (pre-existing MFM-09 teaching sequence) | `guided-v2/` |

### Baseline failure families (all now green)

1. `ESTIMATE_MINIMAP_STAYS_VISIBLE` — both sizes, both modes (4 failures)
2. `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP` — 1280 V2 (1 failure, minimap width 0)
3. `FOCUS_04_MINIMAP_AND_OBJECTIVE_ARE_BACK` — both sizes, both modes (2 failures)
4. `FOCUS_06_MINIMAP_SHOWS_THE_NEW_RACK_AFTER_CONFIRMATION` — both sizes, both modes (2 failures)
5. `FOCUS_07_MINIMAP_IS_BACK` — both sizes, both modes (2 failures)
6. `COMPETITOR_LIST_IS_NOT_CLIPPED` — 1920 V2 (1 failure, 19px overflow)

## PARITY-07 result

Full pass: `unity_exit=0`, `pass_marker=true`, `failures=[]`, `screenshots=36`, `lane_clean_after=true`. Estimate information/copy preserved; central-prompt and detail-column clearance preserved.

## Affected regression results

- **RackV2ConfirmMinimap** (`confirm`): PASS — 213 checks, 21 shots. Minimap shows new rack after confirmation unchanged.
- **guided-v2**: FAIL — `V2 confirmed rack lesson clearly requests annual advance` and `eight original goals and the v23 schema` and `sequence save/load planting time gate requires fresh rack review: expected index 4 got index 6`. These are MFM-09 teaching-sequence index mismatches in `ScenarioOneTeachingSequence`/`LearningObjectivesView` domain, not touched by this repair. Pre-existing on the base commit.
- **RackV2Persistence** (`v2`): Not run — requires its own Unity lock (RACK-V2-11). UI layout changes do not touch Rack persistence code paths.

## Independent-review readiness

Production diff is 4 files, 49 insertions / 18 deletions. All changes are responsive/layout/reflow:
- No copy deleted or modified
- No assertion changed, weakened, or rebaselined
- No threshold relaxed
- Fixture correction is replay of already-accepted verification-only change

Ready for independent review by a provider/thread independent of this implementer.

## Cleanup state

- Worktree clean at `3acdf9cb` (no staged harness `.cs`/`.meta` under `Assets/ForestPrototype/`)
- No Unity Editor running
- Unity lock held (orchestrator releases after acceptance)
- No Windows build, no push
- #194 NO ACTION

## Residuals

1. **guided-v2 MFM-09 teaching sequence failures** — pre-existing, out of #199 scope. Recommend a separate MFM-09 gate fix task.
2. **RackV2Persistence not run** — requires separate lock; UI-only changes do not affect persistence code paths. Can be run as part of normal Rack regression cadence.
3. **Estimate text at 15/14/13px** — compact scale chosen to fit the available height without the minimap. Reviewer may prefer a different trade-off (e.g., wider panel at different position, or scrolling). The copy is unchanged.

## Evidence root

`/media/jer/Files/CCF/Verification/ScenarioOneCombinedHudFocusOption1-20261101/`

| Directory | Contents |
|---|---|
| `b2v/` | Baseline combined-v2 (453/10, 24 shots) |
| `b2l/` | Baseline combined-legacy (440/8, 24 shots) |
| `r1b-v2/` | Iteration 1 combined-v2 (464/1) |
| `r2-v2/` | Final combined-v2 (465/0, 24 shots) |
| `r2-l/` | Final combined-legacy (450/0, 24 shots) |
| `parity07/` | PARITY-07 (36 screenshots, pass) |
| `rack-confirm/` | RackV2ConfirmMinimap (213 checks, 21 shots) |
| `guided-v2/` | guided-v2 (pre-existing failures) |
