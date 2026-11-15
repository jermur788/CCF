# Recorded command transcript

Commands, exit codes, stdout and stderr from isolated worktree preflight onward. Full unfiltered transcript: /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/commands.md. Initial read-only reconnaissance precedes the recorder and is retained in the T3 session, with its operational observations in bootstrap.md below.

Raw live board text and extracted board rows are intentionally kept out of Git, per packet. Their transcript entries link the full external output with its SHA-256. No original lock-state or ledger files are included; only the requested CLI metadata output appears.

Bootstrap observations:
Primary untouched; dirty hashes: AGENTS.md 1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd; StorageArchitecture.md 6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22. Branch/worktree/evidence/scratch paths absent before creation. Files backing device /dev/sdb4 ext4 UUID 2283b135-bf37-42d6-921f-2fb861d0a8d4. Initial live status free, metadata null, Editor PIDs []; Primary status exit 2, mount mismatch source /dev/sdc2 vs /dev/sda2. Remote base fetched: a29c2fa480f1077e12434dcf6679f2f5d9465cdc. Full tool output for initial read-only reconnaissance remains in session transcript.

```text
2026-11-15T22:41:30.109022+00:00
$ git --git-dir=/media/jer/Files/CCF/Repository.git for-each-ref '--format=%(refname)' refs/heads/ops/unity-lease-persistent-identity-20261115-v1
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:41:30.611901+00:00
$ git --git-dir=/media/jer/Files/CCF/Repository.git ls-remote --heads origin ops/unity-lease-persistent-identity-20261115-v1
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:41:30.615331+00:00
$ df -h / /media/jer/Files /media/jer/ZX20
exit: 0
stdout:
Filesystem      Size  Used Avail Use% Mounted on
/dev/sda2       234G  198G   25G  90% /
/dev/sdb4       465G  315G  127G  72% /media/jer/Files
/dev/sdc2       477G  426G   52G  90% /media/jer/ZX20
stderr:
```

```text
2026-11-15T22:41:30.733771+00:00
$ git --git-dir=/media/jer/Files/CCF/Repository.git worktree add --no-checkout -b ops/unity-lease-persistent-identity-20261115-v1 /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 origin/ops/workflow-foundation-v1
exit: 0
stdout:
branch 'ops/unity-lease-persistent-identity-20261115-v1' set up to track 'origin/ops/workflow-foundation-v1'.
stderr:
Preparing worktree (new branch 'ops/unity-lease-persistent-identity-20261115-v1')
```

```text
2026-11-15T22:41:30.739136+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 sparse-checkout set --no-cone /AGENTS.md /CCF-Ops/ /Docs/Project/ /Docs/Operations/ /Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:41:30.847416+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 checkout ops/unity-lease-persistent-identity-20261115-v1
exit: 0
stdout:
Your branch is up to date with 'origin/ops/workflow-foundation-v1'.
stderr:
Already on 'ops/unity-lease-persistent-identity-20261115-v1'
```

```text
2026-11-15T22:41:30.860391+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 status --short
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:41:30.864802+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 rev-parse HEAD
exit: 0
stdout:
a29c2fa480f1077e12434dcf6679f2f5d9465cdc
stderr:
```

```text
2026-11-15T22:41:30.870263+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 log -1 --format=%H -- AGENTS.md CLAUDE.md Docs/Project/AgentWorkflow.md Docs/Project/decision-log.md Docs/Project/current-milestone.md Docs/Project/game-brief.md Docs/Project/unity-project-overview.md Docs/Project/research-index.md Docs/Project/instructions Tools/ProjectContext/generate_project_instructions.py
exit: 0
stdout:
46247486b37904f6b36825bb40fb6ed7bea35e88
stderr:
```

```text
2026-11-15T22:41:59.537176+00:00
$ cat /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/AGENTS.md
exit: 0
stdout:
# CCF operations entry point

Follow the task packet and locked CCF context. User → ChatGPT Overall Manager → mechanical execution checks → thin T3 Execution Manager → disposable task workers. This directory supplies operational state and roles; it does not change canonical authority.

Read README.md, the relevant roles/ file, the live board, and the authoritative active-tasks.md before claiming work. Workers are constrained primarily by packets. Never infer acceptance, ownership, or permission from an idle lane. No autonomous T3 thread dispatch. No gameplay, simulation, assets, scenes, packages or settings work without its own explicit packet. AgentWorkflow.md requires later G4 review and is not edited by this task.

All new CCF data, including Git metadata, caches and temporary data, must stay on verified ZX20 (UUID 18D6-896E). Source environment.sh; verify mount identity before writing. Do not migrate/delete existing SSD data. Stop for ambiguous worktree ownership, dirty unknown work, active Unity ownership, low disk or unavoidable SSD writes. No credentials or account configuration. Use the machine-wide unity-lock for every Unity session; never kill processes through operational tooling.
stderr:
```

```text
2026-11-15T22:41:59.539545+00:00
$ cat /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/README.md
exit: 0
stdout:
# CCF-Ops foundation — G4 corrections

Versioned source: CCF repository `CCF-Ops/` on branch `ops/workflow-foundation-v1` in `/media/jer/ZX20/CCF-NonUnity`. Live deployment: `/media/jer/ZX20/CCF-Ops`. The deployment is an explicit copy, not a second repository or a symlink (ZX20 is exFAT). `deployment.json` records source commit and file hashes. Update scripts/roles/config deliberately from a reviewed commit; never overwrite the live board, ledger or state with a blanket recopy. Initial board is a source snapshot; the deployed board is the operational live copy. Reconcile with authoritative active-tasks before dispatch.

Persistent lanes / disposable threads. User → ChatGPT Overall Manager → mechanical checks → thin T3 Execution Manager → disposable workers. T3 does not autonomously create threads. Scope/authority comes from Manager packets. Until a later canonical context update wires role persistence into project entry files, dispatch must supply `ROLE FILE` plus the explicit task packet. The first-message reminder is reinforcement only; the complete role-loading chain is not operational yet. Role files grant no independent scope or acceptance authority.

## Storage and lanes

ZX20 is UUID `18D6-896E`, exFAT, mounted at `/media/jer/ZX20`. Verify it; do not assume `/media/jer/Files` is ZX20. Machine-local lanes.json stores device, UUID, filesystem and a 15 GiB minimum free-space threshold; these identifiers are not canonical workflow policy. Source `environment.sh` in a dedicated task shell to validate device identity, writable (`rw`) mount and sufficient free space and set ZX20_ROOT, TMPDIR/TMP/TEMP and XDG_CACHE_HOME. Scripts use Python standard library, Git, findmnt and du. No subscriptions, credentials or downloads are configured.

- U1: primary Unity; prepared at supplied context, Library warm-up deferred.
- U2: secondary Unity; prepared at supplied context, Library warm-up deferred.
- Integration: clean, single integrator; `unity: false`, no Editor by default; no accepted branch is changed automatically.
- NonUnity: sparse offline tools/tests/docs; do not launch Unity here.

U1/U2 have `unity: true`; Integration/NonUnity have `unity: false`. Below the configured threshold, status reports BLOCK / LOW SPACE and no new substantial lane/cache/build work is allowed.

The ZX20 common repository was changed locally from unset `core.fsync` to `all` using `git --git-dir=/media/jer/ZX20/CCF-Repository.git config --local core.fsync all`. The older store configuration was not changed. This improves Git write durability on exFAT; it is not a backup. In-flight work is not safely recoverable merely because its worktree exists on ZX20. Every handoff requires a pushed remote branch, or an explicitly recorded Git bundle/backup when pushing is impossible; backups also stay on designated external storage.

Each lane has a distinct branch and common Git store on ZX20/CCF-Repository.git. No per-task worktree creation. Reuse lanes only after checking ownership/dirty state. No Library copying. All future Unity caches stay under their ZX20 lane. Builds/staging/archives: CCF-Builds. Large verification receipts, CSVs, captures and profiling: CCF-Verification. Scratch and tool caches: CCF-Temp. Existing SSD and Files-device material is inventory only; migration is separately authorised.

## Dual Git stores and assignment

The older SSD-backed store `/home/jer/CCF/.git` and ZX20 `CCF-Repository.git` coexist. Origin is the exchange boundary. A branch writable/active in either store must not simultaneously be writable in the other. Before assigning a branch, inspect both configured stores with `lane-status --preflight --lane CCF-NonUnity --branch BRANCH`. Conflicting checkout, unreadable store, dirty lane or invalid board means not READY; do not bypass it or switch another owner’s checkout. No old-store migration/deletion is part of these corrections. Detached candidate validation does not share a writable branch.

## Commands

```bash
source /media/jer/ZX20/CCF-Ops/environment.sh
/media/jer/ZX20/CCF-Ops/scripts/lane-status
/media/jer/ZX20/CCF-Ops/scripts/lane-status --json
/media/jer/ZX20/CCF-Ops/scripts/packet-lint packet.md
/media/jer/ZX20/CCF-Ops/scripts/packet-lint --draft draft.md
/media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md
/media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
```

lane-status never fetches, refreshes the Git index, prunes, changes refs or modifies a lane. Normal inventory returns 0 for missing/dirty/conflicted lanes and displays warnings. Storage readiness failure returns 1; explicit assignment `--preflight` also returns 1 for a blocked assignment. `--storage-only` reports actual free bytes/GiB, threshold and PASS/LOW SPACE. Actual command/config/board errors return 2. Disk is approximate `du` allocation (exFAT overhead included); ahead/behind is local refs only, no network fetch. Every row reports backing device and explicit ZX20: YES/NO. Off-storage lanes are reported ZX20: NO with a STOP warning and are not inspected as writable lanes. Config is lanes.json; malformed paths/duplicates/board/mount identity fail clearly. Missing paths are allowed; unexpected repositories are reported without changing them.

packet-lint: PASS=0, WARN=0, FAIL=1, tool error=2. Explicit `--draft` downgrades missing fields to WARN; invalid explicit values still FAIL. Field syntax: `KEY: value`, multiline values, or Markdown heading followed by value. Required: routing, role, ROLE FILE (reference or NONE), context, base, branch, scope, exclusions, ownership, verification tier (1/2/3), time budget, question/acceptance criterion, stop conditions, handoff format. Full SHAs for context/base. Declare `Unity required: Yes/No`; omission FAILS a non-draft packet. Explicit Unity ownership/lane fields also establish Unity mode; generic Lane, HEAD or Worktree fields do not. `No` plus Unity-specific ownership/lane is contradictory. Canonical aliases such as ROUTING, WORKTREE, BASE, STOP AND ASK IF and HANDOFF are accepted. Empty/none/n/a/- placeholders cannot satisfy substantive fields, including acceptance, stops and handoff (ROLE FILE: NONE is explicitly permitted). Unity additionally requires exact HEAD, a single named Unity owner, an explicit Unity lane, allowed phases, Player/native input Yes/No, and stop conditions. The Unity lane must be configured and marked Unity-capable. Duplicate fields/owners fail. This validates structure, not natural-language truth, permission or acceptance. Fixtures in tests/fixtures.

## Machine-wide Unity ownership

The production state path is fixed at `/media/jer/ZX20/CCF-Ops/state/unity`, across all lanes, source checkouts and clones. No environment variable redirects it. Linux flock on a permanent guard serializes record updates; same-directory rename publishes complete metadata. Ownership persists after the CLI exits. A second acquire cannot replace any existing record, including stale/malformed metadata. No automatic process kill/release.

```bash
scripts/unity-lock acquire --owner OPERATOR --task-id TASK   --lane /media/jer/ZX20/CCF-U1 --branch lane/u1   --head FULL_SHA --eta FUTURE_ISO_TIMESTAMP_WITH_TIMEZONE --notes 'authorised gate'
# Add --pid PID only when known and alive. Save the returned lock_id.
scripts/unity-lock release --owner OPERATOR --task-id TASK --lock-id RETURNED_ID
scripts/unity-lock release --force --owner INSPECTING_ACTOR --reason 'documented stale-state inspection' --manager-approval 'REFERENCE_TO_EXPLICIT_OVERALL_MANAGER_APPROVAL'
```

Acquire fails closed unless the configured lane exists, is Unity-capable, belongs to the configured Git store, is clean, and its actual Git branch/HEAD exactly match the supplied values. It also checks both stores for branch checkout conflicts. It never changes the worktree to match metadata. status shows PID liveness/start ticks/boot identity, ETA overdue and actual running Editor PIDs. Unknown PID means manual inspection, not stale. Existing unclaimed Editor PIDs reject acquisition. Normal release needs owner+task+lock ID; force requires prior explicit Overall Manager authorisation, its reference via `--manager-approval`, explicit flag/actor/reason, and appends the complete previous record (or raw malformed record) to history.jsonl **before** deleting the lock. The approval reference is an auditable cooperative declaration, not cryptographic proof or self-authorisation. Never delete the guard/history manually. The lock coordinates cooperative users on this Linux machine; it is not a security boundary, Unity launcher or supervisor. Test override `--test-state-dir` is restricted to ZX20/CCF-Temp and must never be used operationally. Old launchers need a later explicit adoption packet; installing this foundation does not make them lock-aware.

## Post-integration Unity gates

Integration produces the candidate HEAD → Unity Operator reserves a clean, available U1/U2 lane and checks/places that exact HEAD there → verifies branch/HEAD and ownership → acquires the lock → performs only authorised gates → returns receipts tied to the exact candidate HEAD. Integration does not host the Editor by default. Stop on dirty/owned lanes. Prefer a detached candidate HEAD in the Unity lane (`--branch HEAD`) rather than checking out the active integration branch twice; branch-based placement needs a separate, non-conflicting lane branch and explicit packet. No automatic checkout or Unity launch occurs in these scripts. See pilot/unity-gate-example.md.

## Board, ledger and authority transition

board/active.md has only actionable state, one row per live task and all required fields: Task ID, State, Owner, Harness, Model, Quota source, Lane, Branch, HEAD, Subsystem, Locks, Unity required?, Unity queue position, Verification tier, Open gate, Dependency, Next action. Use None/N/A explicitly. IDs unique; duplicate live Lane or Branch assignments are flagged by packet-lint --board and lane-status/preflight (None/N/A placeholders are unassigned). Append-only board-change history is deferred T2 work: there is no reviewed board writer/authority transition yet; adding one here would enlarge this correction task. HEAD exact or TBD only before assignment. States: READY, ACTIVE, WAITING, BLOCKED, VERIFY, INTEGRATE, DONE-PENDING-LEDGER. Unity queue position is an integer for queued work, None otherwise. Do not claim someone else's files. ledger/ stores compact closed summaries. Role/packet rules and mechanical checks do not change gameplay canon.

Current authoritative coordination remains `Docs/Project/coordination/active-tasks.md` on the designated integration/main coordination branch. A board update cannot confer authority until that file/packet locks are reconciled by its authorised maintainer. See ActiveTasksTransition.md and Docs/Operations/AgentWorkflowProposedChanges.md. G4 is required before canonical AgentWorkflow changes.

## Route identity placeholders

Always record `harness / model / quota source`; verify actual availability and ownership at dispatch. Examples supplied by Manager, not installed/availability claims:

- Claude Code / Sonnet 5.5 xhigh / Claude A
- Claude Code / Sonnet 5.5 xhigh / Claude B
- OpenCode / GPT-6.1 Sol UltraFast / OpenAI allowance
- OpenCode / opencode-go:<model> / Go allowance
- Codex / GPT-6.1 Sol / OpenAI allowance

Claude A/B selection is an explicit Manager pool assignment, never silent account rotation. No API keys, OAuth tokens or secrets belong in board/config/ledger/receipts.

## Offline checks / pilot

`python3 -B tests/self-test.py` creates fixtures only under ZX20/CCF-Temp. It tests clean/dirty/missing lanes, config errors, packet PASS/FAIL/WARN, board/ledger schemas, lock acquire/reject/release/stale/force/malformed handling and concurrent acquisition. No Unity launch. Full receipt belongs in CCF-Verification/WorkflowFoundation. `pilot/manager-packet.md` and `pilot/README.md` describe an isolated, simulated Tier-1 chain; tests write its disposable board, handoff and ledger to scratch. No real worker thread or production gate is implied.

Reviewer A/B/C is a per-packet assessment, mapped separately to existing canonical integration decisions; see roles/reviewer.md. Existing task states and ledger verdicts remain unchanged. Runtime updates preserve board, ledger and Unity lock state; no blanket deployment copy is used.
stderr:
```

