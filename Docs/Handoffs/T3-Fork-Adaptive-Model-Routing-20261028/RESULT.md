# T3 Fork Adaptive Model Routing V1 — Spec Design Result

## VERDICT

**SPEC DRAFTED / READY FOR OVERALL MANAGER REVIEW**

No production code was implemented.

## BASE / CHECKPOINT

- Repository: `jermur788/CCF`
- Starting base: `main @ f59e061a918df8a47a8249d3b32fbe56255aeb6a`
- Branch: `spec/t3-adaptive-model-routing-v1`
- Manager: Overall Manager
- Provider/model: OpenAI / GPT-5.6 Sol
- Claim: #61, marked `[RUNNING]` before substantive work
- T3 Fork architecture inspected at: `jermur788/t3code main @ 02a3d7a3a3129bdc80b283e991fd8bf9347e0f91`

## RESULT

Drafted the governing V1 design package:

- `specs/t3-adaptive-model-routing-v1/spec.md`
- `specs/t3-adaptive-model-routing-v1/plan.md`
- `specs/t3-adaptive-model-routing-v1/tasks.md`
- `specs/t3-adaptive-model-routing-v1/checklists/requirements.md`

The design treats Adaptive Model Routing as an additive observation/recommendation layer beside the existing T3 Runtime routing/execution path, not as a second router or provider registry.

Key fixed boundaries in the draft:

- fixed-cost subscription pools, not API-token economics;
- no API billing/API keys/purchased credits/overage/auto-recharge;
- Fable usage-credit paths remain disabled absent a new explicit product decision;
- exact provider/model/options/runtime/interaction/workspace telemetry plus privacy-safe account/capacity-pool identity;
- permission/tool/MCP/connector context captured only where safely observable, with explicit unknowns;
- provider-native quota/reset observations when available, with multiple windows and explicit source quality;
- scheduled, observed bonus and inferred reset events remain distinct;
- observed OpenAI bonus resets are stochastic evidence, never guaranteed future entitlement;
- predicted difficulty is captured before recommendation; realized difficulty is derived later from execution/review/rework/verification evidence;
- worker completion/handoff is not accepted-work proof;
- CCF canonical ownership, review, integration and verification remain authoritative for CCF outcomes;
- controlled experimentation prefers prospective assignment across comparable tasks and prohibits duplicate side-effect writers;
- recommendation remains advisory in V1 and cannot bypass static routing/eligibility or silently mutate saved routing;
- privacy/redaction and billing safeguards have explicit traceability requirements and future negative tests.

## T3 FORK ARCHITECTURE EVIDENCE INSPECTED

Inspected current fork contracts/services rather than fixing a schema from assumptions:

- `packages/contracts/src/projectRuntime.ts`
  - durable task lifecycle, acceptance criteria, result, handoffs, evidence-backed verification, task routing;
- `packages/contracts/src/runtimeRouting.ts`
  - route candidates, provider instance/driver/model/options, isolation, runtime/interaction modes and fallback semantics;
- `packages/contracts/src/runtimeExecution.ts`
  - durable execution attempts and routing receipts with exact model selection, route resolution, launch settings and task packet;
- `packages/contracts/src/modelSelection.ts`
  - exact provider instance/model/options selection;
- `packages/contracts/src/model.ts`
  - generic provider option selections suitable for preserving reasoning/effort settings without provider-specific schema forks;
- `apps/server/src/runtimeRouting/RuntimeRoutingService.ts`
  - existing route resolution and precedence;
- `apps/server/src/runtimeRouting/RuntimeRoutingProviderView.ts`
  - read-only view over ProviderRegistry, ProviderInstanceRegistry and ServerSettings; Runtime owns no second provider catalog;
- `apps/server/src/projectRuntime/RuntimeTaskExecutionService.ts`
  - static route resolution, final live eligibility recheck and existing orchestration launch path.

The inspected Runtime path has no normalized quota/reset contract. The spec therefore uses an optional read-only capacity-observation adapter and requires unsupported/missing telemetry to remain `unknown` rather than inferred.

## CANONICAL RESEARCH INGESTED

- `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/RESEARCH-CORRECTIONS-2026-10-28.md`
- `Docs/Handoffs/ModelCapabilityCostResearch-ClaudeReplication-20261028/MANAGER-REVIEW.md`

Research is treated as evidence/priors with provenance and uncertainty, not as an implicit accepted routing-policy change.

Canonical corrections preserved:

- MiMo attribution is Xiaomi, not MiniMax;
- Fable usage-credit/overage-like billed paths are excluded by the subscription-only constraint;
- OpenAI bonus reset observations are not contractual guarantees.

## FILES CHANGED

Only specification/handoff documentation on this branch:

- `specs/t3-adaptive-model-routing-v1/spec.md`
- `specs/t3-adaptive-model-routing-v1/plan.md`
- `specs/t3-adaptive-model-routing-v1/tasks.md`
- `specs/t3-adaptive-model-routing-v1/checklists/requirements.md`
- `Docs/Handoffs/T3-Fork-Adaptive-Model-Routing-20261028/RESULT.md`

No T3 production code, CCF gameplay code, Unity assets, save schemas or provider settings were changed.

## VERIFICATION

Completed:

- confirmed CCF `main` remained at the instructed starting head before branching;
- marked claim #61 `[RUNNING]` before substantive design work;
- inspected actual T3 Fork routing/execution/provider-instance architecture;
- ingested all canonical OpenAI/Claude routing research and manager corrections/review;
- traced every requested product constraint into the spec and requirements checklist;
- split implementation into bounded ARM-010 through ARM-090 milestones/tasks;
- explicitly preserved advisory V1, billing/privacy safety and CCF outcome authority.

Not performed because this task is spec-design only:

- production implementation;
- build/typecheck/runtime tests;
- provider quota probes;
- model-routing experiments;
- independent spec review/approval.

## FAILURES / RISKS

No implementation blocker discovered during spec drafting.

Material design questions intentionally left for Overall Manager review rather than silently decided:

1. cross-project learning scope (draft recommends project-local learning aggregates by default while capacity remains environment/pool scoped);
2. whether automatic application of recommendations should remain deferred beyond V1 (draft says yes);
3. exact retention/roll-up defaults for raw adaptive telemetry;
4. whether user-defined human-readable account aliases should be allowed in addition to opaque local pool IDs.

Provider-specific quota surfaces may be unstable or unavailable. The draft treats `unsupported`/`unknown` as valid outcomes and does not require scraping or paid/API access.

## REVIEW

No independent review has occurred yet.

This is a DRAFT spec package. The next authority step is Overall Manager review: accept, amend or reject the visible product decisions and architecture boundaries before implementation tasks are dispatched.

## INTEGRATION STATE

`NOT INTEGRATED`

The spec branch should remain review-only until the Overall Manager approves the governing spec.

## EXACT NEXT ACTION

Overall Manager reviews:

1. `spec.md` product/authority/billing/privacy boundaries;
2. four visible open product decisions;
3. `plan.md` ARM-010 → ARM-090 sequence;
4. `tasks.md` routing/ownership and verification gates;
5. `checklists/requirements.md` traceability completeness.

If approved, mark the spec `APPROVED`, reconcile any accepted decisions into canonical project state, and issue ARM-010 as the first bounded implementation task. Do not begin production implementation while the spec remains `DRAFT`.
