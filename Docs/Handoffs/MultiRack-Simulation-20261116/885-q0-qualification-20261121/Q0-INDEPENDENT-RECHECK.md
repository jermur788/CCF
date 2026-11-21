# Multi #885 Q0 revision 2 — independent changed-lines recheck

Reviewer: OpenAI **GPT-6.1 Sol (gpt-6.1-sol)**, Codex harness in T3 Code, independent nonauthor reviewer. Fresh child context; session/thread UUID not exposed. Date: 2026-11-21. No subagents. Claude Opus 5.5 authored both crosswalk revisions; I authored neither.

## Frozen identities and integrity

Reviewed report: Docs/Handoffs/MultiRack-Simulation-20261116/885-q0-qualification-20261121/Q0-EQUATION-AND-RESEARCH-CROSSWALK.md, branch docs/multi-885-q0-equation-crosswalk-20261121-v1.

| Artifact | Head | Blob | Independently recomputed SHA-256 |
|---|---|---|---|
| Revision 1 | 4be3b2f3cddcbf9c10a60e17769d3d6e1be1846d | bd125f54fdcd0d8c0ee2f3a481b59728e0570455 | b38a52722415061584c80b86d40e7bdad86813c87f1e32e0959ad4b293258bb1 |
| Revision 2 | fc1c00f4dc2d69722d41d0cecd1146c04698f3d0 | 980392974029cecd75e1640a664ed6cfcc704e16 | e797f34de4a738eaabd93e74fed6c22870c129990b228263fe23864e09e3e415 |

MATCH: immutable GitHub commit/content responses confirm revision 2's head object, blob, sole parent 4be3b2f3cddcbf9c10a60e17769d3d6e1be1846d, exactly one modified file, +184/-54 and 507 lines. Sections 0–9 survive; §0.1 exists at line 45. Revision 1 is 377 lines. Frozen commit identity is verified independently of any later branch movement.

Original review: Q0-INDEPENDENT-REVIEW.md on docs/multi-885-q0-crosswalk-review-20261121-v1, blob cb78f64975a6915a062ae733c11b36fabc665367; retrieved and read. Review branch head before publication: daab5ed726c2557551cd54d842ad98c197998d86. All code evidence below is read-only git show at **4cecf87339bea4d32907955a8d5625d1f27f96bd**. LAB means Tools/MultiRackSitkaP1Lab; Core paths below are beneath LAB/src/Core. Unity source means Assets/ForestPrototype.

Scope: the +184/-54 diff and claims it introduces or changes, checked against F1–F8. This is not another 40-anchor review. Local status was empty at intake and after source inspection. Required project context and live authoritative GitHub coordination were consulted; no coordination edit is authorised. The only repository write is this new report, published through authenticated WSL gh as jermur788. Scratch is outside Git. No worktree, source/report/original-review edit, Unity, build/test, engine, server, PR, issue comment, board edit or merge.

## F1–F8 disposition

Line references in this table are revision-2 report lines.

| Finding | Result | Revised location | Justification |
|---|---|---|---|
| F1 | PASS | §2.5, lines 151–175; §6 lines 345–349,381 | Directly compared fields contain no session identity; SessionId is excluded, and recursive SESSION: replacement covers Approved, Events, nested Fell records and Execution's key/event; Pending has no such fields. Within-session checks, engine ordering, same-runtime JSON and bit logging are specified. Contracts.cs:30–50,88–97 and Continuation.cs:190–194,504–534 support this. |
| F2 | PASS | E3/E4 lines 114–115; E12/E13 lines 123–124; §2.4 lines 133–143; measurements line 376 | CI is m^-1, CI50 is 5 m^-1 by the shared E3–E4 definition, and E12 uses that CI50. Remaining dimensionless labels apply correctly to the DBH ratio, response, suppression, RD or annual Bernoulli p. No probability-to-rate conversion remains. |
| F3 | PASS | X3/X4 lines 288–289; H-W2 line 434 | Retains clamp(1-D/120), acknowledges crown response, and narrows the missing feedback to a direct N/ha/RD term; fixed-roster scaling removes the assumed pre-mortality window. AnnualRules.cs:67–98 agrees. |
| F4 | PARTIAL | §7.1 lines 399–412; W5/W11 lines 424,427; §7.3 lines 434–437 | Ranking now has consequences, W11 is a replay/retention risk, and thresholds/confounders are explicit; H-W4 still lacks an executable ΔD decision rule and handling for absent/rank-deficient strata (R1). |
| F5 | PARTIAL | §6 lines 326–344,361–386 | Stems-based S1 equivalence, mismatch/shortfall disclosure, outcome-independent list exhaustion, matched survivors, all-year logging and null semantics are supplied; shared-competitor traversal/deduplication still leaves the selected treatment underdetermined (R2). |
| F6 | PASS | Advance line 191; §2.9 lines 202–205 | Fence/session, source and plan-state checks precede consent and all-empty refusal; checked overflow is an exception, not a refusal code. Continuation.cs:340–405,451–465 agrees. The approximate integer limit is not an assurance of reaching it. |
| F7 | PASS | §2.2 line 106; §8 line 448 | Candidate handoff §1 IMPLEMENTED item 1, lines 38–39 actually contains the cited A/B genesis values; author-reported/not-reverified labels remain. |
| F8 | PASS | E3–E7/E9/E12–E16 lines 114–127; §2.4 lines 133–144 | Float CI accumulation, float N/ha division before widening, double sums/probabilities, height cast/subtract/add/clamp, zero/null guards and potentially rounded-away DBH increments agree with AnnualRules.cs:25–44,62–63,67–117 and SitkaGrowthModel.cs:55–60,103–114. |

