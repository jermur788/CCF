# T3 Adaptive Model Routing V1 Specification

- **Spec ID:** SPEC-T3-ADAPTIVE-MODEL-ROUTING-V1
- **Status:** DRAFT
- **Owner:** Overall Manager
- **Created:** 2026-10-28
- **Last updated:** 2026-10-28
- **Decision log references:** none — draft for Overall Manager review
- **Milestone:** T3 Fork adaptive model routing design
- **Related evidence:** `Docs/Handoffs/T3-Fork-Adaptive-Model-Routing-20261028/HANDOFF.md`; `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`; `Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md`; `Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md`; `Docs/Research/ModelCapabilityCost/RESEARCH-CORRECTIONS-2026-10-28.md`; `Docs/Handoffs/ModelCapabilityCostResearch-ClaudeReplication-20261028/MANAGER-REVIEW.md`
- **Inspected implementation base:** `jermur788/t3code main @ 02a3d7a3a3129bdc80b283e991fd8bf9347e0f91`
- **Supersedes / superseded by:** none

## Product intent

T3 Fork should learn which **eligible subscription-backed model/provider/account/settings combinations** are effective for different kinds of project work while preserving scarce fixed-cost quota for tasks that need stronger capability.

This is not an API-cost optimizer. The current user portfolio is fixed-cost software subscriptions: one OpenAI subscription at €20/month, two Claude subscriptions at €20/month each, and one OpenCode Go Europe subscription at €10/month, for €70/month / €840/year. The user does not use API billing. V1 MUST treat those subscriptions as finite capacity pools whose availability, limits and resets can vary independently.

The outcome sought is **capability-efficient verified work per scarce subscription capacity**, not “use the cheapest model” and not “use the strongest model by default.”

## Problem

T3 Fork already has durable Project Runtime tasks, Development Profiles, static worker routing, provider/model eligibility checks, execution attempts, handoffs and evidence-backed verification. It does not yet have a durable, normalized learning layer that can answer questions such as:

- How difficult was this task predicted to be before routing?
- Which provider instance, model, options, runtime/interaction settings and tool capabilities actually ran it?
- What subscription capacity was known before and after the run?
- Was an observed reset scheduled, unexpected/bonus, or merely inferred?
- How hard did the task turn out to be?
- Was the work accepted without rework, accepted after rework, rejected, or still unverified?
- On comparable tasks, which eligible model/settings combinations produce accepted results with lower pressure on scarce high-capability pools?
- How confident is a routing recommendation, and what evidence supports it?

Without those records, routing remains largely static and project knowledge about model fit is easily lost in chat history or anecdote.

## Goals

1. Capture durable, explainable execution telemetry around Runtime task attempts without creating a second provider, routing or execution system.
2. Capture privacy-safe subscription/account capacity observations, including provider-native quota windows, reset times and observed reset events when available.
3. Record task difficulty twice: a prediction **before candidate choice** and a realized assessment **after outcomes and verification evidence exist**.
4. Reconcile worker attempts with authoritative accepted-work outcomes rather than treating worker self-report or handoff claims as proof.
5. Produce advisory, evidence-backed model-routing recommendations among already eligible candidates while preserving current static routing precedence and normal T3 eligibility.
6. Support controlled comparisons on genuinely comparable eligible tasks without introducing unsafe duplicate writes or weakening CCF ownership/review/verification rules.
7. Keep all V1 behavior subscription-only, privacy-conscious, auditable, reversible and safe against accidental API/credit/overage spend.
8. Provide sufficient provenance for later analysis of predicted-vs-realized difficulty, model/settings fit, quota use, resets, rework and accepted outcomes.

## Non-goals

