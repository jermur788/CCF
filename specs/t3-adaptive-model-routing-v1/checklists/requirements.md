# T3 Adaptive Model Routing V1 Requirements Checklist

- **Spec ID:** SPEC-T3-ADAPTIVE-MODEL-ROUTING-V1
- **Status:** APPROVED traceability checklist
- **Owner:** Overall Manager
- **Last updated:** 2026-10-28

All boxes remain unchecked until implementation evidence exists. Spec approval alone does not satisfy implementation requirements.

## Constraints / governance

- [ ] **CON-001 — Subscription-only operation.** Adaptive routing uses already-authorized subscription-backed provider paths and does not require API billing. **Spec:** Product intent; Billing safeguards. **Planned:** ARM-030, ARM-080, ARM-090.
- [ ] **CON-002 — Fixed-cost pool model.** Routing treats subscriptions as finite capacity pools, not API token-price wallets. **Spec:** Product intent; Subscription capacity. **Planned:** ARM-030, ARM-050.
- [ ] **CON-003 — No API keys.** No adaptive feature requests, stores or requires API keys. **Spec:** Non-goals; Billing safeguards. **Planned:** ARM-010, ARM-030, ARM-090.
- [ ] **CON-004 — No paid credits/overage.** No usage-credit, overage, pay-as-you-go, auto-recharge or purchase path is enabled; Fable usage credits remain disabled. **Spec:** Billing safeguards. **Planned:** ARM-030, ARM-090.
- [ ] **CON-005 — No provider region/privacy mutation.** Adaptive telemetry cannot change provider region/privacy settings. **Spec:** Non-goals; Stop conditions. **Planned:** ARM-030, ARM-090.
- [ ] **GOV-001 — CCF ownership remains authoritative.** Adaptive routing cannot claim work, create overlapping CCF writers or change claim ownership. **Spec:** CCF governance. **Planned:** ARM-060, ARM-080, ARM-090.
- [ ] **GOV-002 — CCF verification remains authoritative.** Worker completion/recommendation cannot close claims or advance accepted CCF verification/integration truth. **Spec:** Accepted-work outcomes; CCF governance. **Planned:** ARM-045, ARM-090.
- [ ] **GOV-003 — Advisory V1.** Learned recommendation is supporting evidence and does not automatically rewrite saved routing or become project authority. **Spec:** Architecture; Adaptive recommendation. **Planned:** ARM-050, ARM-070.

## Architecture

- [ ] **ARCH-001 — Existing routing precedence preserved.** one-run → task → project → Development Profile → normal T3 remains unchanged. **Spec:** Current architecture; Architectural invariants. **Planned:** ARM-020, ARM-050, ARM-090.
- [ ] **ARCH-002 — Existing eligibility preserved.** Recommendation cannot bypass live provider availability, model support, project/provider settings, runtime/interaction restrictions, permission or orchestration authorization. **Spec:** Adaptive recommendation. **Planned:** ARM-020, ARM-050.
- [ ] **ARCH-003 — Existing execution path preserved.** Launches continue through RuntimeTaskExecutionService/orchestration and retain durable-recoverable-or-no-launch invariant. **Spec:** Core architecture. **Planned:** ARM-020.
- [ ] **ARCH-004 — No second provider catalog.** Adaptive services reuse ProviderRegistry/ProviderInstanceRegistry/ServerSettings through existing ownership seams. **Spec:** Current architecture; invariants. **Planned:** ARM-010, ARM-030.
- [ ] **ARCH-005 — Reusable domain services.** State/calculations live in reusable domain services accessible to RPC/MCP/UI/future clients, not only handlers. **Spec:** Architectural invariants. **Planned:** ARM-010, ARM-050, ARM-070.
- [ ] **ARCH-006 — Adaptive failure degrades safely.** Adaptive service failure leaves static routing/core Runtime operational. **Spec:** Failure and degradation. **Planned:** ARM-020, ARM-050.
- [ ] **ARCH-007 — Unknown is first-class.** Missing quota/account/permission/tool telemetry is represented explicitly, never guessed. **Spec:** Architecture; Capacity. **Planned:** ARM-010, ARM-020, ARM-030.

