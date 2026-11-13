# #450 Objective 1 rack-marks progress amendment: independent delta review

**13 November 2026. Verdict for exact candidate `7633eaf1e720821af70d6b243b53fb9424cedd28`: APPROVED (bounded delta review).**

- **Production scope:** exactly the four authorised files, and only display strings.
- **Protected logic:** spray, aim, equip, visit order and target selection are byte-identical to the approved baseline `e115b540`.
- **Field wording:** no rack number or stored rack ID can reach the confirmed-plan walking HUD, minimap or spray prompt.
- **Counter:** "Rack marks: X of 5" comes only from the persisted per-rack `entranceMarked` flags. Any order counts, and a repeat spray cannot add progress.
- **Run:** the native run is a genuine PASS on this exact candidate.
- **Re-runs:** I re-ran the offline gate and the compile gate myself; both pass.
- **Findings:** none blocking. Two minor findings and some cosmetic points are listed below.
  - **R-1** needs a Manager scope ruling because its fix lies outside the four files.
  - **R-2** is a test-quality gap with no product effect.

This verdict is not integration approval and not user acceptance. Manager #424 owns the integration decision. The player's own re-test in their own world is still required: the original missing-arrow symptom has never been reproduced.

## Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), in T3 Code, as the review sub-agent for orchestration #447 on review claim #451. I started no subagents, posted nothing to GitHub issues and did not edit `active-tasks.md`.
- **Did not author the code under review.** The #450 production edit (`be5ff3d6`), the harness amendment (`7633eaf1`) and the native run are by OpenAI GPT-6.1 Sol.
- **Disclosure:** Claude Opus 5.5 sessions also wrote these. I have no memory of any of them; I read their reports.
  - The first #443 review (`d5024fc3`, NEEDS CHANGES).
  - The #449 delta review (`e9637bab`, APPROVED on `e115b540`).
  - The G3 bounded spec (`2d94cc26`).
- **Branch:** `docs/objective1-450-rack-marks-independent-review-20261113-v1`, from `origin/docs/objective1-443-independent-delta-review-20261113-v1`. This file is its only change.
- **Unity:** not launched. The lock was not taken, and no Unity process was running when I checked.
- **Author's lane:** I did not write there. I ran one `git status --porcelain` in it (clean, HEAD `acb4e111`), and the compile gate read `Library/ScriptAssemblies/*.dll` from it.
- **How code was inspected:** `git diff`, `git show` and `git archive` extracts only.
- **Reviewer scratch:** `/media/jer/Files/CCF/Verification/CCF-451-Objective1-RackMarks-Review-20261113/`. It holds:
  - `cand/` (the `7633eaf1` extract) and `base/` (the `e115b540` extract);
  - `probe-scripts/run_offline_patched.py` and `probe-scripts/compile_gate_patched.py` (path and git shims only);
  - `offline-rerun/`, `offline-rerun.log`, `compile-rerun/` and `compile-rerun.log`.

## Acceptance matrix (packet §5, points 1–6)