- No production implementation is authorized by this draft.
- No API-key routing, API billing, token-price optimization or marginal per-token cost calculation.
- No automatic purchase, credit enablement, overage enablement or paid usage-credit path. In particular, Fable usage credits/overage-like billed paths MUST remain disabled unless a separate explicit product decision authorizes them.
- No replacement of existing T3 provider registries, provider instance configuration, Runtime routing, orchestration, worktree or session internals.
- No bypass of provider/model/project eligibility, permissions or authorization.
- No silent mutation of Development Profile, project or task routing from learned evidence.
- No claim that observed OpenAI bonus resets are guaranteed future entitlement.
- No raw credential, auth-token, payment-card or billing-secret collection.
- No CCF gameplay, Unity, save-system or simulation change.
- No cross-user cloud telemetry service in V1.
- No automatic routing policy adoption in V1. Recommendations remain advisory until accepted through project governance.

## Current architecture findings

The following are implementation facts from the inspected T3 fork and constrain this design.

### Existing routing remains authoritative

`RuntimeRoutingService` resolves the current ordered routing layers and reads live provider state through `RuntimeRoutingProviderView`. The provider view reads the existing `ProviderRegistry`, `ProviderInstanceRegistry` and `ServerSettings`; Runtime deliberately does not maintain a second provider catalog.

Current routing precedence remains:

1. explicit one-run override;
2. task routing;
3. project routing;
4. Development Profile routing;
5. ordinary T3 behavior.

Adaptive routing MUST NOT replace or reinterpret this precedence.

### Existing launch receipt already captures important provenance

`RuntimeTaskExecutionService` resolves the static route, rechecks provider/model/project eligibility, resolves a model selection and launches through the existing orchestration path. `RuntimeExecutionAttempt.receipt` already persists:

- exact selected provider instance and model;
- provider option selections;
- static route resolution/provenance;
- runtime mode;
- interaction mode;
- root/shared checkout versus worktree launch strategy;
- the exact Runtime task packet/task revision sent to the worker.

Adaptive telemetry SHOULD reference or extend this durable execution record rather than duplicating it in a disconnected log.

### Provider options are generic

`ModelSelection.options` uses provider-defined option selections. Reasoning/effort settings therefore need not be hard-coded to one provider. The telemetry contract MUST preserve exact option IDs/values used at launch and MAY additionally normalize well-known concepts such as reasoning effort for analysis.

### Accepted work has stronger sources than worker claims

Project Runtime tasks persist lifecycle, acceptance criteria, result, evidence-backed verification milestones and verification evidence. Handoffs are continuation context and can identify provider/model/thread provenance, but they are not proof. Adaptive outcome labels MUST be reconciled from authoritative task/review/integration evidence, not from a worker saying “done.”

### Quota/reset telemetry is not currently normalized in the inspected Runtime path

No normalized quota/reset contract was found in the inspected Runtime routing/execution contracts. V1 therefore MUST introduce an optional read-only capacity-observation boundary. Missing provider signals remain `unknown`; implementation MUST NOT fabricate quota values from guesses, token estimates or pricing tables.

## Core architecture

Adaptive Model Routing V1 is an **observation + recommendation layer beside the existing Runtime routing and execution services**.

Conceptual flow:

```text
Runtime task / role
      |
      v
Predicted difficulty snapshot  (before recommendation)
      |
      v
Existing static routing + live provider eligibility
      |
      +----> eligible candidate set + route provenance
      |
      v
Adaptive advisory recommender
      |       uses historical accepted outcomes + capacity observations
      v
Recommendation + explanation
      |
      |  manager/user may accept, override or ignore
      v
Existing RuntimeTaskExecutionService / orchestration launch
      |
      v
Durable execution observation
      |
      +----> before/after capacity snapshots when available
      |
      v
Later outcome reconciliation from authoritative task/review/integration state
      |
      v
Realized difficulty + accepted-work outcome + learning aggregates
```

### Architectural invariants

- **ARCH-1 — One routing system.** Existing Runtime routing defines policy layers and candidate eligibility. The adaptive layer MUST NOT become a parallel router with independent provider authorization.
- **ARCH-2 — One execution path.** All launches continue through the existing Runtime execution/orchestration path.
- **ARCH-3 — No second provider catalog.** Model/provider availability continues to come from existing provider registries/settings.
- **ARCH-4 — Durable observations.** Adaptive observations and recommendation receipts persist independently of chat/provider sessions.
- **ARCH-5 — Missing data is explicit.** Unknown quota, account, permission or tool data is stored as unknown/unavailable, never inferred as fact.
- **ARCH-6 — Advisory V1.** V1 may recommend/rank already eligible candidates but MUST NOT silently rewrite saved routing or launch a non-static candidate without explicit caller/manager selection.
- **ARCH-7 — Domain-service seam.** New state and calculations belong in reusable domain services/contracts usable by RPC/MCP/UI/future clients, not only model-facing handlers.

