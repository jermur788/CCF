# Multi #885 Q4 independent Q0 review

Reviewer: OpenAI **gpt-6.1-sol**, Codex harness in T3 Code, independent nonauthor. Runtime identifies the model; no T3 thread/session UUID is exposed. No delegates. Date: 2026-11-21. Claude Opus 5.5 authored the reviewed report; I did not.

## Identity and scope

Frozen branch `docs/multi-885-q0-equation-crosswalk-20261121-v1`, head `4be3b2f3cddcbf9c10a60e17769d3d6e1be1846d`, blob `bd125f54fdcd0d8c0ee2f3a481b59728e0570455`.
Path: `Docs/Handoffs/MultiRack-Simulation-20261116/885-q0-qualification-20261121/Q0-EQUATION-AND-RESEARCH-CROSSWALK.md`.
Independently recomputed SHA-256: `b38a52722415061584c80b86d40e7bdad86813c87f1e32e0959ad4b293258bb1`; 377 lines. All identities MATCH.

GitHub APIs confirm branch head, sole parent `3a67d0987be0cf40d007892739d2ad295c84f0e0`, and exactly one added file with that blob. Retrieved bytes at immutable head. Local report lookup failed because that artifact is not present locally; I used GitHub instead, without fetching or mutating the clone.

Code evidence is exclusively `git -C /home/jer/Development/CCF-HP show 4cecf873:<path>` at candidate `4cecf87339bea4d32907955a8d5625d1f27f96bd`. Local HEAD observed `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`; status empty at intake and after inspection. This proves current tracked/untracked cleanliness, not historical author cleanliness or ignored-output cleanliness. The report makes no explicit historical clean-status assertion.

Read AgentWorkflow and project context; fetched authoritative live main coordination. Governing packet 01 and research packet 02 were read at `7a2a13fa8ab850571fb40868fb2ae7f188922b9e` for scope. Direct user grant limits writes to this new review file. No worktree/source/report edits, Unity, builds/tests, simulation, server/listener, PR, issue comment, board edits or merge. Scratch is outside Git. Publication is documentation only.

Abbreviations at candidate: LAB=`Tools/MultiRackSitkaP1Lab/`; CCF=`Assets/ForestPrototype/`; SGM=`Docs/Research/SitkaGrowthMortality/`; Composite=`Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`. MultiArea filenames are under LAB/src/Core/MultiArea; other Core names under LAB/src/Core.

## A–G gates

| Check | Result | Evidence/limit |
|---|---|---|
| A Integrity | PASS for artifact/current status; historical status NOT VERIFIED | Head/blob/hash/377 lines/sole parent/one-file diff match; present clone clean. |
| B Anchors | FAIL | 40 checks listed below; several anchored interpretations are false, plus wrong handoff section citation. |
| C Units/arithmetic | FAIL | Conversions/area/N/Dq/BA/RD correct; CI and annual probability units wrong; numeric exposition incomplete. |
| D X1–X15 | FAIL in part | All rows checked below. X3/X4 overstate implications; sources otherwise support qualified verdicts. Three universal claims correctly refused. |
| E Stage honesty | PASS for report text; historical activity NOT VERIFIED | Explicit no-run/no-measurement/no-calibration/no-acceptance/no-comparability; uploads UNKNOWN; quoted numbers have repository provenance. No invented engine output identified. |
| F Protocol | FAIL | Full-view repeat equality impossible; staged equivalence and adaptive selection/logging underdefined. Contracts/seeds/shared plan/chronology largely correct. |
| G Weaknesses | FAIL | Not all are falsifiable hypotheses; W2 overstates; W3/W4/W7 tests do not isolate claims; ranking lacks consequence rationale. |

## Findings

**0 BLOCKING / 5 MAJOR / 3 MINOR.** MAJOR findings prevent approval as an executable qualification specification. No correction requires an engine-rule change.

