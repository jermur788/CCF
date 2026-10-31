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

---

# Scenario One Local Deep Orchestration Resume 02 — Result (#203)

**Status: PAUSED — MANAGER VERDICT + REPIN REQUIRED. #205 is returned green; #199 is authorised but deliberately not started.**

The preceding #201 result is preserved verbatim above. This is the #203 continuation summary, started 2026-10-31T12:26Z from fresh live state.

## 1. What changed

| Task | Provider/model | Outcome | Exact SHAs |
|---|---|---|---|
| **#205** MFM-08 Integration 01 | OpenCode `opencode-go/deepseek-v4.1-flash` (orchestrator + worker) | **DONE — returned green.** Exact approved delta replayed with no conflicts; post-replay Unity/rendered gates pass. Manager verdict pending. | start `ce673714`; integrated production `34e360f8`; docs tip `3920ef14`; qual harness `c9336186` |
| **#199** Combined HUD / Focus Option 1 | — (Sonnet 5.5 Medium planned) | **NOT STARTED — correctly blocked.** Manager packet forbids starting from the stale `ce673714` base while #205 owns the surface. | stale partial branch `42742a8a` preserved |
| **S1-C T-08 / S1-D T-08** | — | **NOT STARTED.** S1-C not released; S1-D requires a Manager repin decision after the lineage change. | — |
| **MFM-09+ / PARITY / Tier-3** | — | Not released; depend on accepted MFM-08 integration. | — |

No new integrated Tier-3 candidate was created or claimed. `SCENARIO ONE PLAYER-TEST CANDIDATE READY` was not earned and is not claimed. No Windows build.

## 2. #205 integration evidence (OBSERVED FACT)

