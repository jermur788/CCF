# RACK-V2 G4 — Final Independent Conformance Review

**Date:** 2026-10-30  
**Browser review claim:** #185  
**Reviewer:** Overall Manager / OpenAI GPT-5.6 Sol  
**Independence:** browser/GitHub review only; did not implement or locally qualify Rack V2  
**Approved authority:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`, D-064  
**Exact integrated Rack candidate:** `60fd9025e0205e7e8b24203295dd606ffa4a8061`  
**Current published Scenario One integration at review:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`  
**Ancestry:** GitHub compare proves `60fd902...` is the merge base / ancestor of `8fa83bff...` (current integration is 126 commits ahead, 0 behind).

## Verdict

`OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`

- **Spec conformance:** PASS
- **Implementation correctness:** PASS for the reviewed/integrated Rack V2 feature scope
- **Integration:** PASS at exact Rack candidate `60fd902...`, retained in current published integration ancestry
- **User acceptance:** NOT CLAIMED — personal user playtest is still required
- **Windows build:** NOT AUTHORISED / NOT RUN

This G4 verdict closes the independent final review that `Docs/Handoffs/RACK-V2-14/HANDOFF.md` explicitly left separate. It does not promote the entire Scenario One candidate to player-test ready and does not waive later Tier-3 integration checks.

## Evidence reviewed

### Integration / task closure

`Docs/Handoffs/RACK-V2-14/HANDOFF.md` records the integrated Rack candidate `60fd902...`, all RACK-V2-01…14 task states, protected compatibility and the technical exit `INTEGRATED CANDIDATE READY`.

The current published integration `8fa83bff...` descends from `60fd902...`; this review therefore does not rely on a local-only or abandoned Rack candidate.

### Field marking / mark architecture

`Docs/Handoffs/RACK-V2-09/HANDOFF.md` and `INDEPENDENT-REVIEW.md` establish on exact `8f8316b5a815ba8e7b68a9015923f940ae2ec8d2`:

- field gate PASS: `RACK_V2_FIELD_MARKING_PASS checks=99 shots=10`;
- unchanged Rack runtime gate PASS: 14 checks;
- unchanged Rack save gate PASS: 35 checks;
- authoritative entrance targeting with no unrelated-tree substitution;
- fresh use-time aim/range checks;
- orange state projected from the authoritative rack record rather than a second ledger;
- reciprocal Crop/Fell/orange conflict rules;
- Enlarged80/model-1 gating and Legacy/model-0 isolation;
- OPEN-3 changed-rack mark invalidation and executed-rack lock semantics;
- independent review verdict `RACK-V2-09 REVIEW PASS — APPROVED FOR INTEGRATION`.

The independent review found no blocking defect. Its N1–N5 findings were bounded follow-up observations; the real execution path was subsequently exercised by RACK-V2-12.

### Persistence / old saves / malformed data

RACK-V2-11 Unity qualification records exact candidate `a6ad231a13d87fe7b11d3bbef046b92fb9520e84`:

- V2 real-disk checks: 30 PASS;
- cold-start Continue: 2 PASS;
- Legacy40: 7 PASS;
- existing 03B save gate: 35 PASS;
- 21 malformed V2 files refused without crash or world mutation;
- stable rack IDs / `nextRackId` continuation;
- pre-V2 / model-0 behavior remains model 0;
- Reference Future Year-100 anchor `7AD177B3CC2F73C7` unchanged;
- player save slot restored, Unity lane clean and lock released;
- no Rack schema change beyond the already accepted v21 representation.

This is direct disk/cold-start evidence, not only in-memory round-trip evidence.

### End-to-end Objective 1

`Docs/Handoffs/RACK-V2-12/HANDOFF.md` records exact candidate `a900b7faec3e48f6cfc8be7e3bb3b147fe3a61aa` and Unity 6000.6.0f1 qualification:

- `RACKV2_O1E2E_PASS checks=173 shots=24`;
- `RV2L_MODEL0_PASS`;
- `RV2L_LEGACY40_PASS`;
- fresh game → map → Plan Racks → suggested/edit/invalid-refusal/undo/move → Confirm → Tree Marks help → approval-before-marking refused → five native spray interactions and negatives → Work Plan approval → execution / `rack.result` → Objective 2 → real Crop Tree toggle → disk save/load;
- IDs, geometry, marks, work-order state and executed locks preserved.