| ID | Severity | Exact location/evidence | Defect | Minimal correction |
|---|---|---|---|---|
| F1 | MAJOR | Report §2.9 byte-identical views, §6 repeats; MultiAreaContinuation.cs:55-57,191-194,532-534; MultiAreaContracts.cs:88-90 | Fresh sessions have random SessionId exposed in views. PlanId/execution keys embed it. Full-view equality cannot pass, even at genesis. MA-E03 compares Annual and View.Areas instead. | Specify deterministic comparison projection: annual reports and areas directly; validate raw IDs within session and consistently normalise session-derived plan/key identities for ledger comparison. Specify serialization/order/float representation. Keep random session generation. |
| F2 | MAJOR | §2.3 E3 dimensionless CI, E4, E13 /yr; AnnualRules.cs:67-82,88; SitkaGrowthModel.cs:103-114 | DBH ratio divided by distance in metres is m^-1, not dimensionless. CI50 must share that convention, or explicitly use a reference metre. p is a dimensionless Bernoulli probability for one annual step, not instantaneous hazard per year. | Document metre-based CI and CI50 convention and annual probability semantics. Preserve code; forbid unqualified length/time-unit conversion. |
| F3 | MAJOR | X4/W2 “DBH growth does not slow as stand fills”, X3 “only competition-responsive variable”; AnnualRules.cs:89-98 | Hegyi ratio invariance does not remove explicit clamp(1-D/120) growth taper. Missing direct RD feedback is the narrower true statement. Crown also responds to CI. W2 assumes a pre-death window though MA-A begins above onset (MultiAreaTests:77-78). | Narrow X4/W2 to missing direct occupancy feedback, retain taper, describe DBH as principal responsive growth variable alongside crown. Test fixed-roster CI scaling separately from diameter taper; no assumed pre-mortality interval. |
| F4 | MAJOR | §7 W1–W12, especially W3/W4/W5/W7/W11; AnnualRules.cs:18-40,89-99; MultiAreaContracts.cs:56-67 | Not every item is a falsifiable hypothesis. Boundary dominance cannot prove empirical nontransfer, or hazard dominance validate transfer. W4 has no significance rule/control for age/DBH/spacing/dependence. W5 exact ratios drift under f32 rounding/clamps. W7 suitable matched pairs need not exist. W11 “irreproducible unless logs” ignores deterministic replay with full source/definition/timed commands; years×trees rows also fails for null empty-area reports. | Separate established limits from hypotheses, specify actual rejection criteria/tolerances/confounders. Label canonical-data limitations and model diagnostics honestly. State retention risk, not absolute irreproducibility; log replay transcript and reports with empty-area handling. Explain consequence ranking. |
| F5 | MAJOR | §6 Design S, intensity, measurements/comparisons; MultiAreaContinuation.cs:268-275,291-305,364-378 | “Equivalent” single intervention undefined (stems/BA/timing/identities). Adaptive size lacks outcome-independent stopping rule. Stand mean DBH confounds selection with growth. Null S/p/u and empty reports lack logging semantics; all-year capture versus checkpoints is not explicit. | Freeze qualitative rule, tie-breaks and allowable-set stopping criteria before outcomes, without fixed percentage; disclose chosen IDs/intensity before each execution. Define equivalence metric/mismatches; use matched retained-tree increments and selection-only mean shifts separately. Capture every year's reports/commands, preserve null as unevaluated, log empty carry-forward. |
| F6 | MINOR | §2.7 Advance “refused only”, §2.9 “stops only”; MultiAreaContinuation.cs:345-351,451-465 | Fences/session mismatch and missing work consent also refuse, before all-empty check. No year cap is correct. | Qualify as biological stopping condition after valid fence and required consent; distinguish overflow from refusal. |
| F7 | MINOR | §2.2 handoff §1.1 citation | Numeric genesis values exist under handoff §1 IMPLEMENTED item 1. §1.1 is Deviations. Not fabricated, but wrong section. | Correct citation; preserve author-reported/unverified label. |
| F8 | MINOR | §2.4 aggregates/spacing, E6/E13; AnnualRules.cs:88-117; SitkaGrowthModel.cs:55-60,103-106 | N/ha is float division then widening (E9 correctly says so). CI/DBH/crown f32; S/p double after float CI. E6 algebra hides f32 NextHeight then subtract/add/clamp. p has mean-vulnerability<=0 return-zero guard. Positive increments need not exceed f32 spacing: D>=120 gives zero; rounding can stop growth. | State precision/evaluation order/zero/null guards and distinguish algebraic equations from bit-exact implementation. Qualify spacing statement and stored-double versus all-double arithmetic. |

