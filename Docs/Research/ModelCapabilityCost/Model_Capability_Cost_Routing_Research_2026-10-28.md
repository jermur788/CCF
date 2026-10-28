# Model capability, cost and CCF routing research

Research cutoff and source check: **2026-10-28**. Advisory only; no routing or purchasing decision is made here. Starting main: `98bd0d88af76e8d25a9134e5243fdfe145bb94fe`. Prices are USD, before tax/VAT, and can change. An API price is not the price of an included subscription request. Account entitlements and actual bills were not inspected.

## Executive summary

Use Codex `gpt-6.1-sol` for complex local implementation and integration; use `gpt-6.1-terra` for bounded fixes, visual/computer work and continuation. Preserve Claude subscription capacity for independent review and difficult synthesis, with Sonnet 5.5 a balanced working option and Opus/Fable reserved for hard cases. OpenCode Go DeepSeek V4.1 Flash is a promising value lane for bounded implementation, static inspection and watcher coordination, subject to verification and privacy settings. These are routing judgments, not controlled CCF rankings. Use a distinct model/provider for consequential review and validate artifacts independently. Quota exhaustion, tool availability, cache behavior, retries and review time can dominate nominal model cost.

Live local discovery and official pages establish public IDs for Sol, Terra, Astra, Luna and Claude's current models; they do **not** prove that a given subscription request is billed as a public API call or that a wrapper deploys an identical snapshot. `gpt-6x` and the historical MiMo executor remain unresolved. The previous cloud report is described in #52 as incomplete; its full text was not present in the supplied conversation, repository or issue, so only its recorded model/wrapper/deployment distinction could be preserved.

## Runtime inventory and mapping

On 2026-10-28, local `codex-cli 0.164.0` app-server JSON-RPC `initialize` then `model/list` (`includeHidden=true`, limit 100) returned a complete page (`nextCursor=null`). It lists visible `gpt-6.1-sol` (default), `gpt-6-astra`, `gpt-6.1-terra`, `gpt-6-sol`, `gpt-6-luna`, `gpt-5.6-sol`, `gpt-5.6-terra`, `gpt-5.6-luna`; `gpt-reserve` and `codex-auto-review` were hidden, so not asserted user-selectable. Text and image input, Fast tier, and low through max effort are reported; all except Luna also advertise `ultra` with automatic task delegation. App-server default effort: low for Sol/Astra/Terra/5.6 Sol; medium for 6 Sol/Luna and 5.6 Terra/Luna. These are runtime metadata, not API defaults; no context or token pricing was returned. T3 `orchestrator_capabilities` returned the live composer catalog and searched `gpt-6`, `gpt-6x`, `claude`, `deepseek`, and `mimo`. Search results are bounded and can omit entries; catalog display does not prove account entitlement or successful inference. The active executor was `codex` / `gpt-6.1-terra`, high effort, default service tier.