```text
2026-11-15T22:41:59.542151+00:00
$ cat /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/roles/execution-manager.md
exit: 0
stdout:
# Execution Manager

Inspect the live repository; mechanically preflight packets; maintain board/ledger within assigned single-writer ownership; prepare the next bounded execution packet; detect branch, file, lane and Unity queue conflicts; report READY/BLOCKED with evidence.

The Overall Manager owns decisions and dispatch. T3 is a thin execution surface, not an autonomous thread orchestrator. Persistent lanes outlive disposable worker threads. Use packet-lint and lane-status, inspect active-tasks.md, and reconcile locks before work starts. A mechanical PASS establishes field presence, not semantic approval. Stop rather than silently repair authority/ownership conflicts.

Not authorised: gameplay decisions; arbitrary autonomous T3 thread creation; acceptance status without Manager criteria; overriding branch/file ownership; waiving gates; bypassing Unity lock. Move closed work to ledger only after recording the evidence and actual verdict; retain only actionable live state on the board.

Before branch assignment run lane-status --preflight against both configured Git stores; origin is the exchange boundary. A conflicting writable checkout is BLOCKED, including across stores. Check duplicate live board lanes/branches. Until canonical entry files are updated, dispatch must explicitly supply ROLE FILE plus the task packet; the complete loading chain is not yet operational. At handoff require a pushed branch or an explicitly recorded Git bundle/backup; a ZX20 worktree alone is not recovery.
stderr:
```

```text
2026-11-15T22:41:59.544795+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py /media/jer/ZX20/CCF-Ops/scripts/_ccf.py
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py	2026-11-15 22:41:30.783567670 +0000
+++ /media/jer/ZX20/CCF-Ops/scripts/_ccf.py	2026-11-05 17:16:51.040000000 +0000
@@ -3,9 +3,12 @@
 import os
 from pathlib import Path
 import re
+import stat
 import subprocess

 ROOT = Path('/media/jer/ZX20')
+FILES_ROOT = Path('/media/jer/Files')
+DURABLE_CCF = FILES_ROOT / 'CCF'
 BOARD_FIELDS = ['Task ID', 'State', 'Owner', 'Harness', 'Model', 'Quota source',
                 'Lane', 'Branch', 'HEAD', 'Subsystem', 'Locks', 'Unity required?',
                 'Unity queue position', 'Verification tier', 'Open gate', 'Dependency', 'Next action']
@@ -30,11 +33,44 @@
         raise ValueError(f'path is on another filesystem: {path}')
     return path

+def on_durable_ccf(path):
+    path = Path(path).resolve()
+    root = DURABLE_CCF.resolve(strict=True)
+    if not path.is_relative_to(root):
+        raise ValueError(f'path is outside the Files CCF tree: {path}')
+    return path
+
+def same_device(a, b):
+    """True when two paths name the same block device.
+
+    Kernel names such as /dev/sda2 can change between boots, so the config
+    registers a persistent /dev/disk/by-id path and we compare device numbers.
+    """
+    try:
+        sa, sb = os.stat(a), os.stat(b)
+    except OSError:
+        return False
+    return stat.S_ISBLK(sa.st_mode) and stat.S_ISBLK(sb.st_mode) and sa.st_rdev == sb.st_rdev
+
+
+def visible_uuid_devices(uuid):
+    """Every visible block device carrying this filesystem UUID."""
+    data = json.loads(command(['lsblk','-lJ','-o','PATH,UUID']))
+    return [d['path'] for d in data.get('blockdevices', []) if (d.get('uuid') or '').lower() == uuid.lower()]
+
+
+def require_unique_uuid(uuid):
+    devices = visible_uuid_devices(uuid)
+    if len(devices) != 1:
+        raise ValueError(f'filesystem UUID {uuid} must be visible on exactly one device, found: {devices}')
+
+
 def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/sda2'):
     root = Path(root).resolve(strict=True)
     info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
-    if info['source'] != device or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
+    if not same_device(info['source'], device) or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
         raise ValueError(f'ZX20 mount identity mismatch: {info}')
+    require_unique_uuid(uuid)
     return root


@@ -42,9 +78,11 @@
     """Observe storage; test arguments may only lower the reported free space."""
     root = Path(storage['root']).resolve(strict=True)
     info = mount_info or json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID,OPTIONS']))['filesystems'][0]
-    identity = (info['source'] == storage['device'] and info['target'] == str(root)
+    identity = (same_device(info['source'], storage['device']) and info['target'] == str(root)
                 and info['fstype'] == storage['fstype']
                 and (info['uuid'] or '').lower() == storage['uuid'].lower())
+    if identity and mount_info is None and len(visible_uuid_devices(storage['uuid'])) != 1:
+        identity = False  # duplicate visible UUID: cannot trust which device is ZX20
     stats = os.statvfs(root)
     available = stats.f_bavail * stats.f_frsize
     if free_bytes is not None:
@@ -79,7 +117,13 @@
     # Alternate mounts/lock universes are not silently configurable.
     if root != ROOT:
         raise ValueError('config root must be the verified ZX20 mount')
-    repo = on_storage(cfg['repository'], root)
+    repo_path = Path(cfg['repository']).resolve()
+    if repo_path.is_relative_to(ROOT):
+        repo = on_storage(cfg['repository'], root)
+    elif repo_path.is_relative_to(DURABLE_CCF.resolve()):
+        repo = on_durable_ccf(cfg['repository'])
+    else:
+        raise ValueError(f'repository must be on ZX20 or the Files CCF tree: {repo_path}')
     if not repo.is_dir():
         raise ValueError(f'missing repository: {repo}')
     seen_names, seen_paths = set(), set()
@@ -94,7 +138,12 @@
             raise ValueError('lane path must be absolute')
         # Only the read-only inventory may observe off-storage paths so it can
         # report ZX20: NO; mutation callers retain strict storage rejection.
-        resolved = str(Path(lane['path']).resolve() if permit_off_storage_lanes else on_storage(lane['path'], root))
+        if permit_off_storage_lanes:
+            resolved = str(Path(lane['path']).resolve())
+        elif not lane['unity'] and Path(lane['path']).resolve().is_relative_to(DURABLE_CCF.resolve()):
+            resolved = str(Path(lane['path']).resolve())
+        else:
+            resolved = str(on_storage(lane['path'], root))
         if lane['lane'] in seen_names or resolved in seen_paths:
             raise ValueError('duplicate lane name/path')
         seen_names.add(lane['lane']); seen_paths.add(resolved)
stderr:
```

