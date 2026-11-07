# CCF Development Bottleneck Audit — Full Available History

**Audit date:** 7 November 2026 (morning UTC)  
**Status:** Manager research/operational retrospective; **recommendations only**. This report does not change accepted game design, AgentWorkflow, task ownership, verification gates, or production code.  
**Scope:** Full available GitHub history beginning with the first commit on 14 September 2026; detailed issue/claim window 27 October–7 November 2026; uploaded 30 October orchestration ledger and GitHub orchestration, qualification, integration, review and retrospective handoffs.

## Executive assessment

The repeated avoidable cost is not primarily slow game-code implementation or Unity import. It is a sequence of **late player-facing contract discovery → stale or unsuitable verification tools → serial reviewer/Manager disposition → repinned source/Unity candidate**. The three most consequential improvements would be: (1) prove one real user-path vertical slice before broad implementation, (2) qualify the verification approach and old fixtures on the parent baseline, and (3) batch low-risk handoffs/decisions while reserving exact-head independent reviews and Unity qualifications for actual risk boundaries.

The existing safeguards have prevented substantive defects: unknown species loading as Sitka, Rack V2 approved work not reaching lesson progression, missing management fit-replay approval, and source-checker false greens. The recommendations do **not** justify relaxing save/reference/determinism protections, genuine negative testing, exclusive Unity ownership, or the pre-Windows user playtest gate.

## Coverage, counts, and caveats

- The earliest reachable commit in the current repository history is **14 September 2026**; from then through 7 November is approximately **54 calendar days**. GitHub task claims begin **27 October 2026**. Earlier active-time, model quota, task-to-task waiting and number-of-reruns metrics are not available at equivalent granularity.
- GitHub issue-search records show **272 actual issue records created 27 October–7 November**, of which **60 titles explicitly identify a duplicate, no-op, accidental, empty, void, or placeholder claim** (22.1%). A closed accidental issue does not imply a worker spent hours on it, and issue titles are not a complete account of code or test outcomes. The issue count is not hours.
- At audit time the default branch contains **141 named handoff directories** plus one handoff-root file. They are not 141 unique engineering tasks and no direct time attribution is possible.
- The period 29–30 October alone produced **107 issues including 33 explicitly administratively labelled issues**; the same labelling method finds **zero** over 4–7 November. Administrative issue creation has improved in the latest interval and should not be represented as an unchanged present-day failure.
- The already published two-day post-playtest rack retrospective measures **about 44h 18m calendar elapsed**, not engineering time, and includes concurrent work/approval pauses. Its actual Unity lease was **63.5 minutes**, with cold import/compile approximately **9.9 minutes**. Do not subtract or sum overlapping intervals to invent worker-hours.

Daily issue-index audit (all counts are title classifications, and categories such as review/test overlap):

| UTC date | Issues | Explicit admin/duplicate-labelled |
|---|---:|---:|
| 27 Oct | 28 | 7 |
| 28 Oct | 27 | 7 |
| 29 Oct | 77 | 23 |
| 30 Oct | 30 | 10 |
| 31 Oct | 17 | 1 |
| 1 Nov | 15 | 4 |
| 2 Nov | 5 | 3 |
| 3 Nov | 19 | 5 |
| 4 Nov | 14 | 0 |
| 5 Nov | 12 | 0 |
| 6 Nov | 17 | 0 |
| 7 Nov | 11 | 0 |
| **Total** | **272** | **60** |

## Longer chronology

**14 September–4 October: core game and protections took shape.** Initial tree simulation, forestry work, ecology and save systems are visible in repository commits from 14 September. The project then formalised authoritative Git/Docs context and retained existing technical work; the 4 October Scenario One functional integration passed 27/27 integration regressions and the user's interactive smoke. The historical evidence establishes delivery, not a measured benchmark for what later engineering should cost.

**5–12 October: repeated model/schema evolution and widened demo scope.** RNG, regeneration, growth, understorey, storm, stand dimensions and save-format versions changed in a short sequence, with protected legacy model/version paths retained. On 12 October the user selected complete Scenario One as the first external demo and comparable Sitka/Oak/Beech modelling depth. This expanded the legitimate amount of integration and compatibility work; it also made stale version-pinned fixtures a foreseeable risk.

**22–26 October: selective specifications and Rack/MFM feature scope.** The project accepted spec-driven development for substantial/ambiguous features, approved the enlarged-stand Rack V2 and the multi-scale management workflow. Proper scopes are valuable, but feature specs must trace to the real UI/3D/save journey rather than only API and component checks.

