# Multi #730 / #791 — Manager verdict: choose Files ext4 for Year2–10 source lane

**19 November 2026 UTC — #791 custody evidence APPROVED; new source lease NOT YET CREATED; #725 transfer permission remains pending.**

## Manager decision

Manager reviewed the [complete #791 native admission result](https://github.com/jermur788/CCF/blob/docs/multi-791-year2-10-custody-admission-result-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/791-orchestration/Multi-791-YEAR2-10-PHYSICAL-SOURCE-CUSTODY-AND-OFFLINE-ADMISSION-RESULT-20261119.md). Live ZX20 uses stable UUID/by-id identity, live storage check and lock status PASS; the overnight mount discrepancy came from a stale anchor copy, not a live Ops fault. Original #725 clean and immutable under #735 through 27 Nov 18:00Z. Videos ext4 physically suitable but rejected by *current* Ops lane config because it is outside ZX20/Files approved roots; no bypass occurred. Files ext4 38 GiB free last measured; **must be refreshed**. No source read, copy, .NET or lease yet.

**DECISION: Use a NEW disjoint lane on /media/jer/Files within the live, validated CCF Files source root for this bounded Year2–10 .NET implementation.** Do not modify the shared Ops tool to add Videos now. This is an explicitly **temporary local-development placement** ahead of the intended desktop migration; user prohibited new development on the root filesystem, not on Files. Never reuse/overwrite #725's retained original or #784's existing Core/Viewer lanes. Do not create any new lane until live Ops physical custodian has registered a sole-writer reservation with canonical shared flock/lint/readback. Files capacity is adequate for modest local .NET work only if current free-space and per-process cache/output are actively managed; stop before filesystem pressure or RAM pressure. Videos remains unused for source and may be considered in a separate future Ops change if needed; not a reason to block this task.

**G0 prior owner approval:** the exact corrected four-file spec and normal authenticated agent posture Option A for this .NET engine are already APPROVED in [G0 manager packet](https://github.com/jermur788/CCF/blob/docs/multi-730-789-g0-approved-791-custody-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-789-G0-APPROVAL-AND-791-CUSTODY-RESERVATION-HANDOFF-20261119.md). Do not ask again for these.

## Remaining source-owner signature

The verified #725 source custodian is **owner jermur788 through #735 native Codex custody**, not the old #725 source writer's expired lease. Retention alone is not authorisation to read/export. The user's direct authorisation to *coordinate* source-custody grants does not automatically transfer the retained protected bytes. The following narrow permission is still required from that holder:

> I authorise one read-only export of the immutable qualified #725 Year1 source into a newly reserved independent Year2–10 Files lane, limited to the enumerated B-01 source-closure paths and one pinned 32,567-byte fixture, with per-file SHA-256 receipts and no modification to the original. I authorise creating the exclusive new Files lane only after the live Ops custodian validates, books and reads back the lease.

This **does not** authorise new public code, broad access to #710 or #687, or any source edits before distinct B-01/C grants.

## Scoped continuation for physical custodian — one visit

**Claim:** [#791](https://github.com/jermur788/CCF/issues/791) remains OPEN; same issue, physical custodian re-registers actual agent/session/provider as [RUNNING — FILES LEASE / NO SOURCE WRITES] before touching Ops. **Best actor:** fresh normal-auth GPT-6.1 Sol / native Codex T3 custody worker. Not the coding worker; one primary Ops writer. **Difficulty MEDIUM:** safe Files root lease/board check, protected source handoff receipt, single heavy slot. No competing claim for this same work.

1. Re-read live board/lanes, stable UUID, Files root approved validator, active-tasks.md, existing #725/#784 source lanes, locked original source and other active physical owners. Verify fresh free space and MemAvailable/PSI, no heavy render/test contention. Do not trust overnight measurements blindly.
2. Obtain/verify holder's exact narrow export approval above, and source closure list from the #791 result. If absent, return **BLOCKED solely on holder signature**; do not read source bytes or create a writer worktree on assumption.
3. As actual authorised physical Ops custodian, under required nonblocking shared exclusive flock, register an isolated NEW lane on Files under the already permitted CCF source root, with exact new branch/HEAD, named future GPT-6.1 Sol writer, task claim, lease and allowed directories, not overlapping original #725 or viewer lanes. Use the documented atomic board/lane transaction, full lint and readback. No custom tool changes. If lane must be created by a separate custodian tool/step, follow the existing procedure, not a shortcut.
4. Only after verified holder scope AND physical lease exist: perform exact permitted **read-only export** from original #725 into new lane, preserving original immutable and pinned fixture/golden; per-file hashes, fixture SHA and trust receipt. Source roots/actual .NET closure must be established from allowed B-01 read map. Do not run dotnet or write implementation code.
5. Return one durable result including physical lease, clone/checkout state, exact source paths read/copied, hashes, RAM and storage receipt, exact proposed B-01/C and offline .NET/Q scopes from the #791 result, and [HANDED-OFF]. Manager then signs phase-specific development and testing commands on inspected evidence, and only then returns #789 to implementation.

**Proposed Result:** Docs/Handoffs/MultiRack-Simulation-20261116/791-orchestration/Multi-791-FILES-SOURCE-LEASE-AND-YEAR1-NARROW-TRANSFER-RESULT-20261119.md, on a NEW exclusive docs-only branch from refreshed main (branch registered at worker start). The earlier #791 report remains immutable. **No new root worktree, clone, cache, bin/obj, NuGet, temp or log.**

## Pending Manager source and offline testing gates

The #791 suggested B-01 inventory seed list, six-file C update scope plus three .csproj conditional changes, and offline unshare/isolated cache/Year1 golden+refusals/independent Q tests are **proposals**, not signed executable grants, until the specific original holder's permission, physical lease and source map exist. Manager will issue exact B-01 and C and separate offline tests/reviewer/Q only on true receipt. Do not use #784's B hash, #732's prior .NET grant or #725 retention as substitute.

**Implementation after admission:** #789 one outcome handoff; M-1 dead CI target/neighbour and mortality/boundary filters before 1-to-2, mutants R-31/T-19 and R-32/T-20 and unchanged 45,980-byte Year1 output, independent changed-lines PASS, then ten sequential no-intervention years with independent offline Q. Strict L4 FAIL38 remains unwaived. No Scenario One/#770, #784 viewer, Unity, Host, public app, main merge, release, Windows build or GPT-6.2. One heavy process at a time on laptop due previous memory lockup. Dashboard/Drive sync NOT VERIFIED.

**Copy/paste dispatch to physical custodian:**
Continue Multi #791 under Manager decision to use Files ext4 instead of extending Ops tools for Videos. Read the full handoff at this GitHub path, register a fresh native GPT-6.1 Sol/Codex physical Ops custodian and inspect live board/Files capacity/locks. Obtain the actual #725 holder's narrow read-only original source-and-fixture export grant. Then atomically book/lint/read back a new disjoint Files sole-writer lane and export only the approved Year1 closure with hashes, leaving original immutable. No source edits, dotnet, root development, Scenario One/Viewer overlap or GPT-6.2. Return one durable evidence receipt and exact B-01/C/offline-Q scopes to Manager, or one true blocker.
