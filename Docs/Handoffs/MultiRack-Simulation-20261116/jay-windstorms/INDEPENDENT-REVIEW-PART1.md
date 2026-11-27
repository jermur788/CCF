# MULTI-2D-JAY-WINDSTORMS-P1-20261127 independent review

**Integration decision — NEEDS CHANGES.**

Independent reviewer — OpenAI Codex, GPT-6.2 Sol, medium effort, separate review thread. Candidate author from the task packet — Claude Opus 5.5. Findings — 3 blocking, 3 non-blocking, 3 observations.

Reviewed candidate `cc59918f46d3e63bdc164fc70f2125072625bc0e` (parent `7b5c2a07d5b556805b4762dfd8b6d0c086a21962`). The feature worktree was clean at entry and both the local tracking ref and live remote feature ref matched the candidate. The fixed comparison base `c82e4e759c2ceef1c13ddb6a033e034097aef77e` remained the merge base with the live integration tip `fa69d6d3d8c1789af531b53adefb5f426e90bbed`. Frozen sources were read via `origin/main` at `bdeb6a324acaf4129c10fb0846faac8b64974360`.

## Findings

Source and test paths below are relative to `Tools/MultiRackSitkaP1Lab/` unless they start with `Docs/` or `specs/`.

| ID | Severity | Location | Evidence and impact | Required change |
|---|---|---|---|---|
| R1-01 | Blocking | `src/Core/NaturalProcesses/JayAcornDispersal.cs:53`, `:114`; `src/Core/NaturalProcesses/NearbyOakWoodland.cs:19`, `:31`; `src/Core/NaturalProcesses/JayAcornTransport.cs:17` | One accepted on-map candidate with ID `NEARBY-OAK-WOODLAND-West-0` and source tree ID `NEARBY-OAK-WOODLAND-West` collides with the first external acorn when both options are on. The combined result contains the same candidate ID twice and the draw log shows both distance and bearing keys used twice. A caller matching transported candidates to diversion IDs can confuse an external input with a diverted on-map candidate. | Enforce disjoint origin namespaces for both candidate identity and transport keys, or refuse reserved IDs before drawing. Add a combined-origin collision regression that checks identities and observed keys. |
| R1-02 | Blocking | `src/Core/NaturalProcesses/NaturalProcessContracts.cs:70`, `:98`, `:140`, `:183`, `:205`, `:219`; `src/Core/NaturalProcesses/WoodlandFootprint.cs:19`; `src/Core/NaturalProcesses/SalvageAccounting.cs:56` | The positional records retain mutable request collections and return results backed by mutable arrays/lists. In the probe, replacing the input array entry after request construction changed which ID was diverted. Casting the result ID list to `string[]` allowed rewriting the diversion list. Casting `Vertices` to `LocalPoint[]` changed a previously constructed footprint from 6,400 to 4,800 m². These mutations break the promised snapshot and can change seed or deadwood accounting between stages. | Defensively snapshot incoming collections and expose immutable collections or read-only wrappers backed by private snapshots. Cover requests, results, the footprint and public exposure output. Test input mutation and attempted output mutation. |
| R1-03 | Blocking | `src/Core/NaturalProcesses/SalvageAccounting.cs:68` and `:80`; `tests/Core/NaturalProcesses/SalvageAccountingTests.cs:75` | The merge checks extracted IDs only in `added`. After a sale of D1, passing existing stock `[D1]`, the returned extracted IDs `[D1]`, and additions `[D2]` succeeds and returns D1 in stock. This accepts an inconsistent continuation input and lets an extracted stem reappear as deadwood. The replay check with D1 in additions correctly refuses. | Validate the extracted-ID invariant against existing stock and additions before returning a successful merge. Add a regression with an extracted ID in existing stock and a distinct valid addition. |
| R1-04 | Non-blocking | `src/Core/NaturalProcesses/SalvageAccounting.cs:43`, `:46`, `:88` | A sale of 2 m³ with accepted price `decimal.MaxValue` throws `OverflowException`. An aggregate of individually representable lines can also exceed the decimal range. Normal project rates passed. | Bound representable sale pricing and totals or return a documented atomic refusal for overflow. |
| R1-05 | Non-blocking | `Docs/Handoffs/MultiRack-Simulation-20261116/jay-windstorms/IMPLEMENTATION-HANDOFF-PART1.md:75`, `:151`, `:155`; `specs/multi-2d-jay-windstorms-v1/spec.md:50` | The quartet implements both rulings. The main handoff transcript still describes 225 ± 45° from and the main decision text says the task locks it. Repair 1 corrects the direction later in the file. The transform note also reads as open despite the owner's decision. This makes the current receipt ambiguous for a subsequent integrator. | Make the gate transcript and decision summary current, or label the superseded transcript as parent-only. Record both owner rulings as settled. |
| R1-06 | Non-blocking | `tests/Core/NaturalProcesses/JayDispersalTests.cs:93`, `:148`; `tests/Core/NaturalProcesses/StormAdapterTests.cs:58`, `:74`, `:131`; `tests/Core/NaturalProcesses/HygieneAndParityTests.cs:51` | JW-T06 counts carried records without proving each diverted ID appears exactly once. JW-T09 builds its expected stream with the implementation key helper. JW-T23 checks lane invariance but leaves observed per-lane draw counts to other fixtures. JW-T24 names earlier-year rejection but tests only equality and a later year. JW-T27 reuses both runtime numeric helpers for expected victims. JW-T62 lists five of nine existing registry entries. Independent source and probe checks cover the current happy paths and registry comparison, but the committed assertions leave gaps. | Assert exact diversion/transport set equality and uniqueness, literal or independently sourced key vectors, split-lane observed counts, the earlier-year refusal, at least one frozen victim vector, and all nine registry entries. |
| R1-07 | Observation | `src/Core/NaturalProcesses/StormAdapter.cs:69`; frozen `Assets/ForestPrototype/StormModel.cs:135` | Both follow the approved blow-towards convention and use the same draw. The adapter uses a double product and frozen Unity uses a float product. In the 1,000-year fixture 17 of 19 raw bearings differed, by at most 0.0000133514404296875°. The bearing does not affect victims under neutral site/edge. | Choose and record the precision at the Part 2 persistence boundary. Use the frozen float product before widening if exact frozen persisted bearings are required. |
| R1-08 | Observation | `src/Core/NaturalProcesses/NaturalProcessText.cs:35` | The note has the required dates and non-causal wording. Neither the note nor the governing research files substantiate the sentence about the 2026 timber market. JW-T47 checks wording. | Verify and cite the 2026 claim before player-facing use, or remove the factual claim while keeping sourced context. |
| R1-09 | Observation | `specs/multi-2d-jay-windstorms-v1/spec.md:119`; `src/Core/MultiArea/Phase2Rules.cs:197`, `:208`, `:222`; `src/Core/MultiArea/Phase2Annual.cs:79`, `:131`; `src/Core/NaturalProcesses/SalvageAccounting.cs:118` | Existing 2D oak seed rain sums continuous source potentials per cell. It does not provide discrete per-tree candidates or a debit calculation. The Part 1 seam works with a caller-provided candidate set. Similarly, storm context and the once-per-year checkpoint are caller inputs, and the salvage guard checks for a nonblank approval reference without looking up a Work Plan. | In the queued Part 2 packet, specify how discrete candidates are derived and debited without increasing ordinary seed input, including exported candidates. Resolve the storm once with the woodland seed and pre-storm context, apply the year guard across continuation, and obtain salvage references from the authoritative approved Work Plan before calling accounting. Verify grouped settlement there. |