```text
2026-11-15T22:41:59.547275+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status /media/jer/ZX20/CCF-Ops/scripts/lane-status
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status	2026-11-15 22:41:30.783777096 +0000
+++ /media/jer/ZX20/CCF-Ops/scripts/lane-status	2026-11-05 17:16:43.730000000 +0000
@@ -5,7 +5,7 @@
 import argparse
 import json
 from pathlib import Path
-from _ccf import command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts
+from _ccf import same_device, command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts, DURABLE_CCF

 def main():
     p=argparse.ArgumentParser(description=__doc__)
@@ -28,13 +28,15 @@
         path=Path(item['path']);resolved=path.resolve();exists=path.exists();probe=resolved
         while not probe.exists():probe=probe.parent
         mount=json.loads(command(['findmnt','-J','-T',str(probe),'-o','SOURCE,TARGET,UUID']))['filesystems'][0]
-        zx20=(mount['source']==cfg['storage']['device'] and (mount['uuid'] or '').lower()==cfg['storage']['uuid'].lower() and resolved.is_relative_to(Path(cfg['storage']['root'])) and probe.stat().st_dev==Path(cfg['storage']['root']).stat().st_dev)
+        zx20=(same_device(mount['source'],cfg['storage']['device']) and (mount['uuid'] or '').lower()==cfg['storage']['uuid'].lower() and resolved.is_relative_to(Path(cfg['storage']['root'])) and probe.stat().st_dev==Path(cfg['storage']['root']).stat().st_dev)
         registered=any(Path(c['path']).resolve()==resolved and Path(c['git_dir']).resolve()==Path(cfg['repository']).resolve() for c in checkouts)
         row=dict(lane=item['lane'],path=str(path),resolved_path=str(resolved),backing_device=mount['source'],zx20=zx20,unity_capable=item['unity'],exists=exists,registered_worktree=registered,branch=None,HEAD=None,dirty=None,ahead_behind=None,unity_library=(path/'Library').is_dir(),approx_disk_usage=None,owner=[],conflict_warning=list(duplicates),storage=health,dual_store_conflicts=[])
         active=[t for t in tasks if t['Lane'] in {item['lane'],'CCF-'+item['lane'],str(path)}]
         row['owner']=[t['Owner']+' ('+t['Task ID']+')' for t in active]
         if not health['ready']:row['conflict_warning'].append('BLOCK / '+health['status'])
-        if not zx20:row['conflict_warning'].append('ZX20: NO — STOP; writable-lane inspection skipped')
+        off_durable=(not zx20) and (not item['unity']) and resolved.is_relative_to(DURABLE_CCF.resolve())
+        if not zx20 and item['unity']:row['conflict_warning'].append('ZX20: NO — STOP; writable Unity lane inspection skipped')
+        elif not zx20 and not off_durable:row['conflict_warning'].append('ZX20: NO — STOP; off-storage lane is not in the durable Files CCF tree')
         elif exists:
             row['approx_disk_usage']=command(['du','-sh',str(path)]).split('\t')[0]
             if not (path/'.git').exists():row['conflict_warning'].append('occupied path is not a Git worktree')
stderr:
```

```text
2026-11-15T22:41:59.549579+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json /media/jer/ZX20/CCF-Ops/lanes.json
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json	2026-11-15 22:41:30.782419122 +0000
+++ /media/jer/ZX20/CCF-Ops/lanes.json	2026-11-13 20:27:20.980000000 +0000
@@ -4,13 +4,19 @@
     "root": "/media/jer/ZX20",
     "uuid": "18D6-896E",
     "fstype": "exfat",
-    "device": "/dev/sda2",
+    "device": "/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2",
     "minimum_free_bytes": 16106127360
   },
-  "repository": "/media/jer/ZX20/CCF-Repository.git",
+  "repository": "/media/jer/Files/CCF/Repository.git",
   "board": "board/active.md",
   "lanes": [
     {
+      "lane": "Q10",
+      "path": "/media/jer/ZX20/CCF-Q10",
+      "purpose": "Claim #74 controller SeamTransition verification continuation on the existing assigned lane",
+      "unity": true
+    },
+    {
       "lane": "U1",
       "path": "/media/jer/ZX20/CCF-U1",
       "purpose": "primary Unity; Library warm-up deferred",
@@ -23,6 +29,12 @@
       "unity": true
     },
     {
+      "lane": "U3",
+      "path": "/media/jer/ZX20/CCF-U3",
+      "purpose": "temporary Scenario One non-UI checkpoint qualification; own fresh Library; U2 remains reserved/cold",
+      "unity": true
+    },
+    {
       "lane": "Integration",
       "path": "/media/jer/ZX20/CCF-Integration",
       "purpose": "clean integration; serialized single writer",
@@ -30,19 +42,79 @@
     },
     {
       "lane": "NonUnity",
-      "path": "/media/jer/ZX20/CCF-NonUnity",
-      "purpose": "offline tools/tests/docs; never launch Unity",
+      "path": "/media/jer/Files/CCF/Operations/CCF-NonUnity",
+      "purpose": "offline tools/tests/docs on Files HDD; never launch Unity",
       "unity": false
+    },
+    {
+      "lane": "RackV2-09",
+      "path": "/media/jer/ZX20/CCF-RackV2-09",
+      "purpose": "RACK-V2-09 field-marking Unity lane; registered by #91 for the packet-named worktree",
+      "unity": true
+    },
+    {
+      "lane": "S1Obj",
+      "path": "/media/jer/ZX20/CCF-S1ObjOverflow",
+      "purpose": "#224 Scenario One Objectives-window overflow bounded UI repair; registered by #203 for the dedicated clean worktree",
+      "unity": true
+    },
+    {
+      "lane": "IntroAstra243",
+      "path": "/media/jer/ZX20/CCF-Intro-Astra-20261103",
+      "purpose": "#243 authorised isolated onboarding visual verification",
+      "unity": true
+    },
+    {
+      "lane": "AccessSpineMeasure248",
+      "path": "/media/jer/ZX20/CCF-AccessSpine-Measure-20261103",
+      "purpose": "#248 read-only access-spine geometry + MFM mask measurement; Manager-reserved isolated worktree, registered by #248 preflight",
+      "unity": true
+    },
+    {
+      "lane": "UIQual275",
+      "path": "/media/jer/ZX20/CCF-UIQualification-20261104",
+      "purpose": "#275 focused Tier-2 Unity qualification of the isolated overnight UI staging (same exact commit d3ba4fce); registered by #275 preflight because the machine-wide lease admits ZX20 lanes only",
+      "unity": true
+    },
+    {
+      "lane": "MainInt277",
+      "path": "/media/jer/ZX20/CCF-MainIntegration-20261104",
+      "purpose": "main playtest worktree after user-directed #277/#243 integration; warm Library",
+      "unity": true
+    },
+    {
+      "lane": "WorkbenchTier2Reg287",
+      "path": "/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106",
+      "purpose": "#287 workbench-retirement Tier-2 qualification; real worktree of the canonical store, registered by #312 for the packet-named path",
+      "unity": true
+    },
+    {
+      "lane": "UIQual376",
+      "path": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
+      "purpose": "#376 user-authorised separate exact-source UI qualification",
+      "unity": true
+    },
+    {
+      "lane": "ResearchCopyQual391",
+      "path": "/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual",
+      "purpose": "#391 user-authorised new exact-candidate lane for research-copy V1 Unity qualification",
+      "unity": true
+    },
+    {
+      "lane": "Objective1UX443",
+      "path": "/media/jer/ZX20/CCF-S1-Objective1UX-20261112",
+      "purpose": "#443 Objective 1 first repair (and #438 witness) primary lane; user-chosen Claude substitute for Sol",
+      "unity": true
     }
   ],
   "git_stores": [
     {
-      "name": "ZX20",
-      "git_dir": "/media/jer/ZX20/CCF-Repository.git"
+      "name": "Files-canonical",
+      "git_dir": "/media/jer/Files/CCF/Repository.git"
     },
     {
       "name": "legacy-SSD",
-      "git_dir": "/home/jer/CCF/.git"
+      "git_dir": "/media/jer/Files/CCF-Legacy/.git"
     }
   ]
 }
stderr:
```

```text
2026-11-15T22:41:59.715145+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:42:00.775364+00:00
$ gh issue comment 548 --repo jermur788/CCF --body-file /tmp/opencode/ccf-ops-lease-20261115/running.md
exit: 0
stdout:
https://github.com/jermur788/CCF/issues/548#issuecomment-6902588082
stderr:
```

