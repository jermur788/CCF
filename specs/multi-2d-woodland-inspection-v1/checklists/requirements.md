# CCF Multi 2D — Woodland Inspection V1 — Requirements Checklist (Part 1)

| ID | Requirement | Verification |
|---|---|---|
| **WN-REQ-01** | **Option A #1:** built-in qualified catalogue only, exactly two entries, Donadea first then Mersch; no upload, no network. | T1; hygiene test (no `System.Net`/`HttpClient`) |
| **WN-REQ-02** | Catalogue metadata from ATTRIBUTION.json: full citation, `CC BY 4.0` + URL, DOI, dataset key, retrieval date, verbatim `changesMade`, caveats incl. unverified-coordinate inference; both disclosures also carried into the built-in view and its JSON (`disclosures`) without relabelling provenance (R1: WI-02); a `changesMade` `Note:` that differs from the pinned vetted caveat (for example one that mentions `verbatimCoordinates` but drops the unverified-inference qualification) fails to load (R2: WI-08). | T1, T1b |
| **WN-REQ-03** | ATTRIBUTION SHA-256 must equal the pinned value; ZIP imported with `DeclaredArchiveSha256` so tampered bytes are Refused. | T1, T2, T2b |
| **WN-REQ-04** | No boundary is passed or fabricated for built-in plots; both import as InventoryOnly. | T1, T8a |
| **WN-REQ-05** | **Option A #2:** `CanAdvanceTime` true only for Candidate + stand present + zero Simulated fields; false for InventoryOnly and Refused. | T2, T4, T5, T8b, T9 |
| **WN-REQ-06** | Every measurement projected verbatim (provenance, value or null, source column, raw value, note); Missing never estimated. | T6, T7 |
| **WN-REQ-07** | Year-0 provenance boundary: observed year 0, future from year 1, `SimulatedFutureAvailable = false`, `SimulatedValueCount` reported; no `Simulated` provenance in built-in JSON; a stand holding a `Simulated` field is withheld (no simulated value in view or JSON) with a renderable `SimulatedFieldAtYearZero` refusal (R1: WI-01), which a same-code Warning, Info or InventoryOnlyBlocker for the same tree and field never suppresses (R2: WI-07). | T7, T8a, T8b |
| **WN-REQ-08** | Blocker checklist: one item per blocking code, Refusal first then ordinal, plain-English wording for every importer blocker/refusal code (R1: WI-05; R2: WI-09 `ArchiveSha256Mismatch` names both byte mismatch and conflicting declarations) with fallback, sorted tree ids and raw species names; non-blocking issues in notes. | T2, T4, T5, T6, T10b, T10c, T10d |
| **WN-REQ-09** | Species breakdown lists all four categories (total + living) and sorted per-species rows. | T10a, T10b |
| **WN-REQ-10** | Microhabitats from `trem:` attributes, sorted ordinal, values verbatim. | T10a, T10b |
| **WN-REQ-11** | Refused import yields a view with no stand, no trees, no attribution and listed refusal blockers — even when the result object carries a stand (R1: WI-04). | T2, T5, T8c |
| **WN-REQ-12** | Canonical JSON deterministic across repeated projection and independent catalogue loads; golden SHA-256 of each built-in `InspectionView` pinned (re-pinned in R1). | T3, T3b; two runs |
| **WN-REQ-13** | All returned collections are immutable, including the catalogue, entries and each entry's nested import result; no backing collection is reachable through `ICollection.SyncRoot` (R1: WI-03). | T10b, T10e |
| **WN-REQ-14** | **Option A #3:** Importer V1 not reopened — no change under `src/Core/WoodlandImport/*.cs` or `tests/Core/WoodlandImport/**`; Importer V1 suite still passes. | `git diff --name-only`; Importer V1 run 29/29 |
| **WN-REQ-15** | Pure BCL, 0 warnings / 0 errors on SDK 8.0.133; no file/clock/random/network/Unity/simulation-wiring API in `Inspection/`; diff inside the four prefixes. | build output; hygiene test; Importer V1 hygiene test; fence check |

## Part 2 — `/multi` wiring (Playable Milestone 3 Part 2)

| ID | Requirement | Verification |
|---|---|---|
| **WN-P2-REQ-01** | Built-in catalogue served offline from embedded fixtures; Donadea first, Mersch second; no upload, path or network input accepted. | API C01–C08, V02; Edge "dialog lists Donadea first and Mersch second" |
| **WN-P2-REQ-02** | Load returns the Part 1 view verbatim (SHA-256 equals the Part 1 goldens) with catalogue/display metadata. | API L-*-golden; WoodlandInspection T3 |
| **WN-P2-REQ-03** | While a `CanAdvanceTime == false` plot is inspected, every simulation-changing POST (Advance, Skip, Work Plan approve/execute/cancel and edits, Save, Money switch, future routes) is refused with HTTP 409 `WoodlandInspectionReadOnly` in plain English; nothing changes. | API R-* (12 routes ×2), R01; Edge server probes |
| **WN-P2-REQ-04** | Return to normal woodland, a committed New woodland and a committed Load each clear the inspection; a refused New woodland does not; no stale state after reload. | API X01–X16; Edge reload + return checks |
| **WN-P2-REQ-05** | Survey map: flat local metres, X east / Y north as stored, no inferred boundary, plotted/unplotted counts stated. | Edge map checks |
| **WN-P2-REQ-06** | Inspector shows each value or "Not recorded" with provenance and source; never invents height, age, crown radius, position, species or boundary. | API L-*-Missing; Edge inspector checks |
| **WN-P2-REQ-07** | Species mapping (all four categories + per-raw-name counts), TreMs records, full CC BY 4.0 attribution, citation, DOI, observation dates, modifications and caveats are visible; no internal codes on screen. | Edge content + code-leak checks |
| **WN-P2-REQ-08** | Persistent "Survey inspection mode — Year 0" banner with blocker checklist and visibly disabled Advance / Skip / Work Plan execution; Return restores the playable view and Advance. | Edge banner/disabled/restore checks |
| **WN-P2-REQ-09** | Module graph acyclic and reachable, exactly one `<script>` tag, dark theme, two columns only; no regression in builds, engine suites, 9 anchors or UI smoke. | module-graph; builds; suites; smoke |
