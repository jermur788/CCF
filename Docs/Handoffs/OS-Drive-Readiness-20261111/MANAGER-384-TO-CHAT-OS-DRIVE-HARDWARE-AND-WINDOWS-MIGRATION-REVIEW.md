# ChatGPT handoff — operating-system hard drive / hardware and Windows migration readiness

**Status:** READY / NOT RUNNING — browser/GitHub **read-only review only**.  
**Date:** 11 November 2026.  
**Overall Manager:** [#200](https://github.com/jermur788/CCF/issues/200).  
**Review claim:** [#384](https://github.com/jermur788/CCF/issues/384).  
**Difficulty:** MEDIUM, because the question is primarily interpreting actual OS/drive evidence and a safe Windows/WSL migration design; current hardware and system state cannot be measured by ChatGPT.  
**Best suited:** **new ChatGPT chat with connected GitHub/browser**, not local Sol and not a separate paid Claude/Fable reviewer.  
**No execution rights:** no OS commands, disk cleanup, mount/partition/fs changes, database modifications, Windows setup, migration, backup copy or Unity.

## 1. User's intended focus

The user's latest request said to continue the “operating system hardway.” Interpret this within the live ZX20 storage conversation as the **operating-system hard drive, partition/storage hardware and Windows migration implications**, especially the critically full Linux internal root disk. If the user meant a different hardware subject, record that interpretation as provisional; do not invent new hardware facts. The separate local [#383 storage audit](https://github.com/jermur788/CCF/issues/383) is active and provides actual filesystem measurements.

**Goal for this Chat:** Produce an evidence-based OS-drive decision brief that explains **what fills the system disk, what can be investigated safely, how Linux/Windows/WSL storage should be arranged for T3 Fork and dashboard development**, and exactly what remains unknown or needs local evidence/user choice before cleanup/migration. This is **not** another full-disk census. Recommend safe *review steps*, not deletion.

## 2. Source authority and exact evidence to inspect

Start by reading and cross-checking:

1. [Sol #383 original measured inventory — PARTIAL](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md).
2. [Sol #383 actual mount/storage summary](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-MOUNT-AND-STORAGE-SUMMARY.md).
3. [Sol itemised candidate manifest](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-MANIFEST.tsv) and [protected register](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-PROTECTED-REGISTER.md).
4. [Manager #383 review](https://github.com/jermur788/CCF/blob/docs/manager-383-partial-audit-review-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-PARTIAL-MEASURED-AUDIT-DISPOSITION.md) and [targeted local follow-up](https://github.com/jermur788/CCF/blob/docs/manager-383-partial-audit-review-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-TARGETED-CANDIDATE-OWNERSHIP-AND-MIGRATION-CLOSURE.md).
5. **Live** [#383 claim](https://github.com/jermur788/CCF/issues/383) and CCF `Docs/Project/coordination/active-tasks.md` from `main` — at handoff creation **#383 was RUNNING**, not the older PARTIAL/NOT RUNNING snapshot in the Manager text. Check new #383 addenda before concluding.
6. Repository root `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, `Docs/Project/current-milestone.md`, `Docs/Operations/StorageArchitecture.md` if accessible on an authoritative Git ref, and T3 Fork's latest **Current State/Roadmap**, **Coordination/Handoff Protocol**, and explicit **Integration HOLD**. **Primary local `StorageArchitecture.md` is dirty**; do not confuse GitHub committed copy with its uncommitted current local edits.
7. Relevant existing T3 migration preflight / orchestration ledger (published and accessible sources only) and dashboard provenance/ingestion requirements. Distinguish historical preflight evidence from current active owners.

**Truth order:** user direct instructions and accepted current decisions → live project coordination/owner record → measured local worker evidence and published Git state → historical documents. ChatGPT's general Linux/Windows knowledge is **analysis**, not evidence that the user's hardware has a specific slot, filesystem feature, network speed, RAM, CPU, motherboard, Windows build or backup space.

## 3. Known operating-system / storage baseline

Latest operator measured at **2026-11-11 20:34 UTC** (`df -B1`, point in time):

| Mount | Device / fs | Total bytes | Used bytes | Available bytes |
|---|---|---:|---:|---:|
| Linux root `/` | `/dev/sda2` ext4 | 250,375,106,560 | 236,315,041,792 | **1,267,175,424** |
| ZX20 `/media/jer/ZX20` | `/dev/sdd2` exFAT | 512,075,235,328 | 482,487,566,336 | 29,587,668,992 |
| Files `/media/jer/Files` | `/dev/sdb4` ext4 | 498,654,666,752 | 267,268,935,680 | 205,980,205,056 |
| Videos `/media/jer/Videos` | `/dev/sdb1` ext4 | 467,298,177,024 | 381,404,110,848 | 62,081,433,600 |

Additional inspected partitions include an EFI system partition, swap and an unmounted FAT partition; their capacity does not automatically become free space on root. The discrepancy between total-used and available on ext4 may be reserved blocks/metadata; do not call it deletion potential without filesystem verification.

**Root is a pressing operational risk.** Only ~1.27 billion bytes were available, potentially insufficient for package installs, compilation, logging, browsers, T3 runtime writes, crash dumps or temporary files. **This is not an authorisation to delete anything or move active T3 data.** A fresh local `df` result is required before any current-status claim.

Known root-path candidates:

- `/home/jer/dev/t3code/node_modules`: **3,831,660,544 allocated bytes**, dependency directory; pnpm hardlink/store and offline reinstal feasibility not checked; T3 processes active.
- `/home/jer/.t3/userdata`: **5,842,542,592 allocated bytes**, **PROTECTED live runtime** with database + WAL/SHM, sessions, attachments, settings and identity; never label all of it cache.
- `/home/jer/.t3/userdata/logs`: 747,188,224 bytes, **nested within userdata**.
- `/home/jer/.t3/userdata/server-browser`: 200,589,312 bytes, **nested within userdata**; may contain live session/auth state.
- Other root directories, OS package/cache directories, swap/log growth, installed toolchains, containers/virtual machines and home video/archive storage **were not fully inventoried**. Do not assert any are actually present, disposable, or large.

Known ZX20 root candidates are several Unity `Library` caches ~7.0–7.3 billion allocated bytes each, but **U1, U3 and #382 personal qualifier are protected**, and two seemingly older candidate Libraries lack owner/rebuild verification. These are on **ZX20, not the internal OS disk**; deleting them would not fix root's 1.27 GB problem.

**Crucial:** free capacity, occupied bytes, unique reconstructible bytes, reclaimable bytes, approved-for-deletion bytes and Windows migration transfer bytes are **six different quantities**. Current approved cleanup = **0 bytes**; unique reclaimable across volumes = **UNKNOWN**.

## 4. Specific questions Chat must answer

**A. OS health and immediate non-destructive precautions.** From the observed root capacity, identify what operations should be paused (new package installs, large logs, compilation, imports, unbounded local scanning), and suggest a short evidence request for the existing *local* #383 worker to collect only if allowed by its claim and runtime ownership. Avoid uncontrolled recursive searches or extensive hashing on root.

**B. Disk/partition architecture diagram and limits.** Show only *known* partitions and mounts. Explain which are distinct filesystems, and why free space on Files/ZX20 does not translate into root filesystem capacity. Explain exFAT vs ext4/NTFS/WSL trade-offs **as analysis**, particularly symlinks, Unix executable permissions, Git worktrees, Unity metadata, case sensitivity, long paths, SQLite WAL consistency and file locking. Do not recommend repartitioning, formatting or installing Windows from assumptions.

**C. Capacity/transfer design options.** Compare at least:
- ZX20 as temporary transfer medium, with original Linux machine retaining source/backup until verified Windows import;
- Windows native + separate WSL ext4 development storage (if future user/hardware facts support it);
- preserving a Linux ext4 Git/Unity source working copy rather than treating exFAT as a full-fidelity POSIX clone.
Include unknowns: Windows PC disk free capacity, intended native-vs-WSL workflow, T3 runtime compatibility, destination backup drive and database quiescence procedure. Never conflate the exFAT source transfer path with a permanent WSL/Linux worktree without verification.

**D. Safe root-space candidate ranking.** From report evidence, separate:
1. live/protected runtime/data;
2. package/build caches with possible rebuild but unverified links/owners;
3. OS-level cache/log/snapshot hypotheses **not yet measured**;
4. unique source and active logs requiring retention.
State **what local evidence or owner sign-off** would be needed to elevate each into a reversible cleanup proposal. No automatic “delete Linux apt cache”, “remove logs”, “empty trash”, “vacuum journal”, “npm cache clean”, “pnpm store prune”, “remove node_modules”, “git gc” or `rm -rf Library` advice as an authorised action. If giving example read-only commands for a later owner packet, distinguish them from commands to run now.

**E. Decision table.** State whether current evidence supports:
- SAFE TO REVIEW (read-only, Chat);
- REQUIRES LOCAL MEASUREMENT (Sol #383 owner);
- REQUIRES T3/DASHBOARD OWNER APPROVAL;
- REQUIRES USER DECISION (future storage / Windows layout);
- PROHIBITED NOW (destructive or HOLD-related).
Do not propose a new local work claim or overlap #383. If more local measurement is necessary, prepare a *short requested-evidence amendment* that Manager #200 may pass to the existing #383 operator, not a new worker.

## 5. Write-up and handback

**Deliver exactly one GitHub Markdown:**
`Docs/Handoffs/OS-Drive-Readiness-20261111/CHAT-384-OS-DRIVE-HARDWARE-AND-WINDOWS-MIGRATION-REVIEW.md`.

Use a new docs-only review branch derived from `docs/manager-os-drive-hardware-review-chat-20261111-v1` or publish directly there via a safe GitHub API with expected-version checks. **No local worktree or machine access required**. Before publishing, check live #383 report ownership/branch and avoid the worker's report files and live active-tasks file; **Manager #200 owns coordination writes**. Chat #384 may publish only its own independent review file and comment to #384/#200. Do not edit canonical governance, decision log, report TSV, live T3/dashboard files or any production source.

Required report structure: **Executive recommendation; baseline observations vs assumptions; OS-drive risk ranking; mounted filesystem diagram; Windows/WSL and ZX20 migration options; root-space evidence gaps; non-destructive future measurements; user approvals/stop gates; exact links; final status**. Do not manufacture actual hardware specs, volume recoverability or destination capacity.

At completion, leave #384 [HANDED-OFF] and close after the GitHub report is published; send direct URL to Manager #200. Recommend one next action with owner and approval level. **Do not mark T3 HOLD lifted or #383 complete.** Dashboard ingestion/sync remains unverified until observed. No independent paid model review.

## One paste-ready message for a new ChatGPT chat

> ChatGPT / Game Dev Manager-support (browser/GitHub lane): Please review the **operating-system hard drive, partitions and Windows migration hardware/storage plan** using this durable #384 handoff: https://github.com/jermur788/CCF/blob/docs/manager-os-drive-hardware-review-chat-20261111-v1/Docs/Handoffs/OS-Drive-Readiness-20261111/MANAGER-384-TO-CHAT-OS-DRIVE-HARDWARE-AND-WINDOWS-MIGRATION-REVIEW.md . Work from the published #383 measured mount/root-capacity evidence and current T3/CCF coordination. The internal Linux root had just ~1.27GB available; distinguish that urgent capacity risk from ZX20's separate free space. Give an evidence-grounded read-only OS-disk and partition architecture review, options for Windows vs WSL and ZX20 migration, safe candidate rankings and exact unknowns/approval gates. **#383 is already RUNNING as the sole local filesystem auditor: do NOT duplicate local scans or touch its manifest/locks.** Publish one docs-only GitHub assessment to the specified CHAT-384 report path and return the URL to Manager #200. No deletion, repartition, formatting, migration, databases, Unity, login secrets, second reviewer or removal of T3 Fork Integration HOLD.

**User action:** Paste that one message into the new ChatGPT chat. The machine need not be touched to conduct this GitHub-evidence review.