## Persistence boundaries

V1 SHOULD use the existing environment database and append-only fork migration mechanism.

Three logical record families are required:

1. **Project/task execution observations** — project-scoped; keyed to existing Runtime task and execution-attempt IDs.
2. **Subscription capacity observations** — environment/provider-capacity-pool scoped because one subscription/account can serve multiple projects.
3. **Derived recommendations/aggregates/experiment assignments** — durable records that reference the raw observations and the governing calculation version.

Raw observations SHOULD be append-only or immutable after creation except for explicit outcome reconciliation fields with compare-and-set/revision protection. Derived aggregates may be rebuilt from raw observations.

The design MUST NOT copy provider credentials, full conversations or source bodies into adaptive tables.

## Identity model

### Provider/model identity

Every attempt observation MUST preserve:

- provider driver kind;
- provider instance ID;
- exact model slug selected by T3;
- exact provider option selections used;
- optional advertised model/catalog fingerprint or version metadata when the provider supplies it;
- Runtime software/schema/scoring version relevant to the observation.

Model aliases MUST be resolved consistently with normal T3 behavior. Historical observations MUST retain the identity actually used at the time rather than being rewritten when aliases/defaults change.

### Account / seat / capacity-pool identity

Provider instance ID is a useful existing routing identity but is not assumed to be a complete billing/account identity. V1 MUST support a separate opaque `capacityPoolId`/account-seat alias local to the environment.

Requirements:

- raw email, login name, billing account number and payment identifiers MUST NOT be required;
- if a provider exposes a stable account identifier, it MUST be transformed into or mapped to a local opaque alias before adaptive persistence unless the user explicitly opts into storing the human-readable value;
- when account identity cannot be safely observed, the provider instance itself MAY be treated as the finest known pool identity with identity quality marked accordingly;
- multiple accounts/seats for the same provider MUST remain distinguishable when T3 already exposes distinct provider instances or the provider adapter can safely distinguish pools.

## Execution context telemetry

For each observed Runtime task attempt, capture or reference the following as available:

- task ID, task revision and role;
- execution attempt ID, thread/run reference and timestamps;
- provider driver, provider instance, capacity-pool alias and exact model;
- exact model/provider option selections, including reasoning/effort settings where represented;
- normalized reasoning/effort value when a safe provider mapping exists, plus the original option data;
- Runtime mode and interaction mode;
- permission/approval mode or an explicit `unknown` when the current launch receipt does not expose it;
- workspace strategy (shared/root or isolated worktree and base information already safe to retain);
- static route source/winning layer, candidate/rejection provenance and route revision where available;
- tool/MCP/connector capability snapshot;
- predicted difficulty snapshot ID;
- before/after capacity snapshot IDs where available;
- execution terminal state, retries/reconciliation/cancellation facts and timing;
- experiment assignment ID when applicable.

### Tool/MCP/connector privacy

Tool availability can materially affect task performance but connector names may reveal sensitive workspace context. V1 MUST support:

- a normalized capability category list where safe (for example repository write, shell, browser, MCP available);
- a stable local capability fingerprint for comparison;
- optional raw tool/connector IDs only when explicitly configured as safe;
- no connector secrets, tokens, arguments or retrieved private content in telemetry.

## Predicted difficulty

A difficulty prediction MUST be persisted before adaptive candidate recommendation/selection to avoid outcome leakage.

V1 difficulty is multidimensional and versioned. At minimum record:

