# Multi #736 / Lane A — Read-only 2D woodland viewer specification proposal

**18 November 2026 — DRAFT / proposed specification, NOT APPROVED.** One-file GitHub-only documentation result for [#736](https://github.com/jermur788/CCF/issues/736), returned to dedicated Multi Manager [#730](https://github.com/jermur788/CCF/issues/730). **No renderer, prototype, Host, shared API or tests have been implemented.**

## 0. Worker, authority and scope

- **Actual worker:** OpenAI GPT-6 in ChatGPT Game Dev, GitHub connector/browser-documentation harness. Authenticated GitHub connector identity: jermur788. Model effort and session-internal ID are not exposed; no claim is made about either. No local computer, native CCF worktree, CCF-Ops lane, Unity lease or build/test execution.
- **Initial GitHub main HEAD at registration:** b7421c9a5cbf63a1409fe7ae8eed304fdfd4a7e9. **Sole task branch:** docs/multi-736-a-spec-draft-20261118-v1, created from that head. [RUNNING receipt](https://github.com/jermur788/CCF/issues/736#issuecomment-6979722780).
- **Allowed edit:** this exact newly added Markdown document only. Live coordination remains with Manager; no active-tasks, specs, source, assets, fixtures or trust-catalog edits.
- **Binding task:** [complete #736 packet](https://github.com/jermur788/CCF/blob/docs/multi-730-parallel-workstreams-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-736-READONLY-2D-WOODLAND-VIEWER-SPEC-DRAFT-PACKET-20261118.md). Owner has approved *parallel document drafting*, not any new product behaviour or source start. The existing P1 owner-approved four-file technical spec is separate authority for its approved slice; this viewer document proposes additional behaviour for later review. The distinct Scenario One D-084 on main must not be used as Multi authority.

**Classification:** UI architecture and acceptance design proposal grounded in a synthetic engineering fixture; not ecological evidence, an Irish forestry forecast, an existing running UI, or a release qualification.

## 1. Source audit and factual boundary

The following GitHub evidence was read, not re-executed:

1. [#639 P1 foundation result, fixture declaration and P0 boundary](https://github.com/jermur788/CCF/blob/feature/multi-p1-639-f0-30m-foundation-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md); [frozen derived fixture source file on that published branch](https://github.com/jermur788/CCF/blob/feature/multi-p1-639-f0-30m-foundation-20261116-v1/Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json). Read its schema and individual entries via GitHub. This source file is **not** evidence that the separate #725 native source has been pushed to GitHub.
2. [#723 Year-1 independently transcribed arithmetic and per-ID display reference](https://github.com/jermur788/CCF/blob/8890a730d349cb7082be34c213503055b8059b1f/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md). Its rounded display table is **not** a bit-exact API payload.
3. [#725 author implementation report](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md); [#729 independent source/numerical review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md); [#732 focused offline native qualification](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md); [#735 immutable source custody](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md).
4. [Approved P0 2D lab specification](https://github.com/jermur788/CCF/blob/feature/multi-p1-639-f0-30m-foundation-20261116-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md) and [owner-approved P1 adult-rules technical specification](https://github.com/jermur788/CCF/blob/feature/multi-p1-639-f0-30m-foundation-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md). P0 and P1 remain separate stores/domains: do not extend P0 “no biological growth” into the P1 annual engine or silently replace P0 fixture/schema.
5. Live main coordination and issues #736/#730/#740; [Lane E's provisional cross-lane interface](https://github.com/jermur788/CCF/issues/740#issuecomment-6979762166), **DRAFT only**. E owns its final shared snapshot/integration contract; this Lane A document is explicitly subordinate on those topics.

### 1.1 Exact frozen synthetic starting facts

| Topic | Source-backed Year-0 value / interpretation |
| --- | --- |
| Experiment / fixture | DerivedP1ThirtyMetreExperimentV1 / p1d-30m-sitka-adult-v1; format ccf-sitka-p1-derived-fixture, schema v1, synthetic; fixture SHA-256 **bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4**, 32,567 bytes per custody evidence |
| Plot | 30 × 30 m, **900 m²**; local integer-mm **XY** origin at south-west corner, +X east, +Y north, each coordinate inside [0, 30000) mm; flat elevation 0; no terrain claim |
| Stand area for biology | Binary32 hectare value with bits 3db851ec. Do **not** replace the authoritative stand calculation with a screen-polygon or double-precision “0.09” estimate |
| Stems | **100**, P30-001 through P30-100 in ordinal ID/row-major order, all living Sitka MatureEquivalent age 30, 10 × 10 nominal 3 m grid with integer jitter; 12 have Year-0 DBH below the **170 mm action label**, but **all 100** enter Year-0/1 biology |
| Dimensions | Fixture xMm range 1069–29028, yMm 928–29092, DBH 131–349 mm, height 17130–23670 mm; binary32 Year-0 crown radii approximately **1.942–3.606 m** |
| Coordinates to centred XZ if needed | Fixture declares UnityX=(float)((xMm−15000)/1000.0), UnityZ=(float)((yMm−15000)/1000.0), UnityY=0; this is a **declared synthetic mapping**, not proof of existing Unity scene or geometric parity |
| Example tree | P30-001: x=1069 mm, y=1908 mm, DBH=329 mm, height=23070 mm, initial crown radius ≈3.453415155 m. P30-053: (7710,17002) mm, DBH=131 mm, initial crown ≈1.941685200 m. These numbers are fixture readings, not field measurements |
| Scope excluded by fixture | Browsing, canopy/light, juvenile lifecycle, mixed species, pruning, recruitment, regeneration, storms; also **no new topography, rack network, roads or Irish geography** |
| Health | Fixture has **status=live**, not a calibrated or independent “health score”. Competition/suppression diagnostics from Year 1 are not interchangeable with health. Viewer must say **“Health score: unavailable”** rather than fabricate one |

**Year-1 reviewed evidence:** local #725 advanced every stem to age 31; 100 survivors, zero hazard deaths and zero boundary deaths; directed Hegyi pair count 1,622. Reported initial relative density 0.609795828454288 and post-growth 0.6375836886879149; initial/post-growth stand basal area **55.87290918844456 / 58.33954510260964 m²/ha** on the manifest binary32 area basis. #729 independently reviewed source and numerics, and #732 subsequently recorded focused offline native exact-output and negative-case PASS. The **actual Year-1 source remains local and unintegrated**; no accepted general read snapshot, Host endpoint, saved Year 1, user-viewer or Unity parity exists. #729 **M-1 dead-stem filtering before Year 2** remains open. Default .NET **L4 FAIL38, unwaived**.

### 1.2 Honest viewer banner (required copy intent)

> **Synthetic Sitka test stand (flat) · 30 m × 30 m**  
> **ENGINEERING EXPERIMENT · Not an Irish field forecast · Not Unity parity**  
> Year 0 / Year 1 (only when verified snapshot is available) · All 100 stems included in biology  
> No regeneration, browsing, storm, work or geographical data in this fixture.

Any later chosen exact text may be reviewed by Manager, but the scientific and source-status disclosures must remain visible in UI and exported screenshot. “Healthy” green and “unhealthy” red are prohibited substitutes for unsupported physiology.

## 2. Proposed user journey and wireframes

**Goal:** a quick, trustworthy top-down view of the *same identifiable trees* before and after the one verified biological year, with spatial crowns at declared metre scale. This first viewer is read-only: inspect, pan, zoom, select, compare. It does **not** allow marking, plan proposals, approval, executing work, growth advancement, time creation, save/load, replay, accounts or network access.

### 2.1 Desktop / tablet wireframe (text specification only)

~~~text
┌───────────────────────────────────────────────────────────────────────┐
│ Synthetic Sitka test stand (flat)     [Year 0 | Year 1*]  [Fit] [−][+]│
│ ENGINEERING EXPERIMENT · NO FIELD FORECAST · NOT UNITY PARITY         │
├──────────────────────────────────────────────┬────────────────────────┤
│                                              │ TREE INSPECTOR          │
│  +Y north                                    │ Search ID: [P30-___]   │
│    ↑     30 m square boundary                │ Selected: P30-053      │
│    │   ┌──────────────────────────────┐      │ Status: Living         │
│        │   translucent crown circles │      │ Species: Sitka        │
│        │   and clear stem-centre dots │      │ Age: 30/31 y          │
│        │   spatial overlap retained  │      │ XY position: m        │
│        │                              │      │ DBH: cm; height: m     │
│        └──────────────────────────────┘      │ Crown radius: m       │
│  0 ├──────── 5 m scale bar                   │ Health score: n/a     │
│    +X east                                   │ Pre-growth CI: if any │
│                                              │ Death: if known        │
│  Legend: ● living  × dead  ◯ below-action    │ [Prev ID][Next ID]    │
├──────────────────────────────────────────────┴────────────────────────┤
│ Source: synthetic 30 m accepted fixture · status: verified / missing │
└───────────────────────────────────────────────────────────────────────┘
* Year 1 remains disabled until a trusted immutable Year-1 payload is supplied.
~~~

The inspector is an **overlay or side column** that never changes the source. At viewport widths below about 800 CSS px, inspector becomes a collapsible region below the map, and legend + Year control stay outside the canvas. No promise of exact CSS layout or design-system approval is made here.

### 2.2 Interaction state machine

| UI state | Entry/action | Visible response | Source effect |
| --- | --- | --- | --- |
| INITIALIZING | Load declared fixture ID and selected optional snapshot | Loading text; no invented trees | none |
| READY / NO TREE | Year 0 loaded, no selected ID | Fit entire 30 m boundary, legend, metadata; inspector prompt “Select a stem or enter ID” | none |
| HOVER | Pointer near a stem centre | Transient ID tooltip with status; nonkeyboard hover is optional, not sole information channel | none |
| SELECTED | Click/tap centre, keyboard ID search or Prev/Next | Persistent high-contrast stem halo and read-only inspector; selected stable tree ID, not array index | none |
| PAN/ZOOM | Drag background, pointer wheel, touch pinch or controls | View transform changes; selection remains stable; scale bar recomputes | none |
| YEAR_SWITCH | Select Year 0 or trusted Year 1 | Atomically replace read-only frame of all tree/stand values; keep camera and selected stable ID; state label updates | none |
| UNAVAILABLE_YEAR | Year 1 payload absent, wrong identity, wrong hash, incomplete or not authorised | Year 1 disabled or explicit blocked status with reason; **never synthesize Year 1 from rounded #723 table** | none |
| DATA_INVALID | Duplicate ID, unknown/missing source, NaN/negative crown, out-of-domain version, impossible year or mismatch | Refuse that snapshot entirely; keep prior valid frame and selection; alert with a non-sensitive error | none |
| DEAD_TREE (future valid input) | Selected ID has explicit alive=false and sourced cause/year | Keep selectable stem, use dead shape and exact supplied facts; no fabricated dead crown or health | none |
| CLEAR | Escape/clear selection, change fixture only via later approved workflow | Remove highlight, reset inspector without modifying data | none |

Year 0 and Year 1 here represent **read-only snapshots**, not a button that calls the annual engine. Do not present “Advance” or a work button in Lane A.

### 2.3 View controls and interaction detail

- **Fit:** calculate a uniform CSS-pixel-per-metre scale from the plot's declared 30 m extent and padded viewport, keeping the whole square and at least a labelled margin visible. Leave a crown-safe extra margin where available (large crowns can extend beyond stand boundaries). Do not crop or falsely clip a crown at the property edge: if intentionally clipped for viewport space, show an explicit canvas-edge clip rather than imply the biological crown stops at the plot line.
- **Orientation:** +X increases to screen-right; +Y (north in this *synthetic local coordinate convention*) increases screen-up. Display an **“local +Y north”** axis label, not a geographical north arrow. The fixture origin is the bottom-left south-west plot corner.
- **Pan:** drag only from empty/background area; clicking a trunk must not accidentally pan. On pointerdown select pointer intention after a movement threshold (~4 CSS px proposed), preserving click versus drag. Keyboard arrow-key panning may be supported when map has focus. A reset Fit returns to a known camera without altering year or selection.
- **Zoom:** wheel/pinch + explicit accessible +/− buttons; zoom about pointer/focus, not only about plot origin. Proposed initial bounds 0.5×–12× Fit scale; these are **UX calibration**, not an ecological constraint. Clamp numeric scale to positive finite values; refuse invalid transform.
- **Selection:** target the **stem centre**, not the whole overlapping crown. Pointer candidates within ~7 CSS px mouse or ~12 CSS px touch of centre; choose nearest by squared screen-space distance, then **ordinal tree ID** for deterministic ties. When two stems cannot be reliably separated at a zoom level, reveal overlapping candidates by accessible ID picker / searchable list rather than silently select a large foreground crown. Dead stems and below-action threshold stems remain inspectable.
- **Keyboard:** tab to map and controls, Enter/Space on focused ID selection, Escape clear, searchable text ID and Prev/Next **sorted by ordinal stable ID**. Do not force pointer-only selection or steal focus on year toggle. Clearly announced selected ID and year.
- **Labels:** draw tree IDs on demand for hovered/selected and optionally high zoom, not 100 permanently colliding labels. Search input and accessible ordered ID list are the reliable complete inventory index. All 100 IDs must be discoverable. Never reassign IDs according to draw order, sort order, radius or pixels.

## 3. Rendering, units, ordering and accessibility contract (Lane A proposal)

### 3.1 Deterministic world-to-screen model

The **frozen fixture's canonical plan coordinates are integer millimetres**. For a detached snapshot provided in centred X/Z metres by Lane E, project source coordinates consistently:

- From fixture XY: xMetres = xMm / 1000, yMetres = yMm / 1000; **x right, y up**. With a synthetic plot from 0 to 30, the screen transform is px = left + scale*(xMetres − cameraMinX), py = top + scale*(cameraMaxY − yMetres), where scale is finite CSS pixels per metre.
- From centred XZ: xMetres = sourceX + 15, yMetres = sourceZ + 15 **only when** transform ID explicitly equals p1-square30-corner-to-centred-xz-v1. Do not apply this offset to original P0 polygon or future 40/80 source.
- Stem marker centre is (px, py); **crown circle radius in CSS px = sourceCrownRadiusM × scale** (not radius/DBH, not diameter, not a constant glyph width). A radius of 3 m at 20 CSS px/m occupies 60 px from centre. Use an explicit crown alpha outline/fill with centre dot on top. Crowns are synthetic source-rule initialisations/Year-1 modeled values, not laser-scanned outlines.
- Canvas raster size may be CSS-pixel dimensions times device-pixel ratio for crispness, **without changing hit-test, scale-bar or world-space geometry**. Use consistent float interpretation and a bounded, reversible camera matrix. Renderer conversion is presentation-only.
- Boundary is **30 × 30 m axis-aligned square**, not a GIS plot. No contour lines, paths, rocks, access racks, elevation, species mixture, shadow-light map or geography unless a later reviewed source actually supplies them.

For a centred X/Z proposal Lane E must specify exact axis-origin metadata and source representation (f32 bits vs canonical integers). Until approved, this transformation is a **Lane A adapter proposal**, not a shared serialized field contract.

### 3.2 Layer and deterministic draw/hit order

1. Plain, neutral background; labelled synthetic 30 m plot boundary, axes and scale bar.
2. Crown discs for every tree with an explicit, valid crown radius, in **ordinal tree-ID order**. Use low-opacity fill and thin outline; **overlap is expected**. Never sum discs to invent canopy cover.
3. Stem-centre markers, in the same ordinal order, styled from sourced alive/dead state and below-action annotation; show each stem even where crowns overlap.
4. Selected/hover/focus halo and text labels as separate presentation overlays (not source fields). Overlay selected stem last but keep crown data unmodified.

**Do not use DOM insertion order, renderer-specific hash-map traversal or screen X-order** as the ID ordering source. On viewport, year or DPI changes, the same snapshot + camera gives the same centres/radii to displayed pixel precision; the selected tree's identity is stable. Hit-testing indexes stem coordinates only and uses the same screen transform as drawing.

### 3.3 Legend / colour-independent distinction

| Meaning | Proposed visible coding | Truth boundary |
| --- | --- | --- |
| Living stem | Filled circular centre with solid outline | Explicit alive=true; **not “healthy”** |
| Dead stem (future) | Cross / outlined centre with “dead” word in inspector | Only if explicit alive=false; cause/year only when supplied |
| Below action threshold | Extra hollow outer ring or small “under 170 mm at Year 0” icon in legend | **Not** hidden/excluded from biological model or tree inspection. Future eligibility must be sourced for that year |
| Crown footprint | Transparent circular disc, thin border; scale legend explains **radius is metres** | Model crown radius only; not crown diameter or canopy/light measurement |
| Selection / keyboard focus | Double outline/halo + persistent selected-ID text | UI-only, not a marking decision |
| No health assessment | “Health score unavailable” text | Cannot derive from age, CI, crown size, colour or survival |

Propose WCAG 2.2 AA-oriented keyboard usability and distinguishable labels; actual conformance requires later inspection. Nontext glyphs need at least 3:1 contrast against adjacent backgrounds and UI text at least 4.5:1 in later review, with a high-contrast palette; do not rely on hue alone. Use screen-reader names such as “Tree P30-053, living, 13.1 cm DBH, Year 0”; provide a text inventory. Keep chart labels and inspector legible at 200% zoom, 1280×720 and a narrow 360 CSS px viewport; do not assume all 100 markers can each have a visible label at Fit.

### 3.4 Inspector fields and units

Top to bottom: immutable Tree ID; snapshot year/stage and provenance; explicit living/dead status; species; local coordinates as **metres** to suitable display precision with clear “synthetic local X/Y”; age in years; DBH **cm** (or mm as secondary raw value); height **m**; crown **radius m**, not width; action-threshold note where sourced; optional pre-growth competition index with **“modeled / pre-growth, unitless”**; optional exact death year/cause if present; health score unavailable. Footer displays fixture ID/domain/source class, integrity status and snapshot version. Never expose RNG roll or hazard as a measured health diagnosis; advanced debugging fields, if ever requested, must be explicitly labelled model diagnostics.

At stand level show sourced living count N, stems/ha, BA/ha (m²/ha), Dq (cm), RD (dimensionless), selected stage and year. These are **authoritative report fields**, not values reverse-computed from rendered circles or coordinates. If only raw Year-0 fixture is available and a vetted metrics payload is absent, show tree count plus **“Stand metrics unavailable in this view”** instead of approximated authoritative values. Avoid unqualified rounded values used for hash/integrity.

## 4. Fixture mapping / minimal read-only adapter proposal

**First adapter choice: frozen static fixture adapter**, not mock trees and not a live AnnualEngine binding. It may read the accepted fixture **in memory, via approved trusted bounded file/data delivery in a future implementation**; it must not write to the fixture, trust table or engine. Future Year 1 is accepted only through a verified detached canonical #725 report/read projection (or equally provenance-checked payload) supplied after E/Manager decides the custody/import path. Neither a local file path from #725 nor its stdout is available to this GitHub-only worker as a public production API.

| Frozen source / future projection | Lane A uses | Rules / unresolved schema |
| --- | --- | --- |
| fixture format, derivedSchemaVersion, domain, derivedFixtureId, catalogVersion, contentHash | source identity, banner, provenance guard | E owns eventual snapshot version/fixture digest names; never use a screen-generated ID |
| geometry.localFrame, geometry.sideMm, polygonMm, xzTransformId | boundary, axes, Fit and orientation | static source XY mm authoritative; E may deliver centred X/Z m with explicit transform |
| trees[].treeId | stable selection, inspector, tie ordering | ordinal string, unique; never use array index |
| trees[].xMm, yMm and optional unityX/unityZ binary32 bits | stem centres | compare only consistent matching representation; reject a mismatch, not mix two frames |
| trees[].status / future alive | living/dead glyph | initial all live; E defines boolean and death provenance semantics; no fake death on Year 1 |
| trees[].ageYears, dbhMm, heightMm | inspector dimensions | Year 0 source integer values; Year 1 exact numbers from approved snapshot, not from rounded table |
| trees[].dbhCm, heightM, crownRadiusM as {k:f32,b:8 hex digits} | actual f32 number for display and circle radius | decode one binary32 payload with declared byte order; validate finite nonnegative radius, unit **m**; keep bits for provenance, rounded strings only for display |
| ecology.actionEligibleDbhThresholdMm | **visual note** if shown | 170 mm Year-0 action label; 12 below it are still drawn and included in biology |
| ecology.initialEcologicalYear, growthModelVersion, rngModelVersion, simulationSeed | year 0 and debug/source footer | E owns whether seed and model IDs are part of a released read projection |
| #725 Year-1 per-ID before/after, pre-growth CI, age, mortality records | optional year 1 inspector and updated radii; zero-death state | **no unapproved write or recalculation**, no Year 2; Year 1 contract must state exact after-mortality fields |
| #725 before/postgrowth/aftermortality stand metrics | read-only stand summary with stage badge | do not confuse BA before/after or present a prediction |
| Missing health/soil/topography/racks/etc. | explicit unavailable or omitted | no inferred physiological “health,” slope, forestry roads or Irish coordinates |

**Specific identity assertion for year comparison (proposal):** both snapshots must agree on domain, accepted fixture identity/digest, 100 unique IDs and unchanged source positions; Year 0 is ecologicalYear=0, Year 1 ecologicalYear=1. The interface must atomically switch an entire verified record, never mutate a Year-0 tree object into Year 1 in-place. If a valid later snapshot legitimately lacks an ID, E must specify deletion/mortality semantics; default **reject unexplained missing IDs**, not silently drop.

### 4.1 Proposed small interfaces — names are provisional pending #740

~~~text
ReadSnapshotPort.openAcceptedFixture(fixtureId) -> DetachedReadSnapshot | explicit refusal
ReadSnapshotPort.getVerifiedYear(fixtureIdentity, ecologicalYear) -> DetachedReadSnapshot | Unavailable
ReadonlyViewer.setSnapshot(DetachedReadSnapshot) -> result (no source mutation)
ReadonlyViewer.setYear(0 | 1) -> result (read-only, no annual advance)
ReadonlyViewer.setSelectedTreeId(treeId | none) -> result
ReadonlyViewer.fit() / pan(deltaCssPx) / zoom(factor, anchorCssPx)
ReadonlyViewer.getSelectedTreeDetails() -> ReadInspectorView | none
ReadonlyViewer.getDrawingPrimitives() -> immutable view-derived circles/stems/labels
ReadonlyViewer.onSelectionChanged(treeId, snapshotRevision) -> notification only
~~~

The port is a **read interface**, not a Host endpoint, schema revision, command grammar or persistence API. Names, calling shape, numeric-bit policy, trust/refresh semantics, canonical snapshot fields and error classes are owned by **E/#740**, with any direct P1 source-adapter approval by Manager. A future implementation may choose Canvas 2D or SVG; this proposal favours **Canvas for crowns/stems plus accessible HTML list/inspector** to keep draw and hit-test deterministic and avoid a 100-label SVG collision problem. This is a proposal, not an approved technology choice.

**A/C boundary:** #736 owns canvas rendering, screen transform, hit-test, selection (inspector/navigation) and read-only viewer state. #738 owns any future marking gestures, mark lifecycle, editability and management command/approval. “Selected” never implies “marked.” Do not implement marking toggles in A. #737 owns multi-year engine and real states; #739 owns history, persistence/replay proposals; #740 owns the shared immutable snapshot, version/identity/permission boundary and release/integration gates.

If C later needs selected IDs, it can **subscribe to an A read-only selection notification keyed by treeId and snapshot revision**. C decides whether a mark command is legal. A must not hold pending marks as biological truth; if E later authorises read-only mark overlays, they are separately sourced display annotations with explicit provenance and are not created by A's click handler.

## 5. Testable draft acceptance cases (future checks, NOT executed here)

| AC | Given → action | Required observable result |
| --- | --- | --- |
| A01 | Load exact accepted static fixture | 30 m boundary; 100 ordinal IDs; all marked living; banner explicitly synthetic, no field forecast/Unity parity |
| A02 | Select P30-001 in Year 0 | Inspector gives (1.069, 1.908) m local XY, age 30, 32.9 cm DBH, 23.07 m height, ≈3.4534 m crown **radius**, no health score |
| A03 | Select P30-053 in Year 0 | Inspector shows 13.1 cm DBH, below 170 mm action label, but stem remains visible/selectable and stand biology still counts it |
| A04 | Fit at two viewport aspect ratios | Full declared 30 × 30 m square visible, equal scales along X/Y, local orientation and 5 m bar correct |
| A05 | At 20 CSS px per metre, inspect a 3 m crown | Circle centre-to-edge spans **60 CSS px**, not 30/120 arbitrary pixel units; zoom multiplies radius and scale equally |
| A06 | Zoom around pointer, then pan | Tree-to-boundary geometry remains congruent; selected ID persists; scale bar corresponds to new pixels per metre |
| A07 | Overlapping crowns, nearest centre click | Chooses nearest **stem**, tie broken ordinal ID; crown order never changes selection; ambiguity recoverable through ID picker |
| A08 | Render same data/camera twice; resize/DPR change | Same ID ordering; no missing stems; geometry preserves correct metre ratios; CSS-pixel hit-test tracks drawn centre |
| A09 | Click/drag distinction | Clicking selected stem highlights without changing pan; dragging background pans without selecting a tree accidentally |
| A10 | Search/PREV/NEXT/Tab/Enter/Escape | All 100 IDs are reachable without pointer; visible focus; selection and clear announced/legible |
| A11 | Year 1 payload unavailable or not trusted | Year 1 disabled/refused with reason; **no Year 1 constructed from #723 rounded decimals**; Year 0 intact |
| A12 | Valid matched immutable Year 1 payload | Atomic frame swap: 100 stems age 31, no dead markers, year badge “1”, selection/camera preserved; audited stand RD from payload ≈0.6375836887 |
| A13 | Return Year 1 → Year 0 | P30-001 Year-0 values restored exactly to display precision; no growth accumulated by toggling |
| A14 | Duplicate ID, mismatched domain/hash, invalid f32 radius or wrong year | Reject frame wholesale, show bounded error, retain last good frame; no partial 99-tree display |
| A15 | Sourced future dead-tree snapshot | Dead centre shown as noncolour-only distinct glyph; tree ID still selectable; only supplied cause/year displayed; no fabricated health/crown |
| A16 | Repeated click, year switch, inspect, camera actions | No mutation of biological source, fixture bytes, running year, mark state, Work Plan, saves, replay/events or RNG |
| A17 | Mock selection notification consumed by a future C module | Passes stable treeId + revision only; no mark, work or command created; #738 retains edit authority |
| A18 | 1280×720, 1920×1080, 360 CSS px; 200% text zoom | Legend/read-only disclosure and inspector reachable, no hidden required controls, no scroll trap; keyboard focus visible |
| A19 | Contrast tests in dark/light/high-contrast modes | Living/dead/below-action/selected distinguishable by glyph + text, not hue alone; text and interactive control contrast assessed |
| A20 | Instrument read-only adapter with a mutation spy | Zero calls to annual, marking, save or source-file write methods; no Host/network action from interaction |
| A21 | Frozen fixture provenance mismatch / alternate 40/80 or original P0 passed as 30 m | Explicit refusal, no automatic relabel, crop or coordinate rescale |
| A22 | Two successive identical trusted frames | Exact ID set and bit-pattern provenance preserved; no per-year recompute of stand BA, Dq or RD by viewer |
| A23 | Rendering all 100 overlapping crowns at Fit, then selecting every ID from list | Every living centre remains visible or explainably overlapping; every ID is inspectable despite occlusion |
| A24 | No supplied competition/health field | Missing CI stated “unavailable”; health stated “unavailable,” not calculated from crown or mortality |

**Test classes:** A01–A17/A20–A24 are proposed deterministic adapter/UI contract checks; A18–A19 require real rendered browser/manual accessibility evidence. No test plan here authorises running them now. Later E/Manager must assign a suitable local UI/host worker and actual permitted test ceiling. These cases do not change the accepted #725 frozen numeric/fixture anchors or L4 fail-closed status.

## 6. Future implementation plan proposal — not a dispatch

| Order | Work unit | Owner / dependency | Evidence required before next step |
| --- | --- | --- | --- |
| P0 | Manager review of this DRAFT; approve/modify UI intent separately from existing four-file P1 technical spec | Manager #730 and owner for material product choice | Explicit approved scope, interfaces, accessible design minimum and stopping criteria |
| P1 | Reconcile **E/#740** immutable snapshot fields, source units, bit encodings, accepted identity/integrity check and error classes; document read-only import/custody | E + Manager; #725 source still in protected native lane | Reviewed API/fixture trust contract, non-mutating admission, named source export permit |
| P2 | In a **future separately assigned** source worker/lane, implement isolated static Year-0 read adapter and viewer skeleton with no annual/Host mutation | Later source/UI owner, actual Ops admission and claim; exact path allowlist still unassigned | Strict ID/bounds/unit cases, frozen fixture unchanged; no network or source side effects |
| P3 | Add Canvas/SVG projection, Fit/pan/zoom, stem hit-test, ID search and accessible inspector | Later Lane A implementation worker | A01–A10, A18/A19 evidence from approved local browser |
| P4 | Wire *verified* detached Year-1 read-only snapshot; ensure stage/year/ID/crown/stand metrics switch as a whole | E/B output accepted and accessible, Manager approves import | A11–A15, A21/A22; read comparison with qualified Year-1 evidence and explicit provenance |
| P5 | Independent nonauthor review and bounded feature qualification, with security and source-custody checks | Separate reviewer and Manager | Verified one-writer diff, no prohibited action routes; no acceptance/merge inferred |
| P6 | Only after all blockers resolved, decide product integration path and new scope for C marking UX/history | Manager #730, E/#740 and product owner | Separate approved specs/permits; never fold unreviewed C/D behaviour into A |

**No source or integration permission is requested by this result.** An implementation lane/worktree, chosen host and GitHub source branch remain **UNASSIGNED**. The existing #710 R custody (through 23 Nov 2026 18:00 UTC) and immutable #725 custody (through 27 Nov 2026 18:00 UTC) must be reconciled by the real native custodian before any future file access; the no-arbitrary-worker-expiry owner policy is not an automatic extension of existing physical records. No Windows build, Unity, source merger, test rerun or L4 waiver.

## 7. Decision / integration blockers returned to Manager and #740

| Ref | Decision or missing evidence | Proposed disposition / owner |
| --- | --- | --- |
| **OPEN-A1 (BLOCKING)** | No approved E shared snapshot exact fields, per-year identity, binary32 representation, schema, provenance/admission or detached-read adapter | E/#740 publishes proposal; Manager independent review before implementation |
| **OPEN-A2 (BLOCKING Year-1 toggle)** | #725 Year-1 JSON lives in a protected local native lane, not a published integrated API; #723 six-decimal table cannot stand in | Owner/custodian authorises a safe source-derived, versioned, immutable export; Manager verifies digest and provenance |
| **OPEN-A3 (PRODUCT CHOICE)** | Canvas + accessible DOM static adapter vs another UI stack; standalone file/browser vs isolated loopback host not accepted for this slice | Recommend static adapter + Canvas + HTML inspector for the first *read-only* viewer; defer Host and any network entirely; owner/Manager decides |
| **OPEN-A4 (UI RULE)** | Availability and exact semantics of “health” unknown | Expose living status + sourced model diagnostics; **no health score** until an evidence-backed field exists |
| **OPEN-A5 (CROSS-LANE)** | Interaction between A selection and C marking input, overlay/revision semantics not accepted | A publishes stable selected ID notification only; C owns decisions; E owns revision/identity |
| **OPEN-A6 (FUTURE DEAD)** | First qualified fixture has zero Year-1 deaths; #729 M-1 requires dead-stem filtering before Year 2 | Specify render-ready dead glyph, but no invented dead fixture or Year-2 acceptance; defer semantics to B/E |
| **OPEN-A7 (RELEASE)** | P1 source not integrated, default .NET L4 FAIL38 unwaived, no independent viewer/browser run or host security evidence | **No integration/release claim**; E/Manager owns security and integration decisions, not A |

**Recommendation for Manager:** accept this as a bounded **DRAFT UI proposal suitable for independent specification review**, not as a working implementation or production-ready contract. Prioritise OPEN-A1 and OPEN-A2 before allocating a source worker. A read-only Year-0 view can be specified fully without inventing Year-1 state; enabling the actual toggle requires the verified Year-1 payload.

## 8. Handoff and evidence limitations

- **Work completed:** source/document inspection, grounded interaction specification, exact fixture mapping, deterministic coordinate/crown/hit-test proposal, text wireframe, accessibility/scale cases, proposed read-only interfaces, ownership and future gate plan.
- **Review/verification performed here:** GitHub source reading only. No checked code, browser-rendered viewer, native commands/tests/builds, runtime qualification, fixture mutation, or CCF-Ops changes. Published source fixture schema and selected records were inspected via GitHub; the local #725 Year-1 report files and physical custody were **not** accessed.
- **Passed/failed:** authoring checklist *specified*; no new software check passed or failed. Existing #729/#732 evidence is accurately cited; L4 remains FAIL38 UNWAIVED.
- **Outstanding:** #740 shared contract review and Year-1 export decision, Manager approval of draft, independent design review, then separately authorised implementation.
- **Dashboard/Drive sync:** not verified; Manager/canonical publisher owns any sync. No claim of synced status.
- **User action:** none to receive this DRAFT; any material standalone UI/Host choice or later production start requires the appropriate owner/Manager decision and permissions.

**HANDOFF REQUEST:** Manager #730 to review for **APPROVED / NEEDS CHANGES / REJECTED as documentation quality only**, route concrete cross-lane contract questions to #740, and retain the specification status **DRAFT** until actual approval. No self-approval.
