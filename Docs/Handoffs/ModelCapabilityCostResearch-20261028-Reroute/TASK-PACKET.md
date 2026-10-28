# MODEL-CAPABILITY-COST-RESEARCH-02 — Hybrid Completion

## Why this reroute exists

The first Deep Research execution produced a useful methodology/status report but explicitly stopped incomplete: it did not establish the required current provider-wide pricing/limits matrix, did not perform local Codex runtime discovery, did not publish the canonical research report/handoff, and did not complete repository validation. Treat that run as partial evidence, not completion.

This packet continues the original user request and original packet without weakening its evidentiary standard.

## Authority

1. Latest user request: complete a deep current comparison of the models actually available to the CCF Game Dev workflow, including capabilities, strengths/weaknesses, pricing/cost, limits and routing.
2. Original canonical packet: `Docs/Handoffs/ModelCapabilityCostResearch-20261028/TASK-PACKET.md`.
3. Claim for this continuation: #53.

Research cutoff: **2026-10-28**.

## Primary / lane

- **Primary and single repo writer:** local SOL/CODEX research-integrator lane with repository/runtime access; strongest available reasoning model preferred (GPT-6.1 Sol/Terra-class where selectable).
- The primary may delegate **read-only** web-source collection to Deep Research or another web-capable research agent and **read-only** cross-checking/tabulation to DeepSeek/OpenCode.
- All delegated evidence must be independently checked by the primary before it enters the canonical report.
- No Unity lock is required. Do not run Unity.

## Mandatory starting actions

1. Refresh current `main`; record exact starting SHA.
2. Mark claim #53 `[RUNNING]` with executor/model/provider, branch/worktree and starting SHA.
3. Read the original packet in full.
4. Read the first execution report supplied by the user. Preserve its useful architectural distinction: runtime alias, wrapper/agent, provider deployment/SKU and canonical base model are separate fields; unsupported mappings stay `Unverified`.
5. Inspect current task claims/handoffs/config for the model strings actually used by CCF rather than creating a generic market catalogue.

## Completion scope

Finish **all** original-packet requirements, with special attention to the gaps from the first run.

### A. Runtime truth — mandatory local discovery

Use the current local Codex/app-server/runtime discovery mechanism to enumerate the models/identifiers the user's current CCF coding environment can actually select. Record:

- exact runtime identifier;
- product/wrapper/lane presenting it;
- any reported provider/model metadata;
- context/effort/tool metadata exposed by runtime discovery;
- whether mapping to a public provider model/SKU is verified, inferred, or unresolved.

Do not map `gpt-6.1-sol`, `gpt-6.1-terra`, `gpt-6x`, T3/TTree, Codex, OpenCode, Claude A/B, or MiMo aliases to public SKUs without evidence.

If dynamic discovery cannot be executed, capture the exact blocker/evidence and continue every other section. This is a material limitation that must be explicit in the final handoff; do not silently substitute public catalogues.

### B. Current official provider evidence

For every defensibly identified model/access route actually relevant to the user's stack, verify current official sources for, where applicable:

- model identity/version and lifecycle;
- context and maximum output;
- reasoning/effort controls;
- tool/function use and coding/agent support;
- multimodal/web/research capabilities;
- subscription price and included-access model;
- session/five-hour/weekly/account quota semantics;
- extra usage/overage rules;
- API input, cached-input/cache write/read, output and reasoning-token billing;
- batch discounts/long-context premiums/tool/search charges if material;
- rate-limit semantics where publicly documented;
- regional/deployment restrictions where material.

Official provider documentation/pricing/limits pages are the authority for volatile commercial facts. Record access/check date and direct link. If a current number is not publicly available or is account-specific, say so.

### C. Required model inventory

At minimum resolve or explicitly mark unresolved the actual CCF-relevant identities for:

- OpenAI / Codex / T3 / TTree runtime models observed in claims, including `gpt-6.1-sol`, `gpt-6.1-terra`, `gpt-6x` and other currently selectable material OpenAI variants;
- Claude Sonnet 5.5 and other materially selectable Claude models under **Claude subscription 1** and **Claude subscription 2**; do not invent different entitlements between the two subscriptions;
- DeepSeek/OpenCode, including the exact identity behind `deepseek-v4.1-flash` if verifiable;
- MiMo, exact version if defensible;
- any additional model demonstrably used/available in current CCF task state.

