# 2026-11-04 — Post-overnight Manager routing packet

**Status:** READY FOR MULTI-LANE NON-UNITY CONTINUATION; implementation not accepted.  
**Parent:** DeepSeek DPC [#203](https://github.com/jermur788/CCF/issues/203); overnight programme [#254](https://github.com/jermur788/CCF/issues/254); Overall Manager #200 owns product decisions and `active-tasks.md`.  
**Purpose:** preserve the reviewed overnight deliverables, avoid unsafe PR merges across diverged Git histories, finish the independent spec reviews and reduce the remaining MFM product ambiguity without disturbing the user playtest.  
**Default workers:** Claude Sonnet 5.5 **Medium**, OpenAI GPT-6.1 **Medium**, choose by actual pool availability and task; independent review with the opposite provider. User authorised higher thinking effort or **Opus 5.5** for demonstrable reasoning/quality needs with justification in V2 ledger. Ticket/quota/context/reset remain **UNKNOWN** absent live telemetry; if blocked, checkpoint and route another safe lane or await an *observed* reset. No invented reset schedules.

## Review outcome and evidence

Overnight return: [OVERNIGHT-ORCHESTRATION-RESULT.md](OVERNIGHT-ORCHESTRATION-RESULT.md) and [30-event V2 ledger](ROUTING-LEDGER.md) on main following docs-only PR #268. The user test branch stayed unchanged. No Unity Editor or Windows build; disk (~19 GiB free) insufficient for an additional cold Library under 15 GiB minimum free-space floor. The original read-only Option A #253 offline analysis is on main through docs-only PR #267 and remains **PARTIALLY QUALIFIED / NEEDS CHANGES**, not an approved MFM model. All other overnight PRs remain outside product integration until separately reviewed.

**Critical branch warning:** [#261](https://github.com/jermur788/CCF/pull/261), [#262](https://github.com/jermur788/CCF/pull/262) and [#264](https://github.com/jermur788/CCF/pull/264) were opened against divergent **main**, showing about 290 changed files each. Compared against their real player-test candidate base, they have small intentional deltas. **Do not merge these PRs into main.** This is a lineage problem, not evidence that the workers intentionally edited 290 files.

**Correction:** earlier #251/#250 text said R1/R2 corridor rectangles physically overlap. Independent #253 analysis determined a **0.214214 m gap**, yet R1/R2 still fail the separate **2.4 m editor centreline-clearance rule**. A Manager erratum now corrects the canonical review/spec wording; original #251 research text remains as historical evidence with a linked dated ledger correction.

## Dispatch these disjoint READY lanes

### Lane A — #269 isolated UI qualification staging

**Difficulty Medium–High**, exact candidate replay and verification, no Unity.  
**Binding:** [ISOLATED-UI-STAGING-PACKET.md](ISOLATED-UI-STAGING-PACKET.md).  
**Owner/branch/worktree:** designated local T3 Code integrator; conditional `integration/scenario-one-overnight-ui-qualification-v1` / `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` (ensure truly unused).  
**Task:** replay only the candidate-relative #255/#256/#260 13-file whitelist, independently diff review, offline 97-check gate + six focused snapping cases + combined checker with expected CTA-only failure. Publish `ISOLATED-UI-STAGING-HANDOFF.md` and small PR targeted at candidate lineage. **No Unity until separate disk-safe qualification.**  
**Claim:** https://github.com/jermur788/CCF/issues/269

### Lane B — #270 final independent review of DRAFT UX specs

**Difficulty Medium–High**, read-only final review, no Unity.  
**Evidence:** #257 PR #263 (input/sensitivity, binding and consented feedback); #258 PR #265 (next-action CTA and map-like diagram); #259 PR #266 (marking shapes/colour/sourcing).  
**Worker:** reviewers independent of each spec's author; use Sonnet 5.5 Medium for GPT-authored docs and GPT-6.1 Medium for Sonnet-authored docs when available.  
**Task:** source-backed final review, re-run numeric protocol and exact policy assertions where available, reproduce or explicitly leave OPEN the **Method B inventory command** (73/61), check #258 foreground-input ownership and screen reader/keyboard behaviour, check #259 Pro Silva source claims/ΔE76 versus background WCAG meaning, and produce one compact **user/Manager decision table**. Do not approve defaults or change production. Publish `FINAL-DRAFT-SPEC-REVIEW-HANDOFF.md` via docs-only PR.  
**Claim:** https://github.com/jermur788/CCF/issues/270

### Lane C — #271 MFM Option A vs parallel fallback comparison

**Difficulty High**, research and offline-only math, no Unity.  
**Owner/branch/worktree:** separate docs lane `docs/mfm-a-vs-c-offline-20261104-v1` at `/media/jer/Files/CCF/Operations/CCF-MFM-A-vs-C-Docs-20261104` only if unused.  
**Task:** apply identical **experimental** reach caps and sample-policy to both options, identify uncovered/manual share and source stem/mask limitations, test whether C can meet the user-requested full-spine 45° target without inventing geometry, preserve legacy/parallel baseline, report NO-COMPARISON honestly. Publish `MFM-A-VS-C-COMPARISON-HANDOFF.md` and independent review before any Option A production spec approval.  
**Claim:** https://github.com/jermur788/CCF/issues/271

These three lanes do **not** overlap source-writing paths. Only one editor of each handoff/PR file set at a time, and only **DPC** appends to the shared overnight ledger. If an ownership/branch/path collision is observed, STOP the affected lane and seek #200 allocation; progress independent lanes only.

## Shared routing, verification and reporting requirements

- DPC must append *each material model choice, delegated task, quota/context event, blocked attempt, correction, review and handoff* in [ROUTING-LEDGER.md](ROUTING-LEDGER.md) as an actual GitHub update, not retrospectively summarise from memory; source label OBSERVED FACT/ORCHESTRATOR CLAIM/RETROSPECTIVE INFERENCE. Unknown metrics remain **UNKNOWN**. For #253/#271 optionally link the [access-spine programme ledger](../ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md) rather than overwriting historical entries. Re-extract every published number and review total from raw evidence; preserve corrections.
- Claims mark [RUNNING] **only when actual work starts**, with exact provider, effort, branch/worktree and starting ref, then [HANDED-OFF] with direct GitHub handoff link and review status. DPC coordinates; #200 is the only writer of `Docs/Project/coordination/active-tasks.md`. No child claim is permanent production ownership.
- Tests must be proportionate. A source-static check cannot prove physical rack drag, visual map/3D consistency, save/load or Unity compile. **Never weaken tests to get a green report.** The still-unimplemented Objective 1 CTA is expected to remain FAIL and must be reported as such.
- Legacy40, frozen Reference Future v1 and published personal-test candidate/Save untouched. No Rack V2/MFM DRAFT implementation, interior rack entrances, unapproved tree-mark visual semantics or automatic Fell. **No Windows build** until the user completes their personal Scenario One playtest and explicitly authorises afterward.
- Any future Unity candidate must pass its own fresh deployed lock/process/disk and exclusive ownership admission; cannot use the user's warm Library or discard unrelated files to solve storage without explicit safe approval.
- At handoff, report what changed, what passed/failed, corrected errors, reviewed vs implemented vs integrated vs verified vs accepted, exact outstanding product decisions, dashboard synchronization request and user action.

## Paste-ready dispatch to DeepSeek DPC

> DeepSeek DPC: continue CCF management-safe work from https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Overnight-Continuation-20261104/POST-OVERNIGHT-REVIEW-AND-ROUTING-PACKET.md . Execute three independent non-Unity claims serially or in parallel according to actual ownership and capacity: #269 exact-candidate staging of #255/#256/#260 deltas (never merge 290-file PRs to main), #270 independent final review of UX draft specs #257–#259, and #271 offline MFM A-versus-C comparison. Start with Sonnet 5.5 or GPT-6.1 Medium; escalate to higher effort/Opus 5.5 only with logged evidence. Continuously append V2 routing choices/limits/retries/reviews to the overnight ledger with UNKNOWN where not observed and reconcile raw numerical results. Publish the specified durable handoffs/PRs, respect exact file/worktree locks, and return to Manager for integration/product decisions. No Unity on insufficient disk headroom, no DRAFT spec implementation, no Windows build.

**User action required:** send this dispatch to the active DeepSeek DPC orchestration chat. The Manager cannot initiate unattended local agents from browser GitHub tools.