# Plan — Repeated Representative Rack Management

**Status:** DRAFT, no source implementation authority. Related [spec](spec.md), [tasks](tasks.md), [requirements](checklists/requirements.md).

**Phase 0 — Source and condition-map truth audit (GitHub docs/offline):** verify exact newest UI candidate and current MFM-derived source assumptions, existing fixed centre-rack sample, permitted full sample and coverage masks, saving of source rack/year and unit provenance, one-sample current policy and manager-derived per-area historical metrics. Map user intent against existing D-065 first teaching and actual S1-D repeat cycles. No real-world terrain/spatial classifier assumed. Compare representative homogeneous Sitka and intentionally heterogeneous density/species conditions in **offline fixtures** before any UI prototype.

**Phase 1 — Product design refinement:** draft a very short player flow:
`Inspect initial fixed teaching sample → execute → review comparable areas → confirm candidates → accept prescription → approve Work Plan → execute → advance 1 year or skip to review → inspect changed whole stand and prior work → choose whether new sample is needed`.
Show 3 outcomes (all areas comparable, one different rack requiring another sample/manual choice, multiple conditions requiring 2+ samples) without adding a compulsory rack-count objective. Define per-area exclusivity and a policy for a later sample without modifying first sample credit or already-approved prescription history.

**Phase 2 — Data/UX spec revision:** decide how much “see impacts in all racks” requires:
- Tier 0: existing 3D tree/world updates and current state after every annual tick (no new persistence);
- Tier 1: recorded executed-work overlays per rack/year via existing work-order provenance and S1-C history (may be read-only UI);
- Tier 2: true **per-rack yearly ecological snapshot history** (new data contract, likely save/hash-sensitive, separately approved only if necessary).
Prefer Tier 0/1 for a first slice. If Tier 2 is accepted, audit v24+ save/hash migration and requalify frozen compatibility; no invented historical per-rack baseline for old saves.

**Phase 3 — Later-cycle source/rule contract:** specify versioned new selected source sample geometry, current-state demonstration requirements, repeat-after-growth invalidation/recalibration, multiple source coverage ownership and candidate confirmation workload. Preserve completed work and first teaching sample as historical fact. Define Stop conditions and current player judgment.

**Phase 4 — Implementation only after explicit Manager APPROVED four-file spec:** use one source owner, clean isolated branch/worktree from current accepted UI lineage, separate from #382 personal playtest; do not mix with unrelated #396 time-advance changes under concurrent writes. Reuse MFM coverage/rule core where actually appropriate, add minimal new layers, never create a parallel event/history authority. Route bounded C# source to local GPT-6.1 Sol Medium; independent Claude B review if new persistence, multiple sample source or history contract. No concurrent Unity Editors and no Windows build.

**Phase 5 — Proportional evidence:** offline unit/coverage checks first; qualifying two management cycles with elapsed years, one homogeneous and one deliberately not-covered area, both options of annual advance, actual tree/proposal approvals, before/after observable metrics, old saves and frozen Reference Future v1; then focused UI render 720p/1080p. Tier-3 only combined milestone candidate. Manager issues APPROVED / NEEDS CHANGES / REJECTED with exact handoff.

**Priority:** Keep current complete Scenario One and approved MFM baseline intact. This is a D-076 amendment to later management cycles, not an instruction to rewrite MFM V1 or to expand to future heterogeneous terrain modelling.