Counts: **6 PASS / 2 PARTIAL / 0 FAIL**. PASS is documentary/source agreement, not executed harness proof.

## Findings introduced by the revised protocol

### R1 — MAJOR — H-W4 is still not executable as written

Revision-2 line 436 defines a negative edge coefficient and a 20% interior-mean-CI threshold for CI, then says “Repeat with 10-year ΔD on the matched retained set.” The stated growth hypothesis has the opposite direction: edge trees should have larger ΔD. Literally reusing the negative-coefficient rule contradicts it; the text supplies no separate positive-growth rule or growth-scale denominator. It also gives no outcome for a stratum lacking edge/interior survivors or an estimable OLS design. Such a fit cannot yield the coefficient/interior mean needed by its mandatory every-stratum decision. Census status does not resolve estimability.

Smallest correction: explicitly define the growth window, positive ΔD criterion and its tolerance/denominator, and return NOT ESTIMABLE (with counts/reason) when required groups or design rank are missing. Keep CI and growth conclusions separate. The ln(DBH) term is a chosen adjustment, not proof that DBH confounding or edge causality has been eliminated; keep the inference descriptive/model-specific.

### R2 — MAJOR — shared-competitor traversal remains unspecified

Revision-2 §6 steps 2–3 (lines 328–329) exclude trees “already in the plan” before ranking, but also say a tree listed for two Crops is felled once. These encode different algorithms when Crop neighbour sets overlap. Crops more than 8 m apart can still share neighbours within 8 m of both (their separation can be below 16 m).

If lists are frozen first and unioned, duplicate selections consume no replacement slot. If the plan is built sequentially with exclusion before ranking, the later Crop selects its next eligible competitor instead. The latter also needs an explicit Crop traversal order; step 1's DBH traversal selects the Crop set but does not explicitly fix the execution traversal. Both interpretations follow parts of the wording and can produce different orders and intensity. S1's “rank-1 competitors, then rank-2” sequence (line 337) also needs an explicit global ordering and deduplication point.

Smallest correction: choose one algorithm (for example, freeze per-Crop eligible lists before selecting, specify Crop order and rank-layer order, then deduplicate at first occurrence without replacement), and use it consistently for L, M, D, S and S1. Explicitly issue Crop marks before planning and record a no-intervention result if the resulting order set is empty, rather than attempting an empty approval. No ecological coefficient or protected-rule change is needed.

### R3 — NOTE — W15 needs transition-year care

W15/E15 correctly identify the absence of a hazard survivor safeguard; boundary-only retention must not be generalised. However, at the year an area becomes empty its AnnualReport remains non-null and its history NoLivingStems is false (Continuation.cs:364–370). Null/true apply on a subsequent carried-forward year (:372–378), which may never commit if both areas are empty. W15's suggested first NoLivingStems year therefore measures first carry-forward, not first extinction. Use the report's After living count to identify extinction and log carry-forward separately. The deterministic keyed rolls support no claim of empirically demonstrated statistical independence; simultaneous extinction remains an unverified possibility, not a canonical result.

## Regression check

Changed lines preserve Q4 REVIEWED = PENDING (line 11), Q1+Q2 EXECUTED = NO (10), MODEL CALIBRATED and OWNER ACCEPTED = NOT CLAIMED (12–13), and G1 = UNKNOWN (14). No completed approval is claimed; Work Plan approval refers to the engine command, not scientific or owner acceptance. W13 correctly records the old repeat-gate defect; W14 correctly identifies the old underdetermination but overstates closure while R2 remains. H-W2's narrowed code identity and fixed-roster test are supported; its tolerance and roughly 40-neighbour estimate are disclosed protocol choices/geometry arithmetic, not measurements. New rank counts, thresholds and tolerances are labelled protocol choices. No fabricated engine output, imported ecological coefficient, weakened unavailable-source label or authorisation to alter protected rules was found. Stems-only staged equivalence is honestly narrower than BA/identity/timing equivalence. W5's tolerance is a proposed diagnostic, not a proven error bound or ecological calibration. R1–R3 above limit the revised protocol; no full unchanged-anchor or primary-paper revalidation is claimed.

## NOT VERIFIED — carried forward

- Author historical session identity/commands/no-run assertion/clone cleanliness/ignored outputs/custody: declarations, not witnessed. Current status is not historical attestation.
- Three uploaded research returns, Page/Stokes/CONTINUCOVER originals, primary-paper/PDF/table accuracy. Missing uploads were not reconstructed.
- Distinct original accepted MA-1 specification/owner acceptance beyond packet and implementation comments; no separately approved specification established.
- Tests actually passing, 13 cross-boundary pairs, generated manifest bytes/hash, genesis N/ha/BA/Dq/RD independently executed calculations. Constants/assertions and author numbers exist; no generator run.
- 20/50-year completion, trajectories/treatment intensity/seed sensitivity/edge effects/calibration transfer/empirical comparability/G1 or runtime behaviour. Source permission is not a successful run.
- Physical D-091 admission/lease, integrated/verified candidate, eventual harness executability on the selected host, and route handling of overflow.
- Exhaustive absence of private historical 20/50-year evidence.
- Dashboard ingestion/published dashboard/Drive synchronization; no board writes authorised. Outstanding Manager operational handoff.
- The revised projection, replay, selection algorithms, numerical tolerances and statistical diagnostics have not been executed or qualified.

## Verdict

**NEEDS CHANGES**

Scoped exclusively to revision 2's changed lines: unresolved executable decision rules in R1 and treatment ordering/deduplication in R2 still block approval; the next safe action is an author-only documentary correction with Q1/Q2 held until lawful admission and the required review decision, with R3's logging clarification carried into the protocol.
