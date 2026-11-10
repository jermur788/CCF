# #365 — Focused HUD and Objective 1 diagram repair result

**Date:** 10 November 2026. **Claim:** [#365](https://github.com/jermur788/CCF/issues/365). **Parent:** Manager #200.
**Binding packet:** [MANAGER-364-TO-365](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/MANAGER-364-TO-365-HUD-REVIEW-AND-DIAGRAM-BOUNDING-REPAIR.md).
**Repairs the findings of:** [#364 independent review](https://github.com/jermur788/CCF/blob/ed87e0b6286321be5815fa2e9c62166f294ac29b/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/INDEPENDENT-363-AUDIT-AND-FOCUSED-UNITY-QUALIFICATION-RESULT.md) (not edited).

## Status

**IMPLEMENTED in source and OFFLINE VERIFIED by the author. Not reviewed, not Unity-run, not integrated, not user accepted.** Manager decision requested: APPROVED / NEEDS CHANGES / REJECTED, after an independent delta review.

| Item | Result |
|---|---|
| B-1 wrong HUD in Objectives 3–5 | Fixed |
| B-2 Crop Tree prompt while a review is unread | Fixed |
| B-3 opaque figure labels over rack 4, its number, headland and link track | Fixed |
| N-1 to N-9 | All nine addressed in source. N-4 and N-9 carry a rendered-layout risk; see section 4. |
| Offline compile | 189 sources / 116 refs / exit 0 / 0 errors |
| Focused checks | **160 pass: the original 67 unchanged, plus 93 new** |
| Mutations | **7 typed mutants killed, each by its own named assertion (3 original + 4 new), plus 1 source-text regression refused** |
| Unity, lock, U3, saves | Not used |

## 1. Author, lane and commits

- **Author:** Anthropic **Claude Fable 5** (`claude-fable-5-2`), Claude Code harness in T3 Code, local session, no sub-agents. Harness reasoning-effort value 10 (label not exposed). The packet preferred the original OpenAI source owner; the user dispatched this session instead.
- **Independence:** this model also wrote the #364 review. It is now the repair author. **It must not perform the delta review.** The packet suggested an Anthropic delta reviewer on the assumption that OpenAI would do the repair; that should be reversed, with the delta review going to a different provider.
- **Lane:** reused `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, branch `feature/scenario-one-player-feedback-overnight-20261110-v1`. Before the claim: worktree and index strictly clean, no untracked files, local HEAD equal to the remote tip, one worktree on the branch, #363 closed, no earlier claim on #365. [RUNNING receipt](https://github.com/jermur788/CCF/issues/365#issuecomment-6763551183).
- **Starting HEAD:** `9cbe8be4d43a5e26332d4d8a5d8114e91e9da080` (report-only tip; its source equals `bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49`).
- **New source candidate (game + verifier, one commit):** `f6745d1082e4f4088e30964a9c1bb2389f0ceb49`. All checks below ran on a clean `git archive` export of this commit.
- **This report** is a later report-only commit on the same branch. Find it with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/FOCUSED-364-HUD-AND-DIAGRAM-REPAIR-RESULT.md`. It is not a new source test.

## 2. Files changed (`9cbe8be4` → `f6745d10`)

Nine production files, all under `Assets/ForestPrototype/UI/` and all on the packet's list:

```text
LearningObjectivesView.Sequence.cs     B-1, B-2, N-5
ScenarioOneRackBayText.cs              B-1, B-2, N-5 (the pure selector the packet allows for testability)
TeachingDiagramPlan.cs                 B-3, N-9
Map/StandMapLegend.cs                  N-1, N-2, N-3
Map/StandMapCanvas.cs                  N-4
ScenarioOneUiRoot.cs                   N-5, N-6
LearningObjectivesView.cs              N-6
WorkPlanView.cs                        N-7
LearningObjectivesView.Curriculum.cs   N-8
```

Six verification files under `Tools/Verification/PlayerFeedbackOvernight/`: `Checks.cs`, `run_checks.py`, `mutations.py`, `pixel_check.py`, `SOURCE-ALLOWLIST.md`, and new `hud_contract.py`.

Nothing else changed. No file was added or removed under `Assets/`, so no `.meta` or GUID changed. Nothing under `Assets/ForestPrototype/ScenarioOne/` changed: scoring, rack geometry, headland geometry and the manager are byte-identical. No scene, prefab, package, ProjectSettings, save, Reference archive or mask file is in the diff. Total: 15 files, 420 insertions, 49 deletions.

## 3. The three blockers

### B-1 and B-2 — walking HUD words

The choice of words now lives in one pure function, [`ScenarioOneRackBayText.HudWords`](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs#L58-L84), and [`HudModel`](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs#L256-L277) only passes facts in and words out. A pure function was needed because `HudModel` cannot be built without a running Unity scene.

What a Rack V2 player now sees on the walking panel:

| State | Title | Progress block |
|---|---|---|
| Objective 2, rack-year review unread | Review the year | `Rack work done · read the annual results` |
| Objective 2, no review waiting | lesson title | three Near / Middle / Far boxes, count, sides, spacing, then `[E] Inspect / [C] Mark Crop Tree` |
| Objectives 3, 4, 5 | lesson title | one location line (inside or outside the teaching area, which third, which side), then **that lesson's own progress**. No boxes, no `[C]`. |
| Objective 5, release-year review unread | Review the year | `Release work done · read the annual results` |
| Any lesson with the newest Annual Review unread | Review the year | `Annual results unread · read and acknowledge them`; instruction becomes "Read and acknowledge the Annual Review: open the Work Plan [Tab]." |
| Any world that is not Rack V2 (Legacy40, model 0, Reference preview) | unchanged | unchanged: exactly the heading, instruction and progress from before #363 |

Both review predicates are used: the lesson's own missing acknowledgement (`NeedsReviewAcknowledgement`) and the newest unread report (`ui.NeedsAnnualReview`). Stage indexes, completion predicates, the saved acknowledgement and scoring are untouched. A fourth Crop Tree still returns the lesson to Objective 2, as before; the difference is that the HUD no longer invites it. One check pins that unchanged rule.

Judgement call to confirm: I treat **any** unread Annual Review as "review first", including a later year's report during Objective 2. That follows the task wording ("when an Annual Review is unread") and is the more cautious reading.

### B-3 — figure labels

[`TeachingDiagramPlan.cs` lines 611–617](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/TeachingDiagramPlan.cs#L611-L617). Only the three reported boxes moved; "Forest track" and "Old path" are where they were. Units are the figure's design units, the same at every screen size.

| Label | Old box (x, y, width) | New box | Nearest obstacle after the move |
|---|---|---|---|
| Headland | 0, 180, 80 | 104, 100, 120 | headland casing, 7.8 units to its left; racks 5 and 4, about 29 above and below |
| Link track | 170, 356.5, 145 | 104, 344, 140 | rack 2, 4.4 above; link-track casing, 8.2 below; both pads, 3.5 |
| Existing clearing | 290, 145, 205, one line | 300, 176, 120, **two lines** | rack 4, 3.9 above; rack 3, 4.5 below; the clearing and track, 9 or more to the left |

"Existing clearing" is now two short lines on purpose. A single line needed about 224 units and could not fit between the track and the rack numbers without risking a wrap onto rack 3. My first position (x 290) was rejected by the new check because the track bends east there; x 300 is the measured result, not the packet's starting hypothesis.

The five unequal dashed racks, five hollow entrance targets, old-world guard and all access geometry are unchanged, and checks pin the first two.

## 4. The nine smaller findings

| Id | Status | What changed | Remaining risk |
|---|---|---|---|
| N-1 | Fixed | Entrance rows are grouped under "Your plan". | None known. |
| N-2 | Fixed | The hollow-entrance row appears whenever a hollow square is drawn, including suggestions only (0 of 5). It is now omitted when every drawn entrance is field marked. | None known. |
| N-3 | Fixed | Groups are ordered without `List.Sort`; rows keep insertion order inside a group. | None known. |
| N-4 | Fixed in source, **layout unproven** | Teaching-area words on the map are 17 units (was 12) and no longer stack on one another: "Near" grows left from its anchor, "Far" grows right, "Middle" is centred. | Whether they read well and stay inside the map at 1280×720 needs the Unity render. The alignment helper `TextSide` has no offline check because the class needs the Unity runtime. Only `StandMapCanvas.cs` was on the allowlist, so the label wording (in `StandMapDrawPlan.cs`) was not shortened. |
| N-5 | Fixed | Outside a Rack V2 world there is no new HUD border, no "Next:" prefix and no "Review the year" title. The baseline before #363 had none of these, which I checked before removing them. | The grouped legend headings still appear in every world. FR-M2 asks for them and the packet did not list them, so I left them. **Disclosed for the Manager.** |
| N-6 | Fixed | For Objectives 2–8 the reason line in the map, Work Plan and Annual Review windows is the one short instruction. The Objectives window shows none, because its card already has it. The extension checklist appears once on the card and is not repeated under "More detail". The safeguard warning stays visible. | 720p height of the card is a render check. |
| N-7 | Fixed | The total reads "before approval" only while some of the visit's work is Pending; otherwise "(approved work)". The amount and the scope sentence are the same quote in both states. | None known. |
| N-8 | Fixed | "…at the headland-side entrance of each rack". No "nearest the path". | None known. |
| N-9 | Implemented, **layout unproven** | Each of the four lifecycle states has the key's own shape above its words (dashed corridor, solid corridor, striped tree, open strip with stumps). The figure is 30 units taller (770) so nothing is squeezed. No new state and no renderer change. | Appearance needs the render. If it looks crowded, it is the easiest item to drop. |

**Deferred: none of the nine.** Not done, and outside this packet: the Work Plan still shows no primary button in later objectives (a #364 flag for the Manager, not a finding), and the map labels' wording.

## 5. Verification

Run on a clean export of `f6745d10`. Commands are in [`SOURCE-ALLOWLIST.md`](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Tools/Verification/PlayerFeedbackOvernight/SOURCE-ALLOWLIST.md).

| Check | Before (#363 source) | After (`f6745d10`) |
|---|---|---|
| Typed compile against the Player's Unity assemblies | 189 / 116 / 0 errors | **189 / 116 / exit 0 / 0 errors** |
| Focused assertions | 67 | **160 = 67 original + 93 new** |
| Typed mutants killed | 3 | **7** |
| Source-text regression refused | none | **1** |
| Pixel witness 480 / 800, removed-feeder control | 188.32 / 96.89 / 0.00 | **188.32 / 96.89 / 0.00** (unchanged; still not Unity evidence) |

The original 67 assertions are untouched and run first. The only edit to the old part of `Checks.cs` is where the pixel-witness plans are written (`CCF_FEEDBACK_SCRATCH`, defaulting to the old path), so a rerun no longer overwrites the #363 scratch files.

**New checks, by purpose** ([`Repair365` in `Checks.cs`](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Tools/Verification/PlayerFeedbackOvernight/Checks.cs#L114)):

- **HUD words (47):** every row of the table in section 3, including unread first-access review, Objective 2 with exactly three boxes and the `[C]` prompt, Objectives 3–5 keeping their own progress with no prompt, unread release-year review, a later unread review in each lesson, no player, no teaching area, lessons outside 2–5, the completed scenario, and twelve non-V2 cases whose words must come back unchanged.
- **Figure labels (25):** each of the five opaque labels must be at least 3 units clear of all five rack bands, all five rack numbers, the headland, link track and track casings, both pads and the clearing; wide enough for its longest line at the figure font; and inside the map column. Label height is taken as at least 30 units per line. Two more pin the five dashed racks and five hollow entrance targets.
- **Built-in negative controls (5):** the same measure is applied to the three #363 boxes and must report them: the old "Existing clearing" box on rack 4 and its number, hiding 48% of rack 4; the old "Headland" box on the headland and too narrow; the old "Link track" box on the link track.
- **Legend (7), lifecycle shapes (3), map text size, quote wording and entrance wording (6).**

**Mutations** (`mutations.py`; each mutant is compiled, and must now fail on its own named assertion rather than on any crash, which closes a weakness #364 noted):

| Mutant | Rejected by |
|---|---|
| unsafe-pulse, no-teaching-overlay, no-feeder (original three) | their original assertions |
| Crop Tree checklist restored for lessons 2–5 (the B-1 defect) | "Objective 3: no Objective 2 Crop Tree prompt or tick boxes" |
| Review state ignored (the B-2 defect) | "unread first-access review: review title" |
| Old "Existing clearing" box restored (the B-3 defect) | "figure label clear by 3 units: 'Existing clearing' touches rack4 band, rack number 4" |
| Entrance rows ungrouped (N-1) | "legend: suggestions only (0 of 5) lists the hollow entrance target under Your plan" |
| `HudModel` builds the checklist itself again (source text) | HUD source contract, 3 findings |

**Limits, stated plainly:**

- The HUD checks drive `HudWords`, not `HudModel`. The link between them is `hud_contract.py`, a fail-closed **source-text** check that `HudModel` calls `HudWords` once with the lesson index and both review predicates, passes all three words through, and that nothing else in the UI calls or spells the Crop Tree checklist. It also pins the N-5 and N-6 lines. This is a source contract, not a HUD screenshot.
- The figure checks use declared boxes and an assumed 0.6 of the font size per character. Real glyph widths, line height and wrapping are only known in Unity. When the figure is drawn much smaller than its design width, text keeps a 20-unit floor while the geometry shrinks, so clearances reduce.
- All these checks were written by the repair author. They need the independent delta review before anyone relies on them.
- No broad historic suite was rerun. Other historic checkers that pin #363 or earlier wording or label coordinates may flag these intentional changes; none was run, edited or relabelled.

Evidence: `/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-365-Repair-20261110/`

| File | SHA-256 |
|---|---|
| `compile.txt` | `4f967a31d456ff8d90a3e1c14075f3243569e3435d6f77db811e81cae42b5cea` |
| `checks.txt` | `ec11aa25bcfb9851c3917f56aacd16ad54847e99bc38e349df6a07971eb99f30` |
| `pixels.txt` | `a058aec2bdaa7e3a5c6530b271b828d1cd108fdd9cd45b1bc6c984f6c7200c45` |
| `mutations.txt` | `9ed38658992b06a6eccdb2910bf15eb35ea2a745cf15cdb1973146aa94a3d908` |
| `source.diff` (`9cbe8be4`→`f6745d10`) | `c57696fbf24b42e4233c8195d13357031873ab7287f653816cbc43854d20a56f` |

## 6. Protection and what was not done

- **Unity not run. No lock acquired. U3 not touched** (read once, read-only: still `fb599eec`; lock status `free`). #361 remains the user's.
- No save, preference, licence, Reference archive, original Workbench, scene or package file was read for writing or changed. The only writes were the 15 files above in the owned lane, this report, and evidence under the Files verification folder.
- No `__pycache__` or other untracked file was left in the lane. The new scripts disable bytecode writing.
- `active-tasks.md` not edited. No merge to main, no Player build, no new branch or worktree.
- The #363 and #364 reports and their raw receipts are unchanged. Historic results stand as they were: strict Workbench runner exit 1 FAIL, StandMapParity FAIL ×2, save03b FAIL, UxPedagogy 38 FAIL, #360 171 PASS / 2 FAIL, and the 26 black #360 captures remain invalid.
- Files free space after the work: about 7.8 GiB.

## 7. Next steps for the Manager to decide

1. A short **delta-only review** of `9cbe8be4`→`f6745d10` by a provider other than Anthropic, covering the nine production files and the new checks.
2. Then the single warm Tier 2 Unity qualification on `f6745d10`, only after the user releases #361 and admission passes through the real lock tool at `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`.

What that render most needs to look at for this repair: the walking panel height in Objectives 3–5 (it now has one extra location line), the three moved figure labels and the lifecycle shapes, the map's Near / Middle / Far words at 1280×720, and the Work Plan in a later objective.
