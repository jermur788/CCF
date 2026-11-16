# Multi #655 — isolated interactive mock UI prototype result

**Date:** 16 November 2026. **Status:** IMPLEMENTED AND GITHUB-PUBLISHED; NOT INDEPENDENTLY REVIEWED, NOT INTEGRATED OR ACCEPTED. **Requested Manager verdict:** APPROVED AS ISOLATED MOCK / NEEDS CHANGES / REJECTED. **Task:** [#655](https://github.com/jermur788/CCF/issues/655). **Manager:** [#641](https://github.com/jermur788/CCF/issues/641). **Difficulty:** MEDIUM — HTML/CSS/JS client interactions with strong source/provenance boundaries.

## 1. Worker identity, gate and exact custody

- **Actual worker:** OpenAI ChatGPT GPT-6, requested High reasoning (the active runtime effort setting cannot be independently inspected here).
- **Lane:** GitHub connector / browser-only; **no physical laptop worktree, no physical CCF-Ops registration, no Unity lease**. No local repository checkout or Unity process was used.
- **Exact branch:** feature/multi-p1-655-standalone-mock-ui-20261116-v1.
- **Pre-edit starting HEAD:** 374bcb7aba3bb09e270c4eae14451695c2bfeba7 (the D-083 canonical source baseline); live main board later carries the separate prospective ownership entry.
- **Mandatory ownership gate:** passed BEFORE editing. The main coordination board explicitly reserves only Docs/Prototypes/MultiRackSitkaP1MockUI/** and this exact result Markdown for the independent browser worker; [#640 confirmation](https://github.com/jermur788/CCF/issues/655#issuecomment-6931548267) and [#641 release](https://github.com/jermur788/CCF/issues/655#issuecomment-6931646976). Actual [RUNNING receipt](https://github.com/jermur788/CCF/issues/655#issuecomment-6931674116) was posted before creating any prototype blob. The prior [BLOCKED report](https://github.com/jermur788/CCF/issues/655#issuecomment-6931526250) was correctly superseded only after board readback.
- **Binding design:** [complete #655 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-655-isolated-mock-ui-prototype-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-655-ISOLATED-INTERACTIVE-MOCK-UI-PROTOTYPE-PACKET-20261116.md), [#650 Manager-approved-as-DRAFT-input UX flow](https://github.com/jermur788/CCF/blob/docs/multi-p1-650-browser-firstyear-ui-result-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-650-FIRST-YEAR-2D-PLAYER-FLOW-AND-MOCK-UI-DRAFT-20261116.md). P1 owner-approved technical spec remains subordinate only to accepted higher authority; this mock has no production implementation authority.

## 2. Delivered exact paths and purpose

| Path | Function |
|---|---|
| [index.html](../../Prototypes/MultiRackSitkaP1MockUI/index.html) | Semantic page, permanent mock disclosure, three source-domain cards, stand metrics, SVG frame, inspector, inventory controls, refused actions, source limitations, static comparison examples |
| [style.css](../../Prototypes/MultiRackSitkaP1MockUI/style.css) | Responsive forest-operations styling for laptop/mobile widths, high-contrast textual states, visible keyboard focus, reduced-motion support |
| [app.js](../../Prototypes/MultiRackSitkaP1MockUI/app.js) | Fixed 14-row synthetic in-file data, mock area/BA summaries, SVG keyboard/click/hover markers, common ID selection, table search/filter/sort, original/unity refusal panels, tab switch; **no engine, network or persistence** |
| [README.md](../../Prototypes/MultiRackSitkaP1MockUI/README.md) | Local browser open instructions, provenance/units, explicit limitations and future integration boundary |
| This handoff | Verification evidence, exact scope, nonclaims and Manager review request |

**Prototype source commit:** [isolated four-file source](https://github.com/jermur788/CCF/commit/aa66fcf44e11f3867d4708dbd701d258a2daba9c). Exact compare from the registered branch base to this source commit shows **four additions and no modifications or deletions**. The final result commit adds only this separately reserved Markdown path.

## 3. Implemented player-visible behaviour

1. Persistent hard disclosure: **SYNTHETIC MOCK DATA — UI PROTOTYPE, NOT A RUNNING FOREST SIMULATION OR IRISH FIELD FORECAST**. Static context also says F0 annual engine absent.
2. Three non-conflated source choices:
   - DerivedP1ThirtyMetreExperimentV1 → schematic, explicitly separate **30 × 30 m = 900 m² = 0.09 ha**, 14 mock records, with **real P1 manifest not imported / PendingIndependentReview**.
   - OriginalPolygonExploratoryV1 → **source-not-loaded / inspect-only** placeholder with no invented original map, IDs, rescaled area or work.
   - DerivedUnitySquareParityV1 → **future unavailable** 40/80 source/legal gates; no implied numerical Unity parity.
3. Fixed mock Sitka positions render as SVG circles with mouse hover, click, visible selected state, text ID tooltip, tabindex/role and Enter/Space keyboard activation. A native Select button in each table row drives the **same stable mock ID**. Selection updates the map/inspector/table and does not change status, DBH or inventories.
4. Search ID, filter all/living/dead/actionable/living-below-threshold, sort ID ascending/descending and DBH ascending/descending. Selected ID remains in inspector when filtered from the table; a visible notice explains this.
5. Inspector shows mock species, DBH mm/cm, height, crown diameter, age, status, XY coordinates, above/below mock 170 mm work criterion and in-file provenance. Mock dead stems are **cause/year unknown**; there is no fabricated previous ecological year.
6. Two clearly source-labelled metric cards use a geometrical aggregate of the **authored mock data only**. Living N=12 with BA=0.457 m²; mock actionable N=9 with BA=0.409 m²; three small living mock stems excluded only from actionable. Living N/ha=133.3 and BA/ha=5.075 m²/ha. These are *schematic numbers*, not field observations, authoritative P1 values or growth curves. DBH criterion is a P0-inspired **display example**, not an acceptance of a P1 runtime threshold.
7. Plan/Approve/Execute/Advance ecological year are disabled HTML buttons, with refusal reasons. No click handler, annual transaction, work commitment, growth, mortality, save or external request exists for them.
8. Dedicated comparison tab has **EXAMPLE SCREEN — NO ECOLOGICAL YEAR EXECUTED**, two static illustrative arrangements with no time/result meaning, and actual result fields explicitly unavailable. It does not generate year snapshots, work removals, source hashes or numerical forecasts.
9. Source & limits remains readable from a native details disclosure. No framework, package, compilation, network, third-party assets or deployment.

## 4. Tests/evidence actually run

**GitHub byte readback, source/static execution only. BROWSER INTERACTION NOT RUN.** These checks were run against the four committed branch blobs via live GitHub file readback:

| Check | Evidence/result |
|---|---|
| Readback/branch isolation | **PASS** four GitHub blobs readable at dedicated branch; source compare base→source lists only README.md, app.js, index.html, style.css in reserved prototype directory |
| JavaScript syntax | **PASS** V8 Function constructor parse of full app.js; it was **not executed against a browser DOM** |
| HTML ↔ JavaScript contract | **PASS** every statically named el()/setText() DOM ID exists in index.html, and HTML IDs are unique |
| Disclosure and domain identifiers | **PASS** exact synthetic banner, explicit example-screen denial, all three domain identifiers in static HTML |
| Work/annual refusal | **PASS static source inspection:** four genuinely disabled HTML command buttons and no source command dispatch path |
| Network/storage absence | **PASS static source scan:** no fetch, XMLHttpRequest, WebSocket, localStorage, sessionStorage, indexedDB, sendBeacon, worker, timer or persistence calls in app.js; HTML resources are relative |
| Focus, search/filter/sort wiring | **PASS static source inspection:** SVG Enter/Space handlers with focusable markers; input/change handlers for query, filter and sort; a single selected-ID variable |
| Mock fixture invariants | **PASS independent numeric parsing:** 14 unique IDs, all points strictly within 30 m frame; 12 living, 2 dead, 3 living below 170 mm, 9 living actionable |
| Mock metrics arithmetic | **PASS independently recomputed:** living BA 0.457, actionable BA 0.409 m²; living N/ha 133.3 and BA/ha 5.075 m²/ha (rounded display values) |
| Protected files | **PASS GitHub branch diff:** zero P1/P0 production, Assets, Packages, ProjectSettings, specs, saves, coordination or main edits |

**NOT RUN:** browser DOM interaction, rendering/screenshots, keyboard-only witness, responsive viewport audit, accessibility automation, browser console, cross-browser testing, actual P1 API/security/Host execution, source manifest trust audit, annual engine, ecology, save/replay, P0 test suite, Unity, Windows build and any public deployment. No screenshot is claimed. Static source checks are not runtime/browser PASS. There is no GitHub Pages site or live-hosted running application.

## 5. Safety boundaries and remaining review

- No source or P1 trust status promotion. The generated P1 100-tree candidate is **not** copied, loaded, accepted or redesigned. No P0 original 153/1893 etc. source fixture is cloned, remapped or rescaled.
- No ecological year execution; P1 F0 annual gate is not bypassed. No work/felling, tree-status mutation, real model growth, forecast, mortality or persistence.
- No Host/network endpoint, production P1-05 work, approved spec change, assets, Unity, Scenario One, frozen reference or Windows build. Existing #639 sole P1 source custody preserved.
- Main board remains exclusively #640 owned; this worker did not edit main.
- Actual browser testing and accessibility verification are explicitly deferred to a separate, properly owned browser witness, if Manager wants a rendered qualification. **No integration or production UI acceptance follows from this result.**
- Dashboard/Drive/context mirror sync status: **NOT VERIFIED**; this task does not alter canonical project-context sources.

## 6. Handoff request

**Manager #641:** inspect the exact four files and static-source evidence, issue **APPROVED AS ISOLATED MOCK / NEEDS CHANGES / REJECTED**, and keep production P1-05 and first ecological year gated by actual accepted Core/Host interfaces, reviewed manifest/trust state, independent security/UX/browser verification and new writer permissions. Do not merge this mock into P0/P1 or treat it as an annual engine. Close #655 only after Manager receives this GitHub handoff and processes the claim.
