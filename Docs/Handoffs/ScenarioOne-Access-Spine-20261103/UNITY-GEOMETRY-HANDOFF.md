# #248 — Access-spine geometry and MFM mask measurement: measured handoff

**Issue:** [#248](https://github.com/jermur788/CCF/issues/248). **Date:** 2026-11-03. **Parent orchestrator:** DPC (DeepSeek). **Overall Manager:** #200 (unchanged).
**Status:** **[HANDED-OFF]** — measured, independently reviewed, and **corrected after review found a material error** (Appendix B). **No verdict is given and no production change was made.**
**Candidate measured:** `integration/scenario-one-player-test-ready-v1` @ `72b4efbd1c653fa2231f632a79030a83ff76f833`, fresh Enlarged80 world, 1344 living Sitka, isolated config, user save untouched.
**Evidence root:** `/media/jer/Files/CCF/Verification/ScenarioOneAccessSpineMeasure-20261103T2110Z/`

> **Read this first.** The first version of the spacing table in this study was **wrong** and is superseded. It used `Δz·sin α` for branch spacing, which is only valid on a straight north–south spine; the full-spine junctions lie on a **bowed** route. Independent review caught it. Everything below uses the corrected distance `|Δx·cos α − s·Δz·sin α|`. The original JSON files are retained unmodified as the record of what was run; see `CORRECTIONS.md` and Appendix B.

---

## 0. What a Manager can decide from this

### 0.1 The decisive result — five racks at ≥16 m perpendicular spacing

Best achievable **3+2** same-side perpendicular spacing, corrected (`measurements/b7_angle_sweep_corrected.json`; reviewer's independent search in brackets):

| α off north | **NORTH_ONLY** — new northern extension only | **FULL_SPINE** — along the existing in-stand route too | published (wrong) |
|---|---|---|---|
| 20° | 3.147 m | 6.293 m *(6.293)* | 8.65 m |
| 30° | 4.690 m | 11.040 m *(11.032)* | 13.65 m |
| 35° | 5.449 m | 13.606 m *(13.587)* | 15.97 m |
| 40° | 6.190 m | **16.016 m** *(16.009)* — **16 mm margin** | 18.06 m |
| 45° | 6.916 m | **18.294 m** *(18.274)* | 20.08 m |
| 50° | 7.614 m | **20.407 m** *(20.407)* | 21.91 m |
| 60° | 8.911 m | **24.076 m** *(24.057)* | 25.07 m |

- **Extension-only spine: infeasible at every angle.** Maximum 8.911 m at 60° — about half the lower bound of the 16–20 m band. This is not a near miss, and it does **not** depend on the corrected formula: with Δx = 0 above the route end, `Δz·sin α` was already exact there. The independent bound is decisive — two 16 m gaps need `2 × 16 / sin α ≤ L`, so **no angle can beat 32 m of spine**, and the zone supplies ~24.3 m.
- **Full-spine interpretation: technically feasible at 40°, robust from 45°.** At 40° the margin is **16 mm** — any additional constraint (tree-specific end clearance, a rack-length rule, turning room, measuring α from the route tangent instead of due north) could remove it. At 45° the margin is ~2.3 m and at 50° ~4.4 m.
- **"≥ 40°" means the sampled angles only** (20, 30, 35, 40, 45, 50, 60), not a proven continuous threshold. 35° reaches only 13.606 m.
- **No rack count was reduced and no metres were warped.** 3+2 is the best split; 4+1 and 5+0 are strictly worse at every angle.

### 0.2 The question this exposes

Neither the accepted Rack V2 specification nor the DRAFT access-spine specification **defines what "the spine" is**. If it is a new northern continuation only, five racks cannot reach the 16–20 m band at any branch angle. If it is the existing in-stand route plus the extension, they can — narrowly at 40°, comfortably from 45°.

**That single unresolved definition, not trigonometry, decides whether V2 §39.6.1 can be satisfied.** The two columns must not be collapsed, averaged, or presented as one recommendation. This is the first thing a Manager needs to settle.

### 0.3 Three findings that change the cost picture

1. **The existing suggested layout is already marginally non-compliant.** Perpendicular gaps are 18.849 / 16.660 / **15.555** / 19.439 m — one is below the 16 m band before any redesign.
2. **A herringbone is a felling programme, not decoration.** Branches carry **3–38 standing living trees** inside each 2.4 m corridor (centre-in-corridor count, not trunk-surface intersection), and a due-north spine passes within 1.5 m of **22 standing trees**. A1 as "just add path planks" collides with standing timber.
3. **The north spine end has almost no turning room.** It is 2.5 m from z = +40 but only **0.507 m from the nearest trunk centre (PO3919)** — and that is a centre distance, not available clearance — with the invisible North Ridge collider's south face at z = 39.5.

### 0.4 A1 is confirmed not visual-only (measured, not argued)

Every hypothetical spine or branch changes the **coverage** signature. Adding **only** a due-north spine classified as `Track` costs **18 treatable stems and 65.80 m²** of treatable area; spine plus one branch costs **31 stems and 125.96 m²**. Covered count stays 12/14 in all cases, so this is a silent change to treatable ground and prescription input — **with no new save field**, exactly as #246 warned. `Track` versus `Headland` labelling differs by about one tree and 2 m², so **relabelling does not rescue A1**.

---

## 1. Method, and what counts as evidence

| Item | Value |
|---|---|
| Lane | `/media/jer/ZX20/CCF-AccessSpine-Measure-20261103`, branch `verify/scenario-one-access-spine-measure-20261103` |
| Lane state | clean at `72b4efbd` before and after; no harness `.cs`/`.meta`, no `Temp`/`obj`/`Logs` left |
| Unity | 6000.6.0f1, one disposable harness, `-batchmode -nographics`, `systemd-run` scoped, isolated `XDG_CONFIG_HOME` |
| Lease | `62da5753307c4bb886477efc14b4ed96`, owner `DPC-248`, acquired 21:13:11Z **before** any measurement, released normally 21:44:43Z (`forced: false`); not re-acquired |
| Run | 677 s, `ASM_MEASURE_PASS`; free disk 27,183,808,512 → 20,144,848,896 bytes; disk guard never fired |
| Evidence labels | **MEASURED** (observed in the running scene), **DERIVED** (arithmetic/geometry on measured values), **UNMEASURED** (not obtained) |

Coordinate frame: Unity world XZ metres; stand 80 × 80 centred at origin; **+z is north**, z = +40 north edge.

**Side effect, captured and reverted:** opening the Editor rewrote `.vscode/settings.json` (the known Unity side effect — a trailing comma and `dotnet.defaultSolution` renamed to the lane's `.slnx`). The patch is preserved at `side-effects/settings-patch.diff`; the file was restored and the lane verified clean.

---

## 2. Route and surface geometry (MEASURED)

- **`Dirt Path*` planks: 104 active, 2.600 m wide, y = 0.006, no collider on any of them.** 52 authored (`Dirt Path 00…51`, z −14.750 → 10.750, polyline 27.520 m) and 52 appended at runtime (`Dirt Path X000…X051`, z −14.750 → −39.989, polyline 25.500 m). Combined in-stand route **z −39.989 → 10.750**.
- **Map rule confirmed in the running scene:** planks sort by ordinal name, a jump of >2 m starts a new line, and each line takes its width from its first plank. That yields **two** polylines, both 2.6 m wide. The `Dirt Path X` prefix is why the runtime approach segment reaches both the Stand Map and the `Track` mask for free.
- **`Approach Track*` planks (z < −40) are neither mapped nor masked**, because they do not carry the `Dirt Path` prefix. A live inconsistency, not a design.
- **Route north end: (−2.715, 10.750); 29.250 m to z = +40.** The playable north limit is the invisible `North Ridge` BoxCollider, south face **z = 39.5**.
- **Clearing:** (−2.772, 11.000). The **code radius is 2.200 m**; the 9 × 9 visual scale (4.5 m) is cosmetic. Anything reasoning from the visual size is wrong.
- **Headland is presentation-only:** one renderer (204 vertices), **zero colliders**, and no map code reads it. It is **not** absent from production clearance or MFM geometry — it is authoritative there and invisible on the Stand Map and minimap. Strip x = −35.400, z −37.700 → 37.700, 2.6 m wide; feeder z = −27.400 from x = −35.400 to −2.436; pads r 2.2.
- Other route-like ground: 104 plank clearance circles (r 1.5) and 9 circles (r 2.2) covering the clearing, the player start and 7 buildables. Whether any *reads* as a road on screen is **UNMEASURED** — no render was taken.

---

## 3. Five-rack feasibility (DERIVED from MEASURED trunks, route and obstructions)

**Constraints applied:** junctions on the measured route (and the straight extension above it), every branch inside |x|, z ≤ 37.5 (the 2.5 m `RackEditing.BoundarySnapM` window), ≥ 5 m long (`MinLengthM`), 2.4 m corridor (default rack width), exact segment-to-circle clearance against the clearing circles and feeder pads **including the junction itself**. Junction resolution 0.01 m in the corrected run. α is measured from **due north**, not the local route tangent.

- **Zone boundaries are worker assumptions, not spec text.** NORTH_ONLY starts at the clearing's code edge (z = 13.2). FULL_SPINE's southern bound is **z ≈ −23.85**, set by the feeder pad (centre −27.4, mask radius 2.35, plus 1.2 m rack half-width) — not by the stand edge.
- **Corrected 16 m candidates.** At 40° the **west-three** arrangement at junction z −23.85 / −5.35 / 26.30 gives consecutive perpendicular gaps **16.009 / 16.013 m** (reviewer) and 16.002 / 16.001 m (worker's own search). **The published east-three candidate fails**: its gaps are 19.454 / 12.770 m, and its first junction sits inside the feeder-pad exclusion. It fails similarly at 45° (19.767 / 12.462), 50° (19.637 / 12.562) and 60° (19.925 / 13.340).
- **Headland interaction:** westward FULL_SPINE branches reach x ≤ −34.1 from 30° upward, i.e. they **meet** the headland strip rather than avoid it. A junction, not automatically a conflict, but a design decision about where branches terminate.
- **Removals implied:** 3–38 standing trees per branch corridor. A due-north spine masks 22 standing trees within 1.5 m.
- **Entrance trees:** the `PO`-id entrance mechanism is headland-only in code, so an on-spine entrance has no assigned target tree; the nearest living trunk to each junction and branch end is recorded per candidate in `b8_candidate_layouts_corrected.json`.
- **Turning room (MEASURED positions, DERIVED distances):** spine end 2.5 m to z = +40, **2.0 m to the ridge face**, **0.507 m to PO3919's centre** (PO3918 at 1.223 m). Feeder/path pad (−2.436, −27.400): nearest trunk centres 2.426 / 2.529 m. Headland/feeder pad (−35.400, −27.400): 2.462 m. Headland mid: 1.808 m.
- **Real lattice (MEASURED coordinates, DERIVED statistics):** serialised 2.0 m (code default 1.9); measured mean steps 1.963 m (z) / 1.964 m (x); nearest-neighbour distance min 0.968 m, mean 1.619 m; jitter sd 0.244 / 0.249 m (residual-from-row/column-mean estimator; the worker's alternative estimator gives 0.2426 / 0.2474 — the estimator and population must be quoted with the number). 41 × 41 = 1344 trunks within ±39.4 m.

---

## 4. Physical traversal (MEASURED, with a stated method limit)

- **On-path 4.000 m/s; off-path 3.97–4.00 m/s.** No leg stalled.
- Off-path from the route end straight north: 27.74 m to **z = 37.972**, 6.94 s, max lateral offset 0.074 m. First trunk contact at t = 2.62 s with **PO3119 — deflected, not blocked**.
- Diagonally to the headland at PO3803's z: 38.19 m, 9.62 s, 19 contact frames, max offset 0.182 m, worst 1 s window 3.843 m/s — deflected, not blocked.
- **Trunk proxy:** one `CapsuleCollider` per trunk, world radius **0.048–0.111 m** (mean 0.077). Minimum trunk-surface gap between neighbours **0.797 m**; **3989 free / 106 blocked of 4095** neighbour-pair midpoint tests. A 0.3 m player capsule fits between adjacent pairs; clustered third trunks can still block a specific line. This does **not** prove every walking trajectory is passable.
- **Method limit:** the harness steered the player's real `CharacterController` with `ForestPlayer`'s own formula, because `ForestPlayer.Update` movement is gated on window focus and a locked cursor, which batchmode does not have. **Collisions are real; input is not human.** The `L1` leg is a **no-op** — the spawn already lies on the route (distance 0) — and is labelled as such rather than reported as a result.

---

## 5. MFM mask delta (MEASURED production world inputs; hypothetical rack geometry)

**These are computed outputs from production world inputs plus hypothetical suggested-rack geometry — not an operational prescription baseline.**

- **Baseline:** 116 masks = **104 Track + 1 Clearing + 7 Site + 4 Headland**; sample 75 stems / 314.005 m² (2388.5 stems/ha); **12/14 covered (0.857)**; second sample not required; treatable **994 stems** and **4141.99 m²**.

| Scenario | Semantics | Newly masked living | Treatable stems | Treatable area |
|---|---|---|---|---|
| S1 spine due north (58 planks) | Track | 22 | 976 (**−18**) | 4076.19 (**−65.80 m²**) |
| S1 | Headland | 21 | 977 (−17) | 4078.36 (−63.63 m²) |
| S2 spine along trend (64 planks) | Track | 23 | 974 (−20) | 4071.61 (−70.38 m²) |
| S3 one NE-45° branch at z=20 (49 planks) | Track | 17 | 979 (−15) | 4073.39 (−68.60 m²) |
| **S4 = S1 + S3** | Track | 37 | 963 (**−31**) | 4016.03 (**−125.96 m²**) |
| S4 | Headland | 36 | 964 (−30) | 4019.65 (−122.34 m²) |

- **Every scenario changes the coverage signature**; covered count and fraction stay 12/14 and no second sample becomes required. The change is silent in the headline number and real in the treatable ground. **The prescription derivation signature was not recomputed per scenario** — only one baseline `derivationSignature` exists.
- **`Track` vs `Headland` semantics differ by about one tree and 2 m²** in every scenario, because mask *membership* drives exclusion and the kind tag is descriptive. **The semantic choice does not rescue A1.**
- S1's affected 5 m cells are all in column 7 (x −5…0), matching the spine's x ≈ −2.7. Lost target trees concentrate in R4C (77→68 stems, 281.7→252.1 m²) and R5C (68→59, 290.1→253.8 m²). Per-scenario cell/id lists are in `mfm.json`.
- **No persistent gameplay write-back was found:** no save, prescription, mark, order, rack or plank mutation, and none is present in the inspected production paths. The harness *does* create a GameObject, enter Play mode, change timing/background settings and move the player with `cc.Move`; **no save-state hash proves the absolute claim.** The baseline read used the private `ScenarioOneManager.ReadCoverageWorld` via **reflection, read-only**.

---

## 6. A1 / A2 / no-extension, in the light of the measurements

| Option | What the measurements say |
|---|---|
| **A1 — mapped presentation only** | **Not visual-only, measured.** `Dirt Path` planks become `Track` exclusions: −18 stems / −65.80 m² for the spine alone, −31 / −125.96 m² with one branch, coverage signature changes every time. Relabelling as `Headland` saves about one tree. A coverage-neutral representation therefore needs a *new* mask rule deliberately specified and approved — it cannot be obtained by renaming or by reusing planks. |
| **A2 — authoritative anchoring** | Only feasible under the **full-spine** interpretation, technically at 40° (16 mm margin) and robustly from 45°; it implies felling 3–38 trees per branch corridor, no on-spine `PO` entrance mechanism exists, and west branches meet the headland strip from 30° up. Plus the D-064/§10 conflicts already logged by #246. |
| **No extension** | Remains fully consistent with the code as measured; the existing layout's 15.555 m gap is the only pre-existing non-compliance to note. |

---

## 7. What this does NOT establish

- **No fit proof and no V2 §39.6.1 clearance.** "Sensible racks", acceptable rack lengths, turning space and the consequences of the implied removals were not judged. §39.6.1 is a condition on *verified sensible* geometry; this is a geometry measurement.
- **The spine definition and the zone boundaries are assumptions, not spec text.** The two columns in §0.1 must go to the Manager unmerged.
- **Feasibility at 40° rests on a 16 mm margin** and would not survive an additional constraint. It is not a robust result; 45° and above are.
- **"≥ 40°" is a statement about seven sampled angles**, not a proven continuous threshold.
- **Coverage geometry was derived in memory** from the five suggested racks, because production's coverage read is null in a fresh world (supported by measured `rackPlanConfirmed = false`; the harness's `productionReadCoverageNullInFreshWorld = true` flag is **hardcoded**, not measured by calling `ReadCoverage`). The derivation reports `SampleNotQualified`. This is not a prescription run; a herringbone rack set would give different rack-sample geometry and different deltas.
- **Traversal is harness steering**, not human input, at fixed dt 0.02 s.
- **No render was taken.** No Stand Map or minimap capture, and no on-screen judgement of which surfaces read as roads. `plan_view_measured.svg` replaces it with measured coordinates.
- **The measured candidate is not proven to be the user's playtest build.**
- No Windows/Player build, no push to the playtest branch, no implementation of any kind.

---

## 8. What the Manager now needs to decide

1. **Which spine.** Extension only, or the existing in-stand route plus extension. This decides feasibility outright.
2. **Whether a 16 mm margin counts as feasible at 40°**, or whether the band should be required robustly (45°+). This is a judgement about tolerance, not geometry.
3. **Whether the A1 mask rule should exist at all.** If a coverage-neutral mapped presentation is wanted, it needs its own specified mask semantics; reusing `Dirt Path` planks is measured to change treatable ground.
4. **Whether a north-only spine is acceptable at ~8.9 m spacing** as a deliberate deviation, or whether five racks require the full-spine interpretation.
5. **The removal consequence.** 3–38 trees per branch corridor is a silvicultural act requiring its own authorisation, not an art change.
6. **What "sensible" adds to 16–20 m spacing**, since §39.6.1 is not cleared by spacing alone.

---

## 9. Evidence inventory

Root `/media/jer/Files/CCF/Verification/ScenarioOneAccessSpineMeasure-20261103T2110Z/` — `README.md` carries the commands, disk numbers and the sha256 of every harness and results file, verified by DPC against the files themselves.

| File | Content |
|---|---|
| `RESULTS.md` | full findings in packet question order, A1–E17, with the corrections applied |
| **`CORRECTIONS.md`** | **the review-found spacing error, the formula, what is superseded, the corrected table, effect on the conclusion** |
| `measurements/b7_angle_sweep_corrected.json` | **the §0.1 table (cite this)** |
| `measurements/b8_candidate_layouts_corrected.json` | corrected explicit 16 m candidates + corridor tree counts |
| `measurements/b7_angle_sweep.json` | original — **superseded, retained unmodified** (sha256 prefix `6184d718`) |
| `measurements/b8_candidate_layouts.json` | original — **superseded, retained unmodified** (`6cf0408a`) |
| `measurements/meta.json` | frame, world, controller, constants |
| `measurements/scenery.json` | planks, polylines, clearing, headland, sites |
| `measurements/trees.json` | 1344 trunks with species, DBH, extents |
| `measurements/suggested_racks.json` | five racks + entrance trees resolved |
| `measurements/pair_passability.json` | 4095 neighbour-pair passability |
| `measurements/traversal.json` | six legs, contacts, offsets, speeds |
| `measurements/turning_room.json` | nearest trunk centres and colliders at five junctions |
| `measurements/mfm.json` | baseline masks, 8 scenarios, cells, ids, per-area deltas |
| `plan_view_measured.svg` | measured plan view |
| `harness/`, `analysis/`, `driver/`, `logs/` | harness source, search scripts (original + corrected), driver, raw Unity log |
| `preflight/`, `side-effects/`, `logs/lock-*.json` | preflight receipts, Editor side-effect patch, lease release |
| `review/REVIEW-VERDICTS.md` | **verbatim reviewer verdict table — the authority for the 9/10/1 counts, frozen for the PR #249 reconciliation** |

---

# Appendix A — V2 Orchestration Decision Appendix

Live append-only ledger: [`ORCHESTRATION-LEDGER.md`](ORCHESTRATION-LEDGER.md). **Six** timed #248 events were appended: (1) #243 exact-owner release + preflight + lane + lease + dispatch, 21:06–21:16Z; (2) packet refresh and five-step reconciliation, 21:26Z; (3) run-1 PASS with the DPC spine-definition cross-check, 21:33–21:38Z; (4) the b7 two-zone result and b8 cancellation, 21:36–21:41Z; (5) lease release + MATERIAL ERROR review verdict + correction dispatch, 21:45–21:52Z; (6) correction returned and reconciled + handoff published, 21:52–21:58Z. Historical #246 events and the Manager's policy block are untouched.

## A.1 Routing

| Phase | Pool / model | Effort | Role | Why |
|---|---|---|---|---|
| Measurement (a)(b)(c) | `claudeAgent` → `claude-sonnet-5-5` | `medium` | research / test | Long-lived Unity batchmode scripting inside a strict disposable-harness and disk-budget envelope; the Claude pool had just run the #243 lane successfully on this project, so capacity was **observed**, not assumed. |
| Independent review (d) | `codex` → `gpt-6.1-sol` | `reasoningEffort: medium` | review | Cross-provider independence from the worker **and** from the GPT-6-family Manager, against a fixed 20-claim list. |
| Correction | `claudeAgent` → `claude-sonnet-5-5` | `medium` | research | Same worker, resumed with the reviewer's findings; pure Python, no Unity, no lease. |
| Orchestration / synthesis | DPC parent (`opencode-go` → `deepseek-v4.1-flash`) | inherited | routing, verification, publication | Not a permitted measurement worker; used for decomposition, independent cross-check, evidence verification and this document. |

**Exploit/explore:** the measurement route was **exploit** of the just-observed Claude capacity; the review route was **exploit of independence**. No provider switch, retry or salvage was required at any point.

## A.2 Capacity — what was actually observed, and what was not

- **OBSERVED:** both pools enabled with `canRunChildTask: true`; **no cooldown, rate-limit or login failure occurred during the whole run.**
- **UNKNOWN, recorded as UNKNOWN:** subscription quota, rate-limit window, reset time, remaining credit, per-child token input/output, model-call counts and context headroom. This harness exposes no such telemetry. **Nothing here is estimated or reconstructed.**
- **Capacity-to-completion:** `sufficient by task size / unknown by quota`. The brief was self-contained against an exact ref, so a provider failure would have cost only partial evidence already written to durable storage.

## A.3 Failures, classified

| Event | Classification | Action |
|---|---|---|
| Worker turn ended ~21:22Z mid-run with a stale "still importing" message | **harness/process behaviour, not reasoning or quota** | driver kept supervising; DPC attached a completion waiter and resumed the same child with the real result |
| `analysis/b8_layouts.py` unbounded (~10⁹ Python ops, no output) | **worker efficiency defect — missing memoisation** | DPC cancelled the PID; a bounded cached rerun had already completed and was retained as supporting detail |
| **`maxmin_gap` used `Δz·sin α` on a bowed spine (review C8, FAIL)** | **worker quality error — formula valid only under an unstated assumption** | independent review caught it; corrected output published beside the untouched originals; `CORRECTIONS.md` written; conclusion direction preserved, numbers and margin corrected |
| Editor rewrote `.vscode/settings.json` | known Unity side effect | patch preserved, file restored byte-exactly, lane verified clean |
| DPC stray files in `CCF-Primary` | orchestrator housekeeping error | confirmed duplicated in evidence `preflight/`, then removed; CCF-Primary clean |
| Quota cooldown / login failure | **none occurred** | no retry, no switch, no fallback |

## A.4 Milestones

**MEASURED** — yes, on the exact candidate. **REVIEWED** — yes, Appendix B, and the review changed the result. **IMPLEMENTED / INTEGRATED / VERIFIED / ACCEPTED** — **none**, and none claimed. The DRAFT spine spec is untouched and D-064 remains controlling.

---

# Appendix B — Independent review and correction

**Reviewer:** `codex` → `gpt-6.1-sol`, `reasoningEffort: medium`, role `review`, read-only. It launched no Unity, wrote no files, and did not touch the lease. It inspected production code at `72b4efbd`.

## B.1 Verdict

**MATERIAL ERROR.** Over **20 fixed claims: 9 PASS, 10 PASS-WITH-CORRECTION, 1 FAIL** (9 + 10 + 1 = 20). All README sha256 hashes independently matched. The verdict table is frozen verbatim at `review/REVIEW-VERDICTS.md` in the evidence root.

> **Count correction (disclosed).** This document and the #248 comment first stated "13 PASS, 7 PASS-WITH-CORRECTION, 1 FAIL" — wrong distribution, and 13 + 7 + 1 = 21 against 20 claims reviewed. The Manager flagged the mismatch; DPC re-derived the counts from the reviewer's own verdict table. **The review conclusion — MATERIAL ERROR, caused by the C8 spacing-formula failure — is unchanged.**

## B.2 The FAIL, and what it changed

| | |
|---|---|
| Defect | `analysis/b7_sweep.py:maxmin_gap` computed same-side branch spacing as `Δz·sin α` |
| Why wrong | Valid only for a vertical spine. Branches leave a junction along `(s·sin α, cos α)`, so the true perpendicular distance is `|Δx·cos α − s·Δz·sin α|`; FULL_SPINE junctions lie on the **bowed** route where Δx is several metres |
| Damage | the whole FULL_SPINE column and every advertised 16 m candidate; the 40° east-three candidate's real gaps are **19.454 / 12.770 m**, and its first junction was inside the feeder-pad exclusion |
| Not damaged | NORTH_ONLY (Δx = 0 there) and the angle-independent infeasibility bound, both independently reconfirmed |
| Two further defects found while fixing | the clearance test skipped the junction point, admitting z = −24.0/−24.05 inside the feeder-pad exclusion; obstruction clearance was sampled at 0.25 m instead of exact segment-to-circle |
| Correction | corrected search at 0.01 m junction resolution, exact clearance including the junction, southern bound −23.85 |
| Reconciliation | worker's corrected values agree with the reviewer's independent search to **≤ 0.020 m** at every angle; the worker states the reviewer's lower numbers should be trusted where they differ |

**Effect on the conclusion:** extension-only infeasibility **stands**; full-spine feasibility **stands but narrows sharply** — 16.016 m at 40° (a 16 mm margin) rather than 18.06 m, and 13.606 m at 35° rather than 15.97 m. The published candidate layouts are superseded. **No measured direction was overturned; several published numbers were.**

## B.3 The seven labelling corrections (accepted and applied)

1. Headland is **presentation**-only, not absent from production clearance/MFM geometry.
2. Turning-room distances are to the trunk **centre**, DERIVED, and are not available-clearance or turning tests.
3. Lattice step/jitter statistics are DERIVED; the estimator and population must be stated.
4. The five suggested racks are **hypothetical** geometry, so MFM numbers are production world inputs + hypothetical rack geometry, **not** an operational prescription baseline.
5. Only the **coverage** signature changes per scenario; the prescription derivation signature was not recomputed.
6. `productionReadCoverageNullInFreshWorld` is **hardcoded** in the harness, not measured by calling `ReadCoverage` (production code independently supports null via `rackPlanConfirmed = false`).
7. "No mutation" is stated precisely: the harness creates a GameObject, enters Play mode, changes settings and moves the player with `cc.Move`; no persistent gameplay write-back was found, and no save-state hash proves the absolute claim.

Also accepted: "from 40° up" means **at the sampled angles**, not a proven continuous threshold.

## B.4 What the review asked for that this evidence still does not provide

1. An authoritative definition of spine scope and acceptable angles, lengths, spacing and junction sharing.
2. A corrected five-rack candidate checked against production editing rules, corridor clearance, removals and usable turning space.
3. Representative rendered/player review and MFM evaluation of that actual candidate; qualified prescription evidence if prescription consequences inform acceptance.

## B.5 Retrospective

This is the **second** time in this programme that a single-provider output would have shipped a substantive technical error — the first was the MFM mask taxonomy in #246. Both had the same shape: a formula or taxonomy correct under an **unstated simplifying assumption** (straight spine / single mask kind) and silently wrong outside it. The fixed-claim-list cross-provider review caught both. A future brief should require the assumption under which a formula is valid to be written next to the formula.