## Persistence / provenance

- [ ] **PER-001 — Environment DB persistence.** Adaptive raw observations persist in existing environment database through append-only fork migration. **Spec:** Persistence boundaries. **Planned:** ARM-010.
- [ ] **PER-002 — Project execution linkage.** Execution observations link to existing project/task/execution attempt IDs and task revision. **Spec:** Persistence; Execution telemetry. **Planned:** ARM-010, ARM-020.
- [ ] **PER-003 — Capacity pool scope.** Subscription capacity is environment/provider-pool scoped rather than incorrectly tied to one project. **Spec:** Persistence boundaries. **Planned:** ARM-010, ARM-030.
- [ ] **PER-004 — Derived data rebuildable.** Aggregates/recommendations are versioned and raw observations remain sufficient to rebuild them. **Spec:** Persistence; Plan data domains. **Planned:** ARM-010, ARM-050.
- [ ] **PER-005 — Restart persistence.** Predictions, observations, outcome reconciliations, recommendation receipts and experiment assignments survive restart/reload. **Spec:** Acceptance; verification. **Planned:** ARM-010, ARM-020, ARM-060.
- [ ] **PER-006 — Existing Runtime data not rewritten for migration.** Historical attempts without adaptive telemetry remain valid/unknown. **Spec:** Migration implications. **Planned:** ARM-010.

## Model / account / settings telemetry

- [ ] **TEL-001 — Exact provider identity.** Record provider driver + provider instance used. **Spec:** Identity; Execution context. **Planned:** ARM-020.
- [ ] **TEL-002 — Exact model identity.** Record exact model slug and available catalog/version fingerprint without rewriting history when aliases/defaults change. **Spec:** Identity. **Planned:** ARM-020, ARM-050.
- [ ] **TEL-003 — Exact provider options.** Preserve provider-native option IDs/values, including reasoning/effort where represented. **Spec:** Current architecture; Execution telemetry. **Planned:** ARM-020.
- [ ] **TEL-004 — Runtime/interaction settings.** Record effective runtime mode and interaction mode. **Spec:** Execution context. **Planned:** ARM-020.
- [ ] **TEL-005 — Permission/approval context.** Record effective permission/approval mode where safely available or explicit unknown. **Spec:** Execution context. **Planned:** ARM-020.
- [ ] **TEL-006 — Workspace strategy.** Record shared/root vs worktree strategy and safe base provenance. **Spec:** Execution context. **Planned:** ARM-020.
- [ ] **TEL-007 — Route provenance.** Record static winning layer/candidate/rejections/revisions available from resolver receipt. **Spec:** Execution context. **Planned:** ARM-020.
- [ ] **TEL-008 — Task role/type.** Record Runtime worker role and a versioned task/comparability category. **Spec:** Execution context; Comparable-task definition. **Planned:** ARM-020, ARM-060.
- [ ] **TEL-009 — Tool/MCP/connector capability context.** Record safe normalized categories and/or local capability fingerprint; no secrets or private content. **Spec:** Tool privacy. **Planned:** ARM-020.
- [ ] **TEL-010 — Privacy-safe account/seat identity.** Distinguish capacity pools/accounts where available using opaque local aliases rather than requiring raw emails/billing IDs. **Spec:** Identity model. **Planned:** ARM-010, ARM-030.
- [ ] **TEL-011 — Multi-account same-provider support.** Two Claude accounts/provider instances can remain distinguishable when architecture exposes that distinction. **Spec:** Acceptance. **Planned:** ARM-030, ARM-080.

## Difficulty

