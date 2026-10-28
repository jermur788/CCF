# Model capability, cost and CCF routing research

Research cutoff and source check: **2026-10-28**. Subscription-focused amendment following the user’s clarification: two Claude accounts at €20 each, one OpenAI account at €20, and one OpenCode Go Europe subscription at €10 per month. **Total: €70/month or €840/year**, assuming unchanged monthly prices. These amounts are user-reported; plan names, tax treatment, account meters and entitlements have not been inspected. The amendment is advisory and postdates the Manager review of the original report. No account or purchasing changes are made. Starting main: `98bd0d88af76e8d25a9134e5243fdfe145bb94fe`.

## Executive summary

The four subscriptions cost €70/month. Treat OpenCode Go as the high-volume first stop for bounded coding, static checks, documentation and monitoring where the model is available in your Europe settings. Use your OpenAI subscription for demanding local implementation, integration and visual/computer work; use your two Claude accounts for independent review, difficult work and continuity when available. Choose the strongest model actually offered by each account for the task: Sonnet 5.5 is a useful working option; availability of Opus or Fable under either account is not established. Use Sol for complex local work and Terra for bounded fixes when available. This allocation is a hypothesis to test against accepted work and remaining quota, not a measured ranking. These are routing judgments, not controlled CCF rankings. Use a distinct model/provider for consequential review and validate artifacts independently. Quota exhaustion, tool availability, cache behavior, retries and review time can dominate nominal model cost.

Live local discovery and official pages establish public IDs for Sol, Terra, Astra, Luna and Claude's current models; they do **not** prove that a given subscription request is billed as a public API call or that a wrapper deploys an identical snapshot. `gpt-6x` and the historical MiMo executor remain unresolved. The previous cloud report is described in #52 as incomplete; its full text was not present in the supplied conversation, repository or issue, so only its recorded model/wrapper/deployment distinction could be preserved.

## Runtime inventory and mapping

On 2026-10-28, local `codex-cli 0.164.0` app-server JSON-RPC `initialize` then `model/list` (`includeHidden=true`, limit 100) returned a complete page (`nextCursor=null`). It lists visible `gpt-6.1-sol` (default), `gpt-6-astra`, `gpt-6.1-terra`, `gpt-6-sol`, `gpt-6-luna`, `gpt-5.6-sol`, `gpt-5.6-terra`, `gpt-5.6-luna`; `gpt-reserve` and `codex-auto-review` were hidden, so not asserted user-selectable. Text and image input, Fast tier, and low through max effort are reported; all except Luna also advertise `ultra` with automatic task delegation. App-server default effort: low for Sol/Astra/Terra/5.6 Sol; medium for 6 Sol/Luna and 5.6 Terra/Luna. These are runtime metadata, not API defaults; no context or token pricing was returned. T3 `orchestrator_capabilities` returned the live composer catalog and searched `gpt-6`, `gpt-6x`, `claude`, `deepseek`, and `mimo`. Search results are bounded and can omit entries; catalog display does not prove account entitlement or successful inference. The active executor was `codex` / `gpt-6.1-terra`, high effort, default service tier.

