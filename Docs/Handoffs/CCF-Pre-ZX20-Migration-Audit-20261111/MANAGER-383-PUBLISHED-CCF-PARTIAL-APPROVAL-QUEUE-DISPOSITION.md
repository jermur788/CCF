# Manager #200 — Published CCF-only cleanup queue accepted as PARTIAL, no removal approved

**Date:** 11 November 2026, after 23:18 UTC GitHub publication.  
**Verdict:** **APPROVED AS DURABLE PARTIAL EVIDENCE; NEEDS COMPLETION BEFORE AN APPROVABLE CLEANUP BATCH.**  
**Execution claim:** [#383](https://github.com/jermur788/CCF/issues/383) remains **OPEN / PARTIAL / NOT RUNNING** after report delivery.  
**Deletion/migration authority:** **NONE**. No cache, build, worktree, original game, archive, save or runtime data may be removed under this verdict. T3 Fork Integration HOLD remains binding.

## Evidence directly inspected

- [Sol's published CCF-only partial report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CLEANUP-APPROVAL-QUEUE.md).
- [Sol's exact candidate TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CANDIDATE-DECISIONS.tsv).
- Earlier [original audit](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md), [targeted recovery addendum](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-TARGETED-OWNER-AND-RECOVERY-ADDENDUM.md), and [Manager stop disposition](MANAGER-383-TARGETED-ADDENDUM-STOP-AND-OWNERSHIP-DISPOSITION.md).
- **Manager directly fetched and read the latest GitHub Markdown/TSV**, and checked GitHub claims [#275](https://github.com/jermur788/CCF/issues/275), [#277](https://github.com/jermur788/CCF/issues/277), [#361](https://github.com/jermur788/CCF/issues/361) and [#382](https://github.com/jermur788/CCF/issues/382). Local publication byte-for-byte comparison, exact filesizes and runtime process/mount state are the local worker's reported verification, **not** independently rerun from Manager's browser.
- Claim #275 is closed/handoff; #277 is a closed accepted **test-only** repair. Neither gives perpetual exclusive filesystem authority or proves every locally ignored asset can be regenerated. Their previous Unity lease releases are historical facts, not necessarily *current* owner release.

## Accepted classification and implications

| Candidate | Allocated bytes observed | Manager decision |
|---|---:|---|
| ZX20 older UI qualification `CCF-UIQualification-20261104/Library` | 7,210,270,720 | **CONDITIONAL CANDIDATE** Z2; parent has eight benign-seeming modified files. Last reported session release 4 Nov; refs present, but owner retirement and ignored/licensed source plus rebuild path not fully confirmed. |
| ZX20 older main integration `CCF-MainIntegration-20261104/Library` | 7,216,824,320 | **CONDITIONAL CANDIDATE** Z1; same unresolved release/rebuild requirements. |
| Older playable `CCF-U3` user work | 7,270,694,912 Library only | **PROTECTED** (#361); old source behind newer, but shared saves/Prefs and user acceptance unresolved. |
| Newer UI-updated `CCF-S1-UI-Presentation-V1-Qual` user test | 7,012,089,856 Library only | **PROTECTED** (#382). Preferred for next personal test, not yet user-accepted; preserve warm Editor and save isolation. |
| Files S1B smoke executable | 1,225,150,464 | **ARCHIVE BEFORE REMOVAL**. Verification summaries refer to it; no separately proven validated archive. |
| Files `S1-80m-local-output` | 1,260,412,928 | **UNCERTAIN**. Find owner/receipt before categorising. |
| Files Legacy evidence | 556,089,344 | **PROTECTED**. |
| P2 small local output | 3,145,728 | No useful cleanup priority. |

Old two Libraries sum to **14,427,095,040 bytes allocated (~14.43 GB decimal), NOT proven unique recoverable bytes and NOT approved deletion**. Refs/ancestry and harmless-looking Unity float reserialization do **not** prove local ignored/licensed data, backup, import state, own source metadata and personal save data are entirely reconstructible. A cache could be deleted alone without altering parent code *only after* owner/usage release and reimport-cost acceptance; even then user must explicitly approve the exact path.

**Latest local volume snapshot** per report's 23:03 UTC observation: root 19,826,892,800 bytes available, Files 182,951,198,720, ZX20 26,788,233,216. No Unity process seen at that time. These numbers do **not** imply an available Unity lock, verified owner release, or safe-to-delete amount. Filesystem root headroom crisis reported earlier appears improved, but require fresh verification before heavy local work.

## Outstanding work and next decision gate

The #383 report correctly marks **NOT RUN**:
1. Narrow screening of up to ten potentially inactive CCF worktrees among main/Legacy stores, including clean/dirty/untracked/ignored, reachable and independently preservable Git history, actual owner release and exclusive cache bytes.
2. S1B and other Builds independent backup/archive receipts and restoration/evidence role; no Linux smoke binary deletion until verified.
3. Windows migration inclusion/exclusion and destination capacity/backup.
4. Actual separate Unity launch and **save/PlayerPrefs equivalence**, which should not be scheduled merely for cleanup; rely on user personal playtest/approved evidence as a later gate instead.

**Manager stance:** Do not create a second cleanup claim or ask another provider to re-audit the four ZX20 workspaces. Under #383, the local worker can later complete the **remaining documentation-only** worktree and Builds checks, after refreshing current owner and disk. If owner retirement of Z2/Z1 cannot be established, obtain explicit current owner/CCF Ops confirmation, not blanket "released" based solely on past issue closure. When sufficient, assemble an **exact first candidate batch for the user's separate explicit approval** and a tested recovery/rollback plan. Do not execute it now. Since the user has not personally accepted the newer UI copy, keep both personal Scenario One worktrees and warm caches.

No claim of reclaimable bytes should be promoted beyond **conditional maximum**. Realisable and user-authorised savings remain **UNKNOWN and ZERO approved**, respectively.

**Status:** No production source changes, no Unity run, no cleanup, archive or migration. #383 remains OPEN/PARTIAL after published result; ownership and integration HOLD preserved. [Manager #200](https://github.com/jermur788/CCF/issues/200) is the accepting authority for any later task and user-facing cleanup decision.

**Dashboard:** GitHub publication/coordination are distinct from dashboard ingestion and readback; downstream synchronization remains UNVERIFIED.
