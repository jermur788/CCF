# Sen-1 + Multi #822 — Independent cloud-agent critical review: lean CCF Project Instructions

**Purpose:** Obtain a genuine **second opinion by an independent cloud agent** on the CCF portfolio advisor's proposed instruction changes intended to prevent expensive management/security/testing detours and excessive code reinvention, while protecting indispensable safety and qualification. **Review-only.** Do not treat the GPT-6 author's proposal as correct or preapproved.

**Status:** READY FOR OWNER DISPATCH — NOT RUNNING. This packet is documentation, not a reviewer execution receipt. Exact claim: [#822](https://github.com/jermur788/CCF/issues/822). Owner explicitly requested this handoff on 19 November 2026.

**Difficulty:** **Medium–High**. Needs cross-file instruction/authority reconciliation, evidence-ranked causal critique, lean-process design and independent adversarial thinking, but no code execution or native Unity testing.

**Recommended independent provider:** Anthropic Claude cloud (Claude Sonnet/Opus, depending actual capability and availability), **not the GPT-6 proposal author**. A cloud agent with connected GitHub read/write may publish one review result. No assertion that a cloud agent has been launched or is connected.

## 1. The user's actual problem

CCF is a forestry and land-management simulator programme. The product owner has experienced **substantial perceived loss of days and model tokens** because the dedicated managers — most notably the 3D Scenario One manager — repeatedly diverted effort into security/tooling, supplementary qualification, coordination paperwork and review cycles, and sometimes underused code already implemented.

The two active product paths are distinct:
- **Sen-1 / Unity 3D:** Compact30, no internal extraction racks, complete Sitka/Oak/Beech annual forestry management loop is the first personal/closed-beta target. Historical five-rack Enlarged80 contains substantive working management, ecology, Work Plan, finance, history and planting systems that should be safely reused, **not rewritten**. The source-local [#815 reuse candidate](https://github.com/jermur788/CCF/issues/815#issuecomment-7015426935) is encouraging but not genuinely Unity-integrated/qualified. [#809 E1 minimum walking preview](https://github.com/jermur788/CCF/issues/809) demonstrated a productive small-test path. The owner requires a genuine playable game and separately authorises the Windows build only after personal Editor test.
- **Multi / standalone 2D:** Original *primary purpose*, clarified directly by owner, is to take forestry management choices demonstrated in **one representative rack/sample** and test defensible extrapolation to **multiple comparable rack/management areas**. The local 100-Sitka Year0–10 viewer/candidate marking are useful enablers, **not that outcome**. An initial one-undivided-area technical slice should not be mistaken for fulfilment of the programme's goal. See [owner clarification](https://github.com/jermur788/CCF/issues/819#issuecomment-7015429935), [current Stage5 semantic correction](https://github.com/jermur788/CCF/issues/817#issuecomment-7013740967).

The requested **strategic change** is not “abolish safety/tests.” It is **deliver working forestry features faster and prevent uncertain or unrelated tooling risks from swallowing the product roadmap**. Source/file ownership, legitimate dangerous-operation blocks, save/frozen-reference compatibility and truthful qualification still matter.

## 2. Exact primary texts — READ THESE BEFORE OPINING

**A. Proposal to critique — author's exact three-file insertion text; NOT applied to canonical Git:**

[CCF Lean Development — proposed canonical Project Instructions amendment](https://github.com/jermur788/CCF/blob/docs/portfolio-lean-project-instructions-proposal-20261119-v1/Docs/Handoffs/Simulator-Portfolio-20261119/CCF-LEAN-DEVELOPMENT-CANONICAL-INSTRUCTION-AMENDMENT-PROPOSAL-20261119.md)

The proposal adds:
1. **Shared instructions:** outcome-first and REUSE / ADAPT / EXCLUDE; continue independently authorised safe work; do not promote infrastructure or an interim 2D step into completed gameplay.
2. **ChatGPT manager role:** justify diversion from the active milestone, classify hard stop / safe continuation / defer, limit serial review and tooling work; report actual outcomes.
3. **AgentWorkflow:** bounded blocker diagnosis; retain the existing risk-based test tiers and acceptance gates; no reviewer booked before a real candidate; quantify overhead only with real evidence.

**B. Author's separate causal assessment (treat conclusions as claims to challenge, not settled findings):**

[CCF development-efficiency and anti-derailment audit](https://github.com/jermur788/CCF/blob/docs/portfolio-development-efficiency-audit-20261119-v1/Docs/Handoffs/Simulator-Portfolio-20261119/CCF-PORTFOLIO-DEVELOPMENT-EFFICIENCY-AND-ANTI-DERAILMENT-AUDIT-20261119.md)

**C. Actual canonical instructions (read current `main`, do not assume the proposal was adopted):**
- [Shared Project Instructions](https://github.com/jermur788/CCF/blob/main/Docs/Project/instructions/shared-project-instructions.md)
- [ChatGPT Role](https://github.com/jermur788/CCF/blob/main/Docs/Project/instructions/chatgpt-role.md)
- [AgentWorkflow](https://github.com/jermur788/CCF/blob/main/Docs/Project/AgentWorkflow.md), especially spec authority, cheapest valid Tier1/2/3, locks, risk-triggered reviews, manager continuation and one-context publication.
- [Claude Role](https://github.com/jermur788/CCF/blob/main/Docs/Project/instructions/claude-role.md), [AGENTS.md](https://github.com/jermur788/CCF/blob/main/AGENTS.md) and [CLAUDE.md](https://github.com/jermur788/CCF/blob/main/CLAUDE.md) to check cross-agent consistency.

**D. Concrete cases for independent inspection (selectively reopen primary results rather than treating paraphrases as proof):**
- [#652 live Ops hold](https://github.com/jermur788/CCF/issues/652), [#754 scratch QA/deployment gate](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-754-MANAGER-FINAL-SCRATCH-ACCEPTANCE-AND-LIVE-INSTALL-GATE-20261118.md), [#802 targeted cleanup](https://github.com/jermur788/CCF/issues/802#issuecomment-7005995893): investigate what was valid protection versus runaway scope and what actually blocked user playtest.
- [#809 minimum E1 test](https://github.com/jermur788/CCF/issues/809), [#813 invalid JSON/one-comma correction](https://github.com/jermur788/CCF/issues/813#issuecomment-7014326666), [#815 code reuse / final Manager source disposition](https://github.com/jermur788/CCF/issues/815#issuecomment-7015426935): small rapid feedback, focused artifact checks and legitimate native follow-on.
- [#789/#804 archive and compilation provenance](https://github.com/jermur788/CCF/issues/804), [#811 viewer nonauthor review](https://github.com/jermur788/CCF/issues/811), [#814 candidate marking review](https://github.com/jermur788/CCF/issues/814): examples where independent QA caught real defects and/or coordination became cumbersome.
- [#812 → #816 → #817](https://github.com/jermur788/CCF/issues/817) and [owner #819 mission restatement](https://github.com/jermur788/CCF/issues/819#issuecomment-7015429935): avoid turning an ambiguous word (“rack”) into unnecessary geometry or deferring Multi's actual cross-rack extrapolation purpose.
- [Sen-1 + Multi orchestration retrospective](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/Orchestration-Skill-Feedback-20261119/Sen-1-Multi-818-T3-FORK-SKILL-USAGE-RETROSPECTIVE-AND-CREATOR-FEEDBACK-20261119.md), [Multi retrospective](https://github.com/jermur788/CCF/blob/docs/multi-730-818-t3-fork-orchestration-feedback-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-818-T3-FORK-ORCHESTRATION-SKILL-USAGE-FEEDBACK-AND-IMPROVEMENT-HANDOFF-20261119.md) and particularly [owner #818 priority correction](https://github.com/jermur788/CCF/issues/818#issuecomment-7016237892), which **defers additional agent-security-patch work** in favour of faster simulator development, without waiving currently accepted task restrictions.

**Avoid unsupported conclusions:** No verified aggregate token spend, counterfactual days saved, or complete causality measurement exists. Some security work exposed real hazards. The reviewer must distinguish actual block, owner priority, historical safety defect, local review and inferred waste.

## 3. Adversarial questions: this should be a real second opinion

1. **Is the proposal necessary?** Are these new clauses genuinely missing from current shared/role/AgentWorkflow instructions, or do they duplicate existing rules and produce more token-heavy policy? Could **one shorter amendment in AgentWorkflow** be more effective than three copies of similar instructions?
2. **Would it prevent the actual failure modes?** For #652/#754/#802, #809/#815 and #812/#817, say explicitly what each proposed clause would have changed **and what it would not have changed**. Which delays are due to manager judgment, which arise from actual host/physical permissions, and which are unchangeable by prompt instructions?
3. **Could “feature-first” become unsafe?** Identify wording that might be misread as overriding a real Unity/source lease, approved spec, production save, frozen reference, independent high-risk review, security restriction or genuine release gate. Propose clearer wording for such cases; never suggest bypassing accepted controls.
4. **Is reuse-first too rigid?** When should a clean independent implementation be preferred (e.g. incompatible simulation semantics, bad old coupling, model isolation)? What minimum reusable-source inspection is adequate without another large audit?
5. **Could blocker triage itself become bureaucracy?** Is the three-part classification too elaborate? What is the smallest decision rule that avoids repeated security rabbit holes while retaining a safe owner escalation path? Do not invent time-based lease expirations.
6. **What is the right independent review policy?** Distinguish low-risk docs/format from material simulation, save, destructive, mixed-author, authority and integration changes. Audit the potential contradiction between “one parent/one review” and true independent reviewer session/Manager sign-off.
7. **What is the minimal enforceable priority rule?** How to stop an agent from treating new documentation, interim One Area or speculative toolkit development as a substitute for player-visible full-loop progress? What precise escalation request should go to the user before new infrastructure work is elevated?
8. **Can cloud changes be deployed consistently?** Check the proposed Git-canonical → one committed context SHA → generator → Drive mirror → ChatGPT Project attachments pipeline; whether Claude/root instructions also need updates, and whether an incremental update can preserve current frozen worker contexts and locks.
9. **What did the author omit?** Identify any concrete high-impact process fix *not* addressed, especially managerial mandate/priority, cost and bounded approval, source reuse evidence, or scope/quality drift.

**Important:** Aim for maximum playable simulator progress with minimum process ceremony. Do not recommend a new security platform, endless review framework, monitoring dashboard or broad policy rewrite as the default remedy. If a genuine immediate risk needs a stop, explain the smallest safe intervention.

## 4. Expected independent deliverable

Return **one concise, evidence-backed, nonauthor review** with:
- **Verdict:** **APPROVE / NEEDS CHANGES / REJECT** on the **instruction amendment as written**; separate judgement of the original problem and the author's diagnosis.
- **Severity-ranked findings** with exact proposal quotes/section references, real evidence links and whether each finding is blocker/major/minor.
- **Minimal recommended final text**, ideally a directly applicable revised passage (or targeted replacement/deletion) for each affected canonical file. If one file is enough, say so and supply that alternative.
- **Safety and authority check:** explicit preservation of accepted task/source/physical/Unity/save/security/release gates, plus explicit prevention of speculative detours.
- **Adoption plan:** smallest allowed change, focused readback/review, G4 owner/#425 approval and one-context Git/Drive/Project refresh, not agent-initiated adoption.
- **Success test:** two or three practical scenarios proving the policy would change manager routing, plus limited measurable outcomes and failures to watch.

Review must say what was **directly inspected**, what remains **worker-reported**, what is **inferred** and what **cannot be verified**. Avoid merely restating GPT-6's proposal or automatically endorsing it.

## 5. Execution lane and claim

- **Claim:** [#822](https://github.com/jermur788/CCF/issues/822). Current state **READY / NOT RUNNING**. If accepting the task, actual cloud reviewer registers its real model/provider/harness, own docs-review branch and starting main commit as **[RUNNING]** in #822 *before publishing*. Do not invent a local machine or physical lease.
- **Reviewer:** one nonauthor independent Claude cloud agent preferred, with GitHub connector if genuinely available. If connector cannot write, return a complete review in chat and state no GitHub publication occurred; the user can pass it to #425. Do not pretend the GPT-6 author is an independent reviewer.
- **Author proposal:** read-only at its exact link; **do not edit** its branch. Current canonical files on `main` are read-only.
- **Allowed reviewer output (if GitHub write available):** one new Markdown file `Docs/Handoffs/Simulator-Portfolio-20261119/Sen-1-Multi-822-INDEPENDENT-CLOUD-LEAN-INSTRUCTIONS-REVIEW-RESULT-20261119.md` on the reviewer's *own separate docs branch*, with exact readback and issue link. No amendments to canonical instruction files, root entrypoints, Project UI, attached context, source, releases, Ops or Unity. No Merge/main write.
- **No additional reviewers/child agents necessary.** If a material fact is missing, label uncertainty rather than opening an unrelated investigation.
- **Next authority:** present the result to the **owner and Overall Development Manager [#425](https://github.com/jermur788/CCF/issues/425)** for an adoption decision. Portfolio advisor and managers #731/#730 do not self-approve this independent review as a canonical instruction change.
- **Dashboard/Drive context sync:** NOT TO BE PERFORMED for a review packet. Only if later adopted by #425 through the authorised context pipeline.
- **User action required:** paste the dispatch message below into the desired independent cloud agent. No agent is launched by publishing this handoff.

## 6. Paste-ready dispatch

> You are an **independent cloud reviewer**, not the author of the proposed CCF Project Instructions. **Task:** Critically review whether the proposed “lean development / anti-derailment” canonical instruction changes would materially reduce wasted time and tokens from security/tooling detours, repeated testing/reviews and failure to reuse existing 3D code, while preserving real authority/safety gates and the owner's original Multi cross-rack extrapolation purpose. **Read the full binding review packet first:** [DIRECT LINK TO THIS FILE]. Review the exact GPT-6-authored proposal linked within it, the current GitHub canonical instructions and the underlying evidence. Seek defects, duplicates, unenforceable wording and a *shorter better alternative*, not automatic approval. Do **not** change canonical instructions, production source, Ops, Unity, merge or release. Return one severity-ranked APPROVE / NEEDS CHANGES / REJECT verdict and minimal corrected text, with evidence citations. If connected GitHub writing is available, register actual reviewer under #822 and publish the exact single review Markdown on your own docs-only branch; otherwise return a complete review in chat and state that nothing was published. #425 and the owner decide adoption.
