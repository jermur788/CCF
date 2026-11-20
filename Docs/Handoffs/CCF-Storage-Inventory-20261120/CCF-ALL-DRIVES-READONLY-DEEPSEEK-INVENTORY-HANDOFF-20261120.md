# CCF — Read-only inventory of all laptop drives
**DeepSeek local inspection packet | 2026-11-20 | READY / UNASSIGNED / NOT RUNNING**

## Owner's exact outcome
Inspect every accessible CCF-related folder across the laptop's internal and connected drives, identify older copies and which are no longer used, and quantify potential space recovery. **This task is inspection only. The owner has explicitly NOT authorised any deletion.** Produce a complete, evidence-based inventory and a future approval queue, not a cleanup.

## Task identity and authority
- Class: PACKET ONLY (bounded local audit, not game implementation or migration).
- Requested worker: DeepSeek, but only if actually available with laptop filesystem read access. Actual provider/model, host, and readiness are UNKNOWN until the worker verifies them. A browser-only agent must not claim to have inspected physical disks.
- Manager: CCF Overall Manager #425 or its authorised storage delegate. No source custody, Unity reservation, physical Ops write ownership, or active claim transfers by issuing this packet.
- Repository: https://github.com/jermur788/CCF . Observed main base on packet creation: **c94f2a2b0dee439cbe2a1269fc47d7421a3e1be2**. Attached canonical context stamp: **852304d8a668d6ee2b04547aaec751be6b462c53**; worker must refresh and verify live context.
- Execution lane: **read-only laptop inspection**. No writable CCF worktree. Only new report Markdown/TSV files may be written on a dedicated one-writer, docs-only GitHub branch under Docs/Handoffs/CCF-Storage-Inventory-20261120/. Existing project folders are read-only.
- Status: READY / UNASSIGNED. Do not mark RUNNING before genuine local start and fresh ownership checks.

## Absolute safety limits
**NO** deletion, moving, renaming, compression, archiving, project copying, backup creation, cleanup, permission changes, installs, mounting, unmounting, sudo, Git fetch/checkout/reset/stash/clean/prune/gc/worktree remove, branch deletion, Git shared-store modification, Unity launch/import/build, cache eviction, save/PlayerPrefs edits, SQLite/WAL edits, process termination, T3/Ops mutation, merge, or changing any source data. Even "dry run" commands must be assessed for side effects. Report-only writes must not be made inside audited folders. No raw credentials, personal saves, private files, or licensed binary assets may be uploaded to GitHub or a third-party model. Metadata and redacted evidence only. Do not promise physical zero writes if reads may affect filesystem atime.

Permitted: read-only mount and disk-space queries, bounded directory listing and stat/size, current process and ownership checks, read-only Git identity/status with **GIT_OPTIONAL_LOCKS=0**, targeted comparison of file metadata and very limited hashes when justified. Do not hash all Libraries or duplicate huge archives by default.

