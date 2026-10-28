# HANDOFF — MODEL-CAPABILITY-COST-RESEARCH-CLAUDE-20261028 (Blind Independent Replication)

**Date:** 2026-10-28
**Claim:** GitHub issue #57
**Verdict:** Phase A and Phase B complete and published. Research-only. **Not Unity-verified; no Unity was involved.**

## Publication

| Item | Value |
|---|---|
| Starting HEAD | `b70206fa69b2777a797105af2d65176b8b78779f` (`origin/main`, descendant of dispatch `8ed6838`) |
| Branch / worktree | `research/model-capability-cost-claude-replication-20261028` at `/media/jer/ZX20/CCF-ModelCostClaude` (new worktree; the dirty `CCF-Primary` checkout and the existing `CCF-ModelCost` worktree were not touched) |
| **Phase A report** | `Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md` |
| **Phase A frozen commit SHA** | **`3190b08b7016eaf93c87e7dc21e25728b355ef39`** — pushed and recorded on issue #57 **before** the OpenAI report was opened |
| **Phase B report** | `Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md` |
| **Phase B commit SHA** | **`393945241781235b91bca5b797199295a2de67d1`** |
| Handoff | this file, committed as the branch tip after Phase B (a file cannot name its own commit; see `git log` for the tip) |

`git diff b70206f <tip> --name-only` shows exactly three paths: the two reports and this handoff. The OpenAI report, its handoff, Manager reviews and the original task packet are unmodified.

## Model / product / provider used

- **Model:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), **effort `high`**.
- **Product / wrapper:** Claude Code (Agent SDK) hosted in T3 Code, via a Claude **software subscription** (no API key; no API billing).
- **Provider instance:** `claudeAgent_claude_b_pro` ("Claude B — Pro"), as reported by T3 `orchestrator_capabilities`. This was **not known during Phase A**; Phase A says "not identifiable". Correction C1 in the Phase B report.
- **Subscription 1 vs 2:** **unverified.** The config dir `/home/jer/.claude_ccf2` and the "Pro" instance label do not prove which of the user's two €20 accounts this is.
- **Preference-order deviation (needs Manager/user attention).** The packet prefers (1) the strongest Claude model, (2) Opus 5.5, (3) Sonnet 5.5 at high effort. The live T3 catalog lists Opus 5.5 and Fable 5.1 as selectable for both Claude instances. This run used option (3). I did not inspect the catalog before starting, cannot switch my own model, and would not select Fable without confirming it does not bill usage credits. Catalog presence is not an entitlement test. **Phase A was therefore produced by the lowest-preference permitted model.** A stronger-model replication would need a fresh blind run; I did not start one.

## Source-check date

2026-10-28 for all web sources (listed in each report). Several pages could not be read (`chatgpt.com/pricing` 403; one Anthropic help article 404; Artificial Analysis coding-agent page exposed no extractable values). `WebFetch` answers come from a summarising model, so figures are extraction-level.

## Major agreements (brief)

Quota/handoffs/review burden dominate nominal price; subscription quota must not be converted to tokens or euros; wrappers (T3, TTree, Codex, OpenCode, Claude A/B) are not models; `gpt-6.1-sol`/`terra`/`astra`/`luna` are real public IDs; DeepSeek V4.1 Flash needs the **Global** region per Go docs and the user's Europe setting does not prove it; Sol leads local Unity work; different-provider review for consequential work; cross-family benchmark comparison is unsafe; both propose a measured CCF log before any routing change.

## Major disagreements (brief; full classification in the Phase B report)