```text
2026-11-15T22:42:28.210229+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 diff --check
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:42:28.334639+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 diff -- CCF-Ops/scripts/_ccf.py CCF-Ops/scripts/lane-status CCF-Ops/lanes.json
exit: 0
stdout:
diff --git a/CCF-Ops/lanes.json b/CCF-Ops/lanes.json
index 6ad617d5..56090a62 100644
--- a/CCF-Ops/lanes.json
+++ b/CCF-Ops/lanes.json
@@ -4,7 +4,7 @@
     "root": "/media/jer/ZX20",
     "uuid": "18D6-896E",
     "fstype": "exfat",
-    "device": "/dev/sda2",
+    "device": "/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2",
     "minimum_free_bytes": 16106127360
   },
   "repository": "/media/jer/ZX20/CCF-Repository.git",
diff --git a/CCF-Ops/scripts/_ccf.py b/CCF-Ops/scripts/_ccf.py
index ea4c719d..92d57c81 100644
--- a/CCF-Ops/scripts/_ccf.py
+++ b/CCF-Ops/scripts/_ccf.py
@@ -3,6 +3,7 @@ import json
 import os
 from pathlib import Path
 import re
+import stat
 import subprocess

 ROOT = Path('/media/jer/ZX20')
@@ -30,11 +31,37 @@ def on_storage(path, root=ROOT):
         raise ValueError(f'path is on another filesystem: {path}')
     return path

-def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/sda2'):
+def same_device(a, b):
+    """True when two paths name the same block device.
+
+    Kernel names such as /dev/sda2 can change between boots, so the config
+    registers a persistent /dev/disk/by-id path and we compare device numbers.
+    """
+    try:
+        sa, sb = os.stat(a), os.stat(b)
+    except OSError:
+        return False
+    return stat.S_ISBLK(sa.st_mode) and stat.S_ISBLK(sb.st_mode) and sa.st_rdev == sb.st_rdev
+
+
+def visible_uuid_devices(uuid):
+    """Every visible block device carrying this filesystem UUID."""
+    data = json.loads(command(['lsblk','-lJ','-o','PATH,UUID']))
+    return [d['path'] for d in data.get('blockdevices', []) if (d.get('uuid') or '').lower() == uuid.lower()]
+
+
+def require_unique_uuid(uuid):
+    devices = visible_uuid_devices(uuid)
+    if len(devices) != 1:
+        raise ValueError(f'filesystem UUID {uuid} must be visible on exactly one device, found: {devices}')
+
+
+def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2'):
     root = Path(root).resolve(strict=True)
     info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
-    if info['source'] != device or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
+    if not same_device(info['source'], device) or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
         raise ValueError(f'ZX20 mount identity mismatch: {info}')
+    require_unique_uuid(uuid)
     return root


@@ -42,9 +69,11 @@ def storage_health(storage, free_bytes=None, mount_info=None):
     """Observe storage; test arguments may only lower the reported free space."""
     root = Path(storage['root']).resolve(strict=True)
     info = mount_info or json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID,OPTIONS']))['filesystems'][0]
-    identity = (info['source'] == storage['device'] and info['target'] == str(root)
+    identity = (same_device(info['source'], storage['device']) and info['target'] == str(root)
                 and info['fstype'] == storage['fstype']
                 and (info['uuid'] or '').lower() == storage['uuid'].lower())
+    if identity and mount_info is None and len(visible_uuid_devices(storage['uuid'])) != 1:
+        identity = False  # duplicate visible UUID: cannot trust which device is ZX20
     stats = os.statvfs(root)
     available = stats.f_bavail * stats.f_frsize
     if free_bytes is not None:
diff --git a/CCF-Ops/scripts/lane-status b/CCF-Ops/scripts/lane-status
index b219b117..185f40b4 100755
--- a/CCF-Ops/scripts/lane-status
+++ b/CCF-Ops/scripts/lane-status
@@ -5,7 +5,7 @@ sys.dont_write_bytecode = True
 import argparse
 import json
 from pathlib import Path
-from _ccf import command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts
+from _ccf import same_device, command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts

 def main():
     p=argparse.ArgumentParser(description=__doc__)
@@ -28,7 +28,7 @@ def main():
         path=Path(item['path']);resolved=path.resolve();exists=path.exists();probe=resolved
         while not probe.exists():probe=probe.parent
         mount=json.loads(command(['findmnt','-J','-T',str(probe),'-o','SOURCE,TARGET,UUID']))['filesystems'][0]
-        zx20=(mount['source']==cfg['storage']['device'] and (mount['uuid'] or '').lower()==cfg['storage']['uuid'].lower() and resolved.is_relative_to(Path(cfg['storage']['root'])) and probe.stat().st_dev==Path(cfg['storage']['root']).stat().st_dev)
+        zx20=(same_device(mount['source'],cfg['storage']['device']) and (mount['uuid'] or '').lower()==cfg['storage']['uuid'].lower() and resolved.is_relative_to(Path(cfg['storage']['root'])) and probe.stat().st_dev==Path(cfg['storage']['root']).stat().st_dev)
         registered=any(Path(c['path']).resolve()==resolved and Path(c['git_dir']).resolve()==Path(cfg['repository']).resolve() for c in checkouts)
         row=dict(lane=item['lane'],path=str(path),resolved_path=str(resolved),backing_device=mount['source'],zx20=zx20,unity_capable=item['unity'],exists=exists,registered_worktree=registered,branch=None,HEAD=None,dirty=None,ahead_behind=None,unity_library=(path/'Library').is_dir(),approx_disk_usage=None,owner=[],conflict_warning=list(duplicates),storage=health,dual_store_conflicts=[])
         active=[t for t in tasks if t['Lane'] in {item['lane'],'CCF-'+item['lane'],str(path)}]
stderr:
```

```text
2026-11-15T22:42:28.338353+00:00
$ sha256sum /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json
exit: 0
stdout:
429e35ae7b4333f343334e02b30de17843c40a3dca25fe1feacc2f6df4c2ffb3  /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py
a240098779136a0a586956bdbf985624ad79cdd1abb41ad501f3eba380726546  /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status
daf54563c6ddccf7f23fff764093ab44189c3c20e27ab461ce81cf010ea5e2f2  /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json
stderr:
```

```text
2026-11-15T22:42:28.495407+00:00
$ python3 -B -c 'import sys,json;sys.path.insert(0,sys.argv[1]);import _ccf;c=json.load(open(sys.argv[2]));print('"'"'storage_check:'"'"',_ccf.storage_check(**{k:c['"'"'storage'"'"'][k] for k in ['"'"'root'"'"','"'"'uuid'"'"','"'"'fstype'"'"','"'"'device'"'"']}));print(json.dumps(_ccf.storage_health(c['"'"'storage'"'"']),indent=2))' /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json
exit: 0
stdout:
storage_check: /media/jer/ZX20
{
  "status": "PASS",
  "ready": true,
  "backing_device": "/dev/sdc2",
  "writable": true,
  "free_bytes": 55409115136,
  "free_gib": 51.603759765625,
  "minimum_free_bytes": 16106127360,
  "minimum_free_gib": 15.0
}
stderr:
```

```text
2026-11-15T22:42:28.648687+00:00
$ /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: missing repository: /media/jer/ZX20/CCF-Repository.git
```

```text
2026-11-15T22:42:29.013274+00:00
$ python3 -B /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py
exit: 1
stdout:
Scratch: /media/jer/ZX20/CCF-Temp/foundation-test-6p3l0vst
stderr:
Traceback (most recent call last):
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 38, in <module>
    check(cli('packet-lint',f/'valid.md').splitlines()[0]=='PASS','packet valid PASS')
          ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 34, in cli
    def cli(name,*args,code=0): return run([sys.executable,'-B',OPS/'scripts'/name,*args],code)
                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 31, in run
    if p.returncode!=code: raise AssertionError(f'expected {code}, got {p.returncode}: {args}\n{p.stdout}\n{p.stderr}')
                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
AssertionError: expected 0, got 2: ['/usr/bin/python3', '-B', PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/packet-lint'), PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/fixtures/valid.md')]

ERROR: missing repository: /media/jer/ZX20/CCF-Repository.git

```

```text
2026-11-15T22:42:29.155612+00:00
$ /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/packet-lint /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/fixtures/valid.md
exit: 2
stdout:
stderr:
ERROR: missing repository: /media/jer/ZX20/CCF-Repository.git
```

```text
2026-11-15T22:42:29.317978+00:00
$ /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/packet-lint --board /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/board/active.md
exit: 2
stdout:
stderr:
ERROR: missing repository: /media/jer/ZX20/CCF-Repository.git
```

```text
2026-11-15T22:42:48.497918+00:00
$ python3 -B -c 'import sys,json;sys.path.insert(0,sys.argv[1]);import _ccf;c,b=_ccf.config_read(sys.argv[2]);print('"'"'LIVE BOARD:'"'"');[print(json.dumps(r,sort_keys=True)) for r in _ccf.board_read(b)];print('"'"'UNITY LANES:'"'"');[print(json.dumps(x)) for x in c['"'"'lanes'"'"'] if x['"'"'unity'"'"']]' /media/jer/ZX20/CCF-Ops/scripts /media/jer/ZX20/CCF-Ops/lanes.json
exit: 1
stdout:
[Live board content withheld from Git. Original stdout SHA-256 9970dc724b402fde69a6572fb15089b14667208895c17d879275303390dd26ab; see external commands.jsonl at timestamp 2026-11-15T22:42:48.497918+00:00.]
stderr:
Traceback (most recent call last):
  File "<string>", line 1, in <module>
  File "/media/jer/ZX20/CCF-Ops/scripts/_ccf.py", line 180, in board_read
    raise ValueError('Unity required? must be Yes/No')
ValueError: Unity required? must be Yes/No
```

```text
2026-11-15T22:42:56.230295+00:00
$ cat /media/jer/ZX20/CCF-Ops/board/active.md
exit: 0
stdout:
[Live board content withheld from Git. Original stdout SHA-256 1f198f3ad6745e1b083d8d2a3d58259e3bd888b602ac5a7dfdafac300279c28b; see external commands.jsonl at timestamp 2026-11-15T22:42:56.230295+00:00.]
stderr:
```

```text
2026-11-15T22:42:59.062172+00:00
$ git -C /media/jer/ZX20/CCF-U3 status --porcelain=v1 --untracked-files=normal
exit: 0
stdout:
 M .vscode/settings.json
 M Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Berry_Calyx.mat
 M Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Berries.mat
 M Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat
 M Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat
 M Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat
 M Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat
 M Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat
stderr:
```

```text
2026-11-15T22:42:59.066613+00:00
$ git -C /media/jer/ZX20/CCF-U3 rev-parse --abbrev-ref HEAD
exit: 0
stdout:
qualify/scenario-one-annual-review-reset-370-20261110
stderr:
```

```text
2026-11-15T22:42:59.070711+00:00
$ git -C /media/jer/ZX20/CCF-U3 rev-parse HEAD
exit: 0
stdout:
4db486db7a132bd01725442ef33e00f702c4dff9
stderr:
```

```text
2026-11-15T22:42:59.074538+00:00
$ git -C /media/jer/ZX20/CCF-U3 rev-parse --path-format=absolute --git-common-dir
exit: 0
stdout:
/media/jer/Files/CCF/Repository.git
stderr:
```

