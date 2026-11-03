# #250 — 45° full-spine five-rack reference candidate: design handoff

**Issue:** [#250](https://github.com/jermur788/CCF/issues/250). **Date:** 2026-11-03. **Parent orchestrator:** DPC (DeepSeek). **Overall Manager:** #200.
**Status:** **[HANDED-OFF]** — design and measurement only. **No implementation, no rack creation, no felling, no save/schema change, no Windows build.**
**Candidate measured:** `integration/scenario-one-player-test-ready-v1` @ `72b4efbd1c653fa2231f632a79030a83ff76f833`, fresh Enlarged80 world (1344 living Sitka).
**Evidence root:** `/media/jer/Files/CCF/Verification/ScenarioOneFiveRackReference-20261103T2210Z/`

> **Read first.** This candidate is **not approved, not "sensible" in the §D-068.6 sense, and does not clear Rack V2 §39.6.1.** It is a single complete geometry proposal with measured consequences, and the recommendation is **STOP for implementation**. Appendix B records an independent review that returned **MATERIAL ERROR**, and one of those errors was **DPC's own summary**, not the worker's work — corrected here rather than quietly.

---

## 0. Decision summary

**Recommendation: STOP for implementation; retain this layout as the provisional reference geometry for the spec amendment.**

Three findings drive that, and none is about trigonometry:

1. **Production cannot represent the topology.** `ScenarioOneRackSampleGeometry.Derive`, run on the candidate's own five racks, returns **`Valid=false` — "Racks 4 and 5 cross or overlap across the common normal, so they have no defensible order."** The rack-sample/coverage model assumes five parallel racks with a common axis ordering. A 3+2 herringbone has no ordering in that projection. This invalidates candidate-specific teaching/sample/coverage derivation — it does **not** invalidate the geometry, the tree impacts or the raw mask memberships.
2. **The spine runs through the yard.** The straight northern continuation passes **0.035 m from the Forestry Workbench** at (−2.75, 11.0), which lies inside the continuation's z-range. A bypass or relocation decision is required, and none is designed here.
3. **No entrance is currently legal.** Real living entrance trees exist for all five racks, but production accepts **headland entrances only** (`RackEditing.EntranceOnHeadland`), so **none of the five is accepted by current code**.

Everything else — the 45° geometry, the 16–20 m spacing, the corridor counts — is measured and inside the design target. The blockers are model, yard and authoring-rule conflicts.

---

## 1. Spine (DERIVED from MEASURED planks)

| Item | Value |
|---|---|
| Definition | D-068 feeder access axis: **entire existing in-stand route + proposed northern continuation**. Not one of the five racks; **not proven cleared or machine-capable** |
| Existing route | south end (−2.436, −39.989) → route north end (−2.715, 10.750) = **53.020 m** |
| Continuation | (−2.715, 10.750) → (−2.715, 37.5) = **26.750 m**, due north |
| **Total** | **79.770 m** |
| Continuation corridor trees | **18** within 1.3 m (21 within 1.5 m) |
| Nearest trunk to continuation end | 0.5067 m (centre) |
| North Ridge collider south face | z = 39.5, i.e. **2.0 m** beyond the 37.5 window |

**Yard conflict (MEASURED positions, corrected distances):** Forestry Workbench (−2.75, 11.0) is **0.035 m** from the finite continuation — a real on-route conflict. Grinding Stone (−2.5, 9.4) lies **south of the continuation start** (z = 10.75), so the often-quoted 0.215 m is its distance to the *infinite axis*; distance to the **finite** continuation is **1.367 m**, and it is **not** a conflict. Buildable collider extents measure zero, so physical footprints are **UNMEASURED**.

---

## 2. The five racks (45° off geographic north, 2.4 m wide)

| id | side | junction (x, z) | far end (x, z) | usable length | same-side perpendicular gaps | entrance tree (DBH) | corridor trees |
|---|---|---|---|---|---|---|---|
| **R1** | E | (−2.436, −23.700) | (37.500, 16.236) | 56.48 m | R4: **17.855 m** | PO0920 (17.01 cm) | 24 |
| **R2** | W | (−2.436, −21.700) | (−35.400, 11.264) | 46.62 m | R3: **16.623 m** | P0008 (15.39 cm) | 20 |
| **R3** | W | (2.972, −3.600) | (−35.400, 34.772) | 54.27 m | R2: 16.623, R5: **17.510 m** | P0910 (15.57 cm) | 29 |
| **R4** | E | (0.163, 4.150) | (33.513, 37.500) | 47.16 m | R1: **17.855 m** | P1311 (16.47 cm) | 22 |
| **R5** | W | (−2.715, 26.850) | (−13.365, 37.500) | 15.06 m | R3: **17.510 m** | PO3418 (16.44 cm) | 8 |

*3 west (R2, R3, R5) + 2 east (R1, R4). Independent recomputation of the adjacencies gives 16.623103 / 17.509886 / 17.854760 m — all inside the 16–20 m band.*

**Formula:** `|Δx·cos α − s·Δz·sin α|`, s = +1 east / −1 west, valid for parallel 45° branches from junctions on **any** spine including a bowed one. At 45° it reduces to `|Δx − sΔz|/√2`. These are distances between **parallel infinite centrelines**, not clear gaps between corridor edges.

**Honest corrections to the rack table:**

- **R1 and R2 corridors overlap.** Their opposite-side minimum centreline distance is **1.414 m** against 2.4 m of combined half-width, so the two corridors intersect near their junctions (R2's junction is only 2.0 m from R1's along z). No centreline crossing, but the corridors are not clear of each other.
- **Boundary:** all *centrelines* stay within ±37.5 m, but full-width corridor corners reach **38.349 m** — inside the ±40 m stand, outside the ±37.5 m window.
- **The `far_end_inside_snap_window=true` flag is misleading for R2/R3.** Their endpoints are **4.6 m** from the west boundary and do **not** qualify for 2.5 m snapping; the flag reflects being inside the window, not snap eligibility.
- **R1's far end is inside a trunk collider:** 0.073 m from PO2839's centre, collider-face distance **−0.0076 m**.
- **R5 is only 15.06 m long** — the shortest useful rack; R1 and R3 at 54–56 m imply long felling programmes.
- Two west racks terminate **on** the 3D-only headland strip (R2, R3 at x = −35.4). Whether that is a valid termination is **undefined** in the current spec.

---

## 3. Corridor impacts — proposals, not felling

**103 distinct rack-corridor trees** (24 + 20 + 29 + 22 + 8) plus **18 spine-continuation trees**, with exactly one shared tree (PO3518): **120 union** (103 + 18 − 1 = 120, verified independently). Species and DBH per tree are in `corridor_trees.md` and `design_candidate.json`.

These are **impact candidates with consequences**. They are **not** Fell marks, **not** authorised felling, and create **no** Work Plan orders.

---

## 4. MFM impact of the complete candidate

> **Correction notice.** DPC's first summary of these figures mislabelled the `Headland` row as `Track` and then reported the Headland *stem* delta as a *newly-masked tree* count. The table below is taken directly from `mfm_candidate.json` and was verified row by row after independent review returned MATERIAL ERROR. **The worker's own `RESULTS.md` table was correct; the misquote was DPC's.**

Baseline: 116 masks (104 Track + 1 Clearing + 7 Site + 4 Headland); 994 treatable stems; 4141.99 m²; 12/14 covered (0.857).

| Scenario | Semantics | Newly masked living | Treatable stems | Treatable area |
|---|---|---|---|---|
| C1 spine continuation only | Track | 21 | 977 (−17) | 4080.90 (−61.09 m²) |
| C1 | Headland | 20 | 978 (−16) | 4082.31 (−59.68 m²) |
| C2 five racks only | Track | 138 | 897 (−97) | 3734.96 (−407.03 m²) |
| C2 | Headland | 133 | 901 (−93) | 3744.26 (−397.73 m²) |
| **C3 spine + five racks** | **Track** | **156** | **883 (−111)** | **3682.17 (−459.82 m²)** |
| **C3** | **Headland** | **150** | **888 (−106)** | **3693.96 (−448.03 m²)** |

- **Newly masked trees and lost treatable stems are different quantities** — 156 masked versus 111 lost — because only some masked trees were inside covered areas.
- Per-rack newly masked (Track / Headland): R1 34/32, R2 25/24, R3 33/32, R4 36/35, R5 10/10.
- Covered count and fraction stay 12/14 and no second sample becomes required, but the **coverage signature changes in every scenario**.
- `Track` versus `Headland` semantics differ by ~6 trees and ~11 m² — relabelling does **not** make the proposal coverage-neutral.
- **These are production-model outputs under hypothetical suggested-rack geometry**, not measured consequences of an operational herringbone, and the **12/14 / 994 baseline cannot be transferred to the candidate's own coverage model** because `Derive` rejects it (§0.1).

### Proposal-overlay semantics — SPECIFIED ONLY, not implemented

A proposed, unbuilt spine/rack must be a presentation-only layer, structurally separate from `MapPath` (whose `Dirt Path` names feed Track masks) and from `MaskKind`:

1. **Data:** a `ProposedRoute` record (id, polyline, width, kind `spine|rack`, state `Proposed`) held in the draft rack plan or UI session — never as `Dirt Path` planks, never as a mask.
2. **MFM reads zero masks from it**, asserted by a test that the coverage signature is identical with and without proposals.
3. **Stand Map and minimap** draw it dashed/hatched with a distinct legend entry, the same pen on both surfaces, never the solid path pen and never the Confirmed/Marked rack symbols.
4. **3D:** no ground decal; at most the entrance cue.
5. **Must not imply** cleared ground, approved work or a fell mark: no Pending orders, no corridor-tree fell highlights, no treatable-area change. Tree counts appear as "N trees would need removal if built".

State progression: **Proposed → Confirmed plan → Entrance marked → Work Plan approved → Cleared/constructed**. Only the last may create masks, path planks or change cover.

---

## 5. Map / forest consistency and renders

- **MEASURED (production map model, fresh world):** 2 path polylines (both ~2.6 m, 52 points), **0 racks**, 5 dashed E–W guidance suggestions, Clearing landmark. The **headland/feeder are absent from the map model**, confirming the existing 3D-versus-map inconsistency. The candidate cannot appear on either surface under current data plumbing — that means its source/plumbing must change for the proposed semantics, **not** that renderers cannot draw diagonal lines.
- **Renders: UNMEASURED.** Run B reached the startup screen on a private Xvnc display and was then **SIGKILLed by the cgroup (exit −9) after 163.8 s** under `MemoryMax=8G`; no image was produced. **The OOM cause is likely but not proven from the retained evidence** — SIGKILL alone does not establish it, and kernel/cgroup receipts were not captured. Bounded next step: rerun `driver/run_measure.py runB 1500 render` (harness already written: overhead plan, oblique, junction eye-level, HUD minimap) with a ≥12 GB memory cap on a machine with ≥12 GB free, or on the real GPU display with the user's agreement.
- **Delivered instead:** `plan_view_candidate.svg` — measured plan view with rack ids, junctions, entrance trees and spacing.

---

## 6. D-064 / D-068 conflicts requiring amendment

| # | Conflict | Evidence | Needed amendment |
|---|---|---|---|
| 1 | **Entrance on spine** | Real entrance trees exist for all five, but production `EntranceOnHeadland` accepts headland only; `ResolveCreation` silently re-anchors, contrary to §16 | §12/§16/§23 + `RackEditing` to allow spine-junction anchors and define entrance-tree reach |
| 2 | **Five-count / spacing** | Achieved 16.62–17.85 m for 3W+2E; a 3-east split is impossible | Spec must fix the spine definition and permit mixed-orientation racks |
| 3 | **Headland termination** | R2/R3 end on the 3D-only strip; validity undefined | Define termination rule and its map depiction |
| 4 | **Rack-sample / coverage model** | `Derive` returns `Valid=false` for the candidate's own racks | New sample-geometry spec able to represent non-parallel racks |
| 5 | **Corridor overlap** | R1/R2 corridors intersect (1.414 m centreline separation vs 2.4 m combined half-width) | Overlap rule, or re-place R2 |
| 6 | **Useful lengths** | 15.06–56.48 m | Length policy |
| 7 | **Northern turning room** | Spine end 0.507 m to a trunk centre; R1 far end inside a trunk collider; no machine footprint exists in the repo | Bypass/relocation + a defined machine envelope |
| 8 | **Yard** | Workbench 0.035 m from the finite continuation | Bypass or relocation decision |
| 9 | **Removals** | 120 union trees | Forestry/ecology approval via Work Plan |
| 10 | **Frozen state** | Legacy40 unaffected by design (Enlarged80-only; not run). Reference Future v1 / existing saves: PO entrance ids and `Dirt Path` Track masks are inputs; replacing them **not tested** | Compatibility gates |

Rack V2, the save schema and the DRAFT spine spec's status were **not** touched.

---

## 7. What this does NOT establish

- **Not "sensible"** in the §D-068.6 sense: renders, on-foot entrance marking, turning room and old-save behaviour are **UNMEASURED**.
- **No §39.6.1 clearance.** Geometry and counts are not an approved rack network.
- **No operational coverage.** The production model rejects the topology; the MFM tables are sensitivity experiments under hypothetical geometry.
- **No felling authorisation and no Work Plan orders.**
- The suggested E–W layout remains on the minimap as guidance and would contradict this candidate unless replaced — not addressed.
- The measured candidate is **not proven** to be the user's playtest build.

---

## 8. Evidence inventory

Root `/media/jer/Files/CCF/Verification/ScenarioOneFiveRackReference-20261103T2210Z/` — `README.md` carries commands, disk numbers and sha256 of every harness and results file.

| File | Content |
|---|---|
| `RESULTS.md` | worker's full findings; its MFM table is the correct one |
| `corridor_trees.md` | per-rack corridor tree lists with species/DBH |
| `plan_view_candidate.svg` | measured annotated plan view |
| `measurements/design_candidate.json` | spine, five racks, junctions, gaps, entrances, boundary |
| `measurements/design_bounds.json` | clearance-only vs entrance-reach bounds |
| `measurements/mfm_candidate.json` | four scenarios × two semantics, per-area deltas |
| `measurements/map_model.json` | production map model |
| `measurements/rack_spine_overlap.json` | rack/spine overlap arithmetic |
| `measurements/run-runA.json` / `run-runB-render-OOM.json` | run receipts |
| `harness/FiveRackRef.cs`, `analysis/`, `driver/`, `logs/` | harness, design scripts, driver, raw logs |
| `preflight/`, `side-effects/`, `logs/lock-*.json` | preflight, Editor side-effect patches, lease receipts |

---

# Appendix A — V2 Orchestration Decision Appendix

Live ledger: [`ORCHESTRATION-LEDGER.md`](ORCHESTRATION-LEDGER.md). #250 events: preflight/lane-reuse/lease/dispatch; run A PASS + run B render SIGKILL + candidate results; review reconciliation.

## A.1 Routing

| Phase | Pool / model | Effort | Role | Why |
|---|---|---|---|---|
| Design + measurement | `claudeAgent` → `claude-sonnet-5-5` | `medium` | design | Two prior successful phases on this exact project and lane; capacity **observed**, not assumed. |
| Independent review | `codex` → `gpt-6.1-sol` | `reasoningEffort: medium` | review | Other provider; 15 fixed claims; adversarial on arithmetic. |
| Orchestration | DPC (`opencode-go` → `deepseek-v4.1-flash`) | inherited | routing, verification, publication | Not a measurement worker. |

**Exploit/explore:** EXPLOIT of observed capacity and of cross-provider independence. No switch, retry or salvage.

## A.2 Capacity

- **OBSERVED:** both pools enabled; **no cooldown, rate-limit or login failure** at any point.
- **UNKNOWN:** quota, reset window, remaining credit, per-child tokens, model-call counts, context headroom. No telemetry exposed; **not estimated**.

## A.3 Failures, classified

| Event | Classification | Action |
|---|---|---|
| run A first launch FAIL (code 1, 33.9 s) | ordinary implementation iteration | harness fixed, re-run PASS; disclosed, not hidden |
| run B render SIGKILL (code −9, 163.8 s) | **environment/memory limitation** | not retried; bounded next step recorded; cause not overclaimed |
| C9 MFM misquote in DPC's summary | **orchestrator transcription error** | corrected from raw JSON; disclosed in §4 and Appendix B |
| C7 infinite-axis vs finite-route distance | **worker metric misapplication** | corrected in §1 |
| worker turn ended mid-run twice with stale "still waiting" messages | harness/process behaviour, not reasoning or quota | DPC checked state directly and resumed |

## A.4 Milestones

**MEASURED** — yes, bounded. **REVIEWED** — yes, and it changed the result. **IMPLEMENTED / INTEGRATED / VERIFIED / ACCEPTED** — none, none claimed.

---

# Appendix B — Independent review and corrections

**Reviewer:** `codex` → `gpt-6.1-sol`, `reasoningEffort: medium`, read-only; no Unity, no lease interaction, no files written. All README hashes matched.

## B.1 Verdict

**MATERIAL ERROR.** Over 15 fixed claims: **7 PASS, 6 PASS-WITH-CORRECTION, 2 FAIL** (7 + 6 + 2 = 15).

## B.2 The two FAILs

| Claim | Finding | Effect |
|---|---|---|
| **C9 — MFM figures** | The `Track` and `Headland` rows were conflated and a *stem delta* was reported as a *newly-masked tree* count. Correct: **Track 156 masked / −111 stems / −459.82 m²**; **Headland 150 / −106 / −448.03 m²**. | **DPC's error, not the worker's** — `RESULTS.md` had the correct table. Corrected from raw JSON in §4. |
| **C7 — yard distance** | Grinding Stone (z = 9.4) lies **south of the continuation start** (z = 10.75); 0.215 m is the distance to the *infinite axis*, and the finite-route distance is **1.367 m**. | The yard conflict survives for the Workbench (0.035 m) but not for the Grinding Stone. Corrected in §1. |

## B.3 The six corrections accepted

1. **C4** — corridors overlap (R1/R2, 1.414 m centreline separation); full-width corners reach 38.349 m, outside the ±37.5 window though inside the ±40 stand; the R2/R3 snap flag is misleading (4.6 m from the boundary, not snap-eligible).
2. **C8** — R1's endpoint sits **inside** PO2839's collider (−0.0076 m face distance); these are derived centre distances, not turning clearance; "no machine footprint exists in the repo" was not exhaustively established.
3. **C10** — the `Derive` rejection is about **overlapping projections across a common normal**, not physical crossing. It invalidates candidate-specific derivation, not the geometry, tree impacts or raw mask memberships; and it does not prove every possible herringbone must fail. `maxAxisDeviationDegrees=0` is an early-return default, not evidence of parallelism.
4. **C11** — retained evidence establishes a cgroup SIGKILL, **not** the OOM cause; the RSS figures and machine capacity need kernel/cgroup receipts.
5. **C12** — "cannot appear without a source change" means its **data/plumbing** must change for the new semantics, not that renderers cannot draw diagonals.
6. **C15** — preflight alone does not prove final cleanliness; and "lease untouched" needs **actor attribution** — the driver performs no lease operation, but the lease *was released*, by DPC.

## B.4 Review position on the STOP

The reviewer's judgement, accepted: **STOP is proportionate for implementation and operational endorsement**, and **excessive if it also stopped specification investigation**. The geometry and impact evidence support continued reference review conditional on the corrections above.

## B.5 Retrospective

Two consecutive claims in this programme produced a **material error caught only by cross-provider review** — #248 by the worker (formula assumption), #250 partly by **DPC** (transcription). The common shape is a number carried from one context into another without re-deriving it at the destination. The working rule adopted: **any figure published from a harness or a reviewer must be re-extracted from the raw artefact at the point of publication and reconciled against its total**, the same discipline already applied to sha256 hashes.
