# PLAYER-TEST-BROWSER-LANE-01 — Browser/Manager execution handoff

## Identity

- **Parent orchestration:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Browser lane claim:** #105
- **Orchestrator:** DeepSeek agent under #91
- **Purpose:** advance every remaining task that can be completed honestly from the browser/GitHub/project-source lane without local Unity/C# execution.
- **Difficulty:** High — broad cross-spec review/coordination with save/version, UI, parity and MFM dependencies, but no local execution.
- **Default model:** Claude Sonnet 5.5 Medium.
- **Escalation:** Claude Opus 5.5 Low when complexity, ambiguity or reviewer-independence requires it; continue escalating only if necessary. For OpenAI work/cross-provider review use **OpenAI 6.1 Sol**.
- **Starting integration candidate observed by Manager:** `integration/scenario-one-player-test-ready-v1` at `aca74acacc565f72ab802177cf69ca6ebb54d637` on 2026-10-29. **Refresh before acting.**
- **No production ownership is granted by this lane.** `Docs/Project/coordination/active-tasks.md` remains the single-writer lock authority.

## Current reported programme state to verify, not assume

The completion handoff reports Rack V2 through V2-14 integrated; S1-D T-06, PARITY-06, PARITY-05 and S1-C T-03 integrated on the integration candidate; S1-C T-05 v22 qualified but not integrated; PARITY-07 and MFM-04/05 static-qualified with Unity residuals; S1-D T-03 blocked only by a RackLesson candidate/base mismatch. Treat those as leads until verified against branches, claims and handoffs.

GitHub evidence already confirms:

- integration branch `integration/scenario-one-player-test-ready-v1` exists at `aca74ac...`;
- #103 tracks S1-D T-03;
- #104 tracks S1-C T-05 v22;
- #91 is the active DeepSeek umbrella.

## Browser lane: authorised work

This lane may directly perform the following when ownership is clear:

1. **Repository and claim inspection**
   - refresh current integration branch/main, relevant feature branches, open claims and published handoffs;
   - reconcile chat/handoff claims against GitHub evidence;
   - identify stale task-list/status wording without treating it as implementation truth.

2. **Source/static independent review**
   - inspect diffs and source for already-implemented returns;
   - issue `APPROVED / NEEDS CHANGES / REJECTED` review verdicts where runtime/rendered evidence is not required for that review layer;
   - verify scope, protected-anchor intent, save/version sequencing, evidence labels and absence of unauthorised production changes.

3. **Immediate browser priorities**
   - **S1-C T-05 v22:** inspect the exact v22 lineage, Opus re-review result, stale `CurrentVersion == 21` pin inventory and the proposed relabel boundary. Produce a precise execution checklist for T3 Code. Do not edit C#/verification pins here.
   - **S1-D T-03:** inspect the RackLesson candidate/base mismatch (`7.995` vs accepted `8.00385`) and determine whether the evidence supports a bounded harness/anchor reconciliation rather than a production change. Publish the narrowest T3 Code reconciliation packet. Do not change the harness or accepted anchor in browser.
   - **PARITY-07:** source-review the 77/77 static result and state exact remaining Unity/rendered evidence needed; reject any unsupported player-facing claim before Unity work.
   - **MFM-04/05:** source-review deterministic-core and Crop Tree candidate implementation/static evidence, confirm spec boundaries and list exact Unity compile/runtime residuals before MFM-06.

4. **Subsequent planning/review work**
   - prepare just-in-time packets/claims for approved remaining S1-C T-04/T-06/T-07/T-08, S1-D T-04/T-05/T-08/T-09, PARITY-08/09, MFM-06…12 and final Tier-3 work;
   - review each T3 Code return before integration/next dispatch where the packet requires Manager/independent review;
   - perform final independent spec/checklist review when executable evidence exists and the reviewer is independent.

5. **Coordination**
   - after each meaningful return, comment on #91 with: subtask, child claim, lane, model/provider, branch/worktree, exact candidate SHA, status, handoff path, blocker/next action;
   - keep `active-tasks.md` accurate only if assigned integration authority and after refreshing it;
   - never create duplicate claims when an existing feature claim still matches.

## Browser lane: forbidden / hand back to T3 Code

Hand the task to T3 Code whenever any of the following is required:

- edit production C# or verification C# whose correctness requires local compile/execution;
- change `CurrentVersion`, save fields, validation wiring or version-pin assertions;
- local filesystem/worktree inspection not visible through GitHub;
- Unity import/compile/batchmode/runtime execution;
- rendered 720p/1080p capture or visual qualification;
- save/load/hash execution;
- serialized scene/prefab/material edits;
- integration merge/cherry-pick whose safety depends on local compile/Unity verification;
- Tier-2/Tier-3 executable qualification.

Browser review may define the exact bounded task and acceptance criteria, but must not convert source inspection into a false runtime claim.

## Model routing

DeepSeek chooses the least expensive capable worker while maintaining required independence:

1. **Claude Sonnet 5.5 Medium** — default source review, issue/claim triage, packet preparation, bounded static review.
2. **Claude Opus 5.5 Low** — escalate for difficult architecture/save/reconciliation reasoning or when Sonnet returns uncertainty/NEEDS ESCALATION.
3. Continue Claude escalation only if the problem remains unresolved and the higher level is materially justified.
4. **OpenAI 6.1 Sol** — use for cross-provider independent review, difficult repo/source reasoning, or when Claude implemented the change under review.

Do not count a child/helper from the implementing thread as independent review.

## Handoff discipline

Every substantive browser subtask must publish/update a GitHub Markdown handoff with:

- exact sources/branches/SHAs inspected;
- model/provider and independence relationship;
- findings and evidence;
- verdict;
- exact executable residual handed to T3 Code, if any;
- next child claim/packet when clear.

Suggested browser-lane summary path:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-RESULT.md`

Do not wait for user input when the next approved step is clear. Dispatch the next safe subtask through DeepSeek/#91.

## Stop conditions

Stop only the affected thread and return to DeepSeek/Manager if there is:

- a genuine product decision not resolved by accepted authority;
- an ownership conflict;
- evidence that a protected Reference/save/determinism anchor would need to move;
- a source/runtime mismatch that cannot be bounded without local execution;
- an attempt to weaken/rebaseline a gate merely to get green.

Independent work on other branches may continue.

## Completion token

The browser lane is complete only when no remaining browser-capable review/coordination task is blocking the player-test candidate and all executable residuals are clearly handed to T3 Code:

`PLAYER-TEST BROWSER LANE COMPLETE — EXECUTION RESIDUALS HANDED TO T3 CODE`

This is not player-test readiness and not user acceptance.

## Copy/paste dispatch

`Run PLAYER-TEST-BROWSER-LANE-01 from Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-HANDOFF.md under DeepSeek parent #91 and browser claim #105. Refresh the integration branch, #91, #103, #104, current child claims/handoffs and live active-tasks.md first. Default to Claude Sonnet 5.5 Medium; escalate to Claude Opus 5.5 Low when needed, then further only if justified; use OpenAI 6.1 Sol for cross-provider independent review. Work only on browser-safe inspection/review/coordination/static-source tasks. Hand all C#/save-version/Unity/rendered/integration execution to T3 Code claim #106 with an exact bounded packet. Keep #91 updated after every subtask so work continues automatically. No Windows build.`