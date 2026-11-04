# Scenario One next-action guidance specification

- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Status:** DRAFT — no implementation authority
- **Owner:** Overall Manager / user (approval); EVENT 012 design worker (draft)
- **Created / updated:** 2026-11-04
- **Decision references:** D-064 / D-065 binding; D-067–D-069 remain conditional directions
- **Milestone:** Scenario One player-test refinement; personal playtest remains unaccepted
- **Related:** [#258](https://github.com/jermur788/CCF/issues/258), [#254](https://github.com/jermur788/CCF/issues/254)
- **Supersedes:** none

## Product intent and problem

The user's complaint is: Objective 1 needs more detail and a diagram that looks like the Stand Map, not abstract rows of text. The contextual call to action should highlight the Stand Map from the Objectives screen, and then Plan Racks on the map, instead of a generic “Back to forest” button. Always highlight the next logical enabled action as the objective progresses, rather than the default close button.

## Authority and evidence classification

Latest direct user instruction, Decision Log, Current Milestone and Game Brief govern intent in that order. Inspected code governs implementation facts. This DRAFT describes proposals, not existing or approved behaviour. Plan/tasks cannot create product requirements. No ecological research claim is introduced: five racks and 16–20 m are site-specific teaching/gameplay calibration, not universal forestry recommendations. Existing D-064 geometry and D-065 treatment rules remain controlling; diagonal spine research does not authorise new geometry here.

## OBSERVED CODE FACT — source ledger

All paths below are relative to `Assets/ForestPrototype/`. Main means inspected `origin/main` **8f0f230190830855112e3e7a1d7544db94a7f591**; candidate means **72b4efbd1c653fa2231f632a79030a83ff76f833**. Citations are revision-specific, read with `git show`, not runtime evidence.

| ID | Source and observation |
|---|---|
| F1 | Main `UI/LearningObjectivesView.cs:23–54`: current lesson derives from `ScenarioOneTeachingSequence.Current(TeachingFacts())`; V2 instruction/counter delegate to RackLayoutTracker. `:67–69` makes Stand map and Work Plan available but Back to forest primary. Candidate `:23–59,73–75` retains that primary close action while adding presented-lesson selection. |
| F2 | Main `UI/LearningObjectivesView.Curriculum.cs:10–22,25–30`: stable optional Step IDs, reading/prerequisite metadata and legacy five-rack/seven-standing-row explanation. `:86–110` defines GuidedTopicIndex and NextCueStep (action steps before readings). Candidate `:98–104` adds RackV2Instruction with entrance-marking precedence; `:109–131` adds V2 step wording and preserves GuidedTopicIndex. These helpers are not the current lesson selector in F1. |
| F3 | Main `UI/LearningObjectivesView.Sequence.cs:12–37,61–70` derives facts from work/history and confirmed V2 sample geometry. Candidate `:12–37,61–90` retains that derivation with extra treatment extension facts. Do not substitute optional device credits for forest progression. |
| F4 | Main `UI/RackLayoutTracker.cs:56–80`: V2CounterText switches from planned count to entrance-marked count; main instruction jumps from confirmation to Work Plan. Candidate `:56–80` calls RackV2Instruction, correcting the missing entrance step. This dependency must be reconciled with #255; do not duplicate its copy. |
| F5 | Main `UI/StandMapView.cs:246–255,437–445`: `map-plan-rack` toggles planning via ToggleRackMode. `:565–646` builds edit counter, headland drag instructions, advisory 16–20 m spacing and `map-rack-plan-confirm`; CanConfirmRackPlan supplies enabled state/reason. `:649–658` uses ConfirmRackPlanFromMap with consent handling. Candidate `:192–215` retains editor gating; changes elsewhere include a Review layer, so candidate and main are not interchangeable. |
| F6 | Main `UI/Map/StandMapSymbols.cs:80–107`: suggested faint dashed line; player planned rack solid amber; drawing white dashed; refusal red dashed; selected bright outline; entrance blue square; far-end rotation white round handle; harvested pale solid strip. `:109–112` waypoint amber diamond, player blue dot/wedge, scale and north arrow. `:114–115` actual mark symbols are red/blue rings (older curriculum says square/diamond). Use renderer contract. |
| F7 | Main `UI/Map/StandMapDrawPlan.cs:228–248,307–319` draws rack corridor bands, lines, selection square and round handle using world-to-map transform. `UI/Map/StandMapSource.cs:341–365` reads existing Dirt Path planks; do not invent a connected spine. `UI/Map/StandMapRackEditor.cs:14–16,314–338` anchors creation/movement to headland. |
| F8 | Main `UI/ScenarioOneUiRoot.cs:335–355`: O toggles Objectives; M toggles map; Esc closes; Tab closes modal as manager opens Work Plan. `:466` ShowMap is existing entry. Main `UI/StandMapView.cs:284` cell buttons are non-focusable; `:185–188` delegates keys to editor; `UI/Map/StandMapRackEditor.cs:445–450` only Delete/Backspace on selected rack. No full keyboard geometry-authoring path was established. |
| F9 | Main `UI/LearningObjectivesView.cs:79–102,208–219` keeps optional practice in device PlayerPrefs while observing forest/work state; `:133–144` puts words beside TeachingDiagram. `UI/TeachingDiagram.cs:5–11,30–43` is passive code-drawn teaching art, explicitly not the player's map. `Racks/RackPlanner.cs:31–32` describes 2 m planted rows; this must not be mistaken for rack spacing. |

## PROPOSAL — functional requirements

**FR-1 Single cue.** Derive one transient presentation result (action target, visible verb, reason, enabled state) from existing current lesson, rack confirmation/count, entrance marks, rack-owned work status and existing availability validators. Reuse TeachingFacts/CurrentTopicIndex and candidate RackV2Instruction after base reconciliation. Keep GuidedTopicIndex/NextCueStep for their existing optional-practice purpose; do not build a second progression engine. Exactly one enabled primary action per active screen, including any blocking consent dialog. Other controls remain secondary. Re-evaluate after edits, confirmation, reopen, marking, approval, resolution, screen entry and load. Optional credits cannot skip required forest work.

**FR-2 Cross-screen precedence.** Apply the following first matching row; navigation requires explicit player activation. Opening Stand Map MUST NOT automatically enter planning, confirm, approve, mark or advance time.

| Derived condition | Objectives primary | Stand Map / destination primary |
|---|---|---|
| V2 plan unconfirmed; planning not active | Open Stand Map [M] (`ShowMap`) | Plan Racks (`map-plan-rack`, ToggleRackMode; no dedicated key found) |
| Planning active; fewer than five or invalid five | Open Stand Map [M] | Enabled existing corrective control when applicable; otherwise “Read planning guidance” focuses the status/instructions; disabled Confirm retains its reason |
| Exactly five; CanConfirmRackPlan true; no pending consent | Open Stand Map [M] | Confirm Rack Plan (`map-rack-plan-confirm`, ConfirmRackPlanFromCard → ConfirmRackPlanFromMap); button activation, no dedicated shortcut found |
| Consent/reset dialog pending | Open relevant map if elsewhere | Safe enabled “Keep my racks”/cancel choice first; destructive confirmation never auto-focused or executed |
| Confirmed; entrances marked below target | Return to forest to mark entrances (`CloseAll`) | Return to forest to mark entrances; explain P spray marker, aim within 3.5 m and left-click (candidate wording F2) |
| Entrances complete; rack-owned pending work | Review Work Plan [Tab] (`ShowWorkPlan`) | Work Plan navigation; within plan highlight existing approval only when its validator permits, retaining cost/all-pending-work warning |
| Rack work approved, awaiting resolution | Open Work Plan [Tab] | Existing Advance one year only when permitted; retain existing review gates |
| Work resolved | Existing lesson's review/inspection action | Follow TeachingFacts and existing lesson instruction; do not keep pointing at Plan Racks |

Reopened plans return to planning precedence; preserve stored entrance marks and harvested locks. Non-V2/Legacy40 uses existing legacy lesson and controls; never offer unavailable V2 planning. Later lessons follow the same single-primary rule using existing enabled actions; concrete later-lesson mapping needs Manager acceptance before dispatch (Q3).

**FR-3 Blocked/unavailable fallback.** Never highlight a disabled operation or skip its prerequisite. First offer an enabled corrective destination for the existing validator's reason. If no correction is available on this screen, one enabled “Read next-step guidance” action focuses/reveals the reason and existing help; no state mutation. If required services/state are missing, show “Guidance unavailable — reopen Objectives” and one safe Close action. Ordinary Back to forest stays secondary unless forest activity is actually next or no guidance can be supplied. Do not invent validity or bypass existing gates, including known MFM validity limitations.

**FR-4 Detail.** Objective 1 must explain permanent extraction access, five-rack target, site-specific advisory 16–20 m spacing, standing rows retained, and the separate plan → confirm → physically mark entrances → review/approve → annual execution stages. Reuse V2 instruction and derived counter verbatim where applicable. Confirmation creates pending work; approval commits work; successful annual resolution opens corridors. Counter and primary action must describe the same snapshot.

**FR-5 Faithful map-like illustration.** Replace only Objective 1's abstract teaching picture with a passive north-up stand-shaped map illustration. Recommended static teaching example: current approved five parallel racks connected to the existing headland, inside the actual stand boundary, with recognisable canopy/tree crowns and existing access path, grid, north and scale conventions. Reuse map transform/symbol definitions rather than hand-inventing a palette. Show five labelled R1–R5 planned corridor bands, entrance squares at headland, retained planted rows between corridors and a labelled perpendicular spacing bracket. Do not draw five abstract text rows. Retain canopy inside planned corridors: planned lines overlay trees, not holes/stumps. Show the round handle only in an explicitly labelled selected-rack example. Legend must say “Illustration of a proposed layout — not your saved plan. Planning clears nothing.”

Geometry must come from the approved reference layout at the eventual implementation base; verify boundaries, rack widths, actual headland side and spacing before drawing. Seven standing rows is the legacy authored example (F2), not a universal invariant of editable V2 racks: depict/count it only if the chosen reference supports it; otherwise label retained rows without a false count and seek Q2 decision. The figure may imply access intent, relative direction and retained canopy. It MUST NOT imply planned routes are cleared, confirmed, approved, executed or machine-qualified; no proposed diagonal 3+2 spine, automatic removal marks, or new access connection. A metric scale is allowed only with verified metric geometry; otherwise explicitly “schematic, not to scale”. Planned/harvested styles remain distinct. This scope produces a diagram specification, not an image asset.

**FR-6 Accessibility.** Primary emphasis uses “Next:” text, border/weight and visible focus indicator as well as colour. All symbols have shape/pattern plus written legend and text equivalent describing headland, five racks and retained rows. At 1280×720 and 1920×1080 preserve readable body/legend text (target at least 16 physical pixels), no clipped CTA/reason/counter, and scroll access to full explanation. Allow diagram below words at narrow width; never shrink labels to illegibility. Proposed focus order: next enabled action, remaining navigation, objective content/help, secondary close. Enter/Space activate buttons; Esc retains safe exit; no focus stealing during live refresh. Explicit navigation may place focus on next destination control; no automatic navigation or cursor movement. Tab conflicts with Work Plan shortcut (F8): Q1 must select a scoped navigation policy before implementation. Added guidance controls and all flow destinations must be keyboard reachable; full rack geometry editing is not claimed accessible by this draft and needs a separately agreed solution/scope if required for end-to-end keyboard-only play.

## Save / backward compatibility

Cue is derived from saved forest/rack/work state on load; no save schema change or new PlayerPrefs keys required. Preserve optional Step IDs and device-progress separation, confirmed/reopened plans, entrance marks, harvested locks, Legacy40 and frozen Reference Future v1. Session inspection evidence may repeat after load as designed. Exception: remembering dismissed cues, custom focus preferences or new completion flags would require separate approval and save/settings design; excluded here.

## Non-goals / security and write-back

No new backend, guidance engine, auto-navigation, auto-planning/marking/approval/year advance, geometry/rule/MFM change, packages, asset family, keybinding framework, telemetry or save migration. No onboarding artwork changes. Guidance display itself writes no world data; existing explicitly invoked operations retain their existing validation and consent boundaries. No Unity/build or production UI change in this task.

## Acceptance criteria and test matrix

All rows are future gates, **NOT RUN**. Exact implementation HEAD and screenshots/logs required. Use disposable saves, never the user's playtest save.

| Criterion | Required check | Resolution | Responsible role |
|---|---|---|---|
| AC-1 / FR-1–3 | Fresh plan 0–4, invalid five, valid five, unavailable service, consent, reopened and locked rack: exactly one enabled primary, truthful reason, no automatic operation | Both 1280×720 and 1920×1080 | UI implementer; independent reviewer verifies evidence |
| AC-2 / FR-2,4 | Explicit O → Stand Map M → Plan Racks → Confirm → forest P marking 0/5 through 5/5 → Tab review/approval → year/review; inspect pending vs approved vs completed; no duplicate awards/work | Both | Unity qualifier with authorised lease; user judges clarity |
| AC-3 / FR-5 | Compare figure with real Stand Map and approved geometry: five racks, headland, retained rows, widths/spacing, symbol shapes/legend; no false cleared/approved implication | Both, colour and grayscale review | Independent visual reviewer and user; geometry owner validates reference |
| AC-4 / FR-6 | Keyboard-only reachability of guidance/buttons, Enter/Space, Esc, chosen Tab policy, focus after transitions/refresh, readable counter/reason/legend with no overlap | Both | Accessibility/UI reviewer; qualifier records actual input |
| AC-5 / save | Save/load each boundary; reopen preserves marks; harvested locks; old V2 and Legacy40; device credits/reset cannot skip forest lesson; frozen reference unaffected | Both for presentation; offline state tests once per exact base | Implementer/offline tests plus Unity qualifier |
| AC-6 / later lessons | Current instruction and one cue agree through later teaching stages; optional credits do not override; blocked action resolves to truthful guidance | Both | UI reviewer against Manager-approved mapping |

## Stop conditions and open decisions

Stop implementation dispatch while DRAFT. Return to Manager if geometry cannot support truthful depiction, key reachability requires wider input changes, source helpers diverge, base lacks candidate instruction, or save/backend changes appear necessary.

- **Q1 — Manager/user:** choose modal keyboard policy. Recommendation: scoped focus navigation while preserving a clearly documented Work Plan shortcut; existing global Tab cannot simultaneously be assumed normal focus traversal. Reconcile #257 before coding.
- **Q2 — Manager/user:** approve static labelled example versus live read-only saved-plan inset. Recommend static approved parallel example, never an unqualified spine. Geometry owner must validate reference and whether seven standing rows applies.
- **Q3 — Manager:** approve Objective 1 first implementation slice and require an explicit later-lesson action mapping before extending general rule; no speculative actions. User's general next-enabled-action rule remains the product target.
- **Q4 — Manager:** choose integrated implementation base and reconcile #255 wording/helper reuse and #256 rack-handle UI before dispatch; no cherry-pick implied by this draft.
