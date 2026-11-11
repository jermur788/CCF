# Manager #200 — Resume CCF-only cleanup planning after operating-system headroom recovery

**Date:** 11 November 2026, following user's 21:43 report of nearly **20 GB now free** on the drive recently cleaned.  
**Task:** existing [#383](https://github.com/jermur788/CCF/issues/383) — **READY TO RESUME, NOT RUNNING**, retaining all original audit evidence.  
**Objective:** produce the smallest safe, **itemised CCF cleanup approval queue** so the user can reclaim redundant storage **without losing source, tested game states, licences, unique evidence, personal work or migration backups**.  
**Difficulty:** HIGH for worktree/provenance/ignored-file and multi-volume ownership; no production game implementation.  
**Best suited:** **one local OpenAI Sol/Codex High T3 Code** agent, no second reviewer or model orchestration.  
**Authority:** current user instruction is to **continue CCF cleanup**. The user's prior explicit no-deletion instruction is still the destructive-operation boundary. This packet **does not authorise deletion, transfer or source cleanup**. The user must approve each exact batch after seeing evidence. The only allowed writes before that approval are new docs/manifest outputs on a dedicated audit branch, plus safe claim/coordination updates by the authorised owner. The T3 Fork Integration HOLD remains binding.

## What changed and what has NOT changed

The user states the recently cleaned drive now has **almost 20 GB free**, compared with the worker's older internal `/` reading of ~1.19 GB. **This is USER-REPORTED, not current operator-measured; first verify which mount is meant and its exact `df -B1` available bytes, mount source and UUID before resuming local work.** The increased capacity removes the stated *low-headroom reason to consider resumption if verified*, not any ownership, recovery, product, save or user-approval gate. Do not infer the user approved deleting specific CCF paths or that ZX20 capacity increased: last measured ZX20 free was 29,587,668,992 bytes and may be different now.

Existing audited evidence (do not repeat unnecessarily):

- [Sol partial CCF/T3/dashboard inventory](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md).
- [Sol targeted owner/recovery addendum](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-TARGETED-OWNER-AND-RECOVERY-ADDENDUM.md) and [candidate TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-TARGETED-CANDIDATE-ADDENDUM.tsv).
- [Manager stop/ownership disposition](https://github.com/jermur788/CCF/blob/docs/manager-383-partial-audit-review-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-TARGETED-ADDENDUM-STOP-AND-OWNERSHIP-DISPOSITION.md).
- [CCF #382 user personal Editor test](https://github.com/jermur788/CCF/issues/382), [#361 older U3 user work](https://github.com/jermur788/CCF/issues/361), `Docs/Project/coordination/active-tasks.md` **live** from `main`, CCF-Ops current lease/owner/history, latest `Docs/Operations/StorageArchitecture.md` including **pre-existing dirty Primary copy**, and current CCF claims/handoffs.

**Known CCF-only targets (measured allocated byte magnitudes, not approved recoverable bytes):**

| Candidate | Prior allocated bytes | Current classification |
|---|---:|---|
| `/media/jer/ZX20/CCF-MainIntegration-20261104/Library` | 7,216,824,320 | UNCERTAIN; parent worktree has eight dirty files |
| `/media/jer/ZX20/CCF-UIQualification-20261104/Library` | 7,210,270,720 | UNCERTAIN; parent worktree has eight dirty files |
| `/media/jer/Files/CCF/Builds` | 3,053,748,224 | Mixed build/unique regression evidence; per-leaf review |
| Main CCF Git store: `/media/jer/Files/CCF/Repository.git` | 4,268,265,472 | PROTECTED shared Git objects/refs, **never include in reclaim estimate** |
| Older CCF/Legacy store and 152 registered worktrees (107 Main + 45 Legacy) | size / unique data UNKNOWN | Not individually qualified, no blanket removal |

`/media/jer/ZX20/CCF-U1`, `CCF-U3`, `CCF-S1-UI-Presentation-V1-Qual`, original CCF Primary, dirty Workbench, frozen Reference, licensed ignored asset families, all user saves and prefs, unique verification evidence and `CCF-Integration/Scenario1V1` untracked retained data are **PROTECTED**. None is a cleanup candidate without a separate user/product-owner decision.

## Admission: short, safe CCF-only preflight

1. Refresh **live** `Docs/Project/coordination/active-tasks.md`, [#383](https://github.com/jermur788/CCF/issues/383), #382/#361, local CCF-Ops lease registry, CCF Primary dirty state and other CCF worktree owners. Confirm no other local CCF cleanup writer or Unity owner conflicts. This task never inherits a Unity or source writer lock.
2. **One quick non-invasive** `findmnt` / `df -B1` observation on `/`, Files, ZX20 and relevant project volumes. Report true free bytes, device/UUID and timestamp, whether user's almost-20-GB observation applies to `/`, and whether headroom is now reasonably stable. No storage changes or heavy commands; if low headroom recurs, STOP local read/scan and return factual status.
3. Use the existing #383 **GitHub docs-only audit branch** `docs/ccf-pre-zx20-storage-migration-audit-20261111-v1` for *new* outputs, via authenticated contents API if available (previous worker proved it), avoiding additional local worktree creation. If local report lane genuinely needed, create a separate clean one only under coordination; never check out/clean Primary or the named CCF candidate worktrees. Mark #383 [RUNNING] with true operator, report branch and actual writer/starting ref **only after** successful admission. No Unity launch, code/source edits or build/import tests.
4. **CCF only:** ignore T3 node_modules, live T3 SQLite/WAL, dashboard runtime and OS cleanup. Those have their own owners and #384 Chat OS review; do not duplicate them. Do not change T3 Fork Integration HOLD.

## Phase A: turn the existing candidate data into a short cleanup decision sheet

**Priority 1 — two older ZX20 Unity Library caches.** For each parent worktree, check branch/full HEAD, current live owner/lease release **today** (historical CCF-Ops release is insufficient), tracked/untracked/ignored files including licensed art and generated `.meta`/UserSettings, last qualifying use, source/Package lock/Editor import availability and whether the exact **Library cache alone** is disposable without touching its dirty parent. Explain warm-state rebuild cost; a clean import in an *existing independently qualified source* is evidence only if it already exists—**do not launch Unity solely to prove regeneration**. Do not delete parent worktrees. If owner/reimport/licence not proven, mark UNCERTAIN with explicit owner question, not "safe".

**Priority 2 — CCF Builds, leaf-by-leaf.** Examine the already measured leaves, including S1B smoke executable (~1.225 billion allocated bytes), S1A legacy evidence, Enlarged80 local outputs and P2. Find relevant handoff/build receipt/manifest and whether each is a unique program witness. Is an independent archive/backup already verified? For each, recommend **PROTECTED / ARCHIVE BEFORE REMOVAL / UNCERTAIN**, or **REGENERABLE LOW RISK only if explicitly proven**. No blanket `Builds/` deletion or moving files.

**Priority 3 — worktrees without inspecting every single one.** Start from registered 107 Main and 45 Legacy worktrees and current task owner/branch/dirty status. **Do not do another full census of all ignored contents**. Identify **at most 10** candidate unused worktrees using explicit lease/claim completion and non-primary/non-personal indicators. For each candidate, prove reconstructibility of its committed source (full HEAD, available refs/objects, independently backed-up remote or archive), and report whether it has ANY valuable local-only, unpushed, staged, dirty, untracked or ignored files, licensed assets or historical test evidence. If these are not proven absent/recovered, retain or ARCHIVE first. Both shared Git stores remain PROTECTED. **Never call `git worktree prune`, `git gc`, `git clean`, `rm -rf` or reset** to test theory.

**Do not move into T3/dashboard cleanups** because the user asked specifically to continue **CCF files**.

For every candidate assemble a **decision-ready row**:
`id,exact_absolute_path,mount,allocated_bytes,apparent_bytes,independent_vs_nested,parent_state,branch_HEAD,live_owner_release,proof_of_nonunique_data,licensed_and_ignored_status,unique_evidence_status,source_rebuildability,backup_or_archive_destination_and_restore_receipt,conditional_max_bytes,confidence,proposed_exact_action,irreversible_risk,rollback,requires_user_approval`.

Group outputs by **ZX20** and **Files**; do not sum overlapping parent/child or linked objects. Distinguish **theoretical conditional ceiling**, **reclaimable once owner/rebuild requirements met**, and **bytes already approved to delete (still zero)**. Where a row is blocked by owner or backup proof, give a short *exact question* for that owner rather than re-running tests.

## Phase B: prepare one itemised user approval gate; stop BEFORE deletion

After the bounded Phase A review, rank candidates by (verified removable bytes, risk, user-playtest impact, cost to rebuild) and provide **one small batch proposal**. The first batch should comprise only independently proven unused artifacts with actual owner release, no unique source or historical evidence, and a tested backup/reconstruction path. A **Unity `Library` in a dirty worktree is NOT automatically safe**; deletion of only its cache would still require owner consent and product/machine-custody checks.

For each candidate in the proposed batch, give an exact absolute path, filesystem/mount, currently allocated bytes, expected *conditional* recovery, effect on Windows migration inclusion, prerequisite backup, how to verify nothing else is running, and precise rollback. Attach an explicit **USER APPROVAL REQUIRED** field with a yes/no choice or item ID. If no candidate meets proofs, report **NO APPROVABLE BATCH YET**, with the smallest missing owner approval/evidence. **Stop and return to Manager #200**. Do not execute cleanup, even if the new free-space measurement is healthy; user's general request to “continue” is not approval of particular destructive paths.

## Outputs and stop conditions

**Existing claim:** [#383](https://github.com/jermur788/CCF/issues/383), reuse as a CCF-only audit continuation. Remains OPEN/PARTIAL until a new evidence result; no new local job/model or new GitHub issue.  
**Report only, exact files:**  
- `Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CLEANUP-APPROVAL-QUEUE.md`.
- `Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CANDIDATE-DECISIONS.tsv`.

Retain all previous Sol/Manager audit artifacts unchanged; add only new report files to the existing docs-only audit branch. Publish actual safe preflight result and observed ownership, path-specific evidence, owner question list, ranked batch or NO APPROVABLE BATCH, and migration impact. Comment the report URL to Manager [#200](https://github.com/jermur788/CCF/issues/200) and #383, release *only* the report writer, and hand off. Do not close the project-wide pre-ZX20 audit as completed merely because this CCF cleanup queue is published. Mark #383 PARTIAL/HANDED-OFF/NOT RUNNING with clear note if residual migration dependencies remain.

**STOP immediately** if unexpectedly low `/` space, active owner conflict, an unverified save/licensed/Reference dependency, any risk of destructive overwrite, or no safe publication route. Do not create large local evidence, new imported Unity Library or transient binaries while working.

**Manager next action after result:** inspect candidate proofs, issue APPROVED / NEEDS CHANGES / REJECTED on proposed first cleanup batch, then request **specific itemized user approval**; only after that may a separate cleanup execution task exist. Current permission does not include cleanup implementation.

**Dashboard**: GitHub coordination updated by Manager as appropriate; downstream ingestion/readback **not verified**.

## One paste-ready dispatch

> Sol / T3 Code (Codex High): Resume **existing #383** for a CCF-ONLY cleanup approval queue using this binding Manager packet: https://github.com/jermur788/CCF/blob/docs/manager-383-ccf-cleanup-resume-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-CCF-ONLY-CLEANUP-RESUMPTION-AND-APPROVAL-QUEUE.md . The user reports nearly 20 GB free after OS-drive cleanup; FIRST verify which mounted volume now has that headroom using a tiny read-only preflight. Do not assume earlier space/owner conditions automatically clear. Use the existing published #383 measurements; inspect only the two older ZX20 Unity Library candidates, individual CCF Builds/evidence and at most ten likely-unused worktrees, including real owner release, dirty/untracked/ignored/licensed assets, Git reconstruction and validated backup before recommending any deletion. No broad duplicate scans, no T3/dashboard work, no Unity. Publish a Markdown+TSV **itemised user approval queue** or explicit NO APPROVABLE BATCH, return Manager #200. **Do not delete, move, transfer, archive, reset, stash, prune, modify source, touch user U3/#382, or lift T3 Fork HOLD. No second reviewer.**