```text
2026-11-15T22:43:35.594319+00:00
$ python3 -B -c 'from pathlib import Path;import json,subprocess;c=json.load(open('"'"'/media/jer/ZX20/CCF-Ops/lanes.json'"'"'));b=Path('"'"'/media/jer/ZX20/CCF-Ops/board/active.md'"'"').read_text();print('"'"'BOARD TASK/LANE/OWNER (raw, no authority inferred):'"'"');[print('"'"' | '"'"'.join([s[0],s[6],s[2]])) for l in b.splitlines() if l.startswith('"'"'| CCF-'"'"') and len(s:=[x.strip() for x in l.strip('"'"'|'"'"').split('"'"'|'"'"')])==17];print('"'"'CANDIDATE METADATA:'"'"');[(print(x['"'"'lane'"'"'],x['"'"'path'"'"'],subprocess.run(['"'"'git'"'"','"'"'-C'"'"',x['"'"'path'"'"'],'"'"'status'"'"','"'"'--porcelain=v1'"'"'],capture_output=True,text=True).stdout or '"'"'CLEAN'"'"',subprocess.run(['"'"'git'"'"','"'"'-C'"'"',x['"'"'path'"'"'],'"'"'rev-parse'"'"','"'"'--abbrev-ref'"'"','"'"'HEAD'"'"'],capture_output=True,text=True).stdout.strip())) for x in c['"'"'lanes'"'"'] if x['"'"'lane'"'"'] in ['"'"'U1'"'"','"'"'U2'"'"','"'"'WorkbenchTier2Reg287'"'"','"'"'UIQual376'"'"','"'"'UIQual275'"'"','"'"'MainInt277'"'"']]'
exit: 0
stdout:
[Live board content withheld from Git. Original stdout SHA-256 88f474b920aab276fb1504244b9e2b6c07749c248591b9e7aa622c1e41301939; see external commands.jsonl at timestamp 2026-11-15T22:43:35.594319+00:00.]
stderr:
```

```text
2026-11-15T22:44:23.549424+00:00
$ git -C /media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual rev-parse --abbrev-ref HEAD
exit: 0
stdout:
qualify/scenario-one-tagged-text-379-20261111
stderr:
```

```text
2026-11-15T22:44:23.553452+00:00
$ git -C /media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual rev-parse HEAD
exit: 0
stdout:
2aaacad68ed186dbe14531f31ce5bb3a87811441
stderr:
```

```text
2026-11-15T22:44:24.303234+00:00
$ git -C /media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual status --porcelain=v1
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:44:24.460454+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:44:25.617359+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:54:24.460684+00:00 --notes 'User-authorised metadata-only acquire/release proof; no Editor, lane/source/save/prefs writes' --pid 773451 --json
exit: 0
stdout:
{
  "status": "held",
  "metadata": {
    "lock_id": "8d77dccc25ca468baf1a69af92fa45f5",
    "owner": "Codex-Ops548-durability",
    "task_id": "CCF-548-IDENTITY-DURABILITY",
    "lane": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
    "branch": "qualify/scenario-one-tagged-text-379-20261111",
    "head": "2aaacad68ed186dbe14531f31ce5bb3a87811441",
    "pid": 773451,
    "process_identity": {
      "boot_id": "e496e620-b788-49cf-91d7-ab6a3679840d",
      "start_ticks": "13915970"
    },
    "start_timestamp": "2026-11-15T22:44:25.592554+00:00",
    "eta": "2026-11-15T22:54:24.460684+00:00",
    "notes": "User-authorised metadata-only acquire/release proof; no Editor, lane/source/save/prefs writes"
  },
  "inspection": "recorded process is alive",
  "eta_overdue": false,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:44:25.775615+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "held",
  "metadata": {
    "lock_id": "8d77dccc25ca468baf1a69af92fa45f5",
    "owner": "Codex-Ops548-durability",
    "task_id": "CCF-548-IDENTITY-DURABILITY",
    "lane": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
    "branch": "qualify/scenario-one-tagged-text-379-20261111",
    "head": "2aaacad68ed186dbe14531f31ce5bb3a87811441",
    "pid": 773451,
    "process_identity": {
      "boot_id": "e496e620-b788-49cf-91d7-ab6a3679840d",
      "start_ticks": "13915970"
    },
    "start_timestamp": "2026-11-15T22:44:25.592554+00:00",
    "eta": "2026-11-15T22:54:24.460684+00:00",
    "notes": "User-authorised metadata-only acquire/release proof; no Editor, lane/source/save/prefs writes"
  },
  "inspection": "recorded process is alive",
  "eta_overdue": false,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:44:25.936841+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --reason 'Completed metadata-only durability verification; no Editor launched' --json
exit: 0
stdout:
{
  "status": "released",
  "forced": false,
  "previous": {
    "lock_id": "8d77dccc25ca468baf1a69af92fa45f5",
    "owner": "Codex-Ops548-durability",
    "task_id": "CCF-548-IDENTITY-DURABILITY",
    "lane": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
    "branch": "qualify/scenario-one-tagged-text-379-20261111",
    "head": "2aaacad68ed186dbe14531f31ce5bb3a87811441",
    "pid": 773451,
    "process_identity": {
      "boot_id": "e496e620-b788-49cf-91d7-ab6a3679840d",
      "start_ticks": "13915970"
    },
    "start_timestamp": "2026-11-15T22:44:25.592554+00:00",
    "eta": "2026-11-15T22:54:24.460684+00:00",
    "notes": "User-authorised metadata-only acquire/release proof; no Editor, lane/source/save/prefs writes"
  },
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:44:26.096160+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:44:26.856036+00:00
$ git -C /media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual status --porcelain=v1
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:44:58.084512+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.217339+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.350672+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.486637+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.619688+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.780078+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:58.988707+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:44:59.130403+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:44:59.265170+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:44:59.400994+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:59.534180+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:59.668925+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:59.802779+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:44:59.936509+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:00.072503+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:00.204873+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:00.338744+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:00.473006+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:00.610374+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:00.744395+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:00.877497+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:01.014526+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:01.146759+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock acquire --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:01.279427+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock release --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:01.435318+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:45:01.438648+00:00
$ sha256sum /media/jer/ZX20/CCF-Ops/scripts/_ccf.py /media/jer/ZX20/CCF-Ops/scripts/lane-status /media/jer/ZX20/CCF-Ops/lanes.json /media/jer/ZX20/CCF-Ops/deployment.json
exit: 0
stdout:
ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001  /media/jer/ZX20/CCF-Ops/scripts/_ccf.py
e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f  /media/jer/ZX20/CCF-Ops/scripts/lane-status
1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21  /media/jer/ZX20/CCF-Ops/lanes.json
22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed  /media/jer/ZX20/CCF-Ops/deployment.json
stderr:
```

```text
2026-11-15T22:45:29.999382+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.158750+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:29.999633+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.304203+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-uuid/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.440965+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.575042+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:30.441264+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.710170+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-device/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:30.842817+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:30.984035+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:30.843168+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:31.127787+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-root/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:31.273319+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:31.409910+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:31.273552+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:31.545482+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/live-wrong-fstype/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:31.680369+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:31.815808+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:31.680604+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:31.951025+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-uuid/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:32.084654+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:32.217975+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:32.084880+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:32.352362+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-device/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:32.486706+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:32.620544+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:32.487025+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:32.754749+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-root/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdb4', 'target': '/media/jer/Files', 'fstype': 'ext4', 'uuid': '2283b135-bf37-42d6-921f-2fb861d0a8d4'}
```

```text
2026-11-15T22:45:32.889908+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock status --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:33.024400+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock acquire --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lane UIQual376 --branch qualify/scenario-one-tagged-text-379-20261111 --head 2aaacad68ed186dbe14531f31ce5bb3a87811441 --eta 2026-11-15T22:55:32.890198+00:00 --notes 'Negative identity verification only; must reject before lock access' --pid 775399 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:33.158767+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/branch-wrong-fstype/scripts/unity-lock release --owner Codex-Ops548-durability --task-id CCF-548-IDENTITY-DURABILITY --lock-id 8d77dccc25ca468baf1a69af92fa45f5 --json
exit: 2
stdout:
stderr:
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
```

```text
2026-11-15T22:45:33.439857+00:00
$ python3 -B /tmp/opencode/ccf-ops-lease-20261115/verify-identity.py /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json
exit: 0
stdout:
PASS: enumerated source and persistent pin share block device number
PASS: character devices rejected
PASS: regular files rejected even with matching rdev
PASS: absent pin rejected
PASS: exactly one physically visible UUID
PASS: default storage_check uses persistent pin
PASS: physical storage health PASS
PASS: low capacity remains blocked
PASS: read-only remains blocked
PASS: mount target remains bound
PASS: zero/duplicate UUID storage_check rejected: []
PASS: zero/duplicate UUID health blocked
PASS: zero/duplicate UUID storage_check rejected: ['/dev/sdc2', '/dev/CCF-DUPLICATE-FIXTURE']
PASS: zero/duplicate UUID health blocked
PASS: Python parse: _ccf.py
PASS: Python parse: lane-status
PASS: Python parse: unity-lock
PASS: Python parse: packet-lint
IDENTITY_VERIFICATION_PASS 18
stderr:
```

```text
2026-11-15T22:45:33.595940+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
exit: 0
stdout:
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
stderr:
```

```text
2026-11-15T22:45:57.823497+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 diff --check
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:45:57.836030+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 status --short
exit: 0
stdout:
 M CCF-Ops/lanes.json
 M CCF-Ops/scripts/_ccf.py
 M CCF-Ops/scripts/lane-status
stderr:
```

```text
2026-11-15T22:45:58.018223+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 add CCF-Ops/scripts/_ccf.py CCF-Ops/scripts/lane-status CCF-Ops/lanes.json
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:45:58.327028+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 commit -m 'fix(ops): preserve persistent ZX20 block-device identity checks'
exit: 0
stdout:
[ops/unity-lease-persistent-identity-20261115-v1 8483e5b8] fix(ops): preserve persistent ZX20 block-device identity checks
 3 files changed, 35 insertions(+), 6 deletions(-)
stderr:
```

```text
2026-11-15T22:45:58.331420+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 rev-parse HEAD
exit: 0
stdout:
8483e5b844564dcedc47d289c877aed49076cdab
stderr:
```

