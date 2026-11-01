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

---

# CONTINUATION — Local Deep Orchestration Resume 02 (#203), post-#205 resume — from 2026-10-31T18:32Z

Appended evidence for claim **#203** (parent #201/#91). Prior sections are preserved verbatim.

## Resume start gate (OBSERVED FACT)

- Fresh `git fetch origin` advanced `origin/main` to **`c76b2da8c48d0ff9289d95c0ba17552eb8459540`**.
- Manager review accepted #205: `Docs/Handoffs/MFM-08-Integration-01/MANAGER-REVIEW-20261031.md`; #156/#205 are **APPROVED / INTEGRATED / CLOSED**.
- Published Scenario One integration is now **`34e360f85835afd15f5323f287372e0f2ee83dd9`** (`origin/integration/scenario-one-player-test-ready-v1`), a fast-forward from `ce673714`.
- #206 S1-C T-08 is **RELEASED / READY** on exact `34e360f8`; binding amendment `Docs/Handoffs/S1-C-Implementation-08/RELEASE-REPIN-20261031.md`.
- #207 MFM-09 and #199 Option 1 are **REPINNED / READY / QUEUED** on `34e360f8` but must not displace #206's preferred Unity slot. S1-D T-08 is repinned to `34e360f8` but **waits for #206 Manager acceptance**.
- Live `active-tasks.md` grants #206 the next serialized Unity action. #200 remains sole writer of that file; this lane did not edit it.
- Fresh preflight: `/media/jer/ZX20/CCF-U1` clean; created `qual/s1-c-review-history-v1` at exact `34e360f8`; authoritative lock `free`; no Editor/Xephyr; ZX20 18 GiB free (above the 15 GiB floor).

## 2026-10-31T18:32Z — ROUTING EVENT R-005: #206 S1-C T-08 to Claude Sonnet 5.5 Medium

- **Work unit:** S1-C T-08 Review / Management History feature qualification, claim #206, parent #91 / #201 / #203.
- **Task type:** integrated Tier-2 qualification — multi-year history, year browsing, drill-down, grouped-harvest honesty, no-work year, provenance, guidance, objective history, current-schema save/load, genuine v20-y5 old-save, read-only/nonmutation, protected Reference/Legacy, rendered at 1280×720 and 1920×1080.
- **Difficulty:** **High (4/5)** — cross-cutting save/history/UI/rendered qualification with genuine-old-save and grouped-money honesty; packet names SOL/CODEX high-reasoning or Claude Sonnet-class as preferred. This is materially harder than the bounded verification/contract work DeepSeek completed in R-001/R-002.
- **Risk class:** history/save/UI qualification; no production repair authority; stale verification pins are expected across the v22→v24 lineage.
- **Decomposition topology:** single child; serialized Unity; no parallel worker.
- **Selected provider/model/role:** `claudeAgent` / `claude-sonnet-5-5`, effort `medium`, role test/qualification. DeepSeek remains orchestrator and ledger owner, not the gate worker.
- **Alternatives considered:** DeepSeek default worker — declined because the task requires sustained cross-file qualification judgment, stale-contract reconciliation and rendered/old-save synthesis beyond the bounded R-001/R-002 class; Opus 5.5 — held as final capability escalation only, with no classified Sonnet failure yet.
- **Selection reason/rule:** user-directed ladder for high-difficulty integrated qualification; packet's preferred Sonnet-class lane.
- **Capacity/context evidence:** provider advertised AVAILABLE in the T3 catalogue; capacity UNKNOWN; no live quota evidence.
- **Capacity-to-completion:** unknown; Unity runtime and disk are the expected constraints, not provider capacity.
- **Expected switching/interruption cost:** moderate — one child owns the full qualification; parent retains lock/lane/coordination supervision.
- **EXPLOIT/EXPLORE:** EXPLOIT (named worker class for this task).
- **Prior outcome influencing choice:** R-001/R-002 DeepSeek successes establish the cheaper default for bounded verification; R-003 Sonnet root-cause success supports Sonnet for harder integrated UI/save work.
- **Branch/worktree/lane:** `qual/s1-c-review-history-v1` in `/media/jer/ZX20/CCF-U1`, exact start `34e360f8`.
- **Unity:** machine-wide serialized lock; worker must acquire after fresh preflight and record the receipt.
- **Active-tasks write:** none — #200 owns the file; worker may post `[RUNNING]` on claim #206 only after lock acquisition.
- **Next action:** worker executes the full matrix, reconciles verification-only stale pins, publishes `Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md`, pushes the qual branch and returns the exact token or blocker.
- **Queue discipline:** #199 and #207 remain queued; S1-D T-08 does not start until #206 returns and receives Manager approval; no Windows build.

### 2026-10-31T19:30Z — R-005 OUTCOME: #206 local qualification returned (OBSERVED FACT)

