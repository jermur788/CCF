# Scenario One — Objective 1 Rack Planning & Field Marking V2 Specification

- **Spec ID:** SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1
- **Feature ID:** `SCENARIO-ONE-OBJECTIVE-1-RACK-PLANNING-FIELD-MARKING-V2`
- **Status:** APPROVED — the user's own label is `APPROVED FOR IMPLEMENTATION` (user instruction, 2026-10-26)
- **Owner:** Overall Manager (specification supplied by the user)
- **Created:** 2026-10-26
- **Last updated:** 2026-10-26
- **Primary routing:** `SOL / CODEX`. Suitable bounded sub-work: `GPT-6.1 TERRA` — visual/UI implementation and rendered review. Routing is a recommendation; the Overall Manager assigns work through task packets.
- **Decision log references:** D-063 (spec-driven development); D-056 / D-057 (Enlarged80 stand geometry); D-042 (Reference Future v1 frozen); D-064 (this feature's accepted game design, 2026-10-26).
- **Milestone:** Scenario One — see `Docs/Project/current-milestone.md`
- **Related evidence:** `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md` (pp. 39–42, 58, 84–87); `Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md`; `Docs/Project/research-index.md` (rack and tree-marking extraction); `specs/scenario-one-ui-visual-system-v2/` (on `feature/scenario-one-ui-visual-system-v2`, not on every checkout)
- **Supersedes / superseded by:** Supersedes *behaviour*, not a prior spec ID: Scenario One's first-person freehand rack creation and its separate counters (see §4). Superseded by: none.

> **Provenance of this file.** Sections 1–38 are the specification as supplied by the user on 2026-10-26, with only the headings re-levelled to Markdown conventions. Section 39 is added at authoring time to satisfy `specs/README.md` (non-goals, stop conditions, save/security statements, open decisions). Section 39 restates or indexes the spec above. The only genuinely new content was three decisions raised in §39.7; the Manager resolved OPEN-1 and OPEN-2 on 2026-10-26 (recorded as D-064) and OPEN-3 remains open.

---

## 1. Purpose

Redesign Scenario One Objective 1 so that the player learns how extraction racks are:

1. planned accurately at stand scale;
2. evaluated for spacing and alignment;
3. communicated physically in the forest using forestry paint markings.

The current duplicated rack-creation workflow is superseded.

The revised architecture is:

`MAP = RACK GEOMETRY AND PLAN`

`3D FOREST = FIELD MARKING AND PHYSICAL UNDERSTANDING`

There must be one authoritative rack plan.

---

## 2. Product problem

Current Objective 1 has several connected problems:

- racks can apparently be created through more than one interaction route;
- map-created racks and manually created first-person racks do not behave consistently;
- rack counters can disagree;
- racks cannot be removed after placement;
- racks cannot be moved;
- racks cannot be rotated;
- accurate parallel placement is difficult in first-person view;
- suggested rack positions are understandable on the large map but unclear on the minimap;
- the player is not properly taught rack spacing;
- `Plan Rack` visually behaves like another map tab even though it is an interaction mode;
- real forestry marking practice is not taught;
- the player does not clearly experience the transition from a map plan to physical marks in the stand.

These are workflow problems, not isolated bugs.

---

## 3. Core player-learning goal

Objective 1 should teach:

> Extraction racks are a planned access network. Their spacing and direction affect how the stand can be worked, and the planned routes must be communicated clearly on the ground.

The player should understand:

- why racks exist;
- why spacing matters;
- why unnecessary racks remove productive growing space;
- why rack alignment matters operationally;
- that spacing depends on context rather than one universal CCF rule;
- how rack routes are marked physically in forestry practice.

---

## 4. Authority / supersession

This spec supersedes earlier Scenario One behaviour where rack geometry could be created independently in first-person view.

For **rack geometry only**, spatial design now occurs on the map.

This does not turn the Work Plan into a spatial-design tool.

The 3D forest remains important because the player must:

- locate rack entrances;
- understand the plan physically;
- apply forestry paint marks.

The existing broader physical-presence principle remains intact.

---

## 5. Evidence basis

### 5.1 Rack spacing

The CCF book material gives conditional examples:

- approximately `16–20 m` between racks during early transformation;
- approximately `30 m or greater` may be viable in developed CCF;
- explicitly no hard-and-fast universal rule.

The book also discusses conditional considerations involving:

- rack width;
- row removal;
- slope;
- stand development.

Classification:

`DIRECTLY EVIDENCE-BACKED — SOURCE EXAMPLES / CONDITIONAL GUIDANCE`

Do not describe 16–20 m as universally optimal CCF rack spacing.

---

## 6. Scenario One spacing calibration

Scenario One is an early transformation of an even-aged Sitka plantation.

For this specific teaching stand:

`16–20 m`

is the **Scenario One recommended rack-spacing band**.

Classification:

`GAMEPLAY CALIBRATION INFORMED BY DIRECT SOURCE GUIDANCE`

The UI must explicitly communicate that this is:

`recommended for this stand`

not:

`the universal CCF rule`

Default suggested layout target:

`~18 m centre-to-centre`

Before implementation, inspect the actual current stand geometry.

If the existing stand dimensions/path geometry cannot support five sensible racks while remaining broadly consistent with the 16–20 m teaching band:

`STOP — PRODUCT/GEOMETRY CONFLICT`

Do not silently distort distances merely to retain five.

---

## 7. Number of racks

Scenario One retains:

`5 planned racks`

provided preflight geometry confirms that five is coherent with the current stand dimensions and recommended spacing.

The five-rack target is classified:

`SCENARIO ONE GAMEPLAY CALIBRATION`

It must not be taught as a general forestry quota.

Objective copy should communicate:

> This stand needs five extraction racks to create a workable first access layout.

Not:

> CCF stands require five racks.

---

## 8. Map becomes authoritative rack-planning surface

Rack geometry may only be created or edited through the stand map.

Remove/disable the competing first-person freehand rack-creation pathway for Objective 1.

The map owns:

- rack creation;
- rack positioning;
- rack rotation;
- rack deletion;
- rack spacing feedback;
- suggested layout;
- rack confirmation.

The 3D world consumes the confirmed plan.

---

## 9. Map navigation versus planning mode

`Plan Racks`

must NOT appear as though it is another navigation tab.

Existing map tabs remain navigation.

`Plan Racks`

becomes a visually separate:

`TOOL / MODE CONTROL`

Preferred placement:

- map toolbar;
- below or beside navigation tabs;
- visually distinct from tabs.

States:

`Plan Racks`

`Planning Racks — Active`

The active state must be unmistakable.

---

## 10. Suggested rack layout

When the player enters rack-planning mode for the first time, show a suggested layout.

Suggested racks:

- are guidance, not completed racks;
- use a faint/dashed visual treatment;
- should broadly reflect the ~18 m Scenario One target;
- connect logically with the existing path/headland;
- remain distinguishable from player-created racks.

Suggested racks do not count toward Objective completion.

Legend:

`Dashed = suggested`

`Solid = your planned rack`

---

## 11. Rack creation

A player-created rack is an editable map object.

Each rack must have:

- stable identity;
- entrance/path anchor;
- direction;
- length/extent;
- spacing relationship to adjacent racks;
- planning status;
- field-marking status.

Rack counters must derive from these authoritative objects.

Do not maintain a separate independently incremented counter.

---

## 12. Mouse interaction

The player must be able to manipulate racks directly with the mouse.

Required interactions:

### Create

Create a rack from or toward the path/headland.

The final geometry must establish a valid rack entrance.

### Select

Click an existing rack to select it.

### Move

Drag the rack/body to reposition it.

The entrance should re-snap appropriately to the path/headland.

### Rotate

Provide an obvious mouse-driven rotation interaction.

Preferred:

- drag the interior/far endpoint or rotation handle;
- rotate around the rack entrance.

### Delete

Selected rack can be removed.

Support:

- visible Delete/Remove button;
- Delete/Backspace keyboard action where appropriate.

### Undo

Provide at least:

`Undo last rack edit`

A full general-purpose editor undo system is not required.

### Reset

Provide:

`Reset to suggested layout`

with appropriate confirmation if the player has made edits.

---

## 13. Snapping and parallelism

The system should help the player create practical, parallel rack layouts without placing them automatically.

Required:

- soft snapping toward the suggested/general rack orientation;
- soft snapping parallel to nearby player racks;
- visible spacing measurement while moving/rotating;
- enough freedom to deliberately deviate.

Do not force every rack onto a fixed invisible rail.

A rack should feel editable, not pre-scripted.

---

## 14. Spacing visualization

When a rack is selected or dragged, show:

- distance to nearest neighbouring rack;
- Scenario One recommended band: `16–20 m`;
- approximate status.

Example states:

`18.1 m — within recommended range`

`13.7 m — closer than recommended for this stand`

`22.5 m — wider than recommended for this stand`

The wording must make clear that the recommendation is Scenario One/site-specific.

---

## 15. Tree-row teaching

Because the starting Sitka plantation has regular plantation structure, Objective 1 may also show an approximate relationship between:

- metres;
- planting/tree rows.

However:

- the actual current planting geometry must be measured from the verified scene;
- no arbitrary "X rows" value may be invented.

If a robust row equivalent can be derived from the scene:

display it as teaching support.

Example form:

`≈18 m — approximately N planting rows in this stand`

Classification:

`MODEL/SCENE DERIVATION`

If the tree rows are not sufficiently regular to support a defensible value, omit the row count.

---

## 16. Plan validity

A rack plan is ready for confirmation when:

- five rack objects exist;
- all connect validly with the path/headland;
- racks have non-degenerate lengths;
- no duplicate/overlapping rack geometry exists;
- no rack is obviously outside the property/stand;
- the system can calculate their spacing.

For the tutorial objective, the player should be strongly guided toward the Scenario One 16–20 m recommendation.

Do not silently snap an invalid plan into validity.

Show why a plan needs adjustment.

---

## 17. Confirm Plan

Provide a clear:

`Confirm Rack Plan`

action.

Before confirmation the player may freely:

- move;
- rotate;
- delete;
- recreate racks.

After confirmation:

- the plan becomes authoritative in the forest/minimap;
- Objective 1 transitions to field marking;
- planned rack count becomes fixed until the player explicitly reopens planning.

---

## 18. Editing after confirmation

The player may reopen the rack plan.

Behaviour:

### If no entrance marks have been applied

Reopen freely.

### If field marking has begun

Show a warning:

> Changing a confirmed rack may invalidate its field mark.

Only racks whose geometry changes lose their `entrance marked` state.

If an obsolete painted mark is a game-managed visual, remove/reset that obsolete mark when the rack geometry changes.

This is:

`GAMEPLAY STATE MANAGEMENT`

not a claim about real paint physically disappearing.

---

## 19. Single authoritative counters

During planning:

`Racks planned: X / 5`

After confirmation:

`Rack entrances marked: X / 5`

Do not use one overloaded counter for both.

Do not allow map and first-person systems to increment separate values.

The objective reads directly from authoritative rack objects.

---

## 20. Minimap representation

The minimap must use the same conceptual language as the main map.

Before confirmation:

- suggested racks may appear faint/dashed where helpful;
- do not make them look completed.

After confirmation:

- planned racks appear clearly as solid planned routes.

First-time teaching note:

> Dashed lines show suggested rack positions. Solid lines show your confirmed plan.

Do not require the player to infer what unexplained lines mean.

---

## 21. 3D forest representation

Do not render giant glowing rack lines through the forest.

The 3D world should remain credible.

After plan confirmation:

- each rack entrance receives a subtle field waypoint/context cue;
- the player can identify the correct first tree associated with that rack;
- the minimap continues to provide route context.

The cue disappears after that entrance has been properly marked.

---

## 22. Field-marking interaction

Once the plan is confirmed:

1. player leaves/close map;
2. walks to each planned rack entrance;
3. equips/uses forestry spray marker;
4. marks the first appropriate rack tree nearest the path;
5. mark persists;
6. rack entrance becomes `Marked`.

Objective counter:

`Rack entrances marked: X / 5`

This design intentionally teaches a real physical marking action without requiring the player to manually spray every tree along every rack.

Classification:

`GAME DESIGN DERIVATION INFORMED BY REAL MARKING PRACTICE`

The source does NOT establish that only the first tree at each rack entrance is marked in real operations.

The game must not claim that it does.

---

## 23. Which tree is marked

For each confirmed rack, determine:

`first rack-removal tree nearest the path/headland`

The target must:

- lie sufficiently close to the confirmed rack centreline;
- be part of the rack-removal set;
- be the nearest appropriate tree to the entrance.

Do not allow arbitrary unrelated trees to satisfy the rack mark.

If no suitable tree can be found for a confirmed rack:

`RACK INVALID — ADJUST PLAN`

rather than silently selecting an unrelated tree.

---

## 24. Paint convention for Objective 1

The CCF book supports:

- coloured vertical removal/machine-path stripes;
- on four sides of the tree;
- commonly orange or pink for removal/treatment marking.

For Scenario One Objective 1 use:

`ORANGE — RACK / MACHINE-PATH REMOVAL MARK`

Visual form:

`vertical stripe treatment represented on four sides`

Classification:

- orange/pink removal family: `DIRECT SOURCE GUIDANCE`
- choosing orange specifically: `GAMEPLAY CALIBRATION`
- using it for the first rack-entrance tree only: `GAME DESIGN DERIVATION`

One player spray interaction may apply the full four-sided mark representation.

Do not require four separate clicks around the trunk.

---

## 25. Existing marks

Preserve existing meanings:

`RED — Fell`

`BLUE — Crop Tree / future timber`

Objective 1 adds:

`ORANGE — Rack / machine-path removal`

This does not yet introduce every marking category in the CCF book.

Future marking types may unlock later.

---

## 26. Mark conflict rules

A tree cannot simultaneously display incompatible active management instructions.

At minimum:

- Crop Tree / Blue cannot also be Rack Removal / Orange;
- Crop Tree / Blue cannot also be Fell / Red;
- rack-removal designation must resolve consistently with later felling/execution state.

If a planned rack intersects an already protected/non-removable tree state:

flag the conflict in map planning.

Do not silently overwrite a protected designation.

---

## 27. Spray interaction quality

The spray action should be simple, readable and physical.

Required:

- player within sensible interaction range;
- spray-marker/tool state;
- aim at valid tree;
- subtle valid-target feedback;
- click/use;
- short spray sound/visual action;
- persistent orange mark.

Do not build a freehand paint simulator.

The educational point is:

`identify correct tree → apply correct forestry mark`

---

## 28. Help / learning page

Introduce:

`Tree Marks`

help content when field marking first becomes relevant.

Do not overwhelm the player with all future marks immediately.

Objective 1 help should teach only currently relevant marks plus existing marks already encountered.

Initial table:

`Orange vertical stripes — Rack / machine-path removal`

`Red — Fell`

`Blue — Crop Tree / future timber`

For orange:

> Removal trees associated with a machine route can be marked with vertical stripes. The CCF source material describes removal/treatment colours such as orange or pink; Scenario One uses orange.

Also explain:

> The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.

This prevents a gameplay simplification from being taught as universal practice.

---

## 29. Progressive marking education

Additional paint conventions may be introduced later as Scenario One requires them.

Potential future categories include:

- retained route-side trees;
- deadwood/habitat;
- other management designations.

Do not implement unused marks merely because the source mentions them.

Teach them when they become meaningful.

---

## 30. Objective 1 teaching flow

Required sequence:

### Step 1 — Understand racks

Objective detail explains:

- purpose;
- spacing;
- conditional nature of recommendations.

### Step 2 — Open map

Explicit instruction to use the stand map.

### Step 3 — Plan

Enter `Plan Racks` mode.

Suggested layout appears.

### Step 4 — Edit

Player creates/moves/rotates/deletes racks.

UI shows spacing.

### Step 5 — Confirm

Five valid racks.

`Confirm Rack Plan`

### Step 6 — Field marking

Return to forest.

Mark first rack-removal tree nearest the path for each planned rack.

### Step 7 — Completion

Five entrances marked.

Objective 1 completes / transitions to the next accepted Scenario One stage.

> **Manager decision, 2026-10-26 (D-064, see §39.7 OPEN-2):** field marking happens before work approval. The existing `rack.approve` and `rack.result` steps are kept, in that order, as the stage after marking.

---

## 31. Objective 1 copy — semantic requirements

Objective details must explain:

> Extraction racks allow machinery to reach trees without creating access everywhere across the stand.

> In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. Scenario One uses that range as a guide for this plantation.

> Plan the racks on the map, then mark each rack entrance in the forest.

Do not claim:

`18 m is the ideal spacing for all CCF forests.`

Do not claim:

`five racks are always required.`

---

## 32. Existing map visual system

Use qualified UI Visual System V2.

Do not create a separate map-editor theme.

Map planning should feel like a professional forestry/cartographic tool.

Use:

- existing charcoal/slate surfaces;
- neutral text;
- teal/appropriate active states;
- restrained line weights;
- semantic colour.

---

## 33. Suggested visual language

Suggested rack:

`faint dashed line`

Player rack:

`solid line`

Selected rack:

`solid + restrained highlight/handles`

Invalid/conflicting rack:

`clear warning state`

Confirmed rack:

`solid authoritative route`

Do not rely solely on colour where line pattern can communicate state.

---

## 34. Persistence

Rack planning is persistent gameplay state.

The authoritative save state must support, directly or through the existing architecture:

- rack stable ID;
- geometry;
- confirmed/unconfirmed state as needed;
- entrance-mark state;
- marked tree identity if applicable.

Before changing save/schema:

inspect existing rack persistence.

Reuse existing fields where suitable.

Extend minimally.

Backward compatibility is mandatory.

Old saves must:

- continue loading;
- not crash;
- preserve existing rack state where safely interpretable;
- receive defensible defaults where V2 data is absent.

Do not silently invalidate existing Scenario One saves.

---

## 35. Counter derivation

Never persist an independent `5`-rack objective counter if it can be derived from authoritative rack objects.

Preferred:

`planned count = valid rack objects`

`marked count = valid confirmed racks with current entrance mark`

This avoids the current disagreement between interaction pathways.

---

## 36. Execution versus planning

A painted rack entrance does not itself mean all trees in the rack have been physically felled.

Maintain architectural distinction:

`plan`

`mark`

`execute work`

If the current Scenario One implementation immediately creates/opens a rack when the rack is planned, inspect that behaviour before changing it.

The V2 implementation must provide one coherent transition from:

`confirmed plan`

to:

`authoritative rack work/result`

without duplicating forestry outcomes.

Do not invent a second rack-removal system.

---

## 37. Protected existing behaviour

Unless explicitly superseded above, preserve:

- Scenario One stand geometry;
- path entering from outside the property;
- forestry growth/simulation;
- Crop Tree mechanics;
- competition mechanics;
- saves;
- economy;
- Work Plan architecture;
- subsequent objectives;
- old-save compatibility.

No Windows build.

---

## 38. Acceptance criteria

The feature passes only if:

1. **AC-1** — map is the sole rack-geometry authoring surface;
2. **AC-2** — first-person freehand rack creation no longer creates a competing authoritative rack;
3. **AC-3** — player can create/select/move/rotate/delete racks;
4. **AC-4** — suggested racks are distinct from player racks;
5. **AC-5** — spacing is visible;
6. **AC-6** — 16–20 m is presented as Scenario One guidance, not universal law;
7. **AC-7** — five-rack calibration fits verified stand geometry;
8. **AC-8** — `Plan Racks` is visually a tool, not a tab;
9. **AC-9** — plan can be confirmed;
10. **AC-10** — minimap clearly represents suggested/planned rack states;
11. **AC-11** — rack counter has one authoritative source;
12. **AC-12** — player physically sprays the first appropriate entrance tree for each rack;
13. **AC-13** — orange four-sided vertical rack/removal mark is visible and persistent;
14. **AC-14** — marked-entry counter is separate from planned-rack counter;
15. **AC-15** — incompatible mark conflicts are prevented;
16. **AC-16** — Help explains the marking convention and its gameplay simplification;
17. **AC-17** — old saves load;
18. **AC-18** — no simulation/growth regression occurs;
19. **AC-19** — rendered map and 3D review passes;
20. **AC-20** — user can understand the workflow without prior knowledge of the old implementation.

> **Applicability, Manager decision 2026-10-26 (D-064, see §39.7 OPEN-1):** AC-1 to AC-16 and AC-19 to AC-20 apply to the Enlarged80/V2 path. AC-17 (old saves load) applies to every save, with Legacy40 saves keeping their existing behaviour.

Final implementation verdict:

`OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`

---

## 39. Sections required by `specs/README.md` (added at authoring)

These sections index and restate the spec above. They add no product requirement of their own. The decisions in §39.7 are Manager decisions (OPEN-1 and OPEN-2 accepted 2026-10-26, D-064); OPEN-3 is raised for a decision and is not assumed.

### 39.1 Non-goals

Each is already stated or implied above; the section cited is the source.

- No Windows build (§37; checklist *User-test gate*).
- No change to forestry growth, competition, species models or economy (§37; checklist *Behaviour*).
- No change to Scenario One stand geometry, the path entering from outside the property, Crop Tree mechanics, the Work Plan architecture or subsequent objectives (§37).
- No second rack-removal system (§36).
- No spatial-design role for the Work Plan (§4).
- No general-purpose editor undo (§12).
- No freehand paint simulation, and no requirement to spray every tree along a rack or four separate clicks around a trunk (§22, §24, §27).
- No marking categories beyond Fell (red), Crop Tree (blue) and the Objective 1 rack mark (orange); no pre-built unused marks (§25, §29).
- No unrelated UI redesign and no separate map-editor theme (§32; checklist *Behaviour*).
- No universal rack-spacing rule and no general five-rack quota presented to the player (§5.1, §7, §31).

### 39.2 Requirements and acceptance numbering

The normative requirements are the statements in §§6–36, cited by section number. §38 carries the acceptance criteria as AC-1 to AC-20, and `checklists/requirements.md` restates every one with an evidence slot. This spec deliberately does not duplicate the requirements as a second `FR-n` list, so the two cannot drift apart.

### 39.3 Evidence classification index

| Statement | Classification | Section |
|---|---|---|
| 16–20 m early-transformation examples; 30 m+ in developed CCF; no hard-and-fast rule | `DIRECTLY EVIDENCE-BACKED — SOURCE EXAMPLES / CONDITIONAL GUIDANCE` (book p. 40) | §5.1 |
| 16–20 m as the Scenario One recommended band | `GAMEPLAY CALIBRATION INFORMED BY DIRECT SOURCE GUIDANCE` | §6 |
| ~18 m default layout target | `GAMEPLAY CALIBRATION` (within the band above) | §6 |
| Five racks | `SCENARIO ONE GAMEPLAY CALIBRATION` | §7 |
| Metres-to-planting-rows relationship | `MODEL/SCENE DERIVATION`, shown only if measured from the verified scene | §15 |
| Coloured vertical removal/machine-path stripes on four sides; orange or pink | `DIRECT SOURCE GUIDANCE` (book pp. 84–85) | §24 |
| Choosing orange specifically | `GAMEPLAY CALIBRATION` | §24 |
| Marking only the first entrance tree | `GAME DESIGN DERIVATION`. The source does not establish it and the game must not claim it does. | §22, §24, §28 |
| One spray action applying the four-sided mark | `GAME DESIGN DERIVATION INFORMED BY REAL MARKING PRACTICE` | §22 |
| Resetting an obsolete mark when geometry changes | `GAMEPLAY STATE MANAGEMENT` | §18 |

### 39.4 Save / backward compatibility

The governing statements are §34 and §35. Backward compatibility is mandatory and counters are derived, never persisted.

Authoring-time fact, verified read-only on candidate `13988874` and to be re-confirmed by RACK-V2-01: today a confirmed rack is a session-only grouping of ordinary pending felling orders, and the code states that nothing about the grouping is saved. There is therefore no existing persisted rack field to reuse, so the persistence required by §34 is new data. It must be additive and minimal, and a missing field must mean "no V2 rack data". See `plan.md` for the findings and the Reference Future v1 constraint (D-042).

### 39.5 Security / write-back

Not applicable as a trust-boundary matter: this is local single-player state with no network or external write-back. The standing requirements are:

- loading must not crash on missing or malformed V2 rack data (§34);
- destructive player actions (Delete, Reset to suggested layout, obsolete-mark reset) change rack-plan and mark state only. They never alter tree growth data or overwrite a protected Crop Tree designation (§12, §18, §26).

### 39.6 Stop conditions

Work returns to the Manager/spec; an implementation assumption does not resolve these.

1. The verified stand and path/headland geometry cannot support five sensible racks broadly consistent with the 16–20 m band: `STOP — PRODUCT/GEOMETRY CONFLICT`. Do not distort distances to keep five (§6; plan *Geometry qualification*).
2. Persistence would require changing Reference Future v1, silently invalidating an existing Scenario One save, or breaking old-save loading (§34, §37, D-042).
3. The change would require altering growth, competition, species, economy, Crop Tree mechanics, the Work Plan architecture or a subsequent objective (§37).
4. Satisfying §36 would require a second rack-removal system or duplicated forestry outcomes.
5. The orange mark cannot be made consistent with the existing exclusive Fell/Crop Tree marks without weakening either meaning (§25, §26).
6. A confirmed rack has no suitable entrance tree in the **suggested** layout on the verified scene. For a player-edited rack this is the in-game state `RACK INVALID — ADJUST PLAN` (§23), not a stop; for the suggested layout it is a defect in the calibration.
7. OPEN-3 (§39.7) is reached before it is resolved.

### 39.7 Product decisions

Raised at authoring from read-only inspection of candidate `integration/scenario-one-hud-colour-pass-v1` @ `13988874cc36413d911243f4d3a6524436237bab`. OPEN-1 and OPEN-2 were resolved by the Manager on 2026-10-26 and are recorded in the Decision Log as D-064. OPEN-3 remains open.

**OPEN-1 — Legacy40 saves: `ACCEPTED — V2 APPLIES TO ENLARGED80 ONLY`.**
- Fact (authoring time): D-056 defines Legacy40 as a 40 × 40 m stand, and saves at v19 or earlier, or without `standGeometryModel`, load as Legacy40. Five racks at ≥16 m spacing span ≥64 m, which cannot fit in 40 m. On the candidate, `ScenarioOneRackLayout.AppliesTo` is already Enlarged80-only.
- Decision: Rack Planning & Field Marking V2 applies only where `standGeometryModel == Enlarged80`. Legacy40 saves are not migrated to the V2 rack-layout curriculum. For Legacy40: preserve compatibility; do not require five racks at 16–20 m; do not apply the new spacing-validation gate; preserve the existing viable legacy progression path. Do not distort Legacy40 geometry to make V2 fit.
- Effect on this spec: the acceptance criteria that refer to the map as sole rack author and to the five-rack workflow apply to the Enlarged80/V2 path (see the note under §38). Reference Future v1 stays on its existing behaviour. The §6 geometry stop condition is evaluated for Enlarged80.
- Still to be verified by RACK-V2-01: that a viable Legacy40 progression path exists today, and the exact code boundary that keeps V2 off it.

**OPEN-2 — Work Plan boundary: `ACCEPTED — KEEP rack.approve AND rack.result`.**
- Fact (authoring time): on the candidate, confirming a rack adds one pending `FellTree` order per tree to the draft Work Plan, and the existing curriculum continues `rack.approve` and `rack.result`.
- Decision: the authoritative V2 sequence is `Plan Racks` → `Confirm Rack Plan` → pending rack-removal orders created or refreshed → mark rack entrances in 3D → `rack.approve` → execution → `rack.result`.
- Rules: Confirm creates pending work only; it does not approve work and does not execute felling. Field marking occurs before approval. `rack.approve` remains the explicit work-approval teaching step and `rack.result` remains the post-execution/result step. No parallel rack-execution mechanism is created. If the player edits a confirmed plan before approval, the pending rack-removal orders are reconciled with the revised geometry and not duplicated.
- Effect on this spec: the orange entrance mark is a field-communication state of the rack and is not a second removal system (§36); Objective 1 completes at five entrances marked and hands over to `rack.approve` (§30 step 7).

**OPEN-3 — Reopening a confirmed plan after its work has progressed: `OPEN`.**
- Fact: §18 specifies reopening only in terms of entrance-mark state. Today pending orders can be removed and approved orders cancelled, but executed removals cannot be undone. OPEN-2 settles the before-approval case (reconcile, do not duplicate). What remains open is what may be reopened after approval, after partial execution and after complete execution.
- Recommendation (unchanged, to be tested against the real state machine): allow reopening while a rack has no executed removals. Cancel and recreate its orders through the existing paths, with a warning. Lock a rack with executed removals and show it as harvested.
- Next step: RACK-V2-01 inspects the existing approval/execution state machine and reports the safest boundary. Do not implement OPEN-3 yet.
- Needed before: RACK-V2-05 begins.
