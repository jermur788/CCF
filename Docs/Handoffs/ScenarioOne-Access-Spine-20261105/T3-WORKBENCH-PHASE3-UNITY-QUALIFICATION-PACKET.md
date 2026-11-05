# #287 — T3 workbench retirement: exact-candidate Phase-3 Unity qualification packet

**Status:** **WAITING — NOT RUNNING. No Unity until explicit user-playtest release and a valid exclusive lease.**  
**Parent:** [#284 D-070 workbench implementation](https://github.com/jermur788/CCF/issues/284), [source/offline Manager review](MANAGER-WORKBENCH-RETIREMENT-REVIEW-20261105.md), [worker source handoff](https://github.com/jermur788/CCF/blob/feature/scenario-one-workbench-retirement-20261105/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-RETIREMENT-IMPLEMENTATION-HANDOFF.md).  
**Execution claim:** [#287](https://github.com/jermur788/CCF/issues/287).  
**Classification:** PACKET ONLY — focused verification/test repair of a source-reviewed bounded implementation, not a new player feature or schema policy.  
**Difficulty:** MEDIUM–HIGH because one buildable's new-world suppression affects Unity init order, save/references, MFM Site masks, deterministic world hashes and reference previews.  
**Best local worker:** returning T3 Code **GPT-6.1 Sol Medium** (or similarly capable local C#/Unity worker if capacity requires); Claude Sonnet 5.5 Medium has already independently reviewed the source candidate. Actual provider quotas/availability are unknown until preflight.  
**Exact branch:** `feature/scenario-one-workbench-retirement-20261105`.  
**Exact isolated lane:** `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` (existing *worker-reported self-contained clone*, not a verified clean or leased worktree at this time).  
**Handed-off candidate:** `8b1946dcec00587971d1f7c80004a47ebaa88515`, source implementation `57f10ff09a5e1b4e1e776ed7be2b0d46d6f7c411`, base `50042bbe97734b031210ddebb603b5ec00ff459d`. Refresh remote and local HEAD before any edits; if different, STOP and return to Manager for reconciliation.  
**Primary writer:** #287 may continue #284's workbench-only source lane once #284 closes; no other simultaneous writer on that branch/clone. **#200 is the sole writer of** `Docs/Project/coordination/active-tasks.md`. No edits to the user's active main integration or shared Primary checkout.

## 0. Hard admission conditions (must all pass)

1. **User explicitly states that personal Unity playtest has finished and releases machine occupancy.** Silence, a reported lack of Editor process or a prior `free` receipt is not release. Do not touch the user's save data or current main integration lane.
2. Diagnose the **actual mount identity** for the intended ZX20 clone: `lanes.json` records `/dev/sda2` while the worker observed `/dev/sdc2` for the same reported exfat UUID. Read `findmnt`/`lsblk -f` and the lock service's ownership/lease records; do not change a device string merely to silence its guard. If a safe configuration repair requires authority/tool not assigned to #287, STOP and request a separate Manager-approved lease-configuration maintenance packet. Never force release, fabricate a lock file, kill Unity or reuse another owner's lease.
3. Recheck disk safety floor, Editor/Hub/processes, lock records, current user session, branch, HEAD, dirty/untracked files and worktree/clone ownership. Preserve all unrelated work; no stash, reset, clean or cherry-pick to hide it.
4. Acquire the **genuine exclusive Unity machine-wide lease** as the actual #287 owner, observe tool receipt, then start Editor only in the isolated clone. Release only the matching lease after no owned Editor remains. No Windows/Player build.
5. Mark #287 **[RUNNING]** only at actual work start, recording model/provider, lane/branch and starting HEAD. A blocked gate remains [WAITING]/[BLOCKED], not qualified.

## 1. Allowed phases and ownership

**Phase 0 (possible while waiting, non-Unity/read-only):** read current branch/source, identify exact runner commands and baseline evidence, prepare assertion plan; no changed source/scene and no claim of runtime completion. Do not run a competing Unity or manipulate lock. Existing source-review verdict suffices; do not re-open architecture or broader access-spine work.

**Phase 1 (after user/lease gates):** a focused set of new or adjusted verification assertions, without weakening existing checks:
- New Enlarged80 game: exactly one policy-retired `workbench-01` scene component; no visible/colliding/interactive site; `CaptureData().buildables` **omits** that ID; remaining six Site masks match the independent **nonretired buildable** inventory; no change to path/headland/clearing masks.
- Existing save containing `workbench-01`: load safely via `CanLoad` / `LoadData`; `IsRetiredFromStartingWorld == false`, historical built state preserved; no duplicate or lost IDs; old scene prerequisites remain intact.
- Reference Future v1 preview then exit back to a new-game world: reference's workbench restores while previewing; original new-world workbench is retired again, no leaked collider, Site mask or transient save difference; frozen archive integrity and continuation contract unchanged.
- Fresh new-game save round trip with workbench omitted: reloaded runtime remains retired, its Site mask remains absent; unchanged trees/stems and work orders except the explicitly labelled new-world snapshot identity.
- Negative case: pre-buildables/empty list save retires the workbench, as documented. Do not invent old-site restoration where the save contains no ID; disclose any real historical compatibility mismatch rather than silently migrate.

**Phase 2 (verification oracle alignment):** `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` currently shares `IsRetiredFromStartingWorld` as its filter; add an **independent expected-site inventory assertion** using the intended one scene-retired policy, and independently verify new-world six vs historical seven masks rather than merely comparing two identical production filters. Inspect `Tools/Verification/StandGeometry/Enlarged80Verification.cs` at the two old exclusion loops; if the gate remains in use, bring its oracle into parity with the policy, retaining/no weakening of all existing assertions. Small focused verifier-only edits permitted within #287; production changes require Manager review if they alter contract or enlarge scope.

**Phase 3 (exact candidate behaviour and regression):**
- Unity Editor import/compile in isolated lane; record project/editor version, run start time, errors and exact candidate at execution.
- Verify new Enlarged80 tree count, identities, positions, omissions, and **CurrentWorldHash** at start and Year 3. Old expected labels from #284 are historical and must not be silently replaced. If any tree moved or hash delta exceeds the one intentionally omitted buildable entry, STOP with evidence.
- Re-run MFM-08A Site/Track/Headland/Clearing parity and a narrow Rack V2/Objective 1/Work Plan non-regression, with no new rack topology. Verify old v24 and available older saved worlds, Legacy40, frozen Reference Future v1 archive bytes and preview/continuation behaviour.
- Capture representative 1280×720 and 1920×1080 images if the focused gate uses rendered/UI assertions; inspect physically hidden workbench presentation and no ghost selection prompt; do not substitute source/PNG/Editor play-mode suspicion for actual native input tests.
- Report every assertion as PASS/FAIL/NOT RUN; do not narrow test targets to hide failures or waive a protected gate.

**Tier:** focused **Tier 2 FEATURE QUALIFICATION**, not a full milestone Tier 3. A future integrated milestone candidate may require Tier 3 under the main programme. No Windows build.

## 2. Allowed writes

Within isolated qualification branch only: the existing seven workbench source/scene/verification paths **only if required for bounded fixes**; prefer verifier-only changes under `Tools/Verification/MfmCoverageWorld/` and `Tools/Verification/StandGeometry/`. Any actual source semantic correction needs a fresh exact diff, extra source review and explicit Manager disposition before integration. Write the required tracked qualification handoff and record exact external screenshots/log paths. No edits in `Docs/Project/decision-log.md`, approved Rack V2/MFM specs, `ReferenceFuture` JSON, other scenes/buildables/assets, `active-tasks.md`, production main, the current user's playtest branch/saves, packages, or Windows build settings.

Do not delete/reset untracked external test artifacts. Do not change the lock service or mount validation as an incidental test “fix”. If device mismatch prevents legitimate lease, return BLOCKED with verified diagnostics and ask Manager for separately authorised safe lease repair.

## 3. Acceptance and stop matrix

| Gate | Requirement | Fail handling |
|---|---|---|
| User session and lease | Explicit user release, fresh disk/process checks, real exclusive lock | **WAITING**; no Unity |
| Source | Exact candidate; only approved verifier correction or separately reviewed source fix | **NEEDS CHANGES** on unexpected production diff |
| Four workbench lifecycle assertions | New-game omission, old-save retention, reference preview enter/exit, new-game reload | **STOP** on any fail; preserve raw logs |
| New-world accounting | 1,344 expected tree identities from offline evidence, 0 changed omission slots, six Site masks, hashes **newly measured** | **STOP** on mismatch; do not rebaseline blindly |
| Historical compatibility | Old saved workbench, Legacy40 and frozen v1 hash/bytes preserved | **STOP** on integrity mismatch |
| Gameplay | No unrelated rack/spine/Work Plan change; actual hidden construction state | **STOP** on discovered regression |
| Renders | Required relevant 720p/1080p captures and observable correct state | Mark NOT RUN/FAIL if unavailable |
| Safety | No current playtest worktree, unrelated work, unapproved lock work or Windows build touched | Stop and report |

## 4. Durable return and workflow

**Required GitHub handoff:** `Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-RESULT.md` **on the isolated feature branch**. Include: provider/model, exact runtime candidate HEAD and base, lease preflight and release receipts, source/verification diffs, test invocations and full PASS/FAIL/NOT RUN counts, new/old labelled hashes, reference/Legacy40 and historical-save evidence, screenshot/log external paths, independent review status, residual risks and a clear integration recommendation. Never claim fully verified from an offline run. Mark claim [HANDED-OFF] and return to Manager #200; Manager alone determines APPROVED / NEEDS CHANGES / REJECTED, integration, updated Overview/current milestone if applicable and dashboard sync.

**Next user action:** after personal playtest is actually complete, explicitly say the Unity session is closed and the machine may be used for qualification. This **does not** authorise a Windows build.

**Paste-ready dispatch (for local T3 after release):**
> T3 Code GPT-6.1 Sol Medium — take [CCF #287](https://github.com/jermur788/CCF/issues/287) only after the user ends personal Unity testing and the unity-lock mount/lease gate is legitimately fixed. Read https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-PACKET.md. Qualify candidate `8b1946dcec00587971d1f7c80004a47ebaa88515` on isolated branch `feature/scenario-one-workbench-retirement-20261105` and local lane `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105`. Add the four mandatory workbench save/preview assertions and independent MFM Site/Enlarged80 verifier alignment; run focused Unity Tier 2 with actual exclusive lease; capture new start/year-3 hashes, old-save/Legacy40/Reference Future v1, relevant renders, and publish T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-RESULT.md to the feature branch. No main merge, no work on current player checkout, no Window/Player build. Return to Manager #200.
