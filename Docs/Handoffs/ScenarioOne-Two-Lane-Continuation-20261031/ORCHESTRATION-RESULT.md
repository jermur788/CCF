# Scenario One Long-Running Deep Orchestration 01 — Result (#201)

**Status: PAUSED — MANAGER DECISION REQUIRED. Not complete, not a Tier-3 candidate.**

Run started 2026-10-31T09:18Z from fresh live state. It stopped at the first genuine authority/product-decision boundary after completing the first two critical verification tasks and partially advancing the third. #200 owns the pending verdicts/disposition.

## 1. Final candidate / integration status

- **No new integration or Tier-3 candidate was created or claimed.** No integration, merge, release or push of `main`/integration branches occurred from #201.
- #196: production candidate remains `e5fb424635f16182748efcb0539e8a49539495a9`; verification branch `verify/mfm-08-mapplanning-contract-reconcile-v1` at `eb4be600` (pushed) is **ready for Manager review**.
- #198: production remains `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`; verification branch `verify/guided-v2-v24-contract-reconcile-v1` at `59b278f7` (pushed) is **ready for Manager review**.
- #199: production remains `ce673714`; branch `fix/scenario-one-combined-hud-focus-regression-v1` at `42742a8a` (pushed) is **partial and not green**; Manager disposition requested.
- `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED` was **NOT** earned and is not claimed.
- No Windows build was started or authorised.

## 2. Material delegated attempts and outcomes

| # | Task | Provider/model | Outcome | Evidence |
|---|---|---|---|---|
| 196 | MFM-08 MapPlanning contract reconciliation | OpenCode DeepSeek V4.1 Flash (orchestrator as default worker) | **DONE — gate PASS.** Verification-only patch + handoff; pushed. Manager verdict pending. | `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`; 0 `error CS`; 8 captures |
| 198 | Guided-v2 v24 / D-066 contract reconciliation | OpenCode DeepSeek V4.1 Flash | **DONE — both gates PASS.** Two verification-only patches + handoff; pushed. Manager verdict pending. | guided-v2 535/0/48; guided-legacy 569/0/62 |
| 199 | Combined HUD / focus regression repair | Claude Sonnet 5.5 Medium (implementation), DeepSeek (Unity execution) | **PARTIAL — blocked on Manager disposition.** Fixture-only fix committed; competitor false failures removed; estimate/minimap + FOCUS contract conflict unresolved. | combined-v2 453/10/24; combined-legacy 440/8/24 |

### Completed

- #196 execution: verification-only navigation-tab contract reconciliation; complete authoritative MapPlanning gate executed and passed; lane/lock returned clean.
- #198 execution: stale v23/pre-D-066 expectations reconciled to the accepted v24/D-066 latch; V2 guided flow completed its accepted five entrance marks; both model branches passed; lane/lock returned clean.

### Failed / partial

- #199 implementation: the competitor fixture defect was fixed and the false 1920 panel/tag failures disappeared; a real 1920 competitor-list clipping defect was exposed; the estimate/minimap + FOCUS family is a conflict between PARITY-07's deliberate minimap step-aside and the protected Combined contract. The child stopped rather than weaken the assertion. Gates are not green.

### Cancelled

- None.

## 3. Provider capacity/auth failures, retries, salvages

- **No provider capacity or authentication failure occurred.**
- **One real-input driver failure:** first `guided-legacy` run failed before confirmation (`State wait failed`; swivel never observed). Classified **ENVIRONMENT/RUNNER**, not capability. Retried once alone on the same tree; PASS. Failed run preserved.
- **One Unity startup hang (twice):** after the reaped combined-v2 Editor, the next Editor stopped before import/compile with no `CHC_` lines. Cause: stale `Temp` state. Cleared only `Temp/*` and stale `Library/*DB-lock` files with no Editor running; normal startup returned. No Library/assets deleted.
- **One orchestration transport issue:** the `delegate_task` MCP wait returned a transport timeout after ~2 min while the Sonnet child continued. Recovered by polling the T3 child thread; the child completed normally. No work lost.
- **Salvage:** the #199 child's partial result was preserved and strengthened with orchestrator gate evidence rather than discarded.

## 4. Routing / decomposition decisions

