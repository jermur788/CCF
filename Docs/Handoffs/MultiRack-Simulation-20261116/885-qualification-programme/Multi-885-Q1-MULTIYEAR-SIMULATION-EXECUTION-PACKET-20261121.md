# Multi #885 — Q1 multi-year simulation execution packet

**Status: BLOCKED — NOT AUTHORISED TO RUN.**
This is the operational Q1 packet with the exact integrated SHA pinned. It is **not** an approval. Three named conditions must clear before any experiment may run (§10).

| Item | Value |
|---|---|
| Packet author | DeepSeek V4.1 Flash — orchestration/documentation only. Not the integrator, not the run worker, not the reviewer |
| Prepared | 2026-11-21 |
| Exact source | **`4cecf87339bea4d32907955a8d5625d1f27f96bd`** |
| Delivery ref | `integration/multi-885-ma1-two-area-20261121-v1` — **confirmed present on origin at that exact SHA** |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` (clean, delivered) |
| Integration verdict | **NEEDS CHANGES** — 4 of 6 gates PASS, 2 incomplete. See the Stage I result and the orchestrator reconciliation. |
| SDK | `/home/jer/.dotnet/dotnet` **8.0.134** (system `dotnet` 10.0.115 must never be used) |
| Host | `DESKTOP-DL1BQI7`, WSL2 Ubuntu, uid 1000 `jer` |
| Protocol basis | Q0 revision 3, blob `c927c12e86186188bf9a465e89ef0fbc8b7046f0`, SHA-256 `021c6ff853863ff2803189872364dc18c6174f7a599e419c6781d6b86163d58c` — APPROVED/CLOSED, not reopened |
| Physical lease | **Reserved, unused:** `/home/jer/Development/CCF-885-Qualification-Scratch-20261121`, loopback `:33841` (see §10 condition 2 — the port reservation is not executable) |
| Routing | `dec-885-stage1-integ-20261121T1648Z`; blockers `blk-9fccec6a265b8e3f` (port/lease) and the P0-isolation contract conflict |

**Non-negotiable scope statement.** These are the first genuine multi-year runs of a **synthetic** closed-canopy Sitka model. They are **not** ecological validation, **not** an Irish field forecast, **not** MA-2 empirical comparability (G1 remains NO-GO), and **not** owner-accepted. Regeneration, windthrow, browsing, canopy light, timber grades, economics and cross-area ecological interaction are **NOT MODELLED**.

---

## 1. Why this route, and what cannot be used

Q0 §2.9: **MA-1 has no CLI route.** The only lawful callers are the UI HTTP API and internal types via `InternalsVisibleTo`. The second requires adding code to the repository and is therefore out of scope — it would need a **new bounded source packet**.

**Q1 uses the UI HTTP API exclusively.** No repository file is added, edited or generated. The integrator's lane is **read-only** to Q1.

---

## 2. Verified runtime facts the harness depends on

Confirmed at `4cecf873` by the Stage I integrator:

1. `Program.cs` accepts **only a fixture directory** argument and hard-codes `Listen(IPAddress.Loopback, 0)` with an **empty configuration builder**. It ignores `ASPNETCORE_URLS`, `DOTNET_URLS`, `ASPNETCORE_HTTP_PORTS` and Kestrel endpoint configuration. **A fixed port cannot be requested.**
2. The process prints the capability URL; the two-area page line is exactly:
   `Two-area MA-1 simulator: http://127.0.0.1:<port>/s/<cap>/multi`
3. Every route is under `/s/<cap>/`. `Host` and `Origin` must be `127.0.0.1:<port>` exactly — `localhost` is refused with 403, and a missing/incorrect capability returns an empty 404.
4. One in-memory session per process. A fresh process is the only way to obtain a fresh session. Nothing is saved.
5. Requests need `curl --noproxy '*'`, because a proxy environment would otherwise intercept loopback.

**Consequence:** every session's port is OS-assigned and must be read from the printed line and recorded. §10 condition 2 exists because of this.

### 2.1 Callable surface

