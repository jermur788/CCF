# Scenario One — Objective 1 Rack Planning & Field Marking V2 Bounded Tasks

- **Spec ID:** SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1
- **Spec status:** APPROVED
- **Task list owner:** Overall Manager
- **Last updated:** 2026-10-28

Tasks are implementation units, not new product decisions. They stay inside the approved spec and plan. If a task exposes a material product ambiguity, stop and return it to the Manager/spec, citing the spec section.

**Provenance.** The task IDs, titles, deliverables and exit tokens (`Exit:`) are as supplied with the approved spec. Scope, exclusions, dependencies, verification, stop conditions and completion states were derived at authoring time from the spec and plan and add no product requirement. The dependency order is a recommendation for the Manager.

**Routing (all tasks).** Primary `SOL / CODEX`. Tasks marked *TERRA-suitable* contain bounded visual/UI work or rendered review that `GPT-6.1 TERRA` may take. The Overall Manager assigns owner, lane, Unity ownership and quota source through the task packet. Every task needs a packet that names its exact base, context commit and branch; the implementation base must be the current Scenario One candidate containing qualified UI Visual System V2 (see the plan's *Ownership / concurrency considerations*). Heavy Unity work needs the Unity ownership lock and, for Tier-2, G1 launch approval.

**Decisions** (spec §39.7, Decision Log D-064): OPEN-1 (V2 applies to Enlarged80 only) and OPEN-2 (keep `rack.approve` and `rack.result`; Confirm creates pending work only) are accepted. OPEN-3 (reopening after approval or execution) was accepted by direct user authority on 2026-10-28 (Decision Log D-064): a rack with zero executed removals may be reopened/edited through the authoritative reconciliation/cancellation path; reopening itself cancels nothing; cancelling approved-but-unexecuted work requires a warning and explicit consent; any rack with one or more executed removals is geometry-locked and represented as harvested; executed removals are never reversed; only racks whose geometry changes lose geometry-bound entrance-mark state.

## RACK-V2-01 — Inspect current architecture

- **Task ID:** RACK-V2-01
- **Status:** TODO
- **Goal:** Produce the implementation inventory the plan requires before any edit, and confirm or correct plan findings F-1 to F-10 on the actual implementation base.
- **Scope:** Rack data model; counter ownership; map and first-person creation paths; persistence path; execution/removal path; tree-mark architecture; Help system; minimap representation; Objective 1 state and teaching sequence; stand/path geometry. Verify the facts behind the accepted OPEN-1 and OPEN-2 decisions (the Enlarged80/Legacy40 code boundary and the Confirm → approve → execute → result transitions), report the safest boundary for OPEN-3, and give the per-consumer decision for F-8.
- **Exclusions:** Read-only. No edits, no builds, no Unity writes.
- **Owner / routing:** SOL / Codex
- **Dependencies:** —
- **Verification:** Written inventory with file and line citations, separating verified facts from assumptions.
- **Stop conditions:** The named base lacks the map or rack code; a finding contradicts a spec requirement; the exclusive-mark model cannot support the plan's R-3 (spec §39.6 item 5).
- **Completion state:** Exit: `ARCHITECTURE MAPPED`. The inventory is delivered to the Manager with the OPEN-3 boundary recommendation and confirmation of the facts behind the accepted OPEN-1 and OPEN-2 decisions.

## RACK-V2-02 — Geometry qualification

- **Task ID:** RACK-V2-02
- **Status:** TODO
- **Goal:** Measure the verified scene and decide whether five racks at about 16–20 m is geometrically coherent.
- **Scope:** Usable stand width perpendicular to the intended rack direction; path/headland geometry; current suggested rack positions; plantation row spacing and regularity (to decide whether a row equivalent can be shown, spec §15); outer-rack edge margins. Propose the ~18 m suggested layout if coherent.
- **Exclusions:** No production change. No distortion of distances to keep five. Legacy40 is out of scope for V2 (OPEN-1, D-064).
- **Owner / routing:** SOL / Codex (Unity read-only diagnostic; needs the Unity lock)
- **Dependencies:** RACK-V2-01
- **Verification:** Live Unity measurement, not offline replica arithmetic. Evidence stored under `/media/jer/Files/CCF/Verification/…` with the harness inspected before running.
- **Stop conditions:** Five racks cannot be made coherent with the band on the verified geometry: `STOP — PRODUCT/GEOMETRY CONFLICT` (spec §6, §39.6 item 1).
- **Completion state:** Exit: `FIVE-RACK CALIBRATION VERIFIED`, with measurements, margins and the row-equivalence decision; or exit: `STOP — PRODUCT/GEOMETRY CONFLICT` with the numbers.

## RACK-V2-03 — Authoritative rack model

- **Task ID:** RACK-V2-03
- **Status:** TODO
- **Goal:** Establish one authoritative rack-plan object set with stable identity and state, and make every counter derive from it.
- **Scope:** Rack objects per spec §11 (identity, entrance/path anchor, direction, length, spacing relationship, plan status, field-marking status); derived `Racks planned X / 5` and `Rack entrances marked X / 5`; the confirmed-plan to rack-work transition per the accepted OPEN-2 (Confirm creates or refreshes pending orders only; edits before approval reconcile them without duplicates); the persistence field design (wiring and tests land in RACK-V2-11).
- **Exclusions:** No map UI, no 3D, no second rack-removal system, no growth, competition or economy change.
- **Owner / routing:** SOL / Codex
- **Dependencies:** RACK-V2-01, RACK-V2-02
- **Verification:** Offline/static tests plus a focused runtime check that all counters agree from a single source, including after load.
- **Stop conditions:** The model needs a second removal system or duplicated forestry outcomes (spec §39.6 item 4); the schema change threatens Reference Future v1 or old saves (item 2) and needs G5.
- **Completion state:** Exit: `SINGLE SOURCE OF TRUTH`. No independently incremented rack counter remains.

## RACK-V2-04 — Map planning mode

- **Task ID:** RACK-V2-04
- **Status:** TODO
- **Goal:** Make `Plan Racks` a tool/mode control, visually separate from the navigation tabs, and show the suggested layer.
- **Scope:** Spec §9 (control placement, `Plan Racks` / `Planning Racks — Active` states), §10 (suggested layout on first entry, dashed, not counted), §32 and §33 (qualified UI Visual System V2, dashed/solid language, legend `Dashed = suggested` / `Solid = your planned rack`).
- **Exclusions:** No separate map-editor theme; no change to the navigation tabs' behaviour; no editing interactions yet.
- **Owner / routing:** SOL / Codex; *TERRA-suitable* (visual/UI)
- **Dependencies:** RACK-V2-03
- **Verification:** Tier 1 smoke plus a rendered check of inactive, active and first-entry states.
- **Stop conditions:** UI V2 tokens or components are unavailable on the named base.
- **Completion state:** Exit: `MAP PLANNING MODE READY`.

## RACK-V2-05 — Editing

- **Task ID:** RACK-V2-05
- **Status:** TODO
- **Goal:** Direct mouse manipulation of rack objects.
- **Scope:** Create, select, move (entrance re-snaps to the path/headland), rotate (about the entrance, by dragging the far endpoint or a handle), delete (visible button and Delete/Backspace), `Undo last rack edit`, and `Reset to suggested layout` with confirmation if edited. Soft snapping toward the suggested orientation and parallel to nearby player racks (spec §12, §13), without forcing a fixed rail.
- **Exclusions:** No general-purpose undo; no automatic placement; no reopen-after-confirmation behaviour (RACK-V2-07).
- **Owner / routing:** SOL / Codex; the UI portions are *TERRA-suitable*
- **Dependencies:** RACK-V2-04; OPEN-3 resolved
- **Verification:** Real-pointer interaction tests for each action; negative cases (degenerate length, duplicate/overlapping racks, outside the property).
- **Stop conditions:** A rack cannot establish a valid entrance on the path/headland geometry.
- **Completion state:** Exit: `RACK EDITING READY`.

## RACK-V2-06 — Spacing teaching

- **Task ID:** RACK-V2-06
- **Status:** TODO
- **Goal:** Teach spacing as Scenario One guidance while the player edits.
- **Scope:** While a rack is selected or dragged, show the distance to the nearest neighbour, the `16–20 m` band and a status in the spec §14 wording, with the recommendation clearly site-specific. Show the row equivalent only if RACK-V2-02 verified one (spec §15). Validation messaging that says *why* a plan needs adjustment, and never snaps an invalid plan into validity (spec §16).
- **Exclusions:** No universal-rule wording; no invented row count.
- **Owner / routing:** SOL / Codex; *TERRA-suitable*
- **Dependencies:** RACK-V2-05
- **Verification:** Boundary-value checks (inside, below and above the band), copy review against spec §14, §15, §31.
- **Stop conditions:** The row equivalent is not defensible: omit it (this is not a stop).
- **Completion state:** Exit: `SPACING TEACHING READY`.

## RACK-V2-07 — Confirmation + minimap

- **Task ID:** RACK-V2-07
- **Status:** TODO
- **Goal:** Confirm the plan and propagate it to the forest and minimap.
- **Scope:** `Confirm Rack Plan` enabled only for a valid five-rack plan; the counter switch from `Racks planned X / 5` to `Rack entrances marked X / 5` (spec §17, §19); reopen behaviour with the warning and per-rack mark reset (spec §18, within the resolution of OPEN-3); minimap uses the same dashed/solid language and shows the first-time note (spec §20).
- **Exclusions:** No 3D waypoints or spraying yet (RACK-V2-09).
- **Owner / routing:** SOL / Codex; minimap visuals *TERRA-suitable*
- **Dependencies:** RACK-V2-06
- **Verification:** Confirm, reopen-unmarked and reopen-after-partial-marking paths; only geometry-changed racks lose their mark; minimap and map agree.
- **Stop conditions:** Reopen would require reversing an executed removal or creating a second removal mechanism, rather than locking that rack as harvested (OPEN-3 accepted 2026-10-28).
- **Completion state:** Exit: `CONFIRMED PLAN PROPAGATES`.

## RACK-V2-08 — Remove competing 3D authoring

- **Task ID:** RACK-V2-08
- **Status:** TODO
- **Goal:** Leave the map as the only rack author.
- **Scope:** Disable or remove Objective 1's independent first-person rack creation, including its key entry, controls/hint text and in-forest authoring panel, while preserving the world interaction the objective still needs (spec §8, AC-2).
- **Exclusions:** No removal of the underlying work-order system; no change to ordinary individual Fell/Crop marking.
- **Owner / routing:** SOL / Codex
- **Dependencies:** RACK-V2-07 (the replacement path must exist first)
- **Verification:** Attempt every former first-person route; none creates or counts a rack. Individual marking still works.
- **Stop conditions:** Removal breaks a subsequent objective (spec §37).
- **Completion state:** Exit: `NO COMPETING RACK AUTHOR`.

## RACK-V2-09 — Field marking

- **Task ID:** RACK-V2-09
- **Status:** TODO
- **Goal:** The player physically marks each confirmed rack's entrance tree in orange.
- **Scope:** Entrance-tree targeting (first rack-removal tree nearest the path, near the centreline, in the removal set; otherwise `RACK INVALID — ADJUST PLAN`) (spec §23); subtle waypoint cue that clears after marking (spec §21); spray-marker use with range, aim, valid-target feedback, short sound/visual (spec §27); a persistent orange mark with vertical stripes shown on four sides from one interaction (spec §24); mark-conflict prevention against Crop Tree and Fell (spec §26); mark state persisted on the rack object.
- **Exclusions:** No glowing world rack lines; no freehand painting; no other mark categories; no growth or competition change.
- **Owner / routing:** SOL / Codex; the mark visual is *TERRA-suitable*. Any scene, prefab or material change is single-writer.
- **Dependencies:** RACK-V2-07, RACK-V2-08
- **Verification:** An unrelated tree cannot satisfy the objective; a Crop Tree cannot be orange; the mark survives save/load; the cue clears; the marked-count derives from rack objects.
- **Stop conditions:** The mark cannot be made consistent with the exclusive Fell/Crop marks (spec §39.6 item 5); a needed serialized-asset edit cannot be reviewed as a diff.
- **Completion state:** Exit: `FIELD MARKING READY`.

## RACK-V2-10 — Help / teaching

- **Task ID:** RACK-V2-10
- **Status:** TODO
- **Goal:** Teach the marking convention and its gameplay simplification, contextually.
- **Scope:** `Tree Marks` help unlocked when field marking first becomes relevant, containing only the orange, red and blue entries (spec §28); the orange text and the first-tree simplification disclosure; objective copy meeting spec §31 and the Objective 1 flow of spec §30.
- **Exclusions:** No unused future marks; no universal-practice claims.
- **Owner / routing:** SOL / Codex; *TERRA-suitable* (copy and layout)
- **Dependencies:** RACK-V2-07, RACK-V2-09
- **Verification:** Copy review against spec §5.1, §7, §22, §28, §31; unlock timing; no `18 m ideal` or `always five racks` wording.
- **Stop conditions:** Copy would assert something the evidence index (spec §39.3) does not support.
- **Completion state:** Exit: `MARKING TEACHING READY`.

## RACK-V2-11 — Persistence

- **Task ID:** RACK-V2-11
- **Status:** TODO
- **Goal:** Prove new and old saves behave correctly.
- **Scope:** New game; old save before rack planning; old save with existing rack progress (if such a state exists); save/reload after confirmation; after partial entrance marking; after all marks; stable rack IDs preserved; Legacy40 save loads unchanged and does not enter V2 (OPEN-1, D-064); malformed V2 data loads without a crash.
- **Exclusions:** No reserialisation of Reference Future v1 through current save classes (D-042); no unrelated schema change.
- **Owner / routing:** SOL / Codex
- **Dependencies:** RACK-V2-03, RACK-V2-07, RACK-V2-09
- **Verification:** Tier 2; the harness is inspected first, backs up and restores `forest-save.json`, and leaves no generated `.meta` under `Assets/ForestPrototype/`.
- **Stop conditions:** Any existing Scenario One save is silently invalidated; G5 is needed for the schema change and not yet given.
- **Completion state:** Exit: `RACK V2 SAVE PASS`.

## RACK-V2-12 — Functional qualification

- **Task ID:** RACK-V2-12
- **Status:** TODO
- **Goal:** Verify the whole Objective 1 flow end to end.
- **Scope:** `open map → plan → edit → confirm → walk → spray 5 entrances → complete`, continuing through `rack.approve` → execution → `rack.result` (OPEN-2), with negative cases and a Legacy40 save that never enters V2; subsequent objectives remain reachable; no forestry growth, competition, species or economy regression; existing rack/guided/teaching gates updated or retired explicitly (plan F-10).
- **Exclusions:** No rendered-appearance judgement (RACK-V2-13); no Windows build.
- **Owner / routing:** SOL / Codex
- **Dependencies:** RACK-V2-08, RACK-V2-09, RACK-V2-10, RACK-V2-11
- **Verification:** Tier 2 with a narrow regression set, per plan section 11; G3 for any harness or expected-value change; independent review by a separately assigned reviewer.
- **Stop conditions:** A protected anchor moves, or a subsequent objective is blocked.
- **Completion state:** Exit: `OBJECTIVE 1 FUNCTIONAL PASS`.

## RACK-V2-13 — Rendered review

- **Task ID:** RACK-V2-13
- **Status:** TODO
- **Goal:** Review the rendered map and 3D states at both supported resolutions.
- **Scope:** At 1280 × 720 and 1920 × 1080: map before planning; Plan Racks active; selected rack and handles; spacing display; confirmed map; minimap; first entrance target; orange painted tree; Objective/Help state. Check that handles and spacing text are readable, minimap lines are understandable, orange marks look believable on trunks, and there are no giant glowing rack lines.
- **Exclusions:** No functional re-qualification; no unrelated UI redesign.
- **Owner / routing:** SOL / Codex; *TERRA-suitable* (rendered review)
- **Dependencies:** RACK-V2-10, RACK-V2-12
- **Verification:** Captured images stored in the Files evidence tree and reviewed by a person or reviewer, not asserted from code.
- **Stop conditions:** A state cannot be reached in the live scene.
- **Completion state:** Exit: `OBJECTIVE 1 VISUAL PASS`.

## RACK-V2-14 — Integration / dashboard

- **Task ID:** RACK-V2-14
- **Status:** TODO
- **Goal:** Reconcile with the current Scenario One candidate and the live coordination state.
- **Scope:** Integration with the current candidate and the onboarding visual work; authoritative coordination and dashboard synchronised; G4 review; spec status moved through `IMPLEMENTING` / `VERIFYING` / `VERIFIED` only by the Manager.
- **Exclusions:** No Windows build (not authorised); the user's playtest is still required and is not claimed.
- **Owner / routing:** Integrator assigned by the Overall Manager (one integrator per integration worktree)
- **Dependencies:** RACK-V2-12, RACK-V2-13
- **Verification:** Post-integration gates per AgentWorkflow; conformance evidence recorded in `checklists/requirements.md`.
- **Stop conditions:** Conflicts in shared or serialized systems that cannot be understood from both sides.
- **Completion state:** Exit: `INTEGRATED CANDIDATE READY`. This is not user acceptance.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| RACK-V2-01 | TODO | SOL / Codex | — |
| RACK-V2-02 | TODO | SOL / Codex | RACK-V2-01 |
| RACK-V2-03 | TODO | SOL / Codex | RACK-V2-01, 02 |
| RACK-V2-04 | TODO | SOL / Codex; TERRA-suitable | RACK-V2-03 |
| RACK-V2-05 | TODO | SOL / Codex; UI TERRA-suitable | RACK-V2-04 (OPEN-3 resolved 2026-10-28) |
| RACK-V2-06 | TODO | SOL / Codex; TERRA-suitable | RACK-V2-05 |
| RACK-V2-07 | TODO | SOL / Codex; minimap TERRA-suitable | RACK-V2-06 |
| RACK-V2-08 | TODO | SOL / Codex | RACK-V2-07 |
| RACK-V2-09 | TODO | SOL / Codex; visual TERRA-suitable | RACK-V2-07, 08 |
| RACK-V2-10 | TODO | SOL / Codex; TERRA-suitable | RACK-V2-07, 09 |
| RACK-V2-11 | TODO | SOL / Codex | RACK-V2-03, 07, 09 |
| RACK-V2-12 | TODO | SOL / Codex | RACK-V2-08, 09, 10, 11 |
| RACK-V2-13 | TODO | SOL / Codex; TERRA-suitable | RACK-V2-10, 12 |
| RACK-V2-14 | TODO | Integrator (Manager-assigned) | RACK-V2-12, 13 |