| Event | Decision | Alternative rejected | Reason |
|---|---|---|---|
| R-001 #196 | DeepSeek default | Sonnet 5.5 Medium | Difficulty 3, bounded state-aware verification, clear authority; DeepSeek-first ladder |
| R-002 #198 | DeepSeek default | Sonnet 5.5 Medium | Difficulty 3, explicit D-066 authority, bounded verification diff |
| R-003 #199 | Sonnet 5.5 Medium | DeepSeek; Opus 5.5 Low | Difficulty 4 cross-screen production UI; ladder names Sonnet; Opus not justified before a classified capability failure |

All capacity-to-completion estimates were **UNKNOWN**; no live quota telemetry was available. Decisions therefore used provider **availability from the T3 catalogue** plus task difficulty/authority, not inferred subscription state. No model/provider was escalated for a gate, environment, lock or authority failure.

## 5. Manager acceptance / rejection

- **#196:** Manager verdict not yet posted (UNRESOLVED). Handoff and comment posted; #200 owns the decision.
- **#198:** Manager verdict not yet posted (UNRESOLVED).
- **#199:** Manager disposition requested (UNRESOLVED). Four options documented; no self-authorisation.

"Worker completed" was never treated as acceptance.

## 6. Unity / editor lanes and serialized operations

- One machine-wide Unity lane: `/media/jer/ZX20/CCF-U1`; all Unity runs serialized; authoritative lock tool `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`.
- Locks acquired/released normally: `e30d8889…` (#196), `4953262b…` and `41efe1bd…` (#198), `9ceb2a14…` (#199). Final live status `free`, metadata `null`, no Editor PIDs.
- Editor side effects (`.vscode/settings.json`, SectionFive materials) restored byte-exactly after every run; final lane clean.
- No staged disposable harness or generated `.meta` left in `Assets/ForestPrototype/`.
- Complementarity: #199 was implemented offline by Sonnet in a separate worktree while #201 retained the Unity lane; no Unity concurrency.

## 7. Worktrees and branches

- `/media/jer/ZX20/CCF-DeepOrch` — orchestration evidence branch `orchestration/scenario-one-deep-orchestration-01` (this ledger/result).
- `/media/jer/ZX20/CCF-U1` — serialized Unity lane; used for all gates; returned to a clean branch state.
- `/media/jer/ZX20/CCF-CombinedHud` — temporary #199 implementation worktree; branch pushed, worktree removed.
- Branches: `verify/mfm-08-mapplanning-contract-reconcile-v1`, `verify/guided-v2-v24-contract-reconcile-v1`, `fix/scenario-one-combined-hud-focus-regression-v1`.

## 8. Rendered / runtime qualification (OBSERVED FACT)

| Gate | Result | Captures | Log SHA-256 |
|---|---|---|---|
| RackV2MapPlanning (#196) | `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8` | 8 | `bae82228…3301c3` |
| guided-v2 (#198) | 535 PASS / 0 FAIL / `GUIDED_ONBOARDING_PASS` | 48 | `3e27e475…39af58` |
| guided-legacy (#198) | 569 PASS / 0 FAIL / `GUIDED_ONBOARDING_PASS` | 62 | `b764de92…58c715` |
| combined-v2 (#199) | 453 pass / **10 fail** / 24 shots | 24 | `99ea9abe…299dd5` |
| combined-legacy (#199) | 440 pass / **8 fail** / 24 shots | 24 | `432e096f…5e400` |

`combined-v2` was reaped after verdict + `Cleanup mono` (documented runner shutdown policy), not because of a check failure.

## 9. Independently reviewed save / reference / determinism work

- None in this run. #196 and #198 are verification-only; #199 touched no production save/reference/determinism code. #198 exercised save/load determinism in the guided gate but is not a save implementation.
- No independent review of any #201 result has occurred; independent review/verdict is #200's pending responsibility (and #199 will additionally require an independent reviewer of the production diff if a production repair is authorised).
- **No protected anchor, fingerprint, baseline or Reference Future/Legacy behaviour was changed or rebaselined.**

## 10. Unresolved work

1. #196 Manager verdict; if accepted, #156 may be upgraded per the #195/#196 review.
2. #198 Manager verdict.
3. #199 direction: production layout repair vs estimate-copy change vs formal assertion supersession vs deferral; then the 1920 competitor-list clipping repair; then independent review.
4. #198/#199 remain required pre-Tier-3 debt if their contracts remain applicable.
5. S1-C T-08 (not released), S1-D T-08 (waiting serialized Unity), MFM-09/10/11/12, PARITY-09, final Tier-3 — all not started.
6. Real-input driver order/timing sensitivity remains workflow debt (one retry needed).
7. Unity stale-`Temp` startup hang after a reaped Editor is an operational hazard; workflow note recorded.
8. Disk was 18–20 GiB free (96–97% used) throughout — above the 15 GiB threshold but marginal for long Unity work. No cleanup performed without authorisation.

## 11. Exact next action

**#200:** review the #196 and #198 handoffs and issue `APPROVED / NEEDS CHANGES / REJECTED`; then decide #199 among the four documented options. On a #199 production-repair authorisation, dispatch a bounded local implementation with the same one-writer/Unity rules, run combined-v2/combined-legacy plus PARITY-07 and affected regressions, and obtain independent review before calling it done.

**#201:** resume only after #200 releases the next bounded task; do not start S1-C/S1-D/MFM/Tier-3 out of order, do not resolve #199's contract conflict unilaterally, and do not start a Windows build.

## 12. Adaptive Routing Learning Assessment (non-authoritative)

### Observed metrics

- Material routing decisions: 3 (DeepSeek, DeepSeek, Sonnet 5.5 Medium).
- First-pass technical success: DeepSeek 2/2 on difficulty-3 verification tasks; Sonnet 1/1 on the difficulty-4 diagnosis/safe partial fix, 0/1 on full #199 completion because of an authority conflict, not capability.
- Escalations to Opus: 0. Escalation-success rate: not measurable.
- Provider capacity/auth failures: 0. Environment/runner failures: 2 (one real-input driver, one stale Unity `Temp` hang); both repaired/retried, 0 model escalations.
- Retries/salvage: 1 real-input retry; 1 delegated-task transport recovery; 1 partial-task salvage.
- Manager acceptances/rejections: 0/0 observed before pause.
- Wall time (observed): #196 ~7 min; #198 ~45 min including one failed 901 s legacy run and two startup hangs; #199 ~75 min including two 40-min/25-min startup hangs and a 585 s v2 gate. Reported tokens/model calls: UNKNOWN (not exposed by the runtime).
- Parallel workers: maximum 2 (one Sonnet implementation child + #201 orchestrator/Unity), never two Unity workers.

### Hypotheses + confidence

- **H1 (moderate, n=2):** DeepSeek-first is sufficient for bounded, well-specified verification-contract reconciliations that include a single Unity gate; no cheaper route exists and no escalation was needed.
- **H2 (moderate, n=1):** Sonnet 5.5 Medium is a good difficulty-4 UI diagnosis worker and correctly respects protected contracts; one partial result is not evidence of capability failure.
- **H3 (high, n=2):** classifying driver/startup failures as ENVIRONMENT before model escalation prevented unnecessary expensive dispatches.
- **H4 (moderate, n=1):** offline implementation in a separate worktree while #201 holds the Unity lane preserved one-writer/one-Unity and saved serial time; the handoff boundary cost ~2 min.
- **H5 (low, n=1):** a stronger model would not have resolved #199; the blocker was a product/authority decision.

### Proposed experiments / policy changes (NOT active until #200/user accepts)

1. Add an operational pre-flight after any reaped Unity Editor: clear `Temp/*` and stale `Library/*DB-lock` before the next gate; treat pre-import hangs as environment.
2. For `delegate_task`, treat an MCP wait timeout as UNKNOWN, not failure; poll the child thread and preserve the child's commit.
3. Keep the guided real-input gate first/alone and treat a pre-confirmation driver state-wait as ENVIRONMENT with one clean retry.
4. When a protected assertion conflicts with a later deliberate product trade-off, stop at Manager disposition before any production layout work; do not spend Opus on the authority gap.
5. Record a disk-margin check before long Unity chains; 18–20 GiB free is marginal and should be treated as a risk flag, not a silent pass.

## 13. Evidence index

- Ledger: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/ORCHESTRATION-LEDGER.md`
- #196 evidence: `/media/jer/Files/CCF/Verification/ScenarioOneMapPlanningReconcile-20261031T092600Z/unity/`
- #198 evidence: `/media/jer/Files/CCF/Verification/ScenarioOneGuidedV2V24Reconcile-20261031T093000Z/`
- #199 evidence: `/media/jer/Files/CCF/Verification/ScenarioOneCombinedHudFix-20261031T101500Z/`
- Handoffs: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/HANDOFF.md`, `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/HANDOFF.md`, `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md`
