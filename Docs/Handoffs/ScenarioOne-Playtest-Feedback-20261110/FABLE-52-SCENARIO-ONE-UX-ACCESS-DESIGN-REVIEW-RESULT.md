# Fable 5.2 → Manager #200 — Scenario One playtest UX and access-layout design review

**Date:** 10 November 2026. **Task:** [#362](https://github.com/jermur788/CCF/issues/362). **Binding packet:** [MANAGER-362](MANAGER-362-FABLE-52-GAMEPLAY-UX-AND-ACCESS-LAYOUT-REVIEW.md).

**Result: REVIEW PROPOSAL / NEEDS MORE EVIDENCE.** Nothing here is implemented, verified in Unity, or approved. No production source, scene, save, spec, package or coordination file was changed. No merge, no Player build.

| | |
|---|---|
| Worker | Claude Fable 5.2 (`claude-fable-5-2`, Anthropic), Claude Code harness in local T3 Code. Reasoning-effort setting is not exposed to the worker. Single owner, no nested workers. The work ran in two sessions because the first hit a usage limit; the second re-read every source cited below. |
| Lane | Branch `docs/scenario-one-fable-feedback-review-20261110-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-FableScenarioOneUXReview-20261110`, starting HEAD `913e939c` (origin/main). Docs-only; this file is the only change. |
| Source basis | Tested candidate `fb599eecde983552dec4f06fb9efcf8e38397aed`, read in place from `/media/jer/ZX20/CCF-U3` (HEAD confirmed, C# clean; the 8 known dirty paths are `.vscode/settings.json` and 7 SectionFive `.mat`, untouched). |
| Unity | **Not run.** See section 6. All visual statements come from earlier captures, named where used. |

## 0. Short answers

1. **Objective 2 zones.** The playtest observation is correct. The three zones exist only as arithmetic. Nothing on the Stand Map, the minimap, the objective picture or in the forest shows where they are. The only way to find them is to press E on trees one at a time and read a line of text. This is a first-test progression blocker.
2. **Does "one tree in each of three zones" teach crop-tree marking?** Only half of it. It teaches *spread your choices out*. It does not teach *how to judge a tree*: the gate checks position only, so any three stems in the right boxes pass. I recommend keeping three trees (lessons 3–5 and the MFM sample depend on it) but making the area visible and rewording the rule around the idea rather than "zones". Changing the rule itself is a separate, spec-level choice (section 2, A-full).
3. **Annual Review labels.** Every line carries a chip (RECORDED, CURRENT, DERIVED, RECONSTRUCTED, SESSION-ONLY, NOT RECORDED) and a legend sits at the top. There is no "Confirmed" chip in the Annual Review source; the user most likely saw RECORDED or CURRENT. Worth confirming with the user. Hiding the chips by default is straightforward, but spec S1-C FR-4 says derived and reconstructed values MUST be labelled, so it needs a small approved amendment.
4. **Next-action button.** The guidance code stops after Objective 1 (`else return;`), so from Objective 2 on there is no "Next:" badge at all, and nothing ever pulses. There is also no guidance while walking in the forest, which is where Objective 2 is done. This is the second progression blocker.
5. **Diagram and legend.** The Stand Map legend shows the forest path and the headland with the same swatch, uses the internal term "V2", and in grayscale the planned-rack swatch is indistinguishable from both. The HUD minimap does not draw the headland at all, although every rack starts on it.
6. **Headland.** It is authored starting scenery. Nobody clears it: trees are simply never generated there, and its surface is a flat mesh that reuses the path material. No cost, no work order, no history entry, and no player-facing sentence explains it. The feeder exists only to join it to the one way in. The current layout is already a T (feeder meets headland at a right angle); what makes it read as arbitrary is that the old trail carries on north through the stand and three of the five racks cross it.
7. **Recommended access direction.** Now: explain and restyle, no topology change (options A + B). If the user still wants a simpler network: a short gateway through the **west** boundary straight onto the existing headland (option C1) keeps all five racks, the rack records and the teaching area unchanged. That is a candidate supersession of D-071/D-072 and needs a user decision and a new approved spec.

## 1. Verified observations

Evidence levels: **S** = read in source at `fb599eec`. **R** = seen in an earlier rendered capture (path given; the #358 captures are from `045966d2`, whose `Assets/` differ from `fb599eec` only by 10 lines in `ScenarioOneOnboardingView.cs`). **O** = my own offline arithmetic on an archived save. **U** = not inspected.

| # | Complaint | What the source and evidence show | Level | Class |
|---|---|---|---|---|
| 1a | Zones cannot be found | Zone = third of a 24 m strip along the teaching rack: `Frame.Zone` in `ScenarioOne/ScenarioOneRackBay.cs:382`. No draw call for it exists outside the Objective 6 Treatment review (`UI/Map/StandMapAreaFootprints.cs:44,71`). The only location text is in the tree inspection panel after pressing E: "Teaching rack: near zone, right side." (`UI/LearningObjectivesView.Sequence.cs:139-148`). | S | Teaching/UX deficit, P0 |
| 1b | Objective 2 picture | Two vertical racks, two marked trees, no zones, no third tree, no sides. Real racks run east–west. Capture: `Verification/CCF-358-V2MapParityFinalUnity-20261109/run/uxpedagogy/evidence/05-objective-2-top-1920x1080.png`. | R | Teaching deficit, P1 |
| 1c | Objective 2 in the forest (HUD, marking feedback) | HUD line is "Choose one Crop Tree in each zone, 8 m apart; include both rack sides." and progress "Selections n/3 · zones n/3 · both sides needed" (`UI/ScenarioOneRackBayText.cs:188,201`). Which zone is missing is shown only on the Objectives card. | S; render U | Teaching/UX deficit |
| 2 | Annual Review chips | `AddTagged`/`TaggedLine`/`Tag` in `UI/AnnualReviewView.cs:246-282`, about 60 call sites; legend at line 132. Capture (older commit `a1e15b52`, same chip code): `Verification/PlaytestCandidateA1E15B52-20261104T155338Z/gate-objective1/evidence/c11-annual-review-1920x1080.png` shows 15 chips on the first screenful. | S, R (older) | Presentation, P1 |
| 3 | No conspicuous next action | `RefreshNextActionGuidance`, `UI/ScenarioOneUiRoot.cs:336-461`. Line 441: `else return; // Later-objective mappings and Legacy40 are outside this first guidance slice.` Scope is null while walking (line 350-353). Styling is a static 3 px border and a "Next:" label (`UI/Resources/ScenarioOneUi.uss:589-592`); the stylesheet has no transition for it and the code has no timer. | S; Objective 1 badge R | Missing feature, P0 |
| 4a | Map legend | `UI/Map/StandMapLegend.cs:72-74` adds "Forest path" and "Existing V2 headland / feeder — not a planned rack", both with `MapSymbol.Path`. Capture: `Verification/CCF-359-IndependentAuditPlaytestClosure-20261109/screens/gray/d-stand-map-confirmed-1920x1080-gray.png`. | S, R | Presentation, P1 |
| 4b | Minimap | Headland drawn only `if (full)` (`UI/Map/StandMapDrawPlan.cs:115`). In the 3D capture the minimap shows five racks and the trail, no headland: `.../run/headland-access/evidence/path-feeder-junction-1920.png`. | S, R | Presentation gap (D-072 excluded the minimap on purpose) |
| 4c | Objective 1 figure | Map occupies under half of the frame; one tan key row covers headland, feeder, track and clearing; letters A/B/C are small; all five racks are drawn as planned while the card says "Racks planned: 0 / 5". Capture: `.../uxpedagogy/evidence/05-objective-1-picture-1920x1080.png`. | R | Presentation, P2 |
| 5 | Headland origin | `ScenarioOne/ScenarioOneHeadlandGeometry.cs` (constants), `ScenarioOneHeadlandPresentation.cs` (runtime mesh), `ForestStartingStand.cs:247-252` (stem mask). No occurrence of "headland" or "feeder" in onboarding or help copy. | S; 3D R | New product choice |

Screens I did not see at all: Objective 2 while walking, the inspection panel in the teaching area, the Annual Review at `fb599eec`, the deep-history view, and anything from the user's own session.

### 1.1 Objective 2 rules as actually coded

- **Count:** exactly three Crop Trees inside the footprint (`HasEnoughFuture`, `ScenarioOneRackBay.cs:307`). A fourth inside the footprint blocks completion. Crop Trees elsewhere are allowed and ignored.
- **Footprint, Rack V2 world:** built on the **third** confirmed rack counted across the racks (`ScenarioOneRackSampleGeometry.cs:260-271`). Centre = the point on that rack's centreline closest to the stand centre (0, 0), shifted if needed so 12 m fits each way. Size 24 m along the rack, 9 m each side. All confirmed rack corridors are excluded.
- **Zones:** thirds of 8 m along the rack, measured from the entrance end. **Near = the third closest to the headland.** Far = deepest into the stand. Nothing tells the player what "near" is near to.
- **Sides:** sign of the across-rack distance. The inspection text says "left"/"right", which only makes sense if you imagine standing at the entrance looking along the rack. Never explained.
- **Spacing:** at least 8 m between stem centres. A zone is also 8 m long, so two picks close to a shared zone boundary fail.
- **Legacy40 / pre-V2:** the middle authored layout line, x from −12 to +12, half-width halfway to the neighbouring lines (`ScenarioOneRackBay.Analyse`). Same thirds. Not in scope to change.
- **Persistence:** derived from live trees and orders each time; survives save/load with no save field.

With the suggested layout in the archived fixture world (`Verification/RackV2-Combined-20261027T1334Z/.../forest-save.json`, SHA-256 `e8bb96f4…`, the same fixture `TeachingDiagramPlan` cites), the teaching rack is at z = 0.24, so the footprint is x −12…+12, z −8.8…+9.2: the middle of the stand. **(O)** In that world it holds 75 living trees: near 29, middle 19, far 27. The middle third is thinner because the old trail and the clearing pass through it. Of 14,877 possible one-per-zone triples, 7,505 (50%) also satisfy spacing and both-sides. So a player who has found the zones still fails about half the time unless spacing feedback is visible where they are working. This count is for that world only; the user's world has its own tree positions.

## 2. Proposals by priority

Each item gives a preferred minimal patch and, where one exists, a fuller redesign.

### P0 — first-test progression blockers

**A-min. Make the teaching area visible and name it in plain words.**
1. *Stand Map.* While the current lesson is 2–5 and the rack-area analysis applies, draw the footprint rectangle with its two dividing lines on the Access layer and the Fell & crop marks layer. Print "Near", "Middle", "Far" inside the thirds and "headland end →" at the near edge. Outline dashed with a hatch fill so it does not depend on colour. Crop marks are shown inside it on both layers. Legend row: "Teaching area for Objectives 2–5". Side card: "Teaching area: beside your third rack. Choose three Crop Trees here." with a **Set waypoint to teaching area** button (the existing waypoint, `StandMapView.SetWaypointAtWorld`). The polygon already exists in code as `StandMapAreaFootprints.TeachingPolygon`.
2. *HUD while walking.* Replace the single sentence with a live checklist, updated as the player moves and marks, no key press needed:
   `You are in: Teaching area · Middle third · north side` (or `Outside the teaching area`)
   `Near ☐  Middle ☑  Far ☐   ·   Both sides ☐   ·   Spacing ✓`
   Use compass words for sides (taken from the rack's normal), matched by "N side / S side" labels on the map, instead of left/right.
3. *Feedback at the moment of marking.* When C is pressed on a tree in the footprint, say why it does not count if it does not: "Less than 8 m from P0714", "This is a fourth Crop Tree in the teaching area; the lesson needs exactly three", "Same third as P0714". The mark is still applied. The game never refuses or suggests a stem.
4. *Wording.* Proposed "What to do": "Walk to the teaching area beside your third rack (shown on the Stand Map [M]). Inspect trees [E] and compare them. Mark three Crop Trees [C]: one in the third nearest the headland, one in the middle third, one in the far third. Keep them at least 8 m apart and use both sides of the rack."
5. *Minimap.* Thin outline of the footprint only. This needs to be named in scope explicitly, as D-072 did for minimap changes.

Gameplay effect: no rule changes. The player can complete the objective by reading the map and HUD rather than probing trees. Judgement about which tree stays with the player.

**A-full (spec required, user choice).** Change what the lesson asks, for example: three Crop Trees in the marked area, at least 8 m apart, both sides, with the thirds shown as guidance and reported as feedback rather than gating; and require at least two inspected candidates per choice so that comparison, not position, is what is practised. This touches `ScenarioOneRackBay` (pure rules), the six-portion review in lesson 5, the MFM sample calibration (D-065), offline checks and the design note `Docs/Design/ScenarioOneRackBayLessonV1.md`, which records the thirds as deliberate calibration against a "clustered counterexample". I do not recommend it before the next playtest.

**C-min. Extend next-action guidance past Objective 1 and add motion.**
- Extend the finite mapping to lessons 2–5 first (table in section 3.3), then 6–9.
- Add a forest cue: when the next step is a physical action, the HUD objective card shows a "DO THIS IN THE FOREST" line with the key, for example `[E] inspect · [C] mark Crop Tree`, and the card border pulses. On the Objectives screen and Stand Map the pulsing button is then **Back to forest**.
- Pulse exactly one element. Details and safety tiers in section 3.3.

### P1 — instructional clarity

- **B-min.** Hide provenance chips and the legend by default; keep uncertainty in ordinary words; add one "Show data sources" switch that brings chips and legend back. Section 3.2.
- **D1.** Legend: distinct symbols and plain names; remove "V2". Section 3.4.
- **D2.** Draw the headland and feeder on the HUD minimap (explicit scope decision needed).
- **A-picture.** Replace the Objective 2 picture: one horizontal rack, the footprint with three labelled thirds, three marked trees on both sides, an "8 m" spacing bar.
- **E-A.** Say what the headland is and that it was there before the player arrived. Section 4.3.

### P2 — presentation

- **D3.** Re-lay the Objective 1 figure (larger map, names on the map, lifecycle strip). Section 3.4.
- **A-stakes.** In-forest corner posts and divider stakes for the teaching area: runtime presentation objects with no colliders and no save data, like the headland surface. Not paint on trees, which would read as a mark. Needs Unity visual qualification.
- **B-full.** Reorder the Annual Review into a year summary first (section 3.2).
- **E-B.** Restyle the old trail north of the feeder junction as a footpath. Section 4.3.

### Spec-required topology choice

- **E-C1 / E-C2 / E-D.** Section 4. None should start without a user decision and a new approved four-file spec.

## 3. Player flows and wireframes

### 3.1 Objective 2 zone discovery

```
Annual Review acknowledged
   └─ Objectives [O]: lesson 2 card, new picture (rack + Near/Middle/Far + 3 trees)
        Next: [ Stand map [M] ]  (pulsing)            "Find the teaching area on the map."
   └─ Stand Map: teaching area outlined, thirds labelled
        Next: [ Set waypoint to teaching area ] → then [ Back to forest ] (pulsing)
   └─ Forest: waypoint leads to the area
        HUD:  NOW · 2. Choose promising trees
              You are in: Teaching area · Near third · south side
              Near ☐  Middle ☐  Far ☐ · Both sides ☐ · Spacing –
              DO THIS IN THE FOREST: [E] inspect · [C] mark Crop Tree
   └─ Mark 1..3: checklist fills; a mark that does not count says why
   └─ All conditions met: HUD "Objective 2 complete"; Objectives card moves to lesson 3
```

Stand Map sketch (north up; racks east–west; the area sits on rack 3):

```
        ┌ ─ ─ ─ ─ ┬ ─ ─ ─ ─ ┬ ─ ─ ─ ─ ┐   N side
        ╎  NEAR   ╎ MIDDLE  ╎   FAR   ╎
 ═══════╪═════════╪═════════╪═════════╪═══════  rack 3 (corridor, not part of the area)
        ╎         ╎         ╎         ╎
        └ ─ ─ ─ ─ ┴ ─ ─ ─ ─ ┴ ─ ─ ─ ─ ┘   S side
 ← headland end        8 m each        24 m × 18 m
```

Acceptance checks (for a later spec; none run):
- At lessons 2–5 in a Rack V2 world the full map shows one footprint whose corners equal `TeachingPolygon(sample.Teaching)` within one pixel, for axis-aligned and rotated plans; not shown at lesson 1, at lesson 6+, in Legacy40, or in Reference Preview.
- The thirds are distinguishable in a grayscale capture and each carries a text label at 1280×720 no smaller than the UI minimum text size.
- Walking across a divider changes the HUD third within one refresh; leaving the area shows "Outside the teaching area".
- Each failing condition (too close, fourth tree, same third, one side only) produces its own sentence; the mark is still applied; no stem is ever highlighted as "correct".
- Negative: a Fell mark never changes the checklist; a Crop Tree outside the area never fills a box.
- Save and reload: outline and checklist return identical; no new save field.

### 3.2 Annual Review

What must stay true: the provenance value stays on every line in the model (`HistoryProvenance`), nothing is rebuilt from today's forest, and anything that changes how a number should be read stays visible.

| Kind | Default view (proposed) | With "Show data sources" on |
|---|---|---|
| Recorded | No chip. | RECORDED chip |
| Derived | No chip. | DERIVED chip |
| Current | No chip; the sentence keeps the word "now" (already present in most lines, e.g. "Browsing pressure low now"). | CURRENT chip |
| Reconstructed | No chip; line begins "Estimated from other records:" and the existing storm note stays. | RECONSTRUCTED chip |
| Session-only | No chip; "This session only:" prefix and the existing note stay. | SESSION-ONLY chip |
| Not recorded | No chip; the words "not recorded" stay exactly as now. | NOT RECORDED chip |

Proposed order of the page:

```
Annual review · Year N
[ This year in brief ]   3 lines: work carried out · net money · one forest change
[ Work done ]  [ Money: net result first, then lines ]  [ Forest ]
[ Objectives reached this year ]
[ Management history ]  (collapsed; year browsing and drill-down as now)
                                         ( ) Show data sources
[ I've read the annual results ]
```

Test contract. `Tools/Verification/S1CManagementHistory/AnnualReviewProvenanceVerification.cs` and the drill-down and guidance verifiers find chips by the classes `prov` and `prov-<kind>` and require them to be visible. Proposal: keep creating every chip exactly as now and hide them with one root class (for example `.review:not(.show-provenance) .prov { display: none; }`). Verifiers switch the toggle on before the existing checks, so no existing check is deleted. Add three checks: chips are not visible by default; the words "not recorded", "Estimated from other records" and "This session only" are visible without the toggle; toggling changes no fact text. The first minimal patch could be done with the toggle and prefixes alone and leave the page order for B-full.

Spec impact: `specs/scenario-one-review-history-v1/spec.md` FR-4 says a derived or reconstructed value "MUST be labelled as such". Default-hidden chips for *derived* values need FR-4 amended to accept wording plus an on-request label. That is a small amendment but it is an approved MUST, so it is Manager/user approval, not a worker's call.

720p: removing the chip column gives each line roughly 110–140 px more width in the three-column layout (estimated from the 1920 capture, not measured). The deep-history view uses the same helpers, so one switch covers it; I have not seen it rendered.

### 3.3 One pulsing next action

**Tiers.**

| Tier | Examples | Treatment |
|---|---|---|
| 1. Safe navigation / read receipt | Stand map, Back to forest, Open Work Plan, Treatment review tab, Set waypoint, "I've read the annual results", decline/cancel buttons in open questions | "Next:" badge, heavy border, **pulse** |
| 2. Commitment | Confirm Rack Plan, Approve Work Plan, Advance year, Approve prescription | "Next:" badge and the reason line, **no pulse**, keyboard focus is not moved onto it, existing confirmation steps unchanged |
| 3. Physical action, no button | Mark entrance, inspect, mark Crop Tree, mark competitor, [J]/[K] review | HUD "DO THIS IN THE FOREST" line with key names; HUD card border pulses |
| Never | Fell marks, anything destructive, anything in Reference Preview | No guidance emphasis |

Note: today `workplan-approve`, `workplan-advance` and `map-rack-plan-confirm` do receive the static "Next:" emphasis during Objective 1 (accepted behaviour from the #265 spec). Under this proposal they keep that and simply never gain motion.

**Motion.** One slow pulse of border and background tint, about 1.2 s per cycle, low contrast change (no full on/off), continuous while the element is the eligible target. Well below three flashes per second. The "Next:" text and heavy border remain as the non-motion cue, which is also the reduced-motion presentation. There is no motion setting in the game today (no such preference in `Assets/ForestPrototype`); a "Reduce motion" switch should arrive with this change. Implementation note, unverified: UI Toolkit style sheets have transitions but no keyframe animation, so this is a class toggled by a scheduled callback with a USS transition. Whether that reads well needs a Unity look.

**Suspended** (as now, plus additions): Help open or just closed, session menu, Reference Preview, any modal confirmation (then the safe decline is the target, as now), an open rack question, a tree inspection with unanswered review prompts, and whenever the candidate is disabled or hidden.

**Mapping to add** (lesson numbers as presented; "→" is the single target):

| State | Objectives screen | Stand Map | Work Plan | Forest HUD |
|---|---|---|---|---|
| Any lesson, Annual Review unread | → Work Plan / review (as now) | same | → read results | "Open the Annual Review" |
| 2, area not yet visited | → Stand map | → Set waypoint, then Back to forest | – | waypoint + checklist |
| 2, choosing | → Back to forest | → Back to forest | – | `[E] inspect · [C] mark Crop Tree` |
| 3, marking competitors | → Back to forest | → Back to forest | – | `[E] on a Crop Tree · [1–5] · [X]` with per-tree share |
| 3, all three at target | → Work Plan | → Work Plan | reason line only | "Open the Work Plan [Tab]" |
| 4, outcomes not mixed | → Work Plan | → Work Plan | reason line names what is missing | – |
| 4, ready | → Work Plan | → Work Plan | Approve: tier 2, then Advance: tier 2 | – |
| 5, review unread | → review | – | → read results | – |
| 5, revisiting | → Back to forest | → Back to forest | – | `Revisit n/3 · [J] retain · [K] release further` |
| 6, extension | → Stand map | → Treatment review tab; inside it the next unfinished step; Approve prescription tier 2 | Approve tier 2 | candidates: "Inspect [E], confirm or reject" |
| 7–9 | to be listed in the spec from `ScenarioOneTeachingSequence` facts | | | |

Negative gates for a spec: never more than one pulsing element in the whole UI; nothing pulses when the target is tier 2; no target when cash or validation blocks approval (reason shown instead, as now); guidance never changes which button Enter activates on a confirmation; Legacy40 and Reference Preview unchanged; the Objective 1 behaviour and its existing checkers (#297/#303/#309 family) still pass.

### 3.4 Legend, minimap and Objective 1 figure

Legend, grouped, with shape differences that survive grayscale:

```
ALREADY ON THE GROUND
  ▬▬▬ (tan, dark edge)            Forest track, in from the south gate
  ▬▬▬ (tan, edge ticks ┴┴┴)       Headland and link track: rack entrances start here
  ▒▒  (patch)                     Clearing
YOUR PLAN
  ┄┄┄ (thin dashed, no fill)      Suggested rack position
  ╌╌╌ (white dashed)              Rack you are drawing
  ═══ (solid outline, hatched)    Planned rack: confirmed, work pending, trees still standing
  ■   (square on the headland)    Rack entrance;  ■✓ once marked in the forest
DONE
  ░░░ (pale open strip, stump dots)  Harvested rack, locked
```

The exact glyphs are for whoever implements it; the requirement is that the three "already on the ground / planned / done" groups cannot be confused, and that no row uses an internal version name.

Objective 1 figure: keep it on Objective 1 (it is the lesson about this layout). Proposed changes: map at full frame height; "Headland", "Link track", "Forest track", "Clearing" written on the map instead of A/B/C; all five racks drawn as *suggested* (dashed), because the card beside it says 0 / 5 planned; the four states shown once each in a strip under the map (Suggested → Planned → Entrance marked → Harvested). Keep the five real, uneven positions (z −35.27, −16.42, 0.24, 15.79, 35.23), five entrances, six retained bands, the existing title disclaimer and the old-world guard (`ShowsFiveRackFigure`). No northern connector. A copy on the scenario introduction is optional and would reopen the three-page onboarding spec; I would not do it now.

## 4. Access topology

### 4.1 What is on the ground today (source at `fb599eec`)

Metres, x east, z north, stand −40…+40 on both axes.

```
 z
+40 ┌──────────────────────────────────────────────────────────────┐
    │  H                                                           │
+35 │  H■━━━━━━━━━━━━━━━━━━ rack 5 (z 35.23) ━━━━━━━━━━━━━━━━━━━━━━│
    │  H                                                           │
+16 │  H■━━━━━━━━━━━━━━━━━━ rack 4 (z 15.79) ━━━━━━━━━━━━━━━━━━━━━━│
+11 │  H                      ◯ clearing r 4.5 at (−2.77, 11)       │
    │  H                       ╲                                   │
  0 │  H■━━━━━━━━━━━━━━━━━━━━━━━╪━ rack 3 (z 0.24) ━━━━━━━━━━━━━━━━│  teaching area x −12…+12
    │  H                        │  old trail, winding, x −2.7…+3.0 │
−16 │  H■━━━━━━━━━━━━━━━━━━━━━╪━━ rack 2 (z −16.42) ━━━━━━━━━━━━━━━│
−27 │  H●━━━━ feeder (z −27.4, 33 m) ━━●                           │
−35 │  H■━━━━━━━━━━━━━━━━━━━━━╪━━ rack 1 (z −35.27) ━━━━━━━━━━━━━━━│
−38 │  H                      ┃ approach, x −2.44                  │
−40 └─────────────────────────┃────────────────────────────────────┘
   −40  −35.4               gate (opening in the south ridge)     +40
```

Rack positions are the suggested layout in the fixture world; the player's confirmed racks may differ.

**What is the Dirt Path today?** Three things sharing one name prefix:
- 52 planks saved in the scene (`Dirt Path 00`–`51`, each 2.6 m wide), winding from (−0.93, −14.75) north to a 9 m "Forest Clearing" at (−2.77, 11). This is the original 40 m prototype's path to its work area; the hidden Forestry Workbench of D-070 sits at the clearing.
- For the 80 m stand only, runtime planks `Dirt Path X…` that continue it south, bend to due south and run straight at x = −2.436 to the property edge, plus a scenic cutting beyond the edge (`StandApproachPath.cs`, `StandApproachRoute.cs`). Added because the path "seemed to begin out of nothing".
- A tree-free corridor: no starting tree within 1.5 m of any plank (`ForestStartingStand`, `pathCorridorRadiusMeters`).

**Is the headland pre-existing or player-cut?** Pre-existing and authored. `ScenarioOneHeadlandGeometry` holds constants: headland at x = −35.4 from z = −37.7 to +37.7, feeder at z = −27.4 from the headland to x = −2.436, width 2.6 m, junction pads radius 2.2 m. Its own comment: "Fixed starting-world infrastructure… authored geometry, not save state." Three consequences:
- Trees: in a Rack V2 world the starting stand skips any slot whose stem centre falls on it (`ContainsStemCentre`). Because the stand is then topped up to a fixed target (1,344 trees in the fixture), the total is unchanged; different slots are left empty instead. Nothing is felled. There are no stumps.
- Surface: `ScenarioOneHeadlandPresentation` builds one flat mesh at runtime, 1 mm above the path, using the material of `Dirt Path 00`. No collider, no construction state.
- Game state: no work order, no cost, no time, no history record, no objective text. The only player-facing words are the map legend row and the rack editor hints.

So the user's reading is right: the game gives no account of how it got there. That is a gap in the fiction, not a simulation bug.

**What makes the feeder connect, and why?** The constant `PathX = −2.436166` is exactly the x of the straight southern approach, so the feeder ends on the track with a pad. Its purpose, as far as the source shows, is to join the headland to the only entrance. Why z = −27.4 was chosen is not stated in the files I read (the header cites RACK-V2-02B-R); **open question**.

**Three things the sketch makes plain** (S for geometry, O for the crossings):
- The layout is already a perpendicular T: the feeder meets the headland at a right angle.
- Racks 1, 2 and 3 cross the old trail, and rack 1 runs parallel to the feeder 7.9 m south of it for 33 m.
- About 121 m of existing track (12.6 m approach + 33 m feeder + 75.4 m headland) serves five racks, in a stand where the incoming track already touches three of them.

**What would be lost by removing or re-routing the original trail?**
- The scene planks are shared by Legacy40 and the frozen Reference Future v1 (same `ForestTest.unity`; `MixedSpeciesTest.unity` also has 52). They cannot be deleted; a change would have to be a runtime rule for a new world type.
- `ScenarioOneHeadlandPresentation.CreateSurface` finds `Dirt Path 00` by name to copy its height and material. If that plank is inactive, the headland surface is not created.
- The starting stand's clearance list changes, so the set of omitted slots changes, so tree identities change. `ScenarioOneSuggestedRackLayout` needs the five entrance trees `PO0203, PO1204, PO2003, PO2803, PO3803` to exist; if one is omitted it returns no layout. The diagram fixture and every hash-based check tied to the current world would also move.
- MFM coverage treats every active plank as a Track exclusion (`ScenarioOneCoverageWorld`); D-068 measured that changing planks changes treatable stems and area.
- The clearing, the buildables placed there, the player start and the south gate opening all hang off the trail.
- The Stand Map, minimap and Objective 1 figure all draw it.

### 4.2 Evidence classification

| Statement | Class |
|---|---|
| Headlands connect rack exit points to forest roads; they and linking tracks take the most machine traffic of the unpaved network, belong on the driest ground and should be kept as short as possible. | Evidence-backed (CCF book p. 42, via `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md`) |
| Forest roads, permanent headlands, tracks and a permanent rack system are standing infrastructure that divides a forest. | Evidence-backed (p. 39) |
| Purpose-cut inspection paths, distinct from machine racks, are recommended in closed conifer stands. | Evidence-backed (p. 39). Relevant to how the old trail could be named. |
| Retained trees with two white or blue horizontal lines can demarcate a permanent rack or headland route. | Evidence-backed (p. 87). A possible honest visual cue for the headland. |
| 16–20 m rack spacing in early transformation. | Evidence-backed, conditional (p. 40); used by D-064 as guidance |
| 2.6 m track and headland width, 2.4 m rack width, 2.2 m pads. | Gameplay abstraction. The book gives no rack width in metres (p. 41). Whether a real forwarder fits is **not established** here. |
| Headland position, feeder position, tree-free strips without stumps. | Model derivation / authored scenery |
| Who built the headland, when, and at what cost. | **Open.** I found no source for Irish construction cost or legal requirements and propose none. |
| How wide a turning area a machine needs at a rack entrance or T-junction. | **Open question**; not in the book reference. |

### 4.3 Options

| | A — Teach current | B — Presentation cleanup | C1 — West gateway onto the headland | C2 — South headland, racks turned 90° | D — Playable access creation |
|---|---|---|---|---|---|
| What changes | Words, legend, minimap, figure | A, plus the old trail north of the feeder junction restyled as a footpath in 3D and on maps | New opening in the west boundary at the existing junction pad (−35.4, −27.4) or at mid-headland; track is the 4.6 m from boundary to headland. Feeder no longer needed. | East–west headland along the south edge met by the south approach; five racks run north | Player plans, pays for and waits for the headland |
| Player perceives | Same ground, now explained | "Machine route" and "old footpath" look different | Road outside, gate, headland, five racks. One T. | One T at the south; old trail lies along the middle rack | Access as a first decision with a cost |
| Five existing racks | Unchanged | Unchanged | **Unchanged** | All redrawn; the middle rack coincides with the old trail (x −2.7…+3), so the trail must be replaced or the rack moved | Unchanged or redrawn |
| Rack records, entrances, MFM derivation, teaching area | Unchanged | Unchanged | Unchanged in form. Tree set changes if the feeder or trail masks change. | `ScenarioOneHeadlandGeometry`, `RackEditing` bounds and slide axis, suggested layout (uses entrance tree Z), sample geometry axis, coverage: all affected | New order type, state and history |
| Legacy40 / frozen Reference v1 | Untouched | Untouched if gated like the headland (Rack V2 only); must be proven | Untouched only if gated to a **new** world type | Same, larger surface | Same |
| Existing Rack V2 saves, including the user's #361 save | Unchanged | Unchanged (presentation only) | Must keep their current world. Needs a discriminator for "which access layout was this world made with" (a new saved field, or a derived test in the style of `SaveHasApproach`). | Same | Same, plus migration of scenario state |
| Map / 3D / minimap equality | Improves (minimap gains headland) | Must hold for the restyled trail | New geometry in all three plus the boundary ridge opening | Everything redrawn | Adds "planned but not built" states |
| Work Plan | None | None | None | None | New cost and timing; needs figures that have no source yet |
| Spec | Small text/presentation change; D2 minimap needs explicit scope | Short spec: it touches 3D presentation and must prove masks are unchanged | **New four-file spec; supersedes parts of D-071/D-072** | New spec; effectively a new Rack V2 revision | New spec; D-071 already deferred this to future scenarios |
| Unity qualification | Light | Visual, both resolutions | Full: new world generation, map parity, MFM coverage, old-save load | Full plus rack editing | Full |

**Retain or replace the old trail** (applies to B, C1, C2):

| | Retain | Replace / remove |
|---|---|---|
| Ground | Clearing, start position, south gate stay. Three racks still cross it. The teaching area's middle third stays thinner. | Cleaner network. More planted ground. Teaching thirds become even. |
| Meaning | Can be named honestly as an old path or inspection path to the clearing, visually distinct from machine access. | The clearing, buildables, player start and gate need a new home or retirement (D-070 already wants the workbench footprint gone). |
| Cost and risk | Low. Presentation only if masks are untouched. | New world identity: different trees, possible loss of an entrance tree ID, fixture and hash checks regenerate, `Dirt Path 00` template dependency, MFM Track exclusions change. Old worlds must keep the trail. |

The user's words leave open whether the old trail stays. Nothing above assumes deletion is authorised.

**Recommendation.**
1. Do **A now** with the P1 items: one or two sentences on Objective 1 and in the map legend, of the form "The headland and its link track were already here when you took over this stand. They are existing access, not part of your plan and not a cost." Who built them is a narrative choice for the user (question 5 below).
2. Do **B** if the visual check supports it: the trail north of the feeder junction becomes a narrower, differently textured footpath with its own dotted map symbol. The masks must stay byte-identical so no world changes.
3. Put **C1 with the trail retained as a footpath** to the user as the candidate for "a simpler incoming track meeting a perpendicular headland". It delivers that picture, shortens connecting track from about 121 m to about 80 m (headland plus gateway), matches "keep connecting routes short", and leaves racks, records, MFM and the teaching area alone. Removing the trail can follow later as its own world-generation change.
4. I do not recommend **C2** for Scenario One: it reopens Rack V2 for no teaching gain. I do not recommend **D** for Scenario One: D-071 placed it in future scenarios and its costs have no evidence base yet.

**Conflict to record.** D-071 (5 November) confirmed "the present access path and southern headland" and D-072 aligned the map to it. C1, C2, D and trail removal are candidate supersessions. D-064 (rack geometry authority) and D-065 (MFM) are unaffected by A, B and C1 in form. The 3 November preference for a central spine (D-067) was itself withdrawn by D-071; the present suggestion is a third direction in a week, which is a reason to settle it in a spec before any code.

## 5. Prospective tasks: touch sets, dependencies, tests

Paths are under `Assets/ForestPrototype/` unless stated. None of these is created as a task by this report.

| Task | Files likely touched | Depends on | Save impact | Tests and negative gates |
|---|---|---|---|---|
| A-min zone wayfinding | `UI/Map/StandMapModel.cs`, `StandMapSource.cs`, `StandMapDrawPlan.cs`, `StandMapSymbols.cs`, `StandMapLegend.cs`; `UI/StandMapView.cs`; `UI/ScenarioOneRackBayText.cs`; `UI/ActiveObjectiveHudWidget.cs` / `WalkingHudView.cs`; `UI/LearningObjectivesView.Sequence.cs`; `UI/ScenarioOneTeachingSequence.cs` (wording); `UI/Resources/ScenarioOneUi.uss`. Minimap: `UI/HudMinimapView.cs` if in scope. | None. Reuses `StandMapAreaFootprints.TeachingPolygon`. | None | Section 3.1 list. Offline: polygon equals sample frame for rotated plans; text checks. Legacy40 and Reference Preview draw nothing new. Existing rack-UX and map-parity verifiers still pass. |
| A-picture | `UI/TeachingDiagramPlan.cs` (stage 1 items, labels, legend) | A-min wording | None | Existing diagram plan checks; label size at 720p |
| C-min guidance | `UI/ScenarioOneUiRoot.cs` (mapping, pulse scheduler), `UI/LearningObjectivesView.cs`, `UI/StandMapView.cs`, `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs` (named buttons), HUD widget, USS; a small settings entry for reduced motion | Amendment of the approved Objective 1 next-action spec (#265 lineage); A-min for the lesson 2 targets | PlayerPrefs for the motion setting only | Section 3.3 negative gates; the CTA checker family (#297/#303/#309/#320) must be extended, not bypassed |
| B-min review labels | `UI/AnnualReviewView.cs`, `ScenarioOne/ManagementHistoryText.cs` (prefix wording), USS; verifiers under `Tools/Verification/S1CManagementHistory/` | Approved amendment to S1-C FR-4 | None (toggle may be session-only) | Section 3.2 contract; `S1CProvenanceOfflineCheck` unchanged |
| D1 legend / D2 minimap | `UI/Map/StandMapLegend.cs`, `StandMapSymbols.cs`, `StandMapDrawPlan.cs`, `StandMapLegendView.cs`; `UI/HudMinimapView.cs` | D2: explicit scope under or after D-072 | None | Grayscale distinguishability; the D-072 map-parity witness; minimap text overflow (known pre-existing HUD overflow failures on main must not be counted against this) |
| D3 figure | `UI/TeachingDiagramPlan.cs`, `UI/TeachingDiagram.cs`; `Tools/Verification/ScenarioOneFiveRackDiagram` | D1 vocabulary | None | Five uneven positions, six bands, old-world guard, no northern connector |
| E-A wording | `UI/ScenarioOneTeachingSequence.cs` or the V2 Objective 1 text source, `StandMapLegend.cs` | User wording decision | None | Text checks |
| E-B trail restyle | `StandApproachPath.cs` or a new small presentation component; map symbol files | Visual go/no-go in Unity | None, if masks are proven unchanged | Tree set and MFM coverage identical before/after on the fixture; Legacy40 and Reference untouched |
| E-C1 west gateway | `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneHeadlandPresentation.cs`, `StandApproachPath.cs` / `StandWorldBoundary`, `ForestStartingStand.cs`, `ScenarioOneCoverageWorld.cs`, `ScenarioOneManager.PrescriptionApproval.cs`, map source/draw/legend, `TeachingDiagramPlan.cs` fixture, save controller for the world discriminator | New approved spec; user decisions 6–8 | New field or derived rule; old worlds load unchanged | Old-save load matrix (pre-approach, current V2, new); entrance IDs present; MFM parity; map/3D equality; frozen Reference hash |

**Recommended spec boundaries.**
1. *Scenario One teaching-area wayfinding V1* (A-min, A-picture). New small spec; presentation only.
2. *Next-action guidance V2* (C-min). Amendment or successor to the Objective 1 guidance spec; must include the tier table and the motion setting.
3. *S1-C review presentation amendment* (B-min; B-full optional). Amends FR-4 wording.
4. *Stand Map legend and minimap access presentation* (D1, D2, E-A, optionally D3). Could be an amendment to `specs/scenario-one-stand-map-headland-teaching-alignment-v1`.
5. *Scenario One access layout V2* (E-B if it needs one; E-C1). Only after the user chooses. Supersedes parts of D-071/D-072.

**Decisions for the user.**
1. Keep "exactly three, one per third, both sides" as a gate (recommended for now), or move to a looser rule (A-full)?
2. Annual Review: chips hidden by default with a "Show data sources" switch? And was "Confirmed" the RECORDED chip, or seen on another screen (the Treatment review does have a "CONFIRMED CROP TREE" chip)?
3. Pulse style: continuous slow pulse (recommended), or a few cycles and then steady? Add a "Reduce motion" setting now?
4. Should commitment buttons (Approve Work Plan, Advance year) stay static, as recommended, even when they are the next step?
5. Headland story: is "already here when you took over" enough, or should the game say who made it?
6. Access: A + B only, or also commission a C1 spec?
7. If C1: gateway at the existing junction (z −27.4) or mid-headland (z ≈ 0)?
8. Old trail: keep as a footpath (recommended first), or plan its removal in a new world type?
9. Should the HUD minimap show the headland and the teaching area?

**The user's own save and playtest.** Every P0/P1 proposal is presentation or guidance and adds no save field, so the #361 save should load unchanged under them (to be proven when built). Any C-family change must leave existing worlds on their current layout. I read no file from the user's save directory and wrote none.

## 6. Evidence level, risks and what still needs Unity

**Unity graphical inspection: not performed.** The packet's preconditions were not met and I did not try to work around them:
- #361, the user's personal playtest, is still open and has precedence.
- No lease could be taken: the `unity-lock` tool is not on this session's path, so there was no real lock ID to hold or release.
- Files storage had 7.9 GiB free (99% used) and ZX20 36 GiB, one gibibyte above the 35 GiB floor.

No Editor process was running when I checked, but that alone does not admit a session. Nothing was launched, and CCF-U3 was only read.

**What rests on what.**
- Rules, text, guidance mapping, legend construction, headland generation: read directly from source at the tested commit. High confidence.
- How Objective 1, the Objective 2 card and the Stand Map look: earlier captures from `045966d2` (10-line onboarding-only difference). Two of the Stand Map captures I used are grayscale conversions. Good for layout, not for colour.
- Annual Review appearance: one capture from the older `a1e15b52`; the chip code is the same at `fb599eec` by source reading.
- Tree counts per third and the 50% figure: my arithmetic on one archived world, assuming the suggested layout. Indicative only.
- Everything about how the proposals would look or feel: unverified design.

**Risks.**
- The pulse depends on a UI Toolkit technique I have not run here.
- Hiding chips changes an approved MUST; doing it without the amendment would be a silent spec breach.
- The minimap already has pre-existing text overflow failures on main; adding to it needs care.
- Any change to path planks or headland masks moves tree identities and can silently empty the suggested rack layout.
- Compass side names are ambiguous for a rack plan rotated near 45°; the spec should define the rule.

**When another Unity look is needed.** Before approving A-min visuals (map overlay legibility at 1280×720 and the HUD checklist), before accepting any pulse, and before any decision on E-B. One bounded read-only session at both resolutions would do, after #361 is released and a real lease is available. A photo or short video from the user of the Objective 2 forest HUD and the current Annual Review would also close the two largest gaps, but is not required.

## 7. Return

- Status requested for #362: **HANDED-OFF**, returned to Manager #200 for APPROVED / NEEDS CHANGES / REJECTED on the design recommendations and for the user questions in section 5.
- No implementation task, spec or decision-log entry was created.
- #361 remains open and untouched.