**27–30 October: integration seams and verification debt.** A Rack V2 combined candidate passed many protective checks and approved 149 rack-owned work orders, but the old lesson/progression consumer still saw 0/5 racks: the full accepted player sequence could not proceed. A narrow repair was necessary. The 30 October overnight orchestration then rediscovered multiple old save-version-23 harness pins after version 24, launched successive verification-only repair claims, and encountered an intermittent Editor exit hang after PASS. The same overnight run also found a **real** unknown-species save-validation defect. This period shows why we must fix test infrastructure without waiving high-value tests.

**31 October–3 November: large integration qualification with some successful test discipline.** The multi-scale management final qualification stopped because the deterministic sample-fit calculation had never been connected to the live review/approval screen, despite a passing core and eleven earlier static/offline gates. A later bounded integration fix allowed qualification. On 3 November the final Tier-3 evidence had three remaining gaps (same-frame preview harness effect, contaminated native pointer input, genuine completed-state render), with stale six-tab expectations. The user authorised about a 50% reduction of *remaining redundant test executions*, expressly preserving critical contracts. The Manager reused valid earlier evidence and approved the targeted three-action closure without a production change. This was an effective counterexample to full-matrix rerun habits; the amendment specified a target rather than documenting precise measured savings.

**4–7 November: personal-playtest expectation gap and verification-tool spiral.** The user found absent next-action highlighting and expected full-map features after Rack V2 core qualification. The original Stand Map did not use the mesh representing existing headland/feeder geometry, so the first teaching-diagram plan could not be truthful without another map change and decision. The dedicated two-day retrospective reconstructs this in detail. Source checkers then repeatedly accepted deliberately broken C# examples (successive reviewer groups of 50, 26 and 57 false greens), while the first Workbench Unity gate met unregistered clone/store, package rewrite, legacy fixture, runner and SSD-capacity problems. The current morning work is narrow repair/review, not an integrated, player-ready candidate.

## Ranked recurring causes and preventative changes

### 1. Missing real player-path acceptance early — **very high** confidence

**Evidence:** The October Rack integration discrepancy (149 approved vs tracker 0/5); the 2 November MFM fit-replay core that existed without a live approval UI; the 5 November personal-playtest report of missing cues/map representation. These are different features exposing the same core-to-view/workflow integration gap.

**Change:** Before feature coding, require a one-page acceptance strip with (a) current actual screenshot/source of truth, (b) target screen(s), (c) first-touch player action, (d) authoritative state transition, (e) old-save/Reference behaviour, and (f) explicit do-not-change scope. Implement the *smallest full player journey* early, with one real Editor walkthrough at feature midpoint if it is UI/3D-heavy; do not save the first true user-path test for expensive final qualification. This supplements rather than replaces an approved four-file feature specification for substantial workflows.

### 2. Stale verification fixtures and ambiguous test-runner receipts — **very high** confidence

**Evidence:** Two waves of save-v24 harness reconciliation on 30 October; more historical v23 pins recorded after those waves; stale save/tab assertions in 2–3 November work; v23 fixtures, native input, false-PASS and state reset discrepancies in 7 November Workbench qualification.

**Change:** At task start, run one **baseline verification-health preflight** on the unchanged parent state. Record actual current schema, fixture versions, runner mode, native-input authority, expected PASS/exit status and known red assertions. Maintain fixture-version access through shared test support rather than hardcoded independent copy-pasted versions. Distinguish failed product test from test-harness/runner failure; preserve red evidence. A PASS marker is not success unless process exit, terminal receipt and expected native/run coverage agree.

### 3. Static checker proves too much for its technique — **very high** confidence

**Evidence:** Objective CTA lexical/source checkers entered successive independently reviewed false-green loops. Large mutation counts measure scanner limitations, not necessarily many new player-facing defects. The five-rack source integrity checker needed repeated source-inventory and self-mutation hardening. A checker that attempts to infer actual C# ownership, interpolation, user actions or lifecycle semantics from lexical text is a brittle test oracle.

**Change:** Classify the proof obligation first. Use narrow lexical checks for simple literal/layout/file-presence questions only. When C# semantics matter, use compilation/semantic inspection (Roslyn or equivalently typed/compiled small harness) or a genuine Unity runtime path. Create a small adversarial negative set **before** rolling out a checker, including a deliberately broken real-world variant. After repeated novel false-green classes, stop adding regex patches and open a bounded *tooling architecture* decision. Do not relax negative controls; change the method of proof or explicitly mark the remaining assurance as unproven.

### 4. Too many administrative/Manager round trips for small units — **high** confidence