## Part 1 criterion matrix

`JW-Txx` identifies a test executed in the 41-case standalone suite. “Probe” identifies the independent temporary executable described below.

| Criterion | Decision | Exact check |
|---|---|---|
| JW-REQ-01 Default OFF | PASS | JW-T01/T02 and probe draw log with seven supplied candidates and a mast outcome. |
| JW-REQ-02 Existing mast gate | PASS | JW-T03 and probe with both options on, non-mast, seven supplied candidates and zero logged draws. Inspected early gate in `JayAcornDispersal.Disperse`. |
| JW-REQ-03 Pure Sitka without Nearby | PASS | JW-T04 with zero oak candidates and Nearby OFF. |
| JW-REQ-04 Nearby cap, edge and attribution | PASS | All four JW-T05 edge cases and JW-T14 mixed origin fixture. Inspected the shared transport call. |
| JW-REQ-05 On-map cap, diversion and no double count | FAIL | JW-T06/T07/T14 and probe exact disjoint partition at 5/6/7 supplied pass with ordinary IDs. Mixed-origin probe returns a duplicate identity (R1-01). |
| JW-REQ-06 Kernel and dedicated keys | FAIL | JW-T09/T60 and independent frozen-RNG kernel oracle pass for ordinary source IDs. Mixed-origin probe records two duplicate transport keys (R1-01). |
| JW-REQ-07 OOB export, repeat and no retry | PASS | JW-T08 repeated over 40 years. Independent rectangle oracle checked exact unmodified destinations, seven exports across three boundary cases, and observed draw counts. |
| JW-REQ-08 Ordinary establishment, no shortcuts | PASS | JW-T12 plus review of transport inputs and outputs. Part 2 establishment application was not tested. |
| JW-REQ-09 Nearest integer caps | PASS | JW-T11 and the probe at 5/6/7 supplied on 0.64 ha. JW-T05/T14 produced four external offers at that area. |
| JW-REQ-10 StormModel 0 | PASS | JW-T20 used the draw log over 1,000 years. JW-T21 compared jay results before and after storm calls. |
| JW-REQ-11 Frozen StormModel 1 calibration, formula, event year and severity | PASS | JW-T22/T30/T60/T61 and independent compilation of the frozen sources. All 19 event years/severities matched and the fixture's vulnerability bits and victims matched. |
| JW-REQ-12 One woodland-wide event, neutral lane, year guard | PASS | JW-T23/T24/T30, code inspection of a single `SampleEvent` outside the tree loop and `last >= year`, and probe over 400 trees in two lanes: 403 rolls per storm year, one per calm year. |
| JW-REQ-13 Blow-towards direction and glyph | PASS | JW-T25, event/victim contract inspection, and probe comparison with frozen source. Raw double precision differs as recorded in R1-07. |
| JW-REQ-14 Qualitative bands | PASS | JW-T26 and probe at `BitDecrement(1.5f)`, `1.5f`, `BitDecrement(3f)` and `3f`, checked against frozen `ForestEcologyController.Storms.cs:134`. |
| JW-REQ-15 Two-pass victims and effects | PASS | JW-T27 and `StormAdapter.Resolve` pass ordering inspection. Probe derived victims using the frozen source over both lanes and matched the candidate. |
| JW-REQ-16 Repeat and shuffled order | PASS | JW-T08/T10/T28/T29 and independent reversal of 400 trees for every storm in the 1,000-year fixture. |
| JW-REQ-17 Default leave and approval reference | PASS | JW-T40/T41/T46 and probe with whitespace sale and null keep references. Authoritative approval lookup belongs to Part 2. |
| JW-REQ-18 Sell revenue only, Keep timber, unchanged rate and minimum | PASS | JW-T42/T43 and probe with 2 m³, rate 47 and minimum 2,500. Sell returned 94 and Keep returned zero revenue and 2 m³. R1-04 covers numeric bounds. |
| JW-REQ-19 Extracted deadwood guard | FAIL | JW-T44/T45 and probe of replay in additions pass. Probe of extracted D1 in existing stock with new D2 succeeds erroneously (R1-03). |
| JW-REQ-20 Dated non-causal teaching note | PASS | JW-T47 and reviewed the note text. Factual verification of the 2026 market sentence was not tested (R1-08). |
| JW-REQ-21 Fixed stream IDs and registry prefix fence | PASS | JW-T62 and manual comparison of all six public family entries with all nine existing `RngSalts.Families` entries. Internal on-map/external aliasing is recorded in JW-REQ-06. |
| JW-REQ-22 Pure source, deterministic APIs | PASS | JW-T63, checked every production C# file for clock, filesystem, networking, environment and culture-sensitive APIs. Numeric ID formatting is invariant and string ordering is ordinal. The thread-local draw log is an observer and does not affect rolls. |
| JW-REQ-23 Standalone SDK and path fence | PASS | JW-T64, `dotnet --version`, exact build/run below and `git diff --name-only c82e4e75..HEAD` with prefix check (23/23). |
| Spec §1.5 immutable contracts | FAIL | Independent array and vertex mutation probes (R1-02). |
| Spec quartet traceability and locked decisions | PASS | Read all four files, checked JW-REQ-01 through 23 against registered tests and searched for obsolete direction and probability wording. The quartet marks those proposals as superseded. |
| Handoff current evidence | FAIL | Compared current test output to handoff lines cited in R1-05. |
| Committed test coverage for critical negatives | FAIL | Read assertions against the specified cases. The positive suite leaves the R1-01 to R1-03 cases uncovered and has the gaps in R1-06. |
| Part 2 queued and outside the publication scope | PASS | Reviewed spec §5 and task checklist. Diff contains no annual pipeline, continuation, registry, UI, save or Unity changes. Contract changes identified in this review belong to Part 1. |
| Part 2 runtime, save, approval and grouped settlement | NOT TESTED | Existing entry points inspected for R1-09. No Part 2 implementation was run. |
| 2026 windblow market factual validation | NOT TESTED | Examined the governing research files for a supporting market source and found none there. |

