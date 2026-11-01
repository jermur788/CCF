# Scenario One — Unified Local Orchestration Master Handoff — 2026-11-01

**Programme parent:** #91  
**Local orchestration umbrella:** #201 / **#203**  
**Browser / Overall Manager counterpart:** #200  
**Current published Scenario One integration:** `6784ab17cac922a16815f4898a30737482101018`  
**Orchestrator:** **DeepSeek agent**  
**Additional authorised worker capacity:** **Mimo 2.6 Pro**  
**Difficulty:** High overall — multi-task local execution, routing, Unity serialization, evidence capture and dependency management  
**Purpose:** one durable dispatch for all Scenario One work that is currently executable locally, plus any later local work explicitly released by #200 while this orchestration run is active.

## 1. User direction and authority

This handoff implements the user's 2026-11-01 direction to give the local chat **one handoff** for all work that can be done locally. DeepSeek is the orchestrator and may decompose, sequence, parallelise and assign work as it judges most efficient, subject to the ownership, Unity and acceptance boundaries below.

The user explicitly authorises **Mimo 2.6 Pro** as an additional worker and reports that it has **plenty of tokens left**. Treat that as **OBSERVED FACT — user-reported qualitative capacity**. Do not invent a token count, reset time or provider quota. At run start, replace qualitative/unknown capacity fields only with direct runtime/provider/T3/user evidence.

This handoff does **not** supersede product/spec authority, `Docs/Project/coordination/active-tasks.md`, accepted child packets, exact-SHA repins, Manager review requirements or the machine-wide Unity lock.

## 2. Routing-evidence sources — mandatory

Use these two attached Project sources as the routing-data contract for this run:

1. `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md`
2. `ORCHESTRATION-LEDGER-20261030.md`

Their required evidence discipline is binding for orchestration telemetry:

- capture evidence **during** work; do not reconstruct missing values later;
- keep **OBSERVED FACT**, **ORCHESTRATOR CLAIM** and **RETROSPECTIVE INFERENCE** distinct;
- unknown remains **UNKNOWN**;
- record provider/model/reasoning, task characteristics, decomposition topology, alternatives, reason/rule, live capacity/context evidence, capacity-to-completion, switching cost and `EXPLOIT`/`EXPLORE` for every material routing/switch/retry/decomposition decision;
- record actual outcome, tests/runtime/review evidence, commit/handoff/claim, accepted/rejected/unresolved state, retries/rework/salvage and failure class;
- where observable, record input/output tokens, calls, reasoning effort, wall time, parallel workers and provider failures; never present a proxy as measured energy;
- record learning checkpoints and candidate routing-policy changes as hypotheses with confidence/sample size, not facts or active policy;
- final summary must include attempts, failures/cancellations, capacity failures, switches/salvage, routing decisions, direct vs inferred/unknown capacity decisions, resource observations, review findings, unresolved work and exact next action;
- for game work additionally record worktrees/branches, Unity/editor lanes, serialized Unity operations, rendered/runtime qualification, independently reviewed save/reference/determinism work, and any protected-anchor/baseline change.

The historical 2026-10-30 ledger is an evidence example/prior, not authority to copy stale provider states or old task pins. Use its event vocabulary and provenance discipline, but refresh all live state.

## 3. Current live coordination facts

At publication:

- published integration is exact `6784ab17cac922a16815f4898a30737482101018`;
- #212 MFM-10 browser/static guard prep is completed/closed;
- #213 MFM-10 local residual qualification is **READY**;
- #199 Combined HUD / Focus Option 1 is **REPINNED / READY / QUEUED**, required before Tier-3;
- PARITY-09 and MFM-11 remain blocked until accepted MFM-10 returns `SPECIES GUARD QUALIFIED`;
- MFM-12 waits on accepted MFM-11 on the applicable lineage;
- Scenario One Tier-3 is not released;
- #200 remains the sole primary writer of `Docs/Project/coordination/active-tasks.md`;
- Research Source Consolidation #194 remains **NO ACTION**;
- no Windows build is authorised before the user's personal Scenario One playtest and explicit post-playtest authorisation.