| Method | Path | Purpose |
|---|---|---|
| GET | `/s/<cap>/multi` | two-area page (history headings) |
| GET | `/s/<cap>/api/multi/view` | immutable view: areas, rosters, stands, Crop marks, plan state, events, annual history, source token, session id |
| GET | `/s/<cap>/api/multi/manifest` | manifest; content hash must equal `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` |
| POST | `/s/<cap>/api/multi/competition` | read-only per-tree CI diagnostic |
| POST | `/s/<cap>/api/multi/crop/mark` · `/crop/unmark` | Crop mark / unmark |
| POST | `/s/<cap>/api/multi/pending/add` · `/pending/remove` | plan orders (Fell). Fells nothing |
| POST | `/s/<cap>/api/multi/plan/approve` | Approve0: freezes the batch, mints `PlanId` + key, 0 Fell |
| POST | `/s/<cap>/api/multi/plan/cancel` | CancelApproved, 0 Fell |
| POST | `/s/<cap>/api/multi/plan/execute` | Execute1: one Fell event, Year and AnnualRevision unchanged, WorkRevision +1 |
| POST | `/s/<cap>/api/multi/year/advance` | Year+1, one year per POST, with `consentToCancelWork` |

**Fence.** Every POST body carries the complete fence: `fence.sessionId`, `fence.planStateRevision`, `fence.source.{domain, experimentId, manifestContentHash, areaIds, ecologicalYear, annualRevision, workRevision, rngModelVersion, growthModelVersion, p1AlgorithmVersion, areaModelVersion, actionEligibleDbhThresholdMm}`, plus route members. All must be present and non-null; duplicate, unknown and null members are refused with 400 **before** the engine is called. The harness must **echo the fence it just read**, never a cached or reconstructed one.

**Status codes.** 200 = committed/read; 409 = engine refusal (body carries the refusal code and a fresh view); 400 = malformed; 500 = outcome unconfirmed.

---

## 3. Frozen inputs