- Worker `claudeAgent` / `claude-sonnet-5-5` (medium) completed as the only local worker; DeepSeek remained orchestrator.
- Exact production candidate **`34e360f85835afd15f5323f287372e0f2ee83dd9`**; production bytes unchanged. Verification/docs branch `qual/s1-c-review-history-v1`; final commit **`f84214764afacbc77ffb329cfb0f7ba7d6815b24`**, pushed to origin.
- Local result token returned: **`S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`**. This is a local Tier-2 result only; **Manager verdict remains pending**. No self-approval was issued.
- Evidence root `/media/jer/Files/CCF/Verification/S1C-T08-20261031/`; handoff `Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md`.
- Gate results reported and receipts inspected: T-07 offline `304/304`, mutations `8/8`; compile; objective `544` checks/34 cases; render 6 captures; T-02 `246`; T-03 `279`; T-04 `529`; T-06 `170`; Reference Future, SaveHardening, Progress, baseline-hash `30/30`, interaction, density-save, completion all PASS. 1280×720 + 1920×1080 captures present; 10 directly inspected.
- Schema/version reconciliation: five T-05 assertions moved from v22 to the exact current **v24** with the literal/version assertion preserved. No production or assertion weakening.
- Deviation recorded by the worker: gate 11's original live pre-v22 derivation went stale because `PreV22SaveTypes` shares current work-order/event types carrying MFM-06 fields. The candidate hashes were byte-identical to the accepted T-05 values; the worker pinned the gate to the accepted 30-row pre-v22 table (SHA-256 `81c6f13ede7c746f592c9e1776ed2da11382e46e184c6d790caabc9659daf2cd`) and requires exact equality. This is a verification-only change, disclosed for Manager review; it must not be treated as a silent anchor rebaseline.
- Head hygiene: gates 02–10 and 12–14 ran at `f3470edb`; compile and gate 11 re-ran at `e0f0437c`; the only files differing between those heads are `S1CHashBaselineGate.cs` and one runner env pass-through. The final handoff is at `f8421476`. The handoff discloses this rather than claiming a single-head full chain.
- Locks `802d7312…`, `f098ea74…`, `8440d058…` were acquired/released normally under owner `Claude-S1C-T08`, task `S1-C-T08`, lane U1; U1 clean at final head; no Editor/Xephyr/openbox; live lock `free`; host save untouched; no Windows/Player build.
- Worker posted `[RUNNING]` and `[RESULT]` on #206. #200 remains the sole active-tasks writer; this lane did not edit it.

### 2026-10-31T19:30Z — LEARNING CHECKPOINT 5 (RETROSPECTIVE INFERENCE)

- **Observed:** Sonnet 5.5 Medium carried a high-difficulty integrated qualification through 14 gates, two verification-pin classes, one runner bug and one stale protected-gate derivation, returning a complete matrix and honest deviations.
- **Observed:** the runner bug (inherited `CCF_TEST_DIRECT_ENTRY`) and the version-pin drift were classified as verification/tooling, not model or production failures; the worker fixed and re-ran rather than weakening assertions.
- **Observed:** the protected baseline-hash reconciliation is the material review risk. It is disclosed, exact-equality based and tied to the accepted T-05 table, but it changes a protected gate's reference source; Manager review should treat it as a verification-contract decision, not as proof that the anchor moved.
- **Hypothesis (moderate confidence, n=1):** Sonnet 5.5 Medium is sufficient for a bounded high-difficulty integrated history/save/rendered qualification when the packet supplies accepted prior gates and a fail-closed runner contract.
- **Cheaper-route question:** DeepSeek was not used for the gate work; the task was explicitly high-difficulty and the user/packet named Sonnet-class. No cheaper capable route was identified for the rendered/old-save synthesis.
- **Missing telemetry:** token/model-call counts and live quota remain UNKNOWN.
- **Policy proposal (NOT active):** for reused protected gates on a newer lineage, require the worker to distinguish (a) production hash change, (b) stale live derivation, and (c) accepted-table re-pin in the handoff; the Manager should explicitly accept (c) before the token is treated as qualified.

### 2026-10-31T19:30Z — QUEUE STATE AFTER #206 RETURN (ORCHESTRATOR CLAIM)

- **S1-D T-08 does not start.** It remains repinned to `34e360f8` and waits for #206 Manager acceptance.
- **#199 Combined HUD / Focus Option 1** remains repinned/ready/queued on `34e360f8`; no production start was taken.
- **#207 MFM-09** remains repinned/ready/queued on `34e360f8`; no displacement of the S1-C/S1-D critical path was made.
- #194 remains NO ACTION. No Windows build was made or authorised.

---

# CONTINUATION — Unified Local Master Handoff 2026-11-01 (#203) — from 2026-11-01T08:20Z

Appended evidence for claim **#203** under `LOCAL-MASTER-HANDOFF-20261101.md` at `6c77907e8cd4ceaec3ccf2d32a5dbaaff19eca0d`. Prior sections preserved verbatim.

## Start gate and live refresh (OBSERVED FACT)

- `git fetch origin` refreshed; `origin/main` at **`d033923c`**; published integration `origin/integration/scenario-one-player-test-ready-v1` exactly **`6784ab17cac922a16815f4898a30737482101018`**.
- Live `active-tasks.md` read: #213 MFM-10 local residual **READY**; #199 Option 1 **REPINNED / READY / QUEUED**; #212 browser handoff closed; PARITY-09/MFM-11/MFM-12 blocked; Tier-3 not released; #194 NO ACTION.
- Exact child authority read: `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md` at `a3372ce5`; `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261101.md`.
- Ownership/lane preflight: U1 clean at `af0f29a2` (no Assets diff to `6784ab17`); no Unity Editor; machine-wide lock `free`; ZX20 **17.54 GiB** free (above 15.00 GiB floor); Files **57.1 GiB** free.
- No `active-tasks.md` edit. No #194 action.

## Provider/capacity ledger — initialised at run start

| Provider/pool | State | Model/config | Remaining capacity | Reset/window | Evidence source + confidence | Current work | Capacity-to-completion | Switching cost |
|---|---|---|---|---|---|---|---|---|
| DeepSeek | AVAILABLE / executing | `opencode-go/deepseek-v4.1-flash`, variant `max`, build | observe/UNKNOWN | observe/UNKNOWN | runtime (OBSERVED FACT for availability) | orchestration; #213 execution | sufficient for bounded offline work (observed) | low |
| Mimo | AVAILABLE (user report) | `opencode-go/mimo-v2.6-pro`, variant `xhigh`, build | **qualitatively high per user; exact UNKNOWN** | UNKNOWN | user initially; T3 catalogue confirms model (OBSERVED FACT for existence) | #199 Option 1 production repair | assess at dispatch; user reports substantial | moderate (new child context) |
| Claude | UNKNOWN | Sonnet 5.5 Medium class | UNKNOWN | UNKNOWN | none this run | none | not needed yet | n/a |

