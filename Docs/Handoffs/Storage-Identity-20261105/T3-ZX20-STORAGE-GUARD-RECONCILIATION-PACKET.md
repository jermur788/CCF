# #290 — Safe ZX20 storage-identity guard reconciliation

**Date:** 2026-11-05. **Status:** READY / NOT RUNNING.  
**Classification:** PACKET ONLY — bounded machine-wide operations **read-only diagnostic first**, conditional **config-only** repair after all safeguards. Never a Unity or game task.  
**Difficulty:** MEDIUM–HIGH because the identity predicate protects shared storage and Unity lease tooling; incorrectly editing it can silently misidentify a disk or interfere with the user's ongoing playtest.  
**Best suited:** local T3 GPT-6.1 Sol Medium with trusted Linux/CCF-Ops access, independent Manager review. Provider quota and exact runtime are UNKNOWN.  
**Claim:** [#290](https://github.com/jermur788/CCF/issues/290). Do not mark [RUNNING] until genuinely started with owner/model, local lane and inspected starting HEAD where applicable.  
**Local lane:** the **existing authorised operations environment** at `/media/jer/ZX20/CCF-Ops`, read-only initially; **no new CCF game clone, worktree or Unity session**. No assumption about the current owner or permissions; inspect live coordination and obtain the actual operations owner’s exclusive confirmation for any write. Never write another owner's dirty work.  
**GitHub result branch:** `docs/zx20-storage-identity-20261105-v1`, created for report publication only; isolate any CCF-Ops local modification from repository game files.  
**Exact return:** `Docs/Handoffs/Storage-Identity-20261105/T3-ZX20-STORAGE-GUARD-RECONCILIATION-RESULT.md` on the result branch (link in [#290](https://github.com/jermur788/CCF/issues/290)).  
**Related blocked task:** [#289 context publication](https://github.com/jermur788/CCF/issues/289), with [worker diagnostic](https://github.com/jermur788/CCF/blob/docs/context-publication-d071-20261105/Docs/Handoffs/ProjectContext-20261105/CONTEXT-PUBLICATION-D071-RESULT.md). **Other impacted gate:** #287 workbench Unity preflight, but #287 retains the independent **user playtest release AND machine-wide Unity lease** requirements.

## 1. Precisely observed failure — not a diagnosis of physical disk substitution

Worker #289's read-only preflight reported:
- `findmnt -T /media/jer/ZX20 -o TARGET,SOURCE,FSTYPE,UUID`: `/dev/sdc2`, `exfat`, UUID `18D6-896E`. `lsblk` named NetacPortableSSD. The worker reported ~34 GiB free.
- `/media/jer/ZX20/CCF-Ops/lanes.json` expects `/dev/sda2`, UUID `18D6-896E`, exFAT and at least 15 GiB free.
- Sourcing `environment.sh` exits nonzero with `ValueError: ZX20 mount identity mismatch`. No repair, clone or generated context occurred.
- Matching UUID and filesystem while Linux `/dev/sd*` enumerated device differs suggests **possible enumerator renaming**, not definitive physical-drive proof. It may alternatively indicate misconfiguration, duplicate IDs, another mount, unexpected symlink or changed storage. Do not call this harmless or assert the actual correct code modification from these findings alone.

**Manager STOP:** do not change `/dev/sda2` to `/dev/sdc2` based solely on matching UUID, do not set skip/bypass flags, change a mount, force/reset a Unity lease, kill processes or overwrite shared ops config without establishing safe identity and exclusive ownership.

## 2. Phase A — mandatory read-only Linux/CCF-Ops source and ownership inspection

1. Refresh [live ownership](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), #289 and #290. Confirm who owns deployed CCF-Ops files, whether any local process/user currently depends on them, whether this is on a user-in-progress playtest machine, and any lock leases/processes. Record process/lease facts, not assumptions. If ownership overlap, mutation STOP; read-only report may continue.
2. With **read-only** commands, collect `findmnt` and `lsblk -f/-o NAME,PATH,MODEL,SERIAL,UUID,FSTYPE,MOUNTPOINTS`, `blkid` for the relevant partition, `readlink -f` of the mount source and persistent `/dev/disk/by-id` and `/dev/disk/by-uuid` links. Identify one unambiguous physical disk partition using **stable serial/by-id plus matching expected filesystem UUID and mount target**. Check duplicates and unexpected alternate mounts, whether source is actually the same hardware reported previously; capture result with privacy-safe serial redaction or hash.
3. Inspect but **do not source/run as a bypass** the actual deployed `CCF-Ops/lanes.json`, `environment.sh` and exact Python/bash guard code called from it. Record the **specific identity predicate** that rejects the mount, other invariants it enforces, any safe documented storage re-registration procedure, version/provenance/dirty state and whether a config-only update is even supported. Keep secrets, tokens and private host details out of GitHub.
4. Measure disk availability, any unmounted filesystems, active editor/build/lease processes and config owner; no force unlock, no device remount, no root permission escalation as a shortcut. Read existing `Docs/Operations/StorageArchitecture.md` *if present locally* while preserving uncommitted changes there. It was **not confirmed available in GitHub main**; do not overwrite it based on an imagined published version.
5. Distinguish *physical device stable identity* from volatile Linux enumerator path. Prefer an **existing, documented, fail-closed stable identity mechanism** supported by deployed CCF-Ops (by-id path, UUID plus unique physical identity/mount target) over `/dev/sdX` names. Do not weaken two or more independent checks into UUID-only matching.

**Phase A deliverable:** a concrete source-backed account of what exactly failed and whether the device should be treated as the same authorised physical disk; name all tested predicates, current owner, active leases and the smallest safe repair.

## 3. Phase B — narrowly authorised configuration repair only IF safe

**All must be true before any write:**
- One-to-one physical disk proof (stable by-id/serial or equivalent), correct expected UUID, filesystem, exact mount target and no duplicate/unexpected other device.
- Inspected guard source confirms a **documented config-only** safe re-registration path retaining fail-closed checks. A raw substitution of one volatile `/dev/sdX` string for another without persistent identity assurance **is NOT sufficient**.
- The actual CCF-Ops configuration owner grants exclusive temporary write ownership, no active dependent lease/session would be disrupted, target files are backed up, differences are inspected, no unrelated dirty modifications are absorbed, and the work can be atomically reverted.
- No Unity Editor or user-playtest process may be interrupted. If the machine is actively owned or read-only verification insufficient, STOP at Phase A and return **BLOCKED — CONFIG CHANGE REQUIRES OWNER OR NEW DESIGN**.

If all true, edit **only the documented storage identity configuration keys** through the accepted CCF-Ops operational registration method, not shared guard script logic, with a reversible backup. Verify by non-invasive `environment.sh`/identity check and an **independent negative check** showing that a mismatched wrong mount/UUID/physical source is still rejected without touching real mount/device configuration (use supported safe test fixture or static predicate proof; never intentionally remount/masquerade a disk on the live host). Record before/after config hash and test receipts, with no active Unity/lease changes. Any scripting refactor, change of CCF-Ops invariant, remount, service restart, broad Git mutation, active lease override or manual forced guard bypass **requires another explicit Manager-authorised scope**; STOP and propose it.

## 4. Result and subsequent gates

Return an explicit **REPAIRED/VERIFIED**, **DIAGNOSED/NO WRITE**, or **BLOCKED** operations verdict; do not use Unity or a clone to test it. Publish a durable GitHub handoff containing source paths/line evidence, mounted physical identity checks (redacting device serial), known config provenance, sole writer/lease state, exact allowed mutations or reasons no write, hash/test receipts and rollback, and whether the safe guard runs end to end. Worker mark #290 [HANDED-OFF] and close only once the result is published; return result for Manager acceptance before #289 can be unblocked.

- Even **REPAIRED** does not itself resume #289: Manager first inspects the report and confirms a fresh guard pass and safe clone ownership, then dispatches existing #289 with its original hash/Drive/attachment requirements and a fresh source SHA.
- Even **REPAIRED** does not unblock #287 by itself: user must **explicitly finish/release the personal playtest** and the local worker must obtain a new valid **exclusive machine-wide Unity lease** with its own safety preflight.
- No generated context, manifest, Drive publication, platform attachments, Unity, game/scene/save file, user playtest, Windows build or global ops policy modification falls within #290.

**User-visible dispatch, paste ready:**

> T3 GPT-6.1 Sol Medium — run CCF storage-identity reconciliation [#290](https://github.com/jermur788/CCF/issues/290), using the exact packet https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Storage-Identity-20261105/T3-ZX20-STORAGE-GUARD-RECONCILIATION-PACKET.md . Start read-only: inspect the actual mounted ZX20 hardware/by-id, UUID, exFAT, expected mount, active CCF-Ops scripts/config, live ownership and leases. Do **not** simply swap `/dev/sda2` for `/dev/sdc2`, bypass checks, touch Unity or the playtest. A documented **config-only** fail-closed re-registration is conditionally authorised **only after** unambiguous stable physical identity, exclusive ops ownership and no process/lease impact; otherwise return BLOCKED and an exact repair proposal. Publish `T3-ZX20-STORAGE-GUARD-RECONCILIATION-RESULT.md` on `docs/zx20-storage-identity-20261105-v1`, link #290 and stop. Do not resume #289 or #287 without separate Manager dispatch.

**User action required:** paste the dispatch to an authorised local ops worker. No personal-playtest or build permission is needed for read-only diagnosis; if active operations ownership cannot be cleared, the worker stops safely.
