# MULTI Phase 2 — Sessile Oak and European Beech: closure handoff

**Written:** 2026-11-24 08:05 UTC
**Canonical status:** `PHASE 2 CLOSED (bounded scope) — IMPLEMENTED, INDEPENDENTLY REVIEWED APPROVED, DELTA-VERIFIED, OWNER DECISIONS APPLIED`
**Not integrated. No PR. Nothing merged to `main`.**

This record closes the task. It supersedes the status line in `PHASE2-STEPB-HANDOFF-20261124.md`; that document remains the detailed evidence record and is not rewritten.

---

## 1. Final refs

| Ref | SHA | Role |
|---|---|---|
| `feat/multi-phase2-oak-beech-20261124-v1` | `8894ab866706a500a5fa5ce5d7c6e6e14a1998fc` | implementation + tests + handoffs |
| `docs/multi-phase2-oak-beech-spec-20261124-v1` | `0bb49dfe3aa714407043a7a9820719a584296b7f` | APPROVED quartet + follow-up amendment |
| `review/multi-phase2-oak-beech-20261124-v1` | `4c74533e03ac8a586f64b5dbe4c671e7ffd92662` | P2-11 independent review (APPROVED) |
| `review/multi-phase2-oak-beech-delta-20261124-v1` | `35eff93c76efdce1e40daeeb3a2b8522885ade98` | delta verification (PASS, m18 killed) |
| tag `docs/multi-phase2-oak-beech-spec-draft-20261124` | `3a1eca6c6d5aaf2b69230376deeb4f6c3fd15794` | the DRAFT artifact the owner approved |
| Base — Stage 5 docs tip | `d1db81b59248c13b4f9682ecdf68e37759b15c65` | |
| Reviewed code head | `401dab1835128f70d1ecc28f54ae5db8ac68bf69` | P2-11 reviewed this exact tree |

Links:
- Step B handoff: https://github.com/jermur788/CCF/blob/feat/multi-phase2-oak-beech-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/PHASE2-STEPB-HANDOFF-20261124.md
- P2-11 review: https://github.com/jermur788/CCF/blob/review/multi-phase2-oak-beech-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/MULTI-PHASE2-OAK-BEECH-INDEPENDENT-REVIEW-20261124.md
- Delta verification: https://github.com/jermur788/CCF/blob/review/multi-phase2-oak-beech-delta-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/MULTI-PHASE2-OAK-BEECH-DELTA-VERIFICATION-20261124.md
- Specification: https://github.com/jermur788/CCF/blob/docs/multi-phase2-oak-beech-spec-20261124-v1/specs/multi-phase2-oak-beech-v1/spec.md

## 2. What was delivered

`SpeciesModelVersion { Off = 0, ThreeSpeciesMixed = 1 }` as a fifth model switch; full Phase 2 at `1/1/1/1/1`. Sessile Oak and European Beech adult growth with C2 crowns (Oak linear, Beech power-law); two-part mixed density per D-061 with pure-Sitka Comeau parity; shared mixed natural capacity and the 1-total `5 m` block cap; `PL-*` promotion exempt from that cap; independent Oak/Beech mast streams and cross-rack seed rain; fenced `Plant` / `Shelter` / `SpotClear` with the `~1 m²` selective circular patch; species-aware broadleaf pruning; deadwood records carrying `SpeciesId`. Test-only `MA-1-MIXED-v1` fixture.

**Scope kept:** zero Unity `Assets/` edits, zero `/multi` UI edits, zero save-schema edits, no protected ports, no Phase 3 economics, no other Scenario Two species.

## 3. Evidence

| Gate | Result |
|---|---|
| Clean build, `--no-incremental`, `--disable-build-servers`, external `TMPDIR` | **0 warnings / 0 errors** (three independent reproductions) |
| Full Core suite | **245 / 245** — 209 pre-existing + 33 Phase 2 + 3 follow-up |
| Suite runs | twice by implementer, twice by coordinator, twice by reviewer — identical PASS sets |
| Production change since the reviewed head `401dab18` | **NONE** (nothing under `src/`) |
| Frozen anchors + Q6 | all reproduced and unchanged at `8894ab86` |

