# Requirements / verification checklist — Irish Sitka 2D Reference Laboratory V1

**Status:** **APPROVED FOR P0 REQUIREMENTS ONLY** (direct user decision, MULTI-001, 14 November 2026); acceptance boxes are **still unchecked** until actual implementation tests. P1 requirements remain conditional/not authorised.

## Product and scope

- [ ] **REQ-P01** **OWNER APPROVED P0 scope:** standalone research lab, not Scenario One/Unity map UI; implementer must conform.
- [ ] **REQ-P02** Site is flat, Sitka-only, **checked-in synthetic ENGINEERING TEST FIXTURE**, titled “Synthetic Sitka test stand (flat)” with no false field-measured or typical-Irish claim.
- [ ] **REQ-P03** Original full 2D ↔ 3D gameplay ambition remains intact, but 3D/multi-rack extrapolation is **not implemented** under this spec.
- [ ] **REQ-P04** Separate Scenario One Sitka/Oak/Beech parity and complete-loop demo authority unchanged.
- [ ] **REQ-P05** The raw GBIF/marteloscope access blocker does not magically become solved by this lab.
- [ ] **REQ-P06** **Owner-approved L1** research-only, **L2** no biology in P0 with P1 later, **L3** isolated .NET/browser; L4/L5 expressly deferred and not P0. Check implementer conformance rather than re-requesting approval.

## Core integrity — P0

- [ ] **REQ-C01** Polygon is a single simple ring (no holes/self-crossings/self-touches), ≥3 distinct noncollinear finite integer-mm vertices, positive area, either winding; reject zero-area and **bow-tie**. AreaM² and areaHa use the exact scale factors.
- [ ] **REQ-C02** Every tree has stable unique ID and **integer-mm** location/DBH/height fields; exactly-on-boundary stem is inside, any outside point **rejects entire fixture**; no edge exceptions.
- [ ] **REQ-C03** Input rejects duplicate IDs, fractional/invalid integer-mm units, nonpositive DBH/height, nonfinite/overflow, invalid species and negative inventory age.
- [ ] **REQ-C04** Correct living **above-threshold adult** /area counters and `BA=Σπ(DBH_cm/200)^2` per unique eligible living adult stem; `areaHa = areaM²/10,000`.
- [ ] **REQ-C05** `Dq=sqrt(mean(DBH_cm²))`; no substitution of ordinary arithmetic DBH mean.
- [ ] **REQ-C06** Dead/felled IDs persist for history but never contribute to eligible live counts. Below-threshold stems are displayed but excluded from adult metrics and **ineligible for work**; out-of-polygon fixture **rejected on load**.
- [ ] **REQ-C07** Same checked-in immutable synthetic fixture and runtime/schema yield identical canonical UTF-8 no-BOM/LF compact ordered JSON, SHA-256 hash, sorted tree IDs and Core-derived metrics; **no P0 PRNG**.
- [ ] **REQ-C08** Synthetic numbers never described as measured Irish field inventory or universally correct spacing/caps.
- [ ] **REQ-C09** **No P0 top-height output**; the source-specific GROWFOR sampling rule is P1-only.

## Player decisions — P0

- [ ] **REQ-D01** Separate per-tree UNMARKED/PLAYER_MARKED and per-plan DRAFT/PROPOSED/APPROVED/PENDING_WORK/EXECUTED/REJECTED/CANCELLED/STALE are visible; every transition follows spec A5 with one open-plan membership max.
- [ ] **REQ-D02** Mark, propose, approve, queue pending, reject and cancel produce **zero actual TreeState live/felled or BA changes**; audit/rejection events are not successful work.
- [ ] **REQ-D03** Execute Test Work atomically fells **all and only** unique current eligible planned live adult IDs; one bad/missing/stale/dead/duplicate/below-threshold ID rejects the **entire plan**, audit-only.
- [ ] **REQ-D04** Duplicate execution, dead ID, **load/reset of any fixture**, **baseForestRevision/sessionEpoch mismatch**, calendar advance or load-reset invalidation, and rejected/cancelled plan all fail closed; stale plan cannot be reapproved in place.
- [ ] **REQ-D05** Proposals can be rejected/cancelled without modifying forest; a revised ID list must use **new plan ID**, not silent mutation of an approved plan.
- [ ] **REQ-D06** EventSeq, prevStateHash/postStateHash (**SHA-256 over canonical forest snapshot bytes**) and immutable IDs allow event-by-event deterministic replay and matching actual adult metrics; no silent loss.

