# UI readability and accessibility — static audit (Workstream V)

**Status:** static audit of `UI/*.cs` and `UI/Resources/ScenarioOneUi.uss` at `a8596df` [REPO]. **No rendering was performed in this task.** Prior rendered evidence is cited where it exists (`Docs/Scenario1FinalPresentationAcceptance.md`; P1 captures on `task/scenario-one-pedagogy-p1`). Every BLOCKER needs a rendered confirmation in its fix packet.

## 1. Facts that drive the findings

| Fact | Value | Source |
|---|---|---|
| Panel scaling | `ScaleWithScreenSize`, reference **1600 × 900**, match 0.5 | `ScenarioOneUiRoot.Start` |
| Effective scale | ≈ 0.80 at 1280 × 720; 1.0 at 1600 × 900; 1.2 at 1920 × 1080 | derived |
| Base font sizes | body 15 px · muted 13 · faint 12 · stat label 12 · heading 16 · title 22 · prompt 18 | `.uss` |
| Effective size at 1280 × 720 | body 12 px · **muted 10.4 · faint 9.6 · stat label 9.6** | derived |
| Text colours | ink (236,240,228), muted (176,190,168), faint (132,146,126) | `.uss` tokens |
| Panel background | rgba(14,22,16,0.86) | `.uss` |
| Marking forecast | `treatment = UiKit.Add(bottom, "", "muted")`. **No panel class**, so muted text sits **directly over the 3D scene** | `WalkingHudView.cs:74` |

Contrast on panels (approximate, WCAG relative luminance): muted ≈ 9 : 1, faint ≈ 5.5 : 1. **Adequate on panels.** Over the forest scene: uncontrolled (sky, mist and bright clearings behind the text).

## 2. Findings, prioritised

### BLOCKER (fix before the beginner playtest)

| # | Issue | Evidence | Fix |
|---|---|---|---|
| V1 | **The thinning forecast is the most important teaching line, and it is small muted text with no background over the forest** | `WalkingHudView.cs:74`; P1 record: "small muted text over a busy scene is hard to read" | Put it in a `panel`, body size (15 px), one line per fact (Crop Trees / stand / light / pattern). It becomes the in-world mini version of P3 |
| V2 | **Stat labels and faint text fall below 10 px at 1280 × 720** | `.stat-label`/`.faint` 12 px × 0.8 | Minimum 13 px base for any text carrying information; faint only for decorative captions |
| V3 | **Objectives not reachable as a list from the HUD** (comprehension, not visual) | HUD shows only a count; O opens lessons | Progress panel (P5) |

### IMPORTANT

| # | Issue | Evidence | Fix |
|---|---|---|---|
| V4 | Raw IDs in objectives and review lines | `TerminologyAudit.md` §1.1 | Display names |
| V5 | Long first-use help: Walking HUD help is 5 paragraphs before the player has seen the forest; Work Plan help 4 paragraphs | `MenuHelpView.Explanation` | ≤ 3 short paragraphs per screen; move key lists to an F1 "Controls" card; staged help (P5) |
| V6 | Map cell labels 13 px on coloured cells (≈ 10 px at 1280) | `.map-cell` label | 15 px base; keep the dark label backing (exists) |
| V7 | Inspection card is dense: 10+ stats in a 380 px column, max height 72 %, the "Why" sentence below the fold on small screens | `TreeInspectionView` | Order: identity → Why → competitors (P2) → stats. Collapse rarely used stats (stem volume, reproduction) |
| V8 | Prompt line carries up to 4 key hints plus the clearance label in one 18 px block | `InteractionPromptText` | With the preview behind U (decision #9), the ground prompt shrinks to two hints |
| V9 | Keyboard: O = lessons, but the buttons say "Objectives [O]"; Tab both opens and closes the Work Plan; F5/F9 untaught on main | `ScenarioOneUiRoot.HandleKeys` | Rename to "Lessons [O]" until merged; teach save keys (P1 does) |
| V10 | Work Plan "Approve pending work" approves everything, with no summary | `WorkPlanView` footer | Show "Approve n jobs (€x)", with a list on hover/click |
| V11 | Money colour: red/green for negative/positive, but the sign is also always shown | `.money-negative` | OK (not colour-only). Keep the sign |

### POLISH

| # | Issue | Fix |
|---|---|---|
| V12 | Trend bars: a single colour, values as text (good). Labels "Y12" faint | 13 px |
| V13 | Status words in caps (PENDING, APPROVED) | Sentence case |
| V14 | Help footer "Introductions appear only once on this device" is device language | "You can reopen this with F1." |
| V15 | Tooltips/hover help not used anywhere | Optional term tooltips for the glossary terms (`TerminologyAudit.md` §2) |

## 3. Colour-only meaning check

| Element | Colour-only? | Note |
|---|---|---|
| Fell / Crop marks (world) | Partly: red/blue paint. Shape: square chip "■ FELL", diamond "◆ CROP" in UI | Asset packet 07 (marking contrast validation) |
| Map layers | No: each cell shows a value or count | Good |
| Clearance preview | No: raised diamond pins plus a label | Good |
| Money | No: sign shown | Good |
| Trend bars | No: values shown | Good |
| P2 neighbour tags (proposed) | Must use numbers | Design requires numerals |
| Storm stability bands (proposed) | Must use words | Words Stable/Watch/Exposed |

## 4. Resolution constraints

- Supported test matrix: **1280 × 720, 1600 × 900, 1920 × 1080** (as used by the clearance and P1 rendered gates). Add **1366 × 768** (common laptop) to the beginner-ready gate.
- Ultra-wide and 4K: not tested; scaling is height/width-matched at 0.5, so 4K scales ×2.4. Acceptable, but untested.
- Any new section (P2 list, P3 block, Annual Review v2) must pass a rendered no-overlap check at 1280 × 720 (pattern: `TEACHING_COPY_RENDERED_PASS`).

## 5. Not covered by this static audit

Colour-blind simulation of world marks; screen-reader support (none; Unity UI Toolkit runtime has limited support); controller input (keyboard/mouse only); motion sensitivity (camera bob, if any). Record these as known gaps in the Definition of Done.
