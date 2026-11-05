# Mandatory Orchestration Evidence Appendix — V2

Append to the run/result handoff. Capture evidence during work; never reconstruct missing values later.

### 1. Policy
Record orchestrator provider/model/reasoning; user objective, hard constraints and optimization preferences (quality, money, tokens, time, resource/compute use, exploration tolerance); provider/model order and escalation; concurrency/one-writer and review rules; protected Manager/user decisions.

### 2. Capacity + context
Maintain per provider/pool: state (`AVAILABLE/BUSY/COOLDOWN/LOGIN-UNAVAILABLE/UNKNOWN`), remaining-capacity estimate, reset/window, source/confidence and current work. At dispatch record capacity-to-completion (`sufficient/marginal/insufficient/unknown`) and expected interruption/switching cost.

For the orchestrator record context limit, used/estimated context, source/confidence, compactions and material context-pressure concerns when available. Prefer runtime/provider evidence over model self-report.

### 3. Routing + decomposition
For each material dispatch, switch, retry or decomposition decision record: task/work unit + parent; task characteristics/risk; decomposition topology; previous and selected provider/model/reasoning/role; known alternatives; reason/rule; live capacity/context evidence; capacity fit; switching cost; `exploit/explore`; routing-policy/knowledge version if available; prior outcome affecting the choice. Unknown stays **UNKNOWN**.

### 4. Outcomes + resources
For each significant worker/subtask record actual provider/model/config; outcome; tests/runtime/review evidence; handoff/commit; accepted/rejected/unresolved; rework/retries/salvage; failure class. “Completed” is not acceptance.

Where observable record input/output tokens, model calls, reasoning effort, wall time, parallel workers and provider failures. Any compute/resource estimate must state source/confidence; never label a proxy as measured energy.

### 5. Learning assessment
Record what routing/decomposition worked; waste/incorrect choices; cheaper/lower-resource strategies likely sufficient; justified escalation; capacity/context effects; coordination overhead; missing telemetry; candidate experiments/policy changes. Mark hypotheses + confidence. They are not facts or active policy until evaluated/accepted.

### 6. Evidence rule
Keep distinct:
- **Observed fact:** provider/runtime/repo/test evidence.
- **Orchestrator claim:** what it believed/why it acted.
- **Retrospective inference:** later conclusion.

Preserve provenance; never convert claims/inferences into facts.

### 7. Final summary
Include attempts; completed/failed/cancelled; capacity failures; retries/salvages; material routing/decomposition decisions; direct vs inferred/unknown capacity decisions; resource observations; review findings; unresolved work; exact next action.

For game work also include Unity/editor lanes, serialized Unity operations, worktrees/branches, rendered/runtime qualification, independently reviewed save/reference/determinism work, and changed protected anchors/baselines.
