# Multi #885 — Q1 multi-year simulation protocol (DRAFT)

**Status: DRAFT — NOT AUTHORISED TO RUN.**
This document is the source-independent Q1 run plan and harness design permitted in parallel with Stage I. It is **not** an execution packet and confers no run authority. A separate, finalised Q1 execution packet must pin the exact integrated SHA and the executed physical permissions after Stage I returns a verdict.

| Item | Value |
|---|---|
| Author | DeepSeek V4.1 Flash — orchestration/documentation only; not the integrator, not the experiment runner, not the reviewer |
| Prepared | 2026-11-21, during Stage I integration |
| Frozen source basis | Q0 revision 3, `Docs/Handoffs/MultiRack-Simulation-20261116/885-q0-qualification-20261121/Q0-EQUATION-AND-RESEARCH-CROSSWALK.md` at commit `d9ca6e0ce292f9d23eb7ac761ad6a3ef209cabc1`, blob `c927c12e86186188bf9a465e89ef0fbc8b7046f0`, 559 lines, SHA-256 `021c6ff853863ff2803189872364dc18c6174f7a599e419c6781d6b86163d58c` (independently recomputed by the GPT-6.1 Sol reviewer) |
| Q0 verdict | APPROVED / CLOSED, scoped to R1–R3 and F4–F5, final reviewer `5239c33f`. **Not reopened here.** |
| Code candidate | `4cecf87339bea4d32907955a8d5625d1f27f96bd` — candidate only. An integrated/verified SHA is **not established** at the time of writing. |
| Physical lease | Reserved, **unused**: `/home/jer/Development/CCF-885-Qualification-Scratch-20261121`, loopback `127.0.0.1:33841` |
| Routing decision | `dec-885-stage1-integ-20261121T1648Z` (portable-fallback ledger, see §12) |

**This protocol decides nothing about forestry science.** Every threshold, intensity and stopping rule below is a *protocol choice* recorded before any outcome is seen. None is an ecological coefficient, and none is calibrated.

---

## 0. What Q1 is and is not

**Q1 is:** the first genuine execution of the existing MA-1 two-area annual engine over 1, 5, 10, 20 and (if lawful) 50 years, under pre-registered management scenarios, producing per-tree and per-area trajectories that a separate reviewer can replay and audit.

**Q1 is not:**
- a calibration, validation or field-comparability exercise;
- a MA-2 empirical transfer (G1 remains **NO-GO**; 0 source-verified four-area treatment/outcome groups);
- a claim that regeneration, windthrow, browsing, canopy light, timber grades, economics or cross-area ecological interaction have been modelled — they are **NOT MODELLED** (Q0 §4);
- an owner acceptance. **G2 remains pending and separate.**

---

## 1. Execution route: what is actually callable

Q0 §2.9 records that **MA-1 has no CLI route**. The two lawful callers are:

1. the UI HTTP API (`LAB/src/Ui/PlayerMultiApi.cs`), and
2. internal types via `InternalsVisibleTo("SitkaP1.Core.Tests")`.

Route 2 requires adding code to the repository, which needs a **new bounded source packet**. It is therefore out of scope for Q1.

**Q1 uses route 1 only.** This is the decisive design constraint, and it is why the harness lives outside the repository.

### 1.1 Exact callable surface (verified at the candidate)

All routes are under the printed capability prefix `/s/<64-hex>/`, on one loopback listener, guarded by exact `Host` and `Origin` checks.

