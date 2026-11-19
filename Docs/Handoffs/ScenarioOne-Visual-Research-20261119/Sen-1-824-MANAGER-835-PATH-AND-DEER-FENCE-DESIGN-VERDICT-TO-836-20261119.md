# Sen-1 #824 — Manager review: #835 path/planting-edge and reusable deer-fence research to #836

**19 November 2026 ~19:50 UTC. Scenario One Manager:** [#824](https://github.com/jermur788/CCF/issues/824).  
**Verdict:** **APPROVED for research/concept design direction, with binding implementation clarifications; production implementation still separately gated**. Not owner sign-off of exact final look, asset licensing, unsigned world geometry, native Unity render or deer-exclusion gameplay.

## Evidence reviewed

- Actual [#835 issue registration and completed research handoff](https://github.com/jermur788/CCF/issues/835), [full final research/design report](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-PATH-AND-DEER-FENCE-VISUAL-RESEARCH-AND-T3-DESIGN-HANDOFF-20261119.md), and source content of all **three original, text SVG mockups**:
  - [A / path and planting edge](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-A-PATH-BLEND.svg), 1200×700 plan/before-after schematic
  - [B / fence at walking eye height](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-B-DEER-FENCE-IN-SCENE.svg), 1200×700 visual concept
  - [C / modular fence kit](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-C-FENCE-MODULE-KIT.svg), 1200×700 kit diagram
- Examined document semantics and SVG structures/text. These are **original schematics, not actual Unity screenshots or final mesh renders**. Additional raster concept PNGs in originating research chat are not tracked in GitHub and were **not independently inspected here**.
- Reviewed [#836 one-shot TG Fork production packet](https://github.com/jermur788/CCF/blob/docs/sen-1-path-fence-tg-fork-implementation-packet-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-836-TG-FORK-ONE-SHOT-PATH-PLANTING-EDGE-AND-REUSABLE-DEER-FENCE-IMPLEMENTATION-HANDOFF-20261119.md), overlapping #785/#688 art scope, #691 still-DRAFT graphics authority, and #830 recent worker native-release report.
- Independently cross-checked primary Irish public professional reference [Teagasc / forest fencing](https://teagasc.ie/crops/forestry/advice/establishment/fencing/): deer mesh **1.9 m** and **no barbed wire top**. UK Forestry Commission/Countryside Stewardship 1.8 m is a different jurisdiction/reference, not to silently replace Irish chosen design. Actual construction/engineering suitability, game-scale visibility, post rhythm and site topography remain to be calibrated.

## Approved design direction — concept authority only

### A. Improve actual Compact30 pedestrian woodland footpath

**ACCEPT** worn-earth, muted Irish woodland soil and needle litter with restrained fine grit, organic visual edge breakup and tile/roughness variation. Preserve **1.2 m walkable core and ≤0.20 m visual-only verge per side**. No motorized track or new internal extraction racks. High priority is a continuous, readable shape from the player's viewpoint, not detailed scatter that conceals poor geometry.

### B. Remove square discontinuity before cosmetic blending

**ACCEPT** a continuous ground plane, approved curve-aligned core and feathered distance-based visual mask, with sparse groundcover. **First inspect the actual Unity seam** from overhead/player views and determine whether the defect is a spline/polygon termination, disconnected terrain/ground mesh, UV/material join, planting patch, or vegetation bounds. **Do not merely cover an open geometric hole with decals**, move tree/G1 coordinates, modify collider/authorised planting eligibility or expand path width to hide it. If cure changes the unsigned A7/A8 authored route, ask the exact world owner/Manager for a narrow geometry approval instead of treating the research mockup as a new signed map.

### C. Reusable presentation-only deer-fence visual kit

**ACCEPT** approximate **1.9 m** tall galvanized rectangular wire netting with treated round timber post rhythm, stout braces/corners/strainers, plain top line, terrain-following mesh and no barbed wire. Deliver modular original components for straight, corner, end/strainer, slope adjustment and **optional gate only where actual authorised H1 access requires one**. Fence must be visibly believable in the real walking camera, not thick garden rails or an opaque wall; mesh LOD and performance are essential. Proposed **6–8 m** game-art post rhythm is an initial candidate to visually test, **not adopted Irish field specification**. Use original/procedural editable source files and trace provenance; do not copy third-party photography into production.

**Critical boundary:** Perimeter appearance does **not** itself create deer-exclusion/protection gameplay, alter grazing/browsing, change save data, or establish a new fence gameplay contract. Confirm actual property line and H1 entrance; do not choose convenient ornamental corners inconsistent with scene truth. Retain the accepted Compact30 context as independent 30×30m no-rack slice and existing outside visual continuation shell.

## Route to the already-created #836 one-shot native implementation

**#835 is accepted research; no further browser research/chat required.** The actual worker in [#836](https://github.com/jermur788/CCF/issues/836) must use installed **TG Fork `orchestrate-work` skill**, verify complete actual skill sources and routing, one parent, internal independent review and one final GitHub handoff. Its design input is **#835 report + SVG A/B/C + THIS Manager verdict**. This acceptance satisfies #836 **design-review prerequisite only**; not G1 production/Unity admission.

Before source/asset or physical Unity work:
1. **Existing #785/#688/#786 ownership:** #786 already reported creating a scratch H1/footpath kit. Inspect its existing source/export/render proof and present source/owner liveness; **prefer reuse/repair over a second competing path kit**. If source/asset files intersect, obtain explicit one-writer handoff or disjoint outputs. Oak/Beech kit #787 may still be in non-final review; do not touch it.
2. **#830 Unity:** Worker-reported real Editor 71/71 automated native checks and normal Unity lock release. **Manager has not accepted that result in this #835 review**; no automatic transfer of #830's one-time D-092 physical lease. Recheck live Editor occupancy and physical board before #836 Editor admission.
3. **Graphics/world/content scope:** #691 graphics four-file spec remains DRAFT, D-032 first-party asset rights/storage policy and actual authored C0 curve/geometry may be unresolved. Material art look is accepted for **concept iteration**, not automatic asset import, world byte/signature, save/prop changes, or scene integration. Only allow separately verified safe scratch work with exact claimant/physical owner as authorised; Manager source and Unity actor-specific permission needed before production import.
4. **Native source and lanes:** Exact source file allowlist/asset path, parent worktree/branch and child-writer ownership must be based on live repo/physical evidence. Neither this design approval nor [#836 existing packet] authorises a new Unity lock or duplicated scene writer. One session must be the sole Unity operator; no Windows build.
5. **Visual acceptance evidence:** actual 1280×720 and 1920×1080 player/oblique/overhead before-after of the specific bad join, fence boundary incl H1 access/corners, readability at distance, real-world ground contact, no obtrusive collider, no protected E80/Legacy40/Reference Future change. User's final look acceptance still distinct from native test PASS.

**Next executable management action:** acknowledge accepted design on #836, mark it READY for native G0 skill/ownership/source inspection only while source/Unity remains HELD pending actual file-owner and spec gates. Manager can then grant one coordinated self-contained implementation run when the current environment proves disjoint ownership and a lawful exclusive Editor admission. Avoid splitting the feature into repeated small manager packets. **No extra user visual-style decision requested at this stage**: use the preferred report look as an editable concept until user tests an actual render.

**Dashboard / Drive:** sync not verified; GitHub is canonical.