| # | Packet point | My verdict | Evidence I looked at |
|---|---|---|---|
| 1 | Before the first spray in the clearing, at both sizes: arrow rendered, heading to the bottom rack, minimap ring, distance, instructions, "Rack marks: 0 of 5", no rack ordinal or ID | **PASS** | **Viewed `w03-before-first-spray` at 1280×720 and 1920×1080.** Both show: a yellow arrow (down-left); "Next rack mark · 53 m"; "[P] Spray marker · aim at the cued trunk within 3.5 m"; the minimap ring with "Rack mark" at the bottom-left rack entrance; "Rack marks: 0 of 5" in the instruction, the progress line and the lower-left chip; "Objective 1 of 9". There is no "Rack <n>". I compared it with the R4 `w03` (old copy "Rack 1 entrance", "Rack 1", "Rack entrances marked: 0 / 5"): the layout is the same and only the text changed. **Receipts:** `ARROW_PIXELS` 219 and 487 yellow pixels; `BEFORE_FIRST_SPRAY` target PO0203 (Z −35.271, the lowest), cue on rack 1, player at (−0.43, 5.25). |
| 2 | Any order is legal; arrow and ring follow south→north; no label equates identity with count | **PASS** | **Default plan, viewed:** `w05` at 720p (bottom first: "1 of 5", next mark 19 m) and `g10` at both sizes (top second: "2 of 5", next mark 52 m, ring on the second-lowest rack). **Reversed-ID plan, viewed:** `450-reversed-top-first-count1` at 1080p (stable ID 1 / PO3803 marked first: "1 of 5", arrow still to PO0203 at 71 m) and `450-reversed-second-count2` at 720p ("2 of 5", next PO1204 at 19 m). **Receipts:** 28 `RACK_MARKS_FIELD` receipts (14 states × 2 sizes) give the counter, target tree and HUD line for every state. **Source:** `RackVisitOrder` and `TryGetRackFieldTarget` are byte-identical (scope section). |
| 3 | Repeat spray adds nothing; clear recomputes; a stale target fails closed | **PASS, with limits** | **3a, repeat spray:** PASS by source. `MarkRackEntrance` returns `NoChange` when `entranceMarked` is already true (`ScenarioOneManager.RackPlan.cs:406–407`), and the count is `racks.Count(RackHasCurrentMark)` (`:44`). The native step is weaker than the result doc says (R-2). **3b, clear:** NOT PROVEN and unavailable. `ClearRackEntranceMark` (`:423`) has no caller anywhere in `Assets/`; I confirmed this by grep. **3c, stale target:** PASS for field guidance on a bounded in-memory fixture (see "Invalid-target fixture"). |
| 4 | 5 of 5 → Work Plan approval → Advance → Annual Review acknowledgement; tree, rack and order associations unchanged on both plans | **PASS** | **Viewed:** `g11` at 1080p and `450-reversed-complete` at 720p ("Rack marks: 5 of 5 — complete"; arrow and ring gone); `g13` at 1080p (149 approved; "Advance one year" is the next action); `g14` at 720p; `g15` at 1080p. **Receipts:** `RACKS_AT_5_OF_5`, `SECOND_PLAN_IDENTITIES` and `SECOND_PLAN_FINAL` show own orders 30/18/34/36/31, all Pending and equal to each rack's removal set. Each of the ten successful sprays marked exactly the owning rack of the sprayed tree. `GEOMETRY_HASH` is `307C1ED538EA0D3E` both at confirm and at 5/5. `ANNUAL_EXECUTION`: 149 trees, −177446 cents, acknowledged. |
| 5 | Layout: no overlap; minimap 2.3–2.6× area; F-2 edge; F-3 clears; leak assertions that are neither too strict nor too lax | **PASS** | **Minimap:** all 34 `MINIMAP_AREA` receipts give 2.495–2.511×. I recomputed overlaps of the surface against the bottom bar, the right column and the reticle: 0 overlaps and no null rectangles. **Viewed:** `g08a` at 720p (the teal left edge is visible on the planning card; `PLANNING_CARD_PIXELS` 712 and 2304, the same as R4); `g14` and `g15` (the instruction clears after acknowledgement). **Assertions:** the harness checks are reviewed under "Test quality". |
| 6 | Negative controls; saves and PlayerPrefs protected; no build | **PASS (bounded)** | **Viewed:** `g17` at 720p (old world: "Objective 1 of 8", "Access routes: 0 of 5 planned", no rack target or ring) and `g18` at 1080p (Reference Future v1 banner, no HUD). `NEGATIVE_CONTROLS` all true; `saveWritten: false`. **Re-verified now from `/`:** `sha256sum -c --quiet` on `host37`, `backup81` and `production4011` all exit 0. **Reference files** in my candidate extract: `91cf5059…` and `789e3edd…`, unchanged. A separate personal Legacy40 save load is NOT PROVEN, as the author states. |

## Scope check

- `git diff e115b540..7633eaf1 -- Assets` changes exactly the four authorised files, +12/−10:
  - `UI/WalkingHudView.cs`
  - `UI/HudMinimapView.cs`
  - `ForestTreeMarkingManager.RackEntrances.cs`
  - `UI/RackLayoutTracker.cs`

  `diff -rq` of the two `Assets/` extracts lists only these files. There are no ProjectSettings, Packages, `.meta`, scene or prefab paths. My SHA-256 of each file on both sides equals `R/source-comparison.json`.