| Method | Path | Purpose |
|---|---|---|
| GET | `/s/<cap>/multi` | two-area page (contains the history headings) |
| GET | `/s/<cap>/api/multi/view` | current immutable view: areas, rosters, stands, Crop marks, plan state, events, annual history, source token, session id |
| GET | `/s/<cap>/api/multi/manifest` | published experiment manifest (content hash must equal `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`) |
| POST | `/s/<cap>/api/multi/competition` | read-only per-tree CI diagnostic |
| POST | `/s/<cap>/api/multi/crop/mark`, `/crop/unmark` | Crop mark / unmark |
| POST | `/s/<cap>/api/multi/pending/add`, `/pending/remove` | plan orders (Fell). **Fells nothing.** |
| POST | `/s/<cap>/api/multi/plan/approve` | Approve0 — freezes the cross-area batch, mints `PlanId` + execution key, 0 Fell |
| POST | `/s/<cap>/api/multi/plan/cancel` | CancelApproved — 0 Fell |
| POST | `/s/<cap>/api/multi/plan/execute` | Execute1 — one typed Fell event, Year and AnnualRevision unchanged, WorkRevision +1 |
| POST | `/s/<cap>/api/multi/year/advance` | Year+1, one year per POST, with the consent flag `consentToCancelWork` |
| POST | `/s/<cap>/api/multi/hit` | map hit-test (not used by Q1) |

**Fence.** Every POST body must carry the complete fence with all 13 members: `fence.sessionId`, `fence.planStateRevision`, `fence.source.{domain, experimentId, manifestContentHash, areaIds, ecologicalYear, annualRevision, workRevision, rngModelVersion, growthModelVersion, p1AlgorithmVersion, areaModelVersion, actionEligibleDbhThresholdMm}`, plus the route's own members. Members must all be present and non-null; unknown, duplicate and null members are refused with 400 before the adapter is called. **The harness must send the fence it just read** — never a reconstructed or cached one.

**Status codes.** 200 = committed or read; 409 = refused by the engine (body carries the refusal code and a fresh view); 400 = malformed request; 500 = outcome unconfirmed.

**Session model.** One in-memory session per process. **Every scenario repeat is a fresh process**, which is also how a fresh `SessionId` is obtained.

### 1.2 Blocking finding: the granted port cannot be pinned

The shipped process chooses its own OS-assigned loopback port at each launch and prints the URL. It deliberately ignores `ASPNETCORE_URLS`, `DOTNET_URLS`, `ASPNETCORE_HTTP_PORTS` and Kestrel endpoint configuration (the existing smoke test sets all four to a hostile value specifically to prove they are ignored), and the README states "The operating system picks the port at each launch".

Therefore the D-091 reservation of **`127.0.0.1:33841` is not executable as a pin** without a source change, which is not authorised.

**This is a custodial decision, not an orchestration one.** Q1 execution is blocked on `blk-885-q1-port-pin` until the D-091 custodian either:
- (a) amends the qualification lease to "loopback only, OS-assigned ephemeral port, every bound port recorded per run, no fixed-port requirement", or
- (b) authorises a bounded source packet that makes the port configurable.

Recommendation: **(a)**. It matches how the two already-qualified smoke suites run, changes no product code, and preserves the isolation property that matters (loopback only, not 5187, ephemeral, capability-gated).

---

## 2. Harness: location, disclosure and non-invasiveness

- **Location:** the reserved qualification scratch, `/home/jer/Development/CCF-885-Qualification-Scratch-20261121/<run-id>/harness/`.
- **Nothing is added to the repository.** No tracked file, no project file, no test file, no fixture, no golden. The integrator's locked worktree is read-only to Q1.
- **Disclosure:** the harness is disclosed byte-for-byte with a published SHA-256 manifest (`SHA256SUMS`) *before* the run, and the same manifest is included in the result bundle. A scratch script is **not** a reviewed repository feature and must never be described as one.
- **Dependencies:** bash, `curl`, `python3` standard library only, `sha256sum`. No packages, no network beyond loopback, no `pip`.
- **Isolation:** every process is started and stopped by PID by the harness itself. Nothing else on the host is killed. `#877` (pid 1750123, :33767) and `#882` (pid 1806361, :43649) are protected; `#867` is not touched.
- **Evidence discipline:** the harness never reformats, re-sorts or "fixes" engine output. It records raw response bytes plus a parsed projection.

### 2.1 Harness components

