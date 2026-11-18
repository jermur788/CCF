DRAFT — PROPOSED SPECIFICATION, NOT APPROVED, NOT IMPLEMENTED, NOT TESTED.

# Multi #730 / #789 — sequential adult Sitka annual engine

18 November 2026. Documentation author: OpenAI GPT-6.1 Sol, Codex in T3 Code, designated documentation worktree only. Feature identifier is the repository namespace `multirack-sitka-multiyear-annual-v1`, not a public API. Base/context supplied for this task: `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`.

Companions: [plan.md](plan.md), [tasks.md](tasks.md), [requirements.md](checklists/requirements.md). These four files form one proposal. They grant no source access, physical lease, test execution or implementation authority. No test has been run for this specification.

## 1. Authority and evidence language

Every substantive requirement or factual assertion inherits the evidence tag on its paragraph, table row or enclosing labelled section. “Must” defines proposed acceptance of a future implementation; it does not claim that implementation exists.

| Tag | Meaning |
| --- | --- |
| **[E]** | Directly evidenced by the published receipt cited here, including what its author/reviewer reports. This author has not inspected protected source or re-executed its results. |
| **[A]** | Accepted project/owner intent: the current task packet, D-1…D-4 or the applicable approved P1 baseline. This does not approve this new specification. |
| **[P]** | Model-derived technical proposal, requiring independent review and Manager/owner disposition. |
| **[U]** | Unavailable / NOT DERIVABLE from the permitted published evidence, not a guessed value or successful result. |

### 1.1 Source register

The links below are the actual task references. No protected source or fixture byte is included in this proposal.

