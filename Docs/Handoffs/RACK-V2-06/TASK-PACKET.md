# RACK-V2-06 — Spacing Teaching Implementation / Qualification Packet

**Task ID:** RACK-V2-06  
**Mode:** bounded implementation + focused qualification  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (APPROVED)  
**Prerequisite:** RACK-V2-05 APPROVED / INTEGRATED through PR #44 / merge `0ec6bc4d473975e9938040026737e70a33432c48`; Manager review `Docs/Handoffs/RACK-V2-05/MANAGER-REVIEW.md`  
**Minimum base:** current `main` must descend from the integrated RACK-V2-05 Manager-review lineage; refresh `origin/main` immediately before starting and record the actual starting HEAD in claim #45.

## Goal

Teach Scenario One rack spacing while the player edits, without turning the recommendation into a universal forestry rule and without silently snapping invalid plans into validity.

Deliver, while a rack is selected or actively dragged/rotated:

- distance to the nearest neighbouring **player rack**;
- Scenario One recommended band `16–20 m`;
- clear status wording for below / within / above the band;
- site-specific framing (for this stand / Scenario One), never a universal CCF rule;
- a planting-row equivalent only if the already-qualified RACK-V2-02 evidence supports one defensibly;
- validation feedback that says why a plan needs adjustment.

The player must still be able to deliberately deviate. Do not convert RACK-V2-05's advisory snapping into a hard rail.

## Evidence / wording authority

- D-064 / approved Rack V2 spec: 16–20 m is **GAMEPLAY CALIBRATION INFORMED BY DIRECT SOURCE GUIDANCE**, specific to Scenario One.
- Approximate row-equivalent display is **MODEL/SCENE DERIVATION** and may appear only if RACK-V2-02 measured and accepted a defensible value. If the evidence is absent/ambiguous, omit row count; do not invent one.
- RACK-V2-05 editing-feel thresholds are gameplay calibration. RACK-V2-06 may adjust presentation-facing thresholds only where necessary for spacing teaching; do not materially change editing semantics without returning to Manager.

## Scope

Primary expected surface:

- `Assets/ForestPrototype/UI/StandMapView.cs`
- narrow `Assets/ForestPrototype/UI/Map/*` spacing-display / draw-plan / labels / status plumbing
- `Assets/ForestPrototype/Racks/RackEditing.cs` or Rack editing facade only if a narrow reusable nearest-neighbour measurement API is genuinely cleaner than duplicating geometry in UI
- focused verification under `Tools/Verification/RackV2Spacing/` (or equivalent bounded location)

Reuse authoritative rack geometry. Do not create a second rack list, spacing counter or persisted teaching state.

## Explicit exclusions

- No RACK-V2-07 Confirm/Reopen/minimap implementation.
- No RACK-V2-08 first-person rack-author removal.
- No RACK-V2-09 field marking.
- No RACK-V2-10 Help expansion.
- No RACK-V2-11 save/schema work.
- No MFM implementation.
- No species/ecology/economy changes.
- No whole Objective-1 qualification.
- No Windows build.

## Copy requirements

Examples may follow the approved spec but exact values must come from live geometry:

- `18.1 m — within recommended range`
- `13.7 m — closer than recommended for this stand`
- `22.5 m — wider than recommended for this stand`

The UI must make `16–20 m` visibly advisory/site-specific. Do not say `ideal`, `required everywhere`, `standard CCF spacing`, or equivalent universal language.

If row-equivalent evidence is valid, use approximate wording such as `≈18 m — approximately N planting rows in this stand`; otherwise omit the row-equivalent line entirely.

## Verification

### Static / offline

Prove at minimum:

- nearest-neighbour selection is deterministic and uses player racks, not dashed suggestions;
- boundary values below, at 16 m, inside, at 20 m and above are classified correctly;
- one-rack / no-neighbour state is handled honestly;
- copy contains site-specific framing and no universal-rule wording;
- row-equivalent output is evidence-gated and omitted when unsupported;
- spacing feedback does not mutate rack geometry or authoritative plan state;
- RACK-V2-05 edit semantics and RACK-V2-04 dashed/solid map behaviour remain green;
- runtime type compile succeeds.

### Unity / rendered

Use the exact candidate and machine-wide Unity lock. Exercise real pointer or equivalent existing RACK-V2-05 interaction path at 1280×720 and 1920×1080 for:

- selected rack with a within-band neighbour;
- below-band and above-band feedback;
- active drag/rotate updating the displayed distance/status;
- deliberate off-band placement remaining possible;
- no-neighbour state;
- row-equivalent state only if authorised by evidence.

Capture readable evidence at both supported resolutions. This is focused feature qualification, not RACK-V2-13 final appearance acceptance.

### Queue

MFM-03 claim #39 is independently dispatched and may acquire Unity first. RACK-V2-06 may perform static/UI implementation in parallel. Before Unity, refresh live coordination and the lock; never bypass an actual owner. If #39 owns Unity, wait for its release rather than stopping the task.

## Stop conditions

Return to Manager if:

- RACK-V2-02 evidence is contradictory in a way that affects required copy rather than merely causing row-equivalent omission;
- nearest-neighbour spacing cannot be derived from authoritative rack geometry without duplicating state;
- satisfying the teaching requirement would materially alter RACK-V2-05 edit semantics;
- a conflicting live writer owns required files;
- protected Rack V2 behaviour regresses.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-06/HANDOFF.md`

Include starting main, implementation commits, exact tested candidate, final tip, changed files, copy/evidence classification, offline results, Unity/lock/render evidence, RACK-V2-04/05 preservation results, row-equivalent decision and limitations.

At publication rename the task claim `[HANDED-OFF]`, link the handoff and close it.

## Success exit

`SPACING TEACHING READY — READY FOR MANAGER REVIEW`
