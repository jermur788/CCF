# Task packet — Rack V2 editing-gate free-handle expectation repair — 2026-11-04

**Claim:** [#277](https://github.com/jermur788/CCF/issues/277) — READY / NOT RUNNING.
**Manager review:** [MANAGER-FOCUSED-UI-QUALIFICATION-REVIEW-20261104.md](MANAGER-FOCUSED-UI-QUALIFICATION-REVIEW-20261104.md).
**Source:** [#275 Unity qualification](https://github.com/jermur788/CCF/pull/276), stacked over [#273 staging](https://github.com/jermur788/CCF/pull/273).
**Task classification:** PACKET ONLY (bounded verification repair). No new feature spec or product decision.

## Purpose and difficulty

Fix exactly the *committed test expectation*, not working production code. The existing `RackV2EditingVerification.cs` expects rotating by the round handle 1.2° off level to snap horizontal, but #256's approved behaviour and #275's real-pointer verification show the handle **never snaps**, with or without Shift. A later Undo test then incorrectly expects a level rack, so repairing only the first assertion is insufficient.

**Difficulty: Medium.** One C# real-pointer Unity test with dependent undo/Shift state; bounded to one file but needs machine-locked input verification and independent guard against loosening assertions.

**Best-suited:** local T3 Code `gpt-6.1-sol` Medium, with opposite-family Claude Sonnet 5.5 Medium independent review. Routing may substitute by observed capacity/availability, recording reasons on #277. One primary writer and registered Unity lane.

## Exact lane/branch / start identity

- Starting repository head: `55b7e5b6499af6c268792a6641ad3c97e502a50d` (`qual/scenario-one-overnight-ui-20261104-v1`, current PR #276 qualification docs head). The actual Unity-tested predecessor is `d5fdf1c941065e9c543a090fe8039cafdd97729e`; the final handoff commit is docs only.
- Proposed fix branch: `fix/rack-v2-editing-verification-free-rotation-20261104`, created from that exact start head, **never** from divergent `main`.
- Existing registered Unity lane: `/media/jer/ZX20/CCF-UIQualification-20261104`, **only if** the #275 owner has released it, it is clean and available, and the deployed lane registry allows the proposed branch. If not, stop and ask #200/#203 for a new lane registration. No user `CCF-U1` Library or separate `/media/jer/Files/CCF-Primary` worktree edits.
- Proposed PR base: `qual/scenario-one-overnight-ui-20261104-v1` (PR #276), preserving the stack; no direct PR onto main or published test branch. Do not self-merge.

## Preflight (binding)

1. Refresh live `Docs/Project/coordination/active-tasks.md`, issue #277, exact repository refs, owner and worktree. Do not infer present lock freedom from #275's release.
2. Verify clean worktree, branch/HEAD/ancestry and no unrelated dirty files. No stash, reset/clean, discard or overwrite of other ownership.
3. Check current Unity/Editor/processes, free disk and projected post-run >=15 GiB. Obtain new exclusive deployed CCF-Ops Unity lease with this task ID, correct registered lane and provider/owner.
4. Only after successful preflight mark #277 [RUNNING] with actual model/provider, branch, worktree, starting HEAD and lock details; if failed, [BLOCKED] with reason. No Windows build.

## Allowed changes / acceptance

**Writable:** only `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` in the Part-B real-pointer rotation sequence and newly authored `Docs/Handoffs/Overnight-Continuation-20261104/RACK-V2-EDITING-GATE-REPAIR-HANDOFF.md` on the fix branch. No production/geometry/save/schema/asset/meta/package/scene/Reference Future edits. External evidence logs may be staged outside Git.

- Replace the prior positive `Near(Far.Z, Entrance.Z)` after a **round-handle** 1.2° rotation without Shift with a **positive unsnapped-angle check**, fixed entrance, matched last ghost/committed rack and no misleading snap status. Validate both 1280×720 and 1920×1080; use defensible input tolerances from #275 recorded observations rather than forcing exact manufactured angles.
- Keep large-angle rotation/identity and exact undo, but make the undo sequence test the *actual prior state*. An explicit undo to original level before another experiment is acceptable if it has asserted the previous free rotation; do not make the test pass by hiding a changed state.
- With **real Shift held**, round-handle rotation should still be unsnapped: a positive PASS/FAIL assertion is preferred over the old conditional `Note(... UNVERIFIED)`. Preserve documented limitation if real key input fails; do not print PASS on missing verification.
- **Create** retains soft snap without Shift and free draw with Shift; **Move** retains headland and length/angle behaviour regardless of Shift. Retain all original Part-A facade, refusal/save byte identity, overlap, one-level undo, edit-history and confirmed/locked assertions unaltered.
- Keep draw/move/reshape preview vs committed geometry strong, not waived. Retain existing original gate name and pass marker, increment assertions only when supported. No weakening of S10/S11, CTA static checker or D-064 OPEN-2.
- Source diff must show only the authorised test region and new handoff. If resolving it requires production edits or user-facing policy, **stop / return NEEDS CHANGES**.

## Verification and handoff

- At final exact fix head, use the **committed** `RackV2EditingVerification` harness and actual native OS pointer/Shift input at both sizes (1280×720 and 1920×1080), not only #275's disposable derived harness. Capture stdout, exit code, fail/pass token, raw screenshots and angle/undo evidence.
- Run smallest pertinent static verification and `git diff --check`; compare against #275 physical gate results. No repeat of full Tier-3 or unrelated Objective 1 end-to-end unless the test change affects it.
- Request an independent reviewer of the exact test diff; ensure test strength was not reduced. Keep #275's known deliberate failures in the return report.
- Release owned Unity lease normally; preserve machine cleanliness. Provide full GitHub Markdown handoff at `Docs/Handoffs/Overnight-Continuation-20261104/RACK-V2-EDITING-GATE-REPAIR-HANDOFF.md` and a stacked PR on the task branch. Mark issue [HANDED-OFF] and return Manager review request; do not merge.

**Paste-ready dispatch**

```
Execute CCF #277 — committed Rack V2 editing-gate free-rotation expectation repair — from https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Overnight-Continuation-20261104/RACK-V2-EDITING-GATE-REPAIR-PACKET-20261104.md . Start only from the specified #276 qualification head in the registered Unity lane with clean worktree, refreshed #200 ownership and fresh exclusive Unity lock. Change only the committed test's handle-rotation/undo/Shift assertions, do not weaken gates or edit production code. Run the committed native-pointer editing gate at 720p/1080p, obtain independent review, and return the exact GitHub Markdown handoff. Keep the published personal-playtest branch untouched; no Windows build.
```

**User action required now:** dispatch the text above to a qualified local T3 Code worker. No further product/design decision required for this bounded verification repair. 
