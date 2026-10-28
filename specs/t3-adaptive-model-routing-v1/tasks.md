# T3 Adaptive Model Routing V1 Bounded Tasks

- **Spec ID:** SPEC-T3-ADAPTIVE-MODEL-ROUTING-V1
- **Spec status:** APPROVED
- **Task list owner:** Overall Manager
- **Last updated:** 2026-10-28

These are implementation units, not new product decisions. Dispatch only within the approved governing spec and milestone dependencies below. Every substantial task requires a durable handoff. If implementation requires a material change to routing authority, billing/privacy constraints, CCF governance or experiment safety, stop and return to the spec.

## ARM-010 — Adaptive contracts and persistence skeleton

- **Task ID:** ARM-010
- **Status:** TODO
- **Goal:** Define durable adaptive-routing contracts, IDs, versioning and environment-database persistence without changing launch/routing semantics.
- **Scope:** prediction/realized-difficulty records; execution-observation references; capacity pool/snapshot/reset records; outcome reconciliation records; recommendation receipts; experiment assignment records; append-only fork migration; service/repository skeleton.
- **Exclusions:** scoring policy, provider quota adapters, UI, automatic routing.
- **Owner / routing:** bounded schema/migration work suitable for DeepSeek/OpenCode; Claude/Codex-class review for migration/CAS architecture.
- **Dependencies:** approved spec.
- **Verification:** contract decoding/bounds; fresh DB migration; adopted DB migration; malformed/partial migration fail-closed; restart persistence; existing Runtime records unchanged; no secrets in fixtures.
- **Stop conditions:** existing Runtime rows must be rewritten; a second provider catalog is required; privacy-safe pool identity cannot be represented without raw account data.
- **Completion state:** STATIC PASS + migration REVIEW PASS; durable handoff with exact schema and rebase hotspots.

## ARM-020 — Execution context observation

- **Task ID:** ARM-020
- **Status:** TODO
- **Goal:** Persist exact execution context for Runtime task attempts using the existing execution receipt and narrow additional snapshots.
- **Scope:** link adaptive observation to task/attempt; provider driver/instance/model/options; route source/rejections; runtime/interaction/workspace settings; permission/approval state when safely observable; tool/MCP capability fingerprint; before/after capacity snapshot references; attempt lifecycle facts.
- **Exclusions:** changing static route resolution, eligibility, orchestration launch, provider credentials, scoring.
- **Owner / routing:** Claude/Codex-class worker because execution atomicity/recovery semantics are architecture-sensitive.
- **Dependencies:** ARM-010.
- **Verification:** existing RTE execution/reconcile/cancel suite remains green; telemetry enabled/disabled produces identical launch semantics; restart/replay preserves one-attempt invariant; exact provenance fixture tests; unknown permission/tool data handled explicitly.
- **Stop conditions:** observation insertion could cause duplicate launch or ambiguous launch recovery; launch must occur outside `RuntimeTaskExecutionService`; normal eligibility semantics would change.
- **Completion state:** RUNTIME PASS for launch, restart, cancellation and reconciliation with telemetry.

## ARM-030 — Subscription capacity observation adapters

- **Task ID:** ARM-030
- **Status:** TODO
- **Goal:** Observe subscription quota/reset state through optional read-only provider adapters.
- **Scope:** adapter interface; privacy-safe capacity pool alias; multiple quota windows; exact/approx/unknown values; source quality; reset-event classification; initial OpenAI/Claude/OpenCode adapters only where reliable signals exist.
- **Exclusions:** API billing, token-price estimates, usage-credit enablement, purchases, auto-recharge, provider setting mutation, forced quota exhaustion.
- **Owner / routing:** bounded provider adapters may be split across provider-specific workers after interface approval; security/billing review by Claude/Codex-class reviewer.
- **Dependencies:** ARM-010; provider interface reconnaissance refreshed at task start.
- **Verification:** unsupported provider => unknown; multiple windows; reset timestamps; observed bonus reset classification; no fabricated values; multi-account separation; explicit tests that no adapter exposes or calls mutation/purchase/credit operations; redaction review.
- **Stop conditions:** reliable quota data requires paid/API access, scraping secrets, enabling credits, changing region/privacy, or mutating provider configuration.
- **Completion state:** STATIC PASS + provider runtime probes where available + billing/privacy REVIEW PASS.