Missing consequential weaknesses: F1 makes the proposed repeat gate fail regardless of biology; F5 leaves treatment conclusions underdetermined. Both outrank README drift. Boundary preserves its last survivor, not survival through preceding hazard (AnnualRules:35-59,120-135); do not generalise “last survivor always kept” beyond boundary.

## Anchor audit: 40 inspected checks

PASS means the stated narrow source claim matches, not that tests/runtime passed. Ranges were actually read.

| # | Report scope | Candidate anchor(s) inspected | Result |
|---|---|---|---|
| 1 | §0 linkage | LAB/src/Core/SitkaP1.Core.csproj:27-28; CcfSourcePins.cs:16-23 | PASS; links and ls-tree match all four blob pins. |
| 2 | §2.1 state | AnnualEngine.cs:7-12 | PASS fields/types. |
| 3 | §2.1 age | AnnualRules.cs:99; MultiAreaExperiment.cs:101 | PASS checked increment/initial domain. |
| 4 | §2.1 IDs | MultiAreaExperiment.cs:88-97 | PASS area-prefix/ordinal/global uniqueness. |
| 5 | §2.1 positions | MultiAreaExperiment.cs:116-123 | PASS mm-to-m f64 then f32. |
| 6 | §2.1 dimensions | DerivedManifest.cs:77-85 | PASS conversions and crown initialisation. |
| 7 | §2.2 geometry/seed/cohorts | MultiAreaGenerator.cs:28,38-51 | PASS generator weights/parameters; 70/30 are weights, not proof of exact realised counts. |
| 8 | §2.2 area | MultiAreaGeometry.cs:130; MultiAreaExperiment.cs:45 | PASS denominator. |
| 9 | §2.2 assertions | LAB/tests/Core.Tests/MultiAreaTests.cs:64-78 | PASS actual areas/counts/cohort/RD assertions. |
| 10 | §2.2 site/isolation | MultiAreaExperiment.cs:20,25-29,81 | PASS UNKNOWN/constant site/isolation/seeds. |
| 11 | §2.2 metadata | MultiAreaContracts.cs:152; MultiAreaExperiment.cs:34 | PASS informational threshold. |
| 12 | §2.3 E1/E2 | AnnualRules.cs:15-16 | PASS year/living order. |
| 13 | E3 | AnnualRules.cs:6-7,67-82 | FAIL unit; equation/cutoffs correct (F2). |
| 14 | E4/E5 | AnnualRules.cs:8-10,88-91 | PASS response/site/taper/clamps/no age. |
| 15 | E6 | CCF/SitkaGrowthModel.cs:37-60; AnnualRules.cs:92-93 | PASS algebraically; f32 caveat F8. |
| 16 | E7 | AnnualRules.cs:94-98; CcfSourcePins.cs:28-30 | PASS post-growth DBH potential/relaxation/floor. |
| 17 | E9 | AnnualRules.cs:102-118; WorkContract.cs:122-138 | PASS aggregate arithmetic. |
| 18 | E10 | CCF/SitkaGrowthModel.cs:67-75 | PASS RD/constants. |
| 19 | E11/E12 | AnnualRules.cs:19-33; SitkaGrowthModel.cs:98-101 | PASS mixed chronology and suppression. |
| 20 | E13 | AnnualRules.cs:39-40; SitkaGrowthModel.cs:90-96,103-114 | FAIL probability units/omitted zero guard (F2/F8). |
| 21 | E14 | AnnualRules.cs:41-53; CCF/SimulationRandom.cs:41-55 | PASS conditional keyed 24-bit [0,1) roll. |
| 22 | E15 | AnnualRules.cs:58-59,120-142 | PASS boundary ordering/ties/last survivor within boundary. |
| 23 | E16 | AnnualRules.cs:60-64; MultiAreaContinuation.cs:33-35 | PASS dead carry-forward. |
| 24 | §2.4 integers/bits | MultiAreaGeometry.cs:19-91; F32.cs:10-28; DerivedManifest.cs:46 | PASS long/Int128 and P1 0.09-ha bits (not MA-A denominator). |
| 25 | §2.5 determinism | DerivedManifest.cs:49; SimulationRandom.cs:41-66; MultiAreaGenerator.cs:100-120; MultiAreaTests.cs:20 | PASS RNG/generator/pinned test hash. |
| 26 | §2.5/§2.9 repeats | MultiAreaContinuation.cs:55-57,191-194,532-534 | FAIL full-view equality; biology independence true (F1). |
| 27 | §2.6 diagnostics | MultiAreaContinuation.cs:414-431; MultiAreaProjection.cs:13-32; MultiAreaTests.cs:234-256 | PASS area projection/isolation assertions. |
| 28 | §2.6 CI test | LAB/tests/Core.Tests/PlayerCompetitionTests.cs:11 | PASS test exists, not run. |
| 29 | §2.7 Crop | MultiAreaContinuation.cs:88-131,380-384 | PASS protection/cleared dead marks. |
| 30 | §2.7 pending | MultiAreaContinuation.cs:136-178 | PASS refusals/zero Fell. |
| 31 | §2.7 Approve/Cancel | MultiAreaContinuation.cs:181-210 | PASS minted plan, 0 Fell, discarded orders. |
| 32 | §2.7 execute/replay | MultiAreaContinuation.cs:219-330, especially 229-235,268-275 | PASS exact replay/empty codes/atomic publish. |
| 33 | §2.7 Advance | MultiAreaContinuation.cs:340-405,451-465 | FAIL literal “only” refusal; other chronology correct (F6). |
| 34 | §2.8 chronology | MultiAreaContinuation.cs:278-279,291-296; MultiAreaContracts.cs:37-38; MultiAreaTests.cs:385-386,407-427 | PASS y Fell, post-Fell annual input, natural deaths y+1. |
| 35 | §2.9 horizon | AnnualRules.cs:15,99; MultiAreaContinuation.cs:350-354,395; LAB/tests/Core.Tests/ContinuationTests.cs:44-53; MultiAreaTests.cs:216-232 | PASS no cap/checked integers/12 and 10-year loops; not runtime proof. |
| 36 | §2.9 routes | AnnualEngine.cs:3,84-85; LAB/src/Cli/Program.cs:56-81; LAB/src/Ui/PlayerMultiApi.cs:61-79 | PASS one-year CLI versus internal/UI multi-area routes. |
| 37 | §3 history | MultiAreaContinuation.cs:16-26; MultiAreaContracts.cs:56-67,120 | PASS retained root/history vs transient reports/in-memory only. |
| 38 | §4 absent | DerivedManifest.cs:58-59; AnnualRules.cs:67-98; LAB/src/Core/PlayerSessionView.cs:67; Docs/Research/WindthrowV1/README.md:1-5 | PASS absent systems, unavailable economics, separate dormant Unity storm; Core keyword search finds no volume/assortment/deadwood/damage implementation. |
| 39 | §7 W2/W7/W8/W9/W11 | AnnualRules.cs:17-33,67-99,132; SGM/CurrentModelAudit.md:54; MultiAreaContracts.cs:56-67 | FAIL W2 slowdown and W11 absolute irreproducibility; age independence/crown-only/mixed chronology match (F3/F4). |
| 40 | §7 W12 | LAB/README.md:153; AnnualEngine.cs:57-63; LAB/src/Cli/Program.cs:56-81 | PASS documentation drift. Exact README command lacks fixture directory: code returns usage64 after admission; supplied-directory command opens/advances. Runtime NOT VERIFIED. |