- [ ] **DIF-001 — Prediction before recommendation.** Predicted difficulty is persisted before adaptive candidate recommendation/selection. **Spec:** Predicted difficulty. **Planned:** ARM-040.
- [ ] **DIF-002 — Multidimensional prediction.** Include ambiguity, scope/breadth, reasoning complexity, integration/tool complexity, risk and verification burden. **Spec:** Predicted difficulty. **Planned:** ARM-040.
- [ ] **DIF-003 — Version/confidence.** Prediction includes algorithm/version, band and confidence. **Spec:** Predicted difficulty. **Planned:** ARM-040.
- [ ] **DIF-004 — Override provenance.** Manager/user override preserves original prediction plus override author/reason/time. **Spec:** Predicted difficulty. **Planned:** ARM-040.
- [ ] **DIF-005 — No label leakage.** Post-run/review/outcome data never modifies the historical pre-run prediction input snapshot. **Spec:** Predicted difficulty. **Planned:** ARM-040.
- [ ] **DIF-006 — Realized difficulty is evidence-derived.** Retries, failures, review, rework, verification, integration and scope signals contribute; duration alone is insufficient. **Spec:** Realized difficulty. **Planned:** ARM-040.
- [ ] **DIF-007 — Raw realized signals retained.** Derived realized score is versioned and reproducible from retained signals. **Spec:** Realized difficulty. **Planned:** ARM-040.

## Accepted-work outcomes

- [ ] **OUT-001 — Execution completion is not acceptance.** Terminal attempt alone leaves accepted outcome unresolved. **Spec:** Accepted-work outcomes. **Planned:** ARM-045.
- [ ] **OUT-002 — Handoff/self-report is not proof.** Worker “done”/handoff claims cannot mark accepted. **Spec:** Accepted-work outcomes. **Planned:** ARM-045.
- [ ] **OUT-003 — Evidence-backed reconciliation.** Outcome reconciles from task result/verification/review/integration evidence with source provenance. **Spec:** Accepted-work outcomes. **Planned:** ARM-045.
- [ ] **OUT-004 — Rework distinguished.** accepted_without_rework and accepted_after_rework remain distinct. **Spec:** Accepted-work outcomes. **Planned:** ARM-045.
- [ ] **OUT-005 — Corrections supported.** Later evidence can reconcile/correct an earlier outcome idempotently/revision-safely. **Spec:** Accepted-work outcomes. **Planned:** ARM-045.
- [ ] **OUT-006 — CCF conflict authority.** CCF canonical implementation/verification/integration truth wins for CCF outcome labeling. **Spec:** Accepted-work outcomes; Governance. **Planned:** ARM-045, ARM-080.

## Quota / reset tracking

- [ ] **QUO-001 — Read-only capacity interface.** Provider capacity adapter is optional/read-only and owned through existing provider-instance seams. **Spec:** Capacity adapter. **Planned:** ARM-030.
- [ ] **QUO-002 — Multiple windows.** Support multiple simultaneous provider-native quota windows. **Spec:** Capacity model. **Planned:** ARM-010, ARM-030.
- [ ] **QUO-003 — Native units/source quality.** Store exact provider units/value/reset time where exposed plus source/quality and exact/approx/unknown classification. **Spec:** Capacity model. **Planned:** ARM-030.
- [ ] **QUO-004 — Before/after linkage.** Attempts can reference capacity snapshots from before and after execution where observable. **Spec:** Execution telemetry. **Planned:** ARM-020, ARM-030.
- [ ] **QUO-005 — Scheduled reset.** Provider-reported expected reset can be stored/classified. **Spec:** Reset events. **Planned:** ARM-030.
- [ ] **QUO-006 — Bonus/unexpected reset.** Directly observed unexpected capacity increase/reset can be stored as observed_bonus_reset. **Spec:** Reset events. **Planned:** ARM-030.
- [ ] **QUO-007 — Inferred reset uncertainty.** Inferred discontinuities remain distinct from provider-reported/direct bonus observations. **Spec:** Reset events. **Planned:** ARM-030.
- [ ] **QUO-008 — OpenAI bonus resets not guaranteed.** Historical bonus reset observations cannot be treated as contractual future capacity. **Spec:** Capacity; acceptance. **Planned:** ARM-030, ARM-050.
- [ ] **QUO-009 — No fabricated token economics.** Quota is not estimated from API price/token tables, conversation tokens or duration. **Spec:** Missing telemetry. **Planned:** ARM-030, ARM-050.
- [ ] **QUO-010 — Fixed fees only for reporting.** €70/month portfolio may be recorded for budgeting but not converted to synthetic marginal token/task price. **Spec:** Fixed subscription fees. **Planned:** ARM-070.

## Recommendation / learning