| File | Role |
|---|---|
| `q1-driver.sh` | starts/stops one fresh server per session, drives the ordered command transcript, writes raw + parsed records |
| `q1-select.py` | the frozen selection algorithm (§3), a pure function of the intervention-time view |
| `q1-record.py` | parses each response into the log schema, preserving `null` vs `0` |
| `q1-analyse.py` | offline diagnostics and tables (§6), reading only the logs |
| `q1-replay.sh` | deterministic replay of a transcript into a fresh session and projection comparison |
| `SHA256SUMS` | disclosure of every harness byte |

### 2.2 Driver pseudocode (not code)

```
for scenario in [C, L, M, D, S, S1, I]:
  for repeat in [1, 2]:
    start fresh server process  -> capture capability URL, port, pid
    transcript := []
    read view v0
    assert manifest content hash == pinned c66d566c…
    record header {source SHA, manifest hash, harness SHA256, dotnet version, node version, kernel,
                   host, worktree path, session start order}
    for year y in 0 .. target_horizon:
      if scenario schedules an intervention at y:
        v := read view
        plan := q1-select.py(v, scenario_intensity)     # pure function of v
        record disclosure {crop list, per-crop frozen ranked competitor list,
                           deduplicated sequence, dropped duplicates, per-area counts}
        emit Crop marks first  (areas ordinal by AreaId, crops ascending TreeId)
        emit AddPending orders in plan sequence order
        if selected order set is empty:
          record "no intervention" and continue          # never call Approve on an empty plan
        else:
          r := POST plan/approve  -> record; assert living count and year unchanged
          record exact Approved.Orders before executing
          r := POST plan/execute  -> record receipt; assert Year and AnnualRevision unchanged,
                                     WorkRevision +1, felled cause == "fell", year == y
          record realised removals per area from the event's Before/After
      r := POST year/advance {consentToCancelWork:false}
      record r.Annual (per-area reports, null for carried-forward-empty), refusal code if any
      record projection(r.View) and the per-session ID checks
    stop server (SIGINT), assert clean exit and that nothing was written to the lab tree
```

**Never invent a command.** If a refusal is returned, it is logged with its code and the transcript continues from the returned fresh view. A refusal is a result, not an error to route around.

---

## 3. Frozen selection algorithm (pre-registered, single algorithm)

Implemented exactly as Q0 §6 steps 1–8 (revision 3). Restated for the implementer, with the parts that are easy to get wrong called out:

1. **Crop set.** Rank the area's living trees by DBH descending, ties by ordinal `TreeId`. Walk once, top to bottom; accept a tree as Crop if its float32 E3 distance to every already-accepted Crop is **> 8 m**. DBH order selects the Crop set only — it is **not** the later traversal order.
2. **Frozen competitor lists.** For every Crop, from the *same* intervention-time state and *before* any selection: living neighbours within 8 m, ranked by the Hegyi term `D_j / max(1, D_crop) / max(0.5, d)` in float32, descending, ties by ordinal `TreeId`. A tree on several Crops' lists stays on all of them. By step 1 a Crop cannot appear on any list; if one does, it is a reported defect.
3. **Removal sequence.** Per area, areas in ordinal `AreaId` order. Build by **rank layer**: layer *k* holds, for each Crop in **ascending ordinal Crop TreeId**, that Crop's rank-*k* competitor if it exists. Concatenate layer 1, layer 2, layer 3, … Then **deduplicate at first occurrence**. A dropped duplicate is **not** replaced — the affected Crop does **not** advance to its next competitor. Log every dropped duplicate with its Crop and layer.
4. **Intensity.** `L` = deduplicated layer-1 entries. `M` = deduplicated layers 1–3. Because layer 1 comes first in one sequence, every `L` entry is also an `M` entry.
5. **Command order.** (a) Crop marks **first** (`MarkCrop` refuses an already-planned tree; `AddPending` refuses a Crop-retained tree). (b) Then `AddPending` in sequence order. (c) If the selected order set across all treated areas is empty, do **not** Approve — record "no intervention" and continue. (d) Otherwise Approve, disclose, Execute. Crops are never ordered, so a populated area always keeps a living tree and `WorkWouldLeaveNoLivingStems` cannot be triggered by this rule.
6. **Disclosure before Execute** — mandatory, and it is the audit artefact a reviewer checks first.
7. **Realised intensity** from the engine's own per-area Before/After: removed stems, removed BA, removed fraction of both.
8. **"Genuinely different"** is a measured claim, not an assumption: `M`'s removed BA must be **strictly greater in both areas**. If not, report "not genuinely different in area X" and do not force it.

