# T3 Adaptive Model Routing V1 Implementation Plan

- **Spec ID:** SPEC-T3-ADAPTIVE-MODEL-ROUTING-V1
- **Spec status:** DRAFT
- **Plan owner:** Overall Manager
- **Created:** 2026-10-28
- **Last updated:** 2026-10-28

No production implementation is authorized while the governing spec remains `DRAFT`.

## Architecture / current-state findings

### Inspected T3 Fork base

Architecture reconnaissance used `jermur788/t3code main @ 02a3d7a3a3129bdc80b283e991fd8bf9347e0f91`.

Verified seams:

- `packages/contracts/src/projectRuntime.ts` — durable task lifecycle, task routing, result, handoffs and evidence-backed verification.
- `packages/contracts/src/runtimeRouting.ts` — role routes, candidates, fallback, provider instance/driver/model/options, isolation, runtime mode and interaction mode.
- `packages/contracts/src/runtimeExecution.ts` — durable execution attempt and routing receipt with exact `ModelSelection`, route resolution, runtime/interaction mode and workspace strategy.
- `packages/contracts/src/modelSelection.ts` / `model.ts` — provider instance/model identity and generic provider option selections suitable for preserving reasoning/effort settings without provider-specific schema forks.
- `apps/server/src/runtimeRouting/RuntimeRoutingService.ts` — static route resolution and project/profile/task policy reads.
- `apps/server/src/runtimeRouting/RuntimeRoutingProviderView.ts` — read-only facade over the existing Provider Registry, Provider Instance Registry and Server Settings; Runtime owns no provider catalog.
- `apps/server/src/projectRuntime/RuntimeTaskExecutionService.ts` — final route resolution, provider/model/project eligibility recheck, launch-plan creation and existing orchestration launch path.

No normalized quota/reset signal exists in the inspected Runtime routing/execution contracts. Capacity telemetry therefore needs a new optional read-only boundary; absent provider signals must remain unknown.

### Consequences

1. Adaptive routing must be an additive Runtime domain service, not a replacement resolver.
2. Existing execution attempts are the natural anchor for per-run telemetry.
3. Capacity pools cannot be modeled only as project state because the same subscription/provider account can serve many projects.
4. Accepted outcomes should reconcile from existing task verification/integration truth, not create a competing task-status system.
5. Provider options must remain generic; normalized fields are analytic projections, not replacements for provider-native option data.

## Proposed technical approach

### Data domains

Introduce three related but distinct durable domains.

#### A. Execution observation domain — project scoped

Anchor one observation to a Runtime execution attempt. Store only data not already safely reconstructable from the immutable execution receipt, plus stable references/fingerprints to the receipt and task revision.

Candidate logical records:

- `AdaptiveDifficultyPrediction`
- `AdaptiveExecutionObservation`
- `AdaptiveOutcomeReconciliation`
- `AdaptiveRealizedDifficulty`
- `AdaptiveRecommendationReceipt`
- `AdaptiveExperimentAssignment`

Avoid denormalized copies of whole task packets, conversations or source bodies.

#### B. Subscription capacity domain — environment/pool scoped

Introduce a privacy-safe local `capacityPoolId` and append-only capacity snapshots/reset events. This domain is environment-scoped because subscription capacity is shared outside any one project.

Candidate logical records:

- `AdaptiveCapacityPool`
- `AdaptiveCapacitySnapshot`
- `AdaptiveCapacityWindow`
- `AdaptiveCapacityResetEvent`

The implementation must support unsupported/unknown provider telemetry as a normal state.

#### C. Derived learning domain

Build aggregates/recommendations from the raw domains. Derived state should be rebuildable and versioned.

Candidate logical records:

- comparison bucket/version;
- candidate aggregate statistics;
- scoring version;
- recommendation/explanation receipt.

Do not make derived scores the only copy of raw evidence.

### Capacity observation adapters

Add a small provider-facing read-only interface close to existing provider state, for example conceptually `ProviderCapacityObservation` / `ProviderCapacityObserver`.

Rules:

- registered through existing provider/provider-instance ownership, not a new registry;
- optional per driver/instance;
- no mutation methods;
- no API-key acquisition;
- explicit `unsupported` / `unavailable` result;
- values retained with provider-native units and source quality;
- adapters may emit opaque local account/pool identity material but never credentials.

Do not scrape or infer values if no reliable provider source exists.

### Difficulty pipeline

1. Build a deterministic/versioned feature snapshot from the saved Runtime task and safe repository/project metadata.
2. Persist the prediction before asking the recommender to rank candidates.
3. Permit explicit manager/user override while preserving original prediction and override provenance.
4. After terminal attempts plus review/verification evidence, compute realized difficulty from raw observed signals.
5. Keep prediction and realization algorithms versioned independently.

The initial predictor may be rules-based. A learned predictor is not required to ship V1; the important V1 property is durable labels and evaluation data.

### Outcome reconciliation pipeline

Create a reconciliation service that reads authoritative task/result/verification state and updates/creates an outcome observation idempotently.

For generic projects, derive only what Runtime evidence supports. For CCF, the manager/integration authority remains canonical; CCF-specific accepted outcomes may be supplied through the project’s normal Runtime/manager workflow or a narrow explicit reconciliation input.

Never infer `accepted_without_rework` solely from terminal execution status.

### Recommendation pipeline

V1 recommendation should be invoked explicitly by a manager/UI/RPC/MCP caller.

1. Resolve current static routing/eligibility using the existing service.
2. Snapshot capacity observations for relevant eligible pools where available.
3. Read the precomputed difficulty prediction.
4. Load comparable historical accepted outcomes and realized difficulty.
5. Score candidates using a versioned policy.
6. Persist a recommendation receipt including considered candidates, static route result, evidence counts, capacity state, uncertainty and explanation.
7. Return recommendation without modifying static routing.
8. If the caller chooses a recommendation, launch still flows through existing execution service and final eligibility checks.

The exact integration seam between an accepted advisory recommendation and an execution request must preserve the current “launch is durably recoverable or no execution begins” invariant. Do not create a prelaunch path that can launch outside `RuntimeTaskExecutionService`.

### Learning/statistics approach

Start conservatively:

- predefined research priors by capability band/role are metadata, not accepted project truth;
- use minimum sample gates;
- report raw counts and confidence intervals/uncertainty rather than a single opaque score;
- shrink small candidate samples toward prior/static behavior;
- split or down-weight historical evidence on material model/version change;
- avoid cross-project aggregation by default until Overall Manager accepts a broader privacy scope;
- treat unknown quota as unknown rather than a zero/neutral numerical value.

A simple transparent scorer is preferable to a complex learned model in V1 because explainability and data quality matter more than early sophistication.

### Controlled experiments

Implement experiments after observational telemetry is proven.

Preferred experiment design is prospective assignment across naturally occurring comparable tasks. Add duplicate-run support only for explicitly safe read-only/research/test work.

Every experiment must persist:

- hypothesis;
- comparability key/version;
- candidate set;
- eligibility evidence;
- assignment method and probability/propensity if randomized;
- capacity state at assignment;
- task acceptance rubric reference;
- exclusion checks;
- actual outcome linkage.

Experiment tooling must not own task acceptance or CCF claim state.

## Integration boundaries

### May touch after spec approval

- contracts for adaptive telemetry/recommendations/capacity observations;
- Runtime server domain services;
- environment-database fork migrations;
- `RuntimeTaskExecutionService` only through narrow instrumentation/integration seams;
- `RuntimeRoutingService` through read-only resolution reuse, not precedence changes;
- provider drivers/instance services only for optional read-only capacity observation adapters;
- RPC/MCP/UI surfaces for reading telemetry and asking for recommendations;
- tests/docs.

### Must not touch for semantic ownership

- provider credentials/auth ownership;
- provider registry ownership;
- existing static routing precedence;
- normal provider/model/project eligibility semantics;
- orchestration atomic launch invariant;
- Git/worktree/session ownership;
- CCF task ownership, claim state, review authority or verification authority;
- billing/credit/purchase settings except tests that prove they remain untouched.