## Units/numerics

1 ha=10,000 m²=10^10 mm²; twice-shoelace/2e10 correct. Areas 915/1500 m² imply nominal .0915/.15 ha before f32 rounding. One stem is nominal 10.92896/6.66667 per ha, matching W10 approximations. mm/1000 gives m; mm/10 gives cm. Crown intercept m, slope m per cm DBH.

N/ha is f32 count/areaHa then double. Dq=sqrt(sum(double(D)*D)/count) cm. BA/ha=sum(D_cm²)*pi/40000/areaHa m²/ha (1/4 radius factor and 1/10000 cm² conversion). RD=(N/ha)/(1868 stems/ha)*(Dq/25 cm)^2.063 dimensionless. P/S/response dimensionless; CI/p corrected in F2. Vulnerability clamps S to [0,1], p returns zero for nonpositive pressure/mean vulnerability. Suppression/p/roll may instead be null when unevaluated.

No fixed-point biology. Tree state/CI/DBH/crown/area f32; double accumulation/stand/mortality maths with f32 inputs and N exception. Height double envelope returns float, followed by float subtract/add/clamp. F32 bits preserve storage, not proof every UI JSON uses that representation. Spacing at 100 cm is 2^-17 cm≈7.6294e-6. E6 and spacing need F8; algebra alone cannot reproduce exact evaluation bits.