**Rule (b)** (removal from below by DBH rank) is **not used** — it would need a numeric threshold Q0 cannot justify.

**Timing.**

| Scenario | Definition |
|---|---|
| `C` | no-work control, no Crop, no orders, for the whole horizon |
| `L` | steps 1–8 at year 0, intensity L |
| `M` | steps 1–8 at year 0, intensity M |
| `D` | same rule as `L`, applied first at year 5 |
| `S` | `L` rule at year 0 **and** again at year 10; at year 10 the surviving year-0 Crops are kept (dead Crops' marks are cleared by the engine), competitor lists are re-frozen from the year-10 state, and only that year's deduplicated layer-1 entries are taken. **No new Crop is added at year 10.** |
| `S1` | equivalent single intervention: one year-0 intervention from the **same** Crop set, felling the first `n_A` / `n_B` entries of the **same** year-0 deduplicated removal sequence across all layers, where `n_A`/`n_B` are `S`'s **total realised stems removed per area across years 0 and 10**. Sized only from `S`'s realised counts, never from an `S1` outcome. Primary equivalence is removed stems per area, which is exact. BA, ID-identity and timing mismatches are reported, not hidden. If the year-0 sequence holds fewer than `n_X` trees, `S1` is "not constructible in area X" with the shortfall — no substitute. |
| `I` | isolation: steps 1–8 with L intensity on **MA-A only**; no Crop mark and no order in MA-B |

---

## 4. Horizons, repeats and lawful stops

- **Horizons:** capture **every committed year 1…H**. Years 1, 5, 10, 20 and 50 are summary checkpoints computed from that all-year log, not the only years recorded. `H = 50` unless a lawful stop occurs first.
- **Lawful stops** (any one ends that scenario, recorded as an outcome, never worked around):
  - both areas empty → next `Advance` refuses `ContinuationNoLivingStems`; the lawful horizon is the last committed year;
  - an arithmetic `checked` overflow (int32 year/revision/age, ~2.1·10⁹ years — not reachable at H=50, but recorded if raised);
  - the time/compute budget in §9 is exhausted.
- **Never** alter rules, seeds, the experiment definition or the horizon to obtain a longer run.
- **Repeats:** 2 fresh sessions per scenario (14 sessions). Comparison is by the §5 projection, never byte-identical full views — `SessionId` is random by design and is embedded in every `PlanId`, execution key and Fell record.
- **Reverse-order check:** each area's report is also compared against `MultiAreaAnnual.Step` run in reverse area order.
- **Stochastic replication (H-W6) is NOT in Q1 scope.** The canonical experiment has exactly one seed per area; a seed variant changes the content hash and is outside the approved canonical design. It may run only under a separate Manager authorisation and must be labelled "variant, not MA-1 canonical".

---

## 5. Determinism projection (the repeat gate)

Two fresh sessions must be equal under this projection at **every year and after every command**:

1. **Byte-equal:** `MultiAreaCommandResult.Annual` (including `null` for a carried-forward area); `View.Areas`; `View.Source`; `View.Experiment`; `View.PlanStateRevision`; `View.AnnualHistory`; every `RefusalCode`.
2. **Checked within each session, then excluded:** `SessionId` is 32 lowercase hex and constant, and equals every fence sent; every `PlanId` equals `SessionId + ":ma-plan:" + n` where *n* is the `PlanStateRevision` produced by that Approve; every execution key equals `PlanId + ":execute"`; every Fell record's `PlanId`/`ExecutionKey` equals its event's. Any violation is a **reported defect**, not a normalisation case.
3. **Normalised then compared:** replace the exact leading `SessionId + ":"` of each `PlanId` and key with `SESSION:` in `Pending`, `Approved`, `Events` (with Fell records) and every `Execution` result; require byte equality. Nothing else is rewritten.
4. **Serialisation:** `System.Text.Json` `JsonSerializer.SerializeToUtf8Bytes` with default options, same source SHA, build and runtime. Collection order is the engine's own (areas by `AreaId`, rows/rosters by `TreeId`, history/events in commit order, Fell records by `TreeId`). **The harness must not re-sort.**
5. **Float representation:** every float32 is logged as its 32-bit pattern (8 hex) and every double as its 64-bit IEEE-754 pattern (16 hex). Equality is bit equality. A JSON-byte mismatch with all bit patterns equal is reported as a serialiser difference, not a biological one.

---

## 6. Measurements and null semantics

**Per tree, per committed year, per area** (from every returned report): `TreeId`, age, DBH, height, crown radius, pre-growth CI (m⁻¹), neighbour count, suppression `S`, death probability `p` (per annual step — **not** a hazard rate), roll `u`, death year/cause/mechanism.

**Per area, per committed year:** living count, hazard deaths, boundary deaths, cumulative natural deaths, cumulative Fell, extinction year, first carry-forward year (each separately, or "none"), stems/ha, area-normalised BA m²/ha, Dq, arithmetic mean DBH (harness-computed), RD, density pressure, mean vulnerability.

**Null discipline (mandatory).**
- `null` means **not evaluated**, never zero. `S`, `p`, `u` are `null` when `P ≤ 0`; `u` is `null` when `p` is `null` or 0. `p = 0` is a real evaluated zero.
- Dead carried audit rows hold placeholder `CI = 0` and neighbours `0`; these are logged "not evaluated", not measured zeros.
- **Extinction year:** the report is non-null with `After.LivingCount == 0` and history `NoLivingStems = false`; that year has normal per-tree rows.
- **Carry-forward year:** a *later* committed year has `Report = null` and `NoLivingStems = true`; one "carried forward, no per-tree audit" row, zero per-tree rows. This is a documented state, not missing data.
- Means over an empty set are `null`, never 0.

**Integrity checks per scenario.**
- Living count is non-increasing year on year (no recruitment exists). Any increase is a **defect report**, not a biological finding.
- No-double-application: every Fell ID appears once in the ledger and dies once; Fell + natural deaths = genesis − living.
- Chronology per intervention: Approve leaves living count and year unchanged; Execute keeps `EcologicalYear` and `AnnualRevision` and moves `WorkRevision` +1; felled trees carry `Death.EcologicalYear == y`, cause `fell`; the Year+1 `Before` equals the event's per-area `After`; felled IDs are absent from every neighbour list in that step; natural deaths carry year y+1.
- Isolation: `I` versus `C` must have byte- and bit-equal MA-B `View.Areas[B]`, `Annual.Areas[B].Report` and every history `Areas[B]` entry at every year. Global fields that legitimately differ (`WorkRevision`, `PlanStateRevision`, the event ledger, `CancelledUnexecutedWork`, `SessionId`) are excluded and listed.

**Valid-case boundaries, each attempted and recorded:** fell-to-empty refusal (approval stays), Approve-then-Cancel (0 Fell, PlanState +1, orders not returned), consented advance with pending work (0 Fell, `CancelledUnexecutedWork = true`), replay of an executed key (`Replayed = true`, no change), and the all-empty advance refusal. The last is **not reachable on canonical data by player action** and must be reported as "not exercised on canonical data" unless natural mortality produces it.

**Valid comparisons.**
- **Retained-tree growth** — the matched retained set for [t₀, t₁] is the `TreeId`s living at both t₀ and t₁ in **every** compared session and never felled in any. Report `ΔD` per tree and the paired `ΔD_L − ΔD_C` (and M−C), plus the set's size and share of genesis.
- **Selection-only shift** — at each Execute, with year unchanged and no growth: mean DBH and Dq of post-Fell minus pre-Fell living set, per area.
- **Stand mean DBH** is reported only *alongside* both components and is **never used alone** as growth-response evidence.
- **Frenchpark comparison:** only the **sign** of the matched retained-tree increment difference, with an explicit statement that these are synthetic stands, not yield plots.

---

## 7. Diagnostics (not validity thresholds)

These are pre-registered so they cannot be chosen after seeing results. All are **diagnostics only**; none establishes empirical validity.

| ID | Check | Decision rule |
|---|---|---|
| H-W2(a) | CI invariance to uniform proportional DBH scaling | Offline, on logged rosters of C and M: recompute CI, then with all DBH × λ, λ ∈ {1.1, 1.5}. **Reject** if any tree with `λ·D_i ≥ 1 cm` has `|CI_λ − CI| > 1·10⁻⁵ · max(CI, 1 m⁻¹)` |
| H-W2(b) | ΔD is a function of pre-growth D and CI only | Recompute ΔD from logged D and CI in float32 in coded order; **reject** if any `ΔD_obs` differs by more than 1 ULP of D(t+1) |
| H-W3 | hazard vs boundary dominance | Per scenario, natural deaths by mechanism per 10-year window and whole horizon; **flag** if boundary deaths exceed 50 % of natural deaths in a window with ≥ 10 natural deaths. Windows with fewer deaths reported, not flagged. A "hazard-dominated" result does **not** validate the [C] constants |
| H-W4-CI | edge trees have lower CI (8 m boundary exclusion) | Session C; each area separately; t = 0 and t = 10 separately; stratified by genesis cohort (MA-A: 24; MA-B: 29 and 47) → 6 stratum-years. Fit `CI = a + b·edge + c·ln(D)` by OLS in double. Per stratum-year vs `m_int`: **BIAS** if `b<0` and `|b| > 0.20·m_int`; **SMALL** if `b<0` and `|b| ≤ 0.20·m_int`; **NO BIAS** if `b≥0`; **NOT ESTIMABLE** per shared rules. Overall: CONFIRM if all 6 BIAS; REJECT if all 6 NO BIAS; INCOMPLETE if any NOT ESTIMABLE; MIXED otherwise |
| H-W4-G | edge trees have larger 10-year ΔD | Same strata; retained set in C from year 0 to year 10; fit `ΔD = a + g·edge + c·ln(D₀)`. **GROWTH ADVANTAGE** if `g>0` and `g > 0.20·ΔD_int`; **SMALL**; **NO ADVANTAGE** if `g≤0`; **NOT ESTIMABLE**. Overall as above. Reported **separately** from H-W4-CI; neither implies the other |
| W5 | height envelope ratio drift | Expect `|ρ(t)/ρ(0) − 1| ≤ 8·t·2⁻²⁴` per living tree; trees at the 0.1 m / 60 m clamp excluded and listed. Larger drift = defect in the E6 description |
| W11 | retention | Expected per-tree rows = Σ over non-null reports of audit-row count (incl. dead carried rows); `null` report contributes 0 rows + 1 marker. Exact match required. Plus transcript replay equality |
| W12 | README drift | Run both documented CLI forms of `annual` once; record exit code and output. Runtime outcome unknown until run |

**H-W4 shared rules.** `NOT ESTIMABLE` if fewer than 2 edge or 2 interior trees (retained trees for G), or the n×3 design matrix is rank-deficient (smallest singular value ≤ 1·10⁻⁹ × largest), or the denominator ≤ 0 — each reported with counts and reason, and never counted toward CONFIRM/REJECT. **No p-values** (census, not sample). Trees are not independent replicates; years are not pooled. The `ln(D)` term is a *chosen adjustment*, not proof that DBH confounding is removed, and no causal claim is made.

---

## 8. Result bundle and publication

```
/home/jer/Development/CCF-885-Qualification-Scratch-20261121/<run-id>/
  harness/            q1-driver.sh q1-select.py q1-record.py q1-analyse.py q1-replay.sh SHA256SUMS README.md
  header.json         source SHA, manifest content hash, harness hashes, dotnet/node/kernel, host, worktree
  sessions/<scenario>/r<repeat>/transcript.jsonl     every command, argument, fence, response projection
  sessions/<scenario>/r<repeat>/reports/year-NN.json per-area reports, raw
  sessions/<scenario>/r<repeat>/view-year-NN.json    projections
  sessions/<scenario>/r<repeat>/disclosure-*.json    Crop lists, frozen competitor lists, sequences, dropped duplicates, Approved.Orders
  analysis/           trajectories.csv per_tree_year.csv fells.csv diagnostics.md anomalies.md
  logs/               server stdout/stderr per session, every bound port
```

**Publication:** compact machine-readable CSVs and a GitHub Markdown summary with source-linked tables and trajectory figures. Raw per-tree logs and server stdout **stay in the scratch directory** and are enumerated by path and SHA-256; they are not silently committed. Publication limits are stated explicitly.

---

## 9. Budget, timing and stop conditions

- **Unknown until measured:** total wall-clock for 14 sessions × up to 50 annual steps. The UI process handles one year per POST, so the cost is dominated by HTTP round-trips plus the engine's O(n²) neighbour scan (MA-A 171 trees, MA-B 73 — trivial per year).
- **Budget:** stop a scenario if a single year takes > 60 s or a whole scenario exceeds 30 min; record the stop, the last committed year and the cause. Never extend the horizon or drop the repeat silently to fit a budget.
- **Abort the whole run** on: manifest content-hash mismatch, any projection mismatch between repeats, a `500` unconfirmed outcome, an unexplained living-count increase, or any evidence of a second writer in the lane.
- Every abort is reported as **INCOMPLETE** with concrete evidence. A blocked run is a legitimate, publishable outcome.

---

## 10. Permissions still required before any run

| # | Requirement | Owner | Status |
|---|---|---|---|
| 1 | Exact **integrated and technically qualified** SHA from Stage I | #885 integrator (GPT-6.1 Sol) | **PENDING** — Stage I running |
| 2 | `blk-885-q1-port-pin` resolved: qualification lease reads "loopback only, OS-assigned ephemeral port, every bound port recorded" | D-091 physical custodian | **BLOCKED** — new blocker raised by this draft (§1.2) |
| 3 | Q1 execution packet published **APPROVED TO RUN** on a new docs-only path | #885 Manager | **PENDING** — this draft is not it |
| 4 | Distinct qualified run worker, not the integrator, not the reviewer, not the orchestrator | orchestrator routing + Manager | **PENDING** |
| 5 | Scratch `/home/jer/Development/CCF-885-Qualification-Scratch-20261121` created and allocated | D-091 custodian | RESERVED, unused |
| 6 | Harness bytes disclosed with SHA-256 before the run | run worker | **PENDING** by design |
| 7 | No repository write, no protected file, no Unity, no Windows build, no `main` merge | all | ENFORCED |

---

## 11. Claim ladder

At every stage the programme distinguishes: **PRODUCED / REVIEWED / INTEGRATED / VERIFIED / OWNER ACCEPTED**.

- This document: **PRODUCED** (draft), not reviewed, not authorised.
- After Stage I: the integration result may claim INTEGRATED (branch ref) and technically VERIFIED for the stated gates only.
- After Stage Q: results are **PRODUCED** until the independent reviewer returns.
- After Stage R: technically **REVIEWED** for experiment qualification only.
- **OWNER ACCEPTED: never asserted by any agent in this programme.** G2 is the owner's own playtest and remains pending.

---

## 12. Ledger and continuity

Portable-fallback routing ledger (Runtime routing service not exposed by this host, so `canonical_persistence: false`):
`/home/jer/Development/ccf-885-orchestration/routing-ledger.jsonl` — append-only, 38 rows at the time of writing, labelled `store: portable-fallback`, `runtime: unavailable`. The #885 Manager declined separate publication of this ledger and directed that the local file be preserved without rewriting. Continuity therefore rests on this ledger plus the durable GitHub chain of programme documents.

Related decision ids: `dec-885-stage1-integ-20261121T1648Z` (Stage I), `dec-885-q1-20261121T0923Z` (earlier Q1 NOT-READY evaluation, superseded in part by the D-091 admission).
