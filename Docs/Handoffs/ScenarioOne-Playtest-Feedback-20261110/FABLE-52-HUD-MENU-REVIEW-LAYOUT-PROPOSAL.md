# Fable 5.2 → Manager #200 — Scenario One HUD, menu and Annual Review layout proposal

**Date:** 10 November 2026. **Origin:** requested by the user in chat after [#362](https://github.com/jermur788/CCF/issues/362) closed. There is no task number or Manager packet for this work.

**Result: DESIGN PROPOSAL. Not implemented, not Unity-verified, not approved.** No production source, scene, save, spec, package or coordination file was changed. No merge, no Player build, no Unity session. The [#362 report](FABLE-52-SCENARIO-ONE-UX-ACCESS-DESIGN-REVIEW-RESULT.md) is unchanged.

| | |
|---|---|
| Worker | Claude Fable 5.2 (`claude-fable-5-2`, Anthropic), Claude Code harness in local T3 Code. Single owner, no nested workers. |
| Lane | Branch `docs/scenario-one-fable-ui-layout-proposal-20261110-v1`, cut from the #362 report commit `0d195232`. Docs-only; this file is the only change. |
| Visual baseline | Earlier captures of the `fb599eec` candidate (CCF-358 evidence). I have **not** seen the current candidate running. |
| Source check | Read at `4db486db` (the candidate named in the #371 packet). `TreeInspectionView.cs`, `WalkingHudView.cs` and `ForestTreeMarkingManager.cs` are identical to `fb599eec`, so the inspection HUD analysed here should still be current. `AnnualReviewView.cs`, `WorkPlanView.cs`, `ScenarioOneUiRoot.cs` and the objective views **did** change in #363–#370; see section 5. |
| Images | Ten PNG mock-ups in `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/` (section 8). They are not in Git, under the storage policy. Every design is also described in words below so this file stands alone. |

## 1. The idea in short

The user's complaint is that the HUD looks cluttered. The cause is not the amount of information; it is that everything is shown at once, with equal weight, in six separate panels.

The proposal is one rule applied three times (HUD, menus, Annual Review):

1. **Show what the player needs to decide now.** For an inspected Crop Tree that is three questions: how is this tree doing, who is crowding it, and what does my plan do so far.
2. **Say each thing once.** One objective line, one key bar, one plan status.
3. **Put the rest one key away.** The existing `[V] More detail` card already exists for "everything that is not needed to decide" (`TreeInspectionView.cs:181`).
4. **Bring a figure back when it matters.** A value moved off the default view reappears when a mark changes it or it triggers a warning.

This is the same direction as the Manager ruling's item 6 ("reduce visual clutter without discarding important caveats") and postscript S8 (progressive disclosure). It adds a concrete layout for them.

## 2. What is wrong today

Evidence: capture `07-selected-competitor-ring-1920x1080.png` (CCF-358 run, copied to the image folder as `original-hud-current-build-1920x1080.png`). The player is in Objective 1 and has pressed E on a Crop Tree.

| Problem | What the capture shows |
|---|---|
| Six panels at once | Tree card, objective card, competitor panel, centre prompt, plan-effect strip, status strip. Together they cover most of the left, right and bottom of the view. |
| Repeats | "Racks planned: 0 / 5" appears three times. The Stand Map and Work Plan key hints appear in three places. |
| A pointer between panels | "Competitors are listed on the right." The layout needs a sentence to explain itself. |
| Raw model numbers | "Competition now 5.91", "6.26 → 6.26 (0 %)", "gap light 0.00 → 0.00", "opening 1.0 of 2". None says whether the number is good or bad. |
| Dense strips | Two bottom strips hold nine facts as running text separated by dots. |
| Unclear phrase | "0 lose at least a tenth". In source this is `CropTreesReleased` (`WalkingHudView.cs:359`): the number of Crop Trees whose competition falls by at least a tenth. |

## 3. Recommended HUD: one decision card, with shares shown on the trees

Images: `hud-clean-2-decision-card.png` (the card) and `hud-clean-1-in-the-forest.png` (the on-tree labels). The recommendation is the card from the first plus the labels from the second.

```
+--------------------------------------------+               +--------------------------------+
| NOW  Create access racks · 0 of 5 · [M] Map|               | Sitka spruce      [◆ CROP TREE]|
+--------------------------------------------+               | 17.8 cm · 13.5 m tall · 20 yr  |
                                                             | Crowded. Neighbours hold back  |
        [9 %]      [13 % · ◆ Crop Tree]   [6 %]              | about 54 % of its growth.      |
         (2)              (1)              (3)               | [==========------]             |
                                                             |                                |
                    (forest view,                            | WHO IS CROWDING IT             |
                     rings and paint                         | 1 P1009 ◆ Crop Tree 1.2 m  13 %|
                     marks unobstructed)                     | 2 P0910             1.6 m   9 %|
                                                             | 3 P1109             2.7 m   6 %|
                                                             | 4 P0909             2.5 m   6 %|
                                                             | 5 P0908             3.5 m   4 %|
                                                             |   27 other trees  [======] 61 %|
                                                             | No single neighbour dominates. |
                                                             | The largest that is not a Crop |
                                                             | Tree is 9 %.                   |
                                                             |                                |
                                                             | YOUR PLAN SO FAR               |
                                                             | Marked        FELL 1  ◆ CROP 2 |
                                                             | Pressure on Crop Trees         |
                                                             |                 no change yet  |
                                                             | Wind exposure         ● Stable |
+----------------------------------------------------+       +--------------------------------+
| [1–5] Show in forest  [V] Full detail  [E] Close   |
+----------------------------------------------------+
```

What changes, element by element:

| Element | Today | Proposed |
|---|---|---|
| Objective | Card with title, a two-line instruction and a repeated progress line | One line: title, progress, the one key that advances it. `Details [O]` still opens the full objective. In Objective 2 this line must grow to hold the live checklist the ruling approved (item 1). |
| Selected tree | Separate left card, four large value tiles, a paragraph | Top of the single card. Size on one line; "Crowded" and the 54 % as one sentence with a bar. |
| Competitors | Separate right panel: two explanatory paragraphs, five-column table, three summary rows, a hint and a footnote | Ranked list: number, ID, distance, share. The 27 others as one bar, so the spread is visible. One conclusion line stated as a fact. |
| Plan effect | Two text strips with nine facts | Three rows: what is marked, the effect on Crop Trees in words, wind exposure. |
| Keys | Centre prompt, plus hints inside three panels | One bar listing only the keys that work in the current state. |
| In the forest | Numbered markers only | Each visible numbered marker also shows its share. The neighbour that is itself a Crop Tree says so. |

Where the removed items go:

| Item | Proposed home |
|---|---|
| Tree ID and "original plantation" | `[V]` detail |
| Neighbour DBH column | `[V]` detail |
| "Also within 8 m of other Crop Trees: #2 (1) …" | `[V]` detail |
| Competition index values (5.91, 6.26 → 6.26) | `[V]` detail. The default view says "no change yet" or the percentage change. |
| Volume kept, growth, gap light, opening | `[V]` detail while unchanged. Shown on the card when a mark changes them or a limit is near. |
| "Nearby trees that reduce its growing space. Being small or suppressed does not by itself make a tree a problem." | `[V]` detail and the objective's `Details [O]` |
| "Shares come from the size and distance of trees within 8 m. Strength describes share only; it is not advice." | `[V]` detail. The card's conclusion line is kept factual for the same reason. |
| "Competitors are listed on the right." | Deleted. One card needs no pointer. |

## 4. Alternatives considered

| Image | Design | Why it is not the recommendation |
|---|---|---|
| `hud-clean-1-in-the-forest.png` | No side panel. A small card beside the selected tree, shares on the trees, one top line and one bottom line. | Cleanest, but distances are gone and neighbours 4 and 5 are only a segment in a bar. Its on-tree labels are taken into the recommendation. |
| `hud-alt-a-side-rail.png` | All of today's text kept, rearranged into one right-hand column plus a "Your marks" card. | Tidier, still dense. Useful as the fallback if nothing may leave the default view. |
| `hud-alt-b-top-band.png` | All of today's text kept, in one band across the top, with a row of tiles at the bottom. | Leaves the forest floor clear but letterboxes the view and gets crowded with longer text. |
| `scenario-one-hud-mockup.png` | First sketch, for Objective 2: live checklist, "You are in" line, minimap with headland and teaching area. | Drawn on a placeholder backdrop with illustrative values. Its checklist and minimap ideas match ruling items 1 and 4 and still apply. |

## 5. Menus and Annual Review

These two mock-ups were drawn against older captures (Work Plan from the CCF-358 run; Annual Review from `PlaytestCandidateA1E15B52-20261104`). Both views changed in #363–#370, and I have not compared the mock-ups with the current candidate. Treat them as layout direction only.

**Shared menu shell** (`scenario-one-workplan-mockup.png`):

- The same four tabs on every full screen, in the same order: Objectives, Stand Map, Work Plan, Annual Review.
- Year, cash, Help and one exit, always called "Back to forest [Esc]", in the same corner. The older captures used three different names for leaving.
- One "NEXT" strip directly under the tabs on every screen.
- Work Plan: cash shown once; locked sections collapsed to one line; the small-job minimum explained before approval (ruling S7); Approve and Advance laid out as step 1 and step 2, with Advance disabled until the plan is approved.

**Annual Review** (`scenario-one-review-mockup.png`):

- Three headline tiles first (work done, money, forest), then the detail.
- Provenance chips hidden by default, with a "Show data sources" tick box (ruling item 3; needs the S1-C FR-4 amendment).
- A result line for the year. The older capture listed income and costs without a total.
- One primary button, "I've read the annual results" (ruling S6).

**Two places where my mock-ups conflict with the Manager ruling. The ruling governs.**

1. The Work Plan image draws a highlight ring on "Approve Work Plan". The ruling says commitment controls never pulse. Read the ring as a static mark at most, or drop it.
2. The Annual Review image disables "Back to forest" until the results are acknowledged. Ruling S6 keeps it available as a secondary action. It should stay enabled.

## 6. How this maps to the Manager ruling

| Proposal | Ruling item it serves | Status |
|---|---|---|
| Declutter the inspection HUD; move detail behind `[V]` | Item 6, S8 | Layout is new; direction already approved for specification |
| One key bar showing only valid keys | Item 2 (explicit HUD action text) | Consistent |
| Objective line that expands to a checklist in Objective 2 | Item 1 | Consistent |
| Minimap with headland and teaching area | Item 4 | Needs the D-072 minimap scope amendment the ruling already names |
| Annual Review chips hidden, one primary button | Item 3, S6 | Consistent, apart from conflict 2 above |
| Small-job warning before approval | S7 | Consistent |
| **Tree card and competitor panel merged into one card** | none | **New. Needs a decision.** |
| **Share labels on the numbered forest markers** | none | **New. Needs a decision.** |
| **Raw index values off the default HUD** | none | **New. Needs a decision.** |
| **Plan strip shows only changes and warnings** | none | **New. Needs a decision.** |
| **Shared menu shell with common tabs** | none | **New. Needs a decision.** |
| **Annual Review headline tiles and year result line** | none | **New. Needs a decision.** |

## 7. What a specification would need to settle

**Likely source touch set** (read, not edited):

- `UI/TreeInspectionView.cs`: tree card, competitor panel, "If felled now" panel, `[V]` detail card.
- `UI/WalkingHudView.cs`: plan-effect and status strips, centre prompt.
- `ForestTreeMarkingManager.cs`: it builds the "If felled now …" sentence as one string (line 489). Showing those values as separate rows means the UI needs the numbers, not the sentence.
- `UI/Resources/ScenarioOneUi.uss`: layout classes.
- The code that draws the numbered forest markers, for the on-tree labels. I did not locate or read it.
- Menu shell: every full-screen view (`LearningObjectivesView`, `StandMapView`, `WorkPlanView`, `AnnualReviewView`) and `ScenarioOneUiRoot.cs`. This overlaps the #363–#370 files and is the largest piece.

**Risks and guards:**

- **Key bar must follow the game state.** My HUD images show `[X] Mark to fell`. In the captured state the real hint line omits the marking keys, and `TreeInspectionView.cs:156` blanks them while `RackFirstStageActive` is true. So the images are likely wrong for Objective 1 on this point. The real keys are `[X] Mark to Fell` and `[C] Retain as Crop Tree`.
- **The `[V]` card cannot scroll.** A source comment says the cursor is locked while inspecting. Everything moved there must fit one fixed card, or the detail needs pages.
- **Nothing important may become unreachable.** Wind exposure, opening limits and any warning must return to the default view when they change. "(storms off)" was dropped from my wording and may need to stay.
- **No advice.** The current HUD states that share strength "is not advice". The proposed conclusion line states only what the table shows.
- **Approved wording.** I did not check which approved requirements fix the current HUD strings. Any that do need an explicit amendment, as with S1-C FR-4.
- **Screen sizes.** The mock-ups are drawn in pixels at 1920 × 1080. The UI panel scales from a 1600 × 900 reference, so sizes must be converted, then checked at 720p and in grayscale.
- **Values and wording in the images.** HUD values are copied from capture 07; the wording is mine. "No change yet" is my reading of "6.26 → 6.26 (0 %)". The first HUD sketch, the Work Plan and the Annual Review use illustrative figures.
- **Backdrop.** The four newer HUD images use a backdrop stitched from two real captures, with the old panel areas filled in. It is not a render of any new state.

**Decisions needed from the user or Manager:**

1. Which HUD direction: the decision card with on-tree labels (recommended), the minimal in-forest version, or a rearrangement that keeps all text.
2. Which facts must stay on the default HUD and which may move behind `[V]`.
3. Whether on-tree share labels are wanted. They are a new in-world element.
4. Whether this joins the existing P0/P1 specification packages or becomes its own "HUD declutter" specification.
5. Whether to adopt the shared menu shell, given its overlap with the #363–#370 changes.

## 8. Image files

All in `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/`, each with its editable HTML source alongside.

| File | What it is |
|---|---|
| `original-hud-current-build-1920x1080.png` | The real capture, for comparison (byte-identical copy of CCF-358 capture 07) |
| `hud-clean-2-decision-card.png` | Recommended card |
| `hud-clean-1-in-the-forest.png` | Minimal version; source of the on-tree labels |
| `hud-alt-a-side-rail.png` | All text kept, side rail |
| `hud-alt-b-top-band.png` | All text kept, top band |
| `scenario-one-hud-mockup.png` | First sketch, Objective 2 walking HUD |
| `scenario-one-workplan-mockup.png` | Shared menu shell on the Work Plan |
| `scenario-one-review-mockup.png` | Annual Review |
| `scenario-one-objective1-diagram-mockup.png` | Objective 1 teaching diagram (earlier request) |
| `hud-backdrop-composite.png` | Stitched backdrop used by the four newer HUD images |

## 9. Not done

- No Unity session, so no proposed layout has been seen in the game.
- No comparison with the current candidate's running UI.
- No issue was opened or edited, and #200 and #361 were not touched.
- No specification was drafted.
