# RACK-V2-10 — Help / teaching handoff (static-qualified)

**Task:** RACK-V2-10 Help/teaching, claim #94, umbrella #91.
**Base:** integrated RACK-V2-09 head `6d3228c` (exact candidate `8f8316b`).
**Branch:** `feature/scenario-one-rack-v2-help-teaching-v1`
**Worktree:** `/media/jer/Videos/CCF-RackV2-10`
**Implementation:** Claude B / Claude Opus 5.5 wrote the WIP implementation; the DeepSeek #91 operator completed the static verification after provider rate limits. No independent review or rendered acceptance is claimed.
**Exit:** `MARKING TEACHING READY — STATIC QUALIFIED / RENDERED PENDING`

## What changed

| File | Change |
|---|---|
| `UI/MenuHelpView.Text.cs` | Added `TreeMarks` as the last help menu; `TreeMarksUnlocked(rackPlanningV2Active, rackPlanConfirmed)`; `TreeMarksContext(unlocked, introduced, sprayEquipped)`; the three-entry Tree Marks list (orange/red/blue); orange convention and first-tree simplification disclosure; V2 Rack explanation; V2 step text. |
| `UI/MenuHelpView.cs` | Uses the central `RackV2Explanation` instead of the old inline copy. |
| `UI/LearningObjectivesView.cs` | V2 Objective 1 uses `RackV2Concept` / `RackV2Practice` and `RackV2StepText`; step IDs and saved progress unchanged. |
| `UI/LearningObjectivesView.Curriculum.cs` | Added V2 Objective 1 concept/practice/instruction and rack-step copy. |
| `UI/RackLayoutTracker.cs` | V2 objective instruction now routes through `LearningObjectivesView.RackV2Instruction` and includes the marking step. |
| `UI/ScenarioOneUiRoot.cs` | Walking context selects Tree Marks once a V2 rack plan is confirmed (or while the spray is equipped); local first-use preference only, no save field. |
| `Tools/Verification/MenuTutorialVerification.cs` | Existing five-menu preference check excludes Tree Marks, which is intentionally conditional in a non-V2 flow. |
| `Tools/Verification/RackV2HelpTeaching/` | Bounded offline wording/unlock/flow check. |

No Rack geometry, marking, save/schema, ecology/economy, S1-C/S1-D/MFM/Parity, scene/prefab/material, package or ProjectSettings change.

## Copy / authority mapping

- Tree Marks entries: only orange (rack/machine-path removal), red (Fell), blue (Crop Tree/future timber), per spec §25/§28/§29.
- Orange convention: source describes orange/pink removal stripes; Scenario One chooses orange (calibration); one spray paints all four sides (teaching simplification disclosed).
- 16–20 m is framed as the book's early-transformation example and this stand's calibrated guide, never a universal rule. Five racks is this stand's calibration, not a general requirement.
- Objective 1 flow: plan on the Stand Map, confirm, mark entrances in the forest, then approve work in the Work Plan and advance a year.
- No `18 m ideal`, no `always five racks`, no universal-practice claims.

## Verification actually run

- `python3 -B Tools/Verification/RackV2HelpTeaching/run_offline.py`
- `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`
- `RACK_V2_HELP_TEACHING_PASS static_and_offline=15`
- Covers entry set, unlock condition, context selection, required disclosures, forbidden wording, Objective 1 flow, and no save-field use.

## Not done / residual

- **No Unity import/compile, no rendered inspection.** RACK-V2-13 owns rendered acceptance at 1280x720 and 1920x1080; this handoff does not claim visual pass.
- **No independent review yet.**
- Tree Marks is conditionally introduced; the existing menu-tutorial gate was narrowed only to exclude this intentionally conditional screen, not to weaken its five normal menus.
- No Windows build, no Unity lock, no lane reservation.