## Required preflight
1. Confirm actual host is the intended laptop, not a remote Windows/WSL/scratch host. Enumerate all presently mounted user-data volumes with read-only findmnt, lsblk -f and df -B1 (or equivalent). Record physical filesystem/device/UUID, mountpoint, capacity, free/used, mount flags and timestamp. Unmounted/disconnected drives are **NOT INSPECTED**; never attempt to mount them.
2. Refresh live [active task coordination](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md) and local CCF-Ops owner/lease records READ ONLY. Preserve #425 Overall, #887 art, #885 Multi/2D, #886 collision, #882 2D, personal Unity/Editor tests and newer owners. No process observed does not imply a released lock.
3. Read [existing CCF storage audit #383](https://github.com/jermur788/CCF/issues/383), its [Manager residual ruling](https://github.com/jermur788/CCF/blob/docs/manager-383-residual-owner-and-worktree-decision-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-RESIDUAL-ADDENDUM-B1-CONDITIONAL-CLEANUP-RULING.md), its [partial inventory report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CLEANUP-APPROVAL-QUEUE.md) and linked residual TSV. Reuse already established evidence without repeating expensive scans, but refresh its age/ownership/existence when necessary.
4. #383 on 11 November observed around **107 Main and 45 Legacy worktrees**; ten clean docs-only worktrees on **Files** (about **12.33 GB allocated**) only **conditionally eligible for future user approval**; two older **ZX20 Unity Libraries** totaling about **14.43 GB** with unresolved owner and reimport/licence concerns; builds with backup or evidence gaps. All are dated observations, **not current measurements** and **not authorised deletions**. #383 remains PARTIAL; approved cleanup is ZERO.
5. Specifically protect personal playtest copies #361 and #382, their warm Libraries, source stores and linked worktree pointers, local-only changes, licensed/ignored art, original saves and Unity prefs, frozen Reference Future and Legacy40, live T3 files, and all currently claimed test/worker lanes. Owner-reported #871 scratch removal on 20 November must not be repeated; record actual presence/absence if observed.
6. Prior wide scanning had to stop due to very low root free space. Start metadata-only and use bounded per-volume workload. On low headroom, I/O error, intrusive load, permission conflict or contested active owner: stop that section, report incomplete coverage, continue safe nonconflicting inspection.

Historically known search hints, not assured actual mounts: /home/jer, /home/jer/Documents/CCF Game, /media/jer/Files, /media/jer/Files/CCF, /media/jer/ZX20, /media/jer/ZX20/CCF-Primary. Check actual root and any other mounted user-data volumes.

## Discovery scope — not just folders containing "CCF"
Find **all accessible CCF-related roots** on each mounted user-data filesystem:
- Unity 3D projects, 2D C# projects, independent clones, linked Git worktrees, bare shared repositories, dated versions, branch/integration/test projects, detached roots, scratch harnesses and legacy copies.
- Standalone builds and exported players, archives/backups, CCF assets and art, research documents, photos/audio, logs and evidence, tools, cloud-synced copies, possible historical output caches.
- Renamed directories whose markers or Git remotes identify CCF. For Unity look for Assets + ProjectSettings/ProjectVersion.txt + Packages/manifest.json; for 2D look for solution/project files and remote/manifest evidence; for linked Git, distinguish a .git **file** from independent .git directories and shared bare Git stores.
- Do not search unrelated personal documents by content; use bounded metadata for discovery.

Use a staged scan: mount table and known roots, registered worktree metadata, likely user-data direct children, then deeper directory-name/project-marker discovery. Avoid traversing /proc, /sys, /dev, /run, secret stores or deeply walking .git objects, Unity Library, node_modules and build payloads merely for discovery. Their space is still accounted for separately when relevant. Avoid following symlinks and crossing mounts or double counting nested objects. Label any depth limits as partial.

## Per-item evidence
For EACH distinct root, capture:
1. Absolute canonical path, parent/root nesting, mount filesystem UUID/device, type (Git/Unity/2D/build/archive/research/cache), directory/size observation time.
2. **Allocated** and, when useful, **apparent** bytes; method and uncertainty. For large Unity folders distinguish tracked source, shared Git data, Library, Obj/Temp, Builds, ignored/licensed assets, user save/Prefs locations (metadata only). No duplicate parent+child counting; identify hard links, sparse storage, symlinks and shared Git stores. A Files-volume byte cannot free ZX20/root space.
3. Git identity: linked common Git directory, registered worktree owner, full HEAD/branch/detached, remote reachability without fetch, staged/tracked/untracked **and ignored** changes, any local-only commits. Git ancestry or remote branch existence alone does NOT establish duplicate data or safe removal.
4. Current purpose and use evidence: live coordination and Ops row, actual project/playtest/agent references, process cwd/open project if read-only detectable, timestamps with provenance. Age and no process are weak clues only, not proof of disuse.
5. Unique data, licence/ignored dependencies, backup/restore evidence, standalone tested binary provenance, cold Unity reimport cost, saved-game impact and shared Git metadata implications. UNKNOWN where unsupported.
6. Relationship to newer folders: identical verified bytes vs similar files, shared Git objects, ancestor source commits, branches, independent clones, archive vs unpacked folder. Do not hash terabytes just to prove identity.

## Classification, risk and reclamation
Give EACH item one:
- ACTIVE — PROTECT
- UNIQUE / HISTORICAL / OWNER PROTECTED — PROTECT
- OWNER OR DEPENDENCY UNKNOWN — HOLD
- POSSIBLE REDUNDANT COPY — MORE PROOF
- CONDITIONAL CANDIDATE FOR FUTURE OWNER APPROVAL
- ARCHIVE / BACKUP REQUIRED BEFORE CONSIDERATION
- NOT INSPECTED / OFFLINE / ACCESS BLOCKED

Add evidence type DIRECT OBSERVATION / HISTORICAL #383 / INFERENCE / UNKNOWN and HIGH/MEDIUM/LOW confidence. **None** is permission to delete. Do not call old directories "unused" based only on name or file dates. Show conservative **potential reclaim upper bound** per filesystem, separate from actual directory footprint; note shared bytes, overlaps, hard links, non-reproducible assets, risks and restoration costs. Rank review candidates by *which disk benefits*, not simply folder size. Previously closed issues do not automatically release a newly claimed local worktree.

## Required durable result
One GitHub-tracked **Markdown report** and one compact **CSV/TSV register**, on a new docs-only branch, under Docs/Handoffs/CCF-Storage-Inventory-20261120/. Read back both from GitHub and verify only these new report files changed.

Report must cover:
1. **Drive-by-drive table:** mount device/UUID, free/used bytes, number of CCF roots, CCF measured size, measurement coverage and unknown/offline mounts.
2. **All accessible CCF folder table:** exact path, parent, volume, category, allocated/apparent bytes, Git branch+HEAD+shared common-dir, tracked/untracked/ignored status, owner/lease, last verified use, unique assets, independent backup, category, confidence, evidence and review gate.
3. **Duplicate/related-version map:** what truly matches, what merely shares history, what is protected/active, old copies and why they may still matter.
4. **Storage ranking per disk:** largest space consumers, potential reclaim upper bounds without overlaps, likely reimport/restoration cost, and unresolved evidence.
5. **#383 reconciliation:** retained facts, changed findings, dated observations not reverified, skipped expensive rescans and current owner/backup blockers.
6. **Future approval queue, not a deletion plan:** exact potential paths and evidence still needed, including any owner decision. No general command to delete "old CCF folders".
7. **Nonmutation attestation:** scanning method, timestamps, permissions and limitations; proof no project files, Git metadata, Unity state, saves or caches were altered by the worker; report any accidental write honestly.

Suggested TSV columns:
id | mount_uuid | mountpoint | absolute_path | parent_root | type | allocated_bytes | apparent_bytes | size_method | common_git_dir | branch | head | dirty_tracked | untracked | ignored | live_owner_lock | latest_verified_use | unique_local_data | independent_backup | status | confidence | evidence | possible_reclaim_upper_bytes | next_gate

Do not leave unknown values blank: write UNKNOWN, NOT CHECKED or NOT APPLICABLE as appropriate. Keep sensitive file content out of report.

## Acceptance / stop
Accept only when **every accessible mounted user-data drive is covered or explicitly NOT INSPECTED**, each discovered CCF root is classified with size or documented uncertainty, ownership and backup risks are visible, #383 is reconciled, report files are read back from GitHub, and **zero deletion is claimed approved**. Do not open Unity or run tests (metadata qualification only, no Tier 2/3).

Stop only the unsafe action on disk/IO/permissions/locks, unexpected sensitive content, ambiguous shared Git objects, insufficient backup proof, or if asked to mount/copy/migrate/delete anything. Continue other safe sections and report a partial result rather than escalating silently.

Final handoff to Manager must include actual worker/model/host, mounted drives, count/size/coverage, candidate states, protected locations, historical vs direct evidence, outstanding unknowns, commands class, confirmation of no mutation, direct GitHub report and TSV links, report commit SHA, and *next owner approval questions only*.

**Next action:** An actually laptop-capable DeepSeek worker receives this packet, verifies local host, current mounts and coordination, reconciles #383, and starts read-only discovery. The owner controls every later deletion separately.