- Replay: `cherry-pick cfdc93bf..e5fb4246` onto `ce673714` — **no conflicts**.
- Equivalence: `git diff ce673714..34e360f8` is byte-identical to `git diff cfdc93bf..e5fb4246` (both SHA-256 `9715913ceb7c95379c7978d62f849a17752f8a7c1865cebd5fa2a5a1e8ce7101`); 15 files; the 29 later integration commits are preserved.
- Independent overlap check: the later lineage changes no path touched by the MFM-08 delta (empty intersection).
- Not absorbed: #196 `eef1bdf9`, #199 `42742a8a`, unrelated verification-branch docs.
- Unity compile clean: 0 `error CS`, exit 0 in both gate runs.
- Focused MFM-08 (#156 Tier-2 gate): `MFM08_UNITY_PASS checks=373 shots=25`, 1280×720 and 1920×1080.
- Authoritative RackV2MapPlanning under the approved #196 contract: `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`.
- Save/reference compatibility: `PARITY08_SPECIES_OFFLINE_PASS checks=23` (incl. `teaching latch does not move WorldHash`); replay diff contains no save/schema/reference/anchor path.
- Offline MFM-08 logic: `MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0`, repeat byte-identical. `MFM08_BOUNDARY_FAIL` is expected (pin is the MFM-08 start commit) and is not a defect on an integration lineage.
- Cleanup: locks `f005c4cf…`/`fe3c16f3…` released normally; live `free`; editor side effects restored byte-exactly; lane clean; no Editor/Xephyr.
- Evidence: `/media/jer/Files/CCF/Verification/MFM08-PostIntegration-Unity-R2-20261031T1245Z/`, `…-MapPlanning-20261031T1250Z/`, `…-Parity08-20261031T1255Z`, `…-Offline-20261031T1235Z`.

## 3. Failure classes and retries

| Event | Class | Retries | Model escalation? |
|---|---|---|---|
| `MFM08_UNITY_FAIL … unchanged save version 23` at `d99d2892` | Stale verification contract (harness pinned pre-v24 schema) | 1 (verification-only pin reconciliation + re-acquire) | **No** — a repository fact, not a reasoning gap |
| `MFM08_BOUNDARY_FAIL` in the offline gate | Expected scope-check mismatch on an integration lineage (pin = MFM-08 start commit) | none | No |
| Coordination lag: `active-tasks.md` not yet reconciled at 12:30Z | Coordination latency | none | No |

No provider capacity, auth or login failure occurred. No environment/Unity startup hang occurred this run (no Editor was reaped mid-chain; `Temp` was already clear). No salvage was needed beyond reusing the existing #199 partial branch and the existing registered U1 lane.

## 4. Routing decisions this continuation

| Event | Decision | Alternatives rejected | Reason |
|---|---|---|---|
| R-004 #205 | DeepSeek default worker | Sonnet 5.5 Medium | Mechanical exact-delta replay with provably empty overlap + verification; packet reserves Sonnet for genuine conflict/architecture work, neither occurred |
| #199 Option 1 (not yet dispatched) | Sonnet 5.5 Medium planned | DeepSeek; Opus | Difficulty-4 cross-screen responsive layout with two protected contracts; Manager packet names Sonnet; Opus not justified |
| S1-D T-08 | Not dispatched | — | Released against `ce673714`; live Manager note requires a repin decision after #205 changes the lineage |

All capacity-to-completion estimates remain **UNKNOWN**; no live quota telemetry was available. No model or provider was escalated for a test, environment, lock, quota or authority failure.

## 5. Unity lane and serialization

- One machine-wide lane: `/media/jer/ZX20/CCF-U1`; one Editor at a time; authoritative lock tool used for every acquisition/release.
- Two locks acquired and released normally; final status `free`, metadata `null`, no Editor PIDs.
- Implementation and verification work stayed in the same lane for this task because there was exactly one child and no competing writer.
- Disk 18 GiB free on ZX20 (97 % used) throughout — marginal, treated as a risk flag, no unauthorised cleanup.

## 6. Worktrees / branches

- `/media/jer/ZX20/CCF-DeepOrch` — `orchestration/scenario-one-local-deep-orchestration-02` (this ledger/result).
- `/media/jer/ZX20/CCF-U1` — integration + Unity lane; branches `integration/mfm-08-onto-scenario-one-v1` (`34e360f8`, docs `3920ef14`) and `qual/mfm-08-post-integration-v1` (`c9336186`).
- `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a` — preserved, not merged, not rebased.

## 7. Unresolved work / exact next action

1. **#200:** review #205 and issue `APPROVED / NEEDS CHANGES / REJECTED`; on approval, integrate/publish and then repin **#199** to the exact combined SHA.
2. On the #199 repin, **#203** dispatches Sonnet 5.5 Medium for the production layout repair (restore minimap for ordinary estimate inspection; preserve PARITY-07 copy/prompt/detail clearance; repair 1920 V2 competitor-list clipping; no copy shortening, no assertion weakening), then runs combined-v2/legacy at both sizes + PARITY-07 + focused focus/minimap checks, obtains independent review, and returns the success token.
3. **#200:** decide whether S1-D T-08 requires a fresh repin after #205; release S1-C T-08 when it is the preferred next slot.
4. MFM-09 follows accepted/integrated MFM-08; MFM-10 must return `SPECIES GUARD QUALIFIED`; PARITY-09/MFM-11/MFM-12 then Tier-3.
5. #194 remains **NO ACTION**. No Windows build before the user's personal playtest and explicit authorisation.

## 8. Adaptive-routing learning (non-authoritative)

- **Observed metrics:** 1 material routing decision executed (#205 → DeepSeek); 0 escalations; 0 provider failures; 1 retry (stale pin); 1 verified integration; 0 Manager acceptances observed.
- **H1 (moderate, n=1):** exact-delta integration with a machine-checkable equivalence criterion is Tier-A work for DeepSeek, including post-replay Unity composition.
- **H2 (high, n=3 cumulative):** classifying stale-pin/driver/startup failures as contract/environment before model escalation repeatedly avoided unnecessary expensive dispatches.
- **H3 (low, n=1):** recycling a qualification harness onto a newer lineage is a recurring stale-pin hazard; grep for schema/version pins before acquiring Unity.
- **Missing telemetry:** token counts, model calls, quota — UNKNOWN.
- Proposed policy changes are **NOT active** until #200/user accepts them.
