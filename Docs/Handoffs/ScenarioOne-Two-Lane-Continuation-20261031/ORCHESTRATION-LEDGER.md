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