| Input | Exact value |
|---|---|
| Experiment content hash | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` |
| Area seeds | MA-A **`885101`**, MA-B **`885202`** (unique, fixed; shared across all sessions so per-tree-year draws are common) |
| Generator seed | **`8850001`** (separate from the area seeds) |
| Areas | MA-A pentagon **915 m²** (171 genesis trees, cohort age 24); MA-B quadrilateral **1500 m²** (73 genesis trees; 70 % aged 47, 30 % aged 29); shared edge |
| P0 reference | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, copied not rebuilt |
| Fixture | `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` — the harness copies it to scratch and passes the **directory** |

**Stochastic replication is out of scope.** The canonical experiment has exactly one seed per area; a seed variant changes the content hash and the approved design. Any variant run needs separate Manager authorisation and must be labelled "variant, not MA-1 canonical".

---

## 4. Scenario matrix

| Scenario | Intervention | Horizon |
|---|---|---|
| `C` | none (no-work control) | 50 |
| `L` | selection rule, intensity L, at year 0 | 50 |
| `M` | selection rule, intensity M, at year 0 | 50 |
| `D` | L's rule, first applied at year 5 | 50 |
| `S` | L's rule at year 0 **and** again at year 10 | 50 |
| `S1` | one year-0 intervention sized to `S`'s total realised stems per area | 50 |
| `I` | L's rule on **MA-A only**; MA-B untouched | 50 |

**2 fresh sessions per scenario = 14 sessions.** Capture **every** committed year 1…50; the years 1/5/10/20/50 are summary checkpoints computed from that all-year log, not the only years recorded.

Lawful stops (recorded as outcomes, never worked around): both areas empty → next `Advance` refuses `ContinuationNoLivingStems`; a `checked` arithmetic overflow; the §9 budget. Never change rules, seeds, the experiment definition or the horizon to obtain a longer run.

---

## 5. Selection algorithm — one algorithm, pre-registered

Exactly Q0 §6 steps 1–8 (revision 3). All steps are computed from **one intervention-time state**, before any Crop mark or order of that intervention is issued.

1. **Crop set.** Rank living trees by DBH descending, ties by ordinal `TreeId`. Walk once; accept as Crop if its float32 E3 distance to every already-accepted Crop is **> 8 m**. DBH order selects the Crop set only — it is **not** the later traversal order.
2. **Frozen competitor lists.** For every Crop, from the same state and before any selection: living neighbours within 8 m, ranked by `D_j / max(1, D_crop) / max(0.5, d)` in float32 descending, ties by ordinal `TreeId`. A tree on several lists stays on all. A Crop appearing on any list is a **reported defect**.
3. **Removal sequence.** Per area, areas in ordinal `AreaId` order. Build by rank layer: layer *k* holds, for each Crop in **ascending ordinal Crop TreeId**, that Crop's rank-*k* competitor if it exists. Concatenate layers 1, 2, 3, … Then **deduplicate at first occurrence**; a dropped duplicate is **not replaced** and the affected Crop does **not** advance to its next competitor. Log every dropped duplicate with Crop and layer.
4. **Intensity.** `L` = deduplicated layer 1. `M` = deduplicated layers 1–3. Every `L` entry is necessarily an `M` entry.
5. **Command order.** (a) Crop marks **first** (`MarkCrop` refuses an already-planned tree; `AddPending` refuses a Crop-retained tree). (b) Then `AddPending` in sequence order. (c) An empty selected order set ⇒ **do not call Approve** (it would refuse `WorkEmptyPlan`); record "no intervention" and continue. (d) Otherwise Approve, disclose, Execute. Crops are never ordered, so a populated area always retains a living tree and `WorkWouldLeaveNoLivingStems` cannot be triggered by this rule.
6. **Disclosure before Execute** — Crop list, each Crop's frozen ranked competitor list, the deduplicated sequence with dropped duplicates, and the exact `Approved.Orders` (area, `TreeId`, reason) with per-area counts. The reviewer's first check is this artefact.
7. **Realised intensity** from the engine's own per-area Before/After: removed stems, removed BA, removed fraction of each.
8. **"Genuinely different"** is measured, not assumed: `M`'s removed BA must be **strictly greater in both areas**. Otherwise report "not genuinely different in area X" and do not force it.

**Rule (b)** (removal from below by DBH rank) is **not used** — it needs a numeric threshold Q0 cannot justify.

### 5.1 `S1` equivalence

`S1` = one year-0 intervention from the **same** Crop set, felling the first `n_A`/`n_B` entries of the **same** year-0 deduplicated sequence across all layers, where `n_A`/`n_B` are `S`'s **total realised stems removed per area across years 0 and 10**. Sized only from `S`'s realised counts, never from an `S1` outcome. Primary equivalence (removed stems per area) is exact; removed BA, ID identity and timing mismatches are reported, not hidden. If the year-0 sequence holds fewer than `n_X` trees, `S1` is "not constructible in area X" with the shortfall — no substitute.

---

## 6. Determinism projection (the repeat gate)

Two fresh sessions must be equal under this projection **at every year and after every command**:

1. **Byte-equal:** `Annual` (including `null` for a carried-forward area); `View.Areas`; `View.Source`; `View.Experiment`; `View.PlanStateRevision`; `View.AnnualHistory`; every `RefusalCode`.
2. **Checked within each session, then excluded:** `SessionId` is 32 lowercase hex, constant, and equals every fence sent; every `PlanId` equals `SessionId + ":ma-plan:" + PlanStateRevision-at-approve`; every key equals `PlanId + ":execute"`; every Fell record's `PlanId`/`ExecutionKey` equals its event's. Any violation is a **reported defect**, not a normalisation case.
3. **Normalised then compared:** replace the exact leading `SessionId + ":"` of each `PlanId`/key with `SESSION:` in `Pending`, `Approved`, `Events` (with Fell records) and every `Execution` result; require byte equality. Nothing else is rewritten.
4. **Order** is the engine's own (areas by `AreaId`, rows/rosters by `TreeId`, history/events in commit order, Fell records by `TreeId`). **The harness must not re-sort.**
5. **Float fidelity:** log every float32 as its 32-bit pattern (8 hex) and every double as its 64-bit IEEE-754 pattern (16 hex). Equality is bit equality. A JSON mismatch with all bit patterns equal is a **serialiser** difference, not a biological one.

Raw full-view comparison across sessions is **forbidden** — it is structurally impossible because `SessionId` is random and embedded in every `PlanId`, key and Fell record.

---

## 7. Measurements and null discipline

**Per tree, per committed year, per area:** `TreeId`, age, DBH, height, crown radius, pre-growth CI (m⁻¹), neighbour count, `S`, `p` (probability per annual step — **never** called a hazard rate), `u`, death year/cause/mechanism.

**Per area, per committed year:** living count, hazard deaths, boundary deaths, cumulative natural deaths, cumulative Fell, extinction year, first carry-forward year (separately, or "none"), stems/ha, area-normalised BA m²/ha, Dq, arithmetic mean DBH (harness-computed), RD, density pressure, mean vulnerability.

**Null discipline — mandatory.**
- `null` = **not evaluated**, never zero. `S`, `p`, `u` are `null` when `P ≤ 0`; `u` is `null` when `p` is `null` or 0. `p = 0` is a real evaluated zero.
- Dead carried audit rows hold placeholder `CI = 0` and neighbours `0`; log as **not evaluated**, not measured zeros.
- **Extinction year:** report is non-null with `After.LivingCount == 0`, history `NoLivingStems = false`, normal per-tree rows.
- **Carry-forward year:** a *later* committed year has `Report = null`, `NoLivingStems = true`; one "carried forward, no per-tree audit" row, zero per-tree rows. A documented state, not missing data.
- Means over an empty set are `null`, never 0.

This distinction is the **primary Stage Q risk**: an extinct area's carry-forward `null` must never be reported as "the area ran a year" or converted to zero.

**Integrity checks per scenario:** living count non-increasing (no recruitment exists — an increase is a **defect**, not a finding); no double application (every Fell ID appears once and dies once; Fell + natural deaths = genesis − living); per-intervention chronology (Approve leaves living count and year unchanged; Execute keeps `EcologicalYear`/`AnnualRevision` and moves `WorkRevision` +1; felled trees carry `Death.EcologicalYear == y`, cause `fell`; Year+1 `Before` equals the event's per-area `After`; felled IDs absent from every neighbour list in that step; natural deaths carry y+1).

**Isolation check:** `I` versus `C` must be byte- and bit-equal in MA-B `View.Areas[B]`, `Annual.Areas[B].Report` and every history `Areas[B]` entry, at every year. Legitimately differing global fields (`WorkRevision`, `PlanStateRevision`, event ledger, `CancelledUnexecutedWork`, `SessionId`) are excluded and listed.

**Valid-case boundaries, each attempted and recorded:** fell-to-empty refusal (approval stays), Approve-then-Cancel (0 Fell, PlanState +1, orders not returned), consented advance with pending work (0 Fell, `CancelledUnexecutedWork = true`), replay of an executed key (`Replayed = true`, no change), all-empty advance refusal. The last is **not reachable on canonical data by player action**; report "not exercised on canonical data" unless natural mortality produces it.

**Valid comparisons.** Matched retained set for [t₀,t₁] = `TreeId`s living at both ends in **every** compared session and never felled in any; report ΔD per tree and the paired ΔD_L−ΔD_C / ΔD_M−ΔD_C, plus set size and share of genesis. Selection-only shift at each Execute (year unchanged, no growth): mean DBH and Dq of post-Fell minus pre-Fell living set, per area. Stand mean DBH is reported **only alongside** both components and **never alone** as growth-response evidence. Frenchpark comparison is **sign only**, with an explicit statement that these are synthetic stands, not yield plots.

---

## 8. Diagnostics — pre-registered, diagnostics only

None of these establishes empirical validity; none is a pass/fail gate for the engine.

| ID | Test | Rule |
|---|---|---|
| H-W2(a) | CI invariance to uniform DBH scaling | Recompute CI offline with all DBH × λ, λ ∈ {1.1, 1.5}. **Reject** if any tree with `λ·D_i ≥ 1 cm` has `|CI_λ − CI| > 1·10⁻⁵·max(CI, 1 m⁻¹)` |
| H-W2(b) | ΔD is a function of pre-growth D and CI only | Recompute in float32 in coded order; **reject** if any `ΔD_obs` differs by more than 1 ULP of D(t+1) |
| H-W3 | hazard vs boundary dominance | Per scenario, deaths by mechanism per 10-year window and whole horizon; **flag** if boundary deaths > 50 % of natural deaths in a window with ≥ 10 natural deaths. A "hazard-dominated" result does **not** validate the [C] constants |
| H-W4-CI | edge trees have lower CI | Session C; each area separately; t = 0 and t = 10 separately; strata MA-A{24}, MA-B{29}, MA-B{47} → 6 stratum-years. OLS `CI = a + b·edge + c·ln D`. **BIAS** if `b<0` and `|b| > 0.20·m_int`; **SMALL** if `b<0` and `|b| ≤ 0.20·m_int`; **NO BIAS** if `b≥0`; **NOT ESTIMABLE** per shared rules. Overall CONFIRM if all 6 BIAS / REJECT if all 6 NO BIAS / INCOMPLETE if any NOT ESTIMABLE / MIXED otherwise |
| H-W4-G | edge trees have larger 10-year ΔD | Same strata; year 0→10 retained set in C; OLS `ΔD = a + g·edge + c·ln D₀`. **GROWTH ADVANTAGE** if `g>0` and `g > 0.20·ΔD_int`; **SMALL**; **NO ADVANTAGE** if `g≤0`. Reported **separately** from H-W4-CI |
| W5 | height envelope ratio drift | `|ρ(t)/ρ(0) − 1| ≤ 8·t·2⁻²⁴` per living tree; clamp-touching trees excluded and listed. Larger drift = defect in the E6 description |
| W11 | retention/replay | Expected per-tree rows = Σ over non-null reports of audit-row count (incl. dead carried rows); `null` ⇒ 0 rows + 1 marker. Exact match required. Plus transcript replay equality |
| W12 | README drift | Run both documented CLI `annual` forms once; record exit code and output |

**H-W4 shared rules.** `NOT ESTIMABLE` if fewer than 2 edge or 2 interior trees (retained, for G), or the n×3 design matrix is rank-deficient (smallest singular value ≤ 1·10⁻⁹ × largest), or the denominator ≤ 0 — reported with counts and reason, never counted toward CONFIRM/REJECT. **No p-values** (census, not sample). Trees are not independent replicates; years are not pooled. The `ln D` term is a *chosen adjustment*, not proof that DBH confounding is removed. No causal claim is made.

---

## 9. Harness, budget and output bundle

- **Location:** `/home/jer/Development/CCF-885-Qualification-Scratch-20261121/<run-id>/harness/`. **Nothing is added to the repository.**
- **Dependencies:** bash, `curl`, `python3` standard library, `sha256sum`. No packages, no non-loopback network.
- **Disclosure:** `SHA256SUMS` over every harness byte, published **before** the run and included in the bundle. A scratch script is **not** a reviewed repository feature and must never be described as one.
- **Components:** `q1-driver.sh` (process lifecycle + ordered transcript), `q1-select.py` (the frozen selection, a pure function of the intervention-time view), `q1-record.py` (parse with `null`/`0` preserved), `q1-analyse.py` (offline diagnostics), `q1-replay.sh` (transcript replay + projection comparison), `SHA256SUMS`, `README.md`.
- **Process discipline:** start and stop only the harness's own processes, by PID, with SIGINT and a clean-exit assertion. Never `pkill`/`pgrep`-kill. `#877` (1750123, :33767) and `#882` (1806361, :43649) are protected; `#867` is not touched.
- **Budget:** stop a scenario if a single year exceeds 60 s or the scenario exceeds 30 min; record the stop, last committed year and cause. **Never** extend the horizon or silently drop a repeat to fit the budget.
- **Abort the whole run** on: manifest content-hash mismatch, any projection mismatch between repeats, a 500 unconfirmed outcome, an unexplained living-count increase, or evidence of a second writer in the lane. Every abort is published as **INCOMPLETE** with concrete evidence.