| Ref | Published document and authority limit |
| --- | --- |
| S-01 | [#730/#789 orchestration packet](https://github.com/jermur788/CCF/blob/docs/multi-730-789-year2-10-no-intervention-orchestration-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-789-YEAR2-10-NO-INTERVENTION-SITKA-ENGINE-GATED-ORCHESTRATION-PACKET-20261118.md): [A] task authority and accepted D-1…D-4; source HELD. |
| S-02 | [#737 four-part proposal](https://github.com/jermur788/CCF/blob/docs/multi-737-b-spec-draft-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md): [E] predecessor DRAFT, not implementation authority. |
| S-03 | [#760 decision/errata closure](https://github.com/jermur788/CCF/blob/docs/multi-760-b-year2-decision-closure-result-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-760-LANE-B-YEAR2-ANNUAL-DECISION-CLOSURE-DRAFT-RESULT-20261118.md): [E] DRAFT corrections, including its published account of #748 F1/F3/F4. |
| S-04 | [#780 parent research result](https://github.com/jermur788/CCF/blob/docs/multi-780-deepseek-parent-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/780-orchestration/Multi-780-DEEPSEEK-YEAR2-10-INDEPENDENT-VALIDATION-PARENT-RESULT-20261118.md): [E] 13 synthetic vector families, 12 proposed criteria and 12 future native tests ALL NOT RUN; A2 comparison-only evidence and uncertainty register. |
| S-05 | [#730 research-quality verdict](https://github.com/jermur788/CCF/blob/docs/multi-730-780-research-quality-verdict-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-780-INDEPENDENT-YEAR2-10-RESEARCH-PACK-MANAGER-VERDICT-20261118.md): [E] APPROVED research quality only. Conservative defaults in that verdict were proposals; S-01 subsequently records the four accepted product choices. |
| S-06 | [#779 confinement result](https://github.com/jermur788/CCF/blob/docs/multi-730-779-deepseek-g1b-confinement-result-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-779-ENFORCED-AGENT-TOOL-CONFINEMENT-RESULT-20261118.md) and [Manager G1b verdict](https://github.com/jermur788/CCF/blob/docs/multi-730-779-manager-g1b-decision-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-779-G1B-MANAGER-VERDICT-AND-VIEWER-REENTRY-20261118.md): [E/A] remediation evidence, incomplete security acceptance, G2 source held. |
| S-07 | Owner-approved P1 four files on `docs/multi-p1-637-owner-approved-four-file-20261116-v1`: [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md), [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md), [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md), [requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md): [A] controlling upstream biological/technical baseline, read in full. Current approval headers take precedence over their retained historical DRAFT prose. Bounded P1 approval does not approve this successor. |
| S-08 | [#725 Year1 implementation receipt](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md): [E] local-only implementation/author results; not source publication. |
| S-09 | [#729 independent source/numeric review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md): [E] source review, independent arithmetic, M-1 mandatory before Year2; not a native rerun. |
| S-10 | [#732 focused native qualification receipt](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md): [E] initial network STOP preserved; later separately authorised offline focused PASS. Full suites/repeat-advance were not rerun there. |
| S-11 | [Main Decision Log D-082](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md): [A] standalone synthetic/non-Irish-forecast intent and conditional future load policy. Main D-084 is Scenario One Compact30 and supplies NO Multi authority. |

[A] Authority order is latest direct owner task → project decisions → applicable approved feature baseline. S-01 settles the four new product choices; S-07 §3 controls biological rules, §0/§4/§5 controls applicable trust, identity and numeric boundaries. S-02/S-03 remain historical DRAFT proposals. S-04/S-05 are research, not accepted calibration. Any material conflict with the lawfully inspected retained implementation must return to the Manager; do not silently rewrite either baseline.

### 1.2 Settled owner decisions

| ID | [A] Accepted product intent, 18 November 2026 | Consequence |
| --- | --- | --- |
| D-1 | Zero living adults is terminal. Another step refuses with an explicit typed reason and no mutation. Singleton growth continues with source last-survivor boundary protection; hazard remains source-governed. | Supersedes S-02 S4's proposed empty-year continuation and S-03 B-O01's recommendation. A step that kills the last adults may commit; a subsequent empty step cannot. |
| D-2 | Freeze the dead row at final post-growth DBH, height, crown and age, retaining source-supported cause/mechanism and ecological deathYear. | No later growth, ageing, repeat death or resurrection. Pre-inventory unknown cause/year stay unknown. |
| D-3 | Year0→1→…→10 is ten sequential one-year verification commits only. | No game lifespan cap, fast-forward feature, ten-year jump or Irish forecast. |
| D-4 | No interventions or persistence/Host integration in this slice. | No marks, thinning, felling, recruitment, planting, approved/executed Work Plans, save/load or gameplay integration. Future management-caused population changes require a separately approved executed Work Plan boundary; natural mortality remains this engine's only population loss. |

## 2. Scope and compatibility

### 2.1 Narrow feature boundary [A]

Own only the standalone 2D Multi Simulator's opt-in Year2–10 no-intervention annual continuation, preserving its Year1 one-step behaviour. The population is closed, synthetic, all living Mature-equivalent pure Sitka with Reference demand and source site factor fixed at 1. Living stems below the P0 action threshold remain biological participants. Preserve fixture geometry, positions, ages, dimensions, versions and source declarations; do not regenerate or reinterpret them.

This is not Scenario One, #770, #784 Year0 viewer, Unity, the abandoned custom viewer sandbox, a public app, a forestry forecast or full P1 completion. It grants no original-polygon annual capability, legal-square parity, game source integration, renderer/read ABI, persistence, save/load, cross-process resume, Host, network, external release or new package. No new coefficients, GROWFOR equations, youth/mixed species, disease, storm, canopy/light or soil-response system is added. Retain the upstream label: **CCF adult rules — closed pure-Sitka experimental population. Synthetic inputs. NOT an Irish field forecast.** Carry no-recruitment/no-storm and NOT Unity numerical/geometry parity disclosures into any future internal report.

### 2.2 Frozen Year1 witness [E/A: S-01, S-03, S-08–S-10]

| Protected contract | Required preservation; quoted, not rehashed here |
| --- | --- |
| Existing `annual` command | Fresh trusted genesis, exactly 0→1; stdout 45,980 bytes, SHA-256 `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`. Preserve arguments, output shape, ordering, formatting and refusal semantics. |
| Trusted fixture `p1d-30m-sitka-adult-v1` | 32,567 bytes, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, declared binary32 area bits `3db851ec`. No fixture/catalog/trust-value change. |
| Same one-step engine, second call | Still refuses `AnnualAlreadyAdvanced`. A new continuation mode does not reinterpret this shipped string. |
| Original P0 domain | Still refuses annual with `OriginalAnnualRefused`; original lab, fixture, trust/source pins and no-growth semantics remain protected. |
| Other admission | Unity-square domain unavailable; unknown/case-altered domains, unknown IDs, pending/unaccepted or tampered fixture and unsupported versions retain their existing trust refusals. |

[E: S-02/S-10] The historical old CLI argument sequence is `annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived`. This is a quoted compatibility invocation, not a command grant.

[P] Multi-year capability must use a separate explicit versioned opt-in path; retaining a trusted private state across individual in-process calls is proposed. The public command/API name is undecided (PR-01). A fresh old CLI invocation always creates another Year1, never the successor Year2. The stdout digest is an old report-byte witness, not state identity, a read-view digest, a ten-year golden or history ABI.

[E] S-08 reports author tests and second-call checks; S-09 independently reviewed arithmetic/source; S-10 independently reproduced the focused old stdout and original/tampered refusals offline. They evidence 0→1 only. None qualifies 1→2, ten years, integration, field forecasting or L4.

## 3. M-1 is the first implementation gate

[A/E: S-01, S-09 §7] Before enabling any 1→2, repair the latent dead-input bug and independently review the changed lines and numerical behaviour. Derive one ordinal living-at-start set before every calculation. Dead/nonliving entries must be excluded as CI **targets and neighbours**, and from growth, crown, age, all stand/density stages (Before/PostGrowth/AfterMortality), mean vulnerability, hazard candidates/rolls and boundary selection. Retain every identity in the immutable archival roster. Filtering calculations must never delete or renumber a row.

[E: S-04 §4; S-05] Required discriminator: the synthetic two-living-tree control with an added dead 50 cm near neighbour must retain target CI **0.3125** and requested DBH growth **0.9411764740943909 cm**. Incorrect dead inclusion gives CI **2.8125** and growth **0.6399999856948853 cm**. These are synthetic two-tree controls, **not** qualified 100-tree stand outputs or actual Year2 observations. Full construction is in S-03 §4 and S-04 V-01…V-03.

[P] T-02 must compare controlled A/B states with identical living IDs, authoritative bits, positions, area, seed, year and models; A alone retains the dead archive row. Compare each living tree's CI/count/growth/crown/age, every stand stage, pressure/mean and conditional rolls/outcome. Check dead target absence as well as dead-neighbour exclusion, no D roll/event and byte/field-exact dead-row retention. A/B roster sizes differ deliberately; living results must not. A separate controlled death-present transition must prove that a new victim remains archived and becomes inert in the following year's target and neighbour sets (T-03). The internal seam must not become a public untrusted-fixture admission path. G2 cannot pass on an all-living golden alone.

## 4. Private authoritative state and temporal meaning

### 4.1 Full state [P; PR-03]

Use one immutable committed state and an engine-local experiment identity, distinct even for two runs from the same fixture. Keep the full closed roster in stable case-sensitive ordinal tree-ID order. Identity is verbatim source identity; do not trim, case-fold, prefix, hash into a replacement ID or use array positions, viewport pixels or eligible-only indices.

Bind each successor to experiment identity, accepted domain/fixture/catalog/provenance, trusted fixture identity, geometry/origin/area bits, fixed seed, separate growth-model and RNG-model versions, annual algorithm/numeric-policy version, and declared fixed runtime. Store ecological year and committed generation/revision. Retain a private predecessor relation sufficient to validate a single successor; this is not a public lineage token, cryptographic chain, historical digest, codec or persistence format. A bare year/revision from another experiment never authorises a commit.

Each row retains ID, source position, species/stage/demand, authoritative f32 DBH/height/crown where known, chronological age, living status and existing death provenance. Live and dead rows belong to the same immutable roster; only living subsets feed biology. Pre-inventory unavailable crown/cause/year stays unavailable, never a generated biological measurement or event. Genesis is not required to admit a new dead fixture; controlled dead input belongs only to an approved private test seam.

### 4.2 Stage semantics [P, based on S-03 B-P01…P03 / #748 F1/F3]

| Stage | State year / target | Meaning |
| --- | --- | --- |
| Genesis | Initial fixture year; no executed step | Static admitted starting roster. Does not contain genuinely computed annual CI, hazard or roll. |
| Before(y) | Input state year y; step target y+1 | Full committed input roster plus current living set; any CI used by this step is computed here. |
| PostGrowth(y+1) | Target y+1, same transaction | Living-at-start trees after growth/age/crown, before mortality; not another annual commit. |
| AfterMortality(y+1) | Target y+1, same transaction | Final survivors/newly dead plus prior dead; sole committed successor. |

[P] Only a completed transaction may expose its staged diagnostics. No externally visible half-computed PostGrowth state. “Before(y+1)” in the next transaction consumes precisely AfterMortality(y+1), with fresh CI; it is not permanently Year0. A pre-growth CI displayed after commit keeps its calculation-stage label.

[P] Distinguish sourced, computed, deliberately not evaluated and unavailable data; a genuine computed zero is a value, a skipped roll is absence. At nonpositive pressure mortality locals may not evaluate even though growth/crown ran. At positive pressure p may evaluate to zero while the roll is skipped. Preserve numeric kind and reason; wrapper/type names remain private proposals PR-04. Do not adopt E #740 `ObservationKeyV1` or any public read ABI. Detached future read projections require their own owner/review; #784 remains independent of this engine.

### 4.3 Death freeze [A: D-2; P representation PR-03/04]

At a death commit, the tree has already grown and aged for that year. Freeze final post-growth DBH, stored height, crown and chronological age; retain source cause `self-thinning` and source-supported mechanism `hazard` or `boundary`, separately from ecological `deathYear=targetYear`. Those fields, status and ID are immutable thereafter. No repeat event, growth, age increment, roll, disappearance or resurrection. Existing earlier dead rows retain their earlier metadata; a pre-inventory dead row never acquires an inferred cause or death year.

[A] The initial-age-30 tree dying during its second one-year step retains **age 32 / deathYear 2**. This is a hypothetical controlled freeze case, not a claim about any actual Year2 tree. A later successful step with other living trees must leave that row unchanged. If no living trees remain, D-1 refuses the next step with the whole state unchanged.

## 5. Normative one-year transaction

### 5.1 Required ordered phases [A/P: S-07 §3/§5; S-01; architecture PR-03/05]

| Phase | Inputs and required action |
| --- | --- |
| A — Trust/admission | Validate lawful execution context, accepted domain/fixture, immutable bindings, model/algorithm/runtime policy, private experiment identity and expected current year/revision. Refuse unsupported/untrusted/stale state before biology. Require target=current year+1, not a jump. |
| B — Living partition | Freeze ordinal living-at-start IDs. If none, typed terminal refusal with NO year/revision/event/roster mutation. Otherwise capture Before(y) totals from living only; pass old dead rows through unchanged. |
| C — Pre-growth CI | Each living target sums living non-self neighbours in ordinal ID order, using pre-growth dimensions, horizontal source x/z and source float sites. Inclusive final 8 m check and 0.5 m distance floor; cache CI for this step only. |
| D — Growth/age/crown | Each living-at-start adult grows and ages once under pinned source rules/clamps; crown relaxes using post-growth DBH and cached pre-growth CI. Prior dead do nothing. Stage chronology locally until publish. |
| E — Post-growth density | Compute PostGrowth(y+1) N/BA/Dq/RD from all living-at-start grown rows and declared f32 area; derive pressure from that RD. Preserve pre-growth CI for mortality suppression. |
| F — Conditional hazard | Fixed post-growth candidate set, common pressure and mean vulnerability from living only. Evaluate source hazard and conditional roll; do not recompute pressure/mean after each selected hazard victim. Death iff roll<p. Gather hazard victims once in source ordinal order. |
| G — Survivors-only boundary | Start from hazard survivors. Strict RD>1; decreasing pre-growth suppression then ordinal-ID ties. Source running-density updates after removals, stop at RD≤1 or count≤1. Hazard and boundary victims are disjoint. The boundary cannot force removal of its last survivor; hazard is not waived for singleton. |
| H — Single atomic publish | Build full successor roster, one year's unique deaths and stage diagnostics privately; validate finite/state/accounting invariants and expected identity/revision under commit exclusion. Publish ONE immutable state/result bundle at the single commit point. Nothing staged escapes on failure. |

[P] Never regrow within the same year after mortality. Next year's Before comes from the prior committed full-precision state; recompute CI, pressure and vulnerability instead of replaying cached diagnostics or rounding from JSON. Mortality changes competition in the next step only. No intermediate Work Plan boundary is introduced.

### 5.2 Controlling formulas and known evaluation sites [A/E]

The following restates S-07 spec §3, §5.1 and §9.5 with S-09 §3's published source mapping. It does not re-derive biology. The lawfully admitted linked source functions and their confirmed evaluation/storage sites govern. Algebraic equivalence alone is insufficient. PR-06/ND-04 blocks any unresolved rounding-site choice; no author may select a convenient precision or global tolerance.

| Operation | Accepted rule and published numeric boundary |
| --- | --- |
| Hegyi | `CI = sum((D_neighbour/max(1,D_target))/max(0.5,distance_m))`; source f32 dx/dz, distance stored as f32 after source sqrt, f32 terms and ordinal accumulation. Final distance>8 skips; equality included. Candidate-query slack cannot alter final included pairs. |
| DBH | `gain = 1.2*site*clamp01(1-D/120)/(1+CI/5)` is the mathematical rule, site=1. Source f32 operands/function result and `clamp(D+gain,1,200)` storage. S-09 describes multiplication by the reciprocal. S-04 flags DIV versus reciprocal diagnostic last-bit ambiguity; use the actual pinned linked function, never substitute an algebraic rewrite. |
| Height | Source Class III `E(age)`, anchored at age30 height 20.4 m; relative `E(a+1)/E(a)` under source age handling. Preserve `NextHeight(H,age,1)-H`, then stored `clamp(H+deltaH,0.1,60)`; do not shortcut to ratio-only storage. Full function/native future columns are ND-02. |
| Crown | Source potential from post-growth D: `max(0.1,0.9415+0.07635*D_post)*response(preCI)`; relax using source f32 `R+(target-R)*0.15f`, floor 0.1 m. Reuse pinned response function, do not assume response=1; ND-03. |
| Stand | N counts living only. `Nha=N/areaHa` evaluates int/f32→f32 then is widened where stored f64. f64 ordinal squared-DBH reduction; `Dq=sqrt(mean(D^2))`; `BA=sum(D^2)*pi/40000`, per-ha BA uses the declared f32 area widened without decimal replacement. Confirm exact product/call sites at G1; ND-04. |
| Relative density | `RD=Nha*(Dq/25)^2.063/1868` using source f64 stand path/Math.Pow. British max-density transfer, not an Irish empirical limit. |
| Pressure | `P=max(0,(RD_post-0.6)/0.4)`. Nonpositive pressure skips source mortality work, not growth/crown. |
| Suppression/vulnerability | `S=1-1/(1+CI/5)`, source CI/5 f32 before f64 expression (S-09); vulnerability source S^4, f64 ordinal mean over current living candidates. Do not substitute f32 display suppression or recompute CI after growth. |
| Hazard | When pressure/mean permit, `p=min(0.5,0.08*P^2*S^4/mean(S^4))`. Preserve source zero-denominator/early-return handling; p=0 means no roll. Native pow/accumulation sites must be confirmed, not guessed. |
| RNG | Pinned `SimulationRandom.Roll`, independent RNG version, fixed seed, target ecological year and literal `ADULT-MORT-` + full verbatim ID. Integer-derived 24-bit fraction; actual packing/hash/numerators/victims for future years remain ND-01/05. No stateful RNG cursor or arbitrary resampling. |
| Boundary | Strict `RD>1`, hazard survivors only, source decreasing S then ordinal ID, running reductions in actual removal order. Do not replace source running sums with differently rounded rescans. Final count≤1 guard retained. |

[A/P] f32 tree DBH/height/crown/CI/coordinates/area and f32 Nha division are distinct from f64 stand reductions, mortality S/mean/p and density. Preserve ordinal accumulation, clamps and finite values, signed-zero storage where applicable, exact source function linkage and evaluation order. Report requested gain separately from applied stored difference. No guessed round-trip codec, changed area or display-rounded feedback. A numeric-site map is a G1 exit deliverable and independently reviewed at G2; a mismatch stops for bounded spec reconciliation before code.

### 5.3 Terminal, singleton and accounting [A/P]

D-1 refuses an advance from zero living without evaluating undefined Dq/mean or inventing zero-tree chronology. A legitimate step whose hazard kills all living may commit a terminal AfterMortality with source-defined finite empty metrics and explicit absence for undefined diagnostics. Its exact private absence representation is PR-04; no NaN/Infinity, fake tree or fabricated next year. Terminal post-state creation and terminal next-step refusal are separate checks.

A singleton has an empty neighbour set and genuinely computed CI zero; normal source growth/age/crown continues. Preserve source conditional hazard logic and the boundary's last-survivor guard. A high-RD singleton is a labelled source-function/tiny-area probe, not a claimed legal Unity-square runtime witness. Never force universal RD≤1 or waive hazard as a new immortality policy.

[P] Per successful year: `N_after=N_before-naturalDeaths`. Death IDs partition into disjoint hazard/boundary sets, each original ID can die once across all commits. All-living BA reconciles as Before BA + growth BA on living-at-start − post-growth BA of actual victims. Validate source arithmetic/reduction effects under reviewed PR-06, not a new arbitrary tolerance. No felling count, timber award, work revenue or recruitment ledger. Archive roster cardinality/IDs/positions remain fixed.

## 6. Determinism, concurrency and refusal [P unless [A/E] stated]

Use the same fixed seed throughout. Target year and complete ordinal ID, with the pinned RNG-model version, determine a conditional roll. Ordering cannot depend on dictionary iteration, scheduling, roster omission of dead entries or UI activity. Permuting input must be normalised to ordinal order. Reject duplicate IDs instead of merging them; equal-looking experiments still have different private identity. PR-03/05 governs representation, not biological choices.

Propose an engine-private one-step request carrying expected experiment identity/current ecological year/current generation. Exact type/method names are PR-01/03/05. Compare under one commit gate. Two synchronised requests against the same predecessor must produce exactly one publish and one typed stale/duplicate refusal; the loser must not advance the newly published successor. A pre-commit exception, invalid numeric result, invariant failure or cancellation before the commit point leaves the identical prior state reference, year, revision, roster and death evidence. Retry against that unchanged predecessor must match a clean calculation. An old request after success refuses; returning a cached public command result or cross-process deduplication is not specified.

Stage a state/result bundle and validate before one nonthrowing publication point. Prepare reporting before commit; do not throw a pre-commit-style failure after state was published. This is in-memory atomicity only. Process termination loses the experiment; no durable rollback, persisted event chain, resume or history-hash equality is claimed. An audit of a refused call must not mutate authoritative state or consume a generation.

### 6.1 Refusal classes (illustrative private names only; PR-02)

Every proposed name below is **PROPOSED — requires Manager/owner decision**. The semantic classes are proposed typed results, not approved error-code literals or replacements for shipped errors.

| Semantic class | Proposed private name | Required result |
| --- | --- | --- |
| Trust/domain unavailable | `TrustOrDomainRefused` | Preserve existing specific trust/domain outcomes; no biology. |
| Unknown model/algorithm/runtime binding | `UnsupportedBinding` | Fail closed before calculation; no fallback or implicit migration. |
| Wrong expected year or jump | `ExpectedYearMismatch` | No chronology or state mutation. |
| Wrong experiment/generation, duplicate | `StaleOrDuplicateStep` | No mutation by the refused request; concurrent winner remains intact. |
| Zero living [A: D-1] | `NoLivingAdultsTerminal` | Explicit typed terminal reason; no empty-year commit. |
| Invalid state/finite/accounting invariant | `InvalidAnnualState` | No partial publish. |
| Calculation/cancellation/pre-commit fault | `AnnualStepNotCommitted` | Prior state intact; no fabricated committed result. |

[E/A] Existing `AnnualAlreadyAdvanced`, `OriginalAnnualRefused` and specific legacy admission strings keep their original meanings and precedence. This table grants no new public vocabulary. Proposed admission order is trust/bindings → expected identity/year/revision → living/terminal → calculation/invariants → compare-and-commit; multiple-fault precedence for new types remains PR-02, tested only after disposition. T-10 must never weaken old trust-first refusal to make a new class pass.

## 7. Trust domain and security gates

[E/A: S-06; S-08–S-10] **Default .NET L4 FAIL38 UNWAIVED. G2 SOURCE HELD.** Local non-public, offline-only, fixed-runtime experiment is the boundary, not a security waiver. #732's original build contacted NuGet unexpectedly and stopped; its separately permitted offline resumption is not a reusable grant. No network, Host or persistence claim is made. No public source/app upload, source import, main merge, Unity or Windows work follows from this document.

[E: S-06] Whole-process confinement plus harness restrictions improved admission evidence but the final security acceptance was not granted. Residuals remain: R1 provider-token readability, R2 destructive worktree tool reaching implementation despite deny, R3 symlink resolution bypassing directory checks on mounted targets, R4 OS network/abstract-socket reach. Tool absence alone is not OS offline proof. Any launcher/policy change requires independent requalification. A viewer-only risk decision, if later made, cannot authorise this separate .NET engine. This spec selects no workaround, weakens no analyzer and accepts no residual risk.

[A] Before Stage B of S-01, all these gates must be satisfied by the competent Manager/custodians, not by this proposal:

- Explicit independent approval of the complete four-file specification and applicable owner/Manager decisions.
- Named source custodian's exact grant for retained source and any separately scoped trusted fixture access; retained custody or publication of a receipt grants no access.
- Genuine physical single-writer lease, current board/ownership checks, clean disjoint candidate and exact file/command scope; never take over a retained lane or assume expiry.
- Independently qualified whole-process confined offline CLI compatible with the actual .NET commands, with proven no-network execution and named actor-specific Manager security/source grant. A T3-delegated full-access child cannot become a protected-source writer.
- Separate actor-specific nonauthor review and offline qualification permission, protected P0/fixture integrity controls and frozen candidate.

[A] If any gate is absent, STOP before protected source/fixture read, copy, build or test. Report the exact missing prerequisite. This specification cannot grant any of them. Plan G1 is the admission phase; the Manager security “G2” in S-06 is a different vocabulary, not automatically passed by plan G2 (M-1). Dashboard/Drive synchronization is not verified here.

## 8. Decision register

### 8.1 Settled versus proposed [A/P]

D-1…D-4 in §1.2 are settled product intent. They do not approve API names, codecs, numeric choices, security exceptions or this spec. Each item below remains **PROPOSED — requires Manager/owner decision**; its recommendation supplies no permission. Private interface proposals may be approved technically within owner scope; new public/product/security scope must go to the owner as appropriate.

| Stable ID | Status | Proposal / deliberately unchosen item | Decision owner and blocking point |
| --- | --- | --- | --- |
| PR-01 | PROPOSED — requires Manager/owner decision | Separate explicit versioned opt-in one-year path; private callable seam suggested. Public runner/API/command name and public workflow unchosen. | Manager/spec reviewer; owner for public workflow. Before exposing any new command; private-only implementation needs approved spec scope. |
| PR-02 | PROPOSED — requires Manager/owner decision | Typed refusal classes in §6.1; exact names, literals and new multi-fault precedence unchosen. Preserve shipped strings. | Manager/owner before externally observable error contract; technical reviewer before private tests depend on names. |
| PR-03 | PROPOSED — requires Manager/owner decision | Private engine-local experiment/predecessor identity, full roster, year and generation; no cryptographic ABI. Exact representation unchosen. Public lineage, historical digest, codec and canonical hash deferred. | Manager/technical reviewer before state implementation; public/history work requires separate owner scope. |
| PR-04 | PROPOSED — requires Manager/owner decision | Private immutable stage diagnostics and typed absence; no E #740 read ABI, eventTip or public numeric codec. Empty-final metrics/source absences confirmed after admission. | Manager/numeric reviewer before result representation. Any shared adapter separately owned and excluded here. |
| PR-05 | PROPOSED — requires Manager/owner decision | Expected identity/year/revision compare-and-commit, single state/result publication and controlled test seams; exact private method/locking/fault seam unchosen. | Manager/technical reviewer at G0, confirmed source map at G1 before code. No persistence/idempotency service implied. |
| PR-06 | PROPOSED — requires Manager/owner decision | Fixed-runtime numeric-site/equivalence policy, source-derived internal vectors, explicit reconciliation arithmetic. No arbitrary tolerance, unsupported diagnostic gain bits or new “golden” adopted. | Independent numeric reviewer + Manager after lawful source mapping, before G2 tests/changes. Return discrepancies before implementation. |
| PR-07 | PROPOSED — requires Manager/owner decision | No cross-process resume/persistence/history codec in this slice. Any later contract, portability, caps or save semantics needs a separate spec. | Owner/Manager if future scope is commissioned; does not block this in-memory slice. |
| PR-08 | PROPOSED — requires Manager/owner decision | No new calibration/biological Year2–10 goldens; retain accepted source constants and fixture. A2/GROWFOR comparison-only evidence may inform a separately scoped study, never tune this engine to match it. | Owner/science reviewer/Manager for any future calibration or empirical claim; no new coefficients selected now. |

[A/U] Security/source admission is an external held gate (§7), not a proposed permission or a granted allowlist. No owner product choice settles it. PR-07/08 record exclusions/deferred decisions, not invitations to implement them.

### 8.2 NOT DERIVABLE register [U: S-04 §4.3/§5; S-09/S-10 limits]

| ID | Insufficient published evidence | Required honest handling / future resolution |
| --- | --- | --- |
| ND-01 | Complete RNG key packing/hash and future per-ID/year 24-bit numerators. | Specify key inputs, not guessed outputs; lawful exact-source mapping plus independent future oracle needed. |
| ND-02 | Full Class III function/evaluation sites and native later-year height columns. | Refer to pinned accepted function; no reconstructed curve or future height golden. |
| ND-03 | Full crown-response implementation and native later-year crown columns. | No response=1 assumption; source confirmation and independent future column checks. |
| ND-04 | Every native DBH/product/pow/BA evaluation site, DIV/reciprocal diagnostic last bits and JIT/libm behaviour. | Preserve known f32/f64 sites; stop at unresolved ones; PR-06/source numeric map before execution expectations. |
| ND-05 | Exact hazard victim sets for arbitrary IDs/years with p>0. | Controlled injected comparator/victim seams must be labelled; no claimed native victims until independently derived and executed. |
| ND-06 | Full native Year2–10 tree/stand outputs, H/crown trajectory, annual report bytes and numerical/biological goldens. | Ten-step synthetic arithmetic projection is not a native oracle; report property checks and future actual evidence, never invent numbers. |
| ND-07 | State lineage/history canonicalisation, digest/codec/public ABI, resume and saved history equality. | No hash-as-identity contract, public ObservationKeyV1, codec or cross-process equivalence claim; PR-01/03/04/07. |
| ND-08 | Irish field predictive validity; measured adult mortality/rate/size bias, realised crowns, within-stand variation, later stand trajectories, yield/rotation and top-height definition equivalence; growth-model differences beyond the constrained route. | Unsupported, comparison-only. A2's secondary-source limits and unresolved population denominator remain; no fitted mortality/crown/field threshold. PR-08. |
| ND-09 | Unity/Mono/Windows/cross-platform bit parity, durable crash rollback, full security acceptance or L4 remediation; current native custody/confinement status. | Unavailable/unverified, no new claim. Separate source/physical/security or parity authority required; this author did not inspect native evidence paths. |

[E] #780's 13 families and 12 future tests remain ALL NOT RUN as engine tests; Q's arithmetic review is research-quality evidence. A2's forestry envelope is comparison-only, primary PDFs were not re-read by A2, and GROWFOR stand evidence is not tree-level hazard/crown truth. British density transfer and mortality/crown constants are deliberate gameplay calibration, not Irish laws. No additional general research cycle is proposed.

## 9. Traceability to predecessor proposals

[P] This crosswalk accounts for S-02's clauses, not a rewrite of that historical file. “D-none” means D-1…D-4 do not settle the technical detail; applicable upstream authority or PR decision is shown. Requirement/test IDs resolve in the companion checklist. Plan gate names here are local to this four-file proposal.

### 9.1 #737 Part I clauses

| #737 clause | Destination / tests | Owner decision / disposition |
| --- | --- | --- |
| Opening evidence table, canonical naming conflict, frozen witness and security boundary | §1.1, §2.2, §7–8; T-01/10/17/18 | D-none; prior evidence remains scoped; main D-084 excluded, D-082 disclosure preserved. |
| Goal / feature status / scope class | §1–2; T-00, T-17 | D-3 test-only; D-4 narrow slice; new spec still DRAFT. |
| S1.1 exact Year1 / separate opt-in | §2.2; T-01 | D-none; upstream frozen; PR-01 public path open. |
| S1.2 P0/no source / unavailable domains | §2.2, §7; T-10, T-18 | D-none; upstream trust; security gates held. |
| S1.3 exclusions / future work | §2.1, §7; T-17 | D-4 excludes all interventions; future executed boundary separately approved. |
| S1.4 living population/site/versions | §2.1, §3, §5.2; T-02, T-14 | D-4 closed; upstream P1 biology. |
| S1.5 runtime/forecast claim | §2.1, §5.2, §8.2; T-17 | D-3, PR-06/08; no parity/forecast. |
| S2 experiment-binding bullet | §4.1, §6; T-09, T-10 | D-none; PR-03/06. |
| S2 chronology bullet | §4.1–2, §5.1; T-07, T-09 | D-3 one-year commits; PR-03. |
| S2 stable-roster bullet | §3, §4.1/3; T-02, T-05, T-09 | D-2 freeze; PR-03 representation. |
| S2 ledger/history bullet | §4.2, §6, §8; T-07, T-16 | D-4 no persistence; PR-03/04/07, no durable ledger. |
| S2 numbers/provenance bullet | §4.2, §5.2; T-11, T-16 | D-none; upstream sites, PR-04/06. |
| S2 successor-identity paragraph | §4.1, §6, §8; T-09 | D-none; replace digest tuple with private identity proposal PR-03; codec deferred. |
| S2 dead-stem policy | §4.3; T-05 | D-2 settles freeze/age, not public format. |
| S3 M-1 targets/neighbours/growth/stand/mortality and controlled vector | §3, §5.1; T-02, T-03 | D-2 inert archive; upstream M-1 mandatory. |
| S4 phase A trust | §5.1 A; T-10 | D-none; upstream trust/PR-05. |
| S4 phase B target-year selection | §5.1 A/D; T-07, T-17 | D-3 sequential; D-4 no work. |
| S4 phase C living partition | §5.1 B; T-02, T-06 | D-1 terminal, D-2 archive. |
| S4 phase D competition | §5.1 C, §5.2; T-02, T-11/12 | D-none; upstream P1. |
| S4 phase E growth/age/crown | §5.1 D, §4.3; T-05, T-11/14 | D-2 freeze after death. |
| S4 phase F density/pressure | §5.1 E, §5.2; T-11/13 | D-none; upstream P1. |
| S4 phase G hazard | §5.1 F, §5.2; T-04, T-09 | D-1 no blanket singleton hazard waiver; source governs. |
| S4 phase H boundary | §5.1 G, §5.3; T-13, T-15 | D-1 singleton guard; upstream strict RD>1. |
| S4 phase I finalise | §5.1 H, §6; T-08, T-16 | D-2 death record; PR-05 single publish. |
| S4 same-source formulas | §5.2, §8.2; T-11/14 | D-none; accepted P1 functions, PR-06, unresolved sites ND-01…04. |
| S4 empty/singleton paragraph | §1.2, §5.3; T-06, T-15 | **D-1 SUPERSEDES empty-year continuation**; singleton source rule retained. |
| S4 inter-year link | §5.1, §6; T-03, T-07 | D-2/3; full-precision continuation proposal. |
| S5 seed/keying bullet | §5.2, §6; T-04, T-09 | D-none; upstream RNG identity; ND-01/05. |
| S5 one-write authority/concurrent retry bullet | §6; T-08 | D-none; PR-05. |
| S5 conceptual failures bullet | §6.1, §8; T-06, T-10 | D-1 terminal class; PR-02 names remain open. |
| S5 atomicity/cancellation/crash bullet | §6; T-08 | D-4 no durable guarantee; PR-05/07. |
| S5 one-year vs batch bullet | §1.2, §5.1; T-07 | D-3 ten commits, no new batch API. |
| S6 AnnualStepBefore | §4.2, §5.1; T-16 | D-none; PR-04 private stages. |
| S6 AnnualGrowthDiagnostics | §4.2, §5.2; T-11/14/16 | D-2; PR-04/06; no invented columns. |
| S6 AnnualMortalityDiagnostics | §4.3, §5.2; T-04/05/16 | D-2 cause/year; PR-04. |
| S6 AnnualStepAfter | §4.1–3, §5.1 H; T-05/16 | D-2 successor; PR-03/04. |
| S6 NoInterventionHorizonReport | §2.1, §5.1, §8.2; T-07/17 | D-3/4; report representation PR-04. |
| S6 shared boundary paragraph | §4.2, §7–8; T-16/17 | D-4 no integration; PR-04 separate E contract, no ABI adoption. |

### 9.2 #737 Parts II–IV and open decisions

| #737 clause / checklist / test | Destination / current tests | Settlement or remaining decision |
| --- | --- | --- |
| Part II G0/G1/G2/G3/G4/G5 | plan.md G0/G1/G2/G3/G4/G5 respectively | D-1…D-4 replace old product gaps; security admission still held. |
| Part II source partition / E/D/C/A roles | tasks.md §1, §3; §7 here | D-4 no integration; no prospective source path is a grant. |
| Part II verification tiers and stop conditions | plan.md §2–4; T-00/18 | D-none; bounded gates, no repeat broad suites without risk. |
| Part III B-01/B-02 | tasks.md B-01/B-02/B-03 | M-1 independently reviewed before any 1→2. |
| Part III B-03/B-04 | tasks.md B-04/B-05 | D-1…D-4; private state and sequential engine, PR-01…06. |
| Part III packet policy / no production allowlist | tasks.md §1/§3; plan G1; T-18 | D-none; future actor-specific exact grants required. |
| Part III B-05/B-06 | tasks.md B-06/B-07 | Independent review then separately granted offline qualification. |
| Part III B-07 E integration | §4.2/§8 PR-04; tasks.md B-08 readiness only | D-4 excludes integration; E decision deferred. |
| B-R01 / B-R02 / B-R03 / B-R04 | T-02/03; T-16; T-14; T-11/12 respectively | D-2/4 + upstream; PR-06 numeric gaps. |
| B-R05 / B-R06 / B-R07 / B-R08 | T-05; T-06/15; T-16; T-03/07 respectively | D-1/2/3; zero progression replaced by refusal. |
| B-D01 / B-D02 / B-D03 | T-01; T-07/09; T-07 respectively | D-3; same in-process steps only, no canonical-history digest promise. |
| B-D04 / B-D05 / B-D06 / B-D07 | T-08; T-08; T-10; T-05/09/16 respectively | D-2/3 + PR-02…06. |
| T01 / T02 / T03 / T04 | T-01 / T-02 / T-03 / T-04 | Frozen witness / M-1 / continuity / hazard. |
| T05 / T06 / T07 / T08 | T-13/15 / T-12 / T-06/15 / T-07 | D-1 terminal and D-3 horizon; no empty-year output. |
| T09 / T10 / T11 / T12 | T-08 / T-10 / T-16 / T-18 | T11 limited to private isolation, E integration deferred; grants remain separate. |
| OPEN-B1 / OPEN-B2 / OPEN-B3 | §4.2 PR-04 / §5.3 / §4.3 | B1 remains public-adapter open; B2 settled D-1; B3 freeze settled D-2, format remains PR-03/04. |
| OPEN-B4 / OPEN-B5 / OPEN-B6 | §8 PR-03/07 / PR-01/02 / §7 | Public history/command unresolved; lawful custody gate not granted. |
| OPEN-B7 / OPEN-B8 / OPEN-B9 / OPEN-B10 | §5.2 PR-06 / §7 / §2.1 / plan G0 | D-3 fixed-runtime test only; L4 held; D-4 no work; explicit independent spec/product approval still required. |
| Evidence/final-status classifications | §1/§8.2; T-17 | D-1…D-4 now intent accepted; all tests here NOT RUN. |

### 9.3 #760 complete decision/errata crosswalk

| #760 ID | Destination / tests | Owner settlement or open technical choice |
| --- | --- | --- |
| B-A01 | §2.2; T-01 | Upstream Year1 frozen; D-none; public opt-in naming PR-01. |
| B-A02 | §2.1/§3/§4.3; T-02/03 | D-2 freeze; upstream living-only mandatory M-1 before 1→2. |
| B-A03 | §5–6; T-03/04/11/16 | Upstream causality; D-2/3; source sites PR-06. |
| B-P01 F1 | §4.2/§5.1; T-16 | D-3 sequential year, private stage proposal PR-04, no read ABI. |
| B-P02 F3 | §4.1/§6; T-08/09 | D-none; PR-03/05 private identity, public lineage open. |
| B-P03 F1 absence | §4.2/§5.2/§8.2; T-04/11/16 | D-1 forbids fake empty computation; PR-04 typed absence; native columns unavailable. |
| B-P04 F4 | §4.3; T-05 | D-2 settles freeze/age32 versus deathYear2, not actual Year2 death. |
| B-P05 | §7; plan G0–G4; T-00/18 | D-none; exact new admission/review/security grants still external. |
| B-O01 | §1.2/§5.3; T-06/15 | D-1 settles terminal refusal, supersedes empty-year recommendation. |
| B-O02 | §4.3; T-05 | D-2 settles dead freeze; representation PR-03/04 remains proposed. |
| B-O03 | §1.2/§5.1; T-07 | D-3 settles test-only horizon, no game cap or jump. |
| B-O04 | §2.1/§5.1; T-17 | D-4 settles no interventions; future executed-plan contract remains separate. |
| B-O05 | §2.2/§6.1/§8; T-01/08 | D-none; PR-01/02/05; no public name/code or resume approval. |
| B-O06 | §4.1–2/§6/§8; T-09/16 | D-3 actual commits, D-4 no persistence; PR-03/04/07 history/public identity open. |

[P] S-03 §3's individual S1/S2/S3/S4/S5/S6 replacements are implemented as proposal text in the matching §2/§4/§3/§5/§6/§4.2 chapters above; its zero/singleton replacement is now settled by D-1. S-03 B760-T01…T12 map respectively to T-01, T-02, T-03, T-16, T-05, T-09/04, T-08, T-04/13/15, T-06/15, T-07, T-10/18, T-17. No prior document, test, fixture or public contract is changed by this crosswalk.

## 10. Approval and handoff boundary [A/P]

Deliver this four-file DRAFT to the separately appointed nonauthor scientific/technical reviewer and Manager. G0 requires explicit review and applicable owner/spec approval of the exact candidate, including bounded corrections and focused recheck. Product D-1…D-4 alone is not four-file/code approval. No source access/test/lease is implied. Implementation, independent review, focused native qualification, integration, verification and owner acceptance remain distinct statuses. This author performs documentation inspection only and claims no independent review or engine result. Manager readiness at G5 is not release or automatic main integration.
