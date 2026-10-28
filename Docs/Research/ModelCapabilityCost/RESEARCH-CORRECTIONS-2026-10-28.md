# Model capability/cost research — factual corrections

Date: 2026-10-28

This note records factual maintenance corrections discovered during Manager review of the blind Claude replication. It does not change routing policy, subscription/account settings, or purchasing decisions.

## C-01 — historical MiMo worker identity

The integrated OpenAI-produced report `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md` states that the historical CCF MiMo worker/version is unresolved. That statement is superseded.

Direct repository evidence in `Docs/Handoffs/S1-C-Implementation-01/HANDOFF.md` records:

- worker route: `T3 Code / opencode-go mimo-v2.6-pro`;
- underlying displayed route/model identifier: `opencode-go/mimo-v2.6-pro`.

Therefore the bounded corrected statement is:

> The historical S1-C MiMo worker route is identified as `opencode-go/mimo-v2.6-pro`. The exact hosted snapshot/deployment behind that route may still be unresolved, and catalog visibility does not by itself prove current entitlement.

This correction supersedes only the historical-worker identity statements in the earlier report. It does not establish general MiMo capability, current availability, quota, or pricing beyond the evidence separately cited in the research set.

## C-02 — Claude Fable on Pro / usage credits

During Manager review, current Anthropic documentation was checked and supports the Claude replication's warning that Fable 5 / Fable 5.1 on Pro plans uses pay-as-you-go usage credits rather than the plan's included limits. Whether usage credits are enabled on either of the user's actual Claude accounts was not inspected and remains unresolved.

No account setting was changed.
