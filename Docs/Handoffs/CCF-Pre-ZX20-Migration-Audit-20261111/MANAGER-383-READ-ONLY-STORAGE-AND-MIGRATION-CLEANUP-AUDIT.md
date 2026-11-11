# #383 — READ-ONLY pre-ZX20 migration project storage and cleanup audit

**Binding date:** 11 November 2026  
**Requested by:** CCF project owner  
**Overall Manager:** #200  
**Local worker:** one OpenAI Sol/Codex High / T3 Code session, local read-only audit  
**Task claim:** [#383](https://github.com/jermur788/CCF/issues/383) — READY / NOT RUNNING  
**Difficulty:** HIGH (multi-drive storage, multi-project history and shared dependencies; no implementation required)  
**Authorization:** user's supplied **Project Cleanup Audit — Pre-ZX20 Migration**. This authorizes *immediate read-only inventory, non-destructive verification and report publication*, **NOT** deletion, cleanup, migration, transfer or repair that alters any other project or user data.

## 0. Exact goal, interpretation and stop boundary

Produce a measured, actionable cleanup and migration manifest for project-owned storage **before ZX20 external SSD is used to migrate T3 Fork and dashboard development to the Windows PC**. Cover **CCF, its local execution/orchestration evidence, T3 Fork, dashboard/Overall-Dev and shared dependencies** wherever actual project ownership can be verified. The preceding speech-to-text reference to “ratings / hard rolls / partitions” is clarified by the user's explicit latest request: inspect **files, storage drives, mounts and partitions** rather than assuming a “five ratings” game feature or proposing a redesign of hard rules.

**Do not remove or change a single candidate file.** No `rm`, `mv`, `rsync` transfer, copy-to-migration-volume, archive creation, database mutation, destructive test, `git gc`, `git clean`, `git worktree prune`, reset, stash, checkout over a valued lane, force unmount, filesystem repair, source/build edits, credentials printing or operational cleanup. No Unity Editor launch or heavy Unity lease. **T3 Fork Integration HOLD remains binding.** A manifest classification is a proposal, not an approval to delete. Manager #200 and the user must separately authorize specific destructive actions *after this audit*.

## 1. Canonical discovery and read-only ownership preflight

Before inspecting disk trees, read at their latest authoritative Git refs or actual on-disk owners:

- CCF: `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, current `decision-log.md`, `current-milestone.md`, `game-brief.md`, `unity-project-overview.md`, `Docs/Project/coordination/active-tasks.md` **live from main**, current active task claims/handoffs/specs, project context generation manifest, relevant `Tools/Verification` instructions.
- T3 Fork and dashboard/Overall-Dev: **discover their actual project roots first** and read latest **Current State/Roadmap**, project instructions, **Coordination/Handoff Protocol**, **T3 Fork Integration HOLD**, project active ownership/locks, accepted implementation plans, orchestration ledger and evidence appendix, runtime records, dashboard storage/ingestion dependencies and Windows migration constraints. Report any unavailable document/drive explicitly rather than inventing current state. The preloaded CCF orchestration ledger files may be historical; **live repository/local records win** for current ownership.
- Read active GitHub issues and available handoffs for the personal CCF Editor test [#382](https://github.com/jermur788/CCF/issues/382), older U3 [#361](https://github.com/jermur788/CCF/issues/361), Manager #200, and any T3/dashboard agents. Do **not** infer a lock has expired merely from process absence; actual live owner/lease authority controls.

No broad scanning of unrelated private directories. Read only actual **project-owned** roots, mount metadata and needed immediate ancestors; avoid following symlinks into unrelated user or system trees. If one root belongs to a different project owner/active writer, metadata inspection only in a non-intrusive way; no content or file writes. Avoid reading any secret contents—record their existence, path class and required secure migration, not their values.

**Report-writing branch:** `docs/ccf-pre-zx20-storage-migration-audit-20261111-v1` (prepared from `main`). **Proposed local report-only worktree:** `/media/jer/Files/CCF/Operations/CCF-Pre-ZX20-Storage-Audit-Sol` — not created or owned merely because the path is named. The worker may create this isolated **new** docs-only lane after confirming location, disk space, absence of conflicts and `git status` / worktree ownership. Do not alter any pre-existing source/Unity/T3 Fork/dashboard worktree or lock. Mark #383 **[RUNNING]** with actual provider/model, branch, worktree and starting full commit when the report-writing lane is truly admitted; otherwise [BLOCKED] and publish the reason. The only persistent writes allowed are newly authored audit Markdown, CSV/TSV and sanitized command outputs within this report lane on its own branch.

## 2. Build the true storage map (discovery only)

Inspect all accessible project-owned **mounts and partitions**, including local internal volumes, `/media/jer/Files`, `/media/jer/ZX20`, other project-specific external drives, and any actual T3 Fork/dashboard storage. These are **candidate discovery names, not assertions they are presently mounted or writable**. Record mount source/device/UUID where available, filesystem type, total/free space, mount flags, whether a root is project-owned, and Linux-to-Windows portability concerns (case sensitivity, exFAT metadata/permissions, symlink/ACL support, long paths, Unity `.meta` GUIDs).

Suggested read-only observations (run only when suitable to actual platform and permissions): `findmnt`, `lsblk -f`, `df -hT`, `stat`, `du -x -B1`, `du --apparent-size -x -B1`, targeted `find` without following external symlinks, `git worktree list --porcelain`, `git status --porcelain=v2 --untracked-files=all`, `git log` / branch/remote/reflog inventory, non-invasive process `ps`/`lsof` where permissions allow. Prefer `GIT_OPTIONAL_LOCKS=0` for read-only Git metadata inspection. These are examples, not a mandate to trigger automount, mount, Unity import, backup jobs, access-control changes or long full-disk scans.

For each root record: absolute path, owning project, physical volume/device, mount state, approximate file count, directory and apparent sizes, allocated size, latest meaningful modification, linked/copy relationships, active writer/runtime use, version-control/back-up status, and Windows migration relevance. Identify all local **Git worktrees**, linked `.git` files/directories, the shared object store and any independent Git clones before classifying them. A worktree's checked-out data, associated Git objects and ignored Unity `Library` are different kinds of storage; do not mistake shared objects for duplicate independent copies.

If a drive is unavailable, classify roots on it as **UNOBSERVED / UNKNOWN SIZE**, not zero, missing or removable. If projects refer to off-machine storage/cloud backups, record evidence and access uncertainty, not assumed redundancy.

## 3. Candidate inventory by risk, not just file extension

Audit known classes across the actual project roots:

- **Unity**: per-worktree `Library`, `Temp`, `Logs`, `obj`, generated `.sln/.csproj`, shader/import caches, editor/player build folders, crash reports, screenshots, disposable test Projects; distinguish regenerated outputs from the **source Assets/Packages/ProjectSettings/`.meta`** and any ignored **licensed assets** that cannot be regenerated without vendor materials.
- **Git / workers**: stale/abandoned worktrees, duplicate clones, orphaned temporary checkouts, generated worktree `Library`, detached commit pointers, local-only branches, unpushed commits, reflog-only or unreachable-yet-valuable commits, active dirty/untracked/ignored user modifications, active writer lease metadata, unmerged feature work. Before proposing removal of any Git worktree, **prove reconstructibility** from preserved refs+objects+remote or an immutable archival bundle and name how to verify it; dirty and ignored original data must have a complete preservation plan. Never run `git worktree prune` to prove reconstructibility.
- **Verification and orchestration**: outputs under `Verification/`, engine/capture logs, saved test fixtures, fixture hashes, approved handoffs, task-packet snapshots, context composites, zipped bootstrap bundles, orchestration ledgers, run manifests and proofs of accepted or failed gates. Some large logs are **unique evidence** or sole proof of a model calibration; don't classify as safe cache simply because they are old or generated.
- **T3 Fork / dashboard**: current source, runtime records, databases, configuration and schemas, ingestion state, API/connector configurations, indexed/generated dashboard snapshots, local artifacts, worker scratch, environments/tooling caches, package caches, lock/lease files, project-specific migration prerequisites. **Databases, runtime ledgers and dashboard ingestion identity are protected** absent a separately approved migration protocol. Integration HOLD forbids T3 Fork integration activity.
- **Archives and duplicates**: same-content backups, exports, zip/tar snapshots, downloaded artifacts, large PDFs and media evidence. Use content hashes **selectively** where practical; distinguish exact duplicate bytes from semantically distinct generations, provenance, hardlinks, reflinks, symlinks, compressed archives and independent copies on other volumes. An archive may be the **only migration-safe recovery copy** despite duplicating source.
- **Secrets and credentials**: identify *presence and required secure backup treatment*, never open, paste, commit, hash into a public manifest if it reveals a secret, or transfer them. Report only sanitized path category and access constraints. No credential disclosure.

Also identify inter-project dependencies, such as shared Git object stores, Unity installations/licences, Node/Python/.NET environments, T3 Fork/dashboard databases and shared path references, and artifacts needed by a live CCF user playtest or future Windows project import. Mark any shared dependency **PROTECTED/UNCERTAIN** unless both owners and restore procedure are established.

## 4. Classify each finding with an exact, auditable manifest row

Every candidate item receives **one of exactly these classifications**:

1. **REGENERABLE — LOW RISK**: reconstruction source/process/tool version independently shown; exclude from minimal migration backup only if prerequisites are included.
2. **LIKELY OBSOLETE — USER APPROVAL REQUIRED**: no current dependency found, but deletion not authorised.
3. **ARCHIVE BEFORE REMOVAL**: may be old, but contains unique branch, source, log, historical proof, user work or operational metadata requiring verified backup and restore.
4. **PROTECTED — RETAIN**: active file/owner; canonical instruction/decision/handoff/spec/research/runtime/ledger; frozen Reference Future v1; Legacy40; user saves/Prefs; source and `.meta`; licensed assets; current T3 Fork/dashboard state; critical accepted calibration/verification witness.
5. **UNCERTAIN — INVESTIGATE**: ambiguous owner or dependency, unreachable/unmounted volume, partial provenance, pending verification, conflicting evidence.

Do not label anything “safe to delete” merely because it matches `Library` or `Temp`: verify no active Editor/import and whether contents are genuinely regenerable with retained source/licence and acceptable rebuild cost. Do not reclassify essential provenance as low-risk due to old age. No user-approved destructive action is implicit in this audit.

**Structured manifest** (CSV/TSV; use stable IDs, sanitized paths, UTF-8; no secrets). Minimum columns:

`id,project,volume_or_mount,absolute_path,kind,classification,logical_bytes,allocated_bytes,unique_reclaimable_bytes_min,unique_reclaimable_bytes_max,size_confidence,active_owner_or_process,git_worktree_branch_head_or_unpushed_state,backup_status,linked_shared_dependency,rebuild_or_archive_proof,proposed_action,reason,prerequisites,rollback_plan,migration_include,approval_required,evidence_reference`.

When a root contains many similar disposable items, a **scoped aggregate row** is acceptable only with an exact enumerated leaf path list or glob plus exclusions—no vague `all old files`. Note discovered files outside owned/project roots without scanning or disclosing unrelated content; simply label out of scope.

## 5. Space accounting and migration readiness

Produce **separate estimates for each physical volume** and a distinct estimate for **Windows migration transfer volume**:

- Actual measured **allocated bytes** vs logical/apparent bytes, source filesystem and free space.
- **Reclaimable space conservative / likely / upper** grouped by classification and confidence. Do not count protected or uncertain as available to free. Do not count the same inode/hardlink twice, include symlink targets again, claim a Git shared object store is removed when one linked worktree goes, or double-count a parent cache and its children.
- Reflinks, sparse files, filesystem compression and deduplication may prevent exact reclaim estimates; state where per-file allocated blocks **cannot prove exclusive reclaim**. For non-POSIX drives with weak inode data, prefer conservative uncertainty.
- Separate **cleanup savings on Linux volume X** from **bytes copied to ZX20/Windows**; removing a cache from one drive doesn't necessarily create space on the intended Windows destination.
- For migration, list **mandatory INCLUDE** (Git source/history including local-only commits and ignored unique/licensed dependencies; Unity project source/metadata; validated config/schema/runtime databases where authorised; canonical docs/ledgers/specs and current dashboard/T3 state; original evidence fixtures needing continuity; recoverable secure secrets procedure), **REGENERATE ON WINDOWS**, **ARCHIVE ELSEWHERE BEFORE TRANSFER**, and **EXCLUDE ONLY AFTER APPROVAL**.
- Provide a reversible **rollback/recovery checklist** for each planned destructive batch, with required external backup path/capacity, checksum/clone validation, source refs, owner sign-off, Windows import and path constraints, and a later post-migration parity/restore test. **Plan only**; never create backup/transfer now without separate approval.

## 6. Non-destructive validation and report requirements

Validation must be based on observed data and command output, not guesses. At minimum:

1. Mount list checked; owned roots discovered; sizes measured or reported as inaccessible.
2. Git worktrees mapped to shared/private object stores; current branch/HEAD/dirty/untracked/ignored/unpushed and local-only refs recorded; **reconstructibility proof** stated for any proposed worktree removal. Do not call source clean without local status.
3. Active writer/process/lease registry checked non-invasively. No assumption that an absent process waives another owner's lock.
4. Unique runtime/evidence, precious user saves/Prefs, licensed ignored assets, frozen Reference and T3/dashboard HOLD identified as protected even if large.
5. Cross-project dependencies mapped. Ownership uncertain paths held for investigation.
6. Per-volume estimates with explicit anti-double-counting methodology and confidence.
7. No destructive commands, simulated delete tests, filesystem cleanup, migration, database writes, new Unity run or credential disclosure.

**No Unity tests required** for a metadata-only audit, and **no destructive reconstruction test**. Read-only Git history/ref evidence and documented reproducible reconstruction procedure are sufficient at this stage. If safe reconstruction cannot be established, classify ARCHIVE or UNCERTAIN instead of pretending removable.

## 7. Required durable outputs, review and stop

**Exact primary GitHub Markdown report:**  
`Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md`.

**Structured evidence:** adjacent `SOL-383-CLEANUP-MANIFEST.tsv` or CSV, `SOL-383-MOUNT-AND-STORAGE-SUMMARY.md`, `SOL-383-PROTECTED-REGISTER.md`, plus sanitized/read-only scripts or outputs where helpful. Huge raw file lists should live outside the repo on already-approved project storage, with exact paths/checksums and GitHub summary; do not add giant binaries or sensitive inventories to GitHub. Report any access-limited project as UNKNOWN, not assumed reviewed.

The report must open with **measured recoverable space by volume** (range+confidence) and top **10 recommended safe-to-consider cleanup actions**, then an explicit **PROTECTED/KEEP** register, Windows transfer INCLUDE/EXCLUDE/REBUILD recommendation, exact action order and approvals, rollback instructions, inaccessible roots and tests performed. Include path-specific evidence for each recommendation. Label every proposed action **NOT EXECUTED — REQUIRES USER APPROVAL**.

At handoff: commit/push only report files on docs-only branch; keep #383 [HANDED-OFF] and close after report writer released (or mark [BLOCKED] if access prevents meaningful audit), notify Manager #200 with direct report URL. Manager checks findings and prepares a **separate itemized approval request**; no cleanup or migration begins automatically. **No extra model reviewer** or T3 Fork integration. Dashboard state/readback is a separate operational issue; do not touch dashboard production just to make the report appear synced.

**One paste-ready dispatch line:**

> Sol / T3 Code (Codex High): Execute the **read-only Pre-ZX20 Migration Cleanup Audit** for CCF, T3 Fork and dashboard storage under GitHub claim #383 using this binding packet: https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-READ-ONLY-STORAGE-AND-MIGRATION-CLEANUP-AUDIT.md . Inspect live ownership, canonical Current State/Roadmap, coordination/handoff protocol, T3 Fork Integration HOLD, actual drives/partitions, local Git worktrees/dirty/unpushed state, builds/caches/verification archives/runtime records and cross-project dependencies. Measure paths and space **without double counting**. Classify each candidate as regenerable-low-risk, likely obsolete, archive-before-removal, protected-retain, or uncertain. Publish the exact GitHub Markdown/TSV cleanup manifest, protected register, per-volume recoverable estimates, backup/migration INCLUDE/EXCLUDE and rollback plan. **Do not delete, move, reset, stash, transfer, migrate, run Unity, touch production databases or reveal credentials.** Stop after durable report, release only your report-writer ownership and return result to Manager #200. No other model review.