## Expected affected T3 systems / files

Exact file names may be refined after implementation-base refresh, but anticipated seams are:

- `packages/contracts/src/` — new adaptive-routing contracts and exports.
- `apps/server/src/projectRuntime/` — execution observation/outcome reconciliation integration and domain service.
- `apps/server/src/runtimeRouting/` — read-only recommendation composition around the existing resolver.
- provider registry/driver area — optional read-only capacity adapter interface and provider implementations.
- fork migration registry / Environment DB migration files.
- RPC/MCP registration for manager-facing reads/recommendations.
- web settings/Runtime task surfaces only in the later UI milestone.
- docs/internals and operator docs where behavior becomes user-facing.

## Upstream / rebase hotspots

Highest expected upstream overlap:

1. provider registry/provider-driver interfaces;
2. server settings/provider-instance structures;
3. Runtime task execution service;
4. contracts exports/RPC registration;
5. web Runtime task/settings UI.

Mitigation:

- keep adaptive contracts/services isolated;
- prefer event/read seams to copying provider/orchestrator internals;
- minimize edits to upstream provider drivers;
- add capacity observation as optional capability rather than required provider API;
- keep derived learning logic independent from static routing resolver implementation;
- re-inspect upstream immediately before each substantial implementation milestone.

## Ownership / concurrency considerations

- One writer for contract/migration lane during schema work.
- One writer for execution-service instrumentation lane.
- Provider-specific capacity adapters can proceed in parallel only after the adapter contract is fixed and each worker owns a distinct provider lane.
- UI may proceed after read APIs are stable.
- Do not allow parallel writers in `RuntimeTaskExecutionService` or migration manifests.
- Every substantial worker must leave a durable handoff with base/head, files, verification and exact next action.

## Migration / persistence implications

Adaptive records are new fork-owned environment-database state.

Requirements:

- append-only fork migration(s), registered in the existing fail-closed migration sequence;
- migration IDs/version recorded in the governing plan before implementation;
- no rewrite of existing Runtime task/execution records merely to add telemetry;
- existing attempts without adaptive observations remain valid and appear as historical `not_observed`/unknown, not migration errors;
- restart/reload must preserve raw observations, recommendation receipts and experiment assignments;
- rollback is restore-from-backup/forward-fix consistent with existing fork migration policy;
- deletion/retention of adaptive telemetry must not delete authoritative Runtime/CCF state.

## Verification strategy

### Implementation correctness

- contract decoding/bounds tests;
- migration fresh/adopted/malformed-history tests;
- domain-service unit/integration tests;
- execution instrumentation and idempotency tests;
- provider-capacity adapter tests including unsupported/unknown;
- recommendation deterministic fixture tests;
- experiment assignment tests;
- privacy/billing negative tests.

### Spec conformance

Maintain `checklists/requirements.md` as traceability matrix. Each requirement must point to a concrete test/evidence artifact before V1 is verified.

### Verification tiers

- **STATIC PASS:** typecheck/lint/contracts/schema/reasoning fixtures.
- **RUNTIME PASS:** real server DB/restart and provider observation/execution paths.
- **REVIEW PASS:** independent architecture/security/privacy review plus repairs.
- **INTEGRATED:** accepted work on intended T3 fork integration base.
- **VERIFIED:** cross-provider qualification, restart proof, billing/privacy safeguards and requirement checklist all pass.

At least OpenAI, Claude and OpenCode should be exercised where their subscription/provider interfaces make the required observations practical. Unsupported quota telemetry is an acceptable provider result if correctly represented as unknown.

## Risks