## ARM-040 — Predicted and realized difficulty

- **Task ID:** ARM-040
- **Status:** TODO
- **Goal:** Create versioned pre-run difficulty predictions and post-evidence realized-difficulty derivations.
- **Scope:** deterministic feature snapshot; multidimensional prediction; band/confidence; manager override provenance; realized signals from attempts/review/rework/verification; calculation-version retention.
- **Exclusions:** opaque ML requirement; candidate scoring; using post-run data in original prediction.
- **Owner / routing:** straightforward predictor implementation can use DeepSeek/OpenCode; independent statistical/reasoning review by Claude/Codex-class worker.
- **Dependencies:** ARM-010, ARM-020.
- **Verification:** prediction timestamp/order precedes recommendation/launch; override does not destroy original; realized difficulty waits for required evidence; duration alone cannot determine outcome; historical derivations remain reproducible by version.
- **Stop conditions:** prediction needs private conversation content not in Runtime; labels cannot be computed without weakening verification semantics.
- **Completion state:** STATIC PASS + REVIEW PASS on label leakage and derivation logic.

## ARM-045 — Authoritative accepted-outcome reconciliation

- **Task ID:** ARM-045
- **Status:** TODO
- **Goal:** Reconcile execution observations with authoritative task/review/integration outcomes.
- **Scope:** idempotent/revision-aware outcome service; accepted-without-rework, accepted-after-rework, rejected/superseded, failed/cancelled/unresolved; CCF authority provenance; correction/reconciliation history.
- **Exclusions:** closing CCF claims; changing task verification automatically; trusting handoff/worker self-report as proof.
- **Owner / routing:** Claude/Codex-class worker due authority/integrity sensitivity.
- **Dependencies:** ARM-010, ARM-020; may proceed in parallel with ARM-040 after base contracts stabilize.
- **Verification:** terminal execution alone remains unresolved; handoff “done” remains unresolved; evidence-backed task verification can advance outcome; later failing evidence can reconcile/correct; CCF manager/integration truth wins on conflict; CAS/idempotency tests.
- **Stop conditions:** implementation would make T3 the owner of CCF integration/verification state.
- **Completion state:** RUNTIME PASS + independent authority REVIEW PASS.

## ARM-050 — Advisory recommendation engine

- **Task ID:** ARM-050
- **Status:** TODO
- **Goal:** Rank/explain eligible candidates using difficulty, accepted outcomes, reliability and known subscription capacity without changing static routing authority.
- **Scope:** transparent versioned scorer; research priors; comparable-task statistics; minimum sample/shrinkage; capacity/reserve pressure; model drift; recommendation receipt; no-recommendation path.
- **Exclusions:** silent saved-route mutation; automatic paid capacity; bypassing normal eligibility; black-box scorer with no explanation.
- **Owner / routing:** Claude/Codex-class architecture-sensitive implementation; independent reviewer required.
- **Dependencies:** ARM-030, ARM-040, ARM-045.
- **Verification:** only eligible candidates considered; manual/static policy remains authoritative; unknown quota lowers confidence without becoming zero; small sample behavior; model-version drift; deterministic fixtures; explanations expose evidence count/confidence/reasons.
- **Stop conditions:** scorer must modify `RuntimeRoutingResolver` precedence, or requires API-token prices to function.
- **Completion state:** STATIC PASS + recommendation integration RUNTIME PASS + REVIEW PASS.

## ARM-060 — Comparable-task experimentation

- **Task ID:** ARM-060
- **Status:** TODO
- **Goal:** Support controlled evidence gathering across genuinely comparable eligible tasks.
- **Scope:** comparability key/version; prospective assignment; candidate eligibility snapshot; randomized/controlled assignment metadata; propensity; experiment exclusions; opt-in safe read-only duplicate runs.
- **Exclusions:** duplicate implementation writers; changed acceptance criteria; automatic experiments on risky/destructive/integration/security tasks; quota exhaustion for experimentation.
- **Owner / routing:** Claude/Codex-class worker for experiment/concurrency design; independent statistical review.
- **Dependencies:** ARM-040, ARM-045, ARM-050.
- **Verification:** durable assignment before run; same rubric; one-writer CCF rule; exclusion fixtures; no side-effect duplicate path by default; experiment opt-out; accepted-outcome linkage; small-sample reporting.
- **Stop conditions:** experiment requires overlapping write lanes or unrepeatable external side effects.
- **Completion state:** STATIC PASS + safety REVIEW PASS; runtime qualification on safe tasks only.

