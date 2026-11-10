# Scenario One UI Presentation V1 — Integrated UX Specification

**Status: APPROVED FOR BOUNDED IMPLEMENTATION (source-first, guarded Unity).** Approved by the user's 10 November 2026 request to specify and implement all discussed screens using **Sol as author** and **Fable as independent reviewer**. This is a presentation/UI specification, not approval of a gameplay or ecology change, a merge, a Windows build, or player acceptance.

**Authority:** latest direct user direction > `Docs/Project/decision-log.md` > current milestone > game brief > subordinate approved task specs. In particular, do not undo the existing November playtest fixes and review-first action priority. The previous `specs/scenario-one-inspection-hud-declutter-v1/` is **subsumed by this complete UI spec for new work**; its scientific and compatibility guards are carried forward. The Fable 5.2 design-only proposal is input, **not verified UI**.

## Goal and scope

Create one consistent, less-cluttered Scenario One UI across: (A) the walking/inspection HUD and contextual bottom buttons, (B) the compact persistent Year/Cash display, (C) Objectives/Stand Map/Work Plan/Annual Review full-screen navigation shell, (D) Work Plan presentation and approval hierarchy, and (E) the end-of-year Annual Review. Preserve all underlying player decisions, full on-demand information, gameplay permissions and warning gates.

**Out of scope:** ecology/growth/biodiversity metrics, cash or contractor pricing formula, rack geometry/access topology, MFM algorithm or commitment semantics, new saves/schema, objective/progression rules, first-review acknowledgement semantics, Windows build, novel mouse controller, forestry workbench retirement, world visuals. Do **not** add permanent browsing pressure or a biodiversity/habitat number to the HUD.

## Binding annotated images

These four committed SVG files are **layout/visual acceptance references**; accompanying rules here and existing authoritative behavior prevail if illustrative words/values differ:

1. [A · HUD / inspection](visuals/01-hud-inspection.svg)
2. [B · Work Plan](visuals/02-work-plan.svg)
3. [C · Annual Review](visuals/03-annual-review.svg)
4. [D · Navigation and safety](visuals/04-navigation-and-guards.svg)

All values (e.g., Year 2, €12,000, shares, quote amounts), trees and operations in the diagrams are **examples only, never test fixtures or game state**. Wireframes are **not actual Unity renders**. The previously generated concept/moodboard pictures are **inspiration only and have non-authoritative key, action and numerical examples**; never copy a mock's false [F] Fell key, wind safety, mandatory advancing, invented cash outcome or false biodiversity total.

## FR-01 — Walking HUD: one clear hierarchy

- **Top-left:** one current-action cue, objective title, necessary real progress/checklist and `[O]` for details. Existing Objective 2 three-zone live checklist and latest-unread Annual Review priority **must remain visible when applicable**; do not shrink these into a misleading one-liner. Rack-first retains true access-stage wording.
- **Top-right:** quiet, compact, persistent **current Year and current Cash** sourced from existing `CurrentEcologicalYear` and `CashCents`. No browsing pressure, biodiversity/forest health score, empty placeholder or speculative ecological number. No multiple cash/status panels; avoid duplicated key hints.
- **Bottom:** user-requested, visually distinct keycap + verb controls, selected for the **actual state**. Examples: ordinary walking `[O] Objectives`, `[M] Stand Map`, `[Tab] Work Plan`; inspection `[1–5] Neighbours` (only available ranks), `[V] More/Less detail`, `[E] Close`. In real stage where applicable `[X] Mark to fell`, `[C] Crop Tree`; **never display these when Rack-first blocks them**. Other states only show live valid commands. Keycaps are prompts, **not click targets when the cursor is locked**. If an unlocked view has clickable controls, route through existing handlers and guards.
- Forest view and reticle stay legible. Waypoint, minimap, rack-preview and ground-regeneration detail get context-aware space; don't simply remove required safety/wayfinding information. The minimap may suppress during dense tree inspection as existing source does, returning afterward.
- The active next-action control may pulse only where already explicitly safe; **Approve Work Plan, destructive and commitment buttons must never pulse**.

## FR-02 — Selected tree and neighbour competition: one responsive decision card

