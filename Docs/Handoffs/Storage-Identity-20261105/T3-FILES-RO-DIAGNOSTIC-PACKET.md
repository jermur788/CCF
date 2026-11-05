# T3 — Durable Files read-only filesystem diagnosis

**Date:** 2026-11-05. **Claim:** [#291](https://github.com/jermur788/CCF/issues/291), **READY / NOT RUNNING**. **Classification:** PACKET ONLY, read-only operational diagnosis. **Difficulty HIGH** because the mounted filesystem is the shared durable verification/backup location; incorrect recovery may destroy evidence or interfere with the user's project. **Best suited:** local T3 Code, OpenAI GPT-6.1 Sol Medium, with Linux storage experience; capacity UNKNOWN. Manager #200 reviews and remains sole writer of live coordination.

## Why this exists and authority

The [#290 diagnostic and attempted-repair handoff](https://github.com/jermur788/CCF/blob/docs/zx20-storage-identity-20261105-v1/Docs/Handoffs/Storage-Identity-20261105/T3-ZX20-STORAGE-GUARD-RECONCILIATION-RESULT.md) identified a stable-by-id guard-design need. User **subsequently authorised the bounded ZX20 guard repair**, but no deployed files changed. The first required backup creation failed with errno 30 at 2026-11-05 17:13:13 UTC. A read-only check reported `/media/jer/Files` mounted as ext4 from `/dev/sdb4` with `ro,nosuid,nodev,relatime,errors=remount-ro`. About 88 GiB remained. A recent journal inspection did not establish a cause. This packet authorises **only diagnosis of the Files volume**. It does not extend the ZX20 repair authority to mount, filesystem or hardware changes.

Related tasks: [#289 context publication](https://github.com/jermur788/CCF/issues/289) **OPEN/BLOCKED**, [#287 Unity qualification](https://github.com/jermur788/CCF/issues/287) **WAITING** for explicit user-playtest release and a separate valid exclusive Unity lease. Preserve D-071 five-parallel-rack Scenario One, old saves, Legacy40, frozen Reference Future v1 and the user's personal playtest.

## Lane and ownership

- **Local inspection lane:** existing `/media/jer/ZX20/CCF-Ops`, observational/read-only only. It is an explicit-copy deployment, **not a Git worktree**. Do not create a game clone, copy verification artifacts to an alternative storage volume or modify deployed ops scripts.
- **GitHub report-only branch:** `docs/files-ro-preflight-20261105-v1`, based on current main for this documentation. No production changes. Current branch/repository HEAD must be inspected and recorded at task start; do not assume an unowned local checkout.
- **Expected durable handoff:** `Docs/Handoffs/Storage-Identity-20261105/T3-FILES-RO-PREFLIGHT-RESULT.md` on that branch. Include privacy-safe evidence and exact commands/outputs.
- Refresh [live active-tasks](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), the open claim and #290; confirm no overlapping worker/owner before the investigation. Mark #291 [RUNNING] only on a real start, naming provider/model, exact lane, report branch and initial repository HEAD. If ownership conflicts, mark [BLOCKED] and stop.

## Phase A — mandatory read-only observations

1. Independently confirm mountpoint, source, filesystem, UUID, mount options, underlying physical device and stable by-id identity of **both** Files and ZX20; distinguish device names from persistent identities. Record whether only the Files volume is read-only and whether writable mounts depend on it. Do not write a test file.
2. Inspect recent relevant kernel and system logs (journalctl/dmesg if accessible without escalation) for ext4 errors, I/O faults, journal aborts, drive disconnects, intentional administrator remount or boot-time fstab/systemd options. Record dates/times and reliability. No error seen is not proof of filesystem health.
3. Inspect read-only device/partition/mount/space/SMART evidence **only when safe and accessible without privileged device mutation**. SMART probes may be omitted if they would wake, reset or otherwise disturb the device; do not launch a self-test.
4. Examine any documented storage owner, backup location, provenance and previous known mount configuration using read-only paths. Check active processes, mounted dependent folders and expected durable verification receipts without opening private content unnecessarily. Record whether a safe repair requires downtime and which owner must consent.
5. Test that relevant ops scripts/config remain untouched using read-only hashes where available, and ensure #290's prior diagnosis of the ZX20 literal-device check is not silently reclassified as repaired.

## Hard stops and exclusions

**No** `mount -o remount,rw`, `fsck`, `e2fsck`, device repair, `tune2fs`, write SMART self-test, `sudo` repair, storage replacement, data migration, volume formatting, backup redirection to ZX20, bypassed guard, forced lease, process termination, Unity, Windows build, gameplay/scene/save modifications, context regeneration, Drive publication or platform attachment update. Do not restore Files writes opportunistically; that requires a **separate owner-approved recovery plan** after diagnosis.

If dangerous errors, active user work, ownership uncertainty or unavailable log access is discovered, stop and report rather than invent cause or repair. Do not weaken tests or trade data safety for unblocking #289.

## Verification and handoff

The result must distinguish **observed facts, hypotheses and unknowns**; list exact mount/device/log evidence with timestamps; describe whether the ro state appears configuration-driven, kernel-protective or indeterminate; give a smallest safe recovery proposal with preflight, backup, rollback and downtime implications **without executing it**; identify the necessary owner/user authorisation. Include no local filesystem writes, no Unity lease and no gameplay/build changes as explicit checks. Record remaining blockers for the already-authorised ZX20 stable-by-id guard repair.

Publish the GitHub Markdown handoff, read it back, mark #291 [HANDED-OFF] and close after returning. Manager issues an APPROVED / NEEDS CHANGES / REJECTED diagnostic verdict, then authors any separately authorised recovery packet if evidence warrants it.

**Copy/paste dispatch:** T3 GPT-6.1 Sol Medium — take #291 read-only durable Files-volume diagnostic using https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Storage-Identity-20261105/T3-FILES-RO-DIAGNOSTIC-PACKET.md . Refresh live coordination and claim, inspect `/media/jer/Files` mount/physical identity/logs/owner without writing or changing mount state, and publish `Docs/Handoffs/Storage-Identity-20261105/T3-FILES-RO-PREFLIGHT-RESULT.md` on `docs/files-ro-preflight-20261105-v1`. No remount/fsck/repair, no guard bypass, Unity, context publication or Windows build. Stop if ownership conflicts; return evidence and an owner-gated proposal only.

**User action:** paste the dispatch to the existing trusted local T3 ops worker. No filesystem-repair permission is implied.