- **Every changed line is a string expression.**
  - The arrow line (`WalkingHudView.cs:178`), the minimap label (`HudMinimapView.cs:120`) and `RackFieldStatus` (`RackEntrances.cs:114–116`) change only their text.
  - In `RackSprayPrompt` (`:184–205`) only string literals differ. Every branch and call is the same; the author's offline contract checks this and I re-ran it.
  - `V2CounterText` (`RackLayoutTracker.cs:65–67`) changes only text.
  - The " — complete" suffix compares the existing derived count with the constant. It adds no state.
- **Byte-identical to `e115b540`:** `TrySprayRackEntrance` (`:132–160`), `TryRackSprayAim`, `EquipRackSpray`, `RackVisitOrder`, `RackVisitIndex`, `TryGetRackFieldTarget`, cue placement and `CueRackId`. The diff hunks touch none of them, and the offline contracts compare all six.
- **Minimap ring:**
  - The gate is unchanged: `model.HasWaypoint && ui.MapSource.RackFieldTargetOrdinal > 0` (`HudMinimapView.cs:112`).
  - The ring and label positions, the clamp, the motion pulse and `StandMapSource.RackFieldTargetOrdinal` are all unchanged.
  - The harness now also asserts the ring's left and top against the projected waypoint on every field capture.
- **No new state:** there is no new field, counter, save data or second ledger.

## Player-facing strings in the confirmed-plan guided field stage (as they now read)

I searched the source for every string that can reach this stage, not only the changed lines. Bold rows changed in this amendment.

| Surface / state | Text now | Source |
|---|---|---|
| Objective widget header | `NOW` · `Objective 1 of 9` · `Details [O]` · `Create access racks` | unchanged (curriculum position, legitimately numbered) |
| **Objective instruction, 0–4 marks** | `Next: Rack marks: X of 5. Return to the forest and mark the first designated rack-removal tree at the headland-side entrance of each rack: [P] spray marker, aim within 3.5 m, left-click.` | `Curriculum.cs:102` (unchanged), counter from `RackLayoutTracker` |
| **Objective instruction, 5 marks** | `Next: Rack marks: 5 of 5 — complete. Next, review and approve the rack removal work in the Work Plan [Tab], then advance one year.` (after approval: `… Open the Work Plan [Tab] and advance one year.`) | `Curriculum.cs:103–104` |
| **Objective progress line; lower-left chip** | `Rack marks: X of 5` / `Rack marks: 5 of 5 — complete` (chip appends `  ·  marks: FELL n, CROP n` if any) | `RackLayoutTracker.cs:65`, `WalkingHudView.cs:292` |
| **Arrow panel** | `Next rack mark · N m` / `[P] Spray marker · aim at the cued trunk within 3.5 m` | `WalkingHudView.cs:178,193` |
| **Minimap** | `Rack mark` (plus `N` compass) | `HudMinimapView.cs:120` |
| **Spray prompt, no trunk in reach** | `Spray marker · aim at a rack mark trunk within 3.5 m · [P] Stow` | `RackEntrances.cs:187` |
| **Spray prompt, valid unmarked entrance** | `Rack mark · [Left-click] Spray orange · [P] Stow` | `:202` |
| **Spray prompt, already marked** | `Rack mark already made · Orange · [P] Stow` | `:192–193` |
| **Spray prompt, locked rack** | `Rack mark locked · rack already harvested` (practically unreachable, see C-2) | `:198` |
| **Spray prompt, other trunk** | `Not a rack mark trunk · follow the rack mark cue · [P] Stow` | `:204` |
| Spray prompt, conflicts / invalid | `Crop Tree · remove its blue designation before spraying`; `Fell mark · remove its red designation before spraying`; `RACK INVALID — ADJUST PLAN: …` (3 variants, unnumbered) | `:199–201`, `RackPlan.cs:368,374,386` |
| Spray flash (0.18 s) | `Sprayed orange · four sides marked` | `WalkingHudView.cs:249` |
| Not spray-equipped, aiming at a tree | `[E] Inspect · [M] Stand Map · Plan Racks · [Tab] Plan` | `WalkingHudView.cs:241` (pre-existing) |
| Transient row | `Spray marker equipped · [Left-click] Spray · [P] Stow.`; `Spray marker stowed.`; `Marked the entrance tree PO0203.`; `This rack entrance is already marked.`; `Aim at the entrance trunk and move within 3.5 m to spray.`; `That tree is not a rack entrance. Follow the entrance cue.`; `This rack has already been worked; its entrance cannot be marked now.`; Crop/Fell conflict lines; `Orange rack entrance: reopen and change that rack …` | `RackEntrances.cs:95–96,138–147`, `RackPlan.cs:400–419`, `ForestTreeMarkingManager.cs:233,239,270`. All unnumbered. `There is no rack {id}.` (`RackPlan.cs:403`) is unreachable from the spray because the ID comes from `ConfirmedRacks`. |
| Full screens during the stage | Objectives: progress `Rack marks: X of 5`. Stand Map confirmed card: `Rack marks: X of 5`. **Work Plan before 5/5: top line `Rack marks: X of 5. Return to the forest …` and bottom line `Mark the rack entrances in the forest before approving the rack work: X / 5 marked. …` (R-1)** | `StandMapView.cs:597`; `ScenarioOneUiRoot.cs:451,485`; `WorkPlanView.cs:150–153`; `Sequence.cs:196` |

