# CCF Game Dev — Overall Manager Transition Handoff

**Date:** 2026-10-28
**Status:** CURRENT CCF OVERALL MANAGER HANDOFF
**Continuation claim:** #64
**Pre-handoff CCF main:** `7f477b097e45c3ea0c6480c6590ae99583fe8503`

## Purpose

This is the durable continuation handoff for a fresh Overall Manager chat in the **CCF Game Dev project**. It transfers current management state, active gates, authority, routing guidance and immediate priorities without requiring replay of the previous conversation.

This file is management context, not implementation proof. Where it conflicts with newer repository/Unity evidence or a later direct user instruction, inspect and use the newer authority.

## New-manager startup order

1. Read the Game Dev Project Instructions.
2. Read this handoff.
3. Read current `Docs/Project/decision-log.md`.
4. Read current `Docs/Project/current-milestone.md`.
5. Read current `Docs/Project/game-brief.md`.
6. Read live `Docs/Project/coordination/active-tasks.md` from current `main`.
7. Refresh the open GitHub claims named below before dispatching or integrating anything.
8. Inspect repo/Unity evidence before accepting worker claims.
9. Continue automatically through already-authorised review/integration/next-task gates; stop only for a product decision, scope change, ownership conflict, mandatory review, unsafe integration, user test or material ambiguity.

On takeover, mark claim #64 `[RUNNING]` with the actual Manager model/provider and current CCF `main` SHA. After reconciling this handoff against live repo/claims, close #64 as `[HANDED-OFF]`.

---

## Authority and Manager role

Accepted product intent remains, in order:

1. latest direct user instruction;
2. `Docs/Project/decision-log.md`;
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

Approved specs under `specs/` are task-level authority only and remain subordinate. Implementation truth is inspected repo / verified Unity evidence, not chat claims.

The Overall Manager owns direction, milestone boundaries, ownership/concurrency, routing, specs, handoff review, integration decisions and verification gates. Do not become the primary implementation lane for substantial work. Substantial Unity/C#/tooling/debugging/research belongs in delegated lanes with a canonical packet/handoff and task claim.

`Docs/Project/coordination/active-tasks.md` is live single-writer coordination authority. Claims track execution presence only. Refresh it before claiming a lock or writer is absent. Preserve unrelated dirty work; never stash, reset, discard, overwrite, clean or absorb it.

Machine-wide Unity execution remains serialized. Verify branch/HEAD/ownership before acquiring the lock. Never bypass another owner or kill its process. No Windows build before the user personally playtests the Scenario One candidate and explicitly authorises a Windows build.

Keep IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED and ACCEPTED distinct.

---

## Current product boundary

Current milestone remains **Scenario One — Complete-Loop Three-Species Demo Readiness**.

The first external demo is the **complete Scenario One**, not an early first-intervention pilot. Sitka spruce, Oak and Beech require comparable modelling depth. S1-C and S1-D are pre-demo. The player loop remains:

`decide -> advance -> walk -> observe -> understand why -> decide again`

### Rack V2 / D-064

Rack V2 is APPROVED for Enlarged80. Rack geometry is planned on the Stand Map; confirmed entrances are physically marked in 3D; Legacy40 and Reference Future v1 compatibility remain mandatory.

OPEN-3 authority is canonical:

- zero executed removals => a rack can reopen/edit through authoritative reconciliation/cancellation;
- reopening itself cancels nothing;
- edits that cancel approved-but-unexecuted work require warning + explicit consent;
- >=1 executed removal => that rack geometry is locked/harvested;
- executed removals are never reversed;
- only the geometry-changed rack loses its geometry-bound entrance mark.

Reference Future v1 remains frozen.

### Multi-Scale Forestry Management / D-065

MFM V1 is APPROVED FOR IMPLEMENTATION PLANNING and required before beginner/external testing. It applies demonstrated sample treatment to comparable rack-served areas; proposals are not Fell marks; player judgment remains required; Work Plan approval is the commitment gate.

Production coding remains gated by Rack V2 persistence/integration. MFM measurement/calibration work may proceed within its packet.

---

## Critical active lane — RACK-V2-08 and its layout blocker

### RACK-V2-08 — claim #49

Issue #49 is currently **OPEN / BLOCKED**.

Task: remove competing first-person rack authoring from Rack V2 while retaining model-0/Legacy40 first-person compatibility and preserving ordinary Fell/Crop/inspection/later entrance interaction.

Canonical packet:

`Docs/Handoffs/RACK-V2-08/TASK-PACKET.md`

Manager G3 split disposition:

`Docs/Handoffs/RACK-V2-08/G3-DISPOSITION.md`

Combined HUD blocker disposition:

`Docs/Handoffs/RACK-V2-08/COMBINED-HUD-MANAGER-DISPOSITION.md`

