# CCF Model Capability / Subscription-Value / Routing Research — Independent Claude Replication (Phase A)

**Research cutoff and source-check date:** 2026-10-28
**Status:** Phase A — blind independent research. Advisory evidence only; no routing, purchasing or account change is made or implied by this document.
**Task:** `MODEL-CAPABILITY-COST-RESEARCH-CLAUDE-20261028` (claim issue #57)
**Starting repository HEAD:** `b70206fa69b2777a797105af2d65176b8b78779f` (`origin/main`, descendant of dispatch `8ed6838`)

## 0. Provenance, blindness and method limits

| Item | Value |
|---|---|
| Researcher | **Claude Sonnet 5.5** (`claude-sonnet-5-5`), as the session itself reports, run through the Claude Code harness (Agent SDK) hosted in T3 Code, on a Claude **software subscription** (no API key, no API billing). |
| Not used | Claude Opus 5.5 and Claude Fable 5.1 are named in the runtime catalogue but were **not** the session model; this report does not claim them. The session's reasoning-effort setting is not displayed to the researcher and is therefore **not recorded**. |
| Claude account used | **Not identifiable.** The runtime config directory is `/home/jer/.claude_ccf2`; that is a label, not proof it is "account 2". |
| Blindness | The OpenAI-produced report (`Model_Capability_Cost_Routing_Research_2026-10-28.md`), its Manager reviews, its worker handoff and PR #55/#56 content were **not opened, searched, quoted or inferred from**. The working tree was a sparse checkout with those paths absent from disk. The original task packet was read (permitted). |
| Web method | `WebSearch` and `WebFetch`. `WebFetch` returns an answer produced by a smaller summarising model over the page text, so figures below are **extraction-level**, not raw-page-verified. Where two extractions disagreed, the conflict is reported. Several official pages returned 403/404 (`chatgpt.com/pricing`, one Anthropic help article) and could not be read. |
| Currency | The user states €20 / €20 / €20 / €10. Provider pages show **USD** (Claude Pro $20/mo or $17 annual-equivalent; OpenCode Go $10, Go Plus $40) and say tax is excluded. EUR prices, VAT and the user's actual plan tier were **not verified**; the user's figures are used as given. |

### Evidence labels used throughout

| Label | Meaning |
|---|---|
| **[OFF]** | Official provider documentation / help page / model page read on 2026-10-28 |
| **[PROV]** | Provider-reported benchmark or claim (self-run; scaffolds, effort and comparison versions are provider-chosen) |
| **[IND]** | Independent measurement (Artificial Analysis, Vals.ai) |
| **[3P]** | Third-party article / aggregator / community; secondary |
| **[CCF]** | CCF project history: anecdotal, uncontrolled, project-specific |
| **[J]** | The researcher's own judgment |

Confidence: **H** high, **M** medium, **L** low.

---

## 1. Executive summary

1. **The binding constraint is subscription quota, not model capability.** In the single recorded night of RACK-V2-05 (claim #43), every premium pool was exhausted in about two hours of heavy agentic Unity work: Codex `gpt-6.1-sol` hit a usage limit at 00:29Z, Claude A hit its 5-hour session limit at 01:40Z, and Codex `gpt-6.1-terra` hit a limit at 02:20Z. The OpenCode/DeepSeek watcher reported no quota event in the same period. **[CCF]**, anecdotal, but consistent with the documented mechanics.
2. **Only one of the four subscriptions has publicly quantified allowances.** OpenCode Go publishes per-model dollar-equivalent allowances (5-hour 20%, weekly 50%, monthly 100%). Claude Pro publishes no numbers (only "five-hour session" + "weekly limit", shared across claude.ai, Claude Code and Desktop). ChatGPT Plus publishes per-5-hour *estimates* per model. **Quota cannot be converted to a common currency; any "tokens per euro" figure for Claude or OpenAI subscriptions would be invented.** **[OFF]** H.
3. **Model identities are mostly resolvable.** `claude-sonnet-5-5` (released 2026-09-28), `gpt-6.1-sol` (2026-09-29), `gpt-6.1-terra` (documented in OpenAI's Codex pages), `deepseek-v4.1-flash` (OpenCode Go model ID; release 2026-09-10, "1017" build 2026-10-17 per secondary sources) and `mimo-v2.6-pro` (OpenCode Go model ID) are all real, named routes. T3 Code, TTree Code, Codex, OpenCode and "Claude A/B" are **wrappers/lanes**, not models. **Unresolved:** which Claude account is "A" vs "B"; whether Fable 5.1 or Opus 5.5 is usable on the user's Pro plans; whether DeepSeek's Global-region requirement is satisfied by the user's "Europe" workspace (it evidently works in practice — see §10).
4. **"Strongest Claude" is not simply Fable.** By Anthropic's own table Opus 5.5 outscores Fable 5.1 on every listed coding/agentic benchmark, Artificial Analysis ranks Opus 5.5 first on all six capability domains shown (Fable 5.1 between 2nd and 8th), and Anthropic's own docs say Fable can **bill to usage credits** rather than the plan's included limits. For a €20 subscriber the practical ladder is Opus 5.5 → Sonnet 5.5, with Fable something to avoid unless usage credits are deliberately enabled. **[PROV][IND][OFF]** M.
5. **Sonnet 5.5 is the best quota-value Claude model for CCF's routine heavy work; Opus 5.5 is the reserve for hard, high-stakes tasks.** Anthropic reports Sonnet 5.5 at 70.6% on Terminal-Bench 4.0 versus Opus 5.5 at 66.4% (xhigh), but Opus 5.5 leads on FrontierCode, GDPval and SWE-style breadth and Anthropic itself says Opus is "clearly stronger at complex, open-ended work requiring sustained judgment". **[PROV]** M. Anthropic also warns that Sonnet 5.5 at **Max** effort scored *lower* than at xhigh on FrontierCode because it made out-of-scope changes — a direct argument against `max` for CCF's scope-bounded packets. **[PROV]** M-H.
6. **Recommended shape (judgment, §9):** Go/DeepSeek as the high-volume workhorse and watcher, scheduled off-peak; OpenAI Plus (Sol, with Terra for bounded work) as the first-choice Unity-capable implementer; the two Claude subscriptions held as a *primary + reserve pair* for independent review, hard reasoning, research and continuation — not split evenly by task. Keep Claude `usage credits` **off** (§8), because T3 Code is an Agent-SDK application and Claude Code bills Fable credits there without a consent prompt.
7. **Biggest evidence gaps:** no independent coding-agent scores found for Sonnet 5.5, GPT-6.1 Sol/Terra, or MiMo-V2.6-Pro (Artificial Analysis' coding-agent page exposed no extractable values); Claude Pro's actual weekly size is unpublished; the researcher could not read OpenAI's pricing page.

---

## 2. Model vs wrapper vs product mapping

| CCF name / lane | Product / wrapper | Underlying model | Provider | Access route | Pricing basis | Confidence |
|---|---|---|---|---|---|---|
| "Claude A" (`T3 Code / Claude A`) | T3 Code → Claude Code (Agent SDK) | `claude-sonnet-5-5` observed (xhigh) | Anthropic | Claude software subscription (plan tier **not recorded**; presumably Pro) | €20/mo (user), 5-hour + weekly pooled limits | M — model **[CCF]**; account mapping unverified |
| "Claude B Pro" (`claudeAgent_claude_b_pro`) | T3 Code → Claude Code | `claude-sonnet-5-5` observed (xhigh) | Anthropic | Claude **Pro** subscription (provider ID says Pro) | €20/mo (user) | M-H — provider ID **[CCF]** |
| "Claude subscription 1 / 2" (task packets) | Account labels | any model the plan exposes | Anthropic | Two separate accounts | €20 each (user) | **L** — packets themselves say not to assume A=1, B=2 |
| "Claude Opus 5.5" in `Scenario1FinalPresentationAcceptance` | Unrecorded wrapper | `claude-opus-5-5` (per prose) | Anthropic | **Unrecorded** account/plan | Unrecorded | L — evidence only that an Opus 5.5 session existed once **[CCF]** |
| "Claude Fable 5.1" | — | `claude-fable-5-1` | Anthropic | Not evidenced in CCF; plan access unclear | May bill usage credits **[OFF]** | n/a — see §10 |
| "Codex gpt-6.1-sol" | T3 Code → Codex harness ("configured Codex provider") | `gpt-6.1-sol` | OpenAI | ChatGPT software subscription (Plus per user) | €20/mo (user); per-5h estimates + weekly | H on model **[OFF]**, M on plan |
| "Codex gpt-6.1-terra" | T3 Code → Codex | `gpt-6.1-terra` | OpenAI | same | same | H model **[OFF]** (third-party "unreleased/leak" claims conflict with OpenAI's own pages — official wins) |
| "Astra" in `Scenario1FinalPresentationAcceptance` ("ForestFloorV1 (Astra)") | Asset-production lane label | **Not established** (may or may not be `gpt-6-astra`) | — | — | — | **Unresolved** |
| "Luna" | — | `gpt-6-luna` / `gpt-5.6-luna` exist | OpenAI | Plus (Codex) and OpenCode Go | — | No CCF usage evidence |
| "OpenCode / deepseek-v4.1-flash" | OpenCode (agent/harness, also run inside T3 Code) | `deepseek-v4.1-flash` (ID `opencode-go/deepseek-v4.1-flash`) | DeepSeek, served via OpenCode Go | OpenCode Go subscription | €10/mo (user); $10 on official page; dollar-equivalent allowances | H model/ID, **L** on region (§10) |
| "T3 Code / MiMo" | T3 Code → OpenCode Go | `mimo-v2.6-pro` (`opencode-go/mimo-v2.6-pro`) | Xiaomi, served via OpenCode Go | OpenCode Go | same | H — ID in two handoffs **[CCF]** + Go table **[OFF]** |
| "gpt-6x" | — | **Cannot be identified** | — | — | — | Unresolved; no evidence it is a distinct model |
| "TTree Code" | Wrapper/lane (appears as "TTree Code local lane", "TTree Code / Claude Sonnet 5.5") | Whatever it hosts | — | — | — | Treated as a wrapper only; **no public documentation found** |

**Wrapper/product names that are not models:** T3 Code, TTree Code, Codex, OpenCode, Claude Code, "Claude A/B", "subscription 1/2", "Game Dev agent", "watcher". **[J]** H.

---

## 3. Per-model strengths and weaknesses

### 3.1 Claude Sonnet 5.5 (`claude-sonnet-5-5`)

- **Facts [OFF]:** released 2026-09-28; 1M context, 128K max output; adaptive thinking (cannot be fully disabled; lowest setting `between_tools`); effort levels low/medium/high/xhigh/max; Claude Code default effort medium; latency "Fast"; tool use and image input; no web search is a property of the *harness*, not the model.
- **Strengths:** Anthropic-reported Terminal-Bench 4.0 70.6%, CursorBench 4.0 55.5%, OSWorld 2.1 80.1%, GDPval-AA 1844 Elo, HLE-with-tools 64.5% **[PROV]**. Customer-reported token efficiency gains versus Sonnet 5 (12–14% fewer tokens in two cases; a finance task at ~121k vs ~497k tokens). **[PROV]** L-M. In CCF it has carried long Unity qualification, requalification and repair tasks end to end (Combined-Repair-01, S1-C-IMPL-02 requalification, PARITY-04 preflight/Unity, RACK-V2-07). **[CCF]**
- **Weaknesses:** Anthropic states Opus 5.5 "remains clearly stronger at complex, open-ended work requiring sustained judgment". At **Max** effort it over-reached (out-of-scope changes; subagent code review that timed out), lowering FrontierCode vs xhigh. Anthropic: "Sonnet 5.5 may have tendencies we haven't found." Higher-risk cyber tasks fall back to Sonnet 5. **[PROV]** M-H. CCF: it, like all pooled Claude routes, hit the 5-hour limit within ~70 minutes of heavy work in RACK-V2-05. **[CCF]**

### 3.2 Claude Opus 5.5 (`claude-opus-5-5`)

- **Facts [OFF]:** released 2026-09-22; 1M context; 128K output; adaptive thinking *always on*; default effort medium; latency "Moderate"; positioned "for long-running agentic coding and knowledge work". Per Claude Code docs, **Opus 5.5 is the default model on Pro/Max/Team/Enterprise** (v ≥ 2.1.280).
- **Strengths:** Anthropic-reported Terminal-Bench 4.0 66.4% (xhigh), FrontierCode v1.1 54.4%, CursorBench 4.0 57.8%, GDPval-AA 1846 Elo, HLE-with-tools 67.7%, OSWorld 2.1 81.8% **[PROV]**. Artificial Analysis capability table: **rank 1 in all six domains shown** (e.g. Engineering 65 vs Astra 62, Sol 59, Sonnet 5.5 58, Fable 5.1 57) **[IND]** — extraction-level, "Max" settings. A claimed 40% cost reduction vs Opus 5 through fewer tokens per task **[PROV]** L-M.
- **Weaknesses:** Anthropic: "benchmark margins have become a less reliable guide to real-world differences"; the model "often suspects it is being evaluated"; cyber tasks are re-routed to Opus 4.8 unless verified, and AutomationBench counted safeguard interventions as failures. **[PROV]** Quota impact on a €20 plan versus Sonnet is **not published**; the Claude Code docs only say model choice affects usage. **Unknown, L.**

### 3.3 Claude Fable 5.1 (`claude-fable-5-1`)

- **Facts [OFF]:** "for demanding reasoning and long-horizon agentic work"; latency "Slower"; always-on adaptive thinking; **not the default on any plan**; "Depending on your plan and seat tier, Fable usage can bill to usage credits instead of drawing on your plan's included limits"; **Max plans have a separate weekly Fable limit**.
- **Evidence on quality:** In Anthropic's own Opus 5.5 table Fable 5.1 trails Opus 5.5 on Terminal-Bench 4.0 (55.8 vs 66.4), FrontierCode (50.3 vs 54.4), CursorBench (51.8 vs 57.8), HLE-with-tools; AA ranks it 2nd–8th by domain **[PROV][IND]**. Anthropic says its real-world gap to Opus 5.5 is narrower than the table suggests. **[3P via LLM Stats summary]** L.
- **Implication:** Not evidenced as a better CCF engineering model, probably not covered by a €20 plan's included limits. **[J]** M.

### 3.4 OpenAI GPT-6.1 Sol (`gpt-6.1-sol`)

- **Facts [OFF]:** released 2026-09-29 at DevDay; "near-Astra performance for complex work at a lower cost than Astra"; available to Plus/Pro/Business/Enterprise/Edu in Work and Codex (not Chat); 1.05M-token context and low→max reasoning effort per secondary reporting **[3P]**.
- **Strengths [PROV]:** OpenAI reports DeepSWE v1.1 parity with Astra, OSWorld 2.0 within 2.1 points of Astra, AutomationBench 2.2 points above Opus 5.5 at medium effort. Artificial Analysis states Intelligence Index 1 point below Astra at under a quarter of the cost per task — attributed to AA by a secondary article **[3P→IND]**. AA capability table: Engineering 59 (3rd). **[IND]** CCF: implemented RACK-V2-04A and executed Unity gates; started the final presentation review. **[CCF]**
- **Weaknesses:** OpenAI's own caveat that Astra still leads Terminal-Bench Science and the hardest research tasks; comparison values for older models "may come from later versions". **[PROV]** On Plus, Sol allows only an *estimated* 15–160 messages per 5 hours (a wide range), and CCF hit the limit "after workspace preparation only" in RACK-V2-05. **[OFF][CCF]**

### 3.5 OpenAI GPT-6.1 Terra (`gpt-6.1-terra`)

- **Facts [OFF]:** "a fast, efficient model for clear writing, visual understanding, and computer use" (note: **coding is not in OpenAI's own one-line description**); Codex CLI ≥ 0.159.0; available in Work/Codex on Plus and above; Plus estimate 35–285 messages per 5 hours; credit rate about half of Sol's.
- **Evidence:** AA capability table places Terra 8th–11th (Engineering 54) **[IND]**; no OpenAI coding benchmark found for Terra. CCF used Terra xhigh as a primary implementer (RACK-V2-05 attempt, RACK-V2-06 full task including Unity qualification). **[CCF]**
- **Identity note:** Several third-party sites describe "GPT-6.1 Terra" as an unreleased code-repo leak; OpenAI's current Codex pages list it with an ID, usage estimate and credit rate. The official pages are authoritative and more recent than those posts. **Factual conflict resolved in favour of [OFF].**
- **Weakness:** The independent benchmark gap to Sol, with only ~2x the Plus message headroom, makes it a *bounded-task* model. **[J]** M.

### 3.6 OpenAI GPT-6 Astra / Luna (context only)

- Astra (`gpt-6-astra`) is OpenAI's flagship; Plus gives an estimated **5–45** messages per 5 hours — realistically a single hard review per window. Luna (`gpt-6-luna`) is the high-volume small model (350–3,000/5h). No evidence of CCF use of either as a model. **[OFF]**

### 3.7 DeepSeek V4.1 Flash (`deepseek-v4.1-flash`, via OpenCode Go)

- **Facts:** OpenCode Go model ID `deepseek-v4.1-flash` **[OFF]**; release 2026-09-10, "1017" update 2026-10-17, MIT weights, up to 1M context and 384K output, parameter count conflicting (748B vs 552B) **[3P]**. Peak/off-peak pricing; peak = 2× off-peak.
- **Strengths [IND]:** Vals.ai (1017 build): Vals Index 61.26% ±1.05; Vibe Code Bench v1.1 87.73%; Code Migration v1.1 67.52%. **[PROV]** DeepSeek reports DeepSWE v1.1 77.0.
- **Weaknesses [IND]:** Vals Terminal-Bench 4.0 31.82% (rank 20/56) — compare DeepSeek's own 42.3% **using Claude Code as the harness**: a ten-point scaffold sensitivity. AA capability table: bottom of the set (Engineering 37 vs 58–65 for the Claude/OpenAI flagships). HLE 36.8 vs Opus 5's 56.3 **[PROV]**. **Rank conflict:** one extraction said "rank 9/28" on Vals, a secondary source said "#14 of 50"; treat the ranking as unreliable, the score as indicative.
- **CCF record:** substantial and mostly successful: MFM-01 qualification, RACK-V2-04 map planning mode implementation, RACK-V2-OPEN3 doc sync, PARITY-05 preflight, MFM-02 final Unity qualification and publication, the standing overnight watcher. **[CCF]** One credential/endpoint failure (`opencode/` Zen endpoint vs `opencode-go/`). **[CCF]**

### 3.8 MiMo-V2.6-Pro (`mimo-v2.6-pro`, via OpenCode Go)

- **Facts:** Go-table price/allowance entries; 1.02T-parameter sparse MoE, 42B active, ~1M context (Hugging Face / OpenRouter, **[3P]**), released ~2026-09-21. Xiaomi-reported Terminal Bench 2.1 89.9, OSWorld-Verified 82.0, Toolathlon 76.9 **[PROV, unchecked at primary source]**.
- **Independent evidence:** AA Intelligence Index 46.3 (via an OpenRouter summary) **[3P]**; absent from the AA capability table fetched. One review: near frontier on general agentic/tool use, behind on the hardest long-horizon coding. **[3P]** L.
- **CCF record:** S1-C-IMPL-01 (bounded C# and offline tests, completed), S1-C review/history spec author. **[CCF]**
- **Weakness:** Go allowance for MiMo-V2.6-Pro is **$15/month** (≈16,300 est. requests) versus $80 for DeepSeek V4.1 Flash — it burns the Go pool ~5× faster. The Go page lists "MiMo Code" (a client, not the model) as having problematic session support. **[OFF]**
- **Identity:** the CCF alias "MiMo" is sufficient in two handoffs (`opencode-go mimo-v2.6-pro`) but older/other MiMo lanes (V2.5, Flash) exist in Go; do not generalise.

---

## 4. Capability matrix (qualitative, with evidence labels)

Grades: ●●● strong · ●● solid · ● adequate/weak · ? unknown. Labels in brackets give the *basis*, not a measure of the grade.

| Dimension | Opus 5.5 | Sonnet 5.5 | Fable 5.1 | GPT-6.1 Sol | GPT-6.1 Terra | DeepSeek V4.1 Flash | MiMo-V2.6-Pro |
|---|---|---|---|---|---|---|---|
| Repo coding / SWE | ●●● [PROV][IND] | ●●● [PROV][IND] | ●●● [PROV][IND] | ●●● [PROV][IND] | ●● [IND] | ●● [PROV][IND] | ●● [PROV][3P] |
| Agentic terminal / tool use | ●●● [PROV] | ●●● [PROV] | ●●● [PROV] | ●●● [PROV] | ●● ? | ● (scaffold-sensitive: ~10-point swing between harnesses) [IND][PROV] | ●● [PROV] |
| Debugging / sustained judgment | ●●● [PROV][J] | ●● [PROV] | ●●● [PROV] | ●●● [J] | ● [J] | ● [J] | ● [J] |
| Long context (1M) | ●●● [OFF] | ●●● [OFF] | ●●● [OFF] | ●●● (≈1.05M) [3P] | ? | ●● (1M claimed) [3P] | ●● (~1M) [3P] |
| Reasoning / science | ●●● [PROV] | ●●● [PROV] | ●●● [PROV] | ●●● [PROV] | ●● [IND] | ● [PROV] | ●● [PROV] |
| Research synthesis (with web tool) | ●●● [PROV: HLE-tools] | ●●● [PROV] | ●●● | ●●● [J] | ●● [J] | ● [J] | ● [J] |
| Instruction following / scope control | ●●● [J] | ●● (Max effort over-reach) [PROV] | ●●● [J] | ●●● [J] | ●● [J] | ●● [CCF] | ●● [CCF] |
| Multimodality / computer use | ●●● OSWorld 81.8 [PROV] | ●●● OSWorld 80.1 [PROV] | ●●● 80.7 [PROV] | ●●● [PROV] | ●●● (documented role) [OFF] | ● (V4.1 Flash has "native vision" per secondary) [3P] | ●● OSWorld-V 82.0 [PROV] |
| Latency | Moderate [OFF] | Fast [OFF] | Slower [OFF] | not found | "fast" [OFF] | ? Vals 39m55s per Vals Index run [IND] | ? |
| Quota efficiency (this portfolio) | ? unpublished | ? unpublished | worse, if credits | tight on Plus (15–160/5h) [OFF] | better (35–285) [OFF] | best, quantified [OFF] | moderate [OFF] |
| Long autonomous reliability | ●●● [PROV][J] | ●● [CCF] | ●●● [J] | ●● [CCF] | ● [CCF] | ●● watcher/overnight [CCF] | ? |

**Important cross-family caveat.** Provider tables compare against *selected* rivals and versions (e.g. Anthropic's Opus 5.5 table compares with GPT-5.6 Sol, not GPT-6.1 Sol; OpenAI compares Sol with Opus "with fallbacks"). Neither Terminal-Bench scaffold nor effort setting is equalised. **No cross-family ranking is supported at better than "tier" precision.** **[J]** H.

**Benchmark disagreement worth noting:** on Terminal-Bench 4.0, Anthropic reports Sonnet 5.5 (70.6) > Opus 5.5 (66.4 at xhigh) > Astra (57.9, at high per OpenAI), whereas on AA's capability table Opus 5.5 (65) > Sonnet 5.5 (58) in Engineering. These are different tasks, scaffolds and effort budgets; they do not contradict each other, but they do mean "Sonnet 5.5 is the cheaper model that is *close enough* on agentic terminal work" is evidence-supported while "Sonnet 5.5 is as good as Opus 5.5" is not.

---

## 5. Software subscription and usage-limit matrix (not API economics)

| Route | User-stated cost | Window mechanics | Quantified? | Model gating | At the limit | Notes / source |
|---|---|---|---|---|---|---|
| **Claude Pro ×2** (separate accounts) | €20 each | Rolling **5-hour session** limit + **weekly** limit, applying across all models; **shared across claude.ai, Claude Desktop and Claude Code** per account. Pro gets a one-time-per-week "wrap-up" allowance after hitting the session limit mid-response. | **No.** "Not a fixed message count"; depends on conversation size, features, model, effort. A community figure (~$157 API-equivalent/week) exists but I could not verify it. | Opus 5.5 is Claude Code's default on Pro per docs; Fable not default and may bill credits; Max-only separate Fable weekly cap; legacy 1M-context variants (Opus/Sonnet 4.6) need credits on Pro. | Wait for reset, enable **usage credits** (pay-as-you-go at API rates, spend cap you set), or upgrade. Two accounts = two independent pools (not shared) **[J]** — but see §8 on terms. | **[OFF]** support.claude.com, code.claude.com, claude.com/pricing; Anthropic doubled Claude Code 5-hour limits and removed the Pro/Max peak-hour reduction on 2026-05-06 **[OFF]** |
| **ChatGPT Plus** (Codex/Work) | €20 | 5-hour window (reinstated for Plus 2026-08-25 per secondary reports) + weekly limit; local messages and cloud tasks draw on the same allowance; cloud tasks cost more. | **Estimates per 5 h (Plus):** Astra 5–45, **Sol 15–160**, **Terra 35–285**, GPT-6 Sol 15–150, Luna 350–3,000. | Sol/Terra available in Work and Codex (not Chat). | Wait, or **buy credits** (Sol 50/2.5/250, Terra 25/1.25/125 credits per 1M in/cached/out). | **[OFF]** learn.chatgpt.com/docs/pricing, /models; 5-hour return **[3P]**. Plus price/VAT not verified (403). |
| **OpenCode Go (Europe)** | €10 | Dollar-equivalent allowances per model: **5-hour = 20%**, **weekly = 50%**, **monthly = 100%** of the model's monthly limit. DeepSeek V4.1 Flash **and V4 Pro** peak hours 01:00–04:00 and 06:00–10:00 UTC Mon–Fri at 2× price; otherwise off-peak. | **Yes.** DeepSeek V4.1 Flash: $80 monthly / $40 weekly / $16 per 5 h (≈173,330 est. requests/month). MiMo-V2.6-Pro: $15 / $7.50 / $3 (≈16,300). DeepSeek V4 Pro: $15. Plan list price $10; Go Plus $40. | Requires **Global** region setting for DeepSeek V4.1 Flash / V4 Pro (§10). | "You can continue using the free models"; optional Extra Usage from a Console balance. | **[OFF]** opencode.ai/docs/go. DeepSeek ZDR agreement "valid through October 31, 2026" (renewed monthly). |

**Comparison caveats:** 5-hour/weekly units differ in kind (opaque pools vs messages vs dollar-equivalents). Never convert Claude or ChatGPT quota to tokens or euros. **[J]** H.

---

## 6. Subscription-value analysis for the €70/month portfolio (€840/year)

**Metric adopted:** accepted CCF work per euro, net of quota burn, rework, delay and review overhead. No dataset exists to compute it; the CCF history gives only anecdotes (§6.3).

### 6.1 Spend structure

| Pool | € / month | Share |
|---|---|---|
| Claude (2 × €20) | 40 | 57% |
| OpenAI Plus | 20 | 29% |
| OpenCode Go | 10 | 14% |
| **Total** | **70** | 100% |

### 6.2 What the evidence supports

1. **Go is the only pool whose allowance is both large and legible.** DeepSeek V4.1 Flash's documented monthly allowance is $80 for a €10 plan — the single clearest "volume per euro" fact available. **[OFF]** H. Quality per task is lower (AA/Vals) so volume only converts to accepted work when tasks are *well-specified and verifiable* (static checks, bounded fixes, watchers). **[J]** M.
2. **The two Claude pools are the best known "quality reserve" but are unmetered to the user.** Their value depends on how the *marginal* hour of Claude work is spent. Anthropic's own cost guidance: subagents and agent teams (~7× tokens in plan mode), long contexts, cache misses after idle gaps, scheduled `/loop` tasks, goal check-ins and cross-session messages all burn plan usage invisibly. **[OFF]** H.
3. **OpenAI Plus has the tightest published numbers for the flagship routes.** Sol 15–160 and Astra 5–45 per 5 h make Plus a *burst* resource. Terra and Luna are the headroom models but Terra's independent evidence is weakest of the OpenAI set. **[OFF][IND]**
4. **A second Claude subscription doubles reachable quota only if the two pools are actually independent and used in sequence.** Evidence of independence: in RACK-V2-05, Claude A was limited at 01:40Z while Claude B Pro then ran from 02:31Z. **[CCF]** M. Whether running two personal accounts is within Anthropic's terms is outside this study; secondary reports say multiple accounts are not prohibited as such but limit evasion/sharing is the risk area (§8). **[3P]** L.
5. **Untested alternatives (no recommendation):** Go Plus ($40) quadruples most Go allowances; a third Claude pool; Claude Max. The evidence here cannot rank these against the current portfolio because no per-euro acceptance data exists.

### 6.3 CCF operational evidence (anecdotal, uncontrolled)

| Task | Models in sequence (as recorded) | Outcome notes |
|---|---|---|
| RACK-V2-05 (claim #43) | Codex `gpt-6.1-sol` → Claude A Sonnet 5.5 xhigh → Codex `gpt-6.1-terra` xhigh → Claude B Pro Sonnet 5.5 xhigh; DeepSeek watcher throughout | "Provider limits, not task problems, caused every transition"; passing editing gate achieved by Terra; no work lost. |
| RACK-V2-06 | Codex `gpt-6.1-terra` xhigh | Completed with Unity qualification. |
| RACK-V2-04A | Codex `gpt-6.1-sol` xhigh | Completed; focused Unity operator. |
| RACK-V2-04 | `opencode-go/deepseek-v4.1-flash` | Completed. |
| RACK-V2-07 | Claude B Pro Sonnet 5.5 by user instruction (claim preferred Terra); DeepSeek helper | Handed off; G3 item noted in project memory. |
| RACK-V2-Combined-Repair-01 | Sonnet 5.5 (claim recommended Sol) | Qualified. |
| S1-C-IMPL-01 / S1-C spec | MiMo `mimo-v2.6-pro` | Bounded C# + offline tests; spec authored. |
| MFM-01 / MFM-02 / PARITY-05 / OPEN3 | DeepSeek V4.1 Flash (MFM-02 implemented by Sonnet 5.5 then qualified by DeepSeek) | Completed. |
| Scenario 1 final presentation acceptance | Sol 6.1 → "Claude Opus 5.5 completed it after Sol's session ended" | Completed; the only evidence of an Opus 5.5 session; plan unrecorded. |

**Reading these honestly [J]:** (a) every premium pool was *limit-bound* on heavy Unity work; (b) cheap-model lanes completed substantial tasks when the specification was tight; (c) the "Terra completed the passing run, then hit its limit before reporting" and "Sol hit the limit after workspace preparation only" incidents show that **quota is lost to overhead (workspace setup, large context) before any useful output**; (d) no task shows a model *failing on capability* where another succeeded, so capability is not shown to be the bottleneck. This is weak evidence but consistent with §1.1.

---

## 7. CCF routing matrix

Columns: **Lead** (best quality) · **Value** (best accepted-work per quota) · **Fast/cheap** · **Review/Support** · **Avoid as default**. Bases: **[J]** unless a stronger label is shown. Recommendations are advisory and do not change Manager routing.

| Task class | Lead | Value | Fast/cheap | Fallback / support | Avoid as default (reason) |
|---|---|---|---|---|---|
| Manager planning / spec / review | Claude Opus 5.5 (if the account exposes it) or Sol (high) | Claude Sonnet 5.5 high | DeepSeek for drafts only | Alternate Claude account; Sol | Fable (credits risk, no coding edge); DeepSeek alone (judgment depth) |
| Large Unity / C# implementation | Sol (high/xhigh) with Unity tooling [CCF] | Sol → Sonnet 5.5 xhigh by pool freshness | Terra for bounded slices | Sonnet 5.5 xhigh on the other vendor's pool | Astra (5–45/5h on Plus); `max` effort on Sonnet (scope over-reach [PROV]); DeepSeek for architecture-level work |
| Bounded bug fixes | DeepSeek V4.1 Flash [CCF] | DeepSeek; MiMo-V2.6-Pro | Luna/Haiku-class (unverified) | Terra → Sonnet 5.5 after two failed attempts | Opus/Sol for trivial fixes (waste of scarce quota) |
| Git integration / rebase / publication | DeepSeek (mechanical) [CCF] | DeepSeek | DeepSeek | Sonnet 5.5 for non-trivial conflicts, especially Unity YAML | Any model doing mechanical conflict resolution of serialized Unity files without a human/spec check |
| Static / offline code inspection | DeepSeek or MiMo [CCF] | DeepSeek (very large allowance) | DeepSeek | Sonnet 5.5 as independent second pass | Flagship models (cost with no measured benefit) |
| Deep research / evidence synthesis | Opus 5.5 / Sonnet 5.5 high with web tools | Sonnet 5.5 high | Not applicable (weak evidence for cheap models with web) | OpenAI Work/Sol as cross-check | DeepSeek/MiMo for sourced claims (no evidence of reliable citation behaviour) |
| Long overnight autonomous runs | Sol on Plus (Codex) if quota is fresh [CCF] | DeepSeek in **off-peak** window (04:00–06:00, 10:00–01:00 UTC, weekends) with watcher | DeepSeek | Claude B reserve (§8) | A Claude session left with `/loop`, goal check-ins or cross-session messages as the "watcher" (invisible quota burn [OFF]) |
| Independent review / adversarial checking | Opus 5.5 (high stakes); Sonnet 5.5 xhigh default [CCF policy] | Sonnet 5.5 high | DeepSeek first-pass lint-style review | **Different family from the author** [J] | Same-family review of own work |
| Documentation / context refresh | DeepSeek / MiMo [CCF] | DeepSeek | DeepSeek | Sonnet 5.5 for canonical spec text | Flagship models |
| Watcher / continuation | `opencode-go/deepseek-v4.1-flash` [CCF] | same | same | MiMo (higher Go burn); Haiku 5.5 via Go (unverified) | `opencode/` Zen endpoint after the recorded credential failure unless credits exist [CCF] |

---

## 8. Quota-preservation strategy

**Claude (two accounts)**
1. Treat the pair as **primary + reserve**, not parallel halves. Run one account until near its session/weekly ceiling, then switch; read `/usage` before and after long tasks to learn the real burn. **[J]**
2. **Default to Sonnet 5.5 at medium–high; reserve xhigh for qualification-grade work; avoid `max`.** Opus 5.5 only for high-stakes review/planning where the extra capability is documented to matter. **[PROV][J]**
3. **Keep usage credits off or tightly capped.** Claude Code docs: with Fable and credits enabled, non-interactive (`-p`) and Agent-SDK sessions that don't show the consent prompt bill credits **without asking**. T3 Code is an Agent-SDK harness. Never pin `fable`/`best` in automation. I did not check whether credits are on for either account. **[OFF]**
4. Reduce silent burn: `/clear` between unrelated tasks, short CLAUDE.md, no unattended `/loop` or goal check-ins, avoid agent teams (~7× tokens), delegate verbose output to cheaper subagents, keep prompts specific, plan before implementing. **[OFF]**
5. Do not use Claude as the standing watcher; use Go.

**OpenAI Plus**
6. Use Sol for the hard Unity slices, Terra for bounded work, **avoid Astra** (5–45/5h). Prepare workspaces with a cheap model first, because overhead can consume a window before the first edit (RACK-V2-05 attempt 1). **[CCF][J]**
7. Cloud tasks cost more than local messages; prefer local for CCF's local-Unity work. **[OFF]**

**OpenCode Go**
8. Prefer DeepSeek V4.1 Flash (largest allowance) over MiMo-V2.6-Pro (≈5× faster burn) unless MiMo is measurably better on the task. Schedule long runs **off-peak** (peak = 2× burn on 01:00–04:00 and 06:00–10:00 UTC weekdays — the user's mornings; Dublin is on UTC since 2026-10-25). **[OFF][J]**
9. Watch the **DeepSeek ZDR agreement expiry (2026-10-31)**; availability after renewal is unverified. **[OFF]**

**Cross-cutting**
10. Hand off at *clean boundaries* (tree clean, pushed) — the RACK-V2-05 practice lost no work across four provider transitions. **[CCF]**
11. Log, per task: pool used, `/usage` percentage before/after (Claude), message-count estimate (OpenAI), allowance cost (`opencode.allowance`, Go) and accepted/rework outcome. This is the only way to turn this report's judgments into evidence.

---

## 9. Recommended default / fallback / watcher policy (advisory)

| Class | Default | Fallback 1 | Fallback 2 | Watcher |
|---|---|---|---|---|
| Unity/C# heavy implementation | Codex `gpt-6.1-sol` high | Claude Sonnet 5.5 xhigh (reserve account) | Codex `gpt-6.1-terra` xhigh (slices) | DeepSeek |
| Bounded fixes, docs, static inspection, Git mechanics | DeepSeek V4.1 Flash | MiMo-V2.6-Pro | Terra / Sonnet 5.5 | DeepSeek |
| Independent review (G3-type) | Claude Sonnet 5.5 xhigh (other vendor than author) | Opus 5.5 for high stakes | Sol | n/a |
| Manager planning / research | Claude Sonnet 5.5 high (Opus 5.5 if available and justified) | Sol | Other Claude account | n/a |
| Overnight runs | DeepSeek off-peak + watcher; Sol only for hard slices | Claude reserve | Terra | DeepSeek |

This is a restatement of what CCF already does in practice, tightened by the quota facts above. **No change to Manager routing rules is proposed or made by this document.** **[J]**

---

## 10. Uncertainties and unresolved identities

1. **Claude account mapping.** Whether "Claude A/B" = subscription 1/2, and A's plan tier, are unrecorded. B is labelled "Pro" in its provider ID. **Open.**
2. **Model access on the user's Pro plans.** Claude Code docs say Opus 5.5 is Pro's default; CCF shows Sonnet 5.5 chosen by the user and one unattributed Opus 5.5 session. Fable 5.1 on Pro is unresolved (docs vague; secondary sources say credits only). **Open.**
3. **Quota size** of Claude Pro (weekly) and the exact meaning of ChatGPT Plus' 15–160 estimate. **Unpublished/wide.**
4. **OpenCode region.** The Go page says DeepSeek V4.1 Flash/V4 Pro require **Global** region; "default allowed regions are Europe & United States"; there is no "Europe plan" on the page. CCF runs `opencode-go/deepseek-v4.1-flash` routinely, so access works in practice for the user's workspace. Whether this is because Global is enabled, an earlier setting, or another reason is **unverified**; no settings were inspected or changed. If the "Europe" label means a data-residency constraint, DeepSeek serving may not honour it — **flag for the user, not for me to resolve.**
5. **EUR/VAT pricing** of all four subscriptions: unverified.
6. **`gpt-6x`:** no model identity established. **"Astra"** (CCF asset label) vs `gpt-6-astra`: not established. **"TTree Code":** no public documentation.
7. **Benchmarks:** no independent coding-agent scores found for Sonnet 5.5, GPT-6.1 Sol/Terra, MiMo-V2.6-Pro; AA's Coding Agent Index page exposed no values to extraction; one AA table I relied on came through a summarising model and uses "Max" effort for all entries. Vals rank figures conflicted (9/28 vs 14/50).
8. **Conflicts between sources found:** "GPT-6.1 Terra unreleased" (3P) vs official pages (official preferred); Sonnet 5.5 default effort (API: high; Claude Code/announcement: medium); Anthropic help-article search-snippet "13 days old" vs the article's own date (2026-05-06); OpenCode third-party "$60" figures vs the official page's per-model table.
9. **Multiple-account terms.** Only secondary sources; the official Consumer Terms were not read. Not a legal conclusion.
10. **ChatGPT Plus 5-hour window** (reinstated 2026-08-25) is from news coverage; the official page confirms "per five hours" estimates and weekly limits but I did not read an official statement of the reinstatement.

---

## 11. Sources (all checked 2026-10-28)

**Official / [OFF]**
- Anthropic, Claude models overview — https://platform.claude.com/docs/en/about-claude/models/overview
- Claude Sonnet 5.5 model page — https://platform.claude.com/docs/en/models/sonnet-5-5/overview
- Claude Opus 5.5 model page — https://platform.claude.com/docs/en/models/opus-5-5/overview
- Anthropic, Claude Opus 5.5 announcement — https://www.anthropic.com/claude-opus-5-5
- Anthropic, Claude Sonnet 5.5 announcement — https://www.anthropic.com/claude-sonnet-5-5
- Claude Code, model configuration — https://code.claude.com/docs/en/model-config
- Claude Code, manage costs — https://code.claude.com/docs/en/costs
- Claude Help Center, using Claude Code with Pro/Max — https://support.claude.com/en/articles/11145838-using-claude-code-with-your-pro-or-max-plan
- Claude Help Center, What is the Pro plan — https://support.claude.com/en/articles/8325606-what-is-the-pro-plan
- Claude Help Center, How do usage and length limits work — https://support.claude.com/en/articles/11647753-how-do-usage-and-length-limits-work
- Claude pricing — https://claude.com/pricing
- Anthropic news, higher limits (2026-05-06) — https://www.anthropic.com/news/higher-limits-spacex
- OpenAI, Codex/ChatGPT plan usage and credit rates — https://learn.chatgpt.com/docs/pricing (redirect of developers.openai.com/codex/pricing)
- OpenAI, Codex models — https://learn.chatgpt.com/docs/models (redirect of developers.openai.com/codex/models)
- OpenCode Go docs — https://opencode.ai/docs/go/

**Independent [IND]**
- Artificial Analysis (Capability Index Comparison table; Intelligence Index v5.0 noted) — https://artificialanalysis.ai/
- Artificial Analysis Coding Agent Index page (no extractable scores) — https://artificialanalysis.ai/agents/coding-agents
- Vals.ai, DeepSeek V4.1 Flash 1017 — https://www.vals.ai/models/deepseek_deepseek-v4.1-flash-1017

**Third-party / aggregators [3P]**
- TestingCatalog, GPT-6.1 Sol launch — https://testingcatalog.com/openai-launches-gpt-6-1-sol-at-one-fifth-of-astra-pricing.md
- Unite.ai, GPT-6.1 Sol at DevDay — https://www.unite.ai/openai-unveils-gpt-6.1-sol-at-devday-with-new-codex-and-chatgpt-tools/
- Vellum, Opus 5.5 benchmarks explained — https://www.vellum.ai/blog/claude-opus-5-5-benchmarks-explained
- ComputingForGeeks, Opus 5.5 — https://computingforgeeks.com/claude-opus-5-5-released-features-benchmarks/
- Apidog, Sonnet 5.5 benchmarks — https://apidog.com/es/blog/claude-sonnet-5-5-benchmarks/
- DataLearner / OrcaRouter / kie.ai on "GPT-6.1 Terra" (conflicting; superseded by official pages) — https://www.datalearner.com/ai-models/pretrained-models/gpt-6-1-terra
- The Next Web / ProPakistani / Pasquale Pillitteri on DeepSeek V4.1 Flash — https://thenextweb.com/news/deepseek-v4-1-flash-launch-v4-pro-retired-price-cut
- OpenRouter / Codersera on MiMo-V2.6-Pro — https://openrouter.ai/xiaomi/mimo-v2.6-pro
- AiWeekly / Notebookcheck on ChatGPT Plus 5-hour cap — https://aiweekly.co/alerts/openai-brings-back-5-hour-codex-work-cap-for-chatgpt-plus
- Metricnexus and others on multi-account bans — https://metricnexus.ai/blog/anthropic-banning-multiple-claude-accounts
- Search aggregator listings (benchlm.ai, wokey.ai, modelgrep.com) surfaced in results but **not relied on**.

**CCF project evidence [CCF]** (all in this repository at `b70206f`)
- `Docs/Handoffs/RACK-V2-05/HANDOFF.md` (provider transition log), `RACK-V2-04A`, `RACK-V2-04`, `RACK-V2-06`, `RACK-V2-07`, `RACK-V2-Combined-Repair-01`, `RACK-V2-OPEN3-Authority-Sync`, `MFM-Implementation-01/02`, `S1-C-Implementation-01/02`, `S1-ThreeSpeciesParity-P0-Implementation-04*`, `Docs/Scenario1FinalPresentationAcceptance.md`, `Docs/Project/AgentWorkflow.md` (routing labels), `Docs/Project/coordination/active-tasks.md`.
