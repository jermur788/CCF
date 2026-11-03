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

---

# Unified Local Master Handoff 2026-11-01 — Run Summary (#203)

**Status at this append: PAUSED AT MANAGER BOUNDARY — #213 returned with token withheld; #199 Option 1 returned and independently endorsed for Manager review.**  
**Authority:** `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md` at `6c77907e`; live coordination `active-tasks.md` on `origin/main` `d033923c`.  
**Published integration:** exact `6784ab17cac922a16815f4898a30737482101018`.

## 1. Children attempted

| Child | Provider/model | State | Exact SHA / branch | Evidence |
|---|---|---|---|---|
| **#213** MFM-10 local residual | DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`) | **EXECUTED — TOKEN WITHHELD at Manager disposition boundary** | `verify/mfm-10-species-guard-v1` from `6784ab17`; docs commit `43e26370`; branch pushed | `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`; `/media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/` |
| **#199** Option 1 production repair | Mimo 2.6 Pro (`opencode-go/mimo-v2.6-pro`, xhigh) | **COMPLETE — READY FOR MANAGER REVIEW (independently endorsed)** | branch `fix/scenario-one-combined-hud-focus-regression-v1-option1`; production `3acdf9cb`; handoff `0d889898`; independent review `48e96ebd`; pushed | `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md` + `INDEPENDENT-REVIEW-20261101.md`; `/media/jer/Files/CCF/Verification/ScenarioOneCombinedHudFocusOption1-20261101/` |
| PARITY-09 / MFM-11 / MFM-12 / Tier-3 | — | **NOT RELEASED** | — | blocked on #200 acceptance/release |
| #194 Research Source Consolidation | — | **NO ACTION** | — | untouched |

## 2. #213 result (OBSERVED FACT)