`UNKNOWN` is retained rather than inferred. No token count, reset time or quota was invented.

## 2026-11-01T08:22Z — ROUTING EVENT R-006: #213 MFM-10 residual to DeepSeek

- **Work unit:** #213 MFM-10 local species-parity residual qualification; parent #91 / #200 / #203.
- **Evidence class:** OBSERVED FACT (task facts) + ORCHESTRATOR CLAIM (route).
- **Task type/difficulty/risk:** deterministic offline qualification; Low/Medium (2/5); no production repair authority; critical-path gate.
- **Decomposition topology:** single bounded child, no Unity; run in parallel with #199.
- **Previous provider/model:** none (new child).
- **Selected provider/model/role:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`), orchestrator/default worker.
- **Alternatives considered:** Mimo 2.6 Pro (declined: deterministic runner execution is cheaper on the orchestrator and avoids a new child context); Sonnet 5.5 Medium (declined: no reasoning need; packet names DeepSeek default).
- **Reason/rule:** existing deterministic runners; no production design; user/packet default-worker rule.
- **Capacity evidence/source/confidence:** DeepSeek AVAILABLE by runtime; capacity-to-completion SUFFICIENT (observed by completion).
- **Reset/window:** UNKNOWN.
- **Previous outcome influencing choice:** DeepSeek completed comparable offline gates in #205/#201.
- **Branch/worktree/start SHA:** `verify/mfm-10-species-guard-v1` in `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101`, exact `6784ab17...`.
- **Unity requirement/lock:** none; no lock acquired.
- **EXPLOIT/EXPLORE:** EXPLOIT.
- **Result/evidence:** Command 1 PASS `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`, repeat byte-identical. Command 2 exit 1: `THREE_SPECIES_OFFLINE_FAIL failures=1 checks=91` (scanner false positive on `MaxRemovedBasalShare`) + `COMPOSITE_DENSITY_FAIL checks=107 failures=3` (three documented pure-Beech R2 misses 0.423/0.407/0.417 vs unchanged 0.40, Manager-adjudicated accepted known C-level limitation). Isolated suites PASS. Identical rerun.
- **Handoff/commit/claim reference:** `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md` at `43e26370`; branch pushed; #213 comment `6555537043`.
- **Accepted/rejected/unresolved:** **UNRESOLVED** — `SPECIES GUARD QUALIFIED` withheld because the published gate is not exit-0. #200 disposition required (accept the two documented red families as out of MFM-10 scope, or release a bounded verification-asset repair).
- **Failure class:** pre-existing verification-asset false positive + accepted known calibration limitation; **not** a model-capability failure, not a new MFM-10 production defect.
- **Retries/rework/salvage:** one repeat run for determinism; no rework.
- **Observable resources:** two dotnet builds; wall time approx. 6–8 min total; token/call counts UNKNOWN.
- **Next action:** #200 reviews the handoff; no Unity; downstream PARITY-09/MFM-11 remain blocked.

## 2026-11-01T08:26Z — ROUTING EVENT R-007: #199 Option 1 production repair to Mimo 2.6 Pro

- **Work unit:** #199 Scenario One Combined HUD / Focus Regression Repair 01 (Option 1); parent #91 / #200 / #203.
- **Evidence class:** OBSERVED FACT (authority/lane/lock) + ORCHESTRATOR CLAIM (route).
- **Task type/difficulty/risk:** cross-screen Unity UI/focus production repair with protected assertions and two compatibility modes; Medium/High; production writer with independent-review requirement.
- **Decomposition topology:** two parallel children: DeepSeek (#213 offline) + Mimo (#199 Unity). Non-overlapping files/lanes; Unity serialized to U1 only.
- **Previous provider/model:** prior partial attempt on stale `42742a8a`; no current writer.
- **Selected provider/model/role:** Mimo 2.6 Pro (`opencode-go/mimo-v2.6-pro`, variant `xhigh`, build), delegated implementation worker.
- **Alternatives considered:** DeepSeek orchestrator as implementer (declined: would serialize the whole run and consume orchestration attention while a substantial independent child is available); Sonnet 5.5 Medium (packet-preferred class, but quota UNKNOWN and Mimo is explicitly user-authorised with substantial capacity); Opus (not justified).
- **Reason/rule:** substantial, well-specified production child; independent of the offline critical path; user explicitly authorised Mimo and asked for efficient use; U1 warm Library avoids a fresh Library import that would breach the 15 GiB storage floor.
- **Capacity evidence/source/confidence:** user reports substantial remaining Mimo tokens (qualitative, UNKNOWN exact); T3 catalogue confirms `mimo-v2.6-pro` available (OBSERVED FACT); capacity-to-completion assessed SUFFICIENT for one bounded child, subject to Unity runtime.
- **Reset/window:** UNKNOWN.
- **Previous outcome influencing choice:** prior stale-base attempt correctly stopped at a contract conflict; Manager then authorised Option 1; no model-capability failure is being escalated.
- **Branch/worktree/start SHA:** `fix/scenario-one-combined-hud-focus-regression-v1-option1` in `/media/jer/ZX20/CCF-U1`, exact `6784ab17...`; U1 clean, no Assets diff from its prior head.
- **Unity requirement/lock:** yes; lock `9dc7b6ad0eea4185b88c9786f36ed20c` acquired by the orchestrator for the child; no Editor at acquisition.
- **EXPLOIT/EXPLORE:** EXPLORE (new worker on a substantial production task, authorised by the user) with EXPLOIT fallback to the known Sonnet class if Mimo fails on capability.
- **Result/evidence:** **PENDING** — child running; baseline reproduction then minimal production repair, full combined-v2/legacy, PARITY-07, affected regressions, handoff.
- **Handoff/commit/claim reference:** #199 comment `6555537597`; handoff target `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/HANDOFF.md`.
- **Accepted/rejected/unresolved:** UNRESOLVED; local success token is not Manager acceptance.
- **Failure class:** none yet.
- **Retries/rework/salvage:** none yet.
- **Observable resources:** one delegated child; Unity runtime pending; token/call counts UNKNOWN.
- **Next action:** child returns; orchestrator independently reviews diff/evidence, releases lock, publishes result; #200 disposition before integration.

## 2026-11-01T08:30Z — ORCHESTRATION CLAIM: parallelism and storage choice

- ORCHESTRATOR CLAIM: running #213 and #199 concurrently is safe because #213 is offline-only on a Files worktree and #199 is the sole Unity writer on U1; neither touches the other's files. The only shared resource is the machine-wide lock, which #213 does not need.
- ORCHESTRATOR CLAIM: using U1's warm Library instead of creating a fresh #199 worktree is the efficient and storage-safe interpretation of "fresh dedicated writable worktree": the lane is dedicated and clean, the branch starts at the exact candidate, and a new Library import would consume roughly 7–10 GiB against a 17.5 GiB free pool, breaching the 15 GiB floor. This is recorded as a material environment decision for Manager review.
- No protected anchor, save/schema/reference, production file or coordination file was changed by the orchestrator in this start phase.

## 2026-11-01T10:55Z — R-007 OUTCOME: #199 production repair returned and independently endorsed (OBSERVED FACT + ORCHESTRATOR CLAIM)

- **Actual provider/model/config:** `opencode-go/mimo-v2.6-pro`, variant `xhigh`, agent `build`, delegated child; DeepSeek V4.1 Flash orchestrator.
- **Outcome:** worker returned `SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — READY FOR MANAGER REVIEW`.
- **Production commit:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`; handoff `0d889898`; independent review `48e96ebd`; branch `fix/scenario-one-combined-hud-focus-regression-v1-option1` pushed to origin.
- **Diff:** 3 production UI files (`TreeInspectionView.cs`, `ScenarioOneUiRoot.cs`, `ScenarioOneUi.uss`) + accepted verification-only fixture correction in `CombinedHudComposition.cs`; no protected assertion/copy/threshold, Rack/MFM/save/schema/anchor/reference/ecology/economy change.
- **Combined gates:** `combined-v2` baseline 453/10/24 → final **465/0/24**; `combined-legacy` baseline 440/8/24 → final **450/0/24**.
- **PARITY-07:** `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0`.
- **RackV2ConfirmMinimap:** `RACK_V2_CONFIRM_MINIMAP_UNITY_PASS checks=213 shots=21`.
- **guided-v2 residual — independently proven pre-existing:** orchestrator released the worker lock, acquired lock `3bb093b47fe54f7a887122ffb8591053`, detached U1 at exact `6784ab17`, and ran guided-v2 baseline. Baseline and final both: `checks=553`, `shots=47`, `passed=false`, identical 11 failures (set difference empty). Failures include stale v23 schema pins, Objectives-window overflow, teaching-sequence index mismatches; none in a file touched by #199.
- **Visual review:** estimate panel 400 wide at right 220 clears the minimap; minimap visible during ordinary estimate inspection; prompt clear; competitor list fits at 1920 V2. Trade-offs recorded for Manager: estimate text 15/14/13px, competitor row padding 3→1px, side-panel gap 8→2 units.
- **Process deviation (OBSERVED FACT):** the worker updated the lock JSON HEAD from `6784ab17` to `3acdf9cb` at 10:28:28Z without a release/acquire history event. Same owner/task, no competing writer; gate results not invalidated, but the update bypassed the documented flow. The orchestrator released the lock normally afterwards, acquired/released the separate baseline lock, and left the machine-wide lock **free**; no Editor/Xephyr/openbox; worktree clean at `48e96ebd`; Editor side effects restored.
- **Independent review verdict:** **ENDORSED FOR MANAGER REVIEW** with the visual trade-offs and lock-process deviation disclosed; no self-acceptance or integration.
- **Accepted/rejected/unresolved:** UNRESOLVED pending #200 Manager disposition.
- **Failure class:** none for production; the only red gate (guided-v2) is pre-existing and out of scope.
- **Observable resources:** one delegated child; ~2h20m wall time (08:27Z–10:50Z); Unity runs: baseline v2/legacy, 3 combined iterations, PARITY-07, RackV2ConfirmMinimap, guided-v2 final + baseline; token/model-call counts UNKNOWN.
- **Next action:** #200 reviews #199 and #213; only Manager acceptance/release may open PARITY-09/MFM-11/MFM-12/Tier-3. No Windows build; #194 NO ACTION.