**Evidence:** 272 issue records, 60 explicit administrative/no-ops, the large handoff directory inventory, repeated separate source reviewer/Manager/repair chains and multi-turn orchestration commentary. Long-running browser/orchestration umbrella issues also carry many status edits, but comments do not equal wasted work and all high-risk approvals cannot be delegated away.

**Change:** One substantive execution claim per bounded work product, not a new issue for each small check, doc correction or failed attempt. Make claim creation idempotent, enforce unique task key before create, and keep existing claim through in-scope retries. Publish **one concise stage handoff** linking machine receipts instead of repeating full roadmap and historical state. Within a pre-approved **repair envelope**, the worker may remedy expected in-scope check failures and rerun Tier 1 without a fresh Manager decision; external G2/G3/G4 decisions remain at genuine product, measurement, acceptance and integration boundaries. Batch low-risk sibling changes under one independent stage review when file ownership and traceability permit. Require immediate escalation for scope/API/save/reference ambiguity, conflicts or failed protected gates.

### 5. Late integration seam/lineage checking — **high** confidence

**Evidence:** Rack V2 consumers reading legacy rack layout; MFM fit core missing UI call path; the diagram checker inventory failing after an authorised new headland checker was added to another part of the same source lineage; multiple fresh candidate repinnings and partial source-only approvals.

**Change:** Every feature packet names state producer, state consumers, UI event routing, save identity and exact integration owner. Perform an early thin integrated test, not only parallel isolated component tests. Record a single candidate-evidence map: which proofs belong to changed bytes and which exact unchanged ancestor proofs can be safely inherited. Do not rerun unrelated Tier 3 after docs-only/source-tool changes, as already authorised by AgentWorkflow.

### 6. Unity/storage/lock readiness checked too late — **high operational severity**, moderate contribution to measured recent elapsed time

**Evidence:** The Workbench source gate was ready well before Unity lease admission. An isolated clone was not in the authentic registered Git common-store/worktree registry. A cold Library consumed around 6.6 GiB, leaving insufficient safe room for another. The Editor rewrote eight tracked files (including package-version drift). The 7 November lease itself was ~63.5 minutes, not the entirety of the overnight run.

**Change:** Perform a **no-Editor admission rehearsal** at the start of any likely Unity task: canonical worktree identity/registration, protected clean starting bytes, installed package and lockfile version parity, verified old-save fixture, available genuine lock, estimated one-Library disk budget and stop thresholds. Do not start Unity solely to discover a missing registry path. Preserve the 15 GiB floor and actual exclusive lease. Qualify one exact head and reuse a safe Library only under an approved no-contamination policy; never reset or delete unrelated dirty Unity work.

### 7. Product-option research and model availability — **medium** evidence of avoidable critical-path cost

**Evidence:** Access-spine/diagonal variants were researched then superseded by retaining five parallel racks for Scenario One. Both overnight ledgers record model quota/auth complications, but the 7 November directly documented failover was on the order of ten minutes, not the 44-hour explanation. Many capacity fields remain UNKNOWN.

**Change:** Timebox multiple-option research, obtain the actual product selection before full spec/implementation, and place later-scenario options in the parking lot. At dispatch, preflight capacity-to-completion and reserve an independent reviewer/fallback. Keep model choice pragmatic; token capacity and provider brand alone are not proxies for quality or root cause.

## Appropriate verification rather than "more" or "less" testing

| Work type | Preferred evidence during iteration | Required handoff/integration boundary |
|---|---|---|
| Simple copy, docs, reversible source-only UI formatting | Fast exact-file checks / compile if source affected | No Unity solely for documentation; visual check only for actual UI change |
| Rack/map/objective/teaching UI | Narrow compiled/semantic test plus an early real path; accurate screen/3D source truth | Bounded Tier 2 native input, 720p/1080p legibility when relevant, truthful old-world conditional render |
| Save schema, persistence, Reference Future, RNG, identity | Strict negative/malformed-save/deterministic fixture tests, hash boundary, independent review | Exact candidate save/load/back-compat and protected Tier 2; final Tier 3 once integrated |
| Forestry growth/calibration | Offline reduced-model parameter search, representative deterministic comparison | Focused Unity equivalence, broader seeds/long-horizon only for a stated uncertainty or milestone |
| Cross-system Work Plan and management | Real producer-to-screen-to-approval-to-execution journey | Feature end-to-end Tier 2; exactly one integrated complete-loop Tier 3 on release candidate |
| Verification tools themselves | One red control for each claimed critical proof, process exit correctness | Independent G3 review when assurance, expected hashes or thresholds are changed |

Retain accepted tests on unchanged production bytes. Explicitly label inherited evidence and residual limitations. Do not substitute a synthetic unit fixture for an observed player path or treat source analysis as native Unity behaviour. The existing Tier 1/2/3 cost policy is sound in principle; the opportunity is **enforcement and proof-to-risk matching**, not another wholesale framework.

