# MULTI-STANDMAP-2D-SELECTED-UI-V1 — Product and UX specification

- **Status: APPROVED FOR BOUNDED UI IMPLEMENTATION — Manager #863 under explicit owner delegation, 2026-11-20 14:38 UTC.** No source work has yet begun; visual-reference fidelity remains an unverified acceptance gate.
- **Owner:** CCF owner; **manager:** Multi #863. **Task:** [#868](https://github.com/jermur788/CCF/issues/868).
- **Prepared:** 2026-11-20, from owner feedback at 13:06, 13:12 and 13:56 UTC, source-inspected #849, two independently authored mock prototypes and the three generated UI comparison concepts.
- **Source candidate protected:** `4c42e2012a4c1bee3a422e41a58535343d63ac9f` on `feat/multi-standmap-playable2d-20261120-v1` (NOT on main). This draft does not authorise changing any file.
- **Approval:** The owner explicitly delegated #868 specification approval to Multi Manager #863 on 2026-11-20 at 14:38 UTC. Manager approves this four-file specification and its proposed five-option reason vocabulary for bounded front-end authoring. The actual image-reference file/hash and independent visual-fidelity check remain OPEN; they do not prohibit honest first-pass styling against the documented concept direction. A fresh sole writer/physical lane claim and precise file grants are required before source work; this approval grants no Core/API/Unity/P0/fixtures/test/merge authority.

## 1. Product problem and smallest genuine result

The actual #849 one-area 2D Sitka simulation operates but the owner found its long single-column desktop page cumbersome; finding the per-tree reason requires extensive scrolling; seven independent full-text pan/zoom controls waste space; and the appearance is less developed than desired.

**A satisfactory real end state:** at 1280×720 desktop, the player sees the large forest map, selected tree inspection and core draft→approve→execute controls within one useful screen; selects an actual engine-owned tree, chooses a short recorded reason, approves without felling, executes once with real engine results, advances a genuine ecological year, and reads the state change without page-length navigation. The current 30×30m, 100 synthetic Sitka reference fixture and underlying simulation remain authoritative.

## 2. Decisions, design authority and image provenance

**USER CONFIRMED / binding direction:**
- **V1:** visual direction resembles **comparison image #1** presented in the 2026-11-20 manager chat (forest-focused, substantial realistic-looking canopy map, composed game-style interface, high perceived polish, single-screen hierarchy). Adopt its *composition and visual character*, **not** fictional stand data or invented UI features.
- **V2:** incorporate **comparison image #3's richer legend**, distinguishing **tree species/type** and **management/felling state/operation**, without conflating these meanings.
- **V3:** compact map-edge directional D-pad with +/− zoom and Fit; discard the seven sprawling text buttons.
- **V4:** **predefined felling-reason dropdown with Other/custom**, retaining human-readable **per-tree recorded reason** in Work Plan and receipt/history; typing is only necessary for Other.
- **V5:** eliminate page-length desktop scrolling for normal core gameplay; secondary data remains reachable.

**Important evidence limitation:** comparison images #1–#3 were **AI-generated design concepts, NOT captured screenshots of Unity or either running prototype**, and are not yet stored as immutable GitHub reference assets. Their image-specific composition can guide this draft; **pixel-fidelity/reference-lock approval cannot be claimed** until an owner-selected **actual image file** is given a permanent versioned path and SHA-256, with provenance `generated-concept` explicitly recorded. No image is created or silently substituted by this text spec. The Unity repository's genuine map/UI code and screenshots are separately labelled **existing implementation references**, not automatically the selected concept; the previously approved Scenario One visual-system spec applies to its own 3D context and is not silently superseded.

**Illustration not product data:** concept #1's mixed-species 1 ha fictional stand, additional stands, UI controls, percentages and economics do NOT become #868 scope. Model still shows **one 30×30m Sitka-only experiment** and truthful numeric units/source status. Likewise, image #3's multi-species legend must not suggest Beech/Oak or unrelated species are simulated: category list may show **Sitka spruce** as the only populated species, with scope note.