```text
2026-11-15T22:45:58.352453+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py /media/jer/ZX20/CCF-Ops/scripts/_ccf.py
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/_ccf.py	2026-11-15 22:42:21.985817779 +0000
+++ /media/jer/ZX20/CCF-Ops/scripts/_ccf.py	2026-11-05 17:16:51.040000000 +0000
@@ -7,6 +7,8 @@
 import subprocess

 ROOT = Path('/media/jer/ZX20')
+FILES_ROOT = Path('/media/jer/Files')
+DURABLE_CCF = FILES_ROOT / 'CCF'
 BOARD_FIELDS = ['Task ID', 'State', 'Owner', 'Harness', 'Model', 'Quota source',
                 'Lane', 'Branch', 'HEAD', 'Subsystem', 'Locks', 'Unity required?',
                 'Unity queue position', 'Verification tier', 'Open gate', 'Dependency', 'Next action']
@@ -31,6 +33,13 @@
         raise ValueError(f'path is on another filesystem: {path}')
     return path

+def on_durable_ccf(path):
+    path = Path(path).resolve()
+    root = DURABLE_CCF.resolve(strict=True)
+    if not path.is_relative_to(root):
+        raise ValueError(f'path is outside the Files CCF tree: {path}')
+    return path
+
 def same_device(a, b):
     """True when two paths name the same block device.

@@ -56,7 +65,7 @@
         raise ValueError(f'filesystem UUID {uuid} must be visible on exactly one device, found: {devices}')


-def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2'):
+def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/sda2'):
     root = Path(root).resolve(strict=True)
     info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
     if not same_device(info['source'], device) or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
@@ -108,7 +117,13 @@
     # Alternate mounts/lock universes are not silently configurable.
     if root != ROOT:
         raise ValueError('config root must be the verified ZX20 mount')
-    repo = on_storage(cfg['repository'], root)
+    repo_path = Path(cfg['repository']).resolve()
+    if repo_path.is_relative_to(ROOT):
+        repo = on_storage(cfg['repository'], root)
+    elif repo_path.is_relative_to(DURABLE_CCF.resolve()):
+        repo = on_durable_ccf(cfg['repository'])
+    else:
+        raise ValueError(f'repository must be on ZX20 or the Files CCF tree: {repo_path}')
     if not repo.is_dir():
         raise ValueError(f'missing repository: {repo}')
     seen_names, seen_paths = set(), set()
@@ -123,7 +138,12 @@
             raise ValueError('lane path must be absolute')
         # Only the read-only inventory may observe off-storage paths so it can
         # report ZX20: NO; mutation callers retain strict storage rejection.
-        resolved = str(Path(lane['path']).resolve() if permit_off_storage_lanes else on_storage(lane['path'], root))
+        if permit_off_storage_lanes:
+            resolved = str(Path(lane['path']).resolve())
+        elif not lane['unity'] and Path(lane['path']).resolve().is_relative_to(DURABLE_CCF.resolve()):
+            resolved = str(Path(lane['path']).resolve())
+        else:
+            resolved = str(on_storage(lane['path'], root))
         if lane['lane'] in seen_names or resolved in seen_paths:
             raise ValueError('duplicate lane name/path')
         seen_names.add(lane['lane']); seen_paths.add(resolved)
stderr:
```

```text
2026-11-15T22:45:58.355559+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status /media/jer/ZX20/CCF-Ops/scripts/lane-status
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status	2026-11-15 22:42:22.327482772 +0000
+++ /media/jer/ZX20/CCF-Ops/scripts/lane-status	2026-11-05 17:16:43.730000000 +0000
@@ -5,7 +5,7 @@
 import argparse
 import json
 from pathlib import Path
-from _ccf import same_device, command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts
+from _ccf import same_device, command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts, DURABLE_CCF

 def main():
     p=argparse.ArgumentParser(description=__doc__)
@@ -34,7 +34,9 @@
         active=[t for t in tasks if t['Lane'] in {item['lane'],'CCF-'+item['lane'],str(path)}]
         row['owner']=[t['Owner']+' ('+t['Task ID']+')' for t in active]
         if not health['ready']:row['conflict_warning'].append('BLOCK / '+health['status'])
-        if not zx20:row['conflict_warning'].append('ZX20: NO — STOP; writable-lane inspection skipped')
+        off_durable=(not zx20) and (not item['unity']) and resolved.is_relative_to(DURABLE_CCF.resolve())
+        if not zx20 and item['unity']:row['conflict_warning'].append('ZX20: NO — STOP; writable Unity lane inspection skipped')
+        elif not zx20 and not off_durable:row['conflict_warning'].append('ZX20: NO — STOP; off-storage lane is not in the durable Files CCF tree')
         elif exists:
             row['approx_disk_usage']=command(['du','-sh',str(path)]).split('\t')[0]
             if not (path/'.git').exists():row['conflict_warning'].append('occupied path is not a Git worktree')
stderr:
```

```text
2026-11-15T22:45:58.358276+00:00
$ diff -u /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json /media/jer/ZX20/CCF-Ops/lanes.json
exit: 1
stdout:
--- /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/lanes.json	2026-11-15 22:42:22.630122604 +0000
+++ /media/jer/ZX20/CCF-Ops/lanes.json	2026-11-13 20:27:20.980000000 +0000
@@ -7,10 +7,16 @@
     "device": "/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2",
     "minimum_free_bytes": 16106127360
   },
-  "repository": "/media/jer/ZX20/CCF-Repository.git",
+  "repository": "/media/jer/Files/CCF/Repository.git",
   "board": "board/active.md",
   "lanes": [
     {
+      "lane": "Q10",
+      "path": "/media/jer/ZX20/CCF-Q10",
+      "purpose": "Claim #74 controller SeamTransition verification continuation on the existing assigned lane",
+      "unity": true
+    },
+    {
       "lane": "U1",
       "path": "/media/jer/ZX20/CCF-U1",
       "purpose": "primary Unity; Library warm-up deferred",
@@ -23,6 +29,12 @@
       "unity": true
     },
     {
+      "lane": "U3",
+      "path": "/media/jer/ZX20/CCF-U3",
+      "purpose": "temporary Scenario One non-UI checkpoint qualification; own fresh Library; U2 remains reserved/cold",
+      "unity": true
+    },
+    {
       "lane": "Integration",
       "path": "/media/jer/ZX20/CCF-Integration",
       "purpose": "clean integration; serialized single writer",
@@ -30,19 +42,79 @@
     },
     {
       "lane": "NonUnity",
-      "path": "/media/jer/ZX20/CCF-NonUnity",
-      "purpose": "offline tools/tests/docs; never launch Unity",
+      "path": "/media/jer/Files/CCF/Operations/CCF-NonUnity",
+      "purpose": "offline tools/tests/docs on Files HDD; never launch Unity",
       "unity": false
+    },
+    {
+      "lane": "RackV2-09",
+      "path": "/media/jer/ZX20/CCF-RackV2-09",
+      "purpose": "RACK-V2-09 field-marking Unity lane; registered by #91 for the packet-named worktree",
+      "unity": true
+    },
+    {
+      "lane": "S1Obj",
+      "path": "/media/jer/ZX20/CCF-S1ObjOverflow",
+      "purpose": "#224 Scenario One Objectives-window overflow bounded UI repair; registered by #203 for the dedicated clean worktree",
+      "unity": true
+    },
+    {
+      "lane": "IntroAstra243",
+      "path": "/media/jer/ZX20/CCF-Intro-Astra-20261103",
+      "purpose": "#243 authorised isolated onboarding visual verification",
+      "unity": true
+    },
+    {
+      "lane": "AccessSpineMeasure248",
+      "path": "/media/jer/ZX20/CCF-AccessSpine-Measure-20261103",
+      "purpose": "#248 read-only access-spine geometry + MFM mask measurement; Manager-reserved isolated worktree, registered by #248 preflight",
+      "unity": true
+    },
+    {
+      "lane": "UIQual275",
+      "path": "/media/jer/ZX20/CCF-UIQualification-20261104",
+      "purpose": "#275 focused Tier-2 Unity qualification of the isolated overnight UI staging (same exact commit d3ba4fce); registered by #275 preflight because the machine-wide lease admits ZX20 lanes only",
+      "unity": true
+    },
+    {
+      "lane": "MainInt277",
+      "path": "/media/jer/ZX20/CCF-MainIntegration-20261104",
+      "purpose": "main playtest worktree after user-directed #277/#243 integration; warm Library",
+      "unity": true
+    },
+    {
+      "lane": "WorkbenchTier2Reg287",
+      "path": "/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106",
+      "purpose": "#287 workbench-retirement Tier-2 qualification; real worktree of the canonical store, registered by #312 for the packet-named path",
+      "unity": true
+    },
+    {
+      "lane": "UIQual376",
+      "path": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
+      "purpose": "#376 user-authorised separate exact-source UI qualification",
+      "unity": true
+    },
+    {
+      "lane": "ResearchCopyQual391",
+      "path": "/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual",
+      "purpose": "#391 user-authorised new exact-candidate lane for research-copy V1 Unity qualification",
+      "unity": true
+    },
+    {
+      "lane": "Objective1UX443",
+      "path": "/media/jer/ZX20/CCF-S1-Objective1UX-20261112",
+      "purpose": "#443 Objective 1 first repair (and #438 witness) primary lane; user-chosen Claude substitute for Sol",
+      "unity": true
     }
   ],
   "git_stores": [
     {
-      "name": "ZX20",
-      "git_dir": "/media/jer/ZX20/CCF-Repository.git"
+      "name": "Files-canonical",
+      "git_dir": "/media/jer/Files/CCF/Repository.git"
     },
     {
       "name": "legacy-SSD",
-      "git_dir": "/home/jer/CCF/.git"
+      "git_dir": "/media/jer/Files/CCF-Legacy/.git"
     }
   ]
 }
stderr:
```

```text
2026-11-15T22:45:58.378940+00:00
$ git -C /media/jer/Files/CCF-Primary status --short
exit: 0
stdout:
 M AGENTS.md
 M Docs/Operations/StorageArchitecture.md
stderr:
```

```text
2026-11-15T22:45:58.382008+00:00
$ sha256sum /media/jer/Files/CCF-Primary/AGENTS.md /media/jer/Files/CCF-Primary/Docs/Operations/StorageArchitecture.md
exit: 0
stdout:
1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd  /media/jer/Files/CCF-Primary/AGENTS.md
6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22  /media/jer/Files/CCF-Primary/Docs/Operations/StorageArchitecture.md
stderr:
```

