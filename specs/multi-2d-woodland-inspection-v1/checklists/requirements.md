# CCF Multi 2D — Woodland Inspection V1 — Requirements Checklist (Part 1)

| ID | Requirement | Verification |
|---|---|---|
| **WN-REQ-01** | **Option A #1:** built-in qualified catalogue only, exactly two entries, Donadea first then Mersch; no upload, no network. | T1; hygiene test (no `System.Net`/`HttpClient`) |
| **WN-REQ-02** | Catalogue metadata from ATTRIBUTION.json: full citation, `CC BY 4.0` + URL, DOI, dataset key, retrieval date, verbatim `changesMade`, caveats incl. unverified-coordinate inference; both disclosures also carried into the built-in view and its JSON (`disclosures`) without relabelling provenance (R1: WI-02). | T1, T1b |
| **WN-REQ-03** | ATTRIBUTION SHA-256 must equal the pinned value; ZIP imported with `DeclaredArchiveSha256` so tampered bytes are Refused. | T1, T2, T2b |
| **WN-REQ-04** | No boundary is passed or fabricated for built-in plots; both import as InventoryOnly. | T1, T8a |
| **WN-REQ-05** | **Option A #2:** `CanAdvanceTime` true only for Candidate + stand present + zero Simulated fields; false for InventoryOnly and Refused. | T2, T4, T5, T8b, T9 |
| **WN-REQ-06** | Every measurement projected verbatim (provenance, value or null, source column, raw value, note); Missing never estimated. | T6, T7 |
| **WN-REQ-07** | Year-0 provenance boundary: observed year 0, future from year 1, `SimulatedFutureAvailable = false`, `SimulatedValueCount` reported; no `Simulated` provenance in built-in JSON; a stand holding a `Simulated` field is withheld (no simulated value in view or JSON) with a renderable `SimulatedFieldAtYearZero` refusal (R1: WI-01). | T7, T8a, T8b |
| **WN-REQ-08** | Blocker checklist: one item per blocking code, Refusal first then ordinal, plain-English wording for every importer blocker/refusal code (R1: WI-05) with fallback, sorted tree ids and raw species names; non-blocking issues in notes. | T2, T4, T5, T6, T10b, T10c, T10d |
| **WN-REQ-09** | Species breakdown lists all four categories (total + living) and sorted per-species rows. | T10a, T10b |
| **WN-REQ-10** | Microhabitats from `trem:` attributes, sorted ordinal, values verbatim. | T10a, T10b |
| **WN-REQ-11** | Refused import yields a view with no stand, no trees, no attribution and listed refusal blockers — even when the result object carries a stand (R1: WI-04). | T2, T5, T8c |
| **WN-REQ-12** | Canonical JSON deterministic across repeated projection and independent catalogue loads; golden SHA-256 of each built-in `InspectionView` pinned (re-pinned in R1). | T3, T3b; two runs |
| **WN-REQ-13** | All returned collections are immutable, including the catalogue, entries and each entry's nested import result; no backing collection is reachable through `ICollection.SyncRoot` (R1: WI-03). | T10b, T10e |
| **WN-REQ-14** | **Option A #3:** Importer V1 not reopened — no change under `src/Core/WoodlandImport/*.cs` or `tests/Core/WoodlandImport/**`; Importer V1 suite still passes. | `git diff --name-only`; Importer V1 run 29/29 |
| **WN-REQ-15** | Pure BCL, 0 warnings / 0 errors on SDK 8.0.133; no file/clock/random/network/Unity/simulation-wiring API in `Inspection/`; diff inside the four prefixes. | build output; hygiene test; Importer V1 hygiene test; fence check |
