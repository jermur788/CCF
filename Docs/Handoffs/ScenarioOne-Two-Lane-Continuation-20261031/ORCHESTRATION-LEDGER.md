# PLAYER-TEST-DEEP-ORCH-01 — Orchestration Evidence Ledger — 2026-10-31

Append-oriented evidence ledger for claim **#201**, Scenario One Long-Running Deep Orchestration 01.
Evidence classes used exactly as required: **OBSERVED FACT**, **ORCHESTRATOR CLAIM**, **RETROSPECTIVE INFERENCE**.
A claim or inference is never promoted to observed fact.

## Run identity

| Field | Value |
|---|---|
| Claim | #201 (parent #91) |
| Counterpart lane | #200 browser/GitHub Manager |
| Canonical handoff | `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/DEEP-ORCHESTRATION-HANDOFF.md` at `b9e6b57133693cc9033e991c9c1724b7c0937b89` |
| Handoff context snapshot | `d05b27ad376ce02748ee4703429ad3ff66a65542` |
| Main at split | `8bd03bca142bbea6d379ad5f0a506f3c0922d2ec` (browser handoff published after it) |
| Published Scenario One integration at split | `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` |
| Fresh live `origin/main` observed at run start | `2926171e` ("coordination: mark browser Manager lane 03 running") — OBSERVED FACT 2026-10-31T09:12Z |
| Orchestration evidence worktree | `/media/jer/ZX20/CCF-DeepOrch`, branch `orchestration/scenario-one-deep-orchestration-01`, created from `origin/main` at `2926171e` |
| Orchestrator | OpenCode `opencode-go/deepseek-v4.1-flash`, role orchestrator/default worker |

## Source/reference material used

| Source | SHA-256 | Role |
|---|---|---|
| `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md` (`/home/jer/Downloads/orch/`) | `7f033e0894142f4ace7d8863b91a3022a3ff09e21e765e3e786a776852c4fd8a` (verified matches handoff) | Current candidate evidence-capture contract |
| `ORCHESTRATION-LEDGER-20261030.md` (`/home/jer/Downloads/orch/`) | `d568a2969bf38a707f74e28396c94602d34226d0307b2b9b118a14ed8d00f16e` (verified matches handoff) | Historical observed evidence (2026-10-30 overnight), NOT current policy |
| `T3-Fork-Orchestration-Evidence-Bootstrap-20261030.zip` (`/home/jer/Downloads/`) | `bb2590845f6add84bd5cb3382211df3a7e01b91c42078dc9abe3d3c5cdd3ecb5` (verified matches handoff) | Storage/provenance bootstrap and historical references |

Pinned historical overnight continuation (`4112718` / `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-CONTINUATION-20261030.md`) is reference only and does not override the current user routing instruction.

## Starting policy (as directed)

- **User objective (ORCHESTRATOR CLAIM, derived from handoff):** progress Scenario One to an exact integrated Tier-3-qualified candidate ready for the user's personal playtest, while capturing adaptive-routing evidence.
- **Two-lane split:** #200 owns browser/GitHub Manager reviews/releases/packet/active-tasks writes; #201 owns non-browser local/Unity execution orchestration.
- **Quality/cost preference:** least-cost capable worker; DeepSeek default; Claude Sonnet 5.5 Medium for harder work; Opus 5.5 Low (then Medium only if justified) after a classified capability/high-consequence need.
- **Hard constraints:** one writer per subsystem/file set; one machine-wide serialized Unity lane; no test weakening; no unauthorised product/save/reference/schema changes; no Windows build before the user personally playtests the exact candidate and explicitly authorises a build.
- **Review independence:** the independent reviewer must be separate from the implementing thread; a helper child of the implementer is not independent review.
- **Current integration / first task:** published integration `ce673714`; #196 MFM-08 MapPlanning contract reconciliation on exact `e5fb4246` is the first critical local action unless #200/live coordination supersedes it.
- **Explicitly out of scope:** #194 research-source consolidation audit; do not act on it.

## Provider / pool ledger — initial state

OBSERVED FACT (T3 capability catalogue, 2026-10-31T09:19Z): the following child-capable provider instances were advertised.