- Replace simultaneous tree card/competitor card/forecast clutter with **one responsive default decision card**, preferably right side while inspecting, without hiding selected tree or objective cue. Compact title: species, authoritative mark state, necessary size and condition; distinctive **Crop Tree / Fell mark / ordinary / MFM candidate or proposal** labels. MFM proposals are **not Fell marks**.
- For a Crop Tree, show source-backed local competition explanation and **up to three** ranked neighbour rows in compact view (rank, stable tree identifier, distance, contribution share). Show correctly calculated "other neighbours" remainder if total is available. `[1–5]` may still navigate the actual available five; no misleading rank shown if nonexistent. Full rankings, DBHs, raw indices, site/regeneration/growth, wind, opening, rounding/model limits and provenance stay accessible under `[V]`.
- **Scientific denominator rule:** neighbour `13%` = share of **local competition index**, NOT `13%` of diameter growth. Any value like `54%` growth withheld is specifically **potential diameter growth withheld**, only if computed by the present model. No automatic Fell recommendation. "Stable" must not imply a wind-safe forest when storm simulation is disabled.
- The card's "Your draft plan" summary uses actual pending marks and source-backed impact; distinguishes changes since marks from baseline; highlights material wind/opening/other safety warnings and source-unavailable states. It must not imply spent cash, felled trees, committed work or prescribed action. If numerical deltas are not available as structured values, do not parse English summary strings to fabricate them; reuse current truthful prose or a strictly presentation-only typed adapter approved in the same touched-file boundary.
- `[V]` must fit cursor-locked keyboard-only use: bounded pages/sections with visible navigation and no inaccessible scrolling. All existing material information classes remain reachable.
- **In-world top ranked share labels:** bounded **conditional** enhancement only if genuine Unity captures show legibility, correct depth/occlusion and no clutter at 720p; each displays rank and competition-index share, only for active selected tree. If unreadable, omit visual labels and retain ranks/shares in card; record go/no-go rather than bypassing the gate.

## FR-03 — Shared full-screen navigation shell

For Objectives, Stand Map, Work Plan and Annual Review, keep the same navigation vocabulary/order wherever a destination is currently available: **Objectives | Stand Map | Work Plan | Annual Review**, Help `[F1]`, visible current Year/Cash in the menu header if it fits without duplicated amounts, and **Back to forest [Esc]** as a consistently named, usable exit. A tab or destination unavailable in the actual current state is disabled or omitted with the existing reason; don't invent a new global route or bypass a gate.

Give the existing safe "NEXT" guidance a consistent visual location, using `ScenarioOneUiRoot`'s existing action priority. The latest unread Annual Review must still supersede Crop instruction across HUD, Objectives, Map and Work Plan. The Objective 1 five-rack teaching diagram, existing headland/feeder map, rack-entry marking, waypoint/checklist and old-world guards are **not** layout targets to redraw from the mock-ups.

Where possible implement by reusing existing view/button constructors and a small presentation helper, not replacing menu states or input architecture.

## FR-04 — Work Plan hierarchy and commitment

- Layout: clear title and shared navigation, one concise **proposed work**/status introduction, collapsible job group list with all existing rack, harvest, planting, shelter, pruning, nursery, reference, MFM and warnings intact, and an **accurate quote summary** in one place. Keep real source totals: work costs, expected timber revenue, net outlook, cash after approved costs, owner-time and **contractor minimum/eligibility**. Do not present forecast revenue as banked cash, or blur "pending / approved / completed".
- The source `WorkPlanView` already supplies **preapproval quote and small-job minimum**; presentation should reorganize, **not recompute or replace** this functionality. Keep edit/remove/cancel approval, supplier choices and review/confirmation warnings accessible.
- Static **Approve Work Plan** (commitment gate) and **Advance one year** (separate step) in a clear footer; never make approval pulse. Do not force disabling Advance for every pending job: **preserve the current source's unapproved-work confirmation choices, eligibility guards, and safe return path**. Any unread Annual Review takes safe progression priority.
- Make it clear that execution only occurs on approved year advance through existing authoritative handlers; no new action semantics.

## FR-05 — Annual Review: scannable overview, complete records

