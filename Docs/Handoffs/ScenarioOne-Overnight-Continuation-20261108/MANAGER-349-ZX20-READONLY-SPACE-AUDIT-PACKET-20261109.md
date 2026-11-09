# Manager #200 — ZX20 read-only storage recovery audit before one supervised Unity witness

**Decision:** 9 November 2026. User selected **Option 1** (prepare for one supervised Unity test) and expressly asked to **inspect what can be removed from ZX20 before testing**. **This authorises inspection and options only.** No deletion, moving, caching cleanup, Unity, installed lock modification, real lease or transition is approved.

**Claim:** [#349](https://github.com/jermur788/CCF/issues/349) — **READY / NOT RUNNING**, single local worker, then STOP for Manager/user cleanup selection. **The prior #345 operational NO-GO remains binding.** [#347/#348 Manager-approved source-only candidate and lease proposal](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-347-348-SOURCE-ACCEPTANCE-AND-UNITY-HOLD-20261109.md) are **not** deployed or Unity-qualified.

## 1. Plain-English purpose and thresholds

ZX20 had roughly **22 GiB free** on the last #345/#346 local observation, only about **2 GiB above the 20-GiB immediate Unity STOP threshold**. This is a historical reading, not a current one. **27 GiB is the cold Unity entry gate**; it is *not* an automatic warm-lane requirement. For a supervised warm test we need substantially safer space above 20 GiB, calm device I/O, a reliable 20-second disk STOP monitor, a valid candidate and owned machine-wide lease. How much additional headroom is feasible must be based on **today's actual measurements** and the likely warm-run consumption. Do not turn a planning preference into an accepted threshold.

The task is to produce a trustworthy shortlist: "These exact folders use this much space. This one is still needed; this one might be movable if a verified backup exists; this one might be a true duplicate. Here is the conservative space saving and the risk of each choice." **Do not remove or move any of them.**

Existing reference evidence:
- [#345 read-only local readiness](https://github.com/jermur788/CCF/blob/d8951e277b9afcee7cc01b8cbd3112a8b1ce93ba/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/UNITY-LEASE-SUPERVISED-DEPLOYMENT-READINESS.md): deployed lock/helper original, no lease/Unity then; registered warm Workbench old HEAD and 8 protected bytes backed by #318; Library about 6.6 GiB; ZX20 ~22 GiB, Files ~13 GiB (98% used), high I/O then.
- [#333 Unity package/storage/tree readiness](https://github.com/jermur788/CCF/blob/f1a501b17609c6c61fda7ccbb09ffe628e66d2e1/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/UNITY-PACKAGE-STORAGE-AND-TREE-DIAGNOSTIC-READINESS-RESULT.md): multiple old warm Libraries and recovery candidates exist but are not automatically disposable; exFAT inode/stat noise may increase after broad scans without changing file bytes.
- [#345 Manager NO-GO on deployment](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-345-READINESS-RULING-NO-DEPLOYMENT-20261108.md): device/owner/global flock/timeout risk not resolved simply by freeing space.
- [#347/#348 reviewed source state](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-347-348-SOURCE-ACCEPTANCE-AND-UNITY-HOLD-20261109.md): verified B3 source candidate C and post-transition profile are *source only*, not deployed. B3 actual Unity NOT RUN. Any eventual Unity run needs exact candidate identity and archive-field checks because runner's EVIDENCE_COMPLETE can be misleading.

## 2. Worker, ownership and branch

- **Purpose/difficulty:** **MEDIUM**, because this is shared exFAT storage with warm Unity Libraries, backups, single-writer ownership and recovery risks. Primarily a local inspection and decision report, not a development/implementation task.
- **Best-suited model/provider:** whichever legitimately available local T3 Code worker has reliable Linux disk and Unity operations experience (Claude Sonnet or capable Codex, effort Medium/High if offered). Report actual provider/model/effort; do not misstate effort or seek security-filter workarounds.
- **Claim:** [#349](https://github.com/jermur788/CCF/issues/349). Before substantive inventory, re-read live Docs/Project/coordination/active-tasks.md, relevant active work, mount/device identity and current owner/process/Unity lease state. Do not seize an owned machine or drive. If device/mount is inconsistent, I/O pressured, unreadable or competing Unity is active, **STOP BLOCKED** without broad scanning. Post an accurate **[RUNNING]** receipt with provider/model/effort, branch/worktree/start HEAD and preflight before work.
- **Conditional new writable documentation lane** only if unused/clean/space-safe: branch docs/zx20-space-inventory-20261109-v1 from freshly checked origin/main, sparse docs worktree /media/jer/Files/CCF/Operations/CCF-ZX20SpaceInventory-20261109, using common Git store /media/jer/Files/CCF/Repository.git. T3 must actually bind the worker to it. A small existing source metadata fetch is allowed for this own new lane, not rewrites of other worktrees; no full clone or Unity Library copy.
- **Evidence:** /media/jer/Files/CCF/Verification/CCF-ZX20SpaceInventory-20261109/ — *small text logs and hashes only*. Files has historically been 98% used, so do not make a large backup, archive, copy, disk image or scratch clone there. If the lane cannot be created safely, publish a BLOCKED issue receipt rather than writing elsewhere unapproved.

## 3. The inspection sequence — strictly no filesystem changes

**Phase 0: machine and drive safety.** Measure current ZX20, Files and root free/used in GiB; mount type and correct device; I/O pressure; active Unity/editor, lock/acquire and other heavy file jobs, current ownership. Use read-only tools. If an active owner or high I/O makes the inventory unsafe, stop. This is NOT a Unity lease acquisition.

**Phase 1: reuse existing evidence first.** Read historical storage inventory, original recovery-layout notes and source/handoff records; if Docs/Operations/StorageArchitecture.md exists as an uncommitted local file, it is **protected** and may be *read* for classification, not edited/staged/committed. Verify any potentially removable archive/Library against the live task/branch/ownership record, not simply filename or old timestamp. Treat original user data and recovery copies as KEEP until independently verified recoverable.

**Phase 2: bounded size accounting.** Start with directory metadata and shallow sizes at the ZX20 root and already known named locations; avoid a recursive full-device du/find of all Libraries or 116,000 cached files. Expand **only specific promising candidates** if current I/O conditions permit, with bounded execution, low scheduling priority and minimal stat/atime side effects; do not perform a raw 1.138-GiB content scan or hash gigabytes of caches. Record allocated (filesystem-used) bytes where possible rather than apparent size, and avoid double counting hard links, parent directories, shared directories, sparse files or the same candidate twice. If not safely measurable, mark size UNKNOWN or bounded estimate and state why. No process kill on a hung read.
  
**Phase 3: classify, do not act.** For the largest likely directories, research their live use, owner and independent restore/backup; candidate classes to **inspect**, not presume safe: obsolete build outputs/logs, obsolete editor/import scratch, duplicate package download archives, old worktree copies/recovery clones, extra warm Libraries and cached Unity packages. **Explicitly protect** until separately authorised:
- registered original warm Workbench at /media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106 including its **Library**, current branch/HEAD/index, 8 protected tracked working files, original backup chain and #318 evidence;
- the original standalone clone/recovery anchor, main integration/other warm Unity lanes and Libraries unless owner+recovery proofs establish their status;
- deployed CCF-Ops scripts/helpers/config, global guard/lock/event history, Unity Editor installation and licence, current package caches, frozen Reference Future v1, Legacy40, saves, scenes/assets, and any user's unrelated files;
- evidence archives needed for prior/source review, personal playtest or candidate provenance. No Git clean/reset/stash/gc/repack, Unity cache deletion, or guessed 'safe' folder purge.

**Phase 4: evaluate offload destinations.** If a candidate might be movable, identify an **actually available separate device** (never assume Files, which is full). Show free capacity, verify whether a complete independent backup with checksum and readable restore exists, and say whether moving would truly increase free space on ZX20. An archive on ZX20 does not free ZX20. Deleting originals before an independently verified copy/rollback is NOT allowed.

## 4. Report format and user decision gate

**Exact GitHub report:** Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/ZX20-READONLY-STORAGE-RECOVERY-OPTIONS-20261109.md.

Report must include a short PASS/FAIL/UNKNOWN table for device and owner preflight, current disk metrics (time and units), scan completeness, and a ranked table of ideally 5–10 candidates with:
- exact path and allocated GiB (or UNKNOWN with method);
- owner/current use and whether this is part of warm Library, package, recovery, backup or active lane;
- **KEEP/PROTECTED**, **POSSIBLE MOVE AFTER VERIFIED BACKUP + APPROVAL**, **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL**, or **UNKNOWN / DO NOT TOUCH**;
- user-visible plain-English reason and downside of each option, independent restoration source and proof status, realistic recoverable GiB (conservative; avoid double count), confidence;
- distinguish current measurement vs historical estimate; total low-risk potential recovery vs higher-risk speculative recovery;
- whether a safe external destination actually has space, and a recommended selection for Manager/user to approve **later**. No recommendation can silently approve an operation.

Finish with explicit GO/NO-GO for whether **only the read-only audit** was completed and a concise proposed safe cleanup packet if the user selects specific paths; list outstanding Unity gate requirements even if enough space is found (global lock duration, operator, current owner, licence, current warmed candidate and disk stop monitor). Confirm **0 bytes intentionally deleted/moved/modified on ZX20**. Keep evidence small and publish report to the fresh docs branch. Mark #349 **[HANDED-OFF]** or **[BLOCKED]**, release only its own Files lane (leave in place; no forced cleanup) and **STOP for Manager #200**.

**Manager response after handoff:** inspect the evidence, accept/reject, then show the user a *short plain-English candidate table with pros/cons and exactly how much each could save*. Seek separate explicit approval for named paths and operations. **Do not dispatch a cleanup worker or Unity until that choice is made.**

## 5. Hard exclusions

No delete, move, archive, rename, trash, copy of huge content, clear cache, Git clean/gc/repack, reformat/repair, changing owner/permissions/index, changing any tracked/working byte or HEAD, stopping processes, altering deployed scripts, lock records, original backups, Unity licence, other owners' Libraries, creating an actual Unity lease or running the Editor, production source changes, integration, A–O, Linux or Windows build. The user did not authorise removal, only **inspection**. No attempts to bypass active machine locks or the outstanding #345 deployment NO-GO.

## 6. One paste-ready local T3 dispatch

> CCF #349 — READ-ONLY ZX20 storage inventory before a supervised Unity test. Read the full binding Manager packet: https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-349-ZX20-READONLY-SPACE-AUDIT-PACKET-20261109.md . User selected Option 1 and asks to inspect what can be removed; user has **NOT** authorised deleting/moving anything. On a newly verified sparse Files docs lane after live coordination/owner/device/disk/I/O preflight, examine ZX20 shallow directory and existing storage/recovery records with bounded low-impact read-only measurements. Preserve original warm Workbench/Library, eight dirty tracked files/backup chain/index, all other owned lanes/recovery, CCF-Ops, Unity licence and evidence. Avoid full recursive Library/device scans, busy I/O or big copies to nearly full Files; STOP BLOCKED if unsafe. Publish ZX20-READONLY-STORAGE-RECOVERY-OPTIONS-20261109.md with current ZX20 capacity, ranked exact candidate paths, real reclaimable GiB, ownership/backups, plain-English pros/cons and separate USER APPROVAL needed. Do not delete, move, clean, stage, deploy Unity-lock, acquire a lease, run Unity or modify original ZX20. HANDOFF/BLOCKED and STOP Manager #200. No automatic cleanup or Unity task follows.

**User action:** paste this single inspection-only packet into a local T3 Code worker/orchestrator. Any subsequent move/delete requires a new user decision.