**Both BlindA #869 and BlindB #870** are isolated **mock HTML/CSS/JS** experiments, source-delivered but not real-engine qualified. Their components can inform layout controls; their dummy TreeIDs, invented model, fake growth, synthetic counts, prototype branding and browser-only atomicity **must never be copied as factual engine state**.

## 3. User-visible design requirements

### R-01 Desktop composition

For a **normal 1280×720 viewport at 100% zoom**, present a concise identity/status rail with ecological year, living trees, basal area and plan state; a **dominant north-up map occupying most available content area**; a compact persistent selected-tree Inspector plus Work Plan on the right; a concise legend attached to map/its edge. The layout may use a narrow optional tree selector/search on the left **only if the map remains substantially dominant and core tasks fully visible**. At 1920×1080 expand map rather than bloating chrome. Where status or details exceed available space, use bounded **internal pane scroll/drawer/tab**, not a main document requiring player to scroll past a full 100-row table. At desktop 1280×720, select→reason→add→approve→execute→Year +1 controls must each be visible or reachable with a **single obvious in-place panel action** (e.g. tab), not via full-page scroll. Prioritise map/Work Plan over decoration.

### R-02 Map design and spatial truth

North up, exact extent/axes, actual positions, radii, status and hit tests **from the #849 server view/scene**. Preserve engine identity (exact IDs and case-sensitive contracts), coordinate orientation, current source/extent, selection/overlap resolution, accurate tree count, grid/north arrow/metric scale, and real canopy gaps/stumps after felling. Render forest-ground/canopy richer than bare undifferentiated discs **where safe and based solely on existing scene geometry and state**. Aesthetic raster/texture overlays must be clearly **non-authoritative**; no invented tree locations, species, spatial controls, growth or property layout. Maintain hit targets that map to the same exact authoritative tree; decorative foliage and operation glyphs must never intercept or distort hit tests. If actual rich-canopy parity is infeasible within the existing per-tree scene, implement a restrained composed cartographic styling and flag a visual deviation rather than painting fictional species/stand data.

### R-03 Map controls

Show a compact visually grouped **four-way pan D-pad**, **+ / −** zoom and **Fit/whole area**, preferably in a corner/gutter. Icons must have name/tooltip and semantic accessible label; full keyboard Tab/Enter/Space path; 44×44 CSS px minimum pointer targets; legible focus. No overlay obscuring selectable trees or legend at Fit. Preserve exact existing scale/zoom/pan semantics and no page horizontal expansion. Optional mouse drag and wheel zoom only if separately verified not to turn a click into a pan or change hit-test semantics; they are **nonessential**, not source scope by default.

### R-04 Compact Inspector and reason picker

After exact tree selection show engine-supplied ID, species (Sitka), DBH, height, state and actionable order status. Use a required `select` with placeholder “Choose a reason…” and at least four plausible choices **proposed, not approved vocabulary**:
1. Release competition around a retained tree
2. Poor form or stem quality
3. Damaged, unhealthy or declining tree
4. Create a light gap for regeneration
5. Other — specify reason

**The five reason option labels in this section are MANAGER-APPROVED implementation vocabulary under the owner's explicit delegation; subject to later owner playtest refinement, without changing reason semantics.** Do not infer disease, windthrow, regeneration effectiveness or a crop-tree relationship from simply choosing a reason. These are **operator intentions**, not validated ecological diagnoses. Other requires non-empty trimmed text; cap to existing engine reason length/validation. For a predefined selection, send its exact readable string to the existing `AddPending(fence, treeId, reason)` route — no new backend enum/save schema and no hidden default reason. Show selected reason beside each pending/approved order, in confirmation and history. Owner approved the dropdown+Other **mechanism**; Manager #863 approves these exact labels for #868 implementation under the owner's delegated approval, with actual per-tree text recorded. Future label revision remains a product change.

### R-05 Work Plan and ecological year