| CCF identifier/lane | Wrapper / provider instance | Model/deployment and route | Confidence and billing |
|---|---|---|---|
| Sol, Terra, Astra, Luna | T3 / Codex `codex` | Same exact public OpenAI model IDs appear in [OpenAI catalog](https://developers.openai.com/api/docs/models); local runtime identifies the IDs | Verified identifier match; exact deployment/snapshot and subscription billing unverified. API prices below are illustrative alternatives, not a claim about this session. |
| `gpt-6-sol`, 5.6 variants | Codex app-server | Older runtime IDs, as listed above | Runtime verified; current access and exact API billing not established. |
| `gpt-6x` | CCF reference / T3 | No matching catalog search result | Unresolved, not assigned a SKU. |
| T3 Code / TTree / Codex | application, agent or lane | They are wrappers, not model identities | Exact TTree implementation and equivalence to T3 unresolved. |
| Claude subscription 1 / 2 | T3 `claudeAgent` (Claude A) and `claudeAgent_claude_b_pro` (Claude B — Pro label) | Both catalog instances offer `claude-sonnet-5-5`, `claude-opus-5-5`, `claude-fable-5-1`, `claude-haiku-5-5`; public Anthropic IDs match | Identifier match verified; login, actual plan, entitlement and underlying account mapping not inspected. Label B is not proof of either subscription's terms. |
| `opencode-go/deepseek-v4.1-flash` | OpenCode Go | [OpenCode Go](https://opencode.ai/docs/go/) and [DeepSeek release](https://api-docs.deepseek.com/news/news260910/) identify DeepSeek V4.1 Flash; first-party API ID `deepseek-flash` | Family match supported by both providers; exact OpenCode snapshot and deployment unverified. DeepSeek's 1017 release is [documented](https://api-docs.deepseek.com/news/news261017/). Go and direct API have distinct billing. |
| MiMo historical S1-C worker | unidentified wrapper | Xiaomi MiMo family, historical version and route unknown | Unresolved; do not price historical run. Live T3 search lists `opencode-go/mimo-v2.5`, `mimo-v2.5-pro`, `mimo-v2.6-flash`, `xiaomi-token-plan-ams/mimo-v2.6-flash`, `opencode/mimo-v2.6-flash-free`; catalog presence is not proof of usable entitlement. |
| OpenCode other | `opencode` | Catalog lists Beam Free, Big Pickle, Claude and other providers; `opencode` and `opencode-go` prefixes are different routes | Underlying deployment, current availability and cost require route-specific verification. Disabled Cursor, Grok, Antigravity, Muse and Pi instances are not available child lanes. |

T3 exposes Codex effort low/medium/high/xhigh/max/ultra (Luna through max), default/priority service tier; Claude exposes effort plus fast mode on Opus, and OpenCode uses variant and build/plan agent controls. These wrapper choices need not equal the public API's accepted controls. For example, [Sol API](https://developers.openai.com/api/docs/models/gpt-6.1-sol) documents low through max, not runtime `ultra`. API limits and usage vary by account/tier; listing is not an inference test.

## Model profiles and capability evidence

* **Astra / Sol:** OpenAI describes Astra as its most capable for demanding reasoning and coding, and Sol as a lower-cost complex-work model. Both have 1.05M context, 128K max output, text/image input and text output, and documented function, web, file and computer tools. Sol's API tool calling requires Responses. These are provider claims, not proof of CCF task success. [Catalog](https://developers.openai.com/api/docs/models), [Sol model](https://developers.openai.com/api/docs/models/gpt-6.1-sol).
* **Terra / Luna:** Same public context/output ceilings. OpenAI positions Terra for visual reasoning, computer use, writing and scoped fixes, Luna for focused high-volume work. Terra's Responses tools include shell, patch, web, file and computer use; actual wrapper permissions differ. [Terra](https://developers.openai.com/api/docs/models/gpt-6.1-terra), [selection guide](https://developers.openai.com/api/docs/guides/model-selection). Lower price does not establish equal success on complex Unity work.
* **Claude Fable 5.1 / Opus 5.5 / Sonnet 5.5 / Haiku 5.5:** Anthropic reports 1M context and 128K output, text/image input and text output, adaptive thinking and tools. Fable and Opus thinking is always on. Anthropic positions Fable for demanding long-horizon reasoning, Opus for agentic coding, Sonnet for balanced coding, Haiku for high-volume low-latency work. Fable is slower, costs more, and requires 30-day retention on the described API route; check product eligibility. Sonnet and Opus do not accept forced tool choice in the current API. Provider-reported comparative latency is not a measured CCF turnaround. [Selection](https://platform.claude.com/docs/en/about-claude/models/choosing-a-model), [Sonnet](https://platform.claude.com/docs/en/models/sonnet-5-5/overview), [Opus](https://platform.claude.com/docs/en/models/opus-5-5/overview), [Fable](https://platform.claude.com/docs/en/models/fable-5-1/overview), [Haiku](https://platform.claude.com/docs/en/models/haiku-5-5/overview).
* **DeepSeek V4.1 Flash:** first-party docs specify 1M context, 384K max output, image input, tool calls and thinking/non-thinking; API effort low/high/max. DeepSeek's [pricing](https://api-docs.deepseek.com/quick_start/pricing/) reports account concurrency 2,500. OpenCode gateway limits/snapshot may differ. A low token price cannot prove low total task cost or correctness.
* **MiMo:** Live selectable names are versioned, but historical CCF `MiMo` is not. The OpenCode [Go model table](https://opencode.ai/docs/go/) offers versioned MiMo routes. Xiaomi's price/model pages could not be independently read by this primary through the available web tool; do not adopt delegated figures as verified. Historical version, multimodality, price and rate limits are unresolved for this lane.

### Portfolio and limits of comparison

The following are **working suitability judgments** (H = plausible strong choice, M = candidate with verification, ? = insufficient comparable evidence), grounded in provider descriptions and CCF records, not measured grades. All need source checking and acceptance tests.

| Dimension | Astra / Sol | Terra | Claude Fable / Opus / Sonnet | DeepSeek Flash | Luna / Haiku / MiMo |
|---|---|---|---|---|---|
| Repo engineering and agent tools | H/H | M | H/H/H | M; project evidence | M/M/? |
| Debugging and long autonomous work | H/H | M | H/H/M | M | M/M/? |
| Long-context retrieval/editing | H/H | H | H/H/H | M | M/M/? |
| Mathematics/science | H/H | M | H/H/M | ? | ? |
| Research synthesis/instruction following | H/H | H | H/H/H | M | M/M/? |
| Images/computer work | H/H | H | H/H/H | M image; gateway tools ? | M/M/? |
| Speed | ?/M | M | provider says slower/moderate/fast | ? | provider says fast/fastest; MiMo ? |
| Reliability/retries | ? | ? | ? | ? | ? |
| Cost efficiency | depends on successful tasks | depends on task | depends on quota and task | favorable listed rates; success uncertain | low listed API rates for Luna/Haiku; MiMo ? |

Independent same-harness results for these exact 2026 versions, budgets and CCF environment were not established; no numeric ranking is justified. [Terminal-Bench 2.1](https://www.tbench.ai/news/terminal-bench-2-1) revised 28 of 89 tasks; versions and scaffolds cannot be mixed. Its [integrity update](https://www.tbench.ai/news/leaderboard-integrity-update) identifies reward hacking and benchmark leakage. Anthropic's [infrastructure study](https://www.anthropic.com/research/infrastructure-noise) reports a six-point Terminal-Bench 2.0 spread across resource configurations in its own tests. DeepSeek's [1017 announcement](https://api-docs.deepseek.com/news/news261017/) describes its own mini-swe-v2 evaluation; this is vendor evidence, not a comparable independent CCF run. For research, math, retrieval and instruction following, no reproducible matched portfolio for these exact routes was established; do not extrapolate from terminal scores. Measure CCF success, elapsed time, retries and review burden on matched tasks before changing policy.

CCF observations are project-specific, not a benchmark: `Docs/Project/coordination/active-tasks.md` records RACK-V2-05 continuation after Sol, Terra and Claude A usage stops, with Claude B and an OpenCode watcher; the qualified candidate had 326 checks and 17 captures. It also records DeepSeek's RACK-V2-04 and PARITY-05 preflight work, MiMo's S1-C query foundation and Sonnet's parity audit/patch. Different scopes, collaborators and verification gates preclude causal rankings. Prefer a single writer, explicit handoff, exact commit and independent gates; a watcher observes and escalates rather than silently rewriting work.

## Pricing and quotas

All API prices are per million tokens. **I/R/W/O** = uncached input / cache read / cache write / output. Model card sources are linked above; [OpenAI pricing](https://developers.openai.com/api/docs/pricing), [Anthropic pricing](https://platform.claude.com/docs/en/about-claude/pricing), and [DeepSeek pricing](https://api-docs.deepseek.com/quick_start/pricing/) checked 2026-10-28. Individual runtime access to these API routes is unverified.

| Exact public route | I / R / W / O | Material conditions |
|---|---|---|
| OpenAI Astra | 10 / 1 / 12.5 / 50 | 30m write. |
| OpenAI Sol 6.1 | 2 / 0.10 / 2.50 / 10 | >272K input/request: all input/cache rates 2x, output 1.5x. |
| OpenAI Terra 6.1 | 1 / 0.05 / 1.25 / 5 | Same >272K premium. |
| OpenAI Luna 6 | 0.10 / 0.01 / 0.125 / 0.50 | Verify threshold on model page before use. |
| Anthropic Fable 5.1 | 10 / 0.25 / 12.5 / 50 | 5m write; 1h write $20. |
| Anthropic Opus 5.5 | 4 / 0.20 / 5 / 20 | 5m write; 1h write $8. |
| Anthropic Sonnet 5.5 | 2 / 0.10 / 2.50 / 10 | 5m write; 1h write $4. |
| Anthropic Haiku 5.5 | 0.10 / 0.01 / 0.125 / 0.50 | At ≤100K input/request; >100K: 0.50 / 0.05 / 0.625 / 2.50; 1h write 0.20 or 1. |
| DeepSeek direct Flash peak | 0.30 / 0.006 / miss 0.30 / 1.20 | First-party API; cache miss treated as write. |
| DeepSeek direct Flash off-peak | 0.15 / 0.003 / miss 0.15 / 0.60 | Half peak. Peak Mon–Fri 01–04 and 06–10 UTC, check holidays at source. |
| OpenCode Go DeepSeek Flash | Peak 0.30 / 0.006 / miss / 1.20; off-peak 0.15 / 0.003 / miss / 0.60 | Go $10/month: $80 monthly model allowance; Go Plus $40/month: $160. |
| OpenCode Go MiMo V2.6 Flash | 0.14 / 0.0028 / miss / 0.28 | Go $60, Plus $120 monthly model allowance; provider reports no separate write rate. |
| OpenCode Go MiMo V2.5 | 0.14 / 0.0028 / miss / 0.28 | Same advertised allowances as V2.6 Flash; historical CCF MiMo mapping unresolved. |

OpenAI Fast is 2x standard for Sol/Terra; Sol Ultrafast 6x, Batch/Flex half standard; eligible regional processing adds 10%. OpenAI search is $10/1,000 calls plus tokens; containers have separate charges. [Sol](https://developers.openai.com/api/docs/models/gpt-6.1-sol), [Terra](https://developers.openai.com/api/docs/models/gpt-6.1-terra), [tools](https://platform.openai.com/pricing). Anthropic Batch is 50% off input/output; most models have standard full-window pricing but Haiku has the threshold above. Server-side search and tools can add fees. API thinking tokens count as output; exact hidden reasoning usage depends on request/model and is not separately estimated. Cache writes replace the input charge for those tokens, not an extra charge; misses/expiry invalidate assumed savings. [OpenAI caching](https://developers.openai.com/api/docs/guides/prompt-caching), [Anthropic pricing](https://platform.claude.com/docs/en/about-claude/pricing). Rate limits depend on API tier and provider; quotas cannot be inferred from the context window. The user's Codex plan, its price, credits and account limits are unverified.

OpenCode defines each model’s five-hour and weekly limits as 20% and 50% of its monthly allowance, respectively; counts in its table are estimates, not guaranteed requests. DeepSeek Go requires Global region enabled. Actual plan and access were not inspected. [OpenCode Go](https://opencode.ai/docs/go/). The rate examples below can estimate allowance consumption when the same documented rates apply, not additional cash charges. For MiMo Flash Go, A = 0.168, B = 1.68, C = `1.1×0.14 + 8.9×0.0028 + 0.28 = 0.45892`, D = 0.56 allowance dollars, assuming a miss for the first prefix; actual accounting may differ.

Anthropic publishes Pro $20/month or $200/year, Max 5x $100/month and Max 20x $200/month; these are plan prices, not verified purchases for either account. Paid plans have five-hour session and weekly limits shared across Claude surfaces and Claude Code; Max has an additional Fable weekly limit. Usage varies with model, effort, context and tools; there is no universal published token allowance. Opt-in usage credits continue at standard API rates after included limits; they are separately billed and not the same as Max's monthly API credits. [Plans](https://support.claude.com/en/articles/11049762-choose-a-claude-plan), [limits](https://support.claude.com/en/articles/11647753-how-do-usage-and-length-limits-work), [credits](https://support.claude.com/en/articles/12429409-manage-usage-credits-for-paid-claude-plans). Two named T3 instances do not establish distinct accounts, independent pools, equal entitlements or double capacity. Check each account's usage display; do not invent token-equivalent subscription economics. Higher effort can generate more reasoning/tool output and long sessions resend context; thus they can deplete allowance faster even without extra billed dollars. Compaction and cache can help but do not guarantee quota savings.

## Normalized examples

Illustrative **direct API** dollars, not subscription charges. A = 1M uncached input + 0.1M output; B = 10M + 1M over multiple requests. C = 100 coding turns of 100K input each: 0.1M cache write, 8.9M cache reads, 1M uncached input, 1M output; all requests ≤100K and cache stays valid. D = 2M uncached input + 1M output including reasoning, split into requests ≤100K. No retries, tools, taxes, batch, regional premium or search calls. Formula C = `0.1W + 8.9R + 1I + 1O`; D = `2I + O`. DeepSeek's first cached prefix is billed as a miss. For example Sol C = `0.1×2.50 + 8.9×0.10 + 2 + 10 = $13.14`. Actual hit rates are not guaranteed. Tokenizers differ, so equal tokens are not equal work. One failed retry duplicating the workload doubles its corresponding cost; adding 100 OpenAI searches adds $1 plus search content tokens.

| Direct API | A | B | C | D |
|---|---:|---:|---:|---:|
| Astra | 15 | 150 | 70.15 | 70 |
| Sol 6.1 | 3 | 30 | 13.14 | 14 |
| Terra 6.1 | 1.50 | 15 | 6.57 | 7 |
| Luna 6 | 0.15 | 1.50 | 0.7015 | 0.70 |
| Fable 5.1 | 15 | 150 | 63.475 | 70 |
| Opus 5.5 | 6 | 60 | 26.28 | 28 |
| Sonnet 5.5 | 3 | 30 | 13.14 | 14 |
| Haiku 5.5 ≤100K | 0.15 | 1.50 | 0.7015 | 0.70 |
| DeepSeek Flash peak | 0.42 | 4.20 | 1.5834 | 1.80 |
| DeepSeek Flash off-peak | 0.21 | 2.10 | 0.7917 | 0.90 |

A and B assume multiple short requests, not an impossible one-million-token output. If Haiku C's requests exceed 100K, each component in C is 5x, or $3.5075; if Sol requests exceed 272K, full-request input/cache prices double and output rises 50%. D's assumed 1M output includes invisible reasoning: it is a scenario, not a provider's typical usage. Real research may have search costs and retries, and long coding can repeatedly rewrite context or miss cache. Go's included quota and subscription cost cannot be computed from this API table.

## CCF routing proposal

Judgment below is conditional on availability and verified tools. Quality choices largely rely on provider positioning, not a matched independent comparison. Value considers limited quota and operational handoffs; actual cost per accepted task needs measurement. Fallback must inherit the exact packet, commit, ownership and outstanding gates.

| Task | Best quality | Best value / default | Fast / cheap | Fallback | Avoid as default and why |
|---|---|---|---|---|---|
| Manager planning/spec/review | Astra or Fable | Sol | Terra | Opus | Flash-only final authority: high ambiguity needs review |
| Large Unity/C# implementation | Sol or Opus | Sol | DeepSeek Flash bounded parts | Sonnet or Terra continuation | Luna/Haiku solo on broad scope: insufficient CCF evidence |
| Bounded bugs | Sol | Terra | DeepSeek Flash | Sonnet | Fable for routine edits: scarce capacity |
| Git integration/publication | Sol | Terra with exact checks | DeepSeek read-only preflight | Sonnet | unattended cheap writer for conflicts: branch hygiene critical |
| Static/offline inspection | Opus | DeepSeek Flash with cited findings | Luna/Haiku | Terra | single model as sole acceptance gate |
| Deep research | Fable or Astra with web | Sol plus sourced collection | DeepSeek collection | Sonnet | unsourced synthesis from any model |
| Overnight autonomous | Sol or Opus | Sonnet or Sol with checkpoints | DeepSeek bounded worker | Terra with explicit handoff | high effort without quota/reset plan |
| Adversarial review | Fable / Astra independent of writer | alternate provider Sonnet / Sol | DeepSeek cross-check | Opus or Terra | same writer as sole reviewer |
| Docs/context refresh | Sol | Terra | DeepSeek or Haiku with checks | Sonnet | expensive frontier model for mechanical updates |
| Watcher/continuation | Sol for difficult recovery | OpenCode Flash watcher; Terra continuation | Flash | Sonnet | autonomous second writer; conflicts and duplicated work |

Reserve both Claude subscriptions' available capacity for independent review or hard continuations; check each reset and weekly meter without assuming separate pools. Use lower effort on well-specified work, bounded file sets, summaries with exact SHAs and fresh context when appropriate. Escalate effort for ambiguous cross-system changes. Stop on limit, preserve state and transfer one writer; use event-driven watchers when available. Keep production gates and human/Manager acceptance independent of token savings. No purchasing, account or routing changes are authorized by this report.

## Uncertainties and follow-up

Account plans, quota pools, credits, actual per-run usage, API access, hosted snapshots, MiMo historical identity, `gpt-6x`, and cross-model benchmark comparability remain unverified. T3's catalog is not an entitlement test; its search is bounded. Official commercial pages are mutable; recheck before spending. The first cloud execution's full report was not accessible here, though #52 records its incomplete outcome. A useful next step is a small blinded CCF task portfolio with fixed briefs, tests, model/effort/harness, tokens, retries and reviewer time; this is a proposal, not an authorized project decision.

## Source index

All links embedded above were checked on 2026-10-28. Primary sources: [OpenAI models](https://developers.openai.com/api/docs/models), [OpenAI pricing](https://developers.openai.com/api/docs/pricing), [Anthropic model selection](https://platform.claude.com/docs/en/about-claude/models/choosing-a-model), [Anthropic pricing](https://platform.claude.com/docs/en/about-claude/pricing), [Claude quota](https://support.claude.com/en/articles/11647753-how-do-usage-and-length-limits-work), [DeepSeek pricing](https://api-docs.deepseek.com/quick_start/pricing/), and [OpenCode Go](https://opencode.ai/docs/go/). Independent methodology: [Terminal-Bench 2.1](https://www.tbench.ai/news/terminal-bench-2-1); provider methodology: [Anthropic infrastructure noise](https://www.anthropic.com/research/infrastructure-noise). Local evidence: app-server and T3 catalog discovery and `Docs/Project/coordination/active-tasks.md` at the starting commit.