| Provider/pool | Model | State | Capacity estimate | Reset/window | Source/confidence | Current work | Context state |
|---|---|---|---|---|---|---|---|
| `opencode` | `opencode-go/deepseek-v4.1-flash` | AVAILABLE | UNKNOWN | UNKNOWN | T3 catalogue / observed successful use | #201 orchestrator, #196 worker | UNKNOWN |
| `claudeAgent` (Claude A) | `claude-sonnet-5-5` (effort medium) | AVAILABLE | UNKNOWN | UNKNOWN | T3 catalogue | none yet | UNKNOWN |
| `claudeAgent` (Claude A) | `claude-opus-5-5` (effort low) | AVAILABLE | UNKNOWN | UNKNOWN | T3 catalogue | none yet | UNKNOWN |
| `claudeAgent_claude_b_pro` (Claude B Pro) | same Claude models | AVAILABLE | UNKNOWN | UNKNOWN | T3 catalogue | none yet | UNKNOWN |
| `codex` / `codex_...OpenAI B` | GPT models | AVAILABLE | UNKNOWN | UNKNOWN | T3 catalogue | none — NOT in this run's default ladder unless #200/user reintroduces | UNKNOWN |
| `cursor`, `grok`, `antigravity`, `muse`, `pi` | — | LOGIN-UNAVAILABLE / disabled | UNKNOWN | UNKNOWN | T3 catalogue constraint text | none | UNKNOWN |

No subscription state is inferred from absence of an error. Capacity-to-completion for every dispatch below is `unknown` unless later evidence changes it.

## Event log

### 2026-10-31T09:12Z — Fresh live state read (OBSERVED FACT)

- `git fetch origin` advanced `origin/main` `20bc2ff6` → `2926171e`; live `active-tasks.md` still names **#196 READY** as the first MFM Unity priority. No #200 supersession observed at read time.
- #196 GitHub issue state OPEN, title `[READY] MFM-08 MapPlanning Contract Reconciliation 01 — additive Treatment Review tab`, no comments at read time.
- #201 issue OPEN, body matches the two-lane split and routing policy.
- Live authoritative lock tool `/media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json` → `{"status":"free","metadata":null,"unity_editor_pids":[]}` (OBSERVED FACT 2026-10-31T09:20Z).
- `/media/jer/ZX20` mounted `/dev/sdd2`, exFAT, UUID `18D6-896E`, 20 GiB free (96% used) — above the 15 GiB threshold but **marginal** for Unity work (OBSERVED FACT).
- Unity 6000.6.0f1 at `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`; Xephyr/openbox/xdotool present; two licence-copy candidates present (OBSERVED FACT).
- U1 `/media/jer/ZX20/CCF-U1` was clean, detached at exact `e5fb424635f16182748efcb0539e8a49539495a9` (OBSERVED FACT).
- `origin/verify/mfm-08-mapplanning-contract-reconcile-v1` exists at exactly `e5fb4246` (no commits yet) (OBSERVED FACT).

### 2026-10-31T09:21Z — ROUTING EVENT R-001: #196 dispatch to DeepSeek default worker

