# CCF — Proposed canonical project-instruction amendment: outcome-first, reuse-first, proportionate assurance

**19 November 2026 | PROPOSAL / NOT CANONICAL / OWNER + #425 G4 APPROVAL REQUIRED**

**User question:** “Doesn't this necessitate a change to the project instructions?” **Answer:** yes. The [portfolio efficiency audit](https://github.com/jermur788/CCF/blob/docs/portfolio-development-efficiency-audit-20261119-v1/Docs/Handoffs/Simulator-Portfolio-20261119/CCF-PORTFOLIO-DEVELOPMENT-EFFICIENCY-AND-ANTI-DERAILMENT-AUDIT-20261119.md) alone is advisory and does not alter how present/future manager chats operate.

**Source inspection:** current `main` `Docs/Project/instructions/shared-project-instructions.md`, `Docs/Project/instructions/chatgpt-role.md`, `Docs/Project/AgentWorkflow.md`, `Docs/Project/instructions/claude-role.md`, root `AGENTS.md`, `CLAUDE.md`. The three recommended edits below strengthen existing guidance rather than creating a competing policy framework. Shared instructions already say “Small playable improvements; simplest maintainable solution ... no speculative infrastructure”; AgentWorkflow already uses risk-proportionate Tier 1/2/3; ChatGPT role already avoids unnecessary independent reviews. The defect is insufficiently *operational* guidance on priorities, reuse and blocker containment, not lack of security controls.

## 1. Exact proposed additions — await acceptance, do not apply to main yet

### A. `Docs/Project/instructions/shared-project-instructions.md`

**Insertion point:** immediately after the two paragraphs under `## Development approach`, before `## Ecology and simulation`.

**Proposed Markdown:**

> ### Outcome-first development and code reuse
>
> Make the currently accepted **player-visible gameplay outcome or source-verifiable simulation question** the default development priority. Every substantive task must name its smallest honest working demonstration and how that result advances the current milestone. A useful preparation step is not the product outcome; do not reclassify tooling, documentation or one-area manual work as completed gameplay or comparable-area extrapolation.
>
> **Inspect and reuse existing functioning source first.** Before commissioning replacement or parallel code, identify the relevant proven services and record a concise `REUSE / ADAPT / EXCLUDE` rationale. New implementation is appropriate only where an actual requirement, source incompatibility, isolation, safety or maintainability gap justifies it. Preserve all protected old-world, save and reference contracts; reuse does not excuse verification.
>
> If security, infrastructure, testing or coordination blocks one operation, **stop the affected operation**, describe the concrete hazard and required existing gate, and continue other *independently authorised and non-conflicting* work. Do not convert a blocked integration/Unity operation into an open-ended tool-hardening programme unless the defect directly prevents the selected milestone, threatens protected state, or the owner separately prioritises it. Never treat a safe-lane continuation as permission to bypass source, physical, Unity, save, review or release gates.
>
> When the owner clarifies purpose or scope, evaluate current plans against the actual desired outcome and supersede irrelevant assumptions promptly. Prefer one completed outcome and concise truthful evidence to many task packets, separate micro-reviews or documents.

### B. `Docs/Project/instructions/chatgpt-role.md`

**Insertion point:** immediately before `## Cost-aware task routing`.

**Proposed Markdown:**

> ## Outcome-first management and anti-derailment
>
> Keep the owner's active playable/simulation goal visible in each substantial dispatch and Manager status. First inspect the current source and ask which existing components should be `REUSED / ADAPTED / EXCLUDED`; a new parallel implementation needs a specific technical justification. For a blocked operation, classify (1) the actual hard stop and its authoritative gate, (2) independently authorised safe work that may continue, and (3) unrelated enhancements to defer or return to the owner's priority decision. A source-only candidate may proceed when lawful even if a distinct Unity integration gate remains blocked; it cannot claim Unity or release acceptance.
>
> Do not start a new infrastructure/security programme, repeated general audit, large verification campaign or serial review chain merely because a risk was mentioned. First identify a reproduced defect and explain how fixing it unblocks the **selected** feature or protects an actually threatened state. Use the existing AgentWorkflow's risk-based verification gates, one accountable substantial task with bounded internal repair/review, and one durable outcome handoff. Keep mandatory independent acceptance and real operational safety where they apply; do not self-waive approved gates. For minor reversible editorial/format fixes, use direct readback or focused validation without reflexively creating a new agent/claim.
>
> In the Manager update, state the gameplay/numerical outcome achieved, the true evidence tier, any current blocking action, and the next safe step. Do not count intermediate documents, tests or infrastructure as player-feature completion.

### C. `Docs/Project/AgentWorkflow.md`

**Insertion point:** immediately before `## Verification cost policy`. Do not replace the existing Tier 1/2/3, review gates or safety procedures.

**Proposed Markdown:**

