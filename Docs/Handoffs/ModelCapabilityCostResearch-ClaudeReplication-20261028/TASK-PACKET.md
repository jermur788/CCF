# MODEL-CAPABILITY-COST-RESEARCH-CLAUDE-20261028 — Blind Independent Replication

## Purpose

Independently repeat the 2026-10-28 CCF model capability/cost/routing research using a Claude model, then compare the independently reached conclusions against the already-integrated OpenAI-produced report.

This task is designed to measure **research divergence**. Claude must first reach and freeze its own conclusions without reading the OpenAI-produced report; only after that may it read the OpenAI report and write a comparison.

## Authority and baseline

- Direct user request: 2026-10-28.
- Starting repository baseline: current `main`, at dispatch `8ed6838b6a7dc84bcc4f37180b53b73330ca42c7` or descendant.
- Original research task packet: `Docs/Handoffs/ModelCapabilityCostResearch-20261028/TASK-PACKET.md`.
- Canonical OpenAI-produced report exists at `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`, but **MUST NOT be read during Phase A**.

## Critical user-cost constraint — software subscriptions only

The user does **not use API billing for this workflow**. Do not optimize around API token prices and do not present API pricing as the user's cost model.

The actual paid software/subscription portfolio to analyze is:

- Claude subscription/account 1: **€20/month**.
- Claude subscription/account 2: **€20/month**.
- OpenAI software subscription/account: **€20/month**.
- OpenCode Go Europe subscription: **€10/month**.
- Total stated recurring cost: **€70/month / €840/year**, assuming unchanged prices.

Treat those amounts as user-provided facts. Do not infer plan names, taxes, entitlements, quota size, or token-equivalent economics unless independently verified.

API pricing is **out of scope for value/routing recommendations** because the user is not using the API. It may be mentioned only when necessary to distinguish a provider model/SKU from a software subscription product, and must never be used as a proxy for the user's software subscription economics.

The core value question is therefore: **which paid software subscription produces the most accepted CCF work, with the least quota burn, rework, delay and review overhead?**

## Research question

For every model/provider/lane actually available in the user's CCF Game Dev software stack:

1. What is the exact model, and what is merely a wrapper/agent/product/lane name?
2. What are the model's strongest capabilities and material weaknesses?
3. Which CCF task classes should it lead, support, review or avoid?
4. What software features matter: context, reasoning/effort, tools, web/research, coding-agent behavior, multimodality, long-running work, continuity and handoff behavior?
5. What are the current software/subscription usage limits, reset windows, weekly/session mechanics or model restrictions where documented?
6. How should the user's **two €20 Claude subscriptions, one €20 OpenAI subscription, and €10 OpenCode Go Europe subscription** be allocated to maximize accepted project work per euro while preserving scarce quota?
7. Which recommendations are supported by independent evidence, which by provider claims, and which are judgment or CCF-specific observation?

## Model/provider inventory

Investigate only models/routes actually evidenced as available or in active CCF use. At minimum:

### OpenAI / Codex / T3/TTree
- `gpt-6.1-sol`, `gpt-6.1-terra`, Astra/Luna and other visible current Codex/runtime variants where evidenced.
- T3 Code, TTree Code and Codex as wrappers/lanes, not assumed model identities.
- `gpt-6x` only if identity can be established; otherwise mark unresolved.

### Anthropic / Claude
- Claude Sonnet 5.5.
- Other Claude models actually visible/selectable under the user's software routes where defensible.
- Two separate Claude subscriptions/accounts at €20 each.
- Five-hour/session and weekly quota mechanics, shared Claude/Claude Code usage where applicable, and the effect of model/effort/context/tool use on subscription depletion.
- Do not assume T3 Claude A/B maps cleanly to account 1/account 2 unless verified.

### OpenCode Go Europe
- DeepSeek V4.1 Flash and other current Go models actually relevant to the CCF workflow.
- Treat OpenCode as the product/wrapper and separate it from the underlying model.
- Verify the Europe/Global region constraint. If DeepSeek requires a Global workspace setting, state that availability under the user's existing Europe configuration is unverified unless directly established.
- Do not change region/privacy/account settings.

### MiMo
- Identify the exact current/historical MiMo route only if evidence supports it. Otherwise keep it unresolved.

## Phase A — blind independent research

### Blindness rule

During Phase A, **do not open, read, search, quote, summarize or infer from**:

- `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`
- its Manager reviews
- its worker handoff beyond knowing that a prior report exists
- PR #55 / PR #56 discussion or diffs that expose its conclusions

You may read the **original task packet** because it defines the research question, and you may inspect project execution evidence needed to identify actual model/lane usage.

### Evidence priority

1. Current official provider/product documentation for software plans, model availability, features and usage limits.
2. Official model cards/system cards/announcements.
3. Reputable independent benchmarks with reproducible methodology.
4. High-quality third-party evaluations for gaps.
5. CCF project history as explicitly labelled anecdotal/project-specific evidence.

Do not reduce model comparison to one benchmark. Separate coding, agentic tool use, debugging, long context, reasoning, research synthesis, instruction following, multimodality/computer use, latency and quota/value efficiency.

### Required Phase A report

Publish the independent Claude report to:

`Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md`

It must include:

1. Executive summary.
2. Model vs wrapper vs product mapping.
3. Per-model strengths/weaknesses.
4. Capability matrix with evidence labels and uncertainty.
5. Software subscription/usage-limit matrix — **not API pricing economics**.
6. Subscription-value analysis for the stated €70/month portfolio.
7. CCF routing matrix covering Manager planning/review, large Unity/C#, bounded fixes, Git integration, static inspection, deep research, long autonomous runs, independent review, docs/context refresh and watcher/continuation roles.
8. Quota-preservation strategy for the two Claude subscriptions plus OpenAI and Go.
9. Recommended default/fallback/watcher policy.
10. Uncertainties and unresolved identities.
11. Source list with check date.

### Freeze Phase A

Commit the Phase A report **before opening the OpenAI report**. Record that Phase A commit SHA in the handoff. After that commit, Phase A conclusions are frozen except for clearly marked factual corrections discovered during comparison.

## Phase B — divergence comparison

Only after Phase A is committed may Claude read:

`Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`

Then publish:

`Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md`

Compare the two reports explicitly:

- agreements;
- materially different model rankings;
- different routing recommendations;
- different interpretations of quota/value;
- different confidence/evidence standards;
- claims one report includes and the other omits;
- contradictions in factual claims;
- where either report appears biased toward its own provider/model family;
- where one report is more conservative or more speculative;
- which differences are likely caused by evidence selection versus reasoning/judgment;
- what additional test/data would resolve each meaningful disagreement.

Do not silently rewrite the Phase A report to converge with the OpenAI report.

## Comparison discipline

The purpose is not to declare Claude or OpenAI the winner. The output should show where independent research converges and where it diverges.

For every material disagreement, classify it as one of:

- factual/source disagreement;
- model-identity/access ambiguity;
- benchmark/evaluation-method difference;
- value/quota assumption difference;
- project-routing judgment difference;
- unsupported/speculative difference.

## Scope exclusions

- No Unity.
- No production code.
- No save/schema/gameplay/spec decisions.
- No account/subscription/region/privacy changes.
- No API keys.
- No purchasing action.
- No generated context/mirror/dashboard edits.
- No routing-policy change merely because this research exists.

## Target researcher/model

Use a **Claude model through one of the user's Claude software subscriptions** with web/research capability.

Preferred order if genuinely selectable under the subscription:
1. strongest Claude research/reasoning model available;
2. Claude Opus 5.5;
3. Claude Sonnet 5.5 at high reasoning/effort.

Do not claim a model was used unless the product/runtime actually identifies it. Record the exact displayed model/product/provider and which Claude subscription/account was used where known.

## Claim and handoff

Before substantive research, mark the task claim `[RUNNING]` and record:
- Claude model/product/provider;
- Claude subscription/account used if identifiable;
- starting repo HEAD;
- branch/worktree/publication lane.

At completion publish:

`Docs/Handoffs/ModelCapabilityCostResearch-ClaudeReplication-20261028/HANDOFF.md`

The handoff must record:
- Phase A independent report path + frozen commit SHA;
- Phase B divergence report path + final commit SHA;
- exact Claude model/product/provider used;
- source-check date;
- major agreements/disagreements in brief;
- unresolved account/model/access questions;
- confirmation that the OpenAI report was not read before Phase A was committed;
- validation performed;
- dashboard status.

Then mark the claim `[HANDED-OFF]`, close it, and return exactly:

`CLAUDE MODEL RESEARCH REPLICATION COMPLETE — READY FOR MANAGER REVIEW`