**Result:** no `Rack <number>`, stored rack ID, `Rack entrances marked` or `X / 5` text can reach the walking HUD, the objective widget, the prompt, the minimap or the transient row in this stage.

- **Captures:** all field captures I viewed agree.
- **Harness:** `CheckGuidedField` runs the no-number guard over *all* visible `TextElement`s on every confirmed-plan field capture: 28 receipts, all PASS.
- **Left alone, as the packet requires:** "Objective 1 of 9", the planning counter "Racks planned: X / 5" (`RackLayoutTracker.cs:67`, `StandMapView.cs:598`), tree IDs and the F-1 lines.
- **The one old-format string still reachable** in this stage is the Work Plan approval advisory (R-1).

## Counting and identity

**Counting**

- `MarkedRackEntranceCount` is defined as `rackPlanConfirmed ? racks.Count(RackHasCurrentMark) : 0`, where `RackHasCurrentMark` is `entranceMarked && entranceTreeId` non-empty. Each rack is one persisted record with a single boolean, so:
  - the count is the number of distinct marked racks, in any order;
  - a repeat spray cannot add to it (`NoChange`).
- **The total "5" is correct for every reachable plan.**
  - `RackPlanTargetCount` is the constant 5.
  - A plan cannot be confirmed with any other count (`RackEditing.cs:114–115`).
  - Save validation rejects a confirmed V2 plan without exactly five racks (`ForestSaveValidation.RackV2.cs:60–61`).
  - So "X of 5" cannot show a wrong total.
- **Before confirmation:**
  - the shared counter reads "Racks planned: X / 5" (unchanged);
  - `RackFieldStatus` is `""`;
  - `TryGetRackFieldTarget` returns false, so there is no arrow and no ring.
- **On reopen:** the counter returns to "Racks planned". The marks are kept in the records and return to the count on re-confirmation (unchanged behaviour).
- **Native counts:** `w03` 0, `w05` 1, `g10` 2, then 3/4/5 (`SPRAY_FOLLOWING_CUE` `countBefore` 2→3, 3→4, 4→5). Reversed plan 0→1 (top first)→2 (bottom)→…→5. All agree.

**Identity, on both plans**

- **Default plan:** stable IDs 1–5 run south to north (PO0203, PO1204, PO2003, PO2803, PO3803).
- **Reversed plan:** stable IDs 5–1 run south to north.
- **Every spray:** the receipts show `newlyMarkedRackIds` equal to the owner of the sprayed tree, orange paint on that tree, and own orders equal to the frozen removal set, all Pending.
- **Geometry and orders:** the hash is unchanged after marking.
- **No renumbering:** no rack ID or order association is renumbered by this amendment; the manager file is untouched since `e115b540`.

## Invalid-target fixture (packet point 3c): accepted as a bounded PASS

`InvalidTargetProbe` (`Objective1UxNative.cs`) runs at zero marks, right after `w03`.

