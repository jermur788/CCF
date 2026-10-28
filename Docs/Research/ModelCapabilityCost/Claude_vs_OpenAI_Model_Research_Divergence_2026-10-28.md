# Claude vs OpenAI Model Research — Divergence Analysis (Phase B)

**Date:** 2026-10-28
**Task:** `MODEL-CAPABILITY-COST-RESEARCH-CLAUDE-20261028` (claim issue #57)
**Compared documents**

| | Document | Produced by (as each document states or the repo records) |
|---|---|---|
| **Claude report** | `Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md` — **frozen at `3190b08b7016eaf93c87e7dc21e25728b355ef39`** (Phase A, written blind) | Claude Sonnet 5.5, Claude Code in T3 Code |
| **OpenAI report** | `Model_Capability_Cost_Routing_Research_2026-10-28.md` (103 lines; subscription-focused amendment) | An OpenAI lane — the report states its executor was `codex` / `gpt-6.1-terra`, high effort |

**Purpose.** Show where two independent researchers converge and diverge. It does **not** declare a winner, and it changes no routing, purchasing or account setting.

**Disclosure of my position.** I am a Claude model comparing a Claude report against an OpenAI report. I also wrote Phase A and have an interest in it looking sound. I have tried to read it adversarially, but the reader should weight my judgments about *which report is better* below my judgments about *what each report contains*. The OpenAI report was authored inside the OpenAI family, so both reports share the same structural bias risk.

**Phase A is not rewritten.** The only changes to Phase A conclusions are the clearly marked factual corrections in §1. Nothing was moved to converge with the OpenAI report.

### Classification used for every material disagreement

| Code | Meaning |
|---|---|
| **F** | factual / source disagreement |
| **I** | model-identity / access ambiguity |
| **B** | benchmark / evaluation-method difference |
| **V** | value / quota assumption difference |
| **R** | project-routing judgment difference |
| **U** | unsupported / speculative difference |

---

## 1. Factual corrections to Phase A discovered during comparison

These are **corrections or upgrades of facts** I established *after* the freeze, using sources I did not use in Phase A (the live T3 catalog, and pages the OpenAI report cited). They are reported here, not edited into the frozen file.

| # | Phase A statement | Post-freeze finding | Effect |
|---|---|---|---|
| C1 | Claude account used "not identifiable"; effort "not displayed to the researcher". | T3 `orchestrator_capabilities` reports this session's inherited provider instance as **`claudeAgent_claude_b_pro` ("Claude B — Pro")**, model `claude-sonnet-5-5`, **effort `high`**. The packet's third-preference option ("Sonnet 5.5 at high") is therefore what was actually used. | Provenance correction. Whether "Claude B — Pro" is subscription 1 or 2 remains **unverified**. |
| C2 | Opus 5.5 / Fable 5.1 access "open / unresolved". | The live T3 catalog lists `claude-opus-5-5` and `claude-fable-5-1` (plus `fable-5`, `opus-5`, `opus-4.8/4.7/4.6`) as selectable under **both** Claude A and Claude B — Pro, with Opus fast mode. | **Visibility is established; entitlement and billing are not.** Catalog presence is not an entitlement test, and Anthropic's docs say Fable may bill usage credits. See the compliance note below. |
| C3 | ChatGPT Plus 5-hour window "from news coverage; not officially confirmed". | OpenAI's own page states: "the five-hour usage limit is shared across all apps where they use their ChatGPT plan… no app receives a separate allowance", and that it does **not** apply to Pro users. | Upgrade §10 item 10 from [3P] to [OFF]. Also confirms any T3/Codex use draws on the same Plus pool as the ChatGPT apps. |
| C4 | `mimo-v2.6-pro` identified from CCF handoffs and the Go table. | The live catalog lists **`opencode-go/mimo-v2.6-pro`** (and `xiaomi-token-plan-ams/mimo-v2.6-pro`). | Strengthens Phase A's MiMo identity. |
| C5 | GPT-6.1 Terra "long context: ?"; "coding is not in OpenAI's own one-line description". | OpenAI's Terra model page documents **1,050,000-token context (922K max input), 128K output**, effort none→max, hosted shell/apply-patch/computer-use tools. Its description is "visual reasoning, precise computer use, and clear, natural writing"; **the page does not mention coding or "scoped fixes"** — the OpenAI report attributes "scoped fixes" to a separate model-selection guide that I did not read. | Phase A's Terra long-context "?" should read ●●●-documented; my coding-description statement stands for the model page but is **unverified against the selection guide**. |
| C6 | DeepSeek 1M context / 384K output / MIT weights / specific benchmark values. | DeepSeek's official 1017 announcement page, as read, gives **no** API model name, context, max output, license or prices, and names only "DeepSWE v1.1 Pass@1 … mini-swe-v2 harness". My values came from [3P] articles. | Those numbers stay [3P], not [OFF]. The OpenAI report cites first-party DeepSeek docs (`news260910`) for 1M/384K; I did not verify that page. |

**Compliance note on the model instruction (material).** The task asked for the strongest Claude model actually available, preferring (1) strongest research model, (2) Opus 5.5, (3) Sonnet 5.5 high. The live catalog (C2) shows (1) and (2) were selectable. This session ran on option (3). I could not change my own model mid-session and I did not inspect the catalog before starting, so I did not choose; I also would not have selected Fable without confirming it does not bill usage credits. **Phase A was therefore produced by the lowest-preference permitted model.** If the Manager wants a stronger-model replication, it needs a fresh blind run on Opus 5.5 (or Fable 5.1, after checking the usage-credits consent label); I did not start one because that would re-spend quota on a decision the user should make.

---

## 2. Executive comparison

The two reports **agree on the structure of the answer** and **diverge mainly on evidence depth and on who gets the "default" slot for everyday work**.

1. **Same architecture of conclusion.** Both say quota, handoffs and review burden dominate nominal model price; both refuse to convert subscription quota to tokens/euros; both insist the user is not on API pricing; both treat Go as high-volume/bounded, Sol as the local-implementation lead, and Claude as review/continuity; both flag the Global-region requirement for DeepSeek V4.1 Flash and decline to change settings.
2. **Evidence depth differs sharply.** The OpenAI report contains **no benchmark scores and no numeric usage or allowance figures**; the Claude report contains many, with provider/independent labels. The OpenAI report is more conservative and more explicit that no numeric ranking is justified; the Claude report is more informative but more exposed to over-reading provider tables.
3. **The two reports contain disjoint evidence sets.** The OpenAI report has **live runtime discovery** (Codex app-server model list, T3 catalog, hidden/disabled instances) and benchmark-integrity literature; the Claude report has **official plan-limit pages, Go allowance table, OpenAI per-5-hour estimates, Anthropic/OpenAI/Artificial Analysis/Vals numbers** and Claude Code quota-burn mechanics. Neither is a superset of the other (§6).
4. **Most real disagreements are about the "value/default" column** (who handles bounded fixes, Git, docs, planning) and about **whether Fable/Astra deserve a "best quality" slot**. These are mostly V and R differences driven by *what each researcher looked at*, not by contradictory facts.
5. **One factual contradiction found:** the OpenAI report calls the historical MiMo worker unresolved and its MiMo list omits `mimo-v2.6-pro`; the repo handoffs and the live catalog name it (§5, F-1).

---

## 3. Agreements

| Topic | Both reports say | Evidence standard |
|---|---|---|
| Model vs wrapper | T3 Code, TTree Code, Codex, OpenCode, "Claude A/B" are wrappers/lanes, not models; TTree implementation is unresolved | Claude: repo labels; OpenAI: live catalog + repo |
| OpenAI IDs | `gpt-6.1-sol`, `gpt-6.1-terra`, `gpt-6-astra`, `gpt-6-luna` and 5.6 variants are real public IDs | Claude: official OpenAI pages; OpenAI: official + live Codex |
| `gpt-6x` | Not identifiable; unresolved | Both |
| Claude A/B ↔ subscription 1/2 | Not established; do not assume | Both |
| Subscription economics | Do not convert quota to tokens or euros; no per-task value until measured | Both |
| DeepSeek V4.1 Flash | A real Go model (`opencode-go/deepseek-v4.1-flash`); requires **Global** region per Go docs; user's Europe setting does not prove it is enabled; do not change region/privacy | Both cite the same Go page |
| OpenCode vs OpenCode Go | `opencode/` and `opencode-go/` are different routes | Both (Claude via a recorded credential failure) |
| Plus 5-hour limit | Shared across apps | OpenAI report cites the same OpenAI page I confirmed post-freeze (C3) |
| Claude quota | 5-hour session + weekly limits, shared across Claude surfaces and Claude Code, depend on length/model/features/effort | Both cite `support.claude.com/.../11647753` |
| Sol is the Unity/C# implementation lead | Both | Judgment in both |
| Independent/different-provider review for consequential work | Both | Judgment in both |
| Watcher should not become a second writer | Both (OpenAI explicit; Claude: "watcher observes") | Judgment |
| Cross-family benchmark comparison is not safe | Both state it | Claude: "tier precision only"; OpenAI: "no numeric ranking is justified" |
| Next step is a measured CCF comparison | Both propose recording accepted work, quota, retries, review time | Judgment |
| Neither authorises purchases/routing change | Both | — |

---

## 4. Material disagreements

Ranked roughly by impact on a routing decision.

### D1 — Who gets "best quality" for Manager planning, deep research and adversarial review? **[B + V]**

| | Claude report | OpenAI report |
|---|---|---|
| Planning | Opus 5.5 if the account exposes it; else Sonnet 5.5 high or Sol. **Avoid Fable.** | **Astra or Fable if included**; Sol default |
| Deep research | Opus 5.5 / Sonnet 5.5 high with web | **Fable or Astra with web if included**; Sol + sourced collection |
| Adversarial review | Opus 5.5 (high stakes), Sonnet 5.5 xhigh default; different family from author | **Fable / Astra independent of writer, if included**; alternate-provider Sonnet/Sol |

- **Why they differ.** The OpenAI report ranks by provider *positioning* ("most capable", "demanding long-horizon reasoning") with no numbers. I used Anthropic's own table (Fable 5.1 *behind* Opus 5.5 on Terminal-Bench 4.0, FrontierCode, CursorBench, HLE-with-tools), Artificial Analysis' capability table (Opus 5.5 first in all six domains shown; Fable 2nd–8th), Anthropic's docs that Fable "can bill to usage credits instead of drawing on your plan's included limits", and OpenAI's per-5-hour estimates (Astra 5–45 on Plus).
- **Cause.** Evidence selection, not a reasoning disagreement: the OpenAI report never consulted the plan-usage pages or any benchmark table. It would likely agree that "if included" is doing a lot of work.
- **Residual weakness in the Claude position.** Anthropic's tables are self-reported with provider-chosen comparison models; AA's table was read through a summarising model, uses "Max" effort for all entries, and covers breadth, not CCF-style Unity work. Anthropic itself says "benchmark margins have become a less reliable guide to real-world differences". So "Opus ≥ Fable for CCF" is **supported but not established**; "Astra is the best" is **not supported by the evidence cited in that report**.
- **Resolving data.** (a) In the `/model` picker, does the Fable row on each account show "Requires usage credits"? (b) A matched blind review: same G3 artifact reviewed by Opus 5.5, Sonnet 5.5 xhigh, Fable 5.1, Sol, Astra; the Manager scores findings accepted vs false-positive, and quota consumed is logged.

### D2 — Default for bounded fixes, Git mechanics, docs, static inspection, watcher **[V + R]**

| Class | Claude "value/default" | OpenAI "value/default" |
|---|---|---|
| Bounded bugs | DeepSeek / MiMo | **Terra** |
| Git integration | DeepSeek (mechanical); Sonnet for conflicts | **Terra with exact checks** (Go only for read-only preflight) |
| Static inspection | DeepSeek / MiMo | Go model with cited findings |
| Docs/context refresh | DeepSeek / MiMo | **Terra** |
| Watcher | DeepSeek | Go model watcher; Terra continuation |

- **Internal tension in the OpenAI report.** Its executive summary says "Treat OpenCode Go as the high-volume first stop for bounded coding, static checks, documentation and monitoring", but its routing table makes **Terra** the default for bounded bugs and documentation and puts Go in the "fast/low quota" column. The summary and the table disagree with each other.
- **Why the reports differ.** I had the Go allowance table (DeepSeek V4.1 Flash $80 monthly / $40 weekly / $16 per 5 h, vs $15 for MiMo-V2.6-Pro) and the Plus per-5-hour estimates (Terra 35–285), and CCF handoffs showing DeepSeek completing MFM-01, RACK-V2-04, PARITY-05, OPEN3 and the final MFM-02 qualification. The OpenAI report cites the Go page but quotes none of its figures, and treats Go availability as unverified.
- **Possible own-family lean.** Counting the OpenAI report's "best value/default" column, an OpenAI model is primary in 6 of 10 rows (planning, large Unity, bounded, Git, research, docs). In the Claude report a Claude model is primary in 3 of 10 value cells (planning, research, review) and Go in 6. Both reports therefore **favour the family that wrote them in the value column**, the OpenAI report more strongly. Part of this is plausibly access, not preference: the OpenAI report was produced inside Codex with direct local discovery; I had richer Claude documentation. I cannot separate those with the available evidence.
- **Category.** V (quota assumptions) and R (judgment). Not F.
- **Resolving data.** Two weeks of logging per §8 Test T2: accepted-task rate and allowance/quota used for the same bounded task classes on Go vs Terra.

### D3 — Is the second Claude subscription an independent pool? **[V]**

- **OpenAI report:** "check each reset and weekly meter **without assuming separate pools**"; "Do not assume exactly twice the useful work."
- **Claude report:** two independent pools, judgment [J] M, used as **primary + reserve**, citing RACK-V2-05 (Claude A at its limit 01:40Z; Claude B Pro working from 02:31Z).
- **Assessment.** The OpenAI position is the more conservative and is correct that nobody has measured it. The RACK-V2-05 sequence supports "B had capacity while A was limited" but does not show *independence in general* (e.g. a shared-account or org-level rule would not be visible in one night). The Claude report's own confidence label (M) is defensible; its phrase "independent pool (not shared)" in the matrix is stronger than the evidence.
- **Resolving data.** Read `/usage` on both accounts at the same moment before and after a measured task; confirm each shows its own session/weekly bar and independent reset times.

### D4 — Evidence standard and benchmark discipline **[B]**

| | Claude report | OpenAI report |
|---|---|---|
| Numeric benchmarks | Yes (Terminal-Bench 4.0, FrontierCode, CursorBench, GDPval-AA, HLE, OSWorld; AA table; Vals) | None |
| Quota figures | Yes (Plus per-5h; Go dollar allowances) | None |
| Capability grades | ●●● with evidence labels and a cross-family caveat | H / M / ? working judgments, explicitly "not measured grades" |
| Benchmark integrity | Not covered | Terminal-Bench 2.1 task revisions; leaderboard integrity update (reward hacking, leakage); Anthropic "infrastructure noise" (six-point spread) |
| Scaffold sensitivity | One datum: DeepSeek 31.8 (Vals) vs 42.3 (Claude Code harness) | General caveat only |

- **Which is better?** Neither alone. The OpenAI report is **more conservative and cites relevant methodology literature that Phase A lacks**; the Claude report is **more informative and traceable** but its ●●● grades blur provider-reported and independent evidence.
- **A shared limitation.** The OpenAI integrity sources concern Terminal-Bench **2.0/2.1**; the 2026 provider tables use **Terminal-Bench 4.0**. Neither report verifies that the same integrity concerns apply to 4.0.
- **Category.** B. Largely **reasoning/standards**, not evidence access.

### D5 — Quota-aware routing of long autonomous runs **[V + R]**

- **OpenAI:** value = "Sonnet or Sol with checkpoints"; Go as bounded worker; avoid "high effort without quota/reset plan".
- **Claude:** value = DeepSeek in **off-peak** windows (peak = 2× burn, 01:00–04:00 and 06:00–10:00 UTC weekdays) with a watcher; Sol only for hard slices; avoid using a Claude `/loop`/goal check-in as a watcher.
- **Cause.** Evidence selection (peak/off-peak table, Claude Code burn mechanics) plus judgment. The Claude position is *untested speculation built on documented mechanics* (U-adjacent); the OpenAI position is generic.
- **Resolving data.** Run the same overnight bounded task at peak vs off-peak and read `opencode.allowance` per response; run one Claude session with and without an idle `/loop` and compare `/usage`.

### D6 — Claude model entitlement on the €20 plans **[I]**

- **OpenAI:** "availability of Opus or Fable under either account is not established" — while also reporting that both T3 Claude instances offer Opus/Fable.
- **Claude:** unresolved; Claude Code docs say Opus 5.5 is Pro's default model; Fable may bill credits; plus one unattributed CCF "Opus 5.5" session.
- **Assessment.** Compatible; the difference is that I could quote Anthropic's default-model text and the credits-consent behaviour. Both are now additionally supported by the live catalog (C2), which is **not** an entitlement test. Only the user's account can settle this.

### D7 — Is DeepSeek usable under the Europe subscription? **[I]**

- **OpenAI:** do not assume enabled; verify region first.
- **Claude:** works in practice (many successful CCF runs through `opencode-go/deepseek-v4.1-flash`); the *basis* (Global enabled vs another reason) is unverified.
- **Assessment.** Both are right; the Claude report adds the CCF operational evidence, the OpenAI report is the more careful about not asserting entitlement. A further risk only the Claude report states: DeepSeek's ZDR agreement is "valid through October 31, 2026" and renewed monthly — three days from the check date.

### D8 — Caveats on Terra **[B]**

Claude report: Terra weakest independent evidence of the OpenAI set (AA Engineering 54), and OpenAI describes it around writing/vision/computer use. OpenAI report: Terra "H" for long context, research and instruction following, "M" for engineering, and **value default for bounded fixes**. These are not incompatible (M for engineering), but the OpenAI report's use of Terra as default for Git and docs is **not backed by any evidence it cites** beyond provider positioning and CCF's RACK-V2-06 success.

---

## 5. Contradictions of factual claims

| ID | Claim | Claude report | OpenAI report | Resolution |
|---|---|---|---|---|
| F-1 | MiMo identity | CCF handoffs (`S1-C-Implementation-01`, `S1-C-ReviewHistory-Spec`) record **`opencode-go mimo-v2.6-pro`**; Go table lists it | Historical MiMo "unresolved"; live T3 search lists v2.5, v2.5-pro, v2.6-flash, `xiaomi-token-plan-ams/mimo-v2.6-flash`, `opencode/mimo-v2.6-flash-free` — **no v2.6-pro** | Live T3 catalog (checked post-freeze) **does** list `opencode-go/mimo-v2.6-pro`. The `mimo` search returned 8 entries and reported **2 omitted**, matching the OpenAI report's note that "search is bounded and can omit entries". Cause: **evidence selection** (the OpenAI report relied on `active-tasks.md`, not the handoffs). Claude report is correct on the identity; neither establishes entitlement. |
| F-2 | Terra release status | Documents that third-party sites call "GPT-6.1 Terra" an unreleased leak, and resolves it in favour of OpenAI's official pages | Treats Terra as a normal public ID; no mention of the conflict | Not a disagreement of fact; the Claude report has additional conflict reporting. |
| F-3 | DeepSeek facts (context 1M, output 384K) | From [3P] | From first-party DeepSeek docs | Agree on values; Claude's sourcing is weaker (C6). |
| F-4 | Starting commit | `b70206f` (this task) | `98bd0d8` | Different tasks; not a contradiction. |
| F-5 | Whether Plus 5-hour limit exists | Phase A: from news; now [OFF] | Cites OpenAI page | Agree (C3). |

No other direct factual contradiction was found. Where the reports are silent on the same fact (e.g. neither verified Claude Pro's weekly size), the gap is shared.

---

## 6. What one report includes and the other omits

**Only in the OpenAI report**
- Live runtime discovery: `codex-cli 0.164.0` app-server `model/list` (complete page, hidden entries `gpt-reserve`, `codex-auto-review` not asserted selectable); the `ultra` effort with automatic task delegation; default efforts per model; the point that wrapper effort controls need not equal API controls; Sol API documents low→max, not runtime `ultra`.
- T3 catalog findings: both Claude instances offering Opus/Fable; disabled Cursor/Grok/Antigravity/Muse/Pi lanes; OpenCode `build/plan` agents and variants.
- API-level capability documentation (tools, Responses requirement), DeepSeek first-party docs, and the DeepSeek 1017 announcement's harness caveat.
- Benchmark-integrity and infrastructure-noise sources (§4 D4).
- The prior cloud report (#52) status, and the fact that its full text was unavailable.
- A proposal for a blinded CCF task portfolio with fixed briefs/tests/harness.

**Only in the Claude report**
- ChatGPT Plus per-5-hour estimates (Astra 5–45, Sol 15–160, Terra 35–285, Luna 350–3,000) and credit rates.
- OpenCode Go per-model dollar allowances, the 20%/50%/100% window rule, peak/off-peak hours, request estimates, Go Plus, ZDR expiry.
- Claude Pro mechanics: wrap-up allowance, default-model text, Fable-credits billing and its **consent-bypass in non-interactive/Agent-SDK sessions**, Max-only Fable weekly cap, usage-burn causes (subagents, agent teams ~7×, idle loops, cache misses), the 2026-05-06 limit doubling.
- Benchmark numbers (Anthropic, OpenAI, AA, Vals) and the scaffold-sensitivity datum.
- Anthropic's own warning that Sonnet 5.5 at **Max** effort over-reaches scope.
- A CCF operational evidence table across ~12 tasks, including the quota-overhead incidents.
- Claim-by-claim evidence labels and a unified uncertainty list; recommendation of primary+reserve use of the Claude pair and a logging template.

**Where each omission matters most.** The Claude report's missing live catalog and integrity literature weaken its identity and benchmark discipline; the OpenAI report's missing allowance/limit data weakens its value analysis, which is the user's stated core question ("which subscription produces the most accepted CCF work with the least quota burn…").

---

## 7. Bias, conservatism and speculation

| Dimension | Claude report | OpenAI report |
|---|---|---|
| **Own-family lean** | Moderate: uses Anthropic's tables for Claude models; Claude is primary in review/planning/research value cells; anchors partly on existing CCF policy text ("REGULAR CLAUDE VALUE… Sonnet 5.5 xhigh"). Mitigations: demotes Fable (within-family), states Plus is a burst resource, makes Sol the Unity and long-run lead, and reports OpenAI numbers from secondary sources only | Moderate: OpenAI model primary in 6/10 value cells; never discusses Plus per-window scarcity while scrutinising Claude's pool assumptions; "Astra or Fable" top-quality picks without evidence. Mitigations: lists Opus/Fable as quality picks; reserves Claude for review/continuity |
| **Conservatism** | Mixed: labelled evidence, but ●●● grades and "independent pool" phrasing run ahead of evidence | High: "hypothesis to test", "no numeric ranking is justified", entitlement never assumed |
| **Speculation** | Off-peak scheduling, primary+reserve and some grades are reasoned from docs, untested | Quality-column picks (Astra/Fable) and Terra defaults rest on provider positioning only |
| **Anchoring** | Both reports lean on existing CCF routing labels and the `active-tasks.md` narrative | same |

I can **not** show from the documents alone that either author's family bias drove a conclusion; the differences are equally well explained by access to different evidence.

---

## 8. Likely cause of each difference, and what would resolve it

| ID | Difference | Class | Likely cause | Test / data that would resolve it |
|---|---|---|---|---|
| D1 | Fable/Astra "best quality" vs Opus 5.5 | B + V | **Evidence selection** | T1: matched blind review of one artifact by Opus 5.5 / Sonnet 5.5 xhigh / Fable 5.1 / Sol / Astra; Manager scores accepted findings; log quota. Plus read the `/model` credits label for Fable. |
| D2 | Bounded/Git/docs default: Go vs Terra | V + R | Evidence selection + judgment; both families lean to their own | T2: same 10 bounded tasks routed to DeepSeek, MiMo, Terra, Sonnet; record acceptance, retries, allowance/quota, reviewer time. |
| D3 | Independence of the two Claude pools | V | Evidence + conservatism | Paired `/usage` readings on both accounts around the same task. |
| D4 | Numeric evidence vs none; integrity literature | B | **Reasoning/standards** | Re-check whether Terminal-Bench 4.0 shares the 2.1 integrity issues; run a CCF-native mini-benchmark. |
| D5 | Overnight routing / off-peak | V + R | Evidence selection | `opencode.allowance` per response at peak vs off-peak; `/usage` with vs without idle loops. |
| D6 | Opus/Fable entitlement | I | Evidence the user alone holds | User opens each Claude account's `/model` picker and Settings → Usage; no researcher can settle this. |
| D7 | DeepSeek usable under "Europe" | I | Evidence selection (operational history) | User reads Settings → Privacy → Regions (read-only); optional: confirm data-residency expectation. |
| D8 | Terra default | B | Judgment | Part of T2. |
| F-1 | MiMo identity | F + I | **Evidence selection** | Resolved: live catalog lists `opencode-go/mimo-v2.6-pro`. Entitlement still needs one test call by the user. |

**Suggested single experiment (consistent with both reports):** a two-week log across all tasks with fields *pool, model, effort, wrapper, task class, accepted (Y/N), retries, wall time, quota delta, reviewer minutes*. T1 and T2 can be built from that log without special runs.

---

## 9. What neither report establishes

- Which Claude account is subscription 1 or 2; plan tier of Claude A; whether Fable or Opus is covered by the plan's included limits.
- Claude Pro's weekly allowance in any unit; how Opus 5.5 vs Sonnet 5.5 quota burn compares.
- Whether `gpt-6x` is a distinct model; what "Astra" means as a CCF asset lane label; what TTree Code is.
- Any matched same-harness comparison of Sonnet 5.5, Opus 5.5, Sol, Terra, DeepSeek V4.1 Flash and MiMo-V2.6-Pro on CCF tasks.
- EUR/VAT prices and the actual meters of the four subscriptions.

---

## 10. Conclusion

The independent replication **converges on the shape** — quota is the binding constraint; Sol leads local Unity work; Claude is the review/continuity reserve; Go is high-volume and bounded; identities and entitlements are mostly unresolved. It **diverges on evidence and defaults**: the OpenAI report is more cautious and carries live-runtime and methodology evidence; the Claude report carries the quantified plan-limit and benchmark evidence that the user's value question needs. Most disagreements trace to **what each author looked at**, not to opposing reasoning, and each has a concrete, cheap test (§8). Neither report should be treated as the routing authority until the log-based experiment exists.

## Sources

Primary sources for the corrections and comparisons in this document (checked 2026-10-28):
- T3 Code `orchestrator_capabilities` live catalog (searches: `claude`, `mimo`, `mimo-v2.6-pro`, `deepseek`) — session-local.
- OpenAI, plan usage sharing — https://developers.openai.com/siwc/token-sharing-open-source/profiles-and-sessions
- OpenAI, GPT-6.1 Terra model page — https://developers.openai.com/api/docs/models/gpt-6.1-terra
- DeepSeek, V4.1 Flash 1017 release — https://api-docs.deepseek.com/news/news261017/
- Source lists of the two compared reports (see each report).
- CCF repository: `Docs/Handoffs/S1-C-Implementation-01/HANDOFF.md`, `Docs/Handoffs/S1-C-ReviewHistory-Spec/HANDOFF.md`, `Docs/Handoffs/RACK-V2-05/HANDOFF.md`, `Docs/Project/AgentWorkflow.md`.