- **ambiguity** — how under-specified or decision-heavy the task is;
- **scope/breadth** — expected affected systems/files/surfaces;
- **reasoning complexity** — depth of reasoning/debugging/design expected;
- **integration/tool complexity** — number/complexity of external systems, tools or coordination boundaries;
- **risk** — concurrency, security, data-loss, migration, architecture or other consequence risk;
- **verification burden** — expected testing/review/runtime evidence cost.

Each dimension SHOULD use a bounded ordinal scale (for example 0–4). The predictor derives a human-readable band such as `routine`, `moderate`, `hard`, `critical`, plus confidence and prediction-version metadata.

Prediction inputs MUST be limited to information available before routing/launch: task fields, acceptance criteria, role, repository/project metadata and explicitly available source metadata. Post-run evidence MUST NOT be used to backfill the original prediction.

A manager/user MAY override a prediction. The system MUST preserve both original prediction and override provenance rather than silently replacing the model output.

## Realized difficulty

Realized difficulty is derived after sufficient execution/outcome evidence exists. It is not simply wall-clock duration or token use.

Inputs SHOULD include, where available:

- number of attempts/retries;
- cancellations, interruptions or reconciliation-required states;
- worker turns/tool activity summaries if safely observable;
- changed-file/system breadth when available through existing trusted project tooling;
- scope expansion or blocker classifications;
- independent review findings;
- rework count and whether a different provider/model was needed;
- verification effort and failures;
- integration conflicts;
- accepted/rejected final outcome.

The raw signals and the versioned realized-difficulty derivation MUST both be retained so later scoring revisions do not rewrite history invisibly.

## Accepted-work outcomes

Adaptive learning MUST distinguish execution completion from accepted work.

Minimum outcome states:

- `unresolved` — no authoritative acceptance outcome yet;
- `accepted_without_rework`;
- `accepted_after_rework`;
- `rejected_or_superseded`;
- `failed`;
- `cancelled`.

Outcome evidence MUST include source/provenance and timestamp. Generic T3 may reconcile from Runtime task lifecycle, result, verification milestone/evidence and later integration state. For CCF work specifically:

- CCF canonical project/task state and Overall Manager verification/integration decisions remain authoritative;
- a worker handoff or terminal provider run MUST NOT mark work accepted;
- learned recommendations MUST NOT advance CCF verification milestones, close claims or integrate code;
- if T3 and CCF evidence conflict, CCF implementation truth and accepted project state govern the CCF outcome label and the conflict is recorded for reconciliation.

Outcome reconciliation MAY occur long after the execution attempt. It MUST be idempotent and revision-aware.

## Subscription capacity / quota observations

### Capacity model

A subscription is modeled as one or more **capacity pools**, not as an API wallet.

For each observed capacity window, retain as available:

- provider driver and capacity-pool alias;
- provider-native window identifier/type if supplied;
- observed-at timestamp;
- usage/remaining/limit values exactly as exposed by the provider, with unit label;
- provider-reported reset time when available;
- observation source and quality (`provider_reported`, `t3_observed`, `user_entered`, `inferred`);
- whether values are exact, approximate or unknown;
- optional subscription-plan label only if user-supplied or safely exposed;
- no monetary API balance unless a future product decision explicitly changes the subscription-only constraint.

Providers can expose multiple windows (for example short rolling limits and longer weekly limits). The schema MUST support multiple simultaneous windows rather than forcing one universal percentage.

### Reset events

The system MUST be able to record reset events separately from ordinary snapshots:

- `scheduled_reset` — consistent with a provider-reported reset boundary;
- `observed_bonus_reset` — capacity increased/reset outside the expected schedule and is directly observed;
- `manual_correction` — user/operator corrected an observation;
- `inferred_reset` — discontinuity suggests a reset but evidence is insufficient to classify it as provider-reported.

OpenAI’s repeated additional seven-day-limit resets are a user-observed phenomenon and may be stored as `observed_bonus_reset` evidence when directly observed. They MUST NOT be extrapolated as guaranteed future capacity. Absence of comparable Claude resets is also observational, not proof that such behavior can never occur.

### Missing telemetry

If a provider does not expose a reliable quota/reset signal, adaptive routing MUST use `unknown` and lower confidence. It MUST NOT estimate quota from conversational token counts, published API pricing, task duration or benchmark throughput.

