# #443 G3 — Objective-1-only bounded acceptance list and allowed file set

**Status: PROPOSED, awaiting Manager #424 approval. No production source has been edited.** Prepared 12 November 2026 by Anthropic Claude Opus 5.5 (`claude-opus-5-5`). The packet names Sol/Codex GPT-6.1 Sol High. Sol hit its usage limit before doing any work, and the user then explicitly chose Claude for this task.

- **Branch:** `feature/scenario-one-objective1-ux-repair-20261112-v1`, from `55863f1b7719011bba16b3f3ab377e379c0b988a`.
- **Lane:** `/media/jer/ZX20/CCF-S1-Objective1UX-20261112`.
- **Basis:** [#443 packet](https://github.com/jermur788/CCF/blob/docs/manager-424-objective1-first-repair-routing-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-OBJECTIVE1-FIRST-UX-AND-ACCESS-REPAIR-DISPATCH-20261112.md), the #440 DRAFT [spec](https://github.com/jermur788/CCF/blob/docs/manager-424-playtest-ux-workstream-20261112-v1/specs/scenario-one-playtest-ux-repair-v1/spec.md) / [plan](https://github.com/jermur788/CCF/blob/docs/manager-424-playtest-ux-workstream-20261112-v1/specs/scenario-one-playtest-ux-repair-v1/plan.md) / [AC-01..AC-26](https://github.com/jermur788/CCF/blob/docs/manager-424-playtest-ux-workstream-20261112-v1/specs/scenario-one-playtest-ux-repair-v1/checklists/requirements.md), the [D-073 wording options](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-WORDING-OPTIONS-AND-OBJECTIVE2-FOLLOWUP-20261112.md), and the G2 [#438 witness result, class A](https://github.com/jermur788/CCF/blob/docs/scenario-one-438-rack-entrance-native-witness-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-438-RACK-ENTRANCE-NATIVE-ID-WITNESS-RESULT.md).

## Global rules for G4

These apply to every item:

- **Objective 1 only.** No Objective 2 curriculum/sample/selection/Crop Tree logic. No MFM, later-cycle, Work Plan mechanics, approval/advance gates, save schema, persistent rack IDs, rack/access/tree-mask geometry or Legacy40/Reference behaviour.
- **Existing UI Visual System V2 only.** Reuse the `.keycap` token (`ScenarioOneUi.uss:612`), `UiKit.KeyPrompts`, the `next-action` guidance and `AnimateGuidance`. No new palette, no new framework, no new asset files.
- **Reduce motion keeps its current default (ON, `ScenarioOneUiRoot.cs:214`).** Every pulse has an equally clear static state, which is what most players see by default. Changing the default needs a separate decision.
- **Pulse allowlist unchanged** (`ScenarioOneTeachingSequence.SafePulseTarget`): no approve/delete/fell/advance control ever pulses.
- **Small reversible commits.** If an item turns out to need mechanics or a file outside the allowed set, stop and report it; do not widen scope.

## Bounded acceptance list (IN SCOPE if approved)

| ID | From | Acceptance (observable) | Notes |
|---|---|---|---|
| O1-01 | AC-01 / #1 | After the third intro page closes in an Objective 1 world, the HUD objective panel (which carries the `[O] Details` cue) shows the guidance border before Objectives has ever been opened. It is static under Reduce motion and a slow 1.2 s tint otherwise. It stops once Objectives is opened. | Today `AnimateGuidance` excludes `CurrentTopicIndex == 0`. Session-only latch; nothing persisted. |
| O1-02 | AC-02 / #2 | On the Objectives screen in Objective 1, the Stand Map CTA keeps its highlight but the literal `Next:` badge is not inserted. The badge stays in other contexts. | `nextBadge` (`ScenarioOneUiRoot.cs:215`) suppressed only for `objective-primary-cta` during Objective 1. |
| O1-03 | AC-03 / #3 | Objectives: title is plainly the largest text, tabs/buttons are larger, and the instruction line is not smaller than the secondary line. Fits at 1280×720. | USS sizes/weights, plus the class mapping in `LearningObjectivesView.cs`. |
| O1-04 | AC-04 / #4 | The Objective 1 diagram stays beside the current description at the same visible anchor after More detail open → close and after Esc → reopen, 3× at both resolutions, with no new scrolling needed. | Root cause first (`scrollOffset` restore at `LearningObjectivesView.cs:151/220` is a suspect). **If the fix is not contained in `LearningObjectivesView.cs` + USS, report and DEFER.** |
| O1-05 | AC-05 / #5 | Stand Map first-use title is `Stand Map` (no "· find where to look"). The sentence "The map helps you decide where to look, not what to do." is removed. In More detail, "The map helps you find where to look." is removed. Genuine keys M/Esc are shown as keycaps where the help renders them; **Back to forest** stays button-styled wording, not a keycap. | Exact current bytes are in `MenuHelpView.Text.cs`. The keycap in help prose needs a small inline-keycap render in `MenuHelpView.cs` (reusing `.keycap`). If that is not possible without a new text framework, keep plain text and DEFER only the keycap part. |
| O1-06 | AC-06 / #6 | First guided Stand Map visit in Objective 1, before any rack plan: `map-plan-rack` carries the next-action highlight and the existing gentle pulse (static under Reduce motion). It stops once Plan Racks is active or the plan exists. No other control pulses. | `map-plan-rack` is already on the allowlist; only the guidance target selection changes. |
| O1-07 | AC-07 / #7 | In Plan Racks, the right-hand instruction card is visibly distinct from the centre overview/legend (heading weight plus a restrained accent edge). Map and legend legibility are unchanged. | `StandMapView.cs` class + USS. |
| O1-10 | AC-11, AC-20 / #10, #16 | First-use help/info overlays (Introducing Tree Marking, Work Plan first use) get a darker backdrop, so underlying text is not readable through it at 720p/1080p. The foreground window stays opaque and high-contrast. Pointer and keys stay contained (the existing `UiKit.MakeModal`), and Esc/Continue still work. | `--backdrop` token / `.modal-backdrop` in USS. Shared with other help screens; that is accepted as consistent. |
| O1-13 | AC-14 / #13 (deletion only) | In the Tree Marking explanation, delete exactly "A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark." **"Marking cuts nothing: … Work Plan [Tab]." stays unchanged** (its rewrite is wording set 2, deferred). | **Unique-rule audit:** the one-mark rule is still enforced and explained in-world by `RackSprayPrompt` ("Crop Tree · remove its blue designation…", "Fell mark · remove its red designation…") and `MarkRackEntrance` refusals. The orange/red/blue table, "marking cuts nothing" and the Approve-then-Advance sequence remain. |
| O1-14 | AC-15 / #14A | Before the first spray (and after Tree Marking help is dismissed), the minimap shows the current rack field target as a destination marker, **from the same `TryGetRackFieldTarget` the HUD uses**. On first appearance it gets a first-time emphasis (static ring under Reduce motion). The HUD direction box already shows the target (witness `w03`); it gets O1-15's label. | Read-only projection in `StandMapSource`/`WalkingHudView`; the minimap writes nothing. |
| O1-15 | AC-16 / #14B (presentation only) | The field target order and the player-facing label use a **south→north visit index** (by confirmed entrance Z, ties broken by stable ID): "Rack 1" = southernmost unmarked-or-marked entrance, and so on. After any mark, the cue goes to the next **unmarked** entrance in that visit order. **Stable rack IDs, saved fields, spray association, Work Plan orders and history are unchanged.** On a fresh suggested plan this is identical to today (witness: IDs = visit order). It differs only for edited or hand-drawn plans. | `TryGetRackFieldTarget` iteration order + label text in `ForestTreeMarkingManager.RackEntrances.cs` (`RackSprayPrompt`) and `WalkingHudView.cs`. **The spray still resolves by tree→own rack, unchanged.** Pure test: visit index ≠ stable ID allowed; association never uses the visit index. |
| O1-16 | AC-18 / #14D | The minimap is substantially larger. Target: ≈2.3–2.6× **area** (≈1.5–1.6× linear; `SidePx` 196 → ~300). Measured at 1280×720 and 1920×1080 with no overlap with the bottom control bar, right-column panels or the reticle, and the player/destination symbols remain legible. Baseline (witness): surface 157×156 px @720p, 235×236 @1080p. | If no overlap-free size ≥2× area exists at 720p, ship the largest that fits and report it. |
| O1-17 | AC-19 / #15 | After 5/5, `[Tab]` in the HUD objective instruction renders as a keycap. No other change to the completion text or logic. | `ActiveObjectiveHudWidget.cs` instruction line via `UiKit.KeyPrompts`-style segmentation. |
| O1-18 | AC-21 / #17 (Objective 1 encounter only) | Work Plan: title > tabs > section headings > body, with more spacing between groups, fitting 720p/1080p. Approve and Advance stay distinct and unclipped. No data/controls removed. | USS + class names in `WorkPlanView.cs` only; **no logic edits.** |
| O1-19 | AC-22 / #18 | While the Annual Review screen is open, no guidance line tells the player to open the Work Plan [Tab] to read or acknowledge it. It says "Read and acknowledge the Annual Review." The latest-year acknowledgement, earlier-year read-only behaviour and review-first gate are unchanged. Outside the review screen the existing instruction remains. | `ScenarioOneRackBayText.ReviewInstruction` / `LearningObjectivesView.ReviewFirstInstruction`, made screen-aware through `ScenarioOneUiRoot`. |
| O1-20 | AC-23 / #19 | `AnnualReviewReadPrompt` no longer begins "Read WORK DONE, MONEY and FOREST." The rest of the sentence (wording set 3) is unchanged. Sections and the "I've read the annual results" button are unchanged. | `MenuHelpView.Text.cs` only. |

## DEFERRED / read-only (NOT implemented unless the user or Manager chooses)

| Item | Status |
|---|---|
| Wording set 1 (orange spray sentence, #11 / AC-12) | **DEFERRED.** Option not chosen; the current sentence stays. |
| Wording set 2 ("Marking cuts nothing" replacement, #13 / AC-14 rewrite) | **DEFERRED.** Current sentence stays; only the O1-13 deletion is proposed. |
| Wording set 3 (Annual Review "One thinning…", #20 / AC-24) | **DEFERRED.** Current sentence stays; only the O1-20 first-sentence deletion is proposed. |
| "Now that you've confirmed your rack plan…" lead (#12 / AC-13) | **DEFERRED.** Single Manager proposal, not user-confirmed. (Current source starts "After Confirm Rack Plan, …", not all-caps; the all-caps impression may come from the term itself.) |
| Rotation **angle snapping** (#8B / AC-09) | **DEFERRED / read-only.** Rotation deliberately rotates freely today (`StandMapRackEditor.cs:19`). There is no accepted reference/tolerance/drag-away ruling. |
| Lower-left rack geometry (#9 / AC-10) | **DEFERRED / read-only design investigation.** No rack, access, tree-mask or entrance change. |
| Rotation-grip **cursor** (#8A / AC-08) | **Approve separately, or defer.** No runtime cursor mechanism exists in the project. It would need a code-generated cursor texture (no asset) set on hover/drag of the round handle, in `StandMapRackEditor.cs`. Recommendation: include as O1-08 only if the Manager accepts that new small mechanism; otherwise DEFER. |
| Objective 2 copy/logic (#21) | Out of scope (#439/#441/#444). |

## Allowed file set (exact; G4 may touch only these)

Production (all under `Assets/ForestPrototype/`):

- `UI/ScenarioOneUiRoot.cs` (O1-01, O1-02, O1-06, O1-19 screen-awareness)
- `UI/LearningObjectivesView.cs` (O1-03, O1-04, O1-19). **`LearningObjectivesView.Sequence.cs` and `.Curriculum.cs` are read-only.**
- `UI/ScenarioOneRackBayText.cs` (O1-19 string only)
- `UI/StandMapView.cs` (O1-07)
- `UI/Map/StandMapSource.cs` (O1-14, read-only projection)
- `UI/HudMinimapView.cs` (O1-14 emphasis, O1-16 size)
- `UI/WalkingHudView.cs` (O1-15 label, O1-16 layout reservation)
- `UI/ActiveObjectiveHudWidget.cs` (O1-17)
- `UI/MenuHelpView.cs` (O1-05 inline keycap render, if contained)
- `UI/MenuHelpView.Text.cs` (O1-05, O1-13, O1-20 copy only)
- `UI/WorkPlanView.cs` (O1-18 class names/layout only, no logic)
- `UI/Resources/ScenarioOneUi.uss` (O1-03, O1-07, O1-10, O1-18)
- `ForestTreeMarkingManager.RackEntrances.cs` (O1-15 only: iteration order of `TryGetRackFieldTarget`, label in `RackSprayPrompt`. **`TrySprayRackEntrance` association code unchanged.**)
- `UI/Map/StandMapRackEditor.cs`: **only if O1-08 is approved.**
- `UI/ScenarioOneTeachingSequence.cs`: reserved to this owner, **expected unchanged** (the allowlist already covers the targets).

Tests and tooling (non-shipping):

- New Tier 1 offline tests under `Tools/Verification/Objective1UxRepair/` (pure copy/visit-index/guidance-state checks).
- A disposable Tier 2 native harness in the same folder, staged/removed by the documented runner.

Not touched: scenes, prefabs, ScriptableObjects, ProjectSettings, packages, `.meta`/GUIDs, `ScenarioOneManager*`, save/validation, `RackGeometry`, Objective 2 files, MFM, Reference/Legacy40.

## G5 verification plan (unchanged from packet)

- **Tier 1:** offline copy/visit-index/guidance-state tests, plus existing relevant offline checks, with pre-existing failures disclosed separately.
- **One focused Tier 2 native route at 1280×720 and 1920×1080:** third intro page → Objectives → Stand Map/Plan Racks → confirm → 3D first and later entrances → Work Plan → Approve → Advance → actual Annual Review acknowledgement. It includes Reduce motion ON and OFF, diagram fold/reopen + Esc/reopen, modal dimming/containment, minimap bounding boxes, a Legacy40/old-world negative (no Tree Marks/minimap rack target), save-slot isolation, and a Reference untouched check.
- No Tier 3, no weakened tests, no Windows/Player build, no main integration.

## Approval requested

Manager #424, please reply on #443 with **APPROVED / NEEDS CHANGES / REJECTED** for:

1. O1-01…O1-20 as listed.
2. The allowed file set.
3. O1-08 (cursor): include or defer.
4. Confirmation that the three wording sets and the "Now that you've confirmed" lead stay deferred.

Production edits start only after approval is posted.
