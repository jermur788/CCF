# Manager #200 — #383 partial measured pre-ZX20 storage audit disposition

**Date:** 11 November 2026, after local operator's 20:34 UTC filesystem census.  
**Manager verdict:** **APPROVED AS A PARTIAL READ-ONLY INVENTORY / NEEDS COMPLETION FOR CLEANUP OR MIGRATION DECISIONS**. No single candidate is independently proven recoverable; **cleanable bytes authorised = 0**, **unique recoverable bytes = UNKNOWN on all volumes**. This is NOT permission to delete, move, archive, copy, migrate, run cleanup tests, change databases, release T3 locks or modify personal Unity lanes. **T3 Fork Integration HOLD remains binding.**

## Evidence inspected on GitHub

- [Sol #383 report: PARTIAL](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md).
- [Sol #383 itemised TSV manifest](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-MANIFEST.tsv).
- [Sol #383 protected register](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-PROTECTED-REGISTER.md).
- [Sol #383 observed volumes/mounts](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-MOUNT-AND-STORAGE-SUMMARY.md).
- Claim [#383](https://github.com/jermur788/CCF/issues/383) remains OPEN / PARTIAL / NOT RUNNING. Report-only GitHub publication used; no local report worktree/source edit/migration was claimed.

Manager reviewed these documents; all local sizes, mount options, Git status/paths and process observations are **operator-provided evidence**, not measurements rerun by Manager on the user's machine. Unmounted/uninspected stores remain UNKNOWN.

## Measured baseline (not savings)

| Volume | Used bytes | Available bytes | Interpretation |
|---|---:|---:|---|
| Internal `/` ext4 | 236,315,041,792 | **1,267,175,424** | **Urgent capacity risk** for ordinary system/runtime operations. No automatic T3 cache deletion permitted. |
| ZX20 exFAT | 482,487,566,336 | **29,587,668,992** | Constrained space for the proposed Windows migration; more source/destination sizing and backup plan required. |
| Files ext4 | 267,268,935,680 | 205,980,205,056 | Could be backup *candidate capacity* only; it has active source/dirty data and no owner-approved backup allocation. |
| Videos ext4 | 381,404,110,848 | 62,081,433,600 | Project ownership and spare-capacity eligibility not established. |

These are a 2026-11-11 20:34 UTC snapshot of `df -B1`, not promises of continuing free space. Ext4 reserved blocks/metadata and exFAT allocation units require care; no cross-volume reclaim total.

## Accepted findings and protected areas

1. **ZX20 Unity Libraries**: MainIntegration 7,216,824,320 allocated; UIQualification 7,210,270,720; U1 7,250,771,968; U3 7,270,694,912; personal UI Qual 7,012,089,856. Large caches exist, but none passes complete owner/asset/actual regeneration proof. **U1, U3 and #382 personal qualifier protected; MainIntegration parent dirty with eight tracked changes.** Cache size cannot be counted as recoverable.
2. **T3 `node_modules`**: 3,831,660,544 allocated, but active T3 consumers, pnpm store/link relationships, pinned toolchain and offline rebuild not verified. Root system disk is critically constrained; T3 and runtime records stay protected during audit.
3. **CCF Builds**: 3,053,748,224 allocated; individual binaries/captures may be unique QA or qualification evidence. Classify UNCERTAIN until per-build provenance and independent backup.
4. **Live T3 userdata**: 5,842,542,592 allocated; current runtime including SQLite/WAL/SHM, attachments, logs and browser state. Do not truncate, copy live database as consistent backup or mark regenerable. Logs 747,188,224 and server-browser 200,589,312 are nested inside parent userdata—**do not sum**.
5. **CCF Git store**: 4,268,265,472 allocated; shared by worktrees. Report's 1,855 commits reachable under local refs but not remote refs is not a list of safe unpushed branches or proof of independent backup. Preserve all refs, objects, stashes, detached heads and reflogs.
6. **Unique and dirty state**: Primary's `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` pre-existing changes; eight dirty Main Integration/U3 paths; `CCF-Integration/Scenario1V1` 598,867,968 allocated untracked; licensed ignored assets, frozen Reference Future, personal saves/Prefs, T3 B2.1 staged changes, separate dashboard repo and live runtime. **Retain.**
7. **Cross-project dependencies not closed**: Overall-Dev/dashboard ingestion and state, T3 active owner, B2.1 and migration protocol, source project backups, Windows/WSL portability, secure credentials handback and destination backup capacity.

**No registered Git worktree has proven safe deletion or reconstructibility.** Worktree and its linked object store/ignored files must be assessed separately. A cache called regenerable in theory is not necessarily economical to discard before a player test.

## Manager risk and priority

**Immediate operations warning:** Internal root's 1.27 GB available is low; avoid long logs, package downloads, import/cache creation or new data on `/` until the operator confirms safe headroom. This warning **does not authorise any deletion**. Ask the T3 owner before inspecting active runtime retention; never truncate live logs/database/browser profiles.

**Prioritise remaining audit, not another general crawl:** Resolve ownership/rebuild/recovery of the **two apparently inactive ZX20 Unity Libraries** (MainIntegration and UIQualification) without touching their parent source/dirty content; inventory CCF Builds and worktree unique branches/ignored data; establish T3 node_modules/shared pnpm store/rebuild feasibility; confirm current T3/dashboard owner and consistent runtime backup plan; determine destination Windows storage/backup capacity. If owner cannot be reached or backup cannot be verified, leave candidate UNCERTAIN/PROTECTED and quantify **conditional**, not unconditional, savings.

### Evidence correction guard

Manifest rows have `unique_reclaimable_bytes_min=0` and `max=UNKNOWN`. This is appropriate. Do not simply total 7.22+7.21+3.83 GB to claim reclaim. If a later audit establishes non-overlapping standalone caches on one mount with owner release and reproducible import, it may report *conditional recoverable* space separately from *approved-to-delete* (still zero until user approves). Resolve apparent/apportioned allocations and live file handles as needed.

## Next executable action

Keep #383 **OPEN / PARTIAL**, no new claim or model review. The [bounded targeted read-only completion packet](MANAGER-383-TARGETED-CANDIDATE-OWNERSHIP-AND-MIGRATION-CLOSURE.md) specifies which missing findings will materially change the user decision, caps repeated broad scans and provides honest stop conditions. Its report becomes an addendum to the existing audit and manifest on docs-only branch. Return to Manager #200 when a candidate owner/rebuild/backup proof is established, or when a critical unknown is genuinely blocked.

**Approval gate:** Only Manager review of the completed report can create a proposed, item-by-item user decision; *only user approval of each destructive batch* may authorise later cleanup or migration. Separate Windows/T3 HOLD decision required. The audit alone never authorises destructive work.

**Status of integrations, user data and dashboard:** CCF #382 personal Editor test and #361 U3 still protected and not user accepted. No production source/Unity modification, migration or T3 Fork integration. Dashboard downstream ingestion/readback remains **UNVERIFIED**.