| CCF identifier/lane | Wrapper / provider instance | Model/deployment and route | Confidence and billing |
|---|---|---|---|
| Sol, Terra, Astra, Luna | T3 / Codex `codex` | Same exact public OpenAI model IDs appear in [OpenAI catalog](https://developers.openai.com/api/docs/models); local runtime identifies the IDs | Verified identifier match; exact deployment/snapshot and subscription billing unverified. Included subscription route as reported by the user; actual plan and model entitlement not inspected. |
| `gpt-6-sol`, 5.6 variants | Codex app-server | Older runtime IDs, as listed above | Runtime verified; access through the user’s subscription is not established. |
| `gpt-6x` | CCF reference / T3 | No matching catalog search result | Unresolved, not assigned a SKU. |
| T3 Code / TTree / Codex | application, agent or lane | They are wrappers, not model identities | Exact TTree implementation and equivalence to T3 unresolved. |
| Claude subscription 1 / 2 | T3 `claudeAgent` (Claude A) and `claudeAgent_claude_b_pro` (Claude B — Pro label) | Both catalog instances offer `claude-sonnet-5-5`, `claude-opus-5-5`, `claude-fable-5-1`, `claude-haiku-5-5`; public Anthropic IDs match | The user confirms two separate €20 accounts. The mapping of T3 A/B to those accounts, plan names, model entitlement and actual remaining limits have not been inspected. |
| `opencode-go/deepseek-v4.1-flash` | OpenCode Go | [OpenCode Go](https://opencode.ai/docs/go/) and [DeepSeek release](https://api-docs.deepseek.com/news/news260910/) identify DeepSeek V4.1 Flash; first-party API ID `deepseek-flash` | Family match supported by both providers; exact OpenCode snapshot and deployment unverified. DeepSeek's 1017 release is [documented](https://api-docs.deepseek.com/news/news261017/). Go Europe availability depends on workspace region settings; do not assume this model is enabled. |
| MiMo historical S1-C worker | unidentified wrapper | Xiaomi MiMo family, historical version and route unknown | Unresolved; do not price historical run. Live T3 search lists `opencode-go/mimo-v2.5`, `mimo-v2.5-pro`, `mimo-v2.6-flash`, `xiaomi-token-plan-ams/mimo-v2.6-flash`, `opencode/mimo-v2.6-flash-free`; catalog presence is not proof of usable entitlement. |
| OpenCode other | `opencode` | Catalog lists Beam Free, Big Pickle, Claude and other providers; `opencode` and `opencode-go` prefixes are different routes | Underlying deployment, current availability and cost require route-specific verification. Disabled Cursor, Grok, Antigravity, Muse and Pi instances are not available child lanes. |

T3 exposes Codex effort low/medium/high/xhigh/max/ultra (Luna through max), default/priority service tier; Claude exposes effort plus fast mode on Opus, and OpenCode uses variant and build/plan agent controls. These wrapper choices need not equal the public API's accepted controls. For example, [Sol API](https://developers.openai.com/api/docs/models/gpt-6.1-sol) documents low through max, not runtime `ultra`. API limits and usage vary by account/tier; listing is not an inference test.

## Model profiles and capability evidence

* **Astra / Sol:** OpenAI describes Astra as its most capable for demanding reasoning and coding, and Sol as a lower-cost complex-work model. Both have 1.05M context, 128K max output, text/image input and text output, and documented function, web, file and computer tools. Sol's API tool calling requires Responses. These are provider claims, not proof of CCF task success. [Catalog](https://developers.openai.com/api/docs/models), [Sol model](https://developers.openai.com/api/docs/models/gpt-6.1-sol).
* **Terra / Luna:** Same public context/output ceilings. OpenAI positions Terra for visual reasoning, computer use, writing and scoped fixes, Luna for focused high-volume work. Terra's Responses tools include shell, patch, web, file and computer use; actual wrapper permissions differ. [Terra](https://developers.openai.com/api/docs/models/gpt-6.1-terra), [selection guide](https://developers.openai.com/api/docs/guides/model-selection). Lower price does not establish equal success on complex Unity work.
* **Claude Fable 5.1 / Opus 5.5 / Sonnet 5.5 / Haiku 5.5:** Anthropic reports 1M context and 128K output, text/image input and text output, adaptive thinking and tools. Fable and Opus thinking is always on. Anthropic positions Fable for demanding long-horizon reasoning, Opus for agentic coding, Sonnet for balanced coding, Haiku for high-volume low-latency work. Fable is described as slower; its inclusion in either subscription is unverified. Higher capability or effort can consume allowance faster. Provider-reported comparative latency is not a measured CCF turnaround. [Selection](https://platform.claude.com/docs/en/about-claude/models/choosing-a-model), [Sonnet](https://platform.claude.com/docs/en/models/sonnet-5-5/overview), [Opus](https://platform.claude.com/docs/en/models/opus-5-5/overview), [Fable](https://platform.claude.com/docs/en/models/fable-5-1/overview), [Haiku](https://platform.claude.com/docs/en/models/haiku-5-5/overview).
* **DeepSeek V4.1 Flash:** first-party docs specify 1M context, 384K max output, image input, tool calls and thinking/non-thinking; API effort low/high/max. OpenCode gateway limits/snapshot may differ. Inclusion and availability in the Europe-configured Go workspace must be verified; a large allowance cannot prove low review effort or correctness.
* **MiMo:** Live selectable names are versioned, but historical CCF `MiMo` is not. The OpenCode [Go model table](https://opencode.ai/docs/go/) offers versioned MiMo routes. Historical version, multimodality and route-specific limits remain unresolved for this lane.

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
| Cost efficiency | depends on successful tasks | depends on task | depends on quota and task | Go allowance and success vary | included availability and success uncertain |

Independent same-harness results for these exact 2026 versions, budgets and CCF environment were not established; no numeric ranking is justified. [Terminal-Bench 2.1](https://www.tbench.ai/news/terminal-bench-2-1) revised 28 of 89 tasks; versions and scaffolds cannot be mixed. Its [integrity update](https://www.tbench.ai/news/leaderboard-integrity-update) identifies reward hacking and benchmark leakage. Anthropic's [infrastructure study](https://www.anthropic.com/research/infrastructure-noise) reports a six-point Terminal-Bench 2.0 spread across resource configurations in its own tests. DeepSeek's [1017 announcement](https://api-docs.deepseek.com/news/news261017/) describes its own mini-swe-v2 evaluation; this is vendor evidence, not a comparable independent CCF run. For research, math, retrieval and instruction following, no reproducible matched portfolio for these exact routes was established; do not extrapolate from terminal scores. Measure CCF success, elapsed time, retries and review burden on matched tasks before changing policy.

CCF observations are project-specific, not a benchmark: `Docs/Project/coordination/active-tasks.md` records RACK-V2-05 continuation after Sol, Terra and Claude A usage stops, with Claude B and an OpenCode watcher; the qualified candidate had 326 checks and 17 captures. It also records DeepSeek's RACK-V2-04 and PARITY-05 preflight work, MiMo's S1-C query foundation and Sonnet's parity audit/patch. Different scopes, collaborators and verification gates preclude causal rankings. Prefer a single writer, explicit handoff, exact commit and independent gates; a watcher observes and escalates rather than silently rewriting work.

## Subscription costs, limits and practical value

| Your subscription | Monthly cost | What it contributes to CCF | Limit and uncertainty |
|---|---:|---|---|
| Claude account 1 | €20 | Claude work, independent review or continuation | User confirms account and price; plan and live meter not inspected. |
| Claude account 2 | €20 | Additional Claude capacity and continuity | User confirms a second account. Separate account-level usage is expected, but actual limits, terms and T3 A/B mapping require checking. Do not assume exactly twice the useful work. |
| OpenAI account | €20 | ChatGPT and, where included, Codex work in the local lane | Plan name, remaining allowance and model entitlements not inspected. |
| OpenCode Go Europe | €10 | Included access to supported Go coding models for bounded work, checks and monitoring | Model availability depends on workspace settings and region; limits change. |
| **Total** | **€70** | Four paid subscriptions | **€840/year** at these stated unchanged monthly prices. |

The reported €20 prices are consistent with commonly advertised individual plans, but price alone does not verify a plan's name or entitlement. [Claude usage documentation](https://support.claude.com/en/articles/11647753-how-do-usage-and-length-limits-work) says paid Claude plans have a five-hour session limit and weekly limits, shared across Claude surfaces and Claude Code on the same account. Conversation length, model, features and effort affect use; no universal allowance in completed tasks is promised. Your two accounts may provide two separate account meters, but measure each and its reset timing rather than assuming identical capacity. Long context, high effort, tool output and retries can exhaust allowance early. Reserve the second account for continuity or an independent pass when useful; avoid sending both the same long context unless the review benefit warrants it.

For **ChatGPT Plus**, OpenAI states that the five-hour usage limit for ChatGPT-plan access is shared across supported apps; the user's exact plan and whether each T3 operation uses it remain unverified. [OpenAI account and session documentation](https://developers.openai.com/siwc/token-sharing-open-source/profiles-and-sessions). The CCF RACK-V2-05 handoff records Sol and Terra usage stops; it does not establish the exact pool or a predictable amount of work.

[OpenCode Go](https://opencode.ai/docs/go/) documents model-specific five-hour, weekly and monthly usage limits, and says model availability and limits can change. Its usage estimates are based on assumed request sizes and do not predict how many CCF tasks you can complete. The documented workspace settings require **Global** to enable DeepSeek V4.1 Flash; your Europe subscription alone does not prove that setting is enabled or that Global is acceptable to you. Use a model actually available under your existing settings; do not change region or privacy settings as part of this research. If Go's optional balance fallback or other extra usage is enabled, included-use assumptions may no longer describe your bill; the setting has not been inspected.

### How to judge subscription value

At these fixed monthly costs, additional included work has no separately stated per-task subscription charge while within the plan allowance. The practical scarce resources are quota, elapsed time, successful completion, rework and independent review. A €10 Go subscription can be excellent value for high-volume work if its enabled models solve and verify tasks reliably; a €20 frontier subscription can be better value for a hard task if it avoids enough retries or delay. Monthly subscription price alone cannot rank them, and one account's idle allowance does not automatically transfer to another.

For a fair comparison, record over a representative month: accepted tasks by size and type; which account/model/effort handled each; time to completion; quota consumed or limit/reset reached; failures and review/rework time. Compare completed and accepted CCF work per €20 Claude account, per €20 OpenAI account and per €10 Go account, while noting tasks differ in difficulty. Do not calculate a per-task value until enough comparable work exists. The yearly total above is a fixed-cost projection, not a usage or savings forecast.

## CCF routing proposal

Judgment below is conditional on availability and verified tools. Quality choices largely rely on provider positioning, not a matched independent comparison. Value considers limited quota and operational handoffs; actual cost per accepted task needs measurement. Fallback must inherit the exact packet, commit, ownership and outstanding gates.

| Task | Best quality | Best value / default | Fast / low quota use | Fallback | Avoid as default and why |
|---|---|---|---|---|---|
| Manager planning/spec/review | Astra or Fable if included | Sol | Terra | Opus | Flash-only final authority: high ambiguity needs review |
| Large Unity/C# implementation | Sol or Opus if included | Sol | Go model available in Europe for bounded parts | Sonnet or Terra continuation | Luna/Haiku solo on broad scope: insufficient CCF evidence |
| Bounded bugs | Sol | Terra | Available Go model | Sonnet | Fable for routine edits: scarce capacity |
| Git integration/publication | Sol | Terra with exact checks | Available Go model for read-only preflight | Sonnet | unattended cheap writer for conflicts: branch hygiene critical |
| Static/offline inspection | Opus | Go model with cited findings | Luna/Haiku | Terra | single model as sole acceptance gate |
| Deep research | Fable or Astra with web if included | Sol plus sourced collection | Go model for collection where web is supported | Sonnet | unsourced synthesis from any model |
| Overnight autonomous | Sol or Opus if included | Sonnet or Sol with checkpoints | Go model as bounded worker | Terra with explicit handoff | high effort without quota/reset plan |
| Adversarial review | Fable / Astra independent of writer, if included | alternate provider Sonnet / Sol | Go model cross-check | Opus or Terra | same writer as sole reviewer |
| Docs/context refresh | Sol | Terra | Go model or Haiku with checks | Sonnet | expensive frontier model for mechanical updates |
| Watcher/continuation | Sol for difficult recovery | Go model watcher; Terra continuation | Available Go model | Sonnet | autonomous second writer; conflicts and duplicated work |

Reserve both Claude subscriptions' available capacity for independent review or hard continuations; check each reset and weekly meter without assuming separate pools. Use lower effort on well-specified work, bounded file sets, summaries with exact SHAs and fresh context when appropriate. Escalate effort for ambiguous cross-system changes. Stop on limit, preserve state and transfer one writer; use event-driven watchers when available. Keep production gates and human/Manager acceptance independent of token savings. No purchasing, account or routing changes are authorized by this report.

## Uncertainties and follow-up

Account plan names, exact quota pools and model entitlements, region-enabled Go models, actual per-run usage, hosted snapshots, MiMo historical identity, `gpt-6x`, and cross-model benchmark comparability remain unverified. T3's catalog is not an entitlement test; its search is bounded. Official commercial pages are mutable; recheck before spending. The first cloud execution's full report was not accessible here, though #52 records its incomplete outcome. A useful next step is a small blinded CCF task portfolio with fixed briefs, tests, model/effort/harness, tokens, retries and reviewer time; this is a proposal, not an authorized project decision.

## Source index

All links embedded above were checked on 2026-10-28. Primary sources: [OpenAI models](https://developers.openai.com/api/docs/models), [OpenAI subscription usage](https://developers.openai.com/siwc/token-sharing-open-source/profiles-and-sessions), [Anthropic model selection](https://platform.claude.com/docs/en/about-claude/models/choosing-a-model), [Claude quota](https://support.claude.com/en/articles/11647753-how-do-usage-and-length-limits-work), and [OpenCode Go](https://opencode.ai/docs/go/). Independent methodology: [Terminal-Bench 2.1](https://www.tbench.ai/news/terminal-bench-2-1); provider methodology: [Anthropic infrastructure noise](https://www.anthropic.com/research/infrastructure-noise). Local evidence: app-server and T3 catalog discovery and `Docs/Project/coordination/active-tasks.md` at the starting commit.