Do not expand into a general catalogue of vendors/models the user does not have.

### D. Capability evidence

Use a portfolio rather than a single benchmark. Separate:

- repo/software engineering;
- agentic coding/tool use;
- debugging;
- long-context retrieval/editing;
- mathematical/scientific reasoning;
- research synthesis/instruction following;
- multimodal capability where relevant;
- speed/latency where defensibly comparable;
- reliability/retries;
- cost efficiency.

Keep provider-reported scores separate from independent same-harness measurements. Note benchmark date, reasoning/tool scaffold and comparability limitations. Prefer reproducible/actively maintained evaluations.

### E. CCF operational evidence

Use project execution only as labelled project-specific evidence. Relevant examples include provider/session-limit handoffs, long Unity/repo tasks, watcher/continuation behavior, bounded documentation work, G3 reviews, static verification and branch hygiene. Do not generalize from one run.

### F. Pricing and normalized cost calculations

For every exact route with defensible official pricing, provide transparent current pricing and calculate at least:

1. 1M input + 100k output tokens;
2. 10M input + 1M output tokens;
3. a long autonomous coding run with repeated large-context turns;
4. a deep-research run with high reasoning/output usage.

State cache-hit, repeated-context, reasoning-token/tool/search and retry assumptions. Show arithmetic. Do not invent a token-equivalent price for subscription quotas that are not numerically specified.

For the user's two Claude subscriptions, explain whether they are independent quota pools only to the extent supported by the account/product model; otherwise say the exact economics are not directly derivable. Explicitly analyze why high/xhigh/long-context autonomous use may consume subscription allowance rapidly.

### G. CCF routing recommendation

Produce a routing matrix for:

- Manager planning/spec/review;
- large Unity/C# implementation;
- bounded bug fixes;
- Git integration/rebase/publication;
- static/offline code inspection;
- deep research/evidence synthesis;
- long overnight autonomous runs;
- independent/adversarial review;
- docs/context refresh;
- watcher/continuation.

For each: **best quality**, **best value**, **fast/cheap**, **fallback**, and **avoid-as-default** where justified.

Recommendations must explicitly balance capability against scarce subscription quota and effective task cost. Keep model-derived routing judgement distinct from official/benchmark evidence.

## Required canonical deliverables

Publish the final research report at exactly:

`Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`

It must contain the complete sections specified by the original packet, including executive summary, mapping table, per-model profiles, capability matrix, pricing/quota matrix, normalized costs, routing matrix, quota-preservation strategy, recommended default/fallback/watcher policy, uncertainties and source list.

Publish the handoff at exactly:

`Docs/Handoffs/ModelCapabilityCostResearch-20261028/HANDOFF.md`

The handoff must record:

- exact report commit/path;
- starting and final SHA;
- runtime-discovery method/result;
- source-check date;
- exact inventory covered;
- unresolved identities/pricing/access;
- major routing recommendations;
- which recommendations depend mainly on provider self-report;
- validation results.

## Validation

Run `python -m unittest discover -v` from the repository root if the current repository supports that command as specified by the original execution contract. Record exact PASS/FAIL and distinguish unrelated/pre-existing failures. Do not change tests to manufacture green.

Also verify:

- only research/handoff documentation and any narrowly required research tooling/output changed;
- no production, Unity, gameplay/spec decision, save/schema, generated context/mirror/dashboard or subscription/account state changed;
- report links/source dates are present;
- every price/limit fact is source-backed or explicitly `Unverified`/account-specific.

## Stop / blocker rules

Stop and report rather than inventing if:

- a runtime alias cannot be mapped safely;
- pricing is unavailable for the actual access route;
- a quota is account-specific and not publicly quantified;
- benchmark comparisons are not methodologically comparable.

These unresolved cells do **not** automatically block the whole report if they are honestly labelled and the user still receives a useful routing/cost analysis.

## Claim / publication closure

At successful publication:

1. rename claim #53 `[HANDED-OFF]`;
2. link the canonical handoff;
3. close #53;
4. leave original #52 as historical incomplete/capability-stop provenance unless the Manager later closes/relabels it;
5. return exactly:

`MODEL CAPABILITY / COST RESEARCH COMPLETE — READY FOR MANAGER REVIEW`
