# Multi #737 Lane B — Multi-year annual engine: four-part SPEC-DRAFT proposal

**Date:** 18 November 2026  
**Status:** **DRAFT — NOT APPROVED / NOT IMPLEMENTED / NOT VERIFIED.** This is one documentation result combining proposed `spec.md`, `plan.md`, `tasks.md` and `checklists/requirements.md`; it does **not** create or edit files in `specs/`. Manager #730 and an independent reviewer must reconcile it with Lane E #740 and obtain applicable approvals before any production packet.  
**Claim:** [#737](https://github.com/jermur788/CCF/issues/737); **Manager:** [#730](https://github.com/jermur788/CCF/issues/730).  
**Worker:** OpenAI GPT-6, ChatGPT Game Dev / browser GitHub connector, authenticated to GitHub as `jermur788`; model effort and internal session identifier are not exposed. **Native host/worktree/Ops lease: none.**  
**Docs branch:** `docs/multi-737-b-spec-draft-20261118-v1`; freshly observed starting `main` revision `b7421c9a5cbf63a1409fe7ae8eed304fdfd4a7e9`. [RUNNING registration](https://github.com/jermur788/CCF/issues/737#issuecomment-6979724185).  
**Write scope:** only this new Markdown file. Original #725 and #710 worktrees, board, `main`, canonical decisions, all P0/P1 source, fixtures and any Unity data were untouched.

## Evidence, accepted boundaries and limitations

This is a source-*report*-based design review, **not direct physical inspection of the local, unpushed #725 source**. The following evidence is authoritative for what was observed; actual code implementation must later inspect the exact retained local tree with its own lawful physical grant.

| Evidence | What it establishes; what it does not |
| --- | --- |
| [#725 exact-R Year-1 implementation result](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md) | Five local P1 source/test paths implement a trust-gated, in-memory **0→1** step; one-successful-advance guard, immutable snapshot publication, source-derived annual arithmetic, 43/43 author checks. No second year or persistent save. |
| [#729 independent source and numeric review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md) | 0 blocking/major; 700 per-tree source comparison values and 100 RNG numerators independently matched. **M-1**: current `AnnualRules.Calculate` incorrectly includes dead input stems in competition, growth and `Stand(before)`, unreachable in Year 1 but **mandatory to repair before Year 2**. M-2 concurrent-call test overlap not guaranteed; M-3 uncapped hazard and RD exactly 1 branches untested; M-4/M-5 reporting/provenance notes. |
| [#732 focused native qualification, original BLOCKED + offline PASS resumption](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md) and [Manager acceptance](https://github.com/jermur788/CCF/issues/730#issuecomment-6978632144) | Original .NET build unexpectedly contacted NuGet and stopped; distinct resumed worker used explicit network-isolated grant. Focused .NET build, two identical annual outputs, exact reviewed numbers, original-P0 and tamper refusals passed. **No** Year-2 execution, full suites, save/load, Unity, field forecasting or security-L4 pass. |
| [#735 retained local qualified source](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md) | Qualified #725 source kept locally immutable through **27 Nov 2026 18:00 UTC** with no auto-purge; separate original repaired R #710 retained through **23 Nov 18:00 UTC**. Retention is not new source-write permission. |
| [#723 independent Year-1 oracle and gates](https://github.com/jermur788/CCF/blob/8890a730d349cb7082be34c213503055b8059b1f/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md) | Exact frozen fixture, source ordering, conditional RNG and separate synthetic edge-test requirements. No empirical Year-2 calibration. |
| [Owner's approval of existing four-file P1 specification](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md) and `specs/multirack-sitka-p1-adult-rules-comparator-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` on `docs/multi-p1-637-owner-approved-four-file-20261116-v1` | **Approved for bounded standalone P1 F0 packets**, not automatically a new multi-year API, durable archive, cross-platform parity or new implementation lane. The approved spec's annual causality, event integrity, three-domain and C-1/C-2/C-6 boundaries remain controlling. The four files are **unmerged from this branch to current main**. |
| [#740 Lane E provisional snapshot proposal](https://github.com/jermur788/CCF/issues/740#issuecomment-6979762166) | Parallel, explicitly **unapproved** proposal for versioned, immutable, detached `MultiSitkaReadSnapshotV1`. E—not B—owns final shared field names, versioning and renderer/command interfaces. |

**Canonical naming conflict:** `main` Decision Log **D-084** concerns **Scenario One Compact30**, whereas an unmerged standalone Multi-P1 branch also used “D-084” for Multi product approval. Do not treat main D-084 as Multi authority. Main **D-082** contains accepted standalone Sitka experiment design boundaries (original verified P0 inventory, conditional plan-staleness on load, synthetic/experimental/not-Irish-forecast labels). The separately approved P1 four-file artifact and explicit owner packet are cited by branch/URL, not by an ambiguous D number. No Scenario One Compact30 scene, save, rack or ownership is in scope.

**Protected Year-1 witness:** derived `p1d-30m-sitka-adult-v1` manifest, 32,567 bytes, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`; 100 initially living age-30 Mature-equivalent Sitka / Reference-demand stems, including 12 below 170 mm; 30 × 30 m, declared binary32 area `3db851ec`; seed `20261116`, growth model 2, RNG model 1. Initially RD `0.609795828454288`; after Year-1 growth RD `0.6375836886879149`; all 100 stems age 30→31; 1,622 directed Hegyi pairs; 0 deaths. The **existing one-year CLI JSON stdout** is 45,980 bytes, SHA-256 **`574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`**, independently reproduced twice. This digest is a compatibility anchor, **not** a claimed digest for any future ten-year report. The P0 52-file tree, fixture bytes, four pinned source blobs, existing original-domain refusal and L4 strict analyzer must remain unchanged.

**Security:** default .NET L4 remains **FAIL38, UNWAIVED**. The focused #732 offline qualification is not release or full security qualification. No new network, Host, persistence or security relaxation is authorised here.

---

# PART I — Proposed `spec.md`: consecutive-year adult Sitka experiment

**Feature ID proposal:** `MULTI-P1-ANNUAL-CONTINUATION-V1`. **Lifecycle:** DRAFT, requiring independent review, #730 integration/interface reconciliation and applicable owner approval.  
**Scope class:** SPEC REQUIRED: cross-year biological state, deterministic chronology, natural mortality and future interface consequences.  
**Goal:** Starting only from the already trusted separate P1-derived experimental fixture, advance **one ecological year at a time** to a minimum of **ten consecutive no-intervention years** in a closed adult pure-Sitka population, while preserving the exact existing Year-1 result and fail-closed domain/security posture. This is a developmental acceptance horizon, **not** a design decision that the game limits itself to ten years or that ten years predicts Irish growth.

## S1. Boundaries and non-goals

1. **Year 1 stays exact.** The existing `annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived` invocation must still create a fresh trusted genesis, execute exactly 0→1 and emit byte-identical stdout with the frozen SHA-256 above. The current one-step engine's second-call refusal `AnnualAlreadyAdvanced` is **not** retroactively reinterpreted as a defect: it is its intentional Year-1 lifetime contract. The multi-year facility must be opt-in and versioned (separate runner/request or explicit compatible extension), not a silent change to this command.
2. **No P0 annual or source write.** `OriginalPolygonExploratoryV1` stays read-only and refuses annual work (`OriginalAnnualRefused`); `DerivedUnitySquareParityV1` remains inactive/not source-captured; unknown domains, unaccepted/modified fixture bytes and unreviewed versions fail before any biology.
3. No planting, recruitment, mixed species, juvenile phases, storm, seasonal weather, new site response, interventions/marking/work execution, top-height science calibration, renderer, Host/network, durable save/load, Unity capture, Windows game build or new Scenario One source. Work/felling can later be inserted only at an approved **between-year execution** boundary; proposals alone never change a tree.
4. All living Mature-equivalent Sitka are biological participants, regardless of P0 actionable-thinning threshold. Preserve species, Reference demand, positions, initial site=1, growth/RNG versions and synthetic-input disclosure. No silent use of GROWFOR stand equations or additional mortality coefficients.
5. This is a **source-derived experimental rule continuation** on a declared .NET runtime, not a claim of numerical Unity/Mono equivalence, cross-platform byte stability or real Irish forest prediction.

## S2. Proposed immutable authoritative state (internal to Lane B)

A private, whole-experiment immutable `AnnualState`-like conceptual record (name illustrative, **not an approved shared API**) carries:

- **Experiment binding:** exact domain and case-sensitive fixture/catalog ID, accepted source fixture digest, source provenance, approved growth model and RNG model **as separate version fields**, algorithm/step-semantics version, fixed seed, area bits/units, geometry axes/origin, fixed exclusions and declared runtime/numeric policy. Every successor is bound to the same genesis, not only the same filename.
- **Chronology:** nonnegative `ecologicalYear` `y`, monotonically advanced to `y+1`; committed revision/generation; successor parent-state identity and the exact one-year transition identification. No implicit wall-clock timing or resetting the seed to start a new year.
- **Stable sorted roster:** same immutable ordinal `treeId` and source x/z position per entry, species/stage/demand, plus authoritative float state (DBH cm, height m, crown radius m; preserve the source-equivalent numeric round-trip/bit representation) and age years; explicit `isLiving`, work eligibility **separate** from biological participation, optional death cause, death mechanism and death year. **Do not remove a dead ID from the roster** merely because it drops out of living computations.
- **Ledger/history of committed years:** a causally ordered year transition and uniquely identified biological death per tree; separate year-level stand snapshots for stages `Before`, `PostGrowth`, `AfterMortality`, with no accidental reapplication. A detached view is derived only **after successful commit**, not a live pointer to editable engine collections.
- **Numbers/provenance:** source float32 vs binary64 storage and summation boundaries; per-ID CI, suppression, growth, p and roll are values for the current annual step **only**. Historical diagnostics may be retained in a separate audit result, but must not be read as a cached next-year CI.

**Successor identity proposal:** `(experimentIdentity, model/algorithm versions, ecologicalYear, committedGeneration, parentStateDigest, authoritativeStateDigest)`. A stable tree identity is `(experimentIdentity, ordinal treeId)`; year/revision identifies the *observation of that tree*. This is an internal integrity concept pending E #740 and D #739 agreement on digest canonicalisation, coverage and exposed names. Do **not** advertise or persist an undefined “hash” computed from display-rounded floats. Cross-platform or history-hash equality is not approved without a defined canonical encoding and runtime contract.

**Dead-stem state policy proposal:** a tree dying during Year `k` has its final post-growth size and age `k`, living=false, `deathYear=k`, biological cause `self-thinning`, mechanism `hazard` or `boundary`; on years `k+1` onward those fields and death record remain unchanged, with no further ageing/growth. A pre-inventory dead entry (if ever admitted by another separately trusted mode) must retain `cause=null`, `deathYear=null`, a separate `pre-inventory-dead` provenance marker, and no synthetic death event. Age-freeze/empty-stand policy must be affirmed at Manager/spec review; it does not affect the all-living frozen Year 1.

## S3. First hard dependency: correct M-1 before any Year 2

The #729 M-1 finding is not “optional later cleanup”. Every transition `y→y+1`, including the first year over an internal test vector with deaths, must derive a stable **living-at-start subset** before:

- Hegyi target enumeration **and neighbour enumeration**;
- adult DBH/height growth and crown relaxation;
- all-living `Stand(Before)`, `Stand(PostGrowth)` and mortality denominators;
- any vulnerability mean, hazard candidate list or boundary selection.

Dead entries are retained for audit only and can never exert competition, grow, age, receive a new roll, die twice or contribute BA/N/RD. The correction must preserve the existing all-living 100-stem Year-1 path **bit for bit**; it cannot be implemented by deleting entries from the canonical roster or changing the fixture/catalog/trust criteria. Add a **controlled dead-stem input vector** containing at least one dead, nonzero-diameter, near-neighbour stem and living competitors; compare against a version of the same population with that dead ID omitted **for every living tree's CI, growth and stand metrics** (with identical seed/year and stable IDs), and verify the historical dead row is inert and remains auditable. Also exercise a dead formerly high-CI/high-DBH tree and a dead target ID to detect asymmetric inclusion.

This is a **production gate**, not a test of the already frozen all-living Year-1 witness.

## S4. Annual chronological transaction (proposed normative order)

For each single request `Advance(y, expectedIdentity)`, under the same exclusive in-memory transaction boundary as #725:

| Phase | State consumed | Required transition/result |
| --- | --- | --- |
| A. Trust/admission | Current committed snapshot, case-sensitive accepted domain/fixture, immutable versions, expected `y`/state identity | Reject stale, conflicting, unsupported, untrusted or unauthorized inputs **before computation/publication**. No direct public synthetic-vector injection. Require `requestedYear=y+1`, not an arbitrary year jump. |
| B. Target-year selection | Current committed year `y` | Derive `targetYear=y+1` exactly once. Keep work/felling *disabled* in this feature. If later approved, only already executed changes are present in the `Before` state. |
| C. Living-at-start partition | Full immutable roster | Freeze living IDs `L_y` in **ordinal string order**. Nonliving IDs pass through unchanged. Capture `Before` all-living totals from `L_y`. |
| D. Pre-growth competition | `L_y` sizes/positions, source horizontal x/z | Hegyi living target versus living neighbours only; exact final **≤8 m** cutoff, 0.5 m distance floor, `max(1, DBH_target)`, source float operation order, deterministic ordinal accumulation. Cache this step's CI and suppression. |
| E. Growth and ageing | `L_y`, cached CI, current age/site/model | Each living adult advances one age and source Sitka DBH/height growth once, with clamps; then **post-growth** target-crown relaxation from post-growth DBH and cached pre-growth CI. Dead do not move/change. No reevaluation of CI within this year. |
| F. Pre-mortality density | Living after E, declared binary32 stand area | Compute `PostGrowth` N, BA, Dq, RD, and pressure based on **post-growth** density. Suppression/hazard vulnerability uses **pre-growth** CI, not a post-death competition recompute. |
| G. Hazard phase | Stable post-growth living candidates, mean vulnerability over current living, exact RNG model/seed/`targetYear`/ID | Compute `p` using source pressure/S/mean formula and cap 0.5. Evaluate `SimulationRandom.Roll` with key `ADULT-MORT-` + full ordinal ID **only when p>0**. Death iff `roll<p`. Simultaneous selected victims receive one natural death record; do not let earlier victims change this phase's mean/pressure. Preserve `roll=null` / `notEvaluated` when skipped. |
| H. Boundary phase | Hazard **survivors only** | If RD>1, remove by decreasing **pre-growth** suppression, then ordinal ID, recomputing density for survivors until RD≤1 or count≤1. A last surviving singleton may exceed RD1, as in the source function-level/tiny-area probe; do not invent a forced extinction. Record hazard and boundary victim sets as disjoint with cause `self-thinning` and exact mechanism. |
| I. Finalize one transaction | All untouched dead + surviving grown adults + newly dead and events | Create `AfterMortality` with all-living totals; check roster/ID/population/BA/event invariants; atomically swap one immutable state reference and its audit result. Only now may Lane E publish a detached read projection for the new revision. |

Same-source rule values are those in approved P1 spec §3: DBH `1.2*site*clamp01(1-D/120)/(1+CI/5)`; source Class III relative height; crown target `max(0.1,0.9415+0.07635*D_post)*response`, relax 15%; RD `(N/ha)*(Dq/25)^2.063/1868`; `P=max(0,(RD-0.6)/0.4)`; `S=1-1/(1+CI/5)`; `p=min(0.5,0.08*P^2*S^4/mean(S^4))` when denominators valid. Their exact float evaluation sites and source function linkage, not rewritten algebraic simplification, govern any production implementation. `SitkaGrowthModel` and `SimulationRandom` remain the approved pinned functions; no new growth/height/mortality calibration is proposed.

**Empty stand / singleton:** proposed annual-step result for zero living stems is a valid new year with zero N and BA, RD 0, Dq/top-height **null/not-applicable** (not NaN/Infinity, arbitrary zero centimetres or a fictional tree), no CI, growth, rolls or deaths. Singleton: no neighbours; growth remains defined; no ordinary neighbour CI; density boundary preserves the final survivor. The empty-stand continuation policy and serialization of null diagnostics require explicit technical/product approval before implementation, not invention in the worker's code.

**Inter-year link:** `Before(y+1)` must be derived from exactly `AfterMortality(y)` living biological state at full precision (not reloaded/requantized from the displayed Year-y JSON), with exactly the survivor ages/sizes/crowns and original stable positions. Recompute neighbours and stand density at every annual boundary; never reuse the prior year's CI/pressure or apply prior year's deaths as a second event. A removed neighbour affects the *next* year's CI, not post-mortality same-year regrowth.

## S5. Seeded determinism, identity, rollback and refusal

- Key the current source `SimulationRandom.Roll` by **fixed experiment seed, target ecological year, RNG-model version and full ordinal tree ID**; preserve exact source integer hash/24-bit numerator, no shared mutable RNG cursor and no dependence on dictionary insertion order, parallel scheduling or which other stems died. Seed and RNG-model version are fixed across the run and serialized only under a future approved D #739 policy. Year key changes on `y→y+1`, and the roll is **not** drawn at `p=0`. If output exposes the deterministic counterfactual roll for skipped hazard, it must not mislabel it “evaluated”.
- One write/commit authority: only the engine changes biological state. A concurrent pair with the same expected revision yields one committed successor and one stale/conflict refusal; no double year. Retry after a pre-commit throw computes afresh against the identical prior committed snapshot. Retry after a successful commit with the same request identity **never applies biology again**.
- Explicit conceptual failures: `UntrustedFixture`/existing trust-specific refusal, `UnsupportedDomain`, `UnsupportedModelOrAlgorithmVersion`, `WrongExpectedYear`, `StaleStateRevision`, `DuplicateAdvance`, `InvalidStateInvariant`, `CalculationFailedNoCommit`. **Names are proposed**, not changes to existing `AnnualAlreadyAdvanced`/`OriginalAnnualRefused`/other shipped error strings; Lane E #740 and Manager must select the public error vocabulary. A refusal must leave same object identity, year, tree values, event count, hashes and view revision as before, and cannot emit a report falsely labelled as committed.
- Commit is logically atomic (stage all state/events, validate, **single publish**). A calculation error, cancellation, injected failure just before commit or unsupported intermediate result must leave the old state published; no partial mortality rows. In memory this is not a durable save guarantee. Crash/process exit loses the experiment until separate **D #739** archive/replay approval.
- An accepted run with no work may progress `0→1→2→…→10` one step at a time. A request for `0→10` is not a biological shortcut; any future batch API must internally apply/validate each annual transition and share exactly the same rules/digest schedule. No skip, fast-forward, interpolation or per-request resampling.

## S6. Annual output and shared read boundary

**B-owned internal result proposal**, not a committed public schema:

| Record | Required contents / honesty |
| --- | --- |
| `AnnualStepBefore` | Source identity, expected year/revision; all-living N/Nha, BA/BAha, Dq/RD, eligible subset separately, exact numeric units and area bits |
| `AnnualGrowthDiagnostics` | Per living tree pre-CI, neighbour count, CI-derived suppression, age/DBH/H/crown before and post-growth, growth deltas, postgrowth RD and pressure; nonliving entries explicitly not evaluated |
| `AnnualMortalityDiagnostics` | By ID probability, conditional 24-bit source roll or null, selected hazard and boundary phases, biological cause/year/mechanism; aggregate natural death counts/BA; no duplicate IDs |
| `AnnualStepAfter` | All-roster preservation, all-living `AfterMortality` stand, final post-step per-tree state and year/revision; internally validated parent→successor chain and whole-step audit |
| `NoInterventionHorizonReport` | Ordered yearly step results 1..N, explicit provenance/exclusions, clearly **synthetic/experimental/not Irish field forecast**, no new empirical yield claims |

**Shared boundary:** [#740 provisional proposal](https://github.com/jermur788/CCF/issues/740#issuecomment-6979762166) names `MultiSitkaReadSnapshotV1` as a **detached immutable, versioned read projection**, potentially exposing tree ID, source x/z (metres), age, DBH, height, crown, live/death and stage-tagged annual stand fields. Lane B should publish a *completed* state revision only to the E-owned adapter. E owns contract version, field names, numeric encoding, snapshot identity, API/assembly boundary and presentation conversions; B owns biological transition semantics and private authoritative state. Renderer A #736, marking C #738 and history/persistence D #739 consume the reviewed contract, not `AnnualState` internals. **A displayed mark, hover or future queued proposal must not invoke this engine.** No renderer-owned mutation, UI pixel coordinates as simulation positions, cross-lane mutable lists or silent Host/save/stream capabilities.

---

# PART II — Proposed `plan.md`: bounded phases and gates

| Phase | Deliverable (future, after approval) | Precondition / exit condition |
| --- | --- | --- |
| **G0. Design consolidation** | Resolve E #740 shared snapshot ownership/identity, M-1 policy, empty-stand/dead-age decisions, public step/revision errors; independently review this draft and its regression requirements | **NOW: DRAFT only.** Manager #730 judges conformance to existing approved P1 spec and any owner-level scope/API decisions; no implementation claim |
| **G1. Physical/source admission** | Inspect exact immutable retained #725 source; create a *new* isolated source candidate from lawfully admitted qualified source, not a second writer on #725/#710 | Explicit Manager implementation start; task claim [RUNNING] actual model/provider/host/worktree/branch/HEAD and exact allowlist; native CCF-Ops registered physical source writer and valid exclusive lock; unmodified preflight. Resolve source import/integration and current-main branch separation. No worker arbitrary timeout; physical Ops capability must be reconciled, not bypassed |
| **G2. M-1 repair first** | Dead-target and dead-neighbour exclusion, controlled vector with past-dead age/cause retention, frozen Year-1 bit-exact golden | Independent changed-lines review of M-1, regression matrix; stop on any change to accepted manifest, P0 or Year-1 stdout hash. **Do not enable Year2 before this passes** |
| **G3. Consecutive in-memory years** | Versioned opt-in successor runner and immutable transactional `y→y+1` up to N=10; no work, save or network | Tests: chronology, all-living baseline, deterministic distinct year rolls, death/after-year effects, singleton/empty, end-to-end 0..10, retry and concurrency. Preserve one-year legacy CLI |
| **G4. Proportionate independent qualification** | Separate nonauthor source and numeric review; bounded offline native positive/negative check of exact candidate plus P0 isolation | Author test results are not independent qualification. Offline only under explicit grant; strict default L4 FAIL38 recorded unwaived. No automatic full-suite repetition unless changed risk requires it |
| **G5. Manager integration-readiness verdict** | `APPROVED` / `NEEDS CHANGES` / `REJECTED` on exact handoff; negotiate read-view contract E and later D policy | **No main merge, gameplay acceptance, save claims, Windows, Unity, L4 waiver or first external demo release** implied. Any source import requires separate authority and physical integration lane |

**Source work partition (recommendation only):** one future B biological owner may touch existing `Tools/MultiRackSitkaP1Lab/src/Core/Annual*.cs` and bounded tests/CLI dispatch *only after inspecting actual #725 tree*, with exact files fixed in its production packet. E #740 independently owns future shared read DTO/API integration, D #739 owns archive/history grammar, C #738 owns reversible UI marks and A #736 owns read-only renderer. There is no preallocated source writer, branch, worktree, build budget or physical lock in this document.

**Verification tiers:** targeted Tier-1 tests for every changed branch; bounded Tier-2 feature qualification on the exact admitted candidate after independent review; no speculative Tier-3 integrated/Unity qualification for this standalone lab draft. Never dilute or regenerate the frozen oracle to make new tests pass. The old Year-1 author 43/43 suite and native #732 golden are fixed regression witnesses, not automatic evidence that Year2 works.

**Compatibility/security stop conditions:** unknown source lineage; #725/R permission absent; missing Ops registration; dirty/overlapping source; source P0/fixture/pins altered; original P0 annual enabled; Year1 digest drift; dead contributes to CI/RD/growth; duplicate mortality/roll; successor initialises from rounded JSON; state mutation on failure; unauthorized save/Host or library/network contact; L4 analyzer weakening; unapproved cross-platform/Unity parity claim; owner-dependent decision unresolved. Mark BLOCKED rather than silently repairing protected history.

---

# PART III — Proposed `tasks.md`: implementation packets to write *only after* approval

| Task | Owner / difficulty | Dependencies | Concrete output and proof |
| --- | --- | --- | --- |
| **B-01** Read-only exact-source admission | Future local C# owner, **Medium** (custody/ancestry) | G0, G1 | Scoped source map: current one-step engine and actual source file/line contracts; clean protected readback, independent source origin, no code change; stop if source not safely available |
| **B-02** M-1 all-living fix | Sole biological writer, **Medium–High** (float-sensitive protected Year1) | B-01 | Living-only CI/growth/stand/death with archive retention of dead; controlled dead-neighbour vectors; unchanged 100-tree year1 hash; one independent focused review before Year2 |
| **B-03** Internal successor-state transaction | Same biological writer, **High** (identity/atomicity/repeatability) | B-02, E decisions needed for public revision shape | Versioned private state, step expected-year/revision guard, precommit atomicity/retry and deterministic numeric continuation, no new Host/save |
| **B-04** Up-to-ten-year no-work runner | Same writer, **Medium–High** (chronology/edge cases) | B-03 | Opt-in, no-jump steps 0..10, per-year outcome reconciliation, later-year mortality probes including zero/one live, stable seed/year source rolls; legacy CLI untouched |
| **B-05** Bounded independent source/numeric review | Independent nonauthor reviewer, **High** (model/rounding/security edges) | B-02–04 handoff | Review exact changed lines, branch margin and output provenance; match Year1 anchor and targeted Year2 vectors; decide if source repair needed without rewriting frozen witness |
| **B-06** Focused offline native qualification | Separate authorised native qualifier, **Medium–High** (exact version/permissions) | B-05 and physical offline grant | Targeted positive/negative execution, deterministic two-run comparison, genuine absent/dead and rollback vectors, P0/refusal/mode/provenance checks, documented L4 FAIL38 and network isolation; no Unity/game acceptance |
| **B-07** E integration decision only | Manager #730 + E #740 owners, **High** (cross-lane contract) | E proposal reviewed + B-05/06 evidence | Decide whether to publish detached snapshot adapter and separate UI/history work; versioned ownership and source gate. **No implied integration or main merge** |

**Packet policy:** tasks above are planning units, not GitHub claims or granted production allowlists. Manager will decide which are safe to combine, allocate physical single-writer access, link every relevant approval and permission, and issue a paste-ready exact handoff. Do not split one biological file set across two concurrent writers. In particular, B-02 must precede B-03/04 execution even if source prep or docs run in parallel.

---

# PART IV — Proposed `checklists/requirements.md`: acceptance and regression matrix

All boxes remain **unchecked**: this DRAFT cannot demonstrate execution. A future implementation's checked boxes require linked exact-source evidence, and independent reviewer/Manager disposition where specified.

## R. Biological state and annual causality

- [ ] **B-R01 M-1 blocking prerequisite:** every living-only computation filters dead **targets and neighbours** before competition/growth/stand/density/mortality; dead archive rows preserve identity and old death details; controlled dead-stem vector verifies no phantom competition/BA/age/death/roll.
- [ ] **B-R02 Stage order:** each `y→y+1` increments year once; pre-growth CI → live adult DBH/height → crown relaxation from postgrowth DBH → postgrowth RD/pressure → hazard → survivors-only boundary; no same-year release regrowth.
- [ ] **B-R03 Population:** all living Mature-equivalent Sitka—including below P0 work eligibility—participate; nonliving never do. Other stages/species/demands and untrusted mode rejected.
- [ ] **B-R04 Numbers:** source float/double order, clamps, inclusive 8 m/floor, source Class III height, prescribed site=1, exact binary32 stand area and source RD/hazard expressions retained; no invented coefficients or Unity parity.
- [ ] **B-R05 Death integrity:** exact cause `self-thinning`, recorded hazard vs boundary mechanism and year; no double-victim or repeated death; dead never age/grow again on later years (subject to dead-age policy approval).
- [ ] **B-R06 Empty/one live:** explicit zero living and singleton policies with finite, correctly typed stand diagnostics; no NaN, Infinity, fabricated recruitment or forced removal of last boundary survivor.
- [ ] **B-R07 Closed accounting:** per year `N_after=N_before-naturalDeaths` when no work; annual natural death IDs partition into disjoint hazard/boundary groups; no economic felling revenue or work logs invented. BA reconciles to pre-growth + growth - actual postgrowth BA of deceased (within the declared numeric round-off policy).
- [ ] **B-R08 Year-to-year causality:** survivor values flow at authoritative precision; CI recomputed next year after deaths; no stale cached CI, omitted deaths, reseeding, annual shortcuts or intra-year second growth.

## D. Determinism, identity and rollback

- [ ] **B-D01** Frozen same-arguments `annual` Year1 stdout byte SHA-256 **`574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`** and 100 living / zero deaths, including 700 per-tree reviewed values and 100 source RNG numerators, remain identical; trusted fixture digest unchanged.
- [ ] **B-D02** Fresh `0→1→...→10` runs twice using identical pinned version/seed/fixture/runtime yield exactly the same per-year authoritative outputs and ordered per-ID death records; year-specific roll change is explained by year key, not new seed; do not extrapolate Year2 expected totals without a verified oracle.
- [ ] **B-D03** A ten-year run stepped individually vs a runner that calls the *same* individual step ten times is identical under the same canonical history (including hash only when encoding/history truly identical); different session/approval events may change raw history and are not ignored.
- [ ] **B-D04** Wrong expected year, duplicate request after success and two concurrent requests with a synchronised overlap test (barrier/latch, not merely unsynchronised `Parallel.Invoke`) cannot advance twice.
- [ ] **B-D05** Inject fault after calculation before publish; exact prior state reference/year/roster/events/view revision remain unchanged; retry produces same successor as clean reference.
- [ ] **B-D06** Unknown growth/RNG/step version, malformed state, wrong model/fixture, altered catalog/manifest, original P0 annual, derived-Unity unavailable domain and noncanonical numeric fields all refuse atomically with existing/approved codes.
- [ ] **B-D07** Events carry unique experiment/year/tree identity and no duplicate deaths across years; stable tree IDs never renumber because a stem died; stage-tagged metrics cannot be misread as a living current state.

## T. Focused future test matrix (no tests were run in this docs lane)

| Test | Input/control | Exact expected property (not invented future numeric result) | Level |
| --- | --- | --- | --- |
| **T01 Frozen compatibility** | Unmodified accepted 100-tree 30m genesis, `annual` | Byte digest and all categorical Year1 facts unchanged; original one-step second call still refuses | B-02 + regression |
| **T02 Dead target/competitor** | Controlled living pair plus one initially-dead nonzero stem near both | Living CI/growth/N/BA/RD equal vector with dead excluded; dead unchanged and no new event | **Gate before Year2** |
| **T03 Controlled Year1 victim→Year2** | In-memory trusted test vector with documented high-pressure hazard/boundary death | Dead victim inert in Year2, survivors' next-year CI changes appropriately, death cause/year unique | Core |
| **T04 Uncapped hazard** | Controlled `0<p<0.5`, roll below p and adjacent counterexample | Exact `roll<p`, correct conditional no-roll for p=0, distinguishes capped vs uncapped | Core |
| **T05 Density boundary** | Controlled RD exactly 1 and just above 1, suppression/ID tie, tiny-area singleton | Strict `RD>1`, ordinal stable victim order, final singleton rule, no repeat victims | Core |
| **T06 Spatial float boundaries** | Distance 8.0, next representable float >8, 0.25 and 0.5 metres, equal/ordinal IDs | Inclusive cutoff, floor, source sum and stable ordering across years | Core |
| **T07 Mortality extremes** | All hazard victims; no living; one remaining; no pressure | Defined post-state and later years; zero duplicate rolls, zero bogus growth, no NaN/Infinity | Core |
| **T08 10 annual steps** | Original fixture; no commands/work; same runtime, fixed models/seed | Continuous 0..10, each 1-step, nonincreasing living N, stable IDs, explicit per-year reconciliation; deterministic repeat | Feature |
| **T09 Expected revision/faults** | Two synchronised callers, stale request, injected precommit throw, retry | Exactly one publish or refusal, no partial state, identical retry output | Feature |
| **T10 Trust-domain negative** | Original P0, Unity square, modified accepted fixture copy, unknown ID/domain, wrong models | Same exact refusal-before-biology boundaries as Year1; no output falsely claiming committed year | Feature |
| **T11 Output snapshot isolation** | E-shaped detached snapshot held by renderer while next year runs | Prior view never mutates; new revision atomically visible; source x/z m stable; renderer cannot mutate B state | Cross-lane E gate |
| **T12 Protected guard and offline scope** | Qualified original R/P0, new source candidate and source trees | Protected tree/files/fixture unchanged; narrow independently reviewed native test receipts, no silent network restore; L4 **FAIL38 stays UNWAIVED** until separate explicit release security decision | Physical qualification |

**Acceptance semantics:** T01–T10 (and T12 at their appropriate production gates) are requirements to call this *standalone multi-year feature candidate* ready for Manager review; T11 additionally gates any shared UI integration. A missing future save/load or Host is **not** made green by pretending it was tested: those are explicitly outside this B phase and owned by separate later approvals. No full ten-year oracle numbers are stated without an actual independently qualified execution.

## Cross-lane interface dependencies and unanswered decisions

| ID / owner to decide | Proposed question; default **not approved** | Consequence if unresolved |
| --- | --- | --- |
| **OPEN-B1 — Manager #730 + E #740** | What exact versioned detached snapshot type, numeric bit encoding, source identity and read-only API/assembly maps private B state to A/C/D? E's `MultiSitkaReadSnapshotV1` is provisional, not executable. | No shared UI/API implementation, though internal draft design can proceed |
| **OPEN-B2 — Manager #730 / owner if gameplay choice** | Is an empty forest still advanced as an annual empty-state experiment, or is it a terminal end-state? Proposed empty-step rule above is gameplay/technical policy, not established source behaviour. | Cannot assert no-intervention ten-year outcome for an all-dead synthetic control |
| **OPEN-B3 — Manager/spec reviewer** | Confirm freeze-at-death age/final size and lossless historical dead-row format, plus pre-inventory-dead distinction for future domains. | Must not implement later-year death archives without this |
| **OPEN-B4 — Manager + D #739** | Future save/replay form, canonical state/history hashing and numerical platform fingerprint; conditional plan staleness required by D-082 when work/load exists; distinguish equal biological state from equal raw event bytes. | No persistent resume, save or cross-process split-run equivalence claim |
| **OPEN-B5 — Manager + owner if workflow changes** | Opt-in runner/CLI name, duplicate/year guard public codes, step result and permitted future intervention boundary. Preserve existing `annual` bytes/refusal. | No new public/Host/UI command until contract approved |
| **OPEN-B6 — Manager + qualified source custodian** | How to admit the **local-only** #725 tree safely into a new physical source lane and reconcile it with current main and unmerged approved P1 spec? Physical Ops registration must be genuine, with no arbitrary worker-time expiry and no takeover of #725/#710. | All production code and testing BLOCKED |
| **OPEN-B7 — Manager + independent science/numeric review** | What runtime and numeric acceptance class for Year2–10 without legal Unity capture? Proposed internal fixed-runtime deterministic comparison only; defer any C-2b cross-platform/Unity parity until independently captured. | No Unity/numerical equivalence or Irish growth-forecast claim |
| **OPEN-B8 — Manager / security custodian** | When and how is default L4 FAIL38 independently resolved without altering analyzer rules, and are offline .NET namespace commands separately permitted? | Development may remain under explicitly bounded policy; no release/security acceptance |
| **OPEN-B9 — Manager + C #738 + D #739** | Should future executed interventions occur between years, and how are mark → approved work → executed felling events bound? The current ten-year proof has **no interventions**. | No marking/approval workflow can change biological state through B |
| **OPEN-B10 — Manager/owner** | Clarify approved P1 four-file scope vs this new repeated-year design: whether independent review plus Manager acceptance suffices for internal continuation or a separate explicit product-owner approval is needed for changed player workflow/persistence. | Keep this proposal **DRAFT**, never silently promote approval |

**Recommended near-term disposition:** #730 asks E #740 to incorporate this private-state/read-projection boundary into its shared-interface review, obtains independent nonauthor review of B's biological/temporal specification, and decides B1–B3/B5/B6/B10 before creating any physical source task. M-1 repair comes first in any authorised coding sequence. Leave save/persistence, marks, viewer and release security in their separately owned lanes. Owner action is **none for this documentation handoff**; explicit approval may be requested later if the Manager finds a genuine product decision.

## Evidence classification and final status

- **Directly backed by published implementation/review/qualification receipts:** Year1 report identity, all-living Year1 numbers, zero deaths, source derivation, 43/43 author tests, independent 700-value check, #732 focused offline native PASS, #729 M-1 finding and #735 immutable custody. The underlying unpushed local source was **not accessed in this browser lane**.
- **Existing accepted project intent:** D-082 original-fixture initial state, conditional work-approval staleness upon future load and synthetic/non-Irish-forecast labelling; separately owner-approved P1 four-file technical-spec baseline, with unmerged status.
- **Model-derived architecture proposal:** living-only multi-year transitions, immutable successor identity, dead-row age/size freeze, no-work ten-year runner, stage outputs, private digest and E boundary. Proposed names/types are **not shipped APIs**.
- **Gameplay/technical calibration pending decision:** empty-stand terminal policy; later intervention timing and public UI/command semantics; numeric tolerance/fingerprint strategy beyond the fixed-runtime experiment. No unsupported empirical Irish parameter has been inserted.
- **Unsupported / unavailable:** Year2–10 actual run, 2D gameplay/rendered verification, persistent save/replay, Unity Mono or Windows parity, field forecasting, full security L4 PASS, integrated source/main release.

**Handoff state:** **HANDED-OFF FOR MANAGER SPEC REVIEW ONLY**, with every checklist item deliberately unchecked. `APPROVED`, `IMPLEMENTED`, `INTEGRATED`, `VERIFIED` and `ACCEPTED` remain separate future gates. Dashboard/Drive sync not verified by this GitHub-only worker.
