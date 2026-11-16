# Multi P1 — standalone 2D forest interface mock (#655)

**Status: isolated UI research prototype, not P1 production.** This folder is a complete dependency-free static page. Its only data are **14 hand-authored MOCK-### synthetic records** in app.js. No P0/P1 fixture, source manifest, imported original polygon, growth engine, Unity file, Host or API is used.

## Open locally

1. Obtain this folder from the feature/multi-p1-655-standalone-mock-ui-20261116-v1 GitHub branch.
2. Open index.html directly in a modern browser. No npm, build, package, local server or login is required. All paths are relative.
3. Use the **Derived 30 m experiment** synthetic mock view. Click a map circle, or Tab to a marker and press Enter/Space. The inspector and tree-table Select buttons follow the same ID.
4. Search for a MOCK-### ID; filter living/dead/actionable/below-threshold records and sort. The selected ID remains stable even if a filter hides its table row.
5. Switch domain radio buttons: **Original polygon** and **Unity parity** show explicit unavailable/source-not-loaded states rather than invented geometry. Select **Illustrative comparison** to inspect two static, non-time-linked diagram examples.

All work and annual actions are disabled. They cannot be enabled in this mock. The **EXAMPLE SCREEN — NO ECOLOGICAL YEAR EXECUTED** screen is not a historical or predicted year. No files are created or saved.

## Provenance and numbers

* **30 × 30 m**, 900 m² / **0.09 ha**: illustrative local XY square. Diagram circle placements are scaled from the authored x/y fields; they are **not** the separately generated P1 100-tree manifest coordinates.
* **Trees:** 14 fixed examples; 12 labelled living, 2 labelled dead. Status, species, age, DBH, height, crown and XY are all illustrative guesses used to demonstrate layout. For the two dead records, death cause and year are deliberately **unknown**.
* **Mock work threshold:** DBH ≥ **170 mm** (17 cm) among living stems. This is an illustrative eligibility filter informed by the P0 example, **not an authoritative P1 runtime rule** and not a biological census cutoff. Nine mock trees match; three living trees fall below it. No tree is marked, proposed, approved or executed.
* **Metrics:** All-living N=12 includes every mock living stem regardless of size. Actionable N=9 is a subset. BA = Σ(π/4) × (DBH_mm/1000)² in m²; dead records are excluded from both. N/ha = N/0.09 and BA/ha = BA/0.09; output rounding is to 1 decimal for N/ha and 3 for BA/BA per hectare. Arithmetic is on the immutable synthetic fixture, not on actual P1 or P0 metrics. No growth or mortality calculations.
* **Source domains:** The distinct tags DerivedP1ThirtyMetreExperimentV1, OriginalPolygonExploratoryV1, and DerivedUnitySquareParityV1 are shown as design references only. P1 F0 #639 recorded derived/catalog and original trust records as **PendingIndependentReview**, not accepted. This mock never reads or promotes them. Original P0 polygon coordinates, IDs and area are not claimed or duplicated. Legal Unity 40/80 candidates remain unavailable.
* **No ecology:** It does not run an annual step, infer growth, simulate light, canopy, wind, storm, deer, recruitment, mortality, timber prices or ecological consequences. All future first-year metrics and years are absent.

## Accessibility and limitations

Semantic controls with visible focus; HTML table selection (native buttons), SVG role=button / tabindex=0 map points with Enter/Space, accessible marker names, text-based status, filtered-row notice, responsive laptop/mobile layouts and a reduced-motion rule. Mouse hover also reveals IDs. The map is a sketch, and the table is the non-visual alternative.

There is **no localStorage, cookies, file I/O, navigation to an API, fetch, WebSocket, timer, autosave, persistence, ecological year, work endpoint, asset import or public deployment**. Refresh restores initial UI selection, domain and tab. JavaScript must be enabled to populate map/table/inspector; static disclosures remain in HTML if disabled. This project does **not** prove browser compatibility, security review, runtime/keyboard accessibility, site/legal validation or Irish forestry accuracy.

## Future integration boundaries (NOT implemented)

A future approved P1-05 UI would need a separately authorised, stable Core/Host read API with reviewed manifests and trust state, accurate source domain descriptors, real all-living and actionable definitions with metric units, stable tree IDs, read-only inspection, source-controlled permissions, event/revision guards for marking and work commitments, an actual permitted annual command with deduplication/unknown-outcome resolution, and authoritative before/after/ledger responses. It would need independent browser/Host/security and accessibility tests. None may be inferred from this mock.

Do not merge this into Tools/MultiRackSitkaP1Lab/** or Tools/MultiRackSitkaLab/** without a new Manager-approved task and file reservation. No Unity, save schema or Windows build is involved.

**References:** GitHub #655 binding packet; #650 browser-only first-year flow draft (Manager-approved as **draft input**, not as production UI spec); #639 F0 foundation source handoff. This folder is not an implementation of those future contracts.