**What it does**

1. It takes the live `RackSaveData` record of the first rack in `RackVisitOrder`.
2. It replaces that record's `entranceTreeId` with the absent ID `450-MISSING-TREE`.
3. It checks the real `TryGetRackFieldTarget`: it must return false, return **that same first rack**, a null tree and a non-empty problem.
4. It then adds the absent ID to the removal set, settles 16 frames and rechecks. The result must again be false and the same rack, now with "no longer standing".
5. It checks that `CueRackId == 0` and `RackFieldTargetOrdinal == 0`, that the minimap ring and the arrow panel are hidden, and that no marks were added.
6. In `finally` it restores the ID and the set membership. It then verifies the geometry and orders hash, first target and cue are back as they were.

**Why I accept it:** this exercises the production selection code and the real HUD and minimap rendering with a corrupted rack record. It is exactly what the packet asks: an invalid target fails closed and does not jump to the next rack or an arbitrary tree. The `INVALID_TARGET` receipt matches: no target, no cue, no HUD line, no minimap destination.

**What it does not cover:**

- loading a corrupted disk save (save validation would refuse most such saves anyway);
- spraying at an invalid target;
- a world where the player has set an ordinary map waypoint. There, `WalkingHudView` falls back to that player-chosen destination. That is pre-existing behaviour and not an arbitrary tree.

**N-4 still applies:** the player is told nothing about *why* the arrow vanished (see the open-notes table).

## Test quality (harness diff `e115b540..7633eaf1`)

**Replaced assertions**

| Old assertion | New assertion | Judgement |
|---|---|---|
| HUD label `StartsWith("Rack 1 entrance · ")` + `EndsWith(" m")` + `CurrentOrdinal()==1` | `StartsWith("Next rack mark · ")` + `EndsWith(" m")` + `CurrentOrdinal()==1`; plus, on every field capture, the exact text `Next rack mark · {horizontal distance:0} m` | Equal or stronger |
| Minimap label `== "Rack 1"` + `RackFieldTargetOrdinal==1` + ring visible | `== "Rack mark"` + the same ordinal and visibility checks; plus waypoint = exact target tree position and ring left/top = projected pixel −12 | Stronger |
| Far tree prompt `StartsWith("Rack 5 entrance")` | Exact `Rack mark · [Left-click] Spray orange · [P] Stow` + no-number guard | Weaker at this line: the prompt can no longer name *which* rack is aimed at. The identity check moved to `SprayAndRecord`, which requires the newly marked rack to own the aimed tree and `EntranceTreeId == tree.TreeId`, with own Pending orders. **Net equal.** |
| Already-marked prompt `StartsWith("Rack 5 entrance · Orange")` | Exact `Rack mark already made · Orange · [P] Stow` + guard | Equal (identity covered by the `SECOND_SPRAY_TOP` receipt) |
| Reversed plan `StartsWith("Rack "+ordinal+" entrance")` | Exact unnumbered copy, then the per-spray own-rack and own-orders checks; the order is changed to top first | Equal; the identity proof is unchanged |
| Offline `StartsWith("Rack 1/5 entrance")` ×3 | Exact strings + guard; plus the real `V2CounterText` getter executed at 0/2/5, after repeat, and planning | Equal or stronger |

**Additions:** `CheckGuidedField` adds pairwise non-overlap of four panels, the next suggestion equal to the first unmarked/unlocked rack in `RackVisitOrder`, cue identity, and agreement of the counter with the objective progress. Nothing was dropped. The check count rose from 522 to 928.

**The leak guard** is `UnnumberedRackLabel`: `\brack\s+[0-9]+\b`, case-insensitive, and it requires non-empty text.

- Empty or whitespace text fails, so it cannot pass vacuously. The exact-equality checks beside it also fail on empty text.
- It accepts "Objective 1 of 9", "Racks planned: 1 / 5", "Rack marks: 1 of 5" and tree IDs (offline examples).
- It would **miss** `Rack1`, `Rack #1`, `rack-1` or word numerals (C-4). No such form exists in production today, and the exact-string checks would catch any change to the four seams.

**Run integrity**