## X1–X15 research audit

Audits repository composites/extraction/matrix, not unseen original papers. EM means actual file line including header. Original paper/PDF accuracy remains NOT VERIFIED.

| Row | Citations actually checked | Assessment |
|---|---|---|
| X1 | Composite §4; GROWFOR_Technical_Evidence_Extraction_v1 §5; EM2,3; IrishHeightValidation table; SitkaGrowthModel1:31-33 | PASS Irish H30=20.4/shape/rescaled b0. GROWFOR 47.0216/.4571 distinct. Beyond-age30 values modelled. |
| X2 | Composite §4 bullet2/§5 forms; GROWFOR §4.5; EM4 | PASS qualified direction/simplification. Top-height independence does not validate invariant individual ratios; report discloses limitation. |
| X3 | Composite §5; GROWFOR §14.2; EM10,12 | FAIL wording F3. Age/vitality absence and [C] potential correctly stated; crown also responds to CI. |
| X4 | Composite §5 Hegyi; GROWFOR §2A.4/§14.2; EM16; CurrentModelAudit:54 | FAIL inference F3. Mechanism agreement supportable; response/CI50 abstraction. Ratio invariance is not absence of DBH slowing. Historical audit predates current mortality. |
| X5 | Composite §5 Frenchpark table; GROWFOR §7; EM11 | PASS qualified direction/unknown transfer. DBH 23.7/29.8/31.1/32.4 and calibration 1.28/1.38 vs 1.26/1.37 exist. Selection-effect caveat present. Not new MA measurements. |
| X6 | Composite §5/§8; EM13–15 | PASS envelope/k starting range, not empirical validation of realised crown response. Positive pooled coefficient not imported. |
| X7 | Composite §2/§3; GROWFOR §2A.3; EM17,18,23; AdultMortalityCalibration:15-17 | PASS British transfer/unverified Ireland; GROWFOR stocking dynamics do not validate individual hazard. |
| X8 | GROWFOR §2A.3; EM19,24–27; AdultMortalityCalibration:51-60 and starting-stand context | PASS calibration/transfer limitation. EM19 earlier prototype, not selected formula; Forest Yield unobtained. |
| X9 | Composite §2; GROWFOR §4.1/§14.3; EM21; AdultMortalityCalibration:3 | PASS definitions and one-trial ~55 BA flag, no universal cap. Unsupported 80–90 ceiling dropped. |
| X10 | Composite §4/§9; GROWFOR §3.2/§4.5/§11.1; EM2,9 | PASS top-height/site variable absent; tentative derivation convention; screenshot-specific 49.3-m limit retained. |
| X11 | Composite §5 volume; GROWFOR §8/§10 | PASS cited volume/assortment descriptions exist; lab absent. |
| X12 | Composite §5 light/seed/dispersal/establishment; GROWFOR §14.2 | PASS absent systems, not missing passing thresholds. |
| X13 | Composite §6; EM22 | PASS stand-level wind warning/unsourced H-D indicator not promoted; lab absent. |
| X14 | GROWFOR §3.3/§9 | PASS removed/retained state controls and transition in extraction; absent automatic lab prescription. |
| X15 | Packet02 Input/R1; candidate Docs search Page et al/Stokes2009/CONTINUCOVER | PASS UNKNOWN. No matching mentions in candidate Docs search. Packet summaries do not replace unavailable uploads. |