The in-scope fix makes the D-064 commitment sequence explicit: Rack V2 work cannot be approved before required entrance marking is complete. The manager's ordinary approval API/model-0 path was not replaced.

### Rendered review

`Docs/Handoffs/RACK-V2-13/RENDERED-REVIEW.md` is a direct inspection of real 1280×720 and 1920×1080 captures and returns `OBJECTIVE 1 VISUAL PASS`.

It confirms:

- dashed suggested versus solid player racks;
- visible site-specific 16–20 m advisory and spacing state;
- confirmed-map/minimap readability;
- Tree Marks help with orange/red/blue and the first-tree simplification disclosure;
- marking-before-approval guidance;
- real first-entrance cue and spray prompt;
- believable face-on orange vertical stripe;
- no material clipping/unreadable text in reviewed states;
- Objective 2 transition remains understandable.

The five reported visual findings — prompt/stripe competition, duplicated marked count, narrow edge-on orange patches, weak disabled-button styling and 720p modal coverage — are retained as non-blocking user-playtest/polish notes. They do not contradict an acceptance criterion.

## Requirements / acceptance matrix

### Product and evidence framing

| Requirement | Verdict | Evidence |
|---|---|---|
| Map is sole rack-geometry author | PASS | RACK-V2-08 accepted in R14 task table; R12 complete flow authors/edits from Stand Map only. |
| 3D remains part of learning via physical marking | PASS | R09 native spray path; R12 five entrance interactions. |
| No competing first-person rack creation | PASS | R08 completion carried into R14; R12 legacy/model-0 checks preserve only intended legacy behavior. |
| Five racks coherent with actual Enlarged80 geometry | PASS | R02 `FIVE-RACK CALIBRATION VERIFIED` carried as completed in R14; R12 runs the five-rack plan and R13 displays 17.8 m within the site-specific band. |
| Five / 16–20 m are Scenario One calibration, not universal law | PASS | R10 teaching qualification carried into R14; R13 directly observes `this stand` guidance. |
| Orange source/derivation wording honest | PASS | R10/Tree Marks help and R13 rendered review; first-tree simplification is explicitly disclosed. |

### Map UX / counters / minimap

| Requirement | Verdict | Evidence |
|---|---|---|
| Plan Racks is a tool/mode, not navigation tab | PASS | R04/R10 accepted; R13 rendered state. |
| Suggested dashed / player solid | PASS | R13 direct rendered inspection. |
| Select / move / rotate / delete / undo / reset | PASS | R05 accepted in R14; R12 exercises edit, invalid refusal, undo and move in the integrated flow. |
| Spacing shown and invalid plan explained | PASS | R06/R13; R12 exercises four-rack confirm refusal. |
| Confirm Rack Plan exists and propagates | PASS | R07/R12. |
| Planned/marked counters derive from rack records and remain separate | PASS | R09 independent architecture review; R12/R13 displayed state. |
| Minimap suggested/planned distinction and confirmed propagation | PASS | R07 completion; R13 direct rendered inspection. |

### Field marking / conflicts

| Requirement | Verdict | Evidence |
|---|---|---|
| Player physically approaches and sprays authoritative entrance target | PASS | R09 native input/range/aim negatives; R12 five native spray interactions. |
| Unrelated / invalid target cannot satisfy | PASS | R09 99-check gate + independent review. |
| Orange vertical four-side representation | PASS | R09 structural four-stripe representation plus direct face-on capture; R13 visual acceptance. Edge-on thinness retained as non-blocking polish. |
| Mark persists save/load | PASS | R09 in-memory + R11 real disk/cold-start. |
| Entrance cue clears / advances | PASS | R09 independent review. |
| Orange/Crop/Fell incompatibilities prevented | PASS | R09 independent review and negative gates. |
| OPEN-3 edit/lock behavior preserved | PASS | R09 independent review plus R12 execution path; executed removals are not reversed. |

### Teaching

| Requirement | Verdict | Evidence |
|---|---|---|
| Why racks / spacing / site-specific recommendation explained | PASS | R10 static qualification; R13 rendered review. |
| Paint meaning and gameplay simplification explained | PASS | R10 + R13 Tree Marks help. |
| Contextual help exists | PASS | R12/R13. |

### Persistence / compatibility

