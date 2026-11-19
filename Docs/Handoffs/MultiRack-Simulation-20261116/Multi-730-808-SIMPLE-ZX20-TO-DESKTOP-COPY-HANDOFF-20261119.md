# Multi #808 — Copy Year0 Viewer and Year2–10 Engine archives from ZX20 to desktop

**19 November 2026 — READY. The owner confirms ZX20 is connected to the desktop. Single simple file-copy task, not a rebuild or integration task.**

## Job

Copy the entire existing folder **`CCF-Transfers/Multi-20261119`** from the attached **ZX20** drive to a new dedicated folder on the desktop's **local storage**. Keep the original folder on ZX20 untouched. Do not copy just the HTML viewer or the Git bundles: copy **everything in that folder**, including Viewer and Engine subfolders, SHA256SUMS, MANIFEST.txt and RESTORE.md.

The #789 source reported a corrected, restore-tested **1.15 GiB** archive set, including full-history Viewer and Engine Git bundles. The first viewer bundle failed before correction; use the **final existing archive set**, never an old failed bundle. The drive identity reported is exFAT UUID **18D6-896E**. [Source result](https://github.com/jermur788/CCF/blob/docs/multi-789-one-shot-result-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/789-orchestration/Multi-789-ONE-SHOT-YEAR2-10-ENGINE-IMPLEMENTATION-QUALIFICATION-ZX20-TRANSFER-RESULT-20261119.md).

**Difficulty LOW–MEDIUM** — straightforward copy plus integrity verification. **Best agent:** normal authenticated Codex/native desktop operator (any model with actual desktop filesystem access; browser-only GitHub cannot copy files). **Issue:** [#808](https://github.com/jermur788/CCF/issues/808). No source-code branch or development worktree required. **Source:** attached ZX20 `CCF-Transfers/Multi-20261119/`. **Destination:** a new, clearly named folder on local desktop storage, e.g. `CCF-Archives/Multi-20261119/` within a user-writable data volume. Discover real source drive letter/mount and destination path; do not assume Windows or Linux. No formatting, installing software, relocating existing data or changing Git source.

## Steps

1. Register #808 **[RUNNING]** with actual machine/OS, operator and source/destination paths. Confirm the attached drive is ZX20 and find the exact existing `CCF-Transfers/Multi-20261119` directory. Check destination local-disk free space (allow at least **3 GiB** for this ~1.15 GiB copy) and choose a **new empty folder** without overwriting anything.
2. Copy the whole folder recursively using the OS file manager or a reliable normal filesystem copy. Preserve every file, name, nested folder and byte; no move/cut operation, delete or cleanup. No archive extraction, Git clone, WSL2 setup, build or tests in this task.
3. Verify source and destination have the **same relative file list, file sizes**, and all entries of **SHA256SUMS** validate **against the destination files** (previous #789 carrier receipt: 9/9 OK). Use the actual manifest and checksum file; if a check fails, retry only the incomplete copy safely, do not claim PASS or alter source.
4. Record actual destination location, total bytes, file count, checksum results, whether copy is fully complete, and any failure. Publish one brief **GitHub Markdown result** at `Docs/Handoffs/MultiRack-Simulation-20261116/808-orchestration/Multi-808-ZX20-TO-DESKTOP-FILE-COPY-RESULT-20261119.md` on a new docs-only branch; link it on #808 and #730. Mark #808 HANDED-OFF. If GitHub not accessible on the desktop, report the exact on-disk receipt/path to Manager without inventing a published handoff.

## Stop / exclusions

Do not erase ZX20 contents or laptop originals. Do not merge or integrate into main, unpack/restore Git repos, run Unity/.NET/Node, produce Windows game builds, alter #784/#789/source/fixture, attempt Host/save, change physical Ops board or waive L4 FAIL38. This is **only a verified desktop file copy**. Later restoration into working development repositories is a separate step if wanted, covered by [#801 desktop guide](https://github.com/jermur788/CCF/blob/docs/multi-801-gpt6-desktop-restore-dossier-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-801-DESKTOP-RESTORE-AND-INDEPENDENT-QUALIFICATION-ACCEPTANCE-DOSSIER-20261119.md). #806's optional 2D spec review does not block the copy.

**Paste-ready dispatch:** Execute Multi #808: ZX20 is already attached to the desktop. Copy the ENTIRE `CCF-Transfers/Multi-20261119` folder from ZX20 into a NEW `CCF-Archives/Multi-20261119` folder on a local desktop data drive. Check at least 3 GiB destination free, copy all subfolders/files including Git bundles, archives, evidence, SHA256SUMS, MANIFEST and RESTORE, then verify the destination's file list/sizes and all 9 SHA256SUMS entries. Do not delete the ZX20 source, unzip anything, build or integrate. Publish one short GitHub receipt with the actual destination path and PASS/FAIL.
