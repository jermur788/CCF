# Requirements / verification checklist — Irish Sitka 2D Reference Laboratory V1

**Status:** DRAFT. These are proposed acceptance tests, not execution results. Mark PASS only after actual run evidence.

## Product and scope

- [ ] **REQ-P01** Owner confirms this is a standalone research lab, not new Scenario One/Unity map UI.
- [ ] **REQ-P02** Site is flat, Irish-context, Sitka-only, synthetic and tagged visibly at all times.
- [ ] **REQ-P03** Original full 2D ↔ 3D gameplay ambition remains intact, but 3D/multi-rack extrapolation is **not implemented** under this spec.
- [ ] **REQ-P04** Separate Scenario One Sitka/Oak/Beech parity and complete-loop demo authority unchanged.
- [ ] **REQ-P05** The raw GBIF/marteloscope access blocker does not magically become solved by this lab.
- [ ] **REQ-P06** OPEN-L1..L5 reviewed before promoting to APPROVED.

## Core integrity — P0

- [ ] **REQ-C01** Polygon area handles skewed/nonrectangular perimeter, zero-area and reverse winding; no double count.
- [ ] **REQ-C02** Every synthetic tree has stable unique ID and finite in-bounds local XY in metres.
- [ ] **REQ-C03** Input rejects duplicate IDs, invalid units, nonpositive DBH/height, NaN/overflow, invalid species.
- [ ] **REQ-C04** Correct living-stem/area counters and \`BA=Σπ(DBH_cm/200)^2\` per unique living stem.
- [ ] **REQ-C05** \`Dq=sqrt(mean(DBH_cm²))\`; no substitution of ordinary arithmetic DBH mean.
- [ ] **REQ-C06** Dead/felled tree and out-of-polygon exceptions are explicit and never counted as eligible living sample without authorisation.
- [ ] **REQ-C07** Same fixture configuration/seed yields reproducible tree table, ID ordering, fingerprints and metrics.
- [ ] **REQ-C08** Synthetic numbers never described as measured Irish field inventory or universally correct spacing/caps.

## Player decisions — P0

- [ ] **REQ-D01** Marked vs proposed vs approved/pending vs executed are distinct, visible statuses.
- [ ] **REQ-D02** Mark, propose, approve and pending produce **zero actual tree-state and BA changes**.
- [ ] **REQ-D03** Explicit Execute Test Work fell only eligible original unique live IDs and records actual previous/current state.
- [ ] **REQ-D04** Duplicate execution, dead ID, stale snapshot and rejected pending work fail closed.
- [ ] **REQ-D05** Proposals can be rejected or revised without modifying forest state.
- [ ] **REQ-D06** Historical event replay and saved snapshots reconcile same IDs and numeric sums; no silent event loss.

## Map and local runtime — P0

- [ ] **REQ-U01** Single canonical headless state drives all displayed map features.
- [ ] **REQ-U02** True metre-scale 2D plot, pan/zoom, selection of actual tree ID and DBH/height/status inspector.
- [ ] **REQ-U03** Preview removal N and BA (actual vs proposed), no misleading “felled” while pending.
- [ ] **REQ-U04** Status legend and labels remain distinguishable without relying only on colour.
- [ ] **REQ-U05** Real browser interaction tests including rejected proposals, duplicate execution and save/replay.
- [ ] **REQ-U06** Clear LAB-ONLY data path; no Unity, live game saves or production model import.
- [ ] **REQ-U07** Clear text \`SYNTHETIC TEST FOREST\` / \`NO BIOLOGICAL GROWTH\` throughout P0.

## Evidence and source safety

- [ ] **REQ-E01** GROWFOR/Broad–Lynch accurately described as **stand-state** growth reference \`B,N,H\`, not per-tree truth.
- [ ] **REQ-E02** Irish growth sources and example data include domain/range and newer-model-version caveats.
- [ ] **REQ-E03** P1 dynamics NOT claimed without inspected CCF growth source and independent gate.
- [ ] **REQ-E04** No false ecological accurate/field measured statement; L0–L4 individually labelled.
- [ ] **REQ-E05** Source branch and worktree actual and clean; one writer per path; live coordination read only to unauthorised writers.
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