> ## Lean feature delivery and blocker triage
>
> Task packets for substantive features state one accepted **observable outcome**, the smallest valid witness, known source components to `REUSE / ADAPT / EXCLUDE`, and the verification tier appropriate to the risk. Before designing a replacement system, inspect currently functioning services, fixtures and prior acceptance evidence.
>
> When a gate blocks work, perform a **bounded diagnosis** sufficient to name the blocked action, exact permission or test failure, and responsible authority. Classify:
>
> - **HARD STOP for the affected action:** missing approval/ownership or live exclusive lease; destructive or uncontrolled edits; unsafe save/frozen-reference mutation; source/test evidence unavailable for a required acceptance claim. Do not bypass or self-grant the gate.
> - **SAFE-LANE CONTINUATION:** separately authorised source, read-only research, docs, bounded offline verification or other non-conflicting work can continue without falsely claiming the blocked integration/Unity/release result.
> - **DEFER / OWNER PRIORITY DECISION:** nonblocking infrastructure hardening, speculative security research, broad test expansion, polish or architecture exploration without a reproduced immediate dependency. Do not silently elevate it above the selected feature.
>
> Do not reopen an already characterised blocker in repeated new research tasks unless new evidence materially changes risk. Return one concise blocker/alternative recommendation to the Manager when a safe workaround within existing authority is impossible. This is a limit on scope proliferation, **not an automatic agent/Unity lease expiry** or an instruction to ignore active safety problems.
>
> Preserve the existing risk-tiered verification policy, mandatory G2/G3/G4/G5 reviews where triggered, single-writer/physical Unity locks, old-save/reference constraints and truthful separate state labels. Require native-format validation of changed artifacts and tests directly covering affected behaviour. Book an independent reviewer for a substantial/high-risk **actual frozen candidate**, not before the candidate exists; prefer bounded fixes and changed-lines recheck in one task rather than extra managerial microtasks. No false PASS, weakened tests or self-acceptance.
>
> For process evaluation, compare actual accepted outcomes, defects/reopens, independent reviews and repeated handoff count. Record actual token/cost/time data only when available; otherwise mark them UNKNOWN.

## 2. Why exactly these changes

- **User priority:** The player-facing Compact30 forestry loop and Multi's representative-rack-to-multiple-comparable-areas purpose must not be replaced by better tooling or easier-to-test intermediate work.
- **Observed positive counterexample:** [#809](https://github.com/jermur788/CCF/issues/809) provided a personal E1 walking witness using bounded smoke checks; [#815](https://github.com/jermur788/CCF/issues/815#issuecomment-7015426935) reused original Scenario One management services for a source-local E2 candidate. Actual E2 Unity/save remains unqualified.
- **Observed detours:** [#652](https://github.com/jermur788/CCF/issues/652) / [#754](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-754-MANAGER-FINAL-SCRATCH-ACCEPTANCE-AND-LIVE-INSTALL-GATE-20261118.md) / [#802](https://github.com/jermur788/CCF/issues/802#issuecomment-7005995893) needed technical custody/safety work but were not substitutes for gameplay qualification. [#812–#817](https://github.com/jermur788/CCF/issues/817#issuecomment-7013740967) show product-noun semantics diverging from the real Multi end goal. [#818](https://github.com/jermur788/CCF/issues/818#issuecomment-7016237892) already records the user's near-term decision to defer further agent-security hardening; this proposal should not reactivate it.
- **Current safeguards already justified:** malformed #813 JSON, #789 archive/provenance gaps, and #802 deletion classification demonstrate value from focused actual validators and reviewer independence for consequential work.
- **Avoid broader policy churn:** the existing Git authority, SDD task classes, context generation, owner approval and shared source/Unity locks do not need redesign. Do **not** change Game Brief or forest mechanics Decision Log merely to encode a Manager working policy.

## 3. Approval, adoption and context publication

**Proposed G4 process under #425 / owner authority:**

1. Owner approves the **intent and wording**, or #425 provides a bounded correction proposal and obtains owner confirmation for this project-wide process change. No unilateral canonical instruction edits by the portfolio advisor.
2. Overall Manager or explicitly owned doc writer creates one disjoint source branch; edit **only the three named canonical files** with the bounded insertions. Inspect current GitHub blobs again first; reconcile concurrent changes and verify the diff/hashes/Markdown and absence of other paths. One focused reviewer/Manager readback proportionate to instruction scope; do not launch a multi-agent policy study.
3. Manager makes the approved change through the authorised canonical Git integration process. A branch-only proposal is **not** active project instructions.
4. At **one clean committed context SHA**, run `Tools/ProjectContext/generate_project_instructions.py`, verify manifest/source/mirror hashes, publish permanent Drive mirrors in place, refresh the current composite/compact ChatGPT Project Instructions and all relevant project attachments. The old `d05b27a...` upload/context remains historical, not “current” after the change. `AgentWorkflow.md` explicitly requires this; **do not hand-edit generated composites**.
5. Tell both active managers #731 and #730 to reload the same newly published context with their current live ownership records and preserve accepted task locks. Do not silently change existing workers' frozen context mid-task.

**If owner approval is not yet obtained:** leave this exact text as proposal, continue existing milestone work under the current instructions and issue-level owner #818 priorities; do not claim the Project instructions changed.

## 4. Evidence and authority limits

This Markdown is a **reviewable patch specification**, not an edited project setting, accepted source, runtime verification or new task grant. No CCF source/assets/scene/save/Ops/physical board/main/Drive/UI writes, no agent launch, no waiver. Project-context/dashboards **not synced or verified**. No measured aggregate savings/overall token spend available.
