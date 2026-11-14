# Task breakdown — Irish Sitka 2D Reference Laboratory V1

**Status:** DRAFT / NOT EXECUTABLE until the complete feature spec is APPROVED and ownership is registered. Tasks are proposed, not claimed, and do not authorise T3 Code/Unity. No worktree or code has been created.

| Task | Goal | Difficulty / best lane | Dependencies / evidence gate | Deliverable and verdict |
|---|---|---|---|---|
| **LAB-00** | Independent four-file spec & scientific-scope review; reconcile OPEN-L1..L5 | Medium; distinct non-author GPT-6/Claude reasoning/repo reviewer | Review \`spec.md\`, project authority, #472/475/479 and Irish GROWFOR extraction | APPROVED / NEEDS CHANGES / REJECTED; no source coding before APPROVED |
| **LAB-01** | P0 local lane/technology preflight; no-overlap ownership | Low–medium; assigned local Sol/Codex High | Formal approved P0, actual issue claim and branch/HEAD/worktree, physical Ops/host/disk state | Written preflight + isolated new directory only |
| **LAB-02** | Pure C# schema, synthetic fixture generator, polygon, ID and BA/N/Dq exact mathematics | Medium; sole local core writer | LAB-01 and P0 approved scope | Unit tests, deterministic manifest, malformed-input negatives |
| **LAB-03** | Immutable mark/proposal/approval/pending/executed events and isolated lab save/replay | Medium–High; same sole writer | LAB-02 | Explicit unchanged-state until execution; de-dup/dead/stale tests, matching historical replay |
| **LAB-04** | Lightweight 2D map and local browser host bound to domain, not independent tree store | Medium; same source lane (or strictly non-overlap UI subowner after API contract accepted) | LAB-02/03; avoid shared simultaneous writes | Real browser click sequence/screenshots, status and accessible legend |
| **LAB-05** | P0 Tier 1 headless + Tier 2 actual browser qualification, negative controls, independent review | Medium; independent reviewer after writer stops | LAB-02–04 | Verified one-time felling, no-cut, stale work and save roundtrip, explicit P0-only verdict |
| **LAB-06** | Optional audited Sitka annual model comparator choice and source parity feasibility, NO engine code | High; independent source/science reviewer | P0 accepted; read exact current CCF implementation + GROWFOR extraction and constraints | Separate evidence report: model choice, deterministic fixtures, provenance, applicability limits, P1 APPROVE/HOLD |
| **LAB-07** | Conditional P1 growth/competition/mortality model and stand-level GROWFOR comparison | High; approved local core writer plus independent science/evidence review | LAB-06 model gate approved and own distinct implementation claim | Per-year tree ID ecology ledger, no-cut/thin scenarios, model version and errors/uncertainty; no L4 claim |
| **LAB-08** | Future measured Irish marteloscope / later 3D and other species | Out of scope | Actual licensed field IDs/XY/DBH/polygon and new specs and source ownership | No implementation in V1 |

## P0 evidence/acceptance checklist

- [ ] All LAB-00 product/technical boundaries approved; branch/claim/worktree real, non-overlapping.
- [ ] Same seed + fixture gives byte-identical sorted IDs/geometry/metrics.
- [ ] Tree ID, site area, adult DBH and state fields carry units + synthetic provenance.
- [ ] BA, N, Dq test values match independently verified hand-computable oracles, including dead/stump/nonrectangular polygon.
- [ ] No actual change from mark/propose/reject/approve/pending without explicit Execute Test Work.
- [ ] Explicit execute only once for valid live IDs, creates correct historical record, aggregates uniquely.
- [ ] Dead, stale, repeated, duplicate, invalid coordinate and malformed file cases fail closed.
- [ ] UI map and headless core display same IDs, statuses and statistics.
- [ ] Local-only lab save roundtrip maintains schema, snapshots and sourceClass Synthetic; no CCF save touch.
- [ ] Browser exercised in actual runtime; no fabricated screenshots or build/test PASS.
- [ ] P0 displays NO BIOLOGICAL GROWTH. Annual calendar/replay is not mislabelled forestry prediction.
- [ ] Independent review and GitHub handoff published; no claim of user acceptance, Irish accuracy, Scenario One integration, or Windows build.

## Conditional P1 acceptance checklist

- [ ] Current Sitka source/mechanics and D-048 source-version paths inspected; parity vectors published.
- [ ] Model name/version/unit, source and provenance documented; no copied unlicensed binaries or silent coefficient substitution.
- [ ] Year-by-year biological changes driven by changed living stand after executed work; no growth on proposal-only.
- [ ] DBH/height/competition and mortality measured separately, reproducible deterministic seeds or stated stochastic repetitions.
- [ ] Within-domain, stand-level comparisons with Irish GROWFOR/Broad–Lynch reported with error; no model-prediction parity rule invented.
- [ ] Individual-tree ecological predictive accuracy and future marteloscope validation remain UNKNOWN unless independent longitudinal field data received.

## Dispatch rules

Do not route a local implementation chat until **P0 spec APPROVED** and live lane ownership is confirmed. Then create a separate implementation claim and a paste-ready **direct GitHub packet URL**, including actual branch/worktree, exact allowed paths, provider, gates, output report path and owner action. The reference lab is parallel to—not a rewrite of—Scenario One.
