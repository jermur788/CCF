# #243 — Safe release of the completed onboarding Unity lease

**Date:** 2026-11-03. **Purpose:** recover the machine-wide serialized Unity slot after the finished #243 introductory-visual qualification. This is operational cleanup, **not** another Unity qualification.

**Claim:** [#243](https://github.com/jermur788/CCF/issues/243), still OPEN solely for lock cleanup.  
**Difficulty:** **Low** if original lease owner can perform the verified release; **STOP / escalate** on any identity mismatch.  
**Best-suited provider:** the **same local Astra-243 lane/owner that acquired the lease**, under local T3 Code / deployed CCF-Ops; a different reviewer or Browser Manager is **not** authorised to impersonate this owner.  
**Exact lane/worktree:** `/media/jer/ZX20/CCF-Intro-Astra-20261103`; worker verification branch `verify/scenario-one-intro-astra-20261103`. Feature source `feature/scenario-one-intro-astra-visual-refresh-20261103` remains protected; no local file edits required.  
**Lease identity recorded by the implementation handoff:** `7256c06f75c447208806e2414e604761`, owner `Astra-243`. Read live metadata before acting; historical data is not a free-slot receipt.

## Procedure and guard

1. Original lease-owning process/agent refreshes live `Docs/Project/coordination/active-tasks.md`, #243, and the deployed **`/media/jer/ZX20/CCF-Ops/scripts/unity-lock`** (do not use older repository copy). Check the actual `status` and lock metadata, owner and lease ID against the exact existing receipt.
2. Verify no active Unity Editor/Xephyr/child display processes in this lane and no simultaneous user playtest or second worker holding the machine lock. If holder differs, ID mismatches, processes remain, lease has been reissued, or status is ambiguous: **STOP**. Do not force-release, kill processes, clean worktrees, modify host saves or delete files.
3. **Only the exact owner** runs the deployed lock's documented normal release for its own matching lease, then rereads status to confirm **FREE**. Capture receipt: check time, lease owner/ID, command and output, Editor/process status, clean branch/worktree, and current disk-free bytes. Post to #243 (no secrets).
4. No Unity run or new test. A separate review found ~28 GB free, above the 15 GiB floor; that observation is historical, so record a fresh disk check if relevant. A later Unity admission still needs independent current preflight.
5. Return success token `#243 UNITY LEASE RELEASED — READY FOR MANAGER CLOSURE` only if the normal matching release and post-check succeeded. Browser Overall then closes #243. No change to published playtest branch, accepted staging ref or baseline.

**User action:** route this packet to the original lease-holding local execution session/agent; if it cannot resume, tell Overall Manager the exact block rather than assuming the lock is free.

**Copy/paste dispatch:**

> Original Astra-243 Unity lease owner: read https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/UNITY-LEASE-RELEASE-PACKET.md and #243. Check the deployed CCF-Ops lock owner and lease against your recorded acquisition, verify no Editor/display processes, release **only your own exact lease** normally, capture a fresh FREE receipt and post it on #243. Do not run Unity, force-unlock, kill processes, delete files, or modify production. Stop and report any mismatch.

**Hard limits:** Personal playtest in progress; no Windows/Player build. #244 waiver accepted without changing the formal onboarding FAIL; #245 staged visuals separately from the active player-test branch. #194 NO ACTION.