- [ ] **REC-001 — Eligible candidates only.** Recommender only evaluates candidates valid under current T3 route/eligibility semantics. **Spec:** Candidate boundary. **Planned:** ARM-050.
- [ ] **REC-002 — Explainable receipt.** Recommendation persists candidate, difficulty, static route, evidence count, capacity, confidence, scoring version and human-readable reasons. **Spec:** Advisory output. **Planned:** ARM-050.
- [ ] **REC-003 — No recommendation path.** Low confidence/no eligible comparative evidence can return no_recommendation. **Spec:** Advisory output. **Planned:** ARM-050.
- [ ] **REC-004 — Accepted success over self-score.** Historical candidate performance uses authoritative accepted outcomes/rework, not worker self-rating. **Spec:** Objective function. **Planned:** ARM-050.
- [ ] **REC-005 — Reserve scarce strong pools.** Score can account for capacity/reset and predicted future need without making quota the only objective. **Spec:** Objective function. **Planned:** ARM-050.
- [ ] **REC-006 — Cold-start priors labeled.** Canonical research may seed priors but remains distinguishable from project evidence. **Spec:** Cold start. **Planned:** ARM-050.
- [ ] **REC-007 — Minimum sample/shrinkage.** Small samples do not create overconfident routing. **Spec:** Cold start. **Planned:** ARM-050.
- [ ] **REC-008 — Model drift.** Model/version changes retain provenance and decay/split historical relevance. **Spec:** Cold start; acceptance. **Planned:** ARM-050.
- [ ] **REC-009 — Manual authority.** Explicit one-run/manual choice remains highest and recommendation is not authorization. **Spec:** Advisory output. **Planned:** ARM-050, ARM-070.

## Controlled experiments

- [ ] **EXP-001 — Comparable key is pre-run/versioned.** Experiment comparability uses role, predicted difficulty, category/context, tool fingerprint, isolation, risk/verification burden and version. **Spec:** Comparable-task definition. **Planned:** ARM-060.
- [ ] **EXP-002 — Prospective assignment preferred.** Default experiment compares future comparable tasks rather than duplicating implementation writes. **Spec:** Experiment design. **Planned:** ARM-060.
- [ ] **EXP-003 — No duplicate side-effect writers.** Duplicate runs are prohibited for implementation/write tasks unless a future explicit decision changes this. **Spec:** Experiment design. **Planned:** ARM-060.
- [ ] **EXP-004 — Safe duplication only opt-in.** Read-only/research/test duplicate runs require explicit enablement and no side effects. **Spec:** Optional duplicated experiments. **Planned:** ARM-060.
- [ ] **EXP-005 — Risk exclusions.** Security, migration, concurrency, integration, urgent recovery and unique-effect tasks excluded from automatic experimentation. **Spec:** Exclusions. **Planned:** ARM-060.
- [ ] **EXP-006 — Durable assignment before run.** Candidate set, method/propensity, eligibility, capacity, comparability and exclusion checks persisted before execution. **Spec:** Assignment integrity. **Planned:** ARM-060.
- [ ] **EXP-007 — Same acceptance rubric.** Objective/acceptance/verification plan cannot be changed to favor an experiment arm. **Spec:** Assignment integrity. **Planned:** ARM-060.
- [ ] **EXP-008 — CCF one-writer preserved.** Experiment scheduler cannot violate CCF ownership lane. **Spec:** CCF governance. **Planned:** ARM-060, ARM-080.

## Privacy / security

