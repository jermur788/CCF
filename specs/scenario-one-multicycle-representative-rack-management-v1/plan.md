# Plan — Repeated Representative Rack Management V1

**DRAFT / no MFM source, UI or persistence implementation release.** Related [spec](spec.md), [tasks](tasks.md) and [requirements](checklists/requirements.md).

## Phases and dependencies

**R0 — Preserve the qualified base.** Pin a current accepted post-C-01/D-077 source candidate; read current D-065 Rack V2, ScenarioOneCoverage, prescription/approval, Work Plan and S1-C source. First fixed centre-rack lesson, three Crop Trees, geometry, old-save and Sitka-only rule remain byte-unchanged until a new approved spec explicitly amends them. Browser Part 5 helper is read-only history only; not new source-selection authority.

**R1 — Offline source/heterogeneity experiments.** Run deterministic homogeneous Sitka and deliberately different-condition stand fixtures. Record actual coverage-unit IDs, source footprint, density/DBH/Hegyi data, mask and excluded units, boundary fit, presence of Oak/Beech, manual override and source-year context. Explore a second source only where the initial comparison fails, not simply “one sample per rack.” No site moisture/wind/slope data is invented.

**R2 — Manager product decisions.** Set intended first shipping tier (3D + executed work history vs historical rack ecology); choose source eligibility and reuse, one-source+manual vs simultaneous multi-source episode; settle unit ownership, explicit conflict resolution and treatment-size calibration. Need an independently reviewed save/identity design if a new sourceArea/episode record is required.

**R3 — Data and API design.** Separate live transient proposal from durable accepted prescription, Pending Work Plan and recorded annual resolution. Proposed source identity includes source rack/area, episode, sample year and rule fingerprint; ownership map keys by episode + coverage-unit ID, fail-closed on collision. Provide versioned stale-state invalidation triggered by changed actual stand before new approval. Old prescribed jobs retain historical original IDs; new jobs must not conflict or re-fell already resolved trees.

**R4 — Bounded future implementation.** Once APPROVED, route a local C# source worker for rack sample-selection/coverage extension and separate UI work only after #397 finishes and Manager grants exclusive file ownership. Use existing MFM pure rule core where compatible rather than modifying completed first-teaching semantics. If multiple-source support adds persisted identity, require save/schema/hash reviewer; do not bypass current all-or-nothing approval/rollback and Work Plan commitment.

**R5 — Verification.** Tier 1 pure geometry/coverage/duplicate unit fixture; Tier 2 exact-head C#/Unity first teaching, second-cycle nontrivial sample, accepted proposal→Pending→Work Plan→actual trees; 3D post-year changes in all treated racks, chronological review with realistic missing-year behavior, 720p/1080p, old-save/Reference world IDs. Unrelated Tier-3 only at next integrated milestone candidate.

**R6 — Manager verdict and integration.** Publish full source and actual qualified observations, independent review for altered persistence/coverage provenance, Manager APPROVED/NEEDS CHANGES/REJECTED, then controlled integration/coordination and dashboard sync. Not automatically done because a draft or static source exists.

## Proposed two-interface ownership

**3D forest:** manual crop-tree choice, sample decisions, candidate confirmation and actual physical observation. **Stand Map/Management:** confirmed rack geometry and coverage groups, source vs comparable/manual/excluded areas, navigation and status; never bulk Fell marks. **Work Plan:** final commitment and money. **Annual Review:** executed recorded work and modelled stand/species time changes, never unrecorded per-rack historic ecology. **D-077:** annual navigation only.

Stop and return Manager if new fields would change protected hash/reference or if source area identity cannot be made stable through saved rack revisions. No direct merge to diverged main, no user #382 lane, no Windows build.