```
<run-id>/
  harness/            the six files + SHA256SUMS
  header.json         source SHA, manifest hash, harness hashes, dotnet/node/kernel, host, worktree, session start order
  sessions/<scenario>/r<repeat>/transcript.jsonl
  sessions/<scenario>/r<repeat>/reports/year-NN.json
  sessions/<scenario>/r<repeat>/view-year-NN.json
  sessions/<scenario>/r<repeat>/disclosure-*.json
  analysis/  trajectories.csv  per_tree_year.csv  fells.csv  diagnostics.md  anomalies.md
  logs/      server stdout/stderr per session, every bound port
```

**Publication:** compact CSVs plus a GitHub Markdown summary with source-linked tables and trajectory figures. Raw per-tree logs and server stdout stay in scratch, enumerated by path and SHA-256; they are not silently committed. Publication limits are stated explicitly. Negative results, unexpected mortality, extinction events and numerical anomalies are preserved, never filtered.

---

## 10. Blocking conditions — all three must clear

| # | Condition | Owner | Status |
|---|---|---|---|
| 1 | **P0 isolation L4** executed lawfully, or a documented Manager disposition that pin agreement + L0–L3 + the two smoke no-write checks are sufficient for Stage I closure. The conflict: `tests/p0-isolation.sh` unconditionally deletes `.p0ref` and P1 build outputs and invokes `build/build-p0-core-ref.sh` (a reference rebuild), while the P1 spec/README says the reference is never rebuilt on this path; the D-091 receipt prohibits rebuilding **except through** that script and names both scripts in its step 4. Needs one bounded ruling. | #885 Manager (+ D-091 custodian) | **BLOCKED** |
| 2 | **Fixed-port reservations are not executable.** `Program.cs` hard-codes `Listen(IPAddress.Loopback, 0)` with an empty configuration builder, so neither `:33811` (integration) nor `:33841` (qualification) can be pinned. Needs the lease amended to "loopback only, OS-assigned ephemeral port, every bound port recorded per run, no fixed-port requirement" — or an authorised bounded source packet making the port configurable. Recommended: the amendment. | D-091 custodian | **BLOCKED** — `blk-9fccec6a265b8e3f` |
| 3 | **N1 naturally-empty history witness.** The empty branch is not reachable through the shipped API at short horizons. It may occur naturally during a 50-year no-work control once an area empties by hazard mortality (Q0 W15 says this is possible, not certain). Options: (a) accept the Stage I limitation and treat the first natural extinction in Stage Q as the witness; (b) obtain a documented Manager disposition accepting the current partial evidence. Manufacturing emptiness by editing source, fixture or payload is **not authorised** under any option. | #885 Manager | **BLOCKED / decision needed** |