## ARM-070 — Operator read/explanation surfaces

- **Task ID:** ARM-070
- **Status:** TODO
- **Goal:** Let managers inspect telemetry and understand recommendations without confusing them with authorization.
- **Scope:** reusable read APIs/RPC/MCP; task-level recommendation view; static-route vs recommendation vs actual-launch vs accepted-outcome display; quota/reset history; predicted-vs-realized difficulty; confidence and unknown-data display; privacy-safe account aliases.
- **Exclusions:** dashboard-specific wallboard; automatic route editing; raw secrets/account identifiers.
- **Owner / routing:** UI/read API may use DeepSeek/OpenCode for bounded implementation; rendered inspection and privacy review required.
- **Dependencies:** ARM-050; ARM-060 experiment view can be additive.
- **Verification:** rendered desktop/narrow views; accessibility; no secret leakage; correct unknown states; read-only behavior; recommendation clearly marked advisory.
- **Stop conditions:** UI needs to expose credentials/payment identity or mutate routing merely to display advice.
- **Completion state:** RUNTIME PASS + rendered REVIEW PASS.

## ARM-080 — Cross-provider subscription qualification

- **Task ID:** ARM-080
- **Status:** TODO
- **Goal:** Prove the system with real fixed-cost subscription pools and realistic CCF-style tasks.
- **Scope:** OpenAI, two Claude accounts where distinguishable, OpenCode Go Europe; multiple task roles/difficulty bands; restart persistence; provider unavailable cases; unknown quota; naturally observed reset events; accepted/reworked outcomes; cross-provider continuation.
- **Exclusions:** forced paid usage, purchased credits, API keys, deliberately exhausting accounts only to generate data.
- **Owner / routing:** Overall Manager coordinates; least-cost capable workers for routine probes; stronger independent review for failures/architecture.
- **Dependencies:** ARM-020 through ARM-070.
- **Verification:** real evidence bundle with provider/model/options/account-alias provenance; quota source quality; outcome authority; restart/reload; no billing safeguard breach; static routing semantics unchanged.
- **Stop conditions:** qualification would require paid overage/API use or privacy/region changes.
- **Completion state:** RUNTIME PASS across practical provider paths; unsupported capacity interfaces explicitly documented, not hidden.

## ARM-090 — Independent review and integration

- **Task ID:** ARM-090
- **Status:** TODO
- **Goal:** Independently review, repair, integrate and verify Adaptive Model Routing V1.
- **Scope:** architecture; routing authority; execution atomicity; migrations; privacy/security; billing safeguards; difficulty/outcome validity; experimentation; upstream compatibility; requirement traceability; post-integration tests.
- **Exclusions:** scope expansion to automatic routing V2.
- **Owner / routing:** fresh independent Claude/Codex-class reviewer; Overall Manager owns acceptance/integration.
- **Dependencies:** ARM-080.
- **Verification:** all requirements checklist items satisfied or explicitly deferred by accepted spec revision; clean final diff; appropriate package/full checks; restart and rendered inspection; post-integration evidence.
- **Stop conditions:** unresolved security/billing/integration conflict or genuine product decision.
- **Completion state:** `T3 ADAPTIVE MODEL ROUTING V1 VERIFIED` on intended integration base.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| ARM-010 | TODO | DeepSeek/OpenCode + strong review | Approved spec |
| ARM-020 | TODO | Claude/Codex-class | ARM-010 |
| ARM-030 | TODO | Provider-lane workers + strong security review | ARM-010 |
| ARM-040 | TODO | DeepSeek/OpenCode + strong reasoning review | ARM-010, ARM-020 |
| ARM-045 | TODO | Claude/Codex-class | ARM-010, ARM-020 |
| ARM-050 | TODO | Claude/Codex-class + independent review | ARM-030, ARM-040, ARM-045 |
| ARM-060 | TODO | Claude/Codex-class + statistical review | ARM-040, ARM-045, ARM-050 |
| ARM-070 | TODO | Bounded UI worker + rendered review | ARM-050 |
| ARM-080 | TODO | Overall Manager coordinated | ARM-020–070 |
| ARM-090 | TODO | Independent reviewer + Overall Manager | ARM-080 |