1. **Provider telemetry instability.** Quota surfaces may be unofficial or change. Mitigation: optional adapters, source-quality fields, no guessed values.
2. **Account privacy.** Account identifiers can expose email/billing identity. Mitigation: local opaque aliases and redaction by default.
3. **False causal learning.** Task mix differs by model. Mitigation: predicted difficulty, comparability buckets, controlled assignment, sample/confidence reporting.
4. **Self-fulfilling routing.** Strong models receive only hard tasks, weaker models only easy work. Mitigation: explicit comparable-task experiments when safe and exploration limits.
5. **Quota over-conservation.** Saving strong-model quota could reduce throughput. Mitigation: objective uses accepted success plus capacity, not quota alone; manual override remains available.
6. **Bonus-reset overfitting.** Observed OpenAI extra resets could be treated as expected. Mitigation: stochastic event class and conservative forecast confidence.
7. **Billing escape hatch.** Provider products may expose paid credits. Mitigation: hard no-mutation/no-credit requirements and negative tests.
8. **Schema bloat.** Telemetry could duplicate large Runtime payloads. Mitigation: references/fingerprints and raw-observation separation.
9. **Upstream rebase cost.** Provider/execution seams are active upstream areas. Mitigation: isolated optional interfaces and reusable services.
10. **CCF authority drift.** T3 telemetry might be mistaken for accepted project truth. Mitigation: explicit outcome authority/provenance and no claim/integration write path.

## Implementation sequence

### ARM-010 — Contracts, persistence and provenance skeleton

Define adaptive IDs/contracts, raw observation schemas, capacity-pool aliases, recommendation receipts, versioning and fork migration. No scorer beyond fixtures.

**Gate:** STATIC PASS + migration review before any execution instrumentation.

### ARM-020 — Execution context telemetry capture

Attach observation creation to the existing Runtime attempt/launch lifecycle. Capture model/options/route/settings/workspace and safe capability/permission context. Prove no launch semantic change.

**Gate:** RUNTIME PASS for launch/restart/reconcile with telemetry enabled and disabled.

### ARM-030 — Subscription capacity observation

Add optional read-only capacity adapter contract and initial provider adapters only where reliable subscription signals are available. Persist snapshots/reset events and privacy-safe pool identity.

**Gate:** billing/privacy review; explicit proof that adapters expose no mutation/purchase/credit path.

### ARM-040 — Difficulty and authoritative outcome reconciliation

Implement versioned predicted difficulty, override provenance, realized difficulty derivation and idempotent outcome reconciliation from Runtime/CCF-authoritative evidence.

**Gate:** tests proving prediction precedes recommendation and worker self-report cannot mark accepted outcome.

### ARM-050 — Advisory recommender and explanation

Implement transparent scoring, cold-start priors, minimum-sample/confidence behavior, model-version drift and persisted recommendation explanations.

**Gate:** recommendation cannot bypass eligibility or rewrite routing; independent architecture review.

### ARM-060 — Controlled comparable-task experiments

Implement comparability keys, prospective assignment, exclusion rules and opt-in read-only duplication where explicitly safe.

**Gate:** one-writer/side-effect safety review and statistical sanity fixtures.

### ARM-070 — Operator inspection surfaces

Expose static route vs adaptive recommendation vs actual launch vs accepted outcome, including capacity and confidence explanations. Keep controls advisory.

**Gate:** rendered UI inspection and privacy/redaction review.

### ARM-080 — Cross-provider subscription qualification

Run real qualification across available OpenAI, Claude and OpenCode subscription pools, including multiple Claude accounts, missing quota cases, resets if naturally observed, restart and cross-provider continuation.

**Gate:** RUNTIME PASS with evidence; no artificial paid usage created to force quota events.

### ARM-090 — Independent review and integration

Fresh independent review of architecture, security/privacy, billing safeguards, statistics/experimentation, upstream compatibility and requirement traceability. Repair findings, integrate only after acceptance, then run post-integration verification.

**Target verdict:** `T3 ADAPTIVE MODEL ROUTING V1 VERIFIED`.

## Deferred beyond V1

- automatically applying recommendations without explicit manager/user opt-in;
- cross-user/cloud federated learning;
- purchased/API capacity optimization;
- sophisticated ML prediction/scoring when transparent rules/statistics are adequate;
- automatic subscription purchasing/upgrading;
- dashboard-specific visualization beyond reusable read APIs.