Existing branch/worktree:

- branch: `feature/scenario-one-rack-v2-no-competing-author-v1`
- worktree: `/media/jer/ZX20/CCF-RackV2-08`
- production implementation: `6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5`
- exact Unity-tested candidate: `7dd54ee679729da07013c3e129d8485c6067495f`
- published blocked tip: `5b0ed56` (short form recorded in handoff/claim)

What already passes on the candidate:

- genuine Legacy40 native first-person B/origin/reselect/cancel/confirm path: 15 checks PASS;
- model-0 Guided/Sequential: 600 PASS / 0 FAIL;
- Rack V2 Guided/Sequential: 553 PASS / 0 FAIL;
- unchanged RACK-V2-07 confirm/reopen/minimap gate: 213 PASS;
- protected non-intro assertion inventory retained.

The Combined HUD gate fails because of **pre-existing mainline layout defects outside the RACK-V2-08 production diff**. The same failures reproduced on unmodified main. Therefore #49 is not rejected; it is blocked pending a separate repair.

The new V2-only `Show trees` check correction is Manager-approved: the original new check incorrectly required a legacy RackGroup control that V2 never creates. The replacement asserts V2's real per-job presentation and Sell/Remove controls. The protected model-0 `Show trees` assertion remains unchanged and passes.

### RACK-V2-08-LAYOUT-REPAIR-01 — claim #59

Issue #59 is the **active repair dependency** and was RUNNING when this handoff was written.

Executor recorded on #59:

- T3 Code / OpenCode Go / `deepseek-v4.1-flash`
- variant `max`
- branch `fix/rack-v2-combined-hud-layout-v1`
- code worktree `/media/jer/ZX20/CCF-RackV2Layout`
- Unity qualification lane `/media/jer/ZX20/CCF-U1`
- starting HEAD `4923484cd819f707207782a12754b8f7809a1f16`

Scope is deliberately narrow:

1. Stand Map caption/window overflow introduced by the separate `map-tools` row;
2. Rack V2 minimap teaching-note height squeezing estimate/competitor panels.

Protected Combined HUD assertions must not be weakened/rebaselined. Required qualification is both model branches at 1280x720 and 1920x1080, preserving RACK-V2-07 confirm/minimap and applicable Stand Map/Guided checks.

**Next Manager action on Rack V2:** consume #59's handoff when it arrives, inspect exact candidate/diff/Unity evidence, issue APPROVED / NEEDS CHANGES / REJECTED, integrate if approved, then resume #49 on its existing branch/worktree by bringing in repaired `main` and rerunning Combined HUD plus proportionate preservation gates. Only after that may #49 return `NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`.

Do not let #49 absorb the layout repair before #59 is resolved.

---

## Other open CCF lanes to refresh before acting

These claims were still open/dispatched at handoff creation. Do not create duplicates; fetch current issue comments/status first.

### MFM-03 — claim #39

`[DISPATCHED] MFM-03 — coverage calibration measurement`

Packet: `Docs/Handoffs/MFM-Implementation-03/TASK-PACKET.md`

Measurement/calibration only: rack-served variation, former-edge effect, outer remainders, regeneration/access exclusions, candidate-spacing workload, sensitivities/tolerances. No prescription/proposal/save/Work Plan/UI implementation. Unity is permitted only under the machine-wide lock.

### PARITY-05 — claim #32

`[DISPATCHED] PARITY-05-PATCH — early-life/browse conformance labels and verification`

Packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/TASK-PACKET.md`

Accepted decisions remain:

- preserve response-curve architecture;
- browse palatability Sitka 0.15 / Beech 0.5 / Oak 1.0;
- binary protection;
- 0.2 is background browse, not a protected multiplier;
- Beech combined evidence is `FALLBACK-CONSERVATIVE`;
- unknown-save-species handling is deferred to PARITY-08.

The patch is deliberately no-behaviour-change unless later authority says otherwise.

### S1-C T-03 — claim #19

`[DISPATCHED] S1-C-IMPL-03 — intervention drill-down and money honesty`

Packet: `Docs/Handoffs/S1-C-Implementation-03/TASK-PACKET.md`

T-01/T-02 are integrated. T-03 is aggregate-first Annual Review intervention drill-down with honest grouped-harvest settlement attribution. No T-04+, save/schema, ecology/economy, Rack, S1-D or MFM expansion.

### S1-D T-01 — claim #13

`[DISPATCHED] S1-D-T01 — repeated-intervention baseline re-verification`

Packet: `Docs/Handoffs/S1-D-Implementation-01/TASK-PACKET.md`

Read-only baseline verification. No production edits. The expected success token is `S1-D BASELINE RE-VERIFIED — T-02..T-06 MAY BE DISPATCHED`.

### Coordination-file caution

At handoff creation, `Docs/Project/coordination/active-tasks.md` contains some stale historical status text (for example older Rack V2 rows). Treat the file as ownership authority, but the new Manager should reconcile stale statuses against current merged PRs and live claims before making new dispatch decisions. Do not casually rewrite unrelated rows.

---

## Recently accepted/integrated CCF results

### Rack V2

- RACK-V2-05 map editing: APPROVED / INTEGRATED, PR #44 merge `0ec6bc4d473975e9938040026737e70a33432c48`.
- RACK-V2-06 spacing teaching: APPROVED / INTEGRATED, PR #46 merge `3b5e157fa5ea7678d54606b4f0f5285a039f47e3`.
- RACK-V2-07 confirmation + minimap: APPROVED / INTEGRATED, PR #48 merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`.
- OPEN-3 authority sync: APPROVED / INTEGRATED, PR #51 merge `c15df9f916f4dab5fca66ad8348a620370841911`.