### Fixed subscription fees

The portfolio’s fixed monthly fees MAY be recorded for human budgeting/reporting. V1 MUST NOT allocate those fees into synthetic per-token or per-task marginal costs for routing decisions.

## Read-only capacity observation boundary

Because quota telemetry is not normalized in the inspected Runtime path, V1 SHOULD introduce an optional provider-facing **capacity observation adapter** that:

- is read-only;
- is associated with existing provider instances/drivers rather than a new provider registry;
- returns zero or more typed capacity-window observations and optional pool identity aliases;
- can explicitly report unsupported/unavailable;
- cannot purchase credits, enable billing, change plan, change region or mutate provider settings;
- cannot expose credentials to Runtime telemetry;
- is callable independently of execution so managers can inspect capacity state without launching work.

Provider-specific adapters are optional. No adapter means unknown capacity, not an implementation failure.

## Adaptive recommendation

### Candidate boundary

The adaptive recommender receives only candidates that have already survived the existing static route/eligibility process, or it evaluates alternative candidates strictly through the existing resolver/eligibility service. It MUST NOT resurrect a provider/model rejected by normal T3 eligibility.

### Advisory output

A V1 recommendation MUST include:

- recommended candidate and settings, or `no_recommendation`;
- predicted difficulty and confidence;
- static route source and eligible candidate set considered;
- historical comparable-task sample count;
- accepted-success/rework/failure summary by candidate;
- relevant capacity/quota state and reset proximity, including unknowns;
- reservation pressure for scarce high-capability pools;
- recommendation confidence;
- scoring/calculation version;
- human-readable reasons and uncertainty notes.

A recommendation is **not authorization**. Manual one-run choice remains highest authority, and saved route policy is not rewritten by recommendation generation.

### Objective function

The V1 scoring objective SHOULD balance:

1. predicted capability fit for the task difficulty/role;
2. accepted-work success on comparable tasks;
3. rework/review-failure penalty;
4. current known capacity remaining and reset timing;
5. preservation of scarce stronger-model capacity for harder predicted tasks;
6. provider/model availability and recent operational reliability;
7. confidence/sample size and model-version drift;
8. fair utilization across equivalent subscription pools without exhausting all pools simultaneously.

The scorer MUST NOT use API token prices as a hidden proxy for quality or cost.

### Cold start and priors

When evidence is sparse, recommendations MAY use the canonical research as documented priors, but MUST label them as priors rather than project evidence. Small samples MUST be shrunk toward prior/default behavior or withheld behind a minimum-evidence threshold.

Model/provider version changes MUST decay or reset the relevance of historical evidence rather than treating model slugs as timeless capabilities.

## Comparable-task definition

Experiments and learned comparisons require a durable comparability key/version derived before assignment. Relevant dimensions include:

- Runtime worker role;
- predicted difficulty band/dimensions;
- task category or feature family;
- repository/project context class;
- required tool-capability fingerprint;
- isolation/workspace strategy;
- verification burden/risk class.

The exact bucket algorithm MUST be versioned. Similar titles alone are insufficient.

## Controlled comparable-task experimentation

V1 experimentation exists to reduce uncertainty, not to maximize novelty.

### Safe default design

Prefer **prospective assignment across future comparable tasks**: when two or more candidates are already eligible and evidence is weak, a manager-enabled experiment may assign different comparable tasks to different candidates over time.

Do not create duplicate concurrent implementation writes merely to compare models.

### Optional duplicated experiments

A duplicated run MAY be allowed only for explicitly safe read-only/research/test work when:

- the task has no side effects or overlapping write lane;
- duplication is explicitly enabled by the manager/user;
- normal provider eligibility holds for each candidate;
- the experiment does not consume scarce quota that the manager has reserved for higher-priority work;
- results are compared against the same acceptance/evaluation rubric.

### Exclusions

Automatic experimentation MUST exclude, unless explicitly authorized:

- security-sensitive tasks;
- destructive/migration work;
- difficult concurrency/atomicity work;
- integration/conflict-resolution tasks;
- urgent recovery tasks;
- tasks with unique/non-repeatable external effects;
- tasks whose CCF ownership lane would be violated by a second writer.

### Assignment integrity

Experiment assignment MUST be durable and recorded before execution. Capture candidate set, assignment method, propensity/probability when randomized, comparability version, capacity snapshot and exclusion checks. Do not alter objective, acceptance criteria or verification plan to favor one candidate.

Experiment outcomes are judged by authoritative accepted-work and resource/capacity evidence, not worker self-rating.

## Billing safeguards

- **BILL-1:** V1 MUST operate only with already-authorized subscription-backed provider paths.
- **BILL-2:** V1 MUST NOT add, request or store API keys for adaptive routing.
- **BILL-3:** V1 MUST NOT enable provider usage credits, overage, pay-as-you-go, API billing, auto-recharge or purchases.
- **BILL-4:** Fable usage credits/overage-like paths remain disabled unless a separate explicit product decision changes this requirement.
- **BILL-5:** A provider adapter that cannot prove an observation is read-only MUST NOT be used for quota telemetry.
- **BILL-6:** Any future request to relax these safeguards is a PRODUCT DECISION and requires a new governing spec revision.

## Privacy and security

- Adaptive raw telemetry remains local to the T3 environment database by default.
- No credentials, auth tokens, payment information or raw secret environment variables are persisted.
- Human account identifiers are not required; opaque local pool aliases are preferred.
- Logs and error messages MUST redact sensitive account identifiers and provider payloads.
- Tool/connector telemetry follows the capability/fingerprint rules above.
- Export/sharing of adaptive telemetry is opt-in and outside automatic V1 behavior.
- Retention/cleanup MUST preserve the ability to audit recommendations while allowing the user to delete adaptive telemetry without deleting core Runtime task truth.
- Deleting adaptive data MUST NOT rewrite task, handoff, execution or CCF verification records.

## CCF governance and authority

For CCF use:

1. Claim/ownership rules remain defined by CCF coordination state.
2. The adaptive recommender cannot claim a task, create an overlapping writer or change ownership.
3. Worker completion/handoff cannot close a CCF claim.
4. Independent review and verification gates remain mandatory where CCF requires them.
5. CCF canonical project/task/integration state remains the accepted outcome authority for CCF work.
6. A routing recommendation is supporting evidence only until the Overall Manager accepts or encodes it in governing routing policy.
7. Controlled experiments must respect one-writer lanes and may not weaken verification to obtain cleaner metrics.

## Explainability and operator surfaces

Future UI/API surfaces MUST distinguish:

- **Static route:** what current saved routing/normal T3 behavior says.
- **Adaptive recommendation:** what the evidence-driven layer suggests among eligible choices.
- **Actual launch:** what provider/model/settings were actually used.
- **Outcome:** what authoritative verification later established.

The operator should be able to answer “why this model?” from one recommendation receipt. Required explanation elements include predicted difficulty, route/eligibility, capacity state, historical comparable evidence, confidence and any reservation/uncertainty reason.

Unknown data MUST be visible as unknown rather than hidden behind a confident recommendation.

## Failure and degradation behavior

- If adaptive storage/service is unavailable, static routing and normal Runtime execution continue unchanged unless a future explicitly accepted policy says otherwise.
- If capacity telemetry is unavailable, recommendation confidence drops; execution is not blocked solely for missing quota telemetry.
- If learning data is corrupt or schema-incompatible, fail closed for adaptive recommendation while preserving core Runtime operation.
- If recommendation and static/manual choice conflict, the manual/static governing choice wins in V1 and the disagreement may be logged as an advisory comparison.
- If a model version drifts materially, historical evidence is retained but down-weighted or excluded according to the scoring version.

## Acceptance criteria