Additional requirements, not blockers:

| # | Requirement | Owner |
|---|---|---|
| 4 | Q1 packet flipped to **APPROVED TO RUN** on this exact SHA | #885 Manager |
| 5 | A **distinct** qualified run worker — not the integrator, not the reviewer, not the orchestrator | orchestrator routing + Manager |
| 6 | Scratch directory created and allocated | D-091 custodian |
| 7 | Harness bytes disclosed with SHA-256 before the run | run worker |
| 8 | No repository write, no protected file, no Unity, no Windows build, no `main` merge, no owner-acceptance claim | all |

---

## 11. Stage Q → R → S handoff expectations

- **Q produces:** the executed per-year/per-tree bundle, the comparative outcomes (no intervention, L, M, delayed, staged, `S1`, isolation), realised intensities, mortality and extinction events, anomaly register, reproducibility evidence, and an honest verdict that may be **INCOMPLETE**.
- **R (independent review)** must be a genuinely separate, non-author session. It verifies exact engine identity, scenario inputs, raw outcomes, calculations, deterministic replay and interpretation, and it separates **software correctness** from **scientific validity**. Verdict applies to technical experiment qualification only.
- **S (next improvement)** is a bounded correction packet or, where new mechanics or scientific direction would be required, a four-file **SPEC REQUIRED** draft. It is **not** an implementation grant. No G1/G4/G5 action is implied.

---

## 12. Claim ladder

**PRODUCED / REVIEWED / INTEGRATED / VERIFIED / OWNER ACCEPTED.**

- This packet: **PRODUCED**, not approved, not executable.
- Stage I: integration ref delivered at `4cecf873` = **INTEGRATED (branch ref only)**; technically **VERIFIED for 4 of 6 gates**; verdict **NEEDS CHANGES**.
- Stage Q results: **PRODUCED** until the independent reviewer returns.
- Stage R: technically **REVIEWED** for experiment qualification only.
- **OWNER ACCEPTED: never asserted by any agent in this programme.** G2 is the owner's own playtest and remains pending.