After RACK-V2-08 completes, inspect/dispatch the next approved Rack V2 task (normally RACK-V2-09) only after confirming current spec/tasks and ownership.

### MFM

MFM-01 approved. MFM-02 rack-served sample geometry is APPROVED / INTEGRATED through PR #38 merge `07eee3d035a95e02b294e233482e791ce06d4250`. A real player-authored Rack V2 requalification remains a later integration concern; the five-rack fixture is not a general rebaseline.

---

## Model / subscription research — accepted advisory evidence

The project now has both an OpenAI-produced study and a blind Claude Sonnet 5.5 replication. Both are **advisory evidence**, not immutable routing authority.

Canonical research set:

- `Docs/Research/ModelCapabilityCost/Model_Capability_Cost_Routing_Research_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/Claude_Independent_Model_Capability_Cost_Routing_Research_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/Claude_vs_OpenAI_Model_Research_Divergence_2026-10-28.md`
- `Docs/Research/ModelCapabilityCost/RESEARCH-CORRECTIONS-2026-10-28.md`

User's fixed-cost software portfolio:

- 1 x OpenAI software subscription: €20/month;
- 2 x Claude software subscriptions: €20/month each;
- 1 x OpenCode Go Europe: €10/month;
- total €70/month / €840/year at unchanged user-reported prices.

The user does **not** use APIs for this workflow. Do not optimize around API token prices and do not silently enable API keys, usage credits, overage, auto-recharge or pay-as-you-go routes.

Important operating observation from the user: **all subscription pools are being exhausted** under sustained work. In recent weeks OpenAI has repeatedly granted extra seven-day-limit resets, while Claude has not shown comparable resets. Treat these OpenAI resets as observed stochastic bonus capacity, never guaranteed entitlement.

Practical routing priors:

- **Astra:** hardest architecture/product decisions, ambiguous cross-system reasoning, difficult specs/G3/G4, deep synthesis, high-stakes review when appropriate.
- **Sol:** major Unity/C# implementation, difficult debugging, integration and cross-system work.
- **Terra:** bounded OpenAI implementation/fixes, continuation, mechanical repo/Git/docs and suitable computer/visual tasks.
- **Claude Sonnet/Opus where included:** major productive implementation, long-running work, difficult reasoning/research and alternate-provider review/continuity. The two Claude accounts are productive capacity, not merely reserves.
- **OpenCode Go / DeepSeek / MiMo:** high-volume bounded implementation, static inspection, docs/checks, watchers and low-risk repairs where verified.

Avoid the simplistic policy `OpenAI writes -> Claude checks`. OpenAI and Claude should both be primary production capacity. Consequential review should preferably use a different provider/model family from the author when practical.

Historical S1-C MiMo route is corrected to `opencode-go/mimo-v2.6-pro`.

Current Anthropic guidance indicates Fable on Pro may consume usage credits rather than included quota. Do not dispatch Fable in this fixed-cost workflow unless the user explicitly authorises the billing path or verified account state proves it is included without extra billing.

---

## T3 Fork work is a separate cross-project lane

A separate T3 Fork Overall Manager transition handoff already exists on CCF `main`:

`Docs/Handoffs/T3-Fork-Overall-Manager-Transition-20261028/HANDOFF.md`

Do not conflate that project with CCF Game Dev product authority.

The T3 Adaptive Model Routing V1 **design** is now APPROVED / REVIEW PASS / INTEGRATED. Claim #61 is closed DONE. Governing artifacts are:

- `specs/t3-adaptive-model-routing-v1/spec.md`
- `specs/t3-adaptive-model-routing-v1/plan.md`
- `specs/t3-adaptive-model-routing-v1/tasks.md`
- `specs/t3-adaptive-model-routing-v1/checklists/requirements.md`