- **AC-1:** The design can persist predicted difficulty before recommendation and realized difficulty only after run/outcome evidence, with immutable provenance.
- **AC-2:** Exact provider instance, model, provider options, runtime/interaction/workspace settings and available permission/tool context are attributable to each observed attempt.
- **AC-3:** Account/capacity-pool identity is distinguishable where available without requiring raw account emails, billing identifiers or credentials.
- **AC-4:** Provider-native capacity windows and reset events can be recorded, including scheduled, observed bonus and inferred reset classifications; missing telemetry remains explicit unknown.
- **AC-5:** OpenAI bonus-reset observations cannot be interpreted by the system as guaranteed entitlement.
- **AC-6:** Worker self-report or terminal execution alone cannot create an accepted outcome; CCF accepted outcomes follow CCF authority.
- **AC-7:** The recommender considers only normally eligible candidates and cannot bypass existing routing/provider/project authorization.
- **AC-8:** V1 recommendations are advisory and do not silently mutate saved task/project/profile routing.
- **AC-9:** Controlled experiments preserve objective/acceptance criteria, one-writer CCF ownership and billing/privacy safeguards.
- **AC-10:** No API billing, API-key, usage-credit, overage, auto-recharge or purchase path is introduced.
- **AC-11:** Raw observations survive restart/reload and derived aggregates can be rebuilt from durable evidence.
- **AC-12:** Recommendation output is explainable, versioned and exposes sample size/confidence/unknown data.
- **AC-13:** Adaptive failure does not corrupt or ambiguously change existing Runtime routing/execution truth.
- **AC-14:** Multi-account same-provider observations remain attributable to the correct privacy-safe capacity pool when T3/provider evidence makes that distinction available.
- **AC-15:** Model/version drift is retained in provenance and affects evidence relevance rather than silently pooling incompatible observations.

## Required verification for later implementation

Implementation is not accepted until tests cover at least:

- migration installation, restart/reload and malformed-history behavior;
- exact attempt/route/model/options provenance;
- provider unavailable/disabled/ineligible cases;
- static fallback semantics unchanged;
- no eligibility/permission bypass;
- multi-account same-provider identity;
- quota unavailable/unknown;
- multiple simultaneous quota windows;
- provider-reported reset and unexpected/bonus reset classification;
- no fabricated quota from API/token pricing;
- predicted-before-recommendation ordering;
- realized-after-evidence ordering;
- authoritative accepted-outcome reconciliation and later correction;
- worker handoff/self-report not treated as verification proof;
- rework and review-failure attribution;
- scoring confidence/minimum-sample behavior;
- model/version drift;
- controlled experiment assignment durability, comparability and exclusions;
- concurrent/CAS writes and idempotent reconciliation;
- privacy/redaction and telemetry deletion boundaries;
- explicit tests proving no API key, credit, overage, purchase or billing mutation path;
- cross-provider continuation and restart persistence;
- UI/rendered inspection for explanation surfaces when UI is introduced.

## Stop conditions

Return to the Overall Manager/spec instead of assuming a solution if implementation would require:

- automatic paid/API/credit/overage use;
- changing existing routing precedence or authorization semantics;
- making adaptive recommendations automatically authoritative;
- storing raw sensitive account/billing identity to make the feature work;
- introducing a second provider registry/catalog;
- allowing experiments to violate a single-writer lane or duplicate side effects;
- changing CCF ownership, review, integration or verification authority;
- changing provider region/privacy settings;
- a material cross-project learning/privacy policy beyond the project-local default;
- a material new data-export/cloud-telemetry policy.

## Open product decisions for Overall Manager review

These are deliberately left visible rather than silently fixed by implementation:

1. **Cross-project learning scope.** Draft recommendation: raw execution observations remain project-scoped; capacity is environment/pool-scoped; adaptive aggregates default to project-scoped evidence, with broader environment-wide learning requiring explicit opt-in or a later decision.
2. **Automatic recommendation application.** Draft V1 is advisory-only. A later V2 could allow an explicitly enabled automatic selector after sufficient evidence and independent qualification.
3. **Retention defaults.** Exact raw-observation retention period/roll-up policy should be set during implementation planning after estimating record volume; security/privacy invariants above are fixed regardless.
4. **Human-readable account labels.** Draft default is opaque local aliases, with optional user-defined display labels that do not expose provider credentials or billing identifiers.