## Proposed next-feature operating procedure (not yet accepted policy)

1. **Manager scope gate once:** approved feature specification (if required), visual acceptance strip, risk classification, protected boundaries and pre-authorised repair envelope. Batch all genuine user product questions into one decision.
2. **Baseline health and resources:** runner fixture-version/known-red preflight and a no-Editor Unity admission rehearsal. Fail before starting expensive work if the lane is ineligible.
3. **One bounded primary source implementation:** produce a thin full vertical slice, run Tier 1, preserve observed reds. Use the same substantive claim for permitted in-scope repairs rather than new administrative claims.
4. **Appropriate independent review:** mandatory on protected G2/G3 changes, otherwise risk-scoped; reviewers inspect exact changed production bytes and actual negative tests. One consolidated Manager disposition for a complete reviewable stage.
5. **One exact-head Tier 2 Unity feature qualification:** source-to-native path, scoped compatibility and screen evidence, no unrelated full-regression rerun. Review and integrate once accepted.
6. **One final integrated Tier 3 candidate:** reused immutable evidence with a transparent changed-surface delta, mandatory complete-loop/source-of-truth and save/reference coverage, independent release review; personal user playtest before any Windows build authorisation.

This does **not** permit workers to self-approve protected changes, silently change specs, bypass independent review, launch Unity without a real lock, reduce assertions to green, or skip a real blocker. It lowers communication overhead between those gates.

## Measure improvement over the next two features

Record six timestamps on the existing task claim: READY, actual RUNNING, WAITING (reason), HANDED-OFF, REVIEW VERDICT, INTEGRATED/VERIFIED. Separate actual active execution from queue/Manager wait and from Unity lease minutes. Attach the exact candidate and test receipt. Count: (a) time to first working player-visible slice; (b) stale fixture failures first discovered after Unity; (c) negative-checker false greens discovered after an author-declared pass; (d) non-substantive issue claims; (e) number of independent review cycles per bounded stage; (f) redundant full-matrix reruns; (g) accepted ancestor evidence reused; (h) total critical-path calendar elapsed. **Do not infer saved time from issue counts, check counts or model branding.**

Suggested pilot success criteria: no accidental/duplicate claims, no historical version-pin surprise during Tier 2, no duplicate full Tier 3 on unchanged production, and an actual user-path proof before final feature handoff. Keep the same protected acceptance quality. If any metric worsens, review rather than automatically weaken controls.

## Evidence index (representative primary/manager records)

- [Canonical Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), [AgentWorkflow verification cost/review policy](https://github.com/jermur788/CCF/blob/main/Docs/Project/AgentWorkflow.md).
- [Rack V2 combined integration Manager review](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/RACK-V2-Combined-Integration/MANAGER-REVIEW.md).
- [30 October overnight result](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-RESULT-20261030.md) and [append-only orchestration ledger](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-LEDGER-20261030.md).
- [MFM-12 Manager stop after missing live fit replay](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md).
- [3 November Tier-3 targeted test reduction](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/TEST-REDUCTION-AMENDMENT-20261103.md) and [final accepted evidence closure](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/MANAGER-FINAL-REVIEW-20261103.md).
- [5 November playtest feedback](https://github.com/jermur788/CCF/issues/281), [D-072 checkers and review decision](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-STREAM-A-B-REVIEW-RULING-20261106.md).
- [7 November two-day rack-update retrospective](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Throughput-Audit-20261107/MANAGER-RACK-UPDATE-TWO-DAY-RETROSPECTIVE.md), [Mimo overnight ledger](https://github.com/jermur788/CCF/blob/docs/mimo-scenario-one-overnight-20261107-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MIMO-OVERNIGHT-EXECUTION-LEDGER.md), [morning Manager D1–D7 disposition](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-200-OVERNIGHT-MORNING-D1-D7-DISPOSITION-20261107.md).

**Limitation:** GitHub's issue index and selected representative handoffs were reviewed; this is not a claim of opening every individual historical Git blob or accurately timing every chat interaction. The issue-index counting is reproducible from day-scoped GitHub issue searches (not PR counts). Provider-time/cost attribution remains unknown where not directly logged. No accepted project authority or active task is changed by this audit.

**Operational status:** Research handoff only. No production edit, no new Unity lease, no change to existing live owners or waiting/running tasks. Dashboard state should not advance merely because this advisory report is published to its own docs branch. Any policy adoption requires deliberate Manager/user acceptance and subsequent separately authorised canonical workflow edit.