# PROPOSED merged installed CCF-Ops README — v2 (NOT DEPLOYED)

**STATUS: PROPOSAL ONLY. NOT DEPLOYED. NOT INSTALLED. NOT REVIEWED.** Nothing here has been written to
`/media/jer/ZX20/CCF-Ops`. The installed README today is the unchanged live file
(sha256 `249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517`).

**Provenance.** v2 is subtask A's proposed merged README bytes (commit `e25edd7a`, attachment
`ccfbf11f3a2cf3d3be00a011a1d36b205b53f12071d7ea9e7638c79633e20fe2`, pure bytes
`bde3c6cf286b79685d51e5a4f2882fede49a57cc3475d1b6d8f99320ccdd8881`) with **exactly one** orchestrator
repair applied. A's v1 attachment and its evidence documents are left byte-untouched as the author's own
artefacts at their original hashes.

**Repair R-1 (orchestrator, #741, in-scope).** Manager #731 §2 item 5 requires the merged README to carry
the D-091 *Editor PID* instructions, and #652 preflight §7 records the concrete hazard that a recorded
launcher or terminal shell PID blocks normal release. The merged v1 carried only "`Add --pid PID only
when known and alive`". R-1 replaces that single comment line with the Editor-PID identity rule. No other
byte changed: a reviewer can diff the two pure-byte streams to confirm one replaced line.

**How to read this file.** The proposed README bytes are everything after the marker line below, to end of file.
Extraction (byte-exact):

```bash
sed '1,/^<!-- BEGIN-PROPOSED-README-BYTES -->$/d' Sen-1-741-A2-MERGED-INSTALLED-README-v2-ORCHESTRATOR-REPAIR.proposed-NOT-DEPLOYED.md | sha256sum
# expected: 3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123
```

<!-- BEGIN-PROPOSED-README-BYTES -->
# CCF-Ops foundation — G4 corrections

Versioned source: CCF repository `CCF-Ops/` (source worktree `/media/jer/Files/CCF/Operations/CCF-NonUnity`; verify its branch/HEAD against `deployment.json` before redeploying). Live deployment: `/media/jer/ZX20/CCF-Ops`. The deployment is an explicit copy, not a second repository or a symlink (ZX20 is exFAT). `deployment.json` records source commit and file hashes. Update scripts/roles/config deliberately from a reviewed commit; never overwrite the live board, ledger or state with a blanket recopy. Initial board is a source snapshot; the deployed board is the operational live copy. Reconcile with authoritative active-tasks before dispatch.

Persistent lanes / disposable threads. User → ChatGPT Overall Manager → mechanical checks → thin T3 Execution Manager → disposable workers. T3 does not autonomously create threads. Scope/authority comes from Manager packets. Until a later canonical context update wires role persistence into project entry files, dispatch must supply `ROLE FILE` plus the explicit task packet. The first-message reminder is reinforcement only; the complete role-loading chain is not operational yet. Role files grant no independent scope or acceptance authority.

## Storage and lanes

ZX20 is UUID `18D6-896E`, exFAT, mounted at `/media/jer/ZX20`. Verify it; do not assume `/media/jer/Files` is ZX20. Machine-local lanes.json stores a persistent /dev/disk/by-id partition path (compared by block-device identity, because kernel names like /dev/sdX change between boots), UUID, filesystem and a 15 GiB minimum free-space threshold; these identifiers are not canonical workflow policy. Source `environment.sh` in a dedicated task shell to validate device identity, unique visible UUID (duplicates are rejected), writable (`rw`) mount and sufficient free space and set ZX20_ROOT, TMPDIR/TMP/TEMP and XDG_CACHE_HOME. Scripts use Python standard library, Git, findmnt and du. No subscriptions, credentials or downloads are configured.

- U1: primary Unity; prepared at supplied context, Library warm-up deferred.
- U2: secondary Unity; prepared at supplied context, Library warm-up deferred.
- Integration: clean, single integrator; `unity: false`, no Editor by default; no accepted branch is changed automatically.
- NonUnity: sparse offline tools/tests/docs; do not launch Unity here.

U1/U2 have `unity: true`; Integration/NonUnity have `unity: false`. Below the configured threshold, status reports BLOCK / LOW SPACE and no new substantial lane/cache/build work is allowed.

The canonical common repository now lives at `/media/jer/Files/CCF/Repository.git` (moved 2026-10-21); its local `core.fsync=all` setting was preserved. The earlier ZX20 store was removed after verified cutover. The store is not a backup. In-flight work is not safely recoverable merely because its worktree exists on ZX20. Every handoff requires a pushed remote branch, or an explicitly recorded Git bundle/backup when pushing is impossible; backups also stay on designated external storage.

Each lane has a distinct branch and shares `/media/jer/Files/CCF/Repository.git`. Unity lanes and their Library/cache stay on ZX20; the non-Unity lane is on the Files HDD. No per-task worktree creation. Reuse lanes only after checking ownership/dirty state. No Library copying. Durable builds/archives: `/media/jer/Files/CCF/Builds` and `Archives`. Verification receipts, CSVs, captures and profiling: `/media/jer/Files/CCF/Verification`. Scratch and tool caches: `/media/jer/ZX20/CCF-Temp`. See `CCF-Primary/Docs/Operations/StorageArchitecture.md`.

## Dual Git stores and assignment

The canonical Files store `/media/jer/Files/CCF/Repository.git` and the legacy `/media/jer/Files/CCF-Legacy/.git` store coexist. Origin is the exchange boundary. A branch writable/active in either store must not simultaneously be writable in the other. Before assigning a branch, inspect both configured stores with `lane-status --preflight --lane CCF-NonUnity --branch BRANCH`. Conflicting checkout, unreadable store, dirty lane or invalid board means not READY; do not bypass it or switch another owner’s checkout. The old ZX20 store was migrated in the 2026-10-21 storage task; the legacy `/media/jer/Files/CCF-Legacy/.git` store remains for historical checkouts. Detached candidate validation does not share a writable branch.

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

lane-status never fetches, refreshes the Git index, prunes, changes refs or modifies a lane. Normal inventory returns 0 for missing/dirty/conflicted lanes and displays warnings. Storage readiness failure returns 1; explicit assignment `--preflight` also returns 1 for a blocked assignment. `--storage-only` reports actual free bytes/GiB, threshold and PASS/LOW SPACE. Actual command/config/board errors return 2. Disk is approximate `du` allocation (exFAT overhead included); ahead/behind is local refs only, no network fetch. Every row reports backing device and explicit ZX20: YES/NO. Off-storage non-Unity lanes are allowed on the Files CCF tree and are inspected read-only; an off-storage Unity lane is reported ZX20: NO with a STOP warning. Configuration rejects arbitrary roots (including /), sibling paths and resolved symlink escapes outside ZX20 or the bounded Files CCF tree with exit 2 before inventory. Default configuration readers admit registered, resolved bounded Files CCF lanes only when unity:false (owner-approved Option A). Inventory reports ZX20: NO and retains repository identity checks; this grants no writable or Unity capability. NonUnity acquire rejects before creating state or a guard. Unity lanes and explicit writable preflight remain ZX20-only. Config is lanes.json; malformed paths/duplicates/board/mount identity fail clearly. Missing paths are allowed; unexpected repositories are reported without changing them.

packet-lint: PASS=0, WARN=0, FAIL=1, tool error=2. Explicit `--draft` downgrades missing fields to WARN; invalid explicit values still FAIL. Field syntax: `KEY: value`, multiline values, or Markdown heading followed by value. Required: routing, role, ROLE FILE (reference or NONE), context, base, branch, scope, exclusions, ownership, verification tier (1/2/3), time budget, question/acceptance criterion, stop conditions, handoff format. Full SHAs for context/base. Declare `Unity required: Yes/No`; omission FAILS a non-draft packet. Explicit Unity ownership/lane fields also establish Unity mode; generic Lane, HEAD or Worktree fields do not. `No` plus Unity-specific ownership/lane is contradictory. Canonical aliases such as ROUTING, WORKTREE, BASE, STOP AND ASK IF and HANDOFF are accepted. Empty/none/n/a/- placeholders cannot satisfy substantive fields, including acceptance, stops and handoff (ROLE FILE: NONE is explicitly permitted). Unity additionally requires exact HEAD, a single named Unity owner, an explicit Unity lane, allowed phases, Player/native input Yes/No, and stop conditions. The Unity lane must be configured and marked Unity-capable. Duplicate fields/owners fail. This validates structure, not natural-language truth, permission or acceptance. Fixtures in tests/fixtures.

## Machine-wide Unity ownership

The production state path is fixed at `/media/jer/ZX20/CCF-Ops/state/unity`, across all lanes, source checkouts and clones. No environment variable redirects it. Linux flock on a permanent guard serializes record updates; same-directory rename publishes complete metadata. Ownership persists after the CLI exits. A second acquire cannot replace any existing record, including stale/malformed metadata. No automatic process kill/release.

```bash
scripts/unity-lock acquire --owner OPERATOR --task-id TASK   --lane /media/jer/ZX20/CCF-U1 --branch lane/u1   --head FULL_SHA --no-expiry --notes 'authorised gate'
# Add --pid PID only when known and alive: record the actual Unity Editor PID, never a launcher
# or terminal shell PID (a long-lived shell PID blocks normal release). Save the returned lock_id.
scripts/unity-lock release --owner OPERATOR --task-id TASK --lock-id RETURNED_ID
scripts/unity-lock release --force --owner INSPECTING_ACTOR --reason 'documented stale-state inspection' --manager-approval 'REFERENCE_TO_EXPLICIT_OVERALL_MANAGER_APPROVAL'
```

Acquire fails closed unless the configured lane exists, is Unity-capable, belongs to the configured Git store, is clean, and its actual Git branch/HEAD exactly match the supplied values. It also checks both stores for branch checkout conflicts. It never changes the worktree to match metadata. status shows PID liveness/start ticks/boot identity, ETA overdue and actual running Editor PIDs. Unknown PID means manual inspection, not stale. Existing unclaimed Editor PIDs reject acquisition. Normal release needs owner+task+lock ID and refuses while any Unity Editor or the recorded PID still exists, including a reused PID; inspect and verify safe process exit first. With an unknown PID the operator must additionally verify that the worker/session has stopped writing. Force requires prior explicit Overall Manager authorisation, its reference via `--manager-approval`, explicit flag/actor/reason, and appends the complete previous record (or raw malformed record) to history.jsonl **before** deleting the lock. The approval reference is an auditable cooperative declaration, not cryptographic proof or self-authorisation. Never delete the guard/history manually. The lock coordinates cooperative users on this Linux machine; it is not a security boundary, Unity launcher or supervisor. Test override `--test-state-dir` is restricted to ZX20/CCF-Temp and must never be used operationally. Old launchers need a later explicit adoption packet; installing this foundation does not make them lock-aware.

Owner D-091 custody has no elapsed-time cutoff. Acquisition must explicitly choose `--no-expiry` or the mutually exclusive legacy `--eta FUTURE_ISO_TIMESTAMP_WITH_TIMEZONE`. The new mode writes `eta:null` with `no_expiry:true`; an unmarked null, missing ETA or inconsistent mode is malformed and remains blocking. Legacy timestamp records remain readable without migration and an overdue estimate is advisory only. Neither mode expires, kills a process, reclaims a stale heartbeat or transfers custody automatically. An unresponsive worker remains owned until investigated and explicitly handed off or safely released; do not manufacture a far-future timestamp.

Safe handoff is event-driven: record the actual worker/session, scope, lane, branch/HEAD and source status; confirm no worker can continue writing and the Editor/recorded process has exited; preserve evidence and release with matching identity. A custodian reconciles assignment fields under the existing board-directory flock. `HANDED-OFF`/`RELEASED` belong in receipts/ledger, not as new board states: a `DONE-PENDING-LEDGER` row still reserves its Lane/Branch until deliberately cleared or moved to the ledger. Retaining a worktree confers no writer permission. Never migrate live held lock JSON in place. Old scripts cannot read the new marked null record: rollback requires safe explicit release first or an independently reviewed compatible fallback, never deleting ownership to make rollback pass.

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

`python3 -B tests/self-test.py` creates fixtures only under ZX20/CCF-Temp. It tests clean/dirty/missing lanes, config errors, packet PASS/FAIL/WARN, board/ledger schemas, lock acquire/reject/release/stale/force/malformed handling and concurrent acquisition. No Unity launch. Full receipt belongs in `/media/jer/Files/CCF/Verification/WorkflowFoundation`. `pilot/manager-packet.md` and `pilot/README.md` describe an isolated, simulated Tier-1 chain; tests write its disposable board, handoff and ledger to scratch. No real worker thread or production gate is implied.

Reviewer A/B/C is a per-packet assessment, mapped separately to existing canonical integration decisions; see roles/reviewer.md. Existing task states and ledger verdicts remain unchanged. Runtime updates preserve board, ledger and Unity lock state; no blanket deployment copy is used.

## Installed deployment projection

This README is a deployment-only document for the installed `/media/jer/ZX20/CCF-Ops`. It is not the reviewed source `CCF-Ops/README.md` and must not be copied back over it. The installed provenance authority is `deployment.json`.

The source-only optional document `DeploymentProvenance.md` is not installed in this five-file projection (the five replaced files are `scripts/unity-lock`, `scripts/_ccf.py`, `tests/self-test.py`, `roles/unity-operator.md` and this README); do not look for it here. `scripts/verify-deployment` is likewise source-only and not installed: run it from the reviewed source worktree against the installed `deployment.json`, never from installed Ops. `environment.sh` and `AGENTS.md` are deliberate installed-only overrides retained as-is; the source copies must not replace them. Never copy the source `deployment.json`, `lanes.json`, `board/`, `ledger/` or `state/` over the installed copies.

Option A authority: the owner-approved `Sen-1-652-OWNER-APPROVED-OPTION-A-NONUNITY-DEFAULT-ADMISSION-AMENDMENT-20261116.md` supersedes only the historical phrase “default callers remain ZX20-only” in the source-only `DeploymentProvenance.md`, which is retained unchanged in source. Unity admission, physical lock state and writable preflight remain verified ZX20-only.
