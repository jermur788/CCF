# MFM-09 — Orchestration Evidence Ledger — 2026-10-31

Parent: #203 / #201. Child claim: #207. Manager release: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/RELEASE-REPIN-20261031.md`.
Orchestrator: opencode-go/deepseek-v4.1-flash (T3/OpenCode).

## Starting policy record

- Objective: implement and Tier-2 qualify the approved `Extend your treatment` teaching / Work Plan integration on exact `34e360f85835afd15f5323f287372e0f2ee83dd9`; publish HANDOFF; return `SCENARIO ONE EXTENSION FLOW QUALIFIED` or a bounded blocker.
- Hard constraints: preserve all eight existing teaching IDs and the v24 D-066 latch; insert new extension ID(s) after `response` and before `recruitment`; prescription approval creates ordinary Pending work only; Work Plan approval remains the commitment gate; do not duplicate/redesign MFM-08 review UX; no save/schema/hash/Reference/Rack/Annual Review/contractor/economy changes; no Windows build; #194 NO ACTION.
- Routing policy: DeepSeek orchestrates/default for bounded work; Sonnet 5.5 Medium for genuinely hard cross-system implementation/qualification; Opus only after a classified Sonnet capability/reasoning failure or separately justified high-consequence need.
- Fresh coordination state: `origin/main` `1b09e8f4`; #208/#209 APPROVED/CLOSED; #207 READY and next local priority; published integration `34e360f8...`; U1 clean at candidate after branch creation; ZX20 18 GiB free; Unity lock free; no Editor/Xephyr.
- Lane/branch: `/media/jer/ZX20/CCF-U1`, `feature/mfm-09-teaching-workplan-integration-v1`, start `34e360f8...`.
- Unity lock: free at preflight; to be acquired fresh by the worker only for the Unity phase.
- Evidence root: this directory. Handoff target: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/HANDOFF.md`.

## Provider pool state (at dispatch)

| Pool | State | Model/effort | Capacity estimate | Source/confidence | Current work | Capacity-to-completion |
|---|---|---|---|---|---|---|
| DeepSeek | AVAILABLE | deepseek-v4.1-flash | unknown | runtime identity | orchestration | sufficient for orchestration |
| Claude A | AVAILABLE at dispatch | Sonnet 5.5, medium | unknown | T3 provider catalogue | #207 implementation/qualification | unknown until run |
| Claude B | UNKNOWN | Sonnet 5.5 | UNKNOWN | not queried | none | UNKNOWN |
| OpenAI/Codex | UNKNOWN | not selected | UNKNOWN | not queried | none | UNKNOWN |

## Event — 2026-10-31T23:05Z — preflight, branch, claim

- **Work unit:** #207 MFM-09.
- **Evidence class:** OBSERVED FACT.
- **Task characteristics/risk:** Medium/High cross-system teaching + Work Plan integration; production changes authorised on a bounded surface; Unity Tier-2 required; no save/schema/hash changes.
- **Selected provider/model/role:** DeepSeek orchestrator; Claude A / `claude-sonnet-5-5` / effort medium / implementation + qualification.
- **Reason/rule:** cross-system teaching/Work Plan integration explicitly routed to Sonnet-class execution; DeepSeek remains orchestrator/default for bounded work.
- **Branch/worktree:** U1 `feature/mfm-09-teaching-workplan-integration-v1` at `34e360f8`.
- **Preflight:** origin/main `1b09e8f4`; lock free; no Editor/Xephyr; U1 clean; 18 GiB free.
- **Claim:** #207 RUNNING with provider/model/branch/start SHA; lock receipt deferred to the Unity phase.
- **Next action:** dispatch Sonnet implementation/qualification worker.

## Event — 2026-10-31T23:08Z — execution dispatch

- **Work unit:** #207 MFM-09.
- **Evidence class:** OBSERVED FACT (dispatch) / ORCHESTRATOR CLAIM (routing judgement).
- **Selected provider/model/role:** Claude A / `claude-sonnet-5-5` / effort `medium` / implementation + Tier-2 qualification, sole Unity operator.
- **Alternatives considered:** DeepSeek direct execution (bounded-work default, but this is a cross-system teaching/Work Plan production change plus Unity qualification); Opus (no classified Sonnet capability failure); Claude B (not needed; Claude A available first per user direction).
- **Reason/rule:** user routing direction + packet difficulty + cross-system production/Unity scope.
- **Task ID:** `node:delegated-task:command:mcp:addbb180f-7f5c-47af-9ab1-35ad7e1de630:delegate-task:mfm09-claude-a-medium-20261031-01`.
- **Child thread:** `thread:delegated-task:command:mcp:addbb180f-7f5c-47af-9ab1-35ad7e1de630:delegate-task:mfm09-claude-a-medium-20261031-01`.
- **Mode/status:** async, running/working.
- **Lock:** free at dispatch; worker must acquire fresh only for Unity and reacquire at each exact new HEAD.
- **Expected interruption/switch cost:** medium-high; Unity state and branch/lock ownership would need handover if provider fails. Durable commits/evidence required.
- **Next action:** await worker completion/blocker; inspect evidence; do not duplicate work.

## Event — 2026-10-31T23:35Z — Claude A capacity failure; salvage checkpoint

- **Work unit:** #207 MFM-09.
- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM.
- **Failure:** Claude A (`claudeAgent`, `claude-sonnet-5-5`, medium) terminated with `Claude API rate limit reached. Try again later.` This is a provider capacity failure, not a capability/reasoning failure.
- **Lane state at failure:** U1 on `feature/mfm-09-teaching-workplan-integration-v1`; HEAD `917390a545e7b18519748ffb8210d99b795e5adc`; clean; lock free; no Editor.
- **Partial work preserved:** commit `917390a5` changes only the four expected teaching/Work Plan production files and adds `Tools/Verification/Mfm09ExtensionFlow/` (offline check + runner). No save/schema/hash/Reference/MFM-08/Rack/ScenarioOneUiRoot file changed.
- **Partial evidence:** `/media/jer/Files/CCF/Verification/MFM09-20261031/offline-dev/` shows `MFM09_OFFLINE_PASS checks=32` and all 17 mutation controls caught; the two broad regression logs fail/abort against the stale `bccd2b90` base (many pre-existing accepted changes) and are not evidence of an MFM-09 defect.
- **Classification:** provider capacity; no code failure established. Do not escalate to Opus.
- **Routing decision:** mark Claude A COOLDOWN; resume the same work on Claude B (Sonnet 5.5 medium) with the durable checkpoint above. No second writer: lane is free/clean and the prior child is terminal.
- **Next action:** dispatch salvage worker to verify/finish offline regressions, run Unity Tier-2, and publish handoff.

## Event — 2026-10-31T23:38Z — Claude B salvage dispatch

- **Work unit:** #207 MFM-09.
- **Evidence class:** OBSERVED FACT (dispatch) / ORCHESTRATOR CLAIM (routing).
- **Previous worker/pool:** Claude A (`claudeAgent`, Sonnet 5.5 medium) — COOLDOWN, rate limit after durable checkpoint `917390a5`.
- **Selected worker/pool:** Claude B — Pro (`claudeAgent_claude_b_pro`), Sonnet 5.5 medium, same branch/worktree/HEAD.
- **Alternatives considered:** retry Claude A (unknown reset); DeepSeek direct (harder cross-system implementation/Unity; not the preferred escalation); Codex (not preferred while a Sonnet pool is available); Opus (not justified — capacity failure is not a capability failure).
- **Reason/rule:** same-model-family capacity fallback to the second Claude subscription pool; preserves capability fit and avoids Opus.
- **Capacity-to-completion:** unknown at dispatch; Claude B observed AVAILABLE in the provider catalogue.
- **Switching cost:** low — clean lane, committed checkpoint, evidence preserved; no duplicate writer.
- **Task ID:** `node:delegated-task:command:mcp:addbb180f-7f5c-47af-9ab1-35ad7e1de630:delegate-task:mfm09-claude-b-salvage-20261031-01`.
- **Child thread:** `thread:delegated-task:command:mcp:addbb180f-7f5c-47af-9ab1-35ad7e1de630:delegate-task:mfm09-claude-b-salvage-20261031-01`.
- **Mode/status:** async, running/working.
- **Next action:** await completion/blocker; inspect evidence; do not duplicate work.

## Event — 2026-11-01T01:10Z — Claude B completed; orchestrator verification

- **Work unit:** #207 MFM-09.
- **Evidence class:** OBSERVED FACT.
- **Worker:** Claude B — Pro (`claudeAgent_claude_b_pro`), Sonnet 5.5 medium; one session; no Opus; no further capacity failure observed.
- **Result reported:** `SCENARIO ONE EXTENSION FLOW QUALIFIED` (worker handoff, not Manager acceptance).
- **Branch/HEAD:** `feature/mfm-09-teaching-workplan-integration-v1`; tested production `6784ab17...`; handoff commit `a0393712...` before this appendix.
- **Lane state:** clean including untracked; lock `380f3eeece3444a19bfa84de8c13ba87` released normally; `unity-lock status` free; no Editor/Xephyr; 18 GiB free.
- **Production diff:** exactly four expected UI files, +92/−7; no save/schema/hash/Reference/MFM-08/Rack/ScenarioOneUiRoot change.
- **Evidence verified:** 29 Unity receipts (one render path failure re-run as 29-render), extension `MFM09_UNITY_PASS checks=261 shots=16`, 27/28 protected gates first-time pass with accepted markers, all receipts clean/host-save-unchanged; offline `MFM09_OFFLINE_PASS checks=32`, 17/17 mutants; T-04/T-04P/T-05/S1-C/MFM regressions pass in accepted current forms.
- **Residual:** no-Covered-area/no-prescription world keeps the extension instead of recruitment; product/teaching decision for Manager. Harness debt: stale regression bases and pre-existing UX/RackV2 help failures. No independent-provider review.
- **Claim:** #207 to be retitled `[HANDED-OFF]` with token, branch/HEAD, evidence and residual.
- **Next action:** #200 Manager review of handoff/evidence and no-Covered-area decision; integrate or request changes.

## Final V2 summary

- **Policy:** DeepSeek orchestrates; Sonnet 5.5 Medium executes; Claude A -> rate limit -> Claude B salvage; no Opus.
- **Capacity:** Claude A COOLDOWN after rate limit; Claude B available and sufficient; token/context numbers UNKNOWN.
- **Decomposition:** one coupled implementation/qualification unit; committed checkpoint made pool switch cheap.
- **Outcome:** worker token `SCENARIO ONE EXTENSION FLOW QUALIFIED`; production diff four expected UI files; 29 Unity receipts; 17/17 offline mutants; lane clean/lock free.
- **Learning:** second Sonnet pool recovered without Opus; no-Covered-area residual and stale harness bases are the notable follow-ups; no independent review yet.
- **Evidence rule:** receipts/Git/lock/logs = observed facts; routing = orchestrator claim; sufficiency/learning = retrospective inference.
