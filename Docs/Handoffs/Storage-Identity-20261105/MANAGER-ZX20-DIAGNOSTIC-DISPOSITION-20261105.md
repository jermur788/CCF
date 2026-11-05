# Manager disposition — ZX20 storage diagnostic and Files read-only blocker

**Date:** 2026-11-05. **Overall Manager / Browser Lane #200.**
**Source:** [#290 result](https://github.com/jermur788/CCF/blob/docs/zx20-storage-identity-20261105-v1/Docs/Handoffs/Storage-Identity-20261105/T3-ZX20-STORAGE-GUARD-RECONCILIATION-RESULT.md), [#290 claim/comments](https://github.com/jermur788/CCF/issues/290); [#289 blocked context publication](https://github.com/jermur788/CCF/issues/289).

## Formal verdict

**APPROVED — DIAGNOSTIC AND SAFETY STOP ONLY. NOT REPAIRED, NOT VERIFIED, NOT INTEGRATED.**

The published evidence directly inspected the mounted Netac by-id aliases (one visible physical partition), exFAT, UUID, mount target and current deployed predicates. A literal expected `/dev/sda2` conflicts with observed `/dev/sdc2`; the currently observed internal root partition at the old enumerator path further makes a raw string substitution unsafe. Neither UUID match nor read-only mount inspection proves historical physical-serial authorisation. The deployed copy differs from recorded deployment hashes for several components; no automatic revert/redeployment or clean-source provenance may be assumed. No exclusive operations writer was established in the initial diagnostic.

#290's diagnostic outcome is valid and closed as **HANDED OFF**. Its issue title saying READY was stale and must be corrected in status surfaces. The return did **not** clear storage, Unity or publication gates.

## Later observed event — bounded guard repair authorised but stopped

After the diagnostic, the T3 worker recorded **explicit user authorisation of a persistent-by-id guard correction**. That supersedes the initial *scope-authorisation* blocker for that bounded design; it does not certify exclusive operations ownership, deployment provenance, hardware history or backup health. Before touching deployed files, creation of the required durable backup directory failed with `errno 30: Read-only file system`. The later handoff records `/media/jer/Files` as ext4 on `/dev/sdb4`, mounted read-only with `errors=remount-ro`. Root cause is **unknown**; available free space does not cure a read-only mount.

**No local repair occurred.** The worker reported unchanged deployed hashes, no remount, fsck, force-release, Unity process action or game change. This return is **APPROVED AS A SAFE STOP / BLOCKED BY DURABLE FILES READ-ONLY**. A prior comment marking repair RUNNING is historical and must not be displayed as an active repair. Do not simply retry a backup on another volume or silently remount Files.

## Next executable step

**[#291 — Files volume read-only diagnosis](https://github.com/jermur788/CCF/issues/291)** is a separate read-only diagnostic claim. Its [packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Storage-Identity-20261105/T3-FILES-RO-DIAGNOSTIC-PACKET.md) requires stable identity, logs, mount and ownership checks without write, remount, filesystem repair, privilege escalation or changing the user's playtest. **Status: READY, NOT RUNNING** until the T3 worker actually starts it. Best route local T3 Code GPT-6.1 Sol Medium; existing ops environment is an inspection lane, not an acquired writer lock. Difficulty HIGH due to shared durable backup data.

After #291 Manager review, any actual Files filesystem recovery requires **a separately owner-approved and safety-reviewed plan**. Only after safe durable backup availability, explicit exclusive ops write authority, preservation/reconciliation of deployed drift and fresh hardware/lease preflight should the previously user-authorised stable-by-id guard fix be reattempted under its own bounded, non-overlapping execution claim/packet. Keep target path, unique UUID, filesystem, rw, free-space and physical identity fail-closed checks, independently test negative cases and review source/hashes before any claim of REPAIRED.

## Downstream locks

- **#289:** OPEN/BLOCKED. No clean one-context generation, manifest/hashes, permanent Drive mirrors or project attachment replacement. Current static attachments are historical, not D-071-current. Requires a new Manager release after verified safe guard repair and context-lane preflight.
- **#287:** WAITING. Even a verified storage repair cannot release it without the user's explicit personal-playtest completion/release **and** a fresh valid machine-wide exclusive Unity lease. #284 remains source-reviewed only, not integrated/Unity-qualified.
- **D-071:** Existing southern-headland entrance with five parallel Scenario One racks; no central spine/herringbone/road construction. No game/scene/saves/reference/Windows changes authorised.
- **Coordination:** #200 is sole primary writer. Worker claims record task presence, not exclusive ownership.

## Dashboard and closure

Update live `active-tasks.md` and the Overall-Dev dashboard source to show #290 diagnostic handed off, guard repair stopped on Files read-only, #291 read-only diagnostic READY, #289 BLOCKED and #287 WAITING. A GitHub file readback confirms source publication only; **automated dashboard ingestion and downstream readback remain NOT VERIFIED** until separately established. No generated dashboard state is to be hand-edited.

**Manager user action:** paste #291's dispatch to trusted local T3 ops worker; no authority for Files filesystem repair is granted by this review. Separate user playtest and Windows-build permissions remain outstanding.