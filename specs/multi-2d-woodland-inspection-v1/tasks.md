# CCF Multi 2D — Woodland Inspection V1 — Task Checklist

- **Spec ID:** `SPEC-multi-2d-woodland-inspection-V1`
- **Branch (Part 1):** `feat/multi-2d-woodland-inspection-engine-v1`
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`)

## Part 1 — Inspection engine (ACTIVE)

| Task | Title | Acceptance | Status |
|---|---|---|---|
| **WN-T00** | Preflight | `origin/integration/2d-sim` == `c82e4e75…`; context SHA recorded; branch created from base. | `DONE` |
| **WN-T01** | View contracts | Immutable records with immutable (`ImmutableList`) collections; Missing values are `null`; `InspectionDisclosures`. | `DONE` (R1: WI-02, WI-03) |
| **WN-T02** | Built-in catalogue | Fixed order Donadea → Mersch; ATTRIBUTION.json as single source; pinned SHA cross-check; import via existing reader with `DeclaredArchiveSha256`; never a boundary; offline; built-in `InspectionView` with disclosures; nested import result exposed only as an immutable snapshot; the `changesMade` `Note:` caveat must equal the pinned vetted text. | `DONE` (directory loader is test-side — spec §4.1 deviation; R1: WI-02, WI-03; R2: WI-08) |
| **WN-T03** | Projector | Readiness, `CanAdvanceTime` rule, identity, Year-0 trees/origin/boundary, verbatim fields, species breakdown, microhabitats, attribution, provenance boundary, blocker checklist + notes; Refused ⇒ no stand data; Simulated field ⇒ stand withheld with a Refusal item, which a same-code non-Refusal issue never suppresses. | `DONE` (R1: WI-01, WI-04; R2: WI-07) |
| **WN-T04** | Blocker wording | Plain-English title/explanation for every blocker/refusal code the importer emits (57); message fallback. | `DONE` (R1: WI-05; R2: WI-09) |
| **WN-T05** | Serializer | Canonical JSON, fixed key order, invariant numbers, `-0.0` → `0`, SHA-256; `disclosures` object. | `DONE` (R1: WI-02) |
| **WN-T06** | Test executable | Standalone Exe; T1–T10 plus T1b, T2b, T3b, T8c, T10c, T10d, T10e and hygiene; golden hashes pinned. | `DONE` (20/20 after R2; goldens re-pinned in R1, unchanged in R2) |
| **WN-T07** | Gates and fence | SDK 8.0.133 restore/build/run, 0 warnings/0 errors, two identical runs; Importer V1 suite still 29/29; diff confined to the four prefixes. | `DONE` |
| **WN-T08** | Handoff + push | `IMPLEMENTATION-HANDOFF-PART1.md`; push feature branch and `review/…` ref; no merge, no PR. | `DONE` |
| **WN-T09** | Independent review | Separate fresh-thread reviewer, not the builder. | `NEEDS CHANGES` on `24250caf` (GPT-6.2 Sol: WI-01..WI-06) → Repair R1 `00fb77be`; delta review `NEEDS CHANGES` on `00fb77be` (GPT-6.2 Sol: WI-07..WI-09) → Repair R2 handed off; delta re-review `PENDING` |

R1 spec.md §4.1–4.3 corrections ratified by owner/Manager 2026-11-27 after delta-review ruling (ii).

## Part 2 — "Inspect Real Woodland" UI (QUEUED behind Playable Milestone 1)

| Task | Title | Status |
|---|---|---|
| **WN-P2-01** | On-screen picker over `BuiltInWoodlandCatalogue` (Donadea first) | `QUEUED` |
| **WN-P2-02** | Inspection screen rendering `WoodlandInspectionView` with zero UI-side readiness logic | `QUEUED` |
| **WN-P2-03** | Any wiring into `MultiAreaContinuation`, `AnnualPipeline`, saves or `/multi` | `QUEUED` (needs its own packet) |

Deferred and out of scope for both parts: importer items NF-07, NF-08, NF-09; local ZIP upload; network retrieval.