- `result.json` `head` = `candidate.txt` = `7633eaf1…`.
- `staged-sources.json` SHA-256 `57c174c9…` equals `git show 7633eaf1:…/Objective1UxNative.cs | sha256sum`.
- The log has `OBJECTIVE1_UX_NATIVE_PASS checks=928 mismatches=0 saveWritten=False` at line 35645, and `Cleanup mono` at line 35745.
- The reap (exit −9, `reaped_after_verdict: true`) came after both the verdict and Unity's cleanup marker. All PNGs and receipts were written before the verdict line.
- **The reap does not affect the validity of the evidence.** It means only that this Editor did not exit normally, the same known shutdown hang as earlier runs.

**Offline re-run (mine):** `run_offline.py` from `7633eaf1`, path- and git-shimmed to my extract (`diff 55863f1b..7633eaf1` and `e115b540..7633eaf1`). Result: **54 probes + 46 contracts, exit 0**, the author's count. The working-tree-only checks (`diff HEAD`, `ls-files --others`) are trivially empty against an extract.

**Compile re-run (mine):** `compile_gate.py`, sources from my extract, references from the lane's `Library` (read only). Result: **PASS, 193 sources, 261 refs.**

## Status of D-1 and N-1..N-4

| Note | Status now |
|---|---|
| **D-1** Stand Map editor, plan-edit feedback, harvested lock and Treatment Review use stored creation-order rack numbers | **Open, Manager decision. Less severe.** The field no longer shows any number, so the "Rack 1 in the field vs Rack 5 on the map" contradiction is gone. A player who hand-draws or edits racks still sees creation-order numbers on the map: the `StandMapView.cs:613` heading, `StandMapRackEditor.cs:204,348,375,399,405,465`, `RackPlan.cs:141,190,233` and `ScenarioOneTreatmentReview.cs:202`. All are unchanged by this candidate. I would still recommend a later, separately scoped copy cleanup (option b in #449). |
| N-1 help backdrop alpha 0.94 | Carried forward, unchanged. User judgement. |
| N-2 full-map Destination diamond vs stored waypoint | Carried forward, unchanged. |
| N-3 enlarged minimap vs later planting hotbar | Carried forward; not exercised. |
| **N-4** invalid first target: guidance fails closed, problem text invisible | Carried forward. It now has native fail-closed evidence (above). `RackFieldStatus` is still rendered nowhere, so both its problem suffix and its new " — complete" suffix are invisible. **This matters for the own-world re-test.** If the player's first-in-order rack is invalid while others are still markable, the arrow and ring disappear with no explanation. The global "No unmarked rack entrance is currently available…" reason appears only when *no* rack is markable (`ScenarioOneUiRoot.cs:485–496`). |

## New findings, ranked

### R-1: the Work Plan shows both the new counter and the old "X / 5 marked" advisory before 5/5 (Minor; NON-BLOCKING; needs a scope ruling)

- **Where:**
  - `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs:196` (`RackMarkingProblem`, the excluded file): "Mark the rack entrances in the forest before approving the rack work: {X} / 5 marked. [P] spray marker, aim within 3.5 m, left-click."
  - It is displayed by `WorkPlanView.cs:152` as the bottom feedback line, and by `ScenarioOneUiRoot.cs:127` on a Skip attempt, which also needs the Work Plan.
- **Failure scenario:**
  1. The player sprays two marks and presses [Tab]; the bottom bar advertises "[Tab] Work Plan".
  2. The Work Plan opens. Its next-action line (`ScenarioOneUiRoot.cs:451`; the `<5` branch at `:485` leaves `reason = learning.CurrentInstruction`) reads "Rack marks: 2 of 5. Return to the forest and mark …".
  3. The feedback line at the bottom of the same screen reads "… rack entrances … 2 / 5 marked."

  So the two formats *do* appear side by side, on the Work Plan screen. The author's statement that they never appear together holds only for the walking HUD.
- **Effect:** inconsistent terminology and number format, not an identity leak. The numbers agree, and no rack number is shown. The packet's wording rule covers the guided field HUD, so this does not fail an acceptance point.
- **Evidence:** source analysis only. This run has no capture of the Work Plan before 5/5; `w07`, `g12` and `g13` are all after 5/5.
- **Fix:** outside the four files. A one-line copy change at `Sequence.cs:196`, for example "Mark every rack entrance in the forest before approving the rack work. Rack marks: {X} of 5. …", or reusing `ui.RackLayout.V2CounterText`. **This needs explicit Manager/user scope release.** I recommend accepting it for this candidate and folding it into the D-1 copy cleanup.

### R-2: the native repeat-spray step does not prove the click reached the game (Minor; test quality; NON-BLOCKING)

- **Where:** `Objective1UxNative.cs`, the step after `SECOND_SPRAY_TOP`.
  - It runs `yield return Native(Click());` and then checks `count == 2 && CurrentOrdinal() == 2`.
  - The `REPEAT_SPRAY` receipt records count and guidance only.
- **Failure scenario:** if the OS click were dropped (focus lost or cursor unlocked), the check would still pass, because an unprocessed click also leaves the count at 2.
  - The driver log shows that click seq 17 was sent at `…186.606`.
  - A handled repeat spray always sets the transient "This rack entrance is already marked." (`TrySprayRackEntrance` → `NoChange` → `SetMessage`). The harness does not assert it.
  - The `g10` capture cannot settle it either way. Its 1280×720 PNG was written at `…189.467`, about 2.9 s after the click, so the 2.5 s message window had probably expired. Neither `g10` image shows any message.
- **Effect:** the author's claim (3a) "a real OS left-click on the already orange far tree … leaves count 2" is true only for the *sent* click. That the game *handled* a repeat spray is not natively proven.
- **Why not blocking:** the property itself is proven by source (count derived from per-rack booleans, `NoChange` on a marked rack), and no production change is involved.
- **Fix, in the allowed test directory only:** after the click, assert `marks.TransientMessage == "This rack entrance is already marked."` and capture a `post-repeat` pair within 2.5 s. Then rerun only if the Manager wants native proof of this step.

### Cosmetic (not blockers)

- **C-1, new copy semantics.**
  - "aim at a rack mark trunk" and "Not a rack mark trunk · follow the rack mark cue" read slightly as if the trunk already carried a mark. "aim at the cued trunk", the arrow panel's own phrase, would be clearer.
  - "Rack mark · [Left-click] Spray orange" on an unmarked tree reads acceptably as "this is where the rack mark goes".
  - The transient messages still say "entrance": "This rack entrance is already marked.", "Aim at the entrance trunk …", "That tree is not a rack entrance. Follow the entrance cue." They are in the byte-protected `TrySprayRackEntrance` or the excluded manager. The packet allows entrance-tree explanation, so the vocabulary is mixed but not wrong.
- **C-2, unreachable branch:** "Rack mark locked · rack already harvested" can show only for a locked rack whose entrance is unmarked. In V2, approval requires all marks, and a harvested entrance tree is felled, so aim fails first. The wording "mark locked" for a mark that does not exist is therefore unlikely ever to be seen.
- **C-3, repetition (pre-existing):**
  - "Next: Rack marks: …" stutters, and "Next: … complete. Next, review …" repeats "Next".
  - The counter appears three times in the HUD (instruction prefix, progress line, lower-left chip).
  - After 5/5 the bottom bar still offers "[M] Stand Map · Plan Racks", and inside the Work Plan the line says "Open the Work Plan [Tab]".
  - The 720p "3.5 / m" wrap in the arrow panel remains (#449 C-1).
- **C-4, leak-guard gaps:** the guard misses `Rack1`, `Rack #1` and word numerals (see "Test quality").
- **C-5, stale comments:** `RackLayoutTracker.cs:57` and `StandMapView.cs:596` still describe the old counter text. They are not displayed; the author disclosed this.

## Result document accuracy (`acb4e111`)

**Accurate:**

- the exact candidate and the four-file, +12/−10 diff;
- the before/after table (I checked every row against source);
- byte identity of the six methods;
- counts and run receipts;
- the arrow pixels and minimap figures;
- the reap described as a shutdown workaround after the verdict;
- save, lease and drift handling;
- the reference hashes;
- `ClearRackEntranceMark` having no in-game caller;
- the invalid-target fixture and its limits;
- D-1 audited but not resolved;
- the four G3 deferrals unchanged;
- the unreproduced own-world arrow stated as a limit;
- no approval, integration or acceptance claimed.

**Inaccurate or overstated (minor):**

1. "old and new counters do **not** appear side by side" is wrong for the Work Plan screen before 5/5 (R-1).
2. Point 3a overstates the native repeat-spray proof (R-2).
3. The table lists `RackFieldStatus` rows as player-facing strings. That property is rendered nowhere; the visible counter is `V2CounterText`. The doc says this only for the problem suffix.
4. "The actual counter property executes … after repeat": the getter is real, but the "repeat" uses the offline stub's `MarkRackEntrance`, so it is tautological. The production no-double-count guarantee comes from source.

None of these changes the verdict.

## Verified myself versus taken from receipts

**Verified myself:**

- **Source and scope:**
  - the full `Assets` diff, file hashes and absence of non-code paths;
  - byte identity of protected methods (diff hunks plus re-run contracts);
  - the minimap gate and placement;
  - the counter derivation and total-of-5 invariants (confirm and save validation).
- **Wording:**
  - the source-wide search for numbered or ID rack strings and the guided-stage string inventory;
  - the Work Plan side-by-side path (R-1);
  - transient-message reachability.
- **Harness:**
  - the invalid-target fixture logic;
  - the assertion-by-assertion harness diff and the guard regex behaviour;
  - the repeat-spray timing from driver and PNG mtimes (R-2).
- **Run evidence:**
  - `result.json`, candidate, staged-harness hash and log token order;
  - parsing of all receipts, with overlaps and area ratios recomputed.
- **Re-runs and integrity:**
  - offline gate re-run (54 + 46) and compile gate re-run (193/261);
  - protected-save manifests (exit 0 ×3);
  - Reference hashes;
  - no Unity process running.

**Screenshots I viewed:**

| Run | Captures |
|---|---|
| 450 run (`7633eaf1`) | `w03` (both sizes), `w05` (720), `g10` (both), `second_spray_top-post-spray-rack-5-stable-5` (720), `450-reversed-top-first-count1` (1080), `450-reversed-second-count2` (720), `450-reversed-complete` (720), `g11` (1080), `w07` (720), `g08a` (720), `g13` (1080), `g14` (720), `g15` (1080), `g17` (720), `g18` (1080) |
| Baseline R4 (`e115b540`) | `w03` (720), for comparison |

**Taken from receipts and logs, not re-observed:**

- that the input was real OS input;
- annual execution and the second-advance refusal;
- old world unmodified and disk slot unwritten;
- lease acquisition and release, Editor drift restoration and licence-copy removal;
- the 1080p F-2 pixel counts;
- the content of the 69 PNGs I did not open.

## Not verified

- The player's own-world "no arrow from the clearing" symptom: unreproduced, cause unknown. N-4 is one plausible mechanism, unproven.
- The Work Plan screen before 5/5 (R-1): source only, no capture. **Settled by:** a native capture of `[Tab]` at 2 marks in a fresh V2 world.
- That a native repeat spray is handled (R-2). **Settled by:** asserting the `NoChange` transient within 2.5 s of the click.
- Native clear-mark (no caller), a corrupted disk-save load, a save/load round trip after marking, a personal Legacy40 save, and a continuous walked route (trunk approaches are teleported aiming fixtures).
- Later-objective HUD fit, Player or Windows build, FPS.
- Human judgement of the new wording, the F-2 edge and the N-1 backdrop.

## Smallest follow-ups, if the Manager wants them (none required for this verdict)

1. **R-1 (needs scope release for `LearningObjectivesView.Sequence.cs`):** a one-line copy change so the Work Plan advisory uses "Rack marks: X of 5". Rerun the offline gate and one native capture of the Work Plan at 2 marks.
2. **R-2 (test directory only):** assert the repeat-spray `NoChange` message and capture it within 2.5 s. Rerun the native route only if wanted.
3. **C-1 (inside the four files, optional):** change "aim at a rack mark trunk" to "aim at the cued trunk", and "Not a rack mark trunk · follow the rack mark cue" to "Not the cued trunk · follow the cue". Rerun the offline gate and the field captures.
