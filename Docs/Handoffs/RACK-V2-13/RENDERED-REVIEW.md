# RACK-V2-13 — Objective 1 rendered review

**Task:** RACK-V2-13, claim #98, umbrella #91.
**Reviewed candidate:** `dc4f59b` integration head; functional captures produced at `a900b7f`.
**Reviewer:** DeepSeek #91 operator (actual model: DeepSeek V4.1 Flash), because the Claude/OpenAI delegated reviewers were rate-limited. This is a direct inspection of the real PNG captures, not an assertion from code.
**Verdict:** `OBJECTIVE 1 VISUAL PASS` — with the non-blocking findings below.

## Captures inspected

- RACK-V2-12 `r12-02/evidence`: `c02-map-before-planning`, `c03-plan-racks-suggested`, `c04-edited-plan-spacing`, `c05-confirmed-map`, `c06-tree-marks-help`, `c07-workplan-before-marking`, `c08-first-entrance-target`, `c09-five-entrances-marked`, `c10-workplan-approved`, `c12-objective-2-crop-tree`; 1280x720 and/or 1920x1080 as named below.
- RACK-V2-09 `field-05/evidence`: `03-orange-side-0`, `03-orange-side-1`, `03-orange-side-2` at 1920x1080.

## Findings

### Passes

- **Map before planning / suggested / edited / confirmed (720p and 1080p):** dashed suggested routes and solid player racks are visually distinct; the legend confirms the language. No giant glowing rack rails. The 16–20 m advisory is framed as this stand's guide, and the selected-rack panel shows 17.8 m to the nearest rack as within range.
- **Confirmed map:** the 0 / 5 entrance counter, 166 pending orders and Reopen action are clear.
- **Tree Marks help:** readable, only orange/red/blue entries, with the orange convention and first-tree simplification disclosure.
- **Work Plan before marking:** the marking-before-approval guidance is visible and understandable; the functional gate confirms approval is refused.
- **First entrance target:** the cue names the rack and distance (`Rack 1 entrance · 2 m`), with a clear spray prompt.
- **Orange mark:** the face-on side (field-05 `03-orange-side-0`) shows a bright, believable orange vertical stripe on the trunk; side-1/side-2 show a thinner edge-on sliver.
- **Both resolutions:** no material clipping or unreadable text in the reviewed states.
- **Objective 2 transition:** the confirmed rack remains the teaching area and the real C Crop Tree toggle works.

### Non-blocking polish findings

1. The action prompt sits across the middle of the trunk/stripe in the marking state; the stripe remains visible above and below, but the prompt competes with the mark.
2. The marked count appears twice: in the Objective card and in the lower status line.
3. The orange patches are narrow (24° each); from an angle between patches the mark is a thin sliver. A wider patch or a small radial offset would make four-side readability more consistent.
4. The Work Plan's disabled `Approve Work Plan` button is visually similar to the enabled `Advance one year` button; the functional refusal is correct, but the disabled state could be clearer.
5. At 720p the modal Help panel covers much of the HUD; this is expected for a modal but worth watching in a small window.

None of these is a functional failure or a blocker to RACK-V2-14; they are recorded for the user's playtest and any later visual polish.

## Limitations

- Reviews are of the supplied 1280x720/1920x1080 captures, not a live walked-through session.
- The four-side guarantee is from the code's four stripe objects plus the face-on capture; the three supplied side captures do not all show a wide face-on stripe.
- No functional re-qualification, production change, Unity run or Windows build was made for this review.
