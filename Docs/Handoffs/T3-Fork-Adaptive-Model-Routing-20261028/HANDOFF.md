# HANDOFF — T3 Fork Adaptive Model / Subscription Routing and Learning V1

**Date:** 2026-10-28
**Target:** T3 Fork Manager / orchestrator design lane
**Status:** Product/design handoff. **SPEC REQUIRED before implementation.**
**Starting main:** `4923484cd819f707207782a12754b8f7809a1f16` or descendant.

## Purpose

Design a T3-fork capability that learns how to route CCF work across the user's paid AI software subscriptions by recording real project outcomes, subscription-capacity behaviour, model/settings choices and verification/rework cost over time.

The goal is not to declare one model permanently "best". The goal is to make the T3 Fork Manager increasingly good at answering:

1. Which model/account/settings should own this specific task now?
2. Which route gives the best probability of accepted work for the quota/time it consumes?
3. When should a different provider be used for independent review?
4. Which effort/thinking/tool settings work best for each task class?
5. Where is current subscription capacity being wasted, saturated or unexpectedly replenished?
6. When two comparable tasks exist, can T3 route them differently to learn which route is more resource-efficient without compromising project safety?
7. If more subscriptions are added later, which provider/account would relieve the actual bottleneck?

This is a cross-system workflow + telemetry + persistence + adaptive-routing feature. Per project rules, production implementation requires an approved spec (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`). This handoff supplies product intent and design constraints; it does not authorize coding.

---

## User's current paid software portfolio

The user does **not** use API billing for the CCF workflow. Treat API-token economics as out of scope for routing/value decisions.

Current user-reported software subscriptions:

| Capacity pool | User-reported recurring cost |
|---|---:|
| OpenAI software subscription | €20/month |
| Claude subscription/account A | €20/month |
| Claude subscription/account B | €20/month |
| OpenCode Go Europe | €10/month |
| **Total** | **€70/month / €840/year** at unchanged prices |

Do not infer plan names, VAT, account entitlement or token-equivalent economics merely from these prices.

### Important corrected operating observation

The user is **exhausting all of these subscription pools**. Do not interpret the two Claude accounts as idle capacity.

However, in recent weeks the user has observed:

- the OpenAI account has repeatedly received additional resets of its seven-day limit, roughly often enough that the single paid OpenAI account can sometimes provide something operationally closer to multiple weekly allocations;
- the Claude accounts have not received comparable discretionary resets during the same recent period;
- all pools still reach their limits under sustained CCF work.

Treat this as **observed project telemetry**, not a contractual entitlement. A provider-granted reset is a stochastic/temporary capacity bonus and must never be assumed to continue.

The correct distinction is therefore:

- **nominal capacity:** number/type/cost of subscriptions;
- **observed effective capacity:** actual work completed between limits, including discretionary resets;
- **guaranteed/baseline capacity:** only what the subscription terms/documented limits support.

T3 should route day-to-day work using observed effective capacity while keeping risk planning conservative about non-guaranteed resets.

---

## Existing model research to ingest

Read these canonical research sources before drafting the spec:

1. `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`
2. `Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md`
3. `Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md`
4. `Docs/Research/ModelCapabilityCost/RESEARCH-CORRECTIONS-2026-10-28.md`
5. `Docs/Handoffs/ModelCapabilityCostResearch-ClaudeReplication-20261028/MANAGER-REVIEW.md`

Important correction: historical S1-C MiMo is identified as `opencode-go/mimo-v2.6-pro`; do not preserve the earlier "historical MiMo unresolved" statement.

Important fixed-cost guard: current research says Fable 5/5.1 on Claude Pro may use usage credits rather than included plan quota. T3 must not silently enable or consume pay-as-you-go/usage-credit routes. Current account settings remain unverified.

---

## Current routing priors — starting hypotheses, not permanent policy

These are reasonable priors for the learning system. T3 must be able to update its recommendations from measured CCF outcomes rather than treating these as immutable rankings.

### OpenAI

**Astra**
- hardest architecture/product decisions;
- ambiguous cross-system reasoning;
- difficult specs and G3/G4 reasoning;
- deep research synthesis;
- high-stakes independent review when it is not the authoring family;
- use selectively because it is a scarce high-capability route, but do not leave it unused merely to preserve quota if the task genuinely benefits from it.

**Sol**
- serious Unity/C# implementation;
- difficult debugging;
- integration and cross-system implementation;
- major production work where exact repo/tool access matters;
- should remain a major production lane, not a universal default for every difficult task.

**Terra**
- bounded OpenAI implementation/fixes;
- continuation after another OpenAI model stops;
- mechanical repo/Git/docs work where appropriate;
- computer/visual tasks where its tool profile fits;
- candidate to compare directly against Go routes on bounded work.

### Claude

The two Claude subscriptions are **productive capacity**, not merely reserves or reviewers.

**Sonnet / Opus where actually included and selectable**
- substantial implementation;
- long-running work;
- difficult debugging/reasoning;
- research/spec work;
- independent review of OpenAI-authored work;
- continuity when OpenAI hits a limit;
- one Claude account may author while the other remains available for continuation or independent review, but do not hard-code account A/B roles if live capacity says otherwise.

Avoid a simplistic policy of `OpenAI writes -> Claude checks`. OpenAI and Claude should both be able to lead primary production work. Consequential review should preferably use a different provider/model family from the author.

### OpenCode Go

**DeepSeek V4.1 Flash / MiMo and other verified Go routes**
- high-volume bounded implementation;
- static/offline inspection;
- docs and mechanical checks;
- watcher/coordinator roles;
- low-risk bounded fixes;
- candidate for frequent experimentation against Terra/Sonnet on comparable work.

Do not change region/privacy/account settings merely to expose a model. Record what route actually ran.

---

## Core T3-fork design idea

T3 should maintain a **model-routing telemetry ledger** for every substantive task and use it to learn task-specific routing preferences over time.

The system should distinguish at least four identities:

`task -> agent/lane/wrapper -> provider account/subscription -> underlying model/runtime ID`

Never collapse `T3 Code`, `TTree`, `Claude A/B`, `Codex`, `OpenCode`, etc. into model identity.

### A. Record task characteristics before execution

For each task, capture a structured pre-execution profile such as:

- task/claim ID and canonical packet/spec;
- task class: implementation, bounded fix, debugging, integration, review, research, docs, static verification, Unity qualification, watcher, continuation, etc.;
- subsystem(s) and file ownership area;
- estimated difficulty: Low / Medium / High plus optional finer ordinal score if later justified;
- ambiguity level / requirements certainty;
- expected breadth: files/subsystems touched;
- expected need for Unity/computer/web/image/local shell/Git tools;
- whether independent provider review is required;
- verification tier/gates expected;
- whether task is safe for experimental routing;
- urgency / critical-path status;
- initial route recommendation and confidence.

Do not manufacture precision. Difficulty and ambiguity are estimates and should later be compared with realized difficulty.

### B. Record execution configuration

For every execution attempt or continuation, capture:

- provider;
- account/subscription alias;
- exact displayed/runtime model ID;
- wrapper/agent lane (T3/TTree/Codex/Claude Code/OpenCode/etc.);
- reasoning/thinking effort (`low`, `medium`, `high`, `xhigh`, `max`, etc.) exactly as actually selected;
- fast/priority/default service tier where exposed;
- plan/build mode or comparable agent mode;
- tool availability and tools actually used;
- local/cloud lane;
- starting HEAD / branch / worktree;
- context strategy: fresh packet, inherited summary, full-context refresh, continuation handoff;
- watcher/reviewer configuration;
- start/end timestamps and active/blocked periods;
- model transition/handoff events and why they occurred.

The system should learn not only `model X is good at task Y`, but also whether **particular settings** are good for that task class. Example: Sonnet high vs xhigh, Sol high vs xhigh, Terra for bounded repair, watcher present vs absent, fresh packet vs continuation summary.

### C. Record subscription capacity state

Where the software exposes the information safely, capture:

- limit/quota state before and after work;
- five-hour/session/weekly/monthly window state;
- remaining allowance or qualitative meter if available;
- exact limit-hit timestamp;
- reset timestamp;
- whether reset was scheduled/ordinary or an unexpected provider-granted reset where distinguishable;
- work completed between resets;
- account temporarily unavailable / model unavailable;
- continuation to a different account/provider.

Do **not** convert opaque subscription limits into fabricated tokens/euros.

For OpenAI discretionary resets, maintain historical observations separately from baseline entitlement, for example trailing 4/8/12-week:

- reset count;
- average time between limit and reset;
- accepted tasks completed per effective allocation;
- variance / confidence;
- current assumption that extra resets may cease at any time.

### D. Record outcome quality and total project cost

A task is not successful merely because the executor says it finished. Measure accepted project outcome.

Record at least:

- IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED states separately;
- first-pass verification result;
- tests/checks/captures passed and failed;
- reviewer verdict (`APPROVED / NEEDS CHANGES / REJECTED`);
- number and severity of defects found in review;
- rework attempts;
- regressions/out-of-scope edits;
- ownership/branch hygiene issues;
- handoff completeness;
- human/Manager intervention required;
- total elapsed time to accepted outcome;
- model/provider time before handoff/limit;
- whether another model had to finish the job;
- user playtest/acceptance result where applicable.

The central measure should be **accepted work**, not raw completions or benchmark-like self-reported success.

---

## Metrics the Manager should learn from

Do not pick one universal score prematurely. Preserve the components so policy can be changed later.

Useful metrics include:

- accepted-task rate by task class;
- first-pass verification/approval rate;
- elapsed time to accepted outcome;
- rework/retry count;
- reviewer defect rate and severity;
- quota/limit burden per accepted task or task class, using provider-native meters rather than fabricated token conversion;
- handoff/continuation frequency;
- failure due to context/tool/provider limits;
- human intervention rate;
- idle paid capacity versus blocked useful work;
- effective accepted work per subscription per calendar week;
- effective accepted work per euro per calendar period, using actual fixed subscription cost and observed accepted work, not API pricing;
- observed reset bonus capacity separately from baseline capacity;
- performance by model **and settings** within each task class;
- performance under local vs cloud execution where applicable.

The long-term routing question is closer to:

> What route gives the highest expected probability of an accepted result, at acceptable elapsed time and review risk, while consuming the least scarce current subscription capacity?

Not:

> Which model has the highest public benchmark score?

---

## Adaptive routing concept

The Manager should eventually calculate a route preference from multiple factors rather than a static ordered list.

A conceptual utility model may include:

- expected acceptance probability for this task class;
- expected verification/rework burden;
- expected elapsed time;
- current quota scarcity / likelihood of near-term limit;
- tool/local-access suitability;
- context/continuation suitability;
- independent-review diversity requirement;
- uncertainty in the estimate;
- learning value of trying an under-sampled but plausible route.

Do not hard-code numeric weights in V1 without evidence. The spec should define how recommendations are explainable and auditable.

### Hard feasibility filters come first

Before any adaptive score, eliminate routes that cannot safely perform the task because of:

- missing tool/local/Unity access;
- account/model unavailable;
- region/privacy restriction;
- pay-as-you-go/usage-credit route not explicitly authorised;
- ownership/worktree conflict;
- insufficient task/spec authority;
- required independent review would be violated by same-family reuse;
- provider limit currently reached.

Project authority and safety always override model-optimization logic.

---

## Controlled experimentation / learning

The user specifically wants T3 to learn by experimentation when comparable tasks are available.

### Preferred experiment design

When two **similar but separate** eligible tasks occur, T3 may deliberately route them to different plausible model/settings combinations and compare outcomes over time.

Examples:

- Terra vs DeepSeek on comparable bounded fixes;
- Sonnet high vs Sol high on comparable medium implementation tasks;
- Sonnet high vs xhigh on comparable reasoning-heavy but scope-bounded work;
- Go watcher vs no watcher on otherwise comparable long tasks;
- fresh-context dispatch vs structured continuation packet on comparable continuation work.

Use repeated crossover/matched comparisons across many tasks rather than drawing conclusions from one result.

### Do not duplicate production work casually

The default experiment should use **different but comparable tasks**, not have two writers implement the same production change. Exact duplicate implementation wastes quota and conflicts with single-writer ownership.

Duplicate/blind parallel runs may be allowed only for explicitly authorised research/review/static-analysis experiments where no production ownership conflict exists.

### Experiment eligibility

Do not experiment on a task if experimentation could materially threaten:

- milestone/release critical path;
- save/schema safety;
- user test candidate stability;
- Unity lock discipline;
- ownership boundaries;
- a high-risk integration decision;
- a task with only one route capable of the required tools.

The spec should define an exploration budget and confidence thresholds. Do not silently sacrifice project progress merely to gather data.

---

## Learning task difficulty itself

T3 should track both:

- **predicted difficulty before execution**, and
- **realized difficulty after completion**.

Realized difficulty can be informed by retries, files/subsystems touched, verification burden, handoffs, model-limit events, defects and Manager intervention.

Over time, this lets T3 calibrate its own task classifier. A task initially labelled Medium that repeatedly requires multi-model continuation and substantial rework should influence future estimates for similar work.

This prevents model comparisons from being distorted simply because one model happened to receive harder tasks.

---

## Subscription expansion / purchasing intelligence

The T3 Manager may **recommend** where an additional subscription would add capacity, but must never purchase, upgrade, enable usage credits, change region/privacy settings or alter billing autonomously.

When the T3 fork increases throughput, the user expects subscriptions to be exhausted faster and may add more accounts.

Future recommendations should be based on observed bottlenecks such as:

- useful work queued because provider/account quota is exhausted;
- percentage of calendar time each pool is saturated;
- amount of useful work diverted to lower-fit models because a preferred pool is unavailable;
- accepted work per current subscription;
- persistent unused capacity in other pools;
- discretionary-reset frequency and volatility;
- expected value of adding another account in each provider family.

A provider with repeated discretionary resets may currently deliver more effective capacity than its nominal single subscription suggests. That bonus should influence short-term routing, but should be discounted when making long-term purchase recommendations because it is not guaranteed.

---

## Privacy / billing / account safeguards

V1 must preserve these hard rules:

- no API billing assumptions; this workflow uses software subscriptions;
- no API keys;
- no autonomous purchases/upgrades;
- no autonomous enablement of usage credits/pay-as-you-go balance;
- no autonomous region/privacy changes;
- do not infer entitlement from a model catalog entry;
- record actual account/model selected where possible;
- do not expose secrets or account identifiers in telemetry;
- use stable internal account aliases rather than personally identifying data;
- all telemetry must be auditable and deletable/rotatable according to the eventual T3 data policy.

---

## Interaction with existing CCF management rules

Adaptive routing must remain subordinate to project authority:

1. latest direct user instruction;
2. Decision Log;
3. current milestone;
4. game brief;
5. approved task/spec authority;
6. active ownership/locks;
7. exact repo/Unity evidence.

The adaptive system must never:

- create a second production writer because a model score is higher;
- bypass `active-tasks.md` ownership;
- bypass the machine-wide Unity lock;
- weaken tests to improve a model's apparent score;
- treat research/benchmark results as product authority;
- auto-change canonical routing policy merely because a statistical preference changed.

Learned recommendations are advisory until accepted by the T3 Manager / Overall Manager under normal project governance.

---

## Recommended V1 architecture direction

The exact implementation is for the T3-fork spec, but a sensible decomposition is:

1. **Task Profile** — pre-execution task features and estimated difficulty.
2. **Route Attempt** — model/account/wrapper/settings/tools used for each execution segment.
3. **Capacity Event** — limit hits, resets, allowance observations and availability transitions.
4. **Outcome Record** — verification, review, rework, integration and acceptance results.
5. **Route Statistics** — rolling aggregates per task class/model/settings/account without destroying raw evidence.
6. **Routing Recommender** — explainable recommendation + confidence + fallback routes.
7. **Experiment Planner** — identifies safe comparable tasks and proposes controlled A/B-style routing.
8. **Subscription Capacity Dashboard** — current saturation, resets, effective weekly capacity and bottleneck indicators.
9. **Policy Gate** — hard project/safety/ownership constraints that recommendations cannot override.

Prefer append-only raw telemetry plus derived aggregates. Preserve enough provenance to recompute recommendations if the scoring model changes.

Do not store model-generated narrative as the only evidence; preserve structured fields and links to claims/handoffs/commits/verdicts.

---

## Required spec/design output from T3 Fork Manager

Produce a DRAFT spec under:

`specs/t3-adaptive-model-routing-v1/`

with canonical files:

- `spec.md`
- `plan.md`
- `tasks.md`
- `checklists/requirements.md`

The draft must cover:

1. telemetry schema and persistence location;
2. account/model/wrapper identity model;
3. task taxonomy and difficulty calibration;
4. execution-setting capture;
5. quota/reset/capacity event model;
6. outcome/verification/rework model;
7. explainable routing recommendation logic;
8. controlled experimentation rules;
9. privacy/billing/account safety;
10. migration/history/retention policy;
11. dashboard/reporting requirements;
12. how recommendations become—or do not become—canonical routing authority;
13. how future subscriptions/accounts are added without breaking historical comparisons;
14. test strategy with synthetic records plus real historical CCF backfill where defensible;
15. failure modes: missing telemetry, wrong model identity, provider reset anomaly, continuation across models, reviewer bias, task-difficulty confounding.

Also include a proposed **minimum viable telemetry V1** that can begin collecting useful evidence before sophisticated adaptive scoring exists. Data collection should precede aggressive automated optimization.

Do not implement production code until the spec reaches an implementation-authorising status.

---

## Acceptance questions for Manager review of the spec

A good design should let the Manager answer, with traceable evidence:

- Which route is currently best supported for this task class?
- How confident are we, and from how many comparable tasks?
- Did high/xhigh/max actually improve accepted outcomes enough to justify quota burn?
- Is Terra or Go more efficient for bounded CCF fixes?
- When should Sonnet/Opus lead implementation rather than only review it?
- How often is Astra actually worth its scarce capacity?
- Which provider/account is the current bottleneck?
- How much effective OpenAI capacity came from discretionary resets this month?
- If those resets stopped tomorrow, what would routing look like?
- Which subscription should be added next if the user chooses to add capacity?
- Can the system explain why it chose a route without relying on opaque self-generated scores?

---

## Research/evidence classification

Keep the following distinctions explicit:

- provider/documentation fact;
- independent benchmark/evaluation;
- CCF project observation;
- direct user observation (including reset behaviour);
- model-derived design judgment;
- gameplay/project authority.

The reset behaviour described above is a **direct user observation**, not a provider guarantee.

---

## Handoff instruction

Target agent: **T3 Fork Manager / strongest available planning-reasoning model**.

This is a Manager/spec-design task, not a Unity or production implementation task. Use repository evidence and the model-research documents above. Inspect the actual T3-fork architecture before fixing persistence/schema details in the spec.

At substantial task start, create/mark the task claim `[RUNNING]` with actual manager/model/provider and starting HEAD. Publish the draft spec plus a short handoff summarizing key architecture decisions, unresolved product decisions and any implementation prerequisites.

Expected exit token:

`T3 ADAPTIVE MODEL ROUTING SPEC DRAFTED — READY FOR OVERALL MANAGER REVIEW`