## Commands and evidence

- Worktree preflight used `git status --short`, `git rev-parse HEAD HEAD^ origin/feat/multi-2d-jay-windstorms-engine-v1 origin/main origin/integration/2d-sim`, `git ls-remote` for the feature, integration and review refs, `git merge-base c82e4e75 origin/integration/2d-sim`, and `git diff --check c82e4e75..HEAD`. The name-only check filtered by the four authorized prefixes and found 23 paths.
- From `Tools/MultiRackSitkaP1Lab`, SDK `8.0.133` ran the commands below. Build exit 0 with zero warnings and zero errors. Run exit 0 with 41/41 passed (17 jay, 11 storm, 8 salvage, 5 hygiene/parity).

```sh
dotnet --version
dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
```

- Independent probe used `git show origin/main:Assets/ForestPrototype/StormModel.cs` and `git show origin/main:Assets/ForestPrototype/SimulationRandom.cs` to stage reference files in a temporary project outside the worktree. `dotnet run --project Probe.csproj` compiled those files with a minimal `Mathf` shim and linked the candidate C# sources. It exited 0 after checking the reproductions above. The probe emitted one nullable-return warning in the frozen source under its own nullable-enabled settings. The required candidate build emitted zero warnings.
- For reproduction, use seed 12345, the 80 × 80 m footprint, year 108, source (1,1) and on-map IDs A0 through A6. Compare `JAY-ACORN-v1-oak-{i}/distance` and `/bearing` rolls directly to returned destinations and classify against rectangle bounds. Use the specific IDs in R1-01 for the collision. The storm fixture used years 1–1000, seed 12345, tree IDs T1 through T400 alternating by lane, height `20 + i % 15`, DBH `20 + i % 8`, top height 30, light 0.5 and recent opening 0.3. Expected victims came from frozen vulnerability, chance and ID-keyed roll.
- The numeric caps, transport kernel, D-050 values and band boundaries are gameplay calibration. The review verifies their implementation and labels. It supplies no new ecological calibration. The review branch is the bounded coordination record. Dashboard ingestion was not verified under this publication scope.

