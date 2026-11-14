# MULTI-006 — Narrow P0 lab save-root write-race repair and size-guard test

**Date:** 14 November 2026  
**Execution claim:** [CCF #510](https://github.com/jermur788/CCF/issues/510), **READY / NOT RUNNING**; no worker, local worktree, physical host or coordination lock is registered yet.  
**Difficulty:** **MEDIUM–HIGH**. Bounded source paths, but correct Linux \`openat\` / \`renameat\` / \`unlinkat\` descriptor ownership, failure cleanup and adversarial folder swapping require careful engineering.  
**Best-suited implementation worker:** **Sol/Codex High in local T3 Code** if available; independent capable **Claude Sonnet 5.5/Claude Code** implementation is also acceptable with a separate **Opus/Codex independent recheck**. Record actual worker rather than the recommendation.  
**Purpose:** Repair only R-1 MAJOR and R-2 MINOR in the independent MULTI-005 review. No full P0 reimplementation, no P1 biology or Unity.

## 1. Binding source, authority and Manager verdict

- [Independent MULTI-005 review — NEEDS CHANGES (narrow)](https://github.com/jermur788/CCF/blob/review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md), [#504](https://github.com/jermur788/CCF/issues/504) HANDED-OFF/CLOSED. **One MAJOR, one MINOR**, no CRITICAL.
- [MULTI-004 author handoff](https://github.com/jermur788/CCF/blob/feature/multi-004-sitka-lab-p0-file-safety-hardening-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-HARDENING-IMPLEMENTATION-HANDOFF-20261114.md), [#502](https://github.com/jermur788/CCF/issues/502) HANDED-OFF/CLOSED. Current P0 read/FIFO security and layered-guard work passed independent recheck, **but the full MULTI-004 hardening is NOT accepted**.
- [MULTI-003 earlier independently APPROVED engineering P0](https://github.com/jermur788/CCF/blob/review/multi-003-sitka-2d-lab-p0-independent-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md) remains approval for the **original** candidate, not retroactive approval for these code changes.
- [Approved P0 scope](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md): separate synthetic Sitka engineering lab, **NO BIOLOGICAL GROWTH**, C#/.NET local browser, only lab-root saves. No permission to change feature scope.

**Manager decision:** **Do NOT accept R-1 as an exception.** Same-user local process is needed and CCF game saves were not targetable, but writes did reach a different directory and overwrite test files, with temp-file leftovers and misleading responses. That violates the existing approved lab-save isolation and the purpose of the F-5 repair. Make an actual narrow repair and return for independent proof.

**R-1 — MAJOR, reproduced by reviewer:**
- \`src/Core/LabDataStore.cs:41-58,151\`: store classifies using a trusted directory handle but later uses absolute paths for \`FileStream(CreateNew)\`, \`File.Move\` and \`File.Delete\`. A concurrent same-user \`renameat2(RENAME_EXCHANGE)\` swapping \`.lab-data\` and a symlink makes these resolve against different directories.
- Reviewer observed **1,223 outside overwrites** at store level and **12 outside overwrites** through the real host; temporary files accumulated in both folders; an HTTP response sometimes indicated success or a refusal after a write had already occurred. These are independently reproduced observations, not worker claims.
- The **read side** uses safe handle-relative \`NoFollowFiles\` open; exploit is on the **write side**. Do not undo its protections.

**R-2 — MINOR, independently proved:**
- \`src/Core/NoFollowFiles.cs:162\` second bounded-read size limit works after a \`statx\` size=0 result, but removing it survives the Core suite. A deterministic test against the **regular procfs file** \`/proc/<current numeric PID>/status\` returns more than 10 bytes despite size=0; call the low-level read helper with max=10 to require \`FileTooLarge\`. Add one targeted mutant that removes the second check and must be **CAUGHT**, not build-broken.

## 2. Candidate branch and no-collision preflight

**Created and untouched implementation branch:** \`feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1\`, from the exact MULTI-004 candidate handoff HEAD \`9ba88b6e3fff3d2ebff10fc88797bf73c0ebc5cc\`.

**Proposed, NOT CREATED / NOT RESERVED local worktree:**
\`/media/jer/Files/CCF/Operations/CCF-MULTI-006-SitkaLab-WriteRepair-20261114\`

**Proposed off-repo evidence:** \`/media/jer/Files/CCF/Verification/MULTI-006-SitkaLab-WriteRepair-20261114/\` (not created). The independent reviewer's attack probes/smoke scripts are under \`/media/jer/Files/CCF/Verification/MULTI-005-SitkaLab-DeltaReview-20261114/\`, accessible **only if actually present on the local host**; copy/read those receipts only with permission and without changing them.

**Allowed tracked paths after actual ownership preflight**:
- \`Tools/MultiRackSitkaLab/src/Core/NoFollowFiles.cs\`
- \`Tools/MultiRackSitkaLab/src/Core/LabDataStore.cs\`
- \`Tools/MultiRackSitkaLab/tests/**\` (only directly necessary store/host/guard tests and mutation script)
- \`Tools/MultiRackSitkaLab/README.md\` (update the stale path-based write limitation)
- ONE new final Markdown: \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md\`

**Forbidden:** approved four-file \`specs/**\`, core tree/plan biology/actions, UI fixtures/schema, \`Assets/**\`, \`Packages/**\`, game \`Docs/Project/**\` and live coordination board, main, previous owner worktrees, CCF production save locations and Reference Future, 3D/racks/Oak/Beech/P1 growth, Windows/Player build, Unity. Do not broaden local native code changes to unrelated features.

**Before any source edit or test**:
1. Fresh-read [#510](https://github.com/jermur788/CCF/issues/510), this packet, independent review, live [active-tasks.md](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), current branch HEAD, all existing worktrees, local physical machine/host and processes, unrelated dirty files and any Unity lease. Previous chats' "no MULTI row" is not a lock grant.
2. If the proposed laptop/Files-drive worktree is unavailable, another worker owns overlapping paths, no authorised board writer can resolve a conflict, or the host is unsafe: mark [#510](https://github.com/jermur788/CCF/issues/510) **BLOCKED** with details. Do not stash/clean/reset/kill unrelated work, reuse another worktree or self-authorise a board edit.
3. Once ownership and host are safe, create/verify separate worktree at exact branch/base, then record **actual model/provider, worktree/host, starting FULL HEAD, read/write scope and phase** as **[RUNNING] on #510 BEFORE production edits or tests**. Claim tracks execution presence, not ownership. No Unity process may be started for this task.

## 3. Exact code repair — R-1 (no path-resolution swap on saves)

**Desired safety invariant:** for one save operation, **opening the trusted \`.lab-data\` folder, creating temporary file, writing bytes, renaming it to the final name and removing any failed temp file MUST ALL use the SAME opened directory handle**, not re-resolve \`.lab-data\` by absolute pathname between steps. Under adversarial swapping of the directory path and symlinks, do not create or overwrite a file in the *attacker-controlled alternate directory*. The original trusted directory inode may be moved by that same-user actor; distinguish that inode-anchored security guarantee from immutable absolute path geography when reporting results.

Implement this in/under \`NoFollowFiles.cs\` and the \`ILabFileAccess\` / \`RealLabFileAccess\` / \`LabDataStore\` write seam:
- Open the lab folder with the existing fail-closed \`OpenDirectory\` logic (\`O_NOFOLLOW|O_DIRECTORY|O_CLOEXEC\`) and retain its FD for the **entire** transaction. Use a bare validated final filename, never a concatenated absolute file path for subsequent open/move/delete operations.
- Create uniquely named temp with \`openat(dfd, tmpName, O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC, 0600)\`; check exact flags/ABI on supported Linux x64. Write all bytes with EINTR handling, partial-write handling, bounds and FD closing; then **\`renameat(dfd, tmpName, dfd, finalName)\`**. If any stage fails after creation, **\`unlinkat(dfd, tmpName, 0)\`** under the SAME dfd. Only delete this operation's temp, not a neighbouring or other worker's file. An optional fsync must not be used to expand the task into durability redesign.
- Preserve existing allowed name shapes, approved header/schema separation and platform fail-closed. No symlink following and no FIFO/directory/blocked-file opening. Preserve controlled error codes, existing save round-trip and refusal of squatting names, and no physical-path leakage or spurious success.
- Verify original store's preclassification is **not** the sole protection against final-name swaps; the actual rename target is a directory entry in the trusted dfd. If accepting a legitimate overwrite of an existing regular lab save, it must never overwrite in a different attacker-substituted directory. Ensure the outcome reported matches the actual transaction state as far as practical under same-user interference.
- Update README accurately. Document that **Linux x64 alone was independently tested**, Linux arm64 remains conditional/untested and other OS file operations intentionally fail closed 501. Do not add a Windows implementation in this task.

**Required R-1 tests:** a deterministic hook after prechecks but before dir-anchored write, plus a bounded concurrent \`RENAME_EXCHANGE\` swap of the folder with an alternate symlink to valid lab-json saves; verify **zero outside alternate-folder overwrites**, **zero temp orphans** after settled test in either directory where possible, no unrelated/decoy deletes, genuine save/load remains functional, correctly reported status. Test HTTP host path as well as store, including save while the directory is swapped, with counts and timed stop. Use safe scratch-only directories and no production save root. Add a mutation control intentionally reverting to path-based temp/open/rename or removing the handle anchoring; it must turn the targeted tests RED (not merely fail to build). Keep F-1/F-2 read-side tests intact.

## 4. Exact test repair — R-2 (post-statx growth guard)

Add a deterministic Linux test under \`tests/Core.Tests/\` using \`/proc/<numeric current process ID>/status\`: the procfs regular file reports a zero statx size but produces a readable body, and \`NoFollowFiles.ReadRegular\` must reject the content at a tiny max (e.g. 10 bytes) with **\`FileTooLarge\`**. A generous max may be used as a positive control to establish the actual content. Use current PID's real numeric directory rather than symlinked \`/proc/self\` (which \`O_NOFOLLOW\` would refuse). Verify the file is a regular file via the existing descriptor guard; make test failure explicit if this supported Linux host's procfs is unavailable, rather than silently skipping an expected qualification.

Add an **XG-equivalent mutation** deleting the \`ReadAll\` loop's **second** size check and prove that this new test catches it, while the original max-before-read \`statx\` check still works. Do not weaken the existing limit to make it pass.

## 5. Proportionate verification and publication

**Actual worker-side verification after edits:**
- Tier 1 clean build with 0 warnings/errors, all Core and Host suites, three fixture SHA-256 manifest checks **from \`fixtures/synthetic/\`** (not root), schema checks, preserved event/replay/atomicity tests. Re-run original M01–M17 and expanded M18–M30 and the new write/size guards in a **disposable scratch copy**, with count/results evidence and no unrelated source contamination.
- Targeted Tier 2 live Host/API save-root swap attack, FIFO/load state responsiveness, out-of-root valid lab-json sentinel untouched, alternate-folder temp count, controlled errors and no false save success; one headless Chromium smoke or existing 15-case suite for save/load, note actual 2,000-tree measurement if run. Do not repeat full Unity/release/forestry qualification.
- Reproduce the independent MULTI-005 R-1 exploit using **reviewer-owned scripts**, if available read-only; preserve before/after log counts and full worker environment identity. Review all off-repo writes after testing; no process, FIFO, temp folder or socket left running unless owned and intentionally preserved as evidence.
- **Expected result is no outside alternate-dir write/overwrite**. If still reproducible: report **NEEDS CHANGES**, do not call secure. Any remaining user-controlled "same inode moved" limitation must be explained precisely.

**One exact final GitHub Markdown handoff, on implementation branch only:**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md\`

Include exact candidate/source HEAD and actual registered owner/worktree/host, changed-file diff against MULTI-004, R-1 and R-2 before/after code/tests, writer run receipts (no independent PASS claims), mutation XG and write-swap result, Linux-only caveats, residual race risks, no changes to approved P0 gameplay or scientific scope, evidence files/hashes. Read back the GitHub handoff and verify only permitted paths changed. Then mark #510 **HANDED-OFF**, close and notify Manager. **Manager's next step is a short independent MULTI-007 delta recheck** for exactly R-1/R-2, not another full P0 review.

**Dashboard/Drive mirror:** NOT VERIFIED. No source/Unity merge or user acceptance authorised by this packet.

## 6. Paste-ready local T3 Code dispatch

> Take ownership of **MULTI-006 / CCF #510 — narrow Sitka Lab P0 write safety repair**. Read and follow the complete binding packet at https://github.com/jermur788/CCF/blob/docs/multi-006-sitka-lab-p0-write-safety-repair-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-HANDLE-ANCHORED-WRITE-REPAIR-DISPATCH-20261114.md. This repairs independent MULTI-005 **NEEDS CHANGES**: R-1 MAJOR path-based saves can overwrite files outside \`.lab-data\` during folder swaps and orphan temp files, plus R-2 MINOR missing post-statx size guard test. **Do not accept R-1 as a caveat.** Branch is \`feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1\`, forked untouched from the MULTI-004 source. Proposed distinct worktree (NOT created/reserved): \`/media/jer/Files/CCF/Operations/CCF-MULTI-006-SitkaLab-WriteRepair-20261114\`. Recommended Sol/Codex High T3 Code or capable Sonnet 5.5 implementation; separate Opus/Codex reviews afterwards. Verify fresh live board/host/worktree/source, register **actual worker/provider/worktree/starting HEAD as RUNNING on #510 before edits/tests**, and stop BLOCKED if conflict. Change only \`NoFollowFiles.cs\`, \`LabDataStore.cs\`, necessary lab tests/mutation script and lab README; publish the exact MULTI-006 Markdown handoff. Use one opened lab directory FD for **openat temp + write + renameat + unlinkat cleanup**, handle concurrent folder swapping, preserve read defenses, errors and current P0 data/biology behaviour. Add deterministic \`/proc/<numeric pid>/status\` size-growth regression and mutant; test attacker-controlled alternate-directory zero overwrites, no temp orphans, Core/Host, old+new mutations, scoped browser/HTTP. P0 Linux x64 alone is qualified; arm64 untested, native Windows/macOS file operations remain fail-closed. **No Unity, production saves, P1 ecology, real data, approved spec or live coordination edits, Windows build, or out-of-scope changes.** Return the GitHub handoff for **independent MULTI-007 focused recheck**.
