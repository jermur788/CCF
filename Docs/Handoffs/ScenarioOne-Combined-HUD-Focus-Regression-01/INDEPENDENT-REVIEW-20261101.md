# Scenario One Combined HUD / Focus Regression Repair 01 (#199) — Independent Review

**Reviewer:** DeepSeek V4.1 Flash orchestrator (`opencode-go/deepseek-v4.1-flash`), independent of the Mimo 2.6 Pro implementer.  
**Claim:** #199  
**Exact base:** `6784ab17cac922a16815f4898a30737482101018`  
**Production commit under review:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Handoff commit:** `0d8898980b192d651360a10c55cde7e73bc4ba31`  
**Branch:** `fix/scenario-one-combined-hud-focus-regression-v1-option1`  
**Lane/worktree:** `/media/jer/ZX20/CCF-U1`  
**Verdict:** **ENDORSED FOR MANAGER REVIEW — READY FOR MANAGER ACCEPTANCE** with the visual trade-offs and one process deviation recorded below. This review does not self-accept or integrate the repair.

## 1. What was reviewed

- Full production diff `6784ab17..3acdf9cb` (3 production files + 1 verification fixture).
- `combined-v2` and `combined-legacy` baseline and final logs, counts and screenshots at 1280×720 and 1920×1080.
- PARITY-07 rendered gate log and screenshots.
- RackV2ConfirmMinimap gate log.
- guided-v2 final log **and an independent baseline guided-v2 run at exact `6784ab17`** under a separate lock.
- The worker handoff and cleanup state.
- Key rendered screenshots (`c02-inspect-estimate-with-minimap` both sizes, `c03-competitor-list-minimap-aside` 1920).

## 2. Production diff — OBSERVED FACT

| File | Change | Review |
|---|---|---|
| `Assets/ForestPrototype/UI/TreeInspectionView.cs` | `EstimateWidth` 600→400; estimate `right` 14→220 | Layout-only; clears the minimap horizontally |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | minimap hidden only for competitor list; `PlaceSidePanel` skips minimap reservation for the estimate; prompt width recalculated; side-panel top gap 8→2 | Layout-only; no Rack/MFM/save/ecology/economy path |
| `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` | estimate text 15/14/13px (heading 18px); competitor panel padding 4×10 and row padding 1px | Presentation-only; **visual trade-off, see §5** |
| `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` | `RackOwnedTreeIds()` / `Choppable()` skips Rack V2-owned trees | Replay of the already-accepted verification-only fixture correction; **no assertion changed** |

No copy, protected assertion, threshold, baseline, save/schema/anchor/reference, Rack geometry/marking/persistence, MFM semantics, ecology or economy change was found in the diff.

## 3. Gate evidence — OBSERVED FACT

| Gate | Baseline | Final | Verdict |
|---|---|---|---|
| `combined-v2` | `CHC_DONE pass=453 fail=10 shots=24` | `CHC_DONE pass=465 fail=0 shots=24` | **PASS** |
| `combined-legacy` | `CHC_DONE pass=440 fail=8 shots=24` | `CHC_DONE pass=450 fail=0 shots=24` | **PASS** |
| PARITY-07 | — | `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0` | **PASS** |
| RackV2ConfirmMinimap | — | `RACK_V2_CONFIRM_MINIMAP_UNITY_PASS checks=213 shots=21` | **PASS** |
| guided-v2 | (not run by worker) | 553 checks / 47 shots / 11 failures | **Pre-existing — see §4** |

The previously failing families (`ESTIMATE_MINIMAP_STAYS_VISIBLE`, `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP`, `FOCUS_04/06/07`, `COMPETITOR_LIST_IS_NOT_CLIPPED`) are green in the final logs. The final `combined-v2`/`combined-legacy` runs (10:15Z / 10:24Z) ran on the production content that was committed immediately afterwards as `3acdf9cb` (10:25Z); the later `0d889898` commit is docs-only. PARITY-07 (10:33Z) and RackV2ConfirmMinimap (10:38Z) ran after the production commit.

## 4. guided-v2 residual — independently proven pre-existing

The worker handoff classifies the guided-v2 failures as MFM-09 teaching-sequence index mismatches. The independent baseline run at exact `6784ab17` under lock `3bb093b47fe54f7a887122ffb8591053` produced **exactly the same result** as the final commit:

- baseline: `checks=553`, `shots=47`, `passed=false`, **11 failures**;
- final: `checks=553`, `shots=47`, `passed=false`, **11 failures**;
- failure lists are **identical** (set difference empty both ways).

The failures are therefore pre-existing on the base and not introduced by #199. They are, however, broader than the worker's wording: they include stale v23 schema pins (`eight original goals and the v23 schema`, `the save schema version is unchanged`), Objectives-window content overflow (`current objective fits` at both sizes), teaching-sequence index mismatches (`expected index 4 got index 6/7`, `three future trees and six stand portions reviewed`), and `V2 confirmed rack lesson clearly requests annual advance`. None is in a file touched by #199. They remain out of #199 scope and should be handled by a separate MFM-09/verification-pin task.

Evidence: `guided-v2-baseline/evidence/guided-result.json` vs `guided-v2/evidence/guided-result.json`.

## 5. Visual review and trade-offs — OBSERVED FACT + ORCHESTRATOR CLAIM

Rendered screenshots inspected:

- `r2-v2/evidence/c02-inspect-estimate-with-minimap-1280x720.png`
- `r2-v2/evidence/c02-inspect-estimate-with-minimap-1920x1080.png`
- `r2-v2/evidence/c03-competitor-list-minimap-aside-1920x1080.png`

Observed: the minimap stays visible during ordinary estimate inspection; the estimate panel sits left of it and does not overlap; the central prompt is clear; the competitor list is fully inside its panel at 1920 V2; the HUD pieces do not overlap. The layout is functional and readable.

Trade-offs the Manager should explicitly accept:

1. **Estimate typography** — the estimate panel now uses 15px body / 14px muted / 13px faint / 18px heading instead of the shared scale. The copy is unchanged, but this is a local visual-system deviation. The worker handoff flags it as a residual. A different trade-off (e.g., a wider/reflowed panel) would preserve the shared scale but may require more layout work.
2. **Competitor row density** — row padding 3→1px and panel padding 6→4×12→10. The list now fits with ~0 spare height, so it is tight and fragile to future font/scale changes.
3. **Side-panel gap** — the gap under the HUD column is 8→2 units. Functional, but visually tighter.

These are presentation choices, not protected-assertion or copy changes. They are the reason this review endorses the repair **for Manager review** rather than treating it as an unqualified visual acceptance.

## 6. Process deviation — lock metadata update

**OBSERVED FACT:** the worker updated `/media/jer/ZX20/CCF-Ops/state/unity/lock.json` from HEAD `6784ab17` to HEAD `3acdf9cb` at 10:28:28Z without a corresponding release/acquire event in `history.jsonl`. The lock remained owned by the same task/owner and no competing writer existed; the PARITY-07 gate's lock preflight then saw the updated HEAD. The gate results are not invalidated, but the metadata update bypassed the documented acquire/release flow.

The orchestrator released the worker lock normally after the run, acquired a separate lock for the independent baseline guided-v2, released it, and left the machine-wide lock **free** with no Editor/Xephyr/openbox processes. The final worktree is clean at `0d889898`.

## 7. Cleanup and boundaries

- Worktree clean at `0d889898`; no staged harness `.cs`/`.meta` under `Assets/ForestPrototype/`.
- Machine-wide Unity lock **free**; no Unity Editor process.
- No Windows build, no integration, no push of the branch at review time.
- Research Source Consolidation **#194 untouched**.
- The worker's `.vscode/settings.json`/material Editor side effects were restored to HEAD after the independent baseline run.

## 8. Residuals

1. guided-v2 pre-existing failures (stale v23 pins, Objectives overflow, teaching-sequence indices) — separate task.
2. RackV2Persistence not run; the diff does not touch Rack persistence code paths. Proportionate Rack confirmation/minimap gate passed.
3. Estimate/competitor/side-panel visual trade-offs — Manager acceptance or a follow-up visual-polish task.

## 9. Recommendation

**Endorse `SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`.** The production repair is bounded, the protected Combined and PARITY-07 gates are green on the exact production commit, the only verification change is the already-accepted fixture correction, and the guided-v2 residual is independently proven pre-existing at the exact base. Manager acceptance should explicitly note the visual trade-offs in §5 and the lock-metadata process deviation in §6.