Anchors: single-area Model 0 `7e3a2a86…`, `0/0/0/0` `a692a6d5…`, `0/0/1/0` `814e8165…`, `1/1/1/0` `8a926776…`, `1/1/1/1` `c4b670ed…`, P0 pin `9a99f781…`, Q6 `776a68df…` (equal both sides), `MA-1-MIXED` `93e9abda…`.

## 4. Review chain

| Stage | Verdict | Head |
|---|---|---|
| P2-11 independent review | **APPROVED**, G2 PASS, G3 PASS, no blocking findings | `4c74533e` on `401dab18` |
| Delta verification | **PASS**, m18 **KILLED** | `35eff93c` on `8894ab86` |

The delta check established that `P2-06b` is the **only** test that fails under m18, and that it fails for the intended reason (zero vegetation-attributable deaths), not by crash. It also showed `P2-04b` fails when the sliver refusal is disabled, and `P2-07b` fails under three separate planted-promotion breaks — including a cell-centre snap that no older test can see, because the older tests plant at cell centres.

## 5. Owner decisions applied

| # | Decision | Applied |
|---|---|---|
| **F1** | Add paired survival test; reviewer confirms m18 killed | `P2-06b` added; 76.0 % unweeded (42 vegetation deaths) vs 88.5 % spot-cleared (16); **m18 confirmed killed** |
| **F2** | Ratify `Phase2PointWithoutGroundCell`; spec amendment + unit test | Amendment record on docs branch; `P2-04b` added and mutation-sensitive |
| **F8/F9** | Keep spec and feature branches separate; no merge, no PR | Honoured — three branches, none merged |
| **F10** | Acknowledge Oak's low recruitment | Recorded in the Step B handoff; no constant changed |
| soft spot #2 | End-to-end plant → promote | `P2-07b`: `PJ0001` → `PL-PJ0001`, year 9, height 3.750 m, cell light 0.683, block credit equal to an unplanted twin every year |

## 6. Residual limitations (deliberate, disclosed, none blocking)

1. **Six of the original mutation survivors remain unasserted** — m10, m21, m22, m29, m30, m31. Each still passes 245/245, and each moves the 30-year fingerprint, so none is a no-op; they are simply not covered by an assertion. Closing them would mean adding tests for individual constants, which the owner has not asked for.
2. **m27 was not re-run** in the delta check — its site could not be reconstructed, and the new tests cannot observe it.
3. **`P2-06b` is qualified only at default browse pressure, a 6-year horizon and its 200-seed set.** Not swept across other pressures or horizons.
4. **The 76 % / 88.5 % gap is not the reviewer's 66 % / 95 %.** Different setup (browse left on, 6-year window, patch regrows from year 2). Mechanism and direction agree; magnitude does not.
5. **Nothing was run in Unity.** No `Assets/` behaviour, `/multi` UI, browser, save/load or rendered verification. Unity source was read only, to confirm constants and rule semantics.
6. **Determinism is single-host.** No CI or second-machine run.
7. **Oak is a rare recruit and promoter** under the ratified light tables (Oak growth 0.70 at light 0.35 vs Beech 1.00). Expected, documented, not a defect.

## 7. Outstanding administrative item

The three new checklist rows (`P2-04b`, `P2-06b`, `P2-07b`) were added **unticked**, each noting "Observed PASS at `8894ab86`, pending independent review / m18 verification". That review has now passed. The checklist states its owner is the **Overall Manager** and that marks are made only from observed evidence — so ticking them, and any status word on the quartet itself, is the owner's/Manager's act, not the implementer's and not the coordinator's.

## 8. Topology

| Branch | Carries | Head |
|---|---|---|
| `docs/multi-phase2-oak-beech-spec-20261124-v1` | APPROVED spec + amendment | `0bb49dfe` |
| `feat/multi-phase2-oak-beech-20261124-v1` | engine, tests, handoffs | `8894ab86` |
| `review/…-20261124-v1` | P2-11 review | `4c74533e` |
| `review/…-delta-20261124-v1` | delta verification | `35eff93c` |

The feature branch contains no copy of the spec. The reviewer trial-merged spec and feature branches read-only and reports they merge cleanly. All four remain unmerged, stacked on the unmerged Stage 5 lineage, per the owner's C6 decision.

## 9. Status

`PHASE 2 CLOSED (bounded scope) — §7 checklist ticking remains with the owner/Overall Manager. Not integrated, not merged, no PR.`
