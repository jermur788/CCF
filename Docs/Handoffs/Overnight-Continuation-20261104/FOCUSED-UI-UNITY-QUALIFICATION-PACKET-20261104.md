# Focused UI staging / Unity qualification — Manager packet — 2026-11-04

**Claim:** [#275](https://github.com/jermur788/CCF/issues/275), **READY / not RUNNING / Unity lock not acquired**.
**Purpose:** Qualify the small, independently reviewed Objective 1/help and Stand Map round-handle changes staged in [PR #273](https://github.com/jermur788/CCF/pull/273), while preserving the user's unchanged published personal-playtest candidate.

## Task class, skill and lane

- **PACKET ONLY** — a bounded verification/hygiene repair, not a new player workflow.
- **Difficulty: Medium–High.** Physical UI Toolkit input and rendered fit require Unity, one machine-wide lock, correct candidate ancestry, and strict evidence accounting; the large copied reports require independently auditable cleanup.
- **Best-suited provider:** local **T3 Code** with Unity/Linux access, GPT-6.1 Medium/High or Claude Sonnet 5.5 Medium according to available capacity; independent opposite-family source/evidence reviewer if any new production diff is introduced. Browser-only work is insufficient.
- **Exact initial lane:** `integration/scenario-one-overnight-ui-qualification-v1` at `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104`; reviewed staging head `d3ba4fce3994d5c59b51786c6671dd654791e7d1`, original user candidate `72b4efbd1c653fa2231f632a79030a83ff76f833`.
- The worker **must not** write into the user's `CCF-U1` worktree or reuse its warm Library. Use the existing staging worktree only if truly free and clean; otherwise stop for ownership allocation instead of stashing/cleaning.
- Allowed production changes: **none without a new Manager decision.** Evidence/hygiene-only changes may occur within the isolated staging lane, audited and tested at final exact head. No retarget/merge of divergent #261/#262/#264; no fast-forward of the playtest branch.

## Before touching source or Unity

1. Fetch live `Docs/Project/coordination/active-tasks.md` (single primary writer #200), review #203 machine leases and any local `RUNNING` claims. Explicitly secure a **new exclusive Unity lease**. A prior FREE receipt is insufficient.
2. Verify branch/worktree path, original base ancestry and current HEAD, `git status --short` clean, and no competing owner or process (`pgrep -a Unity` and related verification processes).
3. Verify actual disk headroom on intended volumes. A cold import must leave at least **15 GiB free after projected Library/verification usage**. If insufficient, mark #275 BLOCKED; do not delete unknown caches or borrow the user's Library.
4. Mark claim **[RUNNING]** with exact model/provider, branch, worktree, starting HEAD, lease owner and admission results only after checks succeed.

## Bounded work

**Phase A — source-only hygiene, optional before final Unity head:** The staged OBJECTIVE-COPY and RACK-HANDLE return Markdown files are respectively ~4,884 and ~3,889 lines of copied provenance/transcripts. Manager ruling: remove or compact the duplicated payload before final integration, but preserve permanent source-PR links, source-head metadata and independently archived #269 evidence. Make this a separately reviewable docs-only step. It changes the 13-original-path byte-identity inventory for the new head: report the new file count and preserve the original staging proof as historical evidence. **Do not modify source C# merely to make a test green.** Scope guards S10/S11 must remain intact.

**Phase B — exact-final-head Unity Tier-2 feature qualification:** Import and compile, zero new errors; run `Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs` in its documented Unity harness; inspect Objective 1 help/Tree Marks on 1280×720 and 1920×1080 where feasible; physically create and move racks and drag the **round handle** to freely rotate with Shift released/held, confirming no snap during reshape while Create/Move retain their prior Shift behaviour; compare ghost, committed geometry, selection and repeat after reopen where valid; check Full Stand Map vs minimap/trails using the same confirmed plan; verify no unintended rack-persistence or editing regression. Run the scoped offline check with explicit actual base to interpret S10/S11 correctly, and rerun the baseline checker; **FAIL 08 must still be recorded as a real, deliberate Objective 1 CTA defect.**

**Phase C — independent review and stop gate:** Source diff and evidence reviewed from the exact qualified head. Preserve error logs, screenshots and true input observations. Do not assert full pass if deliberate FAIL 08 persists; present feature-specific results and the known unresolved failure separately. If missing Unity capacity or scope creep, stop rather than substituting an offline-only verdict.

## Protection / non-goals

- No CTA implementation, pink/orange colour change, focus-policy change, new settings/preferences, new MFM topology, access-spine assets, Rack V2 save-schema change, Legacy40/Reference Future migration or anchor update.
- No Unity heavy work in parallel, no unrelated clean/reset/stash, no direct edits to #200 active-task file by the local worker.
- No Windows/Player build. No claim of user acceptance or change to current playtest save/branch.

## Deliverable, review and dispatch

- **Exact GitHub Markdown handoff:** `Docs/Handoffs/Overnight-Continuation-20261104/FOCUSED-UI-UNITY-QUALIFICATION-HANDOFF.md` on a pushed isolated qualification branch/PR. Include command/log evidence, Unity screenshots, compile/input/rotation observations, source/hygiene ancestry, exceptions, residual FAIL, lease release receipt, and integration recommendation `APPROVED / NEEDS CHANGES / REJECTED`.
- At handoff, mark #275 `[HANDED-OFF]` and send Manager #200 the GitHub link. Manager alone makes the production integration decision and closes the claim.
- **Paste-ready dispatch:** `Execute CCF #275 focused Unity qualification from https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Overnight-Continuation-20261104/FOCUSED-UI-UNITY-QUALIFICATION-PACKET-20261104.md ; first obtain the live #200 Unity lease and verify exact worktree, process and 15 GiB post-import disk floor. Keep the published user-playtest branch unchanged. Return the specified GitHub Markdown handoff; no Windows build.`
- **User action now:** none to prepare the task; local orchestration must obtain exclusive Unity admission first. User still owes personal playtest before eventual Windows authorisation.
