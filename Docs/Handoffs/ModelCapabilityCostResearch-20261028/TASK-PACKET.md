# MODEL-CAPABILITY-COST-RESEARCH-20261028 — Deep Research Task Packet

## Task

Produce a current, evidence-backed comparison of **all AI models/providers actually available in the user's CCF Game Dev operating stack**, with emphasis on capability, weaknesses, context/usage limits, pricing/cost, and best routing by task type.

This is a **research-only** task. Do not edit production code, Unity assets, gameplay specs, save/schema files, project decisions, coordination locks, generated context, mirrors, or dashboard state.

## Authority and date

- Requested directly by the user on 2026-10-28.
- Research must reflect the **current market/product state as of 2026-10-28**, not model-memory assumptions.
- Current repository baseline when dispatched: `8f8e6f03c65894b0d933aad23a4ad69227fb33e8` on `main`.
- This research is advisory evidence until the Manager/user accepts any routing or purchasing recommendation.

## Research question

For every model/provider/lane the user currently has access to or is actively using for CCF work:

1. What is the exact underlying model, and what is merely an agent/lane/wrapper/product name?
2. What are its strongest capabilities?
3. What are its material weaknesses and common failure modes?
4. What task classes is it best suited to?
5. What task classes should it generally not be the first choice for?
6. What are its context-window, reasoning/effort, tool-use, coding-agent, multimodal, web/research and long-running-agent capabilities?
7. What are its current usage limits, subscription/session limits, rate limits or quota mechanics where publicly documented?
8. What does it cost to the user under each relevant access route: subscription, included quota, overage/extra usage, API input/output/cache pricing, or hosted third-party pricing?
9. How does cost change in practice for long autonomous coding/research runs, especially high/xhigh reasoning and large-context tasks?
10. How should the CCF Manager route real project work across these models to maximize quality per euro/dollar and preserve scarce subscription quota?

## Required model/provider inventory

Start from the models and lanes already observed in CCF project state and task claims, then verify exact names and expand only where the user's actual access is supported by evidence.

At minimum investigate:

### OpenAI / Codex family

- OpenAI models used through T3 Code / TTree Code / Codex lanes.
- Model strings observed in project execution, including `gpt-6.1-sol` and `gpt-6.1-terra` where they appear.
- Any current ChatGPT/Codex subscription-accessible OpenAI models that materially affect this workflow.
- Clearly distinguish **public model/product names** from internal/agent aliases. If a model string has no public official pricing page, say so; do not invent pricing.

### Anthropic / Claude

- Claude Sonnet 5.5 as used by the project.
- The user's two Claude subscriptions: label them **Claude subscription 1** and **Claude subscription 2**; do not infer they have different entitlements unless evidence says so.
- Any other Claude model actually selectable under those subscriptions that is materially useful to CCF work.
- Research the five-hour/session and weekly quota mechanics, model effort effects, Claude Code sharing of limits, and any paid overage/extra-usage mechanisms if current.

### DeepSeek / OpenCode

- `deepseek-v4.1-flash` / the exact DeepSeek model exposed through OpenCode, if that is an alias.
- OpenCode as a wrapper/agent must be separated from the underlying model/provider.
- Current hosted/API pricing and limits for the exact model/provider route actually usable here, if identifiable.

### MiMo

- Identify the exact MiMo model/version represented by the current project lane, or explicitly report that the alias is insufficient to price precisely.
- Research current official capability and pricing for the matching model family only when identity is defensible.

### Other actually available models

Inventory current repo task claims, handoffs and current platform/provider offerings for any additional models the user is demonstrably using or has access to. Do **not** turn this into a catalogue of every model on the market.

## Critical distinction: model vs product vs lane

Create an explicit mapping table:

`CCF name / lane` → `product/wrapper` → `underlying model` → `provider` → `access route` → `pricing basis` → `confidence`.

Examples of names that may be wrappers rather than models include T3 Code, TTree Code, Codex, OpenCode, Claude A/B, subscription 1/2, or Game Dev agent names. Verify rather than assuming equivalence.

## Evidence requirements

### Source priority

Use current sources in this order:

1. official provider model, pricing, limits and documentation pages;
2. official provider announcements/model cards/system cards;
3. reputable independent benchmark organisations with reproducible methodology;
4. high-quality third-party evaluations for dimensions official benchmarks do not cover;
5. community reports only for clearly labelled anecdotal operational behaviour.

For pricing, quota, context length and feature availability, prefer **official sources only** whenever available.

### Benchmark discipline

Do not compare models by one headline benchmark. Separate at least:

- software engineering / repo coding;
- agentic coding and tool use;
- debugging;
- long-context retrieval/reasoning;
- mathematical/scientific reasoning;
- general knowledge/research synthesis;
- instruction following;
- visual/multimodal work where relevant;
- speed/latency where defensible;
- cost efficiency.

Record benchmark date, methodology caveats and whether results are provider-reported or independently reproduced. Treat benchmark contamination, differing scaffolds, tool access and reasoning budgets as confounders.

### Practical CCF evidence

Use the project's recent execution history as **operational evidence**, not as a controlled benchmark. Examples may include:

- long Unity/repo implementation tasks;
- provider/session-limit handoffs;
- watcher/continuation use;
- bounded documentation work;
- G3 review/reasoning;
- static/offline verification;
- Git/branch hygiene.

Do not infer broad model quality from one successful or failed task. Clearly label project observations as anecdotal/project-specific.

## Pricing analysis

For each exact model/access route, capture where applicable:

- monthly subscription price;
- whether two subscriptions simply double independent quota pools or have another structure;
- included usage/session/weekly limits;
- extra usage/overage pricing;
- API input token price;
- API cached-input/read/write price where applicable;
- API output token price;
- batch/discount pricing if relevant;
- hosted third-party price if OpenCode or another route changes cost;
- currency and taxes/VAT caveat;
- date checked.

Then calculate illustrative **normalized costs** for at least:

- 1M input + 100k output tokens;
- 10M input + 1M output tokens;
- a long coding run with repeated large-context turns (state assumptions explicitly);
- a deep-research run with high output/reasoning usage.

Do not convert subscription quota into a fake per-token price unless the quota is publicly quantified. Where exact subscription economics cannot be computed, report `not directly derivable` and explain why.

## Required deliverable

Publish:

`Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`

The report must contain:

1. **Executive summary** — the practical answer in one page or less.
2. **Inventory/mapping table** — model vs wrapper vs access route.
3. **Per-model profiles** — strengths, weaknesses, context/tools/limits, pricing.
4. **Capability comparison matrix** using qualitative grades with evidence notes; avoid pseudo-precision.
5. **Pricing and quota matrix** with source dates.
6. **Normalized cost examples** with transparent arithmetic/assumptions.
7. **CCF task-routing matrix**, at minimum covering:
   - Manager planning/spec/review;
   - large Unity/C# implementation;
   - bounded bug fixes;
   - Git integration/rebase/publication;
   - static/offline code inspection;
   - deep research/evidence synthesis;
   - long overnight autonomous runs;
   - independent review/adversarial checking;
   - documentation/context refresh;
   - watcher/continuation role.
8. **Quota-preservation strategy** for the two Claude subscriptions and other scarce/high-cost routes.
9. **Recommended default routing policy**: primary, fallback and watcher for each task class.
10. **Uncertainties / unverified identities** — especially internal aliases or non-public pricing.
11. **Source list with direct links and access/check date**.

## Recommendation format

For every task class, recommend:

- **Best quality choice**;
- **Best value choice**;
- **Fast/cheap choice**;
- **Fallback when the preferred model hits quota/context/tool limits**;
- **Model to avoid as default**, if any, with a specific reason.

Recommendations must distinguish:

- empirical/public benchmark evidence;
- official provider claims;
- model-derived judgement;
- CCF project-specific operational observations.

## Research method and freshness

- Browse the web extensively; this task requires current verification.
- Check publication dates and current pricing/limits pages rather than relying on search snippets.
- Where provider documentation conflicts with marketing pages, report the conflict and prefer the more operationally specific source.
- If a model/product name appears newer than the researcher's prior knowledge, verify it rather than treating it as a typo.
- Record the research cutoff explicitly as 2026-10-28.

## Scope exclusions

- No purchasing action.
- No subscription/account changes.
- No API-key handling.
- No production code or Unity.
- No change to current Manager routing rules until the report is reviewed and accepted.
- No fabricated price for internal aliases.
- No generic catalogue of models the user does not have.

## Handoff and claim

Before substantive research, mark the task claim `[RUNNING]` with researcher/model/provider and starting repo HEAD.

At completion publish:

`Docs/Handoffs/ModelCapabilityCostResearch-20261028/HANDOFF.md`

The handoff must identify:

- exact report path and commit;
- source-check date;
- model inventory covered;
- any unresolved alias/access/pricing identities;
- major routing recommendations in brief;
- whether any recommendation depends mainly on provider self-report rather than independent evidence.

Then rename the claim `[HANDED-OFF]`, link the handoff, close it, and return exactly:

`MODEL CAPABILITY / COST RESEARCH COMPLETE — READY FOR MANAGER REVIEW`