```text
2026-11-15T22:46:24.607316+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 show 8483e5b844564dcedc47d289c877aed49076cdab:CCF-Ops/scripts/_ccf.py
exit: 0
stdout:
"""Small shared parsing/storage checks; Python standard library only."""
import json
import os
from pathlib import Path
import re
import stat
import subprocess

ROOT = Path('/media/jer/ZX20')
BOARD_FIELDS = ['Task ID', 'State', 'Owner', 'Harness', 'Model', 'Quota source',
                'Lane', 'Branch', 'HEAD', 'Subsystem', 'Locks', 'Unity required?',
                'Unity queue position', 'Verification tier', 'Open gate', 'Dependency', 'Next action']
STATES = {'READY','ACTIVE','WAITING','BLOCKED','VERIFY','INTEGRATE','DONE-PENDING-LEDGER'}

def command(args, cwd=None):
    env = dict(os.environ, GIT_OPTIONAL_LOCKS='0', PYTHONDONTWRITEBYTECODE='1')
    p = subprocess.run(args, cwd=cwd, env=env, capture_output=True, text=True)
    if p.returncode:
        raise ValueError(f'{args[0]} failed ({p.returncode}): {p.stderr.strip()}')
    return p.stdout.strip()

def on_storage(path, root=ROOT):
    root = root.resolve(strict=True)
    path = Path(path).resolve()
    if not path.is_relative_to(root):
        raise ValueError(f'path escapes ZX20: {path}')
    parent = path
    while not parent.exists():
        parent = parent.parent
    if os.stat(parent).st_dev != os.stat(root).st_dev:
        raise ValueError(f'path is on another filesystem: {path}')
    return path

def same_device(a, b):
    """True when two paths name the same block device.

    Kernel names such as /dev/sda2 can change between boots, so the config
    registers a persistent /dev/disk/by-id path and we compare device numbers.
    """
    try:
        sa, sb = os.stat(a), os.stat(b)
    except OSError:
        return False
    return stat.S_ISBLK(sa.st_mode) and stat.S_ISBLK(sb.st_mode) and sa.st_rdev == sb.st_rdev


def visible_uuid_devices(uuid):
    """Every visible block device carrying this filesystem UUID."""
    data = json.loads(command(['lsblk','-lJ','-o','PATH,UUID']))
    return [d['path'] for d in data.get('blockdevices', []) if (d.get('uuid') or '').lower() == uuid.lower()]


def require_unique_uuid(uuid):
    devices = visible_uuid_devices(uuid)
    if len(devices) != 1:
        raise ValueError(f'filesystem UUID {uuid} must be visible on exactly one device, found: {devices}')


def storage_check(root=ROOT, uuid='18D6-896E', fstype='exfat', device='/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2'):
    root = Path(root).resolve(strict=True)
    info = json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID']))['filesystems'][0]
    if not same_device(info['source'], device) or info['target'] != str(root) or info['fstype'] != fstype or (info['uuid'] or '').lower() != uuid.lower():
        raise ValueError(f'ZX20 mount identity mismatch: {info}')
    require_unique_uuid(uuid)
    return root


def storage_health(storage, free_bytes=None, mount_info=None):
    """Observe storage; test arguments may only lower the reported free space."""
    root = Path(storage['root']).resolve(strict=True)
    info = mount_info or json.loads(command(['findmnt','-J','-T',str(root),'-o','SOURCE,TARGET,FSTYPE,UUID,OPTIONS']))['filesystems'][0]
    identity = (same_device(info['source'], storage['device']) and info['target'] == str(root)
                and info['fstype'] == storage['fstype']
                and (info['uuid'] or '').lower() == storage['uuid'].lower())
    if identity and mount_info is None and len(visible_uuid_devices(storage['uuid'])) != 1:
        identity = False  # duplicate visible UUID: cannot trust which device is ZX20
    stats = os.statvfs(root)
    available = stats.f_bavail * stats.f_frsize
    if free_bytes is not None:
        if type(free_bytes) is not int or not 0 <= free_bytes <= available:
            raise ValueError('test free bytes must be nonnegative and cannot increase actual capacity')
        available = free_bytes
    threshold = storage['minimum_free_bytes']
    writable = 'rw' in info.get('options','').split(',')
    status = 'PASS' if identity and writable and available >= threshold else 'WRONG DEVICE' if not identity else 'READ ONLY' if not writable else 'LOW SPACE'
    return dict(status=status,ready=status=='PASS',backing_device=info['source'],writable=writable,
                free_bytes=available,free_gib=available/1024**3,minimum_free_bytes=threshold,
                minimum_free_gib=threshold/1024**3)


def require_storage(cfg):
    health = storage_health(cfg['storage'])
    if not health['ready']:
        raise ValueError(f"BLOCK / {health['status']}: free={health['free_bytes']} threshold={health['minimum_free_bytes']}")
    return health

def config_read(path, permit_off_storage_lanes=False):
    path = Path(path)
    cfg = json.loads(path.read_text())
    if not isinstance(cfg,dict) or cfg.get('schema') != 1 or not isinstance(cfg.get('lanes'), list) or not cfg['lanes']:
        raise ValueError('config requires schema 1 and nonempty lanes list')
    s = cfg['storage']
    if not isinstance(s,dict):
        raise ValueError('storage must be an object')
    if type(s.get('minimum_free_bytes')) is not int or s['minimum_free_bytes'] <= 0 or not s.get('device'):
        raise ValueError('storage needs device and positive minimum_free_bytes')
    root = storage_check(s['root'], s['uuid'], s['fstype'], s['device'])
    # Alternate mounts/lock universes are not silently configurable.
    if root != ROOT:
        raise ValueError('config root must be the verified ZX20 mount')
    repo = on_storage(cfg['repository'], root)
    if not repo.is_dir():
        raise ValueError(f'missing repository: {repo}')
    seen_names, seen_paths = set(), set()
    for lane in cfg['lanes']:
        if not isinstance(lane,dict):
            raise ValueError('each lane must be an object')
        if type(lane.get('unity')) is not bool:
            raise ValueError('each lane requires unity: true/false')
        if not isinstance(lane.get('lane'), str) or not lane['lane'].strip() or not lane.get('purpose'):
            raise ValueError('lane needs nonempty lane, purpose and absolute path')
        if not Path(lane['path']).is_absolute():
            raise ValueError('lane path must be absolute')
        # Only the read-only inventory may observe off-storage paths so it can
        # report ZX20: NO; mutation callers retain strict storage rejection.
        resolved = str(Path(lane['path']).resolve() if permit_off_storage_lanes else on_storage(lane['path'], root))
        if lane['lane'] in seen_names or resolved in seen_paths:
            raise ValueError('duplicate lane name/path')
        seen_names.add(lane['lane']); seen_paths.add(resolved)
    stores = cfg.get('git_stores')
    if not isinstance(stores,list) or not stores:
        raise ValueError('config requires nonempty git_stores for branch preflight')
    store_paths = []
    for store in stores:
        if not isinstance(store,dict) or not store.get('name') or not Path(store['git_dir']).is_absolute():
            raise ValueError('store requires name and absolute git_dir')
        store_paths.append(str(Path(store['git_dir']).resolve()))
    if len(set(store_paths)) != len(store_paths) or str(repo.resolve()) not in store_paths:
        raise ValueError('duplicate Git store or configured repository missing from git_stores')
    board = on_storage(path.resolve().parent / cfg['board'], root)
    return cfg, board

def board_read(path):
    lines = [line.strip() for line in Path(path).read_text().splitlines() if line.strip().startswith('|')]
    if len(lines) < 2:
        raise ValueError('board table/header missing')
    split = lambda line: [v.strip() for v in line.strip('|').split('|')]
    headers = split(lines[0])
    if headers != BOARD_FIELDS or len(split(lines[1])) != len(headers):
        raise ValueError('board must use the documented ordered required fields')
    rows, ids = [], set()
    for line in lines[2:]:
        cells = split(line)
        if len(cells) != len(headers) or any(not c for c in cells):
            raise ValueError('malformed board row; use None/N/A for empty values')
        row = dict(zip(headers,cells))
        if row['State'] not in STATES or row['Task ID'] in ids:
            raise ValueError('invalid board state or duplicate ID')
        if row['Unity required?'] not in {'Yes','No'}:
            raise ValueError('Unity required? must be Yes/No')
        if row['HEAD'] != 'TBD' and not re.fullmatch(r'[0-9a-fA-F]{40}',row['HEAD']):
            raise ValueError('board HEAD must be exact SHA or TBD')
        ids.add(row['Task ID']); rows.append(row)
    return rows

def resolve_lane(cfg, label):
    for lane in cfg['lanes']:
        if str(label) in {lane['lane'], 'CCF-'+lane['lane'], lane['path']}:
            return lane
    raise ValueError('lane is not configured: '+str(label))


def store_checkouts(cfg):
    """Both stores are read-only inputs. Registered branch checkouts are potential writers."""
    result = []
    for store in cfg['git_stores']:
        raw = command(['git','--git-dir='+store['git_dir'],'worktree','list','--porcelain'])
        for block in raw.split('\n\n'):
            entry = dict(line.split(' ',1) if ' ' in line else (line,True) for line in block.splitlines())
            if 'worktree' not in entry or 'bare' in entry:
                continue
            result.append(dict(store=store['name'],git_dir=store['git_dir'],path=entry['worktree'],
                               branch=entry.get('branch','').removeprefix('refs/heads/'),
                               head=entry.get('HEAD'),locked=entry.get('locked')))
    return result


def branch_conflicts(checkouts, branch, selected_path):
    return [c for c in checkouts if c['branch'] == branch
            and Path(c['path']).resolve() != Path(selected_path).resolve()]


def board_conflicts(rows, cfg=None):
    warnings = []
    for field in ['Lane','Branch']:
        seen = {}
        for row in rows:
            key = row[field]
            if key.lower() in {'none','n/a','tbd','-'}:
                continue
            if field == 'Lane' and cfg:
                try: key = resolve_lane(cfg,key)['path']
                except ValueError: pass
            if key in seen:
                warnings.append(f"duplicate live {field}: {key} ({seen[key]}, {row['Task ID']})")
            seen[key] = row['Task ID']
    return warnings


def fields_read(text):
    """Colon fields and Markdown headings; no natural-language interpretation."""
    fields, current = {}, None
    for line in text.splitlines():
        if line.strip().startswith('```'):
            continue
        match = re.match(r'^\s*(?:#{1,6}\s+)?([A-Za-z][A-Za-z0-9 /?_-]*):\s*(.*)$',line)
        heading = re.match(r'^#{1,6}\s+([A-Za-z][A-Za-z0-9 /?_-]*)\s*$',line)
        if match or heading:
            key = (match or heading).group(1).strip().lower()
            value = match.group(2).strip() if match else ''
            fields.setdefault(key,[]).append(value)
            current = fields[key]
        elif current is not None and line.strip():
            current[-1] += '\n' + line.strip()
    return fields
stderr:
```

```text
2026-11-15T22:46:24.612954+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 show 8483e5b844564dcedc47d289c877aed49076cdab:CCF-Ops/scripts/lane-status
exit: 0
stdout:
#!/usr/bin/env python3
"""Read-only lane/storage/branch checks; --preflight gates a specific assignment."""
import sys
sys.dont_write_bytecode = True
import argparse
import json
from pathlib import Path
from _ccf import same_device, command, config_read, board_read, board_conflicts, storage_health, resolve_lane, store_checkouts, branch_conflicts

