# Task breakdown — Irish Sitka 2D Reference Laboratory V1

**Status:** DRAFT / NOT EXECUTABLE until the complete feature spec is APPROVED and ownership is registered. Tasks are proposed, not claimed, and do not authorise T3 Code/Unity. No worktree or code has been created.

| Task | Goal | Difficulty / best lane | Dependencies / evidence gate | Deliverable and verdict |
|---|---|---|---|---|
| **LAB-00** | Independent four-file delta review and explicit user/designated Overall Manager approval of OPEN-L1..L3; OPEN-L4/L5 deferred outside P0 | Medium; distinct non-author GPT-6/Claude reasoning/repo reviewer | Review `spec.md`, project authority, #472/475/479 and Irish GROWFOR extraction | APPROVED / NEEDS CHANGES / REJECTED; no source coding before APPROVED |
| **LAB-01** | P0 local lane/technology preflight; no-overlap ownership | Low–medium; assigned local Sol/Codex High | Formal approved P0, actual issue claim and branch/HEAD/worktree, physical Ops/host/disk state | Written preflight + isolated new directory only |
| **LAB-02** | Pure C# canonical integer-mm schema, checked-in synthetic fixtures (NO P0 PRNG), valid polygon, ID and BA/N/Dq exact mathematics | Medium; sole local core writer | LAB-01 and P0 approved scope | Golden immutable fixture SHA, units/adult threshold/polygon (simple, boundary-inclusive; bow-tie/outside rejected), BA/Dq exact hand-oracle tests |
| **LAB-03** | Explicit tree-mark/plan-state table, atomic work/approval/pending/stale tokens, immutable events and canonical lab-only save/replay | Medium–High; same sole writer | LAB-02 | Atomic reject-all on one bad ID; baseForestRevision/sessionEpoch, eventSeq and SHA-256 per-step replay, bad load/CCF-save/path negative tests |
| **LAB-04** | Lightweight 2D map and local browser host bound to domain, not independent tree store | Medium; SAME SOLE P0 SOURCE WRITER across Core, Host and UI | LAB-02/03; avoid shared simultaneous writes | Real browser click sequence/screenshots, status/legend, Core metric parity and 127.0.0.1 host security |
| **LAB-05** | P0 Tier 1 headless + Tier 2 actual browser qualification, negative controls, independent review | Medium; independent reviewer after writer stops | LAB-02–04 | Verified atomic one-time felling, no-cut, stale work, CCF-shaped/escaping-path rejection, 2,000-tree ≤2s local selection/preview, save roundtrip, P0-only verdict |
| **LAB-06** | Optional audited Sitka annual model comparator choice and source parity feasibility, NO engine code | High; independent source/science reviewer | P0 accepted; read exact current CCF implementation + GROWFOR extraction and constraints | Separate evidence report: model choice, deterministic fixtures, provenance, applicability limits, P1 APPROVE/HOLD |
| **LAB-07** | Conditional P1 growth/competition/mortality model and stand-level GROWFOR comparison | High; approved local core writer plus independent science/evidence review | LAB-06 model gate approved and own distinct implementation claim | Per-year tree ID ecology ledger, no-cut/thin scenarios, model version and errors/uncertainty; no L4 claim |
| **LAB-08** | Future measured Irish marteloscope / later 3D and other species | Out of scope | Actual licensed field IDs/XY/DBH/polygon and new specs and source ownership | No implementation in V1 |

## P0 evidence/acceptance checklist

- [ ] All LAB-00 product/technical boundaries approved; branch/claim/worktree real, non-overlapping.
- [ ] Same checked-in immutable fixture + pinned runtime/schema gives byte-identical canonical snapshot JSON/IDs and Core-derived metrics; no generator/PRNG required for P0.
- [ ] Tree ID, site area, adult DBH and state fields carry units + synthetic provenance.
- [ ] BA, N, Dq test values match independently verified hand-computable oracles, including dead/stump/nonrectangular polygon; bow-tie invalid, on-boundary valid, outside invalid, below-threshold displayed but not actionable.
- [ ] Explicit tree mark and plan DRAFT→PROPOSED→APPROVED→PENDING_WORK transitions; mark/propose/reject/approve/queue/pending do not change living tree count, BA or tree status.
- [ ] Execute is **atomic** only once for all valid planned live adult IDs, never silently drops invalid IDs; a single failed ID rejects the entire execution with only an audit rejection event.
- [ ] Dead, below-threshold, stale forest revision/session epoch, repeated, duplicate, out-of-polygon, self-intersecting, invalid coordinate, outside-directory/symlink and CCF-shaped JSON cases fail closed.
- [ ] UI map and Core display same tree/plan IDs, statuses, actual and preview statistics; browser performs no independent forestry arithmetic.
- [ ] 127.0.0.1-only lab host and fixed `.lab-data` root save roundtrip preserve format marker/schema, integer-mm canonical bytes, SHA-256 and sourceClass Synthetic; no unrestricted file picker and no CCF saves accessed.
- [ ] Browser exercised in actual runtime; no fabricated screenshots or build/test PASS.
- [ ] P0 displays NO BIOLOGICAL GROWTH. Calendar-only experiment step does not increment `ageAtInventoryYears`, DBH or height; invalidates existing approved/pending plans.
- [ ] Independent review and GitHub handoff published; no claim of user acceptance, Irish accuracy, Scenario One integration, or Windows build.

## Conditional P1 acceptance checklist

- [ ] Current Sitka source/mechanics and D-048 source-version paths inspected; parity vectors published.
- [ ] Model name/version/unit, source and provenance documented; no copied unlicensed binaries or silent coefficient substitution.
- [ ] Year-by-year biological changes driven by changed living stand after executed work; no growth on proposal-only.
- [ ] DBH/height/competition and mortality measured separately, reproducible deterministic seeds or stated stochastic repetitions.
- [ ] Within-domain, stand-level comparisons with Irish GROWFOR/Broad–Lynch reported with error; no model-prediction parity rule invented.
- [ ] Individual-tree ecological predictive accuracy and future marteloscope validation remain UNKNOWN unless independent longitudinal field data received.

## Dispatch rules

Do not route a local implementation chat until **user or designated Overall Manager records explicit P0 spec APPROVED** after independent delta re-check and resolves OPEN-L1..L3, and live lane ownership is confirmed. Then create a separate implementation claim and a paste-ready **direct GitHub packet URL**, including actual branch/worktree, exact allowed paths, provider, gates, output report path and owner action. The reference lab is parallel to—not a rewrite of—Scenario One.
