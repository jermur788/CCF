# Orchestration Evidence Ledger Standard — V3

**Status:** Proposed CCF project-local standard  
**Date:** 2026-11-01

Use this standard for new material orchestration runs. CCF keeps its own detailed ledgers and evidence; Overall Dev coordinates the cross-project standard but does not own CCF implementation truth.

## Core evidence rule

Capture evidence when it becomes known. Do not reconstruct missing decision-time values later.

Keep these classes distinct:

- **OBSERVED_FACT** — directly supported by provider/runtime/repository/test/tool/user evidence.
- **ORCHESTRATOR_CLAIM** — what the orchestrator believed or why it acted.
- **RETROSPECTIVE_INFERENCE** — a conclusion drawn after later outcomes are known.
- **DERIVED_RECORD** — a recomputable statistic, prediction, recommendation, graph edge, feature, or other transformation; never treat it as an observed fact.

Use `UNKNOWN` when evidence is absent.

## Authority

For T3-observed work, reference T3 Runtime/ARM identities rather than manually duplicating machine evidence. T3 remains authoritative for the operational events it actually records.

For work T3 does not observe, the CCF ledger may temporarily be the primary audit evidence. Each material event should include a stable event ID and a schema-versioned machine block so it can later be extracted deterministically.

Do not maintain a second hand-written JSONL copy of the same event.

## Time and hindsight

Record separately where applicable:

- `occurred_at` — when the event is believed to have happened;
- `recorded_at` — when it was actually recorded;
- `observed_at` — when capacity/readiness/provider state was measured.

Do not backdate later evidence to make it look contemporaneous.

For later analysis, decision-time features must not include evidence recorded after the decision.

## Starting policy record

At the beginning of a material orchestration run record:

- user objective and protected decisions;
- hard constraints;
- optimization preferences: quality, cost, tokens, time, compute/resource use, exploration tolerance;
- provider/model ordering and escalation rules;
- concurrency/one-writer rules;
- review and independence requirements;
- current routing policy/profile/version if known;
- CCF-specific lane/worktree/Unity restrictions.

## Capacity, readiness, and context

Where material, maintain provider/pool observations with:

- opaque provider/pool identity;
- state such as `AVAILABLE`, `BUSY`, `COOLDOWN`, `QUOTA_EXHAUSTED`, `LOGIN_UNAVAILABLE`, `UNKNOWN`;
- remaining-capacity estimate if actually known;
- reset/window information;
- source, `observed_at`, confidence, current work, and freshness/staleness.

At dispatch record capacity-to-completion as `SUFFICIENT`, `MARGINAL`, `INSUFFICIENT`, or `UNKNOWN`, plus expected switching cost and material context pressure where known.

Prefer provider/runtime evidence over model self-report. Do not infer precise remaining quota merely from the absence of errors.

## Routing/decomposition decisions

Create an event for each material dispatch, switch, retry, escalation/de-escalation, decomposition, reviewer assignment, salvage/recovery decision, or intentional cancellation.

Preserve separately:

- requested provider/model/role;
- selected provider/model/role;
- T3-resolved route where applicable;
- actually executed provider/model/role.

Record task/work unit, parent, task risk/characteristics, decomposition, reason/rule, previous route, policy/profile version, prior outcome influencing the choice, capacity/context evidence, switching cost, and deciding actor/model where known.

### Choice set

Preserve realistic alternatives, not only the selected route. For each relevant candidate record:

- provider/model/pool;
- observed readiness/capacity state and source/time;
- orchestrator belief;
- `CONSIDERED`, `NOT_CONSIDERED`, or `UNKNOWN`;
- eligibility/rejection reason;
- capacity-to-completion estimate.

If exploration uses a known selection probability, record it; otherwise `UNKNOWN`.

Do not make unavailable, rejected, and never-evaluated candidates indistinguishable.

## Execution evidence

For each attempt record where observable:

- actual provider/model/config/reasoning effort;
- agent/role;
- task/thread/run/attempt IDs;
- branch/worktree/browser/Unity lane;
- start/end/status;
- provider failures;
- tokens/cost/latency;
- queue/serialization wait;
- commit/diff/artifact hash.

## Outcome and assessment chain

Worker completion is not acceptance.

Keep assessment events append-only. Preserve separately:

- worker completion;
- independent review;
- `NEEDS_CHANGES`/rejection;
- rework;
- re-review;
- acceptance;
- integration;
- runtime/Unity verification;
- unresolved/cancelled/reverted/superseded outcomes.

Each assessment should state assessor identity/role, relationship to worker/provider/model, exact commit/diff/artifact judged, time, verdict, evidence references, independence, scope, caveats, and requested rework.

For CCF/Unity work also record serialized Unity operations, lock/lane evidence, rendered/runtime qualification, and independently reviewed save/reference/determinism work where applicable.

## Human interventions

Record material user/Manager steer, interrupt, approve/reject, route override, restart, scope clarification, and protected-decision enforcement separately when they change the course of work.

## Privacy

Do not commit credentials, secrets, raw authentication material, or account-identifying capacity data.

Git-friendly evidence should prefer opaque IDs, enums, numbers, hashes, timestamps, and provenance references. Sensitive prompts/bodies remain local unless explicitly permitted and may be referenced by content hash/evidence reference.

## Machine block for non-T3-observed events

```json
{
  "schema_version": "orchestration-evidence/v3",
  "event_id": "oev-YYYYMMDD-example-001",
  "event_type": "routing_decision",
  "evidence_class": "ORCHESTRATOR_CLAIM",
  "occurred_at": "UNKNOWN",
  "recorded_at": "UNKNOWN",
  "project": "CCF",
  "work_unit": "UNKNOWN",
  "decision_id": "UNKNOWN",
  "requested_route": {"provider": "UNKNOWN", "model": "UNKNOWN"},
  "selected_route": {"provider": "UNKNOWN", "model": "UNKNOWN"},
  "choice_set": [],
  "capacity_refs": [],
  "reason": "UNKNOWN",
  "policy_version": "UNKNOWN",
  "executed_attempt_id": "UNKNOWN",
  "artifact_hash": "UNKNOWN",
  "evidence_refs": []
}
```

## Human-readable event template

### EVENT — `<timestamp>`

- **Event ID:**
- **Decision ID:**
- **Work unit / parent:**
- **Evidence class:**
- **Occurred at / recorded at:**
- **Requested provider/model/role:**
- **Selected provider/model/role:**
- **Actually executed provider/model/role:**
- **Choice set / alternatives:**
- **Reason / routing rule:**
- **Selection mechanism:**
- **Capacity/context evidence and references:**
- **Previous failure/outcome influencing decision:**
- **Branch/worktree/browser/Unity lane / thread/run/attempt:**
- **Artifact/commit/diff hash:**
- **Result/evidence:**
- **Assessment/review status:**
- **Accepted / rejected / unresolved:**
- **Failure classification if any:**
- **Human intervention if any:**
- **Next action:**

## Final run summary

Summarize attempts, completed/failed/cancelled/unresolved work, capacity failures, retries/reroutes/salvages, material routing/decomposition decisions, direct versus inferred/unknown capacity evidence, resource observations, review findings/independence, rework cycles, accepted/integrated/verified work, missing telemetry, and exact next action.

Retrospective learning conclusions must be marked as inference/hypothesis and are not active routing policy until separately evaluated and accepted.

## V0 minimum

The first learning-evidence V0 only needs enough evidence to reconstruct reliably:

1. why a route was chosen;
2. what information and realistic options existed then;
3. what actually executed;
4. what happened afterwards;
5. whether the work was ultimately accepted.

Do not delay genuine CCF work for graph, embedding, model-training, or other future learning infrastructure unless a later explicit decision changes the sequencing.