| Requirement | Verdict | Evidence |
|---|---|---|
| New game / old save / partial plan / confirmed / partial marking / complete marking save-load | PASS | R11 real disk matrix. |
| Stable rack IDs | PASS | R11. |
| Legacy40 remains outside V2 | PASS | R11 + R12 `RV2L_LEGACY40_PASS`. |
| Malformed V2 refused safely | PASS | R11 21 malformed cases. |
| Frozen Reference Future preserved | PASS | R11 Y100 `7AD177B3CC2F73C7`, R14 compatibility record. |

### Behaviour / visual / user-test gate

| Requirement | Verdict | Evidence |
|---|---|---|
| No growth / competition / species / economy redesign | PASS | Rack production scope does not introduce those model changes; R12 model-0/Legacy progression remains green; R14 records no such regression or schema/reference movement. |
| Subsequent objectives remain reachable | PASS | R12 reaches Objective 2 and performs a real Crop Tree toggle. |
| No unrelated UI redesign | PASS | bounded Rack UI/teaching/marking scope in accepted task chain. |
| 720p + 1080p map and 3D review | PASS | R13 direct capture inspection. |
| Handles / spacing / minimap / orange marks readable; no giant glowing world rack lines | PASS | R13, with documented non-blocking polish notes. |
| Objective 1 internally qualified | PASS | R09–R14 evidence chain. |
| Candidate integrated with Scenario One | PASS | exact `60fd902...`; ancestry retained in `8fa83bff...`. |
| User player test still required | PASS AS GATE STATE | explicitly not claimed by R14 or this review. |
| No Windows build before authorization | PASS AS PROCESS STATE | no Windows build reported in the reviewed Rack evidence; still prohibited. |

## AC-1 … AC-20 disposition

- **AC-1** map sole geometry author — PASS.
- **AC-2** no competing first-person geometry author — PASS.
- **AC-3** create/select/move/rotate/delete — PASS.
- **AC-4** suggested/player racks distinct — PASS.
- **AC-5** spacing visible — PASS.
- **AC-6** 16–20 m framed as Scenario One guidance — PASS.
- **AC-7** five-rack calibration fits verified geometry — PASS.
- **AC-8** Plan Racks visually a tool — PASS.
- **AC-9** plan confirmation — PASS.
- **AC-10** minimap states understandable — PASS.
- **AC-11** one authoritative rack counter/source — PASS.
- **AC-12** physical entrance-tree spraying — PASS.
- **AC-13** orange four-side vertical representation visible/persistent — PASS, with edge-on readability retained as non-blocking polish.
- **AC-14** marked counter separate from planned counter — PASS.
- **AC-15** incompatible mark conflicts prevented — PASS.
- **AC-16** help explains convention + simplification — PASS.
- **AC-17** old saves load — PASS.
- **AC-18** no simulation/growth regression attributable to Rack V2 — PASS on scoped-diff + protected/model-0/Legacy progression evidence; this is not a claim that every later Scenario One subsystem is globally regression-free.
- **AC-19** rendered map/3D review — PASS.
- **AC-20** workflow understandable without old implementation knowledge — PASS for internal qualification based on R12 end-to-end path + R13 rendered states; personal user comprehension remains to be tested in the user's playtest.

## D-064 accepted decisions

- **OPEN-1 — V2 applies to Enlarged80 only:** PASS. Legacy40/model-0 remains viable and is refused V2 state where appropriate.
- **OPEN-2 — Confirm creates/refreshes pending work; marking precedes `rack.approve`; ordinary execution → `rack.result`:** PASS. Directly exercised by R12.
- **OPEN-3 — zero-executed racks may reopen/edit through reconciliation; executed racks lock; no executed removal reversed:** PASS. R09 independent review plus integrated flow evidence.

No parallel rack execution mechanism was introduced.

## Non-blocking retained notes

Carry to the personal user playtest / later polish backlog:

1. spray/action prompt visually competes with the orange stripe;
2. marked count is duplicated in two HUD locations;
3. the four orange patches can appear narrow edge-on;
4. disabled `Approve Work Plan` styling could be more distinct;
5. 720p Help modal covers much of the HUD.

These should be observed during the eventual personal Scenario One playtest. They are not acceptance blockers on current evidence.

## Manager disposition

Rack V2 / D-064 is **VERIFIED for its feature scope** and may be treated as an accepted dependency for the later exact integrated Scenario One candidate, subject to ancestry preservation and the final Tier-3 integrated qualification.

This review does not claim:

- the current whole Scenario One integration is Tier-3 qualified;
- MFM/parity/S1-C/S1-D are complete;
- the user has accepted the feature;
- a Windows build is authorised.

No production files, saves or Unity state were changed by this browser review.