- **Work unit:** MFM-08 MapPlanning Contract Reconciliation 01, claim #196, parent #201/#91.
- **Evidence class:** ORCHESTRATOR CLAIM for the routing decision; task facts OBSERVED.
- **Task type:** bounded verification-contract edit + serialized Unity gate run.
- **Difficulty:** 3/5 — small diff but verification-weakening risk and Unity execution; one stale exact-five-tab expectation against an authorised additive `Treatment review` layer.
- **Risk class:** UI-runtime verification contract (no production/save/determinism/reference change).
- **Decomposition topology:** single child, no parallel dependency; blocks #156 MFM-08 approval.
- **Previous provider/model/role:** none (task READY).
- **Selected provider/model/reasoning/role:** OpenCode `opencode-go/deepseek-v4.1-flash`, orchestrator also acting as default bounded worker.
- **Alternatives considered:** Claude Sonnet 5.5 Medium (packet's preferred provider at packet time; stronger Unity/semantic fit) — deferred because the edit is a small, state-aware assertion change with a clear contract and the user-directed ladder makes DeepSeek first at difficulty 3 unless architecture-sensitive.
- **Selection reason/rule:** DeepSeek-first ladder for difficulty 3; task has local/Unity capability; no architecture sensitivity. Escalate to Sonnet only on a classified capability failure, not on an ordinary gate/environment failure.
- **Capacity/context evidence:** provider AVAILABLE via T3 catalogue; capacity estimate UNKNOWN; no live quota evidence.
- **Capacity-to-completion:** unknown (although local disk is marginal at 20 GiB free; an ENVIRONMENT risk, not a model risk).
- **Expected switching/interruption cost:** low — no other Unity worker; no handoff boundary.
- **EXPLOIT/EXPLORE:** EXPLOIT (default ladder, known task shape).
- **Prior outcome influencing choice:** historical 2026-10-30 ledger shows DeepSeek handling bounded verification and salvaging interrupted work; not treated as current policy.
- **Branch/worktree/lane:** `verify/mfm-08-mapplanning-contract-reconcile-v1` in `/media/jer/ZX20/CCF-U1`; starting HEAD `e5fb4246`.
- **Unity requirement:** yes, serialized machine-wide lock; currently FREE.
- **Next action:** check out branch in U1, confirm clean, acquire lock normally, patch only the verification harness, run complete `RackV2MapPlanning` gate at 1280x720 and 1920x1080.
- **Active-tasks `[RUNNING]` marker:** #200 is the sole writer of `active-tasks.md`; #201 will post execution state on claim #196 and in this ledger and leave the coordination-file write to #200.

### 2026-10-31T09:23Z — R-001 OUTCOME: #196 gate PASS (OBSERVED FACT)

- Verification patch committed `eef1bdf9`; handoff `eb4be600`; branch pushed to origin.
- Unity exit 0; `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`; 0 `error CS`; 8 captures; inspected 1280x720 capture shows the five original tabs plus additive `Treatment review`.
- Lock `e30d88891af84a539c4eab6e804a37c0` released normally; live status `free`; lane clean; Editor side effects (`.vscode/settings.json` + three Woodland_Forbs materials) restored byte-exactly.
- Handoff posted to claim #196; state returned to #200 for verdict. **Manager acceptance: UNRESOLVED at this timestamp.**
- Wall time: route start 09:21Z → gate done 09:23Z (Unity 102 s); total ~7 min including inspection/restoration/handoff.
- Failure class: none. No retries, no salvage, no provider switch.
- Resource observations: tokens/calls not exposed by the runtime → UNKNOWN. Disk 20 GiB free at start, 96% used → marginal but sufficient.

### 2026-10-31T09:27Z — LEARNING CHECKPOINT 1 (RETROSPECTIVE INFERENCE)

- **Observed:** DeepSeek first-pass technical success on one difficulty-3 verification-contract edit plus serialized Unity gate; no escalation; one gate run only.
- **Hypothesis (moderate confidence, n=1):** DeepSeek is sufficient for bounded, well-specified verification-contract repairs with a clear authority and a single Unity harness, including Unity execution and cleanup.
- **Confidence limits:** one task shape; task had unusually explicit Manager authority and a single failing assertion. Does not support a claim about cross-file UI repair or save/determinism work.
- **Cheaper-route question:** none cheaper than the default worker was available; the alternative (Sonnet) would have cost more for no observed gain here.
- **Policy proposal (NOT active):** keep difficulty-3 verification-plus-Unity on DeepSeek-first; escalate on a classified capability failure or when the harness semantics are ambiguous.

### 2026-10-31T09:27Z — ROUTING EVENT R-002: #198 to DeepSeek default worker

- **Work unit:** Guided-v2 v24 / D-066 Contract Reconciliation 01, claim #198, parent #201/#91.
- **Task type:** verification-only contract reconciliation across guided/SequentialObjectives harnesses, then serialized Unity (guided-v2 + companion model-0/legacy coverage).
- **Difficulty:** 3/5 — clear authority (D-066 spec/plan) but two harness files, real-input/native coverage and save/load semantics.
- **Risk class:** teaching/save-load verification contract; no production/schema/anchor change.
- **Selected:** OpenCode `opencode-go/deepseek-v4.1-flash`.
- **Alternatives:** Sonnet 5.5 Medium (packet's preferred provider at packet time) — deferred; the authority is explicit and the diff is bounded. Escalate only on a classified capability failure.
- **Capacity/context evidence:** provider AVAILABLE (T3 catalogue); capacity UNKNOWN. Disk free 20 GiB marginal; Unity lane serialized.
- **Capacity-to-completion:** unknown.
- **Switching cost:** low; #196 released the lane normally.
- **EXPLOIT/EXPLORE:** EXPLOIT.
- **Prior outcome:** R-001 success on a comparable bounded verification task.
- **Branch/lane:** `verify/guided-v2-v24-contract-reconcile-v1` in `/media/jer/ZX20/CCF-U1`; exact start `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.
- **Next action:** checkout exact integration, reconcile only the stale v23 / pre-latch reload expectations to D-066, then fresh-preflight/acquire Unity and run `guided-v2` + `guided-legacy`.

### 2026-10-31T10:05Z — R-002 OUTCOME: #198 gates PASS after one real-input retry (OBSERVED FACT)

- Verification commits `bac20456`, `1b8127a2`; handoff `59b278f7`; branch pushed. Production unchanged at `ce673714`.
- `guided-v2`: 535 PASS / 0 FAIL / 0 `error CS`, 48 captures, `GUIDED_ONBOARDING_PASS` (327 s).
- `guided-legacy`: 569 PASS / 0 FAIL / 0 `error CS`, 62 captures, `GUIDED_ONBOARDING_PASS` (321 s, second attempt).
- First `guided-v2` attempt found one genuinely stale expectation (`V2 confirmed rack lesson clearly requests annual advance`); the harness had confirmed the plan but skipped the accepted RACK-V2-09 entrance-marking step. Fixed verification-only.
- First `guided-legacy` attempt failed in the real-input driver before confirmation (`State wait failed`; swivel never observed: selections=1, reselections=0, cancellations=0, confirmed=0). Classified **ENVIRONMENT/RUNNER**, not capability. Retried alone and passed at the same tree.
- Locks `4953262b6e1942538e34e6c0029dc78a` and `41efe1bde2ac4d0c852e33f756c7528b` released normally; lane restored clean; Editor side effects reverted; no Editor/Xephyr.
- Handoff posted to #198. **Manager acceptance: UNRESOLVED at this timestamp.**

### 2026-10-31T10:05Z — LEARNING CHECKPOINT 2 (RETROSPECTIVE INFERENCE)

- **Observed:** DeepSeek completed a second verification-only reconciliation, including finding a third stale expectation that the packet's known list did not name. No model failure; no escalation.
- **Observed:** one failure was a known-flaky real-input driver, not a contract or production defect; one clean retry passed. The classification rule (environment ≠ capability) prevented a needless Sonnet/Opus dispatch.
- **Hypothesis (moderate confidence, n=2):** DeepSeek is sufficient for bounded verification-contract reconciliation when authority is explicit, even when the gate surfaces additional stale assertions. It is not evidence for cross-file production UI repair.
- **Waste observation:** the failed 901 s legacy run plus 321 s retry cost ~15 min; retrying the real-input gate alone earlier would have avoided only the first 901 s, not the contract work. No cheaper route identified beyond the existing "run the guided real-input gate first/alone" workflow note.
- **Policy proposal (NOT active):** for guided real-input gates, treat a pre-confirmation driver state-wait failure as ENVIRONMENT and retry once alone before any model/provider change.

### 2026-10-31T10:05Z — ROUTING EVENT R-003: #199 to Claude Sonnet 5.5 Medium

- **Work unit:** Scenario One Combined HUD / Focus Regression Repair 01, claim #199, parent #201/#91.
- **Task type:** bounded production UI/focus repair across HUD composition, minimap/estimate, competitor presentation and focus ownership, in both Rack V2 and legacy modes; independent review required.
- **Difficulty:** 4/5 — cross-screen UI/focus root cause with protected assertions; not architecture-wide but materially harder than #196/#198.
- **Risk class:** UI-runtime production repair; no save/determinism/reference.
- **Selected:** `claudeAgent` / `claude-sonnet-5-5`, effort `medium`.
- **Alternatives considered:** DeepSeek (default) — rejected because this is difficulty 4 cross-screen production UI, where the user-directed ladder names Sonnet 5.5 Medium; Opus 5.5 Low — held in reserve until a classified Sonnet capability failure or high-consequence finding.
- **Selection reason/rule:** difficulty-4 production UI/focus repair per the current ladder.
- **Capacity/context evidence:** Claude A advertised AVAILABLE by T3 catalogue; capacity UNKNOWN; no live quota evidence.
- **Capacity-to-completion:** unknown.
- **Expected switching/interruption cost:** moderate — handoff boundary between implementer and Unity executor; mitigated by a dedicated worktree and a read-only-to-Unity instruction until implementation is committed.
- **EXPLOIT/EXPLORE:** EXPLOIT (ladder-named worker).
- **Prior outcome influencing choice:** #196/#198 DeepSeek successes establish the cheaper default for bounded verification, not for cross-screen UI repair.
- **Branch/worktree:** `fix/scenario-one-combined-hud-focus-regression-v1` in `/media/jer/ZX20/CCF-CombinedHud`, exact start `ce673714`.
- **Unity:** none during implementation; #201 retains the serialized Unity lane and will run the combined gates after the child commits.
- **Next action:** dispatch Sonnet with the packet, exact worktree, no-Unity/one-writer constraints, and require a committed diff + HANDOFF draft + root-cause evidence.

### 2026-10-31T10:08Z — R-003 OUTCOME: #199 implemented partially by Sonnet; gates not green (OBSERVED FACT)

- Delegation: `claudeAgent` / `claude-sonnet-5-5`, effort `medium`, child thread created 10:06Z; completed 10:08Z (run wall time ~2.5 min; MCP `delegate_task` wait call itself returned a transport timeout after ~2 min, so the parent recovered via `t3_thread_list` / `t3_thread_read` and the child thread ID).
- Child output: commit `07fdddf8` — verification fixture only (`CombinedHudComposition.cs` now excludes Rack V2-owned trees from the competitor fixture) + `HANDOFF.md`. No production `Assets/` change.
- Child finding: estimate/minimap and `FOCUS_04/06/07` trace to PARITY-07's deliberate `ShowMinimap(!(inspecting && ...))` step-aside and a 600-wide estimate panel; the child classified this as a contract conflict requiring Manager disposition, not a code bug it could safely repair. It stopped rather than weaken the assertion. This is correct scope discipline, not a capability failure.
- #201 Unity execution on the same commit: `combined-v2` 453/10/24; `combined-legacy` 440/8/24. The fixture fix removed the false 1920 competitor panel/tag failures and exposed a real 1920 `COMPETITOR_LIST_IS_NOT_CLIPPED` defect (915 vs 896). Estimate/minimap + FOCUS remain.
- Environment: two legacy attempts hung before import/compile (no `CHC_` lines) after the v2 Editor was reaped; clearing only `Temp/*` and stale `Library/*DB-lock` files (no Editor) restored startup. Classified ENVIRONMENT/runner, not code, not capability.
- Manager disposition requested on #199 with four options; Manager acceptance UNRESOLVED. #196/#198 also await Manager verdict. No further production work is authorised in this state.

### 2026-10-31T11:38Z — LEARNING CHECKPOINT 3 (RETROSPECTIVE INFERENCE)

- **Observed:** Sonnet 5.5 Medium produced a precise root-cause and a safe partial fix in ~2.5 min, and correctly refused to resolve a protected-contract conflict unilaterally. First-pass technical success on the part it could do; blocked on authority, not capability.
- **Observed:** the delegated child's completion notification did not reach the parent because the MCP wait returned a transport timeout; polling the child thread recovered the result. Orchestration overhead ~2 min.
- **Hypothesis (moderate confidence, n=1):** Sonnet 5.5 Medium is a good fit for difficulty-4 UI root-cause work where the deliverable is a diagnosis + safe bounded fix; it is not evidence about its ability to complete a contested product-layout repair.
- **Waste/incorrect choices:** none material. The 40-minute first legacy timeout was avoidable by checking stale Unity `Temp` state immediately after a reaped Editor; record that as a workflow check.
- **Escalation assessment:** no escalation to Opus is justified. The blocker is a product/authority decision (`PARITY-07` minimap trade-off vs protected Combined contract), which no stronger model can resolve.
- **Policy proposal (NOT active):** after any reaped Unity Editor, clear `Temp/*` and stale `Library/*DB-lock` before the next gate; treat pre-import startup hangs as environment. For delegated children, treat MCP transport timeout as unknown, not failure, and poll the child thread.

---

# CONTINUATION — Local Deep Orchestration Resume 02 (#203) — from 2026-10-31T12:26Z

Appended evidence for claim **#203** (parent #201/#91). The preceding #201 events are historical evidence and are preserved verbatim above; nothing was rewritten.

## Continuation run identity

| Field | Value |
|---|---|
| Claim | #203 (parent #201/#91) |
| Counterpart lane | #200 / bounded browser claim #202 |
| Canonical handoff | `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-DEEP-ORCHESTRATION-RESUME-20261031T1221Z.md` at `a02e5e439c109e9ceea95530e893baf2a6c3ad87` |
| Orchestration evidence branch | `orchestration/scenario-one-local-deep-orchestration-02`, created from fresh `origin/main` `e19f6e31` |
| Orchestrator | OpenCode `opencode-go/deepseek-v4.1-flash` — orchestrator + default worker (difficulty 1–3) |
| Live `origin/main` at run start | `e19f6e3123f52196d7d2f08fea094c57f9c82a54` — OBSERVED FACT 2026-10-31T12:30Z |
| Published Scenario One integration at start | `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` (re-confirmed identical to `origin/integration/scenario-one-player-test-ready-v1`) |

## Start gate (OBSERVED FACT then ORCHESTRATOR CLAIM)

- OBSERVED: the #200/#202 Manager release was published as durable artefacts on `origin/main` before any local work: `e2d9db39` (MFM-08 feature approved for integration), `d0b514e9` (Option 1 authorised for #199), `e19f6e31` (#199 Option 1 continuation packet), plus `bd712ec3` (#196 APPROVED) and `2d14a4cb` (#198 APPROVED), and OPEN claim #205 with `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- OBSERVED: at 12:30Z live `active-tasks.md` on `origin/main` had **not yet** been rewritten for the post-verdict release; #200 was still committing. It was reconciled at `dd408604` ("coordination: record #205 local integration start") while this run was in its first Unity phase, and then records #205 RUNNING under #203, #199 authorised-waiting-repin, and the S1-C/S1-D release state.
- ORCHESTRATOR CLAIM: proceeding on the durable Manager artefacts while the coordination file lagged is within the start gate, because the gate's substance is "Manager release published and child ownership can start". The lag was recorded rather than waited on indefinitely, and no coordination file were written by this lane. **RETROSPECTIVE INFERENCE:** the lag was ~10 minutes of coordination latency and cost nothing because the released task was unambiguous; a stricter reading would have idled the Unity lane.

## Provider / pool ledger — continuation state

| Provider/pool | Model | State | Capacity estimate | Reset/window | Source/confidence | Current work | Context state |
|---|---|---|---|---|---|---|---|
| `opencode` | `opencode-go/deepseek-v4.1-flash` | AVAILABLE | UNKNOWN | UNKNOWN | observed successful use this run | #203 orchestrator + #205 integration/gates + evidence | no compaction observed; no provider-reported context telemetry → UNKNOWN |
| `claudeAgent` | `claude-sonnet-5-5` (medium) | AVAILABLE (catalogue) | UNKNOWN | UNKNOWN | prior run used it successfully | queued for #199 production repair after repin | UNKNOWN |
| `claudeAgent` | `claude-opus-5-5` | AVAILABLE (catalogue) | UNKNOWN | UNKNOWN | not used | not dispatched; no escalation trigger met | UNKNOWN |

No provider capacity/auth failure occurred. No quota/reset state is inferred from absence of an error.

## Event log (continuation)

### 2026-10-31T12:31Z — ROUTING EVENT R-004: #205 MFM-08 Integration 01 to DeepSeek default worker

- **Work unit:** MFM-08 Integration 01, claim #205, parent #156 / #201 / #203.
- **Task type:** exact-delta replay (3 commits) + post-replay Unity/rendered composition verification + save/reference compatibility checks.
- **Difficulty:** 2/5 mechanical replay / 3/5 verification (packet classifies Medium) — clean browser overlap, no conflict resolution expected, but Unity composition against 29 later commits must be observed.
- **Risk class:** integration replay; protected lineage preservation; no new product behaviour.
- **Decomposition topology:** single child, serialized; unblocks MFM-09 and the #199 repin.
- **Previous provider/model/role:** none for this unit (fresh claim #205).
- **Selected provider/model/reasoning/role:** OpenCode `opencode-go/deepseek-v4.1-flash`, orchestrator acting as bounded integration/gate worker.
- **Alternatives considered:** Claude Sonnet 5.5 Medium — deferred; the packet explicitly reserves Sonnet for genuine conflict resolution or architecture-sensitive composition, and neither occurred. Opus — not considered (bounded mechanical work).
- **Selection reason/rule:** Tier A default worker; recipient of the user-directed ladder for mechanical/verification work.
- **Capacity/context evidence:** provider AVAILABLE by observed successful use; capacity UNKNOWN; disk 18 GiB free (97 % used) = marginal; Unity lock FREE at preflight.
- **Capacity-to-completion:** unknown.
- **Expected switching/interruption cost:** low (no competing writer; U1 lane free).
- **EXPLOIT/EXPLORE:** EXPLOIT.
- **Prior outcome influencing choice:** R-001/R-002 DeepSeek first-pass successes on bounded verification + Unity; #201 learning checkpoint 1–2.
- **Branch/worktree/lane:** `integration/mfm-08-onto-scenario-one-v1` in `/media/jer/ZX20/CCF-U1`; starting HEAD `ce673714`.
- **Unity requirement:** yes, machine-wide serialized; lock acquired `f005c4cf…`.
- **Active-tasks write:** none — #200 owns the file; `[RUNNING]` posted on claim #205 only.

### 2026-10-31T12:33Z — R-004 INTERMEDIATE: replay clean and provably equivalent (OBSERVED FACT)

- `git diff --name-only cfdc93bf..ce673714` ∩ `git diff --name-only cfdc93bf..e5fb4246` = **empty** — the later 29-commit lineage touches no path the MFM-08 delta touches. This independently confirms the browser overlap review.
- Cherry-pick of the 3 approved commits onto `ce673714`: **no conflicts**, tip `34e360f8`.
- Equivalence: `git diff ce673714..34e360f8` and `git diff cfdc93bf..e5fb4246` are the **same byte stream** (SHA-256 `9715913c…`), i.e. the replay is content-identical to the approved delta.
- #196 `eef1bdf9` and #199 `42742a8a` not absorbed; no save/schema/reference path in the replay diff.

### 2026-10-31T12:40Z — R-004 FAILURE then CLASSIFICATION: stale save-version pin in the #156 gate (OBSERVED FACT → ORCHESTRATOR CLAIM)

- First post-integration Unity attempt at `d99d2892` (production candidate + qualification harness): `MFM08_UNITY_FAIL checks=3 … System.InvalidOperationException: unchanged save version 23`.
- Root cause: `Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs` pinned `ForestSaveData.CurrentVersion == 23`; the accepted current lineage is **v24** (D-066 first-guided-review latch), Manager-accepted in #198 (`2d14a4cb`).
- Classification: **stale verification contract**, not a production regression, not a capability failure, not an environment failure. Same class as the #198 guided-v2 v23→v24 reconciliation.
- Action: verification-only 2-line reconciliation in the throwaway qual branch (`qual/mfm-08-post-integration-v1`), production branch untouched; retried once.
- **This is the failure a wrong router fires a stronger model for.** It was a repository-contract fact, so escalation would have produced no additional capability. Recorded as a routing-policy data point, not a model comparison.

### 2026-10-31T12:52Z — R-004 OUTCOME: all post-integration gates PASS (OBSERVED FACT)

- Integrated production candidate `34e360f8` (docs/handoff tip `3920ef14`); qual harness `c9336186`.
- Unity compile: `Assembly-CSharp` / `Assembly-CSharp-Editor` built, **0** `error CS`, Editor exit 0 for both gate runs.
- Focused MFM-08: **`MFM08_UNITY_PASS checks=373 shots=25`** (290 s), 1280×720 + 1920×1080; includes in-memory save/world-hash round trip and "no save file written".
- Authoritative RackV2MapPlanning under the approved #196 contract: **`RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`** (90 s); capture inspected — five original tabs + additive `Treatment review`, `Plan Racks` still a sibling control.
- Save/reference compatibility: **`PARITY08_SPECIES_OFFLINE_PASS checks=23`** incl. `teaching latch does not move WorldHash`.
- Offline MFM-08 logic gate: **`MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0`**, repeat byte-identical. Its `MFM08_BOUNDARY_FAIL` is expected on an integration lineage (boundary pinned to `cfdc93bf`) and is **not** treated as a defect; the real scope check is the byte-equivalence proof.
- Locks `f005c4cf…` and `fe3c16f3…` released normally; editor side effects restored byte-exactly; lane clean; no Editor/Xephyr; live lock `free`.
- Handoff published `Docs/Handoffs/MFM-08-Integration-01/HANDOFF.md`; claim #205 returned with `MFM-08 INTEGRATED ON CURRENT SCENARIO ONE LINEAGE — READY FOR MANAGER REVIEW`. **Manager acceptance: UNRESOLVED at this timestamp.**

### 2026-10-31T12:55Z — LEARNING CHECKPOINT 4 (RETROSPECTIVE INFERENCE)

- **Observed:** DeepSeek completed a clean exact-delta replay with a machine-checked equivalence proof and two rendered Unity gates, with no escalation.
- **Observed:** the only failure in this unit was a stale `v23` pin in a gate written for the pre-v24 lineage. It cost one 114 s Unity run and one re-acquire; classification kept it away from model escalation.
- **Hypothesis (moderate confidence, n=1 for integration):** DeepSeek is sufficient for exact-delta integration replays whose overlap is provably empty, including post-replay Unity composition, when the packet supplies an equivalence criterion.
- **Waste observation:** the first Unity attempt could have been avoided by grepping the qualification gate for schema pins before acquiring the lock. Cheap, and now a workflow note.
- **Cheaper-route question:** none; DeepSeek was the cheapest capable worker and succeeded.
- **Capacity/context effects:** none observed; no compaction, no provider interruption.
- **Missing telemetry:** token/model-call counts and live quota remain UNKNOWN.
- **Policy proposal (NOT active):** before running a recycled qualification harness on a newer lineage, diff its schema/version pins against the accepted schema constants (`ForestSaveData.CurrentVersion`, save/anchor tokens) and reconcile stale pins as verification-only work.

### 2026-10-31T13:00Z — BOUNDARY REACHED: Manager verdict + repin required (ORCHESTRATOR CLAIM)

- #205 is returned and awaits `APPROVED / NEEDS CHANGES / REJECTED`.
- Per `MANAGER-DISPOSITION-OPTION1-20261031.md`, #199 production repair **must not** start from the stale `ce673714` branch while #205 is active; #200 must repin it to the exact combined SHA after accepting #205.
- S1-D T-08 was released on `ce673714`; live `active-tasks.md` now states #200 must determine whether it needs a fresh repin after the lineage changes. That is a Manager decision, so this lane did not start it.
- S1-C T-08 remains dependency-cleared but **not released**. Not started.
- No new production writer was opened while #205 owned the UI integration surface. One-writer/one-Unity discipline held throughout.

### 2026-10-31T13:02Z — BOUNDARY CONFIRMED: #200 not auto-reviewing #205 (OBSERVED FACT)

- OBSERVED: bounded polling 12:47Z–13:02Z; `origin/main` stayed at `dd408604`; #205 stayed OPEN with no Manager review comment; #202 was closed `[COMPLETE]` at 12:33Z and its post-close note states "browser umbrella #200 now waits to review the #205 handoff".
- OBSERVED: `#205`, `#199` and `S1-D T-08` are all Manager-gated at this timestamp; no locally released production or qualification work remains that this lane may start without an ownership/repin decision.
- ORCHESTRATOR CLAIM: the correct action is to pause and return the exact next action to the user, not to self-accept the integration or open a competing UI writer. No Manager-owned file was written; no verdict was self-issued.
- **Note for the receiving chat:** the programme resumes with one browser-lane turn (review #205 → repin #199 → decide S1-D T-08 repin). Everything needed is durable on origin.