## Delta re-review (cc59918f -> 3170cd2)

**Integration decision — NEEDS CHANGES.**

Independent delta reviewer, OpenAI Codex, GPT-6.2 Sol, medium effort. Candidate `3170cd2c0d8e5d921ddc66b4c4f516b8de145987` had a clean feature worktree and matched both the local tracking ref and the live feature ref. Its parent is `cc59918f46d3e63bdc164fc70f2125072625bc0e` and its merge base with the specified base is `c82e4e759c2ceef1c13ddb6a033e034097aef77e`. The 23 changed paths all match the four authorized prefixes. `git diff --check c82e4e75..HEAD` passed.

### Prior findings

| ID | Delta status | Exact independent check |
|---|---|---|
| R1-01 | RESOLVED | Read the validation before RNG creation and checked ordinal matching. Across four edges with Nearby ON and OFF, isolated the candidate ID, source tree ID and combined conflict (24 refusals). Every result and draw log showed zero draws. Eight accepted calls used lowercase and bare-prefix look-alikes. With both origins on, two on-map plus four external candidates yielded six unique IDs and 16 distinct draw keys per edge. JW-T16 also exercises above-cap selection. Removing only the guard in a temporary copy made JW-T15 fail and the suite exit 1 (47/48). |
| R1-02 | PARTIALLY RESOLVED | Probed all 14 collection properties through constructors and replacement via `with`, plus footprint vertices and standalone exposure bands. Caller-array writes and direct array/list casts did not alter the views. Generic and nongeneric list setters and generic add/clear threw. However `ICollection.SyncRoot` exposes the mutable backing arrays on SDK 8.0.133. A plain `with { }` copy can mutate the original and the footprint changes area. See R2-01. |
| R1-03 | RESOLVED | Sold D1, passed existing [D1] / extracted [D1] / additions [D2], and got `DeadwoodAlreadyExtracted` with stock [D1] intact. Inverse placement with existing [D2] and added D1 refused with stock [D2]. Empty extracted history accepted [D1, D2]. JW-T44/T48 passed. |
| R1-04 | RESOLVED | Refused negative, cap + 0.01 and `decimal.MaxValue` for price and minimum independently. Each refusal left D1 in stock with no lines, extraction, revenue or volume change. Exactly 1,000 m³ at 1,000,000,000 produced 1,000,000,000,000. JW-T49's 50-line aggregate passed. Each line is at most 10¹² and even `int.MaxValue` lines total below 2.2 × 10²¹, within decimal's roughly 7.9 × 10²⁸ range. The spec and code document both limits. |
| R1-05 | PARTIALLY RESOLVED | Read the handoff header, parent-only transcript, §5 and Repair 2 together with the quartet. Both owner decisions are explicit in the handoff and the current transcript is easy to identify. Spec §1.3 still labels the settled BoundedRational decision “[ENG, flagged for review]” at line 50. The handoff branch bullet at line 15 still says “no review/* branch created” despite the existing review acknowledged in Repair 2. Label that bullet as implementation publication history and align the spec decision label with the owner ruling. |
| R1-06 | RESOLVED | Inspected and ran all six named cases. T06 compares exactly the carried and diverted ID sets and checks their uniqueness and the supplied partition. T09 compares six literal stream keys to the observed log and uses those literals for the numeric oracle, with T60 covering frozen RNG parity. T23 verifies single event keys, 400 distinct tree keys split 200/200 in the split fixture and the single calm draw. T24 exercises year 108 with checkpoint 200. T27 contains literal 108/229 victim vectors independently reproduced from the frozen sources. T62's nine literals match the nine resolved base registry values in order, and its loops test actual prefixes bidirectionally. |
| R1-07 | OBSERVATION ONLY | Spec §5 item 10 queues precision selection. The independent frozen-source probe found 17 raw bearing differences in 19 events, maximum 0.0000133514404296875°. Each candidate direction exactly matched the frozen RNG draw widened before multiplication by 360 and equalled its glyph. Victim sets matched. |
| R1-08 | RESOLVED | Read the runtime teaching constant, §1.4 and the checklist. The old factual market sentence is absent. December 2024 and January 2025 remain, and JW-T47 passed. |
| R1-09 | RESOLVED | Spec §5 items 7–9 explicitly queue derivation and debit of on-map candidates including exports, woodland-seed/pre-storm resolution with a persisted year guard, and authoritative approved Work Plan lookup with grouped settlement verification. See R2-02 for a wording conflict introduced in the new queue text. |

### New findings

Source paths are relative to `Tools/MultiRackSitkaP1Lab/` unless they start with `specs/`.

| ID | Severity | Location | Evidence and required change |
|---|---|---|---|
| R2-01 | Blocking | `src/Core/NaturalProcesses/NaturalProcessContracts.cs:45`, `src/Core/NaturalProcesses/WoodlandFootprint.cs:16`, `src/Core/NaturalProcesses/StormAdapter.cs:52` | On .NET 8 the array-backed `ReadOnlyCollection<T>` delegates nongeneric `ICollection.SyncRoot` to the array, whose sync root is itself. The independent probe overwrote every nonempty request/result collection through that property, including after `with` replacement, and altered a source request through a plain copy. The same path modified standalone exposure feedback and a footprint's area from 6,400 to 4,800 m². Use a snapshot implementation that does not expose mutable storage through its interfaces, including `SyncRoot`, and extend the regression accordingly. The pre-existing `NaturalProcessStreams.All` at `src/Core/NaturalProcesses/NaturalProcessRandom.cs:21` uses the same pattern and should be covered by the public-collection audit. Update the spec to promise the behavior independently of the wrapper type. |
| R2-02 | Non-blocking | `specs/multi-2d-jay-windstorms-v1/spec.md:137` | “Never increases total oak seed input” in the new debit item can include Nearby contributions, which §1.2 explicitly adds as external seed. Scope the accounting invariant to the on-map derived input and state how delivered external input counts separately. |
| R2-03 | Observation | `src/Core/NaturalProcesses/NaturalProcessContracts.cs:172` | The generated record equality remains reference-based for collection fields, but two equal scalar StormRequests constructed or initialized from the same nonempty array now own distinct wrappers. A two-version probe comparing `request with { Trees = trees }` to a second copy with that same array returned `true` on the parent and `false` on the candidate. The standalone seam has no production caller and the spec promises no structural equality. Document the consequence if accepting it with the defensive-copy change. |

Minimal R2-01 reproduction on the candidate:

```csharp
var footprint = WoodlandFootprint.Rectangle(0, 0, 80, 80);
var list = (System.Collections.ICollection)footprint.Vertices;
var backing = (LocalPoint[])list.SyncRoot;
backing[2] = new LocalPoint(80, 40);
Console.WriteLine(footprint.AreaSquareMetres); // 4800, previously 6400
```

### Changed-area criterion matrix

| Criterion | Result | Evidence |
|---|---|---|
| Exact candidate, base, file fence and standalone gate | PASS | Clean entry, local and remote SHA match, 23/23 in scope, zero warnings/errors, 48/48, exit 0. |
| Jay origin identities, keys and pre-draw refusal | PASS | JW-T06/T09/T15/T16, independent four-edge probe and guard-removal negative control. |
| Constructor and `with` input snapshots | PASS | Post-construction mutation checks on every request/result collection and footprint input. |
| Public immutable collection outputs | FAIL | R2-01, including change to footprint geometry and a plain record copy affecting the original. |
| Extracted-stock and added-ID invariants | PASS | Independent two-path refusal and unchanged stock plus valid merge. |
| Atomic sale pricing and aggregate arithmetic | PASS | Independent exact caps, six refusal combinations, JW-T49 aggregate and arithmetic bound. |
| D-050 parity, storm direction, glyph and probability transform | PASS | JW-T22/T25/T27/T60/T61. Temporary frozen-source compilation independently matched 19/19 event years/severities and all 19 victim sets with 60 trees, including both T27 vectors and formula bits. The persistence precision decision is queued. |
| Public signatures, stable refusal values and deterministic keys | PASS | Positional signatures and property types preserved, codes 0–22 preserved and 23 appended, unchanged roll and vulnerability sources. The reserved-ID and price-domain refusals are explicit. Record equality change recorded in R2-03. |
| Six named test strengthenings | PASS | Assertions and literal registry/vector cross-checks in R1-06. |
| Current and consistent handoff/spec wording | FAIL | Decisions are implemented and the current transcript identified. The remaining stale authority and publication wording is recorded in R1-05 and the Part 2 debit scope in R2-02. |
| Dated teaching text and removal of old market claim | PASS | JW-T47 and source/spec inspection. |
| Three required Part 2 queue entries | PASS | Spec §5 items 7–9. |
| Part 2 runtime, Unity, save/load and settlement | NOT TESTED | Queued outside the standalone Part 1 delta. |

Required candidate commands, from `Tools/MultiRackSitkaP1Lab`:

```sh
dotnet --version
dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
```

SDK 8.0.133, build exit 0 with 0 warnings and 0 errors. Run exit 0 with 48/48 (20 jay, 12 storm, 11 salvage, 5 hygiene/parity). The mutation test, collection/accounting probes and frozen-source oracle ran in temporary projects outside the feature worktree. The frozen oracle used `StormModel.cs` and `SimulationRandom.cs` from context `bdeb6a324` with a minimal `Mathf` shim. Publication is this review branch commit.

## Final delta re-review (3170cd2 -> ca601d8d)

**Integration decision — NEEDS CHANGES.** G2 acceptance, standalone Part 1. Spec conformance fails on the immutable snapshot rule. Integration target is `integration/2d-sim`.

Independent reviewer, OpenAI Codex GPT-6.2 Sol, medium effort. Reviewed `feat/multi-2d-jay-windstorms-engine-v1` at `ca601d8d7fa8f4507340cc6c7c7f4958d5805f7f`, parent `3170cd2c0d8e5d921ddc66b4c4f516b8de145987`, merge base `c82e4e759c2ceef1c13ddb6a033e034097aef77e`, context `bdeb6a324acaf4129c10fb0846faac8b64974360`. Clean feature worktree at entry, local and live remote feature refs matched, 23/23 paths inside the four spec prefixes, and `git diff --check c82e4e75..HEAD` passed.

### Delta findings

| ID | Status | Independent evidence |
|---|---|---|
| R2-01 | PARTIALLY RESOLVED | All 14 record collection properties and three standalone outputs (vertices, exposure bands, stream list) reject every requested mutable-interface, array and `ReadOnlyCollection<T>` cast. The collection type exposes the read-only list/enumeration interfaces and `IEquatable`. Reflection found no `SyncRoot` or writable field/property on the wrapper. Both enumeration paths yield elements and expose no backing array. The exact prior SyncRoot reproduction changed 6,400 to 4,800 m² on the parent. The candidate throws `InvalidCastException` at the first cast and stays at 6,400 m². Caller-provided collections can still break the snapshot guarantee through `CopyTo` (R3-01). |
| R2-02 | RESOLVED | Spec §5 item 7 debits all on-map diversions, including exports, against ordinary on-map input. It explicitly tracks delivered Nearby seed as intentional external input, separately from the debit, consistent with §1.2. |
| R2-03 | RESOLVED | Independently constructed records across all seven record types, with different list instances, compare equal with equal hashes. Plain copies and equivalent init replacements compare equal with equal hashes. For each of the 14 collection fields, reordering, removing or substituting an element, or passing null, breaks equality. Jay requests share the footprint instance in the equality check. An independently constructed equivalent outline compares unequal under the footprint identity rule documented in §1.5. |

### New finding

| ID | Severity | Location | Reproduction and required change |
|---|---|---|---|
| R3-01 | Blocking | `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/NaturalProcessContracts.cs:59`, `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/WoodlandFootprint.cs:15` | On SDK 8.0.133, LINQ `ToArray()` passes its destination to a source implementing `ICollection<T>.CopyTo`. A custom input implementing both `IReadOnlyList<T>` and `ICollection<T>` copied all items correctly and recorded that destination. A write to the retained destination after construction changed a StormRequest, its plain `with` copy, a request made with an init replacement, and a JayDispersalResult. A footprint built from four points also changed from 6,400 to 4,800 m². The repaired `Vertices` view uses a second copy and still reported (80, 80) at index 2 while the geometry used (80, 40). The input alias existed on the parent, and the delta introduces the visible mismatch between geometry and the reported vertices. Copy through enumeration or indexed reads without handing the final storage to the input; use one verified snapshot for both the outline and the geometry. Add a regression whose `CopyTo` saves the destination and exercise construction and init replacement. |

The independent probe used a small custom collection whose `CopyTo(T[] destination, int index)` calls `items.CopyTo(destination, index)` and stores `destination` in a property. The footprint input was the usual four points for the 80 × 80 m rectangle. Writing `new LocalPoint(80, 40)` to index 2 of that retained destination after construction reproduced the footprint failure. No reflection was needed to reach storage in this case.

### Prior findings and regression

| ID | Status | Confirmation |
|---|---|---|
| R1-01 | RESOLVED | JW-T15/T16 passed. Reserved-prefix pre-draw refusal and mixed-origin identity and keys are unchanged. |
| R1-02 | PARTIALLY RESOLVED | Output cast and SyncRoot paths are closed. R3-01 still breaks input isolation. |
| R1-03 | RESOLVED | JW-T44/T45/T48 passed. Extracted-ID checks in both stock and additions are unchanged. |
| R1-04 | RESOLVED | JW-T49 passed. The atomic pricing refusal and numeric bounds are unchanged. |
| R1-05 | PARTIALLY RESOLVED | Repair 3 is identified as the current transcript and both rulings are explicit in the handoff. The prior nonblocking stale labels remain in the handoff branch bullet at line 16 and spec §1.3 at line 50. |
| R1-06 | RESOLVED | Strengthened JW-T06/T09/T23/T24/T27/T62 passed. |
| R1-07 | OBSERVATION ONLY | Across 19 storms, direction exactly matched the frozen RNG draw widened before multiplication by 360 and equalled the glyph. Spec §5 item 10 retains the Part 2 persistence precision decision. |
| R1-08 | RESOLVED | JW-T47 passed. The dated teaching constant is unchanged. |
| R1-09 | RESOLVED | Spec §5 items 7–9 queue on-map debit, persisted once-per-year storm resolution and authoritative Work Plan approval with grouped settlement. |

### Changed-area criterion matrix

| Criterion | Result | Evidence |
|---|---|---|
| Candidate identity, clean tree, base, fence and standalone suite | PASS | Exact local and live refs, 23 of 23 fenced paths, zero warnings/errors and 48 of 48 tests. |
| Read-only collection surface and enumerators | PASS | 17 exposed collections audited. The parent SyncRoot reproduction fails at the candidate cast. |
| Defensive snapshot with ordinary arrays and with-copies | PASS | All record collections audited for caller-array mutation after construction and init replacement. Ordinary footprint array input leaves 6,400 m² intact. |
| Defensive snapshot across the public input types | FAIL | R3-01 on caller-implemented collections, including records, init replacement and footprint geometry. |
| Record sequence equality and matching hashes | PASS | Independently constructed records, plain copies, replacement lists, and reordered, missing and different inputs checked under the documented footprint identity rule. |
| On-map debit and Nearby external input | PASS | Spec §§1.2 and 5 agree. |
| D-050, direction, calibration visibility and gameplay regression | PASS | JW-T60/T61 passed. Storm vulnerability source and sampling formulas are unchanged. `Severities` and `Weights` are fields on the internal `StormVulnerability` type and public storm methods do not return either array. The independent parent/candidate fixture matched all recorded dispersal, storm and salvage outputs. |
| Current handoff and ruling labels | PARTIAL | Carryover R1-05 wording. |
| Unity, Part 2 runtime, save/load, grouped settlement and dashboard sync | NOT VERIFIED | Standalone review and bounded review-branch publication. |

SDK `8.0.133`, from `Tools/MultiRackSitkaP1Lab`:

```sh
dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
```

Build exit 0, 0 warnings and 0 errors. Run exit 0, 48/48 passed (20 jay, 12 storm, 11 salvage, 5 hygiene/parity). Temporary external probes compiled the candidate and parent separately. One regression probed 1,000 years of storms and salvage with 400 trees and 100 years of jay dispersal on each edge. Both versions produced SHA-256 `74FBBD8B33ACFD660DA4C63FFBD91761A8ED516492AF77BCBF7332039F10C62F` over serialized results. Direction was checked against `SimulationRandom.Roll` from the frozen context. The probe projects emitted the expected unused draw-log warning because they omit the suite's observer. The required build was warning-free.

## Final re-review (ca601d8d -> 923634c6)

**APPROVED FOR INTEGRATION.** G2 acceptance for standalone Part 1 at `923634c62f18444af1b57206d7ee87981958b7b4`, based on `c82e4e759c2ceef1c13ddb6a033e034097aef77e`. The feature worktree opened clean. HEAD, the local origin-tracking ref and GitHub's live feature ref matched. All 23 paths fit the four spec, source, test and handoff prefixes.

### R3-01 — RESOLVED

JW-T65 passed. A separate .NET probe compiled the parent and candidate sources from Git in temporary directories. The probe supplied its own `IReadOnlyList<T>` / `ICollection<T>` implementation whose `CopyTo` saved the destination array after copying, then attempted to overwrite every retained element. The LINQ control retained one array and overwrote it. The parent handed arrays to the caller in every checked area, and the writes changed the footprint, jay and storm requests, salvage requests and a directly constructed jay result.

The candidate called the caller's `CopyTo` zero times across all cases. Jay and storm requests, plain `with` copies, init replacements and resolved results stayed equal to their baselines. The same held for salvage stock, orders, extracted IDs, request copies, a successful merge and merge refusals for extracted IDs in existing stock and additions. Standalone exposure feedback and direct result construction and init replacement stayed stable. Footprint vertices stayed identical, containment at (70, 70) remained true, and area remained **6,400 m²**.

Every materialization in `src/Core/NaturalProcesses` was checked. `SnapshotCopy.ToArray` reads caller elements through enumeration into a list it owns. Only that list's `ToArray` allocates the final contents. `ContractSnapshot.Of` routes record constructors and init setters through it, preserving null values where their existing contract allows them. Footprint geometry and the exposed outline derive from the same private snapshot. Exposure sorting and merge materialization consume private arrays. Remaining LINQ materializations and the salvage `HashSet` constructors consume snapshots or library-owned intermediates. No source-taking `List<T>` constructor or caller `CopyTo` destination was found.

### Prior findings and regression

| ID | Status | Confirmation |
|---|---|---|
| R1-01 | RESOLVED | JW-T15/T16 passed. The ordinal reserved-prefix check precedes draws and mixed-origin identities and keys remain distinct. |
| R1-02 | RESOLVED | JW-T17/T31/T50/T65 and the retaining-collection probe confirm input isolation and immutable public views, including footprint geometry. |
| R1-03 | RESOLVED | JW-T44/T45/T48 passed. The probe confirmed that either placement of an extracted ID returns a refusal with existing stock unchanged. See the enumeration-order observation below. |
| R1-04 | RESOLVED | JW-T49 passed. Atomic pricing refusal and numeric bounds are unchanged. |
| R1-05 | PARTIALLY RESOLVED | The owner rulings are explicit in the handoff. Its line 16 branch bullet and spec §1.3's line 50 review label are still stale, and the implementation handoff lacks a Repair 4 section. Carry these as a non-blocking follow-up docs-only commit. |
| R1-06 | RESOLVED | Strengthened JW-T06/T09/T23/T24/T27/T62 passed and their assertions are unchanged. |
| R1-07 | OBSERVATION ONLY | JW-T25 passed. Direction uses the same draw times 360 in double, blow-towards, with glyph equal to direction. The raw Unity float-bearing precision choice remains queued for Part 2 persistence in spec §5 item 10. |
| R1-08 | RESOLVED | JW-T47 passed. The dated non-causal note is unchanged. |
| R1-09 | RESOLVED | Spec §5 items 7–9 still queue on-map debit including exports, persisted storm resolution and authoritative Work Plan checks with grouped settlement. |
| R2-01 | RESOLVED | JW-T17/T31/T50 cover mutable interfaces and SyncRoot, and the snapshot wrapper and enumeration surface are unchanged. JW-T65 and the probe close the remaining caller-side alias. |
| R2-02 | RESOLVED | Spec §5 item 7 still defines the debit on on-map seed and accounts for delivered Nearby seed as separate external input. |
| R2-03 | RESOLVED | JW-T17/T31/T50 passed the sequence equality assertions. The independent probe checked equal records and hashes for plain copies and equivalent replacements for jay, storm and salvage. The footprint identity rule is unchanged. |

### New findings

| ID | Severity | Location | Evidence and effect |
|---|---|---|---|
| R4-01 | Low, non-blocking | `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/SalvageAccounting.cs:80` | Additions are enumerated before the existing-stock validation. With an extracted ID in existing stock, a counting additions list was enumerated once on the candidate and zero times on the parent. Both returned `DeadwoodAlreadyExtracted` with unchanged stock. An additions iterator that throws could mask that refusal. Deferring the additions snapshot until after existing-stock validation would preserve the previous short circuit. |
| R4-02 | Low, non-blocking | `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/StormAdapter.cs:52` | `ExposureBands(null!)` changed from `ArgumentNullException` to an empty snapshot. The parameter is declared non-nullable, so this affects callers outside its declared input contract. An explicit null guard would preserve the prior failure. |

### Changed-area criterion matrix

| Criterion | Result | Evidence |
|---|---|---|
| Exact head, parent, base and four-prefix fence | PASS | Clean entry, exact local and live origin refs, 23 fenced paths and clean diff whitespace check. |
| Standalone build and suite | PASS | SDK 8.0.133, build exit 0 with 0 warnings and 0 errors, test exit 0 with 49/49 passed. |
| Caller input isolation, public read-only views and record copies | PASS | JW-T17/T31/T50/T65 and the external probe. Every caller `CopyTo` counter stayed at zero. |
| Footprint view, area and containment | PASS | The overwrite attempt left the vertices intact, (70, 70) inside and area 6,400 m². |
| Merge success, extracted-stock and extracted-addition refusals | PASS | Independent probe and JW-T44/T45/T48. Enumeration timing is recorded in R4-01. |
| Frozen D-050 rolls, BoundedRational, direction and glyph | PASS | JW-T22/T25/T27/T60/T61 passed. Failure transform and event sampling are unchanged by the delta. |
| Ordinary simulation outputs across the delta | PASS | Serialized storm and salvage results over 1,000 years with 60 trees and jay results over 100 years on all four edges had the same SHA-256 on both revisions, `87855F1CDA03DC1BFCBE98E3644F27BF572305BF9A4B2B1ADB5AFA9383ECA980`. |
| Edge-case compatibility | PARTIAL | R4-01 and R4-02 record the two observable changes. |
| Implementation handoff and ruling labels | PARTIAL | R1-05 and the missing Repair 4 section remain a docs-only follow-up. |
| Unity, Part 2 runtime, save/load, grouped settlement and dashboard sync | NOT VERIFIED | The review gate covered the standalone Part 1 engine. |

Required commands from `Tools/MultiRackSitkaP1Lab`:

```sh
dotnet --version
dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
```

Suite totals were 20 jay, 12 storm, 11 salvage and 6 hygiene/parity. Both scratch compilations emitted the unused draw-log observer warning because the probe omits the suite's observer. The required project build emitted zero warnings.