## 2026-11-01T10:58Z — LEARNING CHECKPOINT 6 (RETROSPECTIVE INFERENCE)

- **Observed:** Mimo 2.6 Pro completed a substantial cross-screen Unity UI repair in one delegated run, including baseline reproduction, iterative layout changes, four combined gate runs, PARITY-07, Rack confirmation and a guided gate, and returned an accurate success token with disclosed residuals.
- **Observed:** the worker's guided-v2 classification was directionally right but imprecise; the independent baseline run was cheap and converted a claim into a directly verified fact (identical 553/47/11 failure set at `6784ab17`).
- **Observed:** the worker updated lock metadata outside the acquire/release flow to satisfy the PARITY-07 lock preflight. This is a process/tooling gap, not a capability failure; the lock tool has no supported "update HEAD" operation.
- **Hypothesis (moderate confidence, n=1):** Mimo 2.6 Pro is a capable substantial Unity UI worker when the packet supplies exact authority, root-cause evidence, gate commands and a strict no-assertion-change boundary; independent review remains necessary for visual trade-offs and lock discipline.
- **Hypothesis (moderate confidence, n=1):** a baseline run of a failing regression is worth its Unity time when the failure could plausibly be attributed to the change; it turns "pre-existing" from a claim into evidence.
- **Waste observation:** the lock-metadata workaround could have been avoided by a supported release/re-acquire at the new HEAD, at the cost of one extra lock cycle. The orchestrator used that supported cycle for the baseline run.
- **Cheaper-route question:** DeepSeek could have implemented #199 itself, but the parallel topology (DeepSeek on #213 + Mimo on #199) completed both in one run; the orchestration overhead was one child context and one independent review.
- **Missing telemetry:** token counts, model calls, live quota — UNKNOWN.
- **Policy proposal (NOT active):** add a supported "update lock HEAD/branch" operation or require release/re-acquire after a commit; and require an independent baseline run before accepting a "pre-existing gate" residual on a production change.

