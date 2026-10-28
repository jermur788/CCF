# MODEL-CAPABILITY-COST-RESEARCH-02 — Manager Review

**Verdict: APPROVED / READY FOR INTEGRATION AS ADVISORY EVIDENCE.**

This review accepts the research report and handoff as a current evidence package for CCF model-routing and cost decisions. It does **not** make the report's routing table a binding project policy and does not authorize purchases, subscription changes, or account changes.

## Reviewed artifacts

- Report: `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`
- Worker report commit: `25e9bf95054a87ee1978d02ea7f52235e0e7ad08`
- Handoff: `Docs/Handoffs/ModelCapabilityCostResearch-20261028/HANDOFF.md`
- Published branch tip before this review: `1465b3702f0ed147bab34f6f522f6ecf567afb70`
- Claim: #53 `[HANDED-OFF]`, closed.

## Acceptance basis

1. **Runtime identity discipline:** the report separates CCF/T3/TTree/Codex/OpenCode wrappers from runtime identifiers, provider-published model IDs and billing routes. Dynamic local Codex app-server discovery and T3 catalog discovery are used for runtime inventory rather than assuming public catalog availability equals local entitlement.
2. **Pricing discipline:** official provider documentation is used for volatile API pricing and quota semantics. Subscription/account entitlements are explicitly left unverified where the worker did not inspect the user's accounts.
3. **Capability discipline:** provider positioning, independent benchmark methodology and CCF project observations are labelled separately. No single vendor benchmark is promoted into a controlled cross-model ranking.
4. **Cost arithmetic:** normalized examples state their token/cache assumptions and correctly warn that subscription usage, retries, hidden reasoning, tools, cache misses and different tokenizers can materially alter effective task cost.
5. **Routing recommendations:** the report provides practical quality/value/fast/fallback choices by CCF task class while explicitly labelling them provisional judgments pending matched CCF benchmarking.
6. **Known unknowns preserved:** `gpt-6x`, historical MiMo identity, exact account entitlements/quota pools, hosted snapshots and same-harness cross-model performance remain unresolved instead of being guessed.
7. **Scope:** branch changes are documentation/research only. No production code, Unity, save/schema, project decisions, generated context, mirror, dashboard or account state changed.

## Manager primary-source spot check

A bounded spot check on 2026-10-28 confirmed that the report's volatile values for OpenAI GPT-6.1 Sol, Anthropic Claude Sonnet 5.5 and DeepSeek V4.1 Flash match their current official model/pricing documentation at review time. This is not a full re-research of every row.

## Validation disposition

The handoff reports that unqualified `python -m unittest discover -v` could not run because pyenv had no selected Python; an explicitly selected Python exited 0 with **0 tests discovered**, while `python3` also found 0 tests and exited 5. Therefore **no repository unit-test PASS is claimed**. `git diff --check` passed. Because this branch adds only Markdown research/handoff documents and no executable project code, the disclosed environment/test-discovery limitation does not block acceptance of the research artifact.

## Status and use

- Research status: **APPROVED AS ADVISORY EVIDENCE**.
- Routing status: **PROVISIONAL / NON-BINDING** until the Manager/user adopts specific recommendations.
- Purchase/account status: **NO CHANGE AUTHORIZED**.
- Recommended follow-up if stronger routing confidence is needed: a blinded, fixed-brief CCF benchmark portfolio measuring accepted-task success, retries, elapsed time, model/effort, token use and reviewer effort across candidate lanes.

No Unity or Windows build is required for this documentation-only research integration.
