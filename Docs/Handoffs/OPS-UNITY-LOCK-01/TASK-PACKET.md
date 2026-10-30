# OPS-UNITY-LOCK-01 — Repair stale Unity lock configuration

**Date:** 2026-10-30  
**Claim:** #189  
**Parent dependency:** #156 MFM-08 Tier-2 Unity  
**Status:** RELEASED — LOCAL OPS REPAIR  
**Difficulty:** 1/5  
**Preferred executor:** Ultra Code

## Purpose

Restore the existing authoritative machine-wide Unity lock mechanism after the local storage migration so Unity qualification jobs can resume without bypassing coordination policy.

This is an ops/config repair only. It is not part of MFM-08 implementation or verification.

## Reported blocker to re-confirm locally

Ultra Code reported during #156 preflight:

- `unity-lock` error: ZX20 mount identity mismatch;
- live mount source `/dev/sdd2`, target `/media/jer/ZX20`, fstype `exfat`, UUID `18D6-896E`;
- `/media/jer/ZX20/CCF-Primary/CCF-Ops/lanes.json` still expects `/dev/sda2`;
- local storage README says the canonical bare repository moved on 2026-10-21 to `/media/jer/Files/CCF/Repository.git`;
- `CCF-Ops/state/unity/` does not currently exist;
- no Unity Editor processes were observed, but that does not establish a free lock.

Treat those as reported local evidence until independently re-confirmed by this repair task.

## Exact authority

You are authorised to make the **smallest shared ops/config correction required for the existing `unity-lock` mechanism to operate normally**.

Before editing:

1. Refresh #189, #156 and `Docs/Project/coordination/active-tasks.md`.
2. Re-confirm the live ZX20 mount source/target/filesystem/UUID using the normal system tools.
3. Read the authoritative local storage README and confirm the current canonical repository path.
4. Inspect the existing `unity-lock` implementation/config schema and determine which fields actually drive mount identity, repo identity and state location.
5. Preserve the existing lock mechanism and shared state semantics. Do not create an alternate/private lock system.
6. Post `[RUNNING — OPS]` on #189 with provider/model and the config/state paths being examined. Do not mark #156 running.

## Allowed changes

Only the minimum required local ops configuration/state bootstrap to make the established lock tool authoritative again, for example:

- correcting a stale device-source field while preserving the verified UUID/mount identity;
- correcting the canonical repository path after confirming the storage migration record;
- allowing the normal lock tool to initialise its documented shared state directory if that is its intended behavior.

Do not blindly substitute the values above. Re-confirm them first and follow the actual config schema/tool behavior.

## Forbidden

- no Unity launch under #189;
- no gameplay, UI, verification-test or project-content edits;
- no MFM-08 production repair;
- no bypass flag, manual fake lock file or alternate state directory chosen merely to get green;
- no deleting/overwriting an existing holder record;
- no killing Unity processes;
- no Windows build;
- no broad storage migration beyond what the lock tool requires.

If an authoritative existing lock holder appears at any point, stop and report it.

## Verification required

After the minimal correction:

1. Run the normal `unity-lock` read/status/preflight path without bypasses.
2. It must return an authoritative state: either an identified current holder or an explicit free state according to the tool itself.
3. Confirm the tool resolves the correct mount UUID and canonical repository identity.
4. Confirm the expected shared state root used by the normal tool.
5. Do not acquire the #156 Unity lock under this ops claim unless the tool's documented repair/diagnostic flow itself requires a harmless acquire/release self-test. If such a self-test is necessary, use a clearly named OPS test owner, release immediately, and record the exact receipt.
6. Re-run status after any self-test and prove the final state is not left owned by OPS.

## Required handoff

Publish:

`Docs/Handoffs/OPS-UNITY-LOCK-01/HANDOFF.md`

Include:

- exact before/after config fields relevant to the repair (no secrets);
- live mount/UUID evidence;
- storage README/current-repository evidence;
- exact lock-tool commands and outputs sufficient to establish authoritative status;
- state-root behavior;
- any acquire/release self-test receipt;
- final lock state;
- rollback note;
- confirmation that #156 production/worktree files were untouched.

Expected success token:

`UNITY LOCK GOVERNANCE RESTORED — #156 MAY RE-PREFLIGHT`

If the normal tool cannot establish authoritative state after the bounded repair:

`BLOCKED — UNITY LOCK AUTHORITY STILL UNRESOLVED`

## Resume rule for #156

A successful #189 handoff does **not** automatically start MFM-08. After Manager review/acceptance of #189, Ultra Code must return to #156 and repeat the entire fresh preflight: active-tasks, branch/head/worktree cleanliness, free space, Editor processes, current authoritative lock status and lock acquisition. Only then post `[RUNNING — UNITY]` and begin Tier-2.

## Copy/paste dispatch line

`Run OPS-UNITY-LOCK-01 from Docs/Handoffs/OPS-UNITY-LOCK-01/TASK-PACKET.md under claim #189. Re-confirm the live ZX20 UUID/mount, authoritative local storage README/current bare-repo path, unity-lock implementation/config schema and shared state root. Make only the smallest ops/config correction needed for the existing lock tool to operate normally; do not bypass it, invent a private lock, launch Unity, touch gameplay/project production files or build Windows. Post [RUNNING — OPS] on #189, verify the normal unity-lock status path returns an authoritative holder or explicit free state, publish Docs/Handoffs/OPS-UNITY-LOCK-01/HANDOFF.md, and return UNITY LOCK GOVERNANCE RESTORED — #156 MAY RE-PREFLIGHT or the exact blocker.`

## User action required

Dispatch the line above to the same Ultra Code worker that stopped #156 preflight. After it returns, Overall Manager reviews #189 before #156 resumes.