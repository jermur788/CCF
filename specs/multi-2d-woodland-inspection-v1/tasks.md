# CCF Multi 2D — Woodland Inspection V1 — Task Checklist

- **Spec ID:** `SPEC-multi-2d-woodland-inspection-V1`
- **Branch (Part 1):** `feat/multi-2d-woodland-inspection-engine-v1`
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`)

## Part 1 — Inspection engine (ACTIVE)

| Task | Title | Acceptance | Status |
|---|---|---|---|
| **WN-T00** | Preflight | `origin/integration/2d-sim` == `c82e4e75…`; context SHA recorded; branch created from base. | `DONE` |
| **WN-T01** | View contracts | Immutable records with read-only collections; Missing values are `null`. | `DONE` |
| **WN-T02** | Built-in catalogue | Fixed order Donadea → Mersch; ATTRIBUTION.json as single source; pinned SHA cross-check; import via existing reader with `DeclaredArchiveSha256`; never a boundary; offline. | `DONE` (directory loader is test-side — spec §4.1 deviation) |
| **WN-T03** | Projector | Readiness, `CanAdvanceTime` rule, identity, Year-0 trees/origin/boundary, verbatim fields, species breakdown, microhabitats, attribution, provenance boundary, blocker checklist + notes. | `DONE` |
| **WN-T04** | Blocker wording | Plain-English title/explanation for each known importer code; message fallback. | `DONE` |
| **WN-T05** | Serializer | Canonical JSON, fixed key order, invariant numbers, `-0.0` → `0`, SHA-256. | `DONE` |
| **WN-T06** | Test executable | Standalone Exe; T1–T10 plus T2b, T3b, T10c and hygiene; golden hashes pinned. | `DONE` (16/16) |
| **WN-T07** | Gates and fence | SDK 8.0.133 restore/build/run, 0 warnings/0 errors, two identical runs; Importer V1 suite still 29/29; diff confined to the four prefixes. | `DONE` |
| **WN-T08** | Handoff + push | `IMPLEMENTATION-HANDOFF-PART1.md`; push feature branch and `review/…` ref; no merge, no PR. | `DONE` |
| **WN-T09** | Independent review | Separate fresh-thread reviewer, not the builder. | `PENDING` |

## Part 2 — "Inspect Real Woodland" UI (QUEUED behind Playable Milestone 1)

| Task | Title | Status |
|---|---|---|
| **WN-P2-01** | On-screen picker over `BuiltInWoodlandCatalogue` (Donadea first) | `QUEUED` |
| **WN-P2-02** | Inspection screen rendering `WoodlandInspectionView` with zero UI-side readiness logic | `QUEUED` |
| **WN-P2-03** | Any wiring into `MultiAreaContinuation`, `AnnualPipeline`, saves or `/multi` | `QUEUED` (needs its own packet) |

Deferred and out of scope for both parts: importer items NF-07, NF-08, NF-09; local ZIP upload; network retrieval.