## Map and local runtime — P0

- [ ] **REQ-U01** Single canonical headless state drives all displayed map features and **all actual/proposed forestry calculations**, UI only formats Core responses.
- [ ] **REQ-U02** True metre-scale 2D plot, pan/zoom, selection of actual tree ID and DBH/height/status inspector.
- [ ] **REQ-U03** Preview removal N and BA (actual vs proposed) sourced from Core API; no browser BA calculation or misleading “felled” while pending.
- [ ] **REQ-U04** Status legend and labels remain distinguishable without relying only on colour.
- [ ] **REQ-U05** Real browser interaction tests including marked→proposed→approved→queued→executed, rejected/stale/cancelled plans, duplicate execution, Core/UI metric equality and save/replay.
- [ ] **REQ-U06** Fixed `Tools/MultiRackSitkaLab/.lab-data/` gitignored data root, **format=`ccf-sitka-lab`** and schema marker; deny absolute/outside/traversal/symlink paths and CCF-shaped JSON before acceptance; no Unity or production game-save use.
- [ ] **REQ-U07** Clear text `SYNTHETIC TEST FOREST` / `NO BIOLOGICAL GROWTH` throughout P0; “Advance experiment step (calendar only)” leaves `ageAtInventoryYears`, DBH and height unchanged and makes approved plans stale.

- [ ] **REQ-U08** Local host binds to **127.0.0.1 only**, restrictive CORS, rejects unexpected Host/Origin; no accounts, remote requests or telemetry.
- [ ] **REQ-U09** A **2,000-tree** synthetic performance fixture supports selection/inspector/Core-preview refresh within **≤2 seconds** on the registered test host/browser; no missing tree identities.

## Evidence and source safety

- [ ] **REQ-E01** GROWFOR/Broad–Lynch accurately described as **stand-state** growth reference `B,N,H`, not per-tree truth.
- [ ] **REQ-E02** Irish growth sources include documented domain/range and model-version caveats; GROWFOR canonical source is `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md`.
- [ ] **REQ-E03** P1 dynamics NOT claimed without inspected CCF growth source and independent gate.
- [ ] **REQ-E04** No falsely validated/field-measured claim; L0–L4 separately labelled and all P0 values **ENGINEERING TEST FIXTURE**.
- [ ] **REQ-E05** Source branch/worktree actually registered clean, **single P0 Core+Host+UI source writer**, authorised live board writer decides coordination; unassigned reviewers read only.
- [ ] **REQ-E06** Real local headless and browser receipts; no fabricated PASS, Unity claim or machine lease.
- [ ] **REQ-E07** GitHub handoff and independent reviewer verdict; qualified P0 ≠ approved game integration or production ecological model.
- [ ] **REQ-E08** Dashboard/Drive NOT VERIFIED unless actually synced.

## Separate P1 conditions — not part of P0 approval

- [ ] **REQ-M01** Exact relevant CCF Sitka methods/version IDs inspected from live source; document model transition semantics and protected Reference Future rules.
- [ ] **REQ-M02** Approved test-adapter/parity programme, reference vectors and scientific provenance recorded.
- [ ] **REQ-M03** Annual state account reflects actual historical work and competition; no invented “unchanged” ecology on no-work years.
- [ ] **REQ-M04** No separate undocumented store that could conflict with authoritative CCF tree/cell world.
- [ ] **REQ-M05** Aggregate Irish GROWFOR within-domain N/BA/top-height benchmarks and uncertainty given; no unjustified “perfect model” score.
- [ ] **REQ-M06** L4 measured longitudinal individual-tree accuracy remains NOT VALIDATED pending independent field observations.