Cross-repo implementation claim #63 is currently `[READY] ARM-010 — T3 Adaptive contracts and persistence skeleton`, targeting `jermur788/t3code`. It is not a Game Dev implementation task and must not take ownership of CCF gameplay/Unity files.

The approved T3 adaptive-routing principles are relevant to future Manager routing: track task class/difficulty, exact model/provider/account/options, quota/reset behavior, verification/rework and accepted outcomes; compare similar eligible tasks safely; keep recommendations advisory in V1; preserve one-writer and verification authority.

---

## Context / publication state

The old project context snapshot `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` is stale relative to many accepted changes. Do not call older attachments current.

The older queued context packet:

`Docs/Handoffs/ContextRefresh-20261027-ApprovedSpecs/TASK-PACKET.md`

is stale and should be inspected/amended/superseded before execution. Prefer a stable milestone point for a full context regeneration unless stale platform context is actively causing execution risk.

Git is canonical; Drive is a one-way read-only mirror. Generated composites/mirrors must come from one clean context SHA and must not be hand-maintained. `active-tasks.md` is excluded from the snapshot and read live.

Dashboard publisher was not available in the previous Manager environment. Never manually edit generated dashboard state.

---

## Evidence discipline

Use the existing evidence labels honestly:

- directly evidence-backed;
- model derivation;
- gameplay calibration;
- fallback-conservative default;
- unsupported-needs decision.

Do not manufacture empirical precision or present game abstractions as standard forestry practice.

Growth wording remains:

`EMPIRICAL SOURCE-DATA VALIDATION CONFIRMED; FINAL GROWTH-MODEL PREDICTIVE VALIDATION NOT DEMONSTRATED.`

GROWFOR-informed/benchmarked, not "based on" GROWFOR.

---

## Immediate priorities for the new CCF Overall Manager

1. **Take over #64:** mark RUNNING with actual Manager model/provider and current main SHA; reconcile this document against live repo/claims.
2. **Check #59 first:** it is the critical dependency for RACK-V2-08. If a handoff exists, review it immediately; if still RUNNING, do not create a competing writer.
3. **After #59 approval/integration, resume #49:** existing branch/worktree only; rebase/merge repaired main; rerun Combined HUD and proportionate preservation gates; review/integrate RACK-V2-08 when genuinely green.
4. **Refresh #39/#32/#19/#13:** determine whether any worker has started since this handoff; do not duplicate dispatches. Parallel static/read-only work is acceptable when ownership is isolated; Unity remains serialized.
5. **Keep Rack V2 on the milestone critical path.** After RACK-V2-08, inspect the spec/tasks and dispatch the next executable Rack V2 handoff rather than waiting for user prompting.
6. **Treat T3 fork #63 separately.** It may proceed cross-repo, but do not let T3 infrastructure work silently redefine CCF milestone scope or authority.
7. **Reconcile live coordination:** update stale status rows proportionately after verified handoffs/integration; preserve unrelated rows.
8. **No Windows build** until the required user playtest and explicit authorisation.

---

## User-facing status format

After substantial handoff/result/review/qualification/integration/user-test/status changes, report:

- what changed;
- what passed/failed;
- what it means;
- next step;
- exact user action required or none;
- `DASHBOARD SYNC: PASS / FAIL / PENDING / NOT AVAILABLE`;
- `Published dashboard reflects this task: YES / NO`.

Do not leave the user to discover files or infer routing. Every substantive dispatch should state the exact handoff, purpose, difficulty, best-suited model/provider, destination lane, copy/paste dispatch line and task claim.

---

## First message to paste into the new Manager chat

> Act as Overall Manager for the CCF Game Dev project. Start from the Project Instructions and `Docs/Handoffs/CCF-Overall-Manager-Transition-20261028/HANDOFF.md` on `jermur788/CCF` main. Mark continuation claim #64 RUNNING with your model/provider and current main SHA, then reconcile current `decision-log.md`, `current-milestone.md`, `game-brief.md`, live `active-tasks.md` and open claims before acting. Prioritise the RACK-V2-08 dependency chain: inspect #59 first, then resume/review #49 only after the layout repair is approved and integrated. Refresh #39, #32, #19 and #13 without duplicating work. Keep T3 Fork #63 as a separate cross-repo infrastructure lane. Preserve one-writer ownership, the machine-wide Unity lock, approved spec authority, frozen Reference Future v1 and the no-Windows-build-before-user-playtest rule. Continue automatically through already-authorised gates and stop only for a genuine product/scope/safety decision.

## Handoff completion condition

The new Manager should close #64 as `[HANDED-OFF]` only after it has:

- read this handoff;
- refreshed current CCF main;
- inspected live coordination;
- refreshed the named open claims;
- recorded any material divergence from this snapshot;
- explicitly accepted Overall Manager ownership for the continuation.
