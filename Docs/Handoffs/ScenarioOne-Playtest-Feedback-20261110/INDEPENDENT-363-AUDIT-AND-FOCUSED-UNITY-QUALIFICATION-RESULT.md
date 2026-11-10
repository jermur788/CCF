# #364 — Independent audit of #363 source and focused Unity qualification result

**Date:** 10 November 2026. **Claim:** [#364](https://github.com/jermur788/CCF/issues/364). **Parent:** Manager #200.
**Binding packet:** [MANAGER-363-TO-364](MANAGER-363-TO-364-INDEPENDENT-SOURCE-AUDIT-AND-CONDITIONAL-WARM-UNITY-QUALIFICATION.md).

## Verdict

| Item | Result |
|---|---|
| **Source review (this feature only)** | **NEEDS CHANGES.** Not rejected. Three blocking product defects, all small and all in UI files the task already owns. |
| **Phase A to Phase B gate** | **NOT PASSED.** `SOURCE REVIEW APPROVED FOR FOCUSED UNITY QUALIFICATION` is **not** recorded. |
| **Unity Tier 2 qualification** | **NOT RUN.** Stopped before Unity as the packet requires. No lock was acquired, no Editor was started, U3 was not touched. |
| Offline evidence rerun | Reproduced exactly: compile 189 sources / 116 refs / 0 errors, 67 focused checks, 3/3 mutations, pixel witness 188.32 / 96.89 and 0.00. |
| Test honesty | No false green found. The checks are true but narrow: none of them exercises the code where the three defects are. |

A reviewer recommends; it does not accept. This is not user acceptance, integration approval or an external-demo statement.

## Reviewer, lane and commits

- **Reviewer:** Anthropic **Claude Fable 5** (`claude-fable-5-2`), Claude Code harness in T3 Code, local session, no sub-agents. Harness reasoning-effort value 10 (label not exposed to the reviewer). Different provider from the source author (OpenAI GPT-6.1 Sol Medium). Disclosure: the same model family wrote the earlier read-only #362 design review. It wrote none of the #363 source. The packet recommended Claude Opus 5.5; the model actually used is the one named here.
- **Docs lane:** new sparse docs-only worktree `/media/jer/Files/CCF/Operations/CCF-IndependentPlayerFeedbackReview-20261110`, branch `docs/ccf-364-independent-player-feedback-review-20261110-v1`, start HEAD `0e5ec3a3fc8d9593972e9015cc0de818fb884751` (live `origin/main`). Sparse because Files had about 7.8 GiB free. Branch and path were checked absent before creation.
- **RUNNING receipt:** [issue comment](https://github.com/jermur788/CCF/issues/364#issuecomment-6762825479).
- **Source under review:** `bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49` against baseline `fb599eecde983552dec4f06fb9efcf8e38397aed`.
- **Report-only commit of the author:** `9cbe8be4d43a5e26332d4d8a5d8114e91e9da080`. It changes one Markdown file and no source. It is not a source test and was not treated as one.
- No production file, verifier file, `active-tasks.md`, source lane or U3 file was modified by this review.

## 1. Diff and authority

Checked from Git objects, not from the worker report.

- `fb599eec` is an ancestor of `bfd8e2a4`. Commits between them: `8c49460d` and `58f694ab` (reports only), then `b7ced0e3`, `ec4abdbd`, `f5f21748`, `cbf218e7`, `bfd8e2a4`.
- `git diff --name-status fb599eec bfd8e2a4` lists **exactly 20 modified production files, all under `Assets/ForestPrototype/UI/`**, two added Markdown reports and five added files under `Tools/Verification/PlayerFeedbackOvernight/`. No file was added or deleted under `Assets/`, so no `.meta` or GUID changed.
- Nothing under `Assets/ForestPrototype/ScenarioOne/` changed. That includes `ScenarioOneRackBay.cs`, `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneHeadlandPresentation.cs` and the manager. No scene, prefab, package, ProjectSettings, Reference archive, save or mask file is in the diff.
- The only new persisted value is the display preference `CCF.UI.ReduceMotion` in PlayerPrefs. No forest save field was added.
- **Base note, not a defect:** the merge base of the candidate with `origin/main` is `50042bbe`. The candidate sits on the long unmerged focused-playtest lineage, as the spec's baseline `fb599eec` does. Integration to main is a separate, larger question that this review does not address.

## 2. Blocking findings

All links are to the exact candidate commit.

### B-1 — Walking HUD shows the Objective 2 crop-tree prompt during Objectives 3, 4 and 5 and hides their real progress

- [`LearningObjectivesView.Sequence.cs` lines 267–269](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs#L265-L269): `Progress` is `ScenarioOneRackBayText.Walking(...)` whenever `TeachingBay != null`.
- [`TeachingBay`, lines 111–118](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs#L111-L118) is non-null for lesson indexes 1 to 4, which are Objectives 2 to 5.
- [`Walking`, lines 58–80 of `ScenarioOneRackBayText.cs`](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs#L58-L80) takes no lesson argument. It always prints the Near/Middle/Far tick boxes and ends with `[E] Inspect / [C] Mark Crop Tree`.

Consequences:

1. In Objectives 3 to 5 the HUD no longer shows that lesson's progress. At the baseline it showed, for example, `Initial release: 0% minimum / about 30% each · trunk area 0%/40%` (Objective 3) and `Revisited n/3 · decisions n/3 · stand portions n/6` (Objective 5). Those lines are replaced.
2. The HUD's one named action in those lessons is `[C] Mark Crop Tree`, which is the wrong action. A player who follows it marks a fourth Crop Tree in the footprint. [`CurrentUnlatched`, line 135](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs#L135) then returns the player to Objective 2, because `HasEnoughFuture` becomes false.

Reviewer probe output (compiled candidate, no Unity):

```text
P1 lesson index with three valid Crop Trees = 2 (2 = Objective 3)
P1 baseline HUD progress for that lesson  = Initial release: 0% minimum / about 30% each · trunk area 0%/40%
P1 candidate HUD progress (Walking), which HudModel uses for lesson indexes 1..4:
Inside teaching area · Middle · left looking from headland into rack
☑ Near  ☑ Middle  ☑ Far · 3/3
Both sides ✓ · 8m spacing ✓
[E] Inspect / [C] Mark Crop Tree
P1 after following the prompt ([C] on a fourth tree): HasEnoughFuture=False, lesson index=1 (1 = back to Objective 2)
```

Requirement affected: FR-Z3 (one current action), FR-N3 (names only the active key), spec section 9 "no false green". Scoring code is untouched; the defect is the guidance.

**Minimum patch:** in `HudModel`, use the full `Walking` text only when `index == 1` and no review acknowledgement is pending. For indexes 2 to 4 keep `TeachingProgress(f)` and, if wanted, prefix only the location line (inside/outside, third, side). One file, a few lines.

### B-2 — "Review the year" HUD state still tells the player to mark Crop Trees (checklist N-01)

Same lines as B-1. After the rack year, while the Annual Review is unread, the lesson index is 1 with `AccessReviewed == false`. `TeachingBay` is already non-null there because it tests only `AccessOpen`. The HUD then shows title `Review the year`, instruction `Next: Read the Annual Review, then walk back and look at the opened stand.` and, directly under it, a progress block ending `[E] Inspect / [C] Mark Crop Tree`.

```text
P1b unread Annual Review: lesson index=1; ... candidate Walking last line=One rack side absent. [E] Inspect / [C] Mark Crop Tree
```

Checklist N-01 says the HUD "never says Crop Tree now if an unread annual result gates progression". FR-N1 says the same. The same contradiction occurs at Objective 5 before the release review is acknowledged.

**Minimum patch:** the B-1 condition already covers it if it includes `!NeedsReviewAcknowledgement`.

### B-3 — Objective 1 figure: the "Existing clearing" label covers suggested rack 4, and two other labels sit on the infrastructure they name

[`TeachingDiagramPlan.cs` lines 608–612](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/TeachingDiagramPlan.cs#L608-L612). These labels are "backed": [`TeachingDiagram.AddText`](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/TeachingDiagram.cs#L97-L116) gives the whole declared box an opaque dark background. The result follows from the declared constants and does not depend on font metrics or screen size. The panel uses a fixed 1600×900 reference, so the layout in UI units is the same at 1280×720 and 1920×1080.

```text
P2 OVERLAP backed label 'Existing clearing' box x[290.0,495.0] y[145.0,173.0] covers rack4 band y[159.6,172.1] for 190.0 of 392.1 units (48%)
P2 OVERLAP backed label 'Existing clearing' with label '4' at x[486.0,510.0] y[151.9,179.9]
P2 'Headland' label box x[0.0,80.0] (width 80); headland casing x[79.7,96.2]; gap=-0.3 units; 8 letters at 22-unit font need about 97 units
P2 'Link track' label y[356.5,386.5] vs feeder casing top y=382.2
```

- **Certain:** an opaque box hides about 48% of the dashed line of suggested rack 4 and runs into the rack number "4". A figure whose purpose is to show five suggested racks shows about half of one of them.
- **Certain:** the "Headland" box ends 0.3 units inside the headland casing, and the "Link track" box overlaps the top edge of the feeder by about 4 units.
- **Predicted, needs a render to confirm:** "Headland" at the 22-unit figure font needs roughly 97 units and has an 80-unit box, so it will either clip or spill onto the headland corridor. This is the same complaint the Manager recorded against the 01:48 mock-up.

Requirement affected: FR-D2 (labels must not touch the headland corridor or mask a rack), checklist D-02 and D-05.

**Minimum patch:** move "Existing clearing" to a box that lies between rack 4 and rack 3 (for example y about 178 to 208, right edge at or before x 480), give "Headland" a box at least as wide as its text and clear of x 79.7, and lift "Link track" by about 6 units. Then add one geometric assertion to `Checks.cs` that no backed label box intersects a `rack*` corridor band, a rack number label, or the headland and feeder casings. The reviewer probe `P2` is a working model of that assertion.

## 3. Non-blocking findings to fix in the same patch

| Id | Finding | Where | Suggested change |
|---|---|---|---|
| N-1 | Entrance rows are grouped under "Already on the ground". The spec puts entrances under "Your plan". | [`StandMapLegend.cs` 132–136](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/Map/StandMapLegend.cs#L132-L136); probe `P3` | Add `e.Symbol == MapSymbol.RackEntrance` to the "Your plan" test. |
| N-2 | With suggestions only (0/5), hollow entrance squares are drawn on the map but the legend has no row for them. | [`StandMapLegend.cs` 73–76](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/Map/StandMapLegend.cs#L73-L76); probe `P3b` | Show the hollow row when any suggested or player rack is drawn. |
| N-3 | `List.Sort` is not a stable sort. With more than 16 legend rows the order inside a group can change. | `StandMapLegend.cs` line 136 | Use `OrderBy` (stable) or add the original index as a tie-break. |
| N-4 | Teaching-area map labels are fixed at 12 UI units, about 9.6 pixels at 1280×720. The project's own floors elsewhere are 15, 17 and 20. | [`StandMapCanvas.cs` 37–42](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/Map/StandMapCanvas.cs#L37-L42) | Raise to the reading floor and check for overlap in the render. FR-Z2 asks for 720p legibility. |
| N-5 | The walking HUD border has no V2 gate, so a Legacy40 world in Objectives 2 to 8 gets a new gold border on the objective panel. The HUD instruction is also prefixed `Next: ` in every world and in the completed state. | [`ScenarioOneUiRoot.cs` 509–525](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs#L509-L525), `LearningObjectivesView.Sequence.cs` line 266 | Gate both on `RackPlanningV2Active` unless the Manager wants them everywhere. Presentation only, harmless, but it is a change to old worlds that the report does not mention. |
| N-6 | For Objectives 2 to 8 the 20-unit `next-action-reason` line in each window is set to the full `CurrentInstruction`. On the Objectives window this puts the long text back at the top, above the card that FR-C3 just shortened. When the extension is active the checklist also appears twice on the card. | `ScenarioOneUiRoot.cs` line 362 and 448–457; `LearningObjectivesView.cs` line 166 | For the later-objective branch use the short HUD instruction as the reason, and add the checklist once. |
| N-7 | "Total quoted contractor charge before approval" stays on the card after the plan is approved, because the card lists every open harvest order. | [`WorkPlanView.cs` 354–355](https://github.com/jermur788/CCF/blob/bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49/Assets/ForestPrototype/UI/WorkPlanView.cs#L354-L355) | Drop "before approval" or show it only while work is pending. |
| N-8 | Wording: "at the headland-side rack entrance at each rack entrance". | `LearningObjectivesView.Curriculum.cs` line 102 | Remove the repeated phrase. |
| N-9 | The "What happens to a rack" strip is text only. FR-D4 asks for symbols that do not rely on colour. The key above it does have symbols. | `TeachingDiagramPlan.cs` 615–617 | Reuse the four key shapes in the strip, or record it as deliberately not done. |

## 4. What passed source review

These were read in the candidate source, with the unchanged authorities they depend on.

**Objective 2 geometry and feedback**

- `TeachingBay` returns the cached `Facts.Bay`, which comes from the unchanged `ScenarioOneRackSampleGeometry.Derive` over the confirmed racks. There is no second geometry definition. It is null in Legacy40 and model-0 worlds, during Reference preview, while the plan is unconfirmed or invalid, in lesson 1 and in lesson 6 onward.
- The frame is anchored on the third rack by position across the common normal, and its "along" direction is that rack's own entrance-to-far vector. So "Near" is always the entrance (headland) end and "left / right looking from the headland into the rack" is correct for rotated plans too. The sign matches the existing `RackInspectionPrompt`. The reviewer first suspected an inversion from the 0–180° common-axis normalisation; reading `Derive` to the end showed the teaching frame does not use that axis, so there is no inversion.
- The map outline uses the existing `StandMapAreaFootprints.TeachingPolygon`; the separators are at −4 m and +4 m along. The overlay is drawn on the Full map only for the Access and Marks layers and on the minimap as outline only.
- The waypoint goes through the existing `SetWaypointCell` path to the frame anchor, which is on the rack centreline in the opened corridor. It is a reachable point, not a tree. It is shown only when a teaching frame exists.
- No mark is refused, chosen or converted. `ScenarioOneRackBay` scoring is byte-identical to the baseline.

**Next-action safety**

- The pulse can only be applied to a button whose name is in `SafePulseTarget`. Approve Work Plan, Advance, Confirm Rack Plan, prescription approval, the consent and reset answers, Fell and Help are not in it. The only other pulse target is the walking objective panel, which is not clickable.
- Suspended when Help is open or just closed, in Reference preview, in the session menu, at time scale 0, and while a rack consent, reset, reopen or advance confirmation is pending.
- `Time.timeScale` is set to 0 only by the session menu, which was already a suspension, so the added test does not switch off the existing Objective 1 guidance.
- No focus call was added. A hidden or detached target is dropped on the next frame by the existing visibility test and its border is reset. The layer buttons are rebuilt before guidance is re-derived, so the Treatment review target is not stale.
- `Reduce motion` defaults to true and is read once in `Start`. With it on, the border is static.

**Annual Review**

- Chips are hidden by one USS rule on a root class; the chip elements and `HistoryProvenance` values are still created. The legend is built only when the toggle is on.
- Plain qualifiers are added for Derived, Reconstructed, Session-only, Not recorded and Current ("now") regardless of the toggle. Recorded gets none.
- The toggle calls `Refresh(true)`, which keeps the browsed year and open details and writes nothing.
- The acknowledgement footer is hidden for an earlier year and its callback is separately guarded by `showingEarlierYear`. "Walk the forest" is primary only when the footer is hidden, so there is one primary button.

**Work Plan quote**

- The new total is `job.CostCents` from the existing `GetHarvestQuote`. The minimum line still appears only when `MinimumJobAdjustmentCents > 0` and still prints the definition's value. No number is hard-coded and no price, eligibility or approval rule changed. The existing cash and validation blocks are untouched.

**Map, feeder and old worlds**

- The map's headland and feeder and the 3D mesh read the same constants in `ScenarioOneHeadlandGeometry`, and `ReadExistingHeadland` keeps its V2 + Enlarged80 + not-preview + live-presentation gate. The worker's statement that the baseline model already contained the feeder is correct.
- The new plan draws the original path first, then access casing, pale fill, pads and one dashed centre on the feeder only. No new segment is created. The "old path" styling repaints only the part of the existing path north of the feeder's z; it adds no spur.
- `RackSuggested` is produced only in V2 worlds (legacy uses other symbols), so the hollow suggestion squares do not reach Legacy40. Nothing reads `RackEntrance` items for hit testing, so drawing a square on every player rack does not change interaction.
- Changing `Line()` to carry dash values affects only the new symbols; the grid style has no dash.
- The five-rack figure guard is stricter than before (adds not-preview and Enlarged80). The five racks are `Draft` (dashed), entrances are outlined unless marked, and the title is "Suggested rack locations".
- Painter safety: text-only draw items have no points and the painter skips items with fewer than two points.

**Could not be judged from source (needs the Phase B render):** real contrast of the feeder over the canopy picture, minimap clutter from five hollow squares, Annual Review qualifier density, header-row overflow from the two new toggles, 720p HUD height with the four-line progress block, and every pixel requirement.

## 5. FR traceability

Author status is from the #363 report. Reviewer status is from this audit. No row is rendered-verified by anyone.

| FR | Reviewer source status |
|---|---|
| Z1 | Source acceptable. Uses `Derive`; guards correct. |
| Z2 | Source acceptable; label size risk N-4. |
| Z3 | **Defect B-1, B-2.** Correct in Objective 2 after the review is read; wrong in Objectives 3–5 and in the unread-review state. |
| Z4 | Source acceptable in Objective 2. Marks are never refused. |
| Z5 | Source acceptable. |
| N1 | **Defect B-2** for the HUD. Card and title are correct. |
| N2 | Source acceptable. Allowlist is the only route to a button pulse. |
| N3 | Affected by B-1 (names the wrong key in Objectives 3–5). Old-world gate N-5. |
| N4 | Source acceptable. Only the stored Accepted prescription earns a tick. Duplicate text N-6. |
| N5 | Source acceptable. |
| R1, R2 | Source acceptable. |
| R3 | Column order changed only, as the author states. |
| C1 | Source acceptable; label wording N-7. |
| C2 | Source acceptable; wording N-8. Both strings are in V2-only text. |
| C3 | Partly undone by N-6. |
| C4 | As N1 and N5. |
| M1 | Source acceptable. Visibility itself is unproven until rendered. |
| M2 | Source acceptable; grouping N-1, N-3. |
| M3 | Source acceptable; legend gap N-2. |
| M4 | Confirmed. No geometry, plank, mask, scene or save change. |
| D1 | Source acceptable (figure is full width and 740 units high; it will scroll). |
| D2 | **Defect B-3.** |
| D3 | Source acceptable. |
| D4 | Source acceptable; strip symbols N-9. |
| D5 | Confirmed. No capture is claimed. |

## 6. Offline evidence rerun and test strength

Rerun on a clean export of `bfd8e2a4` (`git archive`, C# files only), not on the author's worktree.

| Check | Author | Reviewer rerun |
|---|---|---|
| Typed compile | 189 / 116 / exit 0 / 0 errors | **189 / 116 / exit 0 / 0 errors** |
| Focused checks | 67 PASS | **67 PASS** |
| Mutations | 3/3 killed | **3/3 killed** |
| Pixel witness 480 / 800 | 188.32 / 96.89, control 0.00 | **188.32 / 96.89, control 0.00** |

The three scripts hard-code `/tmp/ccf363-*` paths. To avoid overwriting the author's scratch files they were run as copies with only those paths changed; the diff is saved as `rerun-path-substitutions.diff`. No large historic suite was rerun.

Assessment of strength:

- **No false positive.** Every assertion tests what its message says.
- **27 of the 67 checks assert code the candidate did not change** (zone, containment and scoring in `ScenarioOneRackBay`, `Derive`, the two world guards). They would pass on the baseline. They are fair regression checks but they are not evidence for the new feature.
- **Nothing tests `HudModel`, the objective card, guidance derivation, `AnimateGuidance`, the Annual Review view or the Work Plan view.** That is where B-1 and B-2 are. The author's row "Z4 OFFLINE VERIFIED" is supported only by the unchanged scoring checks, not by a check of the new feedback text.
- **Nothing tests figure label placement**, which is where B-3 is.
- The mutation script counts any unhandled exception as a kill. The removed-feeder mutant dies on `Single()` throwing, not on a named assertion. The kill is real; the discriminator is loose.
- The "endpoint parity" check converts the link's pixels back through the same transform that produced them. It proves the transform round-trips. Parity with the 3D mesh rests on the shared constants (confirmed by reading), not on that check.
- The pixel witness paints the plan on a flat synthetic background with Pillow. At side 800 its sample row falls on the dark dashes, which is why the figure halves to 96.89. The removed-feeder control of 0.00 is true by construction. As the author says, it is not Unity evidence and does not show real 720p or 1080p visibility.

## 7. Unity qualification — NOT RUN

Stop condition: **Phase A found source defects (B-1, B-2, B-3).** The packet says to stop before Unity in that case.

Independently of that, admission was not available: the user had not personally released the #361 Editor slot. #361 is open, last updated 02:10 UTC with feedback and no release.

Read-only observations at 08:44 UTC, for the next worker:

| Gate | Observed | Note |
|---|---|---|
| #361 personal release | Not given | Blocks Phase B on its own. |
| Lock tool | Present at `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`; not on PATH. Run as `python3 -B <path> status --json`. | The #363 worker reported it missing because it only searched PATH. |
| Lock state | `free`, no metadata, no Editor PIDs | Not acquired. |
| Editor process | None. Unity Hub is running. | Hub is not an Editor. |
| U3 | `/media/jer/ZX20/CCF-U3` at `fb599eec` on `qualify/scenario-one-onboarding-focus-20261109-v3`; 8 dirty paths: `.vscode/settings.json` and 7 Section Five `.mat` | Matches the packet's description. Hashes were **not** compared; that belongs to admission. Read with optional locks disabled so the index was not rewritten. |
| ZX20 free | 38,575,669,248 bytes (about 35.93 GiB) | Just above the 35 GiB entry floor. |
| Files free | 8,370,327,552 bytes (about 7.80 GiB) | Enough for screenshots and logs, not for a checkout. |
| Licence, 27 licensed assets, Reference archive, save isolation | Not checked | Not reached. |

No rendered evidence exists for this candidate. The 26 black #360 captures remain invalid. Historic results are unchanged and were not rerun or relabelled: strict Workbench runner exit 1 FAIL, StandMapParity FAIL ×2, save03b FAIL, UxPedagogy 38 FAIL, #360 171 PASS / 2 FAIL.

## 8. Evidence

Directory: `/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-364-IndependentReview-20261110/`

| File | SHA-256 |
|---|---|
| `compile-bfd8e2a4.txt` | `3eeceab750bc3a245a8316860ad87be2040021f3992bb8c1df28b38a7d3ace5b` |
| `checks-rerun.txt` | `b3cd977ddb970841b44eb5f10078eb889c4b604206dbf4962d7745f8e1b14c81` |
| `pixels-rerun.txt` | `a058aec2bdaa7e3a5c6530b271b828d1cd108fdd9cd45b1bc6c984f6c7200c45` |
| `mutations-rerun.txt` | `d70bac8de4a037e3135c32d15420a0ed200e3b4f193d5ac657d2723843cdbd35` |
| `rerun-path-substitutions.diff` | `e0d21bd40afdc8d8390aedd163a63406726852960f50e7d79835df62f251515e` |
| `probe/Probe364.cs` | `9719b71da0d91d0e63f421e9f3342550e291dcb608e215fd6712a1b8ab0202f8` |
| `probe/probe-output.txt` | `1c50d2e00396d6d54a623376e893baac5154510e6d4646ae8755f229f60efc95` |

`Probe364.cs` is a reviewer-written, verification-only console program. It references the compiled candidate assembly and calls its public plain-data functions. It is not in the repository, changes no production file and should itself be reviewed before anyone relies on it as acceptance evidence. Its B-1 and B-3 results can also be confirmed by reading the linked lines.

## 9. Recommendation and next bounded action

**NEEDS CHANGES for this feature.** The implementation is otherwise careful: geometry reuse, safety allowlist, provenance handling and world guards all read correctly, and the offline evidence is honest about its limits.

Suggested next task for the Manager to decide:

1. One small source repair on the existing feature branch by the source owner: B-1 and B-2 (one condition in `HudModel`), B-3 (three label boxes), and the N items the Manager wants. Estimated touch: `LearningObjectivesView.Sequence.cs`, `TeachingDiagramPlan.cs`, `StandMapLegend.cs`, and optionally `ScenarioOneUiRoot.cs`, `StandMapCanvas.cs`, `WorkPlanView.cs`, `LearningObjectivesView.Curriculum.cs`.
2. Add focused checks for exactly those repairs: HUD progress per lesson index and in the unread-review state, and the label-box intersection assertion.
3. A short delta review of that repair, then the single warm Tier 2 qualification, once the user has released #361 and the lock is taken through the real tool.

Open flags for the Manager:

- N-5 and the legend headings change what Legacy40 worlds show, slightly. Decide whether that is wanted.
- Later objectives in a V2 world now remove the primary style from Approve Work Plan and Advance in the Work Plan and offer no next target there. That follows the spec's "no pulse on commitment" rule, but it is a visible change to check in the render.
- Phase B will need a real Objective 2 state. A reflection-forced stage failed before; plan for an authentic progressed fixture or a played-through isolated save.

Nothing was merged, built or accepted. No Tier 3, Player build or main merge was started.
