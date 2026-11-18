# Multi #740 Lane E — shared immutable snapshot and integration/security readiness — specification DRAFT result

**18 November 2026 | SPECIFICATION/ARCHITECTURE PROPOSAL ONLY | STATUS: DRAFT — NOT APPROVED, NOT IMPLEMENTED**

**Claim:** [#740](https://github.com/jermur788/CCF/issues/740); **dedicated Multi Manager:** [#730](https://github.com/jermur788/CCF/issues/730); **binding dispatch:** [complete Lane E packet](https://github.com/jermur788/CCF/blob/docs/multi-730-parallel-workstreams-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-PACKET-20261118.md).

**Worker actually registered:** OpenAI GPT-6 / ChatGPT Game Dev, browser-only GitHub connector; connector identity jermur788. Reasoning-effort setting and session identifier not exposed; neither is claimed. [RUNNING registration](https://github.com/jermur788/CCF/issues/740#issuecomment-6979713011) precedes this file. Starting fresh GitHub main HEAD: b7421c9a5cbf63a1409fe7ae8eed304fdfd4a7e9. Separate documentation branch: docs/multi-740-e-spec-draft-20261118-v1. **Physical lane/worktree:** none; no CCF-Ops ownership, source-access grant, source build or runtime execution. Only this new Markdown file is written. Live main coordination was inspected, not edited; original local #710/#725 protected source was not entered.

**Evidence class:** source-derived contract proposal / simulation interface design, **not** observed running UI, integration, persistence, Irish empirical data, Unity parity, security qualification or product acceptance. Names, API surfaces and types below are **proposed**, except where explicitly identified as already recorded in the cited Year-1 evidence.

## 1. Executive determination and the five-sentence minimal contract

1. A Multi 2D viewer receives an **immutable, versioned and fully detached projection** of a successfully admitted experiment state, never a reference to the annual engine's writable tree collection.
2. Every tree is addressed by stable source **tree ID** within a uniquely identified experiment/domain/fixture lineage, independently of array ordering, viewport location, rendered sprite or whether the tree is alive or actionable.
3. All forest positions and distances originate in the declared source **X/Z metre plane** with explicit geometry, origin, area and numeric semantics; a viewport transforms those positions into pixels without changing simulation state.
4. A snapshot distinguishes **Year 0**, **Year 1**, and **Before / PostGrowth / AfterMortality** observations and carries source-precision fields, model provenance and separately defined stand totals, while all displayed rounding remains non-authoritative.
5. Rendering, inspection and reversible marking proposals may read IDs and snapshot revisions but **never execute work, advance time, persist state, open a Host or bypass trust/security gates** by virtue of receiving that snapshot.

**Immediate recommendation, not owner decision:** define one narrow read-side interchange between the existing qualified Year-1 report and future independent 2D lanes first, keeping commands, saves and the network Host outside it. A provisional interface was posted to [#740 for A/C/D cross-lane inspection](https://github.com/jermur788/CCF/issues/740#issuecomment-6979762166). Manager #730 must reconcile any conflicting proposals and commission review before approving an executable specification.

## 2. Authority, immutable evidence and current truth

| Source | What it actually establishes | What it does NOT establish |
|---|---|---|
| [#725 author result](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md) | Five-file local annual adapter; immutable in-memory Year-0→Year-1 commit; source-derived 30m adult Sitka, author 43/43 checks and deterministic JSON. | A pushed production source branch, durable save, browser UI, Unity parity, L4 clearance, Year 2. |
| [#729 independent review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md) | APPROVED source/numerical review of exact five changed files; 700 per-tree values and all 100 RNG numerators independently recomputed, source ordering and stand units checked. | New feature approval or native runtime/release result. M-1 is a mandatory living-tree fix before Year 2. |
| [#732 focused native qualification, final amended report](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md) and [Manager verdict](https://github.com/jermur788/CCF/issues/730) | First network-contact STOP preserved as history; subsequently approved offline resume with exact P0 DLL, build 0 warnings/errors, two identical 45,980-byte golden reports, original-domain and tamper refusals, 700/700 check. | Full 43/98-test re-run, L4 success, save, new years, UI or integration. |
| [#735 retention](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md), [Manager acceptance](https://github.com/jermur788/CCF/issues/735#issuecomment-6979208355) | Original #725 qualified source retained, clean and immutable through **27 November 2026 18:00 UTC**, with explicit owner disposition/no auto-purge; original repaired R #710 remains separately retained through **23 November 18:00 UTC**. | A fresh source-writing grant, permission to access those native stores, an approved import/merge or a remote source branch. |
| [owner four-file Multi P1 G2 approval](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md) and [approved P1 spec candidate](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md) | Owner explicitly approved **those four files** for bounded separate P1 tasks, with derived 30m first, original P0 read-only, Unity 40/80 oracle later, source reuse and strict security design. | Automatic approval of the *new* shared-snapshot/UI/persistence specification or of P1 code integration. Historical DRAFT text inside the approved file does not override its top current approval preface. |
| [main Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), [main Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), [main Game Brief](https://github.com/jermur788/CCF/blob/main/Docs/Project/game-brief.md) and [spec rules](https://github.com/jermur788/CCF/blob/main/specs/README.md) | Current main is canonical product policy; main D-082 governs the experiment's synthetic origin, conditional stale approvals and honest warning. Scenario One owns the current main D-084. DRAFT specs do not authorise implementation. | Reclassification of the separate synthetic 2D Multi experiment as Scenario One Compact30, or automatic source integration because both contain a 30m square. |

**Qualified local source identity, for future custody checks only:** repaired R parent 35d26f5e64a00a400c700871927ccc2173370568 (tree 33f84db2e7fdac527c343a758775757f49687c42), #725 child df467dcc302eca564343e6da40f2fb88529a5d16 (tree 4237ece5c4efcd167857c1cb29e16bbcc2279ad6); exact five-file delta confined to Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs, AnnualRules.cs, src/Cli/Program.cs, tests/Core.Tests/AnnualTests.cs and tests/Core.Tests/Program.cs. Frozen derived fixture SHA-256 bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4. These are forensic comparison pins, **not** permission to reparent, check out, modify or publish the retained native stores.

**Security status:** default .NET L4 = **FAIL38, UNWAIVED**. #725's narrow traced annual CLI still found unresolved ftruncate on memfd:doublemapper and fchmod on a socket under the unchanged analyzer. #732's offline network exclusion resolves only the limited qualification network-contact incident; it is NOT L4 clearance. Do not change the runtime, guards, analyzer or accepted P0 invariants to make a gate appear green.

## 3. Canonical identity and schema (proposal V1, no durable API adopted)

**Suggested name for independent review:** MultiSitkaReadSnapshotV1. This is a **read-model envelope**, not a save/archive/event schema or a renaming of #725's existing CLI JSON. A first adapter must be reviewed against the actual retained #725 source and report before its fields are declared implemented. Unknown required fields, versions, enum values, unsafe numbers, duplicate IDs or cross-domain substitution **refuse**; no silent default of missing model identity.

**Record families and relations:** one immutable Header, zero or more immutable TreeRows ordered by **ordinal treeId**, up to three named StageAggregates for observed stages, optional ReadAudit rows and non-authoritative DisplayHints. Dead trees remain identifiable rows; the **living subset** alone participates in future biological calculations (M-1). Stage aggregate counts, where emitted, must reconcile against the relevant stage's living set and explicit declared stand area.

### 3.1 Header: identity, source plane, numeric and model provenance

| Field (proposed) | Type / units | Required meaning and boundary |
|---|---|---|
| schemaId, schemaVersion | fixed string + integer | Distinct read schema identity and major version; reject unknown major. Not P0 format and not a P1 save version. |
| experimentId, generation | opaque ID + nonnegative revision | Scoped experiment instance and state-generation/restart fence. Generation prevents stale selections after a different genesis/load; **not currently evidenced as implemented**. |
| domain | discriminated enum | Exact modes: DerivedP1ThirtyMetreExperimentV1 / DerivedUnitySquareParityV1 / OriginalPolygonExploratoryV1. In #725 only **the first** admits annual advance; second is unavailable, third inspect-only/annual-refused. |
| fixtureId, fixtureByteSha256 | exact catalog ID + 32-byte digest | Immutable accepted input identity, distinct original-vs-derived catalogs. No caller-provided source tree may bypass admission. |
| sourceClass, disclosure | enum + unaltered warning | Synthetic/derived/original provenance and prominent experimental NOT-Irish-forecast, NOT-Unity-parity disclosure. Original fixture is independently synthetic; “original” does not mean empirical Ireland. |
| originEvidence, sourceCommitOrReviewRef | structured provenance references | Reviewed source/capture/fixture derivation or designed initial values; ref is provenance only, never a runtime lookup, trust substitute or source-authority upgrade. |
| modelVersions, rngModelVersion | explicit version set | Annual algorithm, growth, density, mortality, fixture/catalog and RNG identity; compatibility must match reviewed source. Do not infer from display defaults. |
| numericPolicyId, runtimeFingerprint | explicit string/structured record | Pins f32/f64 operation and storage points plus arithmetic-relevant runtime information for exact replay only when qualified; A-prime cross-host compatibility remains review-dependent. |
| ecologicalYear, inventoryAgeBasis | integer years | State year and provenance of per-tree ages. A post-growth stage within Year 1 is NOT a fresh annual year. Never use browser wall-clock as ecological time. |
| seed | exact integer plus keying version | Source hazard/RNG seed key, not display-randomness seed. Any client visual variation must have its own separate deterministic namespace. |
| coordinateSystem | source-plane enum | **Source X/Z**, metres, axes/origin/sign declaration and handedness. Do not silently map source XY to pixels or assume a compass bearing not in the fixture. |
| geometryKind, bounds, standAreaM2 | mode-specific shape + exact units | #725 admitted 30 × 30m square, nominal 900 m². Future polygon/rectangle must retain actual geometry, not bounding square. Bounds are not an alternative area. |
| standAreaHaBits | f32 raw 8-hex bits | Actual area used by annual engine for per-hectare reductions; Year-1 fixture **3db851ec** = 0.0900000035762786865234375 ha; do NOT recompute against exact decimal 0.09. |
| snapshotId, predecessorId, sourceStage | digest/revision + optional prior + enum | Identifies immutable observation, and whether Before, PostGrowth or AfterMortality; must not pretend unavailable intermediate stages are authoritative saved states. |
| acceptedInputDigest, status | digest + explicit admission/status enum | Bind source, stage and model; status distinguishes accepted completed state, refused/unavailable mode and proposal. A refused request has no tree snapshot. |

**Origin policy:** local source x/z values are not pixels. Do not adopt screen origin (0,0), Y-down coordinate inversion, east/north compass mapping, zoom, aspect ratio or pixel rounding as scientific or persisted geometry. The original P1 spec contains an XY→XZ source-coordinate discussion; a future reviewed adapter must report the actual accepted transform and exact source bits in the header. Do not fabricate the actual world coordinate transform from this proposal.

### 3.2 TreeRow: field-by-field proposed immutable read contract

| Field (proposed) | Type / units | Rule |
|---|---|---|
| treeId | ordinal UTF-8 string | Stable within fixture/experiment across all years, stages, views, labels and audit; never array index, new sprite ID or hitbox ID. Exactly one row per registered ID in a complete snapshot. |
| speciesId | controlled enum/string | #725 accepted adult population is pure **Sitka spruce**; no inferred Oak/Beech or juvenile rows. Future species require new authorised model. |
| sourceLifeClass, ageYears | named class + integer years | Mature-equivalent adult inclusion, age 30→31 in qualified Year 1; age is not marking eligibility. |
| xBits, zBits | canonical f32 hex or exact manifest integers with scale | Source X/Z metres, exact conversion declared by header; immutable under normal adult growth. Do not derive from sprite placement. |
| alive | boolean | Biological living status. Remains explicit for dead historical rows. No renderer may substitute invisibility for biological mortality. |
| dbhCmBits | f32 hex / cm | Diameter at breast height source float; below-action threshold trees stay in annual biology. |
| heightMBits | f32 hex / m | Height; do not equate with screen sprite height or measured top-height statistic. |
| crownRadiusMBits | f32 hex / m | Source crown state, not CSS/canvas pixel radius; clamp and relaxation determined by model, not renderer. |
| preGrowthCompetitionIndexBits | f32 hex / dimensionless | Explicit pre-growth Hegyi CI for the Year-1 advance, tied to source calculation stage and model. Do not recalculate from changed post-growth crowns and relabel it pre-growth. |
| neighbourCount | nonnegative integer | Count at source's inclusive final 8m cutoff/0.5m floor; observational diagnostic, not map hit count. |
| lastNaturalDeath | optional {year,cause,mechanism} | Natural mortality if observed: year, biological cause self-thinning, optional hazard/boundary mechanism. Zero observed deaths in frozen Year 1. Do not conflate a Fell job with natural death. |
| lastExecutedFell | optional authorised work-event ref | **Future** executed operation only. Proposed mark, approval or display filtering never writes this. #725 has no felling/replay implementation. |
| actionableHint | optional derived boolean + reason | Only a UI eligibility view, derived from an approved work rule; it cannot decide whether this living tree is included in biology. The initial 12 trees below 170 mm DBH illustrate this separation. |
| auditRef | optional structured read reference | References per-ID year comparison or work provenance; no implicit authoritative event chain. |
| presentationTags | optional non-authoritative values | Derived colour/form/selection labels; excluded from simulation hashes and from all work/biological decisions. |

**Exact-number rule:** authoritative f32 values use raw 32-bit hex, f64 values use raw 64-bit hex; year, count and RNG numerator are integers. Human-readable decimal displays may accompany them but **do not become identity or recomputation inputs**. This follows the approved P1 proposed canonical-bit principle; the eventual snapshot schema still needs a reviewed choice on whether to transmit bits as strings, bytes or both. The current #725 JSON's actual property names and representation are **not asserted** to match this table. Display labels such as “170 mm” are an action eligibility boundary, not a species growth threshold.

### 3.3 StandAggregate and AnnualAudit: stage separation

| Field | Units / exactness | Requirement |
|---|---|---|
| stage, ecologicalYear | enumerated observation + integer | Before (= Year-0 input), PostGrowth (= same annual transaction after adult growth/crown), AfterMortality (= committed Year-1 living set); distinguish stage names from independent years. |
| livingStems, totalRegisteredIds | counts | Living count excludes dead; total ID inventory retains dead rows. Report both, never infer 100 living forever. |
| stemsPerHa | source f32 or declared numeric kind | Count divided by actual f32 area; exact type/operation must be as inspected, not silently rounded. |
| basalAreaM2PerHa | f64 source accumulation | Sum squared DBH of living trees with correct pi/40000 and actual f32 area as source; do not use exact decimal 0.09. |
| quadraticMeanDbhCm | f64 | Explicit from matching living-count and BA/D² formula, not arithmetic average DBH. |
| relativeDensity | f64 | Source density measure by stage, not canopy cover or a measured Irish yield indicator. |
| postGrowthPressure, meanVulnerability | f64, optional | Year-1 diagnostic values with calculation-phase provenance; absent/unsupported in stages where not computed. |
| directedHegyiPairs, maxCompetitionIndex | count and f32 with treeId | Source pair and maximum pre-growth CI, for audit of source rule; not proof of ecological validity in Ireland. |
| hazardSum, maximumHazard | f64, optional | Calculated model hazard metrics, not observed field mortality probabilities. |
| naturalDeaths, fellExecutions | ordered typed event IDs | Distinct outputs; in Year-1 frozen input naturalDeaths=0, fellExecutions=0. Future work events require approved work grammar. |
| annualRollAudit | optional per-ID exact RNG numerator/model | Future audit adapter may expose the 24-bit integer numerator only when source actually did a roll; zero-probability cases skip a roll. Do not synthesize a roll-call event from a helper. |

**Complete vs partial read contract:** a Complete snapshot promises header + all rows + reconciled aggregate of that exact stage; a Detail request may return selected tree IDs but must declare its base complete snapshot ID. No partial row set may be presented as a complete stand, used to calculate forest RD or silently passed to next-year biology. Any projected UI “before/after” comparison is a read-only pair of identified completed source observations; it never replays authoritative actions.

## 4. View-model and API boundaries (all names PROPOSED)

**Proposed logical interface, not existing executable source:** ReadCurrentSnapshot(experimentKey, requestedStage) → Either {accepted, complete immutable snapshot} or {typed refusal}; ReadTree(snapshotId, treeId) → a detached immutable row; SubscribeCommittedSnapshot(callback) → notification after the **single authoritative annual commit**, containing snapshot ID only; ReadComparison(beforeSnapshotId, afterSnapshotId) → two immutable projections with matching domain and fixture identity. These are four responsibilities, not a request to add four methods without review.

**Snapshot construction:** a future **Core-owned** projection adapter may copy from the trusted #725 immutable state or from its exact audited CLI result only after successful admission and commit. On annual failure, cancellation or stale revision it publishes **nothing** and does not give a caller a half-year tree set; a revision token prevents mixing rows from Year 0 and Year 1. The adapter must distinguish a source observation from a recomputation. It can be materialised inside one process or through a separately approved read-only data transport; neither an assembly nor an HTTP API path is selected by this DRAFT.

**Viewer A / #736:** consumes Header, TreeRows, StageAggregates and source-plane transform hints; owns viewport size, zoom, pan, sprite/layer styles, hover/inspection and accessibility labels; must not mutate Core or interpret a pixel scale as metres. A sprite remains keyed by treeId plus snapshot revision. A may propose separate visual filters for dead, alive and non-actionable stems but may not alter biological inclusion.

**Marking UX C / #738:** may keep a **local reversible, non-authoritative ProposalSelection** keyed by experimentId, generation, snapshotId and treeId with a mark type/status and user change sequence. Proposal changes are UI state, not work orders; a snapshot revision invalidates/rebases the preview only under an explicitly approved policy. No Approved work, Fell, natural death or in-flight engine mutation from pointer clicks. Future command submission, approvals and execution must be a separate reviewed service with ID validation, one-time commit and actual work-policy ownership; no endpoint is implied here.

**Time B / #737:** sole future biological writer for annual state; owns live-tree filtering **before** CI, growth, stand aggregates and mortality (M-1), annual order, deterministic seed/version and publication. It must not treat a read-model round-trip as source-of-truth. Year 2 requires its own approved implementation and new independent tests; #725's one-year memory engine refuses a second advance.

**History/persistence D / #739:** may read typed observation links and future persisted event receipts, but no save schema, replay chain, chronological event list or Host is created by E. D owns proposals for archive/load fences, session-bound stale approvals (per main D-082), idempotent command IDs, rejected-command policy, event identity, durability failures, full replay and history projection. Save must not be inferred by serialising this read snapshot. A read event/visual proposal cannot be retroactively labelled executed work.

**Origin/viewport mapping proposal:** UI holds a separate versioned 2D affine transform, including canvas dimensions, pan and zoom; it maps source (x,z) metres to screen pixels and inverts for hit testing. Geometry bounds/hulls remain the source representation. A/C must share a single UI transform contract or separately document deterministic equivalence; neither may round-trip pixel x/y back to engine storage. If a selected ID is hidden by a layer or a live snapshot replaces its predecessor, UI selection is updated by stable ID under an explicit policy rather than by tree-array index.

## 5. File, assembly, API and custody ownership matrix

| Surface / file family | Sole proposed later writer | Lane E authority now | Cross-lane rule |
|---|---|---|---|
| Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs, AnnualRules.cs; trusted fixture catalog | **B / dedicated Core source implementer only after Manager source dispatch** | Read GitHub evidence only; no code writing | Source is local #725; do not touch retained original or publish an ad hoc replacement. M-1 before Year 2. |
| Proposed Core/ReadSnapshot projection type + exact versioned schema, source-to-read adapter | **One designated future Core-contract writer** chosen by #730 (could be B or separate integration lane) | Propose field/schema contract; **not** implementation ownership | A/C/D consume a reviewed frozen interface; no duplicate classes in each lane. |
| Future 2D renderer HTML/canvas/assets or dedicated UI assembly | **A / #736** | Interface review only | Read-only painting and hit tests; no engine reference mutation. |
| Future proposed-mark selection UI/workflow surface | **C / #738** | Interface review only | Proposal UI distinct from approved/executed work and Core mutation. |
| Future annual ledger/history/archive/file/Host transport | **D / #739** for design, later one specifically assigned writer per file set | Interface and security design only | Cannot claim persistence from a read JSON report. Coordinate command/event/versioning with B and C. |
| P0 lab Tools/MultiRackSitkaLab/**, original #710 repaired R, protected #725 stores/fixtures | **Existing physical owner/custodian only** | None | Immutable by default; protected under L0–L4 and specific owner retention. |
| Main Docs/Project decision and coordination | **Manager #730 / authorised canonical integrator** | Read only | #740 never edits live active-tasks or Decision Log. |
| This result Markdown | **E / #740** | Exclusive writer | Exactly one new GitHub Markdown on isolated docs branch; no edits by other lanes. |

No existing compiled assembly reference, public API namespace, binary distribution or host port is asserted in this ownership table. Names must be checked against the exact imported source under a future authorised native read grant. File paths for future renderer/Host are *not* assigned by this proposal; no lane may infer a writable path from a conceptual name.

## 6. Three distinct domains and pinned Year-0/Year-1 reference witness

**Domain refusal is a first-class contract.** The accepted P1 owner-approved phase plan distinguishes (a) 30 × 30m **derived synthetic** adult Sitka experiment, (b) 40/80m **derived Unity-square reference** for later separate Editor capture, and (c) verified original P0 **polygon/exploratory** fixture for read-only import until later true polygon annual support. A UI must never rename (a) as a production Scenario One Compact30 forest just because both have a 30m square; (b) has no accepted annual parity yet; (c) must reject annual advance now. No polygon bounding-square approximation.

**Frozen first read/view witness** (directly from #725/#729/#732, not a new computation):

| Check | Year 0 / stage | Year 1 / stage | Contract significance |
|---|---|---|---|
| Domain/fixture | DerivedP1ThirtyMetreExperimentV1 / p1d-30m-sitka-adult-v1 | same input lineage | Strict tag and digest; no original substitution. |
| Source area | 900 m², areaHa f32 bits 3db851ec | unchanged | Distinguish declared float from decimal 0.09. |
| Registered/living Sitka | 100 / 100; 12 below 170mm | 100 / 100, same IDs | No work-eligibility filtering from biology. |
| Age | 30 | 31 | One completed annual age advance. |
| Directed Hegyi pairs | 1,622 on pre-growth state | same pre-growth audit | Inclusive 8m neighbour boundary. |
| Maximum CI | 8.432238578796387 at P30-053 | pre-growth audit remains attached | Cached CI must not become post-growth recalculation. |
| Basal area/ha | 55.87290918844456 | 58.33954510260964 after growth | f32 area divisor; f64 accumulator. |
| Dq (cm) | 25.303288374529917 | 25.855791791844318 after growth | Not mean arithmetic DBH. |
| Relative density | 0.609795828454288 | 0.6375836886879149 after growth | Must not be misreported as canopy fraction. |
| Post-growth pressure | not applicable | 0.09395922171978738 | Model output; not an empirical forecast. |
| Mean vulnerability | not applicable | #729 exact recomputation 0.03541018395366311 | Source f64 sequence; #725 displayed different by a few ULPs in its separate #723 comparison. |
| Hazard outcomes | none yet | 0 hazard, 0 boundary deaths | No synthetic mortality event, no felling. |
| Numerical readback | baseline fixture admitted | two 45,980-byte golden outputs, SHA-256 574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53 | Exact **current CLI** witness; no claim the new V1 read-model serialises to this hash. |

#729's model agrees with the candidate on **700/700 per-tree numbers**, including all 100 model-1 roll numerators; P30-001 model-1 = 2780209 and model-0 control = 14399697. The test should compare numerical bits and discrete IDs/order where supported, with a clearly stated baseline, **not** compare a newly shaped viewer JSON file's bytes to #725's report hash. #729 also corrects the #723 decimal-area baseline: use **3db851ec**, not exact mathematical 0.09 for source reductions. For any optional per-ID read detail, Year 1 reproduction must preserve the admitted 100 IDs and correct alive/age/metric state, with absent audit fields shown as **unavailable**, never invented.

**M-1 is mandatory** before any Year 2 or multi-year projection: #729 observes that the internal Calculate path does not filter IsLiving, so a dead tree would wrongly enter competition, growth and before-stand metrics if a later year were allowed. A future real dead-row fixture must verify living-only CI/growth/BA and stable dead ID/history without erasing the corpse from the read projection. #729's M-2 concurrent barrier and M-3 uncapped hazard/RD=1 vectors are optional test-strengthening notes, not grounds to dilute M-1.

## 7. Trust, security, write barriers and UI Host alternatives

**Admission and trust:** exactly the compiled trusted fixture/domain catalog may open the bounded no-follow reader, enforce allowed ID + byte length + SHA-256, strict schema and accepted status. An unknown original/derived/Unity tag, an altered same-size fixture, a symlink/hardlink/nonregular input or a foreign save must not become a valid read-model source. A UI may display a controlled refusal, not reinterpret it or fall back to a “demo forest” using arbitrary browser JSON. No client can set Core truth by supplying a TreeRow or clicking “Load JSON.”

**Transport:** keep detached immutable data separate from the authoritative game engine. If a read-only browser Host is later chosen, it must have a separate P1 root/session/origin/version; loopback only, exact Host/Origin/CSRF and no P0 port 5187 collision under the approved P1 security contract. Do not imply a port or network service exists today. Cross-origin, external bind, network persistence, OAuth/cloud sync, platform-neutral save portability and file:// page access are **not** granted by this proposal. A local static report can be visualised without any Host but does not by itself provide an authoritative live engine.

**Save/write:** no current P1-F0 save, durable reload, write API, network transport or command router is demonstrated. The approved P1 technical spec demands no-follow descriptor-relative trusted files, separated P1 store and origin, exact/unique command IDs, fail-closed unknown versions, load/approval staleness (main D-082), atomic save, file and directory fsync with truthful **postrename DurabilityUnknown** when applicable, independent adversarial review and verified local filesystem. These are future gates, not implemented properties inferred from an in-memory immutable snapshot. Do not make a display snapshot loadable as a P1 authoritative archive without a separately approved typed migration.

### UI host choices to be decided — all proposals, none owner-approved here

| Choice | Benefit | Cost/risk and scope |
|---|---|---|
| **H-A: offline read-only visualiser of exported exact #725 report** | Smallest first viewer, zero new API write path, useful to A for zoom/pan/inspect and to C for purely local marking preview. **Recommended initial DESIGN candidate** pending owner/Manager acceptance. | Export integrity still verified by the receiving tool; not a running live world, no saved marks, static provenance and absent audit must be disclosed. A browser may need file selection or a local static development server: do not assume file:// fetch permission. |
| **H-B: standalone P1 loopback read-only Host (later)** | Live committed snapshots/selection and future history transport; clear distinction from P0 Host and Unity. | New surface requiring independent origin/CSRF/localhost/file handling/security/L4 qualification; do not adopt speculative port 5188 merely because earlier spec suggests it. |
| **H-C: same-process local desktop/webview viewer** | No socket if true in-process, can consume immutable snapshot directly. | A new dependency/packaging/runtime/persistence/security surface; version and actual native platform support unknown. Not inherently safer or approved. |

**No implied save/Host/network:** A/C/D may design against a *logical* read transport but must not implement or infer an HTTP endpoint, file writer, shared mutable singleton, permission model or real-time streaming. Any host product choice and public Git publication exposure require explicit decisions and separate implementation packets.

## 8. Integration import and rollback: proposed executable gate order, not executed

1. **Decision/authority intake:** Manager #730 reviews this DRAFT and all A/B/C/D proposals for contract conflict, independent non-author review is commissioned, owner resolves product choices, and a new four-file spec is explicitly approved. Register a new production claim/lane for any source code; the #740 browser registration is not production authority.
2. **Native custody and physical read permission:** owner/authorised native custodian reconciles live physical CCF-Ops board/lane, #710 and #725 retention and exact source HEAD/tree; records a new separate read/import grant, physical store/worktree and one-writer allowlist. The GitHub-only Manager or E worker must **not** enter #725/#710 stores or alter their board. Respect no-arbitrary-worker-time-limit owner policy; the specific retained historical custody dates remain real until lawful owner disposition.
3. **One-way verified candidate transfer:** from the *retained local qualified #725 commit*, an authorised native integrator creates a **separate isolated POSIX/ext4 integration Git store/worktree** (no source-store hardlinks/alternates and no unreviewed shared objects). Import exact parent/child/tree and protected fixture/P0 pins with independent manifests and readback; never substitute moving main, merge code straight from a docs branch, or discard the source owner’s dirty/untracked files. The transport mechanism (bundle/archive/verified Git object copy) and exact paths require physical custodian approval; nothing here performs it.
4. **Reconcile canonical lineages without collision:** compare the qualified R→#725 candidate against current main, P0 qualified ancestor and approved unmerged four-file P1 candidate. Determine if the UI contract needs a new revision, conflict resolution or review; update product Decision Log through Manager only after explicit owner authority. Do not let “P1 spec APPROVED on its branch” imply the current main tree already contains that source.
5. **Proportionate isolated Tier-1 and security regression:** under a separately granted candidate, preserve original #725 reference vectors and run only specified new source/contract tests (immutable/stale snapshot, original refusal, 100-ID annual witness, new full Year-2 living-filter vectors if B is in scope, cross-lane interfaces). Perform P0 L0/L1 committed blob/tree and fixture checks and **L2/L3/L3+/L4 as actually required** on the exact candidate and named protected build-input tree; P1 may not write any protected P0 tree, including ignored files, reference DLL or prior evidence. No extra qualification cycles merely because a new document was written.
6. **Default L4 decision is a hard release gate:** the existing **FAIL38 is UNWAIVED**. An exact-authorised security policy change, runtime remediation and independent qualification would require their own approved lane; no declassifying memfd/socket attempts, silently changing dotnet defaults or reinterpreting narrow traces as full L4. If L4 remains failed, mark **BLOCKED FOR RELEASE/INTEGRATION** even if read-model fields match Year 1. Manager may continue separately authorised, non-release, isolated documentary work.
7. **Integration candidate (later):** only an authorised integrator, with non-author source/security review, protected baseline, full diff manifest, rollback plan and exact integration branch, may make a merge/commit/push. Validate one writer, branch/base and source provenance. Preserve the original local #725 store immutably until its custodian explicitly releases it; do not migrate or rewrite historical P0 records. No automatic main merge from #740.
8. **Rollback and stop:** on any mismatch (fixture SHA, P0 tree, five-file delta, executable provenance, unexpected protected write, hostile input acceptance, failed L4, bad refusal precedence, lost/corrupt commit or source/board conflict), **STOP**; preserve original source, original evidence and before-state. Restore only new isolated candidate through its approved recoverable rollback, never clean/stash/reset #710/#725 or unrelated dirty work. Post-rename ambiguous durable-save results are reported DurabilityUnknown, not “rolled back safely” without proof.
9. **Unity and Windows remain separate:** numerical Unity 40/80 C-2b oracle requires actual Editor capture and the separately approved **C-6 owner GO**, exclusive machine lock and new qualification. Current 30m derived source is not Unity-verified and no new Unity task is dispatched. No Windows Player build is authorised by this work; Scenario One's separate personal-test/build gate remains intact.
10. **Completion reporting:** integration reviewer issues APPROVED / NEEDS CHANGES / REJECTED only against exact evidence; distinguish IMPLEMENTED, SOURCE REVIEWED, FOCUSED NATIVE QUALIFIED, INTEGRATED, VERIFIED, ACCEPTED and RELEASE READY. Update authoritative coordination/dashboard via Manager; a docs-branch result alone is not published integration.

## 9. Testable acceptance criteria for the future shared contract (all NOT RUN here)

| ID | Proposed acceptance witness (exact candidate required later) | Negative/failure witness |
|---|---|---|
| E-01 Source admission | One accepted derived 30m fixture yields matching digest/mode/model with 100 ordered unique IDs | Unknown mode, original annual, unavailable Unity mode and altered same-length fixture refuse; no forest fallback |
| E-02 Immutable ownership | Two readers holding same committed version cannot change engine state, and a failed annual computation publishes no new revision | Mutation attempt, partially constructed row or stale generation cannot affect committed state |
| E-03 Identity | All 100 IDs stable Year0→Year1; 12 below 170mm still in biology; selection key survives sort/pan/zoom | Duplicate/missing ID, wrong experiment ID, dead selection or list-index substitution cannot execute work |
| E-04 Geometry | X/Z metre source positions are preserved; reversible viewport projection acts only on UI state | Pixel round-trip, axis/sign swap, boundary-square substitution or renderer radius cannot change source geometry |
| E-05 Source numerics | Before and postgrowth aggregates match table in §6, including area f32 3db851ec; 700 independently reviewed per-tree fields exact as supported | Division by exact 0.09 or rounded display DBH changes reference and is detected |
| E-06 Chronology | Year0 ages 30, Year1 ages 31, 1,622 pairs, cached pregrowth CI, source-order growth/crown/density/mortality and 0 deaths | Labeling pregrowth CI as postgrowth computation or creating a second Year1 commit rejected |
| E-07 Provenance | Disclosure accompanies every viewer/chart/export/selection; original vs derived vs Unity tagged distinctly | “Irish forecast,” “Unity numerical parity,” or “original field plot” claim blocked without evidence |
| E-08 Read-only work boundary | Proposal marks are reversible read-side selections; approved/executed work requires a separate future command gate | Click/hover/drag never Fell, age, grow, save, create PlanApproved or alter biology |
| E-09 Future living-only policy | After authorised Year2 M-1 fix, a dead registered ID stays readable but does not enter living CI/growth/BA/mortality | A dead-row sentinel in input exposes false competition/aggregate; must fail until fixed |
| E-10 Host/save boundary | No read API, static viewer or report is accepted by P1 as a save/command; future Host requires reviewed loopback/origin/CSRF and typed trust | Foreign/cross-version file, stale command, unsupported fs, symlink/hardlink/FIFO and postrename failure follow typed refusal/unknown semantics |
| E-11 P0 security/isolation | Exact P0 blob/fixture and output-tree evidence preserved; L0–L4 directly exercised to required level before promotion | Any protected tracked/ignored write, filesystem unsafe condition or L4 FAIL38 blocks promotion |
| E-12 Release authority | Independent review + owner approval of new four-file spec, exact named physical source grant, qualification and Manager integration verdict present | Draft or browser claim alone never grants implementation, Unity or merge |

These are **future executable acceptance requirements proposed for review**, not tests run by #740. The qualified old golden JSON hash is a witness for the old CLI, not the future snapshot wire schema. In particular E-10/E-11 may remain **BLOCKED** pending a real policy/security decision rather than forcing a new test campaign during documentation.

## 10. Dependency and decision/blocker register

| ID | Status today | Decision or evidence needed | Owner / consequence |
|---|---|---|---|
| D-1 Canonical numbering conflict | **BLOCKING TO CANONICAL PROMOTION** | Main [D-084](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) is **Scenario One Compact30 shelter/teaching/finance**, while [unmerged Multi P1 owner's D-084](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/Docs/Project/decision-log.md) used the same number. Do **not** overwrite/repurpose main D-084, or pretend approved Multi P1 product decision has merged. Manager and owner must choose a **new unused canonical Multi ID** and publish an explicit historical alias/crosswalk with the owner’s original receipt, retaining both meanings and approved four-file spec lineage; this DRAFT does **not** assign an ID. | Manager #730 + owner/canonical editor; until then cite “Multi P1 owner approval 16 Nov” and its URL, not unqualified “D-084.” |
| D-2 Cross-lane schema/API | OPEN / PROPOSED | Review field set, exact precision, origin convention, dead-row semantics, staged snapshots and who implements Core-owned adapter; A/C/D changes return to #740 and #730, not parallel duplicate writers. | #730 after #736/#737/#738/#739 outputs; no coding yet. |
| D-3 UI host choice | OWNER PRODUCT/SECURITY CHOICE OPEN | H-A offline static viewer first vs H-B P1 loopback read-only Host vs H-C desktop/webview; accepted root/origin/transport and eventual packaging needed. | Owner + #730. H-A recommended only as lowest-risk first *proposal*. |
| D-4 Command/event/save boundary | OPEN / FUTURE SCOPE | Whether snapshot is pure projection or needs separate event/delta protocol; approve immutable command IDs, stale selection/approval semantics, file format and durability before any persistence implementation. | #739 history/persistence + #738 marking, #730 final integration; main D-082 remains binding. |
| D-5 Qualified local source access | NOT AUTHORIZED HERE | Authorised physical owner grant, new isolated integration lane, exact source/tree/fixture witness, manager source start. Retained #725 is local and immutable, no remote source push. | Native custodian/Manager; no GitHub-only access or physical lease assumed. |
| D-6 L4 FAIL38 | **BLOCKING SECURITY/RELEASE** | Independent policy-compliant remediation/qualification or explicit separately governed release decision; no silent waiver and no weakened analyzer. | Owner/security reviewer + #730; default is UNWAIVED/STOP. |
| D-7 Year2 living policy | **M-1 MANDATORY BEFORE YEAR2** | Source fix and death-present vectors, independent review, B's approved year/stand history contract. | B/#737 future code lane; Year1 never proves multi-year. |
| D-8 Unity source parity | C-2b/C-6 PENDING | Separate owner GO, exclusive Unity machine, lawful Editor capture and independently set tolerances after evidence; 30m is synthetic, 40/80 remain future oracle. | Owner + Manager; not a docs dependency for E draft. |
| D-9 R/P0 source retention | OWNER CUSTODY, NO AUTO-PURGE | #710 original R 23 Nov 18:00 UTC; #725 immutable child 27 Nov 18:00 UTC; owner disposition at those dates and no arbitrary worker time limits. | Physical custodian, independent of #740. |
| D-10 External claim and dashboard | UNVERIFIED SYNC | Manager review, disposition, formal task close and actual dashboard/Drive reflection, if publication mechanism exists. | Manager #730; #740 cannot claim sync from GitHub-only read. |

**No requirement for a new user decision to review this draft.** There **are** owner decisions before any API/Host/product promotion: choose the UI host/first demonstrable read mode, approve/reject a stable snapshot and work-proposal separation (if it changes the already approved P1 product scope), decide canonical Multi decision numbering through the Manager, and approve any material save/transport/security policy beyond the existing four-file P1 scope. No pending user approval is silently supplied by this handoff.

## 11. Proposed four-file follow-on specification — not created or approved

**Suggested future root:** specs/multirack-sitka-shared-snapshot-integration-v1/ (**new folder**, not an overwrite of the approved adult-rules comparator). Exactly four files if separately assigned after Manager review:

| Future file | Content, approval and verification boundary |
|---|---|
| **spec.md** | Current authority/policy, accepted host decision, normative read-only identity/header/TreeRow/aggregate schema, source-vs-viewport coordinates, trust, UI/selection/save boundary, version/migration/refusal and E-01…E-12 acceptance. Initial status DRAFT until independent review and explicit approval. |
| **plan.md** | Dependency graph and safe physical source transfer; P0 L0–L4/Security L4 gate; Core source adapter vs UI ownership; stage-by-stage offline → qualified integration, explicit rollback and no Unity until separate GO. |
| **tasks.md** | Separate bounded tasks for source adapter, read-only visualiser, local reversible marks, annual/multi-year filter, history/save/Host only after relevant policy; actor/branch/physical lane and nonauthor review per dispatch. |
| **checklists/requirements.md** | Requirement-to-evidence matrix, trust and field schema checks, golden Year0/Year1 anchors, cross-lane/API completeness, negative security and stale-commit tests, explicit PENDING/NOT RUN status. |

The existing approved four-file spec under specs/multirack-sitka-p1-adult-rules-comparator-v1/ remains intact, with its unrun requirements and source gates. This #740 result is a **proposal for integrating the parallel drafts**, not a newly approved specification. No four-file source was written, no status changed and no implementation packet was issued by this worker.

## 12. Verification performed and final Lane E handoff

**Documentation verification only:** complete #730 dispatch read; live #740, #730 and main coordination checked; immutable #725/#729/#732/#735 GitHub reports inspected; owner-approved P1 packet and operative approval preface read; current main D-082/D-084/D-085 and separate Multi D-084 lineage contrasted; proposed interface posted to #740 for parallel readers. All numerical witnesses above were **taken from qualified/reviewed GitHub reports** rather than recomputed by E. No local source, physical Ops, Unity, dotnet, network Host, test, build, save, runtime, Windows, L4 or merge action performed.

**What passed:** draft completeness against #740's requested field schema, minimal five-sentence contract, ownership, three-domain source limits, Year0/1 witnesses, file/API boundary, Host alternatives, physical import/rollback gates, blockers, owner decisions and proposed four-file spec.

**What remains unverified:** actual #725 code imported to any browser/UI assembly; source/schema implementation; security L4; Year2 M-1; persistence; 2D player behaviour; Unity comparison; owner approval of this DRAFT; cross-lane reconciliation; integration and dashboard sync. Documentary completion cannot change these states.

**Handoff to Manager #730:** independently review this new specification proposal and reconcile A #736, B #737, C #738, D #739. Assign review verdict **APPROVED DOCUMENT QUALITY / NEEDS CHANGES / REJECTED** for this result, separately from any later product-spec approval. Preserve main D-084 as Scenario One, correct Multi canonical numbering through authorised owner decision, and only then consider a new independently reviewed four-file spec. No production work or security exception is requested here. **User action immediately required for #740 documentation handoff: none.**

**Dashboard/Drive sync:** NOT VERIFIED; GitHub readback and one-file diff check belong to the publication verification, not to dashboard ingestion. 