1. **"Best quality" slot:** OpenAI report names Astra/Fable (if included) for planning, research and adversarial review without numbers; Claude report finds Opus 5.5 ≥ Fable 5.1 in Anthropic's and Artificial Analysis' tables, Fable possibly billing usage credits, and Astra only 5–45 messages per 5 h on Plus. **Cause: evidence selection.**
2. **Value default for bounded fixes, Git, docs:** OpenAI report defaults to Terra (despite its own summary calling Go the "first stop"); Claude report defaults to DeepSeek/MiMo using the Go allowance table. Both lean toward their own family in the value column.
3. **Second Claude subscription:** OpenAI report "do not assume separate pools"; Claude report treats them as independent (primary + reserve) on thin evidence.
4. **Evidence standard:** OpenAI report cites no numbers but cites benchmark-integrity literature; Claude report cites numbers but not integrity caveats.
5. **One factual contradiction:** the OpenAI report calls the historical MiMo worker unresolved; the repo handoffs and the live catalog name **`opencode-go/mimo-v2.6-pro`**. Cause: bounded catalog search (2 results omitted) and reliance on `active-tasks.md`.

## Post-freeze factual corrections to Phase A (listed, not edited into the frozen file)

C1 session identity/effort; C2 Opus/Fable visible in T3 catalog (entitlement still unverified); C3 Plus 5-hour limit is now officially confirmed; C4 `mimo-v2.6-pro` in live catalog; C5 Terra long-context is documented (1.05M) and its model page does not mention coding; C6 DeepSeek 1M/384K/MIT values are third-party, not on DeepSeek's 1017 page.

## Unresolved account / model / access questions

- Which Claude account is subscription 1 or 2; Claude A's plan tier; "Claude B — Pro" ↔ subscription mapping.
- Whether Opus 5.5 and/or Fable 5.1 are covered by the included limits of either €20 plan, or bill usage credits (check the `/model` picker's "Requires usage credits" label and Settings → Usage on each account). Whether usage credits are enabled on either account was **not checked**. Under Claude Code docs, a non-interactive/Agent-SDK session with credits enabled bills Fable **without a consent prompt**.
- Claude Pro weekly allowance in any unit (unpublished).
- Whether DeepSeek V4.1 Flash is enabled in the user's OpenCode workspace because Global is on, or for another reason (CCF runs it routinely). No settings were inspected or changed. DeepSeek's ZDR agreement is documented as valid only through **2026-10-31**.
- `gpt-6x`, "Astra" as a CCF asset label, "TTree Code": unresolved.
- EUR/VAT prices and plan tiers of all four subscriptions.

## Confirmation: OpenAI report not read before Phase A was committed

**Confirmed.** Until commit `3190b08`, the OpenAI-produced report, its Manager reviews/amendment, its worker handoff, and PR #55/#56 content were not opened, searched, quoted or inferred from. The worktree was a sparse checkout with those paths absent from disk; the original task packet was read via `git show` (permitted). The sparse checkout was disabled only after the Phase A SHA was pushed and recorded on issue #57. Limits of this assertion: it relies on the researcher's own conduct plus the verifiable ordering (commit timestamp, push, issue comment). The repository's own docs may contain routing statements derived from the earlier research; I read `AgentWorkflow.md`, handoffs and `active-tasks.md` for project evidence and cannot rule out incidental influence from those.

## Validation performed

- Report structure checked against the packet's 11 required Phase A sections and the Phase B comparison list.
- `git diff --name-only` against the starting HEAD shows only the three intended documents.
- The Phase A file was not modified after its commit (`git diff 3190b08 <tip> -- <phase-A-path>` is empty).
- Facts used in corrections were re-checked against live sources (T3 catalog, OpenAI/DeepSeek pages) on 2026-10-28.
- **Not done:** no Unity, no code, no tests, no account/billing/region inspection, no independent re-verification of third-party benchmark numbers at raw-page level.

## Dashboard / context status

No dashboard, generated-context, mirror, `active-tasks.md` or decision-log edits. No routing-policy change. Dashboard status: **unchanged / not touched**.

## Claim

Issue #57 is marked `[HANDED-OFF]`, linked here, and closed.

`CLAUDE MODEL RESEARCH REPLICATION COMPLETE — READY FOR MANAGER REVIEW`