Preserve the actual engine state machine and server responses: add pending **fells 0**; pending removal; approval **fells 0 and locks**; cancel/withdraw approved plan only via existing allowed command; execute **only** after a separate accurate confirmation identifying exact PlanId/execution key/orders and chosen tree IDs; genuine `fell (manual-work-plan)` results from engine; visible stump, living count/basal area and truthful execution receipt; Year+1 actual annual continuation and required consent/refusal for outstanding work. **Never** implement mock arithmetic, local approval, local woodland state, stale/fabricated revision, auto-execute, duplicate replay or fake effects. Retain session/source/revision fences, origin/host/capability protections, fail-closed unconfirmed outcomes, re-read lock and anti-replay semantics. If the interface is narrowed, the player must still see refusals, confirmation changes, cancel and recovery controls.

### R-06 Legend with two independent dimensions

A readable **Tree/species** section: currently **Sitka spruce** as the only actual species. A separate **Management state** section: Living/standing, Pending Fell (proposed only), Approved Fell (still standing), Fell/Executed (actual stump/canopy gap), Natural Dead if present. If any “operation type” beyond Fell is illustrated in concept #3, mark it **NOT SUPPORTED HERE** or omit; no cutting categories, extraction/rack/access or cross-area intervention actions exist in #849. Use glyph/pattern/shape plus colours, not colours alone. Legend counts sourced from real current view; no invented operation outcomes. Selected-tree halo remains clearly distinct from planning status.

### R-07 Data access, source provenance, accessibility

Keep full 100-tree exact-ID table/search available via labelled drawer/tab/secondary contained panel rather than permanently below the fold. Keep full history, status details and original scientific/source provenance (fixture SHA, content hashes, model versions, session ID) available from accessible “Details / technical provenance”, NOT deleted or fabricated; allow copy. Retain an always-visible concise “synthetic fixture / session-only / no save / G1 and money unavailable” disclosure. Use text semantics, live-region notices, keyboard exact-ID alternative to SVG pointer, focus return from dialogs, 44px controls, at least WCAG AA contrast for ordinary body text, non-colour-only marks. At **actual browser 200% zoom** and widths 360 CSS px and 640 CSS px allow a **stacked reflow with vertical document scroll**; do NOT force desktop single-screen at accessibility zoom. No inaccessible clipped dialogs/buttons or horizontal page scroll. The old one-column CSS repair at 200% zoom is **protected behavioural intent**; its literal always-one-column implementation can only be superseded by independently tested equivalent accessibility, not by weakening tests.

## 4. Out of scope

NO G1 empirical comparable areas or treatment propagation, no multi-rack/area gameplay, no new species engine, economic calculations, saves/persistence, extra map layers without underlying state, Unity scene/tooling changes, G4 main merge, Windows Player build, fixture/P0/golden mutation, new library/theme framework, third-party fetched assets or fonts, public/network host change, changes to physical Ops/policies, or lifecycle/test weakening. All genuine forestry and plan contracts remain #849's.

## 5. Reference and implementation gates

**Manager-delegated approval granted for bounded UI authoring:** The owner expressly authorised Manager #863 to approve and dispatch this work at 14:38 UTC; four-file approval and reason labels are established on this branch. **Before a worker writes:** exact candidate base/branch/clean exclusive local lane and compile-linked file allowlist, current coordination and physical locks. **Before final visual-fidelity ACCEPTANCE:** permanently archived actual generated image #1, SHA-256/provenance and comparison; until then label style match NOT VERIFIED. Independent qualification is a later gate, not an automatic privilege. If reference asset is unavailable, this draft can guide a style implementation but cannot claim **visual fidelity to a frozen reference**. Use one bounded writer and check live coordination/physical locks before launch. Approval of text alone does not confer G2/G4/main/source rights.

## 6. Acceptance evidence

Owner personally sees and controls the actual 2D live server page at normal 1280×720 with forest dominant, right-side reason/Work Plan and compact controls; screenshot compare to **frozen reference**, 1920×1080, actual 200% zoom, 360 px reflow; narrow targeted Tier 1/Tier 2 checks including keyboard selection, map overlap/identity, no operation/status misrepresentation, dropdown Other, approval zero Fell, confirmed execute exactly once, genuine Year+1, stale/host/origin guard refusals, no save/network expansion; independent reviewer APPROVED; owner G2 acceptance separate. No claim based solely on compilation, synthetic DOM stubs, generated images or worker self-report.