- Three descriptive, non-question **headline sections** in this exact conceptual order: **Work done**, **Money**, **Forest changes**. Use straightforward headings, **not** "What did we do?", "What did it cost?" or "What changed?". The current code already has Work/Forest/Cash columns; improve the order and hierarchy rather than duplicating their data.
- At top, concise annual facts from **the selected year's recorded AnnualReport/YearHistoryView and EcologicalSnapshot** only. Work done: succeeded/rejected work totals only when supported. Money: year income, actual contractor/material costs and **net result only when defensibly derived from the same annual authority**; preserve once-per-year grouped harvest settlement and nursery purchase timing. If an amount cannot be reconstructed faithfully, show "Not recorded" or omit the aggregate, not zero/estimates. Forest: recorded regeneration, deadwood, canopy/stock changes where actually available; do not imply causal certainty or make a biodiversity score.
- Detailed records, interventions, previous treatment, objective history, trends, storm review, deadwood/salvage, and source uncertainty remain reachable below. Existing **Show data sources** default OFF and provenance semantics retained. Historical selected years are read-only, never mixed with current forest facts; missing earlier snapshot is explicitly missing.
- For **latest unread** review, one prominent `I've read the annual results` action, with exact existing acknowledgement state mutation. **Back to forest [Esc] remains available as a secondary action before acknowledgement.** For previously acknowledged/latest and earlier history use state-appropriate controls, **no acknowledgement on historical year**. Do not introduce "Continue to Year N" as an implicit alternative to the existing Work Plan approval/advance.
- Review-first priority and objective progression must not be changed or inferred from imagery.

## FR-06 — Layout, contrast and visual consistency

Visual design: calm woodland greens/cream, strong legibility over forest, a single dominant decision card, very limited highlight/animation, consistent spacing, state colors with text labels (not color alone). Crop is distinct from player Fell mark; MFM proposal distinct from either. Avoid moving the game's meaningful instructional content off-screen. Render and inspect **1280×720 and 1920×1080**, colour and grayscale, with longer labels, busy forest, real selection, expanded work items and year-history content. Responsive stacking/wrapping is allowed if equivalent information and commands survive.

Use the vector references as **composition/typography targets** rather than pixel-perfect asset replacements; real Unity screenshot evidence decides whether the composition works. No edited/static mock counts as a Unity pass.

## Invariants / stop conditions

1. Never modify computation, economy, forestry growth/browsing/wind, marks, access geometry, historical records, acceptance/commitment, saves, menu gating or progression to "make the layout easier." Any required non-presentational behavior change is **STOP and Manager decision**.
2. No new numeric biodiversity or permanent global browsing pressure. Existing local browsing diagnosis in regeneration detail is not removed.
3. Legacy40 and frozen Reference Future v1 must retain original behavior/display guards.
4. Preserve no-Windows-build-before-user-playtest + explicit permission policy.
5. Preserve source-handoff and Unity-lease/saves/dirty-worktree discipline; Sol and Fable may **not** write the same UI files concurrently.

## Latest user visual selection

The latest image supplied by the user at **19:40 UTC on 10 November 2026** is explicitly preferred to the preceding posters for its visual design. When its exact pixels are available in the local worker lane, treat it as the **first look-and-feel target**, above older moodboards/posters, but **below this spec's behavior, data truth and guards**. The user-uploaded inline image is not yet a verified Git asset or named local file; therefore do not falsely claim an exact-image visual review. The task remains implementable against the four committed SVGs and the three archived user posters if that image cannot be accessed. The final user-visible HUD/Work Plan/Annual Review still require actual Unity screenshot verification.

## User-supplied three-poster visual style supplement

The user supplied three high-resolution rebuilt PNG posters after this spec was approved. They are **adopted as supplementary visual references for look-and-feel, not game/UI source authority**. See the [poster access manifest and mandatory errata](visuals/POSTER-SUPPLEMENT-AND-ERRATA.md). In Sol and Fable's local work, check the user-reported `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters/` files and their SHA-256 hashes; these PNG binaries are not committed to Git. All four committed SVGs and the requirements above remain the binding behavior/layout references. If unavailable locally, the SVG/spec implementation can proceed, but the actual three-poster match must be listed **NOT INSPECTED**.

Do **not** copy mock-only `[F]` Fell or novel shortcut keys; invented money/canopy/competition percentages; unsupported ring-barking/area rates; or the `Continue to Year 6` Annual Review action. The latest unread review still acknowledges and allows Back, and Work Plan remains the year-advance gate. In-world share labels remain conditional, and the upper-right HUD remains Year + Cash only. Preserve the posters' useful visual clarity and alignment without importing their example errors.

## Approval and acceptance boundary

This spec authorises **Sol source implementation** after live single-writer preflight and issue claim RUNNING, followed by one focused, exclusive Unity Tier-2 when safely permitted; **Fable performs independent source/visual-evidence review after Sol handoff, not parallel edits**. No production source, Unity or Fable review is claimed complete by writing this spec. Manager integrates only after evidence and a separate APPROVED verdict, then requests player visual acceptance. See [plan](plan.md), [tasks](tasks.md), [requirements](checklists/requirements.md) and the binding Sol/Fable packets.
