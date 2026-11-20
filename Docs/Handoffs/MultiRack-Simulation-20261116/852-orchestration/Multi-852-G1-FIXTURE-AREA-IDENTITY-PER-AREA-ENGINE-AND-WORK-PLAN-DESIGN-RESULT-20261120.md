# Multi #852: G1 multi-area fixture identity, per-area engine and cross-area Work Plan design (result)

**20 November 2026 | Issue [#852](https://github.com/jermur788/CCF/issues/852) | Decision owner: Multi Manager [#730](https://github.com/jermur788/CCF/issues/730) | DESIGN RESULT, READ-ONLY. Not a spec approval, implementation, fixture or test result.**

## 0. Verdict

**CONDITIONAL GO for a first, comparator-free implementation packet (MA-1). NO-GO for positive G1 comparison (MA-3) until #851 supplies real evidence and Q-D is answered.**

- **Design-ready now (PROPOSED):** a separate versioned multi-area fixture family with its own catalog, globally unique area and tree identities, per-area seeds, per-area annual ecology that calls the unchanged `AnnualRules.Calculate` once per area, and one cross-area Work Plan that commits all-or-none. This design needs **no edit to any existing source file** apart from one test-registration line.
- **Buildable only after owner answers:** MA-1 needs Q-A (a labelled synthetic teaching fixture without comparability claims), Q-C (isolated per-area ecology) and Q-E (all-or-none in one in-process root). §6 gives the options.
- **Blocked:** a COMPARABLE result for ≥2 targets. No real between-area feature exists, and no tolerance has been accepted. MA-2 can ship a feature extractor that returns UNKNOWN for every target and keeps Manual/NotCovered. That is honest, but it does **not** satisfy #828 R-02/R-05 or X-01/X-03. The ≥2-target requirement is kept. It is not lowered to get a green gate.

Every interface, key name and rule below is **PROPOSED**. Every test is **NOT RUN**. No source, fixture, build, .NET, Unity or test was touched.

## 1. Actor, inputs and boundaries

| Item | Value |
|---|---|
| Actor | Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), Claude Code in T3 Code. Lead designer per the #852 dispatch. Separate from the #849 author thread. No subagents; no Sonnet or Sol work. |
| Host | DESKTOP-DL1BQI7, Windows 11 reading WSL2 Ubuntu `jer`. Method: `git show`, `git ls-tree`, `git diff --stat` and `gh api` reads only. |
| Claim | [#852 RUNNING registration](https://github.com/jermur788/CCF/issues/852#issuecomment-7037669241), 2026-11-20 10:51 UTC |
| Main observed | `cba0a905b766ce8fc0356d351ac5bff1c2a9a008` (#849 claim closed as HANDED-OFF). No #852 row on the board. |
| Branch / path | `docs/Multi-G1-Fixture-852-area-identity-per-area-engine-design-20261120-v1` from `cba0a905`. This one Markdown file is the only write. |
| Design baseline | **`4c42e2012a4c1bee3a422e41a58535343d63ac9f`**: the #849 Manager-G2-approved source on `feat/multi-standmap-playable2d-20261120-v1`. A-1 ancestor `4bb80973…`. |
| Consumed, not repeated | #842 result blob `87d0b56d8e97d5405a1e92506ad9a13e67b906b7`; #843 result blob `a602dd876ce23fc19b153f5f077513229b5e7098`; #828 DRAFT branch head `2cd025504066823fc9116a57c724e40f7b2bb77e`; [#849 Manager G2 verdict](https://github.com/jermur788/CCF/blob/docs/Multi-849-Manager-G2-Scope-Acceptance-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-849-Manager-G2-Source-and-Standalone-UI-Scope-Acceptance-20261120.md) |

**Baseline change from the dispatch.** The dispatch said to pin A-1 and take a fresh delta after #849 qualified. #849 has now qualified: Manager G2 approved `4c42e201`, and the claim closed at main `cba0a905`. This design is therefore written against `4c42e201`, which contains A-1 unchanged plus the 15 #849 paths. Paths below are relative to `Tools/MultiRackSitkaP1Lab/` at `4c42e201`.

**Fact the future packet must respect:** `Tools/MultiRackSitkaP1Lab/` **does not exist on `main`**. `git diff --stat 4c42e201 origin/main` shows 46 lab files absent. Any implementation branch must start from the feature branch or `4c42e201`, not from `main`.

## 2. Source delta since #842 that matters (not a full re-audit)

#842 audited `y2engine` HEAD `6f9d2e99`. The relevant changes up to `4c42e201` are:

| Area | Fact at `4c42e201` | Design consequence |
|---|---|---|
| One commit root | `AnnualContinuation` holds one `WorkRoot` (`AnnualContinuation.cs:16-24`) under one `sync` lock (`:32`). Roster, both revisions, event ledger, executed-key map, pending, approved plan, plan-state revision and annual history are published by **one reference assignment** (`:235-236`, `:297-303`). | All-or-none is proven for one area **because** of the single root. Several `AnnualContinuation` instances cannot be committed together: each `Execute` publishes inside its own lock, so a fault in area 2 after area 1 publishes leaves a partial batch. A multi-area session needs **one root covering every area** (§5). |
| Token | `WorkSourceToken` carries one `FixtureId`, one `ManagementScopeId`, one year/annual revision/work revision (`WorkContract.cs:29-41`). `Token()` always writes `ManagementScope.WholeUndividedThirtyMetres` (`AnnualContinuation.cs:307-311`; const at `WorkContract.cs:19-22`, whose comment anticipates "a later multi-area slice"). | A multi-area plan needs a new token type naming the experiment and every area (§5.1). The existing token stays untouched. |
| Execute rules | Duplicate ID, not-live ID and stale-source refusals all run before the publish (`:186-198`). `beforePublish` is the test fault seam (`:226-227`). Replay with the same key and payload returns the receipt (`:169-176`). The player facade mints the PlanId (`:370`) and execution key (`:474`) and refuses unminted keys (`:414-416`). | MA-1 copies these rules one for one, across areas. |
| Non-empty rule | `WorkRules.RequireNonEmptyStand` refuses a batch that would leave **the stand** empty (`WorkContract.cs:143-152`, D-093 P2). | For several areas, a per-area or whole-experiment reading needs a decision (D-4, §6). |
| Advance | Unexecuted work needs explicit consent and is then cancelled at zero Fell (`AnnualContinuation.cs:260-271`). An advance with no living stems is refused (`:283-284`). | Keep consent unchanged. Per-area empty handling: D-5. |
| Shared helpers | `AnnualRules.Calculate` is `internal static` and pure (`AnnualRules.cs:13`). `WorkRules.Summarize` is internal (`WorkContract.cs:122`). `StrictJson`, `J` and `CanonicalWriter` are public (`StrictJson.cs:23,111,150`). | New code inside the same `SitkaP1.Core` assembly can call all of them without changing them. |
| Build graph | `SitkaP1.Core.csproj` has no explicit `Compile` list for lab code, only the two linked CCF files (`csproj:26-29`). SDK globbing compiles every `.cs` below `src/Core/`. The test project also globs (`tests/Core.Tests/SitkaP1.Core.Tests.csproj`). Tests are registered in `tests/Core.Tests/Program.cs:440-442`. | New files under `src/Core/MultiArea/` and `tests/Core.Tests/` compile **with no csproj edit**. Only the test registration lines change. |
| Domain gate | `P1Domain` has three values (`Domain.cs:7-15`). `DerivedRecordFor` refuses IDs without the `p1d-` prefix (`FixtureAccess.cs:114-115`) and every domain other than the 30 m one (`:118-119`). `AnnualEngine.RequireAnnualDomain` admits only through the `AnnualGate` sentinel (`AnnualEngine.cs:57-63`; `FixtureAccess.cs:148-159`). | Recommend **no new `P1Domain` value** (§3.1). The new family uses its own domain string and its own gate, and the existing parsers refuse it by construction. This also removes the #842 §5 "MINIMAL `Domain.cs`/`FixtureAccess.cs`" item. |
| RNG | `SimulationRandom.Roll(model, id, year, seed)` hashes the whole ID string with FNV-1a, then mixes with year and seed using splitmix64 (`Assets/ForestPrototype/SimulationRandom.cs:41-55`). The mortality key is `"ADULT-MORT-" + TreeId` (`AnnualRules.cs:43-44`). | With globally unique IDs, each tree's stream is distinct even under one seed. Distinct per-area seeds are still required as defence in depth (§3.4). |
| Site | `Grow` reads `DerivedManifest.SiteProductivityBits` (1.0f) as a constant (`AnnualRules.cs:89`). | Unchanged from #842. Site cannot vary per area without editing a protected file. |

Protected baseline at `4c42e201`: **92/92 Core tests** (66 original, 20 PlayerSession, 6 MapScene), **137/137 UI smoke**, and ten no-work Year 1–10 digests (`tests/Core.Tests/ContinuationTests.cs:11`). The dispatch's "66/66" is now a subset of the 92.

## 3. F1: distinct experiment identity, manifest and geometry (PROPOSED)

### 3.1 Family separation

| Item | Original P1 (unchanged) | New multi-area family |
|---|---|---|
| Format string | `ccf-sitka-p1-derived-fixture` (`DerivedManifest.cs:31`) | `ccf-sitka-multiarea-fixture` |
| Schema version | 1 | 1, in its own family |
| ID prefix | `p1d-` (`:34`) | `mad-` (refused by `FixtureAccess.cs:114`, so it can never load as P1) |
| File suffix | `.sitkap1fx.json` | `.sitkamafx.json` |
| Catalog | `DerivedCatalog`, version `p1-f0-derived-catalog-v1`, `Digest()` (`TrustTables.cs:91-120`) | New `MultiAreaCatalog`, version `ccf-multiarea-catalog-v1`, its own `Digest()`. `DerivedCatalog.Records` and its digest stay byte-identical. |
| Domain | `P1Domain` enum | String `MultiAreaSitkaExperimentV1`, held in new code. `P1DomainWire.Parse` refuses it (`Domain.cs:28-34`), and that refusal is a negative test. |
| Folder | `fixtures/derived/` | `fixtures/multiarea/` |
| Disclosure | `P1Disclosure.Derived30` | New constant, for example: "SYNTHETIC MULTI-AREA TEACHING EXPERIMENT. AREAS ARE AUTHORED, NOT SURVEYED. NO AREA IS CLAIMED COMPARABLE. NOT AN IRISH FIELD FORECAST." |

Trust follows the existing pattern: `PendingIndependentReview` until an independent review and a Manager acceptance are cited, and the author never sets `Accepted` (`TrustTables.cs:5-10`).

### 3.2 Root keys (exact set; any extra or missing key is refused)

`format`, `schemaVersion`, `experimentId`, `catalogVersion`, `generator` {`id`, `version`, `seed`}, `localFrame` {`description`, `originRule`, `xzTransformId`}, `models` {`rngModelVersion`=1, `growthModelVersion`=2, `algorithmVersion`, `siteProductivityInEngine`=1.0f bits and the fixed text "engine constant; per-area site values below are NOT active"}, `initialEcologicalYear`=0, `sourceInventoryYear` (null or int, with provenance), `provenance` {`class`, `sourceRef`, `statement`}, `disclosure`, `areas`[], `contentHash`.

`contentHash` = SHA-256 of the canonical bytes written by `CanonicalWriter` with `contentHash` set to empty, which is the same sealing idea as P1. Key order is fixed by the writer. The parser applies a size cap, strict JSON (no BOM, no duplicate keys) and exact key sets via `J.Keys`.

### 3.3 Area keys

| Key | Rule |
|---|---|
| `areaId` | `^MA-[A-Z0-9]{1,16}(-[A-Z0-9]{1,16}){0,3}$`, ordinal-ascending, unique. **Neutral names only** (for example `MA-A01`). Role names such as "sample", "target" or "control" are **forbidden in the fixture**, because role is a per-treatment, per-year judgement (#843 §1 read-across), not a property of an area. |
| `polygon` | ≥3 int64-mm vertices in the experiment frame, counter-clockwise, implicitly closed, no repeated vertex, no self-intersection (exact integer orientation tests). Every coordinate in `[0, 10 000 000]` mm, so products stay inside int64. |
| `areaM2Exact` | Exact rational from the integer shoelace sum: `|Σ| / 2` mm² ÷ 10⁶. Stored as a decimal string and recomputed by the parser. |
| `areaHa` | f32 bits of `(double)(|Σ| / 2) / 1e10`, one cast. Recomputed and compared bit for bit. This is the engine denominator for this area only. |
| `simulationSeed` | int32. Pairwise distinct across areas (§3.4). |
| `site` | Object with `siteIndex`, `yieldClass`, `soil`, `drainage`, `exposure`, `slopeDeg`, `elevationM`, `windthrowHistory`, `thinningHistory`, `restrictions`. Each is `{ "value": … or null, "class": "measured" or "externally-estimated" or "synthetic" or "unknown", "sourceRef", "resolution", "year", "uncertainty" }`. `null` with class `unknown` is the **default**. A missing value **means UNKNOWN, never 1.0 or "equal"**. The parser refuses `value != null` with class `unknown`, and `value == null` with any other class. **None of these values reaches the engine** (§4.4). |
| `trees`[] | Exact keys `treeId`, `xMm`, `yMm`, `ageYears`, `dbhMm`, `heightMm`, plus `provenance` {`xy`, `dbh`, `height`, `age`} each with a class from the same four values. Ranges as P1 (`DerivedManifest.cs:60-63`). Crown radius is derived by the existing rule (`:54-55,80-85`), not stored as a measurement. Every tree is live at genesis. |

**Engine-essential tree fields can never be `unknown`:** the engine cannot run a tree without DBH, height, age and XY. If a real source lacks them, that tree is out of the fixture. Its value is not invented.

### 3.4 Identity and RNG isolation rules (each one is a parser refusal)

1. **Global tree IDs.** `treeId` matches `^[A-Za-z0-9_-]{1,64}$` (same `SafeId`, `DerivedManifest.cs:65`). It is unique across the **whole experiment** by ordinal comparison **and** by `OrdinalIgnoreCase`, so a case-shifted twin such as `MA-A01-t001` / `MA-A01-T001` is refused. IDs are ordinal-ascending within an area.
2. **Area-prefixed IDs.** Every `treeId` starts with its own `areaId` + `-`. This makes cross-area mislabelling detectable without a lookup. (PROPOSED convenience; the global uniqueness rule is the real guard.)
3. **No P30 reuse.** Any ID matching `^P30-` is refused, so the original roster can never be re-keyed or split into "areas" (#842 §2, N-9).
4. **Membership.** Each tree point is strictly inside its own polygon. A point on an edge is refused, which avoids ambiguous ownership. A tree lies in exactly one area.
5. **No overlap.** Area polygons must not overlap in positive area (exact segment-intersection plus containment tests). Shared edges are allowed and have **no ecological meaning** under the isolated model (§4.3).
6. **Distinct seeds.** `simulationSeed` must be pairwise distinct across areas. With unique IDs this is defence in depth: no two areas can draw from one `(id, year, seed)` stream even if a later rule shortened IDs.
7. **Duplicate XY** within an area is refused, as in P1 (`DerivedManifest.cs:318`).
8. **Generator honesty.** `generator.id` names the actual new generator, never `p1-f0-square30-generator`. This prevents the misattribution #842 §3a(a) warned about.

### 3.5 First fixture proposal for MA-1 (labelled synthetic; NOT authored here)

If the owner picks Q-A option A2 (§6), MA-1 ships one fixture, `mad-sitka-teaching-5area-v1`, with five neutrally named areas of different shape and size, roughly 0.06–0.25 ha. Each is generated by a separate seeded rule with a deliberately different stocking, size distribution or cohort age. All tree fields carry class `synthetic`. All site fields are `unknown`, and the disclosure says so. Size estimate: the P1 fixture is 32,567 bytes for 100 trees (~326 B/tree). About 450 trees with provenance blocks comes to roughly **180–220 KB of JSON text**, one file, no binaries.

The fixture carries **no role labels and no comparability claim**. Any later assessor labels (S/T1/T2/D/U, #843 §3) belong in a separate holdout file owned by the assessor, so the engine fixture cannot leak labels (#843 N-09).

## 4. F2: per-area annual dynamics (PROPOSED)

### 4.1 State

One `MultiAreaRoot` (§5.2) holds, for each `areaId` in ordinal order, an `AnnualState` (`AnnualEngine.cs:15`) with that area's trees only. It also holds the area's `areaHa` (f32) and `simulationSeed`, plus **one shared ecological year** and **one shared annual revision**. Areas advance in lock-step: there is no per-area year.

### 4.2 Year advance

```
for areaId in ordinal order:
    report[areaId] = AnnualRules.Calculate(area.State, area.Seed, area.AreaHa)   // unchanged rule
    next[areaId]   = state from report[areaId].Trees.After
beforeCommit?.Invoke()           // test seam: everything calculated, nothing published
root = root with { every area's next state, revision+1, history += per-area AnnualYearRecord }
```

- **Area-local metrics:** `Calculate` computes count, N/ha, BA m²/ha, Dq and RD from that area's own trees and denominator (`AnnualRules.cs:102-118`). Density pressure (`:20`), hazard mortality (`:35-56`) and boundary thinning (`:120-142`) are therefore area-local. Units come from the source: N in stems/ha (double), BA in m²/ha (double), Dq in cm, RD dimensionless. Tree values are f32 cm and m, and comparisons use bit words.
- **Competition:** Hegyi CI (8 m cutoff, `AnnualRules.cs:6-7,67-82`) only sees trees in the same state, so neighbours across an area edge are ignored.
- **Natural death:** keeps the existing causes `self-thinning`/`hazard` and `self-thinning`/`boundary` (`:49,138`), recorded per area.
- **Determinism:** each area's result depends only on its own state, seed and denominator, so area order cannot change any number. A test proves this by running in reverse order and comparing bytes (MA-E-03).
- **Error handling:** any exception in any area's calculation propagates before the publish. The root is unchanged, which is the same guarantee as `AnnualContinuation.cs:294-303`.

### 4.3 Edge interactions: isolated ecology (PROPOSED, not owner-adopted)

**Recommendation:** isolated per-area ecology. Physical adjacency has no ecological effect, and this is disclosed in the UI and the disclosure string.

**Why this is the only route that needs no protected change:** `Calculate` takes one `AnnualState`, and `Competition` is private (`AnnualRules.cs:67`). Edge-coupled competition would need either an edit to `AnnualRules.cs` (protected; it carries the Year 1–10 oracle) or a second competition implementation in new code. The dispatch forbids a second biology engine. So the adjacent model is **gated** behind a separate, oracle-safe engine task. Its cost is a disclosed simplification: compartments with a shared edge do not shade or crowd each other.

### 4.4 Site

The engine keeps `site = 1.0f` for every area (`AnnualRules.cs:89`). Per-area `site` values are **descriptive data for the comparator only** and are **inactive in growth**. The fixture states this explicitly (§3.2 `models`). Making site active per area means changing `Grow`, which is a protected, oracle-carrying change and a separate gated task (Q-B).

### 4.5 Equivalence oracle (the strongest regression guard)

A multi-area session opened with **one** area built from the original accepted P1 fixture must produce, for Years 1–10, `ContinuationSnapshot` + `AnnualReport` bytes whose SHA-256 values equal the ten digests at `ContinuationTests.cs:11`. Only the area ID is new, and it is excluded from the hashed payload. The original fixture is loaded read-only through the existing `FixtureAccess.ReadDerived`. This proves that the per-area runner is the accepted engine, not a re-implementation (MA-E-01).

## 5. F3: cross-area Work Plan and atomicity (PROPOSED)

### 5.1 Contracts (new types; existing `WorkContract.cs` unchanged)

| Type | Fields |
|---|---|
| `MultiAreaSourceToken` | `Domain` (`MultiAreaSitkaExperimentV1`), `ExperimentId`, `FixtureFileSha256`, `FixtureContentHash`, `CatalogVersion`, `AreaIds` (ordinal list), `EcologicalYear`, `AnnualRevision`, `WorkRevision`, `RngModelVersion`, `GrowthModelVersion`, `AlgorithmVersion`, `ActionEligibleDbhThresholdMm`. Compared with value equality. A list needs an explicit sequence comparison, because C# record equality on `IReadOnlyList` is reference equality. Implementation trap: must be tested (MA-W-14). |
| `MultiAreaWorkOrder` | `AreaId`, `TreeId`, `WorkAction.Fell` (reuses the existing enum), `Reason`, `Origin` = `Manual`, or `ApprovedPrescription(PrescriptionId, PrescriptionRevision)`, which MA-1 refuses because there is no prescription stage yet |
| `MultiAreaWorkPlan` | `PlanId` (engine-minted), `Source` token, `Orders` |
| `MultiAreaWorkEvent` | `ExecutionKey`, `PlanId`, `EcologicalYear`, `WorkRevision`, per-area `{AreaId, Before, After}` `AnnualStand`, `Fells` (existing `FellRecord` plus `AreaId`) |
| `MultiAreaFrame` | Token, per-area `{AreaId, AreaHa, Trees, Stand}`, events and history, all built from one root |

### 5.2 One root, one lock, one publish

`MultiAreaContinuation` mirrors `AnnualContinuation` (§2), with one immutable root:

`MultiAreaRoot(Year, AnnualRevision, WorkRevision, Areas[ordinal] {AreaId, State}, Events, Executed map, Pending, ApprovedUnexecuted, PlanStateRevision, AnnualHistory)`

**Execute, all-or-none:**

1. Under `sync`: session check, then the A-2 replay rule (same key + same PlanId + same source → receipt; different payload → `WorkExecutionKeyConflict`), then the fence (token + plan-state revision), then the minted-key check. Same order as `AnnualContinuation.cs:390-418`.
2. Validate **every** order across **every** area before any change: the area exists in the token; the tree exists, is live and has the exact case; the tree really belongs to that `AreaId` (`MultiAreaWrongArea` otherwise); no duplicate tree ID; the non-empty rule (D-4).
3. Build every area's new roster, the per-area before/after `AnnualStand` via `WorkRules.Summarize`, and the one event.
4. `beforePublish?.Invoke()` is the test seam. The **MA-1 addition** is a second seam, `afterAreaBuilt(areaId)`, called after each area's roster is built, so a test can fault mid-batch between areas.
5. **One reference assignment** publishes every area's roster, the event, `WorkRevision+1`, the executed-key entry and the cleared approval.

**Why this is atomic:** before step 5 nothing reachable has changed. Step 5 is one reference write under the lock. Readers take the same lock and read one root (`AnnualContinuation.cs:338-341` pattern). So no reader can see area A felled while area B is not. This is the same argument Sol's Tier-2 review accepted for one area in #849, applied to a root that covers every area. **Limits (not claimed):** in-process only. No crash durability, save/restart or cross-process recovery (G3 stays deferred). The engine is not concurrent across processes.

### 5.3 Proposal → prescription → PendingWork → approval → Execute

- **Manual (MA-1):** `AddPending(fence, areaId, treeId, reason)` validates directly, as #849 does. One `Approve` covers the whole pending set across all areas and fells nothing. `Execute` is separate.
- **G1 (MA-3, blocked):** a comparator proposal is a `ProposalRecord` held **outside** the root. It never writes PendingWork and never kills a tree. A player accept creates a draft prescription. A separate prescription review and approval turns it into orders with `Origin = ApprovedPrescription`. Any edit to the proposal invalidates the prescription. Those orders then join the same pending set and the same single whole-plan approval.
- **No per-area success masking:** the API returns one result for the whole plan. The UI must show the plan as one unit, with no per-area "done" tick before the single event exists.

### 5.4 Stale, year and replay rules

- Any Fell in any area, or any year advance, changes the one token, so every approved plan goes stale. That is the intent: one plan, one source moment.
- Year advance with unexecuted work needs consent and cancels **every** area's orders at zero Fell, exactly as `AnnualContinuation.cs:260-271` does.
- Executed keys, events and history are scoped to the experiment and record `AreaId` per Fell, so history reads per area.
- **Regression guard:** the existing 92 Core tests and 137 smoke checks run unchanged and must pass. `AnnualContinuation`, `PlayerWorkPlanAdapter` and the #849 UI are untouched, so the single-area game keeps working exactly as approved.

## 6. Owner and Manager decisions (#842 Q-A…Q-E, plus three small design decisions)

These are **not self-approved**. Each has a recommended option, and what it costs to defer.

| # | Question | Options | Recommendation (conditional) | Cost / risk of deferral |
|---|---|---|---|---|
| **Q-A** | Source of area data | **A1** wait for real comparability data (#851). **A2** labelled synthetic teaching fixture with **no comparability claim**, used only for identity, engine and Work Plan. **A3** synthetic designed-comparability fixture labelled as such. | **A2 for MA-1 now; A1 for MA-3.** A2 needs no evidence because it claims none. A3 is the tautology #842 §3c(1) rejected. | Deferring A2 blocks all multi-area engine and Work Plan work, although none of it depends on comparability. |
| **Q-B** | Site / yield class | **B1** required feature, so every area is UNKNOWN until the engine has per-area site. **B2** recorded with provenance but inactive in growth; site-sensitive treatments stay UNKNOWN. | **B2.** An active per-area site needs a protected `AnnualRules.cs:89` change in its own oracle-safe task, only if #851 finds real site data. | None for MA-1. Without real site data, any site-sensitive treatment stays UNKNOWN anyway. |
| **Q-C** | Edge ecology | **C1** isolated per area. **C2** cross-boundary competition. | **C1** (§4.3). C2 needs a protected or duplicate biology change. | MA-1 cannot fix its engine semantics without this answer. |
| **Q-D** | Comparator predicate and tolerance | #851 data plus an **independent forestry assessor** proposes; Manager adopts; holdout per #843 §3. | Defer. **No tolerance now.** MA-2 ships UNKNOWN-only. | Positive G1 stays NOT TESTABLE. This is the honest state. |
| **Q-E** | Whole plan all-or-none across areas | **E1** yes, one root, in-process (§5.2). **E2** per-area plans only. | **E1.** It matches #828 §3.5 and has a concrete mechanism proof sketch plus tests MA-W-08…10. | E2 would weaken #828 R-11/R-12. |
| D-4 | D-093 P2 non-empty rule | per area, or whole experiment | **Per area.** A batch that would empty **any** area is refused as a whole. | A minor policy extension, so the owner should confirm. |
| D-5 | An area reaches 0 living stems through natural death | refuse the whole advance (copying `:283-284`) or carry the area forward unchanged | **Carry forward** with an explicit "no living stems" year record. Refuse only when **every** area is empty. | Refusing would let one empty area freeze the whole experiment. |
| D-6 | First fixture shape | 5 areas, about 450 trees (§3.5), or smaller | 5 areas, so the future S/T1/T2/D/U coverage can be **labelled later** by the assessor without a new fixture | Fixture size and review effort |

## 7. F4: comparator bridge without fake positives (PROPOSED)

- **Extractor `ma-features-v1`:** read-only over one `MultiAreaFrame`. Per area: count, N/ha, BA/ha, Dq, RD (from `Summarize`); DBH and height quantiles (bit words); age set; Hegyi CI and neighbour distribution, taken from the `PreGrowthCi` and `Neighbours` columns of a **discarded** `AnnualRules.Calculate` probe on the current state (`AnnualEngine.cs:20-21`). That probe is pure and is never published. Plus each `site` field with its provenance class. Every feature records its source, units, class and missingness.
- **Classifier `g1-cmp-uncalibrated-v0`:** for **one** declared treatment type (crop-tree release by Fell), it returns **UNKNOWN for every target**, with a factor-by-factor audit. Reasons: no accepted predicate; site, soil, exposure and history are unknown; tree fields are synthetic. **DIFFERENT** is designed but **disabled** until a Manager-accepted rule exists (#843 §1: DIFFERENT also needs an accepted rule). Binding failures (stale token, wrong experiment, dead or unknown ID) are **refusals**, not a third state.
- **No automatic targets.** While the classifier is UNCALIBRATED, there are no proposals. The player can still make manual, source-backed selections in any area, and the UI shows **Manual/NotCovered** for each area.
- **The bar stays:** MA-3 acceptance still needs a sample plus **≥2 independently supported COMPARABLE** targets plus a control. MA-2 passing does **not** check #828 R-05, X-01 or X-03.

## 8. F5: future implementation packets and exact allowlist (PROPOSED, NOT A GRANT)

### 8.1 Packet split

| Packet | Content | Gate |
|---|---|---|
| **MA-1** | Fixture family, parser, catalog, geometry, generator, teaching fixture, per-area engine, cross-area manual Work Plan, tests. **Core only, no UI.** | Q-A=A2, Q-C, Q-E, D-4…D-6; #828 amended and approved by an authorised docs lane; source task with a named single writer, branch, host and lock; independent nonauthor reviewer |
| **MA-2** | Feature extractor plus UNKNOWN-only classifier plus tests | MA-1 accepted |
| **MA-3** | Positive G1: predicate, proposals, prescription stage | #851 real data; Q-D accepted predicate; assessor holdout file; Q-B if site is used |
| **MA-4** | Multi-area player UI (map per area, one Work Plan panel) | MA-1 accepted; separate UI allowlist; #849 UI files are **modify-only under a new grant** |

### 8.2 MA-1 exact paths (all under `Tools/MultiRackSitkaP1Lab/`)

**New (preferred):**

1. `src/Core/MultiArea/MultiAreaManifest.cs`: format constants, strict parse, canonical write, content hash, §3.4 rules
2. `src/Core/MultiArea/MultiAreaGeometry.cs`: integer polygon validation, shoelace, point-in-polygon, overlap
3. `src/Core/MultiArea/MultiAreaCatalog.cs`: records, trust status, `Digest()`, no-follow read through the existing `NoFollowRead`, own gate
4. `src/Core/MultiArea/MultiAreaWorkContract.cs`: §5.1 types
5. `src/Core/MultiArea/MultiAreaContinuation.cs`: §4 and §5 root, advance, Work Plan, Execute
6. `src/Core/MultiArea/MultiAreaGenerator.cs`: labelled synthetic generator
7. `fixtures/multiarea/mad-sitka-teaching-5area-v1.sitkamafx.json`: generated bytes (§3.5)
8. `tests/Core.Tests/MultiAreaManifestTests.cs`
9. `tests/Core.Tests/MultiAreaContinuationTests.cs`

**Modified (minimal, compile-linked):**

10. `tests/Core.Tests/Program.cs`: add 2 registration lines after `:442`. No other change.
11. *(optional)* `README.md`: one new section. Can be dropped.

**Explicitly excluded:** both `.csproj` files (globbing makes them unnecessary), `Domain.cs`, `FixtureAccess.cs`, `TrustTables.cs`, `DerivedManifest.cs`, `ManifestGenerator.cs`, `AnnualRules.cs`, `AnnualEngine.cs`, `AnnualContinuation.cs`, `WorkContract.cs`, `Player*.cs`, `src/Ui/**`, `src/Cli/**`, `fixtures/derived/**`, `.p0ref`, every existing test file other than the two lines, `Tools/MultiRackSitkaLab/**` (P0), `Assets/**` (including the linked `SimulationRandom.cs`/`SitkaGrowthModel.cs`).

**Fixture generation without a CLI change:** a generator test writes candidate bytes to a scratch directory and asserts byte equality with the committed fixture. The author creates the first committed bytes by copying from that scratch output, and the reviewer regenerates them independently. If the Manager prefers a CLI `generate-multiarea` command, that adds `src/Cli/Program.cs` to the allowlist and needs its own grant.

**Lane:** base `4c42e2012a4c1bee3a422e41a58535343d63ac9f` (or the head of the feature branch at grant time, if Manager re-pins). New branch `feat/multi-g1-ma1-multiarea-engine-<date>-v1`. One writer. Exclusive lock on the 10–11 paths. Independent reviewer from a different provider with native .NET 8.0.134 build and test rights. The P0 DLL is **copied and 4-way verified, never rebuilt**, so `p0-isolation.sh` and `build-output-safety.sh` must not rebuild P0 (#849 open item 3).

### 8.3 Future acceptance matrix (ALL NOT RUN)

**Fixture and identity (MA-F)**

| ID | Case | Expected |
|---|---|---|
| F-01 | Valid 5-area teaching fixture | Parses; catalog digest printed; disclosure present; all site fields `unknown` |
| F-02 | Duplicate tree ID across two areas | `TreeIdNotUnique` |
| F-03 | Case-shifted twin (`…-t001`/`…-T001`) | `TreeIdCaseCollision` |
| F-04 | `P30-001` or any `^P30-` ID | `TreeIdReservedFamily` |
| F-05 | Tree ID without its own area prefix, or a point outside or on its polygon | `TreeAreaMismatch` / `TreeOnBoundary` |
| F-06 | Self-intersecting, clockwise, repeated-vertex or <3-vertex polygon | `PolygonInvalid` |
| F-07 | Overlapping areas | `AreasOverlap` |
| F-08 | `areaHa` or `areaM2Exact` not equal to the recomputed value | `AreaInvalid` |
| F-09 | Two areas with the same seed | `SeedNotUnique` |
| F-10 | Site value with class `unknown`, null value with class `measured`, or an unknown provenance class | `ProvenanceInvalid` |
| F-11 | Role words (`sample`, `target`, `control`) in the fixture | `RoleLabelForbidden` |
| F-12 | `p1d-` ID or P1 format through the new loader; `mad-` ID through `FixtureAccess.ReadDerived` | Both refused (`FixtureAccess.cs:114`) |
| F-13 | `MultiAreaSitkaExperimentV1` through `P1DomainWire.Parse` | `UnknownDomain` |
| F-14 | Extra or missing key, BOM, duplicate JSON key, tampered byte or content hash | Refused before any engine state exists |
| F-15 | Generator regenerates the committed bytes | Byte-identical |

**RNG (MA-R)**

| ID | Case | Expected |
|---|---|---|
| R-01 | For every tree pair across areas, `(key, seed)` pairs are distinct | Asserted over the whole fixture |
| R-02 | Mutation: force two areas to the same seed and the same IDs | Parser refuses; with the parser bypassed in the test, roll streams are equal, which shows why the rule exists |

**Engine (MA-E)**

| ID | Case | Expected |
|---|---|---|
| E-01 | One-area session from the original P1 fixture, Years 1–10 | Ten digests equal `ContinuationTests.cs:11` |
| E-02 | 5-area session, Years 1–10, twice | Byte-identical repeat |
| E-03 | Reverse area evaluation order | Byte-identical results |
| E-04 | Areas of different stocking | Different per-area RD, pressure and death counts, each equal to a standalone `Calculate` on that area alone |
| E-05 | A tree at an area edge with a neighbour across the boundary within 8 m | That neighbour is absent from its CI (isolation proof) |
| E-06 | Fault in area 3 during advance (seam) | Root unchanged: year, revision, every roster |
| E-07 | One area empty, others live (D-5) | Per the decision; every area empty → refused |

**Work Plan (MA-W)**

| ID | Case | Expected |
|---|---|---|
| W-01 | Add / approve with orders in 3 areas | Zero Fell; one approved plan |
| W-02 | Execute | One event; per-area before/after BA and count from engine rows; typed `fell` cause |
| W-03 | Year y Fell, then advance | Year y+1 growth and natural death on actual survivors per area; felled trees stay dead and their cause is unchanged |
| W-04 | No-work year | No Fell events; history row present |
| W-05 | Pending work + advance without consent | Refused; nothing changes |
| W-06 | Pending work + advance with consent | All areas' orders cancelled at zero Fell, then a real year |
| W-07 | Order with the wrong `AreaId` for its tree | `MultiAreaWrongArea`; zero mutation |
| W-08 | Fault via `afterAreaBuilt` after area 2 of 3 | No roster, event, revision or key change in **any** area |
| W-09 | Fault via `beforePublish` | Same as W-08 |
| W-10 | Concurrent readers during Execute | Never see a partial multi-area event (PS-01 pattern) |
| W-11 | Same-key replay, same payload | Receipt, `Replayed=true`, no second Fell |
| W-12 | Same key, different plan or source; unminted key | `WorkExecutionKeyConflict` |
| W-13 | Stale token (year, annual, work or plan-state revision) | Refused; zero mutation |
| W-14 | Token equality with equal-content but different `AreaIds` list instances | Equal. Different order or content → not equal |
| W-15 | Batch that empties one area (D-4) | Whole batch refused |
| W-16 | Order with `Origin=ApprovedPrescription` in MA-1 | Refused (no prescription stage) |
| W-17 | Mixed batch: one dead tree among live trees in another area | Whole batch refused |

**Protection (MA-P)**

| ID | Case | Expected |
|---|---|---|
| P-01 | Existing 92/92 Core tests and 137/137 UI smoke | Unchanged PASS; no test edited except two registration lines |
| P-02 | Ten no-work digests, `DerivedCatalog.Digest()`, P1 fixture SHA `bcaaa5c3…`, P0 DLL `9a99f781…` | Byte-identical |
| P-03 | `git diff --name-status 4c42e201..<candidate>` | Only the §8.2 paths |

**MA-2 (comparator, later):**

| ID | Case | Expected |
|---|---|---|
| C-01 | Every target | UNKNOWN with factor audit |
| C-02 | Site `unknown` | Never equal |
| C-03 | Feature with NaN or non-finite bits | UNKNOWN |
| C-04 | Stale or other-experiment frame | Refused |
| C-05 | Zero proposals and zero PendingWork created by any comparator call | Asserted |
| C-06 | A threshold constant without an acceptance citation | Build-time test fails (#842 N-11) |

**MA-3 positive** (sample plus T1 and T2 COMPARABLE plus control): **NOT TESTABLE** until #851 and Q-D.

## 9. Proposed redlines to the #828 DRAFT (review proposals only; nothing committed)

| #828 location | Current | Proposed wording |
|---|---|---|
| spec.md §2 G1-C | "…include sample, at least two supported comparable targets and one DIFFERENT/UNKNOWN control…" | Add: "The engine fixture carries **no role labels**; S/T/D/U labels belong to a separate assessor holdout file. A first fixture may be a labelled synthetic teaching fixture **without** comparability claims (Q-A=A2); it qualifies identity, engine and Work Plan only, never G1 positive." |
| spec.md §2 G2 | "Multi-area atomicity/area grouping must be qualified against the **actual** shared source transaction." | Add: "Required mechanism: one immutable multi-area root under one lock, published by one reference assignment (#852 §5.2); in-process only, no durability." |
| spec.md §4 `ManagementScope` | "Immutable source-linked scope ID…" | Add: "`areaId` matches `^MA-…`, neutral, ordinal-unique; tree IDs globally unique (ordinal and ignore-case) and area-prefixed; `^P30-` reserved; per-area distinct seeds; site fields carry a provenance class and default to UNKNOWN, inactive in growth unless a separate engine task activates them." |
| spec.md §3.6 | "genuine next-year … on actual survivors" | Add: "Per-area ecology is **isolated** (no cross-boundary competition) unless the owner adopts Q-C=C2 and a separate engine task delivers it." |
| requirements.md R-02 / X-01 | unchanged | Keep the ≥2-target requirement. Add a note: "MA-1 teaching fixture satisfies identity sub-items only; R-02 stays UNCHECKED until MA-3." |
| requirements.md (new) | — | **R-17** per-area isolation disclosed; **X-15** one-area equivalence to the ten frozen digests (MA-E-01); **X-16** mid-batch cross-area fault leaves zero partial event (MA-W-08). |
| plan.md / tasks.md | — | Replace any single-packet plan with the MA-1…MA-4 split (§8.1). |

These need an authorised #828 docs lane after the owner's decisions. This result does not change those files.

## 10. Alignment with #851, #849 and others

- **#851** (research, READY / NOT RUNNING at read time) is the only route to MA-3. This design gives #851 a concrete target: the §3.3 `site` block and tree provenance classes, which real records would fill. #851 supplies data; it does not need to design format.
- **#849:** no files, lease, build or session reused. MA-1 adds no change to the #849 surface. MA-4 would need a new UI grant.
- **No numeric tolerance, site value, location or comparability result was invented.**

## 11. Status of every claim

| Claim | State |
|---|---|
| Source facts in §2 | Read at `4c42e201` with `git show`; file:line cited |
| §3–§8 contracts, rules, paths | **PROPOSED** |
| Atomicity argument §5.2 | Design reasoning by analogy to a reviewed single-area mechanism. **Not executed** |
| Tests §8.3 | **NOT RUN**. No R/X box checked |
| Q-A…Q-E, D-4…D-6 | **OPEN**, returned to Manager and owner |
| Builds, .NET, Unity, fixtures, runtime | **None performed** |

**Changed path (only one):** `Docs/Handoffs/MultiRack-Simulation-20261116/852-orchestration/Multi-852-G1-FIXTURE-AREA-IDENTITY-PER-AREA-ENGINE-AND-WORK-PLAN-DESIGN-RESULT-20261120.md`.
**Independent reviewer:** none yet. A GPT-6.1 Sol nonauthor architecture and negative-matrix review is recommended once the Manager has read this result.
**Handback:** [Multi Manager #730](https://github.com/jermur788/CCF/issues/730).