- [ ] **PRIV-001 — Local-first raw telemetry.** Raw adaptive telemetry remains in local environment DB by default. **Spec:** Privacy. **Planned:** ARM-010.
- [ ] **PRIV-002 — No credentials/payment data.** Auth tokens, API keys, payment details and secret env values are excluded. **Spec:** Privacy. **Planned:** ARM-010, ARM-030.
- [ ] **PRIV-003 — Opaque account aliases.** Human account identity is optional; local opaque pool aliases are default. **Spec:** Identity; Privacy. **Planned:** ARM-010, ARM-030.
- [ ] **PRIV-004 — Tool privacy.** Tool/connector telemetry stores safe categories/fingerprint by default and never arguments/private retrieved content. **Spec:** Tool privacy. **Planned:** ARM-020.
- [ ] **PRIV-005 — Redacted logs/errors.** Sensitive provider/account payloads are not emitted to ordinary logs/errors. **Spec:** Privacy. **Planned:** ARM-030, ARM-090.
- [ ] **PRIV-006 — Export opt-in.** No automatic cloud/upload/export of adaptive learning data. **Spec:** Privacy. **Planned:** ARM-070, ARM-090.
- [ ] **PRIV-007 — Adaptive deletion boundary.** Adaptive telemetry can be removed without deleting authoritative Runtime/CCF records. **Spec:** Privacy. **Planned:** ARM-010, later retention task.

## Billing safety

- [ ] **BILL-001 — No purchase methods.** Capacity adapters expose no purchase/upgrade/credit-enablement method. **Spec:** Billing. **Planned:** ARM-030.
- [ ] **BILL-002 — Negative billing tests.** Test suite proves adaptive flows do not invoke API billing/credit/overage/auto-recharge paths. **Spec:** Verification. **Planned:** ARM-030, ARM-090.
- [ ] **BILL-003 — Fable credit safeguard.** Fable usage credits stay disabled in all V1 qualification. **Spec:** Billing. **Planned:** ARM-080, ARM-090.
- [ ] **BILL-004 — Billing relaxation is product decision.** Any future paid-capacity change requires governing spec revision. **Spec:** Billing; Stop conditions. **Planned:** governance review.

## Operator / UX

- [ ] **UX-001 — Static vs adaptive vs actual vs accepted distinguished.** UI/API makes four states explicit. **Spec:** Explainability. **Planned:** ARM-070.
- [ ] **UX-002 — Why-this-model explanation.** Predicted difficulty, route/eligibility, capacity, comparable evidence, confidence and uncertainty visible. **Spec:** Explainability. **Planned:** ARM-070.
- [ ] **UX-003 — Unknowns visible.** Missing capacity/account/settings data cannot be hidden behind confident-looking output. **Spec:** Explainability. **Planned:** ARM-070.
- [ ] **UX-004 — Advisory label.** Recommendation cannot be mistaken for authorization/accepted routing policy. **Spec:** Explainability. **Planned:** ARM-070.
- [ ] **UX-005 — Rendered accessibility review.** Operator surfaces receive rendered desktop/narrow/accessibility inspection. **Spec:** Verification. **Planned:** ARM-070, ARM-090.

## Verification / qualification

- [ ] **VER-001 — Migration qualification.** Fresh/adopted/malformed histories and restart persistence pass. **Planned:** ARM-010.
- [ ] **VER-002 — Execution atomicity regression.** Existing RTE launch/replay/reconcile/cancel invariant remains green with telemetry. **Planned:** ARM-020.
- [ ] **VER-003 — Eligibility/fallback regression.** Existing route fallback and provider/project eligibility behavior remains unchanged. **Planned:** ARM-020, ARM-050.
- [ ] **VER-004 — Outcome authority tests.** Terminal worker/handoff cannot forge accepted result; evidence correction reconciles. **Planned:** ARM-045.
- [ ] **VER-005 — Quota/reset cases.** Unknown, multiple windows, scheduled reset, bonus reset and inferred reset tested. **Planned:** ARM-030.
- [ ] **VER-006 — Cross-provider qualification.** OpenAI, Claude and OpenCode paths exercised where practical without paid overage/API use. **Planned:** ARM-080.
- [ ] **VER-007 — Multiple Claude accounts.** Distinct pool provenance tested if provider-instance architecture exposes the accounts distinctly. **Planned:** ARM-080.
- [ ] **VER-008 — Cross-provider continuation.** Durable task/adaptive state supports continuation after provider switch/restart. **Planned:** ARM-080.
- [ ] **VER-009 — Independent review.** Fresh reviewer examines architecture, privacy/security, billing, statistics/experiments and final evidence. **Planned:** ARM-090.
- [ ] **VER-010 — Post-integration verification.** Accepted implementation reruns required checks on intended integration base before VERIFIED. **Planned:** ARM-090.