def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--config',type=Path,default=Path(__file__).resolve().parent.parent/'lanes.json')
    p.add_argument('--json',action='store_true');p.add_argument('--storage-only',action='store_true')
    p.add_argument('--preflight',action='store_true');p.add_argument('--lane');p.add_argument('--branch')
    p.add_argument('--test-free-bytes',type=int,help='Lower-only capacity simulation; scratch configs only')
    a=p.parse_args();cfg,board=config_read(a.config,permit_off_storage_lanes=True)
    if a.test_free_bytes is not None and not a.config.resolve().is_relative_to(Path(cfg['storage']['root'])/'CCF-Temp'):
        p.error('capacity simulation is restricted to isolated scratch configs')
    health=storage_health(cfg['storage'],a.test_free_bytes)
    if a.storage_only:
        print(json.dumps(health,indent=2) if a.json else f"{'READY' if health['ready'] else 'BLOCK'} / {health['status']} | device={health['backing_device']} rw={health['writable']} | free={health['free_bytes']} bytes ({health['free_gib']:.2f} GiB) threshold={health['minimum_free_bytes']} bytes ({health['minimum_free_gib']:.2f} GiB)")
        return 0 if health['ready'] else 1
    tasks=board_read(board);duplicates=board_conflicts(tasks,cfg);checkouts=store_checkouts(cfg)
    selected=resolve_lane(cfg,a.lane) if a.preflight and a.lane else None
    if a.preflight and (not selected or not a.branch):p.error('--preflight requires --lane and --branch')
    result=[]
    for item in cfg['lanes']:
        path=Path(item['path']);resolved=path.resolve();exists=path.exists();probe=resolved
        while not probe.exists():probe=probe.parent
        mount=json.loads(command(['findmnt','-J','-T',str(probe),'-o','SOURCE,TARGET,UUID']))['filesystems'][0]
        zx20=(same_device(mount['source'],cfg['storage']['device']) and (mount['uuid'] or '').lower()==cfg['storage']['uuid'].lower() and resolved.is_relative_to(Path(cfg['storage']['root'])) and probe.stat().st_dev==Path(cfg['storage']['root']).stat().st_dev)
        registered=any(Path(c['path']).resolve()==resolved and Path(c['git_dir']).resolve()==Path(cfg['repository']).resolve() for c in checkouts)
        row=dict(lane=item['lane'],path=str(path),resolved_path=str(resolved),backing_device=mount['source'],zx20=zx20,unity_capable=item['unity'],exists=exists,registered_worktree=registered,branch=None,HEAD=None,dirty=None,ahead_behind=None,unity_library=(path/'Library').is_dir(),approx_disk_usage=None,owner=[],conflict_warning=list(duplicates),storage=health,dual_store_conflicts=[])
        active=[t for t in tasks if t['Lane'] in {item['lane'],'CCF-'+item['lane'],str(path)}]
        row['owner']=[t['Owner']+' ('+t['Task ID']+')' for t in active]
        if not health['ready']:row['conflict_warning'].append('BLOCK / '+health['status'])
        if not zx20:row['conflict_warning'].append('ZX20: NO — STOP; writable-lane inspection skipped')
        elif exists:
            row['approx_disk_usage']=command(['du','-sh',str(path)]).split('\t')[0]
            if not (path/'.git').exists():row['conflict_warning'].append('occupied path is not a Git worktree')
            else:
                top=Path(command(['git','-C',str(path),'rev-parse','--show-toplevel'])).resolve()
                common=Path(command(['git','-C',str(path),'rev-parse','--path-format=absolute','--git-common-dir'])).resolve()
                if top!=resolved or common!=Path(cfg['repository']).resolve():row['conflict_warning'].append('worktree belongs to unexpected repository')
                else:
                    row['HEAD']=command(['git','-C',str(path),'rev-parse','HEAD'])
                    row['branch']=command(['git','-C',str(path),'rev-parse','--abbrev-ref','HEAD'])
                    row['dirty']=bool(command(['git','-C',str(path),'status','--porcelain']))
                    if row['dirty']:row['conflict_warning'].append('dirty: inspect ownership before writes')
                    upstream=command(['git','-C',str(path),'for-each-ref','--format=%(upstream)','refs/heads/'+row['branch']])
                    if upstream:row['ahead_behind']=command(['git','-C',str(path),'rev-list','--left-right','--count','HEAD...'+upstream]).split()
                    assignment=a.branch if selected and selected['path']==item['path'] else row['branch']
                    conflicts=branch_conflicts(checkouts,assignment,path);row['dual_store_conflicts']=conflicts
                    if conflicts:row['conflict_warning'].append('BLOCK: branch has another writable checkout: '+assignment)
                    for task in active:
                        if task['Branch']!=row['branch'] or task['HEAD']!=row['HEAD']:row['conflict_warning'].append('board branch/HEAD differs: '+task['Task ID'])
            if not registered:row['conflict_warning'].append('not registered in configured repository')
        else:row['conflict_warning'].append('lane absent; no creation performed')
        result.append(row)
    if a.json:print(json.dumps(result,indent=2))
    else:
        print(f"Storage: {'READY' if health['ready'] else 'BLOCK'} / {health['status']} | free={health['free_bytes']} bytes ({health['free_gib']:.2f} GiB) | threshold={health['minimum_free_bytes']} bytes ({health['minimum_free_gib']:.2f} GiB)")
        for r in result:
            print(f"{r['lane']} | {r['path']} | device={r['backing_device']} | ZX20: {'YES' if r['zx20'] else 'NO'} | unity={r['unity_capable']} exists={r['exists']} registered={r['registered_worktree']}")
            print(f"  branch={r['branch']} HEAD={r['HEAD']} dirty={r['dirty']} ahead/behind={r['ahead_behind']} Library={r['unity_library']} disk~={r['approx_disk_usage']} owner={', '.join(r['owner']) or 'unassigned'}")
            print('  warnings: '+('; '.join(r['conflict_warning']) or 'none'))
    if a.preflight:
        target=next(r for r in result if r['path']==selected['path'])
        ready=health['ready'] and target['zx20'] and target['exists'] and target['registered_worktree'] and not target['conflict_warning']
        if not a.json:print('READY' if ready else 'BLOCKED: resolve assignment conflicts before dispatch')
        return 0 if ready else 1
    return 0 if health['ready'] else 1
if __name__=='__main__':
    try:sys.exit(main())
    except (ValueError,KeyError,TypeError,OSError) as e:print('ERROR: '+str(e),file=sys.stderr);sys.exit(2)
stderr:
```

```text
2026-11-15T22:46:24.618771+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 show 8483e5b844564dcedc47d289c877aed49076cdab:CCF-Ops/lanes.json
exit: 0
stdout:
{
  "schema": 1,
  "storage": {
    "root": "/media/jer/ZX20",
    "uuid": "18D6-896E",
    "fstype": "exfat",
    "device": "/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2",
    "minimum_free_bytes": 16106127360
  },
  "repository": "/media/jer/ZX20/CCF-Repository.git",
  "board": "board/active.md",
  "lanes": [
    {
      "lane": "U1",
      "path": "/media/jer/ZX20/CCF-U1",
      "purpose": "primary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "U2",
      "path": "/media/jer/ZX20/CCF-U2",
      "purpose": "secondary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "Integration",
      "path": "/media/jer/ZX20/CCF-Integration",
      "purpose": "clean integration; serialized single writer",
      "unity": false
    },
    {
      "lane": "NonUnity",
      "path": "/media/jer/ZX20/CCF-NonUnity",
      "purpose": "offline tools/tests/docs; never launch Unity",
      "unity": false
    }
  ],
  "git_stores": [
    {
      "name": "ZX20",
      "git_dir": "/media/jer/ZX20/CCF-Repository.git"
    },
    {
      "name": "legacy-SSD",
      "git_dir": "/home/jer/CCF/.git"
    }
  ]
}
stderr:
```

```text
2026-11-15T22:46:24.726269+00:00
$ python3 -B -c 'from pathlib import Path;import json;print('"'"'Unity check script launch references:'"'"');[print(p.name,[(i,l.strip()) for i,l in enumerate(p.read_text().splitlines(),1) if '"'"'subprocess'"'"' in l or '"'"'Popen'"'"' in l or '"'"'Unity/'"'"' in l]) for p in (Path('"'"'/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts'"'"')).iterdir() if p.is_file()]'
exit: 0
stdout:
Unity check script launch references:
_ccf.py [(7, 'import subprocess'), (17, 'p = subprocess.run(args, cwd=cwd, env=env, capture_output=True, text=True)')]
lane-status []
unity-lock []
packet-lint []
stderr:
```

```text
2026-11-15T22:46:24.883864+00:00
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock status
exit: 0
stdout:
Unity lock: free
  metadata: None
  unity_editor_pids: []
stderr:
```

```text
2026-11-15T22:46:24.887813+00:00
$ findmnt -J -T /media/jer/ZX20 -o SOURCE,TARGET,FSTYPE,UUID,OPTIONS
exit: 0
stdout:
{
   "filesystems": [
      {
         "source": "/dev/sdc2",
         "target": "/media/jer/ZX20",
         "fstype": "exfat",
         "uuid": "18D6-896E",
         "options": "rw,relatime,uid=1000,gid=1000,fmask=0022,dmask=0022,iocharset=utf8,errors=remount-ro"
      }
   ]
}
stderr:
```

```text
2026-11-15T22:46:24.889953+00:00
$ readlink -f /dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2
exit: 0
stdout:
/dev/sdc2
stderr:
```

```text
2026-11-15T22:46:24.899794+00:00
$ lsblk -lJ -o PATH,UUID
exit: 0
stdout:
{
   "blockdevices": [
      {
         "path": "/dev/sda",
         "uuid": null
      },{
         "path": "/dev/sda1",
         "uuid": "7753-0B64"
      },{
         "path": "/dev/sda2",
         "uuid": "d393b49a-24aa-4f88-b8c4-5121b96c240d"
      },{
         "path": "/dev/sdb",
         "uuid": null
      },{
         "path": "/dev/sdb1",
         "uuid": "60e276b9-4abb-4325-a913-f4c43c14294b"
      },{
         "path": "/dev/sdb2",
         "uuid": "F624-7937"
      },{
         "path": "/dev/sdb3",
         "uuid": "28cc8d37-1817-4993-ae4e-0599a2983a43"
      },{
         "path": "/dev/sdb4",
         "uuid": "2283b135-bf37-42d6-921f-2fb861d0a8d4"
      },{
         "path": "/dev/sdc",
         "uuid": null
      },{
         "path": "/dev/sdc1",
         "uuid": null
      },{
         "path": "/dev/sdc2",
         "uuid": "18D6-896E"
      },{
         "path": "/dev/sdd",
         "uuid": null
      },{
         "path": "/dev/sr0",
         "uuid": null
      }
   ]
}
stderr:
```

```text
2026-11-15T22:46:24.906525+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 diff --check
exit: 0
stdout:
stderr:
```

```text
2026-11-15T22:46:24.914008+00:00
$ git -C /media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115 diff a29c2fa480f1077e12434dcf6679f2f5d9465cdc --name-only
exit: 0
stdout:
CCF-Ops/lanes.json
CCF-Ops/scripts/_ccf.py
CCF-Ops/scripts/lane-status
stderr:
```

Display note: trailing whitespace is trimmed in this Markdown view; unmodified stdout/stderr remain in the external JSONL record. Exact patch text is preserved in the linked JSON diff artifacts.
