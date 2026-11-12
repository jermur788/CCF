# Unified UX Repair V1 — requirement and acceptance matrix

**DRAFT acceptance criteria; not yet a PASS.** Each entry must carry source/offline evidence where applicable and 1280×720 / 1920×1080 native evidence for visible behaviour. P = PASS, F = FAIL, U = UNVERIFIED (all currently U).

| ID | Finding | Acceptance witness | Current |
|---|---|---|---|
| AC-01 | #1 | First post-onboarding HUD makes [O]/Objectives unmistakable without an unrelated target competing; Reduce motion displays equally actionable static cue | U |
| AC-02 | #2 | Objectives' Stand Map button highlighted; redundant adjacent word "Next" absent only there; functional route intact | U |
| AC-03 | #3 | Objectives title plainly largest level; tabs legible and clearly separated from subtitle/body at 720/1080 | U |
| AC-04 | #4 | Objective 1 current diagram at same visible anchor before/after More detail open/close and Esc/reopen, without unexpected scroll; repeat 3 times, both resolutions | U |
| AC-05 | #5 | Stand Map help title/summary/detail remove redundant phrases; M/Esc rendered keycaps, clickable Back to forest not masquerading as a key | U |
| AC-06 | #6 | Actual Plan Racks target highlighted plus gentle first-guided-only pulse; Reduce motion static cue; no approval/delete/advance pulse | U |
| AC-07 | #7 | Map left working context and middle overview/legend readable; right Plan Racks instructional card visually distinguishable | U |
| AC-08 | #8A | Rotation grip conveys hover and drag affordance, different from pan/move/default; no mistaken click or focus loss | U |
| AC-09 | #8B | ONLY IF separately approved: near grounded suggested alignment snap, obvious indicator and reversible drag-away; Shift/manual override, ghost/result consistency; otherwise explicitly DEFERRED | U |
| AC-10 | #9 | Source geometry and native map screenshot compare existing paths, lower-left planned corridor and strips; decision recorded, **no unapproved edit** | U |
| AC-11 | #10 | Tree Marking modal background significantly de-emphasized; foreground high contrast; no click-through, focus escape or broken Esc/continue | U |
| AC-12 | #11 | Selected natural orange four-side spray instruction shown; physical semantics intact | U |
| AC-13 | #12 | Contextual post-confirm intro and P keycap correct; no wrong implied action | U |
| AC-14 | #13 | Exact requested deletion span removed only after unique-rule audit; preserves orange/red/blue distinctions, marking does not fell, Work Plan Approve then separate Advance | U |
| AC-15 | #14A | **Before first spray** lower/bottom entrance clearly cued in HUD + minimap, initial first-use pulse/static alternative | U |
| AC-16 | #14B | After lower rack marked, next unmarked target and label correspond to actual entrance in stable south-to-north visit order; no false Rack 1 at wrong end | U |
| AC-17 | #14C | **SAFETY GATE** each paint action attaches exact tree ID to the intended stable rack; preview/pending work order and 5/5 identity trace consistent; any mismatch STOP, not cosmetic PASS | U |
| AC-18 | #14D | Minimap perceptually much more legible and substantially larger while keeping world view, HUD, player/waypoint readable at 720/1080; record measured bounds and overlaps | U |
| AC-19 | #15 | After 5/5, Tab is keycap styled; displayed count never treated as mapping proof | U |
| AC-20 | #16 | Work Plan modal dimmed background, focus and pointer intercept sound; genuine buttons reachable | U |
| AC-21 | #17 | Work Plan title/tabs/body clear hierarchy and breathing room, panels/scrollers/buttons fit 720/1080 and Approve/Advance remain distinct | U |
| AC-22 | #18 | With Annual Review already on screen, guidance never says open Work Plan [Tab] to read the review; latest-year acknowledgement state still reachable, earlier years read-only | U |
| AC-23 | #19 | Redundant "Read WORK DONE, MONEY and FOREST" footer instruction absent; sections and acknowledgement unchanged | U |
| AC-24 | #20 | User-selected ongoing-management footer copy avoids false "one thinning" after rack clearance; relevant detailed help reconciled | U |
| AC-25 | all | No gameplay/rack/persistence/review-history/world hash/reference/Legacy40 regression; no source overlap with #441 | U |
| AC-26 | all | Actual 720/1080 native captures + scoped offline tests + independent review + Manager verdict; no Windows build, Tier-3 claim or user acceptance from docs alone | U |

**Release condition:** all mandatory entries PASS, AC-09 approved-and-PASS or explicitly deferred, AC-10 documented decision/no unsafe geometry change, AC-17 independently witnessed, and outstanding user choices resolved before their corresponding source copy ships.