All three non-valid claims correctly refused: Composite §5 says BA about30 can sometimes provide about20% RLI depending on arrangement, not BA>30=>zero growth. Composite §6 is site-dependent stand windthrow, not >50% removal=>windthrow. Composite §5's 0.85/.44 m³ means are one trial, not universal doubling of retained-tree volume. Relevant subsystems are absent. No universal law imported on these points.

## Protocol/weakness disposition

Approve0/Execute1, atomic two-area event, unchanged annual clock on Fell, y+1 natural deaths, cancellation consent, replay and exact empty refusal agree with code. Canonical area seeds fixed; optional replication variants separately authorised and labelled, correctly. Shared two-area plan explicit; no percentage fixed. No correction authorises protected-rule changes. External harness still needs later lawful physical/source admission and route decision under packet01.

W1 recruitment absence, W8 output-only crown, W10 granularity are established limits, not uncertain ecological hypotheses. W3 transfer/W4 edges/W6 seeds are useful concerns, but sensitivity diagnostics do not prove ecological validity. W5/W9 are implementation conventions, W7 follows directly from code. W11 retention/replay, W12 documentation drift. W2's formulation is false. Ranking may reasonably put scope/transfer above drift, but exact ordinal consequences are unsubstantiated; group these explicitly and justify rank.

## Explicit NOT VERIFIED

- Author historical session identity/commands/no-run assertion/clone cleanliness/ignored outputs/custody: declarations, not witnessed. Current status is not historical attestation.
- Three uploaded research returns, Page/Stokes/CONTINUCOVER originals, primary-paper/PDF/table accuracy. Repository evidence checked; missing uploads not reconstructed.
- Distinct original accepted MA-1 specification/owner acceptance beyond packet and implementation comments; no separately approved specification established here.
- Tests actually passing, 13 cross-boundary pairs, generated manifest bytes/hash, genesis N/ha/BA/Dq/RD independently executed calculations. Constants/assertions and author numbers exist; no generator run.
- 20/50-year completion, any trajectories/treatment intensity/seed sensitivity/edge effects/calibration transfer/empirical comparability/G1 or runtime behaviour. Source permits 50 years, does not prove success.
- Physical D-091 admission/lease, integrated and verified candidate, eventual harness executability on selected host.
- Exhaustive absence of private historical 20/50-year evidence. Cited code loops 10 MA/12 single-area years; no claim to have inventoried private runs.
- Dashboard ingestion/published dashboard/Drive synchronization; no board writes authorised. Outstanding Manager operational handoff.

## Verdict

**NEEDS CHANGES**

Artifact integrity passes, most equation anchors match, and absent-system/evidence-transfer boundaries generally hold. Approval is withheld because full-view repeat equality is structurally impossible, CI/probability units are wrong, the growth limitation is overstated, several proposed falsifying tests cannot establish their conclusions, and staged/adaptive treatments lack executable decision rules. These need documentary/protocol repair, not engine changes. No fabricated engine result identified. This verdict grants no integration, ecological validation or owner acceptance.

Smallest correction set to flip: (1) deterministic projection/ID normalisation; (2) CI/p units plus precision/zero/null caveats; (3) narrow X3/X4/W2 and correct weakness testability/tolerances/consequence ranking; (4) staged equivalence, outcome-independent allowable-set stopping, matched-tree comparisons and all-year logging; (5) qualify refusal language and fix handoff section citation. Freeze revised report and obtain bounded independent changed-lines recheck. Keep Q1 conditional on lawful admission; do not alter protected rules or run engine merely to satisfy this review.