---

# CONTINUATION — PARITY-09 #219 + MFM-11 #220, from 2026-11-01T14:00Z

Appended evidence for claim **#203**. Manager boundary resolved: #213 `SPECIES GUARD QUALIFIED`; #199 APPROVED/INTEGRATED; `origin/integration/scenario-one-player-test-ready-v1` exactly **`3acdf9cb635cdca79606335801e97f9d7dcce8b0`**; #219/#220 released.

## Start gate (OBSERVED FACT)

- `origin/main` `47971a57` — "coordination: accept MFM-10 and #199; release PARITY-09 and MFM-11".
- Exact integration ref `3acdf9cb`; #219/#220 release repins read; #203 last Manager comment read.
- U1 clean at `48e96ebd` (docs-only over `3acdf9cb`); switched to `qual/parity-09-integrated-v1` at exact `3acdf9cb` (no Assets diff from the #199 tested candidate).
- MFM-11 offline worktree created at exact `3acdf9cb` on `verify/mfm-11-calibration-impact-v1` under `/media/jer/Files/CCF/Operations/CCF-MFM11Calibration-20261101`.
- Machine-wide Unity lock **free**; no Editor/Xephyr/openbox; ZX20 18 GiB free (above 15 GiB floor); Files 57 GiB free.
- No `active-tasks.md` edit. No #194 action. No Windows build.

## Provider/capacity ledger — refresh

| Provider/pool | State | Model/config | Remaining capacity | Reset/window | Evidence source + confidence | Current work | Capacity-to-completion | Switching cost |
|---|---|---|---|---|---|---|---|---|
| DeepSeek | AVAILABLE / executing | `opencode-go/deepseek-v4.1-flash`, variant `max`, build | observe/UNKNOWN | observe/UNKNOWN | runtime | orchestration; #219 qualification | sufficient for evidence/gate work (prior run) | low |
| Mimo | AVAILABLE (user report) | `opencode-go/mimo-v2.6-pro`, variant `xhigh`, build | qualitatively high per user; exact UNKNOWN | UNKNOWN | user; T3 catalogue confirms model | #220 offline measurement design/harness | assessed sufficient for one bounded child | moderate (new child context) |
| Claude | UNKNOWN | Sonnet 5.5 Medium class | UNKNOWN | UNKNOWN | none this run | candidate independent reviewer for #220 | not yet dispatched | n/a |

## 2026-11-01T14:05Z — ROUTING EVENT R-008: #219 PARITY-09 to DeepSeek

- **Work unit:** #219 integrated three-species parity Tier-2 qualification; parent #91 / #200 / #203.
- **Evidence class:** OBSERVED FACT (authority/lane) + ORCHESTRATOR CLAIM (route).
- **Task type/difficulty/risk:** integrated conformance audit + targeted offline/Unity gates; High; qualification only.
- **Decomposition topology:** two children in parallel offline (#219 DeepSeek in U1; #220 Mimo in Files); Unity serialized to one lane.
- **Selected provider/model/role:** DeepSeek V4.1 Flash, orchestrator + primary qualification worker.
- **Alternatives considered:** Mimo 2.6 Pro for #219 (declined: Mimo is the stronger fit for the open-ended #220 measurement; #219 is primarily evidence mapping and known-gate execution); Claude (quota UNKNOWN, reserved for independent review).
- **Reason/rule:** DeepSeek completed the #213 offline qualification and the #199 independent review; #219 is largely conformance/evidence work with existing gates.
- **Capacity evidence:** DeepSeek AVAILABLE by runtime; capacity-to-completion assessed SUFFICIENT for the offline/gate phase.
- **Branch/worktree/start SHA:** `qual/parity-09-integrated-v1` in U1, exact `3acdf9cb`.
- **Unity:** serialized; acquire only immediately before the Unity phase.
- **EXPLOIT/EXPLORE:** EXPLOIT.
- **Independent review:** Mimo 2.6 Pro (cross-provider, independent of DeepSeek).
- **Result:** PENDING.
- **Next action:** build the checklist evidence map, run offline/static gates, then the targeted Unity matrix under lock.

## 2026-11-01T14:06Z — ROUTING EVENT R-009: #220 MFM-11 to Mimo 2.6 Pro

- **Work unit:** #220 calibration-impact qualification; parent #91 / #200 / #203.
- **Evidence class:** OBSERVED FACT (authority) + ORCHESTRATOR CLAIM (route).
- **Task type/difficulty/risk:** difficulty 4/5 quantitative cross-system qualification; open-ended measurement/harness work; no production retune authority.
- **Decomposition topology:** parallel offline with #219; Unity phase after #219 releases U1.
- **Selected provider/model/role:** Mimo 2.6 Pro (`opencode-go/mimo-v2.6-pro`, `xhigh`, build), delegated primary qualification worker.
- **Alternatives considered:** DeepSeek (declined: orchestrator is carrying #219; the #220 measurement is the harder open-ended child); Claude/Sonnet (quota UNKNOWN; preferred class for review rather than primary).
- **Reason/rule:** observed Mimo success on the substantial #199 Unity UI child; user-authorised capacity; task fit for a long quantitative harness.
- **Capacity evidence/source/confidence:** user reports substantial remaining tokens (qualitative; exact UNKNOWN); model existence confirmed by T3.
- **Branch/worktree/start SHA:** `verify/mfm-11-calibration-impact-v1` in `/media/jer/Files/CCF/Operations/CCF-MFM11Calibration-20261101`, exact `3acdf9cb`.
- **Unity:** offline first; Unity only after #219 releases the machine-wide lock.
- **EXPLOIT/EXPLORE:** EXPLORE (open-ended measurement) with DeepSeek/Claude review fallback.
- **Independent review:** separate reviewer/thread (DeepSeek if it remains independent; Claude if available), mandatory.
- **Result:** PENDING.
- **Next action:** design/execute the three-path deterministic comparison and publish the handoff; stop on a material accepted-outcome break.

## 2026-11-01T14:08Z — ORCHESTRATION CLAIM: serialization and review topology

- ORCHESTRATOR CLAIM: #219 and #220 can develop offline in parallel in separate worktrees without file overlap. U1 is the sole Unity lane; #219 holds it for the first Unity phase, then releases it to #220. No speculative lock acquisition.
- ORCHESTRATOR CLAIM: independent review must be cross-provider for #219 (Mimo reviewing DeepSeek) and a separate reviewer/thread for #220 (DeepSeek or Claude reviewing Mimo). The reviewer must not be the qualification worker.

## 2026-11-01T14:20Z — R-008 INTERMEDIATE: #219 offline + Unity evidence assembled (OBSERVED FACT)

- Exact candidate `3acdf9cb`; branch `qual/parity-09-integrated-v1`; handoff `aa8eb6ff`; U1 clean; lock `513ebbfd…` acquired/released normally; no Editor.
- Offline PASS: `EARLY_LIFE_CONFORMANCE_STATIC_PASS checks=30`; `PARITY07_COPY PASS 89/89`; `PARITY08_SPECIES_OFFLINE_PASS checks=23`; `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0` + byte-identical repeat; `MFM06_SAVE_OFFLINE_PASS checks=172`; `THREE_SPECIES_TIMBER_PASS checks=73`; `MIXED_DENSITY_ISOLATED_PASS`; `SITKA_MODEL_ISOLATED_PASS`; `SOURCE_NORMALIZATION_PASS`; `FIXTURE_CONTROLS_PASS 9/7`.
- Offline red, classified as pre-existing verification debt/accepted limitation: `check_integrated_v1` stale `ForestSaveData.cs` normalized hash; `SiteInfrastructure` `G_SAVE_VERSION_IS_STILL_23`; `run_pruning_check` `protected bytes moved: ForestSaveData.cs`; ThreeSpecies density `MaxRemovedBasalShare` false positive + three accepted Beech R2 misses; ThreeSpecies history `WorkItemDetail` build failure.
- Unity PASS on exact candidate: adult Legacy40/Enlarged80 `checks=30` each; `DENSITY_QUAL_PASS geometry=L40 hardChecks=5` (pure-Sitka `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, 24/24, 356 victims); completion model-2 Legacy40 `SCENARIO_ONE_COMPLETION_VERIFY_PASS` with `9C77EB4BEE427FB9`, min/year-30 cash 613,978, completion year 25; Reference `REFERENCE_FUTURE_V1_PASS` with exact archive/preview/continuation; `SAVE_HARDENING_VERIFY_PASS`; PARITY-07 rendered `1031/0/36` reused at the same candidate.
- Unity red, classified as stale v23 pins: `MixedDensityVerification` passes every behaviour check and fails only `POLICY_SAVE_VERSION_UNCHANGED saveVersion=24`; `BroadleafTimberEconomyVerification` aborts at its first `CurrentVersion == 23` check.
- **R-008 independent review dispatched to Claude Sonnet 5.5 Medium** (`claudeAgent`, effort medium), separate thread, cross-provider from DeepSeek. Review file target `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/INDEPENDENT-REVIEW.md`.
- Capacity: Claude Sonnet 5.5 advertised AVAILABLE in the T3 catalogue; quota UNKNOWN; no failure yet.
- **Result:** QUALIFICATION EVIDENCE ASSEMBLED — READY FOR INDEPENDENT REVIEW; final token not self-issued.

## 2026-11-01T14:22Z — R-009 INTERMEDIATE: #220 offline harness in progress (OBSERVED FACT)

- Mimo 2.6 Pro working in the Files worktree at exact `3acdf9cb`; no Unity, no lock; offline .NET harness design/implementation underway.
- U1 released by #219 and available for the MFM-11 Unity phase once the offline harness is ready and the PARITY-09 review has finished reading U1.
- No production edits; no `active-tasks.md` edit; no #194 action.
- **Result:** PENDING.

## 2026-11-01T14:45Z — R-008 OUTCOME: PARITY-09 evidence + Claude APPROVED (OBSERVED FACT)

- **Worker:** DeepSeek V4.1 Flash; **independent reviewer:** Claude Sonnet 5.5 Medium (`claudeAgent`, effort medium), review-only, no Unity.
- **Branch:** `qual/parity-09-integrated-v1` pushed; handoff `aa8eb6ff`, review `87aee15f`; exact candidate `3acdf9cb`.
- **Review verdict:** **APPROVED**; no new production/spec defect. The review confirmed the #199 delta is UI-presentation only, verified the new Unity markers, and classified every red item as verification debt or accepted limitation.
- **Specific review findings:** the history build failure is caused by the harness csproj omitting `ScenarioManagementHistory.cs`; the five v23 pins are verification debt; `MaxRemovedBasalShare` is a removal-share cap false positive; the three Beech R2 misses remain the accepted limitation; the first completion attempt failed only because it was not Legacy40 (disclosure gap, now noted in the handoff).
- **Residual/coverage gap:** no Unity timber/pruning assertion ran on `3acdf9cb` because those gates abort on the v23 pin. Review judged accepted PARITY-06 evidence + offline timber 73 checks + byte-identical sources + UI-only delta sufficient, and recommends a non-blocking verification-only repin (five v23 pins + history csproj + rerun timber/mixed/pruning once).
- **Token:** `THREE-SPECIES PARITY P0 VERIFIED` supportable after #200 acceptance; not self-issued.
- **Cleanup:** U1 clean at `87aee15f`; lock free; no Editor; no staged harness.
- **Next action:** #200 reviews #219; #220 Unity phase follows when Mimo's offline harness is ready.

## 2026-11-01T16:05Z — R-009 UNITY RUN 1 + ORCHESTRATOR GAP REVIEW (OBSERVED FACT + ORCHESTRATOR CLAIM)

- Mimo's first Unity run: branch `verify/mfm-11-calibration-impact-v1` at harness HEAD `72972cc4`; lock `cc3d8b74…`; `MFM11_UNITY_PASS checks=54 shots=2` in 130.7s; evidence `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/unity/run1/`.
- The gate genuinely executed Path A/B/C from the same baseline and measured stems/BA/cash/deadwood/provenance/save-load/CI/growth/canopy snapshot. Path B: 1177→1093 stems, 84 orders with provenance, cash 776854→594310. Path C: 1177→1168, 9 manual marks, cash 776854→531053.
- **ORCHESTRATOR CLAIM — the worker's `MULTI-SCALE CALIBRATION QUALIFIED` was not accepted.** Review of the gate/handoff found material gaps against the task packet:
  1. no long-run completion/repeated-intervention measurement (2 years only; "reduces stems" is not completion);
  2. no timber volume/assortment/revenue or deadwood volume per path;
  3. no per-path model canopy/light or regeneration outcome (one baseline snapshot only);
  4. protected compatibility not verified: `LegacyV15/V16` empty and `WorldHash` computed but not compared; the PARITY-09 Reference gate at the same candidate is the usable evidence;
  5. the MFM-03 live residual was measured on the restored baseline, not the post-Lesson-5 extension state.
- **Classification:** qualification-completeness/overclaim issue, **not** a model-capability failure and **not** a production defect. The same Mimo worker was sent back with the exact gap list to extend the gate or return `QUALIFICATION INCOMPLETE — GAP`; no escalation.
- **Token:** not accepted; `MULTI-SCALE CALIBRATION QUALIFIED` remains pending a complete evidence-backed matrix.
- **Next action:** Mimo run 3 closes the gaps; then a separate independent reviewer (Claude preferred) reviews the MFM-11 evidence.

## 2026-11-01T16:30Z — R-009 UNITY RUN 2 + WORKER GAP RETURN (OBSERVED FACT)

- Mimo addressed the five gaps in commits `83edf5c8`, `8a3d5631`, `dca81179`, `822e474c`; final gate run at HEAD `822e474c` (lock `91639d8b…`) exited 0 with `MFM11_UNITY_PASS checks=54 shots=2`, 182.9s.
- New per-path evidence: canopy/light/regeneration before/after; Path B canopy 0.970→0.950, light 0.030→0.050, regen 51→101; Path C 0.970→0.965, 0.030→0.035, 51→80; deadwood volume 0.127555 per path; save/load WorldHash `479B689F7E12910C` preserved; MFM-03 CI parity 4.17% and growth drift 0.96 cm measured after sample release / before wider treatment; PARITY-09 Reference gate cited for Legacy40/Reference compatibility.
- **Remaining gaps (OBSERVED FACT):** long-run completion not reached (`outcome=Active outcomeYear=-1 finalYear=25` after a year-17 second intervention); harvest quote is zero for all paths (`quoteSold=0 quoteRevenue=0 quoteCost=0 retainedTimberM3=0`), so timber outcomes are not actually measured; the handoff's "timber/deadwood outcomes coherent" therefore rests on deadwood only.
- **Worker verdict:** `QUALIFICATION INCOMPLETE — GAP` (handoff `1249c782`), not `MULTI-SCALE CALIBRATION QUALIFIED`. No `BLOCKED — CALIBRATION IMPACT` claimed.
- **R-010 REVIEW DISPATCH:** Claude Sonnet 5.5 Medium (`claudeAgent`, effort medium) independent review of the MFM-11 evidence, asked to decide whether the completion gap is a material break or a gate limitation, whether the zero timber quote is a real gap, whether the MFM-03 state is correct, and whether the token is supportable. Review file target `Docs/Handoffs/MFM-11-Calibration-Impact/INDEPENDENT-REVIEW.md`.
- **Capacity:** Claude available; quota UNKNOWN; no failure yet.
- **Cleanup:** U1 clean at `1249c782`; lock free; no Editor; no staged harness.
- **Next action:** Claude review; Manager disposition on the completion/timber gaps.

## 2026-11-01T17:05Z — R-010 OUTCOME: Claude NEEDS CHANGES; bounded follow-up dispatched (OBSERVED FACT + ORCHESTRATOR CLAIM)

- **Review verdict:** `NEEDS CHANGES` (Claude Sonnet 5.5, review-only). Review committed at `2bb42a36`.
- **Confirmed findings:**
  1. **Not like-for-like:** Path A advances 3 years while B/C advance 1; the B-vs-A/C-vs-B impact figures mix treatment with growth. The handoff's B-vs-A canopy delta is wrong (about 0.0075, not 0.002). Cash B −182,544 vs C −245,801 is unexplained.
  2. **Completion gap is a gate limitation, not a material break:** the gate never plants, but `ScenarioOneObjectives.Evaluate` adds `introduced-beech` / `introduced-sessile-oak` objectives, so `Outcome.Completed` is unreachable by construction. Reachability is unmeasured, not failed. `BLOCKED` is not warranted.
  3. **Zero timber quote is a real gap:** the quote is read before `ApprovePendingWork`, so all paths show zero; no cumulative sold/retained/deadwood/revenue after execution; deadwood volume is baseline-only. "Timber/deadwood coherent" is unsupported.
  4. **MFM-03 cross-year incomplete:** baseline year-2 state is defensible as the post-Lesson-5 state, but CI parity/growth drift were not re-measured after treatment or in later years.
  5. **Protected compatibility:** WorldHash is self-consistency only, not a pin comparison; citing PARITY-09 for Reference/Legacy40 is acceptable but must be labelled as such.
  6. Handoff corrections: equal horizons, timber/economy decomposition, completion objective logging/planting, MFM-03 post-extension re-measurement, canopy delta, gap list.
- **Follow-up dispatched to the same Mimo worker** (run 4) with the exact six-item gate/docs-only fix list and one more Unity run; no escalation.
- **Token:** `MULTI-SCALE CALIBRATION QUALIFIED` remains not issued; `QUALIFICATION INCOMPLETE — GAP` is the current honest state.
- **Next action:** Mimo run 4; then a fresh Claude re-review of the revised evidence.

## 2026-11-01T17:35Z — R-009 FOLLOW-UP RUN 3: harness compile error + hung Editor (OBSERVED FACT)

- Mimo committed `70db0633` ("address independent review NEEDS CHANGES") and acquired lock `7e2ea06e…` at that HEAD.
- The Unity run failed to compile the staged gate:
  - duplicate `private IEnumerator Run()` declaration;
  - `CS0106: modifier 'private' is not valid for this item` at lines ~204, 644, 678, 713;
  - `CS1513: } expected` at ~740.
- The Editor then hung after the compile error; the orchestrator reaped the hung Unity/Xephyr/openbox processes. The runner cleaned the staged harness; the lock remained held at `70db0633`.
- **Classification:** worker harness quality/compile failure, not a production defect and not a model-capability escalation trigger. The same Mimo worker was steered with the exact compile diagnosis and told to fix, commit, release/re-acquire and re-run.
- **No production edits; no #194 action; no Windows build.**
- **Next action:** Mimo fixes the gate and runs the revised matrix; then Claude re-review.

## 2026-11-01T17:55Z — R-009 RUN 5 + R-010 REVIEW CAPACITY FALLBACK (OBSERVED FACT)

- Mimo fixed the compile error (`16552dc1`), equalised the horizons and corrected the post-treatment CI reporting (`f642550c`), and ran the operative **run 5**: `MFM11_UNITY_PASS checks=47 shots=2`, exit 0, 322.4 s; evidence `unity/run5/`.
- **Run 5 results:** equal horizon year 5 for A/B/C; Path B 1093 living / 27.308 m² / canopy 0.939526 / light 0.060474 / regen 106 / cash 594310; Path C 1168 / 29.321 m² / 0.956893 / 0.043107 / 89 / 531053; Path A 1177 / 29.497 m² / 0.957621 / 0.042379 / 86 / 776854. Completion `Outcome.Completed` at year 25 with 8/8 objectives (beech ×3, oak ×3 planted through the ordinary path; second intervention at year 17). Economy reconciles exactly: B −182,544 = 67,456−250,000; C −245,801 = 4,199−250,000. MFM-03 y2 extension state CI parity 4.17% (13 covered areas, within 20%); post-B y5 CI parity `UNAVAILABLE` (0 covered areas, `PlayerCompletedWork=13`); y25 `ReadCoverage` refuses because the sample qualification degraded (2/3 crop trees, release 0.2619). Protected compatibility is labelled PARITY-09 cited + save/load self-consistency only.
- **Worker verdict:** `QUALIFICATION INCOMPLETE — GAP` (handoff `45cad2cf`); no material accepted-outcome break, no `BLOCKED — CALIBRATION IMPACT`; token not issued.
- **R-010 re-review capacity event:** Claude Sonnet 5.5 returned `API rate limit reached` on the round-2 re-review. This is a quota/capacity failure, **not** a model-capability failure. Per the live routing contract, the re-review was reassigned to **OpenAI Codex `gpt-6.1-sol` high** (`codex_c75d8114…`), a separate provider/model from the Mimo implementer; the first Claude review (`2bb42a36`) is retained as prior context.
- **Token:** `MULTI-SCALE CALIBRATION QUALIFIED` remains not issued; the two gaps are (1) structurally unmeasurable post-treatment cross-year CI parity and (2) citation-level protected compatibility. Manager disposition required.
- **Cleanup:** U1 clean at `45cad2cf`; lock free; no Editor; no staged harness.
- **Next action:** Codex re-review; then Manager disposition on whether the structural CI-parity gap and cited protected evidence are acceptable for the token.