- Command 1: `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; `MFM04_REPEAT_PASS two processes, byte-identical output: True`.
- Command 2: exit 1 — `THREE_SPECIES_OFFLINE_FAIL failures=1 checks=91` (scanner false positive on `MaxRemovedBasalShare`) and `COMPOSITE_DENSITY_FAIL checks=107 failures=3` (three pure-Beech R2 misses 0.423/0.407/0.417 vs unchanged 0.40; already adjudicated ACCEPTED KNOWN C-LEVEL LIMITATION). Isolated suites PASS; identical rerun.
- MFM-10 species-guard/parity assertions pass; no Unity required; no production/test/threshold change.
- **`SPECIES GUARD QUALIFIED` was not issued** because the published gate requires both commands to exit 0. #200 must either accept the two documented pre-existing red families as out of MFM-10 scope or release a bounded verification-asset repair for the scanner false positive.

## 3. #199 outcome (OBSERVED FACT + ORCHESTRATOR CLAIM)

- Mimo 2.6 Pro completed the substantial production repair in one delegated run: baseline reproduction, iterative layout changes, combined gates, PARITY-07, Rack confirmation, guided-v2 and handoff. DeepSeek remained orchestrator and independent reviewer.
- **Production commit `3acdf9cb`** (3 UI files + accepted verification-only fixture correction), handoff `0d889898`, independent review `48e96ebd`; branch pushed to origin.
- **Combined gates:** `combined-v2` 453/10/24 → **465/0/24**; `combined-legacy` 440/8/24 → **450/0/24**.
- **PARITY-07:** `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0`. **RackV2ConfirmMinimap:** `RACK_V2_CONFIRM_MINIMAP_UNITY_PASS checks=213 shots=21`.
- **guided-v2 residual independently proven pre-existing:** the orchestrator released the worker lock, acquired lock `3bb093b4…`, detached U1 at exact `6784ab17`, and ran guided-v2 baseline. Baseline and final both `checks=553`, `shots=47`, `passed=false`, **identical 11 failures**. The failures include stale v23 schema pins, Objectives-window overflow and teaching-sequence index mismatches; none is in a file touched by #199.
- **Visual review:** estimate panel 400 wide at right 220 clears the minimap; minimap visible during ordinary estimate inspection; prompt clear; competitor list fits at 1920 V2. Disclosed trade-offs: estimate text 15/14/13px, competitor row padding 3→1px, side-panel gap 8→2.
- **Process deviation:** the worker updated the lock JSON HEAD to `3acdf9cb` outside the acquire/release history. Same owner/task, no competing writer; gates not invalidated. The orchestrator released/re-acquired normally for the baseline and left the lock free.
- **Verdict:** `SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`; independent review **ENDORSED FOR MANAGER REVIEW**. No self-acceptance, integration or Windows build.

## 4. Routing / capacity observations

- **R-006 (#213 → DeepSeek):** deterministic offline runner work; cheapest capable route; completed without escalation. Capacity-to-completion SUFFICIENT (observed).
- **R-007 (#199 → Mimo 2.6 Pro):** substantial cross-screen UI/focus production child; EXPLORE with Sonnet-class EXPLOIT fallback. Capacity evidence: user reports substantial remaining tokens (qualitative; exact UNKNOWN); model existence confirmed by T3 catalogue.
- DeepSeek: AVAILABLE/executing; exact quota UNKNOWN. Mimo: user-reported high; exact UNKNOWN. Claude: UNKNOWN, not used.
- No failure was classified as a model-capability failure. The #213 red families are verification/calibration facts, not reasoning gaps.

## 5. Locks / worktrees / storage

- #213: `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101` (offline NonUnity-class lane on Files); no Unity lock; clean.
- #199: `/media/jer/ZX20/CCF-U1`, branch `fix/scenario-one-combined-hud-focus-regression-v1-option1` at `48e96ebd`; worker lock `9dc7b6ad…` released normally; independent baseline lock `3bb093b4…` acquired/released; machine-wide lock **free** at append time; no Editor/Xephyr/openbox; worktree clean; Editor side effects restored.
- Storage: ZX20 17.54 GiB free (above 15.00 GiB floor); Files 57.1 GiB free.
- No protected anchor/baseline changed by the orchestrator. No Windows build. #194 untouched.

## 6. Exact next action

1. **#200 reviews #199** (`HANDOFF.md` + `INDEPENDENT-REVIEW-20261101.md`, branch `fix/scenario-one-combined-hud-focus-regression-v1-option1`) and either accepts it for integration or returns the visual trade-offs for a follow-up. Do not integrate from this local result alone.
2. **#200 reviews #213** and either explicitly accepts the two documented pre-existing red families as out of MFM-10 scope (issuing `SPECIES GUARD QUALIFIED`) or releases a bounded verification-asset repair for the `MaxRemovedBasalShare` scanner false positive.
3. Only after #200 release: PARITY-09 / MFM-11 / MFM-12 / Tier-3. No self-release; no Windows build before user playtest + explicit authorisation.

---

# PARITY-09 #219 + MFM-11 #220 Continuation — Run Summary

**Status at this append: PARITY-09 evidence + Claude APPROVED (Manager verdict pending); MFM-11 revised Unity run returned `QUALIFICATION INCOMPLETE — GAP`; independent review running.**

## 1. Children

| Child | Provider/model | State | Exact SHA / branch | Evidence |
|---|---|---|---|---|
| **#219** PARITY-09 | DeepSeek V4.1 Flash (qualification) + Claude Sonnet 5.5 Medium (independent review) | **QUALIFICATION COMPLETE — INDEPENDENT REVIEW APPROVED; Manager verdict pending** | `qual/parity-09-integrated-v1` pushed; handoff `aa8eb6ff`; review `87aee15f`; exact `3acdf9cb` | `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/HANDOFF.md` + `INDEPENDENT-REVIEW.md`; `/media/jer/Files/CCF/Verification/PARITY-09-20261101/` |
| **#220** MFM-11 | Mimo 2.6 Pro (`opencode-go/mimo-v2.6-pro`, xhigh) | **REVISED UNITY RUN COMPLETE — `QUALIFICATION INCOMPLETE — GAP`; independent review running** | `verify/mfm-11-calibration-impact-v1` in U1; offline `66476643` (27/0); Unity run 2 `822e474c`; handoff `1249c782` | `Docs/Handoffs/MFM-11-Calibration-Impact/MEASUREMENT-DESIGN.md` + `HANDOFF.md`; `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/` |
| MFM-12 / Tier-3 | — | **NOT RELEASED** | — | blocked on accepted MFM-11 |
| #194 | — | **NO ACTION** | — | untouched |

## 2. PARITY-09 result (OBSERVED FACT)

- New Unity PASS on exact `3acdf9cb`: adult 30/30 at Legacy40 and Enlarged80; DensityQualification 5 hard checks (anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, 24/24, 356 victims); completion model-2 Legacy40 12 phases, `9C77EB4BEE427FB9`, cash 613,978, completion year 25; Reference archive/preview/continuation exact; SaveHardening PASS.
- Offline PASS: early-life 30; player-facing 89/89; Parity08 23; MFM prescription core 240/0 byte-identical; MFM save provenance 172; timber 73; density-isolated and Sitka-isolated.
- Red items are verification debt/accepted limitation, not new defects: five stale v23 save pins; `MaxRemovedBasalShare` scanner false positive; three accepted Beech R2 misses; history csproj missing `ScenarioManagementHistory.cs`; 11 pre-existing guided-v2 failures.
- **Claude Sonnet 5.5 independent review: APPROVED.** No new production/spec defect. Residual: no Unity timber/pruning assertion on `3acdf9cb` because those gates abort on the v23 pin; the review judges accepted PARITY-06 + offline timber + byte-identical sources + UI-only #199 delta sufficient, and recommends a non-blocking verification-only repin.
- `THREE-SPECIES PARITY P0 VERIFIED` is supportable after #200 acceptance; not self-issued.

## 3. MFM-11 offline + run 5 equal-horizon matrix (OBSERVED FACT + ORCHESTRATOR CLAIM)

- Mimo built a verification-only offline harness and measurement design. Independent orchestrator rerun: `MFM11_CALIBRATION_IMPACT_PASS checks=27 failed=0`, byte-identical repeat.
- Path A sample-only: 34 stems / 0.521 m² BA removed; Path B prescription: 118 stems / 2.366 m²; Path C manual override: 85 stems / 1.091 m². All per-unit safeguards ≤40%; canopy retained 89.8% (B) / 95.3% (C); CI parity 3.76%; growth drift 0.076 cm; species guard green (single-species fixture).
- **Operative run 5** (`MFM11_UNITY_PASS checks=47 shots=2`, 322.4 s, HEAD `f642550c`): equal horizon year 5 for A/B/C. Path A 1177 living / 29.497 m² / canopy 0.957621 / light 0.042379 / regen 86 / cash 776854; Path B 1093 / 27.308 m² / 0.939526 / 0.060474 / 106 / 594310; Path C 1168 / 29.321 m² / 0.956893 / 0.043107 / 89 / 531053.
- **Completion demonstrated:** `Outcome.Completed` at year 25, 8/8 objectives; beech ×3 and oak ×3 planted through the ordinary shop → Pending → Work Plan approval → execution path; second intervention at year 17.
- **Timber/economy reconciled:** B −182,544 = 67,456 revenue − 250,000 contractor; C −245,801 = 4,199 − 250,000; both observed cash deltas match exactly; the B/C anomaly is the fixed 250,000-cent minimum-job fee against B's 12.54 m³ vs C's 0.89 m³.
- **MFM-03:** y2 extension-state CI parity 4.17% (13 covered areas, within 20% GAMEPLAY CALIBRATION); post-B y5 CI parity `UNAVAILABLE` (0 covered areas, `PlayerCompletedWork=13`); y25 `ReadCoverage` refuses because the sample qualification degraded (2/3 crop trees, release 0.2619). Cross-year CI parity is **structurally unmeasurable**, reported as a gap, not silently resolved.
- **Protected compatibility:** labelled PARITY-09 Reference/Legacy40 evidence cited + save/load self-consistency only; no frozen-pin comparison in this gate.
- **Worker verdict:** `QUALIFICATION INCOMPLETE — GAP` (handoff `45cad2cf`); no material accepted-outcome break, no `BLOCKED — CALIBRATION IMPACT`; token not issued.
- **Review capacity event:** Claude Sonnet 5.5 returned `API rate limit reached` on the round-2 re-review. Quota/capacity failure, not model capability. The re-review was reassigned to OpenAI Codex `gpt-6.1-sol` high (separate provider/model from Mimo); Claude's first `NEEDS CHANGES` review at `2bb42a36` is retained.
- **Codex round-2 re-review: `APPROVED`** (review `fb13792b`, branch pushed). It independently verified the equal horizons, timber/economy reconciliation, completion 8/8 and handoff corrections from the raw run-5 markers. It recommends accepting the measured y2 actual-extension CI parity (4.17%) and retaining post-treatment source-vs-Covered CI parity as **NOT DEMONSTRATED** (structural, not a numerical failure), and accepting the scoped exact-candidate PARITY-09 Reference/anchor/completion evidence for protected compatibility.
- **Token:** `MULTI-SCALE CALIBRATION QUALIFIED` is **supportable after #200 accepts those two evidence-boundary decisions**; not self-issued. No additional mandatory measurement run or production retune requested.
- **Cleanup:** U1 clean at `fb13792b`; lock free; no Editor; no staged harness; no production changes; no Windows build; #194 untouched.

## 4. Routing / capacity

- **R-008 (#219 → DeepSeek, review → Claude Sonnet 5.5 Medium):** DeepSeek for evidence/gate execution; Claude cross-provider for the independent review. Claude AVAILABLE in the catalogue; quota UNKNOWN; review completed.
- **R-009 (#220 → Mimo 2.6 Pro xhigh):** substantial open-ended quantitative qualification; user-authorised capacity; offline phase completed; Unity phase continuing.
- DeepSeek orchestrator; exact token/call/quota telemetry UNKNOWN. No failure classified as a model-capability failure.

## 5. State

- U1 now on `verify/mfm-11-calibration-impact-v1` at `66476643`; MFM-11 Unity phase owns the lane.
- PARITY-09 branch pushed; U1 clean at handoff/review before the switch.
- Machine-wide lock free at this append; no Editor/Xephyr/openbox.
- No production edits, no Windows build, #194 untouched.

## 6. Exact next action

1. MFM-11 Unity phase completes and publishes its handoff/evidence (or returns `BLOCKED — CALIBRATION IMPACT`).
2. Independent review of MFM-11 by a separate reviewer/thread (Claude Sonnet 5.5 preferred; DeepSeek fallback if Claude is unavailable).
3. #200 reviews #219 and #220; accepted MFM-11 releases MFM-12. No self-release; no Windows build.

---

# Scenario One Final Tier-3 #231 — Result (backfilled 2026-11-03 under #241)

**Status at checkpoint: BLOCKED / NEEDS MORE EVIDENCE. Not a Tier-3 pass. No player-test token.**

This section appends the missing 2026-11-03 #231 episode; it does not rewrite any earlier conclusion and does not change the qualification verdict.

## 1. Routing and exact identity

| Field | Value |
|---|---|
| Claim | #231; parent #203; Browser Overall #200 |
| Exact candidate | `6849caa0ef05baf1a52b95465ec6bba4ff94202b` |
| Branch / lane | `qual/scenario-one-final-tier3-v1`; `/media/jer/ZX20/CCF-U1` |
| Evidence head | `a95c4c4e35decb7a518d543488678473430f69b6`; production diff to candidate empty |
| Qualification worker | OpenCode `opencode-go/deepseek-v4.1-flash` (reported; unobserved identity UNKNOWN) |
| Independent reviewer | OpenAI GPT-6.1-Sol, high reasoning, read-only cross-provider |
| Duration | Approximately 07:49–12:45 UTC, about 4 h 56 min |
| Outcome | BLOCKED / NEEDS MORE EVIDENCE; independent verdict `NEEDS MORE EVIDENCE` |

**ORCHESTRATOR CLAIM:** the prepared preference was Claude Opus 5.5 Low; the run used the release/repin fallback to an equivalently capable local Unity/code worker. **UNKNOWN:** whether the preferred Anthropic lane was operationally unavailable. No production-repair authority existed inside #231.

## 2. Qualification outcome

The reduced plan retained the highest-value current-candidate evidence and completed:

- exact-candidate compile/import;
- mandatory fresh S1-C T-05 v24 objective/progression save/history chain — 544 checks / 34 cases / 12 shots, plus rendered recorded/derived/not-recorded states at 1280×720 and 1920×1080;
- current v24 / old v20–v21 / malformed-save rejection-before-mutation / provenance and hardening;
- frozen Reference Future and Legacy40 protection;
- MFM approval/coverage/extension/treatment-review and complete-loop data through Year-25 completion, 8/8 objectives and solvency;
- S1-C T-02/T-03/T-04/T-06 and S1-D guide/previous-vs-proposed plus the three-cycle history witness;
- Rack confirm/field/save/persistence and Objective-1 E2E;
- guided-v2, Combined HUD, PARITY-07, interaction, species/economy/geometry.

It did **not** produce a Tier-3 pass. Three unresolved evidence items remained at checkpoint:

1. **Reference-preview lifecycle/timing:** the same-frame harness path lost trees (1344 → 335). This is a confirmed same-frame lifecycle failure; an ordinary player-facing defect was **not established**. A frame-separated Year-20 round trip was required.
2. **Rack 1920×1080 rotation:** the receipt was input-contaminated (pointer left the intended endpoint), so a product defect was **not established**. A clean native-input rerun was required.
3. **Rendered completion:** the MFM-11 final captures were black and were taken after baseline restoration. Completion data was valid, but no usable rendered Completed state existed.

The Rack five-tab failure was classified as a **stale verification assertion** against the accepted six-tab V2 UI, not a product failure.

## 3. Provider / operational / verification / product failures

| Class | Event | Disposition |
|---|---|---|
| Provider/session | OpenCode upgrade/re-authentication; new OpenCode sessions failed to open | Operational restart requirement. Not a model-capability or implementation failure. Existing Task 1 session continued. |
| Environment/runner | Invalid `ScenarioHistoryVerification` method; blank-capture first attempts; input-contaminated pointer receipt | Runner/capture/input issues. Correct reruns passed or were required; no product defect. |
| Verification-harness | Same-frame preview entry/exit; five-tab assertion; MFM-11 capture timing | Harness/lifecycle/stale-assertion findings. No production repair prescribed; narrow evidence actions released. |
| Evidence | Black completion captures; mutating economy diagnostics; transport wait for independent review | Evidence gaps/invalid diagnostics. Clean receipts and a fresh delta review required; no capability conclusion. |
| Product | No confirmed product defect | No production repair under #231. |
| Review | `NEEDS MORE EVIDENCE` | Evidence insufficiency, not rejection and not success. |

**OBSERVED FACT:** no production edit was made; production bytes remained unchanged across the qualification lineage. **UNKNOWN:** token usage, model calls, quota and provider capacity.

## 4. Serial-time inefficiencies

- The invalid `ScenarioHistoryVerification` invocation waited about **3,692 seconds** before timeout; the correct `.Begin` rerun took about **73 seconds**.
- Verification-only head advances forced repeated compile runs and repeated Unity-lock release/re-acquire cycles. The exact counts differ between the final handoff and the continuation record, so they remain **UNKNOWN**; the churn itself is observed.
- Combined-v2 first attempt (blank captures, ~304 s) plus clean rerun (~853 s); S1-C T-06 first attempt (blank captures, ~399 s) plus clean rerun (~99 s).
- Economy/reference-preview diagnostics produced eight runs (`49b`–`49i`), including one mutating diagnostic that was invalidated; `49i` is the clean receipt.
- The independent-review MCP wait timed out at the transport layer while the child continued; the result was read from the child thread. This is an orchestration/tool wait, not slow reasoning.

## 5. Independent review

OpenAI GPT-6.1-Sol, high reasoning, read-only and cross-provider, returned **`NEEDS MORE EVIDENCE`**. It found no false green in the retained compile, v24 S1-C history, save-safety, MFM boundary or complete-loop data, and no confirmed production defect. It corrected three handoff overstatements:

- the preview failure is a same-frame lifecycle failure, not proof of ordinary player-facing loss;
- the Rack five-tab failure is a stale assertion, and the 1920 rotation receipt is input-contaminated;
- rendered completion is not demonstrated because the MFM-11 captures are black and post-restoration.

## 6. Test-volume reduction (#238)

The user-directed amendment targeted approximately **50% fewer remaining executions** by removing duplicate/overlapping reruns and inheriting valid evidence where safe. It explicitly preserved the mandatory S1-C T-05 exact-candidate rerun, save/corruption/reference/Legacy40 protection, integrated MFM judgement/commitment path, complete-loop witness, both-resolution rendered evidence and independent review. No assertion was weakened, no threshold lowered and no anchor rebaselined. The handoff's `10–15 avoided executions` is an estimate, not a gate-by-gate reconciliation; the exact remaining count is **UNKNOWN**.

#240 later clarified that it is a new **evidence-closure continuation**, not item 2 of the original 12-step intake decomposition; the “Task 2 of 12” wording must not be read as eleven remaining execution tasks.

## 7. #240 current state (not its outcome)

**OBSERVED FACT at backfill time:** #240 is OPEN/RUNNING under Claude Code / Anthropic `claude-opus-5-5`, branch `qual/scenario-one-final-tier3-v1`, worktree `/media/jer/ZX20/CCF-U1`, starting HEAD `2a232885912857a1d1c2e43d4f147df1cc0888c6`, with exactly three authorized evidence actions: frame-separated Year-20 preview round trip; clean Rack 1920×1080 rotation; genuine Year-25 Completed rendering at both resolutions. **No outcome is recorded or speculated here.** A later append is required when Browser Overall wants #240's eventual result in this ledger.

## 8. Adaptive-routing learning assessment (non-authoritative)

- **Observed metrics:** one qualification worker (OpenCode/DeepSeek), one independent reviewer (OpenAI Sol); repeated compile/lock cycles; one provider/session restart failure; one review verdict of `NEEDS MORE EVIDENCE`; no confirmed product defect; no token/quota/call telemetry.
- **Provider choice:** the fallback lane completed a large qualification matrix and produced useful retained evidence. It also produced several verification/invocation/capture inefficiencies. This run does not establish a general model-capability ranking.
- **RETROSPECTIVE INFERENCE (high confidence, n=1):** validate long-running Unity entry points before execution; the 3,692 s vs 73 s pair is the clearest avoidable serial cost.
- **RETROSPECTIVE INFERENCE (moderate, n=1):** batch verification-only edits before compile/import where safe; the churn is observed, but the exact avoidable cycle count is UNKNOWN.
- **RETROSPECTIVE INFERENCE (moderate, n=1):** prefer one discriminating diagnostic over repeated broad diagnostics; the eight-run economy sequence is the example.
- **RETROSPECTIVE INFERENCE (high, n=2):** treat blank captures as capture/evidence failures and rerun the smallest affected gate; Combined-v2 and T-06 both passed clean reruns.
- **RETROSPECTIVE INFERENCE (high, n=1):** provider/auth/session failures are not model-capability failures; the existing session continued while new sessions could not open.
- **RETROSPECTIVE INFERENCE (moderate, n=1):** reducing duplicate coverage while preserving assertions is preferable to weakening tests; the #238 amendment is the evidence.
- **RETROSPECTIVE INFERENCE (high, n=1):** independent cross-provider delta review corrected evidence overstatement cheaply and narrowed the work to three actions without a full rerun.

These are hypotheses, not active policy. No token, quota, call-count or capacity values are inferred.

## 9. Exact next state

- #231 remains **BLOCKED / NEEDS MORE EVIDENCE** at its Task 1 checkpoint; no Tier-3 success token.
- #240 owns the three evidence-closure actions and is RUNNING; its outcome is pending.
- #241 is the documentation backfill only and does not alter any verdict.
- No Windows build. #194 remains NO ACTION.