Refresh all of these before execution. A stale copy of this handoff never overrides newer live coordination or direct user/Manager authority.

## 4. Local orchestration start protocol

Before any child work:

1. fetch/refresh origin and verify the live integration ref;
2. read `Docs/Project/coordination/active-tasks.md` and #203;
3. read the exact child claim and its newest handoff/repin;
4. verify branch/worktree ownership and starting HEAD;
5. inspect existing dirty/untracked work and preserve unrelated work — never stash, reset, discard, clean or overwrite it;
6. inspect free space and Editor processes before any Unity task;
7. acquire the authoritative machine-wide Unity lock only immediately before a Unity phase;
8. mark each substantive child claim `[RUNNING]` with actual provider/model, branch, worktree and exact starting HEAD;
9. initialise the routing ledger with DeepSeek and Mimo capacity/context observations and evidence sources;
10. do **not** edit `active-tasks.md` from the local lane.

Claims track execution presence only; they do not grant ownership.

## 5. Work available now

### A. #213 — MFM-10 local species-parity residual qualification

**Purpose:** close the executable residual left by the completed browser/static audit.  
**Difficulty:** Low/Medium — deterministic qualification only; no production implementation authority.  
**Exact candidate:** `6784ab17cac922a16815f4898a30737482101018`.  
**Suggested branch:** `verify/mfm-10-species-guard-v1`.  
**Required source:** `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md`.

Do not repeat browser-capable spec, ancestry, broad source-search or matrix-design work. After fresh local preflight, run only:

```bash
python3 -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/01-prescription-core
python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/02-three-species-density
```

If both pass and there is no contradictory local evidence, **do not run Unity** for MFM-10. Publish:

`Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`

The handoff must combine the browser result and local receipts and may return `SPECIES GUARD QUALIFIED` only if the complete gate is actually satisfied.

A genuine code defect is not repair authority under #213: stop that child, preserve evidence and request/create a separate bounded repair claim subject to Manager authority. A desire to apply Sitka quantitative MFM treatment to Oak/Beech is a spec/product change, not a bug fix.

### B. #199 — Scenario One Combined HUD / Focus Option 1 repair

**Purpose:** complete the approved bounded production layout/reflow repair while preserving accepted Combined, PARITY-07, Rack V2, MFM-08 and MFM-09 behaviour.  
**Difficulty:** Medium/High — cross-screen Unity UI/focus work with protected assertions.  
**Exact fresh start:** `6784ab17cac922a16815f4898a30737482101018`.  
**Branch:** `fix/scenario-one-combined-hud-focus-regression-v1` in a fresh dedicated writable worktree after live preflight.  
**Binding amendment:** `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261101.md`.

Do not use stale production bases `42742a8a`, `ce673714...` or `34e360f8...`. Do not cherry-pick stale production hunks. Preserve MFM-08 `TreeInspectionView` behaviour and integrated MFM-09 teaching/Work Plan behaviour.

Required product boundary:

- restore minimap visibility for ordinary estimate inspection;
- retain accepted PARITY-07 estimate information and central-prompt/detail-column clearance;
- keep protected Combined assertions semantically unchanged;
- repair the exposed 1920 V2 competitor-list clipping;
- preserve Rack geometry/marking/persistence, MFM semantics, save/schema/anchors/reference/ecology/economy;
- do not shorten accepted copy or weaken/rebaseline protected tests merely to obtain green.

Required verification includes the full `combined-v2` and `combined-legacy` gates at 1280×720 and 1920×1080, PARITY-07 rendered/UI clearance, affected minimap/inspection/focus regressions, proportionate Rack/guided checks for touched surfaces, independent implementation review, and clean worktree/Editor/Unity-lock return.

Publish/update:

`Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md`

Expected worker success token remains:

`SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`

Do not integrate the production repair merely because the worker/reviewer is green. Return it for Manager review/integration disposition.

## 6. Decomposition and concurrency authority

DeepSeek decides the efficient topology. The following is explicitly allowed, not required:

- run #213 and #199 in parallel because #213 is offline qualification-only and #199 is a separate production/UI lane, **provided** worktrees/file ownership do not overlap;
- delegate either child or a bounded subtask to Mimo 2.6 Pro when task fit, capacity and independence make it efficient;
- use one worker for implementation and another for independent review where the review must be independent;
- keep DeepSeek as the orchestration/control thread even when Mimo performs substantive work;
- checkpoint/push valid work before provider switches and salvage rather than restart when safe.

Heavy Unity remains serialized machine-wide. #199 may own Unity only after fresh lock preflight/acquisition. #213 should not consume Unity unless contradictory evidence invalidates the browser narrowing and the worker stops for Manager disposition rather than silently expanding scope.

For every concurrency/decomposition choice, record the topology, dependency, expected benefit, ownership separation, capacity evidence, switching/coordination cost and result using the mandatory evidence contract.

## 7. Model/provider routing guidance

### DeepSeek

DeepSeek is the orchestrator and default worker. Use it for bounded implementation/verification, repository/worktree handling, deterministic test execution, evidence classification, straightforward debugging and coordination where capability is sufficient.

### Mimo 2.6 Pro

Mimo 2.6 Pro is explicitly available for this run. The user reports substantial remaining token capacity. Confirm live availability at dispatch and record exact/qualitative capacity evidence honestly.

DeepSeek may use Mimo for:

- a substantial child task when it is the best capacity/quality fit;
- bounded implementation or debugging;
- independent review when it is independent of the implementer;
- parallel non-overlapping analysis/verification;
- salvage/resume if another worker is interrupted and ownership is clean.

Do not use Mimo merely to burn capacity. Record why it was selected over DeepSeek and any known alternative.

### Other providers/models

Existing project policy remains available as fallback: Sonnet 5.5 Medium may be used for genuinely architecture-sensitive/hard work when DeepSeek/Mimo are not the best safe fit; Opus remains a final capability escalation only. Environment, quota, lock, stale state, genuine red tests, authority conflicts or under-specification are **not** model-capability failures.

The orchestrator may adjust routing based on observed outcomes, but must record the decision and may not convert a learning hypothesis into project policy without Manager/user acceptance.

## 8. Live provider/capacity ledger — initialise at run start

Create/continue a table with at least:

| Provider / pool | State | Model/config | Remaining capacity | Reset/window | Evidence source + confidence | Current work | Capacity-to-completion | Switching cost |
|---|---|---|---|---|---|---|---|---|
| DeepSeek | confirm at runtime | actual orchestrator model | observe/UNKNOWN | observe/UNKNOWN | runtime/provider | orchestration | assess | assess |
| Mimo | user reports usable | Mimo 2.6 Pro | **qualitatively high per user; exact UNKNOWN** | UNKNOWN unless observed | user initially, then runtime/provider | unassigned at publication | assess at dispatch | assess |

Add any other pool actually considered or used. Do not omit a material rejected alternative merely because it was not dispatched.

## 9. Routing-event record — minimum fields

For each material dispatch, switch, retry, salvage or decomposition decision capture at decision time:

- timestamp;
- work unit + parent claim;
- evidence class;
- task type/difficulty/risk;
- decomposition topology/dependencies;
- previous provider/model/role;
- selected provider/model/reasoning/role;
- alternatives considered;
- reason/routing rule;
- live capacity/context evidence and source/confidence;
- capacity-to-completion;
- switching/interruption/coordination cost;
- `EXPLOIT` or `EXPLORE`;
- prior outcome influencing the choice;
- branch/worktree/start SHA;
- Unity requirement and live lock state when relevant;
- result/evidence;
- tests/runtime/rendered receipts;
- commit/handoff/claim reference;
- accepted/rejected/unresolved state;
- retries/rework/salvage;
- failure class;
- observable tokens/calls/reasoning/wall time/parallelism/provider failures;
- exact next action.

Use the event style and evidence vocabulary from `ORCHESTRATION-LEDGER-20261030.md` as the pattern, but never copy its old capacity states as current facts.

## 10. Current orchestration evidence outputs

Continue the existing durable local-orchestration evidence rather than creating disconnected telemetry:

- append new events to `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-LEDGER.md`;
- update or append the latest run summary in `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-RESULT.md` without destroying historical evidence;
- publish the specific child handoff for every substantive implementation, verification, review or qualification result.

The routing ledger is execution evidence. It does not itself grant acceptance or override the child handoff.

## 11. Downstream work during the same local run

After finishing a child, refresh live `active-tasks.md`, #203 and relevant claims. Continue automatically into **newly released local work** when #200 has published the required exact-SHA repin/release and ownership is clear.

Expected dependency shape:

1. #213 returns candidate MFM-10 result;
2. #200 performs Manager review; only accepted combined evidence with `SPECIES GUARD QUALIFIED` releases PARITY-09 and MFM-11;
3. run locally released PARITY-09 and/or MFM-11 child work in the most efficient safe order/concurrency allowed by their packets and Unity/file ownership;
4. #200 reviews required returned evidence; accepted MFM-11 allows MFM-12 release;
5. run released MFM-12 local qualification;
6. ensure #199 is completed and returned for Manager review if not already done;
7. only after the required accepted systems converge on one exact candidate may #200 release final Scenario One Tier-3 local qualification;
8. local lane may execute the released Tier-3 packet, but **user personal playtest remains mandatory after Tier-3**;
9. no Windows build until the user personally playtests that same candidate and explicitly authorises the build.

Do not self-release PARITY-09, MFM-11, MFM-12 or Tier-3 from this handoff alone. A local success token is not Manager acceptance. If #200 has not yet released the next child, stop at that Manager boundary with a complete handoff/result rather than inventing authority.

## 12. Hard stops and protected boundaries

Stop the affected child and return evidence for:

- product/spec conflict or desired scope expansion;
- code/spec mismatch outside the child repair authority;
- ownership/file overlap that cannot be resolved locally;
- unsafe integration conflict;
- attempt to weaken/rebaseline protected tests or anchors;
- persistence/schema/reference change without explicit authority;
- required independent review that cannot be obtained;
- missing Manager release for a blocked downstream task;
- user playtest boundary;
- any Windows build before explicit post-playtest authorisation.

Research Source Consolidation Audit **#194 remains NO ACTION**. Do not inspect, review, approve/reject, merge, update, clean up, reorganise or otherwise act on it.

## 13. Final orchestration summary

When this local run reaches a real Manager/user boundary, publish a consolidated result that states:

- which children were attempted/completed/failed/cancelled;
- actual provider/model/config for each;
- all material routing/decomposition decisions and why;
- DeepSeek and Mimo capacity/context evidence, including UNKNOWNs;
- switches, retries, salvage and coordination overhead;
- test/Unity/rendered evidence and exact candidate SHAs;
- worker result vs independent review vs Manager acceptance status;
- worktrees/branches and cleanup state;
- any changed protected anchor/baseline (normally none);
- unresolved work and exact next action;
- learning hypotheses with confidence/sample size;
- explicit confirmation that #194 was untouched and no Windows build was performed.

## 14. Copy/paste dispatch line

`Run Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md under local orchestration claim #203. You are the DeepSeek orchestrator. Execute all currently local-ready work, beginning with #213 MFM-10 residual qualification and #199 Option 1 as ownership/Unity sequencing permits; decompose or parallelise them when safe. Mimo 2.6 Pro is explicitly available and the user reports substantial remaining token capacity, so use it whenever your routing judgement says it is the efficient worker/reviewer, while recording the reason and live capacity evidence. Use ORCHESTRATION-EVIDENCE-APPENDIX-V2.md and ORCHESTRATION-LEDGER-20261030.md as the mandatory routing-telemetry contract: record every material dispatch/switch/decomposition/outcome with OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE kept distinct. Refresh active-tasks.md and exact child authority before every start, but do not edit active-tasks.md. Continue into later local tasks only after #200 explicitly releases them. Preserve all protected contracts, #194 NO ACTION, serialized Unity, independent review requirements and the no-Windows-build boundary. Publish child handoffs plus the updated orchestration ledger/result, then stop only at a genuine Manager/product/user boundary.`
